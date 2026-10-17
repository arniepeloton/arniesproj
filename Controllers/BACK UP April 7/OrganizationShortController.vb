Namespace WebProj
    Public Class OrganizationShortController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationAddress() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationRepresentative() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationSummary() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.OrganizationID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("OrganizationID", WebSession.Instance.OrganizationID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckOrgUniqueOrgName() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrganizationID")
            clsOrganization.CompanyName = Common.WebRequest.Data("CompanyName")

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.CheckOrgUniqueOrgName())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPersonUniqueName() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String
            Dim isFull As Boolean
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrganizationID")
            clsOrganization.PersonTitleID = Common.WebRequest.Data("PersonTitleID")
            clsOrganization.PersonSurname = Common.WebRequest.Data("PersonSurname")
            clsOrganization.PersonFirstName = Common.WebRequest.Data("PersonFirstName")
            clsOrganization.PersonMiddleName = Common.WebRequest.Data("PersonMiddleName")
            clsOrganization.PersonSuffixName = Common.WebRequest.Data("PersonSuffixName")
            clsOrganization.PersonDOB = Common.WebRequest.Data("PersonDOB")
            clsOrganization.PersonMotherMaidenSurname = Common.WebRequest.Data("PersonMotherMaidenSurname")
            clsOrganization.PersonMotherMaidenFirstName = Common.WebRequest.Data("PersonMotherMaidenFirstName")
            clsOrganization.PersonMotherMaidenMiddleName = Common.WebRequest.Data("PersonMotherMaidenMiddleName")
            If (Common.WebRequest.Data("IsFull") Is Nothing) Then
                isFull = True
            Else
                isFull = Convert.ToBoolean(Common.WebRequest.Data("IsFull"))
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.CheckPersonUniqueName(isFull))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationList() As String
            Dim strKeyWord As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrganizationList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgTradeNameList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgTradeNameList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgCategoryList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgCategoryList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationInfo() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrganizationID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrganizationInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationInfo() As String
            Dim clsOrganizationFull As New Organization
            Dim clsOrganization As New OrganizationShort
            Dim strOrgCustID As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("IsOrganization") = "0") Then
                If (Common.WebRequest.Data("PersonDOB").Trim().Length > 0 And Common.WebRequest.Data("PersonDOB").Trim() <> "//") Then
                    If (Not IsDate(Common.WebRequest.Data("PersonDOB"))) Then
                        strReturn = JsonDictionary.Serialize("OrganizationData", "PersonDOBError")
                        Return strReturn
                    End If
                End If
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrganizationID")

            clsOrganization.IsOrganization = Common.WebRequest.Data("IsOrganization")
            clsOrganization.PersonTitleID = Common.WebRequest.Data("PersonTitleID")
            clsOrganization.PersonSurname = Common.WebRequest.Data("PersonSurname")
            clsOrganization.PersonFirstName = Common.WebRequest.Data("PersonFirstName")
            clsOrganization.PersonMiddleName = Common.WebRequest.Data("PersonMiddleName")
            clsOrganization.PersonSuffixName = Common.WebRequest.Data("PersonSuffixName")
            clsOrganization.PersonDOB = Common.WebRequest.Data("PersonDOB")
            clsOrganization.PersonMotherMaidenSurname = Common.WebRequest.Data("PersonMotherMaidenSurname")
            clsOrganization.PersonMotherMaidenFirstName = Common.WebRequest.Data("PersonMotherMaidenFirstName")
            clsOrganization.PersonMotherMaidenMiddleName = Common.WebRequest.Data("PersonMotherMaidenMiddleName")

            'additional july 16

            clsOrganization.PersonSeniorCitizensIDNo = Common.WebRequest.Data("PersonSeniorCitizensIDNo")
            clsOrganization.PersonPWDIDNo = Common.WebRequest.Data("PersonPWDIDNo")
            clsOrganization.PersonPassportNo = Common.WebRequest.Data("PersonPassportNo")
            clsOrganization.PersonDriversLicenseNo = Common.WebRequest.Data("PersonDriversLicenseNo")
            clsOrganization.PersonSSSNo = Common.WebRequest.Data("PersonSSSNo")
            clsOrganization.PersonGSISIDNo = Common.WebRequest.Data("PersonGSISIDNo")
            clsOrganization.PersonPRCIDNo = Common.WebRequest.Data("PersonPRCIDNo")
            clsOrganization.PersonPostalIDNo = Common.WebRequest.Data("PersonPostalIDNo")
            clsOrganization.PersonVotersIDNo = Common.WebRequest.Data("PersonVotersIDNo")
            clsOrganization.PersonOtherIDType = Common.WebRequest.Data("PersonOtherIDType")
            clsOrganization.PersonOtherIDNo = Common.WebRequest.Data("PersonOtherIDNo")
            ' end of additional


            clsOrganization.CompanyName = Common.WebRequest.Data("CompanyName")
            clsOrganization.CompanyNameInitial = Common.WebRequest.Data("CompanyNameInitial")
            clsOrganization.CorporateTIN = Common.WebRequest.Data("TIN")
            clsOrganization.OwnershipType = Common.WebRequest.Data("OwnershipType")
            clsOrganization.IsFranchisor = Common.WebRequest.Data("IsFranchisor")

            clsOrganization.OfficeAddressBuildingNo = Common.WebRequest.Data("OfficeAddressBuildingNo")
            clsOrganization.OfficeAddressStreet = Common.WebRequest.Data("OfficeAddressStreet")
            clsOrganization.OfficeAddressCityID = Common.WebRequest.Data("OfficeAddressCityID")
            clsOrganization.OfficeAddressCountryID = Common.WebRequest.Data("OfficeAddressCountryID")

            clsOrganization.BusinessAddressBuildingNo = Common.WebRequest.Data("BusinessAddressBuildingNo")
            clsOrganization.BusinessAddressStreet = Common.WebRequest.Data("BusinessAddressStreet")
            clsOrganization.BusinessAddressCityID = Common.WebRequest.Data("BusinessAddressCityID")
            clsOrganization.BusinessAddressCountryID = Common.WebRequest.Data("BusinessAddressCountryID")

            clsOrganization.OrgTradeNameList = Common.WebRequest.AllData("OrgTradeNameList")
            clsOrganization.OrgCategoryList = Common.WebRequest.AllData("OrgCategoryList")

            clsOrganization.IsAddAsCustomer = Common.WebRequest.Data("IsAddAsCustomer")

            WebSession.Instance.OrganizationID = clsOrganization.SaveOrganizationInfo()
            strReturn = JsonDictionary.Serialize("OrganizationData", WebSession.Instance.OrganizationID)

            '''Auto Add as Organization Customer
            If (clsOrganization.IsAddAsCustomer = "1") Then

                clsOrganizationFull.ID = ""
                clsOrganizationFull.CustomerID = WebSession.Instance.OrganizationID
                clsOrganizationFull.ActivationDate = DateTime.Today.ToString("MM/dd/yyyy")
                clsOrganizationFull.IsDeactivated = "0"
                clsOrganizationFull.DeactivationDate = ""
                clsOrganizationFull.DeactivationReason = ""
                clsOrganizationFull.Comments = ""
                clsOrganizationFull.IsBan = "0"

                strOrgCustID = JsonDictionary.Serialize("OrgCustomerData", clsOrganizationFull.SaveOrganizationCustomer())
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTLTOList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTLTOList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTLTFRBList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTLTFRBList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTSectionList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTSectionList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTBPermitList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTBPermitList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTTelList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTTelList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTMobileList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTMobileList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTEmailList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTEmailList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTInfo() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgFOWST() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFOWSTID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.FOWSTTypeID = Common.WebRequest.Data("FOWSTTypeID")
            clsOrganization.FOWSTCode = Common.WebRequest.Data("FOWSTCode")
            clsOrganization.FOWSTName = Common.WebRequest.Data("FOWSTName")
            clsOrganization.FOWSTDescription = Common.WebRequest.Data("FOWSTDescription")

            clsOrganization.FOWSTStreetNumber = Common.WebRequest.Data("FOWSTStreetNumber")
            clsOrganization.FOWSTStreet = Common.WebRequest.Data("FOWSTStreet")
            clsOrganization.FOWSTCityID = Common.WebRequest.Data("FOWSTCityID")
            clsOrganization.FOWSTCountryID = Common.WebRequest.Data("FOWSTCountryID")

            clsOrganization.FOWSTSubTypeID = Common.WebRequest.Data("FOWSTSubTypeID")
            clsOrganization.FOWSTPlateNo = Common.WebRequest.Data("FOWSTPlateNo")
            clsOrganization.FOWSTNoOfWheels = Common.WebRequest.Data("FOWSTNoOfWheels")
            clsOrganization.FOWSTBodyColor = Common.WebRequest.Data("FOWSTBodyColor")
            clsOrganization.FOWSTWeight = Common.WebRequest.Data("FOWSTWeight")
            clsOrganization.FOWSTDimension = Common.WebRequest.Data("FOWSTDimension")

            clsOrganization.FOWSTOwnershipType = Common.WebRequest.Data("FOWSTOwnershipType")
            clsOrganization.FOWSTContractStart = Common.WebRequest.Data("FOWSTContractStart")
            clsOrganization.FOWSTContractEnd = Common.WebRequest.Data("FOWSTContractEnd")
            clsOrganization.FOWSTCostPerMonth = Common.WebRequest.Data("FOWSTCostPerMonth")
            clsOrganization.FOWSTIsMortgaged = Common.WebRequest.Data("FOWSTIsMortgaged")

            clsOrganization.FOWSTIsFranchisee = Common.WebRequest.Data("FOWSTIsFranchisee")
            clsOrganization.FOWSTFranchisorID = Common.WebRequest.Data("FOWSTFranchisorID")

            clsOrganization.OrgFOWSTLTOList = Common.WebRequest.AllData("OrgFOWSTLTOList")
            clsOrganization.OrgFOWSTLTFRBList = Common.WebRequest.AllData("OrgFOWSTLTFRBList")
            clsOrganization.OrgFOWSTSectionList = Common.WebRequest.AllData("OrgFOWSTSectionList")
            clsOrganization.OrgFOWSTBPermitList = Common.WebRequest.AllData("OrgFOWSTBPermitList")
            clsOrganization.OrgFOWSTTelList = Common.WebRequest.AllData("OrgFOWSTTelList")
            clsOrganization.OrgFOWSTMobileList = Common.WebRequest.AllData("OrgFOWSTMobileList")
            clsOrganization.OrgFOWSTEmailList = Common.WebRequest.AllData("OrgFOWSTEmailList")

            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.SaveOrgFOWST())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepContactTelList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrganizationID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepContactTelList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepContactMobileList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrganizationID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepContactMobileList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepContactEmailList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrganizationID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepContactEmailList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepInfo() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgRep() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgRepID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.LastName = Common.WebRequest.Data("LastName")
            clsOrganization.FirstName = Common.WebRequest.Data("FirstName")
            clsOrganization.MiddleName = Common.WebRequest.Data("MiddleName")
            clsOrganization.Title = Common.WebRequest.Data("Title")
            clsOrganization.GenerationalSuffix = Common.WebRequest.Data("GenerationalSuffix")
            clsOrganization.IsActive = Common.WebRequest.Data("IsActive")

            clsOrganization.OrgRepContactTelList = Common.WebRequest.AllData("OrgRepContactTelList")
            clsOrganization.OrgRepContactMobileList = Common.WebRequest.AllData("OrgRepContactMobileList")
            clsOrganization.OrgRepContactEmailList = Common.WebRequest.AllData("OrgRepContactEmailList")

            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.SaveOrgRep())

            Return strReturn

        End Function

        <HttpPost()>
        Function SetInActiveOrgRep() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgRepID")

            strReturn = JsonDictionary.Serialize("OrganizationRepData", IIf(clsOrganization.SetInActiveOrgRep(), "TRUE", "FALSE"))

            Return strReturn

        End Function

    End Class

End Namespace