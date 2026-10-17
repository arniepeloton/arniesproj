Public Class TransESales

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetData(vYear As Integer, vMachineNumber As String) As DataRow

        vMachineNumber = Trim(vMachineNumber)
        Dim pSql = "SELECT * FROM TransESalesYearly" & vbCrLf &
            " WHERE ZReadYear = " & vYear & vbCrLf &
            " AND MachineNumber = " & ReadyString(vMachineNumber)
        Dim pDR = mProData.ExecuteRow(pSql)
        Return pDR

    End Function

    Public Class SaveInput
        Public ZReadYear As Integer
        Public MachineNumber As String
        Public VatRate As Decimal
        Public OutputTaxRate As Decimal
        Public PreviousGrossSales As Decimal
        Public PreviousVatSales As Decimal
        Public TotalGrossSales As Decimal
        Public TotalVatSales As Decimal
        Public UserID As String

        Public InvoiceNumber01 As Integer
        Public InvoiceNumber02 As Integer
        Public InvoiceNumber03 As Integer
        Public InvoiceNumber04 As Integer
        Public InvoiceNumber05 As Integer
        Public InvoiceNumber06 As Integer
        Public InvoiceNumber07 As Integer
        Public InvoiceNumber08 As Integer
        Public InvoiceNumber09 As Integer
        Public InvoiceNumber10 As Integer
        Public InvoiceNumber11 As Integer
        Public InvoiceNumber12 As Integer

        Public GrossSales01 As Decimal
        Public GrossSales02 As Decimal
        Public GrossSales03 As Decimal
        Public GrossSales04 As Decimal
        Public GrossSales05 As Decimal
        Public GrossSales06 As Decimal
        Public GrossSales07 As Decimal
        Public GrossSales08 As Decimal
        Public GrossSales09 As Decimal
        Public GrossSales10 As Decimal
        Public GrossSales11 As Decimal
        Public GrossSales12 As Decimal

        Public AccumulatedGrossSales01 As Decimal
        Public AccumulatedGrossSales02 As Decimal
        Public AccumulatedGrossSales03 As Decimal
        Public AccumulatedGrossSales04 As Decimal
        Public AccumulatedGrossSales05 As Decimal
        Public AccumulatedGrossSales06 As Decimal
        Public AccumulatedGrossSales07 As Decimal
        Public AccumulatedGrossSales08 As Decimal
        Public AccumulatedGrossSales09 As Decimal
        Public AccumulatedGrossSales10 As Decimal
        Public AccumulatedGrossSales11 As Decimal
        Public AccumulatedGrossSales12 As Decimal

        Public VatSales01 As Decimal
        Public VatSales02 As Decimal
        Public VatSales03 As Decimal
        Public VatSales04 As Decimal
        Public VatSales05 As Decimal
        Public VatSales06 As Decimal
        Public VatSales07 As Decimal
        Public VatSales08 As Decimal
        Public VatSales09 As Decimal
        Public VatSales10 As Decimal
        Public VatSales11 As Decimal
        Public VatSales12 As Decimal

        Public AccumulatedVatSales01 As Decimal
        Public AccumulatedVatSales02 As Decimal
        Public AccumulatedVatSales03 As Decimal
        Public AccumulatedVatSales04 As Decimal
        Public AccumulatedVatSales05 As Decimal
        Public AccumulatedVatSales06 As Decimal
        Public AccumulatedVatSales07 As Decimal
        Public AccumulatedVatSales08 As Decimal
        Public AccumulatedVatSales09 As Decimal
        Public AccumulatedVatSales10 As Decimal
        Public AccumulatedVatSales11 As Decimal
        Public AccumulatedVatSales12 As Decimal

        Public OutputTax01 As Decimal
        Public OutputTax02 As Decimal
        Public OutputTax03 As Decimal
        Public OutputTax04 As Decimal
        Public OutputTax05 As Decimal
        Public OutputTax06 As Decimal
        Public OutputTax07 As Decimal
        Public OutputTax08 As Decimal
        Public OutputTax09 As Decimal
        Public OutputTax10 As Decimal
        Public OutputTax11 As Decimal
        Public OutputTax12 As Decimal
    End Class

    Public Sub Save(vInput As SaveInput)

        vInput.MachineNumber = CStr2(vInput.MachineNumber).Trim()
        Dim pIU As New InsertUpdate("TransESalesYearly")
        pIU("ZReadYear") = ReadyString(vInput.ZReadYear)
        pIU("MachineNumber") = ReadyString(vInput.MachineNumber)
        pIU("VatRate") = CDec2(vInput.VatRate)
        pIU("OutputTaxRate") = CDec2(vInput.OutputTaxRate)
        pIU("PreviousGrossSales") = CDec2(vInput.PreviousGrossSales)
        pIU("PreviousVatSales") = CDec2(vInput.PreviousVatSales)
        pIU("UserID") = ReadyString(vInput.UserID)

        pIU("InvoiceNumber01") = CDec2(vInput.InvoiceNumber01)
        pIU("InvoiceNumber02") = CDec2(vInput.InvoiceNumber02)
        pIU("InvoiceNumber03") = CDec2(vInput.InvoiceNumber03)
        pIU("InvoiceNumber04") = CDec2(vInput.InvoiceNumber04)
        pIU("InvoiceNumber05") = CDec2(vInput.InvoiceNumber05)
        pIU("InvoiceNumber06") = CDec2(vInput.InvoiceNumber06)
        pIU("InvoiceNumber07") = CDec2(vInput.InvoiceNumber07)
        pIU("InvoiceNumber08") = CDec2(vInput.InvoiceNumber08)
        pIU("InvoiceNumber09") = CDec2(vInput.InvoiceNumber09)
        pIU("InvoiceNumber10") = CDec2(vInput.InvoiceNumber10)
        pIU("InvoiceNumber11") = CDec2(vInput.InvoiceNumber11)
        pIU("InvoiceNumber12") = CDec2(vInput.InvoiceNumber12)

        pIU("GrossSales01") = CDec2(vInput.GrossSales01)
        pIU("GrossSales02") = CDec2(vInput.GrossSales02)
        pIU("GrossSales03") = CDec2(vInput.GrossSales03)
        pIU("GrossSales04") = CDec2(vInput.GrossSales04)
        pIU("GrossSales05") = CDec2(vInput.GrossSales05)
        pIU("GrossSales06") = CDec2(vInput.GrossSales06)
        pIU("GrossSales07") = CDec2(vInput.GrossSales07)
        pIU("GrossSales08") = CDec2(vInput.GrossSales08)
        pIU("GrossSales09") = CDec2(vInput.GrossSales09)
        pIU("GrossSales10") = CDec2(vInput.GrossSales10)
        pIU("GrossSales11") = CDec2(vInput.GrossSales11)
        pIU("GrossSales12") = CDec2(vInput.GrossSales12)

        pIU("AccumulatedGrossSales01") = CDec2(vInput.AccumulatedGrossSales01)
        pIU("AccumulatedGrossSales02") = CDec2(vInput.AccumulatedGrossSales02)
        pIU("AccumulatedGrossSales03") = CDec2(vInput.AccumulatedGrossSales03)
        pIU("AccumulatedGrossSales04") = CDec2(vInput.AccumulatedGrossSales04)
        pIU("AccumulatedGrossSales05") = CDec2(vInput.AccumulatedGrossSales05)
        pIU("AccumulatedGrossSales06") = CDec2(vInput.AccumulatedGrossSales06)
        pIU("AccumulatedGrossSales07") = CDec2(vInput.AccumulatedGrossSales07)
        pIU("AccumulatedGrossSales08") = CDec2(vInput.AccumulatedGrossSales08)
        pIU("AccumulatedGrossSales09") = CDec2(vInput.AccumulatedGrossSales09)
        pIU("AccumulatedGrossSales10") = CDec2(vInput.AccumulatedGrossSales10)
        pIU("AccumulatedGrossSales11") = CDec2(vInput.AccumulatedGrossSales11)
        pIU("AccumulatedGrossSales12") = CDec2(vInput.AccumulatedGrossSales12)

        pIU("VatSales01") = CDec2(vInput.VatSales01)
        pIU("VatSales02") = CDec2(vInput.VatSales02)
        pIU("VatSales03") = CDec2(vInput.VatSales03)
        pIU("VatSales04") = CDec2(vInput.VatSales04)
        pIU("VatSales05") = CDec2(vInput.VatSales05)
        pIU("VatSales06") = CDec2(vInput.VatSales06)
        pIU("VatSales07") = CDec2(vInput.VatSales07)
        pIU("VatSales08") = CDec2(vInput.VatSales08)
        pIU("VatSales09") = CDec2(vInput.VatSales09)
        pIU("VatSales10") = CDec2(vInput.VatSales10)
        pIU("VatSales11") = CDec2(vInput.VatSales11)
        pIU("VatSales12") = CDec2(vInput.VatSales12)

        pIU("AccumulatedVatSales01") = CDec2(vInput.AccumulatedVatSales01)
        pIU("AccumulatedVatSales02") = CDec2(vInput.AccumulatedVatSales02)
        pIU("AccumulatedVatSales03") = CDec2(vInput.AccumulatedVatSales03)
        pIU("AccumulatedVatSales04") = CDec2(vInput.AccumulatedVatSales04)
        pIU("AccumulatedVatSales05") = CDec2(vInput.AccumulatedVatSales05)
        pIU("AccumulatedVatSales06") = CDec2(vInput.AccumulatedVatSales06)
        pIU("AccumulatedVatSales07") = CDec2(vInput.AccumulatedVatSales07)
        pIU("AccumulatedVatSales08") = CDec2(vInput.AccumulatedVatSales08)
        pIU("AccumulatedVatSales09") = CDec2(vInput.AccumulatedVatSales09)
        pIU("AccumulatedVatSales10") = CDec2(vInput.AccumulatedVatSales10)
        pIU("AccumulatedVatSales11") = CDec2(vInput.AccumulatedVatSales11)
        pIU("AccumulatedVatSales12") = CDec2(vInput.AccumulatedVatSales12)

        pIU("OutputTax01") = CDec2(vInput.OutputTax01)
        pIU("OutputTax02") = CDec2(vInput.OutputTax02)
        pIU("OutputTax03") = CDec2(vInput.OutputTax03)
        pIU("OutputTax04") = CDec2(vInput.OutputTax04)
        pIU("OutputTax05") = CDec2(vInput.OutputTax05)
        pIU("OutputTax06") = CDec2(vInput.OutputTax06)
        pIU("OutputTax07") = CDec2(vInput.OutputTax07)
        pIU("OutputTax08") = CDec2(vInput.OutputTax08)
        pIU("OutputTax09") = CDec2(vInput.OutputTax09)
        pIU("OutputTax10") = CDec2(vInput.OutputTax10)
        pIU("OutputTax11") = CDec2(vInput.OutputTax11)
        pIU("OutputTax12") = CDec2(vInput.OutputTax12)

        pIU("TotalGrossSales") = vInput.PreviousGrossSales +
            vInput.GrossSales01 +
            vInput.GrossSales02 +
            vInput.GrossSales03 +
            vInput.GrossSales04 +
            vInput.GrossSales05 +
            vInput.GrossSales06 +
            vInput.GrossSales07 +
            vInput.GrossSales08 +
            vInput.GrossSales09 +
            vInput.GrossSales10 +
            vInput.GrossSales11 +
            vInput.GrossSales12

        pIU("TotalVatSales") = vInput.PreviousVatSales +
            vInput.VatSales01 +
            vInput.VatSales02 +
            vInput.VatSales03 +
            vInput.VatSales04 +
            vInput.VatSales05 +
            vInput.VatSales06 +
            vInput.VatSales07 +
            vInput.VatSales08 +
            vInput.VatSales09 +
            vInput.VatSales10 +
            vInput.VatSales11 +
            vInput.VatSales12

        Dim pSql = pIU.GetUpdate("ZReadYear = " & ReadyString(vInput.ZReadYear) & vbCrLf &
            " AND MachineNumber = " & ReadyString(vInput.MachineNumber))
        mProData.BeginTransaction()
        Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
        If pAffectedRecords = 0 Then
            pIU("ID") = "NEWID()"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If
        UpdateFutureTotals(vInput.ZReadYear, vInput.MachineNumber, pIU("TotalGrossSales"), pIU("TotalVatSales"))
        mProData.CommitTransaction()

    End Sub

    Function GetPastTotals(vYear As Integer, vMachineNumber As String) As DataRow

        Dim pSql = "SELECT MAX(ZReadYear) AS ZReadYear" & vbCrLf &
            ",SUM(GrossSales01 + GrossSales02 + GrossSales03 + GrossSales04 + GrossSales05 + GrossSales06 + GrossSales07 + GrossSales08 + GrossSales09 + GrossSales10 + GrossSales11 + GrossSales12) AS GrossSales" &
            ",SUM(VatSales01 + VatSales02 + VatSales03 + VatSales04 + VatSales05 + VatSales06 + VatSales07 + VatSales08 + VatSales09 + VatSales10 + VatSales11 + VatSales12) AS VatSales" &
            " FROM TransESalesYearly" & vbCrLf &
            " WHERE ZReadYear < " & vYear & vbCrLf &
            " AND MachineNumber = " & ReadyString(CStr2(vMachineNumber).Trim()) & vbCrLf
        Dim pDR = mProData.ExecuteRow(pSql)
        Return pDR

    End Function

    Function UpdateFutureTotals(vYear As Integer, vMachineNumber As String,
        vPreviousGrossSales As Decimal, vPreviousVatSales As Decimal) As DataRow

        Dim pSql = "SELECT ID, ZReadYear" & vbCrLf &
            ",(GrossSales01 + GrossSales02 + GrossSales03 + GrossSales04 + GrossSales05 + GrossSales06 + GrossSales07 + GrossSales08 + GrossSales09 + GrossSales10 + GrossSales11 + GrossSales12) AS GrossSales" &
            ",(VatSales01 + VatSales02 + VatSales03 + VatSales04 + VatSales05 + VatSales06 + VatSales07 + VatSales08 + VatSales09 + VatSales10 + VatSales11 + VatSales12) AS VatSales" &
            " FROM TransESalesYearly" & vbCrLf &
            " WHERE ZReadYear > " & vYear & vbCrLf &
            " AND MachineNumber = " & ReadyString(CStr2(vMachineNumber).Trim()) & vbCrLf &
            " ORDER BY ZReadYear"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("TransESalesYearly")
            pIU("PreviousGrossSales") = vPreviousGrossSales
            pIU("PreviousVatSales") = vPreviousVatSales
            vPreviousGrossSales = CDec2(pDR("GrossSales"))
            vPreviousVatSales = CDec2(pDR("VatSales"))
            pIU("TotalGrossSales") = vPreviousGrossSales
            pIU("TotalVatSales") = vPreviousVatSales
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next

    End Function
End Class