Imports ProPack
Imports ProWeb
Public Class DeviceMDMConfiguration

    Public Property ID As String
    Public Property PhoneName As String
    Public Property PhoneModel As String
    Public Property IMEI As String
    Public Property UUID As String
    Public Property OS As String
    Public Property DeviceID As String

    Public Property ConfigCode As String
    Public Property DevicePolicyProfileID As String
    Public Property ConfigValue As String

    Public Property DevicePolicyDescription As String
    Public Property DevicePolicyMostStrictID As String

    Public Property DevicePolicyProfileName As String
    Public Property OrgID As String
    Public Property favorUUID As Boolean = True

    Public Property UserName As String
    Public Property UserPassword As String
    Public Property PeopleID As String

    Public Property IsActive As String = ""
    'PolicyItem
    <HttpPost()>
    Function GetDetailsMDM() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizDUC")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ConfigID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigID"
                End With

                With .Add("ConfigCode", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigCode"
                End With

                With .Add("ConfigDesc", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigDesc"
                End With

                With .Add("ConfigValue", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigValue"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDevicePolicyItem.LastUpdate"
                End With

                With .Add("IsVoid", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDevicePolicyItem.IsVoid"
                End With

                With .Add("ConfigSetID", SqlDbType.BigInt)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigSetID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePolicyItem "
            If DevicePolicyProfileID.Length > 0 Then
                .SqlWhere = "WHERE DevicePolicyProfileID = " & ReadyString(DevicePolicyProfileID)
            End If
            .SqlOrder = "ORDER BY ConfigID asc"
            pDT = .Requery
        End With

        Return pDT
    End Function



    'PolicyItem
    <HttpPost()>
    Function GetDetailsMDM_MostStrict() As DataTable
        Common.WebRequest.ProData.ExecuteSQL("Mobiz")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.ID"
                End With

                With .Add("ConfigCode", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.ConfigCode"
                End With

                With .Add("ConfigDesc", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.ConfigDesc"
                End With

                With .Add("ConfigValue", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.ConfigValue"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.LastUpdate"
                End With

                With .Add("IsVoid", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.IsVoid"
                End With

                With .Add("DevicePolicyProfileID", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.DevicePolicyProfileID"
                End With

                With .Add("OrganizationID", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.OrganizationID"
                End With

            End With

            .SqlFrom = "FROM LibraryDevicePolicyMostStrict "
            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(IMEI)
            End If

            pDT = .Requery
        End With

        Return pDT
    End Function




    <HttpPost()>
    Function GetDevicePolicyProfileIDByDeviceUUID() As String
        Dim result = ""

        '   Common.WebRequest.ProData.ExecuteSQL("Use MobizDUC")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevice.DevicePolicyProfileID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(IMEI)
            End If

            .SqlOrder = "ORDER BY ID desc"
            pDT = .Requery
            If pDT.Rows.Count > 0 Then
                result = pDT.Rows(0).Item(0).ToString()
            End If
        End With
        Return result
    End Function





    <HttpPost()>
    Function GetDeviceListForApproval(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

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
            .SqlWhere = "WHERE IsActive = 0"
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function UpdateDevicePolicyAppointment() As String
        Dim p As New InsertUpdate("LibraryDevice")
        Dim pSql As String
        Dim strIDReturn As String

        p("DevicePolicyProfileID") = ReadyString(DevicePolicyProfileID)
        p("DevicePolicyLastUpdate") = " getDate() "
        Try
            Common.WebRequest.ProData.BeginTransaction()
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            DeviceID = strIDReturn
            Common.WebRequest.ProData.CommitTransaction()
            Return strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function


    'Device
    Function SaveDeviceInfo() As String
        Dim p As New InsertUpdate("LibraryDevice")
        Dim pSql As String
        Dim strIDReturn As String

        p("PhoneName") = ReadyString(PhoneName)
        p("PhoneModel") = ReadyString(PhoneModel)
        p("LastUpdated") = " getdate() "
        p("IMEI") = ReadyString(IMEI)
        p("UUID") = ReadyString(UUID)
        p("OS") = ReadyString(OS)
        p("IsActive") = ReadyString("0")
        ' p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length > 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

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

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function


    Function CheckDeviceUniqueIDRegistered() As Boolean
        Dim pFinder As Finder
        Dim pDTResult As Boolean = False

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

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

            If favorUUID Then
                .SqlWhere = "WHERE dev.IsActive=1 and dev.UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE dev.IsActive=1 and dev.IMEI = " & ReadyString(IMEI)
            End If


            If .Requery.Rows.Count > 0 Then
                pDTResult = True
            Else
                pDTResult = False
            End If

        End With

        Return pDTResult

    End Function


    Function IsPendingRegDeviceByID() As Boolean
        Dim pFinder As Finder
        Dim pDTResult As Boolean = False

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

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
            If favorUUID Then
                .SqlWhere = "WHERE dev.IsActive=0 and dev.UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE dev.IsActive=0 and dev.IMEI = " & ReadyString(IMEI)
            End If


            If .Requery.Rows.Count > 0 Then
                pDTResult = True
            Else
                pDTResult = False
            End If

        End With

        Return pDTResult

    End Function




    Function CheckIfPolicyItemsExist() As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result As Boolean = False
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyItem.ID"
                End With
            End With
            .SqlFrom = "FROM LibraryDevicePolicyItem "
            .SqlWhere = "WHERE DevicePolicyProfileID = " & ReadyString(DevicePolicyProfileID) _
                        & " AND ConfigCode = " & ReadyString(ConfigCode)
            .SqlOrder = "ORDER BY ConfigID asc"
            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            If Not IsDBNull(pDT.Rows(0).Item(0)) Then
                result = True
            End If
        End If
        Return result
    End Function




    <HttpPost()>
    Function GetPolicyDetailsByProfileID() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyProfileID"
                End With


                With .Add("DevicePolicyProfileName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyProfileName"
                End With

                With .Add("DevicePolicyDescription", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyDescription"
                End With

                With .Add("CreatedBy", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.CreatedBy"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDevicePolicyProfile.LastUpdate"
                End With

                With .Add("IsVoid", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDevicePolicyProfile.IsVoid"
                End With

                With .Add("UpdatedBy", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevicePolicyProfile.UpdatedBy"
                End With

                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDevicePolicyProfile.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePolicyProfile "
            If DevicePolicyProfileID.Length > 0 Then
                .SqlWhere = "WHERE dbo.LibraryDevicePolicyProfile.DevicePolicyProfileID  = " & ReadyString2(DevicePolicyProfileID)
            End If

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetPolicyDetailsByProfileIDWithKeyWords(ByVal keyword As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyProfileID"
                End With


                With .Add("DevicePolicyProfileName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyProfileName"
                End With

                With .Add("DevicePolicyDescription", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyDescription"
                End With
            End With

            If keyword.Length > 0 Then
                .SearchString = keyword
            End If
            .SqlFrom = "FROM LibraryDevicePolicyProfile "

            pDT = .Requery
        End With

        Return pDT
    End Function



    <HttpPost()>
    Function GetPolicyItemsByProfileID() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ConfigID", SqlDbType.BigInt)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigID"
                End With

                With .Add("ConfigCode", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigCode"
                End With

                With .Add("ConfigDesc", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigDesc"
                End With

                With .Add("ConfigValue", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigValue"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDevicePolicyItem.LastUpdate"
                End With

                With .Add("IsVoid", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDevicePolicyItem.IsVoid"
                End With

                With .Add("ConfigSetID", SqlDbType.BigInt)
                    .Expression = "dbo.LibraryDevicePolicyItem.ConfigSetID"
                End With

                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevicePolicyItem.DevicePolicyProfileID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePolicyItem "
            .SqlOrder = "ORDER BY ConfigID asc"
            If DevicePolicyProfileID.Length > 0 Then
                .SqlWhere = "WHERE dbo.LibraryDevicePolicyItem.DevicePolicyProfileID  = " & ReadyString2(DevicePolicyProfileID)
            End If

            pDT = .Requery
        End With

        Return pDT
    End Function



    <HttpPost()>
    Function GetPolicyProfilesLatestUpdate() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result = ""
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("LastUpdate", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.LastUpdate"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePolicyProfile"
            .SqlWhere = "WHERE dbo.LibraryDevicePolicyProfile.DevicePolicyProfileID = " & ReadyString(DevicePolicyProfileID)

            pDT = .Requery
        End With
        If pDT.Rows.Count > 0 Then
            If Not IsDBNull(pDT.Rows(0).Item(0)) Then
                result = pDT.Rows(0).Item(0)
            End If
        End If
        Return result
    End Function

    <HttpPost()>
    Function GetPolicyProfilesLatestUpdate_MostStrict() As String
        Common.WebRequest.ProData.ExecuteSQL("Mobiz")
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result = ""
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("LastUpdate", SqlDbType.VarChar) 'we user varchar so we dont need to convert
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.LastUpdate"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePolicyMostStrict"
            .SqlWhere = "WHERE LastUpdate = (SELECT MAX(LastUpdate) from LibraryDevicePolicyMostStrict)"

            pDT = .Requery
        End With
        If pDT.Rows.Count > 0 Then
            If Not IsDBNull(pDT.Rows(0).Item(0)) Then
                result = pDT.Rows(0).Item(0)
            End If
        End If
        Return result
    End Function



    
    '  <HttpPost()>
    Function GetDevicePolicyLastUpdate() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result = ""
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("DevicePolicyLastUpdate", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevice.DevicePolicyLastUpdate"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice"
            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(IMEI)
            End If



            pDT = .Requery
        End With
        If pDT.Rows.Count > 0 Then
            If Not IsDBNull(pDT.Rows(0).Item(0)) Then
                result = pDT.Rows(0).Item(0)
            End If
        End If
        Return result
    End Function


    '  <HttpPost()>
    Function GetWhiteListedApps() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result = ""
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData


            With .Fields
                With .Add("ApplicationName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With
            End With



            With .Fields
                With .Add("ApplicationLabel", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With
            End With


            With .Fields
                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Directory"
                End With
            End With



            .SqlFrom = "FROM LibraryDeviceWhiteListApps"

            pDT = .Requery
        End With

        Return pDT
    End Function



    '  <HttpPost()>
    Function GetBlockedAppsList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result = ""
        Dim customWhere = ""
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData


            With .Fields
                With .Add("ApplicationName", SqlDbType.VarChar)
                    .Expression = "ins.ApplicationLabel"
                End With
            End With



            With .Fields
                With .Add("ApplicationLabel", SqlDbType.VarChar)
                    .Expression = "ins.ApplicationLabel"
                End With
            End With


            With .Fields
                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "ins.Directory"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceInstalledApplicationsDetails ins "
            If favorUUID Then
                customWhere = " WHERE ins.DeviceUUID = " + ReadyString(UUID)
            Else
                customWhere = " WHERE ins.DeviceIMEI = " + ReadyString(IMEI)
            End If
            .SqlWhere = customWhere + " AND ins.ApplicationName not in (SELECT ApplicationName FROM LibraryDeviceWhiteListApps WHERE DevicePolicyProfileID IS NULL OR DevicePolicyProfileID = " + ReadyString(DevicePolicyProfileID) + ")"
            pDT = .Requery
        End With

        Return pDT
    End Function



    <HttpPost()>
    Function GetAllPolicyProfiles() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ID", SqlDbType.BigInt)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyProfileID"
                End With

                With .Add("DevicePolicyProfileName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevicePolicyProfile.DevicePolicyProfileName"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePolicyProfile"
            .SqlOrder = "ORDER BY DevicePolicyProfileName asc"
            .SqlWhere = "WHERE dbo.LibraryDevicePolicyProfile.IsVoid  = 0 "

            pDT = .Requery
        End With

        Return pDT
    End Function


    Function SavePolicyItem() As String
        Dim p As New InsertUpdate("LibraryDevicePolicyItem")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String

        p("ConfigCode") = ReadyString(ConfigCode)
        p("ConfigValue") = ReadyString(ConfigValue)
        p("ConfigDesc") = ReadyString(ConfigCode)
        p("ConfigID") = "0"
        p("ConfigSetID") = "0"
        p("LastUpdate") = "getDate()"
        p("IsVoid") = "0"
        p("DevicePolicyProfileID") = ReadyString(DevicePolicyProfileID)
        Try

            Common.WebRequest.ProData.BeginTransaction()

            If CheckIfPolicyItemsExist() Then
                strIDReturn = ID
                Dim conditionWhere As String = " DevicePolicyProfileID = " & ReadyString2(DevicePolicyProfileID) & " AND ConfigCode = " & ReadyString(ConfigCode)
                pSql = p.GetUpdate(conditionWhere)
            Else
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            UpdatePolicyProfileLastUpdate()
            Return strIDReturn
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
        Return strIDReturn
    End Function





    Function CheckIfMostStrictPolicyExist() As String
        Common.WebRequest.ProData.ExecuteSQL("Use Mobiz")
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim result As String = ""
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDevicePolicyMostStrict.ID"
                End With
            End With
            .SqlFrom = "FROM LibraryDevicePolicyMostStrict "
            .SqlWhere = "WHERE DeviceID = " & ReadyString(DeviceID) _
                        & " AND ConfigCode = " & ReadyString(ConfigCode)
            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            If Not IsDBNull(pDT.Rows(0).Item(0)) Then
                result = pDT.Rows(0).Item("ID").ToString()
            End If
        End If
        Return result
    End Function



    Function SavePolicyItem_MostStrict() As String


        Common.WebRequest.ProData.ExecuteSQL("Use Mobiz")
        Dim p As New InsertUpdate("LibraryDevicePolicyMostStrict")
        Dim pSql As String
        Dim strIDReturn As String

       

        p("ConfigCode") = ReadyString(ConfigCode)
        p("ConfigValue") = ReadyString(ConfigValue)
        p("ConfigDesc") = ReadyString(ConfigCode)
        p("LastUpdate") = "getDate()"
        p("IsVoid") = "0"
        p("DevicePolicyProfileID") = ReadyString(DevicePolicyProfileID)
        p("OrganizationID") = ReadyString(OrgID)
        p("UUID") = ReadyString(UUID)
        p("IMEI") = ReadyString(IMEI)
        p("DeviceID") = ReadyString(DeviceID)


        Try

            Common.WebRequest.ProData.BeginTransaction()

            If Not String.IsNullOrEmpty(DevicePolicyMostStrictID) Then
                strIDReturn = DevicePolicyMostStrictID
                Dim conditionWhere As String = " DevicePolicyProfileID = " & ReadyString(DevicePolicyProfileID) & " AND ConfigCode = " & ReadyString(ConfigCode) & " AND  DeviceID = " & ReadyString(DeviceID)
                pSql = p.GetUpdate(conditionWhere)
            Else
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()


            Return strIDReturn
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
        Return strIDReturn
    End Function


    Function GetDeviceCodesByDevicePolicyProfileID() As DataTable
        Dim result As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("IMEI", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.IMEI"
                End With
                With .Add("UUID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.UUID"
                End With
                With .Add("DEVICEID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.ID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            .SqlWhere = "WHERE dbo.LibraryDevice.DevicePolicyProfileID  = " & ReadyString(DevicePolicyProfileID)
            pDT = .Requery
          

        End With

        Return pDT


    End Function




    Function GetDeviceCodesByDeviceID() As DataTable
        Dim result As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("IMEI", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.IMEI"
                End With
                With .Add("UUID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.UUID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            .SqlWhere = "WHERE dbo.LibraryDevice.ID  = " & ReadyString(DeviceID)
            pDT = .Requery


        End With

        Return pDT


    End Function


    Function GetDevicePolicyProfileIDByDeviceID() As DataTable
        Dim result As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.DevicePolicyProfileID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            .SqlWhere = "WHERE dbo.LibraryDevice.ID  = " & ReadyString(DeviceID)
            pDT = .Requery


        End With

        Return pDT


    End Function


    Function SavePolicyProfile() As String
        Dim p As New InsertUpdate("LibraryDevicePolicyProfile")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String




        p("DevicePolicyDescription") = ReadyString(DevicePolicyDescription)
        p("DevicePolicyProfileName") = ReadyString(DevicePolicyProfileName)
        p("LastUpdate") = "getDate()"
        p("IsVoid") = "'False'"
        p("IsWhiteList") = CInt(Common.WebRequest.Data("IsWhiteList"))
        p("IsBlackList") = CInt(Common.WebRequest.Data("IsBlackList"))
        p("IsNoRestriction") = CInt(Common.WebRequest.Data("IsNoRestriction"))

        If String.IsNullOrWhiteSpace(IsActive) Then
            p("IsActive") = "'True'"
        Else
            p("IsActive") = ReadyString(IsActive)
        End If

        'p("CreatedBy") = IIf(WebSession.Instance().PeopleUserID.Trim().Length > 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        'p("UpdatedBy") = IIf(WebSession.Instance().PeopleUserID.Trim().Length > 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        Try
            ID = ""
            If (Not DevicePolicyProfileID Is Nothing) Then
                If (Not String.IsNullOrWhiteSpace(DevicePolicyProfileID)) Then
                    ID = DevicePolicyProfileID
                End If
            End If


            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("DevicePolicyProfileID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = DevicePolicyProfileID
                pSql = p.GetUpdate(" DevicePolicyProfileID = " & ReadyString2(DevicePolicyProfileID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()
            Return strIDReturn
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
        Return strIDReturn
    End Function


    Function UpdatePolicyProfileLastUpdate()
        Dim p As New InsertUpdate("LibraryDevicePolicyProfile")
        Dim pSql As String

        p("LastUpdate") = "getDate() "
        Try
            pSql = p.GetUpdate(" DevicePolicyProfileID = " & ReadyString2(DevicePolicyProfileID))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Catch ex As Exception
        End Try

    End Function



    <HttpPost()>
    Function GetPolicyProfileIDByID()
        Dim result As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryDevice.DevicePolicyProfileID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            .SqlOrder = "ORDER BY ID asc"
            .SqlWhere = "WHERE dbo.LibraryDevice.ID  = " & ReadyString2(WebRequest.Instance.Data("ID")) 'sdfa
            pDT = .Requery
            If pDT.Rows.Count > 0 Then
                result = pDT.Rows(0).Item(0).ToString()
            End If

        End With

        Return result
    End Function


    Public Function ReadyString2(ByVal str As String) As String
        If String.IsNullOrEmpty(str) Then
            Return ""
        Else
            ReadyString2 = "'" & str.ToString & "'"
        End If


    End Function




    Function GetDeviceListBySearch(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

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
                With .Add("DateEnrolled", SqlDbType.Int)
                    .Expression = "dev.DateEnrolled"
                End With
                With .Add("IsActive", SqlDbType.Int)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UserID", SqlDbType.Int)
                    .Expression = "dev.UserID"
                End With
                With .Add("LastUpdated", SqlDbType.Int)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("UserFullName", SqlDbType.Int)
                    .Expression = "dev.UserFullName"
                End With
                With .Add("DevicePolicyLastUpdate", SqlDbType.Int)
                    .Expression = "dev.DevicePolicyLastUpdate"
                End With
                With .Add("DevicePolicyProfileID", SqlDbType.Int)
                    .Expression = "dev.DevicePolicyProfileID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice dev "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
            pDT = .Requery
        End With

        Return pDT
    End Function


    Function GetDeviceList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

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
                With .Add("DateEnrolled", SqlDbType.DateTime)
                    .Expression = "dev.DateEnrolled"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UserID", SqlDbType.Int)
                    .Expression = "dev.UserID"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("UserFullName", SqlDbType.NVarChar)
                    .Expression = "dev.UserFullName"
                End With
                With .Add("DevicePolicyLastUpdate", SqlDbType.DateTime)
                    .Expression = "dev.DevicePolicyLastUpdate"
                End With
                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.DevicePolicyProfileID"
                End With
                With .Add("IsCertificateValid", SqlDbType.Bit)
                    .Expression = "dev.IsCertificateValid"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice dev "
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
            pDT = .Requery
        End With

        Return pDT
    End Function


    Function GetDeviceListByID() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

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
                With .Add("DateEnrolled", SqlDbType.Int)
                    .Expression = "dev.DateEnrolled"
                End With
                With .Add("IsActive", SqlDbType.Int)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UserID", SqlDbType.Int)
                    .Expression = "dev.UserID"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("UserFullName", SqlDbType.NVarChar)
                    .Expression = "dev.UserFullName"
                End With
                With .Add("DevicePolicyLastUpdate", SqlDbType.DateTime)
                    .Expression = "dev.DevicePolicyLastUpdate"
                End With
                With .Add("DevicePolicyProfileID", SqlDbType.Int)
                    .Expression = "dev.DevicePolicyProfileID"
                End With
                With .Add("DevicePolicyProfileName", SqlDbType.NVarChar)
                    .Expression = "dep.DevicePolicyProfileName"
                End With
              
            End With

            .SqlFrom = "FROM LibraryDevice dev LEFT JOIN LibraryDevicePolicyProfile dep ON dev.DevicePolicyProfileID = dep.DevicePolicyProfileID  "
            .SqlWhere = " WHERE dev.ID = " + ReadyString(DeviceID)
            pDT = .Requery
        End With

        Return pDT
    End Function



    Function GetDatabaseByOrgID() As String
        Common.WebRequest.ProData.ExecuteSQL("Use Mobiz")
        Dim result As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("Database", SqlDbType.VarChar)
                    .Expression = "dbo.LibrarySubscription.[Database]"
                End With
            End With

            .SqlFrom = "FROM LibrarySubscription "
            .SqlOrder = " ORDER BY ID asc "
            .SqlWhere = "WHERE dbo.LibrarySubscription.OrganizationID  = " & ReadyString(OrgID)
            pDT = .Requery
            If pDT.Rows.Count > 0 Then
                result = pDT.Rows(0).Item(0).ToString()
            End If

        End With
        Return result
    End Function



    Function CheckPeopleUserAccount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolusr.PeopleID"
                    .Searchable = False
                End With
                With .Add("Username", SqlDbType.VarChar)
                    .Expression = "ppolusr.Username"
                End With
                With .Add("UserPassword", SqlDbType.VarChar)
                    .Expression = "ppolusr.UserPassword"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleUserAccount ppolusr " &
                " INNER JOIN dbo.MobizLibraryPeople ppol ON (ppolusr.PeopleID = ppol.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)"

            .SqlWhere = " WHERE ppolusr.Username = " & ReadyString(UserName)

            pDT = .Requery

        End With

        Return pDT

    End Function



    Function SendWipeOutSignal() As String

        Dim pSql As String

        pSql = "Update LibraryDevice set ToWipeOut=1 where ID = " & ReadyString(DeviceID)

        Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function DeactivateWipeOutSignal() As String

        Dim pSql As String
        Dim sqlWhere = ""
        pSql = " Update LibraryDevice set ToWipeOut=0  "
        If favorUUID Then
            sqlWhere = "WHERE UUID = " & ReadyString(UUID)
        Else
            sqlWhere = "WHERE IMEI = " & ReadyString(IMEI)
        End If

        Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function PerformWipeOut() As Boolean
        Dim result As String = ""
        Dim pFinder As Finder
        Dim pDT As DataTable


        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("ToWipeOut", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDevice.[ToWipeOut]"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            .SqlOrder = " ORDER BY ID asc "

            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(IMEI)
            End If
            pDT = .Requery
            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Item("ToWipeOut").ToString().Trim.ToLower.Equals("true") Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

            Return False
        End With
    End Function



    

       


End Class
