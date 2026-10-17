Imports DataImportExport

Namespace WebProj
    Public Class MobizExportController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /MobizExport
        Dim mProData As ProData = Common.WebRequest.ProData
        Dim mZip As ProZip
        Dim mTables As New MobizExport
        'Dim mTablesLasyUpdate As New MobizExportLazyUpdate
        Dim mFilename As String

        Shared mProgress As String
        Shared mExecutingTable As String
        Dim mKey As String
        Dim vSubFolder As String
        Dim vFailureCounter As Integer
        Shared mFinishedProduct As String
        Shared mDatabase As String
        Shared mPeopleID As String
        Shared m_fileNames As DataTable
        Shared IsDatabaseExists As Boolean

        Public Enum TransType
            FromDevice
            ToDevice
        End Enum

        Function Index() As ActionResult
            Return View()
        End Function

        Function Download() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("fID")
            Dim pSourceURL As String = Common.ServerPath & "\temp\DataExport\Upload\" & pFileTitle & ".zip"
            Dim pContentType As String = "application/zip"

            Try
                Return File(pSourceURL, pContentType, pFileTitle)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        Function RequestDataFromServer() As String

            If WebRequest.Instance.HandleOrganization(True) Then 'change db
                If Not WebSession.Instance.HasCurrentRequest Then
                    If Not Common.WebRequest.Data("PeopleID") Is Nothing Then
                        WebSession.Instance.PeopleUserID = Common.WebRequest.Data("PeopleID")
                        WebSession.Instance.HasCurrentRequest = True
                    End If

                    mKey = GenerateString(75)

                    vSubFolder = GenerateString(75)

                    If IsDeviceAllowedToDownload() Then
                        If Common.WebRequest.Data("IsLazyUpdate") Is Nothing OrElse Not CBool(Common.WebRequest.Data("IsLazyUpdate")) Then
                            If Not IsDownloadDirectly() Then
                                If CreateSqliteDatabase() Then
                                    InitSQLiteDBase()
                                End If
                            End If
                        Else
                            CompareTableTime()
                        End If
                        
                    End If

                    WebSession.Instance.HasCurrentRequest = False
                    Return JsonDictionary.Serialize("Results", mProgress)
                End If
            End If
        End Function

        Private Sub CompareTableTime()
            Dim StrData As String()
            Dim m_Tables As New DataTable
            Dim m_TableCount As Integer

            UpdateTablesLastUpdatedTable()

            m_Tables.Columns.Add("Table")
            m_Tables.Columns.Add("DeviceTime")
            m_Tables.Columns.Add("ServerTime")

            For Each pObj As Object In Common.WebRequest.AllData("Tables")

                StrData = Split(pObj.ToString, ",")

                Dim pSql As String
                Dim pDt As DataTable

                pSql = "Select LastUpdated from TablesLastUpdated where TableName = " & ReadyString(StrData(0))

                pDt = Common.WebRequest.ProData.Execute(pSql)

                If pDt.Rows.Count > 0 Then
                    If StrData(1) = vbNullString OrElse
                        Not DateDiff(DateInterval.Minute, CDate(pDt.Rows(0).Item("LastUpdated")), CDate(StrData(1))) = 0 Then
                        Dim dr As DataRow

                        dr = m_Tables.NewRow
                        dr(0) = StrData(0)
                        dr(1) = Replace(StrData(1), "Z", "")
                        dr(2) = pDt.Rows(0).Item("LastUpdated")

                        m_Tables.Rows.Add(dr)
                    End If
                End If
            Next

            If m_Tables.Rows.Count > 0 Then
                GetUpdatedData(m_Tables, mKey)
                Exit Sub
            End If

            mProgress = "nothing"
        End Sub

        Private Function GetUpdatedData(ByVal Tables As DataTable, ByVal mKey As String) As Boolean
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim JsonString As String
            'Dim TableNamePrivate As String
            'Dim TableNameMobiz As String
            Dim pDirectory As String
            Dim OrgID As String
            Dim pSql As String

            pDirectory = Common.ServerPath & "\temp\DataExport\Upload\"

            If Not System.IO.Directory.Exists(pDirectory) Then
                IO.Directory.CreateDirectory(pDirectory)
            End If

            If System.IO.File.Exists(pDirectory & "\" & mKey & ".zip") Then
                System.IO.File.Delete(pDirectory & "\" & mKey & ".zip")
            End If

            mFilename = pDirectory & "\" & mKey & ".zip" '

            If Not IO.File.Exists(mFilename) Then
                CreateZip()
            End If

            Static sSL As New Threading.SpinLock
            Dim pStarted As Boolean

            If sSL.IsHeld Then Return False ' app is already running

            mDatabase = mProData.GetCurrentDatabase()
            mPeopleID = WebSession.Instance.PeopleUserID
            'IsDatabaseExists = CBool(Common.WebRequest.Data("IsDatabaseExists"))

            TimedThread(
                Sub()

                    Try
                        If sSL.IsHeld Then Exit Sub
                        sSL.Enter(False)

                        mProgress = 0
                        mFinishedProduct = Nothing

                        Dim mBusiness As Business

                        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))

                        mBusiness.ProData.Execute("USE " & EscapeSqlName(mDatabase))

                        For i As Integer = 0 To Tables.Rows.Count - 1

                            mProgress = "Requesting Data from Server"

                            'TableNameMobiz = If(CStr(Tables.Rows(i).Item("Table")).Contains("Mobiz"), GetMobizTable(CStr(Tables.Rows(i).Item("Table"))), CStr(Tables.Rows(i).Item("Table")))

                            'TableNamePrivate = CStr(Tables.Rows(i).Item("Table"))

                            'If Not CStr(Tables.Rows(i).Item("Table")).Contains("Mobiz") Then
                            pSql = "DECLARE @pSql1 varchar(1000) " & vbCrLf
                            pSql &= "DECLARE @pSql2 varchar(1000) " & vbCrLf

                            Dim pTableName As String

                            pTableName = IIf(CStr(Tables.Rows(i).Item("Table")).Contains("Mobiz"), GetMobizTable(CStr(Tables.Rows(i).Item("Table"))), CStr(Tables.Rows(i).Item("Table")))

                            pSql &= "SET @pSql1 = 'SELECT DISTINCT * " & pTableName & "'" & vbCrLf
                            pSql &= "SET @pSql2 = 'SELECT DISTINCT * from " & pTableName & " Where LastUpdated BETWEEN '" & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & "' AND '" & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime")))) & "''" & vbCrLf 'extra "'" cause this is sql script

                            pSql &= "IF COL_LENGTH(" & ReadyString(pTableName) & ",'LastUpdated') IS NULL " & vbCrLf &
                                    " BEGIN " & vbCrLf &
                                    " EXECUTE(@pSql1) " & vbCrLf &
                                    " END " & vbCrLf &
                                    " ELSE " & vbCrLf &
                                    " BEGIN " & vbCrLf &
                                    " EXECUTE(@pSql2) " & vbCrLf &
                                    " END"

                            pDT = mBusiness.ProData.Execute(pSql)

                            AddFileToZip(pDT, CStr(Tables.Rows(i).Item("Table")))


                            Dim TableNameParent As String = CStr(Tables.Rows(i).Item("Table"))
                            Dim pSqlIDs As String = ""

                            If pDT.Rows.Count > 0 Then
                                pSqlIDs = ReadyString(CStr2(pDT.Rows(0).Item("ID")))

                                For j As Integer = 1 To pDT.Rows.Count - 1
                                    pSqlIDs += "," + ReadyString(CStr2(pDT.Rows(j).Item("ID")))
                                Next
                            End If


                            If mTables.ParentTables(TableNameParent).Children.Count > 0 Then
                                For j As Integer = 0 To mTables.ParentTables(TableNameParent).Children.Count - 1

                                    Dim TableNameChild = mTables.ParentTables(TableNameParent).Children.Values(j).ChildTable
                                    Dim pSqlFromFor3rdLayer As String

                                    mExecutingTable = TableNameChild

                                    pSql = "Select DISTINCT * From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")

                                    pSqlFromFor3rdLayer = "Select DISTINCT ID From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")

                                    pDT = mBusiness.ProData.Execute(pSql)

                                    '''Insert into SQL Lite
                                    AddFileToZip(pDT, CStr(TableNameChild))
                                    'CreateAndSaveToSQLiteTable(strOrgID, TableNameChild, pDT)

                                    If pDT.Rows.Count > 0 Then
                                        Dim m_TableNameChild As String()

                                        If TableNameChild = "MobizLibraryOrganizationFOWST" Then
                                            m_TableNameChild = {"FowstID", "MobizLibraryOrganizationFOWSTTel", "MobizLibraryOrganizationFOWSTEmail", "MobizLibraryOrganizationFOWSTOperationHRs"}
                                        ElseIf TableNameChild = "MobizLibraryOrganizationRepresentatives" Then
                                            m_TableNameChild = {"OrgRepID", "MobizLibraryOrganizationRepresentativesEmail", "MobizLibraryOrganizationRepresentativesMobile", "MobizLibraryOrganizationRepresentativesTel"}
                                        End If

                                        If Not m_TableNameChild Is Nothing Then
                                            For k As Integer = 1 To m_TableNameChild.Length - 1

                                                mExecutingTable = m_TableNameChild(k)
                                                mProgress = "Requesting Data from Server"

                                                If (m_TableNameChild(k) = "MobizLibraryOrganizationRepresentativesEmail" Or
                                                    m_TableNameChild(k) = "MobizLibraryOrganizationRepresentativesMobile" Or
                                                    m_TableNameChild(k) = "MobizLibraryOrganizationRepresentativesTel") Then

                                                    pSql = "Select DISTINCT orgrepchld.* From " & m_TableNameChild(k) & " orgrepchld " & vbCrLf &
                                                           " INNER JOIN MobizLibraryOrganizationRepresentatives orgrep ON (orgrep.ID = orgrepchld.OrgRepID) " & vbCrLf &
                                                           " where orgrep.OrganizationID IN (" & pSqlIDs & ")"

                                                ElseIf (m_TableNameChild(k) = "MobizLibraryOrganizationFOWSTTel" Or
                                                    m_TableNameChild(k) = "MobizLibraryOrganizationFOWSTEmail" Or
                                                    m_TableNameChild(k) = "MobizLibraryOrganizationFOWSTOperationHRs") Then

                                                    pSql = "Select DISTINCT orgfowstchld.* From " & m_TableNameChild(k) & " orgfowstchld " & vbCrLf &
                                                           " INNER JOIN MobizLibraryOrganizationFOWST orgfowst ON (orgfowst.ID = orgfowstchld.FOWSTID) " & vbCrLf &
                                                           " where orgfowst.OrganizationID IN (" & pSqlIDs & ")"

                                                Else

                                                    pSql = "Select DISTINCT * From " & m_TableNameChild(k) & vbCrLf &
                                                                " where " & m_TableNameChild(0) & " IN (" & pSqlFromFor3rdLayer & ")"
                                                End If

                                                Dim pDTChild As DataTable

                                                pDTChild = mBusiness.ProData.Execute(pSql)

                                                '''Insert into SQL Lite
                                                AddFileToZip(pDT, m_TableNameChild(k))

                                                'CreateAndSaveToSQLiteTable(strOrgID, m_TableNameChild(k), pDTChild)

                                            Next
                                        End If
                                        m_TableNameChild = Nothing
                                    End If
                                Next
                            End If
                        Next

                        mProgress = mKey

                        mFinishedProduct = DateTimeStamp()
                    Catch ex As Exception
                        mProgress = "Error in getting updates. Please contact Mobiz for assistance."
                    Finally
                        If sSL.IsHeldByCurrentThread Then
                            ' do something here
                            sSL.Exit()
                        End If
                    End Try

                End Sub)
        End Function

        Private Function IsDeviceAllowedToDownload() As Boolean
            Dim pSql As String
            Dim pDt As DataTable

            pSql = "Select * from LibraryDevice where UUID = " & ReadyString(Common.WebRequest.Data("DeviceUUID")) & " AND IsActive=1"

            pDt = Common.WebRequest.ProData.Execute(pSql)

            If pDt.Rows.Count > 0 Then
                Return True
            Else
                mProgress = "Error"
            End If
        End Function

        Private Function IsDownloadDirectly() As Boolean
            Dim pDirectory As String
            Dim pSql As String
            Dim dtCurrentDB As DataTable
            Dim dtLastUpdate As DataTable

            'Edited By: Francis H
            'Date: 06/19/14

            pSql = "Select * From TablesLastUpdated where TableName = 'Current Database'"

            dtCurrentDB = mProData.Execute(pSql)

            If dtCurrentDB.Rows.Count = 0 Then
                pSql = "Insert Into TablesLastUpdated(TableName, FileName, LastUpdated) values('Current Database', 'Creating Database', Getdate())"
                mProData.ExecuteSQL(pSql)

                pSql = "Insert Into TablesLastUpdated(TableName, FileName, LastUpdated) values('Previous Database', '', Getdate())"
                mProData.ExecuteSQL(pSql)

                mProgress = "Creating Database"
            ElseIf CStr(dtCurrentDB.Rows(0).Item("FileName")) = "Creating Database" Then
                'kulang pa ni
                mProgress = "Creating Database"
            ElseIf Not System.IO.File.Exists(Common.ServerPath & "temp\DataExport\Upload\" & CStr(dtCurrentDB.Rows(0).Item("FileName")) & ".zip") Then
                mProgress = "Creating Database"
            Else
                pSql = "Select top 1 LastUpdated from TablesLastUpdated order by LastUpdated Desc"
                dtLastUpdate = mProData.Execute(pSql)

                'if yes, then create db ***New Updates Found from Libraries 
                If DateDiff(DateInterval.Minute, CDate(dtCurrentDB.Rows(0).Item("LastUpdated")), CDate(dtLastUpdate.Rows(0).Item("LastUpdated"))) > 0 Then
                    Dim dtPreviousDB As DataTable

                    pSql = "Select * From TablesLastUpdated where TableName = 'Previous Database'"

                    dtPreviousDB = mProData.Execute(pSql)

                    If dtPreviousDB.Rows.Count > 0 Then
                        'If System.IO.File.Exists(Common.ServerPath & "temp\DataExport\Upload\" & CStr(dtPreviousDB.Rows(0).Item("FileName")) & ".zip") Then
                        '    'delete Previous Database
                        '    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & CStr(dtPreviousDB.Rows(0).Item("FileName")) & ".zip")
                        'End If

                        'Rename Previous Database to Current Database
                        pSql = "Update TablesLastUpdated Set FileName = " & ReadyString(CStr(dtCurrentDB.Rows(0).Item("FileName"))) &
                                ", LastUpdated = " & ReadyString(dtCurrentDB.Rows(0).Item("LastUpdated")) &
                                " Where TableName = 'Previous Database'"
                        mProData.ExecuteSQL(pSql)
                    End If


                    'Change current db to 'Creating Database'
                    pSql = "Update TablesLastUpdated Set FileName = 'Creating Database' " &
                                    " Where TableName = 'Current Database'"
                    mProData.ExecuteSQL(pSql)

                    mProgress = "Creating Database"
                Else
                    mProgress = CStr(dtCurrentDB.Rows(0).Item("FileName"))
                    Return True
                End If
            End If

            Return False
            'pDirectory = Common.ServerPath & "\temp\DataExport\Upload\"

            'If IO.File.Exists(pDirectory & Common.WebRequest.Data("OrgID") & ".zip") Then
            '    pSql = "Select LastUpdated From TablesLastUpdated where TableName = 'Database'"
            '    dtDatabaseLastUpdated = mProData.Execute(pSql)

            '    If dtDatabaseLastUpdated.Rows.Count = 0 Then
            '        pSql = "Insert Into TablesLastUpdated(TableName, LastUpdated) values('Database', Getdate())"
            '        mProData.ExecuteSQL(pSql)
            '        Return False
            '        Exit Function
            '    Else
            '        pSql = "Select top 1 LastUpdated from TablesLastUpdated order by LastUpdated Desc"
            '        dtLastUpdate = mProData.Execute(pSql)

            '        If DateDiff(DateInterval.Minute, CDate(dtDatabaseLastUpdated.Rows(0).Item("LastUpdated")), CDate(dtLastUpdate.Rows(0).Item("LastUpdated"))) > 0 Then
            '            Return False
            '            Exit Function
            '        End If
            '    End If
            '    Return True
            'Else
            '    Return False
            'End If
        End Function

        'Private Function RequestAllDatabaseData(ByVal mKey) As String
        '    Dim pFinder As Finder
        '    Dim pDT As DataTable
        '    Dim JsonString As String
        '    Dim TableNamePrivate As String
        '    Dim TableNameMobiz As String
        '    Dim pDirectory As String
        '    Dim OrgID As String
        '    Dim pSql As String

        '    pDirectory = Common.ServerPath & "\temp\DataExport\Upload\"

        '    If Not System.IO.Directory.Exists(pDirectory) Then
        '        IO.Directory.CreateDirectory(pDirectory)
        '    End If

        '    mFilename = pDirectory & "\" & mKey & ".zip" '

        '    If Not IO.File.Exists(mFilename) Then
        '        CreateZip()
        '    End If

        '    Static sSL As New Threading.SpinLock
        '    Dim pStarted As Boolean

        '    If sSL.IsHeld Then Return False ' app is already running

        '    mDatabase = mProData.GetCurrentDatabase()
        '    mPeopleID = WebSession.Instance.PeopleUserID
        '    IsDatabaseExists = CBool(Common.WebRequest.Data("IsDatabaseExists"))

        '    TimedThread(
        '        Sub()

        '            Try
        '                If sSL.IsHeld Then Exit Sub
        '                sSL.Enter(False)

        '                mProgress = 0
        '                mFinishedProduct = Nothing

        '                Dim mBusiness As Business

        '                mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))

        '                mBusiness.ProData.Execute("USE " & EscapeSqlName(mDatabase))

        '                For i As Integer = 0 To mTables.ParentTables.Count - 1

        '                    'mProgress = mTables.ParentTables.Values(i).Name
        '                    mProgress = "Requesting Data from Server"

        '                    TableNameMobiz = If(mTables.ParentTables.Values(i).Name.Contains("Mobiz"), GetMobizTable(mTables.ParentTables.Values(i).Name), mTables.ParentTables.Values(i).Name)

        '                    TableNamePrivate = mTables.ParentTables.Values(i).Name

        '                    '3rd level table ex. Pim->PimMeasurement->*MobizLibraryPIMProductMeasurementBarcode

        '                    If TableNamePrivate = "MobizLibraryPIMProduct" Then
        '                        pSql = "SELECT " & IIf(Not IsDatabaseExists, " TOP 1 ", "") & " MobizLibraryPIMProduct.ID, MobizLibraryPIMProduct.LastUpdated from MobizLibraryPIMProduct " & vbCrLf &
        '                            " INNER JOIN LibraryOrganizationPIM ON (LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID) "
        '                        'OR " & vbCrLf &
        '                        '                                "MobizLibraryPIMProduct.ID IN (Select LibraryOrganizationCustomerProduct.ProductID from LibraryOrganizationCustomerProduct) "
        '                    ElseIf TableNamePrivate = "MobizLibraryOrganization" Then
        '                        pSql = "SELECT DISTINCT " & IIf(Not IsDatabaseExists, " TOP 1 ", "") & " MobizLibraryOrganization.ID, MobizLibraryOrganization.LastUpdated from MobizLibraryOrganization " & vbCrLf &
        '                                "WHERE MobizLibraryOrganization.ID IN (Select LibraryOrganizationCustomer.OrganizationID as OrgID from LibraryOrganizationCustomer " & vbCrLf &
        '                                    " UNION " & vbCrLf &
        '                                    " Select LibraryOrganizationSupplier.SupplierID as OrgID from LibraryOrganizationSupplier " & vbCrLf &
        '                                    " UNION " & vbCrLf &
        '                                    " Select TransReceiving.SupplierID as OrgID from TransReceiving " & vbCrLf &
        '                                    " UNION " & vbCrLf &
        '                                    " Select OrganizationID as OrgID from MobizLibraryPeopleOrganization where PeopleID = " & ReadyString(mPeopleID) &
        '                                    " UNION " & vbCrLf &
        '                                    " SELECT DISTINCT prd.PublishedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
        '                                    " UNION " & vbCrLf &
        '                                    " SELECT DISTINCT prd.UploadedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
        '                                    ") "
        '                    ElseIf TableNamePrivate = "MobizLibraryPeople" Then
        '                        pSql = "SELECT DISTINCT " & IIf(Not IsDatabaseExists, " TOP 1 ", "") & " MobizLibraryPeople.ID, MobizLibraryPeople .LastUpdated from MobizLibraryPeople " & vbCrLf &
        '                                " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "
        '                    ElseIf TableNamePrivate = "MobizLibraryPIMProductMeasurementBarcode" Then
        '                        pSql = "SELECT " & IIf(Not IsDatabaseExists, " TOP 1 ", "") & " MobizLibraryPIMProductMeasurementBarcode.ID from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
        '                            "INNER JOIN MobizLibraryPIMProductMeasurement ON  " & vbCrLf &
        '                            "MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
        '                            "INNER JOIN LibraryOrganizationPIM ON  " & vbCrLf &
        '                            "MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"
        '                    ElseIf TableNamePrivate = "MobizLibraryPIMProductPriceLevel" Then
        '                        pSql = "Select " & IIf(Not IsDatabaseExists, " TOP 1 ", "") & " MobizLibraryPIMProductPriceLevel.ID, MobizLibraryPIMProductPriceLevel.LastUpdated from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID"
        '                    Else
        '                        pSql = "DECLARE @pSql1 varchar(1000) " & vbCrLf
        '                        pSql &= "DECLARE @pSql2 varchar(1000) " & vbCrLf

        '                        If (Not IsDatabaseExists) Then
        '                            pSql &= "SET @pSql1 = 'SELECT TOP 1 ID from " & TableNamePrivate & "'" & vbCrLf
        '                            pSql &= "SET @pSql2 = 'SELECT TOP 1 ID, LastUpdated from " & TableNamePrivate & "'" & vbCrLf
        '                        Else
        '                            pSql &= "SET @pSql1 = 'SELECT ID from " & TableNamePrivate & "'" & vbCrLf
        '                            pSql &= "SET @pSql2 = 'SELECT ID, LastUpdated from " & TableNamePrivate & "'" & vbCrLf
        '                        End If

        '                        pSql &= "IF COL_LENGTH(" & ReadyString(TableNameMobiz) & ",'LastUpdated') IS NULL " & vbCrLf &
        '                                " BEGIN " & vbCrLf &
        '                                " EXECUTE(@pSql1) " & vbCrLf &
        '                                " END " & vbCrLf &
        '                                " ELSE " & vbCrLf &
        '                                " BEGIN " & vbCrLf &
        '                                " EXECUTE(@pSql2) " & vbCrLf &
        '                                " END"
        '                    End If

        '                    pDT = mBusiness.ProData.Execute(pSql)

        '                    AddFileToZip(pDT, TableNamePrivate)
        '                Next

        '                mProgress = mKey

        '                mFinishedProduct = DateTimeStamp()
        '            Catch ex As Exception
        '                mProgress = "Error in " & mProgress
        '            Finally
        '                If sSL.IsHeldByCurrentThread Then
        '                    ' do something here
        '                    sSL.Exit()
        '                End If
        '            End Try

        '        End Sub)
        'End Function

        Public Function RequestFollowUp() As String

            Return JsonDictionary.Serialize("Result", mProgress)

        End Function

        Function GetMobizTable(ByVal tablename) As String
            Return "Mobiz" & ".dbo." & Replace(tablename, "Mobiz", "")
        End Function

        Function UploadRequestedData(ByVal objFile As String) As String
            Dim pDirectory As String
            Dim pFilename As String
            Dim pFS As IO.FileStream
            Dim pSR As IO.StreamReader
            Dim m_file As System.IO.FileInfo
            Dim pJsonString As String
            Dim pJsonDictionary As JsonDictionary
            Dim pDT As DataTable
            Dim pDTChild As DataTable
            Dim ctr As Integer
            Dim TableNameParent As String
            Dim mZipDownloaded As ProZip
            Dim JsonString As String
            'Dim mKey As String
            Dim pLine As Dictionary(Of String, String)
            Dim pCsvReader As CsvReader

            If Not WebRequest.Instance.HandleOrganization(True) Then Exit Function 'change db 

            Try

                pDirectory = Common.ServerPath & "temp\DataExport\Download\"

                If Not System.IO.Directory.Exists(pDirectory) Then
                    IO.Directory.CreateDirectory(pDirectory)
                End If

                ' pFilename = pDirectory & Replace(WebRequest.Instance.Data("OrgID"), "-", "") & Format(Now, "MMddyyyyhhmmss") & ".zip"
                If Not CBool(Common.WebRequest.Data("IsDatabaseExists")) Then
                    pFilename = Common.ServerPath & "temp\DataExport\Upload\" & Common.WebRequest.Data("MainKey") & ".zip"
                Else
                    pFilename = AndroidJsTestController.FirstFilename(Common.WebRequest.Data("TimeStamp"))
                End If

                If pFilename = "" Then
                    Return JsonDictionary.Serialize("Results", "An Error occur while file")
                End If
                ' Request.Files(0).SaveAs(pFilename)

                'open downloaded zip file
                mZipDownloaded = New ProZip(pFilename)
                mZipDownloaded.Open(pFilename)

                mKey = GenerateString(75)

                'create new zip file to be uploaded
                mFilename = Common.ServerPath & "\temp\DataExport\Upload\" & mKey & ".zip"
                CreateZip()

                'get all files in downloaded zip file
                m_fileNames = mZipDownloaded.GetFilenames

                Static sSL As New Threading.SpinLock
                Dim pStarted As Boolean

                If sSL.IsHeld Then Return False ' app is already running

                IsDatabaseExists = CBool(Common.WebRequest.Data("IsDatabaseExistsActual"))
                mDatabase = mProData.GetCurrentDatabase()
                mPeopleID = WebSession.Instance.PeopleUserID
                'pStarted = mProgress = 0
                TimedThread(
                    Sub()

                        Try
                            If sSL.IsHeld Then Exit Sub
                            sSL.Enter(False)

                            mFinishedProduct = Nothing

                            Dim mBusiness As Business

                            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))

                            mBusiness.ProData.Execute("USE " & EscapeSqlName(mDatabase))

                            For i As Integer = 0 To m_fileNames.Rows.Count - 1

                                Dim pSql As String
                                Dim pSqlIDs As String = ""
                                Dim pSqlFromFor3rdLayer As String = ""
                                Dim pSql2 As String
                                Dim fNameSplit As String()
                                Dim IDs As String = ""

                                TableNameParent = m_fileNames.Rows(i).Item("DirectoryName")

                                'mProgress = m_fileNames.Rows(i).Item("DirectoryName")
                                mProgress = "Requesting Data from Server"

                                If Not IsDatabaseExists Then
                                    If TableNameParent = "MobizLibraryPIMProduct" Then
                                        pSql = "SELECT DISTINCT MobizLibraryPIMProduct.* from MobizLibraryPIMProduct " & vbCrLf &
                                            " INNER JOIN LibraryOrganizationPIM ON (LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID) "

                                        pSqlIDs = "SELECT DISTINCT MobizLibraryPIMProduct.ID from MobizLibraryPIMProduct " & vbCrLf &
                                            " INNER JOIN LibraryOrganizationPIM ON (LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID) "

                                    ElseIf TableNameParent = "MobizLibraryOrganization" Then
                                        pSql = "SELECT DISTINCT MobizLibraryOrganization.* from MobizLibraryOrganization " & vbCrLf &
                                                "WHERE MobizLibraryOrganization.ID IN (Select LibraryOrganizationCustomer.OrganizationID as OrgID from LibraryOrganizationCustomer " & vbCrLf &
                                                    " UNION " & vbCrLf &
                                                    " Select LibraryOrganizationSupplier.SupplierID as OrgID from LibraryOrganizationSupplier " & vbCrLf &
                                                    " UNION " & vbCrLf &
                                                    " Select TransReceiving.SupplierID as OrgID from TransReceiving " & vbCrLf &
                                                    " UNION " & vbCrLf &
                                                    " Select OrganizationID as OrgID from MobizLibraryPeopleOrganization where PeopleID = " & ReadyString(mPeopleID) &
                                                    " UNION " & vbCrLf &
                                                    " SELECT DISTINCT prd.PublishedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                                    " UNION " & vbCrLf &
                                                    " SELECT DISTINCT prd.UploadedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                                ") "

                                        pSqlIDs = "Select LibraryOrganizationCustomer.OrganizationID as OrgID from LibraryOrganizationCustomer " & vbCrLf &
                                                    " UNION " & vbCrLf &
                                                    " Select LibraryOrganizationSupplier.SupplierID as OrgID from LibraryOrganizationSupplier " & vbCrLf &
                                                    " UNION " & vbCrLf &
                                                    " Select TransReceiving.SupplierID as OrgID from TransReceiving " & vbCrLf &
                                                    " UNION " & vbCrLf &
                                                    " Select OrganizationID as OrgID from MobizLibraryPeopleOrganization where PeopleID = " & ReadyString(mPeopleID) &
                                                    " UNION " & vbCrLf &
                                                    " SELECT prd.PublishedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                                    " UNION " & vbCrLf &
                                                    " SELECT prd.UploadedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) "

                                    ElseIf TableNameParent = "MobizLibraryPeople" Then
                                        pSql = "SELECT DISTINCT MobizLibraryPeople.* from MobizLibraryPeople " & vbCrLf &
                                                " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "

                                        pSqlIDs = "SELECT DISTINCT MobizLibraryPeople.ID from MobizLibraryPeople " & vbCrLf &
                                                " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "

                                    ElseIf TableNameParent = "MobizLibraryPIMProductMeasurementBarcode" Then
                                        pSql = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.* from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                            "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                            "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"

                                        pSqlIDs = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.ID from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                            "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                            "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"

                                    ElseIf TableNameParent = "MobizLibraryPIMProductPriceLevel" Then
                                        pSql = "Select DISTINCT MobizLibraryPIMProductPriceLevel.* from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID "

                                        pSqlIDs = "Select DISTINCT MobizLibraryPIMProductPriceLevel.ID from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID "

                                    Else
                                        pSql = "Select DISTINCT * from " & TableNameParent
                                        pSqlIDs = "Select ID from " & TableNameParent

                                    End If
                                Else
                                    pCsvReader = New CsvReader(pFilename, TableNameParent)
                                    pLine = pCsvReader.ReadLine
                                    If pLine Is Nothing Then Exit For

                                    pSql = "Select DISTINCT * from " & TableNameParent & vbCrLf &
                                            " WHERE ID IN ( "

                                    IDs &= ReadyString(pLine("ID"))

                                    Do
                                        pLine = pCsvReader.ReadLine
                                        If pLine Is Nothing Then Exit Do
                                        IDs &= "," & ReadyString(pLine("ID"))
                                    Loop
                                    pSql &= IDs
                                    pSql &= ")"
                                End If

                                pDT = mBusiness.ProData.Execute(pSql)
                                Debug.Print("/* SQL */" & vbCrLf & pSql)

                                AddFileToZip(pDT, TableNameParent)

                                If mTables.ParentTables(TableNameParent).Children.Count > 0 Then
                                    For j As Integer = 0 To mTables.ParentTables(TableNameParent).Children.Count - 1

                                        Dim TableNameChild = mTables.ParentTables(TableNameParent).Children.Values(j).ChildTable

                                        'mProgress = TableNameChild
                                        mProgress = "Requesting Data from Server"

                                        If Not IsDatabaseExists Then
                                            pSql = "Select DISTINCT * From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")
                                            pSqlFromFor3rdLayer = "Select DISTINCT ID From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")
                                        Else
                                            pSql = "Select DISTINCT * From " & TableNameChild & vbCrLf &
                                                " where " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN ( " & IDs & vbCrLf
                                            pSql &= ")"
                                        End If

                                        pDT = mBusiness.ProData.Execute(pSql)

                                        AddFileToZip(pDT, TableNameChild)

                                        If pDT.Rows.Count > 0 Then
                                            Dim m_TableNameChild As String()

                                            If TableNameChild = "MobizLibraryOrganizationFOWST" Then
                                                m_TableNameChild = {"FowstID", "MobizLibraryOrganizationFOWSTTel", "MobizLibraryOrganizationFOWSTEmail", "MobizLibraryOrganizationFOWSTOperationHRs"}
                                            ElseIf TableNameChild = "MobizLibraryOrganizationRepresentatives" Then
                                                m_TableNameChild = {"OrgRepID", "MobizLibraryOrganizationRepresentativesEmail", "MobizLibraryOrganizationRepresentativesMobile", "MobizLibraryOrganizationRepresentativesTel"}
                                            End If

                                            If Not m_TableNameChild Is Nothing Then
                                                For k As Integer = 1 To m_TableNameChild.Length - 1

                                                    'mProgress = m_TableNameChild(k)
                                                    mProgress = "Requesting Data from Server"

                                                    If Not IsDatabaseExists Then
                                                        pSql = "Select DISTINCT * From " & m_TableNameChild(k) & vbCrLf &
                                                                    " where " & m_TableNameChild(0) & " IN (" & pSqlFromFor3rdLayer & ")"
                                                    Else
                                                        pSql = "Select DISTINCT * From " & m_TableNameChild(k) & vbCrLf &
                                                                    " where " & m_TableNameChild(0) & " IN (" & GetChildIDS(pDT) & ")"

                                                    End If

                                                    pDTChild = mBusiness.ProData.Execute(pSql)

                                                    AddFileToZip(pDTChild, m_TableNameChild(k))
                                                Next
                                            End If
                                            m_TableNameChild = Nothing
                                        End If
                                    Next
                                End If
                            Next
                            mProgress = mKey
                            mFinishedProduct = DateTimeStamp()
                        Catch ex As Exception
                            mProgress = "Error in " & mProgress
                            AppendToLog(ex.Message)
                        Finally
                            If sSL.IsHeldByCurrentThread Then
                                ' do something here
                                sSL.Exit()
                            End If
                        End Try

                    End Sub)

                'Return JsonDictionary.Serialize("Results", mKey)
            Catch ex As Exception
                JsonDictionary.Serialize("Results", ex.Message)
                AppendToLog(ex.Message)
            End Try

        End Function

        Function InitSQLiteDBase() As String
            Dim pDirectory As String
            Dim pFilename As String
            
            Dim pFS As IO.FileStream
            Dim pSR As IO.StreamReader
            Dim m_file As System.IO.FileInfo
            Dim pJsonString As String
            Dim pJsonDictionary As JsonDictionary
            Dim pDT As DataTable
            Dim pDTChild As DataTable
            Dim ctr As Integer
            Dim TableNameParent As String
            Dim mZipDownloaded As ProZip
            Dim JsonString As String
            'Dim mKey As String
            Dim pLine As Dictionary(Of String, String)
            Dim pCsvReader As CsvReader
            Dim strOrgID As String
            Dim IsSuccessfullySaveInSqlite As Boolean = True
            Dim pSqlTemp As String


            Try

                If Not WebRequest.Instance.HandleOrganization(True) Then Exit Function 'change db 

                'pDirectory = Common.ServerPath & "temp\DataExport\Download\"

                'If Not System.IO.Directory.Exists(pDirectory) Then
                '    IO.Directory.CreateDirectory(pDirectory)
                'End If

                Static sSL As New Threading.SpinLock
                Dim pStarted As Boolean

                If sSL.IsHeld Then Return False ' app is already running

                mDatabase = mProData.GetCurrentDatabase()
                mPeopleID = WebSession.Instance.PeopleUserID
                strOrgID = WebRequest.Instance.OrgID

                'pStarted = mProgress = 0
                TimedThread(
                    Sub()

                        Try
                            If sSL.IsHeld Then Exit Sub
                            sSL.Enter(False)

                            mFinishedProduct = Nothing

                            Dim mBusiness As Business

                            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))

                            mBusiness.ProData.Execute("USE " & EscapeSqlName(mDatabase))

                            For i As Integer = 0 To mTables.ParentTables.Count - 1

                                Dim pSql As String
                                Dim pSql2 As String
                                Dim pSqlIDs As String = ""
                                Dim pSqlFromFor3rdLayer As String = ""

                                TableNameParent = mTables.ParentTables.Values(i).Name
                                mExecutingTable = TableNameParent
                                mProgress = "Requesting Data from Server"

                                If TableNameParent = "MobizLibraryPIMProduct" Then
                                    pSql = " SELECT DISTINCT MobizLibraryPIMProduct.* " & vbCrLf &
                                        " FROM MobizLibraryPIMProduct" & vbCrLf &
                                        " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                        " WHERE LibraryOrganizationPIM.IsDeactivated = 0" & vbCrLf &
                                        " UNION " & vbCrLf &
                                        " SELECT DISTINCT MobizLibraryPIMProduct.*" & vbCrLf &
                                        " FROM MobizLibraryPIMProduct" & vbCrLf &
                                        " JOIN MobizLibraryPIMProductContents " & vbCrLf &
                                        " ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                        " OR MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID"

                                    pSqlIDs = " SELECT DISTINCT MobizLibraryPIMProduct.ID " & vbCrLf &
                                        " FROM MobizLibraryPIMProduct" & vbCrLf &
                                        " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                        " WHERE LibraryOrganizationPIM.IsDeactivated = 0" & vbCrLf &
                                        " UNION " & vbCrLf &
                                        " SELECT DISTINCT MobizLibraryPIMProduct.ID" & vbCrLf &
                                        " FROM MobizLibraryPIMProduct" & vbCrLf &
                                        " JOIN MobizLibraryPIMProductContents " & vbCrLf &
                                        " ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                        " OR MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID"

                                ElseIf TableNameParent = "MobizLibraryOrganization" Then
                                    pSql = "SELECT DISTINCT MobizLibraryOrganization.* from MobizLibraryOrganization " & vbCrLf &
                                            "WHERE MobizLibraryOrganization.ID IN ( " &
                                                " Select ID as OrgID from MobizLibraryOrganization WHERE IsABank = 1 " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select LibraryOrganizationCustomer.OrganizationID as OrgID from LibraryOrganizationCustomer " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select LibraryOrganizationSupplier.SupplierID as OrgID from LibraryOrganizationSupplier " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select TransReceiving.SupplierID as OrgID from TransReceiving " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select OrganizationID as OrgID from MobizLibraryPeopleOrganization where PeopleID = " & ReadyString(mPeopleID) &
                                                " UNION " & vbCrLf &
                                                " SELECT prd.PublishedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                                " UNION " & vbCrLf &
                                                " SELECT prd.UploadedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                            ") "

                                    pSqlIDs = "" &
                                                " Select ID as OrgID from MobizLibraryOrganization WHERE IsABank = 1 " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select LibraryOrganizationCustomer.OrganizationID as OrgID from LibraryOrganizationCustomer " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select LibraryOrganizationSupplier.SupplierID as OrgID from LibraryOrganizationSupplier " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select TransReceiving.SupplierID as OrgID from TransReceiving " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select OrganizationID as OrgID from MobizLibraryPeopleOrganization where PeopleID = " & ReadyString(mPeopleID)
                                    '" UNION " & vbCrLf &
                                    '" SELECT prd.PublishedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                    '" UNION " & vbCrLf &
                                    '" SELECT prd.UploadedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) "

                                ElseIf TableNameParent = "MobizLibraryPeople" Then
                                    pSql = "SELECT DISTINCT MobizLibraryPeople.* from MobizLibraryPeople " & vbCrLf &
                                            " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "

                                    pSqlIDs = "SELECT DISTINCT MobizLibraryPeople.ID from MobizLibraryPeople " & vbCrLf &
                                            " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "

                                ElseIf TableNameParent = "MobizLibraryPIMProductMeasurementBarcode" Then
                                    pSql = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.* from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                        "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                        "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"

                                    pSqlIDs = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.ID from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                        "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                        "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"

                                ElseIf TableNameParent = "MobizLibraryPIMProductPriceLevel" Then
                                    pSql = "Select DISTINCT MobizLibraryPIMProductPriceLevel.* from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID "

                                    pSqlIDs = "Select DISTINCT MobizLibraryPIMProductPriceLevel.ID from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID "
                                ElseIf TableNameParent.ToLowerInvariant() = "MobizLibraryPIMMeasureDefinition".ToLowerInvariant() Then
                                    pSql = "SELECT DISTINCT * FROM MobizLibraryPIMMeasureDefinition" &
                                        " WHERE OrganizationID = " & ReadyString(strOrgID)

                                    pSqlIDs = "SELECT ID FROM MobizLibraryPIMMeasureDefinition" &
                                        " WHERE OrganizationID = " & ReadyString(strOrgID)
                                ElseIf TableNameParent.ToLowerInvariant() = "MobizLibraryPIMProductContents".ToLowerInvariant() Then
                                    pSql = "SELECT DISTINCT * FROM MobizLibraryPIMProductContents" &
                                        " WHERE OrganizationID = " & ReadyString(strOrgID)

                                    pSqlIDs = "SELECT ID FROM MobizLibraryPIMProductContents" &
                                        " WHERE OrganizationID = " & ReadyString(strOrgID)
                                Else
                                    pSql = "Select DISTINCT * from " & TableNameParent
                                    pSqlIDs = "Select ID from " & TableNameParent
                                End If


                                pDT = mBusiness.ProData.Execute(pSql)
                                Debug.Print("/* SQL */" & vbCrLf & pSql)

                                '''Insert into SQL Lite
                                If Not CreateAndSaveToSQLiteTable(strOrgID, TableNameParent, pDT, vSubFolder) Then
                                    IsSuccessfullySaveInSqlite = False
                                End If

                                If mTables.ParentTables(TableNameParent).Children.Count > 0 Then
                                    For j As Integer = 0 To mTables.ParentTables(TableNameParent).Children.Count - 1

                                        Dim TableNameChild = mTables.ParentTables(TableNameParent).Children.Values(j).ChildTable

                                        mExecutingTable = TableNameChild
                                        mProgress = "Requesting Data from Server"

                                        If (TableNameChild = "MobizLibraryPIMProductVariableFeatures001" Or TableNameChild = "LibraryPIMProductVariableFeatures001") Then
                                            pSql = "Select DISTINCT ID, ProductID, C0017, C0020 From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")
                                        Else
                                            pSql = "Select DISTINCT * From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")
                                        End If

                                        pSqlFromFor3rdLayer = "Select DISTINCT ID From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")

                                        pDT = mBusiness.ProData.Execute(pSql)

                                        '''Insert into SQL Lite
                                        If Not CreateAndSaveToSQLiteTable(strOrgID, TableNameChild, pDT, vSubFolder) Then
                                            IsSuccessfullySaveInSqlite = False
                                        End If

                                        If pDT.Rows.Count > 0 Then
                                            Dim m_TableNameChild As String()

                                            If TableNameChild = "MobizLibraryOrganizationFOWST" Then
                                                m_TableNameChild = {"FowstID", "MobizLibraryOrganizationFOWSTTel", "MobizLibraryOrganizationFOWSTEmail", "MobizLibraryOrganizationFOWSTOperationHRs"}
                                            ElseIf TableNameChild = "MobizLibraryOrganizationRepresentatives" Then
                                                m_TableNameChild = {"OrgRepID", "MobizLibraryOrganizationRepresentativesEmail", "MobizLibraryOrganizationRepresentativesMobile", "MobizLibraryOrganizationRepresentativesTel"}
                                            End If

                                            If Not m_TableNameChild Is Nothing Then
                                                For k As Integer = 1 To m_TableNameChild.Length - 1

                                                    mExecutingTable = m_TableNameChild(k)
                                                    mProgress = "Requesting Data from Server"

                                                    If (m_TableNameChild(k) = "MobizLibraryOrganizationRepresentativesEmail" Or
                                                        m_TableNameChild(k) = "MobizLibraryOrganizationRepresentativesMobile" Or
                                                        m_TableNameChild(k) = "MobizLibraryOrganizationRepresentativesTel") Then

                                                        pSql = "Select DISTINCT orgrepchld.* From " & m_TableNameChild(k) & " orgrepchld " & vbCrLf &
                                                               " INNER JOIN MobizLibraryOrganizationRepresentatives orgrep ON (orgrep.ID = orgrepchld.OrgRepID) " & vbCrLf &
                                                               " where orgrep.OrganizationID IN (" & pSqlIDs & ")"

                                                    ElseIf (m_TableNameChild(k) = "MobizLibraryOrganizationFOWSTTel" Or
                                                        m_TableNameChild(k) = "MobizLibraryOrganizationFOWSTEmail" Or
                                                        m_TableNameChild(k) = "MobizLibraryOrganizationFOWSTOperationHRs") Then

                                                        pSql = "Select DISTINCT orgfowstchld.* From " & m_TableNameChild(k) & " orgfowstchld " & vbCrLf &
                                                               " INNER JOIN MobizLibraryOrganizationFOWST orgfowst ON (orgfowst.ID = orgfowstchld.FOWSTID) " & vbCrLf &
                                                               " where orgfowst.OrganizationID IN (" & pSqlIDs & ")"

                                                    Else

                                                        pSql = "Select DISTINCT * From " & m_TableNameChild(k) & vbCrLf &
                                                                    " where " & m_TableNameChild(0) & " IN (" & pSqlFromFor3rdLayer & ")"
                                                    End If

                                                    pDTChild = mBusiness.ProData.Execute(pSql)

                                                    '''Insert into SQL Lite
                                                    If Not CreateAndSaveToSQLiteTable(strOrgID, m_TableNameChild(k), pDTChild, vSubFolder) Then
                                                        IsSuccessfullySaveInSqlite = False
                                                    End If
                                                Next
                                            End If
                                            m_TableNameChild = Nothing
                                        End If
                                    Next
                                End If

                                If Not IsSuccessfullySaveInSqlite Then
                                    'rollback process
                                    mProgress = "Error in creating database. Please contact Mobiz asap."
                                    Exit Sub
                                End If
                            Next

                            If System.IO.File.Exists(Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip") Then
                                System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip")
                            End If

                            'additional by janine june 30 2014 2pm. 
                            'create a copy databse of the private db to be used for testing. 
                            'the id below points to Mobiz Data Test Organization where the test data will be saved so the actual database will not be disrupted
                            If System.IO.File.Exists(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\7c794971-1cd2-4ad4-922f-36a93edde09d.db") Then
                                System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\7c794971-1cd2-4ad4-922f-36a93edde09d.db")
                            End If
                            System.IO.File.Copy(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".db", Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\7c794971-1cd2-4ad4-922f-36a93edde09d.db")
                            'end of additional by janine june 30 2014 2pm

                            mZip = New ProZip(Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip")
                            mZip.AddFolder(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder, "*")

                            pSqlTemp = "Update TablesLastUpdated Set Lastupdated = GetDate(), FileName = " & ReadyString(mKey) &
                                    " Where TableName = 'Current Database'"

                            mBusiness.ProData.Execute(pSqlTemp)

                            mProgress = mKey

                            Try
                                System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\Mobiz.db")
                                System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".db")
                                System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\7c794971-1cd2-4ad4-922f-36a93edde09d.db")
                                System.IO.Directory.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder)
                            Catch ex As Exception

                            End Try

                            mFinishedProduct = DateTimeStamp()
                        Catch ex As Exception
                            mProgress = "Error in " & mExecutingTable
                            AppendToDebug(mProgress, True, ex)
                        Finally
                            If sSL.IsHeldByCurrentThread Then
                                ' do something here
                                sSL.Exit()
                            End If
                        End Try

                    End Sub)

                'Return JsonDictionary.Serialize("Results", mKey)
            Catch ex As Exception
                JsonDictionary.Serialize("Results", ex.Message)
                AppendToLog(ex.Message)
                AppendToDebug(mProgress, True, ex)
            End Try

        End Function

        Function CreateAndSaveToSQLiteTable(ByVal vOrgID As String, ByVal vTableName As String, ByVal dtData As DataTable, ByVal vSubFolder As String) As Boolean
            Dim sqliteDB As New ProPack.ProSqlite
            Dim pSQL As String
            Dim pDT As New DataTable
            Dim pDC As DataColumn
            Dim pDRV As DataRowView
            Dim pInsertUpdate As New InsertUpdate
            Dim p As Integer
            Dim mFilenameFolder As String
            Dim mFilenameParentFolder As String
            Dim mFilenamePublicDB As String
            Dim mFilenamePrivateDB As String

            mFilenameFolder = Common.ServerPath & "temp\DataExport\Upload\" & vOrgID & "\" & vSubFolder
            mFilenameParentFolder = Common.ServerPath & "temp\DataExport\Upload\" & vOrgID

            If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameParentFolder)
            End If

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            mFilenamePrivateDB = mFilenameFolder & "\" & vOrgID & ".db"
            mFilenamePublicDB = mFilenameFolder & "\Mobiz" & ".db"

            sqliteDB = New ProPack.ProSqlite
            If (vTableName.Trim().StartsWith("Mobiz")) Then
                sqliteDB.Open(mFilenamePublicDB)
            Else
                sqliteDB.Open(mFilenamePrivateDB)
            End If

            ' '''check if Table exist
            'pSQL = "SELECT name FROM sqlite_master WHERE type='table' AND name='" & vTableName & "';"
            'pDT = sqliteDB.Execute(pSQL)

            ' '''create SQL if not exist
            'If (pDT.Rows.Count <= 0) Then

            '    pSQL = "DROP TABLE IF EXISTS [" & vTableName & "]"
            '    sqliteDB.ExecuteSQL(pSQL)

            '    pSQL = ""
            '    For Each pDC In dtData.Columns
            '        If pSQL <> "" Then pSQL &= ", "
            '        pSQL &= ReadyString(pDC.ColumnName) & " "
            '        Select Case MajorDataType(pDC.DataType)
            '            Case eMajorDataType.Date
            '                pSQL &= "DATETIME"

            '            Case eMajorDataType.Numeric
            '                pSQL &= "NUMBER"

            '            Case eMajorDataType.String
            '                pSQL &= "STRING"

            '            Case Else
            '                Throw New Exception("Invalid column type: " & pDC.DataType.ToString)
            '        End Select
            '        pSQL &= " collate nocase"
            '    Next pDC
            '    pSQL = "CREATE TABLE " & ReadyString(vTableName) & "(" & pSQL & ")"
            '    sqliteDB.ExecuteSQL(pSQL)

            'End If

            Try
                pInsertUpdate.TableName = ReadyString(Replace(vTableName, "Mobiz", ""))
                sqliteDB.BeginTransaction()
                For p = 0 To dtData.Rows.Count - 1
                    pDRV = dtData.DefaultView(p)
                    For Each pDC In dtData.Columns
                        If pDRV(pDC.ColumnName) Is DBNull.Value Then
                            pInsertUpdate.Value(ReadyString(pDC.ColumnName)) = "NULL"
                        Else
                            Select Case MajorDataType(pDC.DataType)
                                Case eMajorDataType.String
                                    pInsertUpdate.Value(ReadyString(pDC.ColumnName)) = ReadyString(pDRV(pDC.ColumnName))

                                Case eMajorDataType.Numeric
                                    pInsertUpdate.Value(ReadyString(pDC.ColumnName)) = CDec2(pDRV(pDC.ColumnName))

                                Case eMajorDataType.Date
                                    If pDC.DataType Is GetType(TimeSpan) Then
                                        pInsertUpdate.Value(ReadyString(pDC.ColumnName)) = sqliteDB.ReadyDate(New Date(CType(pDRV(pDC.ColumnName), TimeSpan).Ticks))
                                    Else
                                        pInsertUpdate.Value(ReadyString(pDC.ColumnName)) = sqliteDB.ReadyDate(pDRV(pDC.ColumnName))
                                    End If

                            End Select
                        End If
                    Next pDC
                    pSQL = pInsertUpdate.GetInsert

                    sqliteDB.ExecuteSQL(pSQL)

                Next p
                sqliteDB.CommitTransaction()
                sqliteDB.Close()
                Return True
            Catch ex As Exception
                sqliteDB.RollbackTransaction()
                sqliteDB.Close()
                Return False
                Stop2()
                ' just ignore any errors
            End Try

        End Function


        Function RequestDataFromTable() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim pJsonString As String

            If WebRequest.Instance.HandleOrganization(True) Then

                pSql = "Select * From " & WebRequest.Instance.Data("Table") & " where ID = " & ReadyString(WebRequest.Instance.Data("ID"))

                pDT = mProData.Execute(pSql)

                pJsonString = JsonDictionary.Serialize("Results", pDT)

                Return pJsonString
            End If

        End Function

        Private Sub CreateZip()
            mZip = New ProZip(mFilename)
            mZip.Open(mFilename)
        End Sub

        Private Sub AddFileToZip(ByVal pDT As DataTable, ByVal TableName As String)

            Dim pFile As IO.FileInfo

            mZip.AddTableCsv(pDT, TableName)

        End Sub

        Public Function WriteToLog(ByVal path As String, ByVal filename As String, ByVal Transaction As TransType)
            Dim pFilePath As String

            pFilePath = GetLogPath(path, filename)
            If Not System.IO.File.Exists(pFilePath) Then
                System.IO.File.Create(pFilePath)
            End If

            Dim sw As New System.IO.StreamWriter(pFilePath)

            sw.WriteLine("OrgID: " & WebRequest.Instance.Data("OrgID"))
            sw.WriteLine("Transaction Type: " & Transaction)
            sw.WriteLine("Server Date: " & Today)
            sw.WriteLine("Device: " & WebRequest.Instance.Data("DeviceID"))
            sw.WriteLine("People: " & WebRequest.Instance.Data("PeopleID"))
            sw.Close()
        End Function

        Public Function GetLogPath(ByVal Ppath As String, ByVal PFilename As String)
            Return Ppath & "\" & PFilename & ".log"
        End Function

        Public Function GetChildIDS(ByVal pDT As DataTable) As String
            Dim pSql As String

            For ctr As Integer = 0 To pDT.Rows.Count - 1
                pSql &= ReadyString(pDT.Rows(ctr).Item("ID"))

                If ctr < pDT.Rows.Count - 2 Then pSql &= ","

                ctr += 1
            Next

            Return pSql
        End Function

        Function IsUserAllowedToDOwnload() As Boolean
            Dim pSql As String
            Dim pDT As DataTable

            pSql = "SELECT dbo.LibraryOrganizationEmployee.ID AS PeopleID, dbo.LibraryDevice.ID AS DeviceID " & vbCrLf &
                    "FROM dbo.LibraryOrganizationEmployeeFOWST INNER JOIN " & vbCrLf &
                      "dbo.LibraryDeviceFowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = dbo.LibraryDeviceFowst.FowstID INNER JOIN " & vbCrLf &
                      "dbo.LibraryDevice ON dbo.LibraryDeviceFowst.DeviceID = dbo.LibraryDevice.ID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationEmployee ON dbo.LibraryOrganizationEmployeeFOWST.PeopleID = dbo.LibraryOrganizationEmployee.ID " & vbCrLf &
                      "WHERE dbo.LibraryOrganizationEmployee.ID = " & ReadyString(WebRequest.Instance.Data("PeopleID")) & vbCrLf &
                      " AND dbo.LibraryDevice.UUID = " & ReadyString(WebRequest.Instance.Data("DeviceID"))

            pDT = mProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                Return True
                Exit Function
            End If

            Return False
        End Function

        Function UpdateTablesLastUpdatedTable()
            Dim pSql As String
            Dim pDt As DataTable
            Dim pResult As String

            pSql = "Select * from TablesLastUpdated where TableName = 'MobizLibraryPeople'"

            pDt = Common.WebRequest.ProData.Execute(pSql)

            If pDt.Rows.Count > 0 Then
                For i As Integer = 0 To pDt.Rows.Count - 1
                    Try
                        pSql = "Select top 1 LastUpdated from " & CStr(pDt.Rows(i).Item("TableName")) & " Order by LastUpdated desc"
                        pResult = CStr(Common.WebRequest.ProData.ExecuteScalar(pSql))
                    Catch ex As Exception
                        pResult = ""
                    End Try

                    If pResult.Length > 0 Then
                        If Not pResult = pDt.Rows(i).Item("LastUpdated") Then
                            pSql = "Update TablesLastUpdated Set Lastupdated = " & ReadyString(pResult) & " Where TableName = " & ReadyString(CStr(pDt.Rows(i).Item("TableName")))
                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                    End If
                Next
            End If
        End Function

        Function CreateSqliteDatabase() As Boolean
            Dim pSqlitePrivateDB As ProSqlite
            Dim pSqlitePublicDB As ProSqlite

            Dim mFilenamePrivateDB As String
            Dim mFilenamePublicDB As String

            Dim mFilenamePrivateDBTemp As String
            Dim mFilenamePublicDBTemp As String

            Dim mFilenameFolder As String
            Dim mFilenameParentFolder As String

            mFilenameFolder = Common.ServerPath & "temp\DataExport\Upload\" & CStr(Common.WebRequest.Data("OrgID")) & "\" & vSubFolder
            mFilenameParentFolder = Common.ServerPath & "temp\DataExport\Upload\" & CStr(Common.WebRequest.Data("OrgID"))

            If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameParentFolder)
            End If

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            mFilenamePrivateDB = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".db"
            mFilenamePublicDB = mFilenameFolder & "\Mobiz" & ".db"

            mFilenamePrivateDBTemp = mFilenamePrivateDB & ".temp"
            pSqlitePrivateDB = New ProSqlite
            pSqlitePrivateDB.Open(mFilenamePrivateDBTemp)

            mFilenamePublicDBTemp = mFilenamePublicDB & ".temp"
            pSqlitePublicDB = New ProSqlite
            pSqlitePublicDB.Open(mFilenamePublicDBTemp)

            'mDatabase = mProData.GetCurrentDatabase()
            Dim ParentTableName As String

            Try
                mProData.BeginTransaction()
                'mProData.ExecuteSQL("USE " & mDatabase)

                For i As Integer = 0 To mTables.ParentTables.Count - 1

                    ParentTableName = mTables.ParentTables.Values(i).Name

                    If ParentTableName.Contains("Mobiz") Then
                        pSqlitePublicDB.CreateTable(Replace(ParentTableName, "Mobiz", ""), GetTableSchema(ParentTableName))
                    Else
                        pSqlitePrivateDB.CreateTable(ParentTableName, GetTableSchema(ParentTableName))
                    End If


                    If mTables.ParentTables(ParentTableName).Children.Count > 0 Then

                        For j As Integer = 0 To mTables.ParentTables(ParentTableName).Children.Count - 1

                            Dim TableNameChild = mTables.ParentTables(ParentTableName).Children.Values(j).ChildTable

                            If TableNameChild.Contains("Mobiz") Then
                                pSqlitePublicDB.CreateTable(Replace(TableNameChild, "Mobiz", ""), GetTableSchema(TableNameChild))
                            Else
                                pSqlitePrivateDB.CreateTable(TableNameChild, GetTableSchema(TableNameChild))
                            End If

                        Next
                    End If
                Next

                pSqlitePrivateDB.CreateTable("TransCustomerPayment", GetTableSchema("TransCustomerPayment"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentCheckAmount", GetTableSchema("TransCustomerPaymentCheckAmount"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentChecks", GetTableSchema("TransCustomerPaymentChecks"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentCreditCard", GetTableSchema("TransCustomerPaymentCreditCard"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentDebitCard", GetTableSchema("TransCustomerPaymentDebitCard"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentInvoices", GetTableSchema("TransCustomerPaymentInvoices"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentSRV", GetTableSchema("TransCustomerPaymentSRV"))
                pSqlitePrivateDB.CreateTable("TransCustomerPaymentOther", GetTableSchema("TransCustomerPaymentOther"))
                pSqlitePrivateDB.CreateTable("TransInventoryLedger", GetTableSchema("TransInventoryLedger"))
                pSqlitePrivateDB.CreateTable("TransInventoryProductCount", GetTableSchema("TransInventoryProductCount"))
                pSqlitePrivateDB.CreateTable("TransInventoryProductCountStock", GetTableSchema("TransInventoryProductCountStock"))
                pSqlitePrivateDB.CreateTable("TransInvoice", GetTableSchema("TransInvoice"))
                pSqlitePrivateDB.CreateTable("TransInvoiceCharges", GetTableSchema("TransInvoiceCharges"))
                pSqlitePrivateDB.CreateTable("TransInvoiceDiscount", GetTableSchema("TransInvoiceDiscount"))
                pSqlitePrivateDB.CreateTable("TransInvoiceSeniorCitizenPWDCustomer", GetTableSchema("TransInvoiceSeniorCitizenPWDCustomer")) ' additional by janine july 30
                pSqlitePrivateDB.CreateTable("TransInvoiceItems", GetTableSchema("TransInvoiceItems"))
                pSqlitePrivateDB.CreateTable("TransInvoiceItemSalesReps", GetTableSchema("TransInvoiceItemSalesReps"))
                pSqlitePrivateDB.CreateTable("TransInvoiceItemSerials", GetTableSchema("TransInvoiceItemSerials"))
                pSqlitePrivateDB.CreateTable("TransMemo", GetTableSchema("TransMemo"))
                pSqlitePrivateDB.CreateTable("TransMemoSalesReturn", GetTableSchema("TransMemoSalesReturn"))
                pSqlitePrivateDB.CreateTable("TransPIMAlerts", GetTableSchema("TransPIMAlerts"))
                pSqlitePrivateDB.CreateTable("TransReceiving", GetTableSchema("TransReceiving"))
                pSqlitePrivateDB.CreateTable("TransReceivingStock", GetTableSchema("TransReceivingStock"))
                pSqlitePrivateDB.CreateTable("TransSalesQuotations", GetTableSchema("TransSalesQuotations"))
                pSqlitePrivateDB.CreateTable("TransSalesQuotationsItems", GetTableSchema("TransSalesQuotationsItems"))
                pSqlitePrivateDB.CreateTable("TransSalesReturnLedger", GetTableSchema("TransSalesReturnLedger"))

                pSqlitePrivateDB.CreateTable("TransBomMaker", GetTableSchema("TransBomMaker"))
                pSqlitePrivateDB.CreateTable("TransBomMakerContents", GetTableSchema("TransBomMakerContents"))
                pSqlitePrivateDB.CreateTable("TransBomMakerProducts", GetTableSchema("TransBomMakerProducts"))

                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTTel", GetTableSchema("MobizLibraryOrganizationFOWSTTel"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTEmail", GetTableSchema("MobizLibraryOrganizationFOWSTEmail"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTOperationHRs", GetTableSchema("MobizLibraryOrganizationFOWSTOperationHRs"))

                pSqlitePublicDB.CreateTable("LibraryOrganizationRepresentativesEmail", GetTableSchema("MobizLibraryOrganizationRepresentativesEmail"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationRepresentativesMobile", GetTableSchema("MobizLibraryOrganizationRepresentativesMobile"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationRepresentativesTel", GetTableSchema("MobizLibraryOrganizationRepresentativesTel"))

                mProData.RollbackTransaction() ' dont commit

                pSqlitePrivateDB.Close()
                If IO.File.Exists(mFilenamePrivateDB) Then IO.File.Delete(mFilenamePrivateDB)
                IO.File.Move(mFilenamePrivateDBTemp, mFilenamePrivateDB)

                pSqlitePublicDB.Close()
                If IO.File.Exists(mFilenamePublicDB) Then IO.File.Delete(mFilenamePublicDB)
                IO.File.Move(mFilenamePublicDBTemp, mFilenamePublicDB)

                'try
                'mProgress = "34901a22-064f-4d7a-a398-f8c4b55c0709"
                Return True

            Catch ex As Exception
                Try
                    pSqlitePrivateDB.Close()
                    IO.File.Delete(mFilenamePrivateDB)

                    pSqlitePublicDB.Close()
                    IO.File.Delete(mFilenamePublicDB)
                    AppendToDebug("Error in creating " & ParentTableName & " Table for sqlite database", , ex, True)
                Catch ' just ignore any err
                End Try
                Throw
            End Try

        End Function

        Function GetTableSchema(ByVal TableName) As DataTable
            Dim pSql As String

            pSql = "Select top 0 * from " & TableName

            Return mProData.Execute(pSql)
        End Function

        Function AdjustZipFile() As String
            Dim mZipFile As ProZip
            Dim pFilename As String

            System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & Common.WebRequest.Data("OrgID") & "\" & Common.WebRequest.Data("OrgID") & ".db")
            System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & Common.WebRequest.Data("OrgID") & ".zip")

            mZip = New ProZip(Common.ServerPath & "temp\DataExport\Upload\" & Common.WebRequest.Data("OrgID") & ".zip")
            mZip.AddFolder(Common.ServerPath & "temp\DataExport\Upload\" & Common.WebRequest.Data("OrgID"), "*")

            Return JsonDictionary.Serialize("Results", Common.WebRequest.Data("OrgID"))
        End Function

     
    End Class

End Namespace

Public Class MobizExport
    Inherits DataImport

    Public Sub New()

        With MyBase.ParentTables
            .Add("MobizLibraryPIMPriceLevel")
            .Add("MobizLibraryPIMMeasure")
            .Add("MobizLibraryPIMProductMeasurementBarcode")
            .Add("MobizLibraryPIMMeasureSign")
            .Add("MobizLibraryPeoplePreNominalSuffix")
            .Add("MobizLibraryCountries")
            .Add("MobizLibraryCities")
            .Add("MobizLibraryAreaCodes")
            .Add("MobizLibraryTelcos")
            .Add("MobizLibraryTelcoPrefixes")
            .Add("MobizLibraryPIMMeasureDefinition")
            .Add("MobizLibraryPIMProductContents")
            .Add("MobizLibraryPIMMeasureConversion")

            With .Add("MobizLibraryPeople")
                With .Children
                    .Add("MobizLibraryPeopleUserAccount", "PeopleID")
                    .Add("MobizLibraryPeopleOrganization", "PeopleID")
                End With
            End With


            With .Add("MobizLibraryOrganization")
                With .Children
                    .Add("MobizLibraryOrganizationContactEmail", "OrganizationID")
                    .Add("MobizLibraryOrganizationTradeName", "OrganizationID")
                    .Add("MobizLibraryOrganizationFOWST", "OrganizationID")
                    .Add("MobizLibraryOrganizationRepresentatives", "OrganizationID")
                End With
            End With

            With .Add("MobizLibraryBanks")
                With .Children
                    .Add("MobizLibraryBankBranch", "BankID")
                End With
            End With

            With .Add("MobizLibraryPIMProduct")
                With .Children
                    .Add("MobizLibraryPIMProductMeasurement", "ProductID")
                    .Add("MobizLibraryPIMProductVariableFeatures001", "ProductID")
                End With
            End With

            .Add("MobizLibraryPIMProductPriceLevel")

            .Add("SystemDefaults")
            .Add("MobizSystemDefaults")
            .Add("MobizLibraryCountriesLanguageLabel")
            .Add("LibraryDiscountSettings")
            .Add("LibraryOrganizationPriceLevel")
            .Add("LibraryOrganizationCustomer")
            .Add("LibraryOrganizationEmployee")
            .Add("LibraryOrganizationEmployeeFOWST")
            .Add("LibraryOrganizationPIM")
            .Add("LibraryFowstBin")
            .Add("LibraryFowstBinProduct")
            .Add("LibraryOrganizationCustomerFowstBinProduct")

            With .Add("LibrarySecurityModuleMain")
                With .Children
                    .Add("LibrarySecurityModule", "MainModuleID")
                End With
            End With

            With .Add("LibrarySecurityRole")
                With .Children
                    .Add("LibrarySecurityRoleModules", "RoleID")
                    '.Add("LibrarySecurityRoleUser", "RoleID")
                End With
            End With

            'this should be independent since its using peopleid and role id for updating
            .Add("LibrarySecurityRoleUser")

            With .Add("LibraryReceiptSettings")
                With .Children
                    .Add("LibraryReceiptSettingsFowst", "ReceiptSettingsID")
                    .Add("LibraryReceiptSettingsMessage", "ReceiptSettingsID")
                End With
            End With

            With .Add("LibraryDevicePrinter")
                With .Children
                    .Add("LibraryDevicePrinterCategory", "PrinterID")
                    .Add("LibraryDevicePrinterFowst", "PrinterID")
                End With
            End With

            With .Add("LibraryDevice")
                With .Children
                    .Add("LibraryDeviceFowst", "DeviceID")
                    .Add("LibraryDeviceFowstPrinter", "DeviceID")
                End With
            End With

            'additional by janine mar 6 3pm. for hot buttons
            With .Add("LibraryPimMenu")
                With .Children
                    .Add("LibraryPimGroups", "MenuID")
                    .Add("LibraryPimGroupPims", "MenuID")
                    .Add("LibraryPimGroupFowsts", "MenuID")
                End With
            End With

            .Add("TablesLastUpdated")
        End With
    End Sub
End Class

'Public Class MobizExportLazyUpdate
'    Inherits DataImport

'    Public Sub New()

'        With MyBase.ParentTables
'            With .Add("MobizLibraryPeople")
'                With .Children
'                    .Add("MobizLibraryPeopleUserAccount", "PeopleID")
'                    .Add("MobizLibraryPeopleOrganization", "PeopleID")
'                End With
'            End With

'            With .Add("MobizLibraryOrganization")
'                With .Children
'                    .Add("MobizLibraryOrganizationContactEmail", "OrganizationID")
'                    .Add("MobizLibraryOrganizationTradeName", "OrganizationID")
'                    .Add("MobizLibraryOrganizationFOWST", "OrganizationID")
'                    .Add("MobizLibraryOrganizationRepresentatives", "OrganizationID")
'                End With
'            End With

'            .Add("LibraryDiscountSettings")
'            .Add("LibraryOrganizationPriceLevel")
'            .Add("LibraryOrganizationCustomer")
'            .Add("LibraryOrganizationEmployee")
'            .Add("LibraryOrganizationEmployeeFOWST")
'            .Add("LibraryOrganizationPIM")


'            With .Add("LibrarySecurityModuleMain")
'                With .Children
'                    .Add("LibrarySecurityModule", "MainModuleID")
'                End With
'            End With

'            With .Add("LibrarySecurityRole")
'                With .Children
'                    .Add("LibrarySecurityRoleModules", "RoleID")
'                    '.Add("LibrarySecurityRoleUser", "RoleID")
'                End With
'            End With

'            'this should be independent since its using peopleid and role id for updating
'            .Add("LibrarySecurityRoleUser")

'            With .Add("LibraryReceiptSettings")
'                With .Children
'                    .Add("LibraryReceiptSettingsFowst", "ReceiptSettingsID")
'                    .Add("LibraryReceiptSettingsMessage", "ReceiptSettingsID")
'                End With
'            End With

'            With .Add("LibraryDevicePrinter")
'                With .Children
'                    .Add("LibraryDevicePrinterCategory", "PrinterID")
'                    .Add("LibraryDevicePrinterFowst", "PrinterID")
'                End With
'            End With

'            With .Add("LibraryDevice")
'                With .Children
'                    .Add("LibraryDeviceFowst", "DeviceID")
'                    .Add("LibraryDeviceFowstPrinter", "DeviceID")
'                End With
'            End With

'            'additional by janine mar 6 3pm. for hot buttons
'            With .Add("LibraryPimMenu")
'                With .Children
'                    .Add("LibraryPimGroups", "MenuID")
'                    .Add("LibraryPimGroupPims", "MenuID")
'                    .Add("LibraryPimGroupFowsts", "MenuID")
'                End With
'            End With

'            .Add("TablesLastUpdated")
'        End With
'    End Sub
'End Class