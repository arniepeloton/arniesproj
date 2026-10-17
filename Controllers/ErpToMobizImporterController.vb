Imports C1.C1Zip
Imports System.Reflection
Imports System.Collections.Specialized

Public Class ErpToMobizImporterController
    Inherits System.Web.Mvc.AsyncController

    Private Shared mImporter As ErpToMobizImporter
    Private Shared mIndexLock As New Object

    <HttpGet>
    Public Function Index() As String

        Return "nothing to see here vb2MZXQXTC8Y!"

    End Function

    <HttpPost>
    Public Function Upload() As String

        Dim pFilename As String

        If Request.Files.Count = 0 Then
            Return JsonDictionary.ToString("Error", "No uploaded file vbXTSN5X93CM!")
        End If
        pFilename = Common.WebRequest.Data("Filename")
        pFilename = Common.ServerPath & "temp\ImportErpToMobiz\" & pFilename
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If
        CreateFolder(pFilename)
        Request.Files(0).SaveAs(pFilename)
        mImporter = Nothing

        Return JsonDictionary.ToString("Result", "ok")

    End Function

    <HttpPost>
    Public Function Start() As String

        Dim pFilename As String

        If mImporter IsNot Nothing Then
            Return JsonDictionary.ToString("Status", "Import already in progress")
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

        Return JsonDictionary.ToString("Status", "Import stated")

    End Function

    <HttpGet>
    Public Function GetStatus() As String

        If mImporter Is Nothing Then
            Return JsonDictionary.ToString("Error", "Not started")
        End If

        Return JsonDictionary.ToString("Status", mImporter.Status)

    End Function

#If DEBUG Then ' separate region from above. this region is permanently for debug
    <HttpGet>
    Public Function TestRowCountCheckSum() As String

        Dim p As RowCountChecksumDuc
        Dim pFilename As String

        pFilename = Common.ServerPath & "temp\ImportErpToMobiz\2015_03_30__14_18_09_9425.db"
        p = New RowCountChecksumDuc(pFilename)
        Return p.Process()

    End Function
#End If
End Class

