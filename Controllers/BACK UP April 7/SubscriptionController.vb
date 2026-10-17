Namespace WebProj
    Public Class SubscriptionController
        Inherits System.Web.Mvc.AsyncController

        Dim WithEvents mDbProvisioner As DbProvisioner

        Dim pCsvReaderOrgInfo As CsvReader
        Dim pCsvReaderOrgTradeName As CsvReader
        Dim pCsvReaderOrgIndustryClass As CsvReader

        Dim pCsvReaderPeople As CsvReader
        Dim pCsvReaderPeopleContactEmail As CsvReader
        Dim pCsvReaderPeopleContactMobile As CsvReader
        Dim pCsvReaderPeopleOrganization As CsvReader
        Dim pCsvReaderPeopleSecurityQuestion As CsvReader
        Dim pCsvReaderPeopleUserAccount As CsvReader

        Dim pLineOrg As Dictionary(Of String, String)
        Dim pLineOrgTradeName As Dictionary(Of String, String)
        Dim pLineOrgIndustryClass As Dictionary(Of String, String)

        Dim pLinePeople As Dictionary(Of String, String)
        Dim pLinePeopleContactEmail As Dictionary(Of String, String)
        Dim pLinePeopleContactMobile As Dictionary(Of String, String)
        Dim pLinePeopleOrganization As Dictionary(Of String, String)
        Dim pLinePeopleSecurityQuestion As Dictionary(Of String, String)
        Dim pLinePeopleUserAccount As Dictionary(Of String, String)

        Dim pFilename As String

        Dim pDTStatusUpdate As New DataTable
        Shared strStatus As String

        Public Property OrgID As String

        Function CheckForSubscriber() As ActionResult

            DownloadnewSubscriber()

            Return View()

        End Function

        <HttpPost()>
        Function GetStatus() As String
            Dim strReturn As String

            strReturn = JsonDictionary.Serialize("Status", strStatus)

            Return strReturn

        End Function

        <HttpPost()>
        Function DownloadNewSubscriber()
            Dim clsOrganization As New Organization
            Dim webClient As New Net.WebClient

            strStatus = ""
            pFilename = Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioning.zip"
            webClient.DownloadFile("http://mobiz-apps.com/Confirmed/GetSubscribers", pFilename)

            '''Check Library Organization
            pCsvReaderOrgInfo = New CsvReader(pFilename, "LibraryOrganization")

            pDTStatusUpdate = New DataTable
            pDTStatusUpdate.Columns.Add("OrganizationID")
            pDTStatusUpdate.Columns.Add("Status")

            ProcessDatabaseProvisioning()

        End Function

        <HttpPost()>
        Function ProcessDatabaseProvisioning()
            Dim clsOrganization As New Organization
            Dim clsPeople As New People

            Dim webClient As New Net.WebClient
            Dim objTradeNameList As New ArrayList
            Dim objIndustryList As New ArrayList
            Dim intCntr As Int16

            pLineOrg = pCsvReaderOrgInfo.ReadLine
            If pLineOrg Is Nothing Then
                '''re initialize Library Organization
                pFilename = Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioning.zip"
                pCsvReaderOrgInfo = New CsvReader(pFilename, "LibraryOrganization")

                '''Restore DBase Now
                mDbProvisioner = New DbProvisioner
                mDbProvisioner.ProData = Common.WebRequest.ProData
                ExecuteAsync()
                Return False
            End If

            strStatus = "Processing data. "

            '''Organization Info
            clsOrganization.ID = CStr2(pLineOrg("ID"))
            clsOrganization.IsOrganization = Convert.ToInt16(Convert.ToBoolean(pLineOrg("IsOrganization")))

            clsOrganization.PersonTitleID = ""
            clsOrganization.PersonSurname = ""
            clsOrganization.PersonFirstName = ""
            clsOrganization.PersonMiddleName = ""
            clsOrganization.PersonSuffixName = ""
            clsOrganization.PersonDOB = ""
            clsOrganization.PersonMotherMaidenSurname = ""
            clsOrganization.PersonMotherMaidenFirstName = ""
            clsOrganization.PersonMotherMaidenMiddleName = ""

            clsOrganization.CompanyName = CStr2(pLineOrg("CompanyName"))
            clsOrganization.CompanyNameInitial = CStr2(pLineOrg("CompanyNameInitial"))

            clsOrganization.OwnershipType = CStr2(pLineOrg("OwnershipType"))
            clsOrganization.IsFranchisor = Convert.ToInt16(Convert.ToBoolean(pLineOrg("IsFranchisor")))
            clsOrganization.RegistrationType = CStr2(pLineOrg("RegistrationType"))
            clsOrganization.RegistrationNo = CStr2(pLineOrg("RegistrationNo"))
            clsOrganization.RegistrationDate = CStr2(pLineOrg("RegistrationDate"))
            clsOrganization.CorporateTIN = CStr2(pLineOrg("TinNumber"))

            clsOrganization.OfficeAddressBuildingNo = CStr2(pLineOrg("OfficeAddressBuildingNo"))
            clsOrganization.OfficeAddressStreet = CStr2(pLineOrg("OfficeAddressStreet"))
            clsOrganization.OfficeAddressCityID = CStr2(pLineOrg("OfficeAddressCityID"))
            clsOrganization.OfficeAddressCountryID = CStr2(pLineOrg("OfficeAddressCountryID"))

            clsOrganization.BusinessAddressBuildingNo = CStr2(pLineOrg("BusinessAddressBuildingNo"))
            clsOrganization.BusinessAddressStreet = CStr2(pLineOrg("BusinessAddressStreet"))
            clsOrganization.BusinessAddressCityID = CStr2(pLineOrg("BusinessAddressCityID"))
            clsOrganization.BusinessAddressCountryID = CStr2(pLineOrg("BusinessAddressCountryID"))

            '''Organization Trade Name Info
            objTradeNameList.Clear()
            pCsvReaderOrgTradeName = New CsvReader(pFilename, "LibraryOrganizationTradeName")
            Do
                pLineOrgTradeName = pCsvReaderOrgTradeName.ReadLine
                If pLineOrgTradeName Is Nothing Then Exit Do

                If (CStr2(pLineOrgTradeName("OrganizationID")).Trim().ToLower() = CStr2(pLineOrg("ID")).Trim().ToLower()) Then
                    pLineOrgTradeName.Add("RecState", "New")
                    objTradeNameList.Add(pLineOrgTradeName)
                End If
            Loop
            clsOrganization.OrgTradeNameList = objTradeNameList.ToArray()

            '''Check Library Organization Industry Classification
            objIndustryList.Clear()
            pCsvReaderOrgIndustryClass = New CsvReader(pFilename, "LibraryOrganizationIndustryClass")
            Do
                pLineOrgIndustryClass = pCsvReaderOrgTradeName.ReadLine
                If pLineOrgIndustryClass Is Nothing Then Exit Do

                If (CStr2(pLineOrgIndustryClass("OrganizationID")).Trim().ToLower() = CStr2(pLineOrg("ID")).Trim().ToLower()) Then
                    pLineOrgIndustryClass.Add("RecState", "New")
                    objIndustryList.Add(pLineOrgIndustryClass)
                End If
            Loop
            clsOrganization.OrgIndustryClassificationList = objIndustryList.ToArray()

            clsOrganization.SaveOrganizationInfo()

            '''Save People INformation
            pCsvReaderPeopleOrganization = New CsvReader(pFilename, "LibraryPeopleOrganization")
            Do
                pLinePeopleOrganization = pCsvReaderPeopleOrganization.ReadLine
                If pLinePeopleOrganization Is Nothing Then Exit Do

                If (CStr2(pLinePeopleOrganization("OrganizationID")).Trim().ToLower() = CStr2(pLineOrg("ID")).Trim().ToLower()) Then

                    '''Check Library People Organization
                    pCsvReaderPeople = New CsvReader(pFilename, "LibraryPeople")
                    Do
                        pLinePeople = pCsvReaderPeople.ReadLine
                        If pLinePeople Is Nothing Then Exit Do

                        If (CStr2(pLinePeople("ID")).Trim().ToLower() = CStr2(pLinePeopleOrganization("PeopleID")).Trim().ToLower()) Then

                            clsPeople.ID = CStr2(pLinePeople("ID"))
                            clsPeople.TitleID = ""
                            clsPeople.Surname = CStr2(pLinePeople("Surname"))
                            clsPeople.FirstName = CStr2(pLinePeople("FirstName"))
                            clsPeople.MiddleName = CStr2(pLinePeople("MiddleName"))
                            clsPeople.PersonalSuffixName = CStr2(pLinePeople("PersonalSuffixName"))

                            clsPeople.DateOfBirth = CStr2(pLinePeople("DateOfBirth"))

                            clsPeople.NickName = CStr2(pLinePeople("NickName"))
                            clsPeople.MotherMaidenSurname = CStr2(pLinePeople("MotherMaidenSurname"))
                            clsPeople.MotherMaidenFirstName = CStr2(pLinePeople("MotherMaidenFirstName"))
                            clsPeople.MotherMaidenMiddleName = CStr2(pLinePeople("MotherMaidenMiddleName"))
                            clsPeople.Gender = CStr2(pLinePeople("Gender"))
                            clsPeople.SavePeopleInfoName()

                            '''Save Primary IDs
                            clsPeople.SSSNo = CStr2(pLinePeople("SSSNo"))
                            clsPeople.PhilHealthNo = CStr2(pLinePeople("PhilHealthNo"))
                            clsPeople.TIN = CStr2(pLinePeople("TIN"))
                            clsPeople.HDMFNo = CStr2(pLinePeople("HDMFNo"))
                            clsPeople.NBINo = CStr2(pLinePeople("NBINo"))
                            clsPeople.PassportNo = CStr2(pLinePeople("PassportNo"))
                            clsPeople.ResidenceCertNo = ""
                            clsPeople.ResidenceCertIssuedAtID = ""
                            clsPeople.ResidenceCertIssuedOn = DateTime.Today
                            clsPeople.PassportIssuedAtID = ""
                            clsPeople.PassportIssuedOn = DateTime.Today
                            clsPeople.PassportExpirationDate = DateTime.Today
                            clsPeople.SavePeoplePrimaryID()

                            '''Save user Account
                            pCsvReaderPeopleUserAccount = New CsvReader(pFilename, "LibraryPeopleUserAccount")
                            Do
                                pLinePeopleUserAccount = pCsvReaderPeopleUserAccount.ReadLine
                                If pLinePeopleUserAccount Is Nothing Then Exit Do

                                If (CStr2(pLinePeople("ID")).Trim().ToLower() = CStr2(pLinePeopleUserAccount("PeopleID")).Trim().ToLower()) Then

                                    clsPeople.PeopleID = CStr2(pLinePeopleUserAccount("PeopleID"))
                                    clsPeople.UserName = CStr2(pLinePeopleUserAccount("Username"))
                                    clsPeople.UserPassword = CStr2(pLinePeopleUserAccount("UserPassword"))

                                    '''Save User Security Question
                                    intCntr = 1
                                    pCsvReaderPeopleSecurityQuestion = New CsvReader(pFilename, "LibraryPeopleSecurityQuestion")
                                    Do
                                        pLinePeopleSecurityQuestion = pCsvReaderPeopleSecurityQuestion.ReadLine
                                        If pLinePeopleSecurityQuestion Is Nothing Then Exit Do

                                        If (CStr2(pLinePeople("ID")).Trim().ToLower() = CStr2(pLinePeopleSecurityQuestion("PeopleID")).Trim().ToLower()) Then
                                            If (intCntr = 1) Then
                                                clsPeople.SecurityQuestion1ID = CStr2(pLinePeopleSecurityQuestion("ID"))
                                                clsPeople.SecurityQuestion1 = CStr2(pLinePeopleSecurityQuestion("SecurityQuestion"))
                                                clsPeople.SecurityQuestion1Answer = CStr2(pLinePeopleSecurityQuestion("SecurityAnswer"))
                                            Else
                                                clsPeople.SecurityQuestion2ID = CStr2(pLinePeopleSecurityQuestion("ID"))
                                                clsPeople.SecurityQuestion2 = CStr2(pLinePeopleSecurityQuestion("SecurityQuestion"))
                                                clsPeople.SecurityQuestion2Answer = CStr2(pLinePeopleSecurityQuestion("SecurityAnswer"))
                                            End If

                                            intCntr = intCntr + 1
                                        End If
                                    Loop '''End Save User Security Question

                                    clsPeople.SavePeopleUserAccount()
                                End If
                            Loop '''End Save user Account

                            '''People Email
                            pCsvReaderPeopleContactEmail = New CsvReader(pFilename, "LibraryPeopleContactEmail")
                            Do
                                pLinePeopleContactEmail = pCsvReaderPeopleContactEmail.ReadLine
                                If pLinePeopleContactEmail Is Nothing Then Exit Do

                                If (CStr2(pLinePeople("ID")).Trim().ToLower() = CStr2(pLinePeopleContactEmail("PeopleID")).Trim().ToLower()) Then
                                    clsPeople.ID = CStr2(pLinePeopleContactEmail("ID"))
                                    clsPeople.PeopleID = CStr2(pLinePeopleContactEmail("PeopleID"))
                                    clsPeople.PersonContactEmail = CStr2(pLinePeopleContactEmail("EmailAddress"))
                                    clsPeople.PersonContactEmailType = CStr2(pLinePeopleContactEmail("EmailType"))
                                    clsPeople.SavePeopleContactEmail()
                                End If
                            Loop '''End People Email

                            '''People Mobile
                            pCsvReaderPeopleContactMobile = New CsvReader(pFilename, "LibraryPeopleContactMobile")
                            Do
                                pLinePeopleContactMobile = pCsvReaderPeopleContactMobile.ReadLine
                                If pLinePeopleContactMobile Is Nothing Then Exit Do

                                If (CStr2(pLinePeople("ID")).Trim().ToLower() = CStr2(pLinePeopleContactMobile("PeopleID")).Trim().ToLower()) Then
                                    clsPeople.ID = CStr2(pLinePeopleContactMobile("ID"))
                                    clsPeople.PeopleID = CStr2(pLinePeopleContactMobile("PeopleID"))
                                    clsPeople.PersonContactCountryID = CStr2(pLinePeopleContactMobile("CountryID"))
                                    clsPeople.PersonContactTelCoPrefixID = CStr2(pLinePeopleContactMobile("TelCoPrefixID"))
                                    clsPeople.PersonContactNo = CStr2(pLinePeopleContactMobile("ContactNo"))
                                    clsPeople.PersonContactType = CStr2(pLinePeopleContactMobile("ContactType"))
                                    clsPeople.SavePeopleContactMobile()
                                End If
                            Loop '''End People Mobile

                        End If
                    Loop '''Check Library People Organization



                End If
            Loop '''Save People INformation


            '''Next subscriber
            ProcessDatabaseProvisioning()

        End Function

        <HttpPost()>
        <AsyncTimeout(Integer.MaxValue)>
        Public Overloads Sub ExecuteAsync()

            AsyncManager.OutstandingOperations.Increment()

            pLineOrg = pCsvReaderOrgInfo.ReadLine
            If pLineOrg Is Nothing Then
                GenerateStatusUpdate()
                Return
            End If

            strStatus = "Restoring database. "

            OrgID = CStr2(pLineOrg("ID"))
            mDbProvisioner.OrgID = OrgID
            mDbProvisioner.Execute()

        End Sub

        Public Function ExecuteCompleted(vOut As JsonDictionary) As String

            Return vOut.Serialize()

        End Function

        Private Sub mDbProvisioner_Completed(sender As Object, e As DbProvisioner.CompletedArgs) Handles mDbProvisioner.Completed
            Dim clsOrganization As New Organization
            Dim pJson As JsonDictionary
            Dim p As New InsertUpdate("LibraryOrganizationEmployee")
            Dim pSql As String
            Dim objRow(1) As Object

            pJson = New JsonDictionary
            If e.Success Then

                '''Set Synonyms for the Newly restored DBase
                With New Synonyms()
                    .Prefix = "Mobiz"

                    Try
                        .SourceDatabase = "Mobiz"
                        .ProData = ProData.CreateFromConnectionInfo( _
                            CType(sender, DbProvisioner).ProData.Server, _
                            "Mobiz",
                            CType(sender, DbProvisioner).ProData.Username, _
                            CType(sender, DbProvisioner).ProData.Password,
                            CType(sender, DbProvisioner).ProData.IntegratedSecurity)
                        .Generate()
                    Catch ex As System.Exception
                        AppendToDebug(, True, ex, True)
                    End Try

                    Try
                        .ProData = ProData.CreateFromConnectionInfo( _
                            CType(sender, DbProvisioner).ProData.Server, _
                            e.DatabaseName,
                            CType(sender, DbProvisioner).ProData.Username, _
                            CType(sender, DbProvisioner).ProData.Password,
                            CType(sender, DbProvisioner).ProData.IntegratedSecurity)
                        .Generate()
                    Catch ex As System.Exception
                        AppendToDebug(, True, ex, True)
                    End Try
                End With

                '''Use New Restored DBase
                pSql = "USE [" & e.DatabaseName & "]"
                CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)

                '''Save Admininstrator as and Employee of Organization
                strStatus = "Populating database. "

                pCsvReaderPeopleOrganization = New CsvReader(pFilename, "LibraryPeopleOrganization")
                Do
                    pLinePeopleOrganization = pCsvReaderPeopleOrganization.ReadLine
                    If pLinePeopleOrganization Is Nothing Then Exit Do

                    If (CStr2(pLinePeopleOrganization("OrganizationID")).Trim().ToLower() = CStr2(OrgID).Trim().ToLower()) Then

                        p = New InsertUpdate("LibraryOrganizationEmployee")
                        p("PeopleID") = ReadyString(CStr2(pLinePeopleOrganization("PeopleID")))
                        p("DateHired") = ReadyString(CStr2(pLinePeopleOrganization("DateTimeInvited")))
                        p("DeActivated") = "0"
                        p("DeactivationDate") = "NULL"
                        p("DeactivationReason") = "''"
                        p("Comments") = "''"
                        p("IsBan") = "0"
                        p("LastUpdated") = "GetDate()"
                        p("ID") = ReadyString(CStr2(pLinePeopleOrganization("ID")))
                        pSql = p.GetInsert()
                        CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)

                        '''Send Employment confirmation --edited. automatic confirm
                        p = New InsertUpdate("MobizLibraryPeopleOrganization")
                        p("PeopleID") = ReadyString(CStr2(pLinePeopleOrganization("PeopleID")))
                        p("OrganizationID") = ReadyString(OrgID)
                        p("IsTerminated") = "0"
                        p("DateTimeTerminated") = "NULL"
                        p("IsConfirmed") = "1"
                        p("DateTimeConfirmed") = "GetDate()"
                        p("UserID") = "'D4FE8FC8-06B5-4C61-8882-CDD9E1DD2BA4'"
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)

                        '''Give Super Administrator Role
                        p = New InsertUpdate("LibrarySecurityRoleUser")
                        p("PeopleID") = ReadyString(CStr2(pLinePeopleOrganization("PeopleID")))
                        p("RoleID") = ReadyString("474F9F49-2876-4391-A005-107EB18F2762") '''Fixed ID for Super Admin Role
                        p("UserID") = "'D4FE8FC8-06B5-4C61-8882-CDD9E1DD2BA4'"
                        p("LastUpdated") = "GetDate()"
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)

                    End If
                Loop '''Save Admininstrator as and Employee of Organization

                objRow(0) = CStr2(OrgID)
                objRow(1) = "Success"
            Else
                objRow(0) = CStr2(OrgID)
                objRow(1) = "Failed : " & e.ErrorMessage
            End If

            '''Save Status as Success
            pDTStatusUpdate.Rows.Add(objRow)

            AsyncManager.Parameters("vOut") = pJson
            AsyncManager.OutstandingOperations.Decrement()

            ''' Restore Next Org
            ExecuteAsync()

        End Sub

        Public Function GenerateStatusUpdate() As String
            Dim webClient As New Net.WebClient
            Dim pZip As ProZip
            Dim pZipFilename As String

            pZipFilename = Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioningResult.zip"
            If IO.File.Exists(pZipFilename) Then
                IO.File.Delete(pZipFilename)
            End If
            CreateFolder(pZipFilename)
            pZip = New ProZip(pZipFilename)
            pZip.CsvEncoderMaxLength = 10000 ' example lang ni

            pZip.AddTableCsv(pDTStatusUpdate, "DBaseProvisioningResult")

            '''Inform WinHost for the result
            webClient.UploadFile("http://mobiz-apps.com/Confirmed/DBProvisioningResult", pZipFilename)

            strStatus = "Success"

        End Function


    End Class

End Namespace