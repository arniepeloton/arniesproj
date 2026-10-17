Public Class RowCountChecksumDuc
    Implements IDisposable

    Public Const OrgID As String = "9D4B1500-13C0-49F9-A8E5-A16C13EC899E"
    Public SqliteFilename As String
    Dim mSqlite As ProSqlite

    Dim mProData As ProData
    Dim mBusiness As Business

    Public Sub New(vSqliteFilename As String)

        SqliteFilename = vSqliteFilename

    End Sub

    Public Function Process() As Boolean

        If ConnectToDatabase() = False Then
            Return False
        End If

        If IO.File.Exists(SqliteFilename) = False Then
            AddError("Sqlite file, " & SqliteFilename & ", does not exist.")
            Return False
        End If

        Using pSqlite = New ProSqlite()
            mSqlite = pSqlite
            mSqlite.Open(SqliteFilename)
        End Using

        CheckTables()

        Return True

    End Function

    Private Function CheckTables() As Boolean

        Dim pCount As Integer
        Dim pDT As DataTable
        Dim pSql As String
        Dim pDiffTables As String = ""

        pSql = "SELECT * FROM RowCount"
        pDT = mSqlite.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            If CStr2(pDR("Table")).ToLower <> "TotalRowCount".ToLower() Then
                pCount = CheckTable(pDR("Table"))
                AddCount(pDR("Table"), CDec2(pDR("Count")), pCount)

                If pCount <> pDR("Count") Then
                    If pDiffTables <> "" Then pDiffTables &= ", "
                    pDiffTables &= pDR("Table")
                End If
            End If
        Next pDR

        If pDiffTables <> "" Then
            mBusiness.TransDevAlerts.AddEntry("DUC row count does not equal SQL Server Mobiz database row count on: " & pDiffTables, "RowCountChecksumDuc", OrgID)
        End If

    End Function

    Private Function CheckTable(vTableName As String) As Integer

        Dim pTableName As String
        Dim pCount As Integer
        Dim pSql As String
        Dim pDT As DataTable
        Dim pDrTest As DataRow
        Dim pStringFields() As String
        Dim pWhere As String

        Select Case vTableName.ToLower()
            Case "MobizLibraryPIMProduct".ToLower(), _
                "MobizLibraryOrganization".ToLower(), _
                "MobizLibraryOrganizationFOWST".ToLower(),
                "MobizLibraryPIMProductMeasurement".ToLower(), _
                "MobizLibraryPIMProductMeasurementBarcode".ToLower(), _
                "MobizLibraryPIMProductPriceLevel".ToLower(),
                "MobizLibraryPIMPriceLevel".ToLower()

                pStringFields = {"ID"}

            Case "MobizLibraryPIMProductVariableFeatures001".ToLower()
                pStringFields = { _
                    "ProductID", _
                    "C0020", _
                    "C0056", _
                    "C0057", _
                    "C0058", _
                    "C0059", _
                    "C0060", _
                    "C0061", _
                    "C0063"
                }
                ' old = C0062; new = C0020
            Case Else
                Return -1

        End Select

        pTableName = vTableName
        '// create SELECT
        pSql = ""
        For Each pField As String In pStringFields
            If pSql <> "" Then pSql &= ", "
            pSql &= pField
        Next pField
        pSql = "SELECT " & pSql & " FROM " & vTableName

        pDT = mSqlite.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            '// create WHERE
            pWhere = ""
            For Each pField As String In pStringFields
                If pWhere <> "" Then pWhere &= " AND "
                pWhere &= " ISNULL(" & pField & ", '') = " & ReadyString(pDR(pField))
            Next pField

            pSql = "SELECT ID FROM " & EscapeSqlName(pTableName) &
              " WHERE " & pWhere
            pDrTest = mProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                pCount += 1
            End If
        Next pDR

        Return pCount

    End Function

    Private Function IsTableExist(vTableName As String) As Boolean
        ' cant use mProData.TableExist
        ' mProData.TableExist ignores synonyms

        Try
            mProData.Execute("SELECT TOP 0 * FROM " & EscapeSqlName(vTableName))
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Function ConnectToDatabase() As Boolean

        Dim pBusiness As Business
        Dim pDR As DataRow

        mProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=Common.ConnectionInfo.Database,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
        pBusiness = New Business(mProData)
        pDR = pBusiness.LibraryOrganization.ByID(OrgID)
        If pDR Is Nothing Then
            AddError("OrgID, " & OrgID & ", does not exist in the database.")
            Return False
        End If

        If CStr2(pDR("Database")).Trim() = "" Then
            AddError("OrgID, " & OrgID & ", is not a subscriber.")
            Return False
        End If

        Try
            mProData.ExecuteSQL("USE " + EscapeSqlName(pDR("Database")))
        Catch ex As Exception
            AddError("An error occured while connecting to database, " & pDR("Database") & ", for OrgID, " & OrgID & ".")
            Return False
        End Try

        mProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=pDR("Database"),
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
        mBusiness = New Business(mProData)

        Return True

    End Function

    Private Sub AddError(vMessage As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            pIU = New InsertUpdate("MobizTransRowCount")
            pIU("ID") = "newid()"
            pIU("OrgID") = ReadyString(OrgID)
            'pIU("DeviceID") = ReadyString(DeviceID)
            pIU("DateLogged") = "GETDATE()"
            pIU("ErrorMessage") = ReadyString(vMessage)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Stop2()
        End Try

    End Sub

    Private Sub AddCount(vTableName As String, vDeviceRowCount As Integer, vServerRowCount As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            If vDeviceRowCount = 0 _
                AndAlso vServerRowCount = 0 Then

                Exit Sub
            End If

            pIU = New InsertUpdate("MobizTransRowCount")
            pIU("ID") = "newid()"
            pIU("OrgID") = ReadyString(OrgID)
            'pIU("DeviceID") = ReadyString(DeviceID)
            pIU("DateLogged") = "GETDATE()"
            pIU("DeviceRowCount") = vDeviceRowCount
            pIU("ServerRowCount") = vServerRowCount
            pIU("TableName") = ReadyString(vTableName)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Stop2()
        End Try

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Try
                    mProData.Dispose()
                Catch ex As Exception
                    Stop2()
                End Try
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
