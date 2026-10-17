Public Class DbConnectController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If Request.QueryString("SetSynonyms") = "1" Then
            With New Synonyms()
                .Prefix = "Mobiz"

                Try
                    .SourceDatabase = "Mobiz"
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "Mobiz",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizABC",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizProton",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizNike",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizMobiz",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizDUC",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizDSME",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizTestCompany",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizTemplate",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizAsian_Finds_Inc_",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizBN_whitening_Shoppe",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizJWS_BEAUTY_AND_BEYOND_CO_",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizK",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizLots_A_Pizza",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizNtek_Global_Inc_",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizResourceful_International",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizStarbike",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizRC_Inc_Test",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .ProData = ProData.CreateFromConnectionInfo( _
                        Common.WebRequest.ProData.Server, _
                        "MobizRobert_Inc_Test",
                        Common.WebRequest.ProData.Username, _
                        Common.WebRequest.ProData.Password,
                        Common.WebRequest.ProData.IntegratedSecurity)
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try
            End With
            AppendToLog("Synonyms has been triggered successfully")
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function Save() As String

        Dim pCI As ConnectionInfo
        Dim pProData As ProData

        If CBool2(Common.WebRequest.Data("NoUpdate")) Then
            If IO.File.Exists(Common.ConnectionInfo.Filename) Then
                Return JsonDictionary.Serialize("Error", "Database connection setting file already exist.")
            End If
        Else
            If Common.WebRequest.Data("DbConnectPassword") <> "secret lng gud" Then
                Return JsonDictionary.Serialize("Error", "Invalid DB Connect password")
            End If
        End If
        

        '// check if connected
        pCI = Common.ConnectionInfo
        pCI.Filename = Common.ServerPath & "settings\db.data"
        pCI.Server = Common.WebRequest.Data("Server")
        pCI.Database = Common.WebRequest.Data("Database")
        pCI.IntegratedSecurity = Common.WebRequest.Data("IntegratedSecurity")
        pCI.Username = Common.WebRequest.Data("Username")
        pCI.Password = Common.WebRequest.Data("Password")
        'pCI.ConnectionString = pData.TryGet("")
        pProData = New ProData
        pProData.Connect(pCI.Server, _
            pCI.Database, _
            pCI.Username, _
            pCI.Password, _
            pCI.IntegratedSecurity)
        If pProData.Connected = False Then
            Return JsonDictionary.Serialize("Error", pProData.ConnectError)
        End If

        '// check if saved
        If Common.ConnectionInfo.Save() Then
            Common.ConnectionInfo.Open()
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Error", "Failed to save connection info. " & Common.ConnectionInfo.ErrorMessage)
        End If

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pOut As JsonDictionary

        Try
            pOut = New JsonDictionary
            pOut.Add("Server", Common.ConnectionInfo.Server)
            pOut.Add("Database", Common.ConnectionInfo.Database)
            pOut.Add("Result", "ok")

            Return pOut.Serialize
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
End Class