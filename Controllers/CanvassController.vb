Imports Aspose.Cells
Imports Newtonsoft.Json
Imports System.IO
Imports Ionic.Zip
Imports System.Collections.Generic

Namespace WebProj
    Public Class CanvassController
        Inherits System.Web.Mvc.Controller

        Shared strExcelFileProgress, canvassid As String
        Shared isOkay As Boolean = True
        Shared isUploading As Boolean = True
        Shared mBusiness As Business

        Dim jss = New JavaScriptSerializer()

        Function Index() As ActionResult
            Return PartialView("~/Views/Canvass/PublicLink.vbhtml")
            'Return View()
        End Function

        Function PublicLink() As ActionResult
            Return View()
        End Function

        Function CheckIfUserCanLinkSku()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT * FROM LibraryPriceCanvassAllowUserToLinkSKU WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "ok")
            Else
                Return JsonDictionary.Serialize("Results", "denied")
            End If
        End Function

        Function CheckUserAccountToLinkSku()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPeople As New People
            Dim dtaTable As New DataTable
            Dim intCntr As Integer

            clsPeople.UserName = Common.WebRequest.Data("Username")
            clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
            dtaTable = clsPeople.CheckPeopleUserAccount()

            If (dtaTable.Rows.Count > 0) Then
                If (ProPack.Security.Hash.Verify(clsPeople.UserPassword, dtaTable.Rows(0).Item("UserPassword").ToString())) Then
                    Dim pSql As String = "SELECT * FROM LibraryPriceCanvassAllowUserToLinkSKU WHERE PeopleID = " & ReadyString(dtaTable.Rows(0).Item("PeopleID").ToString())

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                        Return JsonDictionary.Serialize("Results", "ok")
                    Else
                        Return JsonDictionary.Serialize("Results", "invalid")
                    End If
                Else
                    Return JsonDictionary.Serialize("Results", "invalid")
                End If
            Else
                Return JsonDictionary.Serialize("Results", "invalid")
            End If
        End Function

        Function SaveUserToLinkSKU()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim arrUser As Object = Common.WebRequest.AllData("data")
                Dim pSql As String

                Common.WebRequest.ProData.BeginTransaction()

                For Each user As Object In arrUser
                    Dim p As New InsertUpdate("LibraryPriceCanvassAllowUserToLinkSKU")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = ReadyString(user("ID"))
                    p("ModifiedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = "getdate()"

                    pSql = "SELECT ID FROM LibraryPriceCanvassAllowUserToLinkSKU WHERE PeopleID = " & ReadyString(user("ID"))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next user

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetUserToLinkSKU()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT link.*, " & vbCrLf &
                        " mod.FirstName + ' ' + mod.Surname as ModifiedBy, " & vbCrLf &
                        " users.Surname + ', ' + users.FirstName + ' ' + users.MiddleName as EmployeeUser " & vbCrLf &
                        " FROM LibraryPriceCanvassAllowUserToLinkSKU as link " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as mod ON mod.ID = link.ModifiedByID " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as users ON link.PeopleID = users.ID " & vbCrLf &
                        " ORDER BY mod.Surname ASC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DeleteUserToLinkSKU()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "DELETE FROM LibraryPriceCanvassAllowUserToLinkSKU WHERE ID = " & ReadyString(Common.WebRequest.Data("ID").ToString())
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "success")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetLinkData()
            Dim pSql As String = "SELECT * FROM LibraryPriceCanvassShareableLink WHERE PublicCode = " & ReadyString(Common.WebRequest.Data("code")) & vbCrLf &
                                " COLLATE Latin1_General_CS_AS "
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function CheckLinkIfValid()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String

            'pSql = "SELECT vendor.*, org.CompanyName as VendorName FROM LibraryPriceCanvassVendor as vendor " & vbCrLf &
            '                    " JOIN MobizLibraryOrganization as org ON vendor.VendorID = org.ID " & vbCrLf &
            '                    " WHERE vendor.CanvassID = " & ReadyString(Common.WebRequest.Data("cid")) & vbCrLf &
            '                    " AND vendor.CanvassVendorCode = " & ReadyString(Common.WebRequest.Data("code")) & vbCrLf &
            '                    " COLLATE Latin1_General_CS_AS "
            pSql = "SELECT * FROM LibraryPriceCanvass " & vbCrLf &
                    " OUTER APPLY (SELECT VendorID FROM LibraryPriceCanvassVendor WHERE CanvassVendorCode = " & ReadyString(Common.WebRequest.Data("vcode")) & ") as v " & vbCrLf &
                    " WHERE ID = " & ReadyString(Common.WebRequest.Data("cid")) & vbCrLf
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                If Not CBool2(pDT.Rows(0)("NeverExpireLink")) Then
                    If Date.Compare(Date.Now().ToString("MMM dd, yyyy"), pDT.Rows(0)("LinkExpirationDate")) = 0 Then
                        'Dim p As New InsertUpdate("LibraryPriceCanvass")

                        'p("IsActiveLink") = 0

                        'Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0)("ID").ToString())))
                        pDT = New DataTable
                    End If
                End If
            End If

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetLinkCanvassData()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return Json(New With {Key .data = GetSpecificCanvassData(Common.WebRequest.Data("PriceCanvassID"), Common.WebRequest.ProData)})
        End Function

        Function SaveLinkedVendorData()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim vendorcode As String = Common.WebRequest.Data("vcode")
                Dim data As Object = Common.WebRequest.AllData("data")

                Common.WebRequest.ProData.BeginTransaction()

                For Each price As Object In data("data")
                    Dim errmsg As String = ""
                    If SaveVendorPrice(price, data("DBID"), data("ID"), errmsg) = False Then
                        Common.WebRequest.ProData.RollbackTransaction()
                        Return JsonDictionary.Serialize("Results", errmsg)
                    End If
                Next price

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function DownloadCanvassTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            Dim fileDetail As IO.FileInfo = My.Computer.FileSystem.GetFileInfo(Common.ServerPath & "/FileTemplates/Canvass.xlsx")
            fileDetail.IsReadOnly = False
            pFS = New IO.FileStream(Common.ServerPath & "/FileTemplates/Canvass.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "FileTemplates\Canvass.xlsx"
            pContentType = "application/xls"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "CanvassTemplate.xlsx")
        End Function

        Function UploadTemplate()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim file As Object = Request.Files
            Dim canvassdetails As Object = jss.Deserialize(Of Object)(Request.Form("data"))
            Dim template As HttpPostedFileBase = file(0)
            Dim pFilename, pSql As String
            Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\"
            Dim dtaExcelData = New DataTable
            Dim row As Object
            Dim canvassnumber As Integer = 0
            Dim rownumber As Integer = 0
            Dim result As String
            Dim strHeader = {"Product Number", "Product Name", "Brand", "Category", "Vendor Product Comment", "Unit Measure", "List Price", "Trade Discount", "Trade Discount Label", "Price Net of Trade Discount", "Mobiz Vendor Code", "Vendor Name", "3rd Party Vendor Code"}
            Dim p As New InsertUpdate("LibraryPriceCanvass")
            Dim pFS As IO.FileStream
            isOkay = True

            If Not System.IO.Directory.Exists(pSourceURL) Then
                System.IO.Directory.CreateDirectory(pSourceURL)
            End If

            pFilename = pSourceURL & Guid.NewGuid.ToString() & "_Canvass.xls"
            CreateFolder(pFilename)
            template.SaveAs(pFilename)

            pFS = New IO.FileStream(pFilename, IO.FileMode.Open)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pFilename, strHeader, "")

                If dtaExcelData Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid Template. Please upload valid canvass template.")
                End If

                Common.WebRequest.ProData.BeginTransaction()

                p("CanvassName") = ReadyString(canvassdetails("Name"))
                p("LastUpdated") = "getdate()"
                p("Type") = ReadyString(canvassdetails("Type"))

                If canvassdetails("CanvassID") = "" Then
                    pSql = "SELECT MAX(CanvassNumber) as CanvassNumber FROM LibraryPriceCanvass WHERE ISNULL(Type, 'vendor') = " & ReadyString(canvassdetails("Type"))
                    row = Common.WebRequest.ProData.Execute(pSql).Rows

                    If row.Count = 0 Then
                        canvassnumber += 1
                    Else
                        canvassnumber = CDec2(row(0)("CanvassNumber")) + 1
                    End If

                    canvassid = Guid.NewGuid.ToString()

                    p("ID") = ReadyString(canvassid)
                    p("CanvassNumber") = canvassnumber
                    p("CreatedBy") = ReadyString(Request.Params.Get("UID").ToString())
                    pSql = p.GetInsert()
                Else
                    canvassid = canvassdetails("CanvassID")
                    pSql = p.GetUpdate(" ID = " & ReadyString(canvassid))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                pSql = " SELECT MAX(RowNumber) as RowNumber FROM LibraryPriceCanvassItems WHERE CanvassID = " & ReadyString(canvassid)
                row = Common.WebRequest.ProData.Execute(pSql).Rows

                If row.Count = 0 Then
                    rownumber = 0
                Else
                    rownumber = CDec2(row(0)("RowNumber")) + 1
                End If

                Static sSL As New Threading.SpinLock
                strExcelFileProgress = JsonDictionary.Serialize("Uploading", "Uploading data from Excel File. This might take a few moments. Please wait ...")
                Dim dbname As String = Common.WebRequest.ProData.GetCurrentDatabase()
                Dim noOfRecordsToBeUploaded As Integer = 0
                isUploading = True

                For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                    Dim col As Object = dtaExcelData.Rows(i)
                    Dim productno As String = Trim(CStr2(col.Item("Product Number")))
                    Dim productname As String = Trim(CStr2(col.Item("Product Name")))
                    Dim listprice As String = Trim(CStr2(col.Item("List Price")))
                    Dim mobizvendorcode As String = Trim(CStr2(col.Item("Mobiz Vendor Code")))
                    Dim tradediscount As String = Trim(CStr2(col.Item("Trade Discount")))
                    Dim vendorcol, msg As String

                    If productno = "" Then
                        If CBool2(canvassdetails("IsFirstUploadCanvass")) Then
                            isOkay = False
                            strExcelFileProgress = JsonDictionary.Serialize("NoSKURecords", "Please enter Product No. in row number " & (i + 2))
                            Exit For
                        Else
                            Continue For
                        End If
                    End If

                    If productname = "" Then
                        isOkay = False
                        strExcelFileProgress = JsonDictionary.Serialize("Error", "Please enter Product Name in row number " & (i + 2))
                        Exit For
                    End If

                    If tradediscount <> "" Then
                        Dim disc As Object = Split(tradediscount, ",")
                        Dim err As Boolean = False

                        For Each d As Object In disc
                            If CDec2(d) < 0 Then
                                err = True
                                strExcelFileProgress = JsonDictionary.Serialize("Error", "Negative Trade Discount is not allowed. See in row number " & (i + 2))
                                Exit For
                            End If

                            If Not IsNumeric(d) Then
                                err = True
                                strExcelFileProgress = JsonDictionary.Serialize("Error", "Invalid Trade Discount. See row number " & (i + 2) & " <i>(Separate trade discounts with commas and do not include % symbol. Example: 5,2,3 )</i>")
                                Exit For
                            End If
                        Next

                        If err Then
                            isOkay = False
                            Exit For
                        End If
                    End If

                    pSql = "SELECT pim.ID, pim.ProductNo " & vbCrLf &
                            " FROM LibraryOrganizationPIM as orgpim " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = orgpim.ProductID " & vbCrLf &
                            " WHERE pim.ProductNo = " & ReadyString(productno)
                    row = Common.WebRequest.ProData.Execute(pSql).Rows

                    If row.Count = 0 Then
                        isOkay = False
                        strExcelFileProgress = JsonDictionary.Serialize("Error", "Product No. in row " & (i + 2) & " was not found. Please enter an existing Product Number. You can search for the product code by going to Home > System Administration > PIM and then look up the product code.")
                        Exit For
                    End If

                    If Not IsNumeric(listprice) And listprice <> "" Then
                        isOkay = False
                        strExcelFileProgress = JsonDictionary.Serialize("Error", "Please enter valid List Price in row number " & i + 2)
                        Exit For
                    End If

                    If mobizvendorcode <> "" Then
                        vendorcol = "VendorCode"
                        msg = "vendor code"
                    Else
                        vendorcol = "ThirdPartyID"
                        msg = "3rd party ID"
                        mobizvendorcode = Trim(CStr2(col.Item("3rd Party Vendor Code")))
                    End If

                    pSql = "SELECT SupplierID, " & vendorcol & vbCrLf &
                            " FROM LibraryOrganizationSupplier " & vbCrLf &
                            " WHERE " & vendorcol & " = " & ReadyString(mobizvendorcode)
                    row = Common.WebRequest.ProData.Execute(pSql).Rows

                    If row.Count = 0 Then
                        isOkay = False
                        strExcelFileProgress = JsonDictionary.Serialize("Error", "Mobiz Vendor or 3rd Party ID code in row " & (i + 2) & " was not found. Please enter an existing " & msg & ". You can search for the " & msg & " by going to Home > System Administration > Vendor/ Supplier List and then look up the vendor and their respective " & msg & " or you can ask assistance from the system administrator.")
                        Exit For
                    End If
                    noOfRecordsToBeUploaded += 1
                Next

                If isOkay Then
                    strExcelFileProgress = JsonDictionary.Serialize("Uploading", "Uploading 1 of " & noOfRecordsToBeUploaded & " data from Excel File. Please wait...")
                    TimedThread(
                        Sub()

                            Try
                                Dim err As String = ""
                                isOkay = SaveTemplateData(dtaExcelData, canvassid, rownumber, dbname, noOfRecordsToBeUploaded, canvassdetails, err)

                                If isOkay Then
                                    isUploading = False 'done
                                Else
                                    isUploading = True
                                    strExcelFileProgress = JsonDictionary.Serialize("Error", err)
                                End If

                                If (IO.File.Exists(pFilename)) Then
                                    IO.File.Delete(pFilename)
                                End If
                            Catch ex As Exception
                                strExcelFileProgress = JsonDictionary.Serialize("Error", "Encountered error while isUploading data from Excel File. " + Truncate(ex.Message, 300))
                                AppendToLog(ex.Message)
                            Finally
                                If sSL.IsHeldByCurrentThread Then
                                    ' do something here
                                    sSL.Exit()
                                End If
                            End Try

                        End Sub
                    )
                    Common.WebRequest.ProData.CommitTransaction()
                Else
                    Common.WebRequest.ProData.RollbackTransaction()
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
                strExcelFileProgress = JsonDictionary.Serialize("Error", ex.Message)
            End Try

            Return strExcelFileProgress
        End Function

        Function SaveTemplateData(ByVal dtaExcelData As DataTable, ByVal canvassid As String, ByVal rownumber As String, ByVal dbname As String, ByVal noOfRecordsToBeUploaded As Integer, ByVal canvassdetails As Object, ByRef err As String) As Boolean
            Dim pSql As String
            Dim p As New InsertUpdate("LibraryPriceCanvass")
            Dim row As Object

            isOkay = True

            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(dbname))

            mBusiness.ProData.BeginTransaction()

            Dim pc As New LibraryPriceCanvass(mBusiness.ProData, mBusiness)

            Try
                For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                    Dim col As Object = dtaExcelData.Rows(i)
                    Dim productno As String = Trim(CStr2(col.Item("Product Number")))
                    Dim productname As String = Trim(CStr2(col.Item("Product Name")))
                    Dim vendorproductcomment As String = Trim(CStr2(col.Item("Vendor Product Comment")))
                    Dim listprice As String = Trim(CStr2(col.Item("List Price")))
                    Dim mobizvendorcode As String = Trim(CStr2(col.Item("Mobiz Vendor Code")))
                    Dim tradediscount As String = Trim(CStr2(col.Item("Trade Discount")))
                    Dim tradediscountlabel As String = Trim(CStr2(col.Item("Trade Discount Label")))
                    Dim tradediscountnetprice As String = Trim(CStr2(col.Item("Price Net of Trade Discount")))
                    Dim productID, vendorid, vendorcol, msg As String

                    strExcelFileProgress = JsonDictionary.Serialize("Uploading", "Uploading " & (i + 1) & " of " & noOfRecordsToBeUploaded & " data from Excel File. Please wait...")
                    isUploading = True

                    If Not CBool2(canvassdetails("IsFirstUploadCanvass")) And productno = "" Then
                        Continue For
                    End If

                    pSql = "SELECT pim.ID, pim.ProductNo " & vbCrLf &
                            " FROM LibraryOrganizationPIM as orgpim " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = orgpim.ProductID " & vbCrLf &
                            " WHERE pim.ProductNo = " & ReadyString(productno)
                    row = mBusiness.ProData.Execute(pSql).Rows
                    productID = row(0)("ID").ToString()

                    Dim canvassitemid As String

                    p = New InsertUpdate("LibraryPriceCanvassItems")

                    p("CanvassID") = ReadyString(canvassid)
                    p("SKUID") = ReadyString(productID)
                    If CBool2(canvassdetails("IsFinalize")) = False Then 'only draft trans can change description
                        p("Description") = ReadyString(productname)
                    End If

                    pSql = "SELECT ID FROM LibraryPriceCanvassItems " & vbCrLf &
                            " WHERE CanvassID = " & ReadyString(canvassid) & vbCrLf &
                            " AND SKUID = " & ReadyString(productID)
                    row = mBusiness.ProData.Execute(pSql).Rows

                    If row.Count = 0 Then
                        canvassitemid = Guid.NewGuid.ToString()
                        p("RowNumber") = rownumber
                        p("ID") = ReadyString(canvassitemid)
                        pSql = p.GetInsert()
                    Else
                        canvassitemid = row(0)("ID").ToString()
                        pSql = p.GetUpdate(" ID = " & ReadyString(canvassitemid))
                    End If

                    mBusiness.ProData.Execute(pSql)

                    If mobizvendorcode <> "" Then
                        vendorcol = "VendorCode"
                        msg = "vendor code"
                    Else
                        vendorcol = "ThirdPartyID"
                        msg = "3rd party ID"
                        mobizvendorcode = Trim(CStr2(col.Item("3rd Party Vendor Code")))
                    End If

                    If mobizvendorcode <> "" Then
                        pSql = "SELECT SupplierID, " & vendorcol & vbCrLf &
                            " FROM LibraryOrganizationSupplier " & vbCrLf &
                            " WHERE " & vendorcol & " = " & ReadyString(mobizvendorcode)
                        row = mBusiness.ProData.Execute(pSql).Rows
                        vendorid = row(0)("SupplierID").ToString()

                        pSql = "SELECT ID FROM LibraryPriceCanvassVendor " & vbCrLf &
                            " WHERE CanvassID = " & ReadyString(canvassid) & vbCrLf &
                            " AND VendorID = " & ReadyString(vendorid)
                        row = mBusiness.ProData.Execute(pSql).Rows
                        Dim canvassvendorid As String = Guid.NewGuid.ToString()

                        If row.Count = 0 Then
                            p = New InsertUpdate("LibraryPriceCanvassVendor")

                            p("ID") = ReadyString(canvassvendorid)
                            p("CanvassID") = ReadyString(canvassid)
                            p("VendorID") = ReadyString(vendorid)

                            mBusiness.ProData.Execute(p.GetInsert())
                        Else
                            canvassvendorid = row(0)("ID").ToString()
                        End If

                        Dim resultTDNetPrice As Decimal = 0
                        Dim povendorprice As Decimal = 0

                        If tradediscountnetprice = "" And tradediscount <> "" Then 'COMPUTE THE Net Price trade discount by mobiz                              
                            resultTDNetPrice = CDec2(listprice) * ComputePriceNetOfTradeDiscount(tradediscount)
                            povendorprice = resultTDNetPrice
                        ElseIf tradediscountnetprice = "" And tradediscount = "" Then
                            povendorprice = CDec2(listprice)
                        ElseIf tradediscount <> "" And tradediscountnetprice <> "" Then
                            resultTDNetPrice = CDec2(tradediscountnetprice)
                            povendorprice = resultTDNetPrice
                        End If

                        p = New InsertUpdate("LibraryPriceCanvassVendorPrice")
                        p("CanvassVendorID") = ReadyString(canvassvendorid)
                        p("CanvassItemSkuID") = ReadyString(canvassitemid)
                        p("Price") = IIf(listprice = "", "NULL", ReadyString(listprice))
                        p("TradeDiscountLess") = IIf(tradediscount = "", "NULL", ReadyString(tradediscount))
                        p("TradeDiscountRemarks") = ReadyString(tradediscountlabel)
                        p("PriceNetOfTradeDiscount") = resultTDNetPrice
                        p("VendorComment") = ReadyString(vendorproductcomment)

                        pSql = "SELECT ID FROM LibraryPriceCanvassVendorPrice " & vbCrLf &
                                " WHERE CanvassVendorID = " & ReadyString(canvassvendorid) & vbCrLf &
                                " AND CanvassItemSkuID = " & ReadyString(canvassitemid)
                        row = mBusiness.ProData.Execute(pSql).Rows

                        If row.Count = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                        Else
                            pSql = p.GetUpdate(" ID = " & ReadyString(row(0)("ID").ToString()))
                        End If

                        mBusiness.ProData.Execute(pSql)
                        pc.UpdateLibraryVendorPrices(vendorid, productID, povendorprice, 1)
                    End If
                Next

                isUploading = isOkay

                If isOkay Then
                    strExcelFileProgress = JsonDictionary.Serialize("Success", "Success")
                    mBusiness.ProData.CommitTransaction()
                End If
            Catch ex As Exception
                isOkay = False
                err = ex.Message
                AppendToLog(ex.Message)
            End Try

            Return isOkay
        End Function

        <HttpPost()>
        Public Function UploadRequestFollowUp()
            If isUploading Then
                Return strExcelFileProgress
            Else
                Return Json(New With {Key .data = GetSpecificCanvassData(canvassid, mBusiness.ProData)})
            End If
        End Function

        Function ComputePriceNetOfTradeDiscount(ByVal tradediscount As String) As Decimal
            Dim arrTD As Object = Split(tradediscount, ",")
            Dim resultTDNetPrice As Decimal = 0

            For Each dis As Object In arrTD
                If resultTDNetPrice = 0 Then
                    resultTDNetPrice = 1 - (CDec2(dis) / 100)
                Else
                    resultTDNetPrice = resultTDNetPrice * (1 - (CDec2(dis) / 100))
                End If
            Next

            Return resultTDNetPrice
        End Function

        Function GetSpecificCanvassData(ByVal canvassid As String, ByVal mProdata As ProData) As Object
            Dim pSql As String = "SELECT cvs.*, " & vbCrLf &
                            "ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as ModifiedBy " & vbCrLf &
                            "FROM LibraryPriceCanvass as cvs " & vbCrLf &
                            "INNER JOIN MobizLibraryPeople as ppol " & vbCrLf &
                            "ON ppol.ID = CreatedBy " & vbCrLf &
                            "WHERE cvs.ID = " & ReadyString(canvassid) & vbCrLf
            Dim mycanvass As Object
            GetCanvassData(mycanvass, mProdata.Execute(pSql).Rows(0), mProdata)
            Return mycanvass
        End Function

        Function RemoveCanvass() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()
                Dim cID As String = Common.WebRequest.Data("CanvassID")
                Dim pDt As DataTable = Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryPriceCanvassItems WHERE CanvassID = " & ReadyString(cID))

                For i As Integer = 0 To pDt.Rows.Count - 1
                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryPriceCanvassItemSku WHERE CanvassItemID = " & ReadyString(pDt.Rows(i)("ID")))
                Next

                pDt = Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryPriceCanvassVendor WHERE CanvassID = " & ReadyString(cID))

                For i As Integer = 0 To pDt.Rows.Count - 1
                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryPriceCanvassVendorPrice WHERE CanvassVendorID = " & ReadyString(pDt.Rows(i)("ID")))
                Next

                Common.WebRequest.ProData.Execute("DELETE FROM LibraryPriceCanvassItems WHERE CanvassID = " & ReadyString(cID))
                Common.WebRequest.ProData.Execute("DELETE FROM LibraryPriceCanvassVendor WHERE CanvassID = " & ReadyString(cID))
                Common.WebRequest.ProData.Execute("DELETE FROM LibraryPriceCanvass WHERE ID = " & ReadyString(cID))

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Results", "deleted")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetPriceCanvass()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
            Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
            Dim mainquery As String = "SELECT cvs.*, ROW_NUMBER() OVER (ORDER BY cvs.LastUpdated DESC) AS RowNum, " & vbCrLf &
                                "ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as ModifiedBy, " & vbCrLf &
                                " (SELECT COUNT(ID) FROM LibraryPriceCanvassVendor WHERE CanvassID = cvs.ID) as Vendor " & vbCrLf &
                                "FROM LibraryPriceCanvass as cvs " & vbCrLf &
                                "INNER JOIN MobizLibraryPeople as ppol " & vbCrLf &
                                "ON ppol.ID = CreatedBy " & vbCrLf &
                                "WHERE ISNULL(cvs.Type, 'vendor') = " & ReadyString(CStr2(Common.WebRequest.QueryString("type"))) & vbCrLf &
                                " AND ISNULL(cvs.IsActive, 1) = " & Common.WebRequest.QueryString("IsActive") & vbCrLf &
                                IIf(CStr2(Common.WebRequest.QueryString("keywords")) = "", "", " AND cvs.CanvassName LIKE " & ReadyStringWild(CStr2(Common.WebRequest.QueryString("keywords"))))

            Dim pSql As String = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
            Dim pDTPriceCanvass As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim pDTCountPriceCanvass As DataTable = Common.WebRequest.ProData.Execute(mainquery)
            'Dim arrCanvass(pDTPriceCanvass.Rows.Count - 1) As Object
            'Dim cls As New CRMDashboard

            'If pDTPriceCanvass.Rows.Count > 0 Then
            '    For i As Integer = 0 To pDTPriceCanvass.Rows.Count - 1
            '        Dim errmsg As String = ""
            '        GetCanvassData(arrCanvass(i), pDTPriceCanvass.Rows(i), Common.WebRequest.ProData, errmsg)

            '        If errmsg <> "" Then
            '            Return Json(New With {Key .data = errmsg})
            '            Exit For
            '        End If
            '    Next
            'End If
            Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDTPriceCanvass)), .recordsTotal = pDTCountPriceCanvass.Rows().Count, .recordsFiltered = pDTCountPriceCanvass.Rows().Count}, JsonRequestBehavior.AllowGet)
        End Function

        Function GetCanvassData(ByRef arrCanvass As Object, ByVal data As Object, ByVal mProdata As ProData, Optional ByRef errmsg As String = "") As String
            Try
                Dim pDTItems As DataTable = mProdata.Execute("SELECT item.*, lbl.Label as UomName, cat.C0020 as Category, pim.Brand, ISNULL(NULLIF(LTRIM(RTRIM(pim.Brand)), '') + ' ','') + pim.ProductName as ProductName FROM LibraryPriceCanvassItems as item " & vbCrLf &
                                                            " LEFT JOIN MobizLibraryPIMMeasure msr On item.UnitOfMeasure = msr.ID " & vbCrLf &
                                                            " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                                                            " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = item.SkuID " & vbCrLf &
                                                            " LEFT JOIN MobizLibraryPIMProduct as pim ON pim.ID = item.SkuID " & vbCrLf &
                                                            " WHERE CanvassID = " & ReadyString(data("ID").ToString()) & vbCrLf &
                                                            " ORDER BY item.RowNumber ASC ")
                Dim arrItems(pDTItems.Rows.Count - 1) As Object
                Dim arrSku As Object
                For m As Integer = 0 To pDTItems.Rows.Count - 1
                    'Dim pDTSku As DataTable = Common.WebRequest.ProData.Execute("SELECT itemsku.CanvassItemID, pim.ProductName, pim.ID, itemsku.ID as DBID, 'Edit' as State FROM LibraryPriceCanvassItemSku as itemsku " & vbCrLf &
                    '                                                            " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = itemsku.SkuID " & vbCrLf &
                    '                                                          "WHERE CanvassItemID = " & ReadyString(pDTItems.Rows(m)("ID").ToString()))
                    'arrSku = jss.Deserialize(Of Object)(cls.GetJson(pDTSku))

                    arrItems(m) = New With {Key .DBID = pDTItems.Rows(m)("ID").ToString(),
                                                .ID = pDTItems.Rows(m)("SkuID"),
                                                .Description = pDTItems.Rows(m)("Description"),
                                                .ProductName = pDTItems.Rows(m)("ProductName"),
                                                .Category = pDTItems.Rows(m)("Category"),
                                                .ProductComment = pDTItems.Rows(m)("ProductComment"),
                                                .Brand = pDTItems.Rows(m)("Brand"),
                                                .Qty = pDTItems.Rows(m)("Qty"),
                                                .Uom = pDTItems.Rows(m)("UnitOfMeasure").ToString(),
                                                .UomName = pDTItems.Rows(m)("UomName"),
                                                .Frequency = pDTItems.Rows(m)("Frequency"),
                                                .IsOtherFrequency = pDTItems.Rows(m)("IsOtherFrequency"),
                                                .RowNumber = pDTItems.Rows(m)("RowNumber"),
                                                .State = "Edit"}
                Next

                Dim pDTVendor As DataTable = mProdata.Execute("SELECT vendor.ID as DBID, vendor.CanvassVendorCode, " & vbCrLf &
                                                            " CASE WHEN vendor.EndDate = NULL THEN '' ELSE convert(varchar, vendor.EndDate, 107) END as EndDate, " & vbCrLf &
                                                            " vendor.IsActiveLink, 'Edit' as State, org.ID, org.IsOrganization, " & vbCrLf &
                                                            " org.CompanyName, org.PersonFirstName as FirstName, org.PersonMiddleName as MiddleName, " & vbCrLf &
                                                            " org.PersonSurname as Surname " & vbCrLf &
                                                            " FROM LibraryPriceCanvassVendor as vendor" & vbCrLf &
                                                            " INNER JOIN MobizLibraryOrganization as org ON org.ID = vendor.VendorID " & vbCrLf &
                                                            " WHERE CanvassID = " & ReadyString(data("ID").ToString()) & vbCrLf &
                                                            IIf(CStr2(Common.WebRequest.Data("VendorID")) = "", "", " AND vendor.VendorID = " & ReadyString(Common.WebRequest.Data("VendorID"))) & vbCrLf &
                                                            IIf(CStr2(Common.WebRequest.Data("vcode")) = "", "", " AND vendor.CanvassVendorCode = " & ReadyString(Common.WebRequest.Data("vcode"))) & " -- V as vendorcode" & vbCrLf &
                                                            " ORDER BY org.CompanyName ASC ")
                Dim arrVendor(pDTVendor.Rows.Count - 1) As Object

                If pDTVendor.Rows.Count > 0 Then
                    For v As Integer = 0 To pDTVendor.Rows.Count - 1
                        Try
                            Dim pDTVendorPrice As DataTable = mProdata.Execute("SELECT vp.CanvassItemSkuID as ItemDBID, vp.Price, vp.TradeDiscountLess, vp.TradeDiscountRemarks, vp.PriceNetOfTradeDiscount, vp.ID as VendorPriceID, 'Edit' as State, " & vbCrLf &
                                                                            " vp.VolumeDiscount, vp.VendorComment, vp.IsLowestNetTradeDiscount, vp.IsLowestNetTradeAndVolumeDiscount, vp.IsLowestNetTradeVolumeAndCashDiscount, vp.LowestPrice, vp.PriceNetOfTradeAndVolumeDiscount, vp.PriceNetOfTradeVolumeAndCashDiscount, vp.CashDiscountIfPaidIn, vp.CashDiscountRate, vp.CashDiscountNetAmountToBePaidIn, ISNULL(convert(varchar(MAX), item.SkuID), '') as SkuID " & vbCrLf &
                                                                            " FROM LibraryPriceCanvassVendorPrice as vp " & vbCrLf &
                                                                            " INNER JOIN LibraryPriceCanvassItems as item ON item.ID = vp.CanvassItemSkuID " & vbCrLf &
                                                                            " WHERE vp.CanvassVendorID = " & ReadyString(pDTVendor.Rows(v)("DBID").ToString()))

                            Dim arrSkuPrice As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDTVendorPrice))
                            arrVendor(v) = New With {Key .DBID = pDTVendor.Rows(v)("DBID"),
                                                         .State = pDTVendor.Rows(v)("State"),
                                                         .ID = pDTVendor.Rows(v)("ID"),
                                                         .CompanyName = pDTVendor.Rows(v)("CompanyName"),
                                                         .CanvassVendorCode = pDTVendor.Rows(v)("CanvassVendorCode"),
                                                         .EndDate = pDTVendor.Rows(v)("EndDate"),
                                                         .IsActiveLink = pDTVendor.Rows(v)("IsActiveLink"),
                                                         .FirstName = pDTVendor.Rows(v)("FirstName"),
                                                         .MiddleName = pDTVendor.Rows(v)("MiddleName"),
                                                         .Surname = pDTVendor.Rows(v)("Surname"),
                                                         .data = arrSkuPrice,
                                                         .IsOrganization = pDTVendor.Rows(v)("IsOrganization")
                                                         }
                        Catch ex As Exception
                            errmsg = ex.Message
                            AppendToLog(errmsg)
                            Exit For
                        End Try
                    Next
                End If

                arrCanvass = New With {Key .CanvassID = data("ID").ToString(),
                                        .Name = data("CanvassName"),
                                        .IsSaveAsFinal = IIf(CStr2(data("IsSaveAsFinal")) = "", 0, data("IsSaveAsFinal")),
                                        .No = data("CanvassNumber"),
                                        .CanvassCode = data("PublicCode"),
                                        .NeverExpireLink = IIf(CStr2(data("NeverExpireLink")) = "", True, CBool2(data("NeverExpireLink"))),
                                        .LinkExpirationDate = CStr2(data("LinkExpirationDate")),
                                        .Item = arrItems,
                                        .Vendor = arrVendor,
                                        .ModifiedBy = data("ModifiedBy"),
                                        .LastUpdated = Format(data("LastUpdated"), "MMM dd, yyyy hh:mm:ss tt"),
                                        .IsActive = IIf(CStr2(data("IsActive")) = "", 1, data("IsActive"))}
            Catch ex As Exception
                errmsg = ex.Message
                AppendToLog(ex.Message)
            End Try
        End Function

        Function SavePriceCanvass()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim cID As String = Common.WebRequest.Data("CanvassID")
            Dim cName As String = Common.WebRequest.Data("Name")
            Dim cItems As Object = Common.WebRequest.AllData("Items")
            Dim cVendor As Object = Common.WebRequest.AllData("Vendor")
            Dim cState As String = Common.WebRequest.Data("State")
            Dim cSaveAs As String = Common.WebRequest.Data("SaveAs")
            Dim cPCType As String = Common.WebRequest.Data("Type")
            Dim cStatus As String = Common.WebRequest.Data("IsActive")
            Dim cNeverExpireLink As String = Common.WebRequest.Data("NeverExpireLink")
            Dim cLinkExpirationDate As String = Common.WebRequest.Data("LinkExpirationDate")
            Dim IsCopy As Boolean = IIf(cSaveAs = "copy", True, False)
            Dim pSql As String
            Dim pDTSkus As DataTable
            Dim publicCode As String = GenerateShortCode("canvass", cPCType)

            Try
                Dim p As New InsertUpdate("LibraryPriceCanvass")

                p("IsSaveAsFinal") = IIf(cSaveAs = "final", 1, 0)
                p("CanvassName") = ReadyString(cName & IIf(IsCopy, " - copy", ""))
                p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"
                p("NeverExpireLink") = ReadyString(cNeverExpireLink)
                p("LinkExpirationDate") = ReadyString(cLinkExpirationDate)
                p("Type") = ReadyString(cPCType)
                p("IsActive") = cStatus

                If cState = "New" Or IsCopy Then
                    cID = Guid.NewGuid.ToString()
                    p("CanvassNumber") = ReadyString(GenerateNumber())
                    p("PublicCode") = ReadyString(publicCode)
                    p("ID") = ReadyString(cID)
                    pSql = p.GetInsert()
                Else
                    pSql = "SELECT ISNULL(PublicCode, '') as PublicCode FROM LibraryPriceCanvass WHERE ID = " & ReadyString(cID)
                    publicCode = CStr2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("PublicCode"))
                    If publicCode = "" Then
                        publicCode = GenerateShortCode("canvass", cPCType)
                        p("PublicCode") = ReadyString(publicCode)
                    End If

                    pSql = p.GetUpdate("ID = " & ReadyString(cID))
                End If
                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("MobizLibraryPriceCanvassShareableLink")

                p("OrgID") = ReadyString(WebRequest.Instance.OrgID)
                p("PriceCanvassID") = ReadyString(cID)
                p("PublicCode") = ReadyString(publicCode)

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PriceCanvassID = " & ReadyString(cID))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                For Each item As Object In cItems
                    p = New InsertUpdate("LibraryPriceCanvassItems")
                    Dim itemID = ReadyString(item("DBID"))
                    p("CanvassID") = ReadyString(cID)
                    p("RowNumber") = ReadyString(item("RowNumber"))
                    p("Description") = ReadyString(item("Description"))
                    p("Qty") = IIf(item("Qty") = "", "NULL", CDec2(item("Qty")))
                    p("UnitOfMeasure") = IIf(item("Uom") = "", "NULL", ReadyString(item("Uom")))
                    p("SkuID") = IIf(item("SkuID") = "", "NULL", ReadyString(item("SkuID")))
                    p("Frequency") = ReadyString(item("Frequency"))
                    p("IsOtherFrequency") = ReadyString(item("IsOtherFrequency"))

                    If item("State") = "New" Or IsCopy Then
                        'itemID = ReadyString(Guid.NewGuid.ToString())
                        p("ID") = itemID
                        pSql = p.GetInsert()
                    ElseIf item("State") = "Edit" Then
                        pSql = p.GetUpdate("ID = " & itemID)
                    Else
                        pSql = "DELETE FROM LibraryPriceCanvassItems WHERE ID = " & itemID
                    End If

                    Common.WebRequest.ProData.Execute(pSql)

                    If item("State") = "Remove" Then
                        pSql = "DELETE FROM LibraryPriceCanvassVendorPrice WHERE CanvassItemSkuID = " & itemID
                        Common.WebRequest.ProData.Execute(pSql)

                        pSql = "DELETE FROM LibraryPriceCanvassItemSku WHERE CanvassItemID = " & itemID
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next item

                For Each vendor As Object In cVendor
                    Dim vID As String = Guid.NewGuid.ToString()
                    Dim vendorID As String = vendor("ID")

                    If vendor.ContainsKey("IsFromPrivateVendor") Then
                        If vendor("IsFromPrivateVendor") = False Then
                            pSql = "SELECT ID, CompanyName, TinNumber FROM MobizLibraryOrganization WHERE ID = " & ReadyString(vendor("ID").ToString())
                            Dim details As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                            p = New InsertUpdate("MobizLibraryOrganization")
                            vendorID = Guid.NewGuid.ToString()

                            p("ID") = ReadyString(vendorID)
                            p("CompanyName") = ReadyString(details("CompanyName"))
                            p("TinNumber") = ReadyString(CStr2(details("TinNumber")))
                            p("OriginalKeyID") = ReadyString(vendor("ID"))
                            p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))

                            Common.WebRequest.ProData.Execute(p.GetInsert())

                            p = New InsertUpdate("LibraryOrganizationSupplier")

                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("SupplierID") = ReadyString(vendorID)
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    End If

                    If cPCType = "competitor" Then
                        pSql = "SELECT ID FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(vendorID)

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                            p = New InsertUpdate("LibraryOrganizationSupplier")

                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("SupplierID") = ReadyString(vendorID)
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    End If

                    p = New InsertUpdate("LibraryPriceCanvassVendor")

                    p("CanvassID") = ReadyString(cID)
                    p("VendorID") = ReadyString(vendorID)

                    If vendor.ContainsKey("IsActiveLink") Then
                        p("IsActiveLink") = CInt2(vendor("IsActiveLink"))
                    Else
                        p("IsActiveLink") = 0
                    End If

                    If vendor.ContainsKey("EndDate") Then
                        If CStr2(vendor("EndDate")) <> "" Then
                            p("EndDate") = ReadyString(CStr2(vendor("EndDate")))

                            If Date.Compare(Date.Now().ToString("MMM dd, yyyy"), CStr2(vendor("EndDate"))) = 0 Then
                                p("IsActiveLink") = 0
                            End If
                        End If
                    End If

                    If vendor.ContainsKey("CanvassVendorCode") Then
                        If CStr2(vendor("CanvassVendorCode")) = "" Then
                            p("CanvassVendorCode") = ReadyString(GenerateShortCode("vendor", cPCType))
                        End If
                    Else
                        p("CanvassVendorCode") = ReadyString(GenerateShortCode("vendor", cPCType))
                    End If

                    If vendor("State") = "New" Or IsCopy Then
                        p("ID") = ReadyString(vID)
                        pSql = p.GetInsert()
                    ElseIf vendor("State") = "Edit" Then
                        vID = vendor("DBID")
                        pSql = p.GetUpdate("ID = " & ReadyString(vID))
                    ElseIf vendor("State") = "Remove" Then
                        vID = vendor("DBID")
                        pSql = "DELETE FROM LibraryPriceCanvassVendor WHERE ID = " & ReadyString(vID) & vbCrLf
                    End If
                    Common.WebRequest.ProData.Execute(pSql)

                    If vendor("State") <> "Remove" Then
                        For Each price As Object In vendor("data")
                            Dim errmsg As String = ""
                            If SaveVendorPrice(price, vID, vendorID, errmsg) = False Then
                                Common.WebRequest.ProData.RollbackTransaction()
                                Return JsonDictionary.Serialize("Results", errmsg)
                            End If
                        Next price
                    Else
                        pSql = "DELETE FROM LibraryPriceCanvassVendorPrice WHERE CanvassVendorID = " & ReadyString(vID) & vbCrLf
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                Next vendor

                Common.WebRequest.ProData.CommitTransaction()
                Return Json(New With {Key .data = GetSpecificCanvassData(cID, Common.WebRequest.ProData)})
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveVendorPrice(ByVal price As Object, ByVal cvid As String, ByVal vendorid As String, ByRef errmsg As String)
            Try
                Dim poprice As String

                Dim pSql = "SELECT ISNULL(convert(varchar(MAX), SkuID), '') as SkuID, ISNULL(convert(varchar(MAX), UnitOfMeasure), '') as UnitOfMeasure FROM LibraryPriceCanvassItems WHERE ID = " & ReadyString(price("ItemDBID"))
                Dim dt As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim linkedskuid As String = ""
                Dim pUnitQty As Integer = 1

                If dt.Rows.Count > 0 Then
                    linkedskuid = dt.Rows(0)("SkuID")
                    If String.IsNullOrWhiteSpace(linkedskuid) = False And String.IsNullOrWhiteSpace(dt.Rows(0)("UnitOfMeasure")) = False Then
                        Dim pDrUnit = WebRequest.Instance.Business.LibraryStock.GetUnit(linkedskuid, CStr2(dt.Rows(0)("UnitOfMeasure")))
                        If pDrUnit IsNot Nothing Then
                            pUnitQty = pDrUnit("Quantity")
                        End If
                    End If
                End If

                Dim p = New InsertUpdate("LibraryPriceCanvassVendorPrice")
                p("CanvassVendorID") = ReadyString(cvid)
                p("CanvassItemSkuID") = ReadyString(price("ItemDBID"))

                If price.ContainsKey("VendorComment") Then
                    p("VendorComment") = ReadyString(price("VendorComment"))
                End If

                If price.ContainsKey("TradeDiscountLess") Then
                    p("TradeDiscountLess") = ReadyString(price("TradeDiscountLess"))
                End If

                If price.ContainsKey("TradeDiscountRemarks") Then
                    p("TradeDiscountRemarks") = ReadyString(price("TradeDiscountRemarks"))
                End If

                If price.ContainsKey("PriceNetOfTradeDiscount") Then
                    If CDec2(price("PriceNetOfTradeDiscount")) <> 0 Then
                        poprice = CDec2(price("PriceNetOfTradeDiscount"))
                    End If
                    p("PriceNetOfTradeDiscount") = IIf(CDec2(price("PriceNetOfTradeDiscount")) = 0, "NULL", CDec2(price("PriceNetOfTradeDiscount")))
                End If

                If price.ContainsKey("Price") Then
                    If String.IsNullOrWhiteSpace(poprice) Then
                        poprice = CDec2(price("Price"))
                    End If
                    p("Price") = IIf(CStr2(price("Price")) = "", "NULL", CDec2(price("Price"))) 'THIS IS SRP
                End If

                If price.ContainsKey("CashDiscountRate") Then
                    p("CashDiscountRate") = IIf(CStr2(price("CashDiscountRate")) = "", "NULL", ReadyString(price("CashDiscountRate")))
                    p("CashDiscountIfPaidIn") = IIf(CStr2(price("CashDiscountIfPaidIn")) = "", "NULL", ReadyString(price("CashDiscountIfPaidIn")))
                    p("CashDiscountNetAmountToBePaidIn") = IIf(CStr2(price("CashDiscountNetAmountToBePaidIn")) = "", "NULL", ReadyString(price("CashDiscountNetAmountToBePaidIn")))
                End If

                If price.ContainsKey("PriceNetOfTradeAndVolumeDiscount") Then
                    p("PriceNetOfTradeAndVolumeDiscount") = IIf(CStr2(price("PriceNetOfTradeAndVolumeDiscount")) = "", "NULL", ReadyString(price("PriceNetOfTradeAndVolumeDiscount")))
                End If

                If price.ContainsKey("PriceNetOfTradeVolumeAndCashDiscount") Then
                    p("PriceNetOfTradeVolumeAndCashDiscount") = IIf(CStr2(price("PriceNetOfTradeVolumeAndCashDiscount")) = "", "NULL", ReadyString(price("PriceNetOfTradeVolumeAndCashDiscount")))
                End If

                If price.ContainsKey("VolumeDiscount") Then
                    p("VolumeDiscount") = IIf(CStr2(price("VolumeDiscount")) = "", "NULL", ReadyString(price("VolumeDiscount")))
                End If

                If price.ContainsKey("LowestPrice") Then
                    p("LowestPrice") = IIf(CStr2(price("LowestPrice")) = "", "NULL", ReadyString(price("LowestPrice")))
                End If

                If price.ContainsKey("IsLowestNetTradeDiscount") Then
                    If CBool2(price("IsLowestNetTradeDiscount")) Then
                        If price.ContainsKey("IsNewLowestNetTradeDiscount") Then
                            p("IsLowestNetTradeDiscount") = 1
                        Else
                            p("IsLowestNetTradeDiscount") = 0
                        End If
                    Else
                        p("IsLowestNetTradeDiscount") = CInt2(price("IsLowestNetTradeDiscount"))
                    End If
                Else
                    p("IsLowestNetTradeDiscount") = 0
                End If

                If price.ContainsKey("IsLowestNetTradeAndVolumeDiscount") Then
                    If CBool2(price("IsLowestNetTradeAndVolumeDiscount")) Then
                        If price.ContainsKey("IsNewLowestNetTradeAndVolumeDiscount") Then
                            p("IsLowestNetTradeAndVolumeDiscount") = 1
                        Else
                            p("IsLowestNetTradeAndVolumeDiscount") = 0
                        End If
                    Else
                        p("IsLowestNetTradeAndVolumeDiscount") = CInt2(price("IsLowestNetTradeAndVolumeDiscount"))
                    End If
                Else
                    p("IsLowestNetTradeAndVolumeDiscount") = 0
                End If

                If price.ContainsKey("IsLowestNetTradeVolumeAndCashDiscount") Then
                    If CBool2(price("IsLowestNetTradeVolumeAndCashDiscount")) Then
                        If price.ContainsKey("IsNewLowestNetTradeVolumeAndCashDiscount") Then
                            p("IsLowestNetTradeVolumeAndCashDiscount") = 1
                        Else
                            p("IsLowestNetTradeVolumeAndCashDiscount") = 0
                        End If
                    Else
                        p("IsLowestNetTradeVolumeAndCashDiscount") = CInt2(price("IsLowestNetTradeVolumeAndCashDiscount"))
                    End If
                Else
                    p("IsLowestNetTradeVolumeAndCashDiscount") = 0
                End If

                If price.ContainsKey("VendorPriceID") Then
                    If price("VendorPriceID") = "" Then 'for save a copy
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate("ID = " & ReadyString(price("VendorPriceID")))
                    End If
                Else
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                End If

                'If price("State") = "New" Then
                '    p("ID") = ReadyString(Guid.NewGuid.ToString())
                '    pSql = p.GetInsert()
                'ElseIf price("State") = "Edit" Then

                'Else
                '    pSql = "DELETE FROM LibraryPriceCanvassVendorPrice WHERE ID = " & ReadyString(price("VendorPriceID"))
                'End If

                Common.WebRequest.ProData.Execute(pSql)
                If String.IsNullOrWhiteSpace(poprice) = False And linkedskuid <> "" Then
                    WebRequest.Instance.Business.LibraryPriceCanvass.UpdateLibraryVendorPrices(vendorid, linkedskuid, poprice, pUnitQty)
                End If

                Return True
            Catch ex As Exception
                errmsg = ex.Message
                Return False
            End Try
        End Function

        Function GenerateNumber() As Integer
            Dim pSql As String = "SELECT MAX(CanvassNumber) as CanvassNumber FROM LibraryPriceCanvass"
            Dim pDt As DataTable = Common.WebRequest.ProData.Execute(pSql)

            If pDt.Rows.Count > 0 Then
                Return CInt2(pDt.Rows(0)("CanvassNumber")) + 1
            Else
                Return 1
            End If
        End Function

        Function DownloadCanvassVendor()
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            Try
                Dim canvassid As String = Request.Params.Get("cid")
                Dim dbvendorid As String = Request.Params.Get("dbvid")
                Dim vendorid As String = Request.Params.Get("vid")
                Dim isdisplayprices As String = Request.Params.Get("displayprices")
                Dim pSql As String
                Dim clsOrganization As New Organization
                Dim pDT As DataTable
                Dim pFS As IO.FileStream
                Dim pWB As Workbook
                Dim pWS As Worksheet
                Dim intCntr As String
                Dim intRowCntr As Integer
                Dim cvfiles As New ArrayList()
                Dim filename As String
                Dim noOfVendors As Integer = 0
                Dim companyname As String

                pSql = "SELECT CanvassName FROM LibraryPriceCanvass WHERE ID = " & ReadyString(canvassid)
                Dim canvassname As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("CanvassName") & " " & DateTime.Now.ToString("MM-dd-yyyy")

                pSql = "SELECT * FROM LibraryPriceCanvassVendor WHERE CanvassID = " & ReadyString(canvassid) & IIf(vendorid = Nothing, "", " AND VendorID = " & ReadyString(vendorid))
                pDT = Common.WebRequest.ProData.Execute(pSql)

                For Each cv As Object In pDT.Rows
                    If pFS IsNot Nothing Then pFS.Dispose()
                    Dim fileDetail As IO.FileInfo = My.Computer.FileSystem.GetFileInfo(Common.ServerPath & "/FileTemplates/Canvass.xlsx")
                    fileDetail.IsReadOnly = False
                    pWB = New Workbook
                    pFS = New IO.FileStream(Common.ServerPath & "FileTemplates\Canvass.xlsx", IO.FileMode.Open)
                    pWB.Open(pFS)
                    pWS = pWB.Worksheets(0)

                    '''get List of Product
                    clsOrganization.MaxReturnRow = 9999999
                    Const pRowsCount As Integer = 1000
                    intRowCntr = 1

                    pSql = "SELECT org.CompanyName, ISNULL(VendorCode, '') as VendorCode, ISNULL(ThirdPartyID, '') as ThirdPartyID " & vbCrLf &
                            " FROM LibraryOrganizationSupplier as sup " & vbCrLf &
                            " INNER JOIN MobizLibraryOrganization as org ON org.ID = sup.SupplierID " & vbCrLf &
                            " WHERE SupplierID = " & ReadyString(cv("VendorID"))
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    noOfVendors += 1
                    companyname = pDT.Rows(0)("CompanyName")
                    Dim vendorcode As String = pDT.Rows(0)("VendorCode")
                    Dim thirdpartyid As String = pDT.Rows(0)("ThirdPartyID")

                    If vendorcode = "" Then 'auto create vendor code if empty
                        vendorcode = New OrganizationShort().GenerateCodeNumber(False)
                        Dim p As New InsertUpdate("LibraryOrganizationSupplier")

                        p("VendorCode") = ReadyString(vendorcode)
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" SupplierID = " & ReadyString(cv("VendorID"))))
                    End If

                    pSql = "SELECT pim.ProductNo, item.Description, pim.Brand, cat.C0020 as Category, lbl.Label as UomName, price.Price, price.TradeDiscountLess, price.TradeDiscountRemarks, price.PriceNetOfTradeDiscount " & vbCrLf &
                                "  " & vbCrLf &
                                " FROM LibraryPriceCanvassItems as item " & vbCrLf &
                                " LEFT JOIN MobizLibraryPIMProduct as pim ON pim.ID = item.SkuID " & vbCrLf &
                                " LEFT JOIN LibraryPriceCanvassVendorPrice as price " & vbCrLf &
                                "   ON item.ID = price.CanvassItemSkuID AND price.CanvassVendorID = " & ReadyString(cv("ID")) & vbCrLf &
                                " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = item.SkuID " & vbCrLf &
                                " LEFT JOIN MobizLibraryPIMMeasure msr On item.UnitOfMeasure = msr.ID " & vbCrLf &
                                " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                                " WHERE item.CanvassID = " & ReadyString(canvassid) & vbCrLf &
                                " ORDER BY item.RowNumber ASC "

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    For Each cvdata As Object In pDT.Rows
                        Dim objTest As New ArrayList()

                        objTest.Add(cvdata("ProductNo")) 'Product No
                        objTest.Add(cvdata("Description")) 'Product Description
                        objTest.Add(cvdata("Brand")) 'Brand
                        objTest.Add(cvdata("Category")) 'Category
                        objTest.Add("") 'Vendor Product Comment
                        objTest.Add(cvdata("UomName")) 'Unit Measure

                        If CBool2(isdisplayprices) Then
                            objTest.Add(cvdata("Price")) 'List Price
                        Else
                            objTest.Add("") 'List Price
                        End If

                        objTest.Add("") 'Trade Discount
                        objTest.Add("") 'Trade Discount Label // remove this
                        objTest.Add("") 'Net Price
                        objTest.Add(vendorcode) 'Mobiz Vendor Code
                        objTest.Add(companyname) 'Vendor Name
                        objTest.Add(thirdpartyid) '3rd Party ID

                        pWS.Cells.ImportArrayList(objTest, intRowCntr, 0, False)
                        intRowCntr = intRowCntr + 1
                    Next cvdata

                    If Not System.IO.Directory.Exists(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID) Then
                        System.IO.Directory.CreateDirectory(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID)
                    End If

                    filename = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\" & Regex.Replace(companyname, "[^A-Za-z0-9\-/]", "") & "_" & vendorcode & ".xls"
                    pWB.Save(filename)
                    cvfiles.Add(filename)

                    pFS.Close()
                Next cv

                If noOfVendors > 1 Then
                    Dim zip As New ProZip

                    For Each files As Object In cvfiles
                        zip.AddFile(files, canvassname)
                        If (IO.File.Exists(files)) Then
                            IO.File.Delete(files)
                        End If
                    Next files

                    Dim zipfile As String = Server.MapPath("~/FileTemplates/" & WebRequest.Instance.OrgID & "/" & canvassname & ".zip")
                    Dim file As System.IO.FileInfo = New System.IO.FileInfo(zipfile)

                    Response.Clear()
                    Response.ContentType = "application/octet-stream"
                    Response.AddHeader("content-disposition", "attachment; filename=" & canvassname & ".zip")
                    zip.Save(zipfile)
                    Response.WriteFile(file.FullName)
                    Response.End()
                Else
                    Return File(filename, "application/xls", companyname & ".xls")
                End If

                'Try
                '    If pFS IsNot Nothing Then pFS.Dispose()
                'Catch ex As Exception
                'End Try

                '''Download the File
                ''
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return "Something went wrong. Please contact the system provider. Error Msg: " & ex.Message
            End Try
        End Function

        Public Function GenerateShortCode(ByVal type As String, ByVal PCType As String) As String
            Dim rand As New Random()
            Dim allowableChars() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
            Dim link As String

            For i As Integer = 0 To 10 - 1
                link += allowableChars(rand.Next(allowableChars.Length - 1))
            Next

            'CHECK IF GENERATED LINK EXIST
            Dim pSql As String

            If type = "canvass" Then
                pSql = "SELECT PublicCode FROM LibraryPriceCanvass WHERE PublicCode = " & ReadyString(link) & " AND ISNULL(Type, 'vendor') = " & ReadyString(PCType)
            Else
                pSql = "SELECT CanvassVendorCode FROM LibraryPriceCanvassVendor WHERE CanvassVendorCode = " & ReadyString(link)
            End If

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                link = GenerateShortCode(type, PCType) 'replace the old link with the new generated code
            End If

            Return link
        End Function

        Function DownloadPIMByCategories()
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            Dim pSql As String
            Dim arrcategories As Object = jss.Deserialize(Of Object)(Request.Params.Get("categories"))
            Dim categories As String = ""
            Dim pFS As IO.FileStream
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim intCntr As Integer = 0
            Dim intRowCntr As Integer = 1
            Dim pDT As DataTable
            Dim path As String = Common.ServerPath & "\FileTemplates\Canvass.xlsx"
            Dim fileDetail As IO.FileInfo = My.Computer.FileSystem.GetFileInfo(Common.ServerPath & "/FileTemplates/Canvass.xlsx")
            fileDetail.IsReadOnly = False

            For Each cat As Object In arrcategories
                If categories = "" Then
                    categories = ReadyString(cat("Name"))
                Else
                    categories += ", " & ReadyString(cat("Name"))
                End If
            Next cat

            pFS = New IO.FileStream(path, IO.FileMode.Open)
            pWB = New Workbook
            pWB.Open(pFS)
            pWS = pWB.Worksheets(0)

            Do
                pSql = "SELECT pim.ProductNo, pim.ProductName, pim.Brand, cat.C0020 as Category " & vbCrLf &
                        " FROM LibraryOrganizationPIM as orgpim " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = orgpim.ProductID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = pim.ID " & vbCrLf &
                        " WHERE orgpim.IsDeactivated = 0 AND cat.C0020 IN (" & categories & ")" & vbCrLf &
                        " ORDER BY cat.C0020, pim.ProductName ASC "

                pDT = Common.WebRequest.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    intCntr = 0
                    While (intCntr < pDT.Rows.Count)

                        Dim objTest As New ArrayList()

                        objTest.Add(pDT.Rows(intCntr).ItemArray(0)) 'ProductNumber
                        objTest.Add(pDT.Rows(intCntr).ItemArray(1)) 'ProductName
                        objTest.Add(pDT.Rows(intCntr).ItemArray(2)) 'Brand
                        objTest.Add(pDT.Rows(intCntr).ItemArray(3)) 'Category
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(4))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(5))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(6))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(7))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(8))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(9))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(10))
                        'objTest.Add(pDT.Rows(intCntr).ItemArray(11))

                        pWS.Cells.ImportArrayList(objTest, intRowCntr, 0, False)

                        intCntr = intCntr + 1
                        intRowCntr = intRowCntr + 1

                    End While
                End If

                If pDT.Rows.Count = intCntr Then
                    Exit Do
                End If
            Loop

            If Not System.IO.Directory.Exists(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID) Then
                System.IO.Directory.CreateDirectory(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID)
            End If

            Dim filepath As String = Common.ServerPath & "\FileTemplates\" & WebRequest.Instance.OrgID & "\pimbycategories_" & CStr2(WebRequest.Instance.OrgID) & ".xls"
            pWB.Save(filepath)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try

            '''Download the File

            Return File(filepath, "application/xls", "CanvassTemplate.xls")
        End Function

        Function GetSupplierPriceCanvass()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim vendorID As String = Common.WebRequest.Data("VendorID")
                Dim pSql As String = "SELECT pc.* " & vbCrLf &
                                    " FROM LibraryPriceCanvassVendor as ven " & vbCrLf &
                                    " INNER JOIN LibraryPriceCanvass as pc ON pc.ID = ven.CanvassID " & vbCrLf &
                                    " WHERE ven.VendorID = " & ReadyString(vendorID) & vbCrLf &
                                    " AND IsActive = 1 " & vbCrLf &
                                    IIf(CStr2(Common.WebRequest.Data("keywords")) = "", "", " AND pc.CanvassName LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords")))

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function
    End Class
End Namespace