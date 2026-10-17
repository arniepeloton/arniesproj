Imports System.Threading
Imports DataImportExport
Imports Newtonsoft.Json

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
        Dim mForWebPos As Boolean
        Dim mForCsv As Boolean
        Dim mIncludeProducts As Boolean
        Dim mIncludeHotButtons As Boolean
        Dim mProDataPublic As ProData
        Dim mProDataPrivate As ProData
        Dim mSourceProDataPublic As ProData
        Dim mOrgID As String
        Dim mDatabaseName As String
        ''' <summary>
        ''' in milliseconds. 0 = unlimited
        ''' </summary>
        ''' <remarks></remarks>
        Dim mTableSavingDuration As Integer
        Dim mProZipCsv As ProZip
        Dim mDatabaseCreationStart As Date?
        Dim mAllowedTableNames As String = "" ' ForCsv
        Const mAllowedTableNamesProducts =
            " [MobizLibraryPIMMeasure]" & vbCrLf &
            " [MobizLibraryPIMMeasureConversion]" & vbCrLf &
            " [MobizLibraryPIMMeasureDefinition]" & vbCrLf &
            " [MobizLibraryPIMMeasureSign]" & vbCrLf &
            " [MobizLibraryPIMPriceLevel]" & vbCrLf &
            " [MobizLibraryPIMProduct]" & vbCrLf &
            " [MobizLibraryPIMProductContents]" & vbCrLf &
            " [MobizLibraryPIMProductFast]" & vbCrLf &
            " [MobizLibraryPIMProductMeasurement]" & vbCrLf &
            " [MobizLibraryPIMProductMeasurementBarcode]" & vbCrLf &
            " [MobizLibraryPIMProductParent]" & vbCrLf &
            " [MobizLibraryPIMProductPriceLevel]" & vbCrLf &
            " [MobizLibraryPIMProductVariableFeatures001]"
        Const mAllowedTableNamesHotButtons = " [LibraryItemModifierAssignedCustomer]" & vbCrLf &
            " [LibraryItemModifierAssignedFowst]" & vbCrLf &
            " [LibraryItemModifierCategories]" & vbCrLf &
            " [LibraryItemModifierInventoryAffecting]" & vbCrLf &
            " [LibraryItemModifierNonInventoryAffecting]" & vbCrLf &
            " [LibraryItemModifierRebates]" & vbCrLf &
            " [LibraryItemModifiers]" & vbCrLf &
            " [LibraryItemModifierSorting]" & vbCrLf &
            " [LibraryPimGroupFowsts]" & vbCrLf &
            " [LibraryPimGroupPims]" & vbCrLf &
            " [LibraryPimGroups]" & vbCrLf &
            " [LibraryPimMenu]" & vbCrLf &
            " [MobizLibraryFranchiseGroupPriceLevels]" & vbCrLf

#If DEBUG Then
        Dim mTableNames As String = ""
#End If

        Public Enum TransType
            FromDevice
            ToDevice
        End Enum

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost>
        Public Function SplitFile() As String

            Dim pFileTitle As String
            Dim pSourceFilename As String
            Dim pFolderTitle As String
            Dim pFolder As String
            Dim pCrc As String
            Dim pFileSize As String
            Dim pDestinationFilename As String
            Dim pCount As Integer
            Const pChunkSize As Integer = 10 ^ 6
            Dim pBuffer As Byte()
            Dim pActualChuckSize As Integer
            Dim pInfoFilename As String
            Dim pInfoText As String
            Dim pOut2 As SplitFileJson

            pFileTitle = Common.WebRequest.Data("fID")
            pSourceFilename = Common.ServerPath & "temp\DataExport\Upload\" & pFileTitle & ".zip"
            If IO.File.Exists(pSourceFilename) = False Then
                Return JsonDictionary.Serialize("Error", "File does not exist vb728HYYMX7C!")
            End If

            pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pSourceFilename)
            pFileSize = New IO.FileInfo(pSourceFilename).Length
            pFolderTitle = pFileSize & "-" & pCrc
            pFolder = Common.ServerPath & "temp\SplitFiles\" & pFolderTitle & "\"
            pInfoFilename = IO.Path.Combine(pFolder, "info.txt")
            Try
                If IO.File.Exists(pInfoFilename) Then
                    pInfoText = FileText(pInfoFilename)
                    pOut2 = JsonConvert.DeserializeObject(Of SplitFileJson)(pInfoText)
                    Return JsonConvert.SerializeObject(pOut2)
                End If
            Catch ex As Exception

            End Try
            pCount = Math.Ceiling(pFileSize / pChunkSize)
            pDestinationFilename = pFolder & SplitFileName(1, pCount)
            pOut2 = New SplitFileJson
            pOut2.FolderTitle = pFolderTitle
            pOut2.Count = pCount
            pOut2.Crc = pCrc

            CreateFolder(pDestinationFilename)
            Using pFS As New IO.FileStream(pSourceFilename, IO.FileMode.Open, IO.FileAccess.Read)
                For pIndex As Integer = 0 To pCount - 1
                    pActualChuckSize = Math.Min(pFileSize - pChunkSize * pIndex, pChunkSize)
                    ReDim pBuffer(pActualChuckSize - 1)
                    pActualChuckSize = pFS.Read(pBuffer, 0, pActualChuckSize)
                    pDestinationFilename = pFolder & SplitFileName(pIndex + 1, pCount)
                    If IO.File.Exists(pDestinationFilename) Then
                        IO.File.Delete(pDestinationFilename)
                    End If
                    Using pFsDest As New IO.FileStream(pDestinationFilename, IO.FileMode.CreateNew, IO.FileAccess.Write)
                        pFsDest.Write(pBuffer, 0, pActualChuckSize)
                    End Using
                    pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pDestinationFilename)
                    pOut2.Crcs.Add(pCrc)
                Next pIndex
            End Using

            pInfoText = JsonConvert.SerializeObject(pOut2)
            FileText(pInfoFilename) = pInfoText

            Return pInfoText

        End Function

        Public Class SplitFileJson
            Public FolderTitle As String
            Public Count As Integer
            Public Crc As String
            Public Crcs As List(Of String)

            Public Sub New()
                Crcs = New List(Of String)
            End Sub
        End Class

        Private Function SplitFileName(vIndex As Integer, vCount As Integer) As String

            Dim pOut As String

            pOut = "File " & Format(vIndex, "00000") & " of " & Format(vCount, "00000")

            Return pOut

        End Function

        Function DownloadSplit() As ActionResult

            Dim pFolderTitle As String
            Dim pIndex As String
            Dim pCount As String
            Dim pFilename As String
            Dim pFileTitle As String

            pFolderTitle = Common.WebRequest.QueryString("FolderTitle")
            pIndex = Common.WebRequest.QueryString("Index")
            pCount = Common.WebRequest.QueryString("Count")
            pFileTitle = "File " & FormatAuto(pIndex, "00000", eMajorDataType.Numeric) & " of " & FormatAuto(pCount, "00000", eMajorDataType.Numeric)
            pFilename = Common.ServerPath & "temp\SplitFiles\" & pFolderTitle & "\" & pFileTitle

            Dim pContentType As String = "application/zip"
            Dim pCrc As String

            Try
                pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pFilename)
                Response.AddHeader("CRC", pCrc)
                Return File(pFilename, pContentType, pFileTitle)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        Function Download() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("fID")
            Dim pSourceURL As String = Common.ServerPath & "temp\DataExport\Upload\" & pFileTitle & ".zip"
            Dim pContentType As String = "application/zip"
            Dim pCrc As String

            Try
                pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pSourceURL)
                Response.AddHeader("CRC", pCrc)
                Return File(pSourceURL, pContentType, pFileTitle)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        Function AllowedToExportDatabase() As Boolean

            Dim pText = FileText(Common.ServerPath & "settings/AllowedToExportDatabase.txt")
            If String.IsNullOrWhiteSpace(pText) Then
                FileText(Common.ServerPath & "settings/AllowedToExportDatabase.txt") = 1
                Return True
            End If

            Return CBool2(pText)

        End Function

        Function RequestDataFromServer() As String

            Dim pCommonDeviceFowstPersonCount As Integer

            Try
                If AllowedToExportDatabase() = False Then
                    Return JsonDictionary.Serialize("Error", "This server is not allowed to export database. Please update your Android app.")
                End If

                Dim pPeopleID = Common.WebRequest.Data("PeopleID")
                Dim pDeviceUUID = Common.WebRequest.Data("DeviceUUID")
                If WebRequest.Instance.HandleOrganization(True) Then 'change db
                    mOrgID = ReadyGuid(Common.WebRequest.Data("OrgID"))
                    If Not WebSession.Instance.HasCurrentRequest Then
                        If Not Common.WebRequest.Data("PeopleID") Is Nothing Then
                            WebSession.Instance.PeopleUserID = Common.WebRequest.Data("PeopleID")
                            WebSession.Instance.HasCurrentRequest = True
                        End If

                        mKey = GenerateString(75)

                        vSubFolder = GenerateString(75)

                        If IsDeviceAllowedToDownload() Then
                            If CBool2(Common.WebRequest.Data("DontCheckCommonFowst")) Then
                                pCommonDeviceFowstPersonCount = 1
                            Else
                                pCommonDeviceFowstPersonCount = WebRequest.Instance.Business.LibraryDevice.CommonPersonFowstCount(pPeopleID, pDeviceUUID)
                            End If
                            If pCommonDeviceFowstPersonCount = 0 Then
                                WebSession.Instance.HasCurrentRequest = False
                                Return JsonDictionary.Serialize("Error", "No common device fowst person. PeopleID = " & pPeopleID & ". DeviceUUID = " & pDeviceUUID)
                            Else
                                If HandleDbDownloadFakeOrgID() Then
                                    Return JsonDictionary.Serialize("Results", mProgress)
                                End If
                                If Common.WebRequest.Data("IsLazyUpdate") Is Nothing OrElse Not CBool(Common.WebRequest.Data("IsLazyUpdate")) Then
                                    If mInitSQLiteDBaseThread IsNot Nothing Then
                                        If mInitSQLiteDBaseThread.IsAlive Then
                                            Return JsonDictionary.Serialize("Result", "Requesting Data from Server")
                                        End If
                                    End If
                                    Dim pForWebPos = CBool2(Common.WebRequest.Data("ForWebPos"))
                                    Dim pForCsv = CBool2(Common.WebRequest.Data("ForCsv"))
                                    If pForCsv = False _
                                        AndAlso CBool2(Common.WebRequest.Data("DontGetFromCache")) = False _
                                        AndAlso IsDownloadDirectly(pForWebPos, pForCsv) Then

                                        HandleCompareCrc()
                                    ElseIf pForCsv Then
                                        mForCsv = True
                                        mIncludeProducts = CBool2(Common.WebRequest.Data("IncludeProducts"))
                                        mIncludeHotButtons = CBool2(Common.WebRequest.Data("IncludeHotButtons"))
                                        mDatabaseCreationStart = CDateNull(Common.WebRequest.Data("DatabaseCreationStart"))
                                        mAllowedTableNames = ""
                                        If mIncludeProducts Then
                                            mAllowedTableNames &= mAllowedTableNamesProducts
                                        End If
                                        If mIncludeHotButtons Then
                                            mAllowedTableNames &= mAllowedTableNamesHotButtons
                                        End If
                                        If CreateCsvDatabase() Then
                                            InitSQLiteDBase(CBool2(Common.WebRequest.Data("ForEcommerceServer")))
                                        End If
                                    ElseIf pForWebPos Then
                                        mForWebPos = True
                                        If CreateSqlServerDatabase() Then
                                            InitSQLiteDBase(CBool2(Common.WebRequest.Data("ForEcommerceServer")))
                                        End If
                                    Else
                                        If CreateSqliteDatabase() Then
                                            InitSQLiteDBase(CBool2(Common.WebRequest.Data("ForEcommerceServer")))
                                        End If
                                    End If
                                Else
                                    CompareTableTime()
                                End If
                            End If
                        End If

                        WebSession.Instance.HasCurrentRequest = False
                        Return JsonDictionary.Serialize("Results", mProgress)
                    End If
                End If
            Catch ex As Exception
                AppendToDebug("vb9QPRGUX6BT! " & ex.Message, True, ex, True)
                Return JsonDictionary.Serialize("Error", "vb9QPRGUX6BT! " & ex.Message)
            End Try

        End Function

        Private Function HandleDbDownloadFakeOrgID() As Boolean

            Try
                Dim pFilename = Common.ServerPath & "temp\DbDownloadFakeOrgID.txt"
                If IO.File.Exists(pFilename) = False Then
                    Return False
                End If
                Dim pFileText = FileText(pFilename)
                Dim p = JsonConvert.DeserializeObject(Of UtilitiesController.DbDownloadFakeOrgID)(pFileText)
                If ReadyGuid(p.OriginalOrgID) <> ReadyGuid(mOrgID) Then
                    Return False
                End If
                Dim pFileDate = ParseDate(p.DateCreated)
                If pFileDate Is Nothing Then
                    Return False
                End If
                Dim pAgeInSeconds = (Now - pFileDate.Value).TotalSeconds
                If Math.Abs(pAgeInSeconds) > 60 Then
                    Return False
                End If

                Dim pFileID = GenerateString(75)
                pFilename = Common.ServerPath & "temp\DataExport\Upload\" & pFileID & ".zip"
                Dim pProZip = New ProZip()
                Dim pFakeOrgID = Guid.NewGuid.ToString().ToLower()
                pProZip.AddString("vbHSZ34JEQ4D!", pFakeOrgID & ".private.db")
                pProZip.AddString("vbHSZ34JEQ4D!", pFakeOrgID & ".public.db")
                pProZip.AddString("vbHSZ34JEQ4D!", pFakeOrgID & ".cache.db")
                pProZip.AddString("vbHSZ34JEQ4D!", pFakeOrgID & ".txt")
                pProZip.Save(pFilename)
                mProgress = pFileID
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        Private Sub HandleCompareCrc()

            Dim pPendingDeviceCrc As String
            Dim pFilename As String
            Dim pCrc As String

            Try
                If String.IsNullOrWhiteSpace(mProgress) Then
                    Exit Sub
                End If
                pPendingDeviceCrc = Common.WebRequest.Data("PendingDeviceCrc")
                If String.IsNullOrWhiteSpace(pPendingDeviceCrc) Then
                    Exit Sub
                End If

                pFilename = Common.ServerPath & "temp\DataExport\Upload\" & mProgress & ".zip"
                If IO.File.Exists(pFilename) = False Then
                    Exit Sub
                End If

                pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pFilename)
                If CStr2(pCrc).Trim().ToLower() = pCrc.Trim().ToLower() Then
                    mProgress = "same crc"
                End If
            Catch ex As Exception
                AppendToDebug("HandleCompareCrc vbPPFQW3HNJF!", True, ex, True)
            End Try

        End Sub

        Private Sub CompareTableTime()
            Dim StrData As String()
            Dim m_Tables As New DataTable
            Dim m_TableCount As Integer

            UpdateTablesLastUpdatedTable()

            m_Tables.Columns.Add("Table")
            m_Tables.Columns.Add("DeviceTime")
            m_Tables.Columns.Add("ServerTime")

            Dim drTablesLastUpdated As DataRow

            drTablesLastUpdated = m_Tables.NewRow
            drTablesLastUpdated(0) = "TablesLastUpdated"
            drTablesLastUpdated(1) = "01/01/01"
            drTablesLastUpdated(2) = CStr(Date.Now)
            m_Tables.Rows.Add(drTablesLastUpdated)

            For Each pObj As Object In Common.WebRequest.AllData("Tables")

                StrData = Split(pObj.ToString, ",")

                Dim pSql As String
                Dim pDt As DataTable

                pSql = "Select LastUpdated from TablesLastUpdated where TableName = " & ReadyString(StrData(0))

                pDt = Common.WebRequest.ProData.Execute(pSql)

                Try
                    If pDt.Rows.Count > 0 Then
                        If Not DateDiff(DateInterval.Second, CDate(pDt.Rows(0).Item("LastUpdated")), CDate(StrData(1))) = 0 Then
                            Dim dr As DataRow

                            If StrData(0) = "LibrarySecurityRoleUser" Then
                                dr = m_Tables.NewRow
                                dr(0) = StrData(0)
                                dr(1) = "01/01/01"
                                dr(2) = pDt.Rows(0).Item("LastUpdated")
                            ElseIf StrData(0) = "LibrarySecurityRoleModules" Then
                                dr = m_Tables.NewRow
                                dr(0) = StrData(0)
                                dr(1) = "01/01/01"
                                dr(2) = pDt.Rows(0).Item("LastUpdated")
                            Else
                                dr = m_Tables.NewRow
                                dr(0) = StrData(0)
                                dr(1) = Replace(StrData(1), "Z", "")
                                dr(2) = pDt.Rows(0).Item("LastUpdated")
                            End If

                            m_Tables.Rows.Add(dr)
                        End If
                    End If
                Catch ex As Exception

                    If pDt.Rows(0).IsNull("LastUpdated") And StrData(1).Length > 1 Then
                        Dim dr As DataRow
                        If StrData(0) = "LibrarySecurityRoleUser" Then
                            dr = m_Tables.NewRow
                            dr(0) = StrData(0)
                            dr(1) = "01/01/01"
                            dr(2) = pDt.Rows(0).Item("LastUpdated")
                        ElseIf StrData(0) = "LibrarySecurityRoleModules" Then
                            dr = m_Tables.NewRow
                            dr(0) = StrData(0)
                            dr(1) = "01/01/01"
                            dr(2) = pDt.Rows(0).Item("LastUpdated")
                        Else
                            dr = m_Tables.NewRow
                            dr(0) = StrData(0)
                            dr(1) = Replace(StrData(1), "Z", "")
                            dr(2) = "01/01/01"
                        End If

                        m_Tables.Rows.Add(dr)
                    End If

                    If Not pDt.Rows(0).IsNull("LastUpdated") And StrData(1).Length = 1 Or StrData(1).Length = 0 Then
                        Dim dr As DataRow
                        If StrData(0) = "LibrarySecurityRoleUser" Then
                            dr = m_Tables.NewRow
                            dr(0) = StrData(0)
                            dr(1) = "01/01/01"
                            dr(2) = pDt.Rows(0).Item("LastUpdated")
                        ElseIf StrData(0) = "LibrarySecurityRoleModules" Then
                            dr = m_Tables.NewRow
                            dr(0) = StrData(0)
                            dr(1) = "01/01/01"
                            dr(2) = pDt.Rows(0).Item("LastUpdated")
                        Else
                            dr = m_Tables.NewRow
                            dr(0) = StrData(0)
                            dr(1) = "01/01/01"
                            dr(2) = pDt.Rows(0).Item("LastUpdated")
                        End If

                        m_Tables.Rows.Add(dr)
                    End If

                End Try
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
            Dim pSql As String
            Dim strOrgID As String

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

            If sSL.IsHeld Then Return False ' app is already running

            mDatabase = mProData.GetCurrentDatabase()
            mPeopleID = WebSession.Instance.PeopleUserID
            strOrgID = WebRequest.Instance.OrgID
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
                        mBusiness.LibraryOrganization.RestoreOrganizationTin()
                        For i As Integer = 0 To Tables.Rows.Count - 1

                            mProgress = "Requesting Data from Server"

                            'TableNameMobiz = If(CStr(Tables.Rows(i).Item("Table")).Contains("Mobiz"), GetMobizTable(CStr(Tables.Rows(i).Item("Table"))), CStr(Tables.Rows(i).Item("Table")))

                            'TableNamePrivate = CStr(Tables.Rows(i).Item("Table"))

                            'If Not CStr(Tables.Rows(i).Item("Table")).Contains("Mobiz") Then
                            Dim TableNameParent As String = CStr(Tables.Rows(i).Item("Table"))
                            Dim pSqlIDs As String = ""

                            If TableNameParent = "MobizLibraryPIMProduct" Then
                                pSql = " SELECT DISTINCT MobizLibraryPIMProduct.* " & vbCrLf &
                                    " FROM MobizLibraryPIMProduct" & vbCrLf &
                                    " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                    " WHERE LibraryOrganizationPIM.IsDeactivated = 0" & vbCrLf &
                                    " AND MobizLibraryPIMProduct.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime")))) & vbCrLf &
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
                                    " AND MobizLibraryPIMProduct.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime")))) & vbCrLf &
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

                            ElseIf TableNameParent = "MobizLibraryPeople" Then
                                ' 2016_01_20__11_46_05_4744 arvin: remove DISTINCT to avoid “The text data type cannot be selected as DISTINCT because it is not comparable” error.
                                pSql = "SELECT MobizLibraryPeople.* from MobizLibraryPeople " & vbCrLf &
                                        " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) " & vbCrLf &
                                        " Where MobizLibraryPeople.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime"))))

                                pSqlIDs = "SELECT DISTINCT MobizLibraryPeople.ID from MobizLibraryPeople " & vbCrLf &
                                        " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) " & vbCrLf &
                                        " Where MobizLibraryPeople.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime"))))

                            ElseIf TableNameParent = "MobizLibraryPIMProductMeasurementBarcode" Then
                                pSql = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.* from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                    "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                    "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID" & vbCrLf &
                                    " Where MobizLibraryPIMProductMeasurementBarcode.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime"))))

                                pSqlIDs = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.ID from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                    "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                    "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID" & vbCrLf &
                                    " Where MobizLibraryPIMProductMeasurementBarcode.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime"))))

                            ElseIf TableNameParent = "MobizLibraryPIMProductPriceLevel" Then
                                pSql = "Select DISTINCT MobizLibraryPIMProductPriceLevel.* from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                         " WHERE MobizLibraryPIMProductPriceLevel.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime")))) & vbCrLf &
                                         " AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(strOrgID) & " OR MobizLibraryPIMProductPriceLevel.OrganizationID IN " & vbCrLf &
                                        " (SELECT DISTINCT FranchisorOrgID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                        " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed = 1 AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsActive= 1 AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID) & ")"

                                pSqlIDs = "Select DISTINCT MobizLibraryPIMProductPriceLevel.ID from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                        " WHERE MobizLibraryPIMProductPriceLevel.LastUpdated BETWEEN " & ReadyString(CDate(Tables.Rows(i).Item("DeviceTime"))) & " AND " & ReadyString(DateAdd(DateInterval.Second, 1, CDate(Tables.Rows(i).Item("ServerTime")))) & vbCrLf &
                                        " AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(strOrgID) & " OR MobizLibraryPIMProductPriceLevel.OrganizationID IN " & vbCrLf &
                                        " (SELECT DISTINCT FranchisorOrgID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                        " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsActive= 1 AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID) & ")"
                            Else
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
                            End If

                            pDT = mBusiness.ProData.Execute(pSql)

                            AddFileToZip(pDT, CStr(Tables.Rows(i).Item("Table")))


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
                                    If TableNameParent = "MobizLibraryOrganization" Then
                                        AppendToLog("TableNameParent = " & TableNameParent & vbCrLf &
                                                    "TableNameChild = " & TableNameChild & " ref: vbGZFXWQX4A6!")
                                    End If

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
            Dim pDeviceUUID = Common.WebRequest.Data("DeviceUUID")

            pSql = "Select * from LibraryDevice where UUID = " & ReadyString(pDeviceUUID) & " AND IsActive=1"

            pDt = Common.WebRequest.ProData.Execute(pSql)

            If pDt.Rows.Count > 0 Then
                Return True
            Else
                AppendToLog("IsDeviceAllowedToDownload: " & vbCrLf &
                    "DeviceUUID: " & pDeviceUUID & vbCrLf &
                    "OrgID: " & WebRequest.Instance.OrgID & vbCrLf &
                    "OrgName: " & WebRequest.Instance.OrgName)
                mProgress = "Error"
            End If
        End Function

        Private Function IsDownloadDirectly(vForWebPos As Boolean, vForCsv As Boolean) As Boolean
            Dim pDirectory As String
            Dim pSql As String
            Dim dtCurrentDB As DataTable
            Dim dtLastUpdate As DataTable
            Dim pTableName As String

            If vForWebPos Then
                pTableName = "Current Sql Server Database"
            ElseIf vForCsv Then
                pTableName = "Current CSV Database"
            Else
                pTableName = "Current Database"
            End If
            'Edited By: Francis H
            'Date: 06/19/14

            pSql = "Select * From TablesLastUpdated where TableName = " & ReadyString(pTableName)

            dtCurrentDB = mProData.Execute(pSql)

            If dtCurrentDB.Rows.Count = 0 Then
                pSql = "Insert Into TablesLastUpdated(TableName, FileName, LastUpdated) values(" & ReadyString(pTableName) & ", 'Creating Database', Getdate())"
                mProData.ExecuteSQL(pSql)


                pSql = "Insert Into TablesLastUpdated(TableName, FileName, LastUpdated) values('Previous Database', '', Getdate())"
                mProData.ExecuteSQL(pSql)

                mProgress = "Creating Database"
            ElseIf CStr2(dtCurrentDB.Rows(0).Item("FileName")) = "Creating Database" Then
                'kulang pa ni
                mProgress = "Creating Database"
            ElseIf Not System.IO.File.Exists(Common.ServerPath & "temp\DataExport\Upload\" & CStr2(dtCurrentDB.Rows(0).Item("FileName")) & ".zip") Then
                mProgress = "Creating Database"
            ElseIf Math.Abs(DateDiff(DateInterval.Minute, dtCurrentDB.Rows(0).Item("LastUpdated"), Now)) <= WebRequest.Instance.Business.SystemDefaults.MaxMobizAppDatabaseAge Then
                mProgress = CStr2(dtCurrentDB.Rows(0).Item("FileName"))
                Return True
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
                                    " Where TableName = " & ReadyString(pTableName)
                    mProData.ExecuteSQL(pSql)

                    mProgress = "Creating Database"
                Else
                    mProgress = CStr2(dtCurrentDB.Rows(0).Item("FileName"))
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
                            mBusiness.LibraryOrganization.RestoreOrganizationTin()

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
                                        ' 2016_01_20__11_46_05_4744 arvin: remove DISTINCT to avoid “The text data type cannot be selected as DISTINCT because it is not comparable” error.
                                        pSql = "SELECT MobizLibraryPeople.* from MobizLibraryPeople " & vbCrLf &
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
                                        pSql = "Select DISTINCT MobizLibraryPIMProductPriceLevel.* from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                            " WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & " OR MobizLibraryPIMProductPriceLevel.OrganizationID IN " & vbCrLf &
                                            " (SELECT DISTINCT FranchisorOrgID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                            " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsActive= 1 AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & ")"

                                        pSqlIDs = "Select DISTINCT MobizLibraryPIMProductPriceLevel.ID from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                            " WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & " OR MobizLibraryPIMProductPriceLevel.OrganizationID IN " & vbCrLf &
                                            " (SELECT DISTINCT FranchisorOrgID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                            " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsActive= 1 AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & ")"

                                    ElseIf TableNameParent = "MobizLibraryOrganizationFranchisee" Then
                                        pSql = "SELECT DISTINCT MobizLibraryOrganizationFranchisee.* FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                            " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsActive= 1 AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))

                                        pSqlIDs = "SELECT DISTINCT MobizLibraryOrganizationFranchisee.ID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                            " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND MobizLibraryOrganizationFranchisee.IsActive= 1 AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))


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

                                Debug.Print("/* SQL */" & vbCrLf & pSql)
                                pDT = mBusiness.ProData.Execute(pSql)

                                AddFileToZip(pDT, TableNameParent)

                                If mTables.ParentTables(TableNameParent).Children.Count > 0 Then
                                    For j As Integer = 0 To mTables.ParentTables(TableNameParent).Children.Count - 1

                                        Dim TableNameChild = mTables.ParentTables(TableNameParent).Children.Values(j).ChildTable
                                        If TableNameParent = "MobizLibraryOrganization" Then
                                            AppendToLog("TableNameParent = " & TableNameParent & vbCrLf &
                                                        "TableNameChild = " & TableNameChild & " ref: vbCKULD5X5HF!")
                                        End If
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

        Private Shared mInitSQLiteDBaseThread As Thread

        Private Function InitSQLiteDBase(vForEcommerceServer As Boolean) As String
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

                If ReadyGuid(WebRequest.Instance.OrgID) = ReadyGuid("d362ffdb-15d1-451f-97dc-c567b94fb410") Or ReadyGuid(WebRequest.Instance.OrgID) = ReadyGuid("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba") Then
                    'PC CO/FO OrgID
                    WebRequest.Instance.Business.SystemDefaults.MaxMobizAppDatabaseAge = 120
                End If

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
                mInitSQLiteDBaseThread = TimedThread(
                    Sub()
                        Dim pSql As String
                        Dim mBusiness As Business
                        Dim pDbInfo As DbInfo
                        Dim pDbInfoFilename As String

                        Try
                            If sSL.IsHeld Then Exit Sub
                            sSL.Enter(False)

                            '#If DEBUG Then
                            '                            Debug.WriteLine("artificial delay for InitSQLiteDBase vbFHCBF62H77!")
                            '                            Thread.Sleep(30000)
                            '#End If
                            mFinishedProduct = Nothing

                            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                            mBusiness.ProData.CommandTimeout = 60 * 5
                            mBusiness.ProData.Execute("USE " & EscapeSqlName(mDatabase))
                            mBusiness.SystemDefaults.SaveCompanyOwnedOrFranchisee(strOrgID)

                            For i As Integer = 0 To mTables.ParentTables.Count - 1

                                Dim pSql2 As String
                                Dim pSqlIDs As String = ""
                                Dim pSqlFromFor3rdLayer As String = ""
                                Dim month As Integer = CInt2(Format(Date.Now, "MM"))
                                Dim year As Integer = CInt2(Format(Date.Now, "yyyy"))
                                Dim carryovermonth As Integer = month
                                Dim carryoveryear As Integer = year

                                TableNameParent = mTables.ParentTables.Values(i).Name
                                mExecutingTable = TableNameParent
                                Dim pDrTimeStamp = mDtTimeStamp.NewRow()
                                pDrTimeStamp("TimeStart") = Now
                                pDrTimeStamp("Description") = "Query: " & TableNameParent

                                mProgress = "Requesting Data from Server"
                                pDT = Nothing
                                If mForCsv = False _
                                    OrElse mAllowedTableNames.ToLower().Contains("[" & TableNameParent.ToLower() & "]") Then

                                    If TableNameParent = "MobizLibraryPIMProduct" Then
                                        pSql = " SELECT DISTINCT MobizLibraryPIMProduct.* " & vbCrLf &
                                            " FROM MobizLibraryPIMProduct" & vbCrLf &
                                            " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                            " WHERE LibraryOrganizationPIM.IsDeactivated = 0" & vbCrLf &
                                            " AND LibraryOrganizationPIM.ProductAvailableToMobile = 1" & vbCrLf &
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
                                            " AND LibraryOrganizationPIM.ProductAvailableToMobile = 1" & vbCrLf &
                                            " UNION " & vbCrLf &
                                            " SELECT DISTINCT MobizLibraryPIMProduct.ID" & vbCrLf &
                                            " FROM MobizLibraryPIMProduct" & vbCrLf &
                                            " JOIN MobizLibraryPIMProductContents " & vbCrLf &
                                            " ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                                            " OR MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID"

                                    ElseIf TableNameParent = "MobizLibraryOrganization" Then
                                        If vForEcommerceServer Then
                                            pSql = "Select * from " & TableNameParent
                                            pSqlIDs = "Select ID from " & TableNameParent
                                        Else
                                            pSql = "SELECT DISTINCT MobizLibraryOrganization.* from MobizLibraryOrganization " & vbCrLf &
                                                "WHERE MobizLibraryOrganization.IsOrganization = 1" & vbCrLf &
                                                    " AND MobizLibraryOrganization.ID IN ( " &
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
                                                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.id = LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
                                                " WHERE MobizLibraryOrganization.IsOrganization = 1" & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select LibraryOrganizationSupplier.SupplierID as OrgID from LibraryOrganizationSupplier " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select TransReceiving.SupplierID as OrgID from TransReceiving " & vbCrLf &
                                                " UNION " & vbCrLf &
                                                " Select OrganizationID as OrgID from MobizLibraryPeopleOrganization where PeopleID = " & ReadyString(mPeopleID)
                                        End If

                                        '" UNION " & vbCrLf &
                                        '" SELECT prd.PublishedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) " &
                                        '" UNION " & vbCrLf &
                                        '" SELECT prd.UploadedByID as OrgID FROM LibraryOrganizationPIM orgpim INNER JOIN MobizLibraryPIMProduct prd ON (orgpim.ProductID = prd.ID) "

                                    ElseIf TableNameParent = "MobizLibraryPeople" Then
                                        ' 2016_01_20__11_46_05_4744 arvin: remove DISTINCT to avoid “The text data type cannot be selected as DISTINCT because it Is Not comparable” error.
                                        If vForEcommerceServer Then
                                            pSql = "Select * from " & TableNameParent
                                            pSqlIDs = "Select ID from " & TableNameParent
                                        Else
                                            pSql = "SELECT MobizLibraryPeople.* from MobizLibraryPeople " & vbCrLf &
                                                " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "

                                            pSqlIDs = "SELECT DISTINCT MobizLibraryPeople.ID from MobizLibraryPeople " & vbCrLf &
                                                    " INNER JOIN LibraryOrganizationEmployee ON (LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID) "
                                        End If

                                    ElseIf TableNameParent = "MobizLibraryPIMProductMeasurementBarcode" Then
                                        pSql = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.* from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                            "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                            "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"

                                        pSqlIDs = "SELECT DISTINCT MobizLibraryPIMProductMeasurementBarcode.ID from MobizLibraryPIMProductMeasurementBarcode " & vbCrLf &
                                            "INNER JOIN MobizLibraryPIMProductMeasurement ON  MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " & vbCrLf &
                                            "INNER JOIN LibraryOrganizationPIM ON MobizLibraryPIMProductMeasurement.productID = LibraryOrganizationPIM.ProductID"

                                    ElseIf TableNameParent = "MobizLibraryPIMProductPriceLevel" Then
                                        pSql = "Select DISTINCT MobizLibraryPIMProductPriceLevel.* from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                                 " WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(strOrgID) & " Or MobizLibraryPIMProductPriceLevel.OrganizationID IN " & vbCrLf &
                                                " (SELECT DISTINCT FranchisorOrgID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                                " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed = 1 And MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 And MobizLibraryOrganizationFranchisee.IsActive= 1 And MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID) & ")" & vbCrLf &
                                                " Or MobizLibraryPIMProductPriceLevel.OrganizationID IN (" & vbCrLf &
                                                " 	SELECT LibraryOrganizationSupplier.SupplierID" & vbCrLf &
                                                " 	FROM LibraryOrganizationSupplier" & vbCrLf &
                                                " )"

                                        pSqlIDs = "Select DISTINCT MobizLibraryPIMProductPriceLevel.ID from MobizLibraryPIMProductPriceLevel inner join LibraryOrganizationPIM on MobizLibraryPIMProductPriceLevel.productid = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                                " WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(strOrgID) & " Or MobizLibraryPIMProductPriceLevel.OrganizationID IN " & vbCrLf &
                                                " (SELECT DISTINCT FranchisorOrgID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                                " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 And MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 And MobizLibraryOrganizationFranchisee.IsActive= 1 And MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID) & ")" & vbCrLf &
                                                " Or MobizLibraryPIMProductPriceLevel.OrganizationID IN (" & vbCrLf &
                                                " 	SELECT LibraryOrganizationSupplier.SupplierID" & vbCrLf &
                                                " 	FROM LibraryOrganizationSupplier" & vbCrLf &
                                                " )"

                                    ElseIf TableNameParent = "MobizLibraryOrganizationFranchisee" Then
                                        pSql = "SELECT DISTINCT MobizLibraryOrganizationFranchisee.* FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                               " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed = 1 And MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 And MobizLibraryOrganizationFranchisee.IsActive= 1 And MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID)

                                        pSqlIDs = "SELECT DISTINCT MobizLibraryOrganizationFranchisee.ID FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                                                " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1 And MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 And MobizLibraryOrganizationFranchisee.IsActive= 1 And MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID)


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
                                    ElseIf TableNameParent.ToLowerInvariant() = "MobizLibraryPIMProductParent".ToLowerInvariant() Then
                                        pSql = "Select * from MobizLibraryPIMProductParent WHERE OrganizationID = " & ReadyString(strOrgID)
                                    ElseIf TableNameParent.ToLowerInvariant() = "MobizLibraryFranchiseGroupPriceLevels".ToLowerInvariant() Then
                                        pSql = " SELECT DISTINCT MobizLibraryFranchiseGroupPriceLevels.*" & vbCrLf &
                                            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                                            " JOIN MobizLibraryOrganizationFranchisee ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID" & vbCrLf &
                                            " JOIN MobizLibraryFranchiseGroupPriceLevels ON MobizLibraryFranchiseGroupPriceLevels.GroupID = MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
                                            " WHERE MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed= 1" & vbCrLf &
                                            " And MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1" & vbCrLf &
                                            " And MobizLibraryOrganizationFranchisee.IsActive= 1" & vbCrLf &
                                            " And MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(strOrgID)
                                    ElseIf TableNameParent.ToLowerInvariant() = "LibraryDevicePrinterCategory".ToLowerInvariant() Then
                                        pSql = " SELECT DISTINCT LibraryDevicePrinterCategory.*" & vbCrLf &
                                            " FROM LibraryDevicePrinterCategory" & vbCrLf &
                                            " JOIN LibraryDevicePrinterFowst ON LibraryDevicePrinterFowst.ID = LibraryDevicePrinterCategory.PrinterFowstID" & vbCrLf &
                                            " JOIN LibraryDevicePrinter ON LibraryDevicePrinter.ID = LibraryDevicePrinterFowst.PrinterID" & vbCrLf &
                                            " WHERE LibraryDevicePrinter.IsActive= 1" & vbCrLf
                                    ElseIf TableNameParent.ToLowerInvariant() = "LibraryFowstBin".ToLowerInvariant() Then
                                        '    pSql = " SELECT LibraryFowstBin.* " & vbCrLf &
                                        '            " FROM LibraryFowstBin " & vbCrLf &
                                        '            " INNER JOIN LibraryFowstBinProduct on " & vbCrLf &
                                        '            " LibraryFowstBin.ID = LibraryFowstBinProduct.BinID " & vbCrLf &
                                        '            " INNER JOIN LibraryOrganizationPIM on " & vbCrLf &
                                        '            " LibraryFowstBinProduct.ProductID = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                        '            " WHERE LibraryOrganizationPIM.IsDeactivated = 0 And LibraryFowstBin.IsActive = 1 " & vbCrLf

                                        pSql = "SELECT LibraryFowstBin.*  " & vbCrLf &
                                                "FROM LibraryFowstBin " & vbCrLf &
                                                "inner join MobizLibraryOrganizationFowst on  " & vbCrLf &
                                                "MobizLibraryOrganizationFowst.ID = LibraryFowstBin.FowstID " & vbCrLf
                                        '#If DEBUG = False Then
                                        pSql &= "WHERE /*LibraryFowstBin.FowstID in ( " & vbCrLf &
                                                "Select MobizLibraryOrganizationFowst.ID from MobizLibraryOrganizationFowst " & vbCrLf &
                                                "inner join LibraryOrganizationCustomer  " & vbCrLf &
                                                "on LibraryOrganizationCustomer.OrganizationID = MobizLibraryOrganizationFowst.organizationID " & vbCrLf &
                                                ")" & vbCrLf
                                        '#End If
                                        pSql &= " AND*/ LibraryFowstBin.Name <> 'Unnamed Bin' "

                                    ElseIf TableNameParent.ToLowerInvariant() = "MobizLibrarySubscription".ToLowerInvariant() Then
                                        pSql = "SELECT MobizLibrarySubscription.*" & vbCrLf &
                                            " FROM MobizLibrarySubscription" & vbCrLf &
                                            " WHERE MobizLibrarySubscription.OrganizationID = " & ReadyString(strOrgID)
                                    ElseIf TableNameParent.ToLowerInvariant() = "LibraryFowstBinProduct".ToLowerInvariant() Then
                                        pSql = " SELECT DISTINCT LibraryFowstBinProduct.*  " & vbCrLf &
                                                " FROM LibraryFowstBinProduct " & vbCrLf &
                                                " INNER JOIN LibraryFowstBin on  " & vbCrLf &
                                                " LibraryFowstBin.ID = LibraryFowstBinProduct.BinID " & vbCrLf &
                                                " INNER JOIN LibraryOrganizationPIM on  " & vbCrLf &
                                                " LibraryFowstBinProduct.ProductID = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                                " WHERE LibraryOrganizationPIM.IsDeactivated = 0 AND LibraryOrganizationPIM.ProductAvailableToMobile = 1 AND LibraryFowstBin.IsActive = 1 " & vbCrLf
                                    ElseIf TableNameParent.ToLowerInvariant() = "LibraryOrganizationPriceLevel".ToLowerInvariant() Then
                                        pSql = " SELECT DISTINCT LibraryOrganizationPriceLevel.*  " & vbCrLf &
                                                " FROM LibraryOrganizationPriceLevel " & vbCrLf &
                                                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.id = LibraryOrganizationPriceLevel.OrganizationID" & vbCrLf &
                                                " WHERE MobizLibraryOrganization.IsOrganization = 1 " & vbCrLf
                                    ElseIf TableNameParent.ToLowerInvariant() = "LibraryOrganizationCustomer".ToLowerInvariant() Then
                                        pSql = " SELECT DISTINCT LibraryOrganizationCustomer.*  " & vbCrLf &
                                                " FROM LibraryOrganizationCustomer " & vbCrLf &
                                                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.id = LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
                                                " WHERE MobizLibraryOrganization.IsOrganization = 1 " & vbCrLf
                                    ElseIf TableNameParent.ToLowerInvariant() = "MobizLibraryVouchers".ToLowerInvariant() And strOrgID.ToLowerInvariant = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" Then

                                        '    'dont import for franchisee

                                        pSql = "SELECT top 1 * FROM MobizLibraryVouchers WHERE IsActive = 0 "

                                        'pSql = "SELECT * FROM MobizLibraryVouchers WHERE IsActive = 1 "
                                        'AppendToDebug("MobizLibraryVouchers")
                                    ElseIf TableNameParent.ToLowerInvariant() = "TransVDMLogs".ToLowerInvariant() Then
                                        pSql = "SELECT * FROM TransVDMLogs WHERE ID IS NULL"
                                    Else
                                        If TableNameParent = "LibrarySecurityRole" Then
                                            Debug.WriteLine("LibrarySecurityRole")
                                        End If
                                        pSql = "Select DISTINCT * from " & TableNameParent
                                        pSqlIDs = "Select ID from " & TableNameParent
                                    End If

                                    Debug.Print("/* SQL */" & vbCrLf & pSql)
                                    Dim pSqlOriginal = pSql
                                    If mDatabaseCreationStart IsNot Nothing Then
                                        pSql = "SELECT * FROM (" & pSql & " ) t WHERE LastUpdated >= " & ReadyString(Format(mDatabaseCreationStart, cGlobal.FormatDateTimeSql))
                                    End If
                                    Try
                                        pDT = mBusiness.ProData.Execute(pSql)
                                    Catch ex As ProSqlException
                                        If ex.Message <> "Invalid column name 'LastUpdated'." Then
                                            Throw
                                        End If
                                        pDT = mBusiness.ProData.Execute(pSqlOriginal)
                                    End Try

                                    If TableNameParent.ToLowerInvariant() = "LibraryFowstBinProduct".ToLowerInvariant() Then
                                        If pDT.Columns.Contains("DateImported") Then
                                            For Each pDR As DataRow In pDT.Rows
                                                If IsDate(pDR("DateImported")) = False Then
                                                    pDR("DateImported") = Now
                                                End If
                                            Next pDR
                                        End If
                                    ElseIf TableNameParent.ToLowerInvariant() = "MobizLibrarySubscription".ToLowerInvariant() Then
                                        If pDT.Rows.Count > 0 Then
                                            mDatabaseName = CStr2(pDT.Rows(0)("Database"))
                                        End If
                                    End If

                                    pDrTimeStamp("TimeEnd") = Now
                                    pDrTimeStamp("RecordCount") = pDT.Rows.Count
                                    pDrTimeStamp("TotalSeconds") = (CDate(pDrTimeStamp("TimeEnd")) - CDate(pDrTimeStamp("TimeStart"))).TotalSeconds
                                    mDtTimeStamp.Rows.Add(pDrTimeStamp)
                                    TableToExcelFile(mDtTimeStamp, "", "TimeStart, TimeEnd", mTimeStampFilename)

                                    '''Insert into SQL Lite
                                    If Not CreateAndSaveToSQLiteTable(strOrgID, TableNameParent, pDT, vSubFolder) Then
                                        IsSuccessfullySaveInSqlite = False
                                    End If

                                    If mTables.ParentTables(TableNameParent).Children.Count > 0 Then
                                        For j As Integer = 0 To mTables.ParentTables(TableNameParent).Children.Count - 1

                                            Dim TableNameChild = mTables.ParentTables(TableNameParent).Children.Values(j).ChildTable
                                            pDrTimeStamp = mDtTimeStamp.NewRow()
                                            pDrTimeStamp("TimeStart") = Now
                                            pDrTimeStamp("Description") = "Query: " & TableNameChild

                                            If TableNameParent = "MobizLibraryOrganization" Then
                                                AppendToLog("TableNameParent = " & TableNameParent & vbCrLf &
                                                            "TableNameChild = " & TableNameChild & " ref: vbQ8T9XJQ9WH!")
                                            End If
                                            mExecutingTable = TableNameChild
                                            mProgress = "Requesting Data from Server"

                                            If (TableNameChild = "MobizLibraryPIMProductVariableFeatures001" Or TableNameChild = "LibraryPIMProductVariableFeatures001") Then
                                                pSql = "Select DISTINCT ID, ProductID, C0017, C0020, C0090, C0091 From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")
                                            Else
                                                pSql = "Select DISTINCT * From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")
                                            End If

                                            pSqlFromFor3rdLayer = "Select DISTINCT ID From " & TableNameChild & IIf(pSqlIDs.Trim().Length > 0, " WHERE " & mTables.ParentTables(TableNameParent).Children.Values(j).ChildField & " IN (" & pSqlIDs & ")", "")

                                            pSqlOriginal = pSql
                                            If mDatabaseCreationStart IsNot Nothing Then
                                                pSql = "SELECT * FROM (" & pSql & " ) t WHERE LastUpdated >= " & ReadyString(Format(mDatabaseCreationStart, cGlobal.FormatDateTimeSql))
                                            End If
                                            Try
                                                pDT = mBusiness.ProData.Execute(pSql)
                                            Catch ex As ProSqlException
                                                If ex.Message <> "Invalid column name 'LastUpdated'." Then
                                                    Throw
                                                End If
                                                pDT = mBusiness.ProData.Execute(pSqlOriginal)
                                            End Try
                                            pDrTimeStamp("TimeEnd") = Now
                                            pDrTimeStamp("RecordCount") = pDT.Rows.Count
                                            pDrTimeStamp("TotalSeconds") = (CDate(pDrTimeStamp("TimeEnd")) - CDate(pDrTimeStamp("TimeStart"))).TotalSeconds
                                            mDtTimeStamp.Rows.Add(pDrTimeStamp)
                                            TableToExcelFile(mDtTimeStamp, "", "TimeStart, TimeEnd", mTimeStampFilename)

                                            '''Insert into SQL Lite
                                            If Not CreateAndSaveToSQLiteTable(strOrgID, TableNameChild, pDT, vSubFolder) Then
                                                IsSuccessfullySaveInSqlite = False
                                            End If

                                            If pDT.Rows.Count > 0 Then
                                                Dim m_TableNameChild As String()

                                                If TableNameChild = "MobizLibraryOrganizationFOWST" Then
                                                    m_TableNameChild = {"FowstID", "MobizLibraryOrganizationFOWSTTel", "MobizLibraryOrganizationFOWSTEmail", "MobizLibraryOrganizationFOWSTOperationHRs", "MobizLibraryOrganizationFOWSTMallInfoSM"}
                                                ElseIf TableNameChild = "MobizLibraryOrganizationRepresentatives" Then
                                                    m_TableNameChild = {"OrgRepID", "MobizLibraryOrganizationRepresentativesEmail", "MobizLibraryOrganizationRepresentativesMobile", "MobizLibraryOrganizationRepresentativesTel"}
                                                End If

                                                If Not m_TableNameChild Is Nothing Then
                                                    For k As Integer = 1 To m_TableNameChild.Length - 1

                                                        mExecutingTable = m_TableNameChild(k)
                                                        pDrTimeStamp = mDtTimeStamp.NewRow()
                                                        pDrTimeStamp("TimeStart") = Now
                                                        pDrTimeStamp("Description") = "Query: " & mExecutingTable
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
                                                        pDrTimeStamp("TimeEnd") = Now
                                                        pDrTimeStamp("RecordCount") = pDTChild.Rows.Count
                                                        pDrTimeStamp("TotalSeconds") = (CDate(pDrTimeStamp("TimeEnd")) - CDate(pDrTimeStamp("TimeStart"))).TotalSeconds
                                                        mDtTimeStamp.Rows.Add(pDrTimeStamp)
                                                        TableToExcelFile(mDtTimeStamp, "", "TimeStart, TimeEnd", mTimeStampFilename)

                                                        '''Insert into SQL Lite
                                                        If Not CreateAndSaveToSQLiteTable(strOrgID, m_TableNameChild(k), pDTChild, vSubFolder) Then
                                                            IsSuccessfullySaveInSqlite = False
                                                        End If
                                                    Next
                                                End If
                                                m_TableNameChild = Nothing
                                            End If
                                        Next
                                    End If ' END If mTables.ParentTables(TableNameParent).Children.Count > 0 Then
                                End If ' END If mAllowedTableNames.ToLower().Contains("[" & TableNameParent.ToLower() & "]") = False Then

                                If Not IsSuccessfullySaveInSqlite Then
                                    'rollback process
                                    mProgress = "Error in creating database. Please contact Mobiz asap."
                                    Exit Sub
                                End If
                            Next

                            pDTChild = mBusiness.LibraryStock.FastQuerySource(strOrgID)
                            If Not CreateAndSaveToSQLiteTable(strOrgID, "MobizLibraryPIMProductFast", pDTChild, vSubFolder,
                                    Function(vDB As ProSqlite)
                                        pSql = "CREATE INDEX indexLibraryPIMProductFast ON LibraryPIMProductFast (ProductSearchString COLLATE NOCASE)"
                                        vDB.ExecuteSQL(pSql)

                                        pSql = "CREATE INDEX indexLibraryPIMProductFast_2 ON LibraryPIMProductFast (ReadyBarcode COLLATE NOCASE)"
                                        vDB.ExecuteSQL(pSql)
                                        Return True
                                    End Function
                                ) Then

                                mProgress = "Error in creating database. Please contact Mobiz asap."
                                Exit Sub
                            End If

                            If mForCsv = False Then
                                pDbInfoFilename = Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".txt"
                                pDbInfo = LoadDbInfo(pDbInfoFilename)
                                pDbInfo.DateEnded = Now
                                SaveDbInfo(pDbInfo, pDbInfoFilename)

                                If System.IO.File.Exists(Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip") Then
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip")
                                End If

                                mZip = New ProZip(Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip")
                            End If

                            If mForWebPos Then
                                Dim pDrTimeStamp = mDtTimeStamp.NewRow()
                                pDrTimeStamp("TimeStart") = Now
                                pDrTimeStamp("Description") = "Detach, Compress"

                                pSql = "DBCC SHRINKDATABASE (" & EscapeSqlName("temp_" & ShortGuid(mOrgID) & "_private") & ")"
                                mProData.ExecuteSQL(pSql)
                                mProData.DetachDatabase("temp_" & ShortGuid(mOrgID) & "_private")

                                pSql = "DBCC SHRINKDATABASE (" & EscapeSqlName("temp_" & ShortGuid(mOrgID) & "_public") & ")"
                                mProData.ExecuteSQL(pSql)
                                mProData.DetachDatabase("temp_" & ShortGuid(mOrgID) & "_public")

                                ProPack.Utility.AddDirectoryUser(Common.ServerPath & "temp\DataExport\Upload\" & mOrgID & "\" & vSubFolder)

                                pFilename = Common.ServerPath & "temp\DataExport\Upload\" & mOrgID & "\" & vSubFolder & "\temp_" & ShortGuid(mOrgID) & "_private.mdf"
                                WebPosDbDownloaderController.AddFileUser(pFilename)
                                mZip.AddFileRename(pFilename, mDatabaseName & ".mdf")

                                pFilename = Common.ServerPath & "temp\DataExport\Upload\" & mOrgID & "\" & vSubFolder & "\temp_" & ShortGuid(mOrgID) & "_private.ldf"
                                WebPosDbDownloaderController.AddFileUser(pFilename)
                                mZip.AddFileRename(pFilename, mDatabaseName & ".ldf")

                                pFilename = Common.ServerPath & "temp\DataExport\Upload\" & mOrgID & "\" & vSubFolder & "\temp_" & ShortGuid(mOrgID) & "_public.mdf"
                                WebPosDbDownloaderController.AddFileUser(pFilename)
                                mZip.AddFileRename(pFilename, "Mobiz.mdf")

                                pFilename = Common.ServerPath & "temp\DataExport\Upload\" & mOrgID & "\" & vSubFolder & "\temp_" & ShortGuid(mOrgID) & "_public.ldf"
                                WebPosDbDownloaderController.AddFileUser(pFilename)
                                mZip.AddFileRename(pFilename, "Mobiz.ldf")

                                mZip.AddFile(pDbInfoFilename)

                                pSqlTemp = "Update TablesLastUpdated Set Lastupdated = GetDate(), FileName = " & ReadyString(mKey) &
                                       " Where TableName = 'Current Sql Server Database'"
                                mBusiness.ProData.Execute(pSqlTemp)

                                pDrTimeStamp("TimeEnd") = Now
                                pDrTimeStamp("TotalSeconds") = (CDate(pDrTimeStamp("TimeEnd")) - CDate(pDrTimeStamp("TimeStart"))).TotalSeconds
                                mDtTimeStamp.Rows.Add(pDrTimeStamp)
                                TableToExcelFile(mDtTimeStamp, "", "TimeStart, TimeEnd", mTimeStampFilename)
                            ElseIf mForCsv Then
#If DEBUG Then
                                Debug.WriteLine(mTableNames)
#End If
                                Dim pDbInfoText = mProZipCsv.GetString(mOrgID & ".txt")
                                Dim pDbInfoJson = JsonConvert.DeserializeObject(Of DbInfo)(pDbInfoText)
                                pDbInfoJson.DateEnded = Now
                                pDbInfoJson.DatabaseCreationStart = mDatabaseCreationStart
                                pDbInfoText = JsonConvert.SerializeObject(pDbInfoJson)
                                mProZipCsv.AddString(pDbInfoText, mOrgID & ".txt")

                                pSqlTemp = "Update TablesLastUpdated Set Lastupdated = GetDate(), FileName = " & ReadyString(mKey) &
                                       " Where TableName = 'Current CSV Database'"
                                mBusiness.ProData.Execute(pSqlTemp)
                            Else
                                mZip.AddFolder(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder, "*")

                                pSqlTemp = "Update TablesLastUpdated Set Lastupdated = GetDate(), FileName = " & ReadyString(mKey) &
                                        " Where TableName = 'Current Database'"
                                mBusiness.ProData.Execute(pSqlTemp)

                                Try
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\Mobiz.db")
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".db")
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\7c794971-1cd2-4ad4-922f-36a93edde09d.db")
                                    System.IO.Directory.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder)

                                    ' when the 1 public, 1 private is done:
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".public.db")
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".private.db")
                                    System.IO.File.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder & "\" & strOrgID & ".cache.db")
                                    System.IO.Directory.Delete(Common.ServerPath & "temp\DataExport\Upload\" & strOrgID & "\" & vSubFolder)
                                Catch ex As Exception

                                End Try
                            End If

                            mProgress = mKey
                            mFinishedProduct = DateTimeStamp()
                        Catch ex As Exception
                            mProgress = "Error in " & mExecutingTable & ". ref vb6G4467PC7C!" & vbCrLf &
                                ex.Message & vbCrLf
                            Debug.WriteLine(mProgress)
                            Try
                                mProgress &= "Last SQL: " & mBusiness.ProData.LastSqlException.Sql
                            Catch ex2 As Exception
                                Stop2() ' just ignore any errors here
                            End Try
                            Debug.WriteLine(mProgress)
                            AppendToDebug(mProgress, True, ex, True)
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

        Function FinalizeSqlServer()

        End Function

        Function CreateAndSaveToSQLiteTable(ByVal vOrgID As String, ByVal vTableName As String, ByVal dtData As DataTable, ByVal vSubFolder As String,
            Optional vCallback As Func(Of ProSqlite, Boolean) = Nothing) As Boolean

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
            Dim mFilenameCacheDB As String

            If mForWebPos Then
                Return SaveToSqlServerTable(vTableName, dtData)
            End If
            If mForCsv Then
                Return SaveToCsvTable(vTableName, dtData)
            End If
            If vTableName.Trim().ToLower() = "LibraryDevice".ToLower() Then
                Debug.WriteLine("LibraryDevice. ref: vb7DMMDV6C3S!")
            End If
            mFilenameFolder = Common.ServerPath & "temp\DataExport\Upload\" & vOrgID & "\" & vSubFolder
            mFilenameParentFolder = Common.ServerPath & "temp\DataExport\Upload\" & vOrgID

            If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameParentFolder)
            End If

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            ' when the 1 public, 1 private is done
            mFilenamePrivateDB = mFilenameFolder & "\" & vOrgID & ".private.db"
            CreateFolder(mFilenamePrivateDB)
            mFilenamePublicDB = mFilenameFolder & "\" & vOrgID & ".public.db"
            CreateFolder(mFilenamePublicDB)
            mFilenameCacheDB = mFilenameFolder & "\" & vOrgID & ".cache.db"
            CreateFolder(mFilenameCacheDB)

            sqliteDB = New ProPack.ProSqlite
            If (vTableName.Trim().StartsWith("Mobiz")) Then
                If Replace(vTableName, "Mobiz", "").Trim().StartsWith("Cache") Then
                    sqliteDB.Open(mFilenameCacheDB)
                Else
                    sqliteDB.Open(mFilenamePublicDB)
                End If
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

                If vCallback IsNot Nothing Then
                    If vCallback(sqliteDB) = False Then
                        Return False
                    End If
                End If
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

        Private Function SaveToSqlServerTable(ByVal vTableName As String, ByVal vDT As DataTable) As Boolean

            Dim pProData As ProData
            Dim pStopWatch As Stopwatch
            Dim pTableName As String

            Dim pDrTimeStamp = mDtTimeStamp.NewRow()
            pDrTimeStamp("TimeStart") = Now
            pDrTimeStamp("Description") = "SaveToSqlServerTable: " & vTableName

            pTableName = vTableName
            If (pTableName.Trim().StartsWith("Mobiz")) Then
                pTableName = pTableName.Substring("Mobiz".Length)
                If pTableName.Trim().StartsWith("Cache") Then
                    pProData = mProDataPrivate
                Else
                    pProData = mProDataPublic

                End If
            Else
                pProData = mProDataPrivate
            End If

            pTableName = EscapeSqlName(pTableName)
            pProData.SetInsertIdentity(pTableName, True, True)

            Using pBulkCopy As New SqlBulkCopy(pProData.Connection, SqlBulkCopyOptions.KeepIdentity + SqlBulkCopyOptions.KeepNulls, Nothing)
                'Using pBulkCopy As New SqlBulkCopy(pProData.Connection)
                pBulkCopy.DestinationTableName = pTableName
                pBulkCopy.BulkCopyTimeout = 0
                pBulkCopy.WriteToServer(vDT)
            End Using

            'Dim pInsertUpdate As New InsertUpdate
            'pInsertUpdate.TableName = pTableName
            '            For p = 0 To vDT.DefaultView.Count - 1
            '                Dim pDRV = vDT.DefaultView(p)
            '                For Each pDC In vDT.Columns
            '                    Select Case MajorDataType(pDC.DataType)
            '                        Case eMajorDataType.Date, eMajorDataType.String
            '                            If pDRV(pDC.ColumnName) Is DBNull.Value Then
            '                                pInsertUpdate.Value(EscapeSqlName(pDC.ColumnName)) = "NULL"
            '                            Else
            '                                pInsertUpdate.Value(EscapeSqlName(pDC.ColumnName)) = ReadyString(pDRV(pDC.ColumnName))
            '                            End If

            '                        Case eMajorDataType.Numeric
            '                            If pDRV(pDC.ColumnName) Is DBNull.Value Then
            '                                pInsertUpdate.Value(EscapeSqlName(pDC.ColumnName)) = "NULL"
            '                            Else
            '                                pInsertUpdate.Value(EscapeSqlName(pDC.ColumnName)) = CDec2(pDRV(pDC.ColumnName))
            '                            End If

            '                    End Select
            '                Next pDC

            '                Try
            '                    Dim pSql = pInsertUpdate.GetInsert
            '                    pProData.ExecuteSQL(pSql)
            '#If DEBUG Then
            '                    If mTableSavingDuration > 0 Then
            '                        If pStopWatch Is Nothing Then
            '                            pStopWatch = New Stopwatch
            '                            pStopWatch.Start()
            '                        ElseIf pStopWatch.ElapsedMilliseconds > mTableSavingDuration Then
            '                            Exit For
            '                        End If
            '                    End If
            '#End If
            '                Catch ex As Exception
            '                    ' just ignore any errors
            '                    Debug.WriteLine("vb2P7Y5G5JES! " & ex.Message)
            '                End Try
            '            Next p

            pProData.SetInsertIdentity(pTableName, False, True)
            pDrTimeStamp("TimeEnd") = Now
            pDrTimeStamp("RecordCount") = vDT.DefaultView.Count
            pDrTimeStamp("TotalSeconds") = (CDate(pDrTimeStamp("TimeEnd")) - CDate(pDrTimeStamp("TimeStart"))).TotalSeconds
            mDtTimeStamp.Rows.Add(pDrTimeStamp)
            TableToExcelFile(mDtTimeStamp, "", "TimeStart, TimeEnd", mTimeStampFilename)

            Return True

        End Function

      Function SaveToCsvTable(ByVal vTableName As String, ByVal vDT As DataTable) As Boolean
            
            If mAllowedTableNames.ToLower().Contains("[" & vTableName.ToLower() & "]") = False Then
                Return True
            End If
            Debug.WriteLine("SaveToCsvTable: " & vTableName & " (" & vDT.Rows.Count & " rows)")
#If DEBUG Then
            mTableNames &= "[" & vTableName & "]" & vbTab & " (" & vDT.Rows.Count & " records)" & vbCrLf
            'If vDT.Rows.Count > 50 Then
            '    If vDT.Columns.Contains("LastUpdated") Then
            '        vDT.DefaultView.Sort = "LastUpdated DESC"
            '    End If
            '    vDT = GetTopRecords(vDT.DefaultView, 50).Table
            'End If
#End If
            mProZipCsv.AddTableCsv(vDT, vTableName)

            Return True

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

                    If pResult.Length > 1 Then
                        If Not pResult = pDt.Rows(i).Item("LastUpdated") Then
                            pSql = "Update TablesLastUpdated Set Lastupdated = " & ReadyString(pResult) & " Where TableName = " & ReadyString(CStr(pDt.Rows(i).Item("TableName")))
                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                    End If
                Next
            End If
        End Function

        Dim mDtTimeStamp As DataTable
        Dim mTimeStampFilename As String

        Private Sub CreateTimeStampTable()
            mDtTimeStamp = New DataTable
            mDtTimeStamp.Columns.Add("TimeStart", GetType(Date))
            mDtTimeStamp.Columns.Add("TimeEnd", GetType(Date))
            mDtTimeStamp.Columns.Add("TotalSeconds", GetType(Integer))
            mDtTimeStamp.Columns.Add("RecordCount", GetType(Integer))
            mDtTimeStamp.Columns.Add("Description")
        End Sub

        Private Function CreateSqlServerDatabase() As Boolean
            Dim mFilenamePrivateDB As String
            Dim mFilenamePublicDB As String
            Dim mFilenameCacheDB As String

            Dim mFilenamePrivateDBTemp As String
            Dim mFilenamePublicDBTemp As String
            Dim mFilenameCacheDBTemp As String

            Dim mFilenameFolder As String
            'Dim mFilenameParentFolder As String
            Dim pDbInfo As DbInfo
            Dim pDbInfoFilename As String
            Dim pDtDbFiles = WebRequest.Instance.ProData.GetDatabaseFiles("temp_" & ShortGuid(mOrgID) & "_public")
            If pDtDbFiles.Rows.Count > 0 Then
                Dim pDrDbFile = pDtDbFiles.Rows(0)
                mFilenameFolder = CStr2(pDrDbFile("physical_name"))
                If mFilenameFolder.StartsWith(Common.ServerPath & "temp\DataExport\Upload\" & mOrgID) Then
                    WebRequest.Instance.ProData.DetachDatabase("temp_" & ShortGuid(mOrgID) & "_public")
                End If
            End If

            pDtDbFiles = WebRequest.Instance.ProData.GetDatabaseFiles("temp_" & ShortGuid(mOrgID) & "_private")
            If pDtDbFiles.Rows.Count > 0 Then
                Dim pDrDbFile = pDtDbFiles.Rows(0)
                mFilenameFolder = CStr2(pDrDbFile("physical_name"))
                If mFilenameFolder.StartsWith(Common.ServerPath & "temp\DataExport\Upload\" & mOrgID) Then
                    WebRequest.Instance.ProData.DetachDatabase("temp_" & ShortGuid(mOrgID) & "_private")
                End If
            End If

            CreateTimeStampTable()
            Dim pDrTimeStamp = mDtTimeStamp.NewRow()
            pDrTimeStamp("TimeStart") = now
            pDrTimeStamp("Description") = "CreateSqlServerDatabase"

            mFilenameFolder = Common.ServerPath & "temp\DataExport\Upload\" & mOrgID & "\" & vSubFolder
            mTimeStampFilename = AppendBackslash(mFilenameFolder) & "Timestamp.xlsx"

            'mFilenameParentFolder = Common.ServerPath & "temp\DataExport\Upload\" & CStr(Common.WebRequest.Data("OrgID"))

            'If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
            '    System.IO.Directory.CreateDirectory(mFilenameParentFolder)
            'End If

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            'mFilenamePrivateDB = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".db"
            'mFilenamePublicDB = mFilenameFolder & "\Mobiz" & ".db"

            ' when the 1 public, 1 private is done:
            WebRequest.Instance.ProData.CreateDatabase("temp_" & ShortGuid(mOrgID) & "_public", mFilenameFolder)
            WebRequest.Instance.ProData.CreateDatabase("temp_" & ShortGuid(mOrgID) & "_private", mFilenameFolder)
            pDbInfoFilename = mFilenameFolder & "\" & mOrgID & ".txt"
            pDbInfo = New DbInfo()
            pDbInfo.DateStarted = Now
            SaveDbInfo(pDbInfo, pDbInfoFilename)

            mFilenamePrivateDBTemp = mFilenamePrivateDB & ".temp"
            mProDataPublic = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                .Server = Common.ConnectionInfo.Server,
                .Database = "temp_" & ShortGuid(mOrgID) & "_public",
                .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                .Username = Common.ConnectionInfo.Username,
                .Password = Common.ConnectionInfo.Password})
            mProDataPrivate = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                .Server = Common.ConnectionInfo.Server,
                .Database = "temp_" & ShortGuid(mOrgID) & "_private",
                .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                .Username = Common.ConnectionInfo.Username,
                .Password = Common.ConnectionInfo.Password})
            mSourceProDataPublic = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
            'mDatabase = mProData.GetCurrentDatabase()

            Dim pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                .Server = Common.ConnectionInfo.Server,
                .Database = WebRequest.Instance.ProData.GetCurrentDatabase(),
                .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                .Username = Common.ConnectionInfo.Username,
                .Password = Common.ConnectionInfo.Password})
            Dim pDatabase As New DBCompare.Database(pSourceProData)
            pDatabase.LoadFromSql(True, False, False, True)
            pDatabase.SetCompareStatusToUnique()
            DBCompare.Common.SqlDelimiter = vbCrLf & vbCrLf & "-- delimeter " & GenerateString(20) & vbCrLf & vbCrLf
            Dim pSql = pDatabase.GetScript()
            For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                Try
                    mProDataPrivate.ExecuteSQL(pLine)
                Catch ex As Exception
                    AppendToLog("vb8H7ELPELU2! private database " & ex.Message & vbCrLf & pLine, , "MobizExportController")
                    Throw
                End Try
            Next

            pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                .Server = Common.ConnectionInfo.Server,
                .Database = Common.ConnectionInfo.Database,
                .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                .Username = Common.ConnectionInfo.Username,
                .Password = Common.ConnectionInfo.Password})
            pDatabase = New DBCompare.Database(pSourceProData)
            pDatabase.LoadFromSql(True, False, False, True)
            pDatabase.SetCompareStatusToUnique()
            pSql = pDatabase.GetScript()
            For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                Try
                    mProDataPublic.ExecuteSQL(pLine)
                Catch ex As Exception
                    AppendToLog("vb9JWEFKD99Q! public database " & ex.Message & vbCrLf & pLine, , "MobizExportController")
                    Throw
                End Try
            Next

            With New Synonyms()
                .Prefix = "Mobiz"
                .SourceDatabaseName = "Mobiz"

                Try
                    .SourceDatabase = "temp_" & ShortGuid(mOrgID) & "_public"
                    .ProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                        .Server = Common.ConnectionInfo.Server,
                        .Database = "temp_" & ShortGuid(mOrgID) & "_public",
                        .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                        .Username = Common.ConnectionInfo.Username,
                        .Password = Common.ConnectionInfo.Password})
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try

                Try
                    .SourceDatabase = "temp_" & ShortGuid(mOrgID) & "_public"
                    .ProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                        .Server = Common.ConnectionInfo.Server,
                        .Database = "temp_" & ShortGuid(mOrgID) & "_private",
                        .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                        .Username = Common.ConnectionInfo.Username,
                        .Password = Common.ConnectionInfo.Password})
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try
            End With

            ' BEGIN: commented 2022_09_01__09_22_39_9535
            'pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            '    .Server = Common.ConnectionInfo.Server,
            '    .Database = Common.ConnectionInfo.Database,
            '    .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            '    .Username = Common.ConnectionInfo.Username,
            '    .Password = Common.ConnectionInfo.Password})
            'pDatabase = New DBCompare.Database(pSourceProData)
            'Dim pUdfs = pDatabase.LoadUDFs()

            'Dim pDestinationProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            '    .Server = Common.ConnectionInfo.Server,
            '    .Database = "temp_" & ShortGuid(mOrgID) & "_public",
            '    .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            '    .Username = Common.ConnectionInfo.Username,
            '    .Password = Common.ConnectionInfo.Password})
            'For Each pUdf In pUdfs.Values
            '    Try
            '        pDestinationProData.ExecuteSQL(pUdf.Text)
            '        Debug.WriteLine("breakpoint here")
            '    Catch ex As Exception
            '        If ex.Message.StartsWith("There is already an object named '") = False Then
            '            Throw
            '        End If
            '    End Try
            'Next

            'pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            '    .Server = Common.ConnectionInfo.Server,
            '    .Database = WebRequest.Instance.ProData.GetCurrentDatabase(),
            '    .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            '    .Username = Common.ConnectionInfo.Username,
            '    .Password = Common.ConnectionInfo.Password})
            'pDatabase = New DBCompare.Database(pSourceProData)
            'pUdfs = pDatabase.LoadUDFs()
            'pDestinationProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            '    .Server = Common.ConnectionInfo.Server,
            '    .Database = "temp_" & ShortGuid(mOrgID) & "_private",
            '    .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            '    .Username = Common.ConnectionInfo.Username,
            '    .Password = Common.ConnectionInfo.Password})
            'For Each pUdf In pUdfs.Values
            '    Try
            '        pDestinationProData.ExecuteSQL(pUdf.Text)
            '        Debug.WriteLine("breakpoint here")
            '    Catch ex As Exception
            '        If ex.Message.StartsWith("There is already an object named '") = False Then
            '            Throw
            '        End If
            '    End Try
            'Next
            ' END: commented 2022_09_01__09_22_39_9535

            pDrTimeStamp("TimeEnd") = Now
            pDrTimeStamp("TotalSeconds") = (CDate(pDrTimeStamp("TimeEnd")) - CDate(pDrTimeStamp("TimeStart"))).TotalSeconds
            mDtTimeStamp.Rows.Add(pDrTimeStamp)
            TableToExcelFile(mDtTimeStamp, "", "TimeStart, TimeEnd", mTimeStampFilename)

            Return True

        End Function

        Function CreateSqliteDatabase() As Boolean
            Dim pSqlitePrivateDB As ProSqlite
            Dim pSqlitePublicDB As ProSqlite
            Dim pSqliteCacheDB As ProSqlite

            Dim mFilenamePrivateDB As String
            Dim mFilenamePublicDB As String
            Dim mFilenameCacheDB As String

            Dim mFilenamePrivateDBTemp As String
            Dim mFilenamePublicDBTemp As String
            Dim mFilenameCacheDBTemp As String

            Dim mFilenameFolder As String
            'Dim mFilenameParentFolder As String
            Dim pDbInfo As DbInfo
            Dim pDbInfoFilename As String

            mFilenameFolder = Common.ServerPath & "temp\DataExport\Upload\" & CStr(Common.WebRequest.Data("OrgID")) & "\" & vSubFolder
            'mFilenameParentFolder = Common.ServerPath & "temp\DataExport\Upload\" & CStr(Common.WebRequest.Data("OrgID"))

            'If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
            '    System.IO.Directory.CreateDirectory(mFilenameParentFolder)
            'End If

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            CreateTimeStampTable()
            mTimeStampFilename = AppendBackslash(mFilenameFolder) & "Timestamp.xlsx"

            'mFilenamePrivateDB = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".db"
            'mFilenamePublicDB = mFilenameFolder & "\Mobiz" & ".db"

            ' when the 1 public, 1 private is done:
            mFilenamePrivateDB = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".private.db"
            CreateFolder(mFilenamePrivateDB)
            mFilenameCacheDB = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".cache.db"
            CreateFolder(mFilenameCacheDB)
            mFilenamePublicDB = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".public.db"
            pDbInfoFilename = mFilenameFolder & "\" & Common.WebRequest.Data("OrgID") & ".txt"
            pDbInfo = New DbInfo()
            pDbInfo.DateStarted = Now
            SaveDbInfo(pDbInfo, pDbInfoFilename)

            mFilenamePrivateDBTemp = mFilenamePrivateDB & ".temp"
            pSqlitePrivateDB = New ProSqlite
            pSqlitePrivateDB.Open(mFilenamePrivateDBTemp)

            mFilenamePublicDBTemp = mFilenamePublicDB & ".temp"
            pSqlitePublicDB = New ProSqlite
            pSqlitePublicDB.Open(mFilenamePublicDBTemp)

            mFilenameCacheDBTemp = mFilenameCacheDB & ".temp"
            pSqliteCacheDB = New ProSqlite
            pSqliteCacheDB.Open(mFilenameCacheDBTemp)

            'mDatabase = mProData.GetCurrentDatabase()
            Dim ParentTableName As String

            Try
                Dim pBusiness = New Business(mProData)
                pBusiness.LibraryOrganization.RestoreOrganizationTin()

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
                            If ParentTableName = "MobizLibraryOrganization" Then
                                AppendToLog("ParentTableName = " & ParentTableName & vbCrLf &
                                            "TableNameChild = " & TableNameChild & " ref: vbCE5U4UKE3D!")
                            End If
                            If TableNameChild.Contains("Mobiz") Then
                                pSqlitePublicDB.CreateTable(Replace(TableNameChild, "Mobiz", ""), GetTableSchema(TableNameChild))
                            Else
                                pSqlitePrivateDB.CreateTable(TableNameChild, GetTableSchema(TableNameChild))
                            End If

                        Next
                    End If
                Next

                pSqliteCacheDB.CreateTable("CacheCSR", GetTableSchema("MobizCacheCSR"))
                pSqliteCacheDB.CreateTable("CacheMSR", GetTableSchema("MobizCacheMSR"))
                pSqliteCacheDB.CreateTable("CacheZRead", GetTableSchema("TransZReadCache")) 'diri na masave ang cache ZRead data dili na dapat sa private
                pSqliteCacheDB.CreateTable("CacheCashDenominationBreakdown", GetTableSchema("TransCashDenominationBreakdown"))

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
                pSqlitePrivateDB.CreateTable("TransInvoiceItemsVoided", GetTableSchema("TransInvoiceItemsVoided"))
                pSqlitePrivateDB.CreateTable("TransInvoiceItemsRebates", GetTableSchema("TransInvoiceItemsRebates"))
                pSqlitePrivateDB.CreateTable("TransInvoiceItemSalesReps", GetTableSchema("TransInvoiceItemSalesReps"))
                pSqlitePrivateDB.CreateTable("TransInvoiceItemSerials", GetTableSchema("TransInvoiceItemSerials"))
                pSqlitePrivateDB.CreateTable("TransMemo", GetTableSchema("TransMemo"))
                pSqlitePrivateDB.CreateTable("TransMemoSalesReturn", GetTableSchema("TransMemoSalesReturn"))
                pSqlitePrivateDB.CreateTable("TransPIMAlerts", GetTableSchema("TransPIMAlerts"))
                pSqlitePrivateDB.CreateTable("TransReceiving", GetTableSchema("TransReceiving"))
                pSqlitePrivateDB.CreateTable("TransReceivingStock", GetTableSchema("TransReceivingStock"))
                pSqlitePrivateDB.CreateTable("TransReceivingStockSerials", GetTableSchema("TransReceivingStockSerials"))
                pSqlitePrivateDB.CreateTable("TransSalesQuotations", GetTableSchema("TransSalesQuotations"))
                pSqlitePrivateDB.CreateTable("TransSalesQuotationsItems", GetTableSchema("TransSalesQuotationsItems"))
                pSqlitePrivateDB.CreateTable("TransSalesReturnLedger", GetTableSchema("TransSalesReturnLedger"))
                pSqlitePrivateDB.CreateTable("TransInvoiceAdjustmentReturnedChecks", GetTableSchema("TransInvoiceAdjustmentReturnedChecks"))
                pSqlitePrivateDB.CreateTable("TransInvoiceAdjustments", GetTableSchema("TransInvoiceAdjustments"))

                pSqlitePrivateDB.CreateTable("TransBomMaker", GetTableSchema("TransBomMaker"))
                pSqlitePrivateDB.CreateTable("TransBomMakerContents", GetTableSchema("TransBomMakerContents"))
                pSqlitePrivateDB.CreateTable("TransBomMakerProducts", GetTableSchema("TransBomMakerProducts"))
                pSqlitePrivateDB.CreateTable("TransZReadCache", GetTableSchema("TransZReadCache"))
                pSqlitePrivateDB.CreateTable("TransSMCOINCache", GetTableSchema("TransSMCOINCache"))
                pSqlitePrivateDB.CreateTable("TransSMCOINTempCache", GetTableSchema("TransSMCOINTempCache"))
                pSqlitePrivateDB.CreateTable("TransSalesProductCount", GetTableSchema("TransSalesProductCount"))

                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTTel", GetTableSchema("MobizLibraryOrganizationFOWSTTel"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTMallInfoSM", GetTableSchema("MobizLibraryOrganizationFOWSTMallInfoSM"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTEmail", GetTableSchema("MobizLibraryOrganizationFOWSTEmail"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationFOWSTOperationHRs", GetTableSchema("MobizLibraryOrganizationFOWSTOperationHRs"))

                pSqlitePublicDB.CreateTable("LibraryOrganizationRepresentativesEmail", GetTableSchema("MobizLibraryOrganizationRepresentativesEmail"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationRepresentativesMobile", GetTableSchema("MobizLibraryOrganizationRepresentativesMobile"))
                pSqlitePublicDB.CreateTable("LibraryOrganizationRepresentativesTel", GetTableSchema("MobizLibraryOrganizationRepresentativesTel"))

                pSqlitePrivateDB.CreateTable("TransExpensesCheckInOutDetails", GetTableSchema("TransExpensesCheckInOutDetails"))
                pSqlitePrivateDB.CreateTable("TransExpensesLogDetails", GetTableSchema("TransExpensesLogDetails"))
                pSqlitePrivateDB.CreateTable("TransExpensesReceiveFunds", GetTableSchema("TransExpensesReceiveFunds"))
                pSqlitePrivateDB.CreateTable("TransNetFundBalance", GetTableSchema("TransNetFundBalance"))
                pSqlitePrivateDB.CreateTable("TransLogExpensesCache", GetTableSchema("TransLogExpensesCache"))
                pSqlitePrivateDB.CreateTable("TransReceiveFundsCache", GetTableSchema("TransReceiveFundsCache"))
                pSqlitePrivateDB.CreateTable("TransExpensesLogDetailsHistory", GetTableSchema("TransExpensesLogDetailsHistory"))
                pSqlitePrivateDB.CreateTable("TransExpensesReceiveFundsHistory", GetTableSchema("TransExpensesReceiveFundsHistory"))
                pSqlitePrivateDB.CreateTable("TransCashHandling", GetTableSchema("TransCashHandling"))
                pSqlitePrivateDB.CreateTable("TransCashHandlingCashCount", GetTableSchema("TransCashHandlingCashCount"))
                pSqlitePrivateDB.CreateTable("TransDailyBeginningBalance", GetTableSchema("TransDailyBeginningBalance"))

                pSqlitePublicDB.CreateTable("LibraryPIMProductFast", GetTableSchema("MobizLibraryPIMProductFast"))
                pSqlitePrivateDB.CreateTable("TransDTR", GetTableSchema("TransDTR"))

                pSqlitePrivateDB.CreateTable("LibraryProductsWithoutBarcode", GetTableSchema("LibraryProductsWithoutBarcode"))
                pSqlitePrivateDB.CreateTable("TransPeerImport", GetTableSchema("TransPeerImport"))
                pSqlitePrivateDB.CreateTable("TransInvoiceItemsModifier", GetTableSchema("TransInvoiceItemsModifier"))

                pSqlitePublicDB.CreateTable("TransSmSMessages", GetTableSchema("MobizTransSmSMessages"))
                pSqlitePrivateDB.CreateTable("TransSmSMessages", GetTableSchema("TransSmSMessages"))

                pSqlitePrivateDB.CreateTable("TransActionLogs", GetTableSchema("TransActionLogs"))
                pSqlitePrivateDB.CreateTable("TenantMonitoringLogs", GetTableSchema("TenantMonitoringLogs"))
                mProData.RollbackTransaction() ' dont commit

                pSqlitePrivateDB.Close()
                If IO.File.Exists(mFilenamePrivateDB) Then IO.File.Delete(mFilenamePrivateDB)
                IO.File.Move(mFilenamePrivateDBTemp, mFilenamePrivateDB)

                pSqlitePublicDB.Close()
                If IO.File.Exists(mFilenamePublicDB) Then IO.File.Delete(mFilenamePublicDB)
                IO.File.Move(mFilenamePublicDBTemp, mFilenamePublicDB)

                pSqliteCacheDB.Close()
                If IO.File.Exists(mFilenameCacheDB) Then IO.File.Delete(mFilenameCacheDB)
                IO.File.Move(mFilenameCacheDBTemp, mFilenameCacheDB)

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

        Private Function CreateCsvDatabase() As Boolean

            Dim pFilename = Common.ServerPath & "temp\DataExport\Upload\" & mKey & ".zip"
            CreateFolder(pFilename)
            mProZipCsv = New ProZip(pFilename)

            Dim pDbInfo As DbInfo
            pDbInfo = New DbInfo()
            pDbInfo.DateStarted = Now
            pDbInfo.ForCsv = True
            pDbInfo.OrgID = mOrgID
            Dim pStr = JsonConvert.SerializeObject(pDbInfo)
            mProZipCsv.AddString(pStr, mOrgID & ".txt")

            Return True

        End Function

        Private Class DbInfo
            Public DateStarted As Date
            Public DateEnded As Date?
            Public OrgID As String
            Public ForCsv As Boolean
            Public DatabaseCreationStart As Date
        End Class

        Private Function LoadDbInfo(vFilename As String) As DbInfo

            Dim pOut As DbInfo
            Dim pStr As String

            pStr = FileText(vFilename)
            pOut = JsonConvert.DeserializeObject(Of DbInfo)(pStr)

            Return pOut

        End Function

        Private Sub SaveDbInfo(vDbInfo As DbInfo, vFilename As String)

            Dim pStr As String

            pStr = JsonConvert.SerializeObject(vDbInfo)

            FileText(vFilename) = pStr

        End Sub

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

        Public Function SaveDeviceLog() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInput = JsonConvert.DeserializeObject(Of LibraryOrganization.SaveLogInput)(WebRequest.Instance.InputStreamText)
            WebRequest.Instance.Business.LibraryOrganization.SaveDeviceLog(pInput)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function
    End Class

End Namespace

Public Class MobizExport
    Inherits DataImport

    Public Sub New()

        With MyBase.ParentTables
            .Add("MobizLibraryFowstType")
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
            .Add("MobizLibraryOrganizationFranchisee")
            .Add("MobizLibraryPIMProductParent")
            .Add("MobizLibraryFranchiseGroupPriceLevels")

            .Add("MobizLibraryPIMFeature")
            .Add("LibraryOrganizationDashboardProductFeatures")

            With .Add("MobizLibraryPeople")
                With .Children
                    .Add("MobizLibraryPeopleUserAccount", "PeopleID")
                    .Add("MobizLibraryPeopleOrganization", "PeopleID")
                    .Add("MobizLibraryPeopleContactEmail", "PeopleID")
                End With
            End With


            With .Add("MobizLibraryOrganization")
                With .Children
                    .Add("MobizLibraryOrganizationContactEmail", "OrganizationID")
                    .Add("MobizLibraryOrganizationTradeName", "OrganizationID")
                    .Add("MobizLibraryOrganizationFOWST", "OrganizationID")
                    .Add("MobizLibraryOrganizationRepresentatives", "OrganizationID")
                    .Add("MobizLibraryOrganizationContactPerson", "OrganizationID")
                End With
            End With

            .Add("MobizLibraryOrganizationFOWSTMallInfoAyala")

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
            .Add("MobizLibrarySubscription")
            .Add("LibraryDiscountSettings")
            .Add("LibraryOrganizationPriceLevel")
            .Add("LibraryOrganizationCustomer")
            .Add("LibraryOrganizationEmployee")
            .Add("LibraryOrganizationEmployeeFOWST")
            .Add("LibraryOrganizationPIM")
            .Add("LibraryOrganizationPIMPriceBreak")
            .Add("LibraryFowstBin")
            .Add("LibraryFowstBinProduct")
            .Add("LibraryOrganizationCustomerFowstBinProduct")
            .Add("LibraryPurchaseOrderSettings")
            .Add("LibraryOrganizationSupplier") ' 2015_08_29__10_59_49_8184

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
                    .Add("LibraryDevicePrinterFowst", "PrinterID")
                End With
            End With
            .Add("LibraryDevicePrinterCategory")

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
            With .Add("LibraryItemModifiers")
                With .Children
                    .Add("LibraryItemModifierCategories", "ItemModifierID")
                    .Add("LibraryItemModifierRebates", "ItemModifierID")
                End With
            End With

            With .Add("LibraryTableTemplate")
                With .Children
                    .Add("LibraryTableFowst", "TemplateID")
                    .Add("LibraryTableDiningArea", "TemplateID")
                End With
            End With
            .Add("LibraryTableJoined")
            .Add("LibraryTableTableInfo")
            .Add("LibraryFowstArea")
            '.Add("TransZReadCache")
            .Add("TransSMCOINCache")
            .Add("LibraryItemModifierInventoryAffecting")
            .Add("LibraryItemModifierInventoryAffectingFowst")
            .Add("LibraryItemModifierNonInventoryAffecting")
            .Add("LibraryItemModifierAssignedFowst")
            .Add("LibraryItemModifierAssignedCustomer")
            .Add("LibraryItemModifierSorting")
            .Add("LibraryExpensesCenters")
            .Add("LibraryExpensesRouteSchedules")
            .Add("LibraryExpensesRouteSchedulesEmployees")
            .Add("LibraryCustomDiscounts")
            .Add("LibraryCustomDiscountsFowst")
            .Add("MobizLibraryOtherPayments")
            .Add("LibraryPimUpcomingPrices")
            .Add("MobizLibraryIndustryClass")
            '.Add("TenantMonitoringLogs") ' uncommented 2024_09_05__14_59_14_3554 | commented 2024_09_03__16_27_30_1551
            .Add("TransPimSalesCaching")
            .Add("LibraryPimMenuButtonAssignedStores")
            .Add("LibraryPIMPictures")
            .Add("MobizLibraryVouchers")
            .Add("LibraryVendingMachines")
            .Add("LibraryVendingMachineBoards")
            .Add("LibraryVendingMachinePlanogramTemplateList")
            .Add("LibraryVendingMachineAuthorizers")
            .Add("LibraryVendingMachineAuthorizersAccessRights")
            .Add("LibraryVDMGuides")
            .Add("LibraryVDMGuidesAssignedFowst")
            .Add("LibraryVDMGuidesTemplate")
            .Add("LibraryVDMAds")
            .Add("LibraryVDMAdsTemplate")
            .Add("LibraryVDMAdsAssignedFowst")
            .Add("LibraryVDMProductSelectionGuides")
            .Add("TransVDMLogs")
            .Add("TransInventoryCountItemPerUom")
            .Add("LibraryPimCategories")
            '.Add("TransExpensesLogDetails")
            '.Add("TransLogExpensesCache")
            '.Add("TransExpensesReceiveFunds")
            '.Add("TransReceiveFundsCache")
            '.Add("TransNetFundBalance")
            '.Add("TransExpensesCheckInOutDetails")
            '.Add("LibraryReservationTable") // for Rservation Table by arnie 6292017
            '.Add("LibraryReservationInfo") // for Rservation Table by arnie 6292017
            '.Add("TransInvoiceItemsModifier")
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