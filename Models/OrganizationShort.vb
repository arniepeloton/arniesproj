Imports System.Diagnostics.Eventing.Reader
Imports Newtonsoft.Json

Public Class OrganizationShort
    Inherits System.Web.Mvc.Controller

    Public Property ID As String
    Public Property ThirdPartyID As String
    Public Property ThirdPartyPlatform As String
    Public Property CodeNumber As String
    Public Property OrganizationID As String
    Public Property MaxReturnRow As Integer = 50

    Public Property CompanyName As String
    Public Property CompanyBusinessTradeName As String
    Public Property CompanyNameInitial As String

    Public Property OrgTradeNameList As Object()

    Public Property CorporateTIN As String

    Public Property OwnershipType As String
    Public Property IsFranchisor As String

    Public Property OfficeAddressBuildingNo As String
    Public Property OfficeAddressStreet As String
    Public Property OfficeAddressCityID As String
    Public Property OfficeAddressCountryID As String

    Public Property BusinessAddressBuildingNo As String
    Public Property BusinessAddressStreet As String
    Public Property BusinessAddressCityID As String
    Public Property BusinessAddressCountryID As String

    Public Property PersonSeniorCitizensIDNo As String
    Public Property PersonPWDIDNo As String
    Public Property PersonPassportNo As String
    Public Property PersonDriversLicenseNo As String
    Public Property PersonSSSNo As String
    Public Property PersonGSISIDNo As String
    Public Property PersonPRCIDNo As String
    Public Property PersonPostalIDNo As String
    Public Property PersonVotersIDNo As String
    Public Property PersonOtherIDType As String
    Public Property PersonOtherIDNo As String

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


    Public Property OrgCategoryList As Object()

    Public Property FOWSTTypeID As String
    Public Property FOWSTCode As String
    Public Property FOWSTName As String
    Public Property FOWSTDescription As String
    Public Property FOWSTStreetNumber As String
    Public Property FOWSTStreet As String
    Public Property FOWSTCityID As String
    Public Property FOWSTCountryID As String
    Public Property FOWSTSubTypeID As String
    Public Property FOWSTPlateNo As String
    Public Property FOWSTNoOfWheels As String
    Public Property FOWSTBodyColor As String
    Public Property FOWSTWeight As String
    Public Property FOWSTDimension As String
    Public Property FOWSTOwnershipType As String
    Public Property FOWSTContractStart As String
    Public Property FOWSTContractEnd As String
    Public Property FOWSTCostPerMonth As String
    Public Property FOWSTIsMortgaged As String
    Public Property FOWSTIsFranchisee As String
    Public Property FOWSTFranchisorID As String

    Public Property OrgFOWSTLTOList As Object()
    Public Property OrgFOWSTLTFRBList As Object()
    Public Property OrgFOWSTSectionList As Object()
    Public Property OrgFOWSTBPermitList As Object()
    Public Property OrgFOWSTTelList As Object()
    Public Property OrgFOWSTMobileList As Object()
    Public Property OrgFOWSTEmailList As Object()

    Public Property LastName As String
    Public Property FirstName As String
    Public Property MiddleName As String
    Public Property Title As String
    Public Property GenerationalSuffix As String
    Public Property IsActive As String

    Public Property OrgRepContactTelList As Object()
    Public Property OrgRepContactMobileList As Object()
    Public Property OrgRepContactEmailList As Object()

    Public Property IsAddAsCustomer As String

    Public Property PersonID As String
    Public Property OrgID As String
    Public Property PageStart As Integer
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String
    Public Property Filter As String
    Public Property SearchContact As String
    Public Property IsCountPage As String
    Public Property Status As String
    Public Property CompanyID As String
    Public Property CustomerID As String
    Public Property Tin As String
    Public Property VatType As String
    Public Property CompanyBuildingAddress As String
    Public Property CompanyStreetAddress As String
    Public Property CompanyCityAddress As String
    Public Property CompanyRegionAddress As String
    Public Property CompanyCountryAddress As String
    Public Property CompanyBackground As String
    Public Property CompanyMission As String
    Public Property CompanyVision As String
    Public Property CompanyFoundingDate As String
    Public Property BusinessIndustryID As String
    Public Property BusinessIndustryText As String
    Public Property ObjTelephone As Object
    Public Property ObjMobile As Object
    Public Property ObjEmail As Object
    Public Property ObjWebsite As Object
    Public Property ObjOutlet As Object
    Public Property ObjContactPerson As Object
    Public Property ObjContactPersonTelephone As Object
    Public Property ObjContactPersonMobile As Object
    Public Property ObjContactPersonEmail As Object
    Public Property ObjOrgAff As Object
    Public Property ObjAffiliatedPerson As Object
    Public Property DateTime As DateTime
    Public Property NoOfEmployees As String
    Public Property RevenuePerYear As String
    Public Property Scope As String
    Public Property NoOfFactories As String
    Public Property NoOfOffices As String
    Public Property NoOfWarehouses As String
    Public Property NoOfStores As String
    Public Property NoOfTrucks As String
    Public Property SingleCreditTerm As String ' bit
    Public Property Island As String

    Public Property SalesRepID As String
    Public Property PriceLevel As Object
    Public Property Terms As String
    Public Property Limit As String
    Public Property UnlimitedCredit As Integer ' 0 or 1
    Public Property RequireCnCApproval As Integer ' 0 or 1
    Public Property CustomerRequirePoNum As Integer ' 0 or 1
    Public Property Balance As String
    Public Property AsOf As Date

    Public Property DefaultHotButtonTemplateID As String
    Public Property ThirdPartyBegBalance As String
    Public Property ThirdPartyBegBalanceAsOf As Date?

    Public Property BirthDate As String
    Public Property Position As String
    Public Property PositionDescription As String
    Public Property ObjPersonTelephone As Object
    Public Property ObjPersonMobile As Object
    Public Property ObjPersonEmail As Object
    Public Property ObjPersonOrgAff As Object
    Public Property AddressID As String
    Public Property HouseBuildingNo As String
    Public Property Street As String
    Public Property City As String
    Public Property Region As String
    Public Property Country As String
    Public Property TypeAddress As String
    Public Property ObjPersonElementary As Object
    Public Property ObjPersonHighSchool As Object
    Public Property ObjPersonCollege As Object
    Public Property IsCustomer As String
    Public Property ObjNewVendorPrices As Object
    Public Property CusCreditTerms As Object
    Public Property SalesReps As SaveNewOrgProspectInput.SalesRepsClass
    Public Property SalesChannelID As String

    Public SaveAsCustomer As Boolean

    Dim isImportToCrm As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.AutoImportCustomerRecordsToCRM)

    Function ApplyOutletsToSelectedCustomers() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            For Each customer As Object In ObjOutlet
                For Each outlet As Object In customer("SelectedOutlet")
                    Dim strOutletID As String = Guid.NewGuid.ToString()
                    Dim p = New InsertUpdate("MobizLibraryOrganizationFOWST")
                    Dim pSql As String

                    p("OrganizationID") = ReadyString(customer("ID"))
                    p("Name") = ReadyString(outlet("Name"))
                    p("StreetNumber") = ReadyString("")
                    p("NetSuiteID") = ReadyString(outlet("ThirdPartyID"))
                    p("Street") = ReadyString("")

                    pSql = "SELECT ID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(outlet("ID").ToString())
                    Dim res As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    If res.Count = 0 Then
                        p("ID") = ReadyString(strOutletID)
                        p("IsActive") = "1"
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        strOutletID = res(0)("ID").ToString()
                    End If

                    For Each department As Object In outlet("SelectedDept")
                        p = New InsertUpdate("LibraryFowstBin")

                        p("FowstID") = ReadyString(strOutletID)
                        p("Name") = ReadyString(department("Department"))
                        p("NetSuiteID") = ReadyString(department("ThirdPartyID"))
                        p("IsActive") = "1"

                        p("ID") = ReadyString(Guid.NewGuid.ToString())

                        'If department.ContainsKey("RelatedFowstBinID") Then
                        '    p("RelatedFowstBinID") = ReadyString(department("RelatedFowstBinID"))
                        'End If

                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Next department
                Next outlet
            Next customer

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function ImportDataFromCrm() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            Dim pSql As String = "SELECT ID, CompanyName, OfficeAddressBuildingNo, OfficeAddressStreet, OfficeAddressCityID, OfficeAddressCountryID, " & vbCrLf &
                                " IsActive, Background, Mission, Vision, FoundingDate, IndustryClassID, Region, NoOfFactory, NoOfStore, " & vbCrLf &
                                " NoOfTruck, RevenuePerYear, ScopeOfOperations " & vbCrLf &
                                " FROM LibraryOrganization " & vbCrLf &
                                " ORDER BY CompanyName ASC "
            Dim pRowsOrg As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim p As New InsertUpdate("MobizLibraryOrganization")
            Dim ctrAdded As Integer = 0
            Dim ctrUpdated As Integer = 0

            For Each org As Object In pRowsOrg
                pSql = "SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(org("ID").ToString())
                Dim pOrgCount As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                p = New InsertUpdate("MobizLibraryOrganization")

                p("ID") = ReadyString(org("ID").ToString())
                p("IsOrganization") = 1
                p("CompanyName") = ReadyString(org("CompanyName"))
                p("OfficeAddressBuildingNo") = ReadyString(org("OfficeAddressStreet"))
                p("OfficeAddressCityID") = IIf(CStr2(org("OfficeAddressCityID")) = "", "NULL", ReadyString(org("OfficeAddressCityID")))
                p("OfficeAddressCountryID") = IIf(CStr2(org("OfficeAddressCountryID")) = "", "NULL", ReadyString(org("OfficeAddressCountryID")))
                p("IsActive") = ReadyString(org("IsActive"))
                p("Background") = ReadyString(org("Background"))
                p("Mission") = ReadyString(org("Mission"))
                p("Vision") = ReadyString(org("Vision"))
                p("FoundingDate") = ReadyString(org("FoundingDate"))
                p("IndustryClassID") = IIf(CStr2(org("IndustryClassID")) = "", "NULL", ReadyString(org("IndustryClassID")))
                p("Region") = ReadyString(org("Region"))
                p("NoOfFactory") = ReadyString(CDec2(org("NoOfFactory")))
                p("NoOfStore") = ReadyString(CDec2(org("NoOfStore")))
                p("NoOfTruck") = ReadyString(CDec2(org("NoOfTruck")))
                p("RevenuePerYear") = ReadyString(CDec2(org("RevenuePerYear")))
                p("ScopeOfOperations") = ReadyString(org("ScopeOfOperations"))
                p("LastUpdated") = "getdate()"

                If pOrgCount = 0 Then
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                    ctrAdded += 1
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(org("ID").ToString())))
                    ctrUpdated += 1
                End If

                pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(org("ID").ToString())

                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("OrganizationID") = ReadyString(org("ID").ToString())

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                pSql = "SELECT * FROM LibraryOrganizationContactEmail WHERE OrganizationID = " & ReadyString(org("ID").ToString())
                Dim rowEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each email As Object In rowEmail
                    pSql = "SELECT ID FROM MobizLibraryOrganizationContactEmail WHERE ID = " & ReadyString(email("ID").ToString())

                    p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                    p("OrganizationID") = ReadyString(email("OrganizationID").ToString())
                    p("EmailAddress") = ReadyString(email("EmailAddress"))
                    p("UserID") = IIf(CStr2(email("UserID")) = "", "NULL", ReadyString(email("UserID")))
                    p("LastUpdated") = "getdate()"
                    p("EmailType") = ReadyString(email("EmailType"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(email("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(email("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next email

                pSql = "SELECT * FROM LibraryOrganizationContactMobile WHERE OrganizationID = " & ReadyString(org("ID").ToString())
                Dim rowMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each mobile As Object In rowMobile
                    pSql = "SELECT ID FROM MobizLibraryOrganizationContactMobile WHERE ID = " & ReadyString(mobile("ID").ToString())

                    p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                    p("OrganizationID") = ReadyString(mobile("OrganizationID").ToString())
                    p("CountryID") = IIf(CStr2(mobile("CountryID")) = "", "NULL", ReadyString(mobile("CountryID")))
                    p("TelCoPrefixID") = IIf(CStr2(mobile("TelCoPrefixID")) = "", "NULL", ReadyString(mobile("TelCoPrefixID")))
                    p("ContactNo") = ReadyString(mobile("ContactNo"))
                    p("LastUpdated") = "getdate()"
                    p("ContactType") = ReadyString(mobile("ContactType"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(mobile("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(mobile("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next mobile

                pSql = "SELECT * FROM LibraryOrganizationContactTel WHERE OrganizationID = " & ReadyString(org("ID").ToString())
                Dim rowTel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each tel As Object In rowTel
                    pSql = "SELECT ID FROM MobizLibraryOrganizationContactTel WHERE ID = " & ReadyString(tel("ID").ToString())

                    p = New InsertUpdate("MobizLibraryOrganizationContactTel")

                    p("OrganizationID") = ReadyString(tel("OrganizationID").ToString())
                    p("CountryID") = IIf(CStr2(tel("CountryID")) = "", "NULL", ReadyString(tel("CountryID")))
                    p("AreaCodeID") = IIf(CStr2(tel("AreaCodeID")) = "", "NULL", ReadyString(tel("AreaCodeID")))
                    p("ContactNo") = ReadyString(tel("ContactNo"))
                    p("ExtensionNo") = ReadyString(tel("ExtensionNo"))
                    p("LastUpdated") = "getdate()"
                    p("ContactType") = ReadyString(tel("ContactType"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(tel("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(tel("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next tel

                pSql = "SELECT * FROM LibraryOrganizationContactWebsite WHERE OrganizationID = " & ReadyString(org("ID").ToString())
                Dim rowWebsite As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each web As Object In rowWebsite
                    pSql = "SELECT ID FROM MobizLibraryOrganizationContactWebsite WHERE ID = " & ReadyString(web("ID").ToString())

                    p = New InsertUpdate("MobizLibraryOrganizationContactWebsite")

                    p("OrganizationID") = ReadyString(web("OrganizationID").ToString())
                    p("Website") = ReadyString(web("Website"))
                    p("LastUpdated") = "getdate()"

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(web("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(web("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next web

                pSql = "SELECT * FROM LibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(org("ID").ToString())
                Dim rowCP As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each cp As Object In rowCP
                    pSql = "SELECT ID FROM MobizLibraryOrganizationContactPerson WHERE ID = " & ReadyString(cp("ID").ToString())

                    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                    p("OrganizationID") = ReadyString(cp("OrganizationID").ToString())
                    p("PeopleID") = ReadyString(cp("PeopleID").ToString())
                    p("LastUpdated") = "getdate()"

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(cp("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    End If

                    pSql = "SELECT * FROM LibraryPeople WHERE ID = " & ReadyString(cp("PeopleID").ToString())
                    Dim cpDetails As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    If cpDetails.Count > 0 Then
                        p = New InsertUpdate("MobizLibraryPeople")

                        p("Surname") = ReadyString(cpDetails(0)("Surname"))
                        p("FirstName") = ReadyString(cpDetails(0)("FirstName"))
                        p("MiddleName") = ReadyString(cpDetails(0)("MiddleName"))
                        p("PersonalSuffixName") = ReadyString(cpDetails(0)("PersonalSuffixName"))
                        p("Position") = ReadyString(cpDetails(0)("Position"))
                        p("PositionDescription") = ReadyString(cpDetails(0)("PositionDescription"))

                        pSql = "SELECT ID FROM MobizLibraryPeople WHERE ID = " & ReadyString(cp("PeopleID").ToString())

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                            p("ID") = ReadyString(cpDetails(0)("ID").ToString())
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                            ctrAdded += 1
                        Else
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpDetails(0)("ID").ToString())))
                            ctrUpdated += 1
                        End If

                        pSql = "SELECT * FROM LibraryPeopleAddress WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                        Dim cpAddress As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        If cpAddress.Count > 0 Then
                            p = New InsertUpdate("MobizLibraryPeopleAddress")

                            p("PeopleID") = ReadyString(cpDetails(0)("ID").ToString())
                            p("HouseBuildingNo") = ReadyString(cpAddress(0)("HouseBuildingNo"))
                            p("Street") = ReadyString(cpAddress(0)("Street"))
                            p("CityID") = IIf(CStr2(cpAddress(0)("CityID")) = "", "NULL", ReadyString(cpAddress(0)("CityID")))
                            p("CountryID") = IIf(CStr2(cpAddress(0)("CountryID")) = "", "NULL", ReadyString(cpAddress(0)("CountryID")))
                            p("AddressType") = ReadyString(cpAddress(0)("AddressType"))
                            p("LastUpdated") = "getdate()"

                            pSql = "SELECT ID FROM MobizLibraryPeopleAddress WHERE ID = " & ReadyString(cpAddress(0)("ID").ToString())

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("ID") = ReadyString(cpAddress(0)("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                                ctrAdded += 1
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpAddress(0)("ID").ToString())))
                                ctrUpdated += 1
                            End If
                        End If

                        pSql = "SELECT * FROM LibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                        Dim cpEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        For Each email As Object In cpEmail
                            pSql = "SELECT ID FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("ID").ToString())

                            p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                            p("PeopleID") = ReadyString(email("PeopleID").ToString())
                            p("EmailAddress") = ReadyString(email("EmailAddress"))
                            p("UserID") = IIf(CStr2(email("UserID")) = "", "NULL", ReadyString(email("UserID")))
                            p("LastUpdated") = "getdate()"
                            p("EmailType") = ReadyString(email("EmailType"))

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("ID") = ReadyString(email("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                                ctrAdded += 1
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(email("ID").ToString())))
                                ctrUpdated += 1
                            End If
                        Next email

                        pSql = "SELECT * FROM LibraryPeopleContactTel WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                        Dim cpTel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        For Each tel As Object In cpTel
                            pSql = "SELECT ID FROM MobizLibraryPeopleContactTel WHERE ID = " & ReadyString(tel("ID").ToString())

                            p = New InsertUpdate("MobizLibraryPeopleContactTel")

                            p("PeopleID") = ReadyString(tel("PeopleID").ToString())
                            p("CountryID") = ReadyString(tel("CountryID"))
                            p("AreaCodeID") = ReadyString(tel("AreaCodeID"))
                            p("ContactNo") = ReadyString(tel("ContactNo"))
                            p("ExtensionNo") = ReadyString(tel("ExtensionNo"))
                            p("LastUpdated") = "getdate()"
                            p("ContactType") = ReadyString(tel("ContactType"))

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("ID") = ReadyString(tel("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                                ctrAdded += 1
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(tel("ID").ToString())))
                                ctrUpdated += 1
                            End If
                        Next tel

                        pSql = "SELECT * FROM LibraryPeopleContactMobile WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                        Dim cpMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        For Each mobile As Object In cpMobile
                            pSql = "SELECT ID FROM MobizLibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("ID").ToString())

                            p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                            p("PeopleID") = ReadyString(mobile("PeopleID").ToString())
                            p("CountryID") = IIf(CStr2(mobile("CountryID")) = "", "NULL", ReadyString(mobile("CountryID")))
                            p("TelCoPrefixID") = IIf(CStr2(mobile("TelCoPrefixID")) = "", "NULL", ReadyString(mobile("TelCoPrefixID")))
                            p("ContactNo") = ReadyString(mobile("ContactNo"))
                            p("LastUpdated") = "getdate()"
                            p("ContactType") = ReadyString(mobile("ContactType"))

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("ID") = ReadyString(mobile("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                                ctrAdded += 1
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(mobile("ID").ToString())))
                                ctrUpdated += 1
                            End If
                        Next mobile
                    End If
                Next cp
            Next org

            pSql = "SELECT * FROM LibraryPeople "
            Dim pRowsPpol As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            For Each ppol As Object In pRowsPpol
                pSql = "SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(ppol("ID").ToString())
                Dim pPpolCount As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                p = New InsertUpdate("MobizLibraryOrganization") 'need isave diri kay diri ginasave ang info sa customer person sa CUSTOMER MODULE

                p("IsOrganization") = 0
                p("PersonSurname") = ReadyString(ppol("Surname"))
                p("PersonFirstName") = ReadyString(ppol("FirstName"))
                p("PersonMiddleName") = ReadyString(ppol("MiddleName"))
                p("IsActive") = CDec2(ppol("IsActive"))
                p("PersonDateOfBirth") = ReadyString(ppol("DateOfBirth"))
                p("PersonSuffixName") = ReadyString(ppol("PersonalSuffixName"))
                p("LastUpdated") = "getdate()"

                If pPpolCount = 0 Then
                    p("ID") = ReadyString(ppol("ID").ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                    ctrAdded += 1
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(ppol("ID").ToString())))
                    ctrUpdated += 1
                End If

                pSql = "SELECT ID FROM MobizLibraryPeople WHERE ID = " & ReadyString(ppol("ID").ToString())
                pPpolCount = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                p = New InsertUpdate("MobizLibraryPeople")

                p("Surname") = ReadyString(ppol("Surname"))
                p("FirstName") = ReadyString(ppol("FirstName"))
                p("MiddleName") = ReadyString(ppol("MiddleName"))
                p("IsActive") = CDec2(ppol("IsActive"))
                p("DateOfBirth") = ReadyString(ppol("DateOfBirth"))
                p("PersonalSuffixName") = ReadyString(ppol("PersonalSuffixName"))
                p("LastUpdated") = "getdate()"

                If pPpolCount = 0 Then
                    p("ID") = ReadyString(ppol("ID").ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(ppol("ID").ToString())))
                End If

                pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(ppol("ID").ToString())

                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("OrganizationID") = ReadyString(ppol("ID").ToString())

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                pSql = "SELECT * FROM LibraryPeopleAddress WHERE PeopleID = " & ReadyString(ppol("ID").ToString())
                Dim cpAddress As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If cpAddress.Count > 0 Then
                    p = New InsertUpdate("MobizLibraryPeopleAddress")

                    p("PeopleID") = ReadyString(ppol("ID").ToString())
                    p("HouseBuildingNo") = ReadyString(cpAddress(0)("HouseBuildingNo"))
                    p("Street") = ReadyString(cpAddress(0)("Street"))
                    p("CityID") = IIf(CStr2(cpAddress(0)("CityID")) = "", "NULL", ReadyString(cpAddress(0)("CityID")))
                    p("CountryID") = IIf(CStr2(cpAddress(0)("CountryID")) = "", "NULL", ReadyString(cpAddress(0)("CountryID")))
                    p("AddressType") = ReadyString(cpAddress(0)("AddressType"))
                    p("LastUpdated") = "getdate()"

                    pSql = "SELECT ID FROM MobizLibraryPeopleAddress WHERE ID = " & ReadyString(cpAddress(0)("ID").ToString())

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(cpAddress(0)("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpAddress(0)("ID").ToString())))
                        ctrUpdated += 1
                    End If
                End If

                pSql = "SELECT * FROM LibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(ppol("ID").ToString())
                Dim cpEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each email As Object In cpEmail
                    pSql = "SELECT ID FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("ID").ToString())

                    p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                    p("PeopleID") = ReadyString(email("PeopleID").ToString())
                    p("EmailAddress") = ReadyString(email("EmailAddress"))
                    p("UserID") = IIf(CStr2(email("UserID")) = "", "NULL", ReadyString(email("UserID")))
                    p("LastUpdated") = "getdate()"
                    p("EmailType") = ReadyString(email("EmailType"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(email("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(email("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next email

                pSql = "SELECT * FROM LibraryPeopleContactTel WHERE PeopleID = " & ReadyString(ppol("ID").ToString())
                Dim cpTel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each tel As Object In cpTel
                    pSql = "SELECT ID FROM MobizLibraryPeopleContactTel WHERE ID = " & ReadyString(tel("ID").ToString())

                    p = New InsertUpdate("MobizLibraryPeopleContactTel")

                    p("PeopleID") = ReadyString(tel("PeopleID").ToString())
                    p("CountryID") = ReadyString(tel("CountryID"))
                    p("AreaCodeID") = ReadyString(tel("AreaCodeID"))
                    p("ContactNo") = ReadyString(tel("ContactNo"))
                    p("ExtensionNo") = ReadyString(tel("ExtensionNo"))
                    p("LastUpdated") = "getdate()"
                    p("ContactType") = ReadyString(tel("ContactType"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(tel("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(tel("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next tel

                pSql = "SELECT * FROM LibraryPeopleContactMobile WHERE PeopleID = " & ReadyString(ppol("ID").ToString())
                Dim cpMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each mobile As Object In cpMobile
                    pSql = "SELECT ID FROM MobizLibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("ID").ToString())

                    p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                    p("PeopleID") = ReadyString(mobile("PeopleID").ToString())
                    p("CountryID") = IIf(CStr2(mobile("CountryID")) = "", "NULL", ReadyString(mobile("CountryID")))
                    p("TelCoPrefixID") = IIf(CStr2(mobile("TelCoPrefixID")) = "", "NULL", ReadyString(mobile("TelCoPrefixID")))
                    p("ContactNo") = ReadyString(mobile("ContactNo"))
                    p("LastUpdated") = "getdate()"
                    p("ContactType") = ReadyString(mobile("ContactType"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(mobile("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(mobile("ID").ToString())))
                        ctrUpdated += 1
                    End If
                Next mobile
            Next ppol

            Common.WebRequest.ProData.CommitTransaction()

            If pRowsOrg.Count > 0 Or pRowsPpol.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "Successfully imported data from CRM. <br><br> No. of new customer's data: " & ctrAdded &
                    "<br> No. of updated customer's data: " & ctrUpdated)
            Else
                Return JsonDictionary.Serialize("Results", "No data available")
            End If

        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function SaveNewPersonProspect() As String
        Dim pSql As String
        Dim strPersonID As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pDT As DataTable
        Dim isExistingRecord As String = CBool2(Common.WebRequest.Data("isCreateFromExistingRecord"))
        Dim tagAsNew As Boolean = False ' for edit
        Dim isSelectedRecordTakenByOtherOrg = False
        Dim customercode As Integer = GenerateCodeNumber(CBool2(Common.WebRequest.Data("IsCustomerSettings")))

        Try
            Common.WebRequest.ProData.BeginTransaction()

            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                Dim signup As New SignUp

                signup.UserName = Common.WebRequest.Data("Username")

                If signup.CheckPeopleUniqueUserName().Rows.Count > 0 Then
                    Return "Username is already taken. Please choose another username."
                End If
            End If

            p("FirstName") = ReadyString(FirstName)
            p("MiddleName") = ReadyString(MiddleName)
            p("Surname") = ReadyString(LastName)
            p("DateOfBirth") = ReadyString(BirthDate)
            p("Position") = ReadyString(Position)
            p("PositionDescription") = ReadyString(PositionDescription)
            p("Tin") = ReadyString(Tin)
            p("IsFromOrg") = ReadyString(0)
            p("IsActive") = If(Status = "Active", "1", "0")
            If WebSession.Instance.PeopleUserID <> Nothing Then
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            End If
            p("LastUpdated") = ReadyString(CDate(DateTime))

            If isExistingRecord Then
                'gamit ni pag ang user kay mag-add gamit ang public organization or selected data during search sa customer/supplier

                'pSql = "SELECT ID FROM MobizLibraryOrganization WHERE OriginalKeyID = " & ReadyString(Common.WebRequest.Data("ExistingCustomerID")) & vbCrLf &
                '        " AND CreatedByOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
                pSql = "SELECT OriginalKeyID, CreatedByOrgID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(PersonID)
                pDT = Common.WebRequest.ProData.Execute(pSql)
                'Check if Company exist in the customer list
                If CStr2(pDT.Rows(0)("OriginalKeyID")) = "" And (CStr2(pDT.Rows(0)("CreatedByOrgID")) <> "" And CStr2(pDT.Rows(0)("CreatedByOrgID")) <> CStr2(Common.WebRequest.Data("OrgID"))) Then
                    '    Return "You have already added <b>" & FirstName & " " & MiddleName & LastName & "</b> as your " & IIf(CBool2(SaveAsCustomer), "customer.", "supplier.")
                    'Else
                    isSelectedRecordTakenByOtherOrg = True
                End If
            End If

            If PersonID = "" Then
                strPersonID = Guid.NewGuid.ToString()

                p("ID") = ReadyString(strPersonID)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                p = New InsertUpdate("MobizLibraryOrganization")

                p("ID") = ReadyString(strPersonID)
                p("PersonFirstName") = ReadyString(FirstName)
                p("PersonMiddleName") = ReadyString(MiddleName)
                p("PersonSurname") = ReadyString(LastName)
                p("PersonDateOfBirth") = ReadyString(BirthDate)
                p("TinNumber") = ReadyString(Tin)
                p("IsOrganization") = "0"
                p("IsActive") = If(Status = "Active", "1", "0")
                p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If isSelectedRecordTakenByOtherOrg Then
                    p("OriginalKeyID") = ReadyString(Common.WebRequest.Data("ExistingCustomerID"))  'isave ang ID sa napili na org
                End If

                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If CBool2(Common.WebRequest.Data("IsCustomerSettings")) Then
                    p = New InsertUpdate("LibraryOrganizationCustomer")
                    If CodeNumber = "" Then
                        p("CustomerCode") = ReadyString(customercode)
                    End If
                    p("CustomerRequirePoNum") = CustomerRequirePoNum
                Else
                    p = New InsertUpdate("LibraryOrganizationSupplier")
                    If CodeNumber = "" Then
                        p("VendorCode") = ReadyString(customercode)
                    End If
                End If

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("ActivationDate") = "GetDate()"
                p("DeActivated") = If(Status = "Active", "0", "1")
                'p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
                'p("DeactivationReason") = ReadyString(DeactivationReason)
                'p("Comments") = ReadyString(Comments)
                p("IsBan") = ReadyString("0")
                p("LastUpdated") = "GetDate()"

                p("CreditTermsID") = Terms
                p("CreditLimit") = ReadyString(Limit)
                p("UnlimitedCredit") = UnlimitedCredit
                p("RequireCnCApproval") = RequireCnCApproval
                p("CreditBegBalance") = ReadyString(Balance)
                p("CreditBalanceAsOf") = ReadyString(AsOf)

                If CBool2(Common.WebRequest.Data("IsCustomerSettings")) Then
                    p("SalesRepID") = SalesRepID
                    p("PriceLevelID") = "NULL" 'PriceLevel
                    p("OrganizationID") = ReadyString(strPersonID)
                    p("ThirdPartyPlatform") = ReadyString(ThirdPartyPlatform)
                    p("ThirdPartyID") = ReadyString(ThirdPartyID)
                    p("ThirdPartyBegBalance") = CDec2(ThirdPartyBegBalance)
                    If ThirdPartyBegBalanceAsOf IsNot Nothing Then
                        p("ThirdPartyBegBalanceAsOf") = ReadyString(ThirdPartyBegBalanceAsOf)
                    End If
                Else
                    p("SupplierID") = ReadyString(strPersonID)
                End If
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                'check first kung ang person kay naa nay tag-iya na org(db), kung wala, itag as sariling record or ang value sa CreatedByOrgID kay ang DB ID, kung naa, add new record
                'kapag dili NULL or equal sa DB ID ang CreatedByOrgID dapat mag create ug new record and copy only the specific data

                pSql = "SELECT OriginalKeyID, CreatedByOrgID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(PersonID)
                Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                If CStr2(row("CreatedByOrgID")) = "" Or CStr2(row("CreatedByOrgID")) = Common.WebRequest.Data("OrgID") Then
                    tagAsNew = False
                    strPersonID = PersonID
                Else
                    tagAsNew = True
                    strPersonID = Guid.NewGuid.ToString()
                    PersonID = strPersonID
                End If

                If tagAsNew Then
                    p = New InsertUpdate("MobizLibraryPeople")

                    p("FirstName") = ReadyString(FirstName)
                    p("MiddleName") = ReadyString(MiddleName)
                    p("Surname") = ReadyString(LastName)
                    p("Tin") = ReadyString(Tin)
                    p("IsFromOrg") = ReadyString(0)
                    p("IsActive") = If(Status = "Active", "1", "0")
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))
                    p("ID") = ReadyString(strPersonID)
                End If

                pSql = "SELECT ID FROM MobizLibraryPeople WHERE ID = " & ReadyString(PersonID)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count = 0 Then
                    p("ID") = ReadyString(strPersonID)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(PersonID))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                p = New InsertUpdate("MobizLibraryOrganization")
                p("PersonFirstName") = ReadyString(FirstName)
                p("PersonMiddleName") = ReadyString(MiddleName)
                p("PersonSurname") = ReadyString(LastName)
                p("TinNumber") = ReadyString(Tin)
                p("IsOrganization") = "0"
                p("IsActive") = If(Status = "Active", "1", "0")
                p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If tagAsNew Then
                    p("OriginalKeyID") = ReadyString(PersonID)
                    pSql = p.GetInsert()
                Else
                    p("PersonDateOfBirth") = ReadyString(BirthDate)
                    pSql = p.GetUpdate(" ID = " & ReadyString(PersonID))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If CBool2(Common.WebRequest.Data("IsCustomerSettings")) Then
                    p = New InsertUpdate("LibraryOrganizationCustomer")

                    If CodeNumber = "" Then
                        p("CustomerCode") = ReadyString(customercode)
                    End If
                Else
                    p = New InsertUpdate("LibraryOrganizationSupplier")

                    If CodeNumber = "" Then
                        p("VendorCode") = ReadyString(customercode)
                    End If
                End If

                'ngano ni? comment for now 5-26-2018
                'p = New InsertUpdate("LibraryOrganizationSupplier") ' dapat diri isave dili sa LibraryOrganizationCustomer --- by arnie 5-2-2018

                p("ActivationDate") = "GetDate()"
                p("DeActivated") = If(Status = "Active", "0", "1")
                'p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
                'p("DeactivationReason") = ReadyString(DeactivationReason)
                'p("Comments") = ReadyString(Comments)
                p("IsBan") = ReadyString("0")
                p("LastUpdated") = "GetDate()"

                p("CreditTermsID") = Terms
                p("CreditLimit") = ReadyString(Limit)
                p("UnlimitedCredit") = UnlimitedCredit
                p("RequireCnCApproval") = RequireCnCApproval
                p("CustomerRequirePoNum") = CustomerRequirePoNum
                p("CreditBegBalance") = ReadyString(Balance)
                p("CreditBalanceAsOf") = ReadyString(AsOf)

                If CBool2(Common.WebRequest.Data("IsCustomerSettings")) Then
                    p("SalesRepID") = SalesRepID
                    p("PriceLevelID") = "NULL" 'PriceLevel
                    p("OrganizationID") = ReadyString(strPersonID)
                    p("ThirdPartyPlatform") = ReadyString(ThirdPartyPlatform)
                    p("ThirdPartyID") = ReadyString(ThirdPartyID)
                    p("ThirdPartyBegBalance") = CDec2(ThirdPartyBegBalance)
                    If ThirdPartyBegBalanceAsOf IsNot Nothing Then
                        p("ThirdPartyBegBalanceAsOf") = ReadyString(ThirdPartyBegBalanceAsOf)
                    End If
                Else
                    p("SupplierID") = ReadyString(strPersonID)
                End If

                If CBool2(Common.WebRequest.Data("IsCustomerSettings")) Then
                    pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(PersonID)
                Else
                    pSql = "SELECT ID FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(PersonID)
                End If

                Dim cusrows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                If cusrows.Count = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    If CustomerID = "" Then ' ginagamit using search customer
                        Return "You have already added <b>" & FirstName & " " & MiddleName & LastName & "</b> as your " & IIf(CBool2(SaveAsCustomer), "customer.", "supplier.")
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(CustomerID))
                    End If
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If

            If isImportToCrm Then
                p = New InsertUpdate("LibraryPeople")

                p("FirstName") = ReadyString(FirstName)
                p("MiddleName") = ReadyString(MiddleName)
                p("Surname") = ReadyString(LastName)
                p("DateOfBirth") = ReadyString(BirthDate)
                p("Position") = ReadyString(Position)
                p("Tin") = ReadyString(Tin)
                p("PositionDescription") = ReadyString(PositionDescription)
                p("IsFromOrg") = ReadyString(0)
                p("IsActive") = If(Status = "Active", "1", "0")
                If WebSession.Instance.PeopleUserID <> Nothing Then
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                End If
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If PersonID = "" Then
                    p("ID") = ReadyString(strPersonID)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonID)))
                End If
            End If

            'kapag true, dili na copyhon ang ibang info sa person like address, contacts, etc.
            If Not tagAsNew Then

                If Not CBool2(isExistingRecord) Then
                    p = New InsertUpdate("MobizLibraryPeopleAddress")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("HouseBuildingNo") = ReadyString(HouseBuildingNo)
                    p("Street") = ReadyString(Street)
                    p("CityID") = If(City = "", "NULL", ReadyString(City))
                    p("Region") = ReadyString(Region)
                    p("CountryID") = If(Country = "", "NULL", ReadyString(Country))
                    p("Island") = ReadyString(Island)
                    p("AddressType") = ReadyString(TypeAddress)
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If AddressID = "" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(AddressID))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleAddress")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("HouseBuildingNo") = ReadyString(HouseBuildingNo)
                        p("Street") = ReadyString(Street)
                        p("CityID") = If(City = "", "NULL", ReadyString(City))
                        p("Region") = ReadyString(Region)
                        p("CountryID") = If(City = "", "NULL", ReadyString(Country))
                        p("Island") = ReadyString(Island)
                        p("AddressType") = ReadyString(TypeAddress)
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If AddressID = "" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                        Else
                            pSql = p.GetUpdate(" ID = " & ReadyString(AddressID))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                End If

                For Each orgaff As Object In ObjPersonOrgAff
                    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                    pSql = "SELECT * FROM MobizLibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(orgaff("OrgID").ToString()) & vbCrLf &
                            " AND PeopleID = " & ReadyString(strPersonID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    p("PeopleID") = ReadyString(strPersonID)
                    p("OrganizationID") = ReadyString(orgaff("OrgID").ToString())
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))
                    p("IsUseForPersonOrgAffiliates") = ReadyString(1)

                    If orgaff("State").ToString() = "Add" Then
                        If pDT.Rows.Count = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                    ElseIf orgaff("State").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactPerson WHERE ID = " & ReadyString(orgaff("ID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next orgaff

                For Each tel As Object In ObjTelephone
                    p = New InsertUpdate("MobizLibraryPeopleContactTel")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                    p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                    p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                    p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                    p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If tel("TelState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf tel("TelState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                    ElseIf tel("TelState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleContactTel")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                        p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                        p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                        p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                        p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If tel("TelState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf tel("TelState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                        ElseIf tel("TelState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryPeopleContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next tel

                For Each mobile As Object In ObjMobile
                    p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                    p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                    p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("ContactType") = If(mobile("ContactType") = Nothing, ReadyString(""), ReadyString(mobile("ContactType").ToString()))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If mobile("MobileState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf mobile("MobileState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                    ElseIf mobile("MobileState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleContactMobile")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                        p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                        p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("ContactType") = If(mobile("ContactType") = Nothing, ReadyString(""), ReadyString(mobile("ContactType").ToString()))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If mobile("MobileState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf mobile("MobileState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                        ElseIf mobile("MobileState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next mobile

                For Each email As Object In ObjEmail
                    Dim emaillinkid As String = Guid.NewGuid.ToString()
                    p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("EmailType") = If(email("EmailType") = Nothing, ReadyString(""), ReadyString(email("EmailType").ToString()))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then ' pag nag register via generated public link
                        p("IsEmailConfirmed") = CInt2(email("IsEmailConfirmed"))
                        If CBool2(email("IsEmailConfirmed")) = False Then
                            p("EmailActivationLinkID") = ReadyString(emaillinkid)
                        End If
                    End If

                    If email("EmailState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf email("EmailState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                    ElseIf email("EmailState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleContactEmail")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("EmailType") = If(email("EmailType") = Nothing, ReadyString(""), ReadyString(email("EmailType").ToString()))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                            p("IsEmailConfirmed") = CInt2(email("IsEmailConfirmed"))
                            If CBool2(email("IsEmailConfirmed")) = False Then
                                p("EmailActivationLinkID") = ReadyString(emaillinkid)
                            End If
                        End If

                        If email("EmailState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf email("EmailState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                        ElseIf email("EmailState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If

                    If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                        If CBool2(email("IsEmailConfirmed")) = False Then
                            Dim link As String = Common.WebRequest.Data("Url") & "/Ecommerce/Confirm?id=" & emaillinkid &
                            "&c=" & GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper &
                            "&OrgID=" & Common.WebRequest.Data("OrgID") &
                            "&acctype=" & IIf(CBool2(Common.WebRequest.Data("IsCustomerSettings")), "cus", "sup") &
                            "&mainurl=" & Common.WebRequest.Data("Url")

                            Dim msg As String = "<table style='width:100%; border:none; background-color: #f5f5f5;'>" +
                                                    "<tr><td style='height:100px; text-align:center; padding:20px; vertical-align:middle; background-color: #0083c1; font-size: 40px; color: #FFF;'>WELCOME</td></tr>" +
                                                    "<tr><td style='padding:20px; font-size: 20px;'>Hello " + FirstName.ToUpper() + ",<br /><br />Thanks for providing us your information! Before you can login, please click the button below to activate your account.<br /></td></tr>" +
                                                    "<tr><td style='padding:20px; font-size: 20px; vertical-align:middle; text-align:center;'><a href='" & link & "' style='background-color: #5cb85c; border: none; border-radius: 5px; padding: 15px; color: #fff; text-decoration:none;'>ACTIVATE MY ACCOUNT</a><br /><br /></td></tr>" +
                                                    "<tr><td style='padding:20px; vertical-align:middle; background-color: #dadada; font-size: 16px;'>To know more about Mobiz, please <a href='http://mobiz-apps.com' target='_blank' style='text-decoration:none;'>click here</a>.</td></tr>" +
                                                "</table>"

                            SendMail("mobizecommerce@outlook.com",
                                 email("EmailAdd").ToString(),
                                 "E-commerce",
                                 "Welcome to Mobiz E-commerce!",
                                 msg,
                                 "smtp.live.com",
                                 587,
                                 "mobizecommerce@outlook.com",
                                 "m0b!zp30pl30nly!", , , , , True)
                        End If

                    End If
                Next email

                For Each elem As Object In ObjPersonElementary
                    p = New InsertUpdate("MobizLibraryPeopleEducation")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("AcademicLevel") = ReadyString(elem("Level"))
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("InstitutionName") = ReadyString(elem("Name"))
                    p("YearStarted") = ReadyString(elem("Started"))
                    p("YearGraduated") = ReadyString(elem("Graduated"))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If elem("State").ToString() = "add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf elem("State").ToString() = "edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(elem("ID").ToString()))

                    ElseIf elem("State").ToString() = "remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleEducation WHERE ID = " & ReadyString(elem("ID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleEducation")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("AcademicLevel") = ReadyString(elem("Level"))
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("InstitutionName") = ReadyString(elem("Name"))
                        p("YearStarted") = ReadyString(elem("Started"))
                        p("YearGraduated") = ReadyString(elem("Graduated"))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If elem("State").ToString() = "add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf elem("State").ToString() = "edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(elem("ID").ToString()))

                        ElseIf elem("State").ToString() = "remove" Then
                            pSql = "DELETE FROM LibraryPeopleEducation WHERE ID = " & ReadyString(elem("ID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next elem

                For Each hs As Object In ObjPersonHighSchool
                    p = New InsertUpdate("MobizLibraryPeopleEducation")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("AcademicLevel") = ReadyString(hs("Level"))
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("InstitutionName") = ReadyString(hs("Name"))
                    p("YearStarted") = ReadyString(hs("Started"))
                    p("YearGraduated") = ReadyString(hs("Graduated"))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If hs("State").ToString() = "add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf hs("State").ToString() = "edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(hs("ID").ToString()))

                    ElseIf hs("State").ToString() = "remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleEducation WHERE ID = " & ReadyString(hs("ID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleEducation")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("AcademicLevel") = ReadyString(hs("Level"))
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("InstitutionName") = ReadyString(hs("Name"))
                        p("YearStarted") = ReadyString(hs("Started"))
                        p("YearGraduated") = ReadyString(hs("Graduated"))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If hs("State").ToString() = "add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf hs("State").ToString() = "edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(hs("ID").ToString()))

                        ElseIf hs("State").ToString() = "remove" Then
                            pSql = "DELETE FROM LibraryPeopleEducation WHERE ID = " & ReadyString(hs("ID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next hs

                For Each college As Object In ObjPersonCollege
                    p = New InsertUpdate("MobizLibraryPeopleEducation")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("AcademicLevel") = ReadyString(college("Level"))
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("InstitutionName") = ReadyString(college("Name"))
                    p("YearStarted") = ReadyString(college("Started"))
                    p("YearGraduated") = ReadyString(college("Graduated"))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If college("State").ToString() = "add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf college("State").ToString() = "edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(college("ID").ToString()))

                    ElseIf college("State").ToString() = "remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleEducation WHERE ID = " & ReadyString(college("ID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleEducation")

                        p("PeopleID") = ReadyString(strPersonID)
                        p("AcademicLevel") = ReadyString(college("Level"))
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("InstitutionName") = ReadyString(college("Name"))
                        p("YearStarted") = ReadyString(college("Started"))
                        p("YearGraduated") = ReadyString(college("Graduated"))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If college("State").ToString() = "add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf college("State").ToString() = "edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(college("ID").ToString()))

                        ElseIf college("State").ToString() = "remove" Then
                            pSql = "DELETE FROM LibraryPeopleEducation WHERE ID = " & ReadyString(college("ID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next college

                For Each lvl As Object In PriceLevel
                    p = New InsertUpdate("LibraryOrganizationPriceLevel")

                    p("PriceLevelID") = ReadyString(lvl("ID").ToString())
                    p("OrganizationID") = ReadyString(strPersonID)
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If lvl("State").ToString() = "add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    ElseIf lvl("State").ToString() = "remove" Then
                        pSql = "DELETE FROM LibraryOrganizationPriceLevel WHERE ID = " & ReadyString(lvl("DBID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next lvl

                If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                    p = New InsertUpdate("MobizLibraryOrganizationFowst") 'SAVE ADDRESS

                    pSql = "SELECT * FROM MobizLibraryOrganizationFowst WHERE StreetNumber = " & ReadyString(Trim(HouseBuildingNo & " " & Street & " " & Common.WebRequest.Data("CityName") & " " & Common.WebRequest.Data("CountryName")))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("OrganizationID") = ReadyString(strPersonID)
                        p("StreetNumber") = ReadyString(Trim(HouseBuildingNo & " " & Street & " " & Common.WebRequest.Data("CityName") & " " & Common.WebRequest.Data("CountryName")))
                        p("IsActive") = 1
                        p("ID") = ReadyString(Guid.NewGuid.ToString())

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If

                    p = New InsertUpdate("MobizLibraryPeopleUserAccount")

                    p("Username") = ReadyString(Common.WebRequest.Data("Username"))
                    p("ActivationDate") = "GetDate()"
                    p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("Password")))

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(strPersonID))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("PeopleID") = ReadyString(strPersonID)
                        Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                    End If

                    p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                    p("SecurityQuestion") = ReadyString(Common.WebRequest.Data("SecQ1"))
                    p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("SecQ1Ans"))) 'ReadyString(SecurityQuestion1Answer)

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("SecQ1ID")))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("PeopleID") = ReadyString(strPersonID)
                        Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                    End If

                    p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                    p("SecurityQuestion") = ReadyString(Common.WebRequest.Data("SecQ2"))
                    p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("SecQ2Ans"))) 'ReadyString(SecurityQuestion1Answer)

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("SecQ2ID")))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("PeopleID") = ReadyString(strPersonID)
                        Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                    End If

                    p = New InsertUpdate("MobizLibrarySurveyQuestionaireLink")

                    p("DateEdited") = "getdate()"
                    p("IsDisableLink") = 1

                    Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" Link = " & ReadyString(Common.WebRequest.Data("Link"))))
                End If
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPeople'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return strPersonID.ToString()

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Class SaveNewOrgProspectInput
        Public Class SalesRep
            Public PeopleID As String
            Public State As String
        End Class
        Public Class SalesRepsClass
            Inherits List(Of SalesRep)
        End Class
        Public SalesReps As SalesRepsClass
        Public SalesChannelID As String
    End Class

    Function SaveNewOrgProspect() As String
        Dim pSql As String
        Dim strOrgID As String
        Dim p As New InsertUpdate("MobizLibraryOrganization")
        Dim pDT As DataTable
        Dim isExistingRecord As String = CBool2(Common.WebRequest.Data("isCreateFromExistingRecord"))
        Dim tagAsNew As Boolean = False ' for edit
        Dim customercode As Integer = GenerateCodeNumber(CBool2(Common.WebRequest.Data("IsCustomerSettings")))

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("CompanyName") = ReadyString(CompanyName)
            p("BusinessTradeName") = ReadyString(CompanyBusinessTradeName)
            p("TinNumber") = ReadyString(Tin)
            p("VatType") = ReadyString(VatType)
            p("OfficeAddressBuildingNo") = ReadyString(CompanyBuildingAddress)
            p("OfficeAddressStreet") = ReadyString(CompanyStreetAddress)
            p("Island") = ReadyString(Island)
            p("OfficeAddressCityID") = If(CompanyCityAddress = "", "NULL", ReadyString(CompanyCityAddress))
            p("OfficeAddressCountryID") = If(CompanyCountryAddress = "", "NULL", ReadyString(CompanyCountryAddress))
            p("Region") = ReadyString(CompanyRegionAddress)
            p("Background") = ReadyString(CompanyBackground)
            p("Mission") = ReadyString(CompanyMission)
            p("Vision") = ReadyString(CompanyVision)
            p("FoundingDate") = ReadyString(CompanyFoundingDate)
            p("IsActive") = If(Status = "Active", "1", "0")
            p("IndustryClassID") = If(BusinessIndustryID = "", "NULL", ReadyString(BusinessIndustryID))
            p("BusinessIndustryText") = If(String.IsNullOrWhiteSpace(BusinessIndustryText), "NULL", ReadyString(BusinessIndustryText))
            If WebSession.Instance.PeopleUserID <> Nothing Then
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            End If
            p("LastUpdated") = ReadyString(CDate(DateTime))
            p("NoOfEmployees") = CInt2(NoOfEmployees)
            p("RevenuePerYear") = ReadyString(RevenuePerYear)
            p("ScopeOfOperations") = ReadyString(Scope)
            p("NoOfFactory") = CInt2(NoOfFactories)
            p("NoOfOffice") = CInt2(NoOfOffices)
            p("NoOfWarehouse") = CInt2(NoOfWarehouses)
            p("NoOfStore") = CInt2(NoOfStores)
            p("NoOfTruck") = CInt2(NoOfTrucks)
            p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))

            If isExistingRecord Then
                'gamit ni pag ang user kay mag-add gamit ang public organization or selected data during search sa customer/supplier

                'pSql = "SELECT ID FROM MobizLibraryOrganization WHERE OriginalKeyID = " & ReadyString(Common.WebRequest.Data("ExistingCustomerID")) & vbCrLf &
                '        " AND CreatedByOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
                pSql = "SELECT OriginalKeyID, CreatedByOrgID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(CompanyID)
                pDT = Common.WebRequest.ProData.Execute(pSql)
                'Check if Company exist in the customer list
                If CStr2(pDT.Rows(0)("OriginalKeyID")) = "" And (CStr2(pDT.Rows(0)("CreatedByOrgID")) <> "" And CStr2(pDT.Rows(0)("CreatedByOrgID")) <> CStr2(Common.WebRequest.Data("OrgID"))) Then
                    'Return "You already added <b>" & CompanyName & "</b> as your " & IIf(CBool2(SaveAsCustomer), "customer.", "supplier.")
                    'Else
                    CompanyID = "" 'remove this so that it will create new record
                    p("OriginalKeyID") = ReadyString(Common.WebRequest.Data("ExistingCustomerID"))  'isave ang ID sa napili na org
                End If
            End If

            If CompanyID = "" Then

                strOrgID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strOrgID)
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If SaveAsCustomer Then
                    p = New InsertUpdate("LibraryOrganizationCustomer")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("OrganizationID") = ReadyString(strOrgID)
                    p("ActivationDate") = "GetDate()"
                    p("DeActivated") = If(Status = "Active", "0", "1")
                    'p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
                    'p("DeactivationReason") = ReadyString(DeactivationReason)
                    'p("Comments") = ReadyString(Comments)
                    p("IsBan") = ReadyString("0")
                    p("LastUpdated") = "GetDate()"

                    If CodeNumber = "" Then
                        p("CustomerCode") = ReadyString(customercode)
                    End If
                    p("SalesRepID") = SalesRepID
                    'p("PriceLevelID") = PriceLevelID
                    p("CreditTermsID") = Terms
                    p("CreditLimit") = ReadyString(Limit)
                    p("UnlimitedCredit") = UnlimitedCredit
                    p("RequireCnCApproval") = RequireCnCApproval
                    p("CreditBegBalance") = ReadyString(Balance)
                    p("CreditBalanceAsOf") = ReadyString(AsOf)
                    p("ThirdPartyPlatform") = ReadyString(ThirdPartyPlatform)
                    p("ThirdPartyID") = ReadyString(ThirdPartyID)
                    p("ThirdPartyBegBalance") = CDec2(ThirdPartyBegBalance)
                    p("SingleCreditTerm") = If(CBool2(SingleCreditTerm), 1, 0)
                    p("SalesChannelID") = If(String.IsNullOrWhiteSpace(SalesChannelID), "NULL", ReadyString(SalesChannelID))
                    If ThirdPartyBegBalanceAsOf IsNot Nothing Then
                        p("ThirdPartyBegBalanceAsOf") = ReadyString(ThirdPartyBegBalanceAsOf)
                    End If

                    p("DefaultHotButtonTemplateID") = If(DefaultHotButtonTemplateID = "", "NULL", ReadyString(DefaultHotButtonTemplateID))

                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Else
                'check first kung ang person kay naa nay tag-iya na org(db), kung wala, itag as sariling record or ang value sa CreatedByOrgID kay ang DB ID, kung naa, add new record
                'kapag dili NULL or equal sa DB ID ang CreatedByOrgID dapat mag create ug new record and copy only the specific data

                pSql = "SELECT OriginalKeyID, CreatedByOrgID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(CompanyID)
                Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                If CStr2(row("CreatedByOrgID")) = "" Or CStr2(row("CreatedByOrgID")) = Common.WebRequest.Data("OrgID") Then
                    tagAsNew = False
                Else
                    tagAsNew = True
                End If

                If tagAsNew Then
                    'kani lang ang na info ang copyhon
                    strOrgID = Guid.NewGuid.ToString()
                    p = New InsertUpdate("MobizLibraryOrganization")
                    p("CompanyName") = ReadyString(CompanyName)
                    p("TinNumber") = ReadyString(Tin)
                    p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
                    p("IsActive") = If(Status = "Active", "1", "0")
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))
                    p("OriginalKeyID") = ReadyString(CompanyID) 'isave ang ID sa napili na org
                    p("ID") = ReadyString(strOrgID)

                    pSql = p.GetInsert()
                Else
                    strOrgID = CompanyID
                    pSql = p.GetUpdate(" ID = " & ReadyString(CompanyID))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If SaveAsCustomer Then
                    If isExistingRecord Then
                        pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(strOrgID)

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                            Return "You have already added <b>" & CompanyName & "</b> as your " & IIf(CBool2(SaveAsCustomer), "customer.", "supplier.")
                        End If
                    End If

                    p = New InsertUpdate("LibraryOrganizationCustomer")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("ActivationDate") = "GetDate()"
                    p("DeActivated") = If(Status = "Active", "0", "1")
                    'p("DeactivationDate") = IIf(Convert.ToInt32(IsDeactivated), ReadyString(DeactivationDate), "NULL")
                    'p("DeactivationReason") = ReadyString(DeactivationReason)
                    'p("Comments") = ReadyString(Comments)
                    p("IsBan") = ReadyString("0")
                    p("LastUpdated") = "GetDate()"

                    p("SalesRepID") = SalesRepID
                    'p("PriceLevelID") = PriceLevelID
                    p("CreditTermsID") = Terms
                    p("CreditLimit") = ReadyString(Limit)
                    p("UnlimitedCredit") = UnlimitedCredit
                    p("RequireCnCApproval") = RequireCnCApproval
                    p("CreditBegBalance") = ReadyString(Balance)
                    p("CreditBalanceAsOf") = ReadyString(AsOf)
                    p("ThirdPartyPlatform") = ReadyString(ThirdPartyPlatform)
                    p("ThirdPartyID") = ReadyString(ThirdPartyID)
                    p("ThirdPartyBegBalance") = CDec2(ThirdPartyBegBalance)
                    p("SingleCreditTerm") = If(CBool2(SingleCreditTerm), 1, 0)
                    p("SalesChannelID") = If(String.IsNullOrWhiteSpace(SalesChannelID), "NULL", ReadyString(SalesChannelID))
                    If ThirdPartyBegBalanceAsOf IsNot Nothing Then
                        p("ThirdPartyBegBalanceAsOf") = ReadyString(ThirdPartyBegBalanceAsOf)
                    End If

                    p("DefaultHotButtonTemplateID") = If(DefaultHotButtonTemplateID = "", "NULL", ReadyString(DefaultHotButtonTemplateID))
                    If CodeNumber = "" Then
                        p("CustomerCode") = ReadyString(customercode)
                    End If

                    ' BEGIN transferred in a loop
                    'p("SalesRepID") = SalesRepID
                    ''p("PriceLevelID") = PriceLevelID
                    'p("CreditTermsID") = Terms
                    'p("CreditLimit") = ReadyString(Limit)
                    'p("UnlimitedCredit") = UnlimitedCredit
                    'p("RequireCnCApproval") = RequireCnCApproval
                    'p("CreditBegBalance") = ReadyString(Balance)
                    'p("CreditBalanceAsOf") = ReadyString(AsOf)
                    'p("ThirdPartyPlatform") = ReadyString(ThirdPartyPlatform)
                    'p("ThirdPartyID") = ReadyString(ThirdPartyID)
                    'p("ThirdPartyBegBalance") = CDec2(ThirdPartyBegBalance)
                    'If ThirdPartyBegBalanceAsOf IsNot Nothing Then
                    '    p("ThirdPartyBegBalanceAsOf") = ReadyString(ThirdPartyBegBalanceAsOf)
                    'End If
                    ' END transferred in a loop

                    p("DefaultHotButtonTemplateID") = If(DefaultHotButtonTemplateID = "", "NULL", ReadyString(DefaultHotButtonTemplateID))
                    p("CustomerRequirePoNum") = If(CustomerRequirePoNum, 1, 0)

                    pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(strOrgID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If pDT.Rows.Count = 0 Then
                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0)("ID").ToString()))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Else
                    If isExistingRecord Then
                        pSql = "SELECT ID FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(strOrgID)

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                            Return "You have already added <b>" & CompanyName & "</b> as your supplier."
                        End If
                    End If
                End If
            End If

            If isImportToCrm Then
                p = New InsertUpdate("LibraryOrganization") ' FOR CRM

                p("CompanyName") = ReadyString(CompanyName)
                p("TinNumber") = ReadyString(Tin)
                p("OfficeAddressBuildingNo") = ReadyString(CompanyBuildingAddress)
                p("OfficeAddressStreet") = ReadyString(CompanyStreetAddress)
                p("Island") = ReadyString(Island)
                p("OfficeAddressCityID") = If(CompanyCityAddress = "", "NULL", ReadyString(CompanyCityAddress))
                p("OfficeAddressCountryID") = If(CompanyCountryAddress = "", "NULL", ReadyString(CompanyCountryAddress))
                p("Region") = ReadyString(CompanyRegionAddress)
                p("Background") = ReadyString(CompanyBackground)
                p("Mission") = ReadyString(CompanyMission)
                p("Vision") = ReadyString(CompanyVision)
                p("FoundingDate") = ReadyString(CompanyFoundingDate)
                p("IsActive") = If(Status = "Active", "1", "0")
                p("IndustryClassID") = If(BusinessIndustryID = "", "NULL", ReadyString(BusinessIndustryID))
                'p("BusinessIndustryText") = If(String.IsNullOrWhiteSpace(BusinessIndustryText), "NULL", ReadyString(BusinessIndustryText))
                If WebSession.Instance.PeopleUserID <> Nothing Then
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                End If
                p("LastUpdated") = ReadyString(CDate(DateTime))
                p("NoOfEmployees") = CInt2(NoOfEmployees)
                p("RevenuePerYear") = ReadyString(RevenuePerYear)
                p("ScopeOfOperations") = ReadyString(Scope)
                p("NoOfFactory") = CInt2(NoOfFactories)
                p("NoOfOffice") = CInt2(NoOfOffices)
                p("NoOfWarehouse") = CInt2(NoOfWarehouses)
                p("NoOfStore") = CInt2(NoOfStores)
                p("NoOfTruck") = CInt2(NoOfTrucks)

                If CompanyID = "" Then
                    p("ID") = ReadyString(strOrgID)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(CompanyID)))
                End If
            End If

            'kapag true, dili na copyhon ang ibang info sa company like contactperson, contacts, etc.
            If Not tagAsNew Then
                For Each vprices As Object In ObjNewVendorPrices
                    'for vendor only
                    p = New InsertUpdate("LibraryVendorPrices")

                    p("Price") = CDec2(vprices("NewPrice"))
                    p("LastUpdated") = "getdate()"
                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ProductID = " & ReadyString(vprices("ID")) & " AND VendorID = " & ReadyString(vprices("VendorID")))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("ProductID") = ReadyString(vprices("ID"))
                        p("VendorID") = ReadyString(vprices("VendorID"))
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next

                For Each outlet As Object In ObjOutlet
                    Dim strOutletID As String
                    p = New InsertUpdate("MobizLibraryOrganizationFOWST")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("Name") = ReadyString(outlet("Name"))
                    p("NetSuiteID") = ReadyString(outlet("ThirdPartyID"))
                    p("StreetNumber") = ReadyString(outlet("HouseNo"))
                    p("Street") = ReadyString(outlet("Street"))
                    p("FowstCityID") = IIf(outlet("City") = "", "NULL", ReadyString(outlet("City")))
                    p("FowstCountryID") = IIf(outlet("Country") = "", "NULL", ReadyString(outlet("Country")))

                    If outlet("State") = "add" Then
                        strOutletID = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strOutletID)
                        p("IsActive") = "1"
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    ElseIf outlet("State") = "edit" Then
                        strOutletID = outlet("ID")
                        pSql = p.GetUpdate(" ID = " & ReadyString(strOutletID))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        strOutletID = outlet("ID")
                    End If

                    If outlet("State") <> "Deactivate" Then
                        p("IsActive") = ReadyString(CBool2(outlet("IsActive")))
                        pSql = p.GetUpdate(" ID = " & ReadyString(strOutletID))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        For Each tel As Object In outlet("Telephone")
                            p = New InsertUpdate("MobizLibraryOrganizationFOWSTTel")

                            p("FowstID") = ReadyString(strOutletID)
                            p("CountryID") = ReadyString(tel("CountryCodeID"))
                            p("AreaCodeID") = ReadyString(tel("AreaCodeID"))
                            p("ContactNo") = ReadyString(tel("TelephoneNo"))
                            p("ExtensionNo") = ReadyString(tel("ExtensionNo"))
                            p("ContactType") = ReadyString(tel("ContactType"))
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("LastUpdated") = "getdate()"

                            If tel("TelState") = "Add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.Execute(pSql)
                            ElseIf tel("TelState") = "Edit" Then
                                pSql = p.GetUpdate("ID = " & ReadyString(tel("TelID")))
                                Common.WebRequest.ProData.Execute(pSql)
                            Else
                                pSql = "DELETE FROM MobizLibraryOrganizationFOWSTTel WHERE ID = " & ReadyString(tel("TelID"))
                                Common.WebRequest.ProData.Execute(pSql)
                            End If
                        Next tel

                        For Each mobile As Object In outlet("Mobile")
                            p = New InsertUpdate("MobizLibraryOrganizationFOWSTMobile")

                            p("FowstID") = ReadyString(strOutletID)
                            p("CountryID") = ReadyString(mobile("CountryCodeID"))
                            p("TelCoPrefixID") = ReadyString(mobile("TelCodeID"))
                            p("ContactNo") = ReadyString(mobile("MobileNo"))
                            p("ContactType") = ReadyString(mobile("ContactType"))
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("LastUpdated") = "getdate()"

                            If mobile("MobileState") = "Add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.Execute(pSql)
                            ElseIf mobile("MobileState") = "Edit" Then
                                pSql = p.GetUpdate("ID = " & ReadyString(mobile("MobileID")))
                                Common.WebRequest.ProData.Execute(pSql)
                            Else
                                pSql = "DELETE FROM MobizLibraryOrganizationFOWSTMobile WHERE ID = " & ReadyString(mobile("MobileID"))
                                Common.WebRequest.ProData.Execute(pSql)
                            End If
                        Next mobile

                        For Each email As Object In outlet("Email")
                            p = New InsertUpdate("MobizLibraryOrganizationFOWSTEmail")

                            p("FowstID") = ReadyString(strOutletID)
                            p("EmailAddress") = ReadyString(email("EmailAdd"))
                            p("EmailType") = ReadyString(email("EmailType"))
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("LastUpdated") = "getdate()"

                            If email("EmailState") = "Add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.Execute(pSql)
                            ElseIf email("EmailState") = "Edit" Then
                                pSql = p.GetUpdate("ID = " & ReadyString(email("EmailAddID")))
                                Common.WebRequest.ProData.Execute(pSql)
                            Else
                                pSql = "DELETE FROM MobizLibraryOrganizationFOWSTEmail WHERE ID = " & ReadyString(email("EmailAddID"))
                                Common.WebRequest.ProData.Execute(pSql)
                            End If
                        Next email

                        For Each department As Object In outlet("Department")
                            p = New InsertUpdate("LibraryFowstBin")

                            p("FowstID") = ReadyString(strOutletID)
                            p("Name") = ReadyString(department("Department"))
                            p("NetSuiteID") = ReadyString(department("ThirdPartyID"))
                            p("IsActive") = ReadyString(CBool2(department("IsActive")))

                            If department("State") = "add" Then
                                p("IsActive") = "1"
                                p("ID") = ReadyString(department("ID").ToString())

                                If department.ContainsKey("RelatedFowstBinID") Then
                                    p("RelatedFowstBinID") = ReadyString(department("RelatedFowstBinID"))
                                End If

                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf department("State") = "edit" Or department("State") = "Activate" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(department("ID")))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            Else
                                p("IsActive") = "0"
                                pSql = p.GetUpdate(" ID = " & ReadyString(department("ID")))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        Next department
                    Else
                        p("IsActive") = "0"
                        pSql = p.GetUpdate(" ID = " & ReadyString(outlet("ID")))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next outlet

                ' do this after after FOWST's
                WebRequest.Instance.Business.LibraryOrganizationFOWST.AddOrUpdateFromMainAddress(vOrgID:=strOrgID, vBuildingNum:=CompanyBuildingAddress, vStreet:=CompanyStreetAddress, vCityID:=CompanyCityAddress, vCountryID:=CompanyCountryAddress)

                For Each lvl As Object In PriceLevel
                    p = New InsertUpdate("LibraryOrganizationPriceLevel")

                    p("PriceLevelID") = ReadyString(lvl("ID").ToString())
                    p("OrganizationID") = ReadyString(strOrgID)
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If lvl("State").ToString() = "add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    ElseIf lvl("State").ToString() = "remove" Then
                        pSql = "DELETE FROM LibraryOrganizationPriceLevel WHERE ID = " & ReadyString(lvl("DBID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next lvl

                For Each orgaff As Object In ObjOrgAff
                    p = New InsertUpdate("MobizLibraryOrganizationAffiliates")

                    pSql = "SELECT * FROM MobizLibraryOrganizationAffiliates WHERE OrganizationID = " & ReadyString(strOrgID) & vbCrLf &
                            " AND AffiliatesID = " & ReadyString(orgaff("OrgID").ToString())
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    p("AffiliatesID") = ReadyString(orgaff("OrgID").ToString())
                    p("OrganizationID") = ReadyString(strOrgID)
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If orgaff("State").ToString() = "Add" Then
                        If pDT.Rows.Count = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                    ElseIf orgaff("State").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationAffiliates WHERE ID = " & ReadyString(orgaff("ID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next orgaff

                For Each orgaff As Object In ObjAffiliatedPerson
                    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                    pSql = "SELECT * FROM MobizLibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(strOrgID) & vbCrLf &
                            " AND PeopleID = " & ReadyString(orgaff("PeopleID").ToString())
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    p("PeopleID") = ReadyString(orgaff("PeopleID").ToString())
                    p("OrganizationID") = ReadyString(strOrgID)
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))
                    p("IsUseForPersonOrgAffiliates") = ReadyString(1)

                    If orgaff("State").ToString() = "Add" Then
                        If pDT.Rows.Count = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                    ElseIf orgaff("State").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactPerson WHERE ID = " & ReadyString(orgaff("ID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next orgaff

                For Each tel As Object In ObjTelephone
                    p = New InsertUpdate("MobizLibraryOrganizationContactTel")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("CountryID") = If(tel("CountryCodeID") = Nothing, "Null", ReadyString(tel("CountryCodeID").ToString()))
                    p("AreaCodeID") = If(tel("AreaCodeID") = Nothing, "Null", ReadyString(tel("AreaCodeID").ToString()))
                    p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                    p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                    p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If tel("TelState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf tel("TelState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                    ElseIf tel("TelState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryOrganizationContactTel")

                        p("OrganizationID") = ReadyString(strOrgID)
                        p("CountryID") = If(tel("CountryCodeID") = Nothing, "Null", ReadyString(tel("CountryCodeID").ToString()))
                        p("AreaCodeID") = If(tel("AreaCodeID") = Nothing, "Null", ReadyString(tel("AreaCodeID").ToString()))
                        p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                        p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                        p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If tel("TelState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf tel("TelState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                        ElseIf tel("TelState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryOrganizationContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next tel

                For Each mobile As Object In ObjMobile
                    p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                    p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                    p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("ContactType") = ReadyString(mobile("ContactType").ToString())
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If mobile("MobileState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf mobile("MobileState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                    ElseIf mobile("MobileState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryOrganizationContactMobile")

                        p("OrganizationID") = ReadyString(strOrgID)
                        p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                        p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                        p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("ContactType") = ReadyString(mobile("ContactType").ToString())
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If mobile("MobileState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf mobile("MobileState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                        ElseIf mobile("MobileState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryOrganizationContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next mobile

                For Each email As Object In ObjEmail
                    p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("EmailType") = ReadyString(CStr2(email("EmailType")))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If email("EmailState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf email("EmailState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                    ElseIf email("EmailState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryOrganizationContactEmail")

                        p("OrganizationID") = ReadyString(strOrgID)
                        p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("EmailType") = ReadyString(CStr2(email("EmailType")))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If email("EmailState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf email("EmailState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                        ElseIf email("EmailState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryOrganizationContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next email

                For Each website As Object In ObjWebsite
                    p = New InsertUpdate("MobizLibraryOrganizationContactWebsite")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("Website") = ReadyString(website("Website").ToString())
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If website("WebsiteState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf website("WebsiteState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(website("WebsiteID").ToString()))

                    ElseIf website("WebsiteState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactWebsite WHERE ID = " & ReadyString(website("WebsiteID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryOrganizationContactWebsite")

                        p("OrganizationID") = ReadyString(strOrgID)
                        p("Website") = ReadyString(website("Website").ToString())
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If website("WebsiteState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf website("WebsiteState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(website("WebsiteID").ToString()))

                        ElseIf website("WebsiteState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryOrganizationContactWebsite WHERE ID = " & ReadyString(website("WebsiteID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Next website

                If CusCreditTerms IsNot Nothing Then
                    For Each credit As Object In CusCreditTerms

                        Dim creditid As String
                        If String.IsNullOrWhiteSpace(credit("FowstID")) Then
                            p = New InsertUpdate("LibraryOrganizationCustomer")
                        Else
                            p = New InsertUpdate("LibraryOrganizationCustomerCreditTerms")
                            p("FowstID") = ReadyString(credit("FowstID"))
                        End If

                        p("OrganizationID") = ReadyString(strOrgID)
                        p("SalesRepID") = IIf(IsGuid(credit("SalesRepID")), ReadyString(credit("SalesRepID")), "NULL")
                        'p("PriceLevelID") = PriceLevelID
                        p("CreditTermsID") = IIf(Not String.IsNullOrWhiteSpace(credit("TermID")), ReadyString(credit("TermID")), "NULL")
                        p("CreditLimit") = ReadyString(credit("CreditLimit"))
                        p("UnlimitedCredit") = CInt2(credit("UnlimitedCredit"))
                        p("RequireCnCApproval") = CInt2(credit("RequireCnCApproval"))
                        p("CreditBegBalance") = ReadyString(credit("CreditBegBalance"))
                        p("CreditBalanceAsOf") = IIf(IsDate(credit("CreditAsOf")), ReadyString(credit("CreditAsOf")), "NULL")
                        p("ThirdPartyBegBalance") = IIf(Not String.IsNullOrWhiteSpace(credit("ThirdPartyBegBalance")), ReadyString(credit("ThirdPartyBegBalance")), "NULL")
                        p("CustomerRequirePoNum") = CInt2(credit("CustomerRequirePoNum"))
                        p("ThirdPartyBegBalanceAsOf") = IIf(IsDate(credit("ThirdPartyBegBalanceAsOf")), ReadyString(credit("ThirdPartyBegBalanceAsOf")), "NULL")
                        p("LastUpdated") = "getdate()"

                        If String.IsNullOrWhiteSpace(credit("FowstID")) Then
                            Common.WebRequest.ProData.Execute(p.GetUpdate("OrganizationID = " & ReadyString(strOrgID)))
                            Continue For
                        End If

                        If credit.ContainsKey("ID") = False Then
                            creditid = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(creditid)
                            p("EncodedBy") = ReadyString(Common.WebRequest.Data("UserID"))
                            p("EncodedDate") = "getdate()"
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        Else
                            creditid = credit("ID").ToString()
                            p("UpdatedBy") = ReadyString(Common.WebRequest.Data("UserID"))
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(credit("ID"))))
                        End If

                        For Each lvl As Object In credit("PriceLevels")
                            p = New InsertUpdate("LibraryOrganizationPriceLevel")

                            p("CreditTermID") = ReadyString(creditid)
                            p("PriceLevelID") = ReadyString(lvl("ID").ToString())
                            p("OrganizationID") = ReadyString(strOrgID)
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("LastUpdated") = ReadyString(CDate(DateTime))

                            If lvl("State").ToString() = "add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf lvl("State").ToString() = "remove" Then
                                pSql = "DELETE FROM LibraryOrganizationPriceLevel WHERE ID = " & ReadyString(lvl("DBID").ToString())
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        Next lvl
                    Next credit
                End If

                For Each cpeople As Object In ObjContactPerson
                    Dim ppolID As String = Guid.NewGuid.ToString()

                    If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                        Dim signup As New SignUp

                        signup.UserName = cpeople("Username")

                        If signup.CheckPeopleUniqueUserName().Rows.Count > 0 Then
                            Return "Username is already taken. Please choose another username."
                        End If
                    End If

                    p = New InsertUpdate("MobizLibraryPeople")

                    p("FirstName") = ReadyString(cpeople("Firstname")) ' remove the ToString()
                    p("MiddleName") = ReadyString(cpeople("Middlename"))  ' remove the ToString()
                    p("Surname") = ReadyString(cpeople("Lastname"))  ' remove the ToString()
                    Try
                        p("Tin") = ReadyString(cpeople("TIN"))  ' remove the ToString()
                    Catch ex As Exception
                        Debug.WriteLine(ex.Message & ". ref: P4N6WP7SZB")
                    End Try
                    p("Position") = ReadyString(cpeople("Position"))  ' remove the ToString()
                    p("PositionDescription") = ReadyString(cpeople("PositionDescription"))  ' remove the ToString()
                    p("IsFromOrg") = ReadyString(1)

                    If cpeople.ContainsKey("DateOfBirth") Then
                        p("DateOfBirth") = ReadyString(cpeople("DateOfBirth"))  ' remove the ToString()
                    End If

                    If cpeople("ContactPersonState").ToString() = "Add" Then
                        p("ID") = ReadyString(ppolID)
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                        ppolID = cpeople("PeopleID")
                        If String.IsNullOrWhiteSpace(ppolID) = False Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("PeopleID")))
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                    End If

                    If String.IsNullOrWhiteSpace(ppolID) Then
                        Continue For
                    End If

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeople")

                        p("FirstName") = ReadyString(cpeople("Firstname")) ' remove the ToString()
                        p("MiddleName") = ReadyString(cpeople("Middlename")) ' remove the ToString()
                        p("Surname") = ReadyString(cpeople("Lastname")) ' remove the ToString()
                        p("Position") = ReadyString(cpeople("Position")) ' remove the ToString()
                        p("PositionDescription") = ReadyString(cpeople("PositionDescription")) ' remove the ToString()
                        p("IsFromOrg") = ReadyString(1)

                        If cpeople("ContactPersonState").ToString() = "Add" Then
                            p("ID") = ReadyString(ppolID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                            ppolID = cpeople("PeopleID")
                            If String.IsNullOrWhiteSpace(ppolID) = False Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("PeopleID")))
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        End If
                    End If

                    p = New InsertUpdate("MobizLibraryPeopleAddress")

                    p("PeopleID") = ReadyString(ppolID)
                    p("HouseBuildingNo") = If(cpeople("HouseBuildingNo") = Nothing, "NULL", ReadyString(cpeople("HouseBuildingNo").ToString()))
                    p("Street") = If(cpeople("Street") = Nothing, "NULL", ReadyString(cpeople("Street").ToString()))
                    p("CityID") = If(cpeople("City") = "", "NULL", ReadyString(cpeople("City").ToString()))
                    p("CountryID") = If(cpeople("Country") = "", "NULL", ReadyString(cpeople("Country").ToString()))
                    p("AddressType") = If(cpeople("TypeAddress") = Nothing, "NULL", ReadyString(cpeople("TypeAddress").ToString()))
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If cpeople("AddressID") = "" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("AddressID")))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryPeopleAddress")

                        p("PeopleID") = ReadyString(ppolID)
                        p("HouseBuildingNo") = If(cpeople("HouseBuildingNo") = Nothing, "NULL", ReadyString(cpeople("HouseBuildingNo").ToString()))
                        p("Street") = If(cpeople("Street") = Nothing, "NULL", ReadyString(cpeople("Street").ToString()))
                        p("CityID") = If(cpeople("City") = "", "NULL", ReadyString(cpeople("City").ToString()))
                        p("CountryID") = If(cpeople("Country") = "", "NULL", ReadyString(cpeople("Country").ToString()))
                        p("AddressType") = If(cpeople("TypeAddress") = Nothing, "NULL", ReadyString(cpeople("TypeAddress").ToString()))
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If cpeople("AddressID") = "" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                        Else
                            pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("AddressID")))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If

                    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("PeopleID") = ReadyString(ppolID)
                    If WebSession.Instance.PeopleUserID <> Nothing Then
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    p("LastUpdated") = ReadyString(CDate(DateTime))
                    p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                    If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                        p("IsUseForEcommerce") = 1
                        p("Status") = ReadyString("Pending") 'before maconsider ang new admin (aka Contact Person) as legit admin dapat need ma approve iyang request
                        p("StatusLastUpdated") = ReadyString(CDate(DateTime))
                    End If

                    If cpeople("ContactPersonState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                        Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(cpeople("ID"))))
                    ElseIf cpeople("ContactPersonState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryOrganizationContactPerson WHERE ID = " & ReadyString(cpeople("ID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If

                    If isImportToCrm Then
                        p = New InsertUpdate("LibraryOrganizationContactPerson")

                        p("OrganizationID") = ReadyString(strOrgID)
                        p("PeopleID") = ReadyString(ppolID)
                        If WebSession.Instance.PeopleUserID <> Nothing Then
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        End If
                        p("LastUpdated") = ReadyString(CDate(DateTime))
                        p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                        If cpeople("ContactPersonState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                            Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(cpeople("ID"))))
                        ElseIf cpeople("ContactPersonState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryOrganizationContactPerson WHERE ID = " & ReadyString(cpeople("ID").ToString())
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                    End If

                    Try
                        For Each tel As Object In cpeople("Tel")
                            p = New InsertUpdate("MobizLibraryPeopleContactTel")

                            p("PeopleID") = ReadyString(ppolID)
                            p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                            p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                            p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                            p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                            p("ContactType") = ReadyString(tel("ContactType").ToString())
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("LastUpdated") = ReadyString(CDate(DateTime))

                            If tel("TelState").ToString() = "Add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()

                            ElseIf tel("TelState").ToString() = "Edit" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                            ElseIf tel("TelState").ToString() = "Remove" Then
                                pSql = "DELETE FROM MobizLibraryPeopleContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If isImportToCrm Then
                                p = New InsertUpdate("LibraryPeopleContactTel")

                                p("PeopleID") = ReadyString(ppolID)
                                p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                                p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                                p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                                p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                                p("ContactType") = ReadyString(tel("ContactType").ToString())
                                If WebSession.Instance.PeopleUserID <> Nothing Then
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                End If
                                p("LastUpdated") = ReadyString(CDate(DateTime))

                                If tel("TelState").ToString() = "Add" Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    pSql = p.GetInsert()

                                ElseIf tel("TelState").ToString() = "Edit" Then
                                    pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                                ElseIf tel("TelState").ToString() = "Remove" Then
                                    pSql = "DELETE FROM LibraryPeopleContactTel WHERE ID = " & ReadyString(tel("TelID").ToString())
                                End If

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        Next tel
                    Catch ex As Exception
                        AppendToLog(ex.Message)
                    End Try

                    Try
                        For Each mobile As Object In cpeople("Mobile")
                            p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                            p("PeopleID") = ReadyString(ppolID)
                            p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                            p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                            p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("ContactType") = ReadyString(mobile("ContactType").ToString())
                            p("LastUpdated") = ReadyString(CDate(DateTime))

                            If mobile("MobileState").ToString() = "Add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()

                            ElseIf mobile("MobileState").ToString() = "Edit" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                            ElseIf mobile("MobileState").ToString() = "Remove" Then
                                pSql = "DELETE FROM MobizLibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If isImportToCrm Then
                                p = New InsertUpdate("LibraryPeopleContactMobile")

                                p("PeopleID") = ReadyString(ppolID)
                                p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                                p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                                If WebSession.Instance.PeopleUserID <> Nothing Then
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                End If
                                p("ContactType") = ReadyString(mobile("ContactType").ToString())
                                p("LastUpdated") = ReadyString(CDate(DateTime))

                                If mobile("MobileState").ToString() = "Add" Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    pSql = p.GetInsert()

                                ElseIf mobile("MobileState").ToString() = "Edit" Then
                                    pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                                ElseIf mobile("MobileState").ToString() = "Remove" Then
                                    pSql = "DELETE FROM LibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                                End If

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        Next mobile
                    Catch ex As Exception
                        AppendToLog(ex.Message)
                    End Try

                    Try
                        For Each email As Object In cpeople("Email")
                            Dim emaillinkid As String = Guid.NewGuid.ToString()
                            p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                            p("PeopleID") = ReadyString(ppolID)
                            p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                            If WebSession.Instance.PeopleUserID <> Nothing Then
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            End If
                            p("EmailType") = ReadyString(CStr2(email("EmailType")))
                            p("LastUpdated") = ReadyString(CDate(DateTime))

                            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then ' pag nag register via generated public link
                                p("IsEmailConfirmed") = CInt2(email("IsEmailConfirmed"))
                                If CBool2(email("IsEmailConfirmed")) = False Then
                                    p("EmailActivationLinkID") = ReadyString(emaillinkid)
                                End If
                            End If

                            If email("EmailState").ToString() = "Add" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()

                            ElseIf email("EmailState").ToString() = "Edit" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                            ElseIf email("EmailState").ToString() = "Remove" Then
                                pSql = "DELETE FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If isImportToCrm Then
                                p = New InsertUpdate("LibraryPeopleContactEmail")

                                p("PeopleID") = ReadyString(ppolID)
                                p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                                If WebSession.Instance.PeopleUserID <> Nothing Then
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                End If
                                p("EmailType") = ReadyString(CStr2(email("EmailType")))
                                p("LastUpdated") = ReadyString(CDate(DateTime))

                                If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                                    p("IsEmailConfirmed") = 0
                                    p("EmailActivationLinkID") = ReadyString(emaillinkid)
                                End If

                                If email("EmailState").ToString() = "Add" Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    pSql = p.GetInsert()

                                ElseIf email("EmailState").ToString() = "Edit" Then
                                    pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                                ElseIf email("EmailState").ToString() = "Remove" Then
                                    pSql = "DELETE FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                                End If

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                                If CBool2(email("IsEmailConfirmed")) = False Then
                                    Dim link As String = Common.WebRequest.Data("Url") & "/Ecommerce/Confirm?id=" & emaillinkid &
                                    "&c=" & GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper &
                                    "&OrgID=" & Common.WebRequest.Data("OrgID") &
                                    "&acctype=" & IIf(CBool2(Common.WebRequest.Data("IsCustomerSettings")), "cus", "sup") &
                                    "&mainurl=" & Common.WebRequest.Data("Url")

                                    Dim msg As String = "<table style='width:100%; border:none; background-color: #f5f5f5;'>" +
                                                            "<tr><td style='height:100px; text-align:center; padding:20px; vertical-align:middle; background-color: #0083c1; font-size: 40px; color: #FFF;'>WELCOME</td></tr>" +
                                                            "<tr><td style='padding:20px; font-size: 20px;'>Hello " + cpeople("Firstname").ToString().ToUpper() + ",<br /><br />Thanks for providing us your information! Before you can login, please click the button below to activate your account.<br /></td></tr>" +
                                                            "<tr><td style='padding:20px; font-size: 20px; vertical-align:middle; text-align:center;'><a href='" & link & "' style='background-color: #5cb85c; border: none; border-radius: 5px; padding: 15px; color: #fff; text-decoration:none;'>ACTIVATE MY ACCOUNT</a><br /><br /></td></tr>" +
                                                            "<tr><td style='padding:20px; vertical-align:middle; background-color: #dadada; font-size: 16px;'>To know more about Mobiz, please <a href='http://mobiz-apps.com' target='_blank' style='text-decoration:none;'>click here</a>.</td></tr>" +
                                                        "</table>"

                                    SendMail("mobizecommerce@outlook.com",
                                             email("EmailAdd").ToString(),
                                             "E-commerce",
                                             "Welcome to Mobiz E-commerce!",
                                             msg,
                                             "smtp.live.com",
                                             587,
                                             "mobizecommerce@outlook.com",
                                             "m0b!zp30pl30nly!", , , , , True)
                                End If
                            End If
                        Next email

                        If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                            p = New InsertUpdate("MobizLibraryOrganizationFowst") 'SAVE ADDRESS

                            pSql = "SELECT * FROM MobizLibraryOrganizationFowst WHERE StreetNumber = " & ReadyString(Trim(CompanyBuildingAddress & " " & CompanyStreetAddress & " " & Common.WebRequest.Data("CompanyCityNameAddress") & " " & Common.WebRequest.Data("CompanyCountryNameAddress")))

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("OrganizationID") = ReadyString(strOrgID)
                                p("StreetNumber") = ReadyString(Trim(CompanyBuildingAddress & " " & CompanyStreetAddress & " " & Common.WebRequest.Data("CompanyCityNameAddress") & " " & Common.WebRequest.Data("CompanyCountryNameAddress")))
                                p("IsActive") = 1
                                p("ID") = ReadyString(Guid.NewGuid.ToString())

                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            End If

                            p = New InsertUpdate("MobizLibraryPeopleUserAccount")

                            p("ActivationDate") = "GetDate()"
                            p("Username") = ReadyString(cpeople("Username"))
                            p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(cpeople("Password")))

                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(ppolID))) = 0 Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(ppolID)
                                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                            End If

                            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                            p("SecurityQuestion") = ReadyString(cpeople("SecQ1"))
                            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(cpeople("SecQ1Ans"))) 'ReadyString(SecurityQuestion1Answer)

                            If cpeople("SecQ1ID") = "" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(ppolID)
                                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpeople("SecQ1ID"))))
                            End If


                            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                            p("SecurityQuestion") = ReadyString(cpeople("SecQ2"))
                            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(cpeople("SecQ2Ans"))) 'ReadyString(SecurityQuestion1Answer)

                            If cpeople("SecQ2ID") = "" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(ppolID)
                                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpeople("SecQ2ID"))))
                            End If

                        End If
                    Catch ex As Exception

                    End Try
                Next cpeople
            End If

            If SalesReps IsNot Nothing Then
                For Each pSalesRep In SalesReps
                    If pSalesRep.State = "Remove" Then
                        pSql = "DELETE FROM LibraryOrganizationCustomerSalesReps" & vbCrLf &
                            " WHERE PeopleID = " & ReadyString(pSalesRep.PeopleID) & vbCrLf &
                            " AND OrganizationID = " & ReadyString(CompanyID)
                        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                    ElseIf pSalesRep.State = "New" Then
                        p = New InsertUpdate("LibraryOrganizationCustomerSalesReps")
                        p("ID") = "NEWID()"
                        p("PeopleID") = ReadyString(pSalesRep.PeopleID)
                        p("OrganizationID") = ReadyString(CompanyID)
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "GETDATE()"
                        pSql = p.GetInsert()
                        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                    End If
                Next
            End If

            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                If CBool2(Common.WebRequest.Data("IsFromContactPersonLink")) = False Then
                    'kapag ang link gikan sa mismong existing contact person, no need to disable the link
                    p = New InsertUpdate("MobizLibrarySurveyQuestionaireLink")

                    p("DateEdited") = "getdate()"
                    p("IsDisableLink") = 1

                    Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" Link = " & ReadyString(Common.WebRequest.Data("Link"))))
                End If
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return strOrgID.ToString()

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetContactDetailsPeople() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()

        pOut = New JsonDictionary
        Dim objTelephone As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleTel(PersonID)))
        Dim objMobile As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleMobile(PersonID)))
        Dim objEmail As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleEmail(PersonID)))
        Dim objUrl As Object = jss.Deserialize(Of Object)(GetJson(Common.WebRequest.ProData.Execute("SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE PeopleID = " & ReadyString(PersonID))))

        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("Email", objEmail)
        pOut.Add("Url", objUrl)

        Return pOut.Serialize()
    End Function

    Function GetProspectDetailsPeople() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()

        pOut = New JsonDictionary

        Dim objPeopleDetails As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleDetails(PersonID)))
        Dim objTelephone As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleTel(PersonID)))
        Dim objMobile As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleMobile(PersonID)))
        Dim objEmail As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleEmail(PersonID)))
        Dim objAffiliation As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleAffiliation(PersonID)))
        Dim objAddress As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleAddress(PersonID)))
        Dim objEducation As Object = GetProspectPeopleEducation(PersonID).Data
        Dim objRevenue As Object = GetProspectRevenue(PersonID)
        Dim objPriceLevel As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPriceLevel(PersonID)))

        If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
            pOut.Add("LoginCredentials", jss.Deserialize(Of Object)(GetJson(Common.WebRequest.ProData.Execute("SELECT ID FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(PersonID)))))
            pOut.Add("SecQue", jss.Deserialize(Of Object)(GetJson(Common.WebRequest.ProData.Execute("SELECT ID, SecurityQuestion FROM MobizLibraryPeopleSecurityQuestion WHERE PeopleID = " & ReadyString(PersonID)))))
        End If

        pOut.Add("PeopleDetails", objPeopleDetails)
        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("Email", objEmail)
        pOut.Add("Affiliation", objAffiliation)
        pOut.Add("Address", objAddress)
        pOut.Add("Education", objEducation)
        pOut.Add("PriceLevel", objPriceLevel)
        pOut.Add("Revenue", objRevenue)

        Return pOut.Serialize()
    End Function

    Function GetProspectPeopleEducation(ByVal PeopleID As String) As Object
        Dim pSql As String
        Dim pDT As DataTable
        Dim jss = New JavaScriptSerializer()
        Dim objElem As Object
        Dim objHs As Object
        Dim objCollege As Object

        pSql = "SELECT ROW_NUMBER() OVER(ORDER BY InstitutionName ASC) as No, ID, InstitutionName as Name, AcademicLevel as Level, 'edit' as State, YearStarted as Started, YearGraduated as Graduated FROM MobizLibraryPeopleEducation " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND AcademicLevel = 'elem'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        objElem = jss.Deserialize(Of Object)(GetJson(pDT))

        '''''-----------------------

        pSql = "SELECT ROW_NUMBER() OVER(ORDER BY InstitutionName ASC) as No, ID, InstitutionName as Name, AcademicLevel as Level, 'edit' as State, YearStarted as Started, YearGraduated as Graduated FROM MobizLibraryPeopleEducation " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND AcademicLevel = 'hs'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        objHs = jss.Deserialize(Of Object)(GetJson(pDT))

        '''''-----------------------

        pSql = "SELECT ROW_NUMBER() OVER(ORDER BY InstitutionName ASC) as No, ID, InstitutionName as Name, AcademicLevel as Level, 'edit' as State, YearStarted as Started, YearGraduated as Graduated FROM MobizLibraryPeopleEducation " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND AcademicLevel = 'college'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        objCollege = jss.Deserialize(Of Object)(GetJson(pDT))

        Return Json(New With {Key .Elementary = objElem, .HighSchool = objHs, .College = objCollege})
    End Function

    Function GetProspectPeopleAddress(ByVal PeopleID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT TOP 1 address.*, ISNULL(city.Name, '') as CityName, ISNULL(country.Name, '') as CountryName FROM MobizLibraryPeopleAddress as address " & vbCrLf &
                " LEFT JOIN MobizLibraryCities as city ON City.ID = address.CityID " & vbCrLf &
                " LEFT JOIN MobizLibraryCountries as country ON country.ID = address.CountryID " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " ORDER BY address.LastUpdated DESC"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectPeopleAffiliation(ByVal PeopleID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT cp.ID as ID, org.ID as OrgID, org.CompanyName as CompanyName, IsNew = '0', State= 'Edit' FROM MobizLibraryOrganizationContactPerson as cp  " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization as org ON org.ID = cp.OrganizationID " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND IsUseForPersonOrgAffiliates = 1 " & vbCrLf &
                " ORDER BY org.CompanyName ASC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectPeopleDetails(ByVal PeopleID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim pFieldName As String

        If Common.WebRequest.Data("Type") = "Supplier" Then
            pFieldName = "SupplierID"
        Else
            pFieldName = "OrganizationID"
        End If

        pSql = "SELECT cus.ID as CustomerID, ISNULL(cus.CustomerCode, '') as CodeNumber, cusppol.ID as ID, ISNULL(cusppol.PersonSurname, ppol.Surname) as Surname, " & vbCrLf &
                "ISNULL(cusppol.PersonFirstName, ppol.FirstName) as FirstName, " & vbCrLf &
                "ISNULL(cusppol.PersonMiddleName, ppol.MIddleName) as MiddleName, " & vbCrLf &
                "ISNULL(ppol.Position, '') as Position, " & vbCrLf &
                "ISNULL(TIN, TinNumber) as TIN, " & vbCrLf &
                "ISNULL(ppol.PositionDescription, '') as PositionDescription, " & vbCrLf &
                "CASE WHEN ISNULL(CONVERT(varchar, cus.Deactivated), 0) = 0 THEN 1 ELSE 0 END as IsActive, " & vbCrLf &
                "ISNULL(cusppol.PersonDateOfBirth, ppol.DateOfBirth) as DateOfBirth " & vbCrLf &
                IIf(Common.WebRequest.Data("Type") = "Supplier", ", ''", ", cus.ThirdPartyID") & " as ThirdPartyID," & vbCrLf &
                "ISNULL(CONVERT(varchar(max), DefaultHotButtonTemplateID),'') as DefaultHotButtonTemplateID " & vbCrLf &
                IIf(Common.WebRequest.Data("Type") = "Supplier", " FROM LibraryOrganizationSupplier as cus ", " FROM LibraryOrganizationCustomer as cus ") & vbCrLf &
                "INNER JOIN MobizLibraryOrganization as cusppol ON cusppol.ID = cus." & pFieldName & vbCrLf &
                "LEFT JOIN MobizLibraryPeople as ppol ON cusppol.ID = ppol.ID " & vbCrLf &
                "WHERE cus." & pFieldName & " = " & ReadyString(PeopleID) & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectPeopleTel(ByVal PeopleID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TelID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoltel.ID"
                End With

                With .Add("TelRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY ppoltel.ID)"
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "ppoltel.ContactNo"
                End With

                With .Add("Alpha2Code", SqlDbType.VarChar)
                    .Expression = "ctry.Alpha2Code"
                End With

                With .Add("CountryCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctry.ID"
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.Code + ' - ' + ctry.CallingCode"
                End With

                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "ppoltel.ExtensionNo"
                End With

                With .Add("AreaCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "areacode.ID"
                End With

                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.Area + ' - ' + areacode.CallingCode"
                End With

                With .Add("FullTelNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(CASE WHEN areacode.CallingCode = '' THEN '' ELSE '(+' + CONVERT(varchar, areacode.CallingCode) + ') ' END, '') " & vbCrLf &
                                    " + ' ' + ppoltel.ContactNo + ISNULL(',' + ppoltel.ExtensionNo, '')"
                End With

                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppoltel.ContactType"
                End With

                With .Add("TelState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeopleContactTel as ppoltel " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry " & vbCrLf &
                        "   ON ctry.ID = ppoltel.CountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryAreaCodes as areacode " & vbCrLf &
                        "   ON areacode.ID = ppoltel.AreaCodeID " & vbCrLf

            .SqlWhere = "WHERE ppoltel.PeopleID = " & ReadyString(PeopleID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectPeopleMobile(ByVal PeopleID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) Then
                'kwaon nato ang kahit 1 mobile number sa contact person kapag gikan sa Contact Person public url
                .MaxRecords = 1
            End If

            With .Fields
                With .Add("MobileID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolmobile.ID"
                End With

                With .Add("MobileRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY ppolmobile.ID)"
                End With

                With .Add("CountryCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctry.ID"
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.Code + ' - ' + ctry.CallingCode"
                End With

                With .Add("TelCode", SqlDbType.VarChar)
                    .Expression = "telco.Name + ' - ' + prefix.Prefix"
                End With

                With .Add("TelCodeID", SqlDbType.VarChar)
                    .Expression = "prefix.ID"
                End With

                With .Add("MobileNo", SqlDbType.UniqueIdentifier)
                    .Expression = "CASE WHEN LEN(ppolmobile.ContactNo) >= 10 THEN ppolmobile.ContactNo ELSE prefix.Prefix + '' + ppolmobile.ContactNo END"
                End With

                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppolmobile.ContactType"
                End With

                With .Add("MobileState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With

                With .Add("CallingCode", SqlDbType.UniqueIdentifier)
                    .Expression = "ctry.CallingCode"
                End With

                With .Add("Alpha2Code", SqlDbType.UniqueIdentifier)
                    .Expression = "ctry.Alpha2Code"
                End With

                With .Add("Prefix", SqlDbType.UniqueIdentifier)
                    .Expression = "prefix.Prefix"
                End With

                With .Add("FullMobileNo", SqlDbType.VarChar)
                    .Expression = "'+' + convert(varchar, ctry.CallingCode) + " & vbCrLf &
                                    " CASE WHEN LEN(ppolmobile.ContactNo) >= 10 THEN ppolmobile.ContactNo ELSE prefix.Prefix + '' + ppolmobile.ContactNo END " & vbCrLf
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry " & vbCrLf &
                        "   ON ctry.ID = ppolmobile.CountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcoPrefixes as prefix " & vbCrLf &
                        "   ON prefix.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcos as telco " & vbCrLf &
                        "   ON telco.ID = prefix.TelCoID " & vbCrLf

            .SqlWhere = "WHERE ppolmobile.PeopleID = " & ReadyString(PeopleID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectPeopleEmail(ByVal PeopleID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) = True Then
                .MaxRecords = 1
            End If

            With .Fields
                With .Add("EmailRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY ppolemail.ID)"
                End With

                With .Add("EmailAddID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemail.ID"
                End With

                With .Add("EmailAdd", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemail.EmailAddress"
                End With

                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "ppolemail.EmailType"
                End With

                With .Add("EmailState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With

                With .Add("IsEmailConfirmed", SqlDbType.VarChar)
                    .Expression = "ISNULL(IsEmailConfirmed, 0)"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeopleContactEmail as ppolemail " & vbCrLf

            .SqlWhere = "WHERE ppolemail.PeopleID = " & ReadyString(PeopleID) & vbCrLf

            .SqlOrder = " ORDER BY ppolemail.IsEmailConfirmed DESC "

            pDT = .Requery
        End With

        Return pDT
    End Function

    ''''''''

    Function SaveSelectedPublicOrg(ByVal orgdetails As String) As String
        Dim objDetails As Object = jss.Deserialize(Of Object)(orgdetails)("d")
        Dim org As Object = objDetails("OrgDetails")(0)


    End Function

    Function GetProspectDetails() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()
        Dim pDT As DataTable
        pOut = New JsonDictionary
        Dim objContactPerson As Object

        Dim objOrgDetails As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgDetails(OrgID)))
        Dim objTelephone As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgTel(OrgID)))
        Dim objMobile As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgMobile(OrgID)))
        Dim objEmail As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgEmail(OrgID)))
        Dim objWebsite As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgWebsite(OrgID)))
        Dim objAffiliates As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgAffiliation(OrgID)))
        Dim objAffiliatedPerson As Object = jss.Deserialize(Of Object)(GetJson(GetProspectAffiliatedPerson(OrgID)))

        If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) = False Or
            CStr2(Common.WebRequest.Data("PeopleID")) <> "" Then
            ' kapag Customer/Supplier Public link, no need na kwaon ang mga contact person
            ' kapag ang PeopleID is not empty, it means Contact Person Public link sya, and kwaon ang Personal Info sa specific contact person
            objContactPerson = jss.Deserialize(Of Object)(GetJson(GetProspectOrgContactPerson(OrgID)))
        Else
            pOut.Add("Outlet", GetProspectOutlet(OrgID).Data.Results)
        End If

        If CStr2(Common.WebRequest.Data("PeopleID")) <> "" Then
            ' kapag Contact Person Public link, kwaon nato ang email, mobile details
            pOut.Add("ContactPersonMobile", jss.Deserialize(Of Object)(GetJson(GetProspectPeopleMobile(Common.WebRequest.Data("PeopleID")))))
            pOut.Add("ContactPersonEmail", jss.Deserialize(Of Object)(GetJson(GetProspectPeopleEmail(Common.WebRequest.Data("PeopleID")))))
            pOut.Add("SecQue", jss.Deserialize(Of Object)(GetJson(Common.WebRequest.ProData.Execute("SELECT ID, SecurityQuestion FROM MobizLibraryPeopleSecurityQuestion WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID"))))))
        End If

        pOut.Add("OrgDetails", objOrgDetails)
        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("Email", objEmail)
        pOut.Add("Website", objWebsite)
        pOut.Add("ContactPerson", objContactPerson)
        pOut.Add("Affiliation", objAffiliates)
        pOut.Add("AffiliatedPerson", objAffiliatedPerson)
        pOut.Add("SalesReps", GetProspectOrgSalesReps(OrgID))

        If Not CBool2(Common.WebRequest.Data("IsReadOnly")) Then
            Dim objRevenue As Object = GetProspectRevenue(OrgID)
            Dim objPriceLevel As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPriceLevel(OrgID)))
            'Dim objOutlet As Object = GetProspectOutlet(OrgID)

            pOut.Add("Revenue", objRevenue)
            pDT = New Organization().getOrganizationSupplierInfo(Nothing, OrgID)
            If pDT.Rows.Count > 0 Then
                pOut.Add("Disbursement", pDT.Rows(0))
            End If
            pOut.Add("PriceLevel", objPriceLevel)
            'pOut.Add("Outlet", objOutlet.Data.Results)
        End If

        Return pOut.Serialize()
    End Function

    Function GetProspectOutlet(ByVal OrgID As String) As Object
        WebRequest.Instance.Business.LibraryOrganizationFOWST.AddOrUpdateFromMainAddress(OrgID, False)

        'No IsActive column in table MobizLibraryOrganizationFOWST before June 12, 2017 added by arnie. If IsActive is null, changed the value to 1.
        Dim pSql = "SELECT outlet.NetSuiteID, outlet.ID, ISNULL(outlet.IsActive, 1) as IsActive, outlet.Name, outlet.StreetNumber as HouseNo, outlet.Street, outlet.FowstCityID as City, outlet.FowstCountryID as Country, 'edit' as State " & vbCrLf &
                    ",ISNULL(NULLIF(LTRIM(RTRIM(outlet.Name)), '') + ', ', '')" & vbCrLf &
                    " + ISNULL(outlet.StreetNumber, '')" & vbCrLf &
                    " + ',' + ISNULL(outlet.Street, '')" & vbCrLf &
                    " + ',' + ISNULL(city.Name, '')" & vbCrLf &
                    " + ',' + ISNULL(ctr.Name, '') AS FullAddress" & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST as outlet " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities as city ON city.ID = outlet.FowstCityID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCountries as ctr ON ctr.ID = outlet.FowstCountryID " & vbCrLf &
                    " WHERE outlet.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                    "AND ISNULL(outlet.IsActive, 1) = 1 " & vbCrLf
        Dim keywords As String = CStr2(Common.WebRequest.Data("keywords"))

        If Not String.IsNullOrWhiteSpace(keywords) Then
            pSql += " AND outlet.Name LIKE " & ReadyStringWild(keywords)
        End If

        pSql += " ORDER BY outlet.Name ASC "

        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
        Dim arrOutlet As New List(Of Dictionary(Of String, Object))

        For i As Integer = 0 To pDT.Rows.Count - 1
            Dim outlet As Object = pDT.Rows(i)
            Dim pOutlet As New Dictionary(Of String, Object)

            pSql = "Select ID, Name as Department, NetSuiteID as ThirdPartyID, 'edit' as State, ISNULL(IsActive, 1) as IsActive, RelatedFowstBinID from LibraryFowstBin where FowstID = " & ReadyString(outlet("ID")) & vbCrLf &
                    " ORDER BY Name ASC "
            Dim pDtDepartment As DataTable = Common.WebRequest.ProData.Execute(pSql)

            pOutlet.Add("ID", outlet("ID"))
            pOutlet.Add("Name", outlet("Name"))
            pOutlet.Add("ThirdPartyID", outlet("NetSuiteID"))
            pOutlet.Add("HouseNo", outlet("HouseNo"))
            pOutlet.Add("Street", outlet("Street"))
            pOutlet.Add("City", outlet("City"))
            pOutlet.Add("Country", outlet("Country"))
            pOutlet.Add("State", outlet("State"))
            pOutlet.Add("IsActive", outlet("IsActive"))
            pOutlet.Add("FullAddress", outlet("FullAddress"))
            pOutlet.Add("Department", jss.DeserializeObject(DataTableToJSON.GetJson(pDtDepartment)))
            pOutlet.Add("Telephone", jss.DeserializeObject(DataTableToJSON.GetJson(GetProspectOrgTel(outlet("ID").ToString(), True))))
            pOutlet.Add("Mobile", jss.DeserializeObject(DataTableToJSON.GetJson(GetProspectOrgMobile(outlet("ID").ToString(), True))))
            pOutlet.Add("Email", jss.DeserializeObject(DataTableToJSON.GetJson(GetProspectOrgEmail(outlet("ID").ToString(), True))))
            arrOutlet.Add(pOutlet)
        Next

        Return Json(New With {Key .Results = arrOutlet})
    End Function

    Function GetProspectPriceLevel(ByVal OrgID As String) As DataTable
        Dim pSql As String = "SELECT orglvl.ID as DBID, " & vbCrLf &
                            " lvl.ID, " & vbCrLf &
                            " lvl.PriceLevel, " & vbCrLf &
                            " 'edit' as State " & vbCrLf &
                            " FROM LibraryOrganizationPriceLevel as orglvl " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMPriceLevel as lvl ON lvl.ID = orglvl.PriceLevelID " & vbCrLf &
                            " WHERE OrganizationID = " & ReadyString(OrgID)
        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectRevenue(ByVal OrgID As String)
        Dim pSql As String
        Dim pDT As Object

        pSql = "SELECT --ISNULL(convert(varchar(MAX), lvl.ID), '') as PriceLevelID, ISNULL(lvl.PriceLevel, '') as PriceLevelName, " & vbCrLf &
                "ISNULL(convert(varchar(MAX), ppol.ID), '') as SalesRepID, " & vbCrLf &
                "ISNULL(ppol.Surname + ', ', '') + ISNULL(ppol.FirstName + ' ', '') + ISNULL(ppol.MiddleName + ' ', '') as SalesRepName, " & vbCrLf &
                "ISNULL(convert(varchar(MAX), term.ID), '') as TermID, " & vbCrLf &
                "ISNULL(term.Name, '') as TermName, " & vbCrLf &
                "ISNULL(cus.CreditLimit,'') as CreditLimit, " & vbCrLf &
                "ISNULL(cus.CreditBegBalance,'') as CreditBegBalance, " & vbCrLf &
                "ISNULL(CONVERT(varchar(MAX), cus.CreditBalanceAsOf), '') as CreditAsOf " & vbCrLf &
                " ,cus.ThirdPartyBegBalance" & vbCrLf &
                ",ISNULL(CONVERT(varchar(MAX), cus.ThirdPartyBegBalanceAsOf, 126), '') as ThirdPartyBegBalanceAsOf " & vbCrLf &
                ",ISNULL(cus.UnlimitedCredit, 0) AS UnlimitedCredit" & vbCrLf &
                ",ISNULL(cus.RequireCnCApproval, 0) AS RequireCnCApproval" & vbCrLf &
                ",ISNULL(cus.CustomerRequirePoNum, 0) AS CustomerRequirePoNum" & vbCrLf &
                ",cus.ID, cus.FowstID, fowst.Name as FowstName " & vbCrLf &
                ",ISNULL(enc.Surname + ', ', '') + ISNULL(enc.FirstName + ' ', '') + ISNULL(enc.MiddleName + ' ', '') as EncodedByName " & vbCrLf &
                ",ISNULL(app.Surname + ', ', '') + ISNULL(app.FirstName + ' ', '') + ISNULL(app.MiddleName + ' ', '') as ApprovedByName " & vbCrLf &
                ",MobizLibrarySalesChannels.Name AS SalesChannelName" & vbCrLf

        pSql &= " FROM (" & vbCrLf &
            " 	SELECT LibraryOrganizationCustomerCreditTerms.ID" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.CreditLimit" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.CreditBalanceAsOf" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.SalesRepID" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.EncodedBy" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.ApprovedBy" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.CreditTermsID" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.FowstID" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.OrganizationID" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.CreditBegBalance" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.ThirdPartyBegBalance" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.ThirdPartyBegBalanceAsOf" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.UnlimitedCredit" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.RequireCnCApproval" & vbCrLf &
            "     ,LibraryOrganizationCustomerCreditTerms.CustomerRequirePoNum" & vbCrLf &
            "     ,NULL AS SalesChannelID" & vbCrLf &
            "     FROM LibraryOrganizationCustomerCreditTerms" & vbCrLf &
            " 	UNION" & vbCrLf &
            "     SELECT LibraryOrganizationCustomer.ID" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CreditLimit" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CreditBalanceAsOf" & vbCrLf &
            "     ,LibraryOrganizationCustomer.SalesRepID" & vbCrLf &
            "     ,NULL AS EncodedBy" & vbCrLf &
            "     ,NULL AS ApprovedBy" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CreditTermsID" & vbCrLf &
            "     ,NULL AS FowstID" & vbCrLf &
            "     ,LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CreditBegBalance" & vbCrLf &
            "     ,LibraryOrganizationCustomer.ThirdPartyBegBalance" & vbCrLf &
            "     ,LibraryOrganizationCustomer.ThirdPartyBegBalanceAsOf" & vbCrLf &
            "     ,LibraryOrganizationCustomer.UnlimitedCredit" & vbCrLf &
            "     ,LibraryOrganizationCustomer.RequireCnCApproval" & vbCrLf &
            "     ,LibraryOrganizationCustomer.CustomerRequirePoNum" & vbCrLf &
            "     ,LibraryOrganizationCustomer.SalesChannelID" & vbCrLf &
            "     FROM LibraryOrganizationCustomer" & vbCrLf &
            " ) as cus"

        pSql &= " --LEFT JOIN MobizLibraryPIMPriceLevel as lvl ON lvl.ID = cus.PriceLevelID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = cus.SalesRepID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as enc ON enc.ID = cus.EncodedBy " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as app ON app.ID = cus.ApprovedBy " & vbCrLf &
                " LEFT JOIN LibraryPaymentTerms as term ON term.ID = cus.CreditTermsID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = cus.FowstID " & vbCrLf &
                " LEFT JOIN MobizLibrarySalesChannels ON MobizLibrarySalesChannels.ID = cus.SalesChannelID " & vbCrLf &
                " WHERE cus.OrganizationID = " & ReadyString(OrgID) & vbCrLf
        pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        For Each pl As Object In pDT
            pSql = "SELECT orglvl.ID as DBID, " & vbCrLf &
                    " lvl.ID, " & vbCrLf &
                    " lvl.PriceLevel, " & vbCrLf &
                    " 'edit' as State " & vbCrLf &
                    " FROM LibraryOrganizationPriceLevel as orglvl " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMPriceLevel as lvl ON lvl.ID = orglvl.PriceLevelID " & vbCrLf &
                    " WHERE OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                    " AND orglvl.CreditTermID = " & ReadyString(pl("ID").ToString())

            pl.Add("PriceLevels", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
        Next

        Return pDT
    End Function

    Function GetProspectOrgAffiliation(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT orgaff.ID as ID, org.ID as OrgID, org.CompanyName as CompanyName, IsNew = '0', State= 'Edit' FROM MobizLibraryOrganizationAffiliates as orgaff  " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization as org ON org.ID = orgaff.AffiliatesID " & vbCrLf &
                " WHERE orgaff.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                " ORDER BY org.CompanyName ASC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectAffiliatedPerson(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT cppol.ID as ID, ppol.ID as PeopleID, ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') as PeopleName, IsNew = '0', State= 'Edit' " & vbCrLf &
                " FROM MobizLibraryPeople as ppol " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationContactPerson as cppol ON cppol.PeopleID = ppol.ID " & vbCrLf &
                " WHERE cppol.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                " AND IsUseForPersonOrgAffiliates = 1 " & vbCrLf &
                " ORDER BY Surname ASC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Public Function GetProspectOrgContactPerson(ByVal OrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "contactperson.ID"
                End With

                With .Add("ContactPersonRankNo", SqlDbType.VarChar)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY contactperson.ID)"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With

                With .Add("Firstname", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName"
                End With

                With .Add("Middlename", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.MiddleName, '')"
                End With

                With .Add("Lastname", SqlDbType.VarChar)
                    .Expression = "ppol.Surname"
                End With

                With .Add("Position", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.Position, '')"
                End With

                With .Add("DateOfBirth", SqlDbType.VarChar)
                    .Expression = "ISNULL(CONVERT(varchar(max), DateOfBirth), '')"
                End With

                With .Add("PositionDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.PositionDescription, '')"
                End With

                With .Add("FullName", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.FirstName + ' ' ,'') + " & vbCrLf &
                                    "ISNULL(CASE WHEN ppol.MiddleName  = '' THEN '' ELSE ppol.MiddleName + ' ' END, '') + " & vbCrLf &
                                    "ISNULL(ppol.Surname ,'') "
                End With

                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "ppol.Tin"
                End With

                With .Add("ContactPersonState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With

                With .Add("AddressID", SqlDbType.VarChar)
                    .Expression = "ppoladd.ID"
                End With

                With .Add("HouseBuildingNo", SqlDbType.VarChar)
                    .Expression = "ppoladd.HouseBuildingNo"
                End With

                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "ppoladd.Street"
                End With

                With .Add("City", SqlDbType.VarChar)
                    .Expression = "ppoladd.CityID"
                End With

                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ppoladd.CountryID"
                End With

                With .Add("TypeAddress", SqlDbType.VarChar)
                    .Expression = "ppoladd.AddressType"
                End With

                With .Add("MobileNos", SqlDbType.VarChar)
                    .Expression = " (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                                "       ISNULL(CONVERT(varchar, ppoltelco.Prefix),0) + " & vbCrLf &
                                "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                                "       FROM MobizLibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                                "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                                "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                                "           WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                                "       FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,'') FROM MobizLibraryPeopleContactMobile) " & vbCrLf
                End With

                With .Add("TelNos", SqlDbType.VarChar)
                    .Expression = " (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                                "       ISNULL('-' + CONVERT(varchar, ppolareacode.CallingCode) + '-',0) + " & vbCrLf &
                                "       ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) " & vbCrLf &
                                "       FROM MobizLibraryPeopleContactTel  as ppoltel " & vbCrLf &
                                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                "               ON ppoltel.CountryID = ppolcountries.ID  " & vbCrLf &
                                "           LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                                "               ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                                "           WHERE ppoltel.PeopleID = ppol.ID " & vbCrLf &
                                "       FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,'') FROM MobizLibraryPeopleContactMobile) " & vbCrLf
                End With

                If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) = True Then
                    With .Add("Username", SqlDbType.VarChar)
                        .Expression = "acc.Username"
                    End With
                End If
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationContactPerson as contactperson " & vbCrLf &
                        "LEFT JOIN MobizLibraryPeople as ppol " & vbCrLf &
                        "   ON ppol.ID = contactperson.PeopleID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPeopleAddress as ppoladd " & vbCrLf &
                        "   ON ppoladd.PeopleID = contactperson.PeopleID "

            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) = True Then
                .SqlFrom += " LEFT JOIN MobizLibraryPeopleUserAccount as acc ON acc.PeopleID = contactperson.PeopleID "
            End If

            .SqlWhere = "WHERE contactperson.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                        " AND ISNULL(contactperson.IsUseForPersonOrgAffiliates, 0) = 0 " & vbCrLf


            If CStr2(Common.WebRequest.Data("PeopleID")) <> "" Then
                .SqlWhere += " AND contactperson.PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID"))
            Else
                .SqlWhere += " AND ISNULL(contactperson.Status, 'Approved') = 'Approved' " ' include existing contact person prior sa Contact Person Approval feature
            End If

            If CBool2(Common.WebRequest.Data("IsFromGeneratedPublicURL")) = True Then
                .SqlWhere += " AND contactperson.IsUseForEcommerce = 1 "
            End If

            pDT = .Requery
        End With

        Return pDT
    End Function

    Public Function GetProspectOrgSalesReps(ByVal OrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationCustomerSalesReps.ID"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationCustomerSalesReps.PeopleID"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName()
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationCustomerSalesReps" & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationCustomerSalesReps.PeopleID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlWhere = "WHERE LibraryOrganizationCustomerSalesReps.OrganizationID = " & ReadyString(OrgID) & vbCrLf
            .SqlOrder = "ORDER BY " & BrowserController.ComposePeopleName()
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectOrgWebsite(ByVal OrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("WebsiteID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgwebsite.ID"
                End With

                With .Add("WebsiteRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY orgwebsite.ID)"
                End With

                With .Add("Website", SqlDbType.UniqueIdentifier)
                    .Expression = "orgwebsite.Website"
                End With

                With .Add("WebsiteState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationContactWebsite as orgwebsite " & vbCrLf

            .SqlWhere = "WHERE orgwebsite.OrganizationID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Public Function GetProspectOrgEmail(ByVal OrgID As String, Optional ByVal isOutlet As Boolean = False) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("EmailRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY orgemail.ID)"
                End With

                With .Add("EmailAddID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgemail.ID"
                End With

                With .Add("EmailAdd", SqlDbType.UniqueIdentifier)
                    .Expression = "orgemail.EmailAddress"
                End With

                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "orgemail.EmailType"
                End With

                With .Add("EmailState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With
            End With

            .SqlFrom = "FROM " & IIf(isOutlet, "MobizLibraryOrganizationFOWSTEmail", "MobizLibraryOrganizationContactEmail") & " as orgemail " & vbCrLf

            .SqlWhere = "WHERE orgemail." & IIf(isOutlet, "FowstID", "OrganizationID") & " = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Public Function GetProspectOrgMobile(ByVal OrgID As String, Optional ByVal isOutlet As Boolean = False) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("MobileID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgmobile.ID"
                End With

                With .Add("MobileRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY orgmobile.ID)"
                End With

                With .Add("CountryCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(convert(varchar(max), ctry.ID), 'e783ed13-aa98-4cc1-918e-d49b05c0167e')" 'default to PH
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ISNULL(ctry.Code, 'PHL') + ' - ' + ISNULL(ctry.CallingCode, '63')"
                End With

                With .Add("TelCode", SqlDbType.VarChar)
                    .Expression = "telco.Name + ' - ' + prefix.Prefix"
                End With

                With .Add("TelCodeID", SqlDbType.VarChar)
                    .Expression = "prefix.ID"
                End With

                With .Add("MobileNo", SqlDbType.UniqueIdentifier)
                    .Expression = "CASE WHEN LEN(orgmobile.ContactNo) >= 10 THEN orgmobile.ContactNo ELSE prefix.Prefix + '' + orgmobile.ContactNo END"
                End With

                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgmobile.ContactType"
                End With

                With .Add("MobileState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With

                With .Add("CallingCode", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(ctry.CallingCode, '63')"
                End With

                With .Add("Alpha2Code", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(ctry.Alpha2Code, 'PH')"
                End With

                With .Add("Prefix", SqlDbType.UniqueIdentifier)
                    .Expression = "prefix.Prefix"
                End With

                'With .Add("MobileNo", SqlDbType.VarChar)
                '    .Expression = "'+' + convert(varchar, ctry.CallingCode) + " & vbCrLf &
                '                    " ISNULL(CASE WHEN prefix.Prefix = '' THEN '' ELSE '.' + CONVERT(varchar, prefix.Prefix) END, '') + " & vbCrLf &
                '                    " + '.' + orgmobile.ContactNo " & vbCrLf
                'End With
            End With

            .SqlFrom = "FROM " & IIf(isOutlet, "MobizLibraryOrganizationFOWSTMobile", "MobizLibraryOrganizationContactMobile") & " as orgmobile " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry " & vbCrLf &
                        "   ON ctry.ID = orgmobile.CountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcoPrefixes as prefix " & vbCrLf &
                        "   ON prefix.ID = orgmobile.TelCoPrefixID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcos as telco " & vbCrLf &
                        "   ON telco.ID = prefix.TelCoID " & vbCrLf

            .SqlWhere = "WHERE orgmobile." & IIf(isOutlet, "FowstID", "OrganizationID") & " = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Public Function GetProspectOrgTel(ByVal OrgID As String, Optional ByVal isOutlet As Boolean = False) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TelID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgtel.ID"
                End With

                With .Add("TelRankNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY orgtel.ID)"
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "orgtel.ContactNo"
                End With

                With .Add("Alpha2Code", SqlDbType.VarChar)
                    .Expression = "ctry.Alpha2Code"
                End With

                With .Add("CountryCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctry.ID"
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.Code + ' - ' + ctry.CallingCode"
                End With

                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgtel.ExtensionNo"
                End With

                With .Add("AreaCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "areacode.ID"
                End With

                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.Area + ' - ' + areacode.CallingCode"
                End With

                'With .Add("Number", SqlDbType.VarChar)
                '    .Expression = "ISNULL(CASE WHEN areacode.CallingCode = '' THEN '' ELSE '(+' + CONVERT(varchar, areacode.CallingCode) + ') ' END, '') " & vbCrLf &
                '                    " + ' ' + orgtel.ContactNo "
                'End With

                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgtel.ContactType"
                End With

                With .Add("TelState", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With
            End With

            .SqlFrom = "FROM " & IIf(isOutlet, "MobizLibraryOrganizationFOWSTTel", "MobizLibraryOrganizationContactTel") & " as orgtel " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry " & vbCrLf &
                        "   ON ctry.ID = orgtel.CountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryAreaCodes as areacode " & vbCrLf &
                        "   ON areacode.ID = orgtel.AreaCodeID " & vbCrLf

            .SqlWhere = "WHERE orgtel." & IIf(isOutlet, "FowstID", "OrganizationID") & " = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectOrgDetails(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim pOrgSelect As String
        Dim pOrgFrom As String

        'MobizLibraryIndustryClass.ID as IndustryID, MobizLibraryIndustryClass.Class as IndustryName,
        'LEFT JOIN MobizLibraryIndustryClass ON LibraryOrganization.IndustryClassID = MobizLibraryIndustryClass.ID
        BrowserController.ComposeCustomer(pOrgSelect, pOrgFrom, "org")

        pSql = "SELECT org.ID, cus.ID as CustomerID, " & pOrgSelect & " AS CompanyName" & vbCrLf &
            " , BusinessTradeName, OfficeAddressBuildingNo, TinNumber, OfficeAddressCityID, OfficeAddressCountryID, OfficeAddressStreet" & vbCrLf &
            " , ISNULL(OfficeAddressBuildingNo + ', ', '') + ' ' + ISNULL(OfficeAddressStreet + ', ', '') + ' ' + ISNULL(city.Name, '') + ' ' + ISNULL(country.Name, '') as Address " & vbCrLf &
            " , Background, Mission, Vision, FoundingDate, Region, Island, CASE WHEN ISNULL(CONVERT(varchar, cus.Deactivated), 0) = 0 THEN 1 ELSE 0 END as IsActive, ind.Class as IndustryName, ind.ID as IndustryID " & vbCrLf &
            " , ISNULL(CONVERT(varchar, NoOfEmployees), '') as NoOfEmployees, ISNULL(CONVERT(varchar, RevenuePerYear), '') as RevenuePerYear, ISNULL(CONVERT(varchar, ScopeOfOperations), '') as ScopeOfOperations " & vbCrLf &
            " , ISNULL(CONVERT(varchar, NoOfFactory), '') as NoOfFactory, ISNULL(CONVERT(varchar, NoOfOffice), '') as NoOfOffice, ISNULL(CONVERT(varchar, NoOfWarehouse), '') as NoOfWarehouse, ISNULL(CONVERT(varchar, NoOfStore), '') as NoOfStore, ISNULL(CONVERT(varchar, NoOfTruck), '') as NoOfTruck " & vbCrLf &
            " ,org.BusinessIndustryText " & vbCrLf &
            " ,org.TinNumber as TIN " & vbCrLf &
            " ,org.VatType" & vbCrLf &
            ", cus.ThirdPartyPlatform as ThirdPartyPlatform" & vbCrLf &
            ", cus.ThirdPartyID as ThirdPartyID " & vbCrLf &
            If(Common.WebRequest.Data("Type") = "Supplier", ",1", ",cus.SingleCreditTerm") & " as SingleCreditTerm " & vbCrLf &
            ", city.zip " & vbCrLf &
            ", ISNULL(CONVERT(varchar(max), cus.DefaultHotButtonTemplateID),'') as DefaultHotButtonTemplateID, " & vbCrLf &
            IIf(Common.WebRequest.Data("Type") = "Supplier", " ISNULL(cus.VendorCode, '') ", " ISNULL(cus.CustomerCode, '') ") & " as CodeNumber " & vbCrLf &
            ",MobizLibraryUserDefinedIndustry.Name AS UserDefinedIndustryName " & vbCrLf &
            " FROM MobizLibraryOrganization as org LEFT JOIN MobizLibraryIndustryClass as ind ON org.IndustryClassID = ind.ID " & vbCrLf &
            pOrgFrom & vbCrLf &
            " LEFT JOIN MobizLibraryCities as city ON City.ID = OfficeAddressCityID " & vbCrLf &
            " LEFT JOIN MobizLibraryCountries as country ON country.ID = OfficeAddressCountryID " & vbCrLf &
            IIf(Common.WebRequest.Data("Type") = "Supplier", " LEFT JOIN LibraryOrganizationSupplier as cus ON cus.SupplierID = org.ID ", " LEFT JOIN LibraryOrganizationCustomer as cus ON cus.OrganizationID = org.ID ") & vbCrLf &
            " LEFT JOIN MobizLibraryUserDefinedIndustry ON MobizLibraryUserDefinedIndustry.ID = org.UserDefinedIndustryID" & vbCrLf &
            " WHERE org.ID = " & ReadyString(OrgID)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectList() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " AND RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim whereSearchContent As String = ""
        Dim additionalwhereSearchContent = ""

        If Filter <> "" Then
            Dim arrFilter As Object = Split(Filter, "||")
            Dim txtFilter As String = ""
            Dim isFinishAppendFirstOperator As Boolean = False

            If PageSearchText <> "" Then
                additionalwhereSearchContent = " AND (" & additionalwhereSearchContent
                isFinishAppendFirstOperator = True
            End If

            For Each txtFilter In arrFilter
                Dim arrtxtFilter As Object = Split(txtFilter, "/")

                If isFinishAppendFirstOperator = False Then
                    additionalwhereSearchContent = If(additionalwhereSearchContent <> "", additionalwhereSearchContent & " OR ", "")
                End If

                If arrtxtFilter.length = 2 Then
                    If arrtxtFilter(0) = "City" Then
                        additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(CityID, '') = " & ReadyString(arrtxtFilter(1))
                    ElseIf arrtxtFilter(0) = "Business Industry" Then
                        additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(IndustryID, '') = " & ReadyString(arrtxtFilter(1))
                    End If

                ElseIf arrtxtFilter.length = 3 Then
                    Dim arroperators As Object = Split(arrtxtFilter(2), "-")
                    Dim operators As String

                    If arroperators(0).ToString() = "gt" Then
                        operators = ">"
                    ElseIf arroperators(0).ToString() = "ge" Then
                        operators = ">="
                    ElseIf arroperators(0).ToString() = "e" Then
                        operators = "="
                    ElseIf arroperators(0).ToString() = "lt" Then
                        operators = "<"
                    ElseIf arroperators(0).ToString() = "le" Then
                        operators = "<="
                    End If

                    If arrtxtFilter(1) = "ScopeOfOperations" Then
                        additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(ScopeOfOperations, '') = " & ReadyString(arrtxtFilter(2))
                    Else
                        additionalwhereSearchContent = additionalwhereSearchContent & " " & arrtxtFilter(1) & " " & operators & " " & arroperators(1)
                    End If
                End If

                isFinishAppendFirstOperator = False
            Next

            If PageSearchText <> "" Then
                additionalwhereSearchContent = additionalwhereSearchContent & ") "
            End If
        Else
            additionalwhereSearchContent = ""
        End If

        If PageSearchText <> "" Then
            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent = "" Then
                    whereSearchContent = "ISNULL(ClientName, '') + ' ' + ISNULL(ContactPerson, '') + ' ' + ISNULL(City, '') + ' ' + ISNULL(Country, '') + ' ' + ISNULL(Region, '') + ' ' + ISNULL(Street, '') + ' ' + ISNULL(HouseNo, '') + ' ' + ISNULL(Island, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf & " " & additionalwhereSearchContent
                Else
                    whereSearchContent += "AND ISNULL(ClientName, '') + ' ' + ISNULL(ContactPerson, '') + ' ' + ISNULL(City, '') + ' ' + ISNULL(Country, '') + ' ' + ISNULL(Region, '') + ' ' + ISNULL(Street, '') + ' ' + ISNULL(HouseNo, '') + ' ' + ISNULL(Island, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf & " " & additionalwhereSearchContent
                End If
            Next
            whereSearch = "WHERE (" & whereSearchContent & ")"
        Else
            whereSearch = If(additionalwhereSearchContent <> "", "WHERE (" & additionalwhereSearchContent & ")", "")
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Then
            whereOrder = " ORDER BY ProspectType " & PageColumnDirection & ", ClientName ASC "
        ElseIf PageColumnNo = 1 Then
            whereOrder = " ORDER BY ClientName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY IndustryName " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            'used for sales grinder datatable
            whereOrder = " ORDER BY GrossSales " & PageColumnDirection & ", ClientName ASC "
        ElseIf PageColumnNo = 6 Then
            whereOrder = " ORDER BY LastUpdated " & PageColumnDirection
        ElseIf PageColumnNo = 7 Then
            whereOrder = " ORDER BY CONVERT(datetime, LastUpdated) " & PageColumnDirection
        ElseIf PageColumnNo = 8 Then
            whereOrder = " ORDER BY CONVERT(datetime, LastInteractionDate) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum " & vbCrLf &
            " ,(SELECT STUFF(" & vbCrLf &
            "  	(SELECT '; ' + " & BrowserController.ComposePeopleName() & vbCrLf &
            "      FROM LibraryOrganizationCustomerSalesReps" & vbCrLf &
            "  	JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationCustomerSalesReps.PeopleID" & vbCrLf &
            "  	AND LibraryOrganizationCustomerSalesReps.OrganizationID = v1.ID" & vbCrLf &
            "  	LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            "      --WHERE MobizLibraryOrganizationOwner.OrganizationID = fowst.OrganizationID" & vbCrLf &
            "      ORDER BY " & BrowserController.ComposePeopleName() & vbCrLf &
            "  	FOR XML PATH('')" & vbCrLf &
            "      ), 1, 2, '')" & vbCrLf &
            "  ) AS SalesReps" & vbCrLf &
            " From (SELECT * FROM (SELECT /*DISTINCT*/ org.ID as ID, cus.ID as CustomerID, " & vbCrLf &
                "   CASE WHEN ISNULL(CONVERT(varchar, cus.Deactivated), 0) = 0 THEN 1 ELSE 0 END as Status,  " & vbCrLf &
                "   LTRIM(CASE WHEN org.IsOrganization = 1 THEN org.CompanyName ELSE CASE WHEN ISNULL(org.PersonFullname, '') = '' THEN org.PersonFirstname + ' ' + org.PersonMiddlename + ' ' + org.PersonSurname ELSE org.PersonFullname END END) as ClientName, " & vbCrLf &
                "   (SELECT TOP 1 ISNULL(CASE WHEN orgemail.EmailAddress = '' THEN '' ELSE orgemail.EmailAddress END,'') " & vbCrLf &
                "       FROM MobizLibraryOrganizationContactEmail as orgemail" & vbCrLf &
                "       WHERE(orgemail.OrganizationID = org.ID)) as Email, " & vbCrLf &
                "   (SELECT TOP 1 " & vbCrLf &
                "       ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode) + ' ',0) + " & vbCrLf &
                "       ISNULL(' (' + CONVERT(varchar, orgareacode.CallingCode) + ') ',0) + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, orgtel.ContactNo),0) + " & vbCrLf &
                "       ISNULL(CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE ',' + orgtel.ExtensionNo END,0) " & vbCrLf &
                "       FROM MobizLibraryOrganizationContactTel as orgtel " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                "               ON orgtel.CountryID = orgcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryAreaCodes as orgareacode " & vbCrLf &
                "               ON orgtel.AreaCodeID = orgareacode.ID " & vbCrLf &
                "           WHERE orgtel.OrganizationID = org.ID ) as Telephone, " & vbCrLf &
                "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                "       --ISNULL(' (' + CONVERT(varchar, orgtelco.Prefix) + ') ',0) + " & vbCrLf &
                "       CASE WHEN LEN(orgmobile.ContactNo) <= 7 THEN CONVERT(varchar, orgtelco.Prefix) ELSE '' END + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
                "       FROM MobizLibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                "           WHERE orgmobile.OrganizationID = org.ID) as Mobile, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                "       --ISNULL(CONVERT(varchar, orgtelco.Prefix),0) + " & vbCrLf &
                "       CASE WHEN LEN(orgmobile.ContactNo) <= 7 THEN CONVERT(varchar, orgtelco.Prefix) ELSE '' END + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
                "       FROM MobizLibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                "           WHERE orgmobile.OrganizationID = org.ID " & vbCrLf &
                "       FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM MobizLibraryOrganizationContactMobile) as MobileNos," & vbCrLf &
                "   (SELECT TOP 1 ISNULL(orgwebsite.Website ,'') " & vbCrLf &
                "       FROM MobizLibraryOrganizationContactWebsite  as orgwebsite " & vbCrLf &
                "       WHERE orgwebsite.OrganizationID = org.ID) as Website, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org2.CompanyName ,'') " & vbCrLf &
                "                               FROM MobizLibraryOrganizationAffiliates   as orgaff " & vbCrLf &
                "                               LEFT JOIN MobizLibraryOrganization as org2 " & vbCrLf &
                "                                   ON org2.ID = orgaff.AffiliatesID " & vbCrLf &
                "                               WHERE orgaff.OrganizationID = org.ID " & vbCrLf &
                "   FOR XML PATH('')), 1, 1,'') FROM MobizLibraryOrganizationAffiliates) as Affiliation, " & vbCrLf &
                "   ISNULL((SELECT city.Name " & vbCrLf &
                "       FROM MobizLibraryCities as city " & vbCrLf &
                "       LEFT JOIN MobizLibraryOrganization as org2 " & vbCrLf &
                "           ON org2.OfficeAddressCityID = city.ID " & vbCrLf &
                "       WHERE org2.ID= org.ID), '') as City, " & vbCrLf &
                "   ISNULL(CONVERT(varchar(max), org.OfficeAddressCityID), '') as CityID, " & vbCrLf &
                "   ISNULL((SELECT country.Name " & vbCrLf &
                "       FROM MobizLibraryCountries as country " & vbCrLf &
                "       LEFT JOIN MobizLibraryOrganization as org3 " & vbCrLf &
                "           ON org3.OfficeAddressCountryID = country.ID " & vbCrLf &
                "       WHERE org3.ID= org.ID), '') as Country, " & vbCrLf &
                "   CONVERT(varchar, org.LastUpdated) as LastUpdated, " & vbCrLf &
                "   'Org' as ProspectType, " & vbCrLf &
                "   (SELECT ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') " & vbCrLf &
                "       FROM MobizLibraryOrganization " & vbCrLf &
                "           LEFT JOIN MobizLibraryPeople " & vbCrLf &
                "               ON MobizLibraryOrganization.UserID = MobizLibraryPeople.ID " & vbCrLf &
                "           WHERE MobizLibraryOrganization.ID = org.ID) as ModifiedBy, " & vbCrLf &
                "   '' as ContactPerson, " & vbCrLf &
                "   --(SELECT TOP 1 convert(varchar,LastUpdated) FROM TransCRM WHERE ClientID = org.ID ORDER BY LastUpdated DESC) as LastInteractionDate, " & vbCrLf &
                "   ISNULL(org.Region, '') as Region, " & vbCrLf &
                "   ISNULL(org.OfficeAddressStreet, '') as Street, " & vbCrLf &
                "   ISNULL(org.OfficeAddressBuildingNo, '') as HouseNo, " & vbCrLf &
                "   ISNULL(org.Island, '') as Island, " & vbCrLf &
                "   (SELECT Class From MobizLibraryIndustryClass WHERE ID = org.IndustryClassID) as IndustryName, " & vbCrLf &
                "   ISNULL(CONVERT(varchar(MAX), org.IndustryClassID), '') as IndustryID, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.RevenuePerYear), '') as RevenuePerYear, " & vbCrLf &
                "   ISNULL(org.ScopeOfOperations, '') as ScopeOfOperations, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.NoOfEmployees), '') as NoOfEmployees, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.NoOfFactory), '') as NoOfFactory, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.NoOfOffice), '') as NoOfOffice, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.NoOfWarehouse), '') as NoOfWarehouse, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.NoOfStore), '') as NoOfStore, " & vbCrLf &
                "   ISNULL(convert(varchar(max),org.NoOfTruck), '') as NoOfTruck, " & vbCrLf &
                "   ISNULL(cus.CustomerCode, '') as CustomerCode, " & vbCrLf &
                "   ISNULL(org.TinNumber, '') as TinNumber " & vbCrLf &
                "FROM MobizLibraryOrganization as org" & vbCrLf &
                IIf(Common.WebRequest.Data("Type") = "Supplier", " INNER JOIN LibraryOrganizationSupplier as cus ON cus.SupplierID = org.ID ", " INNER JOIN LibraryOrganizationCustomer as cus ON cus.OrganizationID = org.ID ") & vbCrLf &
                If(Status = "", " WHERE cus.Deactivated = '0' " & vbCrLf & " AND org.IsOrganization = 1 ", If(Status = "All", " --WHERE org.IsOrganization = 1", " WHERE cus.Deactivated = " & ReadyString(If(Status = "1", 0, 1)) & " --AND org.IsOrganization = 1")) & vbCrLf & ") as oTable " & whereSearch & vbCrLf &
                "   " & vbCrLf &
                "UNION ALL  " & vbCrLf &
                "   " & vbCrLf &
                "SELECT * FROM (SELECT DISTINCT ISNULL(cusorg.ID, ppol.ID) as ID, cus.ID as CustomerID,  " & vbCrLf &
                "   CASE WHEN ISNULL(CONVERT(varchar, cus.Deactivated), 0) = 0 THEN 1 ELSE 0 END as Status, " & vbCrLf &
                "   CASE WHEN cusorg.PersonSurname IS NULL THEN LTRIM(ISNULL(cusorg.PersonFullName, ' ')) ELSE LTRIM(ISNULL(FirstName + ' ', cusorg.PersonFirstName + ' ') + ISNULL(CASE WHEN MiddleName = '' THEN cusorg.PersonMiddleName + ' ' ELSE MiddleName + ' ' END, '') + ISNULL(Surname, cusorg.PersonSurname + ' ')) END as ClientName, " & vbCrLf &
                "   (SELECT TOP 1 ISNULL(CASE WHEN ppolemail.EmailAddress = '' THEN '' ELSE ppolemail.EmailAddress END,'') " & vbCrLf &
                "       FROM MobizLibraryPeopleContactEmail as ppolemail " & vbCrLf &
                "           WHERE ppolemail.PeopleID = ISNULL(cusorg.ID, ppol.ID)) as Email," & vbCrLf &
                "   (SELECT TOP 1 " & vbCrLf &
                "       ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode) + ' ',0) + " & vbCrLf &
                "       ISNULL('(' + CONVERT(varchar, ppolareacode.CallingCode) + ') ',0) + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) + " & vbCrLf &
                "       ISNULL(CASE WHEN ppoltel.ExtensionNo = '' THEN '' ELSE ',' + ppoltel.ExtensionNo END,0) " & vbCrLf &
                "       FROM MobizLibraryPeopleContactTel as ppoltel " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                "               ON ppoltel.CountryID = ppolcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                "               ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                "       WHERE ppoltel.PeopleID = ISNULL(cusorg.ID, ppol.ID)) as Telephone," & vbCrLf &
                "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                "       --ISNULL(' (' + CONVERT(varchar, ppoltelco.Prefix) + ') ',0) + " & vbCrLf &
                "       CASE WHEN LEN(ppolmobile.ContactNo) <= 7 THEN CONVERT(varchar, ppoltelco.Prefix) ELSE '' END + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                "       FROM MobizLibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                "           WHERE ppolmobile.PeopleID = ISNULL(cusorg.ID, ppol.ID)) as Mobile, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                "       --ISNULL(CONVERT(varchar, ppoltelco.Prefix),0) + " & vbCrLf &
                "       CASE WHEN LEN(ppolmobile.ContactNo) <= 7 THEN CONVERT(varchar, ppoltelco.Prefix) ELSE '' END + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                "       FROM MobizLibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                "           WHERE ppolmobile.PeopleID = ISNULL(cusorg.ID, ppol.ID) " & vbCrLf &
                "       FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM MobizLibraryPeopleContactMobile) as MobileNos, " & vbCrLf &
                "   '' as Website, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org.CompanyName ,'') " & vbCrLf &
                "                               FROM MobizLibraryOrganizationContactPerson as cp " & vbCrLf &
                "                                   LEFT JOIN MobizLibraryOrganization as org ON org.ID = cp.OrganizationID" & vbCrLf &
                "                               WHERE cp.PeopleID = ISNULL(cusorg.ID, ppol.ID) " & vbCrLf &
                "   FOR XML PATH('')), 1, 1,'') FROM MobizLibraryOrganizationContactPerson) as Affiliation, " & vbCrLf &
                "   ISNULL(city.Name, '') as City, " & vbCrLf &
                "   ISNULL(CONVERT(varchar(max), city.ID), '') as CityID, " & vbCrLf &
                "   ISNULL(ctry.Name, '') as Country, " & vbCrLf &
                "   CONVERT(varchar, cusorg.LastUpdated) as LastUpdated, " & vbCrLf &
                "   'People' as ProspectType, " & vbCrLf &
                "   (SELECT ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') " & vbCrLf &
                "       FROM MobizLibraryPeople " & vbCrLf &
                "       WHERE MobizLibraryPeople.ID = ppol.UserID) as ModifiedBy, " & vbCrLf &
                "   '' as ContactPerson, " & vbCrLf &
                "   --(SELECT TOP 1 convert(varchar,LastUpdated) FROM TransCRM WHERE ClientID = ppol.ID ORDER BY LastUpdated DESC) as LastInteractionDate, " & vbCrLf &
                "   ISNULL(ppoladd.Region, '') as Region, " & vbCrLf &
                "   ISNULL(ppoladd.Street, '') as Street, " & vbCrLf &
                "   ISNULL(ppoladd.HouseBuildingNo, '') as HouseNo, " & vbCrLf &
                "   ISNULL(ppoladd.Island, '') as Island, " & vbCrLf &
                "   '' as IndustryName, " & vbCrLf &
                "   '' as IndustryID, " & vbCrLf &
                "   '' as RevenuePerYear, " & vbCrLf &
                "   '' as ScopeOfOperations, " & vbCrLf &
                "   '' as NoOfEmployees, " & vbCrLf &
                "   '' as NoOfFactory, " & vbCrLf &
                "   '' as NoOfOffice, " & vbCrLf &
                "   '' as NoOfWarehouse, " & vbCrLf &
                "   '' as NoOfStore, " & vbCrLf &
                "   '' as NoOfTruck, " & vbCrLf &
                "   ISNULL(cus.CustomerCode, '') as CustomerCode, " & vbCrLf &
                "   ISNULL(ppol.TIN, cusorg.TinNumber) as TinNumber " & vbCrLf &
                IIf(Common.WebRequest.Data("Type") = "Supplier", " FROM LibraryOrganizationSupplier as cus ", " FROM LibraryOrganizationCustomer as cus ") & vbCrLf &
                "   INNER JOIN MobizLibraryOrganization as cusorg ON " & IIf(Common.WebRequest.Data("Type") = "Supplier", " cus.SupplierID ", " cus.OrganizationID ") & " = cusorg.ID " & vbCrLf &
                "   LEFT JOIN MobizLibraryPeople as ppol ON cusorg.ID = ppol.ID " & vbCrLf &
                "   OUTER APPLY (SELECT TOP 1 * FROM MobizLibraryPeopleAddress WHERE ppol.ID = PeopleID ORDER BY LastUpdated DESC) as ppoladd " & vbCrLf &
                "   LEFT JOIN MobizLibraryCities as city ON city.ID = ppoladd.CityID " & vbCrLf &
                "   LEFT JOIN MobizLibraryCountries as ctry ON ctry.ID = ppoladd.CountryID" & vbCrLf &
                "   WHERE cusorg.IsOrganization = 0 --IsFromOrg = 0 " & vbCrLf & If(Status = "", " AND cus.Deactivated = '0' ", If(Status = "All", "", " and cus.Deactivated = " & ReadyString(If(Status = "1", 0, 1)))) & vbCrLf & ") as pTable " & whereSearch & vbCrLf &
                ") as V1 WHERE ClientName <> '' -- do not include empty person name " & vbCrLf &
                ") as v2 WHERE ClientName <> '' " & whereLimit
        Common.WebRequest.ProData.CommandTimeout = 5 * 60 * 1000
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetJson(ByVal dt As DataTable) As String
        Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows
                                                    Select dt.Columns.Cast(Of DataColumn)().ToDictionary(
                                                        Function(col) col.ColumnName,
                                                        Function(col) dr(col))
                                            )
    End Function

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
                .SqlWhere = "WHERE org.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
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

                If (strWords(intCntr).Trim().Length > 1) Then
                    strInitials = strInitials + strWords(intCntr).Trim().Substring(0, 1)
                End If

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

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE orgtrade.OrganizationID IS NULL", " WHERE orgtrade.OrganizationID = " + ReadyString(strID))

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

    Function getOrganizationInfo(ByVal strID As String) As DataTable
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
                With .Add("PersonDateOfBirth", SqlDbType.Date)
                    .Expression = "org.PersonDateOfBirth"
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
                With .Add("PersonSeniorCitizensIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonSeniorCitizensIDNo"
                End With
                With .Add("PersonPWDIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPWDIDNo"
                End With
                With .Add("PersonPassportNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPassportNo"
                End With
                With .Add("PersonDriversLicenseNo", SqlDbType.VarChar)
                    .Expression = "org.PersonDriversLicenseNo"
                End With
                With .Add("PersonSSSNo", SqlDbType.VarChar)
                    .Expression = "org.PersonSSSNo"
                End With
                With .Add("PersonGSISIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonGSISIDNo"
                End With
                With .Add("PersonPRCIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPRCIDNo"
                End With
                With .Add("PersonPostalIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPostalIDNo"
                End With
                With .Add("PersonVotersIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonVotersIDNo"
                End With
                With .Add("PersonOtherIDType", SqlDbType.VarChar)
                    .Expression = "org.PersonOtherIDType"
                End With
                With .Add("PersonOtherIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonOtherIDNo"
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
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "org.TinNumber"
                End With
                With .Add("IsFranchisor", SqlDbType.Bit)
                    .Expression = "org.IsFranchisor"
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
                    " LEFT JOIN dbo.MobizLibraryPeoplePreNominalSuffix orgpresuffix ON (org.PersonTitleID = orgpresuffix.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE org.ID IS NULL", " WHERE org.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrganizationInfo() As String
        Dim p As New InsertUpdate("MobizLibraryOrganization")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String

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

            'additional july 16
            If CStr2(Common.WebRequest.Data("PersonSeniorCitizensIDNo")) <> "" Then
                p("PersonSeniorCitizensIDNo") = ReadyString(PersonSeniorCitizensIDNo)
            Else
                p("PersonSeniorCitizensIDNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonPWDIDNo")) <> "" Then
                p("PersonPWDIDNo") = ReadyString(PersonPWDIDNo)
            Else
                p("PersonPWDIDNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonPassportNo")) <> "" Then
                p("PersonPassportNo") = ReadyString(PersonPassportNo)
            Else
                p("PersonPassportNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonDriversLicenseNo")) <> "" Then
                p("PersonDriversLicenseNo") = ReadyString(PersonDriversLicenseNo)
            Else
                p("PersonDriversLicenseNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonSSSNo")) <> "" Then
                p("PersonSSSNo") = ReadyString(PersonSSSNo)
            Else
                p("PersonSSSNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonGSISIDNo")) <> "" Then
                p("PersonGSISIDNo") = ReadyString(PersonGSISIDNo)
            Else
                p("PersonGSISIDNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonPRCIDNo")) <> "" Then
                p("PersonPRCIDNo") = ReadyString(PersonPRCIDNo)
            Else
                p("PersonPRCIDNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonPostalIDNo")) <> "" Then
                p("PersonPostalIDNo") = ReadyString(PersonPostalIDNo)
            Else
                p("PersonPostalIDNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonVotersIDNo")) <> "" Then
                p("PersonVotersIDNo") = ReadyString(PersonVotersIDNo)
            Else
                p("PersonVotersIDNo") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonOtherIDType")) <> "" Then
                p("PersonOtherIDType") = ReadyString(PersonOtherIDType)
            Else
                p("PersonOtherIDType") = "NULL"
            End If
            If CStr2(Common.WebRequest.Data("PersonOtherIDNo")) <> "" Then
                p("PersonOtherIDNo") = ReadyString(PersonOtherIDNo)
            Else
                p("PersonOtherIDNo") = "NULL"
            End If
            'end of additional


            p("CompanyName") = ReadyString(CompanyName)
            p("CompanyNameInitial") = ReadyString(CompanyNameInitial)
            p("TinNumber") = ReadyString(CorporateTIN)
            p("OwnershipType") = ReadyString(OwnershipType)
            p("IsFranchisor") = IsFranchisor

            p("OfficeAddressBuildingNo") = ReadyString(OfficeAddressBuildingNo)
            p("OfficeAddressStreet") = ReadyString(OfficeAddressStreet)
            p("OfficeAddressCityID") = IIf(OfficeAddressCityID.Trim().Length > 0, ReadyString(OfficeAddressCityID), "NULL")
            p("OfficeAddressCountryID") = IIf(OfficeAddressCountryID.Trim().Length > 0, ReadyString(OfficeAddressCountryID), "NULL")

            p("BusinessAddressBuildingNo") = ReadyString(BusinessAddressBuildingNo)
            p("BusinessAddressStreet") = ReadyString(BusinessAddressStreet)
            p("BusinessAddressCityID") = IIf(BusinessAddressCityID.Trim().Length > 0, ReadyString(BusinessAddressCityID), "NULL")
            p("BusinessAddressCountryID") = IIf(BusinessAddressCountryID.Trim().Length > 0, ReadyString(BusinessAddressCountryID), "NULL")

            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

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

            '''Save Default Price Level (Wholesale, SRP and Retail Only) 05/22/2013 Temporarily Advice by Sir Robert
            If (ID.Trim().Length <= 0) Then

                '''Insert Retail
                p = New InsertUpdate("LibraryOrganizationPriceLevel")
                p("OrganizationID") = ReadyString(strIDReturn)
                p("PriceLevelID") = ReadyString(WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID())
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                '''Insert SRP
                p = New InsertUpdate("LibraryOrganizationPriceLevel")
                p("OrganizationID") = ReadyString(strIDReturn)
                p("PriceLevelID") = ReadyString("EEF716C6-B290-4B28-B2A6-0A4F52CF4989")
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                '''Insert Wholesale
                p = New InsertUpdate("LibraryOrganizationPriceLevel")
                p("OrganizationID") = ReadyString(strIDReturn)
                p("PriceLevelID") = ReadyString(WebRequest.Instance.Business.SystemDefaults.GetWholesalePriceID())
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
                strIDRec = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDRec)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If

            '''Save Trade Name List
            For Each objOrgTradeName As Object In OrgTradeNameList

                If (objOrgTradeName("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgTradeName("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationTradeName")

                        p("OrganizationID") = ReadyString(strIDReturn)
                        p("BusinessTradeName") = IIf(objOrgTradeName("BusinessTradeName").ToString().Trim().Length = 0, "NULL", ReadyString(objOrgTradeName("BusinessTradeName").ToString().Trim()))
                        p("BusinessTradeNameInitial") = ReadyString(IIf(objOrgTradeName("BusinessTradeNameInitial").ToString().Trim().Length > 0, objOrgTradeName("BusinessTradeNameInitial").ToString().Trim(), getInitials(objOrgTradeName("BusinessTradeName").ToString().Trim())))

                        If (objOrgTradeName("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objOrgTradeName("ID").ToString()
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

            '''Save Category List
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

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function getOrgFOWSTList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FOWSTTypeID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "orgfowst.Code"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "orgfowst.Name"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "orgfowst.Description"
                End With
                With .Add("StreetNumber", SqlDbType.VarChar)
                    .Expression = "orgfowst.StreetNumber"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "orgfowst.Street"
                End With
                With .Add("FowstCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FowstCityID"
                    .Searchable = False
                End With
                With .Add("AddressCity", SqlDbType.VarChar)
                    .Expression = "cty.Name"
                    .Searchable = False
                End With
                With .Add("FowstCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FowstCountryID"
                    .Searchable = False
                End With
                With .Add("AddressCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                    .Searchable = False
                End With
                With .Add("CompleteAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgfowst.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgfowst.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                End With
                With .Add("SubFowstTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.SubFowstTypeID"
                    .Searchable = False
                End With
                With .Add("RetailType", SqlDbType.VarChar)
                    .Expression = "rtype.Name"
                End With
                With .Add("WarehouseType", SqlDbType.VarChar)
                    .Expression = "wtype.Name"
                End With
                With .Add("PlateNo", SqlDbType.VarChar)
                    .Expression = "orgfowst.PlateNo"
                End With
                With .Add("NoOfWheels", SqlDbType.VarChar)
                    .Expression = "orgfowst.NoOfWheels"
                End With
                With .Add("BodyColor", SqlDbType.VarChar)
                    .Expression = "orgfowst.BodyColor"
                End With
                With .Add("Weight", SqlDbType.Float)
                    .Expression = "orgfowst.Weight"
                End With
                With .Add("Dimension", SqlDbType.VarChar)
                    .Expression = "orgfowst.Dimension"
                End With
                With .Add("OwnershipType", SqlDbType.Int)
                    .Expression = "orgfowst.OwnershipType"
                End With
                With .Add("ContractStart", SqlDbType.Date)
                    .Expression = "orgfowst.ContractStart"
                End With
                With .Add("ContractEnd", SqlDbType.Date)
                    .Expression = "orgfowst.ContractEnd"
                End With
                With .Add("CostPerMonth", SqlDbType.Decimal)
                    .Expression = "orgfowst.CostPerMonth"
                End With
                With .Add("IsMortgaged", SqlDbType.Bit)
                    .Expression = "orgfowst.IsMortgaged"
                End With
                With .Add("IsFranchisee", SqlDbType.Bit)
                    .Expression = "orgfowst.IsFranchisee"
                End With
                With .Add("FranchisorID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FranchisorID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST orgfowst " &
                       " LEFT JOIN dbo.MobizLibraryCities cty ON (orgfowst.FowstCityID = cty.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgfowst.FowstCountryID = ctry.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTRetailStoreType rtype ON (orgfowst.SubFowstTypeID = rtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgfowst.SubFowstTypeID = wtype.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowst.OrganizationID IS NULL", " WHERE orgfowst.OrganizationID = " + ReadyString(strID))

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
                With .Add("SecName", SqlDbType.VarChar)
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
                    .Expression = "orgfowsttel.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowsttel.FOWSTID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgfowsttel.CountryID"
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
                    .Expression = "orgfowsttel.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgfowsttel.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "orgfowsttel.ExtensionNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgfowsttel.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTTel orgfowsttel " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgfowsttel.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (orgfowsttel.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowsttel.FOWSTID IS NULL", " WHERE orgfowsttel.FOWSTID = " + ReadyString(strID))

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
                    .Expression = "orgfowstmob.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstmob.FOWSTID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "orgfowstmob.CountryID"
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
                    .Expression = "orgfowstmob.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "orgfowstmob.ContactNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "orgfowstmob.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTMobile orgfowstmob " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (orgfowstmob.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (orgfowstmob.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowstmob.FOWSTID IS NULL", " WHERE orgfowstmob.FOWSTID = " + ReadyString(strID))

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
                    .Expression = "orgfowstemail.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstemail.FOWSTID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "orgfowstemail.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "orgfowstemail.EmailType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTEmail orgfowstemail"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowstemail.FOWSTID IS NULL", " WHERE orgfowstemail.FOWSTID = " + ReadyString(strID))
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

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FOWSTTypeID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "orgfowst.Code"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "orgfowst.Name"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "orgfowst.Description"
                End With
                With .Add("StreetNumber", SqlDbType.VarChar)
                    .Expression = "orgfowst.StreetNumber"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "orgfowst.Street"
                End With
                With .Add("AddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FowstCityID"
                    .Searchable = False
                End With
                With .Add("AddressCity", SqlDbType.VarChar)
                    .Expression = "cty.Name"
                    .Searchable = False
                End With
                With .Add("AddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FowstCountryID"
                    .Searchable = False
                End With
                With .Add("AddressCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                    .Searchable = False
                End With
                With .Add("SubFowstTypeID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.SubFowstTypeID"
                    .Searchable = False
                End With
                With .Add("RetailType", SqlDbType.VarChar)
                    .Expression = "rtype.Name"
                End With
                With .Add("WarehouseType", SqlDbType.VarChar)
                    .Expression = "wtype.Name"
                End With
                With .Add("PlateNo", SqlDbType.VarChar)
                    .Expression = "orgfowst.PlateNo"
                End With
                With .Add("NoOfWheels", SqlDbType.VarChar)
                    .Expression = "orgfowst.NoOfWheels"
                End With
                With .Add("BodyColor", SqlDbType.VarChar)
                    .Expression = "orgfowst.BodyColor"
                End With
                With .Add("Weight", SqlDbType.Float)
                    .Expression = "orgfowst.Weight"
                End With
                With .Add("Dimension", SqlDbType.VarChar)
                    .Expression = "orgfowst.Dimension"
                End With
                With .Add("OwnershipType", SqlDbType.Int)
                    .Expression = "orgfowst.OwnershipType"
                End With
                With .Add("ContractStartMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgfowst.ContractStart)"
                End With
                With .Add("ContractStartDay", SqlDbType.Date)
                    .Expression = "DAY(orgfowst.ContractStart)"
                End With
                With .Add("ContractStartYear", SqlDbType.Date)
                    .Expression = "YEAR(orgfowst.ContractStart)"
                End With
                With .Add("ContractEndMonth", SqlDbType.Date)
                    .Expression = "MONTH(orgfowst.ContractEnd)"
                End With
                With .Add("ContractEndDay", SqlDbType.Date)
                    .Expression = "DAY(orgfowst.ContractEnd)"
                End With
                With .Add("ContractEndYear", SqlDbType.Date)
                    .Expression = "YEAR(orgfowst.ContractEnd)"
                End With
                With .Add("CostPerMonth", SqlDbType.Decimal)
                    .Expression = "orgfowst.CostPerMonth"
                End With
                With .Add("IsMortgaged", SqlDbType.Bit)
                    .Expression = "orgfowst.IsMortgaged"
                End With
                With .Add("IsFranchisee", SqlDbType.Bit)
                    .Expression = "orgfowst.IsFranchisee"
                End With
                With .Add("FranchisorID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.FranchisorID"
                    .Searchable = False
                End With
                With .Add("Franchisor", SqlDbType.Bit)
                    .Expression = "franchisor.CompanyName"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST orgfowst " &
                       " LEFT JOIN dbo.MobizLibraryCities cty ON (orgfowst.FowstCityID = cty.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgfowst.FowstCountryID = ctry.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTRetailStoreType rtype ON (orgfowst.SubFowstTypeID = rtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgfowst.SubFowstTypeID = wtype.ID) " &
                       " LEFT JOIN dbo.MobizLibraryOrganization franchisor ON (orgfowst.FranchisorID = franchisor.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgfowst.ID IS NULL", " WHERE orgfowst.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveOrgFOWST() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String

        Try

            p("OrganizationID") = If(OrganizationID.Trim().Length > 0, ReadyString(OrganizationID), "NULL")
            p("FOWSTTypeID") = If(FOWSTTypeID.Trim().Length > 0, ReadyString(FOWSTTypeID), "NULL")
            p("Code") = ReadyString(FOWSTCode)
            p("Name") = ReadyString(FOWSTName)
            p("Description") = ReadyString(FOWSTDescription)

            p("StreetNumber") = ReadyString(FOWSTStreetNumber)
            p("Street") = ReadyString(FOWSTStreet)
            p("FowstCityID") = If(FOWSTCityID.Trim().Length > 0, ReadyString(FOWSTCityID), "NULL")
            p("FowstCountryID") = If(FOWSTCountryID.Trim().Length > 0, ReadyString(FOWSTCountryID), "NULL")
            p("SubFowstTypeID") = If(FOWSTSubTypeID.Trim().Length > 0, ReadyString(FOWSTSubTypeID), "NULL")

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

            p("IsFranchisee") = If(FOWSTIsFranchisee.Trim().Length > 0, FOWSTIsFranchisee, "0")
            p("FranchisorID") = If(FOWSTFranchisorID.Trim().Length > 0, ReadyString(FOWSTFranchisorID), "NULL")
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))

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

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryOrganizationFOWSTSection " &
                               " WHERE ID " & IIf(objOrgAddSection("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objOrgAddSection("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objOrgAddSection

            '''Save BPermit
            For Each objOrgAddBPermit As Object In OrgFOWSTBPermitList

                If (objOrgAddBPermit("RecState").ToString().Trim().Length > 0) Then

                    If (objOrgAddBPermit("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryOrganizationFOWSTBusinessPermit")

                        p("FOWSTID") = ReadyString(strIDReturn)
                        p("BusinessPermitNo") = ReadyString(objOrgAddBPermit("PermitNo").ToString())
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

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
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
                    .Expression = "custrep.ID"
                    .Searchable = False
                End With
                With .Add("RepresentativeName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custrep.Title)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.LastName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.MiddleName)) + ' ',' '), '') + " &
                                  "(CASE " &
                                  "     WHEN len(rtrim(ltrim(ISNULL(custrep.GenerationalSuffix,'')))) > 0 THEN " &
                                  "          ', ' + custrep.GenerationalSuffix " &
                                  "     ELSE " &
                                  "          '' " &
                                  " END) "
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "ISNULL(custrep.IsActive,1)"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationRepresentatives custrep "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE custrep.OrganizationID IS NULL", " WHERE custrep.OrganizationID = " + ReadyString(strID))

            .SqlOrder = "ORDER BY custrep.IsActive DESC, custrep.LastName ASC, custrep.FirstName ASC"

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
                    .Expression = "custrep.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "custrep.OrganizationID"
                    .Searchable = False
                End With
                With .Add("LastName", SqlDbType.VarChar)
                    .Expression = "custrep.LastName"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "custrep.FirstName"
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "custrep.MiddleName"
                End With
                With .Add("Title", SqlDbType.VarChar)
                    .Expression = "custrep.Title "
                End With
                With .Add("GenerationalSuffix", SqlDbType.VarChar)
                    .Expression = "custrep.GenerationalSuffix"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "ISNULL(custrep.IsActive,0)"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationRepresentatives custrep "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE custrep.ID IS NULL", " WHERE custrep.ID = " + ReadyString(strID))

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
        Dim p As New InsertUpdate("MobizLibraryOrganizationRepresentatives")
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

    Public Function GenerateCodeNumber(ByVal IsCustomer As Boolean) As Integer
        Dim pSql As String
        Dim column As String
        Dim code As Integer = New System.Random().Next(1, 9999999)

        If IsCustomer Then
            pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE CustomerCode = " & ReadyString(code)
        Else
            pSql = "SELECT ID FROM LibraryOrganizationSupplier WHERE VendorCode = " & ReadyString(code)
        End If

        If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
            code = GenerateCodeNumber(IsCustomer)
        End If

        Return code
    End Function
End Class