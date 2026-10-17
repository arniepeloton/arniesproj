Public Class DbProvisionerController
    Inherits System.Web.Mvc.AsyncController

    Dim WithEvents mDbProvisioner As DbProvisioner

    <HttpPost()>
    <AsyncTimeout(Integer.MaxValue)>
    Public Overloads Sub ExecuteAsync()

        AsyncManager.OutstandingOperations.Increment()

        mDbProvisioner = New DbProvisioner
        mDbProvisioner.ProData = Common.WebRequest.ProData
        mDbProvisioner.OrgID = Common.WebRequest.Data("OrgID")
        mDbProvisioner.Execute()

    End Sub

    Public Function ExecuteCompleted(vOut As JsonDictionary) As String

        Return vOut.Serialize()

    End Function

    Private Sub mDbProvisioner_Completed(sender As Object, e As DbProvisioner.CompletedArgs) Handles mDbProvisioner.Completed

        Dim pJson As JsonDictionary

        pJson = New JsonDictionary
        If e.Success Then
            pJson.Add("Result", "ok")
            pJson.Add("DatabaseName", e.DatabaseName)
        Else
            pJson.Add("Error", e.ErrorMessage)
        End If
        AsyncManager.Parameters("vOut") = pJson
        AsyncManager.OutstandingOperations.Decrement()

    End Sub
End Class

Public Class DbProvisioner
    Private Const mTemplateDb As String = "MobizTemplate"
    Private Const mDatabaseNameMaxLength As Integer = 30

    Dim mTemplateBackupFilename As String
    Dim WithEvents mDbBackup As DbBackup
    Dim WithEvents mDbRestore As DbRestore
    Dim mOrgName As String
    Dim mOrgDatabaeName As String
    Dim mBusiness As Business

    Event Completed(sender As Object, e As CompletedArgs)
    Public OrgID As String
    Public ProData As ProData

    Public Class CompletedArgs
        Public Success As Boolean
        Public ErrorMessage As String
        Public Enum eErrorType
            Unknown
            ExistingSubcription
            Backup
            Restore
        End Enum
        Public ErrorType As eErrorType
        Public DatabaseName As String
    End Class

    Public Sub Execute()

        Call New Threading.Thread(
            Sub()
                Dim pError As String
                Dim pErrorType As CompletedArgs.eErrorType

                ProData = ProData.Clone
                If Verify(OrgID, pError, pErrorType) = False Then
                    CompleteWithError(pError, pErrorType)
                End If

                If ProData.IsFileExist(mTemplateBackupFilename) Then
                    RestoreDb()
                Else
                    mDbBackup = New DbBackup
                    mDbBackup.ProData = ProData.Clone
                    mDbBackup.SourceDatabase = mTemplateDb
                    mDbBackup.Filename = mTemplateBackupFilename
                    mDbBackup.Execute()
                End If
                ProData.Dispose()
            End Sub
        ).Start()

    End Sub

    Private Function Verify(vOrgID As String, ByRef oError As String, ByRef oErrorType As CompletedArgs.eErrorType) As Boolean

        Dim pDR As DataRow

        oError = ""

        oErrorType = CompletedArgs.eErrorType.Unknown
        If CStr2(vOrgID).Trim = "" Then
            oError = "vOrgID must not be blank vb84LD85HZ3W!."
            Return False
        End If

        If IsGuid(vOrgID) = False Then
            oError = "vOrgID must be a valid GUID vbKCDWCN2PDE!."
            Return False
        End If
        OrgID = vOrgID

        mBusiness = New Business(ProData)
        pDR = mBusiness.LibraryOrganization.ByID(vOrgID)
        If pDR Is Nothing Then
            oError = "vOrgID, " & vOrgID & ", does not exist vb92HBYCL6UD!"
            Return False
        End If

        If CStr2(pDR("Database")).Trim <> "" Then
            oErrorType = CompletedArgs.eErrorType.ExistingSubcription
            oError = "vOrgID, " & vOrgID & ", is already subscribed vb7L3UXHRH8H!"
            Return False
        End If

        If CStr2(pDR("CompanyName")).Trim = "" Then
            oError = "vOrgID, " & vOrgID & ", has blank company name vbEKPAU7JWC7!"
            Return False
        End If
        mOrgName = pDR("CompanyName")

        If CacheTemplateBackupFilename(oError) = False Then
            Return False
        End If

        Return True

    End Function

    Private Function CacheTemplateBackupFilename(ByRef oError As String) As Boolean

        Dim pFilename As String

        oError = ""
        pFilename = ProData.GetFirstDataFilePath("Mobiz")
        If CStr2(pFilename).Trim = "" Then
            oError = "Unable to determine the location of public database vbUGHQZD7DEA!"
            Return False
        End If

        pFilename = IO.Path.GetDirectoryName(pFilename)
        pFilename = AppendBackslash(pFilename)
        pFilename &= mTemplateDb & ".bak"
        mTemplateBackupFilename = pFilename

        Return True

    End Function

    Private Function CreateDatabaseName(vOrgName As String, ByRef oOutput As String, oError As String) As Boolean

        Dim pSql As String
        Dim pPath As String
        Dim pFilename As String
        Dim pDbs As List(Of String)

        Try
            pDbs = ProData.GetDatabases(True, True)
            pPath = ProData.GetFirstDataFilePath("Mobiz")
            If CStr2(pPath).Trim = "" Then
                oError = "Unable to determine the location of public database vbGC9QVCHRHQ!"
                Return False
            End If
            pPath = IO.Path.GetDirectoryName(pPath)
            pPath = AppendBackslash(pPath)

            pFilename = "Mobiz" & RemoveSymbols(vOrgName)
            For pInt As Integer = 0 To 100
                pSql = IncrementFilename(pFilename, mDatabaseNameMaxLength, pInt).Trim()
                If pDbs.Contains(pSql.ToLower()) Then Continue For
                If CDec2(ProData.ExecuteScalar("SELECT COUNT(*) FROM MobizLibrarySubscription" &
                                               " WHERE [Database] = " & ReadyString(pSql))) > 0 Then

                    Continue For
                End If

                pSql = pPath & pSql
                If ProData.IsFileExist(pSql & ".mdf") Then Continue For
                If ProData.IsFileExist(pSql & ".ldf") Then Continue For

                oOutput = IO.Path.GetFileNameWithoutExtension(pSql)
                Return True
            Next pInt
        Catch ex As System.Exception
            oError = ex.Message
        End Try

        Return False

    End Function

    Private Function CreateSubscriptionRecord() As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            pIU = New InsertUpdate("MobizLibrarySubscription")
            pIU("ID") = "newid()"
            pIU("OrganizationID") = ReadyString(OrgID)
            pIU("[Database]") = ReadyString(mOrgDatabaeName)
            pSql = pIU.GetInsert

            ProData.ExecuteSQL(pSql)
        Catch ex As System.Exception
            CompleteWithError(ex.Message, CompletedArgs.eErrorType.Unknown)
            Return False
        End Try

        Dim pE As CompletedArgs

        pE = New CompletedArgs
        pE.Success = True
        pE.DatabaseName = mOrgDatabaeName
        RaiseEvent Completed(Me, pE)

    End Function

    Private Sub mDbBackup_Completed(sender As Object, e As ProPack.DbBackupRestoreCompletedArgs) Handles mDbBackup.Completed

        If e.Sucess Then
            RestoreDb()
        Else
            CompleteWithError(e.SqlError.Message, CompletedArgs.eErrorType.Backup)
        End If

    End Sub

    Private Sub RestoreDb()

        Dim pError As String

        If CreateDatabaseName(mOrgName, mOrgDatabaeName, pError) = False Then
            CompleteWithError(pError, CompletedArgs.eErrorType.Unknown)
            Exit Sub
        End If

        mDbRestore = New DbRestore
        mDbRestore.ProData = ProData.Clone
        mDbRestore.SourceFilename = mTemplateBackupFilename
        mDbRestore.DestinationFolder = IO.Path.GetDirectoryName(mTemplateBackupFilename)
        mDbRestore.DestinationDatabaseName = mOrgDatabaeName
        mDbRestore.Execute()

    End Sub

    Private Sub CompleteWithError(ByVal vError As String, ByVal vErrorType As CompletedArgs.eErrorType)

        Dim pJson As JsonDictionary
        Dim pE As CompletedArgs

        pJson = New JsonDictionary
        pJson.Add("Error", vError)

        pE = New CompletedArgs
        pE.ErrorMessage = vError
        pE.ErrorType = vErrorType
        RaiseEvent Completed(Me, pE)

    End Sub

    Private Sub mDbRestore_Completed(sender As Object, e As ProPack.DbBackupRestoreCompletedArgs) Handles mDbRestore.Completed

        If e.Sucess Then
            CreateSubscriptionRecord()
        Else
            CompleteWithError(e.SqlError.Message, CompletedArgs.eErrorType.Restore)
        End If

    End Sub
End Class