Public Class Organization

    Public Property ID As String
    Public Property OrganizationID As String
    Public Property MaxReturnRow As Integer = 50

    Public Property CompanyName As String
    Public Property CompanyNameInitial As String

    Public Property OrgTradeNameList As Object()

    Public Property OwnershipType As String
    Public Property IsFranchisor As String
    Public Property IsMobizFranchisorRequestSent As String
    Public Property IsMobizConfirmedFranchisor As String
    Public Property RegistrationType As String
    Public Property RegistrationNo As String
    Public Property RegistrationDate As String

    Public Property CorporateTIN As String

    Public Property OfficeAddressBuildingNo As String
    Public Property OfficeAddressStreet As String
    Public Property OfficeAddressCityID As String
    Public Property OfficeAddressCountryID As String

    Public Property BusinessAddressBuildingNo As String
    Public Property BusinessAddressStreet As String
    Public Property BusinessAddressCityID As String
    Public Property BusinessAddressCountryID As String
    Public Property ReferenceNo As String

    Public Property IsOrganization As String
    Public Property PersonTitleID As String
    Public Property PersonSurname As String
    Public Property PersonFirstName As String
    Public Property PersonMiddleName As String
    Public Property PersonSuffixName As String
    Public Property PersonDOB As String
    Public Property PersonMotherMaidenSurname As String
    Public Property PersonMotherMaidenFirstName As String
    Public Property PersonMotherMaidenMiddleName As String

    Public Property Purpose As String
    Public Property BusinessScope As String

    Public Property NoOfRetailOutlet As Integer
    Public Property NoOfOffice As Integer
    Public Property NoOfWarehouse As Integer
    Public Property NoOfEmployees As Integer
    Public Property IsVAT As Boolean

    Public Property OrgCategoryList As Object()
    Public Property OrgIndustryClassificationList As Object()
    Public Property OrgFranchiseeList As Object()

    Public Property FiscalMonthEnd As String
    Public Property FiscalDayEnd As String
    Public Property NameOfExternalAuditor As String
    Public Property SingningPartnerOfExternalAuditor As String
    Public Property SECAccreditationNoOfExternalAuditor As String
    Public Property VATType As String
    Public Property BIRTaxGroup As String

    Public Property ContactCountryID As String
    Public Property ContactAreaCodeID As String
    Public Property ContactTelCoPrefixID As String
    Public Property ContactNo As String
    Public Property ContactExtensionNo As String

    Public Property ContactEmail As String
    Public Property ContactWebsite As String

    Public Property FOWSTTypeID As String
    Public Property FOWSTStatus As String
    Public Property FOWSTClassification As String
    Public Property FOWSTCode As String
    Public Property FOWSTName As String
    Public Property FOWSTDescription As String
    Public Property FOWSTBusinessTradeNameID As String
    Public Property FOWSTTINBranchCode As String
    Public Property FOWSTStreetNumber As String
    Public Property FOWSTStreet As String
    Public Property FOWSTCityID As String
    Public Property FOWSTRegion As String
    Public Property FOWSTCountryID As String
    Public Property FOWSTSubTypeID As String
    Public Property FOWSTStoreLocationTypeID As String
    Public Property FOWSTPlateNo As String
    Public Property FOWSTNoOfWheels As String
    Public Property FOWSTBodyColor As String
    Public Property FOWSTWeight As String
    Public Property FOWSTDimension As String
    Public Property FOWSTOwnershipType As String
    Public Property FOWSTContractStart As String
    Public Property FOWSTContractEnd As String
    Public Property FOWSTCostPerMonth As String
    Public Property EnforceZRead As String
    Public Property FOWSTIsMortgaged As String
    Public Property FOWSTIsFranchisee As String
    Public Property FOWSTFranchisorID As String
    Public Property DefaultPosView As String
    Public Property DefaultHotButtonTemplateID As String
    Public Property GPSLocation As String

    Public Property FOWSTIsInsideAMall As String
    Public Property Operates247 As String ' bool
    Public Property FOWSTMallName As String
    Public Property FtpServer As String
    Public Property FtpUsername As String
    Public Property FtpPassword As String

    Public Property FOWSTAyalaRecID As String
    Public Property FOWSTAyalaTenantCode As String
    Public Property FOWSTAyalaTenantName As String
    Public Property FOWSTAyalaTenantNameForDBF As String

    Public Property FOWSTAyalaMerchantsLeaseOutNumber As String
    Public Property FOWSTSMRecID As String
    Public Property FOWSTSMMallCode As String
    Public Property FOWSTSMMallTenantNo As String
    Public Property FOWSTSMMallClassCode As String
    Public Property FOWSTSMMallTradeCode As String
    Public Property FOWSTSMMallOutletNo As String
    Public Property FOWSTSMMallSaleType As String

    Public Property FOWSTMegaworldRecID As String
    Public Property FOWSTMegaworldTenantCode As String
    Public Property FOWSTMegaworldSaleType As String
    Public Property FOWSTMegaworldNoOfPOSTerminal As String

    Public Property RobinsonTenantId As String
    Public Property RobinsonPosTerminalNum As String

    Public Property PITXTenantId As String
    Public Property PITXPosTerminalID As String

    Public Property FOWSTIsIncludeReport As Integer
    Public Property FOWSTAccountID As String
    Public Property FOWSTStoreID As String
    Public Property FOWSTAccountID2 As String
    Public Property FOWSTStoreID2 As String

    Public Property FOWSTIsIncludeToShopperTrakReport As Integer
    Public Property FOWSTShopperTrakAccountID As String

    Public Property OrgFOWSTLTOList As Object()
    Public Property OrgFOWSTLTFRBList As Object()
    Public Property OrgFOWSTSectionList As Object()
    Public Property OrgFOWSTBPermitList As Object()
    Public Property OrgFOWSTTelList As Object()
    Public Property OrgFOWSTMobileList As Object()
    Public Property OrgFOWSTEmailList As Object()
    Public Property OrgFOWSTOperationHRList As Object()
    Public Property OrgFOWSTPrinterList As Object()

    Public Property LastName As String
    Public Property FirstName As String
    Public Property MiddleName As String
    Public Property Title As String
    Public Property GenerationalSuffix As String
    Public Property IsActive As String
    Public Property PrintStickers As String ' bool
    Public Property PrintQueueNumbers As String ' bool

    Public Property OrgRepContactTelList As Object()
    Public Property OrgRepContactMobileList As Object()
    Public Property OrgRepContactEmailList As Object()

    Public Property PeopleID As String
    Public Property SystemGeneratedID As String
    Public Property CompanyID As String
    Public Property ProductID As String
    Public Property CustomerID As String
    Public Property SupplierID As String
    Public Property ActivationDate As String
    Public Property DateHired As String
    Public Property IsDeactivated As String
    Public Property DeactivationDate As String
    Public Property DeactivationReason As String
    Public Property ProductAvailableToMobile As String
    Public Property Comments As String
    Public Property IsBan As String
    Public Property IsInventoryCountItem As Boolean
    Public Property IsPromoItem As Boolean
    Public Property IsVatExempt As String
    Public Property MaxAllowableDiscountableAmount As Decimal
    Public Property ItemBasedMaxAllowableDiscount As String
    Public Property DiscountedPriceAction As String
    Public Property IsConfirmed As String
    Public Property RoleTag As String
    Public Property CreditTermsID As String
    Public Property CreditLimit As String
    Public Property CreditBegBalance As String
    Public Property CreditBalanceAsOf As String
    Public ThirdPartyPlatform As String
    Public ThirdPartyID As String
    Public SupplierType As String
    Public VendorCode As String
    Public PaymentTermID As String

    Public Property IsDoNotShareDataToBrandOwner As String

    Public Property JobOccupationID As String
    Public Property BusinessUnit As String
    Public Property IsIncorporator As String
    Public Property IsDirector As String
    Public Property IsStockHolder As String
    Public Property IsOfficer As String
    Public Property IsExecutiveCommittee As String
    Public Property IsOwner As String

    Public Property IsHasHoldingCompany As String
    Public Property OrgHoldingCompanyID As String
    Public Property OrgAffiliatesID As String
    Public Property Remarks As String

    Public Property BrandName As String

    Public Property FowstID As String
    Public Property OrgFOWSTProductReorderPoint As String
    Public Property OrgFOWSTProductBinList As Object()

    Public Property BOMProductID As String
    Public Property Qty As Double
    Public Property UnitQty As Double
    Public Property UnitMeasureID As String
    Public Property ParentBOMProductID As String
    Public Property ComponentName As String
    Public Property ComponentMethod As String

    Public Property ProductDiscount As String
    Public Property ProductMeasurementContentQty As String
    Public Property ProductMeasurementContentUnit As String
    Public Property ProductMeasurementDimLenght As String
    Public Property ProductMeasurementDimLenghtUnit As String
    Public Property ProductMeasurementDimWidth As String
    Public Property ProductMeasurementDimWidthUnit As String
    Public Property ProductMeasurementDimHeight As String
    Public Property ProductMeasurementDimHeightUnit As String
    Public Property ProductMeasurementWeight As String
    Public Property ProductMeasurementWeightUnit As String
    Public Property OrgProductMeasurementBarcodeList As Object()
    Public Property IsTimeBaseUnit As Boolean
    Public Property TimeBaseUnitDescription As String

    Public Property PriceLevelID As String
    Public Property Price As String

    Public Property FilterName As String
    Public Property KeywordList As Object()

    Public Property PeopleEmailAddress As String
    Public Property PeopleFullName As String
    Public Property ConfirmKeyID As String
    Public Property MobizProduct As String

    Public Property FowstBinID As String
    Public Property EnableHotButtonsContextualMenu As Boolean

    Public Property SectionID As String

    Public Property PageStart As Integer
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property IsCountPage As Boolean
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String

    Public Property BarcodeID As String
    Public Property Barcode As String
    Public Property BarcodeType As String

    Public Property IsDirectHired As String
    Public Property IsSelfEmployed As String
    Public Property ManpowerAgencyID As String
    Public Property VendorID As String
    Public Property CoUserID As String

    Public Property ThirdPartySystemName As String
    Public Property ThirdPartySystemID As String
    Public Property PayFrequency As String

    Public Property DepartmentID As String
    Public Property TeamID As String

    Public Property WorkScheduleID As String
    Public Property PayTypeID As String

    Public Property Type As String

    Public Property BankID As String
    Public Property BankBranch As String
    Public Property AccountName As String
    Public Property AccountNo As String
    Public Property BankRole As String
    Public Property IsDefaultBank As Boolean

    Public Property ReceiptSettingsID As String
    Public Property Username As String
    Public Property Userpassword As String
    Public Property ApprovedResetMobizAccBy As String
    Public Property PosVendorID As String
    Public Property PriceLevels As Object
    Public Property RemoveAssignedEmp As Object

    Public Property ObjTelephone As Object
    Public Property ObjMobile As Object
    Public Property ObjEmail As Object

    Function CheckOrgUniqueOrgName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org "

            .SqlWhere = "WHERE org.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND org.CompanyName = " & ReadyString(CompanyName)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function CheckPersonUniqueName(ByVal IsFullCheckName As Boolean) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                End With
                With .Add("PersonSurname", SqlDbType.VarChar)
                    .Expression = "org.PersonSurname"
                End With
                With .Add("PersonFirstName", SqlDbType.VarChar)
                    .Expression = "org.PersonFirstName"
                End With
                With .Add("PersonMiddleName", SqlDbType.VarChar)
                    .Expression = "org.PersonMiddleName"
                End With
                With .Add("PersonDateOfBirth", SqlDbType.Date)
                    .Expression = "org.PersonDateOfBirth"
                End With
                With .Add("PersonMotherMaidenSurname", SqlDbType.VarChar)
                    .Expression = "org.PersonMotherMaidenSurname"
                End With
                With .Add("PersonMotherMaidenFirstName", SqlDbType.VarChar)
                    .Expression = "org.PersonMotherMaidenFirstName"
                End With
                With .Add("PersonMotherMaidenMiddleName", SqlDbType.VarChar)
                    .Expression = "org.PersonMotherMaidenMiddleName"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org "

            If (IsFullCheckName) Then
                .SqlWhere = "WHERE org.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                            " AND org.PersonSurname = " & ReadyString(PersonSurname) &
                            " AND org.PersonFirstName = " & ReadyString(PersonFirstName) &
                            " AND org.PersonMiddleName = " & ReadyString(PersonMiddleName) &
                            " AND org.PersonDateOfBirth " & IIf(PersonDOB.Trim().Length > 0 And PersonDOB.Trim() <> "//", " = " & ReadyString(PersonDOB), " IS NULL") &
                            " AND org.PersonMotherMaidenSurname = " & ReadyString(PersonMotherMaidenSurname) &
                            " AND org.PersonMotherMaidenFirstName = " & ReadyString(PersonMotherMaidenFirstName) &
                            " AND org.PersonMotherMaidenMiddleName = " & ReadyString(PersonMotherMaidenMiddleName)
            Else
                .SqlWhere = "WHERE ppol.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                            " AND org.PersonSurname = " & ReadyString(PersonSurname) &
                            " AND org.PersonFirstName = " & ReadyString(PersonFirstName) &
                            " AND org.PersonMiddleName = " & ReadyString(PersonMiddleName)
            End If

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getInitials(ByVal strValue As String) As String
        Dim strWords As String()
        Dim strInitials As String
        Dim intCntr As Integer

        strInitials = ""

        Try

            strWords = strValue.Trim().Split(" ")

            intCntr = 0
            Do Until (intCntr >= strWords.Length)

                strInitials = strInitials + strWords(intCntr).Trim().Substring(0, 1)

                intCntr = intCntr + 1
            Loop

        Catch ex As Exception

        End Try

        Return strInitials.ToUpper()

    End Function

    Function getOrganizationList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("IsOrganization", SqlDbType.Bit)
                    .Expression = "org.IsOrganization"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("PersonDateOfBirth", SqlDbType.Date)
                    .Expression = "org.PersonDateOfBirth"
                End With
                With .Add("MotherMaidenName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMotherMaidenSurname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMotherMaidenFirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMotherMaidenMiddleName)) + ' ',' '), '') "
                End With
                With .Add("OwnershipType", SqlDbType.VarChar)
                    .Expression = "org.OwnershipType"
                End With
                With .Add("RegistrationType", SqlDbType.VarChar)
                    .Expression = "org.RegistrationType"
                End With
                With .Add("RegistrationNo", SqlDbType.VarChar)
                    .Expression = "org.RegistrationNo"
                End With
                With .Add("OfficeAddressBuildingNo", SqlDbType.VarChar)
                    .Expression = "org.OfficeAddressBuildingNo"
                End With
                With .Add("OfficeAddressStreet", SqlDbType.VarChar)
                    .Expression = "org.OfficeAddressStreet"
                End With
                With .Add("OfficeAddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OfficeAddressCityID"
                    .Searchable = False
                End With
                With .Add("OfficeAddressCity", SqlDbType.VarChar)
                    .Expression = "orgbofccty.[Name]"
                End With
                With .Add("OfficeAddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OfficeAddressCountryID"
                    .Searchable = False
                End With
                With .Add("OfficeAddressCountry", SqlDbType.VarChar)
                    .Expression = "orgofcctry.[Name]"
                End With
                With .Add("BusinessAddressBuildingNo", SqlDbType.VarChar)
                    .Expression = "org.BusinessAddressBuildingNo"
                End With
                With .Add("BusinessAddressStreet", SqlDbType.VarChar)
                    .Expression = "org.BusinessAddressStreet"
                End With
                With .Add("BusinessAddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.BusinessAddressCityID"
                    .Searchable = False
                End With
                With .Add("BusinessAddressCity", SqlDbType.VarChar)
                    .Expression = "orgbbuscty.[Name]"
                End With
                With .Add("BusinessAddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.BusinessAddressCountryID"
                    .Searchable = False
                End With
                With .Add("BusinessAddressCountry", SqlDbType.VarChar)
                    .Expression = "orgbusctry.[Name]"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org " &
                    " LEFT JOIN dbo.MobizLibraryCities orgbofccty ON (org.OfficeAddressCityID = orgbofccty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries orgofcctry ON (org.OfficeAddressCountryID = orgofcctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities orgbbuscty ON (org.BusinessAddressCityID = orgbbuscty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries orgbusctry ON (org.BusinessAddressCountryID = orgbusctry.ID) " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)"

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY (CASE org.IsOrganization " &
                        "     WHEN 1 THEN  org.CompanyName " &
                        "     ELSE  " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                        " END) ASC"
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgTradeNameList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgtrade.ID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "orgtrade.BusinessTradeName"
                End With
                With .Add("BusinessTradeNameInitial", SqlDbType.VarChar)
                    .Expression = "orgtrade.BusinessTradeNameInitial"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationTradeName orgtrade "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgtrade.OrganizationID IS NULL", " WHERE orgtrade.OrganizationID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgCategoryList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcats.ID"
                    .Searchable = False
                End With
                With .Add("CategoryTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcats.CategoryTypeID"
                    .Searchable = False
                End With
                With .Add("CategoryType", SqlDbType.VarChar)
                    .Expression = "orgcattype.CategoryType"
                End With
                With .Add("CategoryID", SqlDbType.VarChar)
                    .Expression = "orgcattype.CategoryID"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "orgcat.Category"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationCategory orgcats " &
                    " INNER JOIN dbo.MobizLibraryOrgType orgcattype ON (orgcats.CategoryTypeID = orgcattype.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrgCategory orgcat ON (orgcattype.CategoryID = orgcat.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcats.OrganizationID IS NULL", " WHERE orgcats.OrganizationID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgIndustryClassificationList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgindustryclass.ID"
                    .Searchable = False
                End With
                With .Add("IndustryClassID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgindustryclass.IndustryClassID"
                End With
                With .Add("IndustryClass", SqlDbType.VarChar)
                    .Expression = "industryclass.Class"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationIndustryClass orgindustryclass " &
                    " INNER JOIN dbo.MobizLibraryIndustryClass industryclass ON (orgindustryclass.IndustryClassID = industryclass.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgindustryclass.OrganizationID IS NULL", " WHERE orgindustryclass.OrganizationID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgFranchiseeList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfr.OrganizationID"
                End With
                With .Add("FranchiseeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfr.FranchiseeID"
                End With
                With .Add("FranchiseeOrgName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("BranchName", SqlDbType.VarChar)
                    .Expression = "orgfrfowst.[Name]"
                End With
                With .Add("IsFranchisorConfirmed", SqlDbType.Bit)
                    .Expression = "orgfr.IsFranchisorConfirmed"
                End With
                With .Add("IsFranchiseeConfirmed", SqlDbType.Bit)
                    .Expression = "orgfr.IsFranchiseeConfirmed"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFranchisee orgfr " &
                    " INNER JOIN MobizLibraryOrganizationFOWST orgfrfowst ON (orgfr.FranchiseeID = orgfrfowst.ID) " &
                    " INNER JOIN MobizLibraryOrganization org ON (orgfrfowst.OrganizationID = org.ID) " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfr.OrganizationID IS NULL", " WHERE orgfr.OrganizationID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrganizationInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        WebRequest.Instance.Business.LibraryOrganization.RestoreOrganizationTin()
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                End With
                With .Add("IsOrganization", SqlDbType.Bit)
                    .Expression = "org.IsOrganization"
                End With
                With .Add("PersonTitleID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.PersonTitleID"
                End With
                With .Add("PersonTitle", SqlDbType.VarChar)
                    .Expression = "orgpresuffix.Code"
                End With
                With .Add("PersonSurname", SqlDbType.VarChar)
                    .Expression = "org.PersonSurname"
                End With
                With .Add("PersonFirstName", SqlDbType.VarChar)
                    .Expression = "org.PersonFirstName"
                End With
                With .Add("PersonMiddleName", SqlDbType.VarChar)
                    .Expression = "org.PersonMiddleName"
                End With
                With .Add("PersonSuffixName", SqlDbType.VarChar)
                    .Expression = "org.PersonSuffixName"
                End With
                With .Add("PersonName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgpresuffix.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') "
                End With
                With .Add("PersonDateOfBirthMonth", SqlDbType.Int)
                    .Expression = "MONTH(org.PersonDateOfBirth)"
                End With
                With .Add("PersonDateOfBirthDay", SqlDbType.Int)
                    .Expression = "DAY(org.PersonDateOfBirth)"
                End With
                With .Add("PersonDateOfBirthYear", SqlDbType.Int)
                    .Expression = "YEAR(org.PersonDateOfBirth)"
                End With
                With .Add("PersonMotherMaidenSurname", SqlDbType.VarChar)
                    .Expression = "org.PersonMotherMaidenSurname"
                End With
                With .Add("PersonMotherMaidenFirstName", SqlDbType.VarChar)
                    .Expression = "org.PersonMotherMaidenFirstName"
                End With
                With .Add("PersonMotherMaidenMiddleName", SqlDbType.VarChar)
                    .Expression = "org.PersonMotherMaidenMiddleName"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("CompanyNameInitial", SqlDbType.VarChar)
                    .Expression = "org.CompanyNameInitial"
                End With
                With .Add("OwnershipType", SqlDbType.VarChar)
                    .Expression = "org.OwnershipType"
                End With
                With .Add("IsFranchisor", SqlDbType.Bit)
                    .Expression = "org.IsFranchisor"
                End With
                With .Add("RegistrationType", SqlDbType.VarChar)
                    .Expression = "org.RegistrationType"
                End With
                With .Add("RegistrationNo", SqlDbType.VarChar)
                    .Expression = "org.RegistrationNo"
                End With
                With .Add("RegistrationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(org.RegistrationDate)"
                End With
                With .Add("RegistrationDateDay", SqlDbType.Int)
                    .Expression = "DAY(org.RegistrationDate)"
                End With
                With .Add("RegistrationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(org.RegistrationDate)"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "org.TinNumber"
                End With
                With .Add("TinNumberBackup", SqlDbType.VarChar)
                    .Expression = "org.TinNumberBackup"
                End With
                With .Add("OfficeAddressBuildingNo", SqlDbType.VarChar)
                    .Expression = "org.OfficeAddressBuildingNo"
                End With
                With .Add("OfficeAddressStreet", SqlDbType.VarChar)
                    .Expression = "org.OfficeAddressStreet"
                End With
                With .Add("OfficeAddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OfficeAddressCityID"
                End With
                With .Add("OfficeAddressCity", SqlDbType.VarChar)
                    .Expression = "orgbofccty.[Name]"
                End With
                With .Add("OfficeAddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OfficeAddressCountryID"
                End With
                With .Add("OfficeAddressCountry", SqlDbType.VarChar)
                    .Expression = "orgofcctry.[Name]"
                End With
                With .Add("BusinessAddressBuildingNo", SqlDbType.VarChar)
                    .Expression = "org.BusinessAddressBuildingNo"
                End With
                With .Add("BusinessAddressStreet", SqlDbType.VarChar)
                    .Expression = "org.BusinessAddressStreet"
                End With
                With .Add("BusinessAddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.BusinessAddressCityID"
                End With
                With .Add("BusinessAddressCity", SqlDbType.VarChar)
                    .Expression = "orgbbuscty.[Name]"
                End With
                With .Add("BusinessAddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.BusinessAddressCountryID"
                End With
                With .Add("BusinessAddressCountry", SqlDbType.VarChar)
                    .Expression = "orgbusctry.[Name]"
                End With
                With .Add("Purpose", SqlDbType.VarChar)
                    .Expression = "org.Purpose"
                End With
                With .Add("BusinessScope", SqlDbType.VarChar)
                    .Expression = "org.BusinessScope"
                End With
                With .Add("NoOfRetailOutlet", SqlDbType.Int)
                    .Expression = "org.NoOfRetailOutlet"
                End With
                With .Add("NoOfOffice", SqlDbType.Int)
                    .Expression = "org.NoOfOffice"
                End With
                With .Add("NoOfWarehouse", SqlDbType.Int)
                    .Expression = "org.NoOfWarehouse"
                End With
                With .Add("NoOfEmployees", SqlDbType.Int)
                    .Expression = "org.NoOfEmployees"
                End With
                With .Add("IsMobizFranchisorRequestSent", SqlDbType.Bit)
                    .Expression = "org.IsMobizFranchisorRequestSent"
                End With
                With .Add("IsMobizConfirmedFranchisor", SqlDbType.Bit)
                    .Expression = "org.IsMobizConfirmedFranchisor"
                End With
                With .Add("LogoFileName", SqlDbType.VarChar)
                    .Expression = "org.LogoFileName"
                End With
                With .Add("IsVAT", SqlDbType.VarChar)
                    .Expression = "org.IsVAT"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganization org " &
                    " LEFT JOIN dbo.MobizLibraryCities orgbofccty ON (org.OfficeAddressCityID = orgbofccty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries orgofcctry ON (org.OfficeAddressCountryID = orgofcctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities orgbbuscty ON (org.BusinessAddressCityID = orgbbuscty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries orgbusctry ON (org.BusinessAddressCountryID = orgbusctry.ID) " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgpresuffix ON (org.PersonTitleID = orgpresuffix.ID)"


            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE org.ID IS NULL", " WHERE org.ID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetPeopleEmailAddress() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemail.ID"
                End With
                With .Add("EmailAddress", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemail.EmailAddress"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactEmail ppolemail "

            .SqlWhere = "WHERE ppolemail.PeopleID = " & ReadyString(PeopleID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SaveEmpEmailAddress() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleContactEmail")
        Dim pSql As String
        Dim strIDReturn As String

        Try
            p("PeopleID") = ReadyString(PeopleID)
            p("EmailAddress") = ReadyString(ContactEmail)
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()
            Return "Saved"
        Catch ex As Exception
            Return "Error"
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function

    Function getPeopleOrgID() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.ID"
                End With
                With .Add("IsConfirmed", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.IsConfirmed"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleOrganization ppolorg "

            .SqlWhere = "WHERE ppolorg.PeopleID = " & ReadyString(PeopleID) &
                        " AND ppolorg.IsConfirmed = 0" &
                        " AND ppolorg.IsTerminated = 0"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SendConfirmationViaEmail() As String
        Dim strBody As String
        Dim mp As String

        Try
            If MobizProduct = "" Then
                mp = "mcr" ' mobiz complete retail
            Else
                mp = "mc" ' mobiz construct
            End If

            strBody = "<p>Hello " & PeopleFullName & "! </p>" &
                        "<p>" &
                            "Mobiz needs to confirm that you are currently employed in <strong>" & CompanyName & "</strong> by clicking on the link below:<br>" &
                            "<br><a href='122.52.123.251/ConfirmEmployment?key=" & ConfirmKeyID & "&id=" & PeopleID & "&mp=" & mp & "' target='_blank'>Click here to confirm</a>" &
                        "</p>" &
                        "<p>" &
                            "You will be able to use Mobiz business applications associated with <strong>" & CompanyName & "</strong> once you confirm your employment. <br> <br> Thanks." &
                        "</p>" &
                        "<p><br>" &
                            "<br> All the Best,<br> <br> Mobiz Team" &
                        "</p>"


            SendMail("customercare@mobiz-apps.com",
                 PeopleEmailAddress,
                 "Mobiz",
                 "Employment Confirmation",
                 strBody,
                 "smtp.live.com",
                 587,
                 "customercare@mobiz-apps.com",
                 "MobizCustCare2221", , , , , True)

            Return "Sent"

        Catch ex As Exception
            Return "An error occured while sending email: " & ex.Message
        End Try

    End Function

    Function SaveOrganizationInfo() As String
        Dim p As New InsertUpdate("MobizLibraryOrganization")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        Try

            CompanyNameInitial = IIf(CompanyNameInitial.Trim().Length > 0, CompanyNameInitial.Trim(), getInitials(CompanyName))

            p("IsOrganization") = IsOrganization

            p("PersonTitleID") = IIf(PersonTitleID.Trim().Length > 0, ReadyString(PersonTitleID), "NULL")
            p("PersonSurname") = ReadyString(PersonSurname)
            p("PersonFirstName") = ReadyString(PersonFirstName)
            p("PersonMiddleName") = ReadyString(PersonMiddleName)
            p("PersonSuffixName") = ReadyString(PersonSuffixName)
            p("PersonDateOfBirth") = IIf(PersonDOB.Trim().Length > 0 And PersonDOB.Trim() <> "//", ReadyString(PersonDOB), "NULL")
            p("PersonMotherMaidenSurname") = ReadyString(PersonMotherMaidenSurname)
            p("PersonMotherMaidenFirstName") = ReadyString(PersonMotherMaidenFirstName)
            p("PersonMotherMaidenMiddleName") = ReadyString(PersonMotherMaidenMiddleName)

            p("CompanyName") = ReadyString(CompanyName)
            p("CompanyNameInitial") = ReadyString(CompanyNameInitial)
            p("OwnershipType") = ReadyString(OwnershipType)

            'p("IsFranchisor") = IsFranchisor
            'additional by janine oct 16
            p("IsMobizFranchisorRequestSent") = IsFranchisor

            p("RegistrationType") = ReadyString(RegistrationType)
            p("RegistrationNo") = ReadyString(RegistrationNo)
            p("RegistrationDate") = IIf(RegistrationDate.Trim().Length > 0, ReadyString(RegistrationDate), "NULL")
            Dim pError As String
            If ValidateTin(CorporateTIN, pError) = False Then
                Throw New ApplicationException(pError)
            End If
            p("TinNumber") = ReadyString(CorporateTIN)
            p("TinNumberBackup") = ReadyString(CorporateTIN)

            p("OfficeAddressBuildingNo") = ReadyString(OfficeAddressBuildingNo)
            p("OfficeAddressStreet") = ReadyString(OfficeAddressStreet)
            p("OfficeAddressCityID") = IIf(OfficeAddressCityID.Trim().Length > 0, ReadyString(OfficeAddressCityID), "NULL")
            p("OfficeAddressCountryID") = IIf(OfficeAddressCountryID.Trim().Length > 0, ReadyString(OfficeAddressCountryID), "NULL")

            p("BusinessAddressBuildingNo") = ReadyString(BusinessAddressBuildingNo)
            p("BusinessAddressStreet") = ReadyString(BusinessAddressStreet)
            p("BusinessAddressCityID") = IIf(BusinessAddressCityID.Trim().Length > 0, ReadyString(BusinessAddressCityID), "NULL")
            p("BusinessAddressCountryID") = IIf(BusinessAddressCountryID.Trim().Length > 0, ReadyString(BusinessAddressCountryID), "NULL")

            p("Purpose") = ReadyString(Purpose)
            p("BusinessScope") = ReadyString(BusinessScope)
            p("NoOfRetailOutlet") = NoOfRetailOutlet
            p("NoOfOffice") = NoOfOffice
            p("NoOfWarehouse") = NoOfWarehouse
            p("NoOfEmployees") = NoOfEmployees
            p("IsVAT") = CInt2(IsVAT)

            If String.IsNullOrWhiteSpace(ReferenceNo) = False Then
                p("ReferenceNo") = ReadyString(ReferenceNo)
            End If

            If (WebSession.Instance().PeopleUserID Is Nothing) Then '''Value is nothing when called from winhost
                p("UserID") = "'D4FE8FC8-06B5-4C61-8882-CDD9E1DD2BA4'"
            Else
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            End If

            p("LastUpdated") = "getdate()"

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else

                '''Check if ID exist then update otherwise insert; used in subscription wherein organization has ID already from Winhost
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData
                    .MaxRecords = MaxReturnRow

                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "org.ID"
                        End With
                    End With

                    .SqlFrom = "FROM MobizLibraryOrganization org "

                    .SqlWhere = IIf(ID.Trim().Length = 0, "WHERE org.ID IS NULL", " WHERE org.ID = " + ReadyString(ID))

                    pDT = .Requery
                End With

                strIDReturn = ID
                If (pDT.Rows.Count > 0) Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(ID))
                Else
                    p("ID") = ReadyString(strIDReturn)
                    pSql = p.GetInsert()
                End If
            End If

            Common.WebRequest.ProData.BeginTransaction()

            'pSql = "Update LibraryOrganization Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If (ID.Trim().Length <= 0 Or pDT.Rows.Count = 0) Then

                ' '''Save Default Organization FOWST

                p = New InsertUpdate("MobizLibraryOrganizationFOWST")

                p("Name") = ReadyString(CompanyName)
                p("OrganizationID") = ReadyString(strIDReturn)
                p("StreetNumber") = ReadyString(BusinessAddressBuildingNo)
                p("Street") = ReadyString(BusinessAddressStreet)

                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            '''Save Trade Name List
            If (Not OrgTradeNameList Is Nothing) Then
                For Each objOrgTradeName As Object In OrgTradeNameList

                    If (objOrgTradeName("RecState").ToString().Trim().Length > 0) Then

                        If (objOrgTradeName("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("dbo.MobizLibraryOrganizationTradeName")

                            p("OrganizationID") = ReadyString(strIDReturn)
                            p("BusinessTradeName") = IIf(objOrgTradeName("BusinessTradeName").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgTradeName("BusinessTradeName").ToString().Trim()))
                            p("BusinessTradeNameInitial") = ReadyString(IIf(objOrgTradeName("BusinessTradeNameInitial").ToString().Trim().Length > 0, objOrgTradeName("BusinessTradeNameInitial").ToString().Trim(), getInitials(objOrgTradeName("BusinessTradeName").ToString().Trim())))

                            If (objOrgTradeName("ID").ToString().Trim().Length <= 0) Then
                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()
                            Else
                                '''Check if ID exist then update otherwise insert; used in subscription wherein organization has ID already from Winhost
                                pDT = New DataTable()
                                pFinder = New Finder
                                With pFinder
                                    .ProData = Common.WebRequest.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "ID"
                                        End With
                                    End With

                                    .SqlFrom = "FROM MobizLibraryOrganizationTradeName "

                                    .SqlWhere = IIf(ID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " + ReadyString(objOrgTradeName("ID").ToString()))

                                    pDT = .Requery
                                End With

                                strIDRec = objOrgTradeName("ID").ToString()
                                If (pDT.Rows.Count > 0) Then
                                    pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                                Else
                                    p("ID") = ReadyString(strIDRec)
                                    pSql = p.GetInsert()
                                End If


                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.MobizLibraryOrganizationTradeName " &
                                   " WHERE ID " & IIf(objOrgTradeName("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgTradeName("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objOrgTradeName
            End If

            '''Save Category List
            If (Not OrgCategoryList Is Nothing) Then
                For Each objOrgCategory As Object In OrgCategoryList

                    If (objOrgCategory("RecState").ToString().Trim().Length > 0) Then

                        If (objOrgCategory("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("MobizLibraryOrganizationCategory")

                            p("OrganizationID") = ReadyString(strIDReturn)
                            p("CategoryTypeID") = IIf(objOrgCategory("CategoryTypeID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgCategory("CategoryTypeID").ToString().Trim()))

                            If (objOrgCategory("ID").ToString().Trim().Length <= 0) Then
                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()
                            Else
                                strIDRec = objOrgCategory("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.MobizLibraryOrganizationCategory " &
                                   " WHERE ID " & IIf(objOrgCategory("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgCategory("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objOrgCategory
            End If

            '''Save Industry Classification List
            If (Not OrgIndustryClassificationList Is Nothing) Then
                For Each objOrgIndustryClass As Object In OrgIndustryClassificationList

                    If (objOrgIndustryClass("RecState").ToString().Trim().Length > 0) Then

                        If (objOrgIndustryClass("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("MobizLibraryOrganizationIndustryClass")

                            p("OrganizationID") = ReadyString(strIDReturn)
                            p("IndustryClassID") = IIf(objOrgIndustryClass("IndustryClassID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgIndustryClass("IndustryClassID").ToString().Trim()))

                            If (objOrgIndustryClass("ID").ToString().Trim().Length <= 0) Then
                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()
                            Else
                                strIDRec = objOrgIndustryClass("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.MobizLibraryOrganizationIndustryClass " &
                                   " WHERE ID " & IIf(objOrgIndustryClass("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgIndustryClass("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objOrgIndustryClass
            End If

            If Common.WebRequest.ProData.GetCurrentDatabase.ToLower() <> "mobiz" Then
                'Wala ni labot pag public db ang current db na gigamit, by Arnie 07-24-2019
                'reason kay mag-error diri pag magcreate ug new database
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            ' '''Save Franchisee List
            'If (Not OrgFranchiseeList Is Nothing) Then
            '    For Each objOrgFranchisee As Object In OrgFranchiseeList

            '        If (objOrgFranchisee("RecState").ToString().Trim().Length > 0) Then

            '            If (objOrgFranchisee("RecState").ToString().Trim() <> "Remove") Then
            '                p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

            '                p("OrganizationID") = ReadyString(strIDReturn)
            '                p("FranchiseeID") = IIf(objOrgFranchisee("FranchiseeID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgFranchisee("FranchiseeID").ToString().Trim()))
            '                p("IsFranchisorConfirmed") = 1
            '                p("IsFranchiseeConfirmed") = 0

            '                If (objOrgFranchisee("ID").ToString().Trim().Length <= 0) Then
            '                    strIDRec = Guid.NewGuid.ToString()
            '                    p("ID") = ReadyString(strIDRec)
            '                    pSql = p.GetInsert()
            '                Else
            '                    strIDRec = objOrgFranchisee("ID").ToString()
            '                    pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
            '                End If

            '                Common.WebRequest.ProData.ExecuteSQL(pSql)

            '            Else

            '                pSql = "DELETE " &
            '                       " FROM dbo.MobizLibraryOrganizationFranchisee " &
            '                       " WHERE ID " & IIf(objOrgFranchisee("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgFranchisee("ID").ToString().Trim()))

            '                Common.WebRequest.ProData.ExecuteSQL(pSql)

            '            End If

            '        End If

            '    Next objOrgFranchisee
            'End If

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction(, True)
            Throw
        End Try

        Return strIDReturn

    End Function

    Public Shared Function ValidateTin(vText As String, ByRef oError As String) As Boolean

        vText = CStr2(vText)
        If vText.Length < 9 _
            OrElse vText.Length > 30 Then

            oError = "TIN must be 9 to 30 characters in length. Allowed characters are numbers and dash (-)."
            Return False
        End If

        If Regex.IsMatch(vText, "^[0-9-]+$") = False Then
            oError = "TIN has invalid data. Allowed characters are numbers and dash (-)."
            Return False
        End If

        If vText.Contains("--") Then
            oError = "TIN has invalid data. Consecutive dashes (--) are not allowed"
            Return False
        End If

        Return True

    End Function

    Function getOrgFinancialReportInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfin.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfin.OrganizationID"
                    .Searchable = False
                End With
                With .Add("FiscalMonthEnd", SqlDbType.Int)
                    .Expression = "orgfin.FiscalMonthEnd"
                End With
                With .Add("FiscalDayEnd", SqlDbType.Int)
                    .Expression = "orgfin.FiscalDayEnd"
                End With
                With .Add("NameOfExternalAuditor", SqlDbType.VarChar)
                    .Expression = "orgfin.NameOfExternalAuditor"
                End With
                With .Add("SingningPartnerOfExternalAuditor", SqlDbType.VarChar)
                    .Expression = "orgfin.SingningPartnerOfExternalAuditor"
                End With
                With .Add("SECAccreditationNoOfExternalAuditor", SqlDbType.VarChar)
                    .Expression = "orgfin.SECAccreditationNoOfExternalAuditor"
                End With
                With .Add("VATType", SqlDbType.VarChar)
                    .Expression = "orgfin.VATType"
                End With
                With .Add("BIRTaxGroup", SqlDbType.VarChar)
                    .Expression = "orgfin.BIRTaxGroup"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFinancialReportInfo orgfin "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfin.OrganizationID IS NULL", " WHERE orgfin.OrganizationID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgFinancialReport() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFinancialReportInfo")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("FiscalMonthEnd") = FiscalMonthEnd
        p("FiscalDayEnd") = FiscalDayEnd
        p("NameOfExternalAuditor") = ReadyString(NameOfExternalAuditor)
        p("SingningPartnerOfExternalAuditor") = ReadyString(SingningPartnerOfExternalAuditor)
        p("SECAccreditationNoOfExternalAuditor") = ReadyString(SECAccreditationNoOfExternalAuditor)
        p("VATType") = ReadyString(VATType)
        p("BIRTaxGroup") = ReadyString(BIRTaxGroup)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function getOrgContactTelList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "orgcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ExtensionNo"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactTel orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrganizationID IS NULL", " WHERE orgcon.OrganizationID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgContactTelInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "orgcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ExtensionNo"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactTel orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.ID IS NULL", " WHERE orgcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgContactTel() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationContactTel")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("CountryID") = ReadyString(ContactCountryID)
        p("AreaCodeID") = ReadyString(ContactAreaCodeID)
        p("ContactNo") = ReadyString(ContactNo)
        p("ExtensionNo") = ReadyString(ContactExtensionNo)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemoveOrgContactTel() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationContactTel " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getOrgContactFaxList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "orgcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ExtensionNo"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactFax orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrganizationID IS NULL", " WHERE orgcon.OrganizationID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgContactFaxInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "orgcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ExtensionNo"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactFax orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.ID IS NULL", " WHERE orgcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgContactFax() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationContactFax")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("CountryID") = ReadyString(ContactCountryID)
        p("AreaCodeID") = ReadyString(ContactAreaCodeID)
        p("ContactNo") = ReadyString(ContactNo)
        p("ExtensionNo") = ReadyString(ContactExtensionNo)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemoveOrgContactFax() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationContactFax " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getOrgContactMobileList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("TelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "orgcon.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactMobile orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (orgcon.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrganizationID IS NULL", " WHERE orgcon.OrganizationID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgContactMobileInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("TelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "orgcon.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactMobile orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (orgcon.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.ID IS NULL", " WHERE orgcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgContactMobile() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationContactMobile")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("CountryID") = ReadyString(ContactCountryID)
        p("TelCoPrefixID") = ReadyString(ContactTelCoPrefixID)
        p("ContactNo") = ReadyString(ContactNo)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemoveOrgContactMobile() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationContactMobile " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getOrgContactEmailList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "orgcon.EmailAddress"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactEmail orgcon"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrganizationID IS NULL", " WHERE orgcon.OrganizationID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgContactEmailInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "orgcon.EmailAddress"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactEmail orgcon"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.ID IS NULL", " WHERE orgcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgContactEmail() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationContactEmail")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("EmailAddress") = ReadyString(ContactEmail)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemoveOrgContactEmail() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationContactEmail " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getOrgContactWebsiteList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("Website", SqlDbType.VarChar)
                    .Expression = "orgcon.Website"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactWebsite orgcon"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrganizationID IS NULL", " WHERE orgcon.OrganizationID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgContactWebsiteInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrganizationID"
                    .Searchable = False
                End With
                With .Add("Website", SqlDbType.VarChar)
                    .Expression = "orgcon.Website"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationContactWebsite orgcon"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.ID IS NULL", " WHERE orgcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgContactWebsite() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationContactWebsite")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("Website") = ReadyString(ContactWebsite)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemoveOrgContactWebsite() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationContactWebsite " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getOrgFOWSTList(ByVal strID As String, ByVal SearchKeyword As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            '.MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FOWSTTypeID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "orgadd.Code"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "orgadd.Name"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "orgadd.Description"
                End With
                With .Add("BusinessTradeNameID", SqlDbType.VarChar)
                    .Expression = "orgadd.BusinessTradeNameID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "custtrade.BusinessTradeName"
                End With
                With .Add("StreetNumber", SqlDbType.VarChar)
                    .Expression = "orgadd.StreetNumber"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "orgadd.Street"
                End With
                With .Add("FowstCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FowstCityID"
                    .Searchable = False
                End With
                With .Add("FowstCity", SqlDbType.VarChar)
                    .Expression = "cty.Name"
                    .Searchable = False
                End With
                With .Add("FowstCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FowstCountryID"
                    .Searchable = False
                End With
                With .Add("FowstCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                    .Searchable = False
                End With
                With .Add("CompleteFowstAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgadd.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgadd.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                End With
                With .Add("SubFOWSTTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.SubFOWSTTypeID"
                    .Searchable = False
                End With
                With .Add("RetailType", SqlDbType.VarChar)
                    .Expression = "rtype.Name"
                End With
                With .Add("WarehouseType", SqlDbType.VarChar)
                    .Expression = "wtype.Name"
                End With
                With .Add("PlateNo", SqlDbType.VarChar)
                    .Expression = "orgadd.PlateNo"
                End With
                With .Add("NoOfWheels", SqlDbType.VarChar)
                    .Expression = "orgadd.NoOfWheels"
                End With
                With .Add("BodyColor", SqlDbType.VarChar)
                    .Expression = "orgadd.BodyColor"
                End With
                With .Add("Weight", SqlDbType.Float)
                    .Expression = "orgadd.Weight"
                End With
                With .Add("Dimension", SqlDbType.VarChar)
                    .Expression = "orgadd.Dimension"
                End With
                With .Add("OwnershipType", SqlDbType.Int)
                    .Expression = "orgadd.OwnershipType"
                End With
                With .Add("ContractStart", SqlDbType.Date)
                    .Expression = "orgadd.ContractStart"
                End With
                With .Add("ContractEnd", SqlDbType.Date)
                    .Expression = "orgadd.ContractEnd"
                End With
                With .Add("CostPerMonth", SqlDbType.Decimal)
                    .Expression = "orgadd.CostPerMonth"
                End With
                With .Add("IsMortgaged", SqlDbType.Bit)
                    .Expression = "orgadd.IsMortgaged"
                End With
                With .Add("IsFranchisee", SqlDbType.Bit)
                    .Expression = "orgadd.IsFranchisee"
                End With
                With .Add("FranchisorID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FranchisorID"
                    .Searchable = False
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "orgadd.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST orgadd " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationTradeName custtrade ON (orgadd.BusinessTradeNameID = custtrade.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCities cty ON (orgadd.FowstCityID = cty.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgadd.FowstCountryID = ctry.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTType rtype ON (orgadd.SubFOWSTTypeID = rtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgadd.SubFOWSTTypeID = wtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTPOSSettings posset ON (orgadd.ID = posset.FOWSTID) " &
                       " LEFT JOIN dbo.MobizLibraryPeople ppol ON (orgadd.UserID = ppol.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgadd.OrganizationID IS NULL", " WHERE orgadd.OrganizationID = " & ReadyString(strID)) &
                        " AND (orgadd.Name LIKE '%" & SearchKeyword & "%' OR orgadd.Code LIKE '%" & SearchKeyword & "%')" &
                        " AND ISNULL(CONVERT(VARCHAR(36), orgadd.FowstTypeID), '') <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'" 'excluded fowst type PROJECT by arnie june 29

            .SqlOrder = " ORDER BY orgadd.Name ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgFOWSTPrinterIP(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            'With .Fields
            '    With .Add("ID", SqlDbType.UniqueIdentifier)
            '        .Expression = "orgaddpntr.ID"
            '        .Searchable = False
            '    End With
            '    With .Add("PrinterIPAddress", SqlDbType.Float)
            '        .Expression = "orgaddpntr.PrinterIPAddress"
            '    End With
            'End With

            '.SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTPrinter orgaddpntr "

            '.SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgaddpntr.FOWSTID IS NULL", " WHERE orgaddpntr.FOWSTID = " + ReadyString(strID))

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowstpntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterIPAddress", SqlDbType.VarChar)
                    .Expression = "fowstpntr.PrinterIPAddress"
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "ISNULL(fowstpntr.PrinterName, '')"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "dp.PrinterType"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceFowstPrinter devfowstpntr " &
                    " INNER JOIN LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " &
                    " INNER JOIN LibraryDevicePrinter dp on dp.ID = fowstpntr.PrinterID "

            .SqlWhere = If(strID.Trim().Length = 0, "WHERE fowstpntr.FOWSTID IS NULL", " WHERE fowstpntr.FOWSTID = " + ReadyString(strID)) &
                If(String.IsNullOrWhiteSpace(WebRequest.Instance.DeviceID), "", " AND devfowstpntr.DeviceID = " + ReadyString(WebRequest.Instance.DeviceID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgFOWSTLTOList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orglto.ID"
                    .Searchable = False
                End With
                With .Add("ORNo", SqlDbType.VarChar)
                    .Expression = "orglto.ORNo"
                End With
                With .Add("DateIssuedMonth", SqlDbType.Date)
                    .Expression = "MONTH(orglto.DateIssue)"
                End With
                With .Add("DateIssuedDay", SqlDbType.Date)
                    .Expression = "DAY(orglto.DateIssue)"
                End With
                With .Add("DateIssuedYear", SqlDbType.Date)
                    .Expression = "YEAR(orglto.DateIssue)"
                End With
                With .Add("ExpirationDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(orglto.ExpirationDate)"
                End With
                With .Add("ExpirationDateDay", SqlDbType.Date)
                    .Expression = "DAY(orglto.ExpirationDate)"
                End With
                With .Add("ExpirationDateYear", SqlDbType.Date)
                    .Expression = "YEAR(orglto.ExpirationDate)"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTLTO orglto "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orglto.FOWSTID IS NULL", " WHERE orglto.FOWSTID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgFOWSTLTFRBList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgltfrb.ID"
                    .Searchable = False
                End With
                With .Add("ORNo", SqlDbType.VarChar)
                    .Expression = "orgltfrb.ORNo"
                End With
                With .Add("DateIssuedMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgltfrb.DateIssue)"
                End With
                With .Add("DateIssuedDay", SqlDbType.Date)
                    .Expression = "DAY(orgltfrb.DateIssue)"
                End With
                With .Add("DateIssuedYear", SqlDbType.Date)
                    .Expression = "YEAR(orgltfrb.DateIssue)"
                End With
                With .Add("ExpirationDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgltfrb.ExpirationDate)"
                End With
                With .Add("ExpirationDateDay", SqlDbType.Date)
                    .Expression = "DAY(orgltfrb.ExpirationDate)"
                End With
                With .Add("ExpirationDateYear", SqlDbType.Date)
                    .Expression = "YEAR(orgltfrb.ExpirationDate)"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTLTFRB orgltfrb "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgltfrb.FOWSTID IS NULL", " WHERE orgltfrb.FOWSTID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgFOWSTSectionList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgsec.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "orgsec.Code"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "orgsec.Name"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "orgsec.Description"
                End With
                With .Add("FloorArea", SqlDbType.VarChar)
                    .Expression = "orgsec.FloorArea"
                End With
                With .Add("SpecialFeatures", SqlDbType.VarChar)
                    .Expression = "orgsec.SpecialFeatures"
                End With
                With .Add("RecState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTSection orgsec "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgsec.FOWSTID IS NULL", " WHERE orgsec.FOWSTID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgFOWSTBPermitList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgbpermit.ID"
                    .Searchable = False
                End With
                With .Add("BusinessPermitNo", SqlDbType.VarChar)
                    .Expression = "orgbpermit.BusinessPermitNo"
                End With
                With .Add("BusinessID", SqlDbType.VarChar)
                    .Expression = "orgbpermit.BusinessID"
                End With
                With .Add("TradeName", SqlDbType.VarChar)
                    .Expression = "orgbpermit.TradeName"
                End With
                With .Add("BuildingHouseNo", SqlDbType.VarChar)
                    .Expression = "orgbpermit.BuildingHouseNo"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "orgbpermit.Street"
                End With
                With .Add("AddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgbpermit.AddressCityID"
                    .Searchable = False
                End With
                With .Add("AddressCity", SqlDbType.VarChar)
                    .Expression = "cty.Name"
                End With
                With .Add("AddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgbpermit.AddressCountryID"
                    .Searchable = False
                End With
                With .Add("AddressCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                End With
                With .Add("ORNo", SqlDbType.VarChar)
                    .Expression = "orgbpermit.ORNo"
                End With
                With .Add("BusinessPlateNo", SqlDbType.VarChar)
                    .Expression = "orgbpermit.BusinessPlateNo"
                End With
                With .Add("DateOfPaymentMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgbpermit.DateOfPayment)"
                End With
                With .Add("DateOfPaymentDay", SqlDbType.Date)
                    .Expression = "DAY(orgbpermit.DateOfPayment)"
                End With
                With .Add("DateOfPaymentYear", SqlDbType.Date)
                    .Expression = "YEAR(orgbpermit.DateOfPayment)"
                End With
                With .Add("DateIssuedMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgbpermit.DateIssued)"
                End With
                With .Add("DateIssuedDay", SqlDbType.Date)
                    .Expression = "DAY(orgbpermit.DateIssued)"
                End With
                With .Add("DateIssuedYear", SqlDbType.Date)
                    .Expression = "YEAR(orgbpermit.DateIssued)"
                End With
                With .Add("RecState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With
                With .Add("DatePayment", SqlDbType.VarChar)
                    .Expression = "ISNULL(convert(varchar, orgbpermit.DateOfPayment, 107), '')"
                End With
                With .Add("DateIssued", SqlDbType.VarChar)
                    .Expression = "ISNULL(convert(varchar, orgbpermit.DateIssued, 107), '')"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTBusinessPermit orgbpermit " &
                       " LEFT JOIN dbo.MobizLibraryCities cty ON (orgbpermit.AddressCityID = cty.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgbpermit.AddressCountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgbpermit.FOWSTID IS NULL", " WHERE orgbpermit.FOWSTID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgFOWSTTelList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaddtel.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaddtel.FOWSTID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgaddtel.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "orgaddtel.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgaddtel.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgaddtel.ExtensionNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgaddtel.ContactType"
                End With
                With .Add("RecState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTTel orgaddtel " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgaddtel.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgaddtel.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgaddtel.FOWSTID IS NULL", " WHERE orgaddtel.FOWSTID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgFOWSTMobileList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaddmob.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaddmob.FOWSTID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgaddmob.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("TelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "orgaddmob.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgaddmob.ContactNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgaddmob.ContactType"
                End With
                With .Add("RecState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTMobile orgaddmob " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgaddmob.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (orgaddmob.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgaddmob.FOWSTID IS NULL", " WHERE orgaddmob.FOWSTID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgFOWSTEmailList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaddemail.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaddemail.FOWSTID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "orgaddemail.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "orgaddemail.EmailType"
                End With
                With .Add("RecState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTEmail orgaddemail"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgaddemail.FOWSTID IS NULL", " WHERE orgaddemail.FOWSTID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgFOWSTOperationHRList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "opthr.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "opthr.FOWSTID"
                    .Searchable = False
                End With
                With .Add("WeekDay", SqlDbType.VarChar)
                    .Expression = "wk.WeekDay"
                End With
                With .Add("OpeningTimeHour", SqlDbType.Int)
                    .Expression = "CASE " &
                                "     WHEN DATEPART(hour,opthr.OpeningTime) = 0 THEN 12 " &
                                "     WHEN (DATEPART(hour,opthr.OpeningTime) > 0 AND DATEPART(hour,opthr.OpeningTime) <= 12) THEN DATEPART(hour,opthr.OpeningTime) " &
                                "     WHEN DATEPART(hour,opthr.OpeningTime) > 12 THEN DATEPART(hour,opthr.OpeningTime) - 12 " &
                                "  END"
                End With
                With .Add("OpeningTimeMinute", SqlDbType.Int)
                    .Expression = "DATEPART(minute,opthr.OpeningTime)"
                End With
                With .Add("OpeningTimeAMPM", SqlDbType.Int)
                    .Expression = "CASE " &
                                "     WHEN DATEPART(hour,opthr.OpeningTime) < 12 THEN 'AM' " &
                                "     WHEN DATEPART(hour,opthr.OpeningTime) >=12 THEN 'PM' " &
                                "  END"
                End With
                With .Add("ClosingTimeHour", SqlDbType.Int)
                    .Expression = "CASE " &
                                "     WHEN DATEPART(hour,opthr.ClosingTime) = 0 THEN 12 " &
                                "     WHEN (DATEPART(hour,opthr.ClosingTime) > 0 AND DATEPART(hour,opthr.ClosingTime) <= 12) THEN DATEPART(hour,opthr.ClosingTime) " &
                                "     WHEN DATEPART(hour,opthr.ClosingTime) > 12 THEN DATEPART(hour,opthr.ClosingTime) - 12 " &
                                "  END"
                End With
                With .Add("ClosingTimeMinute", SqlDbType.Int)
                    .Expression = "DATEPART(minute,opthr.ClosingTime)"
                End With
                With .Add("ClosingTimeAMPM", SqlDbType.Int)
                    .Expression = "CASE " &
                                "     WHEN DATEPART(hour,opthr.ClosingTime) < 12 THEN 'AM' " &
                                "     WHEN DATEPART(hour,opthr.ClosingTime) >=12 THEN 'PM' " &
                                "  END"
                End With
                With .Add("IsClosed", SqlDbType.Int)
                    .Expression = "IsClosed "
                End With
            End With

            .SqlFrom = " FROM (SELECT 1 as WeekDay UNION " &
                 "       SELECT 2 as WeekDay UNION " &
                 "       SELECT 3 as WeekDay UNION " &
                 "       SELECT 4 as WeekDay UNION " &
                 "       SELECT 5 as WeekDay UNION " &
                 "       SELECT 6 as WeekDay UNION " &
                 "       SELECT 7 as WeekDay) wk " &
                 " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTOperationHRs opthr ON (opthr.WeekDay = wk.WeekDay " & IIf(strID.Trim().Length = 0, " AND opthr.FOWSTID IS NULL", " AND opthr.FOWSTID = " + ReadyString(strID)) & ") "

            .SqlOrder = " ORDER BY wk.WeekDay ASC"
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgFOWSTOperationHRListByWeekday(ByVal strID As String, ByVal weekDay As Integer) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTID"
                    .Searchable = False
                End With
                With .Add("OpeningTime", SqlDbType.Time)
                    .Expression = "OpeningTime"
                End With
                With .Add("OpeningTimeHour", SqlDbType.Int)
                    .Expression = "DATEPART(hour,OpeningTime)"
                End With
                With .Add("OpeningTimeMinute", SqlDbType.Int)
                    .Expression = "DATEPART(minute,OpeningTime)"
                End With
                With .Add("ClosingTime", SqlDbType.Time)
                    .Expression = "ClosingTime"
                End With
                With .Add("ClosingTimeHour", SqlDbType.Int)
                    .Expression = "DATEPART(hour,ClosingTime)"
                End With
                With .Add("ClosingTimeMinute", SqlDbType.Int)
                    .Expression = "DATEPART(minute,ClosingTime)"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTOperationHRs  "

            .SqlWhere = " WHERE FOWSTID = " & ReadyString(strID) & " AND WeekDay = " & weekDay

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgFOWSTInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 1
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FOWSTTypeID"
                    .Searchable = False
                End With
                With .Add("FOWSTStatus", SqlDbType.VarChar)
                    .Expression = "ISNULL(orgadd.Status, 'Available')"
                End With
                With .Add("FOWSTClassification", SqlDbType.VarChar)
                    .Expression = "ISNULL(orgadd.Classification, 'New Store')"
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "orgadd.Code"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "orgadd.Name"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "orgadd.Description"
                End With
                With .Add("BusinessTradeNameID", SqlDbType.VarChar)
                    .Expression = "orgadd.BusinessTradeNameID"
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "custtrade.BusinessTradeName"
                End With
                With .Add("TINBranchCode", SqlDbType.VarChar)
                    .Expression = "orgadd.TINBranchCode"
                End With
                With .Add("StreetNumber", SqlDbType.VarChar)
                    .Expression = "orgadd.StreetNumber"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "orgadd.Street"
                End With
                With .Add("FowstCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FowstCityID"
                    .Searchable = False
                End With
                With .Add("FowstCity", SqlDbType.VarChar)
                    .Expression = "cty.Name"
                    .Searchable = False
                End With
                With .Add("FowstCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.FowstCountryID"
                    .Searchable = False
                End With
                With .Add("Region", SqlDbType.VarChar)
                    .Expression = "orgadd.Region"
                    .Searchable = False
                End With
                With .Add("FowstCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                    .Searchable = False
                End With
                With .Add("SubFOWSTTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.SubFOWSTTypeID"
                    .Searchable = False
                End With
                With .Add("RetailType", SqlDbType.VarChar)
                    .Expression = "rtype.Name"
                End With
                With .Add("WarehouseType", SqlDbType.VarChar)
                    .Expression = "wtype.Name"
                End With
                With .Add("PlateNo", SqlDbType.VarChar)
                    .Expression = "orgadd.PlateNo"
                End With
                With .Add("NoOfWheels", SqlDbType.VarChar)
                    .Expression = "orgadd.NoOfWheels"
                End With
                With .Add("BodyColor", SqlDbType.VarChar)
                    .Expression = "orgadd.BodyColor"
                End With
                With .Add("Weight", SqlDbType.Float)
                    .Expression = "orgadd.Weight"
                End With
                With .Add("Dimension", SqlDbType.VarChar)
                    .Expression = "orgadd.Dimension"
                End With
                With .Add("OwnershipType", SqlDbType.Int)
                    .Expression = "orgadd.OwnershipType"
                End With
                With .Add("ContractStartMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgadd.ContractStart)"
                End With
                With .Add("ContractStartDay", SqlDbType.Date)
                    .Expression = "DAY(orgadd.ContractStart)"
                End With
                With .Add("ContractStartYear", SqlDbType.Date)
                    .Expression = "YEAR(orgadd.ContractStart)"
                End With
                With .Add("ContractEndMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgadd.ContractEnd)"
                End With
                With .Add("ContractEndDay", SqlDbType.Date)
                    .Expression = "DAY(orgadd.ContractEnd)"
                End With
                With .Add("ContractEndYear", SqlDbType.Date)
                    .Expression = "YEAR(orgadd.ContractEnd)"
                End With
                With .Add("CostPerMonth", SqlDbType.Decimal)
                    .Expression = "orgadd.CostPerMonth"
                End With
                With .Add("IsMortgaged", SqlDbType.Bit)
                    .Expression = "orgadd.IsMortgaged"
                End With
                With .Add("EnforceZRead", SqlDbType.Bit)
                    .Expression = "orgadd.EnforceZRead"
                End With
                With .Add("IsFranchisee", SqlDbType.Bit)
                    .Expression = "orgadd.IsFranchisee"
                End With
                With .Add("PrintStickers", SqlDbType.Bit)
                    .Expression = "orgadd.PrintStickers"
                End With
                With .Add("PrintQueueNumbers", SqlDbType.Bit)
                    .Expression = "orgadd.PrintQueueNumbers"
                End With
                With .Add("FranchisorID", SqlDbType.UniqueIdentifier)
                    .Expression = "(SELECT TOP 1 FranchisorOrgID FROM MobizLibraryOrganizationFranchisee " &
                        " WHERE IsActive = 1 AND FranchiseeOrgFowstID = orgadd.ID AND LastUpdated = (SELECT MAX(LastUpdated) FROM MobizLibraryOrganizationFranchisee WHERE IsActive = 1 AND FranchiseeOrgFowstID = orgadd.ID)) "
                    .Searchable = False
                End With
                With .Add("Franchisor", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 MobizLibraryOrganization.CompanyName FROM MobizLibraryOrganizationFranchisee INNER JOIN " &
                        " MobizLibraryOrganization ON (MobizLibraryOrganizationFranchisee.FranchisorOrgID = MobizLibraryOrganization.ID) " &
                        " WHERE MobizLibraryOrganizationFranchisee.IsActive = 1 AND MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = orgadd.ID " &
                        " AND MobizLibraryOrganizationFranchisee.LastUpdated = (SELECT MAX(LastUpdated) FROM MobizLibraryOrganizationFranchisee WHERE IsActive = 1 AND FranchiseeOrgFowstID = orgadd.ID))"
                End With
                With .Add("IsFranchiseeActive", SqlDbType.Bit)
                    .Expression = "franchisee.IsActive"
                End With
                With .Add("IsFranchiseeConfirmed", SqlDbType.Bit)
                    .Expression = "franchisee.IsFranchiseeConfirmed"
                End With
                With .Add("IsFranchisorConfirmed", SqlDbType.Bit)
                    .Expression = "franchisee.IsFranchisorConfirmed"
                End With
                With .Add("VATRate", SqlDbType.Float)
                    .Expression = "ISNULL(ctry.VATRate,0)"
                End With
                With .Add("AccountID", SqlDbType.VarChar)
                    .Expression = "orgadd.AccountID"
                End With
                With .Add("StoreID", SqlDbType.VarChar)
                    .Expression = "orgadd.StoreID"
                End With
                With .Add("AccountID2", SqlDbType.VarChar)
                    .Expression = "orgadd.AccountID2"
                End With
                With .Add("StoreID2", SqlDbType.VarChar)
                    .Expression = "orgadd.StoreID2"
                End With
                With .Add("IsIncludeSpecialReport", SqlDbType.Bit)
                    .Expression = "orgadd.IsIncludeSpecialReport"
                    .Searchable = False
                End With
                With .Add("ShopperTrakAccountID", SqlDbType.VarChar)
                    .Expression = "orgadd.ShopperTrakAccountID"
                End With
                With .Add("IsIncludeToShopperTrakReport", SqlDbType.Bit)
                    .Expression = "orgadd.IsIncludeToShopperTrakReport"
                    .Searchable = False
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "orgadd.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "orgadd.FOWSTMallName"
                End With
                With .Add("FtpServer", SqlDbType.VarChar)
                    .Expression = "orgadd.FtpServer"
                End With
                With .Add("FtpUsername", SqlDbType.VarChar)
                    .Expression = "orgadd.FtpUsername"
                End With
                'With .Add("FtpPassword", SqlDbType.VarChar)
                '    .Expression = "orgadd.FtpPassword"
                'End With
                With .Add("AyalaMallRecID", SqlDbType.UniqueIdentifier)
                    .Expression = "ayala.ID"
                End With
                With .Add("AyalaTenantCode", SqlDbType.VarChar)
                    .Expression = "ayala.TenantCode"
                End With
                With .Add("AyalaTenantName", SqlDbType.VarChar)
                    .Expression = "ayala.TenantName"
                End With
                With .Add("TenantNameForDBF", SqlDbType.VarChar)
                    .Expression = "ayala.TenantNameForDBF"
                End With
                With .Add("MerchantsLeaseOutNumber", SqlDbType.VarChar)
                    .Expression = "ayala.MerchantsLeaseOutNumber"
                End With
                With .Add("SMMallRecID", SqlDbType.UniqueIdentifier)
                    .Expression = "sm.ID"
                End With
                With .Add("SMMallCode", SqlDbType.VarChar)
                    .Expression = "sm.MallCode"
                End With
                With .Add("SMMallTenantNo", SqlDbType.VarChar)
                    .Expression = "sm.TenantNo"
                End With
                With .Add("SMMallClassCode", SqlDbType.Int)
                    .Expression = "sm.ClassCode"
                End With
                With .Add("SMMallTradeCode", SqlDbType.VarChar)
                    .Expression = "sm.TradeCode"
                End With
                With .Add("SMMallOutletNo", SqlDbType.VarChar)
                    .Expression = "sm.OutletNo"
                End With
                With .Add("SMMallSaleType", SqlDbType.VarChar)
                    .Expression = "sm.SaleType"
                End With
                With .Add("MegaworldRecID", SqlDbType.UniqueIdentifier)
                    .Expression = "mega.ID"
                End With
                With .Add("MegaworldTenantCode", SqlDbType.VarChar)
                    .Expression = "mega.TenantCode"
                End With
                With .Add("MegaworldSaleType", SqlDbType.VarChar)
                    .Expression = "mega.SaleType"
                End With
                With .Add("MegaworldTenantPOSTerminalNo", SqlDbType.VarChar)
                    .Expression = "mega.TenantPOSTerminalNo"
                End With
                With .Add("RobinsonTenantId", SqlDbType.VarChar)
                    .Expression = "orgadd.RobinsonTenantId"
                End With
                With .Add("RobinsonPosTerminalNum", SqlDbType.VarChar)
                    .Expression = "orgadd.RobinsonPosTerminalNum"
                End With
                With .Add("PITXTenantId", SqlDbType.VarChar)
                    .Expression = "orgadd.PITXTenantId"
                End With
                With .Add("PITXPosTerminalID", SqlDbType.VarChar)
                    .Expression = "orgadd.PITXPosTerminalID"
                End With
                With .Add("EnableHotButtonsContextualMenu", SqlDbType.Bit)
                    .Expression = "orgadd.EnableHotButtonsContextualMenu"
                End With
                With .Add("DefaultPosView", SqlDbType.VarChar)
                    .Expression = "orgadd.DefaultPosView"
                End With
                With .Add("IsActive", SqlDbType.VarChar)
                    .Expression = "ISNULL(orgadd.IsActive, 1)"
                End With
                With .Add("IsVAT", SqlDbType.Bit)
                    .Expression = "ISNULL(orgadd.IsVAT, 1)"
                End With
                With .Add("DefaultHotButtonTemplateID", SqlDbType.VarChar)
                    .Expression = "ISNULL(convert(varchar(max), orgadd.DefaultHotButtonTemplateID), '')"
                End With
                With .Add("GPSLocation", SqlDbType.VarChar)
                    .Expression = "orgadd.GPSLocation"
                End With
                With .Add("StoreLocationTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.StoreLocationTypeID"
                End With
                With .Add("Operates247", SqlDbType.Bit)
                    .Expression = "orgadd.Operates247"
                End With
                With .Add("ReceiptSettingsID", SqlDbType.VarChar)
                    .Expression = "cussrecset.ReceiptSettingsID"
                End With
                With .Add("PosVendorID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgadd.PosVendorID"
                End With
                With .Add("PosVendorName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPosVendors.Name"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST orgadd " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationTradeName custtrade ON (orgadd.BusinessTradeNameID = custtrade.ID) " &
                       " LEFT JOIN dbo.LibraryReceiptSettingsFowst cussrecset ON (cussrecset.FOWSTID = orgadd.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCities cty ON (orgadd.FowstCityID = cty.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgadd.FowstCountryID = ctry.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTType rtype ON (orgadd.SubFOWSTTypeID = rtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgadd.SubFOWSTTypeID = wtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTPOSSettings posset ON (orgadd.ID = posset.FOWSTID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTMallInfoAyala ayala ON (orgadd.ID = ayala.FOWSTID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTMallInfoSM sm ON (orgadd.ID = sm.FOWSTID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTMallInfoMegaworld mega ON (orgadd.ID = mega.FOWSTID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFranchisee franchisee ON (orgadd.ID = franchisee.FranchiseeOrgFowstID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganization franchisor ON (franchisee.FranchisorOrgID = franchisor.ID) " & vbCrLf &
                       " LEFT JOIN MobizLibraryPosVendors ON MobizLibraryPosVendors.ID = orgadd.PosVendorID"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgadd.ID IS NULL", " WHERE orgadd.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgFOWST() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String
        'Dim pFowstTypeName As String = ""

        Try
            'If String.IsNullOrWhiteSpace(FOWSTTypeID) = False Then
            '    Dim pDrFowstType = WebRequest.Instance.Business.LibraryOrganizationFOWST.FowstType(FOWSTTypeID)
            '    If pDrFowstType IsNot Nothing Then
            '        pFowstTypeName = CStr2(pDrFowstType("Name"))
            '    End If
            'End If
            p("OrganizationID") = If(OrganizationID.Trim().Length > 0, ReadyString(OrganizationID), "NULL")
            p("FOWSTTypeID") = If(FOWSTTypeID.Trim().Length > 0, ReadyString(FOWSTTypeID), "NULL")
            p("Status") = If(FOWSTStatus.Trim().Length > 0, ReadyString(FOWSTStatus), "NULL")
            p("Classification") = If(String.IsNullOrWhiteSpace(FOWSTClassification), "NULL", ReadyString(FOWSTClassification))
            p("Code") = ReadyString(FOWSTCode)
            p("Name") = ReadyString(FOWSTName)
            p("Description") = ReadyString(FOWSTDescription)
            p("BusinessTradeNameID") = If(String.IsNullOrWhiteSpace(FOWSTBusinessTradeNameID), "NULL", ReadyString(FOWSTBusinessTradeNameID))
            p("TINBranchCode") = ReadyString(FOWSTTINBranchCode)
            p("DefaultHotButtonTemplateID") = If(DefaultHotButtonTemplateID <> "", ReadyString(DefaultHotButtonTemplateID), "NULL")
            p("GPSLocation") = ReadyString(GPSLocation)

            p("StreetNumber") = ReadyString(FOWSTStreetNumber)
            p("Street") = ReadyString(FOWSTStreet)
            p("FowstCityID") = If(FOWSTCityID.Trim().Length > 0, ReadyString(FOWSTCityID), "NULL")
            p("FowstCountryID") = If(FOWSTCountryID.Trim().Length > 0, ReadyString(FOWSTCountryID), "NULL")
            p("Region") = ReadyString(FOWSTRegion)
            p("SubFOWSTTypeID") = If(FOWSTSubTypeID.Trim().Length > 0, ReadyString(FOWSTSubTypeID), "NULL")
            p("StoreLocationTypeID") = If(FOWSTStoreLocationTypeID.Trim().Length > 0, ReadyString(FOWSTStoreLocationTypeID), "NULL")

            p("PlateNo") = ReadyString(FOWSTPlateNo)
            p("NoOfWheels") = ReadyString(FOWSTNoOfWheels)
            p("BodyColor") = ReadyString(FOWSTBodyColor)
            p("Weight") = If(FOWSTWeight.Trim().Length > 0, FOWSTWeight, "0")
            p("Dimension") = ReadyString(FOWSTDimension)

            p("OwnershipType") = FOWSTOwnershipType
            p("ContractStart") = If(FOWSTContractStart.Trim().Length > 0, ReadyString(FOWSTContractStart), "NULL")
            p("ContractEnd") = If(FOWSTContractEnd.Trim().Length > 0, ReadyString(FOWSTContractEnd), "NULL")
            p("CostPerMonth") = If(FOWSTCostPerMonth.Trim().Length > 0, FOWSTCostPerMonth, "0")
            p("IsMortgaged") = If(FOWSTIsMortgaged.Trim().Length > 0, FOWSTIsMortgaged, "0")
            p("EnforceZRead") = If(FOWSTIsMortgaged.Trim().Length > 0, EnforceZRead, "0")

            p("IsFOWSTInsideAMall") = If(FOWSTIsInsideAMall.Trim().Length > 0, FOWSTIsInsideAMall, "0")
            p("Operates247") = If(CBool2(Operates247), "1", "0")
            p("FOWSTMallName") = ReadyString(FOWSTMallName)
            p("FtpServer") = ReadyString(FtpServer)
            p("FtpUsername") = ReadyString(FtpUsername)
            If String.IsNullOrEmpty(FtpPassword) = False Then
                p("FtpPassword") = ReadyString(FtpPassword)
            End If

            p("IsFranchisee") = If(FOWSTIsFranchisee.Trim().Length > 0, FOWSTIsFranchisee, "0")
            p("PrintStickers") = If(FOWSTIsFranchisee.Trim().Length > 0, FOWSTIsFranchisee, "0")
            p("FranchisorID") = If(FOWSTFranchisorID.Trim().Length > 0, ReadyString(FOWSTFranchisorID), "NULL")
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

            p("AccountID") = If(FOWSTAccountID.Trim().Length > 0, ReadyString(FOWSTAccountID), "NULL")
            p("StoreID") = If(FOWSTStoreID.Trim().Length > 0, ReadyString(FOWSTStoreID), "NULL")
            p("AccountID2") = If(FOWSTAccountID2.Trim().Length > 0, ReadyString(FOWSTAccountID2), "NULL")
            p("StoreID2") = If(FOWSTStoreID2.Trim().Length > 0, ReadyString(FOWSTStoreID2), "NULL")
            p("IsIncludeSpecialReport") = FOWSTIsIncludeReport

            p("IsIncludeToShopperTrakReport") = FOWSTIsIncludeToShopperTrakReport
            p("ShopperTrakAccountID") = ReadyString(FOWSTShopperTrakAccountID)

            p("EnableHotButtonsContextualMenu") = If(EnableHotButtonsContextualMenu, 1, 0)
            p("DefaultPosView") = ReadyString(DefaultPosView)
            p("IsActive") = CInt2(IsActive)
            p("PrintStickers") = CInt2(PrintStickers)
            p("PrintQueueNumbers") = CInt2(PrintQueueNumbers)
            p("IsVAT") = CInt2(IsVAT)

            p("RobinsonTenantId") = ReadyString(RobinsonTenantId)
            p("RobinsonPosTerminalNum") = ReadyString(RobinsonPosTerminalNum)

            p("PITXTenantId") = ReadyString(PITXTenantId)
            p("PITXPosTerminalID") = ReadyString(PITXPosTerminalID)

            p("LastUpdated") = "getdate()"
            If String.IsNullOrWhiteSpace(PosVendorID) Then
                p("PosVendorID") = "NULL"
            Else
                p("PosVendorID") = ReadyString(PosVendorID)
            End If

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                p("FOWSTNo") = "ISNULL((SELECT MAX(FOWSTNo) FROM MobizLibraryOrganizationFOWST WHERE OrganizationID " & If(OrganizationID.Trim().Length > 0, " = " & ReadyString(OrganizationID), " IS NULL") & "),0) + 1"
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Delete from LibraryReceiptSettingsFowst WHERE FOWSTID=" & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("LibraryReceiptSettingsFowst")

            p("ReceiptSettingsID") = ReadyString(ReceiptSettingsID)
            p("FowstID") = ReadyString(strIDReturn)

            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Ayala Info
            p = New InsertUpdate("MobizLibraryOrganizationFOWSTMallInfoAyala")

            p("FOWSTID") = ReadyString(strIDReturn)
            p("TenantCode") = ReadyString(FOWSTAyalaTenantCode)
            p("TenantName") = ReadyString(FOWSTAyalaTenantName)
            p("TenantNameForDBF") = ReadyString(FOWSTAyalaTenantNameForDBF)
            p("MerchantsLeaseOutNumber") = ReadyString(FOWSTAyalaMerchantsLeaseOutNumber)

            If (FOWSTAyalaRecID.Trim().Length > 0) Then
                strIDRec = FOWSTAyalaRecID
                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
            Else
                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
            End If
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save SM Info
            p = New InsertUpdate("MobizLibraryOrganizationFOWSTMallInfoSM")

            p("FOWSTID") = ReadyString(strIDReturn)
            p("MallCode") = ReadyString(FOWSTSMMallCode)
            p("TenantNo") = ReadyString(FOWSTSMMallTenantNo)
            p("ClassCode") = IIf(FOWSTSMMallClassCode.Trim().Length > 0, FOWSTSMMallClassCode, "0")
            p("TradeCode") = ReadyString(FOWSTSMMallTradeCode)
            p("OutletNo") = IIf(FOWSTSMMallOutletNo.Trim().Length > 0, FOWSTSMMallOutletNo, "0")
            p("SaleType") = ReadyString(FOWSTSMMallSaleType)

            If (FOWSTSMRecID.Trim().Length > 0) Then
                strIDRec = FOWSTSMRecID
                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
            Else
                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
            End If
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Megaworld Info
            p = New InsertUpdate("MobizLibraryOrganizationFOWSTMallInfoMegaworld")

            p("FOWSTID") = ReadyString(strIDReturn)
            p("TenantCode") = ReadyString(FOWSTMegaworldTenantCode)
            p("SaleType") = ReadyString(FOWSTMegaworldSaleType)
            p("TenantPOSTerminalNo") = ReadyString(FOWSTMegaworldNoOfPOSTerminal)

            If (FOWSTMegaworldRecID.Trim().Length > 0) Then
                strIDRec = FOWSTMegaworldRecID
                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
            Else
                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
            End If
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save LTO
            For Each objOrgAddLTO As Object In OrgFOWSTLTOList

                If (objOrgAddLTO("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddLTO("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTLTO")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("ORNo") = ReadyString(objOrgAddLTO("ORNo").ToString())
                        p("DateIssue") = IIf(objOrgAddLTO("DateIssue").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddLTO("DateIssue").ToString().Trim()))
                        p("ExpirationDate") = IIf(objOrgAddLTO("ExpirationDate").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddLTO("ExpirationDate").ToString().Trim()))

                        If (objOrgAddLTO("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddLTO("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTLTO " &
                               " WHERE ID " & IIf(objOrgAddLTO("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddLTO("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddLTO

            '''Save LTFRB
            For Each objOrgAddLTFRB As Object In OrgFOWSTLTFRBList

                If (objOrgAddLTFRB("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddLTFRB("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTLTFRB")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("ORNo") = ReadyString(objOrgAddLTFRB("ORNo").ToString())
                        p("DateIssue") = IIf(objOrgAddLTFRB("DateIssue").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddLTFRB("DateIssue").ToString().Trim()))
                        p("ExpirationDate") = IIf(objOrgAddLTFRB("ExpirationDate").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddLTFRB("ExpirationDate").ToString().Trim()))

                        If (objOrgAddLTFRB("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddLTFRB("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTLTFRB " &
                               " WHERE ID " & IIf(objOrgAddLTFRB("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddLTFRB("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddLTFRB

            '''Save Section
            For Each objOrgAddSection As Object In OrgFOWSTSectionList

                If (objOrgAddSection("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddSection("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTSection")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("Code") = ReadyString(objOrgAddSection("Code").ToString())
                        p("Name") = ReadyString(objOrgAddSection("Name").ToString())
                        p("Description") = ReadyString(objOrgAddSection("Description").ToString())
                        p("FloorArea") = ReadyString(objOrgAddSection("FloorArea").ToString())
                        p("SpecialFeatures") = ReadyString(objOrgAddSection("SpecialFeatures").ToString())

                        If (objOrgAddSection("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddSection("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        'If pFowstTypeName.ToLower() = "warehouse" Then
                        p = New InsertUpdate("LibraryFowstBin")
                        p("Area") = ReadyString(objOrgAddSection("Name").ToString())
                        pSql = p.GetUpdate("SectionID = " & ReadyString(strIDRec) & vbCrLf &
                            " AND FowstID = " & ReadyString(strIDReturn))
                        Dim pAffectedRecords = Common.WebRequest.ProData.ExecuteSQL(pSql)
                        If pAffectedRecords = 0 Then
                            p("FowstID") = ReadyString(strIDReturn)
                            p("Name") = ReadyString(objOrgAddSection("Name").ToString())
                            p("Description") = ReadyString(objOrgAddSection("Description").ToString())
                            p("SectionID") = ReadyString(strIDRec)
                            p("IsActive") = 1
                            p("PeopleID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                        'End If
                    Else
                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTSection " &
                               " WHERE ID " & IIf(objOrgAddSection("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddSection("ID").ToString().Trim()))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        'If pFowstTypeName.ToLower() = "warehouse" Then
                        If objOrgAddSection("ID").ToString().Trim().Length > 0 Then
                            pSql = "DELETE FROM LibraryFowstBin" & vbCrLf &
                               " WHERE SectionID = " & ReadyString(objOrgAddSection("ID").ToString().Trim()) & vbCrLf &
                               " AND FowstID = " & ReadyString(strIDReturn)
                            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                            'End If
                        End If
                    End If
                End If

            Next objOrgAddSection

            '''Save BPermit
            For Each objOrgAddBPermit As Object In OrgFOWSTBPermitList

                If (objOrgAddBPermit("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddBPermit("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTBusinessPermit")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("BusinessPermitNo") = ReadyString(objOrgAddBPermit("BusinessPermitNo").ToString())
                        p("BusinessID") = ReadyString(objOrgAddBPermit("BusinessID").ToString())
                        p("TradeName") = ReadyString(objOrgAddBPermit("TradeName").ToString())
                        p("BuildingHouseNo") = ReadyString(objOrgAddBPermit("BuildingHouseNo").ToString())
                        p("Street") = ReadyString(objOrgAddBPermit("Street").ToString())
                        p("AddressCityID") = IIf(objOrgAddBPermit("AddressCityID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddBPermit("AddressCityID").ToString().Trim()))
                        p("AddressCountryID") = IIf(objOrgAddBPermit("AddressCountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddBPermit("AddressCountryID").ToString().Trim()))
                        p("ORNo") = ReadyString(objOrgAddBPermit("ORNo").ToString())
                        p("BusinessPlateNo") = ReadyString(objOrgAddBPermit("BusinessPlateNo").ToString())
                        p("DateOfPayment") = IIf(objOrgAddBPermit("DatePayment").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddBPermit("DatePayment").ToString().Trim()))
                        p("DateIssued") = IIf(objOrgAddBPermit("DateIssued").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddBPermit("DateIssued").ToString().Trim()))

                        If (objOrgAddBPermit("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddBPermit("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTBusinessPermit " &
                               " WHERE ID " & IIf(objOrgAddBPermit("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddBPermit("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddBPermit

            '''Save Tel List
            For Each objOrgAddTel As Object In OrgFOWSTTelList

                If (objOrgAddTel("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddTel("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTTel")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("CountryID") = IIf(objOrgAddTel("CountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddTel("CountryID").ToString().Trim()))
                        p("AreaCodeID") = IIf(objOrgAddTel("AreaCodeID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddTel("AreaCodeID").ToString().Trim()))
                        p("ContactNo") = ReadyString(objOrgAddTel("ContactNo").ToString())
                        p("ExtensionNo") = ReadyString(objOrgAddTel("ExtensionNo").ToString())
                        p("ContactType") = ReadyString(objOrgAddTel("ContactType").ToString())
                        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        p("LastUpdated") = "getdate()"

                        If (objOrgAddTel("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddTel("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTTel " &
                               " WHERE ID " & IIf(objOrgAddTel("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddTel("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddTel

            '''Save Mobile List
            For Each objOrgAddMobile As Object In OrgFOWSTMobileList

                If (objOrgAddMobile("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddMobile("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTMobile")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("CountryID") = IIf(objOrgAddMobile("CountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddMobile("CountryID").ToString().Trim()))
                        p("TelCoPrefixID") = IIf(objOrgAddMobile("TelCoPrefixID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgAddMobile("TelCoPrefixID").ToString().Trim()))
                        p("ContactNo") = ReadyString(objOrgAddMobile("ContactNo").ToString())
                        p("ContactType") = ReadyString(objOrgAddMobile("ContactType").ToString())
                        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        p("LastUpdated") = "getdate()"

                        If (objOrgAddMobile("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddMobile("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTMobile " &
                               " WHERE ID " & IIf(objOrgAddMobile("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddMobile("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddMobile

            '''Save Email List
            For Each objOrgAddEmail As Object In OrgFOWSTEmailList

                If (objOrgAddEmail("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddEmail("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTEmail")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("EmailAddress") = ReadyString(objOrgAddEmail("EmailAddress").ToString())
                        p("EmailType") = ReadyString(objOrgAddEmail("EmailType").ToString())
                        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        p("LastUpdated") = "getdate()"

                        If (objOrgAddEmail("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgAddEmail("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTEmail " &
                               " WHERE ID " & IIf(objOrgAddEmail("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddEmail("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddEmail

            '''Save Operation HR List
            For Each objOrgAddOptHR As Object In OrgFOWSTOperationHRList

                'If (objOrgAddOptHR("OpeningTimeHR").ToString().Trim().Length > 0) Then
                p = New InsertUpdate("MobizLibraryOrganizationFOWSTOperationHRs")

                p("FOWSTID") = ReadyString(strIDReturn)
                p("WeekDay") = ReadyString(objOrgAddOptHR("WeekDay").ToString())

                If (objOrgAddOptHR("OpeningTimeHR").ToString().Trim().Length > 0) Then
                    p("OpeningTime") = ReadyString(objOrgAddOptHR("OpeningTimeHR").ToString() + ":" + objOrgAddOptHR("OpeningTimeMin").ToString() + ":00 " + objOrgAddOptHR("OpeningTimeAMPM").ToString())
                End If

                If (objOrgAddOptHR("ClosingTimeHR").ToString().Trim().Length > 0) Then
                    p("ClosingTime") = ReadyString(objOrgAddOptHR("ClosingTimeHR").ToString() + ":" + objOrgAddOptHR("ClosingTimeMin").ToString() + ":00 " + objOrgAddOptHR("ClosingTimeAMPM").ToString())
                End If

                p("IsClosed") = CInt2(objOrgAddOptHR("IsClosed"))
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                p("LastUpdated") = "getdate()"

                If (objOrgAddOptHR("ID").ToString().Trim().Length <= 0) Then
                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()
                Else
                    strIDRec = objOrgAddOptHR("ID").ToString()
                    pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                'Else

                'pSql = "DELETE " &
                '           " FROM dbo.MobizLibraryOrganizationFOWSTOperationHRs " &
                '           " WHERE ID " & IIf(objOrgAddOptHR("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddOptHR("ID").ToString().Trim()))

                '    Common.WebRequest.ProData.ExecuteSQL(pSql)

                'End If

            Next objOrgAddOptHR

            'additional by janine
            '''Save Printer List
            For Each objOrgAddPrinter As Object In OrgFOWSTPrinterList

                If (objOrgAddPrinter("PrinterIPAddress").ToString().Trim().Length > 0) Then
                    p = New InsertUpdate("LibraryDevicePrinterFowst")

                    p("PrinterName") = ReadyString(objOrgAddPrinter("PrinterName").ToString())
                    p("PrinterID") = ReadyString(objOrgAddPrinter("PrinterID").ToString())
                    p("PrinterIPAddress") = ReadyString(objOrgAddPrinter("PrinterIPAddress").ToString())
                    p("FowstID") = ReadyString(strIDReturn)

                    If (objOrgAddPrinter("ID").ToString().Trim().Length <= 0) Then
                        strIDRec = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strIDRec)
                        pSql = p.GetInsert()
                    Else
                        strIDRec = objOrgAddPrinter("ID").ToString()
                        pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                Else

                    pSql = "DELETE " &
                           " FROM dbo.LibraryDevicePrinterFowst " &
                           " WHERE ID " & IIf(objOrgAddPrinter("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddPrinter("ID").ToString().Trim()))

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                End If

            Next objOrgAddPrinter

            '''Save Franchise Info
            ''' 'TODO
            ''' 

            'check if it's not a franchisee
            If (FOWSTIsFranchisee.Trim() = "0") Then

                pSql = "SELECT ID FROM MobizLibraryOrganizationFranchisee WHERE IsActive = 1 AND FranchiseeOrgFowstID = " & ReadyString(strIDReturn)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                'if it is already an active franchisee, then deactivate it
                'auto activate aug. 16, 2022
                If (pDT.Rows.Count > 0) Then
                    p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

                    p("IsFranchiseeConfirmed") = "1"
                    p("IsFranchisorConfirmed") = "1"
                    p("IsActive") = "1"
                    p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                    p("LastUpdated") = "getdate()"

                    strIDRec = pDT.Rows(0).Item("ID").ToString()
                    pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "SELECT ID FROM MobizLibraryOrganizationFranchiseStatus WHERE FranchiseID = " & ReadyString(strIDRec) &
                   " AND DateDeactivated IS NULL ORDER BY LastUpdated DESC"
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    'if there is an existing franchise, deactivate it because the user says it's not a franchisee
                    If (pDT.Rows.Count > 0) Then
                        p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")

                        p("FranchiseID") = ReadyString(strIDRec)
                        p("DateDeactivated") = "getdate()"
                        p("DeactivatedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        p("LastUpdated") = "getdate()"

                        strIDRec = pDT.Rows(0).Item("ID").ToString()
                        pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If




            Else

                If (FOWSTFranchisorID.ToString().Trim().Length > 0) Then
                    pSql = "SELECT fran.ID, fran.IsActive, stat.ID as StatusID, stat.DeactivatedByID FROM MobizLibraryOrganizationFranchisee fran INNER JOIN " &
                        " MobizLibraryOrganizationFranchiseStatus stat ON fran.ID = stat.FranchiseID WHERE FranchisorOrgID = " & ReadyString(FOWSTFranchisorID.ToString().Trim()) &
                    " AND FranchiseeOrgFowstID = " & ReadyString(ID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)


                    If (pDT.Rows.Count > 0) Then
                        'if theres an existing record, update isactive then the status also
                        If (Not Convert.ToBoolean(pDT.Rows(0).Item("IsActive").ToString())) Then
                            p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

                            p("IsFranchiseeConfirmed") = "1"
                            p("IsActive") = "1"
                            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                            p("LastUpdated") = "getdate()"

                            If CBool2(Common.WebRequest.Data("IsAutoApproveFranchisorOrFranchiseeRequest")) Then
                                p("IsFranchisorConfirmed") = "1"
                            End If

                            strIDRec = pDT.Rows(0).Item("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            '''''''''''''''''
                            'if no deactivated record, update existing.
                            'if deactivated already, create new status
                            If (CStr2(pDT.Rows(0).Item("DeactivatedByID").ToString()) = "") Then
                                p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")

                                p("FranchiseID") = ReadyString(strIDRec)
                                p("DateRequested") = "getdate()"
                                p("RequestedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                                p("LastUpdated") = "getdate()"

                                If CBool2(Common.WebRequest.Data("IsAutoApproveFranchisorOrFranchiseeRequest")) Then
                                    p("DateActivated") = "getdate()"
                                    p("ActivatedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                                End If

                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("StatusID").ToString()))


                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                            Else
                                p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")

                                p("FranchiseID") = ReadyString(strIDRec)
                                p("DateRequested") = "getdate()"
                                p("RequestedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                                p("LastUpdated") = "getdate()"

                                If CBool2(Common.WebRequest.Data("IsAutoApproveFranchisorOrFranchiseeRequest")) Then
                                    p("DateActivated") = "getdate()"
                                    p("ActivatedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                                End If

                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()


                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                            End If



                        End If

                    Else
                        'if no existing, insert
                        p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

                        p("FranchisorOrgID") = ReadyString(FOWSTFranchisorID.ToString().Trim())
                        p("FranchiseeOrgFowstID") = ReadyString(strIDReturn)
                        p("IsFranchiseeConfirmed") = "1"
                        p("IsActive") = "1"
                        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        p("LastUpdated") = "getdate()"

                        If CBool2(Common.WebRequest.Data("IsAutoApproveFranchisorOrFranchiseeRequest")) Then
                            p("IsFranchisorConfirmed") = "1"
                        End If

                        strIDRec = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strIDRec)
                        pSql = p.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        ''''''''''''''''''''
                        p = New InsertUpdate("MobizLibraryOrganizationFranchiseStatus")

                        p("FranchiseID") = ReadyString(strIDRec)
                        p("DateRequested") = "getdate()"
                        p("RequestedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        p("LastUpdated") = "getdate()"

                        If CBool2(Common.WebRequest.Data("IsAutoApproveFranchisorOrFranchiseeRequest")) Then
                            p("DateActivated") = "getdate()"
                            p("ActivatedByID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                        End If

                        strIDRec = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strIDRec)
                        pSql = p.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql)


                    End If
                End If

            End If
            'end of additional

            'quota

            pSql = "Delete from LibraryOrganizationFOWSTQuota where FOWSTID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If IsNumeric(Common.WebRequest.Data("JanQuota")) And IsNumeric(Common.WebRequest.Data("JanIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")

                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("1/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("JanQuota")), CDbl(Common.WebRequest.Data("JanQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("JanIncentive")), CInt(Common.WebRequest.Data("JanIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("FebQuota")) And IsNumeric(Common.WebRequest.Data("FebIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("2/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("FebQuota")), CDbl(Common.WebRequest.Data("FebQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("FebIncentive")), CInt(Common.WebRequest.Data("FebIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("MarQuota")) And IsNumeric(Common.WebRequest.Data("MarIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("3/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("MarQuota")), CDbl(Common.WebRequest.Data("MarQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("MarIncentive")), CInt(Common.WebRequest.Data("MarIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("AprQuota")) And IsNumeric(Common.WebRequest.Data("AprIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("4/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("AprQuota")), CDbl(Common.WebRequest.Data("AprQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("AprIncentive")), CInt(Common.WebRequest.Data("AprIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("MayQuota")) And IsNumeric(Common.WebRequest.Data("MayIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("5/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("MayQuota")), CDbl(Common.WebRequest.Data("MayQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("MayIncentive")), CInt(Common.WebRequest.Data("MayIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("JunQuota")) And IsNumeric(Common.WebRequest.Data("JunIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("6/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("JunQuota")), CDbl(Common.WebRequest.Data("JunQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("JunIncentive")), CInt(Common.WebRequest.Data("JunIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("JulQuota")) And IsNumeric(Common.WebRequest.Data("JulIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("7/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("JulQuota")), CDbl(Common.WebRequest.Data("JulQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("JulIncentive")), CInt(Common.WebRequest.Data("JulIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("AugQuota")) And IsNumeric(Common.WebRequest.Data("AugIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("8/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("AugQuota")), CDbl(Common.WebRequest.Data("AugQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("AugIncentive")), CInt(Common.WebRequest.Data("AugIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("SepQuota")) And IsNumeric(Common.WebRequest.Data("SepIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("9/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("SepQuota")), CDbl(Common.WebRequest.Data("SepQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("SepIncentive")), CInt(Common.WebRequest.Data("SepIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("OctQuota")) And IsNumeric(Common.WebRequest.Data("OctIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("10/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("OctQuota")), CDbl(Common.WebRequest.Data("OctQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("OctIncentive")), CInt(Common.WebRequest.Data("OctIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("NovQuota")) And IsNumeric(Common.WebRequest.Data("NovIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("11/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("NovQuota")), CDbl(Common.WebRequest.Data("NovQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("NovIncentive")), CInt(Common.WebRequest.Data("NovIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("DecQuota")) And IsNumeric(Common.WebRequest.Data("DecIncentive")) Then
                p = New InsertUpdate("LibraryOrganizationFOWSTQuota")
                p("FOWSTID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("12/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("DecQuota")), CDbl(Common.WebRequest.Data("DecQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("DecIncentive")), CInt(Common.WebRequest.Data("DecIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            'end of quota

            For Each pl As Object In PriceLevels
                If pl("State") = "add" Then
                    p = New InsertUpdate("MobizLibraryOrganizationFOWSTPriceLevel")

                    p("FOWSTID") = ReadyString(strIDReturn)
                    p("PriceLevelID") = ReadyString(pl("ID").ToString())
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                ElseIf pl("State") = "remove" Then
                    pSql = "DELETE FROM MobizLibraryOrganizationFOWSTPriceLevel WHERE FOWSTID = " & ReadyString(strIDReturn) & vbCrLf &
                            " AND PriceLevelID = " & ReadyString(pl("ID").ToString())
                    Common.WebRequest.ProData.Execute(pSql)
                End If
            Next pl

            For Each emp As Object In RemoveAssignedEmp
                pSql = "DELETE FROM LibraryOrganizationEmployeeFOWST WHERE FOWSTID = " & ReadyString(strIDReturn) & vbCrLf &
                            " AND PeopleID = " & ReadyString(emp("ID").ToString())
                Common.WebRequest.ProData.Execute(pSql)
            Next emp

            pSql = "Update MobizLibraryOrganization Set LastUpdated = GetDate() Where ID = " & If(OrganizationID.Trim().Length > 0, ReadyString(OrganizationID), "NULL")
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevicePrinter'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            AppendToDebug("vbPBN3HYTSUA!", True, ex, True)
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function getOrgRepList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgrep.ID"
                    .Searchable = False
                End With
                With .Add("RepresentativeName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.Title)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.LastName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.MiddleName)) + ' ',' '), '') + " &
                                  "(CASE " &
                                  "     WHEN len(rtrim(ltrim(ISNULL(orgrep.GenerationalSuffix,'')))) > 0 THEN " &
                                  "          ', ' + orgrep.GenerationalSuffix " &
                                  "     ELSE " &
                                  "          '' " &
                                  " END) "
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "ISNULL(orgrep.IsActive,1)"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationRepresentatives orgrep "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgrep.OrganizationID IS NULL", " WHERE orgrep.OrganizationID = " + ReadyString(strID))

            .SqlOrder = "ORDER BY orgrep.IsActive DESC, orgrep.LastName ASC, orgrep.FirstName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgRepContactTelList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrgRepID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrgRepID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "orgcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ExtensionNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationRepresentativesTel orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrgRepID IS NULL", " WHERE orgcon.OrgRepID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgRepContactMobileList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrgRepID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrgRepID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryName", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("TelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "orgcon.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgcon.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationRepresentativesMobile orgcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (orgcon.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrgRepID IS NULL", " WHERE orgcon.OrgRepID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgRepContactEmailList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.ID"
                    .Searchable = False
                End With
                With .Add("OrgRepID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcon.OrgRepID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "orgcon.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "orgcon.EmailType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationRepresentativesEmail orgcon"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcon.OrgRepID IS NULL", " WHERE orgcon.OrgRepID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgRepInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgrep.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgrep.OrganizationID"
                    .Searchable = False
                End With
                With .Add("LastName", SqlDbType.VarChar)
                    .Expression = "orgrep.LastName"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "orgrep.FirstName"
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "orgrep.MiddleName"
                End With
                With .Add("Title", SqlDbType.VarChar)
                    .Expression = "orgrep.Title "
                End With
                With .Add("GenerationalSuffix", SqlDbType.VarChar)
                    .Expression = "orgrep.GenerationalSuffix"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "ISNULL(orgrep.IsActive,0)"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationRepresentatives orgrep "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgrep.ID IS NULL", " WHERE orgrep.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgRep() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationRepresentatives")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String

        Try

            p("OrganizationID") = If(OrganizationID.Trim().Length > 0, ReadyString(OrganizationID), "NULL")
            p("LastName") = ReadyString(LastName)
            p("FirstName") = ReadyString(FirstName)
            p("MiddleName") = ReadyString(MiddleName)
            p("Title") = ReadyString(Title)
            p("GenerationalSuffix") = ReadyString(GenerationalSuffix)
            p("IsActive") = IsActive
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "getdate()"

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Tel List
            For Each objOrgRepTel As Object In OrgRepContactTelList

                If (objOrgRepTel("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgRepTel("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationRepresentativesTel")

                        p("OrgRepID") = ReadyString(strIDReturn)
                        p("CountryID") = IIf(objOrgRepTel("CountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgRepTel("CountryID").ToString().Trim()))
                        p("AreaCodeID") = IIf(objOrgRepTel("AreaCodeID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgRepTel("AreaCodeID").ToString().Trim()))
                        p("ContactNo") = ReadyString(objOrgRepTel("ContactNo").ToString())
                        p("ExtensionNo") = ReadyString(objOrgRepTel("ExtensionNo").ToString())
                        p("ContactType") = ReadyString(objOrgRepTel("ContactType").ToString())

                        If (objOrgRepTel("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgRepTel("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationRepresentativesTel " &
                               " WHERE ID " & IIf(objOrgRepTel("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgRepTel("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgRepTel

            '''Save Mobile List
            For Each objOrgRepMobile As Object In OrgRepContactMobileList

                If (objOrgRepMobile("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgRepMobile("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationRepresentativesMobile")

                        p("OrgRepID") = ReadyString(strIDReturn)
                        p("CountryID") = IIf(objOrgRepMobile("CountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgRepMobile("CountryID").ToString().Trim()))
                        p("TelCoPrefixID") = IIf(objOrgRepMobile("TelCoPrefixID").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgRepMobile("TelCoPrefixID").ToString().Trim()))
                        p("ContactNo") = ReadyString(objOrgRepMobile("ContactNo").ToString())
                        p("ContactType") = ReadyString(objOrgRepMobile("ContactType").ToString())

                        If (objOrgRepMobile("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgRepMobile("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationRepresentativesMobile " &
                               " WHERE ID " & IIf(objOrgRepMobile("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgRepMobile("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgRepMobile

            '''Save Email List
            For Each objOrgRepEmail As Object In OrgRepContactEmailList

                If (objOrgRepEmail("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgRepEmail("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationRepresentativesEmail")

                        p("OrgRepID") = ReadyString(strIDReturn)
                        p("EmailAddress") = ReadyString(objOrgRepEmail("EmailAddress").ToString())
                        p("EmailType") = ReadyString(objOrgRepEmail("EmailType").ToString())

                        If (objOrgRepEmail("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgRepEmail("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationRepresentativesEmail " &
                               " WHERE ID " & IIf(objOrgRepEmail("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgRepEmail("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgRepEmail

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SetInActiveOrgRep() As Boolean
        Dim p As New InsertUpdate("dbo.MobizLibraryOrganizationRepresentatives")
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            p("IsActive") = 0
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function CheckOrganizationEmployee() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgemp.ID"
                End With
                With .Add("PeopleID", SqlDbType.VarChar)
                    .Expression = "orgemp.PeopleID"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationEmployee orgemp "

            .SqlWhere = "WHERE orgemp.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgemp.PeopleID = " & ReadyString(PeopleID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationEmpList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgemp.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.VarChar)
                    .Expression = "orgemp.PeopleID"
                End With
                With .Add("PeopleFirstName", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
                With .Add("DateHired", SqlDbType.Date)
                    .Expression = "orgemp.DateHired"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgemp.DeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgemp.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgemp.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgemp.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgemp.IsBan"
                End With
                With .Add("IsConfirmed", SqlDbType.Bit)
                    .Expression = "ppolorg.IsConfirmed"
                End With
                With .Add("IsTerminated", SqlDbType.Bit)
                    .Expression = "ppolorg.IsTerminated"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "orgemp.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol2.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol2.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationEmployee orgemp " &
                " INNER JOIN MobizLibraryPeople ppol ON (orgemp.PeopleID = ppol.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)" &
                " LEFT JOIN MobizLibraryPeopleOrganization ppolorg ON ppolorg.PeopleID = orgemp.PeopleID AND ppolorg.OrganizationID = " & ReadyString(WebRequest.Instance.Data("OrgID")) &
                " LEFT JOIN MobizLibraryPeople ppol2 ON (ppol2.ID = ppolorg.UserID)"

            .SqlOrder = "ORDER BY orgemp.IsBan ASC, orgemp.DeActivated ASC, (ISNULL(ppol.Surname,'') + ', ' + ISNULL(ppol.FirstName,'') + ' ' + ISNULL(ppol.MiddleName,'')) ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationEmpList(ByVal strSearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgemp.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.VarChar)
                    .Expression = "orgemp.PeopleID"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
                With .Add("DateHired", SqlDbType.Date)
                    .Expression = "orgemp.DateHired"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgemp.DeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgemp.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgemp.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgemp.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgemp.IsBan"
                End With
                With .Add("IsConfirmed", SqlDbType.Bit)
                    .Expression = "ppolorg.IsConfirmed"
                End With
                With .Add("IsTerminated", SqlDbType.Bit)
                    .Expression = "ppolorg.IsTerminated"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationEmployee orgemp " &
                " INNER JOIN MobizLibraryPeople ppol ON (orgemp.PeopleID = ppol.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)" &
                " LEFT JOIN MobizLibraryPeopleOrganization ppolorg ON (ppolorg.PeopleID = ppol.ID  AND ppolorg.OrganizationID = " & ReadyString(WebRequest.Instance.Data("OrgID")) & ")"

            .SqlWhere = "WHERE ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') LIKE " & ReadyStringWild(strSearchKeyWord)

            .SqlOrder = "ORDER BY orgemp.IsBan ASC, orgemp.DeActivated ASC, (ISNULL(ppol.Surname,'') + ', ' + ISNULL(ppol.FirstName,'') + ' ' + ISNULL(ppol.MiddleName,'')) ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationEmpInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgemp.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.VarChar)
                    .Expression = "orgemp.PeopleID"
                End With
                With .Add("SystemGeneratedID", SqlDbType.VarChar)
                    .Expression = "orgemp.SystemGeneratedID"
                End With
                With .Add("CompanyID", SqlDbType.VarChar)
                    .Expression = "orgemp.CompanyID"
                End With
                With .Add("Surname", SqlDbType.VarChar)
                    .Expression = "ppol.Surname"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName"
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "ppol.MiddleName"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
                With .Add("DateHiredMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgemp.DateHired)"
                End With
                With .Add("DateHiredDay", SqlDbType.Int)
                    .Expression = "DAY(orgemp.DateHired)"
                End With
                With .Add("DateHiredYear", SqlDbType.Int)
                    .Expression = "YEAR(orgemp.DateHired)"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgemp.DeActivated"
                End With
                With .Add("DeactivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgemp.DeactivationDate)"
                End With
                With .Add("DeactivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgemp.DeactivationDate)"
                End With
                With .Add("DeactivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgemp.DeactivationDate)"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgemp.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgemp.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgemp.IsBan"
                End With
                With .Add("RoleTagID", SqlDbType.VarChar)
                    .Expression = "orgemp.RoleTagID"
                End With
                With .Add("ThirdPartySystemName", SqlDbType.VarChar)
                    .Expression = "orgemp.ThirdPartySystemName"
                End With
                With .Add("ThirdPartySystemID", SqlDbType.VarChar)
                    .Expression = "orgemp.ThirdPartySystemID"
                End With
                With .Add("PayFrequency", SqlDbType.VarChar)
                    .Expression = "orgemp.PayFrequency"
                End With
                With .Add("DepartmentID", SqlDbType.VarChar)
                    .Expression = "orgemp.DepartmentID"
                End With
                With .Add("Department", SqlDbType.VarChar)
                    .Expression = "dep.TeamName"
                End With
                With .Add("TeamID", SqlDbType.VarChar)
                    .Expression = "orgemp.TeamID"
                End With
                With .Add("Team", SqlDbType.VarChar)
                    .Expression = "team.TeamName"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationEmployee orgemp " &
                " INNER JOIN MobizLibraryPeople ppol ON (orgemp.PeopleID = ppol.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)" &
                " LEFT JOIN LibraryCRMTeams dep on dep.ID = orgemp.DepartmentID " &
                " LEFT JOIN LibraryCRMTeams team on team.ID = orgemp.TeamID "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgemp.ID IS NULL", " WHERE orgemp.ID = " + ReadyString(strID))

            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            If CStr2(pDT.Rows(0).Item("SystemGeneratedID")) = vbNullString Then
                Dim pSql As String
                Dim SystemGeneratedNumber As String
                Dim ID As String

                SystemGeneratedNumber = GenerateSystemGeneratedID(CStr2(pDT.Rows(0).Item("DateHiredYear")))

                pDT.Rows(0).Item("SystemGeneratedID") = SystemGeneratedNumber
                pDT.Rows(0).Item("CompanyID") = SystemGeneratedNumber
            End If
        End If

        Return pDT

    End Function

    Function GenerateSystemGeneratedID(DateHired As String) As String
        Dim YearNow As String
        Dim RandomNumber As Integer
        Dim RandomNumberString As String
        Dim pSql As String
        Dim SystemGeneratedNumber As String

        YearNow = CStr2(Now.Year)
        YearNow = Mid(YearNow, 2, 1)

        DateHired = DateHired
        DateHired = Mid(DateHired, 3, 2)

        While True
            RandomNumber = RandomInteger(0, 99999)

            RandomNumberString = Format(RandomNumber, "00000")

            SystemGeneratedNumber = YearNow & DateHired & RandomNumberString

            pSql = "Select ID from LibraryOrganizationEmployee where SystemGeneratedID = " & ReadyString(SystemGeneratedNumber)
            ID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

            If ID.Length = 0 Then
                Exit While
            End If
        End While

        Return SystemGeneratedNumber

    End Function

    Function GetEmployeeFowst() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            '.Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstName", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(fowst.Code)) + ' - ',' - '), '') + fowst.Name"
                    .Searchable = False
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST AS fowst LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID INNER JOIN " &
                      "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID"

            .SqlWhere = "Where dbo.LibraryOrganizationEmployeeFOWST.PeopleID = " & ReadyString(WebRequest.Instance.Data("EmployeeID"))

            .SqlOrder = " ORDER BY fowst.Name ASC "
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetEmployeeSecurityRole() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "role.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "role.Code"
                End With
                With .Add("Role", SqlDbType.VarChar)
                    .Expression = "role.Role"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "role.Description"
                End With
            End With

            .SqlFrom = "FROM LibrarySecurityRoleUser roleusr " &
                      " INNER JOIN LibrarySecurityRole role ON (roleusr.RoleID = role.ID) "

            .SqlWhere = "Where roleusr.PeopleID = " & ReadyString(WebRequest.Instance.Data("EmployeeID"))

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetEmployeeWorkSchedule() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryDTRSchedule.ID"
                    .Searchable = False
                End With
                .Add("ScheduleName", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM LibraryDTRSchedule " &
                "inner join LibraryDTRScheduleEmployees on LibraryDTRScheduleEmployees.ScheduleID = LibraryDTRSchedule.ID "

            .SqlWhere = "Where LibraryDTRScheduleEmployees.EmployeeID = " & ReadyString(WebRequest.Instance.Data("EmployeeID"))

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetEmployeeIDs() As DataTable
        Dim pSql As String

        pSql = "Select * from MobizLibraryPeopleIDs WHERE PeopleID = " & ReadyString(WebRequest.Instance.Data("EmployeeID"))

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetEmployeeBankAccount() As DataTable
        Dim pSql As String

        pSql = "Select LibraryEmployeesBankAccount.ID, MobizLibraryOrganization.ID BankID, MobizLibraryOrganization.CompanyName BankName " & vbCrLf &
                ", BankBranch " & vbCrLf &
                ",AccountName, AccountNo, BankRole, IsDefaultForThisRole from LibraryEmployeesBankAccount " & vbCrLf &
                "inner join MobizLibraryOrganization on LibraryEmployeesBankAccount.BankID = MobizLibraryOrganization.ID " & vbCrLf &
                "WHERE LibraryEmployeesBankAccount.EmployeeID = " & ReadyString(Common.WebRequest.Data("EmployeeID")) & " AND BankRole='Payroll'"

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetDeminimis(PayType As String) As DataTable
        Dim pSql As String

        pSql = "Select LibraryDeMinimis.ID, AllowanceName, Amount, MaxAmount, MaxTaxExemptAmount, FrequencyOfAllowance, DeminimisFrequency, DisbursementSchedule " & vbCrLf &
              "from LibraryDeMinimis " & vbCrLf &
              "LEFT join LibraryDeminimisEmployees on LibraryDeminimisEmployees.DeminimisID = LibraryDeMinimis.ID " & vbCrLf

        If Not CStr2(WebRequest.Instance.Data("EmployeeID")) = vbNullString Then
            pSql &= " AND LibraryDeminimisEmployees.EmployeeID = " & ReadyString(WebRequest.Instance.Data("EmployeeID")) & vbCrLf
        End If

        pSql &= "WHERE LibraryDeMinimis.SalaryGradeID = " & ReadyString(PayType) & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetHolidays(PayType As String) As DataTable
        Dim pSql As String

        pSql = "Select FROM LibraryPhilippineHolidays"

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetEmployeePayType() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibrarySalaryGrade.ID"
                    .Searchable = False
                End With
                .Add("SalaryGradeName", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM LibrarySalaryGrade " &
                        "inner join LibrarySalaryGradeEmployees on LibrarySalaryGradeEmployees.SalaryGradeID = LibrarySalaryGrade.ID "

            .SqlWhere = "Where LibrarySalaryGradeEmployees.EmployeeID = " & ReadyString(WebRequest.Instance.Data("EmployeeID"))

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveOrganizationEmployee() As String
        Dim p As New InsertUpdate("LibraryOrganizationEmployee")
        Dim pSql As String
        Dim strIDReturn As String

        If SystemGeneratedID = vbNullString Then
            SystemGeneratedID = GenerateSystemGeneratedID(CStr2(CDate(DateHired).Year))
        End If

        If CompanyID = vbNullString Then
            CompanyID = SystemGeneratedID
        End If

        ID = WebRequest.Instance.Data("ID")

        p("PeopleID") = ReadyString(PeopleID)
        p("SystemGeneratedID") = ReadyString(SystemGeneratedID)
        p("CompanyID") = ReadyString(CompanyID)
        p("PeopleID") = ReadyString(PeopleID)
        p("DateHired") = ReadyString(DateHired)
        p("DeActivated") = IsDeactivated
        p("DeactivationDate") = IIf(Convert.ToBoolean(Convert.ToInt32(IsDeactivated)), ReadyString(DeactivationDate), "NULL")
        p("DeactivationReason") = ReadyString(DeactivationReason)
        p("Comments") = ReadyString(Comments)
        p("RoleTagID") = IIf(RoleTag Is Nothing, "NULL", ReadyString(RoleTag))
        p("IsBan") = IsBan
        p("LastUpdated") = "GetDate()"
        p("ThirdPartySystemName") = ReadyString(ThirdPartySystemName)
        p("ThirdPartySystemID") = ReadyString(ThirdPartySystemID)
        p("PayFrequency") = ReadyString(PayFrequency)

        If Not DepartmentID = "" Then
            p("DepartmentID") = ReadyString(DepartmentID)
        End If

        If Not TeamID = "" Then
            p("TeamID") = ReadyString(TeamID)
        End If

        Try
            Common.WebRequest.ProData.BeginTransaction()

            If CInt(WebRequest.Instance.Data("IsNew")) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pSql = "Delete from LibraryOrganizationEmployeeFOWST Where PeopleID = " & ReadyString(PeopleID)
                pSql = pSql & "; Delete from LibrarySecurityRoleUser Where PeopleID = " & ReadyString(PeopleID)
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If Not String.IsNullOrWhiteSpace(PersonFirstName) And Not String.IsNullOrWhiteSpace(PersonSurname) Then
                p = New InsertUpdate("MobizLibraryPeople")

                p("Surname") = ReadyString(PersonSurname)
                p("FirstName") = ReadyString(PersonFirstName)
                p("MiddleName") = ReadyString(PersonMiddleName)

                pSql = p.GetUpdate(" ID = " & ReadyString(PeopleID))
                Common.WebRequest.ProData.Execute(pSql)
            End If

            For Each tel As Object In ObjTelephone
                p = New InsertUpdate("MobizLibraryPeopleContactTel")

                p("PeopleID") = ReadyString(PeopleID)
                p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())

                If tel.ContainsKey("ContactType") Then
                    p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                End If

                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If tel("TelState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf tel("TelState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                ElseIf tel("TelState").ToString() = "Remove" Then
                    pSql = "DELETE FROM MobizLibraryPeopleContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next tel

            For Each mobile As Object In ObjMobile
                p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                p("PeopleID") = ReadyString(PeopleID)
                p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If mobile.ContainsKey("ContactType") Then
                    p("ContactType") = If(mobile("ContactType") = Nothing, ReadyString(""), ReadyString(mobile("ContactType").ToString()))
                End If

                p("LastUpdated") = "getdate()"

                If mobile("MobileState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf mobile("MobileState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                ElseIf mobile("MobileState").ToString() = "Remove" Then
                    pSql = "DELETE FROM MobizLibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next mobile

            For Each email As Object In ObjEmail
                p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                p("PeopleID") = ReadyString(PeopleID)
                p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If email.ContainsKey("EmailType") Then
                    p("EmailType") = If(email("EmailType") = Nothing, ReadyString(""), ReadyString(email("EmailType").ToString()))
                End If

                p("LastUpdated") = "getdate()"

                If email("EmailState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf email("EmailState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                ElseIf email("EmailState").ToString() = "Remove" Then
                    pSql = "DELETE FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next email

            p = New InsertUpdate("MobizLibraryEmployeeOrganizationRelationship")

            p("IsDirectHired") = ReadyString(IsDirectHired)
            p("IsSelfEmployed") = ReadyString(IsSelfEmployed)
            p("EmployeeID") = ReadyString(strIDReturn)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "getdate()"

            If IsSelfEmployed Then
                p("DirectEmployerID") = ReadyString(strIDReturn)
                p("ManpowerAgencyClientID") = "NULL"
                p("AssignedThirdPartyClientID") = "NULL"

                p("DirectEmployerIDDescription") = ReadyString("employerid")
                p("ManpowerAgencyClientIDDescription") = ReadyString("")
                p("AssignedThirdPartyClientIDDescription") = ReadyString("")
            Else
                If CBool2(IsDirectHired) Then
                    p("DirectEmployerID") = ReadyString(Common.WebRequest.Data("OrgID"))
                    p("ManpowerAgencyClientID") = "NULL"
                    p("AssignedThirdPartyClientID") = "NULL"

                    p("DirectEmployerIDDescription") = ReadyString("dbid")
                    p("ManpowerAgencyClientIDDescription") = ReadyString("")
                    p("AssignedThirdPartyClientIDDescription") = ReadyString("")
                Else
                    If CStr2(ManpowerAgencyID) <> "" And CStr2(VendorID) = "" And CStr2(CoUserID) = "" Then
                        p("DirectEmployerID") = ReadyString(ManpowerAgencyID)
                        p("ManpowerAgencyClientID") = ReadyString(Common.WebRequest.Data("OrgID"))
                        p("AssignedThirdPartyClientID") = "NULL"

                        p("DirectEmployerIDDescription") = ReadyString("manpower")
                        p("ManpowerAgencyClientIDDescription") = ReadyString("dbid")
                        p("AssignedThirdPartyClientIDDescription") = ReadyString("")
                    ElseIf CStr2(ManpowerAgencyID) <> "" And CStr2(VendorID) <> "" And CStr2(CoUserID) = "" Then
                        p("DirectEmployerID") = ReadyString(ManpowerAgencyID)
                        p("ManpowerAgencyClientID") = ReadyString(VendorID)
                        p("AssignedThirdPartyClientID") = ReadyString(Common.WebRequest.Data("OrgID"))

                        p("DirectEmployerIDDescription") = ReadyString("manpower")
                        p("ManpowerAgencyClientIDDescription") = ReadyString("vendor")
                        p("AssignedThirdPartyClientIDDescription") = ReadyString("dbid")
                    ElseIf CStr2(ManpowerAgencyID) <> "" And CStr2(VendorID) = "" And CStr2(CoUserID) <> "" Then
                        p("DirectEmployerID") = ReadyString(ManpowerAgencyID)
                        p("ManpowerAgencyClientID") = ReadyString(CoUserID)
                        p("AssignedThirdPartyClientID") = "NULL"

                        p("DirectEmployerIDDescription") = ReadyString("manpower")
                        p("ManpowerAgencyClientIDDescription") = ReadyString("couser")
                        p("AssignedThirdPartyClientIDDescription") = ReadyString("")
                    ElseIf CStr2(ManpowerAgencyID) = "" And CStr2(VendorID) <> "" And CStr2(CoUserID) = "" Then
                        p("DirectEmployerID") = ReadyString(VendorID)
                        p("ManpowerAgencyClientID") = "NULL"
                        p("AssignedThirdPartyClientID") = ReadyString(Common.WebRequest.Data("OrgID"))

                        p("DirectEmployerIDDescription") = ReadyString("vendor")
                        p("ManpowerAgencyClientIDDescription") = ReadyString("")
                        p("AssignedThirdPartyClientIDDescription") = ReadyString("dbid")
                    ElseIf CStr2(ManpowerAgencyID) = "" And CStr2(VendorID) = "" And CStr2(CoUserID) <> "" Then
                        p("DirectEmployerID") = ReadyString(CoUserID)
                        p("ManpowerAgencyClientID") = "NULL"
                        p("AssignedThirdPartyClientID") = "NULL"

                        p("DirectEmployerIDDescription") = ReadyString("couser")
                        p("ManpowerAgencyClientIDDescription") = ReadyString("")
                        p("AssignedThirdPartyClientIDDescription") = ReadyString("")
                    End If
                End If
            End If

            pSql = "SELECT ID FROM MobizLibraryEmployeeOrganizationRelationship WHERE EmployeeID = " & ReadyString(strIDReturn)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" EmployeeID = " & ReadyString(strIDReturn))
            End If

            Common.WebRequest.ProData.Execute(pSql)

            pSql = "DELETE FROM LibraryDeminimisEmployees WHERE EmployeeID = " & ReadyString(PeopleID)
            Common.WebRequest.ProData.Execute(pSql)

            For Each pObj As Object In Common.WebRequest.AllData("Deminimis")

                p = New InsertUpdate("LibraryDeminimisEmployees")

                p("DeminimisID") = ReadyString(pObj("ID"))
                p("EmployeeID") = ReadyString(PeopleID)
                p("Amount") = ReadyString(pObj("Amount"))

                pSql = p.GetInsert()
                Common.WebRequest.ProData.Execute(pSql)

            Next

            For Each pObj As Object In Common.WebRequest.AllData("ValidIDs")
                Dim ValidID As String

                pSql = "Select ID from MobizLibraryPeopleIDs WHERE PeopleID = " & ReadyString(PeopleID) & " AND IDType = " & ReadyString(pObj("IDType"))
                ValidID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                p = New InsertUpdate("MobizLibraryPeopleIDs")

                p("PeopleID") = ReadyString(PeopleID)
                p("IDType") = ReadyString(pObj("IDType"))
                p("IDNo") = ReadyString(pObj("IDNo"))

                If ValidID = vbNullString Then
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(ValidID))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                ValidID = vbNullString
            Next

            If (Not Common.WebRequest.AllData("Fowst") Is Nothing) Then
                For Each pObj As Object In Common.WebRequest.AllData("Fowst")

                    p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                    p("PeopleID") = ReadyString(PeopleID)
                    p("FowstID") = ReadyString(pObj("FowstID"))
                    p("LastUpdated") = "GetDate()"

                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next
            End If

            If (Not Common.WebRequest.AllData("SecurityRole") Is Nothing) Then
                For Each pObj As Object In Common.WebRequest.AllData("SecurityRole")

                    p = New InsertUpdate("LibrarySecurityRoleUser")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = ReadyString(PeopleID)
                    p("RoleID") = ReadyString(pObj("SecRoleID"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = "GetDate()"

                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next
            End If

            p = New InsertUpdate("MobizLibraryPeopleOrganization")

            p("PeopleID") = ReadyString(PeopleID)
            p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
            p("IsTerminated") = IsDeactivated
            p("DateTimeTerminated") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            If (WebRequest.Instance.Data("IsConfirm").Trim() = "1") Then
                p("IsConfirmed") = CInt(WebRequest.Instance.Data("IsConfirm"))
                p("DateTimeConfirmed") = "getDate()"
            End If

            If CInt(WebRequest.Instance.Data("IsNew")) Then
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" PeopleID = " & ReadyString(PeopleID) & " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID))
            End If
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If Not WorkScheduleID = "" Then

                pSql = "DELETE FROM LibraryDTRScheduleEmployees WHERE EmployeeID = " & ReadyString(PeopleID)
                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("LibraryDTRScheduleEmployees")
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("EmployeeID") = ReadyString(PeopleID)
                p("ScheduleID") = ReadyString(WorkScheduleID)

                pSql = p.GetInsert()
                Common.WebRequest.ProData.Execute(pSql)

            End If

            If Not PayTypeID = "" Then

                pSql = "Delete from LibrarySalaryGradeEmployees WHERE EmployeeID = " & ReadyString(PeopleID)
                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("LibrarySalaryGradeEmployees")
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("EmployeeID") = ReadyString(PeopleID)
                p("SalaryGradeID") = ReadyString(PayTypeID)

                pSql = p.GetInsert()
                Common.WebRequest.ProData.Execute(pSql)

            End If

            If Not BankID = vbNullString Then

                Dim EmpBankID As String = vbNullString
                Dim PayeeBankID As String = vbNullString

                pSql = "Select ID from LibraryEmployeesBankAccount WHERE EmployeeID = " & ReadyString(PeopleID) & " AND BankRole = 'Payroll'"
                EmpBankID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                pSql = "Select BankID from LibraryOrganizationBankAccount WHERE BankRole = 'Payroll' AND IsDefaultForThisRole=1"
                PayeeBankID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                p = New InsertUpdate("LibraryEmployeesBankAccount")

                p("BankID") = ReadyString(BankID)
                p("EmployeeID") = ReadyString(PeopleID)
                p("BankBranch") = ReadyString(BankBranch)
                p("AccountName") = ReadyString(AccountName)
                p("AccountNo") = ReadyString(AccountNo)
                p("BankRole") = ReadyString(BankRole)
                p("IsDefaultForThisRole") = CInt2(IsDefaultBank)

                If Not PayeeBankID = vbNullString Then
                    p("PayeeBankID") = ReadyString(PayeeBankID)
                End If

                If EmpBankID = vbNullString Then
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate("ID=" & ReadyString(EmpBankID))
                End If

                Common.WebRequest.ProData.Execute(pSql)

            End If
            If Not String.IsNullOrWhiteSpace(Username) And Not String.IsNullOrWhiteSpace(Userpassword) Then
                p = New InsertUpdate("MobizLibraryPeopleUserAccount")
                p("Username") = ReadyString(Username)
                p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(Userpassword))
                p("LastUpdated") = "getdate()"
                p("ApprovedBy") = IIf(String.IsNullOrWhiteSpace(ApprovedResetMobizAccBy), "NULL", ReadyString(ApprovedResetMobizAccBy))
                Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(PeopleID)))
            End If
            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationEmployee'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationEmployeeFOWST'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibrarySecurityRoleUser'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction(, True)
            AppendToDebug("vbKWECHY6NBM! " & ex.Message, True, ex, True)
            Return "Error: " & ex.Message
        End Try

    End Function

    Function CheckOrganizationCustomer() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcust.ID"
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcust.OrganizationID"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomer orgcust "

            'edited by arnie june 23, 2015 1:30pm
            '" AND orgcust.OrganizationID = " & ReadyString(OrganizationID) -> ORIGINAL
            .SqlWhere = "WHERE orgcust.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgcust.OrganizationID = " & ReadyString(CustomerID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcust.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.VarChar)
                    .Expression = "orgcust.OrganizationID"
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = " (CASE cust.IsOrganization " &
                                  "     WHEN 1 THEN  cust.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgcust.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgcust.DeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgcust.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgcust.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgcust.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgcust.IsBan"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "orgcust.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomer orgcust " &
                " INNER JOIN MobizLibraryOrganization cust ON (orgcust.OrganizationID = cust.ID) " &
                " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = cust.UserID)"

            .SqlOrder = "ORDER BY orgcust.IsBan ASC, orgcust.DeActivated ASC, cust.CompanyName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustList(ByVal strSearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcust.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.VarChar)
                    .Expression = "orgcust.OrganizationID"
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = " (CASE cust.IsOrganization " &
                                  "     WHEN 1 THEN  cust.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgcust.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgcust.DeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgcust.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgcust.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgcust.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgcust.IsBan"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomer orgcust " &
                " INNER JOIN MobizLibraryOrganization cust ON (orgcust.OrganizationID = cust.ID) "

            .SqlWhere = "WHERE (CASE cust.IsOrganization " &
                        "       WHEN 1 THEN  cust.CompanyName " &
                        "       ELSE  " &
                        "           ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                        "           ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                        "           ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                        "           ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                        "END) LIKE " & ReadyStringWild(strSearchKeyWord)

            .SqlOrder = "ORDER BY orgcust.IsBan ASC, orgcust.DeActivated ASC, cust.CompanyName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcust.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.VarChar)
                    .Expression = "orgcust.OrganizationID"
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = " (CASE cust.IsOrganization " &
                                  "     WHEN 1 THEN  cust.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("ActivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgcust.ActivationDate)"
                End With
                With .Add("ActivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgcust.ActivationDate)"
                End With
                With .Add("ActivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgcust.ActivationDate)"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgcust.DeActivated"
                End With
                With .Add("DeactivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgcust.DeactivationDate)"
                End With
                With .Add("DeactivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgcust.DeactivationDate)"
                End With
                With .Add("DeactivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgcust.DeactivationDate)"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgcust.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgcust.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgcust.IsBan"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomer orgcust " &
                " INNER JOIN MobizLibraryOrganization cust ON (orgcust.OrganizationID = cust.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgcust.ID IS NULL", " WHERE orgcust.ID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveOrganizationCustomer() As String
        Dim p As New InsertUpdate("LibraryOrganizationCustomer")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(CustomerID)
        p("ActivationDate") = ReadyString(ActivationDate)
        p("DeActivated") = IsDeactivated
        p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
        p("DeactivationReason") = ReadyString(DeactivationReason)
        p("Comments") = ReadyString(Comments)
        p("IsBan") = IsBan
        p("LastUpdated") = "GetDate()"

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Default Price Level (Wholesale, SRP and Retail Only) 05/22/2013 Temporarily Advice by Sir Robert

            '''Insert Retail
            p = New InsertUpdate("LibraryOrganizationPriceLevel")
            p("OrganizationID") = ReadyString(CustomerID)
            p("PriceLevelID") = ReadyString(WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID())
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Insert SRP
            p = New InsertUpdate("LibraryOrganizationPriceLevel")
            p("OrganizationID") = ReadyString(CustomerID)
            p("PriceLevelID") = ReadyString("EEF716C6-B290-4B28-B2A6-0A4F52CF4989")
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Insert Wholesale
            p = New InsertUpdate("LibraryOrganizationPriceLevel")
            p("OrganizationID") = ReadyString(CustomerID)
            p("PriceLevelID") = ReadyString(WebRequest.Instance.Business.SystemDefaults.GetWholesalePriceID())
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        End If



        'Try
        '    Common.WebRequest.ProData.BeginTransaction()

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationCustomer'"
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
        'Common.WebRequest.ProData.ExecuteSQL(pSql)

        '    For Each pObj As Object In Common.WebRequest.AllData("Products")
        '        p = New InsertUpdate("LibraryOrganizationCustomerPIM")

        '        p("CustomerID") = ReadyString(strIDReturn)
        '        p("ProductID") = ReadyString(pObj("ProductID"))
        '        'p("Bin") = Quantity
        '        'p("MaxCap") = Quantity * pObj("ActualQuantity")
        '        p("MinCap") = ReadyString(pObj("UnitMeasureID"))
        '        p("CustomerBarCode") = ReadyString(pObj("UnitMeasureID"))
        '        p("SellingPrice") = ReadyString(pObj("UnitMeasureID"))

        '        Common.WebRequest.ProData.ExecuteSQL(pSql)
        '    Next

        Return strIDReturn
        'Catch ex As Exception
        '    Common.WebRequest.ProData.RollbackTransaction()
        '    Return ex.Message.ToString
        'End Try
    End Function

    Function CheckOrganizationSupplier() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgsup.ID"
                End With
                With .Add("SupplierID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgsup.SupplierID"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier orgsup "

            .SqlWhere = "WHERE orgsup.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgsup.SupplierID = " & ReadyString(SupplierID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationSupplierList(ByVal SearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgsup.ID"
                    .Searchable = False
                End With
                With .Add("SupplierID", SqlDbType.VarChar)
                    .Expression = "orgsup.SupplierID"
                End With
                With .Add("SupplierName", SqlDbType.VarChar)
                    .Expression = " (CASE sup.IsOrganization " &
                                  "     WHEN 1 THEN  sup.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(sup.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgsup.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgsup.DeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgsup.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgsup.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgsup.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgsup.IsBan"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "orgsup.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier orgsup " &
                " INNER JOIN MobizLibraryOrganization sup ON (orgsup.SupplierID = sup.ID) " &
                " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = sup.UserID)"

            .SqlWhere = " WHERE sup.CompanyName LIKE '%" & SearchKeyWord & "%'"
            .SqlOrder = "ORDER BY orgsup.IsBan ASC, orgsup.DeActivated ASC, sup.CompanyName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationSupplierInfo(
        Optional ByVal strID As String = Nothing,
        Optional vOrgID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgsup.ID"
                    .Searchable = False
                End With
                With .Add("SupplierID", SqlDbType.VarChar)
                    .Expression = "orgsup.SupplierID"
                End With
                With .Add("SupplierName", SqlDbType.VarChar)
                    .Expression = " (CASE sup.IsOrganization " &
                                  "     WHEN 1 THEN  sup.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(sup.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgsup.ActivationDate"
                End With
                With .Add("ActivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgsup.ActivationDate)"
                End With
                With .Add("ActivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgsup.ActivationDate)"
                End With
                With .Add("ActivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgsup.ActivationDate)"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgsup.DeActivated"
                End With
                With .Add("DeactivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgsup.DeactivationDate)"
                End With
                With .Add("DeactivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgsup.DeactivationDate)"
                End With
                With .Add("DeactivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgsup.DeactivationDate)"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgsup.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgsup.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgsup.IsBan"
                End With
                With .Add("CreditTermsID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgsup.CreditTermsID"
                End With
                With .Add("CreditTermsDays", SqlDbType.Int)
                    .Expression = "LibraryPaymentTerms.Days"
                End With
                With .Add("CreditTermsName", SqlDbType.VarChar)
                    .Expression = "LibraryPaymentTerms.Name"
                End With
                With .Add("CreditLimit", SqlDbType.Money)
                    .Expression = "orgsup.CreditLimit"
                End With
                With .Add("CreditBegBalance", SqlDbType.Money)
                    .Expression = "orgsup.CreditBegBalance"
                End With
                With .Add("CreditBalanceAsOf", SqlDbType.Date)
                    .Expression = "orgsup.CreditBalanceAsOf"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier orgsup " & vbCrLf &
                " INNER JOIN MobizLibraryOrganization sup ON (orgsup.SupplierID = sup.ID) " & vbCrLf &
                " LEFT JOIN LibraryPaymentTerms  ON LibraryPaymentTerms.ID = orgsup.CreditTermsID" & vbCrLf
            If String.IsNullOrWhiteSpace(strID) = False Then
                .SqlWhere = " WHERE orgsup.ID = " + ReadyString(strID)
            ElseIf String.IsNullOrWhiteSpace(vOrgID) = False Then
                .SqlWhere = " WHERE sup.ID = " + ReadyString(vOrgID)
            End If

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveOrganizationSupplier() As String
        Dim p As New InsertUpdate("LibraryOrganizationSupplier")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pRecordCount As Integer
        Dim codenumber As String = CStr2(New OrganizationShort().GenerateCodeNumber(False))

        p("SupplierID") = ReadyString(SupplierID)
        p("ActivationDate") = ReadyString(ActivationDate)
        p("DeActivated") = IsDeactivated
        p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
        p("DeactivationReason") = ReadyString(DeactivationReason)
        p("Comments") = ReadyString(Comments)
        p("IsBan") = IsBan
        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        p("LastUpdated") = "getdate()"

        If CStr2(VendorCode) = "" Then
            p("VendorCode") = ReadyString(codenumber)
        End If

        If CStr2(SupplierType) <> "" Then 'for dtr
            p("TaggedAs") = ReadyString(SupplierType)
        End If

        If String.IsNullOrWhiteSpace(CreditTermsID) Then
            p("CreditTermsID") = "NULL"
        Else
            p("CreditTermsID") = ReadyString(CreditTermsID)
        End If
        If String.IsNullOrWhiteSpace(CreditLimit) Then
            p("CreditLimit") = "NULL"
        Else
            p("CreditLimit") = CDec(CreditLimit)
        End If
        If String.IsNullOrWhiteSpace(CreditBegBalance) Then
            p("CreditBegBalance") = "NULL"
        Else
            p("CreditBegBalance") = CDec(CreditBegBalance)
        End If

        If String.IsNullOrWhiteSpace(CreditBalanceAsOf) Then
            p("CreditBalanceAsOf") = "NULL"
        Else
            p("CreditBalanceAsOf") = ReadyString(Format(ParseDate(CreditBalanceAsOf), cGlobal.FormatDateTimeSql2))
        End If

        If String.IsNullOrWhiteSpace(ThirdPartyPlatform) Then
            p("ThirdPartyPlatform") = "NULL"
        Else
            p("ThirdPartyPlatform") = ReadyString(ThirdPartyPlatform)
        End If

        If String.IsNullOrWhiteSpace(ThirdPartyID) Then
            p("ThirdPartyID") = "NULL"
        Else
            p("ThirdPartyID") = ReadyString(ThirdPartyID)
        End If

        pSql = p.GetUpdate("SupplierID = " & ReadyString(SupplierID))
        pRecordCount = Common.WebRequest.ProData.ExecuteSQL(pSql)
        If pRecordCount = 0 Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Else
            strIDReturn = ID
        End If

        WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryOrganizationSupplier")

        Return strIDReturn

    End Function

    Function CheckOrganizationBrand() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgbrand.ID"
                End With
                With .Add("BrandName", SqlDbType.VarChar)
                    .Expression = "orgbrand.BrandName"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationBrand orgbrand "

            .SqlWhere = "WHERE orgbrand.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgbrand.BrandName = " & ReadyString(BrandName)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationBrandList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgbrand.ID"
                    .Searchable = False
                End With
                With .Add("BrandName", SqlDbType.VarChar)
                    .Expression = "orgbrand.BrandName"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationBrand orgbrand "

            .SqlWhere = "WHERE orgbrand.OrganizationID " & IIf(WebSession.Instance.OrganizationID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(WebSession.Instance.OrganizationID))

            .SqlOrder = "ORDER BY orgbrand.BrandName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationBrandInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgbrand.ID"
                    .Searchable = False
                End With
                With .Add("BrandName", SqlDbType.VarChar)
                    .Expression = "orgbrand.BrandName"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationBrand orgbrand "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgbrand.ID IS NULL", " WHERE orgbrand.ID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveOrganizationBrand() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationBrand")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
        p("BrandName") = ReadyString(BrandName)

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function CheckOrganizationProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ProductID"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationPIM orgprd "

            .SqlWhere = "WHERE orgprd.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgprd.ProductID = " & ReadyString(ProductID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgCompanyWideProductCount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ProductCount", SqlDbType.UniqueIdentifier)
                    .Expression = "COUNT(orgprd.ID)"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationPIM orgprd "

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationProductList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgprd.ProductID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("IsDoNotShareTransDataToBrandOwner", SqlDbType.Bit)
                    .Expression = "orgprd.IsDoNotShareTransDataToBrandOwner"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgprd.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgprd.IsDeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgprd.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgprd.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgprd.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgprd.IsBan"
                End With
                With .Add("Brand", SqlDbType.VarChar)
                    .Expression = "prd.Brand"
                End With
                With .Add("RetailPrice", SqlDbType.Int)
                    .Expression = "prd.SRP"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationPIM orgprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) "

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getCompanyProductList(ByVal strSearchKeyWord As String, Optional vStartingRowNumber As Integer = 0, Optional vRowsCount As Integer = Integer.MaxValue, Optional vendorid As String = Nothing,
        Optional vIncludeActive As Boolean = True, Optional vIncludeInactive As Boolean = False) As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim pEndingRowNumber = vStartingRowNumber + vRowsCount - 1

        pSql = "SELECT DISTINCT " & vbCrLf &
        "pim.ProductNo, " & vbCrLf &
        "pim.ProductName, " & vbCrLf &
        "pim.Brand, " & vbCrLf &
        "prd001.c0020 Category" & vbCrLf &
        ",'' UPC, " & vbCrLf

        If Not vendorid Is Nothing Then
            pSql &= " LibraryVendorPrices.Price as CostFromVendor," & vbCrLf
        Else
            pSql &= "ISNULL((select top 1 prodPriceLvlVendor.Price  " & vbCrLf &
            "from MobizLibraryPIMProductPriceLevel prodPriceLvlVendor   " & vbCrLf &
            "where prodPriceLvlVendor.PriceLevelID = '01000012-0002-0000-0000-000000000000'  " & vbCrLf &
            "AND (prodPriceLvlVendor.ProductID = orgpim.ProductID)),0) AS CostFromVendor,  " & vbCrLf
        End If

        pSql &= "ISNULL((select top 1 prodPriceLvlVendor.Price  " & vbCrLf &
        "from MobizLibraryPIMProductPriceLevel prodPriceLvlVendor   " & vbCrLf &
        "where prodPriceLvlVendor.PriceLevelID = '01000012-0001-0000-0000-000000000000'  " & vbCrLf &
        "AND (prodPriceLvlVendor.ProductID = orgpim.ProductID)),0) AS ShippingCost,  " & vbCrLf &
        "ISNULL((select top 1 prodPriceLvlVendor.Price " & vbCrLf &
        "from MobizLibraryPIMProductPriceLevel prodPriceLvlVendor   " & vbCrLf &
        "where prodPriceLvlVendor.PriceLevelID = '01000012-0003-0000-0000-000000000000'  " & vbCrLf &
        "AND (prodPriceLvlVendor.ProductID = orgpim.ProductID)),0) AS LandedCost,  " & vbCrLf &
        "ISNULL((select top 1 prodPriceLvlVendor.Price  " & vbCrLf &
        "from MobizLibraryPIMProductPriceLevel prodPriceLvlVendor   " & vbCrLf &
        "where prodPriceLvlVendor.PriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989'  " & vbCrLf &
        "AND (prodPriceLvlVendor.ProductID = orgpim.ProductID)),0) AS SRP,  " & vbCrLf &
        "ISNULL((select top 1 prodPriceLvlVendor.Price  " & vbCrLf &
        "from MobizLibraryPIMProductPriceLevel prodPriceLvlVendor   " & vbCrLf &
        "where prodPriceLvlVendor.PriceLevelID = '01000012-0005-0000-0000-000000000000'  " & vbCrLf &
        "AND (prodPriceLvlVendor.ProductID = orgpim.ProductID)),0) AS RetailPrice, " & vbCrLf &
        "(select top 1 meas.Quantity from MobizLibraryPIMProductMeasurement meas   " & vbCrLf &
        "LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)  " & vbCrLf &
        "LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)  " & vbCrLf &
        "WHERE meas.ProductID = pim.ID order by meas.Quantity) AS PackingSizeQty,  " & vbCrLf &
        "(select top 1 lbl.label from MobizLibraryPIMProductMeasurement meas   " & vbCrLf &
        "LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)  " & vbCrLf &
        "LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)  " & vbCrLf &
        "WHERE meas.ProductID = pim.ID order by meas.Quantity) AS PackingSizeUnit " & vbCrLf &
        ",CASE WHEN ISNULL(pim.IsAsset, 0) = 0 THEN 'No' ELSE 'Yes' END IsAsset" & vbCrLf &
        ",ROW_NUMBER() OVER (ORDER BY orgpim.ID) AS rn" & vbCrLf &
        ",upprc.Price as UpcomingPrice" & vbCrLf &
        ",upprc.EffectivityDate as UpcomingPriceEffectivityDate " & vbCrLf &
        ",~ISNULL(orgpim.IsDeactivated, 0) AS Active" & vbCrLf &
        " FROM MobizLibraryPIMProduct pim  " & vbCrLf &
        " LEFT JOIN LibraryOrganizationPIM orgpim ON pim.ID = orgpim.ProductID" & vbCrLf &
        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (orgpim.ProductID = prd001.ProductID)  " & vbCrLf &
        " LEFT JOIN LibraryPimUpcomingPrices upprc ON (upprc.PriceLevelID = '01000012-0005-0000-0000-000000000000' AND upprc.ProductID  = pim.ID) " & vbCrLf

        If Not vendorid Is Nothing Then
            pSql &= " JOIN LibraryVendorPrices ON LibraryVendorPrices.ProductID = orgpim.ProductID AND LibraryVendorPrices.VendorID = " & ReadyString(vendorid)
        End If

        pSql &= " WHERE (1=0" & vbCrLf
        If vIncludeActive Then
            pSql &= " OR ISNULL(orgpim.IsDeactivated, 0) = 0" & vbCrLf
        End If
        If vIncludeInactive Then
            pSql &= " or ISNULL(orgpim.IsDeactivated, 0) = 1" & vbCrLf
        End If
        pSql &= ")" & vbCrLf

        pSql = "SELECT * FROM (" & pSql & ") t" & vbCrLf &
            " WHERE rn BETWEEN " & vStartingRowNumber & " AND " & pEndingRowNumber & vbCrLf &
            " ORDER BY Brand ASC, ProductName ASC"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If Now.Date = #2024/4/10# Then
            AppendToDebug("getCompanyProductList " & pSql, True)
        End If

        Return pDT

    End Function

    Function getOrganizationProductList(ByVal strSearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow
            .SearchString = strSearchKeyWord

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgprd.ProductID"
                    .Searchable = False
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("IsDoNotShareTransDataToBrandOwner", SqlDbType.Bit)
                    .Expression = "orgprd.IsDoNotShareTransDataToBrandOwner"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgprd.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgprd.IsDeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgprd.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgprd.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgprd.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgprd.IsBan"
                End With
                With .Add("IsBrandOwner", SqlDbType.Bit)
                    .Expression = "prd.IsBrandOwner"
                End With
                With .Add("IsPriceCount", SqlDbType.Int)
                    .Expression = "(SELECT COUNT(*) FROM MobizLibraryPIMProductPriceLevel WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) & " AND ProductID = orgprd.ProductID AND PriceLevelID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000'))"
                End With
                With .Add("Brand", SqlDbType.VarChar)
                    .Expression = "prd.Brand"
                End With
                With .Add("RetailPrice", SqlDbType.Int)
                    .Expression = "prd.SRP"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationPIM orgprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) "

            ' commented by Arvin. Jan 10, 2015
            ' uncommented by Arnie. April 7, 2015
            ' commented again by Arnie June 29, 2016
            '.SqlWhere = "WHERE prd.ProductSearchString LIKE " & ReadyStringWild(strSearchKeyWord) &
            '            " OR prd.ProductName LIKE " & ReadyStringWild(strSearchKeyWord) &
            '            " OR prd.ProductNo LIKE " & ReadyStringWild(strSearchKeyWord)

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Public Function GetOrganizationProductPriceBreakList(vProductID As String, Optional vID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationPIMPriceBreak.ID"
                    .Searchable = False
                End With
                .Add("UnitMeasure", SqlDbType.VarChar, "MobizLibraryCountriesLanguageLabel.Label")
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationPIMPriceBreak.UnitMeasureID"
                    .Searchable = False
                End With
                .Add("Qty", SqlDbType.Decimal, "LibraryOrganizationPIMPriceBreak.Qty")
                .Add("Discount", SqlDbType.Decimal, "LibraryOrganizationPIMPriceBreak.Discount")
                .Add("DiscountType", SqlDbType.TinyInt, "ISNULL(LibraryOrganizationPIMPriceBreak.DiscountType, 0)")
                .Add("UnitQuantity", SqlDbType.Decimal, "MobizLibraryPIMProductMeasurement.Quantity")
            End With
            .SqlFrom = " FROM LibraryOrganizationPIMPriceBreak" & vbCrLf &
                " JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = LibraryOrganizationPIMPriceBreak.UnitMeasureID" & vbCrLf &
                " JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                " JOIN MobizLibraryPIMProductMeasurement " & vbCrLf &
                "  	ON MobizLibraryPIMProductMeasurement.ProductID = LibraryOrganizationPIMPriceBreak.ProductID " & vbCrLf &
                "     AND MobizLibraryPIMProductMeasurement.UnitMeasureID = LibraryOrganizationPIMPriceBreak.UnitMeasureID"
            .SqlWhere = " WHERE LibraryOrganizationPIMPriceBreak.ProductID = " & ReadyString(vProductID)
            If vID IsNot Nothing Then
                .SqlWhere &= " AND LibraryOrganizationPIMPriceBreak.ID = " & ReadyString(vID)
            End If
            .SqlOrder = " ORDER BY UnitQuantity, Qty"
        End With
        pDT = pFinder.Requery

        Return pDT

    End Function

    Function getOrganizationProductInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                End With
                With .Add("Brand", SqlDbType.VarChar)
                    .Expression = "prd.Brand"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ModelNo", SqlDbType.VarChar)
                    .Expression = "prd.ModelNo"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductGroupNo"
                End With
                With .Add("DivisionDescription", SqlDbType.VarChar)
                    .Expression = "prd.DivisionDescription"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "prdfea1.C0020"
                End With
                With .Add("MarketingText", SqlDbType.VarChar)
                    .Expression = "prd.MarketingText"
                End With
                With .Add("Warranty", SqlDbType.VarChar)
                    .Expression = "prd.Warranty"
                End With
                With .Add("PublishedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.PublishedByID"
                End With
                With .Add("PublishedBy", SqlDbType.VarChar)
                    .Expression = "orgpub.CompanyName"
                End With
                With .Add("UploadedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.UploadedByID"
                End With
                With .Add("UploadedBy", SqlDbType.VarChar)
                    .Expression = "orgupl.CompanyName"
                End With
                With .Add("IsDoNotShareTransDataToBrandOwner", SqlDbType.Bit)
                    .Expression = "orgprd.IsDoNotShareTransDataToBrandOwner"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgprd.ActivationDate"
                End With
                With .Add("ActivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(ISNULL(orgprd.ActivationDate,getdate()))"
                End With
                With .Add("ActivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(ISNULL(orgprd.ActivationDate,getdate()))"
                End With
                With .Add("ActivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(ISNULL(orgprd.ActivationDate,getdate()))"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgprd.IsDeActivated"
                End With
                With .Add("DeactivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(ISNULL(orgprd.DeactivationDate,getdate()))"
                End With
                With .Add("DeactivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(ISNULL(orgprd.DeactivationDate,getdate()))"
                End With
                With .Add("DeactivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(ISNULL(orgprd.DeactivationDate,getdate()))"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgprd.Comments"
                End With
                With .Add("IsVatExempt", SqlDbType.Bit)
                    .Expression = "orgprd.IsVatExempt"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgprd.IsBan"
                End With
                With .Add("ProductAvailableToMobile", SqlDbType.Bit)
                    .Expression = "orgprd.ProductAvailableToMobile"
                End With
                With .Add("MaxAllowableDiscountableAmount", SqlDbType.Decimal)
                    .Expression = "orgprd.MaxAllowableDiscountableAmount"
                End With
                With .Add("DiscountedPriceAction", SqlDbType.Decimal)
                    .Expression = "orgprd.DiscountedPriceAction"
                End With
                With .Add("IsInventoryCountItem", SqlDbType.Decimal)
                    .Expression = "orgprd.IsInventoryCountItem"
                End With
                With .Add("IsPromoItem", SqlDbType.Decimal)
                    .Expression = "orgprd.IsPromoItem"
                End With
                With .Add("IsAsset", SqlDbType.Bit)
                    .Expression = "prd.IsAsset"
                End With
                With .Add("IsEcommerceVendorAllowedToSell", SqlDbType.Bit)
                    .Expression = "ISNULL(prd.IsEcommerceVendorAllowedToSell, 1)"
                End With
                .Add("PaymentTermID", SqlDbType.UniqueIdentifier, "orgprd.PaymentTermID")
                .Add("PaymentTerm", SqlDbType.VarChar, "LibraryPaymentTerms.Name")
                With .Add("MaxAllowableDiscountReferenceSKUID", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(orgprd.ItemBasedMaxAllowableDiscount, NULL)"
                End With
                With .Add("MaxAllowableDiscountReferenceSKUName", SqlDbType.VarChar)
                    .Expression = "ISNULL(itembasedmaxdis.ProductNo + ' - ' + itembasedmaxdis.ProductName, '')"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProduct prd " &
                " LEFT JOIN LibraryOrganizationPIM orgprd  ON (orgprd.ProductID = prd.ID) " &
                " LEFT JOIN MobizLibraryPIMProduct itembasedmaxdis  ON (orgprd.ItemBasedMaxAllowableDiscount = itembasedmaxdis.ID) " &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prdfea1 ON (prdfea1.ProductID = prd.ID) " &
                " LEFT JOIN MobizLibraryOrganization orgpub ON (prd.PublishedByID = orgpub.ID) " &
                " LEFT JOIN MobizLibraryOrganization orgupl ON (prd.UploadedByID = orgupl.ID) " & vbCrLf &
                " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = orgprd.PaymentTermID" & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prd.ID IS NULL", " WHERE prd.ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationProductMeasurementList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.ID"
                End With
                With .Add("MeasurementLevel", SqlDbType.Int)
                    .Expression = "prdmea.MeasurementLevel"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.UnitMeasureID"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "mealbl.Label"
                End With
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "prdmea.Quantity"
                End With
                With .Add("Discount", SqlDbType.Decimal)
                    .Expression = "prdmea.Discount"
                End With
                With .Add("ContentQty", SqlDbType.Decimal)
                    .Expression = "prdmea.ContentQty"
                End With
                With .Add("ContentUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ContentUnit"
                End With
                With .Add("ProdDimensionLength", SqlDbType.Decimal)
                    .Expression = "prdmea.ProdDimensionLength"
                End With
                With .Add("ProdDimensionLengthUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProdDimensionLengthUnit"
                End With
                With .Add("ProdDimensionWidth", SqlDbType.Decimal)
                    .Expression = "prdmea.ProdDimensionWidth"
                End With
                With .Add("ProdDimensionWidthUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProdDimensionWidthUnit"
                End With
                With .Add("ProdDimensionHeight", SqlDbType.Decimal)
                    .Expression = "prdmea.ProdDimensionHeight"
                End With
                With .Add("ProdDimensionHeightUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProdDimensionHeightUnit"
                End With
                With .Add("ProductWeight", SqlDbType.Decimal)
                    .Expression = "prdmea.ProductWeight"
                End With
                With .Add("ProductWeightUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProductWeightUnit"
                End With
                With .Add("BaseUnit", SqlDbType.VarChar)
                    .Expression = "tblBaseUnit.UnitMeasure"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductMeasurement prdmea " &
                " INNER JOIN MobizLibraryPIMMeasure mea ON (prdmea.UnitMeasureID = mea.ID) " &
                " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) " & vbCrLf &
                " CROSS APPLY (" & vbCrLf &
                " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "     WHERE MobizLibraryPIMProductMeasurement.ProductID = prdmea.ProductID" & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                " ) tblBaseUnit" & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prdmea.ProductID IS NULL", " WHERE prdmea.ProductID = " & ReadyString(strID))

            .SqlOrder = "ORDER BY prdmea.Quantity, prdmea.MeasurementLevel ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationProductMeasurementInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.ID"
                End With
                With .Add("MeasurementLevel", SqlDbType.Int)
                    .Expression = "prdmea.MeasurementLevel"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.UnitMeasureID"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "mealbl.Label"
                End With
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "prdmea.Quantity"
                End With
                With .Add("Discount", SqlDbType.Decimal)
                    .Expression = "prdmea.Discount"
                End With
                With .Add("ContentQty", SqlDbType.Decimal)
                    .Expression = "prdmea.ContentQty"
                End With
                With .Add("ContentUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ContentUnit"
                End With
                With .Add("ProdDimensionLength", SqlDbType.Decimal)
                    .Expression = "prdmea.ProdDimensionLength"
                End With
                With .Add("ProdDimensionLengthUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProdDimensionLengthUnit"
                End With
                With .Add("ProdDimensionWidth", SqlDbType.Decimal)
                    .Expression = "prdmea.ProdDimensionWidth"
                End With
                With .Add("ProdDimensionWidthUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProdDimensionWidthUnit"
                End With
                With .Add("ProdDimensionHeight", SqlDbType.Decimal)
                    .Expression = "prdmea.ProdDimensionHeight"
                End With
                With .Add("ProdDimensionHeightUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProdDimensionHeightUnit"
                End With
                With .Add("ProductWeight", SqlDbType.Decimal)
                    .Expression = "prdmea.ProductWeight"
                End With
                With .Add("ProductWeightUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ProductWeightUnit"
                End With
                With .Add("IsTimeBaseUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.IsTimeBaseUnit"
                End With
                With .Add("TimeBaseUnitDescription", SqlDbType.VarChar)
                    .Expression = "prdmea.TimeBaseUnitDescription"
                End With
                With .Add("NoOfDaysInTimeUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.NoOfDaysInTimeUnit"
                End With
                With .Add("BaseUnit", SqlDbType.VarChar)
                    .Expression = "tblBaseUnit.UnitMeasure"
                End With
                With .Add("BaseUnitQty", SqlDbType.Decimal)
                    .Expression = "tblBaseUnit.UnitQty"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductMeasurement prdmea " &
                " INNER JOIN MobizLibraryPIMMeasure mea ON (prdmea.UnitMeasureID = mea.ID) " &
                " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) " & vbCrLf &
                " CROSS APPLY (" & vbCrLf &
                " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "     WHERE MobizLibraryPIMProductMeasurement.ProductID = prdmea.ProductID" & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                " ) tblBaseUnit" & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prdmea.ID IS NULL", " WHERE prdmea.ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationProductMeasurementBarcodeList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmeabr.ID"
                    .Searchable = False
                End With
                With .Add("StockMeasurementsID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmeabr.StockMeasurementsID"
                End With
                With .Add("Barcode", SqlDbType.VarChar)
                    .Expression = "prdmeabr.Barcode"
                End With
                With .Add("BarcodeType", SqlDbType.VarChar)
                    .Expression = "CASE WHEN prdmeabr.BarcodeTypeID = 'f848c611-a271-4aea-94a3-1a5164333dd4' THEN 'UPC' " & vbCrLf &
                                " WHEN prdmeabr.BarcodeTypeID = '25332b69-ab93-4742-b924-f5c05a7b26b7' THEN 'EAN' " & vbCrLf &
                                " WHEN prdmeabr.BarcodeTypeID = 'dad61485-803e-4f5f-ad6e-7a91952a94f0' THEN 'ISBN' " & vbCrLf &
                                " ELSE (SELECT BarcodeType FROM LibraryProprietaryBarcodeType WHERE ID = prdmeabr.BarcodeTypeID) " & vbCrLf &
                                " END "
                End With
                With .Add("BarcodeTypeID", SqlDbType.VarChar)
                    .Expression = "prdmeabr.BarcodeTypeID"
                End With
                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "prdmeabr.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductMeasurementBarcode prdmeabr "

            .SqlWhere = If(String.IsNullOrWhiteSpace(strID), "WHERE prdmeabr.StockMeasurementsID IS NULL", " WHERE prdmeabr.StockMeasurementsID = " & ReadyString(strID))

            .SqlOrder = "ORDER BY prdmeabr.Barcode ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationProductPriceList(ByVal strID As String, Optional vShowLandedCosts As Boolean = True) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 0

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprc.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprc.ID"
                End With
                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprc.PriceLevelID"
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prc.PriceLevel"
                End With
                With .Add("Price", SqlDbType.Money)
                    .Expression = "prdprc.Price"
                End With
                With .Add("UpdaterName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName()
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "prdprc.LastUpdated"
                End With
                With .Add("UpcomingPrice", SqlDbType.Money)
                    .Expression = "upprc.Price"
                End With
                With .Add("UpcomingPriceEffectivityDate", SqlDbType.Money)
                    .Expression = "upprc.EffectivityDate"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprc " &
                " INNER JOIN MobizLibraryPIMPriceLevel prc ON (prdprc.PriceLevelID = prc.ID) " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = prdprc.UserID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                " LEFT JOIN LibraryPimUpcomingPrices upprc ON (prdprc.PriceLevelID = upprc.PriceLevelID AND upprc.ProductID " & IIf(strID.Trim().Length = 0, " IS NULL", " = " & ReadyString(strID)) & " ) " & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prdprc.ProductID IS NULL", " WHERE prdprc.ProductID = " & ReadyString(strID)) &
                IIf(WebRequest.Instance().OrgID.Trim().Length = 0, " AND prdprc.OrganizationID IS NULL", " AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance().OrgID.Trim()))

            If vShowLandedCosts = False Then
                .SqlWhere &= " AND prdprc.PriceLevelID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000', '01000012-0003-0000-0000-000000000000')" & vbCrLf
            End If

            .SqlOrder = "ORDER BY prc.PriceLevel ASC"

            If Now < #8/15/2018# Then
                Dim pSql = .CreateSql
                AppendToLog("getOrganizationProductPriceList vbJ6EDBMCGRW!" & vbCrLf & pSql)
            End If

            pDT = .Requery
        End With


        pDT.Columns.Add("GrossProfit", GetType(Decimal))
        pDT.Columns.Add("GrossProfitMargin", GetType(Decimal)) ' in percent

        Dim pLandedCostID = ReadyGuid(WebRequest.Instance.Business.SystemDefaults.GetLandedCostID())
        If String.IsNullOrWhiteSpace(pLandedCostID) = False Then
            ' find the landed cost value
            Dim pLandedCost As Decimal
            For Each pDR As DataRow In pDT.Rows
                If ReadyGuid(pDR("PriceLevelID")) = pLandedCostID Then
                    pLandedCost = CDec2(pDR("Price"))
                    Exit For
                End If
            Next

            For Each pDR As DataRow In pDT.Rows
                Dim pPrice = CDec2(pDR("Price"))
                If pPrice <= 0 Then
                    Continue For
                End If
                Dim pGrossProfit = pPrice - pLandedCost
                If pGrossProfit <= 0 Then
                    Continue For
                End If
                pDR("GrossProfit") = pGrossProfit
                pDR("GrossProfitMargin") = pGrossProfit / pPrice * 100
            Next
        End If

        Return pDT

    End Function

    Function getOrganizationProductPriceInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprc.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprc.ID"
                End With
                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprc.PriceLevelID"
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prc.PriceLevel"
                End With
                With .Add("Price", SqlDbType.Money)
                    .Expression = "prdprc.Price"
                End With
                With .Add("UpcomingPriceID", SqlDbType.UniqueIdentifier)
                    .Expression = "upprc.ID"
                End With
                With .Add("UpcomingPrice", SqlDbType.Money)
                    .Expression = "upprc.Price"
                End With
                With .Add("UpcomingPriceEffectivityDate", SqlDbType.Date)
                    .Expression = "upprc.EffectivityDate"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprc " &
                " INNER JOIN MobizLibraryPIMPriceLevel prc ON (prdprc.PriceLevelID = prc.ID) " &
                " LEFT JOIN LibraryPimUpcomingPrices upprc ON (prdprc.PriceLevelID = upprc.PriceLevelID AND upprc.ProductID  = prdprc.ProductID) " & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prdprc.ID IS NULL", " WHERE prdprc.ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetOrganizationProductBOMProductMember(ByVal strOrgID As String, ByVal strProductID As String, ByVal strParentBOMID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprdbom.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprdbom.ProductID"
                End With
                With .Add("ParentBOMProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprdbom.ParentBOMProductID"
                End With
                With .Add("ParentBOMComponentName", SqlDbType.VarChar)
                    .Expression = "orgprdbomparent.ComponentName"
                End With
                With .Add("ParentBOMProductName", SqlDbType.VarChar)
                    .Expression = "orgprdbomparentprd.ProductName"
                End With
                With .Add("ComponentName", SqlDbType.VarChar)
                    .Expression = "orgprdbom.ComponentName"
                End With
                With .Add("BOMProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprdbom.BOMProductID"
                End With
                With .Add("BOMProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("Qty", SqlDbType.Decimal)
                    .Expression = "orgprdbom.Qty"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprdbom.UnitMeasureID"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "unitlbl.Label"
                End With
                With .Add("ComponentMethod", SqlDbType.VarChar)
                    .Expression = "orgprdbom.ComponentMethod"
                End With
                With .Add("ChildCount", SqlDbType.Int)
                    .Expression = "(SELECT COUNT(ID) " &
                                  " FROM MobizLibraryPIMProductBOM " &
                                  " WHERE " & IIf(strProductID.Trim().Length = 0, " ProductID IS NULL", " ProductID = " & ReadyString(strProductID)) &
                                  IIf(strOrgID.Trim().Length = 0, " AND OrganizationID IS NULL", " AND OrganizationID = " & ReadyString(strOrgID)) &
                                  " AND ParentBOMProductID = orgprdbom.ID" &
                                  ")"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductBOM orgprdbom " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprdbom.BOMProductID = prd.ID) " &
                " INNER JOIN MobizLibraryPIMMeasure unitm ON (orgprdbom.UnitMeasureID = unitm.ID) " &
                " INNER JOIN MobizLibraryCountriesLanguageLabel unitlbl ON (unitm.MeasureNameID = unitlbl.ID) " &
                " LEFT JOIN MobizLibraryPIMProductBOM orgprdbomparent ON (orgprdbomparent.ID = orgprdbom.ParentBOMProductID) " &
                " LEFT JOIN MobizLibraryPIMProduct orgprdbomparentprd ON (orgprdbomparentprd.ID = orgprdbomparent.BOMProductID) "

            .SqlWhere = IIf(strProductID.Trim().Length = 0, "WHERE orgprdbom.ProductID IS NULL", " WHERE orgprdbom.ProductID = " & ReadyString(strProductID)) &
                IIf(strOrgID.Trim().Length = 0, " AND orgprdbom.OrganizationID IS NULL", " AND orgprdbom.OrganizationID = " & ReadyString(strOrgID)) &
                IIf(strParentBOMID.Trim().Length = 0, " AND orgprdbom.ParentBOMProductID IS NULL", " AND orgprdbom.ParentBOMProductID = " & ReadyString(strParentBOMID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetOrganizationProductMeasurement(ByVal strProductID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String
        Dim mProData As ProData = Common.WebRequest.ProData

        pSql = " SELECT DISTINCT " &
               "    mea.ID as UnitMeasureID " &
               "    ,mealbl.Label as UnitMeasure " &
               " FROM MobizLibraryPIMProductMeasurement prdmea " &
               " INNER JOIN MobizLibraryPIMMeasure mea ON (prdmea.UnitMeasureID = mea.ID) " &
               " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) " &
               " WHERE ProductID " & IIf(strProductID.Trim().Length > 0, " = " & ReadyString(strProductID), " IS NULL ") &
               " UNION " &
               " SELECT " &
               "    mea.ID " &
               "    ,mealbl.Label " &
               " FROM MobizLibraryPIMMeasure mea " &
               " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID)  " &
               " WHERE (CASE " &
               "            (SELECT " &
               "                (CASE  " &
               "                    WHEN IsStandardLength = 1 then 'LENGTH'  " &
               "                    WHEN IsStandardTime = 1 then 'TIME' " &
               "                    WHEN IsStandardArea = 1 then 'AREA' " &
               "                    WHEN IsStandardVolCapacity = 1 then 'VOLUME CAPACITY' " &
               "                    WHEN IsStandardVolDry = 1 then 'VOLUME DRY' " &
               "                    WHEN IsStandardWeight = 1 then 'WEIGHT' " &
               "                    WHEN IsStandardCooking = 1 then 'COOKING' " &
               "                    WHEN IsStandardElectricity = 1 then 'ELECTRICITY' " &
               "                    ELSE NULL " &
               "                END) as MeasureFamily " &
               "            FROM MobizLibraryPIMMeasure mea " &
               "            INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) " &
               "            WHERE (SELECT TOP 1 ContentUnit " &
               "            FROM MobizLibraryPIMProductMeasurement " &
               "            WHERE ProductID " & IIf(strProductID.Trim().Length > 0, " = " & ReadyString(strProductID), " IS NULL ") & ") LIKE '%' + mealbl.Label + '%' " &
               "            )  " &
               "    WHEN 'LENGTH' THEN IsStandardLength  " &
               "    WHEN 'TIME' THEN IsStandardTime  " &
               "    WHEN 'AREA' THEN IsStandardArea  " &
               "    WHEN 'VOLUME CAPACITY' THEN IsStandardVolCapacity  " &
               "    WHEN 'VOLUME DRY' THEN IsStandardVolDry  " &
               "    WHEN 'WEIGHT' THEN IsStandardWeight  " &
               "    WHEN 'COOKING' THEN IsStandardCooking  " &
               "    WHEN 'ELECTRICITY' THEN IsStandardElectricity  " &
               "    END) = 1 " &
               " ORDER BY Label "

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Function SaveOrganizationProduct() As String
        Dim p As New InsertUpdate("LibraryOrganizationPIM")
        Dim pSql As String
        Dim strIDReturn As String

        p("ProductID") = ReadyString(ProductID)
        p("IsDoNotShareTransDataToBrandOwner") = IsDoNotShareDataToBrandOwner
        p("ActivationDate") = ReadyString(ActivationDate)
        p("IsDeActivated") = IsDeactivated
        p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
        p("DeactivationReason") = ReadyString(DeactivationReason)
        p("ProductAvailableToMobile") = ReadyString(ProductAvailableToMobile)
        p("Comments") = ReadyString(Comments)
        p("IsBan") = IsBan
        p("IsInventoryCountItem") = If(IsInventoryCountItem, 1, 0)
        p("IsPromoItem") = If(IsPromoItem, 1, 0)
        p("IsVatExempt") = CInt2(IsVatExempt)
        p("MaxAllowableDiscountableAmount") = CDec2(MaxAllowableDiscountableAmount)
        p("ItemBasedMaxAllowableDiscount") = IIf(CStr2(ItemBasedMaxAllowableDiscount) = "", "NULL", ReadyString(ItemBasedMaxAllowableDiscount))
        p("DiscountedPriceAction") = IIf(CStr2(DiscountedPriceAction) = "", ReadyString("Exact Price"), ReadyString(DiscountedPriceAction))
        p("LastUpdated") = "GetDate()"
        If String.IsNullOrWhiteSpace(PaymentTermID) = False Then
            p("PaymentTermID") = ReadyString(PaymentTermID)
        End If

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        If CBool2(IsDeactivated) Then
            Dim hb As New ProductGroupController
            hb.RemoveProductInHB(ProductID)
        End If

        'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
        'Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function SaveBarcode() As String
        Dim pOut As New JsonDictionary

        Try
            Dim p As New InsertUpdate("MobizLibraryPIMProductMeasurement")
            Common.WebRequest.ProData.BeginTransaction()

            p("ProductID") = ReadyString(ProductID)
            p("UnitMeasureID") = ReadyString(UnitMeasureID)
            p("Quantity") = Qty
            p("MeasurementLevel") = "ISNULL((SELECT Max(MeasurementLevel) + 1 FROM MobizLibraryPIMProductMeasurement WHERE ProductID = " & ReadyString(ProductID) & "),1)"
            p("Discount") = ProductDiscount
            p("ContentQty") = ProductMeasurementContentQty
            p("ContentUnit") = ReadyString(ProductMeasurementContentUnit)
            p("ProdDimensionLength") = ProductMeasurementDimLenght
            p("ProdDimensionLengthUnit") = ReadyString(ProductMeasurementDimLenghtUnit)
            p("ProdDimensionWidth") = ProductMeasurementDimWidth
            p("ProdDimensionWidthUnit") = ReadyString(ProductMeasurementDimWidthUnit)
            p("ProdDimensionHeight") = ProductMeasurementDimHeight
            p("ProdDimensionHeightUnit") = ReadyString(ProductMeasurementDimHeightUnit)
            p("ProductWeight") = ProductMeasurementWeight
            p("ProductWeightUnit") = ReadyString(ProductMeasurementWeightUnit)
            p("IsTimeBaseUnit") = CInt2(IsTimeBaseUnit)
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "GetDate()"

            If CStr2(ID) = "" Then
                ID = Guid.NewGuid.ToString()

                p("ID") = ReadyString(ID)
                Common.WebRequest.ProData.Execute(p.GetInsert())

                pOut.Add("StockMeasurementID", ID)
                pOut.Add("UnitMeasureID", UnitMeasureID)
                pOut.Add("Quantity", Qty)
            Else
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(ID)))
            End If

            Dim bID As String = Guid.NewGuid.ToString()
            p = New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")

            p("StockMeasurementsID") = ReadyString(ID.ToString())
            p("Barcode") = ReadyString(Barcode)
            p("BarcodeTypeID") = ReadyString(BarcodeType)
            p("LastUpdated") = "GETDATE()"

            If CStr2(BarcodeID) = "" Then
                p("ID") = ReadyString(bID)
                Common.WebRequest.ProData.Execute(p.GetInsert())
            Else
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(BarcodeID)))
            End If

            Common.WebRequest.ProData.CommitTransaction()

            pOut.Add("Results", "saved")

        Catch ex As Exception
            pOut.Add("Results", ex.Message)
        End Try

        Return pOut.Serialize()
    End Function

    Function SaveOrganizationProductMeasurement() As String
        Dim p As New InsertUpdate("MobizLibraryPIMProductMeasurement")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable

        Try

            p("ProductID") = ReadyString(ProductID)
            p("UnitMeasureID") = ReadyString(UnitMeasureID)
            p("Quantity") = Qty
            p("Discount") = ProductDiscount
            p("ContentQty") = ProductMeasurementContentQty
            p("ContentUnit") = ReadyString(ProductMeasurementContentUnit)
            p("ProdDimensionLength") = ProductMeasurementDimLenght
            p("ProdDimensionLengthUnit") = ReadyString(ProductMeasurementDimLenghtUnit)
            p("ProdDimensionWidth") = ProductMeasurementDimWidth
            p("ProdDimensionWidthUnit") = ReadyString(ProductMeasurementDimWidthUnit)
            p("ProdDimensionHeight") = ProductMeasurementDimHeight
            p("ProdDimensionHeightUnit") = ReadyString(ProductMeasurementDimHeightUnit)
            p("ProductWeight") = ProductMeasurementWeight
            p("ProductWeightUnit") = ReadyString(ProductMeasurementWeightUnit)
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "GetDate()"
            p("IsTimeBaseUnit") = CInt2(IsTimeBaseUnit)

            If CBool2(IsTimeBaseUnit) Then
                p("TimeBaseUnitDescription") = ReadyString(TimeBaseUnitDescription)

                If TimeBaseUnitDescription.ToLower() = "day/ night" Then
                    p("NoOfDaysInTimeUnit") = "1"
                ElseIf TimeBaseUnitDescription.ToLower() = "week" Then
                    p("NoOfDaysInTimeUnit") = "7"
                ElseIf TimeBaseUnitDescription.ToLower() = "month" Then
                    p("NoOfDaysInTimeUnit") = "30"
                ElseIf TimeBaseUnitDescription.ToLower() = "yearly" Then
                    p("NoOfDaysInTimeUnit") = "360"
                End If
            Else
                p("TimeBaseUnitDescription") = ReadyString("")
                p("NoOfDaysInTimeUnit") = "NULL"
            End If

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("MeasurementLevel") = "ISNULL((SELECT Max(MeasurementLevel) + 1 FROM MobizLibraryPIMProductMeasurement WHERE ProductID = " & ReadyString(ProductID) & "),1)"
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Measurement Barcode
            For Each objOrgProductMeasurementBarcode As Object In OrgProductMeasurementBarcodeList

                If (objOrgProductMeasurementBarcode("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgProductMeasurementBarcode("RecState").ToString().Trim() <> "Remove") Then

                        p = New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")

                        p("StockMeasurementsID") = ReadyString(strIDReturn)
                        p("Barcode") = ReadyString(objOrgProductMeasurementBarcode("Barcode").ToString().Trim())
                        p("LastUpdated") = "GETDATE()"

                        Dim strIDRec As String = ""
                        If (objOrgProductMeasurementBarcode("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgProductMeasurementBarcode("ID").ToString().Trim()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        'Notify the TablesLastUpdated that there were changes done in the MobizLibraryPIMProductMeasurement
                        pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'MobizLibraryPIMProductMeasurement'"
                        pDT = Common.WebRequest.ProData.Execute(pSql)
                        If pDT.Rows.Count > 0 Then
                            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductMeasurement'"
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Else
                            pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'MobizLibraryPIMProductMeasurement', NULL, NULL)"
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductMeasurement'"
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If

                    Else

                        pSql = "DELETE " &
                               " FROM MobizLibraryPIMProductMeasurementBarcode " &
                               " WHERE ID " & IIf(objOrgProductMeasurementBarcode("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgProductMeasurementBarcode("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgProductMeasurementBarcode

            If CDec2(ProductMeasurementContentQty) > 0 Then
                WebRequest.Instance.Business.LibraryStock.CreateUnitDefinition(
                    vProductID:=ProductID,
                    vDestinationMeasure:=ProductMeasurementContentUnit,
                    vQuantity:=ProductMeasurementContentQty,
                    vSourceMeasureID:=UnitMeasureID,
                    vOrgID:=WebRequest.Instance.OrgID)
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return ex.Message
        End Try

        Return strIDReturn

    End Function

    Function SaveOrganizationProductPrice() As String
        Dim p As New InsertUpdate("MobizLibraryPIMProductPriceLevel")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable
        Dim pSqlSELECT As String
        Dim upcomingpriceid As String = Common.WebRequest.Data("UpcomingPriceID")
        Dim upcomingprice As String = Common.WebRequest.Data("UpcomingPrice")
        Dim upcomingpriceeffdate As String = Common.WebRequest.Data("UpcomingPriceEffectivityDate")
        Try

            p("OrganizationID") = ReadyString(WebRequest.Instance().OrgID)
            p("ProductID") = ReadyString(ProductID)
            p("PriceLevelID") = ReadyString(PriceLevelID)
            p("Price") = Price
            p("DeactivateDiscount") = 0
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "GetDate()"

            'check first if existing
            pSqlSELECT = "SELECT ID FROM MobizLibraryPIMProductPriceLevel WHERE OrganizationID = " &
                ReadyString(WebRequest.Instance().OrgID) & " AND PriceLevelID = " & ReadyString(PriceLevelID) &
                " AND ProductID = " & ReadyString(ProductID)

            pDT = Common.WebRequest.ProData.Execute(pSqlSELECT)

            If (pDT.Rows.Count > 0) Then
                strIDReturn = pDT.Rows(0).Item("ID").ToString()
                pSql = p.GetUpdate(" ID = " & ReadyString(strIDReturn))
            Else

                If (ID.Trim().Length <= 0) Then
                    strIDReturn = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDReturn)
                    pSql = p.GetInsert()
                Else
                    strIDReturn = ID
                    pSql = p.GetUpdate(" ID = " & ReadyString(ID))
                End If

            End If


            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If String.IsNullOrWhiteSpace(upcomingprice) And IsGuid(CStr2(upcomingpriceid)) Then
                'delete upcoming price
                Common.WebRequest.ProData.Execute("DELETE FROM LibraryPimUpcomingPrices WHERE ID = " & ReadyString(upcomingpriceid))
            Else
                If Not String.IsNullOrWhiteSpace(upcomingprice) Then
                    p = New InsertUpdate("LibraryPimUpcomingPrices")

                    p("Price") = CDec2(upcomingprice)
                    p("EffectivityDate") = ReadyString(upcomingpriceeffdate)
                    p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = "getdate()"

                    If Not String.IsNullOrWhiteSpace(upcomingpriceid) Then
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(upcomingpriceid)))
                    Else
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("ProductID") = ReadyString(ProductID)
                        p("PriceLevelID") = ReadyString(PriceLevelID)
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                End If
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SaveOrganizationProductBOM() As String
        Dim p As New InsertUpdate("MobizLibraryPIMProductBOM")
        Dim pSql As String
        Dim strIDReturn As String

        p("OrganizationID") = ReadyString(OrganizationID)
        p("ProductID") = ReadyString(ProductID)
        p("ComponentName") = ReadyString(ComponentName)
        p("BOMProductID") = ReadyString(BOMProductID)
        p("Qty") = Qty
        p("UnitMeasureID") = ReadyString(UnitMeasureID)
        p("ComponentMethod") = ReadyString(ComponentMethod)
        p("ParentBOMProductID") = IIf(ParentBOMProductID.Trim().Length > 0, ReadyString(ParentBOMProductID), "NULL")

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function CheckOrganizationCustomerProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ProductID"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerProduct orgprd "

            .SqlWhere = "WHERE orgprd.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgprd.CustomerID = " & ReadyString(CustomerID) &
                        " AND orgprd.ProductID = " & ReadyString(ProductID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrgCustomerProductCount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ProductCount", SqlDbType.UniqueIdentifier)
                    .Expression = "COUNT(orgprd.ID)"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerProduct orgprd "

            .SqlWhere = "WHERE orgprd.CustomerID = " & ReadyString(CustomerID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerProductList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.CustomerID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgprd.ProductID"
                    .Searchable = False
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgprd.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgprd.IsDeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgprd.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgprd.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgprd.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgprd.IsBan"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerProduct orgprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) "

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerProductList(ByVal strSearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.CustomerID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ProductID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgprd.ActivationDate"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgprd.IsDeActivated"
                End With
                With .Add("DeactivationDate", SqlDbType.Date)
                    .Expression = "orgprd.DeactivationDate"
                End With
                With .Add("DeactivationReason", SqlDbType.VarChar)
                    .Expression = "orgprd.DeactivationReason"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgprd.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgprd.IsBan"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerProduct orgprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) "

            .SqlWhere = "WHERE prd.ProductSearchString LIKE " & ReadyStringWild(strSearchKeyWord)

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerProductInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.CustomerID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ActivationDate", SqlDbType.Date)
                    .Expression = "orgprd.ActivationDate"
                End With
                With .Add("ActivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgprd.ActivationDate)"
                End With
                With .Add("ActivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgprd.ActivationDate)"
                End With
                With .Add("ActivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgprd.ActivationDate)"
                End With
                With .Add("DeActivated", SqlDbType.Bit)
                    .Expression = "orgprd.IsDeActivated"
                End With
                With .Add("DeactivationDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(orgprd.DeactivationDate)"
                End With
                With .Add("DeactivationDateDay", SqlDbType.Int)
                    .Expression = "DAY(orgprd.DeactivationDate)"
                End With
                With .Add("DeactivationDateYear", SqlDbType.Int)
                    .Expression = "YEAR(orgprd.DeactivationDate)"
                End With
                With .Add("Comments", SqlDbType.VarChar)
                    .Expression = "orgprd.Comments"
                End With
                With .Add("IsBan", SqlDbType.Bit)
                    .Expression = "orgprd.IsBan"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerProduct orgprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgprd.ID IS NULL", " WHERE orgprd.ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveOrganizationCustomerProduct() As String
        Dim p As New InsertUpdate("LibraryOrganizationCustomerProduct")
        Dim pSql As String
        Dim strIDReturn As String

        p("ProductID") = ReadyString(ProductID)
        p("CustomerID") = ReadyString(CustomerID)
        p("ActivationDate") = ReadyString(ActivationDate)
        p("IsDeActivated") = IsDeactivated
        p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
        p("DeactivationReason") = ReadyString(DeactivationReason)
        p("Comments") = ReadyString(Comments)
        p("IsBan") = IsBan

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function CheckOrganizationFOWSTProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.ID"
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.FOWSTID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.ProductID"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganizationPIMByFOWST orgfowstprd "

            .SqlWhere = "WHERE orgfowstprd.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgfowstprd.ProductID = " & ReadyString(ProductID) &
                        " AND orgfowstprd.FOWSTID = " & ReadyString(FowstID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationFOWSTProductList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            ' ''.MaxRecords = MaxReturnRow       ''''Remove Limit prder by Sir robert 08242013

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.FOWSTID"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgfowstprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganizationPIMByFOWST orgfowstprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgfowstprd.ProductID = prd.ID) "

            .SqlWhere = "WHERE orgfowstprd.FOWSTID = " & ReadyString(FowstID)

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationFOWSTProductList(ByVal strSearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            ' ''.MaxRecords = MaxReturnRow '''Order by  Sir Robert 08/24/2013 to just show all records

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.FOWSTID"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgfowstprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganizationPIMByFOWST orgfowstprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgfowstprd.ProductID = prd.ID) "

            .SqlWhere = "WHERE orgfowstprd.FOWSTID = " & ReadyString(FowstID) &
                " AND prd.ProductSearchString LIKE " & ReadyStringWild(strSearchKeyWord)

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationFowstProductBinList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.FowstID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.ProductID"
                End With
                With .Add("BinID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.BinID"
                End With
                With .Add("BinName", SqlDbType.VarChar)
                    .Expression = "fowstbin.Name"
                End With
                With .Add("MinPerProduct", SqlDbType.Float)
                    .Expression = "ISNULL(fowstbinprd.MinPerProduct,0) "
                End With
                With .Add("MaxPerProduct", SqlDbType.Float)
                    .Expression = "ISNULL(fowstbinprd.MaxPerProduct,0)"
                End With
            End With

            .SqlFrom = " FROM LibraryFowstBinProduct fowstbinprd " &
                " INNER JOIN LibraryFowstBin fowstbin ON (fowstbinprd.BinID = fowstbin.ID)"

            .SqlWhere = "WHERE fowstbinprd.FOWSTID = " & ReadyString(FowstID) &
                " AND fowstbinprd.ProductID = " & ReadyString(ProductID)

            .SqlOrder = "ORDER BY fowstbin.Name ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationFowstProductInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.FOWSTID"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgfowstprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ReorderPoint", SqlDbType.Float)
                    .Expression = "orgfowstprd.ReorderPoint"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationPIMByFOWST orgfowstprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgfowstprd.ProductID = prd.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowstprd.ID IS NULL", " WHERE orgfowstprd.ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveOrganizationFowstProduct() As String
        Dim p As New InsertUpdate("LibraryOrganizationPIMByFOWST")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String
        Dim strBinID As String

        Try

            Common.WebRequest.ProData.BeginTransaction()

            p("FOWSTID") = ReadyString(FowstID)
            p("ProductID") = ReadyString(ProductID)
            p("ReorderPoint") = OrgFOWSTProductReorderPoint

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Bin
            For Each objOrgFowstBin As Object In OrgFOWSTProductBinList

                If (objOrgFowstBin("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgFowstBin("RecState").ToString().Trim() <> "Remove") Then

                        '''check if Bin exist
                        If (objOrgFowstBin("BinID").ToString().Trim().Length <= 0) Then
                            strBinID = Guid.NewGuid.ToString()

                            p = New InsertUpdate("LibraryFowstBin")
                            p("ID") = ReadyString(strBinID)
                            p("FowstID") = ReadyString(objOrgFowstBin("FowstID").ToString())
                            p("Name") = ReadyString(objOrgFowstBin("BinName").ToString())
                            p("Description") = ReadyString("")

                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else
                            strBinID = objOrgFowstBin("BinID").ToString()
                        End If

                        p = New InsertUpdate("LibraryFowstBinProduct")

                        p("FowstID") = ReadyString(objOrgFowstBin("FowstID").ToString())
                        p("BinID") = ReadyString(strBinID)
                        p("ProductID") = ReadyString(objOrgFowstBin("ProductID").ToString())
                        p("IsDefault") = IIf(objOrgFowstBin("BinName").ToString() = "Unnamed Bin", 1, 0)
                        p("MinPerProduct") = IIf(objOrgFowstBin("BinMin").ToString().Trim().Length > 0, objOrgFowstBin("BinMin").ToString().Trim(), 0)
                        p("MaxPerProduct") = IIf(objOrgFowstBin("BinMax").ToString().Trim().Length > 0, objOrgFowstBin("BinMax").ToString().Trim(), 0)

                        If (objOrgFowstBin("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgFowstBin("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.LibraryFowstBinProduct " &
                               " WHERE ID " & IIf(objOrgFowstBin("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgFowstBin("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgFowstBin

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function CheckOrganizationCustomerFOWSTProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.ID"
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.CustomerFowstID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.ProductID"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganizationCustomerProductByFowst orgcustfowstprd "

            .SqlWhere = "WHERE orgcustfowstprd.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND orgcustfowstprd.ProductID = " & ReadyString(ProductID) &
                        " AND orgcustfowstprd.CustomerFowstID = " & ReadyString(FowstID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerFOWSTProductList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.CustomerFowstID"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgcustfowstprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganizationCustomerProductByFowst orgcustfowstprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgcustfowstprd.ProductID = prd.ID) "

            .SqlWhere = "WHERE orgcustfowstprd.CustomerFowstID = " & ReadyString(FowstID)

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerFOWSTProductList(ByVal strSearchKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgcustfowstprd.CustomerFowstID"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgcustfowstprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganizationCustomerProductByFowst orgcustfowstprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgcustfowstprd.ProductID = prd.ID) "

            .SqlWhere = "WHERE orgcustfowstprd.CustomerFowstID = " & ReadyString(FowstID) &
                " AND prd.ProductSearchString LIKE " & ReadyStringWild(strSearchKeyWord)

            .SqlOrder = "ORDER BY prd.ProductName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerFowstProductBinList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.FowstID"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.ProductID"
                End With
                With .Add("BinID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbinprd.BinID"
                End With
                With .Add("BinName", SqlDbType.VarChar)
                    .Expression = "fowstbin.Name"
                End With
                With .Add("MinPerProduct", SqlDbType.Float)
                    .Expression = "ISNULL(fowstbinprd.MinPerProduct,0) "
                End With
                With .Add("MaxPerProduct", SqlDbType.Float)
                    .Expression = "ISNULL(fowstbinprd.MaxPerProduct,0)"
                End With
            End With

            .SqlFrom = " FROM LibraryFowstBinProduct fowstbinprd " &
                " INNER JOIN LibraryFowstBin fowstbin ON (fowstbinprd.BinID = fowstbin.ID)"

            .SqlWhere = "WHERE fowstbinprd.FowstID = " & ReadyString(FowstID) &
                " AND fowstbinprd.ProductID = " & ReadyString(ProductID)

            .SqlOrder = "ORDER BY fowstbin.Name ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getOrganizationCustomerFowstProductInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.ID"
                    .Searchable = False
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstprd.CustomerFowstID"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "orgfowstprd.ProductID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ReorderPoint", SqlDbType.Float)
                    .Expression = "orgfowstprd.ReorderPoint"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerProductByFowst orgfowstprd " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgfowstprd.ProductID = prd.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowstprd.ID IS NULL", " WHERE orgfowstprd.ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveOrganizationCustomerFowstProduct() As String
        Dim p As New InsertUpdate("LibraryOrganizationCustomerProduct")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String
        Dim strBinID As String
        Dim pDT As DataTable
        Dim pFinder As Finder

        Try

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = MaxReturnRow

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "custprd.ID"
                        .Searchable = False
                    End With
                End With

                .SqlFrom = "FROM LibraryOrganizationCustomerProduct custprd "

                .SqlWhere = " WHERE custprd.CustomerID IN (SELECT OrganizationID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(FowstID) & ") " &
                    " AND custprd.ProductID = " & ReadyString(ProductID)

                pDT = .Requery
            End With

            Common.WebRequest.ProData.BeginTransaction()

            ''Insert to Customer Product List if Not yet exist in their product list
            If (pDT.Rows.Count <= 0) Then
                p = New InsertUpdate("LibraryOrganizationCustomerProduct")
                p("ProductID") = ReadyString(ProductID)
                p("CustomerID") = "(SELECT OrganizationID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(FowstID) & ") "
                p("ActivationDate") = "GETDATE()"
                p("IsDeActivated") = "0"
                p("DeactivationDate") = "NULL"
                p("DeactivationReason") = "''"
                p("Comments") = "''"
                p("IsBan") = "0"

                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            '''save to FOWST
            p = New InsertUpdate("LibraryOrganizationCustomerProductByFowst")
            p("CustomerFowstID") = ReadyString(FowstID)
            p("ProductID") = ReadyString(ProductID)
            p("ReorderPoint") = OrgFOWSTProductReorderPoint

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Bin
            For Each objOrgFowstBin As Object In OrgFOWSTProductBinList

                If (objOrgFowstBin("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgFowstBin("RecState").ToString().Trim() <> "Remove") Then

                        '''check if Bin exist
                        If (objOrgFowstBin("BinID").ToString().Trim().Length <= 0) Then
                            strBinID = Guid.NewGuid.ToString()

                            p = New InsertUpdate("LibraryFowstBin")
                            p("ID") = ReadyString(strBinID)
                            p("FowstID") = ReadyString(FowstID)
                            p("Name") = ReadyString(objOrgFowstBin("BinName").ToString())
                            p("Description") = ReadyString("")

                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else
                            strBinID = objOrgFowstBin("BinID").ToString()
                        End If

                        p = New InsertUpdate("LibraryFowstBinProduct")

                        p("FowstID") = ReadyString(FowstID)
                        p("BinID") = ReadyString(strBinID)
                        p("ProductID") = ReadyString(ProductID) ''' ReadyString(objOrgFowstBin("ProductID").ToString())
                        p("IsDefault") = IIf(objOrgFowstBin("BinName").ToString() = "Unnamed Bin", 1, 0)
                        p("MinPerProduct") = IIf(objOrgFowstBin("BinMin").ToString().Trim().Length > 0, objOrgFowstBin("BinMin").ToString().Trim(), 0)
                        p("MaxPerProduct") = IIf(objOrgFowstBin("BinMax").ToString().Trim().Length > 0, objOrgFowstBin("BinMax").ToString().Trim(), 0)

                        If (objOrgFowstBin("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgFowstBin("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM LibraryFowstBinProduct " &
                               " WHERE ID " & IIf(objOrgFowstBin("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgFowstBin("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgFowstBin

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function getOrgOwnerList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgowner.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgowner.OrganizationID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgowner.PeopleID"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
                With .Add("JobOccupationID", SqlDbType.VarChar)
                    .Expression = "orgowner.JobOccupationID"
                End With
                With .Add("JobOccupation", SqlDbType.VarChar)
                    .Expression = "jobocc.JobOccupation"
                End With
                With .Add("BusinessUnit", SqlDbType.VarChar)
                    .Expression = "orgowner.BusinessUnit"
                End With
                With .Add("IsIncorporator", SqlDbType.Bit)
                    .Expression = "orgowner.IsIncorporator"
                End With
                With .Add("IsDirector", SqlDbType.Bit)
                    .Expression = "orgowner.IsDirector"
                End With
                With .Add("IsStockHolder", SqlDbType.Bit)
                    .Expression = "orgowner.IsStockHolder"
                End With
                With .Add("IsOfficer", SqlDbType.Bit)
                    .Expression = "orgowner.IsOfficer"
                End With
                With .Add("IsExecutiveCommittee", SqlDbType.Bit)
                    .Expression = "orgowner.IsExecutiveCommittee"
                End With
                With .Add("IsOwner", SqlDbType.Bit)
                    .Expression = "orgowner.IsOwner"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationOwner orgowner " &
                       " INNER JOIN dbo.MobizLibraryPeople ppol ON (orgowner.PeopleID = ppol.ID) " &
                       " LEFT JOIN dbo.MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID) " &
                       " INNER JOIN dbo.MobizLibraryJobOccupation jobocc ON (orgowner.JobOccupationID = jobocc.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgowner.OrganizationID IS NULL", " WHERE orgowner.OrganizationID = " & ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgOwnerInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgowner.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgowner.OrganizationID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgowner.PeopleID"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
                With .Add("JobOccupationID", SqlDbType.VarChar)
                    .Expression = "orgowner.JobOccupationID"
                End With
                With .Add("JobOccupation", SqlDbType.VarChar)
                    .Expression = "jobocc.JobOccupation"
                End With
                With .Add("BusinessUnit", SqlDbType.VarChar)
                    .Expression = "orgowner.BusinessUnit"
                End With
                With .Add("IsIncorporator", SqlDbType.Bit)
                    .Expression = "orgowner.IsIncorporator"
                End With
                With .Add("IsDirector", SqlDbType.Bit)
                    .Expression = "orgowner.IsDirector"
                End With
                With .Add("IsStockHolder", SqlDbType.Bit)
                    .Expression = "orgowner.IsStockHolder"
                End With
                With .Add("IsOfficer", SqlDbType.Bit)
                    .Expression = "orgowner.IsOfficer"
                End With
                With .Add("IsExecutiveCommittee", SqlDbType.Bit)
                    .Expression = "orgowner.IsExecutiveCommittee"
                End With
                With .Add("IsOwner", SqlDbType.Bit)
                    .Expression = "orgowner.IsOwner"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationOwner orgowner " &
                       " INNER JOIN dbo.MobizLibraryPeople ppol ON (orgowner.PeopleID = ppol.ID) " &
                       " LEFT JOIN dbo.MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID) " &
                       " INNER JOIN dbo.MobizLibraryJobOccupation jobocc ON (orgowner.JobOccupationID = jobocc.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgowner.ID IS NULL", " WHERE orgowner.ID = " & ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgOwner() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationOwner")
        Dim clsPeople As New People
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String

        Try

            '''Save Organization Owner
            p("OrganizationID") = ReadyString(OrganizationID)
            p("PeopleID") = ReadyString(PeopleID)
            p("JobOccupationID") = ReadyString(JobOccupationID)
            p("BusinessUnit") = ReadyString(BusinessUnit)
            p("IsIncorporator") = IsIncorporator
            p("IsDirector") = IsDirector
            p("IsStockHolder") = IsStockHolder
            p("IsOfficer") = IsOfficer
            p("IsExecutiveCommittee") = IsExecutiveCommittee
            p("IsOwner") = IsOwner
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "getdate()"

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Catch ex As Exception
        End Try

        Return strIDReturn

    End Function

    Function RemoveOrgOwner() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationOwner " &
                   " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getOrgHoldingCompanyInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("IsHasHoldingCompany", SqlDbType.Bit)
                    .Expression = "org.IsHasHoldingCompany"
                End With
                With .Add("HoldingCompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.HoldingCompanyID"
                End With
                With .Add("HoldingCompanyName", SqlDbType.VarChar)
                    .Expression = "orghold.CompanyName"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganization org " &
                       " LEFT JOIN dbo.MobizLibraryOrganization orghold ON (org.HoldingCompanyID = orghold.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE org.ID IS NULL", " WHERE org.ID = " & ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgHoldingCompany() As String
        Dim p As New InsertUpdate("MobizLibraryOrganization")
        Dim pSql As String
        Dim strIDReturn As String

        Try

            p("IsHasHoldingCompany") = IsHasHoldingCompany
            p("HoldingCompanyID") = IIf(OrgHoldingCompanyID.Trim().Length > 0, ReadyString(OrgHoldingCompanyID), "NULL")

            strIDReturn = OrganizationID
            pSql = p.GetUpdate(" ID = " & ReadyString(OrganizationID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function getOrgAffliatesList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaff.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaff.OrganizationID"
                End With
                With .Add("AffiliatesID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaff.AffiliatesID"
                End With
                With .Add("AffiliateCompanyName", SqlDbType.VarChar)
                    .Expression = "aff.CompanyName"
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "orgaff.Remarks"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationAffiliates orgaff " &
                       " INNER JOIN dbo.MobizLibraryOrganization aff ON (orgaff.AffiliatesID = aff.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgaff.OrganizationID IS NULL", " WHERE orgaff.OrganizationID = " & ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOrgAffliatesInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaff.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaff.OrganizationID"
                End With
                With .Add("AffiliatesID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgaff.AffiliatesID"
                End With
                With .Add("AffiliateCompanyName", SqlDbType.VarChar)
                    .Expression = "aff.CompanyName"
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "orgaff.Remarks"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationAffiliates orgaff " &
                       " INNER JOIN dbo.MobizLibraryOrganization aff ON (orgaff.AffiliatesID = aff.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgaff.ID IS NULL", " WHERE orgaff.ID = " & ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgAffiliates() As String
        Dim p As New InsertUpdate("dbo.MobizLibraryOrganizationAffiliates")
        Dim pSql As String
        Dim strIDReturn As String

        Try

            p("OrganizationID") = ReadyString(OrganizationID)
            p("AffiliatesID") = ReadyString(OrgAffiliatesID)
            p("Remarks") = ReadyString(Remarks)
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "getdate()"

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function RemoveOrgAffiliates() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryOrganizationAffiliates " &
                   " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function


    Function SaveWorkingHours() As String
        Dim p As New InsertUpdate("LibraryOrganizationWorkSchedule")
        Dim pSql As String
        Dim strIDReturn As String

        p("WorkScheduleName") = ReadyString(Common.WebRequest.Data("Name"))
        p("Description") = ReadyString(Common.WebRequest.Data("Description"))
        p("LastUpdated") = "GetDate()"

        'p("Monday")
        If IsDate(Common.WebRequest.Data("MondayFrom")) And IsDate(Common.WebRequest.Data("MondayTo")) Then
            p("MondayFrom") = ReadyString(CDate(Common.WebRequest.Data("MondayFrom")))
            p("MondayTo") = ReadyString(CDate(Common.WebRequest.Data("MondayTo")))
        Else
            p("MondayFrom") = "NULL"
            p("MondayTo") = "NULL"
        End If

        'p("Monday") lunch
        If IsDate(Common.WebRequest.Data("MondayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("MondayLunchBreakTo")) Then
            p("MondayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("MondayLunchBreakFrom")))
            p("MondayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("MondayLunchBreakTo")))
        Else
            p("MondayLunchBreakFrom") = "NULL"
            p("MondayLunchBreakTo") = "NULL"
        End If

        'p("Monday") dinner
        If IsDate(Common.WebRequest.Data("MondayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("MondayDinnerBreakTo")) Then
            p("MondayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("MondayDinnerBreakFrom")))
            p("MondayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("MondayDinnerBreakTo")))
        Else
            p("MondayDinnerBreakFrom") = "NULL"
            p("MondayDinnerBreakTo") = "NULL"
        End If

        'p("Tuesday")
        If IsDate(Common.WebRequest.Data("TuesdayFrom")) And IsDate(Common.WebRequest.Data("TuesdayTo")) Then
            p("TuesdayFrom") = ReadyString(CDate(Common.WebRequest.Data("TuesdayFrom")))
            p("TuesdayTo") = ReadyString(CDate(Common.WebRequest.Data("TuesdayTo")))
        Else
            p("TuesdayFrom") = "NULL"
            p("TuesdayTo") = "NULL"
        End If

        'p("Tuesday") lunch
        If IsDate(Common.WebRequest.Data("TuesdayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("TuesdayLunchBreakTo")) Then
            p("TuesdayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("TuesdayLunchBreakFrom")))
            p("TuesdayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("TuesdayLunchBreakTo")))
        Else
            p("TuesdayLunchBreakFrom") = "NULL"
            p("TuesdayLunchBreakTo") = "NULL"
        End If

        'p("Tuesday") dinner
        If IsDate(Common.WebRequest.Data("TuesdayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("TuesdayDinnerBreakTo")) Then
            p("TuesdayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("TuesdayDinnerBreakFrom")))
            p("TuesdayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("TuesdayDinnerBreakTo")))
        Else
            p("TuesdayDinnerBreakFrom") = "NULL"
            p("TuesdayDinnerBreakTo") = "NULL"
        End If

        'p("Wednesday")
        If IsDate(Common.WebRequest.Data("WednesdayFrom")) And IsDate(Common.WebRequest.Data("WednesdayTo")) Then
            p("WednesdayFrom") = ReadyString(CDate(Common.WebRequest.Data("WednesdayFrom")))
            p("WednesdayTo") = ReadyString(CDate(Common.WebRequest.Data("WednesdayTo")))
        Else
            p("WednesdayFrom") = "NULL"
            p("WednesdayTo") = "NULL"
        End If

        'p("Wednesday")lunch
        If IsDate(Common.WebRequest.Data("WednesdayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("WednesdayLunchBreakTo")) Then
            p("WednesdayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("WednesdayLunchBreakFrom")))
            p("WednesdayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("WednesdayLunchBreakTo")))
        Else
            p("WednesdayLunchBreakFrom") = "NULL"
            p("WednesdayLunchBreakTo") = "NULL"
        End If

        'p("Wednesday")dinner
        If IsDate(Common.WebRequest.Data("WednesdayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("WednesdayDinnerBreakTo")) Then
            p("WednesdayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("WednesdayDinnerBreakFrom")))
            p("WednesdayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("WednesdayDinnerBreakTo")))
        Else
            p("WednesdayDinnerBreakFrom") = "NULL"
            p("WednesdayDinnerBreakTo") = "NULL"
        End If


        'p("Thursday")
        If IsDate(Common.WebRequest.Data("ThursdayFrom")) And IsDate(Common.WebRequest.Data("ThursdayTo")) Then
            p("ThursdayFrom") = ReadyString(CDate(Common.WebRequest.Data("ThursdayFrom")))
            p("ThursdayTo") = ReadyString(CDate(Common.WebRequest.Data("ThursdayTo")))
        Else
            p("ThursdayFrom") = "NULL"
            p("ThursdayTo") = "NULL"
        End If

        'p("Thursday") lunch
        If IsDate(Common.WebRequest.Data("ThursdayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("ThursdayLunchBreakTo")) Then
            p("ThursdayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("ThursdayLunchBreakFrom")))
            p("ThursdayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("ThursdayLunchBreakTo")))
        Else
            p("ThursdayLunchBreakFrom") = "NULL"
            p("ThursdayLunchBreakTo") = "NULL"
        End If

        'p("Thursday") dinner
        If IsDate(Common.WebRequest.Data("ThursdayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("ThursdayDinnerBreakTo")) Then
            p("ThursdayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("ThursdayDinnerBreakFrom")))
            p("ThursdayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("ThursdayDinnerBreakTo")))
        Else
            p("ThursdayDinnerBreakFrom") = "NULL"
            p("ThursdayDinnerBreakTo") = "NULL"
        End If

        'p("Friday")
        If IsDate(Common.WebRequest.Data("FridayFrom")) And IsDate(Common.WebRequest.Data("FridayTo")) Then
            p("FridayFrom") = ReadyString(CDate(Common.WebRequest.Data("FridayFrom")))
            p("FridayTo") = ReadyString(CDate(Common.WebRequest.Data("FridayTo")))
        Else
            p("FridayFrom") = "NULL"
            p("FridayTo") = "NULL"
        End If

        'p("Friday") lunch
        If IsDate(Common.WebRequest.Data("FridayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("FridayLunchBreakTo")) Then
            p("FridayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("FridayLunchBreakFrom")))
            p("FridayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("FridayLunchBreakTo")))
        Else
            p("FridayLunchBreakFrom") = "NULL"
            p("FridayLunchBreakTo") = "NULL"
        End If

        'p("Friday") dinner
        If IsDate(Common.WebRequest.Data("FridayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("FridayDinnerBreakTo")) Then
            p("FridayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("FridayDinnerBreakFrom")))
            p("FridayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("FridayDinnerBreakTo")))
        Else
            p("FridayDinnerBreakFrom") = "NULL"
            p("FridayDinnerBreakTo") = "NULL"
        End If

        'p("Saturday")
        If IsDate(Common.WebRequest.Data("SaturdayFrom")) And IsDate(Common.WebRequest.Data("SaturdayTo")) Then
            p("SaturdayFrom") = ReadyString(CDate(Common.WebRequest.Data("SaturdayFrom")))
            p("SaturdayTo") = ReadyString(CDate(Common.WebRequest.Data("SaturdayTo")))
        Else
            p("SaturdayFrom") = "NULL"
            p("SaturdayTo") = "NULL"
        End If

        'p("Saturday") lunch
        If IsDate(Common.WebRequest.Data("SaturdayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("SaturdayLunchBreakTo")) Then
            p("SaturdayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("SaturdayLunchBreakFrom")))
            p("SaturdayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("SaturdayLunchBreakTo")))
        Else
            p("SaturdayLunchBreakFrom") = "NULL"
            p("SaturdayLunchBreakTo") = "NULL"
        End If

        'p("Saturday") dinner
        If IsDate(Common.WebRequest.Data("SaturdayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("SaturdayDinnerBreakTo")) Then
            p("SaturdayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("SaturdayDinnerBreakFrom")))
            p("SaturdayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("SaturdayDinnerBreakTo")))
        Else
            p("SaturdayDinnerBreakFrom") = "NULL"
            p("SaturdayDinnerBreakTo") = "NULL"
        End If

        'p("Sunday")
        If IsDate(Common.WebRequest.Data("SundayFrom")) And IsDate(Common.WebRequest.Data("SundayTo")) Then
            p("SundayFrom") = ReadyString(CDate(Common.WebRequest.Data("SundayFrom")))
            p("SundayTo") = ReadyString(CDate(Common.WebRequest.Data("SundayTo")))
        Else
            p("SundayFrom") = "NULL"
            p("SundayTo") = "NULL"
        End If

        'p("Sunday")lunch
        If IsDate(Common.WebRequest.Data("SundayLunchBreakFrom")) And IsDate(Common.WebRequest.Data("SundayLunchBreakTo")) Then
            p("SundayLunchBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("SundayLunchBreakFrom")))
            p("SundayLunchBreakTo") = ReadyString(CDate(Common.WebRequest.Data("SundayLunchBreakTo")))
        Else
            p("SundayLunchBreakFrom") = "NULL"
            p("SundayLunchBreakTo") = "NULL"
        End If

        'p("Sunday")dinner
        If IsDate(Common.WebRequest.Data("SundayDinnerBreakFrom")) And IsDate(Common.WebRequest.Data("SundayDinnerBreakTo")) Then
            p("SundayDinnerBreakFrom") = ReadyString(CDate(Common.WebRequest.Data("SundayDinnerBreakFrom")))
            p("SundayDinnerBreakTo") = ReadyString(CDate(Common.WebRequest.Data("SundayDinnerBreakTo")))
        Else
            p("SundayDinnerBreakFrom") = "NULL"
            p("SundayDinnerBreakTo") = "NULL"
        End If

        Try

            If CInt(WebRequest.Instance.Data("IsNew")) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = WebRequest.Instance.Data("ID")
                'p("IsActive") = CInt(Common.WebRequest.Data("IsActive"))
                pSql = p.GetUpdate(" ID = " & ReadyString(strIDReturn))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationWorkingHours'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationWorkingHours'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return strIDReturn
        Catch ex As Exception
            Return "error"
        End Try
    End Function

    Function RemoveEmployeeWorkSchedule() As String
        Dim pSql As String

        pSql = "Delete from LibraryOrganizationWorkScheduleEmployee where ID = " & ReadyString(Common.WebRequest.Data("EmployeeScheduleID"))
        Common.WebRequest.ProData.ExecuteSQL(pSql)
    End Function

    Function GetAllSchedule() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With

                .Add("WorkScheduleName", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
                .Add("MondayFrom", SqlDbType.VarChar)
                .Add("MondayTo", SqlDbType.VarChar)

                .Add("MondayLunchBreakFrom", SqlDbType.VarChar)
                .Add("MondayLunchBreakTo", SqlDbType.VarChar)
                .Add("MondayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("MondayDinnerBreakTo", SqlDbType.VarChar)

                .Add("TuesdayFrom", SqlDbType.VarChar)
                .Add("TuesdayTo", SqlDbType.VarChar)

                .Add("TuesdayLunchBreakFrom", SqlDbType.VarChar)
                .Add("TuesdayLunchBreakTo", SqlDbType.VarChar)
                .Add("TuesdayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("TuesdayDinnerBreakTo", SqlDbType.VarChar)

                .Add("WednesdayFrom", SqlDbType.VarChar)
                .Add("WednesdayTo", SqlDbType.VarChar)

                .Add("WednesdayLunchBreakFrom", SqlDbType.VarChar)
                .Add("WednesdayLunchBreakTo", SqlDbType.VarChar)
                .Add("WednesdayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("WednesdayDinnerBreakTo", SqlDbType.VarChar)

                .Add("ThursdayFrom", SqlDbType.VarChar)
                .Add("ThursdayTo", SqlDbType.VarChar)

                .Add("ThursdayLunchBreakFrom", SqlDbType.VarChar)
                .Add("ThursdayLunchBreakTo", SqlDbType.VarChar)
                .Add("ThursdayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("ThursdayDinnerBreakTo", SqlDbType.VarChar)

                .Add("FridayFrom", SqlDbType.VarChar)
                .Add("FridayTo", SqlDbType.VarChar)

                .Add("FridayLunchBreakFrom", SqlDbType.VarChar)
                .Add("FridayLunchBreakTo", SqlDbType.VarChar)
                .Add("FridayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("FridayDinnerBreakTo", SqlDbType.VarChar)

                .Add("SaturdayFrom", SqlDbType.VarChar)
                .Add("SaturdayTo", SqlDbType.VarChar)

                .Add("SaturdayLunchBreakFrom", SqlDbType.VarChar)
                .Add("SaturdayLunchBreakTo", SqlDbType.VarChar)
                .Add("SaturdayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("SaturdayDinnerBreakTo", SqlDbType.VarChar)

                .Add("SundayFrom", SqlDbType.VarChar)
                .Add("SundayTo", SqlDbType.VarChar)

                .Add("SundayLunchBreakFrom", SqlDbType.VarChar)
                .Add("SundayLunchBreakTo", SqlDbType.VarChar)
                .Add("SundayDinnerBreakFrom", SqlDbType.VarChar)
                .Add("SundayDinnerBreakTo", SqlDbType.VarChar)

                .Add("IsActive", SqlDbType.Bit)
            End With

            .SqlFrom = "FROM LibraryOrganizationWorkSchedule"

            If Not WebRequest.Instance.Data("ID") Is Nothing Then
                .SqlWhere = "WHERE ID = " & ReadyString(WebRequest.Instance.Data("ID"))
            End If

            .SqlOrder = "Order by WorkScheduleName"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveEmployeeWorkingHours() As String
        Dim p As New InsertUpdate("LibraryOrganizationWorkScheduleEmployee")
        Dim pSql As String
        Dim strIDReturn As String

        Dim m_ID As String

        'If Not CBool(Common.WebRequest.Data("IsInfiniteEffectivityDate")) Then
        '    If DateDiff(DateInterval.Day, CDate(Common.WebRequest.Data("EffectivityDateFrom")), CDate(Common.WebRequest.Data("EffectivityDateTo"))) < 0 Then
        '        Return "Error: Invalid effectivity date. 'Effectivity Date From' should not be later than 'Effectivity Date To :'"
        '    ElseIf DateDiff(DateInterval.Day, Today, CDate(Common.WebRequest.Data("EffectivityDateFrom"))) <= 0 Then
        '        Return "Error: 'Effectivity Date From' should be later than date today."
        '    End If
        'End If

        m_ID = GetWorkScheduleEmployeeID()

        p("WorkScheduleID") = ReadyString(Common.WebRequest.Data("ScheduleID"))
        p("PeopleID") = ReadyString(Common.WebRequest.Data("PeopleID"))
        p("FowstID") = ReadyString(Common.WebRequest.Data("FOWSTID"))
        p("EffectivityDateFrom") = ReadyString(CDate(Common.WebRequest.Data("EffectivityDateFrom")))
        p("EffectivityDateTo") = ReadyString(IIf(CBool(Common.WebRequest.Data("IsInfiniteEffectivityDate")), CDate("01/01/3000"), CDate(Common.WebRequest.Data("EffectivityDateTo"))))
        p("IsInfiniteEffectivityDate") = CInt(Common.WebRequest.Data("IsInfiniteEffectivityDate"))
        p("LastUpdated") = "GetDate()"

        Try
            If m_ID.Length = 0 Then
                m_ID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(m_ID)
                pSql = p.GetInsert()
            Else
                p("IsActive") = CInt(Common.WebRequest.Data("IsActive"))
                pSql = p.GetUpdate(" ID = " & ReadyString(m_ID))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationWorkingHours'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationWorkingHours'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return "True"
        Catch ex As Exception
            Return "False"
        End Try
    End Function

    Function GetEmployeeSchedule() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryOrganizationWorkScheduleEmployee.ID"
                End With

                With .Add("WorkScheduleName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryOrganizationWorkSchedule.WorkScheduleName"
                End With

                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("EffectivityDateFrom", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryOrganizationWorkScheduleEmployee.EffectivityDateFrom"
                End With

                With .Add("EffectivityDateTo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryOrganizationWorkScheduleEmployee.EffectivityDateTo"
                End With

                With .Add("IsInfiniteEffectivityDate", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryOrganizationWorkScheduleEmployee.IsInfiniteEffectivityDate"
                End With

            End With

            .SqlFrom = "FROM dbo.LibraryOrganizationWorkSchedule INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationWorkScheduleEmployee ON " & vbCrLf &
                      "dbo.LibraryOrganizationWorkSchedule.ID = dbo.LibraryOrganizationWorkScheduleEmployee.WorkScheduleID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.LibraryOrganizationWorkScheduleEmployee.FowstID = dbo.MobizLibraryOrganizationFOWST.ID"

            .SqlWhere = "WHERE dbo.LibraryOrganizationWorkScheduleEmployee.PeopleID = " & ReadyString(WebRequest.Instance.Data("PeopleID"))

            Return .Requery
        End With

    End Function

    Function GetWorkScheduleEmployeeID() As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
            End With

            .SqlFrom = "FROM LibraryOrganizationWorkScheduleEmployee"


            .SqlWhere = "WHERE WorkScheduleID = " & ReadyString(WebRequest.Instance.Data("ScheduleID")) & vbCrLf &
                        "AND PeopleID = " & ReadyString(WebRequest.Instance.Data("PeopleID")) & vbCrLf &
                        "AND FowstID = " & ReadyString(WebRequest.Instance.Data("FOWSTID"))

            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0).Item("ID"))
        Else
            Return ""
        End If
    End Function

    Function GetWorkingScheduleID() As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                .Add("WorkScheduleID", SqlDbType.UniqueIdentifier)
            End With

            .SqlFrom = "FROM LibraryOrganizationWorkScheduleEmployee"


            .SqlWhere = "WHERE PeopleID = " & ReadyString(WebRequest.Instance.Data("PeopleID")) & vbCrLf &
                        "AND FowstID = " & ReadyString(WebRequest.Instance.Data("FOWSTID"))

            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0).Item("WorkScheduleID"))
        Else
            Return ""
        End If
    End Function

    Function AddOrgOwnerProductList(ByVal brandOwnerID As String) As Integer
        Dim p As New InsertUpdate("LibraryOrganizationPIM")
        Dim p2 As New InsertUpdate("MobizLibraryPIMProductPriceLevel")

        Dim pSql As String
        Dim pSql2 As String
        Dim pFinder As Finder
        Dim pCount As Integer

        Dim pDT As DataTable

        Try
            pSql = "SELECT prcLvl.ProductID, prcLvl.price from MobizLibraryPIMProductPriceLevel prcLvl" &
                 " INNER JOIN MobizLibraryPIMPriceLevel prc ON prcLvl.PriceLevelID = prc.ID WHERE " &
                 " prc.PriceLevel = 'RETAIL PRICE' AND prcLvl.OrganizationID = " & ReadyString(brandOwnerID) &
                 " AND prcLvl.ProductID IN (SELECT pim.ID " &
                 " FROM MobizLibraryPIMProduct pim LEFT JOIN LibraryOrganizationPIM orgPIM ON pim.ID = orgPIM.ProductID " &
                 " LEFT JOIN MobizLibraryPIMProductPriceLevel prcLvl ON prcLvl.ProductID = orgPIM.ProductID " &
                 " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1)"

            pDT = Common.WebRequest.ProData.Execute(pSql)


            If pDT.Rows.Count > 0 Then

                For Each pDR As DataRow In pDT.Rows

                    'add to private LibraryOrganizationPIM
                    p("ProductID") = ReadyString(pDR("ProductID"))
                    p("ActivationDate") = "CONVERT(date, getdate())"

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)


                    'RETAIL PRICE
                    'add to public pricelevel MobizLibraryPIMProductPriceLevel
                    p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                    p2("ProductID") = ReadyString(pDR("ProductID"))
                    p2("PriceLevelID") = ReadyString("01000012-0005-0000-0000-000000000000")
                    p2("Price") = Convert.ToDouble(pDR("Price"))
                    p2("DeactivateDiscount") = 0
                    p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                    p2("LastUpdated") = "getdate()"

                    p2("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql2 = p2.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql2)

                    'FREIGHT COST
                    'add to public pricelevel MobizLibraryPIMProductPriceLevel
                    'DEFAULT TO 0 because we dont know yet. must be changed by the org later
                    p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                    p2("ProductID") = ReadyString(pDR("ProductID"))
                    p2("PriceLevelID") = ReadyString("01000012-0001-0000-0000-000000000000")
                    p2("Price") = "0"
                    p2("DeactivateDiscount") = 0
                    p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                    p2("LastUpdated") = "getdate()"

                    p2("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql2 = p2.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql2)


                    'COST FROM VENDOR
                    'add to public pricelevel MobizLibraryPIMProductPriceLevel
                    'DEFAULT TO 0 because we dont know yet. must be changed by the org later
                    p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                    p2("ProductID") = ReadyString(pDR("ProductID"))
                    p2("PriceLevelID") = ReadyString("01000012-0002-0000-0000-000000000000")
                    p2("Price") = "0"
                    p2("DeactivateDiscount") = 0
                    p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                    p2("LastUpdated") = "getdate()"

                    p2("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql2 = p2.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql2)


                    'SRP (Suggested Retail Price)
                    'add to public pricelevel MobizLibraryPIMProductPriceLevel
                    p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                    p2("ProductID") = ReadyString(pDR("ProductID"))
                    p2("PriceLevelID") = ReadyString("EEF716C6-B290-4B28-B2A6-0A4F52CF4989")
                    p2("Price") = Convert.ToDouble(pDR("Price"))
                    p2("DeactivateDiscount") = 0
                    p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                    p2("LastUpdated") = "getdate()"

                    p2("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql2 = p2.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql2)


                Next pDR

            End If

            pCount += CopyBom(brandOwnerID)
            pCount += CopyPimParent(brandOwnerID)
            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            pCount += pDT.Rows.Count

            Return pCount

        Catch ex As Exception
            AppendToLog("vbXSGHBKMSUY! " & GetStackTrace(, ex), , "AddOrgOwnerProductList")
            Return 0
        End Try
    End Function
    Private Function CopyBom(vBrandOwnerID As String) As Integer
        Dim pDT As DataTable
        Dim pSql As String
        Dim pIU As InsertUpdate
        pSql = " DECLARE @BrandOwnerID UNIQUEIDENTIFIER = " & ReadyString(vBrandOwnerID) & vbCrLf &
            " DECLARE @DestinationID UNIQUEIDENTIFIER = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf
        pSql &= " SELECT OwnerContents.*" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN MobizLibraryPIMProductContents OwnerContents ON OwnerContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " AND OwnerContents.OrganizationID = @BrandOwnerID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProductContents DestinationContents ON DestinationContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " AND DestinationContents.OrganizationID = @DestinationID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.PublishedByID = @BrandOwnerID" & vbCrLf &
            " AND DestinationContents.ID IS NULL"
        pDT = Common.WebRequest.ProData.Execute(pSql)
        AppendToLog("vbWVW88T3CF2! No. of records: " & pDT.Rows.Count & vbCrLf & pSql, , "CopyBom")
        pIU = New InsertUpdate("MobizLibraryPIMProductContents")
        pIU("ID") = "NEWID()"
        pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
        For Each pDR As DataRow In pDT.Rows
            pIU("ParentProductID") = ReadyString(pDR("ParentProductID"))
            pIU("ChildProductID") = ReadyString(pDR("ChildProductID"))
            pIU("Quantity") = CDec2(pDR("Quantity"))
            If pDR("UnitMeasureID") Is DBNull.Value Then
                pIU.Remove("UnitMeasureID")
            Else
                pIU("UnitMeasureID") = ReadyString(pDR("UnitMeasureID"))
            End If
            pIU("UnitMeasure") = ReadyString(pDR("UnitMeasure"))
            pSql = pIU.GetInsert()
            Try
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Catch ex As Exception
                AppendToLog("vb3F3UD8EA5H! " & ex.Message & vbCrLf & pIU.GetUpdate(""), , "CopyBom")
            End Try
        Next pDR
        Return pDT.Rows.Count
    End Function
    Private Function CopyPimParent(vBrandOwnerID As String) As Integer
        Dim pDT As DataTable
        Dim pSql As String
        Dim pIU As InsertUpdate
        pSql = " DECLARE @BrandOwnerID UNIQUEIDENTIFIER = " & ReadyString(vBrandOwnerID) & vbCrLf &
            " DECLARE @DestinationID UNIQUEIDENTIFIER = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf
        pSql &= " SELECT OwnerParent.*" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN MobizLibraryPIMProductParent OwnerParent ON OwnerParent.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " AND OwnerParent.OrganizationID = @BrandOwnerID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProductParent DestinationParent ON DestinationParent.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " AND DestinationParent.OrganizationID = @DestinationID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.PublishedByID = @BrandOwnerID" & vbCrLf &
            " AND DestinationParent.ID IS NULL"
        pDT = Common.WebRequest.ProData.Execute(pSql)
        AppendToLog("vb7JDRSQWMMA! No. of records: " & pDT.Rows.Count & vbCrLf & pSql, , "CopyPimParent")
        pIU = New InsertUpdate("MobizLibraryPIMProductParent")
        pIU("ID") = "NEWID()"
        pIU("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
        For Each pDR As DataRow In pDT.Rows
            pIU("ProductID") = ReadyString(pDR("ProductID"))
            pIU("Jit") = If(CBool2(pDR("Jit")), 1, 0)
            pIU("Instructions") = ReadyString(pDR("Instructions"))
            pIU("UserID") = ReadyString(pDR("UserID"))
            pIU("LastUpdated") = ReadyString(pDR("LastUpdated"))
            pSql = pIU.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Next pDR
        Return pDT.Rows.Count

    End Function

    Function GetBrand(ByVal brandOwnerID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "ISNULL(CASE WHEN pim.Brand = '' THEN 'N/A' ELSE pim.Brand END,'N/A')"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProduct pim LEFT JOIN LibraryOrganizationPIM orgPIM ON pim.ID = orgPIM.ProductID " &
                 " LEFT JOIN MobizLibraryPIMProductPriceLevel prcLvl ON prcLvl.ProductID = orgPIM.ProductID "

            .SqlWhere = " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1"
            .SqlOrder = "ORDER BY Label"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetCategory(ByVal brandOwnerID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "ISNULL(CASE WHEN feat.c0020 = '' THEN 'N/A' ELSE feat.c0020 END,'N/A')"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMProduct pim LEFT JOIN LibraryOrganizationPIM orgPIM ON pim.ID = orgPIM.ProductID " &
                 " LEFT JOIN MobizLibraryPIMProductPriceLevel prcLvl ON prcLvl.ProductID = orgPIM.ProductID " &
                    " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID "
            .SqlWhere = " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1"
            .SqlOrder = "ORDER BY Label"


            pDT = .Requery


        End With

        Return pDT
    End Function

    Function GetProductNames(ByVal brandOwnerID As String) As DataTable

        Dim pSql As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim count As Integer = 1

        Try

            For Each pKeyword As Object In KeywordList
                pSql &= "SELECT pim.ProductNo, pim.Brand, pim.ProductName from MobizLibraryPIMProductPriceLevel prcLvl" &
                 " INNER JOIN MobizLibraryPIMPriceLevel prc ON prcLvl.PriceLevelID = prc.ID  " &
                 " INNER JOIN MobizLibraryPIMProduct pim ON pim.ID = prcLvl.ProductID WHERE " &
                 " prc.PriceLevel = 'RETAIL PRICE' AND prcLvl.OrganizationID = " & ReadyString(brandOwnerID) &
                 " AND prcLvl.ProductID IN (SELECT pim.ID " &
                 " FROM MobizLibraryPIMProduct pim LEFT JOIN LibraryOrganizationPIM orgPIM ON pim.ID = orgPIM.ProductID " &
                 " LEFT JOIN MobizLibraryPIMProductPriceLevel prcLvl ON prcLvl.ProductID = orgPIM.ProductID "

                If pKeyword.Trim() <> "" Then

                    If FilterName.Trim() = "Brand" Then
                        pSql &= " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1 "
                        If (pKeyword.Trim() = "N/A") Then
                            pSql &= " AND pim.Brand = '')"
                        Else
                            pSql &= " AND pim.Brand =" & ReadyString(pKeyword.Trim()) & ")"
                        End If

                    Else
                        pSql &= " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID " &
                        " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1 "

                        If (pKeyword.Trim() = "N/A") Then
                            pSql &= " AND feat.C0020 ='' )"
                        Else
                            pSql &= " AND feat.C0020 =" & ReadyString(pKeyword.Trim()) & ")"
                        End If
                    End If

                End If
                'if greater than 1 and not last element
                If KeywordList.Length > 1 And count < KeywordList.Length Then
                    pSql &= " UNION "
                End If


                count = count + 1

            Next
            pDT = Common.WebRequest.ProData.Execute(pSql)


            Return pDT
        Catch ex As Exception

        End Try

    End Function


    Function AddOrgOwnerProductListCustom(ByVal brandOwnerID As String) As Integer
        Dim p As New InsertUpdate("LibraryOrganizationPIM")
        Dim p2 As New InsertUpdate("MobizLibraryPIMProductPriceLevel")

        Dim pSql As String
        Dim pSql2 As String
        Dim pFinder As Finder

        Dim pDT As DataTable
        Dim counter As Integer = 0
        Try

            For Each pKeyword As Object In KeywordList
                pSql = "SELECT prcLvl.ProductID, prcLvl.price from MobizLibraryPIMProductPriceLevel prcLvl" &
                 " INNER JOIN MobizLibraryPIMPriceLevel prc ON prcLvl.PriceLevelID = prc.ID WHERE " &
                 " prc.PriceLevel = 'RETAIL PRICE' AND prcLvl.OrganizationID = " & ReadyString(brandOwnerID) &
                 " AND prcLvl.ProductID IN (SELECT pim.ID " &
                 " FROM MobizLibraryPIMProduct pim LEFT JOIN LibraryOrganizationPIM orgPIM ON pim.ID = orgPIM.ProductID " &
                 " LEFT JOIN MobizLibraryPIMProductPriceLevel prcLvl ON prcLvl.ProductID = orgPIM.ProductID "

                If pKeyword.Trim() <> "" Then

                    If FilterName.Trim() = "Brand" Then
                        pSql &= " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1 "
                        If (pKeyword.Trim() = "N/A") Then
                            pSql &= " AND pim.Brand = '')"
                        Else
                            pSql &= " AND pim.Brand =" & ReadyString(pKeyword.Trim()) & ")"
                        End If

                    Else
                        pSql &= " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID " &
                        " WHERE orgPIM.ID IS NULL AND prcLvl.ID IS NULL AND pim.PublishedByID = " & ReadyString(brandOwnerID) & " AND pim.IsActive = 1 "

                        If (pKeyword.Trim() = "N/A") Then
                            pSql &= " AND feat.C0020 ='' )"
                        Else
                            pSql &= " AND feat.C0020 =" & ReadyString(pKeyword.Trim()) & ")"
                        End If
                    End If

                End If

                pDT = Common.WebRequest.ProData.Execute(pSql)

                'count all records
                counter = counter + pDT.Rows.Count

                If pDT.Rows.Count > 0 Then

                    For Each pDR As DataRow In pDT.Rows

                        'add to private LibraryOrganizationPIM
                        p("ProductID") = ReadyString(pDR("ProductID"))
                        p("ActivationDate") = "CONVERT(date, getdate())"

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql)


                        'RETAIL PRICE
                        'add to public pricelevel MobizLibraryPIMProductPriceLevel
                        p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                        p2("ProductID") = ReadyString(pDR("ProductID"))
                        p2("PriceLevelID") = ReadyString("01000012-0005-0000-0000-000000000000")
                        p2("Price") = Convert.ToDouble(pDR("Price"))
                        p2("DeactivateDiscount") = 0
                        p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                        p2("LastUpdated") = "getdate()"

                        p2("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql2 = p2.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql2)

                        'FREIGHT COST
                        'add to public pricelevel MobizLibraryPIMProductPriceLevel
                        'DEFAULT TO 0 because we dont know yet. must be changed by the org later
                        p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                        p2("ProductID") = ReadyString(pDR("ProductID"))
                        p2("PriceLevelID") = ReadyString("01000012-0001-0000-0000-000000000000")
                        p2("Price") = "0"
                        p2("DeactivateDiscount") = 0
                        p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                        p2("LastUpdated") = "getdate()"

                        p2("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql2 = p2.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql2)


                        'COST FROM VENDOR
                        'add to public pricelevel MobizLibraryPIMProductPriceLevel
                        'DEFAULT TO 0 because we dont know yet. must be changed by the org later
                        p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                        p2("ProductID") = ReadyString(pDR("ProductID"))
                        p2("PriceLevelID") = ReadyString("01000012-0002-0000-0000-000000000000")
                        p2("Price") = "0"
                        p2("DeactivateDiscount") = 0
                        p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                        p2("LastUpdated") = "getdate()"

                        p2("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql2 = p2.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql2)


                        'SRP (Suggested Retail Price)
                        'add to public pricelevel MobizLibraryPIMProductPriceLevel
                        p2("OrganizationID") = ReadyString(WebSession.Instance.OrganizationID)
                        p2("ProductID") = ReadyString(pDR("ProductID"))
                        p2("PriceLevelID") = ReadyString("EEF716C6-B290-4B28-B2A6-0A4F52CF4989")
                        p2("Price") = Convert.ToDouble(pDR("Price"))
                        p2("DeactivateDiscount") = 0
                        p2("UserID") = ReadyString(WebSession.Instance().PeopleUserID)
                        p2("LastUpdated") = "getdate()"

                        p2("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql2 = p2.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql2)


                    Next pDR

                End If

            Next

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Return counter

        Catch ex As Exception

            Return 0
        End Try

    End Function

    Function SaveFowstBinSection() As String
        Dim p As New InsertUpdate("LibraryFowstBin")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable


        pSql = "SELECT ID, SectionID FROM LibraryFowstBin WHERE ID = " & ReadyString(ID)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If (pDT.Rows.Count > 0) Then
            If (CStr2(pDT.Rows(0).Item("SectionID")) = "") Then
                p("SectionID") = ReadyString(SectionID)
                p("LastUpdated") = "getdate()"

                pSql = p.GetUpdate("ID = " & ReadyString(ID))

                Common.WebRequest.ProData.ExecuteSQL(pSql)
                Return "ok"
            Else
                If (CBool2(Common.WebRequest.Data("IsMove"))) Then

                    p("SectionID") = ReadyString(SectionID)
                    p("LastUpdated") = "getdate()"

                    pSql = p.GetUpdate("ID = " & ReadyString(ID))

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Return "ok"

                Else
                    If (pDT.Rows(0).Item("SectionID").ToString() = SectionID) Then
                        Return "ok"
                    Else
                        Return "existing"

                    End If

                End If

            End If

        End If


    End Function
    Function RemoveFowstBinSection() As String
        Dim p As New InsertUpdate("LibraryFowstBin")
        Dim pSql As String

        p("SectionID") = "NULL"
        p("LastUpdated") = "getdate()"

        pSql = p.GetUpdate("ID = " & ReadyString(ID))

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return "ok"

    End Function
    Function SaveFowstSectionPeople() As String
        Dim p As New InsertUpdate("LibraryFowstSectionPeople")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable

        pSql = "SELECT ID, IsActive FROM LibraryFowstSectionPeople WHERE PeopleID = " & ReadyString(PeopleID) &
            " AND SectionID = " & ReadyString(SectionID) & " AND FowstID =" & ReadyString(FowstID)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        p("FowstID") = ReadyString(FowstID)
        p("SectionID") = ReadyString(SectionID)
        p("PeopleID") = ReadyString(PeopleID)
        p("IsActive") = 1
        p("LastUpdated") = "getdate()"


        If (pDT.Rows.Count < 1) Then

            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Else

            If (Not CBool2(pDT.Rows(0).Item("IsActive").ToString())) Then

                pSql = p.GetUpdate("ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

        End If

        Return "ok"

    End Function
    Function RemoveFowstSectionPeople() As String
        Dim p As New InsertUpdate("LibraryFowstSectionPeople")
        Dim pSql As String

        p("IsActive") = 0
        p("LastUpdated") = "getdate()"

        pSql = p.GetUpdate("ID = " & ReadyString(ID))

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return "ok"

    End Function

    Function GetOrgFOWSTSectionBinList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
            End With
            With .Fields
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryFowstBin"

            .SqlWhere = "WHERE SectionID = " & ReadyString(SectionID) & vbCrLf &
                        "AND FowstID = " & ReadyString(FowstID)
            .SqlOrder = " ORDER BY Name"
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetOrgFOWSTSectionPeopleList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True


            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "sec.ID"
                    .Searchable = False
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "ppl.Surname + ', ' + ppl.FirstName"
                End With
            End With

            .SqlFrom = "FROM LibraryFowstSectionPeople sec INNER JOIN MobizLibraryPeople ppl ON ppl.ID = sec.PeopleID"

            .SqlWhere = " WHERE SectionID = " & ReadyString(SectionID) & vbCrLf &
                        " AND FowstID = " & ReadyString(FowstID) & " AND sec.IsActive = 1"
            .SqlOrder = " ORDER BY Name"
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetFOWSTQuota() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("FOWSTID", SqlDbType.VarChar)
                .Add("MonthYear", SqlDbType.VarChar)
                .Add("Tier1Quota", SqlDbType.VarChar)
                .Add("Tier1Incentive", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM LibraryOrganizationFOWSTQuota "
            .SqlWhere = " WHERE LibraryOrganizationFOWSTQuota.FOWSTID = " & ReadyString(CStr(Common.WebRequest.Data("ID")))

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function getOrganizationCustomerList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryOrganization.ID"
                    .Searchable = False
                End With

                With .Add("Organization Name", SqlDbType.VarChar)
                    .Expression = "CompanyName"
                End With

                With .Add("Complete Name", SqlDbType.VarChar)
                    .Expression = "''"
                End With
                With .Add("First Name", SqlDbType.VarChar)
                    .Expression = "PersonFirstName"
                End With
                With .Add("Middle Name", SqlDbType.VarChar)
                    .Expression = "PersonMiddleName"
                End With
                With .Add("Last Name", SqlDbType.VarChar)
                    .Expression = "PersonSurname"
                End With
                With .Add("Person Generation Suffix", SqlDbType.VarChar)
                    .Expression = "ISNULL(PersonSuffixName, '')"
                End With

                With .Add("Country Code1", SqlDbType.VarChar)
                    .Expression = "ContactMobile.CallingCode"
                End With

                With .Add("Area or Telco Code1", SqlDbType.VarChar)
                    .Expression = "ContactMobile.Prefix"
                End With

                With .Add("Number1", SqlDbType.VarChar)
                    .Expression = "ContactMobile.ContactNo"
                End With

                With .Add("Country Code2", SqlDbType.VarChar)
                    .Expression = "ContactTel.CallingCode"
                End With

                With .Add("Area or Telco Code2", SqlDbType.VarChar)
                    .Expression = "ContactTel.AreaCode"
                End With

                With .Add("Number2", SqlDbType.VarChar)
                    .Expression = "ContactTel.ContactNo"
                End With

                With .Add("Email Address", SqlDbType.VarChar)
                    .Expression = "ContactEMail.EmailAddress"
                End With

                With .Add("Website", SqlDbType.VarChar)
                    .Expression = "ContactWebsite.Website"
                End With

                With .Add("Building No Address", SqlDbType.VarChar)
                    .Expression = "officeaddressbuildingno"
                End With

                With .Add("Street Address", SqlDbType.VarChar)
                    .Expression = "officeaddressstreet"
                End With

                With .Add("City Address ID", SqlDbType.UniqueIdentifier)
                    .Expression = "officeaddresscityID"
                End With

                With .Add("City Address", SqlDbType.VarChar)
                    .Expression = "MobizLibraryCities.Name"
                End With

                With .Add("Country Address ID", SqlDbType.UniqueIdentifier)
                    .Expression = "officeaddresscountryid"
                End With

                With .Add("Country Address", SqlDbType.VarChar)
                    .Expression = "MobizLibraryCountries.Name"
                End With

                With .Add("Business Industry", SqlDbType.VarChar)
                    .Expression = "Class"
                End With

                With .Add("Business Industry ID", SqlDbType.UniqueIdentifier)
                    .Expression = "IndustryClassID"
                End With

                With .Add("Contact Person FirstName", SqlDbType.VarChar)
                    .Expression = "Representative.FirstName"
                End With

                With .Add("Contact Person MiddleName", SqlDbType.VarChar)
                    .Expression = "Representative.MiddleName"
                End With

                With .Add("Contact Person LastName", SqlDbType.VarChar)
                    .Expression = "Representative.LastName"
                End With

                With .Add("Interested In", SqlDbType.VarChar)
                    .Expression = "''"
                End With

                With .Add("Contact Source", SqlDbType.VarChar)
                    .Expression = "''"
                End With

                With .Add("Contact Owner", SqlDbType.VarChar)
                    .Expression = "''"
                End With
            End With

            .SqlFrom = "from MobizLibraryOrganization  " & vbCrLf &
                    "inner join LibraryOrganizationCustomer on MobizLibraryOrganization.id = LibraryOrganizationCustomer.OrganizationID " & vbCrLf &
                    "left join MobizLibraryCities on MobizLibraryCities.ID = officeaddresscityID " & vbCrLf &
                    "left join MobizLibraryCountries on MobizLibraryCountries.ID = officeaddresscountryid " & vbCrLf &
                    "left join MobizLibraryOrganizationIndustryClass on MobizLibraryOrganizationIndustryClass.OrganizationID = MobizLibraryOrganization.id " & vbCrLf &
                    "left join MobizLibraryIndustryClass on MobizLibraryIndustryClass.ID = MobizLibraryOrganizationIndustryClass.IndustryClassID " & vbCrLf &
                    "outer apply " & vbCrLf &
                    "(Select top 1 MobizLibraryTelcoPrefixes.Prefix, MobizLibraryCountries.CallingCode, TelCoPrefixID, ContactNo from LibraryOrganizationContactMobile  " & vbCrLf &
                    "inner join MobizLibraryCountries on MobizLibraryCountries.ID = LibraryOrganizationContactMobile.CountryID " & vbCrLf &
                    "inner join MobizLibraryTelcoPrefixes on MobizLibraryTelcoPrefixes.ID = LibraryOrganizationContactMobile.TelCoPrefixID " & vbCrLf &
                    "where OrganizationID = MobizLibraryOrganization.id ) ContactMobile " & vbCrLf &
                    "outer apply " & vbCrLf &
                    "(Select top 1 MobizLibraryCountries.CallingCode, AreaCodeID, ContactNo, MobizLibraryAreaCodes.CallingCode as AreaCode from LibraryOrganizationContactTel  " & vbCrLf &
                    "inner join MobizLibraryCountries on MobizLibraryCountries.ID = LibraryOrganizationContactTel.CountryID " & vbCrLf &
                    "inner join MobizLibraryAreaCodes on MobizLibraryAreaCodes.ID = LibraryOrganizationContactTel.AreaCodeID " & vbCrLf &
                    "where OrganizationID = MobizLibraryOrganization.id) ContactTel " & vbCrLf &
                    "outer apply " & vbCrLf &
                    "(Select top 1 EmailAddress from LibraryOrganizationContactEmail " & vbCrLf &
                    "where OrganizationID = MobizLibraryOrganization.id) ContactEMail " & vbCrLf &
                    "outer apply " & vbCrLf &
                    "(Select top 1 Website from LibraryOrganizationContactWebsite  " & vbCrLf &
                    "where OrganizationID = MobizLibraryOrganization.id) ContactWebsite " & vbCrLf &
                    "outer apply " & vbCrLf &
                    "(Select top 1 LastName, FirstName, MiddleName from MobizLibraryOrganizationRepresentatives  " & vbCrLf &
                    "where OrganizationID = MobizLibraryOrganization.id) Representative " & vbCrLf &
                    "ORDER BY CompanyName ASC, PersonSurName ASC"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetJson(ByVal dt As DataTable) As String
        Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
    End Function

    Function GetFowst() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE orgadd.OrganizationID = " & ReadyString(OrganizationID) & vbCrLf &
                                    " AND ISNULL(orgadd.IsActive, 1) = " & CInt2(IsActive) & vbCrLf &
                                    " AND ISNULL(CONVERT(VARCHAR(36), orgadd.FowstTypeID), '') <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b' "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If Not String.IsNullOrWhiteSpace(FOWSTStatus) Then
            whereSearch &= " AND (ISNULL(orgadd.Status, 'Store and device are operational.') = " & ReadyString(FOWSTStatus) & vbCrLf

            If FOWSTStatus = "Store and device are operational." Then
                'Available
                whereSearch &= " OR ISNULL(orgadd.Status, 'Store and device are operational.') = 'Available'" & vbCrLf
            ElseIf FOWSTStatus = "Store is permanently closed." Then
                'Closed
                whereSearch &= " OR ISNULL(orgadd.Status, 'Store and device are operational.') = 'Closed'" & vbCrLf
            ElseIf FOWSTStatus = "Store is temporarily closed for holiday." Then
                'Termporary Closed
                whereSearch &= " OR ISNULL(orgadd.Status, 'Store and device are operational.') = 'Temporary Closed'" & vbCrLf
            End If

            whereSearch &= ")" & vbCrLf
        End If

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(orgadd.Name, '') + ' ' +" & vbCrLf &
                                "ISNULL(orgadd.Code, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = whereSearch & " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.LastUpdated " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT DISTINCT orgadd.ID AS [ID] " & vbCrLf &
                ", orgadd.FOWSTTypeID AS [FOWSTTypeID] " & vbCrLf &
                ", orgadd.Code AS [Code] " & vbCrLf &
                ", orgadd.Name AS [Name] " & vbCrLf &
                ", orgadd.Description AS [Description] " & vbCrLf &
                ", orgadd.BusinessTradeNameID AS [BusinessTradeNameID] " & vbCrLf &
                ", custtrade.BusinessTradeName AS [BusinessTradeName] " & vbCrLf &
                ", orgadd.StreetNumber AS [StreetNumber] " & vbCrLf &
                ", orgadd.Street AS [Street] " & vbCrLf &
                ", orgadd.FowstCityID AS [FowstCityID] " & vbCrLf &
                ", cty.Name AS [FowstCity] " & vbCrLf &
                ", orgadd.FowstCountryID AS [FowstCountryID] " & vbCrLf &
                ", ctry.Name AS [FowstCountry] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(orgadd.StreetNumber)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(orgadd.Street)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '') AS [CompleteFowstAddress] " & vbCrLf &
                ", orgadd.SubFOWSTTypeID AS [SubFOWSTTypeID] " & vbCrLf &
                ", rtype.Name AS [RetailType] " & vbCrLf &
                ", wtype.Name AS [WarehouseType] " & vbCrLf &
                ", orgadd.PlateNo AS [PlateNo] " & vbCrLf &
                ", orgadd.NoOfWheels AS [NoOfWheels] " & vbCrLf &
                ", orgadd.BodyColor AS [BodyColor] " & vbCrLf &
                ", orgadd.Weight AS [Weight] " & vbCrLf &
                ", orgadd.Dimension AS [Dimension] " & vbCrLf &
                ", orgadd.OwnershipType AS [OwnershipType] " & vbCrLf &
                ", orgadd.ContractStart AS [ContractStart] " & vbCrLf &
                ", orgadd.ContractEnd AS [ContractEnd] " & vbCrLf &
                ", orgadd.CostPerMonth AS [CostPerMonth] " & vbCrLf &
                ", orgadd.IsMortgaged AS [IsMortgaged] " & vbCrLf &
                ", orgadd.IsFranchisee AS [IsFranchisee] " & vbCrLf &
                ", orgadd.FranchisorID AS [FranchisorID] " & vbCrLf &
                ", ISNULL(orgadd.IsActive, 1) AS [IsActive] " & vbCrLf &
                ", orgadd.LastUpdated AS [LastUpdated] " & vbCrLf &
                "-- ,tblLastTransaction.LastUpdated AS LastTransactionDate -- commented 2021_11_18__14_11_11_0234" & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '')  AS [ModifiedBy] " & vbCrLf &
                " FROM dbo.MobizLibraryOrganizationFOWST orgadd " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryOrganizationTradeName custtrade ON (orgadd.BusinessTradeNameID = custtrade.ID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryCities cty ON (orgadd.FowstCityID = cty.ID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgadd.FowstCountryID = ctry.ID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryFOWSTType rtype ON (orgadd.SubFOWSTTypeID = rtype.ID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgadd.SubFOWSTTypeID = wtype.ID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTPOSSettings posset ON (orgadd.ID = posset.FOWSTID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryPeople ppol ON (orgadd.UserID = ppol.ID) " & vbCrLf &
                "  /* -- commented 2021_11_18__14_11_11_0234" & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "  	SELECT TOP 1 LastUpdated" & vbCrLf &
                "     FROM (" & vbCrLf &
                "       	SELECT LastUpdated" & vbCrLf &
                "       	FROM TransInventoryLedger" & vbCrLf &
                "       	WHERE TransInventoryLedger.FowstID = orgadd.ID" & vbCrLf &
                "     ) t" & vbCrLf &
                "     ORDER BY LastUpdated DESC" & vbCrLf &
                "  ) tblLastTransaction" & vbCrLf &
                "  */" & vbCrLf &
                "" & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetDevice() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = "WHERE dev.IsActive = " & If(CBool2(IsActive), 1, 0) & vbCrLf
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(dev.PhoneName, '') + ' ' + " & vbCrLf &
                                "ISNULL(dev.PhoneModel, '') + ' ' + " & vbCrLf &
                                "ISNULL(dev.IMEI, '') + ' ' + " & vbCrLf &
                                "ISNULL(dev.UUID, '') + ' ' + " & vbCrLf &
                                "ISNULL(dev.OS, '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), dev.DeviceNo), '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), dev.IsActive), '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), dev.LastUpdated), '') + ' ' + " & vbCrLf &
                                "ISNULL(ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') , '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.PhoneName " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY mainsql.IsActive " & PageColumnDirection
        ElseIf PageColumnNo = 6 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 7 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT dev.ID AS [ID] " & vbCrLf &
                ", dev.PhoneName AS [PhoneName] " & vbCrLf &
                ", dev.PhoneModel AS [PhoneModel] " & vbCrLf &
                ", dev.IMEI AS [IMEI] " & vbCrLf &
                ", dev.UUID AS [UUID] " & vbCrLf &
                ", dev.OS AS [OS] " & vbCrLf &
                ", dev.DeviceNo AS [DeviceNo] " & vbCrLf &
                ", dev.IsActive AS [IsActive] " & vbCrLf &
                ", dev.LastUpdated AS [LastUpdated] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '')  AS [ModifiedBy] " & vbCrLf &
                " FROM LibraryDevice dev " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryPeople ppol ON (dev.PeopleID = ppol.ID) " & vbCrLf &
                "" & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        Try
            pDT = Common.WebRequest.ProData.Execute(pSql)
        Catch ex As Exception
            AppendToLog("vbSK3MZL6GZE! " & ex.Message & vbCrLf & pSql)
            Throw
        End Try

        Return pDT
    End Function

    Function GetReceipt() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(org.CompanyName, '') + ' ' + " & vbCrLf &
                                "ISNULL(orgtrade.BusinessTradeName, '') + ' ' + " & vbCrLf &
                                "ISNULL(org.TinNumber, '') + ' ' + " & vbCrLf &
                                "ISNULL(rset.PaperSize, '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), rset.LastUpdated), '') + ' ' + " & vbCrLf &
                                "ISNULL(ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') , '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = "WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.CompanyName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.BusinessTradeName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.PaperSize " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT rset.ID AS [ID] " & vbCrLf &
                ", org.CompanyName AS [CompanyName] " & vbCrLf &
                ", rset.OrgTradeNameID AS [OrgTradeNameID] " & vbCrLf &
                ", orgtrade.BusinessTradeName AS [BusinessTradeName] " & vbCrLf &
                ", org.TinNumber AS [TIN] " & vbCrLf &
                ", rset.PaperSize AS [PaperSize] " & vbCrLf &
                ", rset.LastUpdated AS [LastUpdated] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '')  AS [ModifiedBy] " & vbCrLf &
                " FROM LibraryReceiptSettings rset " & vbCrLf &
                " INNER JOIN dbo.MobizLibraryOrganizationTradeName orgtrade ON (rset.OrgTradeNameID = orgtrade.ID) " & vbCrLf &
                " INNER JOIN dbo.MobizLibraryOrganization org ON (orgtrade.OrganizationID = org.ID) " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryPeople ppol ON (rset.UserID = ppol.ID) " & vbCrLf &
                "" & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetSecurityRole() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = "WHERE (secrole.IsCompleteRetail = 'True' OR secrole.IsCompleteRetail IS NULL) AND secrole.IsActive = " & If(CBool2(IsActive), 1, 0)
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(CONVERT(varchar(MAX), secrole.ID), '') + ' ' + " & vbCrLf &
                                "ISNULL(secrole.Code, '') + ' ' + " & vbCrLf &
                                "ISNULL(secrole.Role, '') + ' ' + " & vbCrLf &
                                "ISNULL(secrole.Description, '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), secrole.IsActive), '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), secrole.LastUpdated), '') + ' ' + " & vbCrLf &
                                "ISNULL(ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') , '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = whereSearch & " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Code " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Role " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        End If

        'Temporary for BIR demo only, dili ipakita ang Super Admin Access if lain user ang naka login
        'Only Mobiz people lang ang pwede maka add og "Super Admin" access role
        'by Arnie 3112026
        If WebRequest.Instance.OrgID.ToLower() = "565c260c-3844-4069-ac41-d26f0598e562" Then
            Dim isadmin As Integer = Common.WebRequest.ProData.Execute("SELECT * FROM LibrarySecurityRoleUser WHERE RoleID = '474f9f49-2876-4391-a005-107eb18f2762' AND PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)).Rows.Count

            If isadmin = 0 Then
                'BIR DB OrgID
                whereSearch += " AND secrole.ID NOT IN ('474f9f49-2876-4391-a005-107eb18f2762') "
            End If
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT secrole.ID AS [ID] " & vbCrLf &
                ", secrole.Code AS [Code] " & vbCrLf &
                ", secrole.Role AS [Role] " & vbCrLf &
                ", secrole.Description AS [Description] " & vbCrLf &
                ", secrole.IsActive AS [IsActive] " & vbCrLf &
                ", secrole.LastUpdated AS [LastUpdated] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '')  AS [ModifiedBy] " & vbCrLf &
                " FROM LibrarySecurityRole secrole " & vbCrLf &
                " LEFT JOIN dbo.MobizLibraryPeople ppol ON (secrole.UserID = ppol.ID) " & vbCrLf &
                "" & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetEmployee() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                If PageSearchText = "InActive" OrElse PageSearchText = "Active" Then
                    whereSearchContent = "(case when DeActivated=0 THEN 'Active' when DeActivated=1 THEN 'InActive' end) = " & ReadyString(PageSearchText)
                Else
                    whereSearchContent = whereSearchContent & operators & " ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ' ' + " & vbCrLf &
                                "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                                "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ' ' + " & vbCrLf &
                                "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                End If

            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.PeopleName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT DISTINCT orgemp.ID AS [ID] " & vbCrLf &
                ", orgemp.PeopleID AS [PeopleID] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') AS [PeopleName] " & vbCrLf &
                ", orgemp.DateHired AS [DateHired] " & vbCrLf &
                ", orgemp.DeActivated AS [DeActivated] " & vbCrLf &
                ", orgemp.DeactivationDate AS [DeactivationDate] " & vbCrLf &
                ", orgemp.DeactivationReason AS [DeactivationReason] " & vbCrLf &
                ", orgemp.Comments AS [Comments] " & vbCrLf &
                ", orgemp.IsBan AS [IsBan] " & vbCrLf &
                ", ppolorg.IsConfirmed AS [IsConfirmed] " & vbCrLf &
                ", ppolorg.IsTerminated AS [IsTerminated], ppol.FirstName as [PeopleFirstName] " & vbCrLf &
                " FROM LibraryOrganizationEmployee orgemp " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppol ON (orgemp.PeopleID = ppol.ID) " & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID) " & vbCrLf &
                " LEFT JOIN MobizLibraryPeopleOrganization ppolorg ON (ppolorg.PeopleID = ppol.ID " & vbCrLf &
                " AND ppolorg.OrganizationID = " & ReadyString(OrganizationID) & ") " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPIM() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = "WHERE orgprd.IsDeactivated = " & ReadyString(IIf(IsActive = 1, 0, 1)) 'swap
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(prd.ProductNo, '') + ' ' +" & vbCrLf &
                                "ISNULL(prd.ProductNo, '') + ' ' +" & vbCrLf &
                                "ISNULL(prd.ProductName, '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), orgprd.IsDoNotShareTransDataToBrandOwner), '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), orgprd.ActivationDate), '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), orgprd.IsDeActivated), '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), orgprd.DeactivationDate), '') + ' ' +" & vbCrLf &
                                "ISNULL(orgprd.DeactivationReason, '') + ' ' +" & vbCrLf &
                                "ISNULL(orgprd.Comments, '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), orgprd.IsBan), '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), prd.IsBrandOwner), '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), (SELECT COUNT(*) FROM MobizLibraryPIMProductPriceLevel WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND ProductID = orgprd.ProductID AND PriceLevelID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000'))), '') + ' ' +" & vbCrLf &
                                "ISNULL(prd.Brand, '') + ' ' +" & vbCrLf &
                                "ISNULL((SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = prd.ID), '') + ' ' +" & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), prd.SRP), '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.ProductName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.ProductNo " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY Srp " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY RetailPrice " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY WholesalePrice " & PageColumnDirection
        ElseIf PageColumnNo = 6 Then
            whereOrder = " ORDER BY Brand " & PageColumnDirection
        ElseIf PageColumnNo = 7 Then
            whereOrder = " ORDER BY Category " & PageColumnDirection
        ElseIf PageColumnNo = 8 Then
            whereOrder = " ORDER BY ActivationDate " & PageColumnDirection
        ElseIf PageColumnNo = 9 Then
            whereOrder = " ORDER BY LastUpdated " & PageColumnDirection
        ElseIf PageColumnNo = 10 Then
            whereOrder = " ORDER BY DeActivated " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT orgprd.ID AS [ID] " & vbCrLf &
                ", orgprd.ProductID AS [ProductID] " & vbCrLf &
                ", prd.ProductNo AS [ProductNo] " & vbCrLf &
                ", prd.ProductName AS [ProductName] " & vbCrLf &
                ", orgprd.IsDoNotShareTransDataToBrandOwner AS [IsDoNotShareTransDataToBrandOwner] " & vbCrLf &
                ", orgprd.ActivationDate AS [ActivationDate] " & vbCrLf &
                ", orgprd.IsDeActivated AS [DeActivated] " & vbCrLf &
                ", orgprd.DeactivationDate AS [DeactivationDate] " & vbCrLf &
                ", orgprd.DeactivationReason AS [DeactivationReason] " & vbCrLf &
                ", orgprd.Comments AS [Comments] " & vbCrLf &
                ", orgprd.IsBan AS [IsBan] " & vbCrLf &
                ", orgprd.LastUpdated" & vbCrLf &
                ", prd.Brand AS [Brand] " & vbCrLf &
                ", Retail.Price AS RetailPrice " & vbCrLf &
                ", Retail.LastUpdated AS RetailPriceLastUpdated " & vbCrLf &
                ", Srp.Price AS Srp " & vbCrLf &
                ", Srp.LastUpdated AS SrpLastUpdated " & vbCrLf &
                ", WholesalePrice.Price AS WholesalePrice " & vbCrLf &
                ", WholesalePrice.LastUpdated AS WholesalePriceLastUpdated " & vbCrLf &
                ", (SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = prd.ID) AS Category " & vbCrLf &
                ", (SELECT COUNT(*) FROM MobizLibraryPIMProductPriceLevel WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND ProductID = orgprd.ProductID AND PriceLevelID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000')) AS [IsPriceCount] " & vbCrLf &
                ", (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE Size = 'small' AND ProductID = prd.ID AND IsDefault = 1 AND IsFromVendor = 0) as Image" & vbCrLf

        pSql &= " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf
        pSql &= " OUTER APPLY (SELECT TOP 1 Price" & vbCrLf &
            "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            " 	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(modClient.RetailPriceID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID = prd.ID" & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) AS Retail    " & vbCrLf
        pSql &= " OUTER APPLY (SELECT TOP 1 Price" & vbCrLf &
            "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            " 	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(modClient.SrpID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID = prd.ID" & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) AS Srp    " & vbCrLf
        pSql &= " OUTER APPLY (SELECT TOP 1 Price" & vbCrLf &
            "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            " 	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(modClient.WholesalePriceID) & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID = prd.ID" & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) AS WholesalePrice    " & vbCrLf
        pSql &= "" & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit
        Common.WebRequest.ProData.CommandTimeout = 120
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPIMBrands() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE brand.IsActive = " & IsActive
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If

                whereSearchContent = whereSearchContent & operators &
                                "ISNULL(Brand, '')" & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Brand " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT brand.*, ppol.FirstName + ' ' + ppol.Surname as LastUpdatedByName " & vbCrLf &
                " FROM LibraryPimBrands brand " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppol ON (ppol.ID = brand.LastUpdatedBy) " & vbCrLf &
                whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit
        Common.WebRequest.ProData.CommandTimeout = 120
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPIMCategories() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE cat.IsActive = " & IsActive
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If

                whereSearchContent = whereSearchContent & operators &
                                "ISNULL(Category, '')" & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Category " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT cat.*, ppol.FirstName + ' ' + ppol.Surname as LastUpdatedByName " & vbCrLf &
                " FROM LibraryPimCategories cat " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppol ON (ppol.ID = cat.LastUpdatedBy) " & vbCrLf &
                whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit
        Common.WebRequest.ProData.CommandTimeout = 120
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPrinterRole() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(prntr.PrinterRole, '') + ' ' + " & vbCrLf &
                                "ISNULL(prntr.PrinterType, '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), prntr.IsActive), '') + ' ' + " & vbCrLf &
                                "ISNULL(CONVERT(varchar(MAX), prntr.LastUpdated), '') + ' ' + " & vbCrLf &
                                "ISNULL(ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') , '') + ' ' + " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.PrinterRole " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.PrinterType " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.IsActive " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT prntr.ID AS [ID] " & vbCrLf &
                ", prntr.PrinterRole AS [PrinterRole] " & vbCrLf &
                ", prntr.PrinterType AS [PrinterType] " & vbCrLf &
                ", prntr.IsActive AS [IsActive] " & vbCrLf &
                ", prntr.LastUpdated AS [LastUpdated] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '')  AS [ModifiedBy] " & vbCrLf &
                " FROM LibraryDevicePrinter prntr " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = prntr.UserID) " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetHotButton() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE pim.IsActive = 1 "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " pim.Name " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = whereSearch & " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT pim.ID " & vbCrLf &
                ", pim.Name " & vbCrLf &
                ", pim.LastUpdated " & vbCrLf &
                ", ppl.FirstName " & vbCrLf &
                ", ppl.MiddleName " & vbCrLf &
                ", ppl.Surname " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppl.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + '', ''), '')  AS [ModifiedBy]" & vbCrLf &
                " FROM LibraryPimMenu pim " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppl ON pim.UserID = ppl.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetOnlineStoreCategory() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE template.IsActive = 1 "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " template.Name " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = whereSearch & " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT template.ID " & vbCrLf &
                ", template.Name " & vbCrLf &
                ", template.LastUpdated " & vbCrLf &
                ", template.IsDefault " & vbCrLf &
                ", ppl.FirstName " & vbCrLf &
                ", ppl.MiddleName " & vbCrLf &
                ", ppl.Surname " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppl.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + '', ''), '')  AS [ModifiedBy]" & vbCrLf &
                " FROM LibraryStoreCategoryTemplate template " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppl ON template.UserID = ppl.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetBarcodeType() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " barcode.BarcodeType " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.BarcodeType " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.StickerDimensions " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT barcode.ID " & vbCrLf &
                ", barcode.BarcodeType " & vbCrLf &
                ", barcode.StickerDimensions " & vbCrLf &
                ", barcode.LastUpdated " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppl.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + '', ''), '')  AS [ModifiedBy]" & vbCrLf &
                " FROM LibraryProprietaryBarcodeType barcode " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppl ON barcode.UserID = ppl.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetWorkScheduleDetails() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        'ScheduleName, ScheduleDescription, Type, TimeFrom, TimeTo, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday 

        pSql = "select * from LibraryDTRSchedule " & vbCrLf &
                "inner join LibraryDTRScheduleTime on LibraryDTRScheduleTime.ScheduleID = LibraryDTRSchedule.ID " & vbCrLf &
                "WHERE LibraryDTRSchedule.ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID")))

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Function GetPayGradeDetails() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "Select * from LibrarySalaryGrade " & vbCrLf &
                "WHERE ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID")))

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Function GetCenterList() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE Type = " & ReadyString(Type) & " AND ISNULL(cen.IsActive, 1) = " & CInt2(IsActive)
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " cen.Name " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch += " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.Description) " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateCreated) " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.ModifiedDate) " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT cen.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppl.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppl.Surname)) + '', ''), '')  AS [LastModifiedBy]" & vbCrLf &
                " FROM LibraryExpensesCenters as cen " & vbCrLf &
                " INNER JOIN MobizLibraryPeople ppl ON cen.ModifiedBy = ppl.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetHolidays() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE Type = " & ReadyString(Type)
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " holiday.Description " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch += " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
            'ElseIf PageColumnNo = 2 Then
            '    whereOrder = " ORDER BY CONVERT(datetime, mainsql.Description) " & PageColumnDirection
            'ElseIf PageColumnNo = 3 Then
            '    whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateCreated) " & PageColumnDirection
            'ElseIf PageColumnNo = 4 Then
            '    whereOrder = " ORDER BY CONVERT(datetime, mainsql.ModifiedDate) " & PageColumnDirection
            'ElseIf PageColumnNo = 5 Then
            '    whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT holiday.* " & vbCrLf &
                " FROM LibraryPhilippineHolidays as holiday " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetVMPlanogramTemplate() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " models.Name " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateModified) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT main.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(modified.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(modified.Surname)) + '', ''), '')  AS [ModifiedByName]" & vbCrLf &
                " FROM LibraryVendingMachinePlanogramTemplate main " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople modified ON main.ModifiedBy = modified.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function
    Function GetVMMainBoard() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " models.Name " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.NoOfBoards " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.MainBoardName " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateModified) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT main.* " & vbCrLf &
                ", models.Name as MainBoardName " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(modified.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(modified.Surname)) + '', ''), '')  AS [ModifiedByName]" & vbCrLf &
                " FROM LibraryVendingMachines main " & vbCrLf &
                " LEFT JOIN LibraryVendingMachineBoards models ON models.ID = main.BoardID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople modified ON models.ModifiedBy = modified.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetVDMModels() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " models.Name + ' ' + models.Description " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.NoOfMotors " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateCreated) " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateModified) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT models.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(created.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(created.Surname)) + '', ''), '')  AS [CreatedByName]" & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(modified.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(modified.Surname)) + '', ''), '')  AS [ModifiedByName]" & vbCrLf &
                " FROM LibraryVendingMachineBoards models " & vbCrLf &
                " INNER JOIN MobizLibraryPeople created ON models.CreatedBy = created.ID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople modified ON models.ModifiedBy = modified.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetVDMAuthorizers() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " models.EmployeeName " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.EmployeeName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.UpdatedByName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.UpdatedOn) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT auth.ID, auth.EmployeeID, auth.UpdatedOn " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(emp.Surname)) + ', ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Firstname)) + ' ', ''), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Middlename)) + '', ''), '')  AS [EmployeeName]" & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(modified.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(modified.Surname)) + '', ''), '')  AS [UpdatedByName]" & vbCrLf &
                " FROM LibraryVendingMachineAuthorizers auth " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople emp ON auth.EmployeeID = emp.ID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople modified ON auth.UpdatedByID = modified.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetVDMPhysicalButtons() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " temp.Name + ' ' + temp.Descriptions " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.UserName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.UpdatedOn) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT temp.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(emp.Surname)) + ', ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Firstname)) + ' ', ''), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Middlename)) + '', ''), '')  AS [UserName]" & vbCrLf &
                " FROM LibraryVDMPhysicalButtonTemplate temp " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople emp ON temp.UserID = emp.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function VDMRiceDispensingGuides() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " temp.Name + ' ' + temp.Descriptions " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.UserName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.UpdatedOn) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT temp.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(emp.Surname)) + ', ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Firstname)) + ' ', ''), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Middlename)) + '', ''), '')  AS [UserName]" & vbCrLf &
                " FROM LibraryVDMGuidesTemplate temp " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople emp ON temp.UserID = emp.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetVDMAds() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " temp.Name + ' ' + temp.Descriptions " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.UserName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.UpdatedOn) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT temp.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(emp.Surname)) + ', ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Firstname)) + ' ', ''), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Middlename)) + '', ''), '')  AS [UserName]" & vbCrLf &
                " FROM LibraryVDMAdsTemplate temp " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople emp ON temp.UserID = emp.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPromoCampaignCode() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE promo.IsActive = " & IsActive
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " promo.CampaignName " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.CampaignName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY CONVERT(date, mainsql.StartDate) " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(date, mainsql.EndDate) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT promo.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(created.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(created.Surname)) + '', ''), '')  AS [ModifiedByName]" & vbCrLf &
                " FROM MobizLibraryPromoCampaign as promo  " & vbCrLf &
                " INNER JOIN MobizLibraryPeople created ON promo.ModifiedBy = created.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetVouchers() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE voucher.IsActive = " & IsActive
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " voucher.ReferenceNo + ' ' + ISNULL(voucher.ProductCode, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.ReferenceNo " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.VoucherDescription " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.TransactionDate) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT voucher.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(created.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(created.Surname)) + '', ''), '')  AS [UploadedBy]" & vbCrLf &
                " FROM MobizLibraryVouchers as voucher  " & vbCrLf &
                " INNER JOIN MobizLibraryPeople created ON voucher.UploadedByID = created.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetGroupSettings() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                    " AND ISNULL(IsActive, 1) = " & IsActive

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " GroupName " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.GroupName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT grp.ID AS [ID] " & vbCrLf &
                ", grp.GroupName AS [GroupName] " & vbCrLf &
                ", ISNULL(grp.Description, '') AS [Description] " & vbCrLf &
                ", grp.LastUpdated AS [LastUpdated] " & vbCrLf &
                ", ISNULL(grp.IsActive, 1) AS [IsActive] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') AS [ModifiedBy] " & vbCrLf &
                " FROM LibraryFranchiseGroup grp " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = grp.UserID) " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetRegionalGroupSettings() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE grp.IsActive = " & IsActive & vbCrLf
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " RegionName " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.RegionName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.IsActive " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.UpdatedByName " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT grp.ID AS [ID] " & vbCrLf &
                ", grp.RegionName " & vbCrLf &
                ", grp.IsActive " & vbCrLf &
                ", grp.LastUpdated " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') AS [UpdatedByName] " & vbCrLf &
                " FROM LibraryRegionalGroupSettings grp " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = grp.UserID) " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetCustomer() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " (CASE cust.IsOrganization " & vbCrLf &
                                " WHEN 1 THEN  cust.CompanyName " & vbCrLf &
                                " ELSE " & vbCrLf &
                                " ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                                " ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                " ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                " ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') END) " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.CustomerName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.ActivationDate) " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.DeActivated " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT orgcust.ID AS [ID] " & vbCrLf &
                ", orgcust.OrganizationID AS [OrganizationID] " & vbCrLf &
                ", (CASE cust.IsOrganization      WHEN 1 THEN  cust.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '')  END) AS [CustomerName] " & vbCrLf &
                ", orgcust.ActivationDate AS [ActivationDate] " & vbCrLf &
                ", orgcust.DeActivated AS [DeActivated] " & vbCrLf &
                ", orgcust.DeactivationDate AS [DeactivationDate] " & vbCrLf &
                ", orgcust.DeactivationReason AS [DeactivationReason] " & vbCrLf &
                ", orgcust.Comments AS [Comments] " & vbCrLf &
                ", orgcust.IsBan AS [IsBan], ISNULL(orgcust.CustomerCode, '') as CustomerCode " & vbCrLf &
                " FROM LibraryOrganizationCustomer orgcust " & vbCrLf &
                " INNER JOIN MobizLibraryOrganization cust ON (orgcust.OrganizationID = cust.ID) " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetSupplier() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If

                whereSearchContent = whereSearchContent & operators & " SupplierName " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.SupplierName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.DeActivated " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT orgsup.ID AS [ID] " & vbCrLf &
                ", sup.IsOrganization " & vbCrLf &
                ", orgsup.SupplierID AS [SupplierID] " & vbCrLf &
                ", (CASE sup.IsOrganization      WHEN 1 THEN  sup.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(sup.PersonSuffixName)),', '), '')  END) AS [SupplierName] " & vbCrLf &
                ", orgsup.ActivationDate AS [ActivationDate] " & vbCrLf &
                ", orgsup.DeActivated AS [DeActivated] " & vbCrLf &
                ", orgsup.DeactivationDate AS [DeactivationDate] " & vbCrLf &
                ", orgsup.DeactivationReason AS [DeactivationReason] " & vbCrLf &
                ", orgsup.Comments AS [Comments] " & vbCrLf &
                ", orgsup.IsBan AS [IsBan], ISNULL(orgsup.VendorCode, '') as VendorCode " & vbCrLf &
                ", CONVERT(varchar, orgsup.LastUpdated, 107) AS [LastUpdated] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '')  AS [ModifiedBy] " & vbCrLf &
                " FROM LibraryOrganizationSupplier orgsup " & vbCrLf &
                " INNER JOIN MobizLibraryOrganization sup ON (orgsup.SupplierID = sup.ID) " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = orgsup.UserID) " & vbCrLf &
                " WHERE orgsup.Deactivated =" & If(CInt2(IsActive) = 1, 0, 1) & " AND ISNULL(orgsup.TaggedAs, '') = '' -- TaggedAs use in DTR " & vbCrLf &
                ") as mainsql " & whereSearch & " ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetSalesTeam() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE ISNULL(Type, 'team') = " & ReadyString(Type)
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " TeamName + ' ' + " & vbCrLf &
                                " Description " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch += " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.TeamName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.Description " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT [ID] " & vbCrLf &
                ", [TeamName] " & vbCrLf &
                ", [Description] " & vbCrLf &
                ", [LastUpdated] " & vbCrLf &
                ", [Type] " & vbCrLf &
                " FROM LibraryCRMTeams " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetItemModifier() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE ISNULL(LibraryItemModifiers.IsActive, 1) = " & ReadyString(IsActive) & vbCrLf
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(CONVERT(varchar(MAX), LibraryItemModifiers.ID), '') + ' ' + " & vbCrLf &
                                " ISNULL(LibraryItemModifiers.Name, '') + ' ' + " & vbCrLf &
                                " ISNULL(CONVERT(varchar(MAX), LibraryItemModifiers.LastUpdated), '') + ' ' + " & vbCrLf &
                                " ISNULL(ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), ''), '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch += " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.UpdatedByName " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT LibraryItemModifiers.ID AS [ID] " & vbCrLf &
                ", LibraryItemModifiers.Name AS [Name] " & vbCrLf &
                ", LibraryItemModifiers.Description AS [Description] " & vbCrLf &
                ", LibraryItemModifiers.Type AS [Type] " & vbCrLf &
                ", LibraryItemModifiers.Max AS [Max] " & vbCrLf &
                ", LibraryItemModifiers.IsRequired AS [IsRequired] " & vbCrLf &
                ", LibraryItemModifiers.IsActive AS [IsActive] " & vbCrLf &
                ", LibraryItemModifiers.LastUpdated AS [LastUpdated] " & vbCrLf &
                ", LibraryItemModifiers.ChoicesReference " & vbCrLf &
                ", LibraryItemModifiers.PromoBuyQty " & vbCrLf &
                ", LibraryItemModifiers.PromoTakeQty " & vbCrLf &
                ", LibraryItemModifiers.PromoTakePrice " & vbCrLf &
                ", LibraryItemModifiers.PromoTakeDiscount " & vbCrLf &
                ", ISNULL(LibraryItemModifiers.PreferenceType, 'category') AS [PreferenceType] " & vbCrLf &
                ", LibraryItemModifiers.IsRestrictedModifier AS [IsRestrictedModifier] " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '') AS [UpdatedByName] " & vbCrLf &
                " FROM LibraryItemModifiers " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryItemModifiers.UpdatedBy " & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPrinters() As DataTable

        Dim pFinder As Finder
        Dim pOut As DataTable
        Dim pSql As String

        pFinder = CreateFinder()
        'OLD CODE - commented by arnie 1-20-2023
        'With pFinder
        '    With .Fields
        '        .Add("ID", SqlDbType.UniqueIdentifier, "LibraryDevicePrinterFowst.ID")
        '        With .Add("RowNum", SqlDbType.Int, "ROW_NUMBER() OVER (ORDER BY PrinterName DESC)")
        '            .Searchable = False
        '        End With
        '        .Add("FowstID", SqlDbType.UniqueIdentifier, "LibraryDevicePrinterFowst.FowstID")
        '        .Add("PrinterName", SqlDbType.VarChar, "LibraryDevicePrinterFowst.PrinterName")
        '        .Add("ConnectionType", SqlDbType.VarChar, "LibraryDevicePrinterFowst.ConnectionType")
        '        .Add("PrinterType", SqlDbType.VarChar, "LibraryDevicePrinter.PrinterType")
        '        .Add("PrinterIPAddress", SqlDbType.VarChar, "LibraryDevicePrinterFowst.PrinterIPAddress")
        '        .Add("FowstName", SqlDbType.VarChar, "MobizLibraryOrganizationFOWST.Name")
        '        .Add("PrinterRole", SqlDbType.VarChar, "LibraryDevicePrinter.PrinterRole")
        '        .Add("Brand", SqlDbType.VarChar, "LibraryDevicePrinter.Brand")
        '        .Add("Model", SqlDbType.VarChar, "LibraryDevicePrinter.Model")
        '        .Add("SerialNum", SqlDbType.VarChar, "LibraryDevicePrinterFowst.SerialNum")
        '        With .Add("DeviceNames", SqlDbType.VarChar, "(SELECT ISNULL(STUFF((" & vbCrLf &
        '            " 	SELECT '; ' + LibraryDevice.PhoneName" & vbCrLf &
        '            "     FROM LibraryDevice" & vbCrLf &
        '            "     JOIN LibraryDeviceFowstPrinter ON LibraryDeviceFowstPrinter.DeviceID = LibraryDevice.ID" & vbCrLf &
        '            "     WHERE LibraryDeviceFowstPrinter.PrinterFowstID = LibraryDevicePrinterFowst.ID" & vbCrLf &
        '            "     ORDER BY LibraryDevice.PhoneName" & vbCrLf &
        '            "     FOR XML PATH ('')) " & vbCrLf &
        '            "     ,1 ,2, ''), ''))")
        '            .Searchable = False
        '        End With
        '    End With
        '    .SqlFrom = " FROM LibraryDevicePrinter" & vbCrLf &
        '        " JOIN LibraryDevicePrinterFowst ON LibraryDevicePrinterFowst.PrinterID = LibraryDevicePrinter.ID" & vbCrLf &
        '        " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryDevicePrinterFowst.FowstID"
        '    pSql = .CreateSql
        '    pOut = .ProData.Execute(pSql)
        'End With

        pSql = "SELECT *, ROW_NUMBER() OVER (ORDER BY PrinterName ASC) AS [RowNum] FROM (" & vbCrLf &
                "--for new setup - 1:N ang pag assign og FOWST" & vbCrLf &
                "   SELECT LibraryDevicePrinter.ID AS [ID] " & vbCrLf &
                "   , NULL AS [FowstID] " & vbCrLf &
                "   , PrinterName AS [PrinterName] " & vbCrLf &
                "   , ConnectionType AS [ConnectionType] " & vbCrLf &
                "   , PrinterType AS [PrinterType] " & vbCrLf &
                "   , PrinterIPAddress AS [PrinterIPAddress] " & vbCrLf &
                "   , '' AS [FowstName] " & vbCrLf &
                "   , PrinterRole AS [PrinterRole] " & vbCrLf &
                "   , Brand AS [Brand] " & vbCrLf &
                "   , Model AS [Model] " & vbCrLf &
                "   , SerialNum AS [SerialNum] " & vbCrLf &
                "   , ISNULL(LibraryDevicePrinter.IsNewSetup, 0) as IsNewSetup " & vbCrLf &
                "   , '' AS [DeviceNames] " & vbCrLf &
                "   FROM LibraryDevicePrinter " & vbCrLf &
                "   WHERE ISNULL(IsNewSetup, 0) = 1 " & vbCrLf &
                "UNION ALL" & vbCrLf &
                "--for old setup - 1:1 ang pag assign og FOWST" & vbCrLf &
                "   SELECT LibraryDevicePrinterFowst.ID AS [ID] " & vbCrLf &
                "   , LibraryDevicePrinterFowst.FowstID AS [FowstID] " & vbCrLf &
                "   , LibraryDevicePrinterFowst.PrinterName AS [PrinterName] " & vbCrLf &
                "   , LibraryDevicePrinterFowst.ConnectionType AS [ConnectionType] " & vbCrLf &
                "   , LibraryDevicePrinter.PrinterType AS [PrinterType] " & vbCrLf &
                "   , LibraryDevicePrinterFowst.PrinterIPAddress AS [PrinterIPAddress] " & vbCrLf &
                "   , MobizLibraryOrganizationFOWST.Name AS [FowstName] " & vbCrLf &
                "   , LibraryDevicePrinter.PrinterRole AS [PrinterRole] " & vbCrLf &
                "   , LibraryDevicePrinter.Brand AS [Brand] " & vbCrLf &
                "   , LibraryDevicePrinter.Model AS [Model] " & vbCrLf &
                "   , LibraryDevicePrinterFowst.SerialNum AS [SerialNum] " & vbCrLf &
                "   , ISNULL(LibraryDevicePrinter.IsNewSetup, 0) as IsNewSetup " & vbCrLf &
                "   , (SELECT ISNULL(STUFF(( " & vbCrLf &
                "       SELECT '; ' + LibraryDevice.PhoneName " & vbCrLf &
                "       FROM LibraryDevice " & vbCrLf &
                "       JOIN LibraryDeviceFowstPrinter ON LibraryDeviceFowstPrinter.DeviceID = LibraryDevice.ID " & vbCrLf &
                "       WHERE LibraryDeviceFowstPrinter.PrinterFowstID = LibraryDevicePrinterFowst.ID " & vbCrLf &
                "       ORDER BY LibraryDevice.PhoneName " & vbCrLf &
                "       FOR XML PATH ('')) " & vbCrLf &
                "       ,1 ,2, ''), '')) AS [DeviceNames]" & vbCrLf &
                "   FROM LibraryDevicePrinter " & vbCrLf &
                "   JOIN LibraryDevicePrinterFowst ON LibraryDevicePrinterFowst.PrinterID = LibraryDevicePrinter.ID " & vbCrLf &
                "   JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryDevicePrinterFowst.FowstID " & vbCrLf &
                "   WHERE ISNULL(IsNewSetup, 0) = 0 " & vbCrLf &
                ") as Devices ORDER BY IsNewSetup DESC, PrinterName ASC " & vbCrLf

        pOut = Common.WebRequest.ProData.Execute(pSql)

        For Each pDR As DataRow In pOut.Rows
            pDR("PrinterRole") = LibraryPrinters.ConvertRoleText(CStr2(pDR("PrinterRole")))
        Next pDR

        Return pOut

    End Function

    Function GetSalesRep() As DataTable

        Dim pFinder As Finder
        Dim pOut As DataTable
        Dim pSql As String

        Dim pModifiedBySelect = BrowserController.ComposePeopleName()

        pFinder = CreateFinder()
        With pFinder
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "LibrarySalesRep.ID")
                With .Add("RowNum", SqlDbType.Int, "ROW_NUMBER() OVER (ORDER BY Name DESC)")
                    .Searchable = False
                End With
                .Add("IsActive", SqlDbType.Bit, "ISNULL(LibrarySalesRep.IsActive, 1)")
                .Add("Name", SqlDbType.VarChar, "LibrarySalesRep.Name")
                .Add("LastUpdated", SqlDbType.DateTime2, "LibrarySalesRep.LastUpdated")
                .Add("ModifiedBy", SqlDbType.VarChar, pModifiedBySelect)
            End With
            .SqlFrom = " FROM LibrarySalesRep" & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibrarySalesRep.UserID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlWhere = " WHERE ISNULL(LibrarySalesRep.IsActive, 1) = 1"
            pSql = .CreateSql
            pOut = .ProData.Execute(pSql)
        End With

        Return pOut

    End Function
    Function GetTableView() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(temp.Name, '') + ' ' + " & vbCrLf &
                                " ISNULL(ppol.FirstName, '') + ' ' + " & vbCrLf &
                                " ISNULL(ppol.Surname, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.ModifiedBy " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT temp.*, " & vbCrLf &
                " ISNULL(ppol.FirstName, '') + ' ' + ISNULL(ppol.Surname, '') as ModifiedBy, " & vbCrLf &
                " ISNULL((SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + fowst.Name FROM " & vbCrLf &
                "   MobizLibraryOrganizationFowst as fowst " & vbCrLf &
                "   INNER JOIN LibraryTableFowst as tempfowst ON tempfowst.FowstID = fowst.ID " & vbCrLf &
                "   WHERE tempfowst.TemplateID = temp.ID " & vbCrLf &
                " FOR XML PATH('')), 1, 1,'')), '') as Locations " & vbCrLf &
                " FROM LibraryTableTemplate as temp " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = temp.UserID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPaymentTerms() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(terms.Name, '') + ' ' + " & vbCrLf &
                                " ISNULL(terms.Description, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch = " WHERE (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.Name " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT terms.*, ISNULL(ppol.FirstName + ' ', '') + ISNULL(ppol.Surname, '') as ModifiedBy" & vbCrLf &
                " FROM LibraryPaymentTerms as terms " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON terms.UserID = ppol.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetPriceLevelCustom() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = " WHERE (lvl.IsActive = 1 AND lvl.OrgID IS NULL OR lvl.OrgID = " & ReadyString(WebRequest.Instance.OrgID) & ") "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If

                whereSearchContent = whereSearchContent & operators & " ISNULL(PriceLevel, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch += " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.PriceLevel " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT lvl.*, ISNULL(ppol.FirstName + ' ', '') + ISNULL(ppol.Surname, '') as ModifiedBy" & vbCrLf &
                " FROM MobizLibraryPIMPriceLevel as lvl " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as ppol ON lvl.UserID = ppol.ID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Private Function CreateFinder() As Finder

        'todo: add common security
        Dim pOut As Finder

        pOut = New Finder
        'pOut.MaxRecords = 50
        pOut.SearchString = PageSearchText
        pOut.ProData = Common.WebRequest.ProData

        Return pOut

    End Function

    Sub SaveGroup(GroupName As String, FOWSTID As String, GroupHeadID As String, distributionfowstid As String)
        Dim p As New InsertUpdate("LibraryFranchiseGroup")
        Dim pSql As String
        Dim strIDReturn As String
        Dim ctrOtherPriceLvl As Integer = 0
        Dim ID As String = CheckGroupName(GroupName)

        p("GroupName") = ReadyString(GroupName)
        'p("Description") = ReadyString(Common.WebRequest.Data("Description"))
        p("FranchisorFowstID") = IIf(String.IsNullOrWhiteSpace(distributionfowstid), "NULL", ReadyString(distributionfowstid))
        'p("RoyaltyFee") = ReadyString(Common.WebRequest.Data("RoyaltyFee"))
        'p("MarketingFee") = ReadyString(Common.WebRequest.Data("MarketingFee"))
        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        p("LastUpdated") = "GetDate()"

        Try
            'Common.WebRequest.ProData.BeginTransaction()

            If ID.Trim().Length <= 0 Then

                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

            'p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("FranchisorOrgID") = ReadyString(WebRequest.Instance.OrgID)
            p("FranchiseeOrgFowstID") = ReadyString(FOWSTID)
            p("IsFranchisorConfirmed") = 1
            p("IsFranchiseeConfirmed") = 1
            p("IsActive") = 1
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "getDate()"

            pSql = "SELECT * FROM MobizLibraryOrganizationFranchisee WHERE FranchiseeOrgFowstID = " & ReadyString(FOWSTID) & " AND FranchiseGroupID = " & ReadyString(strIDReturn)
            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                Common.WebRequest.ProData.Execute(p.GetInsert())
            End If

            pSql = "Update MobizLibraryOrganizationFranchisee set FranchiseGroupID = null where FranchiseGroupID = " & ReadyString(strIDReturn) & " AND FranchisorOrgID = " & ReadyString(CStr2(WebRequest.Instance.OrgID))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'For Each pObj As Object In Common.WebRequest.AllData("Franchisee")

            '",MaxInventoryValue = " & CDec2(pObj("MaxInventoryValue")) & vbCrLf &
            '",IsPOExclusiveToPrincipal = " & CInt2(pObj("IsPOExclusiveToPrincipal")) & vbCrLf &
            '",POPriceLevel = " & ReadyString(pObj("POPriceLevelID")) & vbCrLf &

            'If Not pObj("InventoryValuePriceLevel") = "-" Then
            pSql = "Update MobizLibraryOrganizationFranchisee set " & vbCrLf &
                "FranchiseGroupID = " & ReadyString(strIDReturn) & vbCrLf &
                ",InventoryValuePriceLevel = " & ReadyString("COGS") & vbCrLf &
                ",DefaultSellingPriceLevel = " & ReadyString("01000012-0005-0000-0000-000000000000") & vbCrLf &
                ",OwnershipType = " & ReadyString("Franchised") & vbCrLf &
                ",POPriceLevel = " & ReadyString("01000012-0005-0000-0000-000000000000") & vbCrLf &
                ",IsPOExclusiveToPrincipal = " & CInt2(1) & vbCrLf &
                ",MaxInventoryValue = " & CDec2(0) & vbCrLf &
                " WHERE FranchiseeOrgFowstID = " & ReadyString(FOWSTID) & " AND FranchisorOrgID = " & ReadyString(CStr2(WebRequest.Instance.OrgID))
            'Else
            'pSql = "Update MobizLibraryOrganizationFranchisee set " & vbCrLf &
            '    "FranchiseGroupID = " & ReadyString(strIDReturn) & vbCrLf &
            '    ",MaxInventoryValue = " & CDec2(pObj("MaxInventoryValue")) & vbCrLf &
            '    ",InventoryValuePriceLevel = " & ReadyString(pObj("InventoryValuePriceLevel")) & vbCrLf &
            '    ",DefaultSellingPriceLevel = NULL " & vbCrLf &
            '    ",IsPOExclusiveToPrincipal = " & CInt2(pObj("IsPOExclusiveToPrincipal")) & vbCrLf &
            '    ",POPriceLevel = NULL" & vbCrLf &
            '    ",OwnershipType = " & ReadyString(pObj("OwnershipType")) &
            '    " WHERE FranchiseeOrgFowstID = " & ReadyString(pObj("ID")) & " AND FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            'End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)
            'Next

            'pSql = "Delete from LibraryFranchiseGroupCutOffDetails where FranchiseGroupID = " & ReadyString(strIDReturn)
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Dim vFranchiseGroupCutOffID As String
            Dim dtTemp As DataTable

            pSql = "Select ID from  LibraryFranchiseGroupCutOff where FranchiseGroupID = " & ReadyString(strIDReturn)
            dtTemp = Common.WebRequest.ProData.Execute(pSql)


            If dtTemp.Rows.Count > 0 Then
                vFranchiseGroupCutOffID = CStr2(dtTemp.Rows(0).Item("ID"))
            Else
                vFranchiseGroupCutOffID = Guid.NewGuid.ToString

                pSql = "Insert into LibraryFranchiseGroupCutOff(ID, FranchiseGroupID, ScheduleType)" &
                    "Values(" &
                    ReadyString(vFranchiseGroupCutOffID) & "," &
                    ReadyString(strIDReturn) & "," &
                    ReadyString("Daily") & ")"

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            'For Each pObj As Object In Common.WebRequest.AllData("Schedule")
            '    Dim mSchedule As New InsertUpdate("LibraryFranchiseGroupCutOffDetails")

            '    If Not IsGuid(CStr2(pObj("ID"))) Then
            '        mSchedule("FranchiseGroupCutOffID") = ReadyString(vFranchiseGroupCutOffID)
            '        mSchedule("FranchiseGroupID") = ReadyString(strIDReturn)
            '        mSchedule("CutOffName") = ReadyString(pObj("ScheduleName"))
            '        mSchedule("WeekDay") = WeekdayValue(pObj("ScheduleDay"))
            '        mSchedule("CutOffTime") = ReadyString(pObj("ScheduleTime"))

            '        pSql = mSchedule.GetInsert()
            '    End If

            '    Common.WebRequest.ProData.ExecuteSQL(pSql)
            'Next

            'For Each objOtherPriceLevel As Object In Common.WebRequest.AllData("OtherPriceLevel")
            '    Dim mOtherPriceLevel As New InsertUpdate("MobizLibraryFranchiseGroupPriceLevels")

            '    mOtherPriceLevel("OrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
            '    mOtherPriceLevel("GroupID") = ReadyString(strIDReturn)
            '    mOtherPriceLevel("PriceLevelID") = ReadyString(objOtherPriceLevel("ID").ToString())
            '    mOtherPriceLevel("IsActive") = ReadyString(CBool(objOtherPriceLevel("IsActive").ToString()))
            '    mOtherPriceLevel("LastUpdated") = "getdate()"

            '    If objOtherPriceLevel("State").ToString() = "add" Then
            '        mOtherPriceLevel("ID") = ReadyString(Guid.NewGuid.ToString)
            '        pSql = mOtherPriceLevel.GetInsert()

            '    ElseIf objOtherPriceLevel("State").ToString() = "edit" Then
            '        pSql = mOtherPriceLevel.GetUpdate(" ID = " & ReadyString(objOtherPriceLevel("SID").ToString()))

            '    ElseIf objOtherPriceLevel("State").ToString() = "delete" Then
            '        pSql = "DELETE FROM MobizLibraryFranchiseGroupPriceLevels WHERE ID = " & ReadyString(objOtherPriceLevel("SID").ToString())
            '    End If

            '    Common.WebRequest.ProData.ExecuteSQL(pSql)
            'Next

            pSql = "Delete from LibraryFranchiseGroupHeads where FranchiseGroupID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'For Each pObj As Object In Common.WebRequest.AllData("GroupHeads")
            Dim mSchedule As New InsertUpdate("LibraryFranchiseGroupHeads")

            'mSchedule("CompanyEmailAddress") = ReadyString(pObj("CompanyEmailAdd"))
            mSchedule("FranchiseGroupID") = ReadyString(strIDReturn)
            mSchedule("PeopleID") = ReadyString(GroupHeadID)

            pSql = mSchedule.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            'Next

            'Common.WebRequest.ProData.CommitTransaction()

            'UpdateFranchiseeOrgs(strIDReturn)

            'Return strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            AppendToLog(ex.Message)
            'oError = ex.Message
        End Try

    End Sub

    Function CheckGroupName(groupname As String) As String
        Dim pSql As String = "SELECT TOP 1 * FROM LibraryFranchiseGroup WHERE GroupName = " & ReadyString(groupname)
        Dim pRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
        If pRows.Count > 0 Then
            Return pRows(0)("ID").ToString()
        Else
            Return ""
        End If
    End Function

    Function GetFOWSTAssignedEmployees() As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPeople.ID, MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.SurName as EmployeeName, " & vbCrLf &
                " LibraryDTRRole.RoleName as Position " & vbCrLf &
                " FROM LibraryOrganizationEmployeeFOWST " & vbCrLf &
                " LEFT join MobizLibraryPeople on MobizLibraryPeople.ID = LibraryOrganizationEmployeeFOWST.PeopleID " & vbCrLf &
                " LEFT JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = LibraryOrganizationEmployeeFOWST.PeopleID " & vbCrLf &
                " LEFT JOIN LibraryDTRRole ON LibraryDTRRole.ID = LibraryOrganizationEmployee.RoleTagID " & vbCrLf &
                " WHERE LibraryOrganizationEmployeeFOWST.FOWSTID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
                " AND MobizLibraryPeople.ID IS NOT NULL " & vbCrLf &
                " ORDER BY MobizLibraryPeople.FirstName ASC " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function
End Class
