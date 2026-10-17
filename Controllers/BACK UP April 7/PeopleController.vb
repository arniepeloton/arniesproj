Namespace WebProj
    Public Class PeopleController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            Transfer("/Signin")

            Return Content("Please go to /Signin")

        End Function

        Function PeopleMenu() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function

        Function PeopleInfoName() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleInfoAlias() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleInfoAddress() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeoplePersonalInfo() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleContactNo() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleEducation() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleEmpHistory() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleRelatedPerson() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeoplePrimeIDReference() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleUserAccount() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleInfoSummary() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PeopleDTRPIN() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.PeopleID = Common.WebRequest.Data("ID").ToString()
            WebSession.Instance.PeopleUserID = Common.WebRequest.Data("ID").ToString() 'Used in HandleAccess

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PeopleID", WebSession.Instance.PeopleID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function setSignOut() As String
            Dim strReturn As String

            WebSession.Instance.IsNew = 0
            WebSession.Instance.PeopleID = ""
            WebSession.Instance.PeopleUserID = ""

            strReturn = JsonDictionary.Serialize("SignOut", "TRUE")

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUniqueName() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim blnFullCheckName As Boolean

            clsPeople.ID = Common.WebRequest.Data("PeopleID")
            clsPeople.Surname = Common.WebRequest.Data("Surname")
            clsPeople.FirstName = Common.WebRequest.Data("FirstName")
            clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
            clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
            clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
            clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
            clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")
            blnFullCheckName = Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("FullCheckName")))

            strReturn = JsonDictionary.Serialize("PeopleNameData", clsPeople.CheckPeopleUniqueName(blnFullCheckName))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleInfoNameInitial() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If (Not IsDate(Common.WebRequest.Data("DateOfBirth"))) Then
                strReturn = JsonDictionary.Serialize("PeopleNameData", "DateOfBirthError")
            Else

                clsPeople.ID = ""
                clsPeople.TitleID = ""
                clsPeople.Surname = Common.WebRequest.Data("Surname")
                clsPeople.FirstName = Common.WebRequest.Data("FirstName")
                clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
                clsPeople.PersonalSuffixName = ""
                clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
                clsPeople.NickName = ""
                clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
                clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
                clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")

                clsPeople.UserName = Common.WebRequest.Data("Username")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")

                clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
                clsPeople.SecurityQuestion1Answer = Common.WebRequest.Data("Question1Answer")
                clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
                clsPeople.SecurityQuestion2Answer = Common.WebRequest.Data("Question1Answer2")

                WebSession.Instance.PeopleID = clsPeople.SavePeopleInfoNameInitial()
                WebSession.Instance.PeopleUserID = WebSession.Instance.PeopleID

                strReturn = JsonDictionary.Serialize("PeopleNameData", WebSession.Instance.PeopleID)

            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleInfoName() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            If (Not IsDate(Common.WebRequest.Data("DateOfBirth"))) Then
                strReturn = JsonDictionary.Serialize("PeopleNameData", "DateOfBirthError")
            Else

                clsPeople.ID = Common.WebRequest.Data("PeopleID")
                clsPeople.TitleID = Common.WebRequest.Data("TitleID")
                clsPeople.Surname = Common.WebRequest.Data("Surname")
                clsPeople.FirstName = Common.WebRequest.Data("FirstName")
                clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
                clsPeople.PersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")
                clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
                clsPeople.NickName = Common.WebRequest.Data("NickName")
                clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
                clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
                clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")

                clsPeople.PostSuffixList = Common.WebRequest.AllData("PostSuffixList")

                WebSession.Instance.PeopleID = clsPeople.SavePeopleInfoName()
                strReturn = JsonDictionary.Serialize("PeopleNameData", WebSession.Instance.PeopleID)

            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleInfoName() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("PeopleID")
            WebSession.Instance.PeopleID = strID
            strReturn = JsonDictionary.Serialize("PeopleNameData", clsPeople.getPeopleInfoName(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeoplePostSuffixList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                If (WebSession.Instance.PeopleID = Nothing) Then
                    strID = Common.WebRequest.Data("PeopleID")
                Else
                    strID = IIf(WebSession.Instance.PeopleID.Trim().Length > 0, WebSession.Instance.PeopleID, Common.WebRequest.Data("PeopleID"))
                End If
            Catch ex As Exception
            End Try

            strReturn = JsonDictionary.Serialize("PeopleNameData", clsPeople.getPeoplePostSuffixList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeoplePrimaryID() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            If (Not IsDate(Common.WebRequest.Data("ResidenceCertIssuedOn")) And Common.WebRequest.Data("ResidenceCertIssuedOn").Trim().Length > 0) Then
                strReturn = JsonDictionary.Serialize("PeoplePrimeIDData", "ResidenceCertIssuedOnError")
            ElseIf (Not IsDate(Common.WebRequest.Data("PassportIssuedOn")) And Common.WebRequest.Data("PassportIssuedOn").Trim().Length > 0) Then
                strReturn = JsonDictionary.Serialize("PeoplePrimeIDData", "PassportIssuedOnError")
            ElseIf (Not IsDate(Common.WebRequest.Data("PassportExpirationDate")) And Common.WebRequest.Data("PassportExpirationDate").Trim().Length > 0) Then
                strReturn = JsonDictionary.Serialize("PeoplePrimeIDData", "PassportExpirationDateError")
            Else
                clsPeople.ID = WebSession.Instance.PeopleID
                clsPeople.ResidenceCertNo = Common.WebRequest.Data("ResidenceCertNo")
                clsPeople.ResidenceCertIssuedAtID = Common.WebRequest.Data("ResidenceCertIssuedAtID")
                clsPeople.ResidenceCertIssuedOn = IIf(Common.WebRequest.Data("ResidenceCertIssuedOn").Trim().Length = 0, Date.Today(), Common.WebRequest.Data("ResidenceCertIssuedOn"))
                clsPeople.SSSNo = Common.WebRequest.Data("SSSNo")
                clsPeople.PhilHealthNo = Common.WebRequest.Data("PhilHealthNo")
                clsPeople.TIN = Common.WebRequest.Data("TIN")
                clsPeople.HDMFNo = Common.WebRequest.Data("HDMFNo")
                clsPeople.NBINo = Common.WebRequest.Data("NBINo")
                clsPeople.PassportNo = Common.WebRequest.Data("PassportNo")
                clsPeople.PassportIssuedAtID = Common.WebRequest.Data("PassportIssuedAtID")
                clsPeople.PassportIssuedOn = IIf(Common.WebRequest.Data("PassportIssuedOn").Trim().Length = 0, Date.Today(), Common.WebRequest.Data("PassportIssuedOn"))
                clsPeople.PassportExpirationDate = IIf(Common.WebRequest.Data("PassportExpirationDate").Trim().Length = 0, Date.Today(), Common.WebRequest.Data("PassportExpirationDate"))

                WebSession.Instance.PeopleID = clsPeople.SavePeoplePrimaryID()
                strReturn = JsonDictionary.Serialize("PeoplePrimeIDData", WebSession.Instance.PeopleID)
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeoplePrimaryID() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleID
            strReturn = JsonDictionary.Serialize("PeoplePrimeIDData", clsPeople.getPeoplePrimaryIDInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeoplePersonalInfo() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            If (Not IsNumeric(Common.WebRequest.Data("Height"))) Then
                strReturn = JsonDictionary.Serialize("PeoplePersonalInfo", "HeightError")
            ElseIf (Not IsNumeric(Common.WebRequest.Data("Weight"))) Then
                strReturn = JsonDictionary.Serialize("PeoplePersonalInfo", "WeightError")
            Else

                clsPeople.ID = WebSession.Instance.PeopleID
                clsPeople.Gender = Common.WebRequest.Data("Gender")
                clsPeople.CitizenshipID = Common.WebRequest.Data("CitizenshipID")
                clsPeople.PlaceOfBirthCityID = Common.WebRequest.Data("PlaceOfBirthCityID")
                clsPeople.PlaceOfBirthCountryID = Common.WebRequest.Data("PlaceOfBirthCountryID")
                clsPeople.Height = Common.WebRequest.Data("Height")
                clsPeople.Weight = Common.WebRequest.Data("Weight")
                clsPeople.CivilStatus = Common.WebRequest.Data("CivilStatus")

                WebSession.Instance.PeopleID = clsPeople.SavePeoplePersonalInfo()
                strReturn = JsonDictionary.Serialize("PeoplePersonalInfo", WebSession.Instance.PeopleID)

            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeoplePersonalInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleID
            strReturn = JsonDictionary.Serialize("PeoplePersonalInfo", clsPeople.getPeoplePersonalInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleList() As String
            Dim strKeywords As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strKeywords = Common.WebRequest.Data("Keywords")
            strReturn = JsonDictionary.Serialize("PeopleList", clsPeople.getPeopleList(strKeywords))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUniqueAliasName() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("AliasID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.AliasSurname = Common.WebRequest.Data("AliasSurname")
            clsPeople.AliasFirstName = Common.WebRequest.Data("AliasFirstName")
            clsPeople.AliasMiddleName = Common.WebRequest.Data("AliasMiddleName")

            strReturn = JsonDictionary.Serialize("PeopleAliasData", clsPeople.CheckPeopleUniqueAliasName())

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleAliasInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleAliasInfo", clsPeople.getPeopleAliasInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleAliasList() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strReturn = JsonDictionary.Serialize("PeopleAliasList", clsPeople.getPeopleAliasList(WebSession.Instance.PeopleID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleAliasPostSuffixList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleAliasData", clsPeople.getPeopleAliasPostSuffixList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleAliasName() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("AliasID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.AliasTitleID = Common.WebRequest.Data("AliasTitleID")
            clsPeople.AliasSurname = Common.WebRequest.Data("AliasSurname")
            clsPeople.AliasFirstName = Common.WebRequest.Data("AliasFirstName")
            clsPeople.AliasMiddleName = Common.WebRequest.Data("AliasMiddleName")
            clsPeople.AliasPersonalSuffixName = Common.WebRequest.Data("AliasPersonalSuffixName")

            clsPeople.AliasPostSuffixList = Common.WebRequest.AllData("PostSuffixList")

            strReturn = JsonDictionary.Serialize("PeopleAliasData", clsPeople.SavePeopleAliasName())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleAliasName() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("AliasID")

            strReturn = JsonDictionary.Serialize("PeopleAliasData", IIf(clsPeople.RemovePeopleAliasName(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleAddressInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleAddressInfo", clsPeople.getPeopleAddressInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleAddressList() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strReturn = JsonDictionary.Serialize("PeopleAddressList", clsPeople.getPeopleAddressList(WebSession.Instance.PeopleID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleAddressInfo() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("AddressID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.AddressHouseBuildingNo = Common.WebRequest.Data("AddressHouseBuildingNo")
            clsPeople.AddressStreet = Common.WebRequest.Data("AddressStreet")
            clsPeople.AddressCityID = Common.WebRequest.Data("AddressCityID")
            clsPeople.AddressCountryID = Common.WebRequest.Data("AddressCountryID")
            clsPeople.AddressType = Common.WebRequest.Data("AddressType")

            strReturn = JsonDictionary.Serialize("PeopleAddressData", clsPeople.SavePeopleAddess())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleAddress() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("AddressID")

            strReturn = JsonDictionary.Serialize("PeopleAddressData", IIf(clsPeople.RemovePeopleAdress(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUniqueRelatedName() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("RelPersonID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.RelatedSurname = Common.WebRequest.Data("Surname")
            clsPeople.RelatedFirstName = Common.WebRequest.Data("FirstName")
            clsPeople.RelatedMiddleName = Common.WebRequest.Data("MiddleName")

            strReturn = JsonDictionary.Serialize("PeopleRelatedData", clsPeople.CheckPeopleUniqueRelatedName())

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleRelatedInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleRelatedInfo", clsPeople.getPeopleRelatedInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleRelatedList() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strReturn = JsonDictionary.Serialize("PeopleRelatedList", clsPeople.getPeopleRelatedList(WebSession.Instance.PeopleID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleRelatedPostSuffixList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleRelatedData", clsPeople.getPeopleRelatedPostSuffixList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleRelatedInfo() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("RelPersonID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.RelatedTitleID = Common.WebRequest.Data("TitleID")
            clsPeople.RelatedSurname = Common.WebRequest.Data("Surname")
            clsPeople.RelatedFirstName = Common.WebRequest.Data("FirstName")
            clsPeople.RelatedMiddleName = Common.WebRequest.Data("MiddleName")
            clsPeople.RelatedPersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")
            clsPeople.RelatedRelation = Common.WebRequest.Data("Relation")
            clsPeople.RelatedIsEmergencyRef = Common.WebRequest.Data("IsEmergencyRef")
            clsPeople.RelatedTelCountryID = Common.WebRequest.Data("TelCountryID")
            clsPeople.RelatedTelAreaCodeID = Common.WebRequest.Data("TelAreaCodeID")
            clsPeople.RelatedTelContactNo = Common.WebRequest.Data("TelContactNo")
            clsPeople.RelatedTelExtensionNo = Common.WebRequest.Data("TelExtensionNo")
            clsPeople.RelatedTelContactType = Common.WebRequest.Data("TelContactType")
            clsPeople.RelatedMobileCountryID = Common.WebRequest.Data("MobileCountryID")
            clsPeople.RelatedMobileTelCoPrefixID = Common.WebRequest.Data("MobileTelCoPrefixID")
            clsPeople.RelatedMobileContactNo = Common.WebRequest.Data("MobileContactNo")
            clsPeople.RelatedMobileContactType = Common.WebRequest.Data("MobileContactType")
            clsPeople.RelatedEmail = Common.WebRequest.Data("EmailAddress")
            clsPeople.RelatedEmailType = Common.WebRequest.Data("EmailType")

            clsPeople.RelatedPostSuffixList = Common.WebRequest.AllData("PostSuffixList")


            If (clsPeople.RelatedIsEmergencyRef = "1") Then

                If (clsPeople.RelatedTelContactNo.Trim().Length > 0 And Not IsNumeric(clsPeople.RelatedTelContactNo)) Then
                    strReturn = JsonDictionary.Serialize("PeopleRelatedData", "Invalid Tel. No.")
                    Return strReturn
                End If

                If (clsPeople.RelatedTelExtensionNo.Trim().Length > 0 And Not IsNumeric(clsPeople.RelatedTelExtensionNo)) Then
                    strReturn = JsonDictionary.Serialize("PeopleRelatedData", "Invalid Extension or Local No.")
                    Return strReturn
                End If

                If (clsPeople.RelatedMobileContactNo.Trim().Length > 0 And Not IsNumeric(clsPeople.RelatedMobileContactNo)) Then
                    strReturn = JsonDictionary.Serialize("PeopleRelatedData", "Invalid Mobile No.")
                    Return strReturn
                End If

                If (clsPeople.RelatedEmail.Trim().Length > 0 And Not IsEmail(clsPeople.RelatedEmail)) Then
                    strReturn = JsonDictionary.Serialize("PeopleRelatedData", "Invalid Email")
                    Return strReturn
                End If

            End If

            strReturn = JsonDictionary.Serialize("PeopleRelatedData", clsPeople.SavePeopleRelated())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleRelated() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("RelPersonID")

            strReturn = JsonDictionary.Serialize("PeopleRelatedData", IIf(clsPeople.RemovePeopleRelated(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleContactTelInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleContactInfo", clsPeople.getPeopleContactTelInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleContactTelList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            If (WebSession.Instance.PeopleID = Nothing) Then
                strID = Common.WebRequest.Data("PeopleID")
            Else
                strID = IIf(WebSession.Instance.PeopleID.Trim().Length > 0, WebSession.Instance.PeopleID, Common.WebRequest.Data("PeopleID"))
            End If

            strReturn = JsonDictionary.Serialize("PeopleContactList", clsPeople.getPeopleContactTelList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleContactTel() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonContactID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.PersonContactCountryID = Common.WebRequest.Data("ContactCountryID")
            clsPeople.PersonContactAreaCodeID = Common.WebRequest.Data("ContactAreaCodeID")
            clsPeople.PersonContactNo = Common.WebRequest.Data("PersonContacNo")
            clsPeople.PersonContactExtensionNo = Common.WebRequest.Data("PersonContacExtensionNo")
            clsPeople.PersonContactType = Common.WebRequest.Data("PersoncontactType")

            If (clsPeople.PersonContactNo.Trim().Length > 0 And Not IsNumeric(clsPeople.PersonContactNo)) Then
                strReturn = JsonDictionary.Serialize("PeopleContactData", "Invalid Tel. No.")
                Return strReturn
            End If

            If (clsPeople.PersonContactExtensionNo.Trim().Length > 0 And Not IsNumeric(clsPeople.PersonContactExtensionNo)) Then
                strReturn = JsonDictionary.Serialize("PeopleContactData", "Invalid Extension or Local No.")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("PeopleContactData", clsPeople.SavePeopleContactTel())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleContactTel() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonContactID")

            strReturn = JsonDictionary.Serialize("PeopleContactData", IIf(clsPeople.RemovePeopleContactTel(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleContactMobileInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleContactInfo", clsPeople.getPeopleContactMobileInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleContactMobileList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            If (WebSession.Instance.PeopleID = Nothing) Then
                strID = Common.WebRequest.Data("PeopleID")
            Else
                strID = IIf(WebSession.Instance.PeopleID.Trim().Length > 0, WebSession.Instance.PeopleID, Common.WebRequest.Data("PeopleID"))
            End If

            strReturn = JsonDictionary.Serialize("PeopleContactList", clsPeople.getPeopleContactMobileList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleContactMobile() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonContactID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.PersonContactCountryID = Common.WebRequest.Data("PersonContactCountryID")
            clsPeople.PersonContactTelCoPrefixID = Common.WebRequest.Data("PersonContactTelCoPrefixID")
            clsPeople.PersonContactNo = Common.WebRequest.Data("PersonContacNo")
            clsPeople.PersonContactType = Common.WebRequest.Data("PersonContactType")

            If (clsPeople.PersonContactNo.Trim().Length > 0 And Not IsNumeric(clsPeople.PersonContactNo)) Then
                strReturn = JsonDictionary.Serialize("PeopleContactData", "Invalid Mobile No.")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("PeopleContactData", clsPeople.SavePeopleContactMobile())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleContactMobile() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonContactID")

            strReturn = JsonDictionary.Serialize("PeopleContactData", IIf(clsPeople.RemovePeopleContactMobile(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleContactEmailInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleContactInfo", clsPeople.getPeopleContactEmailInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleContactEmailList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            If (WebSession.Instance.PeopleID = Nothing) Then
                strID = Common.WebRequest.Data("PeopleID")
            Else
                strID = IIf(WebSession.Instance.PeopleID.Trim().Length > 0, WebSession.Instance.PeopleID, Common.WebRequest.Data("PeopleID"))
            End If

            strReturn = JsonDictionary.Serialize("PeopleContactList", clsPeople.getPeopleContactEmailList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleContactEmail() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonContactID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.PersonContactEmail = Common.WebRequest.Data("PersonEmail")
            clsPeople.PersonContactEmailType = Common.WebRequest.Data("PersonEmailType")

            If (clsPeople.PersonContactEmail.Trim().Length > 0 And Not IsEmail(clsPeople.PersonContactEmail)) Then
                strReturn = JsonDictionary.Serialize("PeopleContactData", "Invalid Email")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("PeopleContactData", clsPeople.SavePeopleContactEmail())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleContactEmail() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonContactID")

            strReturn = JsonDictionary.Serialize("PeopleContactData", IIf(clsPeople.RemovePeopleContactEmail(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleEmpHistInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleEmpHistInfo", clsPeople.getPeopleEmpHistoryInfo(strID))

            Return strReturn

        End Function

        Function getPeopleEmpHistJobOccList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleEmpHistJobOccInfo", clsPeople.getPeopleEmpHistoryJobOccList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleEmpHistList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleID
            strReturn = JsonDictionary.Serialize("PeopleEmpHistList", clsPeople.getPeopleEmpHistoryList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleEmpHistory() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonEmpHistID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.PersonEmpHistCompany = Common.WebRequest.Data("CompanyName")
            clsPeople.PersonEmpHistJobOccList = Common.WebRequest.AllData("JobOccList")

            strReturn = JsonDictionary.Serialize("PeopleEmpHistData", clsPeople.SavePeopleEmpHistory())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleEmpHistory() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonEmpHistID")

            strReturn = JsonDictionary.Serialize("PeopleEmpHistData", IIf(clsPeople.RemovePeopleEmpHistory(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleEducationInfo() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("PeopleEducationInfo", clsPeople.getPeopleEducationInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleEducationList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            strID = WebSession.Instance.PeopleID
            strReturn = JsonDictionary.Serialize("PeopleEducationData", clsPeople.getPeopleEducationList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleEducation() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonEducationID")
            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.PersonEducCourseID = Common.WebRequest.Data("EducationCourseID")
            clsPeople.PersonEducAcademicLevel = Common.WebRequest.Data("AcademicLevel")
            clsPeople.PersonEducInstitutionName = Common.WebRequest.Data("InstitutionName")
            clsPeople.PersonEducAddressHouseBuildingNo = Common.WebRequest.Data("HouseBuildingNo")
            clsPeople.PersonEducAddressStreet = Common.WebRequest.Data("Street")
            clsPeople.PersonEducAddressCityID = Common.WebRequest.Data("CityID")
            clsPeople.PersonEducAddressCountryID = Common.WebRequest.Data("CountryID")
            clsPeople.PersonEducYearStarted = Common.WebRequest.Data("YearStarted")
            clsPeople.PersonEducYearGraduated = Common.WebRequest.Data("YearGraduated")
            clsPeople.PersonEducIsGraduated = Common.WebRequest.Data("IsGraduated")

            strReturn = JsonDictionary.Serialize("PeopleEducationData", clsPeople.SavePeopleEducation())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemovePeopleEducation() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("PersonEducationID")

            strReturn = JsonDictionary.Serialize("PeopleEducationData", IIf(clsPeople.RemovePeopleEduction(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUserAccount() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As New DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            Try
                clsPeople.UserName = Common.WebRequest.Data("Username")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
                dtaTable = clsPeople.CheckPeopleUserAccount()
                pOut = New JsonDictionary
                If (dtaTable.Rows.Count > 0) Then
                    intCntr = 0
                    Do Until (intCntr >= dtaTable.Rows.Count)

                        If (ProPack.Security.Hash.Verify(clsPeople.UserPassword, dtaTable.Rows(intCntr).Item("UserPassword").ToString())) Then
                            clsPeople.PeopleID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                            '' ''WebSession.Instance.UserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                            WebSession.Instance.PeopleUserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                            WebSession.Instance.FirstName = dtaTable.Rows(intCntr).Item("FirstName").ToString()
                            WebSession.Instance.FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                            WebSession.Instance.DeviceSerial = Common.WebRequest.Data("Serial")
                            WebSession.Instance.IsMobile = CDec2(Common.WebRequest.Data("IsMobile"))
                            If CStr2(WebSession.Instance.DeviceSerial).Trim = "" _
                               AndAlso WebSession.Instance.IsMobile Then

                                Return JsonDictionary.Serialize("Error", "Device serial cannot be blank if mobile")
                                ' Throw New ApplicationException("Device serial cannot be blank if mobile")
                            End If

                            If CDec2(Common.WebRequest.Data("IsCookieless")) <> 0 Then
                                pOut.Add("ASP.NET_SessionId", HttpContext.Session.SessionID)
                                '// the ff commented code dont work
                                'For Each pCookieName As String In Response.Cookies
                                '    pCookie = Response.Cookies(pCookieName)
                                '    pOut.Add(pCookie.Name, pCookie.Value)
                                'Next pCookieName
                            End If

                            If CStr2(Common.WebRequest.Data("LoginHash")).Length = 50 Then

                            End If
                            pOut.Add("PeopleUserAccountData", WebSession.Instance.PeopleUserID)

                            Exit Do
                        End If

                        intCntr = intCntr + 1
                    Loop
                End If

                Return pOut.Serialize
            Catch ex As System.Exception
                AppendToDebug("vsMXWE4SRNRM!", , ex, True)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function VerifyPeopleUserAccount() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As New DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            Try
                clsPeople.UserName = Common.WebRequest.Data("Username")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
                dtaTable = clsPeople.CheckPeopleUserAccount()
                pOut = New JsonDictionary
                If (dtaTable.Rows.Count > 0) Then
                    intCntr = 0
                    Do Until (intCntr >= dtaTable.Rows.Count)

                        If (ProPack.Security.Hash.Verify(clsPeople.UserPassword, dtaTable.Rows(intCntr).Item("UserPassword").ToString())) Then
                            pOut.Add("PeopleUserAccountData", dtaTable.Rows(intCntr).Item("PeopleID").ToString())
                            Exit Do
                        End If

                        intCntr = intCntr + 1
                    Loop
                End If

                Return pOut.Serialize
            Catch ex As System.Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function getPeopleUserAccount() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.PeopleID = WebSession.Instance.PeopleID
            clsPeople.UserName = Common.WebRequest.Data("Surname")
            clsPeople.UserPassword = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("FirstName"))

            strReturn = JsonDictionary.Serialize("PeopleUserAccountData", clsPeople.getPeopleUserAccount())

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleUserAccount() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            clsPeople.UserName = Common.WebRequest.Data("Username")
            clsPeople.UserPassword = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("UserPassword"))

            clsPeople.SecurityQuestion1ID = Common.WebRequest.Data("Question1ID")
            clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
            clsPeople.SecurityQuestion1Answer = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("Question1Answer"))
            clsPeople.SecurityQuestion2ID = Common.WebRequest.Data("Question2ID")
            clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
            clsPeople.SecurityQuestion2Answer = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("Question1Answer2"))

            strReturn = JsonDictionary.Serialize("PeopleUserAccountData", clsPeople.SavePeopleUserAccount())

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleDTRPIN() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            
            strReturn = JsonDictionary.Serialize("PeopleUserAccountData", clsPeople.SavePeopleDTRPIN())

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleSecurityQuestionList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As DataTable
            Dim dtaRow As DataRow

            strID = ""
            clsPeople.ID = ""
            clsPeople.Surname = Common.WebRequest.Data("Surname")
            clsPeople.FirstName = Common.WebRequest.Data("FirstName")
            clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
            clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
            clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
            clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
            clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")

            dtaTable = New DataTable()
            dtaTable = clsPeople.CheckPeopleUniqueName(True)
            If (dtaTable.Rows.Count > 0) Then
                dtaRow = dtaTable.Rows(0)
                strID = dtaRow("ID").ToString()
            End If

            strReturn = JsonDictionary.Serialize("PeopleSecurityData", clsPeople.getPeopleSecurityQuestionList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleSecurityQuestionByPeopleIDList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleID
            strReturn = JsonDictionary.Serialize("PeopleSecurityData", clsPeople.getPeopleSecurityQuestionList(strID))

            Return strReturn

        End Function

        'EDITED by Janine
        <HttpPost()>
        Function CheckPeopleSecurityQuestions() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            clsPeople.SecurityQuestion1ID = Common.WebRequest.Data("Question1ID")
            clsPeople.SecurityQuestion1Answer = Common.WebRequest.Data("Question1Answer")
            clsPeople.SecurityQuestion2ID = Common.WebRequest.Data("Question2ID")
            clsPeople.SecurityQuestion2Answer = Common.WebRequest.Data("Question2Answer")

            'dtaTable = clsPeople.CheckPeopleSecurityQuestion(clsPeople.PeopleID, clsPeople.SecurityQuestion1ID, clsPeople.SecurityQuestion1Answer)
            'If (dtaTable.Rows.Count <= 0) Then
            '    strReturn = JsonDictionary.Serialize("PeopleSecurityData", "INVALID")
            '    Return strReturn
            'End If

            'If (clsPeople.SecurityQuestion2ID.Trim().Length > 0) Then
            '    dtaTable = clsPeople.CheckPeopleSecurityQuestion(clsPeople.PeopleID, clsPeople.SecurityQuestion2ID, clsPeople.SecurityQuestion2Answer)
            '    If (dtaTable.Rows.Count <= 0) Then
            '        strReturn = JsonDictionary.Serialize("PeopleSecurityData", "INVALID")
            '        Return strReturn
            '    End If
            'End If

            'WebSession.Instance.PeopleID = clsPeople.PeopleID
            'WebSession.Instance.PeopleUserID = dtaTable.Rows(0).Item("PeopleID").ToString()

            'strReturn = JsonDictionary.Serialize("PeopleSecurityData", "VALID")
            'Return strReturn

            'EDITED by Janine
            'Check whether answer to security question is correct (compare hash)

            strReturn = "INVALID"
            pOut = New JsonDictionary
            dtaTable = clsPeople.CheckPeopleSecurityQuestion(clsPeople.PeopleID, clsPeople.SecurityQuestion1ID, clsPeople.SecurityQuestion1Answer)
            If (dtaTable.Rows.Count > 0) Then
                intCntr = 0
                Do Until (intCntr >= dtaTable.Rows.Count)
                    'old security answers
                    If ((ProPack.Security.Hash.Verify(clsPeople.SecurityQuestion1Answer, dtaTable.Rows(intCntr).Item("SecurityAnswer").ToString())) Or (dtaTable.Rows(intCntr).Item("SecurityAnswer").ToString() = clsPeople.SecurityQuestion1Answer)) Then
                        strReturn = "VALID"
                        Exit Do
                    Else
                        pOut.Add("PeopleSecurityData", "INVALID")
                        Return pOut.Serialize
                    End If

                    intCntr = intCntr + 1
                Loop

            End If

            If (clsPeople.SecurityQuestion2ID.Trim().Length > 0) Then
                dtaTable = clsPeople.CheckPeopleSecurityQuestion(clsPeople.PeopleID, clsPeople.SecurityQuestion2ID, clsPeople.SecurityQuestion2Answer)
                If (dtaTable.Rows.Count > 0) Then
                    intCntr = 0
                    Do Until (intCntr >= dtaTable.Rows.Count)

                        If ((ProPack.Security.Hash.Verify(clsPeople.SecurityQuestion2Answer, dtaTable.Rows(intCntr).Item("SecurityAnswer").ToString())) Or (dtaTable.Rows(intCntr).Item("SecurityAnswer").ToString() = clsPeople.SecurityQuestion2Answer)) Then
                            strReturn = "VALID"
                            Exit Do
                        Else
                            pOut.Add("PeopleSecurityData", "INVALID")
                            Return pOut.Serialize
                        End If

                        intCntr = intCntr + 1
                    Loop

                    WebSession.Instance.PeopleID = clsPeople.PeopleID
                    WebSession.Instance.PeopleUserID = dtaTable.Rows(0).Item("PeopleID").ToString()

                End If

                pOut.Add("PeopleSecurityData", strReturn)

                Return pOut.Serialize

            End If

        End Function

        <HttpPost()>
        Function getPeopleOrganizationList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleUserID
            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleOrganizationList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleMessageList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleUserID
            If CStr2(strID) = "" Then
                strID = Common.WebRequest.Data("PeopleUserID")
                If CStr2(strID) = "" Then
                    Return JsonDictionary.Serialize("Error", "Access denied")
                End If
            End If
            If IsGuid(strID) = False Then
                Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleMessageList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleMessageT1Details() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("MsgID")
            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleMessageT1Details(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function ConfirmPeopleMessageT1Details() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsPeople.ID = Common.WebRequest.Data("MsgID")

            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.ConfirmPeopleMessageT1Details())

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleMessageT2Details() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("MsgID")
            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleMessageT2Details(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetDatabaseName() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim mProData As ProData = Common.WebRequest.ProData
            Dim i As Integer


            pSql = "SELECT dbo.MobizLibraryOrganization.CompanyName, dbo.MobizLibrarySubscription.* " & vbCrLf &
                "FROM dbo.MobizLibrarySubscription INNER JOIN " & vbCrLf &
                "dbo.MobizLibraryOrganization ON dbo.MobizLibrarySubscription.OrganizationID = dbo.MobizLibraryOrganization.ID  " & vbCrLf &
                "Where OrganizationID = " & ReadyString(WebRequest.Instance.Data("OrgID"))

            'For Each pObj As Object In Common.WebRequest.AllData("OrgID")
            '    If i = 0 Then
            '        pSql &= " OrganizationID = " & ReadyString(pObj("OrgID"))
            '    Else
            '        pSql &= " OR OrganizationID = " & ReadyString(pObj("OrgID"))
            '    End If

            '    i += 1
            'Next

            pDT = mProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        <HttpPost()>
        Function GetPeopleInfo() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim mProData As ProData = Common.WebRequest.ProData
            Dim i As Integer

            'WebSession.Instance.PeopleUserID is assigned in CheckPeopleUserAccount function
            pSql = "Select * from LibraryPeople " & vbCrLf &
                "Where ID = " & ReadyString(WebSession.Instance.PeopleUserID)

            pDT = mProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        <HttpPost()>
        Function GetPeopleOrganizationInfo() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim mProData As ProData = Common.WebRequest.ProData
            Dim i As Integer

            'WebSession.Instance.PeopleUserID is assigned in CheckPeopleUserAccount function
            pSql = "select * from LibraryPeopleOrganization " & vbCrLf &
                "Where PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)

            pDT = mProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        <HttpPost()>
        Function GetPeopleUserAccountInfo() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim mProData As ProData = Common.WebRequest.ProData
            Dim i As Integer

            'WebSession.Instance.PeopleUserID is assigned in CheckPeopleUserAccount function
            pSql = "select * from LibraryPeopleUserAccount " & vbCrLf &
                "Where PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)

            pDT = mProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        <HttpPost()>
        Function GetPeopleSecurityRole() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim mProData As ProData = Common.WebRequest.ProData
            Dim i As Integer

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'WebSession.Instance.PeopleUserID is assigned in CheckPeopleUserAccount function
            pSql = "select * from LibrarySecurityRoleUser " & vbCrLf &
                "Where PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)

            pDT = mProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        <HttpPost()>
        Function GetOrganizationEmployeeFOWST() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim mProData As ProData = Common.WebRequest.ProData
            Dim i As Integer

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'WebSession.Instance.PeopleUserID is assigned in CheckPeopleUserAccount function
            pSql = "select * from LibraryOrganizationEmployeeFOWST " & vbCrLf &
                "Where PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)

            pDT = mProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function


    End Class
End Namespace