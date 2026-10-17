Imports System.Collections.Specialized
Imports System.Net
Imports System.Threading
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ErpToMobizExportCycler
    Shared mStatus As String
    Shared ReadOnly mLock As New Object
    Private Shared mExporter As ErpToMobizExporter
#If DEBUG Then
    Public Const DestinationUrl As String = "http://localhost:8080/"
#Else
    'Public Const DestinationUrl As String = "http://122.52.123.251/"
    Public Const DestinationUrl As String = "http://localhost:8083/"
    'Public Const DestinationUrl As String = "http://localhost/"
#End If
    Shared WithEvents mTimer As Timers.Timer
    Shared ReadOnly mSemaphore As New SemaphoreSlim(1)
    Public Shared NoRepeat As Boolean
    Public Shared DirectCompare As Boolean

    Public Shared Function Execute(Optional vDontStart As Boolean = False) As String

        Dim pError As String

        If mSemaphore.Wait(2000) = False Then
            Return DateTimeStamp() & " Another export process is in progress. NoRepeat=" & NoRepeat & "; DontStart=" & vDontStart & "; DirectCompare=" & DirectCompare
        End If
        If mExporter Is Nothing _
            OrElse mExporter.Busy = False Then

            If CanExport(pError) = False Then
                Return "Can't export now: " & pError & ". NoRepeat=" & NoRepeat & "; DontStart=" & vDontStart & "; DirectCompare=" & DirectCompare
            End If

            If vDontStart Then
                mSemaphore.Release()
                Return "No export process is currenty executing"
            End If

            mExporter = New ErpToMobizExporter()
#If DEBUG Then
            'mExporter.LastestRecordsFirst = True
#End If
            TimedThread(
                Sub()
                    Process()
                End Sub)

            Return "Export started. NoRepeat=" & NoRepeat & " ; " & DateTimeStamp()
        Else
            If mStatus IsNot Nothing Then
                Return mStatus
            Else
                Return "Export in progress: " & mExporter.Status & ". NoRepeat=" & NoRepeat
            End If
        End If

    End Function

    Private Shared Sub Process()

        Try
            AppendToLog("ErpToMobizExportCycler_Process. vbPVCFRBWEBA!", , ErpToMobizExporter.LogFilename)
            If mExporter.Export() = False Then
                Return
            End If
            AppendToLog("ErpToMobizExportCycler_Process after export. vbLF7UNQV8AY!", , ErpToMobizExporter.LogFilename)

            'If mExporter.TotalRecords = 0 Then
            '    'mExporter = Nothing
            '    'mStatus = "No records to export"
            '    'Exit Sub
            '    Threading.Thread.Sleep(5 * 60 * 1000)
            'End If

            Dim pValues As NameValueCollection
            Dim pJson As JsonDictionary
            Dim pResult As String

            pJson = New JsonDictionary
            pJson.Add("Filename", IO.Path.GetFileName(mExporter.ZipFilename))

            pValues = New NameValueCollection
            pValues.Add("json", pJson.ToString())

            mStatus = "uploading " & DisplayableSize(New IO.FileInfo(mExporter.ZipFilename).Length) & " with " & mExporter.TotalRecords & " records"
            AppendToDebug("vbDXNKWUKB9R!" & vbCrLf & mStatus)
            pResult = UploadFile(mExporter.ZipFilename, DestinationUrl & "ErpToMobizImporter/Upload", pValues)

            Dim pWeb As WebClient

            pJson = New JsonDictionary
            pJson.Add("Filename", IO.Path.GetFileName(mExporter.ZipFilename))

            pValues = New NameValueCollection
            pValues.Add("json", pJson.ToString())

            pWeb = New WebClient
            mStatus = "processing"
            pResult = Encoding.UTF8.GetString(pWeb.UploadValues(DestinationUrl & "ErpToMobizImporter/Start", pValues))

            mTimer = New Timers.Timer
            If mExporter.TotalRecords = 0 Then
#If DEBUG Then
                mTimer.Interval = 1 * 60 * 1000
#Else
                mTimer.Interval = 5 * 60 * 1000
#End If
            Else
                mTimer.Interval = 5000
            End If
            AddHandler mTimer.Elapsed, AddressOf mTimer_Timer
            mTimer.Enabled = True
            mStatus = pResult
            mExporter.Busy = False
        Catch ex As Exception
            AppendToDebug("vb6F2CTL63Z5! " & ex.Message, True, ex, True)
            mTimer = New Timers.Timer
            mTimer.Interval = 5000
            AddHandler mTimer.Elapsed, AddressOf mTimer_Timer
            mTimer.Enabled = True
        End Try

    End Sub

    Shared Sub mTimer_Timer(vSender As Object, e As Timers.ElapsedEventArgs)

        mTimer.Enabled = False

        Dim pRemoteStatus As String
        Dim pRemoteStatusObj As RemoteStatusClass

        Try
            AppendToLog("vb6GU3MB7GFQ! before GetRemoteStatus", , ErpToMobizExporter.LogFilename)
            pRemoteStatus = GetRemoteStatus()
            AppendToLog("vb5ZRTSKFAM5! after GetRemoteStatus: " & pRemoteStatus, , ErpToMobizExporter.LogFilename)
            'pRemoteStatusObj = JObject.Parse(pRemoteStatus).SelectToken("d").ToObject(Of RemoteStatusClass)()
            pRemoteStatusObj = JsonConvert.DeserializeObject(Of RemoteStatusClass)(pRemoteStatus)
            If CStr2(pRemoteStatusObj.Error) <> "" Then
                If NoRepeat Then
                    mSemaphore.Release()
                Else
                    Process()
                End If
                Return 'True
            End If
            Select Case CStr2(pRemoteStatusObj.Status).ToLower()
                Case "done"
                    mExporter.SaveLastUpdated()
                    ErpToMobizExporter.ClearExportableProductID()
                    ErpToMobizExporter.ClearExportableVendorPriceProductID()
                    If DirectCompare Then
                        DirectCompare = False
                        Call New ErpToMobizComparerDirect().Process()
                    End If
                    If NoRepeat Then
                        mSemaphore.Release()
                    Else
                        Process()
                    End If
                    AppendToLog("vbF65CU6SGEZ! after done: " & pRemoteStatus, , ErpToMobizExporter.LogFilename)

                    Return 'True

                Case "error"
                    If NoRepeat Then
                        mSemaphore.Release()
                    Else
                        Process()
                    End If
                    Return 'True

                Case Else
                    'oError = pRemoteStatusObj.Status
                    mTimer.Enabled = True
                    Return 'False

            End Select
        Catch ex As Exception
            If NoRepeat Then
                mSemaphore.Release()
            Else
                Process()
            End If
            AppendToLog("vbMZWPEPCS83! error: " & ex.Message, , ErpToMobizExporter.LogFilename)
            AppendToDebug("vbEYDEYDX86M! " & ex.Message, True, ex, True)
            Return 'True
        End Try

    End Sub

    Private Shared Function CanExport(ByRef oError As String) As Boolean

        Dim pRemoteStatus As String
        Dim pRemoteStatusObj As RemoteStatusClass

        oError = ""
        Try
            pRemoteStatus = GetRemoteStatus()
            'pRemoteStatusObj = JObject.Parse(pRemoteStatus).SelectToken("d").ToObject(Of RemoteStatusClass)()
            pRemoteStatusObj = JsonConvert.DeserializeObject(Of RemoteStatusClass)(pRemoteStatus)
            If CStr2(pRemoteStatusObj.Error) <> "" Then
                Return True
            End If
            Select Case CStr2(pRemoteStatusObj.Status).ToLower()
                Case "done"
                    Return True

                Case Else
                    oError = pRemoteStatusObj.Status
                    Return False

            End Select
        Catch ex As Exception
            AppendToDebug("vbNKVPG6ARNR! " & ex.Message, True, ex, True)
            Return True
        End Try

    End Function

    Private Shared Function GetRemoteStatus() As String

        Dim pWeb As WebClient
        Dim pOut As String
        Dim pUrl As String

        Try
            pWeb = New WebClient
            pUrl = AppendIfNeeded(DestinationUrl, "/") & "ErpToMobizImporter/GetStatus"
            pOut = pWeb.DownloadString(pUrl)
        Catch ex As Exception
            Throw New ApplicationException(ex.Message & vbCrLf & "URL: " & pUrl, ex)
        End Try

        Return pOut

    End Function

    Public Shared ReadOnly Property Status As String
        Get

            Return mStatus

        End Get
    End Property

    Private Class RemoteStatusClass
        Public [Error] As String
        Public Status As String
    End Class
End Class