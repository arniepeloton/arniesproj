Public Class TransDeliveryLogger

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetSerial(Optional vDeliveryEquipmentId As String = Nothing,
        Optional vMaxDate As Date? = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pTop As String
        Dim pWhere As String
        Dim p As Integer

        If vDeliveryEquipmentId Is Nothing Then
            pWhere = " WHERE (TransDeliveryLoggerSerials.Reportable = 1" &
                " OR TransDeliveryLoggerSerials.IsNewSerial = 1)" &
                " AND TransDeliveryLoggerSerials.DateEmailed IS NULL" &
                " ORDER BY TransDeliveryLoggerSerials.LastUpdated"
        Else
            pTop = "TOP 1 "
            pWhere = " WHERE TransDeliveryLoggerSerials.LastUpdated < " & ReadyString(vMaxDate) &
                " AND TransDeliveryLogger.DeliveryEquipmentID = " & ReadyString(vDeliveryEquipmentId) &
                " ORDER BY TransDeliveryLoggerSerials.LastUpdated DESC"
        End If

        pSql = "SELECT " & pTop & "TransDeliveryLoggerSerials.ID" &
            ",TransDeliveryLogger.DeliveryEquipmentID" &
            ",TransDeliveryLoggerSerials.Serial" &
            ",TransDeliveryLoggerSerials.IsNewSerial" &
            ",TransDeliveryLoggerSerials.Tampered" &
            ",TransDeliveryLoggerSerials.LastUpdated" &
            ",TransDeliveryLoggerSerials.LowerFuel" &
            ",LibraryDeliveryEquipments.VehicleType" &
            ",LibraryDeliveryEquipments.Model" &
            ",LibraryDeliveryEquipments.PlateNumber" &
            ",emp.LastName + ', ' + emp.FirstName + ' ' + emp.MiddleName AS FullName" &
            ",tblDriver.LastName + ', ' + tblDriver.FirstName + ' ' + tblDriver.MiddleName AS DriverName"
        For p = 1 To 6
            pSql = pSql & ",tblCrew" & p & ".LastName + ', ' + tblCrew" & p & ".FirstName + ' ' + tblCrew" & p & ".MiddleName AS Crew" & p & "Name"
        Next p
        For p = 1 To 3
            pSql = pSql & ",tblChecker" & p & ".LastName + ', ' + tblChecker" & p & ".FirstName + ' ' + tblChecker" & p & ".MiddleName AS Checker" & p & "Name"
        Next p

        pSql &= " FROM TransDeliveryLoggerSerials" &
            " LEFT JOIN TransDeliveryLogger ON TransDeliveryLogger.ID = DeliveryLoggerId" &
            " LEFT JOIN LibraryDeliveryEquipments ON LibraryDeliveryEquipments.ID = TransDeliveryLogger.DeliveryEquipmentID" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles emp ON emp.ID = TransDeliveryLoggerSerials.UserId" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblDriver ON tblDriver.ID = TransDeliveryLogger.DriverID"
        For p = 1 To 6
            pSql = pSql & " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblCrew" & p & _
                " ON tblCrew" & p & ".ID = TransDeliveryLogger.Crew" & p & "ID"
        Next p
        For p = 1 To 3
            pSql = pSql & " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblChecker" & p & _
                " ON tblChecker" & p & ".ID = TransDeliveryLogger.Checker" & p & "ID"
        Next p
        pSql &= pWhere
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ComposeMessage(vCurrent As DataRow, vPrevious As DataTable) As String

        Dim pStr As String
        Dim p As Integer

        If CDec2(vCurrent("LowerFuel")) <> 0 Then
            pStr = "Fuel is substantially lower than dipstick benchmark. "
        End If
        If vCurrent("Tampered") Then
            pStr &= "The fuel inlet seal has been tampered." & vbCrLf
        ElseIf vCurrent("IsNewSerial") Then
            pStr &= "A new fuel inlet seal has been installed." & vbCrLf
        ElseIf vPrevious.Rows.Count > 0 _
            AndAlso vCurrent("Serial") <> vPrevious.Rows(0)("Serial") Then

            pStr &= "The current fuel inlet serial number does not match the previous one." & vbCrLf
        Else
            ' normal
        End If

        pStr &= vbCrLf

        pStr &= "Current fuel inlet serial number: " & vCurrent("Serial") & vbCrLf
        If vPrevious.Rows.Count > 0 Then
            pStr &= "Previous fuel inlet serial number: " & vPrevious.Rows(0)("Serial") & vbCrLf
        End If

        pStr &= "Tampered (current): " & If(vCurrent("Tampered"), "Yes", "No") & vbCrLf
        If vPrevious.Rows.Count > 0 Then
            pStr &= "Tampered (previous): " & If(vPrevious.Rows(0)("Tampered"), "Yes", "No") & vbCrLf
        End If

        pStr &= "Date encoded (current): " & vCurrent("LastUpdated") & vbCrLf
        If vPrevious.Rows.Count > 0 Then
            pStr &= "Date encoded (previous): " & vPrevious.Rows(0)("LastUpdated") & vbCrLf
        End If

        pStr &= "Encoded by (current): " & vCurrent("FullName") & vbCrLf
        If vPrevious.Rows.Count > 0 Then
            pStr &= "Encoded by (previous): " & vPrevious.Rows(0)("FullName") & vbCrLf
        End If

        pStr &= "Delivery equipment model: " & vCurrent("Model") & vbCrLf
        pStr &= "Delivery equipment type: " & vCurrent("VehicleType") & vbCrLf
        pStr &= "Delivery equipment plate number: " & vCurrent("PlateNumber") & vbCrLf

        pStr &= "Current driver: " & vCurrent("DriverName") & vbCrLf
        If vPrevious.Rows.Count > 0 Then
            pStr &= "Previous driver: " & vPrevious.Rows(0)("DriverName") & vbCrLf
        End If

        For p = 1 To 6
            pStr &= "Current crew #" & p & ": " & vCurrent("Crew" & p & "Name") & vbCrLf
        Next p
        If vPrevious.Rows.Count > 0 Then
            For p = 1 To 6
                pStr &= "Previous crew #" & p & ": " & vPrevious.Rows(0)("Crew" & p & "Name") & vbCrLf
            Next p
        End If

        For p = 1 To 3
            pStr &= "Current checker #" & p & ": " & vCurrent("Checker" & p & "Name") & vbCrLf
        Next p
        If vPrevious.Rows.Count > 0 Then
            For p = 1 To 3
                pStr &= "Previous checker #" & p & ": " & vPrevious.Rows(0)("Checker" & p & "Name") & vbCrLf
            Next p
        End If

        pStr &= vbCrLf
        pStr &= "Please do not reply to this email." & vbCrLf
        pStr &= "Thank you." & vbCrLf

        Return pStr

    End Function

    Public Function Cache() As Integer

        Dim pDtUncached As DataTable
        Dim pDrUncached As DataRow
        Dim pDtPrevious As DataTable

        'With New Synonyms()
        '    .ProData = mProData
        '    .SourceDatabase = "Drp"
        '    .Generate()
        'End With

        pDtUncached = GetUncached()
        For Each pDrUncached In pDtUncached.Rows
            pDtPrevious = GetSerial(CStr2(pDrUncached("DeliveryEquipmentID")), pDrUncached("LastUpdated"))
            SaveCache(pDrUncached, pDtPrevious)
        Next pDrUncached

        Return pDtUncached.Rows.Count

    End Function

    Private Sub SaveCache(vCurrent As DataRow, vPrevious As DataTable)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim p As Integer

        pIU = New InsertUpdate("DrpCacheDeliveryLogger")
        pIU("DeliverLoggerSerialId") = ReadyString(vCurrent("ID"))

        If vCurrent("Tampered") Then
            pIU("Reason") = "'tampered'"
        ElseIf vCurrent("IsNewSerial") Then
            pIU("Reason") = "'new seal'"
        ElseIf vPrevious.Rows.Count > 0 _
            AndAlso vCurrent("Serial") <> vPrevious.Rows(0)("Serial") Then

            pIU("Reason") = "'serial mismatch'"
        Else
            pIU("Reason") = "'normal'"
        End If

        pIU("SerialCurrent") = ReadyString(vCurrent("Serial"))
        If vPrevious.Rows.Count > 0 Then
            pIU("SerialPrevious") = ReadyString(vPrevious.Rows(0)("Serial"))
        End If

        pIU("TamperedCurrent") = CDec2(vCurrent("Tampered"))
        If vPrevious.Rows.Count > 0 Then
            pIU("TamperedPrevious") = CDec2(vPrevious.Rows(0)("Tampered"))
        End If

        pIU("LastUpdatedCurrent") = ReadyString(vCurrent("LastUpdated"))
        If vPrevious.Rows.Count > 0 Then
            pIU("LastUpdatedPrevious") = ReadyString(vPrevious.Rows(0)("LastUpdated"))
        End If

        pIU("UserCurrent") = ReadyString(vCurrent("FullName"))
        If vPrevious.Rows.Count > 0 Then
            pIU("UserPrevious") = ReadyString(vPrevious.Rows(0)("FullName"))
        End If

        pIU("Model") = ReadyString(vCurrent("Model"))
        pIU("VehicleType") = ReadyString(vCurrent("VehicleType"))
        pIU("PlateNumber") = ReadyString(vCurrent("PlateNumber"))

        pIU("DriverCurrent") = ReadyString(vCurrent("DriverName"))
        If vPrevious.Rows.Count > 0 Then
            pIU("DriverPrevious") = ReadyString(vPrevious.Rows(0)("DriverName"))
        End If

        For p = 1 To 6
            pIU("Crew" & p & "Current") = ReadyString(vCurrent("Crew" & p & "Name"))
        Next p
        If vPrevious.Rows.Count > 0 Then
            For p = 1 To 6
                pIU("Crew" & p & "Previous") = ReadyString(vPrevious.Rows(0)("Crew" & p & "Name"))
            Next p
        End If

        For p = 1 To 3
            pIU("Checker" & p & "Current") = ReadyString(vCurrent("Checker" & p & "Name"))
        Next p
        If vPrevious.Rows.Count > 0 Then
            For p = 1 To 3
                pIU("Checker" & p & "Previous") = ReadyString(vPrevious.Rows(0)("Checker" & p & "Name"))
            Next p
        End If

        pIU("LowerFuelCurrent") = CDec2(vCurrent("LowerFuel"))
        If vPrevious.Rows.Count > 0 Then
            pIU("LowerFuelPrevious") = CDec2(vPrevious.Rows(0)("LowerFuel"))
        End If

        psql = pIU.GetInsert
        mProData.ExecuteSQL(psql)

    End Sub

    Private Function GetUncached() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim p As Integer

        pSql = "SELECT TransDeliveryLoggerSerials.ID"
        pSql &= ",TransDeliveryLogger.DeliveryEquipmentID" &
            ",TransDeliveryLoggerSerials.Serial" &
            ",TransDeliveryLoggerSerials.IsNewSerial" &
            ",TransDeliveryLoggerSerials.Tampered" &
            ",TransDeliveryLoggerSerials.LastUpdated" &
            ",TransDeliveryLoggerSerials.LowerFuel" &
            ",LibraryDeliveryEquipments.VehicleType" &
            ",LibraryDeliveryEquipments.Model" &
            ",LibraryDeliveryEquipments.PlateNumber" &
            ",emp.LastName + ', ' + emp.FirstName + ' ' + emp.MiddleName AS FullName" &
            ",tblDriver.LastName + ', ' + tblDriver.FirstName + ' ' + tblDriver.MiddleName AS DriverName"
        For p = 1 To 6
            pSql = pSql & ",tblCrew" & p & ".LastName + ', ' + tblCrew" & p & ".FirstName + ' ' + tblCrew" & p & ".MiddleName AS Crew" & p & "Name"
        Next p
        For p = 1 To 3
            pSql = pSql & ",tblChecker" & p & ".LastName + ', ' + tblChecker" & p & ".FirstName + ' ' + tblChecker" & p & ".MiddleName AS Checker" & p & "Name"
        Next p

        pSql &= " FROM TransDeliveryLoggerSerials" &
            " LEFT JOIN TransDeliveryLogger ON TransDeliveryLogger.ID = DeliveryLoggerId" &
            " LEFT JOIN LibraryDeliveryEquipments ON LibraryDeliveryEquipments.ID = TransDeliveryLogger.DeliveryEquipmentID" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles emp ON emp.ID = TransDeliveryLoggerSerials.UserId" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblDriver ON tblDriver.ID = TransDeliveryLogger.DriverID"
        For p = 1 To 6
            pSql = pSql & " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblCrew" & p & _
                " ON tblCrew" & p & ".ID = TransDeliveryLogger.Crew" & p & "ID"
        Next p
        For p = 1 To 3
            pSql = pSql & " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblChecker" & p & _
                " ON tblChecker" & p & ".ID = TransDeliveryLogger.Checker" & p & "ID"
        Next p
        pSql &= " LEFT JOIN DrpCacheDeliveryLogger ON DrpCacheDeliveryLogger.DeliverLoggerSerialId = TransDeliveryLoggerSerials.ID" &
            " WHERE DrpCacheDeliveryLogger.DeliverLoggerSerialId IS NULL"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class
