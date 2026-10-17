Imports Aspose.Cells

Namespace WebProj

    Public Class OrganizationController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company Profile")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationSummary() As ActionResult
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

        Function OrganizationFinancialReport() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationContact() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationAddress() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company Places (FOWST)")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationRepresentative() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationOwner() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationAffiliation() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationEmployee() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company Employee")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationCustomer() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company Customer")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationSupplier() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company Supplier")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationProduct() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company-Wide Product List")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationFOWSTProduct() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Company-Wide Product List")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationFOWSTProductList() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("FOWST Level Product List")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationCustomerProduct() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company Wide Product List")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationCustomerFOWST() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            '' ''If (Not SecurityRole.Instance.HandleRoleAccess("Company Wide Product List")) Then Exit Function

            Return View()
        End Function

        Function OrganizationCustomerFOWSTProductList() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            '' ''If (Not SecurityRole.Instance.HandleRoleAccess("Company Wide Product List")) Then Exit Function

            Return View()
        End Function

        Function OrganizationBrands() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            '' ''If (Not SecurityRole.Instance.HandleRoleAccess("Company Wide Product List")) Then Exit Function

            Return View()
        End Function

        Function DownloadCompanyWideProductList() As ActionResult
            Dim clsOrganization As New Organization
            Dim pDT As DataTable
            Dim pFS As IO.FileStream
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim intCntr As String
            Dim intRowCntr As String

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            'End If
            If WebSession.Instance.PeopleUserID Is Nothing Then Exit Function
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            If Not System.IO.Directory.Exists(Common.ServerPath & "/Content/PIM") Then
                System.IO.Directory.CreateDirectory(Common.ServerPath & "/Content/PIM")
            End If

            pFS = New IO.FileStream(Common.ServerPath & "/Content/PIM/CompanyWideProductList.xls", IO.FileMode.Open)
            pWB = New Workbook
            pWB.Open(pFS)
            pWS = pWB.Worksheets(0)

            '''get List of Product
            pDT = New DataTable()
            clsOrganization.MaxReturnRow = 9999999
            pDT = clsOrganization.getOrganizationProductList("")
            If (pDT.Rows.Count > 0) Then


                If (pDT.Rows.Count > 0) Then
                    intCntr = 0
                    intRowCntr = 1
                    While (intCntr < pDT.Rows.Count)

                        Dim objTest As New ArrayList()
                        objTest.Add(pDT.Rows(intCntr).ItemArray(2))
                        objTest.Add(pDT.Rows(intCntr).ItemArray(3))

                        If (pDT.Rows(intCntr).ItemArray(5).ToString().Length > 0) Then
                            objTest.Add(Convert.ToDateTime(pDT.Rows(intCntr).ItemArray(5)).ToString("MM/dd/yyyy"))
                        Else
                            objTest.Add(pDT.Rows(intCntr).ItemArray(5))
                        End If

                        If (pDT.Rows(intCntr).ItemArray(4).ToString().Length > 0) Then
                            objTest.Add(IIf(pDT.Rows(intCntr).ItemArray(4).ToString() = "0", "No", "Yes"))
                        Else
                            objTest.Add(pDT.Rows(intCntr).ItemArray(4))
                        End If

                        pWS.Cells.ImportArrayList(objTest, intRowCntr, 0, False)

                        intCntr = intCntr + 1
                        intRowCntr = intRowCntr + 1

                    End While

                End If

            End If

            pWB.Save(Common.ServerPath & "temp\PIMTemplate\CompanyWideProductList.xls")

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try

            '''Download the File
            Dim pContentType As String
            Dim pSourceURL As String
            pSourceURL = Common.ServerPath & "temp\PIMTemplate\CompanyWideProductList.xls"
            pContentType = "application/xls"

            Return File(pSourceURL, pContentType, "CompanyWideProductList.xls")

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
        Function getSubscriberOrganization() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("OrganizationID", WebRequest.Instance.OrgID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckOrgUniqueOrgName() As String
            Dim clsOrganization As New Organization
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

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.CheckPersonUniqueName(True))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationList() As String
            Dim strKeyWord As String
            Dim clsOrganization As New Organization
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
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = IIf(WebSession.Instance.OrganizationID = Nothing, WebRequest.Instance.OrgID, WebSession.Instance.OrganizationID)
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgTradeNameList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgCategoryList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgCategoryList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationIndustryClassList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgIndustryClassificationList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFranchiseeList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgFranchiseeList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrganizationID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrganizationInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationInfo() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("IsOrganization") = "1") Then
                If (Not IsDate(Common.WebRequest.Data("RegistrationDate"))) Then
                    strReturn = JsonDictionary.Serialize("OrganizationData", "RegistrationDateError")
                    Return strReturn
                End If
            Else
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

            clsOrganization.CompanyName = Common.WebRequest.Data("CompanyName")
            clsOrganization.CompanyNameInitial = Common.WebRequest.Data("CompanyNameInitial")

            clsOrganization.OwnershipType = Common.WebRequest.Data("OwnershipType")
            clsOrganization.IsFranchisor = Common.WebRequest.Data("IsFranchisor")
            clsOrganization.RegistrationType = Common.WebRequest.Data("RegistrationType")
            clsOrganization.RegistrationNo = Common.WebRequest.Data("RegistrationNo")
            clsOrganization.RegistrationDate = Common.WebRequest.Data("RegistrationDate")
            clsOrganization.CorporateTIN = Common.WebRequest.Data("TIN")

            clsOrganization.OfficeAddressBuildingNo = Common.WebRequest.Data("OfficeAddressBuildingNo")
            clsOrganization.OfficeAddressStreet = Common.WebRequest.Data("OfficeAddressStreet")
            clsOrganization.OfficeAddressCityID = Common.WebRequest.Data("OfficeAddressCityID")
            clsOrganization.OfficeAddressCountryID = Common.WebRequest.Data("OfficeAddressCountryID")

            clsOrganization.BusinessAddressBuildingNo = Common.WebRequest.Data("BusinessAddressBuildingNo")
            clsOrganization.BusinessAddressStreet = Common.WebRequest.Data("BusinessAddressStreet")
            clsOrganization.BusinessAddressCityID = Common.WebRequest.Data("BusinessAddressCityID")
            clsOrganization.BusinessAddressCountryID = Common.WebRequest.Data("BusinessAddressCountryID")

            clsOrganization.Purpose = Common.WebRequest.Data("Purpose")
            clsOrganization.BusinessScope = Common.WebRequest.Data("BusinessScope")
            clsOrganization.NoOfRetailOutlet = If(Common.WebRequest.Data("NoOfRetailOutlet").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfRetailOutlet"), 0)
            clsOrganization.NoOfOffice = If(Common.WebRequest.Data("NoOfOffice").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfOffice"), 0)
            clsOrganization.NoOfWarehouse = If(Common.WebRequest.Data("NoOfWarehouse").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfWarehouse"), 0)
            clsOrganization.NoOfEmployees = If(Common.WebRequest.Data("NoOfEmployees").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfEmployees"), 0)

            clsOrganization.OrgTradeNameList = Common.WebRequest.AllData("OrgTradeNameList")
            clsOrganization.OrgCategoryList = Common.WebRequest.AllData("OrgCategoryList")
            clsOrganization.OrgIndustryClassificationList = Common.WebRequest.AllData("OrgIndustryClassificationList")
            'clsOrganization.OrgFranchiseeList = Common.WebRequest.AllData("OrgFranchiseeList")

        

            WebSession.Instance.OrganizationID = clsOrganization.SaveOrganizationInfo()

            'send email to mobiz
            If Common.WebRequest.Data("IsFranchisor") = 1 Then
                Dim pSql As String
                Dim pDT As DataTable
                pSql = "SELECT ID FROM MobizLibraryOrganization WHERE IsMobizFranchisorRequestSent = 0 AND ID = " & ReadyString(WebSession.Instance.OrganizationID)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    'send email to admin for notification
                    Dim strBody As String

                    strBody = "Organization is requesting to be a Franchisor. Please confirm. <br/> Company: " & Common.WebRequest.Data("CompanyName") &
                        " <br/> TIN: " & Common.WebRequest.Data("TIN") &
                        " <br/> Mobiz ID: " & WebSession.Instance.OrganizationID

#If DEBUG Then
                    SendMail("customercare@mobiz-apps.com",
                         "janine.son@mobiz-apps.com",
                         "Mobiz",
                         "Franchisor Request",
                         strBody,
                         "smtp.live.com",
                         587,
                         "customercare@mobiz-apps.com",
                         "m0b!zp30pl30nly!", , , , , True)


#Else
                 SendMail("customercare@mobiz-apps.com",
                     "janine.son@mobiz-apps.com, robert.chong@mobiz-apps.com",
                     "Mobiz",
                     "New Mobiz Customer",
                     strBody,
                     "smtp.live.com",
                     587,
                     "customercare@mobiz-apps.com",
                     "m0b!zp30pl30nly!", , , , , True)


#End If


                    pSql = "UPDATE MobizLibraryOrganization SET IsMobizFranchisorRequestSent = 1 WHERE ID = " & ReadyString(WebSession.Instance.OrganizationID)
                    Common.WebRequest.ProData.Execute(pSql)


                End If
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", WebSession.Instance.OrganizationID)

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFinancialReportInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgFinancialReportInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgFinancialReport() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("FinancialReportID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.FiscalMonthEnd = Common.WebRequest.Data("FiscalMonthEnd")
            clsOrganization.FiscalDayEnd = Common.WebRequest.Data("FiscalDayEnd")
            clsOrganization.NameOfExternalAuditor = Common.WebRequest.Data("NameOfExternalAuditor")
            clsOrganization.SingningPartnerOfExternalAuditor = Common.WebRequest.Data("SingningPartnerOfExternalAuditor")
            clsOrganization.SECAccreditationNoOfExternalAuditor = Common.WebRequest.Data("SECAccreditationNoOfExternalAuditor")
            clsOrganization.VATType = Common.WebRequest.Data("VATType")
            clsOrganization.BIRTaxGroup = Common.WebRequest.Data("BIRTaxGroup")

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.SaveOrgFinancialReport())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactTelList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactTelList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactTelInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactTelInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactTel() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.ContactCountryID = Common.WebRequest.Data("ContactCountryID")
            clsOrganization.ContactAreaCodeID = Common.WebRequest.Data("ContactAreaCodeID")
            clsOrganization.ContactNo = Common.WebRequest.Data("ContacNo")
            clsOrganization.ContactExtensionNo = Common.WebRequest.Data("ContacExtensionNo")

            If (clsOrganization.ContactNo.Trim().Length > 0 And Not IsNumeric(clsOrganization.ContactNo)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Tel. No.")
                Return strReturn
            End If

            If (clsOrganization.ContactExtensionNo.Trim().Length > 0 And Not IsNumeric(clsOrganization.ContactExtensionNo)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Extension or Local No.")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.SaveOrgContactTel())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgContactTel() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")

            strReturn = JsonDictionary.Serialize("OrganizationData", IIf(clsOrganization.RemoveOrgContactTel(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactFaxList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactFaxList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactFaxInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactFaxInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactFax() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.ContactCountryID = Common.WebRequest.Data("ContactCountryID")
            clsOrganization.ContactAreaCodeID = Common.WebRequest.Data("ContactAreaCodeID")
            clsOrganization.ContactNo = Common.WebRequest.Data("ContacNo")
            clsOrganization.ContactExtensionNo = Common.WebRequest.Data("ContacExtensionNo")

            If (clsOrganization.ContactNo.Trim().Length > 0 And Not IsNumeric(clsOrganization.ContactNo)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Tel. No.")
                Return strReturn
            End If

            If (clsOrganization.ContactExtensionNo.Trim().Length > 0 And Not IsNumeric(clsOrganization.ContactExtensionNo)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Extension or Local No.")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.SaveOrgContactFax())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgContactFax() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")

            strReturn = JsonDictionary.Serialize("OrganizationData", IIf(clsOrganization.RemoveOrgContactFax(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactMobileList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactMobileList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactMobileInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactMobileInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactMobile() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.ContactCountryID = Common.WebRequest.Data("ContactCountryID")
            clsOrganization.ContactTelCoPrefixID = Common.WebRequest.Data("ContactTelCoPrefixID")
            clsOrganization.ContactNo = Common.WebRequest.Data("ContactNo")

            If (clsOrganization.ContactNo.Trim().Length > 0 And Not IsNumeric(clsOrganization.ContactNo)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Mobile No.")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.SaveOrgContactMobile())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgContactMobile() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")

            strReturn = JsonDictionary.Serialize("OrganizationData", IIf(clsOrganization.RemoveOrgContactMobile(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactEmailList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactEmailList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactEmailInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactEmailInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactEmail() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.ContactEmail = Common.WebRequest.Data("ContactEmail")

            If (clsOrganization.ContactEmail.Trim().Length > 0 And Not IsEmail(clsOrganization.ContactEmail)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Email")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.SaveOrgContactEmail())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgContactEmail() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")

            strReturn = JsonDictionary.Serialize("OrganizationData", IIf(clsOrganization.RemoveOrgContactEmail(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactWebsiteList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactWebsiteList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgContactWebsiteInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactWebsiteInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactWebsite() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.ContactWebsite = Common.WebRequest.Data("ContactWebsite")

            If (clsOrganization.ContactWebsite.Trim().Length > 0 And Not IsUrl(clsOrganization.ContactWebsite)) Then
                strReturn = JsonDictionary.Serialize("OrganizationData", "Invalid Website")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.SaveOrgContactWebsite())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgContactWebsite() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgContactID")

            strReturn = JsonDictionary.Serialize("OrganizationData", IIf(clsOrganization.RemoveOrgContactWebsite(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTList() As String
            Dim strID As String
            Dim SearchKeyword As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            SearchKeyword = Common.WebRequest.Data("SearchKeyword")

            If (Common.WebRequest.Data("OrganizationID") <> Nothing) Then
                strID = Common.WebRequest.Data("OrganizationID")
            End If

            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTList(strID, SearchKeyword))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTPrinterIP() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebRequest.Instance.FowstID
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTPrinterIP(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTLTOList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTLTOList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTLTFRBList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTLTFRBList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTSectionList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTSectionList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTBPermitList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTBPermitList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTTelList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTTelList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTMobileList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTMobileList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTEmailList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTEmailList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTOperationHRList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgFOWSTID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTOperationHRList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgFOWSTInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgFOWST() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFOWSTID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.FOWSTTypeID = Common.WebRequest.Data("FOWSTTypeID")
            clsOrganization.FOWSTCode = Common.WebRequest.Data("FOWSTCode")
            clsOrganization.FOWSTName = Common.WebRequest.Data("FOWSTName")
            clsOrganization.FOWSTDescription = Common.WebRequest.Data("FOWSTDescription")
            clsOrganization.FOWSTBusinessTradeNameID = Common.WebRequest.Data("FOWSTBusinessTradeNameID")
            clsOrganization.FOWSTTINBranchCode = Common.WebRequest.Data("FOWSTTINBranchCode")

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

            clsOrganization.FOWSTIsInsideAMall = Common.WebRequest.Data("FOWSTIsInsideAMall")
            clsOrganization.FOWSTMallName = Common.WebRequest.Data("FOWSTMallName")

            clsOrganization.FOWSTAyalaRecID = Common.WebRequest.Data("FOWSTAyalaRecID")
            clsOrganization.FOWSTAyalaTenantCode = Common.WebRequest.Data("FOWSTAyalaTenantCode")
            clsOrganization.FOWSTAyalaTenantName = Common.WebRequest.Data("FOWSTAyalaTenantName")

            clsOrganization.FOWSTSMRecID = Common.WebRequest.Data("FOWSTSMRecID")
            clsOrganization.FOWSTSMMallCode = Common.WebRequest.Data("FOWSTSMMallCode")
            clsOrganization.FOWSTSMMallTenantNo = Common.WebRequest.Data("FOWSTSMMallTenantNo")
            clsOrganization.FOWSTSMMallClassCode = Common.WebRequest.Data("FOWSTSMMallClassCode")
            clsOrganization.FOWSTSMMallTradeCode = Common.WebRequest.Data("FOWSTSMMallTradeCode")
            clsOrganization.FOWSTSMMallOutletNo = Common.WebRequest.Data("FOWSTSMMallOutletNo")
            clsOrganization.FOWSTSMMallSaleType = Common.WebRequest.Data("FOWSTSMMallSaleType")

            clsOrganization.FOWSTIsIncludeReport = CInt(Common.WebRequest.Data("FOWSTIsIncludeToReport"))
            clsOrganization.FOWSTAccountID = Common.WebRequest.Data("FOWSTAccountID")
            clsOrganization.FOWSTStoreID = Common.WebRequest.Data("FOWSTStoreID")

            clsOrganization.FOWSTIsIncludeToShopperTrakReport = CInt(Common.WebRequest.Data("FOWSTIsIncludeToShopperTrakReport"))
            clsOrganization.FOWSTShopperTrakAccountID = Common.WebRequest.Data("FOWSTShopperTrakAccountID")

            clsOrganization.OrgFOWSTLTOList = Common.WebRequest.AllData("OrgFOWSTLTOList")
            clsOrganization.OrgFOWSTLTFRBList = Common.WebRequest.AllData("OrgFOWSTLTFRBList")
            clsOrganization.OrgFOWSTSectionList = Common.WebRequest.AllData("OrgFOWSTSectionList")
            clsOrganization.OrgFOWSTBPermitList = Common.WebRequest.AllData("OrgFOWSTBPermitList")
            clsOrganization.OrgFOWSTTelList = Common.WebRequest.AllData("OrgFOWSTTelList")
            clsOrganization.OrgFOWSTMobileList = Common.WebRequest.AllData("OrgFOWSTMobileList")
            clsOrganization.OrgFOWSTEmailList = Common.WebRequest.AllData("OrgFOWSTEmailList")
            clsOrganization.OrgFOWSTOperationHRList = Common.WebRequest.AllData("OrgFOWSTOperationHRList")
            clsOrganization.OrgFOWSTPrinterList = Common.WebRequest.AllData("OrgFOWSTPrinterList")

            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.SaveOrgFOWST())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
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
            Dim clsOrganization As New Organization
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
            Dim clsOrganization As New Organization
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
            Dim clsOrganization As New Organization
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
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgRep() As String
            Dim clsOrganization As New Organization
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
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgRepID")

            strReturn = JsonDictionary.Serialize("OrganizationRepData", IIf(clsOrganization.SetInActiveOrgRep(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationEmployee() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgEmployeeID")
            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")

            strReturn = JsonDictionary.Serialize("OrgEmployeeData", clsOrganization.CheckOrganizationEmployee())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationEmpList() As String
            Dim clsOrganization As New Organization
            Dim strSearchString As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
                strReturn = JsonDictionary.Serialize("OrgEmployeeData", clsOrganization.getOrganizationEmpList())
            Else
                strSearchString = Common.WebRequest.Data("SearchKeyword")
                strReturn = JsonDictionary.Serialize("OrgEmployeeData", clsOrganization.getOrganizationEmpList(strSearchString))
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationEmpInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgEmployeeData", clsOrganization.getOrganizationEmpInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetEmployeeFowst() As String
            Dim clsOrganization As New Organization

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeFowst())

        End Function

        <HttpPost()>
        Function GetEmployeeSecurityRole() As String
            Dim clsOrganization As New Organization

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeSecurityRole())

        End Function

        <HttpPost()>
        Function SaveOrganizationEmployee() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = ""
            If (Not IsDate(Common.WebRequest.Data("DateHired"))) Then
                strReturn = JsonDictionary.Serialize("OrgEmployeeData", "DateHiredError")
                Return strReturn
            ElseIf (Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("IsDeactivated")))) Then
                If (Not IsDate(Common.WebRequest.Data("DeactivationDate"))) Then
                    strReturn = JsonDictionary.Serialize("OrgEmployeeData", "DeactivationDateError")
                    Return strReturn
                End If
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrgEmployeeID")
            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")
            clsOrganization.DateHired = Common.WebRequest.Data("DateHired")
            clsOrganization.IsDeactivated = Common.WebRequest.Data("IsDeactivated")
            clsOrganization.DeactivationDate = Common.WebRequest.Data("DeactivationDate")
            clsOrganization.DeactivationReason = Common.WebRequest.Data("DeactivationReason")
            clsOrganization.Comments = Common.WebRequest.Data("Comments")
            clsOrganization.IsBan = Common.WebRequest.Data("IsBan")
            clsOrganization.IsConfirmed = Common.WebRequest.Data("IsConfirm")

            strReturn = JsonDictionary.Serialize("OrgEmployeeData", clsOrganization.SaveOrganizationEmployee())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationCustomer() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgCustomerID")
            clsOrganization.OrganizationID = WebRequest.Instance.OrgID
            clsOrganization.CustomerID = Common.WebRequest.Data("CustomerID")

            strReturn = JsonDictionary.Serialize("OrgCustomerData", clsOrganization.CheckOrganizationCustomer())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustList() As String
            Dim clsOrganization As New Organization
            Dim strSearchString As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
                strReturn = JsonDictionary.Serialize("OrgCustomerData", clsOrganization.getOrganizationCustList())
            Else
                strSearchString = Common.WebRequest.Data("SearchKeyword")
                strReturn = JsonDictionary.Serialize("OrgCustomerData", clsOrganization.getOrganizationCustList(strSearchString))
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgCustomerData", clsOrganization.getOrganizationCustInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationCustomer() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = ""
            If (Not IsDate(Common.WebRequest.Data("ActivationDate"))) Then
                strReturn = JsonDictionary.Serialize("OrgCustomerData", "ActivationDateError")
                Return strReturn
            ElseIf (Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("IsDeactivated")))) Then
                If (Not IsDate(Common.WebRequest.Data("DeactivationDate"))) Then
                    strReturn = JsonDictionary.Serialize("OrgCustomerData", "DeactivationDateError")
                    Return strReturn
                End If
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrgCustomerID")
            clsOrganization.CustomerID = Common.WebRequest.Data("CustomerID")
            clsOrganization.ActivationDate = Common.WebRequest.Data("ActivationDate")
            clsOrganization.IsDeactivated = Common.WebRequest.Data("IsDeactivated")
            clsOrganization.DeactivationDate = Common.WebRequest.Data("DeactivationDate")
            clsOrganization.DeactivationReason = Common.WebRequest.Data("DeactivationReason")
            clsOrganization.Comments = Common.WebRequest.Data("Comments")
            clsOrganization.IsBan = Common.WebRequest.Data("IsBan")

            strReturn = JsonDictionary.Serialize("OrgCustomerData", clsOrganization.SaveOrganizationCustomer())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationSupplier() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgSupplierID")
            clsOrganization.SupplierID = Common.WebRequest.Data("SupplierID")

            strReturn = JsonDictionary.Serialize("OrgSupplierData", clsOrganization.CheckOrganizationSupplier())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationSupplierList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String
            Dim SearchKeyword As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            SearchKeyword = Common.WebRequest.Data("SearchKeyWord")
            strReturn = JsonDictionary.Serialize("OrgSupplierData", clsOrganization.getOrganizationSupplierList(SearchKeyword))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationSupplierInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgSupplierData", clsOrganization.getOrganizationSupplierInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationSupplier() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = ""
            If (Not IsDate(Common.WebRequest.Data("ActivationDate"))) Then
                strReturn = JsonDictionary.Serialize("OrgSupplierData", "ActivationDateError")
                Return strReturn
            ElseIf (Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("IsDeactivated")))) Then
                If (Not IsDate(Common.WebRequest.Data("DeactivationDate"))) Then
                    strReturn = JsonDictionary.Serialize("OrgSupplierData", "DeactivationDateError")
                    Return strReturn
                End If
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrgSupplierID")
            clsOrganization.SupplierID = Common.WebRequest.Data("SupplierID")
            clsOrganization.ActivationDate = Common.WebRequest.Data("ActivationDate")
            clsOrganization.IsDeactivated = Common.WebRequest.Data("IsDeactivated")
            clsOrganization.DeactivationDate = Common.WebRequest.Data("DeactivationDate")
            clsOrganization.DeactivationReason = Common.WebRequest.Data("DeactivationReason")
            clsOrganization.Comments = Common.WebRequest.Data("Comments")
            clsOrganization.IsBan = Common.WebRequest.Data("IsBan")

            strReturn = JsonDictionary.Serialize("OrgSupplierData", clsOrganization.SaveOrganizationSupplier())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationBrand() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgBrandRecID")
            clsOrganization.OrganizationID = WebRequest.Instance.OrgID
            clsOrganization.BrandName = Common.WebRequest.Data("BrandName")

            strReturn = JsonDictionary.Serialize("OrgBrandData", clsOrganization.CheckOrganizationBrand())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationBrandList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("OrgBrandData", clsOrganization.getOrganizationBrandList())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationBrandInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgBrandData", clsOrganization.getOrganizationBrandInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationBrand() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgBrandRecID")
            clsOrganization.BrandName = Common.WebRequest.Data("BrandName")

            strReturn = JsonDictionary.Serialize("OrgBrandData", clsOrganization.SaveOrganizationBrand())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgProductID")
            clsOrganization.OrganizationID = WebRequest.Instance.OrgID
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.CheckOrganizationProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgCompanyWideProductCount() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("OrgProductData", Convert.ToInt32(clsOrganization.getOrgCompanyWideProductCount().Rows(0).Item("ProductCount")).ToString("#,##0"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String
            Dim strSearchString As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
                strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.getOrganizationProductList())
            Else
                strSearchString = Common.WebRequest.Data("SearchKeyword")
                strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.getOrganizationProductList(strSearchString))
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.getOrganizationProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductMeasurementList() As String
            Dim clsOrganization As New Organization
            Dim strID As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ProductID")
            strReturn = JsonDictionary.Serialize("OrgProductMeasurementData", clsOrganization.getOrganizationProductMeasurementList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductMeasurementInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgProductMeasurementData", clsOrganization.getOrganizationProductMeasurementInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductPriceList() As String
            Dim clsOrganization As New Organization
            Dim strID As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ProductID")
            strReturn = JsonDictionary.Serialize("OrgProductPriceData", clsOrganization.getOrganizationProductPriceList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductPriceInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgProductPriceData", clsOrganization.getOrganizationProductPriceInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Public Function GetOrganizationProductBOM() As String

            Dim pID As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pID = Common.WebRequest.Data("ID")
            If CStr2(pID).Trim = "" Then
                pID = ""
            ElseIf IsGuid(pID) Then
                ' ok lang
            Else
                Return JsonDictionary.Serialize("Error", "Invalid data for pID")
            End If

            pOut = New JsonDictionary

            Dim strOrgID As String = Common.WebRequest.Data("OrganizationID")
            Dim strProductID As String = Common.WebRequest.Data("ProductID")

            GetOrganizationProductBOMTree(strOrgID, strProductID, pID, pOut)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Private Sub GetOrganizationProductBOMTree(ByVal vOrgID As String, ByVal vProductID As String, ByVal vProductBOMID As String, ByRef rJson As JsonDictionary)

            Dim pDT As DataTable
            Dim pDTChild As DataTable
            Dim pJson As JsonDictionary
            Dim clsOrganization As New Organization

            pDT = clsOrganization.GetOrganizationProductBOMProductMember(vOrgID, vProductID, vProductBOMID)
            If pDT.Rows.Count > 0 Then
                rJson.Add("ProductBOM", pDT)
            End If

            If (pDT.Rows.Count > 0) Then
                For Each pDR As DataRow In pDT.Rows
                    pDTChild = clsOrganization.GetOrganizationProductBOMProductMember(vOrgID, vProductID, CStr2(pDR("ID")))
                    If pDTChild.Rows.Count > 0 Then
                        With rJson.AddList("SubProductBOM")
                            'For Each pDRChild As DataRow In pDTChild.Rows
                            pJson = .AddDictionary()
                            GetOrganizationProductBOMTree(vOrgID, vProductID, CStr2(pDR("ID")), pJson)
                            'Next pDRChild
                        End With
                    End If
                Next pDR
            End If

        End Sub

        <HttpPost()>
        Public Function GetOrganizationProductMeasurement() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim strProductID As String = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.GetOrganizationProductMeasurement(strProductID))

            Return strReturn

        End Function

        <HttpPost()>
        Public Function GetOrganizationProductMeasurementBarcode() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim strMeasurementID As String = Common.WebRequest.Data("MeasurementID")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.getOrganizationProductMeasurementBarcodeList(strMeasurementID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationProduct() As String
            Dim clsOrganization As New Organization
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = ""
            If (Not IsDate(Common.WebRequest.Data("ActivationDate"))) Then
                strReturn = JsonDictionary.Serialize("OrgProductData", "ActivationDateError")
                Return strReturn
            ElseIf (Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("IsDeactivated")))) Then
                If (Not IsDate(Common.WebRequest.Data("DeactivationDate"))) Then
                    strReturn = JsonDictionary.Serialize("OrgProductData", "DeactivationDateError")
                    Return strReturn
                End If
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrgProductID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.IsDoNotShareDataToBrandOwner = Common.WebRequest.Data("IsDoNotShareData")
            clsOrganization.ActivationDate = Common.WebRequest.Data("ActivationDate")
            clsOrganization.IsDeactivated = Common.WebRequest.Data("IsDeactivated")
            clsOrganization.DeactivationDate = Common.WebRequest.Data("DeactivationDate")
            clsOrganization.DeactivationReason = Common.WebRequest.Data("DeactivationReason")
            clsOrganization.Comments = Common.WebRequest.Data("Comments")
            clsOrganization.IsBan = Common.WebRequest.Data("IsBan")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.SaveOrganizationProduct())

            '''Save Product info
            If (Common.WebRequest.Data("OrgID").Trim() = Common.WebRequest.Data("UploadedByID").ToString()) Then
                clsPIMProduct.ID = Common.WebRequest.Data("ProductID")
                clsPIMProduct.OrganizationID = Common.WebRequest.Data("OrgID")
                clsPIMProduct.Brand = Common.WebRequest.Data("Brand")
                clsPIMProduct.ProductNo = Common.WebRequest.Data("ProductNo")
                clsPIMProduct.ProductName = Common.WebRequest.Data("ProductName")
                clsPIMProduct.ModelNo = Common.WebRequest.Data("ModelNo")
                clsPIMProduct.GroupNo = Common.WebRequest.Data("GroupNo")
                clsPIMProduct.Division = Common.WebRequest.Data("Division")
                clsPIMProduct.Category = Common.WebRequest.Data("Category")
                clsPIMProduct.MarketingText = Common.WebRequest.Data("MarketingText")
                clsPIMProduct.Warranty = Common.WebRequest.Data("Warranty")
                clsPIMProduct.SaveProductInfo()
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationProductMeasurement() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("ID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.UnitMeasureID = Common.WebRequest.Data("UnitMeasureID")
            clsOrganization.Qty = CDec2(Common.WebRequest.Data("Quantity"))
            clsOrganization.ProductDiscount = CDec2(Common.WebRequest.Data("Discount"))
            clsOrganization.ProductMeasurementContentQty = CDec2(Common.WebRequest.Data("ContentQty"))
            clsOrganization.ProductMeasurementContentUnit = Common.WebRequest.Data("ContentUnit")
            clsOrganization.ProductMeasurementDimLenght = CDec2(Common.WebRequest.Data("DimensionLenght"))
            clsOrganization.ProductMeasurementDimLenghtUnit = Common.WebRequest.Data("DimensionLenghtUnit")
            clsOrganization.ProductMeasurementDimWidth = CDec2(Common.WebRequest.Data("DimensionWidth"))
            clsOrganization.ProductMeasurementDimWidthUnit = Common.WebRequest.Data("DimensionWidthUnit")
            clsOrganization.ProductMeasurementDimHeight = CDec2(Common.WebRequest.Data("DimensionHeight"))
            clsOrganization.ProductMeasurementDimHeightUnit = Common.WebRequest.Data("DimensionHeightUnit")
            clsOrganization.ProductMeasurementWeight = CDec2(Common.WebRequest.Data("ContentWeight"))
            clsOrganization.ProductMeasurementWeightUnit = Common.WebRequest.Data("ContentWeightUnit")
            clsOrganization.OrgProductMeasurementBarcodeList = Common.WebRequest.AllData("OrgProductMeasurementBarcodeList")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.SaveOrganizationProductMeasurement())
            
            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationProductPrice() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("ID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.PriceLevelID = Common.WebRequest.Data("PriceLevelID")
            clsOrganization.Price = CDec2(Common.WebRequest.Data("Price"))

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.SaveOrganizationProductPrice())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationProductBOM() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgProductBOMRecID")
            clsOrganization.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.ComponentName = Common.WebRequest.Data("ComponentName")
            clsOrganization.BOMProductID = Common.WebRequest.Data("BOMProductID")
            clsOrganization.Qty = Common.WebRequest.Data("Qty")
            clsOrganization.UnitMeasureID = Common.WebRequest.Data("UnitMeasureID")
            clsOrganization.ComponentMethod = Common.WebRequest.Data("ComponentMethod")
            clsOrganization.ParentBOMProductID = Common.WebRequest.Data("ParentBOMProductID")

            strReturn = JsonDictionary.Serialize("OrgProductBOMData", clsOrganization.SaveOrganizationProductBOM())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgOwnerList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrgOwnerList", clsOrganization.getOrgOwnerList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgOwnerInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgOwnerInfo", clsOrganization.getOrgOwnerInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgOwner() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgOwnerID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID

            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")
            clsOrganization.JobOccupationID = Common.WebRequest.Data("JobOccupationID")
            clsOrganization.BusinessUnit = Common.WebRequest.Data("BusinessUnit")
            clsOrganization.IsIncorporator = Common.WebRequest.Data("IsIncorporator")
            clsOrganization.IsDirector = Common.WebRequest.Data("IsDirector")
            clsOrganization.IsStockHolder = Common.WebRequest.Data("IsStockHolder")
            clsOrganization.IsOfficer = Common.WebRequest.Data("IsOfficer")
            clsOrganization.IsExecutiveCommittee = Common.WebRequest.Data("IsExecutiveCommittee")
            clsOrganization.IsOwner = Common.WebRequest.Data("IsOwner")

            strReturn = JsonDictionary.Serialize("OrgOwnerData", clsOrganization.SaveOrgOwner())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgOwner() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgOwnerID")

            strReturn = JsonDictionary.Serialize("OrgOwnerData", IIf(clsOrganization.RemoveOrgOwner(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationFOWSTProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFowstProductID")
            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgFowstProductData", clsOrganization.CheckOrganizationFOWSTProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationFowstProductBinList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgFowstProductData", clsOrganization.getOrganizationFowstProductBinList())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationFowstProductList() As String
            Dim clsOrganization As New Organization
            Dim strSearchKeyword As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            strSearchKeyword = Common.WebRequest.Data("SearchKeyword")

            strReturn = JsonDictionary.Serialize("OrgFowstProductData", clsOrganization.getOrganizationFOWSTProductList(strSearchKeyword))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationFowstProductInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgFowstProductData", clsOrganization.getOrganizationFowstProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationFowstProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFowstProductID")
            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.OrgFOWSTProductReorderPoint = Common.WebRequest.Data("ReOrderPoint")

            clsOrganization.OrgFOWSTProductBinList = Common.WebRequest.AllData("OrgFOWSTProductBinList")

            strReturn = JsonDictionary.Serialize("OrgFowstProductData", clsOrganization.SaveOrganizationFowstProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationCustomerProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgProductID")
            clsOrganization.CustomerID = IIf(Common.WebRequest.Data("OrgCustomerID").Trim().Length > 0, Common.WebRequest.Data("OrgCustomerID"), WebSession.Instance.OrganizationID)
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgCustProductData", clsOrganization.CheckOrganizationCustomerProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgCustomerProductCount() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.CustomerID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrgCustProductData", Convert.ToInt32(clsOrganization.getOrgCustomerProductCount().Rows(0).Item("ProductCount")).ToString("#,##0"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustomerProductList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String
            Dim strSearchString As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.CustomerID = WebSession.Instance.OrganizationID
            If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
                strReturn = JsonDictionary.Serialize("OrgCustProductData", clsOrganization.getOrganizationCustomerProductList())
            Else
                strSearchString = Common.WebRequest.Data("SearchKeyword")
                strReturn = JsonDictionary.Serialize("OrgCustProductData", clsOrganization.getOrganizationCustomerProductList(strSearchString))
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustomerProductInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgCustProductData", clsOrganization.getOrganizationCustomerProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationCustomerProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = ""
            If (Not IsDate(Common.WebRequest.Data("ActivationDate"))) Then
                strReturn = JsonDictionary.Serialize("OrgCustProductData", "ActivationDateError")
                Return strReturn
            ElseIf (Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("IsDeactivated")))) Then
                If (Not IsDate(Common.WebRequest.Data("DeactivationDate"))) Then
                    strReturn = JsonDictionary.Serialize("OrgCustProductData", "DeactivationDateError")
                    Return strReturn
                End If
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrgProductID")
            clsOrganization.CustomerID = IIf(Common.WebRequest.Data("OrgCustomerID").Trim().Length > 0, Common.WebRequest.Data("OrgCustomerID"), WebSession.Instance.OrganizationID)
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.ActivationDate = Common.WebRequest.Data("ActivationDate")
            clsOrganization.IsDeactivated = Common.WebRequest.Data("IsDeactivated")
            clsOrganization.DeactivationDate = Common.WebRequest.Data("DeactivationDate")
            clsOrganization.DeactivationReason = Common.WebRequest.Data("DeactivationReason")
            clsOrganization.Comments = Common.WebRequest.Data("Comments")
            clsOrganization.IsBan = Common.WebRequest.Data("IsBan")

            strReturn = JsonDictionary.Serialize("OrgCustProductData", clsOrganization.SaveOrganizationCustomerProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function VerifyOrganizationCustomerFOWST() As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebSession.Instance.OrganizationFowstID = Common.WebRequest.Data("FowstID")

            strReturn = JsonDictionary.Serialize("Success", "TRUE")

            Return strReturn
        End Function

        <HttpPost()>
        Function CheckOrganizationCustomerFOWSTProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFowstProductID")
            clsOrganization.FowstID = WebSession.Instance.OrganizationFowstID
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgCustFowstProductData", clsOrganization.CheckOrganizationCustomerFOWSTProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustomerFowstProductBinList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = WebSession.Instance.OrganizationFowstID
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgCustFowstProductData", clsOrganization.getOrganizationCustomerFowstProductBinList())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustomerFOWSTProductList() As String
            Dim clsOrganization As New Organization
            Dim strSearchKeyword As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = WebSession.Instance.OrganizationFowstID
            strSearchKeyword = Common.WebRequest.Data("SearchKeyword")

            strReturn = JsonDictionary.Serialize("OrgCustFowstProductData", clsOrganization.getOrganizationCustomerFOWSTProductList(strSearchKeyword))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationCustomerFowstProductInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgCustFowstProductData", clsOrganization.getOrganizationCustomerFowstProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationCustomerFowstProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFowstProductID")
            clsOrganization.FowstID = WebSession.Instance.OrganizationFowstID
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.OrgFOWSTProductReorderPoint = Common.WebRequest.Data("ReOrderPoint")

            clsOrganization.OrgFOWSTProductBinList = Common.WebRequest.AllData("OrgFOWSTProductBinList")

            strReturn = JsonDictionary.Serialize("OrgCustFowstProductData", clsOrganization.SaveOrganizationCustomerFowstProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgHoldingCompanyInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.getOrgHoldingCompanyInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgHoldingCompany() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (WebSession.Instance.OrganizationID = Common.WebRequest.Data("OrgHoldingCompanyID")) Then
                strReturn = JsonDictionary.Serialize("OrgAffiliationData", "InvalidHoldingCompany")
            End If

            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.IsHasHoldingCompany = Common.WebRequest.Data("IsHasHoldingCompany")
            clsOrganization.OrgHoldingCompanyID = Common.WebRequest.Data("OrgHoldingCompanyID")

            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.SaveOrgHoldingCompany())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgAffliatesList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.getOrgAffliatesList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgAffliatesInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.getOrgAffliatesInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgAffiliates() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (WebSession.Instance.OrganizationID = Common.WebRequest.Data("OrgAffiliatesID")) Then
                strReturn = JsonDictionary.Serialize("OrgAffiliationData", "InvalidAffiliateCompany")
            End If

            clsOrganization.ID = Common.WebRequest.Data("OrgAffiliationID")
            clsOrganization.OrganizationID = WebSession.Instance.OrganizationID
            clsOrganization.OrgAffiliatesID = Common.WebRequest.Data("OrgAffiliatesID")
            clsOrganization.Remarks = Common.WebRequest.Data("Remarks")

            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.SaveOrgAffiliates())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveOrgAffiliates() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgAffiliationID")

            strReturn = JsonDictionary.Serialize("OrgAffiliationData", IIf(clsOrganization.RemoveOrgAffiliates(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationName() As String
            Return JsonDictionary.Serialize("OrganizationName", WebRequest.Instance.OrgName)
        End Function

        Function OrganizationEmployeeWorkingHours() As ActionResult

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            '    If (Not SecurityRole.Instance.HandleRoleAccess("WorkingHours")) Then Exit Function
            'End If

            Return View()
        End Function

        Function SaveWorkingHours() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.SaveWorkingHours())

        End Function

        Function RemoveEmployeeWorkSchedule() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.RemoveEmployeeWorkSchedule())
        End Function

        Function GetAllSchedule() As String

            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetAllSchedule())
        End Function

        Function OrganizationEmployeeAssignWorkSchedule() As ActionResult

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            '    If (Not SecurityRole.Instance.HandleRoleAccess("WorkingHours")) Then Exit Function
            'End If

            Return View()
        End Function

        Function SaveEmployeeWorkSchedule() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.SaveEmployeeWorkingHours())

        End Function

        Function GetEmployeeSchedule() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeSchedule())
        End Function


        Function OrganizationEmployeePayGrade() As ActionResult

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            '    If (Not SecurityRole.Instance.HandleRoleAccess("WorkingHours")) Then Exit Function
            'End If

            Return View()
        End Function



        <HttpPost()>
        Function AddOrgOwnerProductList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("OrgData", clsOrganization.AddOrgOwnerProductList(Common.WebRequest.Data("ID")))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProductNames() As String
            Dim pDT As DataTable
            Dim cls As New Organization
            Dim strReturn As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cls.FilterName = Common.WebRequest.Data("FilterName")

            If cls.FilterName.Trim() <> "" Then
                cls.KeywordList = Common.WebRequest.AllData("Keywords")
            End If

            pDT = cls.GetProductNames(Common.WebRequest.Data("ID"))

            strReturn = JsonDictionary.Serialize("OrgData", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function AddOrgOwnerProductListCustom() As String

            Dim cls As New Organization
            Dim strReturn As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cls.FilterName = Common.WebRequest.Data("FilterName")

            If cls.FilterName.Trim() <> "" Then
                cls.KeywordList = Common.WebRequest.AllData("Keywords")
            End If

    
            strReturn = JsonDictionary.Serialize("OrgData", cls.AddOrgOwnerProductListCustom(Common.WebRequest.Data("ID")))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetOrgProductFilterResult() As String
            Dim pDT As DataTable
            Dim cls As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cls.FilterName = Common.WebRequest.Data("FilterName")

            If cls.FilterName = "Brand" Then
                pDT = cls.GetBrand(Common.WebRequest.Data("ID"))
            ElseIf cls.FilterName = "Category" Then
                pDT = cls.GetCategory(Common.WebRequest.Data("ID"))
            End If


            Return JsonDictionary.Serialize("Filter", pDT)

        End Function

      
    End Class

End Namespace