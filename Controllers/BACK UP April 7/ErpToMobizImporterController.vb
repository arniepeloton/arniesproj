Imports C1.C1Zip
Imports System.Reflection
Imports System.Collections.Specialized

Public Class ErpToMobizImporterController
    Inherits System.Web.Mvc.AsyncController
#If DEBUG Then

    Private Shared mImporter As ErpToMobizImporter
    Private Shared mIndexLock As New Object
    Const mDestinationUrl As String = "http://localhost:8080/"
    Shared mStatus As String

    <HttpGet>
    Public Function Index() As String

        Return "nothing to see here vb2MZXQXTC8Y!"

    End Function

    <HttpPost>
    Public Function Upload() As String

        Dim pFilename As String

        If Request.Files.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "No uploaded file vbXTSN5X93CM!")
        End If
        pFilename = Common.WebRequest.Data("Filename")
        pFilename = Common.ServerPath & "temp\ImportErpToMobiz\" & pFilename
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If
        CreateFolder(pFilename)
        Request.Files(0).SaveAs(pFilename)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function Start() As String

        Dim pFilename As String

        If mImporter IsNot Nothing Then
            Return JsonDictionary.Serialize("Status", "Imported already in progress")
        End If

        pFilename = Common.WebRequest.Data("Filename")
        TimedThread(
            Sub()
                pFilename = Common.ServerPath & "temp\ImportErpToMobiz\" & pFilename
                mImporter = New ErpToMobizImporter
                mImporter.SourceZipFilename = pFilename
                mImporter.Process()
            End Sub
        )

        Return JsonDictionary.Serialize("Status", "Import stated")

    End Function

    <HttpGet>
    Public Function GetStatus() As String

        If mImporter Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Not started")
        End If

        Return JsonDictionary.Serialize("Status", mImporter.Status)

    End Function
#End If
End Class

