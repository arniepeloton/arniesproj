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
        Public Property pSql As String

        Function Erp() As ActionResult
            Return View()
        End Function

        Function SendEmailToInformMobizForNewCustomer() As String
            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim strBody As String

                strBody = "<table border='0' cellpadding='0' cellspacing='0' style='font-family: monospace; max-width:600px;'>" +
                            "<tbody>" +
                                "<tr><td colspan='2' style='color:#fff; background-color:#4caf50; font-size:20px; padding:20px;'>We have a new customer!</td></tr>" +
                                "<tr><td colspan='2' style='color:#fff; font-size:20px; background-color:#28435d; padding:20px;'>Company Profile</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Corporate Name:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("CorporateName") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Corporate Name Initials:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("CorporateNameInitial") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business or Trade Name:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("BusinessName") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business or Trade Name Initials:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("BusinessTradeNameInitial") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Ownership Type:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("Ownership") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Are you a franchisor?</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("IsFranchisor") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Registration Type:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("RegistrationType") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Registration Number:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("RegistrationNo") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Registration Date:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("RegistrationDate") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Corporate TIN:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("TinNumber") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business Industry:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("BusinessIndustry") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Office Address:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("OfficeAddress") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business Address:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("BusinessAddress") + "</td></tr>" +
                                "<tr><td colspan='2' style='color:#fff; font-size:20px; background-color:#2f608e; padding:20px;'>Administrator Profile</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Fullname:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("AdministratorName") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Generational Suffix:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("PersonalSuffixName") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Nickname:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("Nickname") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Gender:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("Gender") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Date of Birth:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + Format(data("DateOfBirth"), "MMMM d, yyyy") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Mother's Maiden Name:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("MothersName") + "</td></tr>" +
                                "<tr><td colspan='2' style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c; text-align:center;'>Contact Information</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Mobile Number:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>+63." + data("MobileNumber") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Email Address:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("EmailAddress") + "</td></tr>" +
                                "<tr><td colspan='2' style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c; text-align:center;'>Primary ID Reference</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Primary ID Type:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("IDType") + "</td></tr>" +
                                "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>" + data("IDType") + " ID:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + data("IDNo") + "</td></tr>" +
                                "<tr><td colspan='2' style='color:#fff; font-size:20px; background-color:#2196f3; padding:20px;'>To activate please go to <a href='http://mobiz-apps.com/MobizPortal' style='font-weight:bold; color:#FFF;'>Mobiz Portal</a></td></tr>" +
                            "</tbody>" +
                        "</table>"
#If DEBUG Then
                SendMail("mobizcompleteretail@outlook.com",
                     "arniepeloton17@gmail.com",
                     "Mobiz",
                     "New Mobiz Customer",
                     strBody,
                     "smtp.live.com",
                     587,
                     "mobizcompleteretail@outlook.com",
                     "m0b!zp30pl30nly!", , , , , True)
#Else
                SendMail("mobizcompleteretail@outlook.com",
                    "arniepeloton17@gmail.com, robert.chong@mobiz-apps.com",
                    "Mobiz",
                    "New Mobiz Customer",
                    strBody,
                    "smtp.live.com",
                    587,
                    "mobizcompleteretail@outlook.com",
                    "m0b!zp30pl30nly!", , , , , True)
#End If
                Return JsonDictionary.Serialize("Results", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetSubscriberList() As String
            Dim keywords As String = Common.WebRequest.Data("keywords")
            pSql = "SELECT org.ID, org.CompanyName FROM MobizLibrarySubscription as sub " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganization as org ON org.ID = sub.OrganizationID " & vbCrLf &
                    IIf(keywords = "", "", " WHERE org.CompanyName LIKE " & ReadyStringWild(keywords)) & vbCrLf &
                    " ORDER BY org.CompanyName ASC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function CheckForSubscriber() As ActionResult

            DownloadNewSubscriber()

            Return View()

        End Function

        <HttpPost()>
        Function GetStatus() As String
            Dim strReturn As String

            strReturn = JsonDictionary.Serialize("Status", strStatus)

            Return strReturn

        End Function

#If DEBUG Then
        Public Function DownloadNewSubscriber() As String
#Else
        <HttpPost()>
        Function DownloadNewSubscriber()
#End If
            Dim clsOrganization As New Organization
            Dim webClient As New Net.WebClient

            strStatus = ""
            pFilename = Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioning.zip"

#If DEBUG Then
            Dim pSourceFile As String = Common.ServerPath & "data\DBaseProvisioning.zip"
            If IO.File.Exists(pSourceFile) Then
                CreateFolder(pFilename)
                CopyFile(pSourceFile, pFilename)
            Else
                webClient.DownloadFile("http://192.168.1.110:8080/Confirmed/GetSubscribers", pFilename)
            End If
#Else
            webClient.DownloadFile("http://mobiz-apps.com/Confirmed/GetSubscribers", pFilename)
#End If

            '''Check Library Organization
            pCsvReaderOrgInfo = New CsvReader(pFilename, "LibraryOrganization")

            pDTStatusUpdate = New DataTable
            pDTStatusUpdate.Columns.Add("OrganizationID")
            pDTStatusUpdate.Columns.Add("Status")

            ProcessDatabaseProvisioning()
            CopyTransSubscription(pFilename)
            CopyTransSubscriptionCart(pFilename)
            CreateSubscriptionUsage()

            Return "ok"
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
            clsOrganization.ReferenceNo = CStr2(pLineOrg("ReferenceNo"))

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
                            clsPeople.DTRPIN = CStr2(pLinePeople("DTRPIN"))
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

        Private Function CopyTransSubscription(vFilename As String) As Boolean

            Dim pCsvReader As CsvReader
            Dim pBinder As DictionaryBinder
            Dim pLine As Dictionary(Of String, String)
            Dim pError As String = ""
            Dim pIU As InsertUpdate
            Dim pSql As String

            Try
                pCsvReader = New CsvReader(vFilename, "TransSubscription")
                pBinder = New DictionaryBinder
                With pBinder.Items
                    .Add("ID", True, SqlDbType.UniqueIdentifier)
                    .Add("SubscriptionNo", False, SqlDbType.Decimal)
                    .Add("SubscriptionDate", False, SqlDbType.Date)
                    .Add("OrganizationID", True, SqlDbType.UniqueIdentifier)
                    .Add("SuperAdminID", False, SqlDbType.UniqueIdentifier)
                    .Add("PromoCode", False, SqlDbType.VarChar)
                    .Add("LastUpdated", False, SqlDbType.DateTime)
                    .Add("IsSubscriptionAgreementAgreed", False, SqlDbType.Bit)
                    .Add("AgentName", False, SqlDbType.VarChar)
                    .Add("SourceOfInfo", False, SqlDbType.VarChar)
                    .Add("TransferStatus", False, SqlDbType.VarChar)
                End With
                Do
                    pLine = pCsvReader.ReadLine
                    If pLine Is Nothing Then Exit Do
                    pBinder.Source = pLine
                    If pBinder.Validate(pError) = False Then
                        Continue Do
                    End If
                    pIU = pBinder.CreateInsertUpdate(pError)
                    If pIU Is Nothing Then
                        Continue Do
                    End If
                    pIU.TableName = "TransSubscription"
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                    If Common.WebRequest.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Loop
            Catch ex As Exception
                AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), "temp\logs\SubscriptionController", Format(Now, "yyyy_MM_dd"))
            End Try

        End Function ' CopyTransSubscription

        Private Function CopyTransSubscriptionCart(vFilename As String) As Boolean

            Dim pCsvReader As CsvReader
            Dim pBinder As DictionaryBinder
            Dim pLine As Dictionary(Of String, String)
            Dim pError As String = ""
            Dim pIU As InsertUpdate
            Dim pSql As String

            Try
                pCsvReader = New CsvReader(vFilename, "TransSubscriptionCart")
                pBinder = New DictionaryBinder
                With pBinder.Items
                    .Add("ID", True, SqlDbType.UniqueIdentifier)
                    .Add("SubscriptionID", True, SqlDbType.UniqueIdentifier)
                    .Add("ProductID", True, SqlDbType.UniqueIdentifier)
                    .Add("Quantity", True, SqlDbType.Int, "Qty")
                    .Add("ActualQuantity", False, SqlDbType.Decimal)
                    .Add("UnitMeasure", False, SqlDbType.VarChar)
                    .Add("UnitPrice", False, SqlDbType.Money)
                    .Add("UnitDiscount", False, SqlDbType.Float)
                    .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier)
                    .Add("SubscriptionDuration", False, SqlDbType.Decimal)
                    .Add("SubscriptionUnit", False, SqlDbType.VarChar)
                    .Add("SubscriptionDiscount", False, SqlDbType.Float)
                    .Add("UnitNetPrice", False, SqlDbType.Float)
                    .Add("SubscriptionStatus", False, SqlDbType.VarChar)
                    .Add("ActivationDate", False, SqlDbType.Date)
                End With
                Do
                    pLine = pCsvReader.ReadLine
                    If pLine Is Nothing Then Exit Do
                    pBinder.Source = pLine
                    If pBinder.Validate(pError) = False Then
                        Continue Do
                    End If
                    pIU = pBinder.CreateInsertUpdate(pError)
                    If pIU Is Nothing Then
                        Continue Do
                    End If
                    pIU.TableName = "TransSubscriptionCart"
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                    If Common.WebRequest.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                Loop
            Catch ex As Exception
                AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), "temp\logs\SubscriptionController", Format(Now, "yyyy_MM_dd"))
            End Try

        End Function ' CopyTransSubscriptionCart

        Private Function CreateSubscriptionUsage() As Integer

            Dim pSql As String
            Dim pDT As DataTable
            Dim pIU As InsertUpdate

            pSql = " SELECT MobizTransSubscriptionCart.ID AS CartID" & vbCrLf &
                " ,MobizTransSubscription.OrganizationID" & vbCrLf &
                " ,MobizTransSubscriptionCart.ProductID" & vbCrLf &
                " ,MobizTransSubscription.SubscriptionDate" & vbCrLf &
                " FROM MobizTransSubscription" & vbCrLf &
                " JOIN MobizTransSubscriptionCart ON MobizTransSubscriptionCart.SubscriptionID = MobizTransSubscription.ID" & vbCrLf &
                " WHERE MobizTransSubscriptionCart.ID NOT IN (" & vbCrLf &
                " 	SELECT MobizTransSubscriptionUsage.CartID" & vbCrLf &
                "     FROM MobizTransSubscriptionUsage" & vbCrLf &
                " )" & vbCrLf &
                " ORDER BY MobizTransSubscription.LastUpdated"
            pDT = Common.WebRequest.ProData.Execute(pSql)
            If pDT.Rows.Count = 0 Then
                Return 0
            End If

            pIU = New InsertUpdate("MobizTransSubscriptionUsage")
            pIU("ID") = "newid()"
            pIU("LastUpdated") = "GETDATE()"
            For Each pDR As DataRow In pDT.Rows
                pIU("CartID") = ReadyString(pDR("CartID"))
                pIU("OrganizationID") = ReadyString(pDR("OrganizationID"))
                pIU("ProductID") = ReadyString(pDR("ProductID"))
                pIU("DateSubscribed") = ReadyString(pDR("SubscriptionDate"))
                pSql = pIU.GetInsert
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next pDR

            Return pDT.Rows.Count

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

        Private Sub AddLog(vText)

            AppendToLog(vText, , "SubscriptionController vbZ2PPPM5F3F!")

        End Sub

        Private Sub mDbProvisioner_Completed(sender As Object, e As DbProvisioner.CompletedArgs) Handles mDbProvisioner.Completed
            Dim clsOrganization As New Organization
            Dim pJson As JsonDictionary
            Dim p As New InsertUpdate("LibraryOrganizationEmployee")
            Dim pSql As String
            Dim objRow(1) As Object
            Dim dtFOWST As DataTable

            pJson = New JsonDictionary
            If e.Success Then
                AddLog("mDbProvisioner_Completed success vbTCZAVBAALN")

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
                AddLog("before loop vbJN4VGLKJAK")
                pCsvReaderPeopleOrganization = New CsvReader(pFilename, "LibraryPeopleOrganization")
                Do
                    pLinePeopleOrganization = pCsvReaderPeopleOrganization.ReadLine
                    If pLinePeopleOrganization Is Nothing Then Exit Do

                    If ReadyGuid(pLinePeopleOrganization("OrganizationID")) = ReadyGuid(OrgID) Then
                        'If (CStr2(pLinePeopleOrganization("OrganizationID")).Trim().ToLower() = CStr2(OrgID).Trim().ToLower()) Then

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
                        AddLog("Inserting LibraryOrganizationEmployee vbEANAC5XLYK!" & vbCrLf &
                            p.GetUpdate("INSERT"))
                        CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)

                        pSql = "Select ID from MobizLibraryOrganizationFOWST where OrganizationID =  " & ReadyString(OrgID)
                        dtFOWST = CType(sender, DbProvisioner).ProData.Execute(pSql)

                        If dtFOWST.Rows.Count > 0 Then
                            ' '''Save Organization FOWST
                            p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                            p("PeopleID") = ReadyString(CStr2(pLinePeopleOrganization("PeopleID")))
                            p("FowstID") = ReadyString(CStr2(dtFOWST.Rows(0).Item("ID")))

                            pSql = p.GetInsert()
                            AddLog("inserting LibraryOrganizationEmployeeFOWST vb37EUNKXWXQ" & vbCrLf &
                                p.GetUpdate("insert"))
                            CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)
                        Else
                            AddLog("org, " & OrgID & " has no fowsts vbFQ6ZEKZ9Q6!")
                        End If

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
                        AddLog("inserting MobizLibraryPeopleOrganization vbR8QNLGZ9PA!" & vbCrLf &
                            p.GetUpdate("insert"))
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

                        '''Give Super Administrator Role
                        p = New InsertUpdate("LibrarySecurityRoleUser")
                        p("PeopleID") = ReadyString(CStr2(pLinePeopleOrganization("PeopleID")))
                        p("RoleID") = ReadyString("474F9F49-2876-4391-A005-107EB18F2762") '''Fixed ID for Super Admin Role
                        p("UserID") = "'D4FE8FC8-06B5-4C61-8882-CDD9E1DD2BA4'"
                        p("LastUpdated") = "GetDate()"
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        CType(sender, DbProvisioner).ProData.ExecuteSQL(pSql)

                        Try
                            Call New SecurityAccess().GrantAllToSysAdmin(CType(sender, DbProvisioner).ProData)
                        Catch ex As Exception
                            AppendToLog(GetStackTrace(, ex), , "SecurityAccess_GrantAllToSysAdmin")
                        End Try
                    End If
                Loop '''Save Admininstrator as and Employee of Organization

                objRow(0) = CStr2(OrgID)
                objRow(1) = "Success"
            Else
                AddLog("mDbProvisioner_Completed failed vbC6VLHLU28F " & e.ErrorMessage)
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
#If DEBUG Then
#Else
            webClient.UploadFile("http://mobiz-apps.com/Confirmed/DBProvisioningResult", pZipFilename)
#End If
            strStatus = "Success"

        End Function


    End Class

End Namespace