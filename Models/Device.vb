Public Class Device

    Public Property ID As String
    Public Property DeviceID As String
    Public Property MaxReturnRow As Integer = 50

    Public Property PhoneName As String
    Public Property PhoneModel As String
    Public Property IMEI As String
    Public Property UUID As String
    Public Property UuidDisplay As String
    Public Property TinDisplay As String
    Public Property OS As String
    Public Property Accreditation As String
    Public Property MobizAppVersion As String
    Public Property BusinessName As String
    Public Property BusinessRegisteredName As String
    Public Property IsActive As Integer

    Public Property FowstID As String
    Public Property POSRegistrationNo As String
    Public Property POSPTUDateIssued As Date
    Public Property BIRMachineNo As String
    Public Property DevFowstPrinterIP As String
    Public Property SMMachineNo As String
    Public Property TerminalNo As String
    Public Property BegBalDate As String
    Public Property BegBalance As String
    Public Property BegBalSetDate As String
    Public Property POSLastORNo As String
    Public Property POSLastSRVNo As String
    Public Property POSLastRefNo As String
    Public Property IsBeginningBalSet As String

    Public Property IsAuthorizedForPOS As Boolean
    Public Property ReasonForNonRegistration As Integer
    Public Property ApplicationReferenceNo As String
    Public Property ApplicationDate As String
    Public Property ShowCompanyDetailsInHeader As Boolean

    Public Property PrinterID As String
    Public Property PrinterFowstID As String
    Public Property PrintServer As String

    Public ProData As ProData

    Function DeactivateDevice() As String
        Dim pSql As String

        pSql = "Update LibraryDevice set IsActive=0 where ID = " & ReadyString(WebSession.Instance.DeviceID)

        Common.WebRequest.ProData.Execute(pSql)

    End Function

    Function CheckDeviceUniquePhoneName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice dev "

            .SqlWhere = "WHERE dev.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND dev.PhoneName = " & ReadyString(PhoneName) &
                        " AND ISNULL(dev.IsActive, 0) = 1"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function CheckDeviceUnique() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice dev "

            .SqlWhere = "WHERE dev.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND dev.UUID = " & ReadyString(UUID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getDeviceList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice dev "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getDeviceInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("UuidDisplay", SqlDbType.VarChar)
                    .Expression = "dev.UuidDisplay"
                End With
                With .Add("TinDisplay", SqlDbType.VarChar)
                    .Expression = "dev.TinDisplay"
                End With
                With .Add("Accreditation", SqlDbType.VarChar)
                    .Expression = "dev.Accreditation"
                End With
                With .Add("AccreditationText", SqlDbType.VarChar)
                    .Expression = " CASE Accreditation" & vbCrLf &
                        " WHEN 'duc' THEN 'Davao Unified Corporation'" & vbCrLf &
                        " ELSE '1MOBIZ, INC.' " & vbCrLf &
                        " END"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("BusinessName", SqlDbType.VarChar)
                    .Expression = "dev.BusinessName"
                End With
                With .Add("BusinessRegisteredName", SqlDbType.VarChar)
                    .Expression = "dev.BusinessRegisteredName"
                End With
                With .Add("MobizAppVersion", SqlDbType.VarChar)
                    .Expression = "dev.MobizAppVersion"
                End With
                .Add("IsActive", SqlDbType.Bit)
            End With

            .SqlFrom = " FROM LibraryDevice dev "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE dev.ID IS NULL", " WHERE dev.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveDeviceInfo() As String
        Dim p As New InsertUpdate("LibraryDevice")
        Dim pSql As String
        Dim strIDReturn As String

        p("PhoneName") = ReadyString(PhoneName)
        p("PhoneModel") = ReadyString(PhoneModel)
        p("IMEI") = ReadyString(IMEI)
        p("UUID") = ReadyString(UUID)
        If String.IsNullOrWhiteSpace(UuidDisplay) Then
            p("UuidDisplay") = "NULL"
        Else
            p("UuidDisplay") = ReadyString(UuidDisplay)
        End If
        If String.IsNullOrWhiteSpace(TinDisplay) Then
            p("TinDisplay") = "NULL"
        Else
            p("TinDisplay") = ReadyString(TinDisplay)
        End If
        p("OS") = ReadyString(OS)
        p("Accreditation") = ReadyString(Accreditation)
        p("MobizAppVersion") = ReadyString(MobizAppVersion)
        p("PeopleID") = If(CStr2(WebSession.Instance().PeopleUserID).Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("IsActive") = IsActive
        p("LastUpdated") = "GetDate()"
        p("BusinessName") = IIf(String.IsNullOrWhiteSpace(BusinessName), "NULL", ReadyString(BusinessName))
        p("BusinessRegisteredName") = IIf(String.IsNullOrWhiteSpace(BusinessRegisteredName), "NULL", ReadyString(BusinessRegisteredName))

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then

                p("DeviceNo") = " ISNULL((SELECT MAX(DeviceNo) FROM LibraryDevice),0) + 1"

                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            DeviceID = strIDReturn

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'for mobiz trax
            Dim dtTrax As DataTable
            
            pSql = "Select * from MobizTransDeviceOrganizationPairing where OrgID = " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND UUID = " & ReadyString(UUID)
            dtTrax = Common.WebRequest.ProData.Execute(pSql)

            If dtTrax.Rows.Count = 0 Then
                pSql = "Insert into MobizTransDeviceOrganizationPairing(OrgID, UUID) values(" & ReadyString(Common.WebRequest.Data("OrgID")) & ", " & ReadyString(UUID) & ")"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function

    Function CheckDeviceFowst() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.ID"
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.DeviceID"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.FowstID"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowst devfowst "

            .SqlWhere = "WHERE devfowst.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND devfowst.DeviceID = " & ReadyString(DeviceID) &
                        " AND devfowst.FowstID = " & ReadyString(FowstID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getDeviceFOWSTList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.DeviceID"
                    .Searchable = False
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTNo", SqlDbType.Int)
                    .Expression = "fowst.FOWSTNo"
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "fowst.[Name]"
                End With
                With .Add("POSRegistrationNo", SqlDbType.VarChar)
                    .Expression = "devfowst.POSRegistrationNo"
                End With
                With .Add("BIRMachineNo", SqlDbType.VarChar)
                    .Expression = "devfowst.BIRMachineNo"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowst devfowst " &
                " INNER JOIN LibraryDevice dev ON (dev.ID = devfowst.DeviceID) " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = devfowst.FowstID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE devfowst.DeviceID IS NULL", " WHERE devfowst.DeviceID = " + ReadyString(strID)) &
                        " AND devfowst.IsActive = 1"

            .SqlOrder = "ORDER BY fowst.[Name] ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function RemoveDeviceFowst() As String
        Dim pSql As String

        Try
            pSql = "UPDATE LibraryDeviceFowst SET IsActive = 0 WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.Execute(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return "ok"
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Function GetPrinterNames() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "fowst.PrinterName"
                End With
                With .Add("PrinterIPAddress", SqlDbType.VarChar)
                    .Expression = "fowst.PrinterIPAddress"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinterFowst fowst INNER JOIN LibraryDevicePrinter pr ON (pr.ID = fowst.PrinterID)"

            .SqlWhere = " WHERE fowst.FowstID = " & ReadyString(FowstID) '& " AND pr.PrinterRole = 'Point of Sale (POS)'"

            .SqlOrder = "ORDER BY PrinterName ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDeviceFOWSTInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim PrinterFowstID As String

        '''Check Printer Fowst if FOWST exists. this is where you get the printer name
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("PrinterFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "printer.PrinterFowstID"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceFowstPrinter printer LEFT JOIN LibraryDevicePrinterFowst fowst ON (fowst.ID = printer.PrinterFowstID)"
            .SqlWhere = " WHERE printer.DeviceID = " & ReadyString(DeviceID) & " AND fowst.FowstID = " & ReadyString(FowstID) & " AND PrinterFowstID IS NOT NULL"

            pDT = .Requery
        End With

        If (pDT.Rows.Count > 0) Then
            PrinterFowstID = pDT.Rows(0).Item("PrinterFowstID").ToString()
        Else
            PrinterFowstID = ""
        End If

        '''''''''

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.DeviceID"
                    .Searchable = False
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.FowstID"
                    .Searchable = False
                End With
                With .Add("FOWSTNo", SqlDbType.Int)
                    .Expression = "fowst.FOWSTNo"
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "fowst.[Name]"
                End With
                With .Add("PrinterFowstID", SqlDbType.VarChar)
                    .Expression = "prntrfowst.ID"
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "prntrfowst.PrinterName"
                End With
                With .Add("PrinterIPAddress", SqlDbType.VarChar)
                    .Expression = "prntrfowst.PrinterIPAddress"
                End With
                With .Add("POSRegistrationNo", SqlDbType.VarChar)
                    .Expression = "devfowst.POSRegistrationNo"
                End With
                With .Add("POSPTUDateIssued", SqlDbType.VarChar)
                    .Expression = "devfowst.PTUDateIssued"
                End With
                With .Add("BIRMachineNo", SqlDbType.VarChar)
                    .Expression = "devfowst.BIRMachineNo"
                End With
                With .Add("TerminalNo", SqlDbType.Int)
                    .Expression = "ISNULL(devfowst.TerminalNo,0)"
                End With
                With .Add("SMMachineNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(devfowst.SMMachineNo,'')"
                End With
                With .Add("BegBalDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(devfowst.BegBalDate)"
                End With
                With .Add("BegBalDateDay", SqlDbType.Int)
                    .Expression = "DAY(devfowst.BegBalDate)"
                End With
                With .Add("BegBalDateYear", SqlDbType.Int)
                    .Expression = "YEAR(devfowst.BegBalDate)"
                End With
                With .Add("BegBalance", SqlDbType.Money)
                    .Expression = "devfowst.BegBalance"
                End With
                With .Add("IsBegBalNotEditable", SqlDbType.Int)
                    .Expression = "CASE ISNULL(devfowst.BegBalSetDate,0) " &
                      "           WHEN 0 THEN " &
                      "                 0 " &
                      "           ELSE " &
                      "                 1 " &
                      "           END"
                End With
                With .Add("POSLastORNo", SqlDbType.Int)
                    .Expression = "ISNULL(devfowst.POSLastORNo,0)"
                End With
                With .Add("POSLastSRVNo", SqlDbType.Int)
                    .Expression = "ISNULL(devfowst.POSLastSRVNo,0)"
                End With
                With .Add("POSLastRefNo", SqlDbType.Int)
                    .Expression = "ISNULL(devfowst.POSLastRefNo,0)"
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "fowst.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "fowst.FOWSTMallName"
                End With
                '.Add("PrintServer", SqlDbType.VarChar, "devfowst.PrintServer")
                With .Add("IsAuthorizedForPOS", SqlDbType.Bit)
                    .Expression = "devfowst.IsAuthorizedForPOS"
                End With
                With .Add("ReasonForNonRegistration", SqlDbType.Int)
                    .Expression = "devfowst.ReasonForNonRegistration"
                End With
                With .Add("ApplicationReferenceNo", SqlDbType.VarChar)
                    .Expression = "devfowst.ApplicationReferenceNo"
                End With
                With .Add("ApplicationDate", SqlDbType.DateTime2)
                    .Expression = "devfowst.ApplicationDate"
                End With
                With .Add("ShowCompanyDetailsInHeader", SqlDbType.Bit)
                    .Expression = "devfowst.ShowCompanyDetailsInHeader"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowst devfowst " &
                " LEFT JOIN LibraryDevice dev ON (dev.ID = devfowst.DeviceID) " &
                " LEFT JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = devfowst.FowstID) " &
                " LEFT JOIN LibraryDeviceFowstPrinter fowstprntr ON (dev.ID = fowstprntr.DeviceID) " &
                " LEFT JOIN LibraryDevicePrinterFowst prntrfowst ON (fowstprntr.PrinterFowstID = prntrfowst.ID)"


            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE devfowst.ID IS NULL ", " WHERE devfowst.ID = " & ReadyString(strID)) &
                 IIf(FowstID.Trim().Length = 0 Or PrinterFowstID.Trim().Length = 0, " ", " AND devfowst.FowstID = prntrfowst.FowstID ") & " AND " &
                 IIf(PrinterFowstID.Trim().Length = 0, " prntrfowst.ID IS NULL ", " prntrfowst.ID = " & ReadyString(PrinterFowstID))

            .SqlOrder = "ORDER BY fowst.[Name] ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveDeviceFOWST()
        Dim p As New InsertUpdate("LibraryDeviceFowst")
        Dim pSql As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        Try
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "devfowst.ID"
                        .Searchable = False
                    End With
                    With .Add("IsBegBalEditable", SqlDbType.Int)
                        .Expression = "CASE ISNULL(devfowst.BegBalSetDate,0) " &
                          "           WHEN 0 THEN " &
                          "                 1 " &
                          "           ELSE " &
                          "                 0 " &
                          "           END"
                    End With
                End With

                .SqlFrom = " FROM dbo.LibraryDeviceFowst devfowst "
                .SqlWhere = IIf(ID.Trim().Length = 0, " WHERE devfowst.ID IS NULL", " WHERE devfowst.ID  = " & ReadyString(ID))

                pDT = .Requery

            End With


            p("DeviceID") = ReadyString(DeviceID)
            p("FowstID") = ReadyString(FowstID)

            p("POSRegistrationNo") = ReadyString(POSRegistrationNo)
            p("PTUDateIssued") = ReadyString(POSPTUDateIssued)
            p("BIRMachineNo") = ReadyString(BIRMachineNo)
            p("SMMachineNo") = ReadyString(SMMachineNo)
            p("IsActive") = 1
            p("PrintServer") = ReadyString(PrintServer)

            If (pDT.Rows.Count > 0) Then
                If (BegBalDate.Trim().Length > 0) Then
                    If (Convert.ToInt16(pDT.Rows(0).Item("IsBegBalEditable")) = 1) Then
                        p("BegBalDate") = ReadyString(BegBalDate)
                        p("BegBalance") = BegBalance
                        p("BegBalSetDate") = "getdate()"
                        p("POSLastORNo") = POSLastORNo
                        p("POSLastSRVNo") = POSLastSRVNo
                        p("POSLastRefNo") = POSLastRefNo
                    End If
                End If
            Else
                If (BegBalDate.Trim().Length > 0) Then
                    p("BegBalDate") = ReadyString(BegBalDate)
                    p("BegBalance") = BegBalance
                    p("BegBalSetDate") = "getdate()"
                    p("POSLastORNo") = POSLastORNo
                    p("POSLastSRVNo") = POSLastSRVNo
                    p("POSLastRefNo") = POSLastRefNo
                End If
            End If

            p("IsAuthorizedForPOS") = CInt(IsAuthorizedForPOS)
            p("ReasonForNonRegistration") = ReasonForNonRegistration
            p("ApplicationReferenceNo") = ReadyString(ApplicationReferenceNo)
            p("ApplicationDate") = ReadyString(ApplicationDate)
            p("ShowCompanyDetailsInHeader") = CInt(ShowCompanyDetailsInHeader)

            If (ID.ToString().Trim().Length <= 0) Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())

                p("TerminalNo") = "ISNULL((SELECT MAX(TerminalNo) FROM LibraryDeviceFowst WHERE FowstID = " & ReadyString(FowstID) & "),0) + 1" 'DeviceID = " & ReadyString(DeviceID) & " AND (i think wala dapat labot ang deviceid, since need icount ang kung pila ang POS per branch.)

                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(ID).ToString())
            End If

            Common.WebRequest.ProData.BeginTransaction()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Check Printer Fowst if exists
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "fowstprntr.ID"
                        .Searchable = False
                    End With
                End With

                .SqlFrom = " FROM LibraryDeviceFowstPrinter fowstprntr INNER JOIN LibraryDevicePrinterFowst prntrfowst ON (prntrfowst.ID = fowstprntr.PrinterFowstID)"
                .SqlWhere = " WHERE fowstprntr.DeviceID = " & ReadyString(DeviceID) & " AND prntrfowst.FowstID = " & ReadyString(FowstID)

                pDT = .Requery
            End With

            p = New InsertUpdate("dbo.LibraryDeviceFowstPrinter")
            If (PrinterFowstID.Length > 0) Then
                p("PrinterFowstID") = ReadyString(PrinterFowstID)
            Else
                p("PrinterFowstID") = "NULL"
            End If

            p("DeviceID") = ReadyString(DeviceID)
            p("IsActive") = "1"
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "getdate()"

            If (pDT.Rows.Count > 0) Then
                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
            Else
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update LibraryDevice Set LastUpdated = GetDate() Where ID = " & ReadyString(DeviceID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()


        Catch ex As Exception

            Common.WebRequest.ProData.RollbackTransaction()

        End Try
    End Function

    Function CheckUniqueDevicePrinter() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntrdev.ID"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceFowstPrinter prntrdev "

            .SqlWhere = "WHERE prntrdev.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND prntrdev.DeviceID = " & ReadyString(WebSession.Instance().DeviceID) &
                        " AND prntrdev.PrinterID = " & ReadyString(PrinterID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getDevicePrinterList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntrdev.ID"
                    .Searchable = False
                End With
                With .Add("PrinterID", SqlDbType.VarChar)
                    .Expression = "prntrdev.PrinterID"
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "devprntr.PrinterName"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "devprntr.PrinterType"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "devprntr.FowstID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("IPAddress", SqlDbType.VarChar)
                    .Expression = "devprntr.IPAddress"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "devprntr.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowstPrinter prntrdev " &
                " INNER JOIN LibraryDevicePrinter devprntr ON (prntrdev.PrinterID = devprntr.ID) " &
                " INNER JOIN MobizLibraryOrganizationFowst fowst ON (fowst.ID = devprntr.FowstID) "

            .SqlWhere = " WHERE prntrdev.DeviceID = " & ReadyString(WebSession.Instance().DeviceID)

            .SearchString = strKeyWord

            .SqlOrder = "ORDER BY devprntr.PrinterName ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDevicePrinterInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntrdev.ID"
                    .Searchable = False
                End With
                With .Add("PrinterID", SqlDbType.VarChar)
                    .Expression = "prntrdev.PrinterID"
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "devprntr.PrinterName"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "devprntr.PrinterType"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "devprntr.FowstID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("IPAddress", SqlDbType.VarChar)
                    .Expression = "devprntr.IPAddress"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "devprntr.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowstPrinter prntrdev " &
                " INNER JOIN LibraryDevicePrinter devprntr ON (prntrdev.PrinterID = devprntr.ID) " &
                " INNER JOIN MobizLibraryOrganizationFowst fowst ON (fowst.ID = devprntr.FowstID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prntrdev.ID IS NULL", " WHERE prntrdev.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveDevicePrinterInfo() As String
        Dim p As New InsertUpdate("LibraryDeviceFowstPrinter")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("DeviceID") = IIf(DeviceID.Trim().Length <= 0, "NULL", ReadyString(DeviceID))
        p("PrinterID") = IIf(PrinterID.Trim().Length <= 0, "NULL", ReadyString(PrinterID))
        p("IsActive") = IsActive
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn
    End Function

    Function GetLibraryDevicePrinter() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "dev.PrinterType"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.UserID"
                End With
                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("PrinterRole", SqlDbType.VarChar)
                    .Expression = "dev.PrinterRole"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinter dev "
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetLibraryDeviceFowstPrinter() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.VarChar)
                    .Expression = "dev.DeviceID"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.UserID"
                End With
                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("PrinterFowstID", SqlDbType.VarChar)
                    .Expression = "dev.PrinterFowstID"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowstPrinter dev "
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetLibraryDevicePrinterFowst() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.VarChar)
                    .Expression = "dev.FowstID"
                End With
                With .Add("PrinterID", SqlDbType.Bit)
                    .Expression = "dev.PrinterID"
                End With
                With .Add("PrinterIPAddress", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.PrinterIPAddress"
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "dev.PrinterName"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinterFowst dev "
            pDT = .Requery
        End With

        Return pDT
    End Function

End Class
