Namespace WebProj
    Public Class FranchiseeController
        Inherits System.Web.Mvc.Controller

        Dim WithEvents mDbProvisioner As DbProvisioner
        Dim WithEvents mDbBackup As DbBackup
        Dim data As Object
        Dim FranchisorID As String
        Dim FranchiseReqApprovedBy As String
        Dim sourcedb As String

        Function Index() As ActionResult
            ViewData("FranchisorName") = Common.WebRequest.ProData.Execute("SELECT ID, CompanyName FROM MobizLibraryOrganization where ID =" & ReadyString(WebRequest.Instance.OrgID)).Rows(0)("CompanyName")
            Return View()
        End Function

        Function success() As ActionResult
            ViewData("FranchisorName") = Common.WebRequest.ProData.Execute("SELECT ID, CompanyName FROM MobizLibraryOrganization where ID =" & ReadyString(WebRequest.Instance.OrgID)).Rows(0)("CompanyName")
            Return View()
        End Function

        Function GetAdditionalDateForSignin() As String
            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim browse As New BrowserController
            Dim org As New Organization
            Dim p As New JsonDictionary

            p.Add("BusinessIndustries", jss.DeserializeObject(browse.LibraryIndustryClassification(True))("d")("Results")("Rows"))
            p.Add("Cities", jss.DeserializeObject(browse.LibraryCities(True))("d")("Results")("Rows"))
            p.Add("Countries", jss.DeserializeObject(browse.LibraryCountries(True))("d")("Results")("Rows"))
            p.Add("Telco", jss.DeserializeObject(browse.LibraryTelcoPrefixes(True))("d")("Results")("Rows"))
            p.Add("BusinessTradeName", jss.DeserializeObject(DataTableToJSON.GetJson(org.getOrgTradeNameList(WebRequest.Instance.OrgID))))

            Return p.Serialize()
        End Function

        Function CreateAccount(ByVal usertype As String) As String
            Dim p As New InsertUpdate("MobizLibraryPeople")
            Dim ppolID As String = Guid.NewGuid().ToString()
            Dim emaillinkid As String = Guid.NewGuid.ToString()
            Dim details As Object = Common.WebRequest.AllData("details")
            Dim signup As New SignUp
            Dim orguserid As String
            Dim pSql As String

            Try
                Common.WebRequest.ProData.BeginTransaction()

                signup.UserName = details("Username")

                If signup.CheckPeopleUniqueUserName().Rows.Count > 0 Then
                    Return JsonDictionary.Serialize("Results", "exist")
                End If

                p("ID") = ReadyString(ppolID)
                p("FirstName") = ReadyString(details("FirstName"))
                p("MiddleName") = ReadyString(details("MiddleName"))
                p("Surname") = ReadyString(details("LastName"))
                p("PersonalSuffixName") = ReadyString(details("GenSuffix"))
                p("DateOfBirth") = ReadyString(CDate(details("DateOfBirth")))
                p("IsActive") = 1
                p("LastUpdated") = ReadyString(CDate(details("DateNow")))

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                p("PeopleID") = ReadyString(ppolID)
                p("EmailAddress") = ReadyString(details("PersonEmailAdd"))
                p("EmailType") = ReadyString("Work Email")
                p("LastUpdated") = ReadyString(CDate(details("DateNow")))
                p("IsEmailConfirmed") = 0
                p("EmailActivationLinkID") = ReadyString(emaillinkid)

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryPeopleAddress")

                p("PeopleID") = ReadyString(ppolID)
                p("HouseBuildingNo") = ReadyString(details("PersonBldgNo"))
                p("Street") = ReadyString(details("PersonStreetName"))
                p("CityID") = ReadyString(details("PersonCity"))
                p("CountryID") = ReadyString(details("PersonCountry"))
                p("AddressType") = ReadyString("Permanent Address")
                p("LastUpdated") = "getdate()"

                p("ID") = ReadyString(Guid.NewGuid.ToString())

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryPeopleUserAccount")

                p("PeopleID") = ReadyString(ppolID)
                p("Username") = ReadyString(details("Username"))
                p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(details("Password")))
                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

                p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                p("PeopleID") = ReadyString(ppolID)
                p("SecurityQuestion") = ReadyString(details("SecQ1"))
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(details("SecQ1Ans"))) 'ReadyString(SecurityQuestion1Answer)
                p("ID") = ReadyString(Guid.NewGuid.ToString())

                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

                p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                p("PeopleID") = ReadyString(ppolID)
                p("SecurityQuestion") = ReadyString(details("SecQ2"))
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(details("SecQ2Ans"))) 'ReadyString(SecurityQuestion1Answer)
                p("ID") = ReadyString(Guid.NewGuid.ToString())

                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

                Dim companyid As String = Guid.NewGuid.ToString()
                orguserid = companyid

                p = New InsertUpdate("MobizLibraryOrganization") 'SAVE COMPANY PROFILE

                p("ID") = ReadyString(companyid)
                p("CompanyName") = ReadyString(details("CompanyName"))
                p("TinNumber") = ReadyString(details("Tin"))
                p("IndustryClassID") = ReadyString(details("BusinessIndustry"))
                p("OfficeAddressBuildingNo") = ReadyString(details("CompanyBldgNo"))
                p("OfficeAddressStreet") = ReadyString(details("CompanyStreetName"))
                p("OfficeAddressCityID") = ReadyString(details("CompanyCity"))
                p("OfficeAddressCountryID") = ReadyString(details("CompanyCountry"))
                p("LastUpdated") = ReadyString(CDate(details("DateNow")))
                p("IsActive") = 1
                p("IsOrganization") = 1

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationContactEmail") 'SAVE COMPANY EMAIL

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("OrganizationID") = ReadyString(companyid)
                p("EmailAddress") = ReadyString(details("CompanyEmailAdd"))
                p("UserID") = ReadyString(ppolID)
                p("LastUpdated") = "getdate()"
                p("EmailType") = ReadyString("Work Email")

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationContactMobile") 'SAVE COMPANY MOBILE

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("OrganizationID") = ReadyString(companyid)
                p("CountryID") = ReadyString("e783ed13-aa98-4cc1-918e-d49b05c0167e")
                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(details("CompanyMobileNo"))("ID"))
                p("ContactNo") = ReadyString(details("CompanyMobileNo"))
                p("UserID") = ReadyString(ppolID)
                p("LastUpdated") = "getdate()"
                p("ContactType") = ReadyString("Work Mobile")

                Common.WebRequest.ProData.Execute(p.GetInsert())

                Dim fowstid As String = Guid.NewGuid.ToString()

                p = New InsertUpdate("MobizLibraryOrganizationFowst") 'SAVE FOWST

                p("OrganizationID") = ReadyString("10000000-1000-1000-1000-100000000000") '10000000-1000-1000-1000-100000000000 temporary OrgID lang ni since required sa database, need pa iapprove ang FOWST na franchisee application --- BY ARNIE May 11, 2021
                p("FOWSTTypeID") = ReadyString(details("FowstType").ToString())
                p("Name") = ReadyString(details("StoreName").ToString())
                p("BusinessTradeNameID") = ReadyString(details("BusinessTradeNameID").ToString())
                p("StreetNumber") = ReadyString(details("StoreBldgNo"))
                p("Street") = ReadyString(details("StoreStreetName"))
                p("FowstCityID") = ReadyString(details("StoreCity"))
                p("FowstCountryID") = ReadyString(details("StoreCountry"))
                p("IsActive") = 1
                p("IsFranchisee") = 1
                p("FranchisorID") = ReadyString(WebRequest.Instance.OrgID)
                p("ID") = ReadyString(fowstid)

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationFOWSTFranchiseeRequests") 'SAVE TO FRANCHISEE REQUEST

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("DBID") = ReadyString(WebRequest.Instance.OrgID)
                p("CompanyID") = ReadyString(companyid)
                p("FowstID") = ReadyString(fowstid)
                p("AdminID") = ReadyString(ppolID)
                p("ApplicationDate") = "getdate()"

                Common.WebRequest.ProData.Execute(p.GetInsert())

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetAllRequests() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
            Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
            Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
            Dim IsCountPage As Boolean = False
            Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
            Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()
            Dim pSql As String
            Dim pDT As DataTable
            Dim whereSearch As String = " WHERE ISNULL(CONVERT(varchar, req.IsApproved), '') = '' " 'NEW
            Dim whereOrder As String = ""
            Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

            If Request.Params.Get("status") <> "new" Then
                whereSearch = " WHERE req.IsApproved = " & Request.Params.Get("status")
            End If

            whereSearch &= " AND DBID = " & ReadyString(WebSession.Instance.OrganizationID)

            If PageSearchText <> "" Then
                Dim whereSearchContent As String = ""
                Dim operators As String = ""

                Dim arr As Object = Split(PageSearchText, " ")

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " AND "
                    End If

                    whereSearchContent = whereSearchContent & operators &
                                    "ISNULL(CompanyName, '') + ' ' + " & vbCrLf &
                                    "ISNULL(fowst.Name, '') + ' ' + " & vbCrLf &
                                    "ISNULL(ppol.FirstName + ' ' + ppol.Surname, '')" & vbCrLf &
                                    " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next
                whereSearch &= " AND (" & whereSearchContent & ")"
            End If

            If IsCountPage = True Then
                whereLimit = ""
            End If

            If PageColumnNo = 0 Or PageColumnNo = 1 Then
                whereOrder = " ORDER BY mainsql.IsDBCreated, mainsql.CompanyName " & PageColumnDirection
            End If

            pSql = RequestsQuery(whereOrder, whereLimit, whereSearch)
            Common.WebRequest.ProData.CommandTimeout = 120
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pSql = RequestsQuery(whereOrder, "", whereSearch)
            Dim pDTCountPage As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
        End Function

        Function RequestsQuery(ByVal whereOrder As String, ByVal whereLimit As String, ByVal whereSearch As String)
            Return "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                    " SELECT req.*, org.CompanyName, fowst.Name as StoreName, ppol.FirstName + ' ' + ppol.Surname as Administrator, " & vbCrLf &
                    " upby.FirstName + ' ' + upby.Surname as UpdatedByName, " & vbCrLf &
                    " CASE WHEN sub.ID IS NOT NULL THEN 1 ELSE 0 END IsDBCreated " & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWSTFranchiseeRequests as req " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganization org ON (org.ID = req.CompanyID) " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = req.FowstID) " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople ppol ON (ppol.ID = req.AdminID) " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople upby ON (upby.ID = req.UpdatedBy) " & vbCrLf &
                    " LEFT JOIN MobizLibrarySubscription sub ON (sub.OrganizationID = req.CompanyID) " & vbCrLf &
                    whereSearch & vbCrLf &
                    ") as mainsql ) as v2 " & whereLimit
        End Function

        Function GetDetails()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")
            Dim pSql As String
            Dim pOut As New JsonDictionary

            pSql = "SELECT org.*, city.Name as CityName, country.Name as CountryName, ind.Class as BusinessIndustry, " & vbCrLf &
                    " (SELECT TOP 1 EmailAddress FROM MobizLibraryOrganizationContactEmail WHERE OrganizationID = org.ID) as EmailAddress, " & vbCrLf &
                    " (SELECT TOP 1 ContactNo FROM MobizLibraryOrganizationContactMobile WHERE OrganizationID = org.ID) as MobileNo " & vbCrLf &
                    " FROM MobizLibraryOrganization as org" & vbCrLf &
                    " INNER JOIN MobizLibraryCities as city ON city.ID = org.OfficeAddressCityID " & vbCrLf &
                    " INNER JOIN MobizLibraryCountries as country ON country.ID = org.OfficeAddressCountryID " & vbCrLf &
                    " INNER JOIN MobizLibraryIndustryClass as ind ON ind.ID = org.IndustryClassID " & vbCrLf &
                    " WHERE org.ID = " & ReadyString(data("CompanyID").ToString())
            pOut.Add("Company", jss.DeserializeObject(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT fowst.*, city.Name as CityName, country.Name as CountryName, trade.BusinessTradeName " & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST as fowst" & vbCrLf &
                    " INNER JOIN MobizLibraryCities as city ON city.ID = fowst.FowstCityID " & vbCrLf &
                    " INNER JOIN MobizLibraryCountries as country ON country.ID = fowst.FowstCountryID " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationTradeName as trade ON trade.ID = fowst.BusinessTradeNameID " & vbCrLf &
                    " WHERE fowst.ID = " & ReadyString(data("FowstID").ToString())
            pOut.Add("Store", jss.DeserializeObject(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT people.*, (SELECT TOP 1 EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = people.ID) as EmailAddress " & vbCrLf &
                    " FROM MobizLibraryPeople as people " & vbCrLf &
                    " WHERE people.ID = " & ReadyString(data("AdminID").ToString())
            pOut.Add("Admin", jss.DeserializeObject(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            Return pOut.Serialize()
        End Function

        Function Disapprove()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim data As Object = Common.WebRequest.AllData("data")

            If CBool2(data("IsApproved")) Then
                Return JsonDictionary.Serialize("Results", "Sorry! This request has already been approved!")
            End If

            Common.WebRequest.ProData.BeginTransaction()

            Try
                Dim p As New InsertUpdate("MobizLibraryOrganizationFOWSTFranchiseeRequests")

                p("IsApproved") = 0
                p("LastUpdated") = "getdate()"
                p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function Approve()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            data = Common.WebRequest.AllData("data")
            FranchisorID = WebRequest.Instance.OrgID
            FranchiseReqApprovedBy = WebSession.Instance.PeopleUserID

            If CStr2(data("IsApproved")) <> "" Then
                If Not CBool2(data("IsApproved")) Then
                    Return JsonDictionary.Serialize("Results", "Sorry! This request has already been rejected!")
                End If
            End If

            Common.WebRequest.ProData.BeginTransaction()

            sourcedb = Common.WebRequest.ProData.Connection.Database

            Try
                Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")

                'p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                'p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                'p("LastUpdated") = "getdate()"
                'Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("FowstID"))))

                If Not CBool2(Common.WebRequest.ProData.ExecuteRow("SELECT IsApproved FROM MobizLibraryOrganizationFOWSTFranchiseeRequests WHERE ID = " & ReadyString(data("ID")))("IsApproved")) Then
                    p = New InsertUpdate("MobizLibraryOrganizationFOWSTFranchiseeRequests")

                    p("IsApproved") = 1
                    p("LastUpdated") = "getdate()"
                    p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
                End If

                'p = New InsertUpdate("LibraryOrganizationEmployee")

                'p("ID") = ReadyString(Guid.NewGuid.ToString())
                'p("PeopleID") = ReadyString(data("AdminID"))
                'Common.WebRequest.ProData.Execute(p.GetInsert())

                'p = New InsertUpdate("MobizLibraryPeopleOrganization")

                'p("PeopleID") = ReadyString(data("AdminID"))
                'p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                'p("IsConfirmed") = 1
                'p("DateTimeConfirmed") = "getDate()"
                'p("IsTerminated") = 0
                'p("DateTimeTerminated") = "NULL"
                'p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                'Common.WebRequest.ProData.Execute(p.GetInsert())

                'p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                'p("ID") = ReadyString(Guid.NewGuid.ToString())
                'p("PeopleID") = ReadyString(data("AdminID"))
                'p("FowstID") = ReadyString(data("FowstID"))
                'Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("OrganizationID") = ReadyString(data("CompanyID"))
                p("CustomerCode") = ReadyString(New OrganizationShort().GenerateCodeNumber(True))
                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" OrganizationID = " & ReadyString(data("CompanyID")))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                p("OrganizationID") = ReadyString(data("CompanyID"))
                p("PeopleID") = ReadyString(data("AdminID"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getDate()"
                p("IsUseForPersonOrgAffiliates") = 0
                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" OrganizationID = " & ReadyString(data("CompanyID")) & " AND PeopleID = " & ReadyString(data("AdminID")))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

                p("FranchisorOrgID") = ReadyString(WebRequest.Instance.OrgID)
                p("FranchiseeOrgFowstID") = ReadyString(data("FowstID"))
                p("IsFranchisorConfirmed") = 1
                p("IsFranchiseeConfirmed") = 1
                p("IsActive") = 1
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getDate()"
                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" FranchisorOrgID = " & ReadyString(WebRequest.Instance.OrgID) & " AND FranchiseeOrgFowstID = " & ReadyString(data("FowstID")))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizTransSubscription")
                Dim subscriptionid As String = Guid.NewGuid.ToString()

                p("OrganizationID") = ReadyString(data("CompanyID"))
                p("SuperAdminID") = ReadyString(data("AdminID"))
                p("SubscriptionDate") = "getDate()"
                p("LastUpdated") = "getDate()"
                p("IsSubscriptionAgreementAgreed") = 1
                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" OrganizationID = " & ReadyString(data("CompanyID")) & " AND SuperAdminID = " & ReadyString(data("AdminID")))) = 0 Then
                    p("ID") = ReadyString(subscriptionid)
                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    p = New InsertUpdate("MobizTransSubscriptionCart")
                    p("SubscriptionID") = ReadyString(subscriptionid)
                    p("ProductID") = ReadyString("73de1888-5c2c-4de0-be47-4df64d77a36c")
                    p("UnitPrice") = "1750"
                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" SubscriptionID = " & ReadyString(subscriptionid))) = 0 Then
                        p("ID") = ReadyString(subscriptionid)
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If

                    If Not SendConfirmationLink(data) Then
                        AppendToLog("There was an error while sending an e-mail to Franchisee POS Registrant: " & data("AdminName") & " (" & data("AdminEmailAdd") & ")")
                    End If
                End If

                Common.WebRequest.ProData.CommitTransaction()

                'Update the BAK file of MobizTemplate db. Para always updated ang icreate na franchisee db.
                Dim pFilename As String
                Dim ProData As ProData
                Dim oError = ""

                ProData = Common.WebRequest.ProData.Clone
                pFilename = ProData.GetFirstDataFilePath("Mobiz")
                If CStr2(pFilename).Trim = "" Then
                    AppendToLog("Unable to determine the location of public database vbUGHQZD7DEA! [Creating db for franchisee]")
                    Return False
                End If

                pFilename = IO.Path.GetDirectoryName(pFilename)
                pFilename = AppendBackslash(pFilename)
                pFilename &= "MobizTemplate.bak"

                mDbBackup = New DbBackup
                mDbBackup.ProData = Common.WebRequest.ProData.Clone
                mDbBackup.SourceDatabase = "MobizTemplate"
                mDbBackup.Filename = pFilename

                AddHandler mDbBackup.Completed,
                    Sub(sender As Object, e As DbBackupRestoreCompletedArgs)
                        If e.Sucess Then
                            'Create own database once approved
                            Dim mDbProvisioner = New DbProvisioner
                            mDbProvisioner.ProData = Common.WebRequest.ProData
                            mDbProvisioner.OrgID = data("CompanyID").ToString()
                            AddHandler mDbProvisioner.Completed,
                                    Sub(sender2 As Object, e2 As DbProvisioner.CompletedArgs)
                                        If e2.Success Then
                                            PrepareFranchiseeDB(mDbProvisioner.ProData)
                                        Else
                                            AppendToLog(e2.ErrorMessage) ' analyze d error
                                        End If
                                    End Sub
                            mDbProvisioner.Execute()
                        Else
                            If e.SqlError IsNot Nothing Then
                                AppendToLog(e.SqlError.Message)
                            ElseIf e.Exception IsNot Nothing Then
                                AppendToLog(e.Exception.Message)
                            Else
                                AppendToLog("Backup database failure: unknown error")
                            End If
                        End If
                    End Sub

                mDbBackup.Execute()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function Repopulate()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                data = Common.WebRequest.AllData("data")
                FranchisorID = WebRequest.Instance.OrgID
                FranchiseReqApprovedBy = WebSession.Instance.PeopleUserID
                sourcedb = Common.WebRequest.ProData.Connection.Database

                'Dim mDbProvisioner = New DbProvisioner
                'mDbProvisioner.ProData = Common.WebRequest.ProData
                'mDbProvisioner.OrgID = data("CompanyID").ToString()

                PrepareFranchiseeDB(Common.WebRequest.ProData)

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.StackTrace)
            End Try
        End Function

        Function PrepareFranchiseeDB(ByVal mProData As ProData)
            Try
                AppendToLog("Preparing franchisee db")

                Dim orgname As String = mProData.Execute("SELECT [Database] FROM MobizLibrarySubscription WHERE OrganizationID = " & ReadyString(data("CompanyID").ToString())).Rows(0)("Database")

                With New Synonyms()
                    .Prefix = "Mobiz"

                    Try
                        .SourceDatabase = "Mobiz"
                        .ProData = mProData.CreateFromConnectionInfo(
                            mProData.Server,
                            "Mobiz",
                            mProData.Username,
                            mProData.Password,
                            mProData.IntegratedSecurity)
                        .Generate()
                    Catch ex As System.Exception
                        AppendToDebug(, True, ex, True)
                    End Try

                    Try
                        .ProData = mProData.CreateFromConnectionInfo(
                            mProData.Server,
                            RemoveSymbols(orgname),
                            mProData.Username,
                            mProData.Password,
                            mProData.IntegratedSecurity)
                        .Generate()
                    Catch ex As System.Exception
                        AppendToDebug(, True, ex, True)
                    End Try
                End With

                Dim pSql = "USE [" & RemoveSymbols(orgname) & "]"
                mProData.Execute(pSql)

                Dim row As Object = mProData.Execute("SELECT * FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(data("FowstID").ToString())).Rows(0)
                Dim tradenamerow As Object = mProData.Execute("SELECT * FROM MobizLibraryOrganizationTradeName WHERE ID = " & ReadyString(row("BusinessTradeNameID").ToString())).Rows(0)
                Dim newtradenameid As String = Guid.NewGuid.ToString()

                mProData.BeginTransaction()

                'copy trade name of franchisor
                Dim p = New InsertUpdate("MobizLibraryOrganizationTradeName")

                p("OrganizationID") = ReadyString(data("CompanyID").ToString())
                p("BusinessTradeName") = ReadyString(tradenamerow("BusinessTradeName").ToString())
                p("BusinessTradeNameInitial") = ReadyString(tradenamerow("BusinessTradeNameInitial").ToString())
                p("MaxAllowableDiscountableAmount") = ReadyString(CDec2(tradenamerow("MaxAllowableDiscountableAmount")))
                If mProData.ExecuteSQL(p.GetUpdate(" OrganizationID = " & ReadyString(data("CompanyID")))) = 0 Then
                    p("ID") = ReadyString(newtradenameid)
                    mProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryOrganizationFOWST")
                p("OrganizationID") = ReadyString(data("CompanyID").ToString())
                p("UserID") = ReadyString(FranchiseReqApprovedBy)
                p("DefaultPosView") = ReadyString("Hot Buttons View")
                p("LastUpdated") = "getdate()"
                mProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("FowstID"))))

                p = New InsertUpdate("LibraryOrganizationEmployee")

                p("PeopleID") = ReadyString(data("AdminID"))
                If mProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(data("AdminID")))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    mProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryPeopleOrganization")

                p("PeopleID") = ReadyString(data("AdminID"))
                p("OrganizationID") = ReadyString(data("CompanyID").ToString())
                p("IsConfirmed") = 1
                p("DateTimeConfirmed") = "getDate()"
                p("IsTerminated") = 0
                p("DateTimeTerminated") = "NULL"
                p("UserID") = ReadyString(FranchiseReqApprovedBy)
                If mProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(data("AdminID")) & " AND OrganizationID = " & ReadyString(data("CompanyID")))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    mProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                p("PeopleID") = ReadyString(data("AdminID"))
                p("FowstID") = ReadyString(data("FowstID"))
                If mProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(data("AdminID")) & " AND FowstID = " & ReadyString(data("FowstID")))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    mProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("LibrarySecurityRoleUser")
                p("PeopleID") = ReadyString(data("AdminID"))
                p("RoleID") = ReadyString("474F9F49-2876-4391-A005-107EB18F2762") '''Fixed ID for Super Admin Role
                p("UserID") = ReadyString(FranchiseReqApprovedBy)
                p("LastUpdated") = "GetDate()"
                If mProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(data("AdminID")) & " AND RoleID = '474F9F49-2876-4391-A005-107EB18F2762'")) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    mProData.Execute(p.GetInsert())
                End If

                'ADD DEFAULT CUSTOMER [CASH PAYMENT]
                p = New InsertUpdate("LibraryOrganizationCustomer")
                p("OrganizationID") = ReadyString("cd71a0f7-08a9-4601-876e-5f13c064f873")
                p("Deactivated") = "0"
                If mProData.ExecuteSQL(p.GetUpdate(" OrganizationID = 'cd71a0f7-08a9-4601-876e-5f13c064f873' ")) = 0 Then
                    p("ID") = ReadyString(newtradenameid)
                    p("ActivationDate") = "getdate()"
                    mProData.Execute(p.GetInsert())
                End If

                PopulateFranchiseeDB(FranchisorID, sourcedb, data, mProData, False)

                Call New SecurityAccess().GrantAllToSysAdmin(mProData)

                mProData.CommitTransaction()

                AppendToLog(data("CompanyName") & " database created")
            Catch ex As Exception
                AppendToLog(ex.Message)
                mProData.RollbackTransaction()
            End Try
        End Function

        Function GetFranchiseeLastUpdate()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT TOP 1 logs.* " & vbCrLf &
                                " , ppol.FirstName + ' ' + ppol.Surname as UpdatedByName " & vbCrLf &
                                " FROM MobizTransFranchiseeSyncDataLogs as logs " & vbCrLf &
                                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = logs.UpdatedBy " & vbCrLf &
                                " WHERE FranchiseeID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                                " ORDER BY LastUpdated DESC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function UpdateDB()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            Dim errmsg As String

            Try
                Dim secrole As New SecurityRole

                If secrole.CheckSecurityCurrentUserAcccess("Allow to sync data from franchisor database", WebRequest.Instance.OrgID, WebSession.Instance.PeopleUserID).Rows.Count = 0 Then
                    Return JsonDictionary.Serialize("NoAccess", True)
                End If

                Common.WebRequest.ProData.BeginTransaction()

                Dim pSql As String = "SELECT * FROM MobizLibraryOrganizationFOWSTFranchiseeRequests WHERE CompanyID = " & ReadyString(WebRequest.Instance.OrgID)
                Dim franchiseedata As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                If franchiseedata.Length = 0 Then
                    Return JsonDictionary.Serialize("Results", "invalid")
                End If

                franchiseedata = franchiseedata(0)

                Dim franchisorid As String = franchiseedata("DBID").ToString()

                franchiseedata.Add("CompanyName", Common.WebRequest.ProData.Execute("SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = " & ReadyString(franchiseedata("CompanyID").ToString())))

                pSql = "SELECT * FROM MobizLibrarySubscription WHERE OrganizationID = " & ReadyString(franchisorid)
                Dim sourcedb As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("Database").ToString()

                PopulateFranchiseeDB(franchisorid, sourcedb, franchiseedata, Common.WebRequest.ProData, True, errmsg)

                If errmsg <> "" Then
                    Common.WebRequest.ProData.RollbackTransaction()
                    AppendToLog(errmsg)
                    Return JsonDictionary.Serialize("Results", "error")
                End If

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryOrganizationPIM")

                Dim p As New InsertUpdate("MobizTransFranchiseeSyncDataLogs")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("FranchisorID") = ReadyString(franchisorid)
                p("FranchiseeID") = ReadyString(franchiseedata("CompanyID").ToString())
                p("LastUpdated") = "getdate()"
                p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                Common.WebRequest.ProData.Execute(p.GetInsert())

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(errmsg)
                AppendToLog(ex.Message)
                Return Json(New With {Key .Results = "error", .Message = errmsg})
            End Try
        End Function

        Function PopulateFranchiseeDB(ByVal FranchisorID As String, ByVal sourcedb As String, ByVal data As Object, ByVal mProData As ProData,
                          ByVal IsUpdateData As Boolean, Optional ByRef errmsg As String = "")

            Dim tablename As String = ""

            Try
                AppendToLog("Populating data to franchisee db")

                If mProData Is Nothing Then
                    mProData = Common.WebRequest.ProData
                End If

                'COPY DATA FROM FRANCHISOR'S TABLES
                Dim tables As New List(Of String) From {"LibraryOrganizationPIM", "LibraryPimBrands",
                                                        "LibraryPimCategories", "LibraryItemModifiers",
                                                        "LibraryItemModifierCategories", "LibraryItemModifierRebates",
                                                        "LibraryItemModifierInventoryAffecting", "LibraryItemModifierNonInventoryAffecting",
                                                        "LibraryItemModifierSorting", "LibraryPimMenu", "LibraryPIMGroups", "LibraryPimGroupPims",
                                                        "MobizLibraryPIMProductPriceLevel", "TablesLastUpdated", "SystemDefaults",
                                                        "LibraryOrganizationPriceLevel", "MobizLibraryPIMProductContents", "LibrarySecurityRoleModules",
                                                        "MobizLibraryPIMProductParent", "LibraryPimMenuButtonAssignedStores"}
                Dim pSql As String
                Dim pDT As DataTable
                Dim franchiseeid As String = RemoveSymbols(data("CompanyID"))

                For Each table As String In tables
                    tablename = table
                    If table = "MobizLibraryPIMProductPriceLevel" Then
                        'copyhon ang prices from franchisor
                        pSql = "IF OBJECT_ID(N'tempdb..#temppricelevel_" & franchiseeid & "') IS NOT NULL BEGIN DROP TABLE #temppricelevel_" & franchiseeid & " END " & vbCrLf &
                                " SELECT * INTO #temppricelevel_" & franchiseeid & vbCrLf &
                                " FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                                " WHERE OrganizationID = " & ReadyString(FranchisorID) & vbCrLf &
                                " UPDATE #temppricelevel_" & franchiseeid & " SET ID = NEWID(), OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf
                        mProData.Execute(pSql)

                        pSql = "INSERT INTO " & table & " SELECT * FROM #temppricelevel_" & franchiseeid & " as temp " & vbCrLf &
                                    " WHERE NOT EXISTS( " & vbCrLf &
                                    "   SELECT * FROM MobizLibraryPIMProductPriceLevel as pl " & vbCrLf &
                                    "   WHERE pl.OrganizationID = temp.OrganizationID " & vbCrLf &
                                    "   AND pl.ProductID = temp.ProductID AND pl.PriceLevelID = temp.PriceLevelID " & vbCrLf &
                                    " ) "
                        mProData.Execute(pSql)

                        If IsUpdateData Then
                            pSql = " UPDATE destinationtable SET " & vbCrLf &
                                " destinationtable.Price = sourcetable.Price " & vbCrLf &
                                " , destinationtable.LastUpdated = sourcetable.LastUpdated " & vbCrLf &
                                " , destinationtable.DeactivateDiscount = sourcetable.DeactivateDiscount " & vbCrLf &
                                " , destinationtable.UserID = sourcetable.UserID " & vbCrLf &
                                " , destinationtable.SourceFlag = sourcetable.SourceFlag " & vbCrLf &
                                " FROM " & table & " as destinationtable " & vbCrLf &
                                " INNER JOIN #temppricelevel_" & franchiseeid & " as sourcetable " & vbCrLf &
                                " ON destinationtable.OrganizationID = sourcetable.OrganizationID " & vbCrLf &
                                " AND destinationtable.ProductID = sourcetable.ProductID " & vbCrLf &
                                " AND destinationtable.PriceLevelID = sourcetable.PriceLevelID " & vbCrLf
                            mProData.Execute(pSql)
                        End If
                    ElseIf table = "LibraryPimMenu" Then
                        'copyhon ang HB Template pero ichange ang UserID coz only employee profile/data ang naa sa MobizLibraryPeople sqlite
                        pSql = "IF OBJECT_ID(N'tempdb..#temppimmenu_" & franchiseeid & "') IS NOT NULL BEGIN DROP TABLE #temppimmenu_" & franchiseeid & " END " & vbCrLf &
                                " SELECT * INTO #temppimmenu_" & franchiseeid & vbCrLf &
                                " FROM " & sourcedb & ".dbo.LibraryPimMenu" & vbCrLf &
                                " UPDATE #temppimmenu_" & franchiseeid & " SET UserID = " & ReadyString(data("AdminID")) & vbCrLf
                        mProData.Execute(pSql)

                        pSql = "INSERT INTO " & table & " SELECT * FROM #temppimmenu_" & franchiseeid & vbCrLf &
                                " WHERE ID NOT IN (SELECT ID FROM " & table & ") "
                        mProData.Execute(pSql)
                    ElseIf table = "MobizLibraryPIMProductContents" Then
                        pSql = "IF OBJECT_ID(N'tempdb..#tempbom_" & franchiseeid & "') IS NOT NULL BEGIN DROP TABLE #tempbom_" & franchiseeid & " END " & vbCrLf &
                                " SELECT * INTO #tempbom_" & franchiseeid & vbCrLf &
                                " FROM MobizLibraryPIMProductContents" & vbCrLf &
                                " WHERE OrganizationID = " & ReadyString(FranchisorID) & vbCrLf &
                                " UPDATE #tempbom_" & franchiseeid & " SET ID = NEWID(), OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf
                        mProData.Execute(pSql)

                        'Insert new BOM of SKU
                        pSql = "INSERT INTO " & table & " SELECT * FROM #tempbom_" & franchiseeid & " as temp " & vbCrLf &
                                    " WHERE NOT EXISTS( " & vbCrLf &
                                    "   SELECT * FROM MobizLibraryPIMProductContents as pc " & vbCrLf &
                                    "   WHERE pc.OrganizationID = temp.OrganizationID " & vbCrLf &
                                    "   AND pc.ParentProductID = temp.ParentProductID AND pc.ChildProductID = temp.ChildProductID " & vbCrLf &
                                    " ) "
                        mProData.Execute(pSql)

                        If IsUpdateData Then
                            'Update modified BOM from franchisor
                            pSql = "UPDATE destinationtable SET destinationtable.Quantity = sourcetable.Quantity " & vbCrLf &
                                    ", destinationtable.UnitMeasureID = sourcetable.UnitMeasureID " & vbCrLf &
                                    " FROM MobizLibraryPIMProductContents as destinationtable " & vbCrLf &
                                    " INNER JOIN #tempbom_" & franchiseeid & " as sourcetable " & vbCrLf &
                                    " ON destinationtable.ParentProductID = sourcetable.ParentProductID " & vbCrLf &
                                    " AND destinationtable.ChildProductID = sourcetable.ChildProductID " & vbCrLf &
                                    " AND destinationtable.OrganizationID = sourcetable.OrganizationID " & vbCrLf
                            mProData.Execute(pSql)

                            'Remove also the deleted BOM in franchisee db
                            pSql = " DELETE FROM MobizLibraryPIMProductContents " & vbCrLf &
                                    " WHERE ID NOT IN ( " & vbCrLf &
                                    "   SELECT f1.ID " & vbCrLf &
                                    "   FROM MobizLibraryPIMProductContents as f1 " & vbCrLf &
                                    "   RIGHT JOIN #tempbom_" & franchiseeid & " as temp " & vbCrLf &
                                    "   ON f1.ParentProductID = temp.ParentProductID " & vbCrLf &
                                    "   AND f1.ChildProductID = temp.ChildProductID " & vbCrLf &
                                    "   WHERE f1.OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf &
                                    "   ) AND OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf
                            mProData.Execute(pSql)
                        End If
                    ElseIf table = "MobizLibraryPIMProductParent" Then
                        pSql = "IF OBJECT_ID(N'tempdb..#tempprodtype_" & franchiseeid & "') IS NOT NULL BEGIN DROP TABLE #tempprodtype_" & franchiseeid & " END " & vbCrLf &
                                " SELECT * INTO #tempprodtype_" & franchiseeid & vbCrLf &
                                " FROM MobizLibraryPIMProductParent" & vbCrLf &
                                " WHERE OrganizationID = " & ReadyString(FranchisorID) & vbCrLf &
                                " UPDATE #tempprodtype_" & franchiseeid & " SET ID = NEWID(), OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf
                        mProData.Execute(pSql)

                        'Insert Production type of new BOM
                        pSql = "INSERT INTO " & table & " SELECT * FROM #tempprodtype_" & franchiseeid & " as temp " & vbCrLf &
                                    " WHERE NOT EXISTS( " & vbCrLf &
                                    "   SELECT * FROM MobizLibraryPIMProductParent as pp " & vbCrLf &
                                    "   WHERE pp.OrganizationID = temp.OrganizationID " & vbCrLf &
                                    "   AND pp.ProductID = temp.ProductID " & vbCrLf &
                                    " ) "
                        mProData.Execute(pSql)

                        If IsUpdateData Then
                            'Update Production type of BOM
                            pSql = "UPDATE destinationtable SET destinationtable.Jit = sourcetable.Jit " & vbCrLf &
                                    ", destinationtable.Instructions = sourcetable.Instructions " & vbCrLf &
                                    ", destinationtable.IsSeperateIntoComponentDuringSalesReturns = sourcetable.IsSeperateIntoComponentDuringSalesReturns " & vbCrLf &
                                    ", destinationtable.Upcharge = sourcetable.Upcharge " & vbCrLf &
                                    ", destinationtable.UserID = sourcetable.UserID " & vbCrLf &
                                    ", destinationtable.LastUpdated = sourcetable.LastUpdated " & vbCrLf &
                                    " FROM MobizLibraryPIMProductParent as destinationtable " & vbCrLf &
                                    " INNER JOIN #tempprodtype_" & franchiseeid & " as sourcetable " & vbCrLf &
                                    " ON destinationtable.ProductID = sourcetable.ProductID " & vbCrLf &
                                    " AND destinationtable.OrganizationID = sourcetable.OrganizationID " & vbCrLf
                            mProData.Execute(pSql)

                            'Remove
                            pSql = " DELETE FROM MobizLibraryPIMProductParent " & vbCrLf &
                                    " WHERE ID NOT IN ( " & vbCrLf &
                                    "   SELECT f1.ID " & vbCrLf &
                                    "   FROM MobizLibraryPIMProductParent as f1 " & vbCrLf &
                                    "   RIGHT JOIN #tempprodtype_" & franchiseeid & " as temp " & vbCrLf &
                                    "   ON f1.ProductID = temp.ProductID " & vbCrLf &
                                    "   WHERE f1.OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf &
                                    "   ) AND OrganizationID = " & ReadyString(data("CompanyID").ToString()) & vbCrLf
                            mProData.Execute(pSql)
                        End If
                    Else
                        pSql = "INSERT INTO " & table & " SELECT * FROM " & sourcedb & ".dbo." & table & vbCrLf

                        If table = "SystemDefaults" Then
                            pSql += " WHERE Description NOT IN (SELECT Description FROM " & table & ") "
                        ElseIf table = "LibraryPimMenuButtonAssignedStores" Then
                            pSql = "DELETE FROM LibraryPimMenuButtonAssignedStores WHERE ID NOT IN (SELECT ID FROM " & sourcedb & ".dbo." & table & ")"
                            mProData.Execute(pSql)

                            pSql = "INSERT INTO " & table & " SELECT * FROM " & sourcedb & ".dbo." & table & vbCrLf
                            pSql += " WHERE FowstID NOT IN (SELECT FowstID FROM " & table & ") " & vbCrLf &
                                    " AND HotButtonID NOT IN (SELECT HotButtonID FROM " & table & ") "
                        Else
                            pSql += " WHERE ID NOT IN (SELECT ID FROM " & table & ") "
                        End If

                        mProData.Execute(pSql)
                    End If

                    If table <> "MobizLibraryPIMProductPriceLevel" Then
                        If IsUpdateData Then
                            pSql = " Select * FROM " & sourcedb & ".dbo." & table & vbCrLf &
                                    " WHERE ID In (Select ID FROM " & table & ") "
                            pDT = mProData.Execute(pSql)

                            pSql = " UPDATE destinationtable Set " & vbCrLf

                            For i As Integer = 0 To pDT.Columns.Count - 1
                                Dim col As String = pDT.Columns(i).ToString()
                                pSql &= IIf(i > 0, ", ", "") & " destinationtable." & col & " = sourcetable." & col & vbCrLf
                            Next

                            pSql &= " FROM " & table & " As destinationtable " & vbCrLf &
                                    " INNER JOIN " & sourcedb & ".dbo." & table & " As sourcetable " & vbCrLf &
                                    " On destinationtable.ID = sourcetable.ID "

                            mProData.Execute(pSql)
                        End If
                    End If
                Next

                Return True
            Catch ex As Exception
                errmsg = ex.Message
                AppendToDebug("Error While populating data To franchise db  " & data("CompanyName") & ". Error " & errmsg & " While executing table '" & tablename & "'")
                Return False
            End Try
        End Function

        Function SendConfirmationLink(ByVal data As Object) As Boolean
            Try
                Dim msg As String = "<table style='width:100%; margin-top:20px; font-size:20px;'>" +
                                        "<tbody>" +
                                            "<tr>" +
                                                "<td>" +
                                                    "<center>" +
                                                        "<table style='width:560px; margin:0 auto; border-collapse:collapse; border-spacing:0;'>" +
                                                            "<tbody>" +
                                                                "<tr>" +
                                                                    "<td>" +
                                                                        "Dear Sir/Madam " + data("AdminName") + "," +
                                                                        "<br /><br />" +
                                                                        "Good day! " +
                                                                        "<br /><br />" +
                                                                        "This is to inform you that your Franchisee POS Registration has been <b>approved</b> by " + data("LoggedInUser") + " on " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt") + "." +
                                                                        "<br /><br />" +
                                                                        "Please contact any representative from <b>" + data("Franchisor") + "</b> to give you more details about the POS." +
                                                                        "<br /><br />" +
                                                                        "Thank you and have a great day!" +
                                                                        "<br /><br /><br />" +
                                                                        "<font face='arial, sans-serif' size='4' color='#ff0000'><i><b>***This is a system-generated email. Please do not&nbsp;reply directly to this email***</b></i></font>" +
                                                                    "</td>" +
                                                                "</tr>" +
                                                            "</tbody>" +
                                                        "</table>" +
                                                    "</center>" +
                                                "</td>" +
                                            "</tr>" +
                                        "</tbody>" +
                                    "</table>"
                Return True 'pls remove this if needed
                SendMail("customercare@mobiz-apps.com",
                        Trim(data("AdminEmailAdd")),
                        "1Mobiz Inc.",
                        "Franchisee POS Registration Status",
                        msg,
                        "smtp-mail.outlook.com",
                         587,
                         "customercare@mobiz-apps.com",
                         "MobizCustCare2221", , , , , True)

                Return True
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return False
            End Try
        End Function
    End Class
End Namespace