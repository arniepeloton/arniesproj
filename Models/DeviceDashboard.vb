<Serializable()> _
Public Class DeviceDashboard
    Public Property ID As String = ""
    Public Property ApplicationName As String
    Public Property Directory As String
    Public Property ApplicationInfo As String
    Public Property UID As String
    Public Property ApplicationLabel As String = ""
    Public Property ApplicationDescription As String = ""
    Public Property DeviceAppID As String
    Public Property DeviceUUID As String
    Public Property DataBaseName As String
    Public Property DeviceIMEI As String
    Public Property favorUUID As Boolean = True
    <HttpPost()>
    Function SaveDeviceApplicationDetails() As String
        Try


            Common.WebRequest.ProData.ExecuteSQL("Use " + DataBaseName)

            Dim p As New InsertUpdate("LibraryDeviceInstalledApplicationsDetails")
            Dim pSql As String
            Dim strIDReturn As String

            p("ApplicationName") = ReadyString(ApplicationName)
            p("Directory") = ReadyString(Directory)
            p("ApplicationInfo") = ReadyString(ApplicationInfo)
            p("UID") = ReadyString(UID)
            p("ApplicationLabel") = ReadyString(ApplicationLabel)
            p("ApplicationDescription") = ReadyString(ApplicationLabel)
            p("DeviceUUID") = ReadyString(DeviceUUID)
            p("DateCreated") = " getdate() "
            p("LastUpdate") = " getdate() "
            p("DeviceIMEI") = ReadyString(DeviceIMEI)

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

            DeviceAppID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Return ""
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function

    Function DeleteDeviceAppToAvoidDuplicate() As String
        Try
            Common.WebRequest.ProData.ExecuteSQL(" Use " & DataBaseName)
            Dim pSqlDel As String


            'let's delete applist so that no duplicates. mdm will always return all apps
            'in some cases, device only have uuid or imei
            If favorUUID Then
                pSqlDel = "DELETE FROM dbo.LibraryDeviceInstalledApplicationsDetails WHERE " &
               " DeviceUUID = " & ReadyString(DeviceUUID)
            Else
                pSqlDel = "DELETE FROM dbo.LibraryDeviceInstalledApplicationsDetails WHERE " &
             " DeviceIMEI = " & ReadyString(DeviceIMEI)
            End If
            Common.WebRequest.ProData.ExecuteSQL(pSqlDel)
        Catch ex As Exception
            Return ""
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function
    Function getDeviceApplicationInfo(ByVal strID As String) As DataTable
        Try
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
                    With .Add("ApplicationName", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationName"
                    End With
                    With .Add("Directory", SqlDbType.VarChar)
                        .Expression = "dev.Directory"
                    End With
                    With .Add("ApplicationInfo", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationInfo"
                    End With
                    With .Add("UID", SqlDbType.VarChar)
                        .Expression = "dev.UID"
                    End With
                    With .Add("ApplicationLabel", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationLabel"
                    End With
                    With .Add("ApplicationDescription", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationDescription"
                    End With
                    'With .Add("Found", SqlDbType.VarChar)
                    '    .Expression = "Found=case when exists (select * from LibraryDeviceWhiteListApps where dev.[ApplicationName] = LibraryDeviceWhiteListApps.[ApplicationName]) then 1 else 0 end"
                    'End With
                End With

                .SqlFrom = " FROM LibraryDeviceInstalledApplicationsDetails dev "

                .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE dev.ID IS NULL", " WHERE dev.ID = " + ReadyString(DeviceAppID))

                pDT = .Requery

            End With

            Return pDT
        Catch ex As Exception
        End Try
    End Function

    Function GetDeviceApplicationInfoByID() As DataTable
        Try
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
                    With .Add("ApplicationName", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationName"
                    End With
                    With .Add("Directory", SqlDbType.VarChar)
                        .Expression = "dev.Directory"
                    End With
                    With .Add("ApplicationInfo", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationInfo"
                    End With
                    With .Add("UID", SqlDbType.VarChar)
                        .Expression = "dev.UID"
                    End With
                    With .Add("ApplicationLabel", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationLabel"
                    End With
                    With .Add("ApplicationDescription", SqlDbType.VarChar)
                        .Expression = "dev.ApplicationDescription"
                    End With
                 
                End With

                .SqlFrom = " FROM LibraryDeviceInstalledApplicationsDetails dev "

                .SqlWhere = " WHERE ID = " & ReadyString(ID)
                pDT = .Requery

            End With

            Return pDT
        Catch ex As Exception
        End Try
    End Function


    Function GetDeviceApplicationInfoByDeviceID(ByVal strUUID As String, ByVal strIMEI As String) As DataTable

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
                With .Add("ApplicationName", SqlDbType.VarChar)
                    .Expression = "dev.ApplicationName"
                End With
                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dev.Directory"
                End With
                With .Add("ApplicationInfo", SqlDbType.VarChar)
                    .Expression = "dev.ApplicationInfo"
                End With
                With .Add("UID", SqlDbType.VarChar)
                    .Expression = "dev.UID"
                End With
                With .Add("ApplicationLabel", SqlDbType.VarChar)
                    .Expression = "dev.ApplicationLabel"
                End With
                With .Add("ApplicationDescription", SqlDbType.VarChar)
                    .Expression = "dev.ApplicationDescription"
                End With
                With .Add("Found", SqlDbType.VarChar)
                    .Expression = " case when exists (select * from LibraryDeviceWhiteListApps where dev.[ApplicationName] = LibraryDeviceWhiteListApps.[ApplicationName]) then 1 else 0 end"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceInstalledApplicationsDetails dev "
            If favorUUID Then
                .SqlWhere = " WHERE dev.DeviceUUID = " + ReadyString(strUUID)
            Else
                .SqlWhere = " WHERE dev.DeviceIMEI = " + ReadyString(strIMEI)
            End If

            pDT = .Requery

        End With

        Return pDT

    End Function



End Class

