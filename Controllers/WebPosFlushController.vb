Imports System.Collections.Specialized
Imports System.Net
Imports System.Security.AccessControl
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports WebProj.WebProj

Public Class WebPosFlushController
    Inherits System.Web.Mvc.AsyncController

    Function Index() As ActionResult

        WebPosFlush.StartAsync()
        Return View()

    End Function

    Public Function GetRecentMessages() As String

        Dim pLastTimeStamp = Common.WebRequest.Data("LastTimeStamp")
        Dim pTimeStamp As Date? = ParseDate(pLastTimeStamp)
        If pTimeStamp Is Nothing Then
            pTimeStamp = Now
        End If
        Dim pRecent = WebPosFlush.GetRecentMessages(pTimeStamp.Value)

        Dim pOut = JsonConvert.SerializeObject(pRecent)
        Return pOut

    End Function

    Public Function GetUnflushedInfo() As String

        Dim pOut = New JsonDictionary()

        pOut.Add("UnflushedSalesReturnRowCount", WebPosFlush.UnflushedSalesReturnRowCount)
        pOut.Add("LastQueryTime", Format(WebPosFlush.LastQueryTime, cGlobal.FormatDateTimeSql))

        Return pOut.Serialize()

    End Function

    Public Function Check() As String
        Dim pOut = WebPosDb.Start()
        Return pOut
    End Function
End Class
