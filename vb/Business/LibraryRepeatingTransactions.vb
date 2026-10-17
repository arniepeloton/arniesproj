Imports System.IO

Public Class LibraryRepeatingTransactions
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function HandleAllOrg() As Integer

        Dim pDtDbs As DataTable
        Dim pDatabase As String
        Dim pOrgID As String
        Dim pError As String

        pDtDbs = mParent.LibraryOrganization.GetAll
        For Each pDR As DataRow In pDtDbs.Rows
            Try
                ' it is important to clear these first
                pDatabase = ""
                pOrgID = ""

                pDatabase = pDR("Database")
                pOrgID = CStr2(pDR("ID"))
                mProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                Handle1Org(pOrgID)
            Catch ex As Exception
                pError = ex.Message & vbCrLf &
                    "Database: " & pDatabase & vbCrLf &
                    "OrgID: " & pOrgID & vbCrLf
                AppendToLog(pError, , "LibraryRepeatingTransactions_HandleAllOrg")
            End Try
        Next pDR

    End Function

    Public Function Handle1Org(vOrgID As String) As Integer

        Dim pSuccessAll, pSuccess1 As Integer
        Dim pCheckOutTime As Date

        pCheckOutTime = mParent.SystemDefaults.CheckOutTimeDefault
        WebRequest.Instance.Business.LibraryRepeatingTransactions.SearchAndCreateDefinition(pCheckOutTime)
        Do
            pSuccess1 = WebRequest.Instance.Business.LibraryRepeatingTransactions.SearchAndCreateInvoiceFromDefinitions(vOrgID, pCheckOutTime)
            pSuccessAll += pSuccess1
            If pSuccess1 = 0 Then
                Exit Do
            End If
        Loop
        If pSuccessAll > 0 Then
            WebRequest.Instance.Business.TransInvoice.CacheInvoicesWithCredit()
        End If

        Return pSuccessAll

    End Function

    Private Function SearchAndCreateDefinition(vCheckOutTime As Date) As Integer

        Dim pDT As DataTable
        Dim pSourceDefinition As SourceDefinition

        pDT = CreateableDefinition()
        For Each pDR As DataRow In pDT.Rows
            pSourceDefinition = New SourceDefinition()
            With pSourceDefinition
                If String.IsNullOrWhiteSpace(CStr2(pDR("RepeatingTransactionID"))) = False Then
                    .RepeatingTransactionID = CStr2(pDR("RepeatingTransactionID"))
                End If
                .MethodType = CStr2(pDR("MethodType")).Trim().ToLower()
                If .MethodType = "bydaysinterval" Then
                    .RepeatInterval = SourceDefinition.eRepeatInterval.Daily
                    .IntervalRepeatFrequency = CInt2(pDR("NoOfDays"))
                ElseIf .MethodType = "bynthday" Then
                    .RepeatInterval = SourceDefinition.eRepeatInterval.Monthly
                    .IntervalRepeatFrequency = 1 ' temporary
                    .DateOfMonth = CInt2(pDR("NoOfDays")) ' workaround
                End If
                .DateStart = pDR("CheckedInUsedOn")
                .CheckedOut = pDR("CheckedOutReturnedOn")
                .SourceTransRentableAssetID = CStr2(pDR("ID"))
                .IsDone = CBool2(pDR("IsDone"))
#If DEBUG Then
                .Prepaid = True ' debug
                .Prepaid = CStr2(pDR("MonthlyBillType")).ToLower() = "prepaid" ' prod
#Else
                .Prepaid = CStr2(pDR("MonthlyBillType")).ToLower() = "prepaid"
#End If
            End With
            SetRunDate(pSourceDefinition, vCheckOutTime)
            If pSourceDefinition.Prepaid Then
                pSourceDefinition.NextRunDate = Now.Date
            End If
            CreateDefinition(pSourceDefinition)
        Next pDR

    End Function

    Private Sub SetRunDate(ByRef rSourceDefinition As SourceDefinition, vCheckOutTime As Date)

        Dim pMonthRange As DateRange
        Dim pFindDateOfMonth As Integer

        If rSourceDefinition.LastRunDate Is Nothing Then
            rSourceDefinition.LastRunDate = rSourceDefinition.DateStart
        End If

        Select Case rSourceDefinition.RepeatInterval
            Case SourceDefinition.eRepeatInterval.None
                If rSourceDefinition.MethodType = "uponcheckin" Then
                    rSourceDefinition.NextRunDate = rSourceDefinition.DateStart
                Else
                    rSourceDefinition.NextRunDate = New Date(2222, 2, 2)
                End If

            Case SourceDefinition.eRepeatInterval.Daily
                rSourceDefinition.NextRunDate = rSourceDefinition.LastRunDate.Value.Date.AddDays(rSourceDefinition.IntervalRepeatFrequency).Add(vCheckOutTime.TimeOfDay)

            Case SourceDefinition.eRepeatInterval.Monthly
                If rSourceDefinition.DateOfMonth <= 0 Then
                    rSourceDefinition.NextRunDate = rSourceDefinition.LastRunDate.Value.Date.AddMonths(rSourceDefinition.IntervalRepeatFrequency).Add(vCheckOutTime.TimeOfDay)
                Else
                    pMonthRange = MonthRange(rSourceDefinition.LastRunDate.Value)
                    If rSourceDefinition.LastRunDate.Value.Day < rSourceDefinition.DateOfMonth Then
                        pMonthRange.DateFrom = pMonthRange.DateFrom.AddMonths(rSourceDefinition.IntervalRepeatFrequency - 1)
                    Else
                        pMonthRange.DateFrom = pMonthRange.DateFrom.AddMonths(rSourceDefinition.IntervalRepeatFrequency)
                    End If
                    pMonthRange = MonthRange(pMonthRange.DateFrom)
                    pFindDateOfMonth = Math.Min(rSourceDefinition.DateOfMonth, pMonthRange.DateTo.Day)
                    rSourceDefinition.NextRunDate = New Date(pMonthRange.DateTo.Year, pMonthRange.DateTo.Month, pFindDateOfMonth).Add(vCheckOutTime.TimeOfDay)
                End If

            Case Else
                Throw New NotImplementedException()

        End Select

    End Sub

    Private Function CreateableDefinition() As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "  -- source for creating repeatable trans" & vbCrLf &
            "  SELECT TransRentableAssets.ID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            "  ,MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "  ,LibraryInvoicingMethod.MethodType" & vbCrLf &
            "  ,LibraryInvoicingMethod.NoOfDays" & vbCrLf &
            "  ,LibraryInvoicingMethod.MonthlyBillType" & vbCrLf &
            "  ,TransRentableAssets.CheckedInUsedOn" & vbCrLf &
            "  ,TransRentableAssets.ActualCheckedOutReturnedOn" & vbCrLf &
            "  ,TransRentableAssets.CheckedOutReturnedOn" & vbCrLf &
            "  ,LibraryRepeatingTransactions.ID AS RepeatingTransactionID" & vbCrLf &
            "  ,TransRentableAssets.IsDone" & vbCrLf &
            "  FROM TransRentableAssets" & vbCrLf &
            "  JOIN LibraryRentableAssets ON LibraryRentableAssets.ID = TransRentableAssets.AssetID" & vbCrLf &
            "  JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryRentableAssets.ProductID" & vbCrLf &
            "  JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryRentableAssets.ProductID" & vbCrLf &
            "  JOIN LibraryInvoicingMethodCategory ON LibraryInvoicingMethodCategory.Category = MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "  JOIN LibraryInvoicingMethod ON LibraryInvoicingMethod.ID = LibraryInvoicingMethodCategory.InvoicingMethodID" & vbCrLf &
            "  LEFT JOIN LibraryRepeatingTransactions ON LibraryRepeatingTransactions.SourceTransRentableAssetID = TransRentableAssets.ID" & vbCrLf &
            "  WHERE ISNULL(TransRentableAssets.IsCancelled, 0) = 0" & vbCrLf &
            "  AND TransRentableAssets.CheckedInUsedOn IS NOT NULL" & vbCrLf &
            "  AND (" & vbCrLf &
            "  	(" & vbCrLf &
            "  		1=1 -- ISNULL(TransRentableAssets.IsDone, 0) = 0" & vbCrLf &
            " 		AND LibraryRepeatingTransactions.ID IS NULL" & vbCrLf &
            "  	) " & vbCrLf &
            " -- 	OR" & vbCrLf &
            " --    (" & vbCrLf &
            " --		TransRentableAssets.IsDone = 1" & vbCrLf &
            " --	    AND LibraryRepeatingTransactions.ID IS NOT NULL" & vbCrLf &
            " --	    AND LibraryRepeatingTransactions.IsActive = 1" & vbCrLf &
            " --		AND LibraryRepeatingTransactions.DateEnd IS NULL" & vbCrLf &
            " -- 	)" & vbCrLf &
            "  )" & vbCrLf &
            "  AND TransRentableAssets.LastUpdated >= '2018/10/2'" & vbCrLf &
            "  ORDER BY TransRentableAssets.CheckedInUsedOn"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function CreateDefinition(vSourceDefinition As SourceDefinition) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("LibraryRepeatingTransactions")
        pIU("RepeatingInterval") = ReadyString(vSourceDefinition.RepeatInterval.ToString())
        pIU("IntervalRepeatFrequency") = vSourceDefinition.IntervalRepeatFrequency
        pIU("DateStart") = ReadyString(Format(vSourceDefinition.DateStart, cGlobal.FormatDateTimeSql2))
        pIU("LastRunDate") = ReadyString(Format(vSourceDefinition.LastRunDate.Value, cGlobal.FormatDateTimeSql2))
        pIU("NextRunDate") = ReadyString(Format(vSourceDefinition.NextRunDate.Value, cGlobal.FormatDateTimeSql2))
        pIU("SourceTransRentableAssetID") = ReadyString(vSourceDefinition.SourceTransRentableAssetID)
        pIU("DateOfMonth") = vSourceDefinition.DateOfMonth
        pIU("IsPrepaid") = If(vSourceDefinition.Prepaid, 1, 0)
        If vSourceDefinition.IsDone Then
            pIU("DateEnd") = "GETDATE()"
        End If
        If vSourceDefinition.RepeatingTransactionID Is Nothing Then
            pIU("ID") = "NEWID()"
            pIU("IsActive") = 1
            pSql = pIU.GetInsert()
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(vSourceDefinition.RepeatingTransactionID))
        End If
        mProData.ExecuteSQL(pSql)

    End Function

    Public Function SearchAndCreateInvoiceFromDefinitions(vCurrentOrgID As String, vCheckOutTime As Date, Optional vCurrentDate As Date? = Nothing) As Integer

        Dim pDT As DataTable
        Dim pSourceDefinition As SourceDefinition
        Dim pSuccessCount As Integer
        Dim pAffectedRecords As Integer

        If vCurrentDate Is Nothing Then
            vCurrentDate = Date.Now
        End If

        pDT = GetInvoiceableDefinitions(vCurrentOrgID, vCurrentDate)
        For Each pDR As DataRow In pDT.Rows
            mProData.BeginTransaction()
            pSourceDefinition = DataRowToSourceDefinition(pDR, vCheckOutTime, vCurrentOrgID, False, vCurrentDate)
            If pSourceDefinition.Quantity > 0 _
                OrElse pSourceDefinition.EarlyCheckIn > 0 _
                OrElse pSourceDefinition.LateCheckOut > 0 Then
                If CreateInvoice(vCurrentOrgID, pDR, pSourceDefinition) Then
                    If pSourceDefinition.Prepaid Then
                        pDR("LastRunDate") = pDR("NextRunDate")
                    End If
                    pAffectedRecords = SaveRunDate(pDR, vCheckOutTime, vCurrentOrgID, vCurrentDate)
                    mProData.CommitTransaction()
                    If pAffectedRecords > 0 Then
                        pSuccessCount += 1
                    End If
                Else
                    mProData.RollbackTransaction()
                End If
            Else
                pSourceDefinition.Quantity = 0
                pAffectedRecords = SaveRunDate(pDR, vCheckOutTime, vCurrentOrgID, vCurrentDate)
                mProData.CommitTransaction()
                If pAffectedRecords > 0 Then
                    pSuccessCount += 1
                End If
            End If
        Next pDR

        Return pSuccessCount

    End Function

    ''' <summary>
    ''' call this after creating an invoice
    ''' </summary>
    ''' <param name="vDrInvoiceableDefinition"></param>
    ''' <param name="vCurrentDate"></param>
    ''' <remarks></remarks>
    Private Function SaveRunDate(vDrInvoiceableDefinition As DataRow, vCheckOutTime As Date, vOrgID As String, Optional vCurrentDate As Date? = Nothing) As Integer

        Dim pSourceDefinition As SourceDefinition
        Dim pIU As InsertUpdate
        Dim pSql As String

        If vCurrentDate Is Nothing Then
            vCurrentDate = Date.Now
        End If

        pIU = New InsertUpdate("LibraryRepeatingTransactions")
        pSourceDefinition = DataRowToSourceDefinition(vDrInvoiceableDefinition, vCurrentDate, vOrgID, True, Nothing)
        'pSourceDefinition.LastRunDate = pSourceDefinition.NextRunDate
        If pSourceDefinition.DateEnd Is Nothing Then
            If pSourceDefinition.NextRunDate.Value > vCurrentDate.Value Then
                ' just do nothing here
                'pSourceDefinition.NextRunDate = vCurrentDate
                'pIU("IsActive") = 0
            ElseIf pSourceDefinition.Prepaid = False Then
                pSourceDefinition.LastRunDate = pSourceDefinition.NextRunDate
                SetRunDate(pSourceDefinition, vCheckOutTime)
            End If
        Else
            pIU("DateEnd") = ReadyString(Format(pSourceDefinition.DateEnd.Value, cGlobal.FormatDateTimeSql2))
        End If
        pIU("LastRunDate") = ReadyString(Format(pSourceDefinition.LastRunDate.Value, cGlobal.FormatDateTimeSql2))
        pIU("NextRunDate") = ReadyString(Format(pSourceDefinition.NextRunDate.Value, cGlobal.FormatDateTimeSql2))
        If pSourceDefinition.Prepaid Then
            If pSourceDefinition.LastRunDate.Value <= vCurrentDate Then
                pIU("NextRunDate") = ReadyString(Format(vCurrentDate.Value.Date, cGlobal.FormatDateTimeSql2))
            End If
        End If
        Dim pWhereSql = "ID = " & ReadyString(pSourceDefinition.RepeatingTransactionID) & vbCrLf &
            " AND (" & vbCrLf &
            "1=0" & vbCrLf
        For Each pItem In pIU.Items
            pWhereSql &= " OR " & pItem.Key & " <> " & pItem.Value & vbCrLf
        Next
        pWhereSql &= ")"
        pSql = pIU.GetUpdate(pWhereSql)
        Dim pAffectedRecords = mProData.ExecuteSQL(pSql)

        Return pAffectedRecords

    End Function

    Private Function DataRowToSourceDefinition(vDR As DataRow, vCheckOutTime As Date, vOrgID As String, vDontCancelInvoice As Boolean, vCurrentDate As Date?) As SourceDefinition

        Dim pOut As New SourceDefinition()
        Dim pMinimumQuantityIfZero As Integer = 1
        Dim pDays, pDays2 As GetDaysOutput

        If vCurrentDate Is Nothing Then
            vCurrentDate = Date.Now
        End If

        pOut.RepeatingTransactionID = CStr2(vDR("RepeatingTransactionID"))
        pOut.LastRunDate = vDR("LastRunDate")
        pOut.RepeatInterval = [Enum].Parse(GetType(SourceDefinition.eRepeatInterval), CStr2(vDR("RepeatingInterval")))
        pOut.IntervalRepeatFrequency = CInt2(vDR("IntervalRepeatFrequency"))
        pOut.NextRunDate = vDR("NextRunDate")
        pOut.DateOfMonth = CInt2(vDR("DateOfMonth"))
        pOut.Prepaid = CBool2(vDR("IsPrepaid"))
        pOut.MethodType = CStr2(vDR("MethodType")).Trim().ToLower()
        If IsDate(vDR("CheckedInUsedOn")) Then
            pOut.DateStart = vDR("CheckedInUsedOn")
        End If

        Dim pCheckInTimeDefault = mParent.SystemDefaults.CheckInTimeDefault
        Dim pEarlyCheckInTime = mParent.SystemDefaults.EarlyCheckInTime
        Dim pLateCheckOutTime = mParent.SystemDefaults.LateCheckOutTime
        If vDR("ActualCheckedOutReturnedOn") IsNot DBNull.Value Then
            'If CDate(vDR("ActualCheckedOutReturnedOn")) <= pOut.NextRunDate Then
            If vDontCancelInvoice = False _
                AndAlso pOut.MethodType = "uponcheckin" Then

                If String.IsNullOrWhiteSpace(pOut.RepeatingTransactionID) = False Then
                    Dim pSql = "SELECT ID" & vbCrLf &
                        " FROM TransInvoice" & vbCrLf &
                        " WHERE RepeatingTransactionID = " & ReadyString(pOut.RepeatingTransactionID)
                    Dim pInvoiceID = CStr2(mProData.ExecuteScalar(pSql))
                    If String.IsNullOrWhiteSpace(pInvoiceID) = False Then
                        Dim pError As String
                        If mParent.TransInvoice.CancelInvoice(pInvoiceID, modClient.EmployeeWebGuidID, vOrgID, pError) = False Then
                            If pError <> "Invoice was already cancelled." Then
                                If String.IsNullOrWhiteSpace(pError) = False Then
                                    Throw New ApplicationException("vbZTPNQTLB44! " & pError)
                                End If
                                Throw New ApplicationException("vbMJB58STVJT! failed")
                            End If
                        End If
                    End If
                End If

                'pDays = GetDays(vDR("CheckedInUsedOn"), vDR("ActualCheckedOutReturnedOn"), pCheckInTimeDefault, vCheckOutTime, pEarlyCheckInTime, pLateCheckOutTime)
                'pDays.EarlyCheckIn = 0
                'pDays2 = GetDays(vDR("CheckedInUsedOn"), vDR("CheckedOutReturnedOn"), pCheckInTimeDefault, vCheckOutTime, pEarlyCheckInTime, pLateCheckOutTime)
                'pDays2.EarlyCheckIn = 0
                'pOut.Quantity = pDays.NormalDays - pDays2.NormalDays
                'pMinimumQuantityIfZero = 0
            End If
            If pOut.MethodType = "uponcheckin" _
                OrElse pOut.MethodType = "uponcheckout" Then

                pDays = GetDays(vDR("CheckedInUsedOn"), vDR("ActualCheckedOutReturnedOn"), pCheckInTimeDefault, vCheckOutTime, pEarlyCheckInTime, pLateCheckOutTime)
                pOut.Quantity = pDays.NormalDays
                pMinimumQuantityIfZero = 0
            End If
            vDR("NextRunDate") = vDR("ActualCheckedOutReturnedOn")
            pOut.NextRunDate = vDR("ActualCheckedOutReturnedOn")
            If vDR("DateEnd") Is DBNull.Value Then
                pOut.DateEnd = pOut.NextRunDate
            End If
            'End If
        ElseIf pOut.Prepaid Then
            SetRunDate(pOut, New Date())
            vDR("NextRunDate") = pOut.NextRunDate
        ElseIf pOut.MethodType = "uponcheckin" Then
            pDays = GetDays(vDR("CheckedInUsedOn"), vDR("CheckedOutReturnedOn"), pCheckInTimeDefault, vCheckOutTime, pEarlyCheckInTime, pLateCheckOutTime)
            pOut.Quantity = pDays.NormalDays
            pOut.LastRunDate = vDR("CheckedOutReturnedOn")
            vDR("NextRunDate") = vDR("CheckedOutReturnedOn") ' for invoice remarks
            pOut.NextRunDate = New Date(2222, 2, 2)
        End If

        Select Case pOut.RepeatInterval
            Case SourceDefinition.eRepeatInterval.Daily
                pDays = GetDays(pOut.LastRunDate.Value, pOut.NextRunDate.Value, pCheckInTimeDefault, vCheckOutTime, pEarlyCheckInTime, pLateCheckOutTime)
                pOut.Quantity = pDays.NormalDays

            Case SourceDefinition.eRepeatInterval.Monthly
                If vDR("ActualCheckedOutReturnedOn") IsNot DBNull.Value Then
                    If pOut.NextRunDate.Value.Date < pOut.LastRunDate.Value.Date Then
                        pOut.Quantity = 0
                        Return pOut
                    Else
                        pOut.Quantity = GetMonthsBase30Days(pOut.LastRunDate.Value.Date, pOut.NextRunDate.Value.Date)
                    End If
                Else
                    pOut.Quantity = GetMonthsBase30Days(pOut.LastRunDate.Value.Date, pOut.NextRunDate.Value.Date.AddDays(-1))
                End If

        End Select

        If pOut.Quantity <= 0 Then
            pOut.Quantity = pMinimumQuantityIfZero
        End If

        If pDays IsNot Nothing Then
            pOut.EarlyCheckIn = pDays.EarlyCheckIn
            pOut.LateCheckOut = pDays.LateCheckOut
        End If

        Return pOut

    End Function

    Public Shared Function AddDays(vDateFrom As Date, vDaysToAdd As Integer, vCutOffTime As Date) As Date

        Dim pOut As Date
        Dim pDaysToAdd As Integer

        pDaysToAdd = vDaysToAdd
        If vDateFrom.TimeOfDay < vCutOffTime.TimeOfDay Then
            pDaysToAdd -= 1
        End If
        pOut = vDateFrom.Date.AddDays(pDaysToAdd).Add(vCutOffTime.TimeOfDay)

        Return pOut

    End Function

    Public Shared Function GetDays_old(vDateFrom As Date, vDateTo As Date, vCutOffTime As Date) As Decimal

        Dim pOut As Decimal
        Dim pAdditionals As Decimal = 0

        If Math.Abs((vDateTo - vDateFrom).TotalSeconds) < 0.1 Then
            Return 0
        End If

        pOut = (vDateTo - vDateFrom).TotalDays

        If Math.Abs(vCutOffTime.TimeOfDay.TotalSeconds - 0) < 0.1 Then
            pOut = Math.Ceiling(pOut)
            Return Decimal.Round(pOut, 2)
        End If

        If vDateFrom.TimeOfDay < vCutOffTime.TimeOfDay Then
            pAdditionals += 1
        End If

        If vCutOffTime.TimeOfDay < vDateTo.TimeOfDay Then
            pAdditionals += 1
        End If

        If pAdditionals = 0 Then
            pOut = Math.Ceiling(pOut)
        Else
            pOut = Math.Floor(pOut)
        End If
        pOut += pAdditionals

        Return Decimal.Round(pOut, 2)

    End Function

    Public Class GetDaysOutput
        Public NormalDays As Decimal
        Public EarlyCheckIn As Decimal
        Public LateCheckOut As Decimal
    End Class

    Public Shared Function GetDays(vDateFrom As Date, vDateTo As Date,
            vDefaultCheckIn As Date, vDefaultCheckOut As Date,
            vEarliestCheckIn As Date, vLastestCheckOut As Date) As GetDaysOutput

        Dim pOut As New GetDaysOutput

        If Math.Abs((vDateTo - vDateFrom).TotalSeconds) < 0.1 Then
            pOut.NormalDays = 1
            Return pOut
        End If

        pOut.NormalDays = (vDateTo.Date - vDateFrom.Date).TotalDays
        If vDefaultCheckIn.TimeOfDay <= vDateFrom.TimeOfDay Then
            ' checked in after the default check in time
        ElseIf vEarliestCheckIn.TimeOfDay <= vDateFrom.TimeOfDay Then
            ' checked in before the default check in time but after the earliest check in time
            pOut.EarlyCheckIn = 1
        Else
            'late checked in, do not add 1day
            'pOut.NormalDays += 1
        End If

        If vDateTo.TimeOfDay <= vDefaultCheckOut.TimeOfDay Then
            ' checked out before the default check out time
        ElseIf vDateTo.TimeOfDay <= vLastestCheckOut.TimeOfDay Then
            ' checked out after the default check out time but before the latest check out time
            pOut.LateCheckOut = 1
        Else
            'pOut.NormalDays += 1
        End If

        Return pOut

    End Function

    Public Shared Function GetMonthsBase30Days(vDateFrom As Date, vDateTo As Date) As Decimal

        Dim pOut As Decimal
        Dim pLastMonth As Date

        If vDateTo < vDateFrom Then
            Throw New ApplicationException("vDateTo (" & Format(vDateTo, cGlobal.FormatDateTimeGrid) & ") must be later than vDateFrom (" & Format(vDateFrom, cGlobal.FormatDateTimeGrid) & "). ref: vb7FVRBKDPJL!")
        End If

        If vDateFrom.Date = vDateTo.Date Then
            pOut = 1 / 30
            Return Decimal.Round(pOut, 4)
        End If

        pOut = DateDiff(DateInterval.Month, vDateFrom, vDateTo)
        If vDateFrom.Day = vDateTo.Day + 1 Then
            Return Decimal.Round(pOut, 4)
        End If

        If vDateFrom.Day < vDateTo.Day Then
            pOut += (vDateTo.Day - vDateFrom.Day + 1) / 30
            Return Decimal.Round(pOut, 4)
        End If

        If vDateTo.Day < vDateFrom.Day Then
            pLastMonth = vDateTo.AddMonths(-1)
            pLastMonth = New Date(pLastMonth.Year, pLastMonth.Month, vDateFrom.Day)
            pOut += ((vDateTo.Date - pLastMonth).TotalDays + 1) / 30 - 1
        End If

        Return Decimal.Round(pOut, 4)

    End Function

    Private Function GetInvoiceableDefinitions(vCurrentOrgID As String, Optional vCurrentDate As Date? = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        If vCurrentDate Is Nothing Then
            vCurrentDate = Date.Now
        End If

        pSql = Format(vCurrentDate.Value.Date, cGlobal.FormatDateTimeSql2)
        pSql = "  -- invoiceable definition" & vbCrLf &
            "  SELECT TransRentableAssets.ID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            "  ,MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "  ,TransRentableAssets.CheckedInUsedOn" & vbCrLf &
            "  ,LibraryRepeatingTransactions.ID AS RepeatingTransactionID" & vbCrLf &
            "  ,LibraryRepeatingTransactions.DateEnd" & vbCrLf &
            "  ,LibraryRentableAssets.ProductID" & vbCrLf &
            "  ,tblPrice.Price" & vbCrLf &
            "  ,LibraryRepeatingTransactions.LastRunDate" & vbCrLf &
            "  ,LibraryRepeatingTransactions.NextRunDate" & vbCrLf &
            "  ,LibraryRepeatingTransactions.RepeatingInterval" & vbCrLf &
            "  ,LibraryRepeatingTransactions.IntervalRepeatFrequency" & vbCrLf &
            "  ,LibraryRepeatingTransactions.IsPrepaid" & vbCrLf &
            "  ,LibraryRepeatingTransactions.DateOfMonth" & vbCrLf &
            "  ,TransRentableAssets.PriceLevelID" & vbCrLf &
            "  ,TransRentableAssets.ClientID" & vbCrLf &
            "  ,TransRentableAssets.OrgID" & vbCrLf &
            "  ,TransRentableAssets.ActualCheckedOutReturnedOn" & vbCrLf &
            "  ,TransRentableAssets.CheckedOutReturnedOn" & vbCrLf &
            "  ,LibraryInvoicingMethod.MethodType" & vbCrLf &
            "  FROM TransRentableAssets" & vbCrLf &
            "  JOIN LibraryRentableAssets ON LibraryRentableAssets.ID = TransRentableAssets.AssetID" & vbCrLf &
            "  JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryRentableAssets.ProductID" & vbCrLf &
            "  JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryRentableAssets.ProductID" & vbCrLf &
            "  JOIN LibraryRepeatingTransactions ON LibraryRepeatingTransactions.SourceTransRentableAssetID = TransRentableAssets.ID" & vbCrLf &
            "   OUTER APPLY (" & vbCrLf &
            "       SELECT TOP 1 TransInvoice.ID" & vbCrLf &
            "       FROM TransInvoice" & vbCrLf &
            "       WHERE TransInvoice.RepeatingTransactionID = LibraryRepeatingTransactions.ID" & vbCrLf &
            "       ORDER BY TransInvoice.LastUpdated DESC" & vbCrLf &
            "   ) tblInvoice" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vCurrentOrgID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID = LibraryRentableAssets.ProductID" & vbCrLf &
            "     AND TransRentableAssets.PriceLevelID = MobizLibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
            "  ) tblPrice" & vbCrLf &
            "  LEFT JOIN LibraryInvoicingMethodCategory ON LibraryInvoicingMethodCategory.Category = MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "  LEFT JOIN LibraryInvoicingMethod ON LibraryInvoicingMethod.ID = LibraryInvoicingMethodCategory.InvoicingMethodID" & vbCrLf &
            "  WHERE LibraryRepeatingTransactions.IsActive = 1" & vbCrLf &
            "  AND (CONVERT(datetime, LibraryRepeatingTransactions.NextRunDate) <= " & ReadyString(pSql) & vbCrLf &
            "        OR TransRentableAssets.ActualCheckedOutReturnedOn IS NOT NULL" & vbCrLf &
            "        OR (" & vbCrLf &
            "        		LibraryRepeatingTransactions.NextRunDate = LibraryRepeatingTransactions.LastRunDate" & vbCrLf &
            "        		AND LibraryInvoicingMethod.MethodType = 'uponcheckin'" & vbCrLf &
            "        )" & vbCrLf &
            ")" & vbCrLf &
            "   AND (" & vbCrLf &
            "   	LibraryRepeatingTransactions.DateEnd IS NULL" & vbCrLf &
            "       OR tblInvoice.ID IS NULL" & vbCrLf &
            "   )" & vbCrLf &
            "   -- AND TransRentableAssets.ID = '{0CE2D5C2-F28D-46B0-9829-DA041B0629B9}' -- for debugging only" & vbCrLf &
            "  ORDER BY TransRentableAssets.CheckedInUsedOn"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function CreateInvoice(vCurrentOrgID As String, vDrInvoiceableDefinition As DataRow, vSourceDefinition As SourceDefinition) As Boolean

        Dim pCreator As TransInvoice.Creator
        Dim pDR As DataRow
        Dim pError As String
        Dim pOut As Boolean
        Dim pNum As Integer
        Dim pCustomerID As String

        pCreator = New TransInvoice.Creator(mProData, mParent.TransInvoice)
        pDR = mParent.LibraryOrganizationFOWST.GetFirstByOrgID(vCurrentOrgID)
        If pDR Is Nothing Then
            Throw New ApplicationException("OrgID, " & vCurrentOrgID & ", has no FOWST. ref: vbVAVD6WF2S4!")
        End If
        pCreator.FowstID = CStr2(pDR("ID"))
        pDR = mParent.LibraryDevice.GetOrCreateDesktop()
        If pDR Is Nothing Then
            Throw New ApplicationException("No desktop device and cannot create one. ref: vbDFR4PQRNP5!")
        End If
        pCreator.DeviceID = CStr2(pDR("ID"))
        pCreator.UserID = modClient.EmployeeWebGuidID
        pCreator.InvoiceNumber = 0
        If CBool2(vDrInvoiceableDefinition("IsPrePaid")) _
            OrElse vDrInvoiceableDefinition("NextRunDate") > Now Then

            pCreator.InvoiceDate = Now
        Else
            pCreator.InvoiceDate = vDrInvoiceableDefinition("NextRunDate")
        End If
        mParent.TransInvoice.VerifyCounter(pCreator.DeviceID, pCreator.FowstID, pNum)
        pCreator.InvoiceNumber = pNum + 1
        pCreator.Remarks = If(CBool2(vDrInvoiceableDefinition("IsPrePaid")), "Prepaid", "Post Paid") & " Invoice for " & Format(pCreator.InvoiceDate.Value, "M/d/yyyy") & ". " & vbCrLf &
            "Coverage: " & Format(CDate(vDrInvoiceableDefinition("LastRunDate")), "M/d/yyyy h:mm tt") &
            " to " & Format(CDate(vDrInvoiceableDefinition("NextRunDate")), "M/d/yyyy h:mm tt")

        pDR = Nothing
        If vDrInvoiceableDefinition("OrgID") IsNot DBNull.Value Then
            pCustomerID = CStr2(vDrInvoiceableDefinition("OrgID"))
            pDR = mParent.LibraryOrganization.ByID(pCustomerID)
        End If
        If pDR Is Nothing Then
            If vDrInvoiceableDefinition("ClientID") IsNot DBNull.Value Then
                pCustomerID = CStr2(vDrInvoiceableDefinition("ClientID"))
                pDR = mParent.LibraryOrganization.ByID(pCustomerID)
            End If
        End If
        If pDR Is Nothing Then
            pCustomerID = mParent.SystemDefaults.CustomerOrganizationID()
        End If
        pCreator.CustomerID = pCustomerID
        pDR = mParent.LibraryOrganizationFOWST.GetOrCreateByOrg(pCreator.CustomerID, "Main")
        If pDR Is Nothing Then
            Throw New ApplicationException("OrgID, " & pCreator.CustomerID & ", has no FOWST. ref: vb3J8PRRREH2!")
        End If
        pCreator.BillingAddressID = CStr2(pDR("ID"))
        pCreator.DeliveryAddressID = CStr2(pDR("ID"))
        pCreator.RepeatingTransactionID = CStr2(vDrInvoiceableDefinition("RepeatingTransactionID"))
        pCreator.TransType = TransInvoice.SaveSalesOrderJson.eTransType.Invoice.ToString()
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
        If vSourceDefinition.Quantity > 0 Then
            pDR = pCreator.Stocks.NewRow()
            pDR("StockID") = CStr2(vDrInvoiceableDefinition("ProductID"))
            pDR("Quantity") = vSourceDefinition.Quantity
            pDR("ActualQuantity") = 1
            pDR("UnitMeasure") = "PC"
            pDR("UnitPrice") = vDrInvoiceableDefinition("Price")
            pDR("Subtotal") = vSourceDefinition.Quantity * CDec2(vDrInvoiceableDefinition("Price"))
            pDR("Cost") = CDec2(mParent.LibraryStock.GetLandedCost(CStr2(vDrInvoiceableDefinition("ProductID")), vCurrentOrgID)) * vSourceDefinition.Quantity
            pDR("PriceLevelID") = CStr2(vDrInvoiceableDefinition("PriceLevelID"))
            pCreator.Stocks.Rows.Add(pDR)
            AddProductsFromOtherCharges(pCreator.Stocks, CStr2(vDrInvoiceableDefinition("ID")), vCurrentOrgID)
        End If

        Dim pProductID As String
        Dim pDtProducts As DataTable
        Dim pPrice As Decimal

        If vSourceDefinition.EarlyCheckIn >= 1 Then
            pProductID = mParent.SystemDefaults.EarlyCheckInProductID
            If IsGuid(pProductID) Then
                pDtProducts = mParent.LibraryStock.ByIds(ReadyString(pProductID))
                If pDtProducts.Rows.Count > 0 Then
                    pPrice = mParent.LibraryStock.GetRetailPrice(pProductID, vCurrentOrgID)
                    pDR = pCreator.Stocks.NewRow()
                    pDR("StockID") = pProductID

                    pDR("Quantity") = 1
                    pDR("ActualQuantity") = 1
                    pDR("UnitMeasure") = "PC"
                    pDR("UnitPrice") = pPrice
                    pDR("Subtotal") = pPrice
                    pDR("Cost") = 0
                    pDR("PriceLevelID") = mParent.SystemDefaults.GetRetailPriceID()
                    pCreator.Stocks.Rows.Add(pDR)
                End If
            End If
        End If

        If vSourceDefinition.LateCheckOut >= 1 Then
            pProductID = mParent.SystemDefaults.LateCheckOutProductID
            If IsGuid(pProductID) Then
                pDtProducts = mParent.LibraryStock.ByIds(ReadyString(pProductID))
                If pDtProducts.Rows.Count > 0 Then
                    pPrice = mParent.LibraryStock.GetRetailPrice(pProductID, vCurrentOrgID)
                    pDR = pCreator.Stocks.NewRow()
                    pDR("StockID") = pProductID

                    pDR("Quantity") = 1
                    pDR("ActualQuantity") = 1
                    pDR("UnitMeasure") = "PC"
                    pDR("UnitPrice") = pPrice
                    pDR("Subtotal") = pPrice
                    pDR("Cost") = 0
                    pDR("PriceLevelID") = mParent.SystemDefaults.GetRetailPriceID()
                    pCreator.Stocks.Rows.Add(pDR)
                End If
            End If
        End If

        pOut = pCreator.Save(True, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Throw New ApplicationException(pError)
        End If

        Return pOut

    End Function

    Private Sub AddProductsFromOtherCharges(ByRef rDtProducts As DataTable, vTransID As String, vCurrentOrgID As String)

        Dim pDtSource = DeriveOtherCharges(vTransID)

        For Each pDrSource In pDtSource.Rows
            Dim pDrDestination = rDtProducts.NewRow
            pDrDestination("StockID") = CStr2(pDrSource("ProductID"))
            pDrDestination("Quantity") = CDec2(pDrSource("Qty"))
            pDrDestination("ActualQuantity") = CDec2(pDrSource("UnitQty"))
            pDrDestination("UnitMeasure") = CStr2(pDrSource("UnitMeasure"))
            pDrDestination("UnitPrice") = CDec2(mParent.LibraryStock.GetPrice(CStr2(pDrSource("ProductID")), 1, CStr2(pDrSource("PriceLevelID"))))
            pDrDestination("Subtotal") = CDec2(pDrSource("UnitQty")) * pDrDestination("UnitPrice") * CDec2(pDrSource("Qty"))
            pDrDestination("Cost") = CDec2(mParent.LibraryStock.GetLandedCost(CStr2(pDrSource("ProductID")), vCurrentOrgID)) * pDrDestination("ActualQuantity") * pDrDestination("Quantity")
            pDrDestination("PriceLevelID") = CStr2(pDrSource("PriceLevelID"))
            rDtProducts.Rows.Add(pDrDestination)
        Next pDrSource

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vTransID">TransRentableAssets.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DeriveOtherCharges(vTransID As String) As DataTable

        Dim pSql = " SELECT LibraryAssetOtherCharges.ProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransRentableAssetOtherCharges.Qty" & vbCrLf &
            " ,LibraryAssetOtherCharges.PriceLevelID" & vbCrLf &
            " ,LibraryAssetOtherCharges.UomID" & vbCrLf &
            " ,LibraryAssetOtherCharges.InvoicingType" & vbCrLf &
            " ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
            " FROM TransRentableAssetOtherCharges" & vbCrLf &
            " JOIN LibraryAssetOtherCharges ON LibraryAssetOtherCharges.ID = TransRentableAssetOtherCharges.OtherChargesID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryAssetOtherCharges.ProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.UnitMeasureID = LibraryAssetOtherCharges.UomID" & vbCrLf &
            " AND MobizLibraryPIMProductMeasurement.ProductID = LibraryAssetOtherCharges.ProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = LibraryAssetOtherCharges.UomID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " WHERE TransRentableAssetOtherCharges.TransRentableAssetID = " & ReadyString(vTransID)
        Dim pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Private Function GetActiveInvoiceUponCheckInInvoices(vTransRentableAssetsID As String) As DataTable
        ' TransRentableAssets.ID

        Dim pSql = " SELECT TransInvoice.ID AS InvoiceID" & vbCrLf &
            " ,LibraryRepeatingTransactions.ID AS RepeatingTransactionID" & vbCrLf &
            " FROM TransRentableAssets" & vbCrLf &
            " JOIN LibraryRentableAssets ON LibraryRentableAssets.ID = TransRentableAssets.AssetID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryRentableAssets.ProductID" & vbCrLf &
            " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryRentableAssets.ProductID" & vbCrLf &
            " JOIN LibraryRepeatingTransactions ON LibraryRepeatingTransactions.SourceTransRentableAssetID = TransRentableAssets.ID" & vbCrLf &
            " JOIN LibraryInvoicingMethodCategory ON LibraryInvoicingMethodCategory.Category = MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            " JOIN LibraryInvoicingMethod ON LibraryInvoicingMethod.ID = LibraryInvoicingMethodCategory.InvoicingMethodID" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.RepeatingTransactionID = LibraryRepeatingTransactions.ID" & vbCrLf &
            " WHERE TransRentableAssets.ID = " & ReadyString(vTransRentableAssetsID) & vbCrLf &
            " AND LibraryInvoicingMethod.MethodType = 'uponcheckin'" & vbCrLf &
            " AND ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
            " AND ISNULL(TransRentableAssets.IsDone, 0) = 0"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub HandleUponCheckInInvoices(vTransRentableAssetsID As String, vUserID As String, vOrgID As String)

        Dim pError As String
        Dim pDT = GetActiveInvoiceUponCheckInInvoices(vTransRentableAssetsID)
        For Each pDR In pDT.Rows
            If mParent.TransInvoice.CancelInvoice(CStr2(pDR("InvoiceID")), vUserID, vOrgID, pError) = False Then
                Throw New ApplicationException(pError)
            End If
            Dim pIU = New InsertUpdate("LibraryRepeatingTransactions")
            pIU("LastRunDate") = "GETDATE()"
            pIU("NextRunDate") = "GETDATE()"
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("RepeatingTransactionID")))
            mProData.ExecuteSQL(pSql)
        Next

    End Sub

    Shared mReconstructionDataLock = New Object()
    Public Sub ReconstructionData(vTransRentableAssetsID As String, vOrgID As String)

        SyncLock mReconstructionDataLock
            Try
                Dim pFilename = Path.Combine(Common.ServerPath, "settings", "AutoReconstructGuestFolio.txt")
                If IO.File.Exists(pFilename) = False Then
                    FileText(pFilename) = 0
                    Return
                End If
                Dim pFileText = FileText(pFilename)
                Dim pCount = CInt2(pFileText)
                If pCount = 0 Then
                    Return
                End If

                Dim pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM TransInvoiceItems" & vbCrLf &
                " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
                " JOIN LibraryRepeatingTransactions ON LibraryRepeatingTransactions.ID = TransInvoice.RepeatingTransactionID" & vbCrLf &
                " JOIN TransRentableAssets ON TransRentableAssets.ID = LibraryRepeatingTransactions.SourceTransRentableAssetID" & vbCrLf &
                " WHERE TransRentableAssets.ID = " & ReadyString(vTransRentableAssetsID) & vbCrLf &
                " -- AND TransInvoice.CancelledByID IS NULL"
                If CInt2(mProData.ExecuteScalar(pSql)) > 0 Then
                    Return ' no need to reconstruct data
                End If

                Handle1Org(vOrgID)
                FileText(pFilename) = pCount - 1
            Catch ex As ProSqlException
                AppendToDebug("vbAHS68JT32T! " & ex.Message & vbCrLf &
                    ex.Sql, True, ex, True)
            Catch ex As Exception
                AppendToDebug("vbB2JSLVR384! " & ex.Message, True, ex, True)
            End Try
        End SyncLock

    End Sub

    Private Class SourceDefinition
        Public RepeatingTransactionID As String ' LibraryRepeatingTransactions.ID
        Public IsDone As Boolean ' TransRentableAssets.IsDone
        Public Enum eRepeatInterval
            None
            Daily
            Weekly
            Monthly
        End Enum
        Public RepeatInterval As eRepeatInterval
        Public IntervalRepeatFrequency As Integer
        Public DateOfMonth As Integer
        Public DateOfYear As Date
        Public DateStart As Date
        Public DateEnd As Date?
        Public SourceTransRentableAssetID As String
        Public MethodType As String
        Public LastRunDate As Date?
        Public NextRunDate As Date?
        Public CheckedOut As Date? ' CheckedOutReturnedOn. not actual check out. used on uponcheckin method type
        Public Quantity As Decimal
        Public Prepaid As Boolean
        Public EarlyCheckIn As Decimal
        Public LateCheckOut As Decimal
    End Class
End Class
