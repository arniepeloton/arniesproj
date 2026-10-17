<Serializable()> _
Public Class DeviceAppsWhiteList
    Public Property ID As String = ""
    Public Property ApplicationLabel As String
    Public Property ApplicationName As String
    Public Property Directory As String
    Public Property DevicePolicyProfileID As String

    <HttpPost()>
    Function SaveApplicationWhiteList() As String

        Dim p As New InsertUpdate("LibraryDeviceWhiteListApps")
        Dim pSql As String
        Dim strIDReturn As String

        p("ApplicationLabel") = ReadyString(ApplicationLabel)
        p("ApplicationName") = ReadyString(ApplicationName)
        p("Directory") = ReadyString(Directory)
        p("DateCreated") = " getdate() "
        p("LastUpdate") = " getdate() "
        If Not DevicePolicyProfileID Is Nothing Then
            If Not String.IsNullOrEmpty(DevicePolicyProfileID) Then
                p("DevicePolicyProfileID") = ReadyString(DevicePolicyProfileID)
            End If
        End If

        Try
            Common.WebRequest.ProData.BeginTransaction()

            If (String.IsNullOrEmpty(ID)) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Return ""
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function



    Function DeleteDeviceWhiteAppByID() As String
        Try

            Dim pSql As String
            pSql = "DELETE FROM dbo.LibraryDeviceWhiteListApps WHERE " &
           " ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Return ""
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function



    <HttpPost()>
    Function GetApplicationWhiteList() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ID"
                End With

                With .Add("ApplicationLabel", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With

                With .Add("ApplicationName", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationName"
                End With

                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Directory"
                End With

                With .Add("Keywords", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Keywords"
                End With

                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.DateCreated"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.LastUpdate"
                End With

                With .Add("Void", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Void"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceWhiteListApps "
            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetApplicationWhiteListByID() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ID"
                End With

                With .Add("ApplicationName", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationName"
                End With


                With .Add("ApplicationLabel", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With


                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Directory"
                End With


                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.DateCreated"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.LastUpdate"
                End With

                With .Add("Void", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Void"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceWhiteListApps "
            .SqlWhere = " Where ID =  " & ReadyString(ID)
            pDT = .Requery
        End With

        Return pDT
    End Function




    Function GetDeviceApplicationInfoByAppID(ByVal strID As String) As DataTable

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
            .SqlWhere = "WHERE dev.ID = " + ReadyString(strID)
            pDT = .Requery

        End With

        Return pDT



    End Function



    <HttpPost()>
    Function GetApplicationWhiteListByPolicyProfileID() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ID"
                End With

                With .Add("ApplicationLabel", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With

                With .Add("ApplicationName", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationName"
                End With

                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Directory"
                End With

                With .Add("Keywords", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Keywords"
                End With

                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.DateCreated"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.LastUpdate"
                End With

                With .Add("Void", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Void"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceWhiteListApps"
            .SqlWhere = "WHERE DevicePolicyProfileID IS NULL "
            'OR DevicePolicyProfileID = " + ReadyString(DevicePolicyProfileID)

            pDT = .Requery
        End With

        Return pDT
    End Function


    <HttpPost()>
    Function GetApplicationWhiteListByOrgID() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ID"
                End With

                With .Add("ApplicationLabel", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With

                With .Add("ApplicationName", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationName"
                End With

                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Directory"
                End With

                With .Add("Keywords", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Keywords"
                End With

                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.DateCreated"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.LastUpdate"
                End With

                With .Add("Void", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Void"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceWhiteListApps"
            .SqlWhere = "WHERE DevicePolicyProfileID IS NULL "

            pDT = .Requery
        End With

        Return pDT
    End Function


    <HttpPost()>
    Function GetApplicationWhiteListByOrgIDAndPolicyProfileID() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ID"
                End With

                With .Add("ApplicationLabel", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationLabel"
                End With

                With .Add("ApplicationName", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ApplicationName"
                End With

                With .Add("Directory", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Directory"
                End With

                With .Add("Keywords", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Keywords"
                End With

                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.DateCreated"
                End With

                With .Add("LastUpdate", SqlDbType.DateTime)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.LastUpdate"
                End With

                With .Add("Void", SqlDbType.Bit)
                    .Expression = "dbo.LibraryDeviceWhiteListApps.Void"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceWhiteListApps "
            .SqlWhere = "WHERE DevicePolicyProfileID IS NULL OR DevicePolicyProfileID = " + ReadyString(DevicePolicyProfileID)
            pDT = .Requery
        End With

        Return pDT
    End Function


    <HttpPost()>
    Function CheckApplicationIfExistByPolicyProfileID() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strWhere As String = ""
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryDeviceWhiteListApps.ID"
                End With
            End With



            .SqlFrom = "FROM LibraryDeviceWhiteListApps"
            strWhere = "WHERE" + " (ApplicationLabel =  " + ReadyString(ApplicationLabel) + " OR  ApplicationName = " + ReadyString(ApplicationName) + " )"

            If (Not DevicePolicyProfileID Is Nothing) Then
                If (Not String.IsNullOrWhiteSpace(DevicePolicyProfileID)) Then
                    strWhere = strWhere + " AND  DevicePolicyProfileID =  " + ReadyString(DevicePolicyProfileID)
                End If
            End If

            .SqlWhere = strWhere

            pDT = .Requery
            Return pDT
        End With
    End Function


End Class

