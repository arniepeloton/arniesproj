Public Class LibraryDeviceFowst
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetLastNumber(vFieldName As String, vFowstID As String, vDeviceID As String) As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = "SELECT " & EscapeSqlName(vFieldName) & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Sub UpdateLastNumber(vFieldName As String, vFowstID As String, vDeviceID As String, vValue As Integer, ByRef oUpdateFileAction As Action)

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pAffectedRecords As Integer

        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU(vFieldName) = vValue
        pSql = pIU.GetUpdate("FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID))
        pAffectedRecords = mProData.ExecuteSQL(pSql, True)

        If pAffectedRecords = 0 Then
            pIU("FowstID") = ReadyString(vFowstID)
            pIU("DeviceID") = ReadyString(vDeviceID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If CStr2(vFieldName).ToLower() = "POSLastORNo".ToLower() Then
            ' AppendToLog("vbH6GSFZD57N! vFieldName=" & vFieldName) ' commented on 2023_08_01__16_53_43_4501
            Dim pHasAction = oUpdateFileAction IsNot Nothing
            oUpdateFileAction =
                Sub()
                    Dim pFilename = Common.ServerPath & "settings\Counters\"
                    ProPack.Utility.AddDirectoryUser(pFilename)
                    pFilename &= ReadyGuid(vFowstID) & "-" & ReadyGuid(vDeviceID) & "-InvoiceNumber.txt"
                    Dim pFileCounter = CInt2(FileText(pFilename))
                    AppendToLog("vbCA2ZQP4LDK! pFileCounter=" & pFileCounter & "; vValue=" & vValue)
                    If pFileCounter < vValue Then
                        FileText(pFilename) = vValue
                    End If
                End Sub
            If pHasAction = False Then
                oUpdateFileAction.Invoke()
            End If
        End If

    End Sub

    Public Function GetZeroOut(vFowstID As String, vDeviceID As String) As DataRow

        Dim pSql = "SELECT ZeroOutDate" & vbCrLf &
            ",ZeroOutByUserID" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ByID(vID As String) As DataRow

        Dim pSql = "SELECT FowstID" & vbCrLf &
            ",DeviceID" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ByDeviceID(vDeviceID As String) As DataTable

        Dim pSql = "SELECT ID" & vbCrLf &
            ",FowstID" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ZeroOutDeviceFowstTransactions(vID As String, vUserID As String, ByRef oError As String) As Integer

        oError = Nothing

        Dim pDR = ByID(vID)
        If pDR Is Nothing Then
            oError = "Record does not exist."
            Return -1
        End If

        Dim pFowstID As String = CStr2(pDR("FowstID"))
        Dim pDeviceID As String = CStr2(pDR("DeviceID"))

        Dim pDrZeroOut As DataRow = GetZeroOut(pFowstID, pDeviceID)
        If pDrZeroOut("ZeroOutDate") IsNot DBNull.Value Then
            oError = "This Device-FOWST's transaction history was already zeroed out last " & Format(pDrZeroOut("ZeroOutDate"), cGlobal.FormatDateTime)
            Return -1
        End If

        Dim pDrFowst = mParent.LibraryOrganizationFOWST.ByID(pFowstID)
        If pDrFowst Is Nothing Then
            oError = "FOWST, " & pFowstID & ", does not exist."
            Return -1
        End If

        Dim pNewFowstName = pDrFowst("Name") & "-old-" & Format(Now, "yyyy-MM-dd")
        mProData.BeginTransaction()
        Dim pNewFowstID = mParent.LibraryOrganizationFOWST.CreateByOrgID(CStr2(pDrFowst("OrganizationID")), pNewFowstName)
        Dim pIU = New InsertUpdate("TransInvoice")
        pIU("FowstID") = ReadyString(pNewFowstID)
        Dim pSql = pIU.GetUpdate("FowstID = " & ReadyString(pFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(pDeviceID))
        Dim pAffectedCount As Integer = mProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("ZeroOutDate") = "GETDATE()"
        pIU("ZeroOutByUserID") = ReadyString(vUserID)
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
        pIU("XReadCount") = 0
        pIU("LastStockTransferRequisitionNum") = 0
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)
        mProData.CommitTransaction()

        Return pAffectedCount

    End Function

    Public Function ByDeviceFowst(vDeviceID As String, vFowstID As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            ",ZReadDate" & vbCrLf &
            ",POSRegistrationNo" & vbCrLf &
            ",IsActive" & vbCrLf &
            ",DeviceFowstTransactionNumber" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID)
        Dim pDR = mProData.ExecuteRow(pSql)
        'If pDR IsNot Nothing Then
        '    If pDR("ZReadDate") Is DBNull.Value Then
        '        pDR("ZReadDate") = New Date(2021, 6, 5)
        '    End If
        'End If

        Return pDR

    End Function

    Public Function IsAllowed(vDeviceID As String, vFowstID As String, vUserID As String, ByRef oError As String) As Boolean

        Dim pDR = ByDeviceFowst(vDeviceID, vFowstID)
        If pDR Is Nothing _
            OrElse CBool2(pDR("IsActive")) = False Then

            oError = "The current device is not allowed in the current FOWST. Please login via Mobiz Controller."
            Return False
        End If

        Dim pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM LibraryOrganizationEmployeeFOWST" & vbCrLf &
            " WHERE PeopleID = " & ReadyString(vUserID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND ISNULL(IsActive, 0) = 1"
        If CInt2(mProData.ExecuteScalar(pSql)) = 0 Then
            oError = "The current user is not allowed in the current FOWST. Please login via Mobiz Controller."
            Return False
        End If

        Return True

    End Function

    Public Function CopyToDatabase(vDeviceID As String, vDestinationDatabase As String) As Integer

        Dim pSql = "SELECT LibraryDeviceFowst.*" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vDestinationDatabase) & ".dbo.LibraryDeviceFowst LibraryDeviceFowst_2 ON LibraryDeviceFowst_2.ID = LibraryDeviceFowst.ID" & vbCrLf &
            " WHERE LibraryDeviceFowst.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND LibraryDeviceFowst_2.ID IS NULL"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = EscapeSqlName(vDestinationDatabase) & ".dbo.LibraryDeviceFowst"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

        Return pDT.Rows.Count

    End Function

    Public Sub UpdateTransactionNumber(vValue As Integer, vDeviceID As String, vFowstID As String)

        Dim pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("DeviceFowstTransactionNumber") = vValue
        Dim pSql = pIU.GetUpdate("DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID))
        Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
        Debug.WriteLine("UpdateTransactionNumber: " & pAffectedRecords)

    End Sub
End Class
