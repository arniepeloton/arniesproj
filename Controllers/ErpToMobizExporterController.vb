Imports C1.C1Zip
Imports System.Reflection
Imports System.Collections.Specialized
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ErpToMobizExporterController
    Inherits System.Web.Mvc.Controller

    Private Shared mExporter As ErpToMobizExporter
    Private Shared ReadOnly mIndexLock As New Object
    Shared mStatus As String

    <HttpGet>
    Public Function Index(OrgID As String, NoRepeat As String, DontStart As String, DirectCompare As String) As String

        AppendToLog("ErpToMobizExporterController_Index. vb5P5AAQ959R! DontStart=" & DontStart & "; DirectCompare=" & DirectCompare, , ErpToMobizExporter.LogFilename)
        If String.IsNullOrWhiteSpace(DirectCompare) = False Then
            ErpToMobizExportCycler.DirectCompare = CBool2(DirectCompare)
        End If
        If String.IsNullOrWhiteSpace(NoRepeat) = False Then
            ErpToMobizExportCycler.NoRepeat = CBool2(NoRepeat)
        End If
        If IsGuid(OrgID) Then
            ' http://localhost:8080/ErpToMobizExporter?OrgID=00000000-0000-0000-0000-000000000001
            ErpToMobizImporter.OrgID = OrgID
        End If
        Return ErpToMobizExportCycler.Execute(CBool2(DontStart))

    End Function

End Class

