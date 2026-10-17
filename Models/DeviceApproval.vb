Imports ProPack
Imports ProWeb
Public Class DeviceApproval

    Public Property ID As String
    Public Property PhoneName As String
    Public Property PhoneModel As String
    Public Property IMEI As String
    Public Property UUID As String
    Public Property OS As String
    Public Property DeviceID As String

    <HttpPost()>
    Function getDeviceByDevID() As DataTable
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
            .SqlWhere = "WHERE ID = " & ReadyString(ID)
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
            pDT = .Requery
        End With

        Return pDT

    End Function



    <HttpPost()>
    Function updateDeviceByID() As DataTable
        'Dim pSql As String
        'pSql = "Update LibraryDevice set IsActive=1 where ID = " & ReadyString(ID)
        'Common.WebRequest.ProData.Execute(pSql)

        Dim p As New InsertUpdate("LibraryDevice")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String

        p("IsActive") = "1"
        p("DateEnrolled") = " getdate() "
        p("UserFullName") = ReadyString(WebSession.Instance.FullName)
        Try

            Common.WebRequest.ProData.BeginTransaction()
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))


            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()
        Catch ex As Exception

        End Try
    End Function

End Class
