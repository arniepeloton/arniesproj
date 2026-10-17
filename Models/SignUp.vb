Public Class SignUp
    Public Property ID As String
    Public Property PeopleID As String
    Public Property MaxReturnRow As Integer = 50

    Public Property TitleID As String
    Public Property Surname As String
    Public Property FirstName As String
    Public Property MiddleName As String
    Public Property PersonalSuffixName As String
    Public Property PostSuffixList As Object()
    Public Property DateOfBirth As Date
    Public Property NickName As String
    Public Property MotherMaidenSurname As String
    Public Property MotherMaidenFirstName As String
    Public Property MotherMaidenMiddleName As String

    Public Property UserName As String
    Public Property UserPassword As String

    Public Property SecurityQuestion1ID As String
    Public Property SecurityQuestion1 As String
    Public Property SecurityQuestion1Answer As String
    Public Property SecurityQuestion2ID As String
    Public Property SecurityQuestion2 As String
    Public Property SecurityQuestion2Answer As String

    Public Property PersonContactCountryIDTel As String
    Public Property PersonContactCountryIDMobile As String
    Public Property PersonContactAreaCodeID As String
    Public Property PersonContactTelCoPrefixID As String
    Public Property PersonContactNoTel As String
    Public Property PersonContactNoMobile As String
    Public Property PersonContactExtensionNo As String
    Public Property PersonContactTypeTel As String
    Public Property PersonContactTypeMobile As String

    Public Property PersonContactEmail As String
    Public Property PersonContactEmailType As String

    Public Property IsTermAgreed As String
    Public Property IsCertifiedCorrect As String
    Public Property IsFirstPartySignUp As String

    Function SavePeopleInfoNameInitial() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String = ""

        Try
            p("TitleID") = IIf(TitleID.Trim().Length = 0, "NULL", ReadyString(TitleID))
            p("Surname") = ReadyString(Surname)
            p("FirstName") = ReadyString(FirstName)
            p("MiddleName") = ReadyString(MiddleName)
            p("PersonalSuffixName") = ReadyString(PersonalSuffixName)
            p("DateOfBirth") = ReadyString(DateOfBirth)
            p("NickName") = ReadyString(NickName)
            p("MotherMaidenSurname") = ReadyString(MotherMaidenSurname)
            p("MotherMaidenFirstName") = ReadyString(MotherMaidenFirstName)
            p("MotherMaidenMiddleName") = ReadyString(MotherMaidenMiddleName)
            p("IsTermAgreed") = IsTermAgreed
            p("IsCertifiedCorrect") = IsCertifiedCorrect
            p("IsFirstPartySignUp") = IsFirstPartySignUp

            If Not CStr2(Common.WebRequest.Data("PIN")) = vbNullString Then
                p("DTRPIN") = ReadyString(Common.WebRequest.Data("PIN"))
            End If


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

            ' '''Save User Account
            p = New InsertUpdate("MobizLibraryPeopleUserAccount")

            p("PeopleID") = ReadyString(strIDReturn)
            p("Username") = ReadyString(UserName)
            p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(UserPassword))
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If (IsFirstPartySignUp = 1) Then
                ' '''Save Security Question1
                p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                p("PeopleID") = ReadyString(strIDReturn)
                p("SecurityQuestion") = ReadyString(SecurityQuestion1)
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion1Answer)) 'ReadyString(SecurityQuestion1Answer)
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If (SecurityQuestion2.Trim().Length > 0) Then
                    ' '''Save Security Question2
                    p = New InsertUpdate("mobizLibraryPeopleSecurityQuestion")

                    p("PeopleID") = ReadyString(strIDReturn)
                    p("SecurityQuestion") = ReadyString(SecurityQuestion2)
                    p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion2Answer)) 'ReadyString(SecurityQuestion2Answer)
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            End If


            ' '''Save Email Address
            p = New InsertUpdate("MobizLibraryPeopleContactEmail")

            p("PeopleID") = ReadyString(strIDReturn)

            If CStr2(PersonContactEmailType) <> "" Then
                p("EmailType") = ReadyString(PersonContactEmailType)
            End If

            If CStr2(PersonContactEmail) <> "" Then
                p("EmailAddress") = ReadyString(PersonContactEmail)

                ' Save only if the user entered an email address
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            ' '''Save Telephone Contact
            p = New InsertUpdate("MobizLibraryPeopleContactTel")

            p("PeopleID") = ReadyString(strIDReturn)

            If CStr2(PersonContactTypeTel) <> "" Then
                p("ContactType") = ReadyString(PersonContactTypeTel)
            End If

            If CStr2(PersonContactCountryIDTel) <> "" Then
                p("CountryID") = ReadyString(PersonContactCountryIDTel)
            End If

            If CStr2(PersonContactAreaCodeID) <> "" Then
                p("AreaCodeID") = ReadyString(PersonContactAreaCodeID)
            End If

            If CStr2(PersonContactNoTel) <> "" Then
                p("ContactNo") = ReadyString(PersonContactNoTel)

                ' Save only if the user entered a telephone number
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If


            ' '''Save Mobile Contact
            p = New InsertUpdate("MobizLibraryPeopleContactMobile")

            p("PeopleID") = ReadyString(strIDReturn)

            If CStr2(PersonContactTypeMobile) <> "" Then
                p("ContactType") = ReadyString(PersonContactTypeMobile)
            End If

            If CStr2(PersonContactCountryIDMobile) <> "" Then
                p("CountryID") = ReadyString(PersonContactCountryIDMobile)
            End If

            If CStr2(PersonContactTelCoPrefixID) <> "" Then
                p("TelCoPrefixID") = ReadyString(PersonContactTelCoPrefixID)
            End If

            If CStr2(PersonContactNoMobile) <> "" Then
                p("ContactNo") = ReadyString(PersonContactNoMobile)

                ' Save only if the user entered a mobile number
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            AppendToLog(GetStackTrace(False, ex))
            Common.WebRequest.ProData.RollbackTransaction()
            Return "Error"
        End Try

        Return strIDReturn

    End Function

    Function SaveIsFirstPartySignUp() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String

        Try
            p("IsFirstPartySignUp") = IsFirstPartySignUp

            strIDReturn = PeopleID
            pSql = p.GetUpdate(" ID = " & ReadyString(PeopleID))


            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    
    Function CheckPeopleUniqueName(ByVal IsFullCheckName As Boolean) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

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

            .SqlFrom = "FROM MobizLibraryPeople ppol "

            If (IsFullCheckName) Then
                .SqlWhere = "WHERE ppol.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                            " AND ppol.Surname = " & ReadyString(Surname) &
                            " AND ppol.FirstName = " & ReadyString(FirstName) &
                            " AND ppol.MiddleName = " & ReadyString(MiddleName) &
                            " AND ppol.DateOfBirth = " & ReadyString(DateOfBirth) &
                            " AND ppol.MotherMaidenSurname = " & ReadyString(MotherMaidenSurname) &
                            " AND ppol.MotherMaidenFirstName = " & ReadyString(MotherMaidenFirstName) &
                            " AND ppol.MotherMaidenMiddleName = " & ReadyString(MotherMaidenMiddleName) &
                            " AND ppol.PersonalSuffixName = " & ReadyString(PersonalSuffixName)
            Else
                .SqlWhere = "WHERE ppol.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
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

            .SqlFrom = "FROM MobizLibraryPeopleUserAccount ppol "

            .SqlWhere = "WHERE ppol.Username = " & ReadyString(UserName)


            pDT = .Requery
        End With

        Return pDT

    End Function


    Function CheckPeopleIsFirstPartySignUp() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName"
                End With
                With .Add("FullName", SqlDbType.VarChar)
                    .Expression = "ppol.FirstName + ' ' + ISNULL(ppol.MiddleName + ' ', '') + ppol.Surname"
                End With
                With .Add("IsFirstPartySignUp", SqlDbType.Bit)
                    .Expression = "ppol.IsFirstPartySignUp"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeople ppol "

            .SqlWhere = "WHERE ppol.ID = " & ReadyString(PeopleID)


            pDT = .Requery
        End With

        Return pDT

    End Function

    'Unused Function
    Function CheckNeedEmailVerification() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("IsEmailConfirmed", SqlDbType.Bit)
                    .Expression = "IsEmailConfirmed"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeopleContactEmail  "

            .SqlWhere = "WHERE PeopleID = " & ReadyString(PeopleID) &
                " AND IsEmailConfirmed = 0"


            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleSecurityQuestionList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "sec.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "sec.PeopleID"
                    .Searchable = False
                End With
                With .Add("SecurityQuestion", SqlDbType.VarChar)
                    .Expression = "sec.SecurityQuestion"
                End With
                With .Add("SecurityAnswer", SqlDbType.VarChar)
                    .Expression = "sec.SecurityAnswer"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleSecurityQuestion sec "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE sec.PeopleID IS NULL", " WHERE sec.PeopleID = " & ReadyString(strID))

            .SqlOrder = " ORDER BY sec.SecurityQuestion DESC"
            pDT = .Requery

        End With

        Return pDT

    End Function


    Function SavePeopleUserAccountRecovery() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleUserAccount")
        Dim pSql As String
        Dim strIDReturn As String

        Try

            p("Username") = ReadyString(UserName)
            p("UserPassword") = ReadyString(UserPassword)

            strIDReturn = PeopleID
            pSql = p.GetUpdate(" PeopleID = " & ReadyString(PeopleID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SavePeopleUserAccountManage() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleUserAccount")
        Dim pSql As String
        Dim strIDReturn As String

        Try

            p("Username") = ReadyString(UserName)
            p("UserPassword") = ReadyString(UserPassword)

            strIDReturn = PeopleID
            pSql = p.GetUpdate(" PeopleID = " & ReadyString(PeopleID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ' '''Save Security Question1
            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

            p("PeopleID") = ReadyString(strIDReturn)
            p("SecurityQuestion") = ReadyString(SecurityQuestion1)
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion1Answer)) 'ReadyString(SecurityQuestion1Answer)
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ' '''Save Security Question2
            p = New InsertUpdate("mobizLibraryPeopleSecurityQuestion")

            p("PeopleID") = ReadyString(strIDReturn)
            p("SecurityQuestion") = ReadyString(SecurityQuestion2)
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion2Answer)) 'ReadyString(SecurityQuestion2Answer)
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function
End Class
