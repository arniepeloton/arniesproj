Public Class PremiseDatabasesMaker
    Private Const mPublicDatabaseName As String = "Mobiz"
    Dim mPrivateDatabaseName As String
    Dim mModifiedPublicDatabaseFilename As String
    Dim mModifiedPrivateDatabaseFilename As String
    Public OrganizationID As String
    Public ConnectionInfo As ConnectionInfo ' no need for database
    Public DatabaseFolder As String
    Public BlankPublicDatabaseFilename As String

    Event Completed(sender As PremiseDatabasesMaker, e As CompletedArgs)

    Public Sub Execute()

        Dim pError As String = Nothing ' set to nothing to prevent compiler warning
        Dim pErrorType As DbProvisioner.CompletedArgs.eErrorType

        If Verify(pError, pErrorType) Then
            RestoreBlankPublic()
        End If

    End Sub

    Public ReadOnly Property PublicDatabaseFilename As String
        Get

            Return mModifiedPublicDatabaseFilename

        End Get
    End Property

    Public ReadOnly Property PrivateDatabaseFilename As String
        Get

            Return mModifiedPrivateDatabaseFilename

        End Get
    End Property

    Private Function Verify(ByRef oError As String, ByRef oErrorType As DbProvisioner.CompletedArgs.eErrorType) As Boolean

        Dim pDR As DataRow
        Dim mBusiness As Business
        Dim mOrgName As String
        Dim pProData As ProData

        Try
            oError = ""
            oErrorType = DbProvisioner.CompletedArgs.eErrorType.Unknown

            If CStr2(OrganizationID).Trim = "" Then
                oError = "vOrgID must not be blank vbKZLFK62U6A!."
                Return False
            End If

            If IsGuid(OrganizationID) = False Then
                oError = "vOrgID must be a valid GUID vbQTAY3C8EVP!."
                Return False
            End If

            pProData = ProData.CreateFromConnectionInfo(
                vServer:=ConnectionInfo.Server,
                vDatabase:=mPublicDatabaseName,
                vIntegratedSecurity:=ConnectionInfo.IntegratedSecurity,
                vUsername:=ConnectionInfo.Username,
                vPassword:=ConnectionInfo.Password)

            mBusiness = New Business(pProData)
            pDR = mBusiness.LibraryOrganization.ByID(OrganizationID)
            If pDR Is Nothing Then
                oError = "vOrgID, " & OrganizationID & ", does not exist vb3T5KNVEH8E!"
                Return False
            End If

            If CStr2(pDR("Database")).Trim = "" Then
                oErrorType = DbProvisioner.CompletedArgs.eErrorType.ExistingSubcription
                oError = "vOrgID, " & OrganizationID & ", is not subscribed vbA7B3UL4G8Y!"
                Return False
            End If
            mPrivateDatabaseName = pDR("Database")

            If CStr2(pDR("CompanyName")).Trim = "" Then
                oError = "vOrgID, " & OrganizationID & ", has blank company name vb4S7LKWXHLW!"
                Return False
            End If
            mOrgName = pDR("CompanyName")

            Return True
        Catch ex As System.Exception
            oErrorType = DbProvisioner.CompletedArgs.eErrorType.Unknown
            oError = ex.Message
        End Try

    End Function

    Private Sub RestoreBlankPublic()
        ' async completion

        Dim pProData As ProData
        Dim pDbRestore As DbRestore

        pProData = ProData.CreateFromConnectionInfo(ConnectionInfo)
        pDbRestore = New DbRestore()
        pDbRestore.ProData = pProData
        pDbRestore.SourceFilename = BlankPublicDatabaseFilename
        pDbRestore.DestinationFolder = DataBaseFolder
        pDbRestore.DestinationDatabaseName = mPublicDatabaseName & "_Public_" & ReadyGuid(OrganizationID) & "_" & DateTimeStamp()
        AddHandler pDbRestore.Completed,
            Sub(sender As Object, e As DbBackupRestoreCompletedArgs)
                If e.Sucess Then
                    PopulateBlankPublic(pDbRestore.DestinationDatabaseName)
                Else
                    ' failure
                    Stop2()
                    If e.SqlError IsNot Nothing Then
                        RaiseEvent Completed(Me, New CompletedArgs(e.SqlError))
                    ElseIf e.Exception IsNot Nothing Then
                        RaiseEvent Completed(Me, New CompletedArgs(e.Exception))
                    Else
                        RaiseEvent Completed(Me, New CompletedArgs("Restore database failure: unknown error"))
                    End If
                End If
            End Sub
        AddHandler pDbRestore.ErrorMessage,
            Sub(sender As Object, e As Exception)

            End Sub
        pDbRestore.Execute()

    End Sub

    Private Sub PopulateBlankPublic(ByVal vDestinationPublicDatabaseName As String)
        ' async completion

        Dim pBulkCopier As BulkCopier

        pBulkCopier = New BulkCopier()
        pBulkCopier.OrganizationID = OrganizationID
        pBulkCopier.SourceProData = ProData.CreateFromConnectionInfo( _
            vServer:=ConnectionInfo.Server,
            vDatabase:=mPublicDatabaseName,
            vIntegratedSecurity:=ConnectionInfo.IntegratedSecurity,
            vUsername:=ConnectionInfo.Username,
            vPassword:=ConnectionInfo.Password
        )
        pBulkCopier.DestinationConnection = New SqlConnection( _
            ProData.CreateConnectionString( _
                vServer:=ConnectionInfo.Server,
                vDatabase:=vDestinationPublicDatabaseName,
                vIntegratedSecurity:=ConnectionInfo.IntegratedSecurity,
                vUsername:=ConnectionInfo.Username,
                vPassword:=ConnectionInfo.Password
            )
        )
        pBulkCopier.DestinationConnection.Open()
        pBulkCopier.CopyPublic()
        BackupModifiedPublic(vDestinationPublicDatabaseName) 'BackupPrivate()

    End Sub

    Private Sub BackupModifiedPublic(vSourcePublicDatabaseName As String)

        Dim pDbBackup As DbBackup

        pDbBackup = New DbBackup
        pDbBackup.ProData = ProData.CreateFromConnectionInfo(
            vServer:=ConnectionInfo.Server,
            vDatabase:=vSourcePublicDatabaseName,
            vIntegratedSecurity:=ConnectionInfo.IntegratedSecurity,
            vUsername:=ConnectionInfo.Username,
            vPassword:=ConnectionInfo.Password
        )
        pDbBackup.SourceDatabase = vSourcePublicDatabaseName
        pDbBackup.Filename = AppendBackslash(DataBaseFolder) & "Mobiz_" & ReadyGuid(OrganizationID) & "_" & DateTimeStamp() & ".bak"
        AddHandler pDbBackup.Completed, _
            Sub(sender As Object, e As DbBackupRestoreCompletedArgs)
                If e.Sucess Then
                    DropModifiedPublic(pDbBackup.ProData.Clone(), vSourcePublicDatabaseName)
                    mModifiedPublicDatabaseFilename = pDbBackup.Filename
                    BackupPrivate()
                Else
                    Stop2()
                    If e.SqlError IsNot Nothing Then
                        RaiseEvent Completed(Me, New CompletedArgs(e.SqlError))
                    ElseIf e.Exception IsNot Nothing Then
                        RaiseEvent Completed(Me, New CompletedArgs(e.Exception))
                    Else
                        RaiseEvent Completed(Me, New CompletedArgs("Backup database failure: unknown error"))
                    End If
                End If
            End Sub
        pDbBackup.Execute()

    End Sub

    Private Sub DropModifiedPublic(vProData As ProData, vPublicDatabaseName As String)

        Dim pSql As String

        pSql = "ALTER DATABASE " & EscapeSqlName(vPublicDatabaseName) & _
            " SET OFFLINE" & _
            " WITH ROLLBACK IMMEDIATE"
        Try
            vProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Stop2()
        End Try

        pSql = "ALTER DATABASE " & EscapeSqlName(vPublicDatabaseName) & _
            " SET ONLINE" & _
            " WITH ROLLBACK IMMEDIATE"
        Try
            vProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Stop2()
        End Try

        pSql = "ALTER DATABASE " & EscapeSqlName(vPublicDatabaseName) & " SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
        vProData.ExecuteSQL(pSql)

        pSql = "DROP DATABASE " & EscapeSqlName(vPublicDatabaseName)
        vProData.ExecuteSQL(pSql)

    End Sub

    Private Sub BackupPrivate()
        ' async completion

        Dim pDbBackup As DbBackup

        pDbBackup = New DbBackup()
        pDbBackup.ProData = ProData.CreateFromConnectionInfo(
            vServer:=ConnectionInfo.Server,
            vDatabase:=mPrivateDatabaseName,
            vIntegratedSecurity:=ConnectionInfo.IntegratedSecurity,
            vUsername:=ConnectionInfo.Username,
            vPassword:=ConnectionInfo.Password
        )
        pDbBackup.SourceDatabase = mPrivateDatabaseName
        pDbBackup.Filename = AppendBackslash(DatabaseFolder) & "Private_" & ReadyGuid(OrganizationID) & "_" & DateTimeStamp() & ".bak"
        AddHandler pDbBackup.Completed, _
            Sub(sender As Object, e As DbBackupRestoreCompletedArgs)
                If e.Sucess Then
                    mModifiedPrivateDatabaseFilename = pDbBackup.Filename
                    RaiseEvent Completed(Me, New CompletedArgs() With {.Success = True})
                Else
                    ' failure
                    Stop2()
                    If e.SqlError IsNot Nothing Then
                        RaiseEvent Completed(Me, New CompletedArgs(e.SqlError))
                    ElseIf e.Exception IsNot Nothing Then
                        RaiseEvent Completed(Me, New CompletedArgs(e.Exception))
                    Else
                        RaiseEvent Completed(Me, New CompletedArgs("Backup database failure: unknown error"))
                    End If
                End If
            End Sub
        pDbBackup.Execute()

    End Sub

    Public Class CompletedArgs
        Public Success As Boolean
        Public SqlError As SqlError
        Public Exception As Exception
        Public ErrorMessage As String

        Public Sub New()

        End Sub

        Public Sub New(vSqlError As SqlError)

            SqlError = vSqlError

        End Sub

        Public Sub New(vException As Exception)

            Exception = vException

        End Sub

        Public Sub New(vErrorMessage As String)

            ErrorMessage = vErrorMessage

        End Sub

        Public Function GetErrorText() As String

            If Success Then
                Return Nothing
            End If

            If CStr2(ErrorMessage).Trim() <> "" Then
                Return ErrorMessage
            End If

            If SqlError IsNot Nothing Then
                Return SqlError.Message
            End If

            If Exception IsNot Nothing Then
                Return Exception.Message
            End If

            Return Nothing ' return nothing just to remove the compiler warning

        End Function
    End Class

#If DEBUG Then
    Public Shared Sub Test()

        Dim p As New PremiseDatabasesMaker

        ' restore blank public
        ' populate blank public
        ' backup populated blank public
        ' backup private

        ' server
        ' organization ID
        ' 
        p.BlankPublicDatabaseFilename = "E:\database backups-sql2\Mobiz blank.bak"
        p.OrganizationID = "cfe39df6-8809-4973-9515-810df75aed12" ' mobizABC
        p.ConnectionInfo = New ConnectionInfo()
        p.ConnectionInfo.Server = "philhr\sql2"
        p.ConnectionInfo.Username = "sa"
        p.ConnectionInfo.Password = "ducmain2"
        p.DatabaseFolder = "E:\database backups-sql2 to premise"
        AddHandler p.Completed,
            Sub(sender As PremiseDatabasesMaker, e As CompletedArgs)
                If e.Success Then
                    Debug.Print("PremiseDatabasesMaker.Completed: success")
                    Debug.Print("PublicDatabaseFilename: " & p.PublicDatabaseFilename)
                    Debug.Print("PrivateDatabaseFilename: " & p.PrivateDatabaseFilename)
                Else
                    Debug.Print("PremiseDatabasesMaker.Completed: fail: " & e.GetErrorText())
                End If
            End Sub
        p.Execute()

    End Sub
#End If

End Class
