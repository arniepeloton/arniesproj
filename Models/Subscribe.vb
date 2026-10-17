Public Class Subscribe

    Public Property OrganizationID As String
    Public Property PeopleID As String

    ' People
    Public Property Surname As String
    Public Property FirstName As String
    Public Property MiddleName As String
    Public Property PersonalSuffixName As String
    Public Property DateOfBirth As Date
    Public Property NickName As String
    Public Property MotherMaidenSurname As String
    Public Property MotherMaidenFirstName As String
    Public Property MotherMaidenMiddleName As String

    Public Property Gender As String

    Public Property UserName As String
    Public Property UserPassword As String

    Public Property SecurityQuestion1 As String
    Public Property SecurityQuestion1Answer As String
    Public Property SecurityQuestion2 As String
    Public Property SecurityQuestion2Answer As String

    Public Property PersonContactCountryIDMobile As String
    Public Property PersonContactTelCoPrefixID As String
    Public Property PersonContactNoMobile As String
    Public Property PersonContactEmail As String
    Public Property ContactType As String
    Public Property EmailType As String

    Public Property PrimaryIDType As String
    Public Property SSSNo As String
    Public Property PhilHealthNo As String
    Public Property TIN As String
    Public Property HDMFNo As String
    Public Property NBINo As String
    Public Property PassportNo As String
    Public Property DriversLicenseNo As String

    Public Property IsCertifiedCorrect As String

    Public Property EmailActivationLinkID As String
    Public Property ConfirmationCode As String
    Public Property IsEmailConfirmed As Boolean

    Public Property DTRPin As String

    ' Organization
    Public Property MaxReturnRow As Integer = 50

    Public Property CompanyName As String
    Public Property CompanyNameInitial As String

    Public Property OrgTradeNameList As Object()

    Public Property OwnershipType As String
    Public Property IsFranchisor As String
    Public Property RegistrationType As String
    Public Property RegistrationNo As String
    Public Property RegistrationDate As String

    Public Property CorporateTIN As String

    Public Property CategoryTypeID As String

    Public Property OfficeAddressBuildingNo As String
    Public Property OfficeAddressStreet As String
    Public Property OfficeAddressCityID As String
    Public Property OfficeAddressCountryID As String

    Public Property BusinessAddressBuildingNo As String
    Public Property BusinessAddressStreet As String
    Public Property BusinessAddressCityID As String
    Public Property BusinessAddressCountryID As String

    Public Property BusinessTradeName As String
    Public Property BusinessTradeNameInitial As String

    Public Property IsOrganization As String
    Public Property ReferenceNo As String

    Public Property ProductID As String
    Public Property Qty As Integer
    Public Property Price As Double
    Public Property TotalAmount As Double

    Public Property InvoiceID As String
    Public Property CartID As String
    Public Property IsUpdate As Boolean

    Public Property SubscriptionCartID As String

    Public Property PromoCode As String
    Public Property AgentName As String
    Public Property SourceOfInfo As String

    Public Property ContactEmailID As String

    ' ORGANIZATION FUNCTIONS
    Function CheckOrganizationUnique() As DataTable
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
            End With

            .SqlFrom = "FROM LibraryOrganization org "

            .SqlWhere = "WHERE org.CompanyName = " & ReadyString(CompanyName)

            pDT = .Requery
        End With

        Return pDT

    End Function


    Function SaveOrganizationInfo() As String
        Dim p As New InsertUpdate("LibraryOrganization")
        Dim pSql As String
        Dim strIDReturn As String
        Try

            p("IsOrganization") = IsOrganization

            p("CompanyName") = ReadyString(CompanyName)
            p("CompanyNameInitial") = ReadyString(CompanyNameInitial)
            p("OwnershipType") = ReadyString(OwnershipType)
            p("IsFranchisor") = IsFranchisor

            p("RegistrationType") = ReadyString(RegistrationType)

            'save only if NOT unregistered
            If (RegistrationType <> "Unregistered") Then
                p("RegistrationDate") = ReadyString(RegistrationDate)
                p("RegistrationNo") = ReadyString(RegistrationNo)
            End If

            p("TinNumber") = ReadyString(CorporateTIN)
            p("IsABank") = "0"
            p("OfficeAddressBuildingNo") = ReadyString(OfficeAddressBuildingNo)
            p("OfficeAddressStreet") = ReadyString(OfficeAddressStreet)
            p("OfficeAddressCityID") = IIf(OfficeAddressCityID.Trim().Length > 0, ReadyString(OfficeAddressCityID), "NULL")
            p("OfficeAddressCountryID") = IIf(OfficeAddressCountryID.Trim().Length > 0, ReadyString(OfficeAddressCountryID), "NULL")

            p("BusinessAddressBuildingNo") = ReadyString(BusinessAddressBuildingNo)
            p("BusinessAddressStreet") = ReadyString(BusinessAddressStreet)
            p("BusinessAddressCityID") = IIf(BusinessAddressCityID.Trim().Length > 0, ReadyString(BusinessAddressCityID), "NULL")
            p("BusinessAddressCountryID") = IIf(BusinessAddressCountryID.Trim().Length > 0, ReadyString(BusinessAddressCountryID), "NULL")

            p("LastUpdated") = "DateAdd(HH,16,Getdate())"

            If ReferenceNo.Length > 0 Then
                p("ReferenceNo") = ReadyString(ReferenceNo)
            End If

            If (OrganizationID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = OrganizationID
                pSql = p.GetUpdate(" ID = " & ReadyString(OrganizationID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ' '''Save Organization Trade Name
            p = New InsertUpdate("LibraryOrganizationTradeName")

            p("OrganizationID") = ReadyString(strIDReturn)
            p("BusinessTradeName") = ReadyString(BusinessTradeName)
            p("BusinessTradeNameInitial") = ReadyString(BusinessTradeNameInitial)

            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ' '''Save Organization Industry Class
            p = New InsertUpdate("LibraryOrganizationIndustryClass")

            p("OrganizationID") = ReadyString(strIDReturn)
            p("IndustryClassID") = ReadyString(CategoryTypeID)

            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn
        Catch ex As Exception
            Return ex.Message
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function

    ' PEOPLE FUNCTIONS
    Function CheckPeopleUniqueName(ByVal IsFullCheckName As Boolean) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        Dim a As String = OrganizationID
        Dim b As String = HttpContext.Current.Request.QueryString("OrgID")

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
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
                With .Add("PersonalSuffixName", SqlDbType.VarChar)
                    .Expression = "ppol.PersonalSuffixName"
                End With
                With .Add("DateOfBirth", SqlDbType.VarChar)
                    .Expression = "ppol.DateOfBirth"
                End With
                With .Add("MotherMaidenSurname", SqlDbType.VarChar)
                    .Expression = "ppol.MotherMaidenSurname"
                End With
                With .Add("MotherMaidenFirstName", SqlDbType.VarChar)
                    .Expression = "ppol.MotherMaidenFirstName"
                End With
                With .Add("MotherMaidenMiddleName", SqlDbType.VarChar)
                    .Expression = "ppol.MotherMaidenMiddleName"
                End With
            End With

            .SqlFrom = "FROM LibraryPeople ppol "

            If (IsFullCheckName) Then
                .SqlWhere = "WHERE ppol.ID " & IIf(PeopleID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(PeopleID)) &
                            " AND ppol.Surname = " & ReadyString(Surname) &
                            " AND ppol.FirstName = " & ReadyString(FirstName) &
                            " AND ppol.MiddleName = " & ReadyString(MiddleName) &
                            " AND ppol.DateOfBirth = " & ReadyString(DateOfBirth) &
                            " AND ppol.MotherMaidenSurname = " & ReadyString(MotherMaidenSurname) &
                            " AND ppol.MotherMaidenFirstName = " & ReadyString(MotherMaidenFirstName) &
                            " AND ppol.MotherMaidenMiddleName = " & ReadyString(MotherMaidenMiddleName) &
                            " AND ppol.PersonalSuffixName = " & ReadyString(PersonalSuffixName)
            Else
                .SqlWhere = "WHERE ppol.ID " & IIf(PeopleID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(PeopleID)) &
                            " AND ppol.Surname = " & ReadyString(Surname) &
                            " AND ppol.FirstName = " & ReadyString(FirstName) &
                            " AND ppol.MiddleName = " & ReadyString(MiddleName)
            End If

            pDT = .Requery
        End With

        Return pDT

    End Function


    Function CheckPeopleUniqueUserName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.PeopleID"
                End With
                With .Add("Username", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.Username"
                End With
            End With

            .SqlFrom = "FROM LibraryPeopleUserAccount ppol "

            .SqlWhere = "WHERE ppol.Username = " & ReadyString(UserName)


            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetEmailActivationLinkID() As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("EmailActivationLinkID", SqlDbType.UniqueIdentifier)
                    .Expression = "EmailActivationLinkID"
                End With
            End With

            .SqlFrom = "FROM LibraryPeopleContactEmail "

            .SqlWhere = "WHERE PeopleID = " & ReadyString(PeopleID)


            pDT = .Requery
        End With

        Return pDT.Rows(0).Item("EmailActivationLinkID").ToString()

    End Function

    'Get Mobiz Sales agent code / Promo code
    Function GetAgentCode() As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("SalesAgentCode", SqlDbType.VarChar)
                    .Expression = "SalesAgentCode"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganization "

            .SqlWhere = "WHERE CompanyName = " & ReadyString("Mobiz Inc.")


            pDT = .Requery
        End With

        Return pDT.Rows(0).Item("SalesAgentCode").ToString()

    End Function

    Function SavePeopleInfo() As String
        Dim p As New InsertUpdate("LibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String = ""

        Try
            p("Surname") = ReadyString(Surname)
            p("FirstName") = ReadyString(FirstName)
            p("MiddleName") = ReadyString(MiddleName)
            p("PersonalSuffixName") = ReadyString(PersonalSuffixName)
            p("DateOfBirth") = ReadyString(DateOfBirth)
            p("NickName") = ReadyString(NickName)
            p("MotherMaidenSurname") = ReadyString(MotherMaidenSurname)
            p("MotherMaidenFirstName") = ReadyString(MotherMaidenFirstName)
            p("MotherMaidenMiddleName") = ReadyString(MotherMaidenMiddleName)
            p("Gender") = ReadyString(Gender)

            If (PrimaryIDType = "SSS") Then
                p("SSSNo") = ReadyString(SSSNo)
            End If

            If (PrimaryIDType = "Phil. Health") Then
                p("PhilHealthNo") = ReadyString(PhilHealthNo)
            End If

            If (PrimaryIDType = "TIN") Then
                p("TIN") = ReadyString(TIN)
            End If

            If (PrimaryIDType = "HDMF") Then
                p("HDMFNo") = ReadyString(HDMFNo)
            End If

            If (PrimaryIDType = "NBI") Then
                p("NBINo") = ReadyString(NBINo)
            End If

            If (PrimaryIDType = "Passport") Then
                p("PassportNo") = ReadyString(PassportNo)
            End If

            If (PrimaryIDType = "Driver's License") Then
                p("DriversLicenseNo") = ReadyString(DriversLicenseNo)
            End If

            p("IsCertifiedCorrect") = IsCertifiedCorrect
            p("IsFirstPartySignUp") = 1 ' set default to 1. super admin is signing up as his own
            p("IsTermAgreed") = 1

            p("LastUpdated") = "DateAdd(HH,16,Getdate())"

            p("DTRPIN") = ReadyString(DTRPin)

            If (PeopleID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = PeopleID
                pSql = p.GetUpdate(" ID = " & ReadyString(PeopleID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ' '''Save User Account
            p = New InsertUpdate("LibraryPeopleUserAccount")

            p("PeopleID") = ReadyString(strIDReturn)
            p("Username") = ReadyString(UserName)
            p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(UserPassword))
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ' '''Save Security Question1
            p = New InsertUpdate("LibraryPeopleSecurityQuestion")

            p("PeopleID") = ReadyString(strIDReturn)
            p("SecurityQuestion") = ReadyString(SecurityQuestion1)
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion1Answer)) 'ReadyString(SecurityQuestion1Answer)
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If (SecurityQuestion2.Trim().Length > 0) Then
                ' '''Save Security Question2
                p = New InsertUpdate("LibraryPeopleSecurityQuestion")

                p("PeopleID") = ReadyString(strIDReturn)
                p("SecurityQuestion") = ReadyString(SecurityQuestion2)
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion2Answer)) 'ReadyString(SecurityQuestion2Answer)
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If


            ' '''Save Email Address
            p = New InsertUpdate("LibraryPeopleContactEmail")

            p("PeopleID") = ReadyString(strIDReturn)
            p("IsEmailConfirmed") = 0
            p("EmailActivationLinkID") = ReadyString(Guid.NewGuid.ToString())
            p("LastUpdated") = "DateAdd(HH,16,Getdate())"

            If CStr2(PersonContactEmail) <> "" Then
                p("EmailAddress") = ReadyString(PersonContactEmail)
                p("EmailType") = ReadyString(EmailType)
                
                ' Save only if the user entered an email address
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            ' '''Save Mobile Contact
            p = New InsertUpdate("LibraryPeopleContactMobile")

            p("PeopleID") = ReadyString(strIDReturn)

            If CStr2(PersonContactCountryIDMobile) <> "" Then
                p("CountryID") = ReadyString(PersonContactCountryIDMobile)
            End If

            If CStr2(PersonContactTelCoPrefixID) <> "" Then
                p("TelCoPrefixID") = ReadyString(PersonContactTelCoPrefixID)
            End If

            If CStr2(PersonContactNoMobile) <> "" Then
                p("ContactNo") = ReadyString(PersonContactNoMobile)
                p("ContactType") = ReadyString(ContactType)
                ' Save only if the user entered a mobile number
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If


            ' '''Save People Organization
            p = New InsertUpdate("LibraryPeopleOrganization")

            p("PeopleID") = ReadyString(strIDReturn)
            p("IsConfirmed") = 0 'set to 0 by default. it means, the user needs to wait for the email confirmation
            p("DateTimeInvited") = "DateAdd(HH,16,Getdate())"

            If CStr2(OrganizationID) <> "" Then
                p("OrganizationID") = ReadyString(OrganizationID)

                ' Save only if there's a valid org id
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If


            ' '''Save Security Role
            p = New InsertUpdate("LibrarySecurityRoleUser")

            p("RoleID") = ReadyString("474F9F49-2876-4391-A005-107EB18F2762") 'Super Admin role id
            p("PeopleID") = ReadyString(strIDReturn)
            p("UserID") = ReadyString(strIDReturn)
            p("LastUpdated") = "getdate()"

            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()
            Return strIDReturn

        Catch ex As Exception
            Return ex.Message
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

    End Function

    Function SaveSubscriptionInfo() As String
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")
        Dim p As New InsertUpdate("TransSubscription")
        Dim pSql As String
        Dim strIDReturn As String = ""


        p("SubscriptionDate") = "DateAdd(HH,16,Getdate())"
        p("OrganizationID") = ReadyString(OrganizationID)
        p("SuperAdminID") = ReadyString(PeopleID)
        p("PromoCode") = ReadyString(PromoCode)

        p("SourceOfInfo") = ReadyString(SourceOfInfo)

        'if not empty
        If (AgentName <> "Name of Mobiz representative" Or AgentName <> String.Empty) Then
            p("AgentName") = ReadyString(AgentName)
        End If
        p("TransferStatus") = ReadyString("Pending")
        p("IsSubscriptionAgreementAgreed") = 1
        p("LastUpdated") = "DateAdd(HH,16,Getdate())"

        'this is the final invoice id
        strIDReturn = Guid.NewGuid.ToString()
        p("ID") = ReadyString(strIDReturn)
        pSql = p.GetInsert()


        Common.WebRequest.ProData.BeginTransaction()
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Common.WebRequest.ProData.CommitTransaction()


        Return strIDReturn

    End Function

    Function SaveSubscriptionCartInfo() As String
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")
        Dim p As New InsertUpdate("TransSubscriptionCart")
        Dim pSql As String
        Dim strIDReturn As String = ""

        Try
            p("SubscriptionID") = ReadyString(InvoiceID)
            p("ProductID") = ReadyString(ProductID)
            p("Qty") = ReadyString(Qty)
            p("UnitPrice") = ReadyString(Price)
            p("SubscriptionStatus") = ReadyString("Pending")

            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()


        Catch ex As Exception
            strIDReturn = "Error"
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SaveConfirmationDetails() As String
        Dim p As New InsertUpdate("LibraryPeopleContactEmail")
        Dim pSql As String
        Dim strReturn As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDT2 As DataTable

        Try
            ' GET PeopleID
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                        .Expression = "PeopleID"
                    End With
                End With

                .SqlFrom = " FROM LibraryPeopleContactEmail " &
                    " WHERE ID = " & ReadyString(ContactEmailID)

                pDT = .Requery
            End With

            p("ConfirmationCode") = ReadyString(ConfirmationCode)
            p("IsEmailConfirmed") = 1 'set to 1. email is confirmed


            Common.WebRequest.ProData.BeginTransaction()

            pSql = p.GetUpdate(" ID = " & ReadyString(ContactEmailID))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If (pDT.Rows.Count > 0) Then
                'update parent table people
                p = New InsertUpdate("LibraryPeople")

                p("LastUpdated") = "DateAdd(HH,16,Getdate())"

                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("PeopleID").ToString()))
                Common.WebRequest.ProData.ExecuteSQL(pSql)


                'Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")

                ' '''Update Lastupdated in parent table (Subscription)
                p = New InsertUpdate("TransSubscription")
                p("TransferStatus") = ReadyString("Confirmed")
                p("LastUpdated") = "DateAdd(HH,16,Getdate())"

                pSql = p.GetUpdate(" SuperAdminID = " & ReadyString(pDT.Rows(0).Item("PeopleID").ToString()))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                'TODO - will change after the free trial period
                'for the free version of mobiz complete retail, this is the trigger that the details should be sent to our server
                'if not free, save only if the user has paid the subscription fee
                'SAVE FROM WINHOST DB TO MOBIZ DB

               
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' GET OrganizationID
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                        .Expression = "OrganizationID"
                    End With
                End With

                .SqlFrom = " FROM LibraryPeopleOrganization " &
                    " WHERE PeopleID = " & ReadyString(pDT.Rows(0).Item("PeopleID").ToString())

                pDT2 = .Requery
            End With


            If (pDT2.Rows.Count > 0) Then

                'additional aug 5 2014 set status 
                p = New InsertUpdate("LibraryOrganization")
                p("LastUpdated") = "DateAdd(HH,16,Getdate())"

                pSql = p.GetUpdate(" ID = " & ReadyString(pDT2.Rows(0).Item("OrganizationID").ToString()))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If


            Common.WebRequest.ProData.CommitTransaction()

            strReturn = "True"
        Catch ex As Exception
            strReturn = ex.Message
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strReturn

    End Function


    Function UpdateTransferStatus(ByVal id As String, ByVal status As String) As String
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")
        Dim pSql As String
        Dim p As New InsertUpdate("TransSubscription")
        Try

            ' '''Update Lastupdated in parent table (Subscription)
            p = New InsertUpdate("TransSubscription")

            If status.Length > 50 Then
                p("TransferStatus") = ReadyString(status.Substring(0, 50))
            Else
                p("TransferStatus") = ReadyString(status)
            End If
  
            p("LastUpdated") = "DateAdd(HH,16,Getdate())"

            pSql = p.GetUpdate(" OrganizationID = " & ReadyString(id))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return "ok"

        Catch ex As Exception
            Return "Error"
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

    End Function


    Function CheckConfirmedEmail() As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        Try
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = MaxReturnRow

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "ID"
                    End With
                End With

                .SqlFrom = "FROM LibraryPeopleContactEmail "

                .SqlWhere = "WHERE EmailActivationLinkID = " & ReadyString(EmailActivationLinkID) & " AND ConfirmationCode IS NULL"

                pDT = .Requery
            End With

            If (pDT.Rows.Count > 0) Then
                Return pDT.Rows(0).Item("ID").ToString() 'get ID
            Else
                Return "Confirmed"
            End If

        Catch ex As Exception
            Return "Error"
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

    End Function

    Function CheckOrganizationIDExists() As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable

        Try
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = MaxReturnRow

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "ID"
                    End With
                End With

                .SqlFrom = "FROM LibraryOrganization "

                .SqlWhere = "WHERE ID = " & ReadyString(OrganizationID)

                pDT = .Requery
            End With

            If (pDT.Rows.Count > 0) Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

    End Function

    Function getProductList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable


        'Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")

        pFinder = New Finder

        Try
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "pimpro.ID"
                        .Searchable = False
                    End With
                    With .Add("Price", SqlDbType.Money)
                        .Expression = "pricelevel.Price"
                        .Searchable = False
                    End With
                    With .Add("ProductName", SqlDbType.UniqueIdentifier)
                        .Expression = "pimpro.ProductName"
                    End With
                    With .Add("IsActive", SqlDbType.UniqueIdentifier)
                        .Expression = "pimpro.IsActive"
                    End With
                End With

                .SqlFrom = " FROM LibraryPIMProduct pimpro " &
                                " INNER JOIN LibraryOrganizationPIM orgpim ON (pimpro.ID = orgpim.ProductID) " &
                                " INNER JOIN LibraryPIMProductPriceLevel pricelevel ON (pimpro.ID = pricelevel.ProductID) " &
                                " WHERE orgpim.IsDeactivated = 0" 'Show only active products

                .SqlOrder = "ORDER BY pimpro.ProductName asc"

                pDT = .Requery
            End With

            Return pDT
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

    End Function

    Function getSubscriptionDuration() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        Try
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "pimpro.ID"
                        .Searchable = False
                    End With
                    With .Add("SubscriptionDuration", SqlDbType.Int)
                        .Expression = "pimpro.SubscriptionDuration"
                    End With
                    With .Add("SubscriptionUnitMeasureID", SqlDbType.UniqueIdentifier)
                        .Expression = "pimpro.SubscriptionUnitMeasureID"
                    End With
                    With .Add("SubscriptionUnitMeasure", SqlDbType.VarChar)
                        .Expression = "label.Label"
                    End With
                    With .Add("SubscriptionDiscount", SqlDbType.Float)
                        .Expression = "pimpro.SubscriptionDiscount"
                    End With

                End With

                'TODO synonym
                .SqlFrom = " FROM LibraryPIMProductSubscription pimpro " &
                                " INNER JOIN LibraryPIMMeasure measure ON (pimpro.SubscriptionUnitMeasureID = measure.ID) " &
                                " INNER JOIN LibraryCountriesLanguageLabel label ON (measure.MeasureNameID = label.ID) "

                .SqlWhere = " WHERE pimpro.ProductID = " & ReadyString(ProductID)

                .SqlOrder = "ORDER BY pimpro.SubscriptionDuration asc"

                pDT = .Requery
            End With

            Return pDT
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

    End Function

    ''' <summary>
    ''' '''''''''''''''
    ''' </summary>
    ''' <param name="strID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetOrganizationInfo() As DataTable
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

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
                With .Add("IsOrganization", SqlDbType.Bit)
                    .Expression = "org.IsOrganization"
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
                With .Add("RegistrationDate", SqlDbType.Date)
                    .Expression = "org.RegistrationDate"
                End With
                With .Add("RegistrationNo", SqlDbType.VarChar)
                    .Expression = "org.RegistrationNo"
                End With
                With .Add("TinNumber", SqlDbType.VarChar)
                    .Expression = "org.TinNumber"
                End With
                With .Add("IsABank", SqlDbType.Bit)
                    .Expression = "org.IsABank"
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
                With .Add("OfficeAddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OfficeAddressCountryID"
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
                With .Add("BusinessAddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.BusinessAddressCountryID"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "org.LastUpdated"
                End With
                With .Add("ReferenceNo", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ReferenceNo"
                End With
            End With

            .SqlFrom = " FROM LibraryOrganization org INNER JOIN TransSubscription sub ON org.ID = sub.OrganizationID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed'"
            pDT = .Requery
        End With

        Return pDT

    End Function

    
    Function GetOrgTradeNameInfo() As DataTable
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OrganizationID"
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "org.BusinessTradeName"
                End With
                With .Add("BusinessTradeNameInitial", SqlDbType.VarChar)
                    .Expression = "org.BusinessTradeNameInitial"
                End With

            End With

            .SqlFrom = " FROM LibraryOrganizationTradeName org INNER JOIN TransSubscription sub ON org.OrganizationID = sub.OrganizationID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetOrgIndustryClassInfo() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OrganizationID"
                End With
                With .Add("IndustryClassID", SqlDbType.VarChar)
                    .Expression = "org.IndustryClassID"
                End With

            End With

            .SqlFrom = " FROM LibraryOrganizationIndustryClass org INNER JOIN TransSubscription sub ON org.OrganizationID = sub.OrganizationID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function


    Function GetPeopleInfo() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData


            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.ID"
                End With
                With .Add("Surname", SqlDbType.VarChar)
                    .Expression = "ppl.Surname"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "ppl.FirstName"
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "ppl.MiddleName"
                End With
                With .Add("PersonalSuffixName", SqlDbType.VarChar)
                    .Expression = "ppl.PersonalSuffixName"
                End With
                With .Add("DateOfBirth", SqlDbType.Date)
                    .Expression = "ppl.DateOfBirth"
                End With
                With .Add("NickName", SqlDbType.VarChar)
                    .Expression = "ppl.NickName"
                End With
                With .Add("MotherMaidenSurname", SqlDbType.VarChar)
                    .Expression = "ppl.MotherMaidenSurname"
                End With
                With .Add("MotherMaidenFirstName", SqlDbType.VarChar)
                    .Expression = "ppl.MotherMaidenFirstName"
                End With
                With .Add("MotherMaidenMiddleName", SqlDbType.VarChar)
                    .Expression = "ppl.MotherMaidenMiddleName"
                End With
                With .Add("Gender", SqlDbType.VarChar)
                    .Expression = "ppl.Gender"
                End With
                With .Add("SSSNo", SqlDbType.VarChar)
                    .Expression = "ppl.SSSNo"
                End With
                With .Add("PhilHealthNo", SqlDbType.VarChar)
                    .Expression = "ppl.PhilHealthNo"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "ppl.TIN"
                End With
                With .Add("HDMFNo", SqlDbType.VarChar)
                    .Expression = "ppl.HDMFNo"
                End With
                With .Add("NBINo", SqlDbType.VarChar)
                    .Expression = "ppl.NBINo"
                End With
                With .Add("PassportNo", SqlDbType.VarChar)
                    .Expression = "ppl.PassportNo"
                End With
                With .Add("DriversLicenseNo", SqlDbType.VarChar)
                    .Expression = "ppl.DriversLicenseNo"
                End With
                With .Add("IsCertifiedCorrect", SqlDbType.Bit)
                    .Expression = "ppl.IsCertifiedCorrect"
                End With
                With .Add("IsFirstPartySignUp", SqlDbType.Bit)
                    .Expression = "ppl.IsFirstPartySignUp"
                End With
                With .Add("IsTermAgreed", SqlDbType.Bit)
                    .Expression = "ppl.IsTermAgreed"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "ppl.LastUpdated"
                End With
                With .Add("DTRPIN", SqlDbType.DateTime)
                    .Expression = "ppl.DTRPIN"
                End With

            End With

            .SqlFrom = " FROM LibraryPeople ppl INNER JOIN TransSubscription sub ON ppl.ID = sub.SuperAdminID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetPeopleUserAccountInfo() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.PeopleID"
                End With
                With .Add("Username", SqlDbType.VarChar)
                    .Expression = "ppl.Username"
                End With
                With .Add("UserPassword", SqlDbType.VarChar)
                    .Expression = "ppl.UserPassword"
                End With

            End With

            .SqlFrom = " FROM LibraryPeopleUserAccount ppl INNER JOIN TransSubscription sub ON ppl.PeopleID = sub.SuperAdminID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetPeopleSecurityQuestionInfo() As DataTable
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.ID"
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.PeopleID"
                End With
                With .Add("SecurityQuestion", SqlDbType.VarChar)
                    .Expression = "ppl.SecurityQuestion"
                End With
                With .Add("SecurityAnswer", SqlDbType.VarChar)
                    .Expression = "ppl.SecurityAnswer"
                End With

            End With

            .SqlFrom = " FROM LibraryPeopleSecurityQuestion ppl INNER JOIN TransSubscription sub ON ppl.PeopleID = sub.SuperAdminID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetPeopleContactEmailInfo() As DataTable
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.ID"
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.PeopleID"
                End With
                With .Add("IsEmailConfirmed", SqlDbType.Bit)
                    .Expression = "ppl.IsEmailConfirmed"
                End With
                With .Add("EmailActivationLinkID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.EmailActivationLinkID"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "ppl.LastUpdated"
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "ppl.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "ppl.EmailType"
                End With

            End With

            .SqlFrom = " FROM LibraryPeopleContactEmail ppl INNER JOIN TransSubscription sub ON ppl.PeopleID = sub.SuperAdminID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetPeopleContactMobileInfo() As DataTable
        'Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.ID"
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.PeopleID"
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.CountryID"
                End With
                With .Add("TelCoPrefixID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.TelCoPrefixID"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "ppl.ContactNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppl.ContactType"
                End With

            End With

            .SqlFrom = " FROM LibraryPeopleContactMobile ppl INNER JOIN TransSubscription sub ON ppl.PeopleID = sub.SuperAdminID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetPeopleOrganizationInfo() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.ID"
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.PeopleID"
                End With
                With .Add("IsConfirmed", SqlDbType.Bit)
                    .Expression = "ppl.IsConfirmed"
                End With
                With .Add("DateTimeInvited", SqlDbType.DateTime)
                    .Expression = "ppl.DateTimeInvited"
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppl.OrganizationID"
                End With
            End With

            .SqlFrom = " FROM LibraryPeopleOrganization ppl INNER JOIN TransSubscription sub ON ppl.PeopleID = sub.SuperAdminID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetSubscriberList() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "OrganizationID"
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "SuperAdminID"
                End With
                With .Add("SubscriptionDate", SqlDbType.DateTime)
                    .Expression = "SubscriptionDate"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "LastUpdated"
                End With
                With .Add("PromoCode", SqlDbType.VarChar)
                    .Expression = "PromoCode"
                End With
                With .Add("AgentName", SqlDbType.VarChar)
                    .Expression = "AgentName"
                End With
                With .Add("SourceOfInfo", SqlDbType.VarChar)
                    .Expression = "SourceOfInfo"
                End With
                With .Add("IsSubscriptionAgreementAgreed", SqlDbType.Bit)
                    .Expression = "IsSubscriptionAgreementAgreed"
                End With
            End With

            .SqlFrom = " FROM TransSubscription"

            .SqlWhere = " WHERE TransferStatus = 'Confirmed' "

            .SqlOrder = " ORDER BY LastUpdated "
            pDT = .Requery
        End With

        Return pDT

    End Function


    Function GetSubscriptionCartList() As DataTable
        ' Common.WebRequest.ProData.ExecuteSQL("Use MobizWebsite")

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cart.ID"
                End With
                With .Add("SubscriptionID", SqlDbType.UniqueIdentifier)
                    .Expression = "cart.SubscriptionID"
                End With
                With .Add("Quantity", SqlDbType.Decimal)
                    .Expression = "cart.Qty"
                End With
                With .Add("UnitPrice", SqlDbType.Money)
                    .Expression = "cart.UnitPrice"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "cart.ProductID"
                End With
                With .Add("SusbcriptionStatus", SqlDbType.VarChar)
                    .Expression = "cart.SubscriptionStatus"
                End With
                With .Add("ActivationDate", SqlDbType.DateTime)
                    .Expression = "cart.ActivationDate"
                End With
            End With

            .SqlFrom = " FROM TransSubscription sub INNER JOIN TransSubscriptionCart cart ON sub.ID = cart.SubscriptionID "

            .SqlWhere = " WHERE sub.TransferStatus = 'Confirmed' "

            pDT = .Requery
        End With

        Return pDT

    End Function
End Class
