
Public Class People

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
    Public Property DTRPIN As String

    Public Property Gender As String
    Public Property CitizenshipID As String
    Public Property PlaceOfBirthCityID As String
    Public Property PlaceOfBirthCountryID As String
    Public Property Height As Double
    Public Property Weight As Double
    Public Property CivilStatus As String

    Public Property PersonContactCountryID As String
    Public Property PersonContactAreaCodeID As String
    Public Property PersonContactTelCoPrefixID As String
    Public Property PersonContactNo As String
    Public Property PersonContactExtensionNo As String
    Public Property PersonContactType As String

    Public Property PersonContactEmail As String
    Public Property PersonContactEmailType As String

    Public Property PersonEmpHistCompany As String
    Public Property PersonEmpHistJobOccList As Object()

    Public Property PersonEducAcademicLevel As String
    Public Property PersonEducCourseID As String
    Public Property PersonEducInstitutionName As String
    Public Property PersonEducAddressHouseBuildingNo As String
    Public Property PersonEducAddressStreet As String
    Public Property PersonEducAddressCityID As String
    Public Property PersonEducAddressCountryID As String
    Public Property PersonEducYearStarted As Integer
    Public Property PersonEducYearGraduated As Integer
    Public Property PersonEducIsGraduated As String

    Public Property AliasTitleID As String
    Public Property AliasSurname As String
    Public Property AliasFirstName As String
    Public Property AliasMiddleName As String
    Public Property AliasPersonalSuffixName As String
    Public Property AliasPostSuffixList As Object()

    Public Property AddressHouseBuildingNo As String
    Public Property AddressStreet As String
    Public Property AddressCityID As String
    Public Property AddressCountryID As String
    Public Property AddressType As String

    Public Property RelatedTitleID As String
    Public Property RelatedSurname As String
    Public Property RelatedFirstName As String
    Public Property RelatedMiddleName As String
    Public Property RelatedPersonalSuffixName As String
    Public Property RelatedRelation As String
    Public Property RelatedIsEmergencyRef As String
    Public Property RelatedTelCountryID As String
    Public Property RelatedTelAreaCodeID As String
    Public Property RelatedTelContactNo As String
    Public Property RelatedTelExtensionNo As String
    Public Property RelatedTelContactType As String
    Public Property RelatedMobileCountryID As String
    Public Property RelatedMobileTelCoPrefixID As String
    Public Property RelatedMobileContactNo As String
    Public Property RelatedMobileContactType As String
    Public Property RelatedEmail As String
    Public Property RelatedEmailType As String
    Public Property RelatedPostSuffixList As Object()

    Public Property ResidenceCertNo As String
    Public Property ResidenceCertIssuedAtID As String
    Public Property ResidenceCertIssuedOn As Date
    Public Property SSSNo As String
    Public Property PhilHealthNo As String
    Public Property TIN As String
    Public Property HDMFNo As String
    Public Property NBINo As String
    Public Property PassportNo As String
    Public Property PassportIssuedAtID As String
    Public Property PassportIssuedOn As Date
    Public Property PassportExpirationDate As Date

    Public Property UserName As String
    Public Property UserPassword As String

    Public Property SecurityQuestion1ID As String
    Public Property SecurityQuestion1 As String
    Public Property SecurityQuestion1Answer As String
    Public Property SecurityQuestion2ID As String
    Public Property SecurityQuestion2 As String
    Public Property SecurityQuestion2Answer As String

    Public Property TestOrgID As String = "7c794971-1cd2-4ad4-922f-36a93edde09d" ' this is the test org id. do not show in messages employment

    Function SavePeopleInfoNameInitial() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String

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

            '''Save User Account
            p = New InsertUpdate("MobizLibraryPeopleUserAccount")

            p("PeopleID") = ReadyString(strIDReturn)
            p("Username") = ReadyString(UserName)
            p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(UserPassword))
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Security Question1
            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

            p("PeopleID") = ReadyString(strIDReturn)
            p("SecurityQuestion") = ReadyString(SecurityQuestion1)
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion1Answer)) 'ReadyString(SecurityQuestion1Answer) EDITED by Janine. Encrypt answers to security question
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If (SecurityQuestion2.Trim().Length > 0) Then
                '''Save Security Question2
                p = New InsertUpdate("mobizLibraryPeopleSecurityQuestion")

                p("PeopleID") = ReadyString(strIDReturn)
                p("SecurityQuestion") = ReadyString(SecurityQuestion2)
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(SecurityQuestion2Answer)) 'ReadyString(SecurityQuestion2Answer) EDITED by Janine. Encrypt answers to security question
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SavePeopleInfoName() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String
        Dim pFinder As Finder
        Dim pDT As DataTable

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
            p("DTRPIN") = ReadyString(DTRPIN)

            If (WebSession.Instance().PeopleUserID Is Nothing) Then '''Value is nothing when called from winhost
                p("UserID") = "'D4FE8FC8-06B5-4C61-8882-CDD9E1DD2BA4'"
            Else
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            End If

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                '''Check if ID exist then update otherwise insert; used in subscription wherein People has ID already from Winhost
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData
                    .MaxRecords = MaxReturnRow

                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "ID"
                        End With
                    End With

                    .SqlFrom = "FROM MobizLibraryPeople "

                    .SqlWhere = IIf(ID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " + ReadyString(ID))

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
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Post Suffix List
            If (Not PostSuffixList Is Nothing) Then
                For Each objPostSuffix As Object In PostSuffixList

                    If (objPostSuffix("RecState").ToString().Trim().Length > 0) Then

                        If (objPostSuffix("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("MobizLibraryPeoplePostSuffix")

                            p("PeopleID") = ReadyString(strIDReturn)
                            p("PostSuffixID") = IIf(objPostSuffix("PostSuffixID").ToString().Trim().Length = 0, "NULL", ReadyString(objPostSuffix("PostSuffixID").ToString().Trim()))

                            If (objPostSuffix("ID").ToString().Trim().Length <= 0) Then
                                strIDPostSuffixRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDPostSuffixRec)
                                pSql = p.GetInsert()
                            Else
                                strIDPostSuffixRec = objPostSuffix("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDPostSuffixRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.MobizLibraryPeoplePostSuffix " &
                                   " WHERE ID " & IIf(objPostSuffix("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objPostSuffix("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objPostSuffix
            End If

            pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function getPeopleList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeople ppol " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)"

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY (ISNULL(ppol.Surname,'') + ', ' + ISNULL(ppol.FirstName,'') + ' ' + ISNULL(ppol.MiddleName,'')) ASC"
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeoplePostSuffixList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolpostsuffix.ID"
                    .Searchable = False
                End With
                With .Add("PostSuffixID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolpostsuffix.PostSuffixID"
                End With
                With .Add("PostSuffixCode", SqlDbType.VarChar)
                    .Expression = "postsuffix.Code"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeoplePostSuffix ppolpostsuffix " &
                    " INNER JOIN dbo.MobizLibraryPeoplePostNominalSuffix postsuffix ON (ppolpostsuffix.PostSuffixID = postsuffix.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolpostsuffix.PeopleID IS NULL", " WHERE ppolpostsuffix.PeopleID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleInfoName(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With
                With .Add("TitleID", SqlDbType.VarChar)
                    .Expression = "ppol.TitleID"
                End With
                With .Add("Title", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Code"
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
                With .Add("ProfSuffixName", SqlDbType.VarChar)
                    .Expression = "dbo.PeoplePostSuffixName(ppol.ID)"
                End With
                With .Add("DateOfBirthMonth", SqlDbType.VarChar)
                    .Expression = "Month(ppol.DateOfBirth)"
                End With
                With .Add("DateOfBirthDay", SqlDbType.Int)
                    .Expression = "Day(ppol.DateOfBirth)"
                End With
                With .Add("DateOfBirthYear", SqlDbType.Int)
                    .Expression = "Year(ppol.DateOfBirth)"
                End With
                With .Add("NickName", SqlDbType.VarChar)
                    .Expression = "ppol.NickName"
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
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolpresuffix.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeople ppol " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolpresuffix ON (ppol.TitleID = ppolpresuffix.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppol.ID IS NULL", " WHERE ppol.ID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

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
                            " AND ppol.MotherMaidenMiddleName = " & ReadyString(MotherMaidenMiddleName)
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

    Function getPeoplePersonalInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With
                With .Add("Gender", SqlDbType.VarChar)
                    .Expression = "ppol.Gender"
                End With
                With .Add("CitizenshipID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.CitizenshipID"
                End With
                With .Add("Citizenship", SqlDbType.VarChar)
                    .Expression = "ctzenshp.Citizenship"
                End With
                With .Add("PlaceOfBirthCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.PlaceOfBirthCityID"
                End With
                With .Add("PlaceOfBirthCity", SqlDbType.VarChar)
                    .Expression = "pobcty.[Name]"
                End With
                With .Add("PlaceOfBirthCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.PlaceOfBirthCountryID"
                End With
                With .Add("PlaceOfBirthCountry", SqlDbType.VarChar)
                    .Expression = "pobctry.[Name]"
                End With
                With .Add("Height", SqlDbType.Float)
                    .Expression = "ppol.Height"
                End With
                With .Add("Weight", SqlDbType.Float)
                    .Expression = "ppol.Weight"
                End With
                With .Add("CivilStatus", SqlDbType.VarChar)
                    .Expression = "ppol.CivilStatus"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeople ppol " &
                    " LEFT JOIN dbo.MobizLibraryCitizenship ctzenshp ON (ppol.CitizenshipID = ctzenshp.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities pobcty ON (ppol.PlaceOfBirthCityID = pobcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries pobctry ON (ppol.PlaceOfBirthCountryID = pobctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppol.ID IS NULL", " WHERE ppol.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeoplePersonalInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String

        p("Gender") = ReadyString(Gender)
        p("CitizenshipID") = IIf(CitizenshipID.Trim().Length = 0, "NULL", ReadyString(CitizenshipID))
        p("PlaceOfBirthCityID") = IIf(PlaceOfBirthCityID.Trim().Length = 0, "NULL", ReadyString(PlaceOfBirthCityID))
        p("PlaceOfBirthCountryID") = IIf(PlaceOfBirthCountryID.Trim().Length = 0, "NULL", ReadyString(PlaceOfBirthCountryID))
        p("Height") = Height
        p("Weight") = Weight
        p("CivilStatus") = ReadyString(CivilStatus)

        strIDReturn = ID
        pSql = p.GetUpdate(" ID = " & ReadyString(ID))

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function getPeoplePrimaryIDInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With
                With .Add("ResidenceCertNo", SqlDbType.VarChar)
                    .Expression = "ppol.ResidenceCertNo"
                End With
                With .Add("ResidenceCertIssuedAtID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ResidenceCertIssuedAtID"
                End With
                With .Add("ResidenceCertIssuedAt", SqlDbType.VarChar)
                    .Expression = "rescertcty.[Name]"
                End With
                With .Add("ResidenceCertIssuedOnMonth", SqlDbType.Int)
                    .Expression = "Month(ppol.ResidenceCertIssuedOn)"
                End With
                With .Add("ResidenceCertIssuedOnDay", SqlDbType.Int)
                    .Expression = "Day(ppol.ResidenceCertIssuedOn)"
                End With
                With .Add("ResidenceCertIssuedOnYear", SqlDbType.Int)
                    .Expression = "Year(ppol.ResidenceCertIssuedOn)"
                End With
                With .Add("SSSNo", SqlDbType.VarChar)
                    .Expression = "ppol.SSSNo"
                End With
                With .Add("PhilHealthNo", SqlDbType.VarChar)
                    .Expression = "ppol.PhilHealthNo"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "ppol.TIN"
                End With
                With .Add("HDMFNo", SqlDbType.VarChar)
                    .Expression = "ppol.HDMFNo"
                End With
                With .Add("NBINo", SqlDbType.VarChar)
                    .Expression = "ppol.NBINo"
                End With
                With .Add("PassportNo", SqlDbType.VarChar)
                    .Expression = "ppol.PassportNo"
                End With
                With .Add("PassportIssuedAtID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.PassportIssuedAtID"
                End With
                With .Add("PassportIssuedAt", SqlDbType.VarChar)
                    .Expression = "passcty.[Name]"
                End With
                With .Add("PassportIssuedOnMonth", SqlDbType.Int)
                    .Expression = "Month(ppol.PassportIssuedOn)"
                End With
                With .Add("PassportIssuedOnDay", SqlDbType.Int)
                    .Expression = "Day(ppol.PassportIssuedOn)"
                End With
                With .Add("PassportIssuedOnYear", SqlDbType.Int)
                    .Expression = "Year(ppol.PassportIssuedOn)"
                End With
                With .Add("PassportExpirationDateMonth", SqlDbType.Int)
                    .Expression = "Month(ppol.PassportExpirationDate)"
                End With
                With .Add("PassportExpirationDateDay", SqlDbType.Int)
                    .Expression = "Day(ppol.PassportExpirationDate)"
                End With
                With .Add("PassportExpirationDateYear", SqlDbType.Int)
                    .Expression = "Year(ppol.PassportExpirationDate)"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeople ppol " &
                    " LEFT JOIN dbo.MobizLibraryCities rescertcty ON (ppol.ResidenceCertIssuedAtID = rescertcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities passcty ON (ppol.PassportIssuedAtID = passcty.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppol.ID IS NULL", " WHERE ppol.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeoplePrimaryID() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String

        p("ResidenceCertNo") = ReadyString(ResidenceCertNo)
        p("ResidenceCertIssuedAtID") = IIf(ResidenceCertIssuedAtID.Trim().Length = 0, "NULL", ReadyString(ResidenceCertIssuedAtID))
        p("ResidenceCertIssuedOn") = IIf(ResidenceCertNo.Trim().Length = 0, "NULL", ReadyString(ResidenceCertIssuedOn))
        p("SSSNo") = ReadyString(SSSNo)
        p("PhilHealthNo") = ReadyString(PhilHealthNo)
        p("TIN") = ReadyString(TIN)
        p("HDMFNo") = ReadyString(HDMFNo)
        p("NBINo") = ReadyString(NBINo)
        p("PassportNo") = ReadyString(PassportNo)
        p("PassportIssuedAtID") = IIf(PassportIssuedAtID.Trim().Length = 0, "NULL", ReadyString(PassportIssuedAtID))
        p("PassportIssuedOn") = IIf(PassportNo.Trim().Length = 0, "NULL", ReadyString(PassportIssuedOn))
        p("PassportExpirationDate") = IIf(PassportNo.Trim().Length = 0, "NULL", ReadyString(PassportExpirationDate))

        strIDReturn = ID
        pSql = p.GetUpdate(" ID = " & ReadyString(ID))

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function CheckPeopleUniqueAliasName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolalias.ID"
                End With
                With .Add("AliasName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppolalias.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppolalias.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppolalias.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppolalias.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeopleAliasPostSuffixName(ppolalias.ID))),' '), '') "
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleAlias ppolalias " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppolalias.TitleID = ppolprenominal.ID)"

            .SqlWhere = " WHERE ppolalias.PeopleID = " + ReadyString(PeopleID) &
                        " AND ppolalias.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND ppolalias.Surname = " & ReadyString(AliasSurname) &
                        " AND ppolalias.FirstName = " & ReadyString(AliasFirstName) &
                        " AND ppolalias.MiddleName = " & ReadyString(AliasMiddleName)
            
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleAliasList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolalias.ID"
                End With
                With .Add("AliasName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppolalias.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppolalias.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppolalias.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppolalias.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeopleAliasPostSuffixName(ppolalias.ID))),' '), '') "
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleAlias ppolalias " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppolalias.TitleID = ppolprenominal.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE ppolalias.PeopleID IS NULL", " WHERE ppolalias.PeopleID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleAliasPostSuffixList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolaliaspostsuffix.ID"
                    .Searchable = False
                End With
                With .Add("PostSuffixID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolaliaspostsuffix.PostSuffixID"
                End With
                With .Add("PostSuffixCode", SqlDbType.VarChar)
                    .Expression = "postsuffix.Code"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleAliasPostSuffix ppolaliaspostsuffix " &
                    " INNER JOIN dbo.MobizLibraryPeoplePostNominalSuffix postsuffix ON (ppolaliaspostsuffix.PostSuffixID = postsuffix.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolaliaspostsuffix.AliasID IS NULL", " WHERE ppolaliaspostsuffix.AliasID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleAliasInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolalias.ID"
                End With
                With .Add("TitleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolalias.TitleID"
                End With
                With .Add("Title", SqlDbType.VarChar)
                    .Expression = "ppolprenominal.Code"
                End With
                With .Add("Surname", SqlDbType.VarChar)
                    .Expression = "ppolalias.Surname"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "ppolalias.FirstName"
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "ppolalias.MiddleName"
                End With
                With .Add("PersonalSuffixName", SqlDbType.VarChar)
                    .Expression = "ppolalias.PersonalSuffixName"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleAlias ppolalias " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppolalias.TitleID = ppolprenominal.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE ppolalias.ID IS NULL", " WHERE ppolalias.ID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SavePeopleAliasName() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleAlias")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String

        Try

            p("PeopleID") = ReadyString(PeopleID)
            p("TitleID") = IIf(AliasTitleID.Trim().Length = 0, "NULL", ReadyString(AliasTitleID))
            p("Surname") = ReadyString(AliasSurname)
            p("FirstName") = ReadyString(AliasFirstName)
            p("MiddleName") = ReadyString(AliasMiddleName)
            p("PersonalSuffixName") = ReadyString(AliasPersonalSuffixName)

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

            '''Save Post Suffix List
            For Each objPostSuffix As Object In AliasPostSuffixList

                If (objPostSuffix("RecState").ToString().Trim().Length > 0) Then

                    If (objPostSuffix("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryPeopleAliasPostSuffix")

                        p("AliasID") = ReadyString(strIDReturn)
                        p("PostSuffixID") = IIf(objPostSuffix("PostSuffixID").ToString().Trim().Length = 0, "NULL", ReadyString(objPostSuffix("PostSuffixID").ToString().Trim()))

                        If (objPostSuffix("ID").ToString().Trim().Length <= 0) Then
                            strIDPostSuffixRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDPostSuffixRec)
                            pSql = p.GetInsert()
                        Else
                            strIDPostSuffixRec = objPostSuffix("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDPostSuffixRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryPeopleAliasPostSuffix " &
                               " WHERE ID " & IIf(objPostSuffix("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objPostSuffix("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objPostSuffix

            pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function RemovePeopleAliasName() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
               " FROM dbo.MobizLibraryPeopleAliasPostSuffix " &
               " WHERE AliasID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleAlias " &
                   " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getPeopleAddressList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "addrs.ID"
                End With
                With .Add("HouseBuildingNo", SqlDbType.VarChar)
                    .Expression = "addrs.HouseBuildingNo"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "addrs.Street"
                End With
                With .Add("CityID", SqlDbType.UniqueIdentifier)
                    .Expression = "addrs.CityID"
                End With
                With .Add("City", SqlDbType.VarChar)
                    .Expression = "cty.[Name]"
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "addrs.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("AddressType", SqlDbType.VarChar)
                    .Expression = "addrs.AddressType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPeopleAddress addrs " &
                    " INNER JOIN dbo.MobizLibraryCities cty ON (addrs.CityID = cty.ID) " &
                    " INNER JOIN dbo.MobizLibraryCountries ctry ON (addrs.CountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE addrs.PeopleID IS NULL", " WHERE addrs.PeopleID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleAddressInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "addrs.ID"
                End With
                With .Add("HouseBuildingNo", SqlDbType.VarChar)
                    .Expression = "addrs.HouseBuildingNo"
                End With
                With .Add("Street", SqlDbType.VarChar)
                    .Expression = "addrs.Street"
                End With
                With .Add("CityID", SqlDbType.UniqueIdentifier)
                    .Expression = "addrs.CityID"
                End With
                With .Add("City", SqlDbType.VarChar)
                    .Expression = "cty.[Name]"
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "addrs.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("AddressType", SqlDbType.VarChar)
                    .Expression = "addrs.AddressType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPeopleAddress addrs " &
                    " INNER JOIN dbo.MobizLibraryCities cty ON (addrs.CityID = cty.ID) " &
                    " INNER JOIN dbo.MobizLibraryCountries ctry ON (addrs.CountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE addrs.ID IS NULL", " WHERE addrs.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleAddess() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleAddress")
        Dim pSql As String
        Dim strIDReturn As String

        p("PeopleID") = ReadyString(PeopleID)
        p("HouseBuildingNo") = ReadyString(AddressHouseBuildingNo)
        p("Street") = ReadyString(AddressStreet)
        p("CityID") = IIf(AddressCityID.Trim().Length = 0, "NULL", ReadyString(AddressCityID))
        p("CountryID") = IIf(AddressCountryID.Trim().Length = 0, "NULL", ReadyString(AddressCountryID))
        p("AddressType") = ReadyString(AddressType)

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

    Function RemovePeopleAdress() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleAddress " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function CheckPeopleUniqueRelatedName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "rel.ID"
                End With
                With .Add("RelatedName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rel.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rel.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rel.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(rel.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeopleRelatedPostSuffixName(rel.ID))),' '), '') "
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleRelatedPerson rel " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (rel.TitleID = ppolprenominal.ID)"

            .SqlWhere = " WHERE rel.PeopleID = " + ReadyString(PeopleID) &
                        " AND rel.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND rel.Surname = " & ReadyString(RelatedSurname) &
                        " AND rel.FirstName = " & ReadyString(RelatedFirstName) &
                        " AND rel.MiddleName = " & ReadyString(RelatedMiddleName)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleRelatedList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "rel.ID"
                End With
                With .Add("RelatedName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rel.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rel.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rel.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(rel.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeopleRelatedPostSuffixName(rel.ID))),' '), '') "
                End With
                With .Add("Relation", SqlDbType.VarChar)
                    .Expression = "rel.Relation"
                End With
                With .Add("IsEmergencyRef", SqlDbType.Bit)
                    .Expression = "rel.IsEmergencyRef"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPeopleRelatedPerson rel " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (rel.TitleID = ppolprenominal.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE rel.PeopleID IS NULL", " WHERE rel.PeopleID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleRelatedInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "rel.ID"
                End With
                With .Add("TitleID", SqlDbType.UniqueIdentifier)
                    .Expression = "rel.TitleID"
                End With
                With .Add("Title", SqlDbType.VarChar)
                    .Expression = "ppolprenominal.Code"
                End With
                With .Add("Surname", SqlDbType.VarChar)
                    .Expression = "rel.Surname"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "rel.FirstName"
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "rel.MiddleName"
                End With
                With .Add("PersonalSuffixName", SqlDbType.VarChar)
                    .Expression = "rel.PersonalSuffixName"
                End With
                With .Add("Relation", SqlDbType.VarChar)
                    .Expression = "rel.Relation"
                End With
                With .Add("IsEmergencyRef", SqlDbType.Bit)
                    .Expression = "rel.IsEmergencyRef"
                End With

                With .Add("TelCountryID", SqlDbType.VarChar)
                    .Expression = "rel.TelCountryID"
                End With
                With .Add("TelCountryCode", SqlDbType.VarChar)
                    .Expression = "telctry.Code"
                End With
                With .Add("TelCountryCallingCode", SqlDbType.VarChar)
                    .Expression = "telctry.CallingCode"
                End With
                With .Add("TelAreaCodeID", SqlDbType.VarChar)
                    .Expression = "rel.TelAreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("TelContactNo", SqlDbType.VarChar)
                    .Expression = "rel.TelContactNo"
                End With
                With .Add("TelExtensionNo", SqlDbType.VarChar)
                    .Expression = "rel.TelExtensionNo"
                End With
                With .Add("TelContactType", SqlDbType.VarChar)
                    .Expression = "rel.TelContactType"
                End With

                With .Add("MobileCountryID", SqlDbType.VarChar)
                    .Expression = "rel.MobileCountryID"
                End With
                With .Add("MobileCountryCode", SqlDbType.VarChar)
                    .Expression = "mobctry.Code"
                End With
                With .Add("MobileCountryCallingCode", SqlDbType.VarChar)
                    .Expression = "mobctry.CallingCode"
                End With
                With .Add("MobileTelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "rel.MobileTelCoPrefixID"
                End With
                With .Add("MobilePrefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("MobileTelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("MobileContactNo", SqlDbType.VarChar)
                    .Expression = "rel.MobileContactNo"
                End With
                With .Add("MobileContactType", SqlDbType.VarChar)
                    .Expression = "rel.MobileContactType"
                End With

                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "rel.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "rel.EmailType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPeopleRelatedPerson rel " &
                       " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (rel.TitleID = ppolprenominal.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries telctry ON (rel.TelCountryID = telctry.ID) " &
                       " LEFT JOIN dbo.MobizLibraryAreaCodes areacode ON (rel.TelAreaCodeID = areacode.ID) " &
                       " LEFT JOIN dbo.MobizLibraryCountries mobctry ON (rel.MobileCountryID = mobctry.ID) " &
                       " LEFT JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (rel.MobileTelCoPrefixID = telcopref.ID) " &
                       " LEFT JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE rel.ID IS NULL", " WHERE rel.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleRelatedPostSuffixList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolrelpostsuffix.ID"
                    .Searchable = False
                End With
                With .Add("PostSuffixID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolrelpostsuffix.PostSuffixID"
                End With
                With .Add("PostSuffixCode", SqlDbType.VarChar)
                    .Expression = "postsuffix.Code"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleRelatedPersonPostSuffix ppolrelpostsuffix " &
                    " INNER JOIN dbo.MobizLibraryPeoplePostNominalSuffix postsuffix ON (ppolrelpostsuffix.PostSuffixID = postsuffix.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolrelpostsuffix.RelatedID IS NULL", " WHERE ppolrelpostsuffix.RelatedID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleRelated() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleRelatedPerson")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String

        Try

            p("PeopleID") = ReadyString(PeopleID)
            p("TitleID") = IIf(RelatedTitleID.Trim().Length = 0, "NULL", ReadyString(RelatedTitleID))
            p("Surname") = ReadyString(RelatedSurname)
            p("FirstName") = ReadyString(RelatedFirstName)
            p("MiddleName") = ReadyString(RelatedMiddleName)
            p("PersonalSuffixName") = ReadyString(RelatedPersonalSuffixName)
            p("Relation") = ReadyString(RelatedRelation)
            p("IsEmergencyRef") = RelatedIsEmergencyRef
            p("TelCountryID") = IIf(RelatedTelCountryID.Trim().Length = 0, "NULL", ReadyString(RelatedTelCountryID))
            p("TelAreaCodeID") = IIf(RelatedTelAreaCodeID.Trim().Length = 0, "NULL", ReadyString(RelatedTelAreaCodeID))
            p("TelContactNo") = ReadyString(RelatedTelContactNo)
            p("TelExtensionNo") = ReadyString(RelatedTelExtensionNo)
            p("TelContactType") = ReadyString(RelatedTelContactType)
            p("MobileCountryID") = IIf(RelatedMobileCountryID.Trim().Length = 0, "NULL", ReadyString(RelatedMobileCountryID))
            p("MobileTelCoPrefixID") = IIf(RelatedMobileTelCoPrefixID.Trim().Length = 0, "NULL", ReadyString(RelatedMobileTelCoPrefixID))
            p("MobileContactNo") = ReadyString(RelatedMobileContactNo)
            p("MobileContactType") = ReadyString(RelatedMobileContactType)
            p("EmailAddress") = ReadyString(RelatedEmail)
            p("EmailType") = ReadyString(RelatedEmailType)

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

            '''Save Post Suffix List
            For Each objPostSuffix As Object In RelatedPostSuffixList

                If (objPostSuffix("RecState").ToString().Trim().Length > 0) Then

                    If (objPostSuffix("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryPeopleRelatedPersonPostSuffix")

                        p("RelatedID") = ReadyString(strIDReturn)
                        p("PostSuffixID") = IIf(objPostSuffix("PostSuffixID").ToString().Trim().Length = 0, "NULL", ReadyString(objPostSuffix("PostSuffixID").ToString().Trim()))

                        If (objPostSuffix("ID").ToString().Trim().Length <= 0) Then
                            strIDPostSuffixRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDPostSuffixRec)
                            pSql = p.GetInsert()
                        Else
                            strIDPostSuffixRec = objPostSuffix("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDPostSuffixRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.MobizLibraryPeopleRelatedPersonPostSuffix " &
                               " WHERE ID " & IIf(objPostSuffix("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objPostSuffix("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objPostSuffix

            pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function RemovePeopleRelated() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleRelatedPersonPostSuffix " &
                   " WHERE RelatedID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleRelatedPerson " &
                   " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getPeopleContactTelList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.PeopleID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "ppolcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "ppolcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "ppolcon.ExtensionNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactTel ppolcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (ppolcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (ppolcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolcon.PeopleID IS NULL", " WHERE ppolcon.PeopleID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleContactTelInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.PeopleID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "ppolcon.CountryID"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryCallingCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("AreaCodeID", SqlDbType.VarChar)
                    .Expression = "ppolcon.AreaCodeID"
                End With
                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "areacode.Area"
                End With
                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "areacode.CallingCode"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactNo"
                End With
                With .Add("ExtensionNo", SqlDbType.VarChar)
                    .Expression = "ppolcon.ExtensionNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactTel ppolcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (ppolcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryAreaCodes areacode ON (ppolcon.AreaCodeID = areacode.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolcon.ID IS NULL", " WHERE ppolcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleContactTel() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleContactTel")
        Dim pSql As String
        Dim strIDReturn As String

        p("PeopleID") = ReadyString(PeopleID)
        p("CountryID") = ReadyString(PersonContactCountryID)
        p("AreaCodeID") = ReadyString(PersonContactAreaCodeID)
        p("ContactNo") = ReadyString(PersonContactNo)
        p("ExtensionNo") = ReadyString(PersonContactExtensionNo)
        p("ContactType") = ReadyString(PersonContactType)

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemovePeopleContactTel() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleContactTel " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getPeopleContactMobileList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.PeopleID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "ppolcon.CountryID"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("TelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "ppolcon.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactMobile ppolcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (ppolcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (ppolcon.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolcon.PeopleID IS NULL", " WHERE ppolcon.PeopleID = " + ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleContactMobileInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.PeopleID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.VarChar)
                    .Expression = "ppolcon.CountryID"
                End With
                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "ctry.Code"
                End With
                With .Add("CountryCallingCode", SqlDbType.VarChar)
                    .Expression = "ctry.CallingCode"
                End With
                With .Add("TelCoPrefixID", SqlDbType.VarChar)
                    .Expression = "ppolcon.TelCoPrefixID"
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "telcopref.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactNo"
                End With
                With .Add("ContactType", SqlDbType.VarChar)
                    .Expression = "ppolcon.ContactType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactMobile ppolcon " &
                       " INNER JOIN dbo.MobizLibraryCountries ctry ON (ppolcon.CountryID = ctry.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcoPrefixes telcopref ON (ppolcon.TelCoPrefixID = telcopref.ID) " &
                       " INNER JOIN dbo.MobizLibraryTelcos telco ON (telcopref.TelcoId = telco.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolcon.ID IS NULL", " WHERE ppolcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleContactMobile() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleContactMobile")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        p("PeopleID") = ReadyString(PeopleID)
        p("CountryID") = ReadyString(PersonContactCountryID)
        p("TelCoPrefixID") = ReadyString(PersonContactTelCoPrefixID)
        p("ContactNo") = ReadyString(PersonContactNo)
        p("ContactType") = ReadyString(PersonContactType)

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            '''Check if ID exist then update otherwise insert; used in subscription wherein People has ID already from Winhost
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = MaxReturnRow

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "ID"
                    End With
                End With

                .SqlFrom = "FROM MobizLibraryPeopleContactMobile "

                .SqlWhere = IIf(ID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " + ReadyString(ID))

                pDT = .Requery
            End With

            strIDReturn = ID
            If (pDT.Rows.Count > 0) Then
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            Else
                p("ID") = ReadyString(ID)
                pSql = p.GetInsert()
            End If

        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function RemovePeopleContactMobile() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleContactMobile " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getPeopleContactEmailList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.PeopleID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "ppolcon.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "ppolcon.EmailType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactEmail ppolcon"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolcon.PeopleID IS NULL", " WHERE ppolcon.PeopleID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPeopleContactEmailInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolcon.PeopleID"
                    .Searchable = False
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "ppolcon.EmailAddress"
                End With
                With .Add("EmailType", SqlDbType.VarChar)
                    .Expression = "ppolcon.EmailType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleContactEmail ppolcon"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolcon.ID IS NULL", " WHERE ppolcon.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleContactEmail() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleContactEmail")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        p("PeopleID") = ReadyString(PeopleID)
        p("EmailAddress") = ReadyString(PersonContactEmail)
        p("EmailType") = ReadyString(PersonContactEmailType)

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            '''Check if ID exist then update otherwise insert; used in subscription wherein People has ID already from Winhost
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = MaxReturnRow

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "ID"
                    End With
                End With

                .SqlFrom = "FROM MobizLibraryPeopleContactEmail "

                .SqlWhere = IIf(ID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " + ReadyString(ID))

                pDT = .Requery
            End With

            strIDReturn = ID
            If (pDT.Rows.Count > 0) Then
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            Else
                p("ID") = ReadyString(ID)
                pSql = p.GetInsert()
            End If
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
        Common.WebRequest.ProData.ExecuteSQL(pSql)


        Return strIDReturn

    End Function

    Function RemovePeopleContactEmail() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleContactEmail " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getPeopleEmpHistoryList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemphist.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemphist.PeopleID"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ppolemphist.CompanyName"
                End With
                With .Add("DateStart", SqlDbType.Date)
                    .Expression = " (SELECT TOP 1 DateStart " &
                            " FROM dbo.MobizLibraryPeopleEmpHistOccupation ppolemphistjobocc " &
                            " WHERE ppolemphistjobocc.EmpHistID = ppolemphist.ID " &
                            " ORDER BY DateStart ASC)"
                End With
                With .Add("DateEnd", SqlDbType.Date)
                    .Expression = " (SELECT TOP 1 DateEnd " &
                            " FROM dbo.MobizLibraryPeopleEmpHistOccupation ppolemphistjobocc " &
                            " WHERE ppolemphistjobocc.EmpHistID = ppolemphist.ID " &
                            " ORDER BY DateEnd ASC)"
                End With
                With .Add("IsUntilPresent", SqlDbType.Date)
                    .Expression = " ISNULL((SELECT TOP 1 IsUntilPresent " &
                            " FROM dbo.MobizLibraryPeopleEmpHistOccupation ppolemphistjobocc " &
                            " WHERE ppolemphistjobocc.EmpHistID = ppolemphist.ID " &
                            " AND IsUntilPresent = 1),0)"
                End With

            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleEmpHist ppolemphist "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolemphist.PeopleID IS NULL", " WHERE ppolemphist.PeopleID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleEmpHistoryInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemphist.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemphist.PeopleID"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ppolemphist.CompanyName"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleEmpHist ppolemphist "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolemphist.ID IS NULL", " WHERE ppolemphist.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleEmpHistoryJobOccList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemphistjobocc.ID"
                    .Searchable = False
                End With
                With .Add("OccupationID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolemphistjobocc.OccupationID"
                End With
                With .Add("JobOccupation", SqlDbType.VarChar)
                    .Expression = "jobocc.JobOccupation"
                End With
                With .Add("DateStartMonth", SqlDbType.VarChar)
                    .Expression = "Month(ppolemphistjobocc.DateStart)"
                End With
                With .Add("DateStartDay", SqlDbType.VarChar)
                    .Expression = "Day(ppolemphistjobocc.DateStart)"
                End With
                With .Add("DateStartYear", SqlDbType.VarChar)
                    .Expression = "Year(ppolemphistjobocc.DateStart)"
                End With
                With .Add("DateEndMonth", SqlDbType.VarChar)
                    .Expression = "Month(ppolemphistjobocc.DateEnd)"
                End With
                With .Add("DateEndDay", SqlDbType.VarChar)
                    .Expression = "Day(ppolemphistjobocc.DateEnd)"
                End With
                With .Add("DateEndYear", SqlDbType.VarChar)
                    .Expression = "Year(ppolemphistjobocc.DateEnd)"
                End With
                With .Add("IsUntilPresent", SqlDbType.VarChar)
                    .Expression = "ppolemphistjobocc.IsUntilPresent"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleEmpHistOccupation ppolemphistjobocc " &
                    " INNER JOIN dbo.MobizLibraryJobOccupation jobocc ON (ppolemphistjobocc.OccupationID = jobocc.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolemphistjobocc.EmpHistID IS NULL", " WHERE ppolemphistjobocc.EmpHistID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleEmpHistory() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleEmpHist")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDJobOccRec As String

        Try

            p("PeopleID") = ReadyString(PeopleID)
            p("CompanyName") = ReadyString(PersonEmpHistCompany)

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

            '''Save Job Occuaption List
            For Each objJobOcc As Object In PersonEmpHistJobOccList

                If (objJobOcc("RecState").ToString().Trim().Length > 0) Then

                    If (objJobOcc("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("MobizLibraryPeopleEmpHistOccupation")

                        p("EmpHistID") = ReadyString(strIDReturn)
                        p("OccupationID") = IIf(objJobOcc("JobOccupationID").ToString().Trim().Length = 0, "NULL", ReadyString(objJobOcc("JobOccupationID").ToString().Trim()))
                        p("DateStart") = ReadyString(objJobOcc("DateStart").ToString())
                        p("DateEnd") = IIf(Convert.ToBoolean(objJobOcc("IsUntilPresent")), "NULL", ReadyString(objJobOcc("DateEnd").ToString()))
                        p("IsUntilPresent") = objJobOcc("IsUntilPresent").ToString()

                        If (objJobOcc("ID").ToString().Trim().Length <= 0) Then
                            strIDJobOccRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDJobOccRec)
                            pSql = p.GetInsert()
                        Else
                            strIDJobOccRec = objJobOcc("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDJobOccRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.LibraryPeopleEmpHistOccupation " &
                               " WHERE ID " & IIf(objJobOcc("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objJobOcc("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objJobOcc

            pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function RemovePeopleEmpHistory() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            '''Delete Job Occupation
            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleEmpHistOccupation " &
                   " WHERE EmpHistID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleEmpHist " &
                   " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function getPeopleEducationList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.PeopleID"
                    .Searchable = False
                End With
                With .Add("AcademicLevel", SqlDbType.VarChar)
                    .Expression = "ppoledu.AcademicLevel"
                End With
                With .Add("EducationCourseID", SqlDbType.VarChar)
                    .Expression = "ppoledu.EducationCourseID"
                End With
                With .Add("CourseTitle", SqlDbType.VarChar)
                    .Expression = "educ.CourseTitle"
                End With
                With .Add("InstitutionName", SqlDbType.VarChar)
                    .Expression = "ppoledu.InstitutionName"
                End With
                With .Add("HouseBuildingNo", SqlDbType.VarChar)
                    .Expression = "ppoledu.HouseBuildingNo"
                End With
                With .Add("AddressStreet", SqlDbType.VarChar)
                    .Expression = "ppoledu.AddressStreet"
                End With
                With .Add("AddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.AddressCityID"
                End With
                With .Add("AddressCity", SqlDbType.VarChar)
                    .Expression = "cty.[Name]"
                End With
                With .Add("AddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.AddressCountryID"
                End With
                With .Add("AddressCountry", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("YearStarted", SqlDbType.VarChar)
                    .Expression = "ppoledu.YearStarted"
                End With
                With .Add("YearGraduated", SqlDbType.VarChar)
                    .Expression = "ppoledu.YearGraduated"
                End With
                With .Add("IsGraduated", SqlDbType.VarChar)
                    .Expression = "ppoledu.IsGraduated"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleEducation ppoledu " &
                    " INNER JOIN dbo.MobizLibraryEducationCourse educ ON (ppoledu.EducationCourseID = educ.ID) " &
                    " INNER JOIN dbo.MobizLibraryCities cty ON (ppoledu.AddressCityID = cty.ID) " &
                    " INNER JOIN dbo.MobizLibraryCountries ctry ON (ppoledu.AddressCountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppoledu.PeopleID IS NULL", " WHERE ppoledu.PeopleID = " + ReadyString(strID))

            .SqlOrder = " ORDER BY ppoledu.YearGraduated DESC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleEducationInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.PeopleID"
                    .Searchable = False
                End With
                With .Add("AcademicLevel", SqlDbType.VarChar)
                    .Expression = "ppoledu.AcademicLevel"
                End With
                With .Add("EducationCourseID", SqlDbType.VarChar)
                    .Expression = "ppoledu.EducationCourseID"
                End With
                With .Add("CourseTitle", SqlDbType.VarChar)
                    .Expression = "educ.CourseTitle"
                End With
                With .Add("InstitutionName", SqlDbType.VarChar)
                    .Expression = "ppoledu.InstitutionName"
                End With
                With .Add("HouseBuildingNo", SqlDbType.VarChar)
                    .Expression = "ppoledu.HouseBuildingNo"
                End With
                With .Add("AddressStreet", SqlDbType.VarChar)
                    .Expression = "ppoledu.AddressStreet"
                End With
                With .Add("AddressCityID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.AddressCityID"
                End With
                With .Add("AddressCity", SqlDbType.VarChar)
                    .Expression = "cty.[Name]"
                End With
                With .Add("AddressCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppoledu.AddressCountryID"
                End With
                With .Add("AddressCountry", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("YearStarted", SqlDbType.VarChar)
                    .Expression = "ppoledu.YearStarted"
                End With
                With .Add("YearGraduated", SqlDbType.VarChar)
                    .Expression = "ppoledu.YearGraduated"
                End With
                With .Add("IsGraduated", SqlDbType.VarChar)
                    .Expression = "ppoledu.IsGraduated"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleEducation ppoledu " &
                    " INNER JOIN dbo.MobizLibraryEducationCourse educ ON (ppoledu.EducationCourseID = educ.ID) " &
                    " INNER JOIN dbo.MobizLibraryCities cty ON (ppoledu.AddressCityID = cty.ID) " &
                    " INNER JOIN dbo.MobizLibraryCountries ctry ON (ppoledu.AddressCountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppoledu.ID IS NULL", " WHERE ppoledu.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleEducation() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleEducation")
        Dim pSql As String
        Dim strIDReturn As String

        p("PeopleID") = ReadyString(PeopleID)
        p("AcademicLevel") = ReadyString(PersonEducAcademicLevel)
        p("EducationCourseID") = IIf(PersonEducCourseID.Trim().Length = 0, "NULL", ReadyString(PersonEducCourseID))
        p("InstitutionName") = ReadyString(PersonEducInstitutionName)
        p("HouseBuildingNo") = ReadyString(PersonEducAddressHouseBuildingNo)
        p("AddressStreet") = ReadyString(PersonEducAddressStreet)
        p("AddressCityID") = IIf(PersonEducAddressCityID.Trim().Length = 0, "NULL", ReadyString(PersonEducAddressCityID))
        p("AddressCountryID") = IIf(PersonEducAddressCountryID.Trim().Length = 0, "NULL", ReadyString(PersonEducAddressCountryID))
        p("YearStarted") = PersonEducYearStarted
        p("YearGraduated") = PersonEducYearGraduated
        p("IsGraduated") = PersonEducIsGraduated

        If (ID.Trim().Length <= 0) Then
            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()
        Else
            strIDReturn = ID
            pSql = p.GetUpdate(" ID = " & ReadyString(ID))
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
        Common.WebRequest.ProData.ExecuteSQL(pSql)


        Return strIDReturn

    End Function

    Function RemovePeopleEduction() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.MobizLibraryPeopleEducation " &
                   " WHERE ID = " & ReadyString(ID)

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function CheckPeopleUserAccount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolusr.PeopleID"
                    .Searchable = False
                End With
                With .Add("Username", SqlDbType.VarChar)
                    .Expression = "ppolusr.Username"
                End With
                With .Add("UserPassword", SqlDbType.VarChar)
                    .Expression = "ppolusr.UserPassword"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
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
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleUserAccount ppolusr " &
                " INNER JOIN dbo.MobizLibraryPeople ppol ON (ppolusr.PeopleID = ppol.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)"

            .SqlWhere = " WHERE ppolusr.Username = " & ReadyString(UserName)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleUserAccount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolusr.PeopleID"
                    .Searchable = False
                End With
                With .Add("Username", SqlDbType.VarChar)
                    .Expression = "ppolusr.Username"
                End With
                With .Add("UserPassword", SqlDbType.VarChar)
                    .Expression = "ppolusr.UserPassword"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleUserAccount ppolusr "

            .SqlWhere = " WHERE ppolusr.PeopleID = '" & ReadyString(PeopleID) & "' "

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePeopleUserAccount() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleUserAccount")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strSecIDReturn As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        Try

            p("Username") = ReadyString(UserName)
            p("UserPassword") = ReadyString(UserPassword)

            '''Check if ID exist then update otherwise insert; used in subscription wherein People has ID already from Winhost
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = MaxReturnRow

                With .Fields
                    With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                        .Expression = "PeopleID"
                    End With
                End With

                .SqlFrom = "FROM MobizLibraryPeopleUserAccount "

                .SqlWhere = IIf(PeopleID.Trim().Length = 0, "WHERE PeopleID IS NULL", " WHERE PeopleID = " + ReadyString(PeopleID))

                pDT = .Requery
            End With

            strIDReturn = PeopleID
            If (pDT.Rows.Count > 0) Then
                pSql = p.GetUpdate(" PeopleID = " & ReadyString(PeopleID))
            Else
                p("PeopleID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Security Question1
            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")
            p("PeopleID") = ReadyString(strIDReturn)
            p("SecurityQuestion") = ReadyString(SecurityQuestion1)

            If CStr2(SecurityQuestion1Answer) <> "" Then
                p("SecurityAnswer") = ReadyString(SecurityQuestion1Answer) 'ReadyString(SecurityQuestion1Answer)
            End If

            If (SecurityQuestion1ID.Trim().Length <= 0) Then
                strSecIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strSecIDReturn)
                pSql = p.GetInsert()
            Else
                '''Check if ID exist then update otherwise insert; used in subscription wherein People has ID already from Winhost
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData
                    .MaxRecords = MaxReturnRow

                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "ID"
                        End With
                    End With

                    .SqlFrom = "FROM MobizLibraryPeopleSecurityQuestion "

                    .SqlWhere = IIf(SecurityQuestion1ID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " + ReadyString(SecurityQuestion1ID))

                    pDT = .Requery
                End With

                strSecIDReturn = SecurityQuestion1ID
                If (pDT.Rows.Count > 0) Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(strSecIDReturn))
                Else
                    p("ID") = ReadyString(strSecIDReturn)
                    pSql = p.GetInsert()
                End If

            End If
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If (SecurityQuestion2.Trim().Length > 0) Then
                '''Save Security Question2
                p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")
                p("PeopleID") = ReadyString(strIDReturn)
                p("SecurityQuestion") = ReadyString(SecurityQuestion2)

                If CStr2(SecurityQuestion2Answer) <> "" Then
                    p("SecurityAnswer") = ReadyString(SecurityQuestion2Answer) 'ReadyString(SecurityQuestion2Answer)
                End If

                If (SecurityQuestion2ID.Trim().Length <= 0) Then
                    strSecIDReturn = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strSecIDReturn)
                    pSql = p.GetInsert()
                Else
                    '''Check if ID exist then update otherwise insert; used in subscription wherein People has ID already from Winhost
                    pFinder = New Finder
                    With pFinder
                        .ProData = Common.WebRequest.ProData
                        .MaxRecords = MaxReturnRow

                        With .Fields
                            With .Add("ID", SqlDbType.UniqueIdentifier)
                                .Expression = "ID"
                            End With
                        End With

                        .SqlFrom = "FROM MobizLibraryPeopleSecurityQuestion "

                        .SqlWhere = IIf(SecurityQuestion2ID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " + ReadyString(SecurityQuestion2ID))

                        pDT = .Requery
                    End With

                    strSecIDReturn = SecurityQuestion2ID
                    If (pDT.Rows.Count > 0) Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(strSecIDReturn))
                    Else
                        p("ID") = ReadyString(strSecIDReturn)
                        pSql = p.GetInsert()
                    End If

                End If
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                If (SecurityQuestion2ID.Trim().Length > 0) Then
                    pSql = "DELETE " &
                           " FROM dbo.MobizLibraryPeopleSecurityQuestion " &
                           " WHERE ID = " & ReadyString(SecurityQuestion2ID)

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            End If

            pSql = "Update LibraryPeople Set LastUpdated = GetDate() Where ID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '' ''pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPeople'"
            '' ''Common.WebRequest.ProData.ExecuteSQL(pSql) ' not possible since the current db is Mobiz and not the private database

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SavePeopleDTRPIN() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strSecIDReturn As String
        Dim pFinder As Finder
        Dim pDT As DataTable

        Try

            p("DTRPIN") = ReadyString(Common.WebRequest.Data("PIN"))
            strIDReturn = PeopleID

            pSql = p.GetUpdate(" ID = " & ReadyString(PeopleID))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Dim clsPeople As New People
            Dim dtOrganization As DataTable

            dtOrganization = clsPeople.getPeopleOrganizationList(strIDReturn)

            If dtOrganization.Rows.Count > 0 Then
                Dim pOrganizationID As String

                pOrganizationID = ReadyString(CStr2(dtOrganization.Rows(0).Item("OrganizationID")))

                For i As Integer = 1 To dtOrganization.Rows.Count - 1
                    pOrganizationID &= "," & ReadyString(CStr2(dtOrganization.Rows(i).Item("OrganizationID")))
                Next

                pSql = "select * from Librarysubscription where OrganizationID IN(" & pOrganizationID & ")"

                dtOrganization = Common.WebRequest.ProData.Execute(pSql)

                For i As Integer = 0 To dtOrganization.Rows.Count - 1
                    Common.WebRequest.ProData.Execute("USE " & CStr(dtOrganization.Rows(i).Item("Database")))

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPeople'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next
            End If
        Catch ex As Exception
            Return ""
        End Try

        Return strIDReturn

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

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE sec.PeopleID IS NULL", " WHERE sec.PeopleID = " + ReadyString(strID))

            .SqlOrder = " ORDER BY sec.SecurityQuestion DESC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleSecurityQuestionInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

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

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE sec.ID IS NULL", " WHERE sec.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function CheckPeopleSecurityQuestion(ByVal strPeopleID As String, ByVal strID As String, ByVal strAnswer As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

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

            '.SqlWhere = IIf(strID.Trim().Length = 0, "WHERE sec.ID IS NULL", " WHERE sec.ID = " + ReadyString(strID)) &
            '    IIf(strPeopleID.Trim().Length = 0, " AND sec.PeopleID IS NULL", " AND sec.PeopleID = " + ReadyString(strPeopleID)) &
            '    " AND sec.SecurityAnswer = " & ReadyString(strAnswer)
            'EDITED by Janine
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE sec.ID IS NULL", " WHERE sec.ID = " + ReadyString(strID)) &
                IIf(strPeopleID.Trim().Length = 0, " AND sec.PeopleID IS NULL", " AND sec.PeopleID = " + ReadyString(strPeopleID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleOrganizationList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.PeopleID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.OrganizationID"
                End With
                With .Add("CompanyName", SqlDbType.UniqueIdentifier)
                    .Expression = "org.CompanyName"
                End With
                With .Add("IsFranchisee", SqlDbType.UniqueIdentifier)
                    .Expression = "CASE WHEN franchisee.ID IS NULL THEN 0 ELSE 1 END"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPeopleOrganization ppolorg " &
                " JOIN MobizLibraryOrganization org ON (org.ID = ppolorg.OrganizationID) " &
                " INNER JOIN dbo.MobizLibraryPeople emp ON (ppolorg.PeopleID = emp.ID) " & vbCrLf &
                " OUTER APPLY( " & vbCrLf &
                "   SELECT ID FROM MobizLibraryOrganizationFOWSTFranchiseeRequests " & vbCrLf &
                "   WHERE CompanyID = org.ID " & vbCrLf &
                " ) as franchisee " & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE emp.ID IS NULL", " WHERE emp.ID = " & ReadyString(strID)) &
                    " AND ppolorg.IsConfirmed = 1 " &
                    " AND ppolorg.IsTerminated = 0 "

            .SqlOrder = " ORDER BY org.CompanyName ASC "

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPeopleMessageList(ByVal strID As String) As DataTable
        '''Dim pFinder As Finder
        Dim strSQL As String
        Dim pDT As DataTable

        '''MessageType
        '''1 - Organization Invitation for Employment
        '''2 - For other type of message for future use 

        '' ''pFinder = New Finder
        '' ''With pFinder
        '' ''    .ProData = Common.WebRequest.ProData

        '' ''    .Distinct = True

        '' ''    With .Fields
        '' ''        With .Add("ID", SqlDbType.UniqueIdentifier)
        '' ''            .Expression = "ppolorg.ID"
        '' ''            .Searchable = False
        '' ''        End With
        '' ''        With .Add("MessageType", SqlDbType.Int)
        '' ''            .Expression = "1"
        '' ''            .Searchable = False
        '' ''        End With
        '' ''        With .Add("SenderName", SqlDbType.VarChar)
        '' ''            .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(senderprenominal.Code)) + ' ', ' '), '') + " &
        '' ''                          "ISNULL(NULLIF(LTRIM(RTRIM(sender.Surname)) + ', ',', '), '') + " &
        '' ''                          "ISNULL(NULLIF(LTRIM(RTRIM(sender.FirstName)) + ' ',' '), '') + " &
        '' ''                          "ISNULL(NULLIF(LTRIM(RTRIM(sender.MiddleName)) + ' ',' '), '') + " &
        '' ''                          "ISNULL(NULLIF(', ' + LTRIM(RTRIM(sender.PersonalSuffixName)),', '), '') + " &
        '' ''                          "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppolorg.PeopleID))),' '), '')"
        '' ''        End With
        '' ''        With .Add("Subject", SqlDbType.VarChar)
        '' ''            .Expression = "'Please confirm your employment with ' + org.CompanyName"
        '' ''        End With
        '' ''        With .Add("MessageDateTime", SqlDbType.UniqueIdentifier)
        '' ''            .Expression = "ppolorg.DateTimeInvited"
        '' ''        End With
        '' ''    End With

        '' ''    .SqlFrom = " FROM MobizLibraryPeopleOrganization ppolorg " &
        '' ''        " INNER JOIN MobizLibraryOrganization org ON (org.ID = ppolorg.OrganizationID) " &
        '' ''        " INNER JOIN dbo.MobizLibraryPeople sender ON (ppolorg.UserID = sender.ID) " &
        '' ''        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix senderprenominal ON (sender.TitleID = senderprenominal.ID)"

        '' ''    .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolorg.PeopleID IS NULL", " WHERE ppolorg.PeopleID = " + ReadyString(strID)) &
        '' ''            " AND ppolorg.IsConfirmed = 0 " &
        '' ''            " AND ppolorg.IsTerminated = 0 "

        '' ''    pDT = .Requery

        '' ''End With

        strSQL = "SELECT ppolorg.ID " &
            "   ,1 as MessageType" &
            "   ,ISNULL(NULLIF(LTRIM(RTRIM(senderprenominal.Code)) + ' ', ' '), '') + " &
            "   ISNULL(NULLIF(LTRIM(RTRIM(sender.Surname)) + ', ',', '), '') + " &
            "   ISNULL(NULLIF(LTRIM(RTRIM(sender.FirstName)) + ' ',' '), '') + " &
            "   ISNULL(NULLIF(LTRIM(RTRIM(sender.MiddleName)) + ' ',' '), '') + " &
            "   ISNULL(NULLIF(', ' + LTRIM(RTRIM(sender.PersonalSuffixName)),', '), '') + " &
            "   ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppolorg.PeopleID))),' '), '') as SenderName" &
            "   ,'Please confirm your employment with ' + org.CompanyName as Subject " &
            "   ,'' as Message" &
            "   ,'' as Category" &
            "   ,'' as Product" &
            "   ,ppolorg.DateTimeInvited as MessageDateTime " &
            "   ,org.CompanyName" & vbCrLf &
            " FROM MobizLibraryPeopleOrganization ppolorg " &
            " INNER JOIN MobizLibraryOrganization org ON (org.ID = ppolorg.OrganizationID) " &
            " INNER JOIN dbo.MobizLibraryPeople sender ON (ppolorg.UserID = sender.ID) " &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix senderprenominal ON (sender.TitleID = senderprenominal.ID) " &
            IIf(strID.Trim().Length = 0, " WHERE ppolorg.PeopleID IS NULL", " WHERE ppolorg.PeopleID = " + ReadyString(strID)) &
            " AND ppolorg.IsConfirmed = 0 " &
            " AND ppolorg.IsTerminated = 0 " &
            " AND ppolorg.OrganizationID <> " & ReadyString(TestOrgID)

        strSQL = strSQL & " UNION "

        strSQL = strSQL & " SELECT updlog.ID " &
            "   ,2 as MessageType" &
            "   ,ISNULL(NULLIF(LTRIM(RTRIM(senderprenominal.Code)) + ' ', ' '), '') + " &
            "   ISNULL(NULLIF(LTRIM(RTRIM(sender.Surname)) + ', ',', '), '') + " &
            "   ISNULL(NULLIF(LTRIM(RTRIM(sender.FirstName)) + ' ',' '), '') + " &
            "   ISNULL(NULLIF(LTRIM(RTRIM(sender.MiddleName)) + ' ',' '), '') + " &
            "   ISNULL(NULLIF(', ' + LTRIM(RTRIM(sender.PersonalSuffixName)),', '), '') + " &
            "   ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(updlog.UserID))),' '), '') as SenderName" &
            "   ,updlog.Title as Subject " &
            "   ,updlog.Message" &
            "   ,updlog.Category" &
            "   ,updlog.Product" &
            "   ,updlog.LastUpdated as MessageDateTime, '' as CompanyName " &
            " FROM MobizLibraryUpdateLog updlog " &
            " INNER JOIN dbo.MobizLibraryPeople sender ON (updlog.UserID = sender.ID) " &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix senderprenominal ON (sender.TitleID = senderprenominal.ID) "

        strSQL = strSQL & " ORDER BY MessageDateTime DESC"

        pDT = Common.WebRequest.ProData.Execute(strSQL)

        Return pDT

    End Function

    Function getPeopleMessageT1Details(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.ID"
                    .Searchable = False
                End With
                With .Add("SenderName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(senderprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(sender.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(sender.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(sender.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(sender.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppolorg.PeopleID))),' '), '')"
                End With
                With .Add("Subject", SqlDbType.VarChar)
                    .Expression = "'Please confirm your employment with ' + org.CompanyName"
                End With
                With .Add("MessageDateTime", SqlDbType.DateTime)
                    .Expression = "ppolorg.DateTimeInvited"
                End With
                With .Add("Message", SqlDbType.VarChar)
                    .Expression = "'You have received an invitation to confirm your employment with ' + org.CompanyName + '.'"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPeopleOrganization ppolorg " &
                " INNER JOIN MobizLibraryOrganization org ON (org.ID = ppolorg.OrganizationID) " &
                " INNER JOIN dbo.MobizLibraryPeople sender ON (ppolorg.UserID = sender.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix senderprenominal ON (sender.TitleID = senderprenominal.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ppolorg.ID IS NULL", " WHERE ppolorg.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function ConfirmPeopleMessageT1Details() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleOrganization")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDJobOccRec As String

        Try

            p("IsConfirmed") = 1
            p("DateTimeConfirmed") = "getdate()"

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

    Function getPeopleMessageT2Details(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "updlog.ID"
                    .Searchable = False
                End With
                With .Add("SenderName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(senderprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(sender.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(sender.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(sender.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(sender.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(updlog.UserID))),' '), '')"
                End With
                With .Add("Subject", SqlDbType.VarChar)
                    .Expression = "updlog.Title"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "updlog.Category"
                End With
                With .Add("Product", SqlDbType.VarChar)
                    .Expression = "updlog.Product"
                End With
                With .Add("MessageDateTime", SqlDbType.DateTime)
                    .Expression = "updlog.LastUpdated"
                End With
                With .Add("Message", SqlDbType.VarChar)
                    .Expression = "updlog.Message"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryUpdateLog updlog  " &
                " INNER JOIN dbo.MobizLibraryPeople sender ON (updlog.UserID = sender.ID) " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix senderprenominal ON (sender.TitleID = senderprenominal.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE updlog.ID IS NULL", " WHERE updlog.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function



End Class
