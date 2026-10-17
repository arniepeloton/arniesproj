Imports System.Threading.Tasks

Public Class DbSchemaCoder
    Public Function StartAsync() As Task

        Return Task.Run(AddressOf Start)

    End Function

    Public Function Start() As String

        Dim pChanged As Integer
        Dim pLog As String
        Dim pConnectionInfos As New List(Of ConnectionInfo)

        Try
            pConnectionInfos.Add(Common.ConnectionInfo) ' main connection info

            ' flushing connection info (optional)
            Dim pFilename = Common.ServerPath & "settings\FlushingDb.data"
            If IO.File.Exists(pFilename) Then
                Dim pFlushingConnectionInfo = New ConnectionInfo()
                pFlushingConnectionInfo.Filename = pFilename
                If pFlushingConnectionInfo.Open() Then
                    pConnectionInfos.Add(pFlushingConnectionInfo)
                End If
            End If

            For Each pConnectionInfo In pConnectionInfos
                AppendToLog("ApplicationStart > DbSchemaCoder Begin vbPV96J92KW6!", , "Global_Start")
                pLog = "ApplicationStart > DbSchemaCoder vbD353WVAQ4R!" & vbCrLf &
                        "ConnectionInfo.Server: " & pConnectionInfo.Server & vbCrLf &
                        "ConnectionInfo.Database: " & pConnectionInfo.Database & vbCrLf
                AppendToLog(pLog, , "Global_Start")
                Dim pDbsFinal As New List(Of String)
                Using pProData = ProData.CreateFromConnectionInfo(pConnectionInfo)
                    pLog = "ApplicationStart > DbSchemaCoder vbPMUEVUZZAW!" & vbCrLf &
                        "pProData.Server: " & pProData.Server & vbCrLf &
                        "pProData.Database: " & pProData.Database & vbCrLf
                    AppendToLog(pLog, , "Global_Start")
                    Dim pPublicTablesAdded As Integer
                    pChanged += UpdatePublicDatabase(pProData, pPublicTablesAdded)

                    Dim pDbs = pProData.GetDatabases()
                    For Each pDB As String In pDbs
                        If DbConnectController.IsPrivateDatabase(pDB) Then
                            pDbsFinal.Add(pDB)
                        End If
                    Next pDB

                    For Each pDB In pDbsFinal
                        pChanged += UpdatePrivateDatabase(pProData, pDB, pPublicTablesAdded)
                    Next
                End Using

            Next pConnectionInfo

            AppendToLog("ApplicationStart > DbSchemaCoder Success", , "Global_Start")
            If pChanged > 0 Then
                Try
                    pFilename = Common.ServerPath & "temp\schema.zip"
                    If IO.File.Exists(pFilename) Then
                        IO.File.Delete(pFilename)
                    End If
                Catch ex As Exception
                    Return "Number of schema changes :" & pChanged & vbCrLf &
                        "Error occured while deleting " & pFilename & ": " & ex.Message
                End Try
            End If
            Return pChanged
        Catch ex As Exception
            Dim pStr = "ApplicationStart > DbSchemaCoder Error" & vbCrLf &
                ex.Message & vbCrLf &
                GetStackTrace(, ex)
            If ex.GetType().IsAssignableFrom(GetType(ProSqlException)) Then
                pStr = "SQL: " & CType(ex, ProSqlException).Sql & vbCrLf & pStr
            End If

            AppendToLog(pStr, , "Global_Start")
            Return pStr
        End Try

    End Function

    Private Function UpdatePublicDatabase(vProData As ProData, ByRef oTablesAdded As Integer) As Integer

        Dim pChanged As Integer
        Dim pSql As String = ""

        oTablesAdded = 0
        If vProData.ExecuteRow("SELECT ID FROM MobizLibraryPeople WHERE ID = " & ReadyString(modClient.EmployeeWebGuidID)) Is Nothing Then
            Dim pIU = New InsertUpdate("MobizLibraryPeople")
            pIU("ID") = ReadyString(EmployeeWebGuidID)
            pIU("Surname") = "'System Generated'"
            pIU("MiddleName") = "''"
            pIU("FirstName") = "''"
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pSql = pIU.GetInsert()
            vProData.ExecuteSQL(pSql)
        End If

        If vProData.TableExist("LibraryVouchers") = False Then
            pSql = " CREATE TABLE dbo.LibraryVouchers (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   SourceOrgID uniqueidentifier NULL," & vbCrLf &
                "   FOWSTID uniqueidentifier NULL," & vbCrLf &
                "   ReferenceNo varchar(50) NULL," & vbCrLf &
                "   ProductCode varchar(50) NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   UploadedByID uniqueidentifier NULL," & vbCrLf &
                "   UploadedByName varchar(MAX) NULL," & vbCrLf &
                "   CashierID uniqueidentifier NULL," & vbCrLf &
                "   CashierName varchar(MAX) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   TransactionID uniqueidentifier NULL," & vbCrLf &
                "   TransactionDate datetime NULL," & vbCrLf &
                "   DateImported datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        pChanged += If(ChangeLibraryVouchers(vProData, "Mobiz"), 1, 0)
        pChanged += If(ChangeLibraryOrganizationContactWebsite(vProData, "Mobiz"), 1, 0)

        If vProData.TableExist("LibrarySalesChannels") = False Then
            pSql = " CREATE TABLE dbo.LibrarySalesChannels (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL," & vbCrLf &
                "   IsActive bit NOT NULL," & vbCrLf &
                "   LastUpdated datetime NOT NULL," & vbCrLf &
                "   UserID uniqueidentifier NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)," & vbCrLf &
                "   UNIQUE (Name)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            oTablesAdded += 1
            pChanged += 1
        End If

        If vProData.TableExist("LibraryUserDefinedIndustry") = False Then
            pSql = " CREATE TABLE dbo.LibraryUserDefinedIndustry (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL," & vbCrLf &
                "   IsActive bit NOT NULL," & vbCrLf &
                "   LastUpdated datetime NOT NULL," & vbCrLf &
                "   UserID uniqueidentifier NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)," & vbCrLf &
                "   UNIQUE (Name)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            oTablesAdded += 1
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPIMProductRelationship") = False Then
            pSql = " CREATE TABLE dbo.LibraryPIMProductRelationship (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   OrganizationID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   RelationshipID uniqueidentifier NULL," & vbCrLf &
                "   Tag varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   IsActive bit NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            oTablesAdded += 1
            pChanged += 1
        End If

        If vProData.TableExist("TransMobizIssues") = False Then
            pSql = " CREATE TABLE dbo.TransMobizIssues (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   OrgID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   TicketNo varchar(50) NULL," & vbCrLf &
                "   ReportedBy uniqueidentifier NULL," & vbCrLf &
                "   DateReported datetime NULL," & vbCrLf &
                "   Product varchar(150) NULL," & vbCrLf &
                "   Subject varchar(150) NULL," & vbCrLf &
                "   Urgency varchar(50) NULL," & vbCrLf &
                "   Importance varchar(50) NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   Notify bit NULL," & vbCrLf &
                "   Status varchar(50) NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("TransFranchiseeSyncDataLogs") = False Then
            pSql = " CREATE TABLE dbo.TransFranchiseeSyncDataLogs (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FranchisorID uniqueidentifier NULL," & vbCrLf &
                "   FranchiseeID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("LibraryDeviceFOWSTMonitoring") = False Then
            pSql = " CREATE TABLE dbo.LibraryDeviceFOWSTMonitoring (" & vbCrLf &
                "   ID uniqueidentifier DEFAULT newid() ROWGUIDCOL NOT NULL," & vbCrLf &
                "   OrgID uniqueidentifier NOT NULL," & vbCrLf &
                "   UUID varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   DeviceID uniqueidentifier NULL," & vbCrLf &
                "   FOWSTID uniqueidentifier NULL," & vbCrLf &
                "   LastDataFlushReceivedInServer datetime2(7) NULL," & vbCrLf &
                "   LastDateOfTransactionFlushedFromDevice datetime2(7) NULL," & vbCrLf &
                "   LastDataSync datetime2(7) NULL," & vbCrLf &
                "   InstalledDatabaseCreationDate datetime2(7) NULL," & vbCrLf &
                "   APKVersion varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   APKBuildDate datetime2(7) NULL," & vbCrLf &
                "   LastUpdated datetime2(7) DEFAULT getdate() NULL," & vbCrLf &
                "   CONSTRAINT LibraryDeviceFOWSTMonitoring_pk PRIMARY KEY NONCLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("LibraryOrganizationFOWSTPriceLevel") = False Then
            pSql = " CREATE TABLE dbo.LibraryOrganizationFOWSTPriceLevel (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   PriceLevelID uniqueidentifier NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("LibraryOrganizationFOWSTMallInfoMegaworld") = False Then
            pSql = " CREATE TABLE dbo.LibraryOrganizationFOWSTMallInfoMegaworld (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   TenantCode varchar(50) NULL," & vbCrLf &
                "   SaleType varchar(50) NULL," & vbCrLf &
                "   TenantPOSTerminalNo int NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("LibraryPromoCampaign") = False Then
            pSql = " CREATE TABLE dbo.LibraryPromoCampaign (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   OrgID uniqueidentifier NULL," & vbCrLf &
                "   CampaignName varchar(MAX) NULL," & vbCrLf &
                "   PromoMessage varchar(MAX) NULL," & vbCrLf &
                "   PromoStart date NULL," & vbCrLf &
                "   PromoEnd date NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   ModifiedBy uniqueidentifier NULL," & vbCrLf &
                "   IsAllowMultiplePromoCode bit NULL," & vbCrLf &
                "   IsActive bit NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("LibraryPromoCampaignCode") = False Then
            pSql = " CREATE TABLE dbo.LibraryPromoCampaignCode (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   CampaignID uniqueidentifier NULL," & vbCrLf &
                "   PromoCode varchar(MAX) NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   DeviceID uniqueidentifier NULL," & vbCrLf &
                "   FowstCode varchar(MAX) NULL," & vbCrLf &
                "   FowstName varchar(MAX) NULL," & vbCrLf &
                "   DeviceName varchar(MAX) NULL," & vbCrLf &
                "   CashierID uniqueidentifier NULL," & vbCrLf &
                "   CashierName varchar(MAX) NULL," & vbCrLf &
                "   Status varchar(MAX) NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier NULL," & vbCrLf &
                "   InvoiceNo int NULL," & vbCrLf &
                "   DateImported datetime NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If vProData.TableExist("LibraryPromoCampaignProducts") = False Then
            pSql = " CREATE TABLE dbo.LibraryPromoCampaignProducts (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   CampaignID uniqueidentifier NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   ProductNo varchar(MAX) NULL," & vbCrLf &
                "   ProductName varchar(MAX) NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        pChanged += If(vProData.AddColumn("LibraryVouchers", "VoucherDescription", "varchar(30)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganization", "ChildName", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganization", "ChildBirthdate", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganization", "ChildAge", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganization", "TinNumberBackup", "varchar(30)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganization", "UserDefinedIndustryID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganization", "VatType", "varchar(30)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPeople", "Fullname", "varchar(200)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPeople", "Age", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMobizIssues", "DelegateToClient", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMobizIssues", "CustomFowst", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPeopleUserAccount", "LastUpdated", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPeopleUserAccount", "ApprovedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFOWSTMonitoring", "IsActive", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "EnforceZRead", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "IsVAT", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "Status", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "Classification", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWSTOperationHRs", "IsClosed", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWSTMallInfoMegaworld", "TenantPOSTerminalNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "LastSalesDataImportedToServer", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "RobinsonTenantId", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "RobinsonPosTerminalNum", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "PITXTenantId", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "PITXPosTerminalID", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "DataImportationPort", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "PrintStickers", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "PrintQueueNumbers", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "FtpServer", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "FtpUsername", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "FtpPassword", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "Operates247", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFOWST", "PosVendorID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOtherPayments", "LockDiscount", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOtherPayments", "StrictReferenceCharLength", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOtherPayments", "PaymentReferenceLength", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOtherPayments", "DataType", "varchar(50)"), 1, 0)

        If vProData.TableExist("LibraryPosVendors") = False Then
            pSql = " CREATE TABLE dbo.LibraryPosVendors (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(500) NOT NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
            oTablesAdded += 1
        End If

        If oTablesAdded > 0 Then
            With New Synonyms()
                .Prefix = "Mobiz"
                Try
                    .SourceDatabase = "Mobiz"
                    .ProData = vProData
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try
            End With
        End If

        Return pChanged

    End Function

    Private Function UpdatePrivateDatabase(vProData As ProData, vDatabase As String, vPublicTablesAdded As Integer) As Integer

        Dim pChanged As Integer
        Dim pSql As String
        vProData.ExecuteSQL("USE " & EscapeSqlName(vDatabase))
        AppendToLog("ApplicationStart > UpdatePrivateDatabase > " & EscapeSqlName(vDatabase), , "Global_Start")

        If vProData.TableExist("TransItemRequisitions") = False Then
            pSql = " CREATE TABLE dbo.TransItemRequisitions (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   VendorID uniqueidentifier NULL," & vbCrLf &
                "   RequestedQty numeric(18, 6) NOT NULL," & vbCrLf &
                "   RequestedByID uniqueidentifier NULL," & vbCrLf &
                "   UserID uniqueidentifier NOT NULL," & vbCrLf &
                "   Remarks varchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   DateRequested datetime2(7) NOT NULL," & vbCrLf &
                "   UnitMeasure varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   UnitQty money NULL," & vbCrLf &
                "   IsDeactivated bit NULL," & vbCrLf &
                "   CreatedPurchaseOrderID uniqueidentifier NULL," & vbCrLf &
                "   BackorderQty int NULL," & vbCrLf &
                "   ComputedRequestedYearWeek AS (('Y'+CONVERT([varchar],datepart(year,[DateRequested]),0))+'|W')+right('00'+isnull(CONVERT([varchar],datepart(iso_week,[DateRequested]),0),''),(2))," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   RequestedQtyTotal numeric(18, 6) NOT NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransCashDenominationBreakdown") = False Then
            pSql = " CREATE TABLE dbo.TransCashDenominationBreakdown (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   SalesDate date NULL," & vbCrLf &
                "   LastInvoiceID uniqueidentifier NULL," & vbCrLf &
                "   FiveCents int NULL," & vbCrLf &
                "   TwentyFiveCents int NULL," & vbCrLf &
                "   FiftyCents int NULL," & vbCrLf &
                "   One int NULL," & vbCrLf &
                "   Five int NULL," & vbCrLf &
                "   Ten int NULL," & vbCrLf &
                "   Twenty int NULL," & vbCrLf &
                "   Fifty int NULL," & vbCrLf &
                "   OneHundred int NULL," & vbCrLf &
                "   TwoHundred int NULL," & vbCrLf &
                "   FiveHundred int NULL," & vbCrLf &
                "   OneThousand int NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransDeliveryLogger") = False Then
            pSql = " CREATE TABLE dbo.TransDeliveryLogger (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   DeliveryMethod varchar(100) NULL," & vbCrLf &
                "   DeliveryTruckFowstID uniqueidentifier NULL," & vbCrLf &
                "   Delivery3PlCompanyID uniqueidentifier NULL," & vbCrLf &
                "   Delivery3PlContactPerson varchar(500) NULL," & vbCrLf &
                "   DeliveryWaybillNumber varchar(500) NULL," & vbCrLf &
                "   DeliveryCustomerRepresentative varchar(500) NULL," & vbCrLf &
                "   DeliveryEncodingDate datetime NULL," & vbCrLf &
                "   DeliveryNotes varchar(MAX) NULL," & vbCrLf &
                "   RoutingSheetNumber varchar(10) NULL," & vbCrLf &
                "   IsDraft bit NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransDeliveryLoggerInvoices") = False Then
            pSql = " CREATE TABLE dbo.TransDeliveryLoggerInvoices (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   DeliveryLoggerID uniqueidentifier NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier NULL," & vbCrLf &
                "   Status varchar(50) NULL," & vbCrLf &
                "   DeliveryEncodingDate datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransDeliveryLogs") = False Then
            pSql = " CREATE TABLE dbo.TransDeliveryLogs (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   DeliveryLoggerID uniqueidentifier NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier NULL," & vbCrLf &
                "   Reason varchar(MAX) NULL," & vbCrLf &
                "   DeliveryEncodingDate datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        pChanged += If(vProData.AddColumn("TransDeliveryLogs", "UserID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransItemRequisitions", "CreatedPurchaseOrderID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransItemRequisitions", "BackorderQty", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransItemRequisitions", "ComputedRequestedYearWeek", GetYearWeekExpression("DateRequested"), TriState.UseDefault), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "RoutingSheetNumber", "VARCHAR(10)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "DeliveryStatus", "VARCHAR(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "DontAutoInvoice", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ComputedSalesOrderYearWeek", GetYearWeekExpression("SalesOrderDate"), TriState.UseDefault), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ComputedInvoiceYearWeek", GetYearWeekExpression("InvoiceDate"), TriState.UseDefault), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ComputedPurchaseOrderYearWeek", GetYearWeekExpression("PurchaseOrderDate"), TriState.UseDefault), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ComputedQuotationYearWeek", GetYearWeekExpression("QuotationDate"), TriState.UseDefault), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "CncDisapprovedByID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "CncDisapprovedDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ReprintedDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "DeviceFowstTransactionNumber", "int"), 1, 0)
        If vProData.TableExist("TransVendorPriceHistory") = False Then
            pSql = " CREATE TABLE dbo.TransVendorPriceHistory (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   VendorID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   TransDate datetime NOT NULL," & vbCrLf &
                "   TransID uniqueidentifier NOT NULL," & vbCrLf &
                "   PreviousPrice money NULL," & vbCrLf &
                "   CurrentPrice money NOT NULL," & vbCrLf &
                "   HistoryCreationDate datetime NOT NULL," & vbCrLf &
                "   TransType varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("LibraryFOWSTBinProduct", "Brand", "VARCHAR(250)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "PaymentTermID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "IsInventoryCountItem", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "StockReceivingRankingNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "StockTransferRankingNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "InventoryRankingNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "ItemBasedMaxAllowableDiscount", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "MaxQtyToBeRcvTfer", "int"), 1, 0)

        'DILI ni mao.
        'pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "StockReceivingRanking", "decimal(18, 2) DEFAULT 0"), 1, 0)
        'pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "StockTransferRanking", "decimal(18, 2) DEFAULT 0"), 1, 0)
        'pChanged += If(vProData.AddColumn("LibraryOrganizationPIM", "InventoryCountRanking", "decimal(18, 2) DEFAULT 0"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "ServedDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "ComputedServedDateYearWeek", GetYearWeekExpression("ServedDate"), TriState.UseDefault), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "CancelledByID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "ShippingRef", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "BinID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "ThirdPartyProductNumber", "VARCHAR(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "IsForFollowUp", "bit"), 1, 0) 'gamit ni para madetermine ang mga dapat ifollowup na Price Quote and dapat DRAFT
        pChanged += If(vProData.AddColumn("LibraryOrganizationCustomer", "IsForFollowUp", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationCustomer", "RequireCnCApproval", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationCustomer", "SingleCreditTerm", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationCustomer", "SalesChannelID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "PoReceivedPercent", "decimal(5, 2)"), 1, 0) ' TransInvoice.UpdatePoReceivedPercent()
        pChanged += If(vProData.AddColumn("LibraryPimGroups", "DefaultOtherPaymentID", "varchar(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransExpensesLogDetails", "IsCancelled", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransExpensesLogDetails", "CancelledBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransExpensesLogDetails", "CancelledOn", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "IsActive", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "UpdatedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "UnitMeasure", "VARCHAR(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "UnitQty", "DECIMAL(10, 2)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "UnitPrice", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "Url", "text"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendorPrices", "Note", "text"), 1, 0)

        ' transferred from TransInvoice.vb:CacheInvoicesWithCredit
        pChanged += If(vProData.AddColumn("CacheInvoices", "CashNetOfChange", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Eatigo", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "DeliRush", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "PoodTrip", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Share Treats", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Booky", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "IsImportedToFranchisor", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "FromFranchisee", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "RegionID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "RegionName", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "PotatoCornerGC", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "TotalPotatoCornerGC", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "GrabPickup", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Lazada", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Pickaroo", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Riderko", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "HomeCredit", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "FOWSTCode", "VARCHAR(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "InvoiceNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "ChargeAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "TaxAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "Categories", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "DeliveryMethod", "VARCHAR(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "UserID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "IsSentToLessor", "bit DEFAULT 0"), 1, 0)

        pChanged += If(ChangeCacheReceivings(vProData, vDatabase), 1, 0)

        If vProData.TableExist("LibraryExpensesRouteSchedules") = False Then
            pSql = " CREATE TABLE dbo.LibraryExpensesRouteSchedules (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   EmployeeID uniqueidentifier NULL," & vbCrLf &
                "   LocationID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   Day int NULL," & vbCrLf &
                "   StartTime time(7) NULL," & vbCrLf &
                "   EndTime time(7) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   LastUpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        pChanged += If(ChangeLibraryExpensesRouteSchedules(vProData, vDatabase), 1, 0)
        pChanged += If(ChangeTransKnowledgeManagement(vProData, vDatabase), 1, 0)
        pChanged += If(ChangeSystemDefaults(vProData, vDatabase), 1, 0)
        'pChanged += If(ChangeTransCustomerPaymentOther(vProData, vDatabase), 1, 0)

        If vProData.TableExist("LibraryExpensesRouteSchedulesEmployees") = False Then
            pSql = " CREATE TABLE dbo.LibraryExpensesRouteSchedulesEmployees (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   EmployeeID uniqueidentifier NULL," & vbCrLf &
                "   RouteName varchar(500) NULL," & vbCrLf &
                "   RouteDescription varchar(MAX) NULL," & vbCrLf &
                "   StrictlyFollowSched bit NULL," & vbCrLf &
                "   Frequency varchar(50) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   LastUpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        pChanged += If(vProData.AddColumn("TransVendorPriceHistory", "TagByID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransVendorPriceHistory", "TagDate", "datetime"), 1, 0)
        pChanged += If(ChangeLibraryEcommerceProductQtyChoices(vProData, vDatabase), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "IsOnlinePayment", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPayment", "CancelledByID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPayment", "DateCancelled", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationFinancialDetails", "OnlinePayment", "bit"), 1, 0)

        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "BankID2", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "BankName2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "BankBranchName2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "AccountName2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "AccountNumber2", "varchar(150)"), 1, 0)

        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "BankID2", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "BankName2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "BankBranchName2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "AccountName2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "AccountNumber2", "varchar(150)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "IsOnlinePayment", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "IsGiftCertificate", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositStatus", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositGroupID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositsDepositDate", "datetime2"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositoryBankID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositoryBankBranch", "varchar(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositoryAccountName", "varchar(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositoryAccountNumber", "varchar(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "TimeChecked", "datetime2"), 1, 0)

        pChanged += If(vProData.AddColumn("LibraryFOWSTBinProduct", "LastInventoryCountDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryFOWSTBinProduct", "LastInventoryCountBy", "varchar(500)"), 1, 0)

        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "TimeChecked", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPayment", "TimeChecked", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentCreditCard", "TimeChecked", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentDebitCard", "TimeChecked", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDocumentControl", "UniversalNumberSeries", "bit"), 1, 0) ' if true, only 1 series for this document type regardless of fowst/device
        pChanged += If(vProData.AddColumn("LibraryDocumentControl", "MaxLineItemsPerPage", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDocumentControl", "MaxNoOfSupplementalBlindCopies", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDocumentControl", "UnitPriceSurchargePercent", "decimal(5, 2)"), 1, 0)

        pChanged += If(vProData.AddColumn("CacheInvoices", "FOWSTName", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "StoreType", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "StoreLocation", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "GroupName", "VARCHAR(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "FranchiseGroupID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "SubFOWSTTypeID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "StoreLocationTypeID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "SupercardVoucher", "money"), 1, 0)

        If vProData.TableExist("TransReceivingPaymentOther") = False Then
            pSql = " CREATE TABLE dbo.TransReceivingPaymentOther (" & vbCrLf &
                "   ID uniqueidentifier DEFAULT newid() ROWGUIDCOL NOT NULL," & vbCrLf &
                "   PaymentID uniqueidentifier NOT NULL," & vbCrLf &
                "   PaymentType varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   Amount money NULL," & vbCrLf &
                "   IssuingOrganization varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   CardNumber varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   CardHolderName varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   CardIssuanceDate date NULL," & vbCrLf &
                "   CardExpirationDate date NULL," & vbCrLf &
                "   ApprovalNo varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   RoomNo varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   Remarks varchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   GCashRefNo varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   GrossTaxableAmount money NULL," & vbCrLf &
                "   VatRate money NULL," & vbCrLf &
                "   TaxableAmount money NULL," & vbCrLf &
                "   WitholdingTaxRate money NULL" & vbCrLf &
                " )"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        Dim pFileText = FileText(Common.ServerPath & "Content/OtherPaymentTypes.txt")
        Dim pSplits = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        For Each pSplit In pSplits
            pChanged += If(vProData.AddColumn("TransSalesReturnLedger", "GrossRefundAmount" & pSplit, "money"), 1, 0)
        Next

        If vProData.TableExist("TransPriceUpdateRequest") = False Then
            pSql = " CREATE TABLE dbo.TransPriceUpdateRequest (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   RequestedByID uniqueidentifier NOT NULL," & vbCrLf &
                "   Remarks varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   DateRequested datetime NOT NULL," & vbCrLf &
                "   ApprovedByID uniqueidentifier NULL," & vbCrLf &
                "   ApprovedDate datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("TransInvoice", "ReceivablePurchaseOrderQty", "numeric(18, 6)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryCRMAttachments", "InvoiceRefNo", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryCRMAttachments", "InvoiceDate", "date"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryCRMAttachments", "NoOfLineItems", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryCRMAttachments", "InvoiceAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryCRMAttachments", "Remarks", "varchar(MAX)"), 1, 0)

        If vProData.TableExist("TransReceivingInvoiceRefImages") = False Then
            pSql = " CREATE TABLE dbo.TransReceivingInvoiceRefImages (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ReceivingID uniqueidentifier NULL," & vbCrLf &
                "   CRMAttachmentID uniqueidentifier NULL," & vbCrLf &
                "   SupplierID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   AddedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransReceivingPaymentAppliedCredits") = False Then
            pSql = " CREATE TABLE TransReceivingPaymentAppliedCredits (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   SourcePaymentID uniqueidentifier NOT NULL," & vbCrLf &
                "   DestinationPaymentID uniqueidentifier NOT NULL," & vbCrLf &
                "   Amount money NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        Else
            If vProData.ColumnExist("TransReceivingPaymentAppliedCredits", "SourcePaymentID") = False Then
                If CInt2(vProData.ExecuteScalar("SELECT COUNT(*) FROM TransReceivingPaymentAppliedCredits")) = 0 Then
                    pSql = "DROP TABLE TransReceivingPaymentAppliedCredits"
                    vProData.ExecuteSQL(pSql)
                    pChanged += 1
                End If
            End If
        End If

        If vProData.TableExist("TransCustomerPaymentAppliedCredits") = False Then
            pSql = " CREATE TABLE TransCustomerPaymentAppliedCredits (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   SourcePaymentID uniqueidentifier NOT NULL," & vbCrLf &
                "   DestinationPaymentID uniqueidentifier NOT NULL," & vbCrLf &
                "   Amount money NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryQuickRemarks") = False Then
            pSql = " CREATE TABLE dbo.LibraryQuickRemarks (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Message varchar(MAX) NULL," & vbCrLf &
                "   Tag varchar(50) NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransPlayRoom") = False Then
            pSql = " CREATE TABLE dbo.TransPlayRoom (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   PeopleID uniqueidentifier NULL," & vbCrLf &
                "   TotalNoOfChildren int NULL," & vbCrLf &
                "   TotalNoOfAdult int NULL," & vbCrLf &
                "   PlayDate date NULL," & vbCrLf &
                "   EnterTime time(7) NULL," & vbCrLf &
                "   ExitTime time(7) NULL," & vbCrLf &
                "   TotalDuration varchar(50) NULL," & vbCrLf &
                "   BagLocker varchar(50) NULL," & vbCrLf &
                "   ShoeLocker varchar(50) NULL," & vbCrLf &
                "   Receptionist uniqueidentifier NULL," & vbCrLf &
                "   TotalAmount money NULL," & vbCrLf &
                "   ORNo varchar(50) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransPlayRoomChildren") = False Then
            pSql = " CREATE TABLE dbo.TransPlayRoomChildren (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   PlayRoomID uniqueidentifier NULL," & vbCrLf &
                "   PeopleID uniqueidentifier NULL," & vbCrLf &
                "   PlayDate date NULL," & vbCrLf &
                "   EnterTime time(7) NULL," & vbCrLf &
                "   ExitTime time(7) NULL," & vbCrLf &
                "   ActualExitTime datetime NULL," & vbCrLf &
                "   TotalDuration varchar(50) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransImportBenchmark2") = False Then
            pSql = " CREATE TABLE dbo.TransImportBenchmark2 (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   LastUpdated datetime2(7) NOT NULL," & vbCrLf &
                "   ZipFilename varchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL," & vbCrLf &
                "   ProcessName varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL," & vbCrLf &
                "   RecordCount int NOT NULL," & vbCrLf &
                "   AverageMilliseconds int NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]" & vbCrLf
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryComplianceReminders") = False Then
            pSql = " CREATE TABLE dbo.LibraryComplianceReminders (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ComplianceOrg varchar(50) NULL," & vbCrLf &
                "   OtherComplianceOrg varchar(MAX) NULL," & vbCrLf &
                "   TaxType varchar(500) NULL," & vbCrLf &
                "   FormType varchar(500) NULL," & vbCrLf &
                "   FilingStartDate date NULL," & vbCrLf &
                "   Frequency varchar(50) NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   DueDate varchar(50) NULL," & vbCrLf &
                "   ComplianceTaskTile varchar(500) NULL," & vbCrLf &
                "   MarkAsUnread bit NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPimUpcomingPrices") = False Then
            pSql = " CREATE TABLE dbo.LibraryPimUpcomingPrices (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   PriceLevelID uniqueidentifier NULL," & vbCrLf &
                "   EffectivityDate date NULL," & vbCrLf &
                "   Price money NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryCRMTodoCategories") = False Then
            pSql = " CREATE TABLE dbo.LibraryCRMTodoCategories (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   CategoryName varchar(500) NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   Module varchar(500) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransCRMTodoAssignedEmployee") = False Then
            pSql = " CREATE TABLE dbo.TransCRMTodoAssignedEmployee (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   TodoID uniqueidentifier NULL," & vbCrLf &
                "   EmployeeID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransTwoFactorAuthentication") = False Then
            pSql = " CREATE TABLE dbo.TransTwoFactorAuthentication (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   PeopleID uniqueidentifier NULL," & vbCrLf &
                "   Code varchar(50) NULL," & vbCrLf &
                "   Sent datetime NULL," & vbCrLf &
                "   ExpirationDate datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPriceBulletin") = False Then
            pSql = " CREATE TABLE dbo.LibraryPriceBulletin (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(MAX) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPriceBulletinProducts") = False Then
            pSql = " CREATE TABLE dbo.LibraryPriceBulletinProducts (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   BulletinID uniqueidentifier NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   DisplayName varchar(MAX) NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPurchasingLimits") = False Then
            pSql = " CREATE TABLE dbo.LibraryPurchasingLimits (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   EmployeeID uniqueidentifier NULL," & vbCrLf &
                "   LimitsPerPO money NULL," & vbCrLf &
                "   LimitsPerMonth money NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPimFileAttachments") = False Then
            pSql = " CREATE TABLE dbo.LibraryPimFileAttachments (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   FileName varchar(MAX) NULL," & vbCrLf &
                "   FileLocation varchar(MAX) NULL," & vbCrLf &
                "   FileType varchar(MAX) NULL," & vbCrLf &
                "   UploadedBy uniqueidentifier NULL," & vbCrLf &
                "   DateUploaded datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        pChanged += If(vProData.AddColumn("TransMemo", "Exportable", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemoSalesReturn", "Exportable", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentReceivings", "WithHoldingTaxAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItems", "DateCreated", "datetime2(7) DEFAULT getdate()"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryFowstBin", "BinLongName", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingStock", "PoItemID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingStock", "IsShortLanded", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingStock", "IsOverLanded", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationCustomer", "CustomerRequirePoNum", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPayment", "UsedUnappliedCredit", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPayment", "OtherPaymentAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryComplianceReminders", "ComplianceDescription", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryComplianceReminders", "DateCreated", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryComplianceReminders", "SeenOn", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "InternalRemarks", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryQuickRemarks", "IsInternalRemarks", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransPlayRoom", "ActualTotalDuration", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransPlayRoomChildren", "ActualTotalDuration", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "FowstID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "CustomFowst", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "CreatedOn", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "LastUpdatedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "TodoCategoryID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "TodoNumber", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "ToDoTitle", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "Urgency", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "Importance", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCRM", "TicketStatusConfirmedBy", "varchar(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibrarySecurityModule", "Label", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryKnowledgeManagementArticleTopics", "ProductID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryExpensesRouteSchedules", "ParentID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryExpensesRouteSchedules", "Day", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryExpensesRouteSchedules", "Tag", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryExpensesRouteSchedules", "Frequency", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryExpensesRouteSchedules", "Week", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPayment", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoices", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemo", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemo", "DeviceFowstTransactionNumber", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemoSalesReturn", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemoSalesReturn", "UnitDiscountAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemoSalesReturn", "VATSales", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransMemoSalesReturn", "VATExempt", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransSalesReturnLedger", "ConsolidationDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransSalesReturnLedger", "DeviceFowstTransactionNumber", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifiers", "DateFrom", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifiers", "DateTo", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifiers", "IsPromo", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifiers", "Min", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifiers", "Description", "varchar(max)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevice", "IsDummyDevice", "bit DEFAULT 0"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevice", "FromFranchisee", "bit DEFAULT 0"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevice", "BusinessName", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevice", "BusinessRegisteredName", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevice", "TinDisplay", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevice", "MobizAppVersion", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryOrganizationPriceLevel", "CreditTermID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryFranchiseGroup", "IsFranchisee", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryFranchiseGroup", "IsActive", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "Delivery3PlCompanyID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "IsAttachedOnEarlierSOA", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransRentableAssets", "ReservationWasMadeBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevicePrinter", "PrinterIPAddress", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevicePrinter", "PrinterName", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevicePrinter", "FlushToServer", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevicePrinter", "ConnectionType", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevicePrinter", "SerialNum", "varchar(100)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDevicePrinter", "IsNewSetup", "bit"), 1, 0)

        '// "Freight and Handling" tab
        pChanged += If(vProData.AddColumn("TransInvoice", "ShipperID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "WaybillNumber", "VARCHAR(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "PackageQuantity", "VARCHAR(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "GrossWeight", "VARCHAR(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "GrossVolume", "VARCHAR(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "Freight", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "Handling", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "Arrastre", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "OtherCharges", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "PhNatTeamDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "SoloParentDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "GrossSalesToSC", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "GrossSalesToRegDiner", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "GrossSalesToRegDiner", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "WeightedSCDiscountRate", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "FreightAndHandling", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ShipDate", "DATETIME"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ErrorCorrectAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "ErrorCorrectCount", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "CreatedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoice", "SpaviCacheDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheFc", "CreatedByName", "varchar(max)"), 1, 0)
        'ChangeTransInvoiceSCDiscountExempt(vProData, vDatabase)
        pChanged += If(vProData.AddColumn("TransInvoice", "SCDiscountExempt", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "PrevSCDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "PrevPhNatTeamDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "PrevSoloParentDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "CurPhNatTeamDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "CurSoloParentDiscount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "PrevVATExempt", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "CurTotalVatablePortionOfSCDTransaction", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "DiscountRefund", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "CurSCDiscountExempt", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "PrevSCDiscountExempt", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "PrevSRVAmount", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "CurVoidItemQty", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "CurVoid", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "ZReadCount", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransZReadCache", "DateImported", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItemsModifier", "PremiumPrice", "numeric(18, 6)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCashHandling", "TotalChargeSlip", "decimal(18, 2) DEFAULT 0"), 1, 0)

        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "Shortlanded", "decimal(18, 2)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "Overlanded", "decimal(18, 2)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "UnitMeasure", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "LandedCost", "decimal(18, 2)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "ProductDefect", "decimal(18, 2)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "InventoryCount", "decimal(18, 2)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "InventoryCountdateAndtime", "datetime2(7)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "CountedBy", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "Productname", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "Productname", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "Groupname", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "FranchisegroupID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "FOWSTName", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "FOWSTCode", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInventoryMovement", "ProductNo", "varchar(500)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimMenu", "VDMModelID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimMenu", "IsDisplayAllProducts", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimGroupPims", "VDMMotorNo", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimGroupPims", "VDMBinNo", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimGroupPims", "VDMachineID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimGroupPims", "PlanogramTempID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimGroupPims", "IsUseUOM", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimGroupPims", "IsDoNotIncludeBaseUOM", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryPimCategories", "SortOrder", "int"), 1, 0)

        If vProData.TableExist("LibraryVendingMachineBoards") = False Then
            pSql = " CREATE TABLE dbo.LibraryVendingMachineBoards (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(MAX) NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   NoOfMotors int NULL," & vbCrLf &
                "   CreatedBy uniqueidentifier NULL," & vbCrLf &
                "   DateCreated datetime NULL," & vbCrLf &
                "   ModifiedBy uniqueidentifier NULL," & vbCrLf &
                "   DateModified datetime NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMAds") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMAds (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   TemplateID uniqueidentifier NULL," & vbCrLf &
                "   FileName varchar(MAX) NULL," & vbCrLf &
                "   Orientation varchar(MAX) NULL," & vbCrLf &
                "   OrderNo int NULL," & vbCrLf &
                "   Media varchar(MAX) NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransVDMLogs") = False Then
            pSql = " CREATE TABLE dbo.TransVDMLogs (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FOWSTID uniqueidentifier NULL," & vbCrLf &
                "   DeviceID uniqueidentifier NULL," & vbCrLf &
                "   Page varchar(MAX) NULL," & vbCrLf &
                "   Action varchar(MAX) NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier NULL," & vbCrLf &
                "   LogID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   DateImported datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMAdsAssignedFowst") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMAdsAssignedFowst (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   TemplateID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMAdsTemplate") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMAdsTemplate (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(MAX) NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMProductSelectionGuides") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMProductSelectionGuides (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FileName varchar(MAX) NULL," & vbCrLf &
                "   OrderNo int NULL," & vbCrLf &
                "   Page varchar(MAX) NULL," & vbCrLf &
                "   Media varchar(MAX) NULL," & vbCrLf &
                "   UploadedBy uniqueidentifier NULL," & vbCrLf &
                "   UploadedOn datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVendingMachineAuthorizers") = False Then
            pSql = " CREATE TABLE dbo.LibraryVendingMachineAuthorizers (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   EmployeeID uniqueidentifier NOT NULL," & vbCrLf &
                "   Pin varchar(MAX) NULL," & vbCrLf &
                "   UploadedByID uniqueidentifier NULL," & vbCrLf &
                "   UploadedOn datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVendingMachineAuthorizersAccessRights") = False Then
            pSql = " CREATE TABLE dbo.LibraryVendingMachineAuthorizersAccessRights (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   VDMAuthorizerID uniqueidentifier NOT NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMGuides") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMGuides (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   GuideID uniqueidentifier NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   Filename varchar(MAX) NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   OrderNo int NULL," & vbCrLf &
                "   OnScreenTime int NULL," & vbCrLf &
                "   IsDisplayInfo bit NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMGuidesAssignedFowst") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMGuidesAssignedFowst (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   TemplateID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryVDMGuidesTemplate") = False Then
            pSql = " CREATE TABLE dbo.LibraryVDMGuidesTemplate (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(MAX) NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryMessages") = False Then
            pSql = " CREATE TABLE dbo.LibraryMessages (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Title varchar(MAX) NULL," & vbCrLf &
                "   Message varchar(MAX) NULL," & vbCrLf &
                "   CreatedBy uniqueidentifier NULL," & vbCrLf &
                "   CreatedOn datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryItemModifierAssignedCustomer") = False Then
            pSql = " CREATE TABLE dbo.LibraryItemModifierAssignedCustomer (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ItemModifierID uniqueidentifier NULL," & vbCrLf &
                "   CustomerOrgID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryItemModifierAssignedFowst") = False Then
            pSql = " CREATE TABLE dbo.LibraryItemModifierAssignedFowst (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ItemModifierID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryItemModifierAssignedFowst") = False Then
            pSql = " CREATE TABLE dbo.LibraryItemModifierAssignedFowst (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ItemModifierID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryItemModifierSorting") = False Then
            pSql = " CREATE TABLE dbo.LibraryItemModifierSorting (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   CategoryName varchar(MAX) NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   ItemModifierID uniqueidentifier NULL," & vbCrLf &
                "   OrderNo int NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransDeviceAvailability") = False Then
            pSql = " CREATE TABLE dbo.TransDeviceAvailability (" & vbCrLf &
                "   ID uniqueidentifier DEFAULT newid() ROWGUIDCOL NOT NULL," & vbCrLf &
                "   DeviceID uniqueidentifier NOT NULL," & vbCrLf &
                "   TransactionDate datetime2 NOT NULL," & vbCrLf &
                "   IsAvailable bit DEFAULT 1 NOT NULL," & vbCrLf &
                "   ReasonForInability varchar(500) NULL" & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("TransDeviceAvailability", "DeviceName", "varchar(500)"), 1, 0)

        If vProData.TableExist("LibraryCustomDiscounts") = False Then
            pSql = " CREATE TABLE dbo.LibraryCustomDiscounts (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   Name varchar(MAX) NULL," & vbCrLf &
                "   Description varchar(MAX) NULL," & vbCrLf &
                "   MinPurchaseAmount float NULL," & vbCrLf &
                "   DiscountAmount float NULL," & vbCrLf &
                "   DiscountPercentage float NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   IsApplyToAllFowst bit NULL," & vbCrLf &
                "   AvailabilityDateFrom date NULL," & vbCrLf &
                "   AvailabilityDateTo date NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryCustomDiscountsFowst") = False Then
            pSql = " CREATE TABLE dbo.LibraryCustomDiscountsFowst (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   CustomDiscountID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryItemModifierInventoryAffectingFowst") = False Then
            pSql = " CREATE TABLE dbo.LibraryItemModifierInventoryAffectingFowst (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   InventoryAffectingID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransInvoiceDeliveryDetails") = False Then
            pSql = " CREATE TABLE dbo.TransInvoiceDeliveryDetails (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier NULL," & vbCrLf &
                "   PeopleID uniqueidentifier NULL," & vbCrLf &
                "   Designation varchar(50) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("TransInvoiceDeliveryDetails", "IsActive", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceDeliveryDetails", "DeliveryLoggerID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifierInventoryAffecting", "PremiumPrice", "numeric(18, 6)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifierInventoryAffecting", "OrderNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCashHandling", "NoOfOneCents", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCashHandlingCashCount", "NoOfOneCents", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifierInventoryAffecting", "OrderNo", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifierNonInventoryAffecting", "NoOfCharacters", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryItemModifierNonInventoryAffecting", "OthersDescription", "varchar(MAX)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendingMachineBoards", "NoOfPhysicalSelectionButtons", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendingMachineBoards", "NoOfPhysicalNavigationButtons", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendingMachineBoards", "NoOfMotorActuators", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendingMachineBoards", "NoOfBins", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendingMachineBoards", "Type", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVendingMachineBoards", "NoOfDirectConnMotors", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryVDMGuides", "Media", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TransVDMLogs", "UserID", "uniqueidentifier"), 1, 0)

        If vProData.TableExist("LibraryOrganizationCustomerCreditTerms") = False Then
            pSql = " CREATE TABLE dbo.LibraryOrganizationCustomerCreditTerms (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   OrganizationID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   SalesRepID uniqueidentifier NULL," & vbCrLf &
                "   PriceLevelID uniqueidentifier NULL," & vbCrLf &
                "   CreditTermsID uniqueidentifier NULL," & vbCrLf &
                "   CreditLimit float NULL," & vbCrLf &
                "   CreditBegBalance float NULL," & vbCrLf &
                "   CreditBalanceAsOf date NULL," & vbCrLf &
                "   ThirdPartyBegBalance money NULL," & vbCrLf &
                "   ThirdPartyBegBalanceAsOf datetime NULL," & vbCrLf &
                "   UnlimitedCredit bit NULL," & vbCrLf &
                "   CustomerRequirePoNum bit NULL," & vbCrLf &
                "   RequireCnCApproval bit NULL," & vbCrLf &
                "   EncodedBy uniqueidentifier NULL," & vbCrLf &
                "   EncodedDate datetime NULL," & vbCrLf &
                "   ApprovedBy uniqueidentifier NULL," & vbCrLf &
                "   ApprovedDate datetime NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransDateImported") = False Then
            pSql = " CREATE TABLE dbo.TransDateImported (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   DeviceID uniqueidentifier NOT NULL," & vbCrLf &
                "   TableName varchar(100) NOT NULL," & vbCrLf &
                "   RowID uniqueidentifier NOT NULL," & vbCrLf &
                "   RowLastUpdated datetime NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TriggerExist("TransInvoiceItems_update", "TransInvoiceItems") = False Then
            pSql = " CREATE TRIGGER TransInvoiceItems_update ON TransInvoiceItems" & vbCrLf &
                " WITH EXECUTE AS CALLER" & vbCrLf &
                " FOR UPDATE" & vbCrLf &
                " AS" & vbCrLf &
                " IF EXISTS (" & vbCrLf &
                "    SELECT *" & vbCrLf &
                "    FROM INSERTED I" & vbCrLf &
                "    JOIN DELETED D ON D.id = I.id " & vbCrLf &
                "    AND D.InvoiceID <> I.InvoiceID" & vbCrLf &
                " )" & vbCrLf &
                " RAISERROR('Changes to InvoiceID in table TransInvoiceItems are NOT allowed. ref: sqlFB7P85LQQW!', 16,1);"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryRegionalGroupSettings") = False Then
            pSql = " CREATE TABLE dbo.LibraryRegionalGroupSettings (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   RegionName varchar(500) NULL," & vbCrLf &
                "   IsActive bit NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryRegionalGroupSettingsDetails") = False Then
            pSql = " CREATE TABLE dbo.LibraryRegionalGroupSettingsDetails (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   RegionalGroupID uniqueidentifier NULL," & vbCrLf &
                "   GroupID uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TenantMonitoringLogs") = False Then
            pSql = " CREATE TABLE dbo.TenantMonitoringLogs (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FOWSTID uniqueidentifier NULL," & vbCrLf &
                "   SalesDate datetime2(7) NULL," & vbCrLf &
                "   LastTransactionID uniqueidentifier NULL," & vbCrLf &
                "   Mall varchar(500) NULL," & vbCrLf &
                "   Reported bit NULL," & vbCrLf &
                "   ReportDate datetime2(7) NULL," & vbCrLf &
                "   DateImported datetime2(7) NULL," & vbCrLf &
                "   BatchNo int NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   UserID uniqueidentifier NULL," & vbCrLf &
                "   FileStatus varchar(50) NULL," & vbCrLf &
                "   ErrorMessage varchar(MAX) NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibrarySecurityModuleMainPerRole") = False Then
            pSql = " CREATE TABLE dbo.LibrarySecurityModuleMainPerRole (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ModuleMainID uniqueidentifier NULL," & vbCrLf &
                "   RoleID uniqueidentifier NULL," & vbCrLf &
                "   Status varchar(50) NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryPimMenuButtonAssignedStores") = False Then
            pSql = " CREATE TABLE dbo.LibraryPimMenuButtonAssignedStores (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   HotButtonID uniqueidentifier NULL," & vbCrLf &
                "   FowstID uniqueidentifier NULL," & vbCrLf &
                "   IsGroup bit NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("CacheInvoiceModifiers") = False Then
            pSql = " CREATE TABLE dbo.CacheInvoiceModifiers (" & vbCrLf &
                "   InvoiceDate date NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductNo varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL," & vbCrLf &
                "   ProductName varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   ProductBrand varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   ProductQuantity money NULL," & vbCrLf &
                "   ProductAmount money NULL," & vbCrLf &
                "   ModifierProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   ModifierProductNumber varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   ModifierProductName varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   ModifierBrand varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   ModifierQuantity money NULL," & vbCrLf &
                "   ModifierAmount money NULL," & vbCrLf &
                "   ModifierUnitMeasure varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL," & vbCrLf &
                "   Flavor varchar(999) COLLATE SQL_Latin1_General_CP1_CI_AS NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "FowstID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "ItemModifierID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "FranchiseGroupID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "FowstName", "VARCHAR(1000)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "ItemModifierName", "VARCHAR(1000)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "GroupName", "VARCHAR(1000)"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "IsPromo", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("CacheInvoiceModifiers", "InvoiceCount", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "DeviceID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "EndOfDayCounter", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "Filename", "varchar(50)"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "FirstPrintedDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "FirstPrintedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "NextPrintedDate", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TenantMonitoringLogs", "NextPrintedBy", "uniqueidentifier"), 1, 0)

        If vProData.TableExist("LibraryOrganizationCustomerSalesReps") = False Then
            pSql = " CREATE TABLE dbo.LibraryOrganizationCustomerSalesReps (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   OrganizationID uniqueidentifier NOT NULL," & vbCrLf &
                "   PeopleID uniqueidentifier NOT NULL," & vbCrLf &
                "   UserID uniqueidentifier NOT NULL," & vbCrLf &
                "   LastUpdated datetime NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)

            pSql = " CREATE UNIQUE NONCLUSTERED INDEX LibraryOrganizationCustomerSalesReps_uq ON dbo.LibraryOrganizationCustomerSalesReps" & vbCrLf &
                "   (OrganizationID, PeopleID)" & vbCrLf &
                " WITH (" & vbCrLf &
                "   PAD_INDEX = OFF," & vbCrLf &
                "   IGNORE_DUP_KEY = OFF," & vbCrLf &
                "   DROP_EXISTING = OFF," & vbCrLf &
                "   STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "   SORT_IN_TEMPDB = OFF," & vbCrLf &
                "   ONLINE = OFF," & vbCrLf &
                "   ALLOW_ROW_LOCKS = ON," & vbCrLf &
                "   ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        pChanged += If(vProData.AddColumn("TransImports", "ID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransImports", "OrigDateReceived", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransImports", "FileSize", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransImports", "ServerIdentity", "VARCHAR(30)"), 1, 0)

        If vProData.TableExist("LibraryPimSalesCaching") = False Then
            pSql = " CREATE TABLE LibraryPimSalesCaching (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   Interval int NOT NULL," & vbCrLf &
                "   UpdatedBy uniqueidentifier NOT NULL," & vbCrLf &
                "   LastUpdated datetime NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransPimSalesCaching") = False Then
            pSql = " CREATE TABLE TransPimSalesCaching (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   FowstID uniqueidentifier NOT NULL," & vbCrLf &
                "   DateIntervalStart datetime NOT NULL," & vbCrLf &
                "   DateIntervalEnd datetime NOT NULL," & vbCrLf &
                "   Qty numeric(18, 6) NOT NULL," & vbCrLf &
                "   Interval int NOT NULL," & vbCrLf &
                "   LastUpdated datetime NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransInventoryCountItemPerUom") = False Then
            pSql = " CREATE TABLE TransInventoryCountItemPerUom (" & vbCrLf &
                "   ID uniqueidentifier NOT NULL," & vbCrLf &
                "   FOWSTID uniqueidentifier NULL," & vbCrLf &
                "   FOWSTCode varchar(MAX) NULL," & vbCrLf &
                "   FOWSTName varchar(MAX) NULL," & vbCrLf &
                "   CountDate date NULL," & vbCrLf &
                "   ProductID uniqueidentifier NULL," & vbCrLf &
                "   ProductNo varchar(MAX) NULL," & vbCrLf &
                "   ProductName varchar(MAX) NULL," & vbCrLf &
                "   UnitMeasureID uniqueidentifier NULL," & vbCrLf &
                "   UnitMeasureName varchar(MAX) NULL," & vbCrLf &
                "   Qty decimal(18, 0) NOT NULL," & vbCrLf &
                "   Count decimal(18, 0) NOT NULL," & vbCrLf &
                "   UomState varchar(50) NOT NULL," & vbCrLf &
                "   Status varchar(50) NOT NULL," & vbCrLf &
                "   CountedByUser uniqueidentifier NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   DateImported datetime NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("TransPimSalesCaching", "SalesQty", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransPimSalesCaching", "BomQty", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransPimSalesCaching", "ItemModifierQty", "money"), 1, 0)

        pChanged += If(vProData.AddColumn("TransInventoryLedger", "SalesQty", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInventoryLedger", "BomQty", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInventoryLedger", "ItemModifierQty", "money"), 1, 0)

        pChanged += If(vProData.AddColumn("TransCustomerPayment", "CashDepositedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPayment", "CreditCardVat", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransReceivingPaymentChecks", "DepositedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentChecks", "DepositedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentCreditCard", "DepositedBy", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentDebitCard", "DepositedBy", "uniqueidentifier"), 1, 0)

        If vProData.TableExist("CacheInvoiceFlavors") Then
            pChanged += If(vProData.AddColumn("CacheInvoiceFlavors", "SalesValue", "money"), 1, 0)
        End If
        If vProData.TableExist("CacheInvoiceFinishedProducts") Then
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "SalesValue", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "FOWSTCode", "varchar(50)"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "StoreLocation", "VARCHAR(500)"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "Category", "VARCHAR(500)"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "StoreLocationTypeID", "uniqueidentifier"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "FromFranchisee", "bit"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoiceFinishedProducts", "IsImportedToFranchisor", "bit"), 1, 0)
        End If
        If vProData.TableExist("CacheInvoicesDaily") Then
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "WalkInSales", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "DeliverySales", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "TransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "WalkInTransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "DeliveryTransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "SupercardVoucher", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "ChargeAmount", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesDaily", "TaxAmount", "money"), 1, 0)
        End If
        If vProData.TableExist("CacheInvoicesHourly") Then
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "WalkInSales", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "DeliverySales", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "TransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "WalkInTransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "DeliveryTransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "InvoiceHour", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "ChargeAmount", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesHourly", "TaxAmount", "money"), 1, 0)
        End If
        If vProData.TableExist("CacheDailySalesPerSKUReport") Then
            pChanged += If(vProData.AddColumn("CacheDailySalesPerSKUReport", "UnitMeasure", "VARCHAR(50)"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheDailySalesPerSKUReport", "Qty", "money"), 1, 0)
        End If
        If vProData.TableExist("CacheInvoicesMonthly") Then
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "InvoiceMonth", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "InvoiceYear", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "WalkInSales", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "DeliverySales", "money"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "TransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "WalkInTransactionCount", "int"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheInvoicesMonthly", "DeliveryTransactionCount", "int"), 1, 0)
        End If

        pChanged += If(vProData.AddColumn("TransReceiving", "InvoiceID", "uniqueidentifier"), 1, 0) ' used for vending machine
        pChanged += If(vProData.AddColumn("TransReceiving", "IsRefund", "bit"), 1, 0) ' used for vending machine
        pChanged += If(vProData.AddColumn("TransReceiving", "DeviceFowstTransactionNumber", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFowst", "FlushToServer", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFowst", "IsFromDevice", "bit"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFowst", "ZReadCount", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFowst", "VoidNumber", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFowst", "DeviceFowstTransactionNumber", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("LibraryDeviceFowst", "PTUDateIssued", "date"), 1, 0)

        If vProData.TableExist("LibraryProductMapProducts") = False Then
            pSql = " CREATE TABLE LibraryProductMapProducts (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   ProductMapID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductNumber varchar(50) NOT NULL," & vbCrLf &
                "   ProductName varchar(100) NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   LastUpdated datetime2(7) NOT NULL," & vbCrLf &
                "   UserID uniqueidentifier NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("LibraryProductMapFowsts") = False Then
            pSql = " CREATE TABLE LibraryProductMapFowsts (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   FowstID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductMapID uniqueidentifier NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)," & vbCrLf &
                "   CONSTRAINT LibraryProductMapFowsts_uq UNIQUE (FowstID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransUnflushedCounts") = False Then
            pSql = " CREATE TABLE TransUnflushedCounts (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   FlushingDate datetime2(7) NOT NULL," & vbCrLf &
                "   FowstID uniqueidentifier NOT NULL," & vbCrLf &
                "   DeviceID uniqueidentifier NOT NULL," & vbCrLf &
                "   TransactionDate datetime2(7) NOT NULL," & vbCrLf &
                "   UnflushedCount int NOT NULL," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        If vProData.TableExist("CacheSalesTableInvoice") = False Then
            pSql = " CREATE TABLE CacheSalesTableInvoice (" & vbCrLf &
                "   id uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   store_code varchar(10)," & vbCrLf &
                "   tran_no int," & vbCrLf &
                "   tx_date date," & vbCrLf &
                "   tx_time datetime," & vbCrLf &
                "   time_closed datetime," & vbCrLf &
                "   tx_type varchar(50)," & vbCrLf &
                "   customer_id uniqueidentifier," & vbCrLf &
                "   entered_by varchar(50)," & vbCrLf &
                "   cashier_name varchar(50)," & vbCrLf &
                "   status varchar(50)," & vbCrLf &
                "   cashier uniqueidentifier," & vbCrLf &
                "   invoice_no int," & vbCrLf &
                "   loyalty_card_pan varchar(100)," & vbCrLf &
                "   loyalty_points money," & vbCrLf &
                "   vatable_sales money," & vbCrLf &
                "   non_vatable_sales money," & vbCrLf &
                "   vat_exempt_sales money," & vbCrLf &
                "   vat money," & vbCrLf &
                "   sc money," & vbCrLf &
                "   sr_count int," & vbCrLf &
                "   discount money," & vbCrLf &
                "   amount_due money," & vbCrLf &
                "   sub_tx_type varchar(100)," & vbCrLf &
                "   amount_gross money," & vbCrLf &
                "   amount_pwd_discount money," & vbCrLf &
                "   amount_senior_citizen_discount money," & vbCrLf &
                "   terminal varchar(255)," & vbCrLf &
                "   is_from varchar(255)," & vbCrLf &
                "   date_ingested_frm_raw datetime," & vbCrLf &
                "   timedispatched datetime," & vbCrLf &
                "   DateSent datetime," & vbCrLf &
                "   PRIMARY KEY CLUSTERED (ID)" & vbCrLf &
                "     WITH (" & vbCrLf &
                "       PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("CacheSalesTableInvoice", "DateError", "datetime"), 1, 0)

        If vProData.TableExist("CacheSalesTableOrder") = False Then
            pSql = " CREATE TABLE CacheSalesTableOrder (" & vbCrLf &
                "   id varchar(100) NOT NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier," & vbCrLf &
                "   adj_net money," & vbCrLf &
                "   adj_sc money," & vbCrLf &
                "   adj_tax money," & vbCrLf &
                "   class_name varchar(50)," & vbCrLf &
                "   discount money," & vbCrLf &
                "   item_id varchar(50)," & vbCrLf &
                "   date_ingested_frm_raw datetime," & vbCrLf &
                "   disc_perc money," & vbCrLf &
                "   is_from varchar(100)," & vbCrLf &
                "   item_name varchar(100)," & vbCrLf &
                "   [key] varchar(100)," & vbCrLf &
                "   loyalty_card_pan varchar(100)," & vbCrLf &
                "   loyalty_point int," & vbCrLf &
                "   net money," & vbCrLf &
                "   original_price money," & vbCrLf &
                "   pwd_discount_amount money," & vbCrLf &
                "   quantity money," & vbCrLf &
                "   senior_discount_amount  money," & vbCrLf &
                "   status varchar(100)," & vbCrLf &
                "   store_code varchar(10)," & vbCrLf &
                "   tax money," & vbCrLf &
                "   tax_exempt money," & vbCrLf &
                "   terminal varchar(255)," & vbCrLf &
                "   tran_no int," & vbCrLf &
                "   tx_date date," & vbCrLf &
                "   tx_time datetime," & vbCrLf &
                "   tx_type varchar(50)," & vbCrLf &
                "   updated_by varchar(255)," & vbCrLf &
                "   vat_exempt money," & vbCrLf &
                "   vatable_sales money," & vbCrLf &
                "   DateSent datetime" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("CacheSalesTablePayments") = False Then
            pSql = " CREATE TABLE CacheSalesTablePayments (" & vbCrLf &
                "   id uniqueidentifier NOT NULL," & vbCrLf &
                "   store_code varchar(10)," & vbCrLf &
                "   tx_date date," & vbCrLf &
                "   tran_no int," & vbCrLf &
                "   tender_id varchar(50)," & vbCrLf &
                "   tender_name varchar(50)," & vbCrLf &
                "   number varchar(100)," & vbCrLf &
                "   reference varchar(200)," & vbCrLf &
                "   tender_type varchar(50)," & vbCrLf &
                "   amount money," & vbCrLf &
                "   status varchar(50) NULL," & vbCrLf &
                "   tip money NOT NULL," & vbCrLf &
                "   senior_citizen_count int NULL," & vbCrLf &
                "   authorize_id varchar(50) NULL," & vbCrLf &
                "   authorize_name varchar(50)," & vbCrLf &
                "   tin varchar(50)," & vbCrLf &
                "   is_from varchar(255)," & vbCrLf &
                "   terminal varchar(255)," & vbCrLf &
                "   date_ingested_frm_raw datetime," & vbCrLf &
                "   DateSent datetime" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("CacheSalesTableFinancial") = False Then
            pSql = " CREATE TABLE CacheSalesTableFinancial (" & vbCrLf &
                "   InvoiceID uniqueidentifier NOT NULL," & vbCrLf &
                "   store_code varchar(10)," & vbCrLf &
                "   tx_date date," & vbCrLf &
                "   gross money," & vbCrLf &
                "   discount money," & vbCrLf &
                "   sc money," & vbCrLf &
                "   tax money," & vbCrLf &
                "   adj_net money," & vbCrLf &
                "   adj_tax money," & vbCrLf &
                "   take_out money," & vbCrLf &
                "   delivery money," & vbCrLf &
                "   refund money," & vbCrLf &
                "   cancelled money," & vbCrLf &
                "   void money," & vbCrLf &
                "   tx_count int," & vbCrLf &
                "   cash money," & vbCrLf &
                "   debit money," & vbCrLf &
                "   credit money," & vbCrLf &
                "   certificate money," & vbCrLf &
                "   [check] money," & vbCrLf &
                "   vatable_sales money," & vbCrLf &
                "   non_vatable_sales money," & vbCrLf &
                "   vat_exempt money," & vbCrLf &
                "   sr_count int," & vbCrLf &
                "   sr_discount money," & vbCrLf &
                "   eod_time datetime," & vbCrLf &
                "   upload_date datetime," & vbCrLf &
                "   is_from varchar(255)," & vbCrLf &
                "   terminal varchar(255)," & vbCrLf &
                "   DateSent datetime" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("TransInvoiceItemsVoided") = False Then
            pSql = " CREATE TABLE TransInvoiceItemsVoided (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   InvoiceID uniqueidentifier NOT NULL," & vbCrLf &
                "   ProductID uniqueidentifier NOT NULL," & vbCrLf &
                "   Quantity numeric(18, 6) NOT NULL," & vbCrLf &
                "   ActualQuantity numeric(18, 6) NOT NULL," & vbCrLf &
                "   UnitMeasure varchar(50) NOT NULL," & vbCrLf &
                "   DateTimeRemoved datetime NOT NULL," & vbCrLf &
                "   RemovedByPeopleID uniqueidentifier NOT NULL," & vbCrLf &
                "   Reason varchar(max) NULL" & vbCrLf &
                " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If
        pChanged += If(vProData.AddColumn("TransInvoiceItemsVoided", "UnitPrice", "money"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItemsVoided", "VoidNumber", "int"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItemsVoided", "DateImported", "datetime"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItemsVoided", "DeviceID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItemsVoided", "FowstID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransInvoiceItemsVoided", "CashierID", "uniqueidentifier"), 1, 0)
        pChanged += If(vProData.AddColumn("TransCustomerPaymentSRV", "DateImported", "datetime"), 1, 0)

        If vProData.TableExist("TransESalesYearly") = False Then
            pSql = " CREATE TABLE TransESalesYearly (" & vbCrLf &
                "   ID uniqueidentifier ROWGUIDCOL NOT NULL," & vbCrLf &
                "   UserID uniqueidentifier NOT NULL," & vbCrLf &
                "   LastUpdated datetime NULL," & vbCrLf &
                "   ZReadYear int NOT NULL," & vbCrLf &
                "   MachineNumber varchar(100) NOT NULL," & vbCrLf &
                "   VatRate money NOT NULL," & vbCrLf &
                "   OutputTaxRate money NOT NULL," & vbCrLf &
                "   PreviousGrossSales money NOT NULL," & vbCrLf &
                "   PreviousVatSales money NOT NULL," & vbCrLf &
                "   TotalGrossSales money NOT NULL," & vbCrLf &
                "   TotalVatSales money NOT NULL," & vbCrLf
            For pMonth As Integer = 1 To 12
                pSql &= " InvoiceNumber" & Format(pMonth, "00") & " int NOT NULL," & vbCrLf
                pSql &= " GrossSales" & Format(pMonth, "00") & " money NOT NULL," & vbCrLf
                pSql &= " AccumulatedGrossSales" & Format(pMonth, "00") & " money NOT NULL," & vbCrLf
                pSql &= " VatSales" & Format(pMonth, "00") & " money NOT NULL," & vbCrLf
                pSql &= " AccumulatedVatSales" & Format(pMonth, "00") & " money NOT NULL," & vbCrLf
                pSql &= " OutputTax" & Format(pMonth, "00") & " money NOT NULL," & vbCrLf
            Next
            pSql &= " )" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)

            pSql = " CREATE UNIQUE NONCLUSTERED INDEX TransESalesYearly_uq ON dbo.TransESalesYearly" & vbCrLf &
                "   (ZReadYear, MachineNumber)" & vbCrLf &
                " WITH (" & vbCrLf &
                "   PAD_INDEX = OFF," & vbCrLf &
                "   IGNORE_DUP_KEY = OFF," & vbCrLf &
                "   DROP_EXISTING = OFF," & vbCrLf &
                "   STATISTICS_NORECOMPUTE = OFF," & vbCrLf &
                "   SORT_IN_TEMPDB = OFF," & vbCrLf &
                "   ONLINE = OFF," & vbCrLf &
                "   ALLOW_ROW_LOCKS = ON," & vbCrLf &
                "   ALLOW_PAGE_LOCKS = ON)" & vbCrLf &
                " ON [PRIMARY]"
            vProData.ExecuteSQL(pSql)
            pChanged += 1
        End If

        If vProData.TableExist("CacheDailySalesPerSKUReport") Then
            pChanged += If(vProData.AddColumn("CacheDailySalesPerSKUReport", "ProductID", "uniqueidentifier"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheDailySalesPerSKUReport", "ProductNo", "VARCHAR(100)"), 1, 0)
            pChanged += If(vProData.AddColumn("CacheDailySalesPerSKUReport", "CategoryName", "VARCHAR(MAX)"), 1, 0)
        End If

        If vPublicTablesAdded > 0 Then
            With New Synonyms()
                .Prefix = "Mobiz"
                Try
                    .SourceDatabase = "Mobiz"
                    .ProData = vProData
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try
            End With
        End If

        Return pChanged

    End Function

    Private Function ChangeLibraryEcommerceProductQtyChoices(vProData As ProData, vDatabase As String) As Boolean

        If vProData.AddColumn("LibraryEcommerceProductQtyChoices", "QtyChoices", "numeric(20, 10)") Then
            Return True
        End If

        Dim pColumnInfo = vProData.ColumnInformation("LibraryEcommerceProductQtyChoices", "QtyChoices")
        If CStr2(pColumnInfo("TypeName")).ToLower() = "numeric" Then
            Return False
        End If
        Dim pSql = " SELECT TOP 1 QtyChoices" & vbCrLf &
            " FROM LibraryEcommerceProductQtyChoices" & vbCrLf &
            " WHERE ISNUMERIC(QtyChoices) = 0"
        Dim pDrInvalidValue = vProData.ExecuteRow(pSql)
        If pDrInvalidValue IsNot Nothing Then
            Throw New ApplicationException("vbFFHBM2BY4Z! " & vDatabase & ".LibraryEcommerceProductQtyChoices.QtyChoices has a non-numeric value: " & pDrInvalidValue("QtyChoices"))
        End If

        vProData.BeginTransaction()
        pSql = " EXEC sp_rename N'dbo.LibraryEcommerceProductQtyChoices.QtyChoices', N'QtyChoices_old', 'COLUMN'"
        vProData.ExecuteSQL(pSql)

        vProData.AddColumn("LibraryEcommerceProductQtyChoices", "QtyChoices", "numeric(20, 10)")

        pSql = "UPDATE LibraryEcommerceProductQtyChoices" & vbCrLf &
            " SET QtyChoices = QtyChoices_old"
        vProData.ExecuteSQL(pSql)

        pSql = " ALTER TABLE dbo.LibraryEcommerceProductQtyChoices" & vbCrLf &
            " DROP COLUMN QtyChoices_old"
        vProData.ExecuteSQL(pSql)
        vProData.CommitTransaction()

        Return True

    End Function

    Private Function ChangeCacheReceivings(vProData As ProData, vDatabase As String) As Boolean

        If vProData.AddColumn("CacheReceivings", "Remarks", "varchar(MAX)") Then
            Return True
        End If

        Dim pColumnInfo = vProData.ColumnInformation("CacheReceivings", "Remarks")
        If CStr2(pColumnInfo("TypeName")).ToLower() = "varchar" And CDec2(pColumnInfo("Length")) = -1 Then
            Return False
        End If
        Dim pSql = " SELECT TOP 1 Remarks" & vbCrLf &
            " FROM CacheReceivings" & vbCrLf &
            " WHERE ISNUMERIC(QtyChoices) = 0"
        'Dim pDrInvalidValue = vProData.ExecuteRow(pSql)
        'If pDrInvalidValue IsNot Nothing Then
        '    Throw New ApplicationException("vbFFHBM2BY4Z! " & vDatabase & ".CacheReceivings.Remarks has a non-numeric value: " & pDrInvalidValue("Remarks"))
        'End If

        vProData.BeginTransaction()
        pSql = " EXEC sp_rename N'dbo.CacheReceivings.Remarks', N'Remarks_old', 'COLUMN'"
        vProData.ExecuteSQL(pSql)

        vProData.AddColumn("CacheReceivings", "Remarks", "varchar(MAX)")

        pSql = "UPDATE CacheReceivings" & vbCrLf &
            " SET Remarks = Remarks_old"
        vProData.ExecuteSQL(pSql)

        pSql = " ALTER TABLE dbo.CacheReceivings" & vbCrLf &
            " DROP COLUMN Remarks_old"
        vProData.ExecuteSQL(pSql)
        vProData.CommitTransaction()

        Return True

    End Function

    Private Function ChangeLibraryExpensesRouteSchedules(vProData As ProData, vDatabase As String) As Boolean

        If vProData.AddColumn("LibraryExpensesRouteSchedules", "Day", "int") Then
            Return True
        End If

        Dim pColumnInfo = vProData.ColumnInformation("LibraryExpensesRouteSchedules", "Day")
        If CStr2(pColumnInfo("TypeName")).ToLower() = "int" Then
            Return False
        End If
        Dim pSql As String

        vProData.BeginTransaction()
        pSql = " EXEC sp_rename N'dbo.LibraryExpensesRouteSchedules.Day', N'Day_old', 'COLUMN'"
        vProData.ExecuteSQL(pSql)

        vProData.AddColumn("LibraryExpensesRouteSchedules", "Day", "int")

        pSql = " ALTER TABLE dbo.LibraryExpensesRouteSchedules" & vbCrLf &
            " DROP COLUMN Day_old"
        vProData.ExecuteSQL(pSql)
        vProData.CommitTransaction()

        Return True

    End Function

    Private Function ChangeTransKnowledgeManagement(vProData As ProData, vDatabase As String) As Boolean

        If vProData.AddColumn("TransKnowledgeManagement", "Steps", "varchar(MAX)") Then
            Return True
        End If

        Dim pColumnInfo = vProData.ColumnInformation("TransKnowledgeManagement", "Steps")
        If CStr2(pColumnInfo("TypeName")).ToLower() = "varchar" And CDec2(pColumnInfo("Length")) = -1 Then
            Return False
        End If
        Dim pSql As String

        vProData.BeginTransaction()
        pSql = " EXEC sp_rename N'dbo.TransKnowledgeManagement.Steps', N'Steps_old', 'COLUMN'"
        vProData.ExecuteSQL(pSql)

        vProData.AddColumn("TransKnowledgeManagement", "Steps", "varchar(MAX)")

        pSql = "UPDATE TransKnowledgeManagement" & vbCrLf &
            " SET Steps = Steps_old"
        vProData.ExecuteSQL(pSql)

        pSql = " ALTER TABLE dbo.TransKnowledgeManagement" & vbCrLf &
            " DROP COLUMN Steps_old"
        vProData.ExecuteSQL(pSql)
        vProData.CommitTransaction()

        Return True

    End Function

    Private Function ChangeLibraryVouchers(vProData As ProData, vDatabase As String) As Boolean

        If vProData.AddColumn("LibraryVouchers", "ProductCode", "varchar(MAX)") Then
            Return True
        End If

        Dim pColumnInfo = vProData.ColumnInformation("LibraryVouchers", "ProductCode")
        If CStr2(pColumnInfo("TypeName")).ToLower() = "varchar" And CDec2(pColumnInfo("Length")) = -1 Then
            Return False
        End If
        Dim pSql As String

        vProData.BeginTransaction()
        pSql = " EXEC sp_rename N'dbo.LibraryVouchers.ProductCode', N'ProductCode_old', 'COLUMN'"
        vProData.ExecuteSQL(pSql)

        vProData.AddColumn("LibraryVouchers", "ProductCode", "varchar(MAX)")

        pSql = "UPDATE LibraryVouchers" & vbCrLf &
            " SET ProductCode = ProductCode_old"
        vProData.ExecuteSQL(pSql)

        pSql = " ALTER TABLE dbo.LibraryVouchers" & vbCrLf &
            " DROP COLUMN ProductCode_old"
        vProData.ExecuteSQL(pSql)
        vProData.CommitTransaction()

        Return True

    End Function

    Private Function ChangeLibraryOrganizationContactWebsite(vProData As ProData, vDatabase As String) As Boolean

        Try
            If vProData.AddColumn("LibraryOrganizationContactWebsite", "Website", "text") Then
                Return True
            End If

            Dim pColumnInfo = vProData.ColumnInformation("LibraryOrganizationContactWebsite", "Website")
            If CStr2(pColumnInfo("TypeName")).ToLower() = "varchar" And CDec2(pColumnInfo("Length")) = -1 Then
                Return False
            End If
            Dim pSql As String

            vProData.BeginTransaction()
            pSql = " EXEC sp_rename N'dbo.LibraryOrganizationContactWebsite.Website', N'Website_old', 'COLUMN'"
            vProData.ExecuteSQL(pSql)

            vProData.AddColumn("LibraryOrganizationContactWebsite", "Website", "text")

            pSql = "UPDATE LibraryOrganizationContactWebsite" & vbCrLf &
            " SET Website = Website_old"
            vProData.ExecuteSQL(pSql)

            pSql = " ALTER TABLE dbo.LibraryOrganizationContactWebsite" & vbCrLf &
            " DROP COLUMN Website_old"
            vProData.ExecuteSQL(pSql)
            vProData.CommitTransaction()

            Return True
        Catch ex As Exception
            vProData.RollbackTransaction(True, True)
            AppendToDebug("Error in ChangeLibraryOrganizationContactWebsite. vb5K3BHXPWHR! vbDZJ5GNELEV!", True, ex, True)
            'Throw New ApplicationException("Error in ChangeLibraryOrganizationContactWebsite. vb5K3BHXPWHR!", ex)
        End Try

    End Function

    Private Function ChangeSystemDefaults(vProData As ProData, vDatabase As String) As Boolean
        Dim pSql As String

        If vProData.Execute("SELECT * FROM sys.indexes WHERE name='IX_SystemDefaults' AND object_id = OBJECT_ID('SystemDefaults')").Rows.Count > 0 Then
            pSql = " DROP INDEX SystemDefaults.IX_SystemDefaults "
            vProData.ExecuteSQL(pSql)
        End If

        Return True
    End Function

    Private Function GetYearWeekExpression(vFieldName As String) As String

        Return "AS 'Y' + CONVERT(varchar, datepart(year," & EscapeSqlName(vFieldName) & ")) + '|W' + RIGHT('00'+ISNULL(CONVERT(varchar, datepart(iso_week, " & EscapeSqlName(vFieldName) & ")), ''), 2)"

    End Function

    Public Function ChangeTransInvoiceSCDiscountExempt(vProData As ProData)
        If vProData.Execute("SELECT 1 FROM sys.columns WHERE Name = N'SCDiscountExempt_old' AND Object_ID = Object_ID(N'dbo.TransInvoice')").Rows.Count = 0 Then
            vProData.ExecuteSQL("EXEC sp_rename N'dbo.TransInvoice.SCDiscountExempt', N'SCDiscountExempt_old', 'COLUMN'")
        End If

        Return True
    End Function
End Class
