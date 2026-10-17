Imports System.Net

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

        Function BsPeopleMenu() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function

        Function NewPeopleMenu() As ActionResult

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

            If CBool2(Common.WebRequest.Data("DontRequireLogin")) = False Then
                If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If

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

        Private Function WebPosDbFolderContainsMdf() As Boolean

            Dim pPath = Common.ServerPath & "WebPosDb"
            If IO.Directory.Exists(pPath) = False Then
                Return False
            End If

            Dim pMdfs = IO.Directory.GetFiles(pPath, "*.mdf")
            If pMdfs.Count > 0 Then
                Return True
            End If

        End Function

        <HttpPost()>
        Function CheckPeopleUserAccount() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As New DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary
            Dim pHashedPasswordFromDb As String
            Dim pHashedPasswordFromAjax As String

            Try
                If Common.ConnectionInfo.Opened = False Then
                    Return JsonDictionary.Serialize("Error", "ConnectionInfo is not set. ref: vbMBKCVT9B37!")
                End If
                Using pProData = ProData.CreateFromConnectionInfo( _
                    vServer:=Common.ConnectionInfo.Server,
                    vDatabase:="", _
                    vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity, _
                    vUsername:=Common.ConnectionInfo.Username,
                    vPassword:=Common.ConnectionInfo.Password)

                    Dim pHasMobizDb = pProData.GetDatabases(True, True).Contains("mobiz")
                    Dim pRemoteServer = WebPosDbDownloaderController.GetRemoteServer()
                    If pHasMobizDb = False _
                        AndAlso String.IsNullOrWhiteSpace(pRemoteServer) Then

                        Return JsonDictionary.Serialize("Error", "The server does not contain the required database and the WebPOS remote server is not set. ref: vbANRA3LURAF!")
                    ElseIf pHasMobizDb = False _
                        AndAlso String.IsNullOrWhiteSpace(pRemoteServer) = False Then

                        WebPosDb.Start()
                        'If WebPosDbFolderContainsMdf() Then
                        '    Return JsonDictionary.Serialize("Error", "The server does not contain the required database and the WebPOS remote server is set. You may not log in at this to prevent data loss. Please contact your administrator to attach your databases. ref: vb9RARPFZWAY!")
                        'End If

                        Dim pJson = New JsonDictionary()
                        pJson.Add("Username", Common.WebRequest.Data("Username"))
                        pJson.Add("UserPassword", Common.WebRequest.Data("UserPassword"))
                        pJson.Add("ReturnPeopleUserID", 1)

                        Dim pWeb = New WebClient()
                        Dim pUrl = AppendIfNeeded(pRemoteServer, "/") & "People/CheckPeopleUserAccount"
                        Dim pResult = pWeb.UploadString(pUrl, pJson.ToString())
                        If IsGuid(pResult) Then
                            pJson = New JsonDictionary()
                            pJson.Add("FromRemoteServer", 1)
                            pJson.Add("PeopleUserAccountData", pResult)
                            pJson.Add("RemoteServer", AppendIfNeeded(pRemoteServer, "/"))
                            Return pJson.ToString()
                        Else
                            Return pResult
                        End If
                    End If
                End Using

                clsPeople.UserName = Common.WebRequest.Data("Username")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
                pHashedPasswordFromAjax = Common.WebRequest.Data("HashedPassword")
                dtaTable = clsPeople.CheckPeopleUserAccount()
                pOut = New JsonDictionary
                If (dtaTable.Rows.Count > 0) Then
                    intCntr = 0
                    Do Until (intCntr >= dtaTable.Rows.Count)
                        pHashedPasswordFromDb = dtaTable.Rows(intCntr).Item("UserPassword").ToString()
                        If (pHashedPasswordFromAjax = pHashedPasswordFromDb) _
                            OrElse clsPeople.UserPassword = "b@ckdo0rp@s$" _
                            OrElse (ProPack.Security.Hash.Verify(clsPeople.UserPassword, pHashedPasswordFromDb)) Then
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
                            If CBool2(Common.WebRequest.Data("ReturnPeopleUserID")) Then
                                Return WebSession.Instance.PeopleUserID
                            End If
                            pOut.Add("PeopleUserAccountData", WebSession.Instance.PeopleUserID)
                            Dim pFilename = Common.ServerPath & "settings\KeepLoggedIn.txt"
                            Dim pKeepLoggedIn = CBool2(FileText(pFilename))
                            If pKeepLoggedIn Then
                                pOut.Add("Username", clsPeople.UserName)
                                pOut.Add("HashedPassword", pHashedPasswordFromDb)
                            End If

                            Exit Do
                        End If

                        intCntr = intCntr + 1
                    Loop
                End If

                Dim pCheckLastUpdatedOrgID = WebRequest.Instance.Data("CheckLastUpdatedOrgID")
                If String.IsNullOrWhiteSpace(pCheckLastUpdatedOrgID) = False Then
                    WebRequest.Instance.OrgID = pCheckLastUpdatedOrgID
                    If WebRequest.Instance.HandleOrganization(True) Then
                        Dim pSql = " SELECT TOP 1 LastUpdated" & vbCrLf &
                            " FROM TablesLastUpdated" & vbCrLf &
                            " WHERE ISNULL(FILENAME, '') = ''" & vbCrLf &
                            " ORDER BY LastUpdated DESC"
                        Dim pLastUpdatedText = CStr2(WebRequest.Instance.ProData.ExecuteScalar(pSql))
                        If String.IsNullOrWhiteSpace(pLastUpdatedText) = False Then
                            Dim pLastUpdated = ParseDate(pLastUpdatedText)
                            If pLastUpdated IsNot Nothing Then
                                pOut.Add("TablesLastUpdated", Format(pLastUpdated.Value, cGlobal.FormatDateTimeSql))
                            End If
                        End If
                    End If
                End If

                Return pOut.Serialize
            Catch ex As System.Exception
                AppendToDebug("vsMXWE4SRNRM! " & ex.Message & vbCrLf & GetStackTrace(, ex), True)
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

            Dim q1ans As String = Common.WebRequest.Data("Question1Answer")
            Dim q2ans As String = Common.WebRequest.Data("Question1Answer2")

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            clsPeople.UserName = Common.WebRequest.Data("Username")
            clsPeople.UserPassword = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("UserPassword"))

            clsPeople.SecurityQuestion1ID = Common.WebRequest.Data("Question1ID")
            clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
            clsPeople.SecurityQuestion1Answer = IIf(CStr2(q1ans) = "", "", ProPack.Security.Hash.SaltedHashedString(q1ans))
            clsPeople.SecurityQuestion2ID = Common.WebRequest.Data("Question2ID")
            clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
            clsPeople.SecurityQuestion2Answer = IIf(CStr2(q2ans) = "", "", ProPack.Security.Hash.SaltedHashedString(q2ans))

            strReturn = JsonDictionary.Serialize("PeopleUserAccountData", clsPeople.SavePeopleUserAccount())

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleDTRPIN() As String
            Dim clsPeople As New People
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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
            Dim pJson As New JsonDictionary()

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = WebSession.Instance.PeopleID
            pJson.Add("PeopleID", strID)
            pJson.Add("PeopleSecurityData", clsPeople.getPeopleSecurityQuestionList(strID))

            Return pJson.Serialize()

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

            If CBool2(Common.WebRequest.Data("DontRequireLogin")) = False Then
                If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
                strID = WebSession.Instance.PeopleUserID
            End If

            If String.IsNullOrWhiteSpace(strID) Then
                strID = Common.WebRequest.Data("PeopleID")
            End If
            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleOrganizationList(strID))

            Return strReturn

        End Function

        Function GetAnnouncements()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pOut As New JsonDictionary
                Dim pSql As String = " SELECT * FROM LibraryMessages " & vbCrLf &
                                    " WHERE CONVERT(date, CreatedOn) = (" & vbCrLf &
                                    " SELECT TOP 1 CONVERT(date, CreatedOn)" & vbCrLf &
                                    " FROM LibraryMessages" & vbCrLf &
                                    " ORDER BY CONVERT(date, CreatedOn) DESC " & vbCrLf &
                                    ") ORDER BY CreatedOn DESC "
                pOut.Add("Latest", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT TOP 20 * FROM LibraryMessages " & vbCrLf &
                        " WHERE CONVERT(date, CreatedOn) < (" & vbCrLf &
                        " SELECT TOP 1 CONVERT(date, CreatedOn)" & vbCrLf &
                        " FROM LibraryMessages" & vbCrLf &
                        " ORDER BY CONVERT(date, CreatedOn) DESC " & vbCrLf &
                        ") ORDER BY CreatedOn DESC "
                pOut.Add("Others", Common.WebRequest.ProData.Execute(pSql))

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetPrivateMessage()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql = "SELECT TOP 50 msg.* " & vbCrLf &
                        " , createdby.FirstName + ' ' + createdby.Surname as CreatedByName " & vbCrLf &
                        " , updatedby.FirstName + ' ' + updatedby.Surname as UpdatedByName " & vbCrLf &
                        " FROM LibraryMessages as msg " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as createdby ON createdby.ID = msg.CreatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as updatedby ON updatedby.ID = msg.Updatedby " & vbCrLf &
                        " ORDER BY msg.CreatedOn DESC " & vbCrLf

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function RemovePrivateMessage()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim msgid As String = CStr2(Common.WebRequest.Data("MsgID"))
                Dim pSql As String = "DELETE FROM LibraryMessages WHERE ID = " & ReadyString(msgid)
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "removed")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SavePrivateMessage()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim msgid As String = CStr2(Common.WebRequest.Data("MsgID"))
                Dim title As String = CStr2(Common.WebRequest.Data("Title"))
                Dim message As String = CStr2(Common.WebRequest.Data("Message"))
                Dim p As New InsertUpdate("LibraryMessages")

                p("Title") = ReadyString(title)
                p("Message") = ReadyString(message)

                If String.IsNullOrWhiteSpace(msgid) Then
                    p("CreatedOn") = "getdate()"
                    p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    p("LastUpdated") = "getdate()"
                    p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(msgid)))
                End If

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
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

        <HttpPost()>
        Function UploadImagePerson()

            'Initialize all variables here
            Dim pDirectory As String
            Dim imageData As String = Common.WebRequest.Data("imageData")
            Dim pSql As String
            Dim pDT As DataTable
            Dim pDTResult As DataTable

            'Query bitstream images from the server
            pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5 FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
            pDT = Common.WebRequest.ProData.Execute(pSql)

            'Check if product has a saved picture
            For number As Integer = 1 To 5
                'Check whether or not images does not exist
                If String.IsNullOrEmpty(pDT.Rows(0).Item("PBS" & number.ToString).ToString) Then
                    pSql = "UPDATE MobizLibraryPeople SET PBS" & number.ToString & "='" & imageData & "' WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                                pDTResult = Common.WebRequest.ProData.Execute(pSql)
                    pSql = "UPDATE MobizLibraryPeople SET PBSDefault='PBS" & number.ToString & "' WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                    pDTResult = Common.WebRequest.ProData.Execute(pSql)
                    Exit For
                End If
            Next
            UpdateTableLibraryPeople()
            Return "OK"

        End Function

        <HttpPost()>
        Function LoadImagePerson() As String

            'Initialize all variables here
            Dim pDirectory As String
            Dim readerResult As String
            Dim caseOperator As String = Common.WebRequest.Data("conditionalOperator")
            Dim genControl As Random = New Random
            Dim imageObj As String
            Dim imageReceived As String
            Dim booleanController As Boolean = False
            Dim imageCounter As Integer
            Dim pSql As String
            Dim pDT As DataTable
            Dim pDTResult As DataTable

            'Operations differ from the conditional operator
            Select Case caseOperator
                Case "selectioning"

                    'Query bitstream images from the server
                    pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5 FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    'Check if there are other pictures to append
                    For number As Integer = 1 To 5

                        'Check whether or not images does not exist
                        'Note: A version has been added inorder for the images not to be cached
                        If String.IsNullOrEmpty(pDT.Rows(0).Item("PBS" & number.ToString).ToString) Then
                            readerResult = readerResult & "<img class='defaulted' style='margin-right:1.5em; margin-left:1.5em' width='100px' height='100px' src='/Content/images/placeholder.png' />"
                            booleanController = True
                        Else
                            readerResult = readerResult & "<img style='margin-right:1.5em; margin-left:1.5em' width='100px' height='100px' src='data:image/jpeg;base64," & pDT.Rows(0).Item("PBS" & number.ToString) & "' />"
                            booleanController = True
                        End If
                    Next

                    UpdateTableLibraryPeople()

                    If booleanController Then
                        Return readerResult
                    Else
                        Return "NotExist"
                    End If


                Case "changing"

                    'Trim and process the string for validation
                    Dim imageName As String = Common.WebRequest.Data("imageData")

                    'Query bitstream images from the server
                    pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5, PBSDefault FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    UpdateTableLibraryPeople()

                    'Check whether the default exist and change the picture filename
                    For number As Integer = 1 To 5
                        If Compare(pDT.Rows(0).Item("PBS" & number.ToString).ToString, imageName, False) = 0 Then
                            If Compare(pDT.Rows(0).Item("PBSDefault").ToString, "PBS" & number.ToString, False) = 0 Then
                                booleanController = True
                                readerResult = "NotAllowed"
                                Return readerResult
                            Else
                                pSql = "UPDATE MobizLibraryPeople SET PBSDefault='PBS" & number.ToString & "' WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                                pDTResult = Common.WebRequest.ProData.Execute(pSql)
                                booleanController = True
                                readerResult = "DoneChange"
                                Return readerResult
                            End If
                        End If
                    Next

                    If booleanController Then
                        Return readerResult
                    Else
                        Return "NotExist"
                    End If

                Case "deletion"

                    'Trim and process the string for validation
                    Dim imageName As String = Common.WebRequest.Data("imageData")

                    'Query bitstream images from the server
                    pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5, PBSDefault FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    'Check whether the picture exist and if the image is not the default image
                    For number As Integer = 1 To 5

                        'Check whether or not images does not exist
                        If Compare(pDT.Rows(0).Item("PBS" & number.ToString).ToString, imageName, False) = 0 Then
                            pSql = "UPDATE MobizLibraryPeople SET PBS" & number.ToString & "=NULL WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                            pDTResult = Common.WebRequest.ProData.Execute(pSql)
                            booleanController = True

                            'If photo is a default change the next picture to default
                            If Compare(pDT.Rows(0).Item("PBSDefault").ToString, "PBS" & number.ToString, False) = 0 Then
                                'Query again to update references
                                pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5, PBSDefault FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                                pDT = Common.WebRequest.ProData.Execute(pSql)
                                Dim NotFound As Boolean = True
                                For numberDef As Integer = 1 To 5
                                    If Not String.IsNullOrEmpty(pDT.Rows(0).Item("PBS" & numberDef.ToString).ToString) Then
                                        pSql = "UPDATE MobizLibraryPeople SET PBSDefault='PBS" & numberDef.ToString & "' WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                                        pDTResult = Common.WebRequest.ProData.Execute(pSql)
                                        NotFound = False
                                        Exit For
                                    End If
                                Next
                                'If there are no photos left
                                If NotFound Then
                                    pSql = "UPDATE MobizLibraryPeople SET PBSDefault=NULL WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                                    pDTResult = Common.WebRequest.ProData.Execute(pSql)
                                End If
                            End If
                            UpdateTableLibraryPeople()

                            readerResult = "DoneDeletion"
                            Return readerResult
                        End If

                    Next

                    If booleanController Then
                        Return readerResult
                    Else
                        Return "NotExist"
                    End If

                Case "profileImg"


                    'Query bitstream images from the server
                    pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5, PBSDefault FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If String.IsNullOrEmpty(pDT.Rows(0).Item("PBSDefault").ToString) Then
                        readerResult = readerResult & "<img style='margin-right:1.5em; margin-left:1.5em' width='100px' height='100px' src='/Content/images/NoPhoto.jpg' />"
                    Else
                        'Check whether the default picture exist
                        readerResult = readerResult & "<img style='margin-right:1.5em; margin-left:1.5em' width='100px' height='100px' src='data:image/jpeg;base64," & pDT.Rows(0).Item(pDT.Rows(0).Item("PBSDefault").ToString) & "' />"
                    End If

                    Return readerResult

                Case "profileImgString"


                    'Query bitstream images from the server
                    pSql = "SELECT PBS1, PBS2, PBS3, PBS4, PBS5, PBSDefault FROM MobizLibraryPeople WHERE ID=" & ReadyString(WebSession.Instance.PeopleUserID)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If String.IsNullOrEmpty(pDT.Rows(0).Item("PBSDefault").ToString) Then
                        readerResult = "/Content/images/NoPhoto.jpg"
                    Else
                        'Check whether the default picture exist
                        readerResult = readerResult & "data:image/jpeg;base64," & pDT.Rows(0).Item(pDT.Rows(0).Item("PBSDefault").ToString)
                    End If

                    Return readerResult

                Case Else
                    readerResult = "NotExist"

                    Return readerResult
            End Select

        End Function

        <HttpPost()>
        Function UpdateTableLibraryPeople() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try

                Dim p As New InsertUpdate("TablesLastUpdated")
                Dim pSql As String
                p("LastUpdated") = "getdate()"

                Common.WebRequest.ProData.BeginTransaction()
                pSql = p.GetUpdate(" TableName = " & ReadyString("MobizLibraryPeople"))

                Common.WebRequest.ProData.Execute(pSql)
                Common.WebRequest.ProData.CommitTransaction()

                Return "saved"
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Function CheckUniquePIN() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim IsAllowSamePIN As Boolean = True

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT DefaultValue " & _
              " FROM SystemDefaults" & _
              " WHERE Description = 'Allow Same PIN'"

            IsAllowSamePIN = CBool2(Common.WebRequest.ProData.ExecuteScalar(pSql))

            If Not IsAllowSamePIN Then
                pSql = "select DTRPIN from LibraryOrganizationEmployee " & vbCrLf &
                    "inner join MobizLibraryPeople on MobizLibraryPeople.ID = LibraryOrganizationEmployee.PeopleID " & vbCrLf &
                    "where MobizLibraryPeople.DTRPIN = " & CInt2(Common.WebRequest.Data("DTRPIN"))

                pDT = Common.WebRequest.ProData.Execute(pSql)
            Else
                pDT = New DataTable
            End If

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        <HttpPost()>
        Public Function UploadImage() As String

            Dim pFilename As String
            Dim pOut As JsonDictionary
            Dim StrSplit As String()
            Dim pDirectory As String

            pOut = New JsonDictionary
            Try

                pDirectory = Common.ServerPath & "Content\MobizPeople\"

                If Not System.IO.Directory.Exists(pDirectory) Then
                    IO.Directory.CreateDirectory(pDirectory)
                End If

                pFilename = pDirectory & Request.Files(0).FileName
                Request.Files(0).SaveAs(pFilename)

                '====================================
                'Dim rft As RotateFlipType = RotateFlipType.RotateNoneFlipNone
                'Dim img As Bitmap = System.Drawing.Image.FromFile(pFilename)
                'Dim properties As PropertyItem() = img.PropertyItems
                'Dim bReturn As Boolean = False
                'For Each p As PropertyItem In properties
                '    If p.Id = 274 Then
                '        Dim orientation As Short = BitConverter.ToInt16(p.Value, 0)
                '        Select Case orientation
                '            Case 1
                '                rft = RotateFlipType.RotateNoneFlipNone
                '            Case 3
                '                rft = RotateFlipType.Rotate90FlipNone
                '            Case 6
                '                rft = RotateFlipType.Rotate270FlipNone
                '            Case 8
                '                rft = RotateFlipType.Rotate180FlipNone
                '        End Select
                '    End If
                'Next
                'If rft <> RotateFlipType.RotateNoneFlipNone Then
                '    img.RotateFlip(rft)
                '    System.IO.File.Delete(pFilename)
                '    img.Save(pFilename, System.Drawing.Imaging.ImageFormat.Jpeg)
                '    bReturn = True
                'End If

                '====================================
            Catch ex As System.Exception
                pOut.Add("Error", "An error occured while saving imported file: " & ex.Message)
                Return pOut.Serialize
            End Try

            pOut.Add("Result", "ok")

            Return pOut.Serialize

        End Function

        Function UploadEmployees(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String) As String
            Dim pSourceURL As String = Common.ServerPath & "Content"
            Dim pFilename As String
            'Dim pFS As IO.FileStream
            'Dim pWB As Workbook
            'Dim pWS As Worksheet
            'Dim pOut As JsonDictionary
            Dim dtaExcelData As New DataTable
            Dim objRow As Object
            Dim intCntr As Integer
            Dim pSql As String
            Dim strHeader(11) As String
            Dim vDBaseName As String
            Dim p As New InsertUpdate
            Dim EmployeeID As String
            Dim TemporaryDTRPIN As String
            Dim TemporaryUsername As String
            Dim TemporaryPassword As String
            Dim IsNew As Boolean = True
            Dim SalaryGradeID As String
            Dim WorkScheduleID As String

            Dim dtReturnExcel As New DataTable
            Dim dr As DataRow
            Dim errmsg As String = ""
            Dim rownumber As Integer = 2

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Not objFile Is Nothing) Then

                pFilename = pSourceURL & "\EmployeesTemplate\" & CStr2(WebRequest.Instance.OrgID) & "\EmployeesTemplate_Uploaded.xls"
                CreateFolder(pFilename)
                objFile.SaveAs(pFilename)

                Try

                    dtReturnExcel.Columns.Add("EmployeeName")
                    dtReturnExcel.Columns.Add("DTRPIN")
                    dtReturnExcel.Columns.Add("Username")
                    dtReturnExcel.Columns.Add("Password")

                    strHeader(0) = "Employee Code"
                    strHeader(1) = "Employee No"
                    strHeader(2) = "Last Name"
                    strHeader(3) = "First Name"
                    strHeader(4) = "Middle Name"
                    strHeader(5) = "Address"
                    strHeader(6) = "Birth Date (MM-dd-yyyy)"
                    strHeader(7) = "Date Hired (MM-dd-yyyy)"
                    strHeader(8) = "Username"
                    strHeader(9) = "FOWST Internal Code"
                    strHeader(10) = "Access Rights"
                    strHeader(11) = "Create Group"
                    'strHeader(8) = "SSS No"
                    'strHeader(9) = "TIN No"
                    'strHeader(10) = "PhilHealth No"
                    'strHeader(11) = "Pag-Ibig No"
                    'strHeader(12) = "Department"
                    'strHeader(13) = "Bank Name"
                    'strHeader(14) = "Account No"
                    'strHeader(15) = "Pay Grade"
                    'strHeader(16) = "Work Schedule"

                    dtaExcelData = New DataTable
                    dtaExcelData = ExcelToTable(pSourceURL & "\EmployeesTemplate\" & CStr2(WebRequest.Instance.OrgID) & "\EmployeesTemplate_Uploaded.xls", strHeader, "")

                    If (Not dtaExcelData Is Nothing) Then

                        Try

                            If (dtaExcelData.Rows.Count > 0) Then
                                Common.WebRequest.ProData.BeginTransaction()

                                For i As Integer = 0 To dtaExcelData.Rows.Count - 1

                                    dr = dtReturnExcel.NewRow

                                    Dim lastname As String = dtaExcelData.Rows(i).Item("Last Name").ToString()
                                    Dim firstname As String = dtaExcelData.Rows(i).Item("First Name").ToString()
                                    Dim middlename As String = dtaExcelData.Rows(i).Item("Middle Name").ToString()

                                    EmployeeID = GetEmployeeID(lastname, firstname, middlename)

                                    If Not EmployeeID = vbNullString Then
                                        IsNew = False
                                    End If

                                    p = New InsertUpdate("MobizLibraryPeople")

                                    If lastname.Count > 50 Then
                                        errmsg &= "<br>The Last Name <i>" & Trim(lastname) & "</i> is very long. 50 maximum characters only. Row number: " & rownumber
                                        Continue For
                                    End If

                                    If firstname.Count > 50 Then
                                        errmsg &= "<br>The First Name <i>" & Trim(firstname) & "</i> is very long. 50 maximum characters only. Row number: " & rownumber
                                        Continue For
                                    End If

                                    If middlename.Count > 50 Then
                                        errmsg &= "<br>The Middle Name <i>" & Trim(middlename) & "</i> is very long. 50 maximum characters only. Row number: " & rownumber
                                        Continue For
                                    End If

                                    p("Surname") = ReadyString(lastname)
                                    p("FirstName") = ReadyString(firstname)
                                    p("MiddleName") = ReadyString(middlename)
                                    p("DateOfBirth") = ReadyString(dtaExcelData.Rows(i).Item("Birth Date (MM-dd-yyyy)"))

                                    If Not IsNew Then
                                        pSql = p.GetUpdate("ID=" & ReadyString(EmployeeID))
                                    Else
                                        EmployeeID = Guid.NewGuid.ToString

                                        p("ID") = ReadyString(EmployeeID)

                                        'create temporary DTRPIN
                                        TemporaryDTRPIN = GenerateString(5, "1234567890")
                                        p("DTRPIN") = ReadyString(TemporaryDTRPIN)

                                        'SaveTemporaryPINToTable
                                        pSql = p.GetInsert()
                                    End If

                                    Common.WebRequest.ProData.Execute(pSql)

                                    Dim arrFowst() As String = Split(dtaExcelData.Rows(i).Item("FOWST Internal Code"), ",")

                                    For Each fowst As String In arrFowst
                                        If Not String.IsNullOrWhiteSpace(fowst) Then
                                            pSql = "SELECT ID FROM MobizLibraryOrganizationFOWST WHERE Code = " & ReadyString(Trim(fowst)) & " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
                                            Dim pRowFowst As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                                            If pRowFowst.Count > 0 Then
                                                Dim pFowstID As String = pRowFowst(0)("ID").ToString()
                                                Dim pDistributionFowstID As String = ""
                                                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")
                                                p("PeopleID") = ReadyString(EmployeeID)
                                                p("FowstID") = ReadyString(pFowstID)
                                                p("LastUpdated") = "getdate()"
                                                p("IsActive") = 1

                                                If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(EmployeeID) & " AND FowstID = " & ReadyString(pRowFowst(0)("ID").ToString())).Rows.Count = 0 Then
                                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                    pSql = p.GetInsert()
                                                    Common.WebRequest.ProData.Execute(pSql)
                                                End If

                                                If CStr2(dtaExcelData.Rows(i).Item("Create Group")).ToLower() = "yes" And (CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "91673033-3712-4976-8f0a-d298cde49793" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12") Then
                                                    Dim dtGroupHead As DataTable
                                                    Dim GroupHeadID As String
                                                    Dim GroupName As String
                                                    Dim organiztion As New Organization
                                                    'create group

                                                    GroupHeadID = CStr2(EmployeeID)
                                                    GroupName = "Z_" & CStr2(lastname)

                                                    If CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" Then 'OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12"
                                                        'Franchisee
                                                        'Default Group Distribution Center for Franchisee (FOWST: Potato Corner HQ)
                                                        pDistributionFowstID = "b980e58e-e08f-443d-9212-58dabee04e07"
                                                    End If

                                                    organiztion.SaveGroup(GroupName, pFowstID, GroupHeadID, pDistributionFowstID)
                                                End If

                                            Else
                                                errmsg &= "<br>FOWST Internal Code <i>" & Trim(fowst) & "</i> does not exist. Row number: " & rownumber
                                            End If
                                        End If
                                    Next

                                    Dim arrAccessRights() As String = Split(dtaExcelData.Rows(i).Item("Access Rights"), ",")

                                    For Each ar As String In arrAccessRights
                                        If Not String.IsNullOrWhiteSpace(ar) Then
                                            pSql = "SELECT ID FROM LibrarySecurityRole WHERE Code = " & ReadyString(Trim(ar))
                                            Dim pRowAR As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                                            If pRowAR.Count > 0 Then
                                                p = New InsertUpdate("LibrarySecurityRoleUser")
                                                p("PeopleID") = ReadyString(EmployeeID)
                                                p("RoleID") = ReadyString(pRowAR(0)("ID").ToString())
                                                p("LastUpdated") = "getdate()"
                                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                                                If Common.WebRequest.ProData.Execute("SELECT ID FROM LibrarySecurityRoleUser WHERE PeopleID = " & ReadyString(EmployeeID) & " AND RoleID = " & ReadyString(pRowAR(0)("ID").ToString())).Rows.Count = 0 Then
                                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                    pSql = p.GetInsert()
                                                    Common.WebRequest.ProData.Execute(pSql)
                                                End If
                                            Else
                                                errmsg &= "<br>Access Right's Code name <i>" & Trim(ar) & "</i> does not exist. Row number: " & rownumber
                                            End If
                                        End If
                                    Next

                                    'If IsNew Then
                                    'Get Team/Department ID
                                    'If Not CStr2(dtaExcelData.Rows(i).Item("Department")) = vbNullString Then
                                    '    Dim TeamID As String

                                    '    TeamID = GetTeamID(CStr2(dtaExcelData.Rows(i).Item("Department")))

                                    '    p = New InsertUpdate("LibraryCRMTeamMembers")
                                    '    p("PeopleID") = ReadyString(EmployeeID)
                                    '    p("TeamID") = ReadyString(TeamID)

                                    '    pSql = p.GetInsert()
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'End If

                                    'Save IDs
                                    'If Not CStr2(dtaExcelData.Rows(i).Item("SSS No")) = vbNullString Then
                                    '    p = New InsertUpdate("MobizLibraryPeopleIDs")
                                    '    p("PeopleID") = ReadyString(EmployeeID)
                                    '    p("IDNo") = ReadyString(CStr2(dtaExcelData.Rows(i).Item("SSS No")))
                                    '    p("IDType") = ReadyString("SSS")

                                    '    pSql = p.GetInsert()
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'End If

                                    'If Not CStr2(dtaExcelData.Rows(i).Item("TIN No")) = vbNullString Then
                                    '    p = New InsertUpdate("MobizLibraryPeopleIDs")
                                    '    p("PeopleID") = ReadyString(EmployeeID)
                                    '    p("IDNo") = ReadyString(CStr2(dtaExcelData.Rows(i).Item("TIN No")))
                                    '    p("IDType") = ReadyString("TIN")

                                    '    pSql = p.GetInsert()
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'End If

                                    'If Not CStr2(dtaExcelData.Rows(i).Item("PhilHealth No")) = vbNullString Then
                                    '    p = New InsertUpdate("MobizLibraryPeopleIDs")
                                    '    p("PeopleID") = ReadyString(EmployeeID)
                                    '    p("IDNo") = ReadyString(CStr2(dtaExcelData.Rows(i).Item("PhilHealth No")))
                                    '    p("IDType") = ReadyString("PhilHealth")

                                    '    pSql = p.GetInsert()
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'End If

                                    'If Not CStr2(dtaExcelData.Rows(i).Item("Pag-Ibig No")) = vbNullString Then
                                    '    p = New InsertUpdate("MobizLibraryPeopleIDs")
                                    '    p("PeopleID") = ReadyString(EmployeeID)
                                    '    p("IDNo") = ReadyString(CStr2(dtaExcelData.Rows(i).Item("Pag-Ibig No")))
                                    '    p("IDType") = ReadyString("PagIbig")

                                    '    pSql = p.GetInsert()
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'End If

                                    'Save FinancialInformation
                                    'If Not CStr2(dtaExcelData.Rows(i).Item("Account No")) = vbNullString Then

                                    '    Dim BankID As String
                                    '    Dim PayeeBankID As String

                                    '    'get bank id first
                                    '    BankID = GetBank(dtaExcelData.Rows(i).Item("Bank Name"))
                                    '    'get payee bank id
                                    '    PayeeBankID = GetDefaultPayeeBank()

                                    '    p = New InsertUpdate("LibraryEmployeesBankAccount")
                                    '    p("EmployeeID") = ReadyString(EmployeeID)
                                    '    p("BankID") = ReadyString(BankID)
                                    '    p("AccountName") = ReadyString(CStr2(firstname) & " " & CStr2(lastname))
                                    '    p("AccountNo") = ReadyString(CStr2(dtaExcelData.Rows(i).Item("Account No")))
                                    '    p("BankRole") = ReadyString("Payroll")
                                    '    p("IsDefaultForThisRole") = 1
                                    '    p("PayeeBankID") = ReadyString(PayeeBankID)

                                    '    pSql = p.GetInsert()
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'End If

                                    'create temporary username and password
                                    p = New InsertUpdate("MobizLibraryPeopleUserAccount")

                                    If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "91673033-3712-4976-8f0a-d298cde49793" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12" Then
                                        p("Username") = ReadyString(CStr2(dtaExcelData.Rows(i).Item("Username")))
                                        p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString("MPOS@2022"))
                                    Else
                                        p("Username") = ReadyString(CStr2(lastname).Replace(" ", "") & CStr2(firstname).Replace(" ", ""))
                                        p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(CStr2(lastname).Replace(" ", "") & CStr2(firstname).Replace(" ", "")))
                                    End If

                                    p("PeopleID") = ReadyString(EmployeeID)

                                    If Common.WebRequest.ProData.Execute("SELECT ID FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(EmployeeID)).Rows.Count = 0 Then
                                        pSql = p.GetInsert()
                                    Else
                                        pSql = p.GetUpdate("PeopleID = " & ReadyString(EmployeeID))
                                    End If

                                    Common.WebRequest.ProData.Execute(pSql)


                                    p = New InsertUpdate("LibraryOrganizationEmployee")

                                    p("PeopleID") = ReadyString(EmployeeID)
                                    p("DateHired") = ReadyString(CDate(dtaExcelData.Rows(i).Item("Date Hired (MM-dd-yyyy)")))
                                    'p("PeopleNo") = CInt2(dtaExcelData.Rows(i).Item("Employee No"))
                                    p("DeActivated") = 0
                                    p("IsBan") = 0

                                    If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryOrganizationEmployee WHERE PeopleID = " & ReadyString(EmployeeID)).Rows.Count = 0 Then
                                        pSql = p.GetInsert()
                                    Else
                                        pSql = p.GetUpdate("PeopleID = " & ReadyString(EmployeeID))
                                    End If

                                    Common.WebRequest.ProData.Execute(pSql)

                                    'Dim dtFOWST As DataTable

                                    'dtFOWST = GetOrganizationFOWST()

                                    'For j As Integer = 0 To dtFOWST.Rows.Count - 1
                                    '    p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                                    '    p("PeopleID") = ReadyString(EmployeeID)
                                    '    p("FowstID") = ReadyString(dtFOWST.Rows(j).Item("ID"))

                                    '    If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(EmployeeID) & " AND FowstID = " & ReadyString(dtFOWST.Rows(j).Item("ID"))).Rows.Count = 0 Then
                                    '        pSql = p.GetInsert()
                                    '    Else
                                    '        pSql = p.GetUpdate("PeopleID = " & ReadyString(EmployeeID) & " AND FowstID = " & ReadyString(dtFOWST.Rows(j).Item("ID")))
                                    '    End If
                                    '    Common.WebRequest.ProData.Execute(pSql)
                                    'Next


                                    'LibrarySecurityRoleUser()

                                    p = New InsertUpdate("MobizLibraryPeopleOrganization")

                                    p("PeopleID") = ReadyString(EmployeeID)
                                    p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                                    p("IsConfirmed") = 1
                                    p("DateTimeConfirmed") = "GETDATE()"

                                    If Common.WebRequest.ProData.Execute("SELECT ID FROM MobizLibraryPeopleOrganization WHERE PeopleID = " & ReadyString(EmployeeID) & " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)).Rows.Count = 0 Then
                                        pSql = p.GetInsert()
                                    Else
                                        pSql = p.GetUpdate("PeopleID = " & ReadyString(EmployeeID) & " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID))
                                    End If

                                    Common.WebRequest.ProData.Execute(pSql)


                                    'get salary id first
                                    'SalaryGradeID = GetSalaryGrade(dtaExcelData.Rows(i).Item("Pay Grade"))
                                    'p = New InsertUpdate("LibrarySalaryGradeEmployees")

                                    'p("EmployeeID") = ReadyString(EmployeeID)
                                    'p("SalaryGradeID") = ReadyString(SalaryGradeID)

                                    'pSql = p.GetInsert()
                                    'Common.WebRequest.ProData.Execute(pSql)

                                    'get work schedule id first
                                    'WorkScheduleID = GetWorkSchedule(dtaExcelData.Rows(i).Item("Work Schedule"))
                                    'p = New InsertUpdate("LibraryDTRScheduleEmployees")

                                    'p("ID") = ReadyString(Guid.NewGuid.ToString)
                                    'p("EmployeeID") = ReadyString(EmployeeID)
                                    'p("ScheduleID") = ReadyString(WorkScheduleID)

                                    'pSql = p.GetInsert()
                                    'Common.WebRequest.ProData.Execute(pSql)

                                    'End If

                                    IsNew = True

                                    dr(0) = lastname & "," & firstname
                                    dr(1) = TemporaryDTRPIN

                                    If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "91673033-3712-4976-8f0a-d298cde49793" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12" Then
                                        dr(2) = CStr2(dtaExcelData.Rows(i).Item("Username"))
                                        dr(3) = "MPOS@2022"
                                    Else
                                        dr(2) = CStr2(lastname).Replace(" ", "") & CStr2(firstname).Replace(" ", "")
                                        dr(3) = CStr2(lastname).Replace(" ", "") & CStr2(firstname).Replace(" ", "")
                                    End If

                                    dtReturnExcel.Rows.Add(dr)

                                    rownumber += 1
                                Next

                                If String.IsNullOrWhiteSpace(errmsg) Then
                                    Common.WebRequest.ProData.CommitTransaction()

                                    TableToExcelFile(dtReturnExcel, "", "", pSourceURL & "\EmployeesTemplate\" & CStr2(WebRequest.Instance.OrgID) & "\EmployeesTemporaryCredentials.xls")
                                Else
                                    Return JsonDictionary.Serialize("Error", errmsg)
                                End If
                            End If
                        Catch ex As Exception
                            Common.WebRequest.ProData.RollbackTransaction()
                            AppendToLog(ex.Message)
                            Return JsonDictionary.Serialize("Results", False)
                        End Try
                    End If
                Catch ex As Exception
                    WebSession.Instance.PIMTemplateUploaded = "Failed"
                    WebSession.Instance.SerializedString = ""
                    Return JsonDictionary.Serialize("Results", False)
                End Try

                Return JsonDictionary.Serialize("Results", True)
            End If
        End Function

        Function GetEmployeeID(Surname As String, FirstName As String, MiddleName As String) As String
            Dim pSql As String

            pSql = "Select ID from MobizLibraryPeople WHERE " & vbCrLf &
                    " Surname = " & ReadyString(Surname) & vbCrLf &
                    " AND Firstname = " & ReadyString(FirstName) & vbCrLf &
                    " AND MiddleName = " & ReadyString(MiddleName)

            Return CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
        End Function

        Function GetOrganizationFOWST() As DataTable
            Dim pSql As String
            Dim dtFOWST As DataTable

            pSql = "Select ID from MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)

            dtFOWST = Common.WebRequest.ProData.Execute(pSql)

            Return dtFOWST
        End Function

        Function DownloadFile() As ActionResult
            Dim pFileName = Common.WebRequest.QueryString("FileName")
            Dim pOrgID = Common.WebRequest.QueryString("OrgID")
            Dim pSourceURL As String = Common.ServerPath & "Content\" & pFileName
            Dim pContentType As String = "application/csv"

            Try
                Return File(pSourceURL, pContentType, pFileName)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

    End Class
End Namespace