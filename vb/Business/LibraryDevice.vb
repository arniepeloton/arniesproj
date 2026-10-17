Public Class LibraryDevice

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ById(vID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT IsActive" &
            ",UUID" & vbCrLf &
            ",PhoneName" & vbCrLf &
            " FROM LibraryDevice" &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ValidDeviceFowst(ByVal vDeviceID As String, ByVal vFowstID As String, ByVal vPosMode As String, ByRef oError As String) As Boolean

        Dim pDR As DataRow
        Dim pSql As String

        If CStr2(vDeviceID).Trim = "" Then
            oError = "Device ID must not be blank."
            Return False
        End If

        If IsGuid(vDeviceID) = False Then
            oError = "Device ID must be a valid GUID"
            Return False
        End If

        'additional by janine sept 10 2014 3pm
        'if pos mode is so, then it's ok not to check if the device is registered to the fowst. you can enter an 
        ' order even if your device is not assigned to that specific fowst
        ' else if pos, you really need to match the device fowst
        If vPosMode = "so" Then
            pSql = "SELECT ID" & vbCrLf &
           " FROM LibraryDeviceFowst" & vbCrLf &
           " WHERE DeviceID = " & ReadyString(vDeviceID)
        ElseIf vPosMode = "pos" Then
            ' commented 2022_06_18__11_15_22_0851
            'If CStr2(vFowstID).Trim = "" Then
            '    oError = "Fowst ID must not be blank."
            '    Return False
            'End If
            'If IsGuid(vFowstID) = False Then
            '    oError = "Fowst ID must be a valid GUID"
            '    Return False
            'End If
            ' commented 2022_06_18__11_15_22_0851

            ' commented 2022_03_03__11_09_55_8947
            'pSql = "SELECT ID" & vbCrLf &
            '    " FROM LibraryDeviceFowst" & vbCrLf &
            '    " WHERE DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            '    " AND FowstID = " & ReadyString(vFowstID)

            'pDR = mProData.ExecuteRow(pSql)
            'If pDR Is Nothing Then
            '    oError = "The specified DeviceID (" & vDeviceID & ") is not enrolled to the specified FowstID (" & vFowstID & "). Please go to System Administration > Authorized Devices. Select a device then add FOWST to it. ref: vb64CS6EQXAU!"
            '    Return False
            'Else
            'End If
            Return True
        End If
    
        Return True

    End Function

    Public Function ByUuid(vUuid As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT ID" &
            ",IsActive" &
            " FROM LibraryDevice" &
            " WHERE UUID = " & ReadyString(vUuid)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetOrCreateDesktop() As DataRow

        Dim pDR As DataRow

        pDR = GetDesktop()
        If pDR Is Nothing Then
            CreateDesktop()
            pDR = GetDesktop()
        End If
        
        Return pDR

    End Function

    ''' <summary>
    ''' get a device with blank UUID
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDesktop() As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT TOP 1 ID" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " WHERE NULLIF(LTRIM(RTRIM(LibraryDevice.UUID)), '') IS NULL" & vbCrLf &
            " ORDER BY ISNULL(IsActive, 0) DESC"
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    ''' <summary>
    ''' creates a device with blank UUID
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateDesktop() As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As String

        pIU = New InsertUpdate("LibraryDevice")
        pOut = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pOut)
        pIU("UUID") = "''"
        pIU("DeviceNo") = "ISNULL((SELECT MAX(DeviceNo) FROM LibraryDevice), 0) + 1"
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Function GetByUUID(vUUID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT TOP 1 ID" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " WHERE NULLIF(LTRIM(RTRIM(LibraryDevice.UUID)), '') = " & ReadyString(CStr2(vUUID).Trim()) & vbCrLf &
            " ORDER BY ISNULL(IsActive, 0) DESC"
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function CreateByUUID(vUUID As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As String

        pIU = New InsertUpdate("LibraryDevice")
        pOut = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pOut)
        pIU("PhoneName") = ReadyString(vUUID)
        pIU("PhoneModel") = "'auto created'"
        pIU("UUID") = ReadyString(vUUID)
        pIU("DeviceNo") = "ISNULL((SELECT MAX(DeviceNo) FROM LibraryDevice), 0) + 1"
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Function GetOrCreateByUuid(vUuid As String) As String

        Dim pDR = GetByUUID(vUuid)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

        Dim pID = CreateByUUID(vUuid)
        Return pID

    End Function

    Public Function IsDesktop(vID As String) As Boolean

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT TOP 1 ID" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " WHERE NULLIF(LTRIM(RTRIM(LibraryDevice.UUID)), '') IS NULL" & vbCrLf &
            " AND ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR IsNot Nothing

    End Function

    Public Function CommonPersonFowstCount(vPersonID As String, vUuid As String) As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = " -- common devices & fowsts for a person" & vbCrLf &
            " SELECT COUNT(*) AS Count" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " JOIN LibraryDeviceFowst ON LibraryDeviceFowst.DeviceID = LibraryDevice.ID" & vbCrLf &
            " JOIN LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployeeFOWST.FowstID = LibraryDeviceFowst.FowstID" & vbCrLf &
            " WHERE LibraryOrganizationEmployeeFOWST.PeopleID = " & ReadyString(vPersonID) & vbCrLf &
            " AND LibraryDevice.UUID = " & ReadyString(vUuid) & vbCrLf &
            " AND LibraryDevice.IsActive = 1" & vbCrLf &
            " AND LibraryDeviceFowst.IsActive = 1"
        pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    ''' <summary>
    ''' HDD = hard disk drive
    ''' </summary>
    ''' <param name="vSerial"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOrAddByHddSerial(vSerial As String) As String

        Dim pDR As DataRow
        Dim pID As String

        pDR = ByUuid(vSerial)
        If pDR Is Nothing Then
            pID = CreateErpDevice(vSerial)
        Else
            pID = CStr2(pDR("ID"))
        End If

        Return pID

    End Function

    Public Function CreateErpDevice(vSerial As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOut As String

        pIU = New InsertUpdate("LibraryDevice")
        pOut = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pOut)
        pIU("IMEI") = ReadyString("For EAS")
        pIU("DeviceNo") = "ISNULL((SELECT MAX(DeviceNo) FROM LibraryDevice), 0) + 1"
        pIU("UUID") = ReadyString(vSerial)
        pIU("IsActive") = 0
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pSql = pIU.GetInsert()
        Try
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            AppendToDebug("ref: vb5YGYCBC4BR!" & vbCrLf & ex.Message & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf &
                "Original SQL: " & pSql & vbCrLf, True, ex, True)
            Throw
        End Try

        Return pOut

    End Function

    Public Function IsDeviceReset(vUUID As String) As DataTable

        Dim pSql As String
        Dim pOut As Integer

        pSql = "SELECT TOP 1 *" & vbCrLf &
            " FROM LibraryDevice " & vbCrLf &
            " WHERE UUID LIKE " & ReadyStringWild(vUUID & "_old") & vbCrLf &
            " ORDER BY LastUpdated DESC"

        Return mProData.Execute(pSql)

    End Function

    Public Function FowstCount(vID As String) As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE DeviceID = " & ReadyString(vID) & vbCrLf &
            " AND IsActive = 1"
        pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function AddOrgFowsts(vDeviceID As String, vOrgID As String) As Integer

        Dim pDT As DataTable

        mProData.BeginTransaction()
        pDT = mParent.LibraryOrganizationFOWST.GetByOrgID(vOrgID)
        For Each pDR As DataRow In pDT.Rows
            mParent.LibraryOrganizationFOWST.AddDevice(CStr2(pDR("ID")), vDeviceID)
        Next pDR
        mProData.CommitTransaction()

    End Function

    Public Function CopyToDatabase(vID As String, vDestinationDatabase As String) As Boolean

        Dim pSql As String
        Dim pDT As DataTable

        Try
            pSql = "SELECT LibraryDevice.*" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vDestinationDatabase) & ".dbo.LibraryDevice LibraryDevice_2 ON LibraryDevice_2.ID = LibraryDevice.ID" & vbCrLf &
            " WHERE LibraryDevice.ID = " & ReadyString(vID) & vbCrLf &
            " AND LibraryDevice_2.ID IS NULL"
            pDT = mProData.Execute(pSql)
        Catch ex As Exception
            If ex.Message.ToLower().Contains("Invalid object name 'LibraryDevice'".ToLower()) Then
                Throw New ApplicationException(ex.Message & ". Current Database = " & mProData.GetCurrentDatabase() & ". ref: vbN5D54VFCNZ!", ex)
            Else
                Throw
            End If
        End Try

        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = EscapeSqlName(vDestinationDatabase) & ".dbo.LibraryDevice"
            pIU("FromFranchisee") = 1

            If CBool(pDR("IsActive")) Then
                pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM " & EscapeSqlName(vDestinationDatabase) & ".dbo.LibraryDevice" & vbCrLf &
                " WHERE UUID = " & ReadyString(pDR("UUID")) & vbCrLf &
                " AND IsActive = 1"
                If CInt2(mProData.ExecuteScalar(pSql)) > 0 Then
                    pIU("IsActive") = 0
                End If
            End If

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            Return True ' only 0 or 1 record is expected
        Next

    End Function

    Public Function ResetTransaction(vDeviceID As String, vOrgID As String, ByRef oMessage As String, ByRef newdeviceid As String) As Boolean

        oMessage = ""
        vOrgID = ReadyGuid(vOrgID)
        If vOrgID <> ReadyGuid(modClient.AbcOrgID) _
            AndAlso vOrgID <> ReadyGuid(modClient.DemoCorpOrgID) _
            AndAlso vOrgID <> ReadyGuid("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba") _
            AndAlso vOrgID <> ReadyGuid(modClient.CincoOrgID) _
            AndAlso vOrgID <> ReadyGuid("fae6abf4-1176-4f98-a705-898aeaa8a59b") Then 'Marcelino Corp

            oMessage = "Your organization is not allowed to use this feature. ref: vbXNXXJKUQEW!"
            Return False
        End If

        Dim pFowstCount = FowstCount(vDeviceID)
        If pFowstCount > 1 Then
            oMessage = "You may not reset the transactions of this device. This device has " & pFowstCount & " FOWSTs."
            Return False
        End If

        mProData.BeginTransaction()

        Dim pSql = "SELECT *" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " WHERE ID = " & ReadyString(vDeviceID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Dim deviceResetInfo As DataTable = IsDeviceReset(CStr2(pDR("UUID")))
        If deviceResetInfo.Rows.Count > 0 And vOrgID <> ReadyGuid("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba") _
            And vOrgID <> ReadyGuid(modClient.CincoOrgID) And vOrgID <> ReadyGuid("cfe39df6-8809-4973-9515-810df75aed12") Then
            oMessage = "Sorry. This device was reset on " & Format(CDate(Left(Split(deviceResetInfo.Rows(0)("UUID"), "_")(2), 10)), "MMM dd, yyyy") & ", and can only be reset once."
            Return False
        End If

        Dim pIU = New InsertUpdate("LibraryDevice")
        pIU("PhoneName") = ReadyString(CStr2(pDR("PhoneName")) & "_old_" & Format(Now, "MM-dd-yyyy-HH-mm"))
        pIU("UUID") = ReadyString(CStr2(pDR("UUID")) & "_old_" & Format(Now, "MM-dd-yyyy-HH-mm"))
        pIU("LastUpdated") = "getdate()"
        pIU("IsActive") = 0
        pSql = pIU.GetUpdate("ID = " & ReadyString(vDeviceID))
        pSql = mProData.ExecuteSQL(pSql) ' output is for debugging only

        pIU = Business.DataRowToInsertUpdate(pDR)
        pIU.TableName = "LibraryDevice"
        Dim pNewID = Guid.NewGuid.ToString()
        pIU("ID") = ReadyString(pNewID)
        pIU("DeviceNo") = "(SELECT ISNULL(MAX(DeviceNo), 0) + 1 FROM LibraryDevice)"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)
        newdeviceid = pNewID

        pSql = "SELECT *" & vbCrLf &
            " FROM LibraryDeviceFowst" &
            " WHERE DeviceID = " & ReadyString(vDeviceID) &
            " AND IsActive = 1"
        Dim pDtDeviceFowsts = mProData.Execute(pSql)
        For Each pDrDeviceFowst As DataRow In pDtDeviceFowsts.Rows
            pIU = Business.DataRowToInsertUpdate(pDrDeviceFowst)
            pIU.TableName = "LibraryDeviceFowst"
            pIU("ID") = "NEWID()"
            pIU("DeviceID") = ReadyString(pNewID)
            pIU("POSLastORNo") = 0
            pIU("POSLastSRVNo") = 0
            pIU("POSLastRefNo") = 0
            pIU("POSLastSONo") = 0
            pIU("POSLastQuotationNo") = 0
            pIU("POSLastReceivingNo") = 0
            pIU("POSLastTransferOutNo") = 0
            pIU("BomMakerLastNum") = 0
            pIU("ReceivingNum") = 0
            pIU("ReceivingTransferNum") = 0
            pIU("POSLastPONo") = 0
            pIU("OrderSlipNum") = 0
            pIU("ZReadCount") = 0
            pIU("TransNum") = 0
            pIU("TransNumDate") = "null"
            pIU("XReadCount") = 0
            pIU("LastStockTransferRequisitionNum") = 0
            pIU("ZeroOutDate") = "null"
            pIU("ZeroOutByUserID") = "null"
            'pIU("POSLastOTNo") = 0
            pIU("ZReadDate") = "null"
            pIU("DeviceFowstTransactionNumber") = 0
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pDrDeviceFowst

        pSql = "SELECT *" & vbCrLf &
            " FROM LibraryDeviceFowstPrinter" & vbCrLf &
            " WHERE DeviceID = " & ReadyString(vDeviceID)
        Dim pDtPrinter = mProData.Execute(pSql)
        For Each pDrPrinter As DataRow In pDtPrinter.Rows
            pIU = Business.DataRowToInsertUpdate(pDrPrinter)
            pIU.TableName = "LibraryDeviceFowstPrinter"
            pIU("ID") = "NEWID()"
            pIU("DeviceID") = ReadyString(pNewID)
            pSql = pIU.GetInsert()
        Next pDrPrinter

        'mProData.RollbackTransaction()
        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
        mProData.ExecuteSQL(pSql)

        mProData.CommitTransaction()

        Return True

    End Function

    Public Function Unused() As DataTable

        Dim pSql = " SELECT DISTINCT LibraryDevice.ID" & vbCrLf &
            " ,LibraryDevice.UUID" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " WHERE TransInvoice.ID IS NULL"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class