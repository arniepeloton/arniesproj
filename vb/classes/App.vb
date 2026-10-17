Imports DataImportExport

Public Class App
    Private Shared mBusiness As Business

    Public Shared Message As String
    Private Shared mFinishedRun As Boolean

    Private Shared mLastImportExport As Integer ' tickcount

    Public Shared Function RunAgent() As Boolean

        Static sSL As New Threading.SpinLock

        If sSL.IsHeld Then Return False ' app is already running

        TimedThread( _
            Sub()
                Try
                    If sSL.IsHeld Then Exit Sub
                    sSL.Enter(False)

                    Dim pImportExport As ImportExporter
                    Message = ""

                    If New TimeSpan(12 + 7, 0, 0) <= Now.TimeOfDay Then
                        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))

                        AddLog("Import Export")
                        pImportExport = New ImportExporter
                        pImportExport.Business = mBusiness
                        pImportExport.AddLog = AddressOf AddLog
                        pImportExport.MinimumInterval = 60 * 5
                        pImportExport.Execute()

                        'pImportExport.ExecuteWholesaleToMobiz()

                        AddLog("Done")
                    End If
                    mFinishedRun = True
                    RunAgent = True

                Catch ex As Exception
                    AddLog("Error: " & ex.Message & vbCrLf & GetStackTrace(, ex))

                Finally
                    If sSL.IsHeldByCurrentThread Then
                        Try
                            If mBusiness IsNot Nothing Then
                                mBusiness.ProData.Disconnect()
                            End If
                        Catch ex As Exception
                            Stop2() ' just ignore this
                        End Try
                        sSL.Exit()
                    End If
                End Try
            End Sub)

    End Function

    Public Shared ReadOnly Property FinishedRun As Boolean
        Get

            Return mFinishedRun

        End Get
    End Property

    Private Shared Sub AddLog(vMessage As String)

        Message &= "[" & Format(Now, "M/d/yyyy h:mm:ss tt") & "] " & vMessage & vbCrLf

    End Sub

    Private MustInherit Class Agent
        Public Business As Business
        Public AddLog As Action(Of String)
        Public MustOverride Sub Execute()
        Public MustOverride Property LastExecution As Integer

        ''' <summary>
        ''' in seconds
        ''' </summary>
        ''' <remarks></remarks>
        Public MinimumInterval As Integer

        Public ReadOnly Property ProData As ProData
            Get

                Return Business.ProData

            End Get
        End Property

        Public Function CanExecute() As Boolean

            Return MinimumInterval * 1000 <= Environment.TickCount - LastExecution

        End Function

        Public Sub SetLastExecution()

            LastExecution = Environment.TickCount

        End Sub
    End Class

    Private Class ImportExporter
        Inherits Agent

        ReadOnly mExportFilename As String
        ReadOnly mImportResultFilename As String

        Public Sub New()

            Dim pFolder As String

            pFolder = Common.ServerPath & "temp\MobizDuc_Wholesale\"
            mExportFilename = pFolder & "export.zip"
            mImportResultFilename = pFolder & "ImportResult.zip"

        End Sub

        Public Sub ExecuteWholesaleToMobiz()
            Try
                ExportWholesaleToFile()
                ImportMobizFromFile()
                UntagWholesaleToMobiz()
                AddLog("Done with Import Export Wholesale To Mobiz")
            Catch ex As System.Exception
                AddLog(ex.Message)
            End Try
        End Sub

        Private Sub ExportWholesaleToFile()

            Dim pExport As MobizDataExport

            pExport = MobizDataExport.CreateWholesaleToFile(mExportFilename, _
                Common.ConnectionInfo.Server, _
                "Wholesale",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity, _
                "", "")
            AddLog("Exporting Wholesale")
            pExport.ExportWholesaleToFile()

        End Sub

        Private Sub ImportMobizFromFile()

            Dim pDataImport As MobizDataImport
            Dim pFilename As String
            Dim pResults As JsonDictionary

            If IO.File.Exists(mExportFilename) = False Then Exit Sub
            pDataImport = MobizDataImport.CreateWholesaleToMobiz(mExportFilename, _
                Common.ConnectionInfo.Server, _
                "MobizDUC",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity)
            pDataImport.MobizClient = ProData.CreateFromConnectionInfo( _
                Common.ConnectionInfo.Server, _
                "Wholesale",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity)
            pDataImport.OutputFilename = mImportResultFilename
            pResults = pDataImport.Import ' result not yet used
            IO.File.Delete(mExportFilename)

        End Sub

        Public Overrides Sub Execute()

            Try
                If CanExecute() Then
                    Export()
                    Import()
                    Untag()
                    SetLastExecution()
                    AddLog("Done with Import Export")
                End If
            Catch ex As System.Exception
                AddLog(ex.Message & vbCrLf & GetStackTrace(, ex))
            End Try

        End Sub

        Public Overrides Property LastExecution() As Integer
            Get

                Return mLastImportExport

            End Get
            Set(value As Integer)

                mLastImportExport = value

            End Set
        End Property

        Private Sub Export()

            Dim pExport As MobizDataExport

            pExport = MobizDataExport.Create(mExportFilename, _
                Common.ConnectionInfo.Server, _
                "MobizDUC",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity, _
                "", "")
            AddLog("Exporting MobizDUC")
            pExport.Export()

        End Sub

        Private Sub Import()

            Dim pDataImport As MobizDataImport
            Dim pFilename As String
            Dim pResults As JsonDictionary

            If IO.File.Exists(mExportFilename) = False Then Exit Sub
            pDataImport = MobizDataImport.Create(mExportFilename, _
                Common.ConnectionInfo.Server, _
                "Wholesale",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity)
            pDataImport.MobizClient = ProData.CreateFromConnectionInfo( _
                Common.ConnectionInfo.Server, _
                "MobizDUC",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity)
            pDataImport.OutputFilename = mImportResultFilename
            pResults = pDataImport.Import ' result not yet used
            IO.File.Delete(mExportFilename)

        End Sub

        Private Sub Untag()

            Dim pDataUntagger As DataUntagger

            If IO.File.Exists(mImportResultFilename) = False Then Exit Sub
            pDataUntagger = New DataUntagger(mImportResultFilename)
            pDataUntagger.ProData = ProData.CreateFromConnectionInfo( _
                Common.ConnectionInfo.Server, _
                "MobizDUC_DataTag",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity)
            pDataUntagger.Execute()
            IO.File.Delete(mImportResultFilename)

        End Sub

        Private Sub UntagWholesaleToMobiz()

            Dim pDataUntagger As DataUntagger

            If IO.File.Exists(mImportResultFilename) = False Then Exit Sub
            pDataUntagger = New DataUntagger(mImportResultFilename)
            pDataUntagger.ProData = ProData.CreateFromConnectionInfo( _
                Common.ConnectionInfo.Server, _
                "Wholesale_DataTag",
                Common.ConnectionInfo.Username, _
                Common.ConnectionInfo.Password, _
                Common.ConnectionInfo.IntegratedSecurity)
            pDataUntagger.Execute()
            IO.File.Delete(mImportResultFilename)

        End Sub
    End Class
End Class
