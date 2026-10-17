Imports Newtonsoft.Json

Namespace WebProj
    Public Class OrganizationShortController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function NewCustomerPage() As ActionResult
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

        Function OrganizationRepresentativeOld() As ActionResult
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

        Function DownloadCustomerVendorDataMappingTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "\FileTemplates\Customer-Vendor data mapping.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "\FileTemplates\Customer-Vendor data mapping.xlsx"
            pContentType = "application/xlsx"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "Customer-Vendor data mapping.xlsx")
        End Function

        Function UploadVendorPim()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim file As Object = Request.Files
            Dim template As HttpPostedFileBase = file(0)
            Dim pFilename, pSql As String
            Dim pFS As IO.FileStream
            Dim dtaExcelData = New DataTable
            Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\"
            Dim strHeader = {"Product No *", "Cost From Vendor *"}
            Dim vendorid As String = Common.WebRequest.Data("VendorID")
            Dim errProductID As String = ""
            Dim errPrice As String = ""
            Dim pOut As New JsonDictionary

            pFilename = pSourceURL & vendorid & "_VendorPimTemplate.xls"
            CreateFolder(pFilename)
            template.SaveAs(pFilename)

            pFS = New IO.FileStream(pFilename, IO.FileMode.Open)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pFilename, strHeader, "")

                If dtaExcelData Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid Template. Please upload valid template.")
                End If

                Dim len As Integer = dtaExcelData.Rows.Count - 1
                Dim row As Integer = 1
                Common.WebRequest.ProData.BeginTransaction()
                For i As Integer = 0 To len
                    Dim col As Object = dtaExcelData.Rows(i)
                    Dim productno As String = Trim(CStr2(col.Item("Product No *")))
                    Dim price As String = Trim(CStr2(col.Item("Cost From Vendor *")))
                    Dim productrow = WebRequest.Instance.Business.LibraryStock.ByNumber(productno)
                    row += 1

                    If Not productrow Is Nothing Then
                        Dim productid As String = productrow("ID").ToString()
                        Dim p As New InsertUpdate("LibraryVendorPrices")

                        If Not IsNumeric(price) Then
                            errPrice &= "Invalid Price for Product No. " & productno & " in row number " & row & " <br>"
                            Continue For
                        End If

                        p("Price") = price
                        p("LastUpdated") = "getdate()"
                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ProductID = " & ReadyString(productid) & " AND VendorID = " & ReadyString(vendorid))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("ProductID") = ReadyString(productid)
                            p("VendorID") = ReadyString(vendorid)
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    Else
                        errProductID &= " Product No. " & productno & " does not exist in row number " & row & " <br>"
                    End If
                Next
                Common.WebRequest.ProData.CommitTransaction()

                pOut.Add("ProductNos", errProductID)
                pOut.Add("Prices", errPrice)

                Return pOut.Serialize()
            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", "error")
            End Try
        End Function

        Function UploadCustomerVendorDataMapping()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim file As Object = Request.Files
            Dim template As HttpPostedFileBase = file(0)
            Dim pFilename, pSql As String
            Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\"
            Dim dtaExcelData = New DataTable
            Dim row As Object
            Dim canvassnumber As Integer = 0
            Dim rownumber As Integer = 0
            Dim result As String
            Dim strHeader = {"Mobiz Code", "3rd Party ID", "Organization Name", "Outlet's 3rd Party ID", "Outlet Name", "Department's 3rd Party ID", "Department Name"}
            Dim pFS As IO.FileStream
            Dim errmsg As String = ""
            Dim pDT As DataTable
            'isOkay = True

            If Not System.IO.Directory.Exists(pSourceURL) Then
                System.IO.Directory.CreateDirectory(pSourceURL)
            End If

            pFilename = pSourceURL & "CustomerVendorDataMapping.xls"
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
                    Dim mobizcode As String = Trim(CStr2(col.Item("Mobiz Code")))
                    Dim thirdpid As String = Trim(CStr2(col.Item("3rd Party ID")))
                    Dim orgid As String
                    Dim orgname As String = Trim(CStr2(col.Item("Organization Name")))
                    Dim outletid As String = Guid.NewGuid.ToString()
                    Dim outletthirdpid As String = Trim(CStr2(col.Item("Outlet's 3rd Party ID")))
                    Dim outletname As String = Trim(CStr2(col.Item("Outlet Name")))
                    Dim departmentid As String = Guid.NewGuid.ToString()
                    Dim departmentthirdpid As String = Trim(CStr2(col.Item("Department's 3rd Party ID")))
                    Dim departmentname As String = Trim(CStr2(col.Item("Department Name")))
                    Dim p As New InsertUpdate("LibraryOrganizationCustomer")

                    pSql = "SELECT ISNULL(convert(varchar(max), OrganizationID), '') as OrganizationID FROM LibraryOrganizationCustomer WHERE CustomerCode = " & ReadyString(mobizcode)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If pDT.Rows.Count = 0 Then
                        orgid = ""
                    Else
                        orgid = pDT.Rows(0)("OrganizationID")
                    End If

                    If orgid = "" Then
                        errmsg &= "<br>Customer Code: " & mobizcode & " does not exist in the mobiz server."
                    Else
                        If thirdpid <> "" Then
                            p = New InsertUpdate("LibraryOrganizationCustomer")

                            p("ThirdPartyID") = ReadyString(thirdpid)

                            Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" OrganizationID = " & ReadyString(orgid)))
                        End If

                        If outletname = "" Then
                            errmsg &= "<br>Outlet name is required in row no: " & (i + 2)
                        Else
                            If outletthirdpid <> "" Then
                                pSql = "SELECT ISNULL(convert(varchar(max), ID), '') as ID, ISNULL(Name, '') as Name FROM MobizLibraryOrganizationFOWST WHERE NetSuiteID = " & ReadyString(outletthirdpid) & " AND OrganizationID = " & ReadyString(orgid)
                                pDT = Common.WebRequest.ProData.Execute(pSql)
                                outletname = Trim(CStr2(col.Item("Outlet Name"))) 'from excel

                                If pDT.Rows.Count = 0 Then
                                    outletid = Guid.NewGuid.ToString()
                                Else
                                    outletid = pDT.Rows(0)("ID")
                                End If

                                If outletname = "" Then
                                    errmsg &= "<br>Outlet name is required in row no: " & (i + 2) & " since you enter an Outlet ID."
                                Else
                                    p = New InsertUpdate("MobizLibraryOrganizationFOWST")

                                    p("OrganizationID") = ReadyString(orgid)
                                    p("Name") = ReadyString(outletname)
                                    p("StreetNumber") = ReadyString("")
                                    p("NetSuiteID") = IIf(outletthirdpid = "", "NULL", ReadyString(outletthirdpid))
                                    p("Street") = ReadyString("")
                                    p("IsActive") = "1"

                                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(outletid))) = 0 Then
                                        p("ID") = ReadyString(outletid)
                                        Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                                    End If
                                End If
                            Else
                                If outletname = "" Then
                                    errmsg &= "<br>Outlet name is required in row no: " & (i + 2)
                                Else
                                    pSql = "SELECT ISNULL(convert(varchar(max), ID), '') as ID FROM MobizLibraryOrganizationFOWST WHERE Name = " & ReadyString(outletname) & " AND OrganizationID = " & ReadyString(orgid)
                                    pDT = Common.WebRequest.ProData.Execute(pSql)

                                    If pDT.Rows.Count = 0 Then
                                        outletid = Guid.NewGuid.ToString()
                                    Else
                                        outletid = pDT.Rows(0)("ID")
                                    End If

                                    p = New InsertUpdate("MobizLibraryOrganizationFOWST")

                                    p("OrganizationID") = ReadyString(orgid)
                                    p("Name") = ReadyString(outletname)
                                    p("StreetNumber") = ReadyString("")
                                    p("NetSuiteID") = IIf(outletthirdpid = "", "NULL", ReadyString(outletthirdpid))
                                    p("Street") = ReadyString("")
                                    p("IsActive") = "1"

                                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(outletid))) = 0 Then
                                        p("ID") = ReadyString(outletid)
                                        Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                                    End If
                                End If
                            End If

                            If departmentname <> "" Or departmentthirdpid <> "" Then
                                If outletname = "" Then
                                    errmsg &= "<br>Outlet name is required in row no. " & (i + 2) & " since you enter a department id or name."
                                Else
                                    If departmentthirdpid <> "" Then
                                        pSql = "SELECT ISNULL(convert(varchar(max), ID), '') as ID FROM LibraryFowstBin WHERE NetSuiteID = " & ReadyString(departmentthirdpid)
                                        pDT = Common.WebRequest.ProData.Execute(pSql)

                                        If pDT.Rows.Count = 0 Then
                                            departmentid = Guid.NewGuid.ToString()
                                        Else
                                            departmentid = pDT.Rows(0)("ID")
                                        End If

                                        If departmentname = "" Then
                                            errmsg &= "<br>Department Name is required in row no. " & (i + 2) & " since you enter a Department ID."
                                        Else
                                            p = New InsertUpdate("LibraryFowstBin")

                                            p("FowstID") = ReadyString(outletid)
                                            p("Name") = ReadyString(departmentname)
                                            p("NetSuiteID") = ReadyString(departmentthirdpid)
                                            p("IsActive") = "1"

                                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(departmentid))) = 0 Then
                                                p("ID") = ReadyString(departmentid)
                                                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                                            End If
                                        End If
                                    Else
                                        If departmentname <> "" Then
                                            pSql = "SELECT ISNULL(convert(varchar(max), ID), '') as ID FROM LibraryFowstBin WHERE Name = " & ReadyString(departmentname)
                                            pDT = Common.WebRequest.ProData.Execute(pSql)

                                            If pDT.Rows.Count = 0 Then
                                                departmentid = Guid.NewGuid.ToString()
                                            Else
                                                departmentid = pDT.Rows(0)("ID")
                                            End If

                                            p = New InsertUpdate("LibraryFowstBin")

                                            p("FowstID") = ReadyString(outletid)
                                            p("Name") = ReadyString(departmentname)
                                            p("NetSuiteID") = ReadyString(departmentthirdpid)
                                            p("IsActive") = "1"

                                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(departmentid))) = 0 Then
                                                p("ID") = ReadyString(departmentid)
                                                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
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

        Function GetSupplierCanvassForm()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT pcven.ID, pc.ID as CanvassID, CanvassName, LastUpdated " & vbCrLf &
                            " FROM LibraryPriceCanvass as pc " & vbCrLf &
                            " INNER JOIN LibraryPriceCanvassVendor as pcven ON pcven.CanvassID = pc.ID " & vbCrLf &
                            " WHERE pcven.VendorID = " & ReadyString(Common.WebRequest.Data("SupplierID"))

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))

        End Function

        Function RemoveSupplierCanvassForm()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "DELETE FROM LibraryPriceCanvassVendor WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
            Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", "deleted")
        End Function

        Function SaveSupplierCanvassForm()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim forms As Object = Common.WebRequest.AllData("forms")
                Dim supplierID As String = Common.WebRequest.Data("SupplierID")

                Common.WebRequest.ProData.BeginTransaction()

                For Each form As Object In forms
                    Dim p As New InsertUpdate("LibraryPriceCanvassVendor")

                    p("CanvassID") = ReadyString(form("ID").ToString())
                    p("VendorID") = ReadyString(supplierID)

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate("VendorID = " & ReadyString(supplierID) & " AND CanvassID = " & ReadyString(form("ID").ToString()))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("CanvassVendorCode") = ReadyString(New CanvassController().GenerateShortCode("vendor", "vendor"))
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next form

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function UploadFiles() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim id As String = Request.Params.Get("id") ' customer/supplier id
            Dim InvRefNo As String = Request.Params.Get("InvRefNo")
            Dim InvRefDate As String = Request.Params.Get("InvRefDate")
            Dim InvNoOfLineItems As String = Request.Params.Get("InvNoOfLineItems")
            Dim InvRefAmount As String = Request.Params.Get("InvRefAmount")
            Dim Remarks As String = Request.Params.Get("Remarks")
            Dim files As HttpFileCollectionBase = Request.Files
            Dim folderName As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\CustomerSupplierFiles\" & id
            Dim dbID As String

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim name As String = file.FileName

                    If Not InvRefNo Is Nothing Then
                        name = InvRefNo & "." & file.ContentType.Split("/")(1)
                        name = System.Text.RegularExpressions.Regex.Replace(name, "(\s+|\#|\%|\&|\{|\}|\\|\<|\>|\*|\?|\/|\$|\!|\'|\:|\@|\+|\`|\||\=)", "")
                    End If

                    Dim pSourceURL As String = folderName & "\" & name

                    If System.IO.File.Exists(pSourceURL) Then
                        name = Format(Now(), "MMddyyyy_hhmmsstt") & "_" & name
                        pSourceURL = folderName & "\" & name
                    End If

                    Dim p As New InsertUpdate("LibraryCRMAttachments")

                    dbID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(dbID)
                    p("ClientID") = IIf(String.IsNullOrWhiteSpace(id), "NULL", ReadyString(id))
                    p("FileName") = ReadyString(name)
                    p("LastUpdated") = "getdate()"
                    p("InvoiceRefNo") = IIf(InvRefNo = Nothing, "NULL", ReadyString(InvRefNo))
                    p("InvoiceDate") = IIf(InvRefDate = Nothing, "NULL", ReadyString(InvRefDate))
                    p("NoOfLineItems") = IIf(InvNoOfLineItems = Nothing, "NULL", ReadyString(InvNoOfLineItems))
                    p("InvoiceAmount") = IIf(InvRefAmount = Nothing, "NULL", ReadyString(InvRefAmount))
                    p("Remarks") = IIf(Remarks = Nothing, "NULL", ReadyString(Remarks))

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    file.SaveAs(pSourceURL)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Dim pOut As New JsonDictionary

                pOut.Add("Results", "uploaded")
                pOut.Add("ID", dbID)

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function GetFiles()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String

            'Reuse lang na ni table since same column lang kung magcreate ug new table
            pSql = "SELECT ID, FileName, LastUpdated FROM LibraryCRMAttachments WHERE ClientID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf

            If CStr2(Common.WebRequest.Data("keywords")) <> "" Then
                pSql += " AND FileName LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))
            End If

            If CBool2(Common.WebRequest.Data("InvoiceOnly")) Then
                pSql += " AND InvoiceRefNo IS NOT NULL "
            End If

            pSql += " ORDER BY LastUpdated ASC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function DownloadFile() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pOut As New JsonDictionary

                Dim file As New System.IO.FileInfo(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\CustomerSupplierFiles\" & Request.Params.Get("ClientID") & "\" & Request.Params.Get("FileName"))

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & Request.Params.Get("FileName"))
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetNumberContactPersonForApproval()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pOut As New JsonDictionary

            Try
                pOut.Add("Supplier", Common.WebRequest.ProData.Execute("SELECT COUNT(cp.ID) as NoOfRequest FROM MobizLibraryOrganizationContactPerson as cp INNER JOIN LibraryOrganizationSupplier as sup ON sup.SupplierID = cp.OrganizationID WHERE IsUseForEcommerce = 1 AND Status = 'Pending'").Rows(0)("NoOfRequest"))
                pOut.Add("Customer", Common.WebRequest.ProData.Execute("SELECT COUNT(cp.ID) as NoOfRequest FROM MobizLibraryOrganizationContactPerson as cp INNER JOIN LibraryOrganizationCustomer as cus ON cus.OrganizationID = cp.OrganizationID WHERE IsUseForEcommerce = 1 AND Status = 'Pending'").Rows(0)("NoOfRequest"))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetVendorContactPersonRequestList()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim keywords As String = Common.WebRequest.Data("keywords")

                Dim pSql As String = " SELECT cp.*, org.CompanyName as OrgName, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as AdminName " & vbCrLf &
                                        " FROM MobizLibraryOrganizationContactPerson as cp " & vbCrLf &
                                        " INNER JOIN LibraryOrganizationSupplier as sup ON sup.SupplierID = cp.OrganizationID " & vbCrLf &
                                        " INNER JOIN MobizLibraryOrganization as org ON org.ID = cp.OrganizationID " & vbCrLf &
                                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID  = cp.PeopleID " & vbCrLf &
                                        " WHERE IsUseForEcommerce = 1 AND Status = 'Pending'" & vbCrLf &
                                        IIf(keywords = "", "", " AND (ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname LIKE " & ReadyStringWild(keywords) & ")")
                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetCustomerContactPersonRequestList()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim keywords As String = Common.WebRequest.Data("keywords")

                Dim pSql As String = " SELECT cp.*, org.CompanyName as OrgName, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as AdminName " & vbCrLf &
                                        " FROM MobizLibraryOrganizationContactPerson as cp " & vbCrLf &
                                        " INNER JOIN LibraryOrganizationCustomer as cus ON cus.OrganizationID = cp.OrganizationID " & vbCrLf &
                                        " INNER JOIN MobizLibraryOrganization as org ON org.ID = cp.OrganizationID " & vbCrLf &
                                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID  = cp.PeopleID " & vbCrLf &
                                        " WHERE IsUseForEcommerce = 1 AND Status = 'Pending'" & vbCrLf &
                                        IIf(keywords = "", "", " AND (ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname LIKE " & ReadyStringWild(keywords) & ")")
                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function ApproveContactPersonRequest()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("MobizLibraryOrganizationContactPerson")

                p("Status") = ReadyString(Common.WebRequest.Data("Status"))
                p("StatusLastUpdated") = "getdate()"

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID"))))

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GeneratePublicUrl()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim orgshort As New OrganizationShort
            Dim IsModule As String = Common.WebRequest.Data("IsModule")
            Dim companyID As String = Common.WebRequest.Data("CompanyID")
            Dim pOut As New JsonDictionary
            Dim viewid As String = Common.WebRequest.Data("ID")
            Dim modules As String = Common.WebRequest.Data("Module")
            Dim rand As New Random()
            Dim allowableChars() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
            Dim link As String

            For i As Integer = 0 To 100 - 1
                link += allowableChars(rand.Next(allowableChars.Length - 1))
            Next

            Dim p As New InsertUpdate("MobizLibrarySurveyQuestionaireLink") 'just reuse the table from Survey link

            p("OrgID") = ReadyString(Common.WebRequest.Data("OrgID"))

            If CBool2(IsModule) = False Then
                p("PeopleID") = ReadyString(viewid)

                If modules = "customer" Then
                    p("CustomerID") = ReadyString(companyID)
                Else
                    p("SupplierID") = ReadyString(companyID)
                End If
            Else
                If modules = "customer" Then
                    p("CustomerID") = ReadyString(viewid)
                Else
                    p("SupplierID") = ReadyString(viewid)
                End If
            End If

            p("Link") = ReadyString(link)
            p("DateCreated") = "getdate()"
            p("IsDisableLink") = "0"

            If CBool2(IsModule) = False Then
                'kapag contact person, one URL only
                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(viewid))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("IsUseForEcommerce") = 1

                Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(viewid)))
            Else
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                Common.WebRequest.ProData.Execute(p.GetInsert())
            End If

            pOut.Add("Link", link)
            pOut.Add("Results", "generated")

            'NO NEED TO CREATE TEMPORARY ACCOUNT
            'If Common.WebRequest.Data("ProfileType") = "Org" Then
            '    Dim pSql As String

            '    If modules = "customer" Then
            '        pSql = "SELECT CustomerCode FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(viewid)
            '    Else
            '        pSql = "SELECT VendorCode FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(viewid)
            '    End If

            '    Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            '    Dim vcode As String

            '    If rows.Count = 0 Then
            '        vcode = orgshort.GenerateCodeNumber(False)

            '        If modules = "customer" Then
            '            p = New InsertUpdate("LibraryOrganizationCustomer")
            '            p("CustomerCode") = ReadyString(vcode)
            '            Common.WebRequest.ProData.Execute(p.GetUpdate(" OrganizationID = " & ReadyString(viewid)))
            '        Else
            '            p = New InsertUpdate("LibraryOrganizationSupplier")
            '            p("VendorCode") = ReadyString(vcode)
            '            Common.WebRequest.ProData.Execute(p.GetUpdate(" SupplierID = " & ReadyString(viewid)))
            '        End If
            '    Else
            '        If modules = "customer" Then
            '            vcode = rows(0)("CustomerCode").ToString()
            '        Else
            '            vcode = rows(0)("VendorCode").ToString()
            '        End If
            '    End If

            '    p = New InsertUpdate("MobizLibraryPeople")
            '    Dim ppolid As String = Guid.NewGuid.ToString()
            '    p("ID") = ReadyString(ppolid)
            '    p("FirstName") = ReadyString("Admin")
            '    p("MiddleName") = ReadyString("")
            '    p("Surname") = ReadyString(vcode)
            '    p("IsActive") = 1
            '    p("LastUpdated") = "getdate()"
            '    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            '    Common.WebRequest.ProData.Execute(p.GetInsert())

            '    p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

            '    p("ID") = ReadyString(Guid.NewGuid.ToString())
            '    p("OrganizationID") = ReadyString(viewid)
            '    p("PeopleID") = ReadyString(ppolid)
            '    p("LastUpdated") = "getdate()"
            '    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            '    p("IsUseForEcommerce") = 1

            '    Common.WebRequest.ProData.Execute(p.GetInsert())

            '    p = New InsertUpdate("MobizLibraryPeopleUserAccount")

            '    p("ID") = ReadyString(Guid.NewGuid.ToString())
            '    p("PeopleID") = ReadyString(ppolid)
            '    p("Username") = ReadyString("admin_" & vcode)
            '    p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString("admin_" & vcode))
            '    p("IsActive") = 1
            '    Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
            'End If

            Return pOut.Serialize()
        End Function

        Function GetGeneratedPublicUrl()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim viewid As String = Common.WebRequest.Data("ID")
            Dim modules As String = Common.WebRequest.Data("Module")
            Dim pSql As String

            If modules = "customer" Then
                pSql = "SELECT * FROM MobizLibrarySurveyQuestionaireLink WHERE ISNULL(CONVERT(varchar(MAX), PeopleID), '') = '' AND CustomerID = " & ReadyString(viewid)
            Else
                pSql = "SELECT * FROM MobizLibrarySurveyQuestionaireLink WHERE ISNULL(CONVERT(varchar(MAX), PeopleID), '') = '' AND SupplierID = " & ReadyString(viewid)
            End If

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function ApplyOutletsToSelectedCustomers()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsorgshort As New OrganizationShort

            clsorgshort.ObjOutlet = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", clsorgshort.ApplyOutletsToSelectedCustomers())
        End Function

        Function GetOutlet()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Try
                Dim clsorgshort As New OrganizationShort
                Dim pOut As New JsonDictionary

                clsorgshort.ObjOutlet = Common.WebRequest.AllData("data")

                For Each org As Object In clsorgshort.ObjOutlet
                    Dim pSql As String = "SELECT ID, Name FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(org("ID").ToString())

                    org.Add("Outlet", jss.DeserializeObject(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next org

                pOut.Add("Cus", clsorgshort.ObjOutlet)

                Return pOut.Serialize() 'JsonDictionary.Serialize("Results", clsorgshort.ApplyOutletsToSelectedCustomers())
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
        End Function

        <HttpPost>
        Function ImportDataFromCrm() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsorgshort As New OrganizationShort

            Return clsorgshort.ImportDataFromCrm()
        End Function

        <HttpPost()>
        Function SaveNewPersonProspect() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            clsorgshort.CustomerID = CStr2(Common.WebRequest.Data("CustomerID"))
            clsorgshort.OrgID = CStr2(Common.WebRequest.Data("OrgID"))
            clsorgshort.ThirdPartyID = CStr2(Common.WebRequest.Data("ThirdPartyID"))
            clsorgshort.CodeNumber = CStr2(Common.WebRequest.Data("CodeNumber"))
            clsorgshort.PersonID = CStr2(Common.WebRequest.Data("PersonID"))
            clsorgshort.FirstName = CStr2(Common.WebRequest.Data("FirstName"))
            clsorgshort.MiddleName = CStr2(Common.WebRequest.Data("MiddleName"))
            clsorgshort.LastName = CStr2(Common.WebRequest.Data("LastName"))
            clsorgshort.Tin = CStr2(Common.WebRequest.Data("Tin"))
            clsorgshort.BirthDate = CStr2(Common.WebRequest.Data("BirthDate"))
            clsorgshort.Position = CStr2(Common.WebRequest.Data("Position"))
            clsorgshort.PositionDescription = CStr2(Common.WebRequest.Data("PositionDescription"))
            clsorgshort.AddressID = CStr2(Common.WebRequest.Data("AddressID"))
            clsorgshort.HouseBuildingNo = CStr2(Common.WebRequest.Data("HouseBuildingNo"))
            clsorgshort.Street = CStr2(Common.WebRequest.Data("Street"))
            clsorgshort.City = CStr2(Common.WebRequest.Data("City"))
            clsorgshort.Region = CStr2(Common.WebRequest.Data("Region"))
            clsorgshort.Island = CStr2(Common.WebRequest.Data("Island"))
            clsorgshort.Country = CStr2(Common.WebRequest.Data("Country"))
            clsorgshort.TypeAddress = CStr2(Common.WebRequest.Data("TypeAddress"))
            clsorgshort.ObjTelephone = Common.WebRequest.AllData("Telphone")
            clsorgshort.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsorgshort.ObjEmail = Common.WebRequest.AllData("Email")
            clsorgshort.ObjPersonOrgAff = Common.WebRequest.AllData("OrgAffiliation")
            clsorgshort.DateTime = CStr2(Common.WebRequest.Data("DateNow"))
            clsorgshort.Status = CStr2(Common.WebRequest.Data("Status"))
            clsorgshort.ObjPersonElementary = Common.WebRequest.AllData("Elementary")
            clsorgshort.ObjPersonHighSchool = Common.WebRequest.AllData("HighSchool")
            clsorgshort.ObjPersonCollege = Common.WebRequest.AllData("College")

            clsorgshort.SalesRepID = IIf(Common.WebRequest.Data("SalesRepID") = "", "NULL", ReadyString(Common.WebRequest.Data("SalesRepID")))
            clsorgshort.PriceLevel = Common.WebRequest.AllData("PriceLevel")
            clsorgshort.Terms = IIf(Common.WebRequest.Data("Terms") = "", "NULL", ReadyString(Common.WebRequest.Data("Terms")))
            clsorgshort.Limit = IIf(Common.WebRequest.Data("Limit") = "", "0", Common.WebRequest.Data("Limit"))
            clsorgshort.UnlimitedCredit = If(CBool2(Common.WebRequest.Data("UnlimitedCredit")), 1, 0)
            clsorgshort.RequireCnCApproval = If(CBool2(Common.WebRequest.Data("RequireCnCApproval")), 1, 0)
            clsorgshort.CustomerRequirePoNum = If(CBool2(Common.WebRequest.Data("CustomerRequirePoNum")), 1, 0)
            clsorgshort.Balance = IIf(Common.WebRequest.Data("Balance") = "", "0", Common.WebRequest.Data("Balance"))
            clsorgshort.AsOf = IIf(IsDate(Common.WebRequest.Data("AsOf")), Common.WebRequest.Data("AsOf"), Date.Now())

            clsorgshort.SaveAsCustomer = CBool2(Common.WebRequest.Data("IsCustomerSettings"))

            strReturn = JsonDictionary.Serialize("Results", clsorgshort.SaveNewPersonProspect())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveNewOrgProspect() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort
            Dim pError As String

            clsorgshort.OrgID = CStr2(Common.WebRequest.Data("OrgID"))
            clsorgshort.CompanyID = CStr2(Common.WebRequest.Data("CompanyID"))
            clsorgshort.CustomerID = CStr2(Common.WebRequest.Data("CustomerID"))
            clsorgshort.ThirdPartyPlatform = CStr2(Common.WebRequest.Data("ThirdPartyPlatform"))
            clsorgshort.ThirdPartyID = CStr2(Common.WebRequest.Data("ThirdPartyID"))
            clsorgshort.CodeNumber = CStr2(Common.WebRequest.Data("CodeNumber"))
            clsorgshort.CompanyName = CStr2(Common.WebRequest.Data("CompanyName"))
            clsorgshort.CompanyBusinessTradeName = CStr2(Common.WebRequest.Data("CompanyBusinessTradeName"))
            clsorgshort.Tin = CStr2(Common.WebRequest.Data("Tin"))
            clsorgshort.VatType = CStr2(Common.WebRequest.Data("VatType"))
            clsorgshort.CompanyBuildingAddress = CStr2(Common.WebRequest.Data("CompanyBuildingAddress"))
            clsorgshort.CompanyStreetAddress = CStr2(Common.WebRequest.Data("CompanyStreetAddress"))
            clsorgshort.CompanyCityAddress = CStr2(Common.WebRequest.Data("CompanyCityAddress"))
            clsorgshort.CompanyRegionAddress = CStr2(Common.WebRequest.Data("CompanyRegionAddress"))
            clsorgshort.Island = CStr2(Common.WebRequest.Data("Island"))
            clsorgshort.CompanyCountryAddress = CStr2(Common.WebRequest.Data("CompanyCountryAddress"))
            clsorgshort.CompanyBackground = CStr2(Common.WebRequest.Data("CompanyBackground"))
            clsorgshort.CompanyMission = CStr2(Common.WebRequest.Data("CompanyMission"))
            clsorgshort.CompanyVision = CStr2(Common.WebRequest.Data("CompanyVision"))
            clsorgshort.CompanyFoundingDate = CStr2(Common.WebRequest.Data("CompanyFoundingDate"))
            clsorgshort.BusinessIndustryID = CStr2(Common.WebRequest.Data("BusinessIndustryID"))
            clsorgshort.BusinessIndustryText = CStr2(Common.WebRequest.Data("BusinessIndustryText"))
            clsorgshort.ObjTelephone = Common.WebRequest.AllData("Telphone")
            clsorgshort.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsorgshort.ObjEmail = Common.WebRequest.AllData("Email")
            clsorgshort.ObjWebsite = Common.WebRequest.AllData("Website")
            clsorgshort.ObjOutlet = Common.WebRequest.AllData("Outlet")
            clsorgshort.ObjContactPerson = Common.WebRequest.AllData("ContactPerson")
            clsorgshort.ObjContactPersonTelephone = Common.WebRequest.AllData("ContactPersonTelphone")
            clsorgshort.ObjContactPersonMobile = Common.WebRequest.AllData("ContactPersonMobile")
            clsorgshort.ObjContactPersonEmail = Common.WebRequest.AllData("ContactPersonEmail")
            clsorgshort.ObjOrgAff = Common.WebRequest.AllData("OrgAffiliation")
            clsorgshort.ObjAffiliatedPerson = Common.WebRequest.AllData("OrgAffiliatedPerson")
            clsorgshort.DateTime = CStr2(Common.WebRequest.Data("DateNow"))
            clsorgshort.Status = CStr2(Common.WebRequest.Data("Status"))
            clsorgshort.NoOfEmployees = IIf(Common.WebRequest.Data("NoOfEmployees") = "", "NULL", Common.WebRequest.Data("NoOfEmployees"))
            clsorgshort.RevenuePerYear = IIf(Common.WebRequest.Data("Revenue") = "", "NULL", Common.WebRequest.Data("Revenue"))
            clsorgshort.Scope = Common.WebRequest.Data("Scope")
            clsorgshort.NoOfFactories = IIf(Common.WebRequest.Data("Factories") = "", "NULL", Common.WebRequest.Data("Factories"))
            clsorgshort.NoOfOffices = IIf(Common.WebRequest.Data("Offices") = "", "NULL", Common.WebRequest.Data("Offices"))
            clsorgshort.NoOfWarehouses = IIf(Common.WebRequest.Data("Warehouses") = "", "NULL", Common.WebRequest.Data("Warehouses"))
            clsorgshort.NoOfStores = IIf(Common.WebRequest.Data("Stores") = "", "NULL", Common.WebRequest.Data("Stores"))
            clsorgshort.NoOfTrucks = IIf(Common.WebRequest.Data("Trucks") = "", "NULL", Common.WebRequest.Data("Trucks"))
            clsorgshort.SingleCreditTerm = CBool2(Common.WebRequest.Data("SingleCreditTerm"))

            clsorgshort.SalesRepID = IIf(Common.WebRequest.Data("SalesRepID") = "", "NULL", ReadyString(Common.WebRequest.Data("SalesRepID")))
            clsorgshort.PriceLevel = Common.WebRequest.AllData("PriceLevel")
            clsorgshort.Terms = IIf(Common.WebRequest.Data("Terms") = "", "NULL", ReadyString(Common.WebRequest.Data("Terms")))
            clsorgshort.Limit = CDec2(Common.WebRequest.Data("Limit"))
            clsorgshort.UnlimitedCredit = If(CBool2(Common.WebRequest.Data("UnlimitedCredit")), 1, 0)
            clsorgshort.RequireCnCApproval = If(CBool2(Common.WebRequest.Data("RequireCnCApproval")), 1, 0)
            clsorgshort.CustomerRequirePoNum = If(CBool2(Common.WebRequest.Data("CustomerRequirePoNum")), 1, 0)
            clsorgshort.Balance = CDec2(Common.WebRequest.Data("Balance"))
            clsorgshort.AsOf = IIf(IsDate(Common.WebRequest.Data("AsOf")), Common.WebRequest.Data("AsOf"), Date.Now())
            clsorgshort.ThirdPartyBegBalance = CDec2(Common.WebRequest.Data("ThirdPartyBalance"))
            clsorgshort.DefaultHotButtonTemplateID = CStr2(Common.WebRequest.Data("DefaultHotButtonTemplateID"))
            clsorgshort.ObjNewVendorPrices = Common.WebRequest.AllData("NewVendorPrices")
            Dim pInput = JsonConvert.DeserializeObject(Of OrganizationShort.SaveNewOrgProspectInput)(Common.WebRequest.InputStreamText)
            clsorgshort.SalesReps = pInput.SalesReps
            clsorgshort.SalesChannelID = pInput.SalesChannelID

            If IsDate(Common.WebRequest.Data("ThirdPartyAsOf")) Then
                clsorgshort.ThirdPartyBegBalanceAsOf = ParseDate(Common.WebRequest.Data("ThirdPartyAsOf"))
            End If

            Dim pOrganizationShortWithSupplierInfo As OrganizationShortWithSupplierInfo

            pOrganizationShortWithSupplierInfo = JsonConvert.DeserializeObject(Of OrganizationShortWithSupplierInfo)(Common.WebRequest.InputStreamText)
            clsorgshort.SaveAsCustomer = pOrganizationShortWithSupplierInfo.SaveAsCustomerProper

            If CBool2(clsorgshort.SaveAsCustomer) Then
                clsorgshort.CusCreditTerms = Common.WebRequest.AllData("CusCreditTerms")
            End If

            strReturn = clsorgshort.SaveNewOrgProspect()

            If IsGuid(strReturn) Then
                If pOrganizationShortWithSupplierInfo.SaveAsSupplierProper _
                AndAlso pOrganizationShortWithSupplierInfo.SupplierInfo IsNot Nothing Then

                    pOrganizationShortWithSupplierInfo.SupplierInfo.Parent = pOrganizationShortWithSupplierInfo
                    pOrganizationShortWithSupplierInfo.SupplierInfo.SupplierID = strReturn
                    pOrganizationShortWithSupplierInfo.SupplierInfo.IsDeactivated = If(clsorgshort.Status = "Active", 0, 1)
                    pOrganizationShortWithSupplierInfo.SupplierInfo.DeactivationDate = clsorgshort.DateTime
                    pOrganizationShortWithSupplierInfo.SupplierInfo.VendorCode = clsorgshort.CodeNumber
                    pOrganizationShortWithSupplierInfo.SupplierInfo.SupplierType = CStr2(Common.WebRequest.Data("SupplierType"))
                    If pOrganizationShortWithSupplierInfo.SupplierInfo.Verify(pError) Then
                        pOrganizationShortWithSupplierInfo.SupplierInfo.Save()
                    Else
                        Return JsonDictionary.Serialize("Results", pError)
                    End If
                End If
            End If

            strReturn = JsonDictionary.Serialize("Results", strReturn)

            Return strReturn

        End Function

        Private Class OrganizationShortWithSupplierInfo
            Public SaveAsCustomer As String
            Public SaveAsSupplier As String
            Public SupplierInfo As SupplierInfoClass
            Public ThirdPartyPlatform As String
            Public ThirdPartyID As String

            Public Class SupplierInfoClass
                Public Parent As OrganizationShortWithSupplierInfo
                Public OrgSupplierID As String
                Public SupplierID As String
                Public ActivationDate As String
                Public IsDeactivated As String
                Public DeactivationDate As String
                Public DeactivationReason As String
                Public Comments As String
                Public IsBan As String
                Public CreditTermsID As String
                Public CreditLimit As String
                Public CreditBegBalance As String
                Public CreditBalanceAsOf As String
                Public SupplierType As String
                Public VendorCode As String

                Public Function Save() As String

                    Dim pOrganization As New Organization()
                    Dim pOut As String

                    pOrganization.ID = OrgSupplierID
                    pOrganization.SupplierID = SupplierID
                    pOrganization.ActivationDate = ActivationDate
                    pOrganization.IsDeactivated = IsDeactivated
                    pOrganization.DeactivationDate = DeactivationDate
                    pOrganization.DeactivationReason = DeactivationReason
                    pOrganization.Comments = Comments
                    pOrganization.IsBan = IsBan
                    pOrganization.CreditTermsID = CreditTermsID
                    pOrganization.CreditLimit = CreditLimit
                    pOrganization.CreditBegBalance = CreditBegBalance
                    pOrganization.CreditBalanceAsOf = CreditBalanceAsOf
                    pOrganization.ThirdPartyPlatform = Parent.ThirdPartyPlatform
                    pOrganization.ThirdPartyID = Parent.ThirdPartyID
                    pOrganization.SupplierType = SupplierType
                    pOrganization.VendorCode = VendorCode

                    pOut = pOrganization.SaveOrganizationSupplier()
                    Return pOut

                End Function

                Public Function Verify(ByRef oError As String) As Boolean

                    oError = Nothing

                    If (Not IsDate(ActivationDate)) Then
                        oError = "ActivationDateError"
                        Return False
                    ElseIf (Convert.ToBoolean(Convert.ToInt32(IsDeactivated))) Then
                        If (Not IsDate(DeactivationDate)) Then
                            oError = "DeactivationDateError"
                            Return False
                        End If
                    End If

                    Return True

                End Function
            End Class

            Public ReadOnly Property SaveAsCustomerProper As Boolean
                Get

                    Return CBool2(SaveAsCustomer)

                End Get
            End Property

            Public ReadOnly Property SaveAsSupplierProper As Boolean
                Get

                    Return CBool2(SaveAsSupplier)

                End Get
            End Property

        End Class

        <HttpPost()>
        Function GetProspectDetailsPeople() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            clsorgshort.PersonID = Common.WebRequest.Data("ID")

            strReturn = clsorgshort.GetProspectDetailsPeople()

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProspectDetails() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            clsorgshort.OrgID = Common.WebRequest.Data("ID")

            strReturn = clsorgshort.GetProspectDetails()

            Return strReturn

        End Function

        Function GetCustomerOrgOutlets()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsorgshort As New OrganizationShort

            Return clsorgshort.GetProspectOutlet(Common.WebRequest.Data("ID"))
        End Function

        <HttpPost>
        Function GetDataOfSelectedPublicOrgPerson() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsorgshort As New OrganizationShort
            Dim pOut As New JsonDictionary
            Dim pSql As String

            If CBool2(Common.WebRequest.Data("IsOrganization")) Then
                Return JsonDictionary.Serialize("Results", clsorgshort.GetProspectOrgDetails(Common.WebRequest.Data("ID")))
            Else
                pSql = "SELECT * FROM MobizLibraryOrganization WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            End If
        End Function

        <HttpPost()>
        Function GetContactDetailsPeople() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            clsorgshort.PersonID = Common.WebRequest.Data("ID")
            clsorgshort.IsCustomer = Common.WebRequest.Data("IsCustomer")

            strReturn = clsorgshort.GetContactDetailsPeople()

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProspectList() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsorgshort As New OrganizationShort
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsorgshort.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsorgshort.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsorgshort.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsorgshort.IsCountPage = False
            clsorgshort.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsorgshort.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            clsorgshort.Status = Request.Form.GetValues("columns[0][search][value]")(0).ToString()
            clsorgshort.Filter = Request.Form.GetValues("columns[1][search][value]")(0).ToString()
            pDT = clsorgshort.GetProspectList()

            Dim a(pDT.Rows.Count - 1) As Object

            For m As Integer = 0 To pDT.Rows.Count - 1
                a(m) = New With {Key .ID = pDT.Rows(m)("ID"),
                                 .ProspectType = pDT.Rows(m)("ProspectType").ToString(),
                                 .ClientName = pDT.Rows(m)("ClientName"),
                                 .ContactPerson = pDT.Rows(m)("ContactPerson"),
                                 .ContactInfo = If(pDT.Rows(m)("Telephone").ToString() <> "", "<div>T: " & pDT.Rows(m)("Telephone") & "</div>", "") &
                                                If(pDT.Rows(m)("Mobile").ToString() <> "", "<div> M: " & pDT.Rows(m)("Mobile") & "</div>", "") &
                                                If(pDT.Rows(m)("Email").ToString() <> "", "<div> E: " & pDT.Rows(m)("Email") & "</div>", ""),
                                 .City = pDT.Rows(m)("City"),
                                 .Country = pDT.Rows(m)("Country"),
                                 .Affiliation = pDT.Rows(m)("Affiliation"),
                                 .Status = If(pDT.Rows(m)("Status") = "1", "Active", "Deactive")}
                ctr = ctr + 1
            Next

            Dim pSql As String = "SELECT COUNT(ID) as Count FROM ( SELECT ID FROM LibraryOrganization UNION ALL SELECT ID FROM LibraryPeople WHERE IsFromOrg = 0 ) as v2"
            clsorgshort.IsCountPage = True
            Dim pDTCountPage As DataTable = clsorgshort.GetProspectList() 'Common.WebRequest.ProData.Execute(pSql)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsorgshort.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)

        End Function

        <HttpPost()>
        Function getCityNameList() As String
            Dim strReturn As String
            Dim dataTblCityList As DataTable

            dataTblCityList = Common.WebRequest.ProData.Execute("SELECT ID, Name FROM MobizLibraryCities ORDER BY Name")


            strReturn = JsonDictionary.Serialize("CityData", dataTblCityList)

            Return strReturn

        End Function

        <HttpPost()>
        Function getCountryNameList() As String
            Dim strReturn As String
            Dim dataTblCityList As DataTable

            dataTblCityList = Common.WebRequest.ProData.Execute("SELECT ID, Name FROM MobizLibraryCountries ORDER BY Name")


            strReturn = JsonDictionary.Serialize("CountryData", dataTblCityList)

            Return strReturn

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

        <HttpPost()>
        Function CheckOrgUniqueOrgName() As String
            Dim clsOrganization As New OrganizationShort
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
            Dim isFull As Boolean
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationFowstData", clsOrganization.getOrgFOWSTInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgFOWST() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("OrgID") 'WebSession.Instance.OrganizationID
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getOrgRepContactTelList() As String
            Dim strID As String
            Dim clsOrganization As New OrganizationShort
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
            Dim clsOrganization As New OrganizationShort
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
            Dim clsOrganization As New OrganizationShort
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
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("OrganizationRepData", clsOrganization.getOrgRepInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveOrgRep() As String
            Dim clsOrganization As New OrganizationShort
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgRepID")
            clsOrganization.OrganizationID = Common.WebRequest.Data("OrgID") 'WebSession.Instance.OrganizationID
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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsOrganization.ID = Common.WebRequest.Data("OrgRepID")

            strReturn = JsonDictionary.Serialize("OrganizationRepData", IIf(clsOrganization.SetInActiveOrgRep(), "TRUE", "FALSE"))

            Return strReturn

        End Function

        Function UploadContacts(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String, ByVal IsCustomer As String) As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim pSql As String
            Dim pDT As DataTable
            Dim pFilename As String
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim dtaExcelData = New DataTable
            Dim strHeader = {"Organization Name", "First Name", "Middle Name", "Last Name", "Country Code1", "Area or Telco Code1", "Number1", "Country Code2", "Area or Telco Code2", "Number2", "Email Address", "TIN", "3rd Party ID", "Beginning Balance"}
            Dim p As New InsertUpdate
            Dim err As String = ""
            If (Not objFile Is Nothing) Then

                pFilename = pSourceURL & "\Contacts\" & OrgID & "_ContactTemplate.xls"
                CreateFolder(pFilename)
                objFile.SaveAs(pFilename)

                Try
                    dtaExcelData = New DataTable
                    dtaExcelData = ExcelToTable(pSourceURL & "\Contacts\" & OrgID & "_ContactTemplate.xls", strHeader, "")

                    Common.WebRequest.ProData.BeginTransaction()

                    For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                        Dim countrycode1 As String = ""
                        Dim areacode1 As String = ""
                        Dim number1 As String = ""
                        Dim countrycode2 As String = ""
                        Dim areacode2 As String = ""
                        Dim number2 As String = ""
                        Dim areatelcoType1 As String = ""
                        Dim areatelcoType2 As String = ""
                        Dim thirdpartyid As String = CStr2(dtaExcelData.Rows(i).Item("3rd Party ID"))
                        Dim begbal As String = CStr2(dtaExcelData.Rows(i).Item("Beginning Balance"))
                        Dim objTel(1) As Object
                        Dim objMobile(1) As Object

                        'strUploadContactStatus = "Uploading " & (i + 1) & " of " & dtaExcelData.Rows.count & ". Please wait ..."

                        If Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code1"))) <> "" Then
                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryCountries WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code1"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            countrycode1 = pDT.Rows(0)("ID").ToString()

                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryAreaCodes WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            areatelcoType1 = "Tel"

                            If pDT.Rows.Count = 0 Then
                                pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryTelcoPrefixes WHERE Prefix = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1"))))
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count = 0 Then
                                    areacode1 = WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1"))) & Trim(CStr2(dtaExcelData.Rows(i).Item("Number1"))))("ID").ToString()
                                    'Return "We can't find area code or mobile prefix no. " & Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1")))
                                Else
                                    areacode1 = pDT.Rows(0)("ID").ToString()
                                End If
                                areatelcoType1 = "Mobile"
                            Else
                                areacode1 = pDT.Rows(0)("ID").ToString()
                            End If

                            number1 = Trim(CStr2(dtaExcelData.Rows(i).Item("Number1")))
                        End If

                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                        If Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code2"))) <> "" Then
                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryCountries WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code2"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            countrycode2 = pDT.Rows(0)("ID").ToString()

                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryAreaCodes WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            areatelcoType2 = "Tel"

                            If pDT.Rows.Count = 0 Then
                                pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryTelcoPrefixes WHERE Prefix = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2"))))
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count = 0 Then
                                    areacode2 = WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2"))) & Trim(CStr2(dtaExcelData.Rows(i).Item("Number2"))))("ID").ToString()
                                    'Return "We can't find area code or mobile prefix no. " & Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2")))
                                Else
                                    areacode2 = pDT.Rows(0)("ID").ToString()
                                End If

                                areatelcoType2 = "Mobile"
                            Else
                                areacode2 = pDT.Rows(0)("ID").ToString()
                            End If

                            number2 = Trim(CStr2(dtaExcelData.Rows(i).Item("Number2")))
                        End If

                        If Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))) <> "" And (Trim(CStr2(dtaExcelData.Rows(i).Item("First Name"))) <> "" Or Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name"))) <> "" Or Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name"))) <> "") Then
                            Dim existedOrgID As String
                            Dim PersonID As String = Guid.NewGuid.ToString()
                            pSql = "SELECT TOP 1 ID FROM MobizLibraryOrganization WHERE CompanyName = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)

                            If pDT.Rows.Count > 0 Then
                                existedOrgID = pDT.Rows(0)("ID").ToString()
                            Else
                                existedOrgID = Guid.NewGuid.ToString()
                                'pSql = "INSERT INTO LibraryOrganization(ID,CompanyName,OfficeAddressBuildingNo,OfficeAddressStreet,OfficeAddressCityID,OfficeAddressCountryID,Background,Mission,Vision,FoundingDate,UserID) " & vbCrLf &
                                '        "VALUES (" & ReadyString(existedOrgID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name")))) & ",'','',NULL,NULL,'','','',''," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                                p = New InsertUpdate("MobizLibraryOrganization")
                                p("ID") = ReadyString(existedOrgID)
                                p("CompanyName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))))
                                p("OfficeAddressBuildingNo") = ReadyString("")
                                p("OfficeAddressStreet") = ReadyString("")
                                p("OfficeAddressCityID") = "NULL"
                                p("OfficeAddressCountryID") = "NULL"
                                p("Background") = ReadyString("")
                                p("Mission") = ReadyString("")
                                p("Vision") = ReadyString("")
                                p("FoundingDate") = ReadyString("")
                                p("IsOrganization") = ReadyString("1")
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = "getdate()"
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                If AddToCustomerOrSupplier(existedOrgID, IsCustomer, thirdpartyid, begbal, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            'pSql = "INSERT INTO LibraryPeople(ID,FirstName,MiddleName,Surname,Position,PositionDescription,IsFromOrg,UserID) " & vbCrLf &
                            '        "VALUES (" & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name")))) & ",'','','1'," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                            p = New InsertUpdate("MobizLibraryPeople")
                            p("ID") = ReadyString(PersonID)
                            p("FirstName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name"))))
                            p("MiddleName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name"))))
                            p("Surname") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name"))))
                            p("TIN") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("TIN"))))
                            p("Position") = ReadyString("")
                            p("PositionDescription") = ReadyString("")
                            p("IsFromOrg") = ReadyString("1")
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            'pSql = "INSERT INTO LibraryOrganizationContactPerson(ID,OrganizationID,PeopleID,UserID,LastUpdated,IsUseForPersonOrgAffiliates) " & vbCrLf &
                            '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(existedOrgID) & "," & ReadyString(PersonID) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'','0')"

                            p = New InsertUpdate("MobizLibraryOrganizationContactPerson")
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("OrganizationID") = ReadyString(existedOrgID)
                            p("PeopleID") = ReadyString(PersonID)
                            p("IsUseForPersonOrgAffiliates") = ReadyString("0")
                            p("LastUpdated") = ReadyString("")
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If areatelcoType1 = "Tel" Then
                                If SaveCustomerTel(False, PersonID, countrycode1, areacode1, number1, err) <> "saved" Then
                                    Return err
                                End If
                            ElseIf areatelcoType1 = "Mobile" Then
                                If SaveCustomerMobile(False, PersonID, countrycode1, areacode1, number1, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            If areatelcoType2 = "Tel" Then
                                If SaveCustomerTel(False, PersonID, countrycode2, areacode2, number2, err) <> "saved" Then
                                    Return err
                                End If
                            ElseIf areatelcoType2 = "Mobile" Then
                                If SaveCustomerMobile(False, PersonID, countrycode2, areacode2, number2, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            If Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))) <> "" Then
                                'pSql = "INSERT INTO LibraryPeopleContactEmail(ID,PeopleID,EmailAddress,UserID,EmailType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address")))) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("EmailAddress") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))))
                                p("EmailType") = ReadyString("")
                                p("LastUpdated") = ReadyString("")
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                        ElseIf Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))) = "" Then
                            Dim PersonID As String = Guid.NewGuid.ToString()
                            'pSql = "INSERT INTO LibraryPeople(ID,FirstName,MiddleName,Surname,Position,PositionDescription,IsFromOrg,UserID) " & vbCrLf &
                            '        "VALUES (" & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name")))) & ",'','','0'," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                            p = New InsertUpdate("MobizLibraryOrganization")
                            p("ID") = ReadyString(PersonID)
                            p("PersonFirstName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name"))))
                            p("PersonMiddleName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name"))))
                            p("PersonSurname") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name"))))
                            p("TinNumber") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("TIN"))))
                            'p("Position") = ReadyString("")
                            'p("PositionDescription") = ReadyString("")
                            p("LastUpdated") = "getdate()"
                            p("IsOrganization") = ReadyString("0")
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If AddToCustomerOrSupplier(PersonID, IsCustomer, thirdpartyid, begbal, err) <> "saved" Then
                                Return err
                            End If

                            If areatelcoType1 = "Tel" Then
                                If SaveCustomerTel(False, PersonID, countrycode1, areacode1, number1, err) <> "saved" Then
                                    Return err
                                End If
                            ElseIf areatelcoType1 = "Mobile" Then
                                If SaveCustomerMobile(False, PersonID, countrycode1, areacode1, number1, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            If areatelcoType2 = "Tel" Then
                                If SaveCustomerTel(False, PersonID, countrycode2, areacode2, number2, err) <> "saved" Then
                                    Return err
                                End If
                            ElseIf areatelcoType2 = "Mobile" Then
                                If SaveCustomerMobile(False, PersonID, countrycode2, areacode2, number2, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            If Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))) <> "" Then
                                'pSql = "INSERT INTO LibraryPeopleContactEmail(ID,PeopleID,EmailAddress,UserID,EmailType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address")))) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("MobizLibraryPeopleContactEmail")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("EmailAddress") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))))
                                p("EmailType") = ReadyString("")
                                p("LastUpdated") = ReadyString("")
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                        Else
                            Dim OrganizationID As String = Guid.NewGuid.ToString()

                            'pSql = "INSERT INTO LibraryOrganization(ID,CompanyName,OfficeAddressBuildingNo,OfficeAddressStreet,OfficeAddressCityID,OfficeAddressCountryID,Background,Mission,Vision,FoundingDate,UserID) " & vbCrLf &
                            '        "VALUES (" & ReadyString(OrganizationID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name")))) & ",'','',NULL,NULL,'','','',''," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                            p = New InsertUpdate("MobizLibraryOrganization")
                            p("ID") = ReadyString(OrganizationID)
                            p("CompanyName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))))
                            p("OfficeAddressBuildingNo") = ReadyString("")
                            p("OfficeAddressStreet") = ReadyString("")
                            p("OfficeAddressCityID") = "NULL"
                            p("OfficeAddressCountryID") = "NULL"
                            p("Background") = ReadyString("")
                            p("Mission") = ReadyString("")
                            p("Vision") = ReadyString("")
                            p("TinNumber") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("TIN"))))
                            p("FoundingDate") = ReadyString("")
                            p("LastUpdated") = "getdate()"
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If AddToCustomerOrSupplier(OrganizationID, IsCustomer, thirdpartyid, begbal, err) <> "saved" Then
                                Return err
                            End If

                            If areatelcoType1 = "Tel" Then
                                If SaveCustomerTel(True, OrganizationID, countrycode1, areacode1, number1, err) <> "saved" Then
                                    Return err
                                End If
                            ElseIf areatelcoType1 = "Mobile" Then
                                If SaveCustomerMobile(True, OrganizationID, countrycode1, areacode1, number1, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            If areatelcoType2 = "Tel" Then
                                If SaveCustomerTel(True, OrganizationID, countrycode2, areacode2, number2, err) <> "saved" Then
                                    Return err
                                End If
                            ElseIf areatelcoType2 = "Mobile" Then
                                If SaveCustomerMobile(True, OrganizationID, countrycode2, areacode2, number2, err) <> "saved" Then
                                    Return err
                                End If
                            End If

                            If Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))) <> "" Then
                                'pSql = "INSERT INTO LibraryOrganizationContactEmail(ID,OrganizationID,EmailAddress,UserID,EmailType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(OrganizationID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).item("Email Address")))) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("MobizLibraryOrganizationContactEmail")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(OrganizationID)
                                p("EmailAddress") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))))
                                p("EmailType") = ReadyString("")
                                p("LastUpdated") = ReadyString("")
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        End If
                    Next

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Common.WebRequest.ProData.CommitTransaction()

                    Return "Saved"
                Catch ex As Exception
                    Return ex.Message
                End Try
            End If
        End Function

        Function SaveCustomerMobile(ByVal IsOrg As Boolean, ByVal CusID As String, ByVal countrycodeid As String, ByVal areacode As String, ByVal number As String, ByRef err As String)
            Try
                Dim p, pSql

                If IsOrg Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactMobile")
                    p("OrganizationID") = ReadyString(CusID)
                Else
                    p = New InsertUpdate("MobizLibraryPeopleContactMobile")
                    p("PeopleID") = ReadyString(CusID)
                End If

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("CountryID") = ReadyString(countrycodeid)
                p("TelCoPrefixID") = ReadyString(areacode)
                p("ContactNo") = ReadyString(number)
                p("ContactType") = ReadyString("")
                p("LastUpdated") = ReadyString("")
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return "saved"
            Catch ex As Exception
                AppendToLog(ex.Message)
                err = ex.Message
                Return ex.Message
            End Try
        End Function

        Function SaveCustomerTel(ByVal IsOrg As Boolean, ByVal CusID As String, ByVal countrycodeid As String, ByVal areacode As String, ByVal number As String, ByRef err As String)
            Try
                Dim p, pSql

                If IsOrg Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactTel")
                    p("OrganizationID") = ReadyString(CusID)
                Else
                    p = New InsertUpdate("MobizLibraryPeopleContactTel")
                    p("PeopleID") = ReadyString(CusID)
                End If

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("CountryID") = ReadyString(countrycodeid)
                p("AreaCodeID") = ReadyString(areacode)
                p("ContactNo") = ReadyString(number)
                p("ExtensionNo") = ReadyString("")
                p("ContactType") = ReadyString("")
                p("LastUpdated") = ReadyString("")
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return "saved"
            Catch ex As Exception
                AppendToLog(ex.Message)
                err = ex.Message
                Return ex.Message
            End Try
        End Function

        Function AddToCustomerOrSupplier(ByVal ID As String, ByVal IsCustomer As String, ByVal ThirdPartyID As String, ByVal Begbal As String, ByRef err As String) As String
            Dim codenumber As String = CStr2(New OrganizationShort().GenerateCodeNumber(CBool2(IsCustomer)))
            Try
                If CBool2(IsCustomer) Then
                    Dim pSql As String = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(ID)
                    Dim count As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                    If count = 0 Then
                        Dim p As New InsertUpdate("LibraryOrganizationCustomer")

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("OrganizationID") = ReadyString(ID)
                        p("ThirdPartyID") = ReadyString(ThirdPartyID)
                        p("CustomerCode") = ReadyString(codenumber)
                        p("CreditBegBalance") = ReadyString(Begbal)
                        p("LastUpdated") = "getdate()"

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Else
                    Dim pSql As String = "SELECT ID FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(ID)
                    Dim count As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                    If count = 0 Then
                        Dim p As New InsertUpdate("LibraryOrganizationSupplier")

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("SupplierID") = ReadyString(ID)
                        p("ActivationDate") = "getdate()"
                        p("ThirdPartyID") = ReadyString(ThirdPartyID)
                        p("VendorCode") = ReadyString(codenumber)
                        p("CreditBegBalance") = ReadyString(Begbal)
                        p("LastUpdated") = "getdate()"

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                End If

                Return "saved"
            Catch ex As Exception
                err = ex.Message
                AppendToLog(err)
                Return ex.Message
            End Try
        End Function

        Function DeactivateAllCustomerRecords() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim msg As String

            Try
                Dim pSql As String = "UPDATE MobizLibraryOrganization SET LastUpdated = getdate()" & vbCrLf &
                                    " WHERE ID IN " & vbCrLf &
                                    " (SELECT OrganizationID FROM LibraryOrganizationCustomer) "
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "UPDATE LibraryOrganizationCustomer SET Deactivated = " & Common.WebRequest.Data("Status") & ", " & vbCrLf &
                        " LastUpdated = getdate()" & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                msg = "saved"
            Catch ex As Exception
                msg = ex.Message
            End Try

            Return JsonDictionary.Serialize("Results", msg)

        End Function

        Function DeactivateAllSupplierRecords() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim msg As String

            Try
                Dim pSql As String = "UPDATE MobizLibraryOrganization SET LastUpdated = getdate()" & vbCrLf &
                                    " WHERE ID IN " & vbCrLf &
                                    " (SELECT SupplierID FROM LibraryOrganizationSupplier) "
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "UPDATE LibraryOrganizationSupplier SET Deactivated = " & Common.WebRequest.Data("Status") & ", " & vbCrLf &
                        " LastUpdated = getdate()" & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                msg = "saved"
            Catch ex As Exception
                msg = ex.Message
            End Try

            Return JsonDictionary.Serialize("Results", msg)

        End Function
    End Class

End Namespace