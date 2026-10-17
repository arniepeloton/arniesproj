Imports Newtonsoft.Json

Public Class CondorFlushZipConverter
    Const mDummy As String = "test_only"
#If DEBUG Then
    Const mStoreName As String = "test_only"
#Else
    Const mStoreName As String = "PCF00996"
#End If

    Public Sub Execute(vZipFilename As String, ByRef oError As String)

        Dim pZipSource = New ProZip(vZipFilename)
        Dim pDtFilenames = pZipSource.GetFilenames()
        Dim pDrsTransaction = pDtFilenames.Select("FullName LIKE '*_transactions.csv'")
        If pDrsTransaction.Length = 0 Then
            oError = "Zip file does not contain a file that ends in _transactions.csv."
            Return
        End If
        If pDrsTransaction.Length > 1 Then
            oError = "Zip file contains more than 1 file that ends in _transactions.csv."
            Return
        End If
        Dim pTransactionFilename = pDrsTransaction.First()("FullName")
        Dim pDetailsFilename = GetDetailsFilename(pTransactionFilename)
        If String.IsNullOrWhiteSpace(pDetailsFilename) Then
            oError = "Details filename cannot be derived from " & pTransactionFilename & "."
            Return
        End If
        pDrsTransaction = pDtFilenames.Select("FullName = " & ReadyString(pDetailsFilename))
        If pDrsTransaction.Length = 0 Then
            oError = "Zip file does not contain the details file, " & pDetailsFilename & "."
            Return
        End If

        Dim pOutputZipFilename = IO.Path.ChangeExtension(vZipFilename, "mobiz.zip")
        'Dim pOutputZipFilename = cGlobal.AppPath & "temp\ThirdPartyPosFtpUploads\Condor\" & DateTimeStamp(False) & ".zip"
        CreateFolder(pOutputZipFilename)
        Dim pZip As New ProZip(pOutputZipFilename)

        Dim pDtInvoices = New DataTable
        pDtInvoices.Columns.Add("InvoiceNumber", GetType(Integer))
        pDtInvoices.Columns.Add("InvoiceDate", GetType(Date))
        pDtInvoices.Columns.Add("TotalAmount", GetType(Decimal))

        Dim pDtInvoiceItems = New DataTable
        pDtInvoiceItems.Columns.Add("InvoiceNumber", GetType(Integer))
        pDtInvoiceItems.Columns.Add("InvoiceItemNo", GetType(Integer))
        pDtInvoiceItems.Columns.Add("ProductNo")
        pDtInvoiceItems.Columns.Add("Quantity", GetType(Integer))
        pDtInvoiceItems.Columns.Add("UnitPrice", GetType(Decimal))
        pDtInvoiceItems.Columns.Add("SubTotal", GetType(Decimal))

        Dim pDtCustomerPayments = New DataTable
        pDtCustomerPayments.Columns.Add("InvoiceNumber", GetType(Integer))
        pDtCustomerPayments.Columns.Add("CashAmount", GetType(Decimal))
        pDtCustomerPayments.Columns.Add("PaymentDate", GetType(Date))

        Const pInvoiceNumberField = "Order Num / Bill Num"
        Const pInvoiceDateField = "Business Day"
        Const pInvoiceTimeField = "Check Open"
        Const pInvoiceAmountField = "Gross Sales Amount"
        Const pProductNumberField = "Item ID"
        Const pQtyField = "Item Quantity"
        Const pUnitPriceField = "Transaction Item Price"

        Dim pColumns = New String() {
            pInvoiceNumberField,
            pInvoiceDateField,
            pInvoiceTimeField,
            pInvoiceAmountField
        }
        Dim pFileText = pZipSource.GetString(pTransactionFilename)
        If String.IsNullOrWhiteSpace(pFileText) Then
            oError = "Internal file, " & pTransactionFilename & ", is blank."
            Return
        End If

        ' 1st pass. remove returned rows and its invoice
        Dim pCsvSource = CsvReader.CreateFromText(pFileText)
        Dim pReturnedInvoiceNumbers As New List(Of Integer)
        Do
            Dim pLine = pCsvSource.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            Dim pInvoiceNumberText = CStr2(pLine(pInvoiceNumberField))
            Dim pInvoiceNumberComplex = ExtractInvoiceNumber(pInvoiceNumberText)
            If pInvoiceNumberComplex.InvoiceNumber <= 0 Then
                Continue Do
            End If
            If String.Equals(pInvoiceNumberComplex.Prefix, "v", StringComparison.InvariantCultureIgnoreCase) Then
                If pReturnedInvoiceNumbers.Contains(pInvoiceNumberComplex.InvoiceNumber) = False Then
                    pReturnedInvoiceNumbers.Add(pInvoiceNumberComplex.InvoiceNumber)
                End If
                Continue Do
            End If
        Loop

        ' 2nd pass
        Dim pRowNumber = 1
        pCsvSource = CsvReader.CreateFromText(pFileText)
        Do
            Dim pLine = pCsvSource.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pRowNumber += 1
            Dim pInvoiceNumberText = CStr2(pLine(pInvoiceNumberField))
            Dim pInvoiceNumberComplex = ExtractInvoiceNumber(pInvoiceNumberText)
            If pInvoiceNumberComplex.InvoiceNumber <= 0 Then
                oError = "`Order Num / Bill Num` must be a valid number greater than zero."
                Return
            End If
            If pReturnedInvoiceNumbers.Contains(pInvoiceNumberComplex.InvoiceNumber) Then
                Continue Do
            End If
            Dim pRows = pDtInvoices.Select("InvoiceNumber = " & pInvoiceNumberComplex.InvoiceNumber)
            If pRows.Length = 0 Then
                Dim pDrInvoice = pDtInvoices.NewRow()
                pDrInvoice("InvoiceNumber") = CInt2(pLine(pInvoiceNumberField))
                Dim pInvoiceDate = ParseDate(CStr2(pLine(pInvoiceTimeField)))
                If pInvoiceDate Is Nothing Then
                    oError = "Invalid invoice Inv. Date (" & CStr2(pLine(pInvoiceDateField)) & ") in row # " & pRowNumber & "."
                    Return
                End If
                pDrInvoice("InvoiceDate") = pInvoiceDate.Value
                pDrInvoice("TotalAmount") = CDec2(pLine(pInvoiceAmountField))
                pDtInvoices.Rows.Add(pDrInvoice)
            End If
        Loop

        pColumns = New String() {
            pInvoiceNumberField,
            pQtyField,
            pProductNumberField,
            pUnitPriceField
        }
        pFileText = pZipSource.GetString(pDetailsFilename)
        If String.IsNullOrWhiteSpace(pFileText) Then
            oError = "Internal file, " & pDetailsFilename & ", is blank."
            Return
        End If
        pCsvSource = CsvReader.CreateFromText(pFileText)

        pRowNumber = 1
        Dim pInvoiceItemNo = 0
        Do
            Dim pLine = pCsvSource.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pRowNumber += 1
            Dim pInvoiceNumber = CInt2(pLine(pInvoiceNumberField))
            If pInvoiceNumber <= 0 Then
                oError = "`Order Num / Bill Num` must be greater than zero."
                Return
            End If
            If pReturnedInvoiceNumbers.Contains(pInvoiceNumber) Then
                Continue Do
            End If
            Dim pDrInvoiceItem = pDtInvoiceItems.NewRow()
            pDrInvoiceItem("InvoiceNumber") = pInvoiceNumber
            pInvoiceItemNo += 1
            pDrInvoiceItem("InvoiceItemNo") = pInvoiceItemNo
            pDrInvoiceItem("ProductNo") = pLine(pProductNumberField)
            pDrInvoiceItem("Quantity") = CInt2(pLine(pQtyField))
            pDrInvoiceItem("UnitPrice") = CDec2(pLine(pUnitPriceField))
            pDrInvoiceItem("SubTotal") = CInt2(pLine(pQtyField)) * CDec2(pLine(pUnitPriceField))
            pDtInvoiceItems.Rows.Add(pDrInvoiceItem)
        Loop

        Dim pCsvWriter = New CsvWriter()
        pCsvWriter.MaxLength = Integer.MaxValue

        pCsvWriter.Source = pDtInvoices
        pZip.AddString(pCsvWriter.EncodeBatch(), "TransInvoice.csv")

        pCsvWriter.Source = pDtInvoiceItems
        pZip.AddString(pCsvWriter.EncodeBatch(), "TransInvoiceItems.csv")

        For Each pDrInvoice As DataRow In pDtInvoices.Rows
            Dim pDrPayment = pDtCustomerPayments.NewRow
            pDrPayment("InvoiceNumber") = pDrInvoice("InvoiceNumber")
            pDrPayment("CashAmount") = pDrInvoice("TotalAmount")
            pDrPayment("PaymentDate") = pDrInvoice("InvoiceDate")
            pDtCustomerPayments.Rows.Add(pDrPayment)
        Next

        pCsvWriter.Source = pDtCustomerPayments
        pZip.AddString(pCsvWriter.EncodeBatch(), "TransCustomerPayment.csv")

        Dim pInfo = New FlushZipConverter.JsonInfo()
        pInfo.StoreName = mStoreName
        pInfo.CsvFilename = vZipFilename
        pZip.AddString(JsonConvert.SerializeObject(pInfo, Formatting.Indented), "info.json")

    End Sub

    Shared Function GetDetailsFilename(vTransactionFilename As String) As String

        Dim pIndex = vTransactionFilename.LastIndexOf("_transactions.csv")
        If pIndex = -1 Then
            Return ""
        End If
        Dim pPrefix = vTransactionFilename.Substring(0, pIndex)
        Dim pOut = pPrefix & "_transactiondetails.csv"
        Return pOut

    End Function

    Shared Function ExtractInvoiceNumber(vText As String) As (Prefix As String, InvoiceNumber As Integer)

        vText = CStr2(vText).Trim()
        If vText.StartsWith("v", StringComparison.InvariantCultureIgnoreCase) Then
            Dim pInvoiceNumberText = vText.Substring(1)
            Dim pInvoiceNumber = CInt2(pInvoiceNumberText)
            Return (Prefix:="V", InvoiceNumber:=pInvoiceNumber)
        Else
            Return (Prefix:="", invoicenumber:=CInt2(vText))
        End If

    End Function
End Class
