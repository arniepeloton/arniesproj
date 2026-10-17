Public Class LibraryDocumentControl
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function CreateData() As Integer

        Dim pDT As DataTable

        pDT = GetData()

        pDT.DefaultView.RowFilter = "DocumentName = 'Price Quote'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Price Quote",
                vModuleSource:="Revenue > Price Quote",
                vPrintLimit:=-1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Sales Order'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Sales Order",
                vModuleSource:="Revenue > Sales Ordering",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Pick List'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Pick List",
                vModuleSource:="Revenue > Sales Ordering",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Price Canvass'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Price Canvass",
                vModuleSource:="Disbursement > Canvassing",
                vPrintLimit:=-1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Purchase Order'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Purchase Order",
                vModuleSource:="Disbursement > PO",
                vPrintLimit:=-1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'No-Price Purchase Order'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="No-Price Purchase Order",
                vModuleSource:="Disbursement > PO",
                vPrintLimit:=-1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Stock Receiving'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Stock Receiving",
                vModuleSource:="Disbursement > Stock Receiving",
                vPrintLimit:=-1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Sales Invoice'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Sales Invoice",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'STR'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="STR",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'STI'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="STI",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Withdrawal Slip'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Withdrawal Slip",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'CHI'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="CHI",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'POS'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="POS",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

        pDT.DefaultView.RowFilter = "DocumentName = 'Trust Receipt'"
        If pDT.DefaultView.Count = 0 Then
            Save(vDocumentName:="Trust Receipt",
                vModuleSource:="Revenue > Invoicing",
                vPrintLimit:=1,
                vCopiesPerPrint:=1
            )
        End If

    End Function

    Public Function GetData(Optional vID As String = Nothing, Optional vFilter As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " WHERE 1=1" & vbCrLf
        If (vID IsNot Nothing) Then
            pSql &= " AND ID = " & ReadyString(vID) & vbCrLf
        End If
        If String.IsNullOrWhiteSpace(vFilter) = False Then
            pSql &= vFilter & vbCrLf
        End If
        pSql = "SELECT ID" & vbCrLf &
            ",DocumentName" & vbCrLf &
            ",SeriesType" & vbCrLf &
            ",SeriesStartingNum" & vbCrLf &
            ",PermitNum" & vbCrLf &
            ",ModuleSource" & vbCrLf &
            ",PrintLimit" & vbCrLf &
            ",CopiesPerPrint" & vbCrLf &
            ",LabelOfCopy01" & vbCrLf &
            ",LabelOfCopy02" & vbCrLf &
            ",LabelOfCopy03" & vbCrLf &
            ",LabelOfCopy04" & vbCrLf &
            ",LabelOfCopy05" & vbCrLf &
            ",LabelOfCopy06" & vbCrLf &
            ",LabelOfCopy07" & vbCrLf &
            ",LabelOfCopy08" & vbCrLf &
            ",LabelOfCopy09" & vbCrLf &
            ",LabelOfCopy10" & vbCrLf &
            ",IsActive" & vbCrLf &
            ",AutoPrint" & vbCrLf &
            ",Footnote" & vbCrLf &
            ",PrinterName" & vbCrLf &
            ",Preprinted" & vbCrLf &
            ",MaxLineItemsPerPage" & vbCrLf &
            ",MaxNoOfSupplementalBlindCopies" & vbCrLf &
            ",UnitPriceSurchargePercent" & vbCrLf &
            " ,STUFF(ISNULL(',' + NULLIF(LabelOfCopy01, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy02, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy03, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy04, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy05, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy06, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy07, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy08, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy09, ''), '')" & vbCrLf &
            " + ISNULL(',' + NULLIF(LabelOfCopy10, ''), '')" & vbCrLf &
            " , 1, 1, '') AS LabelOfCopies" & vbCrLf &
            " FROM LibraryDocumentControl" & vbCrLf &
            pSql &
            " ORDER BY DocumentName, ModuleSource" & vbCrLf

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetByDocumentNameAndModuleSource(vDocumentName As String, vModuleSource As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        If String.IsNullOrWhiteSpace(vDocumentName) = False Then
            pSql = " AND DocumentName = " & ReadyString(vDocumentName)
        End If
        pSql = "SELECT ID" & vbCrLf &
            ",DocumentName" & vbCrLf &
            ",SeriesType" & vbCrLf &
            ",SeriesStartingNum" & vbCrLf &
            ",PermitNum" & vbCrLf &
            ",ModuleSource" & vbCrLf &
            ",PrintLimit" & vbCrLf &
            ",CopiesPerPrint" & vbCrLf &
            ",LabelOfCopy01" & vbCrLf &
            ",LabelOfCopy02" & vbCrLf &
            ",LabelOfCopy03" & vbCrLf &
            ",LabelOfCopy04" & vbCrLf &
            ",LabelOfCopy05" & vbCrLf &
            ",LabelOfCopy06" & vbCrLf &
            ",LabelOfCopy07" & vbCrLf &
            ",LabelOfCopy08" & vbCrLf &
            ",LabelOfCopy09" & vbCrLf &
            ",LabelOfCopy10" & vbCrLf &
            ",IsActive" & vbCrLf &
            ",Footnote" & vbCrLf &
            ",PrinterName" & vbCrLf &
            ",Preprinted" & vbCrLf &
            ",MaxLineItemsPerPage" & vbCrLf &
            ",MaxNoOfSupplementalBlindCopies" & vbCrLf &
            " FROM LibraryDocumentControl" & vbCrLf &
            " WHERE ModuleSource = " & ReadyString(vModuleSource) & vbCrLf &
            pSql
        pDR = mProData.ExecuteRow(pSql)

        Return pDR
        
    End Function

    Sub Save(vDocumentName As String,
        vModuleSource As String,
        vPrintLimit As Integer,
        vCopiesPerPrint As Integer)

        Dim pSql As String
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("LibraryDocumentControl")
        pIU("ID") = "NEWID()"
        pIU("DocumentName") = ReadyString(vDocumentName)
        pIU("ModuleSource") = ReadyString(vModuleSource)
        pIU("PrintLimit") = vPrintLimit
        pIU("CopiesPerPrint") = vCopiesPerPrint
        pIU("IsActive") = 1
        pSql = pIU.GetInsert()

        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function Save(vJson As SaveJson) As String

        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pID As String

        pIU = New InsertUpdate("LibraryDocumentControl")
        pIU("DocumentName") = ReadyString(vJson.DocumentName)
        pIU("ModuleSource") = ReadyString(vJson.ModuleSource)
        pIU("PrintLimit") = vJson.PrintLimit
        pIU("CopiesPerPrint") = vJson.CopiesPerPrint
        pIU("LabelOfCopy01") = ReadyString(vJson.LabelOfCopy01)
        pIU("LabelOfCopy02") = ReadyString(vJson.LabelOfCopy02)
        pIU("LabelOfCopy03") = ReadyString(vJson.LabelOfCopy03)
        pIU("LabelOfCopy04") = ReadyString(vJson.LabelOfCopy04)
        pIU("LabelOfCopy05") = ReadyString(vJson.LabelOfCopy05)
        pIU("LabelOfCopy06") = ReadyString(vJson.LabelOfCopy06)
        pIU("LabelOfCopy07") = ReadyString(vJson.LabelOfCopy07)
        pIU("LabelOfCopy08") = ReadyString(vJson.LabelOfCopy08)
        pIU("LabelOfCopy09") = ReadyString(vJson.LabelOfCopy09)
        pIU("LabelOfCopy10") = ReadyString(vJson.LabelOfCopy10)
        pIU("IsActive") = If(vJson.IsActiveProper, 1, 0)
        pIU("AutoPrint") = If(vJson.AutoPrint, 1, 0)
        pIU("Preprinted") = If(vJson.PreprintedProper, 1, 0)
        pIU("Footnote") = ReadyString(vJson.Footnote)
        pIU("PrinterName") = ReadyString(Truncate(vJson.PrinterName, 100))
        pIU("SeriesType") = vJson.SeriesTypeProper
        pIU("UnitPriceSurchargePercent") = vJson.UnitPriceSurchargePercent
        pIU("PermitNum") = ReadyString(vJson.PermitNum) ' BIR Computerized Account System Permit No.
        If String.IsNullOrWhiteSpace(vJson.SeriesStartingNum) = False Then
            pIU("SeriesStartingNum") = vJson.SeriesStartingNumProper
        End If
        If String.IsNullOrWhiteSpace(vJson.MaxLineItemsPerPage) = False Then
            pIU("MaxLineItemsPerPage") = CInt2(vJson.MaxLineItemsPerPage)
        End If
        If String.IsNullOrWhiteSpace(vJson.MaxNoOfSupplementalBlindCopies) = False Then
            pIU("MaxNoOfSupplementalBlindCopies") = CInt2(vJson.MaxNoOfSupplementalBlindCopies)
        End If
        If IsGuid(vJson.ID) Then
            pSql = pIU.GetUpdate("ID = " & ReadyString(vJson.ID))
            pID = vJson.ID
        Else
            pID = Guid.NewGuid.ToString()
            pIU("ID") = ReadyString(pID)
            pSql = pIU.GetInsert()
        End If

        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Class SaveJson
        Public ID As String
        Public DocumentName As String
        Public ModuleSource As String
        Public PrintLimit As String
        Public CopiesPerPrint As String
        Public IsActive As String
        Public AutoPrint As String
        Public LabelOfCopy01 As String
        Public LabelOfCopy02 As String
        Public LabelOfCopy03 As String
        Public LabelOfCopy04 As String
        Public LabelOfCopy05 As String
        Public LabelOfCopy06 As String
        Public LabelOfCopy07 As String
        Public LabelOfCopy08 As String
        Public LabelOfCopy09 As String
        Public LabelOfCopy10 As String
        Public Footnote As String
        Public PrinterName As String
        Public Preprinted As String ' boolean
        Public MaxLineItemsPerPage As String ' int
        Public MaxNoOfSupplementalBlindCopies As String ' int
        Public UnitPriceSurchargePercent As String ' dec

        'null/0 = fowst-device;
        '1 = fowst
        Public SeriesType As String
        Public SeriesStartingNum As String
        Public PermitNum As String

        Public Function Verify() As String

            If IsNumeric(PrintLimit) = False Then
                Return "PrintLimit must be numeric"
            End If
            If New Integer() {-1, 1}.Contains(PrintLimitProper) = False Then
                Return "PrintLimit must be -1 or 1"
            End If
            If IsNumeric(CopiesPerPrint) = False Then
                Return "CopiesPerPrint must be numeric"
            End If
            If CopiesPerPrintProper < 1 Then
                Return "CopiesPerPrint must be at greater than or equal to 1"
            End If
            If 10 < CopiesPerPrintProper Then
                Return "CopiesPerPrint must be at lesser than or equal to 10"
            End If

            If String.IsNullOrWhiteSpace(SeriesStartingNum) = False Then
                If IsNumeric(SeriesStartingNum) = False _
                    OrElse SeriesStartingNumProper <= 0 Then

                    Return "Series Starting Num must be a valid number that is at least 1"
                End If
            End If

            If String.IsNullOrWhiteSpace(SeriesType) = False Then
                If New Integer() {0, 1}.Contains(SeriesTypeProper) = False Then
                    Return "SeriesType must be 0 or 1"
                End If
            End If

        End Function

        Public ReadOnly Property SeriesStartingNumProper As Integer
            Get
                Return CInt2(SeriesStartingNum)
            End Get
        End Property

        Public ReadOnly Property PrintLimitProper As Integer
            Get
                Return CInt2(PrintLimit)
            End Get
        End Property

        Public ReadOnly Property CopiesPerPrintProper As Integer
            Get
                Return CInt2(CopiesPerPrint)
            End Get
        End Property

        Public ReadOnly Property IsActiveProper As Boolean
            Get
                Return CBool2(IsActive)
            End Get
        End Property

        Public ReadOnly Property SeriesTypeProper As Integer
            Get
                Return CInt2(SeriesType)
            End Get
        End Property

        Public ReadOnly Property PreprintedProper As Boolean
            Get
                Return CBool2(Preprinted)
            End Get
        End Property
    End Class

    Shared Function Int2Guid(vInt As Integer) As Guid
        ' http://stackoverflow.com/a/7363164/334045

        Dim pBytes As Byte()

        ReDim pBytes(16)
        BitConverter.GetBytes(vInt).CopyTo(pBytes, 0)

        Return New Guid(pBytes)

    End Function

End Class