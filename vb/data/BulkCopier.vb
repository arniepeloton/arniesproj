Public Class BulkCopier
    Public OrganizationID As String
    Public SourceProData As ProData
    Public DestinationConnection As SqlConnection

    Public Sub CopyPublic()

        CopyTable("LibraryAreaCodes")
        CopyTable("LibraryBankBranch")
        CopyTable("LibraryBanks")
        'CopyTable("LibraryCampaign")
        'CopyTable("LibraryCampaignCoupon")
        'CopyTable("LibraryCampaignTargetAdCity")
        'CopyTable("LibraryCampaignTargetAds")
        CopyTable("LibraryCities")
        CopyTable("LibraryCitizenship")
        CopyTable("LibraryCountries")
        CopyTable("LibraryCountriesLanguage")
        CopyTable("LibraryCountriesLanguageLabel")
        CopyTable("LibraryDevicePolicyMostStrict")
        CopyTable("LibraryEducationCourse")
        CopyTable("LibraryFOWSTRetailStoreType")
        CopyTable("LibraryFOWSTType")
        CopyTable("LibraryFOWSTWarehouseType")
        CopyTable("LibraryIndustry")
        CopyTable("LibraryIndustryClass")
        CopyTable("LibraryIndustryGroup")
        CopyTable("LibraryJobOccupation")

        CopySql("SELECT * " &
            " FROM LibraryOrganization" &
            " WHERE ID = " & ReadyString(OrganizationID), "LibraryOrganization")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationAffiliates" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationAffiliates")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationBrand" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationBrand")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationCategory" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationCategory")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationContactEmail" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationContactEmail")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationContactFax" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationContactFax")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationContactMobile" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationContactMobile")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationContactTel" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationContactTel")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationContactWebsite" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationContactWebsite")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationFinancialReportInfo" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFinancialReportInfo")
        CopySql("SELECT * " &
            " FROM LibraryOrganizationFOWST" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWST")
        CopySql("SELECT LibraryOrganizationFOWSTBusinessPermit.* " &
            " FROM LibraryOrganizationFOWSTBusinessPermit" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTBusinessPermit.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTBusinessPermit")
        CopySql("SELECT LibraryOrganizationFOWSTEmail.* " &
            " FROM LibraryOrganizationFOWSTEmail" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTEmail.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTEmail")

        CopySql("SELECT LibraryOrganizationFOWSTLTFRB.* " &
            " FROM LibraryOrganizationFOWSTLTFRB" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTLTFRB.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTLTFRB")

        CopySql("SELECT LibraryOrganizationFOWSTLTO.* " &
            " FROM LibraryOrganizationFOWSTLTO" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTLTO.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTLTO")

        CopySql("SELECT LibraryOrganizationFOWSTMallInfoAyala.* " &
            " FROM LibraryOrganizationFOWSTMallInfoAyala" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTMallInfoAyala.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTMallInfoAyala")

        CopySql("SELECT LibraryOrganizationFOWSTMallInfoSM.* " &
            " FROM LibraryOrganizationFOWSTMallInfoSM" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTMallInfoSM.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTMallInfoSM")

        CopySql("SELECT LibraryOrganizationFOWSTMobile.* " &
            " FROM LibraryOrganizationFOWSTMobile" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTMobile.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTMobile")

        CopySql("SELECT LibraryOrganizationFOWSTOperationHRs.* " &
            " FROM LibraryOrganizationFOWSTOperationHRs" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTOperationHRs.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTOperationHRs")

        CopySql("SELECT LibraryOrganizationFOWSTPOSSettings.* " &
            " FROM LibraryOrganizationFOWSTPOSSettings" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTPOSSettings.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTPOSSettings")

        CopySql("SELECT LibraryOrganizationFOWSTPrinter.* " &
            " FROM LibraryOrganizationFOWSTPrinter" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTPrinter.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTPrinter")

        CopySql("SELECT LibraryOrganizationFOWSTSection.* " &
            " FROM LibraryOrganizationFOWSTSection" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTSection.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTSection")

        CopySql("SELECT LibraryOrganizationFOWSTTel.* " &
            " FROM LibraryOrganizationFOWSTTel" &
            " JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFOWSTTel.FOWSTID" &
            " WHERE LibraryOrganizationFOWST.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationFOWSTTel")

        'CopyTable("LibraryOrganizationFranchisee")       ' dont copy from these tables
        'CopyTable("LibraryOrganizationFranchiseStatus")  ' dont copy from these tables

        CopySql("SELECT * " &
            " FROM LibraryOrganizationIndustryClass" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationIndustryClass")

        CopySql("SELECT * " &
            " FROM LibraryOrganizationOwner" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationOwner")

        CopySql("SELECT * " &
            " FROM LibraryOrganizationPIMTemplate" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationPIMTemplate")

        CopySql("SELECT LibraryOrganizationPIMTemplateColumns.* " &
            " FROM LibraryOrganizationPIMTemplateColumns" &
            " JOIN LibraryOrganizationPIMTemplate ON LibraryOrganizationPIMTemplate.ID = LibraryOrganizationPIMTemplateColumns.PIMTemplateID" &
            " WHERE LibraryOrganizationPIMTemplate.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationPIMTemplateColumns")

        CopySql("SELECT * " &
            " FROM LibraryOrganizationRepresentatives" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationRepresentatives")

        CopySql("SELECT LibraryOrganizationRepresentativesEmail.* " &
            " FROM LibraryOrganizationRepresentativesEmail" &
            " JOIN LibraryOrganizationRepresentatives ON LibraryOrganizationRepresentatives.ID = LibraryOrganizationRepresentativesEmail.OrgRepID" &
            " WHERE LibraryOrganizationRepresentatives.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationRepresentativesEmail")

        CopySql("SELECT LibraryOrganizationRepresentativesMobile.* " &
            " FROM LibraryOrganizationRepresentativesMobile" &
            " JOIN LibraryOrganizationRepresentatives ON LibraryOrganizationRepresentatives.ID = LibraryOrganizationRepresentativesMobile.OrgRepID" &
            " WHERE LibraryOrganizationRepresentatives.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationRepresentativesMobile")

        CopySql("SELECT LibraryOrganizationRepresentativesTel.* " &
            " FROM LibraryOrganizationRepresentativesTel" &
            " JOIN LibraryOrganizationRepresentatives ON LibraryOrganizationRepresentatives.ID = LibraryOrganizationRepresentativesTel.OrgRepID" &
            " WHERE LibraryOrganizationRepresentatives.OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationRepresentativesTel")

        CopySql("SELECT * " &
            " FROM LibraryOrganizationTradeName" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryOrganizationTradeName")

        CopyTable("LibraryOrgCategory")
        CopyTable("LibraryOrgType")

        CopySql("SELECT LibraryPeople.* " &
            " FROM LibraryPeople" &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID" &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeople")

        CopySql("SELECT LibraryPeopleAddress.* " &
            " FROM LibraryPeopleAddress" &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleAddress.PeopleID" &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID" &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleAddress")

        CopySql("SELECT LibraryPeopleAlias.* " &
            " FROM LibraryPeopleAlias" &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleAlias.PeopleID" &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID" &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleAlias")

        CopySql("SELECT LibraryPeopleAliasPostSuffix.*  " & vbCrLf &
            " FROM LibraryPeopleAliasPostSuffix" & vbCrLf &
            " JOIN LibraryPeopleAlias ON LibraryPeopleAlias.ID = LibraryPeopleAliasPostSuffix.AliasID" & vbCrLf &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleAlias.PeopleID " & vbCrLf &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID " & vbCrLf &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleAliasPostSuffix")

        CopySql("SELECT LibraryPeopleContactEmail.*  " & vbCrLf &
            " FROM LibraryPeopleContactEmail" & vbCrLf &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleContactEmail.PeopleID " & vbCrLf &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID " & vbCrLf &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleContactEmail")

        CopySql("SELECT LibraryPeopleContactMobile.*  " & vbCrLf &
            " FROM LibraryPeopleContactMobile" & vbCrLf &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleContactMobile.PeopleID " & vbCrLf &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID " & vbCrLf &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleContactMobile")

        CopySql("SELECT LibraryPeopleContactTel.*  " & vbCrLf &
            " FROM LibraryPeopleContactTel" & vbCrLf &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleContactTel.PeopleID " & vbCrLf &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID " & vbCrLf &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleContactTel")

        CopySql("SELECT LibraryPeopleEducation.*  " & vbCrLf &
            " FROM LibraryPeopleEducation" & vbCrLf &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleEducation.PeopleID " & vbCrLf &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID " & vbCrLf &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleEducation")

        CopySql("SELECT LibraryPeopleEmpHist.*  " & vbCrLf &
            " FROM LibraryPeopleEmpHist" & vbCrLf &
            " JOIN LibraryPeople ON LibraryPeople.ID = LibraryPeopleEmpHist.PeopleID " & vbCrLf &
            " JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeople.ID " & vbCrLf &
            " WHERE LibraryPeopleOrganization.OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleEmpHist")

        'CopyTable("LibraryPeopleEmpHistOccupation")         ' todo

        CopySql("SELECT * " &
            " FROM LibraryPeopleOrganization" &
            " WHERE OrganizationID = " & ReadyString(OrganizationID), "LibraryPeopleOrganization")
        'CopyTable("LibraryPeoplePostNominalSuffix")         ' todo
        'CopyTable("LibraryPeoplePostSuffix")                ' todo
        'CopyTable("LibraryPeoplePreNominalSuffix")          ' todo
        'CopyTable("LibraryPeopleRelatedPerson")             ' todo
        'CopyTable("LibraryPeopleRelatedPersonPostSuffix")   ' todo
        'CopyTable("LibraryPeopleSecurityQuestion")          ' todo
        'CopyTable("LibraryPeopleUserAccount")               ' todo
        'CopyTable("LibraryPIMFeature")
        'CopyTable("LibraryPIMFeatureGroup")
        'CopyTable("LibraryPIMFeatureMeasurements")
        'CopyTable("LibraryPIMFeatureStructure")
        'CopyTable("LibraryPIMFeatureTmp")
        'CopyTable("LibraryPIMMeasure")
        'CopyTable("LibraryPIMMeasureConversion")
        'CopyTable("LibraryPIMMeasureSign")
        'CopyTable("LibraryPIMMeasureTmp")
        'CopyTable("LibraryPIMPriceLevel")
        'CopyTable("LibraryPIMProduct")
        'CopyTable("LibraryPIMProductBOM")
        'CopyTable("LibraryPIMProductDescription")
        'CopyTable("LibraryPIMProductDuplicate")
        'CopyTable("LibraryPIMProductFamily")
        'CopyTable("LibraryPIMProductFeature")
        'CopyTable("LibraryPIMProductMeasurement")
        'CopyTable("LibraryPIMProductMeasurementBarcode")
        'CopyTable("LibraryPIMProductPriceLevel")
        'CopyTable("LibraryPIMProductSubscription")
        'CopyTable("LibraryPIMProductVariableFeatures001")
        'CopyTable("LibraryPIMProductVariableFeatures002")
        'CopyTable("LibraryPIMTemp")
        'CopyTable("LibraryPIMTemplate")
        'CopyTable("LibraryPIMTemplateColumns")
        'CopyTable("LibraryRegisterForUpdates")
        'CopyTable("LibrarySizesODB")
        'CopyTable("LibrarySubscription") ' todo
        'CopyTable("LibrarySubscriptionModules") ' todo
        CopyTable("LibraryTelcoPrefixes")
        CopyTable("LibraryTelcos")
        CopyTable("LibraryUNSPSCClass")
        CopyTable("LibraryUNSPSCCommodity")
        CopyTable("LibraryUNSPSCCommodityFeature")
        CopyTable("LibraryUNSPSCCommodityFeatureGroup")
        CopyTable("LibraryUNSPSCFamily")
        CopyTable("LibraryUNSPSCSegment")
        'CopyTable("LibraryUpdateLog")
        'CopyTable("SystemDefaults") ' todo
        'CopyTable("TestResults")
        'CopyTable("TransDeviceLocation")
        'CopyTable("TransOnlineStoreSubscriber")
        'CopyTable("TransOnlineStoreSubscriberProduct")

    End Sub

    Public Sub CopyPrivate()

        CopyTable("AuditTrailAndDashboardLog")
        CopyTable("LibraryCRMCustomers")
        CopyTable("LibraryCRMTeamMembers")
        CopyTable("LibraryCRMTeamQuota")
        CopyTable("LibraryCRMTeams")
        CopyTable("LibraryDevice")
        CopyTable("LibraryDeviceFowst")
        CopyTable("LibraryDeviceFowstPrinter")
        CopyTable("LibraryDeviceInstalledApplicationsDetails")
        CopyTable("LibraryDeviceInvalidCertificate")
        CopyTable("LibraryDevicePolicyItem")
        CopyTable("LibraryDevicePolicyProfile")
        CopyTable("LibraryDevicePrinter")
        CopyTable("LibraryDevicePrinterCategory")
        CopyTable("LibraryDevicePrinterFowst")
        CopyTable("LibraryDeviceWhiteListApps")
        CopyTable("LibraryDiscountSettings")
        CopyTable("LibraryFowstBin")
        CopyTable("LibraryFowstBinProduct")
        CopyTable("LibraryFranchiseGroup")
        CopyTable("LibraryFranchiseGroupCutOff")
        CopyTable("LibraryFranchiseGroupCutOffDetails")
        CopyTable("LibraryFranchiseGroupCutOffSOGroupNo")
        CopyTable("LibraryFranchiseGroupHeads")
        CopyTable("LibraryFranchiseGroupSortingListNo")
        CopyTable("LibraryInvoiceTypes")
        CopyTable("LibraryLocationGeoFence")
        CopyTable("LibraryLocationGeoFencePoints")
        CopyTable("LibraryLocationTrack")
        CopyTable("LibraryOrganizationCustomer")
        CopyTable("LibraryOrganizationCustomerFowstBin")
        CopyTable("LibraryOrganizationCustomerFowstBinProduct")
        CopyTable("LibraryOrganizationCustomerProduct")
        CopyTable("LibraryOrganizationCustomerProductByFowst")
        CopyTable("LibraryOrganizationDashboardProductFeatures")
        CopyTable("LibraryOrganizationDecimalFeatures")
        CopyTable("LibraryOrganizationDecimalSetting")
        CopyTable("LibraryOrganizationEmployee")
        CopyTable("LibraryOrganizationEmployeeFOWST")
        CopyTable("LibraryOrganizationPIM")
        CopyTable("LibraryOrganizationPIMByFOWST")
        CopyTable("LibraryOrganizationPriceLevel")
        CopyTable("LibraryOrganizationSupplier")
        CopyTable("LibraryOrganizationWorkSchedule")
        CopyTable("LibraryOrganizationWorkScheduleEmployee")
        CopyTable("LibraryPimGroupFowsts")
        CopyTable("LibraryPimGroupPims")
        CopyTable("LibraryPimGroups")
        CopyTable("LibraryPimMenu")
        CopyTable("LibraryPriceLevels")
        CopyTable("LibraryReceiptSettings")
        CopyTable("LibraryReceiptSettingsFowst")
        CopyTable("LibraryReceiptSettingsMessage")
        CopyTable("LibraryRegisterForUpdates")
        CopyTable("LibrarySectionProduct")
        CopyTable("LibrarySections")
        CopyTable("LibrarySecurityModule")
        CopyTable("LibrarySecurityModuleMain")
        CopyTable("LibrarySecurityRole")
        CopyTable("LibrarySecurityRoleModules")
        CopyTable("LibrarySecurityRoleUser")
        'CopyTable("LibraryStockPriceLevelsODB")
        'CopyTable("LibraryTableViewLayout")
        'CopyTable("LibraryTableViewTemplate")
        CopyTable("SystemDefaults")
        'CopyTable("TablesLastUpdated")
        'CopyTable("TransBillingStatements")
        'CopyTable("TransBillingStatementsInvoices")
        'CopyTable("TransCustomerPayment")
        'CopyTable("TransCustomerPaymentCheckAmount")
        'CopyTable("TransCustomerPaymentChecks")
        'CopyTable("TransCustomerPaymentCreditCard")
        'CopyTable("TransCustomerPaymentDebitCard")
        'CopyTable("TransCustomerPaymentInvoices")
        'CopyTable("TransCustomerPaymentSRV")
        'CopyTable("TransFranchiseRedFlag")
        'CopyTable("TransImports")
        'CopyTable("TransInventoryLedger")
        'CopyTable("TransInventoryProductCount")
        'CopyTable("TransInventoryProductCountStock")
        'CopyTable("TransInvoice")
        'CopyTable("TransInvoiceDiscount")
        'CopyTable("TransInvoiceItems", New List(Of String) From {
        '    "ID",
        '    "InvoiceID",
        '    "_RefStockID",
        '    "ProductID",
        '    "OrderedQuantity",
        '    "Quantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "TermDiscount",
        '    "DiscountIfPaidWithIn",
        '    "NetDue",
        '    "UnitPrice",
        '    "SubTotal",
        '    "Cost",
        '    "PriceLevelID",
        '    "_IsPaid",
        '    "IsTermNumeric",
        '    "TermNumericValue",
        '    "_IsReplacement",
        '    "_IsReplacementOut",
        '    "_IsHasBackOrder",
        '    "_IsHasReplacement",
        '    "_IsReturned",
        '    "_PrePrintedInvoiceNo",
        '    "UnitGP",
        '    "UnitGPM",
        '    "LineGP",
        '    "LineGPM",
        '    "LineCost",
        '    "RequestedUnitPrice",
        '    "OriginalUnitPrice",
        '    "_Dimension",
        '    "LineNumber",
        '    "TaxPercent",
        '    "TaxAmount",
        '    "TaxSales",
        '    "DiscountAmount",
        '    "DiscountSales",
        '    "SubcriptionDuration",
        '    "SubscriptionUnitMeasure",
        '    "SubcriptionDiscount"
        '})
        'CopyTable("TransInvoiceItemSalesReps")
        'CopyTable("TransInvoiceItemSerials")
        'CopyTable("TransInvoiceSeniorCitizenPWDCustomer")
        'CopyTable("TransMemo")
        'CopyTable("TransMemoSalesReturn")
        'CopyTable("TransOfftake")
        'CopyTable("TransPIMAlerts")
        'CopyTable("TransReceiving")
        'CopyTable("TransReceivingStock")
        'CopyTable("TransSalesOrder")
        'CopyTable("TransSalesOrderItems", New List(Of String) From {
        '    "ID",
        '    "SalesOrderID",
        '    "StockID",
        '    "Quantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "TermDiscount",
        '    "DiscountIfPaidWithIn",
        '    "NetDue",
        '    "UnitPrice",
        '    "SubTotal",
        '    "OutputTax",
        '    "UnitPriceWOTax",
        '    "IsCancelled",
        '    "PriceLevelID",
        '    "CancellationPeriod",
        '    "Cost",
        '    "RequestedUnitPrice",
        '    "OriginalUnitPrice",
        '    "Dimension",
        '    "LineNumber",
        '    "QuotationNo",
        '    "SalesQuotationsID",
        '    "QuotationItemID"
        '})
        'CopyTable("TransSalesOrderStatus")
        'CopyTable("TransSalesQuotations")
        'CopyTable("TransSalesQuotationsItems")
        'CopyTable("TransSalesReturnLedger")
        'CopyTable("TransStockPickingList")
        'CopyTable("TransStockPickingListItems")
        'CopyTable("TransStockPickingListMeasurements", New List(Of String) From {
        '    "ID",
        '    "StockPickingListID",
        '    "MeasurementLevel",
        '    "StockID",
        '    "Quantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "BasedUnitMeasure",
        '    "IsWithdraw",
        '    "WarehousePersonnel",
        '    "Dimension"
        '})
        'CopyTable("TransStockSortingList")
        'CopyTable("TransStockSortingListItems", New List(Of String) From {
        '    "ID",
        '    "StockSortingListID",
        '    "SalesOrderID",
        '    "RefStockID",
        '    "StockID",
        '    "Quantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "TermDiscount",
        '    "DiscountIfPaidWithIn",
        '    "NetDue",
        '    "UnitPrice",
        '    "PriceLevelID",
        '    "IsInvoiced",
        '    "IsReplacement",
        '    "IsHasReplacement",
        '    "IsHasBackOrder",
        '    "Dimension",
        '    "LastUpdated"
        '})
        'CopyTable("TransStockWithdrawalItemsUnitMeasure", New List(Of String) From {
        '    "ID",
        '    "StockWithdrawalToWarehouseID",
        '    "PickingListMeasurementID",
        '    "StockID",
        '    "ServedQuantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "BasedUnitMeasure",
        '    "IsHold",
        '    "MeasurementLevel",
        '    "Dimension"
        '})
        'CopyTable("TransStockWithdrawalReplacement", New List(Of String) From {
        '    "ID",
        '    "StockWithdrawalToWarehouseID",
        '    "StockWithdrawalItemsID",
        '    "RefStockID",
        '    "StockID",
        '    "Quantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "BasedUnitMeasure",
        '    "MeasurementLevel",
        '    "Dimension"
        '})
        'CopyTable("TransStockWithdrawalToWarehouse")
        'CopyTable("TransStockWithdrawalToWarehouseItems", New List(Of String) From {
        '    "ID",
        '    "StockWithdrawalToWarehouseID",
        '    "StockWithdrawalToWarehouseItemNo",
        '    "PickingListMeasurementID",
        '    "StockID",
        '    "OrderedQuantity",
        '    "ServedQuantity",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "BasedUnitMeasure",
        '    "IsHold",
        '    "IsHasReplacement",
        '    "IsHasBackOrder",
        '    "IsHasRequestFromBranch",
        '    "BranchID",
        '    "QtyRequested",
        '    "QtyServed",
        '    "RequestedUnitMeasure",
        '    "MeasurementLevel",
        '    "Dimension"
        '})
        'CopyTable("TransSubscription")
        'CopyTable("TransSubscriptionCart", New List(Of String) From {
        '    "ID",
        '    "SubscriptionID",
        '    "ProductID",
        '    "Qty",
        '    "ActualQuantity",
        '    "UnitMeasure",
        '    "UnitPrice",
        '    "UnitDiscount",
        '    "PriceLevelID",
        '    "SubscriptionDuration",
        '    "SubscriptionUnit",
        '    "SubscriptionDiscount",
        '    "SubscriptionStatus",
        '    "ActivationDate"
        '})

    End Sub

    Private Sub CopyTable(vTableName As String, Optional vColumns As List(Of String) = Nothing)

        Dim pSql As String

        pSql = "SELECT * FROM " & EscapeSqlName(vTableName)
        CopySql(pSql, vTableName, vColumns)

    End Sub

    Public Sub CopySql(vSql As String, vDestinationTableName As String, Optional vColumns As List(Of String) = Nothing,
        Optional vProgress As Action(Of System.Data.SqlClient.SqlRowsCopiedEventArgs) = Nothing)

        If vColumns Is Nothing Then
            vColumns = New List(Of String)
            Dim pSql = "SELECT name " & vbCrLf &
                ",is_computed" & vbCrLf &
                " FROM sys.columns" & vbCrLf &
                " WHERE is_computed = 0 " & vbCrLf &
                " AND object_id = OBJECT_ID(" & ReadyString(vDestinationTableName) & ")"
            Dim pDT = SourceProData.Execute(pSql)
            For Each pDR As DataRow In pDT.Rows
                vColumns.Add(pDR("name"))
            Next
        End If

        Using pReader As SqlDataReader = SourceProData.ExecuteReader(vSql)
            Using pBulkCopy As SqlBulkCopy = New SqlBulkCopy(DestinationConnection)
                pBulkCopy.BulkCopyTimeout = Integer.MaxValue
                pBulkCopy.DestinationTableName = vDestinationTableName
                pBulkCopy.NotifyAfter = 1000
                pBulkCopy.BatchSize = 5000
                If vColumns IsNot Nothing Then
                    For Each pColumn As String In vColumns
                        pBulkCopy.ColumnMappings.Add(pColumn, pColumn)
                    Next pColumn
                End If
                AddHandler pBulkCopy.SqlRowsCopied,
                    Sub(sender As Object, e As System.Data.SqlClient.SqlRowsCopiedEventArgs)
                        If vProgress IsNot Nothing Then
                            vProgress(e)
                        End If
                        Debug.Print(e.RowsCopied & " rows copied to " & vDestinationTableName)
                    End Sub
                pBulkCopy.WriteToServer(pReader)
            End Using
        End Using

    End Sub

#If DEBUG Then
    Private Function CreateAllTablesCode() As String

        Dim pSql As String
        Dim pDT As DataTable
        Dim pDtColumns As DataTable
        Dim pCode As String

        pSql = "SELECT name FROM sys.tables" &
            " ORDER BY name"
        pDT = SourceProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            pCode &= "CopyTable(""" & pDR("name") & """"

            pSql = "SELECT name " & vbCrLf &
                ",is_computed" & vbCrLf &
                " FROM sys.columns" & vbCrLf & _
                " WHERE object_id = OBJECT_ID(" & ReadyString(pDR("name")) & ")"
            pDtColumns = SourceProData.Execute(pSql)
            pDtColumns.DefaultView.RowFilter = "is_computed = 1"
            If pDtColumns.DefaultView.Count > 0 Then
                pCode &= ", New List(Of String) From {" & vbCrLf
                pDtColumns.DefaultView.RowFilter = "is_computed = 0"
                For pRowViewIndex As Integer = 0 To pDtColumns.DefaultView.Count - 1
                    pCode &= vbTab & """" & Replace(pDtColumns.DefaultView(pRowViewIndex)("name"), """", """""") & """"
                    If pRowViewIndex < pDtColumns.DefaultView.Count - 1 Then
                        pCode &= ","
                    End If
                    pCode &= vbCrLf
                Next pRowViewIndex
                pCode &= "}"
            End If

            pCode &= ")" & vbCrLf
        Next pDR

        Return pCode

    End Function

    Public Shared Sub Test()

        Dim p As New BulkCopier

        'p.OrganizationID = "cfe39df6-8809-4973-9515-810df75aed12" ' mobizABC
        'p.SourceProData = ProData.CreateFromConnectionInfo("philhr", "mobiz", "sa", "ducmain", False, True)
        'p.DestinationConnection = New SqlConnection(ProData.CreateConnectionString("philhr\sql2", "mobiz2", "sa", "ducmain2", False))
        'p.DestinationConnection.Open()
        'p.CopyPublic()

        'p.SourceProData = ProData.CreateFromConnectionInfo("philhr", "MobizTemplate", "sa", "ducmain", False, True)
        'p.DestinationConnection = New SqlConnection(ProData.CreateConnectionString("philhr\sql2", "mobizABC2", "sa", "ducmain2", False))
        'p.DestinationConnection.Open()
        'p.CopyPrivate()

    End Sub
#End If
End Class
