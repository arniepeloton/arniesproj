Public Class DataCleaner
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Public Const RowID As String = "00000000-0000-0000-0000-000000000001"
    Dim mDate As DateTime = New DateTime(2000, 1, 1)
    Dim mExpectedCounts As ExpectedCounts
    Dim mDtSynonyms As DataTable

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Sub Execute()

        If Now.Date <> New Date(2023, 2, 1) Then
            Dim pMsg = "invalid date to run this function. vbZC44YGK686!"
            AddLog(pMsg)
            Throw New ApplicationException(pMsg)
        End If

        'mProData.BeginTransaction()
        mDtSynonyms = mProData.GetSynonyms()
        CreateRequiredData()
        DeleteData()
        CreateAdditionalData()
        CreateSelfTestData()
        CheckPrivateRecordCounts()
        CheckPublicRecordCounts()
        'mProData.RollbackTransaction()

    End Sub

    Private Sub CreateRequiredData()

        Dim pIU As InsertUpdate
        Dim pSql As String

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryPeople WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryPeople")
            pIU("ID") = ReadyString(RowID)
            pIU("Surname") = "'SuperAdmin'"
            pIU("MiddleName") = "'SuperAdmin'"
            pIU("FirstName") = "'SuperAdmin'"
            pIU("UserID") = ReadyString(RowID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryPeople WHERE ID = " & ReadyString(modClient.EmployeeWebGuidID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryPeople")
            pIU("ID") = ReadyString(EmployeeWebGuidID)
            pIU("Surname") = "'Web'"
            pIU("MiddleName") = "'Web'"
            pIU("FirstName") = "'Web'"
            pIU("UserID") = ReadyString(RowID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryOrganization")
            pIU("ID") = ReadyString(RowID)
            pIU("CompanyName") = "'My Company'"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryOrganizationTradeName WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryOrganizationTradeName")
            pIU("ID") = ReadyString(RowID)
            pIU("OrganizationID") = ReadyString(RowID)
            pIU("BusinessTradeName") = "'My Company Trade Name'"
            pIU("BusinessTradeNameInitial") = "'MCTN'"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
            pIU("ID") = ReadyString(RowID)
            pIU("OrganizationID") = ReadyString(RowID)
            pIU("Name") = "'My Fowst'"
            pIU("Code") = "'MF'"
            pIU("StreetNumber") = "'My Street Number'"
            pIU("FOWSTNo") = 1
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM LibraryReceiptSettingsFowst WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("LibraryReceiptSettingsFowst")
            pIU("ID") = ReadyString(RowID)
            pIU("ReceiptSettingsID") = ReadyString(RowID)
            pIU("FowstID") = ReadyString(RowID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryPeopleOrganization WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryPeopleOrganization")
            pIU("ID") = ReadyString(RowID)
            pIU("PeopleID") = ReadyString(RowID)
            pIU("OrganizationID") = ReadyString(RowID)
            pIU("IsConfirmed") = 1
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM LibraryOrganizationEmployee WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("LibraryOrganizationEmployee")
            pIU("ID") = ReadyString(RowID)
            pIU("PeopleID") = ReadyString(RowID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryPeopleUserAccount WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryPeopleUserAccount")
            pIU("ID") = ReadyString(RowID)
            pIU("PeopleID") = ReadyString(RowID)
            pIU("Username") = "'admin'"
            pIU("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString("admin"))
            pIU("IsActive") = 1
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM LibraryOrganizationEmployeeFOWST WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("LibraryOrganizationEmployeeFOWST")
            pIU("ID") = ReadyString(RowID)
            pIU("PeopleID") = ReadyString(RowID)
            pIU("FowstID") = ReadyString(RowID)
            pIU("IsActive") = 1
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Private Sub DeleteData()

        DeleteFromTable("CacheFc")
        DeleteFromTable("CacheFowstProducts")
        DeleteFromTable("CacheInventoryMovement")
        DeleteFromTable("LibraryAutoPrint")
        DeleteFromTable("LibraryDeMinimis")
        DeleteFromTable("LibraryDeMinimisEmployees")
        DeleteFromTable("LibraryDiscountSettings")
        DeleteFromTable("LibraryEcommerceBanners")
        DeleteFromTable("LibraryEcommerceProductQtyChoices")
        DeleteFromTable("LibraryEcommerceProductQtyChoicesCategories")
        DeleteFromTable("LibraryEmployeesBankAccount")
        DeleteFromTable("LibraryExpensesTicketUsers")
        DeleteFromTable("LibraryFowstSectionPeople")
        DeleteFromTable("LibraryFranchiseGroup")
        DeleteFromTable("LibraryFranchiseGroupCutOff")
        DeleteFromTable("LibraryFranchiseGroupCutOffDetails")
        DeleteFromTable("LibraryFranchiseGroupCutOffSOGroupNo")
        DeleteFromTable("LibraryFranchiseGroupHeads")
        DeleteFromTable("LibraryFranchiseGroupSortingListNo")
        DeleteFromTable("LibraryPIMForDeactivationAndReactivation")
        DeleteFromTable("LibraryPimCategories")
        DeleteFromTable("LibraryPimBrands")
        DeleteFromTable("TransDeviceLogs")
        DeleteFromTable("TransProfitability")
        DeleteFromTable("TransNetFundBalance")
        DeleteFromTable("LibraryOrganizationBankAccount")
        DeleteFromTable("LibraryOrganizationCustomerProduct")
        DeleteFromTable("LibraryOrganizationDashboardProductFeatures")
        DeleteFromTable("TransActionLogs")
        DeleteFromTable("TransCashAdvanceEmployeesPayment")
        DeleteFromTable("TransCustomerPaymentCheckAmountDraft")
        DeleteFromTable("TransCustomerPaymentChecksDraft")
        DeleteFromTable("TransCustomerPaymentDraft")
        DeleteFromTable("TransCustomerPaymentInvoicesDraft")
        DeleteFromTable("TransDTRDisbursement")
        DeleteFromTable("TransDTRDisbursementDetails")
        DeleteFromTable("TransEmployeeDeminimis")
        DeleteFromTable("TransEmployeeRequisitionsAndApprovals")
        DeleteFromTable("TransExpensesLogDetailsHistory")
        DeleteFromTable("TransGatePassDetails")
        DeleteFromTable("TransItemRequisitions")
        DeleteFromTable("TransPayroll")
        DeleteFromTable("TransPayrollDetails")
        DeleteFromTable("TransPayrollRequisition")
        DeleteFromTable("TransPriceUpdateRequest")
        DeleteFromTable("TransReceivingPaymentAppliedCredits")
        DeleteFromTable("TransReceivingPaymentOther")
        DeleteFromTable("TransReceivingStockSerials")
        DeleteFromTable("TransVendorPriceHistory")

        DeleteFromTable("MobizLibrarySSSContribution")
        DeleteFromTable("MobizLibraryPriceCanvassShareableLink")
        DeleteFromTable("MobizLibraryPIMTemplateColumns")
        DeleteFromTable("MobizLibraryPIMFeatureTmp")
        DeleteFromTable("MobizLibraryPIMTemplate")
        ' DeleteFromTable("MobizLibraryPIMFeature") ' dont delete the records of this table
        DeleteFromTable("MobizLibraryPhilHealthContribution")
        DeleteFromTable("MobizLibraryBIRTaxTable")
        DeleteFromTable("TransRentableAssetOtherCharges")
        DeleteFromTable("TransRentableAssetCustomerRequest")
        DeleteFromTable("TransRentableAssetCustomer")
        DeleteFromTable("TransPendingCardRequest")
        DeleteFromTable("TransLogExpensesCache")
        DeleteFromTable("TransInvoiceItemSerials")
        DeleteFromTable("TransExpensesLogDetails")
        DeleteFromTable("TransExpensesCheckInOutDetails")
        DeleteFromTable("TransDTRCache")
        DeleteFromTable("LibraryWageRegion")
        DeleteFromTable("LibrarySalesRep")
        DeleteFromTable("LibrarySalaryGradeEmployees")
        DeleteFromTable("LibrarySalaryGrade")
        DeleteFromTable("LibraryPriceCanvassAllowUserToLinkSKU")
        DeleteFromTable("LibraryModuleContract")
        DeleteFromTable("LibraryKnowledgeManagementArticleTopics")
        DeleteFromTable("LibraryExpensesCenters")
        DeleteFromTable("LibraryDTRViolationSanctions", , True)
        DeleteFromTable("LibraryDTRViolations", , True)
        DeleteFromTable("LibraryContractDesignerFreeTypeField")
        DeleteFromTable("LibraryContractDesignerTemplate")
        DeleteFromTable("LibraryOrganizationPriceLevel", " WHERE OrganizationID <> " & ReadyString(modClient.CashOrgID))
        DeleteFromTable("MobizLibraryEmployeeOrganizationRelationship")
        DeleteFromTable("LibraryPeopleEducation")
        DeleteFromTable("LibraryOrganizationContactWebsite")
        DeleteFromTable("LibraryOrganizationContactEmail")
        DeleteFromTable("LibraryEcommerceCustomers")
        DeleteFromTable("LibraryTableViewTemplate")
        DeleteFromTable("LibraryTableViewLayout")
        DeleteFromTable("TransSurveyRespondentScore")
        DeleteFromTable("TransSalesInvoiceAndRebatesHeader")
        DeleteFromTable("TransSalesInvoiceAndRebatesCache")
        DeleteFromTable("TransPeerMessages")
        DeleteFromTable("TransDTRTravelExpense")
        DeleteFromTable("LibraryVendorPrices")
        DeleteFromTable("LibraryTableTemplate")
        DeleteFromTable("LibraryPurchaseOrderSettings")
        DeleteFromTable("LibraryTableTableInfo")
        DeleteFromTable("LibraryTableFowst")
        DeleteFromTable("LibraryTableDiningArea")
        DeleteFromTable("LibrarySurveyRequirements")
        DeleteFromTable("LibrarySurveyQuestionFieldsTunnel")
        DeleteFromTable("LibrarySurveyQuestionFieldRequirements")
        DeleteFromTable("LibrarySurveyPage")
        DeleteFromTable("LibrarySurveyGroupRequirements")
        DeleteFromTable("LibrarySurveyFiles")
        DeleteFromTable("LibraryStoreCategoryTemplate")
        DeleteFromTable("LibraryStoreCategoryGroupPIM")
        DeleteFromTable("LibraryStoreCategoryGroup")
        DeleteFromTable("LibraryProjectRequisition")
        DeleteFromTable("LibraryProjectPO")
        DeleteFromTable("LibraryProjectMaterialsRequisition")
        DeleteFromTable("LibraryProjectJobRolePermission")
        DeleteFromTable("LibraryProjectJobRole")
        DeleteFromTable("LibraryProjectBudgetLaborCost")
        DeleteFromTable("LibraryProjectBudget")
        DeleteFromTable("LibraryPriceTagsLastPrintedDate")
        DeleteFromTable("LibraryPriceLevels [deletable]")
        DeleteFromTable("LibraryPimSupplier")
        DeleteFromTable("LibraryOrganizationSupplier")
        DeleteFromTable("LibraryOrganizationPIMPriceBreak")
        DeleteFromTable("LibraryOrganizationPIM")
        DeleteFromTable("LibraryOrganizationEmployeePrincipals")
        DeleteFromTable("TransCRMCachedActivity")
        DeleteFromTable("TransCRM")
        DeleteFromTable("TransCashHandling")
        DeleteFromTable("TransBomMakerContents")
        DeleteFromTable("TransBomMakerProducts")
        DeleteFromTable("TransBomMaker")
        DeleteFromTable("TablesLastUpdated")
        DeleteFromTable("LibrarySurveyResponse")
        DeleteFromTable("LibrarySurveyRespondent")
        DeleteFromTable("LibrarySurveyQuestions")
        DeleteFromTable("LibrarySurveyQuestionaire")
        DeleteFromTable("LibrarySurveyQuestionFields")
        DeleteFromTable("TransMemoSalesReturn")
        DeleteFromTable("TransInvoiceAdjustmentReturnedChecks")
        DeleteFromTable("TransInvoiceAdjustments")
        DeleteFromTable("TransMemo")
        DeleteFromTable("TransInvoiceCharges")
        DeleteFromTable("TransInvoiceDiscount")
        DeleteFromTable("TransInvoiceItemSalesReps")
        DeleteFromTable("TransInvoiceItemsModifier")
        DeleteFromTable("LibraryOrganizationEmployeeFOWST", " WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("LibraryOrganizationEmployee", " WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("LibraryOrganizationCustomer", " WHERE OrganizationID <> " & ReadyString(modClient.CashOrgID))
        DeleteFromTable("LibraryItemModifierCategories")
        DeleteFromTable("LibraryItemModifierInventoryAffecting") ' band aid solution
        DeleteFromTable("LibraryItemModifierNonInventoryAffecting") ' band aid solution
        DeleteFromTable("LibraryItemModifierRebates") ' band aid solution
        DeleteFromTable("LibraryItemModifiers") ' band aid solution
        DeleteFromTable("LibraryDevice")
        DeleteFromTable("LibraryDeviceFowst")
        DeleteFromTable("LibraryDeviceFowstPrinter")
        DeleteFromTable("LibraryDevicePrinter")
        DeleteFromTable("LibraryDevicePrinterCategory")
        DeleteFromTable("LibraryDevicePrinterFowst")
        DeleteFromTable("AuditTrailAndDashboardLog")
        DeleteFromTable("CacheInvoices")
        DeleteFromTable("CacheMonthlyCustomerInvoice")
        DeleteFromTable("CacheReceivings")
        DeleteFromTable("LibraryCRMActivityQuota")
        DeleteFromTable("LibraryCRMAttachments")
        DeleteFromTable("LibraryCRMFunnelStage")
        DeleteFromTable("LibraryCRMFunnelStageChild")
        DeleteFromTable("LibraryCRMFunnelTemplate")
        DeleteFromTable("LibraryCRMStage", , True)
        DeleteFromTable("LibraryCRMStageGroup", , True)
        DeleteFromTable("LibraryCRMStagingChecklist", , True)
        DeleteFromTable("LibraryCRMAttachments", , True)
        DeleteFromTable("LibraryCRMCustomers", , True)
        DeleteFromTable("LibraryCRMFunnelStage", , True)
        DeleteFromTable("LibraryCRMFunnelStageChild", , True)
        DeleteFromTable("LibraryCRMFunnelTemplate", , True)
        DeleteFromTable("LibraryCRMRMA", , True)
        DeleteFromTable("LibraryCRMRMAItems", , True)
        DeleteFromTable("LibraryCRMStagingChecklist")
        DeleteFromTable("LibraryCRMTeamMembers")
        DeleteFromTable("LibraryCRMTeamQuota")
        DeleteFromTable("LibraryCRMTeams")
        DeleteFromTable("LibraryCRMTicketNotes")
        DeleteFromTable("LibraryDepositAccounts")
        DeleteFromTable("LibraryFowstArea")
        DeleteFromTable("LibraryFowstBin")
        DeleteFromTable("LibraryFowstBinProduct")
        DeleteFromTable("LibraryFowstProduct")
        DeleteFromTable("TransInventoryLedger")
        DeleteFromTable("TransInventoryLedgerOldBalances")
        DeleteFromTable("LibraryDTRSchedule")
        DeleteFromTable("LibraryDTRScheduleEmployees")
        DeleteFromTable("LibraryDTRScheduleTime")
        DeleteFromTable("LibraryFowstProductBarcode")
        DeleteFromTable("LibraryInvoicingMethod")
        DeleteFromTable("LibraryInvoicingMethodCategory")
        DeleteFromTable("LibraryKnowledgeManagement")
        DeleteFromTable("LibraryPimCategoryGpm")
        DeleteFromTable("LibraryPimGroupPims")
        DeleteFromTable("LibraryPimGroups")
        DeleteFromTable("LibraryPimMenu")
        DeleteFromTable("LibraryPIMPictures")
        DeleteFromTable("LibraryPIMPriceTagsLastPrintedDate")
        DeleteFromTable("LibraryPriceCanvass")
        DeleteFromTable("LibraryPriceCanvassItems")
        DeleteFromTable("LibraryPriceCanvassItemSku")
        DeleteFromTable("LibraryPriceCanvassVendor")
        DeleteFromTable("LibraryPriceCanvassVendorPrice")
        DeleteFromTable("LibraryProductsWithoutBarcode")
        DeleteFromTable("LibraryProprietaryBarcodeType")
        DeleteFromTable("LibraryRentableAssets")
        DeleteFromTable("TransZReadCache")
        DeleteFromTable("TransSmsMessages")
        DeleteFromTable("TransSalesReturnLedger")
        DeleteFromTable("TransSalesOrderStatus")
        DeleteFromTable("TransReceivingPaymentDebitCard")
        DeleteFromTable("TransReceivingPaymentCreditCard")
        DeleteFromTable("TransReceivingPaymentChecks")
        DeleteFromTable("TransReceivingPaymentCheckAmount")
        DeleteFromTable("TransReceivingPaymentReceivings")
        DeleteFromTable("TransReceivingPayment")
        DeleteFromTable("TransReceivingStock")
        DeleteFromTable("TransReceiving")
        DeleteFromTable("TransPIMAlerts")
        DeleteFromTable("TransOfftake")
        DeleteFromTable("TransKnowledgeManagement")
        DeleteFromTable("TransInvoiceSeniorCitizenPWDCustomer")

        DeleteFromTable("TransCustomerPaymentCheckAmount")
        DeleteFromTable("TransCustomerPaymentChecks")
        DeleteFromTable("TransCustomerPaymentCreditCard")
        DeleteFromTable("TransCustomerPaymentDebitCard")
        DeleteFromTable("TransCustomerPaymentInvoices")
        DeleteFromTable("TransCustomerPaymentOther")
        DeleteFromTable("TransCustomerPaymentSRV")
        DeleteFromTable("TransCustomerPaymentType")
        DeleteFromTable("TransCustomerPayment")
        DeleteFromTable("TransCustomerTransactionSummaryCache")
        DeleteFromTable("TransInvoicePrints")
        DeleteFromTable("TransInvoiceItemWarehouseWithdrawal")
        DeleteFromTable("TransInvoiceItemWarehouseSections")
        DeleteFromTable("TransInvoiceItemsVoided")
        DeleteFromTable("TransInvoiceItemsRebates")
        mProData.ExecuteSQL("UPDATE TransInvoice SET InvoiceNumber = 0")
        DeleteFromTable("TransInvoiceItems")
        DeleteFromTable("TransCustomerPaymentCheckAmount")
        mProData.ExecuteSQL("UPDATE TransInvoice SET ParentOrderID = NULL")
        DeleteFromTable("TransInvoice")
        DeleteFromTable("LibraryRepeatingTransactions")
        DeleteFromTable("TransRentableAssets")
        DeleteFromTable("TransDailyBeginningBalance")
        DeleteFromTable("TransDocumentTracker")
        DeleteFromTable("TransDTR")
        DeleteFromTable("TransFranchiseRedFlag")
        DeleteFromTable("TransImports")
        DeleteFromTable("TransInventoryLedger")
        DeleteFromTable("TransInventoryLedgerOldBalances")
        DeleteFromTable("TransInventoryProductCount")
        DeleteFromTable("TransInventoryProductCountStock")

        DeleteFromTable("MobizTransRowCount")
        DeleteFromTable("MobizTransDeviceOrganizationPairing")
        DeleteFromTable("MobizTransDeviceLocationOrganization")
        DeleteFromTable("MobizTransDeviceLocation")
        DeleteFromTable("MobizTransDevAlerts")
        DeleteFromTable("MobizTemp")
        DeleteFromTable("MobizTestResults")
        DeleteFromTable("MobizTestResults")
        DeleteFromTable("MobizLibraryPIMProductDuplicate")
        DeleteFromTable("CacheDailyCustomerInvoice")
        DeleteFromTable("MobizLibraryPIMProductMeasurementBarcode")
        DeleteFromTable("MobizLibraryPIMProductMeasurement")
        DeleteFromTable("MobizLibraryPIMMeasureDefinition")
        DeleteFromTable("MobizLibraryPIMProductContents")
        DeleteFromTable("MobizTransSubscriptionUsage")
        DeleteFromTable("MobizLibrarySubscription", " WHERE ID <> " & ReadyString(RowID) & vbCrLf &
            " AND OrganizationID <> " & ReadyString(modClient.MobizTestDataOrgID))
        DeleteFromTable("MobizLibraryPIMProductPriceLevel")
        DeleteFromTable("MobizLibraryPeopleOrganization", " WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("MobizLibraryPeopleUserAccount", " WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("MobizLibraryPIMProductParent")
        DeleteFromTable("MobizLibraryPeople", "WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("MobizCacheInventoryBalancePerFowst")
        DeleteFromTable("MobizLibraryFranchiseGroupPriceLevels")
        DeleteFromTable("MobizLibraryOrganizationAffiliates")
        DeleteFromTable("MobizLibraryOrganizationBrand")
        DeleteFromTable("MobizLibraryOrganizationCategory")
        DeleteFromTable("MobizLibraryOrganizationContactEmail")
        DeleteFromTable("MobizLibraryOrganizationContactFax")
        DeleteFromTable("MobizLibraryOrganizationContactMobile")
        DeleteFromTable("MobizLibraryOrganizationContactPerson")
        DeleteFromTable("MobizLibraryOrganizationContactTel")
        DeleteFromTable("MobizLibraryOrganizationContactWebsite")
        DeleteFromTable("MobizLibraryOrganizationFinancialReportInfo")
        DeleteFromTable("MobizLibraryOrganizationFOWSTBusinessPermit")
        DeleteFromTable("MobizLibraryOrganizationFOWSTEmail")
        DeleteFromTable("MobizLibraryOrganizationFOWSTMallInfoAyala")
        DeleteFromTable("MobizLibraryOrganizationFOWSTMallInfoSM")
        DeleteFromTable("MobizLibraryOrganizationFOWSTMobile")
        DeleteFromTable("MobizLibraryOrganizationFOWSTOperationHRs")
        DeleteFromTable("MobizLibraryOrganizationFOWSTPOSSettings")
        DeleteFromTable("MobizLibraryOrganizationFOWSTSection")
        DeleteFromTable("MobizLibraryOrganizationFOWSTTel")
        DeleteFromTable("MobizLibraryOrganizationFranchisee")
        DeleteFromTable("MobizLibraryOrganizationFranchiseStatus")
        DeleteFromTable("MobizLibraryOrganizationIndustryClass")
        DeleteFromTable("MobizLibraryOrganizationOwner")
        DeleteFromTable("MobizLibraryOrganizationRepresentatives")
        DeleteFromTable("MobizLibraryOrganizationRepresentativesEmail")
        DeleteFromTable("MobizLibraryOrganizationRepresentativesMobile")
        DeleteFromTable("MobizLibraryOrganizationRepresentativesTel")
        DeleteFromTable("MobizLibraryOrganizationTradeName", " WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("MobizLibraryOrganizationFOWST", " WHERE ID <> " & ReadyString(RowID))
        DeleteFromTable("MobizLibraryOrganization", " WHERE ID <> " & ReadyString(RowID) & vbCrLf &
            " AND ID <> " & ReadyString(modClient.MobizTestDataOrgID) & vbCrLf &
            " AND ID <> " & ReadyString(modClient.CashOrgID) & vbCrLf &
            " AND ISNULL(IsABank, 0) = 0")
        DeleteFromTable("MobizLibraryPeopleAddress")
        DeleteFromTable("MobizLibraryPeopleAlias")
        DeleteFromTable("MobizLibraryPeopleContactEmail")
        DeleteFromTable("MobizLibraryPeopleContactMobile")
        DeleteFromTable("MobizLibraryPeopleContactTel")
        DeleteFromTable("MobizLibraryPeopleEducation")
        DeleteFromTable("MobizLibraryPeopleEmpHist")
        DeleteFromTable("MobizLibraryPeopleEmpHistOccupation")
        DeleteFromTable("MobizLibraryPeopleRelatedPerson")
        DeleteFromTable("MobizLibraryPeopleSecurityQuestion")
        DeleteFromTable("MobizLibraryPIMFeatureStructure")
        DeleteFromTable("MobizLibraryPIMProduct")
        DeleteFromTable("MobizLibraryPIMProductFamily")
        DeleteFromTable("MobizLibraryPIMProductFeature")
        DeleteFromTable("MobizLibraryPIMProductVariableFeatures001")
        DeleteFromTable("MobizLibraryPIMSupplier")
        DeleteFromTable("MobizLibraryPIMTemp")

        DeleteFromTable("LibrarySecurityRole")
        DeleteFromTable("LibrarySecurityRoleUser")
        DeleteFromTable("LibraryOrganization")
        DeleteFromTable("LibraryPeople")
        DeleteFromTable("LibraryPeopleAddress")
        DeleteFromTable("LibraryPeopleContactEmail")
        DeleteFromTable("LibraryPeopleContactMobile")
        DeleteFromTable("LibraryPeopleContactTel")
        DeleteFromTable("LibraryInvoiceSubTypes [deletable]")
        DeleteFromTable("LibraryOrganizationContactMobile")
        DeleteFromTable("LibraryOrganizationContactPerson")
        DeleteFromTable("LibraryOrganizationContactTel")
        DeleteFromTable("MobizLibraryPIMMeasureTmp")
        DeleteFromTable("MobizLibraryPIMPriceLevel", " WHERE ID NOT IN ('{01000012-0001-0000-0000-000000000000}'" & vbCrLf &
            ",'{01000012-0002-0000-0000-000000000000}'" & vbCrLf &
            ",'{01000012-0003-0000-0000-000000000000}'" & vbCrLf &
            ",'{01000012-0004-0000-0000-000000000000}'" & vbCrLf &
            ",'{01000012-0005-0000-0000-000000000000}'" & vbCrLf &
            ",'{01000012-0006-0000-0000-000000000000}'" & vbCrLf &
            ",'{01000012-0001-0000-0000-000000000000}')")
        DeleteFromTable("MobizLibrarySurveyQuestionaireLink")
        DeleteFromTable("MobizTransSMSTransactionMessages")
        DeleteFromTable("MobizTransSubscription")
        DeleteFromTable("MobizTransSubscriptionCart")
        DeleteFromTable("MobizLibraryUpdateLog")
        DeleteFromTable("MobizLibraryPIMTemplate", " WHERE OrganizationID NOT IN (" & ReadyString(RowID) & vbCrLf &
            "," & ReadyString(modClient.AbcOrgID) & ")")
        mProData.ExecuteSQL("UPDATE MobizLibraryPIMTemplate SET OrganizationID = " & ReadyString(RowID))

        DeleteFromTable("CacheProductSearch")
        DeleteFromTable("LibraryAssetOtherCharges")
        DeleteFromTable("LibraryOrganizationFinancialDetails")
        DeleteFromTable("LibraryProjectLogs")
        DeleteFromTable("LibraryProjectPhotos")

    End Sub

    Private Sub DeleteFromTable(vTableName As String, Optional vWhere As String = Nothing, Optional vOptional As Boolean = False)

        Dim pSql As String
        Dim pErrorCount As Integer

        If String.IsNullOrWhiteSpace(vWhere) Then
            Try
                AddLog("start TruncateTable " & vTableName & " vbKRGJBDDUZP!")
                TruncateTable(vTableName)
                AddLog("success TruncateTable " & vTableName & " vbEDBBNW8QNT!")
                Return
            Catch ex As Exception
                Try
                    Dim pDrs = mDtSynonyms.Select("synonym_name = " & ReadyString(vTableName))
                    For Each pDR In pDrs
                        pSql = "TRUNCATE TABLE " & pDR("synonym_definition")
                        AddLog("start " & pSql & " vbUM56J93MDT!")
                        Try
                            mProData.Execute(pSql)
                            AddLog("success " & pSql & " vb7UWN8VYEJW!")
                            Return
                        Catch ex2 As Exception
                            AddLog("failed " & pSql & " vbK36U63LCKC! " & vbCrLf & ex2.Message)
                        End Try
                    Next
                Catch ex3 As Exception
                    AddLog("Error while resolving synonym for truncate table: " & ex3.Message & " vbYNHLQGFCVM!")
                End Try
            End Try
        End If

        If vTableName.ToLower().StartsWith(("Mobiz".ToLower())) = False Then
            If mProData.TableExist(vTableName) = False Then
                Return
            End If
        End If

        mProData.CommandTimeout = 10 * 60 * 1000
        Dim pSkippedRowNum As Integer
        Dim pRecordsToBeDeleted = 1000
        Do
            Try
                pSql = " WITH CTE AS (" & vbCrLf &
                    "   SELECT ROW_NUMBER() OVER (ORDER BY ID) AS RowNum_LCSUMR3T3V" & vbCrLf &
                    "   ,*" & vbCrLf &
                    "   FROM " & EscapeSqlName(vTableName) & vbCrLf &
                    vWhere & vbCrLf &
                    " )" & vbCrLf &
                    " DELETE FROM CTE" & vbCrLf &
                    " WHERE RowNum_LCSUMR3T3V >= " & pSkippedRowNum + 1 & vbCrLf &
                    " AND RowNum_LCSUMR3T3V <= " & pSkippedRowNum + pRecordsToBeDeleted
                AddLog("DeleteFromTable " & vTableName & " " & pSql & vbCrLf & "vbJLA7T6BYBL!")
                Dim pRecordCount = mProData.ExecuteSQL(pSql, True)
                If pRecordCount = 0 Then
                    Exit Do
                ElseIf pRecordCount >= 500 Then
                    AddLog("DeleteFromTable " & vTableName & " remaining records: " & Format(pRecordCount, cGlobal.FormatInteger) & " vbEWS4S7K285!")
                    Debug.WriteLine("DeleteFromTable: table " & vTableName & " still has deletable records. ref: vbTJRNCX8NFE!")

                    'If vTableName.ToLower().Contains("pimproduct") Then
                    '    Throw New ApplicationException("DE7CN2QZPH Unexpected Error XC275RT7XR") ' for debugging only
                    'End If
                End If
                pRecordsToBeDeleted = 1000
            Catch ex As Exception
                If ex.Message.ToLower().Contains("unexpected error") Then
                    If pRecordsToBeDeleted > 1 Then
                        pRecordsToBeDeleted \= 10
                        pErrorCount = 0
                    Else
                        pSkippedRowNum += 1
                        pRecordsToBeDeleted = 1000
                    End If
                End If
                AddLog("Error from DeleteFromTable: " & ex.Message & vbCrLf &
                    "SQL: " & pSql & vbCrLf &
                    "vbHAYKH2HNB2!")
                pErrorCount += 1
                If pErrorCount > 10 Then
                    Throw
                End If
            End Try
        Loop

    End Sub

    Private Sub TruncateTable(vTableName As String)

        Dim pSql As String

        pSql = "TRUNCATE TABLE " & EscapeSqlName(vTableName)
        mProData.ExecuteSQL(pSql)

    End Sub

    Private Sub CreateAdditionalData()

        Dim pIU As InsertUpdate
        Dim pSql As String

        If mProData.ExecuteRow("SELECT ID FROM MobizLibrarySubscription WHERE ID = " & ReadyString(RowID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibrarySubscription")
            pIU("ID") = ReadyString(RowID)
            pIU("OrganizationID") = ReadyString(RowID)
            pIU("[Database]") = ReadyString("MobizABC")
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM LibrarySecurityRole WHERE ID = " & ReadyString(SecurityAccess.SysAdminRoleID)) Is Nothing Then
            pIU = New InsertUpdate("LibrarySecurityRole")
            pIU("ID") = ReadyString(SecurityAccess.SysAdminRoleID)
            pIU("Code") = "'SysAdmin'"
            pIU("Description") = "'System Administrator'"
            pIU("IsActive") = 1
            pIU("UserID") = ReadyString(RowID)
            pIU("LastUpdated") = "GETDATE()"
            pIU("IsCompleteRetail") = "'True'"
            pSql = pIU.GetInsert()
            Try
                mProData.ExecuteSQL(pSql)
            Catch ex As Exception
                If ex.Message = "Invalid column name 'True'." Then
                    pIU("IsCompleteRetail") = 1
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                Else
                    Throw
                End If
            End Try
        End If
        Call New SecurityAccess().GrantAllToSysAdmin(mProData)

        pIU = New InsertUpdate("LibrarySecurityRoleUser")
        pIU("ID") = ReadyString(RowID)
        pIU("RoleID") = ReadyString(SecurityAccess.SysAdminRoleID)
        pIU("PeopleID") = ReadyString(RowID)
        pIU("UserID") = ReadyString(RowID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Private Sub CreateSelfTestData()

        Dim pIU As InsertUpdate
        Dim pSql As String

        If mProData.ExecuteRow("SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(modClient.MobizTestDataOrgID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryOrganization")
            pIU("ID") = ReadyString(modClient.MobizTestDataOrgID)
            pIU("CompanyName") = "'My Test Company'"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        If mProData.ExecuteRow("SELECT ID FROM MobizLibrarySubscription WHERE OrganizationID = " & ReadyString(modClient.MobizTestDataOrgID)) Is Nothing Then
            pIU = New InsertUpdate("MobizLibrarySubscription")
            pIU("ID") = "NEWID()"
            pIU("OrganizationID") = ReadyString(modClient.MobizTestDataOrgID)
            pIU("[Database]") = ReadyString("MobizDataTest")
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Private Sub AddSecurityData()

    End Sub

    Private Sub CheckPrivateRecordCounts()

        mExpectedCounts = New ExpectedCounts()

        mExpectedCounts.Add("LibraryOrganizationEmployee", 1, 1)
        mExpectedCounts.Add("LibraryAccountAdjustments", 20, 200) ' band aid solution
        mExpectedCounts.Add("LibraryInvoiceTypes", 9, 9) ' band aid solution
        mExpectedCounts.Add("LibraryOrganizationCustomer", 1, 1)
        mExpectedCounts.Add("LibraryOrganizationDecimalSetting", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibraryOrganizationPriceLevel", 1, 9999) ' band aid solution
        mExpectedCounts.Add("LibraryPaymentTerms", 0, 999999) ' band aid solution

        mExpectedCounts.Add("LibraryReceiptSettings", 1, 2) ' band aid solution
        mExpectedCounts.Add("LibraryReceiptSettingsFowst", 1, 99999) ' band aid solution
        mExpectedCounts.Add("LibraryReceiptSettingsMessage", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibrarySecurityModule", 100, 99999) ' band aid solution
        mExpectedCounts.Add("LibrarySecurityModuleMain", 1, 99999) ' band aid solution
        mExpectedCounts.Add("LibrarySecurityRole", 1, 1)
        mExpectedCounts.Add("LibrarySecurityRoleModules", 100, 99999)
        mExpectedCounts.Add("LibrarySecurityRoleUser", 1, 1)
        mExpectedCounts.Add("LibraryWorkPhase", 6, 6) ' band aid solution
        mExpectedCounts.Add("SystemDefaults", 1, 999999)
        mExpectedCounts.Add("LibraryOrganizationEmployeeFOWST", 1, 1)
        mExpectedCounts.Add("LibraryProjectPermission", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryDocumentControl", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryDTRRole", 1, Integer.MaxValue)

        AssertRecordCounts()

    End Sub

    Private Sub CheckPublicRecordCounts()

        mProData.Database = "Mobiz"
        mProData.ExecuteSQL("USE Mobiz")

        mExpectedCounts = New ExpectedCounts()
        mExpectedCounts.Add("LibraryOrganizationTradeName", 1, 1)
        mExpectedCounts.Add("LibraryOrganizationFOWST", 1, 1)

        ' 2017_04_19__11_06_05_9232
        mExpectedCounts.Add("LibraryAreaCodes", 176, 176) ' band aid solution
        mExpectedCounts.Add("LibraryBankBranch", 1266, 1266) ' band aid solution
        mExpectedCounts.Add("LibraryBanks", 189, 189) ' band aid solution
        mExpectedCounts.Add("LibraryCities", 224, 224) ' band aid solution
        mExpectedCounts.Add("LibraryCitizenship", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibraryCountries", 249, 249) ' band aid solution
        mExpectedCounts.Add("LibraryCountriesLanguage", 2, 2) ' band aid solution
        mExpectedCounts.Add("LibraryCountriesLanguageLabel", 100, 99999) ' band aid solution
        mExpectedCounts.Add("LibraryEducationCourse", 1423, 1423) ' band aid solution
        mExpectedCounts.Add("LibraryFOWSTRetailStoreType", 11, 11) ' band aid solution
        mExpectedCounts.Add("LibraryFOWSTType", 6, 6) ' band aid solution
        mExpectedCounts.Add("LibraryFOWSTWarehouseType", 11, 11) ' band aid solution
        mExpectedCounts.Add("LibraryIndustry", 21, 21) ' band aid solution
        mExpectedCounts.Add("LibraryIndustryClass", 1271, 1271) ' band aid solution
        mExpectedCounts.Add("LibraryIndustryGroup", 245, 245) ' band aid solution
        mExpectedCounts.Add("LibraryJobOccupation", 3273, 3273) ' band aid solution
        mExpectedCounts.Add("LibraryOrganization", 3, 999) ' including banks
        mExpectedCounts.Add("LibraryOrgCategory", 32, 32) ' band aid solution
        mExpectedCounts.Add("LibraryOrgType", 792, 792) ' band aid solution
        mExpectedCounts.Add("LibraryPeople", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibraryPeopleOrganization", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibraryPeoplePostNominalSuffix", 392, 392) ' band aid solution
        mExpectedCounts.Add("LibraryPeoplePreNominalSuffix", 135, 135) ' band aid solution
        mExpectedCounts.Add("LibraryPeopleUserAccount", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibraryPIMMeasure", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryPIMMeasureConversion", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryPIMMeasureSign", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryPIMPriceLevel", 6, 6)
        mExpectedCounts.Add("LibraryPIMProductBOM", 1, 1) ' band aid solution
        mExpectedCounts.Add("LibraryRegisterForUpdates", 8, 8) ' band aid solution
        mExpectedCounts.Add("LibrarySubscription", 2, 2)
        mExpectedCounts.Add("LibrarySubscriptionModules", 2, 2) ' band aid solution
        mExpectedCounts.Add("LibraryTelcoPrefixes", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryTelcos", 1, Integer.MaxValue)
        mExpectedCounts.Add("LibraryUNSPSCClass", 3855, 3855) ' band aid solution
        mExpectedCounts.Add("LibraryUNSPSCCommodity", 49716, 49716) ' band aid solution
        mExpectedCounts.Add("LibraryUNSPSCCommodityFeature", 101, 101) ' band aid solution
        mExpectedCounts.Add("LibraryUNSPSCFamily", 423, 423) ' band aid solution
        mExpectedCounts.Add("LibraryUNSPSCSegment", 56, 56) ' band aid solution
        mExpectedCounts.Add("SystemDefaults", 1, Integer.MaxValue)
        'mExpectedCounts.Add("LibraryPIMTemplate", 1, Integer.MaxValue)
        'mExpectedCounts.Add("LibraryPIMTemplateColumns", 1, Integer.MaxValue)

        AssertRecordCounts()

    End Sub

    Sub AssertRecordCounts()

        Dim pDataDeleter As DataDeleter
        Dim pDT As DataTable
        Dim pTableName As String
        Dim pProblem As String = ""
        Dim pBandAid As String = ""

        pDataDeleter = New DataDeleter()
        pDataDeleter.ProData = mProData
        pDataDeleter.Database = mProData.Database
        pDT = pDataDeleter.PopTables()
        For Each pDR As DataRow In pDT.Rows
            pTableName = pDR("Name")
            Dim pExpectedCount As ExpectedCount = (
                From p As ExpectedCount In mExpectedCounts
                Where p.TableName.ToLowerInvariant() = pTableName.ToLower()
                Select p
            ).SingleOrDefault()

            If pExpectedCount Is Nothing Then
                pExpectedCount = New ExpectedCount() With {.TableName = pTableName}
            End If

            If pDR("Rows") < pExpectedCount.MinCount Then
                pProblem &= "The record count (" & CStr2(pDR("Rows")) & ") of table (" & pTableName & ") is lesser than the minimum expected record count (" & pExpectedCount.MinCount & "). ref: vb6MPC9P9VBN!" & vbCrLf
                pBandAid &= "mExpectedCounts.Add(""" & pTableName & """ , " & CStr2(pDR("Rows")) & ", " & CStr2(pDR("Rows")) & ") ' band aid solution" & vbCrLf
            End If

            If pExpectedCount.MaxCount < pDR("Rows") Then
                pProblem &= "The record count (" & CStr2(pDR("Rows")) & ") of table (" & pTableName & ") is greater than the maximum expected record count (" & pExpectedCount.MaxCount & "). ref: vb38WGGWM94X!" & vbCrLf
                pBandAid &= "mExpectedCounts.Add(""" & pTableName & """ , " & CStr2(pDR("Rows")) & ", " & CStr2(pDR("Rows")) & ") ' band aid solution" & vbCrLf
            End If
        Next pDR

        If String.IsNullOrWhiteSpace(pProblem) = False Then
            AddLog("vbSMW4NPJHPQ!" & vbCrLf & pProblem & vbCrLf & vbCrLf & pBandAid)
            Throw New ApplicationException(pProblem & vbCrLf & vbCrLf & pBandAid)
        End If

    End Sub

    Public Shared Sub AddLog(vText As String)

        AppendToLog(vText, , "DataCleaner vbPGBRQHPUYY")

    End Sub

    Private Class ExpectedCounts
        Inherits List(Of ExpectedCount)

        Public Function Add(vTableName As String, vMinCount As Integer, vMaxCount As Integer, Optional vCondition As String = Nothing) As ExpectedCount

            Dim pOut As ExpectedCount

            pOut = New ExpectedCount
            pOut.TableName = vTableName
            pOut.MinCount = vMinCount
            pOut.MaxCount = vMaxCount
            pOut.Condition = vCondition
            MyBase.Add(pOut)

            Return pOut

        End Function
    End Class

    Private Class ExpectedCount
        Public TableName As String
        Public Condition As String
        Public MinCount As Integer
        Public MaxCount As Integer
    End Class
End Class
