Imports Newtonsoft.Json

Public Class OfflineDataController
    Inherits System.Web.Mvc.Controller

    Private Shared ReadOnly mDataImporters As Dictionary(Of String, ImportDataClass) ' keys = OrgID
    Private Shared mVacantSortingMethod As Integer
    Dim mFilename As String

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
            pOut.Add("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture)) ' "u" cannot be parsed by Android
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
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
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
            " ,'this is to test escape patterns in sqlite' AS [test]" & vbCrLf &
            " ,'dummy' AS [test[2]]]" & vbCrLf &
            " ,'dummy' AS [test3]]]" & vbCrLf &
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

        Try
            CreateSchemaZipfile()
            pFilename = Common.ServerPath & "temp\schema.zip"
            pFile = New IO.FileInfo(pFilename)
            If pFile.Exists Then
                pOut = New JsonDictionary
                pOut.Add("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture)) ' "u" cannot be parsed by Android
                pOut.Add("FileSize", pFile.Length)
                Return pOut.Serialize
            Else
                Return JsonDictionary.Serialize("Error", "File not found")
            End If
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function Schema() As ActionResult

        Dim pFile As IO.FileInfo
        Dim pFilename As String
        Dim pSqliteSchemaExporter As SqliteSchemaExporter
        Dim pDbs As List(Of String)
        Dim pCrc As String

        Debug.Print("OfflineDataController.Schema")
        Try
            CreateSchemaZipfile()
            pFilename = Common.ServerPath & "\temp\schema.zip"
            pFile = New IO.FileInfo(pFilename)
            If pFile.Exists Then
                pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pFilename)
                Response.AddHeader("CRC", pCrc)
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
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
                        ElseIf pDbs.Contains("MobizDUC".ToLower()) Then
                            .DatabaseName = "MobizDUC"
                        ElseIf pDbs.Contains("MobizABC".ToLower()) Then
                            .DatabaseName = "MobizABC"
                        ElseIf pDbs.Contains("MobizHungryApe".ToLower()) Then
                            .DatabaseName = "MobizHungryApe"
                        Else
                            Throw New ApplicationException("No available Mobiz private database to create schema from. vbH7Y68CEQ57!")
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

            pSql = "SELECT sysobjects.ID, sysobjects.Name" &
                ",sysusers.name AS Owner" &
                " FROM sysobjects" &
                " JOIN sysusers" &
                " ON sysusers.uid = sysobjects.uid" &
                " WHERE xtype = 'U'" &
                " AND sysobjects.status >= 0" &
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
        Const pLogFilename As String = "OfflineData_Dump"
        Dim pError As String

        If Request.Files.Count > 0 Then
            pOrgID = Common.WebRequest.Data("OrgID")
            If CStr2(pOrgID).Trim = "" Then
                AppendToLog("OrgID POST data is missing from uploaded file.", , pLogFilename)
                Return JsonDictionary.Serialize("Error", "OrgID POST data is missing from uploaded file.")
            End If
            If IsGuid(pOrgID) = False Then
                AppendToLog("OrgID POST data must be a valid GUID.", , pLogFilename)
                Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
            End If
            pOrgID = ReadyGuid(pOrgID)
            If CanFlush(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "OrgID is not in whitelist. vbE56W2GAANR!")
            End If
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                AppendToLog("OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vbNEHJXDQE86!", , pLogFilename)
                Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vbNEHJXDQE86!")
            End If

            pDeviceID = Common.WebRequest.Data("DeviceID")
            If CStr2(pDeviceID).Trim = "" Then
                AppendToLog("DeviceID POST data is missing from uploaded file.", , pLogFilename)
                Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing from uploaded file.")
            End If
            If IsGuid(pDeviceID) = False Then
                AppendToLog("DeviceID POST data must be a valid GUID.", , pLogFilename)
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

            Dim pDontDeleteUnprocessedTransImport = CBool2(WebRequest.Instance.Data("DontDeleteUnprocessedTransImport"))
            WebRequest.Instance.Business.TransFlushingReport.SaveTransImport(vDeviceID:=pDeviceID, vFilename:=pTimestamp, vFullFilename:=pFilename,
                vDontDeleteUnprocessedTransImport:=pDontDeleteUnprocessedTransImport)
            'ImportOrganization(pOrgID, Nothing, Nothing)
            Me.CallImportOrganization(pOrgID)
            Dim pOut As New JsonDictionary()
            pOut.Add("TimeStamp", pTimestamp)
            pOut.Add("Filename", pFilename)
            Return pOut.Serialize()
        Else
            AppendToLog("No file. vbEXNMB8E9RF!", , pLogFilename)
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
                Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. ref: vbRVB5HHZ2Z9!")
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

    Function CanFlush(vOrgID As String) As Boolean

        vOrgID = ReadyGuid(vOrgID)
        Dim pFilename = Common.ServerPath & "settings\FlushImportOrgIDs.txt"
        If System.IO.File.Exists(pFilename) Then
            Dim pText = GetFileText(pFilename, 999999)
            If String.IsNullOrWhiteSpace(pText) = False Then
                If pText.ToLower().Contains(vOrgID.ToLower()) = False Then
                    Return False
                End If
            End If
        Else
            FileText(pFilename) = ""
        End If

        Return True

    End Function

    Shared ReadOnly mImportOrganizationLock As New Object

    Shared ReadOnly mCallImportOrganizationLock As New Dictionary(Of String, Object)

    Public Function CallImportOrganization(OrgID As String) As String
        ' http://localhost:8080/OfflineData/CallImportOrganization?OrgID=D362FFDB-15D1-451F-97DC-C567B94FB410
        ' http://119.93.147.163:8080/OfflineData/CallImportOrganization?OrgID=D362FFDB-15D1-451F-97DC-C567B94FB410

        OrgID = ReadyGuid(OrgID)
        If CanFlush(OrgID) = False Then
            Return "not in whitelist. vb6MZ98YRE5T!"
        End If

        Dim pMessage As String
        Dim pLoopCount = 0
        Dim pSW = Stopwatch.StartNew()
        Dim pProcesses As New List(Of ImportDataClass)
        Dim pMaxRunningProcesses As Integer = 1

        AppendToLog("vb6XBSFWQ93A! start OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
        If ReadyGuid(OrgID) = ReadyGuid(modClient.CincoOrgID) _
            OrElse ReadyGuid(OrgID) = ReadyGuid("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba") Then
            'If ReadyGuid(OrgID) = ReadyGuid(modClient.CincoOrgID) Then
            pMaxRunningProcesses = 1
        End If

        TimedThread(
            Sub()
                AppendToLog("vbTNBVLS9AKP! outside mCallImportOrganizationLock OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                If mCallImportOrganizationLock.ContainsKey(OrgID) = False Then
                    mCallImportOrganizationLock.Add(OrgID, New Object)
                End If
                SyncLock mCallImportOrganizationLock(OrgID)
                    Try
                        AppendToLog("enter OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                        Do
                            AppendToLog("outer do loop OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                            Do
                                Dim pRunningProcesses = From pProcess In pProcesses
                                                        Where pProcess.IsRunning
                                                        Select pProcess

                                If pRunningProcesses.Count() < pMaxRunningProcesses Then
                                    Exit Do
                                End If
                                AppendToLog("vbNHP45LG58Z! pRunningProcesses.Count() = " & pRunningProcesses.Count() & "; OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                                Threading.Thread.Sleep(1000)
                            Loop

                            Dim pImportDataClass As ImportDataClass
                            If ImportOrganization(OrgID, pMessage, pImportDataClass, True) Then
                                pLoopCount += 1
                                AppendToLog("vbZEF7VR93XU! pLoopCount += 1 OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                            ElseIf String.IsNullOrWhiteSpace(pMessage) Then
                                AppendToLog("vb89ATX7ZAAW! String.IsNullOrWhiteSpace(pMessage) OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                                Exit Do
                            ElseIf pMessage.Contains("does not have pending import") Then
                                AppendToLog("vbSAR5P49NKW! does not have pending import OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                                Exit Do
                            ElseIf pMessage.Contains("is currently importing data from device") Then
                                AppendToLog("vbUM5P3JAP47! is currently importing data from device OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                                Threading.Thread.Sleep(1000)
                            Else
                                'Return pMessage
                                AppendToLog("vbYF2JHRUQSL! Return OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                                Return
                            End If
                            pProcesses.Add(pImportDataClass)
                        Loop

                        Dim pOkProcesses = From pProcess In pProcesses
                                           Where pProcess.IsSuccess
                                           Select pProcess

                        Dim pFailedProcesses = From pProcess In pProcesses
                                               Where pProcess.IsSuccess = False
                                               Select pProcess

                        'Return "Import processes without error: " & pOkProcesses.Count() & "; " & vbCrLf &
                        '    "Import processes with error: " & pFailedProcesses.Count() & "; " & vbCrLf &
                        '    "Number of concurrent import processes: " & pMaxRunningProcesses & vbCrLf &
                        '    "Duration: " & FormatTime(pSW.ElapsedMilliseconds / 1000)
                    Catch ex As Exception
                        AppendToLog("vb7ZHHPNXATN! OrgID=" & OrgID & vbCrLf &
                            ex.Message & vbCrLf &
                            GetStackTrace(, ex), , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                    Finally
                        AppendToLog("exit OrgID=" & OrgID, , "OfflineDataController_CallImportOrganization vbMBPGXX99KP")
                    End Try
                End SyncLock
            End Sub)

        Return DateTimeStamp()

    End Function

    Public Function RunningProcesses() As String

        Dim pRunning = From pImporter In mDataImporters
                       Where pImporter.Value.IsRunning
                       Select pImporter

        Return pRunning.Count()

    End Function

    Private Function ImportOrganization(vOrgID As String, ByRef oMessage As String, ByRef oDataImport As ImportDataClass,
        Optional vUnstartedOnly As Boolean = False) As Boolean

        Dim pDR As DataRow

        SyncLock mImportOrganizationLock ' commented for debugging 2022_09_29__15_34_06_9901
            oMessage = ""

            AppendToLog("vUnstartedOnly = " & vUnstartedOnly,,
                "ImportOrganization vbH2TXMVXJ5M")

            If vUnstartedOnly = False Then
                If mDataImporters.ContainsKey(vOrgID) Then
                    oDataImport = mDataImporters(vOrgID)
                    If oDataImport.IsRunning Then
                        oMessage = "Org, " & vOrgID & ", is currently importing data from device, " & oDataImport.DeviceID & "."
                        AppendToLog("oMessage = " & oMessage,,
                            "ImportOrganization vbH2TXMVXJ5M")
                        Return False
                    End If
                End If
            End If

            ' BEGIN commented for experiment 2022_10_05__13_44_20_3442
            'If WebRequest.Instance.HandleOrganization(True) = False Then
            '    oMessage = "Org, " & vOrgID & ", does not point to a subscriber."
            '    Return False
            'End If
            ' END commented for experiment 2022_10_05__13_44_20_3442

            'Dim pLatestFirst = ReadyGuid(vOrgID) = ReadyGuid(modClient.CincoOrgID) ' for exe only
            Dim pLatestFirst As Boolean = False
            Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pBusiness = New Business(pProData)
                Dim pDrOrg = pBusiness.LibraryOrganization.ByID(vOrgID)
                If pDrOrg Is Nothing Then
                    Throw New Exception("OrgID (" & vOrgID & ") point to a non-existing record. vbZF3X4QFYHZ!")
                End If
                If CStr2(pDrOrg("Database")).Trim = "" Then
                    Throw New Exception("OrgID (" & vOrgID & ") has no database. vbRNUW6252ED!")
                End If
                pProData.ExecuteSQL("USE " & EscapeSqlName(CStr(pDrOrg("Database"))))
                Dim pDontSortByFileSize As Boolean
                Dim pVacantSortingMethod = mVacantSortingMethod
                If ReadyGuid(modClient.PotatoCornerFranchise2OrgID) = ReadyGuid(vOrgID) Then
                    pDontSortByFileSize = True
                    pLatestFirst = True
                Else
                    pVacantSortingMethod += 1
                    If pVacantSortingMethod > 6 Then
                        pVacantSortingMethod = 1
                    End If
                    If pVacantSortingMethod >= 6 Then
                        pDontSortByFileSize = True
                    End If
                End If

                pDR = LoadTransImportVacant(pLatestFirst, vUnstartedOnly, pProData, False, pDontSortByFileSize)
                If pDR Is Nothing Then
                    oMessage = "Org, " & vOrgID & ", does not have pending import."
                    AppendToLog("vbUU8NBMNHP8! oMessage = " & oMessage,,
                            "ImportOrganization vbH2TXMVXJ5M")
                    Return False
                End If
                mVacantSortingMethod = pVacantSortingMethod
                oMessage = "Processing import. Date received: " & Format(pDR("DateReceived"), cGlobal.FormatDateTime)
                AppendToLog("vbK9FZ584GDB! oMessage = " & oMessage,,
                            "ImportOrganization vbH2TXMVXJ5M")
                If ImportDataClass.FastFlushingOrgIDs().Contains(ReadyGuid(vOrgID)) Then
                    AppendToLog("vbXV63Y9Z78S! ImportDataClass.FastFlushingOrgIDs().Contains(ReadyGuid(vOrgID)) = true",,
                            "ImportOrganization vbH2TXMVXJ5M")
                    oDataImport = New ImportDataClass(True, CStr(pDrOrg("Database")), False)
                Else
                    AppendToLog("vbJM9VJFRCE7! ImportDataClass.FastFlushingOrgIDs().Contains(ReadyGuid(vOrgID)) = false",,
                            "ImportOrganization vbH2TXMVXJ5M")
                    oDataImport = New ImportDataClass
                End If
            End Using

            oDataImport.TransImportsID = CStr2(pDR("ID"))
            oDataImport.OrgID = vOrgID
            oDataImport.DeviceID = ReadyGuid(pDR("DeviceID"))
            oDataImport.InputFilename = pDR("Filename")
            oDataImport.Start()

            If mDataImporters.ContainsKey(vOrgID) Then
                AppendToLog("vbB2QR5VXGXB! mDataImporters.ContainsKey(vOrgID) = true",,
                    "ImportOrganization vbH2TXMVXJ5M")
                mDataImporters(vOrgID) = oDataImport
            Else
                AppendToLog("vbH9SSSF5SAA! mDataImporters.ContainsKey(vOrgID) = false",,
                    "ImportOrganization vbH2TXMVXJ5M")
                mDataImporters.Add(vOrgID, oDataImport)
            End If

            AppendToLog("oDataImport.OrgID = " & oDataImport.OrgID & vbCrLf &
                "oDataImport.DeviceID = " & oDataImport.DeviceID & vbCrLf &
                "oDataImport.InputFilename = " & oDataImport.InputFilename,,
                "ImportOrganization vbH2TXMVXJ5M")

            Return True
        End SyncLock ' commented for debugging 2022_09_29__15_34_06_9901

    End Function

    Public Function ImportFile() As String
        ' sample usage: http://119.93.147.163/offlinedata/importfile?OrgID=267DFE7A-B359-4C6E-99D4-BA11DD4C1F23&DeviceID=075E7F40-B841-47E8-9951-3666191D222A&Filename=2021_02_02__11_10_37_0486
        ' http://localhost:8080/offlinedata/importfile?OrgID=9d4b1500-13c0-49f9-a8e5-a16c13ec899e&DeviceID=9d4b1500-13c0-49f9-a8e5-a16c13ec899e&Filename=2022_06_13__12_39_10_0548
        ' http://localhost:8080/offlinedata/importfile?OrgID=f2d5c353-df14-46c8-a613-c6847fffda82&DeviceID=f2d5c353-df14-46c8-a613-c6847fffda82&Filename=2022_06_14__12_37_50_3882
        ' http://localhost:8080/offlinedata/importfile?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&DeviceID=EE9994D2-E9CA-42AC-9994-66C73081C1AC&Filename=2022_08_10__14_32_07_8141
        ' http://124.106.167.156:8083/offlinedata/importfile?OrgID=9D4B1500-13C0-49F9-A8E5-A16C13EC899E&DeviceID=B8A69761-7BD3-43A6-B2AA-95FB91728669&Filename=2022_08_24__12_02_21_4055
        ' optional: http://119.93.147.163/test/DeleteTransImportsData

        ' http://localhost:8080/offlinedata/importfile?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&DeviceID=b9014d0b-a8f3-433e-9b30-908ef2265809&Filename=2023_04_12__14_33_38_1933

        Try
            Dim pDataImport = New ImportDataClass
            Dim pOrgID = Common.WebRequest.QueryString("OrgID")
            Dim pDeviceID = Common.WebRequest.QueryString("DeviceID")
            Dim pFilename = Common.WebRequest.QueryString("Filename")
            pDataImport.OrgID = ReadyGuid(pOrgID)
            pDataImport.DeviceID = ReadyGuid(pDeviceID)
            pDataImport.InputFilename = pFilename
            pDataImport.Start()
            Return DateTimeStamp()
        Catch ex As Exception
            Return ex.Message & vbCrLf & HttpUtility.HtmlEncode(GetStackTrace(, ex)).Replace(vbCrLf, "<br />" & vbCrLf)
        End Try

    End Function

    Public Function Consolidate() As String

        Dim pProData = WebRequest.Instance.ProData
        Dim pDtClients = WebRequest.Instance.Business.LibraryOrganization.GetAll()
        Dim pDbs = pProData.GetDatabases(True, True)
        For Each pDrClient As DataRow In pDtClients.Rows
            If pDbs.Contains(CStr2(pDrClient("Database")).Trim.ToLower()) = False Then
                Continue For
            End If
            If ImportDataClass.FastFlushingOrgIDs().Contains(ReadyGuid(pDrClient("ID"))) = False Then
                Continue For
            End If
            Try
                Dim pDataImport = New ImportDataClass(False, pDrClient("Database"), True)
                Dim pOrgID = CStr2(pDrClient("ID"))
                pDataImport.OrgID = ReadyGuid(pOrgID)
                pDataImport.DeviceID = ""
                pDataImport.InputFilename = ""
                pDataImport.Start()
                Return DateTimeStamp()
            Catch ex As Exception
                Return ex.Message & vbCrLf & HttpUtility.HtmlEncode(GetStackTrace(, ex)).Replace(vbCrLf, "<br />" & vbCrLf)
            End Try
        Next pDrClient

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



    'Private Function LoadTransImport(vDeviceID As String) As DataRow

    '    Dim pDR As DataRow
    '    Dim pSql As String

    '    pSql = "SELECT Filename" &
    '        " FROM TransImports" &
    '        " WHERE DeviceID = " & ReadyString(vDeviceID)
    '    pDR = Common.WebRequest.ProData.ExecuteRow(pSql)

    '    Return pDR

    'End Function

    Private Function LoadTransImportVacant(vLatestFirst As Boolean, Optional vUnstartedOnly As Boolean = False,
        Optional vProData As ProData = Nothing,
        Optional vDescendingFileSize As Boolean = False,
        Optional vDontSortBySize As Boolean = False) As DataRow

        Dim pDR As DataRow
        Dim pSql As String
        Dim pSort As String = ""
        If vProData Is Nothing Then
            vProData = Common.WebRequest.ProData
        End If

        Try
            vProData.BeginTransaction()

            If vDontSortBySize = False Then
                pSort = "FileSize" & If(vDescendingFileSize, " DESC", "") & ", "
            End If
            pSort &= "DateReceived" & If(vLatestFirst, " DESC", "")

            pSql = "SELECT TOP 1 ID" & vbCrLf &
               ",Filename" &
               ",DeviceID" &
               ",DateReceived" &
               " FROM TransImports" &
               " WITH (HOLDLOCK, UPDLOCK)" & vbCrLf &
               " WHERE (" & vbCrLf &
               "   ProcessStart IS NULL" &
               "   OR (" & vbCrLf &
               "       DATEDIFF(minute, ProcessStart, GETDATE()) >= 30" & vbCrLf &
               "       AND ProcessEnd IS NULL" & vbCrLf &
               "   )" & vbCrLf &
               " )" & vbCrLf &
               " AND ProcessEnd IS NULL" & vbCrLf &
               " AND ServerIdentity  = " & ReadyString(modClient.ServerIdentity) & vbCrLf &
               " ORDER BY " & pSort
            pDR = vProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                Return pDR
            End If

            If vUnstartedOnly Then
                Return Nothing
            End If

            pSql = "SELECT TOP 1 Filename" &
                ",DeviceID" &
                ",DateReceived" &
                " FROM TransImports" &
                " WITH (HOLDLOCK, UPDLOCK)" & vbCrLf &
                " WHERE ProcessEnd IS NULL" &
                " AND ProcessEnd IS NULL" & vbCrLf &
                " AND ServerIdentity  = " & ReadyString(modClient.ServerIdentity) & vbCrLf &
                " ORDER BY " & pSort
            pDR = vProData.ExecuteRow(pSql)

            Return pDR
        Finally
            If pDR IsNot Nothing _
                AndAlso CStr2(pDR("ID")) <> "" Then

                pSql = "UPDATE TransImports" & vbCrLf &
                    " SET ProcessStart = GETDATE()" & vbCrLf &
                    " WHERE ID = " & ReadyString(pDR("ID"))
                vProData.ExecuteSQL(pSql)
            End If
            vProData.CommitTransaction()
        End Try

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
        Dim pWebPos = CBool2(Common.WebRequest.Data("WebPos"))

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
            Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. ref: vbUDR43S79RP!")
        End If

        pDeviceID = Common.WebRequest.Data("DeviceID")
        If CStr2(pDeviceID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data is missing.")
        End If
        If IsGuid(pDeviceID) = False Then
            Return JsonDictionary.Serialize("Error", "DeviceID POST data must be a valid GUID.")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        Dim pMustDebug = pDeviceID = "62420B7C-7824-4A80-8210-EE49A342C786"
        Try
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

            pOut = New JsonDictionary

            pSql = "pOrgID: " & pOrgID & vbCrLf &
                "pDeviceID: " & pDeviceID & vbCrLf &
                "GetCurrentDatabase(): " & Common.WebRequest.ProData.GetCurrentDatabase() & vbCrLf &
                "ref: vbNE3G5EWMAL!"
            AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")

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
                pSql = "pOrgID: " & pOrgID & vbCrLf &
                    "pDeviceID: " & pDeviceID & vbCrLf &
                    "has untagged record" & vbCrLf &
                    "ref: vbYF5UUXW7TK!"
                AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
                pFilename = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\" & pDR("Filename") & "_result.txt"
                If pMustDebug Then
                    AppendToLog("pFilename: " & pFilename, , "vbH44Q2Q28GK")
                End If
                If IO.File.Exists(pFilename) Then
                    AppendToLog("pFilename exist: " & pFilename, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
                    If pMustDebug Then
                        AppendToLog("vbGK8X6Z2P9R! file exist pFilename: " & pFilename, , "vbH44Q2Q28GK")
                    End If
                    If (CDec2(pDR("Success")) = 0) Then
                        pIU = New InsertUpdate("TransImports")
                        pIU("DownloadedDeviceDate") = "GETDATE()"
                        pIU("Success") = 0
                        pSql = pIU.GetUpdate("Filename = " & ReadyString(pDR("Filename")))
                        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' get the number of affected records
                    End If
                    pFileText = FileText(pFilename)
                    Dim pHasFilenameFromJson As Boolean
                    Try
                        pHasFilenameFromJson = HasFilenameFromJson(pFileText)
                    Catch ex As Exception
                        AppendToDebug("vb7PXPH38YWP! " & ex.Message & vbCrLf &
                            "pFilename = " & pFilename & vbCrLf &
                            "pFileText = " & pFileText, True, ex, True)
                        pSql = "DELETE FROM TransImports" & vbCrLf &
                            " WHERE DeviceID = " & ReadyString(pDeviceID) & vbCrLf &
                            " AND DownloadedDeviceDate IS NULL" & vbCrLf &
                            " AND Success IS NOT NULL" & vbCrLf & vbCrLf &
                            " AND Filename = " & ReadyString(pDR("Filename"))
                        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                        Throw
                    End Try
                    If pHasFilenameFromJson = False Then
                        AppendToLog("vbWDCX7EQPXX! pFileText: " & pFileText, , "vbU9X8J6CYG5")
                        pIU = New InsertUpdate("TransImports")
                        pIU("Success") = 0
                        pIU("DownloadedDeviceDate") = "GETDATE()"
                        pIU("Result") = ReadyString("filename not found. " & pFilename)
                        pSql = pIU.GetUpdate("Filename = " & ReadyString(pDR("Filename")))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                    If pMustDebug Then
                        AppendToLog("vb84EEH7XCJL! pFileText: " & pFileText, , "vbH44Q2Q28GK")
                    End If
                    Return pFileText
                End If
                AppendToLog("pFilename does not exist: " & pFilename, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")

                pIU = New InsertUpdate("TransImports")
                pIU("Success") = 0
                pIU("DownloadedDeviceDate") = "GETDATE()"
                pIU("Result") = ReadyString("file not found. " & pFilename)
                pSql = pIU.GetUpdate("Filename = " & ReadyString(pDR("Filename")))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pOut.Add("Error", "Import result file not found. vbB52NGCM5AE")
                pOut("MustFollowUp") = 1
                If pMustDebug Then
                    AppendToLog("vbMVMF8LSNXA! pOut.Serialize: " & pOut.Serialize, , "vbH44Q2Q28GK")
                End If
                Return pOut.Serialize
            Else
                pSql = "pOrgID: " & pOrgID & vbCrLf &
                    "pDeviceID: " & pDeviceID & vbCrLf &
                    "has no untagged record" & vbCrLf &
                    "ref: vbXRPYFTYFL2!"
                AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
            End If
            If pMustDebug Then
                AppendToLog("#1 vb62STYSS5G7", , "vbH44Q2Q28GK")
            End If

            ' no untagged record, so determine the current process
            ' BEGIN commented code for experiment 2022_10_05__09_59_15_0079
            'ImportOrganization(pOrgID, pMessage, pDataImport)
            'If pDataImport IsNot Nothing _
            '    AndAlso pDataImport.IsRunning _
            '    AndAlso pDataImport.DeviceID = pDeviceID Then
            '    pOut("Message") = "Data from this device is currently being imported to the server."
            '    pOut("MustFollowUp") = 1
            '    Return pOut.Serialize
            'End If
            ' END commented code for experiment 2022_10_05__09_59_15_0079

            ' BEGIN temp code for experiment 2022_10_05__09_59_15_0079
            If mDataImporters.ContainsKey(pOrgID) Then
                pDataImport = mDataImporters(pOrgID)
                If pDataImport.IsRunning _
                    AndAlso pDataImport.DeviceID = pDeviceID Then
                    pOut("Message") = "Data from this device is currently being imported to the server."
                    pOut("MustFollowUp") = 1

                    pSql = "pOrgID: " & pOrgID & vbCrLf &
                        "pDeviceID: " & pDeviceID & vbCrLf &
                        "GetCurrentDatabase(): " & Common.WebRequest.ProData.GetCurrentDatabase() & vbCrLf &
                        "pOut(""Message"") = " & pOut("Message") & vbCrLf &
                        "ref: vb8L4VBENW5C!"
                    AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
                    Return pOut.Serialize
                End If
            End If
            ' END temp code for experiment 2022_10_05__09_59_15_0079

            If pMustDebug Then
                AppendToLog("#2 vbGY5FUBWPNN", , "vbH44Q2Q28GK")
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
            If ReadyGuid(pOrgID) = ReadyGuid(modClient.DemoCorpOrgID) Then
                pSql = "pOrgID: " & pOrgID & vbCrLf &
                    "pDeviceID: " & pDeviceID & vbCrLf &
                    "pDR is nothing " & (pDR Is Nothing) & vbCrLf &
                    "pSql = " & pSql & vbCrLf &
                    "GetCurrentDatabase(): " & Common.WebRequest.ProData.GetCurrentDatabase() & vbCrLf &
                    "ref: vbMTF28PBX7H!"
                AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
            End If
            If pDR IsNot Nothing Then
                If pDataImport Is Nothing Then
                Else
                    pOut("Message") = "Data from another device (" & pDataImport.DeviceID & ") is currently being imported to the server. Data from this device will be imported later. Date received: " & Format(pDR("DateReceived"), cGlobal.FormatDateTime) & "."
                End If
                pOut("MustFollowUp") = 1
                'pSql = "pOrgID: " & pOrgID & vbCrLf &
                '        "pDeviceID: " & pDeviceID & vbCrLf &
                '        "GetCurrentDatabase(): " & Common.WebRequest.ProData.GetCurrentDatabase() & vbCrLf &
                '        "pOut(""Message"") = " & pOut("Message") & vbCrLf &
                '        "ref: vb8L4VBENW5C!"
                'AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
                If ReadyGuid(pOrgID) = ReadyGuid(modClient.DemoCorpOrgID) Then
                    pSql = "pOrgID: " & pOrgID & vbCrLf &
                        "pDeviceID: " & pDeviceID & vbCrLf &
                        "pDR is nothing " & (pDR Is Nothing) & vbCrLf &
                        "calling CallImportOrganization(" & pOrgID & ")" & vbCrLf &
                        "ref: vbV4JXC2Z7RZ!"
                    AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")
                End If
                CallImportOrganization(pOrgID)
                Return pOut.Serialize
            End If
            If pMustDebug Then
                AppendToLog("#3 vbB6TVR93K2G", , "vbH44Q2Q28GK")
            End If

            If pWebPos Then
                Dim pDeviceDataExporter = New DeviceDataExporter()
                pDeviceDataExporter.Business = WebRequest.Instance.Business
                pDeviceDataExporter.OrgID = pOrgID
                pDeviceDataExporter.DestinationDeviceID = pDeviceID
                Dim pExportedCount = pDeviceDataExporter.Export()
                If pExportedCount > 0 Then
                    pOut("DeviceDataExportKey") = pDeviceDataExporter.Key
                    Return pOut.Serialize
                End If
            End If

            ' tell the device to upload data
            pOut("MustUpload") = 1
            Dim pSettings = WebPosDbDownloaderController.GetWebPosSettings(WebRequest.Instance.Business)
            If String.IsNullOrWhiteSpace(pSettings.UUID) = False _
                AndAlso IsGuid(pSettings.DeviceID) Then

                pOut("DeviceID") = pSettings.DeviceID
            End If

            pSql = "pOrgID: " & pOrgID & vbCrLf &
                "pDeviceID: " & pDeviceID & vbCrLf &
                "GetCurrentDatabase(): " & Common.WebRequest.ProData.GetCurrentDatabase() & vbCrLf &
                "pOut.Serialize = " & pOut.Serialize & vbCrLf &
                "ref: vbBLGXYEDBPD!"
            AppendToLog(pSql, , "OfflineDataController_FollowUpImportData vbVNANVUFJFK")

            Return pOut.Serialize
        Catch ex As Exception
            AppendToDebug("vbvbPX6GU2TB8V! ", True, ex, True)
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
                Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. ref: vbUJYP9G5LUT!")
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

    <HttpGet()>
    Public Function DownloadImportResult() As ActionResult

        Dim pFile As IO.FileInfo
        Dim pFilename As String
        Dim pOrgID As String
        Dim pDeviceID As String
        Dim pCrc As String

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
                pCrc = ProPack.Security.Crypto.GetCRC32FromFile(pFilename)
                Response.AddHeader("CRC", pCrc)
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
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
            Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. ref: vbMF9446W5A7!")
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
            If WebRequest.Instance.Business.LibraryDevice.ValidDeviceFowst(
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

    Public Function DownloadDeviceData() As ActionResult

        Dim pFile As IO.FileInfo
        Dim pOrgID = Common.WebRequest.QueryString("OrgID")
        If IsGuid(pOrgID) = False Then
            Return Content("Error: OrgID must be a valid GUID.")
        End If
        Dim pKey = Common.WebRequest.QueryString("Key")
        If String.IsNullOrWhiteSpace(pKey) Then
            Return Content("Error: Key is required.")
        End If
        Dim pFilename = AppendBackslash(Common.ServerPath) & "temp\DeviceDataExporter\Outbound\" & pOrgID & "\" & pKey & ".zip"

        Try

            If IO.File.Exists(pFilename) = False Then
                Return Content("Error: Device data file does not exist")
            End If

            pFile = New IO.FileInfo(pFilename)
            If pFile.Exists Then
                Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture))
                Response.AddHeader("FileSize", pFile.Length)
                Response.ContentType = "application/" & IO.Path.GetExtension(mFilename)
                Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(mFilename, True))
                Response.TransmitFile(pFilename)
            Else
            End If
        Catch ex As Exception

        End Try

    End Function

    <HttpPost()>
    Public Function DumpImportedIDs() As String

        Dim pFilename As String
        Dim pTimestamp As String
        Dim pOrgID As String
        Dim pDeviceID As String
        Const pLogFilename As String = "OfflineData_DumpImportedIDs"
        Dim pError As String

        If Request.Files.Count > 0 Then
            pOrgID = Common.WebRequest.Data("OrgID")
            If CStr2(pOrgID).Trim = "" Then
                AppendToLog("OrgID POST data is missing from uploaded file.", , pLogFilename)
                Return JsonDictionary.ToString("Error", "OrgID POST data is missing from uploaded file.")
            End If
            If IsGuid(pOrgID) = False Then
                AppendToLog("OrgID POST data must be a valid GUID.", , pLogFilename)
                Return JsonDictionary.ToString("Error", "OrgID POST data must be a valid GUID.")
            End If
            pOrgID = ReadyGuid(pOrgID)
            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                AppendToLog("OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vbKXD9G9ES5X!", , pLogFilename)
                Return JsonDictionary.ToString("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vb4VZ9F92EWJ!")
            End If

            pDeviceID = Common.WebRequest.Data("DeviceID")
            If CStr2(pDeviceID).Trim = "" Then
                AppendToLog("DeviceID POST data is missing from uploaded file.", , pLogFilename)
                Return JsonDictionary.ToString("Error", "DeviceID POST data is missing from uploaded file.")
            End If
            If IsGuid(pDeviceID) = False Then
                AppendToLog("DeviceID POST data must be a valid GUID.", , pLogFilename)
                Return JsonDictionary.ToString("Error", "DeviceID POST data must be a valid GUID.")
            End If
            pDeviceID = ReadyGuid(pDeviceID)

            SyncLock mDumpLock ' prevent more than 1 thread inside
                pTimestamp = DateTimeStamp()
                Threading.Thread.Sleep(1000) ' make sure the time stamp is unique
            End SyncLock

            pFilename = Common.ServerPath & "temp\ImportedIDs\" & pOrgID & "\" & pDeviceID & "\" & pTimestamp & ".zip"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)

            Return JsonDictionary.ToString("TimeStamp", pTimestamp)
        Else
            AppendToLog("No file. vbH77NNXSPGZ!", , pLogFilename)
            Return JsonDictionary.ToString("Error", "No file. vbH77NNXSPGZ!")
        End If

    End Function

    <HttpPost>
    Public Function ProcessImportedIDs() As String

        Dim pError As String
        Dim pTimestamp As String
        Dim pOrgID As String
        Dim pDeviceID As String
        Const pLogFilename As String = "OfflineData_TagImportedDeviceData"

        pOrgID = Common.WebRequest.Data("OrgID")
        If CStr2(pOrgID).Trim = "" Then
            AppendToLog("OrgID POST data is missing from uploaded file.", , pLogFilename)
            Return JsonDictionary.ToString("Error", "OrgID POST data is missing from uploaded file.")
        End If
        If IsGuid(pOrgID) = False Then
            AppendToLog("OrgID POST data must be a valid GUID.", , pLogFilename)
            Return JsonDictionary.ToString("Error", "OrgID POST data must be a valid GUID.")
        End If
        pOrgID = ReadyGuid(pOrgID)
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            AppendToLog("OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vbZH8ETLQF9R!", , pLogFilename)
            Return JsonDictionary.ToString("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vbPD32PHNNGZ!")
        End If

        pDeviceID = Common.WebRequest.Data("DeviceID")
        If CStr2(pDeviceID).Trim = "" Then
            AppendToLog("DeviceID POST data is missing from uploaded file.", , pLogFilename)
            Return JsonDictionary.ToString("Error", "DeviceID POST data is missing from uploaded file.")
        End If
        If IsGuid(pDeviceID) = False Then
            AppendToLog("DeviceID POST data must be a valid GUID.", , pLogFilename)
            Return JsonDictionary.ToString("Error", "DeviceID POST data must be a valid GUID.")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        pTimestamp = Common.WebRequest.Data("TimeStamp")
        If String.IsNullOrWhiteSpace(pTimestamp) Then
            AppendToLog("TimeStamp is required. vb3BWVE575A4!", , pLogFilename)
            Return JsonDictionary.ToString("Error", "TimeStamp is required. vb3BWVE575A4!")
        End If

        Dim pFilename = Common.ServerPath & "temp\ImportedIDs\" & pOrgID & "\" & pDeviceID & "\" & pTimestamp & ".zip"
        If IO.File.Exists(pFilename) = False Then
            Return JsonDictionary.ToString("Error", "File does not exist. ref: vb7SW6TU3SZL")
        End If
        TagDatabaseTables("LibraryOrganizationCustomerCreditTerms", pDeviceID, pFilename)

        Return JsonDictionary.ToString("Result", "ok")

    End Function

    Private Shared Sub TagDatabaseTables(vTableName As String, vDeviceID As String, vFilename As String)

        Dim pTotalAffectedRecords = 0
        Dim pCsvReader = New CsvReader(vFilename, vTableName)
        Do
            Dim pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            Dim pIU = New InsertUpdate("TransDateImported")
            pIU("RowLastUpdated") = ReadyString(Format(CDate(pLine("LastUpdated")), cGlobal.FormatDateTimeSql))

            Dim pSql = "SELECT RowLastUpdated" & vbCrLf &
                " FROM TransDateImported" & vbCrLf &
                " WHERE TableName = " & ReadyString(vTableName) & vbCrLf &
                " AND RowID = " & ReadyString(pLine("ID")) & vbCrLf &
                " AND DeviceID =" & ReadyString(vDeviceID)
            Dim pDR = WebRequest.Instance.ProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                pIU("ID") = "NEWID()"
                pIU("DeviceID") = ReadyString(vDeviceID)
                pIU("TableName") = ReadyString(vTableName)
                pIU("RowID") = ReadyString(pLine("ID"))
                pSql = pIU.GetInsert()
            ElseIf pDR("RowLastUpdated") < pLine("LastUpdated") Then
                pSql = pIU.GetUpdate("RowID = " & ReadyString(pLine("ID")) & vbCrLf &
                    " AND TableName = " & ReadyString(vTableName) & vbCrLf &
                    " AND DeviceID = " & ReadyString(vDeviceID))
            Else
                Continue Do
            End If
            WebRequest.Instance.ProData.ExecuteSQL(pSql)
        Loop

    End Sub

    Public Function OpenSettings() As String

        Dim pResult As String

        ImportDataClass.OpenSettings(pResult)

        Return pResult.Replace(vbCrLf, "<br />")

    End Function

    Public Function CreateSettings() As String

        Dim pResult As String

        If ImportDataClass.CreateSettings(pResult) Then
            Return "Setting file was successfully created."
        Else
            Return pResult.Replace(vbCrLf, "<br />")
        End If

    End Function

    Public Function CanFlushNow() As String

        Dim pNowText = WebRequest.Instance.QueryString("Now")
        Dim pNow As DateTime?
        If String.IsNullOrWhiteSpace(pNowText) Then
            pNow = Now
        Else
            pNow = ParseDate(pNowText)
            If pNow Is Nothing Then
                Return "Now must be a valid time or blank. " & pNowText
            End If
        End If

        Dim pMessage As String
        Dim pOut = ImportDataClass.CanFlushNow(pNow, pMessage)
        pMessage = "Result: " & pOut & vbCrLf & pMessage
        Return pMessage.Replace(vbCrLf, "<br />")

    End Function

    Public Function UnflushedCount() As String

        Try
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pInput = JsonConvert.DeserializeObject(Of TransUnflushedCounts.InputClass)(WebRequest.Instance.InputStreamText)
            WebRequest.Instance.Business.TransUnflushedCounts.Save(pInput)
            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
End Class