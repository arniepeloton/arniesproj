Imports Aspose.Cells
Imports System.Threading.Tasks
Imports Stimulsoft.Report.Components
Imports System.IO
Imports WebProj.WebProj.Data

Public Class TransInvoice

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Public Const SkipNumberReportMinDate As Date = #11/1/2015#
#If DEBUG Then
    Public Const FreeUserMaxInvoiceCount As Integer = 5
#Else
    Public Const FreeUserMaxInvoiceCount As Integer = 200
#End If

    Public Const gIdChi As String = "4092334E-CCD7-47B0-B842-738F80CFBA81"
    Public Const gIdDr As String = "CF3104FA-7F23-48A9-A639-963B38E07AFC"
    Public Const gIdTr As String = "C5933A80-DB20-4B88-ACE7-646AB2AE3BAF"
    Public Const gIdCs As String = "A8B3ED0C-977E-4950-A8D2-77B1B8E73BDB"
    Public CurrentOrgID As String

    Public Sw As New Stopwatch

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public ReadOnly Property Parent() As Business
        Get

            Return mParent

        End Get
    End Property

    Public Function NotYetInvoiced() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT TOP 100 PERCENT " &
            "TransStockSortingListItems.StockSortingListID AS ID," &
            "RIGHT('0000000' + CONVERT(varchar, TransSalesOrder.OrderNo), 7) AS [Sales Order No.]," &
            "TransSalesOrder.OrderDate AS [Sales Order Date]," &
            "PONum AS [PO Number], PODate AS PODate," &
            "LibraryCustomers.CompanyName AS CustomerName," &
            "ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name, '') + COALESCE(LibraryProvinces.Name + LibraryCities.Name, LibraryLocations.LocationName, '') AS [Delivery Address], " &
            "TransSalesOrder.DeliveryDate AS [Delivery Date]," &
            "RIGHT('0000000' + CONVERT(varchar, TransStockSortingList.SortingListNo), 7) AS [Sorting List No.]," &
            "TransStockSortingList.SortingListDate AS [Sorting List Date]," &
            "RIGHT('0000000' + CONVERT(varchar,TransStockPickingList.PickingListNo), 7) AS [Picking List No.]," &
            "TransStockPickingList.PickingListDate AS [Picking List Date]," &
            "TransStockPickingList.IsRushPrint AS [Rush Order]"

        pSql &= ",LibraryCustomers.ID AS CustomerID" &
            ",TransSalesOrder.CustomerAddressID AS BillingAddressID" &
            ",TransSalesOrder.DeliverToID AS DeliveryAddressID" &
            ",TransSalesOrder.DeliveryDate" &
            ",TransSalesOrder.IsTaxable," &
            ",TransSalesOrder.IsFromBackOrder," &
            ",TransSalesOrder.Remarks " &
            ",TransSalesOrder.PONum" &
            ",TransSalesOrder.ID AS SalesOrderID"

        pSql &= " FROM TransStockSortingListItems INNER JOIN " &
            " TransStockSortingList ON TransStockSortingListItems.StockSortingListID = TransStockSortingList.ID INNER JOIN " &
            " TransSalesOrder ON TransStockSortingListItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " &
            " LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID INNER JOIN " &
            " TransStockPickingList ON TransStockSortingList.StockPickingListID = TransStockPickingList.ID" &
            " LEFT JOIN LibraryCustomersAddresses ON TransSalesOrder.DeliverToID = LibraryCustomersAddresses.ID" &
            " LEFT JOIN LibraryLocations ON LibraryCustomersAddresses.LocationID = LibraryLocations.ID "

        pSql &= " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" &
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" &
            " LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" &
            " LEFT JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID"

        pSql &= " WHERE TransStockSortingListItems.IsInvoiced=0" &
            " AND TransSalesOrder.IsCancelled = 0" &
            " ORDER BY RIGHT('0000000' + CONVERT(varchar, TransSalesOrder.OrderNo), 7) ASC," &
            " RIGHT('0000000' + CONVERT(varchar, TransStockSortingList.SortingListNo), 7) ASC "

        pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDefaultId"></param>
    ''' <param name="vExtention">InvoiceNoExtension</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function NextInvoiceNumber(ByVal vDefaultId As String,
        Optional ByVal vExtention As String = "") As Integer

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT Max(InvoiceNumber)" &
            " FROM TransInvoice" &
            " WITH (TABLOCKX)" &
            " WHERE InvoiceDefaultID = " & ReadyString(vDefaultId) &
            " AND InvoiceNoExtension = " & ReadyString(vExtention)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CDec2(pDT.Rows(0)(0)) + 1
        Else
            Return 1
        End If

    End Function

    Public Function ById(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoice.CustomerID" & vbCrLf &
            ",TransInvoice.InvoiceNumber" & vbCrLf &
            ",TransInvoice.RefNum" & vbCrLf &
            ",TransInvoice.InvoiceDate" & vbCrLf &
            ",TransInvoice.FowstID" & vbCrLf &
            ",TransInvoice.TotalAmount" & vbCrLf &
            ",TransInvoice.PhNatTeamDiscount" & vbCrLf &
            ",TransInvoice.SoloParentDiscount" & vbCrLf &
            ",TransInvoice.SCDiscount" & vbCrLf &
            ",TransInvoice.SCDiscountExempt" & vbCrLf &
            ",TransInvoice.ForegoneVAT" & vbCrLf &
            ",TransInvoice.EncodingFowstID" & vbCrLf &
            ",TransInvoice.BillingAddressID" & vbCrLf &
            ",TransInvoice.DeliveryAddressID" & vbCrLf &
            ",TransInvoice.DateNeeded" & vbCrLf &
            ",TransInvoice.PrintCount" & vbCrLf &
            ",TransInvoice.WithdrawalSlipPrintCount" & vbCrLf &
            ",TransInvoice.PickListPrintCount" & vbCrLf &
            ",TransInvoice.SalesOrderNo" & vbCrLf &
            ",TransInvoice.PurchaseOrderNo" & vbCrLf &
            ",TransInvoice.QuotationNo" & vbCrLf &
            ",TransInvoice.TransType" & vbCrLf &
            ",TransInvoice.SalesOrderID" & vbCrLf &
            ",TransInvoice.DocumentNameID" & vbCrLf &
            ",TransInvoice.IsCancelled" & vbCrLf &
            ",TransInvoice.UserID" & vbCrLf &
            ",TransInvoice.CncUnapprovedReason" & vbCrLf &
            ",TransInvoice.CncApprovedByID" & vbCrLf &
            ",TransInvoice.AccountsPayableNum" & vbCrLf &
            ",TransInvoice.FinalizedForApproval" & vbCrLf &
            ",TransInvoice.DeviceID" & vbCrLf &
            ",TransInvoice.FowstID" & vbCrLf &
            ",TransInvoice.DocumentNameID" & vbCrLf &
            ",TransInvoice.UnapprovedProfitability" & vbCrLf &
            ",TransInvoice.AmountDueBeforeCharges" & vbCrLf &
            ",LibraryDocumentControl.DocumentName" & vbCrLf &
            " FROM TransInvoice" &
            " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetItemsTotal(vInvoiceID As String) As Decimal

        Dim pSql = "SELECT SUM(SubTotal) AS SubTotal" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND (TransInvoiceItems.Quantity > 0 OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
            " AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0"
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return 0
        End If
        Dim pOut = CDec2(pDR("SubTotal"))

        Return pOut

    End Function

    Public Function VerifyInvoice(vInvoiceID As String, ByRef oError As String) As Boolean

        Dim pDR As DataRow

        pDR = FromCache(vInvoiceID)
        If pDR Is Nothing Then
            oError = "Invoice " & vInvoiceID & "(todo) was already paid. Please reload the page."
            Return False
        End If
        pDR = ById(vInvoiceID)
        Dim pTotalAmount = CDec(pDR("TotalAmount"))
        Dim pItemsAmount = mParent.TransInvoice.GetItemsTotal(vInvoiceID)
        Dim pDiff = pTotalAmount - pItemsAmount
        pDiff = Math.Abs(pDiff)
        If pDiff > 0.01 Then
            oError = "Invoice amount is " & Format(pTotalAmount, cGlobal.FormatCurrency) & ". Items total is " & Format(pItemsAmount, cGlobal.FormatCurrency) & ". Please save your invoice."
            Return False
        End If

        Return True

    End Function

    Public Function BySalesOrderID(ByVal vSalesOrderID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoice.ID" & vbCrLf &
            " FROM TransInvoice" &
            " WHERE TransInvoice.SalesOrderID = " & ReadyString(vSalesOrderID) &
            " AND TransType = 'Invoice'" & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetByInvoiceNumFowstIDDeviceSerial(vInvoiceNum As Integer, vFowstID As String, vDeviceSerial As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT TransInvoice.ID" & vbCrLf &
            " ,TransInvoice.InvoiceNumber" & vbCrLf &
            " ,LibraryDevice.UUID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.DateImported" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,TransInvoice.TotalAmount" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber = " & vInvoiceNum & vbCrLf &
            " AND TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND LibraryDevice.UUID = " & ReadyString(vDeviceSerial) & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ById2(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoice.CustomerID" & vbCrLf &
            ",TransInvoice.SalesOrderNo" & vbCrLf &
            ",TransInvoice.RefNum" & vbCrLf &
            ",TransInvoice.InvoiceDate" & vbCrLf &
            ",TransInvoice.InvoiceNumber" & vbCrLf &
            ",MobizLibraryOrganization.CompanyName" & vbCrLf &
            ",CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS MIN" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function ByFullNumber(ByVal vFullNumber As String, Optional vID As String = Nothing) As DataRow

        Dim pSelect As String
        Dim pFrom As String

        BrowserController.ComposeCustomer(pSelect, pFrom)
        With New Finder()
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransInvoice.ID")
                .Add("FOWSTID", SqlDbType.UniqueIdentifier, "TransInvoice.FOWSTID")
                .Add("DeviceID", SqlDbType.UniqueIdentifier, "TransInvoice.DeviceID")
                With .Add("InvoiceDate", SqlDbType.DateTime, "TransInvoice.InvoiceDate")
                    .Searchable = False
                End With
                With .Add("InvoiceAmount", SqlDbType.Money, "TransInvoice.TotalAmount")
                    .Searchable = False
                End With
                With .Add("FullNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL(LibraryDocumentControl.DocumentName, '') + '-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber)"
                    .Searchable = False
                End With
                .Add("CustomerName", SqlDbType.VarChar, pSelect)
            End With
            .SqlFrom = " FROM TransInvoice" & vbCrLf &
                " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID" & vbCrLf &
                " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
                pFrom
            If String.IsNullOrWhiteSpace(vID) Then
                .SqlWhere = " WHERE ISNULL(ISNULL(LibraryDocumentControl.DocumentName, '') + '-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber), '') = " & ReadyString(vFullNumber)
            Else
                .SqlWhere = " WHERE TransInvoice.ID = " & ReadyString(vID)
            End If
            .SqlOrder = " ORDER BY TransInvoice.LastUpdated DESC"

            Dim pSql = .CreateSql()
            Dim pDR = mProData.ExecuteRow(pSql)
            Return pDR
        End With

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDR">from ByID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsSavedAsFinal(vDR As DataRow, Optional vConsideredFinalIfPurchaseOrder As Boolean = False) As Boolean

        Dim pTransType As String
        Dim pTransTypeProper As SaveSalesOrderJson.eTransType

        If CStr2(vDR("TransType")) = "" Then
            pTransType = "Invoice"
        Else
            pTransType = CStr2(vDR("TransType"))
        End If

        If [Enum].GetNames(GetType(SaveSalesOrderJson.eTransType)).Contains(pTransType) = False Then
            Return False
        End If
        pTransTypeProper = [Enum].Parse(GetType(SaveSalesOrderJson.eTransType), pTransType)

        Select Case pTransTypeProper
            Case SaveSalesOrderJson.eTransType.Quote
                Return CDec2(vDR("QuotationNo")) <> 0

            Case SaveSalesOrderJson.eTransType.SalesOrder
                Return CDec2(vDR("SalesOrderNo")) <> 0

            Case SaveSalesOrderJson.eTransType.Invoice
                Return CDec2(vDR("InvoiceNumber")) <> 0

            Case SaveSalesOrderJson.eTransType.PurchaseOrder
                If vConsideredFinalIfPurchaseOrder Then
                    Return True
                End If
                Return CDec2(vDR("PurchaseOrderNo")) <> 0

            Case SaveSalesOrderJson.eTransType.AccountsPayable
                Return CDec2(vDR("AccountsPayableNum")) <> 0

        End Select
    End Function

    Public Function CanPrintInvoice(ByVal vInvoiceID As String, vDocumentName As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDR As DataRow
        Dim pWhereDocumentName

        If String.IsNullOrWhiteSpace(vDocumentName) = False Then
            pWhereDocumentName = " OR LibraryDocumentControl.DocumentName = " & ReadyString(vDocumentName) & vbCrLf
        End If
        pSql = "SELECT COUNT(*)" &
            " FROM TransInvoicePrints" &
            " WHERE InvoiceID = TransInvoice.ID" &
            " AND ApprovedByID IS NOT NULL" &
            " AND DeniedByID IS NULL"
        ' the below code has been excluded from the code above
        ' " AND DatePrinted IS NULL" 

        Dim pField = "PrintCount"

        If vDocumentName = "PickList" Then
            pField = "PickListPrintCount"
        ElseIf vDocumentName = "WithdrawalSlip" Then
            pField = "WithdrawalSlipPrintCount"
        End If
        pSql = "SELECT TransInvoice." & pField & " AS PrintCount" &
            ",LibraryDocumentControl.PrintLimit" &
            ",ISNULL((" & pSql & "), 0) AS ApprovedRequests" &
            " FROM TransInvoice" &
            " LEFT JOIN LibraryDocumentControl" &
            " ON TransInvoice.DocumentNameID = LibraryDocumentControl.ID" &
            pWhereDocumentName &
            " WHERE TransInvoice.ID = " & ReadyString(vInvoiceID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            Dim pPrintLimit = CInt2(pDR("PrintLimit"))
            If pPrintLimit < 0 Then
                Return True
            End If
            If pPrintLimit <= 0 Then
                pPrintLimit = 1
            End If
            If pPrintLimit < 0 Then
                Return True
            End If
            If CInt2(pDR("PrintCount")) < pPrintLimit + CInt2(pDR("ApprovedRequests")) Then
                Return True
            End If
        Else
            Return True
        End If

    End Function

    Public Sub RequestReprint(ByVal vInvoiceID As String, ByVal vRemarks As String, ByVal vUserID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoicePrints")
        pIU("ID") = "newid()"
        pIU("InvoiceID") = ReadyString(vInvoiceID)
        pIU("DateRequested") = "GETDATE()"
        pIU("RequestedByID") = ReadyString(vUserID)
        pIU("Remarks") = ReadyString(vRemarks)

        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function HasPendingPrintRequest(ByVal vID As String) As Boolean

        Dim pSql As String

        pSql = "SELECT COUNT(*) AS Count" &
            " FROM TransInvoicePrints" &
            " WHERE InvoiceID = " & ReadyString(vID) &
            " AND DateApproved IS NULL" &
            " AND DateDenied IS NULL"
        Return 0 < CDec2(mProData.ExecuteScalar(pSql))

    End Function

    ' this also used by TransSalesOrder
    Public Function AddProfitabilityColumns(ByVal vDT As DataTable, vOrgID As String,
        Optional vQuantityField As String = "Quantity") As DataTable

        Dim pDR As DataRow
        Dim pUnitGp As Decimal
        Dim pUnitGpm As Double
        Dim pLineGp As Decimal
        Dim pLineGpm As Decimal
        Dim pLandedCost As Decimal

        If vDT.Columns.Contains("Cost") = False Then vDT.Columns.Add("Cost", GetType(Decimal))
        If vDT.Columns.Contains("UnitGP") = False Then vDT.Columns.Add("UnitGP", GetType(Decimal))
        If vDT.Columns.Contains("UnitGPM") = False Then vDT.Columns.Add("UnitGPM", GetType(Decimal))
        If vDT.Columns.Contains("LineGP") = False Then vDT.Columns.Add("LineGP", GetType(Decimal))
        If vDT.Columns.Contains("LineGPM") = False Then vDT.Columns.Add("LineGPM", GetType(Decimal))
        If vDT.Columns.Contains("LineCost") = False Then vDT.Columns.Add("LineCost", GetType(Decimal))
        If vDT.Columns.Contains("CurrentCost") = False Then vDT.Columns.Add("CurrentCost", GetType(Decimal))
        If vDT.Columns.Contains("LineCost") = False Then vDT.Columns.Add("LineCost", GetType(Decimal))

        For Each pDR In vDT.Rows
            pDR("Cost") = CDec2(pDR("Cost"))
            If CDec2(pDR("ActualQuantity")) <= 0 Then
                pDR("ActualQuantity") = 1
            End If
            mParent.LibraryStock.GetProfitability(
                vStockID:=CStr2(pDR("ProductID")),
                vQty:=pDR(vQuantityField),
                vUnitQty:=pDR("ActualQuantity"),
                vUnitPrice:=pDR("UnitPrice"),
                oUnitGP:=pUnitGp,
                oUnitGPM:=pUnitGpm,
                oLineGP:=pLineGp,
                oLineGPM:=pLineGpm,
                oLandedCost:=pLandedCost,
                vHistoricCost:=pDR("Cost") / CDec2(pDR("ActualQuantity")),
                vOrgID:=vOrgID)

            If pDR("Cost") = 0 Then pDR("Cost") = pLandedCost
            'pDR("CurrentCost") = pLandedCost
            pDR("CurrentCost") = pLandedCost * pDR("ActualQuantity") ' test code. mar 16, 2013
            pDR("UnitGP") = pUnitGp
            pDR("UnitGPM") = pUnitGpm
            pDR("LineGP") = pLineGp
            pDR("LineGPM") = pLineGpm
            pDR("LineCost") = pDR("Cost") * pDR(vQuantityField) ' remove pDR("ActualQuantity") *
        Next pDR

        Return vDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vQuantityField"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveProfitability(vID As String, vOrgID As String, vQuantityField As String)

        Dim pDT = GetItemsForProfitability(vID)
        pDT = AddProfitabilityColumns(pDT, vOrgID, vQuantityField)
        For Each pDR In pDT.Rows
            Dim pIU As New InsertUpdate("TransInvoiceItems")
            pIU("Cost") = pDR("Cost")
            pIU("UnitGP") = pDR("UnitGP")
            pIU("UnitGPM") = pDR("UnitGPM")
            pIU("LineGP") = pDR("LineGP")
            pIU("LineGPM") = pDR("LineGPM")
            pIU("LineCost") = pDR("LineCost")
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    Private Function GetItemsForProfitability(vID As String) As DataTable

        Dim pSql = "SELECT ID" & vbCrLf &
            ",ProductID" & vbCrLf &
            ",Cost" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",Quantity" & vbCrLf &
            ",OrderedQuantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Event CreateDone(ByVal sender As Object, ByVal e As CreateDoneArgs)

    Public Class CreateDoneArgs
        Public ErrorMessage As String

        Public Sub New(ByVal vError As String)
            ErrorMessage = vError
        End Sub
    End Class

    Public Function UninvoicedBranchOrders() As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DISTINCT TransStockSortingList.ID" &
            ",TransStockSortingListItems.SalesOrderID" &
            ",TransSalesOrder.CustomerID" &
            ",TransSalesOrder.OrderNo" &
            ",TransSalesOrder.OrderDate" &
            ",LibraryCustomers.CompanyName" &
            " FROM TransStockSortingListItems" &
            " WITH (TABLOCKX)" &
            " JOIN TransStockSortingList ON TransStockSortingList.ID = TransStockSortingListItems.StockSortingListID" &
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransStockSortingListItems.SalesOrderID" &
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransSalesOrder.CustomerID" &
            " WHERE TransSalesOrder.IsCancelled = 0" &
            " AND TransStockSortingListItems.IsInvoiced = 0" &
            " AND LibraryCustomers.IsCompanyBranch = 1" &
            " AND ISNULL(TransSalesOrder.InStore, 0) = 0" &
            " AND TransSalesOrder.OrderDate >= '7/1/2010'"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    '    Public Function AutoCreateBranchInvoices(ByRef oError As String) As DataTable

    '        Dim pDT As DataTable
    '        Dim pDR As DataRow
    '        Dim pCreator As Creator
    '        Dim pBranchInvoiceDefaultId, pCustomerInvoiceDefaultId As String
    '        Dim pInvoiceType As String
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow

    '        oError = Nothing
    '#If Debug = 0 Then
    '            Try
    '#End If
    '        mProData.BeginTransaction()

    '        If mParent.SystemDefaults.InvoiceTypeForBranchAutoInvoice(pBranchInvoiceDefaultId, pInvoiceType, oError) = False Then
    '            mProData.RollbackTransaction()
    '            RaiseEvent CreateDone(Me, New CreateDoneArgs(oError))
    '            Return Nothing
    '        End If
    '        pCustomerInvoiceDefaultId = TransInvoice.gIdChi

    '        '// uninvoiced, served branch orders
    '        pDT = UninvoicedBranchOrders()
    '        If pDT.Rows.Count = 0 Then
    '            mProData.RollbackTransaction()
    '            'oError = "No withdrawn, uninvoiced order from branch."
    '            Return Nothing
    '        End If

    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID", GetType(Guid))
    '        pDtOut.Columns.Add("InvoiceNum", GetType(Integer))
    '        pDtOut.Columns.Add("OrderNo", GetType(Integer))

    '        pDT = pDT.DefaultView.ToTable(True, "ID", "SalesOrderID", "CustomerID", "OrderNo")
    '        For Each pDR In pDT.Rows
    '            pCreator = New Creator(mProData, Me)
    '            pCreator.StockSortingListID = CStr2(pDR("ID"))
    '            If pCreator.GetStocks() = False Then ' set StockSortingListID before calling this
    '                pCreator.InvoiceNumber = 0
    '            End If
    '            pCreator.InvoiceDefaultID = pBranchInvoiceDefaultId
    '            pCreator.InvoiceType = pInvoiceType
    '            pCreator.InvoiceDate = mProData.GetDate.Date
    '            pCreator.SalesOrderID = CStr2(pDR("SalesOrderID"))
    '            pCreator.CustomerID = CStr2(pDR("CustomerID"))
    '            pCreator.BillingAddressID = mParent.LibraryCustomers.GetBillingAddress(CStr2(pDR("CustomerID")))
    '            pCreator.DeliveryAddressID = mParent.LibraryCustomers.GetDeliveryAddress(CStr2(pDR("CustomerID")))
    '            pCreator.Remarks = "Auto-created invoice"
    '            pCreator.UserID = modClient.EmployeeWebID
    '            pCreator.EncodedBy = modClient.EmployeeWebID
    '            pCreator.InvoiceDateTime = mProData.GetDate
    '            If pCreator.Save(True, oError) = False Then
    '                mProData.RollbackTransaction()
    '                Return Nothing
    '            End If

    '            pDrOut = pDtOut.NewRow
    '            pDrOut("ID") = pCreator.ID
    '            pDrOut("InvoiceNum") = pCreator.InvoiceNumber
    '            pDtOut.Rows.Add(pDrOut)
    '        Next pDR
    '        mProData.CommitTransaction()
    '        oError = Nothing
    '        Return pDtOut
    '#If Debug = 0 Then
    '            Catch ex As Exception
    '                oError = ex.Message
    '                Try
    '                    mProData.RollbackTransaction()
    '                Catch ex2 As Exception
    '                End Try
    '                Return Nothing
    '            End Try
    '#End If

    '    End Function

    Public Function CreateCreator() As Creator

        Return New Creator(mProData, Me)

    End Function

    Public Class Creator
        ReadOnly mProData As ProData
        ReadOnly mParent As TransInvoice
        Public Stocks As DataTable
        ''' <summary>
        ''' MainInvoiceDefaults.Abbrevation
        ''' </summary>
        ''' <remarks></remarks>
        Public InvoiceType As String
        Public ID As String
        Public FowstID As String
        Public DeviceID As String
        'Public SubscriberBranchID As String ' not used in Mobiz
        'Public StockSortingListID As String ' not used in Mobiz
        ' Public InvoiceDefaultID As String ' not used in Mobiz
        Public CusRepresentativeID As String
        Public InvoiceNumber As String
        Public InvoiceDate As Date?
        Public SalesOrderID As String
        Dim mCustomerID As String
        Public BillingAddressID As String
        Public DeliveryAddressID As String
        Dim mDeliveryDate As Date?
        Public Remarks As String
        Public InternalRemarks As String
        'Public BracketNumber As Integer ' not used in Mobiz
        Public TotalAmount As Decimal
        Public VAT As Single
        Public NumberOfPrints As Integer
        Public IsCancelled As Boolean
        Public IsHasTaxWithHeld As Boolean
        Public ServedQtyIsEdited As Boolean
        Public IsServedBackOrder As String
        Public IsCustomerFirstOrder As Boolean
        Public UserID As String ' former integer
        Public IsForGovReport As Boolean
        Public IsNoneVATSale As Boolean
        Public ServingFowstID As String
        'Public LastUpdated As Date      ' not used
        'Public EncodedBy As Integer ' not used in Mobiz
        'Public RegisteredDate As Date   ' not used
        'Public IsExported As Boolean
        'Public ExportCount As Integer
        Public GP As Decimal
        Public GPM As Decimal
        Public InvoiceDateTime As Date
        'Public InvoiceNoExtension As String ' not used in Mobiz
        'Public CancelledBy As Integer
        'Public CancelReasonID As String
        'Public PerfectOrder As Boolean
        'Public TeamID As Integer
        'Public PercentIncentive As Single
        'Public isCollected As Boolean
        Dim mCustomerName As String
        Public LastUpdated As Date?
        Public SubNumber As Integer?

        Public ProductID As String
        Public SalesRepList As Object()
        Public ProductSerialList As Object()
        Public SalesOrderNo As Integer?
        Public SalesOrderUserID As String
        Public SalesOrderDate As Date?
        Public RefNum As String
        Public ReasonForReconstructingData As String
        Public CustomerNameReferenceOnly As String
        Public CustomerTableNo As String
        Public CustomerTableNoOfPeople As Integer
        Public CustomerReferenceNumber As Integer
        Public IsTestOnly As Boolean
        Public PurchaseOrderNo As Integer?
        Public PurchaseOrderText As String
        Public AccountsPayableNum As Integer?
        Public PurchaseOrderDate As Date?
        Public PurchaserName As String
        Public SalesOrderSignatory As String
        Public DeliveryMethod As String
        Public OrderedByID As String
        Public OrderMethod As String
        Public CustomerDemographicGender As String
        Public CustomerDemographicAge As String
        Public TotalNumberOfCustomer As Integer
        Public FranchiseeOrgID As String
        Public SODeliveryDate As Date
        Public SupplierID As String
        ''' <summary>
        ''' Populated only when user choose FOWST from own org
        ''' </summary>
        ''' <remarks></remarks>
        Public SupplierFowstID As String ' ref: 2015_08_31__11_05_36_0178
        Public InvoiceTypeCode As String
        Public DucInvoiceNum As String
        Public DucInvoiceType As String
        Public DucInvoiceID As String
        Public ReceivingID As String
        Public SupplierBinID As String
        Public CustomerFowstID As String
        Public CustomerBinID As String

        Public Vatable As Decimal
        Public VATExempt As Decimal
        Public ForegoneVAT As Decimal
        Public GrossSalesToRegDiner As Decimal
        Public GrossSalesToSC As Decimal
        Public WeightedSCDiscountRate As Decimal
        Public PhNatTeamDiscount As Decimal
        Public SoloParentDiscount As Decimal
        Public SCDiscount As Decimal
        Public SCDiscountExempt As Decimal
        Public TotalNonSCDiscount As Decimal
        Public AmountDueBeforeCharges As Decimal
        Public AmountDueNetofCharges As Decimal
        Public DepositAmount As Decimal

        Public PrintCount As Integer?
        Public IsPrinted As Boolean?
        Public QuotationNo As Integer?
        Public QuotationDate As Date?
        Public QuotationNumText As String
        Public SalesPartnerOrgID As String
        Dim mTransType As String
        Public DateNeeded As Date?
        Public SalesRepPeopleID As String
        Public SalesRepText As String
        Public QuotationID As String
        Public CnCUnapprovedReason As String
        Public DocumentNameID As String
        Public RefInvoiceNum As String
        Public PaymentTerm As Integer?
        Public CanvassID As String
        Public RepeatingTransactionID As String
        Public UneditedBackorder As Boolean?
        Public BackorderSourceInvoiceID As String
        Public FowstWideInvoiceNumber As Integer?
        Public ParentOrderID As String
        Public EncodingFowstID As String
        Public InvoiceFowstID As String
        Public SalesInvoiceRef As String
        Public OfficialReceiptRef As String
        Public IsEmailReceipt As String
        Public StockTransferID As String ' TransReceiving.ID
        Public DeviceFowstTransactionNumber As String ' int
        Public ErrorCorrectAmount As Decimal
        Public ErrorCorrectCount As Integer
        Public StrNum As Integer?
        Public FinalizedForApproval As Boolean
        Public Area As String
        Public IsPos As Boolean
        Public DontAutoInvoice As String ' bit
        Public DontSaveTransType As String ' bit 
        Public HeaderStopWatch As New Stopwatch
        Public ItemsStopWatch As New Stopwatch

        '// "Freight and Handling" tab
        Public ShipperID As String
        Public WaybillNumber As String
        Public ShipDate As String ' date
        Public PackageQuantity As String
        Public GrossWeight As String
        Public GrossVolume As String
        Public Freight As String ' money
        Public Handling As String ' money
        Public Arrastre As String ' money
        Public OtherCharges As String ' money

        'Start - For Expense Ticket Module
        Public ExpenseReferenceNo As Integer?
        Public ReimburseToEmployeeName As String
        'END - For Expense Ticket Module

        Friend Sub New(ByVal vProData As ProData, ByVal vParent As TransInvoice)

            mProData = vProData
            mParent = vParent

        End Sub

        Public Property CustomerID As String
            Get

                Return mCustomerID

            End Get
            Set(ByVal value As String)

                Dim pDR As DataRow

                mCustomerID = value
                pDR = mParent.Parent.LibraryCustomers.ById(mCustomerID)
                If pDR IsNot Nothing Then
                    mCustomerName = pDR("CompanyName") & ""
                End If

            End Set
        End Property

        Public Property TransType As String
            Get
                Return mTransType
            End Get
            Set(value As String)
                If String.IsNullOrWhiteSpace(value) = False Then
                    If [Enum].GetNames(GetType(SaveSalesOrderJson.eTransType)).Contains(value) = False Then
                        Throw New ArgumentException("Invalid TransType, " & TransType & ".")
                    End If
                End If
                mTransType = value
            End Set
        End Property

        Public ReadOnly Property DeliveryDate() As Date
            Get

                Return mDeliveryDate

            End Get
        End Property

        Public Function Save(ByVal vNew As Boolean, ByRef oError As String) As Boolean

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDR As DataRow
            Dim pRecords As Integer

            Try
                HeaderStopWatch.Start()
                ' require at least 1 item: 9/24/2014 11:34 AM
                'If Stocks Is Nothing _
                '    OrElse Stocks.Rows.Count = 0 Then

                '    oError = "Invoice must have at least 1 item vbK37NRE5GYZ!"
                '    Return False
                'End If
                oError = ""
                mProData.BeginTransaction()

                pIU = New InsertUpdate("TransInvoice")
                ' not used in Mobiz
                'If CStr2(StockSortingListID).Length > 0 Then
                '    pIU("StockSortingListID") = ReadyString(StockSortingListID)
                'End If
                'pIU("InvoiceDefaultID") = ReadyString(InvoiceDefaultID) ' not used in Mobiz
                ' not used in Mobiz
                'If CStr2(InvoiceNumber).Length = 0 Then
                '    InvoiceNumber = mParent.NextInvoiceNumber(InvoiceDefaultID, InvoiceNoExtension)
                'End If
                If CStr2(CusRepresentativeID).Length > 0 Then
                    pIU("CusRepresentativeID") = ReadyString(CusRepresentativeID)
                End If
                If String.IsNullOrWhiteSpace(FowstID) = False Then
                    pIU("FowstID") = ReadyString(FowstID)
                End If
                pIU("InvoiceNumber") = InvoiceNumber
                If CStr2(SalesOrderID).Length > 0 Then pIU("SalesOrderID") = ReadyString(SalesOrderID)
                pIU("CustomerID") = ReadyString(CustomerID)
                '' additional by janine apr 1,2014
                'If CStr2(CustomerNameReferenceOnly).Length > 0 Then pIU("CustomerNameReferenceOnly") = ReadyString(CustomerNameReferenceOnly)
                If CStr2(CustomerTableNo).Length > 0 Then pIU("CustomerTableNo") = ReadyString(CustomerTableNo)
                'If CStr2(CustomerTableNoOfPeople).Length > 0 Then pIU("CustomerTableNoOfPeople") = CustomerTableNoOfPeople
                'If CStr2(CustomerReferenceNumber).Length > 0 Then pIU("CustomerReferenceNumber") = CustomerReferenceNumber

                If CStr2(DeliveryMethod).Length > 0 Then pIU("DeliveryMethod") = ReadyString(DeliveryMethod)
                If CStr2(OrderedByID).Length > 0 Then pIU("OrderedByID") = ReadyString(OrderedByID)
                If CStr2(OrderMethod).Length > 0 Then pIU("OrderMethod") = ReadyString(OrderMethod)
                If CStr2(CustomerDemographicGender).Length > 0 Then pIU("CustomerDemographicGender") = ReadyString(CustomerDemographicGender)
                If CStr2(CustomerDemographicAge).Length > 0 Then pIU("CustomerDemographicAge") = ReadyString(CustomerDemographicAge)
                '' end of additional
                If CStr2(BillingAddressID) <> "" Then
                    pIU("BillingAddressID") = ReadyString(BillingAddressID)
                End If
                If CStr2(DeliveryAddressID) <> "" Then
                    pIU("DeliveryAddressID") = ReadyString(DeliveryAddressID)
                End If
                If mDeliveryDate IsNot Nothing Then
                    pIU("DeliveryDate") = ReadyString(mDeliveryDate)
                End If
                If Remarks IsNot Nothing Then
                    pIU("Remarks") = ReadyString(Remarks)
                End If
                If InternalRemarks IsNot Nothing Then
                    pIU("InternalRemarks") = ReadyString(InternalRemarks)
                End If
                'pIU("BracketNumber") = BracketNumber ' not used in Mobiz
                pIU("TotalAmount") = Math.Round(TotalAmount, 2)
                pIU("VAT") = VAT
                pIU("UserID") = ReadyString(UserID)
                pIU("LastUpdated") = "GETDATE()"
                'pIU("EncodedBy") = EncodedBy ' not used in Mobiz
                'pIU("RegisteredDate") = pIU("LastUpdated") ' not used in Mobiz
                pIU("GP") = GP
                pIU("GPM") = GPM
                pIU("IsForGovReport") = IsForGovReport
                pIU("IsNoneVATSale") = IsNoneVATSale
                'pIU("InvoiceDateTime") = "GETDATE()" ' not used in Mobiz
                'pIU("InvoiceNoExtension") = ReadyString(InvoiceNoExtension) ' not used in Mobiz
                ' not used in Mobiz
                'If CStr2(SubscriberBranchID).Length > 0 Then
                '    pIU("SubscriberBranchID") = ReadyString(SubscriberBranchID)
                'Else
                '    pIU("SubscriberBranchID") = "null"
                'End If
                If CStr2(DeviceID).Length > 0 Then
                    pIU("DeviceID") = ReadyString(DeviceID)
                End If
                If LastUpdated IsNot Nothing Then
                    pIU("LastUpdated") = ReadyString(LastUpdated)
                End If
                If SalesOrderNo IsNot Nothing Then
                    pIU("SalesOrderNo") = SalesOrderNo
                End If
                If SalesOrderDate IsNot Nothing Then
                    pIU("SalesOrderDate") = ReadyString(SalesOrderDate)
                End If
                If CStr2(SalesOrderUserID).Length > 0 Then
                    pIU("SalesOrderUserID") = ReadyString(SalesOrderUserID)
                End If
                If CStr2(RefNum).Length > 0 Then
                    pIU("RefNum") = ReadyString(RefNum)
                End If
                If CStr2(ReasonForReconstructingData).Length > 0 Then
                    pIU("ReasonForReconstructingData") = ReadyString(ReasonForReconstructingData)
                End If
                If CStr2(StockTransferID).Length > 0 Then
                    pIU("StockTransferID") = ReadyString(StockTransferID)
                End If
                If IsNumeric(DeviceFowstTransactionNumber) Then
                    pIU("DeviceFowstTransactionNumber") = CInt2(DeviceFowstTransactionNumber)
                End If
                pIU("IsTestOnly") = If(IsTestOnly, 1, 0)
                pIU("IsForGovReport") = If(IsForGovReport, 1, 0)
                pIU("IsNoneVATSale") = If(IsNoneVATSale, 1, 0)
                If SubNumber IsNot Nothing Then
                    pIU("SubNumber") = SubNumber
                End If
                'additional by janine aug 6 
                If PurchaseOrderNo IsNot Nothing Then
                    pIU("PurchaseOrderNo") = PurchaseOrderNo
                End If
                If PurchaseOrderText IsNot Nothing Then
                    pIU("PurchaseOrderText") = ReadyString(PurchaseOrderText)
                End If
                If CStr2(PurchaseOrderDate).Length > 0 Then
                    pIU("PurchaseOrderDate") = ProSqlite.ReadyDate(PurchaseOrderDate)
                End If
                If CStr2(PurchaserName).Length > 0 Then
                    pIU("PurchaserName") = ReadyString(PurchaserName)
                End If
                If CStr2(SalesOrderSignatory).Length > 0 Then
                    pIU("SalesOrderSignatory") = ReadyString(SalesOrderSignatory)
                End If
                If CStr2(DeliveryMethod).Length > 0 Then
                    pIU("DeliveryMethod") = ReadyString(DeliveryMethod)
                End If
                If CStr2(OrderMethod).Length > 0 Then
                    pIU("OrderMethod") = ReadyString(OrderMethod)
                End If
                If CStr2(CustomerDemographicGender).Length > 0 Then
                    pIU("CustomerDemographicGender") = ReadyString(CustomerDemographicGender)
                End If
                If CStr2(CustomerDemographicAge).Length > 0 Then
                    pIU("CustomerDemographicAge") = ReadyString(CustomerDemographicAge)
                End If
                If TotalNumberOfCustomer > 0 Then
                    pIU("TotalNumberOfCustomer") = TotalNumberOfCustomer
                End If
                If CStr2(FranchiseeOrgID).Length > 0 Then
                    pIU("FranchiseeOrgID") = ReadyString(FranchiseeOrgID)
                End If
                'end of additional
                If (CStr2(SupplierID).Length > 0) Then
                    pIU("SupplierID") = ReadyString(SupplierID)
                End If
                If (CStr2(SupplierFowstID).Length > 0) Then
                    pIU("SupplierFowstID") = ReadyString(SupplierFowstID)
                End If
                If (CStr2(InvoiceTypeCode).Length > 0) Then
                    pIU("InvoiceTypeCode") = ReadyString(InvoiceTypeCode)
                End If

                If (CStr2(DucInvoiceNum).Length > 0) Then
                    pIU("DucInvoiceNum") = ReadyString(DucInvoiceNum)
                End If

                If (CStr2(DucInvoiceType).Length > 0) Then
                    pIU("DucInvoiceType") = ReadyString(DucInvoiceType)
                End If

                If (CStr2(DucInvoiceID).Length > 0) Then
                    pIU("DucInvoiceID") = ReadyString(DucInvoiceID)
                End If

                If (CStr2(ReceivingID).Length > 0) Then
                    pIU("ReceivingID") = ReadyString(ReceivingID)
                End If
                If (CStr2(SupplierBinID).Length > 0) Then
                    pIU("SupplierBinID") = ReadyString(SupplierBinID)
                End If
                If (CStr2(CustomerFowstID).Length > 0) Then
                    pIU("CustomerFowstID") = ReadyString(CustomerFowstID)
                End If
                If (CStr2(CustomerBinID).Length > 0) Then
                    pIU("CustomerBinID") = ReadyString(CustomerBinID)
                End If

                If (CStr2(Vatable).Length > 0) Then
                    pIU("Vatable") = Vatable
                End If
                If (CStr2(VATExempt).Length > 0) Then
                    pIU("VATExempt") = VATExempt
                End If
                If (CStr2(ForegoneVAT).Length > 0) Then
                    pIU("ForegoneVAT") = ForegoneVAT
                End If
                If (CStr2(GrossSalesToRegDiner).Length > 0) Then
                    pIU("GrossSalesToRegDiner") = GrossSalesToRegDiner
                End If
                If (CStr2(GrossSalesToSC).Length > 0) Then
                    pIU("GrossSalesToSC") = GrossSalesToSC
                End If
                If (CStr2(WeightedSCDiscountRate).Length > 0) Then
                    pIU("WeightedSCDiscountRate") = WeightedSCDiscountRate
                End If
                If (CStr2(SoloParentDiscount).Length > 0) Then
                    pIU("SoloParentDiscount") = SoloParentDiscount
                End If
                If (CStr2(PhNatTeamDiscount).Length > 0) Then
                    pIU("PhNatTeamDiscount") = PhNatTeamDiscount
                End If
                If (CStr2(SCDiscount).Length > 0) Then
                    pIU("SCDiscount") = SCDiscount
                End If
                If (CStr2(SCDiscountExempt).Length > 0) Then
                    pIU("SCDiscountExempt") = SCDiscountExempt
                End If
                If (CStr2(TotalNonSCDiscount).Length > 0) Then
                    pIU("TotalNonSCDiscount") = TotalNonSCDiscount
                End If
                If (CStr2(AmountDueBeforeCharges).Length > 0) Then
                    pIU("AmountDueBeforeCharges") = AmountDueBeforeCharges
                End If
                If (CStr2(AmountDueNetofCharges).Length > 0) Then
                    pIU("AmountDueNetofCharges") = AmountDueNetofCharges
                End If
                If (CStr2(DepositAmount).Length > 0) Then
                    pIU("DepositAmount") = DepositAmount
                End If
                If PrintCount IsNot Nothing Then
                    pIU("PrintCount") = PrintCount
                End If
                If IsPrinted IsNot Nothing Then
                    pIU("IsPrinted") = If(IsPrinted, 1, 0)
                End If
                If QuotationNo IsNot Nothing Then
                    pIU("QuotationNo") = QuotationNo
                End If
                If QuotationDate IsNot Nothing Then
                    pIU("QuotationDate") = ReadyString(QuotationDate)
                End If
                If QuotationNumText IsNot Nothing Then
                    pIU("QuotationNumText") = ReadyString(QuotationNumText)
                End If
                If SalesPartnerOrgID IsNot Nothing Then
                    pIU("SalesPartnerOrgID") = ReadyString(SalesPartnerOrgID)
                End If
                If CBool2(DontSaveTransType) = False Then
                    If TransType IsNot Nothing _
                        AndAlso CBool2(IsPos) = False Then

                        pIU("TransType") = ReadyString(TransType)
                    End If
                End If
                If DateNeeded IsNot Nothing Then
                    pIU("DateNeeded") = ReadyString(DateNeeded)
                End If
                If SalesRepPeopleID IsNot Nothing Then
                    pIU("SalesRepPeopleID") = ReadyString(SalesRepPeopleID)
                End If
                If SalesRepText IsNot Nothing Then
                    pIU("SalesRepText") = ReadyString(SalesRepText)
                End If
                If QuotationID IsNot Nothing Then
                    pIU("QuotationID") = ReadyString(QuotationID)
                End If
                If CnCUnapprovedReason IsNot Nothing Then
                    pIU("CnCUnapprovedReason") = ReadyString(CnCUnapprovedReason)
                End If
                If String.IsNullOrWhiteSpace(RefInvoiceNum) = False Then
                    pIU("RefInvoiceNum") = ReadyString(RefInvoiceNum)
                End If
                If String.IsNullOrWhiteSpace(DocumentNameID) = False Then
                    pIU("DocumentNameID") = ReadyString(DocumentNameID)
                End If
                If PaymentTerm IsNot Nothing Then
                    pIU("PaymentTerm") = PaymentTerm.Value
                End If
                If CanvassID IsNot Nothing Then
                    pIU("CanvassID") = ReadyString(CanvassID)
                End If
                If RepeatingTransactionID IsNot Nothing Then
                    pIU("RepeatingTransactionID") = ReadyString(RepeatingTransactionID)
                End If
                If UneditedBackorder IsNot Nothing Then
                    pIU("UneditedBackorder") = If(UneditedBackorder, 1, 0)
                End If
                If FowstWideInvoiceNumber IsNot Nothing Then
                    pIU("FowstWideInvoiceNumber") = FowstWideInvoiceNumber
                End If
                If String.IsNullOrWhiteSpace(BackorderSourceInvoiceID) = False Then
                    pIU("BackorderSourceInvoiceID") = ReadyString(BackorderSourceInvoiceID)
                End If
                'pIU("IntentionalError") = ReadyString("vb272WJLBLE7") ' 2018_01_13__10_57_19_3447 testing error handler
                If AccountsPayableNum IsNot Nothing Then
                    pIU("AccountsPayableNum") = AccountsPayableNum
                End If
                If ParentOrderID IsNot Nothing Then
                    pIU("ParentOrderID") = ReadyString(ParentOrderID)
                End If
                If String.IsNullOrWhiteSpace(InvoiceFowstID) = False Then
                    pIU("InvoiceFowstID") = ReadyString(InvoiceFowstID)
                End If
                If String.IsNullOrWhiteSpace(SalesInvoiceRef) = False Then
                    pIU("SalesInvoiceRef") = ReadyString(SalesInvoiceRef)
                End If
                If String.IsNullOrWhiteSpace(OfficialReceiptRef) = False Then
                    pIU("OfficialReceiptRef") = ReadyString(OfficialReceiptRef)
                End If
                If StrNum IsNot Nothing Then
                    pIU("StrNum") = StrNum.Value
                End If
                If ExpenseReferenceNo IsNot Nothing Then
                    pIU("ExpenseReferenceNo") = ReadyString(ExpenseReferenceNo)
                End If
                If ReimburseToEmployeeName IsNot Nothing Then
                    pIU("ReimburseToEmployeeName") = ReadyString(ReimburseToEmployeeName)
                End If
                If Area IsNot Nothing Then
                    pIU("Area") = ReadyString(Area)
                End If
                pIU("FinalizedForApproval") = If(FinalizedForApproval, "1", "0")
                pIU("DontAutoInvoice") = If(CBool2(DontAutoInvoice), "1", "0")
                If String.IsNullOrWhiteSpace(ServingFowstID) = False Then
                    pIU("ServingFOWSTID") = ReadyString(ServingFowstID)
                End If
                pIU("ErrorCorrectAmount") = ErrorCorrectAmount
                pIU("ErrorCorrectCount") = ErrorCorrectCount

                '// "Freight and Handling" tab
                If String.IsNullOrWhiteSpace(ShipperID) = False Then
                    pIU("ShipperID") = ReadyString(ShipperID)
                End If
                pIU("WaybillNumber") = ReadyString(WaybillNumber)
                If String.IsNullOrWhiteSpace(ShipDate) Then
                    pIU("ShipDate") = "NULL"
                Else
                    pIU("ShipDate") = ReadyString(ShipDate)
                End If
                pIU("PackageQuantity") = ReadyString(PackageQuantity)
                pIU("GrossWeight") = ReadyString(GrossWeight)
                pIU("GrossVolume") = ReadyString(GrossVolume)
                pIU("Freight") = CDec2(Freight)
                pIU("Handling") = CDec2(Handling)
                pIU("Arrastre") = CDec2(Arrastre)
                pIU("OtherCharges") = CDec2(OtherCharges)
                pIU("FreightAndHandling") = CDec2(Freight) +
                        CDec2(Handling) +
                        CDec2(Arrastre) +
                        CDec2(OtherCharges)
                Dim pUpdateSql As String
                If CStr2(ID) <> "" Then
                    If InvoiceDate IsNot Nothing Then
                        pIU("InvoiceDate") = ReadyString(InvoiceDate)
                    End If

                    If CBool2(DontSaveTransType) Then
                        pIU.Remove("TransType") ' redundant, i know
                    Else
                        ' todo: i-comment ni 2024_01_25__10_36_58_8953 if
                        pSql = "SELECT TransType FROM TransInvoice WHERE ID = " & ReadyString(ID)
                        Dim pExistingRecord = mProData.ExecuteRow(pSql)
                        If pExistingRecord IsNot Nothing Then
                            If CStr2(pExistingRecord("TransType")) <> TransType Then
                                If pExistingRecord("TransType") = "ExpenseTicket" _
                                    AndAlso TransType = "AccountsPayable" Then
                                    ' just allow this change
                                Else
                                    Throw New ApplicationException("Please reload the web browser tab and save the transaction again. The trans type (" & TransType & ") you are saving now is different from the trans type already in record (" & CStr2(pExistingRecord("TransType")) & "). ref: vbCEGHYZTB24!")
                                End If
                            End If
                        End If
                    End If

                    pSql = pIU.GetUpdate("ID = " & ReadyString(ID))
                    pUpdateSql = pSql

                    pRecords = mProData.ExecuteSQL(pSql, True)
                End If
                If pRecords = 0 Then
                    If InvoiceDate Is Nothing Then
                        pIU("InvoiceDate") = "GETDATE()"
                    Else
                        pIU("InvoiceDate") = ReadyString(InvoiceDate)
                    End If
                    If CStr2(ID) = "" Then
                        ID = Guid.NewGuid.ToString
                    End If
                    pIU("ID") = ReadyString(ID)
                    pIU("CreatedBy") = ReadyString(UserID)
                    If String.IsNullOrWhiteSpace(EncodingFowstID) Then
                        pIU("EncodingFowstID") = ReadyString(FowstID)
                    Else
                        pIU("EncodingFowstID") = ReadyString(EncodingFowstID)
                    End If
                    pSql = pIU.GetInsert
                    Try
                        mProData.ExecuteSQL(pSql)
                    Catch ex As Exception
                        'Throw New ApplicationException(ex.Message & " vbJX7R69LXDT! " & vbCrLf & pIU.GetUpdate("INSERT"), ex)
                        AppendToLog(ex.Message & " vbJX7R69LXDT! " & vbCrLf & pUpdateSql)
                        Throw
                    End Try
                    Dim pDnR As New DashBoardsAndReports
                    pDnR.InvoiceID = ID
                    'pDnR.SaveCreatedBy(UserID, Now, mProData) ' commented 2022_03_04__14_55_57_3362
                End If
                HeaderStopWatch.Stop()

                ItemsStopWatch.Start()
                ''Stock
                If Stocks IsNot Nothing Then
                    If mParent.SaveStocks(ID, Stocks, vServedQtyIsEdited:=ServedQtyIsEdited) Then
                        UpdateTotal()

                        ''// customer ledger
                        'If mParent.Parent.LibraryCustomers.AddLedger(CustomerID, InvoiceType & " - " & Format(InvoiceNumber, "0000000") & " (Sales Order)", TotalAmount, 0, oError) = False Then
                        '    mProData.RollbackTransaction()
                        '    Return False
                        'End If
                    Else
                        mProData.RollbackTransaction()
                        Return False
                    End If
                End If

                If ProductID IsNot Nothing Then
                    ''Sales rep
                    If Not mParent.SaveSalesRepresentative(ID, ProductID, SalesRepList) Then
                        mProData.RollbackTransaction()
                        Return False
                    End If
                    'Product Serial
                    If Not mParent.SaveSalesProductSerial(ID, ProductID, ProductSerialList) Then
                        mProData.RollbackTransaction()
                        Return False
                    End If
                End If
                'mParent.SaveOrigFields(ID) ' commented 2022_03_04__14_55_57_3362
                mProData.CommitTransaction()
                ItemsStopWatch.Stop()

                Return True
            Catch ex As Exception
                oError = ex.Message & vbCrLf & GetStackTrace(, ex) & vbCrLf
                If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                    oError &= "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf
                End If
                AppendToLog(oError, , "TransInvoice.Save")
                Try
                    mProData.RollbackTransaction()
                Catch ex2 As Exception
                    oError &= vbCrLf & ex2.Message
                End Try
            End Try

        End Function

        Public Sub UpdateTotal()

            Dim pIU As InsertUpdate
            Dim pSql As String

            '// update total base on items
            pSql = " SELECT TransInvoiceItems.SubTotal" & vbCrLf &
                " ,TransInvoiceItems.LineCost" & vbCrLf &
                " FROM TransInvoiceItems" & vbCrLf &
                " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(ID) & vbCrLf &
                " AND (TransInvoiceItems.Quantity > 0 OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                " AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0"
            Dim pDtStocks = mProData.Execute(pSql)
            ComputeStocks(pDtStocks)
            pIU = New InsertUpdate("TransInvoice")
            pIU("TotalAmount") = Math.Round(TotalAmount, 2)
            pIU("GP") = GP
            pIU("GPM") = GPM
            pIU("UserID") = ReadyString(UserID)
            pSql = pIU.GetUpdate("ID = " & ReadyString(ID))
            mProData.ExecuteSQL(pSql)

        End Sub

        ''' <summary>
        ''' set StockSortingListID before calling this
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Public Function GetStocks() As Boolean
        ' not used in Mobiz

        '    Stocks = mParent.GetSortingListItems(StockSortingListID, True)
        '    If Stocks.Rows.Count = 0 Then Return False
        '    ComputeStocks()

        '    Return CDec2(Stocks.Compute("SUM(Quantity)", "")) > 0

        'End Function

        Private Sub GetStocks2()

            Dim pSql As String
            Dim pDS As DataSet

            pSql = " WHERE TransInvoice.ID = " & ReadyString(ID)
            pDS = mParent.GetReportData(pSql, False)
            Stocks = pDS.Tables("Stocks")

        End Sub

        Public Sub ComputeStocks(Optional vDtStock As DataTable = Nothing)

            Dim pCost As Decimal

            If vDtStock Is Nothing Then
                vDtStock = Stocks
            End If
            If vDtStock Is Nothing Then
                Throw New ApplicationException("Stock is not yet set")
            End If

            If vDtStock.Rows.Count = 0 Then Exit Sub
            TotalAmount = CDec2(vDtStock.Compute("SUM(Subtotal)", ""))
            pCost = CDec2(vDtStock.Compute("SUM(LineCost)", ""))

            GP = TotalAmount - pCost
            If TotalAmount = 0 Then
                If GP > 0 Then
                    GPM = 1
                ElseIf GP < 0 Then
                    GPM = -1
                Else
                    GPM = 0
                End If
            Else
                GPM = GP / TotalAmount
            End If

        End Sub
    End Class

    Public Function SaveStocks(ByVal vInvoiceID As String, ByVal vDtStocks As DataTable,
        Optional vBackOrderSourceSalesOrderID As String = Nothing,
        Optional vBackOrderInvoiceID As String = Nothing,
        Optional vBackOrderSourceID As String = Nothing,
        Optional vFowstID As String = Nothing,
        Optional vUserID As String = Nothing,
        Optional vServedQtyIsEdited As Boolean = False) As Boolean

        Dim pIU As InsertUpdate
        Dim pDR As DataRow
        Dim pSql As String
        Dim pID As String
        Dim pIuSalesRep As InsertUpdate

        For Each pDR In vDtStocks.Rows
            pIU = New InsertUpdate("TransInvoiceItems")
            If vDtStocks.Columns.Contains("ID") _
                AndAlso IsGuid(pDR("ID")) Then

                ' during import, ID is generated from device
                pID = ReadyGuid(pDR("ID"))
            Else
                ' during online mode,no ID is present
                pID = Guid.NewGuid.ToString
            End If
            pIU("ID") = ReadyString(pID)

            pIU("InvoiceID") = ReadyString(vInvoiceID)
            pIU("ProductID") = ReadyString(pDR("StockID"))
            pIU("Quantity") = CDec2(pDR("Quantity"))
            If vServedQtyIsEdited Then
                pIU("ServedQtyIsEdited") = 1 ' no setting back to 0
            End If
            If vDtStocks.Columns.Contains("OrderedQuantity") Then
                pIU("OrderedQuantity") = CDec2(pDR("OrderedQuantity")) ' additional by janine sept 4 2014 for sales order taker
                'pIU("LineCost") = CDec2(pDR("Cost")) * CDec2(pDR("ActualQuantity")) * If(CDec2(pDR("Quantity")) > 0, CDec2(pDR("Quantity")), CDec2(pDR("OrderedQuantity")))
                pIU("LineCost") = CDec2(pDR("Cost")) * If(CDec2(pDR("Quantity")) > 0, CDec2(pDR("Quantity")), CDec2(pDR("OrderedQuantity")))
            Else
                'pIU("LineCost") = CDec2(pDR("Cost")) * CDec2(pDR("ActualQuantity")) * CDec2(pDR("Quantity"))
                pIU("LineCost") = CDec2(pDR("Cost")) * CDec2(pDR("Quantity"))
            End If
            If vDtStocks.Columns.Contains("QuantityBad") Then
                pIU("QuantityBad") = CDec2(pDR("QuantityBad"))
            End If
            If vDtStocks.Columns.Contains("Upcharge") Then
                pIU("Upcharge") = CDec2(pDR("Upcharge"))
            End If
            pIU("ActualQuantity") = CDec2(pDR("ActualQuantity"))
            pIU("UnitMeasure") = ReadyString(pDR("UnitMeasure"))
            'pIU("TermDiscount") = pDR("TermDiscount")
            'pIU("DiscountIfPaidWithIn") = pDR("DiscountIfPaidWithIn")
            'pIU("NetDue") = ReadyString(pDR("NetDue"))
            'pIU("TermNumericValue") = CDec2(NumbersOnly(CStr2(pDR("NetDue"))))
            If pDR.Table.Columns.Contains("TermNumericValue") Then
                pIU("TermNumericValue") = CDec2(pDR("TermNumericValue"))
            End If
            'pIU("IsTermNumeric") = If(IsNumeric(pDR("NetDue")) OrElse CDec(pIU("TermNumericValue")) > 0, 1, 0)
            pIU("UnitPrice") = CDec2(pDR("UnitPrice"))
            If CDec2(pDR("OriginalUnitPrice")) = 0 Then
                pIU("OriginalUnitPrice") = CDec2(pDR("UnitPrice"))
            Else
                pIU("OriginalUnitPrice") = CDec2(pDR("OriginalUnitPrice"))
            End If
            pIU("SubTotal") = CDec2(pDR("Subtotal"))
            pIU("Cost") = CDec2(pDR("Cost"))
            'pIU("COGS") = pDR("Cost") * pIU("ActualQuantity") ' COGS = computed column
            pIU("PriceLevelID") = ReadyString(pDR("PriceLevelID"))
            'pIU("Dimension") = ReadyString(pDR("Dimension")) ' not used in Mobiz
            'pIU("IsReplacement") = CDec2(pDR("IsReplacement")) ' not used in Mobiz
            'pIU("IsHasReplacement") = CDec2(pDR("IsHasReplacement")) ' not used in Mobiz
            'pIU("IsHasBackOrder") = CDec2(pDR("IsHasBackOrder")) ' not used in Mobiz
            ' not used in Mobiz
            'If CStr2(pDR("RefStockID")).Length > 0 Then
            '    pIU("RefStockID") = ReadyString(pDR("RefStockID"))
            'End If
            If vDtStocks.Columns.Contains("DiscountAmount") Then
                If pDR("DiscountAmount") IsNot DBNull.Value Then
                    pIU("DiscountAmount") = CDec2(pDR("DiscountAmount"))
                End If
            End If
            If vDtStocks.Columns.Contains("DiscountSales") Then
                If pDR("DiscountSales") IsNot DBNull.Value Then
                    pIU("DiscountSales") = CDec2(pDR("DiscountSales"))
                End If
            End If
            If vDtStocks.Columns.Contains("RequestedUnitPrice") Then
                If pDR("RequestedUnitPrice") IsNot DBNull.Value Then
                    pIU("RequestedUnitPrice") = CDec2(pDR("RequestedUnitPrice"))
                End If
            End If
            If vDtStocks.Columns.Contains("VendorID") Then
                If CStr2(pDR("VendorID")) <> "" Then
                    pIU("VendorID") = ReadyString(pDR("VendorID"))
                End If
            End If
            'additional by janine
            If vDtStocks.Columns.Contains("VATExemptSale") Then
                If pDR("VATExemptSale") IsNot DBNull.Value Then
                    pIU("VATExemptSale") = CDec2(pDR("VATExemptSale"))
                End If
            End If
            If vDtStocks.Columns.Contains("ZeroRatedSale") Then
                If pDR("ZeroRatedSale") IsNot DBNull.Value Then
                    pIU("ZeroRatedSale") = CDec2(pDR("ZeroRatedSale"))
                End If
            End If
            If vDtStocks.Columns.Contains("NonVATSale") Then
                If pDR("NonVATSale") IsNot DBNull.Value Then
                    pIU("NonVATSale") = CDec2(pDR("NonVATSale"))
                End If
            End If
            If vDtStocks.Columns.Contains("VATableSale") Then
                If pDR("VATableSale") IsNot DBNull.Value Then
                    pIU("VATableSale") = CDec2(pDR("VATableSale"))
                End If
            End If
            If vDtStocks.Columns.Contains("TradeDiscountLess") Then
                pIU("TradeDiscountLess") = ReadyString(pDR("TradeDiscountLess"))
            End If
            If vDtStocks.Columns.Contains("NetDiscountFactor") Then
                pIU("NetDiscountFactor") = CDec2(pDR("NetDiscountFactor"))
            End If
            If vDtStocks.Columns.Contains("NetDiscountRate") Then
                pIU("NetDiscountRate") = CDec2(pDR("NetDiscountRate"))
            End If
            'end of additional
            pIU("UnitGP") = CDec2(pDR("UnitGP"))
            pIU("UnitGPM") = CDec2(pDR("UnitGPM"))
            pIU("LineGP") = CDec2(pDR("LineGP"))
            pIU("LineGPM") = CDec2(pDR("LineGPM"))
            pIU("LineCost") = CDec2(pDR("LineCost"))

            If vDtStocks.Columns.Contains("BinID") Then
                If Not String.IsNullOrWhiteSpace(pDR("BinID").ToString()) Then
                    pIU("BinID") = ReadyString(pDR("BinID"))
                End If
            End If
            If vDtStocks.Columns.Contains("TaxPercent") Then
                pIU("TaxPercent") = CDec2(pDR("TaxPercent"))
            End If
            If vDtStocks.Columns.Contains("TaxAmount") Then
                pIU("TaxAmount") = CDec2(pDR("TaxAmount"))
            End If
            If vDtStocks.Columns.Contains("TaxSales") Then
                pIU("TaxSales") = CDec2(pDR("TaxSales"))
            End If
            If vDtStocks.Columns.Contains("ModifierGroupID") Then
                If pDR("ModifierGroupID") IsNot DBNull.Value Then
                    pIU("ModifierGroupID") = ReadyString(pDR("ModifierGroupID"))
                End If
            End If
            If vDtStocks.Columns.Contains("ThirdPartyProductNumber") Then
                If CStr2(pDR("ThirdPartyProductNumber")) <> "" Then
                    pIU("ThirdPartyProductNumber") = ReadyString(pDR("ThirdPartyProductNumber"))
                End If
            End If

            pSql = pIU.GetInsert
            Try
                mProData.ExecuteSQL(pSql)
            Catch ex As Exception
                Try
                    pSql = pIU.GetUpdate("INSERT")
                Catch ex2 As Exception
                    Throw ex
                End Try
                Throw New ApplicationException(ex.Message & vbCrLf & pSql, ex)
            End Try

            '// inventory ledger
            ' not used in Mobiz
            'If mParent.LibraryStock.AddToTotalStockOut(CStr2(pDR("StockID")), pDR("Quantity") * pDR("ActualQuantity"), oError) = False Then
            '    Return False
            'End If

            ' not used in Mobiz
            'mParent.LibraryStock.SaveIntoInventoryLedger(
            '    vStockID:=CStr2(pDR("StockID")),
            '    vTransaction:=vInvoiceType & " - " & vInvoiceNumber & vInvoiceNoExtension & " " & vCustomerName & " (Invoiced Item)",
            '    vAmount:=pDR("UnitPrice"),
            '    vDebit:=0,
            '    vCredit:=pDR("Quantity") * pDR("ActualQuantity"),
            '    vTransactionType:=LibraryStock.eLedgerTransactionType.LedgerInvoicing,
            '    vReferenceID:=vInvoiceID,
            '    vFreight:=0,
            '    vFactory:=0)
            ' '' ''Dim pSalesRep As Object = pDR("SalesRep")
            ' '' ''If pSalesRep IsNot DBNull.Value Then
            ' '' ''    For Each pStr As String In pSalesRep
            ' '' ''        pIuSalesRep = New InsertUpdate("TransInvoiceItemSalesReps")
            ' '' ''        pIuSalesRep("ID") = "NEWID()"
            ' '' ''        pIuSalesRep("InvoiceItemID") = ReadyString(pID)
            ' '' ''        pIuSalesRep("PeopleID") = ReadyString(pStr)
            ' '' ''        pSql = pIuSalesRep.GetInsert
            ' '' ''        mProData.ExecuteSQL(pSql)
            ' '' ''    Next pStr
            ' '' ''End If

            If String.IsNullOrWhiteSpace(vBackOrderSourceSalesOrderID) = False _
                AndAlso String.IsNullOrWhiteSpace(vBackOrderSourceID) = False _
                AndAlso String.IsNullOrWhiteSpace(vBackOrderInvoiceID) = False Then

                Dim pItemRequisitionID As String
                Dim pMinDate = Now.Date

                ' uncommented if you want to start on the last monday
                'Dim pWeekDay = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(pMinDate) ' 0 = sunday
                'If pWeekDay = DayOfWeek.Sunday Then
                '    pMinDate = pMinDate.AddDays(-6)
                'Else
                '    pMinDate = pMinDate.AddDays(-pWeekDay + 1)
                'End If

                Dim pDrItemRequisition = ItemRequisition(CStr2(pDR("StockID")), 1, vFowstID, pMinDate)
                Dim pIuItemrequisition = New InsertUpdate("TransItemRequisitions")
                If pDrItemRequisition Is Nothing Then
                    pItemRequisitionID = Guid.NewGuid().ToString()
                    pIuItemrequisition("ID") = ReadyString(pItemRequisitionID)
                    pIuItemrequisition("ProductID") = ReadyString(pDR("StockID"))
                    pIuItemrequisition("UnitQty") = 1
                    pIuItemrequisition("UserID") = ReadyString(vUserID)
                    pIuItemrequisition("DateRequested") = "GETDATE()"
                    pIuItemrequisition("RequestedQty") = 0
                    pIuItemrequisition("BackorderQty") = CDec2(pDR("OrderedQuantity")) * CDec2(pDR("ActualQuantity"))
                    pIuItemrequisition("RequestedQtyTotal") = CDec2(pDR("OrderedQuantity")) * CDec2(pDR("ActualQuantity"))
                    Dim pDrBaseUnit = mParent.LibraryStock.GetFirstUnit(CStr2(pDR("StockID")))
                    If pDrBaseUnit IsNot Nothing _
                        AndAlso CDec2(pDrBaseUnit("Quantity")) = 1 Then

                        pIuItemrequisition("UnitMeasure") = ReadyString(pDrBaseUnit("UnitMeasure"))
                    End If
                    pSql = pIuItemrequisition.GetInsert()
                Else
                    pItemRequisitionID = CStr2(pDrItemRequisition("ID"))
                    pIuItemrequisition("RequestedQtyTotal") = pDrItemRequisition("RequestedQtyTotal") + CDec2(pDR("OrderedQuantity")) * CDec2(pDR("ActualQuantity"))
                    pIuItemrequisition("BackorderQty") = CDec2(pDrItemRequisition("BackorderQty")) + CDec2(pDR("OrderedQuantity")) * CDec2(pDR("ActualQuantity"))
                    pSql = pIuItemrequisition.GetUpdate("ID = " & ReadyString(pDrItemRequisition("ID")))
                End If
                mProData.ExecuteSQL(pSql)

                Dim pIuItemRequisitionBackOrder = New InsertUpdate("TransItemRequisitionBackOrder")
                pIuItemRequisitionBackOrder("ID") = "NEWID()"
                pIuItemRequisitionBackOrder("ItemRequisitionID") = ReadyString(pItemRequisitionID)
                pIuItemRequisitionBackOrder("BackOrderSourceSalesOrderID") = ReadyString(vBackOrderSourceSalesOrderID)
                pIuItemRequisitionBackOrder("BackOrderSourceID") = ReadyString(vBackOrderSourceID)
                pIuItemRequisitionBackOrder("BackOrderInvoiceID") = ReadyString(vBackOrderInvoiceID)
                pIuItemRequisitionBackOrder("IncludeRow") = 1
                pSql = pIuItemRequisitionBackOrder.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
        Next pDR

        '// TransStockSortingListItems
        'If CStr2(vStockSortingListID).Length > 0 Then
        '    pSql = "UPDATE TransStockSortingListItems" & _
        '        " SET IsInvoiced = 1" & _
        '        " WHERE StockSortingListID = " & ReadyString(vStockSortingListID)
        '    mProData.ExecuteSQL(pSql)
        'End If

        Return True

    End Function

    Private Function SaveSalesRepresentative(ByVal vInvoiceID As String, ByVal vProductID As String, ByVal vSalesRepList As Object()) As Boolean
        Dim pSql As String
        Dim pID As String
        Dim pIuSalesRep As InsertUpdate
        Dim strIDRec As String

        pSql = "DELETE " &
               " FROM TransInvoiceItemSalesReps " &
               " WHERE InvoiceID " & IIf(vInvoiceID.Trim().Length = 0, "IS NULL", " = " & ReadyString(vInvoiceID.Trim())) &
               " AND ProductID " & IIf(vProductID.Trim().Length = 0, "IS NULL", " = " & ReadyString(vProductID.Trim()))
        mProData.ExecuteSQL(pSql)

        For Each vObject As Object In vSalesRepList
            If (vObject("RecState").ToString().Trim().Length > 0) Then
                If (vObject("RecState").ToString().Trim() <> "Remove") Then

                    pIuSalesRep = New InsertUpdate("TransInvoiceItemSalesReps")

                    pIuSalesRep("InvoiceID") = ReadyString(vInvoiceID)
                    pIuSalesRep("ProductID") = ReadyString(vObject("ProductID"))
                    pIuSalesRep("PeopleID") = ReadyString(vObject("SalesRepID"))

                    strIDRec = Guid.NewGuid.ToString()
                    pIuSalesRep("ID") = ReadyString(strIDRec)
                    pSql = pIuSalesRep.GetInsert()

                    mProData.ExecuteSQL(pSql)

                End If

            End If

        Next vObject

        Return True

    End Function

    Private Function SaveSalesProductSerial(ByVal vInvoiceID As String, ByVal vProductID As String, ByVal vProductSerialsList As Object()) As Boolean
        Dim pSql As String
        Dim pID As String
        Dim pIuProductSerial As InsertUpdate
        Dim strIDRec As String

        pSql = "DELETE " &
               " FROM TransInvoiceItemSerials " &
               " WHERE InvoiceID " & IIf(vInvoiceID.Trim().Length = 0, "IS NULL", " = " & ReadyString(vInvoiceID.Trim())) &
               " AND ProductID " & IIf(vProductID.Trim().Length = 0, "IS NULL", " = " & ReadyString(vProductID.Trim()))
        mProData.ExecuteSQL(pSql)

        For Each vObject As Object In vProductSerialsList
            If (vObject("RecState").ToString().Trim().Length > 0) Then
                If (vObject("RecState").ToString().Trim() <> "Remove") Then

                    pIuProductSerial = New InsertUpdate("TransInvoiceItemSerials")

                    pIuProductSerial("InvoiceID") = ReadyString(vInvoiceID)
                    pIuProductSerial("ProductID") = ReadyString(vObject("ProductID"))
                    pIuProductSerial("SerialNumber") = ReadyString(vObject("ProductSerial"))

                    strIDRec = Guid.NewGuid.ToString()
                    pIuProductSerial("ID") = ReadyString(strIDRec)
                    pSql = pIuProductSerial.GetInsert()

                    mProData.ExecuteSQL(pSql)

                End If

            End If

        Next vObject

        Return True

    End Function

    Public Function IsInvoiceExist(ByVal vInvoiceDefaultID As String, ByVal vInvoicNumber As Long, ByVal vInvoiceNumberExtension As String,
    ByRef oError As String) As Boolean

        Dim strSQL As String
        Dim rsTmp As DataTable
        Dim rsTmp2 As DataTable

        IsInvoiceExist = False

        oError = ""
        IsInvoiceExist = False

        'rsTmp = New ADODB.Recordset
        strSQL = "SELECT ID," &
            "StartNumber," &
            "EndNumber," &
            "IsRestartCount," &
            "Bracket " &
            " FROM MainInvoiceDefaults " &
            " WHERE ID='" & vInvoiceDefaultID & "'"
        rsTmp = mProData.Execute(strSQL)

        'rsTmp.Open(strSQL, DBConnection, adOpenStatic)

        If rsTmp.Rows.Count > 0 Then

            'IsInvoiceExceed = False

            'rsTmp2 = New ADODB.Recordset

            strSQL = "SELECT InvoiceDefaultID," &
                "InvoiceNumber," &
                "BracketNumber " &
                " FROM TransInvoice " &
                " WHERE InvoiceDefaultID ='" & vInvoiceDefaultID & "' " &
                " AND InvoiceNumber = " & vInvoicNumber &
                " AND InvoiceNoExtension = '" & Replace(Trim(vInvoiceNumberExtension), "'", "''") & "' "
            ' AND BracketNumber = " & rsTmp!Bracket
            'rsTmp2 = GetRS(strSQL)
            rsTmp2 = mProData.Execute(strSQL)

            If rsTmp2.Rows.Count > 0 Then
                oError = "Invoice number already exist"
                IsInvoiceExist = True
            Else
                strSQL = "SELECT OrderNo" &
                    " FROM TransSalesOrder" &
                    " WHERE PendingInvoiceNum = " & ReadyString(vInvoicNumber) &
                    " AND PendingInvoiceType = " & ReadyString(vInvoiceDefaultID) &
                    " AND IsCancelled = 0"
                rsTmp2.Dispose()
                rsTmp2 = mProData.Execute(strSQL)
                If rsTmp2.Rows.Count Then
                    oError = "Invoice Number is already assigned to in-store Sales Order Number " & rsTmp2.Rows(0)("OrderNo") & ". Please enter another invoice number or cancel that sales order."
                    IsInvoiceExist = True
                    Exit Function
                End If

                'Check if invoice reach the max invoice number count
                If vInvoicNumber > (rsTmp.Rows(0)("EndNumber")) Then
                    If CDec2(rsTmp.Rows(0)("IsRestartCount")) = 0 Then
                        oError = "Invoice Number exceed the max invoice number count."
                        IsInvoiceExist = True
                        Exit Function
                    Else
                        oError = "Invoice Number exceed the max invoice number count."
                        IsInvoiceExist = True
                        'IsInvoiceExceed = True
                        Exit Function
                        '                    strSQL = "Update MainInvoiceDefaults Set " & _
                        '                        "Bracket=" & rsTmp!Bracket + 1 & _
                        '                        " WHERE ='" & vInvoiceDefaultID & "'"
                        '                    DBConnection.Execute strSQL
                    End If
                End If
            End If
        End If

    End Function

    Public Function GetSummary(ByVal vYear1 As Integer, ByVal vYear2 As Integer, ByVal vStockIds As String, ByVal vGroupByStock As Boolean) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pWhere As String

        If CStr2(vStockIds).Length > 0 Then
            pWhere = " AND LibraryStockInformation.ID IN " & vStockIds
        End If

        '// cash slip
        pSql = " SELECT POSCashSlipSalesItems.StockID" &
            ",SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) AS QtySold" &
            ",SUM(POSCashSlipSalesItems.SubTotal) AS Amount" &
            ",SUM(POSCashSlipSalesItems.COGS) AS COGS" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",dbo.FormatDate(POSCashSlipSales.LastUpdated, 'YYYmm') AS YearMonth" &
            ",DATENAME(month, POSCashSlipSales.LastUpdated) AS DateMonth" &
            ",DATEPART(year, POSCashSlipSales.LastUpdated) AS YearValue" &
            ",DATEPART(month, POSCashSlipSales.LastUpdated) AS MonthValue" &
            " " &
            " FROM POSCashSlipSalesItems" &
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSCashSlipSalesItems.StockID" &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " &
            " " &
            " WHERE (DATEPART(year, POSCashSlipSales.LastUpdated) = " & vYear1 &
            "     OR DATEPART(year, POSCashSlipSales.LastUpdated) = " & vYear2 & ")" &
            pWhere &
            " " &
            " GROUP BY POSCashSlipSalesItems.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",dbo.FormatDate(POSCashSlipSales.LastUpdated, 'YYYmm')" &
            ",DATENAME(month, POSCashSlipSales.LastUpdated)" &
            ",DATEPART(year, POSCashSlipSales.LastUpdated)" &
            ",DATEPART(month, POSCashSlipSales.LastUpdated)" &
            " " &
            " HAVING SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) > 0"

        '// POS
        pSql &= " UNION ALL "
        pSql &= " SELECT POSSalesItems.StockID" &
            ",SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) AS QtySold" &
            ",SUM(POSSalesItems.SubTotal) AS Amount" &
            ",SUM(POSSalesItems.COGS) AS COGS" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",dbo.FormatDate(POSSales.LastUpdated, 'YYYmm') AS YearMonth" &
            ",DATENAME(month, POSSales.LastUpdated) AS DateMonth" &
            ",DATEPART(year, POSSales.LastUpdated) AS YearValue" &
            ",DATEPART(month, POSSales.LastUpdated) AS MonthValue" &
            " " &
            " FROM POSSalesItems" &
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSSalesItems.StockID" &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " &
            " " &
            " WHERE (DATEPART(year, POSSales.LastUpdated) = " & vYear1 &
            "     OR DATEPART(year, POSSales.LastUpdated) = " & vYear2 & ")" &
            pWhere &
            " " &
            " GROUP BY POSSalesItems.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",dbo.FormatDate(POSSales.LastUpdated, 'YYYmm')" &
            ",DATENAME(month, POSSales.LastUpdated)" &
            ",DATEPART(year, POSSales.LastUpdated)" &
            ",DATEPART(month, POSSales.LastUpdated)" &
            " " &
            " HAVING SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) > 0"

        '// invoice
        pSql &= " UNION ALL "
        pSql &= " SELECT TransInvoiceItems.StockID" &
            ",SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) AS QtySold" &
            ",SUM(TransInvoiceItems.SubTotal) AS Amount" &
            ",SUM(TransInvoiceItems.COGS) AS COGS" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",dbo.FormatDate(TransInvoice.LastUpdated, 'YYYmm') AS YearMonth" &
            ",DATENAME(month, TransInvoice.LastUpdated) AS DateMonth" &
            ",DATEPART(year, TransInvoice.LastUpdated) AS YearValue" &
            ",DATEPART(month, TransInvoice.LastUpdated) AS MonthValue" &
            " " &
            " FROM TransInvoiceItems" &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransInvoiceItems.StockID" &
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " &
            " LEFT JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" &
            " " &
            " WHERE (DATEPART(year, TransInvoice.LastUpdated) = " & vYear1 &
            "     OR DATEPART(year, TransInvoice.LastUpdated) = " & vYear2 & ")" &
            " AND LibraryCustomers.IsCompanyBranch = 0" &
            " AND TransInvoice.IsCancelled = 0" &
            pWhere &
            " " &
            " GROUP BY TransInvoiceItems.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",dbo.FormatDate(TransInvoice.LastUpdated, 'YYYmm')" &
            ",DATENAME(month, TransInvoice.LastUpdated)" &
            ",DATEPART(year, TransInvoice.LastUpdated)" &
            ",DATEPART(month, TransInvoice.LastUpdated)" &
            " " &
            " HAVING SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) > 0"

        'pSql &= " ORDER BY ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
        '    " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')"

        If vGroupByStock Then
            pSql = "SELECT *" &
                ",(SELECT TOP 1 LibraryUnitMeasures.UnitMeasure" &
                "   FROM LibraryStockMeasurements" &
                "   JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" &
                "   WHERE LibraryStockMeasurements.StockID = t.StockID" &
                "   ORDER BY LibraryStockMeasurements.Quantity) AS UnitMeasure" &
                " FROM (" & pSql & ") t"
        Else
            pSql = "SELECT SUM(QtySold) AS QtySold" &
                ",SUM(Amount) AS Amount" &
                ",SUM(COGS) AS COGS" &
                ",YearMonth" &
                ",YearValue" &
                ",MonthValue" &
                " FROM (" & pSql & ") t" &
                " GROUP BY YearMonth" &
                ",YearValue" &
                ",MonthValue" &
                " ORDER BY YearMonth"
        End If

        mProData.CommandTimeout = 10 * 60 * 1000
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <param name="vSupplierID">if nothing, include all suppliers</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSummaryPerSupplierAndBranch(ByVal vDateFrom As Date, ByVal vDateTo As Date,
        Optional ByVal vSupplierID As String = Nothing,
        Optional ByVal vShowCustomer As Boolean = False,
        Optional ByVal vIncludeAllSupplierStocks As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pWhere As String

        If CStr2(vSupplierID).Length = 0 Then
            pWhere = " AND LibrarySuppliers.SendEmail = 1 OR LibrarySupplierStocks.SendEmail = 1" & vbCrLf &
                " AND LEN(LTRIM(RTRIM(ISNULL(LibrarySuppliers.EmailAddress, '')))) > 0" & vbCrLf &
                " AND LibrarySuppliers.IsActive = 1" & vbCrLf
        Else
            pWhere = " AND LibrarySuppliers.ID = " & ReadyString(vSupplierID) & vbCrLf
        End If

        '// cash slip
        pSql = " SELECT POSCashSlipSalesItems.StockID" & vbCrLf &
            ",SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) AS QtySold" & vbCrLf &
            ",SUM(POSCashSlipSalesItems.UnitPrice * POSCashSlipSalesItems.Quantity) AS Amount" &
            ",POSCashSlipSales.BranchID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",LibrarySuppliers.SupplierName" &
            ",LibrarySupplierStocks.SupplierID" &
            ",LibrarySuppliers.EmailAddress"
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName"
        End If
        pSql &= " FROM POSCashSlipSalesItems" &
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSCashSlipSalesItems.StockID" &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch ON tblBranch.ID = POSCashSlipSales.BranchID" &
            " " & If(vIncludeAllSupplierStocks, "LEFT ", "") & "JOIN LibrarySupplierStocks ON LibrarySupplierStocks.StockID = LibraryStockInformation.ID" &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID"
        If vShowCustomer Then
            pSql &= " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = POSCashSlipSales.CustomerID"
        End If
        pSql &= " WHERE POSCashSlipSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            pWhere &
            " " &
            " GROUP BY POSCashSlipSalesItems.StockID" &
            ",POSCashSlipSales.BranchID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" &
            ",LibrarySuppliers.SupplierName" &
            ",LibrarySupplierStocks.SupplierID" &
            ",LibrarySuppliers.EmailAddress"
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName"
        End If
        pSql &= " HAVING SUM(POSCashSlipSalesItems.ActualQuantity * POSCashSlipSalesItems.Quantity) > 0"

        '// POS
        pSql &= " UNION ALL " & vbCrLf
        pSql &= " SELECT POSSalesItems.StockID" & vbCrLf &
            ",SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) AS QtySold" & vbCrLf &
            ",SUM(POSSalesItems.UnitPrice * POSSalesItems.Quantity) AS Amount" & vbCrLf &
            ",POSSales.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName"
        End If

        pSql &= " FROM LibrarySupplierStocks" & vbCrLf &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID" & vbCrLf &
            " JOIN POSSalesItems ON POSSalesItems.StockID = LibrarySupplierStocks.StockID" &
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" & vbCrLf &
            " AND POSSales.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & vbCrLf &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = POSSalesItems.StockID" &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch ON tblBranch.ID = POSSales.BranchID"
        If vShowCustomer Then
            pSql &= " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = POSSales.CustomerID"
        End If
        pSql &= " WHERE 1 = 1 " & pWhere & vbCrLf & vbCrLf &
            " GROUP BY POSSalesItems.StockID" & vbCrLf &
            ",POSSales.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName" & vbCrLf
        End If
        pSql &= " HAVING SUM(POSSalesItems.ActualQuantity * POSSalesItems.Quantity) > 0" & vbCrLf

        '// invoice
        pSql &= " UNION ALL " & vbCrLf
        pSql &= " SELECT TransInvoiceItems.StockID" & vbCrLf &
            ",SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) AS QtySold" & vbCrLf &
            ",SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS Amount" & vbCrLf &
            ",TransSalesOrder.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName" & vbCrLf
        End If
        pSql &= " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransInvoiceItems.StockID" & vbCrLf &
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf &
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & vbCrLf &
            " LEFT JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" & vbCrLf &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch ON tblBranch.ID = TransSalesOrder.BranchID" & vbCrLf &
            " JOIN LibrarySupplierStocks ON LibrarySupplierStocks.StockID = LibraryStockInformation.ID" & vbCrLf &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID" & vbCrLf
        'If vShowCustomer Then  ' no need for this condition
        pSql &= " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf
        'End If
        pSql &= " WHERE TransInvoice.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & vbCrLf &
            " AND LibraryCustomers.IsCompanyBranch = 0" & vbCrLf &
            pWhere &
            " " & vbCrLf &
            " GROUP BY TransInvoiceItems.StockID" & vbCrLf &
            ",TransSalesOrder.BranchID" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf &
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" & vbCrLf &
            ",LibrarySuppliers.SupplierName" & vbCrLf &
            ",LibrarySupplierStocks.SupplierID" & vbCrLf &
            ",LibrarySuppliers.EmailAddress" & vbCrLf
        If vShowCustomer Then
            pSql &= ",LibraryCustomers.CompanyName" & vbCrLf
        End If
        pSql &= " HAVING SUM(TransInvoiceItems.ActualQuantity * TransInvoiceItems.Quantity) > 0" & vbCrLf

        pSql = "SELECT *" & vbCrLf &
            ",(SELECT TOP 1 LibraryUnitMeasures.UnitMeasure" & vbCrLf &
            "   FROM LibraryStockMeasurements" & vbCrLf &
            "   JOIN LibraryUnitMeasures ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" & vbCrLf &
            "   WHERE LibraryStockMeasurements.StockID = t.StockID" & vbCrLf &
            "   ORDER BY LibraryStockMeasurements.Quantity) AS UnitMeasure" & vbCrLf &
            " FROM (" & pSql & ") t" & vbCrLf

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSummaryPerCustomerAndAgent(
        ByVal vOldDateFrom As Date, ByVal vOldDateTo As Date,
        ByVal vNewDateFrom As Date, ByVal vNewDateTo As Date) As DataTable

        Dim pSql As String

        pSql = "SELECT * FROM (" &
        GetSummaryPerCustomerAndAgentSubSql(vOldDateFrom, vOldDateTo) & " AS OldTotal," &
        GetSummaryPerCustomerAndAgentSubSql(vNewDateFrom, vNewDateTo) & " AS NewTotal" &
        " " &
        " 	,(SELECT TOP 1 LibraryProvinces.Name" &
        "       FROM LibraryCustomersAddresses" &
        "       JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" &
        "       JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" &
        "       JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" &
        "       JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" &
        "       WHERE LibraryCustomersAddresses.CustomerID = LibraryCustomers.ID" &
        "       ORDER BY LibraryCustomersAddresses.IsBillingAddress DESC" &
        "       ,LibraryProvinces.Name	" &
        "       ,LibraryCities.Name" &
        "       ,LibraryDistricts.Name" &
        "       ,LibraryStreetNames.StreetName" &
        " 	) AS Location      " &
        ",LibraryCustomers.CompanyName" &
        ",EmployeeProfiles.LastName + ', ' + EmployeeProfiles.FirstName + ' ' + EmployeeProfiles.MiddleName AS Agent" &
        ",EmployeeProfiles.EmailAddress AS AgentEmail" &
        ",LibraryCustomersAgents.EmployeeID" &
        "" &
        " FROM LibraryCustomers" &
        " JOIN LibraryCustomersAgents ON LibraryCustomersAgents.CustomerID = LibraryCustomers.ID" &
        " JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles ON EmployeeProfiles.ID = LibraryCustomersAgents.EmployeeID" &
        " ) T" &
        " WHERE Total > 0" &
        " ORDER BY CompanyName"

    End Function

    Private Function GetSummaryPerCustomerAndAgentSubSql(ByVal vDateFrom As Date, ByVal vDateTo As Date) As String

        Return "(SELECT SUM(TransInvoiceItems.SubTotal)" &
            " FROM TransInvoiceItems" &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" &
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" &
            " WHERE ISNULL(TransInvoice.PerfectOrder, 0) = 0" &
            " AND TransInvoice.IsCancelled = 0" &
            " AND TransInvoice.CustomerID = LibraryCustomers.ID" &
            " AND MainInvoiceDefaults.Billable = 1" &
            " AND CONVERT(datetime, FLOOR(CONVERT(float, TransInvoice.InvoiceDate))) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateFrom.Date) &
            ")+ " &
            " (SELECT SUM(TransSalesOrderItems.SubTotal)" &
            " FROM TransSalesOrderItems" &
            " JOIN TransInvoice ON TransInvoice.SalesOrderID = TransSalesOrderItems.SalesOrderID" &
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" &
            " WHERE TransInvoice.PerfectOrder = 1" &
            " AND TransInvoice.IsCancelled = 0" &
            " AND TransInvoice.CustomerID = LibraryCustomers.ID" &
            " AND MainInvoiceDefaults.Billable = 1    " &
            " AND CONVERT(datetime, FLOOR(CONVERT(float, TransInvoice.InvoiceDate))) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateFrom.Date) &
            ")"

    End Function

    Public Function GetInvoiceID(ByVal vInvoiceTypeAndNumber As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        Do Until vInvoiceTypeAndNumber = vInvoiceTypeAndNumber.Replace("  ", " ")
            vInvoiceTypeAndNumber = vInvoiceTypeAndNumber.Replace("  ", " ")
        Loop

        pSql = "SELECT TransInvoice.ID" &
            ",SalesOrderID" &
            " FROM TransInvoice" &
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" &
            " WHERE Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') = " & ReadyString(vInvoiceTypeAndNumber)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT
        End If

        pSql = "SELECT TransInvoice.ID" &
            ",SalesOrderID" &
            " FROM TransInvoice" &
            " JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" &
            " WHERE Abbrevation + ' - ' + CONVERT(varchar, TransInvoice.InvoiceNumber) + ISNULL(TransInvoice.InvoiceNoExtension,'') = " & ReadyString(vInvoiceTypeAndNumber)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetPrintableInvoice(ByVal vBranchInvoiceDefaultID As String, ByVal vCustomerInvoiceDefaultID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 COUNT(*)" & vbCrLf &
            " FROM TransInvoicePrints" & vbCrLf &
            " WHERE InvoiceID = TransInvoice.ID" & vbCrLf &
            " AND ApprovedBy IS NOT NULL" & vbCrLf &
            " AND DeniedBy IS NULL"
        ' the below code has been excluded from the code above
        ' " AND DatePrinted IS NULL" 

        pSql = "SELECT TransInvoice.ID" & vbCrLf &
            ",MainInvoiceDefaults.Abbrevation + ' - ' + RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) + ISNULL(TransInvoice.InvoiceNoExtension,'') AS InvoiceNumber" & vbCrLf &
            ",LibraryCustomers.CompanyName" & vbCrLf &
            ",TransInvoice.NumberOfPrints" & vbCrLf &
            ",MainInvoiceDefaults.NumberOfReprints" & vbCrLf &
            ",ISNULL((" & pSql & "), 0) AS ApprovedRequests" & vbCrLf &
            ",TransInvoice.LastUpdated" & vbCrLf &
            ",TransInvoice.CustomerID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf &
            " JOIN MainInvoiceDefaults ON TransInvoice.InvoiceDefaultID = MainInvoiceDefaults.ID" & vbCrLf &
            " WHERE TransInvoice.InvoiceDefaultID IN (" & ReadyString(vBranchInvoiceDefaultID) & ", " & ReadyString(vCustomerInvoiceDefaultID) & ")" & vbCrLf &
            " AND TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND (SELECT SUM(Quantity) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID) >= 1" & vbCrLf &
            " AND TransInvoice.LastUpdated > '10/8/2011'" & vbCrLf &
            " AND ISNULL(TransInvoice.DontAutoPrint, 0) = 0"

        pSql = "SELECT * FROM (" & pSql & ") t" & vbCrLf &
            " WHERE NumberOfPrints <= NumberOfReprints + ApprovedRequests" & vbCrLf &
            " ORDER BY LastUpdated"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    ' this is the legacy code
    '    Public Function IncrementPrintCount(ByVal vID As String) As Boolean

    '        Dim pDT As DataTable
    '        Dim pSql As String
    '        Dim pIU As InsertUpdate

    '        Try
    '            Try
    '                mProData.BeginTransaction()
    '            Catch ex As Exception
    '                Throw
    '            End Try

    '            pIU = New InsertUpdate
    '            pIU.TableName = "TransInvoice"
    '            pIU("NumberOfPrints") = "NumberOfPrints + 1"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
    '            mProData.ExecuteSQL(pSql)

    '            pSql = "SELECT TOP 1 ID" & _
    '                " FROM TransInvoicePrints" & _
    '                " WHERE InvoiceID = " & ReadyString(vID) & _
    '                " AND ApprovedBy IS NOT NULL" & _
    '                " AND DatePrinted IS NULL" & _
    '                " AND DeniedBy IS NULL" & _
    '                " ORDER BY DateRequested"
    '            pDT = mProData.Execute(pSql)
    '            If pDT.Rows.Count > 0 Then
    '                pIU = New InsertUpdate
    '                pIU.TableName = "TransInvoicePrints"
    '                pIU("DatePrinted") = "GETDATE()"
    '#If _MYTYPE = "Web" Then
    '                pIU("PrintedBy") = WebSession.Instance.EmployeeID
    '#Else
    '                pIU("PrintedBy") = User.EmployeeID
    '#End If
    '                pSql = pIU.GetUpdate("ID = " & ReadyString(pDT.Rows(0)("ID")))
    '                mProData.ExecuteSQL(pSql)
    '            End If
    '            mProData.CommitTransaction()
    '        Catch ex As Exception
    '            Throw
    '        End Try

    '        Return True

    '    End Function

    Public Sub TagTransAsFollowUp(ByVal vID As String)
        'If mag follow up og Price Quote, etc
        'for DRAFT only
        Dim pIU As InsertUpdate
        Dim pSql As String

        pSql = "SELECT * FROM TransInvoice WHERE ID = " & ReadyString(vID)
        Dim pRows As Object = mProData.Execute(pSql).Rows(0)
        Dim isDraft As Boolean = False

        If CStr2(pRows("TransType")) = "PurchaseOrder" Then
            isDraft = IIf(CInt2(pRows("PurchaseOrderNo")) = 0, True, False)
        End If

        If isDraft Then
            pIU = New InsertUpdate("TransInvoice")
            pIU("IsForFollowUp") = 1
            pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
            mProData.ExecuteSQL(pSql)
        End If
    End Sub

    Public Sub IncrementPrintCount(ByVal vID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoice")
        pIU("PrintCount") = "ISNULL(PrintCount, 0) + 1"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub IncrementWithdrawalSlipPrintCount(ByVal vID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoice")
        pIU("WithdrawalSlipPrintCount") = "ISNULL(WithdrawalSlipPrintCount, 0) + 1"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub IncrementPickListPrintCount(ByVal vID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoice")
        pIU("PickListPrintCount") = "ISNULL(PickListPrintCount, 0) + 1"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    ' not used in Mobiz
    'Public Sub ReplaceItems(ByVal vInvoiceID As String, ByVal vSortingListID As String)

    '    Dim pDT As DataTable
    '    Dim pSql As String
    '    Dim pDR As DataRow
    '    Dim pError As String
    '    Dim pIU As InsertUpdate
    '    Dim pTotalAmount As Decimal
    '    Dim pCost As String
    '    Dim pGP, pGPM As Decimal

    '    pSql = "SELECT TransInvoice.InvoiceNumber" & vbCrLf & _
    '        ",TransInvoice.InvoiceNoExtension" & vbCrLf & _
    '        ",MainInvoiceDefaults.Abbrevation" & vbCrLf & _
    '        ",LibraryCustomers.CompanyName" & vbCrLf & _
    '        " FROM TransInvoice" & vbCrLf & _
    '        " LEFT JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & vbCrLf & _
    '        " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = TransInvoice.CustomerID" & vbCrLf & _
    '        " WHERE TransInvoice.ID = " & ReadyString(vInvoiceID)
    '    pDT = mProData.Execute(pSql)
    '    If pDT.Rows.Count <> 1 Then Exit Sub
    '    pDR = pDT.Rows(0)

    '    mProData.BeginTransaction()

    '    '// delete all items 
    '    pSql = "DELETE FROM TransInvoiceItems WHERE InvoiceID = " & ReadyString(vInvoiceID)
    '    mProData.ExecuteSQL(pSql)

    '    pDT = GetSortingListItems(vSortingListID, False)
    '    SaveStocks(vInvoiceID, pDT, pDR("Abbrevation") & "", pDR("InvoiceNumber") & "", pDR("InvoiceNoExtension") & "", pDR("CompanyName") & "", vSortingListID, pError)

    '    pTotalAmount = CDec2(pDT.Compute("SUM(Subtotal)", ""))
    '    pCost = CDec2(pDT.Compute("SUM(LineCost)", ""))

    '    pGP = pTotalAmount - pCost
    '    If pTotalAmount = 0 Then
    '        If pGP > 0 Then
    '            pGPM = 1
    '        ElseIf pGP < 0 Then
    '            pGPM = -1
    '        Else
    '            pGPM = 0
    '        End If
    '    Else
    '        pGPM = pGP / pTotalAmount
    '    End If
    '    pIU = New InsertUpdate("TransInvoice")
    '    pIU("TotalAmount") = pTotalAmount
    '    pIU("GP") = pGP
    '    pIU("GPM") = pGPM
    '    pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceID))
    '    mProData.ExecuteSQL(pSql)

    '    mProData.CommitTransaction()

    'End Sub

#Region "QtyAmt"
    Private Const mDateFormat As String = "MMM yyyy"

    Public Function QtyAmtExcel(ByVal vDateFrom As Date, ByVal vDateTo As Date, ByVal vSupplierID As String,
        ByVal vSupplierName As String, Optional ByVal vShowCustomer As Boolean = False) As Workbook

        '// get data
        Dim pQuantityColumns As List(Of DataColumn)
        Dim pAmountColumns As List(Of DataColumn)
        Dim pDates As List(Of Date)
        Dim pDT As DataTable
        Dim pDtBranches As DataTable
        Dim pWB As Workbook

        pWB = New Workbook

        pDT = GetData(vDateFrom, vDateTo, vSupplierID, pQuantityColumns, pAmountColumns, pDates, pDtBranches, vShowCustomer)

        '// generate excel
        Dim pWS As Worksheet
        Dim pDrBranch As DataRow
        Dim pRowIndex As Integer
        Dim p As Integer
        Dim pDtCustomers As DataTable
        Dim pDrvCustomer As DataRowView
        Dim pDV As DataView

        pWS = pWB.Worksheets(0)
        pDT.DefaultView.Sort = "StockDescription"

        pWS.Cells(pRowIndex, 0).PutValue("Amount and Quantity Sold by Supplier")
        pRowIndex += 1
        pWS.Cells(pRowIndex, 0).PutValue("Supplier:")
        pWS.Cells(pRowIndex, 1).PutValue(vSupplierName)

        pRowIndex += 1
        pWS.Cells(pRowIndex, 0).PutValue(Format(vDateFrom, cGlobal.FormatDate) & " to " & Format(vDateTo, cGlobal.FormatDate) & " Sales")

        For Each pDrBranch In pDtBranches.Rows
            pDT.DefaultView.RowFilter = "BranchID = " & pDrBranch("BranchID")
            If pDT.DefaultView.Count > 0 Then
                pRowIndex += 2
                pWS.Cells(pRowIndex, 0).PutValue("Branch:")
                pWS.Cells(pRowIndex, 1).PutValue(pDrBranch("Name"))

                '// merged headers
                pRowIndex += 1
                pWS.Cells(pRowIndex, 2).PutValue("Amount")
                pWS.Cells.Merge(pRowIndex, 2, 1, pDates.Count + 1)
                pWS.Cells(pRowIndex, 2 + pDates.Count + 1).PutValue("Quantity")
                pWS.Cells.Merge(pRowIndex, 2 + pDates.Count + 1, 1, pDates.Count + 1)

                '// headers
                pRowIndex += 1
                pWS.Cells(pRowIndex, 0).PutValue("Stock No.")
                pWS.Cells(pRowIndex, 1).PutValue("Stock Description")
                For p = 0 To pDates.Count - 1
                    pWS.Cells(pRowIndex, 2 + p).PutValue(Format(pDates(p), mDateFormat))
                    pWS.Cells(pRowIndex, 2 + p + pDates.Count + 1).PutValue(Format(pDates(p), mDateFormat))
                Next p
                pWS.Cells(pRowIndex, 2 + pDates.Count).PutValue("Total Amount")
                pWS.Cells(pRowIndex, 2 + pDates.Count * 2 + 1).PutValue("Total Quantity")

                If vShowCustomer Then
                    pDtCustomers = pDT.DefaultView.ToTable(True, "CompanyName")
                    pDtCustomers.DefaultView.Sort = "CompanyName"
                    For Each pDrvCustomer In pDtCustomers.DefaultView
                        pRowIndex += 1
                        pWS.Cells(pRowIndex, 0).PutValue("Customer:")
                        pWS.Cells(pRowIndex, 0).Style.Font.IsBold = True
                        pWS.Cells(pRowIndex, 1).PutValue(CStr2(pDrvCustomer("CompanyName")))
                        pWS.Cells(pRowIndex, 1).Style.Font.IsBold = True
                        pDV = New DataView(pDT, pDT.DefaultView.RowFilter & " AND ISNULL(CompanyName, '') = " & ReadyString(pDrvCustomer("CompanyName")), "StockDescription", DataViewRowState.CurrentRows)
                        RenderItems(pWS, pDV, pDates, pRowIndex)
                    Next pDrvCustomer
                Else
                    pDV = pDT.DefaultView
                    RenderItems(pWS, pDV, pDates, pRowIndex)
                End If

                '// branch totals
                pRowIndex += 1
                pWS.Cells(pRowIndex, 1).PutValue("Branch Total:")
                For p = 0 To pDates.Count - 1
                    If pDT.Columns.Contains(Format(pDates(p), mDateFormat) & " - Amount") Then
                        pWS.Cells(pRowIndex, 2 + p).PutValue(pDT.Compute("SUM([" & Format(pDates(p), mDateFormat) & " - Amount" & "])", "BranchID = " & pDrBranch("BranchID")))
                        pWS.Cells(pRowIndex, 2 + p).Style.Custom = cGlobal.FormatCurrencyGrid
                        pWS.Cells(pRowIndex, 2 + p + pDates.Count + 1).PutValue(pDT.Compute("SUM([" & Format(pDates(p), mDateFormat) & " - Quantity" & "])", "BranchID = " & pDrBranch("BranchID")))
                        pWS.Cells(pRowIndex, 2 + p + pDates.Count + 1).Style.Custom = cGlobal.FormatIntegerAccounting
                    End If
                Next p
                pWS.Cells(pRowIndex, 2 + pDates.Count).PutValue(pDT.Compute("SUM(TotalAmount)", "BranchID = " & pDrBranch("BranchID")))
                pWS.Cells(pRowIndex, 2 + pDates.Count).Style.Custom = cGlobal.FormatCurrencyGrid
                pWS.Cells(pRowIndex, 2 + pDates.Count * 2 + 1).PutValue(pDT.Compute("SUM(TotalQty)", "BranchID = " & pDrBranch("BranchID")))
                pWS.Cells(pRowIndex, 2 + pDates.Count * 2 + 1).Style.Custom = cGlobal.FormatIntegerAccounting
            End If
        Next pDrBranch

        pWS.AutoFitColumns()
        'pWB.Save(pFilename)

        Return pWB

        'If ShowMessage("The report was successfully saved as " & pFilename & ". Do you want to open it?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
        '    Try
        '        Waiter.Show("Opening file")
        '        Process.Start(pFilename)
        '        Waiter.Hide()
        '    Catch ex As Exception
        '        ShowMessage(ex.Message, "Error opening spreadsheet", , MessageBoxIcon.Exclamation)
        '    End Try
        'End If

    End Function

    Private Sub RenderItems(ByVal vWS As Worksheet, ByVal vDV As DataView, ByVal vDates As List(Of Date), ByRef rRowIndex As Integer)

        Dim pDRV As DataRowView
        Dim p As Integer

        For Each pDRV In vDV
            rRowIndex += 1
            vWS.Cells(rRowIndex, 0).PutValue(pDRV("StockNumber"))
            vWS.Cells(rRowIndex, 1).PutValue(pDRV("StockDescription"))
            For p = 0 To vDates.Count - 1
                If vDV.Table.Columns.Contains(Format(vDates(p), mDateFormat) & " - Amount") Then
                    vWS.Cells(rRowIndex, 2 + p).PutValue(pDRV(Format(vDates(p), mDateFormat) & " - Amount"))
                    vWS.Cells(rRowIndex, 2 + p).Style.Custom = cGlobal.FormatCurrencyGrid
                    vWS.Cells(rRowIndex, 2 + p + vDates.Count + 1).PutValue(pDRV(Format(vDates(p), mDateFormat) & " - Quantity"))
                    vWS.Cells(rRowIndex, 2 + p + vDates.Count + 1).Style.Custom = cGlobal.FormatIntegerAccounting
                End If
            Next p
            vWS.Cells(rRowIndex, 2 + vDates.Count).PutValue(pDRV("TotalAmount"))
            vWS.Cells(rRowIndex, 2 + vDates.Count).Style.Custom = cGlobal.FormatCurrencyGrid
            vWS.Cells(rRowIndex, 2 + vDates.Count * 2 + 1).PutValue(pDRV("TotalQty"))
            vWS.Cells(rRowIndex, 2 + vDates.Count * 2 + 1).Style.Custom = cGlobal.FormatIntegerAccounting
        Next pDRV

    End Sub

    Private Function GetData(ByVal vDateFrom As Date, ByVal vDateTo As Date, ByVal vSupplierID As String,
        ByRef oGeneratedQuantityColumns As List(Of DataColumn),
        ByRef oGeneratedAmountColumns As List(Of DataColumn),
        ByRef oDates As List(Of Date),
        ByRef oDtBranches As DataTable,
        Optional ByVal vShowCustomer As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pDtAll As DataTable
        Dim pDateFrom, pDateTo As Date
        Dim pDR As DataRow
        Dim p As Integer
        Dim pQtyColumns, pAmountColumns As CrossColumn
        Dim pFixedColumns As List(Of String)

        oDates = New List(Of Date)
        For p = 0 To DateDiff(DateInterval.Month, vDateFrom, vDateTo)
            pDateFrom = vDateFrom.AddMonths(p)
            pDateFrom = New Date(pDateFrom.Year, pDateFrom.Month, 1)
            If pDateFrom < vDateFrom Then pDateFrom = vDateFrom

            pDateTo = vDateFrom.AddMonths(p + 1)
            pDateTo = New Date(pDateTo.Year, pDateTo.Month, 1).AddMilliseconds(-1)
            If pDateTo > vDateTo Then pDateTo = vDateTo

#If _MYTYPE = "Windows" _
    Or _MYTYPE = "WindowsForms" Then
            Waiter.Show("Getting data from " & pDateFrom & " to " & pDateTo)
#End If
            oDates.Add(pDateTo)
            pDT = mParent.TransInvoice.GetSummaryPerSupplierAndBranch(pDateFrom, pDateTo, vSupplierID, vShowCustomer)
            pDT.Columns.Add("DisplayDate")
            For Each pDR In pDT.Rows
                pDR("DisplayDate") = Format(pDateTo, mDateFormat)
            Next pDR
            If pDtAll Is Nothing Then
                pDtAll = pDT
            Else
                pDtAll.Merge(pDT)
            End If
        Next p
#If _MYTYPE = "Windows" _
    Or _MYTYPE = "WindowsForms" Then
        Waiter.Hide(oDates.Count)
#End If

        pFixedColumns = New List(Of String)
        pFixedColumns.Add("StockNumber")
        pFixedColumns.Add("StockDescription")
        pFixedColumns.Add("BranchID")
        If vShowCustomer Then
            pFixedColumns.Add("CompanyName")
        End If

        pQtyColumns = New CrossColumn("{DisplayDate} - Quantity", "QtySold", "TotalQty")
        pAmountColumns = New CrossColumn("{DisplayDate} - Amount", "Amount", "TotalAmount")
        pDT = CreateCrossTab(pDtAll, pFixedColumns.ToArray,
            {
                pQtyColumns,
                pAmountColumns
            }
        )
        oGeneratedQuantityColumns = pQtyColumns.GeneratedColumns
        oGeneratedAmountColumns = pAmountColumns.GeneratedColumns

        '// insert branches
        oDtBranches = mParent.LibraryBranches.GetAll

        Return pDT

    End Function
#End Region

    Private Const InvoicesWithHeldTaxStart As String = "SELECT SUM(TransMemoTaxWithheldInvoices.TaxWithheldAmount) AS TaxWithheldAmount" & vbCrLf &
        " ,TransInvoice.InvoiceNumber" & vbCrLf &
        " ,TransInvoice.InvoiceDate" & vbCrLf &
        " ,MAX(TransInvoice.TotalAmount) AS InvoiceAmount" & vbCrLf &
        " ,TransMemo.MemoNumber" & vbCrLf &
        " ,TransMemoTaxWithheldInvoices.InvoiceID" & vbCrLf &
        " FROM TransMemoTaxWithheldInvoices" & vbCrLf &
        " JOIN TransMemo ON TransMemo.ID = TransMemoTaxWithheldInvoices.MemoID" & vbCrLf &
        " JOIN TransInvoice ON TransInvoice.ID = TransMemoTaxWithheldInvoices.InvoiceID" & vbCrLf

    Private Const InvoicesWithHeldTaxEnd As String = " GROUP BY TransInvoice.InvoiceNumber" & vbCrLf &
        " ,TransInvoice.InvoiceDate" & vbCrLf &
        " ,TransMemo.MemoNumber" & vbCrLf &
        " ,TransMemoTaxWithheldInvoices.InvoiceID" & vbCrLf &
        " " & vbCrLf &
        " ORDER BY TransInvoice.InvoiceDate"

    Public Function InvoicesWithHeldTax(ByVal vCustomerID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithHeldTaxStart &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND  TransMemo.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.CustomerId = " & ReadyString(vCustomerID) &
            " " & vbCrLf &
            InvoicesWithHeldTaxEnd

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function InvoicesWithHeldTaxByCustomerPayment(ByVal vPaymentID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithHeldTaxStart &
            " JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN TransCustomerPayment ON TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerPayment.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerPaymentInvoices.CustomerPaymentID = " & ReadyString(vPaymentID) &
            " " & vbCrLf &
            InvoicesWithHeldTaxEnd

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function InvoicesWithHeldTaxByBillingStatement(ByVal vBillingStatementId As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithHeldTaxStart &
            " JOIN TransCustomerBillingStatementInvoices ON TransCustomerBillingStatementInvoices.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN TransCustomerBillingStatement ON TransCustomerBillingStatement.ID = TransCustomerBillingStatementInvoices.BillingStatementID" & vbCrLf &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerBillingStatement.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerBillingStatement.ID = " & ReadyString(vBillingStatementId) &
            " " & vbCrLf &
            InvoicesWithHeldTaxEnd

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    'Public Sub SetHasForm2307(vID As String, vForm2306 As Boolean, vForm2307 As Boolean)

    '    Dim pIU As InsertUpdate
    '    Dim pSql As String

    '    pIU = New InsertUpdate("TransBirForms")
    '    pIU("ID") = "newid()"
    '    pIU("InvoiceID") = ReadyString(vID)
    '    pIU("UserID") = mParent.MainSecurity.UserAccess.EmployeeID
    '    pIU("DateCreated") = "GETDATE()"
    '    pIU("Form2307") = If(vForm2307, 1, 0)
    '    pIU("Form2306") = If(vForm2306, 1, 0)
    '    pSql = pIU.GetInsert
    '    mProData.ExecuteSQL(pSql)

    'End Sub

    ''' <summary>
    ''' returns true if a record was found
    ''' </summary>
    ''' <param name="vInvoiceDefaultID">MainInvoiceDefaults.ID</param>
    ''' <param name="oTransactionType">output MainInvoiceDefaults.TransactionType</param>
    ''' <param name="oAbbrevation">output MainInvoiceDefaults.Abbrevation</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInvoiceType(ByVal vInvoiceDefaultID As String,
        ByRef oTransactionType As String,
        ByRef oAbbrevation As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        oTransactionType = ""
        oAbbrevation = ""
        pSql = "SELECT MainInvoiceDefaults.TransactionType" & vbCrLf &
            " ,MainInvoiceDefaults.Abbrevation" & vbCrLf &
            " FROM MainInvoiceDefaults" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceDefaultID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            oTransactionType = pDT.Rows(0)("TransactionType")
            oAbbrevation = pDT.Rows(0)("Abbrevation")
            Return True
        End If

    End Function

    Public Function CustomerServiceLevel(ByVal vInvoiceDateFrom As Date, ByVal vInvoiceDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pCalendar As System.Globalization.GregorianCalendar

        pSql = "SELECT COUNT(*) AS Total" & vbCrLf &
        " ,ISNULL(SUM(CASE WHEN TransInvoiceItems.IsHasBackOrder = 1 OR TransInvoiceItems.IsReplacement = 1 THEN 1 ELSE 0 END), 0) AS Imperfect" & vbCrLf &
        " FROM TransInvoiceItems" & vbCrLf &
        " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
        " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
        " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vInvoiceDateFrom.Date) & " AND " & ReadyString(vInvoiceDateTo.Date) & vbCrLf

        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("DateRange")
        pDT.Columns.Add("WeekNum", GetType(Integer))

        pDT.Rows(0)("DateRange") = DisplayableDateRange(vInvoiceDateFrom, vInvoiceDateTo)
        pDT.Rows(0)("WeekNum") = New System.Globalization.GregorianCalendar().GetWeekOfYear(vInvoiceDateFrom, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday)

        Return pDT

    End Function

    Public Function CustomerServiceLevel(ByVal vInvoiceDateFrom As Date, ByVal vInvoiceDateTo As Date, ByVal vDurationType As eDurationType) As DataTable

        Dim pDT As DataTable
        Dim pDateRanges As List(Of DateRange)
        Dim pDateRange As DateRange
        Dim pOut As DataTable

        pDateRanges = GetDateRanges(vInvoiceDateFrom, vInvoiceDateTo, vDurationType, False)
        For Each pDateRange In pDateRanges
            pDT = CustomerServiceLevel(pDateRange.DateFrom, pDateRange.DateTo)
            If pOut Is Nothing Then
                pOut = pDT
            Else
                pOut.Merge(pDT)
            End If
        Next pDateRange
        pOut.Columns.Add("Perfect", GetType(Integer), "Total - Imperfect")

        Return pOut

    End Function

    Public Function IsValidCart(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoice.ID" &
            " FROM TransInvoice" &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.InvoiceNumber = 0" & vbCrLf &
            " AND TransInvoice.ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT.Rows.Count > 0

    End Function

    Public Function GetCart(Optional ByVal vID As String = Nothing,
        Optional ByVal vUserID As String = Nothing,
        Optional ByVal vFowstID As String = Nothing,
        Optional vForPickingList As Boolean = False,
        Optional vSectionID As String = Nothing,
        Optional vIncludeZeroQuantity As Boolean = False,
        Optional vIncludeVendors As Boolean = False,
        Optional vIncludeChildOrderItems As Boolean = False,
        Optional vIncludeStockReceivingData As Boolean = False) As DataSet

        Dim pSql As String
        Dim pInvoiceID As String ' TransInvoice.ID
        Dim pDS As DataSet

        pInvoiceID = vID
        If CStr2(pInvoiceID).Length = 0 Then
            pInvoiceID = GetCartID(vUserID, vFowstID)
            If CStr2(pInvoiceID).Length = 0 Then Return Nothing
        End If
        ' pSql = " WHERE TransInvoice.ID = " & ReadyString(pInvoiceID) ' not needed 2020-08-05
        pDS = GetReportData(pInvoiceID, False, vForPickingList, vSectionID, vIncludeZeroQuantity, Not vIncludeZeroQuantity, vIncludeVendors, vIncludeChildOrderItems, vIncludeStockReceivingData)
        pDS.Tables("Invoices").Columns("InvoiceID").ColumnName = "ID"

        Return pDS

    End Function

    Public Function GetCartID(ByVal vUserID As String, ByVal vFowstID As String, Optional vExcludeWithSalesOrderNum As Boolean = False) As String

        Dim pDR As DataRow
        Dim pSql As String

        If vExcludeWithSalesOrderNum Then
            pSql = " AND ISNULL(TransInvoice.SalesOrderNo, 0) = 0" & vbCrLf
        End If
        pSql = "SELECT TOP 1 TransInvoice.ID" &
            " FROM TransInvoice" &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.InvoiceNumber = 0" & vbCrLf &
            pSql &
            " AND EXISTS(SELECT TransInvoiceItems.ID" & vbCrLf &
            " 	FROM TransInvoiceItems" & vbCrLf &
            "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)" & vbCrLf &
            " AND TransInvoice.UserID = " & ReadyString(vUserID) &
            " AND TransInvoice.FowstID = " & IIf(vFowstID.Trim().Length > 0, ReadyString(vFowstID), "NULL") &
            " ORDER BY TransInvoice.LastUpdated DESC"
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

    End Function

    Public Function GetCartIDByParentOrderID(ByVal vParentOrderID As String, ByVal vFowstID As String) As String

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT TOP 1 TransInvoice.ID" &
            " FROM TransInvoice" &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND ISNULL(TransInvoice.SalesOrderNo, 0) = 0" & vbCrLf &
            " AND TransInvoice.ParentOrderID = " & ReadyString(vParentOrderID) & vbCrLf &
            " AND TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

    End Function

    Public Function GetCartStock(ByVal vInvoiceID As String, ByVal vProductID As String, ByVal posmode As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        'additional by janine sept 9 2014 for sales order taker
        If (posmode = "so") Then
            pSql = "SELECT SUM(TransInvoiceItems.OrderedQuantity) AS Quantity" & vbCrLf
        ElseIf posmode = "po" Then
            pSql = "SELECT SUM(CASE ISNULL(TransInvoiceItems.Quantity, 0)" & vbCrLf &
                " WHEN 0 THEN TransInvoiceItems.OrderedQuantity" & vbCrLf &
                " ELSE TransInvoiceItems.Quantity END) AS Quantity" & vbCrLf
        ElseIf posmode = "ap" Then
            pSql = "SELECT SUM(COALESCE(TransInvoiceItems.Quantity, TransInvoiceItems.OrderedQuantity, 0)) AS Quantity" & vbCrLf
        Else
            pSql = "SELECT SUM(TransInvoiceItems.Quantity) AS Quantity" & vbCrLf &
                ",SUM(TransInvoiceItems.OrderedQuantity) AS OrderedQuantity" & vbCrLf
        End If
        pSql &= ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoiceItems.UnitMeasure" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID" & vbCrLf &
            ",TransInvoiceItems.UnitPrice" & vbCrLf &
            ",TransInvoiceItems.OriginalUnitPrice" & vbCrLf &
            ",TransInvoiceItems.TradeDiscountLess" & vbCrLf &
            ",TransInvoiceItems.NetDiscountFactor" & vbCrLf &
            ",TransInvoiceItems.NetDiscountRate" & vbCrLf &
            ",TransInvoiceItems.TermNumericValue" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " GROUP BY TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoiceItems.UnitMeasure" & vbCrLf &
            ",TransInvoiceItems.UnitPrice" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID" & vbCrLf &
            ",TransInvoiceItems.TradeDiscountLess" & vbCrLf &
            ",TransInvoiceItems.NetDiscountFactor" & vbCrLf &
            ",TransInvoiceItems.NetDiscountRate" & vbCrLf &
            ",TransInvoiceItems.TermNumericValue" & vbCrLf &
            ",TransInvoiceItems.OriginalUnitPrice" & vbCrLf
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSalesReps(ByVal vInvoiceID As String, Optional ByVal vProductID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        'pSql = "SELECT DISTINCT TransInvoiceItems.ActualQuantity" & vbCrLf & _
        '    ",TransInvoiceItemSalesReps.PeopleID" & vbCrLf &
        '    " ,ISNULL(MobizLibraryPeoplePreNominalSuffix.Code, '') + ' ' +" & vbCrLf & _
        '    "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf & _
        '    "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') +" & vbCrLf & _
        '    "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " & vbCrLf & _
        '    "  ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)), ', '), '') +" & vbCrLf & _
        '    "  ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))), ' '), '') AS PeopleName" & vbCrLf & _
        '    " FROM TransInvoiceItems " & vbCrLf & _
        '    " JOIN TransInvoiceItemSalesReps ON TransInvoiceItemSalesReps.InvoiceItemID = TransInvoiceItems.ID" & vbCrLf & _
        '    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID" & vbCrLf & _
        '    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf & _
        '    " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
        '    " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
        '    " ORDER BY TransInvoiceItems.ActualQuantity" & vbCrLf

        pSql = "SELECT DISTINCT " & vbCrLf &
            " TransInvoiceItemSalesReps.ID" & vbCrLf &
            " ,TransInvoiceItemSalesReps.InvoiceItemID" & vbCrLf &
            " ,TransInvoiceItemSalesReps.PeopleID" & vbCrLf &
            " ,ISNULL(MobizLibraryPeoplePreNominalSuffix.Code, '') + ' ' +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " & vbCrLf &
            "  ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)), ', '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))), ' '), '') AS PeopleName" & vbCrLf &
            " FROM TransInvoiceItemSalesReps " & vbCrLf &
            " INNER JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " WHERE TransInvoiceItemSalesReps.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        If String.IsNullOrWhiteSpace(vProductID) = False Then
            pSql &= " AND TransInvoiceItemSalesReps.ProductID = " & ReadyString(vProductID)
        End If

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetProductSerial(ByVal vInvoiceID As String, ByVal vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT " & vbCrLf &
            " prdserial.ID, " & vbCrLf &
            " prdserial.SerialNumber " & vbCrLf &
            " FROM TransInvoiceItemSerials prdserial " & vbCrLf &
            " WHERE prdserial.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND prdserial.ProductID = " & ReadyString(vProductID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetByItemSerial(vSerial As String) As DataTable

        Dim pSql = "SELECT TransInvoice.ID" & vbCrLf &
            ",TransInvoice.TransType" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItemSerials ON TransInvoiceItemSerials.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoiceItemSerials.SerialNumber = " & ReadyString(vSerial) & vbCrLf &
            " GROUP BY TransInvoice.ID" & vbCrLf &
            ",TransInvoice.TransType"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetByItemSerialAndStockReceivingItemSerial(vSerial As String) As DataTable

        Dim pSql = "SELECT TransInvoice.ID" & vbCrLf &
            ",TransInvoice.TransType" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItemSerials ON TransInvoiceItemSerials.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoiceItemSerials.SerialNumber = " & ReadyString(vSerial) & vbCrLf &
            " OR TransInvoiceItemSerials.Imei = " & ReadyString(vSerial) & vbCrLf &
            " GROUP BY TransInvoice.ID" & vbCrLf &
            ",TransInvoice.TransType" & vbCrLf
        pSql &= " UNION " & vbCrLf
        pSql &= "SELECT TransReceiving.ID" & vbCrLf &
            ",'StockReceiving' AS TransType" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " JOIN TransInvoiceItemSerials ON TransInvoiceItemSerials.InvoiceID = TransReceiving.ID" & vbCrLf &
            " WHERE TransInvoiceItemSerials.SerialNumber = " & ReadyString(vSerial) & vbCrLf &
            " OR TransInvoiceItemSerials.Imei = " & ReadyString(vSerial) & vbCrLf &
            " GROUP BY TransReceiving.ID"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetMaxLineNumber(ByVal vOrderID As String) As Integer

        Dim pSql As String

        pSql = "SELECT MAX(TransInvoiceItems.LineNumber) AS LineNumber" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vOrderID)

        Return CDec2(mProData.ExecuteScalar(pSql))

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vInvoiceID"></param>
    ''' <param name="vProductID"></param>
    ''' <param name="vUnitMeasure">if numeric, will be compared to ActualQuantity</param>
    ''' <remarks></remarks>
    Public Sub DeleteOrderStock(ByVal vInvoiceID As String, ByVal vProductID As String, ByVal vUnitMeasure As String,
        Optional vItemID As String = Nothing,
        Optional vVendorID As String = Nothing,
        Optional vOrgID As String = Nothing)

        Dim pSql As String
        Dim pSqlWhere As String

        If String.IsNullOrWhiteSpace(vUnitMeasure) Then
            pSqlWhere = "" ' just delete all items w/ this productID
        ElseIf CDec2(vUnitMeasure) > 0 Then
            pSqlWhere = " AND TransInvoiceItems.ActualQuantity = " & CDec2(vUnitMeasure) & vbCrLf
        Else
            pSqlWhere = " AND TransInvoiceItems.UnitMeasure = " & ReadyString(vUnitMeasure) & vbCrLf
        End If

        If String.IsNullOrWhiteSpace(vItemID) = False Then
            pSqlWhere &= " AND TransInvoiceItems.ID = " & ReadyString(vItemID) & vbCrLf
        ElseIf String.IsNullOrWhiteSpace(vVendorID) Then
            pSqlWhere &= " AND TransInvoiceItems.VendorID IS NULL " & vbCrLf
        Else
            pSqlWhere &= " AND TransInvoiceItems.VendorID = " & ReadyString(vVendorID) & vbCrLf
        End If

        pSql = "DELETE t " & vbCrLf &
       " FROM TransInvoiceItemSalesReps t" & vbCrLf &
       " JOIN TransInvoiceItems ON TransInvoiceItems.ID = t.InvoiceItemID" & vbCrLf &
       " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
       " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        'additional by janine sept 9 2014 11am. if blank, ok lang delete all unit measure of that product, if not then delete only specific unit measure
        pSql = pSql & pSqlWhere
        mProData.ExecuteSQL(pSql)

        pSql = "DELETE FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        'additional by janine sept 9 2014 11am. if blank,ok lang delete all unit measure of that product, if not then delete only specific unit measure
        pSql = pSql & pSqlWhere
        mProData.ExecuteSQL(pSql)

        If String.IsNullOrWhiteSpace(vItemID) = False Then
            pSql = "DELETE FROM TransProfitability" & vbCrLf &
                " WHERE ItemID = " & ReadyString(vItemID)
            Dim pRecordCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only

            SaveTransProfitability(vInvoiceID, vOrgID, True)
            'UpdateProfitabilityTransactionColumns(vInvoiceID)
        End If

    End Sub

    Public Function GetOrderedQty(ByVal vInvoiceID As String, ByVal vProductID As String, Optional ByVal vUnitMeasure As String = Nothing) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT OrderedQuantity, UnitMeasure FROM TransInvoiceItems " & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID) & vbCrLf
        If String.IsNullOrWhiteSpace(vUnitMeasure) = False Then
            pSql &= " AND UnitMeasure = " & ReadyString(vUnitMeasure)
        End If
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function UpdateLastUpdated(vInvoiceID As String, vUserID As String) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer

        pIU = New InsertUpdate("TransInvoice")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceID))
        pOut = mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

#Region " invoice print "
#If _MYTYPE = "Windows" _
    Or _MYTYPE = "WindowsForms" Then

    Public Sub ShowPreview(vWhere As String)

        Dim pFrm As ProWin.ProReport
        Dim pDS As DataSet
        Dim pCustomerInvoiceDefaultID As String
        Dim pError As String

        If gBusiness.SystemDefaults.InvoiceTypeForCustomerAutoInvoice(pCustomerInvoiceDefaultID, "", pError) = False Then
            ShowMessage(pError)
            Exit Sub
        End If
        pDS = GetReportData("WHERE TransInvoice.InvoiceDefaultID = " & ReadyString(pCustomerInvoiceDefaultID) & " AND (" & vWhere & ")", False, True)

        pFrm = New Prowin.ProReport
        pFrm.AutoFillHeader = False
        pFrm.SourceFilename = "TransInvoice2"
        pFrm.Variables("CompanyAddress") = gBusiness.SystemDefaults.GetCompanyAddress
        pFrm.Variables("CompanyContactNumber") = gBusiness.SystemDefaults.GetCompanyContactNumbers
        pFrm.Variables("CompanyName") = gBusiness.SystemDefaults.GetCompanyName
        pFrm.Variables("CompanyTin") = "TIN: " & gBusiness.SystemDefaults.GetCompanyTin
        pFrm.Variables("CopyType") = "TRIPLICATE COPY" & vbCrLf & "(Company's Copy)"

#If DEBUG Then
        mProData.CreateTable("rptTransInvoice", pDS.Tables("Invoices"))
        mProData.CreateTable("rptTransInvoiceStocks", pDS.Tables("Stocks"))
        'mProData.CreateTable("rptTransInvoiceBackOrders", pDS.Tables("BackOrders"))
#End If

        Dim pDtInvoices As DataTable
        Dim pDtStocks As DataTable
        Dim pDtBackOrders As DataTable

        pDtInvoices = pDS.Tables("Invoices")
        pDS.Tables.Remove(pDtInvoices) ' have to remove from dataset or else Stimulsoft error

        pDtStocks = pDS.Tables("Stocks")
        pDS.Tables.Remove(pDtStocks)

        'pDtBackOrders = pDS.Tables("BackOrders")
        'pDS.Tables.Remove(pDtBackOrders)

        pFrm.AddTable("rptTransInvoice", pDtInvoices)
        pFrm.AddTable("rptTransInvoiceStocks", pDtStocks)
        'pFrm.AddTable("rptTransInvoiceBackOrders", pDtBackOrders)
        'pFrm.AddTable("rptTransInvoiceBackOrders", New DataTable)

        pFrm.PrintPreview(True)

    End Sub
#End If

    Public Function ComputeTotal(ByVal vID As String,
        ByRef oGrossSrvAmount As Decimal,
        ByRef oGrossRefundAmount As Decimal,
        ByRef oTotalAdjustments As Decimal) As Decimal

        Dim pSql As String
        Dim pDR As DataRow
        Dim pOut As Decimal

        pSql = "SELECT SUM(SubTotal) AS SubTotal" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vID) & vbCrLf &
            " AND (TransInvoiceItems.Quantity > 0 OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
            " AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return 0
        End If
        pOut = CDec2(pDR("SubTotal"))

        ' delete this 2024_03_04__13_52_50_2510
        'pSql = "SELECT SUM(TransInvoiceItemsModifier.SubTotal) AS SubTotal" & vbCrLf &
        '    ",SUM(TransInvoiceItems.SubTotal) AS ItemsSubTotal" & vbCrLf &
        '    " FROM TransInvoiceItemsModifier" & vbCrLf &
        '    " JOIN TransInvoiceItems ON TransInvoiceItems.ID =  TransInvoiceItemsModifier.InvoiceItemID" & vbCrLf &
        '    " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vID)
        'pDR = mProData.ExecuteRow(pSql)
        'If pDR IsNot Nothing Then
        '    pOut += CDec2(pDR("SubTotal"))
        'End If

        pSql = "SELECT SUM(GrossSrvAmount) AS GrossSrvAmount" & vbCrLf &
            ",SUM(GrossRefundAmount) AS GrossRefundAmount" & vbCrLf &
            ",SUM(Fees) AS Fees" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE SourceInvoiceID = " & ReadyString(vID) & vbCrLf
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            oGrossSrvAmount = CDec2(pDR("GrossSrvAmount"))
            oGrossRefundAmount = CDec2(pDR("GrossRefundAmount"))
            oTotalAdjustments = oGrossSrvAmount + oGrossRefundAmount
        Else
            oGrossSrvAmount = 0
            oGrossRefundAmount = 0
            oTotalAdjustments = 0
        End If

        Return pOut

    End Function

    Public Function GetReportData(ByVal vWhere As String,
        Optional ByVal vAddUnitExplaination As Boolean = True,
        Optional vForPickingList As Boolean = False,
        Optional vSectionID As String = Nothing,
        Optional vIncludeZeroQuantity As Boolean = False,
        Optional vExcludeHiddenQuantity As Boolean = False,
        Optional vIncludeVendors As Boolean = False,
        Optional vIncludeChildOrderItems As Boolean = False,
        Optional vIncludeStockReceivingData As Boolean = False) As DataSet

        Dim pDS As DataSet
        Dim pDT As DataTable
        Dim pDtInvoice As DataTable
        Dim pSql As String
        Dim pCustomerSelect, pCustomerFrom As String
        Dim pSalesPartnerSelect, pSalesPartnerFrom As String
        Dim pWhereItems As String = vWhere
        Dim pQuantityField As String = "Quantity"
        Dim pTransID As String

        If IsGuid(vWhere) Then
            pTransID = vWhere
            If vIncludeChildOrderItems Then
                pWhereItems = " WHERE (TransInvoice.ID = " & ReadyString(vWhere) & vbCrLf &
                    " OR TransInvoice.ParentOrderID = " & ReadyString(vWhere) & ")" & vbCrLf
                vWhere = " WHERE TransInvoice.ID = " & ReadyString(vWhere) & vbCrLf
            Else
                vWhere = " WHERE TransInvoice.ID = " & ReadyString(vWhere) & vbCrLf
                pWhereItems = vWhere
            End If
        End If

        If vForPickingList Then
            If vSectionID Is Nothing Then
                Throw New ArgumentException("vSectionID must not be null if vForPickingList is true", "vSectionID")
            End If
        End If

        pDS = New DataSet

        '// invoice ==============================================================================================================================================
        BrowserController.ComposeCustomer(pCustomerSelect, pCustomerFrom)
        BrowserController.ComposeCustomer(pSalesPartnerSelect, pSalesPartnerFrom, "tblSalesPartner", "tblSalesPartnerPerNominalSuffix")
        pSql = "SELECT TransInvoice.ID AS InvoiceID" & vbCrLf &
            " ," & pCustomerSelect & " AS CustomerName" & vbCrLf &
            ",LibraryPaymentTerms.Days AS CreditTerms" & vbCrLf &
            ",tblSupplierPaymentTerms.SupplierCreditTerms" & vbCrLf &
            ",TransInvoice.PaymentTerm" & vbCrLf &
            ",DATEADD(DAY, ISNULL(TransInvoice.PaymentTerm, 0), TransInvoice.InvoiceDate) AS DueDate" & vbCrLf &
            ",TransInvoice.PaymentTermsText" & vbCrLf &
            " ,MobizLibraryOrganization.TinNumber" & vbCrLf &
            " ,MobizLibraryOrganizationRepresentatives.LastName + ', ' + MobizLibraryOrganizationRepresentatives.FirstName + ' ' + MobizLibraryOrganizationRepresentatives.MiddleName AS Representative" & vbCrLf &
            " --,RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS InvoiceNumber" & vbCrLf &
            " ,TransInvoice.InvoiceNumber" & vbCrLf &
            " ,TransInvoice.FowstWideInvoiceNumber" & vbCrLf &
            " ,TransInvoice.InvoiceDate" & vbCrLf &
            " ,TransInvoice.DateCreated" & vbCrLf &
            " ,TransInvoice.DeliveryDate" & vbCrLf &
            " ,TransInvoice.Remarks" & vbCrLf &
            " ,TransInvoice.InternalRemarks" & vbCrLf &
            " ,TransInvoice.CustomerID" & vbCrLf &
            " ,TransInvoice.RefInvoiceNum" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.UserID" & vbCrLf &
            " ,TransInvoice.FinalizePickListByID" & vbCrLf &
            " ,TransInvoice.DontAutoInvoice" & vbCrLf &
            " ,TransInvoice.CncApprovedByID" & vbCrLf &
            " ,TransInvoice.FinalizedForApproval" & vbCrLf &
            " ,TransInvoice.ApprovedDate" & vbCrLf &
            "," & BrowserController.ComposePeopleName("tblPoApprover", "PoApproverNominalSuffix") & " AS ApprovedByName" & vbCrLf &
            " ,TransInvoice.CnCUnapprovedReason" & vbCrLf &
            " ,TransInvoice.UnapprovedProfitability" & vbCrLf &
            ", ISNULL(NULLIF(LTRIM(RTRIM(tblUpdater.Surname)) + ', ',', '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(tblUpdater.FirstName)) + ' ',' '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(tblUpdater.MiddleName)) + ' ',' '), '') AS Updater " & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.RefNum" & vbCrLf &
            " ,TransInvoice.ReasonForReconstructingData" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,TransInvoice.SalesOrderDate" & vbCrLf &
            " ,TransInvoice.SalesOrderNo" & vbCrLf &
            " ,TransInvoice.SalesOrderUserID" & vbCrLf &
            " ,TransInvoice.BillingAddressID" & vbCrLf &
            " ,TransInvoice.DeliveryAddressID" & vbCrLf &
            " ,TransInvoice.SalesOrderID" & vbCrLf &
            " ,TransInvoice.TotalAmount" & vbCrLf &
            " ,TransInvoice.Vatable" & vbCrLf &
            " ,TransInvoice.DiscountAmount" & vbCrLf &
            " ,TransInvoice.NetSuiteNum" & vbCrLf &
            " ,TransInvoice.IsCancelled" & vbCrLf &
            ",TransInvoice.WithdrawalSlipPrintCount" & vbCrLf &
            ",TransInvoice.PickListPrintCount" & vbCrLf &
            ",TransInvoice.PrintCount" & vbCrLf &
            ",CONVERT(varchar, SoFowst.FOWSTNo) + '-' + CONVERT(varchar, SoDevice.DeviceNo) AS SoMIN" & vbCrLf &
            " ,tblSalesOrder.SalesOrderNo AS RefSalesOrderNo" & vbCrLf &
            " ,tblSalesOrder.SalesOrderDate AS RefSalesOrderDate" & vbCrLf &
            ",tblDelivery.Name AS DeliveryFowstName" & vbCrLf &
            " ,TransInvoice.DucInvoiceNum" & vbCrLf &
            " ,MobizLibraryPeople.DUCNumericID" & vbCrLf &
            " ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName AS EmployeeFullName" & vbCrLf &
            " ,createdby.Surname + ', ' + createdby.FirstName AS CreatedbyName" & vbCrLf &
            " ,TransInvoice.InvoiceTypeCode" & vbCrLf &
            " ,TransInvoice.TransType" & vbCrLf &
            " ,TransInvoice.DocumentNameID" & vbCrLf &
            " ,LibraryDocumentControl.DocumentName" & vbCrLf &
            " ,LibraryDocumentControl.PermitNum" & vbCrLf &
            " ,TransInvoice.QuotationID" & vbCrLf &
            " ,TransInvoice.QuotationNo" & vbCrLf &
            " ,TransInvoice.QuotationNumText" & vbCrLf &
            " ,TransInvoice.QuotationDate" & vbCrLf &
            " ,TransInvoice.PurchaseOrderNo" & vbCrLf &
            " ,TransInvoice.PurchaseOrderText" & vbCrLf &
            " ,TransInvoice.PurchaseOrderDate" & vbCrLf &
            " ,TransInvoice.AccountsPayableNum" & vbCrLf &
            " ,TransInvoice.CanvassID" & vbCrLf &
            " ,LibraryPriceCanvass.CanvassNumber" & vbCrLf &
            " ,tblQuote.QuotationNo AS RefQuotationNo" & vbCrLf &
            " ,tblQuote.QuotationDate AS RefQuotationDate" & vbCrLf &
            ",TransInvoice.DeliveryMethod" & vbCrLf &
            ",TransInvoice.DepositAmount" & vbCrLf &
            ",TransInvoice.SalesInvoiceRef" & vbCrLf &
            ",TransInvoice.OfficialReceiptRef" & vbCrLf &
            ",Mobiz.dbo.ContactInfo(TransInvoice.DeliveryAddressID) AS DeliveryContactInfo" & vbCrLf &
            ",Mobiz.dbo.ContactInfo(TransInvoice.BillingAddressID) AS BillingContactInfo" & vbCrLf

        '// "Freight and Handling" tab
        pSql &= " ,TransInvoice.ShipperID" & vbCrLf &
            " ,tblOrgShipper.CompanyName AS ShipperName" & vbCrLf &
            " ,TransInvoice.WaybillNumber" & vbCrLf &
            " ,TransInvoice.PackageQuantity" & vbCrLf &
            " ,TransInvoice.GrossWeight" & vbCrLf &
            " ,TransInvoice.GrossVolume" & vbCrLf &
            " ,TransInvoice.Freight" & vbCrLf &
            " ,TransInvoice.Handling" & vbCrLf &
            " ,TransInvoice.Arrastre" & vbCrLf &
            " ,TransInvoice.OtherCharges" & vbCrLf &
            " ,TransInvoice.FreightAndHandling" & vbCrLf &
            " ,TransInvoice.ShipDate" & vbCrLf

        If CurrentOrgID IsNot Nothing Then
            pSql += " ,tblOrg.CompanyName AS OrgName" & vbCrLf
        End If
        pSql &= " ,ISNULL(tblOrgFowst.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(tblOrgFowst.Street, '')" & vbCrLf &
            " + ',' + ISNULL(tblOrgCity.Name, '') AS OrgFowst" & vbCrLf &
            ",tblOrgFowst.Name AS OrgFowstName" & vbCrLf
        pSql &= ",TransInvoice.EncodingFowstID" & vbCrLf &
            ",ISNULL(tblEncodingFowst.Name, InvFowst.Name) AS EncodingFowstName" & vbCrLf
        pSql &= ",TransInvoice.InvoiceFowstID" & vbCrLf &
            ",ISNULL(tblInvoiceFowst.Name, '') AS InvoiceFowstName" & vbCrLf
        pSql &= ",ISNULL(NULLIF(LTRIM(RTRIM(tblDelivery.Name)), '') + ', ', '')" & vbCrLf &
            " + ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
            " + ',' + ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
            " + ',' + ISNULL(tblDeliveryCountry.Name, '') AS DeliveryAddress" & vbCrLf
        pSql &= ",ISNULL(NULLIF(LTRIM(RTRIM(tblBilling.Name)), '') + ', ', '')" & vbCrLf &
            " + ISNULL(tblBilling.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(tblBilling.Street, '')" & vbCrLf &
            "  + ',' + ISNULL(tblBillingCity.Name, '')" & vbCrLf &
            "  + ',' + ISNULL(tblBillingCountry.Name, '') AS BillingAddress" & vbCrLf

        pSql &= ",TransInvoice.SopaApprovedByID" & vbCrLf &
            ", ISNULL(NULLIF(LTRIM(RTRIM(SopaApprover.Surname)) + ', ',', '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(SopaApprover.FirstName)) + ' ',' '), '') +" & vbCrLf &
            "  ISNULL(NULLIF(LTRIM(RTRIM(SopaApprover.MiddleName)) + ' ',' '), '') AS SopaApproverName " & vbCrLf
        pSql &= ",TransInvoice.SalesPartnerOrgID" & vbCrLf &
            "," & pSalesPartnerSelect & " AS SalesPartner" & vbCrLf &
            ",TransInvoice.SalesRepText" & vbCrLf &
            ",TransInvoice.SalesRepPeopleID" & vbCrLf &
            "," & BrowserController.ComposePeopleName("tblSalesRep", "SalesRepNominalSuffix") & " AS SalesRep" & vbCrLf &
            ",TransInvoice.DateNeeded" & vbCrLf
        pSql &= ",TransInvoice.CustomerFowstID" & vbCrLf &
            ",TransInvoice.CustomerBinID" & vbCrLf &
            ",tblFowstCustomer.Name AS CustomerFowst" & vbCrLf &
            ",tblBinCustomer.Name AS CustomerBin" & vbCrLf
        pSql &= ",COALESCE(CONVERT(varchar, TransInvoice.FowstWideInvoiceNumber), CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber)) AS PrintedInvoiceNumber" & vbCrLf
        pSql &= ",CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.SalesOrderNo) AS PrintedSalesOrderNumber" & vbCrLf
        pSql &= ",CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS MIN" & vbCrLf
        pSql &= ",CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.PurchaseOrderNo) AS PrintedPurchaseOrderNumber" & vbCrLf
        pSql &= ",InvFowst.FOWSTNo" & vbCrLf
        pSql &= ",MobizLibraryIndustryClass.Class AS IndustryClass" & vbCrLf
        pSql &= ",COALESCE(MobizLibraryOrganization.BusinessIndustryText, MobizLibraryIndustryClass.Class) AS BusinessIndustryDisplay" & vbCrLf
        pSql &= ",TransInvoice.ParentOrderID" & vbCrLf
        pSql &= ",CONVERT(varchar, tblParentOrderFowst.FOWSTNo) + '-' + CONVERT(varchar, tblParentOrderDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(tblParentOrder.SalesOrderNo, 0)) AS ParentOrderMin" & vbCrLf
        pSql &= ",TransInvoice.OrderedByID" & vbCrLf
        pSql &= ",TransInvoice.StockTransferID" & vbCrLf
        pSql &= ",TransInvoice.Area" & vbCrLf
        pSql &= "," & BrowserController.ComposePeopleName("tblOrderedByPeople", "tblOrderedByPeoplePreNominalSuffix") & " AS OrderedByName" & vbCrLf
        pSql &= ",tblAccountsPayablePurchaseOrder.FullPoNum AS AccountsPayableFullPoNum" & vbCrLf
        pSql &= ",tblAccountsPayablePurchaseOrder.PurchaseOrderID AS AccountsPayablePurchaseOrderID" & vbCrLf
        pSql &= ",LibraryOrganizationCustomer.CustomerRequirePoNum" & vbCrLf

        pSql &= "  FROM TransInvoice " & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblEncodingFowst ON TransInvoice.EncodingFowstID = tblEncodingFowst.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblInvoiceFowst ON TransInvoice.InvoiceFowstID = tblInvoiceFowst.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID" & vbCrLf &
            pCustomerFrom & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationRepresentatives ON TransInvoice.CusRepresentativeID = MobizLibraryOrganizationRepresentatives.ID " & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople as createdby ON createdby.ID = TransInvoice.CreatedBy" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.BillingAddressID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCities ON MobizLibraryCities.ID = MobizLibraryOrganizationFOWST.FowstCityID " & vbCrLf &
            "  LEFT JOIN MobizLibraryCountries ON MobizLibraryCountries.ID = MobizLibraryOrganizationFOWST.FowstCountryID " & vbCrLf &
            "  LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID " & vbCrLf

        pSql &= " LEFT JOIN MobizLibraryOrganizationFOWST tblOrgFowst ON tblOrgFowst.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblOrgCity ON tblOrgCity.ID = tblOrgFowst.FowstCityID" & vbCrLf
        If CurrentOrgID IsNot Nothing Then
            pSql &= " LEFT JOIN MobizLibraryOrganization tblOrg ON tblOrg.ID = " & ReadyString(CurrentOrgID) & vbCrLf
        End If
        pSql += " LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID" & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID" & vbCrLf
        pSql &= "  LEFT JOIN MobizLibraryOrganizationFOWST tblBilling ON tblBilling.ID = TransInvoice.BillingAddressID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCities tblBillingCity ON tblBillingCity.ID = tblBilling.FowstCityID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCountries tblBillingCountry ON tblBillingCountry.ID = tblBilling.FowstCountryID " & vbCrLf

        pSql &= " LEFT JOIN MobizLibraryPeople SopaApprover ON SopaApprover.ID = TransInvoice.SopaApprovedByID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople tblUpdater ON tblUpdater.ID = TransInvoice.UserID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople tblSalesRep ON tblSalesRep.ID = TransInvoice.SalesRepPeopleID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix SalesRepNominalSuffix ON tblSalesRep.TitleID = SalesRepNominalSuffix.ID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople tblPoApprover ON tblPoApprover.ID = TransInvoice.ApprovedByID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix PoApproverNominalSuffix ON tblPoApprover.TitleID = PoApproverNominalSuffix.ID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryOrganization tblSalesPartner ON tblSalesPartner.ID = TransInvoice.SalesPartnerOrgID" & vbCrLf &
            pSalesPartnerFrom
        pSql &= " LEFT JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = TransInvoice.CustomerID" & vbCrLf
        pSql &= " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf

        pSql &= " OUTER APPLY (" & vbCrLf &
            " SELECT LibraryPaymentTerms.Days AS SupplierCreditTerms" & vbCrLf &
            " FROM LibraryOrganizationSupplier" & vbCrLf &
            " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf &
            " WHERE LibraryOrganizationSupplier.SupplierID = TransInvoice.CustomerID" & vbCrLf &
            ") tblSupplierPaymentTerms" & vbCrLf

        pSql &= " LEFT JOIN TransInvoice tblSalesOrder ON tblSalesOrder.ID = TransInvoice.SalesOrderID" & vbCrLf &
            " LEFT JOIN LibraryDevice SoDevice ON tblSalesOrder.DeviceID = SoDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST SoFowst ON tblSalesOrder.FowstID = SoFowst.ID" & vbCrLf
        pSql &= " LEFT JOIN TransInvoice tblQuote ON tblQuote.ID = TransInvoice.QuotationID" & vbCrLf
        pSql &= "  LEFT JOIN MobizLibraryOrganizationFOWST tblFowstCustomer ON tblFowstCustomer.ID = TransInvoice.CustomerFowstID" & vbCrLf &
            "  LEFT JOIN LibraryFowstBin tblBinCustomer ON tblBinCustomer.ID = TransInvoice.CustomerBinID" & vbCrLf
        pSql &= " LEFT JOIN LibraryPriceCanvass ON LibraryPriceCanvass.ID = TransInvoice.CanvassID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = MobizLibraryOrganization.IndustryClassID" & vbCrLf
        pSql &= " LEFT JOIN TransInvoice tblParentOrder ON tblParentOrder.ID = TransInvoice.ParentOrderID" & vbCrLf &
            " LEFT JOIN LibraryDevice tblParentOrderDevice ON tblParentOrderDevice.ID = tblParentOrder.DeviceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblParentOrderFowst ON tblParentOrderFowst.ID = tblParentOrder.FowstID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople tblOrderedByPeople ON tblOrderedByPeople.ID = TransInvoice.OrderedByID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblOrderedByPeoplePreNominalSuffix ON tblOrderedByPeoplePreNominalSuffix.ID = tblOrderedByPeople.TitleID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryOrganization tblOrgShipper ON tblOrgShipper.ID = TransInvoice.ShipperID" & vbCrLf
        pSql &= "  " & vbCrLf

        pSql &= " OUTER APPLY (" & vbCrLf &
            " 	SELECT tblPo.PurchaseOrderNo" & vbCrLf &
            "     ,TransReceiving.ID AS ReceivingID" & vbCrLf &
            "     ,TransReceiving.PurchaseOrderID" & vbCrLf &
            "     ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblPo.PurchaseOrderNo) AS FullPoNum" & vbCrLf &
            "     FROM TransReceiving" & vbCrLf &
            "     JOIN TransInvoice tblPo ON tblPo.ID = TransReceiving.PurchaseOrderID" & vbCrLf &
            "     LEFT JOIN MobizLibraryOrganizationFOWST ON tblPo.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "     LEFT JOIN LibraryDevice ON tblPo.DeviceID = LibraryDevice.ID" & vbCrLf &
            "     WHERE TransReceiving.ID = TransInvoice.ReceivingID" & vbCrLf &
            " ) tblAccountsPayablePurchaseOrder" & vbCrLf

        pSql &= vWhere & vbCrLf &
        "  ORDER BY TransInvoice.InvoiceNumber" & vbCrLf &
        " " & vbCrLf
        pDtInvoice = mProData.Execute(pSql)
        If pDtInvoice.Rows.Count = 0 Then
            AppendToLog("ref: vbV4W79XZWHH! GetReportData invoice not found." & vbCrLf & pSql)
        End If
        pDtInvoice.TableName = "Invoices"
        pDtInvoice.Columns.Add("SavedAsFinal", GetType(Boolean))
        pDtInvoice.Columns.Add("RefQuote")
        pDtInvoice.Columns.Add("RefSalesOrder")
        pDtInvoice.Columns.Add("RefPurchaseOrder")
        For Each pDR As DataRow In pDtInvoice.Rows
            pDR("SavedAsFinal") = IsSavedAsFinal(pDR)

            If pDR("RefQuotationNo") Is DBNull.Value Then
                pSql = "N/A"
            Else
                pSql = CStr2(pDR("RefQuotationNo"))
            End If
            pSql &= " - "
            If pDR("RefQuotationDate") Is DBNull.Value Then
                pSql &= "N/A"
            Else
                pSql &= Format(pDR("RefQuotationDate"), cGlobal.FormatDateTimeGrid)
            End If
            pDR("RefQuote") = pSql

            If pDR("RefSalesOrderNo") Is DBNull.Value Then
                pSql = "N/A"
            Else
                pSql = CStr2(pDR("RefSalesOrderNo"))
            End If
            pSql &= " - "
            If pDR("RefSalesOrderDate") Is DBNull.Value Then
                pSql &= "N/A"
            Else
                pSql &= Format(pDR("RefSalesOrderDate"), cGlobal.FormatDateTimeGrid)
            End If
            pDR("RefSalesOrder") = pSql

            If String.IsNullOrWhiteSpace(CStr2(pDR("PurchaseOrderText"))) Then
                pSql = "N/A"
            Else
                pSql = CStr2(pDR("PurchaseOrderText"))
            End If
            pSql &= " - "
            If pDR("PurchaseOrderDate") Is DBNull.Value Then
                pSql &= "N/A"
            Else
                pSql &= Format(pDR("PurchaseOrderDate"), cGlobal.FormatDateTimeGrid)
            End If
            pDR("RefPurchaseOrder") = pSql
        Next
        pDS.Tables.Add(pDtInvoice)
        Dim pTransType As String
        If pDtInvoice.Rows.Count > 0 Then
            pTransType = CStr2(pDtInvoice.Rows(0)("TransType"))
            If pTransType.Trim().ToLower() = "salesorder" Then
                pQuantityField = "OrderedQuantity"
            End If
        End If

        '// stocks ==============================================================================================================================================
        If pTransID IsNot Nothing _
            AndAlso IsGuid(CurrentOrgID) Then

            SaveProfitability(pTransID, CurrentOrgID, pQuantityField)
        End If

        Dim pReceivingID As String
        Dim pReceivingSupplierID As String

        If pDtInvoice.Rows.Count > 0 Then
            If CStr2(pDtInvoice.Rows(0)("TransType")).Trim().ToLower() = "AccountsPayable".ToLower() Then
                Dim pDrReceiving = mParent.TransReceiving.GetByAccountsPayableID(CStr2(pDtInvoice.Rows(0)("InvoiceID")))
                If pDrReceiving IsNot Nothing Then
                    pReceivingID = CStr2(pDrReceiving("ID"))
                    pReceivingSupplierID = CStr2(pDrReceiving("SupplierID"))
                End If
            End If
        End If

        pSql = "SELECT DISTINCT TransInvoiceItems.ID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.Brand" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName as ProductNameOnly" & vbCrLf &
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf &
            " ,TransInvoiceItems.Quantity" & vbCrLf &
            " ,TransInvoiceItems.QuantityBad" & vbCrLf &
            " ,TransInvoiceItems.HiddenServedQuantity" & vbCrLf &
            " ,TransInvoiceItems.OrderedQuantity" & vbCrLf &
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMMeasure.ID AS UnitMeasureID" & vbCrLf &
            " ,CONVERT(varchar, TransInvoiceItems.TermDiscount) + '% ' + CONVERT(varchar, TransInvoiceItems.DiscountIfPaidWithIn) + ' NET ' + CONVERT(varchar, TransInvoiceItems.NetDue) AS Terms" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice" & vbCrLf &
            " ,TransInvoiceItems.OriginalUnitPrice" & vbCrLf &
            " ,TransInvoiceItems.SubTotal" & vbCrLf &
            "   ,TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity AS OrderedAmount" & vbCrLf &
            "   ,TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity AS ServedAmount" & vbCrLf &
            " ,TransInvoiceItems.DiscountIfPaidWithIn" & vbCrLf &
            " ,TransInvoiceItems.NetDue" & vbCrLf &
            " ,TransInvoiceItems.TermNumericValue" & vbCrLf &
            " ,TransInvoiceItems.TermDiscount AS TermDiscountNumeric" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ,tblSrp.Price * TransInvoiceItems.ActualQuantity AS Srp" & vbCrLf &
            " ,tblPrice.LastUpdated AS PriceLevelLastUpdated" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.InvoiceID" & vbCrLf &
            " ,TransInvoiceItems.LineNumber" & vbCrLf &
            " ,TransInvoiceItems.DateCreated" & vbCrLf &
            " ,TransInvoiceItems.PriceLevelID " & vbCrLf &
            " ,TransInvoiceItems.Cost " & vbCrLf &
            " ,TransInvoiceItems.LineCost " & vbCrLf &
            " ,TransInvoiceItems.RequestedUnitPrice" & vbCrLf &
            " ,TransInvoiceItems.TaxPercent" & vbCrLf &
            " ,TransInvoiceItems.TaxAmount" & vbCrLf &
            " ,TransInvoiceItems.TaxSales" & vbCrLf &
            " ,TransInvoiceItems.NonVATSale" & vbCrLf &
            " ,TransInvoiceItems.VATExemptSale" & vbCrLf &
            " ,TransInvoiceItems.ZeroRatedSale" & vbCrLf &
            " ,TransInvoiceItems.ServedQtyIsEdited" & vbCrLf &
            " ,TransInvoiceItems.VendorID" & vbCrLf &
            " ,TransInvoiceItems.UnitGP" & vbCrLf &
            " ,TransInvoiceItems.UnitGPM" & vbCrLf &
            " ,TransInvoiceItems.LineGP" & vbCrLf &
            " ,TransInvoiceItems.ServedQtyIsEdited" & vbCrLf &
            " ,TransInvoiceItems.Area" & vbCrLf &
            " ,TransInvoiceItems.ServedDate" & vbCrLf &
            " ,TransInvoiceItems.CancelledByID" & vbCrLf &
            " ,TransInvoiceItems.ShippingRef" & vbCrLf &
            " ,TransInvoiceItems.ThirdPartyProductNumber" & vbCrLf &
            ",tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
            ",tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf &
            ",tblPurchaseOrder.PurchaseOrderStatus" & vbCrLf &
            ",tblPurchaseOrder.PurchaseOrderServedByVendor" & vbCrLf &
            " ,vendor.VendorCode" & vbCrLf &
            IIf(CBool2(mParent.SystemDefaults.EcommerceShowVendorName), " ,vendor.VendorName ", "") & vbCrLf &
            ",vendor.VendorName AS VendorName2" & vbCrLf &
            ", TransInvoiceItems.Upcharge " & vbCrLf &
            ", (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE Size = 'small' AND ProductID = TransInvoiceItems.ProductID AND IsDefault = 1 AND IsFromVendor = 0) as Image" & vbCrLf
        If vForPickingList Then
            pSql &= ",tblWithdrawn.PreparedQuantity" & vbCrLf
        End If
        If vIncludeStockReceivingData Then
            pSql &= " ,tblReceiving.QuantityGood" & vbCrLf
            pSql &= " ,tblReceiving.QuantityDamaged" & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(pReceivingID) = False Then
            pSql &= ",tblPurchaseOrderItem.PurchaseOrderQuantity" & vbCrLf
            pSql &= ",tblPurchaseOrderItem.PurchaseOrderPrice" & vbCrLf
            pSql &= ",tblPurchaseOrderItem.PurchaseOrderNumber" & vbCrLf
            pSql &= ",tblPurchaseOrderItem.PurchaseOrderDate" & vbCrLf
        End If
        pSql &= " FROM TransInvoiceItems " & vbCrLf &
               " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
               " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID  " & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.Label = TransInvoiceItems.UnitMeasure" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf
        pSql &= " OUTER APPLY (" & vbCrLf &
            " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
            "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
            "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " ) tblBaseUnit" & vbCrLf
        pSql &= " OUTER APPLY ( " & vbCrLf &
            "   SELECT VendorCode, org.CompanyName as VendorName " & vbCrLf &
            "   FROM LibraryOrganizationSupplier as sup" & vbCrLf &
            "   INNER JOIN MobizLibraryOrganization as org ON org.ID = sup.SupplierID " & vbCrLf &
            "   WHERE SupplierID = VendorID " & vbCrLf &
            " ) as vendor " & vbCrLf
        pSql &= " OUTER APPLY ( " & vbCrLf &
            "   SELECT TOP 1 TransSalesOrderStatus.Status AS PurchaseOrderStatus" & vbCrLf &
            "    ,CASE WHEN tblPo.FinalizePickListByID IS NULL" & vbCrLf &
            "    THEN 0" & vbCrLf &
            "    ELSE 1 END AS PurchaseOrderServedByVendor" & vbCrLf &
            "   FROM TransInvoice AS tblPo" & vbCrLf &
            "   JOIN TransSalesOrderStatus ON TransSalesOrderStatus.InvoiceID = tblPo.ID" & vbCrLf &
            "   WHERE tblPo.ID = TransInvoiceItems.PurchaseOrderID" & vbCrLf &
            "   ORDER BY TransSalesOrderStatus.LastUpdated DESC" & vbCrLf &
            " ) as tblPurchaseOrder " & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(modClient.SrpID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            "  ) tblSrp" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "   ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            "   FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "   WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = TransInvoiceItems.PriceLevelID" & vbCrLf &
            "   AND MobizLibraryPIMProductPriceLevel.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "   ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            "  ) tblPrice" & vbCrLf
        If vForPickingList Then
            pSql &= "  OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoiceItemWarehouseWithdrawal.PreparedQuantity) AS PreparedQuantity" & vbCrLf &
                "     FROM  TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
                "     WHERE TransInvoiceItemWarehouseWithdrawal.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                "     AND TransInvoiceItemWarehouseWithdrawal.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf &
                "     AND TransInvoiceItemWarehouseWithdrawal.UnitQty = TransInvoiceItems.ActualQuantity" & vbCrLf &
                "  ) tblWithdrawn" & vbCrLf
        End If
        If vIncludeStockReceivingData Then
            pSql &= "  OUTER APPLY (SELECT SUM(TransReceivingStock.QuantityGood) AS QuantityGood" & vbCrLf &
                ",SUM(TransReceivingStock.QuantityDamaged) AS QuantityDamaged" & vbCrLf &
                "  FROM TransReceiving" & vbCrLf &
                "  JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID" & vbCrLf &
                "  WHERE TransReceiving.PurchaseOrderID = TransInvoiceItems.InvoiceID" & vbCrLf &
                "  AND TransReceivingStock.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                "  AND TransReceivingStock.ActualQuantityGood = TransInvoiceItems.ActualQuantity" & vbCrLf &
                " ) tblReceiving" & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(pReceivingID) = False Then
            pSql &= " OUTER APPLY (SELECT tblPoItem.Quantity" & vbCrLf &
                " ,tblPoItem.OrderedQuantity AS PurchaseOrderQuantity" & vbCrLf &
                " ,tblPoItem.UnitPrice AS PurchaseOrderPrice" & vbCrLf &
                " ,TransInvoice.PurchaseOrderDate" & vbCrLf &
                " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.PurchaseOrderNo) AS PurchaseOrderNumber" & vbCrLf &
                " FROM TransInvoiceItems tblPoItem" & vbCrLf &
                " JOIN TransInvoice ON TransInvoice.ID = tblPoItem.InvoiceID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
                " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
                " WHERE tblPoItem.InvoiceID IN" & vbCrLf &
                " (" & vbCrLf &
                "   SELECT TOP 1 TransReceiving.PurchaseOrderID" & vbCrLf &
                "   FROM TransReceiving" & vbCrLf &
                "   WHERE TransReceiving.ID = " & ReadyString(pReceivingID) & vbCrLf &
                " )" & vbCrLf &
                " AND tblPoItem.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                " AND tblPoItem.ActualQuantity = TransInvoiceItems.ActualQuantity" & vbCrLf &
                ") tblPurchaseOrderItem" & vbCrLf
        End If
        pSql &= pWhereItems & vbCrLf

        If vIncludeZeroQuantity = False Then
            pSql &= " AND (TransInvoiceItems.Quantity > 0 OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf
        End If
        If vExcludeHiddenQuantity _
            AndAlso pTransType.Trim.ToLower() <> "salesorder" Then

            pSql &= " AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf
        End If
        If vForPickingList Then
            pSql &= " AND TransInvoiceItems.ProductID NOT IN (" & vbCrLf &
                " 	SELECT TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
                " 	FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
                " 	JOIN TransInvoiceItemWarehouseSections ON TransInvoiceItemWarehouseSections.InvoiceID = TransInvoiceItemWarehouseWithdrawal.InvoiceID" & vbCrLf &
                " 	AND TransInvoiceItemWarehouseSections.ProductID = TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
                " 	AND TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
                " 	WHERE TransInvoiceItemWarehouseWithdrawal.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf &
                "   AND TransInvoiceItemWarehouseWithdrawal.SectionID = " & ReadyString(vSectionID) & vbCrLf &
                " )" & vbCrLf
            If IsGuid(vSectionID) Then
                pSql &= "  AND (EXISTS(" & vbCrLf &
                    "  	SELECT *" & vbCrLf &
                    "     FROM TransInvoiceItemsPickingFowsts" & vbCrLf &
                    "     JOIN MobizLibraryOrganizationFOWSTSection ON MobizLibraryOrganizationFOWSTSection.FowstID = TransInvoiceItemsPickingFowsts.FowstID" & vbCrLf &
                    "     WHERE TransInvoiceItemsPickingFowsts.InvoiceID = TransInvoice.ID" & vbCrLf &
                    "     AND MobizLibraryOrganizationFOWSTSection.ID = " & ReadyString(vSectionID) & vbCrLf &
                    "     AND TransInvoiceItemsPickingFowsts.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                    "  ) -- end exists" & vbCrLf &
                    " OR " & vbCrLf &
                    "         -- there more invoice items than there are item picking fowst" & vbCrLf &
                    "         (" & vbCrLf &
                    "         	(" & vbCrLf &
                    "               SELECT COUNT(*)" & vbCrLf &
                    "               FROM TransInvoiceItemsPickingFowsts" & vbCrLf &
                    "               WHERE TransInvoiceItemsPickingFowsts.InvoiceID = TransInvoice.ID" & vbCrLf &
                    "               AND TransInvoiceItemsPickingFowsts.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                    " 			) " & vbCrLf &
                    "             < " & vbCrLf &
                    " 			(" & vbCrLf &
                    "               SELECT COUNT(*)" & vbCrLf &
                    "               FROM TransInvoiceItems t" & vbCrLf &
                    "               WHERE t.InvoiceID = TransInvoice.ID" & vbCrLf &
                    "               AND t.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                    "             )" & vbCrLf &
                    "         )" & vbCrLf &
                    ") -- END AND" & vbCrLf


            End If
        End If
        pSql &= " ORDER BY TransInvoiceItems.DateCreated, TransInvoiceItems.LineNumber, ProductName, ActualQuantity"
        'additional by janine sept 5 for sales order taker
        'for sales order taker OrderedQuantity
        ''''''''''''''''''''''''''''''''''''''''''''

        ' LEFT JOIN because TransModifiedInvoiceItems sometimes doesnt link to a valid StockID
        pDT = mProData.Execute(pSql)

        If String.IsNullOrWhiteSpace(pReceivingID) = False Then
            pDT.Columns.Add("AccountsPayablePoPrice", GetType(Decimal))
            pDT.Columns.Add("AccountsPayablePoDate", GetType(Date))
            pDT.Columns.Add("AccountsPayablePoType") ' PO, LPO, LC
            pDT.Columns.Add("AccountsPayablePoNum") ' filled only if AccountsPayablePoType = PO
            pDT.Columns.Add("LastApPrice", GetType(Decimal))
            pDT.Columns.Add("LastApDate", GetType(Date))
        End If
        For Each pDR As DataRow In pDT.Rows
            If String.IsNullOrWhiteSpace(pReceivingID) = False Then
                Dim pDrPoLastPoOrLastCanvass = mParent.TransReceiving.GetPoLastPoOrLastCanvass(pReceivingID, pReceivingSupplierID, CStr2(pDR("ProductID")))
                If pDrPoLastPoOrLastCanvass IsNot Nothing Then
                    pDR("AccountsPayablePoPrice") = CDec2(pDrPoLastPoOrLastCanvass("UnitPrice")) / CDec2(pDrPoLastPoOrLastCanvass("ActualQuantity")) * CDec2(pDR("ActualQuantity"))
                    pDR("AccountsPayablePoDate") = pDrPoLastPoOrLastCanvass("LastUpdated")
                    pDR("AccountsPayablePoType") = pDrPoLastPoOrLastCanvass("PoType")
                    If CStr2(pDrPoLastPoOrLastCanvass("PoType")) = "PO" Then
                        pDR("AccountsPayablePoNum") = pDrPoLastPoOrLastCanvass("PurchaseOrderNumber")
                    End If
                End If
                pDrPoLastPoOrLastCanvass = LastAccountsPayable(pReceivingSupplierID, CStr2(pDR("ProductID")), CStr2(pDtInvoice.Rows(0)("InvoiceID")))
                If pDrPoLastPoOrLastCanvass IsNot Nothing Then
                    pDR("LastApPrice") = CDec2(pDrPoLastPoOrLastCanvass("UnitPrice")) / CDec2(pDrPoLastPoOrLastCanvass("ActualQuantity")) * CDec2(pDR("ActualQuantity"))
                    pDR("LastApDate") = pDrPoLastPoOrLastCanvass("LastUpdated")
                End If
            End If
            If String.IsNullOrWhiteSpace(CStr2(pDR("VendorName2"))) = False Then
                pDR("ProductName") = pDR("ProductName") & " (" & pDR("VendorName2") & ")"
            End If
        Next pDR
        If vAddUnitExplaination Then
            pDT = AddUnitsExplanation(pDT)
        End If
        pDT.TableName = "Stocks"
        pDS.Tables.Add(pDT)

        pDtInvoice.Columns.Add("Subtotal", GetType(Decimal))
        pDtInvoice.Columns.Add("OrderedAmount", GetType(Decimal))
        pDtInvoice.Columns.Add("ServedAmount", GetType(Decimal))

        For Each pDR As DataRow In pDtInvoice.Rows
            'pDR("DueDate") = CDate(pDR("InvoiceDate")).AddDays(pDR("TermNumericValue")) ' not used in Mobiz
            pDR("SubTotal") = CDec2(pDT.Compute("SUM(SubTotal)", "InvoiceID = " & ReadyString(pDR("InvoiceID"))))
            pDR("OrderedAmount") = CDec2(pDT.Compute("SUM(OrderedAmount)", "InvoiceID = " & ReadyString(pDR("InvoiceID"))))
            pDR("ServedAmount") = CDec2(pDT.Compute("SUM(ServedAmount)", "InvoiceID = " & ReadyString(pDR("InvoiceID"))))
        Next pDR

        If vIncludeVendors Then
            pDT = GetSuppliers(pDT)
            If pDT IsNot Nothing Then
                pDS.Tables.Add(pDT)
            End If
        End If

        Return pDS

    End Function

    Private Function GetSuppliers(vDtInvoiceProducts As DataTable) As DataTable

        If vDtInvoiceProducts Is Nothing _
            OrElse vDtInvoiceProducts.Rows.Count = 0 Then

            Return Nothing
        End If

        Dim pIDs = ""
        Dim pDtProductIDs = ExtractFieldValues(vDtInvoiceProducts, "ProductID", Integer.MaxValue)
        For Each pDrProductID In pDtProductIDs.Rows
            If pIDs <> "" Then
                pIDs &= ","
            End If
            pIDs &= pDrProductID("ProductID")
        Next pDrProductID

        Dim pSelect As String
        Dim pFrom As String

        BrowserController.ComposeCustomer(pSelect, pFrom)

        Dim pSql = "SELECT LibraryVendorPrices.VendorID" & vbCrLf &
            ",LibraryVendorPrices.ProductID" & vbCrLf &
            "," & pSelect & " AS SupplierName" & vbCrLf &
            " FROM LibraryVendorPrices" & vbCrLf &
            " JOIN MobizLibraryOrganization ON LibraryVendorPrices.VendorID = MobizLibraryOrganization.ID" & vbCrLf &
            pFrom &
            " WHERE LibraryVendorPrices.ProductID IN (" & pIDs & ")" & vbCrLf &
            " ORDER BY SupplierName"
        Dim pDT = mProData.Execute(pSql)
        pDT.TableName = "Suppliers"

        Return pDT

    End Function

    Public Function UpdateSalesRepText() As Integer

        Dim pSql = "SELECT TransInvoice.ID" & vbCrLf &
            "," & BrowserController.ComposePeopleName("tblSalesRep", "SalesRepNominalSuffix") & " AS SalesRep" & vbCrLf &
            ",TransInvoice.SalesRepText" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople tblSalesRep ON tblSalesRep.ID = TransInvoice.SalesRepPeopleID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix SalesRepNominalSuffix ON tblSalesRep.TitleID = SalesRepNominalSuffix.ID" & vbCrLf &
            " WHERE NULLIF(LTRIM(RTRIM(TransInvoice.SalesRepText)), '') IS NULL" &
            " AND TransInvoice.SalesRepPeopleID IS NOT NULL" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated"

        pSql = " UPDATE TransInvoice" & vbCrLf &
            "  SET TransInvoice.SalesRepText = ISNULL(NULLIF(LTRIM(RTRIM(SalesRepNominalSuffix.Code)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(tblSalesRep.Surname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(tblSalesRep.FirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(tblSalesRep.MiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(tblSalesRep.PersonalSuffixName)),', '), '') + ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(tblSalesRep.ID))),' '), '')" & vbCrLf &
            "  FROM TransInvoice" & vbCrLf &
            "  LEFT JOIN MobizLibraryPeople tblSalesRep ON tblSalesRep.ID = TransInvoice.SalesRepPeopleID" & vbCrLf &
            "  LEFT JOIN MobizLibraryPeoplePreNominalSuffix SalesRepNominalSuffix ON tblSalesRep.TitleID = SalesRepNominalSuffix.ID" & vbCrLf &
            "  WHERE NULLIF(LTRIM(RTRIM(TransInvoice.SalesRepText)), '') IS NULL " & vbCrLf &
            "  AND TransInvoice.SalesRepPeopleID IS NOT NULL"
        Return mProData.ExecuteSQL(pSql)

    End Function

    Private Function AddUnitsExplanation(ByVal vDT As DataTable) As DataTable
        ' PARAMS:
        '   vDT = vwReportStockPickingList

        Dim pStr As String
        Dim pBaseUnit As String
        Dim pDR As DataRow

        If vDT.Rows.Count = 0 Then Return vDT

        For Each pDR In vDT.Rows
            pStr = UnitExplanation(CStr2(pDR("StockID")), pDR("ActualQuantity"), pDR("UnitMeasure"), pBaseUnit)
            If pStr <> "" Then
                pStr = " [" & pStr & "]"
                pStr = pStr & " [" & pDR("ActualQuantity") * pDR("Quantity") & " " & pBaseUnit & " total]"
                If vDT.Columns.Contains("Stock Description") Then
                    pDR("Stock Description") = pDR("Stock Description") & pStr
                Else
                    pDR("StockDescription") = pDR("StockDescription") & pStr
                End If
            End If
        Next pDR

        Return vDT

    End Function

    Private Function UnitExplanation(ByVal vStockID As String,
        ByVal vActualQuantity As Integer,
        ByVal vUnit As String,
        Optional ByRef oBaseUnit As String = Nothing) As String

        Dim pUnit As String

        If vActualQuantity = 1 Then Exit Function
        pUnit = BaseUnit(vStockID)
        If pUnit <> "" Then
            oBaseUnit = pUnit
            UnitExplanation = vActualQuantity & " " & pUnit & " / " & vUnit
        End If

    End Function

    Private Function BaseUnit(ByVal vStockID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryUnitMeasures.UnitMeasure" &
            " ,LibraryStockMeasurements.Quantity" &
            " FROM LibraryStockMeasurements" &
            " JOIN LibraryUnitMeasures" &
            " ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID" &
            " WHERE StockID = " & ReadyString(vStockID) &
            " AND LibraryStockMeasurements.Quantity = 1"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            BaseUnit = pDT.Rows(0)("UnitMeasure")
        End If

    End Function
#End Region

    Public Function SaveRemarks(ByVal vID As String, ByVal vRemarks As String) As Boolean

        Dim pSql As String
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("TransInvoice")
        pIU("Remarks") = ReadyString(vRemarks)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pSql = mProData.ExecuteSQL(pSql, True)

        Return CDec(pSql) > 0

    End Function

    Public Sub UpdateAmounts(ByVal vID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pGrossSrvAmount As Decimal
        Dim pGrossRefundAmount As Decimal
        Dim pTotalAdjustments As Decimal

        pIU = New InsertUpdate("TransInvoice")
        pIU("TotalAmount") = Math.Round(mParent.TransInvoice.ComputeTotal(vID, pGrossSrvAmount, pGrossRefundAmount, pTotalAdjustments), 2)

        pIU("GrossSrvAmount") = pGrossSrvAmount
        pIU("GrossRefundAmount") = pGrossRefundAmount
        pIU("TotalAdjustments") = pTotalAdjustments
        pIU("NetInvoiceAmount") = Math.Round(CDec2(pIU("TotalAmount")) + CDec2(pIU("TotalAdjustments")), 2)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)
        AppendToDebug("vbUB5FG5LGL7! TransInvoice.vb.UpdateAmounts. " & vbCrLf & pSql, True)

    End Sub

    Public SwInventoryGetCart As New Stopwatch
    Public SwInventoryMisc As New Stopwatch
    Public SwInventoryModifier As New Stopwatch
    Public SwInventoryIsHasPossibleReconstruction As New Stopwatch
    Public SwInventoryDataNeedsReconstruction As New Stopwatch
    Public SwInventoryAddItemToInventoryLedger As New Stopwatch

    Public Function AddToInventoryLedger(ByVal vID As String, ByVal vOrgID As String,
        Optional ByVal IsHasPossibleReconstruction As Boolean = False,
        Optional vTransType As TransInventoryLedger.eTransType = TransInventoryLedger.eTransType.Invoice) As Boolean
        ' subtransaction type = [Production, Usage] . apr 16, 2015

        Dim pDtStocks As DataTable
        Dim pDrInvoice As DataRow
        Dim pDtStocksUnique As DataTable
        Dim pSql As String
        Dim pDtModifier As DataTable
        Dim pMultiplier As Integer = 1

        SwInventoryGetCart.Start()
        If vTransType = TransInventoryLedger.eTransType.CancelInvoice Then
            pMultiplier = -1
        End If
        SwInventoryGetCart.Stop()
        SwInventoryMisc.Start()
        pDrInvoice = ById(vID)
        If pDrInvoice Is Nothing Then
            mParent.ExtraLog("vbWERG38F4MH! vID:" & vID & "; GetCurrentDatabase:" & mProData.GetCurrentDatabase())
            SwInventoryMisc.Stop()
            Return False
        End If

        pSql = "SELECT ProductID" & vbCrLf &
            ",Quantity" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",BinID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
        pDtStocks = mProData.Execute(pSql)
        pDtStocks.Columns.Add("TotalQty", GetType(Decimal), "Quantity * ActualQuantity")
        For Each pDR As DataRow In pDtStocks.Rows
            If CDec2(pDR("ActualQuantity")) = 0 Then
                pDR("ActualQuantity") = 1
            End If
        Next pDR
        pDtStocks.AcceptChanges()
        pDtStocks.Columns.Add("PriceInBaseUnit", GetType(Decimal), "UnitPrice / ActualQuantity")
        SwInventoryMisc.Stop()

        SwInventoryModifier.Start()
        pSql = " SELECT LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            " ,TransInvoiceItemsModifier.OrderedQty * LibraryItemModifierInventoryAffecting.Qty AS Quantity" & vbCrLf &
            " ,CONVERT(decimal, 1) AS ActualQuantity" & vbCrLf &
            " ,TransInvoiceItemsModifier.ModifierPrice AS UnitPrice" & vbCrLf &
            " ,'Usage Via Item Modifer' AS SubTransactionType" & vbCrLf &
            " FROM LibraryItemModifierInventoryAffecting" & vbCrLf &
            " JOIN TransInvoiceItemsModifier ON TransInvoiceItemsModifier.ModifierID = LibraryItemModifierInventoryAffecting.ID" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID" & vbCrLf &
            " WHERE TransInvoiceItemsModifier.OrderedQty > 0" & vbCrLf &
            " AND TransInvoiceItems.InvoiceID = " & ReadyString(vID)
        ' old code: ,TransInvoiceItemsModifier.OrderedQty * LibraryItemModifierInventoryAffecting.Qty * TransInvoiceItems.Quantity AS Quantity
        pDtModifier = mProData.Execute(pSql)
        pDtStocks.Merge(pDtModifier)

        pDtStocksUnique = New DataView(pDtStocks, "TotalQty > 0", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID", "PriceInBaseUnit", "SubTransactionType", "BinID"})
        If pDtStocksUnique.Rows.Count = 0 Then
            mParent.ExtraLog("vbHN8WQYGEA9! vID:" & vID)
            SwInventoryModifier.Stop()
            Return False
        End If
        mProData.BeginTransaction()
        SwInventoryModifier.Stop()
        For Each pDR As DataRow In pDtStocksUnique.Rows
            Dim TransactionBalanceBeforeCount As String
            Dim pDataNeedsReconstruction As Boolean
            Dim dtTransactionAfterCount As DataTable

            If IsHasPossibleReconstruction Then
                SwInventoryIsHasPossibleReconstruction.Start()
                Dim pOut As String
                Dim pInventoryCountDate As String

                pSql = "SELECT BinID" &
                        " FROM LibraryFowstBinProduct" &
                        " WHERE FowstID = " & ReadyString(pDrInvoice("FowstID")) &
                        " AND ProductID = " & ReadyString(pDR("ProductID")) &
                        " AND IsDefault = 1 "
                pOut = CStr2(mProData.ExecuteScalar(pSql))

                If pOut <> "" Then
                    pSql = "Select top 1 InventoryCountDate from TransInventoryProductCountStock " & vbCrLf &
                            "INNER JOIN TransInventoryProductCount " & vbCrLf &
                            "ON TransInventoryProductCountStock.InventoryProductCountID = TransInventoryProductCount.ID " & vbCrLf &
                            "WHERE " & vbCrLf &
                            "TransInventoryProductCount.FowstID = " & ReadyString(pDrInvoice("FowstID")) & vbCrLf &
                            "AND TransInventoryProductCountStock.BinID = " & ReadyString(pOut) & vbCrLf &
                            "AND TransInventoryProductCount.ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                            " Order by InventoryCountDate desc"

                    pInventoryCountDate = CStr2(mProData.ExecuteScalar(pSql))
                    If IsDate(pInventoryCountDate) Then
                        If (DateDiff(DateInterval.Second, CDate(pDrInvoice("InvoiceDate")), CDate(pInventoryCountDate)) > 0) Then
                            'pDataNeedsReconstruction = True

                            pSql = "Select * from TransInventoryLedger " & vbCrLf &
                                    "where LastUpdated >= " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                                    "AND TransInventoryLedger.FowstID = " & ReadyString(pDrInvoice("FowstID")) & vbCrLf &
                                    "AND TransInventoryLedger.BinID = " & ReadyString(pOut) & vbCrLf &
                                    "AND TransInventoryLedger.ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                                    " ORDER BY LastUpdated asc"
                            dtTransactionAfterCount = mProData.Execute(pSql)

                            pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                                    "where LastUpdated < " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                                    "AND TransInventoryLedger.FowstID = " & ReadyString(pDrInvoice("FowstID")) & vbCrLf &
                                    "AND TransInventoryLedger.BinID = " & ReadyString(pOut) & vbCrLf &
                                    "AND TransInventoryLedger.ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                                    " ORDER BY LastUpdated desc"

                            TransactionBalanceBeforeCount = mProData.ExecuteScalar(pSql)
                        End If
                    End If
                End If
                SwInventoryIsHasPossibleReconstruction.Stop()
            End If

            If pDataNeedsReconstruction Then
                SwInventoryDataNeedsReconstruction.Start()
                mParent.TransInventoryLedger.Add(
                vStockID:=CStr2(pDR("ProductID")),
                vFowstID:=CStr2(pDrInvoice("FowstID")),
                vTransactionID:=vID,
                vTransactionType:=TransInventoryLedger.eTransType.Invoice,
                vUserID:=CStr2(pDrInvoice("UserID")),
                vQty:=-CDec2((pDtStocks.Compute("SUM(TotalQty)", "ProductID = " & ReadyString(pDR("ProductID"))))),
                vBalance:=CDec2(TransactionBalanceBeforeCount + -(pDtStocks.Compute("SUM(TotalQty)", "ProductID = " & ReadyString(pDR("ProductID")))))
            )
                If dtTransactionAfterCount.Rows.Count > 0 Then
                    For i As Integer = 0 To dtTransactionAfterCount.Rows.Count - 1
                        Dim mTransType As TransInventoryLedger.eTransType

                        Select Case CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionType"))
                            Case "Invoice"
                                mTransType = TransInventoryLedger.eTransType.Invoice
                            Case "InventoryCount"
                                mTransType = TransInventoryLedger.eTransType.InventoryCount
                            Case "Receiving"
                                mTransType = TransInventoryLedger.eTransType.Receiving
                            Case "TransferOut"
                                mTransType = TransInventoryLedger.eTransType.TransferOut
                            Case "Sales Returns"
                                mTransType = TransInventoryLedger.eTransType.SalesReturn
                            Case "Product Maker"
                                mTransType = TransInventoryLedger.eTransType.SalesReturn
                        End Select

                        If mTransType <> TransInventoryLedger.eTransType.InventoryCount Then
                            mParent.TransInventoryLedger.Add(
                            vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                            vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                            vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                            vTransactionType:=mTransType,
                            vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                            vQty:=CDec2(dtTransactionAfterCount.Rows(i).Item("Qty"))
                        )
                        Else
                            mParent.TransInventoryLedger.Add(
                            vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                            vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                            vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                            vTransactionType:=mTransType,
                            vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                            vQty:=Nothing,
                            vBalance:=CDec2(dtTransactionAfterCount.Rows(i).Item("Balance"))
                        )
                        End If

                    Next
                End If
                SwInventoryDataNeedsReconstruction.Stop()
            Else
                SwInventoryAddItemToInventoryLedger.Start()
                AddToTempLogs("vb64QMZ2YWK9! productID: " & CStr2(pDR("ProductID")) & vbCrLf &
                    "InvoiceID=" & vID)
                AddItemToInventoryLedger(
                    vProductID:=CStr2(pDR("ProductID")),
                    vOrgID:=vOrgID,
                    vFowstID:=CStr2(pDrInvoice("FowstID")),
                    vBinID:=CStr2(pDR("BinID")),
                    vTransactionID:=vID,
                    vUserID:=CStr2(pDrInvoice("UserID")),
                    vQty:=-CDec2(pDtStocks.Compute("SUM(TotalQty)",
                        "ProductID = " & ReadyString(pDR("ProductID")) &
                        " AND ISNULL(BinID, '') = " & ReadyString(pDR("BinID")) &
                        " AND PriceInBaseUnit = " & CDec2(pDR("PriceInBaseUnit")))) * pMultiplier,
                    vPrice:=CDec2(pDR("PriceInBaseUnit")),
                    vTransType:=vTransType,
                    vSubTransactionType:=CStr2(pDR("SubTransactionType"))
                )
                SwInventoryAddItemToInventoryLedger.Stop()
            End If

        Next pDR

        SwInventoryMisc.Start()
        AddToTempLogs("vb4RH3FNB8GT! before CommitTransaction" & vbCrLf &
            "InvoiceID=" & vID)
        mProData.CommitTransaction()
        SwInventoryMisc.Stop()

        Return True

    End Function

    ''' <summary>
    ''' this will auto make Production & Usage entries
    ''' </summary>
    ''' <param name="vProductID"></param>
    ''' <param name="vFowstID"></param>
    ''' <param name="vTransactionID"></param>
    ''' <param name="vUserID"></param>
    ''' <param name="vQty">qty sold</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Sub AddItemToInventoryLedger(ByVal vProductID As String,
        ByVal vOrgID As String,
        ByVal vFowstID As String,
        ByVal vTransactionID As String,
        ByVal vUserID As String,
        ByVal vQty As Decimal,
        Optional ByVal vBinID As String = "",
        Optional ByVal vParentID As String = "",
        Optional vSubTransactionType As String = "",
        Optional vTransType As TransInventoryLedger.eTransType = TransInventoryLedger.eTransType.Invoice,
        Optional vPrice As Decimal? = Nothing)

        Dim pDrBom As DataRow
        Dim pDtContents As DataTable
        Dim pLedgerID As String
        Dim pChildProductID As String
        Dim pSubTransactionType As String
        Dim pUnitQty As Decimal

        If CStr2(vSubTransactionType) <> "" Then
            pSubTransactionType = vSubTransactionType
        Else
            pSubTransactionType = If(CStr2(vParentID) = "", "Sale", "Usage Via BOM")
        End If

        ' expect vQty to be negative
        pLedgerID = mParent.TransInventoryLedger.Add(
           vStockID:=vProductID,
            vFowstID:=vFowstID,
            vTransactionID:=vTransactionID,
            vTransactionType:=vTransType,
            vQty:=vQty,
            vUserID:=vUserID,
            vSubTransactionType:=pSubTransactionType,
            vParentID:=vParentID,
            vPrice:=vPrice,
            vBinID:=vBinID
        )

        pDrBom = mParent.LibraryStock.GetBomHeader(vProductID, vOrgID)
        If pDrBom Is Nothing Then
            Exit Sub
        End If

        If pDrBom("Jit") = False Then
            Exit Sub
        End If

        ' expect Add to get a positive vQty
        pLedgerID = mParent.TransInventoryLedger.Add(
            vStockID:=vProductID,
            vFowstID:=vFowstID,
            vTransactionID:=vTransactionID,
            vTransactionType:=vTransType,
            vQty:=-vQty,
            vUserID:=vUserID,
            vSubTransactionType:="Production",
            vParentID:=pLedgerID
        )

        pDtContents = mParent.LibraryStock.GetContents(vProductID, vOrgID)
        For Each pDrContent As DataRow In pDtContents.Rows
            pChildProductID = CStr2(pDrContent("ChildProductID"))
            Dim pDrUnit = mParent.LibraryStockMeasurements.GetUnitMeasureQty(pChildProductID, CStr2(pDrContent("UnitMeasure")))
            If pDrUnit Is Nothing Then
                pUnitQty = mParent.LibraryStockMeasurements.StandardToBaseUnitQty(pChildProductID, vOrgID, CStr2(pDrContent("UnitMeasure")))
            Else
                pUnitQty = CDec2(pDrUnit("Quantity"))
                If pUnitQty = 0 Then
                    pUnitQty = 1
                End If
            End If
            AddItemToInventoryLedger(
                vProductID:=pChildProductID,
                vOrgID:=vOrgID,
                vFowstID:=vFowstID,
                vTransactionID:=vTransactionID,
                vUserID:=vUserID,
                vQty:=pDrContent("Quantity") * vQty * pUnitQty,
                vParentID:=pLedgerID,
                vPrice:=vPrice,
                vTransType:=vTransType
            )
        Next pDrContent

    End Sub

    '' not used. apr 16, 2015
    '' remove this?
    'Public Sub AddToInventoryLedgerBOM(ByVal vID As String)

    '    Dim pDS As DataSet
    '    Dim pDtInvoice As DataTable
    '    Dim pDtStocks As DataTable
    '    Dim pDrInvoice As DataRow
    '    Dim pDtStocksUnique As DataTable

    '    pDS = GetCart(vID)
    '    pDtInvoice = pDS.Tables("Invoices")
    '    If pDtInvoice.Rows.Count = 0 Then
    '        Exit Sub
    '    End If
    '    pDrInvoice = pDtInvoice.Rows(0)
    '    pDtStocks = pDS.Tables("Stocks")
    '    pDtStocks.Columns.Add("TotalQty", GetType(Decimal), "Quantity * ActualQuantity")
    '    pDtStocksUnique = New DataView(pDtStocks, "TotalQty > 0", "", DataViewRowState.CurrentRows).ToTable(True, {"ProductID", "UnitMeasure"})
    '    mProData.BeginTransaction()
    '    For Each pDR As DataRow In pDtStocksUnique.Rows
    '        mParent.TransInventoryLedger.Add( _
    '            CStr2(pDR("ProductID")), _
    '            CStr2(pDR("UnitMeasure")), _
    '            CStr2(pDrInvoice("FowstID")), _
    '            vID, _
    '            TransInventoryLedger.eTransType.Invoice, _
    '            CStr2(pDrInvoice("UserID")),
    '            -(pDtStocks.Compute("SUM(TotalQty)", "ProductID = " & ReadyString(pDR("ProductID"))))
    '        )
    '    Next pDR
    '    mProData.CommitTransaction()

    'End Sub

    Public Function CartSummary(ByVal vID As String) As CartSummaryClass

        Dim pOut As CartSummaryClass
        Dim pDtStocks As DataTable

        pOut = New CartSummaryClass

        pDtStocks = GetCart(vID).Tables("Stocks")
        pOut.Qty = pDtStocks.Compute("SUM(Quantity)", "")
        pOut.Amount = pDtStocks.Compute("SUM(SubTotal)", "")
        pDtStocks = New DataView(pDtStocks, "", "", DataViewRowState.CurrentRows).ToTable(True, "ProductID")
        pOut.SkuCount = pDtStocks.Rows.Count

        Return pOut

    End Function

    Public Class CartSummaryClass

        Public Qty As Decimal
        Public SkuCount As Integer
        Private mAmount As Decimal

        Public Property Amount As Decimal
            Get

                Return mAmount

            End Get
            Set(ByVal value As Decimal)

                mAmount = Math.Round(value, 2)

            End Set
        End Property

    End Class

    Public Function GetMaxDate(ByVal vFowstID As String, ByVal vDeviceID As String,
        Optional ByVal vExcludeID As String = Nothing) As Date?

        Dim pDR As DataRow
        Dim pSql As String

        If CStr2(vExcludeID).Length > 0 Then
            pSql = " AND TransInvoice.ID <> " & ReadyString(vExcludeID)
        End If
        pSql = "SELECT MAX(LastUpdated)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND CONVERT(varchar(36), TransInvoice.DeviceID) = " & ReadyString(vDeviceID) & vbCrLf &
            pSql
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR(0)
        End If

    End Function

    Public Function IsValidAdjustedDate(ByVal vDate As Date, ByVal vFowstID As String, ByVal vDeviceID As String, ByRef oError As String,
        Optional ByVal vID As String = Nothing) As Boolean

        Dim pDate As Date?

        oError = ""
        If vDate > mProData.GetDate Then
            oError = "Date must not be later than the current date and time."
            Return False
        End If

        'pDate = GetMaxDate(vFowstID, vDeviceID, vID)
        'If pDate IsNot Nothing Then
        '    If vDate < pDate Then
        '        oError = "Date Time must not be earlier than latest transaction date"
        '        Return False
        '    End If
        'End If

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns>number of updated records</returns>
    ''' <remarks></remarks>
    Public Function UpdatePaymentDate(ByVal vID As String) As Integer


    End Function

    Private Shared ReadOnly mCounterLock As New Object

    Public Function VerifyCounter(ByVal vDeviceID As String, ByVal vFowstID As String, ByRef oCounter As Integer,
        Optional vDocumentNameID As String = Nothing,
        Optional ByRef oFowstWideInvoiceNumber As Integer? = Nothing,
        Optional vFieldName As String = Nothing,
        Optional vInvoiceFieldName As String = Nothing) As Boolean

        Dim pSql As String
        Dim pMax As Integer
        Dim pDR As DataRow
        Dim pSeriesStartingNum As Integer
        Dim pUniversalNumberSeries As Boolean

        SyncLock mCounterLock
            If String.IsNullOrWhiteSpace(vFieldName) Then
                vFieldName = "POSLastORNo"
            End If
            vFieldName = EscapeSqlName(vFieldName)
            If String.IsNullOrWhiteSpace(vInvoiceFieldName) Then
                vInvoiceFieldName = "InvoiceNumber"
            End If
            vInvoiceFieldName = EscapeSqlName(vInvoiceFieldName)
            If String.IsNullOrWhiteSpace(vDocumentNameID) = False Then
                pSql = "SELECT SeriesType" & vbCrLf &
                    ",SeriesStartingNum" & vbCrLf &
                    ",UniversalNumberSeries" & vbCrLf &
                    " FROM LibraryDocumentControl" & vbCrLf &
                    " WHERE ID = " & ReadyString(vDocumentNameID)
                pDR = mProData.ExecuteRow(pSql)
                If pDR IsNot Nothing Then
                    pUniversalNumberSeries = CBool2(pDR("UniversalNumberSeries"))
                    pSeriesStartingNum = CInt2(pDR("SeriesStartingNum")) - 1
                    If CInt2(pDR("SeriesType")) = 1 Then
                        pSql = "SELECT MAX(FowstWideInvoiceNumber)" & vbCrLf &
                            " FROM TransInvoice" & vbCrLf &
                            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf
                        oFowstWideInvoiceNumber = CDec2(mProData.ExecuteScalar(pSql))
                        oFowstWideInvoiceNumber = Math.Max(oFowstWideInvoiceNumber.Value, pSeriesStartingNum)
                    End If
                End If

                pSql = " AND DocumentNameID = " & ReadyString(vDocumentNameID) & vbCrLf
            End If
            pSql = "SELECT MAX(" & vInvoiceFieldName & ")" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE 1=1" & vbCrLf &
                pSql
            If pUniversalNumberSeries = False Then
                pSql &= " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
                    " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf
            End If
            pMax = CDec2(mProData.ExecuteScalar(pSql))
            pMax = Math.Max(pMax, pSeriesStartingNum)

            If String.IsNullOrWhiteSpace(vDocumentNameID) Then
                pSql = "SELECT " & vFieldName & vbCrLf &
                    " FROM LibraryDeviceFowst" & vbCrLf &
                    " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
                    " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf
                oCounter = CDec2(mProData.ExecuteScalar(pSql))
            End If

            If pMax <= oCounter Then
                'Return True
            Else
                oCounter = pMax ' TODO: verify business logic here
            End If

            Dim pFilename = Common.ServerPath & "settings\Counters\"
            ProPack.Utility.AddDirectoryUser(pFilename)
            pFilename &= ReadyGuid(vFowstID) & "-" & ReadyGuid(vDeviceID) & "-"
            If String.IsNullOrWhiteSpace(vDocumentNameID) = False Then
                pFilename &= ReadyGuid(vDocumentNameID) & "-"
            End If
            pFilename &= vInvoiceFieldName & ".txt"
            Dim pFileCounter = CInt2(FileText(pFilename))

            If oCounter < pFileCounter Then
                oCounter = pFileCounter
            End If
        End SyncLock

    End Function

    Public Sub UpdateCounter(ByVal vDeviceID As String, ByVal vFowstID As String, ByVal vValue As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        SyncLock mCounterLock
            If vValue <= 0 Then Return
            pIU = New InsertUpdate("LibraryDeviceFowst")
            pIU("POSLastORNo") = vValue
            pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
                " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
                " AND ISNULL(POSLastORNo, 0) < " & vValue)
            pSql = mProData.ExecuteSQL(pSql) ' get the number of updated records (for debugging only)
        End SyncLock

    End Sub

    Public Sub UpdateSoCounter(ByVal vDeviceID As String, ByVal vFowstID As String, ByVal vValue As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vValue <= 0 Then Return
        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("POSLastSONo") = vValue
        pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND ISNULL(POSLastSONo, 0) < " & vValue)
        pSql = mProData.ExecuteSQL(pSql) ' get the number of updated records (for debugging only)

    End Sub

    Public Sub UpdatePoCounter(ByVal vDeviceID As String, ByVal vFowstID As String, ByVal vValue As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vValue <= 0 Then Return
        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("POSLastPONo") = vValue
        pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND ISNULL(POSLastPONo, 0) < " & vValue)
        pSql = mProData.ExecuteSQL(pSql) ' get the number of updated records (for debugging only)

    End Sub

    Public Function GetSummary(ByVal vFowstID As String, ByVal vDeviceID As String, ByVal LastUpdatedDate As Date) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.UnitPrice) AS ItemsSellingAmount" & vbCrLf &
            " ,SUM(TransInvoiceItems.SubTotal) AS SellingAmount" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,CONVERT(date, TransInvoice.LastUpdated) AS LastUpdatedDate" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " WHERE TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransInvoice.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND CONVERT(date, TransInvoice.LastUpdated) = " & ReadyString(LastUpdatedDate.Date) & vbCrLf &
            " AND TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.InvoiceNumber <> 0" & vbCrLf &
            " GROUP BY TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,CONVERT(date, TransInvoice.LastUpdated)" & vbCrLf &
            " ORDER BY CONVERT(date, TransInvoice.LastUpdated)"
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetMaxSubNumber(ByVal vInvoiceNumber As Integer, ByVal vFowstID As String, ByVal vDeviceID As String) As Integer?

        Dim pDR As DataRow
        Dim pSql As String
        ' todo: i-comment ni 2024_01_25__10_36_58_8953
        pSql = "SELECT TOP 1 (SubNumber)" & vbCrLf &
            " FROM TransInvoice" &
            " WHERE InvoiceNumber = " & vInvoiceNumber & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " ORDER BY SubNumber DESC"
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            If pDR(0) IsNot DBNull.Value Then
                Return pDR(0)
            Else
                Return 0
            End If
        End If

    End Function

    Public Function GetCountByMonth(Optional ByVal vDate As Date? = Nothing) As Integer

        Dim p As DateRange
        Dim pSql As String
        Dim pOut As Integer

        If vDate Is Nothing Then vDate = Now
        p = MonthRange(vDate)
        pSql = "SELECT COUNT(*) " &
            " FROM TransInvoice" &
            " WHERE CONVERT(date, InvoiceDate) >= " & ReadyString(p.DateFrom) &
            " AND CONVERT(date, InvoiceDate) <= " & ReadyString(p.DateTo)
        pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function DucToMobizInvoice() As String

        Dim pOut As String = ""
        Dim pSourceBusiness As Business
        Dim pSql As String
        Dim pDtSource As DataTable
        Dim pCreator As Creator
        Dim pError As String

        If mParent.Utilities.HasWholesaleDatabase = False Then
            Return "No wholesale database"
        End If

        WebRequest.Instance.OrgID = ErpToMobizExporter.OrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        pSourceBusiness = New Business(ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="wholesale",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
        ))

        pSql = " -- source wholesale to mobiz invoice" & vbCrLf &
            " SELECT TransInvoice.ID" & vbCrLf &
            ",TransInvoice.InvoiceNumber" & vbCrLf &
            ",TransSalesOrder.MobizInvoiceID" & vbCrLf &
            ",MainInvoiceDefaults.Abbrevation" & vbCrLf &
            ",MobizSalesOrder.FowstID" & vbCrLf &
            ",MobizSalesOrder.DeviceID" & vbCrLf &
            ",TransInvoice.CustomerID" & vbCrLf &
            ",TransInvoice.InvoiceDate" & vbCrLf &
            "--,MobizSalesOrder." & vbCrLf &
            ",MobizSalesOrder.UserID" & vbCrLf &
            ",MobizSalesOrder.DeliveryAddressID" & vbCrLf &
            ",MobizSalesOrder.BillingAddressID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransInvoice.SalesOrderID" & vbCrLf &
            " JOIN MobizDUC.dbo.TransInvoice MobizSalesOrder ON MobizSalesOrder.ID = TransSalesOrder.MobizInvoiceID" & vbCrLf &
            " LEFT JOIN MainInvoiceDefaults ON MainInvoiceDefaults.ID = TransInvoice.InvoiceDefaultID" & vbCrLf &
            " WHERE TransSalesOrder.MobizInvoiceID IS NOT NULL" & vbCrLf &
            " AND NOT EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "   FROM MobizDUC.dbo.TransInvoice MobizTransInvoice" & vbCrLf &
            "   WHERE MobizTransInvoice.DucInvoiceID = TransInvoice.ID" & vbCrLf &
            " )" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated"
        pDtSource = pSourceBusiness.ProData.Execute(pSql)
        If pDtSource.Rows.Count = 0 Then
            pOut = "No transferrable invoice found."
        End If
        For Each pDrSource As DataRow In pDtSource.Rows
            pCreator = CreateCreator()
            pCreator.InvoiceNumber = 0
            pCreator.FowstID = CStr2(pDrSource("FowstID"))
            pCreator.DeviceID = CStr2(pDrSource("DeviceID"))
            pCreator.CustomerID = CStr2(pDrSource("CustomerID"))
            pCreator.InvoiceDate = CStr2(pDrSource("InvoiceDate"))
            pCreator.UserID = CStr2(pDrSource("UserID"))
            pCreator.DeliveryAddressID = CStr2(pDrSource("DeliveryAddressID"))
            pCreator.BillingAddressID = CStr2(pDrSource("BillingAddressID"))
            pCreator.DucInvoiceNum = pDrSource("InvoiceNumber")
            pCreator.DucInvoiceID = CStr2(pDrSource("ID"))
            pCreator.DucInvoiceType = ReadyString(pDrSource("Abbrevation"))

            pCreator.Stocks = DucToMobizStocks(pSourceBusiness, CStr2(pDrSource("ID")))
            If pCreator.Save(True, pError) Then
                pOut &= "Invoice from " & CStr2(pDrSource("ID")) & " was successfully saved."
            Else
                pOut &= "An error occured while saving invoice from " & CStr2(pDrSource("ID")) & vbCrLf & pError
            End If
        Next pDrSource

        Return pOut

    End Function

    Private Function DucToMobizStocks(vBusiness As Business, vDucInvoiceID As String) As DataTable

        Dim pDtStock As DataTable
        Dim pSql As String
        Dim pDtSource As DataTable

        pDtStock = New DataTable

        With pDtStock.Columns
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Integer))
            .Add("Quantity", GetType(Integer))
            .Add("ActualQuantity", GetType(Integer))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
        End With

        pSql = "SELECT " & vbCrLf &
            "StockID" & vbCrLf &
            ",Quantity" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",UnitMeasure" & vbCrLf &
            ",TermDiscount" & vbCrLf &
            ",DiscountIfPaidWithIn" & vbCrLf &
            ",NetDue" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",Subtotal" & vbCrLf &
            ",Cost" & vbCrLf &
            ",PriceLevelID" & vbCrLf &
            ",Dimension" & vbCrLf &
            ",UnitGP" & vbCrLf &
            ",UnitGPM" & vbCrLf &
            ",LineGP" & vbCrLf &
            ",LineGPM" & vbCrLf &
            ",LineCost" & vbCrLf &
            ",LineNumber" & vbCrLf &
            ",TaxPercent" & vbCrLf &
            ",TaxAmount" & vbCrLf &
            ",TaxSales" & vbCrLf &
            ",RequestedUnitPrice" & vbCrLf &
            " FROM TransInvoiceItems" &
            " WHERE InvoiceID = " & ReadyString(vDucInvoiceID)
        pDtSource = vBusiness.ProData.Execute(pSql)
        'For Each pDrSource As DataRow In pDtSource.Rows

        'Next pDrSource

        Return pDtSource

    End Function

    Public Sub TagDoneWithdrawal(vID As String, vSectionID As String, vProductID As String, vIsDone As Boolean)

        Dim pSql As String
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("TransInvoiceItemWarehouseSections")
        pIU("InvoiceID") = ReadyString(vID)
        pIU("SectionID") = ReadyString(vSectionID)
        pIU("ProductID") = ReadyString(vProductID)
        pIU("IsDone") = If(vIsDone, 1, 0)
        pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(vID) &
            " AND SectionID = " & ReadyString(vSectionID) &
            " AND ProductID = " & ReadyString(vProductID))
        If mProData.ExecuteSQL(pSql) = 0 Then
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If vIsDone Then
            mParent.TransBackOrders.CreateBackOrders()
        End If

    End Sub

    Public Function TagReadyForInvoice(vID As String, vSectionIDs As List(Of String)) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String = ""

        For Each pSectionID As String In vSectionIDs
            If pSql <> "" Then pSql &= ","
            pSql &= ReadyString(pSectionID)
        Next pSectionID

        pIU = New InsertUpdate("TransInvoiceItemWarehouseSections")
        pIU("ReadyForInvoicingDate") = "GETDATE()"
        pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(vID) & vbCrLf &
            " AND SectionID IN (" & pSql & ")" & vbCrLf &
            " AND ReadyForInvoicingDate IS NULL")
        pSql = mProData.ExecuteSQL(pSql, True)

        Return pSql

    End Function

    Public Function OrderedAndWithdrawnQty(vInvoiceID As String, vProductID As String,
        Optional vExcludeSectionID As String = Nothing) As OrderedAndWithdrawnQtyClass

        Dim pSql As String
        Dim pOut As New OrderedAndWithdrawnQtyClass()

        pSql = " SELECT SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) AS Quantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID)
        pOut.OrderedQty = mProData.ExecuteScalar(pSql)

        pSql = " SELECT SUM(TransInvoiceItemWarehouseWithdrawal.PreparedQuantity * TransInvoiceItemWarehouseWithdrawal.UnitQty) AS Quantity" & vbCrLf &
            " FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
            " WHERE TransInvoiceItemWarehouseWithdrawal.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND TransInvoiceItemWarehouseWithdrawal.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        If vExcludeSectionID IsNot Nothing Then
            pSql &= " AND TransInvoiceItemWarehouseWithdrawal.SectionID <> " & ReadyString(vExcludeSectionID)
        End If
        pOut.WithdrawnQty = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function ReadyToInvoiceSections(vInvoiceID As String) As DataTable

        Dim pDT As DataTable
        Dim pFinder As Finder

        pFinder = New Finder
        With pFinder
            .Distinct = True
            .ProData = mProData
            With .Fields
                .Add("FowstID", SqlDbType.UniqueIdentifier, "MobizLibraryOrganizationFOWSTSection.FowstID")
                .Add("FowstName", SqlDbType.VarChar, "MobizLibraryOrganizationFOWST.Name")
                .Add("SectionID", SqlDbType.UniqueIdentifier, "TransInvoiceItemWarehouseSections.SectionID")
                .Add("SectionName", SqlDbType.VarChar, "MobizLibraryOrganizationFOWSTSection.Name")
            End With
            .SqlFrom = " FROM MobizLibraryOrganizationFOWSTSection" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFOWSTSection.FowstID" & vbCrLf &
                " JOIN TransInvoiceItemWarehouseSections ON TransInvoiceItemWarehouseSections.SectionID = MobizLibraryOrganizationFOWSTSection.ID" & vbCrLf

            .SqlWhere = " WHERE TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.ReadyForInvoicingDate IS NULL" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.SortlingListID IS NULL" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
            .SqlOrder = " ORDER BY FowstName, SectionName"
        End With
        pDT = pFinder.Requery

        Return pDT

    End Function

    Public Class OrderedAndWithdrawnQtyClass
        Public OrderedQty As Decimal
        Public WithdrawnQty As Decimal
    End Class

    ''' <summary>
    ''' for all databases
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function CreateDevAlertsSkipNumberAll(vStartingDate As Date) As DataTable

        Dim pDtOrgs As DataTable
        Dim pDvOrgs As DataView
        Dim pCurrentDatabase As String
        Dim pOut As DataTable
        Dim pDT As DataTable

        pCurrentDatabase = mProData.GetCurrentDatabase
        Try
            pDtOrgs = mParent.LibraryOrganization.GetAll()
            pDvOrgs = New DataView(pDtOrgs, "Database IS NOT NULL", "", DataViewRowState.CurrentRows)
            For Each pDrvOrg As DataRowView In pDvOrgs
                mProData.ExecuteSQL("USE " & EscapeSqlName(pDrvOrg("Database")))
                pDT = SkippedInvoiceNumbers(vStartingDate)
                CreateDevAlertsSkipNumberForOrg(CStr2(pDrvOrg("ID")), pDT)
                If pOut Is Nothing Then
                    pOut = pDT
                Else
                    pOut.Merge(pDT)
                End If
            Next pDrvOrg
        Finally
            mProData.ExecuteSQL("USE " & EscapeSqlName(pCurrentDatabase))
        End Try

        Return pOut

    End Function

    Public Function CreateDevAlertsSkipNumberForOrg(vOrgID As String, vDT As DataTable)

        Dim pDtAlert As DataTable ' device, fowst
        Dim pDT As DataTable

        pDtAlert = vDT.DefaultView.ToTable(True, "DeviceID", "PhoneModel", "PhoneName", "UUID", "FowstID", "FowstName")
        For Each pDrAlert As DataRow In pDtAlert.Rows
            pDT = New DataView(vDT, "DeviceID = " & ReadyString(pDrAlert("DeviceID")) &
                " AND FowstID = " & ReadyString(pDrAlert("FowstID")),
                "", DataViewRowState.CurrentRows).ToTable()
            CreateDevAlertSkipNumber(vOrgID:=vOrgID,
                vDT:=pDT,
                vDeviceID:=CStr2(pDrAlert("DeviceID")),
                vDeviceModelAndName:=pDrAlert("PhoneModel") & " " & pDrAlert("PhoneName"),
                vDeviceUuid:=CStr2(pDrAlert("UUID")),
                vFowstID:=CStr2(pDrAlert("FowstID")),
                vFowstName:=CStr2(pDrAlert("FowstName")))
        Next pDrAlert

    End Function

    Private Function CreateDevAlertSkipNumber(vOrgID As String, vDT As DataTable,
        vDeviceID As String,
        vDeviceModelAndName As String,
        vDeviceUuid As String,
        vFowstID As String,
        vFowstName As String)

        Dim pMessage As String = ""
        Dim pEmailTo As String

        Dim pTableBuilder As TagBuilder
        Dim pRowBuilder As TagBuilder
        Dim pHeaderBuilder As TagBuilder
        Dim pCellBuilder As TagBuilder

        pTableBuilder = New TagBuilder("table")
        pTableBuilder.MergeAttribute("style", "border-collapse:collapse")
        pRowBuilder = New TagBuilder("tr")
        pHeaderBuilder = New TagBuilder("th")
        pHeaderBuilder.MergeAttribute("style", "border:1px solid black; padding: 3px")
        pCellBuilder = New TagBuilder("td")
        pCellBuilder.MergeAttribute("style", "border:1px solid black; padding: 3px")

        pRowBuilder.InnerHtml = ""
        pHeaderBuilder.SetInnerText("Skipped Invoice(s) Numbers")
        pRowBuilder.InnerHtml &= pHeaderBuilder.ToString()
        pHeaderBuilder.SetInnerText("Between")
        pHeaderBuilder.MergeAttribute("colspan", 2)
        pRowBuilder.InnerHtml &= pHeaderBuilder.ToString()
        pHeaderBuilder.Attributes.Remove("colspan")
        pHeaderBuilder.SetInnerText("Duration of Gap")
        pRowBuilder.InnerHtml &= pHeaderBuilder.ToString()
        pTableBuilder.InnerHtml = pRowBuilder.ToString()

        For Each pDR As DataRow In vDT.Rows
            If pMessage <> "" Then
                pMessage &= vbCrLf & vbCrLf
            End If
            If CDec2(pDR("LastNumber")) = 0 Then
                pMessage &= "The first number did not start at 1." & vbCrLf
            Else
                pMessage &= "Last number before the skip: " & pDR("LastNumber") & " (" & pDR("LastNumberLastUpdated") & ")" & vbCrLf
            End If

            pMessage &= "First number after the skip: " & pDR("StartingNumber") & " (" & pDR("StartingNumberLastUpdated") & ")"

            ' build the email body
            pRowBuilder.InnerHtml = ""
            If CDec2(pDR("LastNumber")) + 1 = pDR("StartingNumber") - 1 Then
                pCellBuilder.SetInnerText(CDec2(pDR("LastNumber")) + 1)
            Else
                pCellBuilder.SetInnerText((CDec2(pDR("LastNumber")) + 1) & " to " & (pDR("StartingNumber") - 1))
            End If
            pRowBuilder.InnerHtml &= pCellBuilder.ToString()
            If CDec2(pDR("LastNumber")) = 0 Then
                pCellBuilder.SetInnerText("n/a")
            Else
                pCellBuilder.SetInnerText(pDR("LastNumberLastUpdated"))
            End If
            pRowBuilder.InnerHtml &= pCellBuilder.ToString()
            pCellBuilder.SetInnerText(pDR("StartingNumberLastUpdated"))
            pRowBuilder.InnerHtml &= pCellBuilder.ToString()
            If CDec2(pDR("LastNumber")) = 0 Then
                pCellBuilder.SetInnerText("n/a")
            Else
                pCellBuilder.SetInnerText(CDate(pDR("StartingNumberLastUpdated")).Subtract(CDate(pDR("LastNumberLastUpdated"))).TotalMinutes.ToString(cGlobal.FormatInteger) & " minutes")
            End If
            pRowBuilder.InnerHtml &= pCellBuilder.ToString()
            pTableBuilder.InnerHtml &= pRowBuilder.ToString()
        Next pDR

        mParent.TransDevAlerts.AddEntry(vMessage:=pMessage,
            vSource:="Invoice Skip Number",
            vOrgID:=vOrgID,
            vLoggedInUserID:=Nothing,
            vDeviceID:=vDeviceID,
            vFowstID:=vFowstID)

        ' todo: 2016_01_29__09_57_28_3393
        'pSql = " SELECT DISTINCT MobizLibraryOrganizationFranchisee.FranchisorOrgID" & vbCrLf &
        '    " ,LibraryFranchiseGroupHeads.CompanyEmailAddress" & vbCrLf &
        '    " ,MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID" & vbCrLf &
        '    " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
        '    " LEFT JOIN LibraryFranchiseGroup ON LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
        '    " LEFT JOIN LibraryFranchiseGroupHeads ON LibraryFranchiseGroupHeads.FranchiseGroupID = LibraryFranchiseGroup.ID" & vbCrLf &
        '    " WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = newid()" & vbCrLf &
        '    " AND LibraryFranchiseGroupHeads.CompanyEmailAddress IS NOT NULL"
#If DEBUG Then
        pEmailTo = "arvin.meralpis@mobiz-apps.com, arvinboggs@gmail.com"
#Else
        pEmailTo = "customercare@mobiz-apps.com, arvin.meralpis@mobiz-apps.com, robert.chong@mobiz-apps.com"
        'pEmailTo = "customercare@mobiz-apps.com, arvin.meralpis@mobiz-apps.com, arvinboggs@gmail.com"
#End If

        Dim pBodyBuilder As TagBuilder
        Dim pParaBuilder As TagBuilder

        pBodyBuilder = New TagBuilder("body")
        pParaBuilder = New TagBuilder("p")

        pParaBuilder.SetInnerText("Oops, your database is missing some invoice numbers.")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pParaBuilder.SetInnerText("Mobiz makes a unique invoice series for each FOWST+Device combination.")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pParaBuilder.SetInnerText("Missing invoices are indicative of a corrupted or deleted device database.")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pParaBuilder.SetInnerText("Please check with the cashier on duty at the time when invoice numbers have been lost. Overage of monies received by the cashier are highly likely to result due to the missing invoices.")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pParaBuilder.SetInnerText("Below are the missing data in the invoice series of " & vFowstName & " on " & vDeviceModelAndName & " with serial number " & vDeviceUuid & ":")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pBodyBuilder.InnerHtml &= pTableBuilder.ToString()

        pParaBuilder.SetInnerText("We are doing our best to help retrieve the missing data through database backup and/or some other method.")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pParaBuilder.SetInnerText("All the best,")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString
        pParaBuilder.SetInnerText("The Mobiz Team")
        pBodyBuilder.InnerHtml &= pParaBuilder.ToString

        pMessage = pBodyBuilder.ToString()
        Try
            SendMail("customercare@mobiz-apps.com",
                pEmailTo,
                "Mobiz",
                "Notice of Skipped Invoice Numbers",
                pMessage,
                "smtp.live.com",
                587,
                "customercare@mobiz-apps.com",
                "MobizCustCare2221", , , , , True)
        Catch ex As Exception
            Stop2()
            AppendToLog(ex.Message, , "CreateDevAlertSkipNumber")
        End Try

    End Function

    ''' <summary>
    ''' for current private database only
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SkippedInvoiceNumbers(vStartingDate As Date) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT" & vbCrLf &
            " TransInvoice_1.InvoiceNumber AS LastNumber" & vbCrLf &
            " ,TransInvoice_1.LastUpdated AS LastNumberLastUpdated" & vbCrLf &
            " ,TransInvoice.InvoiceNumber AS StartingNumber" & vbCrLf &
            " ,TransInvoice.LastUpdated AS StartingNumberLastUpdated" & vbCrLf &
            " ,TransInvoice.DeviceID" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.UserID" & vbCrLf &
            " ,LibraryDevice.UUID" & vbCrLf &
            " ,LibraryDevice.PhoneModel" & vbCrLf &
            " ,LibraryDevice.PhoneName" & vbCrLf &
            " ,LibraryDevice.OS" & vbCrLf &
            " ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.MiddleName" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT TOP 1 InvoiceNumber" & vbCrLf &
            "     ,LastUpdated" & vbCrLf &
            "     FROM TransInvoice TransInvoice_1" & vbCrLf &
            "     WHERE TransInvoice_1.InvoiceNumber < TransInvoice.InvoiceNumber" & vbCrLf &
            "     AND TransInvoice.DeviceID = TransInvoice_1.DeviceID" & vbCrLf &
            "     AND TransInvoice.FowstID = TransInvoice_1.FowstID" & vbCrLf &
            "     ORDER BY InvoiceNumber DESC" & vbCrLf &
            "     ,LastUpdated DESC" & vbCrLf &
            " ) TransInvoice_1" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 1" & vbCrLf &
            " AND CONVERT(date, TransInvoice.LastUpdated) >= " & ReadyString(vStartingDate.Date) & vbCrLf &
            " AND NOT EXISTS (" & vbCrLf &
            " 	SELECT 1" & vbCrLf &
            "     FROM TransInvoice TransInvoice_2" & vbCrLf &
            "     WHERE TransInvoice_2.InvoiceNumber = TransInvoice.InvoiceNumber - 1" & vbCrLf &
            "     AND TransInvoice.DeviceID = TransInvoice_2.DeviceID" & vbCrLf &
            "     AND TransInvoice.FowstID = TransInvoice_2.FowstID" & vbCrLf &
            " )" & vbCrLf &
            " ORDER BY LastNumber, StartingNumber, LastNumberLastUpdated, StartingNumberLastUpdated"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function AutoRunCreateDevAlertsSkipNumberAll() As DataTable

        Dim pFileText As String
        Dim pDate As Date
        Dim pEnable As Boolean
        Dim pOut As DataTable

        pFileText = GetDevAlertsSkipNumberSettingFileText()
        If IsDate(pFileText) Then
            pDate = pFileText
            If pDate.Date < Now.Date Then
                pEnable = True
            End If
        Else
            pEnable = True
            pDate = SkipNumberReportMinDate
        End If

        If pEnable Then
            pOut = CreateDevAlertsSkipNumberAll(pDate)
            If pOut IsNot Nothing _
                AndAlso pOut.Rows.Count > 0 Then

                pDate = Now
                FileText(GetDevAlertsSkipNumberSettingFilename()) = pDate
            End If
        End If

        Return pOut

    End Function

    Dim mDevAlertsSkipNumberSettingFilename As String

    Private Function GetDevAlertsSkipNumberSettingFilename() As String

        If mDevAlertsSkipNumberSettingFilename IsNot Nothing Then
            Return mDevAlertsSkipNumberSettingFilename
        End If
        mDevAlertsSkipNumberSettingFilename = cGlobal.AppPath & "settings\AutoRunCreateDevAlertsSkipNumberAll.txt"

        Return mDevAlertsSkipNumberSettingFilename

    End Function

    Public Overridable Function GetDevAlertsSkipNumberSettingFileText() As String

        Return FileText(GetDevAlertsSkipNumberSettingFilename())

    End Function

    Public Function IsApproved(vID As String) As Boolean

        Dim pSql = "SELECT SopaApprovedByID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pValue = CStr2(mProData.ExecuteScalar(pSql))

        Return Not String.IsNullOrWhiteSpace(pValue)

    End Function

    Public Function Approve(vID As String, vPeopleID As String, ByRef oError As String, vOrgID As String) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer

        oError = CantApproveReason(vID, vOrgID)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return 0
        End If

        pIU = New InsertUpdate("TransInvoice")
        pIU("SopaApprovedByID") = ReadyString(vPeopleID)
        pIU("UserID") = ReadyString(vPeopleID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pOut = mProData.ExecuteSQL(pSql, True)

        Return pOut

    End Function

    ''' <summary>
    ''' ProData.BeginTransaction must be called before calling this function
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vPeopleID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FinalizePickList(vID As String, vPeopleID As String, vOrgID As String) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pFinalizedForAutoInvoice As Boolean

        pIU = New InsertUpdate("TransInvoice")
        pIU("FinalizePickListByID") = ReadyString(vPeopleID)
        pIU("FinalizePickListTime") = "GETDATE()"
        pIU("UserID") = ReadyString(vPeopleID)
        Dim pAutoCreateCondition = mParent.SystemDefaults.AutoCreateInvoiceConditionProper
        If pAutoCreateCondition = SystemDefaults.eAutoCreateInvoiceCondition.VendorPortal Then
            pIU("FinalizedForAutoInvoice") = 1
        ElseIf pAutoCreateCondition = SystemDefaults.eAutoCreateInvoiceCondition.VendorPortalSelected _
            Or pAutoCreateCondition = SystemDefaults.eAutoCreateInvoiceCondition.StockReceivingSelected Then

            pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM LibraryOrganizationSupplier" & vbCrLf &
                " WHERE LibraryOrganizationSupplier.AutoCreateInvoice = 1" & vbCrLf &
                " AND SupplierID IN (" & vbCrLf &
                "   SELECT TransInvoice.CustomerID" & vbCrLf &
                "   FROM TransInvoice" & vbCrLf &
                "   WHERE TransInvoice.ID = " & ReadyString(vID) & vbCrLf &
                ")"
            Dim pCount = mProData.ExecuteScalar(pSql)
            If pAutoCreateCondition = SystemDefaults.eAutoCreateInvoiceCondition.StockReceivingSelected _
                AndAlso pCount = 0 Then

                pFinalizedForAutoInvoice = True
                pIU("FinalizedForAutoInvoice") = 1
            ElseIf pAutoCreateCondition = SystemDefaults.eAutoCreateInvoiceCondition.VendorPortalSelected _
                AndAlso pCount > 0 Then

                pFinalizedForAutoInvoice = True
                pIU("FinalizedForAutoInvoice") = 1
            End If
        ElseIf AreAllQtyServed(vID) Then
            SalesOrderToInvoice(vID, "", "", "", vOrgID)
        End If
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pOut = mProData.ExecuteSQL(pSql, True)

        UpdateProductTax(vID)

        If pFinalizedForAutoInvoice Then
            AutoFinalizePurchaseOrdersFromVendor(vOrgID)
        End If

        Return pOut

    End Function

    Function AreAllQtyServed(vInvoiceID As String) As Boolean

        Dim pSql = " SELECT COUNT(*)" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE (" & vbCrLf &
            "   (" & vbCrLf &
            "       ISNULL(TransInvoiceItems.Quantity, 0) < ISNULL(TransInvoiceItems.OrderedQuantity, 0)" & vbCrLf &
            "       AND ISNULL(TransInvoiceItems.OrderedQuantity, 0) > 0" & vbCrLf &
            "   )" & vbCrLf &
            " OR ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) > 0" & vbCrLf &
            ")" & vbCrLf &
            " AND InvoiceID = " & ReadyString(vInvoiceID)
        Dim pResult = CInt2(mProData.ExecuteScalar(pSql))

        Return pResult = 0

    End Function

    Public Sub CopyOrderQtyToServedQtyIfServedQtyIsNotEdited(vID As String)

        Dim pSql As String

        pSql = "UPDATE TransInvoiceItems" & vbCrLf &
            " SET Quantity = OrderedQuantity" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID) & vbCrLf &
            " AND ISNULL(ServedQtyIsEdited, 0) = 0"
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

    End Sub

    Function CantApproveReason(vID As String, vOrgID As String) As String

        Dim pDR As DataRow
        Dim pSql As String

        If IsApproved(vID) Then
            Return "This invoice is already approved."
        End If

        vOrgID = ReadyGuid(vOrgID)
        If vOrgID <> ReadyGuid(modClient.DucOrgID) Then
            Return Nothing
        End If
        pSql = "SELECT ApprovedDate" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing _
            Or pDR("ApprovedDate") Is DBNull.Value Then

            Return "This invoice is not yet approved by C&C."
        End If

    End Function

    Public Sub SaveItemFowst(vID As String,
        vProductID As String,
        vFowstID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        mProData.BeginTransaction()
        Try
            DeleteItemFowsts(vID, vProductID)
            If vFowstID = "any" Then
                mProData.CommitTransaction()
                Exit Sub
            End If
            pIU = New InsertUpdate("TransInvoiceItemsPickingFowsts")
            pIU("FowstID") = ReadyString(vFowstID)
            pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(vID) &
                " AND ProductID = " & ReadyString(vProductID))
            If mProData.ExecuteSQL(pSql, True) = 0 Then
                pIU("ID") = "newid()"
                pIU("InvoiceID") = ReadyString(vID)
                pIU("ProductID") = ReadyString(vProductID)
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
            mProData.CommitTransaction()
        Catch ex As Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try

    End Sub

    Public Function GetItemFowsts(vID As String, vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT FowstID" & vbCrLf &
            " FROM TransInvoiceItemsPickingFowsts" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID) &
            " AND ProductID = " & ReadyString(vProductID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function DeleteItemFowsts(vID As String, vProductID As String) As Integer

        Dim pSql As String

        If IsGuid(vID) = False Then
            Throw New ArgumentException("vID must be guid. You passed " & vID & ". vbGRDWG757SG!")
        End If

        If IsGuid(vID) = False Then
            Throw New ArgumentException("vProductID must be guid. You passed " & vID & ". vb9NSG65XHSG!")
        End If

        pSql = "DELETE " & vbCrLf &
            " FROM TransInvoiceItemsPickingFowsts" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID) &
            " AND ProductID = " & ReadyString(vProductID)
        Return mProData.ExecuteSQL(pSql)

    End Function

    Public Function GetMaxSoNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(SalesOrderNo)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ISNULL(EncodingFowstID, FowstID) = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Function GetSoCounter(vDeviceID As String, vFowstID As String) As Integer


        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(POSLastSONo)" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE ISNULL(EncodingFowstID, FowstID) = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Public Function GetNextSoNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pOut As Integer

        pOut = Math.Max(GetMaxSoNumber(vDeviceID, vFowstID),
            GetSoCounter(vDeviceID, vFowstID))
        pOut += 1

        Return pOut

    End Function

    Function GetMaxPoNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(PurchaseOrderNo)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ISNULL(EncodingFowstID, FowstID) = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Public Function GetMaxAccountsPayableNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(AccountsPayableNum)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ISNULL(EncodingFowstID, FowstID) = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Function GetPoCounter(vDeviceID As String, vFowstID As String) As Integer


        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(POSLastPONo)" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Public Function GetNextPoNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pOut As Integer

        pOut = Math.Max(GetMaxPoNumber(vDeviceID, vFowstID),
            GetPoCounter(vDeviceID, vFowstID))
        pOut += 1

        Return pOut

    End Function

    Function GetStrCounter(vDeviceID As String, vFowstID As String) As Integer


        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(LastStockTransferRequisitionNum)" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Function GetMaxStrNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MAX(StrNum)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ISNULL(EncodingFowstID, FowstID) = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CDec2(pDR(0))
        End If

    End Function

    Public Function GetNextStrNumber(vDeviceID As String, vFowstID As String) As Integer

        Dim pOut As Integer

        pOut = Math.Max(GetMaxStrNumber(vDeviceID, vFowstID),
            GetStrCounter(vDeviceID, vFowstID))
        pOut += 1

        Return pOut

    End Function

    Public Sub AddReprintRequest(vInvoiceID As String,
        vRequestedByID As String,
        vRemarks As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoicePrints")
        pIU("ID") = "NEWID()"
        pIU("InvoiceID") = ReadyString(vInvoiceID)
        pIU("DateRequested") = "GETDATE()"
        pIU("RequestedByID") = ReadyString(vRequestedByID)
        pIU("Remarks") = ReadyString(vRemarks)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub ApproveReprintRequest(vInvoicePrintID As String,
        vApprove As Boolean,
        vApprovedByID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoicePrints")
        If vApprove Then
            pIU("DateApproved") = "GETDATE()"
            pIU("ApprovedByID") = ReadyString(vApprovedByID)
        Else
            pIU("DateDenied") = "GETDATE()"
            pIU("DeniedByID") = ReadyString(vApprovedByID)
        End If
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoicePrintID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function UnapprovedReprintRequests(vInvoiceID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT * " & vbCrLf &
            " FROM TransInvoicePrints" & vbCrLf &
            " WHERE TransInvoicePrints.ApprovedByID IS NULL" & vbCrLf &
            " AND TransInvoicePrints.DeniedByID IS NULL" & vbCrLf &
            " AND InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " ORDER BY TransInvoicePrints.DateRequested"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ApprovedUnprintedRequests(vInvoiceID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT *" & vbCrLf &
            " FROM TransInvoicePrints" & vbCrLf &
            " WHERE TransInvoicePrints.ApprovedByID IS NOT NULL" & vbCrLf &
            " AND TransInvoicePrints.DeniedByID IS NULL" & vbCrLf &
            " AND TransInvoicePrints.PrintedByID IS NULL" & vbCrLf &
            " AND InvoiceID = " & ReadyString(vInvoiceID)
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Sub TagRequestAsPrinted(vInvoicePrintID As String, vPrintedByID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoicePrints")
        pIU("DatePrinted") = "GETDATE()"
        pIU("PrintedByID") = ReadyString(vPrintedByID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoicePrintID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function GetProductIdByPrice(vOrgID As String, vPrice As Decimal, vPriceLevelID As String) As String

        Dim pSql As String
        Dim pOut As String

        pSql = " -- find product with same price" & vbCrLf &
            " DECLARE @PriceLevelID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @OrgID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @Price MONEY" & vbCrLf &
            " SET @PriceLevelID = " & ReadyString(vPriceLevelID) & vbCrLf &
            " SET @OrgID = " & ReadyString(vOrgID) & vbCrLf &
            " SET @Price = " & vPrice & vbCrLf &
            " SELECT TOP 1 TransInvoiceItems.ProductID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoiceItems.ProductID IN (" & vbCrLf &
            "   SELECT LibraryOrganizationPIM.ProductID" & vbCrLf &
            "   FROM LibraryOrganizationPIM" & vbCrLf &
            "   JOIN MobizLibraryPIMProductPriceLevel ON MobizLibraryPIMProductPriceLevel.ProductID = LibraryOrganizationPIM.ProductID" & vbCrLf &
            "   AND MobizLibraryPIMProductPriceLevel.OrganizationID = @OrgID" & vbCrLf &
            "   AND MobizLibraryPIMProductPriceLevel.PriceLevelID = @PriceLevelID" & vbCrLf &
            "   AND MobizLibraryPIMProductPriceLevel.Price = @Price" & vbCrLf &
            " )" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        pOut = CStr2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function GetItems(vID As String, vWithQtyOnly As Boolean, Optional vProductID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity AS Quantity" & vbCrLf &
            " ,TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity AS OrderedQuantity" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            " ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            " ,TransInvoiceItems.ID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
        If vWithQtyOnly Then
            pSql &= " AND (TransInvoiceItems.Quantity > 0 OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                " AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf
        End If
        If vProductID IsNot Nothing Then
            pSql &= " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        End If
        pSql &= " ORDER BY MobizLibraryPIMProduct.ProductName" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetItemsByInvoiceID(vInvoiceID As String, vWithQtyOnly As Boolean, Optional vProductID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity AS Quantity" & vbCrLf &
            " ,TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity AS OrderedQuantity" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            " ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID IN " & vbCrLf &
            "(SELECT ID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE TransInvoice.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            ")" & vbCrLf
        If vWithQtyOnly Then
            pSql &= " AND TransInvoiceItems.Quantity > 0" & vbCrLf &
                " AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf
        End If
        If vProductID IsNot Nothing Then
            pSql &= " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        End If
        pSql &= " ORDER BY MobizLibraryPIMProduct.ProductName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetItemByUnitAndInvoiceID(vInvoiceID As String, vProductID As String, vUnitMeasure As String) As DataTable

        Dim pSql = "SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.Quantity" &
            " FROM TransInvoiceItems" &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND TransInvoiceItems.UnitMeasure = " & ReadyString(vUnitMeasure) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetLackingItems(vInvoiceID As String) As DataTable

        Dim pDtInvoiceProducts = GetItems(vInvoiceID, False)
        Dim pDtOrderProducts = GetItemsByInvoiceID(vInvoiceID, False)
        pDtOrderProducts = New DataView(pDtOrderProducts, "", "ProductName, ActualQuantity DESC", DataViewRowState.CurrentRows).ToTable()

        Dim pDtOrderProductsUnique = pDtOrderProducts.DefaultView.ToTable(True, "ProductID")
        'Dim pDtInvoiceProductsUnique = pDtInvoiceProducts.DefaultView.ToTable(True, "ProductID")

        pDtOrderProductsUnique.Columns.Add("InvoicedQty", GetType(Decimal))
        pDtOrderProductsUnique.Columns.Add("OrderedQty", GetType(Decimal))
        pDtOrderProductsUnique.Columns.Add("ConsumedQty", GetType(Decimal))
        For Each pDrOrderProductUnique As DataRow In pDtOrderProductsUnique.Rows
            Dim pDV = New DataView(pDtOrderProducts, "ProductID = " & ReadyString(pDrOrderProductUnique("ProductID")), "", DataViewRowState.CurrentRows)
            For Each pDRV In pDV
                pDrOrderProductUnique("OrderedQty") = CDec2(pDrOrderProductUnique("OrderedQty")) + CDec2(pDRV("OrderedQuantity"))
            Next

            pDV = New DataView(pDtInvoiceProducts, "ProductID = " & ReadyString(pDrOrderProductUnique("ProductID")), "", DataViewRowState.CurrentRows)
            For Each pDRV In pDV
                pDrOrderProductUnique("InvoicedQty") = CDec2(pDrOrderProductUnique("InvoicedQty")) + CDec2(pDRV("Quantity"))
            Next
        Next pDrOrderProductUnique

        For Each pDrOrderProduct As DataRow In pDtOrderProducts.Rows
            If pDtInvoiceProducts.Select("ProductID = " & ReadyString(pDrOrderProduct("ProductID")) & vbCrLf &
                " AND ActualQuantity = " & ReadyString(pDrOrderProduct("ActualQuantity"))).Length = 0 Then

                pDtInvoiceProducts.ImportRow(pDrOrderProduct)
            End If
        Next pDrOrderProduct

        Dim pDtOut = pDtInvoiceProducts.Clone()

        For Each pDrOrderProductUnique As DataRow In pDtOrderProductsUnique.Rows
            Dim pLackingQty = CDec2(pDrOrderProductUnique("OrderedQty")) - CDec2(pDrOrderProductUnique("InvoicedQty"))
            If pLackingQty <= 0 Then
                Continue For
            End If
            Dim pDV = New DataView(pDtInvoiceProducts, "ProductID = " & ReadyString(pDrOrderProductUnique("ProductID")), "ActualQuantity DESC", DataViewRowState.CurrentRows)
            For Each pDRV As DataRowView In pDV
                If pLackingQty < CDec2(pDRV("ActualQuantity")) Then
                    Continue For
                End If
                Dim pQtyToConsume = pLackingQty \ CDec2(pDRV("ActualQuantity"))
                pDRV("OrderedQuantity") = pQtyToConsume
                pDtOut.ImportRow(pDRV.Row)
                pLackingQty -= pQtyToConsume * CDec2(pDRV("ActualQuantity"))
            Next
        Next pDrOrderProductUnique

        Return pDtOut

        pDtOrderProducts.Columns.Add("ConsumedQty", GetType(Decimal))
        For Each pDrOrderProduct As DataRow In pDtOrderProducts.Rows
            Dim pDrOrderProductUnique = pDtOrderProductsUnique.Select("ProductID = " & ReadyString(pDrOrderProduct("ProductID")))(0)
            Dim pConsumable = CDec2(pDrOrderProductUnique("InvoicedQty")) - CDec2(pDrOrderProductUnique("ConsumedQty"))
            Dim pQtyToConsume = pConsumable - (CDec2(pDrOrderProduct("Quantity")) * CDec2(pDrOrderProduct("ActualQuantity")))


            Dim pDV = New DataView(pDtInvoiceProducts, "ProductID = " & ReadyString(pDrOrderProduct("ProductID")) & vbCrLf &
                " AND ISNULL(Consumed, 0) < Quantity", "ProductID, ActualQuantity DESC", DataViewRowState.CurrentRows)
            For Each pDRV As DataRowView In pDV
                pQtyToConsume = CDec2(pDrOrderProduct("Quantity"))
                If CDec2(pDRV("Quantity")) - CDec2(pDRV("Consumed")) < pQtyToConsume Then
                    pQtyToConsume = CDec2(pDRV("Quantity")) - CDec2(pDRV("Consumed"))
                End If
                pDrOrderProduct("Quantity") -= pQtyToConsume
                pDRV("Consumed") = CDec2(pDRV("Consumed")) + pQtyToConsume
            Next pDRV
        Next pDrOrderProduct

        Return pDtOrderProducts

    End Function

    ''' <summary>
    ''' not used for now
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InvoiceInfo
        Public Amount As Decimal
        Public InvoiceDate As Date
        Dim mInvoiceNumComposite As String
        Dim mInvoiceNum As String
        Dim mInvoiceType As String
        Dim mInvoiceNumExtension As String

        Public Property InvoiceNumComposite As String
            Get

                Return mInvoiceNumComposite

            End Get
            Set(value As String)


            End Set
        End Property
    End Class

    ''' <summary>
    ''' this function does not contain pure data-related functions
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyInvoiceNumbers() As String

        ' inputs
        Const pOrgID As String = ""
        Const pOrgName As String = ""
        Const pFowstID As String = ""
        Const pFowstName As String = ""
        Const pDeviceID As String = ""
        Const pDeviceSerial As String = ""
        Const pStartingSourceInvoiceNumber As Integer = 0
        Const pStartingDestinationInvoiceNumber As Integer = 0

        Dim pError As String
        Dim pDrOrg As DataRow
        Dim pDrFowst As DataRow
        Dim pDrDevice As DataRow
        Dim pDiff As Integer

        If pStartingSourceInvoiceNumber <= pStartingDestinationInvoiceNumber Then

        End If

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return "HandleOrganization vb452GURV2MF! " & pError
        End If
        pDrOrg = mParent.LibraryOrganization.ByID(pOrgID)
        If pDrOrg Is Nothing Then
            Return "no org with that id. vbP8D2YN6SQH!"
        End If
        If pDrOrg("CompanyName") <> pOrgName Then
            Return "actual CompanyName is " & pDrOrg("CompanyName") & ", not " & pOrgName
        End If

        pDrFowst = mParent.LibraryOrganizationFOWST.ByID(pFowstID)
        If pDrFowst Is Nothing Then
            Return "no fowst with that id vbFHY9F6MZQV!"
        End If
        If pDrFowst("Name") <> pFowstName Then
            Return "actual fowstname is " & pDrFowst("Name") & ", not " & pFowstName
        End If

        pDrDevice = mParent.LibraryDevice.ById(pDeviceID)
        If pDrDevice Is Nothing Then
            Return "no device with that id vbZEMGJJW7SQ!"
        End If
        If pDrDevice("UUID") <> pDeviceSerial Then
            Return "actual UUID is " & pDrDevice("UUID") & ", not " & pDeviceSerial
        End If

        Dim pDrInvoice As DataRow
        Dim pSql As String
        Dim pDtHindrance As DataTable

        pSql = "SELECT InvoiceDate" & vbCrLf &
            ",LastUpdated" & vbCrLf &
            ",TotalAmount" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FowstID = " & ReadyString(pFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(pDeviceID) & vbCrLf &
            " AND InvoiceNumber = " & pStartingDestinationInvoiceNumber
        pDrInvoice = mProData.ExecuteRow(pSql)
        If pDrInvoice IsNot Nothing Then
            Return "already has pStartingDestinationInvoiceNumber, " & pStartingDestinationInvoiceNumber & ". vb9MSHV5BHUS!"
        End If

        pSql = "SELECT ID" & vbCrLf &
            ",InvoiceNumber" & vbCrLf &
            ",InvoiceDate" & vbCrLf &
            ",LastUpdated" & vbCrLf &
            ",TotalAmount" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FowstID = " & ReadyString(pFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(pDeviceID) & vbCrLf &
            " AND InvoiceNumber > " & pStartingDestinationInvoiceNumber & vbCrLf &
            " AND InvoiceNumber < " & pStartingSourceInvoiceNumber
        pDtHindrance = mProData.Execute(pSql)
        If pDtHindrance.Rows.Count > 0 Then
            Return "number of hindrance: " & pDtHindrance.Rows.Count & ". vbYGKBLNP778!"
        End If

    End Function

    ''' <summary>
    ''' copy library data from 1st instance (vSourceProData) to 2nd instance (mProData)
    ''' </summary>
    ''' <param name="vInvoiceID"></param>
    ''' <param name="vSourceProData">ProData of 1st instance</param>
    ''' <remarks></remarks>
    Public Sub AddDependencyData(vInvoiceID As String, vSourceProData As ProData)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDrInvoice = ById(vInvoiceID)
        If pDrInvoice Is Nothing Then
            Throw New ApplicationException("vbSB3DYBZ2CN! AddDependencyData. Invoice does not exist. InvoiceID: " & vInvoiceID)
        End If

        ' MobizLibraryOrganizationFOWST
        If String.IsNullOrWhiteSpace(CStr2(pDrInvoice("FowstID"))) = False Then
            pSql = "SELECT * FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(pDrInvoice("FowstID"))
            Dim pDrFowst = mProData.ExecuteRow(pSql)
            If pDrFowst Is Nothing Then
                ' no library data in this 2nd instance
                pDrFowst = vSourceProData.ExecuteRow(pSql)
                If pDrFowst Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbP9VDWJW9ZU! Fowst does not exist in 1st instance. FowstID: " & CStr2(pDrInvoice("FowstID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDrFowst)
                    pIU.TableName = "MobizLibraryOrganizationFOWST"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If

            ' MobizLibraryFOWSTRetailStoreType
            pSql = "SELECT MobizLibraryFOWSTRetailStoreType.*" & vbCrLf &
                " FROM MobizLibraryFOWSTRetailStoreType" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.SubFOWSTTypeID = MobizLibraryFOWSTRetailStoreType.ID" & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(pDrInvoice("FowstID"))
            Dim pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbY9YFFBCGR6! MobizLibraryFOWSTRetailStoreType does not exist in 1st instance. FowstID: " & CStr2(pDrInvoice("FowstID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "MobizLibraryFOWSTRetailStoreType"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If

            ' MobizLibraryFOWSTStoreLocationType
            pSql = "SELECT MobizLibraryFOWSTStoreLocationType.*" & vbCrLf &
                " FROM MobizLibraryFOWSTStoreLocationType" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.StoreLocationTypeID = MobizLibraryFOWSTStoreLocationType.ID" & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(pDrInvoice("FowstID"))
            pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vb6HZA95BP2M! MobizLibraryFOWSTStoreLocationType does not exist in 1st instance. FowstID: " & CStr2(pDrInvoice("FowstID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "MobizLibraryFOWSTStoreLocationType"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If

            ' MobizLibraryOrganizationFranchisee
            pSql = "SELECT MobizLibraryOrganizationFranchisee.*" & vbCrLf &
                " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(pDrInvoice("FowstID"))
            pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbH4UD9SVAM6! MobizLibraryOrganizationFranchisee does not exist in 1st instance. FowstID: " & CStr2(pDrInvoice("FowstID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "MobizLibraryOrganizationFranchisee"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If

            ' LibraryFranchiseGroup
            pSql = "SELECT LibraryFranchiseGroup.*" & vbCrLf &
                " FROM LibraryFranchiseGroup" & vbCrLf &
                " JOIN MobizLibraryOrganizationFranchisee ON MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(pDrInvoice("FowstID"))
            pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbBLUFE549BW! LibraryFranchiseGroup does not exist in 1st instance. FowstID: " & CStr2(pDrInvoice("FowstID")))
                Else
                    mProData.SetInsertIdentity("LibraryFranchiseGroup", True, True)
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "LibraryFranchiseGroup"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If
        End If

        ' MobizLibraryOrganization
        If String.IsNullOrWhiteSpace(CStr2(pDrInvoice("CustomerID"))) = False Then
            pSql = "SELECT * FROM MobizLibraryOrganization WHERE ID = " & ReadyString(pDrInvoice("CustomerID"))
            Dim pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbUKJ57DWPTH! Org does not exist in 1st instance. OrgID: " & CStr2(pDrInvoice("CustomerID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "MobizLibraryOrganization"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If
        End If

        If String.IsNullOrWhiteSpace(CStr2(pDrInvoice("DeviceID"))) = False Then
            pSql = "SELECT * FROM LibraryDevice WHERE ID = " & ReadyString(pDrInvoice("DeviceID"))
            Dim pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbJSFBAH6ZHA! Device does not exist in 1st instance. DeviceID: " & CStr2(pDrInvoice("DeviceID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "LibraryDevice"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If
        End If

        If String.IsNullOrWhiteSpace(CStr2(pDrInvoice("UserID"))) = False Then
            pSql = "SELECT * FROM MobizLibraryPeople WHERE ID = " & ReadyString(pDrInvoice("UserID"))
            Dim pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                ' no library data in this 2nd instance
                pDR = vSourceProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    ' no library data in 1st instance
                    Console.WriteLine("vbHBZ5H4K4FW! People does not exist in 1st instance. ID: " & CStr2(pDrInvoice("UserID")))
                Else
                    pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = "MobizLibraryPeople"
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vInvoiceID"></param>
    ''' <param name="vCustomerID"></param>
    ''' <param name="vMaxAgeInDays"></param>
    ''' <param name="v1stInstance">fill up if importing data to 2nd instance</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CacheInvoicesWithCredit(Optional vInvoiceID As String = Nothing,
        Optional vCustomerID As String = Nothing,
        Optional vMaxAgeInDays As Integer = 7,
        Optional v1stInstance As ProData = Nothing,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As Integer

        Dim pIU As InsertUpdate
        Dim pSql, pStr As String
        Dim pOut As Integer
        Dim pSourceSql As String
        'AppendToDebug("vbM5EK2AJLVK! CacheInvoicesWithCredit is called", True, , True)

        If String.IsNullOrWhiteSpace(vInvoiceID) = False _
            AndAlso v1stInstance IsNot Nothing Then

            AddDependencyData(vInvoiceID, v1stInstance)
        End If
        ' transferred to DbSchemaCoder.vb 2021_10_20__11_04_25_0130
        'mProData.AddColumn("CacheInvoices", "CashNetOfChange", "money")
        'mProData.AddColumn("CacheInvoices", "Eatigo", "money")
        'mProData.AddColumn("CacheInvoices", "DeliRush", "money")
        'mProData.AddColumn("CacheInvoices", "PoodTrip", "money")
        'mProData.AddColumn("CacheInvoices", "Share Treats", "money")
        'mProData.AddColumn("CacheInvoices", "Booky", "money")
        'mProData.AddColumn("CacheInvoices", "Lazada", "money")
        'mProData.AddColumn("CacheInvoices", "Pickaroo", "money")
        'mProData.AddColumn("CacheInvoices", "Riderko", "money")
        'mProData.AddColumn("CacheInvoices", "HomeCredit", "money")
        'mProData.AddColumn("CacheInvoices", "RegionID", "money")
        'mProData.AddColumn("CacheInvoices", "RegionName", "money")

        pSourceSql = InvoicesWithCreditSql(vInvoiceID, vCustomerID, vMaxAgeInDays, vInvoiceDateFrom, vInvoiceDateTo)
        mProData.CommandTimeout = 5 * 60
        mProData.BeginTransaction()
        If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoices', N'U')") IsNot DBNull.Value Then
            mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoices")
        End If

        ' BEGIN delete this code 2021_10_20__11_52_14_6447
        'pSql = "DELETE FROM CacheInvoices WHERE 1=1" & vbCrLf
        'If String.IsNullOrWhiteSpace(vInvoiceID) = False Then
        '    pSql &= " AND ID = " & ReadyString(vInvoiceID) & vbCrLf
        'End If
        'If String.IsNullOrWhiteSpace(vCustomerID) = False Then
        '    pSql &= " AND CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        'End If
        'pOut = mProData.ExecuteSQL(pSql) ' output is for debugging only
        ' END delete this code 2021_10_20__11_52_14_6447

        pIU = New InsertUpdate("#DeletableCacheInvoices")
        pIU("ID") = "ID"
        pIU("CustomerID") = "CustomerID"
        pIU("GrossInvoiceAmount") = "GrossInvoiceAmount"
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("LastUpdated") = "LastUpdated"
        pIU("InvoiceNumber") = "InvoiceNumber"
        pIU("InvoiceNo") = "InvoiceNo"
        pIU("CashAmount") = "CashAmount"
        pIU("CashNetOfChange") = "CashAmount - CustomerChangeCash"
        pIU("CheckPaymentAmount") = "CheckPaymentAmount"
        pIU("CheckPaymentAmountPdc") = "CheckPaymentAmountPdc"
        pIU("CreditCardAmount") = "CreditCardAmount"
        pIU("DebitCardAmount") = "DebitCardAmount"
        pIU("SRVAmount") = "SRVAmount"
        pIU("OtherTypeAmount") = "OtherTypeAmount"

        pIU("[Discount Voucher / On Line Deals]") = "[Discount Voucher / On Line Deals]"
        pIU("[E-Plus]") = "[E-Plus]"
        pIU("[FoodPanda]") = "[FoodPanda]"
        pIU("[GCash]") = "[GCash]"
        pIU("[GrabFood]") = "[GrabFood]"
        pIU("[SMAC]") = "[SMAC]"
        pIU("[V-Plus]") = "[V-Plus]"
        pIU("[PayMaya]") = "[PayMaya]"
        pIU("[ShopeeEVoucher]") = "[ShopeeEVoucher]"
        pIU("[ShopeePay]") = "[ShopeePay]"
        pIU("[GoldCard]") = "[GoldCard]"
        pIU("[LocalDelivery]") = "[LocalDelivery]"
        pIU("[Eatigo]") = "[Eatigo]"
        pIU("[DeliRush]") = "[DeliRush]"
        pIU("[PoodTrip]") = "[PoodTrip]"
        pIU("[Share Treats]") = "[Share Treats]"
        pIU("[Booky]") = "[Booky]"
        pIU("[Lazada]") = "[Lazada]"
        pIU("[Pickaroo]") = "[Pickaroo]"
        pIU("PotatoCornerGC") = "PotatoCornerGC"
        pIU("Riderko") = "Riderko"
        pIU("HomeCredit") = "HomeCredit"
        pIU("GrabPickup") = "GrabPickup"
        pIU("SupercardVoucher") = "SupercardVoucher"

        pIU("TotalPayment") = "ROUND(TotalPayment, 2)"
        pIU("UpdatedByName") = "UpdatedByName"
        pIU("CreditTerms") = "CreditTerms"
        pIU("InvoiceDueDate") = "InvoiceDueDate"
        pIU("AdjustmentAmount") = "AdjustmentAmount"
        pIU("DebitMemo") = "DebitMemo"
        pIU("CreditMemo") = "CreditMemo"
        pIU("NetInvoiceAmount") = "ROUND(GrossInvoiceAmount + AdjustmentAmount, 2)" ' add with "minus adjustments"
        pIU("NetInvoiceBalance") = "ROUND(GrossInvoiceAmount + AdjustmentAmount - TotalPayment, 2)" ' replace GrossInvoiceAmount with "net invoice amount"
        pIU("IsPrinted") = "IsPrinted"
        pIU("Remarks") = "Remarks"
        pIU("PurchaseOrderText") = "PurchaseOrderText"
        pIU("PurchaseOrderDate") = "PurchaseOrderDate"
        pIU("GrossSalesNetOfVATExemptions") = "GrossSalesNetOfVATExemptions"
        pIU("Discount") = "Discount"
        pIU("ItemsDiscount") = "ItemsDiscount"
        pIU("SCDiscount") = "SCDiscount"
        pIU("ForegoneVAT") = "ForegoneVAT"
        pIU("FowstID") = "FowstID"
        pIU("FOWSTName") = "FOWSTName"
        pIU("StoreType") = "StoreType"
        pIU("StoreLocation") = "StoreLocation"
        pIU("GroupName") = "GroupName"
        pIU("FranchiseGroupID") = "FranchiseGroupID"
        pIU("SubFOWSTTypeID") = "SubFOWSTTypeID"
        pIU("StoreLocationTypeID") = "StoreLocationTypeID"
        pIU("RegionID") = "RegionID"
        pIU("RegionName") = "RegionName"
        pIU("FOWSTCode") = "FOWSTCode"
        pIU("ChargeAmount") = "ChargeAmount"
        pIU("TaxAmount") = "TaxAmount"
        pIU("Categories") = "Categories"
        pIU("DeliveryMethod") = "DeliveryMethod"
        pIU("UserID") = "UserID"

        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetSelectInto()
        Dim pCurrentData = mProData.GetCurrentDatabase
        Try
            pOut = mProData.ExecuteSQL(pSql)
            'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
            '    "pSql: " & pSql & vbCrLf &
            '    "pOut: " & pOut
            'AppendToLog(pStr, , "CacheInvoicesWithCredit")
        Catch ex As Exception
            AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
            Throw
        End Try

        pIU.TableName = "CacheInvoices"
        pIU.FromSql = " FROM #DeletableCacheInvoices"
        pSql = pIU.GetUpdate("#DeletableCacheInvoices.ID = CacheInvoices.ID",
            vFromTable:=True,
            vUseFieldNameAsValue:=True,
            vValuePrefix:="#DeletableCacheInvoices.")
        Dim pDeletableCount = mProData.ExecuteScalar("SELECT COUNT(*) FROM #DeletableCacheInvoices")
        pSql = "-- rows in #DeletableCacheInvoices: " & pDeletableCount & vbCrLf & pSql
        Try
            pOut = mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            AppendToLog("vbU9BF8LA3R5 " & ex.Message & vbCrLf &
                pSourceSql)
            Throw
        End Try

        ' BEGIN used for debugging only 2022_09_07__11_39_21_8153
        pSql = "SELECT * FROM #DeletableCacheInvoices"
        Dim pDT = mProData.Execute(pSql)
        Dim pIDs = New List(Of String)
        For Each pDR As DataRow In pDT.Rows
            If pIDs.Contains(ReadyGuid(pDR("ID"))) Then
                AppendToLog("vb5WEL6XEY8R! InvoiceID: " & ReadyGuid(pDR("ID")) & "; GetCurrentDatabase: " & mProData.GetCurrentDatabase() & vbCrLf &
                    "pSourceSql: " & pSourceSql)
            End If
            pIDs.Add(ReadyGuid(pDR("ID")))
        Next
        ' END used for debugging only 2022_09_07__11_39_21_8153
        'oSourceRecordCount = CInt2(mProData.ExecuteScalar(pSql))

        pIU.FromSql = " FROM #DeletableCacheInvoices" & vbCrLf &
            " WHERE #DeletableCacheInvoices.ID NOT IN (" & vbCrLf &
            "   SELECT ID" & vbCrLf &
            "   FROM CacheInvoices" & vbCrLf &
            ")"
        pSql = pIU.GetInsert(vFromTable:=True,
            vUseFieldNameAsValue:=True,
            vValuePrefix:="#DeletableCacheInvoices.")
        pStr = "vbVQYAW423K8! pSql: " & pSql
        AppendToLog(pStr, , "CacheInvoicesWithCredit")
        pOut += mProData.ExecuteSQL(pSql)

        If CBool2(FileText(Common.ServerPath & "settings\AllowEas.txt")) Then
            pSql = "DELETE FROM CacheInvoices" & vbCrLf &
                " WHERE ID IN (" & vbCrLf &
                "   SELECT ID" & vbCrLf &
                "   FROM TransInvoice" & vbCrLf &
                "   WHERE TransInvoice.IsCancelled = 1" & vbCrLf &
                ")"
            pOut += mProData.ExecuteSQL(pSql)
        End If

        ' CacheWeightedAverageDaysOverdue(vCustomerID) ' delete this commented line 2021_03_18__11_22_42_5583
        mProData.CommitTransaction()

        Return pOut

    End Function

    Public Function InvoicesWithCreditSql(Optional vInvoiceID As String = Nothing,
        Optional vCustomerID As String = Nothing,
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As String

        Dim pSql As String

        pSql = " SELECT DISTINCT TransInvoice.ID, TransInvoice.CustomerID" & vbCrLf &
            "      ,TransInvoice.TotalAmount AS GrossInvoiceAmount" & vbCrLf &
            "      ,TransInvoice.InvoiceDate" & vbCrLf &
            "      ,TransInvoice.LastUpdated" & vbCrLf &
            "      ,TransInvoice.IsPrinted" & vbCrLf &
            "      ,TransInvoice.FowstID" & vbCrLf &
            "      ,TransInvoice.DeliveryMethod" & vbCrLf &
            "      ,TransInvoice.UserID" & vbCrLf &
            "      ,tblItems.GrossSalesNetOfVATExemptions" & vbCrLf &
            "      ,tblItems.TaxAmount" & vbCrLf &
            "      ,tblItems.Categories" & vbCrLf &
            "      ,tblItems.DiscountAmount + TransInvoice.SCDiscount + TransInvoice.SoloParentDiscount /*+ Transinvoice.ForegoneVAT*/ AS Discount" & vbCrLf & 'Commented by Arnie 090125 - gi comment kay mali ang total Discounts sa Sales Dashboard per group
            "      ,tblItems.DiscountAmount AS ItemsDiscount" & vbCrLf &
            "      ,TransInvoice.SCDiscount" & vbCrLf &
            "      ,Transinvoice.ForegoneVAT" & vbCrLf &
            "      ,TransInvoice.PurchaseOrderText" & vbCrLf &
            "      ,TransInvoice.PurchaseOrderDate" & vbCrLf &
            "      ,MobizLibraryOrganizationFOWST.Name FOWSTName" & vbCrLf &
            "      ,MobizLibraryOrganizationFOWST.Code FOWSTCode" & vbCrLf &
            "      ,MobizLibraryFOWSTRetailStoreType.Name StoreType" & vbCrLf &
            "      ,MobizLibraryFOWSTStoreLocationType.LocationType StoreLocation" & vbCrLf &
            "      ,LibraryFranchiseGroup.GroupName GroupName" & vbCrLf &
            "      ,LibraryFranchiseGroup.RegionID " & vbCrLf &
            "      ,LibraryFranchiseGroup.RegionName" & vbCrLf &
            "      ,LibraryFranchiseGroup.ID FranchiseGroupID" & vbCrLf &
            "      ,MobizLibraryFOWSTRetailStoreType.ID SubFOWSTTypeID" & vbCrLf &
            "      ,MobizLibraryFOWSTStoreLocationType.ID StoreLocationTypeID" & vbCrLf &
            "      ,TransInvoice.InvoiceNumber AS InvoiceNo" & vbCrLf &
            "      ,ISNULL(LibraryDocumentControl.DocumentName, CASE" & vbCrLf &
            "      	WHEN ISNULL(TransInvoice.InvoiceNumber, 0) > 0 THEN 'SI'" & vbCrLf &
            "         WHEN ISNULL(TransInvoice.SalesOrderNo, 0) > 0 THEN 'SO'" & vbCrLf &
            "         WHEN ISNULL(TransInvoice.PurchaseOrderNo, 0) > 0 THEN 'PO'" & vbCrLf &
            "         ELSE ''" & vbCrLf &
            "         END) + '-' +" & vbCrLf &
            "      	+ ISNULL(CONVERT(varchar, MobizLibraryOrganizationFOWST.FowstNo), '') + '-'" & vbCrLf &
            "         + ISNULL(CONVERT(varchar, LibraryDevice.DeviceNo), '') + '-'" & vbCrLf &
            "         + CONVERT(varchar, " & vbCrLf &
            " CASE" & vbCrLf &
            "       	 WHEN ISNULL(TransInvoice.InvoiceNumber, 0) > 0 THEN TransInvoice.InvoiceNumber" & vbCrLf &
            "          WHEN ISNULL(TransInvoice.SalesOrderNo, 0) > 0 THEN TransInvoice.SalesOrderNo" & vbCrLf &
            "          WHEN ISNULL(TransInvoice.PurchaseOrderNo, 0) > 0 THEN TransInvoice.PurchaseOrderNo" & vbCrLf &
            "          ELSE 0" & vbCrLf &
            "          END" & vbCrLf &
            ") AS InvoiceNumber" & vbCrLf &
            "       ,ISNULL(tblPayments.AmountPaid, 0) AS TotalPayment" & vbCrLf &
            "       ,ISNULL(tblPayments.StuffCreditRemarks, '') AS Remarks" & vbCrLf &
            "       ,ISNULL(tblPayments.ChargeAmount, 0) AS ChargeAmount" & vbCrLf &
            "       ,ISNULL(tblAdjustments.Amount, 0) AS AdjustmentAmount" & vbCrLf &
            "       ,ISNULL(tblAdjustments.DebitMemo, 0) AS DebitMemo" & vbCrLf &
            "       ,ISNULL(tblAdjustments.CreditMemo, 0) AS CreditMemo" & vbCrLf &
            "       ,ISNULL(tblCheckPayment.AmountPaid, 0) AS CheckPaymentAmount" & vbCrLf &
            "       ,tblCheckPaymentPdc.AmountPaid AS CheckPaymentAmountPdc" & vbCrLf &
            "       ,ISNULL(tblCustomerPayment.CashAmount, 0) AS CashAmount" & vbCrLf &
            "       ,ISNULL(tblCustomerPayment.CustomerChangeCash, 0) AS CustomerChangeCash" & vbCrLf &
            "       ,ISNULL(tblCustomerPayment.CreditCardAmount, 0) AS CreditCardAmount" & vbCrLf &
            "       ,ISNULL(tblCustomerPayment.DebitCardAmount, 0) AS DebitCardAmount" & vbCrLf &
            "       ,ISNULL(tblCustomerPayment.SRVAmount, 0) AS SRVAmount" & vbCrLf &
            "       ,ISNULL(tblCustomerPayment.OtherTypeAmount, 0) AS OtherTypeAmount" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[Discount Voucher / On Line Deals], 0) AS [Discount Voucher / On Line Deals]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[E-Plus], 0) AS [E-Plus]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[FoodPanda], 0) AS [FoodPanda]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[GCash], 0) AS [GCash]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[GrabFood], 0) AS [GrabFood]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[SMAC], 0) AS [SMAC]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[V-Plus], 0) AS [V-Plus]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[PayMaya], 0) AS [PayMaya]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[ShopeeEVoucher], 0) AS [ShopeeEVoucher]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[ShopeePay], 0) AS [ShopeePay]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[GoldCard], 0) AS [GoldCard]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[LocalDelivery], 0) AS [LocalDelivery]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[Eatigo], 0) AS [Eatigo]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[DeliRush], 0) AS [DeliRush]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[PoodTrip], 0) AS [PoodTrip]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[ShareTreats], 0) AS [Share Treats]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[Booky], 0) AS [Booky]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[Lazada], 0) AS [Lazada]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[Pickaroo], 0) AS [Pickaroo]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[PotatoCornerGC], 0) AS [PotatoCornerGC]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[Riderko], 0) AS [Riderko]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[HomeCredit], 0) AS [HomeCredit]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[GrabPickup], 0) AS [GrabPickup]" & vbCrLf &
            "       ,ISNULL(tblCustomerPaymentOthers.[SupercardVoucher], 0) AS [SupercardVoucher]" & vbCrLf &
            "       ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.MiddleName AS UpdatedByName" & vbCrLf &
            "       ,ISNULL(TransInvoice.PaymentTerm, 0) AS CreditTerms" & vbCrLf &
            "       ,TransInvoice.InvoiceDate + ISNULL(TransInvoice.PaymentTerm, 0) AS InvoiceDueDate" & vbCrLf &
            "       FROM TransInvoice" & vbCrLf &
            "       LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            "       LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            "       LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            "       LEFT JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
            "       LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf &
            "       LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
            "       LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
            "       OUTER APPLY (" & vbCrLf &
            " 			SELECT TOP 1 LibraryFranchiseGroup.*, LibraryRegionalGroupSettings.RegionName, LibraryRegionalGroupSettings.ID RegionID " & vbCrLf &
            " 			FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
            " 			JOIN LibraryFranchiseGroup ON LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
            "           LEFT JOIN LibraryRegionalGroupSettingsDetails on LibraryRegionalGroupSettingsDetails.GroupID = LibraryFranchiseGroup.ID " & vbCrLf &
            "           LEFT JOIN LibraryRegionalGroupSettings on LibraryRegionalGroupSettings.ID = LibraryRegionalGroupSettingsDetails.RegionalGroupID " & vbCrLf &
            " 			WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
            " 	   ) LibraryFranchiseGroup " & vbCrLf &
            "       LEFT JOIN (" & vbCrLf &
            "       	SELECT TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            "           ,SUM(TransCustomerPaymentInvoices.AmountPaid) AS AmountPaid" & vbCrLf &
            "           ,SUM(TransCustomerPayment.ChargeAmount) AS ChargeAmount" & vbCrLf &
            "            /* -- commented 2024_01_06__09_12_43_5349" & vbCrLf &
            "            ,STUFF(" & vbCrLf &
            "            		(SELECT DISTINCT '; ' + TransCustomerPayment_1.OnCreditRemarks" & vbCrLf &
            "                 FROM TransCustomerPayment TransCustomerPayment_1" & vbCrLf &
            "                 JOIN TransCustomerPaymentInvoices TransCustomerPaymentInvoices_1 ON TransCustomerPaymentInvoices_1.CustomerPaymentID = TransCustomerPayment_1.ID" & vbCrLf &
            "                 WHERE TransCustomerPaymentInvoices_1.InvoiceID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            " 				FOR XML PATH (''))" & vbCrLf &
            "            , 1, 1, '') AS StuffCreditRemarks" & vbCrLf &
            "            */ -- commented 2024_01_06__09_12_43_5349" & vbCrLf &
            "           ,'' AS StuffCreditRemarks" & vbCrLf &
            "           FROM TransCustomerPayment" & vbCrLf &
            "           JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPayment.ID" & vbCrLf &
            "           GROUP BY TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            "        ) tblPayments ON tblPayments.InvoiceID = TransInvoice.ID" & vbCrLf &
            "         LEFT JOIN (" & vbCrLf &
            "            SELECT ISNULL(SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions), 0) AS GrossSalesNetOfVATExemptions" & vbCrLf &
            "            ,ISNULL(SUM(TransInvoiceItems.DiscountAmount), 0) AS DiscountAmount" & vbCrLf &
            "            ,ISNULL(SUM(TransInvoiceItems.TaxAmount), 0) AS TaxAmount" & vbCrLf &
            "            ,TransInvoiceItems.InvoiceID" & vbCrLf &
            "            ,STUFF(" & vbCrLf &
            "                (SELECT DISTINCT ', ' + MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "                FROM TransInvoiceItems items" & vbCrLf &
            "                JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = items.ProductID" & vbCrLf &
            "                WHERE items.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "                ORDER BY ', ' + MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "  				 FOR XML PATH (''))" & vbCrLf &
            "             , 1, 1, '') AS Categories" & vbCrLf &
            "            FROM TransInvoiceItems" & vbCrLf &
            "            GROUP BY TransInvoiceItems.InvoiceID" & vbCrLf &
            "         ) tblItems ON tblItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "         LEFT JOIN (" & vbCrLf &
            "            SELECT ISNULL(SUM(TransInvoiceAdjustments.Amount), 0) AS Amount" & vbCrLf &
            "            ,SUM(CASE WHEN TransInvoiceAdjustments.Amount > 0 THEN TransInvoiceAdjustments.Amount END) AS DebitMemo" & vbCrLf &
            "            ,SUM(CASE WHEN TransInvoiceAdjustments.Amount < 0 THEN TransInvoiceAdjustments.Amount END) * -1 AS CreditMemo" & vbCrLf &
            "            ,TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            "            FROM TransInvoiceAdjustments" & vbCrLf &
            "            WHERE TransInvoiceAdjustments.IsActive = 1" & vbCrLf &
            "            GROUP BY TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            "         ) tblAdjustments ON tblAdjustments.InvoiceID = TransInvoice.ID" & vbCrLf &
            " 	LEFT JOIN (" & vbCrLf &
            "     	SELECT TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
            "         ,SUM(TransCustomerPaymentCheckAmount.AmountPaid) AS AmountPaid" & vbCrLf &
            "         FROM TransCustomerPaymentCheckAmount" & vbCrLf &
            "         GROUP BY TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
            "     ) tblCheckPayment ON tblCheckPayment.InvoiceID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN (" & vbCrLf &
            "      	SELECT TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
            "          ,SUM(TransCustomerPaymentCheckAmount.AmountPaid) AS AmountPaid" & vbCrLf &
            "          FROM TransCustomerPaymentCheckAmount" & vbCrLf &
            "          JOIN TransCustomerPaymentChecks ON TransCustomerPaymentChecks.ID = TransCustomerPaymentCheckAmount.CustomerPaymentCheckID" & vbCrLf &
            "          WHERE TransCustomerPaymentChecks.CheckDate > GETDATE()" & vbCrLf &
            "          GROUP BY TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
            "      ) tblCheckPaymentPdc ON tblCheckPaymentPdc.InvoiceID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN (" & vbCrLf &
            "        SELECT TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            "        , SUM(TransCustomerPayment.CashAmount) CashAmount" & vbCrLf &
            "        , SUM(TransCustomerPayment.CustomerChangeCash) CustomerChangeCash" & vbCrLf &
            "        , SUM(TransCustomerPayment.DebitCardAmount) DebitCardAmount" & vbCrLf &
            "        , SUM(TransCustomerPayment.CreditCardAmount) CreditCardAmount" & vbCrLf &
            "        , SUM(TransCustomerPayment.SRVAmount) SRVAmount" & vbCrLf &
            "        , SUM(TransCustomerPayment.OtherTypeAmount) OtherTypeAmount" & vbCrLf &
            "        FROM TransCustomerPaymentInvoices" & vbCrLf &
            "        JOIN TransCustomerPayment on TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            "        GROUP BY TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            "      ) tblCustomerPayment ON tblCustomerPayment.InvoiceID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN ( " & vbCrLf &
            "       SELECT * FROM    " & vbCrLf &
            "       (SELECT TransCustomerPaymentInvoices.InvoiceID, PaymentType, Amount " & vbCrLf &
            "       FROM TransCustomerPaymentInvoices " & vbCrLf &
            "       INNER JOIN TransCustomerPaymentOther on  TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID" & vbCrLf &
            "       ) p  " & vbCrLf &
            "       PIVOT(SUM(Amount)" & vbCrLf &
            "       FOR PaymentType in ( " & vbCrLf &
            "       [Discount Voucher / On Line Deals], [E-Plus], [FoodPanda], [GCash], [GrabFood], [SMAC], [V-Plus], [PayMaya], [ShopeeEVoucher], [ShopeePay], [GoldCard], [LocalDelivery], [Eatigo], [DeliRush], [PoodTrip], [ShareTreats], [Booky], [Lazada], [Pickaroo], [PotatoCornerGC], [Riderko], [HomeCredit], [GrabPickup], [SupercardVoucher])) as OtherPayment" & vbCrLf &
            "     ) tblCustomerPaymentOthers ON tblCustomerPaymentOthers.InvoiceID = TransInvoice.ID" & vbCrLf &
            "     LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " AND ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf
        If String.IsNullOrWhiteSpace(vInvoiceID) = False Then
            pSql &= " AND TransInvoice.ID = " & ReadyString(vInvoiceID) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vCustomerID) = False Then
            pSql &= " AND TransInvoice.CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vInvoiceID) _
            AndAlso String.IsNullOrWhiteSpace(vCustomerID) Then

            If vInvoiceDateFrom IsNot Nothing Then
                pSql &= " AND CONVERT(date, TransInvoice.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If vInvoiceDateTo IsNot Nothing Then
                pSql &= " AND CONVERT(date, TransInvoice.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            pSql &= " AND DATEDIFF(minute, TransInvoice.InvoiceDate, GETDATE()) <= 60 * 24 * " & vMaxAgeInDays & " -- number of minutes in 7 days; dont compare using days" & vbCrLf
        End If
        Return pSql

    End Function

    Public Function CustomersWithCredit(vFilter As String, Optional vWhere As String = Nothing) As DataTable

        Dim pOut As DataTable

        pOut = mProData.Execute(CustomersWithCreditSql(vFilter, vWhere))

        Return pOut

    End Function

    Public Function CacheInvoicesModifiers(Optional vInvoiceID As String = Nothing,
        Optional vCustomerID As String = Nothing,
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pSourceSql As String

        pSourceSql = InvoiceModifiersSql(vInvoiceID, vCustomerID, vMaxAgeInDays, vInvoiceDateFrom:=vInvoiceDateFrom, vInvoiceDateTo:=vInvoiceDateTo)

        If Now.Date = #2/1/2023# Then
            AppendToLog("vbGCA4XXEMCY! pSourceSql: " & pSourceSql, , "CacheInvoicesModifiers")
        End If
        mProData.CommandTimeout = 15 * 60
        mProData.BeginTransaction()
        If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoiceModifiers', N'U')") IsNot DBNull.Value Then
            mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoiceModifiers")
        End If

        pIU = New InsertUpdate("#DeletableCacheInvoiceModifiers")
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("ProductID") = "ProductID"
        pIU("ProductNo") = "ProductNo"
        pIU("ProductName") = "ProductName"
        pIU("ProductBrand") = "ProductBrand"
        pIU("ProductQuantity") = "ProductQuantity"
        pIU("ProductAmount") = "ProductAmount"
        pIU("ModifierProductID") = "ModifierProductID"
        pIU("ModifierProductNumber") = "ModifierProductNumber"
        pIU("ModifierProductName") = "ModifierProductName"
        pIU("ModifierBrand") = "ModifierBrand"
        pIU("ModifierQuantity") = "ModifierQuantity"
        pIU("ModifierAmount") = "ModifierAmount"
        pIU("ModifierUnitMeasure") = "ModifierUnitMeasure"
        pIU("Flavor") = "Flavor"
        pIU("FowstID") = "FowstID"
        pIU("FowstName") = "FowstName"
        pIU("ItemModifierName") = "ItemModifierName"
        pIU("ItemModifierID") = "ItemModifierID"
        pIU("IsPromo") = "IsPromo"
        pIU("GroupName") = "GroupName"
        pIU("FranchiseGroupID") = "FranchiseGroupID"
        pIU("InvoiceCount") = "InvoiceCount"

        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetSelectInto()
        Dim pCurrentData = mProData.GetCurrentDatabase
        Try
            pOut = mProData.ExecuteSQL(pSql)
            'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
            '    "pSql: " & pSql & vbCrLf &
            '    "pOut: " & pOut
            'AppendToLog(pStr, , "CacheInvoicesWithCredit")
        Catch ex As Exception
            AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
            Throw
        End Try

        pIU.TableName = "CacheInvoiceModifiers"
        pIU.FromSql = " FROM #DeletableCacheInvoiceModifiers"
        pSql = pIU.GetUpdate("#DeletableCacheInvoiceModifiers.InvoiceDate = CacheInvoiceModifiers.InvoiceDate" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ProductID = CacheInvoiceModifiers.ProductID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ModifierProductID = CacheInvoiceModifiers.ModifierProductID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ItemModifierID = CacheInvoiceModifiers.ItemModifierID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.FOWSTID = CacheInvoiceModifiers.FOWSTID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ModifierUnitMeasure = CacheInvoiceModifiers.ModifierUnitMeasure" & vbCrLf,
            vFromTable:=True,
            vUseFieldNameAsValue:=True,
            vValuePrefix:="#DeletableCacheInvoiceModifiers.")
        pOut = mProData.ExecuteSQL(pSql)

        pIU.FromSql = " FROM #DeletableCacheInvoiceModifiers" & vbCrLf &
            " WHERE NOT EXISTS (" & vbCrLf &
            "   SELECT 1" & vbCrLf &
            "   FROM CacheInvoiceModifiers" & vbCrLf &
            "   WHERE #DeletableCacheInvoiceModifiers.InvoiceDate = CacheInvoiceModifiers.InvoiceDate" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ProductID = CacheInvoiceModifiers.ProductID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ModifierProductID = CacheInvoiceModifiers.ModifierProductID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ItemModifierID = CacheInvoiceModifiers.ItemModifierID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.FOWSTID = CacheInvoiceModifiers.FOWSTID" & vbCrLf &
            " AND #DeletableCacheInvoiceModifiers.ModifierUnitMeasure = CacheInvoiceModifiers.ModifierUnitMeasure" & vbCrLf &
            ")"
        pSql = pIU.GetInsert(vFromTable:=True,
            vUseFieldNameAsValue:=True,
            vValuePrefix:="#DeletableCacheInvoiceModifiers.")
        pOut += mProData.ExecuteSQL(pSql)

        mProData.CommitTransaction()

        Return pOut

    End Function

    Private Function InvoiceModifiersSql(Optional vInvoiceID As String = Nothing,
        Optional vCustomerID As String = Nothing,
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As String

        Dim pSql = " -- cache invoice modifier" & vbCrLf &
            " SELECT CONVERT(date, TransInvoice.InvoiceDate) AS InvoiceDate" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,tblPimItem.ProductNo" & vbCrLf &
            " ,tblPimItem.ProductName" & vbCrLf &
            " ,tblPimItem.Brand AS ProductBrand" & vbCrLf &
            " ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS ProductQuantity" & vbCrLf &
            " ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.UnitPrice) AS ProductAmount" & vbCrLf &
            " ,tblPimInventoryAffecting.ID AS ModifierProductID" & vbCrLf &
            " ,tblPimInventoryAffecting.ProductNo AS ModifierProductNumber" & vbCrLf &
            " ,tblPimInventoryAffecting.ProductName AS ModifierProductName" & vbCrLf &
            " ,tblPimInventoryAffecting.Brand AS ModifierBrand" & vbCrLf &
            " ,SUM(TransInvoiceItemsModifier.InputQty) AS ModifierQuantity" & vbCrLf &
            " ,SUM(TransInvoiceItemsModifier.SubTotal) AS ModifierAmount" & vbCrLf &
            " ,LibraryItemModifierInventoryAffecting.UnitMeasure AS ModifierUnitMeasure" & vbCrLf &
            " ,LibraryItemModifierInventoryAffecting.Description AS Flavor" & vbCrLf &
            " ,LibraryItemModifiers.Name AS ItemModifierName" & vbCrLf &
            " ,LibraryItemModifiers.IsPromo" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,TransInvoiceItemsModifier.ItemModifierID" & vbCrLf &
            " ,tblGroup.GroupName" & vbCrLf &
            " ,tblGroup.FranchiseGroupID" & vbCrLf &
            " ,COUNT(TransInvoice.ID) AS InvoiceCount" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct tblPimItem ON tblPimItem.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " JOIN TransInvoiceItemsModifier ON TransInvoiceItemsModifier.InvoiceItemID = TransInvoiceItems.ID" & vbCrLf &
            " JOIN LibraryItemModifierInventoryAffecting ON LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID" & vbCrLf &
            " LEFT JOIN LibraryItemModifiers ON LibraryItemModifiers.ID = TransInvoiceItemsModifier.ItemModifierID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct tblPimInventoryAffecting ON tblPimInventoryAffecting.ID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            " OUTER APPLY (SELECT TOP 1 LibraryFranchiseGroup.GroupName" & vbCrLf &
            "   ,MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
            "   FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            "   LEFT JOIN LibraryFranchiseGroup ON MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID" & vbCrLf &
            "   WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed = 1" & vbCrLf &
            "   AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed = 1" & vbCrLf &
            "   AND MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = TransInvoice.FowstID" & vbCrLf &
            ") tblGroup" & vbCrLf &
            " WHERE 1=1" & vbCrLf

        If String.IsNullOrWhiteSpace(vInvoiceID) = False Then
            pSql &= " AND TransInvoice.ID = " & ReadyString(vInvoiceID) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vCustomerID) = False Then
            pSql &= " AND TransInvoice.CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vInvoiceID) _
            AndAlso String.IsNullOrWhiteSpace(vCustomerID) Then

            If vInvoiceDateFrom IsNot Nothing Then
                pSql &= " AND CONVERT(date, TransInvoice.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If vInvoiceDateTo IsNot Nothing Then
                pSql &= " AND CONVERT(date, TransInvoice.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            pSql &= " AND DATEDIFF(minute, TransInvoice.InvoiceDate, GETDATE()) <= 60 * 24 * " & vMaxAgeInDays & " -- number of minutes in 7 days; dont compare using days" & vbCrLf
        End If

        pSql &= " GROUP BY CONVERT(date, TransInvoice.InvoiceDate)" & vbCrLf &
            " ,tblPimItem.ProductName" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,tblPimItem.ProductNo" & vbCrLf &
            " ,tblPimItem.Brand" & vbCrLf &
            " ,tblPimInventoryAffecting.ProductNo" & vbCrLf &
            " ,tblPimInventoryAffecting.ProductName" & vbCrLf &
            " ,tblPimInventoryAffecting.Brand" & vbCrLf &
            " ,LibraryItemModifierInventoryAffecting.UnitMeasure" & vbCrLf &
            " ,LibraryItemModifierInventoryAffecting.Description" & vbCrLf &
            " ,LibraryItemModifiers.Name" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " ,TransInvoiceItemsModifier.ItemModifierID" & vbCrLf &
            " ,LibraryItemModifiers.IsPromo" & vbCrLf &
            " ,tblGroup.GroupName" & vbCrLf &
            " ,tblGroup.FranchiseGroupID" & vbCrLf &
            " ,tblPimInventoryAffecting.ID"

        Return pSql

    End Function

    Public Function CacheInvoiceFlavors(
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As Integer

        Dim pDaysOffset As Integer
        Dim pAffectedRecords As Integer
        Do
            Dim pInvoiceDateFromActual As Date?
            Dim pInvoiceDateToActual As Date?

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                If Math.Abs((vInvoiceDateFrom.Value - vInvoiceDateTo.Value).Days) = 0 Then
                    Exit Do
                End If

                pInvoiceDateFromActual = vInvoiceDateFrom.Value.AddDays(pDaysOffset)
                pInvoiceDateToActual = pInvoiceDateFromActual
            Else
                Exit Do
            End If

            pAffectedRecords += CacheInvoiceFlavors(9999, pInvoiceDateFromActual, pInvoiceDateToActual)

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                pDaysOffset += 1
                If pInvoiceDateFromActual.Value >= vInvoiceDateTo.Value Then
                    Return pAffectedRecords
                End If
            Else
                Return pAffectedRecords
            End If
        Loop

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pSourceSql As String

        pSourceSql = InvoiceFlavorsSql(vMaxAgeInDays, vInvoiceDateFrom:=vInvoiceDateFrom, vInvoiceDateTo:=vInvoiceDateTo)

        mProData.CommandTimeout = 15 * 60
        mProData.BeginTransaction()
        If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoiceFlavors', N'U')") IsNot DBNull.Value Then
            mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoiceFlavors")
        End If

        pIU = New InsertUpdate("#DeletableCacheInvoiceFlavors")
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("ModifierProductID") = "ModifierProductID"
        pIU("ModifierProductNumber") = "ModifierProductNumber"
        pIU("ModifierProductName") = "ModifierProductName"
        pIU("ModifierBrand") = "ModifierBrand"
        pIU("ModifierQuantity") = "ModifierQuantity"
        pIU("ModifierAmount") = "ModifierAmount"
        pIU("ModifierUnitMeasure") = "ModifierUnitMeasure"
        pIU("FowstID") = "FowstID"
        pIU("FowstName") = "FowstName"
        pIU("FowstCode") = "FowstCode"
        pIU("InvoiceCount") = "InvoiceCount"
        pIU("GroupName") = "GroupName"
        pIU("FranchiseGroupID") = "FranchiseGroupID"
        pIU("SalesValue") = "SalesValue"

        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetSelectInto()
        Dim pCurrentData = mProData.GetCurrentDatabase
        Try
            pOut = mProData.ExecuteSQL(pSql)
            'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
            '    "pSql: " & pSql & vbCrLf &
            '    "pOut: " & pOut
            'AppendToLog(pStr, , "CacheInvoicesWithCredit")
        Catch ex As Exception
            AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
            Throw
        End Try

        If mProData.TableExist("CacheInvoiceFlavors") Then
            pIU.TableName = "CacheInvoiceFlavors"
            pIU.FromSql = " FROM #DeletableCacheInvoiceFlavors"

            pSql = pIU.GetUpdate("#DeletableCacheInvoiceFlavors.InvoiceDate = CacheInvoiceFlavors.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceFlavors.ModifierProductID = CacheInvoiceFlavors.ModifierProductID" & vbCrLf &
                " AND #DeletableCacheInvoiceFlavors.FOWSTID = CacheInvoiceFlavors.FOWSTID" & vbCrLf &
                " AND #DeletableCacheInvoiceFlavors.ModifierUnitMeasure = CacheInvoiceFlavors.ModifierUnitMeasure" & vbCrLf,
                vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceFlavors.")
            pOut = mProData.ExecuteSQL(pSql)

            pIU.FromSql = " FROM #DeletableCacheInvoiceFlavors" & vbCrLf &
                " WHERE NOT EXISTS (" & vbCrLf &
                "   SELECT 1" & vbCrLf &
                "   FROM CacheInvoiceFlavors" & vbCrLf &
                "   WHERE #DeletableCacheInvoiceFlavors.InvoiceDate = CacheInvoiceFlavors.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceFlavors.ModifierProductID = CacheInvoiceFlavors.ModifierProductID" & vbCrLf &
                " AND #DeletableCacheInvoiceFlavors.FOWSTID = CacheInvoiceFlavors.FOWSTID" & vbCrLf &
                " AND #DeletableCacheInvoiceFlavors.ModifierUnitMeasure = CacheInvoiceFlavors.ModifierUnitMeasure" & vbCrLf &
                ")"
            pSql = pIU.GetInsert(vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceFlavors.")
            pOut += mProData.ExecuteSQL(pSql)
        Else
            pSql = "SELECT * INTO CacheInvoiceFlavors FROM #DeletableCacheInvoiceFlavors"
            pOut += mProData.ExecuteSQL(pSql)
        End If

        mProData.CommitTransaction()

        Return pOut

    End Function

    Private Function InvoiceFlavorsSql(
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As String

        Dim pSql = "   -- CacheInvoiceFlavors" & vbCrLf &
            "   SELECT CONVERT(date, CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            "   ,MobizLibraryPIMProductVariableFeatures001.ProductID AS ModifierProductID" & vbCrLf &
            "   ,tblPimInventoryAffecting.ProductNo AS ModifierProductNumber" & vbCrLf &
            "   ,MobizLibraryPIMProductVariableFeatures001.c0092 AS ModifierProductName" & vbCrLf &
            "   ,tblPimInventoryAffecting.Brand AS ModifierBrand" & vbCrLf &
            "   ,SUM(TransInvoiceItemsModifier.InputQty) AS ModifierQuantity" & vbCrLf &
            "   ,SUM(TransInvoiceItemsModifier.SubTotal) AS ModifierAmount" & vbCrLf &
            "   ,LibraryItemModifierInventoryAffecting.UnitMeasure AS ModifierUnitMeasure" & vbCrLf &
            " --  ,LibraryItemModifierInventoryAffecting.Description AS Flavor" & vbCrLf &
            "   ,CacheInvoices.FowstID" & vbCrLf &
            "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS SalesValue" & vbCrLf &
            "   ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            "   ,MobizLibraryOrganizationFOWST.Code As FowstCode" & vbCrLf &
            "   ,COUNT(CacheInvoices.ID) AS InvoiceCount" & vbCrLf &
            "  ,CacheInvoices.GroupName" & vbCrLf &
            "  ,CacheInvoices.FranchiseGroupID" & vbCrLf &
            "   FROM CacheInvoices" & vbCrLf &
            "   JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = CacheInvoices.ID" & vbCrLf &
            "   JOIN TransInvoiceItemsModifier ON TransInvoiceItemsModifier.InvoiceItemID = TransInvoiceItems.ID" & vbCrLf &
            "   JOIN LibraryItemModifierInventoryAffecting ON LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID" & vbCrLf &
            "   JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            "   LEFT JOIN LibraryItemModifiers ON LibraryItemModifiers.ID = TransInvoiceItemsModifier.ItemModifierID" & vbCrLf &
            "   LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID" & vbCrLf &
            "   JOIN MobizLibraryPIMProduct tblPimInventoryAffecting ON tblPimInventoryAffecting.ID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            "   WHERE MobizLibraryPIMProductVariableFeatures001.c0092 <> 'N/A'" & vbCrLf

        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        pSql &= " AND DATEDIFF(minute, CacheInvoices.InvoiceDate, GETDATE()) <= 60 * 24 * " & vMaxAgeInDays & " -- number of minutes in 7 days; dont compare using days" & vbCrLf

        pSql &= "  GROUP BY CONVERT(date, CacheInvoices.InvoiceDate)" & vbCrLf &
            "   ,MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
            "   ,LibraryItemModifierInventoryAffecting.UnitMeasure" & vbCrLf &
            "   ,tblPimInventoryAffecting.ProductNo" & vbCrLf &
            "   ,MobizLibraryPIMProductVariableFeatures001.c0092" & vbCrLf &
            "   ,CacheInvoices.FowstID" & vbCrLf &
            "   ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            "   ,MobizLibraryOrganizationFOWST.Code" & vbCrLf &
            "  ,CacheInvoices.GroupName" & vbCrLf &
            "  ,CacheInvoices.FranchiseGroupID" & vbCrLf &
            "  ,tblPimInventoryAffecting.Brand"

        Return pSql

    End Function

    Public Function CacheInvoiceComboMix(
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As Integer

        Dim pDaysOffset As Integer
        Dim pAffectedRecords As Integer
        Do
            Dim pInvoiceDateFromActual As Date?
            Dim pInvoiceDateToActual As Date?

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                If Math.Abs((vInvoiceDateFrom.Value - vInvoiceDateTo.Value).Days) = 0 Then
                    Exit Do
                End If

                pInvoiceDateFromActual = vInvoiceDateFrom.Value.AddDays(pDaysOffset)
                pInvoiceDateToActual = pInvoiceDateFromActual
            Else
                Exit Do
            End If

            pAffectedRecords += CacheInvoiceComboMix(pInvoiceDateFromActual, pInvoiceDateToActual)

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                pDaysOffset += 1
                If pInvoiceDateFromActual.Value >= vInvoiceDateTo.Value Then
                    Return pAffectedRecords
                End If
            Else
                Return pAffectedRecords
            End If
        Loop

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pSourceSql As String

        pSourceSql = InvoiceComboMixSql(vInvoiceDateFrom:=vInvoiceDateFrom, vInvoiceDateTo:=vInvoiceDateTo)

        mProData.CommandTimeout = 15 * 60
        mProData.BeginTransaction()
        If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoiceComboMix', N'U')") IsNot DBNull.Value Then
            mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoiceComboMix")
        End If

        pIU = New InsertUpdate("#DeletableCacheInvoiceComboMix")
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("ProductName") = "ProductName"
        pIU("GroupName") = "GroupName"
        pIU("FranchiseGroupID") = "FranchiseGroupID"
        pIU("FowstID") = "FowstID"
        pIU("FowstName") = "FowstName"
        pIU("FowstCode") = "FowstCode"
        pIU("TotalSales") = "TotalSales"
        pIU("TransactionCount") = "TransactionCount"
        pIU("ProductCount") = "ProductCount"
        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetSelectInto()
        Dim pCurrentData = mProData.GetCurrentDatabase
        Try
            pOut = mProData.ExecuteSQL(pSql)
            'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
            '    "pSql: " & pSql & vbCrLf &
            '    "pOut: " & pOut
            'AppendToLog(pStr, , "CacheInvoicesWithCredit")
        Catch ex As Exception
            AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
            Throw
        End Try

        If mProData.TableExist("CacheInvoiceComboMix") Then
            pIU.TableName = "CacheInvoiceComboMix"
            pIU.FromSql = " FROM #DeletableCacheInvoiceComboMix"

            pSql = pIU.GetUpdate("#DeletableCacheInvoiceComboMix.InvoiceDate = CacheInvoiceComboMix.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceComboMix.ProductName = CacheInvoiceComboMix.ProductName" & vbCrLf &
                " AND #DeletableCacheInvoiceComboMix.FOWSTID = CacheInvoiceComboMix.FOWSTID" & vbCrLf,
                vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceComboMix.")
            pOut = mProData.ExecuteSQL(pSql)

            pIU.FromSql = " FROM #DeletableCacheInvoiceComboMix" & vbCrLf &
                " WHERE NOT EXISTS (" & vbCrLf &
                "   SELECT 1" & vbCrLf &
                "   FROM CacheInvoiceComboMix" & vbCrLf &
                "   WHERE #DeletableCacheInvoiceComboMix.InvoiceDate = CacheInvoiceComboMix.InvoiceDate" & vbCrLf &
                "   AND #DeletableCacheInvoiceComboMix.ProductName = CacheInvoiceComboMix.ProductName" & vbCrLf &
                "   AND #DeletableCacheInvoiceComboMix.FOWSTID = CacheInvoiceComboMix.FOWSTID" & vbCrLf &
                ")"
            pSql = pIU.GetInsert(vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceComboMix.")
            pOut += mProData.ExecuteSQL(pSql)
        Else
            pSql = "SELECT * INTO CacheInvoiceComboMix FROM #DeletableCacheInvoiceComboMix"
            pOut += mProData.ExecuteSQL(pSql)
        End If

        mProData.CommitTransaction()

        Return pOut

    End Function

    Private Function InvoiceComboMixSql(
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As String

        Dim pSql = " -- CacheInvoiceComboMixSql" & vbCrLf &
            " SELECT TOP 100 PERCENT" & vbCrLf &
            " CONVERT(date, CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            " ,CacheInvoices.FowstID" & vbCrLf &
            " ,CacheInvoices.GroupName" & vbCrLf &
            " ,CacheInvoices.FranchiseGroupID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Code As FowstCode" & vbCrLf &
            " ,ISNULL(mp2.ProductName,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')   AS [ProductName]" & vbCrLf &
            " , SUM(ISNULL(dbo.TransInvoiceItems.SubTotal,0) + ISNULL(TransInvoiceItemsModifier .PremiumPrice,0) + ISNULL(TransInvoiceItemsModifier .ModifierPrice,0) )  AS [TotalSales]" & vbCrLf &
            " , Count(distinct CacheInvoices.ID) AS [TransactionCount]" & vbCrLf &
            " , ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0) AS [ProductCount]" & vbCrLf &
            "  FROM CacheInvoices" & vbCrLf &
            " INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID" & vbCrLf &
            " INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID" & vbCrLf &
            " INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID" & vbCrLf &
            " INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID" & vbCrLf &
            " INNER JOIN dbo.MobizLibraryPIMProduct mp2 ON dbo.LibraryItemModifierInventoryAffecting.ProductID = mp2.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID" & vbCrLf &
            " WHERE LibraryItemModifierInventoryAffecting.PremiumPrice=0" & vbCrLf &
            " -- and TransInvoiceItemsModifier.SubTotal>0 -- commented ni para maapil ang buy 1 take 1" & vbCrLf

        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If

        pSql &= " AND ISNULL(CONVERT(varchar(36), FranchiseGroupID), '')  <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf &
            " GROUP BY ISNULL(mp2.ProductName,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')" & vbCrLf &
            " ,CONVERT(date, CacheInvoices.InvoiceDate)" & vbCrLf &
            " ,CacheInvoices.FowstID" & vbCrLf &
            " ,CacheInvoices.GroupName" & vbCrLf &
            " ,CacheInvoices.FranchiseGroupID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Code" & vbCrLf &
            " ORDER BY ISNULL(mp2.ProductName,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')  "

        Return pSql

    End Function

    Public Function CacheInvoiceFinishedProducts(
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As Integer

        Dim pDaysOffset As Integer
        Dim pAffectedRecords As Integer

        Do
            Dim pInvoiceDateFromActual As Date?
            Dim pInvoiceDateToActual As Date?

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                If Math.Abs((vInvoiceDateFrom.Value - vInvoiceDateTo.Value).Days) = 0 Then
                    Exit Do
                End If

                pInvoiceDateFromActual = vInvoiceDateFrom.Value.AddDays(pDaysOffset)
                pInvoiceDateToActual = pInvoiceDateFromActual
            Else
                Exit Do
            End If

            pAffectedRecords += CacheInvoiceFinishedProducts(9999, pInvoiceDateFromActual, pInvoiceDateToActual)

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                pDaysOffset += 1
                If pInvoiceDateFromActual.Value >= vInvoiceDateTo.Value Then
                    Return pAffectedRecords
                End If
            Else
                Return pAffectedRecords
            End If
        Loop

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pSourceSql As String

        pSourceSql = InvoiceFinishedProductsSql(vMaxAgeInDays, vInvoiceDateFrom:=vInvoiceDateFrom, vInvoiceDateTo:=vInvoiceDateTo)

        mProData.CommandTimeout = 15 * 60
        mProData.BeginTransaction()
        If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoiceFinishedProducts', N'U')") IsNot DBNull.Value Then
            mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoiceFinishedProducts")
        End If

        pIU = New InsertUpdate("#DeletableCacheInvoiceFinishedProducts")
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("ProductID") = "ProductID"
        pIU("ProductNo") = "ProductNo"
        pIU("ProductName") = "ProductName"
        pIU("ProductBrand") = "ProductBrand"
        pIU("ProductQuantity") = "ProductQuantity"
        pIU("ProductAmount") = "ProductAmount"
        pIU("FowstID") = "FowstID"
        pIU("FowstName") = "FowstName"
        pIU("FOWSTCode") = "FOWSTCode"
        pIU("GroupName") = "GroupName"
        pIU("FranchiseGroupID") = "FranchiseGroupID"
        pIU("InvoiceCount") = "InvoiceCount"
        pIU("SalesValue") = "SalesValue"
        pIU("StoreLocation") = "StoreLocation"
        pIU("StoreLocationTypeID") = "StoreLocationTypeID"
        pIU("Category") = "Category"
        pIU("FromFranchisee") = "FromFranchisee"

        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetSelectInto()
        Dim pCurrentData = mProData.GetCurrentDatabase
        Try
            pOut = mProData.ExecuteSQL(pSql)
            'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
            '    "pSql: " & pSql & vbCrLf &
            '    "pOut: " & pOut
            'AppendToLog(pStr, , "CacheInvoicesWithCredit")
        Catch ex As Exception
            AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
            Throw
        End Try

        If mProData.TableExist("CacheInvoiceFinishedProducts") Then
            pIU.TableName = "CacheInvoiceFinishedProducts"
            pIU.FromSql = " FROM #DeletableCacheInvoiceFinishedProducts"

            pSql = pIU.GetUpdate("#DeletableCacheInvoiceFinishedProducts.InvoiceDate = CacheInvoiceFinishedProducts.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceFinishedProducts.ProductID = CacheInvoiceFinishedProducts.ProductID" & vbCrLf &
                " AND #DeletableCacheInvoiceFinishedProducts.FOWSTID = CacheInvoiceFinishedProducts.FOWSTID" & vbCrLf,
                vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceFinishedProducts.")
            pOut = mProData.ExecuteSQL(pSql)

            pIU.FromSql = " FROM #DeletableCacheInvoiceFinishedProducts" & vbCrLf &
                " WHERE NOT EXISTS (" & vbCrLf &
                "   SELECT 1" & vbCrLf &
                "   FROM CacheInvoiceFinishedProducts" & vbCrLf &
                "   WHERE #DeletableCacheInvoiceFinishedProducts.InvoiceDate = CacheInvoiceFinishedProducts.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceFinishedProducts.ProductID = CacheInvoiceFinishedProducts.ProductID" & vbCrLf &
                " AND #DeletableCacheInvoiceFinishedProducts.FOWSTID = CacheInvoiceFinishedProducts.FOWSTID" & vbCrLf &
                ")"
            pSql = pIU.GetInsert(vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceFinishedProducts.")
            pOut += mProData.ExecuteSQL(pSql)
        Else
            pSql = "SELECT * INTO CacheInvoiceFinishedProducts FROM #DeletableCacheInvoiceFinishedProducts"
            pOut += mProData.ExecuteSQL(pSql)
        End If

        mProData.CommitTransaction()

        Return pOut

    End Function

    Private Function InvoiceFinishedProductsSql(Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As String

        Dim pSql = " -- InvoiceFinishedProductsSql" & vbCrLf &
            " SELECT CONVERT(date, CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,tblPimItem.ProductNo" & vbCrLf &
            " ,tblPimItem.ProductName" & vbCrLf &
            " ,tblPimItem.Brand AS ProductBrand" & vbCrLf &
            " ,SUM(CASE WHEN TransInvoiceItems.Quantity = 0 THEN TransInvoiceItems.OrderedQuantity ELSE TransInvoiceItems.Quantity END * TransInvoiceItems.ActualQuantity) AS ProductQuantity" & vbCrLf &
            " ,SUM(CASE WHEN TransInvoiceItems.Quantity = 0 THEN TransInvoiceItems.OrderedQuantity ELSE TransInvoiceItems.Quantity END * TransInvoiceItems.UnitPrice) AS ProductAmount" & vbCrLf &
            " ,SUM(TransInvoiceItems.SubTotal) AS SalesValue" & vbCrLf &
            " ,CacheInvoices.FowstID" & vbCrLf &
            " ,CacheInvoices.FromFranchisee" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Code AS FOWSTCode" & vbCrLf &
            " ,CacheInvoices.GroupName" & vbCrLf &
            " ,CacheInvoices.FranchiseGroupID" & vbCrLf &
            " ,COUNT(CacheInvoices.ID) AS InvoiceCount" & vbCrLf &
            " ,CacheInvoices.StoreLocation" & vbCrLf &
            " ,CacheInvoices.StoreLocationTypeID" & vbCrLf &
            " ,MobizLibraryPIMProductVariableFeatures001.C0020 AS Category" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = CacheInvoices.ID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct tblPimItem ON tblPimItem.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID" & vbCrLf &
            " WHERE 1=1" & vbCrLf

        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        pSql &= " AND DATEDIFF(minute, CacheInvoices.InvoiceDate, GETDATE()) <= 60 * 24 * " & vMaxAgeInDays & " -- number of minutes in 7 days; dont compare using days" & vbCrLf

        pSql &= " GROUP BY CONVERT(date, CacheInvoices.InvoiceDate)" & vbCrLf &
            " ,tblPimItem.ProductName" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,tblPimItem.ProductNo" & vbCrLf &
            " ,tblPimItem.Brand" & vbCrLf &
            " ,CacheInvoices.FowstID" & vbCrLf &
            " ,CacheInvoices.FromFranchisee" & vbCrLf &
            " ,CacheInvoices.StoreLocation" & vbCrLf &
            " ,CacheInvoices.StoreLocationTypeID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " ,CacheInvoices.GroupName" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Code" & vbCrLf &
            " ,MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            " ,CacheInvoices.FranchiseGroupID" & vbCrLf

        Return pSql

    End Function

    Public Function CacheDailySalesPerSKUReport(
        vInvoiceDateFrom As Date,
        vInvoiceDateTo As Date,
        vOrgID As String) As Integer

        Dim pDaysOffset As Integer
        Dim pAffectedRecords As Integer

        Do
            Dim pInvoiceDateFromActual As Date
            Dim pInvoiceDateToActual As Date

            If Math.Abs((vInvoiceDateFrom - vInvoiceDateTo).Days) = 0 Then
                Exit Do
            End If

            pInvoiceDateFromActual = vInvoiceDateFrom.AddDays(pDaysOffset)
            pInvoiceDateToActual = pInvoiceDateFromActual

            pAffectedRecords += CacheDailySalesPerSKUReport(pInvoiceDateFromActual, pInvoiceDateToActual, vOrgID)

            pDaysOffset += 1
            If pInvoiceDateFromActual >= vInvoiceDateTo Then
                Return pAffectedRecords
            End If
        Loop

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pSourceSql As String
        Dim pDtFowst = mParent.LibraryOrganizationFOWST.GetByOrgID(vOrgID)

        For Each pDrFowst As DataRow In pDtFowst.Rows
            pSourceSql = DailySalesPerSKUReportSql(
                vInvoiceDateFrom:=vInvoiceDateFrom,
                vInvoiceDateTo:=vInvoiceDateTo,
                vFowstID:=CStr2(pDrFowst("ID")))

            mProData.CommandTimeout = 15 * 60
            mProData.BeginTransaction()
            If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheDailySalesPerSKUReport', N'U')") IsNot DBNull.Value Then
                mProData.ExecuteSQL("DROP TABLE #DeletableCacheDailySalesPerSKUReport")
            End If

            pIU = New InsertUpdate("#DeletableCacheDailySalesPerSKUReport")
            pIU("InvoiceDate") = "InvoiceDate"
            pIU("ProductID") = "ProductID"
            pIU("ProductNo") = "ProductNo"
            pIU("ProductName") = "ProductName"
            pIU("CategoryName") = "CategoryName"
            pIU("FowstID") = "FowstID"
            pIU("FowstName") = "FowstName"
            pIU("FOWSTCode") = "FOWSTCode"
            pIU("GroupName") = "GroupName"
            pIU("FranchiseGroupID") = "FranchiseGroupID"
            pIU("InvoiceCount") = "InvoiceCount"
            pIU("SubTotal") = "SubTotal"
            pIU("StoreLocation") = "StoreLocation"
            pIU("StoreLocationTypeID") = "StoreLocationTypeID"
            pIU("UnitMeasure") = "UnitMeasure"
            pIU("Qty") = "Qty"

            pIU.FromSql = "FROM (" & pSourceSql & ") t"
            pSql = pIU.GetSelectInto()
            Dim pCurrentData = mProData.GetCurrentDatabase
            Try
                pOut = mProData.ExecuteSQL(pSql)
                'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
                '    "pSql: " & pSql & vbCrLf &
                '    "pOut: " & pOut
                'AppendToLog(pStr, , "CacheInvoicesWithCredit")
            Catch ex As Exception
                AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
                Throw
            End Try

            If mProData.TableExist("CacheDailySalesPerSKUReport") Then
                pIU.TableName = "CacheDailySalesPerSKUReport"
                pIU.FromSql = " FROM #DeletableCacheDailySalesPerSKUReport"

                pSql = pIU.GetUpdate("#DeletableCacheDailySalesPerSKUReport.InvoiceDate = CacheDailySalesPerSKUReport.InvoiceDate" & vbCrLf &
                    " AND #DeletableCacheDailySalesPerSKUReport.ProductName = CacheDailySalesPerSKUReport.ProductName" & vbCrLf &
                    " AND #DeletableCacheDailySalesPerSKUReport.FOWSTID = CacheDailySalesPerSKUReport.FOWSTID" & vbCrLf,
                    vFromTable:=True,
                    vUseFieldNameAsValue:=True,
                    vValuePrefix:="#DeletableCacheDailySalesPerSKUReport.")
                pOut = mProData.ExecuteSQL(pSql)

                pIU.FromSql = " FROM #DeletableCacheDailySalesPerSKUReport" & vbCrLf &
                    " WHERE NOT EXISTS (" & vbCrLf &
                    "   SELECT 1" & vbCrLf &
                    "   FROM CacheDailySalesPerSKUReport" & vbCrLf &
                    "   WHERE #DeletableCacheDailySalesPerSKUReport.InvoiceDate = CacheDailySalesPerSKUReport.InvoiceDate" & vbCrLf &
                    " AND #DeletableCacheDailySalesPerSKUReport.ProductName = CacheDailySalesPerSKUReport.ProductName" & vbCrLf &
                    " AND #DeletableCacheDailySalesPerSKUReport.FOWSTID = CacheDailySalesPerSKUReport.FOWSTID" & vbCrLf &
                    ")"
                pSql = pIU.GetInsert(vFromTable:=True,
                    vUseFieldNameAsValue:=True,
                    vValuePrefix:="#DeletableCacheDailySalesPerSKUReport.")
                pOut += mProData.ExecuteSQL(pSql)
            Else
                pSql = "SELECT * INTO CacheDailySalesPerSKUReport FROM #DeletableCacheDailySalesPerSKUReport"
                pOut += mProData.ExecuteSQL(pSql)
            End If

            mProData.CommitTransaction()
        Next

        Return pOut

    End Function

    Private Function DailySalesPerSKUReportSql(vInvoiceDateFrom As Date,
        vInvoiceDateTo As Date,
        vFowstID As String) As String

        Dim pSql = " SELECT CacheInvoices.FowstID" & vbCrLf &
            "  ,COUNT(CacheInvoices.ID) AS InvoiceCount" & vbCrLf &
            ",CacheInvoices.StoreLocationTypeID" & vbCrLf &
            ",CacheInvoices.StoreLocation" & vbCrLf &
            ",CacheInvoices.GroupName" & vbCrLf &
            ",CacheInvoices.FOWSTCode" & vbCrLf &
            ",CacheInvoices.FowstName" & vbCrLf &
            ",CacheInvoices.FranchiseGroupID" & vbCrLf &
            ",convert(date,CacheInvoices.InvoiceDate) InvoiceDate  " & vbCrLf &
            " , ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')   " & vbCrLf &
            " + ' - ' + ISNULL(mp2.ProductName,'') AS [ProductName]  " & vbCrLf &
            " , dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure]  " & vbCrLf &
            " , ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0) AS [Qty]  " & vbCrLf &
            " , ISNULL(SUM(dbo.TransInvoiceItems.SubTotal),0) + ISNULL((SUM(TransInvoiceItemsModifier.PremiumPrice)),0) + ISNULL((SUM(TransInvoiceItemsModifier.ModifierPrice)),0)  AS [SubTotal]   " & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " from CacheInvoices " & vbCrLf &
            " inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
            " INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
            " INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
            " INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
            " INNER JOIN dbo.MobizLibraryPIMProduct mp2 ON dbo.LibraryItemModifierInventoryAffecting.ProductID = mp2.ID   " & vbCrLf &
            " WHERE CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND CONVERT(date, dbo.CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateSql)) & vbCrLf &
            " GROUP BY CacheInvoices.StoreLocationTypeID, CacheInvoices.StoreLocation, CacheInvoices.FranchiseGroupID, CacheInvoices.GroupName, CacheInvoices.FOWSTCode, CacheInvoices.FowstName, CacheInvoices.FowstID, CacheInvoices.FranchiseGroupID, dbo.TransInvoiceItems.UnitMeasure, ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') + ' - ' + ISNULL(mp2.ProductName,''), convert(date,CacheInvoices.InvoiceDate) " & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " HAVING ISNULL((SUM(TransInvoiceItemsModifier.PremiumPrice)),0) + ISNULL((SUM(TransInvoiceItemsModifier.ModifierPrice)),0) > 0 " & vbCrLf &
            " UNION " & vbCrLf &
            " SELECT CacheInvoices.FowstID" &
            "  ,COUNT(CacheInvoices.ID) AS InvoiceCount" & vbCrLf &
            ",CacheInvoices.StoreLocationTypeID" & vbCrLf &
            ",CacheInvoices.StoreLocation" & vbCrLf &
            ",CacheInvoices.GroupName" & vbCrLf &
            ",CacheInvoices.FOWSTCode" & vbCrLf &
            ",CacheInvoices.FowstName" & vbCrLf &
            ",CacheInvoices.FranchiseGroupID" & vbCrLf &
            ",convert(date,CacheInvoices.InvoiceDate) InvoiceDate  " & vbCrLf &
            " , ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') AS [ProductName]  " & vbCrLf &
            " , dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure]  " & vbCrLf &
            " , ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0) AS [Qty]  " & vbCrLf &
            " , ISNULL(SUM(dbo.TransInvoiceItems.SubTotal),0)  AS [SubTotal]   " & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            "  FROM CacheInvoices " & vbCrLf &
            " inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
            " INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
            " WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateSql)) & vbCrLf &
            " AND CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransInvoiceItems.ID NOT IN ( " & vbCrLf &
            "   Select TransInvoiceItems.ID " & vbCrLf &
            "   FROM CacheInvoices  " & vbCrLf &
            "   inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
            "   INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
            "   INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
            "   WHERE CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            "   AND CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateSql)) & vbCrLf &
            "   GROUP BY TransInvoiceItems.ID" & vbCrLf &
            "   HAVING ISNULL((SUM(TransInvoiceItemsModifier.PremiumPrice)),0) + ISNULL((SUM(TransInvoiceItemsModifier.ModifierPrice)),0) > 0 " & vbCrLf &
            " )GROUP BY CacheInvoices.StoreLocationTypeID, CacheInvoices.StoreLocation, CacheInvoices.FranchiseGroupID, CacheInvoices.GroupName, CacheInvoices.FOWSTCode, CacheInvoices.FowstName, CacheInvoices.FowstID, CacheInvoices.FranchiseGroupID, dbo.TransInvoiceItems.UnitMeasure, convert(date,CacheInvoices.InvoiceDate)  , ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')  " & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf

        ' commented 2024_09_03__14_29_48_2486
        '" UNION " & vbCrLf &
        '" SELECT CacheInvoices.FowstID" & vbCrLf &
        '"  ,COUNT(CacheInvoices.ID) AS InvoiceCount" & vbCrLf &
        '",CacheInvoices.StoreLocationTypeID" & vbCrLf &
        '",CacheInvoices.StoreLocation" & vbCrLf &
        '",CacheInvoices.GroupName" & vbCrLf &
        '",CacheInvoices.FOWSTCode" & vbCrLf &
        '",CacheInvoices.FowstName" & vbCrLf &
        '",CacheInvoices.FranchiseGroupID" & vbCrLf &
        '",convert(date,CacheInvoices.InvoiceDate) InvoiceDate  " & vbCrLf &
        '" , ISNULL(mp2.ProductGroupNo,'') + ' ' + ISNULL(mp2.ProductName,'') AS [ProductName] " & vbCrLf &
        '" , dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure]  " & vbCrLf &
        '" , ISNULL(SUM(dbo.TransInvoiceItemsModifier.InputQty),0) AS [Qty]  " & vbCrLf &
        '" , ISNULL((SUM(TransInvoiceItemsModifier .ModifierPrice)),0)  AS [SubTotal]   " & vbCrLf &
        '" from CacheInvoices " & vbCrLf &
        '" inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
        '" INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
        '" INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
        '" INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
        '" INNER JOIN dbo.MobizLibraryPIMProduct mp2 ON dbo.LibraryItemModifierInventoryAffecting.ProductID = mp2.ID   " & vbCrLf &
        '" WHERE CONVERT(date, dbo.CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateSql)) & vbCrLf &
        '" AND mp2.ID  IN ( " & vbCrLf &
        '    "   'fe53d817-feb1-44bc-bfe7-279c3a6380d5', " & vbCrLf &
        '    "   '374b4dac-5aa5-49f9-8143-b7ac3ca1483a', " & vbCrLf &
        '    "   'ddef9ae8-5083-44e1-a72a-fa0e27d4f56f', " & vbCrLf &
        '    "   '7c0e712a-6e94-4543-8af2-1ad909cba139' " & vbCrLf &
        '    ") " & vbCrLf &
        '" AND CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf &
        '" GROUP BY CacheInvoices.StoreLocationTypeID, CacheInvoices.StoreLocation, CacheInvoices.FranchiseGroupID, CacheInvoices.GroupName, CacheInvoices.FOWSTCode, CacheInvoices.FowstName, CacheInvoices.FowstID, CacheInvoices.FranchiseGroupID, dbo.TransInvoiceItems.UnitMeasure, ISNULL(mp2.ProductGroupNo,'') + ' ' + ISNULL(mp2.ProductName,''), convert(date,CacheInvoices.InvoiceDate) "

        pSql = "SELECT t.*" & vbCrLf &
            ",tblCategory.CategoryName" & vbCrLf &
            " FROM (" & pSql & vbCrLf &
            ") t" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 C0020 AS CategoryName" & vbCrLf &
            "     FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
            "     WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = t.ProductID" & vbCrLf &
            "  ) tblCategory" & vbCrLf

        Return pSql

    End Function

    Public Function CacheInvoiceProductSizes(
        Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As Integer

        Dim pDaysOffset As Integer
        Dim pAffectedRecords As Integer
        Do
            Dim pInvoiceDateFromActual As Date?
            Dim pInvoiceDateToActual As Date?

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                If Math.Abs((vInvoiceDateFrom.Value - vInvoiceDateTo.Value).Days) = 0 Then
                    Exit Do
                End If

                pInvoiceDateFromActual = vInvoiceDateFrom.Value.AddDays(pDaysOffset)
                pInvoiceDateToActual = pInvoiceDateFromActual
            Else
                Exit Do
            End If

            pAffectedRecords += CacheInvoiceProductSizes(9999, pInvoiceDateFromActual, pInvoiceDateToActual)

            If vInvoiceDateFrom IsNot Nothing _
                AndAlso vInvoiceDateTo IsNot Nothing Then

                pDaysOffset += 1
                If pInvoiceDateFromActual.Value >= vInvoiceDateTo.Value Then
                    Return pAffectedRecords
                End If
            Else
                Return pAffectedRecords
            End If
        Loop

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pSourceSql As String

        pSourceSql = InvoiceProductSizesSql(vMaxAgeInDays, vInvoiceDateFrom:=vInvoiceDateFrom, vInvoiceDateTo:=vInvoiceDateTo)

        mProData.CommandTimeout = 5 * 60
        mProData.BeginTransaction()
        If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoiceProductSizes', N'U')") IsNot DBNull.Value Then
            mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoiceProductSizes")
        End If

        pIU = New InsertUpdate("#DeletableCacheInvoiceProductSizes")
        pIU("InvoiceDate") = "InvoiceDate"
        pIU("Size") = "Size"
        pIU("ProductQuantity") = "ProductQuantity"
        pIU("ProductAmount") = "ProductAmount"
        pIU("FowstID") = "FowstID"
        pIU("FowstName") = "FowstName"
        pIU("GroupName") = "GroupName"
        pIU("FranchiseGroupID") = "FranchiseGroupID"
        pIU("InvoiceCount") = "InvoiceCount"
        pIU("SalesValue") = "SalesValue"

        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetSelectInto()
        Dim pCurrentData = mProData.GetCurrentDatabase
        Try
            pOut = mProData.ExecuteSQL(pSql)
            'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
            '    "pSql: " & pSql & vbCrLf &
            '    "pOut: " & pOut
            'AppendToLog(pStr, , "CacheInvoicesWithCredit")
        Catch ex As Exception
            AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
            Throw
        End Try

        If mProData.TableExist("CacheInvoiceProductSizes") Then
            pIU.TableName = "CacheInvoiceProductSizes"
            pIU.FromSql = " FROM #DeletableCacheInvoiceProductSizes"

            pSql = pIU.GetUpdate("#DeletableCacheInvoiceProductSizes.InvoiceDate = CacheInvoiceProductSizes.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceProductSizes.Size = CacheInvoiceProductSizes.Size" & vbCrLf &
                " AND #DeletableCacheInvoiceProductSizes.FOWSTID = CacheInvoiceProductSizes.FOWSTID" & vbCrLf,
                vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceProductSizes.")
            pOut = mProData.ExecuteSQL(pSql)

            pIU.FromSql = " FROM #DeletableCacheInvoiceProductSizes" & vbCrLf &
                " WHERE NOT EXISTS (" & vbCrLf &
                "   SELECT 1" & vbCrLf &
                "   FROM CacheInvoiceProductSizes" & vbCrLf &
                "   WHERE #DeletableCacheInvoiceProductSizes.InvoiceDate = CacheInvoiceProductSizes.InvoiceDate" & vbCrLf &
                " AND #DeletableCacheInvoiceProductSizes.Size = CacheInvoiceProductSizes.Size" & vbCrLf &
                " AND #DeletableCacheInvoiceProductSizes.FOWSTID = CacheInvoiceProductSizes.FOWSTID" & vbCrLf &
                ")"
            pSql = pIU.GetInsert(vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoiceProductSizes.")
            pOut += mProData.ExecuteSQL(pSql)
        Else
            pSql = "SELECT * INTO CacheInvoiceProductSizes FROM #DeletableCacheInvoiceProductSizes"
            pOut += mProData.ExecuteSQL(pSql)
        End If

        mProData.CommitTransaction()

        Return pOut

    End Function

    Private Function InvoiceProductSizesSql(Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As String

        Dim pSql = "-- InvoiceProductSizesSql" & vbCrLf &
            "  SELECT CONVERT(date, CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            " , MobizLibraryPIMProductVariableFeatures001.C0036  AS Size" & vbCrLf &
            "  ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS ProductQuantity" & vbCrLf &
            "  ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.UnitPrice) AS ProductAmount" & vbCrLf &
            "  ,SUM(TransInvoiceItems.SubTotal) AS SalesValue" & vbCrLf &
            "  ,CacheInvoices.FowstID" & vbCrLf &
            "  ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            "  ,CacheInvoices.GroupName" & vbCrLf &
            "  ,CacheInvoices.FranchiseGroupID" & vbCrLf &
            "  ,COUNT(CacheInvoices.ID) AS InvoiceCount" & vbCrLf &
            "  FROM CacheInvoices" & vbCrLf &
            "  JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = CacheInvoices.ID" & vbCrLf &
            "  JOIN MobizLibraryPIMProduct tblPimItem ON tblPimItem.ID = TransInvoiceItems.ProductID" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID" & vbCrLf &
            " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "  WHERE 1=1" & vbCrLf

        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        pSql &= " AND DATEDIFF(minute, CacheInvoices.InvoiceDate, GETDATE()) <= 60 * 24 * " & vMaxAgeInDays & " -- number of minutes in 7 days; dont compare using days" & vbCrLf

        pSql &= " AND MobizLibraryPIMProductVariableFeatures001.C0036 <> 'N/A'" & vbCrLf &
            " GROUP BY CONVERT(date, CacheInvoices.InvoiceDate)" & vbCrLf &
            ",MobizLibraryPIMProductVariableFeatures001.C0036" & vbCrLf &
            ",CacheInvoices.FowstID" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            ",CacheInvoices.GroupName" & vbCrLf &
            ",CacheInvoices.FranchiseGroupID"

        Return pSql

    End Function

    Public Function CustomersWithCreditSummary() As DataRow

        Dim pOut As DataRow
        Dim pSql = CustomersWithCreditSql("")

        pSql = "SELECT COUNT(*) AS Count" & vbCrLf &
            ",SUM(NetInvoiceBalance) AS NetInvoiceBalance" & vbCrLf &
            " FROM (" & pSql & ") t"
        pOut = mProData.ExecuteRow(pSql)

        Return pOut

    End Function

    Public Function CacheInvoiceDaily(Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing,
        Optional vGroupByHour As Boolean = False,
        Optional vGroupByMonth As Boolean = False,
        Optional rProgress As Action(Of String) = Nothing,
        Optional vFilterByDateCreated As Boolean = False,
        Optional vFowstID As String = Nothing) As Integer

        Dim pDaysOffset As Integer ' or month offset if vGroupByMonth
        Dim pAffectedRecords As Integer
        Dim pMsg As String

        If vGroupByMonth Then
            If vInvoiceDateFrom Is Nothing Then
                pMsg = "vInvoiceDateFrom is required if vGroupByMonth = true. ref: vb7P59M7LYP6!"
                rProgress?.Invoke(pMsg)
                Throw New ApplicationException(pMsg)
            End If
            If vInvoiceDateTo Is Nothing Then
                pMsg = "vInvoiceDateTo is required if vGroupByMonth = true. ref: vbFBLSMVTUU6!"
                rProgress?.Invoke(pMsg)
                Throw New ApplicationException(pMsg)
            End If
            If vInvoiceDateTo < vInvoiceDateFrom Then
                pMsg = "vInvoiceDateFrom must not be later than vInvoiceDateTo. ref: vbL9WZBL2946!"
                rProgress?.Invoke(pMsg)
                Throw New ApplicationException(pMsg)
            End If

            Dim pInvoiceDateFromActual
            Dim pInvoiceDateToActual

            Do
                If vInvoiceDateFrom.Value.Month = vInvoiceDateTo.Value.Month _
                    AndAlso vInvoiceDateFrom.Value.Year = vInvoiceDateTo.Value.Year Then

                    '  date range is from within the same month
                    Exit Do
                End If

                If pDaysOffset = 0 Then
                    pInvoiceDateFromActual = vInvoiceDateFrom
                Else
                    pInvoiceDateFromActual = New Date(vInvoiceDateFrom.Value.Year, vInvoiceDateFrom.Value.Month + pDaysOffset, 1)
                End If

                Dim pDateRange = DateRange.GetMonth(pInvoiceDateFromActual)
                If pDateRange.DateTo < vInvoiceDateTo Then
                    pInvoiceDateToActual = pDateRange.DateTo
                Else
                    pInvoiceDateToActual = vInvoiceDateTo
                End If

                pAffectedRecords += CacheInvoiceDaily(9999, pInvoiceDateFromActual, pInvoiceDateToActual, False, vGroupByMonth,
                    rProgress:=rProgress,
                    vFilterByDateCreated:=vFilterByDateCreated,
                    vFowstID:=vFowstID)

                If pInvoiceDateToActual = vInvoiceDateTo Then
                    Return pAffectedRecords
                End If
                pDaysOffset += 1
            Loop
        Else
            Do
                Dim pInvoiceDateFromActual As Date?
                Dim pInvoiceDateToActual As Date?

                If vInvoiceDateFrom IsNot Nothing _
                    AndAlso vInvoiceDateTo IsNot Nothing Then

                    If Math.Abs((vInvoiceDateFrom.Value - vInvoiceDateTo.Value).Days) = 0 Then
                        Exit Do
                    End If

                    pInvoiceDateFromActual = vInvoiceDateFrom.Value.AddDays(pDaysOffset)
                    pInvoiceDateToActual = pInvoiceDateFromActual
                Else
                    Exit Do
                End If

                pAffectedRecords += CacheInvoiceDaily(9999, pInvoiceDateFromActual, pInvoiceDateToActual, vGroupByHour, vGroupByMonth,
                    rProgress:=rProgress,
                    vFilterByDateCreated:=vFilterByDateCreated,
                    vFowstID:=vFowstID)

                If vInvoiceDateFrom IsNot Nothing _
                    AndAlso vInvoiceDateTo IsNot Nothing Then

                    pDaysOffset += 1
                    If pInvoiceDateFromActual.Value >= vInvoiceDateTo.Value Then
                        Return pAffectedRecords
                    End If
                Else
                    Return pAffectedRecords
                End If
            Loop
        End If

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As Integer
        Dim pDateFilterField = "InvoiceDate"
        If vFilterByDateCreated Then
            pDateFilterField = "DateCreated"
        End If
        pSql = "SELECT DISTINCT FowstID AS ID" & vbCrLf &
            ",CONVERT(date, InvoiceDate) AS InvoiceDate" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE 1=1" & vbCrLf
        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices." & pDateFilterField & ") >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices." & pDateFilterField & ") <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vFowstID) = False Then
            pSql &= " AND CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf
        End If

        'pSql = "SELECT DISTINCT FowstID AS ID" & vbCrLf &
        '    " FROM CacheInvoices" & vbCrLf &
        '    " WHERE 1=1" & vbCrLf
        'If vInvoiceDateFrom IsNot Nothing Then
        '    pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        'End If
        'If vInvoiceDateTo IsNot Nothing Then
        '    pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        'End If

        mProData.CommandTimeout = 15 * 60
        rProgress?.Invoke("Getting unqiue fowsts. " & pDateFilterField & " = " & vInvoiceDateTo.Value.Date.ToString(cGlobal.FormatDate) & ". vb8TY7WNLPNZ!")
#If DEBUG Then
        'Threading.Thread.Sleep(10000)
#End If
        Dim pDtFowst = mProData.Execute(pSql)

        For Each pDrFowst As DataRow In pDtFowst.Rows
            rProgress?.Invoke("Processing invoice date = " & CDateNull(pDrFowst("InvoiceDate"))?.ToString(cGlobal.FormatDate) & ". FowstID = " & CStr2(pDrFowst("ID")) & ". ref: vbEW3BW48SHR!")
            Dim pSourceSql = CacheInvoicesDailySql(vMaxAgeInDays, pDrFowst("InvoiceDate"), pDrFowst("InvoiceDate"), vGroupByHour, vGroupByMonth,
                vFowstID:=CStr2(pDrFowst("ID")))

            mProData.BeginTransaction()
            If mProData.ExecuteScalar("SELECT OBJECT_ID(N'tempdb..#DeletableCacheInvoicesDaily', N'U')") IsNot DBNull.Value Then
                rProgress?.Invoke("DROP TABLE #DeletableCacheInvoicesDaily")
                mProData.ExecuteSQL("DROP TABLE #DeletableCacheInvoicesDaily")
            End If

            pSql = pSourceSql.Cte & "SELECT *" & vbCrLf &
                " INTO #DeletableCacheInvoicesDaily" & vbCrLf &
                "FROM (" & pSourceSql.Sql & ") t"
            Dim pCurrentData = mProData.GetCurrentDatabase
            Try
                rProgress?.Invoke("SELECT INTO #DeletableCacheInvoicesDaily")
                pOut += mProData.ExecuteSQL(pSql)
                'pStr = "vb3VZ5BUT6FG! pSourceSql: " & pSourceSql & vbCrLf &
                '    "pSql: " & pSql & vbCrLf &
                '    "pOut: " & pOut
                'AppendToLog(pStr, , "CacheInvoicesWithCredit")
            Catch ex As Exception
                AppendToDebug(ex.Message & vbCrLf & "Database: " & pCurrentData & vbCrLf & pSql)
                Throw
            End Try

            pIU = New InsertUpdate()
            pSql = "SELECT TOP 0 * FROM #DeletableCacheInvoicesDaily"
            Dim pDtSchema = mProData.GetSchema(pSql)
            For Each pDrSchema As DataRow In pDtSchema.Rows
                pIU(EscapeSqlName(pDrSchema("ColumnName"))) = EscapeSqlName(pDrSchema("ColumnName"))
            Next

            If vGroupByHour Then
                pIU.TableName = "CacheInvoicesHourly"
            ElseIf vGroupByMonth Then
                pIU.TableName = "CacheInvoicesMonthly"
            Else
                pIU.TableName = "CacheInvoicesDaily"
            End If
            pIU.FromSql = " FROM #DeletableCacheInvoicesDaily"
            pSql = " #DeletableCacheInvoicesDaily.FowstID = " & pIU.TableName & ".FowstID" & vbCrLf
            If vGroupByMonth = False Then
                pSql &= " AND #DeletableCacheInvoicesDaily.InvoiceDate = " & pIU.TableName & ".InvoiceDate" & vbCrLf
            End If
            If vGroupByHour Then
                pSql &= " AND #DeletableCacheInvoicesDaily.InvoiceHour = " & pIU.TableName & ".InvoiceHour" & vbCrLf
            ElseIf vGroupByMonth Then
                pSql &= " AND #DeletableCacheInvoicesDaily.InvoiceMonth = " & pIU.TableName & ".InvoiceMonth" & vbCrLf
                pSql &= " AND #DeletableCacheInvoicesDaily.InvoiceYear = " & pIU.TableName & ".InvoiceYear" & vbCrLf
            End If
            pSql = pIU.GetUpdate(pSql,
                vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoicesDaily.")
            Dim pDeletableCount = mProData.ExecuteScalar("SELECT COUNT(*) FROM #DeletableCacheInvoicesDaily")
            pSql = "-- rows in #DeletableCacheInvoicesDaily: " & pDeletableCount & vbCrLf & pSql
            pOut += mProData.ExecuteSQL(pSql)

            pIU.FromSql = " FROM #DeletableCacheInvoicesDaily" & vbCrLf &
                " WHERE NOT EXISTS (" & vbCrLf &
                "   SELECT 1" & vbCrLf &
                "   FROM " & pIU.TableName & vbCrLf &
                "   WHERE " & pIU.TableName & ".FowstID = #DeletableCacheInvoicesDaily.FowstID" & vbCrLf
            If vGroupByMonth = False Then
                pIU.FromSql &= "   AND " & pIU.TableName & ".InvoiceDate = #DeletableCacheInvoicesDaily.InvoiceDate" & vbCrLf
            End If
            If vGroupByHour Then
                pIU.FromSql &= " AND #DeletableCacheInvoicesDaily.InvoiceHour = " & pIU.TableName & ".InvoiceHour" & vbCrLf
            ElseIf vGroupByMonth Then
                pIU.FromSql &= " AND #DeletableCacheInvoicesDaily.InvoiceMonth = " & pIU.TableName & ".InvoiceMonth" & vbCrLf
                pIU.FromSql &= " AND #DeletableCacheInvoicesDaily.InvoiceYear = " & pIU.TableName & ".InvoiceYear" & vbCrLf
            End If
            pIU.FromSql &= ")"
            rProgress?.Invoke("INSERT INTO " & pIU.TableName)
            pSql = pIU.GetInsert(vFromTable:=True,
                vUseFieldNameAsValue:=True,
                vValuePrefix:="#DeletableCacheInvoicesDaily.")
            pOut += mProData.ExecuteSQL(pSql)

            mProData.CommitTransaction()
        Next pDrFowst

        Return pOut

    End Function

    Class CacheInvoicesDailySqlOutput
        Public Cte As String
        Public Sql As String
    End Class

    Private Function CacheInvoicesDailySql(Optional vMaxAgeInDays As Integer = 7,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing,
        Optional vGroupByHour As Boolean = False,
        Optional vGroupByMonth As Boolean = False,
        Optional vFowstID As String = Nothing) As CacheInvoicesDailySqlOutput

        If String.IsNullOrWhiteSpace(vFowstID) Then
            Throw New ApplicationException("vFowstID is required. vbVVASSL284V!")
        End If

        Dim pOut As New CacheInvoicesDailySqlOutput

        Dim pSql = "WITH CTE AS (" & vbCrLf &
            " SELECT *" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf
        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) >= " & ReadyString(Format(vInvoiceDateFrom.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(date, CacheInvoices.InvoiceDate) <= " & ReadyString(Format(vInvoiceDateTo.Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        pSql &= " AND DATEDIFF(day, CONVERT(date, CacheInvoices.InvoiceDate), CONVERT(date, GETDATE())) <= " & vMaxAgeInDays & vbCrLf
        pSql &= ") " & vbCrLf
        pOut.Cte = pSql

        Dim pSelectSql As New List(Of String)
        Dim pGroupBySql As New List(Of String)
        Dim pGroupBy = New Dictionary(Of String, String)
        pGroupBy.Add("FowstID", "FowstID")
        pGroupBy.Add("FranchiseGroupID", "FranchiseGroupID")
        pGroupBy.Add("SubFOWSTTypeID", "SubFOWSTTypeID")
        pGroupBy.Add("StoreLocationTypeID", "StoreLocationTypeID")
        pGroupBy.Add("RegionID", "RegionID")
        pGroupBy.Add("FOWSTCode", "FOWSTCode")
        pGroupBy.Add("StoreType", "StoreType")
        pGroupBy.Add("StoreLocation", "StoreLocation")
        pGroupBy.Add("GroupName", "GroupName")
        pGroupBy.Add("RegionName", "RegionName")
        pGroupBy.Add("FOWSTName", "FOWSTName")
        pGroupBy.Add("FromFranchisee", "ISNULL(FromFranchisee, 0)")
        If vGroupByMonth Then
        Else
            pGroupBy.Add("InvoiceDate", "CONVERT(date, InvoiceDate)")
        End If

        Dim pDtSchema = mProData.GetTableSchema("CacheInvoices")
        For Each pDrSchema As DataRow In pDtSchema.Rows
            Dim pColumnName = CStr2(pDrSchema("column_name"))
            Dim pInGroupBy = pGroupBy.Where(Function(x) x.Key.ToLower() = pColumnName.ToLower())
            If pInGroupBy.Any() Then
                pSelectSql.Add(pInGroupBy.First().Value & " AS " & pInGroupBy.First().Key)
                pGroupBySql.Add(pInGroupBy.First().Value)
                Continue For
            End If
            Dim pDataType = CStr2(pDrSchema("data_type")).ToLower()
            If pDataType <> "money" _
                AndAlso pDataType <> "int" _
                AndAlso pDataType <> "decimal" _
                AndAlso pDataType <> "numeric" Then

                Continue For
            End If
            pSelectSql.Add("SUM(" & EscapeSqlName(pColumnName) & ") AS " & EscapeSqlName(pColumnName))
        Next
        Dim pSql2 = "SUBSTRING(" & vbCrLf &
            "STUFF((" & vbCrLf &
            " 	SELECT TOP 100 PERCENT '; ' + updatedbyname" & vbCrLf &
            "     from cte cte2" & vbCrLf &
            "     WHERE cte2.FowstID = cte.FowstID" & vbCrLf
        If vGroupByMonth Then
            pSql2 &= " AND DATEPART(MONTH, cte2.InvoiceDate) = DATEPART(MONTH, cte.InvoiceDate)" & vbCrLf &
                " AND DATEPART(YEAR, cte2.InvoiceDate) = DATEPART(YEAR, cte.InvoiceDate)" & vbCrLf
        Else
            pSql2 &= " AND CONVERT(date, cte2.InvoiceDate) = CONVERT(date, cte.InvoiceDate)" & vbCrLf
        End If
        pSql2 &= "     group by updatedbyname" & vbCrLf &
            "     order by updatedbyname" & vbCrLf &
            "     FOR XML PATH('')" & vbCrLf &
            " ), 1,1,'')" & vbCrLf &
            ", 1, 8000) AS UpdatedByName"
        pSelectSql.Add(pSql2)

        Dim pCount = 0
        pSql = "SELECT "
        For Each pItem In pSelectSql
            pCount += 1
            If pCount > 1 Then
                pSql &= ","
            End If
            pSql &= pItem & vbCrLf
        Next
        pSql &= " ,SUM(CASE WHEN tblCustomerPaymentOther.InvoiceID IS NULL THEN 1 ELSE 0 END * GrossInvoiceAmount) AS WalkInSales" & vbCrLf &
            " ,SUM(CASE WHEN tblCustomerPaymentOther.InvoiceID IS NULL THEN 0 ELSE 1 END * GrossInvoiceAmount) AS DeliverySales" & vbCrLf &
            " ,COUNT(DISTINCT CTE.ID) AS TransactionCount" & vbCrLf &
            ",SUM(CASE WHEN tblCustomerPaymentOther.InvoiceID IS NULL THEN 1 ELSE 0 END) AS WalkInTransactionCount" & vbCrLf &
            ",SUM(CASE WHEN tblCustomerPaymentOther.InvoiceID IS NULL THEN 0 ELSE 1 END) AS DeliveryTransactionCount" & vbCrLf
        If vGroupByHour Then
            pSql &= ",DATEPART(HOUR, CTE.InvoiceDate) AS InvoiceHour" & vbCrLf
        ElseIf vGroupByMonth Then
            pSql &= ",DATEPART(MONTH, CTE.InvoiceDate) AS InvoiceMonth" & vbCrLf
            pSql &= ",DATEPART(YEAR, CTE.InvoiceDate) AS InvoiceYear" & vbCrLf
        End If

        pSql &= "  FROM CTE" & vbCrLf &
            "   LEFT JOIN (" & vbCrLf &
            "  	SELECT DISTINCT TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            "     FROM TransCustomerPaymentInvoices" & vbCrLf &
            "   	LEFT JOIN TransCustomerPaymentOther ON TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            "   ) tblCustomerPaymentOther ON tblCustomerPaymentOther.InvoiceID = CTE.ID" & vbCrLf &
            "   	AND (" & vbCrLf &
            "       ISNULL(LocalDelivery , 0) > 0" & vbCrLf &
            "        OR ISNULL(GrabFood, 0) > 0" & vbCrLf &
            "        OR ISNULL(FoodPanda, 0) > 0" & vbCrLf &
            "        OR ISNULL(Pickaroo, 0) > 0" & vbCrLf &
            "        OR ISNULL(Riderko,0) > 0" & vbCrLf &
            "     )" & vbCrLf

        pSql &= " GROUP BY "
        pCount = 0
        For Each pItem In pGroupBySql
            pCount += 1
            If pCount > 1 Then
                pSql &= ","
            End If
            pSql &= pItem & vbCrLf
        Next
        If vGroupByHour Then
            pSql &= ",DATEPART(HOUR, CTE.InvoiceDate)" & vbCrLf
        ElseIf vGroupByMonth Then
            pSql &= ",DATEPART(MONTH, CTE.InvoiceDate)" & vbCrLf
            pSql &= ",DATEPART(YEAR, CTE.InvoiceDate)" & vbCrLf
        End If
        pOut.Sql = pSql
        Return pOut

    End Function

    Private Function CustomersWithCreditSql(vFilter As String, Optional vWhere As String = Nothing) As String

        Dim pSql As String
        Dim pWhere As String

        If String.IsNullOrWhiteSpace(vFilter) Then
            pWhere = "  WHERE t.NetInvoiceBalance > 0"
        ElseIf vFilter = "withoutbalance" Then
            pWhere = "  WHERE t.NetInvoiceBalance = 0"
        ElseIf vFilter = "all" Then
            ' no filter
        ElseIf vFilter = "custom" Then
            pWhere = vWhere
        Else
            Throw New ApplicationException("Unknown filter: " & vFilter & ". ref: vbKKTLXVVHVB!")
        End If

        pSql = "  -- customers with credit" & vbCrLf &
            " SELECT DISTINCT TOP 100 PERCENT MobizLibraryOrganization.ID" & vbCrLf &
            "  ,CASE WHEN MobizLibraryOrganization.CompanyName = '' THEN ISNULL(MobizLibraryOrganization.PersonFirstName + ' ', '') + ISNULL(MobizLibraryOrganization.PersonSurname + ' ', '') ELSE MobizLibraryOrganization.CompanyName END as CustomerName" & vbCrLf &
            "  ,MobizLibraryOrganization.IsOrganization" & vbCrLf &
            "  ,SUM(t.GrossInvoiceAmount) AS TotalInvoiceAmount" & vbCrLf &
            "  ,SUM(t.CashAmount) AS CashAmount" & vbCrLf &
            "  ,SUM(t.CheckPaymentAmount) AS CheckPaymentAmount" & vbCrLf &
            "  ,SUM(t.CreditCardAmount) AS CreditCardAmount" & vbCrLf &
            "  ,SUM(t.DebitCardAmount) AS DebitCardAmount" & vbCrLf &
            "  ,SUM(t.SRVAmount) AS SRVAmount" & vbCrLf &
            "  ,SUM(t.OtherTypeAmount) AS OtherTypeAmount" & vbCrLf &
            "  ,SUM(t.NetInvoiceBalance) AS NetInvoiceBalance" & vbCrLf &
            "  ,SUM(t.WeightedAverageDaysOverdue) AS WeightedAverageDaysOverdue" & vbCrLf &
            "  ,CreditLimit" & vbCrLf &
            "  ,LibraryPaymentTerms.Name AS CreditTerms" & vbCrLf &
            "  ,MobizLibraryIndustryClass.Class AS BusinessIndustry" & vbCrLf &
            "  , (SELECT TOP 1 ISNULL(CASE WHEN orgwebsite.Website = '' THEN '' ELSE orgwebsite.Website END,'') " & vbCrLf &
            "       FROM LibraryOrganizationContactWebsite as orgwebsite" & vbCrLf &
            "       WHERE(orgwebsite.OrganizationID = MobizLibraryOrganization.ID)) as Website " & vbCrLf &
            "  , (SELECT TOP 1 ISNULL(CASE WHEN orgemail.EmailAddress = '' THEN '' ELSE orgemail.EmailAddress END,'') " & vbCrLf &
            "       FROM LibraryOrganizationContactEmail as orgemail" & vbCrLf &
            "       WHERE(orgemail.OrganizationID = MobizLibraryOrganization.ID)) AS Email, " & vbCrLf &
            "   (SELECT TOP 1 " & vbCrLf &
            "       ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode) + ' ',0) + " & vbCrLf &
            "       ISNULL(' (' + CONVERT(varchar, orgareacode.CallingCode) + ') ',0) + " & vbCrLf &
            "       ISNULL(CONVERT(varchar, orgtel.ContactNo),0) + " & vbCrLf &
            "       ISNULL(CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE ',' + orgtel.ExtensionNo END,0) " & vbCrLf &
            "       FROM LibraryOrganizationContactTel as orgtel " & vbCrLf &
            "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
            "               ON orgtel.CountryID = orgcountries.ID " & vbCrLf &
            "           LEFT JOIN MobizLibraryAreaCodes as orgareacode " & vbCrLf &
            "               ON orgtel.AreaCodeID = orgareacode.ID " & vbCrLf &
            "           WHERE orgtel.OrganizationID = MobizLibraryOrganization.ID ) AS Telephone, " & vbCrLf &
            "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
            "       CASE WHEN LEN(orgmobile.ContactNo) <= 7 THEN CONVERT(varchar, orgtelco.Prefix) ELSE '' END + " & vbCrLf &
            "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
            "       FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
            "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
            "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
            "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
            "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
            "           WHERE orgmobile.OrganizationID = MobizLibraryOrganization.ID) AS Mobile " & vbCrLf &
            "  FROM MobizLibraryOrganization" & vbCrLf &
            "  JOIN (" & vbCrLf &
            "  	SELECT CacheInvoices.ID" & vbCrLf &
            "     ,CacheInvoices.CustomerID" & vbCrLf &
            "     ,GrossInvoiceAmount" & vbCrLf &
            "     ,CashAmount" & vbCrLf &
            "     ,CheckPaymentAmount" & vbCrLf &
            "     ,CreditCardAmount" & vbCrLf &
            "     ,DebitCardAmount" & vbCrLf &
            "     ,SRVAmount" & vbCrLf &
            "     ,OtherTypeAmount" & vbCrLf &
            "     ,NetInvoiceBalance" & vbCrLf &
            "     ,WeightedAverageDaysOverdue" & vbCrLf &
            "     ,NULL as CreditLimit" & vbCrLf &
            "     ,NULL as CreditTermsID" & vbCrLf &
            "  	FROM CacheInvoices" & vbCrLf &
            "     UNION" & vbCrLf &
            "     SELECT NULL AS ID" & vbCrLf &
            "     ,LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CreditBegBalance AS GrossInvoiceAmount" & vbCrLf &
            "     ,0 AS CashAmount" & vbCrLf &
            "     ,0 AS CheckPaymentAmount" & vbCrLf &
            "     ,0 AS CreditCardAmount" & vbCrLf &
            "     ,0 AS DebitCardAmount" & vbCrLf &
            "     ,0 AS SRVAmount" & vbCrLf &
            "     ,0 AS OtherTypeAmount" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CreditBegBalance AS NetInvoiceBalance" & vbCrLf &
            "     ,0 AS WeightedAverageDaysOverdue" & vbCrLf &
            "     ,CreditLimit" & vbCrLf &
            "     ,CreditTermsID" & vbCrLf &
            "     FROM LibraryOrganizationCustomer" & vbCrLf &
            "     WHERE LibraryOrganizationCustomer.CreditBegBalance <> 0" & vbCrLf &
            " )  t ON t.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
            " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = t.CreditTermsID " & vbCrLf &
            " LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = MobizLibraryOrganization.IndustryClassID " & vbCrLf &
            pWhere & vbCrLf &
            "  GROUP BY MobizLibraryOrganization.ID" & vbCrLf &
            "  , MobizLibraryOrganization.CompanyName" & vbCrLf &
            "  , MobizLibraryOrganization.PersonFirstName" & vbCrLf &
            "  , MobizLibraryOrganization.PersonSurname" & vbCrLf &
            "  , MobizLibraryOrganization.IsOrganization" & vbCrLf &
            "  , CreditLimit" & vbCrLf &
            "  , LibraryPaymentTerms.Name" & vbCrLf &
            "  , MobizLibraryIndustryClass.Class" & vbCrLf &
            "  ORDER BY SUM(t.NetInvoiceBalance) DESC, CustomerName "

        Return pSql

    End Function

    Public Function InvoicesWithCredit(vCustomerID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = InvoicesWithCreditSql()
        pSql &= " AND TransInvoice.CustomerID = " & ReadyString(vCustomerID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetInvoicesWithCreditFromCache(Optional vCustomerID As String = Nothing,
        Optional ByVal vMaxRecords As Integer = 0,
        Optional vInvoiceID As String = Nothing,
        Optional vIncludeHasPayable As Boolean = True,
        Optional vIncludeFullyPaid As Boolean = False,
        Optional vPdcArePayments As Boolean = False,
        Optional vEarliestDate As Date? = Nothing) As DataTable

        Dim pOut As DataTable
        Dim pSql As String
        Dim pSqlWhere As String
        Dim pAvailableCreditLimitComputation As String

        If vMaxRecords <> 0 Then
            pSql = " TOP " & vMaxRecords
        End If
        If CStr2(vInvoiceID).Trim() <> "" Then
            pSqlWhere &= " AND ID = " & ReadyString(vInvoiceID) & vbCrLf
        End If
        If CStr2(vCustomerID).Trim() <> "" Then
            pSqlWhere &= " AND CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        End If
        If vIncludeHasPayable And vIncludeFullyPaid Then
        ElseIf vIncludeHasPayable Then
            If vPdcArePayments Then
                pAvailableCreditLimitComputation = ""
            Else
                pAvailableCreditLimitComputation = mParent.SystemDefaults.AvailableCreditLimitComputation
            End If
            If pAvailableCreditLimitComputation = "NoPdc" Then
                pSqlWhere &= " AND (NetInvoiceBalance > 0 OR ISNULL(CheckPaymentAmountPdc, 0) > 0)"
            Else
                pSqlWhere &= " AND NetInvoiceBalance > 0"
            End If
        ElseIf vIncludeFullyPaid Then
            pSqlWhere &= " AND NetInvoiceBalance <= 0"
        End If
        If vEarliestDate IsNot Nothing Then
            pSqlWhere &= " AND CONVERT(date, InvoiceDate) > " & ReadyString(Format(vEarliestDate, cGlobal.FormatDateTimeSql)) & vbCrLf
        End If
        pSql = "SELECT" & pSql & " * " & vbCrLf &
            ",DATEDIFF(day, CONVERT(date, InvoiceDueDate), CONVERT(date, GETDATE())) + 1 AS ArAge" & vbCrLf &
            ", (SELECT COUNT(files.ID) FROM TransInvoiceFiles as files WHERE files.InvoiceID = CacheInvoices.ID) as NoOfAttachments " & vbCrLf &
            " -- , (SELECT COUNT(ID) FROM TransCRM WHERE Notes LIKE '%'+ InvoiceNumber +'%') NoOfNotes " & vbCrLf &
            ", 1 AS NoOfNotes " & vbCrLf &
            ", (SELECT ISNULL(IsAttachedOnEarlierSOA, 0) FROM TransInvoice WHERE ID = CacheInvoices.ID) as IsAttachedOnEarlierSOA " & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " OUTER APPLY (SELECT ISNULL(SUM(d.AmountPaid), 0) as APPPendingApproval FROM TransCustomerPaymentInvoicesDraft as d WHERE InvoiceID = CacheInvoices.ID) as PaymentPendingApproval " & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblOrder.SalesOrderNo) AS FullSalesOrderNum" & vbCrLf &
            "     ,tblOrder.SalesOrderDate" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            "     JOIN TransInvoice tblOrder ON tblOrder.ID = TransInvoice.SalesOrderID" & vbCrLf &
            " 	LEFT JOIN MobizLibraryOrganizationFOWST ON tblOrder.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " 	LEFT JOIN LibraryDevice ON tblOrder.DeviceID = LibraryDevice.ID" & vbCrLf &
            "     WHERE TransInvoice.ID = CacheInvoices.ID" & vbCrLf &
            "  ) tblSalesOrder" & vbCrLf &
            " WHERE 1=1 " & vbCrLf &
            pSqlWhere & vbCrLf &
            " ORDER BY ISNULL(InvoiceDueDate, InvoiceDate)"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function CacheWeightedAverageDaysOverdue(Optional vCustomerID As String = Nothing)

        Dim pDtAll = GetInvoicesWithCreditFromCache(vCustomerID)
        mProData.BeginTransaction()
        Dim pSql = "SELECT TOP 1 ID FROM CacheInvoices WITH (TABLOCKX)"
        mProData.ExecuteSQL(pSql)
        Dim pDtCustomer = New DataView(pDtAll, "", "", DataViewRowState.CurrentRows).ToTable(True, "CustomerID")
        For Each pDrCustomer As DataRow In pDtCustomer.Rows
            Dim pDV = New DataView(pDtAll, "CustomerID = " & ReadyString(pDrCustomer("CustomerID")), "", DataViewRowState.CurrentRows)
            CacheWeightedAverageDaysOverdue(CStr2(pDrCustomer("CustomerID")), pDV)
        Next
        mProData.CommitTransaction()
        If String.IsNullOrWhiteSpace(vCustomerID) Then
            mParent.SystemDefaults.WeightedAverageDaysOverdueLastUpdate = Now
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vCustomerID"></param>
    ''' <param name="vDvInvoices">from GetInvoicesWithCreditFromCache</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CacheWeightedAverageDaysOverdue(vCustomerID As String, vDvInvoices As DataView)

        Dim pTotalNetBalance As Decimal = 0
        For Each pDRV As DataRowView In vDvInvoices
            pTotalNetBalance += CDec2(pDRV("NetInvoiceBalance"))
        Next

        For Each pDRV As DataRowView In vDvInvoices
            Dim pIU = New InsertUpdate("CacheInvoices")
            Dim pWeightRelativeToTotalAr = CDec2(pDRV("NetInvoiceBalance")) / pTotalNetBalance
            pIU("WeightRelativeToTotalAr") = pWeightRelativeToTotalAr * 100
            pIU("WeightedAverageDaysOverdue") = pWeightRelativeToTotalAr * CDec2(pDRV("ArAge"))
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDRV("ID")))
            Try
                mProData.ExecuteSQL(pSql)
            Catch ex As ProSqlException
                AppendToLog("vb8D67XYKDLP CacheWeightedAverageDaysOverdue: " & ex.Message & ex.Sql)
                Throw
            Catch ex As Exception
                AppendToLog("vb5GLSK7UFSR CacheWeightedAverageDaysOverdue: " & ex.Message)
                Throw
            End Try
        Next

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDT">from GetInvoicesWithCreditFromCache</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAgingOfAr(vDT As DataTable) As Decimal

        Dim pTotalAmount As Decimal
        Dim pOut As Decimal

        pTotalAmount = CDec2(vDT.Compute("SUM(NetInvoiceBalance)", "ArAge > 0"))
        If pTotalAmount = 0 Then
            Return 0
        End If
        For Each pDR As DataRow In vDT.Rows
            pOut += CInt2(pDR("ArAge")) * CDec2(pDR("NetInvoiceBalance")) / pTotalAmount
        Next pDR

        Return pOut

    End Function

    Public Function FromCache(vID As String) As DataRow

        Dim pOut As DataRow
        Dim pSql As String

        pSql = "SELECT *" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pOut = mProData.ExecuteRow(pSql)

        Return pOut

    End Function

    Public Class SaveAdjustmentJson
        Public ID As String ' set in VB
        Public IsNew As Boolean ' set in VB
        Public InvoiceID As String
        Public ReasonID As String
        Public Amount As String
        Public Remarks As String
        Public ReturnedChecks As List(Of ReturnedChecksJson)
        Public AmountProper As Decimal ' set in VB
        Public MemoID As String ' set in VB
        Public SourceDiscountID As String ' set in ImportDataClass
        Public UserID As String ' optional
        Public DontRecache As Boolean ' set in VB

        Public Function ShallowCopy() As SaveAdjustmentJson

            Return MemberwiseClone()

        End Function

        Public Class ReturnedChecksJson
            Public ID As String
            Public CustomerPaymentCheckAmountID As String
        End Class
    End Class

    Public Sub SaveAdjustment(vJson As SaveAdjustmentJson, ByRef oError As String)

        Dim pDR As DataRow

        If String.IsNullOrWhiteSpace(vJson.ReasonID) Then
            Throw New ApplicationException("vJson.ReasonID is required. ref: vbJ5NVZRAK6J!")
        End If
        pDR = mParent.LibraryAccountAdjustments.GetByID(vJson.ReasonID)
        If pDR Is Nothing _
            OrElse CStr2(pDR("Reason")) <> "Returned Checks" Then

            SaveAdjustmentInternal(vJson, oError)
            Return
        End If

        If vJson.ReturnedChecks Is Nothing _
            OrElse vJson.ReturnedChecks.Count = 0 Then

            Throw New ApplicationException("ReturnedChecks.Count must be at least 1. ref: vb93HZCP2CSD!")
        End If

        mProData.BeginTransaction()
        Dim pDtCheckAmountAll As DataTable
        For Each pReturnCheck In vJson.ReturnedChecks
            Dim pDtCheckAmount = mParent.TransCustomerPayment.GetChequeDistribution(vCustomerPaymentCheckAmountID:=pReturnCheck.CustomerPaymentCheckAmountID)
            If pDtCheckAmountAll Is Nothing Then
                pDtCheckAmountAll = pDtCheckAmount
            Else
                pDtCheckAmountAll.Merge(pDtCheckAmount)
            End If
        Next pReturnCheck

        If pDtCheckAmountAll IsNot Nothing Then
            Dim pDtChequeInvoices = New DataView(pDtCheckAmountAll).ToTable(True, "InvoiceID")
            For Each pDrChequeInvoice As DataRow In pDtChequeInvoices.Rows
                Dim pJson As New SaveAdjustmentJson()

                pJson.InvoiceID = ReadyGuid(pDrChequeInvoice("InvoiceID"))
                pJson.ReasonID = LibraryAccountAdjustments.ReturnedChequesID
                pJson.Remarks = vJson.Remarks
                pJson.IsNew = vJson.IsNew
                pJson.ID = vJson.ID
                pJson.UserID = vJson.UserID
                pJson.MemoID = vJson.MemoID
                pJson.SourceDiscountID = vJson.SourceDiscountID
                pJson.ReturnedChecks = New List(Of SaveAdjustmentJson.ReturnedChecksJson)
                For Each pDrChequeAmount As DataRow In pDtCheckAmountAll.Rows
                    If ReadyGuid(pDrChequeAmount("InvoiceID")) <> pJson.InvoiceID Then
                        Continue For
                    End If
                    'Dim p = From pCheque In vJson.ReturnedChecks
                    '    Where ReadyGuid(pCheque.CustomerPaymentCheckAmountID) = ReadyGuid(pDrChequeAmount("CustomerPaymentCheckAmountID"))
                    '    Select pCheque

                    'If p.Count = 0 Then
                    '    Continue For
                    'End If

                    pJson.Amount = CDec2(pJson.Amount) + CDec2(pDrChequeAmount("AmountPaid"))
                    pJson.ReturnedChecks.Add(New SaveAdjustmentJson.ReturnedChecksJson() _
                        With {
                            .ID = ReadyGuid(pDrChequeAmount("CustomerPaymentCheckID")),
                            .CustomerPaymentCheckAmountID = ReadyGuid(pDrChequeAmount("CustomerPaymentCheckAmountID"))
                        })
                Next pDrChequeAmount
                SaveAdjustmentInternal(pJson, oError)
                If String.IsNullOrWhiteSpace(oError) = False Then
                    mProData.RollbackTransaction()
                    Return
                End If
            Next pDrChequeInvoice
        End If

        mProData.CommitTransaction()

    End Sub

    Public Sub SaveAdjustmentInternal(vJson As SaveAdjustmentJson, ByRef oError As String)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pAmount As Decimal
        Dim pAffectedRecords As Integer

        pAmount = CDec2(vJson.Amount)
        If mParent.LibraryAccountAdjustments.IsAdjustmentDeductive(vJson.ReasonID) Then
            pAmount *= -1
        End If

        pSql = "(SELECT ISNULL(MAX(AdjustmentRef), 0) + 1 FROM TransInvoiceAdjustments)"
        pIU = New InsertUpdate("TransInvoiceAdjustments")
        If vJson.IsNew Then
            vJson.ID = Guid.NewGuid.ToString()
            pIU("ID") = ReadyString(vJson.ID)
        End If
        pIU("InvoiceID") = ReadyString(vJson.InvoiceID)
        pIU("AdjustmentDate") = "GETDATE()"
        pIU("AdjustmentRef") = pSql
        pIU("ReasonID") = ReadyString(vJson.ReasonID)
        pIU("Amount") = pAmount
        pIU("IsActive") = 1
        pIU("Remarks") = ReadyString(vJson.Remarks)
        If String.IsNullOrWhiteSpace(vJson.UserID) Then
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        Else
            pIU("UserID") = ReadyString(vJson.UserID)
        End If
        If String.IsNullOrWhiteSpace(vJson.MemoID) = False Then
            pIU("MemoID") = ReadyString(vJson.MemoID)
        End If
        If String.IsNullOrWhiteSpace(vJson.SourceDiscountID) = False Then
            pIU("SourceDiscountID") = ReadyString(vJson.SourceDiscountID)
        End If
        If vJson.IsNew Then
            pSql = pIU.GetInsert()
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(vJson.ID))
        End If
        mProData.BeginTransaction()
        pAffectedRecords = mProData.ExecuteSQL(pSql)
        If pAffectedRecords = 0 Then
            mProData.RollbackTransaction()
            oError = "No record was affected. ref: vb2QY8XHX8G2!"
            Return
        End If

        pSql = "DELETE FROM TransInvoiceAdjustmentReturnedChecks" & vbCrLf &
                " WHERE InvoiceAdjustmentID = " & ReadyString(vJson.ID)
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        If vJson.ReturnedChecks IsNot Nothing _
            AndAlso ReadyGuid(vJson.ReasonID) = ReadyGuid(LibraryAccountAdjustments.ReturnedChequesID) Then

            For Each pReturnCheck In vJson.ReturnedChecks
                pSql = "SELECT TransInvoiceAdjustments.ID" & vbCrLf &
                    ",TransInvoiceAdjustments.AdjustmentRef" & vbCrLf &
                    ",TransInvoiceAdjustments.AdjustmentDate" & vbCrLf &
                    ",TransCustomerPaymentChecks.BankName" & vbCrLf &
                    ",TransCustomerPaymentChecks.CheckNumber" & vbCrLf &
                    ",TransCustomerPaymentChecks.CheckAmount" & vbCrLf &
                    ",TransCustomerPaymentChecks.CheckDate" & vbCrLf &
                    ",TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckAmountID" & vbCrLf &
                    " FROM TransInvoiceAdjustments" & vbCrLf &
                    " JOIN TransInvoiceAdjustmentReturnedChecks ON TransInvoiceAdjustmentReturnedChecks.InvoiceAdjustmentID = TransInvoiceAdjustments.ID" & vbCrLf &
                    " LEFT JOIN TransCustomerPaymentChecks ON TransCustomerPaymentChecks.ID = TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckID" & vbCrLf &
                    " WHERE TransInvoiceAdjustments.IsActive = 1" & vbCrLf &
                    " AND TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckID = " & ReadyString(pReturnCheck.ID) & vbCrLf &
                    " AND (TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckAmountID = " & ReadyString(pReturnCheck.CustomerPaymentCheckAmountID) & vbCrLf &
                    "   OR TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckAmountID IS NULL" & vbCrLf &
                    ")"
                Dim pDR = mProData.ExecuteRow(pSql)
                If pDR IsNot Nothing Then
                    mProData.RollbackTransaction()
                    oError = "The cheque from " & CStr2(pDR("BankName")) & " with cheque number, " & CStr2(pDR("CheckNumber")) & " was already returned by account adjustment number " & pDR("AdjustmentRef") & " on " & pDR("AdjustmentDate")
                    Return
                End If

                pIU = New InsertUpdate("TransInvoiceAdjustmentReturnedChecks")
                pIU("ID") = "NEWID()"
                pIU("InvoiceAdjustmentID") = ReadyString(vJson.ID)
                pIU("CustomerPaymentCheckID") = ReadyString(pReturnCheck.ID)
                pIU("CustomerPaymentCheckAmountID") = ReadyString(pReturnCheck.CustomerPaymentCheckAmountID)
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)

                pIU = New InsertUpdate("TransCustomerPaymentChecks")
                pIU("DepositStatus") = 2
                pIU("TimeChecked") = "GETDATE()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pReturnCheck.ID))
                pAffectedRecords = mProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    Throw New ApplicationException("No affected record in TransCustomerPaymentChecks. ref: vbXP9Q2NUTHM!")
                End If
            Next pReturnCheck
        End If

        'TODO: 2018_10_30__09_46_41_8584 cache unapplied credit if needed
        If vJson.DontRecache = False Then
            mParent.TransInvoice.CacheInvoicesWithCredit(vJson.InvoiceID)
        End If
        mParent.TransReceiving.CacheReceivingsWithCredit()

        pSql = "SELECT NetInvoiceBalance" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE ID = " & ReadyString(vJson.InvoiceID)
        Dim pDrInvoice = mProData.ExecuteRow(pSql)
        If pDrInvoice IsNot Nothing Then
            If CDec2(pDrInvoice("NetInvoiceBalance")) < 0 Then
                pIU = New InsertUpdate("TransInvoiceAdjustments")
                pIU("UnappliedCredits") = -CDec2(pDrInvoice("NetInvoiceBalance"))
                pSql = pIU.GetUpdate("ID = " & ReadyString(vJson.ID))
                mProData.ExecuteSQL(pSql)
            End If
        Else
            'todo 2018_11_17__13_57_16_2296
        End If
        mProData.CommitTransaction()

    End Sub

    Public Function GetAdjustmentBySourceDiscountID(vSourceDiscountID As String) As DataRow

        Dim pSql = "SELECT *" & vbCrLf &
            " FROM TransInvoiceAdjustments" & vbCrLf &
            " WHERE SourceDiscountID = " & ReadyString(vSourceDiscountID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetAdjustments(vInvoice As String, Optional vAdjustmentID As String = Nothing) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        If vInvoice IsNot Nothing Then
            pSql &= " AND TransInvoiceAdjustments.InvoiceID = " & ReadyString(vInvoice) & vbCrLf
        End If
        If vAdjustmentID IsNot Nothing Then
            pSql &= " AND TransInvoiceAdjustments.ID = " & ReadyString(vAdjustmentID) & vbCrLf
        End If

        pSql = " SELECT TransInvoiceAdjustments.ID" & vbCrLf &
            ",TransInvoiceAdjustments.AdjustmentDate" & vbCrLf &
            " ,TransInvoiceAdjustments.AdjustmentRef" & vbCrLf &
            " ,LibraryAccountAdjustments.AdjustmentType" & vbCrLf &
            " ,TransInvoiceAdjustments.ReasonID" & vbCrLf &
            " ,LibraryAccountAdjustments.Reason" & vbCrLf &
            " ,TransInvoiceAdjustments.Amount" & vbCrLf &
            " ,TransInvoiceAdjustments.Remarks" & vbCrLf &
            " ,TransInvoiceAdjustments.MemoID" & vbCrLf &
            " ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName + MobizLibraryPeople.MiddleName AS UpdateByName" & vbCrLf &
            " FROM TransInvoiceAdjustments" & vbCrLf &
            " LEFT JOIN LibraryAccountAdjustments ON LibraryAccountAdjustments.ID = TransInvoiceAdjustments.ReasonID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoiceAdjustments.UserID" & vbCrLf &
            " WHERE TransInvoiceAdjustments.IsActive = 1" & vbCrLf &
            pSql & vbCrLf &
            " ORDER BY TransInvoiceAdjustments.AdjustmentDate"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetAdjustmentReturnedChecks(vAdjustmentID As String) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = " SELECT TransCustomerPaymentChecks.BankName" & vbCrLf &
            " ,TransCustomerPaymentChecks.CheckNumber" & vbCrLf &
            " ,TransCustomerPaymentChecks.CheckAmount" & vbCrLf &
            " ,TransCustomerPaymentChecks.CheckDate" & vbCrLf &
            " , TransInvoice.InvoiceNumber AS [InvoiceNumber]" & vbCrLf &
            " , TransInvoice.InvoiceDate AS [InvoiceDate]" & vbCrLf &
            " , TransInvoice.TotalAmount AS [InvoiceAmount]" & vbCrLf &
            " , TransCustomerPaymentChecks.ID" & vbCrLf &
            " , TransCustomerPaymentCheckAmount.AmountPaid AS [ChequeAmountPaid]" & vbCrLf &
            " , TransCustomerPaymentCheckAmount.ID AS CustomerPaymentCheckAmountID" & vbCrLf &
            " , CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS [InvoiceMin]" & vbCrLf &
            " FROM TransInvoiceAdjustmentReturnedChecks" & vbCrLf &
            " JOIN TransCustomerPaymentChecks ON TransCustomerPaymentChecks.ID = TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckID" & vbCrLf &
            " LEFT JOIN TransCustomerPaymentCheckAmount ON TransCustomerPaymentCheckAmount.CustomerPaymentCheckID = TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckID" & vbCrLf &
            " AND (TransCustomerPaymentCheckAmount.ID = TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckAmountID" & vbCrLf &
            "   OR TransInvoiceAdjustmentReturnedChecks.CustomerPaymentCheckAmountID IS NULL" & vbCrLf &
            ")" & vbCrLf &
            " LEFT JOIN TransInvoiceAdjustments ON TransInvoiceAdjustments.ID = TransInvoiceAdjustmentReturnedChecks.InvoiceAdjustmentID" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransInvoice.ID = TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " WHERE TransInvoiceAdjustmentReturnedChecks.InvoiceAdjustmentID = " & ReadyString(vAdjustmentID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' returns nothing if success. otherwise, returns the error message
    ''' </summary>
    ''' <param name="vAdjustmentID"></param>
    ''' <param name="vUserID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeactivateAdjustment(vAdjustmentID As String, vUserID As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecordsAffected As Integer

        mProData.BeginTransaction()
        pIU = New InsertUpdate("TransInvoiceAdjustments")
        pIU("IsActive") = 0
        pIU("DeactivatedBy") = ReadyString(vUserID)
        pIU("DateDeactivated") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vAdjustmentID) & vbCrLf &
            " AND IsActive = 1")
        pRecordsAffected = mProData.ExecuteSQL(pSql)
        If pRecordsAffected = 0 Then
            mProData.RollbackTransaction()
            Return "No record was affected while deactivating memo. ref: vbM3HNVXVHB9!"
        End If

        pSql = "SELECT TransInvoiceAdjustments.MemoID" & vbCrLf &
            " ,TransMemo.FowstID" & vbCrLf &
            " FROM TransInvoiceAdjustments" & vbCrLf &
            " JOIN TransMemo ON TransMemo.ID = TransInvoiceAdjustments.MemoID" & vbCrLf &
            " WHERE TransInvoiceAdjustments.ID = " & ReadyString(vAdjustmentID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing _
            AndAlso IsGuid(pDR("MemoID")) Then

            mParent.TransMemoSalesReturn.AddToInventoryLedger(CStr2(pDR("MemoID")), CStr2(pDR("FowstID")), True)
        End If

        mProData.CommitTransaction()

    End Function

    Public Function GetReceiptPrintedStatus(vDateFrom As Date, vDateTo As Date, vFowstID As String) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = " -- GetReceiptPrintedStatus" & vbCrLf &
            " SELECT tblStatus.Status" & vbCrLf &
            ",tblStatus.StatusText" & vbCrLf &
            " ,SUM(CacheInvoices.NetInvoiceAmount) AS NetInvoiceAmount" & vbCrLf &
            " ,COUNT(DISTINCT CacheInvoices.ID) AS Count" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = CacheInvoices.ID" & vbCrLf &
            " RIGHT JOIN (" & vbCrLf &
            " 	SELECT CONVERT(bit, NULL) AS Status" & vbCrLf &
            "     ,'N/A' AS StatusText" & vbCrLf &
            "     UNION" & vbCrLf &
            "     SELECT CONVERT(bit, 1) AS Status" & vbCrLf &
            "     ,'Printed' AS StatusText" & vbCrLf &
            "     UNION" & vbCrLf &
            "     SELECT CONVERT(bit, 0) AS Status" & vbCrLf &
            "     ,'Not Printed' AS StatusText" & vbCrLf &
            " ) tblStatus ON (tblStatus.Status = CacheInvoices.IsPrinted" & vbCrLf &
            " OR (tblStatus.Status IS NULL AND CacheInvoices.IsPrinted IS NULL)" & vbCrLf &
            " )" & vbCrLf &
            " AND TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " GROUP BY CacheInvoices.IsPrinted" & vbCrLf &
            ",tblStatus.StatusText" & vbCrLf &
            " ,tblStatus.Status"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetMaxQuotationNum(vFowstID As String, vDeviceID As String) As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = " SELECT MAX(QuotationNo)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function SaveQty(vInvoiceItemID As String, vQtyMode As String, vValue As Decimal, vUserID As String, ByRef oError As String) As Boolean

        Dim pSql As String
        Dim pInvoiceID As String
        Dim pIU As InsertUpdate

        oError = Nothing
        pSql = "SELECT InvoiceID" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceItemID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oError = "TransInvoiceItems with ID, " & vInvoiceItemID & ", does not exist."
            Return False
        End If
        pInvoiceID = CStr2(pDR("InvoiceID"))
        If String.IsNullOrWhiteSpace(pInvoiceID) Then
            oError = "TransInvoiceItems with ID, " & vInvoiceItemID & ", does not exist."
            Return False
        End If

        pIU = New InsertUpdate("TransInvoiceItems")
        If vQtyMode = "order" Then
            pIU("OrderedQuantity") = vValue
        ElseIf vQtyMode = "serve" Then
            pIU("Quantity") = vValue
            pIU("ServedQtyIsEdited") = 1
            pIU("SubTotal") = vValue & " * UnitPrice"
            UpdateSalesOrderServedQuantityFromVendorPurchaseOrder(pInvoiceID, CStr2(pDR("ProductID")), CDec2(pDR("ActualQuantity")), vValue)
        End If
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceItemID))
        mProData.BeginTransaction()
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only 

        pIU = New InsertUpdate("TransInvoice")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(pInvoiceID))
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        mProData.CommitTransaction()

        Return True

    End Function

    Private Function UpdateSalesOrderServedQuantityFromVendorPurchaseOrder(vPurchaseOrderID As String, vProductID As String, vActualQuantity As Decimal, vQty As Decimal) As Integer

        Dim pSql = "UPDATE TransInvoiceItems" & vbCrLf &
            " SET Quantity = " & vQty & vbCrLf &
            ",SubTotal = " & vQty & " * UnitPrice" & vbCrLf &
            ",ServedQtyIsEdited = 1" & vbCrLf &
            " WHERE PurchaseOrderID = " & ReadyString(vPurchaseOrderID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND ActualQuantity = " & vActualQuantity
        Dim pOut = CInt2(mProData.ExecuteSQL(pSql)) ' output is used for debugging only

        Return pOut


    End Function

    Public Function Verify(vData As SaveSalesOrderJson, vOrgID As String, ByRef oError As String) As Boolean

        If vData.Verify(oError) = False Then
            Return False
        End If

        If vData.TransTypeProper = SaveSalesOrderJson.eTransType.Invoice Then
            If String.IsNullOrWhiteSpace(vData.SalesOrderID) = False Then
                Dim pDR = BySalesOrderID(vData.SalesOrderID)
                If pDR IsNot Nothing Then
                    If IsGuid(vData.ID) = False Then
                        oError = "The sales order was already invoiced."
                        Return False
                    End If
                    If ReadyGuid(pDR("ID")) <> ReadyGuid(vData.ID) Then
                        oError = "The sales order was already invoiced."
                        Return False
                    End If
                End If
            End If
        End If

        '#If DEBUG Then
        '        Dim pDtUnits As DataTable

        '        For Each pProduct As SaveSalesOrderJson.ProductClass In vData.Products
        '            pDtUnits = mParent.LibraryStock.PriceTable(vOrgID, pProduct.ProductID, , , True)
        '            If pDtUnits.Rows.Count = 0 Then
        '                oError = "Product " & pProduct.ProductID & " has no units and/or no price. Ref vbEK9C7T78G3!"
        '                Return False
        '            End If
        '            pProduct.Unit = pDtUnits.Rows(0)("UnitMeasure")
        '            pProduct.PriceLevelID = CStr2(pDtUnits.Rows(0)("PriceLevelID"))
        '            pProduct.UnitQty = CStr2(pDtUnits.Rows(0)("Quantity"))
        '        Next pProduct
        '#End If

        Return True

    End Function

    Public Function Save(vData As SaveSalesOrderJson, vOrgID As String, ByRef oError As String) As String

        Dim pCreator As Creator
        Dim pNum As Integer
        Dim pID As String
        Dim pDR As DataRow
        Dim pDate As Date?
        Dim pFowstWideInvoiceNumber As Integer?
        Dim pTransDate As Date?

        pCreator = New Creator(mProData, Me)
        pCreator.ID = vData.ID
        pCreator.CustomerID = vData.CustomerID
        pCreator.BillingAddressID = vData.BillingAddressID
        pCreator.DeliveryAddressID = vData.DeliveryAddressID
        pCreator.FowstID = vData.FowstID
        pCreator.SupplierFowstID = vData.FowstID ' to make order processing compatible with netsuite
        pCreator.DeviceID = vData.DeviceID
        pCreator.UserID = vData.UserID
        pCreator.InvoiceNumber = 0
        pCreator.InvoiceNumber = 0
        pCreator.SalesPartnerOrgID = vData.SalesPartnerOrgID
        pCreator.DontAutoInvoice = vData.DontAutoInvoice
        If vData.SaveSalesRep Then
            pCreator.SalesRepPeopleID = vData.SalesRepPeopleID
            pCreator.SalesRepText = vData.SalesRepText
        End If
        pCreator.RefInvoiceNum = vData.RefInvoiceNum
        If CBool2(vData.IsPos) Then
            pCreator.IsForGovReport = True
        Else
            pCreator.TransType = vData.TransTypeProper.ToString()
        End If
        pCreator.DeliveryMethod = vData.DeliveryMethod
        pCreator.OrderedByID = vData.OrderedByID
        pCreator.InvoiceFowstID = vData.InvoiceFowstID
        pCreator.SalesInvoiceRef = vData.SalesInvoiceRef
        pCreator.OfficialReceiptRef = vData.OfficialReceiptRef
        pCreator.StockTransferID = vData.StockTransferID
        If String.IsNullOrWhiteSpace(vData.DateNeeded) = False Then
            pCreator.DateNeeded = ParseDate(vData.DateNeeded)
            If pCreator.DateNeeded Is Nothing Then
                oError = "Date Needed must be a valid date."
                Return Nothing
            End If
        End If
        pCreator.Remarks = vData.Remarks
        pCreator.PurchaseOrderText = vData.PurchaseOrderText
        If String.IsNullOrWhiteSpace(vData.PurchaseOrderDate) = False Then
            pDate = ParseDate(vData.PurchaseOrderDate)
            If pDate Is Nothing Then
                oError = "Purchase Date must be a valid date."
                Return Nothing
            End If
            pCreator.PurchaseOrderDate = pDate
        End If

        If String.IsNullOrWhiteSpace(vData.SalesOrderID) = False Then
            pCreator.SalesOrderID = vData.SalesOrderID
        End If
        If String.IsNullOrWhiteSpace(vData.QuotationID) = False Then
            pCreator.QuotationID = vData.QuotationID
        End If
        If IsGuid(vData.ID) Then
            pDR = ById(vData.ID)
        End If
        If String.IsNullOrWhiteSpace(vData.QuotationNo) = False Then
            pCreator.QuotationNo = vData.QuotationNo
        End If
        If String.IsNullOrWhiteSpace(vData.QuotationDate) = False Then
            pCreator.QuotationDate = vData.QuotationDate
        End If
        If String.IsNullOrWhiteSpace(vData.QuotationNumText) = False Then
            pCreator.QuotationNumText = vData.QuotationNumText
        End If
        If IsGuid(vData.DocumentNameID) Then
            If ReadyGuid(vData.DocumentNameID) = ReadyGuid(Guid.Empty) Then
                vData.DocumentNameID = Nothing
            Else
                pCreator.DocumentNameID = vData.DocumentNameID
            End If
        End If
        If String.IsNullOrWhiteSpace(vData.PaymentTerm) = False Then
            pCreator.PaymentTerm = vData.PaymentTerm
        End If
        If String.IsNullOrWhiteSpace(vData.CanvassID) = False Then
            pCreator.CanvassID = vData.CanvassID
        End If
        If String.IsNullOrWhiteSpace(vData.CustomerFowstID) = False Then
            pCreator.CustomerFowstID = vData.CustomerFowstID
        End If
        If String.IsNullOrWhiteSpace(vData.CustomerBinID) = False Then
            pCreator.CustomerBinID = vData.CustomerBinID
        End If
        If String.IsNullOrWhiteSpace(vData.UneditedBackorder) = False Then
            pCreator.UneditedBackorder = CBool2(vData.UneditedBackorder)
        End If
        If String.IsNullOrWhiteSpace(vData.BackorderSourceInvoiceID) = False Then
            pCreator.BackorderSourceInvoiceID = vData.BackorderSourceInvoiceID
        End If
        If String.IsNullOrWhiteSpace(vData.InternalRemarks) = False Then
            pCreator.InternalRemarks = vData.InternalRemarks
        End If

        '// "Freight and Handling" tab
        If String.IsNullOrWhiteSpace(vData.ShipperID) = False Then
            pCreator.ShipperID = vData.ShipperID
        End If
        If String.IsNullOrWhiteSpace(vData.WaybillNumber) = False Then
            pCreator.WaybillNumber = vData.WaybillNumber
        End If
        If String.IsNullOrWhiteSpace(vData.WaybillNumber) = False Then
            pCreator.WaybillNumber = vData.WaybillNumber
        End If
        If String.IsNullOrWhiteSpace(vData.ShipDate) = False Then
            pCreator.ShipDate = vData.ShipDate
        End If
        pCreator.PackageQuantity = vData.PackageQuantity
        pCreator.GrossWeight = vData.GrossWeight
        pCreator.GrossVolume = vData.GrossVolume
        pCreator.Freight = vData.Freight
        pCreator.Handling = vData.Handling
        pCreator.Arrastre = vData.Arrastre
        pCreator.OtherCharges = vData.OtherCharges

        pTransDate = ParseDate(vData.TransDate)
        Select Case vData.TransTypeProper
            Case SaveSalesOrderJson.eTransType.Quote
                If vData.SaveAndFinalizeProper Then
                    If pDR Is Nothing _
                        OrElse CDec2(pDR("QuotationNo")) = 0 Then

                        pNum = GetMaxQuotationNum(pCreator.FowstID, pCreator.DeviceID)
                        pCreator.QuotationNo = pNum + 1
                        If String.IsNullOrWhiteSpace(vData.QuotationDate) Then
                            pCreator.QuotationDate = Now
                        End If
                    End If
                End If

            Case SaveSalesOrderJson.eTransType.SalesOrder
                If vData.SaveAndFinalizeProper Then
                    If pDR Is Nothing _
                        OrElse CDec2(pDR("SalesOrderNo")) = 0 Then

                        pNum = GetMaxSoNumber(pCreator.DeviceID, pCreator.FowstID)
                        pCreator.SalesOrderNo = pNum + 1
                    End If
                    If pTransDate Is Nothing Then
                        pCreator.SalesOrderDate = Now
                    Else
                        pCreator.SalesOrderDate = pTransDate
                    End If
                    If ReadyGuid(vOrgID) = ReadyGuid(modClient.AbcOrgID) _
                        OrElse ReadyGuid(vOrgID) = ReadyGuid(modClient.DucOrgID) Then

                        SegregateSalesOrderByArea(vData.ID, pCreator)
                    End If
                End If

            Case SaveSalesOrderJson.eTransType.Invoice
                If vData.SaveAndFinalizeProper Then
                    Dim pDrInvoice = ById(vData.ID)
                    If CBool2(vData.IsPos) = False _
                        AndAlso (
                            CStr2(pDrInvoice("CncUnapprovedReason")) <> "" _
                            AndAlso CStr2(pDrInvoice("CncApprovedByID")) = "" _
                            OrElse CBool2(pDrInvoice("UnapprovedProfitability"))
                        ) Then

                        pCreator.FinalizedForApproval = True
                        vData.SaveAndFinalize = False
                        vData.FinalizedForApproval = True
                    Else
                        VerifyCounter(pCreator.DeviceID, pCreator.FowstID, pNum, vData.DocumentNameID, pFowstWideInvoiceNumber)
                        If pFowstWideInvoiceNumber IsNot Nothing Then
                            pCreator.FowstWideInvoiceNumber = pFowstWideInvoiceNumber + 1
                        End If
                        pCreator.InvoiceNumber = pNum + 1
                    End If
                End If
                If pTransDate Is Nothing Then
                    If mParent.SystemDefaults.EnforceZRead Then
                        Dim pDrDeviceFowst = WebRequest.Instance.Business.LibraryDeviceFowst.ByDeviceFowst(pCreator.DeviceID, pCreator.FowstID)
                        If pDrDeviceFowst IsNot Nothing Then
                            If pDrDeviceFowst("ZReadDate") IsNot DBNull.Value Then
                                pTransDate = CDate(pDrDeviceFowst("ZReadDate")).Date + Now.TimeOfDay
                            End If
                        End If
                    End If
                End If
                If pTransDate Is Nothing Then
                    pCreator.InvoiceDate = Now
                Else
                    pCreator.InvoiceDate = pTransDate
                End If
            Case SaveSalesOrderJson.eTransType.PurchaseOrder
                If vData.SaveAndFinalizeProper Then
                    If pDR Is Nothing _
                        OrElse CDec2(pDR("PurchaseOrderNo")) = 0 Then

                        pNum = GetNextPoNumber(pCreator.DeviceID, pCreator.FowstID)
                        pCreator.PurchaseOrderNo = pNum
                        pCreator.PurchaseOrderDate = Now
                    End If
                End If

            Case SaveSalesOrderJson.eTransType.AccountsPayable
                If vData.SaveAndFinalizeProper Then
                    If pDR Is Nothing _
                        OrElse CDec2(pDR("AccountsPayableNum")) = 0 Then

                        pNum = GetMaxAccountsPayableNumber(pCreator.DeviceID, pCreator.FowstID) + 1
                        pCreator.AccountsPayableNum = pNum
                    End If
                End If

        End Select

        'With pCreator.Stocks.Columns
        '    .Add("ID")
        '    .Add("InvoiceID")
        '    .Add("StockID")
        '    .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
        '    .Add("Quantity", GetType(Decimal))
        '    .Add("ActualQuantity", GetType(Integer))
        '    .Add("UnitMeasure")
        '    .Add("TermDiscount", GetType(Decimal))
        '    .Add("DiscountIfPaidWithIn", GetType(Decimal))
        '    .Add("NetDue")
        '    .Add("UnitPrice", GetType(Decimal))
        '    .Add("OriginalUnitPrice", GetType(Decimal))
        '    .Add("Subtotal", GetType(Decimal))
        '    .Add("Cost", GetType(Decimal))
        '    .Add("PriceLevelID")
        '    .Add("Dimension")
        '    .Add("IsReplacement", GetType(Boolean))
        '    .Add("IsHasReplacement", GetType(Boolean))
        '    .Add("IsHasBackOrder", GetType(Boolean))
        '    .Add("RefStockID")
        '    .Add("UnitGP", GetType(Decimal))
        '    .Add("UnitGPM", GetType(Decimal))
        '    .Add("LineGP", GetType(Decimal))
        '    .Add("LineGPM", GetType(Decimal))
        '    .Add("LineCost", GetType(Decimal))
        '    .Add("LineNumber", GetType(Integer))
        '    .Add("TaxPercent", GetType(Decimal))
        '    .Add("TaxAmount", GetType(Decimal))
        '    .Add("TaxSales", GetType(Decimal))
        '    '.Add("SalesRep", GetType(Object))
        '    .Add("DiscountAmount", GetType(Decimal))
        '    .Add("DiscountSales", GetType(Decimal))
        '    .Add("RequestedUnitPrice", GetType(Decimal))
        '    .Add("VATExemptSale", GetType(Decimal))
        '    .Add("ZeroRatedSale", GetType(Decimal))
        '    .Add("NonVATSale", GetType(Decimal))
        '    .Add("VATableSale", GetType(Decimal))
        '    .Add("ModifierGroupID")
        'End With

        'Dim pDR As DataRow
        'For Each pProduct As SaveSalesOrderJson.ProductClass In vData.Products
        '    pDR = pCreator.Stocks.NewRow()
        '    pDR("StockID") = pProduct.ProductID
        '    pDR("OrderedQuantity") = pProduct.Quantity
        '    pDR("ActualQuantity") = pProduct.UnitQty
        '    pDR("UnitMeasure") = pProduct.Unit
        '    pDR("UnitPrice") = pProduct.UnitPrice
        '    pDR("Subtotal") = CDec2(pProduct.Quantity) * CDec2(pProduct.UnitPrice)
        '    pDR("Cost") = CDec2(mParent.LibraryStock.GetLandedCost(pProduct.ProductID, vOrgID)) * CDec2(pDR("ActualQuantity"))
        '    pDR("PriceLevelID") = pProduct.PriceLevelID

        '    Dim pUnitGp As Decimal
        '    Dim pUnitGpm As Double
        '    Dim pLineGp As Decimal
        '    Dim pLineGpm As Decimal
        '    Dim pLandedCost As Decimal

        '    mParent.LibraryStock.GetProfitability(
        '        vStockID:=CStr2(pProduct.ProductID),
        '        vQty:=If(CDec2(pDR("Quantity")) > 0, CDec2(pDR("Quantity")), CDec2(pDR("OrderedQuantity"))),
        '        vUnitQty:=pDR("ActualQuantity"),
        '        vUnitPrice:=pDR("UnitPrice"),
        '        oUnitGP:=pUnitGp,
        '        oUnitGPM:=pUnitGpm,
        '        oLineGP:=pLineGp,
        '        oLineGPM:=pLineGpm,
        '        oLandedCost:=pLandedCost,
        '        vHistoricCost:=0,
        '        vOrgID:=vOrgID)
        '    pDR("UnitGP") = pUnitGp
        '    pDR("UnitGPM") = pUnitGpm
        '    pDR("LineGP") = pLineGp
        '    pDR("LineGPM") = pLineGpm
        '    pDR("LineCost") = pLandedCost * CDec2(pDR("Quantity")) * CDec2(pDR("ActualQuantity"))
        '    pDR("TaxPercent") = 12 ' todo

        '    'addtional by janine feb 4 2015
        '    'if SCD (vat exempt, tax sales and vat amount must be 0)
        '    If (CDec2(pDR("VATExemptSale")) = 0 And CDec2(pDR("ZeroRatedSale")) = 0 And CDec2(pDR("NonVATSale")) = 0) Then
        '        pDR("TaxSales") = RoundDown(pDR("SubTotal") / (1 + pDR("TaxPercent") / 100), 4)
        '        pDR("TaxAmount") = pDR("SubTotal") - pDR("TaxSales")
        '    End If
        '    pCreator.Stocks.Rows.Add(pDR)
        'Next pProduct

        pCreator.Stocks = New DataTable()
        If vData.CopyFromCanvassProper Then
            If vData.ID Is Nothing Then
                pCreator.Stocks = CanvassItemsToStockTable(vData.CanvassID, vData.CustomerID)
            End If
        End If
        mProData.BeginTransaction()
        If pCreator.Save(vData.ID Is Nothing, oError) Then
            If String.IsNullOrWhiteSpace(vData.ID) Then
                vData.ID = pCreator.ID
            End If
            If String.IsNullOrWhiteSpace(vData.CopyProductsFromInvoiceID) = False Then
                Dim pCopyProductsFromInvoiceIDs = vData.CopyProductsFromInvoiceID.Split({","}, StringSplitOptions.RemoveEmptyEntries)
                'If pCopyProductsFromInvoiceIDs.Count > 0 Then
                '    If String.IsNullOrWhiteSpace(vData.CopyProductsFromSalesOrderNumbers) Then
                '        oError = "ref: vbAH8JNZ66TL! CopyProductsFromSalesOrderNumbers must not be blank if CopyProductsFromInvoiceID is not blank."
                '        Return Nothing
                '    End If
                'End If
                'Dim pCopyProductsFromSalesOrderNumbers = vData.CopyProductsFromSalesOrderNumbers.Split({","}, StringSplitOptions.RemoveEmptyEntries)
                'If pCopyProductsFromInvoiceIDs.Count <> pCopyProductsFromSalesOrderNumbers.Count Then
                '    oError = "ref: vbCW2DSW2UCW! pCopyProductsFromInvoiceIDs.Count <> pCopyProductsFromSalesOrderNumbers.Count"
                '    Return Nothing
                'End If
                For Each pCopyProductsFromInvoiceID In pCopyProductsFromInvoiceIDs
                    Dim pDrOrder = ById2(pCopyProductsFromInvoiceID)
                    If pDrOrder Is Nothing Then
                        oError = "ref: vbXN5G86G3AQ! Order with ID, " & pCopyProductsFromInvoiceID & ", does not exist"
                        Return Nothing
                    End If
                    Dim pOrderRef = CStr2(pDrOrder("MIN")) & "-" & CStr2(pDrOrder("SalesOrderNo"))
                    'If pCopyProductsFromSalesOrderNumbers.Contains(pOrderRef) = False Then
                    '    oError = "ref: vbKJH5LDEZML! pCopyProductsFromSalesOrderNumbers does not contain " & pOrderRef
                    '    Return Nothing
                    'End If
                    CopyProducts(pCopyProductsFromInvoiceID, pCreator.ID, CBool2(vData.UseUnitPriceFromLibrary), vData.CopyProductsWithVendorID)
                    If vData.SetInvoiceIDProper Then
                        UpdateInvoiceID(pCopyProductsFromInvoiceID, pCreator.ID)
                    End If
                Next
            End If
            If vData.SaveAndFinalizeProper Then
                CacheInvoicesWithCredit(pCreator.ID, pCreator.CustomerID)
                Dim d As New DashBoardsAndReports
                d.OrgID = vOrgID
                d.GetSalesOrderReportFinder("", "inv.ID = " & ReadyString(pCreator.ID), DashBoardsAndReports.eSalesOrderReportMode.Recache,
                    vFowstID:=Nothing,
                    vStatus:=Nothing,
                    vIsInVendorPortal:=False,
                    vOrderDateFrom:=Nothing,
                    vOrderDateTo:=Nothing,
                    vVendorID:=Nothing,
                    vAreas:=Nothing,
                    vSortBy:=Nothing)
            End If
            If vData.ID IsNot Nothing Then
                If vData.SaveAndFinalizeProper Then
                    If vData.TransTypeProper = SaveSalesOrderJson.eTransType.PurchaseOrder _
                        OrElse vData.TransTypeProper = SaveSalesOrderJson.eTransType.AccountsPayable Then

                        mParent.LibraryPriceCanvass.CreateFromPurchaseOrder(vData.ID)
                    End If
                End If
            End If
            UpdateProductTax(pCreator.ID)
            mProData.CommitTransaction()
            Return pCreator.ID
        End If
        mProData.CommitTransaction()

    End Function

    Public Function GetCommonLineItemCount(vInvoiceID1 As String, vInvoiceID2 As String) As Integer

        Dim pSql = " SELECT COUNT(*)" & vbCrLf &
            " FROM TransInvoiceItems TransInvoiceItems_1" & vbCrLf &
            " JOIN TransInvoiceItems TransInvoiceItems_2 ON TransInvoiceItems_2.ProductID = TransInvoiceItems_1.ProductID" & vbCrLf &
            " AND TransInvoiceItems_2.UnitMeasure = TransInvoiceItems_1.UnitMeasure" & vbCrLf &
            " AND TransInvoiceItems_2.PriceLevelID = TransInvoiceItems_1.PriceLevelID" & vbCrLf &
            " AND TransInvoiceItems_2.UnitPrice = TransInvoiceItems_1.UnitPrice" & vbCrLf &
            " WHERE TransInvoiceItems_1.InvoiceID = " & ReadyString(vInvoiceID1) & vbCrLf &
            " AND TransInvoiceItems_2.InvoiceID = " & ReadyString(vInvoiceID2)
        Return CInt2(mProData.ExecuteScalar(pSql))

    End Function

    Public Function CopyProducts(vSourceID As String, vDestinationID As String, vUseUnitPriceFromLibrary As Boolean,
        Optional vVendorID As String = Nothing,
        Optional vUseOrderedQuantity As Boolean = False)

        Dim pSql As String
        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pIsVatable As Boolean
        Dim pIsVatExempt As Boolean

        mProData.BeginTransaction()
        pSql = "SELECT TransInvoiceItems.* " & vbCrLf &
            ",TransInvoice.Area AS HeaderArea" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vSourceID) & vbCrLf
        If vVendorID IsNot Nothing Then
            pSql &= " AND TransInvoiceItems.VendorID = " & ReadyString(vVendorID) & vbCrLf
        End If
        pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            If CDec2(pDR("HiddenServedQuantity")) > 0 Then
                Continue For
            End If
            pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransInvoiceItems"
            pIU.Remove("ID") ' auto
            pIU.Remove("COGS") ' computed
            pIU.Remove("DiscountTax") ' computed
            pIU.Remove("DiscountedUnitPrice") ' computed
            pIU.Remove("GrossSalesNetOfVATExemptions") ' computed
            pIU.Remove("HeaderArea") ' derived column

            pIU("Area") = ReadyString(pDR("HeaderArea"))
            If vUseUnitPriceFromLibrary Then
                Dim pPrice = CDec2(mParent.LibraryStock.GetPrice(CStr2(pDR("ProductID")), CDec2(pDR("ActualQuantity")), CStr2(pDR("PriceLevelID"))))
                If pPrice > 0 Then
                    pIU("UnitPrice") = pPrice
                End If
            End If
            If vUseOrderedQuantity Then
                pDR("SubTotal") = CDec2(pIU("OrderedQuantity")) * CDec2(pIU("UnitPrice"))
            Else
                pDR("SubTotal") = CDec2(pIU("Quantity")) * CDec2(pIU("UnitPrice"))
            End If
            pIU("SubTotal") = pDR("SubTotal")

            pIsVatable = mParent.SystemDefaults.Vatable
            pIU("TaxPercent") = mParent.SystemDefaults.VatPercent
            If pIsVatable = False Then
                pIU("VATableSale") = 0
                pIU("VATExemptSale") = 0
                pIU("NonVATSale") = pDR("SubTotal")
                pIU("ZeroRatedSale") = 0
                pIU("TaxSales") = 0
                pIU("TaxAmount") = 0
            Else
                pIU("VATableSale") = pDR("SubTotal")
                pIU("VATExemptSale") = 0
                pIU("NonVATSale") = 0
                pIU("ZeroRatedSale") = 0
                pIU("TaxSales") = RoundDown(pDR("SubTotal") / (1 + pIU("TaxPercent") / 100), 4)
                pIU("TaxAmount") = pDR("SubTotal") - pIU("TaxSales")
            End If

            pIU("InvoiceID") = ReadyString(vDestinationID)
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next pDR
        mProData.CommitTransaction()

        Return pDT.Rows.Count

    End Function

    'Public Function MoveProducts(vSourceID As String, vDestinationID As String)

    '    Dim pSql As String
    '    Dim pDT As DataTable
    '    Dim pIU As InsertUpdate

    '    mProData.BeginTransaction()
    '    pSql = "SELECT * " & vbCrLf &
    '        " FROM TransInvoiceItems" & vbCrLf &
    '        " WHERE InvoiceID = " & ReadyString(vSourceID) & vbCrLf &
    '        " AND Quantity > 0"
    '    pDT = mProData.Execute(pSql)
    '    For Each pDR As DataRow In pDT.Rows
    '        pIU = New InsertUpdate("TransInvoiceItems")
    '        pIU("Quantity") = "Quantity + " & pDR("Quantity")
    '        pIU("SubTotal") = "(Quantity + " & pDR("Quantity") & ") * UnitPrice"
    '        pIU("InvoiceID") = ReadyString(vDestinationID)
    '        pSql = pIU.GetUpdate("ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
    '            " AND UnitMeasure = " & ReadyString(pDR("UnitMeasure")) & vbCrLf &
    '            " AND InvoiceID = " & ReadyString(vDestinationID))
    '        Dim pAffected = mProData.ExecuteSQL(pSql)
    '        If pAffected = 0 Then
    '            pIU = New InsertUpdate("TransInvoiceItems")
    '            pIU("InvoiceID") = ReadyString(vDestinationID)
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
    '            pAffected = mProData.ExecuteSQL(pSql) ' output is for debugging only
    '        Else
    '            pIU = New InsertUpdate("TransInvoiceItems")
    '            pIU("OrderedQuantity") = "0"
    '            pIU("Quantity") = "0"
    '            pIU("SubTotal") = "0"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
    '            pAffected = mProData.ExecuteSQL(pSql) ' output is for debugging only
    '        End If
    '    Next pDR
    '    UpdateAmounts(vSourceID)
    '    UpdateAmounts(vDestinationID)
    '    mProData.CommitTransaction()

    '    Return pDT.Rows.Count

    'End Function

    ''' <summary>
    ''' this should be in another file
    ''' </summary>
    ''' <param name="vCanvassID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCanvassItems(vCanvassID As String, vVendorID As String) As DataTable

        Dim pOut As DataTable
        Dim pFinder As Finder

        pFinder = New Finder
        pFinder.ProData = mParent.ProData

        With pFinder
            .Distinct = True
            With .Fields
                .Add("ProductID", SqlDbType.UniqueIdentifier, "LibraryPriceCanvassItemSku.SkuID")
                .Add("Qty", SqlDbType.Decimal, "LibraryPriceCanvassItems.Qty")
                .Add("Price", SqlDbType.Decimal, "LibraryPriceCanvassVendorPrice.Price")
                .Add("Unit", SqlDbType.VarChar, "MobizLibraryCountriesLanguageLabel.Label")
                .Add("UnitID", SqlDbType.UniqueIdentifier, "LibraryPriceCanvassItems.UnitOfMeasure")
                .Add("ProductNo", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductNo")
                .Add("ProductName", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductName")
            End With
            .SqlFrom = " FROM LibraryPriceCanvass" & vbCrLf &
                " JOIN LibraryPriceCanvassItems ON LibraryPriceCanvassItems.CanvassID = LibraryPriceCanvass.ID" & vbCrLf &
                " JOIN LibraryPriceCanvassItemSku ON LibraryPriceCanvassItemSku.CanvassItemID = LibraryPriceCanvassItems.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryPriceCanvassItemSku.SkuID" & vbCrLf &
                " JOIN LibraryPriceCanvassVendor ON LibraryPriceCanvassVendor.CanvassID = LibraryPriceCanvass.ID" & vbCrLf &
                " JOIN LibraryPriceCanvassVendorPrice ON LibraryPriceCanvassVendorPrice.CanvassItemSkuID = LibraryPriceCanvassItemSku.ID" & vbCrLf &
                " AND LibraryPriceCanvassVendorPrice.CanvassVendorID = LibraryPriceCanvassVendor.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = LibraryPriceCanvassItems.UnitOfMeasure" & vbCrLf &
                " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf
            .SqlWhere = " WHERE LibraryPriceCanvass.ID = " & ReadyString(vCanvassID) & vbCrLf &
                " AND LibraryPriceCanvassVendor.VendorID = " & ReadyString(vVendorID)
            .SqlOrder = " ORDER BY MobizLibraryPIMProduct.ProductName" & vbCrLf
            pOut = .Requery()
        End With

        Return pOut

    End Function

    Public Function CanvassItemsToStockTable(vSourceCanvassID As String, vSourceVendorID As String) As DataTable

        Dim pOut As DataTable
        Dim pDtCanvassItems As DataTable
        Dim pDrOut As DataRow
        Dim pRetailID As String

        pOut = CreateStockTable()
        pRetailID = mParent.SystemDefaults.GetRetailPriceID
        pDtCanvassItems = GetCanvassItems(vSourceCanvassID, vSourceVendorID)
        For Each pDrCanvassItem As DataRow In pDtCanvassItems.Rows
            pDrOut = pOut.NewRow
            pDrOut("StockID") = pDrCanvassItem("ProductID")
            pDrOut("Quantity") = 1
            pDrOut("OrderedQuantity") = 0
            pDrOut("ActualQuantity") = pDrCanvassItem("Qty")
            pDrOut("UnitMeasure") = pDrCanvassItem("Unit")
            pDrOut("UnitPrice") = pDrCanvassItem("Price")
            pDrOut("PriceLevelID") = pRetailID
            pDrOut("Subtotal") = 0 ' CDec2(pDrOut("Quantity")) * CDec2(pDrCanvassItem("Price"))
            pDrOut("Dimension") = ""
            pDrOut("IsReplacement") = False
            pDrOut("IsHasReplacement") = False
            pDrOut("IsHasBackOrder") = False
            pDrOut("RefStockID") = ""
            pDrOut("Cost") = 0
            pOut.Rows.Add(pDrOut)
        Next pDrCanvassItem

        Return pOut

    End Function

    Public Function UpdateOrderQtyFromOrderToInvoice(vID As String, vStockID As String) As Boolean

        Dim pDrInvoice As DataRow
        Dim pSql As String
        Dim pDtOrder, pDtInvoice As DataTable
        Dim pDtUniqueActualQuantities As DataTable
        Dim pIU As InsertUpdate
        Dim pOrderedQty As Decimal
        Dim pDvOrder As DataView

        pDrInvoice = ById(vID)
        If pDrInvoice Is Nothing Then
            Return False
        End If
        If pDrInvoice("SalesOrderID") Is DBNull.Value Then
            Return False
        End If

        pSql = " SELECT TransInvoiceItems.ProductID" & vbCrLf &
            " ,SUM(TransInvoiceItems.OrderedQuantity) AS Quantity" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(pDrInvoice("SalesOrderID")) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vStockID) & vbCrLf &
            " GROUP BY TransInvoiceItems.ProductID, TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ORDER BY TransInvoiceItems.ActualQuantity"
        pDtOrder = mProData.Execute(pSql)
        If pDtOrder.Rows.Count = 0 Then
            Return False
        End If
        pDtOrder.Columns.Add("Consumed", GetType(Decimal))

        pSql = " SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.Quantity" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vStockID) & vbCrLf &
            " ORDER BY TransInvoiceItems.ActualQuantity DESC"
        pDtInvoice = mProData.Execute(pSql)
        If pDtInvoice.Rows.Count = 0 Then
            Return False
        End If
        pDtInvoice.Columns.Add("OrderedQuantity", GetType(Decimal))

        ' PHASE 1: assign ordered quantity up to the served quantity. same actual quantity
        For Each pDrInvoiceItem As DataRow In pDtInvoice.Rows
            pDvOrder = New DataView(pDtOrder, "ActualQuantity = " & pDrInvoiceItem("ActualQuantity") &
                " AND Quantity - ISNULL(Consumed, 0) > 0",
                "", DataViewRowState.CurrentRows)
            If pDvOrder.Count = 0 Then
                Continue For
            End If
            pOrderedQty = pDrInvoiceItem("Quantity")
            If pDvOrder(0)("Quantity") < pOrderedQty Then
                pOrderedQty = pDvOrder(0)("Quantity")
            End If
            pDvOrder(0)("Consumed") = CDec2(pDvOrder(0)("Consumed")) + pOrderedQty
            pDrInvoiceItem("OrderedQuantity") = CDec2(pDrInvoiceItem("OrderedQuantity")) + pOrderedQty
        Next pDrInvoiceItem

        ' PHASE 2: assign remaining ordered quantity to invoice item. same actual quantity
        For Each pDrInvoiceItem As DataRow In pDtInvoice.Rows
            pDvOrder = New DataView(pDtOrder, "ActualQuantity = " & pDrInvoiceItem("ActualQuantity") &
                " AND Quantity - ISNULL(Consumed, 0) > 0",
                "", DataViewRowState.CurrentRows)
            If pDvOrder.Count = 0 Then
                Continue For
            End If
            pOrderedQty = pDvOrder(0)("Quantity") - CDec2(pDvOrder(0)("Consumed"))
            pDvOrder(0)("Consumed") = CDec2(pDvOrder(0)("Consumed")) + pOrderedQty
            pDrInvoiceItem("OrderedQuantity") = CDec2(pDrInvoiceItem("OrderedQuantity")) + pOrderedQty
        Next pDrInvoiceItem

        ' LAST PHASE: update the invoice item's ordered quantity
        mProData.BeginTransaction()
        For Each pDrInvoiceItem As DataRow In pDtInvoice.Rows
            pIU = New InsertUpdate("TransInvoiceItems")
            pIU("OrderedQuantity") = CDec2(pDrInvoiceItem("OrderedQuantity"))
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrInvoiceItem("ID")))
            mProData.ExecuteSQL(pSql)
        Next pDrInvoiceItem
        mProData.CommitTransaction()

        Return True

    End Function

    Public Function IsCncApprovable(vInvoiceID As String, ByRef oMessage As String) As Boolean

        Dim pSql As String
        Dim pDR As DataRow

        oMessage = ""
        pSql = " SELECT CnCUnapprovedReason" & vbCrLf &
            " ,CncApprovedByID" & vbCrLf &
            " ,CncDisapprovedByID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oMessage = "The specified transaction does not exist."
            Return False
        End If
        If CStr2(pDR("CnCUnapprovedReason")) = "" Then
            oMessage = "The specified transaction does not require an approval."
            Return False
        End If
        If pDR("CncApprovedByID") IsNot DBNull.Value Then
            oMessage = "The specified transaction was already approved."
            Return False
        End If
        If pDR("CncdisapprovedByID") IsNot DBNull.Value Then
            oMessage = "The specified transaction was already disapproved."
            Return False
        End If

        Return True

    End Function

    Public Function IsProfitApprovable(vInvoiceID As String) As Boolean

        Dim pSql = "SELECT UnapprovedProfitability" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return False
        End If

        Return CBool2(pDR("UnapprovedProfitability"))

    End Function

    Public Function SetCnCUnapprovedReason(vInvoiceID As String, vValue As String) As Integer

        Dim pIU As New InsertUpdate("TransInvoice")
        pIU("CnCUnapprovedReason") = ReadyString(vValue)
        Dim pSql As String = pIU.GetUpdate("ID = " & ReadyString(vInvoiceID))
        Dim pOut As Integer = mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Function CncApproveSalesOrder(vInvoiceID As String, vUserID As String, ByRef oMessage As String, vOrgID As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        If IsCncApprovable(vInvoiceID, oMessage) = False Then
            Return False
        End If

        mProData.BeginTransaction()
        pIU = New InsertUpdate("TransInvoice")
        pIU("CncApprovedByID") = ReadyString(vUserID)
        pIU("CncApprovedDate") = "GETDATE()"
        pIU("FinalizedForApproval") = "0"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceID))
        mProData.ExecuteSQL(pSql)

        mParent.TransInvoice.FinalizeWebPos(vInvoiceID, vOrgID, Nothing)
        mProData.CommitTransaction()

        Return True

    End Function

    Public Function CncDisapproveSalesOrder(vInvoiceID As String, vUserID As String, ByRef oMessage As String, vOrgID As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        oMessage = Nothing
        If IsCncApprovable(vInvoiceID, oMessage) = False Then
            Return False
        End If

        mProData.BeginTransaction()
        pIU = New InsertUpdate("TransInvoice")
        pIU("CncDisapprovedByID") = ReadyString(vUserID)
        pIU("CncDisapprovedDate") = "GETDATE()"
        pIU("FinalizedForApproval") = "0"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceID))
        mProData.ExecuteSQL(pSql)

        If mParent.TransInvoice.CancelInvoice(vInvoiceID, vUserID, vOrgID, oMessage) Then
            mProData.CommitTransaction()
        Else
            mProData.RollbackTransaction()
        End If

        Return True

    End Function

    Public Function ApprovePo(vID As String, vUserID As String, ByRef oMessage As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        If IsApprovable(vID, oMessage) = False Then
            Return False
        End If

        pIU = New InsertUpdate("TransInvoice")
        pIU("ApprovedDate") = "GETDATE()"
        pIU("ApprovedByID") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

        Return True

    End Function

    Public Function IsApprovable(vID As String, ByRef oMessage As String) As Boolean

        Dim pSql = "SELECT ApprovedDate" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oMessage = "Transaction with the specified ID does not exist. ref: vbKEUGZFWFCZ!"
            Return False
        End If
        If pDR("ApprovedDate") IsNot DBNull.Value Then
            oMessage = "This transaction was already approved on " & Format(pDR("ApprovedDate"), cGlobal.FormatDateTime)
            Return False
        End If

        Return True

    End Function

    Public Class SaveSalesOrderJson
        Public ID As String
        Public CustomerID As String
        Public FowstID As String
        Public DeviceID As String
        Public BillingAddressID As String
        Public DeliveryAddressID As String
        Public SalesPartnerOrgID As String
        Public SalesRepPeopleID As String
        Public SalesRepText As String
        Public RefInvoiceNum As String
        'Public Products As List(Of ProductClass)
        Public UserID As String
        Public TransType As String
        Public Enum eTransType
            Quote
            SalesOrder
            Invoice
            PurchaseOrder
            AccountsPayable
            StockTransfer
            ExpenseTicket
        End Enum
        Public TransTypeProper As eTransType
        Public IsPos As String ' boolean
        Public SaveAndFinalize As String
        Public FinalizedForApproval As Boolean
        Public DateNeeded As String
        Public Remarks As String
        Public InternalRemarks As String
        ''' <summary>
        ''' comma-separated
        ''' </summary>
        ''' <remarks></remarks>
        Public CopyProductsFromInvoiceID As String
        Public CopyProductsFromSalesOrderNumbers As String
        Public CopyProductsWithVendorID As String
        Public PurchaseOrderNo As String
        Public PurchaseOrderText As String
        Public PurchaseOrderDate As String
        Public SalesOrderID As String
        Public QuotationID As String
        Public QuotationNo As String
        Public QuotationDate As String
        Public QuotationNumText As String
        Public DocumentNameID As String
        Public PaymentTerm As String
        Public CanvassID As String
        Public CopyFromCanvass As String ' boolean
        Public CustomerFowstID As String
        Public CustomerBinID As String
        Public UneditedBackorder As String
        Public BackorderSourceInvoiceID As String
        Public DeliveryMethod As String
        Public OrderedByID As String
        Public InvoiceFowstID As String
        Public ConsolidateSalesOrder As String ' tri-state bool
        Public UseUnitPriceFromLibrary As String ' bool
        Public DontAutoInvoice As String ' bool
        Public TransDate As String ' date
        Public SalesInvoiceRef As String
        Public OfficialReceiptRef As String
        Public StockTransferID As String

        '// "Freight and Handling" tab
        Public ShipperID As String
        Public WaybillNumber As String
        Public ShipDate As String ' date
        Public PackageQuantity As String
        Public GrossWeight As String
        Public GrossVolume As String
        Public Freight As String ' money
        Public Handling As String ' money
        Public Arrastre As String ' money
        Public OtherCharges As String ' money

        Public SaveSalesRep As Boolean ' set on server side

        Public ReadOnly Property FreightAndHandling As Decimal
            Get
                Return CDec2(Freight) +
                        CDec2(Handling) +
                        CDec2(Arrastre) +
                        CDec2(OtherCharges)
            End Get
        End Property

        ''' <summary>
        ''' if true, records, with IDs from CopyProductsFromInvoiceID, will have their InvoiceID set to the generated ID
        ''' </summary>
        ''' <remarks></remarks>
        Public SetInvoiceID As String ' bool

        Public Function Verify(ByRef oError As String) As Boolean

            If String.IsNullOrWhiteSpace(CustomerID) Then
                oError = "CustomerID must not be blank."
                Return False
            End If

            If IsGuid(CustomerID) = False Then
                oError = "CustomerID must be a valid GUID."
                Return False
            End If

            'If Products Is Nothing _
            '    OrElse Products.Count = 0 Then

            '    oError = "Please include at least 1 product."
            '    Return False
            'End If

            If String.IsNullOrWhiteSpace(FowstID) Then
                oError = "FowstID must not be blank."
                Return False
            End If

            If IsGuid(FowstID) = False Then
                oError = "FowstID must be a valid GUID."
                Return False
            End If

            If String.IsNullOrWhiteSpace(BillingAddressID) Then
                BillingAddressID = Guid.Empty.ToString()
            End If

            If ReadyGuid(BillingAddressID) = ReadyGuid(Guid.Empty) Then
                If SaveAndFinalize Then
                    oError = "Billing Address must not be blank."
                    Return False
                End If
            End If

            If IsGuid(BillingAddressID) = False Then
                oError = "Billing Address must be a valid GUID."
                Return False
            End If

            If String.IsNullOrWhiteSpace(DeliveryAddressID) Then
                DeliveryAddressID = Guid.Empty.ToString()
            End If

            If ReadyGuid(DeliveryAddressID) = ReadyGuid(Guid.Empty) Then
                If SaveAndFinalize Then
                    oError = "Delivery Address must not be blank."
                    Return False
                End If
            End If

            If IsGuid(DeliveryAddressID) = False Then
                oError = "Delivery Address must be a valid GUID."
                Return False
            End If

            If [Enum].GetNames(GetType(eTransType)).Contains(TransType) = False Then
                oError = "Invalid TransType, " & TransType & "."
                Return False
            End If
            TransTypeProper = [Enum].Parse(GetType(eTransType), TransType)

            'If String.IsNullOrWhiteSpace(QuotationNo) = False Then
            '    If IsNumeric() Then
            '    End If
            Return True

        End Function

        Public ReadOnly Property SetInvoiceIDProper As Boolean
            Get
                Return CBool2(SetInvoiceID)
            End Get
        End Property

        Public ReadOnly Property SaveAndFinalizeProper As Boolean
            Get
                Return CBool2(SaveAndFinalize)
            End Get
        End Property

        Public ReadOnly Property CopyFromCanvassProper As Boolean
            Get
                Return CBool2(CopyFromCanvass)
            End Get
        End Property

        Public Class ProductClass
            Public ProductID As String
            Public Quantity As String
            Public Unit As String
            Public UnitPrice As String
            Public PriceLevelID As String
            Public UnitQty As String

            Public Function Verify(ByRef oError As String) As Boolean

                If String.IsNullOrWhiteSpace(ProductID) Then
                    oError = "ProductID must not be blank."
                    Return False
                End If

                If IsGuid(ProductID) = False Then
                    oError = "ProductID must be a GUID."
                    Return False
                End If

                If String.IsNullOrWhiteSpace(UnitPrice) Then
                    oError = "UnitPrice must not be black."
                    Return False
                End If

                If IsNumeric(UnitPrice) = False Then
                    oError = "UnitPrice must be a valid amount."
                    Return False
                End If

                If IsGuid(Unit) = False Then

                End If

                If CDec2(UnitPrice) < 0 Then
                    oError = "UnitPrice cannot be less than zero."
                    Return False
                End If

                If String.IsNullOrWhiteSpace(PriceLevelID) Then
                    oError = "PriceLevelID must not be blank."
                    Return False
                End If

                If IsGuid(PriceLevelID) = False Then
                    oError = "PriceLevelID must be a GUID."
                    Return False
                End If

                Return True

            End Function
        End Class

    End Class

    ''' <summary>
    ''' vb version of SalesMenu.js GetInvoiceDetails
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalesMenuGetInvoiceDetails(vTableNum As String) As DataTable

        Dim pDT As DataTable
        Dim pFinder As Finder
        Dim pSelect As String
        Dim pFrom As String

        pFinder = New Finder
        pFinder.ProData = mProData

        BrowserController.ComposeCustomer(pSelect, pFrom)
        With pFinder
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransInvoice.ID")
                .Add("CustomerID", SqlDbType.UniqueIdentifier, "TransInvoice.CustomerID")
                .Add("CompanyName", SqlDbType.VarChar, pSelect)
                .Add("InvoiceDate", SqlDbType.DateTime, "TransInvoice.InvoiceDate")
                With .Add("Total", SqlDbType.Decimal, "(SELECT SUM(SubTotal) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)")
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int, "(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)")
                    .Searchable = False
                End With
                With .Add("TotalOrder", SqlDbType.Decimal, "(SELECT SUM(SubTotal) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)")
                    .Searchable = False
                End With
                With .Add("SkuCountOrder", SqlDbType.Int, "(SELECT COUNT(ID) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)")
                    .Searchable = False
                End With
                .Add("SalesOrderNo", SqlDbType.Int, "ISNULL(SalesOrderNo, 0)")
                .Add("UserID", SqlDbType.UniqueIdentifier, "TransInvoice.UserID")
                .Add("EmployeeFullName", SqlDbType.VarChar, "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName")
                .Add("FowstID", SqlDbType.UniqueIdentifier, "TransInvoice.FowstID")
                .Add("FOWSTName", SqlDbType.VarChar, "MobizLibraryOrganizationFOWST.Name")
                .Add("DeviceName", SqlDbType.VarChar, "LibraryDevice.PhoneName")
                .Add("CurrentUserID", SqlDbType.UniqueIdentifier, ReadyString(WebSession.Instance.PeopleUserID))
                .Add("MIN", SqlDbType.VarChar, "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)")
                .Add("TableNo", SqlDbType.Int, "TransInvoice.CustomerTableNo")
                .Add("DeliveryMethod", SqlDbType.VarChar, "TransInvoice.DeliveryMethod")
                .Add("OrderMethod", SqlDbType.VarChar, "TransInvoice.OrderMethod")
                .Add("CustomerDemographicGender", SqlDbType.VarChar, "TransInvoice.CustomerDemographicGender")
                .Add("CustomerDemographicAge", SqlDbType.VarChar, "TransInvoice.CustomerDemographicAge")
                .Add("TotalNumberOfCustomer", SqlDbType.Int, "TransInvoice.TotalNumberOfCustomer")
                .Add("SearchSO", SqlDbType.VarChar, "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, NULLIF(SalesOrderNo,0)) ")
                .Add("SalesOrderNoRemoveDash", SqlDbType.VarChar, "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + LibraryDevice.DeviceNo + CONVERT(varchar, NULLIF(SalesOrderNo,0)) ")
            End With
            .SqlFrom = " FROM TransInvoice" & vbCrLf &
                " JOIN MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID" & vbCrLf &
                " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                " LEFT JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID" & vbCrLf
            .SqlFrom &= pFrom
            .SqlWhere = " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
                " AND TransInvoice.CustomerTableNo = " & ReadyString("TBL_" + vTableNum) & vbCrLf &
                " AND TransInvoice.InvoiceNumber = 0" & vbCrLf &
                " AND EXISTS(SELECT TransInvoiceItems.ID" & vbCrLf &
                "   FROM TransInvoiceItems" & vbCrLf &
                "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)" & vbCrLf &
                " AND NOT EXISTS(SELECT TransInvoice.ID" & vbCrLf &
                "   FROM TransInvoice" & vbCrLf &
                "   WHERE TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID)" & vbCrLf
            .SqlOrder = " ORDER BY TransInvoice.LastUpdated DESC"
            pDT = .Requery()
        End With

        Return pDT

    End Function

    Public Function MostCommonFowstID() As String

        Dim pSql As String
        Dim pOut As String

        pSql = " SELECT TOP 1 FowstID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " GROUP BY FowstID" & vbCrLf &
            " ORDER BY COUNT(*) DESC"
        pOut = CStr2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function MostCommonDeviceID() As String

        Dim pSql As String
        Dim pOut As String

        pSql = " SELECT TOP 1 DeviceID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " GROUP BY DeviceID" & vbCrLf &
            " ORDER BY COUNT(*) DESC"
        pOut = CStr2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function HideServedQuantity(vInvoiceItemID As String, vUserID As String,
        Optional vHideQuantity As Boolean = False) As Integer

        Dim pOut As Integer
        Dim pIU As InsertUpdate
        Dim pSql As String

        mProData.BeginTransaction()
        pIU = New InsertUpdate("TransInvoiceItems")
        If vHideQuantity Then
            pIU("HiddenServedQuantity") = "Quantity"
        Else
            pIU("HiddenServedQuantity") = "OrderedQuantity"
        End If
        pIU("Quantity") = "0"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceItemID))
        pOut = mProData.ExecuteSQL(pSql)
        If pOut <> 1 Then
            mProData.RollbackTransaction()
            Return pOut
        End If

        pOut = UpdateInvoiceFromItem(vInvoiceItemID, vUserID)
        If pOut <> 1 Then
            mProData.RollbackTransaction()
            Return pOut
        End If

        mProData.CommitTransaction()

        Return 1

    End Function

    Public Function ReaddHideServedQuantity(vInvoiceItemID As String, vUserID As String) As Integer

        Dim pOut As Integer
        Dim pIU As InsertUpdate
        Dim pSql As String

        mProData.BeginTransaction()
        pIU = New InsertUpdate("TransInvoiceItems")
        pIU("Quantity") = "HiddenServedQuantity"
        pIU("HiddenServedQuantity") = "0"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vInvoiceItemID))
        pOut = mProData.ExecuteSQL(pSql)
        If pOut <> 1 Then
            mProData.RollbackTransaction()
            Return pOut
        End If

        pOut = UpdateInvoiceFromItem(vInvoiceItemID, vUserID)
        If pOut <> 1 Then
            mProData.RollbackTransaction()
            Return pOut
        End If

        mProData.CommitTransaction()

        Return 1

    End Function

    Private Function UpdateInvoiceFromItem(vInvoiceItemID As String, vUserID As String) As Integer

        Dim pOut As Integer
        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransInvoice")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(vUserID)
        pSql = "SELECT InvoiceID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceItemID)
        pSql = pIU.GetUpdate("ID IN (" & pSql & ")")
        pOut = mProData.ExecuteSQL(pSql)
        Return pOut

    End Function

    Public Function IsCancelled(vID As String, ByRef oError As String) As Boolean

        Dim pDR As DataRow

        oError = Nothing
        pDR = ById(vID)
        If pDR Is Nothing Then
            oError = "Invoice does not exist."
            Return False
        End If

        Return CBool2(pDR("IsCancelled"))

    End Function

    Public Function CancelInvoice(vID As String, vUserID As String, vOrgID As String, ByRef oError As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        oError = Nothing
        mProData.BeginTransaction()
        If IsCancelled(vID, oError) Then
            If oError Is Nothing Then
                oError = "Invoice was already cancelled."
            End If
            mProData.RollbackTransaction()
            Return False
        End If
        pIU = New InsertUpdate("TransInvoice")
        pIU("IsCancelled") = 1
        pIU("CancelledByID") = ReadyString(vUserID)
        pIU("DateCancelled") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)
        AddToInventoryLedger(vID, vOrgID, , TransInventoryLedger.eTransType.CancelInvoice)
        CacheInvoicesWithCredit()
        UnTagItemRequisition(vID)
        mProData.CommitTransaction()

        Return True

    End Function

    Private Function UnTagItemRequisition(vPurchaseOrderID As String)

        Dim pIU = New InsertUpdate("TransItemRequisitions")
        pIU("CreatedPurchaseOrderID") = "NULL"
        Dim pSql = pIU.GetUpdate("CreatedPurchaseOrderID = " & ReadyString(vPurchaseOrderID))
        pSql = mProData.ExecuteSQL(pSql) ' output is used to debugging only

    End Function

    Public Shared Function CreateStockTable() As DataTable

        Dim pDtStock As DataTable = New DataTable
        With pDtStock.Columns
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Decimal))
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Decimal))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("TradeDiscountLess")
            .Add("NetDiscountFactor", GetType(Decimal))
            .Add("NetDiscountRate", GetType(Decimal))
            .Add("TermNumericValue", GetType(Decimal))
            .Add("VendorID")
            .Add("Upcharge", GetType(Decimal)) 'in percentage format --- added by arnie 12/22/2020
            '.Add("SalesRep", GetType(Object))
        End With

        Return pDtStock

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vPaymentDateFrom"></param>
    ''' <param name="vPaymentDateTo"></param>
    ''' <param name="vMaturityDateFrom"></param>
    ''' <param name="vMaturityDateTo"></param>
    ''' <param name="vExcludeCash"></param>
    ''' <param name="vExcludeChecks"></param>
    ''' <param name="vExcludeCreditCard"></param>
    ''' <param name="vExcludeDebitCard"></param>
    ''' <param name="vExcludeSrv"></param>
    ''' <param name="vCustomerID"></param>
    ''' <param name="vExcludeDeposited"></param>
    ''' <param name="vExcludeReturned"></param>
    ''' <param name="vExcludeUndeposited"></param>
    ''' <param name="vDepositGroupID">if set, other filters are ignored</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CustomerPaymentReport(vPaymentDateFrom As Date?, vPaymentDateTo As Date?,
        vMaturityDateFrom As Date?,
        vMaturityDateTo As Date?,
        vDepositDateFrom As Date?,
        vDepositDateTo As Date?,
        vExcludeCash As Boolean,
        vExcludeChecks As Boolean,
        vExcludeCreditCard As Boolean,
        vExcludeDebitCard As Boolean,
        vExcludeSrv As Boolean,
        vCustomerID As String,
        vExcludeDeposited As Boolean,
        vExcludeReturned As Boolean,
        vExcludeUndeposited As Boolean,
        vDepositGroupID As String,
        vUserID As String,
        vSalesRepID As String,
        vSalesChannelID As String,
        vSearchKeywords As String,
        vGroupByHeader As Boolean,
        vPaymentFowstID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String
        Dim pCustomerSelect As String
        Dim pCustomerFrom As String

        BrowserController.ComposeCustomer(pCustomerSelect, pCustomerFrom)
        pSql = " -- payments report" & vbCrLf &
            " WITH CTE AS (" & vbCrLf &
            " SELECT TransCustomerPayment.ID" & vbCrLf &
            ",TransCustomerPayment.ID AS CustomerPaymentID" & vbCrLf &
            " ,'Cash' AS PaymentType" & vbCrLf &
            " ,NULL AS RefNum" & vbCrLf &
            " ,NULL AS BankID" & vbCrLf &
            " ,NULL AS BankName" & vbCrLf &
            " ,NULL AS MaturityDate" & vbCrLf &
            " ,CashAmount - CustomerChangeCash AS AmountPaid" & vbCrLf &
            " ,TransCustomerPayment.CashDepositStatus AS DepositStatus" & vbCrLf &
            " ,TransCustomerPayment.CashDepositDate AS DepositDate" & vbCrLf &
            " ,TransCustomerPayment.DepositoryBankID" & vbCrLf &
            " ,TransCustomerPayment.DepositoryBankBranch" & vbCrLf &
            " ,TransCustomerPayment.DepositoryAccountName" & vbCrLf &
            " ,TransCustomerPayment.DepositoryAccountNumber" & vbCrLf &
            " ,TransCustomerPayment.CashDepositGroupID AS DepositGroupID" & vbCrLf &
            " ,TransCustomerPayment.TimeChecked" & vbCrLf &
            " ,TransCustomerPayment.CashDepositedBy AS DepositedBy" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " WHERE CashAmount - CustomerChangeCash > 0" & vbCrLf &
            " UNION " &
            " SELECT TransCustomerPaymentChecks.ID" & vbCrLf &
            ",CustomerPaymentID" & vbCrLf &
            " ,'Check' AS PaymentType" & vbCrLf &
            " ,'Check# ' + CheckNumber AS RefNum" & vbCrLf &
            " ,BankID" & vbCrLf &
            " ,LEFT(TransCustomerPaymentChecks.BankName, 100) AS BankName" & vbCrLf &
            " ,CheckDate AS MaturityDate" & vbCrLf &
            " ,CheckAmount AS AmountPaid" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositStatus" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositsDepositDate AS DepositDate" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositoryBankID" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositoryBankBranch" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositoryAccountName" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositoryAccountNumber" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositGroupID" & vbCrLf &
            " ,TransCustomerPaymentChecks.TimeChecked" & vbCrLf &
            " ,TransCustomerPaymentChecks.DepositedBy AS DepositedBy" & vbCrLf &
            " FROM TransCustomerPaymentChecks" & vbCrLf &
            " UNION" & vbCrLf &
            " SELECT TransCustomerPaymentCreditCard.ID" & vbCrLf &
            ",CustomerPaymentID" & vbCrLf &
            " ,'Credit Card' AS PaymentType" & vbCrLf &
            " ,'Approval# ' + TransCustomerPaymentCreditCard.ApprovalNo AS RefNum" & vbCrLf &
            " ,CreditCardBankID AS BankID" & vbCrLf &
            " ,LEFT(TransCustomerPaymentCreditCard.CreditCardBankName, 100) AS BankName" & vbCrLf &
            " ,NULL AS MaturityDate" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.CreditCardAmount AS AmountPaid" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositStatus" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositsDepositDate AS DepositDate" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositoryBankID" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositoryBankBranch" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositoryAccountName" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositoryAccountNumber" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositGroupID" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.TimeChecked" & vbCrLf &
            " ,TransCustomerPaymentCreditCard.DepositedBy AS DepositedBy" & vbCrLf &
            " FROM TransCustomerPaymentCreditCard" & vbCrLf &
            " UNION" & vbCrLf &
            " SELECT TransCustomerPaymentDebitCard.ID" & vbCrLf &
            ",CustomerPaymentID" & vbCrLf &
            " ,'Debit Card' AS PaymentType" & vbCrLf &
            " ,'Approval# ' + TransCustomerPaymentDebitCard.ApprovalNo AS RefNum" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DebitCardBankID AS BankID" & vbCrLf &
            " ,LEFT(TransCustomerPaymentDebitCard.DebitCardBankName, 100) AS BankName" & vbCrLf &
            " ,NULL AS MaturityDate" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DebitCardAmount AS AmountPaid" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositStatus" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositsDepositDate AS DepositDate" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositoryBankID" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositoryBankBranch" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositoryAccountName" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositoryAccountNumber" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositGroupID" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.TimeChecked" & vbCrLf &
            " ,TransCustomerPaymentDebitCard.DepositedBy AS DepositedBy" & vbCrLf &
            " FROM TransCustomerPaymentDebitCard" & vbCrLf &
            "/* UNION" & vbCrLf &
            "  SELECT TransSalesReturnLedger.ID" & vbCrLf &
            ",DestinationPaymentID AS CustomerPaymentID" & vbCrLf &
            "  ,'SRV' AS PaymentType" & vbCrLf &
            "  ,'SRV ' + CONVERT(varchar, SrvNum) AS RefNum" & vbCrLf &
            "  ,NULL AS BankID" & vbCrLf &
            "  ,NULL AS BankName" & vbCrLf &
            "  ,NULL AS MaturityDate" & vbCrLf &
            "  ,-Amount AS AmountPaid" & vbCrLf &
            " ,NULL AS DepositStatus" & vbCrLf &
            " ,NULL AS DepositDate" & vbCrLf &
            " ,NULL AS DepositedBy" & vbCrLf &
            "  FROM TransSalesReturnLedger" & vbCrLf &
            "*/ )" & vbCrLf

        If vGroupByHeader Then
            pSql &= ", CTE2 AS (" & vbCrLf
        End If

        pSql &= " SELECT DISTINCT TOP 100 PERCENT TransCustomerPayment.PaymentDate" & vbCrLf &
            " ," & pCustomerSelect & " AS CompanyName" & vbCrLf &
            " ,ISNULL(CTE.MaturityDate, TransCustomerPayment.PaymentDate) AS MaturityDateFilter" & vbCrLf &
            " ,CTE.*" & vbCrLf &
            " ,orgBank.CompanyName AS DepositoryBankName" & vbCrLf &
            "  ,CASE WHEN PaymentType = 'Cash' Then AmountPaid END AS CashAmount" & vbCrLf &
            "  ,CASE WHEN PaymentType = 'Check' Then AmountPaid END AS CheckAmount" & vbCrLf &
            "  ,CASE WHEN PaymentType = 'Credit Card' Then AmountPaid END AS CreditCardAmount" & vbCrLf &
            "  ,CASE WHEN PaymentType = 'Debit Card' Then AmountPaid END AS DebitCardAmount" & vbCrLf &
            "," & BrowserController.ComposePeopleName("tblPeopleDepositor", "tblPeoplePreNominalSuffixDepositor") & " AS DepositedByName" & vbCrLf &
            " FROM CTE" & vbCrLf &
            " JOIN TransCustomerPayment ON TransCustomerPayment.ID = CTE.CustomerPaymentID" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransCustomerPayment.CustomerID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople tblPeopleDepositor ON tblPeopleDepositor.ID = CTE.DepositedBy" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblPeoplePreNominalSuffixDepositor ON tblPeopleDepositor.TitleID = tblPeoplePreNominalSuffixDepositor.ID" & vbCrLf
        If String.IsNullOrWhiteSpace(vSalesRepID) = False Then
            pSql &= " JOIN LibraryOrganizationCustomerSalesReps ON LibraryOrganizationCustomerSalesReps.OrganizationID = TransCustomerPayment.CustomerID" & vbCrLf &
                " AND LibraryOrganizationCustomerSalesReps.PeopleID = " & ReadyString(vSalesRepID) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vSalesChannelID) = False Then
            pSql &= " JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = TransCustomerPayment.CustomerID" & vbCrLf &
                "  AND LibraryOrganizationCustomer.SalesChannelID = " & ReadyString(vSalesChannelID) & vbCrLf
        End If
        pSql &= " LEFT JOIN MobizLibraryOrganization orgBank ON orgBank.ID = CTE.DepositoryBankID" & vbCrLf &
            pCustomerFrom &
            " WHERE 1 = 1" & vbCrLf
        If String.IsNullOrWhiteSpace(vDepositGroupID) Then
            If vPaymentDateFrom IsNot Nothing Then
                pSql &= " AND CONVERT(date, PaymentDate) >= " & ReadyString(vPaymentDateFrom) & vbCrLf
            End If
            If vPaymentDateTo IsNot Nothing Then
                pSql &= " AND CONVERT(date, PaymentDate) <= " & ReadyString(vPaymentDateTo) & vbCrLf
            End If
            If vMaturityDateFrom IsNot Nothing Then
                pSql &= " AND CONVERT(date, ISNULL(CTE.MaturityDate, TransCustomerPayment.PaymentDate)) >= " & ReadyString(vMaturityDateFrom) & vbCrLf
            End If
            If vMaturityDateTo IsNot Nothing Then
                pSql &= " AND CONVERT(date, ISNULL(CTE.MaturityDate, TransCustomerPayment.PaymentDate)) <= " & ReadyString(vMaturityDateTo) & vbCrLf
            End If

            If vDepositDateFrom IsNot Nothing Then
                pSql &= " AND CONVERT(date, DepositDate) >= " & ReadyString(vDepositDateFrom) & vbCrLf
            End If
            If vDepositDateTo IsNot Nothing Then
                pSql &= " AND CONVERT(date, DepositDate) <= " & ReadyString(vDepositDateTo) & vbCrLf
            End If

            If vExcludeCash Then
                pSql &= " AND PaymentType <> 'Cash'" & vbCrLf
            End If
            If vExcludeChecks Then
                pSql &= " AND PaymentType <> 'Check'" & vbCrLf
            End If
            If vExcludeCreditCard Then
                pSql &= " AND PaymentType <> 'Credit Card'" & vbCrLf
            End If
            If vExcludeDebitCard Then
                pSql &= " AND PaymentType <> 'Debit Card'" & vbCrLf
            End If
            If vExcludeSrv Then
                pSql &= " AND PaymentType <> 'SRV'" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(vCustomerID) = False Then
                pSql &= " AND TransCustomerPayment.CustomerID = " & ReadyString(vCustomerID)
            End If
            If String.IsNullOrWhiteSpace(vPaymentFowstID) = False Then
                pSql &= " AND TransCustomerPayment.FowstID = " & ReadyString(vPaymentFowstID)
            End If
            If vExcludeDeposited Then
                pSql &= " AND ISNULL(DepositStatus, 0) <> 1" & vbCrLf
            End If
            If vExcludeReturned Then
                pSql &= " AND ISNULL(DepositStatus, 0) <> 2" & vbCrLf
            End If
            If vExcludeUndeposited Then
                pSql &= " AND ISNULL(DepositStatus, 0) <> 0" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(vUserID) = False Then
                pSql &= " AND TransCustomerPayment.UserID = " & ReadyString(vUserID)
            End If
        Else
            pSql &= " AND DepositGroupID = " & ReadyString(vDepositGroupID) & vbCrLf
        End If ' END If vDepositGroupID Then
        If vGroupByHeader Then
            pSql &= " AND DepositGroupID IS NOT NULL" & vbCrLf
        End If

        pSql &= " ORDER BY PaymentDate"
        If vGroupByHeader Then
            pSql &= ")" & vbCrLf
        End If

        If vGroupByHeader Then
            Dim pGroupBy = "GROUP BY DepositoryBankID" & vbCrLf

            Dim pFinder As New Finder
            With pFinder
                .MaxRecords = 50
                .SearchString = vSearchKeywords
                With .Fields
                    With .Add("DepositGroupID", SqlDbType.UniqueIdentifier)
                        pGroupBy &= ",DepositGroupID" & vbCrLf
                    End With
                    With .Add("DepositDate", SqlDbType.DateTime)
                        pGroupBy &= ",DepositDate" & vbCrLf
                    End With
                    With .Add("TimeChecked", SqlDbType.DateTime, "MAX(TimeChecked)")
                        'pGroupBy &= ",TimeChecked" & vbCrLf
                        .Searchable = False
                    End With
                    With .Add("DepositoryBankName", SqlDbType.VarChar)
                        pGroupBy &= ",DepositoryBankName" & vbCrLf
                    End With
                    With .Add("DepositoryBankBranch", SqlDbType.VarChar)
                        pGroupBy &= ",DepositoryBankBranch" & vbCrLf
                    End With
                    With .Add("DepositoryAccountName", SqlDbType.VarChar)
                        pGroupBy &= ",DepositoryAccountName" & vbCrLf
                    End With
                    With .Add("DepositoryAccountNumber", SqlDbType.VarChar)
                        pGroupBy &= ",DepositoryAccountNumber" & vbCrLf
                    End With
                    With .Add("AmountDeposited", SqlDbType.Money)
                        .Expression = "SUM(AmountPaid)"
                        .Searchable = False
                    End With
                End With
                .SqlFrom = "FROM CTE2"
                .SqlGroup = pGroupBy
                .SqlOrder = "ORDER BY DepositDate DESC, TimeChecked DESC"
                pSql &= .CreateSql()
            End With
        End If
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function SaveFowst(vID As String, vFowstID As String, vUserID As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pAffectedRecords As Integer
        Dim pDR = ById(vID)

        If pDR Is Nothing Then
            Throw New ApplicationException("Invoice does not exist. ref: vbQ3D8ANXK6W!")
        End If

        pIU = New InsertUpdate("TransInvoice")
        If pDR("EncodingFowstID") Is DBNull.Value Then
            pIU("EncodingFowstID") = "FowstID"
        End If
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("UserID") = ReadyString(vUserID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pAffectedRecords = mProData.ExecuteSQL(pSql)

        Return pAffectedRecords > 0

    End Function

    Public Function CreatePoFromReceiving(vReceivingID As String, vOrgID As String, ByRef oError As String,
        vTransType As String,
        vRemarks As String) As String

        Dim pCreator As TransInvoice.Creator
        Dim pID As String
        Dim pDrReceiving As DataRow
        Dim pDtReceivingStocks As DataTable
        Dim pRefPoID As String

        pDrReceiving = mParent.TransReceiving.ByID(vReceivingID)
        If pDrReceiving Is Nothing Then
            Return Nothing
        End If
        If pDrReceiving("PurchaseOrderID") IsNot DBNull.Value Then
            pRefPoID = CStr2(pDrReceiving("PurchaseOrderID"))
        End If
        pCreator = New Creator(mProData, Me)
        pID = Guid.NewGuid().ToString()
        pCreator.ID = pID
        pCreator.CustomerID = CStr2(pDrReceiving("SupplierID"))
        pCreator.BillingAddressID = CStr2(pDrReceiving("BillingAddressID"))
        pCreator.DeliveryAddressID = CStr2(pDrReceiving("DeliveryAddressID"))
        pCreator.DeviceID = CStr2(pDrReceiving("DeviceID"))
        pCreator.UserID = CStr2(pDrReceiving("UserID"))
        pCreator.InvoiceNumber = 0
        pCreator.SalesRepText = CStr2(pDrReceiving("SalesRepText"))
        pCreator.PaymentTerm = CInt2(pDrReceiving("PaymentTerm"))
        If CBool2(pDrReceiving("IsReceiving")) Then
            If String.IsNullOrWhiteSpace(vRemarks) Then
                pCreator.Remarks = "Created from stock receiving"
            Else
                pCreator.Remarks = vRemarks
            End If
            If String.IsNullOrWhiteSpace(vTransType) _
                OrElse vTransType.ToLower() = "StockReceiving".ToLower() Then
                pCreator.TransType = SaveSalesOrderJson.eTransType.AccountsPayable.ToString()
            Else
                pCreator.TransType = vTransType
            End If
            If pDrReceiving("DateReceived") IsNot DBNull.Value Then
                pCreator.PurchaseOrderDate = pDrReceiving("DateReceived")
            End If
            pCreator.FowstID = CStr2(pDrReceiving("ReceivingPlaceID"))
            pCreator.ReceivingID = vReceivingID
        Else
            ' stock transfer
            If String.IsNullOrWhiteSpace(vRemarks) Then
                pCreator.Remarks = "Created from stock transfer"
            Else
                pCreator.Remarks = vRemarks
            End If
            pCreator.StockTransferID = vReceivingID
            pCreator.TransType = SaveSalesOrderJson.eTransType.StockTransfer.ToString()
            pCreator.SalesOrderDate = Now
            pCreator.FowstID = CStr2(pDrReceiving("ReceiveFromOutlet"))
            pCreator.StrNum = GetNextStrNumber(pCreator.DeviceID, pCreator.FowstID)
        End If
        'pCreator.PurchaseOrderText = vData.PurchaseOrderText
        If CDec2(pDrReceiving("Total")) > 0 Then
            pCreator.TotalAmount = CDec2(pDrReceiving("Total"))
        End If
        pCreator.DateNeeded = Now

        pCreator.Stocks = New DataTable()
        With pCreator.Stocks.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("QuantityBad", GetType(Decimal))
            .Add("ActualQuantity", GetType(Integer))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("ModifierGroupID")
        End With

        pDtReceivingStocks = mParent.TransReceiving.GetStocks(vReceivingID)
        'pDtReceivingStocks.Columns.Add("PoQty", GetType(Decimal))
        Dim pPoStocks As DataTable
        If pRefPoID IsNot Nothing Then
            pPoStocks = mParent.TransInvoice.GetItems(pRefPoID, True)
            For Each pDrPoStock As DataRow In pPoStocks.Rows
                Dim pInSr As Boolean = False
                If CDec2(pDrPoStock("Quantity")) <= 0 Then
                    Continue For
                End If
                For Each pDrReceivingStock As DataRow In pDtReceivingStocks.Rows
                    If ReadyGuid(pDrPoStock("ProductID")) = ReadyGuid(pDrReceivingStock("ProductID")) _
                        AndAlso CStr2(pDrPoStock("UnitMeasure")) = CStr2(pDrReceivingStock("UnitMeasure")) Then

                        'pDrReceivingStock("PoQty") = CDec2(pDrReceivingStock("PoQty")) + CDec2(pDrPoStock("Quantity"))
                        pInSr = True
                        Exit For
                    End If
                Next pDrReceivingStock

                If pInSr = False Then
                    Dim pNewDrReceivingStock As DataRow = pDtReceivingStocks.NewRow
                    pNewDrReceivingStock("ProductID") = pDrPoStock("ProductID")
                    pNewDrReceivingStock("PoQty") = pDrPoStock("Quantity")
                    pNewDrReceivingStock("QuantityGood") = 0
                    pNewDrReceivingStock("ActualQuantityGood") = CStr2(pDrPoStock("ActualQuantity"))
                    pNewDrReceivingStock("UnitMeasure") = CStr2(pDrPoStock("UnitMeasure"))
                    pNewDrReceivingStock("PoUnitPrice") = CStr2(pDrPoStock("UnitPrice"))
                    pDtReceivingStocks.Rows.Add(pNewDrReceivingStock)
                End If
            Next pDrPoStock
        End If ' END If pRefPoID IsNot Nothing Then

        Dim pDR As DataRow


        For Each pDrReceivingStock As DataRow In pDtReceivingStocks.Rows
            Dim ppDrReceivingStock2 = pDrReceivingStock ' copy loop variable to local variable because of linq
            pDR = pCreator.Stocks.NewRow()
            pDR("StockID") = CStr2(pDrReceivingStock("ProductID"))
            If CBool2(pDrReceiving("IsReceiving")) Then
                pDR("OrderedQuantity") = CDec2(pDrReceivingStock("PoQty"))
            Else
                pDR("OrderedQuantity") = CDec2(pDrReceivingStock("QuantityGood"))
            End If
            pDR("Quantity") = CDec2(pDrReceivingStock("QuantityGood"))
            pDR("QuantityBad") = CDec2(pDrReceivingStock("QuantityDamaged"))
            pDR("ActualQuantity") = CDec2(pDrReceivingStock("ActualQuantityGood"))
            pDR("UnitMeasure") = CStr2(pDrReceivingStock("UnitMeasure"))
            pDR("UnitPrice") = pDrReceivingStock("PoUnitPrice")
            pDR("OriginalUnitPrice") = pDrReceivingStock("PoUnitPrice")
            If CDec2(pDR("UnitPrice")) = 0 Then
                Dim pLastPo = LastApOrPoPriceWhicheverIsLower(CStr2(pDrReceiving("SupplierID")), CStr2(pDrReceivingStock("ProductID")))
                If pLastPo IsNot Nothing Then
                    pDR("UnitPrice") = pLastPo("UnitPrice") * pDR("ActualQuantity") / pLastPo("ActualQuantity")
                End If
            End If

            '// BEGIN comment this 2018_08_08__13_57_17_4612
            'If pRefPoID IsNot Nothing Then
            '    Dim pDtPoProducts = GetCartStock(pRefPoID, CStr2(pDrReceivingStock("ProductID")), "po")
            '    Dim pUnitPrices =
            '        From p In pDtPoProducts
            '        Where p("ActualQuantity") = CDec2(ppDrReceivingStock2("ActualQuantityGood"))
            '        Select p
            '    If (pUnitPrices.Any()) Then
            '        pDR("UnitPrice") = CDec2(pUnitPrices.First()("UnitPrice"))
            '    Else
            '        pDR("UnitPrice") = CDec2(mParent.LibraryPriceCanvass.GetVendorPrice(pCreator.CustomerID, CStr2(pDrReceivingStock("ProductID"))))
            '    End If
            'Else
            '    pDR("UnitPrice") = CDec2(mParent.LibraryPriceCanvass.GetVendorPrice(pCreator.CustomerID, CStr2(pDrReceivingStock("ProductID"))))
            'End If
            '// END comment this 2018_08_08__13_57_17_4612

            pDR("Subtotal") = CDec2(pDR("UnitPrice")) * pDR("Quantity")
            pDR("Cost") = CDec2(mParent.LibraryStock.GetLandedCost(CStr2(pDrReceivingStock("ProductID")), vOrgID)) * CDec2(pDR("ActualQuantity"))
            pDR("PriceLevelID") = mParent.SystemDefaults.GetLandedCostID()
            pCreator.Stocks.Rows.Add(pDR)
        Next pDrReceivingStock

        mProData.BeginTransaction()
        If pCreator.Save(True, oError) Then
            mParent.TransReceiving.SaveAccountsPayableID(vReceivingID, pCreator.ID)
            mParent.TransReceiving.CacheReceivingsWithCredit()
            mProData.CommitTransaction()
            Return pCreator.ID
        Else
            Throw New ApplicationException(oError)
        End If

    End Function

    Public Enum eProfitDataGroup
        Customer
        Sku
        ProductCategory
    End Enum

    Public Enum eProfitDataTimeHorizon
        Month
        Quarter
        Year
    End Enum

    Public Function GetProfitData(vGroup As eProfitDataGroup, vTimeHorizon As eProfitDataTimeHorizon, vYears As List(Of Integer)) As DataTable

        Dim pSelect As String
        Dim pSelect2 As String
        Dim pGroupingID As String
        Dim pFrom As String
        Dim pGroupBy As String

        vYears.Sort()
        Select Case vGroup
            Case eProfitDataGroup.Customer
                BrowserController.ComposeCustomer(pSelect, pFrom)
                pGroupingID = "TransInvoice.CustomerID"
                pFrom = " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf & pFrom
                pGroupBy = " ,TransInvoice.CustomerID" & vbCrLf &
                    " ,MobizLibraryOrganization.IsOrganization" & vbCrLf &
                    " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
                    " ,MobizLibraryPeoplePreNominalSuffix.Code" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonSurname" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonFirstName" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonMiddleName" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonSuffixName" & vbCrLf

            Case eProfitDataGroup.Sku
                pSelect = "ISNULL(MobizLibraryPIMProduct.Brand + ' ', '') + MobizLibraryPIMProduct.ProductName"
                pSelect2 = "ISNULL(MobizLibraryPIMProduct.ProductNo + ' ', '')"
                pGroupingID = "TransInvoiceItems.ProductID"
                pFrom = " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf
                pGroupBy = ",MobizLibraryPIMProduct.ProductName" & vbCrLf &
                    ",TransInvoiceItems.ProductID" & vbCrLf &
                    ",MobizLibraryPIMProduct.Brand" & vbCrLf &
                    ",MobizLibraryPIMProduct.ProductNo"

        End Select

        Dim pSql = " -- Gross Profitabiloity Report Format 12-11-2017" & vbCrLf &
            " SELECT SUM((TransInvoiceItems.Quantity - ISNULL(tblReturns.QuantityReturned, 0)) * TransInvoiceItems.UnitPrice) AS TotalAmount" & vbCrLf &
            ",SUM((TransInvoiceItems.Quantity - ISNULL(tblReturns.QuantityReturned, 0)) * TransInvoiceItems.UnitGP) AS GP" & vbCrLf &
            " ,YEAR(TransInvoice.InvoiceDate) AS Year" & vbCrLf &
            "," & pSelect & " AS Grouping" & vbCrLf
        If pSelect2 IsNot Nothing Then
            pSql &= "," & pSelect2 & " AS Grouping2" & vbCrLf
        End If
        pSql &= "," & pGroupingID & " AS GroupingID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            pFrom & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT TransMemoSalesReturn.QuantityReturned" & vbCrLf &
            " 	FROM TransMemoSalesReturn" & vbCrLf &
            " 	WHERE TransMemoSalesReturn.InvoiceID = TransInvoice.ID" & vbCrLf &
            "   AND TransMemoSalesReturn.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            " 	AND TransMemoSalesReturn.ActualQuantity = TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ) tblReturns" & vbCrLf &
            " WHERE YEAR(TransInvoice.InvoiceDate) BETWEEN " & vYears(0) & " AND " & vYears(vYears.Count - 1) & vbCrLf &
            " AND TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " AND (TransInvoice.TransType = 'Invoice'" & vbCrLf &
            " 	OR TransInvoice.TransType IS NULL)" & vbCrLf &
            " GROUP BY YEAR(TransInvoice.InvoiceDate)" & vbCrLf &
            pGroupBy &
            " HAVING SUM(TransInvoiceItems.SubTotal) <> 0" & vbCrLf &
            " OR SUM(TransInvoiceItems.LineGP) <> 0" & vbCrLf &
            " ORDER BY YEAR(TransInvoice.InvoiceDate)" & vbCrLf &
            ",SUM((TransInvoiceItems.Quantity - ISNULL(tblReturns.QuantityReturned, 0)) * TransInvoiceItems.UnitGP) DESC"
        Dim pDT As DataTable
        Dim pFilename As String
        Dim pTimer As Diagnostics.Stopwatch
#If DEBUG Then
        If Now.Date = #11/25/2020# Then
            pFilename = Common.ServerPath & "temp\logs\GetProfitData vbEGHYC5ENAS! input.xml"
            If IO.File.Exists(pFilename) Then
                pDT = New DataTable
                pDT.ReadXml(pFilename)
            End If
        End If
#End If
        If pDT Is Nothing Then
            pTimer = Diagnostics.Stopwatch.StartNew()
            pDT = mProData.Execute(pSql)
            pTimer.Stop()
            AppendToLog("vbHQBY5F5VWW sql = " & pTimer.ElapsedMilliseconds & " milliseconds", , "GetProfitData vbEGHYC5ENAS!")
            pFilename = Common.ServerPath & "temp\logs\GetProfitData vbEGHYC5ENAS! output.xml"
            If Now.Date = #11/25/2020# Then
                pDT.WriteXml(pFilename, XmlWriteMode.WriteSchema)
            End If
        End If

        pDT.Columns.Add("GPM", GetType(Decimal))
        For Each pDR As DataRow In pDT.Rows
            If CDec2(pDR("TotalAmount")) <> 0 Then
                pDR("GPM") = CDec2(pDR("GP")) / pDR("TotalAmount")
            End If
        Next pDR

        pTimer = Diagnostics.Stopwatch.StartNew()
        Dim pCrossGrossProfit = New CrossColumn("GrossProfit-{Year}", "GP")
        Dim pFixedColumns As New List(Of String)
        pFixedColumns.Add("GroupingID")
        pFixedColumns.Add("Grouping")
        If pSelect2 IsNot Nothing Then
            pFixedColumns.Add("Grouping2")
        End If
        pDT = ProPack.Utility.CreateCrossTab(pDT, pFixedColumns.ToArray(), {
            New CrossColumn("NetSales-{Year}", "TotalAmount"),
            pCrossGrossProfit,
            New CrossColumn("GPM-{Year}", "GPM")
        })
        pTimer.Stop()
        Dim pSort As String = "Grouping"
        If pCrossGrossProfit.GeneratedColumns.Count > 0 Then
            pSort = pCrossGrossProfit.GeneratedColumns(pCrossGrossProfit.GeneratedColumns.Count - 1).ColumnName
            pSort = EscapeSqlName(pSort) & " DESC"
        End If
        pDT = New DataView(pDT, "", pSort, DataViewRowState.CurrentRows).ToTable()
        AppendToLog("vb5CACW2Q5BR CreateCrossTab = " & pTimer.ElapsedMilliseconds & " milliseconds", , "GetProfitData vbEGHYC5ENAS!")

        Return pDT

    End Function

    Public Function NonZeroItemCount(ByVal vID As String) As Integer

        Dim pOut As Integer
        Dim pSql As String

        pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID) & vbCrLf &
            " AND (ISNULL(Quantity, 0) > 0" & vbCrLf &
            " OR ISNULL(OrderedQuantity, 0) > 0)" & vbCrLf
        pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function GetTransactionInfo(vID As String) As String

        Dim pSql = " --x with netsuite response" & vbCrLf &
            " SELECT TOP 10 TransInvoice.ID" & vbCrLf &
            " ,TransInvoice.NetSuiteNum" & vbCrLf &
            " ,TransInvoice.NetSuiteReponse" & vbCrLf &
            " ,TransInvoice.NetSuiteError" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.PurchaseOrderNo" & vbCrLf &
            " ,TransInvoice.SalesOrderNo" & vbCrLf &
            " ,TransInvoice.CustomerFowstID" & vbCrLf &
            " ,TransInvoice.CustomerBinID" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " ,MobizLibraryOrganization.NetSuiteID AS CompanyNetSuiteID" & vbCrLf &
            " ,MobizLibraryOrganization.ThirdPartySystemID AS CompanyThirdPartySystemID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.NetSuiteID AS FowstNetSuiteID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.ThirdPartySystemID AS FowstThirdPartySystemID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf &
            " ,LibraryFowstBin.NetSuiteID AS BinNetSuiteID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,CONVERT(varchar, tblInvoiceFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS FowstNumDeviceNum" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = CustomerID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.CustomerFowstID" & vbCrLf &
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInvoice.CustomerBinID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblInvoiceFowst ON tblInvoiceFowst.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vID) & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return ""
        End If

        Dim pOut = "Internal ID: " & ReadyGuid(pDR("ID")) & vbCrLf
        If CInt2(pDR("PurchaseOrderNo")) > 0 Then
            pOut &= "PO " & pDR("FowstNumDeviceNum") & "-" & pDR("PurchaseOrderNo") & vbCrLf
        ElseIf CInt2(pDR("SalesOrderNo")) > 0 Then
            pOut &= "SO " & pDR("FowstNumDeviceNum") & "-" & pDR("SalesOrderNo") & vbCrLf
        End If
        pOut &= "LastUpdated: " & Format(pDR("LastUpdated"), cGlobal.FormatDateTime) & vbCrLf
        pOut &= "Customer: " & pDR("CompanyName") & vbCrLf
        pOut &= "Customer NetSuiteID: " & pDR("CompanyNetSuiteID") & vbCrLf
        pOut &= "Customer ThirdPartySystemID: " & pDR("CompanyThirdPartySystemID") & vbCrLf
        pOut &= "Customer FOWST/Outlet: " & pDR("FowstName") & vbCrLf
        pOut &= "Customer FowstNetSuiteID: " & pDR("FowstNetSuiteID") & vbCrLf
        pOut &= "Customer FowstThirdPartySystemID: " & pDR("FowstThirdPartySystemID") & vbCrLf
        pOut &= "Customer Bin/Department: " & pDR("BinName") & vbCrLf
        pOut &= "Customer BinNetSuiteID: " & pDR("BinNetSuiteID") & vbCrLf

        Return pOut

    End Function

    Public Function EcommerceCart(vID As String) As DataTable

        Dim pSql = " SELECT TOP 10000 TransInvoiceItems.ID" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf

        If CBool2(mParent.SystemDefaults.EcommerceIsAllowBuyersSelectFromVendor) Then
            pSql &= " ,(SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = MobizLibraryPIMProduct.ID AND Size = 'medium' AND IsDefault = 1 " & vbCrLf &
                        " AND ISNULL(IsFromVendor, 0) = 1 " & vbCrLf &
                        " AND ISNULL(TransInvoiceItems.VendorID, NULL) = VendorID " & vbCrLf &
                    ") as FileName" & vbCrLf
        Else
            pSql &= " ,(SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = MobizLibraryPIMProduct.ID AND Size = 'medium' AND IsDefault = 1 AND ISNULL(IsFromVendor, 0) = 0) as FileName" & vbCrLf
        End If

        pSql &= " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
           " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
           " ,TransInvoiceItems.Quantity" & vbCrLf &
           " ,TransInvoiceItems.OrderedQuantity AS Qty" & vbCrLf &
           " ,TransInvoiceItems.UnitMeasure" & vbCrLf &
           " ,TransInvoiceItems.UnitPrice AS Price" & vbCrLf &
           " ,TransInvoiceItems.VendorID " & vbCrLf &
           " ,vendor.VendorCode" & vbCrLf &
           IIf(CBool2(mParent.SystemDefaults.EcommerceShowVendorName), " ,vendor.VendorName ", "") & vbCrLf &
           " FROM TransInvoiceItems " & vbCrLf &
           " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
           " OUTER APPLY ( " & vbCrLf &
           "   SELECT VendorCode, org.CompanyName as VendorName " & vbCrLf &
           "   FROM LibraryOrganizationSupplier as sup" & vbCrLf &
           "   INNER JOIN MobizLibraryOrganization as org ON org.ID = sup.SupplierID " & vbCrLf &
           "   WHERE SupplierID = VendorID " & vbCrLf &
           " ) as vendor " & vbCrLf &
           " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vID) & vbCrLf &
           " ORDER BY MobizLibraryPIMProduct.ProductName"

        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ChildSos(vID As String) As DataTable

        Dim pSql = " SELECT TOP 10 CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + '-' + CONVERT(varchar, ISNULL(LibraryDevice.DeviceNo, 0)) + '-' + CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo, 0)) AS SoMin" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " ,TransInvoice.DeliveryMethod" & vbCrLf &
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf &
            " ,TransInvoiceItems.OrderedQuantity" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " WHERE TransInvoice.ParentOrderID = " & ReadyString(vID) & vbCrLf &
            " OR TransInvoice.ID = " & ReadyString(vID) & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function RelatedSalesOrders(vID As String, vIncludeInvoiced As Boolean) As DataTable

        Dim pSql = "   SELECT TransInvoice.ID" & vbCrLf &
            " ,CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + '-' + CONVERT(varchar, ISNULL(LibraryDevice.DeviceNo, 0)) + '-' + CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo, 0)) AS SoMin" & vbCrLf &
            "   ,ISNULL(MobizLibraryOrganizationFOWST.Name, '') + ' - ' + ISNULL(TransInvoice.Area, '') AS LocationAndSubArea" & vbCrLf &
            "   ,TransInvoice.DeliveryMethod" & vbCrLf &
            "   ,ISNULL(TransInvoice.DateImported, TransInvoice.LastUpdated) AS DateImported" & vbCrLf &
            "   ,CASE WHEN TransInvoice.ParentOrderID IS NULL THEN 'Parent'" & vbCrLf &
            "   ELSE 'Child'" & vbCrLf &
            "   END AS SoType" & vbCrLf &
            "   ,CASE WHEN TransInvoice.FinalizePickListByID IS NULL THEN 'Draft'" & vbCrLf &
            "   ELSE 'Finalized'" & vbCrLf &
            "   END AS FulfillmentStatus" & vbCrLf &
            "   ,TransInvoice.ParentOrderID" & vbCrLf &
            "   FROM (" & vbCrLf &
            "   	SELECT TransInvoice.*" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            "     WHERE TransInvoice.ParentOrderID = " & ReadyString(vID) & vbCrLf &
            "     OR TransInvoice.ID = " & ReadyString(vID) & vbCrLf &
            "     UNION" & vbCrLf &
            "     SELECT TransInvoice.*" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            "     WHERE ID IN (" & vbCrLf &
            "     	SELECT tblChild.ParentOrderID" & vbCrLf &
            "         FROM TransInvoice tblChild" & vbCrLf &
            "         WHERE ID = " & ReadyString(vID) & vbCrLf &
            "     )" & vbCrLf &
            "   ) TransInvoice" & vbCrLf &
            " JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf
        If vIncludeInvoiced = False Then
            pSql &= " WHERE TransInvoice.InvoiceID IS NULL" & vbCrLf &
            " AND TransInvoice.ID NOT IN (" & vbCrLf &
            "   SELECT SalesOrderID" & vbCrLf &
            "   FROM TransInvoice" & vbCrLf &
            "   WHERE SalesOrderID IS NOT NULL" & vbCrLf &
            "   AND TransType = 'Invoice'" & vbCrLf &
            " )" & vbCrLf
        End If
        pSql &= " ORDER BY TransInvoice.LastUpdated"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function FinalizeChildOrders(vID As String)

        Dim pSql = "SELECT ID" & vbCrLf &
            ",FowstID" & vbCrLf &
            ",DeviceID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ParentOrderID = " & ReadyString(vID) & vbCrLf &
            " AND ISNULL(SalesOrderNo, 0) = 0"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            Dim pNum = GetMaxSoNumber(CStr2(pDR("DeviceID")), CStr2(pDR("FowstID")))
            Dim pIU = New InsertUpdate("TransInvoice")
            pIU("SalesOrderNo") = pNum + 1
            pIU("SalesOrderDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vCurrentOrgID"></param>
    ''' <param name="vRequisitionID">LibraryProjectRequisition.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateOrderFromProject(vCurrentOrgID As String, vRequisitionID As String) As Boolean

        Dim pCreator As Creator
        Dim pDR As DataRow
        Dim pError As String
        Dim pOut As Boolean
        Dim pNum As Integer
        Dim pCustomerID As String
        Dim pDtSourceProducts = GetProjectProducts(vRequisitionID)

        If pDtSourceProducts.Rows.Count = 0 Then
            Throw New ApplicationException("Project has no product. ref: vbU94RTBF3GJ!")
        End If
        pCreator = New Creator(mProData, mParent.TransInvoice)
        pDR = mParent.LibraryOrganizationFOWST.GetFirstByOrgID(vCurrentOrgID)
        If pDR Is Nothing Then
            Throw New ApplicationException("OrgID, " & vCurrentOrgID & ", has no FOWST. ref: vbWY8Z6L69FP!")
        End If
        pCreator.FowstID = CStr2(pDR("ID"))
        pDR = mParent.LibraryDevice.GetOrCreateDesktop()
        If pDR Is Nothing Then
            Throw New ApplicationException("No desktop device and cannot create one. ref: vbJZDXSLJ6CT!")
        End If
        pCreator.DeviceID = CStr2(pDR("ID"))
        pCreator.UserID = modClient.EmployeeWebGuidID
        pCreator.InvoiceNumber = 0
        'mParent.TransInvoice.VerifyCounter(pCreator.DeviceID, pCreator.FowstID, pNum, , , "POSLastSONo")
        'pCreator.SalesOrderNo = pNum + 1
        pCreator.LastUpdated = Now
        pCreator.Remarks = "Created from project"

        pDR = Nothing
        If pDR Is Nothing Then
            pCustomerID = mParent.SystemDefaults.CustomerOrganizationID()
        End If
        pCreator.CustomerID = CStr2(pDtSourceProducts.Rows(0)("CustomerID"))
        If String.IsNullOrWhiteSpace(pCreator.CustomerID) Then
            Throw New ApplicationException("CustomerID is required. ref: vbFVWPV8QH8X!")
        End If
        Dim pFowstID = mParent.LibraryOrganizationFOWST.AddOrUpdateFromMainAddress(pCreator.CustomerID)
        If pFowstID Is Nothing Then
            Throw New ApplicationException("OrgID, " & pCreator.CustomerID & ", has no FOWST. ref: vbV3HLXFVFSF!")
        End If
        pCreator.BillingAddressID = pFowstID
        pCreator.DeliveryAddressID = pFowstID
        pCreator.TransType = SaveSalesOrderJson.eTransType.SalesOrder.ToString()
        pCreator.Stocks = New DataTable()
        With pCreator.Stocks.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Integer))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("ModifierGroupID")
        End With
        For Each pDrSourceProduct In pDtSourceProducts.Rows
            pDR = pCreator.Stocks.NewRow()
            pDR("StockID") = CStr2(pDrSourceProduct("ProductID"))
            pDR("OrderedQuantity") = CDec2(pDrSourceProduct("RequestedValue"))
            pDR("ActualQuantity") = 1
            pDR("UnitMeasure") = CStr2(pDrSourceProduct("UnitMeasure"))
            pDR("UnitPrice") = mParent.LibraryStock.GetRetailPrice(CStr2(pDrSourceProduct("ProductID")), vCurrentOrgID)
            pDR("Subtotal") = pDR("UnitPrice") * pDR("OrderedQuantity")
            pDR("Cost") = CDec2(mParent.LibraryStock.GetLandedCost(CStr2(pDrSourceProduct("ProductID")), vCurrentOrgID)) * pDR("OrderedQuantity")
            pDR("PriceLevelID") = mParent.SystemDefaults.GetRetailPriceID()
            pCreator.Stocks.Rows.Add(pDR)
        Next
        pOut = pCreator.Save(True, pError)

        Return pOut

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vRequisitionID">LibraryProjectRequisition.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetProjectProducts(vRequisitionID As String) As DataTable

        Dim pSql = " -- project to sales order" & vbCrLf &
            " SELECT LibraryProjectBudget.AssociateSKUID AS ProductID" & vbCrLf &
            " ,LibraryProjectMaterialsRequisition.RequestedValue" & vbCrLf &
            " ,LibraryProjectBudget.UnitMeasureID" & vbCrLf &
            " ,MobizLibraryCountriesLanguageLabel.Label as UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,LibraryProjectBudget.FOWSTID" & vbCrLf &
            "  ,(SELECT cusprojbud.ProjectOwnerOrgPersonID FROM LibraryProjectBudget as cusprojbud" & vbCrLf &
            " WHERE cusprojbud.FowstID = LibraryProjectBudget.FOWSTID" & vbCrLf &
            " AND ISNULL(convert(varchar(max), cusprojbud.ProjectOwnerOrgPersonID), '') <> '') AS CustomerID" & vbCrLf &
            " FROM LibraryProjectBudget" & vbCrLf &
            " JOIN LibraryProjectMaterialsRequisition ON LibraryProjectMaterialsRequisition.ProjectBudgetID = LibraryProjectBudget.ID" & vbCrLf &
            " JOIN LibraryProjectRequisition ON LibraryProjectRequisition.ID = LibraryProjectMaterialsRequisition.RequisitionID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryProjectBudget.AssociateSKUID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = LibraryProjectBudget.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
            " WHERE LibraryProjectRequisition.ID = " & ReadyString(vRequisitionID)
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function InvoiceableChildOrders(vInvoiceID As String) As DataTable

        Dim pSql = "SELECT SalesOrderID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceID)
        Dim pOrderID = CStr2(mProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pOrderID) Then
            Return Nothing
        End If

        pSql = "SELECT ParentOrderID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(pOrderID)
        Dim pParentID = CStr2(mProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pParentID) Then
            pParentID = pOrderID
        End If

        pSql = " -- InvoiceableChildOrders" & vbCrLf &
            " DECLARE @InvoiceID AS Uniqueidentifier = " & ReadyString(pOrderID) & ";" & vbCrLf &
            " DECLARE @ParentOrderID AS Uniqueidentifier = " & ReadyString(pParentID) & ";" & vbCrLf &
            " SELECT TransInvoiceItems.*" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE (ISNULL(TransInvoiceItems.OrderedQuantity, 0) > 0" & vbCrLf &
            " 	OR ISNULL(TransInvoiceItems.Quantity, 0) > 0" & vbCrLf &
            " ) AND (" & vbCrLf &
            " 	TransInvoice.ID = @ParentOrderID" & vbCrLf &
            "     OR TransInvoice.ParentOrderID = @ParentOrderID" & vbCrLf &
            " ) AND NOT EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "     FROM TransInvoice tblInvoice" & vbCrLf &
            "     WHERE tblInvoice.SalesOrderID = TransInvoice.ID" & vbCrLf &
            " ) AND TransInvoice.SalesOrderNo > 0" & vbCrLf &
            " AND TransInvoice.ID <> @InvoiceID"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Enum eCrossTabAxis
        Encoder
        Vendor
        SalesRep
    End Enum

    Public Function CrossTabReport(vFinder As Finder, vXAxis As eCrossTabAxis, vYAxis As eCrossTabAxis,
        ByVal vInvoiceDateFrom As Date?, vInvoiceDateTo As Date?,
        ByVal vDateNeededFrom As Date?, vDateNeededTo As Date?,
        ByVal vSalesFigure As String,
        ByVal vDateEncodedFrom As Date?, vDateEncodedTo As Date?,
        ByVal vIncludeProducts As Boolean
    ) As DataTable

        ' vSalesFigure:
        ' TransInvoiceItems.SubTotal
        ' TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity
        UpdateSalesRepText()

        Dim pDT As DataTable
        Dim pXAxisOutput, pYAxisOutput As CrossTabAxisOutput
        pXAxisOutput = GetCrossTabAxis(vXAxis)
        pYAxisOutput = GetCrossTabAxis(vYAxis)
        With vFinder
            .MaxRecords = 0
            With .Fields
                If vIncludeProducts Then
                    .Add("InvoiceAmount", SqlDbType.Decimal, "SUM(TransInvoiceItems.SubTotal)")
                    .Add("InvoiceQuantity", SqlDbType.Decimal, "SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity)")
                    .Add("ProductNo", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductNo")
                    .Add("ProductName", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductName")
                Else
                    .Add("SalesFigure", SqlDbType.Decimal, "SUM(" & vSalesFigure & ")")
                End If

                .Add("XAxis", SqlDbType.VarChar, pXAxisOutput.SelectSql)
                .Add("YAxis", SqlDbType.VarChar, pYAxisOutput.SelectSql)
            End With

            .SqlFrom = " FROM TransInvoice" & vbCrLf &
                " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                pXAxisOutput.FromSql & vbCrLf &
                pYAxisOutput.FromSql & vbCrLf
            If vIncludeProducts Then
                .SqlFrom &= " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID  " & vbCrLf
            End If

            .SqlWhere = " WHERE 1=1" & vbCrLf
            If vInvoiceDateFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateTimeSql))
            End If
            If vDateNeededFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.DateNeeded) BETWEEN " & ReadyString(Format(vDateNeededFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vDateNeededTo, cGlobal.FormatDateTimeSql))
            End If
            If vDateEncodedFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.LastUpdated) BETWEEN " & ReadyString(Format(vDateEncodedFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vDateEncodedTo, cGlobal.FormatDateTimeSql))
            End If
            .SqlGroup = " GROUP BY " & pXAxisOutput.GroupSql & vbCrLf &
                ", " & pYAxisOutput.GroupSql
            If vIncludeProducts Then
                .SqlGroup &= ",ProductNo" & vbCrLf &
                    ",ProductName" & vbCrLf
                .SqlHaving = " HAVING SUM(TransInvoiceItems.SubTotal) > 0" & vbCrLf &
                    " OR SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) > 0" & vbCrLf
            Else
                .SqlHaving = " HAVING SUM(" & vSalesFigure & ") > 0"
            End If
            .SqlOrder = " ORDER BY XAxis, YAxis"
            pDT = .Requery()
        End With

        Dim pFilename = IO.Path.Combine(AppPath, "temp", "logs", "CrossTabReport.xml")
        If Now.Date = "2019/07/11" Then
            Try
                pDT.WriteXml(pFilename, XmlWriteMode.WriteSchema)
            Catch ex As Exception
                ' just ignore errors here
                AppendToDebug(ex.Message, True, ex, True)
            End Try
        End If
        If False Then
            pDT = New DataTable
            pDT.ReadXml(pFilename)
        End If

        If vIncludeProducts Then
            Return pDT
        End If
        Dim pDtCrossTab = CreateCrossTab(pDT, {"YAxis"}, {New CrossColumn("{XAxis}", "SalesFigure")})
        pDtCrossTab.Columns("YAxis").ColumnName = pYAxisOutput.ColumnText

        Return pDtCrossTab

    End Function

    Public Shared Function ProductSalesReportByGroup(vDT As DataTable,
        vXAxisName As String, vYAxisName As String) As Workbook

        Dim pWB As New Workbook
        Dim pWS = pWB.Worksheets(0)
        Dim pRow As Integer
        Dim pDvProducts As DataView

        pWS.Cells(pRow, 0).PutValue("Products Sales Report - By Group")

        Dim pDtUniqueX As DataTable = New DataView(vDT).ToTable(True, "XAxis")
        Dim pDtUniqueY As DataTable = New DataView(vDT).ToTable(True, "YAxis")

        For Each pRowX In pDtUniqueX.Rows
            pDvProducts = New DataView(vDT, "XAxis = " & ReadyString(pRowX("XAxis")), "ProductName", DataViewRowState.CurrentRows)
            If pDvProducts.Count = 0 Then
                Continue For
            End If

            pRow += 1
            pWS.Cells(pRow, 0).PutValue(vXAxisName & " : " & pRowX("XAxis"))

            pRow += 1
            pWS.Cells(pRow, 0).PutValue("Item No.")
            pWS.Cells(pRow, 1).PutValue("Item Description")
            pWS.Cells(pRow, 2).PutValue("Quantity Sold")
            pWS.Cells(pRow, 3).PutValue("Total Amount")

            For Each pRowY In pDtUniqueY.Rows
                pDvProducts = New DataView(vDT, "XAxis = " & ReadyString(pRowX("XAxis")) & vbCrLf &
                    " AND YAxis = " & ReadyString(pRowY("YAxis")), "ProductName", DataViewRowState.CurrentRows)
                If pDvProducts.Count = 0 Then
                    Continue For
                End If

                pRow += 2
                pWS.Cells(pRow, 0).PutValue(vYAxisName & " : " & pRowY("YAxis"))

                For Each pDrvProduct In pDvProducts
                    pRow += 1
                    pWS.Cells(pRow, 0).PutValue(CStr2(pDrvProduct("ProductNo")))
                    pWS.Cells(pRow, 1).PutValue(CStr2(pDrvProduct("ProductName")))
                    pWS.Cells(pRow, 2).PutValue(CDec2(pDrvProduct("InvoiceQuantity")))
                    pWS.Cells(pRow, 3).PutValue(CDec2(pDrvProduct("InvoiceAmount")))
                Next pDrvProduct
                pRow += 1
            Next pRowY
        Next pRowX

        Return pWB

    End Function

    Public Shared Sub ProductSalesReportByGroupTest()

        Dim pDT As New DataTable
        Dim pFilename = "C:\dev\debug\Mobiz\WebErp\temp\logs\CrossTabReport.xml"
        pDT.ReadXml(pFilename)

        Dim pWB = TransInvoice.ProductSalesReportByGroup(
            vDT:=pDT,
            vXAxisName:="Encoded By",
            vYAxisName:="Vendor")
        pFilename = "C:\dev\debug\Mobiz\WebErp\temp\logs\ProductSalesReportByGroup.xlsx"
        pWB.Save(pFilename)

    End Sub

    Public Shared Function CrossTabReportSql(vXAxis As eCrossTabAxis, vYAxis As eCrossTabAxis,
            ByVal vInvoiceDateFrom As Date?, vInvoiceDateTo As Date?,
            ByVal vDateNeededFrom As Date?, vDateNeededTo As Date?,
            ByVal vDateEncodedFrom As Date?, vDateEncodedTo As Date?,
            ByVal vXAxisValue As String,
            ByVal vYAxisValue As String
        ) As String

        ' vSalesFigure:
        ' TransInvoiceItems.SubTotal
        ' TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity

        Dim pSql As String
        Dim pXAxisOutput, pYAxisOutput As CrossTabAxisOutput

        pXAxisOutput = GetCrossTabAxis(vXAxis)
        pYAxisOutput = GetCrossTabAxis(vYAxis)

        With New Finder()
            .MaxRecords = 0
            With .Fields
                .Add("ID", SqlDbType.Decimal, "TransInvoice.ID")
            End With

            .SqlFrom = " FROM TransInvoice" & vbCrLf &
                " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                pXAxisOutput.FromSql & vbCrLf &
                pYAxisOutput.FromSql & vbCrLf

            .SqlWhere = " WHERE 1=1" & vbCrLf
            If vInvoiceDateFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If vDateNeededFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.DateNeeded) BETWEEN " & ReadyString(Format(vDateNeededFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vDateNeededTo, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If vDateEncodedFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.LastUpdated) BETWEEN " & ReadyString(Format(vDateEncodedFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vDateEncodedTo, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If vXAxisValue IsNot Nothing Then
                .SqlWhere &= " AND " & pXAxisOutput.SelectSql & " = " & ReadyString(vXAxisValue) & vbCrLf
            End If
            If vYAxisValue IsNot Nothing Then
                .SqlWhere &= " AND " & pYAxisOutput.SelectSql & " = " & ReadyString(vYAxisValue) & vbCrLf
            End If

            pSql = .CreateSql
        End With

        Return pSql

    End Function

    Public Class CrossTabAxisOutput

        Public ColumnText As String
        Public SelectSql As String
        Public FromSql As String
        Public GroupSql As String

    End Class

    Public Shared Function GetCrossTabAxis(vAxis As eCrossTabAxis) As CrossTabAxisOutput

        Dim pOut As New CrossTabAxisOutput
        Dim pCustomerSelect, pCustomerFrom As String

        Select Case vAxis
            Case eCrossTabAxis.SalesRep
                pOut.ColumnText = "Sales Rep"
                pOut.SelectSql = "ISNULL(NULLIF(TransInvoice.SalesRepText, ''), '[No sales rep]')"
                pOut.GroupSql = "TransInvoice.SalesRepText" & vbCrLf

            Case eCrossTabAxis.Encoder
                pOut.ColumnText = "Encoded By"
                pOut.SelectSql = "ISNULL(NULLIF((RTRIM(" & vbCrLf & BrowserController.ComposePeopleName("MobizLibraryPeople_SalesRep", "MobizLibraryPeoplePreNominalSuffix_SalesRep") & vbCrLf & ")),''),'[Unknown Sales Rep]')"
                pOut.FromSql = " JOIN MobizLibraryPeople MobizLibraryPeople_SalesRep ON MobizLibraryPeople_SalesRep.ID = TransInvoice.UserID" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix MobizLibraryPeoplePreNominalSuffix_SalesRep ON MobizLibraryPeople_SalesRep.TitleID = MobizLibraryPeoplePreNominalSuffix_SalesRep.ID" & vbCrLf
                pOut.GroupSql = "MobizLibraryPeoplePreNominalSuffix_SalesRep.Code" & vbCrLf &
                    ",MobizLibraryPeople_SalesRep.Surname" & vbCrLf &
                    ",MobizLibraryPeople_SalesRep.FirstName" & vbCrLf &
                    ",MobizLibraryPeople_SalesRep.MiddleName" & vbCrLf &
                    ",MobizLibraryPeople_SalesRep.PersonalSuffixName" & vbCrLf &
                    ",MobizLibraryPeople_SalesRep.ID" & vbCrLf

            Case eCrossTabAxis.Vendor
                pOut.ColumnText = "Vendor"
                BrowserController.ComposeCustomer(pCustomerSelect, pCustomerFrom, "MobizLibraryOrganization_Vendor", "MobizLibraryPeoplePreNominalSuffix_Vendor")
                pOut.SelectSql = "ISNULL(NULLIF((RTRIM(" & vbCrLf & pCustomerSelect & vbCrLf & ")),''),'[Unknown Vendor]')"
                pOut.FromSql = " LEFT JOIN LibraryVendorPrices ON LibraryVendorPrices.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization MobizLibraryOrganization_Vendor ON MobizLibraryOrganization_Vendor.ID = LibraryVendorPrices.VendorID" & vbCrLf &
                    pCustomerFrom
                pOut.GroupSql = "MobizLibraryPeoplePreNominalSuffix_Vendor.Code" & vbCrLf &
                    ",MobizLibraryOrganization_Vendor.IsOrganization" & vbCrLf &
                    ",MobizLibraryOrganization_Vendor.CompanyName" & vbCrLf &
                    ",MobizLibraryOrganization_Vendor.PersonSurname" & vbCrLf &
                    ",MobizLibraryOrganization_Vendor.PersonFirstName" & vbCrLf &
                    ",MobizLibraryOrganization_Vendor.PersonMiddleName" & vbCrLf &
                    ",MobizLibraryOrganization_Vendor.PersonSuffixName" & vbCrLf

        End Select

        Return pOut

    End Function

    Public Class SaveSomeFieldsInput
        Public ID As String
        Public RefInvoiceNum As String
        Public SalesRepText As String
        Public SalesInvoiceRef As String
        Public OfficialReceiptRef As String
        Public UserID As String
    End Class

    Public Function SaveSomeFields(vInput As SaveSomeFieldsInput) As Integer

        Dim pIU As New InsertUpdate("TransInvoice")
        pIU("RefInvoiceNum") = ReadyString(vInput.RefInvoiceNum)
        pIU("SalesRepText") = ReadyString(vInput.SalesRepText)
        pIU("SalesInvoiceRef") = ReadyString(vInput.SalesInvoiceRef)
        pIU("OfficialReceiptRef") = ReadyString(vInput.OfficialReceiptRef)
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(vInput.UserID)

        'Dim pDrInvoice = ById(vInput.ID)
        'If pDrInvoice IsNot Nothing _
        '    AndAlso CBool2(pDrInvoice("FinalizedForApproval")) _
        '    AndAlso CStr2(pDrInvoice("CncApprovedByID")) <> "" Then

        '    Dim pNum As Integer
        '    Dim pFowstWideInvoiceNumber As Boolean = False
        '    VerifyCounter(CStr2(pDrInvoice("DeviceID")), CStr2(pDrInvoice("FowstID")), pNum, CStr2(pDrInvoice("DocumentNameID")), pFowstWideInvoiceNumber)
        '    pIU("InvoiceNumber") = pNum + 1
        '    pIU("FinalizedForApproval") = "NULL"
        'End If
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vInput.ID))

        Return mProData.ExecuteSQL(pSql)

    End Function

    Public Class SetDeliverInput
        Public InvoiceIDs As List(Of String)
        Public DeliveryMethod As String
        Public DeliveryTruckFowstID As String
        Public DeliveryCheckedByID As String
        Public DeliveryDriverID As String
        Public DeliveryHelperID As String

        Public Delivery3PlCompanyID As String
        Public Delivery3PlCompanyName As String
        Public Delivery3PlContactPerson As String
        Public DeliveryWaybillNumber As String

        Public DeliveryCustomerRepresentative As String
        Public DeliveryRemarks As String
        Public Status As String
        Public DeliveryChecker As Object
        Public DeliveryDriver As Object
        Public DeliveryHelper As Object
        Public IsNewRoutingSheet As Boolean
        Public DeliveryLoggerID As String
        Public Reason As String
        Public DeliveryEncodingDate As String
    End Class

    Public Function SetDeliver(vInput As SetDeliverInput) As Integer

        Dim pIDs As String = ""
        Dim pRoutingSheetNumber = GenerateString(4, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper

        For Each pID In vInput.InvoiceIDs
            If pIDs <> "" Then
                pIDs &= ", "
            End If
            pIDs &= ReadyString(pID)
        Next
        Dim pIU As New InsertUpdate("TransInvoice")

        'pIU("RoutingSheetNumber") = ReadyString(pRoutingSheetNumber)
        If String.IsNullOrWhiteSpace(vInput.Status) = False Then
            pIU("IsDelivered") = IIf(vInput.Status = "Undelivered" Or vInput.Status = "For Delivery", 0, 1)

            If vInput.Status = "Undelivered" Then
                pIU("DeliveryEncodingDate") = "NULL"
            Else
                If String.IsNullOrWhiteSpace(vInput.DeliveryEncodingDate) Then
                    pIU("DeliveryEncodingDate") = "GETDATE()"
                Else
                    pIU("DeliveryEncodingDate") = ReadyString(vInput.DeliveryEncodingDate)
                End If
            End If
        Else
            pIU("IsDelivered") = 1
            If String.IsNullOrWhiteSpace(vInput.DeliveryEncodingDate) Then
                pIU("DeliveryEncodingDate") = "GETDATE()"
            Else
                pIU("DeliveryEncodingDate") = ReadyString(vInput.DeliveryEncodingDate)
            End If
        End If

        pIU("DeliveryStatus") = ReadyString(vInput.Status)
        pIU("LastUpdated") = "GETDATE()"
        If String.IsNullOrWhiteSpace(vInput.DeliveryMethod) = False Then
            pIU("DeliveryMethod") = ReadyString(Truncate(vInput.DeliveryMethod, 100))
        End If

        ' A. Inhouse Delivery
        If String.IsNullOrWhiteSpace(vInput.DeliveryTruckFowstID) = False Then
            pIU("DeliveryTruckFowstID") = ReadyString(Truncate(vInput.DeliveryTruckFowstID, 100))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryCheckedByID) = False Then
            pIU("DeliveryCheckedByID") = ReadyString(Truncate(vInput.DeliveryCheckedByID, 100))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryDriverID) = False Then
            pIU("DeliveryDriverID") = ReadyString(Truncate(vInput.DeliveryDriverID, 100))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryHelperID) = False Then
            pIU("DeliveryHelperID") = ReadyString(Truncate(vInput.DeliveryHelperID, 100))
        End If

        ' B. 3rd Party Delivery
        If String.IsNullOrWhiteSpace(vInput.Delivery3PlCompanyID) = False Then
            pIU("Delivery3PlCompanyID") = ReadyString(vInput.Delivery3PlCompanyID)
        End If
        If String.IsNullOrWhiteSpace(vInput.Delivery3PlCompanyName) = False Then
            pIU("Delivery3PlCompanyName") = ReadyString(Truncate(vInput.Delivery3PlCompanyName, 500))
        End If
        If String.IsNullOrWhiteSpace(vInput.Delivery3PlContactPerson) = False Then
            pIU("Delivery3PlContactPerson") = ReadyString(Truncate(vInput.Delivery3PlContactPerson, 500))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryWaybillNumber) = False Then
            pIU("DeliveryWaybillNumber") = ReadyString(Truncate(vInput.DeliveryWaybillNumber, 500))
        End If

        If String.IsNullOrWhiteSpace(vInput.DeliveryCustomerRepresentative) = False Then
            pIU("DeliveryCustomerRepresentative") = ReadyString(Truncate(vInput.DeliveryCustomerRepresentative, 500))
        End If

        If String.IsNullOrWhiteSpace(vInput.DeliveryRemarks) = False Then
            pIU("DeliveryNotes") = ReadyString(vInput.DeliveryRemarks)
        End If

        Dim pSql = pIU.GetUpdate("ID IN (" & pIDs & ")")

        mProData.ExecuteSQL(pSql)
        Return 1
    End Function

    Public Function SaveToRoutingSheetHeader(vInput As SetDeliverInput) As Integer

        Dim pIDs As String = ""
        Dim pRoutingSheetNumber = GenerateString(4, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper
        Dim pIU As New InsertUpdate("TransDeliveryLogger")
        Dim pSql As String
        Dim deliveryloggerid As String = ""

        mProData.BeginTransaction()

        If String.IsNullOrWhiteSpace(vInput.Status) = False Then
            'pIU("IsDelivered") = IIf(vInput.Status = "Undelivered", 0, 1)

            If vInput.Status = "Undelivered" Then
                pIU("DeliveryEncodingDate") = "NULL"
            Else
                If String.IsNullOrWhiteSpace(vInput.DeliveryEncodingDate) Then
                    pIU("DeliveryEncodingDate") = "GETDATE()"
                Else
                    pIU("DeliveryEncodingDate") = ReadyString(vInput.DeliveryEncodingDate)
                End If
            End If
        Else
            'pIU("IsDelivered") = 1
            If String.IsNullOrWhiteSpace(vInput.DeliveryEncodingDate) Then
                pIU("DeliveryEncodingDate") = "GETDATE()"
            Else
                pIU("DeliveryEncodingDate") = ReadyString(vInput.DeliveryEncodingDate)
            End If
        End If

        pIU("LastUpdated") = "GETDATE()"
        If String.IsNullOrWhiteSpace(vInput.DeliveryMethod) = False Then
            pIU("DeliveryMethod") = ReadyString(Truncate(vInput.DeliveryMethod, 100))
        End If

        ' A. Inhouse Delivery
        If String.IsNullOrWhiteSpace(vInput.DeliveryTruckFowstID) = False Then
            pIU("DeliveryTruckFowstID") = ReadyString(Truncate(vInput.DeliveryTruckFowstID, 100))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryCheckedByID) = False Then
            pIU("DeliveryCheckedByID") = ReadyString(Truncate(vInput.DeliveryCheckedByID, 100))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryDriverID) = False Then
            pIU("DeliveryDriverID") = ReadyString(Truncate(vInput.DeliveryDriverID, 100))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryHelperID) = False Then
            pIU("DeliveryHelperID") = ReadyString(Truncate(vInput.DeliveryHelperID, 100))
        End If

        ' B. 3rd Party Delivery
        If String.IsNullOrWhiteSpace(vInput.Delivery3PlCompanyID) = False Then
            pIU("Delivery3PlCompanyID") = ReadyString(vInput.Delivery3PlCompanyID)
        End If
        'If String.IsNullOrWhiteSpace(vInput.Delivery3PlCompanyName) = False Then
        '    pIU("Delivery3PlCompanyName") = ReadyString(Truncate(vInput.Delivery3PlCompanyName, 500))
        'End If
        If String.IsNullOrWhiteSpace(vInput.Delivery3PlContactPerson) = False Then
            pIU("Delivery3PlContactPerson") = ReadyString(Truncate(vInput.Delivery3PlContactPerson, 500))
        End If
        If String.IsNullOrWhiteSpace(vInput.DeliveryWaybillNumber) = False Then
            pIU("DeliveryWaybillNumber") = ReadyString(Truncate(vInput.DeliveryWaybillNumber, 500))
        End If

        If String.IsNullOrWhiteSpace(vInput.DeliveryCustomerRepresentative) = False Then
            pIU("DeliveryCustomerRepresentative") = ReadyString(Truncate(vInput.DeliveryCustomerRepresentative, 500))
        End If

        If String.IsNullOrWhiteSpace(vInput.DeliveryRemarks) = False Then
            pIU("DeliveryNotes") = ReadyString(vInput.DeliveryRemarks)
        End If

        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

        If CBool2(vInput.IsNewRoutingSheet) Then
            deliveryloggerid = Guid.NewGuid.ToString()
            pIU("RoutingSheetNumber") = ReadyString(pRoutingSheetNumber)
            pIU("ID") = ReadyString(deliveryloggerid)
            pIU("IsDraft") = 1
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Else
            deliveryloggerid = vInput.DeliveryLoggerID
        End If

        For Each pID In vInput.InvoiceIDs
            If pIDs <> "" Then
                pIDs &= ", "
            End If
            pIDs &= ReadyString(pID)

            If vInput.Status = "Undelivered" Then
                'remove invoice from current Routing Sheet and move to logs table for reference
                Dim pRows As DataRow = mProData.ExecuteRow("SELECT * FROM TransDeliveryLoggerInvoices WHERE InvoiceID = " & ReadyString(pID))
                deliveryloggerid = CStr2(pRows("DeliveryLoggerID"))
                pIU = New InsertUpdate("TransDeliveryLogs")
                pIU("DeliveryLoggerID") = ReadyString(deliveryloggerid)
                pIU("InvoiceID") = ReadyString(pID)
                pIU("Reason") = IIf(String.IsNullOrWhiteSpace(vInput.Reason), "NULL", ReadyString(vInput.Reason))
                pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                pIU("DeliveryEncodingDate") = ReadyString(pRows("DeliveryEncodingDate"))
                pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                mProData.Execute(pIU.GetInsert())
            End If

            pIU = New InsertUpdate("TransDeliveryLoggerInvoices")

            pIU("Status") = ReadyString(vInput.Status)

            If vInput.Status = "Undelivered" Then
                pIU("DeliveryLoggerID") = "NULL"
            Else
                pIU("DeliveryLoggerID") = ReadyString(deliveryloggerid)
            End If

            If CBool2(vInput.IsNewRoutingSheet) Then
                pIU("InvoiceID") = ReadyString(pID)
                pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                pIU("DeliveryEncodingDate") = "GETDATE()"
                pSql = pIU.GetInsert()
            Else
                pSql = pIU.GetUpdate(" InvoiceID = " & ReadyString(pID))
            End If

            mProData.ExecuteSQL(pSql)
        Next

        If vInput.Status <> "Undelivered" Then
            If vInput.DeliveryMethod = "Inhouse Delivery" Then
                pSql = "DELETE FROM TransInvoiceDeliveryDetails WHERE InvoiceID IN (" & pIDs & ") AND DeliveryLoggerID = " & ReadyString(deliveryloggerid)
                mProData.ExecuteSQL(pSql)

                For Each pID In vInput.InvoiceIDs
                    For Each people As Object In vInput.DeliveryChecker
                        pIU = New InsertUpdate("TransInvoiceDeliveryDetails")

                        pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                        pIU("DeliveryLoggerID") = ReadyString(deliveryloggerid)
                        pIU("InvoiceID") = ReadyString(pID)
                        pIU("PeopleID") = ReadyString(people("ID").ToString())
                        pIU("Designation") = ReadyString("checker")
                        pIU("LastUpdated") = "getdate()"
                        pIU("IsActive") = 1
                        mProData.Execute(pIU.GetInsert())
                    Next

                    For Each people As Object In vInput.DeliveryDriver
                        pIU = New InsertUpdate("TransInvoiceDeliveryDetails")

                        pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                        pIU("DeliveryLoggerID") = ReadyString(deliveryloggerid)
                        pIU("InvoiceID") = ReadyString(pID)
                        pIU("PeopleID") = ReadyString(people("ID").ToString())
                        pIU("Designation") = ReadyString("driver")
                        pIU("LastUpdated") = "getdate()"
                        pIU("IsActive") = 1
                        mProData.Execute(pIU.GetInsert())
                    Next

                    For Each people As Object In vInput.DeliveryHelper
                        pIU = New InsertUpdate("TransInvoiceDeliveryDetails")

                        pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                        pIU("DeliveryLoggerID") = ReadyString(deliveryloggerid)
                        pIU("InvoiceID") = ReadyString(pID)
                        pIU("PeopleID") = ReadyString(people("ID").ToString())
                        pIU("Designation") = ReadyString("helper")
                        pIU("LastUpdated") = "getdate()"
                        pIU("IsActive") = 1
                        mProData.Execute(pIU.GetInsert())
                    Next
                Next
            End If
        Else
            pIU = New InsertUpdate("TransInvoiceDeliveryDetails")
            pIU("IsActive") = 0
            pIU("LastUpdated") = "getdate()"
            pSql = pIU.GetUpdate(" InvoiceID IN (" & pIDs & ")")
            mProData.Execute(pSql)
        End If

        mProData.CommitTransaction()
        Return 1

    End Function

    Public Function GetDeliver(vID As String) As DataRow

        Dim pFinder As Finder

        pFinder = New Finder()
        pFinder.ProData = mProData
        pFinder.MaxRecords = 1
        With pFinder
            With .Fields
                With .Add("IsDelivered", SqlDbType.Bit)
                    .Expression = "TransInvoice.IsDelivered"
                End With
                With .Add("DeliveryStatus", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryStatus"
                End With
                With .Add("DeliveryEncodingDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.DeliveryEncodingDate"
                End With
                With .Add("DeliveryMethod", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryMethod"
                End With
                With .Add("DeliveryTruckFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.DeliveryTruckFowstID"
                End With
                With .Add("DeliveryTruckFowstName", SqlDbType.VarChar)
                    .Expression = "tblFowstTruck.Name"
                End With
                With .Add("DeliveryCheckedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.DeliveryCheckedByID"
                End With
                With .Add("DeliveryCheckedByName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName("tblPeopleCheckedBy", "tblPeoplePreNominalSuffixCheckedBy")
                End With

                With .Add("DeliveryDriverID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.DeliveryDriverID"
                End With
                With .Add("DeliveryDriverName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName("tblPeopleDriver", "tblPeoplePreNominalSuffixDriver")
                End With

                With .Add("DeliveryHelperID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.DeliveryHelperID"
                End With
                With .Add("DeliveryHelperName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName("tblPeopleHelper", "tblPeoplePreNominalSuffixHelper")
                End With

                With .Add("Delivery3PlCompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.Delivery3PlCompanyID"
                End With
                With .Add("Delivery3PlCompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(tblSupplier.CompanyName, TransInvoice.Delivery3PlCompanyName)"
                End With
                With .Add("Delivery3PlContactPerson", SqlDbType.VarChar)
                    .Expression = "TransInvoice.Delivery3PlContactPerson"
                End With
                With .Add("DeliveryWaybillNumber", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryWaybillNumber"
                End With

                With .Add("DeliveryCustomerRepresentative", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryCustomerRepresentative"
                End With

                With .Add("DeliveryRemarks", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryNotes"
                End With
                With .Add("RoutingSheetNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL(TransInvoice.RoutingSheetNumber, (SELECT RoutingSheetNumber FROM TransDeliveryLogger INNER JOIN TransDeliveryLoggerInvoices ON TransDeliveryLoggerInvoices.DeliveryLoggerID = TransDeliveryLogger.ID WHERE TransDeliveryLoggerInvoices.InvoiceID = TransInvoice.ID))"
                End With
            End With

            .SqlFrom = " FROM TransInvoice" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST tblFowstTruck ON tblFowstTruck.ID = TransInvoice.DeliveryTruckFowstID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople tblPeopleCheckedBy ON tblPeopleCheckedBy.ID = TransInvoice.DeliveryCheckedByID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblPeoplePreNominalSuffixCheckedBy ON tblPeopleCheckedBy.TitleID = tblPeoplePreNominalSuffixCheckedBy.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization tblSupplier ON TransInvoice.Delivery3PlCompanyID = tblSupplier.ID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeople tblPeopleDriver ON tblPeopleDriver.ID = TransInvoice.DeliveryDriverID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblPeoplePreNominalSuffixDriver ON tblPeopleDriver.TitleID = tblPeoplePreNominalSuffixDriver.ID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeople tblPeopleHelper ON tblPeopleHelper.ID = TransInvoice.DeliveryHelperID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblPeoplePreNominalSuffixHelper ON tblPeopleHelper.TitleID = tblPeoplePreNominalSuffixHelper.ID" & vbCrLf
            .SqlWhere = " WHERE TransInvoice.ID = " & ReadyString(vID)
            Dim pDT As DataTable = .Requery()
            If pDT.Rows.Count > 0 Then
                Return pDT.Rows(0)
            End If
        End With

    End Function

    'Public Function CreatePdf()

    '    Dim pInvoiceID As String
    '    Dim pDS As DataSet
    '    Dim pReport As ProReport
    '    Dim pFilename As String
    '    Dim pHeaderInfo As LibraryOrganization.HeaderInfo
    '    Dim pDR As DataRow
    '    Dim pUuid As String
    '    Dim pTransType As String
    '    Dim pTransTypeProper As TransInvoice.SaveSalesOrderJson.eTransType
    '    Dim pDT As DataTable
    '    Dim pDrDocumentControl As DataRow
    '    Dim pPrintLimit As Integer
    '    Dim pModuleSource As String
    '    Dim pDocumentName As String = ""
    '    Dim pTemplate = Common.WebRequest.Data("Template")

    '    If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
    '    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

    '    DeleteOldPrintablePdf()
    '    pUuid = Common.WebRequest.Data("UUID")
    '    pUuid = CStr2(pUuid).Replace(";", "_")
    '    If String.IsNullOrWhiteSpace(pUuid) Then
    '        Return JsonDictionary.Serialize("Error", "UUID is required.")
    '    End If
    '    If pUuid.Length < 1 Then
    '        Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
    '    End If

    '    pTransType = Common.WebRequest.Data("TransType")
    '    If [Enum].GetNames(GetType(TransInvoice.SaveSalesOrderJson.eTransType)).Contains(pTransType) = False Then
    '        Return JsonDictionary.Serialize("Error", "Invalid TransType, " & pTransType & ".")
    '    End If
    '    pTransTypeProper = [Enum].Parse(GetType(TransInvoice.SaveSalesOrderJson.eTransType), pTransType)

    '    pInvoiceID = Common.WebRequest.Data("InvoiceID")
    '    pDR = WebRequest.Instance.Business.TransInvoice.ById(pInvoiceID)
    '    If pDR Is Nothing Then
    '        Return JsonDictionary.Serialize("Error", "Transaction does not exist.")
    '    Else
    '        If CStr2(pTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
    '            pDocumentName = "Withdrawal Slip"
    '            pModuleSource = "Revenue > Invoicing"
    '        ElseIf CStr2(pTemplate).ToLower() = "PickList".ToLower() Then
    '            pDocumentName = "Pick List"
    '            pModuleSource = "Revenue > Sales Ordering"
    '        Else
    '            Select Case pTransTypeProper
    '                Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
    '                    pDocumentName = CStr2(pDR("DocumentName"))
    '                    pModuleSource = "Revenue > Invoicing"
    '                Case TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder
    '                    pModuleSource = "Disbursement > PO"
    '                Case TransInvoice.SaveSalesOrderJson.eTransType.Quote
    '                    pModuleSource = "Revenue > Price Quote"
    '                Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
    '                    pModuleSource = "Revenue > Sales Ordering"
    '            End Select
    '        End If

    '        pDrDocumentControl = WebRequest.Instance.Business.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, pModuleSource)
    '        If pDrDocumentControl IsNot Nothing Then
    '            pPrintLimit = pDrDocumentControl("PrintLimit")
    '            If pPrintLimit = -1 Then
    '                pPrintLimit = Integer.MaxValue
    '            End If
    '        End If
    '        If CStr2(pTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
    '            If CDec2(pDR("WithdrawalSlipPrintCount")) >= pPrintLimit Then
    '                If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Withdrawal", False) = False Then
    '                    Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
    '                End If
    '            End If
    '        ElseIf CStr2(pTemplate).ToLower() = "PickList".ToLower() Then
    '            If CDec2(pDR("PickListPrintCount")) >= pPrintLimit Then
    '                If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Pick List", False) = False Then
    '                    Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
    '                End If
    '            End If
    '        Else
    '            If CDec2(pDR("PrintCount")) >= pPrintLimit Then
    '                If pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Invoice Then
    '                    If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Invoice", False) = False Then
    '                        Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
    '                    End If
    '                Else
    '                    Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
    '                End If
    '            End If
    '        End If
    '    End If
    '    If WebRequest.Instance.Business.TransInvoice.IsSavedAsFinal(pDR) = False Then
    '        Return JsonDictionary.Serialize("Error", "Only transactions that were saved as final can be printed.")
    '    End If

    '    pReport = New ProReport
    '    pReport.AutoFillHeader = False
    '    Dim pDocumentNameID As String
    '    Select Case pTransTypeProper
    '        Case TransInvoice.SaveSalesOrderJson.eTransType.Quote
    '            pReport.SourceFilename = "Quote"

    '        Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
    '            pDocumentName = pTemplate
    '            pReport.SourceFilename = "SalesOrder"
    '            If String.IsNullOrWhiteSpace(pTemplate) Then
    '                pFilename = "SalesOrder"
    '            Else
    '                If CStr2(pTemplate).ToLower() = "PickList".ToLower() Then
    '                    pDocumentName = "Pick List"
    '                End If
    '                pFilename = pTemplate
    '                Dim pDrDocumentType = WebRequest.Instance.Business.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, "Revenue > Sales Ordering")
    '                If pDrDocumentType IsNot Nothing Then
    '                    pDocumentNameID = CStr2(pDrDocumentType("ID"))
    '                End If
    '            End If

    '            If pDocumentNameID Is Nothing Then
    '                pDocumentNameID = CStr2(pDR("DocumentNameID"))
    '            End If
    '            If pDocumentNameID = "" Then
    '            Else
    '                pFilename &= "_" & ReadyGuid(WebRequest.Instance.OrgID) & "_" & ReadyGuid(pDocumentNameID)
    '                If IO.File.Exists(cGlobal.AppPath & "Reports\" & pFilename & ".mrt") = False Then
    '                    'Return JsonDictionary.Serialize("Error", "The print out template file for this invoice type does not exist. Technical note: the expected filename is " & pFilename)
    '                    If String.IsNullOrWhiteSpace(pTemplate) Then
    '                        pFilename = "SalesOrder"
    '                    Else
    '                        pFilename = pTemplate
    '                    End If
    '                End If
    '            End If
    '            pReport.SourceFilename = pFilename
    '            pReport.VariablesOptional("DocumentName") = CStr2(pDocumentName).ToUpper()
    '            pReport.Report.GetComponentByName("Page1").Page.Watermark.Image = Nothing
    '            If pDrDocumentControl IsNot Nothing Then
    '                If String.IsNullOrWhiteSpace(CStr2(pDrDocumentControl("PermitNum"))) = False Then
    '                    Dim pBand As StiComponent = pReport.Report.GetComponentByName("BandPermit")
    '                    If pBand IsNot Nothing Then
    '                        pBand.Enabled = True
    '                    End If
    '                End If
    '            End If

    '        Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
    '            pFilename = "Invoice"
    '            pModuleSource = "Revenue > Invoicing"
    '            If CStr2(pTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
    '                pDocumentName = "Withdrawal Slip"
    '                pModuleSource = "Revenue > Invoicing"
    '            ElseIf CStr2(pTemplate).ToLower() = "PickList".ToLower() Then
    '                pDocumentName = "Pick List"
    '                pModuleSource = "Revenue > Sales Ordering"
    '            Else
    '                Select Case pTransTypeProper
    '                    Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
    '                        pDocumentName = CStr2(pDR("DocumentName"))
    '                        pModuleSource = "Revenue > Invoicing"
    '                    Case TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder
    '                        pModuleSource = "Disbursement > PO"
    '                    Case TransInvoice.SaveSalesOrderJson.eTransType.Quote
    '                        pModuleSource = "Revenue > Price Quote"
    '                    Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
    '                        pModuleSource = "Revenue > Sales Ordering"
    '                End Select
    '            End If
    '            'If String.IsNullOrWhiteSpace(pTemplate) = False Then ' removed this condition
    '            Dim pDrDocumentType = WebRequest.Instance.Business.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, pModuleSource)
    '            If pDrDocumentType IsNot Nothing Then
    '                pDocumentNameID = CStr2(pDrDocumentType("ID"))
    '            End If
    '            'End If

    '            If pDocumentNameID Is Nothing Then
    '                pDocumentNameID = CStr2(pDR("DocumentNameID"))
    '            End If
    '            If pDocumentNameID = "" Then
    '            Else
    '                pFilename &= "_" & ReadyGuid(WebRequest.Instance.OrgID) & "_" & ReadyGuid(pDocumentNameID)
    '                If IO.File.Exists(cGlobal.AppPath & "Reports\" & pFilename & ".mrt") = False Then
    '                    'Return JsonDictionary.Serialize("Error", "The print out template file for this invoice type does not exist. Technical note: the expected filename is " & pFilename)
    '                    If String.IsNullOrWhiteSpace(pTemplate) Then
    '                        pFilename = "Invoice"
    '                    Else
    '                        pFilename = pTemplate
    '                    End If
    '                End If
    '            End If
    '            pReport.SourceFilename = pFilename
    '            pReport.VariablesOptional("DocumentName") = pDocumentName.ToUpper()
    '            pReport.Report.GetComponentByName("Page1").Page.Watermark.Image = Nothing
    '            If pDrDocumentControl IsNot Nothing Then
    '                If String.IsNullOrWhiteSpace(CStr2(pDrDocumentControl("PermitNum"))) = False Then
    '                    Dim pBand As StiComponent = pReport.Report.GetComponentByName("BandPermit")
    '                    If pBand IsNot Nothing Then
    '                        pBand.Enabled = True
    '                    End If
    '                End If
    '            End If
    '        Case TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder
    '            pReport.SourceFilename = "PurchaseOrder"

    '        Case Else
    '            Throw New NotImplementedException()

    '    End Select

    '    pDS = WebRequest.Instance.Business.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pInvoiceID), False)
    '    pDT = pDS.Tables("Stocks")
    '    Dim pVatable As Decimal
    '    Dim pVat As Decimal
    '    For Each pDR In pDT.Rows
    '        pVat += CDec2(pDR("TaxAmount"))
    '        pVatable += CDec2(pDR("TaxSales"))
    '    Next pDR

    '    pDT = pDS.Tables("Invoices")
    '    pDT.Columns.Add("CustomerContactInfos")
    '    pDT.Columns.Add("TotalVatable", GetType(Decimal))
    '    pDT.Columns.Add("TotalVatAmount", GetType(Decimal))
    '    For Each pDR In pDT.Rows
    '        pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(CStr2(pDR("CustomerID")))
    '        pDR("CustomerContactInfos") = CStr2(pHeaderInfo.ContactInfos)
    '        pDR("TotalVatAmount") = pVat
    '        pDR("TotalVatable") = pVatable
    '    Next pDR

    '    pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
    '    AppendToDebug("vbC3HR5EVPEU! pHeaderInfo.CompanyName: " & pHeaderInfo.CompanyName, Now.Date = #1/4/2017#)
    '    pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
    '    pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress
    '    pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos
    '    pReport.VariablesOptional("Footnote") = CStr2(pDrDocumentControl("Footnote"))

    '    pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(CStr2(pDS.Tables("Invoices").Rows(0)("CustomerID")))
    '    pReport.Variables("CustomerName") = pHeaderInfo.CompanyName
    '    pReport.Variables("CustomerAddress") = pHeaderInfo.CompanyAddress
    '    pReport.Variables("CustomerContactInfos") = pHeaderInfo.ContactInfos

    '    pReport.AddTable("Invoices", pDS.Tables("Invoices"))
    '    pReport.AddTable("Stocks", pDS.Tables("Stocks"))
    '    pReport.ExportXmlFile()
    '    Dim pCopies As Integer
    '    pCopies = CInt2(pDrDocumentControl("CopiesPerPrint"))
    '    For p As Integer = 1 To pCopies
    '        Dim pLabel As String = CStr2(pDrDocumentControl("LabelOfCopy" & Format(p, "00")))
    '        pReport.VariablesOptional("LabelOfCopy") = pLabel
    '        pFilename = Common.ServerPath & "temp\PrintablePdf\" & CreateValidFilename(pUuid) & ";" & pDocumentNameID & ";" & Format(p, "00") & "-" & DateTimeStamp() & ".pdf"
    '        CreateFolder(pFilename)
    '        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
    '    Next p

    '    If CStr2(pTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
    '        WebRequest.Instance.Business.TransInvoice.IncrementWithdrawalSlipPrintCount(pInvoiceID)
    '    ElseIf CStr2(pTemplate).ToLower() = "PickList".ToLower() Then
    '        WebRequest.Instance.Business.TransInvoice.IncrementPickListPrintCount(pInvoiceID)
    '    Else
    '        WebRequest.Instance.Business.TransInvoice.IncrementPrintCount(pInvoiceID)
    '    End If

    '    Return JsonDictionary.Serialize("Result", "ok")

    'End Function

    Public Shared Sub DeleteOldPrintablePdf()

        Dim pFolder As String
        Dim pFilename As String

        pFolder = Common.ServerPath & "temp\PrintablePdf"
        If IO.Directory.Exists(pFolder) Then
            For Each pFile As IO.FileInfo In New IO.DirectoryInfo(pFolder).GetFiles()
                If DateDiff(DateInterval.Hour, pFile.LastWriteTime, Now) >= 24 Then
                    Try
                        pFile.Delete()
                    Catch ex As Exception

                    End Try
                End If
            Next pFile
        End If

    End Sub

    Public Sub UpdateAccountsPayableAmount(vID As String, vOrgID As String, vFollow As String)

        If vFollow <> "vendor" _
            AndAlso vFollow <> "landed" Then

            Exit Sub
        End If

        Dim pSql = "SELECT TransInvoice.TransType" & vbCrLf &
            " ,CustomerID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pDrMaster = mProData.ExecuteRow(pSql)
        If pDrMaster Is Nothing Then
            Exit Sub
        End If
        If CStr2(pDrMaster("TransType")).ToLower() <> "AccountsPayable".ToLower() Then
            Exit Sub
        End If

        Dim pDS As DataSet
        pSql = " WHERE TransInvoice.ID = " & ReadyString(vID)
        pDS = GetReportData(pSql, False)
        Dim pDT = pDS.Tables("Stocks")

        Dim pTotalAmount As Decimal = 0
        mProData.BeginTransaction()
        Dim pIU As InsertUpdate
        Dim pUpdateCount As Integer
        For Each pDR As DataRow In pDT.Rows
            If pDR("UnitPrice") = 0 _
                AndAlso CDec2(pDR("Quantity")) > 0 Then

                If vFollow = "landed" Then
                    pDR("UnitPrice") = mParent.LibraryStock.GetLandedCost(CStr2(pDR("ProductID")), vOrgID) * pDR("ActualQuantity")
                Else
                    pDR("UnitPrice") = mParent.LibraryPriceCanvass.GetVendorPrice(CStr2(pDrMaster("CustomerID")), CStr2(pDR("ProductID"))) * pDR("ActualQuantity")
                End If
                pDR("SubTotal") = pDR("UnitPrice") * pDR("Quantity")
                pIU = New InsertUpdate("TransInvoiceItems")
                pIU("UnitPrice") = pDR("UnitPrice")
                pIU("SubTotal") = pDR("SubTotal")
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                mProData.ExecuteSQL(pSql)
                pUpdateCount += 1
            End If

            pTotalAmount += pDR("SubTotal")
        Next

        If pUpdateCount > 0 Then
            pIU = New InsertUpdate("TransInvoice")
            pIU("TotalAmount") = Math.Round(pTotalAmount, 2)
            pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
            mProData.ExecuteSQL(pSql)
            CacheInvoicesWithCredit()
        End If
        mProData.CommitTransaction()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vFieldName">Quantity or OrderedQuantity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTotalSubTotal(vID As String, vFieldName As String) As Decimal

        Dim pSql As String = "SELECT SUM(" & vFieldName & " * UnitPrice)" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
        Dim pOut As Decimal = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Sub CacheCommittedQtys(vID As String)

        Dim pSql = "SELECT FowstID" & vbCrLf &
            ",TransType" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pDrInvoice As DataRow = mProData.ExecuteRow(pSql)
        If pDrInvoice Is Nothing Then
            Throw New ApplicationException("Invoice does not exist. CacheCommittedQtys. ref: vbNGDSKTCKSU!")
        End If

        pSql = "SELECT SUM(OrderedQuantity * ActualQuantity) AS Qty" & vbCrLf &
            ",ProductID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID) & vbCrLf &
            " GROUP BY ProductID"
        Dim pDtItems = mProData.Execute(pSql)
        For Each pDrItem As DataRow In pDtItems.Rows
            mParent.LibraryStock.CacheCommittedQty(CStr2(pDrItem("ProductID")), CStr2(pDrInvoice("FowstID")))
        Next pDrItem

    End Sub

    Public Function SaveOrigFields(vID As String) As Integer

        Dim pSql = "SELECT CustomerOrigID" & vbCrLf &
            ",CustomerID" & vbCrLf &
            ",CustomerFowstOrigID" & vbCrLf &
            ",CustomerFowstID" & vbCrLf &
            ",CustomerBinOrigID" & vbCrLf &
            ",CustomerBinID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pDR As DataRow = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return 0
        End If

        Dim pOut = 0
        Dim pIU = New InsertUpdate("TransInvoice")
        If pDR("CustomerOrigID") Is DBNull.Value _
            AndAlso pDR("CustomerID") IsNot DBNull.Value _
            AndAlso CStr2(pDR("CustomerOrigID")) <> CStr2(pDR("CustomerID")) Then

            pIU("CustomerOrigID") = ReadyString(pDR("CustomerID"))
            pOut += 1
        End If
        If pDR("CustomerFowstOrigID") Is DBNull.Value _
            AndAlso pDR("CustomerFowstID") IsNot DBNull.Value _
            AndAlso CStr2(pDR("CustomerFowstOrigID")) <> CStr2(pDR("CustomerFowstID")) Then

            pIU("CustomerFowstOrigID") = ReadyString(pDR("CustomerFowstID"))
            pOut += 1
        End If
        If pDR("CustomerBinOrigID") Is DBNull.Value _
            AndAlso pDR("CustomerBinID") IsNot DBNull.Value _
            AndAlso CStr2(pDR("CustomerBinOrigID")) <> CStr2(pDR("CustomerBinID")) Then

            pIU("CustomerBinOrigID") = ReadyString(pDR("CustomerBinID"))
            pOut += 1
        End If
        If pOut = 0 Then
            Return 0
        End If
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Function

    Public Function GetActivityQuotation(vID As String, vDontIncludeSalesOrder As Boolean) As DataTable

        Dim pLastModifiedByName = BrowserController.ComposePeopleName()
        Dim pSql = "SELECT TOP 1 'Quotation' AS Activity" & vbCrLf &
            ",TransInvoice.ID" & vbCrLf &
            ",'SQ-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(TransInvoice.QuotationNo, 0)) AS RefNum" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            ",TransInvoice.LastUpdated" & vbCrLf &
            "," & pLastModifiedByName & " AS LastModifiedByName" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vID)
        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return pDT
        End If
        If vDontIncludeSalesOrder = False Then
            Dim pDtSalesOrder = GetActivitySalesOrder(vID:=Nothing, vDontIncludeInvoice:=False, vQuotationID:=CStr2(pDT.Rows(0)("ID")))
            For Each pDrSalesOrder As DataRow In pDtSalesOrder.Rows
                pDT.ImportRow(pDrSalesOrder)
            Next
        End If

        pDT.DefaultView().Sort = "LastUpdated"
        pDT = pDT.DefaultView().ToTable()

        Return pDT

    End Function

    Public Function GetActivitySalesOrder(vID As String, vDontIncludeInvoice As Boolean, vQuotationID As String) As DataTable

        Dim pLastModifiedByName = BrowserController.ComposePeopleName()
        Dim pSql = "SELECT TOP 1 'Sales Order' AS Activity" & vbCrLf &
            ",TransInvoice.ID" & vbCrLf &
            ",'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo, 0)) AS RefNum" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            ",TransInvoice.LastUpdated" & vbCrLf &
            "," & pLastModifiedByName & " AS LastModifiedByName" & vbCrLf &
            ",TransInvoice.QuotationID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
        If vQuotationID IsNot Nothing Then
            pSql &= " WHERE TransInvoice.QuotationID = " & ReadyString(vQuotationID)
        Else
            pSql &= " WHERE TransInvoice.ID = " & ReadyString(vID)
        End If
        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return pDT
        End If
        Dim pDR As DataRow = pDT.Rows(0)
        If vQuotationID Is Nothing Then
            If IsGuid(pDR("QuotationID")) Then
                Dim pDtQuotation = GetActivityQuotation(
                    vID:=CStr2(pDR("QuotationID")),
                    vDontIncludeSalesOrder:=True)
                If pDtQuotation.Rows.Count > 0 Then
                    pDT.ImportRow(pDtQuotation.Rows(0))
                End If
            End If
        End If

        If vDontIncludeInvoice = False Then
            Dim pDtInvoice = GetActivityInvoice(Nothing, CStr2(pDT.Rows(0)("ID")))
            For Each pDrInvoice As DataRow In pDtInvoice.Rows
                pDT.ImportRow(pDrInvoice)
            Next
        End If
        pDT.DefaultView().Sort = "LastUpdated"
        pDT = pDT.DefaultView().ToTable()

        Return pDT

    End Function

    Public Function GetActivityInvoice(vID As String, vOrderID As String) As DataTable

        Dim pLastModifiedByName = BrowserController.ComposePeopleName()
        Dim pSql = "SELECT TOP 1 'Invoice' AS Activity" & vbCrLf &
            ",TransInvoice.ID" & vbCrLf &
            ",'SI-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(TransInvoice.InvoiceNumber, 0)) AS RefNum" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            ",ISNULL(TransInvoice.InvoiceDate, TransInvoice.LastUpdated) AS LastUpdated" & vbCrLf &
            "," & pLastModifiedByName & " AS LastModifiedByName" & vbCrLf &
            ",TransInvoice.SalesOrderID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
        If vID IsNot Nothing Then
            pSql &= " WHERE TransInvoice.ID = " & ReadyString(vID)
        Else
            pSql &= " WHERE TransInvoice.SalesOrderID = " & ReadyString(vOrderID)
        End If
        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return pDT
        End If

        If String.IsNullOrWhiteSpace(vID) Then
            Dim pDR2 = WebRequest.Instance.Business.TransInvoice.BySalesOrderID(vOrderID)
            If pDR2 IsNot Nothing Then
                vID = CStr2(pDR2("ID"))
            End If
        End If

        If String.IsNullOrWhiteSpace(vID) = False Then
            Dim pDrDelivery = GetDeliver(vID)
            If pDrDelivery IsNot Nothing _
                AndAlso (
                    CBool2(pDrDelivery("IsDelivered")) _
                    OrElse CStr2(pDrDelivery("DeliveryStatus")) <> ""
                ) Then

                Dim pDrNew = pDT.NewRow()
                If CBool2(pDrDelivery("IsDelivered")) Then
                    pDrNew("Activity") = "Delivery"
                Else
                    pDrNew("Activity") = CStr2(pDrDelivery("DeliveryStatus"))
                End If
                pDrNew("RefNum") = pDrDelivery("RoutingSheetNumber")
                pDrNew("FowstName") = pDrDelivery("DeliveryTruckFowstName")
                pDrNew("LastUpdated") = pDrDelivery("DeliveryEncodingDate")
                pDrNew("LastModifiedByName") = ""
                pDT.Rows.Add(pDrNew)
            End If
            Dim pDtPayments = mParent.TransCustomerPayment.GetPaymentIdsByInvoiceID(vID)
            For Each pDrPayment As DataRow In pDtPayments.Rows
                Dim pDS As DataSet = WebRequest.Instance.Business.TransCustomerPayment.GetPaymentData(CStr2(pDrPayment("CustomerPaymentID")))
                Dim pDtPayment = pDS.Tables("Master")
                If pDtPayment.Rows.Count > 0 Then
                    Dim pDrPaymentHeader = pDtPayment.Rows(0)
                    Dim pDrNew = pDT.NewRow()
                    pDrNew("Activity") = "Payment"
                    pDrNew("RefNum") = pDrPaymentHeader("ReceiptNumber")
                    pDrNew("FowstName") = ""
                    pDrNew("LastUpdated") = pDrPaymentHeader("PaymentDate")
                    pDrNew("LastModifiedByName") = pDrPaymentHeader("EmployeeFullName")
                    pDT.Rows.Add(pDrNew)
                End If
            Next
        End If

        Dim pDR As DataRow = pDT.Rows(0)
        If vOrderID Is Nothing Then
            If IsGuid(pDR("SalesOrderID")) Then
                Dim pDtSalesOrder = GetActivitySalesOrder(
                    vID:=CStr2(pDR("SalesOrderID")),
                    vDontIncludeInvoice:=True,
                    vQuotationID:=Nothing)
                For Each pDrSalesOrder In pDtSalesOrder.Rows
                    pDT.ImportRow(pDrSalesOrder)
                Next
            End If
        End If

        pDT.DefaultView().Sort = "LastUpdated"
        pDT = pDT.DefaultView().ToTable()

        Return pDT

    End Function

    Public Shared Sub FillElapsedTimeFromPreviousActivity(ByRef rDT As DataTable)

        Dim pDrLast As DataRow

        rDT.Columns.Add("TimeElapsed")
        rDT.DefaultView.Sort = "LastUpdated"
        rDT = rDT.DefaultView.ToTable()
        For Each pDR As DataRow In rDT.Rows
            If pDrLast Is Nothing Then
                pDR("TimeElapsed") = "N/A"
                pDrLast = pDR
            Else
                Try
                    If pDR("LastUpdated") Is DBNull.Value Then
                        pDR("TimeElapsed") = "n/a"
                        Continue For
                    End If
                    Dim pSeconds = (CType(pDR("LastUpdated"), Date) - CType(pDrLast("LastUpdated"), Date)).TotalSeconds
                    Dim pElapsedTime = FormatDurationBySeconds(pSeconds)
                    pDR("TimeElapsed") = CStr2(pElapsedTime)
                    pDrLast = pDR
                Catch ex As Exception
                    AppendToLog("vbKARYBSF4HY! " & ex.Message)
                End Try
            End If
        Next

    End Sub

    Public Function SalesOrderToPurchaseOrder(vID As String, vOrgID As String, ByRef oError As String) As String

        oError = Nothing
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pDS = GetReportData("WHERE TransInvoice.ID = " & ReadyString(vID), False)

        pDT = pDS.Tables("Invoices")
        If pDT.Rows.Count = 0 Then
            oError = "Invoice with ID, " & vID & ", does not exist. ref: vb8KTWCA9DSD!"
            Return Nothing
        End If
        pDR = pDT.Rows(0)
        Dim pDtStocks = pDS.Tables("Stocks")
        Dim pDtVendor = pDtStocks.DefaultView.ToTable(True, "VendorID")

        For Each pDrVendor As DataRow In pDtVendor.Rows
            If pDrVendor("VendorID") Is DBNull.Value Then
                Continue For
            End If
            Dim pInput = New SaveSalesOrderJson
            pInput.TransType = "PurchaseOrder"
            pInput.SalesOrderID = vID
            pInput.CustomerID = CStr2(pDrVendor("VendorID"))
            pInput.FowstID = CStr2(pDR("FowstID"))
            pInput.DeviceID = CStr2(pDR("DeviceID"))
            pInput.UserID = CStr2(pDR("UserID"))
            pInput.BillingAddressID = CStr2(pDR("BillingAddressID"))
            pInput.DeliveryAddressID = CStr2(pDR("DeliveryAddressID"))
            pInput.CopyProductsFromInvoiceID = vID
            pInput.CopyProductsWithVendorID = CStr2(pDrVendor("VendorID"))
            pInput.SaveAndFinalize = True
            pInput.DateNeeded = Now
            pInput.Verify(oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                Return Nothing
            End If
            mProData.BeginTransaction()
            Dim pPurchaseOrderID = Save(pInput, vOrgID, oError)
            Dim pIU = New InsertUpdate("TransInvoiceItems")
            pIU("PurchaseOrderID") = ReadyString(pPurchaseOrderID)
            Dim pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(vID) & vbCrLf &
                " AND VendorID = " & ReadyString(pDrVendor("VendorID")))
            Dim pCount = mProData.ExecuteSQL(pSql) ' output is for debugging only
            mProData.CommitTransaction()
        Next

        Return Nothing

    End Function

    Public Sub UpdateProductTax(vInvoiceID As String)

        Dim pDT As DataTable
        Dim pSql As String
        Dim pIsVatable = mParent.SystemDefaults.Vatable

        ' included but not used: TaxSales, VATableSale, TaxAmount (used for debugging only)
        pSql = "SELECT ID" &
            ",SubTotal" & vbCrLf &
            ",TaxPercent" & vbCrLf &
            ",TaxSales" & vbCrLf &
            ",VATableSale" & vbCrLf &
            ",TaxAmount" & vbCrLf &
            " FROM TransInvoiceItems" &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID) &
            " ORDER BY SubTotal"
        pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU As New InsertUpdate("TransInvoiceItems")
            If Not pIsVatable Then
                pIU("VATableSale") = 0
                pIU("VATExemptSale") = 0
                pIU("NonVATSale") = pDR("SubTotal")
                pIU("ZeroRatedSale") = 0
                pIU("TaxSales") = 0
                pIU("TaxAmount") = 0
            Else
                pIU("VATableSale") = pDR("SubTotal")
                pIU("VATExemptSale") = 0
                pIU("NonVATSale") = 0
                pIU("ZeroRatedSale") = 0
                pIU("TaxSales") = RoundDown(pDR("SubTotal") / (1 + pDR("TaxPercent") / 100), 4)
                pIU("TaxAmount") = pDR("SubTotal") - CDec2(pIU("TaxSales"))
            End If

            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Sub

    Public Function SegregateSalesOrderByArea(vID As String, rCreator As Creator)

        Dim pSql As String
        Dim pDrSalesOrder As DataRow
        Dim pIU As InsertUpdate
        Dim pSubTotals As Decimal

        pSql = "SELECT * FROM TransInvoice WHERE ID = " & ReadyString(vID)
        pDrSalesOrder = mProData.ExecuteRow(pSql)
        If pDrSalesOrder Is Nothing Then
            Throw New ApplicationException("Sales order with ID " & vID & " does not exist. SegregateSalesOrderByArea. ref: vbG7PHYVFFDU!")
        End If

        pSql = "  SELECT TOP 10 MobizLibraryPIMProduct.ProductName" & vbCrLf &
            "  ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            "  ,ISNULL(tblArea.Area, '') AS Area" & vbCrLf &
            "  ,TransInvoiceItems.ID" & vbCrLf &
            "  ,TransInvoiceItems.ProductID" & vbCrLf &
            "  ,ISNULL(TransInvoiceItems.TermNumericValue, 0) AS TermNumericValue" & vbCrLf &
            "  ,TransInvoiceItems.OrderedQuantity" & vbCrLf &
            "  ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            "  ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "  ,TransInvoiceItems.TermDiscount" & vbCrLf &
            "  ,TransInvoiceItems.NetDue" & vbCrLf &
            "  ,TransInvoiceItems.UnitPrice" & vbCrLf &
            "  ,TransInvoiceItems.Cost" & vbCrLf &
            "  ,TransInvoiceItems.SubTotal" & vbCrLf &
            "  ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            "  ,TransInvoiceItems.TaxPercent" & vbCrLf &
            "  ,TransInvoiceItems.TaxAmount" & vbCrLf &
            "  ,TransInvoiceItems.TaxSales" & vbCrLf &
            "  ,TransInvoiceItems.DiscountAmount" & vbCrLf &
            "  ,TransInvoiceItems.DiscountSales" & vbCrLf &
            "  ,TransInvoiceItems.VATExemptSale" & vbCrLf &
            "  ,TransInvoiceItems.ZeroRatedSale" & vbCrLf &
            "  ,TransInvoiceItems.NonVATSale" & vbCrLf &
            "  ,TransInvoiceItems.VATableSale" & vbCrLf &
            "  ,TransInvoiceItems.TradeDiscountLess" & vbCrLf &
            "  ,TransInvoiceItems.VendorID" & vbCrLf &
            "  FROM TransInvoiceItems" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 LibraryFowstBin.Area" & vbCrLf &
            "     FROM LibraryFowstBinProduct" & vbCrLf &
            "     LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = LibraryFowstBinProduct.BinID" & vbCrLf &
            "     WHERE LibraryFowstBinProduct.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "     AND LibraryFowstBinProduct.FowstID = " & ReadyString(pDrSalesOrder("FowstID")) & vbCrLf &
            "  ) tblArea" & vbCrLf &
            "  LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vID)

        Dim pDtItems = mProData.Execute(pSql)
        Dim pDtAreas = pDtItems.DefaultView.ToTable(True, "Area", "TermNumericValue")
        Dim pDefaultArea As String = ""
        pDtAreas.DefaultView.RowFilter = "Area = ''"
        If pDtAreas.DefaultView.Count = 0 Then
            pDefaultArea = pDtAreas.Rows(0)("Area")
        End If
        Dim pTransferCount = 0
        mProData.BeginTransaction()
        Dim pNum = GetMaxSoNumber(CStr2(pDrSalesOrder("DeviceID")), CStr2(pDrSalesOrder("FowstID"))) + 1
        For Each pDrArea As DataRow In pDtAreas.Rows
            If pDrArea("Area") = pDefaultArea Then
                rCreator.Area = Truncate(pDrArea("Area"), 50)
                rCreator.Remarks = "Area: " & pDrArea("Area") & ". " &
                    "Payment Term: " & CInt2(pDrArea("TermNumericValue")) & ". " &
                    rCreator.Remarks
                Continue For
            End If

            Dim pDvItems = New DataView(pDtItems, "Area = " & ReadyString(pDrArea("Area")) & vbCrLf &
                " AND TermNumericValue = " & pDrArea("TermNumericValue"), "", DataViewRowState.CurrentRows)
            If pDvItems.Count > 0 Then
                Dim pSalesOrderID = Guid.NewGuid.ToString()
                pTransferCount += pDvItems.Count
                pSubTotals = 0
                Dim pItemIDs = ""
                For Each pDrvItem In pDvItems
                    If pItemIDs <> "" Then
                        pItemIDs &= ", "
                    End If
                    pItemIDs &= ReadyString(pDrvItem("ID"))
                    pSubTotals += CDec2(pDrvItem("SubTotal"))
                Next pDrvItem

                'pIU = FranchiseeDataExporter.DataRowToInsertUpdate(pDrSalesOrder)
                'pIU.TableName = "TransInvoice"
                'pIU("ID") = ReadyString(pSalesOrderID)
                'pIU("Area") = ReadyString(pDrArea("Area"))
                'pIU("ParentOrderID") = ReadyString(vID)
                'pNum += 1 ' +1 on init and another +1 here. not wrong
                'pIU("SalesOrderNo") = pNum
                'pIU("PaymentTerm") = CInt2(pDrArea("TermNumericValue"))
                'pIU("TotalAmount") = pSubTotals
                'pIU("Remarks") = ReadyString("Area: " & pDrArea("Area") & ". " &
                '    "Payment Term: " & CInt2(pDrArea("TermNumericValue")) & ". " &
                '    CStr2(pDrSalesOrder("Remarks")))
                'pSql = pIU.GetInsert()
                'mProData.ExecuteSQL(pSql)

                pSql = "UPDATE TransInvoiceItems" & vbCrLf &
                    " SET InvoiceID = " & ReadyString(pSalesOrderID) &
                    " WHERE ID IN (" & pItemIDs & ")"
                mProData.ExecuteSQL(pSql)
            End If
        Next pDrArea
        If pTransferCount > 0 Then
            pSubTotals = GetTotalSubTotal(vID, "OrderedQuantity")
            pSql = "UPDATE TransInvoice" &
                " SET TotalAmount = " & pSubTotals & vbCrLf &
                ",Area = " & ReadyString(pDefaultArea) & vbCrLf &
                " WHERE ID = " & ReadyString(vID)
            mProData.ExecuteSQL(pSql)
        End If
        mProData.CommitTransaction()

    End Function

    Private Sub UpdateInvoiceID(vSalesOrderID As String, vInvoiceID As String)

        Dim pIU = New InsertUpdate("TransInvoice")
        pIU("InvoiceID") = ReadyString(vInvoiceID)
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vSalesOrderID))
        Dim pAffectedCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only

    End Sub

    Public Function InvoicableOrdersCount() As Integer

        Dim pSql = " SELECT COUNT(*)" & vbCrLf &
            " FROM TransInvoice " & vbCrLf &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND TransInvoice.TransType = 'SalesOrder'" & vbCrLf &
            " AND TransInvoice.FinalizePickListByID IS NOT NULL" & vbCrLf &
            " AND TransInvoice.InvoiceID IS NULL" & vbCrLf &
            " AND TransInvoice.ID NOT IN (" & vbCrLf &
            "   SELECT SalesOrderID" & vbCrLf &
            "   FROM TransInvoice" & vbCrLf &
            "   WHERE SalesOrderID IS NOT NULL" & vbCrLf &
            "   AND TransType = 'Invoice'" & vbCrLf &
            " )" & vbCrLf
        Dim pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function AutoFinalizePurchaseOrdersFromVendor(vOrgID As String) As Integer

        Dim pSqlFilter = "    SELECT TransInvoice.ID" & vbCrLf &
            "    FROM TransInvoice" & vbCrLf &
            "    JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "    JOIN TransInvoice tblPo ON tblPO.ID = TransInvoiceItems.PurchaseOrderID" & vbCrLf &
            "    WHERE TransInvoice.FinalizePickListByID IS NULL" & vbCrLf &
            "    GROUP BY TransInvoice.ID" & vbCrLf &
            "    ,TransInvoice.TransType" & vbCrLf &
            "    HAVING MIN(ISNULL(CONVERT(int, tblPo.FinalizedForAutoInvoice), 0)) = 1" & vbCrLf

        Dim pSql = " -- auto finalize FC" & vbCrLf &
            " UPDATE TransInvoice" & vbCrLf &
            " SET FinalizePickListByID = " & ReadyString(modClient.EmployeeWebGuidID) & vbCrLf &
            " ,FinalizedForAutoInvoice = 1" & vbCrLf &
            " WHERE TransInvoice.FinalizePickListByID IS NULL" & vbCrLf &
            " AND ID IN (" & vbCrLf &
            pSqlFilter &
            ")"
        Dim pOut = mProData.ExecuteSQL(pSql)

        Dim d As New DashBoardsAndReports
        d.GetSalesOrderReportFinder("", "inv.ID IN (" & pSqlFilter & ")", DashBoardsAndReports.eSalesOrderReportMode.Recache,
            vFowstID:=Nothing,
            vStatus:=Nothing,
            vIsInVendorPortal:=False,
            vOrderDateFrom:=Nothing,
            vOrderDateTo:=Nothing,
            vVendorID:=Nothing,
            vAreas:=Nothing,
            vSortBy:=Nothing)

        AutoCreateInvoice(vOrgID)

        Return pOut

    End Function

    Public Class CountAndAmount
        Public DraftCount As Integer ' excluding backorder
        Public DraftAmount As Decimal ' excluding backorder
        Public FinalizedCount As Integer ' excluding backorder
        Public FinalizedAmount As Decimal ' excluding backorder

        Public BackorderDraftCount As Integer
        Public BackorderDraftAmount As Decimal
        Public BackorderFinalizedCount As Integer
        Public BackorderFinalizedAmount As Decimal
    End Class

    Public Function GetCountAndAmount(vTransType As String, vNumField As String,
        vFowstID As String, vDateFrom As Date, vDateTo As Date,
        vAmountField As String) As CountAndAmount

        vDateFrom = vDateFrom.Date
        vDateTo = vDateTo.Date

        Dim pSql = " SELECT SUM(tblItems." & vAmountField & ") AS TotalAmount" & vbCrLf &
            " ,COUNT(*) AS Count" & vbCrLf &
            " ,CASE WHEN ISNULL(TransInvoice." & vNumField & ", 0) > 0 THEN 1" & vbCrLf &
            " ELSE 0 END AS IsFinalized" & vbCrLf &
            " ,CASE WHEN TransInvoice.BackorderSourceInvoiceID IS NULL THEN 0" & vbCrLf &
            " ELSE 1 END AS IsBackOrder" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            "  	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
            "    ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
            "    ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
            "    ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
            "  	FROM TransInvoiceItems" & vbCrLf &
            "    WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "    AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
            "    OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
            "    AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
            "  ) tblItems    " & vbCrLf &
            " WHERE TransType = " & ReadyString(vTransType) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(Format(vDateFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vDateTo, cGlobal.FormatDateTimeSql)) & vbCrLf &
            " GROUP BY CASE WHEN ISNULL(TransInvoice." & vNumField & ", 0) > 0 THEN 1" & vbCrLf &
            " ELSE 0 END" & vbCrLf &
            " ,CASE WHEN TransInvoice.BackorderSourceInvoiceID IS NULL THEN 0" & vbCrLf &
            " ELSE 1 END" & vbCrLf
        Dim pDT = mProData.Execute(pSql)
        Dim pOut = New CountAndAmount

        For Each pDR In pDT.Rows
            If CBool2(pDR("IsFinalized")) Then
                If CBool2(pDR("IsBackOrder")) Then
                    pOut.BackorderFinalizedCount = CInt2(pDR("Count"))
                    pOut.BackorderFinalizedAmount = CDec2(pDR("TotalAmount"))
                Else
                    pOut.FinalizedCount = CInt2(pDR("Count"))
                    pOut.FinalizedAmount = CDec2(pDR("TotalAmount"))
                End If
            Else
                If CBool2(pDR("IsBackOrder")) Then
                    pOut.BackorderDraftCount = CInt2(pDR("Count"))
                    pOut.BackorderDraftAmount = CDec2(pDR("TotalAmount"))
                Else
                    pOut.DraftCount = CInt2(pDR("Count"))
                    pOut.DraftAmount = CDec2(pDR("TotalAmount"))
                End If
            End If
        Next

        Return pOut

    End Function

    Private Shared mAutoCreateInvoiceLock As New Object

    Public Function AutoCreateInvoice(vOrgID As String) As Integer

        SyncLock mAutoCreateInvoiceLock
            Dim pSql = "SELECT ID, LastUpdated" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE FinalizedForAutoInvoice = 1" & vbCrLf &
                " AND TransType = 'SalesOrder'" & vbCrLf &
                " AND ISNULL(TransInvoice.DontAutoInvoice, 0) = 0" & vbCrLf &
                " AND ID NOT IN (" & vbCrLf &
                "   SELECT SalesOrderID" & vbCrLf &
                "   FROM TransInvoice tblInvoice" & vbCrLf &
                "   WHERE tblInvoice.TransType = 'Invoice'" & vbCrLf &
                "   AND SalesOrderID IS NOT NULL" & vbCrLf &
                ")" & vbCrLf &
                " ORDER BY LastUpdated"
            Dim pDT = mProData.Execute(pSql)
            If pDT.Rows.Count = 0 Then
                Return 0
            End If

            Dim pFowstID = mParent.SystemDefaults.AutoCreateInvoiceFowstID
            If String.IsNullOrWhiteSpace(pFowstID) Then
                Throw New ApplicationException("SystemDefaults.AutoCreateInvoiceFowstID must not be blank. ref: vbW8K9YQZZW6!")
            End If

            Dim pDeviceID = mParent.SystemDefaults.AutoCreateInvoiceDeviceID
            If String.IsNullOrWhiteSpace(pDeviceID) Then
                Throw New ApplicationException("SystemDefaults.AutoCreateInvoiceDeviceID must not be blank. ref: vbETHF5ZL4BF!")
            End If

            Dim pInvoiceTypeID = mParent.SystemDefaults.AutoCreateInvoiceTypeID
            If String.IsNullOrWhiteSpace(pInvoiceTypeID) Then
                Throw New ApplicationException("SystemDefaults.AutoCreateInvoiceTypeID must not be blank. ref: vbDAHL6YLP2V!")
            End If

            For Each pDR In pDT.Rows
                SalesOrderToInvoice(CStr2(pDR("ID")), pInvoiceTypeID, pFowstID, pDeviceID, vOrgID)
            Next

            Return pDT.Rows.Count
        End SyncLock

    End Function

    Shared mSalesOrderToInvoiceLock As New Object

    ''' <summary>
    ''' return the ID of the generated invoice
    ''' </summary>
    ''' <param name="vSalesOrderID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalesOrderToInvoice(vSalesOrderID As String, vInvoiceTypeID As String, vInvoiceFowstID As String, vInvoiceDeviceID As String,
        vOrgID As String) As String

        Dim pSourceSalesOrderDr As DataRow
        Dim pSql As String

        SyncLock mSalesOrderToInvoiceLock
            pSourceSalesOrderDr = mProData.ExecuteRow("SELECT * FROM TransInvoice WHERE ID = " & ReadyString(vSalesOrderID))
            If pSourceSalesOrderDr Is Nothing Then
                Throw New ApplicationException("ref: vbA2HN7K5L4P! The source sales order ID, " & vSalesOrderID & ", does not match any record.")
            End If
            If CBool2(pSourceSalesOrderDr("DontAutoInvoice")) Then
                Return Nothing
            End If
            mProData.BeginTransaction()
            pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE SalesOrderID = " & ReadyString(vSalesOrderID)
            If CInt2(mProData.ExecuteScalar(pSql)) > 0 Then
                Throw New ApplicationException("ref: vbTDHPH46TC6! The source sales order ID, " & vSalesOrderID & ", was already invoiced.")
            End If
            Dim pIU = Business.DataRowToInsertUpdate(pSourceSalesOrderDr)
            pIU.TableName = "TransInvoice"

            Dim pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("SalesOrderID") = ReadyString(vSalesOrderID)
            pIU("LastUpdated") = "GETDATE()"
            pIU("DateCreated") = "GETDATE()"
            pIU("FinalizedForAutoInvoice") = "1"
            pIU("TransType") = ReadyString(SaveSalesOrderJson.eTransType.Invoice.ToString())

            If (String.IsNullOrWhiteSpace(vInvoiceTypeID)) Then
                vInvoiceTypeID = CStr2(pSourceSalesOrderDr("DocumentNameID"))
                If String.IsNullOrWhiteSpace(vInvoiceTypeID) Then
                    mProData.RollbackTransaction()
                    Return ""
                    'Throw New ApplicationException("Invoice type is required to convert sales order to invoice.")
                End If
            End If
            pIU("DocumentNameID") = ReadyString(vInvoiceTypeID)

            If String.IsNullOrWhiteSpace(vInvoiceFowstID) Then
                vInvoiceFowstID = CStr2(pSourceSalesOrderDr("FowstID"))
                If String.IsNullOrWhiteSpace(vInvoiceFowstID) Then
                    Throw New ApplicationException("FowstID is required to convert sales order to invoice.")
                End If
            End If
            pIU("FowstID") = ReadyString(vInvoiceFowstID)

            If String.IsNullOrWhiteSpace(vInvoiceDeviceID) Then
                vInvoiceDeviceID = CStr2(pSourceSalesOrderDr("DeviceID"))
                If String.IsNullOrWhiteSpace(vInvoiceDeviceID) Then
                    Throw New ApplicationException("DeviceID is required to convert sales order to invoice.")
                End If
            End If
            pIU("DeviceID") = ReadyString(vInvoiceDeviceID)
            Dim pInvoiceNum As Integer
            VerifyCounter(vInvoiceDeviceID, vInvoiceFowstID, pInvoiceNum, vInvoiceTypeID)
            pIU("InvoiceNumber") = pInvoiceNum + 1
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pIU.Remove("SalesOrderNo")

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            CopyProducts(vSalesOrderID, pID, False)
            'Dim pDtStocks = mProData.Execute("SELECT * FROM TransInvoiceItems WHERE InvoiceID = " & ReadyString(vSalesOrderID))
            'For Each pDrStock In pDtStocks.Rows
            '    pIU = Business.DataRowToInsertUpdate(pDrStock)
            '    pIU.TableName = "TransInvoiceItems"
            '    pIU("ID") = "NEWID()"
            '    pIU("InvoiceID") = ReadyString(pID)
            '    pSql = pIU.GetInsert()
            '    mProData.ExecuteSQL(pSql)
            'Next

            Dim pTotal = mProData.ExecuteScalar("SELECT SUM(SubTotal) FROM TransInvoiceItems WHERE InvoiceID = " & ReadyString(pID))
            pIU = New InsertUpdate("TransInvoice")
            pIU("TotalAmount") = pTotal
            pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
            mProData.ExecuteSQL(pSql)
            CacheInvoicesWithCredit(vInvoiceID:=pID)
            AddToInventoryLedger(pID, vOrgID)

            mProData.CommitTransaction()
        End SyncLock

    End Function

    ''' <summary>
    ''' return the ID of the generated invoice
    ''' </summary>
    ''' <param name="vSalesOrderID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyTransaction(vID As String, vTransType As String, vCustomerID As String) As String

        Dim pSourceDr As DataRow

        pSourceDr = mProData.ExecuteRow("SELECT * FROM TransInvoice WHERE ID = " & ReadyString(vID))
        If pSourceDr Is Nothing Then
            Throw New ApplicationException("ref: vbUYP7XCHU8A! The source transaction ID, " & vID & ", does not match any record.")
        End If

        mProData.BeginTransaction()
        Dim pIU = Business.DataRowToInsertUpdate(pSourceDr)
        pIU.TableName = "TransInvoice"

        Dim pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("LastUpdated") = "GETDATE()"
        pIU("InvoiceNumber") = 0
        pIU("PurchaseOrderNo") = 0
        pIU("SalesOrderNo") = 0
        pIU("QuotationNo") = 0
        pIU("InvoiceID") = "NULL"
        pIU("AccountsPayableNum") = 0
        If String.IsNullOrWhiteSpace(vTransType) = False Then
            pIU("TransType") = ReadyString(vTransType)
        End If
        If String.IsNullOrWhiteSpace(vCustomerID) = False Then
            pIU("CustomerID") = ReadyString(vCustomerID)
        End If
        pIU.Remove("SalesOrderNo")
        pIU.Remove("ApprovedDate")
        pIU.Remove("ApprovedByID")
        pIU.Remove("UnapprovedProfitability")
        pIU.Remove("CncUnapprovedReason")
        pIU.Remove("CncApprovedByID")
        pIU.Remove("CncApprovedDate")
        pIU.Remove("FinalizePickListByID")

        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        CopyProducts(vID, pID, False, , True)

        Dim pTotal = mProData.ExecuteScalar("SELECT SUM(SubTotal) FROM TransInvoiceItems WHERE InvoiceID = " & ReadyString(pID))
        pIU = New InsertUpdate("TransInvoice")
        pIU("TotalAmount") = pTotal
        pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
        mProData.ExecuteSQL(pSql)

        mProData.CommitTransaction()

        Return pID

    End Function

    Public Sub SaveTransProfitability(vID As String, vOrgID As String, Optional vRecompute As Boolean = False)

        Dim pUnapprovedCount As Integer
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDtApproved As DataTable

        If vRecompute Then
            pSql = "SELECT ItemID" & vbCrLf &
                ",ApprovedByID" & vbCrLf &
                ",ApprovedDate" &
                " FROM TransProfitability" & vbCrLf &
                " WHERE InvoiceID = " & ReadyString(vID) & vbCrLf &
                " AND ApprovedByID IS NOT NULL"
            pDtApproved = mProData.Execute(pSql)
        Else
            pSql = "SELECT COUNT(*) FROM TransProfitability" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
            Dim pCount = CInt2(mProData.ExecuteScalar(pSql))
            If pCount > 0 Then
                Return
            End If
        End If


        pSql = "SELECT" & vbCrLf &
            " CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS MIN" & vbCrLf &
            ",TransInvoice.TransType" & vbCrLf &
            ",TransInvoice.SalesOrderNo" & vbCrLf &
            " ,TransInvoice.SalesOrderDate" & vbCrLf &
            " ,TransInvoice.InvoiceNumber" & vbCrLf &
            " ,TransInvoice.InvoiceDate" & vbCrLf &
            " ,TransInvoice.UserID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.UnapprovedProfitability" & vbCrLf &
            " ,TransInvoice.FinalizedForApproval" & vbCrLf &
            " ,LibraryDocumentControl.DocumentName" & vbCrLf &
            "," & BrowserController.ComposePeopleName & " AS UpdaterName" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vID)
        Dim pDrInvoice = mProData.ExecuteRow(pSql)
        If pDrInvoice Is Nothing Then
            Throw New ApplicationException("vbZVK8UU2Q4T! SaveTransProfitability no matching ID: " & vID)
        End If
        If CStr2(pDrInvoice("TransType")) = "PurchaseOrder" Then
            Return
        End If
        If vRecompute = False Then
            If CBool2(pDrInvoice("UnapprovedProfitability")) Then
                Return ' this transaction was already computed
            End If
        End If

        pSql = "SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductName" & vbCrLf &
            ",TransInvoiceItems.UnitPrice" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID " & vbCrLf &
            ",TransInvoiceItems.Cost " & vbCrLf &
            ",TransInvoiceItems.OrderedQuantity" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoiceItems.UnitMeasure" & vbCrLf &
            ",TransInvoiceItems.UnitGP" & vbCrLf &
            ",TransInvoiceItems.UnitGPM" & vbCrLf &
            ",TransInvoiceItems.LineGP" & vbCrLf &
            " ,MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.Price AS PriceLevelPrice" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.LastUpdated AS PriceLevelPriceLastUpdated" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            "  LEFT JOIN MobizLibraryPIMPriceLevel ON MobizLibraryPIMPriceLevel.ID = TransInvoiceItems.PriceLevelID" & vbCrLf &
            "  LEFT JOIN MobizLibraryPIMProductPriceLevel ON MobizLibraryPIMProductPriceLevel.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "  AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            "  AND MobizLibraryPIMProductPriceLevel.PriceLevelID = TransInvoiceItems.PriceLevelID" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
        Dim pDtItems = mProData.Execute(pSql)

        mProData.BeginTransaction()
        pSql = "DELETE FROM TransProfitability" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vID)
        mProData.ExecuteSQL(pSql)

        For Each pDrItem As DataRow In pDtItems.Rows
            If CDec2(pDrItem("UnitPrice")) = 0 Then
                Continue For
            End If
            Dim pGpm As Decimal = (pDrItem("UnitPrice") - CDec2(pDrItem("Cost"))) / pDrItem("UnitPrice") * 100

            Dim pCategory As String = mParent.LibraryStock.GetCategory(CStr2(pDrItem("ProductID")))

            Dim pMinGpm As Decimal
            If String.IsNullOrWhiteSpace(pCategory) Then
                pMinGpm = mParent.SystemDefaults.FreeTypeAPriceMinimumGpm
            Else
                Dim pMinGpmByCategory As Decimal? = mParent.LibraryStock.GetMinGpmByCategory(pCategory)
                If pMinGpmByCategory Is Nothing Then
                    pMinGpm = mParent.SystemDefaults.FreeTypeAPriceMinimumGpm
                Else
                    pMinGpm = pMinGpmByCategory
                End If
            End If
            If pGpm >= pMinGpm Then
                Continue For
            End If

            pUnapprovedCount += 1
            pIU = New InsertUpdate("TransProfitability")
            pIU("ProductName") = ReadyString(pDrItem("ProductID"))
            pIU("ProductName") = ReadyString(Truncate(CStr2(pDrItem("ProductName")), 250))
            pIU("CreatedByID") = ReadyString(pDrInvoice("UserID"))
            pIU("CreatedByName") = ReadyString(CStr2(pDrInvoice("UpdaterName")))
            pIU("UnitMeasure") = ReadyString(pDrItem("UnitMeasure"))
            pIU("OrderQuantity") = CDec2(pDrItem("OrderedQuantity"))
            pIU("UnitQty") = CDec2(pDrItem("ActualQuantity"))
            pIU("SellingPrice") = CDec2(pDrItem("UnitPrice"))
            pIU("COGS") = CDec2(pDrItem("Cost"))
            pIU("GpmThreshold") = pMinGpm
            pIU("GP") = CDec2(pDrItem("UnitPrice")) - CDec2(pDrItem("Cost"))
            pIU("GPM") = pGpm
            pIU("LineGp") = (CDec2(pDrItem("UnitPrice")) - CDec2(CDec2(pDrItem("Cost")))) * CDec2(pDrItem("OrderedQuantity"))

            Dim pCogsLastUpdated As Date?
            mParent.LibraryStock.GetLandedCost(CStr2(pDrItem("ProductID")), vOrgID, pCogsLastUpdated) ' the output is not needed. we only need the  pCogsLastUpdated
            If pCogsLastUpdated IsNot Nothing Then
                pIU("CogsLastUpdated") = ReadyString(pCogsLastUpdated)
            End If

            If CDec2(pDrItem("UnitPrice")) = CDec2(pDrItem("PriceLevelPrice")) * CDec2(pDrItem("ActualQuantity")) Then
                pIU("IsPriceFreeTyped") = 0
                pIU("SellingPriceLevel") = ReadyString(Truncate(CStr2(pDrItem("PriceLevel")), 50))
                If IsDate(pDrItem("PriceLevelPriceLastUpdated")) Then
                    pIU("SellingPriceLastUpdated") = ReadyString(pDrItem("PriceLevelPriceLastUpdated"))
                End If
            Else
                pIU("IsPriceFreeTyped") = 1
            End If

            pIU("TransItemCount") = pDtItems.Rows.Count
            pIU("TransDate") = ReadyString(pDrInvoice("LastUpdated"))
            If CStr2(pDrInvoice("TransType")) = "SalesOrder" Then
                pIU("TransMin") = ReadyString("SO " + pDrInvoice("MIN") & "-" & pDrInvoice("SalesOrderNo"))
            ElseIf CStr2(pDrInvoice("TransType")) = "Invoice" Then
                If CStr2(pDrInvoice("DocumentName")) = "" Then
                    pIU("TransMin") = ReadyString("SI " + pDrInvoice("MIN") & "-" & pDrInvoice("InvoiceNumber"))
                Else
                    pIU("TransMin") = ReadyString(pDrInvoice("DocumentName") + pDrInvoice("MIN") & "-" & pDrInvoice("InvoiceNumber"))
                End If
            End If
            pIU("ID") = "NEWID()"
            pIU("InvoiceID") = ReadyString(vID)
            pIU("ItemID") = ReadyString(pDrItem("ID"))
            pIU("ProductID") = ReadyString(pDrItem("ProductID"))
            pIU("UnitQty") = CDec2(pDrItem("ActualQuantity"))

            ' save these dummy data. actual data are saved in UpdateProfitabilityTransactionColumns
            pIU("UnapprovedTransTotalGp") = -1
            pIU("UnapprovedTransTotalGpm") = -1
            pIU("ApprovedTransTotalGp") = -1
            pIU("ApprovedTransTotalGpm") = -1

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

        pIU = New InsertUpdate("TransInvoice")
        pIU("UnapprovedProfitability") = If(pUnapprovedCount > 0, 1, 0)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

        UpdateProfitabilityTransactionColumns(vID)

        If vRecompute Then
            For Each pDrApproved As DataRow In pDtApproved.Rows
                ApproveProfitability(CStr2(pDrApproved("ItemID")), CStr2(pDrApproved("ApprovedByID")), True)
            Next
        End If

        mProData.CommitTransaction()

    End Sub

    Public Sub ApproveProfitability(vInvoiceItemID As String, vUserID As String, vUseUnapprovedPrice As Boolean)

        Dim pSql = "SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",TransProfitability.SellingPrice" & vbCrLf &
            ",TransInvoiceItems.UnitPrice" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID " & vbCrLf &
            ",TransInvoiceItems.Cost" & vbCrLf &
            ",TransInvoiceItems.OrderedQuantity" & vbCrLf &
            ",TransProfitability.ApprovedSellingPrice" & vbCrLf &
            ",TransProfitability.COGS" & vbCrLf &
            ",TransProfitability.OrderQuantity" & vbCrLf &
            ",TransProfitability.InvoiceID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransProfitability ON TransProfitability.ItemID = TransInvoiceItems.ID" & vbCrLf &
            " WHERE TransInvoiceItems.ID = " & ReadyString(vInvoiceItemID)
        Dim pDrSource = mProData.ExecuteRow(pSql)
        If pDrSource Is Nothing Then
            Throw New Exception("vb72ENGTEXQ4! ApproveProfitability vInvoiceItemID does not exist " & vInvoiceItemID)
        End If
        Dim pIU = New InsertUpdate("TransProfitability")
        Dim pSellingPrice As Decimal
        If vUseUnapprovedPrice Then
            pSellingPrice = CDec2(pDrSource("SellingPrice"))
        Else
            pSellingPrice = CDec2(pDrSource("UnitPrice"))
        End If
        pIU("ApprovedSellingPrice") = pSellingPrice
        pIU("ApprovedGp") = CDec2(pSellingPrice) - CDec2(CDec2(pDrSource("COGS")))
        pIU("ApprovedGpm") = (pSellingPrice - CDec2(pDrSource("COGS"))) / pSellingPrice * 100
        pIU("ApprovedLineGp") = (CDec2(pSellingPrice) - CDec2(CDec2(pDrSource("COGS")))) * CDec2(pDrSource("OrderQuantity"))
        pIU("ApprovedByID") = ReadyString(vUserID)
        pIU("ApprovedDate") = "GETDATE()"

        Dim pDrPeople = mParent.LibraryPeople.GetByID(vUserID)
        If pDrPeople IsNot Nothing Then
            pIU("ApprovedByName") = ReadyString(Truncate(CStr2(pDrPeople("FullName")), 100))
        End If
        pSql = pIU.GetUpdate("ItemID = " & ReadyString(vInvoiceItemID))

        mProData.BeginTransaction()
        Dim pRecordCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM TransProfitability" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(pDrSource("InvoiceID")) & vbCrLf &
            " AND ApprovedByID IS NULL"
        pRecordCount = mProData.ExecuteScalar(pSql)
        If pRecordCount = 0 Then
            pIU = New InsertUpdate("TransInvoice")
            pIU("UnapprovedProfitability") = 0
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrSource("InvoiceID")))
            pRecordCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only
        End If
        UpdateProfitabilityTransactionColumns(CStr2(pDrSource("InvoiceID")))

        mProData.CommitTransaction()

    End Sub

    Public Sub UpdateProfitabilityTransactionColumns(vInvoiceID As String)

        Dim pUnapprovedSubtotal As Decimal
        Dim pApprovedSubtotal As Decimal
        Dim pCost As Decimal

        Dim pSql = "SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",ISNULL(TransProfitability.SellingPrice, TransInvoiceItems.UnitPrice) AS SellingPrice" & vbCrLf &
            ",TransInvoiceItems.PriceLevelID " & vbCrLf &
            ",TransInvoiceItems.Cost" & vbCrLf &
            ",TransInvoiceItems.OrderedQuantity" & vbCrLf &
            ",TransProfitability.ApprovedSellingPrice" & vbCrLf &
            ",TransProfitability.ApprovedByID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " LEFT JOIN TransProfitability ON TransProfitability.ItemID = TransInvoiceItems.ID" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID)
        Dim pDtItems = mProData.Execute(pSql)
        For Each pDrItem As DataRow In pDtItems.Rows
            pCost += CDec2(pDrItem("Cost")) * CDec2(pDrItem("OrderedQuantity"))
            pUnapprovedSubtotal += CDec2(pDrItem("SellingPrice")) * CDec2(pDrItem("OrderedQuantity"))
            If pDrItem("ApprovedByID") IsNot DBNull.Value Then
                pApprovedSubtotal += CDec2(pDrItem("ApprovedSellingPrice")) * CDec2(pDrItem("OrderedQuantity"))
            End If
        Next pDrItem

        Dim pIU = New InsertUpdate("TransProfitability")
        If pUnapprovedSubtotal = 0 Then
            pIU("UnapprovedTransTotalGp") = 0
            pIU("UnapprovedTransTotalGpm") = 0
        Else
            pIU("UnapprovedTransTotalGp") = pUnapprovedSubtotal - pCost
            pIU("UnapprovedTransTotalGpm") = (pUnapprovedSubtotal - pCost) / pUnapprovedSubtotal * 100
        End If
        If pApprovedSubtotal = 0 Then
            pIU("ApprovedTransTotalGp") = 0
            pIU("ApprovedTransTotalGpm") = 0
        Else
            pIU("ApprovedTransTotalGp") = pApprovedSubtotal - pCost
            pIU("ApprovedTransTotalGpm") = (pApprovedSubtotal - pCost) / pApprovedSubtotal * 100
        End If
        pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(vInvoiceID))
        Dim pRecordCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only

    End Sub

    Public Function UpdateProfitabilityItemIDs(vInvoiceID As String, vProductID As String) As DataTable

        mProData.BeginTransaction()
        ' the TransInvoiceItems.ProductID in SELECT is not needed
        Dim pSql = "SELECT TransInvoiceItems.ID" & vbCrLf &
            ",TransInvoiceItems.ProductID" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID)
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("TransProfitability")
            pIU("ItemID") = ReadyString(pDR("ID"))
            pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
                " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
                " AND UnitQty = " & CDec2(pDR("ActualQuantity")))
            Dim pRecordCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next
        mProData.CommitTransaction()

        Return pDT

    End Function

    Public Function GetGrossProfitabilityValidationCount() As Integer

        Dim pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM TransProfitability" & vbCrLf &
            " WHERE ApprovedByID IS NULL"
        Dim pResult = CInt2(mProData.ExecuteScalar(pSql))

        Return pResult

    End Function

    Public Function LatestPurchaseOrderByVendorProduct(vVendorID As String, vProductID As String) As DataRow

        Dim pSql = " SELECT TOP 1 TransInvoiceItems.UnitPrice" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ,TransInvoice.PurchaseOrderDate" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.PurchaseOrderNo) AS PurchaseOrderNumber" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " WHERE TransInvoice.PurchaseOrderNo > 0" & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND TransInvoice.CustomerID = " & ReadyString(vVendorID) & vbCrLf &
            " ORDER BY TransInvoice.PurchaseOrderDate DESC" & vbCrLf &
            " ,TransInvoice.LastUpdated DESC" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity"
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function TopSellingAndTopProfitableSkus(vDateFrom As Date, vDateTo As Date, vGroup As String,
        vFilterField As String, vFilterValue As String) As DataSet

        Dim pDS = New DataSet

        Dim pSql = " -- top selling and top profitable skus" & vbCrLf &
            " SELECT TOP 100 PERCENT " & vbCrLf

        'If vGroup = "product" Then
        pSql &= "MobizLibraryPIMProduct.ProductNo" & vbCrLf &
                " ,TransInvoiceItems.ProductID" & vbCrLf &
                " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
                " ,MobizLibraryPIMProduct.Brand" & vbCrLf
        'ElseIf vGroup = "product category" Then
        pSql &= ",tblCategory.CategoryName" & vbCrLf
        'ElseIf vGroup = "sales channel" Then
        pSql &= ",LibraryOrganizationCustomer.SalesChannelID" & vbCrLf &
            ",MobizLibrarySalesChannels.Name AS SalesChannelName" & vbCrLf
        'ElseIf vGroup = "customer industry" Then
        pSql &= ",MobizLibraryOrganization.UserDefinedIndustryID" & vbCrLf &
                ", MobizLibraryUserDefinedIndustry.Name AS UserDefinedIndustryName" & vbCrLf
        'ElseIf vGroup = "customer" Then
        pSql &= ",TransInvoice.CustomerID" & vbCrLf &
                ",MobizLibraryOrganization.CompanyName" & vbCrLf
        'End If

        pSql &= " ,AVG(TransInvoiceItems.UnitPrice / TransInvoiceItems.ActualQuantity) AS UnitPrice" & vbCrLf &
            " ,AVG(TransInvoiceItems.Cost) AS AvgCogs" & vbCrLf &
            " ,AVG((TransInvoiceItems.UnitPrice - TransInvoiceItems.Cost)) AS AvgGP" & vbCrLf &
            " ,AVG((TransInvoiceItems.UnitPrice - TransInvoiceItems.Cost) / TransInvoiceItems.UnitPrice / TransInvoiceItems.ActualQuantity) * 100 AS GPM" & vbCrLf &
            " ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS UnitsSold" & vbCrLf &
            " ,SUM(TransInvoiceItems.SubTotal) AS SubTotal" & vbCrLf &
            " ,SUM(TransInvoiceItems.Cost * TransInvoiceItems.Quantity) AS Cogs" & vbCrLf &
            " ,SUM((TransInvoiceItems.UnitPrice - TransInvoiceItems.Cost)) AS GP" & vbCrLf
        pSql &= " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf

        'If vGroup = "sales channel" Then
        pSql &= " JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
                "  LEFT JOIN MobizLibrarySalesChannels ON MobizLibrarySalesChannels.ID = LibraryOrganizationCustomer.SalesChannelID" & vbCrLf
        'ElseIf vGroup = "product category" Then

        pSql &= "  OUTER APPLY (" & vbCrLf &
                "  	SELECT TOP 1 C0020 AS CategoryName" & vbCrLf &
                "     FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                "     WHERE ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "  ) tblCategory" & vbCrLf

        'ElseIf vGroup = "customer industry" Then
        pSql &= " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
                " LEFT JOIN MobizLibraryUserDefinedIndustry ON MobizLibraryUserDefinedIndustry.ID = MobizLibraryOrganization.UserDefinedIndustryID" & vbCrLf
        'ElseIf vGroup = "customer" Then
        'pSql &= " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf
        'End If

        pSql &= " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " And TransInvoiceItems.ActualQuantity <> 0" & vbCrLf &
            " And TransInvoiceItems.UnitPrice <> 0" & vbCrLf &
            " And CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Format(vDateFrom.Date, cGlobal.FormatDateTimeSql)) & " And " & ReadyString(Format(vDateTo.Date, cGlobal.FormatDateTimeSql)) & vbCrLf

        pSql &= " GROUP BY "

        'If vGroup = "product" Then
        pSql &= "MobizLibraryPIMProduct.ID" & vbCrLf &
                ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
                ",MobizLibraryPIMProduct.ProductName" & vbCrLf &
                ",MobizLibraryPIMProduct.Brand" & vbCrLf &
                ",TransInvoiceItems.ProductID" & vbCrLf
        'ElseIf vGroup = "product category" Then
        pSql &= ",tblCategory.CategoryName" & vbCrLf
        'ElseIf vGroup = "sales channel" Then
        pSql &= ",LibraryOrganizationCustomer.SalesChannelID" & vbCrLf &
            ",MobizLibrarySalesChannels.Name" & vbCrLf
        'ElseIf vGroup = "customer industry" Then
        pSql &= ",MobizLibraryOrganization.UserDefinedIndustryID" & vbCrLf &
                ", MobizLibraryUserDefinedIndustry.Name" & vbCrLf
        'ElseIf vGroup = "customer" Then
        pSql &= ",TransInvoice.CustomerID" & vbCrLf &
                ",MobizLibraryOrganization.CompanyName" & vbCrLf
        'End If

        pSql &= " ORDER BY SUM(TransInvoiceItems.SubTotal) DESC"

        pSql = "WITH cte AS (" & vbCrLf &
            pSql & vbCrLf &
            ")" & vbCrLf
        pSql &= "SELECT " & vbCrLf &
            " AVG(UnitPrice) AS UnitPrice" & vbCrLf &
            ",AVG(AvgCogs) AS AvgCogs" & vbCrLf &
            ",AVG(AvgGP) AS AvgGP" & vbCrLf &
            ",AVG(GPM) AS GPM" & vbCrLf &
            ",SUM(UnitsSold) AS UnitsSold" & vbCrLf &
            ",SUM(SubTotal) AS SubTotal" & vbCrLf &
            ",SUM(Cogs) AS Cogs" & vbCrLf &
            ",SUM(GP) AS GP" & vbCrLf
        If vGroup = "product" Then
            pSql &= ",ProductID" & vbCrLf &
                ",ProductNo" & vbCrLf &
                ",ProductName" & vbCrLf &
                ",Brand" & vbCrLf
        ElseIf vGroup = "brand" Then
            pSql &= ",Brand" & vbCrLf
        ElseIf vGroup = "product category" Then
            pSql &= ",CategoryName" & vbCrLf
        ElseIf vGroup = "sales channel" Then
            pSql &= ",SalesChannelID" & vbCrLf &
            ",SalesChannelName" & vbCrLf
        ElseIf vGroup = "customer industry" Then
            pSql &= ",UserDefinedIndustryID" & vbCrLf &
                ",UserDefinedIndustryName" & vbCrLf
        ElseIf vGroup = "customer" Then
            pSql &= ",CustomerID" & vbCrLf &
                ",CompanyName" & vbCrLf
        End If

        pSql &= " FROM CTE" & vbCrLf &
            " WHERE 1=1" & vbCrLf
        If vFilterField = "product" Then
            pSql &= " AND ProductID = " & ReadyString(vFilterValue) & vbCrLf
        ElseIf vFilterField = "brand" Then
            pSql &= " AND ISNULL(Brand, '') = " & ReadyString(vFilterValue) & vbCrLf
        ElseIf vFilterField = "product category" Then
            pSql &= " AND ISNULL(CategoryName, '') = " & ReadyString(vFilterValue) & vbCrLf
        ElseIf vFilterField = "sales channel" Then
            If String.IsNullOrWhiteSpace(vFilterValue) Then
                pSql &= " AND SalesChannelID IS NULL " & vbCrLf
            Else
                pSql &= " AND SalesChannelID = " & ReadyString(vFilterValue) & vbCrLf
            End If
        ElseIf vFilterField = "customer industry" Then
            If String.IsNullOrWhiteSpace(vFilterValue) Then
                pSql &= " AND UserDefinedIndustryID IS NULL" & vbCrLf
            Else
                pSql &= " AND UserDefinedIndustryID = " & ReadyString(vFilterValue) & vbCrLf
            End If
        ElseIf vFilterField = "customer" Then
            pSql &= " AND CustomerID = " & ReadyString(vFilterValue) & vbCrLf
        End If

        pSql &= " GROUP BY " & vbCrLf

        If vGroup = "product" Then
            pSql &= "ProductID" & vbCrLf &
                ",ProductNo" & vbCrLf &
                ",ProductName" & vbCrLf &
                ",Brand" & vbCrLf
        ElseIf vGroup = "brand" Then
            pSql &= "Brand" & vbCrLf
        ElseIf vGroup = "product category" Then
            pSql &= "CategoryName" & vbCrLf
        ElseIf vGroup = "sales channel" Then
            pSql &= "SalesChannelID" & vbCrLf &
            ",SalesChannelName" & vbCrLf
        ElseIf vGroup = "customer industry" Then
            pSql &= "UserDefinedIndustryID" & vbCrLf &
                ",UserDefinedIndustryName" & vbCrLf
        ElseIf vGroup = "customer" Then
            pSql &= "CustomerID" & vbCrLf &
                ",CompanyName" & vbCrLf
        End If

        Dim pDtData = mProData.Execute(pSql)
        pDtData.Columns.Add("ContributionToSales", GetType(Decimal))
        pDtData.Columns.Add("ContributionToCogs", GetType(Decimal))
        pDtData.Columns.Add("ContributionToGp", GetType(Decimal))

        Dim pTotalSubTotal = CDec2(pDtData.Compute("SUM(SubTotal)", ""))
        Dim pTotalCogs = CDec2(pDtData.Compute("SUM(Cogs)", ""))
        Dim pTotalGP = CDec2(pDtData.Compute("SUM(GP)", ""))

        For Each pDrData As DataRow In pDtData.Rows
            If pTotalSubTotal <> 0 Then
                pDrData("ContributionToSales") = pDrData("SubTotal") / pTotalSubTotal * 100
            End If
            If pTotalCogs <> 0 Then
                pDrData("ContributionToCogs") = pDrData("Cogs") / pTotalCogs * 100
            End If
            If pTotalGP <> 0 Then
                pDrData("ContributionToGp") = pDrData("GP") / pTotalGP * 100
            End If
        Next pDrData

        pDtData.TableName = "Data"
        pDS.Tables.Add(pDtData)

        Dim pDtFooter = pDtData.Clone
        Dim pDrFooter = pDtFooter.NewRow()
        pDrFooter("SubTotal") = pTotalSubTotal
        pDrFooter("Cogs") = pTotalCogs
        pDrFooter("GP") = pTotalGP
        pDrFooter("ContributionToSales") = pDtData.Compute("SUM(ContributionToSales)", "")
        pDrFooter("ContributionToCogs") = pDtData.Compute("SUM(ContributionToCogs)", "")
        pDrFooter("ContributionToGp") = pDtData.Compute("SUM(ContributionToGp)", "")
        pDtFooter.Rows.Add(pDrFooter)
        pDtFooter.TableName = "Footer"
        pDS.Tables.Add(pDtFooter)

        Return pDS

    End Function

    Public Function LastAccountsPayable(vVendorID As String, vProductID As String, Optional vExcludeID As String = Nothing) As DataRow

        Dim pSql = "SELECT TOP 1 TransInvoice.LastUpdated" & vbCrLf &
            ",TransInvoice.ID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoice.CustomerID = " & ReadyString(vVendorID) & vbCrLf &
            " AND AccountsPayableNum > 0" & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf
        If vExcludeID IsNot Nothing Then
            pSql &= " AND TransInvoice.ID <> " & ReadyString(vExcludeID) & vbCrLf
        End If
        pSql &= " ORDER BY TransInvoice.LastUpdated DESC"
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return Nothing
        End If

        pSql = "SELECT TransInvoice.LastUpdated" & vbCrLf &
            ",TransInvoiceItems.UnitPrice" & vbCrLf &
            ",TransInvoiceItems.ActualQuantity" & vbCrLf &
            ",TransInvoiceItems.Quantity" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(pDR("ID")) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " ORDER BY TransInvoiceItems.ActualQuantity"
        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Enum eSetUnitPriceSource
        LastPurchaseOrderPrice
        LastAccountsPayablePrice
        ApOrPoWhicheverIsLower
        ApOrPoWhicheverIsNewer
        VendorPrice
    End Enum

    Public Function SetLineItemPrice(vInvoiceID As String, vVendorID As String, vSource As eSetUnitPriceSource) As Boolean

        Dim pSql = "SELECT ID" & vbCrLf &
            ",ProductID" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        Dim pDT = mProData.Execute(pSql)
        mProData.BeginTransaction()
        For Each pDR As DataRow In pDT.Rows
            SetLineItemPrice(vInvoiceID, vVendorID, CStr2(pDR("ProductID")), CStr2(pDR("ID")), vSource)
        Next
        mProData.CommitTransaction()

    End Function

    Public Function SetLineItemPrice(vInvoiceID As String, vVendorID As String, vProductID As String, vItemID As String, vSource As eSetUnitPriceSource) As Boolean

        Dim pDrSource As DataRow
        Dim pDrSourcePo As DataRow
        Dim pDrSourceAp As DataRow
        Dim pQtyField = "Quantity"

        Select Case vSource
            Case eSetUnitPriceSource.LastPurchaseOrderPrice
                pDrSource = LatestPurchaseOrderByVendorProduct(vVendorID, vProductID)

            Case eSetUnitPriceSource.LastAccountsPayablePrice
                pDrSource = LastAccountsPayable(vVendorID, vProductID, vInvoiceID)

            Case eSetUnitPriceSource.ApOrPoWhicheverIsLower
                pDrSource = LastApOrPoPriceWhicheverIsLower(vVendorID, vProductID)

            Case eSetUnitPriceSource.ApOrPoWhicheverIsNewer
                pDrSourcePo = LatestPurchaseOrderByVendorProduct(vVendorID, vProductID)
                pDrSourceAp = LastAccountsPayable(vVendorID, vProductID, vInvoiceID)

                If pDrSourcePo IsNot Nothing _
                    AndAlso pDrSourceAp IsNot Nothing Then

                    If pDrSourceAp("LastUpdated") < pDrSourcePo("LastUpdated") Then
                        pDrSource = pDrSourcePo
                    Else
                        pDrSource = pDrSourceAp
                    End If

                ElseIf pDrSourcePo IsNot Nothing Then
                    pDrSource = pDrSourcePo

                ElseIf pDrSourceAp IsNot Nothing Then
                    pDrSource = pDrSourceAp

                End If

            Case eSetUnitPriceSource.VendorPrice
                Dim pDtVendorPrice = mParent.LibraryStock.VendorPriceProducts(vVendorID, vProductID)
                pDtVendorPrice.Columns("Price").ColumnName = "UnitPrice"
                pDtVendorPrice.Columns.Add("ActualQuantity", GetType(Decimal))
                If pDtVendorPrice.Rows.Count > 0 Then
                    pDrSource = pDtVendorPrice.Rows(0)
                Else
                    pDrSource = pDtVendorPrice.NewRow
                    pDrSource("UnitPrice") = 0
                End If
                pDrSource("ActualQuantity") = 1
                Dim pDrInvoice = ById(vInvoiceID)
                If pDrInvoice IsNot Nothing Then
                    If CStr2(pDrInvoice("TransType")) = "PurchaseOrder" Then
                        pQtyField = "OrderedQuantity"
                    End If
                End If
        End Select

        If pDrSource Is Nothing Then
            Return False
        End If

        Dim pIU As New InsertUpdate("TransInvoiceItems")
        pIU("UnitPrice") = pDrSource("UnitPrice") / pDrSource("ActualQuantity") & " * ActualQuantity"
        pIU("SubTotal") = pDrSource("UnitPrice") / pDrSource("ActualQuantity") & " * ActualQuantity * " & pQtyField

        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vItemID))
        mProData.BeginTransaction()
        Dim pAffectedCount = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        pSql = "SELECT SUM(SubTotal)" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID)
        pSql = "UPDATE TransInvoice" & vbCrLf &
            " SET TotalAmount = (" & vbCrLf &
            pSql & ")" & vbCrLf &
            " WHERE ID = " & ReadyString(vInvoiceID)
        pAffectedCount = mProData.ExecuteSQL(pSql)
        mProData.CommitTransaction()

        Return pAffectedCount = 1

    End Function

    Public Function LastApOrPoPriceWhicheverIsLower(vVendorID As String, vProductID As String) As DataRow

        Dim pDrSource As DataRow
        Dim pDrSourcePo As DataRow
        Dim pDrSourceAp As DataRow

        pDrSourcePo = LatestPurchaseOrderByVendorProduct(vVendorID, vProductID)
        pDrSourceAp = LastAccountsPayable(vVendorID, vProductID)

        If pDrSourcePo IsNot Nothing _
            AndAlso pDrSourceAp IsNot Nothing Then

            If pDrSourcePo("UnitPrice") / pDrSourcePo("ActualQuantity") < pDrSourceAp("UnitPrice") / pDrSourceAp("ActualQuantity") Then
                pDrSource = pDrSourcePo
            Else
                pDrSource = pDrSourceAp
            End If

        ElseIf pDrSourcePo IsNot Nothing Then
            pDrSource = pDrSourcePo

        ElseIf pDrSourceAp IsNot Nothing Then
            pDrSource = pDrSourceAp

        End If

        Return pDrSource

    End Function

    Public Function GetAutoPrintableInvoice() As DataTable

        Dim pSql = "SELECT ID, LastUpdated" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FinalizedForAutoInvoice = 1" & vbCrLf &
            " AND ISNULL(PrintCount, 0) = 0" & vbCrLf &
            " AND TransType = 'Invoice'" & vbCrLf &
            " AND InvoiceNumber > 0" & vbCrLf &
            " ORDER BY LastUpdated"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetByBackorderSourceInvoiceID(vInvoiceID As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE BackorderSourceInvoiceID = " & ReadyString(vInvoiceID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function FixAccountsPayableBillingAndDeliveryAddress(vID As String) As Boolean

        Dim pDR = ById(vID)
        If pDR Is Nothing Then
            Return False
        End If
        If CStr2(pDR("TransType")).Trim().ToLower() = "AccountsPayable".ToLower() Then
            Dim pIU = New InsertUpdate("TransInvoice")
            Dim pAddressID As String
            If ReadyGuid(pDR("DeliveryAddressID")) = ReadyGuid(Guid.Empty) Then ' 
                pAddressID = MostUsedAccountsPayableAddress("DeliveryAddressID")
                If String.IsNullOrWhiteSpace(pAddressID) = False Then
                    pIU("DeliveryAddressID") = ReadyString(pAddressID)
                End If
            End If

            If ReadyGuid(pDR("BillingAddressID")) = ReadyGuid(Guid.Empty) Then ' BillingAddressID
                pAddressID = MostUsedAccountsPayableAddress("BillingAddressID")
                If String.IsNullOrWhiteSpace(pAddressID) = False Then
                    pIU("BillingAddressID") = ReadyString(pAddressID)
                End If
            End If

            If pIU.Count > 0 Then
                Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
                mProData.ExecuteSQL(pSql)
                Return True
            End If
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vFieldName">BillingAddressID or DeliveryAddressID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MostUsedAccountsPayableAddress(vFieldName As String) As String

        Dim pSql = " SELECT TransInvoice." & vFieldName & vbCrLf &
            " ,COUNT(*)" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE TransType = 'AccountsPayable'" & vbCrLf &
            " GROUP BY TransInvoice." & vFieldName & vbCrLf &
            " ORDER BY COUNT(*) DESC"
        Dim pDT = mProData.Execute(pSql)

        For Each pDR As DataRow In pDT.Rows
            If CStr2(pDR(vFieldName)) = "" Then
                Continue For
            End If
            If ReadyGuid(pDR(vFieldName)) <> ReadyGuid(Guid.Empty) Then
                Return ReadyGuid(pDR(vFieldName))
            End If
        Next

    End Function

    ' commented 2021_08_31__14_55_32_6874
    'Public Function GetItemRequisition(vFowstID As String, vDateRequested As Date, vDeactivated As Boolean) As DataTable

    '    mProData.AddColumn("TransItemRequisitions", "FowstID", "uniqueidentifier")
    '    Dim pSql = " -- GetItemRequisition" & vbCrLf &
    '        " SELECT " & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," &  & vbCrLf &
    '        " ," & vbCrLf &
    '        " ," & & vbCrLf &
    '        " FROM TransItemRequisitions" & vbCrLf &
    '        " JOIN MobizLibraryPimProduct ON MobizLibraryPimProduct.ID = TransItemRequisitions.ProductID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransItemRequisitions.VendorID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryPeople tblRequester ON tblRequester.ID = TransItemRequisitions.RequestedByID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblRequesterPeoplePreNominalSuffix ON tblRequester.TitleID = tblRequesterPeoplePreNominalSuffix.ID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryPeople tblEncoder ON tblEncoder.ID = TransItemRequisitions.UserID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblEncoderPeoplePreNominalSuffix ON tblEncoder.TitleID = tblEncoderPeoplePreNominalSuffix.ID" & vbCrLf &
    '        " WHERE 1=1" & vbCrLf
    '    pSql &= " AND ISNULL(TransItemRequisitions.IsDeactivated, 0) = " & If(vDeactivated, 1, 0) & vbCrLf
    '    pSql &= " AND (TransItemRequisitions.FowstID = " & ReadyString(vFowstID) & vbCrLf &
    '        " OR TransItemRequisitions.FowstID IS NULL)" & vbCrLf
    '    pSql &= " AND CONVERT(date, TransItemRequisitions.DateRequested) = " & ReadyString(Format(vDateRequested, cGlobal.FormatDateTimeSql)) & vbCrLf
    '    pSql &= " ORDER BY TransItemRequisitions.DateRequested"
    '    Dim pDT = mProData.Execute(pSql)

    '    Return pDT

    'End Function

    Public Function EmailReceiptHtml(vInvoiceID As String, Optional vSendEmail As Boolean = False,
        Optional vEmailAddresses As String = Nothing) As String

        Dim strmReader As New System.IO.StreamReader(Common.ServerPath & "/Views/SalesPayments/EmailSalesReceipt.vbhtml")
        Dim strBody = strmReader.ReadToEnd
        Dim strInvoiceNo As String
        Dim varBusinessName As String
        Dim strTemp As String
        Dim intCntr As Integer
        Dim mDeviceID As String

        Dim pDrInvoice = ById(vInvoiceID)
        If pDrInvoice IsNot Nothing Then
            mDeviceID = CStr2(pDrInvoice("DeviceID"))
        End If

        '''Get Payment Info
        Dim dtaTable = New DataTable()
        Dim clsSalesPayments As New SalesPayments
        dtaTable = clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(vInvoiceID, mProData)
        If (dtaTable.Rows.Count > 0) Then

            strBody = strBody.Replace("[CustomerNameHere]", "Customer : " & dtaTable.Rows(0).Item("CustomerName").ToString().Trim())
            strBody = strBody.Replace("[CustomerAddressHere]", "Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
            strBody = strBody.Replace("[CustomerTINHere]", "TIN : " & dtaTable.Rows(0).Item("CustomerTIN").ToString().Trim())

            strBody = strBody.Replace("[CustomerBillingAddress]", "Billing Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
            strBody = strBody.Replace("[CustomerDeliveryAddress]", "Delivery Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())

            strInvoiceNo = Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim()

            strBody = strBody.Replace("[InvoiceNoHere]", "Sales Invoice No. " & Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim())
            strBody = strBody.Replace("[InvoiceDateTimeHere]", "Date/Time : " & Convert.ToDateTime(dtaTable.Rows(0).Item("InvoiceDate")).ToString("MMM dd, yyyy HH:mm").Trim())
            strBody = strBody.Replace("[CashierNameHere]", "Cashier : " & dtaTable.Rows(0).Item("CashierName").ToString().Trim())

        End If

        '''Get Header Info
        Dim FowstID As String = CStr2(dtaTable.Rows(0).Item("OrganizationBranchID"))
        Dim POSAccredNo = mParent.SystemDefaults.GetSystemDefaultPublic("Mobiz POS Accreditation Number")

        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getReceiptHeaderInfo(FowstID.ToString().Trim(), mProData, POSAccredNo, mDeviceID)

        If (dtaTable.Rows.Count > 0) Then
            'additional by janine. to show in email receipt
            If (dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim() <> "") Then
                varBusinessName = dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim()
            Else
                varBusinessName = dtaTable.Rows(0).Item("CorporateName").ToString().Trim()
            End If

            strBody = strBody.Replace("[TradeNameHere]", dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim())
            strBody = strBody.Replace("[CorporateNameHere]", dtaTable.Rows(0).Item("CorporateName").ToString().Trim())
            strBody = strBody.Replace("[TINHere]", "TIN " & dtaTable.Rows(0).Item("TIN").ToString().Trim())
            strBody = strBody.Replace("[AddressHere]", dtaTable.Rows(0).Item("CompleteAddress").ToString().Trim())
            strBody = strBody.Replace("[TelNoHere]", "Tel. No. " & dtaTable.Rows(0).Item("TelNo").ToString().Trim())

            strBody = strBody.Replace("[POSAccreditationNo]", "POS Accreditation No. " & dtaTable.Rows(0).Item("POSAccreditationNo").ToString().Trim())
            strBody = strBody.Replace("[POSRegistrationNo]", "POS Registration No. " & dtaTable.Rows(0).Item("POSPermitNo").ToString().Trim())
            strBody = strBody.Replace("[POSMachineNo]", "Machine ID No. (MIN) " & dtaTable.Rows(0).Item("MIN").ToString().Trim())

        End If

        '''Get Payment Items
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptBodyInfo(vInvoiceID, mProData)
        If (dtaTable.Rows.Count > 0) Then

            strTemp = "<tbody>"
            intCntr = 0
            While (intCntr < dtaTable.Rows.Count)

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 35%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("StockDescription").ToString().Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 10%; height:30px; border: 1px dotted Gray;"">" & Convert.ToInt32(dtaTable.Rows(intCntr).Item("Quantity")).ToString("#,##0").Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 15%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("UnitMeasure").ToString().Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("UnitPrice")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                intCntr = intCntr + 1

            End While

            strTemp = strTemp & "</tbody>"

            strBody = strBody.Replace("[BodyGoesHere]", strTemp)

        End If

        '''Get Payment Amount
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptFooterAmountsInfo(vInvoiceID, mProData)
        If (dtaTable.Rows.Count > 0) Then

            '''SKU and Amount due
            strTemp = "<table style=""width:100%"">"
            strTemp = strTemp & "<thead>"
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">SKU Count</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToInt32(dtaTable.Rows(0).Item("SKUCount")).ToString("#,##0").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Sub Total</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            ' additional by janine feb 11 2015 
            If (IsSeniorCitizenDiscount(vInvoiceID)) Then
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Less: " & dtaTable.Rows(0).Item("TaxPercent") & "% VAT</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("SubTotal")) - CDec2(dtaTable.Rows(0).Item("TotalAmountDue"))).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Total Amount</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TotalAmountDue")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

            End If


            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Discount Amount</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("DiscountAmount")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            ' additional by janine feb 11 2015 
            If (mParent.SystemDefaults.IsEnableBillingOption = "1") Then
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Charges</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("ChargeAmount")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Tip</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TipAmount")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

            End If

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Net Amount Due</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("NetAmountDue")) + CDec2(dtaTable.Rows(0).Item("ChargeAmount")) + CDec2(dtaTable.Rows(0).Item("TipAmount"))).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "</thead>"
            strTemp = strTemp & "</table>"

            strTemp = strTemp & "<br />"

            '''VAT Info
            strTemp = strTemp & "<table style=""width:100%"">"
            strTemp = strTemp & "<thead>"
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT-able Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxSales")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Exempt Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Zero-Rated Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Amount</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxAmount")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "</thead>"
            strTemp = strTemp & "</table>"

            strBody = strBody.Replace("[FooterGoesHere]", strTemp)

        End If
        Dim pCustomerEmailAddresses As String

        '''Get Payment Details
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptPaymentInfo(vInvoiceID, mProData)
        If (dtaTable.Rows.Count > 0) Then
            pCustomerEmailAddresses = CStr2(dtaTable.Rows(0)("CustomerEmailAddresses"))
            strBody = strBody.Replace("[CASHPAYMENT]", IIf(CDec2(dtaTable.Rows(0).Item("CashPayment")) <= 0, "", "Cash        : " & CDec2(dtaTable.Rows(0).Item("CashPayment")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CHECKPAYMENT]", IIf(CDec2(dtaTable.Rows(0).Item("CheckPaymentAmount")) <= 0, "", "Check       : " & CDec2(dtaTable.Rows(0).Item("CheckPaymentAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CREDITCARDPAYMENT]", IIf(CDec2(dtaTable.Rows(0).Item("CreditCardAmount")) <= 0, "", "Credit Card : " & CDec2(dtaTable.Rows(0).Item("CreditCardAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[DEBITCARDPAYMENT]", IIf(CDec2(dtaTable.Rows(0).Item("DebitCardAmount")) <= 0, "", "Debit Card : " & CDec2(dtaTable.Rows(0).Item("DebitCardAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[SRVPAYMENT]", IIf(CDec2(dtaTable.Rows(0).Item("SRVAmount")) <= 0, "", "SRV         : " & CDec2(dtaTable.Rows(0).Item("SRVAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CHANGEAMOUNT]", IIf(CDec2(dtaTable.Rows(0).Item("CustomerChangeCash")) <= 0, "", "Change      : " & CDec2(dtaTable.Rows(0).Item("CustomerChangeCash")).ToString("#,##0.00").Trim()))

        End If

        If String.IsNullOrWhiteSpace(vEmailAddresses) = False Then
            pCustomerEmailAddresses = vEmailAddresses
        End If

        If vSendEmail _
            AndAlso String.IsNullOrWhiteSpace(pCustomerEmailAddresses) = False Then

            SendMail("customercare@email.mobiz-apps.com",
                pCustomerEmailAddresses,
                "Mobiz",
                varBusinessName & " - Inv. No. " & strInvoiceNo,
                strBody,
                "smtp.pepipost.com",
                587,
                "arvinboggs",
                "p$P.p/Kus7v%3<A@~dN^", , , , , True)
            Return "ok"
        End If

        Return strBody

    End Function

    Function IsSeniorCitizenDiscount(ByVal vInvoiceID As String) As Boolean
        Dim vDiscountType As String

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DiscountTypeID FROM TransInvoiceDiscount WHERE InvoiceID = " & ReadyString(vInvoiceID)

        pDT = mProData.Execute(pSql)

        If (pDT.Rows.Count > 0) Then
            vDiscountType = pDT.Rows(0).Item("DiscountTypeID").ToString()

            If (vDiscountType.ToUpper() = "055A0A33-DD98-47EC-ADD3-8B2009F3944A" Or vDiscountType.ToUpper() = "674AD791-188B-4238-B95C-97B5C2B9D544") Then
                Return True
            Else
                Return False
            End If


        Else
            Return False
        End If


    End Function

    Public Function UpdateInvoiceDate(vID As String, Optional vInvoiceDate As Date? = Nothing) As Integer

        Dim pIU = New InsertUpdate("TransInvoice")
        If vInvoiceDate.HasValue Then
            pIU("InvoiceDate") = ReadyString(Format(vInvoiceDate, cGlobal.FormatDateTimeSql))
        Else
            pIU("InvoiceDate") = "GETDATE()"
        End If
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        Dim pAffectedCount = mProData.ExecuteSQL(pSql)

        Return pAffectedCount

    End Function

    Public Function FinalizeWebPos(vID As String, vOrgID As String, ByRef oUpdateFileAction As Action) As Boolean

        Dim pNum As Integer
        Dim pFowstWideInvoiceNumber As Integer?
        Dim pTransType As SaveSalesOrderJson.eTransType

        Dim pDR = ById(vID)
        Select Case CStr2(pDR("TransType")).ToLower()
            Case "SalesOrder".ToLower()
                If CInt2(pDR("SalesOrderNo")) > 0 Then
                    ' invoice was previously finalized
                    Return False
                End If
                pTransType = SaveSalesOrderJson.eTransType.SalesOrder
            Case "Invoice".ToLower(), ""
                If CInt2(pDR("InvoiceNumber")) > 0 Then
                    ' invoice was previously finalized
                    Return False
                End If
                pTransType = SaveSalesOrderJson.eTransType.Invoice
            Case Else
                Return True
        End Select

        If CStr2(pDR("DeviceID")).Trim() = "" Then
            Throw New ApplicationException("DeviceID must not be blank. Please re-login using Mobiz Controller and try saving again. ref: vbEKJQ36ARKK!")
        End If
        Dim pDrDevice = mParent.LibraryDevice.ById(CStr2(pDR("DeviceID")))
        If pDrDevice Is Nothing Then
            Throw New ApplicationException("Specified DeviceID does not exist in LibraryDevice. Please re-login using Mobiz Controller and try saving again. ref: vbKVNF3PM32A!")
        End If
        If CStr2(pDrDevice("UUID")).Trim() = "" Then
            Throw New ApplicationException("UUID must not be blank. Please re-login using Mobiz Controller and try saving again. ref: vbQNYM3U6SCP!")
        End If

        UpdateAmounts(vID)

        If pTransType = SaveSalesOrderJson.eTransType.SalesOrder Then
            VerifyCounter(CStr2(pDR("DeviceID")), CStr2(pDR("FowstID")), pNum, CStr2(pDR("DocumentNameID")), pFowstWideInvoiceNumber, "POSLastSONo", "SalesOrderNo")
        ElseIf pTransType = SaveSalesOrderJson.eTransType.Invoice Then
            VerifyCounter(CStr2(pDR("DeviceID")), CStr2(pDR("FowstID")), pNum, CStr2(pDR("DocumentNameID")), pFowstWideInvoiceNumber)
        End If

        Dim pIU = New InsertUpdate("TransInvoice")
        If pTransType = SaveSalesOrderJson.eTransType.SalesOrder Then
            pIU("SalesOrderNo") = pNum + 1
        ElseIf pTransType = SaveSalesOrderJson.eTransType.Invoice Then
            pIU("InvoiceNumber") = pNum + 1
            If pFowstWideInvoiceNumber IsNot Nothing Then
                pIU("FowstWideInvoiceNumber") = pFowstWideInvoiceNumber + 1
            End If
        End If

        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.BeginTransaction()
        pSql = mProData.ExecuteSQL(pSql) ' output is for debugging purposes only

        If pTransType = SaveSalesOrderJson.eTransType.SalesOrder Then
            mParent.LibraryDeviceFowst.UpdateLastNumber("POSLastSONo", CStr2(pDR("FowstID")), CStr2(pDR("DeviceID")), pNum + 1, oUpdateFileAction)
        ElseIf pTransType = SaveSalesOrderJson.eTransType.Invoice Then
            ' UpdateCounter(CStr2(pDR("DeviceID")), CStr2(pDR("FowstID")), pNum) ' dont update counter here
            AddToInventoryLedger(vID, vOrgID)
            pSql = CacheInvoicesWithCredit(vID) ' output is for debugging purposes only
            mParent.LibraryDeviceFowst.UpdateLastNumber("POSLastORNo", CStr2(pDR("FowstID")), CStr2(pDR("DeviceID")), pNum + 1, oUpdateFileAction)
        End If
        mProData.CommitTransaction()

        Return True

    End Function

    Private Function GetInvoiceNumberFromXml(vOrgID As String, vFowstID As String, vDocumentID As String) As Integer

    End Function

    Public Function ItemRequisition(vProductID As String, vUnitQty As Decimal, vFowstID As String, vMinRequesitionDate As Date) As DataRow

        Dim pSql = "SELECT TOP 1 TransItemRequisitions.ID" & vbCrLf &
            ",TransItemRequisitions.ProductID" & vbCrLf &
            ",RequestedQty" & vbCrLf &
            ",BackorderQty" & vbCrLf &
            ",RequestedQtyTotal" & vbCrLf &
            " FROM TransItemRequisitions" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND UnitQty = " & vUnitQty & vbCrLf &
            " AND CONVERT(date, DateRequested) >= " & ReadyString(Format(vMinRequesitionDate.Date, cGlobal.FormatDateTimeSql)) & vbCrLf &
            " AND ISNULL(IsDeactivated, 0) = 0" & vbCrLf &
            " AND CreatedPurchaseOrderID IS NULL" & vbCrLf &
            " ORDER BY DateRequested"
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ItemRequisition(vItemRequisitionID As String) As DataRow

        Dim pSql = "SELECT TransItemRequisitions.ID" & vbCrLf &
            ",TransItemRequisitions.ProductID" & vbCrLf &
            ",RequestedQty" & vbCrLf &
            ",RequestedQtyTotal" & vbCrLf &
            " FROM TransItemRequisitions" & vbCrLf &
            " WHERE TransItemRequisitions.ID = " & ReadyString(vItemRequisitionID) & vbCrLf &
            ""
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ItemRequisitionDetail(vItemRequisitionID As String, vProductID As String) As DataSet

        Dim pDS As New DataSet

        Dim pSql = " -- Item Requisition Backorders" & vbCrLf &
            " SELECT TransInvoice.CustomerID" & vbCrLf &
            " ,TransInvoice.SalesOrderNo" & vbCrLf &
            " ,TransItemRequisitionBackOrder.ID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,ISNULL(TransItemRequisitionBackOrder.IncludeRow, 0) AS IncludeRow" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS MIN" & vbCrLf &
            " ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS Qty" & vbCrLf &
            ", " & BrowserController.ComposePeopleName() & " AS EncodedBy" & vbCrLf &
            " ,tblInvoice.FullInvoiceNumber" & vbCrLf &
            " ,tblInvoice.InvoiceDate" & vbCrLf &
            " ,tblInvoice.DocumentName" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransItemRequisitionBackOrder ON TransItemRequisitionBackOrder.BackOrderSourceID = TransInvoice.ID" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

        pSql &= "  OUTER APPLY (SELECT TOP 1 CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblInvoice.InvoiceNumber) AS FullInvoiceNumber" & vbCrLf &
            " 	,tblInvoice.InvoiceDate" & vbCrLf &
            "     ,LibraryDocumentControl.DocumentName" & vbCrLf &
            "  	FROM TransInvoice tblInvoice" & vbCrLf &
            "  	LEFT JOIN LibraryDevice ON LibraryDevice.ID = tblInvoice.DeviceID" & vbCrLf &
            "  	LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = tblInvoice.FOWSTID" & vbCrLf &
            " 	LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = tblInvoice.DocumentNameID" & vbCrLf &
            "     WHERE tblInvoice.ID = TransItemRequisitionBackOrder.BackOrderInvoiceID" & vbCrLf &
            "  ) tblInvoice" & vbCrLf

        pSql &= " WHERE TransItemRequisitionBackOrder.ItemRequisitionID = " & ReadyString(vItemRequisitionID) & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " GROUP BY TransInvoice.CustomerID" & vbCrLf &
            " ,TransInvoice.SalesOrderNo" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) " & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            "  ,MobizLibraryPeoplePreNominalSuffix.Code" & vbCrLf &
            "  ,MobizLibraryPeople.Surname" & vbCrLf &
            "  ,MobizLibraryPeople.FirstName" & vbCrLf &
            "  ,MobizLibraryPeople.MiddleName" & vbCrLf &
            "  ,MobizLibraryPeople.PersonalSuffixName" & vbCrLf &
            "  ,MobizLibraryPeople.ID" & vbCrLf &
            "  ,TransItemRequisitionBackOrder.ID" & vbCrLf &
            "  ,ISNULL(TransItemRequisitionBackOrder.IncludeRow, 0)" & vbCrLf &
            " ,tblInvoice.FullInvoiceNumber" & vbCrLf &
            " ,tblInvoice.InvoiceDate" & vbCrLf &
            " ,tblInvoice.DocumentName" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated"
        Dim pDT = mProData.Execute(pSql)
        pDT.TableName = "BackOrders"
        pDS.Tables.Add(pDT)

        Dim pDR = ItemRequisition(vItemRequisitionID)
        If pDR Is Nothing Then
            Throw New ApplicationException("vItemRequisitionID, " & vItemRequisitionID & ", does not exist")
        End If
        pDT = pDR.Table
        pDT.TableName = "ItemRequisition"
        pDS.Tables.Add(pDT)

        pDT = mParent.LibraryStock.ByIds(ReadyString(vProductID), , True)
        pDT.TableName = "Product"
        pDS.Tables.Add(pDT)

        pDT = mParent.LibraryStock.GetUnitsNew(vProductID)
        pDT.TableName = "Units"
        pDS.Tables.Add(pDT)

        Return pDS

    End Function

    Public Class SaveItemRequisitionInput
        ''' <summary>
        ''' TransItemRequisitions.ID
        ''' </summary>
        ''' <remarks></remarks>
        Public ID As String
        Public UserID As String ' set in VB
        Public RequestedQty As String ' int
        Public RequestedQtyTotal As String ' int
        Public BackOrders As List(Of BackOrderClass)
        Public Class BackOrderClass
            ''' <summary>
            ''' TransItemRequisitionBackOrder.ID
            ''' </summary>
            ''' <remarks></remarks>
            Public ID As String
            Public IncludeRow As String ' bool
            Public Qty As String ' not saved but needed for total
        End Class
    End Class

    Public Sub SaveItemRequisitionDetail(vInput As SaveItemRequisitionInput)

        Dim pTotalQty As Decimal = 0

        For Each pBackOrder In vInput.BackOrders
            If CBool2(pBackOrder.IncludeRow) Then
                pTotalQty += CDec2(pBackOrder.Qty)
            End If
        Next

        mProData.BeginTransaction()
        Dim pIU As New InsertUpdate("TransItemRequisitions")
        pIU("RequestedQty") = CDec2(vInput.RequestedQty)
        pIU("BackorderQty") = CDec2(pTotalQty)
        pIU("RequestedQtyTotal") = CDec2(vInput.RequestedQtyTotal)
        pIU("UserID") = ReadyString(vInput.UserID)
        pIU("DateRequested") = "GETDATE()"
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vInput.ID))
        mProData.ExecuteSQL(pSql)

        For Each pBackOrder In vInput.BackOrders
            pIU = New InsertUpdate("TransItemRequisitionBackOrder")
            pIU("IncludeRow") = If(CBool2(pBackOrder.IncludeRow), 1, 0)
            pSql = pIU.GetUpdate("ID = " & ReadyString(pBackOrder.ID))
            mProData.ExecuteSQL(pSql)
        Next

        mProData.CommitTransaction()

    End Sub

    Function CacheSoSiWeekQtySalesOrder(vProductID As String) As DataTable
        Dim pSql = " SELECT SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) AS Qty" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoice.ComputedSalesOrderYearWeek" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoice.TransType = 'SalesOrder'" & vbCrLf &
            " AND TransInvoice.SalesOrderNo > 0" & vbCrLf &
            " AND TransInvoice.CancelledByID IS NULL" & vbCrLf &
            " AND TransInvoice.BackorderSourceInvoiceID IS NULL" & vbCrLf &
            " GROUP BY TransInvoice.FowstID" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoice.ComputedSalesOrderYearWeek" & vbCrLf &
            " ORDER BY TransInvoice.ComputedSalesOrderYearWeek"
    End Function

    Function GetVendorPriceHistory(vVendorID As String, vProductID As String) As DataRow

        Dim pSql = "SELECT TOP 1 CurrentPrice" & vbCrLf &
            " FROM TransVendorPriceHistory" & vbCrLf &
            " WHERE VendorID = " & ReadyString(vVendorID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
            " ORDER BY TransDate DESC"
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Function SaveVendorPriceHistory(vPurchaseOrderID As String, Optional vProductID As String = Nothing)

        Dim pUnitQty As Decimal
        Dim pLastUpdated As String
        Dim pDS = GetReportData("WHERE TransInvoice.ID = " & ReadyString(vPurchaseOrderID), False)
        If pDS.Tables("Invoices").Rows.Count = 0 Then
            Return Nothing
        End If
        Dim pVendorID = CStr2(pDS.Tables("Invoices").Rows(0)("CustomerID"))
        If pDS.Tables("Invoices").Rows(0)("PurchaseOrderDate") Is DBNull.Value Then
            pLastUpdated = Format(pDS.Tables("Invoices").Rows(0)("LastUpdated"), cGlobal.FormatDateTimeSql)
        Else
            pLastUpdated = Format(pDS.Tables("Invoices").Rows(0)("PurchaseOrderDate"), cGlobal.FormatDateTimeSql)
        End If
        For Each pDrStock As DataRow In pDS.Tables("Stocks").Rows
            If String.IsNullOrWhiteSpace(vProductID) = False Then
                If ReadyGuid(pDrStock("ProductID")) = ReadyGuid(vProductID) Then
                    Continue For
                End If
            End If
            Dim pUnitPrice As Decimal
            Dim pPreviousPrice As Decimal = 0
            If CDec2(pDrStock("ActualQuantity")) = 0 Then
                pUnitPrice = CDec2(pDrStock("UnitPrice"))
                pUnitQty = 1
            Else
                pUnitPrice = CDec2(pDrStock("UnitPrice")) / CDec2(pDrStock("ActualQuantity"))
                pUnitQty = CDec2(pDrStock("ActualQuantity"))
            End If
            Dim pDrHistory = GetVendorPriceHistory(pVendorID, CStr2(pDrStock("ProductID")))
            If pDrHistory IsNot Nothing Then
                pPreviousPrice = CDec2(pDrHistory("CurrentPrice"))
                If Math.Abs(pPreviousPrice - pUnitPrice) < 0.01 Then
                    Continue For
                End If
            ElseIf pUnitPrice = 0 Then
                Continue For
            End If
            Dim pIU = New InsertUpdate("TransVendorPriceHistory")
            pIU("ID") = "NEWID()"
            pIU("VendorID") = ReadyString(pVendorID)
            pIU("ProductID") = ReadyString(pDrStock("ProductID"))
            pIU("TransDate") = ReadyString(pLastUpdated)
            pIU("TransID") = ReadyString(vPurchaseOrderID)
            pIU("TransType") = ReadyString(pDS.Tables("Invoices").Rows(0)("TransType"))

            If pDrHistory IsNot Nothing Then
                pIU("PreviousPrice") = pPreviousPrice
            End If
            pIU("CurrentPrice") = pUnitPrice
            pIU("HistoryCreationDate") = "GETDATE()"
            Dim pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            mParent.LibraryPriceCanvass.UpdateLibraryVendorPrices(vVendorID:=pVendorID, vProductID:=CStr2(pDrStock("ProductID")), vPrice:=pUnitPrice, vUnitQty:=pUnitQty)
        Next

    End Function

    Public Sub SaveVendorPriceHistory(vVendorID As String, vProductID As String, vUnitPrice As Decimal, vTransDate As Date, vTransID As String, vTransType As String)

        Dim pIU = New InsertUpdate("TransVendorPriceHistory")
        pIU("ID") = "NEWID()"
        pIU("VendorID") = ReadyString(vVendorID)
        pIU("ProductID") = ReadyString(vProductID)
        pIU("TransDate") = ReadyString(vTransDate.ToString(cGlobal.FormatDateTimeSql))
        pIU("TransID") = ReadyString(vTransID)
        pIU("TransType") = ReadyString(vTransType)

        Dim pDrHistory = GetVendorPriceHistory(vVendorID, vProductID)
        If pDrHistory IsNot Nothing Then
            Dim pPreviousPrice = CDec2(pDrHistory("CurrentPrice"))
            If Math.Abs(pPreviousPrice - vUnitPrice) < 0.01 Then
                Return ' dont save this price history
            End If
            pIU("PreviousPrice") = pPreviousPrice
        End If
        pIU("CurrentPrice") = vUnitPrice
        pIU("HistoryCreationDate") = "GETDATE()"
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function UpdateItemPaymentTerm(vInvoiceID As String) As Integer

        mProData.BeginTransaction()
        Dim pSql = " SELECT TransInvoice.PaymentTerm AS InvoiceDays" & vbCrLf &
            " ,TransInvoiceItems.ID AS LineItemID" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,LibraryPaymentTerms.Days AS ProductDays" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationPIM.PaymentTermID" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vInvoiceID)
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("TransInvoiceItems")
            If pDR("ProductDays") Is DBNull.Value Then
                pIU("TermNumericValue") = CInt2(pDR("InvoiceDays"))
            Else
                pIU("TermNumericValue") = Math.Min(CInt2(pDR("ProductDays")), CInt2(pDR("InvoiceDays")))
            End If
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("LineItemID")))
            mProData.ExecuteSQL(pSql)
        Next
        mProData.CommitTransaction()

    End Function

    Public Class MergePurchaseOrdersInput

        Public PurchaseOrders As List(Of PurchaseOrder)

        Public Class PurchaseOrder
            Public ID As String
            Public VendorID As String ' added in VB
        End Class
    End Class

    Public Sub MergePurchaseOrders(vInput As MergePurchaseOrdersInput)

        Dim pVendorIDs As New List(Of String)

        For Each pPurchaseOrder In vInput.PurchaseOrders
            Dim pDr = ById(pPurchaseOrder.ID)
            If pDr Is Nothing Then
                Throw New ApplicationException("Purchase Order " & pPurchaseOrder.ID & " does not exist. ref: vbSVN2BXC7J9!")
            End If
            Dim pVendorID = ReadyGuid(pDr("CustomerID"))
            If pVendorIDs.Contains(pVendorID) = False Then
                pVendorIDs.Add(pVendorID)
            End If
            pPurchaseOrder.VendorID = pVendorID
        Next

        For Each pVendorID In pVendorIDs
            Dim pPurchaseOrders =
                From pPO In vInput.PurchaseOrders
                Where pPO.VendorID = pVendorID
                Select pPO

            If pPurchaseOrders.Count <= 1 Then
                Continue For
            End If

            MergePurchaseOrders(pPurchaseOrders.ToList())
        Next

    End Sub

    Private Sub MergePurchaseOrders(vPurchaseOrders As List(Of MergePurchaseOrdersInput.PurchaseOrder))

        Dim pPoIds As String
        Dim pParentPoId As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pPurchaseOrder In vPurchaseOrders
            If pParentPoId Is Nothing Then
                pParentPoId = pPurchaseOrder.ID
            Else
                If pPoIds IsNot Nothing Then
                    pPoIds &= ", "
                End If
                pPoIds &= ReadyString(pPurchaseOrder.ID)
            End If
        Next

        mProData.BeginTransaction()
        pIU = New InsertUpdate("TransInvoice")
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(pParentPoId))
        pSql = mProData.ExecuteSQL(pSql) 'output is used for debugging only

        pSql = "SELECT * " & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID IN (" & pPoIds & ")"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransInvoiceItems"
            pIU("ID") = "NEWID()"
            pIU("InvoiceID") = ReadyString(pParentPoId)
            pIU.Remove("COGS") ' computed
            pIU.Remove("DiscountTax") ' computed
            pIU.Remove("DiscountedUnitPrice") ' computed
            pIU.Remove("GrossSalesNetOfVATExemptions") ' computed
            pIU.Remove("HeaderArea") ' derived column
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next

        pIU = New InsertUpdate("TransInvoice")
        pIU("IsCancelled") = 1
        pIU("CancelledByID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("DateCancelled") = "GETDATE()"
        pSql = pIU.GetUpdate("ID IN (" & pPoIds & ")")
        pSql = mProData.ExecuteSQL(pSql) 'output is used for debugging only
        mProData.CommitTransaction()
    End Sub

    Public Function PoReceivedPercent(vID As String) As Decimal

        Dim pSql = " -- Fully served purchase order" & vbCrLf &
            " WITH CTE AS (" & vbCrLf &
            " SELECT TransInvoiceItems.ProductID" & vbCrLf &
            " ,SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) AS OrderedQuantity" & vbCrLf &
            " ,tblReceiving.ReceivedUnits" & vbCrLf &
            " ,CASE WHEN SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) < tblReceiving.ReceivedUnits" & vbCrLf &
            " 	THEN SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity)" & vbCrLf &
            "     ELSE tblReceiving.ReceivedUnits" & vbCrLf &
            " 	END AS ReceivedUnitsLimited" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT SUM((TransReceivingStock.QuantityGood + TransReceivingStock.QuantityDamaged) * TransReceivingStock.ActualQuantityGood) AS ReceivedUnits" & vbCrLf &
            "     FROM TransReceiving" & vbCrLf &
            "     JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID" & vbCrLf &
            "     WHERE TransReceiving.PurchaseOrderID = TransInvoice.ID" & vbCrLf &
            "     AND TransReceivingStock.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "     AND TransReceiving.CancelledBy IS NULL" & vbCrLf &
            "     GROUP BY TransReceivingStock.ProductID" & vbCrLf &
            "     -- ,TransReceivingStock.ActualQuantityGood" & vbCrLf &
            " ) tblReceiving" & vbCrLf &
            " WHERE TransInvoice.ID = " & ReadyString(vID) & vbCrLf &
            " GROUP BY TransInvoiceItems.ProductID" & vbCrLf &
            " ,tblReceiving.ReceivedUnits" & vbCrLf &
            " )" & vbCrLf &
            " SELECT SUM(ReceivedUnitsLimited) / SUM(OrderedQuantity) * 100 from cte" & vbCrLf &
            " -- SELECT  *  from cte"
        Dim pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function UpdatePoReceivedPercent(vID As String) As Decimal

        Dim pValue = PoReceivedPercent(vID)
        Dim pIU = New InsertUpdate("TransInvoice")
        pIU("PoReceivedPercent") = pValue
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        Return pValue

    End Function

    Public Sub UpdateLineNumbers(vID As String)

        Dim pLineNumber As Integer
        Dim pDT = GetItems(vID, True)
        For Each pDR As DataRow In pDT.Rows
            pLineNumber += 1
            Dim pIU = New InsertUpdate("TransInvoiceItems")
            pIU("LineNumber") = pLineNumber
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next

    End Sub

    Public Function CacheReceivablePurchaseOrder() As Integer

        Dim pIU = New InsertUpdate("TransInvoice")
        pIU("ReceivablePurchaseOrderQty") = "t.Receivables"

        pIU.FromSql = " -- receivable purchase orders" & vbCrLf &
            " SELECT TOP 100 percent TransInvoice.ID AS [ID]" & vbCrLf &
            " , RIGHT('000000000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 12) AS [InvoiceNum]" & vbCrLf &
            " ,  (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [CompanyName]" & vbCrLf &
            " , TransInvoice.InvoiceDate AS [InvoiceDate]" & vbCrLf &
            " , SUM(tblItems.Total) AS [Total]" & vbCrLf &
            " , SUM(tblItems.SkuCount) AS [SkuCount]" & vbCrLf &
            " , SUM(tblItems.Receivables) AS [Receivables]" & vbCrLf &
            " , MobizLibraryOrganizationFOWST.Name AS [FOWSTName]" & vbCrLf &
            " , LibraryDevice.PhoneName AS [DeviceName]" & vbCrLf &
            " , 	CASE TransInvoice.FOWSTID  WHEN 'A8FDFB5A-CD17-4096-9FB2-3A2640D379CA' THEN 0  ELSE    1  END  AS [OrderByFowst]" & vbCrLf &
            "   FROM TransInvoice" & vbCrLf &
            "  INNER JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "  INNER JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            "  JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
            "  CROSS APPLY" & vbCrLf &
            "  	(SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
            "  	,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
            "    ,SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) - ISNULL(tblReceiving.QuantityReceived, 0) AS Receivables" & vbCrLf &
            "    FROM TransInvoiceItems" & vbCrLf &
            "  	OUTER APPLY" & vbCrLf &
            "      	(SELECT SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS QuantityReceived" & vbCrLf &
            "      	FROM TransReceivingStock" & vbCrLf &
            "        JOIN TransReceiving ON TransReceiving.ID = TransReceivingStock.ReceivingID" & vbCrLf &
            "  		WHERE TransReceiving.IsCancelled = 0" & vbCrLf &
            "        AND TransReceivingStock.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "        AND TransReceiving.PurchaseOrderID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "  	) AS tblReceiving" & vbCrLf &
            "  	WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "    AND TransInvoiceItems.OrderedQuantity > 0" & vbCrLf &
            "  	GROUP BY tblReceiving.QuantityReceived" & vbCrLf &
            "    HAVING SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) > ISNULL(tblReceiving.QuantityReceived, 0)" & vbCrLf &
            "  ) AS tblItems" & vbCrLf &
            "   WHERE ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
            "  AND TransInvoice.PurchaseOrderNo <> 0" & vbCrLf &
            "  AND TransInvoice.ReceivablePurchaseOrderQty IS NULL" & vbCrLf &
            "  AND tblItems.Receivables > 0" & vbCrLf &
            "   GROUP BY TransInvoice.ID" & vbCrLf &
            "  ,RIGHT('000000000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 12)" & vbCrLf &
            "  , (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END)" & vbCrLf &
            "  , TransInvoice.FowstID" & vbCrLf &
            "  , TransInvoice.DeviceID" & vbCrLf &
            "  , TransInvoice.InvoiceDate" & vbCrLf &
            "  , MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            "  , LibraryDevice.PhoneName" & vbCrLf &
            "  ,TransInvoice.LastUpdated" & vbCrLf &
            "    ORDER BY" & vbCrLf &
            "  (CASE TransInvoice.FOWSTID  WHEN 'A8FDFB5A-CD17-4096-9FB2-3A2640D379CA' THEN 0  ELSE    1  END) ASC," & vbCrLf &
            "  TransInvoice.InvoiceDate DESC" & vbCrLf &
            "  ,TransInvoice.LastUpdated DESC"
        pIU.FromSql = " FROM (" & pIU.FromSql & ") t"
        Dim pSql = pIU.GetUpdate("1=1", True) ' no need for condition if using FromSql
        Dim pOut = mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Function SplitItemRequisition(vItemRequisitionsID As String) As Integer


    End Function

    Public Function ItemRequisitionBackOrders(vItemRequisitionsID As String) As DataTable

        Dim pSql As String

        pSql = " SELECT TransItemRequisitionBackOrder.ID AS ItemRequisitionBackOrderID" & vbCrLf &
            " ,tblBackOrder.CustomerID AS [BackOrderCustomerID]" & vbCrLf &
            " ,tblBackOrderItems.ActualQuantity * tblBackOrderItems.OrderedQuantity AS [BackOrderQty]" & vbCrLf &
            " FROM TransItemRequisitions" & vbCrLf &
            " JOIN TransItemRequisitionBackOrder ON TransItemRequisitionBackOrder.ItemRequisitionID = TransItemRequisitions.ID" & vbCrLf &
            " LEFT JOIN TransInvoice tblBackOrder ON tblBackOrder.ID = TransItemRequisitionBackOrder.BackOrderSourceID" & vbCrLf &
            " LEFT JOIN TransInvoiceItems tblBackOrderItems ON tblBackOrderItems.InvoiceID = tblBackOrder.ID" & vbCrLf &
            " AND tblBackOrderItems.ProductID = TransItemRequisitions.ProductID" & vbCrLf &
            " WHERE TransItemRequisitions.ID = " & ReadyString(vItemRequisitionsID)
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function FixItemRequisitionQty() As Integer

        Dim pSql As String

        pSql = "  SELECT TransItemRequisitions.ID" & vbCrLf &
            "  ,TransItemRequisitions.BackorderQty" & vbCrLf &
            "  ,TransItemRequisitions.RequestedQty" & vbCrLf &
            "  ,TransItemRequisitions.RequestedQtyTotal" & vbCrLf &
            "  ,SUM(tblBackOrderItems.ActualQuantity * tblBackOrderItems.OrderedQuantity) AS SumBackOrderQty" & vbCrLf &
            "  FROM TransItemRequisitions" & vbCrLf &
            "  LEFT JOIN TransItemRequisitionBackOrder ON TransItemRequisitionBackOrder.ItemRequisitionID = TransItemRequisitions.ID" & vbCrLf &
            "  LEFT JOIN TransInvoice tblBackOrder ON tblBackOrder.ID = TransItemRequisitionBackOrder.BackOrderSourceID" & vbCrLf &
            "  LEFT JOIN TransInvoiceItems tblBackOrderItems ON tblBackOrderItems.InvoiceID = tblBackOrder.ID" & vbCrLf &
            "  AND tblBackOrderItems.ProductID = TransItemRequisitions.ProductID" & vbCrLf &
            "  GROUP BY TransItemRequisitions.ID" & vbCrLf &
            "  ,TransItemRequisitions.BackorderQty" & vbCrLf &
            "  ,TransItemRequisitions.RequestedQty" & vbCrLf &
            "  ,TransItemRequisitions.RequestedQtyTotal" & vbCrLf &
            "  HAVING TransItemRequisitions.BackorderQty <> ISNULL(SUM(tblBackOrderItems.ActualQuantity * tblBackOrderItems.OrderedQuantity), 0)" & vbCrLf &
            "  OR TransItemRequisitions.RequestedQty + TransItemRequisitions.BackorderQty <> TransItemRequisitions.RequestedQtyTotal"
        Dim pDT = mProData.Execute(pSql)

        Return pDT.Rows.Count

    End Function

    Public Function GetAverageWeeklyDemand(vProductID As String) As DataTable

        Dim pSql = " -- average weekly demand" & vbCrLf &
            " SELECT SUM(TransInvoiceItems.OrderedQuantity) AS OrderedQuantity" & vbCrLf &
            " ,YEAR(TransInvoice.SalesOrderDate) AS SalesOrderYear" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoice.TransType = 'SalesOrder'" & vbCrLf &
            " AND TransInvoice.SalesOrderNo > 0" & vbCrLf &
            " AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " GROUP BY YEAR(TransInvoice.SalesOrderDate)" & vbCrLf &
            " HAVING YEAR(TransInvoice.SalesOrderDate) <= YEAR(GETDATE())" & vbCrLf &
            " ORDER BY YEAR(TransInvoice.SalesOrderDate)"
        Dim pDT = mProData.Execute(pSql)
        pDT.Columns.Add("AverageWeeklyOrderQty", GetType(Decimal))
        pDT.Columns.Add("Days", GetType(Decimal))
        Dim pCalendar = New System.Globalization.GregorianCalendar()
        For Each pDR As DataRow In pDT.Rows
            Dim pDays As Integer
            If New Date(pDR("SalesOrderYear"), 12, 31) > Now.Date Then
                pDays = CInt2((Now.Date - New Date(Now.Date.Year, 1, 1)).TotalDays)
            Else
                pDays = pCalendar.GetDaysInYear(pDR("SalesOrderYear"))
            End If
            pDR("AverageWeeklyOrderQty") = CDec2(pDR("OrderedQuantity")) / (pDays / 7)
            pDR("Days") = pDays
        Next

        Return pDT

    End Function

    Function VendorPriceHistory(vVendorID As String, vProductID As String) As DataTable

        Dim pSql = "SELECT TOP 50 TransVendorPriceHistory.CurrentPrice" & vbCrLf &
            ",TransVendorPriceHistory.PreviousPrice" & vbCrLf &
            ",TransVendorPriceHistory.TransDate" & vbCrLf &
            ",TransVendorPriceHistory.TransType" & vbCrLf &
            " FROM TransVendorPriceHistory" & vbCrLf &
            " WHERE VendorID = " & ReadyString(vVendorID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
            " ORDER BY TransDate DESC"
        Dim pDT = mProData.Execute(pSql)
        pDT.Columns.Add("PercentChange", GetType(Decimal))
        pDT.Columns.Add("NominalChange", GetType(Decimal))
        For Each pDR In pDT.Rows
            pDR("NominalChange") = CDec2(pDR("CurrentPrice")) - CDec2(pDR("PreviousPrice"))
            If CDec2(pDR("PreviousPrice")) > 0.001 Then
                pDR("PercentChange") = pDR("NominalChange") / CDec2(pDR("PreviousPrice")) * 100
            End If
        Next

        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID">TransInvoice.ID</param>
    ''' <param name="vQuantityField">Quantity or OrderedQuantity</param>
    ''' <remarks></remarks>
    Public Sub CopyToItemRequisition(vID As String, vQuantityField As String, vUserID As String, vFowstID As String)

        mProData.BeginTransaction()
        Dim pDtItems = GetItems(vID, True)
        For Each pDrItem As DataRow In pDtItems.Rows
            SaveToItemRequisition(
                vProductID:=CStr2(pDrItem("ProductID")),
                vQty:=CDec2(pDrItem(vQuantityField)),
                vUnitQty:=CDec2(pDrItem("ActualQuantity")),
                vUnitMeasure:=CStr2(pDrItem("UnitMeasure")),
                vUserID:=vUserID,
                vRemarks:="",
                vFowstID:=vFowstID
            )
        Next
        mProData.CommitTransaction()

    End Sub

    Private Sub SaveToItemRequisition(vProductID As String,
        vQty As Decimal,
        vUnitQty As Decimal,
        vUnitMeasure As String,
        vUserID As String,
        vRemarks As String,
        vFowstID As String)

        Dim pIU As New InsertUpdate("TransItemRequisitions")
        pIU("ID") = "NEWID()"
        pIU("ProductID") = ReadyString(vProductID)
        pIU("RequestedByID") = ReadyString(vUserID)
        pIU("RequestedQty") = CDec2(vQty)
        pIU("RequestedQtyTotal") = CDec2(vQty)
        pIU("UnitMeasure") = ReadyString(vUnitMeasure)
        pIU("UnitQty") = CDec2(vUnitQty)
        pIU("UserID") = ReadyString(vUserID)
        pIU("Remarks") = ReadyString(vRemarks)
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("DateRequested") = "GETDATE()"
        Dim pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub ExportToXmlZip(vInvoiceID As String)

        Dim pProZip As New ProZip
        Dim pInvoiceIds As String
        Dim pSqlWhere = " WHERE TransInvoice.ID = " & ReadyString(vInvoiceID) & vbCrLf
        Dim pDT = mProData.Execute("SELECT * FROM TransInvoice" & vbCrLf &
            pSqlWhere &
            " ORDER BY TransInvoice.InvoiceNumber, TransInvoice.LastUpdated")
        If pDT.Rows.Count > 0 Then
            pInvoiceIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            pDT.TableName = "TransInvoice"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceItems.* " +
                " FROM TransInvoiceItems" +
                " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                " ORDER BY TransInvoiceItems.LineNumber")
            pDT.TableName = "TransInvoiceItems"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceItemsModifier.* " +
                " FROM TransInvoiceItemsModifier" +
                " WHERE TransInvoiceItemsModifier.InvoiceItemID IN " & vbCrLf +
                " (SELECT TransInvoiceItems.ID" +
                " 	FROM TransInvoiceItems" +
                " 	WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                ")")
            pDT.TableName = "TransInvoiceItemsModifier"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceDiscount.* " +
                " FROM TransInvoiceDiscount" +
                " WHERE TransInvoiceDiscount.InvoiceID IN (" + pInvoiceIds + ")")
            pDT.TableName = "TransInvoiceDiscount"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceSeniorCitizenPWDCustomer.* " +
                " FROM TransInvoiceSeniorCitizenPWDCustomer" +
                " WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID IN (" + pInvoiceIds + ")")
            pDT.TableName = "TransInvoiceSeniorCitizenPWDCustomer"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceCharges.* " +
                " FROM TransInvoiceCharges" +
                " WHERE TransInvoiceCharges.InvoiceID IN (" + pInvoiceIds + ")")
            pDT.TableName = "TransInvoiceCharges"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceItemSerials.* " +
                " FROM TransInvoiceItemSerials" +
                " WHERE TransInvoiceItemSerials.InvoiceID IN (" + pInvoiceIds + ")")
            pDT.TableName = "TransInvoiceItemSerials"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceItemSalesReps.*" + vbCrLf +
                " FROM TransInvoiceItemSalesReps" + vbCrLf +
                " JOIN TransInvoiceItems ON TransInvoiceItemSalesReps.InvoiceID = TransInvoiceItems.InvoiceID" + vbCrLf +
                " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")")
            pDT.TableName = "TransInvoiceItemSalesReps"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransInvoiceItemsRebates.* " +
                " FROM TransInvoiceItemsRebates" +
                " WHERE TransInvoiceItemsRebates.ModifierGroupID IN " +
                " (SELECT TransInvoiceItems.ModifierGroupID" +
                " 	FROM TransInvoiceItems" +
                " 	WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                ")")
            pDT.TableName = "TransInvoiceItemsRebates"
            pProZip.AddTable(pDT)
        End If

        Dim pSql = "SELECT * FROM TransCustomerPayment" +
            " WHERE ID IN " +
            "   (SELECT TransCustomerPaymentInvoices.CustomerPaymentID" +
            "   FROM TransCustomerPaymentInvoices" +
            "   WHERE TransCustomerPaymentInvoices.InvoiceID IN (" + pInvoiceIds + ")" +
            "   )" & vbCrLf
        pSql &= " ORDER BY TransCustomerPayment.LastUpdated"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Dim pCustomerPaymentIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            pDT.TableName = "TransCustomerPayment"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransCustomerPaymentInvoices.* " +
                    " FROM TransCustomerPaymentInvoices" +
                    " WHERE TransCustomerPaymentInvoices.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pDT.TableName = "TransCustomerPaymentInvoices"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransCustomerPaymentDebitCard.*" + vbCrLf +
                " FROM TransCustomerPaymentDebitCard" + vbCrLf +
                " WHERE TransCustomerPaymentDebitCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pDT.TableName = "TransCustomerPaymentDebitCard"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransCustomerPaymentCreditCard.*" + vbCrLf +
                " FROM TransCustomerPaymentCreditCard" + vbCrLf +
                " WHERE TransCustomerPaymentCreditCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pDT.TableName = "TransCustomerPaymentCreditCard"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransCustomerPaymentChecks.*" + vbCrLf +
                " FROM TransCustomerPaymentChecks" + vbCrLf +
                " WHERE TransCustomerPaymentChecks.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pDT.TableName = "TransCustomerPaymentChecks"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT TransCustomerPaymentOther.*" + vbCrLf +
                " FROM TransCustomerPaymentOther" + vbCrLf +
                " WHERE TransCustomerPaymentOther.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pDT.TableName = "TransCustomerPaymentOther"
            pProZip.AddTable(pDT)
        End If

        Dim pFilename As String = Common.ServerPath & "temp\ExportedInvoices\ExportedInvoices_" & ReadyGuid(vInvoiceID) & ".zip"
        CreateFolder(pFilename)
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If
        pProZip.Save(pFilename)

    End Sub

    Public Function GetFirstEodDate(vFowstID As String, vDeviceID As String) As Date?

        Dim pSql = "SELECT SalesDate" & vbCrLf &
            " FROM TenantMonitoringLogs" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " ORDER BY SalesDate"
        Dim pSalesDate = mParent.ProData.ExecuteScalar(pSql)

        Return pSalesDate

    End Function

    Public Function ExportToCsvZip(vDateFrom As Date, vDateTo As Date, vFowstID As String, vDeviceID As String) As String

        If mProData.CommandTimeout < 5 * 60 Then
            mProData.CommandTimeout = 5 * 60
        End If
        Dim pProZip As New ProZip
        Dim pInvoiceIds As String
        Dim pSqlWhere = " WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vDateFrom.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(vDateTo.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " AND TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransInvoice.DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        Dim pDT = mProData.Execute("SELECT * FROM TransInvoice" & vbCrLf &
            pSqlWhere &
            " ORDER BY TransInvoice.InvoiceNumber, TransInvoice.LastUpdated")
        pInvoiceIds = "SELECT ID FROM TransInvoice" & vbCrLf &
                pSqlWhere & vbCrLf
        If pDT.Rows.Count > 0 Then
            'pInvoiceIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            pProZip.AddTableCsv(pDT, "TransInvoice")

            pDT = mProData.Execute("SELECT TransInvoiceItems.* " +
                " FROM TransInvoiceItems" +
                " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                " ORDER BY TransInvoiceItems.LineNumber")
            pProZip.AddTableCsv(pDT, "TransInvoiceItems")

            pDT = mProData.Execute("SELECT TransInvoiceItemsModifier.* " +
                " FROM TransInvoiceItemsModifier" +
                " WHERE TransInvoiceItemsModifier.InvoiceItemID IN " & vbCrLf +
                " (SELECT TransInvoiceItems.ID" +
                " 	FROM TransInvoiceItems" +
                " 	WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceItemsModifier")

            pDT = mProData.Execute("SELECT TransInvoiceDiscount.* " +
                " FROM TransInvoiceDiscount" +
                " WHERE TransInvoiceDiscount.InvoiceID IN (" + pInvoiceIds + ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceDiscount")

            pDT = mProData.Execute("SELECT TransInvoiceSeniorCitizenPWDCustomer.* " +
                " FROM TransInvoiceSeniorCitizenPWDCustomer" +
                " WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID IN (" + pInvoiceIds + ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceSeniorCitizenPWDCustomer")

            pDT = mProData.Execute("SELECT TransInvoiceCharges.* " +
                " FROM TransInvoiceCharges" +
                " WHERE TransInvoiceCharges.InvoiceID IN (" + pInvoiceIds + ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceCharges")

            pDT = mProData.Execute("SELECT TransInvoiceItemSerials.* " +
                " FROM TransInvoiceItemSerials" +
                " WHERE TransInvoiceItemSerials.InvoiceID IN (" + pInvoiceIds + ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceItemSerials")

            pDT = mProData.Execute("SELECT TransInvoiceItemSalesReps.*" + vbCrLf +
                " FROM TransInvoiceItemSalesReps" + vbCrLf +
                " JOIN TransInvoiceItems ON TransInvoiceItemSalesReps.InvoiceID = TransInvoiceItems.InvoiceID" + vbCrLf +
                " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceItemSalesReps")

            pDT = mProData.Execute("SELECT TransInvoiceItemsRebates.* " +
                " FROM TransInvoiceItemsRebates" +
                " WHERE TransInvoiceItemsRebates.ModifierGroupID IN " +
                " (SELECT TransInvoiceItems.ModifierGroupID" +
                " 	FROM TransInvoiceItems" +
                " 	WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                ")")
            pProZip.AddTableCsv(pDT, "TransInvoiceItemsRebates")

            pDT = mProData.Execute("SELECT TransMemoSalesReturn.*" + vbCrLf +
                " FROM TransMemoSalesReturn" + vbCrLf +
                " WHERE TransMemoSalesReturn.InvoiceID IN (" + pInvoiceIds + ")")
            pProZip.AddTableCsv(pDT, "TransMemoSalesReturn")

            pDT = mProData.Execute("SELECT TransSalesReturnLedger.*" + vbCrLf +
                " FROM TransSalesReturnLedger" & vbCrLf &
                " JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransSalesReturnLedger.SourceMemoID" + vbCrLf +
                " WHERE TransMemoSalesReturn.InvoiceID IN (" + pInvoiceIds + ")" & vbCrLf &
                " UNION " & vbCrLf &
                " SELECT TransSalesReturnLedger.*" + vbCrLf +
                " FROM TransSalesReturnLedger" & vbCrLf &
                " JOIN TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID" + vbCrLf +
                " WHERE CONVERT(date, TransMemo.LastUpdated) BETWEEN " & ReadyString(vDateFrom.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(vDateTo.ToString(cGlobal.FormatDateSql)))
            pProZip.AddTableCsv(pDT, "TransSalesReturnLedger")

        End If

        Dim pSql = "SELECT * FROM TransCustomerPayment" +
            " WHERE ID IN " +
            "   (SELECT TransCustomerPaymentInvoices.CustomerPaymentID" +
            "   FROM TransCustomerPaymentInvoices" +
            "   WHERE TransCustomerPaymentInvoices.InvoiceID IN (" + pInvoiceIds + ")" +
            "   )" & vbCrLf
        pSql &= " ORDER BY TransCustomerPayment.LastUpdated"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            'Dim pCustomerPaymentIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            Dim pCustomerPaymentIds = "SELECT ID FROM TransCustomerPayment" +
            " WHERE ID IN " +
            "   (SELECT TransCustomerPaymentInvoices.CustomerPaymentID" +
            "   FROM TransCustomerPaymentInvoices" +
            "   WHERE TransCustomerPaymentInvoices.InvoiceID IN (" + pInvoiceIds + ")" +
            "   )" & vbCrLf
            pProZip.AddTableCsv(pDT, "TransCustomerPayment")

            pDT = mProData.Execute("SELECT TransCustomerPaymentInvoices.* " +
                    " FROM TransCustomerPaymentInvoices" +
                    " WHERE TransCustomerPaymentInvoices.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pProZip.AddTableCsv(pDT, "TransCustomerPaymentInvoices")

            pDT = mProData.Execute("SELECT TransCustomerPaymentDebitCard.*" + vbCrLf +
                " FROM TransCustomerPaymentDebitCard" + vbCrLf +
                " WHERE TransCustomerPaymentDebitCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pProZip.AddTableCsv(pDT, "TransCustomerPaymentDebitCard")

            pDT = mProData.Execute("SELECT TransCustomerPaymentCreditCard.*" + vbCrLf +
                " FROM TransCustomerPaymentCreditCard" + vbCrLf +
                " WHERE TransCustomerPaymentCreditCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pProZip.AddTableCsv(pDT, "TransCustomerPaymentCreditCard")

            pDT = mProData.Execute("SELECT TransCustomerPaymentChecks.*" + vbCrLf +
                " FROM TransCustomerPaymentChecks" + vbCrLf +
                " WHERE TransCustomerPaymentChecks.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pProZip.AddTableCsv(pDT, "TransCustomerPaymentChecks")

            pDT = mProData.Execute("SELECT TransCustomerPaymentOther.*" + vbCrLf +
                " FROM TransCustomerPaymentOther" + vbCrLf +
                " WHERE TransCustomerPaymentOther.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            pProZip.AddTableCsv(pDT, "TransCustomerPaymentOther")
        End If

        pSql = "SELECT *" & vbCrLf &
            " FROM TransMemo" & vbCrLf &
            " WHERE CONVERT(date, TransMemo.LastUpdated) BETWEEN " & ReadyString(vDateFrom.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(vDateTo.ToString(cGlobal.FormatDateSql)) & vbCrLf
        pDT = mProData.Execute(pSql)
        pProZip.AddTableCsv(pDT, "TransMemo")

        Dim pBaseFilename = DateTimeStamp() & "_" & GenerateString(10)
        Dim pFilename As String = Common.ServerPath & "temp\ExportedInvoicesCsv\ExportedInvoicesCsv_" & pBaseFilename & ".zip"
        CreateFolder(pFilename)
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If
        pProZip.Save(pFilename)

        Return pBaseFilename

    End Function

    Public Function InvoiceNotInInventory(vInvoiceDateFrom As DateTime, vInvoiceDateTo As DateTime) As DataTable

        Dim pSql = " -- compare TransInventoryLedger And TransInvoice" & vbCrLf &
            " SELECT TOP 100 TransInvoice.ID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,TransInvoice.InvoiceDate" & vbCrLf &
            " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber) AS InvoiceNum" & vbCrLf &
            " ,MobizLibraryPimProduct.ProductNo" & vbCrLf &
            " ,TransInvoiceItems.Quantity" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS Fowst" & vbCrLf &
            "  ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName AS EmployeeFullName" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " AND TransInvoiceItems.Quantity > 0" & vbCrLf &
            " LEFT JOIN MobizLibraryPimProduct ON MobizLibraryPimProduct.ID = Transinvoiceitems.ProductID" & vbCrLf &
            " LEFT JOIN TransInventoryLedger ON TransInventoryLedger.TransactionID = TransInvoice.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " AND TransInventoryLedger.ID IS NULL" & vbCrLf &
            " AND (" & vbCrLf &
            "   ISNULL(TransInvoice.TransType, '') = 'invoice'" & vbCrLf &
            "   OR ISNULL(TransInvoice.TransType, '') = ''" & vbCrLf &
            " )" & vbCrLf &
            " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Format(vInvoiceDateFrom, cGlobal.FormatDateSql)) & " AND " & ReadyString(Format(vInvoiceDateTo, cGlobal.FormatDateSql)) & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated "
        Dim pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    Public Function GetSalesBySalesRep(vInvoiceDateFrom As Date, vInvoiceDateTo As Date) As DataTable

        Dim pCustomerSelect As String
        Dim pCustomerFrom As String

        BrowserController.ComposeCustomer(pCustomerSelect, pCustomerFrom,, "MobizLibraryPeoplePreNominalSuffix_Customer")

        Dim pSql = "SELECT " & BrowserController.ComposePeopleName() & " AS SalesRepName" & vbCrLf &
            " ,LibraryOrganizationCustomerSalesReps.PeopleID" & vbCrLf &
            " ,SUM(TransInvoice.TotalAmount) AS [VAT IN Gross]" & vbCrLf &
            " ,SUM(TransInvoice.VAT) AS VAT" & vbCrLf &
            " ,SUM(CacheInvoices.NetInvoiceBalance) AS [VAT IN Net]" & vbCrLf &
            " ,SUM(CacheInvoices.AdjustmentAmount) AS [Adjustments]" & vbCrLf &
            " ,TransInvoice.CustomerID" & vbCrLf &
            "," & pCustomerSelect & " AS Customer" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN CacheInvoices ON CacheInvoices.ID = TransInvoice .ID" & vbCrLf &
            " JOIN LibraryOrganizationCustomerSalesReps ON LibraryOrganizationCustomerSalesReps.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
            " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationCustomerSalesReps.PeopleID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
            pCustomerFrom & vbCrLf &
            " WHERE ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
            " AND TransInvoice.TransType = 'Invoice'" & vbCrLf &
            " AND TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " AND CONVERT(date,TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vInvoiceDateFrom.Date) & " AND " & ReadyString(vInvoiceDateTo.Date) & vbCrLf &
            " GROUP BY " & pCustomerSelect & vbCrLf & vbCrLf &
            "," & BrowserController.ComposePeopleName() & vbCrLf &
            ",LibraryOrganizationCustomerSalesReps.PeopleID" & vbCrLf &
            ",TransInvoice.CustomerID" & vbCrLf &
            " ORDER BY " & BrowserController.ComposePeopleName() & vbCrLf &
            "," & pCustomerSelect
        Dim pDT = mProData.Execute(pSql)
        Dim pDtNew = pDT.Clone()

        Dim pDtCustomers = pDT.DefaultView.ToTable(True, "CustomerID")
        For Each pDrCustomer As DataRow In pDtCustomers.Rows
            Dim pRows = pDT.Select("CustomerID = " & ReadyString(pDrCustomer("CustomerID")))
            Dim pPeopleName = ""
            Dim pDrNew As DataRow = Nothing

#If DEBUG Then
            If pRows.Count > 1 Then
                Console.WriteLine(pRows.Count)
            End If
#End If

            For Each pRow In pRows
                If String.IsNullOrWhiteSpace(pPeopleName) = False Then
                    pPeopleName &= "; "
                End If
                pPeopleName &= CStr2(pRow("SalesRepName"))

                If pDrNew Is Nothing Then
                    pDrNew = pDtNew.NewRow
                    pDrNew.ItemArray = pRow.ItemArray
                    If pRows.Count > 1 Then
                        pDrNew("PeopleID") = DBNull.Value
                    End If
                End If
            Next
            pDrNew("SalesRepName") = pPeopleName
            pDtNew.Rows.Add(pDrNew)
        Next

        pDtNew.DefaultView.Sort = "SalesRepName, Customer"
        pDtNew = pDtNew.DefaultView.ToTable()

        Return pDtNew

    End Function

    Public Function NewDeviceIDInvoiceNumbers(vFowstID As String, vSourceDeviceID As String, vDestinationDeviceID As String) As DataTable

        Dim pSql = "SELECT MAX(InvoiceNumber) AS InvoiceNumber" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDestinationDeviceID)
        Dim pNextInvoiceNumber = CInt2(mProData.ExecuteScalar(pSql)) + 1
        pSql = "SELECT ID AS InvoiceID" & vbCrLf &
            ",InvoiceNumber" & vbCrLf &
            ",TotalAmount AS InvoiceAmount" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vSourceDeviceID) & vbCrLf &
            " ORDER BY InvoiceDate"
        Dim pDT = mProData.Execute(pSql)
        pDT.Columns.Add("NewInvoiceNumber", GetType(Integer))
        For Each pDR As DataRow In pDT.Rows
            pDR("NewInvoiceNumber") = pNextInvoiceNumber
            pNextInvoiceNumber += 1
        Next

        Return pDT

    End Function

    ''' <summary>
    ''' returns the number of updated rows
    ''' </summary>
    ''' <param name="vDT"></param>
    ''' <returns></returns>
    Public Function ChangeInvoiceNumbers(vDT As DataTable, vNewDeviceID As String, ByRef oError As String) As Integer

        vNewDeviceID = ReadyGuid(vNewDeviceID)
        Dim pOut As Integer
        For Each pDR As DataRow In vDT.Rows
            If IsNumeric(pDR("NewInvoiceNumber")) = False Then
                Continue For
            End If
            Dim pSql = "SELECT TotalAmount" & vbCrLf &
                " ,InvoiceNumber" & vbCrLf &
                " ,InvoiceDate" & vbCrLf &
                " ,DeviceID" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE ID = " & ReadyString(pDR("InvoiceID"))
            Dim pDrInvoice = mProData.ExecuteRow(pSql)
            If pDrInvoice Is Nothing Then
                oError = "InvoiceID, " & ReadyGuid(pDR("InvoiceID")) & ", not found. ref: vbCXS9TNL3VQ!"
                Return pOut
            End If
            Dim pExcelInvoiceAmount = CDec2(pDR("InvoiceAmount"))
            Dim pDatabaseInvoiceAmount = CDec2(pDrInvoice("TotalAmount"))
            Dim pInvoiceAmountDiff = Math.Abs(pExcelInvoiceAmount - pDatabaseInvoiceAmount)
            If pInvoiceAmountDiff > 0.01 Then
                oError = "InvoiceID, " & ReadyGuid(pDR("InvoiceID")) & ", invoice amount difference = " & pInvoiceAmountDiff & ". ref: vbM27MF59TVV!"
                Return pOut
            End If
            If CInt2(pDR("NewInvoiceNumber")) = CInt2(pDrInvoice("InvoiceNumber")) _
                AndAlso ReadyGuid(pDrInvoice("DeviceID")) = vNewDeviceID Then

                Continue For
            End If
            Dim pIU = New InsertUpdate("TransInvoice")
            pIU("InvoiceNumber") = CInt2(pDR("NewInvoiceNumber"))
            pIU("DeviceID") = ReadyString(vNewDeviceID)
            'pIU("PrintCount") = 0
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("InvoiceID")))
            mProData.ExecuteSQL(pSql)
            pOut += 1
        Next

        Return pOut

    End Function

    Public Class CreatePdfOutput
        Public [Error] As String
        Public HasUnapprovedReprintRequests As Boolean
        Public Result As String
        Public BaseFilenames As List(Of String)

        Public Function Serialize() As String

            Dim pOut As New JsonDictionary()
            If String.IsNullOrEmpty([Error]) = False Then
                pOut.Add("Error", [Error])
            End If
            If String.IsNullOrEmpty(HasUnapprovedReprintRequests) = False Then
                pOut.Add("HasUnapprovedReprintRequests", HasUnapprovedReprintRequests)
            End If
            If String.IsNullOrEmpty(Result) = False Then
                pOut.Add("Result", Result)
            End If
            If BaseFilenames IsNot Nothing Then
                pOut.Add("BaseFilenames", BaseFilenames)
            End If

            Return pOut.Serialize()

        End Function
    End Class

    Public Function CreatePdf(vInvoiceID As String, vOrgID As String, vTemplate As String, vAllowReprint As Boolean, vPrintBlindCopy As Boolean,
        vPrintAsDraft As Boolean, vUuid As String,
        vTransType As String,
        Optional vOutputPdfBasePath As String = Nothing) As CreatePdfOutput

        Dim pDS As DataSet
        Dim pReport As ProReport
        Dim pFilename As String
        Dim pHeaderInfo As LibraryOrganization.HeaderInfo
        Dim pDR As DataRow
        Dim pTransTypeProper As TransInvoice.SaveSalesOrderJson.eTransType
        Dim pDT As DataTable
        Dim pDrDocumentControl As DataRow
        Dim pPrintLimit As Integer
        Dim pModuleSource As String
        Dim pDocumentName As String = ""
        Dim pMaxLineItemsPerPage As Integer
        Dim pPageCountPerSet As Integer
        Dim pTemplateFileExist As Boolean
        Dim pBaseFilenames As New List(Of String)
        Dim pDontIncreasePrintCount As Boolean
        Dim pOut As New CreatePdfOutput()

        Try
            mParent.LibraryOrganization.RestoreOrganizationTin()

            TransInvoice.DeleteOldPrintablePdf()
            vUuid = CStr2(vUuid).Replace(";", "_")
            If [Enum].GetNames(GetType(TransInvoice.SaveSalesOrderJson.eTransType)).Contains(vTransType) = False Then
                pOut.Error = "Invalid TransType, " & vTransType & "."
                Return pOut
            End If
            pTransTypeProper = [Enum].Parse(GetType(TransInvoice.SaveSalesOrderJson.eTransType), vTransType)

            If vTemplate.Trim.ToLower() <> "PickList".ToLower() _
                    AndAlso vTemplate.Trim.ToLower() <> "WithdrawalSlip".ToLower() _
                    AndAlso (
                        pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.AccountsPayable _
                        OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.ExpenseTicket _
                        OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder _
                        OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Quote _
                        OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder _
                        OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.StockTransfer
                    ) Then

                ' direct PDF download
            Else
                If String.IsNullOrWhiteSpace(vUuid) Then
                    pOut.Error = "UUID is required."
                    Return pOut
                End If
                If vUuid.Length < 1 Then
                    pOut.Error = "UUID must be at least 1 character in length."
                    Return pOut
                End If
            End If

            pDR = ById(vInvoiceID)
            If pDR Is Nothing Then
                pOut.Error = "Transaction does not exist."
                Return pOut
            Else
                If CStr2(vTemplate).ToLower() = "PosReceipt".ToLower() _
                        AndAlso vAllowReprint Then

                    ' just allow it to reprint
                ElseIf vAllowReprint = False _
                    AndAlso CanPrintInvoice(vInvoiceID, vTemplate) = False Then

                    pOut.Error = "Transaction was already printed."
                    pOut.HasUnapprovedReprintRequests = If(UnapprovedReprintRequests(vInvoiceID).Rows.Count > 0, 1, 0)
                    Return pOut
                End If
                If CStr2(vTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
                    pDocumentName = "Withdrawal Slip"
                    pModuleSource = "Revenue > Invoicing"
                ElseIf CStr2(vTemplate).ToLower() = "PickList".ToLower() Then
                    pDocumentName = "Pick List"
                    pModuleSource = "Revenue > Sales Ordering"
                Else
                    Select Case pTransTypeProper
                        Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
                            If CStr2(vTemplate).ToLower() = "PosReceipt".ToLower() Then
                                pDocumentName = "POS"
                            Else
                                pDocumentName = CStr2(pDR("DocumentName"))
                            End If

                            pModuleSource = "Revenue > Invoicing"
                        Case TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder
                            pModuleSource = "Disbursement > PO"
                        Case TransInvoice.SaveSalesOrderJson.eTransType.Quote
                            pModuleSource = "Revenue > Price Quote"
                        Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
                            pModuleSource = "Revenue > Sales Ordering"
                    End Select
                End If

                pDrDocumentControl = mParent.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, pModuleSource)
                If pDrDocumentControl IsNot Nothing Then
                    pPrintLimit = pDrDocumentControl("PrintLimit")
                    If pPrintLimit = -1 Then
                        pPrintLimit = Integer.MaxValue
                    End If
                End If
                'If CStr2(pTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
                '    If CDec2(pDR("WithdrawalSlipPrintCount")) >= pPrintLimit Then
                '        If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Withdrawal", False) = False Then
                '            Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
                '        End If
                '    End If
                'ElseIf CStr2(pTemplate).ToLower() = "PickList".ToLower() Then
                '    If CDec2(pDR("PickListPrintCount")) >= pPrintLimit Then
                '        If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Pick List", False) = False Then
                '            Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
                '        End If
                '    End If
                'Else
                '    'If CDec2(pDR("PrintCount")) >= pPrintLimit Then
                '    '    If pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Invoice Then
                '    '        If pAllowReprint = False _
                '    '            AndAlso New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Invoice", False) = False Then

                '    '            Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
                '    '        End If
                '    '    Else
                '    '        Return JsonDictionary.Serialize("Error", "Transaction was already printed.")
                '    '    End If
                '    'End If
                'End If
            End If

            If vTemplate.Trim.ToLower() = "PickList".ToLower() Then
                If TransInvoice.IsSavedAsFinal(pDR, True) Then
                    If vPrintAsDraft Then
                        pOut.Error = "You may not print Stock Availability Check because the sales order was already saved as final. Search for this transaction again then click print picking list."
                        Return pOut
                    End If
                Else
                    If vPrintAsDraft Then
                        pDontIncreasePrintCount = True
                    Else
                        pOut.Error = "Only transactions that were saved as final can be printed."
                        Return pOut
                    End If
                End If
            Else
                If TransInvoice.IsSavedAsFinal(pDR, True) = False Then
                    pOut.Error = "Only transactions that were saved as final can be printed."
                    Return pOut
                End If
            End If


            pReport = New ProReport
            pReport.AutoFillHeader = False
            Dim pDocumentNameID As String
            Select Case pTransTypeProper
                Case TransInvoice.SaveSalesOrderJson.eTransType.Quote
                    pReport.SourceFilename = "Quote"

                Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
                    pDocumentName = vTemplate
                    pReport.SourceFilename = "SalesOrder"
                    If String.IsNullOrWhiteSpace(vTemplate) Then
                        pFilename = "SalesOrder"
                    Else
                        If CStr2(vTemplate).ToLower() = "PickList".ToLower() Then
                            If vPrintAsDraft Then
                                pDocumentName = "Stock Availability Check"
                            Else
                                pDocumentName = "Pick List"
                            End If
                        End If
                        pFilename = vTemplate
                        Dim pDrDocumentType = mParent.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, "Revenue > Sales Ordering")
                        If pDrDocumentType IsNot Nothing Then
                            pDocumentNameID = CStr2(pDrDocumentType("ID"))
                        End If
                    End If

                    If pDocumentNameID Is Nothing Then
                        pDocumentNameID = CStr2(pDR("DocumentNameID"))
                    End If
                    If pDocumentNameID = "" Then
                    Else
                        pFilename &= "_" & ReadyGuid(vOrgID) & "_" & ReadyGuid(pDocumentNameID)
                        If IO.File.Exists(cGlobal.AppPath & "Reports\" & pFilename & ".mrt") = False Then
                            'Return JsonDictionary.Serialize("Error", "The print out template file for this invoice type does not exist. Technical note: the expected filename is " & pFilename)
                            If String.IsNullOrWhiteSpace(vTemplate) Then
                                pFilename = "SalesOrder"
                            Else
                                pFilename = vTemplate
                            End If
                        Else
                            pTemplateFileExist = True
                        End If
                    End If
                    pReport.SourceFilename = pFilename
                    pReport.VariablesOptional("DocumentName") = CStr2(pDocumentName).ToUpper()
                    pReport.Report.GetComponentByName("Page1").Page.Watermark.Image = Nothing
                    If pDrDocumentControl IsNot Nothing Then
                        If String.IsNullOrWhiteSpace(CStr2(pDrDocumentControl("PermitNum"))) = False Then
                            Dim pBand As StiComponent = pReport.Report.GetComponentByName("BandPermit")
                            If pBand IsNot Nothing Then
                                pBand.Enabled = True
                            End If
                        End If
                    End If

                Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
                    pFilename = "Invoice"
                    pModuleSource = "Revenue > Invoicing"
                    If CStr2(vTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
                        pDocumentName = "Withdrawal Slip"
                        pModuleSource = "Revenue > Invoicing"
                    ElseIf CStr2(vTemplate).ToLower() = "PickList".ToLower() Then
                        pDocumentName = "Pick List"
                        pModuleSource = "Revenue > Sales Ordering"
                    Else
                        Select Case pTransTypeProper
                            Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
                                If CStr2(vTemplate).ToLower() = "PosReceipt".ToLower() Then
                                    pDocumentName = "POS"
                                Else
                                    pDocumentName = CStr2(pDR("DocumentName"))
                                End If
                                pModuleSource = "Revenue > Invoicing"
                            Case TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder
                                pModuleSource = "Disbursement > PO"
                            Case TransInvoice.SaveSalesOrderJson.eTransType.Quote
                                pModuleSource = "Revenue > Price Quote"
                            Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
                                pModuleSource = "Revenue > Sales Ordering"
                        End Select
                    End If
                    'If String.IsNullOrWhiteSpace(pTemplate) = False Then ' removed this condition
                    Dim pDrDocumentType = mParent.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, pModuleSource)
                    If pDrDocumentType IsNot Nothing Then
                        pDocumentNameID = CStr2(pDrDocumentType("ID"))
                        pMaxLineItemsPerPage = CInt2(pDrDocumentControl("MaxLineItemsPerPage"))
                    End If
                    'End If

                    If pDocumentNameID Is Nothing Then
                        pDocumentNameID = CStr2(pDR("DocumentNameID"))
                    End If
                    If pDocumentNameID = "" Then
                        If pDocumentName = "POS" Then
                            pFilename = "POSReceipt"
                        End If
                    Else
                        pFilename &= "_" & ReadyGuid(vOrgID) & "_" & ReadyGuid(pDocumentNameID)
                        If IO.File.Exists(cGlobal.AppPath & "Reports\" & pFilename & ".mrt") = False Then
                            'Return JsonDictionary.Serialize("Error", "The print out template file for this invoice type does not exist. Technical note: the expected filename is " & pFilename)
                            If String.IsNullOrWhiteSpace(vTemplate) Then
                                pFilename = "Invoice"
                            Else
                                pFilename = vTemplate
                            End If
                        Else
                            pTemplateFileExist = True

                            If vPrintBlindCopy And Not String.IsNullOrWhiteSpace(vTemplate) Then
                                pFilename = vTemplate
                            End If
                        End If
                    End If
                    pReport.SourceFilename = pFilename
                    pReport.VariablesOptional("DocumentName") = pDocumentName.ToUpper()
                    pReport.Report.GetComponentByName("Page1").Page.Watermark.Image = Nothing
                    If pDrDocumentControl IsNot Nothing Then
                        If String.IsNullOrWhiteSpace(CStr2(pDrDocumentControl("PermitNum"))) = False Then
                            Dim pBand As StiComponent = pReport.Report.GetComponentByName("BandPermit")
                            If pBand IsNot Nothing Then
                                pBand.Enabled = True
                            End If
                        End If
                    End If

                Case TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder
                    'pReport.SourceFilename = "PurchaseOrder" ' comment this 2021_09_13__13_32_05_8150
                    pDocumentName = vTemplate
                    If String.IsNullOrWhiteSpace(vTemplate) Then
                        pFilename = "Purchase Order"
                    Else
                        pFilename = vTemplate
                        Dim pDrDocumentType = mParent.LibraryDocumentControl.GetByDocumentNameAndModuleSource(pDocumentName, "Disbursement > PO")
                        If pDrDocumentType IsNot Nothing Then
                            pDocumentNameID = CStr2(pDrDocumentType("ID"))
                        End If
                    End If

                    If pDocumentNameID Is Nothing Then
                        pDocumentNameID = CStr2(pDR("DocumentNameID"))
                    End If
                    If pDocumentNameID = "" Then
                    Else
                        pFilename &= "_" & ReadyGuid(vOrgID) & "_" & ReadyGuid(pDocumentNameID)
                        If IO.File.Exists(cGlobal.AppPath & "Reports\" & pFilename & ".mrt") = False Then
                            'Return JsonDictionary.Serialize("Error", "The print out template file for this invoice type does not exist. Technical note: the expected filename is " & pFilename)
                            If String.IsNullOrWhiteSpace(vTemplate) Then
                                pFilename = "Purchase Order"
                            Else
                                pFilename = vTemplate
                            End If
                        End If
                    End If
                    pReport.SourceFilename = pFilename
                    pReport.VariablesOptional("DocumentName") = CStr2(pDocumentName).ToUpper()
                    pReport.Report.GetComponentByName("Page1").Page.Watermark.Image = Nothing
                    If pDrDocumentControl IsNot Nothing Then
                        If String.IsNullOrWhiteSpace(CStr2(pDrDocumentControl("PermitNum"))) = False Then
                            Dim pBand As StiComponent = pReport.Report.GetComponentByName("BandPermit")
                            If pBand IsNot Nothing Then
                                pBand.Enabled = True
                            End If
                        End If
                    End If

                Case Else
                    Throw New NotImplementedException()

            End Select

            If CStr2(vTemplate).ToLower() = "PosReceipt".ToLower() _
                    AndAlso pTemplateFileExist = False Then

                Dim pDeviceID As String
                Dim pDrInvoice = ById(vInvoiceID)
                If pDrInvoice IsNot Nothing Then
                    pDeviceID = CStr2(pDrInvoice("DeviceID"))
                End If

                Dim clsSalesPayments As New SalesPayments
                Dim dtaTable = clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(vInvoiceID, mParent.ProData)
                If (dtaTable.Rows.Count > 0) Then
                    'strBody = strBody.Replace("[CustomerNameHere]", "Customer : " & dtaTable.Rows(0).Item("CustomerName").ToString().Trim())
                    'strBody = strBody.Replace("[CustomerAddressHere]", "Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
                    'strBody = strBody.Replace("[CustomerTINHere]", "TIN : " & dtaTable.Rows(0).Item("CustomerTIN").ToString().Trim())

                    'strBody = strBody.Replace("[CustomerBillingAddress]", "Billing Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
                    'strBody = strBody.Replace("[CustomerDeliveryAddress]", "Delivery Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())

                    'strInvoiceNo = Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim()

                    'strBody = strBody.Replace("[InvoiceNoHere]", "Sales Invoice No. " & Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim())
                    'strBody = strBody.Replace("[InvoiceDateTimeHere]", "Date/Time : " & Convert.ToDateTime(dtaTable.Rows(0).Item("InvoiceDate")).ToString("MMM dd, yyyy HH:mm").Trim())
                    'strBody = strBody.Replace("[CashierNameHere]", "Cashier : " & dtaTable.Rows(0).Item("CashierName").ToString().Trim())
                End If
                pReport.AddTable("InvoiceHeader", dtaTable)

                Dim FowstID As String = CStr2(dtaTable.Rows(0).Item("OrganizationBranchID"))
                Dim POSAccredNo = mParent.SystemDefaults.GetSystemDefaultPublic("Mobiz POS Accreditation Number")

                dtaTable = clsSalesPayments.getReceiptHeaderInfo(FowstID.ToString().Trim(), mParent.ProData, POSAccredNo, pDeviceID)
                'If (dtaTable.Rows.Count > 0) Then
                '    'additional by janine. to show in email receipt
                '    If (dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim() <> "") Then
                '        varBusinessName = dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim()
                '    Else
                '        varBusinessName = dtaTable.Rows(0).Item("CorporateName").ToString().Trim()
                '    End If

                '    strBody = strBody.Replace("[TradeNameHere]", dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim())
                '    strBody = strBody.Replace("[CorporateNameHere]", dtaTable.Rows(0).Item("CorporateName").ToString().Trim())
                '    strBody = strBody.Replace("[TINHere]", "TIN " & dtaTable.Rows(0).Item("TIN").ToString().Trim())
                '    strBody = strBody.Replace("[AddressHere]", dtaTable.Rows(0).Item("CompleteAddress").ToString().Trim())
                '    strBody = strBody.Replace("[TelNoHere]", "Tel. No. " & dtaTable.Rows(0).Item("TelNo").ToString().Trim())

                '    strBody = strBody.Replace("[POSAccreditationNo]", "POS Accreditation No. " & dtaTable.Rows(0).Item("POSAccreditationNo").ToString().Trim())
                '    strBody = strBody.Replace("[POSRegistrationNo]", "POS Registration No. " & dtaTable.Rows(0).Item("POSPermitNo").ToString().Trim())
                '    strBody = strBody.Replace("[POSMachineNo]", "Machine ID No. (MIN) " & dtaTable.Rows(0).Item("MIN").ToString().Trim())

                'End If
                pReport.AddTable("ReceiptHeader", dtaTable)

                '''Get Payment Items
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesPaymentReceiptBodyInfo(vInvoiceID, mParent.ProData)
                If (dtaTable.Rows.Count > 0) Then
                    'intCntr = 0
                    'While (intCntr < dtaTable.Rows.Count)

                    '    strTemp = strTemp & "<tr>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 35%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("StockDescription").ToString().Trim() & "</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 10%; height:30px; border: 1px dotted Gray;"">" & Convert.ToInt32(dtaTable.Rows(intCntr).Item("Quantity")).ToString("#,##0").Trim() & "</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 15%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("UnitMeasure").ToString().Trim() & "</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("UnitPrice")).ToString("#,##0.00").Trim() & "</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
                    '    strTemp = strTemp & "</tr>"

                    '    intCntr = intCntr + 1

                    'End While

                    'strTemp = strTemp & "</tbody>"

                    'strBody = strBody.Replace("[BodyGoesHere]", strTemp)
                End If
                pReport.AddTable("ReceiptBody", dtaTable)

                '''Get Payment Amount
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesPaymentReceiptFooterAmountsInfo(vInvoiceID, mParent.ProData)
                If (dtaTable.Rows.Count > 0) Then
                    ' '''SKU and Amount due
                    'strTemp = "<table style=""width:100%"">"
                    'strTemp = strTemp & "<thead>"
                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">SKU Count</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToInt32(dtaTable.Rows(0).Item("SKUCount")).ToString("#,##0").Trim() & "</td>"
                    'strTemp = strTemp & "</tr>"

                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Sub Total</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
                    'strTemp = strTemp & "</tr>"

                    '' additional by janine feb 11 2015 
                    'If (IsSeniorCitizenDiscount(vInvoiceID)) Then
                    '    strTemp = strTemp & "<tr>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Less: " & dtaTable.Rows(0).Item("TaxPercent") & "% VAT</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("SubTotal")) - CDec2(dtaTable.Rows(0).Item("TotalAmountDue"))).ToString("#,##0.00").Trim() & "</td>"
                    '    strTemp = strTemp & "</tr>"

                    '    strTemp = strTemp & "<tr>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Total Amount</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TotalAmountDue")).ToString("#,##0.00").Trim() & "</td>"
                    '    strTemp = strTemp & "</tr>"

                    'End If


                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Discount Amount</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("DiscountAmount")).ToString("#,##0.00").Trim() & "</td>"
                    'strTemp = strTemp & "</tr>"

                    '' additional by janine feb 11 2015 
                    'If (mParent.SystemDefaults.IsEnableBillingOption = "1") Then
                    '    strTemp = strTemp & "<tr>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Charges</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("ChargeAmount")).ToString("#,##0.00").Trim() & "</td>"
                    '    strTemp = strTemp & "</tr>"

                    '    strTemp = strTemp & "<tr>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    '    strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Tip</td>"
                    '    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TipAmount")).ToString("#,##0.00").Trim() & "</td>"
                    '    strTemp = strTemp & "</tr>"

                    'End If

                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Net Amount Due</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("NetAmountDue")) + CDec2(dtaTable.Rows(0).Item("ChargeAmount")) + CDec2(dtaTable.Rows(0).Item("TipAmount"))).ToString("#,##0.00").Trim() & "</td>"
                    'strTemp = strTemp & "</tr>"

                    'strTemp = strTemp & "</thead>"
                    'strTemp = strTemp & "</table>"

                    'strTemp = strTemp & "<br />"

                    ' '''VAT Info
                    'strTemp = strTemp & "<table style=""width:100%"">"
                    'strTemp = strTemp & "<thead>"
                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT-able Sales</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxSales")).ToString("#,##0.00").Trim() & "</td>"
                    'strTemp = strTemp & "</tr>"

                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Exempt Sales</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
                    'strTemp = strTemp & "</tr>"

                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Zero-Rated Sales</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
                    'strTemp = strTemp & "</tr>"

                    'strTemp = strTemp & "<tr>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                    'strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Amount</td>"
                    'strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxAmount")).ToString("#,##0.00").Trim() & "</td>"
                    'strTemp = strTemp & "</tr>"

                    'strTemp = strTemp & "</thead>"
                    'strTemp = strTemp & "</table>"

                    'strBody = strBody.Replace("[FooterGoesHere]", strTemp)

                End If
                pReport.AddTable("ReceiptFooter", dtaTable)

                '''Get Payment Details
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesPaymentReceiptPaymentInfo(vInvoiceID, mParent.ProData)
                If (dtaTable.Rows.Count > 0) Then
                    'strBody = strBody.Replace("[CASHPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CashPayment")) <= 0, "", "Cash        : " & Convert.ToDouble(dtaTable.Rows(0).Item("CashPayment")).ToString("#,##0.00").Trim()))
                    'strBody = strBody.Replace("[CHECKPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CheckPaymentAmount")) <= 0, "", "Check       : " & Convert.ToDouble(dtaTable.Rows(0).Item("CheckPaymentAmount")).ToString("#,##0.00").Trim()))
                    'strBody = strBody.Replace("[CREDITCARDPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CreditCardAmount")) <= 0, "", "Credit Card : " & Convert.ToDouble(dtaTable.Rows(0).Item("CreditCardAmount")).ToString("#,##0.00").Trim()))
                    'strBody = strBody.Replace("[DEBITCARDPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("DebitCardAmount")) <= 0, "", "Debit Card : " & Convert.ToDouble(dtaTable.Rows(0).Item("DebitCardAmount")).ToString("#,##0.00").Trim()))
                    'strBody = strBody.Replace("[SRVPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("SRVAmount")) <= 0, "", "SRV         : " & Convert.ToDouble(dtaTable.Rows(0).Item("SRVAmount")).ToString("#,##0.00").Trim()))
                    'strBody = strBody.Replace("[CHANGEAMOUNT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CustomerChangeCash")) <= 0, "", "Change      : " & Convert.ToDouble(dtaTable.Rows(0).Item("CustomerChangeCash")).ToString("#,##0.00").Trim()))
                End If
                pReport.AddTable("ReceiptPayment", dtaTable)

                Dim pDtSalesReps = GetSalesReps(vInvoiceID)
                pReport.AddTable("SalesRep", pDtSalesReps)

                pReport.ExportXmlFile()
                pFilename = CreateValidFilename(vUuid) & ";" & pDocumentNameID & ";" & vOrgID & ";" & Format(1, "00") & "-" & DateTimeStamp() & ".pdf"
                pBaseFilenames.Add(pFilename)
                pFilename = Common.ServerPath & "temp\PrintablePdf\" & pFilename
                CreateFolder(pFilename)

                'pReport.Report.Pages(0).PaperSize = Printing.PaperKind.Custom
                'Dim pMarginTop = pReport.Report.Pages(0).Margins.Top
                'Dim pMarginBottom = pReport.Report.Pages(0).Margins.Bottom
                'pReport.Report.Pages(0).Margins.Top = 0
                'pReport.Report.Pages(0).Margins.Bottom = 0
                'pReport.Report.Pages(0).Height = 1
                'pReport.Render(True)

                'Dim pClientHeight = pReport.Report.Pages(0).Height * pReport.Report.RenderedPages.Count + pMarginTop + pMarginBottom
                'pReport.Report.Pages(0).Height = pClientHeight
                'pReport.Report.Pages(0).Margins.Top = pMarginTop
                'pReport.Report.Pages(0).Margins.Bottom = pMarginBottom

                'pReport.Render(True)
                'Do
                '    AppendToLog("vb6ECZJMSWWN! pages: " & pReport.Report.RenderedPages.Count & "; height=" & pReport.Report.RenderedPages.Item(0).Height & "; pFilename=" & pFilename)
                '    If pReport.Report.RenderedPages.Count = 1 _
                '        AndAlso pReport.Report.RenderedPages.Item(0).Height > 3 Then

                '        pReport.Report.Pages(0).Height -= 1
                '        pReport.Render(True)
                '    Else
                '        pReport.Report.Pages(0).Height += 1
                '        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                '        Exit Do
                '    End If
                'Loop
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
            Else
                pDS = GetReportData(" WHERE TransInvoice.ID = " & ReadyString(vInvoiceID), False)
                pDT = pDS.Tables("Stocks")
                Dim pVatable As Decimal
                Dim pVat As Decimal
                For Each pDR In pDT.Rows
                    pVat += CDec2(pDR("TaxAmount"))
                    pVatable += CDec2(pDR("TaxSales"))
                Next pDR

                pDT = pDS.Tables("Invoices")
                pDT.Columns.Add("CustomerContactInfos")
                pDT.Columns.Add("TotalVatable", GetType(Decimal))
                pDT.Columns.Add("TotalVatAmount", GetType(Decimal))
                For Each pDR In pDT.Rows
                    pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(CStr2(pDR("CustomerID")))
                    pDR("CustomerContactInfos") = CStr2(pHeaderInfo.ContactInfos)
                    pDR("TotalVatAmount") = pVat
                    pDR("TotalVatable") = pVatable
                Next pDR

                If pDT.Rows.Count > 0 Then
                    pReport.Variables("CompanyAddress") = CStr2(pDT.Rows(0)("OrgFowstName")) & " - " & CStr2(pDT.Rows(0)("OrgFowst"))
                    Dim pDrBackorder = GetByBackorderSourceInvoiceID(CStr2(pDT.Rows(0)("InvoiceID")))
                    If pDrBackorder IsNot Nothing Then
                        Dim pDsBackorder = GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pDrBackorder("ID")), False)
                        pReport.AddTable("BackorderStocks", pDsBackorder.Tables("Stocks"))
                    End If

                    If pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder Then
                        If CInt2(pDS.Tables("Invoices").Rows(0)("PurchaseOrderNo")) > 0 Then
                            pReport.VariablesOptional("Title") = "PURCHASE ORDER"
                            pReport.VariablesOptional("FullTransNum") = CStr2(pDS.Tables("Invoices").Rows(0)("PrintedPurchaseOrderNumber"))
                        Else
                            pReport.VariablesOptional("Title") = "FOR QUOTATION"
                            pReport.VariablesOptional("FullTransNum") = "DRAFT"
                        End If
                    End If
                End If

                pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(vOrgID)
                AppendToDebug("vbC3HR5EVPEU! pHeaderInfo.CompanyName: " & pHeaderInfo.CompanyName, Now.Date = #1/4/2017#)
                pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
                'pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress ' removed 2023_08_09__11_48_11_1969
                pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos
                pReport.VariablesOptional("Footnote") = CStr2(pDrDocumentControl("Footnote"))

                pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(CStr2(pDS.Tables("Invoices").Rows(0)("CustomerID")))
                pReport.Variables("CustomerName") = pHeaderInfo.CompanyName
                pReport.Variables("CustomerAddress") = pHeaderInfo.CompanyAddress
                pReport.Variables("CustomerContactInfos") = pHeaderInfo.ContactInfos

                If pHeaderInfo.VatType?.ToLower() = "Zero-Rated Sales/Receipts".ToLower() _
                    OrElse pHeaderInfo.VatType?.ToLower() = "Non VATable".ToLower() _
                    OrElse pHeaderInfo.VatType?.ToLower() = "Exempt Sales/Receipts".ToLower() Then

                    Dim pBand As StiComponent = pReport.Report.GetComponentByName("InvoiceVatFooter")
                    If pBand IsNot Nothing Then
                        pBand.Enabled = False
                    End If
                End If

                pReport.AddTable("Invoices", pDS.Tables("Invoices"))
                Dim pDtStocksAll = pDS.Tables("Stocks")
                Dim pDtStocks As DataTable
                Dim pPageMax As Integer
                If pMaxLineItemsPerPage > 0 Then
                    pPageMax = Math.Ceiling(pDtStocksAll.Rows.Count / pMaxLineItemsPerPage)
                Else
                    pPageMax = 1
                    pDtStocks = pDtStocksAll
                End If
                Dim pCopies = CInt2(pDrDocumentControl("CopiesPerPrint"))
                Dim pMaxNoOfSupplementalBlindCopies = CInt2(pDrDocumentControl("MaxNoOfSupplementalBlindCopies"))
                If vPrintBlindCopy Then
                    If pMaxNoOfSupplementalBlindCopies <> 0 Then
                        pCopies = pMaxNoOfSupplementalBlindCopies
                    Else
                        pOut.Error = "Supplemental blind copy is not allowed to be printed."
                        Return pOut
                    End If
                End If
                For p As Integer = 1 To pCopies
                    If p > 1 Then
                        Dim pBand = pReport.Report.GetComponentByName("HeaderBand_of_DatarptTransInvoiceBackOrders")
                        If pBand IsNot Nothing Then
                            pBand.Enabled = False
                        End If

                        pBand = pReport.Report.GetComponentByName("ChildBand_of_HeaderBand_of_DatarptTransInvoiceBackOrders")
                        If pBand IsNot Nothing Then
                            pBand.Enabled = False
                        End If

                        pBand = pReport.Report.GetComponentByName("DatarptTransInvoiceBackOrders")
                        If pBand IsNot Nothing Then
                            pBand.Enabled = False
                        End If

                        pBand = pReport.Report.GetComponentByName("FooteBand_of_DatarptTransInvoiceBackOrders")
                        If pBand IsNot Nothing Then
                            pBand.Enabled = False
                        End If
                    End If
                    For pPageCurrent As Integer = 1 To pPageMax
                        If pMaxLineItemsPerPage > 0 Then
                            pDtStocks = pDtStocksAll.Clone()
                            Dim pRowIndexStart = (pPageCurrent - 1) * pMaxLineItemsPerPage
                            Dim pRowIndexEnd = pPageCurrent * pMaxLineItemsPerPage - 1
                            pRowIndexEnd = Math.Min(pRowIndexEnd, pDtStocksAll.Rows.Count - 1)
                            For pRowIndex As Integer = pRowIndexStart To pRowIndexEnd
                                pDtStocks.ImportRow(pDtStocksAll.Rows(pRowIndex))
                            Next
                        End If
                        pReport.AddTable("Stocks", pDtStocks)
                        pReport.ExportXmlFile()


                        Dim pLabel As String = CStr2(pDrDocumentControl("LabelOfCopy" & Format(p, "00")))
                        pReport.VariablesOptional("LabelOfCopy") = pLabel
                        If vTemplate.Trim.ToLower() <> "PickList".ToLower() _
                                AndAlso vTemplate.Trim.ToLower() <> "WithdrawalSlip".ToLower() _
                                AndAlso (
                                    pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.AccountsPayable _
                                    OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.ExpenseTicket _
                                    OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder _
                                    OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Quote _
                                    OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder _
                                    OrElse pTransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.StockTransfer
                                ) Then

                            pFilename = Format(p, "00") & "-" & DateTimeStamp() & ".pdf"
                            pBaseFilenames.Add(pFilename)
                        Else
                            pFilename = CreateValidFilename(vUuid) & ";" & pDocumentNameID & ";" & vOrgID & ";" & Format(p, "00") & "-" & DateTimeStamp() & ".pdf"
                        End If
                        If String.IsNullOrWhiteSpace(vOutputPdfBasePath) Then
                            pFilename = Common.ServerPath & "temp\PrintablePdf\" & pFilename
                        Else
                            pFilename = vOutputPdfBasePath & "-" & pLabel & ".pdf"
                        End If
                        CreateFolder(pFilename)
                        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                        pPageCountPerSet = pReport.Report.RenderedPages.Count
                    Next pPageCurrent
                Next p
            End If

            If CStr2(vTemplate).ToLower() = "WithdrawalSlip".ToLower() Then
                IncrementWithdrawalSlipPrintCount(vInvoiceID)
            ElseIf CStr2(vTemplate).ToLower() = "PickList".ToLower() Then
                If pDontIncreasePrintCount = False Then
                    IncrementPickListPrintCount(vInvoiceID)
                End If
            Else
                IncrementPrintCount(vInvoiceID)
            End If

            TagTransAsFollowUp(vInvoiceID)

            pOut.Result = "ok"
            pOut.BaseFilenames = pBaseFilenames
            Return pOut
        Catch ex As Exception
            Dim pMsg = ex.Message & vbCrLf &
                "OrgID: " & vOrgID & vbCrLf &
                "InvoiceID: " & vInvoiceID &
                "Template: " & vTemplate
            If pReport IsNot Nothing Then
                pMsg &= vbCrLf & "mrt filename:" & pReport.SourceFilename
            End If
            AppendToLog(pMsg,, "CreatePdf")
            Throw
        End Try

    End Function

    Public Class CreateInvoicesPdfsOutput
        Public InputCount As Integer
        Public OutputCount As Integer
        Public TimeStamp As String
    End Class

    Public Function CreateInvoicesPdfs(vInvoiceNumbers As String, vOrgID As String) As CreateInvoicesPdfsOutput

        Dim pOut As New CreateInvoicesPdfsOutput
        Dim pSplits = vInvoiceNumbers.Split(New String() {vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim pTimeStamp = DateTimeStamp()
        Dim pBasePath = Common.ServerPath & "temp\MassInvoicePdfs\" & pTimeStamp & "\"
        pOut.TimeStamp = pTimeStamp

        pOut.InputCount = pSplits.Length
        For Each pSplit In pSplits
            Dim pDR = ByFullNumber(pSplit)
            If pDR Is Nothing Then
                AppendToLog("vb38TEKQQFGV! ByFullNumber not found: " & pSplit)
                Continue For
            End If

            Dim pOutputPdfBasePath = pBasePath & CreateValidFilename(Format(pDR("InvoiceDate"), "MM-dd-yyyy") & " " & pDR("FullNumber") & " " & CStr2(pDR("CustomerName")))
            Dim pPdfResult = CreatePdf(vInvoiceID:=CStr2(pDR("ID")), vOrgID:=vOrgID,
                vTemplate:="",
                vAllowReprint:=True,
                vPrintBlindCopy:=False,
                vPrintAsDraft:=False,
                vUuid:="x",
                vTransType:="Invoice",
                vOutputPdfBasePath:=pOutputPdfBasePath)
            If String.IsNullOrWhiteSpace(pPdfResult.Error) = False Then
                AppendToLog("vbVPAJXW2CV3! " & Format(pDR("InvoiceDate"), "MM-dd-yyyy") & " " & pDR("FullNumber") & vbCrLf & pPdfResult.Error)
            End If
            If pPdfResult.Result = "ok" Then
                pOut.OutputCount += 1
            End If
        Next

        If pOut.OutputCount = 0 Then
            Return pOut
        End If

        Dim pFilenames = IO.Directory.GetFiles(pBasePath, "*.pdf")
        'Dim pOutpudPdf As New PdfSharp.Pdf.PdfDocument()
        'For Each pFilename In pFilenames
        '    Using pSourcePdf = PdfSharp.Pdf.IO.PdfReader.Open(pFilename, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)
        '        For pPage As Integer = 0 To pSourcePdf.PageCount - 1
        '            pOutpudPdf.AddPage(pSourcePdf.Pages(pPage))
        '        Next
        '        pSourcePdf.Close()
        '    End Using
        'Next
        'pOutpudPdf.Save(pBasePath & "Merged_" & pTimeStamp & ".pdf")

        ' uncomment if u want separate pdfs in a zip file
        'Dim pZipFilename = pBasePath & "MassInvoicePdfs.zip"
        'Dim pZip = New ProZip()
        'Dim pFilenames = IO.Directory.GetFiles(pBasePath, "*.pdf")
        'For Each pFilename In pFilenames
        '    pZip.AddFile(pFilename)
        'Next
        'pZip.Save(pZipFilename)

        Return pOut

    End Function

    Public Function GetNetInvoiceBalance(vInvoiceID As String) As Decimal

        Dim pSql = "SELECT NetInvoiceBalance FROM CacheInvoices" &
            " WHERE ID = " & ReadyString(vInvoiceID)
        Dim pDrCacheInvoice = mProData.ExecuteRow(pSql)
        If pDrCacheInvoice Is Nothing Then
            Return 0
        End If
        Dim pNetInvoiceBalance = CDec2(pDrCacheInvoice("NetInvoiceBalance"))

        Return pNetInvoiceBalance

    End Function

    Public Function ImportMallLessor(vZipFilename As String, vOrgID As String, ByRef oError As String) As Integer

        If File.Exists(vZipFilename) = False Then
            Return "Zip file does not exist. ref: vb4QRQMCLV6C!"
        End If

        Dim pZip = New ProZip()
        pZip.Open(vZipFilename)
        Dim pDtInternalFilenames = pZip.GetFilenames()
        For Each pDrInternalFilename As DataRow In pDtInternalFilenames.Rows
            If pDrInternalFilename("Extension") = "txt" Then
                Dim pFilenameNoExtension = IO.Path.GetFileNameWithoutExtension(pDrInternalFilename("Name"))
                If pFilenameNoExtension.EndsWith("H") Then
                    Dim pFileText = pZip.GetString(pDrInternalFilename("FullName"))
                    Dim pRows = pFileText.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                    For Each pRow In pRows
                        Dim pCells = pRow.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
                        Dim pDate = ParseDate(pCells(0))
                        If pDate Is Nothing Then
                            Continue For
                        End If
                        Dim pTime = ParseDate(pCells(1))
                        If pTime Is Nothing Then
                            Continue For
                        End If
                        pDate = pDate.Value.AddHours(pTime.Value.Hour)
                        Dim pAmount = CDec2(pCells(2))
                        If pAmount = 0 Then
                            Continue For
                        End If
                        CreateMallLessorInvoice(pDate, pAmount, vOrgID, oError)
                    Next
                End If
            End If
        Next

    End Function

    Public Function CreateMallLessorInvoice(vInvoiceDate As Date, vAmount As Decimal, vOrgID As String, ByRef oError As String) As String

        oError = Nothing

        Dim pCustomerID = mParent.LibraryOrganization.GetOrAddByCompanyName("Mall Lessor Customer", "", True, False)
        Dim pFowst = mParent.LibraryOrganizationFOWST.GetFirstByOrgID(vOrgID)
        If pFowst Is Nothing Then
            oError = "No fowst. vbJBVB9TSYT5!"
            Return Nothing
        End If
        Dim pFowstID = CStr2(pFowst("ID"))
        Dim pAddress = mParent.LibraryOrganizationFOWST.GetFirstByOrgID(vOrgID)
        If pAddress Is Nothing Then
            oError = "No address. vbT7LV9C4CNM!"
            Return Nothing
        End If
        Dim pAddressID = CStr2(pAddress("ID"))
        Dim pProductID = mParent.LibraryStock.GetOrCreateMallLessorProduct()

        Dim pDocumentType = mParent.LibraryDocumentControl.GetByDocumentNameAndModuleSource("POS", "Revenue > Invoicing")
        If pDocumentType Is Nothing Then
            oError = "POS Document Type does not exist. vbM27C53LX64!"
            Return Nothing
        End If
        Dim pDocumentTypeID = CStr2(pDocumentType("ID"))

        Dim pDtStocks As DataTable = TransInvoice.CreateStockTable()
        Dim pDrStock As DataRow = pDtStocks.NewRow
        pDrStock("StockID") = pProductID
        pDrStock("Quantity") = 1
        pDrStock("ActualQuantity") = 1
        pDrStock("UnitMeasure") = "Unit"
        pDrStock("UnitPrice") = vAmount
        pDrStock("PriceLevelID") = mParent.SystemDefaults.GetRetailPriceID()
        pDrStock("Subtotal") = vAmount
        pDrStock("Cost") = vAmount
        pDtStocks.Rows.Add(pDrStock)

        Dim pInput = New SaveSalesOrderJson
        pInput.TransType = "Invoice"
        pInput.CustomerID = pCustomerID
        pInput.FowstID = pFowstID
        pInput.DeviceID = CStr2(mParent.LibraryDevice.GetOrCreateDesktop()("ID"))
        pInput.UserID = modClient.EmployeeWebGuidID
        pInput.BillingAddressID = pAddressID
        pInput.DeliveryAddressID = pAddressID
        pInput.DateNeeded = Now
        pInput.DocumentNameID = pDocumentTypeID
        pInput.Verify(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return Nothing
        End If
        mProData.BeginTransaction()
        Dim pInvoiceID = Save(pInput, vOrgID, oError)
        SaveStocks(pInvoiceID, pDtStocks)
        UpdateAmounts(pInvoiceID)

        pInput.SaveAndFinalize = True
        Save(pInput, vOrgID, oError)
        If String.IsNullOrWhiteSpace(pInvoiceID) = False _
            AndAlso String.IsNullOrWhiteSpace(oError) Then

            AddToInventoryLedger(pInvoiceID, vOrgID)
        End If
        mProData.CommitTransaction()

        Return Nothing

    End Function

    Public Function UnflushedData() As (Count As Integer, MaxFileSize As Integer)

        Dim pSql = "SELECT COUNT(*) AS Count" & vbCrLf &
            " ,MAX(FileSize) AS MaxFileSize" & vbCrLf &
            " FROM TransImports" & vbCrLf &
            " WHERE ProcessEnd IS NULL"
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return (0, 0)
        End If

        Return (CInt2(pDR("Count")), CInt2(pDR("MaxFileSize")))

    End Function

    Public Function AddAdjustmentFromSalesReturn(vInvoiceID As String, ByRef oError As String) As Boolean

        Dim pSql = " -- AddAdjustmentFromSalesReturn" & vbCrLf &
            " SELECT tblAdjustment.ID" & vbCrLf &
            ",TransSalesReturnLedger.SourceMemoID" & vbCrLf &
            " ,TransSalesReturnLedger.LastUpdated" & vbCrLf &
            " ,TransSalesReturnLedger.UserID" & vbCrLf &
            " ,TransSalesReturnLedger.GrossRefundAmount + TransSalesReturnLedger.GrossSrvAmount AS Amount" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " LEFT JOIN (" & vbCrLf &
            " 	SELECT TransInvoiceAdjustments.ID" & vbCrLf &
            "     ,TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            "     ,TransInvoiceAdjustments.MemoID" & vbCrLf &
            "     FROM TransInvoiceAdjustments" & vbCrLf &
            "     JOIN LibraryAccountAdjustments ON LibraryAccountAdjustments.ID = TransInvoiceAdjustments.ReasonID" & vbCrLf &
            "     And LibraryAccountAdjustments.Reason = 'Sales Returns'" & vbCrLf &
            " ) tblAdjustment ON tblAdjustment.InvoiceID = TransSalesReturnLedger.SourceInvoiceID" & vbCrLf &
            " AND tblAdjustment.MemoID = TransSalesReturnLedger.SourceMemoID" & vbCrLf &
            " WHERE TransSalesReturnLedger.SourceInvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oError = "No record found. vbXX7U4SVS86!"
            Return False
        End If
        If pDR("ID") IsNot DBNull.Value Then
            oError = "This sales return already had an adjustment. vb66JP7PXGDZ!"
            Return False
        End If

        Dim pDrReason = mParent.LibraryAccountAdjustments.GetByReason("Sales Returns")
        If pDrReason Is Nothing Then
            oError = "No account adjustment reason for Sales Returns. ref: vb9EYJLZLPSE!"
            Return False
        End If
        Dim pSaveAdjustmentJson As New TransInvoice.SaveAdjustmentJson
        pSaveAdjustmentJson.ReasonID = CStr2(pDrReason("ID"))
        pSaveAdjustmentJson.Amount = CDec2(pDR("Amount"))
        pSaveAdjustmentJson.InvoiceID = vInvoiceID
        pSaveAdjustmentJson.Remarks = "Made from POS. vb7GPH3CGM2P!"
        pSaveAdjustmentJson.IsNew = True
        pSaveAdjustmentJson.UserID = CStr2(pDR("UserID"))
        pSaveAdjustmentJson.MemoID = CStr2(pDR("SourceMemoID"))

        mProData.BeginTransaction() ' for debugging only
        mParent.TransInvoice.SaveAdjustment(pSaveAdjustmentJson, oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            mProData.RollbackTransaction(True, True)
            Return False
        End If

        ' check again if exist
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            mProData.RollbackTransaction(True, True)
            oError = "No record found for the second time. vb7C9ZSCL7DU!"
            Return False
        End If
        If pDR("ID") Is DBNull.Value Then
            mProData.RollbackTransaction(True, True)
            oError = "Still no corresponding adjustment after adding. vbZ6ZXB2JQ7H"
            Return False
        End If
        mProData.CommitTransaction()

        Return True
    End Function

    Public Function GetThirdPartyDailySales(
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As DataTable

        'mProData.CommandTimeout = 10 * 60
        Dim pDtOut As DataTable
        Dim pDtDatabases = mParent.LibraryOrganization.GetAll(True)
        For Each pDrDatabase As DataRow In pDtDatabases.Rows
            Dim pDT = GetThirdPartyDailySales(CStr2(pDrDatabase("Database")), CStr2(pDrDatabase("CompanyName")), vInvoiceDateFrom, vInvoiceDateTo)
            If pDT Is Nothing Then
                Continue For
            End If
            If pDtOut Is Nothing Then
                pDtOut = pDT
            Else
                pDtOut.Merge(pDT)
            End If
        Next

        pDtOut.DefaultView.Sort = "InvoiceDate, OrganizationName"
        pDtOut = pDtOut.DefaultView.ToTable()
        Return pDtOut

    End Function

    Public Function GetThirdPartyDailySales(
        vDatabaseName As String,
        vOrganizationName As String,
        Optional vInvoiceDateFrom As Date? = Nothing,
        Optional vInvoiceDateTo As Date? = Nothing) As DataTable

        Dim pSql = "USE " & EscapeSqlName(vDatabaseName)
        mProData.ExecuteSQL(pSql)

        Dim pDrDevice = mParent.LibraryDevice.GetByUUID(FlushZipConverter.DeviceUuid)
        If pDrDevice Is Nothing Then
            Return Nothing
        End If

        pSql = "SELECT SUM(TransInvoice.TotalAmount) AS TotalAmount" & vbCrLf &
            ",CONVERT(DATE, TransInvoice.InvoiceDate) AS InvoiceDate" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Code AS FowstCode" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            "," & ReadyString(vOrganizationName) & " AS OrganizationName" & vbCrLf &
            ",MobizLibraryPosVendors.Name AS PosVendor" & vbCrLf &
            " FROM " & EscapeSqlName(vDatabaseName) & ".dbo.TransInvoice" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryPosVendors ON MobizLibraryPosVendors.ID = MobizLibraryOrganizationFOWST.PosVendorID" & vbCrLf &
            " WHERE TransInvoice.DeviceID = " & ReadyString(pDrDevice("ID")) & vbCrLf

        If vInvoiceDateFrom IsNot Nothing Then
            pSql &= " AND CONVERT(DATE, TransInvoice.InvoiceDate) >= " & ReadyString(vInvoiceDateFrom.Value.ToString(cGlobal.FormatDateSql)) & vbCrLf
        End If
        If vInvoiceDateTo IsNot Nothing Then
            pSql &= " AND CONVERT(DATE, TransInvoice.InvoiceDate) <= " & ReadyString(vInvoiceDateTo.Value.ToString(cGlobal.FormatDateSql)) & vbCrLf
        End If

        pSql &= " GROUP BY CONVERT(DATE, TransInvoice.InvoiceDate)" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Code" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            ",MobizLibraryPosVendors.Name" & vbCrLf &
            " ORDER BY CONVERT(DATE, TransInvoice.InvoiceDate)" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name" & vbCrLf
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function CacheInvoicesDailyNotTallyWithCacheInvoices(vInvoiceDateFrom As Date, vInvoiceDateTo As Date) As DataTable

        Dim pSql = " -- CacheInvoicesDaily not tally with CacheInvoices" & vbCrLf &
            " -- CacheInvoicesDailyNotTallyWithCacheInvoices" & vbCrLf &
            " DECLARE @DateFrom Date = " & ReadyString(vInvoiceDateFrom.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " DECLARE @DateTo Date = " & ReadyString(vInvoiceDateTo.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            "  SELECT tblCacheInvoices.InvoiceDate" & vbCrLf &
            "  ,tblCacheInvoices.FowstID" & vbCrLf &
            "  ,CacheInvoicesDaily.GrossSalesNetOfVATExemptions AS OldGrossSalesNetOfVATExemptions" & vbCrLf &
            "  ,tblCacheInvoices.GrossSalesNetOfVATExemptions" & vbCrLf &
            "  FROM (SELECT SUM(CacheInvoices.GrossSalesNetOfVATExemptions) AS GrossSalesNetOfVATExemptions" & vbCrLf &
            "  	,CONVERT(date, CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            "  	,CacheInvoices.FowstID" & vbCrLf &
            "  	FROM CacheInvoices" & vbCrLf &
            "  	WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN @DateFrom AND @DateTo" & vbCrLf &
            "     GROUP BY CacheInvoices.FowstID" & vbCrLf &
            "     ,CONVERT(date, CacheInvoices.InvoiceDate)" & vbCrLf &
            "  ) tblCacheInvoices " & vbCrLf &
            " LEFT JOIN CacheInvoicesDaily" & vbCrLf &
            " ON tblCacheInvoices.InvoiceDate = CONVERT(date, CacheInvoicesDaily.InvoiceDate)" & vbCrLf &
            " AND tblCacheInvoices.FowstID = CacheInvoicesDaily.FowstID" & vbCrLf &
            " WHERE CacheInvoicesDaily.FowstID IS NULL" & vbCrLf &
            " OR tblCacheInvoices.GrossSalesNetOfVATExemptions <> ISNULL(CacheInvoicesDaily.GrossSalesNetOfVATExemptions, 0)" & vbCrLf &
            " ORDER BY tblCacheInvoices.InvoiceDate" & vbCrLf &
            " ,CacheInvoicesDaily.FowstID"
        Dim pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    Public Function CacheInvoicesHourlyNotTallyWithCacheInvoices(vInvoiceDateFrom As Date, vInvoiceDateTo As Date) As DataTable

        Dim pSql = "  -- CacheInvoicesHourly not tally with CacheInvoices" & vbCrLf &
            "  -- CacheInvoicesHourlyNotTallyWithCacheInvoices" & vbCrLf &
            " DECLARE @DateFrom Date = " & ReadyString(vInvoiceDateFrom.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " DECLARE @DateTo Date = " & ReadyString(vInvoiceDateTo.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            "   SELECT tblCacheInvoices.InvoiceDate" & vbCrLf &
            "   --,tblCacheInvoices.InvoiceHour" & vbCrLf &
            "   ,tblCacheInvoices.FowstID" & vbCrLf &
            "   --,CacheInvoicesHourly.GrossSalesNetOfVATExemptions AS OldGrossSalesNetOfVATExemptions" & vbCrLf &
            "   --,tblCacheInvoices.GrossSalesNetOfVATExemptions" & vbCrLf &
            "   FROM (SELECT SUM(CacheInvoices.GrossSalesNetOfVATExemptions) AS GrossSalesNetOfVATExemptions" & vbCrLf &
            "   	,CONVERT(date, CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            "     ,DATEPART(HOUR, CacheInvoices.InvoiceDate) AS InvoiceHour" & vbCrLf &
            "   	,CacheInvoices.FowstID" & vbCrLf &
            "   	FROM CacheInvoices" & vbCrLf &
            "   	WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN @DateFrom AND @DateTo" & vbCrLf &
            "     GROUP BY CacheInvoices.FowstID" & vbCrLf &
            "     ,CONVERT(date, CacheInvoices.InvoiceDate)" & vbCrLf &
            "     ,DATEPART(HOUR, CacheInvoices.InvoiceDate)" & vbCrLf &
            "   ) tblCacheInvoices" & vbCrLf &
            "  LEFT JOIN CacheInvoicesHourly" & vbCrLf &
            "  ON tblCacheInvoices.InvoiceDate = CONVERT(date, CacheInvoicesHourly.InvoiceDate)" & vbCrLf &
            "  AND tblCacheInvoices.FowstID = CacheInvoicesHourly.FowstID" & vbCrLf &
            "  AND tblCacheInvoices.InvoiceHour = CacheInvoicesHourly.InvoiceHour" & vbCrLf &
            "  WHERE CacheInvoicesHourly.FowstID IS NULL" & vbCrLf &
            "  OR tblCacheInvoices.GrossSalesNetOfVATExemptions <> ISNULL(CacheInvoicesHourly.GrossSalesNetOfVATExemptions, 0)" & vbCrLf &
            "  ORDER BY tblCacheInvoices.InvoiceDate" & vbCrLf &
            " --,tblCacheInvoices.InvoiceHour" & vbCrLf &
            "  ,CacheInvoicesHourly.FowstID"
        Dim pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    Public Class UpdateCacheInvoicesDailyInput
        'Public StartFresh As String
        Public InvoiceDateFrom As String
        Public InvoiceDateTo As String
        Public FowstDates As List(Of FowstDate)
        Public Progress As Action(Of String) ' set in server

        'Public Function StartFreshProper() As Boolean
        '    Return CBool2(StartFresh)
        'End Function

        Public Function InvoiceDateFromProper() As Date

            Return ParseDate(InvoiceDateFrom)

        End Function

        Public Function InvoiceDateToProper() As Date

            Return ParseDate(InvoiceDateTo)

        End Function

        ''' <summary>
        ''' returns nothing if no error
        ''' </summary>
        ''' <returns></returns>
        Public Function Validate() As String

            If ParseDate(InvoiceDateFrom) Is Nothing Then
                Return "InvoiceDateFrom must be a valid date."
            End If

            If ParseDate(InvoiceDateTo) Is Nothing Then
                Return "InvoiceDateTo must be a valid date."
            End If

            If InvoiceDateToProper() < InvoiceDateFromProper() Then
                Return "InvoiceDateTo must not be earlier than InvoiceDateFrom."
            End If

        End Function

        Public Class FowstDate
            Public FowstID As String
            Public InvoiceDate As Date
        End Class
    End Class

    Public Function UpdateCacheInvoicesDailyNotTallyWithCacheInvoices(vInput As UpdateCacheInvoicesDailyInput)

        vInput.Progress("UpdateCacheInvoicesDailyNotTallyWithCacheInvoices: start")
        If vInput.FowstDates Is Nothing Then
            vInput.FowstDates = New List(Of UpdateCacheInvoicesDailyInput.FowstDate)
            vInput.Progress("CacheInvoicesDailyNotTallyWithCacheInvoices: start")
            Dim pDT = CacheInvoicesDailyNotTallyWithCacheInvoices(vInput.InvoiceDateFromProper(), vInput.InvoiceDateToProper())
            vInput.Progress("CacheInvoicesDailyNotTallyWithCacheInvoices: end. Record count = " & pDT.Rows.Count)
            For Each pDR As DataRow In pDT.Rows
                vInput.FowstDates.Add(New UpdateCacheInvoicesDailyInput.FowstDate() With {
                    .FowstID = CStr2(pDR("FowstID")),
                    .InvoiceDate = pDR("InvoiceDate")
                })
            Next
        End If

        If vInput.FowstDates.Any() Then
            mProData.CreateOrUpdateTable("CacheInvoiceDaily", "CacheInvoices")
        End If
        Dim pIndex = 1
        For Each pFowstDate In vInput.FowstDates
            vInput.Progress("CacheInvoiceDaily(FowstID = " & pFowstDate.FowstID & vbCrLf &
                ", InvoiceDate = " & pFowstDate.InvoiceDate.ToString(cGlobal.FormatDate) & vbCrLf &
                ", Record " & pIndex & " of " & vInput.FowstDates.Count & ")")
            '#If DEBUG Then
            '            Threading.Thread.Sleep(5000)
            '#End If
            CacheInvoiceDaily(
                vMaxAgeInDays:=Integer.MaxValue,
                vInvoiceDateFrom:=pFowstDate.InvoiceDate,
                vInvoiceDateTo:=pFowstDate.InvoiceDate,
                vFowstID:=pFowstDate.FowstID)
            pIndex += 1
        Next
        vInput.Progress("UpdateCacheInvoicesDailyNotTallyWithCacheInvoices: end. Record count = " & vInput.FowstDates.Count)

    End Function

    Public Function UpdateCacheInvoicesHourlyNotTallyWithCacheInvoices(vInput As UpdateCacheInvoicesDailyInput)

        Dim pSql As String
        vInput.Progress("UpdateCacheInvoicesHourlyNotTallyWithCacheInvoices: start")
        If vInput.FowstDates Is Nothing Then
            vInput.FowstDates = New List(Of UpdateCacheInvoicesDailyInput.FowstDate)
            vInput.Progress("CacheInvoicesHourlyNotTallyWithCacheInvoices: start")
            Dim pDT = CacheInvoicesHourlyNotTallyWithCacheInvoices(vInput.InvoiceDateFromProper(), vInput.InvoiceDateToProper())
            vInput.Progress("CacheInvoicesHourlyNotTallyWithCacheInvoices: end. Record count = " & pDT.Rows.Count)
            For Each pDR As DataRow In pDT.Rows
                vInput.FowstDates.Add(New UpdateCacheInvoicesDailyInput.FowstDate() With {
                    .FowstID = CStr2(pDR("FowstID")),
                    .InvoiceDate = pDR("InvoiceDate")
                })
            Next
        End If

        If vInput.FowstDates.Any() Then
            mProData.CreateOrUpdateTable("CacheInvoicesHourly", "CacheInvoices")
        End If
        Dim pIndex = 1

        'If vInput.FowstDates.Any() Then
        '    vInput.Progress("first Date: " & vInput.FowstDates.First().InvoiceDate.ToString(cGlobal.FormatDate) & vbCrLf &
        '        ",last Date: " & vInput.FowstDates.Last().InvoiceDate.ToString(cGlobal.FormatDate) & vbCrLf &
        '        ", InvoiceDateFrom = " & vInput.InvoiceDateFromProper().ToString(cGlobal.FormatDate) & vbCrLf &
        '        ", InvoiceDateTo = " & vInput.InvoiceDateToProper().ToString(cGlobal.FormatDate) & vbCrLf &
        '        "SQL: " & pSql)
        '    Return Nothing
        'End If

        For Each pFowstDate In vInput.FowstDates
            vInput.Progress("CacheInvoiceHourly(FowstID = " & pFowstDate.FowstID & vbCrLf &
                ", InvoiceDate = " & pFowstDate.InvoiceDate.ToString(cGlobal.FormatDate) & vbCrLf &
                ", InvoiceDateFrom = " & vInput.InvoiceDateFromProper().ToString(cGlobal.FormatDate) & vbCrLf &
                ", InvoiceDateTo = " & vInput.InvoiceDateToProper().ToString(cGlobal.FormatDate) & vbCrLf &
                ", Record " & pIndex & " of " & vInput.FowstDates.Count & ")")
            '#If DEBUG Then
            '            Threading.Thread.Sleep(5000)
            '#End If
            CacheInvoiceDaily(
                vMaxAgeInDays:=Integer.MaxValue,
                vInvoiceDateFrom:=pFowstDate.InvoiceDate,
                vInvoiceDateTo:=pFowstDate.InvoiceDate,
                vFowstID:=pFowstDate.FowstID,
                vGroupByHour:=True)
            pIndex += 1
        Next
        vInput.Progress("UpdateCacheInvoicesHourlyNotTallyWithCacheInvoices: end. Record count = " & vInput.FowstDates.Count)

    End Function

    Public Function DeleteCacheInvoicesDailyDuplicates() As Integer

        Dim pSql = " WITH CTE AS (" & vbCrLf &
            " 	SELECT CacheInvoicesDaily.FowstID" & vbCrLf &
            "     ,CONVERT(date, CacheInvoicesDaily.InvoiceDate) AS InvoiceDate" & vbCrLf &
            "     ,CacheInvoicesDaily.GrossSalesNetOfVATExemptions" & vbCrLf &
            "     ,ROW_NUMBER() OVER (" & vbCrLf &
            " 	    PARTITION BY FowstID, CONVERT(date, InvoiceDate)" & vbCrLf &
            "         ORDER BY GrossSalesNetOfVATExemptions DESC" & vbCrLf &
            "     ) AS rn" & vbCrLf &
            "     FROM CacheInvoicesDaily" & vbCrLf &
            " )" & vbCrLf &
            " DELETE FROM CTE " & vbCrLf &
            " WHERE rn > 1"
        Dim pOut = mProData.ExecuteSQL(pSql)
        Return pOut

    End Function
End Class
