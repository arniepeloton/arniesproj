Public Class Client
    Public Shared ProgName As String
    Public Const MobizDUC As String = "MobizDUC"
    Public Const InsufficientAccessRight As String = "The feature you are trying to access requires a security access which your user roles do not have. Please ask your administrator to grant you the necessary access rights via main menu -> System Administration -> Security Roles."
    Public Const NoAccess As String = "NoAccess"

    Public Shared Sub GetWithinLastWeek(ByRef oThisYear As DateRange)

        oThisYear = New DateRange(Now2.AddDays(-7), Now2)

    End Sub

    Public Shared Sub GetWithinLast2Weeks(ByRef oThisYear As DateRange)

        oThisYear = New DateRange(Now2.AddDays(-7 * 2), Now2)

    End Sub

    Public Shared Sub GetWithinLast4Weeks(ByRef oThisYear As DateRange)

        oThisYear = New DateRange(Now2.AddDays(-7 * 4), Now2)

    End Sub

    Public Shared Sub GetWithinLast3Months(ByRef oThisYear As DateRange)

        oThisYear = New DateRange(Now2.AddMonths(-3), Now2)

    End Sub

    Public Shared Sub GetWithinLast6Months(ByRef oThisYear As DateRange)

        oThisYear = New DateRange(Now2.AddMonths(-6), Now2)

    End Sub

    Public Shared Sub GetWithinLastYear(ByRef oThisYear As DateRange)

        oThisYear = New DateRange(Now2.AddYears(-1), Now2)

    End Sub

    Public Shared Sub GetWtdDates(ByRef oThisYear As DateRange,
       ByRef oLastYear As DateRange)

        oThisYear = New DateRange(Now2.AddDays(-Now2.DayOfWeek).Date, Now2.Date)
        oLastYear = New DateRange
        oLastYear.DateFrom = FirstFullWeek(Now2.Year - 1)
        oLastYear.DateFrom = oLastYear.DateFrom.AddDays((WeekNumber(Now2) - 1) * 7) ' always a sunday
        oLastYear.DateTo = oLastYear.DateFrom.AddDays(Now2.DayOfWeek) ' 0 = sunday

    End Sub

    Public Shared Sub GetMtdDates(ByRef oThisYear As DateRange,
       ByRef oLastYear As DateRange)

        oThisYear = New DateRange(
            New Date(Now2.Year, Now2.Month, 1),
            Now2.Date)
        oLastYear = New DateRange(
            New Date(Now2.Year - 1, Now2.Month, 1),
            Now2.AddYears(-1))

    End Sub

    Public Shared Sub GetQtdDates(ByRef oThisYear As DateRange,
        ByRef oLastYear As DateRange)

        oThisYear = New DateRange(
            New Date(Now2.Year, Math.Ceiling(Now2.Month / 3) * 3 - 2, 1),
            Now2.Date)
        oLastYear = New DateRange(
            New Date(Now2.Year - 1, Math.Ceiling(Now2.Month / 3) * 3 - 2, 1),
            Now2.AddYears(-1))

    End Sub

    Public Shared Sub GetYtdDates(ByRef oThisYear As DateRange,
        ByRef oLastYear As DateRange)

        oThisYear = New DateRange(
            New Date(Now2.Year, 1, 1),
            Now2.Date)
        oLastYear = New DateRange(
            New Date(Now2.Year - 1, 1, 1),
            Now2.AddYears(-1))

    End Sub

    Private Shared Function Now2() As Date

        Return Now.Date.AddDays(-1)

    End Function

    ''' <summary>
    ''' return true if ok
    ''' </summary>
    ''' <param name="oError"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CompareLocalTimeAndSqlServerTime(ByRef oError As String) As Boolean

        Return Common.WebRequest.ProData.CompareLocalTimeAndSqlServerTime(oError)
        
    End Function

    Public Shared Function IsTodayOrEalier(vText As String) As Boolean

        Return CStr2(vText).Trim().ToLower = "today or earlier"

    End Function

    Public Shared Function StringToArray(vText As String, vSegmentLength As Integer, Optional vMaxSegmentCount As Integer = Integer.MaxValue) As String()

        Dim pOut As New List(Of String)

        For pSegmentIndex = 0 To vMaxSegmentCount - 1
            If pSegmentIndex > vMaxSegmentCount - 1 Then
                pOut.Add(vText)
                Return pOut.ToArray()
            Else
                If vText.Length >= vSegmentLength Then
                    pOut.Add(vText.Substring(0, vSegmentLength))
                    vText = vText.Substring(vSegmentLength)
                Else
                    pOut.Add(vText)
                    Return pOut.ToArray()
                End If
            End If
        Next

        Return pOut.ToArray()

    End Function
End Class
