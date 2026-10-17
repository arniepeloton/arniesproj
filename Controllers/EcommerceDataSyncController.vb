Public Class EcommerceDataSyncController
    Inherits System.Web.Mvc.AsyncController

    Private Shared mEcommerceDataSync As New EcommerceDataSync

    Public Function StatusText() As String

        Return mEcommerceDataSync.StatusText

    End Function

    Public Function IsBusy() As String

        Return mEcommerceDataSync.IsBusy

    End Function

    Public Function Start() As String

        Dim pOrgID As String
        Dim pEcommerceDataSourceServerIP As String
        Dim pError As String

#If DEBUG Then
        If Debugger.IsAttached = False Then
            Return "debugger is not yet attached."
        End If
#End If

        If mEcommerceDataSync.IsBusy Then
            Return mEcommerceDataSync.StatusText
        End If

        TimedThread(
            Sub()
                mEcommerceDataSync.Execute()
            End Sub)
        Threading.Thread.Sleep(1000)

        If mEcommerceDataSync.IsBusy Then
            Return mEcommerceDataSync.StatusText
        End If

#If DEBUG Then
        pOrgID = modClient.AbcOrgID
        pOrgID = modClient.DucOrgID
#Else
        pOrgID = modClient.DucOrgID
#End If

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        pEcommerceDataSourceServerIP = WebRequest.Instance.Business.SystemDefaults.EcommerceDataSourceServerIP
        If String.IsNullOrWhiteSpace(pEcommerceDataSourceServerIP) Then
            Return "No need to import data. EcommerceDataSourceServerIP is not set. ref: vbTZ3WARV5T9!"
        End If
        pEcommerceDataSourceServerIP = AppendIfNeeded(pEcommerceDataSourceServerIP, "/")

        If mEcommerceDataSync.IsBusy = False Then
            If CBool2(Common.WebRequest.Data("DontStart")) = False Then
                mEcommerceDataSync.SaveMilestone(
                vPrivateDatabaseName:=WebRequest.Instance.ProData.GetCurrentDatabase(),
                vOrgID:=pOrgID,
                vPublicServerUrl:=pEcommerceDataSourceServerIP)
            End If
            'QuartzScheduler.TriggerEcommerceDataSyncJob()
        End If

        Return "started"

    End Function

    Public Function CompareData() As String

        Return mEcommerceDataSync.CompareSqlServerData

    End Function

    Private Shared mDataForComparisonKey As String

    Public Function RequestDataForComparison() As String

        Dim pError As String
        Dim pPrivateDatabaseName As String
        Dim pFilename As String
        Dim pOrgID As String

        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If
        pPrivateDatabaseName = WebRequest.Instance.ProData.GetCurrentDatabase()
        pOrgID = WebRequest.Instance.OrgID

        If mDataForComparisonKey Is Nothing Then
            mDataForComparisonKey = DateTimeStamp()
            TimedThread(
                Sub()
                    Using pProData = ProData.CreateFromConnectionInfo(
                        vServer:=Common.ConnectionInfo.Server,
                        vDatabase:=pPrivateDatabaseName,
                        vUsername:=Common.ConnectionInfo.Username,
                        vPassword:=Common.ConnectionInfo.Password,
                        vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

                        Dim pDS = EcommerceDataSync.GetDataForComparison(pProData, pOrgID)
                        pFilename = Common.ServerPath & "temp\ECommerceDataSyncDataForComparison\" & mDataForComparisonKey & ".zip"
                        CreateFolder(pFilename)
                        Dim pMS As New IO.MemoryStream
                        pDS.WriteXml(pMS, XmlWriteMode.WriteSchema)
                        Dim pZip As New ProZip(pFilename & ".temp")
                        pZip.AddStream(pMS, "DataSet.xml")
                        pZip.Close()
                        IO.File.Move(pFilename & ".temp", pFilename)
                    End Using
                End Sub)
            Return JsonDictionary.ToString("Result", "Starting to create data for comparison")
        Else
            pFilename = Common.ServerPath & "temp\ECommerceDataSyncDataForComparison\" & mDataForComparisonKey & ".zip"
            If IO.File.Exists(pFilename) Then
                If (Now - IO.File.GetLastWriteTime(pFilename)).TotalMinutes > 10 Then
                    mDataForComparisonKey = Nothing
                    Return RequestDataForComparison
                End If
                Return JsonDictionary.ToString("DataForComparisonKey", mDataForComparisonKey)
            Else
                Return JsonDictionary.ToString("Result", "Still creating data for comparison")
            End If
        End If

    End Function

    Public Function DownloadDataForComparison() As ActionResult

        Dim pFile As IO.FileInfo
        Dim pCrc As String

        Dim pFilename = Common.ServerPath & "temp\ECommerceDataSyncDataForComparison\" & mDataForComparisonKey & ".zip"

        pFile = New IO.FileInfo(pFilename)
        Try
            If pFile.Exists Then
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(pFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(IO.Path.GetFileName(pFilename), True))
                Response.TransmitFile(pFilename)
            Else
                Throw New IO.FileNotFoundException
            End If
        Catch ex As Exception
            Stop2()
            Throw
        End Try

    End Function
End Class
