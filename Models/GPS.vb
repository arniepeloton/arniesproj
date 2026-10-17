<Serializable()> _
Public Class GPS
    Public Property ID As [String] = ""
    Public Property LocationID As [String] = ""
    Public Property Latitude As [String] = ""
    Public Property Longitude As [String] = ""
    Public Property SortOrder As [String] = ""
    Public Property DateCreated As [String]
    Public Property Uuid As [String] = ""
    Public Property Imei As [String] = ""
    Public Property OrgID As [String] = ""
    Public Property PersonID As [String] = ""
    Public Property DeviceID As [String] = ""
    Public Property favorUUID As Boolean = True
    Public Property dbName As [String] = ""

    <HttpPost()>
    Function SaveDeviceLocationDetails() As String
        Try

            Common.WebRequest.ProData.ExecuteSQL(" Use MobizDUC ")

            Dim p As New InsertUpdate("LibraryLocationTrack")
            Dim pSql As String
            Dim strIDReturn As String

            p("LocationID") = ReadyString(LocationID)
            p("Longitude") = ReadyString(Longitude)
            p("Latitude") = ReadyString(Latitude)
            p("SortOrder") = ReadyString(SortOrder)
            p("LocationRetrieve") = ReadyString(DateCreated)
            p("DateCreated") = " getdate() "
            p("Uuid") = ReadyString(Uuid)
            p("Imei") = ReadyString(Imei)
            p("OrgID") = ReadyString(OrgID)
            p("PersonID") = ReadyString(PersonID)
            p("DeviceID") = ReadyString(DeviceID)


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
            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Return ""
            Common.WebRequest.ProData.RollbackTransaction()
        End Try




    End Function



    <HttpPost()>
    Function GetLocationList(ByVal dateFrom As String, ByVal dateTo As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strWhere As String
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 2000
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryLocationTrack.ID"
                End With

                With .Add("LocationID", SqlDbType.NVarChar)
                    .Searchable = False
                    .Expression = "dbo.LibraryLocationTrack.LocationID"
                End With

                With .Add("Longitude", SqlDbType.Float)
                    .Expression = "dbo.LibraryLocationTrack.Longitude"
                End With

                With .Add("Latitude", SqlDbType.Float)
                    .Expression = "dbo.LibraryLocationTrack.Latitude"
                End With

                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "dbo.LibraryLocationTrack.SortOrder"
                End With

                With .Add("Uuid", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryLocationTrack.Uuid"
                End With

                With .Add("Imei", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryLocationTrack.Imei"
                End With

            End With

            .SqlFrom = "FROM LibraryLocationTrack "
            .SqlOrder = "ORDER BY [LocationRetrieve] ASC"

            If favorUUID Then
                strWhere = "WHERE UUID = " & ReadyString(Uuid)
            Else
                strWhere = "WHERE IMEI = " & ReadyString(Imei)
            End If

            If (Not String.IsNullOrWhiteSpace(dateFrom) And Not String.IsNullOrWhiteSpace(dateFrom)) Then
                strWhere = strWhere & " AND LibraryLocationTrack.[LocationRetrieve] BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo)
            End If
           
            .SqlWhere = strWhere

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetCurrentLocation() As DataTable
        Common.WebRequest.ProData.ExecuteSQL(" Use MobizDUC ")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryLocationTrack.ID"
                End With

                With .Add("LocationID", SqlDbType.NVarChar)
                    .Searchable = False
                    .Expression = "dbo.LibraryLocationTrack.LocationID"
                End With

                With .Add("Longitude", SqlDbType.Float)
                    .Expression = "dbo.LibraryLocationTrack.Longitude"
                End With

                With .Add("Latitude", SqlDbType.Float)
                    .Expression = "dbo.LibraryLocationTrack.Latitude"
                End With

                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "dbo.LibraryLocationTrack.SortOrder"
                End With

                With .Add("Uuid", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryLocationTrack.Uuid"
                End With

                With .Add("Imei", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryLocationTrack.Imei"
                End With

            End With

            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(Uuid)
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(Imei)
            End If
            .SqlFrom = "FROM LibraryLocationTrack "
            '.SqlWhere = "WHERE DeviceID = " & ReadyString(DeviceID)
            .SqlOrder = "ORDER BY LocationRetrieve Desc "
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetGPSSettings() As DataTable
        Dim pSql As String

        pSql = "SELECT * " & _
          " FROM SystemDefaults" & _
          " WHERE Description = 'IsRequireGPS' OR Description = 'GPSInterval'"

        Return Common.WebRequest.ProData.Execute(pSql)

    End Function

    Function SaveGPSSettings() As Boolean
        Dim pSql As String
        Dim pDT As DataTable

        Try

            pSql = "SELECT * " & _
            " FROM SystemDefaults" & _
            " WHERE Description = 'IsRequireGPS'"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count = 0 Then
                pSql = "Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values(" & vbCrLf &
                        ReadyString("IsRequireGPS") & "," &
                        ReadyString(CStr2(Common.WebRequest.Data("IsRequireGPS"))) & "," &
                        "GETDATE()" &
                        ")"
                pDT = Common.WebRequest.ProData.Execute(pSql)
            Else
                pSql = "Update SystemDefaults SET DefaultValue = " & ReadyString(CStr2(Common.WebRequest.Data("IsRequireGPS"))) &
                        " WHERE ID = " & ReadyString(CStr2(pDT.Rows(0).Item("ID")))

                pDT = Common.WebRequest.ProData.Execute(pSql)
            End If

            pSql = "SELECT * " & _
            " FROM SystemDefaults" & _
            " WHERE Description = 'GPSInterval'"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count = 0 Then
                pSql = "Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values(" & vbCrLf &
                        ReadyString("GPSInterval") & "," &
                        ReadyString(CStr2(Common.WebRequest.Data("GPSInterval"))) & "," &
                        "GETDATE()" &
                        ")"
                pDT = Common.WebRequest.ProData.Execute(pSql)
            Else
                pSql = "Update SystemDefaults SET DefaultValue = " & ReadyString(CStr2(Common.WebRequest.Data("GPSInterval"))) &
                        " WHERE ID = " & ReadyString(CStr2(pDT.Rows(0).Item("ID")))

                pDT = Common.WebRequest.ProData.Execute(pSql)
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Catch ex As Exception
            Return False
        End Try
       
        Return True
    End Function

    <HttpPost()>
    Function GetLatestLocationByDeviceID() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryLocationTrack.ID"
                End With

                With .Add("LocationID", SqlDbType.NVarChar)
                    .Searchable = False
                    .Expression = "dbo.LibraryLocationTrack.LocationID"
                End With

                With .Add("Longitude", SqlDbType.Float)
                    .Expression = "dbo.LibraryLocationTrack.Longitude"
                End With

                With .Add("Latitude", SqlDbType.Float)
                    .Expression = "dbo.LibraryLocationTrack.Latitude"
                End With

                With .Add("Uuid", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryLocationTrack.Uuid"
                End With

                With .Add("Imei", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryLocationTrack.Imei"
                End With

            End With

            .SqlFrom = "FROM LibraryLocationTrack "
            .SqlOrder = "ORDER BY LocationRetrieve ASC"
            .SqlWhere = "Where DeviceID = " & ReadyString(DeviceID)
            pDT = .Requery
        End With

        Return pDT
    End Function




    Function GetDeviceIDByDeviceCodes() As String
        Dim result As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryDevice.ID"
                End With
            End With

            .SqlFrom = "FROM LibraryDevice "
            .SqlWhere = "WHERE LibraryDevice.UUID  = " & ReadyString(Uuid) & " OR LibraryDevice.IMEI = " & ReadyString(Imei)
            pDT = .Requery
            If pDT.Rows.Count > 0 Then
                result = pDT.Rows(0).Item(0).ToString
            End If

        End With

        Return result


    End Function





End Class
