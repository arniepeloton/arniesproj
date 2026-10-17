Public Class OfflineDataController
    Inherits System.Web.Mvc.Controller

    Private Shared ReadOnly mDataImporters As Dictionary(Of String, ImportDataClass) ' keys = OrgID

    Shared Sub New()

        mDataImporters = New Dictionary(Of String, ImportDataClass)

    End Sub

    Function Index() As String

        '#If DEBUG Then
        '        WebRequest.Instance.OrgID = "9d4b1500-13c0-49f9-a8e5-a16c13ec899e"
        '        If WebRequest.Instance.HandleOrganization(False) Then
        '            If WebRequest.Instance.Business.SystemDefaults.AllowPriceAdjustment Then
        '                If HasWholesaleDatabase() Then
        '                    Call New InvoiceToOrder(Common.WebRequest.ProData, CreateWholesaleProData).Execute()
        '                End If
        '            End If
        '        End If
        '#End If

        Return "wala ni xay view. puro functions lng ang naa dri"

    End Function

#If DEBUG Then
    Dim mFilename As String

    Public Sub New()

        mFilename = Common.ServerPath & "temp\sqlite.db"

    End Sub

    Dim mDefaultCustomerID As String
    Const mOrganizationID As String = "{9D4B1500-13C0-49F9-A8E5-A16C13EC899E}" ' LibrarySubscription.OrganizationID
    Const mUsernames As String = "'aa', 'bb', 'test', 'frans_eize'"
    Dim mProData As ProData
    Const mDbName As String = "MobizDUC"
    Const mTop As String = "TOP 50"

#Region " export "
    <HttpPost()>
    Public Function FileInfo() As String

        Dim pFile As IO.FileInfo
        Dim pOut As JsonDictionary

        pFile = New IO.FileInfo(mFilename)
        If pFile.Exists Then
            pOut = New JsonDictionary
            pOut.Add("FileDate", pFile.LastWriteTime.ToString("s", Globalization.CultureInfo.InvariantCulture)) ' "u" cannot be parsed by Android
            pOut.Add("FileSize", pFile.Length)
            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("Error", "File not found")
        End If

    End Function

    Public Function DownloadSqliteDb() As ActionResult

        Dim pFile As IO.FileInfo
        Try
            If IO.File.Exists(mFilename) = False Then
                CreateSqliteDb()
            End If

            pFile = New IO.FileInfo(mFilename)
            If pFile.Exists Then
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(mFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(mFilename, True))
                Response.TransmitFile(mFilename)
            Else
            End If
        Catch ex As Exception

        End Try

    End Function

    Private Sub CreateSqliteDb()

        Dim pFilename As String
        Dim pSqlite As ProSqlite

        mProData = Common.WebRequest.ProData
        pFilename = mFilename & ".temp"
        pSqlite = New ProSqlite
        pSqlite.Open(pFilename)
        Try
            mProData.BeginTransaction()
            mProData.ExecuteSQL("USE " & mDbName)

            pSqlite.CreateTable("MobizLibraryPeopleUserAccount", GetUsers)
            pSqlite.CreateTable("SystemDefaults", GetSystemDefaults) ' call this before organization
            pSqlite.CreateTable("MobizSystemDefaults", GetMobizSystemDefaults) ' call this before organization
            pSqlite.CreateTable("MobizLibraryOrganization", GetOrganization)
            pSqlite.CreateTable("MobizLibraryOrganizationContactEmail", GetMobizLibraryOrganizationContactEmail)
            pSqlite.CreateTable("MobizLibraryOrganizationTradeName", GetMobizLibraryOrganizationTradeName)
            pSqlite.CreateTable("MobizLibraryOrganizationFOWST", GetOrganizationFowst)
            pSqlite.CreateTable("MobizLibraryOrganizationFOWSTTel", GetMobizLibraryOrganizationFOWSTTel)
            pSqlite.CreateTable("MobizLibraryOrganizationFOWSTEmail", GetMobizLibraryOrganizationFOWSTEmail)
            pSqlite.CreateTable("MobizLibraryOrganizationFOWSTOperationHRs", GetMobizLibraryOrganizationFOWSTOperationHRs)

            pSqlite.CreateTable("MobizLibraryPIMProduct", GetLibraryPIMProduct) ' #MobizLibraryPIMProduct
            pSqlite.CreateTable("MobizLibraryPeopleOrganization", GetLibraryPeopleOrganization)
            pSqlite.CreateTable("MobizLibraryPeople", GetLibraryPeople)
            pSqlite.CreateTable("MobizLibraryPeoplePreNominalSuffix", GetMobizLibraryPeoplePreNominalSuffix)
            pSqlite.CreateTable("MobizLibraryCities", GetMobizLibraryCities)
            pSqlite.CreateTable("LibraryOrganizationCustomer", GetLibraryOrganizationCustomer)
            pSqlite.CreateTable("MobizLibraryCountries", GetMobizLibraryCountries)
            pSqlite.CreateTable("MobizLibraryAreaCodes", GetMobizLibraryAreaCodes)
            pSqlite.CreateTable("MobizLibraryTelcos", GetMobizLibraryTelcos)
            pSqlite.CreateTable("MobizLibraryTelcoPrefixes", GetMobizLibraryTelcoPrefixes)


            pSqlite.CreateTable("LibraryOrganizationEmployee", GetLibraryOrganizationEmployee)
            pSqlite.CreateTable("LibraryOrganizationEmployeeFOWST", GetLibraryOrganizationEmployeeFOWST)

            pSqlite.CreateTable("LibrarySecurityModule", GetLibrarySecurityModule)
            pSqlite.CreateTable("LibrarySecurityModuleMain", GetLibrarySecurityModuleMain)
            pSqlite.CreateTable("LibrarySecurityRole", GetLibrarySecurityRole)
            pSqlite.CreateTable("LibrarySecurityRoleModules", GetLibrarySecurityRoleModules)
            pSqlite.CreateTable("LibrarySecurityRoleUser", GetLibrarySecurityRoleUser)

            pSqlite.CreateTable("MobizLibraryPIMProductMeasurement", GetMobizLibraryPIMProductMeasurement)
            pSqlite.CreateTable("MobizLibraryPIMMeasure", GetMobizLibraryPIMMeasure)
            pSqlite.CreateTable("MobizLibraryCountriesLanguageLabel", GetMobizLibraryCountriesLanguageLabel)
            pSqlite.CreateTable("MobizLibraryPIMProductPriceLevel", GetMobizLibraryPIMProductPriceLevel)
            pSqlite.CreateTable("MobizLibraryPIMPriceLevel", GetMobizLibraryPIMPriceLevel)
            pSqlite.CreateTable("LibraryOrganizationPriceLevel", GetLibraryOrganizationPriceLevel)
            pSqlite.CreateTable("TransInvoice", GetTransInvoice)
            pSqlite.CreateTable("TransInvoiceItems", GetTransInvoiceItems)
            pSqlite.CreateTable("TransInvoiceDiscount", GetTransInvoiceDiscount)
            pSqlite.CreateTable("TransInvoiceSeniorCitizenPWDCustomer", GetTransInvoiceSeniorCitizenPWDCustomer) 'additional by janine july 26
            pSqlite.CreateTable("TransInvoiceItemSalesReps", GetTransInvoiceItemSalesReps)
            pSqlite.CreateTable("LibraryDevice", GetLibraryDevice)
            pSqlite.CreateTable("LibraryDeviceFowst", GetLibraryDeviceFowst)
            pSqlite.CreateTable("LibraryDeviceFowstPrinter", GetLibraryDeviceFowstPrinter)
            pSqlite.CreateTable("MobizLibraryOrganizationRepresentatives", GetMobizLibraryOrganizationRepresentatives)
            pSqlite.CreateTable("MobizLibraryOrganizationRepresentativesEmail", GetMobizLibraryOrganizationRepresentativesEmail)
            pSqlite.CreateTable("MobizLibraryOrganizationRepresentativesMobile", GetMobizLibraryOrganizationRepresentativesMobile)
            pSqlite.CreateTable("MobizLibraryOrganizationRepresentativesTel", GetMobizLibraryOrganizationRepresentativesTel)

            pSqlite.CreateTable("MobizLibraryBanks", GetMobizLibraryBanks)
            pSqlite.CreateTable("MobizLibraryBankBranch", GetMobizLibraryBankBranch)
            pSqlite.CreateTable("LibraryDiscountSettings", GetLibraryDiscountSettings)

            pSqlite.CreateTable("LibraryReceiptSettings", GetLibraryReceiptSettings)
            pSqlite.CreateTable("LibraryReceiptSettingsFowst", GetLibraryReceiptSettingsFowst)
            pSqlite.CreateTable("LibraryReceiptSettingsMessage", GetLibraryReceiptSettingsMessage)

            pSqlite.CreateTable("TransCustomerPayment", GetTransCustomerPayment)
            pSqlite.CreateTable("TransCustomerPaymentChecks", GetTransCustomerPaymentChecks)
            pSqlite.CreateTable("TransCustomerPaymentInvoices", GetTransCustomerPaymentInvoices)

            pSqlite.CreateTable("TransMemo", GetTransMemo)
            pSqlite.CreateTable("TransMemoSalesReturn", GetTransMemoSalesReturn)
            pSqlite.CreateTable("TransSalesReturnLedger", GetTransSalesReturnLedger)

            mProData.RollbackTransaction() ' dont commit

            pSqlite.Close()
            If IO.File.Exists(mFilename) Then IO.File.Delete(mFilename)
            IO.File.Move(pFilename, mFilename)
        Catch ex As Exception
            Try
                pSqlite.Close()
                IO.File.Delete(pFilename)
            Catch ' just ignore any err
            End Try
            Throw
        End Try

    End Sub

    Private Function GetLibraryReceiptSettingsMessage() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT *" & vbCrLf &
            " FROM LibraryReceiptSettingsMessage"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM LibraryReceiptSettingsMessage"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryReceiptSettingsFowst() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryReceiptSettingsFowst.*" & vbCrLf &
            " FROM LibraryReceiptSettingsFowst"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM LibraryReceiptSettingsFowst"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryReceiptSettings() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryReceiptSettings.*" & vbCrLf &
            " FROM LibraryReceiptSettings"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM LibraryReceiptSettings"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryDiscountSettings() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryDiscountSettings.*" & vbCrLf &
            " FROM LibraryDiscountSettings"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM LibraryDiscountSettings"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryBanks() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MobizLibraryBanks.*" & vbCrLf &
            " FROM MobizLibraryBanks"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM MobizLibraryBanks"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryBankBranch() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MobizLibraryBankBranch.*" & vbCrLf &
            " FROM MobizLibraryBankBranch"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM MobizLibraryBankBranch"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransMemo() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * " & vbCrLf &
            " FROM TransMemo"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransMemo"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransMemoSalesReturn() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * " & vbCrLf &
            " FROM TransMemoSalesReturn"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransMemoSalesReturn"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransSalesReturnLedger() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * " & vbCrLf &
            " FROM TransSalesReturnLedger"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransSalesReturnLedger"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransCustomerPaymentInvoices() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransCustomerPaymentInvoices.*" & vbCrLf &
            " FROM TransCustomerPaymentInvoices"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransCustomerPaymentInvoices"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransCustomerPaymentChecks() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransCustomerPaymentChecks.*" & vbCrLf &
            " FROM TransCustomerPaymentChecks"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransCustomerPaymentChecks"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransCustomerPayment() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransCustomerPayment.*" & vbCrLf &
            " FROM TransCustomerPayment"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransCustomerPayment"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationRepresentativesEmail() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganizationRepresentativesEmail.*" & vbCrLf &
            " FROM MobizLibraryOrganizationRepresentativesEmail"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM MobizLibraryOrganizationRepresentativesEmail"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationRepresentativesMobile() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganizationRepresentativesMobile.* " & vbCrLf &
            " FROM MobizLibraryOrganizationRepresentativesMobile "
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM MobizLibraryOrganizationRepresentativesMobile"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationRepresentativesTel() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganizationRepresentativesTel.* " & vbCrLf &
            " FROM MobizLibraryOrganizationRepresentativesTel "
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM MobizLibraryOrganizationRepresentativesTel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationRepresentatives() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganizationRepresentatives.*" & vbCrLf &
            " FROM MobizLibraryOrganizationRepresentatives"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM MobizLibraryOrganizationRepresentatives"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryDeviceFowstPrinter() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * " & vbCrLf &
            " FROM LibraryDeviceFowstPrinter "
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM LibraryDeviceFowstPrinter"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryDeviceFowst() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryDeviceFowst.*" & vbCrLf &
            " INTO #LibraryDeviceFowst" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " JOIN #LibraryDevice ON #LibraryDevice.ID = LibraryDeviceFowst.DeviceID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #LibraryDeviceFowst"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryDevice() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryDevice.*" & vbCrLf &
            " INTO #LibraryDevice" & vbCrLf &
            " FROM LibraryDevice"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #LibraryDevice"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransInvoiceItemSalesReps() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT *" & vbCrLf &
            " FROM TransInvoiceItemSalesReps "
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM TransInvoiceItemSalesReps "
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransInvoiceDiscount() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoiceDiscount.*" & vbCrLf &
            " INTO #TransInvoiceDiscount" & vbCrLf &
            " FROM TransInvoiceDiscount"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #TransInvoiceDiscount"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function


    Private Function GetTransInvoiceSeniorCitizenPWDCustomer() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoiceSeniorCitizenPWDCustomer.*" & vbCrLf &
            " INTO #TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
            " FROM TransInvoiceSeniorCitizenPWDCustomer"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #TransInvoiceSeniorCitizenPWDCustomer"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransInvoiceItems() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransInvoiceItems.*" & vbCrLf &
            " INTO #TransInvoiceItems" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN #TransInvoice ON #TransInvoice.ID = TransInvoiceItems.InvoiceID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #TransInvoiceItems"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetTransInvoice() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT " & mTop & " TransInvoice.*" & vbCrLf &
            " INTO #TransInvoice" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #TransInvoice"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryOrganizationPriceLevel() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT LibraryOrganizationPriceLevel.*" & vbCrLf &
            " INTO #LibraryOrganizationPriceLevel" & vbCrLf &
            " FROM LibraryOrganizationPriceLevel" & vbCrLf
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #LibraryOrganizationPriceLevel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryPIMPriceLevel() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT MobizLibraryPIMPriceLevel.*" & vbCrLf &
            " INTO #MobizLibraryPIMPriceLevel" & vbCrLf &
            " FROM MobizLibraryPIMPriceLevel" & vbCrLf &
            " JOIN #MobizLibraryPIMProductPriceLevel ON #MobizLibraryPIMProductPriceLevel.PriceLevelID = MobizLibraryPIMPriceLevel.ID" & vbCrLf &
            " AND MobizLibraryPIMPriceLevel.IsActive = 1"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #MobizLibraryPIMPriceLevel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryPIMProductPriceLevel() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT MobizLibraryPIMProductPriceLevel.*" & vbCrLf &
            " INTO #MobizLibraryPIMProductPriceLevel" & vbCrLf &
            " FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            " JOIN #MobizLibraryPIMProduct ON #MobizLibraryPIMProduct.ID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(mOrganizationID)
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #MobizLibraryPIMProductPriceLevel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryCountriesLanguageLabel() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT MobizLibraryCountriesLanguageLabel.*" & vbCrLf &
            " INTO #MobizLibraryCountriesLanguageLabel" & vbCrLf &
            " FROM MobizLibraryCountriesLanguageLabel" & vbCrLf &
            " JOIN #MobizLibraryPIMMeasure ON #MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #MobizLibraryCountriesLanguageLabel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryPIMMeasure() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT MobizLibraryPIMMeasure.*" & vbCrLf &
            " INTO #MobizLibraryPIMMeasure" & vbCrLf &
            " FROM MobizLibraryPIMMeasure" & vbCrLf &
            " JOIN #MobizLibraryPIMProductMeasurement ON #MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #MobizLibraryPIMMeasure"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryPIMProductMeasurement() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT MobizLibraryPIMProductMeasurement.*" & vbCrLf &
            " INTO #MobizLibraryPIMProductMeasurement" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " JOIN #MobizLibraryPIMProduct ON #MobizLibraryPIMProduct.ID = MobizLibraryPIMProductMeasurement.ProductID"
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #MobizLibraryPIMProductMeasurement"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryOrganizationEmployeeFOWST() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSqlBase As String

        pSqlBase = "SELECT " & mDbName & ".dbo.LibraryOrganizationEmployeeFOWST.*" & vbCrLf &
            " FROM " & mDbName & ".dbo.LibraryOrganizationEmployeeFOWST"
        pSql = pSqlBase
        pDT = mProData.Execute(pSql)

        Return pDT
    End Function

    Private Function GetLibraryOrganizationEmployee() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSqlBase As String

        pSqlBase = "SELECT " & mDbName & ".dbo.LibraryOrganizationEmployee.*" & vbCrLf &
            " FROM " & mDbName & ".dbo.LibraryOrganizationEmployee" & vbCrLf &
            " WHERE ISNULL(Deactivated, 0) = 0"
        pSql = pSqlBase
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibrarySecurityModule() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibrarySecurityModule"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibrarySecurityModuleMain() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibrarySecurityModuleMain"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibrarySecurityRole() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibrarySecurityRole"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibrarySecurityRoleModules() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibrarySecurityRoleModules"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibrarySecurityRoleUser() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibrarySecurityRoleUser"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryCountries() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryCountries"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryAreaCodes() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryAreaCodes"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryTelcos() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryTelcos"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryTelcoPrefixes() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryTelcoPrefixes"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryOrganizationCustomer() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSqlBase As String

        pSqlBase = "SELECT " & mDbName & ".dbo.LibraryOrganizationCustomer.*" & vbCrLf &
            " FROM " & mDbName & ".dbo.LibraryOrganizationCustomer" & vbCrLf &
            " WHERE ISNULL(Deactivated, 0) = 0"
        pSql = pSqlBase
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryCities() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSqlBase As String

        pSqlBase = "SELECT " & mTop & " MobizLibraryCities.*" & vbCrLf &
            " FROM MobizLibraryCities" & vbCrLf
        pSql = pSqlBase
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetUsers() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 50 PeopleID, Username, UserPassword FROM MobizLibraryPeopleUserAccount"
        pSql &= " UNION "
        pSql &= "SELECT TOP 50 PeopleID, Username, UserPassword FROM MobizLibraryPeopleUserAccount WHERE Username In (" & mUsernames & ")"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetOrganization() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 50 * FROM MobizLibraryOrganization"
        pSql &= " UNION "
        pSql &= " SELECT * FROM MobizLibraryOrganization " &
            " WHERE IsABank = 1 "
        If CStr2(mDefaultCustomerID) <> "" Then
            pSql &= " UNION "
            pSql &= "SELECT TOP 50 * FROM MobizLibraryOrganization" &
                " WHERE ID = " & ReadyString(mDefaultCustomerID)
        End If

        pSql &= " UNION "
        pSql &= "SELECT TOP 50 MobizLibraryOrganization.* FROM MobizLibraryOrganization INNER JOIN " &
                      "dbo.MobizLibraryPeopleOrganization ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryPeopleOrganization.OrganizationID INNER JOIN " &
                      "dbo.MobizLibraryPeopleUserAccount ON dbo.MobizLibraryPeopleOrganization.PeopleID = dbo.MobizLibraryPeopleUserAccount.PeopleID " &
            " WHERE MobizLibraryPeopleOrganization.PeopleID IN ("
        pSql &= "SELECT TOP 50 PeopleID FROM MobizLibraryPeopleUserAccount WHERE Username In (" & mUsernames & ")" &
            ")"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationContactEmail() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryOrganizationContactEmail"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationTradeName() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryOrganizationTradeName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetOrganizationFowst() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSqlBase As String

        pSqlBase = "SELECT TOP 50 MobizLibraryOrganizationFOWST.*" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf
        pSql = pSqlBase & " UNION " & vbCrLf
        pSql &= pSqlBase &
            " JOIN MobizLibraryPeopleOrganization ON MobizLibraryPeopleOrganization.OrganizationID = MobizLibraryOrganizationFOWST.OrganizationID" &
            " WHERE MobizLibraryPeopleOrganization.PeopleID IN ("
        pSql &= "SELECT TOP 50 PeopleID FROM MobizLibraryPeopleUserAccount WHERE Username In (" & mUsernames & ")" &
            ")"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationFOWSTTel() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryOrganizationFOWSTTel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationFOWSTEmail() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryOrganizationFOWSTEmail"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryOrganizationFOWSTOperationHRs() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryOrganizationFOWSTOperationHRs"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryPIMProduct() As DataTable

        Dim pDT As DataTable
        Dim pSql As String




        pSql = "SELECT " & mTop & " MobizLibraryPIMProduct.*" & vbCrLf &
            " INTO #MobizLibraryPIMProduct" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.IsActive = 1" & vbCrLf &
            " AND ISNULL(MobizLibraryPIMProduct.ProductName, '') <> ''" & vbCrLf
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #MobizLibraryPIMProduct"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizLibraryPeoplePreNominalSuffix() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizLibraryPeoplePreNominalSuffix"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryPeopleOrganization() As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pSqlBase As String

        pSqlBase = "SELECT TOP 50 * FROM MobizLibraryPeopleOrganization" & vbCrLf
        pSql = pSqlBase & " UNION " & vbCrLf
        pSql &= pSqlBase &
            " WHERE ID IN ("
        pSql &= "SELECT TOP 50 PeopleID FROM MobizLibraryPeopleUserAccount WHERE Username In (" & mUsernames & ")" &
            ")"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLibraryPeople() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 50 * FROM MobizLibraryPeople"
        pSql &= " UNION "
        pSql &= "SELECT TOP 50 * FROM MobizLibraryPeople" &
            " WHERE ID IN ("
        pSql &= "SELECT TOP 50 PeopleID FROM MobizLibraryPeopleUserAccount WHERE Username In ('aa', 'bb', 'test', 'frans_eize')" &
            ")"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetMobizSystemDefaults() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM MobizSystemDefaults"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetSystemDefaults() As DataTable

        Dim pDT As DataTable
        Dim pSql As String = ""
        Dim pDV As DataView
        Dim pDescriptions As List(Of String)

        pDescriptions = New List(Of String)
        pDescriptions.Add("Customer Organization ID")
        pDescriptions.Add("POS Delivery Address ID")
        pDescriptions.Add("POS Billing Address ID")

        For Each pItem As String In pDescriptions
            If pSql <> "" Then pSql &= ", "
            pSql &= ReadyString(pItem) & vbCrLf
        Next pItem

        pSql = "SELECT *" & vbCrLf &
            " ,'this is to test escape patterns in sqlite' AS [test]" & vbCrLf & _
            " ,'dummy' AS [test[2]]]" & vbCrLf & _
            " ,'dummy' AS [test3]]]" & vbCrLf & _
            " ,'dummy' AS [test[4]" & vbCrLf &
            " FROM " & mDbName & ".dbo.SystemDefaults" & vbCrLf &
            " WHERE Description IN (" & vbCrLf &
            pSql &
            ")"
        pDT = mProData.Execute(pSql)
        pDV = New DataView(pDT, "Description = 'Customer Organization ID'", "", DataViewRowState.CurrentRows)
        If pDV.Count > 0 Then
            mDefaultCustomerID = CStr2(pDV(0)("DefaultValue"))
        End If

        Return pDT

    End Function

    Private Function GetLibraryFowst() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        'pSql = "SELECT TOP 50 * FROM MobizLibraryPeople"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function
#End Region ' export

#Region " import "
    Public Sub ImportTest()

        Import()

    End Sub

    <HttpPost()>
    Public Function Upload() As String

        Dim pDestination As String
        Dim pOut As JsonDictionary

        pOut = New JsonDictionary
        Try
            pDestination = Common.ServerPath & "temp\DataImport\import.zip"
            Request.Files("filename").SaveAs(pDestination)
        Catch ex As System.Exception
            pOut.Add("Error", "An error occured while saving imported file: " & ex.Message)
            Return pOut.Serialize
        End Try

        Try
            Import()
        Catch ex As Exception
            pOut.Add("Error", "An error occured while importing data: " & ex.Message)
            Return pOut.Serialize
        End Try

        pOut.Add("Result", "ok")

        Return pOut.Serialize

    End Function

    '<HttpPost()> ' in release. comment this line in debug
    Public Sub Import()

        Dim pImport As MobizImport

        pImport = New MobizImport
        pImport.ProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=mDbName,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password)
        pImport.InputFilename = Common.ServerPath & "temp\DataImport\import.zip"
        pImport.OutputFilename = Common.ServerPath & "\temp\DataImport\ImportResult.zip"
        pImport.SourceFileFormat = DataImportExport.DataImport.eSourceFileFormat.Json
        pImport.Import()

    End Sub

    Private Class MobizImport
        Inherits DataImportExport.DataImport

        Public Sub New()

        End Sub

    End Class

#End Region ' import

#End If

    <HttpPost()>
    Public Function SaveSummary() As String


    End Function

    Public Function GetSchemaInfo() As String

        Dim pFile As IO.FileInfo
        Dim pFilename As String
        Dim pOut As JsonDictionary

        CreateSchemaZipfile()
        pFilename = Common.ServerPath & "temp\schema.zip"
        pFile = New IO.FileInfo(pFilename)
        If pFile.Exists Then
            pOut = New JsonDictionary
            pOut.Add("FileDate", pFile.LastWriteTime.ToString("s", Globalization.CultureInfo.InvariantCulture)) ' "u" cannot be parsed by Android
            pOut.Add("FileSize", pFile.Length)
            Return pOut.Serialize
        Else
            Return JsonDictionary.Serialize("Error", "File not found")
        End If

    End Function

    Public Function Schema() As ActionResult

        Dim pFile As IO.FileInfo
        Dim pFilename As String
        Dim pSqliteSchemaExporter As SqliteSchemaExporter
        Dim pDbs As List(Of String)

        Debug.Print("OfflineDataController.Schema")
        Try
            CreateSchemaZipfile()
            pFilename = Common.ServerPath & "\temp\schema.zip"
            pFile = New IO.FileInfo(pFilename)
            If pFile.Exists Then
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(pFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(IO.Path.GetFileName(pFilename), True))
                Response.TransmitFile(pFilename)
            End If
        Catch ex As Exception
            AppendToDebug(ex.Message, True, ex)
            Return Content(ex.Message)
        End Try

    End Function

    Shared mCreateSchemaZipfileLock As New Object

    Private Sub CreateSchemaZipfile()

        Dim pFile As IO.FileInfo
        Dim pFilename As String
        Dim pSqliteSchemaExporter As SqliteSchemaExporter
        Dim pDbs As List(Of String)

        SyncLock mCreateSchemaZipfileLock ' dont let more than 1 thread access this file at the same
            Try
                pFilename = Common.ServerPath & "\temp\schema.zip"
                pFile = New IO.FileInfo(pFilename)
                If pFile.Exists = False Then
                    IO.File.Delete(pFilename & "_temp")
                    pSqliteSchemaExporter = New SqliteSchemaExporter(pFilename & "_temp")
                    With pSqliteSchemaExporter
                        .ProData = Common.WebRequest.ProData.Clone

                        pDbs = Common.WebRequest.ProData.GetDatabases(True, True)
                        If pDbs.Contains("mobizmain") Then
                            .DatabaseName = "MobizMain"
                        Else
                            .DatabaseName = "Mobiz"
                        End If
                        .DestinationDatabaseName = "Mobiz"
                        .AddAllTables()

                        If pDbs.Contains("MobizTemplate".ToLower()) Then
                            .DatabaseName = "MobizTemplate"
                        Else
                            .DatabaseName = "MobizDUC"
                        End If
                        .DestinationDatabaseName = "Private"
                        .AddAllTables()
                    End With
                    IO.File.Move(pFilename & "_temp", pFilename)
                End If
            Catch ex As Exception
                Throw
            End Try
        End SyncLock

    End Sub

    ' this should be a private class
    Public Class SqliteSchemaExporter

        Public ProData As ProData
        ReadOnly mZip As ProZip
        ''' <summary>
        ''' ProData will point to this database. This will also be used as a folder name inside the zip file. 
        ''' If nothing or blank string, zipped json file will reside in the root folder inside the zip file
        ''' </summary>
        ''' <remarks></remarks>
        Public DatabaseName As String
        ''' <summary>
        ''' optional, if set null, will follow DatabaseName
        ''' </summary>
        ''' <remarks></remarks>
        Public DestinationDatabaseName As String

        Public Sub New(vDestinationZipFilename As String)

            mZip = New ProZip(vDestinationZipFilename)

        End Sub

        Public Sub Add(vTableName As String)

            Add(vTableName, "SELECT TOP 0 * FROM " & EscapeSqlName(vTableName))

        End Sub

        Public Sub Add(vTableName As String, vSql As String)

            Dim pDT As DataTable
            Dim pJson As JsonDictionary
            Dim pInternalFolderName As String

            If ProData Is Nothing Then
                Throw New NullReferenceException("This SqliteSchemaExporter instance's ProData must not be null")
            End If

            If CStr2(DatabaseName).Trim <> "" Then
                ProData.ExecuteSQL("USE " & EscapeSqlName(DatabaseName))
            End If

            pDT = ProData.GetSchema(vSql)
            pJson = New JsonDictionary
            With pJson.AddList("Columns")
                For Each pDR As DataRow In pDT.Rows
                    Select Case MajorDataType(pDR("DataType"))
                        Case eMajorDataType.Date, eMajorDataType.Numeric, eMajorDataType.String
                            With .AddDictionary()
                                .Add("ColumnName", pDR("ColumnName"))
                                Select Case MajorDataType(pDR("DataType"))
                                    Case eMajorDataType.Date
                                        .Add("DataType", "DateTime")    ' these strings are case-sensitve and must match eMajorDataType in ProDbHelper.java

                                    Case eMajorDataType.Numeric
                                        .Add("DataType", "Number")

                                    Case eMajorDataType.String
                                        .Add("DataType", "String")

                                    Case Else
                                        Throw New Exception("Invalid column type: " & pDR("DataType").ToString)

                                End Select
                            End With

                    End Select
                Next pDR
            End With

            If CStr2(DestinationDatabaseName).Trim = "" Then
                pInternalFolderName = DatabaseName
            Else
                pInternalFolderName = DestinationDatabaseName
            End If
            mZip.AddString(pJson.ToString, vTableName & ".json", pInternalFolderName & "\")

        End Sub

        Public Sub AddAllTables()

            Dim pDT As DataTable
            Dim pSql As String

            If ProData Is Nothing Then
                Throw New NullReferenceException("This SqliteSchemaExporter instance's ProData must not be null")
            End If

            If CStr2(DatabaseName).Trim <> "" Then
                ProData.ExecuteSQL("USE " & EscapeSqlName(DatabaseName))
            End If

            pSql = "SELECT sysobjects.ID, sysobjects.Name" & _
                ",sysusers.name AS Owner" & _
                " FROM sysobjects" & _
                " JOIN sysusers" & _
                " ON sysusers.uid = sysobjects.uid" & _
                " WHERE xtype = 'U'" & _
                " AND sysobjects.status >= 0" & _
                " ORDER BY sysobjects.Name"
            pDT = ProData.Execute(pSql)
            For Each pDR As DataRow In pDT.Rows
                Add(pDR("Name"))
            Next pDR

        End Sub
    End Class

    Shared ReadOnly mDumpLock As New Object

    <HttpPost()>
    Public Function Dump() As String

        Dim pFilename As String
        Dim pTimestamp As String
        Dim pOrgID As String
        Dim pDeviceID As String

        If Request.Files.Count > 0 Then
            pOrgID = Common.WebRequest.Data("OrgID")
            If CStr2(pOrgID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "OrgID POST data is missing from uploaded file.")
            End If
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
            End If
            pOrgID = ReadyGuid(pOrgID)
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database.")
            End If

            pDeviceID = Common.WebRequest.Data("DeviceID")
            If CStr2(pDeviceID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing from uploaded file.")
            End If
            If IsGuid(pDeviceID) = False Then
                Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
            End If
            pDeviceID = ReadyGuid(pDeviceID)

            SyncLock mDumpLock ' prevent more than 1 thread inside
                pTimestamp = DateTimeStamp()
                Threading.Thread.Sleep(1000) ' make sure the time stamp is unique
            End SyncLock

            pFilename = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pTimestamp & Common.WebRequest.Data("Suffix") & ".zip"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)

            SaveTransImport(vDeviceID:=pDeviceID, vFilename:=pTimestamp)
            ImportOrganization(pOrgID, Nothing, Nothing)
            Return JsonDictionary.Serialize("TimeStamp", pTimestamp)
        Else
            Return JsonDictionary.Serialize("Error", "No file. vbEXNMB8E9RF!")
        End If

    End Function

    <HttpPost()>
    Public Function DumpSummary() As String

        Dim pFilename As String
        Dim pTimestamp As String
        Dim pOrgID As String
        Dim pDeviceID As String

        If Request.Files.Count > 0 Then
            pOrgID = Common.WebRequest.Data("OrgID")
            If CStr2(pOrgID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "OrgID POST data is missing from uploaded file.")
            End If
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
            End If
            pOrgID = ReadyGuid(pOrgID)
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database.")
            End If

            pDeviceID = Common.WebRequest.Data("DeviceID")
            If CStr2(pDeviceID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing from uploaded file.")
            End If
            If IsGuid(pDeviceID) = False Then
                Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
            End If
            pDeviceID = ReadyGuid(pDeviceID)

            SyncLock mDumpLock ' prevent more than 1 thread inside
                pTimestamp = DateTimeStamp()
                Threading.Thread.Sleep(100) ' make sure the time stamp is unique
            End SyncLock

            pFilename = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pTimestamp & "_summary.zip"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)

            Return JsonDictionary.Serialize("TimeStamp", pTimestamp)
        End If

    End Function

    Shared ReadOnly mImportOrganizationLock As New Object

    Private Function ImportOrganization(vOrgID As String, ByRef oMessage As String, ByRef oDataImport As ImportDataClass) As Boolean

        Dim pDR As DataRow

        SyncLock mImportOrganizationLock
            oMessage = ""
            If mDataImporters.ContainsKey(vOrgID) Then
                oDataImport = mDataImporters(vOrgID)
                If oDataImport.IsRunning Then
                    oMessage = "Org, " & vOrgID & ", is currently importing data from device, " & oDataImport.DeviceID & "."
                    Return False
                End If
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then
                oMessage = "Org, " & vOrgID & ", does not point to a subscriber."
                Return False
            End If

            pDR = LoadTransImportVacant()
            If pDR Is Nothing Then
                oMessage = "Org, " & vOrgID & ", does not have pending import."
                Return False
            End If

            oMessage = "Processing import. Date received: " & Format(pDR("DateReceived"), cGlobal.FormatDateTime)
            oDataImport = New ImportDataClass
            oDataImport.OrgID = vOrgID
            oDataImport.DeviceID = ReadyGuid(pDR("DeviceID"))
            oDataImport.InputFilename = pDR("Filename")
            oDataImport.Start()

            If mDataImporters.ContainsKey(vOrgID) Then
                mDataImporters(vOrgID) = oDataImport
            Else
                mDataImporters.Add(vOrgID, oDataImport)
            End If

            Return True
        End SyncLock

    End Function

    '<HttpPost()>
    'Public Function ImportData() As String

    '    Dim pOut As JsonDictionary
    '    Dim pFilename As String
    '    Dim pStr As String
    '    Dim pInfo As String
    '    Dim pInfoJson As JsonDictionary
    '    Dim pOrgID As String
    '    Dim pUuid As String
    '    Dim pError As String
    '    Dim pDataImportLog As DataImportLog
    '    Dim pImportDataClass As ImportDataClass
    '    Dim pDR As DataRow
    '    Dim pDeviceID As String

    '    Try
    '        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
    '        pDataImportLog = New DataImportLog
    '        pStr = Common.WebRequest.Data("TimeStamp")
    '        If CStr2(pStr).Trim = "" Then
    '            pError = "TimeStamp was not found in post data."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If
    '        pDataImportLog.AddLog("TimeStamp (POST data from device): " & pStr)

    '        pOrgID = Common.WebRequest.Data("OrgID")
    '        If CStr2(pOrgID).Trim = "" Then
    '            Return JsonDictionary.Serialize("Error", "OrgID POST data is missing from uploaded file.")
    '        End If
    '        If IsGuid(pOrgID) = False Then
    '            Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
    '        End If
    '        pOrgID = ReadyGuid(pOrgID)
    '        WebRequest.Instance.OrgID = pOrgID
    '        If WebRequest.Instance.HandleOrganization(True) = False Then
    '            Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database.")
    '        End If
    '        pDR = LoadTransImport(pStr)
    '        If pDR Is Nothing Then
    '            Return JsonDictionary.Serialize("Error", "TimeStamp, " & pStr & ", does not exist in OrgID, " & pOrgID & ".")
    '        End If
    '        pDeviceID = CStr2(pDR("DeviceID"))
    '        pFilename = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pStr & ".zip"
    '        If IO.File.Exists(pFilename) = False Then
    '            UpdateTransImport(Common.WebRequest.ProData, pFilename, "file not found")
    '            pError = "File was not found in pending imports folder."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If
    '        pDataImportLog.AddLog("FullFilename: " & pFilename)
    '        If C1.C1Zip.C1ZipFile.IsZipFile(pFilename) = False Then
    '            UpdateTransImport(Common.WebRequest.ProData, pFilename, "invalid zip file")
    '            pError = "First file is not a valid zip file."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If

    '        pInfo = New ProZip(pFilename).GetString("info.json")
    '        If CStr2(pInfo).Trim = "" Then
    '            pError = "Info.json does not exist or is empty."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If
    '        Try
    '            pInfoJson = New JsonDictionary()
    '            pInfoJson.SetJson(pInfo)
    '        Catch ex As System.Exception
    '            pError = "Info.json does not contain a valid JSON."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End Try
    '        pOrgID = pInfoJson("OrgID")
    '        If CStr2(pOrgID).Trim = "" Then
    '            pError = "Info.json does not contain OrgID."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If
    '        pDataImportLog.AddLog("pInfoJson(""OrgID""): " & pOrgID)
    '        If IsGuid(pOrgID) = False Then
    '            pError = "Info.json does not contain a valid GUID OrgID."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If
    '        WebRequest.Instance.OrgID = pOrgID
    '        If WebRequest.Instance.HandleOrganization(True) = False Then
    '            pError = "Info.json's OrgID does not point to a subscriber database."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If

    '        pUuid = pInfoJson("UUID")
    '        If CStr2(pUuid).Trim = "" Then
    '            pError = "Info.json does not contain Uuid."
    '            pDataImportLog.AddLog(pError)
    '            pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '            Return JsonDictionary.Serialize("Error", pError)
    '        End If
    '        pDataImportLog.AddLog("pInfoJson(""UUID""): " & pUuid)

    '        'pDR = WebRequest.Instance.Business.LibraryDevice.ByUuid(pUuid)
    '        'If pDR Is Nothing Then
    '        '    pError = "Device with UUID, " & pUuid & ",was not found on the server."
    '        '    pDataImportLog.AddLog(pError)
    '        '    pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '        '    Return JsonDictionary.Serialize("Error", pError)
    '        'End If
    '        'pDeviceID = CStr2(pDR("ID"))

    '        pImportDataClass = New ImportDataClass
    '        pImportDataClass.ID = GenerateString(10)
    '        pImportDataClass.InputFilename = pFilename
    '        pImportDataClass.OrgID = pOrgID
    '        pImportDataClass.Uuid = pUuid
    '        pImportDataClass.DataImportLog = pDataImportLog
    '        pImportDataClass.Start()
    '        mDataImporters.Add(pImportDataClass.ID, pImportDataClass)

    '        pOut = New JsonDictionary
    '        pOut.Add("DataImportID", pImportDataClass.ID)

    '        Return pOut.Serialize
    '    Catch ex As Exception
    '        pError = ex.Message & vbCrLf & GetStackTrace(, ex)
    '        pDataImportLog.AddLog(pError)
    '        pDataImportLog.Save(DataImportLog.eResultType.Fail)
    '        Return JsonDictionary.Serialize("Error", pError)
    '    End Try

    'End Function

    <HttpPost()>
    Public Function HasPendingImport() As String

        Dim pDeviceID As String
        Dim pOrgID As String
        Dim pMessage As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pDeviceID = Common.WebRequest.Data("DeviceID")
        If CStr2(pDeviceID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing.")
        End If
        If IsGuid(pDeviceID) = False Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        pOut = New JsonDictionary
        If HasPendingImport(pDeviceID, pMessage) Then
            pOut.Add("Value", "1")
        Else
            pOut.Add("Value", "0")
        End If
        pOut.Add("Message", pMessage)

        Return pOut.Serialize

    End Function

    Private Function HasPendingImport(ByVal vDeviceID As String, ByRef oMessage As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDrv As DataRowView

        oMessage = "Device, " & vDeviceID & ", has "
        pSql = "SELECT DateReceived" &
            ",ProcessStart" &
            ",ProcessEnd" &
            " FROM TransImports" &
            " WHERE DeviceID = " & ReadyString(vDeviceID) &
            " ORDER BY DateReceived"
        pDT = Common.WebRequest.ProData.Execute(pSql)
        pDT.DefaultView.RowFilter = "ProcessEnd IS NULL"
        If pDT.DefaultView.Count > 0 Then
            pDrv = pDT.DefaultView(0)
            oMessage &= "a pending import received on " & Format(pDrv("DateReceived"), cGlobal.FormatDateTime)
            If pDrv("ProcessStart") IsNot DBNull.Value Then
                oMessage &= " and started on " & Format(pDrv("ProcessStart"), cGlobal.FormatDateTime)
            End If
            oMessage &= "."
            Return True
        Else
            oMessage &= "no pending import."
            Return False
        End If

    End Function

    ''' <summary>
    ''' requires current organization
    ''' </summary>
    ''' <param name="vDeviceID"></param>
    ''' <param name="vFilename">fileonly, no path, no extension</param>
    ''' <remarks></remarks>
    Private Sub SaveTransImport(vDeviceID As String,
        vFilename As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        DeleteUnprocessedTransImport(vDeviceID)

        pIU = New InsertUpdate("TransImports")
        pIU("DeviceID") = ReadyString(vDeviceID)
        pIU("Filename") = ReadyString(vFilename)
        pIU("DateReceived") = "GETDATE()"
        pSql = pIU.GetInsert()

        Common.WebRequest.ProData.ExecuteSQL(pSql)

    End Sub

    Private Sub DeleteUnprocessedTransImport(vDeviceID As String)

        Dim pSql As String

        pSql = "DELETE FROM TransImports" &
            " WHERE ProcessStart IS NULL" &
            " AND DeviceID = " & ReadyString(vDeviceID)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

    End Sub

    'Private Function LoadTransImport(vDeviceID As String) As DataRow

    '    Dim pDR As DataRow
    '    Dim pSql As String

    '    pSql = "SELECT Filename" &
    '        " FROM TransImports" &
    '        " WHERE DeviceID = " & ReadyString(vDeviceID)
    '    pDR = Common.WebRequest.ProData.ExecuteRow(pSql)

    '    Return pDR

    'End Function

    Private Function LoadTransImportVacant() As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT TOP 1 Filename" &
            ",DeviceID" &
            ",DateReceived" &
            " FROM TransImports" &
            " WHERE ProcessStart IS NULL" &
            " ORDER BY DateReceived"
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR
        End If

        pSql = "SELECT TOP 1 Filename" &
            ",DeviceID" &
            ",DateReceived" &
            " FROM TransImports" &
            " WHERE ProcessEND IS NULL" &
            " ORDER BY DateReceived"
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)

        Return pDR

    End Function



    Private Shared mFollowUpImportDataLock As New Threading.SpinLock

    <HttpPost()>
    Public Function FollowUpImportData() As String

        Dim pID As String
        Dim pOut As JsonDictionary
        Dim pFilename As String

        ' BEGIN new methods
        Dim pDR As DataRow
        Dim pOrgID As String
        Dim pDeviceID As String
        Dim pDataImport As ImportDataClass
        Dim pMessage As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pLockTaken As Boolean
        Dim pFileText As String

        pOrgID = Common.WebRequest.Data("OrgID")
        If CStr2(pOrgID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "OrgID POST data is missing.")
        End If
        If IsGuid(pOrgID) = False Then
            Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
        End If
        pOrgID = ReadyGuid(pOrgID)

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True) = False Then
            Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database.")
        End If

        pDeviceID = Common.WebRequest.Data("DeviceID")
        If CStr2(pDeviceID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing.")
        End If
        If IsGuid(pDeviceID) = False Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        Do
            mFollowUpImportDataLock.TryEnter(1000, pLockTaken)
            If pLockTaken Then
                Exit Do
            End If
            If mFollowUpImportDataLock.IsHeldByCurrentThread Then
                Exit Do
            End If
            Threading.Thread.Sleep(1000)
        Loop

        If False Then
            Threading.Thread.Sleep(60000)
        End If

        Try
            pOut = New JsonDictionary

            ' find untagged record
            pSql = "SELECT TOP 1 Filename" &
                ",Success" &
                " FROM TransImports" &
                " WHERE DeviceID = " & ReadyString(pDeviceID) &
                " AND DownloadedDeviceDate IS NULL" &
                " AND Success IS NOT NULL" & vbCrLf &
                " ORDER BY DateReceived"
            pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                pFilename = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pDR("Filename") & "_result.txt"
                If IO.File.Exists(pFilename) Then
                    If (CDec2(pDR("Success")) = 0) Then
                        pIU = New InsertUpdate("TransImports")
                        pIU("DownloadedDeviceDate") = "GETDATE()"
                        pIU("Success") = 0
                        pSql = pIU.GetUpdate("Filename = " & ReadyString(pDR("Filename")))
                        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' get the number of affected records
                    End If
                    pFileText = FileText(pFilename)
                    If HasFilenameFromJson(pFileText) = False Then
                        pIU = New InsertUpdate("TransImports")
                        pIU("Success") = 0
                        pIU("DownloadedDeviceDate") = "GETDATE()"
                        pIU("Result") = ReadyString("filename not found. " & pFilename)
                        pSql = pIU.GetUpdate("Filename = " & ReadyString(pDR("Filename")))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                    Return pFileText
                End If
                pIU = New InsertUpdate("TransImports")
                pIU("Success") = 0
                pIU("DownloadedDeviceDate") = "GETDATE()"
                pIU("Result") = ReadyString("file not found. " & pFilename)
                pSql = pIU.GetUpdate("Filename = " & ReadyString(pDR("Filename")))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pOut.Add("Error", "Import result file not found. vbB52NGCM5AE")
                pOut("MustFollowUp") = 1
                Return pOut.Serialize
            End If

            ' no untagged record, so determine the current process
            ImportOrganization(pOrgID, pMessage, pDataImport)
            If pDataImport IsNot Nothing _
                AndAlso pDataImport.IsRunning _
                AndAlso pDataImport.DeviceID = pDeviceID Then
                pOut("Message") = "Data from this device is currently being imported to the server."
                pOut("MustFollowUp") = 1
                Return pOut.Serialize
            End If

            ' no data from this device is being imported. see if we can upload
            pSql = "SELECT TOP 1 Filename" &
                ",DateReceived" &
                " FROM TransImports" &
                " WHERE DeviceID = " & ReadyString(pDeviceID) &
                " AND (ProcessEnd IS NULL" &
                " OR ProcessStart IS NULL)" &
                " ORDER BY DateReceived"
            pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                If pDataImport Is Nothing Then
                Else
                    pOut("Message") = "Data from another device (" & pDataImport.DeviceID & ") is currently being imported to the server. Data from this device will be imported later. Date received: " & Format(pDR("DateReceived"), cGlobal.FormatDateTime) & "."
                End If
                pOut("MustFollowUp") = 1
                Return pOut.Serialize
            End If

            ' tell the device to upload data
            pOut("MustUpload") = 1

            Return pOut.Serialize
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", "vbPX6GU2TB8V; " & ex.Message)
        Finally
            Try
                If mFollowUpImportDataLock.IsHeldByCurrentThread Then
                    Try
                        ' do other things here
                    Catch ex As Exception
                        Stop2() ' just ignore this
                    End Try
                    mFollowUpImportDataLock.Exit()
                Else
                    Stop2() ' this is unexpected
                End If
            Catch ex As Exception
                Stop2()
            End Try
        End Try

    End Function

    Private Shared Function HasFilenameFromJson(vJsonString As String) As Boolean

        Dim p As JavaScriptSerializer
        Dim pJson As IDictionary(Of String, Object)

        pJson = New JavaScriptSerializer().Deserialize(Of IDictionary(Of String, Object))(vJsonString)
        If pJson.ContainsKey("d") = False Then
            Return False
        End If

        pJson = pJson("d")
        If pJson.ContainsKey("Filename") = False Then
            Return False
        End If
        If CStr2(pJson("Filename")).Trim() = "" Then
            Return False
        End If

        Return True

    End Function

    <HttpPost()>
    Public Function SetImportedDeviceDate() As String

        Dim pOrgID As String
        Dim pDeviceID As String
        Dim pIU As InsertUpdate
        Dim pDeviceDate As String
        Dim pFilename As String
        Dim pSql As String

        Try
            If False Then
                Throw New Exception("Test exception vbENT3LBKTPA")
            End If

            pOrgID = Common.WebRequest.Data("OrgID")
            If CStr2(pOrgID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "OrgID POST data is missing.")
            End If
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
            End If
            pOrgID = ReadyGuid(pOrgID)

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database.")
            End If

            pDeviceID = Common.WebRequest.Data("DeviceID")
            If CStr2(pDeviceID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing.")
            End If
            If IsGuid(pDeviceID) = False Then
                Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
            End If
            pDeviceID = ReadyGuid(pDeviceID)

            pDeviceDate = Common.WebRequest.Data("DeviceDate")
            pDeviceDate = ParseDate(pDeviceDate)
            If IsDate(pDeviceDate) = False Then
                Return JsonDictionary.Serialize("Error", "DeviceDate must be a valid date.")
            End If

            pFilename = Common.WebRequest.Data("Filename")
            If CStr2(pFilename).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "Filename POST data must not be blank. vbFRUX7XSY45")
            End If

            pIU = New InsertUpdate("TransImports")
            pIU("DownloadedDeviceDate") = ReadyString(ParseDate(pDeviceDate))
            pSql = pIU.GetUpdate("Filename = " & ReadyString(pFilename) &
                                 " AND DeviceID = " & ReadyString(pDeviceID))
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql)
            If CDec2(pSql) = 0 Then
                Return JsonDictionary.Serialize("Error", "SetImportedDeviceDate executed without error but the number of affected records is zero.")
            End If

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", "vbRQ9G8H8LC7 " & ex.Message)
        End Try

    End Function

    ' ImportDataClass = delete this sept 10, 2014
    'Public Class ImportDataClass

    '    Public InputFilename As String
    '    Public OrgID As String
    '    Public Uuid As String
    '    Public DataImportLog As DataImportLog
    '    ReadOnly mBusiness As Business
    '    ReadOnly mFolder As String
    '    Dim mDone As Boolean
    '    Public DeviceID As String
    '    Dim mThread As Threading.Thread

    '    Public Sub New()

    '        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
    '        mFolder = Common.ServerPath & "temp\pending import data replies\"

    '    End Sub

    '    Public ReadOnly Property TimeStamp As String
    '        Get

    '            Return IO.Path.GetFileNameWithoutExtension(InputFilename)

    '        End Get
    '    End Property

    '    Public Sub Start()

    '        Dim pDR As DataRow

    '        If IsGuid(OrgID) = False Then
    '            Throw New Exception("OrgID must be a valid GUID before calling Start. vsGM5UAT9R35")
    '        End If

    '        If IsGuid(DeviceID) = False Then
    '            Throw New Exception("DeviceID must be a valid GUID before calling Start. vbH762G76ENN")
    '        End If

    '        pDR = mBusiness.LibraryOrganization.ByID(OrgID)
    '        If pDR Is Nothing Then
    '            Throw New Exception("OrgID (" & OrgID & ") point to a non-existing record. vbJBSFSVZZR5")
    '        End If
    '        If CStr2(pDR("Database")).Trim = "" Then
    '            Throw New Exception("OrgID (" & OrgID & ") has no database. vbFDPU3SCMLQ")
    '        End If
    '        If mBusiness.ProData.GetDatabases(True, True).Contains(CStr(pDR("Database")).Trim().ToLower()) = False Then
    '            Throw New Exception("OrgID (" & OrgID & ") point to a non-existing database, " & pDR("Database") & ". vbY5HWN6HWRD")
    '        End If

    '        DataImportLog = New DataImportLog
    '        mBusiness.ProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))

    '        mThread = New Threading.Thread(
    '            Sub()
    '                Dim pResult As String
    '                Dim pOk As Boolean

    '                Try
    '                    StartTransImport(mBusiness.ProData, TimeStamp)
    '                    pOk = Process(pResult)
    '                Catch e As Exception
    '                    Try
    '                        pResult = JsonDictionary.Serialize("Error", e.Message & ";" &
    '                            GetStackTrace(True, e))
    '                        UpdateTransImport(mBusiness.ProData, TimeStamp, "error", True)
    '                    Catch ex As Exception
    '                        Stop2()
    '                    End Try
    '                End Try
    '                Try
    '                    FileText(GetFilename()) = pResult
    '                    If pOk Then
    '                        UpdateTransImport(mBusiness.ProData, TimeStamp, "ok", True)
    '                    Else
    '                        UpdateTransImport(mBusiness.ProData, TimeStamp, "error", False)
    '                    End If
    '                Catch ex As Exception
    '                    Stop2()
    '                End Try
    '                mDone = True
    '            End Sub)
    '        mThread.Start()

    '    End Sub

    '    Public ReadOnly Property IsRunning As Boolean
    '        Get

    '            Return (mThread.ThreadState = Threading.ThreadState.Running) _
    '                OrElse (mThread.ThreadState = Threading.ThreadState.WaitSleepJoin)

    '        End Get
    '    End Property

    '    Public Function GetMessage() As String

    '        Return "Server is still processing imported data."

    '    End Function

    '    Public Function IsDone() As Boolean

    '        Return mDone

    '    End Function

    '    Public Function GetFilename() As String

    '        Return Common.ServerPath & "temp\Uploads\" & OrgID & "\" & DeviceID & "\" & InputFilename & "_result.txt"

    '    End Function

    '    Private Function Process(ByRef oMessage As String) As Boolean

    '        Dim pOut As JsonDictionary
    '        Dim pZipOut As ProZip
    '        Dim pError As String
    '        Dim pFilenameOut As String
    '        Dim pFilename As String

    '        Dim pInfo As String
    '        Dim pInfoJson As JsonDictionary

    '        oMessage = ""

    '        If False Then
    '            Throw New Exception("Test exception vbWDVXFXZASB")
    '        End If

    '        If False Then
    '            Threading.Thread.Sleep(60 * 1000)
    '        End If

    '        pFilename = Common.ServerPath & "temp\Uploads\" & OrgID & "\" & DeviceID & "\" & InputFilename & ".zip"
    '        pInfo = New ProZip(pFilename).GetString("info.json")
    '        If CStr2(pInfo).Trim = "" Then
    '            pError = "Info.json does not exist or is empty"
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If
    '        Try
    '            pInfoJson = New JsonDictionary()
    '            pInfoJson.SetJson(pInfo)
    '        Catch ex As System.Exception
    '            pError = "Info.json does not contain a valid JSON"
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End Try

    '        Uuid = pInfoJson("UUID")
    '        If CStr2(Uuid).Trim = "" Then
    '            pError = "Info.json does not contain Uuid"
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If

    '        pFilenameOut = AppendBackslash(IO.Path.GetDirectoryName(pFilename)) & _
    '            TimeStamp & "_imported.zip"
    '        CreateFolder(pFilenameOut)
    '        pZipOut = New ProZip(pFilenameOut)
    '        pOut = New JsonDictionary

    '        mBusiness.ProData.BeginTransaction()
    '        ' additional by janine
    '        If ImportLibraryOrganization(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
    '            DataImportLog.AddLog(pError)
    '            DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If
    '        mBusiness.ProData.CommitTransaction()
    '        ' additional by janine
    '        If ImportLibraryOrganizationFOWST(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
    '            DataImportLog.AddLog(pError)
    '            DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If
    '        ' end of additional

    '        If ImportInvoice(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
    '            DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If

    '        mBusiness.ProData.BeginTransaction()
    '        ' additional by francis sept 03, 2014
    '        If ImportTransInventoryProductCount(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
    '            DataImportLog.AddLog(pError)
    '            DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If
    '        mBusiness.ProData.CommitTransaction()

    '        ' additional by francis sept 04, 2014
    '        If ImportLibraryFOWSTBin(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
    '            DataImportLog.AddLog(pError)
    '            DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            oMessage = JsonDictionary.Serialize("Error", pError)
    '            Return False
    '        End If

    '        If pZipOut.GetFileCount > 0 Then
    '            pOut.Add("Filename", InputFilename)
    '        End If
    '        pOut.Add("Result", "ok")
    '        DataImportLog.Save(DataImportLog.eResultType.Success)
    '        oMessage = pOut.Serialize

    '        Return True

    '    End Function

    '    Private Function ImportInvoice(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
    '               ByVal vOrgID As String, ByVal vUuid As String,
    '               ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut, pDtOutItems As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pNum As Integer
    '        Dim pDtItems As DataTable
    '        Dim pCreator As TransInvoice.Creator
    '        Dim mItemInitialized As Boolean
    '        Dim pDrInvoice As DataRow
    '        Dim pDataImportLogAlreadyImported As DataImportLog
    '        Dim pMaxSubNumber As Integer?

    '        ' customer payments
    '        Dim pDtCustomerPayments As DataTable
    '        Dim pBinderCustomerPayments As DictionaryBinder
    '        Dim pDtOutCustomerPayments As DataTable

    '        Dim pDtCustomerPaymentInvoices As DataTable
    '        Dim pBinderCustomerPaymentInvoices As DictionaryBinder

    '        Dim pDtCustomerPaymentDebitCards As DataTable
    '        Dim pBinderCustomerPaymentDebitCards As DictionaryBinder

    '        Dim pDtCustomerPaymentCreditCards As DataTable
    '        Dim pBinderCustomerPaymentCreditCards As DictionaryBinder

    '        oError = ""

    '        pDtCustomerPayments = CreateCustomerPaymentsDataTable(vInputZipFilename, pBinderCustomerPayments, oError)
    '        If pDtCustomerPayments Is Nothing Then
    '            Return False
    '        End If

    '        pDtCustomerPaymentInvoices = CreateCustomerPaymentInvoicesDataTable(vInputZipFilename, pBinderCustomerPaymentInvoices, oError)
    '        If pDtCustomerPaymentInvoices Is Nothing Then
    '            Return False
    '        End If

    '        pDtCustomerPaymentDebitCards = CreateCustomerPaymentDebitCardDataTable(vInputZipFilename, pBinderCustomerPaymentDebitCards, oError)
    '        If pDtCustomerPaymentDebitCards Is Nothing Then
    '            Return False
    '        End If

    '        pDtCustomerPaymentCreditCards = CreateCustomerPaymentCreditCardDataTable(vInputZipFilename, pBinderCustomerPaymentCreditCards, oError)
    '        If pDtCustomerPaymentCreditCards Is Nothing Then
    '            Return False
    '        End If

    '        DataImportLog.AddLog("Import from TransInvoice")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoice")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InvoiceNumber", True, SqlDbType.Int)
    '            .Add("InvoiceDate", True, SqlDbType.DateTime)
    '            .Add("CustomerID", True, SqlDbType.UniqueIdentifier)
    '            .Add("BillingAddressID", True, SqlDbType.UniqueIdentifier)
    '            .Add("DeliveryAddressID", True, SqlDbType.UniqueIdentifier)
    '            .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
    '            .Add("UserID", True, SqlDbType.UniqueIdentifier)
    '            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
    '            .Add("RefNum", False, SqlDbType.VarChar)
    '            .Add("ReasonForReconstructingData", False, SqlDbType.VarChar)
    '            .Add("Remarks", False, SqlDbType.VarChar)
    '            .Add("DeliveryDate", False, SqlDbType.DateTime)
    '            .Add("IsCancelled", False, SqlDbType.Bit)
    '            .Add("LastUpdated", True, SqlDbType.DateTime)
    '            .Add("SalesOrderNo", False, SqlDbType.Int)
    '            .Add("SalesOrderDate", False, SqlDbType.Date)
    '            .Add("SalesOrderUserID", False, SqlDbType.UniqueIdentifier)
    '            .Add("CustomerTableNo", False, SqlDbType.VarChar)
    '            .Add("IsForGovReport", False, SqlDbType.Bit)
    '            .Add("TotalAmount", False, SqlDbType.Money)
    '            .Add("IsTestOnly", False, SqlDbType.Bit) ' added by janine. for the self test
    '            .Add("PurchaseOrderNo", False, SqlDbType.VarChar) ' added by janine. for sales order taker aug 6
    '            .Add("PurchaseOrderDate", False, SqlDbType.Date) ' added by janine. for sales order taker aug 6
    '            .Add("PurchaserName", False, SqlDbType.VarChar) ' added by janine. for sales order taker aug 6
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")


    '        Do
    '            mBusiness.ProData.RollbackTransaction(True, True)

    '            If False Then
    '                Throw New Exception("Test exception vbWS8E53NK2C")
    '            End If

    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do

    '            'additional by janine sept 8 2014 5pm for sales order taker
    '            Dim posmode As String
    '            If pLine.ContainsKey("SalesOrderNo") AndAlso CStr2(pLine("SalesOrderNo")).Trim <> "" Then
    '                posmode = "so"
    '            Else
    '                posmode = "pos"
    '            End If
    '            ''''''''''''''''''''

    '            If mBusiness.LibraryPeople.Validate(
    '                vPeopleID:=pLine("UserID"),
    '                vOrgID:=vOrgID,
    '                vFowstID:=pLine("FowstID"),
    '                vPosMode:=posmode,
    '                vDeviceID:=pLine("DeviceID"),
    '                vVerifyUuid:=True,
    '                vDeviceUuid:=vUuid,
    '                oError:=oError) = False Then

    '                Exit Do
    '            End If

    '            If mItemInitialized = False Then
    '                mItemInitialized = True
    '                pDtItems = CreateInvoiceItemsDataTable(vInputZipFilename, oError)
    '            End If

    '            If IsGuid(pLine("ID")) Then
    '                pDrInvoice = mBusiness.TransInvoice.ById(pLine("ID"))
    '                If pDrInvoice IsNot Nothing _
    '                    AndAlso CDec2(pDrInvoice("InvoiceNumber")) <> 0 Then

    '                    If pDataImportLogAlreadyImported Is Nothing Then
    '                        pDataImportLogAlreadyImported = New DataImportLog
    '                        pDataImportLogAlreadyImported.AddLog("InputZipFilename: " & vInputZipFilename)
    '                        pDataImportLogAlreadyImported.AddLog("OrgID: " & OrgID)
    '                    End If
    '                    pDataImportLogAlreadyImported.AddLog("InvoiceID: " & pLine("ID"))
    '                    pDrOut = pDtOut.NewRow
    '                    pDrOut("ID") = pLine("ID")
    '                    pDtOut.Rows.Add(pDrOut)
    '                    Continue Do
    '                End If
    '                'mBusiness.TransInvoice.VerifyCounter(pLine("DeviceID"), pLine("FowstID"), pNum)
    '                'If CDec2(pLine("InvoiceNumber")) <= pNum Then
    '                '    DataImportLog.AddLog("Existing max invoice number/counter is: " & pNum & " but the invoice number to be imported is " & pLine("InvoiceNumber") & ". Imported number will become " & pNum + 1 & ". ID: " & pLine("ID") & ". Ref vbXAJ5YY4ELK")
    '                '    pLine("InvoiceNumber") = pNum + 1 ' TODO: apply business logic here
    '                '    Stop2()
    '                'ElseIf CDec2(pLine("InvoiceNumber")) <> pNum + 1 Then
    '                '    DataImportLog.AddLog("The invoice number to be imported is " & pLine("InvoiceNumber") & ". Imported number must be " & pNum + 1 & ". ID: " & pLine("ID") & ". Ref vbPA8DFTRKQX")
    '                '    Stop2()
    '                'End If
    '            Else
    '                Continue Do
    '            End If

    '            If CDec2(pLine("InvoiceNumber")) < 0 Then
    '                pLine("InvoiceNumber") = 0 ' prevent negative number
    '            End If

    '            '' special case ni xa
    '            'If pLine("LastUpdated").Trim.ToLower = "Thu Mar 20 2014 18:08:43 GMT+0800 (PHT)".ToLower Then
    '            '    pLine("LastUpdated") = "2014-03-20 18:08:43"
    '            'End If
    '            'If pLine("LastUpdated").Trim.ToLower = "Fri Mar 21 2014 16:56:47 GMT+0800 (PHT)".ToLower Then
    '            '    pLine("LastUpdated") = "2014-03-21 16:56:47"
    '            'End If
    '            'If pLine("LastUpdated").Trim.ToLower = "Sun Mar 23 2014 17:22:33 GMT+0800 (PHT)".ToLower Then
    '            '    pLine("LastUpdated") = "2014-03-23 17:22:33"
    '            'End If

    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Exit Do
    '            End If
    '            If CDec2(pLine("InvoiceNumber") <= 0) _
    '                AndAlso (
    '                    pLine.ContainsKey("SalesOrderNo") = False _
    '                    OrElse CDec2(pLine("SalesOrderNo") <= 0 _
    '                ) _
    '            ) Then

    '                DataImportLog.AddLog("ID " & pLine("ID") & " has an invoice number of " & CDec2(pLine("InvoiceNumber")) & " and will not be imported. This row will be tagged as imported on the device. Ref VZ8KTV7LAU")
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '                Continue Do
    '            End If

    '            pCreator = New TransInvoice.Creator(mBusiness.ProData, mBusiness.TransInvoice)
    '            If ReadyGuid(pLine("ID")) = ReadyGuid("D60D6C64-3685-3BD6-A7DC-BC04254BF2C2") Then
    '                pCreator.ID = pLine("ID") ' for debugging only
    '            End If
    '            pCreator.ID = pLine("ID")
    '            pCreator.InvoiceNumber = pLine("InvoiceNumber")
    '            pCreator.InvoiceDate = ParseDate(pLine("InvoiceDate"))
    '            pCreator.FowstID = pLine("FowstID")
    '            pCreator.DeviceID = pLine("DeviceID")
    '            pCreator.CustomerID = pLine("CustomerID")
    '            If pLine.ContainsKey("TotalAmount") Then
    '                pCreator.TotalAmount = CDec2(pLine("TotalAmount"))
    '            End If
    '            pCreator.DeliveryAddressID = pLine("DeliveryAddressID") ' WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Delivery Address ID")
    '            pCreator.BillingAddressID = pLine("BillingAddressID") ' WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("POS Billing Address ID")
    '            pCreator.LastUpdated = ParseDate(pLine("LastUpdated"))
    '            pMaxSubNumber = mBusiness.TransInvoice.GetMaxSubNumber( _
    '                vInvoiceNumber:=pLine("InvoiceNumber"),
    '                vDeviceID:=pLine("DeviceID"),
    '                vFowstID:=pLine("FowstID"))
    '            If pMaxSubNumber IsNot Nothing Then
    '                pCreator.SubNumber = pMaxSubNumber + 1
    '            End If

    '            pLine.TryGetValue("RefNum", pCreator.RefNum)
    '            pLine.TryGetValue("ReasonForReconstructingData", pCreator.ReasonForReconstructingData)

    '            pCreator.UserID = pLine("UserID")
    '            If pLine.ContainsKey("Remarks") Then
    '                pCreator.Remarks = Truncate(pLine("Remarks"), 999)
    '            End If
    '            If pLine.ContainsKey("SalesOrderNo") AndAlso CStr2(pLine("SalesOrderNo")).Trim <> "" Then
    '                pCreator.SalesOrderNo = pLine("SalesOrderNo")
    '            End If
    '            If pLine.ContainsKey("SalesOrderDate") AndAlso CStr2(pLine("SalesOrderDate")).Trim <> "" Then
    '                pCreator.SalesOrderDate = ParseDate(pLine("SalesOrderDate"))
    '            End If
    '            If pLine.ContainsKey("SalesOrderUserID") AndAlso CStr2(pLine("SalesOrderUserID")).Trim <> "" Then
    '                pCreator.SalesOrderUserID = pLine("SalesOrderUserID")
    '            End If
    '            If pLine.ContainsKey("CustomerTableNo") AndAlso CStr2(pLine("CustomerTableNo")).Trim <> "" Then
    '                pCreator.CustomerTableNo = pLine("CustomerTableNo")
    '            End If
    '            ' additional by janine june 2
    '            If pLine.ContainsKey("IsForGovReport") AndAlso CStr2(pLine("IsForGovReport")).Trim <> "" Then
    '                pCreator.IsForGovReport = pLine("IsForGovReport")
    '            End If
    '            If pLine.ContainsKey("IsTestOnly") AndAlso CStr2(pLine("IsTestOnly")).Trim <> "" Then
    '                pCreator.IsTestOnly = pLine("IsTestOnly")
    '            End If
    '            'additional aug 6
    '            If pLine.ContainsKey("PurchaseOrderNo") AndAlso CStr2(pLine("PurchaseOrderNo")).Trim <> "" Then
    '                pCreator.PurchaseOrderNo = pLine("PurchaseOrderNo")
    '            End If
    '            If pLine.ContainsKey("PurchaseOrderDate") AndAlso CStr2(pLine("PurchaseOrderDate")).Trim <> "" Then
    '                pCreator.PurchaseOrderDate = pLine("PurchaseOrderDate")
    '            End If
    '            If pLine.ContainsKey("PurchaserName") AndAlso CStr2(pLine("PurchaserName")).Trim <> "" Then
    '                pCreator.PurchaserName = pLine("PurchaserName")
    '            End If
    '            'end of additional

    '            'pCreator.ProductID = pLine("ProductID")
    '            'pCreator.SalesRepList = Common.WebRequest.AllData("SalesRep")
    '            'pCreator.ProductSerialList = Common.WebRequest.AllData("ProductSerial")

    '            pIU = pBinder.CreateInsertUpdate(oError) ' not acctually used. just used for checking (see code below)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            Try
    '                mBusiness.ProData.BeginTransaction()
    '                If mBusiness.ProData.TransDepth <= 0 Then
    '                    Throw New Exception("You must be in a transaction. vb2X7W86WQBX")
    '                End If

    '                '' set InvoiceNumber to zero before deleting items
    '                'pSql = "UPDATE TransInvoice" & vbCrLf &
    '                '    " SET InvoiceNumber = 0" &
    '                '    " WHERE ID = " & ReadyString(pLine("ID"))
    '                'mBusiness.ProData.ExecuteSQL(pSql)

    '                ' trigger (TransInvoiceItems_trd) will cause error for deleting items from non-zero invoices 
    '                pSql = "DELETE FROM TransInvoiceItems" & vbCrLf &
    '                    " WHERE InvoiceID = " & ReadyString(pLine("ID"))
    '                mBusiness.ProData.ExecuteSQL(pSql)
    '                If pDtItems Is Nothing Then
    '                    pCreator.Stocks = Nothing
    '                Else
    '                    pCreator.Stocks = New DataView(pDtItems, "InvoiceID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows).ToTable
    '                    If pDtOutItems Is Nothing Then
    '                        pDtOutItems = pCreator.Stocks
    '                    Else
    '                        pDtOutItems.Merge(pCreator.Stocks)
    '                    End If
    '                End If
    '                If pCreator.Save(False, oError) = False Then
    '                    AppendToDebug("vbM823DUDP23 " & oError)
    '                    'mBusiness.ProData.RollbackTransaction(True, True)
    '                    mBusiness.ProData.RollbackTransaction()
    '                    Exit Do
    '                End If

    '                If mBusiness.SystemDefaults.AllowPriceAdjustment Then
    '                    DataImportLog.AddLog("AllowPriceAdjustment = True")
    '                    If HasWholesaleDatabase() Then
    '                        DataImportLog.AddLog("HasWholesaleDatabase = True")
    '                        Dim pInvoiceToOrder As InvoiceToOrder
    '                        pInvoiceToOrder = New InvoiceToOrder(mBusiness.ProData, CreateWholesaleProData)
    '                        pInvoiceToOrder.Execute(pCreator.ID)
    '                    Else
    '                        DataImportLog.AddLog("HasWholesaleDatabase = False")
    '                    End If
    '                End If

    '                mBusiness.TransInvoice.UpdateCounter(pLine("DeviceID"), pLine("FowstID"), pLine("InvoiceNumber"))
    '                If pLine.ContainsKey("SalesOrderNo") Then
    '                    mBusiness.TransInvoice.UpdateSoCounter(pLine("DeviceID"), pLine("FowstID"), pLine("SalesOrderNo"))
    '                End If

    '                If CDec2(pLine("InvoiceNumber")) > 0 Then
    '                    mBusiness.TransInvoice.AddToInventoryLedger(pLine("ID"))
    '                End If

    '                If ImportInvoiceItemSalesReps(vInputZipFilename, pLine("ID"), oError) = False Then
    '                    DataImportLog.AddLog(oError)
    '                    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                    mBusiness.ProData.RollbackTransaction(True, True)
    '                    Exit Do
    '                End If
    '                If ImportInvoiceItemSerials(vInputZipFilename, pLine("ID"), oError) = False Then
    '                    DataImportLog.AddLog(oError)
    '                    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                    mBusiness.ProData.RollbackTransaction(True, True)
    '                    Exit Do
    '                End If
    '                If ImportInvoiceDiscount(vInputZipFilename, pLine("ID"), oError) = False Then
    '                    DataImportLog.AddLog(oError)
    '                    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                    mBusiness.ProData.RollbackTransaction(True, True)
    '                    Exit Do
    '                End If
    '                If ImportInvoiceSeniorCitizenPWDCustomer(vInputZipFilename, pLine("ID"), oError) = False Then 'additional by janine july 26
    '                    DataImportLog.AddLog(oError)
    '                    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                    mBusiness.ProData.RollbackTransaction(True, True)
    '                    Exit Do
    '                End If
    '                If ImportCustomerPayments(vDT:=pDtCustomerPayments,
    '                    vBinder:=pBinderCustomerPayments,
    '                    rDtOut:=pDtOutCustomerPayments,
    '                    rDtOutInvoices:=pDtCustomerPaymentInvoices,
    '                    oError:=oError,
    '                    vInvoiceID:=pLine("ID"),
    '                    vDtCustomerPaymentInvoices:=pDtCustomerPaymentInvoices,
    '                    vBinderCustomerPaymentInvoices:=pBinderCustomerPaymentInvoices,
    '                    vDtCustomerPaymentDebitCards:=pDtCustomerPaymentDebitCards,
    '                    vBinderCustomerPaymentDebitCards:=pBinderCustomerPaymentDebitCards,
    '                    vDtCustomerPaymentCreditCards:=pDtCustomerPaymentCreditCards,
    '                    vBinderCustomerPaymentCreditCards:=pBinderCustomerPaymentCreditCards) = False Then

    '                    DataImportLog.AddLog(oError)
    '                    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                    mBusiness.ProData.RollbackTransaction(True, True)
    '                    Exit Do
    '                End If

    '                mBusiness.ProData.CommitTransaction()
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                mBusiness.ProData.RollbackTransaction(True, True)
    '                If pDtOut.Rows.Count > 0 Then
    '                    Exit Do
    '                Else
    '                    AppendToDebug("vbLWDB6NFYCA", , ex, True)
    '                    Throw
    '                End If
    '            End Try
    '        Loop

    '        If pDtOut.Rows.Count = 0 Then
    '            If CStr2(oError).Trim <> "" Then
    '                Return False
    '            End If
    '        End If

    '        rZipOut.AddTableCsv(pDtOut, "TransInvoice")
    '        If pDtOutCustomerPayments IsNot Nothing Then
    '            rZipOut.AddTableCsv(pDtOutCustomerPayments, "TransCustomerPayment")
    '        End If

    '        ' TransInvoiceItems is not taggerd anymore. June 12, 2014
    '        'If pDtOutItems IsNot Nothing _
    '        '    AndAlso pDtOutItems.Rows.Count > 0 Then

    '        '    pDtOutItems = New DataView(pDtOutItems, "", "", DataViewRowState.CurrentRows).ToTable(True, "ID")
    '        '    rZipOut.AddTableCsv(pDtOutItems, "TransInvoiceItems")
    '        'End If

    '        If pDataImportLogAlreadyImported IsNot Nothing Then
    '            pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
    '        End If

    '        Return True

    '    End Function
    '    Private Function HasWholesaleDatabase() As Boolean

    '        Dim pList As List(Of String)

    '        pList = mBusiness.ProData.GetDatabases(True)
    '        Dim p = From pStr As String In pList
    '            Where pStr = "wholesale"

    '        Return CType(p, IEnumerable(Of String)).Count > 0

    '    End Function

    '    Private Function CreateWholesaleProData() As ProData

    '        Return ProData.CreateFromConnectionInfo( _
    '            vServer:=Common.ConnectionInfo.Server, _
    '            vDatabase:="wholesale", _
    '            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity, _
    '            vUsername:=Common.ConnectionInfo.Username, _
    '            vPassword:=Common.ConnectionInfo.Password, _
    '            vNoProgressBar:=True)

    '    End Function

    '    ''' <summary>
    '    ''' copy data from MobizDUC.TransInvoice to Wholesale.TransSalesOrder
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Class InvoiceToOrder
    '        ReadOnly mDbSource As ProData
    '        ReadOnly mDbDestination As ProData
    '        ReadOnly mBusinessSource As Business

    '        Public Sub New(ByVal vDbSource As ProData, ByVal vDbDestination As ProData)

    '            mDbSource = vDbSource
    '            mBusinessSource = New Business(mDbSource)
    '            mDbDestination = vDbDestination

    '        End Sub

    '        Public Function Execute(Optional ByVal vInvoiceID As String = Nothing, Optional ByRef oMessage As String = Nothing) As Boolean

    '            Dim pBusiness As Business
    '            Dim pCreator As TransSalesOrder.Creator
    '            Dim pDtUnexported As DataTable
    '            Dim pDS As DataSet
    '            Dim pDrSource As DataRow
    '            Dim pItem As TransSalesOrder.Creator.Item
    '            Dim pSql As String
    '            Dim pCheckPriceRequest As WholesaleCheckPriceRequest
    '            Dim pDrOrder As DataRow


    '            oMessage = Nothing
    '            pDtUnexported = GetUnexportedSalesOrders(vInvoiceID)
    '            If pDtUnexported.Rows.Count = 0 Then
    '                Return True
    '            End If
    '            pBusiness = New Business(mDbDestination)
    '            For Each pDrUnexported As DataRow In pDtUnexported.Rows
    '                pDrOrder = pBusiness.TransSalesOrder.LegacyGetByMobizInvoiceID(CStr2(pDrUnexported("ID")))
    '                If pDrOrder IsNot Nothing Then
    '                    If CDec2(pDrOrder("IsHold")) = 0 Then
    '                        Continue For
    '                    End If
    '                End If

    '                pDS = mBusinessSource.TransInvoice.GetReportData("WHERE TransInvoice.ID = " & ReadyString(pDrUnexported("ID")), False)
    '                If pDS.Tables("Invoices").Rows.Count = 0 Then
    '                    ' invoice not found
    '                    Continue For
    '                End If
    '                pDrSource = pDS.Tables("Invoices").Rows(0)
    '                pCreator = pBusiness.TransSalesOrder.CreateCreator()
    '                pCreator.DeliveryDate = Now
    '                pCreator.BranchID = pBusiness.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 5)
    '                'pCreator.SubscriberBranchID = WebSession.Instance.SubscriberOrganizationBranchID
    '                pCreator.DeviceID = CStr2(pDrSource("DeviceID"))
    '                pCreator.CustomerID = CStr2(pDrSource("CustomerID"))
    '                pCreator.CustomerAddressID = CStr2(pDrSource("BillingAddressID"))
    '                If CStr2(pCreator.CustomerAddressID) = "" Then
    '                    oMessage = "Customer, " & pCreator.CustomerID & " has no billing address"
    '                    Return False
    '                End If
    '                pCreator.DeliverToID = CStr2(pDrSource("DeliveryAddressID"))
    '                If CStr2(pCreator.DeliverToID) = "" Then
    '                    oMessage = "Customer, " & pCreator.CustomerID & " has no delivery address"
    '                    Return False
    '                End If
    '                If CStr2(pDrSource("Remarks")) <> "" Then
    '                    pCreator.Remarks = Truncate(pDrSource("Remarks"), 999)
    '                End If
    '                pCreator.CusRepresentativeID = pBusiness.LibraryCustomers.LegacyGetOrAddContactPerson(pCreator.CustomerID)

    '                pCreator.IsHold = True
    '                pCreator.IsForApproval = True
    '                pCreator.DontAutoInvoice = False
    '                pCreator.MobizInvoiceID = CStr2(pDrUnexported("ID"))

    '                pCreator.UserID = CDec2(pDrSource("DUCNumericID"))
    '                For Each pDrItem As DataRow In pDS.Tables("Stocks").Rows
    '                    pItem = New TransSalesOrder.Creator.Item
    '                    pItem.StockID = CStr2(pDrItem("ProductID"))
    '                    pItem.UnitMeasure = pDrItem("UnitMeasure")
    '                    pItem.PriceLevelID = CStr2(pDrItem("PriceLevelID"))
    '                    pItem.ActualQuantity = pDrItem("ActualQuantity")
    '                    pItem.UnitPrice = pDrItem("UnitPrice")
    '                    If CDec2(pDrItem("RequestedUnitPrice")) > 0 Then
    '                        pItem.RequestedUnitPrice = CDec(pDrItem("RequestedUnitPrice"))
    '                    End If
    '                    pItem.OriginalUnitPrice = pDrItem("UnitPrice")
    '                    pItem.UnitPriceWOTax = pItem.UnitPrice
    '                    pItem.Quantity = CDec2(pDrItem("Quantity"))

    '                    pItem.SubTotal = pItem.UnitPrice * pItem.Quantity

    '                    '''''''''''''''
    '                    'pItem.SubTotal = pItem.UnitPrice * pItem.Quantity 'commented by janine
    '                    pItem.Cost = pBusiness.LibraryStockLegacy.GetLandedCost(pItem.StockID) '* pItem.ActualQuantity
    '                    pItem.CancellationPeriod = CDec2(pBusiness.SystemDefaults.GetOrAddSystemDefault("BackOrder Cancellation Period", 15))

    '                    pItem.TermDiscount = 0 'pStockDiscount
    '                    pItem.DiscountIfPaidWithIn = 0 'Math.Min(CDec2(pCustomerTerm), pStockDiscountIfPaidWithIn)
    '                    pItem.NetDue = 0 'If(pUseCustomerTerm, pCustomerTerm, pStockTerm)

    '                    If IsNumeric(pDrItem("LineNumber")) Then
    '                        pItem.LineNumber = pDrItem("LineNumber")
    '                    Else
    '                        pItem.LineNumber = 1
    '                    End If
    '                    pCreator.Items.Add(pItem)
    '                Next pDrItem
    '                If pDrOrder Is Nothing Then
    '                    pCreator.Save(True)
    '                Else
    '                    pCreator.ID = CStr2(pDrOrder("ID"))
    '                    pCreator.Save(False)
    '                End If

    '                Dim pIU As InsertUpdate

    '                pIU = New InsertUpdate("TransInvoice")
    '                pIU("SalesOrderID") = ReadyString(pCreator.ID)
    '                pSql = pIU.GetUpdate("ID = " & ReadyString(pDrUnexported("ID")))
    '                mDbSource.ExecuteSQL(pSql)

    '                pCheckPriceRequest = New WholesaleCheckPriceRequest(pBusiness, pCreator.ID)
    '                Debug.Print("BNWD7FRCU2 pCheckPriceRequest.Result: " & pCheckPriceRequest.Result)
    '                Debug.Print("DG5GQDSJH6 pCheckPriceRequest.Message: " & pCheckPriceRequest.Message)
    '            Next pDrUnexported

    '            Return True

    '        End Function

    '        Private Function GetUnexportedSalesOrders(Optional ByVal vInvoiceID As String = Nothing) As DataTable

    '            Dim pDT As DataTable
    '            Dim pSql As String

    '            If CStr2(vInvoiceID) <> "" Then
    '                pSql = " ID = " & ReadyString(vInvoiceID)
    '            Else
    '                pSql = "(SalesOrderID IS NULL" &
    '                " AND SalesOrderNo IS NOT NULL)"
    '            End If
    '            pSql = "SELECT ID" &
    '                " FROM TransInvoice" &
    '                " WHERE " &
    '                pSql &
    '                " ORDER BY LastUpdated"
    '            pDT = mDbSource.Execute(pSql)

    '            Return pDT

    '        End Function
    '    End Class ' InvoiceToOrder

    '    Private Function CreateInvoiceItemsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDR As DataRow
    '        Dim pLine As Dictionary(Of String, String)

    '        ' BEGIN get SKU data from database
    '        'Dim pDtStocks As DataTable
    '        'Dim pDvStocks As DataView
    '        'Dim pDrvStock As DataRowView
    '        ' END get SKU data from database

    '        DataImportLog.AddLog("Import from TransInvoiceItems")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItems")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("PriceLevelID", True, SqlDbType.UniqueIdentifier)
    '            .Add("TaxSales", False, SqlDbType.Money)
    '            .Add("SubTotal", True, SqlDbType.Money)
    '            .Add("LineNumber", True, SqlDbType.Int)
    '            .Add("OrderedQuantity", True, SqlDbType.Decimal) ' additional by janine sept 6 2014 for sales order taker
    '            .Add("Quantity", True, SqlDbType.Decimal)
    '            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
    '            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
    '            .Add("ActualQuantity", True, SqlDbType.Int)
    '            .Add("UnitPrice", True, SqlDbType.Money)
    '            .Add("OrignalUnitPrice", False, SqlDbType.Money) ' required but be optional for backwards compatibility
    '            .Add("TaxAmount", False, SqlDbType.Money)
    '            .Add("TaxPercent", False, SqlDbType.Money)
    '            .Add("UnitMeasure", True, SqlDbType.VarChar)

    '            .Add("DiscountAmount", False, SqlDbType.Money)
    '            .Add("DiscountSales", False, SqlDbType.Money)
    '            .Add("RequestedUnitPrice", False, SqlDbType.Money)
    '        End With

    '        pDtOut = New DataTable
    '        With pDtOut.Columns
    '            .Add("ID")
    '            .Add("InvoiceID")
    '            .Add("ProductID")
    '            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
    '            .Add("Quantity", GetType(Decimal))
    '            .Add("ActualQuantity", GetType(Integer))
    '            .Add("UnitMeasure")
    '            .Add("TermDiscount", GetType(Decimal))
    '            .Add("DiscountIfPaidWithIn", GetType(Decimal))
    '            .Add("NetDue")
    '            .Add("UnitPrice", GetType(Decimal))
    '            .Add("OriginalUnitPrice", GetType(Decimal))
    '            .Add("Subtotal", GetType(Decimal))
    '            .Add("Cost", GetType(Decimal))
    '            .Add("PriceLevelID")
    '            .Add("Dimension")
    '            .Add("IsReplacement", GetType(Boolean))
    '            .Add("IsHasReplacement", GetType(Boolean))
    '            .Add("IsHasBackOrder", GetType(Boolean))
    '            .Add("RefStockID")
    '            .Add("UnitGP", GetType(Decimal))
    '            .Add("UnitGPM", GetType(Decimal))
    '            .Add("LineGP", GetType(Decimal))
    '            .Add("LineGPM", GetType(Decimal))
    '            .Add("LineCost", GetType(Decimal))
    '            .Add("LineNumber", GetType(Integer))
    '            .Add("TaxPercent", GetType(Decimal))
    '            .Add("TaxAmount", GetType(Decimal))
    '            .Add("TaxSales", GetType(Decimal))
    '            '.Add("SalesRep", GetType(Object))
    '            .Add("DiscountAmount", GetType(Decimal))
    '            .Add("DiscountSales", GetType(Decimal))
    '            .Add("RequestedUnitPrice", GetType(Decimal))
    '        End With

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return Nothing
    '            End If
    '            pDR = pBinder.CreateDataRow(pDtOut)

    '            ' BEGIN get SKU data from database
    '            'pDtStocks = mBusiness.LibraryStock.PriceTable(OrgID, pDR("ProductID"))
    '            'pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pDR("ActualQuantity")) &
    '            '    " AND PriceLevelID = " & ReadyString(pDR("PriceLevelID")), "", DataViewRowState.CurrentRows)
    '            'If pDvStocks.Count > 0 Then
    '            '    pDrvStock = pDvStocks(0)
    '            ' END get SKU data from database

    '            If CDec2(pDR("Quantity")) >= 0 Then
    '                'pDR("ActualQuantity") = pDrvStock("Quantity")
    '                'pDR("UnitMeasure") = pDrvStock("UnitMeasure")
    '                'pDR("UnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
    '                ' check test
    '                '    If Math.Abs(pDR("UnitPrice") - pLine("UnitPrice")) > 0.01 Then
    '                '       Throw New Exception("Conflicting values in unit price vbQV7L2A8VEY")
    '                '   End If
    '                'pDR("OriginalUnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
    '                pDR("Cost") = mBusiness.LibraryStock.GetLandedCost(pDR("ProductID"), OrgID) * pDR("ActualQuantity")
    '                'If ReadyGuid(pDrvStock("PriceLevelID")) <> pRetailID Then
    '                '    'todo: add security
    '                '    'If WebSession.Instance.UserAccess.PointOfSales_PriceLevelMode.IsEnable _
    '                '    '    OrElse WebSession.Instance.UserAccess.PointOfSales_UnlimitedUserMode.IsEnable Then

    '                '    'Else
    '                '    '    Common.WebRequest.ProData.RollbackTransaction()
    '                '    '    Return JsonDictionary.Serialize("Error", "You are not authorized to set price level other than retail")
    '                '    'End If
    '                'End If
    '                'pDR("PriceLevelID") = CStr2(pDrvStock("PriceLevelID"))
    '                pDR("Dimension") = ""
    '                pDR("RequestedUnitPrice") = pDR("RequestedUnitPrice")
    '                pDR("IsReplacement") = False
    '                pDR("IsHasReplacement") = False
    '                pDR("IsHasBackOrder") = False
    '                pDR("RefStockID") = ""

    '                Dim pUnitGp As Decimal
    '                Dim pUnitGpm As Double
    '                Dim pLineGp As Decimal
    '                Dim pLineGpm As Decimal
    '                Dim pLandedCost As Decimal

    '                'additional by janine sept 6 
    '                If (CDec2(pDR("Quantity")) > 0) Then
    '                    mBusiness.LibraryStock.GetProfitability(
    '                    vStockID:=CStr2(pDR("ProductID")),
    '                    vQty:=pDR("Quantity"),
    '                    vUnitQty:=pDR("ActualQuantity"),
    '                    vUnitPrice:=pDR("UnitPrice"),
    '                    oUnitGP:=pUnitGp,
    '                    oUnitGPM:=pUnitGpm,
    '                    oLineGP:=pLineGp,
    '                    oLineGPM:=pLineGpm,
    '                    oLandedCost:=pLandedCost,
    '                    vHistoricCost:=0,
    '                    vOrgID:=OrgID)

    '                Else
    '                    mBusiness.LibraryStock.GetProfitability(
    '                    vStockID:=CStr2(pDR("ProductID")),
    '                    vQty:=pDR("OrderedQuantity"),
    '                    vUnitQty:=pDR("ActualQuantity"),
    '                    vUnitPrice:=pDR("UnitPrice"),
    '                    oUnitGP:=pUnitGp,
    '                    oUnitGPM:=pUnitGpm,
    '                    oLineGP:=pLineGp,
    '                    oLineGPM:=pLineGpm,
    '                    oLandedCost:=pLandedCost,
    '                    vHistoricCost:=0,
    '                    vOrgID:=OrgID)
    '                End If
    '                '''''''''''''''''''''''''''''''''''

    '                pDR("UnitGP") = pUnitGp
    '                pDR("UnitGPM") = pUnitGpm
    '                pDR("LineGP") = pLineGp
    '                pDR("LineGPM") = pLineGpm
    '                pDR("LineCost") = pLandedCost * CDec2(pDR("Quantity")) * CDec2(pDR("ActualQuantity"))

    '                'additional by janine sept 6 
    '                If (CDec2(pDR("Quantity")) > 0) Then
    '                    pDR("Subtotal") = pDR("Quantity") * pDR("UnitPrice")
    '                Else
    '                    pDR("Subtotal") = pDR("OrderedQuantity") * pDR("UnitPrice")
    '                End If
    '                '''''''''''''''''
    '                'pDR("Subtotal") = pDR("Quantity") * pDR("UnitPrice") 'commented by janine
    '                pDR("TaxPercent") = 12 ' todo
    '                pDR("TaxSales") = RoundDown(pDR("SubTotal") / (1 + pDR("TaxPercent") / 100), 4)
    '                pDR("TaxAmount") = pDR("SubTotal") - pDR("TaxSales")
    '            Else
    '                oError = "No matching unit quantity and price level"
    '                Return Nothing
    '            End If

    '            ' BEGIN get SKU data from database
    '            'End If
    '            ' END get SKU data from database

    '            pDtOut.Rows.Add(pDR)
    '        Loop
    '        pDtOut.Columns("ProductID").ColumnName = "StockID"

    '        Return pDtOut

    '    End Function

    '    Private Function ImportInvoiceItemSalesReps(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
    '        ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow

    '        oError = ""

    '        DataImportLog.AddLog("Import from TransInvoiceItemSalesReps")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemSalesReps")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
    '            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pSql = "DELETE FROM TransInvoiceItemSalesReps" & vbCrLf &
    '            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return False
    '            End If
    '            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
    '                Continue Do
    '            End If
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "TransInvoiceItemSalesReps"
    '            Try
    '                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            pDrOut = pDtOut.NewRow
    '            pDrOut("ID") = pLine("ID")
    '            pDtOut.Rows.Add(pDrOut)
    '        Loop

    '        Return True

    '    End Function

    '    Private Function ImportInvoiceItemSerials(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
    '        ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow

    '        oError = ""

    '        DataImportLog.AddLog("Import from TransInvoiceItemSerials")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemSerials")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("SerialNumber", True, SqlDbType.VarChar)
    '            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pSql = "DELETE FROM TransInvoiceItemSerials" & vbCrLf &
    '            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return False
    '            End If
    '            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
    '                Continue Do
    '            End If
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "TransInvoiceItemSerials"
    '            Try
    '                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            pDrOut = pDtOut.NewRow
    '            pDrOut("ID") = pLine("ID")
    '            pDtOut.Rows.Add(pDrOut)
    '        Loop

    '        Return True

    '    End Function

    '    Private Function ImportInvoiceDiscount(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
    '        ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow

    '        oError = ""

    '        DataImportLog.AddLog("Import from TransInvoiceDiscount")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceDiscount")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
    '            .Add("DiscountPercentage", True, SqlDbType.Float)
    '            .Add("DiscountTypeID", True, SqlDbType.UniqueIdentifier)
    '            .Add("DiscountRemarks", False, SqlDbType.VarChar)
    '            .Add("NoOfPeople", False, SqlDbType.Int) ''additional by janine july 26
    '            .Add("NoOfSeniorCitizens", False, SqlDbType.Int) ''additional by janine july 26
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pSql = "DELETE FROM TransInvoiceDiscount" & vbCrLf &
    '            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return False
    '            End If
    '            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
    '                Continue Do
    '            End If
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "TransInvoiceDiscount"
    '            Try
    '                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            pDrOut = pDtOut.NewRow
    '            pDrOut("ID") = pLine("ID")
    '            pDtOut.Rows.Add(pDrOut)
    '        Loop

    '        Return True

    '    End Function

    '    ''additional by janine july 26
    '    Private Function ImportInvoiceSeniorCitizenPWDCustomer(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
    '        ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow

    '        oError = ""

    '        DataImportLog.AddLog("Import from TransInvoiceSeniorCitizenPWDCustomer")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceSeniorCitizenPWDCustomer")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
    '            .Add("CustomerID", True, SqlDbType.UniqueIdentifier)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pSql = "DELETE FROM TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
    '            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return False
    '            End If
    '            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
    '                Continue Do
    '            End If
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "TransInvoiceSeniorCitizenPWDCustomer"
    '            Try
    '                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            pDrOut = pDtOut.NewRow
    '            pDrOut("ID") = pLine("ID")
    '            pDtOut.Rows.Add(pDrOut)
    '        Loop

    '        Return True

    '    End Function
    '    'end of additional

    '    Private Function CreateCustomerPaymentsDataTable(ByVal vInputZipFilename As String, _
    '        ByRef oBinder As DictionaryBinder,
    '        ByRef oError As String) As DataTable

    '        Dim pCsvReader As CsvReader
    '        Dim pDtOut As DataTable
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate ' used for verfication only

    '        DataImportLog.AddLog("Import from TransCustomerPayment")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPayment")
    '        oBinder = New DictionaryBinder
    '        With oBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("CustomerID", True, SqlDbType.UniqueIdentifier)
    '            .Add("UserID", True, SqlDbType.UniqueIdentifier)
    '            .Add("CheckPaymentAmount", True, SqlDbType.Money)
    '            .Add("AmountChange", True, SqlDbType.Money)
    '            .Add("CashAmount", True, SqlDbType.Money)
    '            .Add("SRVAmount", True, SqlDbType.Money)
    '            .Add("CreditCardAmount", True, SqlDbType.Money)
    '            .Add("DiscountAmount", True, SqlDbType.Money)
    '            .Add("DebitCardAmount", True, SqlDbType.Money)
    '            .Add("TotalAmountDue", True, SqlDbType.Money)
    '            .Add("CustomerChangeCash", True, SqlDbType.Money)
    '            .Add("ReceiptType", True, SqlDbType.VarChar)
    '            .Add("ReceiptNumber", True, SqlDbType.VarChar)
    '            .Add("PaymentDate", True, SqlDbType.DateTime)
    '        End With
    '        pDtOut = oBinder.CreateDataTable

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            oBinder.Source = pLine
    '            pIU = oBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return Nothing
    '            End If
    '            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
    '        Loop

    '        Return pDtOut

    '    End Function

    '    Private Function ImportCustomerPayments(ByVal vDT As DataTable,
    '        ByVal vBinder As DictionaryBinder, _
    '        ByRef rDtOut As DataTable,
    '        ByRef rDtOutInvoices As DataTable,
    '        ByRef oError As String,
    '        ByVal vInvoiceID As String,
    '        ByVal vDtCustomerPaymentInvoices As DataTable,
    '        ByVal vBinderCustomerPaymentInvoices As DictionaryBinder,
    '        ByVal vDtCustomerPaymentDebitCards As DataTable,
    '        ByVal vBinderCustomerPaymentDebitCards As DictionaryBinder,
    '        ByVal vDtCustomerPaymentCreditCards As DataTable,
    '        ByVal vBinderCustomerPaymentCreditCards As DictionaryBinder) As Boolean

    '        Dim pDvCustomerPayments As DataView
    '        Dim pDrOut As DataRow
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pDvCustomerPaymentInvoices As DataView
    '        Dim pDtCustomerPaymentsUnique As DataTable
    '        Dim pDtFieldValues As DataTable
    '        Dim pIds As String

    '        DataImportLog.AddLog("Import from TransCustomerPayment")
    '        If rDtOut Is Nothing Then
    '            rDtOut = New DataTable
    '            rDtOut.Columns.Add("ID")
    '        End If

    '        pDvCustomerPaymentInvoices = New DataView(vDtCustomerPaymentInvoices, "InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
    '        If pDvCustomerPaymentInvoices.Count = 0 Then
    '            ' this invoice has no payment
    '            ' valid during on order taker
    '            Return True
    '        End If
    '        pDtCustomerPaymentsUnique = pDvCustomerPaymentInvoices.ToTable(True, {"CustomerPaymentID"})
    '        pDtFieldValues = ExtractFieldValues(pDtCustomerPaymentsUnique, "CustomerPaymentID", Integer.MaxValue)
    '        If pDtFieldValues.Rows.Count = 0 Then
    '            ' no matching customer payment ID has been found.
    '            ' todo: trap this data error
    '            Stop2()
    '            Return True
    '        End If
    '        pIds = pDtFieldValues.Rows(0)(0)

    '        pDvCustomerPayments = New DataView(vDT, "ID IN (" & pIds & ")", "", DataViewRowState.CurrentRows)
    '        For Each pDrvIn As DataRowView In pDvCustomerPayments
    '            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
    '            If pIU Is Nothing Then
    '                Continue For
    '            End If
    '            pDvCustomerPaymentInvoices = New DataView(vDtCustomerPaymentInvoices, "CustomerPaymentID = " & ReadyString(pDrvIn("ID")) & " AND InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
    '            If pDvCustomerPaymentInvoices.Count = 0 Then
    '                ' you should panic here
    '                Stop2()
    '            End If

    '            pIU.TableName = "TransCustomerPayment"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = rDtOut.NewRow
    '                pDrOut("ID") = CStr2(pDrvIn("ID"))
    '                rDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try

    '            If ImportCustomerPaymentInvoices(
    '                vDT:=pDvCustomerPaymentInvoices.ToTable(),
    '                vBinder:=vBinderCustomerPaymentInvoices,
    '                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
    '                oError:=oError,
    '                rDtOut:=rDtOutInvoices) = False Then

    '                DataImportLog.AddLog(oError)
    '                DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                Return False
    '            End If

    '            If ImportCustomerPaymentDebitCard(
    '                vDT:=vDtCustomerPaymentDebitCards,
    '                vBinder:=vBinderCustomerPaymentDebitCards,
    '                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
    '                oError:=oError,
    '                rDtOut:=rDtOutInvoices) = False Then

    '                DataImportLog.AddLog(oError)
    '                DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                Return False
    '            End If

    '            If ImportCustomerPaymentCreditCard(
    '                vDT:=vDtCustomerPaymentCreditCards,
    '                vBinder:=vBinderCustomerPaymentCreditCards,
    '                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
    '                oError:=oError,
    '                rDtOut:=rDtOutInvoices) = False Then

    '                DataImportLog.AddLog(oError)
    '                DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                Return False
    '            End If
    '            'If ImportCustomerPaymentCreditCard(vInputZipFilename, pLine("ID"), oError) = False Then
    '            '    DataImportLog.AddLog(oError)
    '            '    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            '    Return False
    '            'End If
    '            'If ImportCustomerPaymentDebitCard(vInputZipFilename, pLine("ID"), oError) = False Then
    '            '    DataImportLog.AddLog(oError)
    '            '    DataImportLog.Save(DataImportLog.eResultType.Fail)
    '            '    Return False
    '            'End If
    '        Next pDrvIn

    '        Return True

    '    End Function

    '    Private Shared Function CreateCustomerPaymentInvoicesDataTable(ByVal vInputZipFilename As String, _
    '        ByRef oBinder As DictionaryBinder,
    '        ByRef oError As String) As DataTable

    '        Dim pCsvReader As CsvReader
    '        Dim pDtOut As DataTable
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate ' used for verfication only

    '        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentInvoices")
    '        oBinder = New DictionaryBinder
    '        With oBinder.Items
    '            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
    '            .Add("Balance", True, SqlDbType.Money)
    '            .Add("DiscountAmount", True, SqlDbType.Money)
    '            .Add("InvoiceAmount", True, SqlDbType.Money)
    '            .Add("AmountPaid", True, SqlDbType.Money)
    '        End With
    '        pDtOut = oBinder.CreateDataTable

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            oBinder.Source = pLine
    '            pIU = oBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return Nothing
    '            End If
    '            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
    '        Loop

    '        Return pDtOut

    '    End Function

    '    Private Function ImportCustomerPaymentInvoices(ByVal vDT As DataTable,
    '        ByVal vBinder As DictionaryBinder,
    '        ByVal vCustomerPaymentID As String,
    '        ByRef rDtOut As DataTable,
    '        ByRef oError As String) As Boolean

    '        Dim pDrOut As DataRow
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pDvCustomerPaymentInvoices As DataView

    '        If rDtOut Is Nothing Then
    '            rDtOut = New DataTable
    '            rDtOut.Columns.Add("ID")
    '        End If

    '        pDvCustomerPaymentInvoices = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

    '        ' ToOptimize: delete only when needed
    '        pSql = "DELETE FROM TransCustomerPaymentInvoices" & vbCrLf &
    '            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        For Each pDrvIn As DataRowView In pDvCustomerPaymentInvoices
    '            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
    '            If pIU Is Nothing Then
    '                Return False
    '            End If
    '            pIU.TableName = "TransCustomerPaymentInvoices"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = rDtOut.NewRow
    '                pDrOut("ID") = ReadyString(pDrvIn("ID"))
    '                rDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                oError = "An error occured while inserting data: " & ex.Message
    '                Return False
    '            End Try
    '        Next pDrvIn

    '        Return True

    '    End Function

    '    Private Shared Function CreateCustomerPaymentDebitCardDataTable(ByVal vInputZipFilename As String, _
    '        ByRef oBinder As DictionaryBinder,
    '        ByRef oError As String) As DataTable

    '        Dim pCsvReader As CsvReader
    '        Dim pDtOut As DataTable
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate ' used for verfication only

    '        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentDebitCard")
    '        oBinder = New DictionaryBinder
    '        With oBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
    '            .Add("DebitCardType", False, SqlDbType.VarChar)
    '            .Add("DebitCardBankID", False, SqlDbType.UniqueIdentifier)
    '            .Add("DebitCardBankName", False, SqlDbType.VarChar)
    '            .Add("IsBankFromOrg", True, SqlDbType.Bit)
    '            .Add("BankCountryID", False, SqlDbType.UniqueIdentifier)
    '            .Add("HolderLastName", False, SqlDbType.VarChar)
    '            .Add("HolderFirstName", False, SqlDbType.VarChar)
    '            .Add("HolderMiddleName", False, SqlDbType.VarChar)
    '            .Add("HolderGenerationalSuffix", False, SqlDbType.VarChar)
    '            .Add("DebitCardNumber", True, SqlDbType.VarChar)
    '            .Add("DebitCardIssuanceDate", False, SqlDbType.Date)
    '            .Add("DebitCardExpirationDate", False, SqlDbType.DateTime)
    '            .Add("ApprovalNo", False, SqlDbType.VarChar)
    '            .Add("NoOfInstallmentPayments", False, SqlDbType.Int)
    '            .Add("DebitCardSurcharge", False, SqlDbType.Float)
    '            .Add("DebitCardAmount", False, SqlDbType.Money)
    '        End With
    '        pDtOut = oBinder.CreateDataTable

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            oBinder.Source = pLine
    '            pIU = oBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return Nothing
    '            End If
    '            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
    '        Loop

    '        Return pDtOut

    '    End Function

    '    Private Function ImportCustomerPaymentDebitCard(ByVal vDT As DataTable,
    '        ByVal vBinder As DictionaryBinder,
    '        ByVal vCustomerPaymentID As String,
    '        ByRef rDtOut As DataTable,
    '        ByRef oError As String) As Boolean

    '        Dim pDrOut As DataRow
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pDvDebitCard As DataView

    '        If rDtOut Is Nothing Then
    '            rDtOut = New DataTable
    '            rDtOut.Columns.Add("ID")
    '        End If
    '        pDvDebitCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

    '        pSql = "DELETE FROM TransCustomerPaymentDebitCard" & vbCrLf &
    '            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        For Each pDrvIn As DataRowView In pDvDebitCard
    '            pIU = vBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return False
    '            End If
    '            pIU.TableName = "TransCustomerPaymentDebitCard"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = rDtOut.NewRow
    '                pDrOut("ID") = CStr2(pDrvIn("ID"))
    '                rDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                oError = "An error occured while inserting data: " & ex.Message
    '                Return False
    '            End Try
    '        Next pDrvIn

    '        Return True

    '    End Function

    '    Private Shared Function CreateCustomerPaymentCreditCardDataTable(ByVal vInputZipFilename As String, _
    '        ByRef oBinder As DictionaryBinder,
    '        ByRef oError As String) As DataTable

    '        Dim pCsvReader As CsvReader
    '        Dim pDtOut As DataTable
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate ' used for verfication only

    '        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentCreditCard")
    '        oBinder = New DictionaryBinder
    '        With oBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
    '            .Add("CreditCardType", False, SqlDbType.VarChar)
    '            .Add("CreditCardBankID", False, SqlDbType.UniqueIdentifier)
    '            .Add("CreditCardBankName", False, SqlDbType.VarChar)
    '            .Add("IsBankFromOrg", True, SqlDbType.Bit)
    '            .Add("BankCountryID", False, SqlDbType.UniqueIdentifier)
    '            .Add("HolderLastName", False, SqlDbType.VarChar)
    '            .Add("HolderFirstName", False, SqlDbType.VarChar)
    '            .Add("HolderMiddleName", False, SqlDbType.VarChar)
    '            .Add("HolderGenerationalSuffix", False, SqlDbType.VarChar)
    '            .Add("CreditCardNumber", True, SqlDbType.VarChar)
    '            .Add("CreditCardIssuanceDate", False, SqlDbType.Date)
    '            .Add("CreditCardExpirationDate", False, SqlDbType.DateTime)
    '            .Add("ApprovalNo", False, SqlDbType.VarChar)
    '            .Add("NoOfInstallmentPayments", False, SqlDbType.Int)
    '            .Add("CreditCardSurcharge", False, SqlDbType.Float)
    '            .Add("CreditCardAmount", False, SqlDbType.Money)
    '        End With
    '        pDtOut = oBinder.CreateDataTable

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            oBinder.Source = pLine
    '            pIU = oBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return Nothing
    '            End If
    '            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
    '        Loop

    '        Return pDtOut

    '    End Function

    '    Private Function ImportCustomerPaymentCreditCard(ByVal vDT As DataTable,
    '        ByVal vBinder As DictionaryBinder,
    '        ByVal vCustomerPaymentID As String,
    '        ByRef rDtOut As DataTable,
    '        ByRef oError As String) As Boolean

    '        Dim pDrOut As DataRow
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String
    '        Dim pDvCreditCard As DataView

    '        If rDtOut Is Nothing Then
    '            rDtOut = New DataTable
    '            rDtOut.Columns.Add("ID")
    '        End If
    '        pDvCreditCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

    '        If False Then
    '            Throw New Exception("Test exception vbAKKJTNVUVM")
    '        End If

    '        pSql = "DELETE FROM TransCustomerPaymentCreditCard" & vbCrLf &
    '            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        For Each pDrvIn As DataRowView In pDvCreditCard
    '            pIU = vBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return False
    '            End If
    '            pIU.TableName = "TransCustomerPaymentCreditCard"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
    '            Try
    '                If False Then
    '                    Throw New Exception("Test exception vb9F37BASVPU")
    '                End If
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = rDtOut.NewRow
    '                pDrOut("ID") = CStr2(pDrvIn("ID"))
    '                rDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                oError = "An error occured while inserting data: " & ex.Message
    '                Return False
    '            End Try
    '        Next pDrvIn

    '        Return True

    '    End Function

    '    ' additional by janine

    '    Private Function ImportLibraryOrganization(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
    '        ByVal vOrgID As String, ByVal vUuid As String,
    '        ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String

    '        DataImportLog.AddLog("Import from LibraryOrganization")
    '        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganization")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("IsOrganization", False, SqlDbType.Bit)
    '            .Add("PersonTitleID", False, SqlDbType.UniqueIdentifier)
    '            .Add("PersonSurname", False, SqlDbType.VarChar)
    '            .Add("PersonFirstName", False, SqlDbType.VarChar)
    '            .Add("PersonMiddleName", False, SqlDbType.VarChar)
    '            .Add("PersonSuffixName", False, SqlDbType.VarChar)
    '            .Add("PersonDateOfBirth", False, SqlDbType.Date)
    '            .Add("PersonMotherMaidenSurname", False, SqlDbType.VarChar)
    '            .Add("PersonMotherMaidenFirstName", False, SqlDbType.VarChar)
    '            .Add("PersonMotherMaidenMiddleName", False, SqlDbType.VarChar)
    '            .Add("PersonSeniorCitizensIDNo", False, SqlDbType.VarChar)
    '            .Add("PersonPWDIDNo", False, SqlDbType.VarChar)
    '            .Add("PersonPassportNo", False, SqlDbType.VarChar)
    '            .Add("PersonDriversLicenseNo", False, SqlDbType.VarChar)
    '            .Add("PersonSSSNo", False, SqlDbType.VarChar)
    '            .Add("PersonGSISIDNo", False, SqlDbType.VarChar)
    '            .Add("PersonPRCIDNo", False, SqlDbType.VarChar)
    '            .Add("PersonPostalIDNo", False, SqlDbType.VarChar)
    '            .Add("PersonVotersIDNo", False, SqlDbType.VarChar)
    '            .Add("PersonOtherIDType", False, SqlDbType.VarChar)
    '            .Add("PersonOtherIDNo", False, SqlDbType.VarChar)
    '            .Add("CompanyName", False, SqlDbType.VarChar)
    '            .Add("CompanyNameInitial", False, SqlDbType.VarChar)
    '            .Add("OwnershipType", False, SqlDbType.VarChar)
    '            .Add("IsFranchisor", False, SqlDbType.Bit)
    '            .Add("RegistrationType", False, SqlDbType.VarChar)
    '            .Add("RegistrationNo", False, SqlDbType.VarChar)
    '            .Add("RegistrationDate", False, SqlDbType.Date)
    '            .Add("TinNumber", False, SqlDbType.VarChar)
    '            .Add("OfficeAddressBuildingNo", False, SqlDbType.VarChar)
    '            .Add("OfficeAddressStreet", False, SqlDbType.VarChar)
    '            .Add("OfficeAddressCityID", False, SqlDbType.UniqueIdentifier)
    '            .Add("OfficeAddressCountryID", False, SqlDbType.UniqueIdentifier)
    '            .Add("BusinessAddressBuildingNo", False, SqlDbType.VarChar)
    '            .Add("BusinessAddressStreet", False, SqlDbType.VarChar)
    '            .Add("BusinessAddressCityID", False, SqlDbType.UniqueIdentifier)
    '            .Add("BusinessAddressCountryID", False, SqlDbType.UniqueIdentifier)
    '            .Add("Purpose", False, SqlDbType.VarChar)
    '            .Add("BusinessScope", False, SqlDbType.VarChar)
    '            .Add("NoOfRetailOutlet", False, SqlDbType.Int)
    '            .Add("NoOfOffice", False, SqlDbType.Int)
    '            .Add("NoOfWarehouse", False, SqlDbType.Int)
    '            .Add("NoOfEmployees", False, SqlDbType.Int)
    '            .Add("IsHasHoldingCompany", False, SqlDbType.Bit)
    '            .Add("HoldingCompanyID", False, SqlDbType.UniqueIdentifier)
    '            .Add("IsABank", False, SqlDbType.Bit)
    '            .Add("UserID", False, SqlDbType.UniqueIdentifier)
    '            .Add("LastUpdated", False, SqlDbType.DateTime)
    '            .Add("SalesAgentCode", False, SqlDbType.VarChar)
    '            .Add("IsFromDevice", False, SqlDbType.Bit)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "MobizLibraryOrganization"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            If ImportLibraryOrganizationCustomer(vInputZipFilename, pLine("ID"), oError) = False Then
    '                DataImportLog.AddLog(oError)
    '                DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                Return False
    '            End If
    '            If ImportLibraryOrganizationPriceLevel(vInputZipFilename, pLine("ID"), oError) = False Then
    '                DataImportLog.AddLog(oError)
    '                DataImportLog.Save(DataImportLog.eResultType.Fail)
    '                Return False
    '            End If
    '        Loop
    '        rZipOut.AddTableCsv(pDtOut, "LibraryOrganization")

    '        Return True

    '    End Function

    '    Private Function ImportLibraryOrganizationCustomer(ByVal vInputZipFilename As String, ByVal vOrganizationID As String,
    '    ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String

    '        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationCustomer")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("OrganizationID", True, SqlDbType.UniqueIdentifier)
    '            .Add("ActivationDate", False, SqlDbType.DateTime)
    '            .Add("Deactivated", False, SqlDbType.Bit)
    '            .Add("DeactivationDate", False, SqlDbType.DateTime)
    '            .Add("DeactivationReason", False, SqlDbType.VarChar)
    '            .Add("Comments", False, SqlDbType.VarChar)
    '            .Add("IsBan", False, SqlDbType.Bit)
    '            .Add("LastUpdated", False, SqlDbType.DateTime)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pSql = "DELETE FROM LibraryOrganizationCustomer" & vbCrLf &
    '            " WHERE OrganizationID = " & ReadyString(vOrganizationID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return False
    '            End If
    '            If ReadyGuid(pLine("OrganizationID")) <> ReadyGuid(vOrganizationID) Then
    '                Continue Do
    '            End If

    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "LibraryOrganizationCustomer"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '        Loop

    '        Return True

    '    End Function

    '    Private Function ImportLibraryOrganizationPriceLevel(ByVal vInputZipFilename As String, ByVal vOrganizationID As String,
    '    ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String

    '        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationPriceLevel")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("OrganizationID", False, SqlDbType.UniqueIdentifier)
    '            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier)
    '            .Add("UserID", False, SqlDbType.UniqueIdentifier)
    '            .Add("LastUpdated", False, SqlDbType.DateTime)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pSql = "DELETE FROM LibraryOrganizationPriceLevel" & vbCrLf &
    '            " WHERE OrganizationID = " & ReadyString(vOrganizationID)
    '        mBusiness.ProData.ExecuteSQL(pSql)

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            If pBinder.Validate(oError) = False Then
    '                Return False
    '            End If
    '            If ReadyGuid(pLine("OrganizationID")) <> ReadyGuid(vOrganizationID) Then
    '                Continue Do
    '            End If

    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "LibraryOrganizationPriceLevel"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '        Loop

    '        Return True

    '    End Function

    '    Private Function ImportLibraryOrganizationFOWST(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
    '                ByVal vOrgID As String, ByVal vUuid As String,
    '                ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String

    '        DataImportLog.AddLog("Import from LibraryOrganizationFOWST")
    '        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationFOWST")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("OrganizationID", False, SqlDbType.UniqueIdentifier)
    '            .Add("FOWSTTypeID", False, SqlDbType.UniqueIdentifier)
    '            .Add("Code", False, SqlDbType.VarChar)
    '            .Add("Name", False, SqlDbType.VarChar)
    '            .Add("Description", False, SqlDbType.VarChar)
    '            .Add("BusinessTradeNameID", False, SqlDbType.UniqueIdentifier)
    '            .Add("StreetNumber", False, SqlDbType.VarChar)
    '            .Add("Street", False, SqlDbType.VarChar)
    '            .Add("FowstCityID", False, SqlDbType.UniqueIdentifier)
    '            .Add("FowstCountryID", False, SqlDbType.UniqueIdentifier)
    '            .Add("SubFOWSTTypeID", False, SqlDbType.UniqueIdentifier)
    '            .Add("PlateNo", False, SqlDbType.VarChar)
    '            .Add("NoOfWheels", False, SqlDbType.VarChar)
    '            .Add("BodyColor", False, SqlDbType.VarChar)
    '            .Add("Weight", False, SqlDbType.Float)
    '            .Add("Dimension", False, SqlDbType.VarChar)
    '            .Add("OwnershipType", False, SqlDbType.Bit)
    '            .Add("ContractStart", False, SqlDbType.Date)
    '            .Add("ContractEnd", False, SqlDbType.Date)
    '            .Add("CostPerMonth", False, SqlDbType.Decimal)
    '            .Add("IsMortgaged", False, SqlDbType.Bit)
    '            .Add("IsFranchisee", False, SqlDbType.Bit)
    '            .Add("FranchisorID", False, SqlDbType.UniqueIdentifier)
    '            .Add("IsFOWSTInsideAMall", False, SqlDbType.Bit)
    '            .Add("FOWSTMallName", False, SqlDbType.VarChar)
    '            .Add("UserID", False, SqlDbType.UniqueIdentifier)
    '            .Add("LastUpdated", False, SqlDbType.DateTime)
    '            .Add("FulfillmentCenter", False, SqlDbType.UniqueIdentifier)
    '            .Add("FOWSTNo", False, SqlDbType.Int)
    '            .Add("AccountID", False, SqlDbType.VarChar)
    '            .Add("StoreID", False, SqlDbType.VarChar)
    '            .Add("IsIncludeSpecialReport", False, SqlDbType.Bit)
    '            .Add("ShopperTrakAccountID", False, SqlDbType.UniqueIdentifier)
    '            .Add("IsIncludeToShopperTrakReport", False, SqlDbType.Bit)
    '            .Add("IsFromDevice", False, SqlDbType.Bit)
    '        End With
    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "MobizLibraryOrganizationFOWST"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try

    '        Loop
    '        rZipOut.AddTableCsv(pDtOut, "LibraryOrganizationFOWST")

    '        Return True

    '    End Function
    '    ' end of additional 

    '    Private Function ImportTransInventoryProductCount(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
    '                ByVal vOrgID As String, ByVal vUuid As String,
    '                ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pBinder2 As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String

    '        Dim pDtItems As DataTable
    '        Dim Total As Double

    '        pDtItems = CreateTableForInventoryCountItems(vInputZipFilename, oError)

    '        DataImportLog.AddLog("Import from TransInventoryProductCount")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInventoryProductCount")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("FowstID", False, SqlDbType.UniqueIdentifier)
    '            .Add("ProductID", False, SqlDbType.UniqueIdentifier)
    '            .Add("InventoryCountDate", False, SqlDbType.VarChar)
    '            .Add("UserID", False, SqlDbType.VarChar)
    '        End With

    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        pBinder2 = New DictionaryBinder
    '        With pBinder2.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InventoryProductCountID", True, SqlDbType.UniqueIdentifier)
    '            .Add("BinID", True, SqlDbType.UniqueIdentifier)
    '            .Add("IsGoodStock", True, SqlDbType.Bit)
    '            .Add("ExpirationDate", True, SqlDbType.VarChar)
    '            .Add("UnitMeasureID", True, SqlDbType.UniqueIdentifier)
    '            .Add("Quantity", True, SqlDbType.Int)
    '            .Add("ActualQuantity", True, SqlDbType.Int)
    '        End With

    '        Do
    '            'mBusiness.ProData.BeginTransaction()
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "TransInventoryProductCount"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)

    '                    'TransInventoryProductCount details
    '                    pDtItems.DefaultView.RowFilter = " InventoryProductCountID = " & ReadyString(pLine("ID"))

    '                    Total = 0

    '                    If pDtItems.DefaultView.ToTable.Rows.Count > 0 Then
    '                        For i As Integer = 0 To pDtItems.DefaultView.ToTable.Rows.Count - 1
    '                            Dim p As New InsertUpdate("TransInventoryProductCountStock")

    '                            p("ID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ID"))
    '                            p("InventoryProductCountID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("InventoryProductCountID"))
    '                            p("BinID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID"))
    '                            p("IsGoodStock") = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("IsGoodStock"))

    '                            If Not pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate") = "N/A" Then
    '                                p("ExpirationDate") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate"))
    '                            End If

    '                            p("UnitMeasureID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("UnitMeasureID"))
    '                            p("Quantity") = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity"))
    '                            p("ActualQuantity") = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))

    '                            pSql = p.GetInsert()

    '                            Total += CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) * CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))

    '                            mBusiness.ProData.ExecuteSQL(pSql)
    '                        Next
    '                        ComputeOffTake(pLine("ID"), pLine("FowstID"), pDtItems.DefaultView.ToTable.Rows(0).Item("BinID"), pLine("ProductID"), Total, pLine("InventoryCountDate"))
    '                        AddToInventoryLedger(pLine("ID"), pDtItems.DefaultView.ToTable.Rows(0).Item("BinID"), Total, pDtItems.DefaultView.ToTable.Rows(0).Item("ExpirationDate"), pLine("ProductID"), pLine("FowstID"), pLine("UserID"))
    '                    End If
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                mBusiness.ProData.RollbackTransaction()
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            'mBusiness.ProData.CommitTransaction()
    '        Loop


    '        rZipOut.AddTableCsv(pDtOut, "TransInventoryProductCount")

    '        Return True
    '    End Function

    '    Function ComputeOffTake(ByVal InventoryCountID As String, ByVal FOWSTID As String, ByVal BinID As String, ByVal ProductID As String, ByVal TotalCount As Double, ByVal CountDate As Date)
    '        Dim pSql As String
    '        Dim dtPreviousBalance As DataTable
    '        Dim OffTake As Double

    '        pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
    '                "WHERE ProductID = " & ReadyString(ProductID) & vbCrLf &
    '                " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
    '                " AND BINID = " & ReadyString(BinID) & vbCrLf &
    '                " order by LastUpdated desc"

    '        dtPreviousBalance = mBusiness.ProData.Execute(pSql)

    '        If dtPreviousBalance.Rows.Count > 0 Then
    '            OffTake = CDbl(dtPreviousBalance.Rows(0).Item("Balance")) - TotalCount
    '            Dim p As New InsertUpdate("TransOffTake")

    '            p("OfftakeDate") = ReadyString(CountDate)
    '            p("InventoryCountID") = ReadyString(InventoryCountID)
    '            p("ProductID") = ReadyString(ProductID)
    '            p("FowstID") = ReadyString(FOWSTID)
    '            p("BinID") = ReadyString(BinID)
    '            p("OffTake") = ReadyString(OffTake)

    '            pSql = p.GetInsert

    '            mBusiness.ProData.ExecuteSQL(pSql)
    '        End If
    '    End Function

    '    Private Function ImportLibraryFOWSTBin(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
    '                ByVal vOrgID As String, ByVal vUuid As String,
    '                ByRef oError As String) As Boolean

    '        Dim pCsvReader As CsvReader
    '        Dim pBinder As DictionaryBinder
    '        Dim pDtOut As DataTable
    '        Dim pDrOut As DataRow
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim pIU As InsertUpdate
    '        Dim pSql As String

    '        DataImportLog.AddLog("Import from LibraryFOWSTBin")
    '        pCsvReader = New CsvReader(vInputZipFilename, "LibraryFOWSTBin")
    '        pBinder = New DictionaryBinder
    '        With pBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
    '            .Add("Name", False, SqlDbType.VarChar)
    '            .Add("Description", False, SqlDbType.VarChar)
    '            .Add("IsActive", False, SqlDbType.Bit)
    '        End With

    '        pDtOut = New DataTable
    '        pDtOut.Columns.Add("ID")

    '        Do
    '            'mBusiness.ProData.BeginTransaction()
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            pBinder.Source = pLine
    '            pIU = pBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Continue Do
    '            End If
    '            pIU.TableName = "LibraryFOWSTBin"
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
    '            Try
    '                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
    '                    pSql = pIU.GetInsert
    '                    mBusiness.ProData.ExecuteSQL(pSql)
    '                End If
    '                pDrOut = pDtOut.NewRow
    '                pDrOut("ID") = pLine("ID")
    '                pDtOut.Rows.Add(pDrOut)
    '            Catch ex As Exception
    '                Return JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message)
    '            End Try
    '            'mBusiness.ProData.CommitTransaction()
    '        Loop


    '        rZipOut.AddTableCsv(pDtOut, "LibraryFOWSTBin")

    '        Return True
    '    End Function


    '    Private Function CreateTableForInventoryCountItems(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable
    '        Dim pCsvReader As CsvReader
    '        Dim pDtOut As DataTable
    '        Dim pLine As Dictionary(Of String, String)
    '        Dim oBinder As DictionaryBinder
    '        Dim pIU As InsertUpdate ' used for verfication only

    '        DataImportLog.AddLog("Import from TransInventoryProductCountStock")
    '        pCsvReader = New CsvReader(vInputZipFilename, "TransInventoryProductCountStock")
    '        oBinder = New DictionaryBinder
    '        With oBinder.Items
    '            .Add("ID", True, SqlDbType.UniqueIdentifier)
    '            .Add("InventoryProductCountID", True, SqlDbType.UniqueIdentifier)
    '            .Add("BinID", True, SqlDbType.UniqueIdentifier)
    '            .Add("IsGoodStock", True, SqlDbType.Bit)
    '            .Add("ExpirationDate", True, SqlDbType.VarChar)
    '            .Add("UnitMeasureID", True, SqlDbType.UniqueIdentifier)
    '            .Add("Quantity", True, SqlDbType.Int)
    '            .Add("ActualQuantity", True, SqlDbType.Int)
    '        End With

    '        pDtOut = oBinder.CreateDataTable

    '        Do
    '            pLine = pCsvReader.ReadLine
    '            If pLine Is Nothing Then Exit Do
    '            oBinder.Source = pLine
    '            pIU = oBinder.CreateInsertUpdate(oError)
    '            If pIU Is Nothing Then
    '                Return Nothing
    '            End If
    '            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
    '        Loop

    '        Return pDtOut
    '    End Function

    '    Function AddToInventoryLedger(ByVal TransactionID As String, ByVal BinID As String, ByVal Total As Double, ByVal ExpDate As String, ByVal ProductID As String, ByVal FOWSTID As String, ByVal PeopleID As String)
    '        Dim EDate As Date?

    '        If Not ExpDate = "N/A" Then
    '            EDate = CDate(ExpDate)
    '        End If

    '        mBusiness.TransInventoryLedger.Add( _
    '                        ProductID, _
    '                        FOWSTID, _
    '                        TransactionID, _
    '                        TransInventoryLedger.eTransType.InventoryCount, _
    '                        PeopleID, , _
    '                        Total, _
    '                        BinID,
    '                        EDate
    '                    )
    '    End Function
    'End Class


    <HttpGet()>
    Public Function DownloadImportResult() As ActionResult

        Dim pFile As IO.FileInfo
        Dim pFilename As String
        Dim pOrgID As String
        Dim pDeviceID As String

        pFilename = Common.WebRequest.QueryString("Filename")
        If CStr2(pFilename).Trim = "" Then
            Return Content("Query string must contain filename.")
        End If

        pOrgID = Common.WebRequest.QueryString("OrgID")
        If CStr2(pOrgID).Trim = "" Then
            Return Content("OrgID POST data is missing from uploaded file.")
        End If
        If IsGuid(pOrgID) = False Then
            Return Content("OrgID POST data must be a valid GUID.")
        End If
        pOrgID = ReadyGuid(pOrgID)

        pDeviceID = Common.WebRequest.QueryString("DeviceID")
        If CStr2(pDeviceID).Trim = "" Then
            Return Content("DeviceID POST data is missing. vbYPA75VLLHH")
        End If
        If IsGuid(pDeviceID) = False Then
            Return Content("DeviceID POST data must be a valid GUID.")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        pFilename = Common.ServerPath & "\temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pFilename & "_imported.zip"
        pFile = New IO.FileInfo(pFilename)
        Try
            If pFile.Exists Then
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(pFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(pFilename, True))
                Response.TransmitFile(pFilename)
            Else
                Return Content("Import result file not found vb4UDGN7WW9E.")
            End If
        Catch ex As Exception
            Stop2()
            Return Content(ex.Message)
        End Try

    End Function

#Region " summary "
    <HttpPost()>
    Public Function ProcessSummary() As String

        Dim pOut As JsonDictionary
        Dim pStr As String
        Dim pError As String
        Dim pFilename As String
        Dim pInfo As String
        Dim pInfoJson As JsonDictionary
        Dim pOrgID As String
        Dim pDeviceID As String
        Dim pUuid As String
        Dim pErrorDate As Date?
        Dim pDataImportLog As DataImportLog

        pOut = New JsonDictionary

        pDataImportLog = New DataImportLog

        pOrgID = Common.WebRequest.Data("OrgID")
        If CStr2(pOrgID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "OrgID POST data is missing.")
        End If
        If IsGuid(pOrgID) = False Then
            Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
        End If
        pOrgID = ReadyGuid(pOrgID)

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True) = False Then
            Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database.")
        End If

        pDeviceID = Common.WebRequest.Data("DeviceID")
        If CStr2(pDeviceID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing.")
        End If
        If IsGuid(pDeviceID) = False Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
        End If
        pDeviceID = ReadyGuid(pDeviceID)


        pStr = Common.WebRequest.Data("TimeStamp")
        If CStr2(pStr).Trim = "" Then
            pError = "TimeStamp was not found in post data"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If
        pFilename = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pStr & "_summary.zip"
        pDataImportLog.AddLog("TimeStamp (POST data from device): " & pStr)
        If IO.File.Exists(pFilename) = False Then
            pError = "File was not found in upload summary folder"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If
        pDataImportLog.AddLog("Filename: " & pFilename)
        If C1.C1Zip.C1ZipFile.IsZipFile(pFilename) = False Then
            pError = "Summary file is not a valid zip file"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If

        pInfo = New ProZip(pFilename).GetString("info.json")
        If CStr2(pInfo).Trim = "" Then
            pError = "Info.json does not exist or is empty"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If
        Try
            pInfoJson = New JsonDictionary()
            pInfoJson.SetJson(pInfo)
        Catch ex As System.Exception
            pError = "Info.json does not contain a valid JSON"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End Try
        pOrgID = pInfoJson("OrgID")
        If CStr2(pOrgID).Trim = "" Then
            pError = "Info.json does not contain OrgID"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If
        pDataImportLog.AddLog("pInfoJson(""OrgID""): " & pOrgID)
        If IsGuid(pOrgID) = False Then
            pError = "Info.json does not contain a valid GUID OrgID"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True) = False Then
            pError = "Info.json's OrgID does not point to a subscriber"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If

        pUuid = pInfoJson("UUID")
        If CStr2(pUuid).Trim = "" Then
            pError = "Info.json does not contain Uuid"
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            Return JsonDictionary.Serialize("Error", pError)
        End If
        pDataImportLog.AddLog("pInfoJson(""UUID""): " & pUuid)

        If CheckInvoiceSummary(pFilename, pOrgID, pUuid, pError, pErrorDate, pDataImportLog) = False Then
            pDataImportLog.AddLog(pError)
            pDataImportLog.Save(DataImportLog.eResultType.SummaryFail)
            pOut.Add("Error", pError)
            If pErrorDate IsNot Nothing Then
                pOut.Add("NotTally", 1)
                pOut.Add("ErrorDate", Format(pErrorDate, "yyy/MM/dd"))
            End If
            Return pOut.Serialize
        End If
        pDataImportLog.Save(DataImportLog.eResultType.SummarySuccess)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    Private Function CheckInvoiceSummary(vInputZipFilename As String, vOrgID As String, vUuid As String, ByRef oError As String,
        ByRef oErrorDate As Date?, vDataImportLog As DataImportLog) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pBinder As DictionaryBinder
        Dim pDR As DataRow

        Return True

        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("SellingAmount", True, SqlDbType.Money)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
            .Add("LastUpdatedDate", True, SqlDbType.Date)
        End With

        pCsvReader = New CsvReader(vInputZipFilename, "Invoice")
        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            'additional by janine sept 8 2014 5pm for sales order taker
            Dim posmode As String
            If CDec2(pLine("SalesOrderNo")) > 0 Then
                posmode = "so"
            Else
                posmode = "pos"
            End If
            ''''''''''''''''''''
            If WebRequest.Instance.Business.LibraryDevice.ValidDeviceFowst( _
                vFowstID:=pLine("FowstID"),
                vDeviceID:=pLine("DeviceID"),
                vPosMode:=posmode,
                oError:=oError) = False Then

                oError = "CheckInvoiceSummary.ValidDeviceFowst error: " & oError
                Return False
            End If
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                oError = "CheckInvoiceSummary pBinder.Validate error: " & oError
                Return False
            End If
            vDataImportLog.AddLog("FowstID: " & pLine("FowstID"))
            vDataImportLog.AddLog("DeviceID: " & pLine("DeviceID"))
            vDataImportLog.AddLog("LastUpdatedDate: " & pLine("LastUpdatedDate"))

            pDR = WebRequest.Instance.Business.TransInvoice.GetSummary(pLine("FowstID"), pLine("DeviceID"), pLine("LastUpdatedDate"))
            If pDR Is Nothing Then
                oError = "Server has no invoice record for " & pLine("LastUpdatedDate") & " for FOWST " & pLine("FowstID") & " for device " & pLine("DeviceID")
                oErrorDate = pLine("LastUpdatedDate")
                Return False
            End If
            If CDec2(pDR("SellingAmount")) + 0.1 < CDec2(pLine("SellingAmount")) Then ' 10 centavo tolerance
                oError = "Server invoice amount (" & Format(CDec2(pDR("SellingAmount")), cGlobal.FormatCurrency) & ") is different from device invoice amount (" & Format(CDec2(pLine("SellingAmount")), cGlobal.FormatCurrency) & ") for invoices dated on " & pLine("LastUpdatedDate") & "."
                oErrorDate = pLine("LastUpdatedDate")
                Return False
            Else
                vDataImportLog.AddLog("Server invoice amount (" & Format(CDec2(pDR("SellingAmount")), cGlobal.FormatCurrency) & ") is equal to or greater than the device invoice amount (" & Format(CDec2(pLine("SellingAmount")), cGlobal.FormatCurrency) & ").")
            End If
        Loop

        Return True

    End Function
#End Region

    Private Shared mReimporter As ImportDataClass
    Private mReimportIndex As Integer
    Dim mImportedFiles As List(Of ImportedFile)

    <HttpGet()>
    Private Function Reimport()

        Const pMinDate As Date = #9/22/2014#
        Dim pDirectoryInfo As IO.DirectoryInfo
        Dim pImportedFile As ImportedFile
        Dim pFilenames As String

        If mReimporter IsNot Nothing Then
            Return "Another reimport is in process"
        End If
        mReimportIndex = 0
        mReimporter = New ImportDataClass
        mImportedFiles = New List(Of ImportedFile)
        pDirectoryInfo = New IO.DirectoryInfo(Common.ServerPath & "temp\Uploads\")
        For Each pOrg As IO.DirectoryInfo In pDirectoryInfo.GetDirectories()
            ' 61BE6909-F4C2-4AB2-8DF3-D4841CC2F318  =dsme
            If ReadyGuid(pOrg.Name) <> ReadyGuid("61BE6909-F4C2-4AB2-8DF3-D4841CC2F318") Then
                Continue For
            End If
            For Each pDevice As IO.DirectoryInfo In pOrg.GetDirectories()
                For Each pFile As IO.FileInfo In pDevice.GetFiles()
                    If IO.Path.GetFileNameWithoutExtension(pFile.Name).Length = 25 Then
                        If pFile.LastWriteTime > pMinDate Then
                            pImportedFile = New ImportedFile
                            pImportedFile.FileName = IO.Path.GetFileNameWithoutExtension(pFile.Name)
                            pImportedFile.OrgID = pOrg.Name
                            pImportedFile.DeviceID = pDevice.Name
                            mImportedFiles.Add(pImportedFile)
                            pFilenames &= pFile.FullName & vbCrLf
                        End If
                    End If
                Next pFile
            Next pDevice
        Next pOrg
        AppendToLog(pFilenames, , "For reimport")

        ReimportFile()

    End Function

    Private Sub ReimportFile()

        Dim pImportedFile As ImportedFile
        If mReimportIndex < mImportedFiles.Count Then
            pImportedFile = mImportedFiles(mReimportIndex)
            mReimportIndex += 1
            mReimporter = New ImportDataClass
            mReimporter.AllowReimportNonZeroInvoice = True
            mReimporter.ImportInvoiceOnly = True
            'mReimporter.InvoiceNumbersToImport = ";7887;7888;7892;7896;7898;7880;"
            mReimporter.OrgID = pImportedFile.OrgID
            mReimporter.DeviceID = pImportedFile.DeviceID
            mReimporter.InputFilename = pImportedFile.FileName
            AddHandler mReimporter.Done, AddressOf ReimportFile
            AppendToLog("Importing " & mReimporter.GetFilename(), , "Reimport")
            mReimporter.Start()
        Else
            mReimporter = Nothing
            AppendToLog("done importing", , "Reimport")
        End If

    End Sub

    Private Class ImportedFile
        Public FileName As String
        Public OrgID As String
        Public DeviceID As String
    End Class

    <HttpGet()>
    Public Function CsvDbCheck() As String

        Dim p As CsvDb
        Dim pMessage As String

        p = New CsvDb
        p.VerifyRequirements(pMessage)

        Return pMessage

    End Function

    Private Shared mCsvDb As CsvDb

    Public Function CsvDbCombo() As String

        Dim pSuccess As Boolean

        CsvDb(pSuccess)
        If pSuccess Then
            CsvDbCompare(pSuccess)
            If pSuccess Then
                CsvDbReimport(pSuccess)
                If pSuccess Then
                    Return "vbRRDNR5VM9F! success"
                End If
            End If
        End If

        Return "vbUVJT4JBUKU! failed"

    End Function

    Public Function CsvDb(Optional ByRef oSuccess As Boolean = False) As String

        Dim pMessage As String

        oSuccess = False
        If Request.IsLocal = False Then
            Return "vbZCFFNSZPNH! This feature can only be accessed locally."
        End If

        Try
            If mCsvDb IsNot Nothing Then
                Return "Another CsvDb is already running"
            End If
            mCsvDb = New CsvDb()
            If mCsvDb.ImportOrganizations() Then
                oSuccess = True
                Return "Success"
            Else
                Return "Fail"
            End If
        Catch ex As System.Exception
            Stop2()
            Return "Error " & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        Finally
            mCsvDb = Nothing
        End Try

    End Function

    Public Function CsvDbCompare() As String

        Return CsvDbCompare(Nothing)

    End Function

    Private Function CsvDbCompare(ByRef oSuccess As Boolean) As String

        oSuccess = False
        If Request.IsLocal = False Then
            Return "vbZCFFNSZPNH! This feature can only be accessed locally."
        End If

        Try
            If mCsvDb IsNot Nothing Then
                Return "Another CsvDb is already running"
            End If
            mCsvDb = New CsvDb()
#If DEBUG Then
            mCsvDb.CompareDatabase("MobizABC")
#Else
            mCsvDb.CompareDatabase("MobizDUC")
#End If
            oSuccess = True
            Return "ok"
        Catch ex As System.Exception
            Stop2()
            Return "Error " & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        Finally
            mCsvDb = Nothing
        End Try

    End Function

    <HttpGet()>
    Public Function CsvDbReimport(Optional ByRef oSuccess As Boolean = False) As String

        Dim pMessage As String

        oSuccess = False
        If Request.IsLocal = False Then
            Return "vbZCFFNSZPNH! This feature can only be accessed locally."
        End If

        Try
            If mCsvDb IsNot Nothing Then
                Return "vbG4L6QNWU4V! Another CsvDb is already running"
            End If
            mCsvDb = New CsvDb()
            If mCsvDb.Reimport(pMessage) Then
                oSuccess = True
                Return "vb5BM9NTR7Z6! Success. " & CStr2(pMessage).Replace(vbCrLf, "<br />")
            Else
                Return "vbB5W4WUDVFB! Fail. " & CStr2(pMessage).Replace(vbCrLf, "<br />")
            End If
        Catch ex As System.Exception
            Stop2()
            Return "Error " & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        Finally
            mCsvDb = Nothing
        End Try

    End Function
End Class