Imports System.Net.Mime
Imports Aspose.Cells
Imports Newtonsoft.Json

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

        'temp only
        Function BsOrganizationSummary() As ActionResult
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

        Function BsOrganizationInfo() As ActionResult
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

        Function BsOrganizationFinancialReport() As ActionResult
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

        Function BsOrganizationContact() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationAddress() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company Places (FOWST)")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationRepresentative() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function BsOrganizationRepresentative() As ActionResult
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

        Function BsOrganizationOwner() As ActionResult
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

        Function BsOrganizationAffiliation() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationEmployee() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company Employee")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationCustomer() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company Customer")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationSupplier() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company Supplier")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationProduct() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company-Wide Product List")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationFOWSTProduct() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Company-Wide Product List")) Then Exit Function
            End If

            Return View()
        End Function

        Function OrganizationFOWSTProductList() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("FOWST Level Product List")) Then Exit Function
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

        Function BsOrganizationBrands() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            '' ''If (Not SecurityRole.Instance.HandleRoleAccess("Company Wide Product List")) Then Exit Function

            Return View()
        End Function

        Function SaveFOWSTStatus()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim fowstid As String = Common.WebRequest.Data("FOWSTID")
                Dim status As String = Common.WebRequest.Data("Status")
                Dim pIU As New InsertUpdate("MobizLibraryOrganizationFOWST")

                pIU("Status") = ReadyString(status)
                pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pIU("LastUpdated") = "getdate()"

                Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ID = " & ReadyString(fowstid)))

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try
        End Function

        Function DownloadFOWSTTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "\FileTemplates\FOWSTTemplate.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "\FileTemplates\FOWSTTemplate.xlsx"
            pContentType = "application/xlsx"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "FOWSTTemplate.xlsx")
        End Function

        Function DownloadEmployeesTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "\FileTemplates\EmployeesTemplate.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "\FileTemplates\EmployeesTemplate.xlsx"
            pContentType = "application/xlsx"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "EmployeesTemplate.xlsx")
        End Function

        Function UploadFOWST()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim file As Object = Request.Files
            Dim tradename As String = Request.Params.Get("tradename")
            Dim userid As String = Request.Params.Get("userid")
            Dim orgid As String = Request.Params.Get("OrgID")
            Dim template As HttpPostedFileBase = file(0)
            Dim pFilename, pSql As String
            Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\"
            Dim dtaExcelData = New DataTable
            Dim row As Object
            Dim canvassnumber As Integer = 0
            Dim rownumber As Integer = 2
            Dim result As String
            Dim strHeader = {"FOWST Name", "Internal Code", "House/Unit/Bldg No.", "Street Name", "City", "VAT Type", "GroupHead"}
            Dim pFS As IO.FileStream
            Dim errmsg As String = ""
            Dim pDT As DataTable
            'isOkay = True

            If Not System.IO.Directory.Exists(pSourceURL) Then
                System.IO.Directory.CreateDirectory(pSourceURL)
            End If

            pFilename = pSourceURL & "FOWSTTemplate.xls"
            CreateFolder(pFilename)
            template.SaveAs(pFilename)

            pFS = New IO.FileStream(pFilename, IO.FileMode.Open)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pFilename, strHeader, "")

                If dtaExcelData Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid Template. Please upload valid template.")
                End If

                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                    Dim col As Object = dtaExcelData.Rows(i)
                    Dim fowsttype As String = "6af9bc83-426c-4774-a051-044fc09c552b" 'Store
                    Dim fowstname As String = Trim(CStr2(col.Item("FOWST Name")))
                    Dim fowstinternalcode As String = Trim(CStr2(col.Item("Internal Code")))
                    Dim bldgno As String = Trim(CStr2(col.Item("House/Unit/Bldg No.")))
                    Dim street As String = Trim(CStr2(col.Item("Street Name")))
                    Dim city As String = Trim(CStr2(col.Item("City")))
                    Dim vattype As String = Trim(CStr2(col.Item("VAT Type")))
                    Dim GroupHead As String = Trim(CStr2(col.Item("GroupHead")))
                    Dim cityid As String
                    Dim country As String = "e783ed13-aa98-4cc1-918e-d49b05c0167e" 'Philippines
                    Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")

                    If String.IsNullOrWhiteSpace(fowstname) Then
                        Exit For
                    End If

                    pSql = "SELECT Name FROM MobizLibraryOrganizationFOWST WHERE Name = " & ReadyString(fowstname) & " AND OrganizationID = " & ReadyString(orgid)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If pDT.Rows.Count = 0 Then
                        pSql = "SELECT ID FROM MobizLibraryOrganizationFOWST WHERE Code = " & ReadyString(fowstinternalcode) & " AND OrganizationID = " & ReadyString(orgid)
                        pDT = Common.WebRequest.ProData.Execute(pSql)

                        If pDT.Rows.Count > 0 Then
                            errmsg &= "<br>FOWST Internal code <i>" & fowstinternalcode & "</i> already exist. Row number: " & rownumber
                            rownumber += 1
                            Continue For
                        End If

                        pSql = "SELECT ID FROM MobizLibraryCities WHERE Name = " & ReadyString(city)
                        pDT = Common.WebRequest.ProData.Execute(pSql)

                        If pDT.Rows.Count = 0 Then
                            cityid = Guid.NewGuid.ToString()
                            p = New InsertUpdate("MobizLibraryCities")
                            p("ID") = ReadyString(cityid)
                            p("Name") = ReadyString(city)
                            p("IsActive") = 1
                            p("UserID") = ReadyString(userid)
                            p("LastUpdated") = "getdate()"

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        Else
                            cityid = pDT.Rows(0)("ID").ToString()
                        End If

                        p = New InsertUpdate("MobizLibraryOrganizationFOWST")
                        Dim fowstid As String = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(fowstid)
                        p("OrganizationID") = ReadyString(orgid)
                        p("FOWSTTypeID") = ReadyString(fowsttype)
                        p("Name") = ReadyString(fowstname)
                        p("Code") = ReadyString(fowstinternalcode)
                        p("BusinessTradeNameID") = ReadyString(tradename)
                        p("StreetNumber") = ReadyString(bldgno)
                        p("Street") = ReadyString(street)
                        p("FowstCityID") = ReadyString(cityid)
                        p("FowstCountryID") = ReadyString(country)
                        p("IsActive") = 1
                        p("IsVAT") = IIf(vattype = "" Or vattype = "VAT", 1, 0)
                        p("IsFranchisee") = 0
                        p("FOWSTNo") = "ISNULL((SELECT MAX(FOWSTNo) FROM MobizLibraryOrganizationFOWST WHERE OrganizationID " & If(orgid.Trim().Length > 0, " = " & ReadyString(orgid), " IS NULL") & "),0) + 1"
                        p("UserID") = ReadyString(userid)
                        p("LastUpdated") = "getdate()"

                        If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "91673033-3712-4976-8f0a-d298cde49793" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12" Then
                            'WOW or WOW' Franchise database
                            'by default
                            p("Region") = ReadyString("NCR- Mandaluyong")
                            p("DefaultPosView") = ReadyString("Hot Buttons View")
                            p("DefaultHotButtonTemplateID") = ReadyString("757f1338-7012-4719-bc5b-b8fa33e46252") '"PC" hb template
                        End If

                        Common.WebRequest.ProData.Execute(p.GetInsert())

                        If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "91673033-3712-4976-8f0a-d298cde49793" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12" Then
                            'WOW or WOW' Franchise database
                            'by default
                            p = New InsertUpdate("LibraryReceiptSettingsFowst")

                            If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then
                                'WOW
                                p("ReceiptSettingsID") = ReadyString("26b47572-cca4-40e5-8bcd-890c6025cfc1")
                            Else
                                'Franchise
                                p("ReceiptSettingsID") = ReadyString("78eec5b2-da4b-492e-bf02-6c8ad7efdd9e")
                            End If

                            p("FowstID") = ReadyString(fowstid)

                            Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())


                            If CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "91673033-3712-4976-8f0a-d298cde49793" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12" Then
                                Dim dtGroupHead As DataTable
                                Dim GroupHeadID As String
                                Dim GroupName As String
                                Dim organiztion As New Organization
                                'create group
                                If Not GroupHead = vbNullString Then

                                    pSql = "Select MobizlibraryPeople.ID, MobizlibraryPeople.Surname from MobizlibraryPeople inner join LibraryOrganizationEmployee on LibraryOrganizationEmployee.PeopleID = MobizlibraryPeople.ID " & vbCrLf &
                                            " WHERE SUBSTRING(CompanyID, PATINDEX('%[^0]%', CompanyID + '.'), LEN(CompanyID)) = " & ReadyString(GroupHead)

                                    dtGroupHead = Common.WebRequest.ProData.Execute(pSql)

                                    If dtGroupHead.Rows.Count > 0 Then
                                        GroupHeadID = CStr2(dtGroupHead.Rows(0).Item("ID"))
                                        GroupName = "Z_" & CStr2(dtGroupHead.Rows(0).Item("Surname"))
                                    End If

                                    'If CStr2(WebRequest.Instance.OrgID).ToLower = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" OrElse CStr2(WebRequest.Instance.OrgID).ToLower = "cfe39df6-8809-4973-9515-810df75aed12" Then
                                    '    'Franchisee
                                    '    'Default Group Distribution Center for Franchisee (FOWST: Potato Corner HQ)
                                    '    fowstid = "b980e58e-e08f-443d-9212-58dabee04e07"
                                    'End If

                                    organiztion.SaveGroup(GroupName, fowstid, GroupHeadID, "")
                                End If
                                'add group head

                                'add current fowst

                            End If
                        End If
                    Else
                        errmsg &= "<br><i>" & fowstname & "</i> already exist. Row number: " & rownumber
                    End If

                    rownumber += 1
                Next

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganizationFOWST'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If errmsg = "" Then
                    Common.WebRequest.ProData.CommitTransaction()
                    Return JsonDictionary.Serialize("Results", "saved")
                Else
                    Common.WebRequest.ProData.RollbackTransaction()
                    Return JsonDictionary.Serialize("Validation", errmsg)
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
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

        Function DownloadCompanyProductList() As ActionResult
            Dim clsOrganization As New Organization
            Dim pDT As DataTable
            Dim pFS As IO.FileStream
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim intCntr As String
            Dim intRowCntr As Integer

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            'End If
            If WebSession.Instance.PeopleUserID Is Nothing Then Exit Function
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            Try
                If Not System.IO.Directory.Exists(Common.ServerPath & "/Content/PIM") Then
                    System.IO.Directory.CreateDirectory(Common.ServerPath & "/Content/PIM")
                End If

                Dim pIncludeActive As Boolean = CBool2(WebRequest.Instance.QueryString("IncludeActive"))
                Dim pIncludeInactive As Boolean = CBool2(WebRequest.Instance.QueryString("IncludeInactive"))

                Dim vendorid As String = Request.Params.Get("VendorID")
                pFS = New IO.FileStream(Common.ServerPath & "/Content/PIM/PIMTemplate.xls", IO.FileMode.Open)
                pWB = New Workbook
                pWB.Open(pFS)
                pWS = pWB.Worksheets(0)

                '''get List of Product
                clsOrganization.MaxReturnRow = 9999999
                Const pRowsCount As Integer = 1000
                intRowCntr = 1

                Dim lastcol As Integer = pWS.Cells.MaxDataColumn
                lastcol += 1
                pWS.Cells(0, lastcol).PutValue("Upcoming Retail Price")
                lastcol += 1
                pWS.Cells(0, lastcol).PutValue("Upcoming Retail Price Effectivity Date")
                lastcol += 1
                pWS.Cells(0, lastcol).PutValue("Active")

                Do
                    pDT = clsOrganization.getCompanyProductList("", intRowCntr, pRowsCount, vendorid, pIncludeActive, pIncludeInactive)
                    If (pDT.Rows.Count > 0) Then
                        intCntr = 0
                        While (intCntr < pDT.Rows.Count)

                            Dim objTest As New ArrayList()

                            objTest.Add(pDT.Rows(intCntr).ItemArray(0))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(1))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(2))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(3))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(4))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(5))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(6))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(7))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(8))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(9))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(10))
                            objTest.Add(pDT.Rows(intCntr).ItemArray(11))
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add("")
                            objTest.Add(pDT.Rows(intCntr).ItemArray(12)) ' Rentable/PPE Asset?
                            objTest.Add(pDT.Rows(intCntr).Item("UpcomingPrice"))
                            objTest.Add(If(pDT.Rows(intCntr).Item("Active"), "Yes", "No"))

                            If Not String.IsNullOrWhiteSpace(pDT.Rows(intCntr).Item("UpcomingPriceEffectivityDate").ToString()) Then
                                objTest.Add(Format(CDate(pDT.Rows(intCntr).Item("UpcomingPriceEffectivityDate")), "MM-dd-yyyy"))
                            End If

                            pWS.Cells.ImportArrayList(objTest, intRowCntr, 0, False)

                            intCntr = intCntr + 1
                            intRowCntr = intRowCntr + 1

                        End While
                    End If

                    If pDT.Rows.Count < pRowsCount Then
                        Exit Do
                    End If
                Loop

                pWS.AutoFitColumns()
                If CBool2(Request.Params.Get("IsEditVendorPrice")) Then
                    'idelete ang uban column
                    Dim pimcolumnremove As New List(Of Integer) From {4, 6, 7, 9, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30}

                    For Each col As Integer In pimcolumnremove
                        pWS.Cells.HideColumn(col)
                    Next
                End If

                pWB.Save(Common.ServerPath & "temp\PIMTemplate\PIMTemplate_" & CStr2(WebSession.Instance.OrganizationID) & ".xls")

                Try
                    If pFS IsNot Nothing Then pFS.Dispose()
                Catch ex As Exception
                End Try

                '''Download the File
                Dim pContentType As String
                Dim pSourceURL As String
                pSourceURL = Common.ServerPath & "temp\PIMTemplate\PIMTemplate_" & CStr2(WebSession.Instance.OrganizationID) & ".xls"
                pContentType = "application/xls"

                Return File(pSourceURL, pContentType, "PIMTemplate.xls")
            Catch ex As Exception
                AppendToDebug(ex.Message)
            End Try
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("OrganizationID", WebSession.Instance.OrganizationID)

            Return pOut.Serialize

        End Function

        Function GetMyMobizAccount()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT Username, ActivationDate, MobizLibraryPeopleUserAccount.LastUpdated, ppol.FirstName + ' ' + ppol.Surname as ApprovedByName FROM MobizLibraryPeopleUserAccount " & vbCrLf &
                                " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = MobizLibraryPeopleUserAccount.ApprovedBy " & vbCrLf &
                                " WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("EmployeeID"))

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function SaveVendorPrice()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                WebRequest.Instance.ProData.BeginTransaction()
                WebRequest.Instance.ProData.CommitTransaction()

                WebRequest.Instance.Business.TransInvoice.SaveVendorPriceHistory(vVendorID:=Common.WebRequest.Data("VendorID"), _
                    vProductID:=Common.WebRequest.Data("ProductID"), _
                    vUnitPrice:=Common.WebRequest.Data("Price"), _
                    vTransDate:=Now, _
                    vTransID:=Guid.Empty.ToString, _
                    vTransType:="PimVendorEdit")
                Dim pUnitQty As Decimal = CDec2(Common.WebRequest.Data("UnitQty"))
                If pUnitQty < 0.001 Then
                    pUnitQty = 1
                End If
                WebRequest.Instance.Business.LibraryPriceCanvass.UpdateLibraryVendorPrices(vVendorID:=Common.WebRequest.Data("VendorID"),
                    vProductID:=Common.WebRequest.Data("ProductID"),
                    vPrice:=Common.WebRequest.Data("Price"),
                    vUnitQty:=pUnitQty,
                    vIsActive:=Common.WebRequest.Data("IsActive"),
                    vUserID:=WebSession.Instance.PeopleUserID,
                    vUnitMeasure:=Common.WebRequest.Data("UnitMeasure"),
                    vUrl:=Common.WebRequest.Data("Url"),
                    vNote:=Common.WebRequest.Data("Note"))

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", "error")
            End Try
        End Function

        Function SaveOrganizationFiles()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim id As String = Request.Params.Get("id") ' customer/supplier id
            Dim files As HttpFileCollectionBase = Request.Files
            Dim folderName As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\OrganizationFiles\" & id

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim name As String = file.FileName
                    Dim pSourceURL As String = folderName & "\" & name

                    If System.IO.File.Exists(pSourceURL) Then
                        name = Format(Now(), "MMddyyyy_hhmmsstt") & "_" & file.FileName
                        pSourceURL = folderName & "\" & name
                    End If

                    Dim p As New InsertUpdate("MobizLibraryOrganizationFiles")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("OrganizationID") = ReadyString(id)
                    p("FileName") = ReadyString(name)
                    p("FileType") = ReadyString(file.ContentType)
                    p("DatabaseID") = ReadyString(WebRequest.Instance.OrgID)
                    p("FileLocation") = ReadyString(pSourceURL)
                    p("DateUploaded") = "getdate()"

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    file.SaveAs(pSourceURL)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "uploaded")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function RemoveOrganizationFiles()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "DELETE FROM MobizLibraryOrganizationFiles WHERE ID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf

                System.IO.File.Delete(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\OrganizationFiles\" & Common.WebRequest.Data("CompanyID") & "\" & Common.WebRequest.Data("FileName"))

                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "deleted")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Public Function GetOrganizationFiles()
            Try
                Dim pSql As String = "SELECT * FROM MobizLibraryOrganizationFiles WHERE OrganizationID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
                                    IIf(CStr2(Common.WebRequest.Data("FileType")) = "", "", " AND FileType LIKE " & ReadyStringWild(Common.WebRequest.Data("FileType")))


                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveNewCategoriesOfProductOrServices()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pOut As New JsonDictionary

            Try
                Dim id As String = Guid.NewGuid.ToString()
                Dim pSql As String = ""
                Dim p As New InsertUpdate("MobizLibraryCategoriesOfProductOrServices")

                p("CategoryName") = ReadyString(Common.WebRequest.Data("Name"))

                If Common.WebRequest.Data("ID") = "" Then
                    p("ID") = ReadyString(id)
                    p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrganizationID"))
                    p("CreatedByUserID") = ReadyString(Common.WebRequest.Data("UserID"))
                    p("DateCreated") = "getdate()"
                    pSql = p.GetInsert()
                Else
                    p("ModifiedByOrgID") = ReadyString(Common.WebRequest.Data("OrganizationID"))
                    p("ModifiedByUserID") = ReadyString(Common.WebRequest.Data("UserID"))
                    p("DateModified") = "getdate()"
                    pSql = p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID")))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("Results", "saved")
                pOut.Add("ID", id)

            Catch ex As Exception
                AppendToLog(ex.Message)
                pOut.Add("Results", ex.Message)
            End Try

            Return pOut.Serialize()
        End Function

        Function SaveNewBrandNames()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pOut As New JsonDictionary

            Try
                Dim id As String = Guid.NewGuid.ToString()
                Dim pSql As String = ""
                Dim p As New InsertUpdate("MobizLibraryBrandNames")

                p("BrandName") = ReadyString(Common.WebRequest.Data("Name"))

                If Common.WebRequest.Data("ID") = "" Then
                    p("ID") = ReadyString(id)
                    p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrganizationID"))
                    p("CreatedByUserID") = ReadyString(Common.WebRequest.Data("UserID"))
                    p("DateCreated") = "getdate()"
                    pSql = p.GetInsert()
                Else
                    p("ModifiedByOrgID") = ReadyString(Common.WebRequest.Data("OrganizationID"))
                    p("ModifiedByUserID") = ReadyString(Common.WebRequest.Data("UserID"))
                    p("DateModified") = "getdate()"
                    pSql = p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID")))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("Results", "saved")
                pOut.Add("ID", id)

            Catch ex As Exception
                AppendToLog(ex.Message)
                pOut.Add("Results", ex.Message)
            End Try

            Return pOut.Serialize()
        End Function

        <HttpPost()>
        Function getSubscriberOrganization() As String
            Dim pOut As New JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("OrganizationID", WebRequest.Instance.OrgID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function getPeopleOrgID() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")
            strReturn = JsonDictionary.Serialize("Result", clsOrganization.getPeopleOrgID)

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleEmailAddress() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")

            strReturn = JsonDictionary.Serialize("PeopleEmailAddress", clsOrganization.GetPeopleEmailAddress)

            Return strReturn

        End Function

        <HttpPost()>
        Function SendConfirmationViaEmail() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.PeopleEmailAddress = Common.WebRequest.Data("PeopleEmailAddress")
            clsOrganization.PeopleFullName = Common.WebRequest.Data("PeopleName")
            clsOrganization.ConfirmKeyID = Common.WebRequest.Data("ConfirmKeyID")
            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")
            clsOrganization.CompanyName = Common.WebRequest.Data("OrgName")
            clsOrganization.MobizProduct = Common.WebRequest.Data("Product")

            strReturn = JsonDictionary.Serialize("PeopleEmailAddress", clsOrganization.SendConfirmationViaEmail)

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrgUniqueOrgName() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrganizationID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrganizationInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationInfo() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            clsOrganization.ReferenceNo = Common.WebRequest.Data("ReferenceNo")

            clsOrganization.Purpose = Common.WebRequest.Data("Purpose")
            clsOrganization.BusinessScope = Common.WebRequest.Data("BusinessScope")
            clsOrganization.NoOfRetailOutlet = If(Common.WebRequest.Data("NoOfRetailOutlet").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfRetailOutlet"), 0)
            clsOrganization.NoOfOffice = If(Common.WebRequest.Data("NoOfOffice").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfOffice"), 0)
            clsOrganization.NoOfWarehouse = If(Common.WebRequest.Data("NoOfWarehouse").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfWarehouse"), 0)
            clsOrganization.NoOfEmployees = If(Common.WebRequest.Data("NoOfEmployees").ToString().Trim().Length > 0, Common.WebRequest.Data("NoOfEmployees"), 0)

            If CStr2(Common.WebRequest.Data("IsVAT")).ToString().Trim().Length > 0 Then
                clsOrganization.IsVAT = If(Common.WebRequest.Data("IsVAT").ToString().Trim().Length > 0, CInt2(Common.WebRequest.Data("IsVAT")), 0)
            End If


            clsOrganization.OrgTradeNameList = Common.WebRequest.AllData("OrgTradeNameList")
            clsOrganization.OrgCategoryList = Common.WebRequest.AllData("OrgCategoryList")
            clsOrganization.OrgIndustryClassificationList = Common.WebRequest.AllData("OrgIndustryClassificationList")
            'clsOrganization.OrgFranchiseeList = Common.WebRequest.AllData("OrgFranchiseeList")


            Try
                WebSession.Instance.OrganizationID = clsOrganization.SaveOrganizationInfo()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

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
                         "MobizCustCare2221", , , , , True)


#Else
                 SendMail("customercare@mobiz-apps.com",
                     "janine.son@mobiz-apps.com, robert.chong@mobiz-apps.com",
                     "Mobiz",
                     "New Mobiz Customer",
                     strBody,
                     "smtp.live.com",
                     587,
                     "customercare@mobiz-apps.com",
                     "MobizCustCare2221", , , , , True)


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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgFinancialReportInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgFinancialReport() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactTelInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactTel() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactFaxInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactFax() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactMobileInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactMobile() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactEmailInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactEmail() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationData", clsOrganization.getOrgContactWebsiteInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgContactWebsite() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.getOrgFOWSTInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveEmpEmailAddress() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")
            clsOrganization.ContactEmail = Common.WebRequest.Data("EmailAddress")

            strReturn = JsonDictionary.Serialize("SaveEmailResult", clsOrganization.SaveEmpEmailAddress())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgFOWST() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgFOWSTID")
            clsOrganization.OrganizationID = WebRequest.Instance.OrgID ' old code WebSession.Instance.OrganizationID
            clsOrganization.FOWSTTypeID = Common.WebRequest.Data("FOWSTTypeID")
            clsOrganization.FOWSTStatus = Common.WebRequest.Data("FOWSTStatus")
            clsOrganization.FOWSTClassification = Common.WebRequest.Data("FOWSTClassification")
            clsOrganization.FOWSTCode = Common.WebRequest.Data("FOWSTCode")
            clsOrganization.FOWSTName = Common.WebRequest.Data("FOWSTName")
            clsOrganization.FOWSTDescription = Common.WebRequest.Data("FOWSTDescription")
            clsOrganization.FOWSTBusinessTradeNameID = Common.WebRequest.Data("FOWSTBusinessTradeNameID")
            clsOrganization.FOWSTTINBranchCode = Common.WebRequest.Data("FOWSTTINBranchCode")
            clsOrganization.IsVAT = Common.WebRequest.Data("IsVAT")
            clsOrganization.DefaultPosView = Common.WebRequest.Data("DefaultPosView")
            clsOrganization.IsActive = Common.WebRequest.Data("IsActive")
            clsOrganization.PrintStickers = Common.WebRequest.Data("PrintStickers")
            clsOrganization.PrintQueueNumbers = Common.WebRequest.Data("PrintQueueNumbers")
            clsOrganization.DefaultHotButtonTemplateID = Common.WebRequest.Data("DefaultHotButtonTemplateID")

            clsOrganization.FOWSTStreetNumber = Common.WebRequest.Data("FOWSTStreetNumber")
            clsOrganization.FOWSTStreet = Common.WebRequest.Data("FOWSTStreet")
            clsOrganization.FOWSTCityID = Common.WebRequest.Data("FOWSTCityID")
            clsOrganization.FOWSTRegion = Common.WebRequest.Data("FOWSTRegion")
            clsOrganization.FOWSTCountryID = Common.WebRequest.Data("FOWSTCountryID")

            clsOrganization.FOWSTSubTypeID = Common.WebRequest.Data("FOWSTSubTypeID")
            clsOrganization.FOWSTStoreLocationTypeID = Common.WebRequest.Data("StoreLocationType")
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
            clsOrganization.EnforceZRead = Common.WebRequest.Data("EnforceZRead")

            clsOrganization.FOWSTIsFranchisee = Common.WebRequest.Data("FOWSTIsFranchisee")
            clsOrganization.FOWSTFranchisorID = Common.WebRequest.Data("FOWSTFranchisorID")

            clsOrganization.FOWSTIsInsideAMall = Common.WebRequest.Data("FOWSTIsInsideAMall")
            clsOrganization.Operates247 = Common.WebRequest.Data("Operates247")
            clsOrganization.FOWSTMallName = Common.WebRequest.Data("FOWSTMallName")
            clsOrganization.FtpServer = Common.WebRequest.Data("FtpServer")
            clsOrganization.FtpUsername = Common.WebRequest.Data("FtpUsername")
            clsOrganization.FtpPassword = Common.WebRequest.Data("FtpPassword")

            clsOrganization.FOWSTAyalaRecID = Common.WebRequest.Data("FOWSTAyalaRecID")
            clsOrganization.FOWSTAyalaTenantCode = Common.WebRequest.Data("FOWSTAyalaTenantCode")
            clsOrganization.FOWSTAyalaTenantName = Common.WebRequest.Data("FOWSTAyalaTenantName")
            clsOrganization.FOWSTAyalaTenantNameForDBF = Common.WebRequest.Data("FOWSTAyalaTenantNameForDBF")

            clsOrganization.FOWSTAyalaMerchantsLeaseOutNumber = Common.WebRequest.Data("FOWSTAyalaMerchantsLeaseOutNumber")

            clsOrganization.FOWSTSMRecID = Common.WebRequest.Data("FOWSTSMRecID")
            clsOrganization.FOWSTSMMallCode = Common.WebRequest.Data("FOWSTSMMallCode")
            clsOrganization.FOWSTSMMallTenantNo = Common.WebRequest.Data("FOWSTSMMallTenantNo")
            clsOrganization.FOWSTSMMallClassCode = Common.WebRequest.Data("FOWSTSMMallClassCode")
            clsOrganization.FOWSTSMMallTradeCode = Common.WebRequest.Data("FOWSTSMMallTradeCode")
            clsOrganization.FOWSTSMMallOutletNo = Common.WebRequest.Data("FOWSTSMMallOutletNo")
            clsOrganization.FOWSTSMMallSaleType = Common.WebRequest.Data("FOWSTSMMallSaleType")

            clsOrganization.FOWSTMegaworldRecID = Common.WebRequest.Data("FOWSTMegaworldRecID")
            clsOrganization.FOWSTMegaworldTenantCode = Common.WebRequest.Data("FOWSTMegaworldTenantCode")
            clsOrganization.FOWSTMegaworldSaleType = Common.WebRequest.Data("FOWSTMegaworldSaleType")
            clsOrganization.FOWSTMegaworldNoOfPOSTerminal = Common.WebRequest.Data("FOWSTMegaworldNoOfPOSTerminal")

            clsOrganization.RobinsonTenantId = Common.WebRequest.Data("RobinsonTenantId")
            clsOrganization.RobinsonPosTerminalNum = Common.WebRequest.Data("RobinsonPosTerminalNum")

            clsOrganization.PITXTenantId = Common.WebRequest.Data("PITXTenantId")
            clsOrganization.PITXPosTerminalID = Common.WebRequest.Data("PITXPosTerminalID")

            clsOrganization.FOWSTIsIncludeReport = CInt(Common.WebRequest.Data("FOWSTIsIncludeToReport"))
            clsOrganization.FOWSTAccountID = Common.WebRequest.Data("FOWSTAccountID")
            clsOrganization.FOWSTStoreID = Common.WebRequest.Data("FOWSTStoreID")
            clsOrganization.FOWSTAccountID2 = Common.WebRequest.Data("FOWSTAccountID2")
            clsOrganization.FOWSTStoreID2 = Common.WebRequest.Data("FOWSTStoreID2")
            clsOrganization.PosVendorID = Common.WebRequest.Data("PosVendorID")

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
            clsOrganization.EnableHotButtonsContextualMenu = CBool2(Common.WebRequest.AllData("EnableHotButtonsContextualMenu"))
            clsOrganization.ReceiptSettingsID = Common.WebRequest.AllData("FOWSTReceiptSettingsID")
            clsOrganization.PriceLevels = Common.WebRequest.AllData("PriceLevels")
            clsOrganization.RemoveAssignedEmp = Common.WebRequest.AllData("RemoveAssignedEmp")

            strReturn = JsonDictionary.Serialize("OrganizationFOWSTData", clsOrganization.SaveOrgFOWST())

            Return strReturn

        End Function

        Function CheckIfAutoApproveFranchisorOrFranchiseeRequest()
            Dim CompanyID As String = CStr2(Common.WebRequest.Data("CompanyID"))

            If String.IsNullOrWhiteSpace(CompanyID) Then
                Return JsonDictionary.Serialize("NotASubscriber", True)
            Else
                Return JsonDictionary.Serialize("Result", WebRequest.Instance.Business.LibraryOrganization.CheckIfAutoApproveFranchisorOrFranchiseeRequest(CompanyID))
            End If

        End Function

        <HttpPost()>
        Function getOrgRepList() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgRep() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgRepID")

            strReturn = JsonDictionary.Serialize("OrganizationRepData", IIf(clsOrganization.SetInActiveOrgRep(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationEmployee() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim ppol As New CRMDashboard

            strID = Common.WebRequest.Data("ID")
            Dim pInfo As DataTable = clsOrganization.getOrganizationEmpInfo(strID)
            Dim peopleid As String = pInfo.Rows(0)("PeopleID").ToString()
            pOut.Add("OrgEmployeeData", pInfo)

            Dim pSql As String = "SELECT rel.*, " & vbCrLf &
                                "   CASE WHEN ISNULL(convert(varchar(max), rel.DirectEmployerID), '') <> '' THEN (" & vbCrLf &
                                "       SELECT CompanyName FROM LibraryOrganizationSupplier as sup " & vbCrLf &
                                "           LEFT JOIN MobizLibraryOrganization as org ON org.ID = sup.SupplierID  " & vbCrLf &
                                "           WHERE sup.ID = rel.DirectEmployerID " & vbCrLf &
                                "       UNION ALL " & vbCrLf &
                                "       SELECT CompanyName FROM MobizLibraryOrganization as org " & vbCrLf &
                                "           WHERE org.ID = rel.DirectEmployerID) " & vbCrLf &
                                "   ELSE '' END as DirectEmployerName, " & vbCrLf &
                                "   CASE WHEN ISNULL(convert(varchar(max), rel.ManpowerAgencyClientID), '') <> '' THEN (" & vbCrLf &
                                "       SELECT CompanyName FROM LibraryOrganizationSupplier as sup " & vbCrLf &
                                "           LEFT JOIN MobizLibraryOrganization as org ON org.ID = sup.SupplierID  " & vbCrLf &
                                "           WHERE sup.ID = rel.ManpowerAgencyClientID " & vbCrLf &
                                "       UNION ALL " & vbCrLf &
                                "       SELECT CompanyName FROM MobizLibraryOrganization as org " & vbCrLf &
                                "           WHERE org.ID = rel.ManpowerAgencyClientID) " & vbCrLf &
                                "   ELSE '' END as ManpowerAgencyName, " & vbCrLf &
                                "   CASE WHEN ISNULL(convert(varchar(max), rel.AssignedThirdPartyClientID), '') <> '' THEN ( " & vbCrLf &
                                "       SELECT CompanyName FROM MobizLibraryOrganization as org " & vbCrLf &
                                "           WHERE org.ID = rel.AssignedThirdPartyClientID) " & vbCrLf &
                                "   ELSE '' END as AssignedThirdPartyClientName " & vbCrLf &
                                "FROM MobizLibraryEmployeeOrganizationRelationship as rel " & vbCrLf &
                                "WHERE EmployeeID = " & ReadyString(strID) & vbCrLf

            pOut.Add("Relations", Common.WebRequest.ProData.Execute(pSql))
            ppol.CurrentModule = "crm"
            pOut.Add("Telephone", ppol.GetProspectPeopleTel(peopleid))
            pOut.Add("Mobile", ppol.GetProspectPeopleMobile(peopleid))
            pOut.Add("Email", ppol.GetProspectPeopleEmail(peopleid))

            Return pOut.Serialize()

        End Function

        <HttpPost()>
        Function GetEmployeeFowst() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeFowst())

        End Function

        <HttpPost()>
        Function GetEmployeeSecurityRole() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeSecurityRole())

        End Function

        <HttpPost()>
        Function GetEmployeeWorkSchedule() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeWorkSchedule())

        End Function

        <HttpPost()>
        Function GetEmployeePayType() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String
            Dim pDTPayType As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pJson As New JsonDictionary()

            pDTPayType = clsOrganization.GetEmployeePayType()
            pJson.Add("Results", pDTPayType)

            If pDTPayType.Rows.Count > 0 Then
                pJson.Add("Deminimis", clsOrganization.GetDeminimis(CStr2(pDTPayType.Rows(0).Item("ID"))))
            End If

            strReturn = pJson.Serialize()

            Return strReturn

        End Function

        Function GetDeminimisByPayType() As String

            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Deminimis", clsOrganization.GetDeminimis(CStr2(Common.WebRequest.Data("PayTypeID"))))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetEmployeeIDs() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeIDs())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetEmployeeBankAccount() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Results", clsOrganization.GetEmployeeBankAccount())

            Return strReturn
        End Function

        <HttpPost()>
        Function SaveOrganizationEmployee() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            clsOrganization.SystemGeneratedID = Common.WebRequest.Data("SystemGeneratedID")
            clsOrganization.CompanyID = Common.WebRequest.Data("CompanyID")
            clsOrganization.DateHired = Common.WebRequest.Data("DateHired")
            clsOrganization.IsDeactivated = Common.WebRequest.Data("IsDeactivated")
            clsOrganization.DeactivationDate = Common.WebRequest.Data("DeactivationDate")
            clsOrganization.DeactivationReason = Common.WebRequest.Data("DeactivationReason")
            clsOrganization.Comments = Common.WebRequest.Data("Comments")
            clsOrganization.IsBan = Common.WebRequest.Data("IsBan")
            clsOrganization.IsConfirmed = Common.WebRequest.Data("IsConfirm")
            clsOrganization.RoleTag = Common.WebRequest.Data("RoleTag")
            clsOrganization.IsDirectHired = Common.WebRequest.Data("IsDirectHired")
            clsOrganization.IsSelfEmployed = Common.WebRequest.Data("IsSelfEmployed")
            clsOrganization.ManpowerAgencyID = Common.WebRequest.Data("ManpowerID")
            clsOrganization.VendorID = Common.WebRequest.Data("VendorID")
            clsOrganization.CoUserID = Common.WebRequest.Data("CoUserID")
            clsOrganization.ThirdPartySystemName = Common.WebRequest.Data("ThirdPartySystemName")
            clsOrganization.ThirdPartySystemID = Common.WebRequest.Data("ThirdPartySystemID")
            clsOrganization.PayFrequency = Common.WebRequest.Data("PayFrequency")
            clsOrganization.WorkScheduleID = Common.WebRequest.Data("WorkScheduleID")
            clsOrganization.PayTypeID = Common.WebRequest.Data("PayTypeID")

            clsOrganization.BankID = CStr2(Common.WebRequest.Data("BankID"))
            clsOrganization.BankBranch = CStr2(Common.WebRequest.Data("BankBranch"))
            clsOrganization.AccountName = CStr2(Common.WebRequest.Data("AccountName"))
            clsOrganization.AccountNo = CStr2(Common.WebRequest.Data("AccountNo"))
            clsOrganization.BankRole = CStr2(Common.WebRequest.Data("BankRole"))
            clsOrganization.IsDefaultBank = CBool2(Common.WebRequest.Data("IsDefaultBank"))

            clsOrganization.Username = CStr2(Common.WebRequest.Data("Username"))
            clsOrganization.Userpassword = CStr2(Common.WebRequest.Data("Userpassword"))
            clsOrganization.ApprovedResetMobizAccBy = CStr2(Common.WebRequest.Data("ApprovedResetMobizAccBy"))

            clsOrganization.PersonFirstName = CStr2(Common.WebRequest.Data("FirstName"))
            clsOrganization.PersonMiddleName = CStr2(Common.WebRequest.Data("MiddleName"))
            clsOrganization.PersonSurname = CStr2(Common.WebRequest.Data("LastName"))

            clsOrganization.ObjTelephone = Common.WebRequest.AllData("Telephone")
            clsOrganization.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsOrganization.ObjEmail = Common.WebRequest.AllData("Email")

            If Not CStr2(Common.WebRequest.Data("DepartmentID")) = "" Then
                clsOrganization.DepartmentID = CStr2(Common.WebRequest.Data("DepartmentID"))
            End If

            If Not CStr2(Common.WebRequest.Data("TeamID")) = "" Then
                clsOrganization.TeamID = CStr2(Common.WebRequest.Data("TeamID"))
            End If

            strReturn = JsonDictionary.Serialize("OrgEmployeeData", clsOrganization.SaveOrganizationEmployee())

            Return strReturn

        End Function

        Function SearchPersonOrganizationRelationship() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim type As String = Common.WebRequest.Data("Type")
            Dim pSql As String = " SELECT * FROM (SELECT orgsup.ID AS [ID] " & vbCrLf &
                                ", sup.IsOrganization " & vbCrLf &
                                ", orgsup.SupplierID AS [SupplierID] " & vbCrLf &
                                ", (CASE sup.IsOrganization      WHEN 1 THEN  sup.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(sup.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(sup.PersonSuffixName)),', '), '')  END) AS [SupplierName] " & vbCrLf &
                                " FROM LibraryOrganizationSupplier orgsup " & vbCrLf &
                                " INNER JOIN MobizLibraryOrganization sup ON (orgsup.SupplierID = sup.ID) " & vbCrLf &
                                IIf(type = "partner", "", " WHERE ISNULL(orgsup.TaggedAs, '') = " & ReadyString(type)) & vbCrLf &
                                ") as t ORDER BY SupplierName ASC"

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        <HttpPost()>
        Function CheckOrganizationCustomer() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgCustomerData", clsOrganization.getOrganizationCustInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationCustomer() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgSupplierData", clsOrganization.getOrganizationSupplierInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationSupplier() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            clsOrganization.CreditTermsID = Common.WebRequest.Data("CreditTermsID")
            clsOrganization.CreditLimit = Common.WebRequest.Data("CreditLimit")
            clsOrganization.CreditBegBalance = Common.WebRequest.Data("CreditBegBalance")
            clsOrganization.CreditBalanceAsOf = Common.WebRequest.Data("CreditBalanceAsOf")

            strReturn = JsonDictionary.Serialize("OrgSupplierData", clsOrganization.SaveOrganizationSupplier())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationBrand() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("OrgBrandData", clsOrganization.getOrganizationBrandList())

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationBrandInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgBrandData", clsOrganization.getOrganizationBrandInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationBrand() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("OrgProductData", Convert.ToInt32(clsOrganization.getOrgCompanyWideProductCount().Rows(0).Item("ProductCount")).ToString("#,##0"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductList() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String
            Dim strSearchString As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ProductID")
            strReturn = JsonDictionary.Serialize("OrgProductMeasurementData", clsOrganization.getOrganizationProductMeasurementList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetOrganizationProductPriceBreakList() As String
            Dim clsOrganization As New Organization
            Dim pProductID As String
            Dim strReturn As String
            Dim pID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pProductID = Common.WebRequest.Data("ProductID")
            pID = Common.WebRequest.Data("ID")
            If IsGuid(pID) = False Then
                pID = Nothing
            End If
            strReturn = JsonDictionary.Serialize("OrgProductPriceBreakData", clsOrganization.GetOrganizationProductPriceBreakList(pProductID, pID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductMeasurementInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ProductID")
            Dim isactive As Integer = Common.WebRequest.Data("IsActive")
            Dim pJson As New JsonDictionary()
            Dim pSecurityAccess As New SecurityAccess()
            Dim pShowLandedCost = pSecurityAccess.CheckAccessRights(WebSession.Instance.PeopleUserID, "Access Factory, Freight, and Landed Cost in Universal Product Information", False)
            pJson.Add("OrgProductPriceData", clsOrganization.getOrganizationProductPriceList(strID, pShowLandedCost))
            pJson.Add("VendorPrices", WebRequest.Instance.Business.LibraryStock.VendorPrices(strID, isactive))
            Dim pDR = WebRequest.Instance.Business.LibraryStock.GetFirstUnit(strID)
            If pDR IsNot Nothing Then
                pJson.Add("Unit", pDR)
            End If
            strReturn = pJson.Serialize()

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrganizationProductPriceInfo() As String
            Dim strID As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

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
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim strProductID As String = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.GetOrganizationProductMeasurement(strProductID))

            Return strReturn

        End Function

        <HttpPost()>
        Public Function GetOrganizationProductMeasurementBarcode() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim strMeasurementID As String = Common.WebRequest.Data("MeasurementID")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.getOrganizationProductMeasurementBarcodeList(strMeasurementID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationProduct() As String
            Dim clsOrganization As New Organization
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            clsOrganization.IsVatExempt = Common.WebRequest.Data("IsVatExempt")
            clsOrganization.MaxAllowableDiscountableAmount = Common.WebRequest.Data("MaxAllowableDiscountableAmount")
            clsOrganization.ItemBasedMaxAllowableDiscount = Common.WebRequest.Data("ItemBasedMaxAllowableDiscount")
            clsOrganization.DiscountedPriceAction = Common.WebRequest.Data("DiscountedPriceAction")
            clsOrganization.ProductAvailableToMobile = Common.WebRequest.Data("ProductAvailableToMobile")
            clsOrganization.IsInventoryCountItem = Common.WebRequest.Data("IsInventoryCountItem")
            clsOrganization.PaymentTermID = Common.WebRequest.Data("PaymentTermID")
            clsOrganization.IsPromoItem = Common.WebRequest.Data("IsPromoItem")
            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.SaveOrganizationProduct())

            '''Save Product info
            Dim pDR = WebRequest.Instance.Business.LibraryStock.GetByID(clsOrganization.ProductID)
            If Common.WebRequest.Data("OrgID").Trim() = Common.WebRequest.Data("UploadedByID").ToString() _
                OrElse pDR Is Nothing _
                OrElse String.IsNullOrWhiteSpace(CStr2(pDR("UploadedByID"))) Then

                clsPIMProduct.ID = Common.WebRequest.Data("ProductID")
                clsPIMProduct.OrganizationID = Common.WebRequest.Data("OrgID")
                clsPIMProduct.Brand = Common.WebRequest.Data("Brand")
                clsPIMProduct.IsAsset = Common.WebRequest.Data("IsAsset")
                clsPIMProduct.IsEcommerceVendorAllowedToSell = Common.WebRequest.Data("IsEcommerceVendorAllowedToSell")
                clsPIMProduct.ProductNo = Common.WebRequest.Data("ProductNo")
                clsPIMProduct.ProductName = Common.WebRequest.Data("ProductName")
                clsPIMProduct.ModelNo = Common.WebRequest.Data("ModelNo")
                clsPIMProduct.GroupNo = Common.WebRequest.Data("GroupNo")
                clsPIMProduct.Division = Common.WebRequest.Data("Division")
                clsPIMProduct.Category = Common.WebRequest.Data("Category")
                clsPIMProduct.MarketingText = Common.WebRequest.Data("MarketingText")
                clsPIMProduct.Warranty = Common.WebRequest.Data("Warranty")
                clsPIMProduct.SaveProductInfo()
                If CBool2(Common.WebRequest.Data("IsAsset")) Then
                    Dim p As New InsertUpdate("LibraryRentableAssets")
                    Dim pSql As String = "SELECT ID FROM LibraryRentableAssets WHERE ProductID = " & ReadyString(Common.WebRequest.Data("ProductID").ToString())
                    Dim vRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    If vRows.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("ProductID") = ReadyString(Common.WebRequest.Data("ProductID").ToString())
                        p("TransID") = "NULL"
                        p("Status") = ReadyString("Available")
                        p("LastUpdated") = "getdate()"
                        p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                End If
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationProductMeasurement() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            clsOrganization.IsTimeBaseUnit = Common.WebRequest.Data("IsTimeBaseUnit")
            clsOrganization.TimeBaseUnitDescription = Common.WebRequest.Data("TimeBaseUnit")
            clsOrganization.OrgProductMeasurementBarcodeList = Common.WebRequest.AllData("OrgProductMeasurementBarcodeList")

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.SaveOrganizationProductMeasurement())

            Return strReturn

        End Function

        Function SaveBarcode() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("StockMeasurementsID")
            clsOrganization.BarcodeID = Common.WebRequest.Data("BarcodeID")
            clsOrganization.Barcode = Common.WebRequest.Data("Barcode")
            clsOrganization.BarcodeType = Common.WebRequest.Data("BarcodeType")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.UnitMeasureID = Common.WebRequest.Data("UnitMeasureID")
            clsOrganization.Qty = Common.WebRequest.Data("Quantity")
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
            clsOrganization.IsTimeBaseUnit = Common.WebRequest.Data("IsTimeBaseUnit")
            clsOrganization.TimeBaseUnitDescription = Common.WebRequest.Data("TimeBaseUnit")

            Return clsOrganization.SaveBarcode()
        End Function

        Function DeleteBarcode() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "DELETE FROM MobizLibraryPIMProductMeasurementBarcode WHERE ID = " & ReadyString(Common.WebRequest.Data("BarcodeID").ToString())
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function SaveBarcodeType() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                If Common.WebRequest.Data("State") = "remove" Then
                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryProprietaryBarcodeType WHERE ID = " & ReadyString(Common.WebRequest.Data("ID").ToString()))
                Else
                    Dim p As New InsertUpdate("LibraryProprietaryBarcodeType")

                    p("BarcodeType") = ReadyString(Common.WebRequest.Data("Type"))
                    p("StickerDimensions") = ReadyString(Common.WebRequest.Data("Dimensions"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = "getdate()"

                    If Common.WebRequest.Data("ID").ToString() = "" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID").ToString())))
                    End If
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetBarcodeType() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT ID, BarcodeType FROM LibraryProprietaryBarcodeType"
            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        <HttpPost>
        Public Function DeleteProductUnitMeasure() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pID = Common.WebRequest.Data("ID") ' MobizLibraryPIMProductMeasurement.ID
                Dim pSql = "DELETE FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf &
                    " WHERE StockMeasurementsID = " & ReadyString(pID)

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    " WHERE MobizLibraryPIMProductMeasurement.ID = " & ReadyString(pID)

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function SaveOrganizationProductPrice() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("ID")
            clsOrganization.ProductID = Common.WebRequest.Data("ProductID")
            clsOrganization.PriceLevelID = Common.WebRequest.Data("PriceLevelID")
            clsOrganization.Price = CDec2(Common.WebRequest.Data("Price"))

            strReturn = JsonDictionary.Serialize("OrgProductData", clsOrganization.SaveOrganizationProductPrice())
            WebRequest.Instance.Business.LibraryStock.UpdateParentProductPrice(clsOrganization.ProductID, WebRequest.Instance.OrgID)

            Return strReturn

        End Function

        <HttpPost()>
        Function DeleteOrganizationProductPrice() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pID = Common.WebRequest.Data("ID")
            Dim pSql = "DELETE FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                " WHERE ID = " & ReadyString(pID)
            WebRequest.Instance.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost()>
        Function SaveOrganizationProductBOM() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgOwnerInfo", clsOrganization.getOrgOwnerInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgOwner() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgOwnerID")

            strReturn = JsonDictionary.Serialize("OrgOwnerData", IIf(clsOrganization.RemoveOrgOwner(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationFOWSTProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgFowstProductData", clsOrganization.getOrganizationFowstProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationFowstProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgCustProductData", clsOrganization.getOrganizationCustomerProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationCustomerProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebSession.Instance.OrganizationFowstID = Common.WebRequest.Data("FowstID")

            strReturn = JsonDictionary.Serialize("Success", "TRUE")

            Return strReturn
        End Function

        <HttpPost()>
        Function CheckOrganizationCustomerFOWSTProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgCustFowstProductData", clsOrganization.getOrganizationCustomerFowstProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrganizationCustomerFowstProduct() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.getOrgHoldingCompanyInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgHoldingCompany() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrgAffiliationData", clsOrganization.getOrgAffliatesInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgAffiliates() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.SaveWorkingHours())

        End Function

        Function RemoveEmployeeWorkSchedule() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.RemoveEmployeeWorkSchedule())
        End Function

        Function GetAllSchedule() As String

            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.SaveEmployeeWorkingHours())

        End Function

        Function GetEmployeeSchedule() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("OrgData", clsOrganization.AddOrgOwnerProductList(Common.WebRequest.Data("ID")))

            Return strReturn

        End Function

        <HttpPost>
        Public Function SaveOrganizationProductPriceBreak() As String

            Dim clsOrganization As New Organization
            Dim pProductID As String
            Dim pUnitMeasureID As String
            Dim pQty As Decimal
            Dim pDiscount As Decimal
            Dim pDiscountType As Byte
            Dim pResult As Boolean

            Try
                pProductID = Common.WebRequest.Data("ProductID")
                If IsGuid(pProductID) = False Then
                    Return JsonDictionary.Serialize("Error", "ProductID must be guid. Reference: vbM8DDXU9H8F!")
                End If
                pProductID = ReadyGuid(pProductID)

                pUnitMeasureID = Common.WebRequest.Data("UnitMeasureID")
                If IsGuid(pUnitMeasureID) = False Then
                    Return JsonDictionary.Serialize("Error", "UnitMeasureID must be guid. Reference: vbEU84XY6FTC!")
                End If
                pUnitMeasureID = ReadyGuid(pUnitMeasureID)

                If Decimal.TryParse(Common.WebRequest.Data("Qty"), pQty) = False Then
                    Return JsonDictionary.Serialize("Error", "Qty must be decimal. Reference: vbGH5JDW7VK6!")
                End If
                If pQty <= 0 Then
                    Return JsonDictionary.Serialize("Error", "Qty must be greater than zero. Reference: vb8XSJS5F46E!")
                End If

                If Decimal.TryParse(Common.WebRequest.Data("Discount"), pDiscount) = False Then
                    Return JsonDictionary.Serialize("Error", "Discount must be decimal. Reference: vbCQB6MPGX8T!")
                End If
                If pDiscount <= 0 Then
                    Return JsonDictionary.Serialize("Error", "Discount must be greater than zero. Reference: vb5CSUS4FR8P!")
                End If
                If CInt2(Common.WebRequest.Data("DiscountType")) <> 0 Then
                    pDiscountType = 1
                End If

                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pResult = WebRequest.Instance.Business.LibraryStock.AddPriceBreak(vOrgID:=WebRequest.Instance.OrgID, _
                    vProductID:=pProductID,
                    vUnitMeasureID:=pUnitMeasureID,
                    vQty:=pQty,
                    vDiscount:=pDiscount,
                    vDiscountType:=pDiscountType)

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

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

        <HttpPost()>
        Function GetOrgFOWSTSectionBinList() As String
            Dim pDT As DataTable
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            clsOrganization.SectionID = Common.WebRequest.Data("SectionID")
            Return JsonDictionary.Serialize("Data", clsOrganization.GetOrgFOWSTSectionBinList())

        End Function
        <HttpPost()>
        Function GetOrgFOWSTSectionPeopleList() As String
            Dim pDT As DataTable
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            clsOrganization.SectionID = Common.WebRequest.Data("SectionID")
            Return JsonDictionary.Serialize("Data", clsOrganization.GetOrgFOWSTSectionPeopleList())

        End Function
        <HttpPost()>
        Function SaveFowstBinSection() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("FowstBinID")
            clsOrganization.SectionID = Common.WebRequest.Data("SectionID")

            strReturn = JsonDictionary.Serialize("Result", clsOrganization.SaveFowstBinSection())

            Return strReturn

        End Function
        <HttpPost()>
        Function RemoveFowstBinSection() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("Result", clsOrganization.RemoveFowstBinSection())

            Return strReturn

        End Function
        <HttpPost()>
        Function SaveFowstSectionPeople() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.FowstID = Common.WebRequest.Data("FowstID")
            clsOrganization.SectionID = Common.WebRequest.Data("SectionID")
            clsOrganization.PeopleID = Common.WebRequest.Data("PeopleID")

            strReturn = JsonDictionary.Serialize("Result", clsOrganization.SaveFowstSectionPeople())

            Return strReturn

        End Function
        <HttpPost()>
        Function RemoveFowstSectionPeople() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("Result", clsOrganization.RemoveFowstSectionPeople())

            Return strReturn

        End Function

        Function GetFOWSTQuota() As String
            Dim clsOrganization As New Organization
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetFOWSTQuota)
        End Function

        <HttpPost>
        Function SaveOrgLogo() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("MobizLibraryOrganization")
                Dim files As HttpFileCollectionBase = Request.Files
                Dim folderName As String = Common.ServerPath & "Content\OrganizationLogo"

                If Not System.IO.Directory.Exists(folderName) Then
                    System.IO.Directory.CreateDirectory(folderName)
                End If

                If files.Count > 0 Then
                    Dim f As HttpPostedFileBase = files(0)
                    Dim filename As String = WebSession.Instance.OrganizationID & "." & Split(f.FileName, ".")(1)
                    Dim pSourceURL As String = folderName & "\" & filename

                    f.SaveAs(pSourceURL)

                    p("LogoFileName") = ReadyString(filename)

                Else
                    p("LogoFileName") = ReadyString("") ' to remove logo
                End If

                p("LastUpdated") = "getdate()"

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = ") & ReadyString(WebSession.Instance.OrganizationID))

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost>
        Function GetPIMPictures() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT *, 'edit' as State FROM LibraryPIMPictures WHERE ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & vbCrLf &
                                " AND ISNULL(IsFromVendor, 0) = " & IIf(Common.WebRequest.Data("IsFromVendor") <> Nothing, CInt2(Common.WebRequest.Data("IsFromVendor")), 0) & vbCrLf &
                                IIf(Common.WebRequest.Data("VendorID") <> Nothing, " AND VendorID = " & ReadyString(Common.WebRequest.Data("VendorID")), "") & vbCrLf &
                                    " AND Size = 'medium' ORDER BY IsDefault DESC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        <HttpPost>
        Function GetBulkPIMPictures()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return "" ' WebRequest.InvalidOrganization
            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String
                Dim clsorgshort As New OrganizationShort

                For Each pim As Object In data
                    pSql = "SELECT ISNULL(FileName, '') as FileName FROM LibraryPIMPictures WHERE ProductID = " & ReadyString(pim("ID")) & vbCrLf &
                            " AND ISNULL(IsFromVendor, 0) = 0 " & vbCrLf &
                            " AND Size = " & ReadyString(Common.WebRequest.Data("size")) & IIf(CBool2(Common.WebRequest.Data("IsDefault")), " AND IsDefault = 1 ", "") & vbCrLf &
                            " ORDER BY IsDefault DESC "

                    Dim pdt As DataTable = Common.WebRequest.ProData.Execute(pSql)

                    pim.Remove("Pics")

                    If pdt.Rows.Count > 0 Then
                        pim.Add("Pics", jss.Deserialize(Of Object)(clsorgshort.GetJson(pdt)))
                    Else
                        pim.Add("Pics", "")
                    End If
                Next

                Return Json(New With {Key .data = data}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog("Getting POS picture error msg: " & ex.Message)
                Return ex.Message
            End Try
        End Function

        <HttpPost()>
        Function SaveImagePIM()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                'Initialize all variables here
                Dim pDirectory As String
                Dim ViewID As String = Common.WebRequest.Data("ViewID")
                Dim imageData As String = Common.WebRequest.Data("imageData")
                Dim size As String = Common.WebRequest.Data("Size")
                Dim state As String = Common.WebRequest.Data("State")
                Dim IsDefault As String = Common.WebRequest.Data("IsDefault")
                Dim prefixID As String = Common.WebRequest.Data("PrefixID")
                Dim groupimageID As String = Common.WebRequest.Data("GroupImageID")
                Dim isFromVendor As Boolean = CBool2(Common.WebRequest.Data("isFromVendor"))
                Dim folderBig As String = Common.ServerPath & "Content\MobizImagePIM\" & WebRequest.Instance.OrgID & "\Big" & IIf(isFromVendor, "\" & Common.WebRequest.Data("VendorID"), "")
                Dim folderMedium As String = Common.ServerPath & "Content\MobizImagePIM\" & WebRequest.Instance.OrgID & "\Medium" & IIf(isFromVendor, "\" & Common.WebRequest.Data("VendorID"), "")
                Dim folderSmall As String = Common.ServerPath & "Content\MobizImagePIM\" & WebRequest.Instance.OrgID & "\Small" & IIf(isFromVendor, "\" & Common.WebRequest.Data("VendorID"), "")
                Dim folderOriginal As String = Common.ServerPath & "Content\MobizImagePIM\" & WebRequest.Instance.OrgID & "\Original" & IIf(isFromVendor, "\" & Common.WebRequest.Data("VendorID"), "")
                Dim location As String

                'Check if parent directory does exist
                If Not System.IO.Directory.Exists(folderBig) Then
                    IO.Directory.CreateDirectory(folderBig)
                End If
                If Not System.IO.Directory.Exists(folderMedium) Then
                    IO.Directory.CreateDirectory(folderMedium)
                End If
                If Not System.IO.Directory.Exists(folderSmall) Then
                    IO.Directory.CreateDirectory(folderSmall)
                End If
                If Not System.IO.Directory.Exists(folderOriginal) Then
                    IO.Directory.CreateDirectory(folderOriginal)
                End If

                'Check if product has a saved picture
                'pDirectory = Common.ServerPath & "Content\MobizImagePIM\"
                'If Not System.IO.Directory.Exists(pDirectory & ViewID) Then
                '    IO.Directory.CreateDirectory(pDirectory & ViewID)
                'End If

                Dim pSql As String
                Dim pDT As Object
                Dim filename As String

                Dim p As New InsertUpdate("LibraryPIMPictures")

                If size = "big" Then
                    location = folderBig
                ElseIf size = "medium" Then
                    location = folderMedium
                ElseIf size = "small" Then
                    location = folderSmall
                Else
                    location = folderOriginal
                End If

                Common.WebRequest.ProData.BeginTransaction()

                If state = "new" Then
                    pSql = "SELECT GroupImageID FROM LibraryPIMPictures WHERE ProductID = " & ReadyString(ViewID) & vbCrLf &
                                        " AND ISNULL(IsFromVendor, 0) = " & CInt2(isFromVendor) & vbCrLf &
                                        IIf(isFromVendor, " AND VendorID = " & ReadyString(Common.WebRequest.Data("VendorID")), "") & vbCrLf &
                                        " Group By GroupImageID"
                    pDT = Common.WebRequest.ProData.Execute(pSql).Rows
                    filename = prefixID.ToString() & "_" & size & ".jpg"

                    If pDT.Count <= 1 Then
                        p("IsDefault") = 1
                    Else
                        p("IsDefault") = 0
                    End If

                    p("IsFromVendor") = CInt2(isFromVendor)
                    p("VendorID") = IIf(isFromVendor, ReadyString(Common.WebRequest.Data("VendorID")), "NULL")
                    p("ProductID") = ReadyString(ViewID.ToString())
                    p("GroupImageID") = ReadyString(prefixID.ToString())
                    p("Size") = ReadyString(size)
                    p("FileName") = ReadyString(filename)
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("DateUploaded") = "getdate()"
                    'p("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID())
                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    Using imagePath As IO.FileStream = New IO.FileStream(location & "\" & filename, IO.FileMode.Create)
                        Using binaryWriting As IO.BinaryWriter = New IO.BinaryWriter(imagePath)
                            binaryWriting.Write(Convert.FromBase64String(imageData))
                            binaryWriting.Close()
                        End Using
                    End Using
                ElseIf state = "edit" Then
                    pSql = "SELECT TOP 1 GroupImageID FROM LibraryPIMPictures WHERE IsDefault = 1 AND ProductID = " & ReadyString(ViewID) & vbCrLf &
                            " AND ISNULL(IsFromVendor, 0) = " & CInt2(isFromVendor) & vbCrLf &
                            IIf(isFromVendor, " AND VendorID = " & ReadyString(Common.WebRequest.Data("VendorID")), "") & vbCrLf
                    pDT = Common.WebRequest.ProData.Execute(pSql).Rows

                    pSql = "UPDATE LibraryPIMPictures SET IsDefault = 0 WHERE GroupImageID = " & ReadyString(pDT(0)("GroupImageID").ToString())
                    Common.WebRequest.ProData.Execute(pSql)

                    p("IsDefault") = 1
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" GroupImageID = " & ReadyString(groupimageID)))
                Else
                    If CBool2(IsDefault) Then
                        pSql = "SELECT TOP 1 GroupImageID FROM LibraryPIMPictures WHERE ProductID = " & ReadyString(ViewID) & vbCrLf &
                            " AND ISNULL(IsFromVendor, 0) = " & CInt2(isFromVendor) & vbCrLf &
                            IIf(isFromVendor, " AND VendorID = " & ReadyString(Common.WebRequest.Data("VendorID")), "") & vbCrLf &
                            "AND GroupImageID <> " & ReadyString(groupimageID) & " Group By GroupImageID"
                        pDT = Common.WebRequest.ProData.Execute(pSql).Rows

                        If pDT.Count > 0 Then
                            pSql = "UPDATE LibraryPIMPictures SET IsDefault = 1 WHERE GroupImageID = " & ReadyString(pDT(0)("GroupImageID").ToString())
                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                    End If

                    pSql = "SELECT FileName, Size FROM LibraryPIMPictures WHERE ProductID = " & ReadyString(ViewID) & " AND GroupImageID = " & ReadyString(groupimageID) & vbCrLf &
                            " AND ISNULL(IsFromVendor, 0) = " & CInt2(isFromVendor) & vbCrLf &
                            IIf(isFromVendor, " AND VendorID = " & ReadyString(Common.WebRequest.Data("VendorID")), "") & vbCrLf
                    pDT = Common.WebRequest.ProData.Execute(pSql).Rows

                    For Each f As Object In pDT
                        System.IO.File.Delete(Common.ServerPath & "Content\MobizImagePIM\" & WebRequest.Instance.OrgID & "\" & f("Size") & IIf(isFromVendor, "\" & Common.WebRequest.Data("VendorID"), "") & "\" & f("FileName"))
                    Next f

                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryPIMPictures WHERE GroupImageID = " & ReadyString(groupimageID))
                End If

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()

                'Check whether or not a default file does exist or a number of files
                'If System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg") Then

                '    'Check if there is a slot available from 1-5
                '    Dim number As Integer = 2
                '    Dim savedCheck As Boolean = False
                '    Do While number <= 5 And savedCheck = False
                '        If Not System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-" + number.ToString + ".jpg") Then

                '            'Save image to the path
                '            Using imagePath As IO.FileStream = New IO.FileStream(pDirectory & ViewID & "\" & ViewID & "-100x100-" + number.ToString + ".jpg", IO.FileMode.Create)
                '                Using binaryWriting As IO.BinaryWriter = New IO.BinaryWriter(imagePath)
                '                    binaryWriting.Write(Convert.FromBase64String(imageData))
                '                    binaryWriting.Close()
                '                End Using
                '            End Using

                '            savedCheck = True
                '        End If
                '        number += 1
                '    Loop

                'Else

                '    'Save image to the path
                '    Using imagePath As IO.FileStream = New IO.FileStream(pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg", IO.FileMode.Create)
                '        Using binaryWriting As IO.BinaryWriter = New IO.BinaryWriter(imagePath)
                '            binaryWriting.Write(Convert.FromBase64String(imageData))
                '            binaryWriting.Close()
                '        End Using
                '    End Using

                'End If

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost()>
        Function LoadImagePIM() As String

            'Initialize all variables here
            Dim pDirectory As String
            Dim readerResult As String
            Dim ViewID As String = Common.WebRequest.Data("ViewID")
            Dim caseOperator As String = Common.WebRequest.Data("conditionalOperator")
            Dim genControl As Random = New Random
            Dim imageObj As String
            Dim imageReceived As String
            Dim booleanController As Boolean = True
            Dim imageCounter As Integer


            'Check if parent directory does exist
            If System.IO.Directory.Exists(Common.ServerPath & "temp\MobizImagePIM") Then

                'Check if product has a saved picture folder
                pDirectory = Common.ServerPath & "temp\MobizImagePIM\"
                If System.IO.Directory.Exists(pDirectory & ViewID) Then

                    'Operations differ from the conditional operator
                    Select Case caseOperator
                        Case "selectioning"

                            'Check whether or not file does exist
                            If System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg") Then
                                readerResult = "<img class='selected' src='../temp/MobizImagePIM/" & ViewID & "/" & ViewID & "-100x100-default.jpg?version=" & genControl.Next(256, 512) & "' />"

                                'Check if there are other pictures to append
                                For number As Integer = 2 To 5
                                    If System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-" + number.ToString + ".jpg") Then
                                        readerResult = readerResult & "<img src='../temp/MobizImagePIM/" & ViewID & "/" & ViewID & "-100x100-" + number.ToString + ".jpg?version=" & genControl.Next(256, 512) & "' />"
                                    End If
                                Next

                                'Note: A version has been added inorder for the images not to be cached
                                Return readerResult
                            Else
                                readerResult = "NotExist"
                                Return readerResult
                            End If

                        Case "changing"

                            'Trim and process the string for validation
                            Dim imageName As String = Common.WebRequest.Data("imageData")
                            imageName = imageName.Remove(0, 22)
                            imageName = imageName.Replace("/", "\")
                            imageName = imageName.Remove(Len(imageName) - 12, 12)

                            'Check wether the default exist and change the picture filename
                            imageObj = pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg"
                            imageReceived = pDirectory & imageName
                            If System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg") And Not (imageReceived.Equals(imageObj)) Then
                                System.IO.File.Move(pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg", pDirectory & ViewID & "\" & ViewID & "-100x100-temp.jpg")
                                System.IO.File.Move(pDirectory & imageName, pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg")
                                System.IO.File.Move(pDirectory & ViewID & "\" & ViewID & "-100x100-temp.jpg", pDirectory & imageName)
                                readerResult = "DoneChange"
                                Return readerResult

                            ElseIf imageReceived.Equals(imageObj) Then
                                readerResult = "NotAllowed"
                                Return readerResult

                            Else
                                readerResult = "NotExist"
                                Return readerResult
                            End If

                        Case "deletion"

                            'Trim and process the string for validation
                            Dim imageName As String = Common.WebRequest.Data("imageData")
                            imageName = imageName.Remove(0, 22)
                            imageName = imageName.Replace("/", "\")
                            imageName = imageName.Remove(Len(imageName) - 12, 12)

                            'Check whether the picture exist and if the image is not the default image
                            imageObj = pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg"
                            imageReceived = pDirectory & imageName
                            Dim verb As Boolean = imageReceived.Equals(imageObj)
                            If System.IO.File.Exists(pDirectory & imageName) And (imageReceived.Equals(imageObj)) Then

                                'If photo is a default change the next picture to default
                                System.IO.File.Delete(pDirectory & imageName)
                                imageCounter = 2
                                Do While booleanController And imageCounter <= 5
                                    If System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-" + imageCounter.ToString + ".jpg") Then
                                        System.IO.File.Move(pDirectory & ViewID & "\" & ViewID & "-100x100-" + imageCounter.ToString + ".jpg", pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg")
                                        booleanController = False
                                    End If
                                    imageCounter += 1
                                Loop
                                readerResult = "DoneDeletion"
                                Return readerResult
                            ElseIf System.IO.File.Exists(pDirectory & imageName) Then
                                System.IO.File.Delete(pDirectory & imageName)
                                readerResult = "DoneDeletion"
                                Return readerResult
                            Else
                                readerResult = "NotExist"
                                Return readerResult
                            End If

                        Case "profileImg"

                            'Check whether the default picture exist
                            If System.IO.File.Exists(pDirectory & ViewID & "\" & ViewID & "-100x100-default.jpg") Then
                                readerResult = "../temp/MobizImagePIM/" & ViewID & "/" & ViewID & "-100x100-default.jpg?version=" & genControl.Next(256, 512)
                                Return readerResult
                            Else
                                readerResult = "NotExist"
                                Return readerResult
                            End If

                        Case Else
                            readerResult = "NotExist"
                            Return readerResult
                    End Select

                Else
                    readerResult = "NotExist"
                    Return readerResult
                End If

            Else
                readerResult = "NotExist"
                Return readerResult
            End If

        End Function

        Function DownloadOrganizationCustomers() As ActionResult
            Dim clsOrganization As New Organization
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            If Not System.IO.Directory.Exists(Common.ServerPath & "temp\Customers") Then
                System.IO.Directory.CreateDirectory(Common.ServerPath & "temp\Customers")
            End If

            '''get List of Customers
            pDT = New DataTable()
            clsOrganization.MaxReturnRow = 9999999
            pDT = clsOrganization.getOrganizationCustomerList

            If (pDT.Rows.Count > 0) Then
                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\Customers\CustomerList.xls")
            End If


            '''Download the File
            Dim pContentType As String
            Dim pSourceURL As String
            pSourceURL = Common.ServerPath & "temp\Customers\CustomerList.xls"
            pContentType = "application/xls"

            Return File(pSourceURL, pContentType, "CustomerList.xls")
        End Function

        Function DownloadCustomerList() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("p")
            Dim pSourceURL As String = Common.ServerPath & "temp\" & pFileTitle
            Dim pContentType As String = "application/csv"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        Function GetEmployeeDesignation() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT role.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as ModifiedBy, " & vbCrLf &
                                " (SELECT COUNT(ID) FROM LibraryOrganizationEmployee WHERE RoleTagID = role.ID) NoOfAssignedEmp " & vbCrLf &
                                " FROM LibraryDTRRole as role " & vbCrLf &
                                " LEFT JOIN MobizLibraryPeople as ppol ON role.UserID = ppol.ID " & vbCrLf &
                                " ORDER BY role.RoleName ASC "

            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        <HttpPost()>
        Function GetDataTable() As Object
            Dim clsOrganization As New Organization
            Dim strReturn As String
            Dim strSearchString As String
            Dim pDT As DataTable
            Dim mods As String = Request.QueryString.GetValues(1)(0).ToString() 'parameter mod

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                pDT = GetDataOfModule(mods, False)

                Dim pDTCountPage As DataTable = GetDataOfModule(mods, True)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsOrganization.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
                Return JsonDictionary.Serialize("Error", ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
            End Try
        End Function

        Function GetDataOfModule(ByVal mods As String, ByVal IsCountPage As Boolean) As DataTable
            Dim clsOrganization As New Organization
            Dim pDt As DataTable

            clsOrganization.OrganizationID = Request.QueryString.GetValues(0)(0).ToString() 'parameter OrgID
            clsOrganization.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsOrganization.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsOrganization.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsOrganization.IsCountPage = IsCountPage
            clsOrganization.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsOrganization.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()

            If Request.QueryString.Count = 3 Then
                clsOrganization.IsActive = Request.QueryString.Get("isdeactivate") ' for PIM
            End If

            If mods = "fowst" Then
                clsOrganization.IsActive = Request.Params("isactive")
                clsOrganization.FOWSTStatus = Request.Params("status")
                pDt = clsOrganization.GetFowst()
            ElseIf mods = "device" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetDevice()
            ElseIf mods = "receipt" Then
                pDt = clsOrganization.GetReceipt()
            ElseIf mods = "security" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetSecurityRole()
            ElseIf mods = "employee" Then
                pDt = clsOrganization.GetEmployee()
            ElseIf mods = "pim" Then
                pDt = clsOrganization.GetPIM()
            ElseIf mods = "pimbrands" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetPIMBrands()
            ElseIf mods = "pimcategories" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetPIMCategories()
            ElseIf mods = "printerrole" Then
                pDt = clsOrganization.GetPrinterRole()
            ElseIf mods = "hotbutton" Then
                pDt = clsOrganization.GetHotButton()
            ElseIf mods = "groupsettings" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetGroupSettings()
            ElseIf mods = "regionalgroupsettings" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetRegionalGroupSettings()
            ElseIf mods = "customer" Then
                pDt = clsOrganization.GetCustomer()
            ElseIf mods = "supplier" Then
                clsOrganization.IsActive = Request.Params("isactive")
                pDt = clsOrganization.GetSupplier()
            ElseIf mods = "salesteam" Then
                clsOrganization.Type = Request.Params.Get("type")
                pDt = clsOrganization.GetSalesTeam()
            ElseIf mods = "modifier" Then
                pDt = clsOrganization.GetItemModifier()
            ElseIf mods = "printers" Then
                pDt = clsOrganization.GetPrinters()
            ElseIf mods = "tableview" Then
                pDt = clsOrganization.GetTableView()
            ElseIf mods = "terms" Then
                pDt = clsOrganization.GetPaymentTerms()
            ElseIf mods = "pricelevel" Then
                pDt = clsOrganization.GetPriceLevelCustom()
            ElseIf mods = "salesrep" Then
                pDt = clsOrganization.GetSalesRep()
            ElseIf mods = "storecategories" Then
                pDt = clsOrganization.GetOnlineStoreCategory()
            ElseIf mods = "barcodetype" Then
                pDt = clsOrganization.GetBarcodeType()
            ElseIf mods = "centers" Then
                clsOrganization.IsActive = Request.Params("isactive")
                clsOrganization.Type = Request.Params("type")
                pDt = clsOrganization.GetCenterList()
            ElseIf mods = "holidays" Then
                clsOrganization.Type = Request.Params("type")
                pDt = clsOrganization.GetHolidays()
            ElseIf mods = "vdmmodels" Then
                pDt = clsOrganization.GetVDMModels()
            ElseIf mods = "vmmodelwithcontrollerlisting" Then
                pDt = clsOrganization.GetVMMainBoard()
            ElseIf mods = "vdmplanogramtemplate" Then
                pDt = clsOrganization.GetVMPlanogramTemplate()
            ElseIf mods = "vdmauthorizers" Then
                pDt = clsOrganization.GetVDMAuthorizers()
            ElseIf mods = "vdmphysicalbuttons" Then
                pDt = clsOrganization.GetVDMPhysicalButtons()
            ElseIf mods = "vdmricedispensingguides" Then
                pDt = clsOrganization.VDMRiceDispensingGuides()
            ElseIf mods = "vdmads" Then
                pDt = clsOrganization.GetVDMAds()
            ElseIf mods = "vouchers" Then
                clsOrganization.IsActive = Request.Params("status")
                pDt = clsOrganization.GetVouchers()
            ElseIf mods = "promocode" Then
                clsOrganization.IsActive = Request.Params("status")
                pDt = clsOrganization.GetPromoCampaignCode()
            End If

            Return pDt
        End Function

        Function SavePimCategoriesToCategoriesLibrary()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'kwaon tanan Unique Category Name sa MobizLibraryPIMProductVariableFeatures001 Table then isave sa LibraryPimCategories by arnie 12/7/2020
            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim pSql As String = " SELECT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category] " & vbCrLf &
                " FROM MobizLibraryPIMProduct" & vbCrLf &
                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " WHERE MobizLibraryPIMProductVariableFeatures001.C0020 NOT IN (SELECT Category FROM LibraryPimCategories) " & vbCrLf &
                " GROUP BY MobizLibraryPIMProductVariableFeatures001.C0020 " & vbCrLf

                Dim dt As DataTable = Common.WebRequest.ProData.Execute(pSql)

                For Each d As Object In dt.Rows
                    Dim p As New InsertUpdate("LibraryPimCategories")

                    p("Category") = ReadyString(d("Category"))

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" Category = " & ReadyString(d("Category")))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("LastUpdated") = "getdate()"
                        p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("IsActive") = 1
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SavePimBrandsToBrandsLibrary()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'kwaon tanan Unique Brand Name sa PIM Table then isave sa LibraryPimBrands by arnie 12/7/2020
            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim pSql As String = " SELECT prd.Brand AS [Brand] " & vbCrLf &
                " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                " GROUP BY prd.Brand " & vbCrLf

                Dim dt As DataTable = Common.WebRequest.ProData.Execute(pSql)

                For Each d As Object In dt.Rows
                    Dim p As New InsertUpdate("LibraryPimBrands")

                    p("Brand") = ReadyString(d("Brand"))

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" Brand = " & ReadyString(d("Brand")))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("LastUpdated") = "getdate()"
                        p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("IsActive") = 1
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost>
        Public Function GetPrinterByDevicePrinterFowstID() As String

            Dim pID As String
            Dim pDR As DataRow
            Dim pJson As JsonDictionary
            Dim pDtFowst As DataTable
            Dim pDtCategories As DataTable
            Dim pPrinterID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pJson = New JsonDictionary()

            pID = Common.WebRequest.Data("ID")
            Dim isnewsetup As Boolean = CBool2(Common.WebRequest.Data("IsNewSetup"))
            If String.IsNullOrWhiteSpace(pID) = False Then
                If isnewsetup Then
                    pDR = Common.WebRequest.ProData.ExecuteRow("SELECT * FROM LibraryDevicePrinter WHERE ID = " & ReadyString(pID))
                Else
                    pDR = WebRequest.Instance.Business.LibraryPrinters.GetByDevicePrinterFowstID(pID)
                End If

                pJson.Add("Master", pDR)

                Dim pSql As String = "SELECT MobizLibraryOrganizationFOWST.Name as FowstName, MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                                " FROM LibraryDevicePrinterFowst " & vbCrLf &
                                " INNER JOIN MobizLibraryOrganizationFOWST ON LibraryDevicePrinterFowst.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                                " WHERE LibraryDevicePrinterFowst.PrinterID = " & ReadyString(pID) & vbCrLf &
                                " GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name " & vbCrLf &
                                " ORDER BY MobizLibraryOrganizationFOWST.Name ASC "

                pJson.Add("AssignedFowst", Common.WebRequest.ProData.Execute(pSql))
            End If

            pDtFowst = WebRequest.Instance.Business.LibraryOrganizationFOWST.GetByOrgID(WebRequest.Instance.OrgID)
            pJson.Add("Fowst", pDtFowst)

            pDtCategories = WebRequest.Instance.Business.LibraryPrinters.GetAllCategories(pID)
            pJson.Add("Categories", pDtCategories)

            Return pJson.Serialize

        End Function

        <HttpPost>
        Public Function SavePrinterByDevicePrinterFowst() As String

            Dim pInput As SavePrinterByDevicePrinterFowstInput
            Dim pPrinterID As String ' LibraryDevicePrinter
            Dim pID As String ' LibraryDevicePrinterFowst
            Dim pSql As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInput = JsonConvert.DeserializeObject(Of SavePrinterByDevicePrinterFowstInput)(Common.WebRequest.InputStreamText)
            If pInput Is Nothing Then
                Return JsonDictionary.Serialize("Error", "No input. vb9UK7EVDQCH!")
            End If
            If String.IsNullOrWhiteSpace(pInput.Brand) Then
                Return JsonDictionary.Serialize("Error", "Brand is required. vbW226ELMK5T!")
            End If
            If String.IsNullOrWhiteSpace(pInput.Model) Then
                Return JsonDictionary.Serialize("Error", "Model is required. vbER3JSYW8DX!")
            End If
            If String.IsNullOrWhiteSpace(pInput.FowstID) Then
                Return JsonDictionary.Serialize("Error", "Fowst is required. vbQD5TJ9KXB8!")
            End If
            If IsGuid(pInput.FowstID) = False Then
                Return JsonDictionary.Serialize("Error", "Fowst must be a GUID. vb9C4NFP8ZJZ!")
            End If
            If String.IsNullOrWhiteSpace(pInput.PrinterName) Then
                Return JsonDictionary.Serialize("Error", "Printer Name is required. vb4QT8ATC4P9!")
            End If
            If String.IsNullOrWhiteSpace(pInput.ConnectionType) Then
                Return JsonDictionary.Serialize("Error", "Interface Type is required. vbPXQH3LEAB9!")
            End If
            If pInput.ConnectionType = "Ethernet" Then
                If String.IsNullOrWhiteSpace(pInput.PrinterIPAddress) Then
                    Return JsonDictionary.Serialize("Error", "IP Address is required. vbDR3Y7WB5UA!")
                End If
            End If
            If String.IsNullOrWhiteSpace(pInput.PrinterRole) Then
                Return JsonDictionary.Serialize("Error", "Role is required. vb6CNHAC4MCD!")
            End If

            If CBool2(pInput.IsNewSetup) Then
                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("LibraryDevicePrinter")

                p("PrinterType") = ReadyString(pInput.Brand & " " & pInput.Model)
                p("IsActive") = 1
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"
                p("PrinterRole") = ReadyString(pInput.PrinterRole)
                p("Brand") = ReadyString(pInput.Brand)
                p("Model") = ReadyString(pInput.Model)
                p("PrinterIPAddress") = ReadyString(pInput.PrinterIPAddress)
                p("PrinterName") = ReadyString(pInput.PrinterName)
                p("ConnectionType") = ReadyString(pInput.ConnectionType)
                p("SerialNum") = ReadyString(pInput.SerialNum)
                p("IsNewSetup") = 1

                If CStr2(pInput.ID) = "" Then
                    pID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(pID)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    pID = pInput.ID.ToString()
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(pInput.ID)))
                End If

                For Each fowst As Object In pInput.AssignedFowst
                    If fowst("Status") = "remove" Then
                        Common.WebRequest.ProData.Execute("DELETE FROM LibraryDevicePrinterFowst WHERE FowstID = " & ReadyString(fowst("ID").ToString()) & " AND PrinterID = " & ReadyString(pID))
                    Else
                        p = New InsertUpdate("LibraryDevicePrinterFowst")

                        p("FowstID") = ReadyString(fowst("ID").ToString())
                        p("PrinterID") = ReadyString(pID)
                        p("PrinterIPAddress") = ReadyString(pInput.PrinterIPAddress)
                        p("PrinterName") = ReadyString(pInput.PrinterName)
                        p("ConnectionType") = ReadyString(pInput.ConnectionType)
                        p("SerialNum") = ReadyString(pInput.SerialNum)

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" FowstID = " & ReadyString(fowst("ID").ToString()) & " AND PrinterID = " & ReadyString(pID))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If

                        Dim PrinterFowstID As String = Common.WebRequest.ProData.ExecuteScalar("SELECT ID FROM LibraryDevicePrinterFowst WHERE FowstID = " & ReadyString(fowst("ID").ToString()) & " AND PrinterID = " & ReadyString(pID)).ToString()
                        Dim devicefowst As DataTable = Common.WebRequest.ProData.Execute("SELECT DeviceID FROM LibraryDeviceFowst WHERE FowstID = " & ReadyString(fowst("ID").ToString()))

                        For Each device As Object In devicefowst.Rows
                            pSql = "SELECT ID, PrinterFowstID FROM LibraryDeviceFowstPrinter WHERE DeviceID = " & ReadyString(device("DeviceID").ToString())
                            Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                            p = New InsertUpdate("LibraryDeviceFowstPrinter")

                            p("DeviceID") = ReadyString(device("DeviceID").ToString())
                            p("PrinterFowstID") = ReadyString(PrinterFowstID)
                            p("IsActive") = 1
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            p("LastUpdated") = "getdate()"

                            If row.Count = 0 Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())

                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            Else
                                If CStr2(row(0)("PrinterFowstID").ToString()) = "" Then
                                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(row(0)("ID").ToString())))
                                End If
                            End If
                        Next
                    End If
                Next

                WebRequest.Instance.Business.LibraryPrinters.SaveCategories(pID, pInput.Categories)
                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryDevicePrinter")
                Common.WebRequest.ProData.CommitTransaction()
            Else
                ProWeb.Common.WebRequest.ProData.BeginTransaction()
                pPrinterID = WebRequest.Instance.Business.LibraryPrinters.GetOrCreatePrinter(
                    vBrand:=pInput.Brand,
                    vModel:=pInput.Model,
                    vPrinterRole:=pInput.PrinterRole,
                    vUserID:=WebSession.Instance.PeopleUserID)
                pID = WebRequest.Instance.Business.LibraryPrinters.SavePrinterFowst(
                    vID:=pInput.ID,
                    vFowstID:=pInput.FowstID,
                    vPrinterID:=pPrinterID,
                    vPrinterIPAddress:=pInput.PrinterIPAddress,
                    vPrinterName:=pInput.PrinterName,
                    vSerialNum:=pInput.SerialNum,
                    vConnectionType:=pInput.ConnectionType)
                WebRequest.Instance.Business.LibraryPrinters.SaveCategories(pID, pInput.Categories)
                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryDevicePrinter")
                ProWeb.Common.WebRequest.ProData.CommitTransaction()
            End If

            Return JsonDictionary.Serialize("ID", pID)

        End Function

        <HttpPost>
        Public Function GetWorkScheduleDetails() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetWorkScheduleDetails())

        End Function

        <HttpPost>
        Public Function GetPayGradeDetails() As String
            Dim clsOrganization As New Organization

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetPayGradeDetails())

        End Function

        Private Class SavePrinterByDevicePrinterFowstInput
            Public ID As String
            Public Brand As String
            Public Model As String
            Public FowstID As String
            Public PrinterName As String
            Public PrinterIPAddress As String
            Public ConnectionType As String
            Public PrinterRole As String
            Public SerialNum As String
            Public Categories As List(Of String)
            Public IsNewSetup As String
            Public AssignedFowst As Object
        End Class

        <HttpPost>
        Public Function GetSimilarCustomers() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDeleteCache = CBool2(WebRequest.Instance.Data("DeleteCache"))
            If pDeleteCache Then
                WebRequest.Instance.Business.LibraryOrganization.DeleteSimilarOrganizationsFile()
            End If
            Dim pDT = WebRequest.Instance.Business.LibraryOrganization.GetAllCustomers
            Dim pSimilars = LibraryOrganization.GetSimilarOrganizations(pDT)
            Return JsonConvert.SerializeObject(pSimilars)

        End Function

        <HttpPost>
        Public Function GetSimilarSuppliers() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT = WebRequest.Instance.Business.LibraryOrganization.GetAllSuppliers
            Dim pSimilars = LibraryOrganization.GetSimilarOrganizations(pDT)
            Return JsonConvert.SerializeObject(pSimilars)

        End Function

        Public Class MergeOrganizationsInput

            Public Items As New ItemsClass

            Public Class ItemsClass
                Inherits List(Of Item)
            End Class

            Public Class Item
                Public OrgID As String
                Public Duplicates As List(Of String)
            End Class
        End Class

        <HttpPost>
        Public Function MergeDuplicateCustomers() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSecurityAccess As New SecurityAccess()
            If pSecurityAccess.CheckAccessRights(WebSession.Instance.PeopleUserID, "Merge Duplicate Customers", False) = False Then
                Return JsonDictionary.Serialize("Error", "This feature requires the Merge Duplicate Customers access.")
            End If

            Return MergeOrganizations()

        End Function

        <HttpPost>
        Public Function MergeDuplicateSuppliers() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSecurityAccess As New SecurityAccess()
            If pSecurityAccess.CheckAccessRights(WebSession.Instance.PeopleUserID, "Merge Duplicate Suppliers", False) = False Then
                Return JsonDictionary.Serialize("Error", "This feature requires the Merge Duplicate Suppliers access.")
            End If

            Return MergeOrganizations()

        End Function

        Private Function MergeOrganizations() As String

            LibraryOrganization.CancelSimilarOrganizations()
            Dim pInput = JsonConvert.DeserializeObject(Of MergeOrganizationsInput)(Common.WebRequest.InputStreamText)
            Common.WebRequest.ProData.BeginTransaction()
            For Each pItem In pInput.Items
                For Each pDuplicate In pItem.Duplicates
                    WebRequest.Instance.Business.LibraryCustomers.Deactivate(pDuplicate, "Duplicate record")
                    WebRequest.Instance.Business.LibrarySuppliers.Deactivate(pDuplicate, "Duplicate record")
                    WebRequest.Instance.Business.LibraryCustomers.TransferInvoices(pDuplicate, pItem.OrgID)
                    WebRequest.Instance.Business.LibrarySuppliers.TransferReceivings(pDuplicate, pItem.OrgID)
                    WebRequest.Instance.Business.TransReceiving.CacheReceivingsWithCredit()
                Next
            Next pItem
            WebRequest.Instance.Business.TransInvoice.CacheInvoicesWithCredit()
            Common.WebRequest.ProData.CommitTransaction()
            LibraryOrganization.DeleteSimilarOrganizationsFile()
            'Common.WebRequest.ProData.RollbackTransaction() ' for debugging only

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Function GetFOWSTAssignedEmployees()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsOrganization As New Organization

            Return JsonDictionary.Serialize("Results", clsOrganization.GetFOWSTAssignedEmployees())
        End Function

        Function RemoveFOWSTAssignedEmployees()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String

                For Each emp As Object In Common.WebRequest.AllData("Employees")
                    pSql = "Delete from LibraryOrganizationEmployeeFOWST Where PeopleID = " & ReadyString(emp("ID").ToString()) & vbCrLf &
                            " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))
                    Common.WebRequest.ProData.Execute(pSql)
                Next

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function DownloadEmployeeStoreAssignments() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "\FileTemplates\EmployeeStoreAssignments.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "\FileTemplates\EmployeeStoreAssignments.xlsx"
            pContentType = "application/xlsx"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "EmployeeStoreAssignments.xlsx")
        End Function

        Function UploadEmployeeStoreAssignments()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim file As Object = Request.Files
            Dim userid As String = Request.Params.Get("userid")
            Dim orgid As String = Request.Params.Get("OrgID")
            Dim action As String = Request.Params.Get("Action")
            Dim template As HttpPostedFileBase = file(0)
            Dim pFilename, pSql As String
            Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\"
            Dim dtaExcelData = New DataTable
            Dim rownumber As Integer = 2
            Dim strHeader = {"Group Name", "Store Code", "Store Name", "Area Manager's Company ID", "Area Manager's Name", "Area Manager's Email Address", "District Manager's Company ID", "District Manager's Name", "District Manager's Email Address"}
            Dim pFS As IO.FileStream
            Dim errmsg As String = ""
            Dim cntStoreWithMultipleAMs = 0
            Dim cntStoreWithMultipleDMs = 0
            Dim cntMissingStores = 0
            Dim cntMissingGroups = 0
            'isOkay = True

            If Not System.IO.Directory.Exists(pSourceURL) Then
                System.IO.Directory.CreateDirectory(pSourceURL)
            End If

            pFilename = pSourceURL & "EmployeeStoreAssignments.xls"
            CreateFolder(pFilename)
            template.SaveAs(pFilename)

            pFS = New IO.FileStream(pFilename, IO.FileMode.Open)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pFilename, strHeader, "")

                If dtaExcelData Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid Template. Please upload valid template.")
                End If

                If dtaExcelData.Rows.Count = 0 Then
                    Return JsonDictionary.Serialize("Result", "No data was found.")
                End If

                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                    Dim col As Object = dtaExcelData.Rows(i)
                    Dim groupname As String = Trim(CStr2(col.Item("Group Name")))
                    Dim storecode As String = Trim(CStr2(col.Item("Store Code")))
                    Dim amcompanyid As String = Trim(CStr2(col.Item("Area Manager's Company ID")))
                    Dim amemailadd As String = Trim(CStr2(col.Item("Area Manager's Email Address")))
                    Dim dmcompanyid As String = Trim(CStr2(col.Item("District Manager's Company ID")))
                    Dim dmemailadd As String = Trim(CStr2(col.Item("District Manager's Email Address")))
                    Dim groupid As String
                    Dim storeid As String
                    Dim amid As String
                    Dim dmid As String

                    If IsNumeric(amcompanyid) Then
                        amcompanyid = Format(CDec2(amcompanyid), "00000000")
                    End If

                    If IsNumeric(dmcompanyid) Then
                        dmcompanyid = Format(CDec2(dmcompanyid), "00000000")
                    End If

                    Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")

                    If String.IsNullOrWhiteSpace(groupname) And String.IsNullOrWhiteSpace(storecode) Then
                        errmsg &= "<br>In row #" & rownumber & ", Group Name or Store Code must not be blank in row number."
                    End If

                    If String.IsNullOrWhiteSpace(amcompanyid) And String.IsNullOrWhiteSpace(dmcompanyid) Then
                        errmsg &= "<br>In row #" & rownumber & ", Area or District Manager's Company ID must not be blank."
                    End If

                    If Not String.IsNullOrWhiteSpace(groupname) Then
                        groupid = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT ID FROM LibraryFranchiseGroup WHERE GroupName = " & ReadyString(groupname)))

                        If String.IsNullOrWhiteSpace(groupid) Then
                            cntMissingGroups += 1
                            errmsg &= "<br>In row #" & rownumber & ", the group name <i>'" & groupname & "'</i> does not exist."
                        End If
                    End If

                    If Not String.IsNullOrWhiteSpace(storecode) Then
                        storeid = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT ID FROM MobizLibraryOrganizationFOWST WHERE Code = " & ReadyString(storecode)))

                        If String.IsNullOrWhiteSpace(storeid) Then
                            cntMissingStores += 1
                            errmsg &= "<br>In row #" & rownumber & ", the store code <i>'" & storecode & "'</i> does not exist."
                        End If
                    End If

                    If Not String.IsNullOrWhiteSpace(amcompanyid) Then
                        amid = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT PeopleID FROM LibraryOrganizationEmployee WHERE CompanyID = " & ReadyString(amcompanyid)))

                        If String.IsNullOrWhiteSpace(amid) Then
                            errmsg &= "<br>In row #" & rownumber & ", the Area Manager's Company ID <i>'" & amcompanyid & "'</i> does not exist."
                        Else
                            If Not String.IsNullOrWhiteSpace(storeid) Then
                                pSql = " SELECT LibraryOrganizationEmployeeFOWST.ID " & vbCrLf &
                                    " FROM LibraryOrganizationEmployeeFOWST " & vbCrLf &
                                    " LEFT JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = LibraryOrganizationEmployeeFOWST.PeopleID " & vbCrLf &
                                    " LEFT JOIN LibraryDTRRole as role ON role.ID = LibraryOrganizationEmployee.RoleTagID " & vbCrLf &
                                    " WHERE FowstID = " & ReadyString(storeid) & vbCrLf &
                                    " AND role.RoleName = 'Area Manager' " & vbCrLf &
                                    " AND LibraryOrganizationEmployeeFOWST.PeopleID <> " & ReadyString(amid) 'exclude the manager

                                Dim assignedams As DataTable = Common.WebRequest.ProData.Execute(pSql)

                                If action = "validate" And assignedams.Rows.Count > 0 Then
                                    cntStoreWithMultipleAMs += 1
                                    errmsg &= "<br>In row #" & rownumber & ", Store code <i>'" & storecode & "'</i> already has other assigned area managers."
                                Else
                                    pSql = " DELETE FROM LibraryOrganizationEmployeeFOWST WHERE ID IN (" & pSql & ")"
                                    Common.WebRequest.ProData.Execute(pSql)

                                    p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")
                                    p("FowstID") = ReadyString(storeid)
                                    p("PeopleID") = ReadyString(amid)
                                    p("IsActive") = 1
                                    p("LastUpdated") = "getdate()"

                                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" FowstID = " & ReadyString(storeid) & " AND PeopleID = " & ReadyString(amid))) = 0 Then
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        Common.WebRequest.ProData.Execute(p.GetInsert())
                                    End If
                                End If
                            End If

                            If Not String.IsNullOrWhiteSpace(groupid) Then
                                p = New InsertUpdate("LibraryFranchiseGroupHeads")

                                p("FranchiseGroupID") = ReadyString(groupid)
                                p("PeopleID") = ReadyString(amid)

                                If Not String.IsNullOrWhiteSpace(amemailadd) Then
                                    p("CompanyEmailAddress") = ReadyString(amemailadd)
                                End If

                                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" FranchiseGroupID = " & ReadyString(groupid) & " AND PeopleID = " & ReadyString(amid))) = 0 Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    Common.WebRequest.ProData.Execute(p.GetInsert())
                                End If
                            End If
                        End If
                    End If

                    If Not String.IsNullOrWhiteSpace(dmcompanyid) Then
                        dmid = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT PeopleID FROM LibraryOrganizationEmployee WHERE CompanyID = " & ReadyString(dmcompanyid)))

                        If String.IsNullOrWhiteSpace(dmid) Then
                            errmsg &= "<br>n row #" & rownumber & ", the District Manager's Company ID <i>'" & dmcompanyid & "'</i> does not exist."
                        Else
                            If Not String.IsNullOrWhiteSpace(storeid) Then
                                pSql = " SELECT LibraryOrganizationEmployeeFOWST.ID " & vbCrLf &
                                    " FROM LibraryOrganizationEmployeeFOWST " & vbCrLf &
                                    " LEFT JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = LibraryOrganizationEmployeeFOWST.PeopleID " & vbCrLf &
                                    " LEFT JOIN LibraryDTRRole as role ON role.ID = LibraryOrganizationEmployee.RoleTagID " & vbCrLf &
                                    " WHERE FowstID = " & ReadyString(storeid) & vbCrLf &
                                    " AND role.RoleName = 'District Manager' " & vbCrLf &
                                    " AND LibraryOrganizationEmployeeFOWST.PeopleID <> " & ReadyString(dmid) 'exclude the manager
                                Dim assignedams As DataTable = Common.WebRequest.ProData.Execute(pSql)

                                If action = "validate" And assignedams.Rows.Count > 0 Then
                                    cntStoreWithMultipleDMs += 1
                                    errmsg &= "<br>In row #" & rownumber & ", Store code <i>'" & storecode & "'</i> already has other assigned district managers."
                                Else
                                    pSql = " DELETE FROM LibraryOrganizationEmployeeFOWST WHERE ID IN (" & pSql & ")"
                                    Common.WebRequest.ProData.Execute(pSql)

                                    p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")
                                    p("FowstID") = ReadyString(storeid)
                                    p("PeopleID") = ReadyString(dmid)
                                    p("IsActive") = 1
                                    p("LastUpdated") = "getdate()"

                                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" FowstID = " & ReadyString(storeid) & " AND PeopleID = " & ReadyString(dmid))) = 0 Then
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        Common.WebRequest.ProData.Execute(p.GetInsert())
                                    End If
                                End If
                            End If

                            If Not String.IsNullOrWhiteSpace(groupid) Then
                                p = New InsertUpdate("LibraryFranchiseGroupHeads")

                                p("FranchiseGroupID") = ReadyString(groupid)
                                p("PeopleID") = ReadyString(dmid)
                                p("CompanyEmailAddress") = ReadyString(dmemailadd)

                                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" FranchiseGroupID = " & ReadyString(groupid) & " AND PeopleID = " & ReadyString(dmid))) = 0 Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    Common.WebRequest.ProData.Execute(p.GetInsert())
                                End If
                            End If
                        End If
                    End If

                    rownumber += 1
                Next

                If action = "validate" Then
                    If cntStoreWithMultipleDMs > 0 Or cntStoreWithMultipleAMs > 0 Or Not String.IsNullOrWhiteSpace(errmsg) Then
                        Common.WebRequest.ProData.RollbackTransaction()
                        Dim pOut As New JsonDictionary
                        pOut.Add("NoOfStoreWithMultipleAMs", cntStoreWithMultipleAMs)
                        pOut.Add("NoOfStoreWithMultipleDMs", cntStoreWithMultipleDMs)
                        pOut.Add("NoOfMissingStores", cntMissingStores)
                        pOut.Add("NoOfMissingGroups", cntMissingGroups)
                        pOut.Add("Findings", errmsg)
                        Return pOut.Serialize()
                    Else
                        Return JsonDictionary.Serialize("Findings", "good")
                    End If
                Else
                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationEmployeeFOWST'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Common.WebRequest.ProData.CommitTransaction()
                    Return JsonDictionary.Serialize("Result", "The process of assigning employees to stores has been updated.")
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function UploadPimAttachments() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim files As HttpFileCollectionBase = Request.Files
            Dim ProductID As String = Request.Params.Get("ProductID").ToString()
            Dim folderName As String = Common.ServerPath & "Content\PIM\" & ProductID

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim p As New InsertUpdate("LibraryPimFileAttachments")
                    Dim id As String = Guid.NewGuid.ToString()
                    Dim filename As String = file.FileName

                    p("ID") = ReadyString(id)
                    p("ProductID") = ReadyString(ProductID)
                    p("FileName") = ReadyString(filename)
                    p("FileLocation") = ReadyString(folderName)
                    p("FileType") = ReadyString(System.IO.Path.GetExtension(file.FileName).ToLower())
                    p("DateUploaded") = "getdate()"
                    p("UploadedBy") = ReadyString(If(WebSession.Instance.PeopleUserID Is Nothing, Guid.Empty.ToString(), WebSession.Instance.PeopleUserID))

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    Dim pSourceURL As String = folderName & "\" & filename
                    file.SaveAs(pSourceURL)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function GetPimFileAttachments()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT ID, FileName FROM LibraryPimFileAttachments WHERE ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & " ORDER BY DateUploaded DESC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function RemovePimFileAttachment()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT FileLocation, FileName, FileType FROM LibraryPimFileAttachments WHERE ID = " & ReadyString(Common.WebRequest.Data("ID")) & " ORDER BY DateUploaded DESC "
            Dim dRow As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)
            Dim path As String = dRow("FileLocation") & "\" & dRow("FileName")

            pSql = " DELETE FROM LibraryPimFileAttachments WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
            Common.WebRequest.ProData.Execute(pSql)

            If System.IO.File.Exists(path) Then
                System.IO.File.Delete(path)
            End If

            Return JsonDictionary.Serialize("Results", "removed")
        End Function

        Function DownloadPimFileAttachment()
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            Dim pSql As String = " SELECT FileLocation, FileName, FileType FROM LibraryPimFileAttachments WHERE ID = " & ReadyString(Request.Params.Get("ID")) & " ORDER BY DateUploaded DESC "
            Dim dRow As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)
            Dim file As New System.IO.FileInfo(dRow("FileLocation") & "\" & dRow("FileName"))
            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/octet-stream"
            Response.WriteFile(file.FullName)
            Response.End()
            Return "zread generated"
        End Function
    End Class
End Namespace