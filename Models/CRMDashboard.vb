Imports System
Imports System.Threading
Imports System.ComponentModel
Imports System.IO.Ports
Imports Newtonsoft.Json

Public Class CRMDashboard
    Inherits System.Web.Mvc.Controller

#Region "Variables"
    Public Property MaxReturnRow As Integer = 50
    Public Property OrgID As String
    Public Property CompanyID As String
    Public Property CompanyName As String
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
    Public Property ObjTelephone As Object
    Public Property ObjMobile As Object
    Public Property ObjEmail As Object
    Public Property ObjWebsite As Object
    Public Property ObjContactPerson As Object
    Public Property ObjContactPersonTelephone As Object
    Public Property ObjContactPersonMobile As Object
    Public Property ObjContactPersonEmail As Object
    Public Property ObjOrgAff As Object
    Public Property ObjAffiliatedPerson As Object
    Public Property ObjAddress As Object
    Public Property DateTime As DateTime
    Public Property NoOfEmployees As String
    Public Property RevenuePerYear As String
    Public Property Scope As String
    Public Property NoOfFactories As String
    Public Property NoOfOffices As String
    Public Property NoOfWarehouses As String
    Public Property NoOfStores As String
    Public Property NoOfTrucks As String

    Public Property Status As String

    Public Property PersonID As String
    Public Property FirstName As String
    Public Property MiddleName As String
    Public Property LastName As String
    Public Property Birthday As String
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

    Public Property ClientID As String
    Public Property ObjClient As Object
    Public Property CallNotes As String
    Public Property CallStartDate As String
    Public Property CallEndDate As String
    Public Property CallDuration As String
    Public Property IsOrg As String
    Public Property PageStart As Integer
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String
    Public Property Filter As String
    Public Property YearWeek As String
    Public Property LastWeek As String
    Public Property CrmType As String
    Public Property InvType As String
    Public Property SearchContact As String
    Public Property IsCountPage As String
    Public Property SchedAnActionID As String
    Public Property Location As String
    Public Property Note As String
    Public Property Attachments As Object
    Public Property NextActionName As String
    Public Property Subject As String
    Public Property IsAllDay As Boolean
    Public Property DateTimeFrom As Date
    Public Property DateTimeTo As Date
    Public Property TeamAssign As String
    Public Property TodayDate As Date
    Public Property CurrentModule As String = ""

    Public Property Island As String

    Public Property ObjToDo As Object
    Public Property UnfinishToDo As String = False

    Public Property Search As String
    Public Property GenNumberType As String

    Public Property OpportunityStatus As String
    Public Property OpportunityName As String
    Public Property OpportunityDescription As String
    Public Property OpportunitySize As String
    Public Property OpportunitySizePeriod As String
    Public Property OpportunityTemplate As String
    Public Property OpportunityChecklist As Object

    Private WithEvents SMSPort As SerialPort

    Private SMSThread As Thread

    Private ReadThread As Thread

    Shared _Continue As Boolean = False

    Shared _ContSMS As Boolean = False

    Private _Wait As Boolean = False

    Shared _ReadPort As Boolean = False

    Public Event Sending(ByVal Done As Boolean)

    Public Event DataReceived(ByVal Message As String)

    Private ID As String
#End Region

#Region "Variables Activity Quota"
    Public Property TeamID As String
    Public Property DatePeriod As Object
    Public Property Calls As String
    Public Property Notes As String
    Public Property PhysicalAttended As String
    Public Property VirtualAttended As String
    Public Property Email As String
    Public Property Sms As String
    Public Property ScheduleCall As String
    Public Property ScheduleEmail As String
    Public Property ScheduleSms As String
    Public Property SchedulePhysical As String
    Public Property ScheduleVirtual As String

    Public Property Territory As String
    Public Property User As String
    Public Property DateRange As String
#End Region

#Region "Staging"
    Public Property Stage As Object
    Public Property GroupStageName As String
    Public Property GroupStageID As String
#End Region

    Function ImportCustomerVendorDataToCrm() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            Dim pSql As String = "SELECT OrganizationID FROM LibraryOrganizationCustomer "

            'pSql = "SELECT ID, CompanyName, OfficeAddressBuildingNo, OfficeAddressStreet, OfficeAddressCityID, OfficeAddressCountryID, " & vbCrLf &
            '                    " IsActive, Background, Mission, Vision, FoundingDate, IndustryClassID, Region, NoOfFactory, NoOfStore, " & vbCrLf &
            '                    " NoOfTruck, RevenuePerYear, ScopeOfOperations " & vbCrLf &
            '                    " FROM LibraryOrganization " & vbCrLf &
            '                    " ORDER BY CompanyName ASC "
            Dim pRowsCustomer As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim p As New InsertUpdate("MobizLibraryOrganization")
            Dim ctrAdded As Integer = 0
            Dim ctrUpdated As Integer = 0

            For Each customer As Object In pRowsCustomer
                'ORGANIZATION DETAILS
                pSql = "SELECT * FROM MobizLibraryOrganization WHERE ID = " & ReadyString(customer("OrganizationID").ToString())
                Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If rows.Count <> 0 Then
                    Dim orgDetails As Object = rows(0)

                    If CBool2(orgDetails("IsOrganization")) Then
                        Dim errMsg As String = ""

                        If SaveOrganizationFromCustomerVendorLibraries(orgDetails, ctrAdded, ctrUpdated, errMsg) = False Then
                            Return JsonDictionary.Serialize("Results", errMsg)
                        End If
                    Else
                        p = New InsertUpdate("LibraryPeople")

                        p("Surname") = ReadyString(orgDetails("PersonSurname"))
                        p("FirstName") = ReadyString(orgDetails("PersonFirstName"))
                        p("MiddleName") = ReadyString(orgDetails("PersonMiddleName"))
                        p("IsActive") = 1
                        p("DateOfBirth") = ReadyString(orgDetails("PersonDateOfBirth"))
                        p("PersonalSuffixName") = ReadyString(orgDetails("PersonSuffixName"))
                        p("LastUpdated") = "getdate()"

                        pSql = "SELECT ID FROM LibraryPeople WHERE ID = " & ReadyString(orgDetails("ID").ToString())

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                            p("ID") = ReadyString(orgDetails("ID").ToString())
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                            ctrAdded += 1
                        Else
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(orgDetails("ID").ToString())))
                            ctrUpdated += 1
                        End If

                        pSql = "SELECT * FROM LibraryPeopleAddress WHERE PeopleID = " & ReadyString(orgDetails("ID").ToString())
                        Dim cpAddress As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        If cpAddress.Count > 0 Then
                            p = New InsertUpdate("LibraryPeopleAddress")

                            p("PeopleID") = ReadyString(orgDetails("ID").ToString())
                            p("HouseBuildingNo") = ReadyString(cpAddress(0)("HouseBuildingNo"))
                            p("Street") = ReadyString(cpAddress(0)("Street"))
                            p("CityID") = IIf(CStr2(cpAddress(0)("CityID")) = "", "NULL", ReadyString(cpAddress(0)("CityID")))
                            p("CountryID") = IIf(CStr2(cpAddress(0)("CountryID")) = "", "NULL", ReadyString(cpAddress(0)("CountryID")))
                            p("AddressType") = ReadyString(cpAddress(0)("AddressType"))
                            p("LastUpdated") = "getdate()"

                            pSql = "SELECT ID FROM LibraryPeopleAddress WHERE ID = " & ReadyString(cpAddress(0)("ID").ToString())

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("ID") = ReadyString(cpAddress(0)("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                                ctrAdded += 1
                            Else
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpAddress(0)("ID").ToString())))
                                ctrUpdated += 1
                            End If
                        End If

                        pSql = "SELECT * FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(orgDetails("ID").ToString())
                        Dim cpEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        For Each email As Object In cpEmail
                            pSql = "SELECT ID FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(email("ID").ToString())

                            p = New InsertUpdate("LibraryPeopleContactEmail")

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

                        pSql = "SELECT * FROM MobizLibraryPeopleContactTel WHERE PeopleID = " & ReadyString(orgDetails("ID").ToString())
                        Dim cpTel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        For Each tel As Object In cpTel
                            pSql = "SELECT ID FROM LibraryPeopleContactTel WHERE ID = " & ReadyString(tel("ID").ToString())

                            p = New InsertUpdate("LibraryPeopleContactTel")

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

                        pSql = "SELECT * FROM MobizLibraryPeopleContactMobile WHERE PeopleID = " & ReadyString(orgDetails("ID").ToString())
                        Dim cpMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        For Each mobile As Object In cpMobile
                            pSql = "SELECT ID FROM LibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("ID").ToString())

                            p = New InsertUpdate("LibraryPeopleContactMobile")

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
                End If
            Next customer

            pSql = "SELECT SupplierID FROM LibraryOrganizationSupplier "
            Dim pRowsSupplier As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            For Each supplier As Object In pRowsSupplier
                'ORGANIZATION DETAILS
                pSql = "SELECT * FROM MobizLibraryOrganization WHERE ID = " & ReadyString(supplier("SupplierID").ToString())
                Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If rows.Count <> 0 Then
                    Dim orgDetails As Object = rows(0)
                    Dim errMsg As String = ""

                    If SaveOrganizationFromCustomerVendorLibraries(orgDetails, ctrAdded, ctrUpdated, errMsg) = False Then
                        Return JsonDictionary.Serialize("Results", errMsg)
                    End If
                End If
            Next supplier

            Common.WebRequest.ProData.CommitTransaction()

            If pRowsCustomer.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "saved")
            Else
                Return JsonDictionary.Serialize("Results", "No data available")
            End If

        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Public Function SaveOrganizationFromCustomerVendorLibraries(ByVal orgDetails As Object, ByRef ctrAdded As Integer, ByRef ctrUpdated As Integer, ByRef errMsg As String) As String
        Try
            Dim pSql As String
            Dim p As New InsertUpdate("LibraryOrganization")

            p("ID") = ReadyString(orgDetails("ID").ToString())
            p("CompanyName") = ReadyString(orgDetails("CompanyName"))
            p("OfficeAddressBuildingNo") = ReadyString(orgDetails("OfficeAddressStreet"))
            p("OfficeAddressCityID") = IIf(CStr2(orgDetails("OfficeAddressCityID")) = "", "NULL", ReadyString(orgDetails("OfficeAddressCityID")))
            p("OfficeAddressCountryID") = IIf(CStr2(orgDetails("OfficeAddressCountryID")) = "", "NULL", ReadyString(orgDetails("OfficeAddressCountryID")))
            p("IsOrganization") = "1"
            p("IsActive") = ReadyString(orgDetails("IsActive"))
            p("Background") = ReadyString(orgDetails("Background"))
            p("Mission") = ReadyString(orgDetails("Mission"))
            p("Vision") = ReadyString(orgDetails("Vision"))
            p("FoundingDate") = ReadyString(orgDetails("FoundingDate"))
            p("IndustryClassID") = IIf(CStr2(orgDetails("IndustryClassID")) = "", "NULL", ReadyString(orgDetails("IndustryClassID")))
            p("Region") = ReadyString(orgDetails("Region"))
            p("NoOfFactory") = ReadyString(CDec2(orgDetails("NoOfFactory")))
            p("NoOfStore") = ReadyString(CDec2(orgDetails("NoOfStore")))
            p("NoOfTruck") = ReadyString(CDec2(orgDetails("NoOfTruck")))
            p("RevenuePerYear") = ReadyString(CDec2(orgDetails("RevenuePerYear")))
            p("ScopeOfOperations") = ReadyString(orgDetails("ScopeOfOperations"))
            p("LastUpdated") = "getdate()"

            pSql = "SELECT ID FROM LibraryOrganization WHERE ID = " & ReadyString(orgDetails("ID").ToString())

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                Common.WebRequest.ProData.Execute(p.GetInsert())
                ctrAdded += 1
            Else
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(orgDetails("ID").ToString())))
                ctrUpdated += 1
            End If

            'ORGANIZATION EMAILS

            pSql = "SELECT * FROM MobizLibraryOrganizationContactEmail WHERE OrganizationID = " & ReadyString(orgDetails("ID").ToString())
            Dim rowEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            For Each email As Object In rowEmail
                pSql = "SELECT ID FROM LibraryOrganizationContactEmail WHERE ID = " & ReadyString(email("ID").ToString())

                p = New InsertUpdate("LibraryOrganizationContactEmail")

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

            'ORGANIZATION MOBILE

            pSql = "SELECT * FROM MobizLibraryOrganizationContactMobile WHERE OrganizationID = " & ReadyString(orgDetails("ID").ToString())
            Dim rowMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            For Each mobile As Object In rowMobile
                pSql = "SELECT ID FROM LibraryOrganizationContactMobile WHERE ID = " & ReadyString(mobile("ID").ToString())

                p = New InsertUpdate("LibraryOrganizationContactMobile")

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

            'ORGANIZATION TELEPHONE

            pSql = "SELECT * FROM MobizLibraryOrganizationContactTel WHERE OrganizationID = " & ReadyString(orgDetails("ID").ToString())
            Dim rowTel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            For Each tel As Object In rowTel
                pSql = "SELECT ID FROM LibraryOrganizationContactTel WHERE ID = " & ReadyString(tel("ID").ToString())

                p = New InsertUpdate("LibraryOrganizationContactTel")

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

            'ORGANIZATION WEBSITES

            pSql = "SELECT * FROM MobizLibraryOrganizationContactWebsite WHERE OrganizationID = " & ReadyString(orgDetails("ID").ToString())
            Dim rowWebsite As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            For Each web As Object In rowWebsite
                pSql = "SELECT ID FROM LibraryOrganizationContactWebsite WHERE ID = " & ReadyString(web("ID").ToString())

                p = New InsertUpdate("LibraryOrganizationContactWebsite")

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

            'ORGANIZATION CONTACT PERSON

            pSql = "SELECT * FROM MobizLibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(orgDetails("ID").ToString())
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

                pSql = "SELECT * FROM MobizLibraryPeople WHERE ID = " & ReadyString(cp("PeopleID").ToString())
                Dim cpDetails As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If cpDetails.Count > 0 Then
                    p = New InsertUpdate("LibraryPeople")

                    p("Surname") = ReadyString(cpDetails(0)("Surname"))
                    p("FirstName") = ReadyString(cpDetails(0)("FirstName"))
                    p("MiddleName") = ReadyString(cpDetails(0)("MiddleName"))
                    p("PersonalSuffixName") = ReadyString(cpDetails(0)("PersonalSuffixName"))
                    p("Position") = ReadyString(cpDetails(0)("Position"))
                    p("PositionDescription") = ReadyString(cpDetails(0)("PositionDescription"))

                    pSql = "SELECT ID FROM LibraryPeople WHERE ID = " & ReadyString(cp("PeopleID").ToString())

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(cpDetails(0)("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                        ctrAdded += 1
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpDetails(0)("ID").ToString())))
                        ctrUpdated += 1
                    End If

                    pSql = "SELECT * FROM MobizLibraryPeopleAddress WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                    Dim cpAddress As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    If cpAddress.Count > 0 Then
                        p = New InsertUpdate("LibraryPeopleAddress")

                        p("PeopleID") = ReadyString(cpDetails(0)("ID").ToString())
                        p("HouseBuildingNo") = ReadyString(cpAddress(0)("HouseBuildingNo"))
                        p("Street") = ReadyString(cpAddress(0)("Street"))
                        p("CityID") = IIf(CStr2(cpAddress(0)("CityID")) = "", "NULL", ReadyString(cpAddress(0)("CityID")))
                        p("CountryID") = IIf(CStr2(cpAddress(0)("CountryID")) = "", "NULL", ReadyString(cpAddress(0)("CountryID")))
                        p("AddressType") = ReadyString(cpAddress(0)("AddressType"))
                        p("LastUpdated") = "getdate()"

                        pSql = "SELECT ID FROM LibraryPeopleAddress WHERE ID = " & ReadyString(cpAddress(0)("ID").ToString())

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                            p("ID") = ReadyString(cpAddress(0)("ID").ToString())
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                            ctrAdded += 1
                        Else
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(cpAddress(0)("ID").ToString())))
                            ctrUpdated += 1
                        End If
                    End If

                    pSql = "SELECT * FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                    Dim cpEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    For Each email As Object In cpEmail
                        pSql = "SELECT ID FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(email("ID").ToString())

                        p = New InsertUpdate("LibraryPeopleContactEmail")

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

                    pSql = "SELECT * FROM MobizLibraryPeopleContactTel WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                    Dim cpTel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    For Each tel As Object In cpTel
                        pSql = "SELECT ID FROM LibraryPeopleContactTel WHERE ID = " & ReadyString(tel("ID").ToString())

                        p = New InsertUpdate("LibraryPeopleContactTel")

                        p("PeopleID") = ReadyString(tel("PeopleID").ToString())
                        p("CountryID") = ReadyString(tel("CountryID"))
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

                    pSql = "SELECT * FROM MobizLibraryPeopleContactMobile WHERE PeopleID = " & ReadyString(cpDetails(0)("ID").ToString())
                    Dim cpMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    For Each mobile As Object In cpMobile
                        pSql = "SELECT ID FROM LibraryPeopleContactMobile WHERE ID = " & ReadyString(mobile("ID").ToString())

                        p = New InsertUpdate("LibraryPeopleContactMobile")

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

            Return True

        Catch ex As Exception
            errMsg = ex.Message
            Return False
        End Try
    End Function

    Function GetFiles(ByVal ClientID As String) As DataTable
        Dim pSql As String = "SELECT * FROM(SELECT res.Answer as FileName, " & vbCrLf &
                            "res.Description as ServerFileName, " & vbCrLf &
                            "resd.DateResponded as DateUploaded " & vbCrLf &
                            "FROM LibrarySurveyRespondent as resd " & vbCrLf &
                            "LEFT JOIN LibrarySurveyResponse as res ON resd.ID = res.RespondentID " & vbCrLf &
                            "WHERE ISNULL(convert(varchar(max), res.FieldID), '') = '' AND res.Description <> '' " & vbCrLf &
                            "AND resd.ClientID = " & ReadyString(ClientID) & vbCrLf &
                            "UNION ALL " & vbCrLf &
                            "SELECT " & vbCrLf &
                            "SUBSTRING(att.FileName,CHARINDEX('_',att.FileName,0)+1,LEN(att.FileName)) as FileName, " & vbCrLf &
                            "att.FileName as ServerFileName, " & vbCrLf &
                            "att.LastUpdated as DateUploaded " & vbCrLf &
                            "FROM LibraryCRMAttachments as att " & vbCrLf &
                            "WHERE att.ClientID = " & ReadyString(ClientID) & vbCrLf &
                            ") as t ORDER BY FileName ASC"

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetRMAHistory() As DataTable
        Dim pSql As String = "SELECT rma.*, modified.FirstName + ' ' + modified.MiddleName + ' ' + modified.Surname as ModifiedBy" & vbCrLf &
                                " FROM LibraryCRMRMA as rma LEFT JOIN MobizLibraryPeople as modified ON modified.ID = rma.CreatedBy WHERE rma.ID = " & ReadyString(Common.WebRequest.Data("RMAID")) &
                                " OR rma.RelatedID = " & ReadyString(Common.WebRequest.Data("RMAID")) & vbCrLf &
                                " ORDER BY rma.LastUpdated DESC "
        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function SaveRMA(ByRef data As Object) As String
        GenNumberType = "RMA"

        Dim p As New InsertUpdate("LibraryCRMRMA")
        Dim rmaNo As String = GenerateNumber()
        Dim pSql As String
        Dim status As String
        Dim ID As String
        Dim isExist As Boolean = False

        Try
            p("TicketID") = ReadyString(data("TicketID"))
            p("ReturnTo") = ReadyString(data("ReturnTo"))
            p("RMADate") = ReadyString(data("RMADate"))
            p("LogisticCompany") = ReadyString(data("LogisticCompany"))
            p("WBBLRef") = ReadyString(data("WBBLRef"))
            p("WBBLDate") = IIf(data("WBBLDate") = "", "NULL", ReadyString(data("WBBLDate")))
            p("ModeOfTransport") = ReadyString(data("ModeOfTransport"))
            p("WBBLAmount") = ReadyString(data("WBBLAmount"))
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(data("Date")))

            Common.WebRequest.ProData.BeginTransaction()

            If Trim(data("ID")) = "" Then
                ID = Guid.NewGuid.ToString()
                rmaNo = data("TicketNumber").ToString() + "-" + rmaNo.ToString()
                p("RMANumber") = ReadyString(rmaNo)
                p("ID") = ReadyString(id)
                pSql = p.GetInsert()
                ValidateRMAItems(data("Items"), ID, data("Date"))
                status = "saved"
                Common.WebRequest.ProData.Execute(pSql)
            Else
                Dim pDT As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM LibraryCRMRMA WHERE ID = " & ReadyString(data("ID")))
                Dim pRows As Object = pDT.Rows(0)

                ID = IIf(pRows("RelatedID").ToString() = "", pRows("ID").ToString(), pRows("RelatedID").ToString())

                'Check if there are changes
                If data("ReturnTo") = pRows("ReturnTo") And data("RMADate") = pRows("RMADate") And
                    data("LogisticCompany") = pRows("LogisticCompany") And data("WBBLRef") = pRows("WBBLRef") And
                    data("WBBLDate") = CStr(pRows("WBBLDate").ToString()) And data("ModeOfTransport") = pRows("ModeOfTransport") And
                    CDec(data("WBBLAmount")) = CDec(pRows("WBBLAmount")) Then
                    ValidateRMAItems(data("Items"), ID, data("Date"))
                    status = "exist"
                    isExist = True
                End If

                If isExist = False Then
                    rmaNo = data("RMANumber")
                    p("RMANumber") = ReadyString(rmaNo)
                    p("RelatedID") = ReadyString(ID)
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(pSql)
                    ValidateRMAItems(data("Items"), ID, data("Date"))
                    status = "updated"
                End If
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return status & "||" & rmaNo
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return "error||" & ex.Message
        End Try
    End Function

    Function ValidateRMAItems(ByVal items As Object, ByVal RMAID As String, ByVal DateNow As String) As String
        Dim p As New InsertUpdate("LibraryCRMRMAItems")
        Dim pSql As String

        Try
            'Common.WebRequest.ProData.BeginTransaction()

            For Each item As Object In items
                p("RMAID") = ReadyString(RMAID)
                p("Qty") = ReadyString(item("Qty"))
                p("Unit") = ReadyString(item("Unit"))
                p("Description") = ReadyString(item("Description"))
                p("SerialNo") = ReadyString(item("SerialNo"))
                p("InvoiceRefNo") = ReadyString(item("InvoiceRefNo"))
                p("Remark") = ReadyString(item("Remark"))
                p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateNow))

                If CStr(item("ID").ToString()) = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Status = "saved"
                Else
                    If CStr(item("State")) = "edit" Then
                        pSql = "SELECT * FROM LibraryCRMRMAItems WHERE ID = " & ReadyString(item("ID"))
                        Dim pDTItems As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                        If pDTItems.Count > 0 Then
                            If CInt(item("Qty")) = CInt(pDTItems(0)("Qty")) And item("Unit") = pDTItems(0)("Unit") And
                                item("Description") = pDTItems(0)("Description") And item("SerialNo") = pDTItems(0)("SerialNo") And
                                item("InvoiceRefNo") = CStr(pDTItems(0)("InvoiceRefNo").ToString()) And item("Remark") = pDTItems(0)("Remark") Then
                                Status = "exist"
                            Else
                                p("RelatedID") = ReadyString(item("ID").ToString())
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                                Status = "updated"
                            End If
                        End If
                    Else
                        Dim id As String = IIf(item("RelatedID").ToString() = "", item("ID"), item("RelatedID").ToString())
                        pSql = p.GetUpdate(" ID = " & ReadyString(id) & " OR RelatedID = " & ReadyString(id))
                    End If
                End If

                Common.WebRequest.ProData.Execute(pSql)
            Next item
            'Common.WebRequest.ProData.CommitTransaction()

            Return Status
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try

    End Function

    Function GetAttachedFiles(ByVal RelatedID As String) As DataTable
        Return Common.WebRequest.ProData.Execute("SELECT * FROM LibraryCRMAttachments WHERE RelatedID = " & ReadyString(RelatedID))
    End Function

    Function SaveAttachFile() As Object
        Dim p As New InsertUpdate("LibraryCRMAttachments")

        Try
            Common.WebRequest.ProData.BeginTransaction()

            For i As Integer = 0 To Request.Files.Count - 1
                Dim filePic As HttpPostedFileBase = Request.Files(i)

                'Dim filetype As Object = Split(filePic.ContentType, "/")
                Dim pSourceURL As String = Common.ServerPath & "temp\" & WebSession.Instance.OrganizationID
                Dim location As String
                'pSourceURL = pSourceURL & "\" & OrgID

                location = pSourceURL & "\" & filePic.FileName

                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                filePic.SaveAs(location)

                p("ID") = ReadyString(Split(filePic.FileName, "_")(0).ToString())
                p("RelatedID") = ReadyString(Request.Form.GetValues("NoteID"))
                p("Location") = CInt2(location)
                Common.WebRequest.ProData.Execute(p.GetInsert())
            Next

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
            'Return Json(New With {Key .Location = pFilename,
            '                .CaretPos = caretpos,
            '                .ImgID = imgid,
            '                .Status = "add"
            '            })
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Function GetTemplateDetails(ByVal ID As String) As Object
        Dim tempID As String = ID

        Dim pSql As String = "SELECT * FROM LibraryCRMFunnelTemplate WHERE ID = " & ReadyString(tempID)
        Dim pDTTemp As DataTable = Common.WebRequest.ProData.Execute(pSql)

        Dim dataTemp As Object = pDTTemp.Rows(0)

        pSql = "SELECT * FROM LibraryCRMFunnelStage WHERE FunnelTemplateID = " & ReadyString(dataTemp("ID")) & vbCrLf &
                " ORDER BY OrderNo ASC "
        Dim pDTStage As DataTable = Common.WebRequest.ProData.Execute(pSql)
        Dim stage(pDTStage.Rows.Count - 1) As Object

        For i As Integer = 0 To pDTStage.Rows.Count - 1
            Dim dataStage As Object = pDTStage.Rows(i)
            pSql = "SELECT * FROM LibraryCRMFunnelStageChild WHERE FunnelStageID = " & ReadyString(dataStage("ID")) & vbCrLf &
                    " ORDER BY OrderNo ASC "
            Dim pDTChild As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim child(pDTChild.Rows.Count - 1) As Object

            For m As Integer = 0 To pDTChild.Rows.Count - 1
                Dim dataChild As Object = pDTChild.Rows(m)
                child(m) = New With {Key .SpecialID = dataChild("ID").ToString(),
                                        .Name = dataChild("Description"),
                                        .No = CInt2(dataChild("OrderNo")),
                                        .State = "edit"}
            Next

            stage(i) = New With {Key .SpecialID = dataStage("ID").ToString(),
                                    .Name = dataStage("Description"),
                                    .No = CInt2(dataStage("OrderNo")),
                                    .State = "edit",
                                    .Child = child}
        Next

        Return New With {Key .ID = pDTTemp.Rows(0)("ID"),
                            .TemplateName = pDTTemp.Rows(0)("TemplateName"),
                            .Stages = stage,
                            .State = "edit"}

    End Function

    Function GetFunnelTemplate() As Object
        Dim pSql As String = "SELECT temp.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as CreatedByName  FROM LibraryCRMFunnelTemplate as temp " & vbCrLf &
                "INNER JOIN MobizLibraryPeople as ppol ON temp.UserID = ppol.ID " & vbCrLf &
                "WHERE CONVERT(bit, ISNULL(IsDelete, 0)) <> 1"

        Dim pDt As DataTable = Common.WebRequest.ProData.Execute(pSql)
        Dim arrTemplate(pDt.Rows.Count - 1) As Object

        If CBool(Common.WebRequest.Data("IsFromSystemAdmin")) = True Then
            Return jss.Deserialize(Of Object)(GetJson(pDt))
        Else
            For i As Integer = 0 To pDt.Rows.Count - 1
                arrTemplate(i) = GetTemplateDetails(pDt.Rows(i)("ID").ToString())
            Next

            Return arrTemplate 'Common.WebRequest.ProData.Execute(pSql)
        End If

    End Function

    Function SaveFunnelTemplate() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("LibraryCRMFunnelTemplate")
        Dim templateID As String = Guid.NewGuid.ToString()

        Try
            p("TemplateName") = ReadyString(Common.WebRequest.Data("TemplateName"))
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(Common.WebRequest.Data("DateNow")))

            Common.WebRequest.ProData.BeginTransaction()

            If CStr2(Common.WebRequest.Data("State")) = "new" Then
                p("ID") = ReadyString(templateID)
                pSql = p.GetInsert()
            ElseIf CStr2(Common.WebRequest.Data("State")) = "edit" Then
                templateID = CStr2(Common.WebRequest.Data("ID"))
                pSql = p.GetUpdate(" ID = " & ReadyString(templateID))
            ElseIf CStr2(Common.WebRequest.Data("State")) = "delete" Then
                'pSql = "DELETE FROM LibraryCRMFunnelStageChild WHERE FunnelStageID IN (" & vbCrLf &
                '        "SELECT ID FROM LibraryCRMFunnelStage WHERE FunnelTemplateID = " & ReadyString(CStr2(Common.WebRequest.Data("ID"))) & vbCrLf &
                '        ")"
                'Common.WebRequest.ProData.ExecuteSQL(pSql)

                'pSql = "DELETE FROM LibraryCRMFunnelStage WHERE FunnelTemplateID = " & ReadyString(CStr2(Common.WebRequest.Data("ID")))
                'Common.WebRequest.ProData.ExecuteSQL(pSql)

                'pSql = "DELETE FROM LibraryCRMFunnelTemplate WHERE ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID")))
                'Common.WebRequest.ProData.ExecuteSQL(pSql)

                'pSql = "DELETE FROM LibraryCRMStagingChecklist WHERE FunnelTemplateID = " & ReadyString(CStr2(Common.WebRequest.Data("ID")))
                p("IsDelete") = ReadyString("1")
                pSql = p.GetUpdate(" ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID"))))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If CStr2(Common.WebRequest.Data("State")) <> "delete" Then
                For Each stage As Object In Common.WebRequest.AllData("Stages")
                    Dim stageID As String = Guid.NewGuid.ToString()

                    p = New InsertUpdate("LibraryCRMFunnelStage")

                    p("FunnelTemplateID") = ReadyString(templateID)
                    p("Description") = ReadyString(stage("Name"))
                    p("OrderNo") = ReadyString(stage("No"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = ReadyString(CDate(Common.WebRequest.Data("DateNow")))

                    If stage("State") = "new" Then
                        p("ID") = ReadyString(stageID)
                        pSql = p.GetInsert()
                    ElseIf stage("State") = "edit" Then
                        stageID = CStr2(stage("SpecialID"))
                        pSql = p.GetUpdate(" ID = " & ReadyString(stageID))
                    ElseIf stage("State") = "delete" Then
                        pSql = "DELETE FROM LibraryCRMFunnelStageChild WHERE FunnelStageID = " & ReadyString(CStr2(stage("SpecialID")))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        pSql = "DELETE FROM LibraryCRMFunnelStage WHERE ID = " & ReadyString(CStr2(stage("SpecialID")))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If stage("State") <> "delete" Then
                        For Each child As Object In stage("Child")
                            p = New InsertUpdate("LibraryCRMFunnelStageChild")

                            p("FunnelStageID") = ReadyString(stageID)
                            p("Description") = ReadyString(child("Name"))
                            p("OrderNo") = ReadyString(child("No"))
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            p("LastUpdated") = ReadyString(CDate(Common.WebRequest.Data("DateNow")))

                            If child("State") = "new" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                            ElseIf child("State") = "edit" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(child("SpecialID")))
                            ElseIf child("State") = "delete" Then
                                pSql = "DELETE FROM LibraryCRMFunnelStageChild WHERE ID = " & ReadyString(CStr2(child("SpecialID")))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Next child
                    End If

                Next stage
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function GetOppComments() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "crm.ID"
                End With
                With .Add("Comment", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(crm.Notes, crm.ToDo)"
                End With
                With .Add("CommentedDate", SqlDbType.UniqueIdentifier)
                    .Expression = "crm.LastUpdated"
                End With
                With .Add("CommentedBy", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname"
                End With
                .Add("ActionName", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM TransCRM as crm" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = crm.CreatedBy " & vbCrLf

            .SqlWhere = "WHERE RelatedID = " & ReadyString(Common.WebRequest.Data("OppID")) & vbCrLf &
                        " AND (ActionName = 'Comment' OR ActionName = 'Note' OR ActionName = 'Call' OR ActionName = 'To-Do') " & vbCrLf &
                        " AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 "

            .SqlOrder = "ORDER BY crm.LastUpdated ASC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveOppComment() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("TransCRM")
        Dim commentID As String = Guid.NewGuid.ToString()

        Try
            p("ClientID") = ReadyString(Common.WebRequest.Data("ClientID"))
            p("RelatedID") = ReadyString(Common.WebRequest.Data("OppID"))
            p("Notes") = ReadyString(Common.WebRequest.Data("Comment"))
            p("ActionName") = ReadyString("Comment")
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(Common.WebRequest.Data("Date")))

            Common.WebRequest.ProData.BeginTransaction()

            p("ID") = ReadyString(commentID)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return commentID
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function GetOpportunity() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        Try
            If PageSearchText <> "" Then
                Dim whereSearchContent As String = ""
                Dim operators As String = ""

                Dim arr As Object = Split(PageSearchText, " ")

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " OR "
                    End If

                    whereSearchContent = whereSearchContent & operators & " ClientName " & vbCrLf &
                                    " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next
                whereSearch = " WHERE (" & whereSearchContent & ")"
            End If

            If IsCountPage = True Then
                whereLimit = ""
            End If

            If PageColumnNo = 0 Or PageColumnNo = 1 Then
                whereOrder = " ORDER BY mainsql.ClientName " & PageColumnDirection
            ElseIf PageColumnNo = 2 Then
                whereOrder = " ORDER BY mainsql.NoOfOpportunity " & PageColumnDirection
            ElseIf PageColumnNo = 3 Then
                whereOrder = " ORDER BY mainsql.Won " & PageColumnDirection
            ElseIf PageColumnNo = 4 Then
                whereOrder = " ORDER BY mainsql.Lost " & PageColumnDirection
            ElseIf PageColumnNo = 5 Then
                whereOrder = " ORDER BY mainsql.InProgress " & PageColumnDirection
            ElseIf PageColumnNo = 6 Then
                whereOrder = " ORDER BY mainsql.Total " & PageColumnDirection
            End If

            pSql = "SELECT * FROM  (Select mainsql.*, " & vbCrLf &
                    " ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    " SELECT DISTINCT m.ClientID " & vbCrLf &
                    ", m.ClientName " & vbCrLf &
                    ", m.CreatedBy " & vbCrLf &
                    ", (SELECT COUNT(ID) FROM TransCRM WHERE ActionName = 'Opportunity' AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ClientID = m.ClientID) as NoOfOpportunity " & vbCrLf &
                    ", (SELECT ISNULL(SUM(CONVERT(int, OpportunitySize)), 0) FROM TransCRM WHERE ActionName = 'Opportunity' AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ClientID = m.ClientID AND Status = 'In Progress') AS InProgress " & vbCrLf &
                    ", (SELECT ISNULL(SUM(CONVERT(int, OpportunitySize)), 0) FROM TransCRM WHERE ActionName = 'Opportunity' AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ClientID = m.ClientID AND Status = 'Win') AS Won " & vbCrLf &
                    ", (SELECT ISNULL(SUM(CONVERT(int, OpportunitySize)), 0) FROM TransCRM WHERE ActionName = 'Opportunity' AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ClientID = m.ClientID AND Status = 'Lost') AS Lost " & vbCrLf &
                    ", (SELECT ISNULL(SUM(CONVERT(int, OpportunitySize)), 0) FROM TransCRM WHERE ActionName = 'Opportunity' AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ClientID = m.ClientID) AS Total " & vbCrLf &
                    " FROM ( " & vbCrLf &
                    "       SELECT " & vbCrLf &
                    "       ClientID, " & vbCrLf &
                    "       CASE WHEN ISNULL(CONVERT(varchar(max), ppol.ID), '') = '' " & vbCrLf &
                    "               THEN org.CompanyName  " & vbCrLf &
                    "               ELSE ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname " & vbCrLf &
                    "       END as ClientName, " & vbCrLf &
                    "       created.FirstName + ' ' + created.MiddleName + ' ' + created.Surname as CreatedBy " & vbCrLf &
                    "       FROM TransCRM as crm " & vbCrLf &
                    "       LEFT JOIN LibraryOrganization as org ON org.ID = crm.ClientID " & vbCrLf &
                    "       LEFT JOIN LibraryPeople as ppol ON ppol.ID = crm.ClientID " & vbCrLf &
                    "       LEFT JOIN MobizLibraryPeople as created ON created.ID = crm.CreatedBy " & vbCrLf &
                    "       WHERE ActionName = 'Opportunity' " & vbCrLf &
                    "       AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 " & vbCrLf &
                    "      ) as m GROUP BY m.ClientID, m.ClientName, m.CreatedBy " & vbCrLf &
                    ") as mainsql" & whereSearch & " ) as v2 " & whereLimit

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            Return err
        End Try

    End Function

    Function GetOpportunityByClient() As Object
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        Try
            If PageSearchText <> "" Then
                Dim whereSearchContent As String = ""
                Dim operators As String = ""

                Dim arr As Object = Split(PageSearchText, " ")

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " OR "
                    End If

                    whereSearchContent = whereSearchContent & operators & " ClientName + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Unresolved) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Resolved) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Cancelled) + ' ' + " & vbCrLf &
                                    "TicketNumbers " & vbCrLf &
                                    " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next
                whereSearch = " WHERE (" & whereSearchContent & ")"
            End If

            If IsCountPage = True Then
                whereLimit = ""
            End If

            If PageColumnNo = 0 Or PageColumnNo = 1 Then
                whereOrder = " ORDER BY mainsql.OpportunityName " & PageColumnDirection
            ElseIf PageColumnNo = 2 Then
                whereOrder = " ORDER BY mainsql.OpportunityDescription " & PageColumnDirection
            ElseIf PageColumnNo = 3 Then
                whereOrder = " ORDER BY mainsql.OpportunitySize " & PageColumnDirection
            ElseIf PageColumnNo = 4 Then
                whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
            End If

            pSql = "SELECT * FROM  (Select mainsql.*, " & vbCrLf &
                    " ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    " SELECT DISTINCT ID, OpportunityName, OpportunityDescription, " & vbCrLf &
                    " OpportunitySize, OpportunitySizePeriod, OpportunityTemplate, Status, crm.LastUpdated, RelatedID as ParentOppID " & vbCrLf &
                    " FROM TransCRM as crm " & vbCrLf &
                    " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated FROM TransCRM WHERE ActionName = 'Opportunity' AND ClientID = " & ReadyString(ClientID) & " GROUP BY ID) as updated  " & vbCrLf &
                    " ON CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated " & vbCrLf &
                    " WHERE ActionName = 'Opportunity' AND ClientID = " & ReadyString(ClientID) & vbCrLf &
                    " AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 " & vbCrLf &
                    ") as mainsql" & whereSearch & " ) as v2 " & whereLimit

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT

        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            AppendToLog(ex.Message)
            Return err
        End Try
    End Function

    Function GetClientOpportunityContactPerson(ByVal PeopleID As String) As DataTable
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
                    .Expression = "ppol.MiddleName"
                End With

                With .Add("Lastname", SqlDbType.VarChar)
                    .Expression = "ppol.Surname"
                End With

                With .Add("Position", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.Position, '')"
                End With

                With .Add("PositionDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.PositionDescription, '')"
                End With

                With .Add("FullName", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.FirstName + ' ' ,'') + " & vbCrLf &
                                    "ISNULL(CASE WHEN ppol.MiddleName  = '' THEN '' ELSE ppol.MiddleName + ' ' END, '') + " & vbCrLf &
                                    "ISNULL(ppol.Surname ,'') "
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
                                "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                                "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                                "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                                "           WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                                "       FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,'') FROM LibraryPeopleContactMobile) " & vbCrLf
                End With

                With .Add("TelNos", SqlDbType.VarChar)
                    .Expression = " (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                                "       ISNULL('-' + CONVERT(varchar, ppolareacode.CallingCode) + '-',0) + " & vbCrLf &
                                "       ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) " & vbCrLf &
                                "       FROM LibraryPeopleContactTel  as ppoltel " & vbCrLf &
                                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                "               ON ppoltel.CountryID = ppolcountries.ID  " & vbCrLf &
                                "           LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                                "               ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                                "           WHERE ppoltel.PeopleID = ppol.ID " & vbCrLf &
                                "       FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,'') FROM LibraryPeopleContactMobile) " & vbCrLf
                End With
            End With

            .SqlFrom = "FROM LibraryOpportunityContactPerson as contactperson " & vbCrLf &
                        "LEFT JOIN LibraryPeople as ppol " & vbCrLf &
                        "   ON ppol.ID = contactperson.PeopleID " & vbCrLf &
                        "LEFT JOIN LibraryPeopleAddress as ppoladd " & vbCrLf &
                        "   ON ppoladd.PeopleID = contactperson.PeopleID "

            .SqlWhere = "WHERE contactperson.PeopleID = " & ReadyString(PeopleID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveOpportunity() As String
        Dim pSql As String
        Dim strOrgID As String
        Dim p As New InsertUpdate("TransCRM")
        Dim pDT As DataTable
        Dim oppID As String

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p = New InsertUpdate("TransCRM")
            p("ClientID") = ReadyString(Common.WebRequest.Data("ClientID"))
            p("OpportunityName") = ReadyString(OpportunityName)
            p("OpportunityDescription") = ReadyString(OpportunityDescription)
            p("OpportunitySize") = ReadyString(OpportunitySize)
            p("OpportunitySizePeriod") = ReadyString(OpportunitySizePeriod)
            p("OpportunityTemplate") = IIf(OpportunityTemplate = "", "NULL", ReadyString(OpportunityTemplate))
            p("ActionName") = ReadyString("Opportunity")
            p("Status") = ReadyString(OpportunityStatus)
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID())
            'p("ContactPersonID") = ReadyString(ppolID)

            If Common.WebRequest.Data("ID") = "" Then
                oppID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(oppID)
                p("LastUpdated") = ReadyString(DateTime)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                'oppID = IIf(CStr(Common.WebRequest.Data("ParentOppID")) = "", Common.WebRequest.Data("ID"), Common.WebRequest.Data("ParentOppID"))

                'If CheckIfOppHasChanged() = True Then
                '    p("RelatedID") = ReadyString(oppID)
                '    p("ID") = ReadyString(Guid.NewGuid.ToString())
                '    p("LastUpdated") = ReadyString(DateTime)
                '    pSql = p.GetInsert()
                '    Common.WebRequest.ProData.ExecuteSQL(pSql)
                'Else

                'End If
                p("LastUpdated") = ReadyString(DateTime)
                oppID = Common.WebRequest.Data("ID")
                pSql = p.GetUpdate(" ID = " & ReadyString(oppID.ToString()))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If OpportunityTemplate <> "" Then
                pSql = "DELETE FROM LibraryCRMStagingChecklist WHERE FunnelTemplateID = " & ReadyString(OpportunityTemplate) &
                        " AND OpportunityID = " & ReadyString(oppID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                For Each checklist As Object In OpportunityChecklist
                    p = New InsertUpdate("LibraryCRMStagingChecklist")
                    p("ClientID") = ReadyString(Common.WebRequest.Data("ClientID"))
                    p("FunnelTemplateID") = ReadyString(OpportunityTemplate)
                    p("LastUpdated") = ReadyString(Format(DateTime, "MM-dd-yyyy hh:mm:ss tt"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID())
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("OpportunityID") = ReadyString(oppID)
                    p("DescriptionID") = ReadyString(checklist("SpecialID"))
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next checklist
            End If

            For Each cpeople As Object In ObjContactPerson
                Dim ppolID As String = Guid.NewGuid.ToString()
                p = New InsertUpdate("LibraryPeople")

                p("FirstName") = ReadyString(CStr2(cpeople("Firstname").ToString()))
                p("MiddleName") = ReadyString(CStr2(cpeople("Middlename").ToString()))
                p("Surname") = ReadyString(CStr2(cpeople("Lastname").ToString()))
                p("Position") = ReadyString(CStr2(cpeople("Position").ToString()))
                p("PositionDescription") = ReadyString(CStr2(cpeople("PositionDescription").ToString()))
                p("IsFromOrg") = ReadyString(1)

                If cpeople("ContactPersonState").ToString() = "Add" Then
                    p("ID") = ReadyString(ppolID)
                    pSql = p.GetInsert()

                ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                    ppolID = cpeople("PeopleID")
                    pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("PeopleID")))

                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                p = New InsertUpdate("LibraryPeopleAddress")

                p("PeopleID") = ReadyString(ppolID)
                p("HouseBuildingNo") = If(cpeople("HouseBuildingNo") = Nothing, "NULL", ReadyString(cpeople("HouseBuildingNo").ToString()))
                p("Street") = If(cpeople("Street") = Nothing, "NULL", ReadyString(cpeople("Street").ToString()))
                p("CityID") = If(cpeople("City") = "", "NULL", ReadyString(cpeople("City").ToString()))
                p("CountryID") = If(cpeople("Country") = "", "NULL", ReadyString(cpeople("Country").ToString()))
                p("AddressType") = If(cpeople("TypeAddress") = Nothing, "NULL", ReadyString(cpeople("TypeAddress").ToString()))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If cpeople("AddressID") = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("AddressID")))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                p = New InsertUpdate("LibraryOpportunityContactPerson")

                p("OpportunityID") = ReadyString(oppID)
                p("PeopleID") = ReadyString(ppolID)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If cpeople("ContactPersonState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf cpeople("ContactPersonState").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOpportunityContactPerson WHERE ID = " & ReadyString(cpeople("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                Try
                    For Each tel As Object In cpeople("Tel")
                        p = New InsertUpdate("LibraryPeopleContactTel")

                        p("PeopleID") = ReadyString(ppolID)
                        p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                        p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                        p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                        p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                        p("ContactType") = ReadyString(tel("ContactType").ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                    Next tel
                Catch ex As Exception

                End Try

                Try
                    For Each mobile As Object In cpeople("Mobile")
                        p = New InsertUpdate("LibraryPeopleContactMobile")

                        p("PeopleID") = ReadyString(ppolID)
                        p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                        p("TelCoPrefixID") = ReadyString(mobile("TelCodeID").ToString())
                        p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                    Next mobile
                Catch ex As Exception

                End Try

                Try
                    For Each email As Object In cpeople("Email")
                        p = New InsertUpdate("LibraryPeopleContactEmail")

                        p("PeopleID") = ReadyString(ppolID)
                        p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("EmailType") = ReadyString(email("EmailType").ToString())
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If email("EmailState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf email("EmailState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                        ElseIf email("EmailState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Next email
                Catch ex As Exception

                End Try
            Next cpeople

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Function CheckIfOppHasChanged() As Boolean
        Dim pDt As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM TransCRM WHERE ID = " & ReadyString(Common.WebRequest.Data("ID")))
        Dim rows As Object = pDt.Rows(0)

        If CStr2(rows("OpportunityName")) = Trim(OpportunityName) Or
            CStr2(rows("OpportunityDescription")) = Trim(OpportunityDescription) Or
            CStr2(rows("OpportunitySize")) = Trim(OpportunitySize) Or
            CStr2(rows("OpportunitySizePeriod")) = Trim(OpportunitySizePeriod) Or
            CStr2(rows("OpportunityTemplate")) = Trim(OpportunityTemplate) Or
            CStr2(rows("Status")) = Trim(OpportunityStatus) Then
            Return True
        Else
            Return False
        End If
    End Function

    Function GetClientTicketSummary() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim dborgid, dbname As String

#If DEBUG Then
        'MobizABC dev db
        dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
        dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
        '1Mobiz prod db 
        dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
        dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If

        Try
            If PageSearchText <> "" Then
                Dim whereSearchContent As String = ""
                Dim operators As String = ""

                Dim arr As Object = Split(PageSearchText, " ")

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " OR "
                    End If

                    whereSearchContent = whereSearchContent & operators & " ClientName + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Unresolved) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Resolved) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Cancelled) + ' ' + " & vbCrLf &
                                    "TicketNumbers " & vbCrLf &
                                    " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next
                whereSearch = " WHERE (" & whereSearchContent & ")"
            End If

            If IsCountPage = True Then
                whereLimit = ""
            End If

            If PageColumnNo = 0 Or PageColumnNo = 1 Then
                whereOrder = " ORDER BY ViewTicketFromMobiz DESC, mainsql.ClientName " & PageColumnDirection
            ElseIf PageColumnNo = 2 Then
                whereOrder = " ORDER BY ViewTicketFromMobiz DESC, mainsql.Unresolved " & PageColumnDirection
            ElseIf PageColumnNo = 3 Then
                whereOrder = " ORDER BY ViewTicketFromMobiz DESC, mainsql.Resolved " & PageColumnDirection
            ElseIf PageColumnNo = 4 Then
                whereOrder = " ORDER BY ViewTicketFromMobiz DESC, mainsql.Cancelled " & PageColumnDirection
            End If

            pSql = " WITH mobizcrm AS (SELECT mobizcrm.Status " & vbCrLf &
                    " FROM " & dbname & ".dbo.TransCRM as mobizcrm " & vbCrLf &
                    " OUTER APPLY ( " & vbCrLf &
                    "   SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM " & dbname & ".dbo.TransCRM WHERE ClientID = " & ReadyString(WebRequest.Instance.OrgID) & "	AND ActionName = 'Ticket' AND ISNULL(IsDelete, 0) = 0 GROUP BY TicketNumber " & vbCrLf &
                    " ) as rec " & vbCrLf &
                    " WHERE rec.LastUpdated = mobizcrm.LastUpdated AND rec.TicketNumber = mobizcrm.TicketNumber AND ClientID = " & ReadyString(WebRequest.Instance.OrgID) & " AND ISNULL(mobizcrm.IsDelete, 0) = 0 " & vbCrLf &
                    ") " & vbCrLf &
                    "SELECT * FROM  (Select mainsql.ID, mainsql.ClientName, " & vbCrLf &
                    " mainsql.Unresolved as Unresolved, " & vbCrLf &
                    " mainsql.Resolved, mainsql.Cancelled, mainsql.Telephone, mainsql.ViewTicketFromMobiz, " & vbCrLf &
                    " ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    " SELECT DISTINCT ClientID as ID " & vbCrLf &
                    ", (ISNULL(org.CompanyName, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname)) as ClientName " & vbCrLf &
                    ", (SELECT COUNT(crm2.TicketNumber) From TransCRM as crm2 " & vbCrLf &
                    "   INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated " & vbCrLf &
                    "   ON crm2.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm2.LastUpdated) = updated.LastUpdated " & vbCrLf &
                    "   WHERE crm2.ClientID = crm.ClientID AND Status = 'Unresolved' AND CONVERT(bit, ISNULL(crm2.IsDelete, 0)) <> 1 ) as Unresolved " & vbCrLf &
                    ", (SELECT COUNT(crm2.TicketNumber) From TransCRM as crm2 " & vbCrLf &
                    "   INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated " & vbCrLf &
                    "   ON crm2.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm2.LastUpdated) = updated.LastUpdated " & vbCrLf &
                    "   WHERE crm2.ClientID = crm.ClientID AND Status = 'Resolved' AND CONVERT(bit, ISNULL(crm2.IsDelete, 0)) <> 1 ) as Resolved " & vbCrLf &
                    ", (SELECT COUNT(crm2.TicketNumber) From TransCRM as crm2 " & vbCrLf &
                    "   INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated " & vbCrLf &
                    "   ON crm2.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm2.LastUpdated) = updated.LastUpdated " & vbCrLf &
                    "   WHERE crm2.ClientID = crm.ClientID AND Status = 'Cancelled' AND CONVERT(bit, ISNULL(crm2.IsDelete, 0)) <> 1 ) as Cancelled " & vbCrLf &
                    ", (SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ CONVERT(varchar, TicketNumber) " & vbCrLf &
                    "       FROM TransCRM " & vbCrLf &
                    "       WHERE ClientID = crm.ClientID " & vbCrLf &
                    "       AND ActionName = 'Ticket' " & vbCrLf &
                    "   FOR XML PATH('')), 1, 1,'') FROM TransCRM) as TicketNumbers " & vbCrLf &
                    ", CASE WHEN ISNULL(CONVERT(varchar(max), org.ID), 'people') = 'people' " & vbCrLf &
                    "       THEN " & vbCrLf &
                    "   (SELECT STUFF(( " & vbCrLf &
                    "       SELECT * FROM( " & vbCrLf &
                    "           SELECT ' ' + -- ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode) + '',0) + " & vbCrLf &
                    "               ISNULL('(' + CONVERT(varchar, ppolareacode.CallingCode) + ') ',0) + " & vbCrLf &
                    "               ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) + " & vbCrLf &
                    "               ISNULL(CASE WHEN ppoltel.ExtensionNo = '' THEN '' ELSE ',' + ppoltel.ExtensionNo END,0) as b " & vbCrLf &
                    "               FROM LibraryPeopleContactTel as ppoltel " & vbCrLf &
                    "                   LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "                       ON ppoltel.CountryID = ppolcountries.ID " & vbCrLf &
                    "                   LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                    "                       ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                    "               WHERE ppoltel.PeopleID = ppol.ID " & vbCrLf &
                    "           UNION ALL " & vbCrLf &
                    "           SELECT ' ' +  ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                    "               ISNULL(CONVERT(varchar, ppoltelco.Prefix),0) + " & vbCrLf &
                    "               ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) as b " & vbCrLf &
                    "               FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                    "               LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "                   ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                    "               LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                    "                   ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                    "               WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                    "       ) as unions" & vbCrLf &
                    "   FOR XML PATH('') " & vbCrLf &
                    "), 1, 0, '')) " & vbCrLf &
                    "       ELSE " & vbCrLf &
                    "   (SELECT STUFF(( " & vbCrLf &
                    "       SELECT * FROM( " & vbCrLf &
                    "           SELECT ' ' + -- ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode) + '',0) + " & vbCrLf &
                    "               ISNULL('(' + CONVERT(varchar, orgareacode.CallingCode) + ')',0) + " & vbCrLf &
                    "               ISNULL(CONVERT(varchar, orgtel.ContactNo),0) + " & vbCrLf &
                    "               ISNULL(CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE ',' + orgtel.ExtensionNo END,0) as b " & vbCrLf &
                    "               FROM LibraryOrganizationContactTel as orgtel " & vbCrLf &
                    "                   LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                    "                       ON orgtel.CountryID = orgcountries.ID " & vbCrLf &
                    "                   LEFT JOIN MobizLibraryAreaCodes as orgareacode " & vbCrLf &
                    "                       ON orgtel.AreaCodeID = orgareacode.ID " & vbCrLf &
                    "                   WHERE orgtel.OrganizationID = org.ID " & vbCrLf &
                    "           UNION ALL " & vbCrLf &
                    "           SELECT ' ' +  ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                    "               ISNULL(CONVERT(varchar, orgtelco.Prefix),0) + " & vbCrLf &
                    "               ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) as b " & vbCrLf &
                    "               FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                    "                   LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                    "                       ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                    "                   LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                    "                       ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                    "                   WHERE orgmobile.OrganizationID = org.ID " & vbCrLf &
                    "       ) as unions" & vbCrLf &
                    "   FOR XML PATH('') " & vbCrLf &
                    "), 1, 0, '')) " & vbCrLf &
                    "   END as Telephone, " & vbCrLf &
                    " 0 as ViewTicketFromMobiz " & vbCrLf &
                    " FROM TransCRM as crm " & vbCrLf &
                    " LEFT JOIN LibraryOrganization as org ON org.ID = crm.ClientID " & vbCrLf &
                    " LEFT JOIN LibraryPeople as ppol ON ppol.ID = crm.ClientID " & vbCrLf &
                    " WHERE ActionName = 'Ticket' " & vbCrLf &
                    " UNION ALL " & vbCrLf &
                    " SELECT TOP 1 " & ReadyString(WebRequest.Instance.OrgID) & " as ID, " & vbCrLf &
                    " '*** Tickets Report for Mobiz ***' as ClientName, " & vbCrLf &
                    " (SELECT COUNT(Status) FROM mobizcrm WHERE Status = 'Unresolved') as Unresolved, " & vbCrLf &
                    " (SELECT COUNT(Status) FROM mobizcrm WHERE Status = 'Resolved') as Resolved, " & vbCrLf &
                    " (SELECT COUNT(Status) FROM mobizcrm WHERE Status = 'Cancelled') as Cancelled, " & vbCrLf &
                    " '' as TicketNumbers, " & vbCrLf &
                    " '' as Telephone, " & vbCrLf &
                    " 1 as ViewTicketFromMobiz " & vbCrLf &
                    " FROM mobizcrm " & vbCrLf &
                    ") as mainsql" & whereSearch & " ) as v2 " & whereLimit

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            Return err
        End Try

    End Function

    Function SaveTicketComment() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("TransCRM")
        Dim commentID As String = Guid.NewGuid.ToString()

        Try
            p("ClientID") = ReadyString(Common.WebRequest.Data("ClientID"))
            p("RelatedID") = ReadyString(Common.WebRequest.Data("TicketID"))
            p("Notes") = ReadyString(Common.WebRequest.Data("Comment"))
            p("ActionName") = ReadyString("Comment")
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(Common.WebRequest.Data("Date")))

            Common.WebRequest.ProData.BeginTransaction()

            p("ID") = ReadyString(commentID)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return commentID
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function SaveTicket(ByVal data As Object) As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("TransCRM")
        Dim ticketID As String = Guid.NewGuid.ToString()
        Dim ticketnumber As String
        Dim TicketOpenDate As String
        Dim dborgid, dbname As String

        If CBool2(data("ViewTicketFromMobiz")) And CBool2(data("DelegatedByMobiz")) Then
#If DEBUG Then
            'MobizABC dev db
            dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
            dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                '1Mobiz prod db 
                dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If
            Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))
        End If

        Try
            p("ClientID") = ReadyString(data("ClientID"))
            p("TicketType") = ReadyString(data("Type"))
            p("OtherTicketType") = ReadyString(data("OtherType"))
            p("ProductServices") = ReadyString(data("ProductServices"))
            p("Status") = ReadyString(data("Status"))
            p("ContactPersonName") = ReadyString(data("ContactPerson"))
            p("ContactPersonID") = IIf(data("ContactPersonID") = "", "NULL", ReadyString(data("ContactPerson")))
            p("AddressToService") = ReadyString(data("Address"))
            p("TimeFrom") = IIf(IsDate(data("TimeFrom")), ReadyString(data("TimeFrom")), "NULL")
            p("TimeTo") = IIf(IsDate(data("TimeTo")), ReadyString(data("TimeTo")), "NULL")
            p("Notes") = ReadyString(data("Notes"))
            p("ActionName") = ReadyString("Ticket")
            p("IssueAssignedTo") = IIf(CStr2(data("AssignedID")) = "", "NULL", ReadyString(data("AssignedID")))
            p("IssueAssignedToName") = IIf(CStr2(data("Assigned")) = "Search Employee", "NULL", ReadyString(data("Assigned")))
            p("TicketDueDate") = IIf(CStr2(data("DueDate")) = "Set Date", "NULL", ReadyString(data("DueDate")))
            p("RecommendedAction") = ReadyString(data("RecommendedAction"))
            p("OtherRecommendedAction") = ReadyString(data("OtherRecommendedAction"))
            p("DaysOfService") = ReadyString(data("Days"))
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(data("Date")))
            p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("TicketStatusConfirmedBy") = IIf(CStr2(data("StatusConfirmBy")) = "", "NULL", ReadyString(data("StatusConfirmBy")))

            If Not String.IsNullOrWhiteSpace(data("Fowst")) Then
                If IsGuid(data("Fowst")) Then
                    p("FowstID") = ReadyString(data("Fowst"))
                Else
                    p("CustomFowst") = ReadyString(data("Fowst"))
                End If
            End If

            Common.WebRequest.ProData.BeginTransaction()

            If CStr2(data("ID")) = "" Then
                GenNumberType = "Ticket"
                ticketnumber = GenerateNumber()
                TicketOpenDate = ReadyString(CDate(data("Date")))
                p("TicketNumber") = CInt2(ticketnumber) ' CInt2(data("No"))
                p("TicketOpenDate") = TicketOpenDate
                p("OriginalCreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            Else
                Dim pDTHistory As DataTable = Common.WebRequest.ProData.Execute("SELECT TicketNumber, RelatedID, OriginalCreatedBy, TicketOpenDate FROM TransCRM WHERE ID = " & ReadyString(data("ID")))
                ticketnumber = pDTHistory.Rows(0)("TicketNumber")
                TicketOpenDate = ReadyString(pDTHistory.Rows(0)("TicketOpenDate"))
                p("TicketNumber") = CInt2(ticketnumber)
                p("OriginalCreatedBy") = IIf(CStr2(pDTHistory.Rows(0)("OriginalCreatedBy")) = "", "NULL", ReadyString(CStr2(pDTHistory.Rows(0)("OriginalCreatedBy"))))
                p("TicketOpenDate") = TicketOpenDate
                p("RelatedID") = IIf(CStr2(pDTHistory.Rows(0)("RelatedID")) = "", ReadyString(data("ID")), ReadyString(pDTHistory.Rows(0)("RelatedID")))

                If CheckIfTicketHasChanged(data("ID"), data) Then
                    If Not data("DelegateToClient") Is Nothing Then
                        SaveToTransMobizIssues(data, ticketnumber, TicketOpenDate)
                        Common.WebRequest.ProData.CommitTransaction()
                    End If
                    Return "same"
                End If
            End If

            p("ID") = ReadyString(ticketID)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If String.IsNullOrWhiteSpace(data("ID")) Then
                data("ID") = ticketID
            End If

            If CBool2(data("DelegateToClient")) Then
                SaveToTransMobizIssues(data, ticketnumber, TicketOpenDate)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return ticketID & "||" & ticketnumber
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function CheckIfTicketHasChanged(ByVal ID As String, ByVal data As Object) As String
        Dim pDt As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM TransCRM WHERE ID = " & ReadyString(ID))
        Dim rows As Object = pDt.Rows(0)

        If CStr2(rows("TicketType")) = data("Type") And
            CStr2(rows("OtherTicketType")) = data("OtherType") And
            CStr2(rows("ProductServices")) = data("ProductServices") And
            CStr2(rows("Status")) = data("Status") And
            CStr2(rows("ContactPersonName")) = data("ContactPerson") And
            CStr2(rows("AddressToService")) = data("Address") And
            CDateNull("01-01-2016 " & rows("TimeFrom").ToString()) = CDateNull("01-01-2016 " & data("TimeFrom").ToString()) And
            CDateNull("01-01-2016 " & rows("TimeTo").ToString()) = CDateNull("01-01-2016 " & data("TimeTo").ToString()) And
            CStr2(rows("Notes")) = data("Notes") And
            CStr2(rows("IssueAssignedToName")) = data("Assigned") And
            CDateNull(rows("TicketDueDate")) = CDateNull(data("DueDate")) And
            CStr2(rows("DaysOfService")) = data("Days") And
            CStr2(rows("RecommendedAction")) = data("RecommendedAction") And
            CStr2(rows("OtherRecommendedAction")) = data("OtherRecommendedAction") And
            (CStr2(rows("FowstID")) = data("Fowst") Or
            CStr2(rows("CustomFowst")) = data("Fowst")) Then
            Return True
        Else
            Return False
        End If
    End Function

    Function SaveToTransMobizIssues(ByVal data As Object, ByVal ticketnumber As String, ByVal datereported As String)
        Dim p As New InsertUpdate("MobizTransMobizIssues")

        p("OrgID") = ReadyString(data("ClientID"))
        p("TicketNo") = ReadyString(ticketnumber)
        p("Product") = ReadyString(data("ProductServices"))
        p("Subject") = ReadyString(data("Type"))
        p("Urgency") = ReadyString("N/A")
        p("Importance") = ReadyString("N/A")
        p("Description") = ReadyString(data("Notes"))
        p("Status") = ReadyString(data("Status"))
        p("LastUpdated") = "getdate()"
        p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
        p("DelegateToClient") = CInt2(data("DelegateToClient"))

        If Not String.IsNullOrWhiteSpace(data("Fowst")) Then
            If IsGuid(data("Fowst")) Then
                p("FowstID") = ReadyString(data("Fowst"))
                p("CustomFowst") = "NULL"
            Else
                p("FowstID") = "NULL"
                p("CustomFowst") = ReadyString(data("Fowst"))
            End If
        End If

        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" TicketNo =  " & ReadyString(ticketnumber) & " AND OrgID = " & ReadyString(data("ClientID")))) = 0 Then
            p("ReportedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("DateReported") = datereported
            p("ID") = ReadyString(data("ID"))
            Common.WebRequest.ProData.Execute(p.GetInsert())
        End If

        Return True
    End Function

    Function GenerateNumber() As String
        Dim tableName As String
        Dim columnName As String

        If GenNumberType = "Ticket" Then
            tableName = "TransCRM"
            columnName = "TicketNumber"
        ElseIf GenNumberType = "Todo" Then
            tableName = "TransCRM"
            columnName = "TodoNumber"
        Else
            tableName = "LibraryCRMRMA"
            columnName = "RMANumber"
        End If
        Dim num As Integer = New System.Random().Next(1, 999999)
        Dim availableNum As Object = Common.WebRequest.ProData.Execute("SELECT " & columnName & " FROM " & tableName & " WHERE " & columnName & " = " & num).Rows

        If availableNum.Count > 0 Then
            num = GenerateNumber()
        End If

        Return Format(num, "000000")
    End Function

    Function RemoveGroupStage() As String
        Dim pSql As String = "DELETE FROM LibraryCRMStageGroup WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
        Common.WebRequest.ProData.Execute(pSql)

        pSql = "DELETE FROM LibraryCRMStage WHERE GroupID = " & ReadyString(Common.WebRequest.Data("ID"))
        Common.WebRequest.ProData.Execute(pSql)

        Return "deleted"
    End Function

    Function GetStage() As DataTable
        Dim pSql As String = "SELECT Stage.* FROM LibraryCRMStage as Stage " & vbCrLf &
                "WHERE GroupID = " & ReadyString(Common.WebRequest.Data("ID"))

        Return Common.WebRequest.ProData.Execute(pSql)

    End Function

    Function GetGroupStage() As DataTable
        Dim pSql As String = "SELECT Stage.*, ISNULL(ppol.FirstName, '') + ' ' + ISNULL(ppol.Surname, '') as CreatedBy FROM LibraryCRMStageGroup as Stage " & vbCrLf &
                "INNER JOIN MobizLibraryPeople as ppol ON Stage.UserID = ppol.ID "

        Return Common.WebRequest.ProData.Execute(pSql)

    End Function

    Function SaveStage() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("LibraryCRMStageGroup")
        Try
            Common.WebRequest.ProData.BeginTransaction()
            p("Name") = ReadyString(GroupStageName)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(TodayDate))

            If GroupStageID = "" Then
                GroupStageID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(GroupStageID)
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(GroupStageID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("LibraryCRMStage")

            For Each stages As Object In Stage
                p("GroupID") = ReadyString(GroupStageID)
                p("StageName") = ReadyString(stages("Stage"))
                p("Description") = ReadyString(stages("Description"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(TodayDate))

                If stages("State") = "new" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                ElseIf stages("State") = "remove" Then
                    pSql = "DELETE FROM LibraryCRMStage WHERE ID = " & ReadyString(Trim(stages("ID")))
                Else
                    pSql = p.GetUpdate("ID = " & ReadyString(Trim(stages("ID"))))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Function GetActivityReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransCRMCachedActivity.ID"
                End With

                With .Add("CallActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.Call" & vbCrLf
                End With

                With .Add("NoteActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.Notes" & vbCrLf
                End With

                With .Add("EmailActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.Email" & vbCrLf
                End With

                With .Add("SmsActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.SMS" & vbCrLf
                End With

                With .Add("SchedPhysicalActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.ScheduledPhysicalMeeting" & vbCrLf
                End With

                With .Add("SchedVirtualActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.ScheduledVirtualMeeting" & vbCrLf
                End With

                With .Add("SchedEmailActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.ScheduledEmail" & vbCrLf
                End With

                With .Add("SchedCallActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.ScheduledCall" & vbCrLf
                End With

                With .Add("SchedSmsActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.ScheduledSMS" & vbCrLf
                End With

                With .Add("PhysicalAttendedActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.PhysicalMeetingAttended" & vbCrLf
                End With

                With .Add("VirtualAttendedActual", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.VirtualMeetingAttended" & vbCrLf
                End With

                With .Add("CallTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.Call" & vbCrLf
                End With

                With .Add("NoteTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.Notes" & vbCrLf
                End With

                With .Add("EmailTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.Email" & vbCrLf
                End With

                With .Add("SmsTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.SMS" & vbCrLf
                End With

                With .Add("SchedPhysicalTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.ScheduledPhysicalMeeting" & vbCrLf
                End With

                With .Add("SchedVirtualTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.ScheduledVirtualMeeting" & vbCrLf
                End With

                With .Add("SchedEmailTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.ScheduledEmail" & vbCrLf
                End With

                With .Add("SchedCallTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.ScheduledCall" & vbCrLf
                End With

                With .Add("SchedSmsTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.ScheduledSMS" & vbCrLf
                End With

                With .Add("PhysicalAttendedTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.PhysicalMeetingAttended" & vbCrLf
                End With

                With .Add("VirtualAttendedTarget", SqlDbType.VarChar)
                    .Expression = "LibraryCRMActivityQuota.VirtualMeetingAttended" & vbCrLf
                End With

                With .Add("QuotaID", SqlDbType.VarChar)
                    .Expression = "TransCRMCachedActivity.ActivityQuotaID" & vbCrLf
                End With

                With .Add("HasQuota", SqlDbType.VarChar)
                    .Expression = "CASE WHEN ISNULL(CONVERT(varchar(MAX), TransCRMCachedActivity.ActivityQuotaID), '') = '' THEN 'False' ELSE 'True' END" & vbCrLf
                End With
            End With

            .SqlFrom = "FROM TransCRMCachedActivity " & vbCrLf &
                        "LEFT JOIN LibraryCRMActivityQuota ON LibraryCRMActivityQuota.ID = TransCRMCachedActivity.ActivityQuotaID " & vbCrLf

            .SqlWhere = "WHERE ClientID = " & ReadyString(User) & vbCrLf &
                        " AND (CONVERT(date, DateToday) BETWEEN " & ReadyString(DateRange.Split("-")(0)) & " AND " & ReadyString(DateRange.Split("-")(1)) & ")"

            pDT = .Requery

        End With

        Return pDT

        'Return Json(New With {Key .Report = GetJson(pDT)}, JsonRequestBehavior.AllowGet)
    End Function

    Function GetAvailableTeamDatePeriod() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * " & vbCrLf &
                "FROM LibraryCRMActivityQuota " & vbCrLf &
                "WHERE TeamID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function SaveActivityQuota() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("LibraryCRMActivityQuota")
        Dim rowno As Integer

        Try
            Common.WebRequest.ProData.BeginTransaction()

            pSql = "SELECT TOP 1 RowNo FROM LibraryCRMActivityQuota ORDER BY RowNo DESC"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count = 0 Then
                rowno = 1
            Else
                rowno = CInt(pDT.Rows(0)("RowNo")) + 1
            End If

            p("Call") = ReadyString(Calls)
            p("Notes") = ReadyString(Notes)
            p("SMS") = ReadyString(Sms)
            p("Email") = ReadyString(Email)
            p("ScheduledPhysicalMeeting") = ReadyString(SchedulePhysical)
            p("ScheduledVirtualMeeting") = ReadyString(ScheduleVirtual)
            p("ScheduledEmail") = ReadyString(ScheduleEmail)
            p("ScheduledCall") = ReadyString(ScheduleCall)
            p("ScheduledSMS") = ReadyString(ScheduleSms)
            p("PhysicalMeetingAttended") = ReadyString(PhysicalAttended)
            p("VirtualMeetingAttended") = ReadyString(VirtualAttended)
            p("DateFrom") = ReadyString(CDate(DatePeriod("From")))
            p("DateTo") = ReadyString(CDate(DatePeriod("To")))
            p("TeamID") = ReadyString(TeamID)
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(TodayDate))

            If Common.WebRequest.Data("QuotaID") = "" Then
                p("RowNo") = ReadyString(rowno)
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("QuotaID")))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveTicketNotes() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("LibraryCRMTicketNotes")
        Dim rowno As Integer

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("Notes") = ReadyString(Notes)
            p("IsDefault") = ReadyString(Common.WebRequest.Data("IsDefault"))
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(TodayDate))

            If Common.WebRequest.Data("noteID") = "" Then
                If CBool2(Common.WebRequest.Data("IsDefault")) = True Then
                    Common.WebRequest.ProData.ExecuteSQL("UPDATE LibraryCRMTicketNotes SET IsDefault = 0")
                End If
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("noteID")))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function CheckActivityOverlapDate() As Object
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * FROM LibraryCRMActivityQuota"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If Common.WebRequest.Data("QuotaID") = "" Then
            For Each Dates As Object In pDT.Rows
                Dim dfrom As Date = CDate(Dates("DateFrom"))
                Dim dto As Date = CDate(Dates("DateTo"))

                If (dfrom <= CDate(DatePeriod("To"))) And (dto >= CDate(DatePeriod("From"))) Then
                    'Check to see if there is overlap date range
                    Return Json(New With {Key .IsOverlap = True,
                                          .RowNum = Dates("RowNo"),
                                          .From = Dates("DateFrom").ToString(),
                                          .To = Dates("DateTo").ToString()
                                         },
                            JsonRequestBehavior.AllowGet)
                End If
            Next
        End If

        Return Json(New With {Key .IsOverlap = False}, JsonRequestBehavior.AllowGet)
    End Function

    Function GetActivityQuota() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT LibraryCRMActivityQuota.*, " & vbCrLf &
                "ISNULL(MobizLibraryPeople.FirstName, '' ) + ' ' + ISNULL(MobizLibraryPeople.Surname, '' ) as CreatedByName, " & vbCrLf &
                "LibraryCRMTeams.TeamName " & vbCrLf &
                "FROM LibraryCRMActivityQuota " & vbCrLf &
                "INNER JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryCRMActivityQuota.CreatedBy " & vbCrLf &
                "INNER JOIN LibraryCRMTeams ON LibraryCRMTeams.ID = LibraryCRMActivityQuota.TeamID "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetActivityQuotaData() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * " & vbCrLf &
                "FROM LibraryCRMActivityQuota " & vbCrLf &
                "WHERE ID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetTicketNoteData() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * " & vbCrLf &
                "FROM LibraryCRMTicketNotes " & vbCrLf &
                "WHERE ID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetTicketNotes() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT notes.*, ppol.FirstName + ' ' + ppol.Surname as ModifiedBy " & vbCrLf &
                "FROM LibraryCRMTicketNotes notes " & vbCrLf &
                "INNER JOIN MobizLibraryPeople as ppol ON notes.UserID = ppol.ID " & vbCrLf

        If Common.WebRequest.Data("SearchKeyword") <> "" Then
            pSql = pSql & " WHERE Notes LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKeyword"))
        End If

        pSql = pSql & " ORDER BY notes.LastUpdated "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GenerateActivityReport() As DataTable
        Dim pSql As String

        pSql = "SELECT " & vbCrLf &
                "'Call' as Module, " & vbCrLf &
                "'' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE ActionName = 'Call' " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        pSql = pSql & "UNION " & vbCrLf &
                "SELECT " & vbCrLf &
                "'SMS' as Module, " & vbCrLf &
                "'' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE (ActionName = 'SMS (Sent)' OR ActionName = 'SMS (Pending)') " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        pSql = pSql & "UNION " & vbCrLf &
                "SELECT " & vbCrLf &
                "'Note' as Module, " & vbCrLf &
                "'' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE ActionName = 'Take-A-Note' " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        pSql = pSql & "UNION " & vbCrLf &
                "SELECT " & vbCrLf &
                "'Schedule' as Module, " & vbCrLf &
                "'Call' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE (ActionName = 'Scheduled Action' AND SchedActionName = 'Call') " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        pSql = pSql & "UNION " & vbCrLf &
                "SELECT " & vbCrLf &
                "'Schedule' as Module, " & vbCrLf &
                "'Meeting' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE (ActionName = 'Scheduled Action' AND SchedActionName = 'Meeting') " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        pSql = pSql & "UNION " & vbCrLf &
                "SELECT " & vbCrLf &
                "'Schedule' as Module, " & vbCrLf &
                "'Email' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE (ActionName = 'Scheduled Action' AND SchedActionName = 'Email') " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        pSql = pSql & "UNION " & vbCrLf &
                "SELECT " & vbCrLf &
                "'Schedule' as Module, " & vbCrLf &
                "'SMS' as Action, " & vbCrLf &
                "COUNT(*) as Total " & vbCrLf &
                "FROM TransCRM " & vbCrLf &
                "WHERE (ActionName = 'Scheduled Action' AND SchedActionName = 'SMS') " & vbCrLf &
                "AND CONVERT(date, LastUpdated) BETWEEN '2016-01-01' AND '2016-04-15' " & vbCrLf
        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetCountryCode() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow
            .SearchString = Search

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "Code"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "Name"
                End With
                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "CallingCode"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryCountries"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function CacheBusinessIndustry() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ind.ID"
                End With

                With .Add("Class", SqlDbType.VarChar)
                    .Expression = "Class" & vbCrLf
                End With
            End With

            .SqlFrom = "FROM LibraryOrganization as org " & vbCrLf &
                        " INNER JOIN MobizLibraryIndustryClass as ind ON ind.ID = org.IndustryClassID "

            .SqlOrder = "ORDER BY Class ASC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function CheckLastCacheDate() As DataTable
        Dim pSql As String = "SELECT MAX(convert(date, SalesDate)) as Date FROM TransCustomerTransactionSummaryCache"

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function SaveCustomerInfoFromDataSource(ByVal pJson As String) As String
        Dim pSql As String
        Dim pFrown As Frown
        Dim pDT As DataTable

        pFrown = JsonConvert.DeserializeObject(Of Frown)(pJson)

        If Not pFrown.d Is Nothing Then
            For Each pRow As Frown.dClass.ResultClass.RowsClass In pFrown.d.Results.Rows
                pSql = "SELECT ID FROM TransCustomerTransactionSummaryCache WHERE ID = " & ReadyString(pRow.ID)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count = 0 Then
                    Dim pIU As New InsertUpdate("TransCustomerTransactionSummaryCache")
                    pIU("ID") = ReadyString(pRow.ID)
                    pIU("CustomerID") = ReadyString(pRow.CustomerID)
                    pIU("FOWSTID") = ReadyString(pRow.FOWSTID)
                    pIU("SalesDate") = ReadyString(pRow.SalesDate)
                    pIU("GrossSales") = pRow.GrossSales
                    pIU("Discount") = pRow.Discount
                    pIU("SalesReturns") = pRow.SalesReturns
                    pIU("NetSales") = pRow.GrossSales - pRow.Discount - pRow.SalesReturns
                    pIU("COGSGross") = pRow.COGSGross
                    pIU("COGSNet") = pRow.COGSNet

                    pSql = pIU.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next
        End If

    End Function

    Function GetAllClientList() As DataTable
        Dim keyword As String = Common.WebRequest.Data("keywords")
        Dim prospectType As String = Common.WebRequest.Data("Type")
        Dim pSql As String = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER ( ORDER BY ClientName asc) AS RowNum From (SELECT * FROM (SELECT org.ID as ID, " & vbCrLf &
                            "   LTRIM(org.CompanyName) as ClientName, " & vbCrLf &
                            "   'Org' as ProspectType " & vbCrLf &
                            "FROM LibraryOrganization as org) as oTable " & vbCrLf &
                            "UNION ALL " & vbCrLf &
                            "SELECT * FROM (SELECT ppol.ID as ID," & vbCrLf &
                            "   LTRIM(ISNULL(FirstName + ' ', '') + ISNULL(CASE WHEN MiddleName = '' THEN '' ELSE MiddleName + ' ' END, '') + ISNULL(Surname, '')) as ClientName, " & vbCrLf &
                            "   'People' as ProspectType " & vbCrLf &
                            "   FROM LibraryPeople as ppol " & vbCrLf &
                            "   WHERE IsFromOrg = 0) as pTable" & vbCrLf &
                            ") as V1 ) as v2 " & vbCrLf

        If keyword <> Nothing Then
            pSql += " WHERE ClientName LIKE " & ReadyStringWild(keyword)

            If prospectType <> Nothing Then
                pSql += " AND ProspectType = " & ReadyString(prospectType)
            End If
        Else
            If prospectType <> Nothing Then
                pSql += " WHERE ProspectType = " & ReadyString(prospectType)
            End If
        End If

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetDashboardRequiredData() As Object
        Dim jss = New JavaScriptSerializer()

        Dim objToDo As Object = jss.Deserialize(Of Object)(GetJson(GetOpenToDo()))
        Dim objSchedule As Object = jss.Deserialize(Of Object)(GetJson(GetOpenSchedule()))

        Return Json(New With {Key .ToDo = objToDo, .Schedule = objSchedule}, JsonRequestBehavior.AllowGet)
    End Function

    Function GetOpenToDo() As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (ORDER BY Name ASC) AS RowNum FROM (SELECT DISTINCT ISNULL(org.ID, ppol.ID) as ID, " & vbCrLf &
                "ISNULL(org.CompanyName, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname) as Name, " & vbCrLf &
                "(SELECT COUNT(ID) FROM TransCRM WHERE ActionName = 'To-Do' AND (ClientID = org.ID OR ClientID = ppol.ID) AND ToDoFinish = 0 AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1) as Counts " & vbCrLf &
                "FROM TransCRM as crm " & vbCrLf &
                "LEFT JOIN LibraryOrganization as org " & vbCrLf &
                "   ON org.ID = crm.ClientID " & vbCrLf &
                "LEFT JOIN LibraryPeople as ppol " & vbCrLf &
                "   ON ppol.ID = crm.ClientID " & vbCrLf &
                "WHERE ActionName = 'To-Do' AND ToDoFinish = 0 AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1) as SearchResults ) as SearchResultPagination " & vbCrLf

        If Common.WebRequest.Data("Search") <> Nothing Then
            pSql += " WHERE Name LIKE " & ReadyStringWild(Common.WebRequest.Data("Search"))
        Else
            If Common.WebRequest.Data("Start") = Nothing And Common.WebRequest.Data("End") = Nothing Then
                pSql += " WHERE RowNum >= 1 AND RowNum <= 10 "
            Else
                pSql += " WHERE RowNum >= " & Common.WebRequest.Data("Start") & " AND RowNum <= " & Common.WebRequest.Data("End")
            End If
        End If

        pSql += " ORDER BY Name asc "

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetOpenSchedule() As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (ORDER BY SchedActionDateFrom DESC) AS RowNum FROM (SELECT TOP 100 PERCENT TransCRM.ID AS [ID] " & vbCrLf &
                ", Notes, Location, Subject, CONVERT(datetime, SchedActionDateFrom) as SchedActionDateFrom, CONVERT(datetime, SchedActionDateTo) as SchedActionDateTo " & vbCrLf &
                ", SchedActionTeamID, SchedActionName, IsActionAllDay, TransCRM.LastUpdated as LastUpdated " & vbCrLf &
                ", ISNULL(createdppol.FirstName + ' ', '') + ISNULL(createdppol.MiddleName + ' ', '') + ISNULL(createdppol.Surname + ' ', '') as CreadtedBy " & vbCrLf &
                ", ISNULL(clientorg.CompanyName, ISNULL(clientppol.FirstName + ' ', '') + ISNULL(clientppol.MiddleName + ' ', '') + ISNULL(clientppol.Surname + ' ', '')) as ClientName " & vbCrLf &
                "FROM TransCRM INNER JOIN MobizLibraryPeople as createdppol ON CreatedBy = createdppol.ID " & vbCrLf &
                "   LEFT JOIN LibraryPeople as clientppol ON clientppol.ID = ClientID " & vbCrLf &
                "   LEFT JOIN LibraryOrganization as clientorg ON clientorg.ID = ClientID  WHERE ActionName = 'Scheduled Action' " & vbCrLf &
                "   AND CONVERT(datetime, SchedActionDateFrom) BETWEEN '03-01-2016' AND '03-31-2016') as SchedResults " & vbCrLf &
                " ) as SchedResultPagination" & vbCrLf

        If Common.WebRequest.Data("Search") <> Nothing Then
            pSql += " WHERE (Subject LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                        " OR SchedActionName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                        " OR ClientName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                        ")"
        Else
            If Common.WebRequest.Data("Start") = Nothing And Common.WebRequest.Data("End") = Nothing Then
                pSql += " WHERE RowNum >= 1 AND RowNum <= 10 "
            Else
                pSql += " WHERE RowNum >= " & Common.WebRequest.Data("Start") & " AND RowNum <= " & Common.WebRequest.Data("End")
            End If
        End If

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetSchedule() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransCRM.ID"
                End With

                With .Add("Note", SqlDbType.VarChar)
                    .Expression = "Notes" & vbCrLf
                End With

                With .Add("Location", SqlDbType.VarChar)
                    .Expression = "Location" & vbCrLf
                End With

                With .Add("Subject", SqlDbType.VarChar)
                    .Expression = "Subject" & vbCrLf
                End With

                With .Add("SchedActionDateFrom", SqlDbType.VarChar)
                    .Expression = "CONVERT(datetime, SchedActionDateFrom)" & vbCrLf
                End With

                With .Add("SchedActionDateTo", SqlDbType.VarChar)
                    .Expression = "CONVERT(datetime, SchedActionDateTo)" & vbCrLf
                End With

                With .Add("SchedActionTeamID", SqlDbType.VarChar)
                    .Expression = "SchedActionTeamID" & vbCrLf
                End With

                With .Add("SchedActionName", SqlDbType.VarChar)
                    .Expression = "SchedActionName" & vbCrLf
                End With

                With .Add("IsActionAllDay", SqlDbType.VarChar)
                    .Expression = "IsActionAllDay" & vbCrLf
                End With

                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "TransCRM.LastUpdated" & vbCrLf
                End With

                With .Add("CreatedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(createdppol.FirstName + ' ', '') + ISNULL(createdppol.MiddleName + ' ', '') + ISNULL(createdppol.Surname + ' ', '')" & vbCrLf
                End With

                With .Add("ClientName", SqlDbType.VarChar)
                    .Expression = "ISNULL(clientorg.CompanyName, ISNULL(clientppol.FirstName + ' ', '') + ISNULL(clientppol.MiddleName + ' ', '') + ISNULL(clientppol.Surname + ' ', ''))" & vbCrLf
                End With
            End With

            .SqlFrom = "FROM TransCRM INNER JOIN MobizLibraryPeople as createdppol ON CreatedBy = createdppol.ID " & vbCrLf &
                        "   LEFT JOIN LibraryPeople as clientppol ON clientppol.ID = ClientID " & vbCrLf &
                        "   LEFT JOIN LibraryOrganization as clientorg ON clientorg.ID = ClientID "

            .SqlWhere = "WHERE ActionName = 'Scheduled Action' AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 " & vbCrLf

            If Common.WebRequest.Data("Search") <> Nothing Then
                .SqlWhere += " AND (Subject LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                            " OR SchedActionName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                            " OR clientorg.CompanyName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                            " OR clientppol.FirstName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                            " OR clientppol.MiddleName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                            " OR clientppol.Surname LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf &
                            ")"

            Else
                .SqlWhere += " AND CONVERT(datetime, SchedActionDateFrom) BETWEEN " & ReadyString(Common.WebRequest.Data("From")) & " AND " & ReadyString(Common.WebRequest.Data("To"))
            End If

            .SqlOrder = "ORDER BY SchedActionDateFrom DESC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetTakeANote() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim searchkey As String = CStr2(Common.WebRequest.Data("SearchKey"))

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "crm.ID"
                End With

                With .Add("Note", SqlDbType.VarChar)
                    .Expression = "SUBSTRING(CAST(REPLACE(Notes, '&nbsp;', '&#160;') AS XML).value('.[1]', 'varchar(max)'), 1, 50)" & vbCrLf
                End With

                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "crm.LastUpdated" & vbCrLf
                End With

                With .Add("TaggedID", SqlDbType.UniqueIdentifier)
                    .Expression = "RelatedID"
                End With

                With .Add("TaggedDetails", SqlDbType.VarChar)
                    .Expression = "(SELECT CASE WHEN crm.TaggedType = 'Issue Ticket' THEN 'Ticket No. ' + CONVERT(varchar, TicketNumber) ELSE 'Opportunity named ' + OpportunityName  END From TransCRM WHERE ID = crm.RelatedID)"
                End With

                With .Add("TaggedType", SqlDbType.VarChar)
                    .Expression = "ISNULL(TaggedType, 'General')"
                End With

                With .Add("UpdatedBy", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName + ' ' + ppol.Surname "
                End With
            End With

            .SqlFrom = "FROM TransCRM as crm " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = crm.CreatedBy "

            .SqlWhere = "WHERE ClientID = " & ReadyString(ClientID) & vbCrLf &
                        " AND ActionName = 'Note'"

            If searchkey <> "" Then
                .SqlWhere &= " AND Notes LIKE " & ReadyStringWild(searchkey)
            End If

            .SqlOrder = "ORDER BY LastUpdated DESC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetNote() As DataTable
        Return Common.WebRequest.ProData.Execute("SELECT CAST(REPLACE(Notes, '&nbsp;', '&#160;') AS XML).value('.[1]', 'varchar(max)') as Note FROM TransCRM WHERE ActionName = 'Note' AND ID = " & ReadyString(Common.WebRequest.Data("NoteID")))
    End Function

    Function GetTickets() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim dborgid, dbname As String

        Try
            If PageSearchText <> "" Then
                Dim whereSearchContent As String = ""
                Dim operators As String = ""

                Dim arr As Object = Split(PageSearchText, " ")

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " OR "
                    End If

                    whereSearchContent = whereSearchContent & operators & " CONVERT(varchar, No) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Type) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, ProductServices) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, OtherType) " & vbCrLf &
                                    " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next
                whereSearch = " WHERE (" & whereSearchContent & ")"
            End If

            If IsCountPage = True Then
                whereLimit = ""
            End If

            If PageColumnNo = 0 Then
                whereOrder = " ORDER BY mainsql.No " & PageColumnDirection
            ElseIf PageColumnNo = 1 Then
                whereOrder = " ORDER BY mainsql.Type " & PageColumnDirection
            ElseIf PageColumnNo = 2 Then
                whereOrder = " ORDER BY mainsql.ProductServices " & PageColumnDirection
            ElseIf PageColumnNo = 3 Then
                whereOrder = " ORDER BY mainsql.Notes " & PageColumnDirection
            ElseIf PageColumnNo = 4 Then
                whereOrder = " ORDER BY mainsql.Status " & PageColumnDirection
            ElseIf PageColumnNo = 5 Then
                whereOrder = " ORDER BY CONVERT(datetime, mainsql.DueDate) " & PageColumnDirection
            ElseIf PageColumnNo = 6 Then
                whereOrder = " ORDER BY CONVERT(datetime, mainsql.TicketOpenDate) " & PageColumnDirection
            ElseIf PageColumnNo = 7 Then
                whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
            End If

            pSql = "SELECT * FROM  (Select mainsql.*, " & vbCrLf &
                    " ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    " SELECT DISTINCT crm.ID AS ID " & vbCrLf &
                    ", Notes " & vbCrLf &
                    ", crm.LastUpdated AS [LastUpdated] " & vbCrLf &
                    ", crm.TicketType AS [Type] " & vbCrLf &
                    ", crm.OtherTicketType AS [OtherType] " & vbCrLf &
                    ", crm.TicketNumber AS [No] " & vbCrLf &
                    ", crm.Status " & vbCrLf &
                    ", crm.AddressToService AS [Address] " & vbCrLf &
                    ", [ProductServices] " & vbCrLf &
                    ", [RecommendedAction] " & vbCrLf &
                    ", [OtherRecommendedAction] " & vbCrLf &
                    ", crm.DaysOfService AS [Days] " & vbCrLf &
                    ", crm.TicketDueDate AS [DueDate] " & vbCrLf &
                    ", CONVERT(datetime, TimeFrom) as TimeFrom " & vbCrLf &
                    ", CONVERT(datetime, TimeTo) as TimeTo " & vbCrLf &
                    ", [TicketOpenDate] " & vbCrLf &
                    ", (SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.OriginalCreatedBy) AS [OriginalCreatedBy] " & vbCrLf &
                    ", [ContactPersonID] " & vbCrLf &
                    ", crm.ContactPersonName AS [ContactPerson] " & vbCrLf &
                    ", crm.IssueAssignedTo AS [AssignedID] " & vbCrLf &
                    ", crm.IssueAssignedToName AS [Assigned] " & vbCrLf &
                    ", (SELECT TOP 1 ID FROM TransCRM WHERE TicketNumber = crm.TicketNumber ORDER BY LastUpdated ASC) AS [ParentTicketID] " & vbCrLf &
                    ", (SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.CreatedBy) AS [CreatedBy] " & vbCrLf &
                    " , ISNULL(iss.DelegateToClient, 0) as DelegateToClient " & vbCrLf &
                    " , crm.FowstID, ISNULL(fowst.Name, crm.CustomFowst) as FowstName " & vbCrLf &
                    " , updatedby.FirstName + ' ' + updatedby.Surname as LastUpdatedByName " & vbCrLf &
                    " , crm.TicketStatusConfirmedBy " & vbCrLf &
                    " , (SELECT TOP 1 Notes FROM TransCRM WHERE ActionName = 'Comment' AND RelatedID = (SELECT TOP 1 ID FROM TransCRM WHERE TicketNumber = crm.TicketNumber ORDER BY LastUpdated ASC)) as Comment " & vbCrLf &
                    " , groups.FranchiseGroupID " & vbCrLf &
                    " FROM TransCRM as crm " & vbCrLf &
                    " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' AND ISNULL(IsDelete, 0) = 0 GROUP BY TicketNumber) as updated  " & vbCrLf &
                    " ON crm.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated " & vbCrLf &
                    " LEFT JOIN MobizTransMobizIssues as iss ON iss.TicketNo = crm.TicketNumber " & IIf(CBool2(Common.WebRequest.QueryString("IsFranchisee")), "", " AND ClientID = " & ReadyString(ClientID)) & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = crm.FowstID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee as groups ON groups.FranchiseeOrgFowstID = fowst.ID AND groups.IsActive = 1 " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as updatedby ON updatedby.ID = ISNULL(crm.LastUpdatedBy, crm.CreatedBy) " & vbCrLf &
                    " WHERE " & IIf(CBool2(Common.WebRequest.QueryString("IsFranchisee")), "", " crm.ClientID = " & ReadyString(ClientID) & " AND ") & vbCrLf &
                    " crm.ActionName = 'Ticket' " & vbCrLf &
                    " AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 " & vbCrLf &
                    IIf(Filter = "All", "", " AND crm.Status = " & ReadyString(Filter)) & vbCrLf

            If CStr2(Common.WebRequest.QueryString("FowstGroupID")).ToLower() <> "all" Then
                pSql &= "AND crm.FowstID IN (SELECT FranchiseeOrgFowstID FROM MobizLibraryOrganizationFranchisee WHERE FranchiseGroupID = " & ReadyString(Common.WebRequest.QueryString("FowstGroupID")) & " AND FranchisorOrgID = " & ReadyString(ClientID) & ")"
            End If

            pSql &= ") as mainsql" & whereSearch & " ) as v2 " & whereLimit

            If CBool2(Common.WebRequest.QueryString("ViewTicketFromMobiz")) Then
#If DEBUG Then
                'MobizABC dev db
                dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                '1Mobiz prod db 
                dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If
                Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))
            End If

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(WebRequest.Instance.Business.LibraryOrganization.ByID(WebSession.Instance.OrganizationID)("Database")))

            pDT.Columns.Add("RegionName", GetType(String))
            pDT.Columns.Add("GroupName", GetType(String))

            For Each ticket As Object In pDT.Rows
                If Not String.IsNullOrWhiteSpace(CStr2(ticket("FranchiseGroupID"))) Then
                    pSql = " SELECT RegionName FROM LibraryRegionalGroupSettings " & vbCrLf &
                        " INNER JOIN LibraryRegionalGroupSettingsDetails ON LibraryRegionalGroupSettingsDetails.RegionalGroupID = LibraryRegionalGroupSettings.ID " & vbCrLf &
                        " WHERE LibraryRegionalGroupSettingsDetails.GroupID = " & ReadyString(ticket("FranchiseGroupID"))

                    ticket("RegionName") = Common.WebRequest.ProData.ExecuteScalar(pSql)

                    pSql = " SELECT GroupName FROM LibraryFranchiseGroup " & vbCrLf &
                        " WHERE ID = " & ReadyString(ticket("FranchiseGroupID"))

                    ticket("GroupName") = Common.WebRequest.ProData.ExecuteScalar(pSql)
                End If
            Next

            Return pDT
        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            AppendToLog(ex.Message)
            Return err
        End Try

        'Try
        '    Dim pFinder As Finder
        '    Dim pDT As DataTable

        '    pFinder = New Finder

        '    With pFinder
        '        .ProData = Common.WebRequest.ProData

        '        With .Fields
        '            With .Add("ID", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.ID"
        '            End With
        '            .Add("Notes", SqlDbType.VarChar)
        '            With .Add("LastUpdated", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.LastUpdated"
        '            End With
        '            With .Add("Type", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.TicketType"
        '            End With
        '            With .Add("OtherType", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.OtherTicketType"
        '            End With
        '            With .Add("No", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.TicketNumber"
        '            End With
        '            .Add("Status", SqlDbType.VarChar)
        '            With .Add("Address", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.AddressToService"
        '            End With
        '            .Add("ProductServices", SqlDbType.VarChar)
        '            .Add("RecommendedAction", SqlDbType.VarChar)
        '            .Add("OtherRecommendedAction", SqlDbType.VarChar)
        '            With .Add("Days", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.DaysOfService"
        '            End With
        '            With .Add("DueDate", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.TicketDueDate"
        '            End With
        '            .Add("TimeFrom", SqlDbType.VarChar)
        '            .Add("TimeTo", SqlDbType.VarChar)
        '            .Add("TicketOpenDate", SqlDbType.VarChar)
        '            With .Add("OriginalCreatedBy", SqlDbType.UniqueIdentifier)
        '                .Expression = "(SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.OriginalCreatedBy)"
        '            End With
        '            .Add("ContactPersonID", SqlDbType.VarChar)
        '            With .Add("ContactPerson", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.ContactPersonName"
        '            End With
        '            With .Add("AssignedID", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.IssueAssignedTo"
        '            End With
        '            With .Add("Assigned", SqlDbType.UniqueIdentifier)
        '                .Expression = "crm.IssueAssignedToName"
        '            End With
        '            With .Add("ParentTicketID", SqlDbType.UniqueIdentifier)
        '                .Expression = "(SELECT TOP 1 ID FROM TransCRM WHERE TicketNumber = crm.TicketNumber ORDER BY LastUpdated ASC)"
        '            End With
        '            With .Add("CreatedBy", SqlDbType.UniqueIdentifier)
        '                .Expression = "(SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.CreatedBy)"
        '            End With
        '        End With

        '        If Common.WebRequest.Data("Type") = "Details" Then
        '            .SqlFrom = "FROM TransCRM as crm" & vbCrLf &
        '                    " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated " & vbCrLf &
        '                    " ON crm.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated "

        '            .SqlWhere = "WHERE ClientID = " & ReadyString(Common.WebRequest.Data("ClientID")) & vbCrLf &
        '                        " AND ActionName = 'Ticket' " & vbCrLf &
        '                        IIf(Common.WebRequest.Data("Filter") = "All", "", " AND Status = " & ReadyString(Common.WebRequest.Data("Filter")))

        '            .SqlOrder = "ORDER BY LastUpdated DESC"
        '        Else 'HISTORY
        '            .SqlFrom = "FROM TransCRM as crm " & vbCrLf
        '            .SqlWhere = " WHERE (RelatedID = " & ReadyString(Common.WebRequest.Data("TicketID")) & vbCrLf &
        '                        " OR ID = " & ReadyString(Common.WebRequest.Data("TicketID")) & ") " & vbCrLf &
        '                        " AND ActionName = 'Ticket' " & vbCrLf
        '            .SqlOrder = "ORDER BY LastUpdated DESC"
        '        End If

        '        pDT = .Requery
        '    End With

        '    Return pDT
        'Catch ex As Exception
        '    AppendToLog(ex.Message)
        'End Try
    End Function

    Function GetTicketsHistory() As DataTable
        Try
            Dim pFinder As Finder
            Dim pDT As DataTable

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.ID"
                    End With
                    .Add("Notes", SqlDbType.VarChar)
                    With .Add("LastUpdated", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.LastUpdated"
                    End With
                    With .Add("Type", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.TicketType"
                    End With
                    With .Add("OtherType", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.OtherTicketType"
                    End With
                    With .Add("No", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.TicketNumber"
                    End With
                    .Add("Status", SqlDbType.VarChar)
                    With .Add("Address", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.AddressToService"
                    End With
                    .Add("ProductServices", SqlDbType.VarChar)
                    .Add("RecommendedAction", SqlDbType.VarChar)
                    .Add("OtherRecommendedAction", SqlDbType.VarChar)
                    With .Add("Days", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.DaysOfService"
                    End With
                    With .Add("DueDate", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.TicketDueDate"
                    End With
                    .Add("TimeFrom", SqlDbType.VarChar)
                    .Add("TimeTo", SqlDbType.VarChar)
                    .Add("TicketOpenDate", SqlDbType.VarChar)
                    With .Add("OriginalCreatedBy", SqlDbType.UniqueIdentifier)
                        .Expression = "(SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.OriginalCreatedBy)"
                    End With
                    .Add("ContactPersonID", SqlDbType.VarChar)
                    With .Add("ContactPerson", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.ContactPersonName"
                    End With
                    With .Add("AssignedID", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.IssueAssignedTo"
                    End With
                    With .Add("Assigned", SqlDbType.UniqueIdentifier)
                        .Expression = "crm.IssueAssignedToName"
                    End With
                    With .Add("ParentTicketID", SqlDbType.UniqueIdentifier)
                        .Expression = "(SELECT TOP 1 ID FROM TransCRM WHERE TicketNumber = crm.TicketNumber ORDER BY LastUpdated ASC)"
                    End With
                    With .Add("CreatedBy", SqlDbType.UniqueIdentifier)
                        .Expression = "(SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.CreatedBy)"
                    End With
                End With

                .SqlFrom = "FROM TransCRM as crm " & vbCrLf
                .SqlWhere = " WHERE (RelatedID = " & ReadyString(Common.WebRequest.Data("TicketID")) & vbCrLf &
                            " OR ID = " & ReadyString(Common.WebRequest.Data("TicketID")) & ") " & vbCrLf &
                            " AND ActionName = 'Ticket' " & vbCrLf
                .SqlOrder = "ORDER BY LastUpdated DESC"

                pDT = .Requery
            End With

            Return pDT
        Catch ex As Exception
            AppendToLog(ex.Message)
        End Try
    End Function

    Function GetTicketComments() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "crm.ID"
                End With
                With .Add("Comment", SqlDbType.VarChar)
                    .Expression = "ISNULL(crm.Notes, crm.ToDo)"
                End With
                With .Add("CommentedDate", SqlDbType.VarChar)
                    .Expression = "crm.LastUpdated"
                End With
                With .Add("CommentedBy", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname"
                End With
                .Add("ActionName", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM TransCRM as crm" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = crm.CreatedBy " & vbCrLf

            .SqlWhere = "WHERE RelatedID = " & ReadyString(Common.WebRequest.Data("TicketID")) & vbCrLf &
                        " AND (ActionName = 'Comment' OR ActionName = 'Note' OR ActionName = 'Call' OR ActionName = 'To-Do') " & vbCrLf &
                        " AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 "

            .SqlOrder = "ORDER BY crm.LastUpdated ASC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetClientToDo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ToDoID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With

                With .Add("ToDo", SqlDbType.VarChar)
                    .Expression = "ToDo" & vbCrLf
                End With

                With .Add("ClientID", SqlDbType.VarChar)
                    .Expression = "ClientID" & vbCrLf
                End With

                With .Add("ToDoFinish", SqlDbType.VarChar)
                    .Expression = "ToDoFinish" & vbCrLf
                End With

                With .Add("ToDoDueDate", SqlDbType.VarChar)
                    .Expression = "ISNULL(convert(varchar, convert(date, ToDoDueDate)), '')"
                End With

                With .Add("ToDoOrder", SqlDbType.VarChar)
                    .Expression = "ToDoOrder"
                End With

                With .Add("State", SqlDbType.VarChar)
                    .Expression = "'Edit'"
                End With

                With .Add("IsOrg", SqlDbType.VarChar)
                    .Expression = "IsOrg"
                End With

                With .Add("IsNew", SqlDbType.VarChar)
                    .Expression = "0"
                End With

                With .Add("TodoRankNo", SqlDbType.VarChar)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY ToDoOrder)"
                End With

                With .Add("TaggedID", SqlDbType.UniqueIdentifier)
                    .Expression = "RelatedID"
                End With

                With .Add("TaggedDetails", SqlDbType.VarChar)
                    .Expression = "(SELECT CASE WHEN crm.TaggedType = 'Issue Ticket' THEN 'Ticket No. ' + CONVERT(varchar, TicketNumber) ELSE 'Opportunity named ' + OpportunityName  END From TransCRM WHERE ID = crm.RelatedID)"
                End With

                With .Add("TaggedType", SqlDbType.VarChar)
                    .Expression = "ISNULL(TaggedType, 'General')"
                End With
            End With

            .SqlFrom = "FROM TransCRM as crm " & vbCrLf

            .SqlWhere = "WHERE ClientID = " & ReadyString(ClientID) & vbCrLf &
                        " AND ActionName = 'To-Do' " & vbCrLf &
                        " AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 "

            .SqlOrder = "ORDER BY ToDoDueDate ASC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetInteractionHistory() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        Try
            If PageSearchText <> "" Then
                Dim whereSearchContent As String = ""
                Dim operators As String = ""

                Dim arr As Object = Split(PageSearchText, " ")

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " OR "
                    End If

                    whereSearchContent = whereSearchContent & operators & " ClientName + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Unresolved) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Resolved) + ' ' + " & vbCrLf &
                                    "CONVERT(varchar, Cancelled) + ' ' + " & vbCrLf &
                                    "TicketNumbers " & vbCrLf &
                                    " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next
                whereSearch = " WHERE (" & whereSearchContent & ")"
            End If

            If IsCountPage = True Then
                whereLimit = ""
            End If

            If PageColumnNo = 0 Or PageColumnNo = 1 Then
                whereOrder = " ORDER BY mainsql.LastUpdated " & PageColumnDirection
            ElseIf PageColumnNo = 2 Then
                whereOrder = " ORDER BY mainsql.LastAction " & PageColumnDirection
            ElseIf PageColumnNo = 3 Then
                whereOrder = " ORDER BY mainsql.CreatedBy " & PageColumnDirection
            End If

            pSql = "SELECT * FROM  (Select mainsql.*, " & vbCrLf &
                    " ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    " SELECT DISTINCT crm.ID AS [ID] " & vbCrLf &
                    " , ISNULL(mppol.FirstName + ' ', '') + " & vbCrLf &
                    "   ISNULL(CASE WHEN mppol.MiddleName = '' THEN '' ELSE mppol.MiddleName + ' ' END, '') + " & vbCrLf &
                    "   ISNULL(mppol.Surname, '') AS [CreatedBy] " & vbCrLf &
                    " , ISNULL(crm.Notes, crm.SMSMessage) AS [Notes] " & vbCrLf &
                    " , crm.InteractionStart AS [CallStart] " & vbCrLf &
                    " , crm.InteractionEnd AS [CallEnd] " & vbCrLf &
                    " , crm.CallDuration AS [CallDuration] " & vbCrLf &
                    " , crm.LastUpdated AS [LastUpdated] " & vbCrLf &
                    " , crm.ActionName AS [LastAction] " & vbCrLf &
                    " , (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + att.FileName " & vbCrLf &
                    "       FROM LibraryCRMAttachments as att " & vbCrLf &
                    "       WHERE att.RelatedID = crm.ID " & vbCrLf &
                    "       FOR XML PATH('') " & vbCrLf &
                    "   ), 1, 1,'')) AS [Attachments] " & vbCrLf &
                    " FROM TransCRM as crm " & vbCrLf &
                    "   LEFT JOIN MobizLibraryPeople as mppol " & vbCrLf &
                    "       ON mppol.ID = crm.CreatedBy " & vbCrLf &
                    " WHERE ClientID = " & ReadyString(ClientID) & vbCrLf &
                    "   AND crm.ActionName <> 'To-Do' " & vbCrLf &
                    IIf(Filter = "All", "", " AND ActionName = " & ReadyString(Filter)) & vbCrLf &
                    ") as mainsql" & whereSearch & " ) as v2 " & whereLimit

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT

        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            AppendToLog(ex.Message)
            Return err
        End Try

        'Dim pFinder As Finder
        'Dim pDT As DataTable

        'pFinder = New Finder

        'With pFinder
        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("ID", SqlDbType.UniqueIdentifier)
        '            .Expression = "crm.ID"
        '        End With

        '        With .Add("CreatedBy", SqlDbType.VarChar)
        '            .Expression = "ISNULL(mppol.FirstName + ' ', '') + " & vbCrLf &
        '                            "ISNULL(CASE WHEN mppol.MiddleName = '' THEN '' ELSE mppol.MiddleName + ' ' END, '') + " & vbCrLf &
        '                            "ISNULL(mppol.Surname, '')" & vbCrLf
        '        End With

        '        With .Add("Notes", SqlDbType.VarChar)
        '            .Expression = "ISNULL(crm.Notes, crm.SMSMessage)"
        '        End With

        '        With .Add("CallStart", SqlDbType.VarChar)
        '            .Expression = "crm.InteractionStart"
        '        End With

        '        With .Add("CallEnd", SqlDbType.UniqueIdentifier)
        '            .Expression = "crm.InteractionEnd"
        '        End With

        '        With .Add("CallDuration", SqlDbType.UniqueIdentifier)
        '            .Expression = "crm.CallDuration"
        '        End With

        '        With .Add("LastUpdated", SqlDbType.UniqueIdentifier)
        '            .Expression = "crm.LastUpdated"
        '        End With

        '        With .Add("LastAction", SqlDbType.UniqueIdentifier)
        '            .Expression = "crm.ActionName"
        '        End With

        '        With .Add("Attachments", SqlDbType.UniqueIdentifier)
        '            .Expression = "(SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + att.FileName " & vbCrLf &
        '                            "   FROM LibraryCRMAttachments as att " & vbCrLf &
        '                            "   WHERE att.RelatedID = crm.ID " & vbCrLf &
        '                            "   FOR XML PATH('') " & vbCrLf &
        '                            " ), 1, 1,'')) " & vbCrLf
        '        End With
        '    End With

        '    .SqlFrom = "FROM TransCRM as crm " & vbCrLf &
        '                " LEFT JOIN MobizLibraryPeople as mppol " & vbCrLf &
        '                "   ON mppol.ID = crm.CreatedBy " & vbCrLf

        '    .SqlWhere = "WHERE crm.ClientID = " & ReadyString(ClientID) & vbCrLf &
        '                " AND crm.ActionName <> 'To-Do'"

        '    .SqlOrder = "ORDER BY crm.LastUpdated DESC"

        '    pDT = .Requery
        'End With

        'Return pDT
    End Function

    Function GetContactDetailsPeople() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()

        pOut = New JsonDictionary
        Dim objTelephone As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleTel(PersonID)))
        Dim objMobile As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleMobile(PersonID)))
        Dim objEmail As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleEmail(PersonID)))

        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("Email", objEmail)

        Return pOut.Serialize()
    End Function

    Function GetContactMobileDetailsPeople() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("MobileID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolmobile.ID"
                End With

                With .Add("FullMobileNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL('+' + CONVERT(varchar, countries.NumericCode),'') + " & vbCrLf &
                                    "ISNULL('.' + CONVERT(varchar, telco.Prefix),0) + " & vbCrLf &
                                    "ISNULL('.' + CONVERT(varchar, ppolmobile.ContactNo),0)" & vbCrLf
                End With

                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "ppolmobile.ContactNo"
                End With

                With .Add("NumericCode", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, countries.NumericCode)"
                End With

                With .Add("Prefix", SqlDbType.UniqueIdentifier)
                    .Expression = "telco.Prefix"
                End With

                With .Add("ContactNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolmobile.ContactNo"
                End With
            End With

            .SqlFrom = "FROM LibraryPeople as ppol " & vbCrLf &
                        " LEFT JOIN LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                        "   ON ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as countries " & vbCrLf &
                        "   ON ppolmobile.CountryID = countries.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcoPrefixes as telco " & vbCrLf &
                        "   ON ppolmobile.TelCoPrefixID = telco.ID " & vbCrLf

            .SqlWhere = "WHERE ppol.ID = " & ReadyString(PersonID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetContactTelDetailsPeople() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TelID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoltel.ID"
                End With

                With .Add("FullTelNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL('(+' + CONVERT(varchar, areacode.CallingCode) + ') ',0) + " & vbCrLf &
                                    "ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) + " & vbCrLf &
                                    "ISNULL(CASE WHEN ppoltel.ExtensionNo = '' THEN '' ELSE ',' + ppoltel.ExtensionNo END,0)" & vbCrLf
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "ppoltel.ContactNo"
                End With

                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppoltel.ExtensionNo, '')"
                End With

                With .Add("AreaCode", SqlDbType.UniqueIdentifier)
                    .Expression = "areacode.CallingCode"
                End With
            End With

            .SqlFrom = "FROM LibraryPeople as ppol " & vbCrLf &
                        " LEFT JOIN LibraryPeopleContactTel as ppoltel " & vbCrLf &
                        "   ON ppoltel.PeopleID = ppol.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as countries " & vbCrLf &
                        "   ON ppoltel.CountryID = countries.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryAreaCodes as areacode " & vbCrLf &
                        "   ON ppoltel.AreaCodeID = areacode.ID " & vbCrLf

            .SqlWhere = "WHERE ppol.ID = " & ReadyString(PersonID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetContactDetailsOrg() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()

        pOut = New JsonDictionary
        Dim objTelephone As Object = jss.Deserialize(Of Object)(GetJson(GetContactTelDetailsOrg()))
        Dim objMobile As Object = jss.Deserialize(Of Object)(GetJson(GetContactMobileDetailsOrg()))
        Dim objContactPerson As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgContactPerson(OrgID)))

        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("ContactPerson", objContactPerson)

        Return pOut.Serialize()
    End Function

    Function GetContactMobileDetailsOrg() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("MobileID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgmobile.ID"
                End With

                With .Add("FullMobileNo", SqlDbType.UniqueIdentifier)
                    .Expression = "'+' + CONVERT(varchar, countries.CallingCode) + " & vbCrLf &
                                    "'.' + CONVERT(varchar, telco.Prefix) + " & vbCrLf &
                                    "'.' + CONVERT(varchar, orgmobile.ContactNo)" & vbCrLf
                End With

                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "orgmobile.ContactNo"
                End With

                With .Add("NumericCode", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, countries.CallingCode)"
                End With

                With .Add("Prefix", SqlDbType.UniqueIdentifier)
                    .Expression = "telco.Prefix"
                End With

                With .Add("ContactNo", SqlDbType.UniqueIdentifier)
                    .Expression = "orgmobile.ContactNo"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganization as org " & vbCrLf &
                        " LEFT JOIN LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                        "   ON orgmobile.OrganizationID = org.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as countries " & vbCrLf &
                        "   ON orgmobile.CountryID = countries.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcoPrefixes as telco " & vbCrLf &
                        "   ON orgmobile.TelCoPrefixID = telco.ID " & vbCrLf

            .SqlWhere = "WHERE org.ID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetContactTelDetailsOrg() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TelID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgtel.ID"
                End With

                With .Add("FullTelNo", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(varchar, '(+' + areacode.CallingCode + ') ') + " & vbCrLf &
                                    "CONVERT(varchar, orgtel.ContactNo) + " & vbCrLf &
                                    "CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE ',' + orgtel.ExtensionNo END" & vbCrLf
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "orgtel.ContactNo"
                End With

                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(orgtel.ExtensionNo, '')"
                End With

                With .Add("AreaCode", SqlDbType.UniqueIdentifier)
                    .Expression = "areacode.CallingCode"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganization as org " & vbCrLf &
                        " LEFT JOIN LibraryOrganizationContactTel as orgtel " & vbCrLf &
                        "   ON orgtel.OrganizationID = org.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as countries " & vbCrLf &
                        "   ON orgtel.CountryID = countries.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryAreaCodes as areacode " & vbCrLf &
                        "   ON orgtel.AreaCodeID = areacode.ID " & vbCrLf

            .SqlWhere = "WHERE org.ID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
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

        pOut.Add("PeopleDetails", objPeopleDetails)
        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("Email", objEmail)
        pOut.Add("Affiliation", objAffiliation)
        pOut.Add("Address", objAddress)
        pOut.Add("Education", objEducation)

        Return pOut.Serialize()
    End Function

    Function GetProspectPeopleEducation(ByVal PeopleID As String) As Object
        Dim pSql As String
        Dim pDT As DataTable
        Dim jss = New JavaScriptSerializer()
        Dim objElem As Object
        Dim objHs As Object
        Dim objCollege As Object

        pSql = "SELECT ROW_NUMBER() OVER(ORDER BY InstitutionName ASC) as No, ID, InstitutionName as Name, AcademicLevel as Level, 'edit' as State, YearStarted as Started, YearGraduated as Graduated FROM " & IIf(CurrentModule = "crm", "LibraryPeopleEducation", "MobizLibraryPeopleEducation") & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND AcademicLevel = 'elem'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        objElem = jss.Deserialize(Of Object)(GetJson(pDT))

        '''''-----------------------

        pSql = "SELECT ROW_NUMBER() OVER(ORDER BY InstitutionName ASC) as No, ID, InstitutionName as Name, AcademicLevel as Level, 'edit' as State, YearStarted as Started, YearGraduated as Graduated FROM " & IIf(CurrentModule = "crm", "LibraryPeopleEducation", "MobizLibraryPeopleEducation") & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND AcademicLevel = 'hs'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        objHs = jss.Deserialize(Of Object)(GetJson(pDT))

        '''''-----------------------

        pSql = "SELECT ROW_NUMBER() OVER(ORDER BY InstitutionName ASC) as No, ID, InstitutionName as Name, AcademicLevel as Level, 'edit' as State, YearStarted as Started, YearGraduated as Graduated FROM " & IIf(CurrentModule = "crm", "LibraryPeopleEducation", "MobizLibraryPeopleEducation") & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND AcademicLevel = 'college'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        objCollege = jss.Deserialize(Of Object)(GetJson(pDT))

        Return Json(New With {Key .Elementary = objElem, .HighSchool = objHs, .College = objCollege})
    End Function

    Function GetProspectPeopleAddress(ByVal PeopleID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT *, ISNULL(city.Name, '') as CityName, ISNULL(country.Name, '') as CountryName FROM " & IIf(CurrentModule = "crm", "LibraryPeopleAddress", "MobizLibraryPeopleAddress") & " as address " & vbCrLf &
                " LEFT JOIN MobizLibraryCities as city ON City.ID = address.CityID " & vbCrLf &
                " LEFT JOIN MobizLibraryCountries as country ON country.ID = address.CountryID " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectPeopleAffiliation(ByVal PeopleID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT cp.ID as ID, org.ID as OrgID, org.CompanyName as CompanyName, IsNew = '0', State= 'Edit' FROM " & IIf(CurrentModule = "crm", "LibraryOrganizationContactPerson", "MobizLibraryOrganizationContactPerson") & " as cp  " & vbCrLf &
                " LEFT JOIN LibraryOrganization as org ON org.ID = cp.OrganizationID " & vbCrLf &
                " WHERE PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                " AND IsUseForPersonOrgAffiliates = 1 " & vbCrLf &
                " ORDER BY org.CompanyName ASC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectPeopleDetails(ByVal PeopleID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * FROM " & IIf(CurrentModule = "crm", "LibraryPeople", "MobizLibraryPeople") & " WHERE ID = " & ReadyString(PeopleID)
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

            .SqlFrom = "FROM " & IIf(CStr2(Common.WebRequest.Data("IsCustomer")) = "" Or CurrentModule <> "crm", "LibraryPeopleContactTel", "MobizLibraryPeopleContactTel") & " as ppoltel " & vbCrLf &
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
                                    " ISNULL(CASE WHEN prefix.Prefix = '' THEN '' ELSE '.' + CONVERT(varchar, prefix.Prefix) END, '') + " & vbCrLf &
                                    " + '.' + ppolmobile.ContactNo " & vbCrLf
                End With
            End With

            .SqlFrom = "FROM " & IIf(CStr2(Common.WebRequest.Data("IsCustomer")) = "" Or CurrentModule <> "crm", "LibraryPeopleContactMobile", "MobizLibraryPeopleContactMobile") & " as ppolmobile " & vbCrLf &
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
            End With

            .SqlFrom = "FROM " & IIf(CStr2(Common.WebRequest.Data("IsCustomer")) = "" Or CurrentModule <> "crm", "LibraryPeopleContactEmail", "MobizLibraryPeopleContactEmail") & " as ppolemail " & vbCrLf

            .SqlWhere = "WHERE ppolemail.PeopleID = " & ReadyString(PeopleID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectDetails() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()

        pOut = New JsonDictionary

        Dim objOrgDetails As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgDetails(OrgID)))
        Dim objTelephone As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgTel(OrgID)))
        Dim objMobile As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgMobile(OrgID)))
        Dim objEmail As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgEmail(OrgID)))
        Dim objWebsite As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgWebsite(OrgID)))
        Dim objContactPerson As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgContactPerson(OrgID)))
        Dim objAffiliates As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgAffiliation(OrgID)))
        Dim objAffiliatedPerson As Object = jss.Deserialize(Of Object)(GetJson(GetProspectAffiliatedPerson(OrgID)))
        Dim objAddress As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgAddress(OrgID)))

        pOut.Add("OrgDetails", objOrgDetails)
        pOut.Add("Telephone", objTelephone)
        pOut.Add("Mobile", objMobile)
        pOut.Add("Email", objEmail)
        pOut.Add("Website", objWebsite)
        pOut.Add("ContactPerson", objContactPerson)
        pOut.Add("Affiliation", objAffiliates)
        pOut.Add("AffiliatedPerson", objAffiliatedPerson)
        pOut.Add("Address", objAddress)

        Return pOut.Serialize()
    End Function

    Function GetProspectOrgAddress(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * FROM (SELECT DISTINCT fowst.ID AS [ID]  " & vbCrLf &
                ", dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') + ', ' + ISNULL(fowst.Street, '') + ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, '')) AS [CompleteAddress] " & vbCrLf &
                ", fowst.IsActive " & vbCrLf &
                ", 'edit' as State" & vbCrLf &
                "FROM dbo.MobizLibraryOrganizationFOWST fowst " & vbCrLf &
                "LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " & vbCrLf &
                "LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) " & vbCrLf &
                "WHERE fowst.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                "AND ISNULL(fowst.IsActive, 1) = 1) t " & vbCrLf &
                "WHERE LEN(LTRIM(RTRIM(ISNULL(CompleteAddress, '')))) > 0 ORDER BY CompleteAddress ASC"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectOrgAffiliation(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT orgaff.ID as ID, org.ID as OrgID, org.CompanyName as CompanyName, IsNew = '0', State= 'Edit' FROM LibraryOrganizationAffiliates as orgaff  " & vbCrLf &
                " LEFT JOIN LibraryOrganization as org ON org.ID = orgaff.AffiliatesID " & vbCrLf &
                " WHERE orgaff.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                " ORDER BY org.CompanyName ASC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectAffiliatedPerson(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT cppol.ID as ID, ppol.ID as PeopleID, ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') as PeopleName, IsNew = '0', State= 'Edit' " & vbCrLf &
                " FROM LibraryPeople as ppol " & vbCrLf &
                " LEFT JOIN LibraryOrganizationContactPerson as cppol ON cppol.PeopleID = ppol.ID " & vbCrLf &
                " WHERE cppol.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                " AND IsUseForPersonOrgAffiliates = 1 " & vbCrLf &
                " ORDER BY Surname ASC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectOrgDetails(ByVal OrgID As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        'MobizLibraryIndustryClass.ID as IndustryID, MobizLibraryIndustryClass.Class as IndustryName,
        'LEFT JOIN MobizLibraryIndustryClass ON LibraryOrganization.IndustryClassID = MobizLibraryIndustryClass.ID
        pSql = "SELECT LibraryOrganization.ID, CompanyName, OfficeAddressBuildingNo, OfficeAddressCityID, OfficeAddressCountryID, OfficeAddressStreet" & vbCrLf &
                " , ISNULL(OfficeAddressBuildingNo + ', ', '') + ' ' + ISNULL(OfficeAddressStreet + ', ', '') + ' ' + city.Name + ' ' + country.Name as Address " & vbCrLf &
                " , Background, Mission, Vision, FoundingDate, Region, Island, LibraryOrganization.IsActive, MobizLibraryIndustryClass.Class as IndustryName, MobizLibraryIndustryClass.ID as IndustryID " & vbCrLf &
                " , ISNULL(CONVERT(varchar, NoOfEmployees), '') as NoOfEmployees, ISNULL(CONVERT(varchar, RevenuePerYear), '') as RevenuePerYear, ISNULL(CONVERT(varchar, ScopeOfOperations), '') as ScopeOfOperations " & vbCrLf &
                " , ISNULL(CONVERT(varchar, NoOfFactory), '') as NoOfFactory, ISNULL(CONVERT(varchar, NoOfOffice), '') as NoOfOffice, ISNULL(CONVERT(varchar, NoOfWarehouse), '') as NoOfWarehouse, ISNULL(CONVERT(varchar, NoOfStore), '') as NoOfStore, ISNULL(CONVERT(varchar, NoOfTruck), '') as NoOfTruck " & vbCrLf &
                " FROM LibraryOrganization LEFT JOIN MobizLibraryIndustryClass ON LibraryOrganization.IndustryClassID = MobizLibraryIndustryClass.ID " & vbCrLf &
                " LEFT JOIN MobizLibraryCities as city ON City.ID = OfficeAddressCityID " & vbCrLf &
                " LEFT JOIN MobizLibraryCountries as country ON country.ID = OfficeAddressCountryID " & vbCrLf &
                " WHERE LibraryOrganization.ID = " & ReadyString(OrgID)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProspectOrgTel(ByVal OrgID As String) As DataTable
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

            .SqlFrom = "FROM LibraryOrganizationContactTel as orgtel " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry " & vbCrLf &
                        "   ON ctry.ID = orgtel.CountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryAreaCodes as areacode " & vbCrLf &
                        "   ON areacode.ID = orgtel.AreaCodeID " & vbCrLf

            .SqlWhere = "WHERE orgtel.OrganizationID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectOrgMobile(ByVal OrgID As String) As DataTable
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
                    .Expression = "ctry.CallingCode"
                End With

                With .Add("Alpha2Code", SqlDbType.UniqueIdentifier)
                    .Expression = "ctry.Alpha2Code"
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

            .SqlFrom = "FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry " & vbCrLf &
                        "   ON ctry.ID = orgmobile.CountryID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcoPrefixes as prefix " & vbCrLf &
                        "   ON prefix.ID = orgmobile.TelCoPrefixID " & vbCrLf &
                        " LEFT JOIN MobizLibraryTelcos as telco " & vbCrLf &
                        "   ON telco.ID = prefix.TelCoID " & vbCrLf

            .SqlWhere = "WHERE orgmobile.OrganizationID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectOrgEmail(ByVal OrgID As String) As DataTable
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

            .SqlFrom = "FROM LibraryOrganizationContactEmail as orgemail " & vbCrLf

            .SqlWhere = "WHERE orgemail.OrganizationID = " & ReadyString(OrgID) & vbCrLf

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

            .SqlFrom = "FROM LibraryOrganizationContactWebsite as orgwebsite " & vbCrLf

            .SqlWhere = "WHERE orgwebsite.OrganizationID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectOrgContactPerson(ByVal OrgID As String) As DataTable
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
                    .Expression = "ppol.MiddleName"
                End With

                With .Add("Lastname", SqlDbType.VarChar)
                    .Expression = "ppol.Surname"
                End With

                With .Add("Position", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.Position, '')"
                End With

                With .Add("PositionDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.PositionDescription, '')"
                End With

                With .Add("FullName", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.FirstName + ' ' ,'') + " & vbCrLf &
                                    "ISNULL(CASE WHEN ppol.MiddleName  = '' THEN '' ELSE ppol.MiddleName + ' ' END, '') + " & vbCrLf &
                                    "ISNULL(ppol.Surname ,'') "
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
                                "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                                "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                                "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                                "           WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                                "       FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,'') FROM LibraryPeopleContactMobile) " & vbCrLf
                End With

                With .Add("TelNos", SqlDbType.VarChar)
                    .Expression = " (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                                "       ISNULL('-' + CONVERT(varchar, ppolareacode.CallingCode) + '-',0) + " & vbCrLf &
                                "       ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) " & vbCrLf &
                                "       FROM LibraryPeopleContactTel  as ppoltel " & vbCrLf &
                                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                "               ON ppoltel.CountryID = ppolcountries.ID  " & vbCrLf &
                                "           LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                                "               ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                                "           WHERE ppoltel.PeopleID = ppol.ID " & vbCrLf &
                                "       FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,'') FROM LibraryPeopleContactMobile) " & vbCrLf
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationContactPerson as contactperson " & vbCrLf &
                        "LEFT JOIN LibraryPeople as ppol " & vbCrLf &
                        "   ON ppol.ID = contactperson.PeopleID " & vbCrLf &
                        "LEFT JOIN LibraryPeopleAddress as ppoladd " & vbCrLf &
                        "   ON ppoladd.PeopleID = contactperson.PeopleID "

            .SqlWhere = "WHERE contactperson.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                        " AND contactperson.IsUseForPersonOrgAffiliates = 0"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetJson(ByVal dt As DataTable) As String
        Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
    End Function

    Function GetProspectList() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim whereSearchContent As String = ""
        Dim additionalwhereSearchContent = ""
        Dim surveyClientID As New List(Of String)

        Try
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
                        ElseIf arrtxtFilter(0) = "Survey" Then
                            additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(SurveyIDs, '') LIKE " & ReadyStringWild(arrtxtFilter(1))
                        ElseIf arrtxtFilter(0) = "Survey" Then
                            Dim pSqlSurvey As String = "SELECT DISTINCT resd.ClientID " & vbCrLf &
                                                        " FROM LibrarySurveyResponse as res " & vbCrLf &
                                                        " INNER JOIN LibrarySurveyRespondent as resd ON res.RespondentID = resd.ID " & vbCrLf &
                                                        " WHERE res.FieldID = " & ReadyString(arrtxtFilter(1))
                            Dim pDTSurvey As DataTable = Common.WebRequest.ProData.Execute(pSqlSurvey)

                            If pDTSurvey.Rows.Count > 0 Then
                                If surveyClientID.Contains(pDTSurvey.Rows(0)("ClientID").ToString()) Then
                                    additionalwhereSearchContent = additionalwhereSearchContent.Substring(0, IIf(additionalwhereSearchContent = "", 0, additionalwhereSearchContent.LastIndexOf("OR")))
                                Else
                                    surveyClientID.Add(pDTSurvey.Rows(0)("ClientID").ToString())
                                    additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(ID, '') = " & ReadyString(pDTSurvey.Rows(0)("ClientID").ToString())
                                End If
                            Else
                                additionalwhereSearchContent = additionalwhereSearchContent.Substring(0, IIf(additionalwhereSearchContent = "", 0, additionalwhereSearchContent.LastIndexOf("OR")))
                            End If
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
                whereOrder = " ORDER BY CONVERT(datetime, SalesOrderDate) " & PageColumnDirection
            ElseIf PageColumnNo = 7 Then
                whereOrder = " ORDER BY CONVERT(datetime, InvoiceDate) " & PageColumnDirection
            ElseIf PageColumnNo = 8 Then
                whereOrder = " ORDER BY IsForFollowUp " & PageColumnDirection
            ElseIf PageColumnNo = 9 Then
                whereOrder = " ORDER BY CONVERT(datetime, LastUpdated) " & PageColumnDirection
            ElseIf PageColumnNo = 10 Then
                whereOrder = " ORDER BY CONVERT(datetime, LastInteractionDate) " & PageColumnDirection
            End If

            pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From (SELECT * FROM (SELECT org.ID as ID, " & vbCrLf &
                    "   CONVERT(varchar, org.IsActive) as Status, " & vbCrLf &
                    "   LTRIM(org.CompanyName) as ClientName, " & vbCrLf &
                    "   (SELECT TOP 1 ISNULL(CASE WHEN orgemail.EmailAddress = '' THEN '' ELSE orgemail.EmailAddress END,'') " & vbCrLf &
                    "       FROM LibraryOrganizationContactEmail as orgemail" & vbCrLf &
                    "       WHERE(orgemail.OrganizationID = org.ID)) as Email, " & vbCrLf &
                    "   (SELECT TOP 1 " & vbCrLf &
                    "       ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode) + ' ',0) + " & vbCrLf &
                    "       ISNULL(' (' + CONVERT(varchar, orgareacode.CallingCode) + ') ',0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, orgtel.ContactNo),0) + " & vbCrLf &
                    "       ISNULL(CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE ',' + orgtel.ExtensionNo END,0) " & vbCrLf &
                    "       FROM LibraryOrganizationContactTel as orgtel " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                    "               ON orgtel.CountryID = orgcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryAreaCodes as orgareacode " & vbCrLf &
                    "               ON orgtel.AreaCodeID = orgareacode.ID " & vbCrLf &
                    "           WHERE orgtel.OrganizationID = org.ID ) as Telephone, " & vbCrLf &
                    "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                    "       CASE WHEN LEN(orgmobile.ContactNo) <= 7 THEN CONVERT(varchar, orgtelco.Prefix) ELSE '' END + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
                    "       FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                    "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                    "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                    "           WHERE orgmobile.OrganizationID = org.ID) as Mobile, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, orgtelco.Prefix),0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
                    "       FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                    "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                    "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                    "           WHERE orgmobile.OrganizationID = org.ID " & vbCrLf &
                    "       FOR XML PATH('') " & vbCrLf &
                    "   ), 1, 1,'') FROM LibraryOrganizationContactMobile) as MobileNos," & vbCrLf &
                    "   (SELECT TOP 1 ISNULL(orgwebsite.Website ,'') " & vbCrLf &
                    "       FROM LibraryOrganizationContactWebsite  as orgwebsite " & vbCrLf &
                    "       WHERE orgwebsite.OrganizationID = org.ID) as Website, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org2.CompanyName ,'') " & vbCrLf &
                    "                               FROM LibraryOrganizationAffiliates   as orgaff " & vbCrLf &
                    "                               LEFT JOIN LibraryOrganization as org2 " & vbCrLf &
                    "                                   ON org2.ID = orgaff.AffiliatesID " & vbCrLf &
                    "                               WHERE orgaff.OrganizationID = org.ID " & vbCrLf &
                    "   FOR XML PATH('')), 1, 1,'') FROM LibraryOrganizationAffiliates) as Affiliation, " & vbCrLf &
                    "   ISNULL((SELECT city.Name " & vbCrLf &
                    "       FROM MobizLibraryCities as city " & vbCrLf &
                    "       LEFT JOIN LibraryOrganization as org2 " & vbCrLf &
                    "           ON org2.OfficeAddressCityID = city.ID " & vbCrLf &
                    "       WHERE org2.ID= org.ID), '') as City, " & vbCrLf &
                    "   ISNULL(CONVERT(varchar(max), org.OfficeAddressCityID), '') as CityID, " & vbCrLf &
                    "   ISNULL((SELECT country.Name " & vbCrLf &
                    "       FROM MobizLibraryCountries as country " & vbCrLf &
                    "       LEFT JOIN LibraryOrganization as org3 " & vbCrLf &
                    "           ON org3.OfficeAddressCountryID = country.ID " & vbCrLf &
                    "       WHERE org3.ID= org.ID), '') as Country, " & vbCrLf &
                    "   CONVERT(varchar, org.LastUpdated) as LastUpdated, " & vbCrLf &
                    "   'Org' as ProspectType, " & vbCrLf &
                    "   (SELECT ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') " & vbCrLf &
                    "       FROM LibraryOrganization " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople " & vbCrLf &
                    "               ON LibraryOrganization.UserID = MobizLibraryPeople.ID " & vbCrLf &
                    "           WHERE LibraryOrganization.ID = org.ID) as ModifiedBy, " & vbCrLf &
                    "   '' as ContactPerson, " & vbCrLf &
                    "   (SELECT TOP 1 convert(varchar,LastUpdated) FROM TransCRM WHERE ClientID = org.ID ORDER BY LastUpdated DESC) as LastInteractionDate, " & vbCrLf &
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
                    "   ISNULL((SELECT SUM(TotalScore) " & vbCrLf &
                    "       FROM TransSurveyRespondentScore as score " & vbCrLf &
                    "       LEFT JOIN LibrarySurveyRespondent as res ON res.ID = score.RespondentID " & vbCrLf &
                    "       WHERE res.ClientID = org.ID " & vbCrLf &
                    "   ), 0) as TotalScore, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + ISNULL(convert(varchar(max), res.QuestionaireID), '') " & vbCrLf &
                    "       FROM LibrarySurveyRespondent as res " & vbCrLf &
                    "       WHERE res.ClientID = org.ID " & vbCrLf &
                    "       FOR XML PATH('')), 1, 1,'') FROM LibrarySurveyRespondent) as SurveyIDs " & vbCrLf &
                    "FROM LibraryOrganization as org" & If(Status = "", " WHERE org.IsActive = '1' ", If(Status = "All", "", " WHERE org.IsActive = " & ReadyString(Status))) & ") as oTable " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT TOP 1 TransInvoice.SalesOrderDate " & vbCrLf &
                    "       FROM TransInvoice " & vbCrLf &
                    "       WHERE TransInvoice.TransType = 'SalesOrder' " & vbCrLf &
                    "       AND TransInvoice.CustomerID = oTable.ID " & vbCrLf &
                    "       ORDER BY TransInvoice.SalesOrderDate DESC " & vbCrLf &
                    "   ) tblSalesOrder " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT TOP 1 TransInvoice.InvoiceDate " & vbCrLf &
                    "       FROM TransInvoice " & vbCrLf &
                    "       WHERE TransInvoice.TransType = 'Invoice' " & vbCrLf &
                    "       AND TransInvoice.CustomerID = oTable.ID " & vbCrLf &
                    "       ORDER BY TransInvoice.InvoiceDate DESC " & vbCrLf &
                    "   ) tblInvoice " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT ISNULL(IsForFollowUp, 0) as IsForFollowUp " & vbCrLf &
                    "       FROM LibraryOrganizationCustomer " & vbCrLf &
                    "       WHERE LibraryOrganizationCustomer.OrganizationID = oTable.ID " & vbCrLf &
                    "   ) tblCustomer " & vbCrLf & whereSearch & vbCrLf &
                    "   " & vbCrLf &
                    "UNION ALL  " & vbCrLf &
                    "   " & vbCrLf &
                    "SELECT * FROM (SELECT ppol.ID as ID,   " & vbCrLf &
                    "   CONVERT(varchar, ppol.IsActive) as Status, " & vbCrLf &
                    "   LTRIM(ISNULL(FirstName + ' ', '') + ISNULL(CASE WHEN MiddleName = '' THEN '' ELSE MiddleName + ' ' END, '') + ISNULL(Surname, '')) as ClientName, " & vbCrLf &
                    "   (SELECT TOP 1 ISNULL(CASE WHEN ppolemail.EmailAddress = '' THEN '' ELSE ppolemail.EmailAddress END,'') " & vbCrLf &
                    "       FROM LibraryPeopleContactEmail as ppolemail " & vbCrLf &
                    "           WHERE ppolemail.PeopleID = ppol.ID) as Email," & vbCrLf &
                    "   (SELECT TOP 1 " & vbCrLf &
                    "       ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode) + ' ',0) + " & vbCrLf &
                    "       ISNULL('(' + CONVERT(varchar, ppolareacode.CallingCode) + ') ',0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) + " & vbCrLf &
                    "       ISNULL(CASE WHEN ppoltel.ExtensionNo = '' THEN '' ELSE ',' + ppoltel.ExtensionNo END,0) " & vbCrLf &
                    "       FROM LibraryPeopleContactTel as ppoltel " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "               ON ppoltel.CountryID = ppolcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                    "               ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                    "       WHERE ppoltel.PeopleID = ppol.ID) as Telephone," & vbCrLf &
                    "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                    "       CASE WHEN LEN(ppolmobile.ContactNo) <= 7 THEN CONVERT(varchar, ppoltelco.Prefix) ELSE '' END + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                    "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                    "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                    "           WHERE ppolmobile.PeopleID = ppol.ID) as Mobile, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppoltelco.Prefix),0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                    "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                    "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                    "           WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                    "       FOR XML PATH('') " & vbCrLf &
                    "   ), 1, 1,'') FROM LibraryPeopleContactMobile) as MobileNos, " & vbCrLf &
                    "   '' as Website, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org.CompanyName ,'') " & vbCrLf &
                    "                               FROM LibraryOrganizationContactPerson as cp " & vbCrLf &
                    "                                   LEFT JOIN LibraryOrganization as org ON org.ID = cp.OrganizationID" & vbCrLf &
                    "                               WHERE cp.PeopleID = ppol.ID " & vbCrLf &
                    "   FOR XML PATH('')), 1, 1,'') FROM LibraryOrganizationContactPerson) as Affiliation, " & vbCrLf &
                    "   ISNULL(city.Name, '') as City, " & vbCrLf &
                    "   ISNULL(CONVERT(varchar(max), city.ID), '') as CityID, " & vbCrLf &
                    "   ISNULL(ctry.Name, '') as Country, " & vbCrLf &
                    "   CONVERT(varchar, ppol.LastUpdated) as LastUpdated, " & vbCrLf &
                    "   'People' as ProspectType, " & vbCrLf &
                    "   (SELECT ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') " & vbCrLf &
                    "       FROM MobizLibraryPeople " & vbCrLf &
                    "       WHERE MobizLibraryPeople.ID = ppol.UserID) as ModifiedBy, " & vbCrLf &
                    "   '' as ContactPerson, " & vbCrLf &
                    "   (SELECT TOP 1 convert(varchar,LastUpdated) FROM TransCRM WHERE ClientID = ppol.ID ORDER BY LastUpdated DESC) as LastInteractionDate, " & vbCrLf &
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
                    "   ISNULL((SELECT SUM(TotalScore) " & vbCrLf &
                    "       FROM TransSurveyRespondentScore as score " & vbCrLf &
                    "       LEFT JOIN LibrarySurveyRespondent as res ON res.ID = score.RespondentID " & vbCrLf &
                    "       WHERE res.ClientID = ppol.ID " & vbCrLf &
                    "   ), 0) as TotalScore, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + ISNULL(convert(varchar(max), res.QuestionaireID), '') " & vbCrLf &
                    "       FROM LibrarySurveyRespondent as res " & vbCrLf &
                    "       WHERE res.ClientID = ppol.ID " & vbCrLf &
                    "       FOR XML PATH('')), 1, 1,'') FROM LibrarySurveyRespondent) as SurveyIDs " & vbCrLf &
                    "   FROM LibraryPeople as ppol " & vbCrLf &
                    "   LEFT JOIN LibraryPeopleAddress as ppoladd ON ppol.ID = ppoladd.PeopleID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryCities as city ON city.ID = ppoladd.CityID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryCountries as ctry ON ctry.ID = ppoladd.CountryID" & vbCrLf &
                    "   WHERE IsFromOrg = 0 " & If(Status = "", " AND ppol.IsActive = '1' ", If(Status = "All", "", "AND ppol.IsActive = " & ReadyString(Status))) & ") as pTable " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT TOP 1 TransInvoice.SalesOrderDate " & vbCrLf &
                    "       FROM TransInvoice " & vbCrLf &
                    "       WHERE TransInvoice.TransType = 'SalesOrder' " & vbCrLf &
                    "       AND TransInvoice.CustomerID = pTable.ID " & vbCrLf &
                    "       ORDER BY TransInvoice.SalesOrderDate DESC " & vbCrLf &
                    "   ) tblSalesOrder " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT TOP 1 TransInvoice.InvoiceDate " & vbCrLf &
                    "       FROM TransInvoice " & vbCrLf &
                    "       WHERE TransInvoice.TransType = 'Invoice' " & vbCrLf &
                    "       AND TransInvoice.CustomerID = pTable.ID " & vbCrLf &
                    "       ORDER BY TransInvoice.InvoiceDate DESC " & vbCrLf &
                    "   ) tblInvoice " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT ISNULL(IsForFollowUp, 0) as IsForFollowUp " & vbCrLf &
                    "       FROM LibraryOrganizationCustomer " & vbCrLf &
                    "       WHERE LibraryOrganizationCustomer.OrganizationID = pTable.ID " & vbCrLf &
                    "   ) tblCustomer " & vbCrLf & whereSearch & vbCrLf &
                    ") as V1" & vbCrLf

            If (CrmType = "revsalescrm") Then
                pSql &= " WHERE ID IN (SELECT CustomerID FROM TransInvoice WHERE TransType = " & ReadyString(InvType) & vbCrLf

                If InvType = "Invoice" Then
                    pSql &= " AND ISNULL(InvoiceNumber, 0) > 0 AND ComputedInvoiceYearWeek >= " & ReadyString(YearWeek) & " AND ComputedInvoiceYearWeek <= " & ReadyString(LastWeek) & vbCrLf
                ElseIf InvType = "SalesOrder" Then
                    pSql &= " AND ISNULL(SalesOrderNo, 0) > 0 AND ComputedSalesOrderYearWeek >= " & ReadyString(YearWeek) & " AND ComputedSalesOrderYearWeek <= " & ReadyString(LastWeek) & vbCrLf
                ElseIf InvType = "Quote" Then
                    pSql &= " AND ISNULL(QuotationNo, 0) > 0 AND ComputedQuotationYearWeek >= " & ReadyString(YearWeek) & " AND ComputedQuotationYearWeek <= " & ReadyString(LastWeek) & vbCrLf
                End If

                pSql &= " GROUP BY CustomerID) "
            End If

            pSql &= " ) as v2 " & whereLimit
            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            Return err
        End Try

        '' as IndustryName, " & vbCrLf &
        'Dim pOut As JsonDictionary
        'Dim jss = New JavaScriptSerializer()

        'pOut = New JsonDictionary

        'Dim objOrgDetails As Object = jss.Deserialize(Of Object)(GetJson(GetProspectOrgList()))
        'Dim objPeopleDetails As Object = jss.Deserialize(Of Object)(GetJson(GetProspectPeopleList()))

        'pOut.Add("OrgDetails", objOrgDetails)
        'pOut.Add("PeopleDetails", objPeopleDetails)

        'Return pOut.Serialize()
    End Function

    Function GetListOfEmployees() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim whereSearchContent As String = ""
        Dim additionalwhereSearchContent = ""
        Dim surveyClientID As New List(Of String)

        Try
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
                        ElseIf arrtxtFilter(0) = "Survey" Then
                            additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(SurveyIDs, '') LIKE " & ReadyStringWild(arrtxtFilter(1))
                        ElseIf arrtxtFilter(0) = "Survey" Then
                            Dim pSqlSurvey As String = "SELECT DISTINCT resd.ClientID " & vbCrLf &
                                                        " FROM LibrarySurveyResponse as res " & vbCrLf &
                                                        " INNER JOIN LibrarySurveyRespondent as resd ON res.RespondentID = resd.ID " & vbCrLf &
                                                        " WHERE res.FieldID = " & ReadyString(arrtxtFilter(1))
                            Dim pDTSurvey As DataTable = Common.WebRequest.ProData.Execute(pSqlSurvey)

                            If pDTSurvey.Rows.Count > 0 Then
                                If surveyClientID.Contains(pDTSurvey.Rows(0)("ClientID").ToString()) Then
                                    additionalwhereSearchContent = additionalwhereSearchContent.Substring(0, IIf(additionalwhereSearchContent = "", 0, additionalwhereSearchContent.LastIndexOf("OR")))
                                Else
                                    surveyClientID.Add(pDTSurvey.Rows(0)("ClientID").ToString())
                                    additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(ID, '') = " & ReadyString(pDTSurvey.Rows(0)("ClientID").ToString())
                                End If
                            Else
                                additionalwhereSearchContent = additionalwhereSearchContent.Substring(0, IIf(additionalwhereSearchContent = "", 0, additionalwhereSearchContent.LastIndexOf("OR")))
                            End If
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

            pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    "SELECT ppol.ID as ID, " & vbCrLf &
                    "   CONVERT(varchar, ppol.IsActive) as Status, " & vbCrLf &
                    "   LTRIM(ISNULL(Surname, '') + ', ' + ISNULL(FirstName + ' ', '') + ISNULL(CASE WHEN MiddleName = '' THEN '' ELSE MiddleName + ' ' END, '')) as ClientName, " & vbCrLf &
                    "   (SELECT TOP 1 ISNULL(CASE WHEN ppolemail.EmailAddress = '' THEN '' ELSE ppolemail.EmailAddress END,'') " & vbCrLf &
                    "       FROM LibraryPeopleContactEmail as ppolemail " & vbCrLf &
                    "           WHERE ppolemail.PeopleID = ppol.ID) as Email," & vbCrLf &
                    "   (SELECT TOP 1 " & vbCrLf &
                    "       ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode) + ' ',0) + " & vbCrLf &
                    "       ISNULL('(' + CONVERT(varchar, ppolareacode.CallingCode) + ') ',0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) + " & vbCrLf &
                    "       ISNULL(CASE WHEN ppoltel.ExtensionNo = '' THEN '' ELSE ',' + ppoltel.ExtensionNo END,0) " & vbCrLf &
                    "       FROM LibraryPeopleContactTel as ppoltel " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "               ON ppoltel.CountryID = ppolcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                    "               ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                    "       WHERE ppoltel.PeopleID = ppol.ID) as Telephone," & vbCrLf &
                    "   (SELECT TOP 1 ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                    "       ISNULL(' (' + CONVERT(varchar, ppoltelco.Prefix) + ') ',0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                    "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                    "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                    "           WHERE ppolmobile.PeopleID = ppol.ID) as Mobile, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppoltelco.Prefix),0) + " & vbCrLf &
                    "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                    "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                    "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                    "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                    "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                    "           WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                    "       FOR XML PATH('') " & vbCrLf &
                    "   ), 1, 1,'') FROM LibraryPeopleContactMobile) as MobileNos, " & vbCrLf &
                    "   '' as Website, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org.CompanyName ,'') " & vbCrLf &
                    "                               FROM LibraryOrganizationContactPerson as cp " & vbCrLf &
                    "                                   LEFT JOIN LibraryOrganization as org ON org.ID = cp.OrganizationID" & vbCrLf &
                    "                               WHERE cp.PeopleID = ppol.ID " & vbCrLf &
                    "   FOR XML PATH('')), 1, 1,'') FROM LibraryOrganizationContactPerson) as Affiliation, " & vbCrLf &
                    "   ISNULL(city.Name, '') as City, " & vbCrLf &
                    "   ISNULL(CONVERT(varchar(max), city.ID), '') as CityID, " & vbCrLf &
                    "   ISNULL(ctry.Name, '') as Country, " & vbCrLf &
                    "   CONVERT(varchar, ppol.LastUpdated) as LastUpdated, " & vbCrLf &
                    "   'People' as ProspectType, " & vbCrLf &
                    "   (SELECT ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') " & vbCrLf &
                    "       FROM MobizLibraryPeople " & vbCrLf &
                    "       WHERE MobizLibraryPeople.ID = ppol.UserID) as ModifiedBy, " & vbCrLf &
                    "   '' as ContactPerson, " & vbCrLf &
                    "   (SELECT TOP 1 convert(varchar,LastUpdated) FROM TransCRM WHERE ClientID = ppol.ID ORDER BY LastUpdated DESC) as LastInteractionDate, " & vbCrLf &
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
                    "   ISNULL((SELECT SUM(TotalScore) " & vbCrLf &
                    "       FROM TransSurveyRespondentScore as score " & vbCrLf &
                    "       LEFT JOIN LibrarySurveyRespondent as res ON res.ID = score.RespondentID " & vbCrLf &
                    "       WHERE res.ClientID = ppol.ID " & vbCrLf &
                    "   ), 0) as TotalScore, " & vbCrLf &
                    "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + ISNULL(convert(varchar(max), res.QuestionaireID), '') " & vbCrLf &
                    "       FROM LibrarySurveyRespondent as res " & vbCrLf &
                    "       WHERE res.ClientID = ppol.ID " & vbCrLf &
                    "       FOR XML PATH('')), 1, 1,'') FROM LibrarySurveyRespondent) as SurveyIDs " & vbCrLf &
                    "   FROM LibraryOrganizationEmployee as emp " & vbCrLf &
                    "   INNER JOIN MobizLibraryPeople as ppol on ppol.ID = emp.PeopleID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryPeopleAddress as ppoladd ON ppol.ID = ppoladd.PeopleID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryCities as city ON city.ID = ppoladd.CityID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryCountries as ctry ON ctry.ID = ppoladd.CountryID" & vbCrLf &
                    "   " & vbCrLf &
                    ") as V1 ) as v2 " & whereLimit
            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT
        Catch ex As Exception
            Dim err As New Data.DataTable()
            err.Columns.Add("Error")
            err.Rows.Add(New Object() {ex.Message})
            Return err
        End Try
    End Function

    Function SearchClientContact() As Object
        Dim pSql As String
        Dim pDT As DataTable
        Dim jss = New JavaScriptSerializer()
        Dim pOut As JsonDictionary
        Dim whereLimit = " WHERE RowNum >= 1  AND RowNum <= 50 "
        Dim whereSearch As String = ""
        If IsCountPage = True Then
            whereLimit = ""
        End If

        If Trim(SearchContact) <> "" Then
            Dim whereSearchContent As String = ""

            Dim arr As Object = Split(SearchContact, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent = "" Then
                    whereSearchContent = "ClientName + ' ' + ContactOwner + ' ' + ContactSource + ' ' + Interested + ' ' + IndustryName + ' ' + MobileNos LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Else
                    whereSearchContent += "OR ClientName + ' ' + ContactOwner + ' ' + ContactSource + ' ' + Interested + ' ' + IndustryName + ' ' + MobileNos LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                End If
            Next
            whereSearch = "WHERE (" & whereSearchContent & ")"
        End If

        pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (ORDER BY ClientName ASC) AS RowNum From (SELECT * FROM (SELECT org.ID as ID, " & vbCrLf &
                "   '' as IsFromOrg, '' as CPOrgID, " & vbCrLf &
                "   org.CompanyName as ClientName, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, orgtelco.Prefix),0) + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
                "       FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                "               ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                "               ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                "           WHERE orgmobile.OrganizationID = org.ID " & vbCrLf &
                "       FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM LibraryOrganizationContactMobile) as MobileNos," & vbCrLf &
                "   'Org' as ProspectType, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + InterestedIn " & vbCrLf &
                "       FROM LibraryCRMInterested as interest " & vbCrLf &
                "           WHERE interest.ClientID = org.ID " & vbCrLf &
                "       FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM LibraryCRMInterested) as Interested, " & vbCrLf &
                "   ISNULL(MobizLibraryIndustryClass.Class, '') as IndustryName, " & vbCrLf &
                "   ISNULL(org.ContactOwner, '') as ContactOwner, " & vbCrLf &
                "   ISNULL(org.ContactSource, '') as ContactSource " & vbCrLf &
                "   " & vbCrLf &
                "FROM LibraryOrganization as org LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = org.IndustryClassID) as oTable " & whereSearch & vbCrLf &
                "   " & vbCrLf &
                "UNION ALL  " & vbCrLf &
                "   " & vbCrLf &
                "   SELECT * FROM (SELECT ppol.ID as ID,   " & vbCrLf &
                "   ppol.IsFromOrg as IsFromOrg, " & vbCrLf &
                "   CASE WHEN ppol.IsFromOrg = 1 THEN convert(varchar(MAX), (SELECT TOP 1 OrganizationID FROM LibraryOrganizationContactPerson orgcp WHERE orgcp.PeopleID = ppol.ID)) ELSE '' END as CPOrgID, " & vbCrLf &
                "   ISNULL(FirstName + ' ', '') + ISNULL(CASE WHEN MiddleName = '' THEN '' ELSE MiddleName + ' ' END, '') + ISNULL(Surname, '') as ClientName, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ',' + ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, ppoltelco.Prefix),0) + " & vbCrLf &
                "       ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                "       FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                "           LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                "               ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                "           LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                "               ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                "           WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                "       FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM LibraryPeopleContactMobile) as MobileNos, " & vbCrLf &
                "   'People' as ProspectType, " & vbCrLf &
                "   '' as IndustryName, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + InterestedIn " & vbCrLf &
                "       FROM LibraryCRMInterested as interest " & vbCrLf &
                "           WHERE interest.ClientID = ppol.ID " & vbCrLf &
                "       FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM LibraryCRMInterested) as Interested, " & vbCrLf &
                "   ISNULL(ppol.ContactOwner, '') as ContactOwner, " & vbCrLf &
                "   ISNULL(ppol.ContactSource, '') as ContactSource " & vbCrLf &
                "   FROM LibraryPeople as ppol " & vbCrLf &
                "   LEFT JOIN LibraryPeopleAddress as ppoladd ON ppol.ID = ppoladd.PeopleID " & vbCrLf &
                "   LEFT JOIN MobizLibraryCities as city ON city.ID = ppoladd.CityID " & vbCrLf &
                "   LEFT JOIN MobizLibraryCountries as ctry ON ctry.ID = ppoladd.CountryID" & vbCrLf &
                "   ) as pTable " & whereSearch & vbCrLf &
                "   " & vbCrLf &
                ") as V1 ) as v2" & whereLimit & " ORDER BY ClientName ASC"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Dim a(pDT.Rows.Count - 1) As Object

        If IsCountPage = True Then
            Return pDT
        End If

        For m As Integer = 0 To pDT.Rows.Count - 1
            Dim objTelephone As Object
            If pDT.Rows(m)("ProspectType").ToString() = "Org" Then
                OrgID = pDT.Rows(m)("ID").ToString()
                objTelephone = jss.Deserialize(Of Object)(GetJson(GetProspectOrgContactPerson(OrgID)))
            End If

            a(m) = New With {Key .ID = pDT.Rows(m)("ID"),
                                 .ProspectType = pDT.Rows(m)("ProspectType").ToString(),
                                 .ClientName = pDT.Rows(m)("ClientName").ToString(),
                                 .ContactPerson = objTelephone,
                                 .MobileNos = pDT.Rows(m)("MobileNos").ToString(),
                                 .RowNum = pDT.Rows(m)("RowNum").ToString(),
                             .IsFromOrg = pDT.Rows(m)("IsFromOrg"),
                             .CPOrgID = pDT.Rows(m)("CPOrgID").ToString()}
        Next

        Return a
        'Return Json(New With {Key .data = a}, JsonRequestBehavior.AllowGet)
    End Function

    Function GetProspectPeopleList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID" & vbCrLf
                End With
                With .Add("PeopleFullName", SqlDbType.VarChar)
                    .Expression = "ISNULL(FirstName + ' ', '') + " & vbCrLf &
                                    "ISNULL(CASE WHEN MiddleName = '' THEN '' ELSE MiddleName + ' ' END, '') + " & vbCrLf &
                                    "ISNULL(Surname, '')" & vbCrLf
                End With
                With .Add("Email", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 ISNULL(CASE WHEN ppolemail.EmailAddress = '' THEN '' ELSE ppolemail.EmailAddress END,'') " & vbCrLf &
                                    "FROM LibraryPeopleContactEmail as ppolemail " & vbCrLf &
                                    "WHERE ppolemail.PeopleID = ppol.ID " & vbCrLf &
                                    ")" & vbCrLf
                End With
                With .Add("Telephone", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 " & vbCrLf &
                                    "ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode) + ' ',0) + " & vbCrLf &
                                    "ISNULL('(' + CONVERT(varchar, ppolareacode.CallingCode) + ') ',0) + " & vbCrLf &
                                    "ISNULL(CONVERT(varchar, ppoltel.ContactNo),0) + " & vbCrLf &
                                    "ISNULL(CASE WHEN ppoltel.ExtensionNo = '' THEN '' ELSE '.' + ppoltel.ExtensionNo END,0) " & vbCrLf &
                                    "FROM LibraryPeopleContactTel as ppoltel " & vbCrLf &
                                    "LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                    "   ON ppoltel.CountryID = ppolcountries.ID " & vbCrLf &
                                    "LEFT JOIN MobizLibraryAreaCodes as ppolareacode " & vbCrLf &
                                    "   ON ppoltel.AreaCodeID = ppolareacode.ID " & vbCrLf &
                                    "WHERE ppoltel.PeopleID = ppol.ID " & vbCrLf &
                                    ") " & vbCrLf
                End With
                With .Add("Mobile", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 ISNULL('+' + CONVERT(varchar, ppolcountries.CallingCode),'') + " & vbCrLf &
                                    "ISNULL(' (' + CONVERT(varchar, ppoltelco.Prefix) + ') ',0) + " & vbCrLf &
                                    "ISNULL(CONVERT(varchar, ppolmobile.ContactNo),0) " & vbCrLf &
                                    "FROM LibraryPeopleContactMobile as ppolmobile " & vbCrLf &
                                    "LEFT JOIN MobizLibraryCountries as ppolcountries " & vbCrLf &
                                    "   ON ppolmobile.CountryID = ppolcountries.ID " & vbCrLf &
                                    "LEFT JOIN MobizLibraryTelcoPrefixes as ppoltelco " & vbCrLf &
                                    "   ON ppolmobile.TelCoPrefixID = ppoltelco.ID " & vbCrLf &
                                    "WHERE ppolmobile.PeopleID = ppol.ID " & vbCrLf &
                                    ") " & vbCrLf
                End With
                With .Add("Affiliation", SqlDbType.VarChar)
                    .Expression = "(SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org.CompanyName ,'') " &
                                    "FROM LibraryOrganizationContactPerson as cp " &
                                    "LEFT JOIN LibraryOrganization as org ON org.ID = cp.OrganizationID " & vbCrLf &
                                    "WHERE cp.PeopleID = ppol.ID " & vbCrLf &
                                    "  FOR XML PATH('')" &
                                    "), 1, 1,'')" &
                                    "FROM LibraryOrganizationContactPerson)" & vbCrLf
                End With
                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "ppol.LastUpdated" & vbCrLf
                End With

                With .Add("City", SqlDbType.VarChar)
                    .Expression = "city.Name" & vbCrLf
                End With

                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.Name" & vbCrLf
                End With
            End With

            .SqlFrom = "FROM LibraryPeople as ppol " & vbCrLf &
                        " LEFT JOIN LibraryPeopleAddress as ppoladd ON ppol.ID = ppoladd.PeopleID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCities as city ON city.ID = ppoladd.CityID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as ctry ON ctry.ID = ppoladd.CountryID "

            .SqlWhere = "WHERE IsFromOrg = 0 "

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProspectOrgList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID" & vbCrLf
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName" & vbCrLf
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "ISNULL(org.OfficeAddressBuildingNo + ' ', '') + org.OfficeAddressStreet" & vbCrLf
                End With
                With .Add("Email", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 ISNULL(CASE WHEN orgemail.EmailAddress = '' THEN '' ELSE orgemail.EmailAddress END,'') " & vbCrLf &
                                    "FROM LibraryOrganizationContactEmail as orgemail " & vbCrLf &
                                    "WHERE orgemail.OrganizationID = org.ID " & vbCrLf &
                                    ")" & vbCrLf
                End With
                With .Add("Telephone", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 " & vbCrLf &
                                    "ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode) + ' ',0) + " & vbCrLf &
                                    "ISNULL(' (' + CONVERT(varchar, orgareacode.CallingCode) + ') ',0) + " & vbCrLf &
                                    "ISNULL(CONVERT(varchar, orgtel.ContactNo),0) + " & vbCrLf &
                                    "ISNULL(CASE WHEN orgtel.ExtensionNo = '' THEN '' ELSE '.' + orgtel.ExtensionNo END,0) " & vbCrLf &
                                    "FROM LibraryOrganizationContactTel as orgtel " & vbCrLf &
                                    "LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                                    "   ON orgtel.CountryID = orgcountries.ID " & vbCrLf &
                                    "LEFT JOIN MobizLibraryAreaCodes as orgareacode " & vbCrLf &
                                    "   ON orgtel.AreaCodeID = orgareacode.ID " & vbCrLf &
                                    "WHERE orgtel.OrganizationID = org.ID " & vbCrLf &
                                    ") " & vbCrLf
                End With
                With .Add("Mobile", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 ISNULL('+' + CONVERT(varchar, orgcountries.CallingCode),'') + " & vbCrLf &
                                    "ISNULL('(' + CONVERT(varchar, orgtelco.Prefix) + ') ',0) + " & vbCrLf &
                                    "ISNULL(CONVERT(varchar, orgmobile.ContactNo),0) " & vbCrLf &
                                    "FROM LibraryOrganizationContactMobile as orgmobile " & vbCrLf &
                                    "LEFT JOIN MobizLibraryCountries as orgcountries " & vbCrLf &
                                    "   ON orgmobile.CountryID = orgcountries.ID " & vbCrLf &
                                    "LEFT JOIN MobizLibraryTelcoPrefixes as orgtelco " & vbCrLf &
                                    "   ON orgmobile.TelCoPrefixID = orgtelco.ID " & vbCrLf &
                                    "WHERE orgmobile.OrganizationID = org.ID " & vbCrLf &
                                    ") " & vbCrLf
                End With
                With .Add("Website", SqlDbType.Date)
                    .Expression = "(SELECT TOP 1 ISNULL(orgwebsite.Website ,'') " & vbCrLf &
                                    "FROM LibraryOrganizationContactWebsite  as orgwebsite " & vbCrLf &
                                    "WHERE orgwebsite.OrganizationID = org.ID " & vbCrLf &
                                    ") " & vbCrLf
                End With
                With .Add("Affiliation", SqlDbType.VarChar)
                    .Expression = "(SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ ISNULL(org2.CompanyName ,'') " &
                                    "FROM LibraryOrganizationAffiliates   as orgaff " &
                                    "LEFT JOIN LibraryOrganization as org2 " & vbCrLf &
                                    "   ON org2.ID = orgaff.AffiliatesID " & vbCrLf &
                                    "WHERE orgaff.OrganizationID = org.ID " & vbCrLf &
                                    "  FOR XML PATH('')" &
                                    "), 1, 1,'')" &
                                    "FROM LibraryOrganizationAffiliates)" & vbCrLf
                    '.Expression = "(SELECT TOP 1 ISNULL(ppol.FirstName + ' ' ,'') + " & vbCrLf &
                    '                "ISNULL(CASE WHEN ppol.MiddleName  = '' THEN '' ELSE ppol.MiddleName + ' ' END,0) + " & vbCrLf &
                    '                "ISNULL(ppol.Surname ,'') " & vbCrLf &
                    '                "FROM LibraryOrganizationContactPerson   as orgcontactperson " & vbCrLf &
                    '                "LEFT JOIN LibraryPeople as ppol " & vbCrLf &
                    '                "   ON ppol.ID = orgcontactperson.PeopleID " & vbCrLf &
                    '                "WHERE orgcontactperson.OrganizationID = org.ID " & vbCrLf &
                    '                ")" & vbCrLf
                End With
                With .Add("City", SqlDbType.VarChar)
                    .Expression = "(SELECT city.Name" & vbCrLf &
                                    "FROM MobizLibraryCities as city " & vbCrLf &
                                    "LEFT JOIN LibraryOrganization as org2 " & vbCrLf &
                                    "   ON org2.OfficeAddressCityID = city.ID " & vbCrLf &
                                    "WHERE org2.ID= org.ID " & vbCrLf &
                                    ")" & vbCrLf
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "(SELECT country.Name" & vbCrLf &
                                    "FROM MobizLibraryCountries as country " & vbCrLf &
                                    "LEFT JOIN LibraryOrganization as org3 " & vbCrLf &
                                    "   ON org3.OfficeAddressCountryID = country.ID " & vbCrLf &
                                    "WHERE org3.ID= org.ID " & vbCrLf &
                                    ")" & vbCrLf
                End With
                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "org.LastUpdated" & vbCrLf
                End With
            End With

            .SqlFrom = "FROM LibraryOrganization as org " & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveNewOrgProspect() As String
        Dim pSql As String
        Dim strOrgID As String
        Dim p As New InsertUpdate("LibraryOrganization")
        Dim pDT As DataTable
        Dim isImportCrmToCustomer As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.AutoImportCRMRecordsToCustomer)
        Dim isImportCrmToSupplier As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.AutoImportCRMRecordsToSupplier)

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("CompanyName") = ReadyString(CompanyName)
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
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                pSql = "SELECT * FROM LibraryOrganization WHERE CompanyName = " & ReadyString(CompanyName)
                pDT = Common.WebRequest.ProData.Execute(pSql)
                'Check if Company Name exist
                If pDT.Rows.Count > 0 Then
                    Return "Company Name '<b>" & CompanyName & "</b>' was already exist"
                End If

                strOrgID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strOrgID)
                pSql = p.GetInsert()

            Else
                strOrgID = CompanyID
                pSql = p.GetUpdate(" ID = " & ReadyString(CompanyID))

            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If isImportCrmToCustomer Or isImportCrmToSupplier Then
                p = New InsertUpdate("MobizLibraryOrganization")

                p("CompanyName") = ReadyString(CompanyName)
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
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                If CompanyID = "" Then
                    p("ID") = ReadyString(strOrgID)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(CompanyID)))
                End If
            End If

            Dim codenumber As String = CStr2(New OrganizationShort().GenerateCodeNumber(False))

            If isImportCrmToCustomer Then
                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("OrganizationID") = ReadyString(strOrgID)
                p("ActivationDate") = "GetDate()"
                p("DeActivated") = If(Status = "Active", 0, 1)
                p("IsBan") = ReadyString("0")
                p("LastUpdated") = "GetDate()"

                pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(strOrgID)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count = 0 Then
                    p("CustomerCode") = ReadyString(codenumber)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0)("ID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)
            End If

            If isImportCrmToSupplier Then
                p = New InsertUpdate("LibraryOrganizationSupplier")

                p("SupplierID") = ReadyString(strOrgID)
                p("ActivationDate") = "getdate()"
                p("DeActivated") = If(Status = "Active", 0, 1)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                pSql = "SELECT ID FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(strOrgID)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count = 0 Then
                    p("VendorCode") = ReadyString(codenumber)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0)("ID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)
            End If

            For Each address As Object In ObjAddress
                pSql = "SELECT ID FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(strOrgID)

                p = New InsertUpdate("MobizLibraryOrganizationFOWST")
                p("OrganizationID") = ReadyString(strOrgID)
                p("IsActive") = 1
                p("StreetNumber") = ReadyString(address("CompleteAddress"))

                If address("State") = "new" Then
                    p("Name") = ReadyString("Address " & (CInt2(Common.WebRequest.ProData.Execute(pSql).Rows.Count) + 1))
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                ElseIf address("State") = "edit" Then
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(address("ID").ToString())))
                Else
                    p("IsActive") = 0
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(address("ID").ToString())))
                End If
            Next address

            For Each orgaff As Object In ObjOrgAff
                p = New InsertUpdate("LibraryOrganizationAffiliates")

                pSql = "SELECT * FROM LibraryOrganizationAffiliates WHERE OrganizationID = " & ReadyString(strOrgID) & vbCrLf &
                        " AND AffiliatesID = " & ReadyString(orgaff("OrgID").ToString())
                pDT = Common.WebRequest.ProData.Execute(pSql)

                p("AffiliatesID") = ReadyString(orgaff("OrgID").ToString())
                p("OrganizationID") = ReadyString(strOrgID)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If orgaff("State").ToString() = "Add" Then
                    If pDT.Rows.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                ElseIf orgaff("State").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOrganizationAffiliates WHERE ID = " & ReadyString(orgaff("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next orgaff

            For Each orgaff As Object In ObjAffiliatedPerson
                p = New InsertUpdate("LibraryOrganizationContactPerson")

                pSql = "SELECT * FROM LibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(strOrgID) & vbCrLf &
                        " AND PeopleID = " & ReadyString(orgaff("PeopleID").ToString())
                pDT = Common.WebRequest.ProData.Execute(pSql)

                p("PeopleID") = ReadyString(orgaff("PeopleID").ToString())
                p("OrganizationID") = ReadyString(strOrgID)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))
                p("IsUseForPersonOrgAffiliates") = ReadyString(1)

                If orgaff("State").ToString() = "Add" Then
                    If pDT.Rows.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                ElseIf orgaff("State").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOrganizationContactPerson WHERE ID = " & ReadyString(orgaff("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next orgaff

            For Each tel As Object In ObjTelephone
                p = New InsertUpdate("LibraryOrganizationContactTel")

                p("OrganizationID") = ReadyString(strOrgID)
                p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                p("ContactType") = ReadyString(tel("ContactType").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactTel")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                    p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                    p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                    p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                    p("ContactType") = ReadyString(tel("ContactType").ToString())
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                p = New InsertUpdate("LibraryOrganizationContactMobile")

                p("OrganizationID") = ReadyString(strOrgID)
                p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                    p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                    p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                p = New InsertUpdate("LibraryOrganizationContactEmail")

                p("OrganizationID") = ReadyString(strOrgID)
                p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("EmailType") = ReadyString(email("EmailType").ToString())
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

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("EmailType") = ReadyString(email("EmailType").ToString())
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
                p = New InsertUpdate("LibraryOrganizationContactWebsite")

                p("OrganizationID") = ReadyString(strOrgID)
                p("Website") = ReadyString(website("Website").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactWebsite")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("Website") = ReadyString(website("Website").ToString())
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

            For Each cpeople As Object In ObjContactPerson
                Dim ppolID As String = Guid.NewGuid.ToString()

                p = New InsertUpdate("LibraryPeople")

                p("FirstName") = ReadyString(CStr2(cpeople("Firstname")))
                p("MiddleName") = ReadyString(CStr2(cpeople("Middlename")))
                p("Surname") = ReadyString(CStr2(cpeople("Lastname")))
                p("Position") = ReadyString(CStr2(cpeople("Position")))
                p("PositionDescription") = ReadyString(CStr2(cpeople("PositionDescription")))
                p("IsFromOrg") = ReadyString(1)

                If cpeople("ContactPersonState").ToString() = "Add" Then
                    p("ID") = ReadyString(ppolID)
                    pSql = p.GetInsert()

                ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                    ppolID = cpeople("PeopleID")
                    pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("PeopleID")))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryPeople")

                    p("FirstName") = ReadyString(CStr2(cpeople("Firstname")))
                    p("MiddleName") = ReadyString(CStr2(cpeople("Middlename")))
                    p("Surname") = ReadyString(CStr2(cpeople("Lastname")))
                    p("Position") = ReadyString(CStr2(cpeople("Position")))
                    p("PositionDescription") = ReadyString(CStr2(cpeople("PositionDescription")))
                    p("IsFromOrg") = ReadyString(1)

                    If cpeople("ContactPersonState").ToString() = "Add" Then
                        p("ID") = ReadyString(ppolID)
                        pSql = p.GetInsert()

                    ElseIf cpeople("ContactPersonState").ToString() = "Edit" Then
                        ppolID = cpeople("PeopleID")
                        pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("PeopleID")))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                p = New InsertUpdate("LibraryPeopleAddress")

                p("PeopleID") = ReadyString(ppolID)
                p("HouseBuildingNo") = If(cpeople("HouseBuildingNo") = Nothing, "NULL", ReadyString(cpeople("HouseBuildingNo").ToString()))
                p("Street") = If(cpeople("Street") = Nothing, "NULL", ReadyString(cpeople("Street").ToString()))
                p("CityID") = If(cpeople("City") = "", "NULL", ReadyString(cpeople("City").ToString()))
                p("CountryID") = If(cpeople("Country") = "", "NULL", ReadyString(cpeople("Country").ToString()))
                p("AddressType") = If(cpeople("TypeAddress") = Nothing, "NULL", ReadyString(cpeople("TypeAddress").ToString()))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If cpeople("AddressID") = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("AddressID")))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryPeopleAddress")

                    p("PeopleID") = ReadyString(ppolID)
                    p("HouseBuildingNo") = If(cpeople("HouseBuildingNo") = Nothing, "NULL", ReadyString(cpeople("HouseBuildingNo").ToString()))
                    p("Street") = If(cpeople("Street") = Nothing, "NULL", ReadyString(cpeople("Street").ToString()))
                    p("CityID") = If(cpeople("City") = "", "NULL", ReadyString(cpeople("City").ToString()))
                    p("CountryID") = If(cpeople("Country") = "", "NULL", ReadyString(cpeople("Country").ToString()))
                    p("AddressType") = If(cpeople("TypeAddress") = Nothing, "NULL", ReadyString(cpeople("TypeAddress").ToString()))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If cpeople("AddressID") = "" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(cpeople("AddressID")))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                p = New InsertUpdate("LibraryOrganizationContactPerson")

                p("OrganizationID") = ReadyString(strOrgID)
                p("PeopleID") = ReadyString(ppolID)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))
                p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                If cpeople("ContactPersonState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf cpeople("ContactPersonState").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOrganizationContactPerson WHERE ID = " & ReadyString(cpeople("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                If isImportCrmToCustomer Or isImportCrmToSupplier Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                    p("OrganizationID") = ReadyString(strOrgID)
                    p("PeopleID") = ReadyString(ppolID)
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = ReadyString(CDate(DateTime))
                    p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                    If cpeople("ContactPersonState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    ElseIf cpeople("ContactPersonState").ToString() = "Remove" Then
                        pSql = "DELETE FROM LibraryOrganizationContactPerson WHERE ID = " & ReadyString(cpeople("ID").ToString())
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                End If

                Try
                    For Each tel As Object In cpeople("Tel")
                        p = New InsertUpdate("LibraryPeopleContactTel")

                        p("PeopleID") = ReadyString(ppolID)
                        p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                        p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                        p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                        p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                        p("ContactType") = ReadyString(tel("ContactType").ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                        If isImportCrmToCustomer Or isImportCrmToSupplier Then
                            p = New InsertUpdate("MobizLibraryPeopleContactTel")

                            p("PeopleID") = ReadyString(ppolID)
                            p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                            p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                            p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                            p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                            p("ContactType") = ReadyString(tel("ContactType").ToString())
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                End Try

                Try
                    For Each mobile As Object In cpeople("Mobile")
                        p = New InsertUpdate("LibraryPeopleContactMobile")

                        p("PeopleID") = ReadyString(ppolID)
                        p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                        p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                        p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                        If isImportCrmToCustomer Or isImportCrmToSupplier Then
                            p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                            p("PeopleID") = ReadyString(ppolID)
                            p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                            p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                            p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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

                End Try

                Try
                    For Each email As Object In cpeople("Email")
                        p = New InsertUpdate("LibraryPeopleContactEmail")

                        p("PeopleID") = ReadyString(ppolID)
                        p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("EmailType") = ReadyString(email("EmailType").ToString())
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If email("EmailState").ToString() = "Add" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                        ElseIf email("EmailState").ToString() = "Edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                        ElseIf email("EmailState").ToString() = "Remove" Then
                            pSql = "DELETE FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        If isImportCrmToCustomer Or isImportCrmToSupplier Then
                            p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                            p("PeopleID") = ReadyString(ppolID)
                            p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            p("EmailType") = ReadyString(email("EmailType").ToString())
                            p("LastUpdated") = ReadyString(CDate(DateTime))

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
                    Next email
                Catch ex As Exception

                End Try
            Next cpeople

            Common.WebRequest.ProData.CommitTransaction()

            Return strOrgID.ToString()

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveNewPersonProspect() As String
        Dim pSql As String
        Dim strPersonID As String
        Dim p As New InsertUpdate("LibraryPeople")
        Dim pDT As DataTable
        Dim isImportCrmToCustomer As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.AutoImportCRMRecordsToCustomer)
        Dim codenumber As String = CStr2(New OrganizationShort().GenerateCodeNumber(False))

        Try
            Common.WebRequest.ProData.BeginTransaction()

            If CurrentModule = "dtr" Then
                p = New InsertUpdate("MobizLibraryPeople")
            Else
                p = New InsertUpdate("LibraryPeople")
            End If

            p("FirstName") = ReadyString(FirstName)
            p("MiddleName") = ReadyString(MiddleName)
            p("Surname") = ReadyString(LastName)
            p("DateOfBirth") = IIf(Birthday = "", "NULL", ReadyString(Birthday))
            p("Position") = ReadyString(Position)
            p("PositionDescription") = ReadyString(PositionDescription)
            p("IsFromOrg") = ReadyString(0)
            p("IsActive") = If(Status = "Active", "1", "0")
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(DateTime))

            If PersonID = "" Then
                strPersonID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strPersonID)
                pSql = p.GetInsert()

            Else
                strPersonID = PersonID
                pSql = p.GetUpdate(" ID = " & ReadyString(PersonID))

            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                p = New InsertUpdate("MobizLibraryOrganization")

                p("ID") = ReadyString(strPersonID)
                p("PersonFirstName") = ReadyString(FirstName)
                p("PersonMiddleName") = ReadyString(MiddleName)
                p("PersonSurname") = ReadyString(LastName)
                p("PersonDateOfBirth") = IIf(Birthday = "", "NULL", ReadyString(Birthday))
                p("IsOrganization") = "0"
                p("IsActive") = If(Status = "Active", "1", "0")
                p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
                p("LastUpdated") = ReadyString(CDate(DateTime))

                pSql = "SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(strPersonID)

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(strPersonID))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("OrganizationID") = ReadyString(strPersonID)
                p("ActivationDate") = "GetDate()"
                p("DeActivated") = "0"
                p("IsBan") = ReadyString("0")
                p("LastUpdated") = "GetDate()"

                pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(strPersonID)

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("CustomerCode") = ReadyString(codenumber)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(strPersonID))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If CurrentModule = "dtr" Then
                p = New InsertUpdate("MobizLibraryPeopleAddress")
            Else
                p = New InsertUpdate("LibraryPeopleAddress")
            End If

            p("PeopleID") = ReadyString(strPersonID)
            p("HouseBuildingNo") = ReadyString(HouseBuildingNo)
            p("Street") = ReadyString(Street)
            p("CityID") = If(City = "", "NULL", ReadyString(City))
            p("Region") = ReadyString(Region)
            p("CountryID") = If(City = "", "NULL", ReadyString(Country))
            p("Island") = ReadyString(Island)
            p("AddressType") = ReadyString(TypeAddress)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(CDate(DateTime))

            If AddressID = "" Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(AddressID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                p = New InsertUpdate("MobizLibraryPeopleAddress")

                p("PeopleID") = ReadyString(strPersonID)
                p("HouseBuildingNo") = ReadyString(HouseBuildingNo)
                p("Street") = ReadyString(Street)
                p("CityID") = If(City = "", "NULL", ReadyString(City))
                p("Region") = ReadyString(Region)
                p("CountryID") = If(Country = "", "NULL", ReadyString(Country))
                p("Island") = ReadyString(Island)
                p("AddressType") = ReadyString(TypeAddress)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If AddressID = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(AddressID))
                End If
            End If

            For Each orgaff As Object In ObjPersonOrgAff
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")
                Else
                    p = New InsertUpdate("LibraryOrganizationContactPerson")
                End If

                If CStr2(orgaff("OrgID")) <> "" Then
                    pSql = "SELECT * FROM " & IIf(CurrentModule = "dtr", "MobizLibraryOrganizationContactPerson", "LibraryOrganizationContactPerson") & " WHERE OrganizationID = " & ReadyString(orgaff("OrgID").ToString()) & vbCrLf &
                        " AND PeopleID = " & ReadyString(strPersonID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("OrganizationID") = ReadyString(orgaff("OrgID").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))
                p("IsUseForPersonOrgAffiliates") = ReadyString(1)

                If orgaff("State").ToString() = "Add" Then
                    If pDT.Rows.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                ElseIf orgaff("State").ToString() = "Remove" Then
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryOrganizationContactPerson", "LibraryOrganizationContactPerson") & " WHERE ID = " & ReadyString(orgaff("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next orgaff

            For Each tel As Object In ObjTelephone
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactTel")
                Else
                    p = New InsertUpdate("LibraryPeopleContactTel")
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If tel("TelState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf tel("TelState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(tel("TelID").ToString()))

                ElseIf tel("TelState").ToString() = "Remove" Then
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryPeopleContactTel", "LibraryPeopleContactTel") & " WHERE ID = " & ReadyString(tel("TelID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactTel")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("CountryID") = ReadyString(tel("CountryCodeID").ToString())
                    p("AreaCodeID") = ReadyString(tel("AreaCodeID").ToString())
                    p("ContactNo") = ReadyString(tel("TelephoneNo").ToString())
                    p("ExtensionNo") = ReadyString(tel("ExtensionNo").ToString())
                    p("ContactType") = If(tel("ContactType") = Nothing, ReadyString(""), ReadyString(tel("ContactType").ToString()))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                End If
            Next tel

            For Each mobile As Object In ObjMobile
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactMobile")
                Else
                    p = New InsertUpdate("LibraryPeopleContactMobile")
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("ContactType") = If(mobile("ContactType") = Nothing, ReadyString(""), ReadyString(mobile("ContactType").ToString()))
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If mobile("MobileState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf mobile("MobileState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(mobile("MobileID").ToString()))

                ElseIf mobile("MobileState").ToString() = "Remove" Then
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryPeopleContactMobile", "LibraryPeopleContactMobile") & " WHERE ID = " & ReadyString(mobile("MobileID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("CountryID") = ReadyString(mobile("CountryCodeID").ToString())
                    p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("MobileNo"))("ID"))
                    p("ContactNo") = ReadyString(mobile("MobileNo").ToString())
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                End If
            Next mobile

            For Each email As Object In ObjEmail
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactEmail")
                Else
                    p = New InsertUpdate("LibraryPeopleContactEmail")
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("EmailType") = If(email("EmailType") = Nothing, ReadyString(""), ReadyString(email("EmailType").ToString()))
                p("LastUpdated") = ReadyString(CDate(DateTime))

                If email("EmailState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf email("EmailState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                ElseIf email("EmailState").ToString() = "Remove" Then
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryPeopleContactEmail", "LibraryPeopleContactEmail") & " WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("EmailAddress") = ReadyString(email("EmailAdd").ToString())
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("EmailType") = If(email("EmailType") = Nothing, ReadyString(""), ReadyString(email("EmailType").ToString()))
                    p("LastUpdated") = ReadyString(CDate(DateTime))

                    If email("EmailState").ToString() = "Add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()

                    ElseIf email("EmailState").ToString() = "Edit" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(email("EmailAddID").ToString()))

                    ElseIf email("EmailState").ToString() = "Remove" Then
                        pSql = "DELETE FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("EmailAddID").ToString())
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next email

            For Each elem As Object In ObjPersonElementary
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleEducation")
                Else
                    p = New InsertUpdate("LibraryPeopleEducation")
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("AcademicLevel") = ReadyString(elem("Level"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryPeopleEducation", "LibraryPeopleEducation") & " WHERE ID = " & ReadyString(elem("ID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleEducation")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("AcademicLevel") = ReadyString(elem("Level"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                End If
            Next elem

            For Each hs As Object In ObjPersonHighSchool
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleEducation")
                Else
                    p = New InsertUpdate("LibraryPeopleEducation")
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("AcademicLevel") = ReadyString(hs("Level"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryPeopleEducation", "LibraryPeopleEducation") & " WHERE ID = " & ReadyString(hs("ID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleEducation")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("AcademicLevel") = ReadyString(hs("Level"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                End If
            Next hs

            For Each college As Object In ObjPersonCollege
                If CurrentModule = "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleEducation")
                Else
                    p = New InsertUpdate("LibraryPeopleEducation")
                End If

                p("PeopleID") = ReadyString(strPersonID)
                p("AcademicLevel") = ReadyString(college("Level"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                    pSql = "DELETE FROM " & IIf(CurrentModule = "dtr", "MobizLibraryPeopleEducation", "LibraryPeopleEducation") & " WHERE ID = " & ReadyString(college("ID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If isImportCrmToCustomer And CurrentModule <> "dtr" Then
                    p = New InsertUpdate("MobizLibraryPeopleEducation")

                    p("PeopleID") = ReadyString(strPersonID)
                    p("AcademicLevel") = ReadyString(college("Level"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
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
                End If
            Next college

            Common.WebRequest.ProData.CommitTransaction()

            Return strPersonID.ToString()

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveColdCall() As String
        Dim pSql As String
        Dim p As New InsertUpdate("TransCRM")

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("ClientID") = ReadyString(ObjClient("ID").ToString())
            p("ActionName") = ReadyString("Call")
            p("InteractionStart") = If(CallStartDate = "", "NULL", ReadyString(CDate(CallStartDate)))
            p("InteractionEnd") = If(CallEndDate = "", "NULL", ReadyString(CDate(CallEndDate)))
            p("CallDuration") = ReadyString(CallDuration)
            p("Notes") = ReadyString(CallNotes)
            p("IsOrg") = ReadyString(IsOrg)
            p("LastUpdated") = ReadyString(CDate(DateTime))
            p("TaggedType") = ReadyString(Common.WebRequest.Data("TaggedType"))
            p("RelatedID") = If(Common.WebRequest.Data("TaggedID") = "", "NULL", ReadyString(Common.WebRequest.Data("TaggedID").ToString()))

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            AddActivityQuota("Call", WebSession.Instance.PeopleUserID, CDate(DateTime))

            If CallNotes <> "" Then
                AddActivityQuota("Notes", WebSession.Instance.PeopleUserID, CDate(DateTime))
            End If

            Return "Saved"

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveToDo() As String
        Dim pSql As String = ""
        Dim todoid As String = ""
        Dim p As New InsertUpdate("MobizLibraryOrganization")

        Try
            Common.WebRequest.ProData.BeginTransaction()

            For Each todo As Object In ObjToDo
                Dim clientid As String = todo("ClientID").ToString()

                If String.IsNullOrWhiteSpace(clientid) Then
                    clientid = "4f50f808-16f2-43cc-88fa-69b2dff28803" ' kapag walay gipili na Customer/Supplier ID, meaning, internal To-Do

                    pSql = "SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(clientid)
                    Dim pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
                    If pDR Is Nothing Then
                        p("ID") = ReadyString(clientid)
                        p("CompanyName") = ReadyString("Internal To-Do")
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                End If

                p = New InsertUpdate("TransCRM")

                p("ClientID") = ReadyString(clientid)
                p("ToDoTitle") = ReadyString(todo("ToDoTitle").ToString())
                p("ToDo") = ReadyString(todo("ToDo").ToString())
                p("ActionName") = ReadyString("To-Do")
                p("ToDoFinish") = ReadyString(todo("ToDoFinish").ToString())
                p("ToDoDueDate") = If(todo("ToDoDueDate") = "", "NULL", ReadyString(CDate(todo("ToDoDueDate").ToString())))
                p("ToDoOrder") = ReadyString(todo("ToDoOrder").ToString())
                p("IsOrg") = todo("IsOrg").ToString()
                p("Urgency") = ReadyString(todo("Urgency").ToString())
                p("Importance") = ReadyString(todo("Importance").ToString())
                p("LastUpdated") = ReadyString(CDate(DateTime))
                p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                If todo.ContainsKey("TaggedType") = True Then
                    p("TaggedType") = ReadyString(todo("TaggedType"))
                Else
                    p("TaggedType") = ReadyString("General")
                End If

                If todo.ContainsKey("TaggedID") = True Then
                    p("RelatedID") = If(todo("TaggedID") = "", "NULL", ReadyString(todo("TaggedID").ToString()))
                End If

                If todo.ContainsKey("TodoCategoryID") = True Then
                    p("TodoCategoryID") = If(todo("TodoCategoryID") = "", "NULL", ReadyString(todo("TodoCategoryID").ToString()))
                End If

                If String.IsNullOrWhiteSpace(todo("ToDoID")) Then
                    todoid = Guid.NewGuid.ToString()
                Else
                    todoid = todo("ToDoID").ToString()
                End If

                If todo("State").ToString() = "Add" Then
                    GenNumberType = "Todo"
                    p("ID") = ReadyString(todoid)
                    p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("CreatedOn") = ReadyString(CDate(DateTime))
                    p("TodoNumber") = ReadyString(GenerateNumber())
                    pSql = p.GetInsert()

                ElseIf todo("State").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(todoid))

                ElseIf todo("State").ToString() = "Remove" Then
                    p("IsDelete") = ReadyString("1")
                    pSql = p.GetUpdate(" ID = " & ReadyString(todoid))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If todo.ContainsKey("AssignedEmployee") = True Then
                    For Each emp As Object In todo("AssignedEmployee")
                        p = New InsertUpdate("TransCRMTodoAssignedEmployee")

                        p("TodoID") = ReadyString(todoid)
                        p("EmployeeID") = ReadyString(emp("ID"))
                        p("LastUpdated") = ReadyString(CDate(DateTime))

                        If emp("State") = "new" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                        ElseIf emp("State") = "removed" Then
                            pSql = "DELETE FROM TransCRMTodoAssignedEmployee WHERE TodoID = " & ReadyString(todoid) & " AND EmployeeID = " & ReadyString(emp("EmployeeID"))
                        End If

                        Common.WebRequest.ProData.Execute(pSql)
                    Next
                End If
            Next todo

            Common.WebRequest.ProData.CommitTransaction()

            Dim pOut As New JsonDictionary

            pOut.Add("Results", "Saved")
            pOut.Add("ID", todoid)

            Return pOut.Serialize

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveTakeANote() As String
        Dim pSql As String
        Dim p As New InsertUpdate("TransCRM")
        Dim noteID As String
        Dim result As String = ""

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p = New InsertUpdate("TransCRM")

            p("ClientID") = ReadyString(ClientID)
            p("Notes") = ReadyString(Note)
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(DateTime)
            p("TaggedType") = ReadyString(Common.WebRequest.Data("TaggedType"))
            p("RelatedID") = If(Common.WebRequest.Data("TaggedID") = "", "NULL", ReadyString(Common.WebRequest.Data("TaggedID").ToString()))

            If Common.WebRequest.AllData("Ticket").length > 0 Then
                p("ActionName") = ReadyString("Comment")
                result = SaveTicket(Common.WebRequest.AllData("Ticket")(0)) 'GUID||TicketNumber
                Dim id As String = result.Split("||")(0)
                If IsGuid(id) = False Then
                    Return result
                End If

                p("RelatedID") = ReadyString(id)
            Else
                p("ActionName") = ReadyString("Note")
            End If

            If Common.WebRequest.Data("State") = "add" Then
                noteID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(noteID)
                pSql = p.GetInsert()

                AddActivityQuota("Notes", WebSession.Instance.PeopleUserID, CDate(DateTime))
            ElseIf Common.WebRequest.Data("State") = "edit" Then
                noteID = Common.WebRequest.Data("NoteID")
                pSql = p.GetUpdate(" ID = " & ReadyString(noteID))
            Else
                pSql = "DELETE FROM TransCRM WHERE ID = " & ReadyString(Common.WebRequest.Data("NoteID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If Trim(Note) <> "" Then
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                'For i As Integer = 0 To Attachments.Length - 1
                '    Dim attachStatus As String = SaveAttachFile(Attachments(i).Files, noteID, Attachments(i)("ID"))

                '    If attachStatus <> "saved" Then
                '        Return attachStatus
                '    End If
                'Next

                If result = "" Then
                    'For notes only
                    result = CStr2(noteID)
                Else
                    result += "||" & CStr2(noteID)
                End If
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return result

        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function SaveSchedAnAction() As String
        Dim pSql As String
        Dim p As New InsertUpdate("TransCRM")
        Dim colName As String

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p = New InsertUpdate("TransCRM")

            p("ClientID") = ReadyString(ClientID)
            p("Location") = ReadyString(Location)
            p("Notes") = ReadyString(Note)
            p("ActionName") = ReadyString("Scheduled Action")
            p("SchedActionName") = ReadyString(NextActionName)
            p("Subject") = ReadyString(Subject)
            p("IsActionAllDay") = ReadyString(IsAllDay)
            p("SchedActionDateFrom") = ReadyString(DateTimeFrom)
            p("SchedActionDateTo") = ReadyString(DateTimeTo)
            p("SchedActionTeamID") = If(TeamAssign = "", "NULL", ReadyString(TeamAssign))
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = ReadyString(DateTime)

            If NextActionName = "Call" Then
                colName = "ScheduledCall"
            ElseIf NextActionName = "Email" Then
                colName = "ScheduledEmail"
            ElseIf NextActionName = "SMS" Then
                colName = "ScheduledSMS"
            ElseIf NextActionName = "Physical Meeting" Or NextActionName = "Meeting" Then
                colName = "ScheduledPhysicalMeeting"
            ElseIf NextActionName = "Virtual Meeting" Then
                colName = "ScheduledVirtualMeeting"
            End If

            If Common.WebRequest.Data("State") = "add" Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()

                AddActivityQuota(colName, WebSession.Instance.PeopleUserID, CDate(DateTime))

            ElseIf Common.WebRequest.Data("State") = "edit" Then
                pSql = p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("SchedAnActionID")))
            Else
                'pSql = "DELETE FROM TransCRM WHERE ID = " & ReadyString(Common.WebRequest.Data("SchedAnActionID"))
                p("IsDelete") = ReadyString("1")
                pSql = p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("SchedAnActionID")))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "Saved"

        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Public Sub ConnectToDevice()

        'initialize all values
        Dim COMMPORT As String = "COM3"

        SMSPort = New SerialPort

        Try
            With SMSPort

                'For Each p As String In IO.Ports.SerialPort.GetPortNames
                '    Dim sp As New IO.Ports.SerialPort(p, 9600, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)
                '    sp.ReadTimeout = 1000
                '    sp.Open()
                '    sp.WriteLine("FOO")
                '    Try
                '        sp.ReadLine()
                '        COMMPORT = sp.PortName
                '        'good port here
                '        Exit For
                '    Catch ex As TimeoutException
                '        Stop
                '    End Try
                '    sp.Close()
                'Next

                .PortName = COMMPORT

                .BaudRate = 19200

                .Parity = Parity.None

                .DataBits = 8

                .StopBits = StopBits.One

                .Handshake = Handshake.RequestToSend

                .DtrEnable = True

                .RtsEnable = True

                .NewLine = vbCrLf

                .Open()
            End With
        Catch ex As Exception

        End Try


    End Sub

    Public Function SendSMS() As Boolean

        'SaveSmS(message, sender)
        Dim dtMessages As DataTable

        ConnectToDevice()

        Try
            If SMSPort.IsOpen = True Then
                'sending AT commands

                dtMessages = GetAllUnsentMessages()

                For i As Integer = 0 To dtMessages.Rows.Count - 1

                    SMSPort.WriteLine("AT")
                    Thread.Sleep(1000)
                    SMSPort.WriteLine("AT+CMGF=1" & vbCrLf) 'set command message format to text mode(1)
                    Thread.Sleep(1000)
                    SMSPort.Write("AT+CMGS=" & Chr(34) & dtMessages.Rows(i).Item("SendTo") & Chr(34) & vbCrLf)
                    Thread.Sleep(1000)
                    SMSPort.Write(dtMessages.Rows(i).Item("Message") & Chr(26))
                    Thread.Sleep(5000)

                    Dim pSql As String

                    pSql = "Update TransSmsMessages set SenttoModemTries=1 where ID = " & ReadyString(dtMessages.Rows(i).Item("ID"))
                    Common.WebRequest.ProData.Execute(pSql)

                    AddActivityQuota("SMS", WebSession.Instance.PeopleUserID, CDate(DateTime))
                Next

                Close()

            End If
        Catch ex As Exception
            AppendToLog(ex.Message)
        End Try


    End Function

    Public Function SaveSmS(ByVal Message As String, ByVal Sender As Object) As String

        Dim pSql As String
        Dim pIU As InsertUpdate

        Try
            Common.WebRequest.ProData.BeginTransaction()
            For Each objSMS As Object In Sender

                For i As Integer = 0 To objSMS("Number").length - 1
                    ID = Guid.NewGuid.ToString

                    pIU = New InsertUpdate("TransSmsMessages")

                    pIU("SendToName") = ReadyString(objSMS("Name"))
                    pIU("SendTo") = ReadyString(objSMS("Number")(i))
                    pIU("SendFrom") = ReadyString(WebSession.Instance.PeopleUserID)
                    pIU("Message") = ReadyString(Message)
                    pIU("LastUpdated") = ReadyString(CDate(DateTime))
                    pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    pIU("TransSMSID") = ReadyString(ID)
                    'pIU("ModemID") = ReadyString(vDateTo)

                    pSql = pIU.GetInsert

                    Common.WebRequest.ProData.Execute(pSql)

                    pIU = New InsertUpdate("TransCRM")
                    pIU("ClientID") = ReadyString(objSMS("ID"))
                    pIU("ID") = ReadyString(Guid.NewGuid.ToString)
                    pIU("SMSID") = ReadyString(ID)
                    pIU("SMSMessage") = ReadyString("To: " & objSMS("Name") & " - " & objSMS("Number")(i) & vbCrLf & vbCrLf & Message)
                    pIU("ActionName") = ReadyString("SMS (Pending)")
                    pIU("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    pIU("LastUpdated") = ReadyString(CDate(DateTime))

                    pSql = pIU.GetInsert

                    Common.WebRequest.ProData.Execute(pSql)

                    AddActivityQuota("SMS", WebSession.Instance.PeopleUserID, CDate(DateTime))
                Next

            Next objSMS

            Common.WebRequest.ProData.CommitTransaction()
            'SendSMS()
            Return "Saved"
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Function GetAllUnsentMessages() As DataTable
        Dim pSql As String
        Dim dtMessages As DataTable

        pSql = "Select * from TransSmsMessages where SenttoModemTries = 0"

        dtMessages = Common.WebRequest.ProData.Execute(pSql)

        Return dtMessages

    End Function

    Function GetTotalSmSUsage() As DataTable
        Dim pSql As String
        Dim dtMessages As DataTable

        pSql = "Select COUNT(*) DeviceToModem from TransSmsMessages "

        dtMessages = Common.WebRequest.ProData.Execute(pSql)

        Return dtMessages
    End Function

    Function GetSmSUsage() As DataTable
        Dim pSql As String
        Dim dtMessages As DataTable

        pSql = "Select MobizLibraryTelcos.ID, MobizLibraryTelcos.Name Telco, COUNT(*) DeviceToModem from TransSmsMessages " & vbCrLf &
                "inner join MobizLibraryTelcoPrefixes on MobizLibraryTelcoPrefixes.Prefix = substring(sendto,4,3) " & vbCrLf &
                "inner join MobizLibraryTelcos on MobizLibraryTelcos.ID = MobizLibraryTelcoPrefixes.TelcoId " & vbCrLf &
                " GROUP BY MobizLibraryTelcos.Name, MobizLibraryTelcos.ID " & vbCrLf &
                "UNION " & vbCrLf &
                "Select NULL ID, 'Total' Telco, COUNT(*) DeviceToModem from TransSmsMessages "

        dtMessages = Common.WebRequest.ProData.Execute(pSql)

        Return dtMessages
    End Function

    Function GetSmSUsagePerTelco() As DataTable
        Dim pSql As String
        Dim dtMessages As DataTable
        Dim whereSearch As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim OrderBy As String

        If PageSearchText <> "" Then
            whereSearch = " WHERE (SendToName LIKE " & ReadyStringWild(PageSearchText) &
                            " OR SendTo LIKE" & ReadyStringWild(PageSearchText) &
                            " OR Message LIKE" & ReadyStringWild(PageSearchText) &
                            " OR SentBy LIKE " & ReadyStringWild(PageSearchText) &
                            ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If CInt(PageColumnNo) = 2 Then
            OrderBy = " ORDER BY LastUpdated " & PageColumnDirection

        ElseIf CInt(PageColumnNo) = 3 Then
            OrderBy = " ORDER BY SentBy " & PageColumnDirection

        ElseIf CInt(PageColumnNo) = 4 Then
            OrderBy = " ORDER BY Status " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (ORDER BY ID ASC) AS RowNum From (Select TransSmsMessages.ID, ISNULL(SendToName, '') as SendToName, SendTo, Message, TransSmsMessages.LastUpdated, MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName SentBy, " & vbCrLf &
                "TransCRM.ActionName as Status " & vbCrLf &
                "from TransSmsMessages " & vbCrLf &
                "inner join MobizLibraryTelcoPrefixes on MobizLibraryTelcoPrefixes.Prefix = substring(sendto,4,3) " & vbCrLf &
                "inner join MobizLibraryTelcos on MobizLibraryTelcos.ID = MobizLibraryTelcoPrefixes.TelcoId " & vbCrLf &
                "left join MobizLibraryPeople on MobizLibraryPeople.ID = TransSmsMessages.UserID " & vbCrLf &
                "left join TransCRM on TransCRM.SMSID = TransSmsMessages.TransSMSID " & vbCrLf

        If Not CStr2(Common.WebRequest.QueryString("TelcoID")) = "All" Then
            pSql &= " WHERE MobizLibraryTelcos.ID = " & ReadyString(CStr2(Common.WebRequest.QueryString("TelcoID"))) & vbCrLf
        End If

        pSql &= " ) as V1 " & whereSearch & " ) as v2 " & whereLimit & OrderBy

        dtMessages = Common.WebRequest.ProData.Execute(pSql)

        Return dtMessages
    End Function

    Function AddActivityQuota(ByVal Action As String, ByVal ClientID As String, ByVal DateToday As Date) As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pSqlQuota As String = "NULL"
        Dim pDTQuota As DataTable
        Dim QuotaID As String = "NULL"
        Dim TeamID As String
        Dim p As New InsertUpdate("TransCRMCachedActivity")

        'Check if there's exist activity of current user within today
        pSql = "SELECT * FROM TransCRMCachedActivity WHERE ClientID = " & ReadyString(ClientID) & " AND CONVERT(date, DateToday) = " & ReadyString(DateToday)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        pSqlQuota = "SELECT * FROM LibraryCRMActivityQuota "
        pDTQuota = Common.WebRequest.ProData.Execute(pSqlQuota)

        For Each quota As Object In pDTQuota.Rows
            'Check what Activity Quota would be use based on today date
            If CDate(DateToday) >= CDate(quota("DateFrom")) And CDate(DateToday) <= CDate(quota("DateTo")) Then
                QuotaID = ReadyString(quota("ID").ToString())
                'TeamID = quota("TeamID").ToString()
            End If
        Next

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("ClientID") = ReadyString(ClientID)
            p("DateToday") = ReadyString(DateToday)
            p("LastUpdated") = ReadyString(DateToday)
            p("ActivityQuotaID") = QuotaID

            If pDT.Rows.Count = 0 Then
                p("ID") = ReadyString(Guid.NewGuid().ToString)
                p(Action) = 1
                pSql = p.GetInsert
            Else
                Dim rowid As String = pDT.Rows(0)("ID").ToString()

                p(Action) = CInt2(pDT.Rows(0)(Action)) + 1 'add always 1 in every activity

                pSql = p.GetUpdate(" ID = " & ReadyString(rowid))

            End If

            Common.WebRequest.ProData.Execute(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try

    End Function

    Public Sub Open()

        If Not (SMSPort.IsOpen = True) Then

            SMSPort.Open()

        End If

    End Sub

    Public Sub Close()

        If SMSPort.IsOpen = True Then

            SMSPort.Close()

        End If

    End Sub

    Class Frown
        Public d As dClass

        Public Class dClass
            Public Results As ResultClass

            Public Class ResultClass
                Public Rows As List(Of RowsClass)

                Public Class RowsClass
                    Public ID As String
                    Public CustomerID As String
                    Public FOWSTID As String
                    Public SalesDate As String
                    Public GrossSales As Double
                    Public Discount As Double
                    Public SalesReturns As Double
                    Public NetSales As Double
                    Public COGSGross As Double
                    Public COGSNet As Double
                End Class
            End Class
        End Class
    End Class
End Class