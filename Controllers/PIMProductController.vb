Imports System.IO
Imports Newtonsoft.Json

Namespace WebProj
    Public Class PIMProductController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMProductInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function Brands() As ActionResult
            Return View()
        End Function

        Function Categories() As ActionResult
            Return View()
        End Function

        Function DeactivateDormantSKUSettings() As ActionResult
            Return View()
        End Function

        Function GetDormantSettings() As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut = New JsonDictionary
            Try
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("DormantSKUsPeriod", .DormantSKUsPeriod)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsAutoCleanUpDormantSKUs", .IsAutoCleanUpDormantSKUs)
                End With
            Catch ex As Exception
                pOut.Add("Error", ex.Message)
            End Try

            Return pOut.Serialize
        End Function

        Function SaveDormantSettings()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")

                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    .DormantSKUsPeriod = CDec2(data("DormantPeriod"))
                    .IsAutoCleanUpDormantSKUs = CInt2(data("IsAutoCleanUp"))
                End With

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Results", "success")

            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function ActivateLibraryName()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryPimBrands")

                Common.WebRequest.ProData.BeginTransaction()

                If data("type") = "Product Categories" Then
                    p = New InsertUpdate("LibraryPimCategories")
                Else
                    p = New InsertUpdate("LibraryPimBrands")
                End If

                p("LastUpdated") = "getdate()"
                p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("IsActive") = 1

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DeactivateLibraryName()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryPimBrands")
                Dim columnname, pSql As String

                Common.WebRequest.ProData.BeginTransaction()

                If data("type") = "Product Categories" Then
                    columnname = "Category"

                    If data.ContainsKey("newcategory") Then
                        pSql = " SELECT * FROM (SELECT prd.ID, prd.ProductNo, prd.ProductName, " & vbCrLf &
                            " ISNULL((SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = prd.ID), '') AS Category " & vbCrLf &
                            " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                            " ) as T WHERE ISNULL(Category, '') = " & ReadyString(data("currentname")) & vbCrLf

                        For Each prod As Object In Common.WebRequest.ProData.Execute(pSql).Rows
                            p = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")

                            p("C0020") = ReadyString(data("newcategory"))

                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ProductID = " & ReadyString(prod("ID")))) = 0 Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("ProductID") = ReadyString(prod("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            End If
                        Next prod

                        If data.ContainsKey("currentname") Then
                            If data("currentname") <> "" Then
                                p = New InsertUpdate("LibraryItemModifierCategories")

                                p("CategoryName") = ReadyString(data("newcategory"))

                                Common.WebRequest.ProData.Execute(p.GetUpdate(" CategoryName = " & ReadyString(data("currentname").ToString())))
                            End If
                        End If
                    End If

                    p = New InsertUpdate("LibraryPimCategories")
                Else
                    columnname = "Brand"

                    If data.ContainsKey("newbrand") Then
                        pSql = " SELECT prd.ID, prd.ProductNo, prd.ProductName " & vbCrLf &
                        " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                        " WHERE ISNULL(Brand, '') = " & ReadyString(data("currentname")) & vbCrLf

                        For Each prod As Object In Common.WebRequest.ProData.Execute(pSql).Rows
                            p = New InsertUpdate("MobizLibraryPIMProduct")

                            p(columnname) = ReadyString(data("newbrand"))
                            p("LastUpdated") = "getdate()"

                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(prod("ID").ToString())))
                        Next prod
                    End If

                    p = New InsertUpdate("LibraryPimBrands")
                End If

                p("LastUpdated") = "getdate()"
                p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("IsActive") = 0

                Common.WebRequest.ProData.Execute(p.GetUpdate(columnname & " = " & ReadyString(data("currentname"))))

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetProducts()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")
            Dim pSql As String

            If data("type") = "Brands Library" Then
                pSql = " SELECT prd.ID, prd.ProductNo, prd.ProductName " & vbCrLf &
                    " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                    " WHERE ISNULL(Brand, '') = " & ReadyString(data("currentname")) & vbCrLf
            Else
                pSql = " SELECT * FROM (SELECT prd.ID, prd.ProductNo, prd.ProductName, " & vbCrLf &
                    " ISNULL((SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = prd.ID), '') AS Category " & vbCrLf &
                    " , prd.LastUpdated, ppl.FirstName + ' ' + ppl.Surname as ModifiedBy" & vbCrLf &
                    " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                    " OUTER APPLY( " & vbCrLf &
                    "   SELECT * FROM MobizLibraryPeople WHERE ID = prd.UserID " & vbCrLf &
                    " ) as ppl " & vbCrLf &
                    " ) as T WHERE ISNULL(Category, '') = " & ReadyString(data("currentname")) & vbCrLf
            End If

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function SavePIMLib()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryPimBrands")
                Dim columnname, pSql As String
                Dim pDt As DataTable
                Dim row As Object

                If data("type") = "Product Categories" Then
                    p = New InsertUpdate("LibraryPimCategories")
                    columnname = "Category"

                    If data.ContainsKey("currentname") Then
                        pSql = " SELECT * FROM (SELECT prd.ID, prd.ProductNo, prd.ProductName, " & vbCrLf &
                                " ISNULL((SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = prd.ID), '') AS Category " & vbCrLf &
                                " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                                " ) as T WHERE ISNULL(Category, '') = " & ReadyString(data("currentname")) & vbCrLf
                    End If
                Else
                    p = New InsertUpdate("LibraryPimBrands")
                    columnname = "Brand"

                    If data.ContainsKey("currentname") Then
                        pSql = " SELECT prd.ID, prd.ProductNo, prd.ProductName " & vbCrLf &
                                " FROM LibraryOrganizationPIM orgprd " & vbCrLf &
                                " INNER JOIN MobizLibraryPIMProduct prd ON (orgprd.ProductID = prd.ID) " & vbCrLf &
                                " WHERE ISNULL(Brand, '') = " & ReadyString(data("currentname")) & vbCrLf
                    End If
                End If

                p("LastUpdated") = "getdate()"
                p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("IsActive") = 1

                Common.WebRequest.ProData.BeginTransaction()

                If data("ID") = "" Then
                    ' check if naa nay existing name ang giadd
                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(columnname & " = " & ReadyString(data("name")))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p(columnname) = ReadyString(data("name"))
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Else
                    If data("type") = "Product Categories" Then
                        pDt = Common.WebRequest.ProData.Execute("SELECT * FROM LibraryPimCategories WHERE Category = " & ReadyString(data("currentname")))
                        If pDt.Rows.Count > 0 Then
                            If CBool(pDt.Rows(0)("IsActive")) Then
                                p("IsActive") = 0
                            Else
                                p("IsActive") = 1
                            End If

                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
                        End If

                        p = New InsertUpdate("LibraryPimCategories")

                        p("LastUpdated") = "getdate()"
                        p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("IsActive") = 1

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(columnname & " = " & ReadyString(data("name")))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p(columnname) = ReadyString(data("name"))
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If

                        row = Common.WebRequest.ProData.Execute(pSql).Rows
                        For Each prod As Object In row
                            p = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")

                            p("C0020") = ReadyString(data("name"))

                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ProductID = " & ReadyString(prod("ID")))) = 0 Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("ProductID") = ReadyString(prod("ID").ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            End If
                        Next prod

                        If data.ContainsKey("currentname") Then
                            If data("currentname") <> "" Then
                                p = New InsertUpdate("LibraryItemModifierCategories")

                                p("CategoryName") = ReadyString(data("name"))

                                Common.WebRequest.ProData.Execute(p.GetUpdate(" CategoryName = " & ReadyString(data("currentname").ToString())))
                            End If
                        End If

                        If data.ContainsKey("UpdatedProductCategory") Then
                            For Each prod As Object In data("UpdatedProductCategory")
                                p = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")

                                p("C0020") = ReadyString(prod("NewCategory"))

                                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ProductID = " & ReadyString(prod("ID")))) = 0 Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("ProductID") = ReadyString(prod("ID").ToString())
                                    Common.WebRequest.ProData.Execute(p.GetInsert())
                                End If

                                p = New InsertUpdate("MobizLibraryPIMProduct")
                                p("LastUpdated") = "getdate()"
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(prod("ID"))))
                            Next prod
                        End If
                    Else
                        pDt = Common.WebRequest.ProData.Execute("SELECT * FROM LibraryPimBrands WHERE Brand = " & ReadyString(data("currentname")))
                        If pDt.Rows.Count > 0 Then
                            If CBool(pDt.Rows(0)("IsActive")) Then
                                p("IsActive") = 0
                            Else
                                p("IsActive") = 1
                            End If

                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
                        End If

                        p = New InsertUpdate("LibraryPimBrands")

                        p("LastUpdated") = "getdate()"
                        p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("IsActive") = 1

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(columnname & " = " & ReadyString(data("name")))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p(columnname) = ReadyString(data("name"))
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If

                        row = Common.WebRequest.ProData.Execute(pSql).Rows
                        For Each prod As Object In row
                            p = New InsertUpdate("MobizLibraryPIMProduct")

                            p(columnname) = ReadyString(data("name"))
                            p("LastUpdated") = "getdate()"

                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(prod("ID").ToString())))
                        Next prod
                    End If
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.PIMProductID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMProductID", WebSession.Instance.PIMProductID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckUniqueProduct() As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProduct.ID = Common.WebRequest.Data("PIMProductID")
            clsPIMProduct.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsPIMProduct.ProductNo = Common.WebRequest.Data("ProductNo")
            clsPIMProduct.ProductName = Common.WebRequest.Data("ProductName")
            clsPIMProduct.ProductFamilyID = Common.WebRequest.Data("ProductFamilyID")
            clsPIMProduct.ProductUNSPSCCommID = Common.WebRequest.Data("UNSPSCCommodityID")

            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.CheckUniqueProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductList() As String
            Dim strKeyWord As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductFeatureList() As String
            Dim strID As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("UNSPSCCommID")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductFeatureList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductFeatureValueList() As String
            Dim strID As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMProductID")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductFeatureValueList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductInfo() As String
            Dim strID As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMProductID")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveProductInfo() As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProduct.ID = Common.WebRequest.Data("PIMProductID")
            clsPIMProduct.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsPIMProduct.Brand = Common.WebRequest.Data("ProductNo")
            clsPIMProduct.ProductNo = Common.WebRequest.Data("ProductNo")
            clsPIMProduct.ProductName = Common.WebRequest.Data("ProductName")
            clsPIMProduct.ProductFamilyID = Common.WebRequest.Data("ProductFamilyID")
            clsPIMProduct.ProductUNSPSCCommIDOrig = Common.WebRequest.Data("UNSPSCCommodityIDOrig")
            clsPIMProduct.ProductUNSPSCCommID = Common.WebRequest.Data("UNSPSCCommodityID")
            clsPIMProduct.IsActive = Common.WebRequest.Data("IsActive")
            clsPIMProduct.ProductFeatureList = Common.WebRequest.AllData("ProductFeatureList")

            WebSession.Instance.PIMProductID = clsPIMProduct.SaveProductInfo()
            strReturn = JsonDictionary.Serialize("PIMProductData", WebSession.Instance.PIMProductID)

            Return strReturn

        End Function

        <HttpGet>
        Public Function BomUpload() As ActionResult

            Return View()

        End Function

        <HttpPost>
        Public Function BomUploadFile() As String

            Dim pOrgID As String
            Dim pFilename As String
            Dim pDT As DataTable
            Dim pError As String
            Dim pDV As DataView

            pOrgID = Request.Form("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID.")
            End If
            pOrgID = ReadyGuid(pOrgID)

            If Request.Files.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Please upload a file.")
            End If

            WebRequest.Instance.OrgID = pOrgID
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFilename = Common.ServerPath & "temp\BomUploadFile\" & pOrgID & "\" & DateTimeStamp() & "_" & GenerateString(10).ToUpper() & ".xls"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)

            pDT = ExcelToTable(pFilename, {"Finished Product Number", "Component Product Number", "Quantity", "Unit"}, pError)
            If CStr2(pError).Length Then
                Return JsonDictionary.Serialize("Error", pError)
            End If
            WebRequest.Instance.Business.LibraryStock.ImportFromExcel(pDT, pOrgID)
            pDV = New DataView(pDT, "[Import Result Column] IS NOT NULL", "", DataViewRowState.CurrentRows)
            If pDV.Count > 0 Then
                pError = "An error occured while importing row #" & pDV(0)("Spreadsheet Row Number") & ": " & pDV(0)("Import Result Column")
                Return JsonDictionary.Serialize("Error", pError)
            Else
                Return JsonDictionary.Serialize("Result", "ok")
            End If

        End Function

        <HttpPost>
        Public Function DeactivateAllProducts()

            Dim pAffectedCount As Integer

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pAffectedCount = WebRequest.Instance.Business.LibraryStock.DeactivateAllProducts(WebSession.Instance.PeopleUserID)

            Common.WebRequest.ProData.Execute("DELETE FROM LibraryPimGroupPims WHERE ProductID IS NOT NULL") 'delete deactivated SKUs in HB by arnie 6-15-2021

            Return JsonDictionary.Serialize("AffectedCount", pAffectedCount)

        End Function

        <HttpPost>
        Public Function ReactivateAllProducts()

            Dim pAffectedCount As Integer

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pAffectedCount = WebRequest.Instance.Business.LibraryStock.ReactivateAllProducts(WebSession.Instance.PeopleUserID)

            Return JsonDictionary.Serialize("AffectedCount", pAffectedCount)

        End Function

        <HttpPost>
        Public Function GetSimilarBrands()

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT = WebRequest.Instance.Business.LibraryStock.Brands()
            Dim pSimilars = WebRequest.Instance.Business.LibraryStock.GetSimilarBrands(pDT)

            Return JsonConvert.SerializeObject(pSimilars)

        End Function

        <HttpPost>
        Public Function MergeDuplicateBrands() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSecurityAccess As New SecurityAccess()
            If pSecurityAccess.CheckAccessRights(WebSession.Instance.PeopleUserID, "Merge Duplicate Brands", False) = False Then
                Return JsonDictionary.Serialize("Error", "This feature requires the Merge Duplicate Brands access.")
            End If

            Dim pInput = JsonConvert.DeserializeObject(Of LibraryStock.SimilarBrandsResult)(Common.WebRequest.InputStreamText)

            WebRequest.Instance.Business.LibraryStock.MergeBrands(pInput)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function SaveRelatedProducts()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim sourceproductid As String = Common.WebRequest.Data("SourceProductID")
                Dim tag As String = Common.WebRequest.Data("Tag")
                Dim relprod As Object = Common.WebRequest.AllData("RelatedProducts")

                For Each prod As Object In relprod
                    WebRequest.Instance.Business.LibraryStock.SavePIMRelationship(sourceproductid, prod("ID"), tag)
                Next prod

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost>
        Function DeactivateRelatedProduct()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim p As New InsertUpdate("MobizLibraryPIMProductRelationship")
                p("IsActive") = 0
                p("LastUpdated") = "getdate()"
                p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID"))))

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost>
        Public Function GetRelatedProducts()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            pOut.Add("DirectCompetitors", WebRequest.Instance.Business.LibraryStock.GetRelatedProducts(Common.WebRequest.Data("ProductID"), "Direct Competitor"))
            pOut.Add("Variants", WebRequest.Instance.Business.LibraryStock.GetRelatedProducts(Common.WebRequest.Data("ProductID"), "Variant"))
            pOut.Add("Complements", WebRequest.Instance.Business.LibraryStock.GetRelatedProducts(Common.WebRequest.Data("ProductID"), "Complement"))

            Return pOut.Serialize()
        End Function

        <HttpPost>
        Public Function SaveAs() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSourceProductID = Common.WebRequest.Data("SourceProductID")
            Dim pProductNum = Common.WebRequest.Data("ProductNum")
            Dim pProductName = Common.WebRequest.Data("ProductName")
            Dim pProductBrand = Common.WebRequest.Data("ProductBrand")
            Dim pTagAs = Common.WebRequest.Data("TagAs")
            Dim pDR = WebRequest.Instance.Business.LibraryStock.ByNumberOrName(pProductNum, pProductName)
            If pDR IsNot Nothing Then
                Dim pOut = New JsonDictionary
                pOut.Add("Existing", pDR)
                Return pOut.Serialize()
            End If
            Dim pNewProductID = WebRequest.Instance.Business.LibraryStock.SaveAs(WebRequest.Instance.OrgID, pSourceProductID, pProductNum, pProductName, pProductBrand)
            WebRequest.Instance.Business.LibraryStock.SavePIMRelationship(pSourceProductID, pNewProductID, pTagAs)

            Return JsonDictionary.Serialize("NewProductID", pNewProductID)

        End Function

        <HttpPost>
        Public Function SavePimSalesCaching() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInput = JsonConvert.DeserializeObject(Of LibraryStock.SavePimSalesCachingInput)(Common.WebRequest.InputStreamText)
            WebRequest.Instance.Business.LibraryStock.SavePimSalesCaching(pInput, WebSession.Instance.PeopleUserID)
            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function GetPictureFilenames() As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = ""
            Dim fowstid As String = Common.WebRequest.Data("FowstID")
            'Dim pPath = Path.Combine(Common.ServerPath, "Content\MobizImagePIM", ReadyGuid(WebRequest.Instance.OrgID))
            'If Directory.Exists(pPath) Then
            '    Dim pFiles = IO.Directory.GetFiles(pPath, "*.jpg", SearchOption.AllDirectories)
            '    For Each pFile In pFiles
            '        If pOut <> "" Then
            '            pOut &= vbCrLf
            '        End If
            '        Dim pFilename = GetRelativePath(pPath, pFile)
            '        pFilename = pFilename.Replace("\", "/")
            '        pOut &= pFilename
            '    Next
            'End If

            Dim pFolder = WebRequest.Instance.Data("Folder")
            If String.IsNullOrWhiteSpace(pFolder) Then
                pFolder = "VendingMachine"
            End If

            'Check if FOWST (Vending Machine) is assigned to a Hot Button Template
            Dim hbid As String = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT DefaultHotButtonTemplateID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(fowstid)))

            If String.IsNullOrWhiteSpace(hbid) Then
                Return JsonDictionary.Serialize("Error", "No assigned Hot Button template")
            Else
                If pFolder = "MobizImagePIM" Then
                    'get all SKUs with image under assigned Hot Button Template
                    Dim filenames As Object = Common.WebRequest.ProData.Execute("SELECT FileName FROM LibraryPIMPictures WHERE IsDefault = 1 AND Size = 'big' AND ProductID IN (SELECT ProductID FROM LibraryPimGroupPims WHERE MenuID = " & ReadyString(hbid) & ")").Rows

                    For Each file As Object In filenames
                        If pOut <> "" Then
                            pOut &= vbCrLf
                        End If
                        pOut &= "/Big/" & file("FileName")
                    Next
                Else
                    Dim pPath = Path.Combine(Common.ServerPath, "Content\" & pFolder, ReadyGuid(WebRequest.Instance.OrgID))
                    If Directory.Exists(pPath) Then
                        Dim pFiles = IO.Directory.GetFiles(pPath, "*.*", SearchOption.AllDirectories)
                        For Each pFile In pFiles
                            If New IO.FileInfo(pFile).Length = 0 Then
                                Continue For
                            End If
                            If pOut <> "" Then
                                pOut &= vbCrLf
                            End If
                            Dim pFilename = GetRelativePath(pPath, pFile)
                            pFilename = pFilename.Replace("\", "/")
                            pOut &= pFilename
                        Next
                    End If
                End If
            End If

            Return JsonDictionary.Serialize("Result", pOut)

        End Function

        Public Function GetGuidesPictureFilenames() As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pPath = Path.Combine(Common.ServerPath, "Content\VendingMachine", ReadyGuid(WebRequest.Instance.OrgID))
            Dim pFiles = IO.Directory.GetFiles(pPath, "*", SearchOption.AllDirectories)
            Dim pOut = ""

            For Each pFile In pFiles
                If New IO.FileInfo(pFile).Length = 0 Then
                    Continue For
                End If
                If pOut <> "" Then
                    pOut &= vbCrLf
                End If
                Dim pFilename = GetRelativePath(pPath, pFile)
                pFilename = pFilename.Replace("\", "/")
                pOut &= pFilename
            Next

            Return JsonDictionary.Serialize("Result", pOut)

        End Function
    End Class

End Namespace