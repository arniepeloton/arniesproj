Imports C1.C1Zip
Imports System.Reflection
Imports System.Collections.Specialized
Imports System.Net

Public Class ErpToMobizExporterController
    Inherits System.Web.Mvc.AsyncController
#If DEBUG Then

    Private Shared mExporter As ErpToMobizExporter
    Private Shared mIndexLock As New Object
    Const mDestinationUrl As String = "http://localhost:8080/"
    Shared mStatus As String

    <HttpGet>
    Public Function Index() As String

        SyncLock mIndexLock
            If mExporter Is Nothing Then
                mExporter = New ErpToMobizExporter()
                TimedThread(AddressOf Process)

                Return "Export started"
            Else
                If mStatus IsNot Nothing Then
                    Return mStatus
                Else
                    Return "Export in progress: " & mExporter.Status
                End If
            End If
        End SyncLock

    End Function

    Private Shared Sub Process()

        mExporter.Export()
        If mExporter.TotalRecords = 0 Then
            mStatus = "No records to export"
            Exit Sub
        End If

        Dim pValues As NameValueCollection
        Dim pJson As JsonDictionary
        Dim pResult As String

        pJson = New JsonDictionary
        pJson.Add("Filename", IO.Path.GetFileName(mExporter.ZipFilename))

        pValues = New NameValueCollection
        pValues.Add("json", pJson.ToString())

        mStatus = "uploading " & mExporter.TotalRecords & " records"
        pResult = UploadFile(mExporter.ZipFilename, mDestinationUrl & "ErpToMobizImporter/Upload", pValues)

        Dim pWeb As WebClient

        pJson = New JsonDictionary
        pJson.Add("Filename", IO.Path.GetFileName(mExporter.ZipFilename))

        pValues = New NameValueCollection
        pValues.Add("json", pJson.ToString())

        pWeb = New WebClient
        mStatus = "processing"
        pResult = Encoding.UTF8.GetString(pWeb.UploadValues(mDestinationUrl & "ErpToMobizImporter/Start", pValues))

        mStatus = pResult

    End Sub

    <HttpPost()>
    <AsyncTimeout(Integer.MaxValue)>
    Public Sub ExecuteAsync()

        AsyncManager.OutstandingOperations.Increment()

    End Sub

    Public Function ExecuteCompleted(vOut As JsonDictionary) As String

        Return vOut.Serialize()

    End Function

    'Private Sub mDbProvisioner_Completed(sender As Object, e As DbProvisioner.CompletedArgs) Handles mDbProvisioner.Completed

    '    Dim pJson As JsonDictionary

    '    pJson = New JsonDictionary
    '    If e.Success Then
    '        pJson.Add("Result", "ok")
    '        pJson.Add("DatabaseName", e.DatabaseName)
    '    Else
    '        pJson.Add("Error", e.ErrorMessage)
    '    End If
    '    AsyncManager.Parameters("vOut") = pJson
    '    AsyncManager.OutstandingOperations.Decrement()

    'End Sub

#End If
End Class

