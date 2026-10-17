Imports Aspose.Cells

Public Class ProductGroupController
    Inherits System.Web.Mvc.Controller
    Public firstLevelID As String = ""
    Public secondLevelID As String = ""
    Public thirdLevelID As String = ""
    Public skuID As String = ""
    Dim isDone As Boolean = False
    Dim isParent As Boolean = False
    Dim existingGroupID As String

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function GetFowstHBTemplate()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim fowstid As String = Common.WebRequest.Data("FowstID")
            Dim pSql As String = "SELECT ISNULL(convert(varchar(max), DefaultHotButtonTemplateID), '') as HBTempID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(fowstid)
            Dim hbtempid As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("HBTempID")
            Dim pOut As New JsonDictionary

            If hbtempid = "" Then
                pSql = "SELECT ID, Name FROM LibraryPimMenu "
                pOut.Add("Templates", Common.WebRequest.ProData.Execute(pSql))
            Else
                pOut.Add("DefaultTemplate", hbtempid)
            End If

            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Public Function RemoveProductInHB(ByVal ProductID As String)
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String = "DELETE FROM LibraryPimGroupPims Where ProductID = " & ReadyString(ProductID)
            Common.WebRequest.ProData.Execute(pSql)

            Return True
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return False
        End Try
    End Function

    Function MenuTemplate() As ActionResult
        'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
        '    If WebRequest.Instance.HandleAccess = False Then Exit Function

        'End If

        Return View()

    End Function

    Function AssignFOWST() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function

        End If

        Return View()

    End Function

    Function GenerateBrandsCategories()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim arrData As Object
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim options As String = Common.WebRequest.Data("Option")
            Dim type As String = Common.WebRequest.Data("Type")
            Dim userid As String = Common.WebRequest.Data("UserID")
            Dim pSql As String
            Dim pDT As DataTable
            Dim ctrBrand As Integer = 1
            Dim columnname As String
            Dim brandid As String
            Dim pIU

            Common.WebRequest.ProData.BeginTransaction()

            If type = "custom" Then
                arrData = Common.WebRequest.AllData("Selection")
            Else
                arrData = jss.DeserializeObject(New BrowserController().LibraryPimBrands(True))("d")("Results")("Rows")
            End If

            For Each brand As Object In arrData
                pSql = "SELECT ID FROM LibraryPimGroups WHERE MenuID = " & ReadyString(menuid) & " AND Name = " & ReadyString(brand("Brand")) & " AND ParentID IS NULL "
                Dim pRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                Dim ctrCategory As Integer = 1
                Dim categoryid As String
                Dim arrCategory As Object

                If pRows.Count = 0 Then
                    pIU = New InsertUpdate("LibraryPimGroups")

                    brandid = Guid.NewGuid.ToString()
                    pIU("Name") = ReadyString(brand("Brand"))
                    pIU("LastUpdated") = "GETDATE()"
                    pIU("UserID") = ReadyString(userid)
                    pIU("IsActive") = CDec(1)
                    pIU("MenuID") = ReadyString(menuid)
                    pIU("OrderNo") = CInt2(ctrBrand)

                    pIU("ID") = ReadyString(brandid)
                    pSql = pIU.GetInsert()

                    Common.WebRequest.ProData.Execute(pSql)
                Else
                    brandid = pRows(0)("ID").ToString()
                End If

                ctrBrand += 1

                If type = "all" Then
                    pSql = " SELECT C0020 as Name " & vbCrLf &
                            " FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                            " WHERE ProductID IN ( " & vbCrLf &
                            "   SELECT " & vbCrLf &
                            "   pim.ID " & vbCrLf &
                            "   FROM LibraryOrganizationPIM as mypim " & vbCrLf &
                            "   INNER JOIN MobizLibraryPIMProduct as pim ON mypim.ProductID = pim.ID " & vbCrLf &
                            "   WHERE Brand = " & ReadyString(brand("Brand")) & ") " & vbCrLf &
                            " GROUP BY C0020 ORDER BY C0020 ASC "
                    arrCategory = Common.WebRequest.ProData.Execute(pSql).Rows
                Else
                    arrCategory = brand("SelectedCategories")
                End If

                For Each cat As Object In arrCategory
                    pSql = "SELECT ID FROM LibraryPimGroups WHERE MenuID = " & ReadyString(menuid) & " AND Name = " & ReadyString(cat("Name")) & " AND ParentID = " & ReadyString(brandid)
                    pRows = Common.WebRequest.ProData.Execute(pSql).Rows
                    Dim ctrSku As Integer = 1

                    If pRows.Count = 0 Then
                        pIU = New InsertUpdate("LibraryPimGroups")

                        categoryid = Guid.NewGuid.ToString()
                        pIU("Name") = ReadyString(cat("Name"))
                        pIU("ParentID") = ReadyString(brandid)
                        pIU("LastUpdated") = "GETDATE()"
                        pIU("UserID") = ReadyString(userid)
                        pIU("IsActive") = CDec(1)
                        pIU("MenuID") = ReadyString(menuid)
                        pIU("OrderNo") = CInt2(ctrCategory)

                        pIU("ID") = ReadyString(categoryid)
                        pSql = pIU.GetInsert()

                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        categoryid = pRows(0)("ID").ToString()
                    End If

                    ctrCategory += 1

                    pSql = " SELECT MobizLibraryPIMProductVariableFeatures001.ProductID, MobizLibraryPIMProduct.ProductName " & vbCrLf &
                            " FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductVariableFeatures001.ProductID " & vbCrLf &
                            " INNER JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID " & vbCrLf &
                            " WHERE MobizLibraryPIMProductVariableFeatures001.ProductID IN ( " & vbCrLf &
                            "   SELECT " & vbCrLf &
                            "   pim.ID " & vbCrLf &
                            "   FROM LibraryOrganizationPIM as mypim " & vbCrLf &
                            "   INNER JOIN MobizLibraryPIMProduct as pim ON mypim.ProductID = pim.ID " & vbCrLf &
                            "   WHERE Brand = " & ReadyString(brand("Brand")) & ") " & vbCrLf &
                            " AND C0020 = " & ReadyString(cat("Name")) & " AND LibraryOrganizationPIM.IsDeactivated = 0 " & vbCrLf &
                            " ORDER BY MobizLibraryPIMProduct.ProductName ASC "
                    pRows = Common.WebRequest.ProData.Execute(pSql).Rows

                    For Each prod As Object In pRows
                        Dim shortname As String = Left(prod("ProductName"), 50)
                        Dim newshortname As String = CheckShortName(menuid, categoryid, shortname, 1)
                        If newshortname <> "" Then
                            shortname = newshortname
                        End If

                        If Not IsExist(prod("ProductID").ToString(), categoryid, menuid, "SKU", True) Then
                            pIU = New InsertUpdate("LibraryPimGroupPims")

                            pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                            pIU("GroupID") = ReadyString(categoryid)
                            pIU("ProductID") = ReadyString(prod("ProductID").ToString())
                            pIU("MenuID") = ReadyString(menuid)
                            pIU("IsActive") = CDec(1)
                            pIU("OrderNo") = CInt2(ctrSku)
                            pIU("ShortName") = ReadyString(shortname)

                            pSql = pIU.GetInsert()
                            Common.WebRequest.ProData.Execute(pSql)
                        End If

                        ctrSku += 1
                    Next
                Next
            Next

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    <HttpPost>
    Function GenerateHB() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim arrData As Object
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim options As String = Common.WebRequest.Data("Option")
            Dim type As String = Common.WebRequest.Data("Type")
            Dim pSql As String
            Dim pDT As DataTable
            Dim ctr1 As Integer = 1
            Dim columnname As String

            If options = "category" Then
                If type = "custom" Then
                    arrData = Common.WebRequest.AllData("Selection")
                Else
                    arrData = jss.DeserializeObject(New BrowserController().LibraryPimCategories(True))("d")("Results")("Rows")
                End If

                columnname = "Category"
            ElseIf options = "brand" Then
                If type = "custom" Then
                    arrData = Common.WebRequest.AllData("Selection")
                Else
                    arrData = jss.DeserializeObject(New BrowserController().LibraryPimBrands(True))("d")("Results")("Rows")
                End If

                columnname = "Brand"
            End If

            Common.WebRequest.ProData.BeginTransaction()

            For Each cat As Object In arrData
                Dim name As String = CStr2(cat(columnname))
                If cat.Count > 0 Then
                    If name <> "" Then
                        Dim groupid As String
                        Dim pIU As InsertUpdate
                        Dim ctr2 As Integer = 1
                        Dim isExistGroup As Boolean = False

                        If Not IsExist(Nothing, Nothing, menuid, name, True) Then
                            groupid = Guid.NewGuid.ToString
                            isExistGroup = False
                        Else
                            groupid = existingGroupID
                            isExistGroup = True
                        End If

                        ctr1 += 1

                        pSql = "SELECT MobizLibraryPIMProduct.ID, MobizLibraryPIMProduct.ProductName, LibraryOrganizationPIM.IsDeactivated " & vbCrLf &
                                " FROM MobizLibraryPIMProduct " & vbCrLf &
                                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                                " WHERE " & IIf(options = "category", "MobizLibraryPIMProductVariableFeatures001.C0020", "MobizLibraryPIMProduct.Brand") & " = " & ReadyString(name) & vbCrLf &
                                " AND ISNULL(MobizLibraryPIMProduct.IsAsset, 0) = 0 " & vbCrLf &
                                " Order By MobizLibraryPIMProduct.ProductName ASC "

                        pDT = Common.WebRequest.ProData.Execute(pSql)

                        Dim isGroupButtonCreated = False

                        If pDT.Rows.Count > 0 Then
                            For Each prod As Object In pDT.Rows
                                If Not CBool2(prod("IsDeactivated")) Then
                                    If Not isExistGroup And Not isGroupButtonCreated Then
                                        pIU = New InsertUpdate("LibraryPimGroups")
                                        pIU("Name") = ReadyString(name)
                                        pIU("LastUpdated") = "GETDATE()"
                                        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        pIU("IsActive") = CDec(1)
                                        pIU("MenuID") = ReadyString(menuid)
                                        pIU("OrderNo") = CInt2(ctr1)

                                        pIU("ID") = ReadyString(groupid)
                                        pSql = pIU.GetInsert()

                                        Common.WebRequest.ProData.Execute(pSql)

                                        isGroupButtonCreated = True
                                    End If
                                    Dim shortname As String = Left(prod("ProductName"), 50)
                                    Dim newshortname As String = CheckShortName(menuid, groupid, shortname, 1)
                                    If newshortname <> "" Then
                                        shortname = newshortname
                                    End If

                                    If Not IsExist(prod("ID").ToString(), groupid, menuid, "SKU", True) Then
                                        pIU = New InsertUpdate("LibraryPimGroupPims")

                                        pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                                        pIU("GroupID") = ReadyString(groupid)
                                        pIU("ProductID") = ReadyString(prod("ID").ToString())
                                        pIU("MenuID") = ReadyString(menuid)
                                        pIU("ShortName") = ReadyString(shortname)
                                        pIU("IsActive") = 1
                                        pIU("OrderNo") = CInt2(ctr2)
                                        pSql = pIU.GetInsert()

                                        Common.WebRequest.ProData.Execute(pSql)
                                    End If
                                Else
                                    pSql = "DELETE FROM LibraryPimGroupPims WHERE GroupID = " & ReadyString(groupid) & vbCrLf &
                                            " AND ProductID = " & ReadyString(prod("ID").ToString())
                                    Common.WebRequest.ProData.Execute(pSql)
                                End If

                                ctr2 += 1
                            Next prod
                        End If
                    End If
                End If
            Next cat

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function CheckShortName(ByVal menuid As String, ByVal groupid As String, ByVal name As String, ByVal ctrRename As Integer) As String
        Dim pSql As String = "SELECT ID, ShortName" & vbCrLf &
                            " FROM LibraryPimGroupPims" & vbCrLf &
                            " WHERE IsActive = 1 AND MenuID = " & ReadyString(menuid) & " AND GroupID = " & ReadyString(groupid) & " AND ShortName= " & ReadyString(name)
        Dim row As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

        If row > 0 Then
            Dim newname As String = (ctrRename + 1) & " " & Left(name, 49)
            Return CheckShortName(menuid, groupid, newname, (ctrRename + 1))
        Else
            Return ""
        End If
    End Function

    <HttpPost>
    Function RenameShortName() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim p As New InsertUpdate("LibraryPimGroups")
            Dim NewName As String = Common.WebRequest.Data("Name")
            Dim ID As String = Common.WebRequest.Data("ID")
            Dim Type As String = Common.WebRequest.Data("Type")

            If Type = "sku" Then
                p = New InsertUpdate("LibraryPimGroupPims")
                p("ShortName") = ReadyString(NewName)
            Else
                p("Name") = ReadyString(NewName)
            End If

            pSql = p.GetUpdate("ID = " & ReadyString(ID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.Execute(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try

    End Function

    Function GetCategoriesOfSelectedBrands()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim brands As Object = Common.WebRequest.AllData("Brands")

            For Each brand As Object In brands
                Dim pSql As String = " SELECT C0020 as Name " & vbCrLf &
                                    " FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                                    " WHERE ProductID IN ( " & vbCrLf &
                                    "   SELECT " & vbCrLf &
                                    "   pim.ID " & vbCrLf &
                                    "   FROM LibraryOrganizationPIM as mypim " & vbCrLf &
                                    "   INNER JOIN MobizLibraryPIMProduct as pim ON mypim.ProductID = pim.ID " & vbCrLf &
                                    "   WHERE Brand = " & ReadyString(brand("Brand")) & ") " & vbCrLf &
                                    " GROUP BY C0020 ORDER BY C0020 ASC "

                brand.Add("Categories", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
            Next

            Return Json(New With {Key .data = brands})
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function NewGetHotButtons() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim firstLevel As Object = GetHB(menuid, "").Rows
            Dim arrHB As New List(Of Dictionary(Of String, Object))
            Dim arrSubGroup As New List(Of Dictionary(Of String, Object))

            For Each first As Object In firstLevel
                Dim pHB As New Dictionary(Of String, Object)
                pHB.Add("ID", first("ID"))
                pHB.Add("Name", first("Name"))
                pHB.Add("IsActive", first("IsActive"))
                pHB.Add("IsGroup", first("IsGroup"))
                pHB.Add("OrderNo", first("OrderNo"))
                pHB.Add("State", "edit")

                If CBool2(first("IsGroup")) Then
                    pHB.Add("Level", 1)
                    pHB.Add("DefaultOtherPaymentID", first("DefaultOtherPaymentID"))
                    pHB.Add("SubGroup", New List(Of Dictionary(Of String, Object))) 'GetSubGroup(menuid, first("ID").ToString(), 2)
                Else
                    pHB.Add("ProductID", first("ProductID"))
                    pHB.Add("ShortName", first("ShortName"))
                    pHB.Add("ProductNo", first("ProductNo"))
                    pHB.Add("IsUseUOM", first("IsUseUOM"))
                    pHB.Add("SellingPreference", first("SellingPreference"))
                    pHB.Add("RecommendedSKU", first("RecommendedSKU"))
                    pHB.Add("RecommendedSKUName", first("RecommendedSKUName"))
                    If first.Table.Columns.Contains("CategoryName") Then
                        pHB.Add("CategoryName", first("CategoryName"))
                        pHB.Add("HasModifier", first("HasModifier"))
                    End If

                    If first.Table.Columns.Contains("RetailPrice") Then
                        pHB.Add("RetailPrice", first("RetailPrice"))
                        pHB.Add("UnitMeasure", first("UnitMeasure"))
                        pHB.Add("UnitQty", first("UnitQty"))
                    End If
                End If

                pHB.Add("StoreAssignment", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(GetHotButtonStoreAssignment(first("ID").ToString()))))
                arrHB.Add(pHB)
            Next first

            Dim pSql As String = "SELECT ID, Name as FowstName, 'edit' as State FROM MobizLibraryOrganizationFOWST WHERE DefaultHotButtonTemplateID = " & ReadyString(menuid) &
                                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) &
                                " ORDER BY Name ASC"
            Dim PDTAssignedFowst As DataTable = Common.WebRequest.ProData.Execute(pSql)

            pSql = "SELECT Name as FowstName FROM MobizLibraryOrganizationFOWST WHERE ISNULL(convert(varchar(max), DefaultHotButtonTemplateID), '') = '' " &
                                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) &
                                " AND IsActive = 1 " &
                                " ORDER BY Name ASC"
            Dim PDTUnAssignedFowst As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return Json(New With {Key .Results = arrHB,
                                  .AssignedFowst = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(PDTAssignedFowst)),
                                  .UnAssignedFowst = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(PDTUnAssignedFowst))
                                 })

        Catch ex As Exception
            Return Json(New With {Key .Results = ex.Message})
        End Try
    End Function

    Function GetSubGroup(Optional ByVal menuid As String = "", Optional ByVal GroupID As String = "", Optional ByVal Level As Integer = Nothing) As Object
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim arrHB As New List(Of Dictionary(Of String, Object))
        Dim arrSubGroup As New List(Of Dictionary(Of String, Object))
        Dim pMenuID As String = IIf(menuid = "", Common.WebRequest.Data("MenuID"), menuid)
        Dim pGroupID As String = IIf(GroupID = "", Common.WebRequest.Data("GroupID"), GroupID)
        Dim pLevel As String = IIf(Level = Nothing, Common.WebRequest.Data("Level"), Level)
        Dim subgroup As Object = GetHB(pMenuID, pGroupID).Rows

        For Each subg As Object In subgroup
            Dim pHB As New Dictionary(Of String, Object)
            pHB.Add("ID", subg("ID"))
            pHB.Add("Name", subg("Name"))
            pHB.Add("IsActive", subg("IsActive"))
            pHB.Add("IsGroup", subg("IsGroup"))
            pHB.Add("OrderNo", subg("OrderNo"))
            pHB.Add("State", "edit")

            If CBool2(subg("IsGroup")) Then
                pHB.Add("Level", pLevel)
                pHB.Add("DefaultOtherPaymentID", subg("DefaultOtherPaymentID"))
                pHB.Add("SubGroup", New List(Of Dictionary(Of String, Object))) 'GetSubGroup(menuid, subg("ID").ToString(), 3)
            Else
                pHB.Add("ProductID", subg("ProductID"))
                pHB.Add("ShortName", subg("ShortName"))
                pHB.Add("ProductNo", subg("ProductNo"))
                pHB.Add("VDMMotorNo", subg("VDMMotorNo"))
                pHB.Add("VDMBinNo", subg("VDMBinNo"))
                pHB.Add("CurVDMMotorNo", subg("CurVDMMotorNo"))
                pHB.Add("CurVDMBinNo", subg("CurVDMBinNo"))
                pHB.Add("IsUseUOM", subg("IsUseUOM"))
                pHB.Add("SellingPreference", subg("SellingPreference"))
                pHB.Add("RecommendedSKU", subg("RecommendedSKU"))
                pHB.Add("RecommendedSKUName", subg("RecommendedSKUName"))
                pHB.Add("IsDoNotIncludeBaseUOM", subg("IsDoNotIncludeBaseUOM"))
                If subg.Table.Columns.Contains("CategoryName") Then
                    pHB.Add("CategoryName", subg("CategoryName"))
                    pHB.Add("HasModifier", subg("HasModifier"))
                End If

                If subg.Table.Columns.Contains("RetailPrice") Then
                    pHB.Add("RetailPrice", subg("RetailPrice"))
                    pHB.Add("UnitMeasure", subg("UnitMeasure"))
                    pHB.Add("UnitQty", subg("UnitQty"))
                End If
            End If
            pHB.Add("StoreAssignment", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(GetHotButtonStoreAssignment(subg("ID").ToString()))))
            arrHB.Add(pHB)
        Next subg

        Return Json(New With {Key .Results = arrHB}, JsonRequestBehavior.AllowGet) 'arrHB
    End Function

    Function GetHotButtonStoreAssignment(ByVal hbid As String)
        Dim pSql As String

        pSql = "SELECT fowst.ID, fowst.Name as FowstName " & vbCrLf &
                " FROM LibraryPimMenuButtonAssignedStores as hbstore " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = hbstore.FowstID" & vbCrLf &
                " WHERE HotButtonID = " & ReadyString(hbid) & vbCrLf &
                " ORDER BY fowst.Name ASC " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetHB(ByVal menuid As String, ByVal groupid As String) As DataTable
        Dim IncludeUnitPrice = CBool2(Common.WebRequest.Data("IncludeUnitPrice"))
        Dim FromWebPOS = CBool2(Common.WebRequest.Data("FromWebPOS"))
        Dim pSql As String = "SELECT * FROM ( " & vbCrLf &
                                "   SELECT gr.ID, '' as ProductID, gr.OrderNo, 'edit' as State, 1 as IsGroup, '' as VDMMotorNo, '' as VDMBinNo, '' as CurVDMMotorNo, '' as CurVDMBinNo, " & vbCrLf &
                                "       gr.Name as Name, '' as ProductNo, '' as ShortName, gr.IsActive, ISNULL(convert(varchar(max), DefaultOtherPaymentID), '') as DefaultOtherPaymentID, 0 as IsUseUOM, 0 as IsDoNotIncludeBaseUOM, '' as SellingPreference, '' as RecommendedSKU, '' as RecommendedSKUName " & vbCrLf

        If IncludeUnitPrice Then
            pSql &= ", NULL as RetailPrice, NULL as UnitQty, NULL as UnitMeasure" & vbCrLf
        End If

        If FromWebPOS Then
            pSql &= ", '' as CategoryName, NULL as HasModifier " & vbCrLf
        End If

        pSql &= " FROM LibraryPIMGroups as gr " & vbCrLf &
                " WHERE gr.MenuID = " & ReadyString(menuid) & vbCrLf &
                IIf(groupid = "", " AND ISNULL(convert(varchar(max), ParentID), '') = '' ", " AND ParentID = " & ReadyString(groupid)) & vbCrLf &
                "   UNION ALL " & vbCrLf &
                "   SELECT sku.ID, convert(varchar(max), product.ID) as ProductID, sku.OrderNo, " & vbCrLf &
                "       'edit' as State, 0 as IsGroup, ISNULL(sku.VDMMotorNo, '') as VDMMotorNo, " & vbCrLf &
                "       ISNULL(sku.VDMBinNo, '') as VDMBinNo, " & vbCrLf &
                "       ISNULL(sku.VDMMotorNo, '') as CurVDMMotorNo, " & vbCrLf &
                "       ISNULL(sku.VDMBinNo, '') as CurVDMBinNo, " & vbCrLf &
                "       product.ProductName as Name, product.ProductNo, " & vbCrLf &
                "       sku.ShortName, sku.IsActive, '' as DefaultOtherPaymentID, ISNULL(IsUseUOM, 0) as IsUseUOM, ISNULL(IsDoNotIncludeBaseUOM, 0) as IsDoNotIncludeBaseUOM, ISNULL(SellingPreference, '') as SellingPreference, ISNULL(RecommendedSKU, 'No Recommendation') as RecommendedSKU, " & vbCrLf &
                "       CASE WHEN ISNULL(RecommendedSKU, 'No Recommendation') = 'No Recommendation' THEN '' ELSE (SELECT ProductNo + ' - ' + ProductName FROM MobizLibraryPimProduct WHERE ID = RecommendedSKU) END as RecommendedSKUName "

        If IncludeUnitPrice Then
            pSql &= ", prc.Price as RetailPrice, um.UnitQty, um.UnitMeasure" & vbCrLf
        End If

        If FromWebPOS Then
            pSql &= ", ProductCategories.CategoryName " & vbCrLf &
                    ", CASE WHEN ProductCategories.ProductID IS NOT NULL THEN 1 ELSE 0 END AS HasModifier " & vbCrLf
        End If

        pSql &= " FROM LibraryPimGroupPims as sku " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as product ON product.ID = sku.ProductID " & vbCrLf

        If IncludeUnitPrice Then
            pSql &= " OUTER APPLY (" & vbCrLf &
                    "     SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                    "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                    "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = sku.ProductID" & vbCrLf &
                    "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0005-0000-0000-000000000000'" & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                    "   ) prc" & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                    "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                    " 	    ,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                    "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                    "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                    "       WHERE MobizLibraryPIMProductMeasurement.ProductID = sku.ProductID AND MobizLibraryPIMProductMeasurement.Quantity = 1 " & vbCrLf &
                    "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf
        End If

        If FromWebPOS Then
            pSql &= " OUTER APPLY (SELECT MAX(MobizLibraryPIMProductVariableFeatures001.C0020) AS CategoryName " & vbCrLf &
                    "   ,ISNULL(LibraryItemModifierCategories.ProductID, MobizLibraryPIMProductVariableFeatures001.ProductID) AS ProductID " & vbCrLf &
                    "   FROM LibraryItemModifierCategories " & vbCrLf &
                    "   INNER JOIN LibraryItemModifiers ON LibraryItemModifiers.ID = LibraryItemModifierCategories.ItemModifierID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryPIMProductVariableFeatures001 ON sku.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID " & vbCrLf &
                    "   WHERE LibraryItemModifiers.IsActive = 1 AND (LibraryItemModifierCategories.ProductID = sku.ProductID OR LibraryItemModifierCategories.CategoryName = MobizLibraryPIMProductVariableFeatures001.C0020) " & vbCrLf &
                    "   GROUP BY ISNULL(LibraryItemModifierCategories.ProductID, MobizLibraryPIMProductVariableFeatures001.ProductID) " & vbCrLf &
                    " ) as ProductCategories " & vbCrLf
        End If

        pSql &= " WHERE MenuID = " & ReadyString(menuid) & vbCrLf &
        IIf(groupid = "", " AND ISNULL(convert(varchar(max), GroupID), '') = '' ", " AND GroupID = " & ReadyString(groupid)) & vbCrLf

        pSql &= ") as t ORDER BY OrderNo asc, IsGroup desc, Name asc " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function SaveHotButtons() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim IsDisplayAllProducts As String = Common.WebRequest.Data("IsDisplayAllProducts")
            Dim arrHB As Object = Common.WebRequest.AllData("HotButtons")
            Dim arrHBFowst As Object = Common.WebRequest.AllData("AssignedFowst")
            Dim errMsg As String = ""
            Dim ctr As Integer = 1
            Dim p As New InsertUpdate("LibraryPimMenu")

            Common.WebRequest.ProData.BeginTransaction()

            p("IsDisplayAllProducts") = ReadyString(CInt2(IsDisplayAllProducts))

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(menuid.ToString())))

            For Each hb As Object In arrHB
                If CBool2(hb("IsGroup")) Then
                    If Not NewSaveGroup(ctr, hb, menuid, "", errMsg) Then
                        Return JsonDictionary.Serialize("Results", errMsg)
                    End If
                Else
                    If Not NewSaveSKU(ctr, hb, menuid, "", errMsg) Then
                        Return JsonDictionary.Serialize("Results", errMsg)
                    End If
                End If

                ctr += 1
            Next hb

            For Each fowst As Object In arrHBFowst
                p = New InsertUpdate("MobizLibraryOrganizationFOWST")
                Dim err As String = ""
                If fowst("State") = "remove" Then
                    p("DefaultHotButtonTemplateID") = "NULL"
                Else
                    p("DefaultHotButtonTemplateID") = ReadyString(menuid)
                End If

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(fowst("ID").ToString())))

                If Not GenerateHBBin(fowst("ID").ToString(), menuid, err) Then
                    Throw New ApplicationException(err)
                End If
            Next fowst

            pSql = "Update LibraryPimMenu Set LastUpdated = GetDate(), UserID = " & ReadyString(WebSession.Instance.PeopleUserID()) & " Where ID =" & ReadyString(menuid)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function GenerateHBBin(ByVal FowstID As String, ByVal menuid As String, ByRef err As String)
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String

            pSql = "SELECT ProductID, ISNULL(VDMMotorNo, '') as VDMMotorNo, ISNULL(VDMBinNo, '') as VDMBinNo, ProductNo, ProductName, SRP " & vbCrLf &
                    " FROM LibraryPimGroupPims" & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = LibraryPimGroupPims.ProductID " & vbCrLf &
                    " WHERE LibraryPimGroupPims.IsActive = 1 AND VDMMotorNo IS NOT NULL AND MenuID = " & ReadyString(menuid) & vbCrLf &
                    " ORDER BY VDMMotorNo ASC " & vbCrLf

            Dim arrMotorNos As Object = Common.WebRequest.ProData.Execute(pSql).Rows

            If Not GenerateHBBinMotors("motor", FowstID, arrMotorNos, err) Then
                Throw New ApplicationException(err)
            End If

            If Not GenerateHBBinMotors("bin", FowstID, arrMotorNos, err) Then
                Throw New ApplicationException(err)
            End If

            Return True
        Catch ex As Exception
            err = ex.Message
            Return False
        End Try
    End Function

    Function GenerateHBBinMotors(ByVal vmotortype As String, ByVal FowstID As String, ByVal arrMotorNos As Object, ByRef err As String)
        Try
            Dim pSql As String
            Dim hbmotors As List(Of String) = New List(Of String)
            Dim p
            Dim colName As String
            Dim binname As String

            If vmotortype = "motor" Then
                colName = "VDMMotorNo"
                binname = "Motor No. "
            Else
                colName = "VDMBinNo"
                binname = "Bin No. "
            End If

            For Each motors As Object In arrMotorNos
                Dim motor() As String = motors(colName).ToString().Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)

                For Each no As String In motor
                    no = CDec2(no)
                    hbmotors.Add(no)

                    p = New InsertUpdate("LibraryFowstBin")

                    p("FowstID") = ReadyString(FowstID)
                    p("Name") = ReadyString(binname & no)
                    p("BinNo") = ReadyString(no)
                    p("IsActive") = 1
                    p("LastUpdated") = "getdate()"
                    p("DateImported") = "getdate()"

                    pSql = "SELECT BinID FROM LibraryFowstBinProduct WHERE IsActive = 1 AND FowstID = " & ReadyString(FowstID) & vbCrLf &
                            " AND ProductID = " & ReadyString(motors("ProductID").ToString())

                    Dim curData As DataTable = Common.WebRequest.ProData.Execute(pSql)
                    Dim binid As String = ""
                    Dim binproduct As Boolean = False
                    Dim bin As Boolean = False

                    For Each binpro As Object In curData.Rows
                        binid = binpro("BinID").ToString()
                        pSql = "SELECT ID FROM LibraryFowstBin WHERE ID = " & ReadyString(binid) & vbCrLf &
                                " AND BinNo = " & CInt2(no) &
                                " AND Name = " & ReadyString(binname & CInt2(no))
                        Dim binData As DataTable = Common.WebRequest.ProData.Execute(pSql)

                        If binData.Rows.Count > 0 Then
                            binproduct = True
                            bin = True
                            Exit For
                        End If
                    Next

                    If Not binproduct And Not bin Then
                        binid = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(binid)

                        Common.WebRequest.ProData.Execute(p.GetInsert())

                        p = New InsertUpdate("LibraryFowstBinProduct")

                        p("FowstID") = ReadyString(FowstID)
                        p("BinID") = ReadyString(binid)
                        p("ProductID") = ReadyString(motors("ProductID").ToString())
                        p("IsDefault") = 1
                        p("LastUpdated") = "getdate()"
                        p("DateImported") = "getdate()"
                        p("IsActive") = 1
                        p("Balance") = 0
                        p("ProductNo") = ReadyString(motors("ProductNo"))
                        p("ProductName") = ReadyString(motors("ProductName"))
                        p("SRP") = CDec2(motors("SRP"))

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    Else
                        p = New InsertUpdate("LibraryFowstBin")
                        p("IsActive") = 1
                        p("LastUpdated") = "getdate()"
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID IN (SELECT BinID FROM LibraryFowstBinProduct " & vbCrLf &
                                " WHERE ProductID = " & ReadyString(motors("ProductID").ToString()) & vbCrLf &
                                " AND FOWSTID = " & ReadyString(FowstID) & vbCrLf &
                                " AND BinID IN (SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(FowstID) & vbCrLf &
                                " AND BinNo IN (" & no & ")" & vbCrLf &
                                " AND Name IN ('" & binname & no & "')))"))

                        p = New InsertUpdate("LibraryFowstBinProduct")
                        p("IsActive") = 1
                        p("LastUpdated") = "getdate()"
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID IN (SELECT ID FROM LibraryFowstBinProduct " & vbCrLf &
                                " WHERE ProductID = " & ReadyString(motors("ProductID").ToString()) & vbCrLf &
                                " AND FOWSTID = " & ReadyString(FowstID) & vbCrLf &
                                " AND BinID IN (SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(FowstID) & vbCrLf &
                                " AND BinNo IN (" & no & ")" & vbCrLf &
                                " AND Name IN ('" & binname & no & "')))"))
                    End If
                Next

                If motor.Count > 0 Then
                    Dim updatedNames As String() = hbmotors.Select(Function(name) binname & name).ToArray()
                    'Deactivate motor na hindi na ginamit ng hot Button
                    p = New InsertUpdate("LibraryFowstBin")
                    p("IsActive") = 0
                    p("LastUpdated") = "getdate()"
                    pSql = p.GetUpdate(" ID IN (SELECT BinID FROM LibraryFowstBinProduct " & vbCrLf &
                                        " WHERE ProductID = " & ReadyString(motors("ProductID").ToString()) & vbCrLf &
                                        " AND FOWSTID = " & ReadyString(FowstID) & vbCrLf &
                                        " AND BinID NOT IN (SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(FowstID) & vbCrLf &
                                        " AND BinNo IN (" & String.Join(", ", hbmotors) & ")" & vbCrLf &
                                        " AND Name IN ('" & String.Join("', '", updatedNames) & "')))")
                    Common.WebRequest.ProData.Execute(pSql)

                    p = New InsertUpdate("LibraryFowstBinProduct")
                    p("IsActive") = 0
                    p("IsDefault") = 0
                    p("Balance") = 0
                    p("LastUpdated") = "getdate()"
                    pSql = p.GetUpdate(" ID IN (SELECT ID FROM LibraryFowstBinProduct " & vbCrLf &
                                        " WHERE ProductID = " & ReadyString(motors("ProductID").ToString()) & vbCrLf &
                                        " And FOWSTID = " & ReadyString(FowstID) & vbCrLf &
                                        " And BinID Not IN (SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(FowstID) & vbCrLf &
                                        " AND BinNo IN (" & String.Join(", ", hbmotors) & ")" & vbCrLf &
                                        " AND Name IN ('" & String.Join("', '", updatedNames) & "')))")
                    Common.WebRequest.ProData.Execute(pSql)
                End If
            Next
            Return True
        Catch ex As Exception
            err = ex.Message
            Return False
        End Try
    End Function

    Function DuplicateHBTemplate()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim p As New InsertUpdate("LibraryPimMenu")
            Dim pSql As String = "SELECT * FROM LibraryPimMenu WHERE ID = " & ReadyString(menuid)
            Dim templatedata As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)
            Dim newtempid As String = Guid.NewGuid.ToString()
            Dim oldtempid As String = templatedata("ID").ToString()

            p("ID") = ReadyString(newtempid)
            p("Name") = ReadyString(templatedata("Name") & " - Copy")
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("IsActive") = 1
            p("LastUpdated") = "getdate()"
            p("VDMModelID") = IIf(IsGuid(templatedata("VDMModelID")), templatedata("VDMModelID"), "NULL")
            p("IsDisplayAllProducts") = ReadyString(templatedata("IsDisplayAllProducts"))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.Execute(p.GetInsert())

            Dim firstLevel As Object = GetHB(oldtempid, "").Rows

            For Each first As Object In firstLevel
                If CBool2(first("IsGroup")) Then
                    p = New InsertUpdate("LibraryPimGroups")
                    Dim newgroupid As String = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(newgroupid)
                    p("ParentID") = "NULL"
                    p("Name") = ReadyString(first("Name").ToString())
                    p("IsActive") = CInt2(first("IsActive"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("MenuID") = ReadyString(newtempid)
                    p("LastUpdated") = "getdate()"
                    p("OrderNo") = CInt2(first("OrderNo"))
                    p("DefaultOtherPaymentID") = IIf(IsGuid(first("DefaultOtherPaymentID")), first("DefaultOtherPaymentID"), "NULL")

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    DuplicateGroups(oldtempid, first("ID").ToString(), newtempid, newgroupid)
                Else
                    p = New InsertUpdate("LibraryPimGroupPims")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("ProductID") = ReadyString(first("ProductID").ToString())
                    p("MenuID") = ReadyString(newtempid)
                    p("ShortName") = ReadyString(first("ShortName").ToString())
                    p("IsActive") = CInt2(first("IsActive"))
                    p("OrderNo") = CInt2(first("OrderNo"))
                    p("VDMMotorNo") = IIf(String.IsNullOrEmpty(first("VDMMotorNo")), "NULL", first("VDMMotorNo"))
                    p("IsUseUOM") = IIf(String.IsNullOrEmpty(first("IsUseUOM")), "NULL", first("IsUseUOM"))
                    p("SellingPreference") = IIf(String.IsNullOrEmpty(first("SellingPreference")), "NULL", first("SellingPreference"))
                    p("RecommendedSKU") = ReadyString(first("RecommendedSKU"))
                    p("IsDoNotIncludeBaseUOM") = IIf(String.IsNullOrEmpty(first("IsDoNotIncludeBaseUOM")), "NULL", first("IsDoNotIncludeBaseUOM"))

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If
            Next

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "success")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function DuplicateGroups(ByVal oldmenuid As String, ByVal oldgroupid As String, ByVal newmenuid As String, ByVal newgroupid As String)
        Dim subgroup As Object = GetHB(oldmenuid, oldgroupid).Rows
        Dim p
        For Each subg As Object In subgroup
            If CBool2(subg("IsGroup")) Then
                p = New InsertUpdate("LibraryPimGroups")
                Dim newgrouprowid As String = Guid.NewGuid.ToString()

                p("ID") = ReadyString(newgrouprowid)
                p("ParentID") = IIf(IsGuid(newgroupid), ReadyString(newgroupid), "NULL")
                p("Name") = ReadyString(subg("Name").ToString())
                p("IsActive") = CInt2(subg("IsActive"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("MenuID") = ReadyString(newmenuid)
                p("LastUpdated") = "getdate()"
                p("OrderNo") = CInt2(subg("OrderNo"))
                p("DefaultOtherPaymentID") = IIf(IsGuid(subg("DefaultOtherPaymentID")), subg("DefaultOtherPaymentID"), "NULL")

                Common.WebRequest.ProData.Execute(p.GetInsert())

                DuplicateGroups(oldmenuid, subg("ID").ToString(), newmenuid, newgrouprowid)
            Else
                p = New InsertUpdate("LibraryPimGroupPims")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("GroupID") = IIf(IsGuid(newgroupid), ReadyString(newgroupid), "NULL")
                p("ProductID") = ReadyString(subg("ProductID").ToString())
                p("MenuID") = ReadyString(newmenuid)
                p("ShortName") = ReadyString(subg("ShortName").ToString())
                p("IsActive") = CInt2(subg("IsActive"))
                p("OrderNo") = CInt2(subg("OrderNo"))
                p("VDMMotorNo") = IIf(String.IsNullOrEmpty(subg("VDMMotorNo")), "NULL", subg("VDMMotorNo"))
                p("IsUseUOM") = IIf(String.IsNullOrEmpty(subg("IsUseUOM")), "NULL", subg("IsUseUOM"))
                p("SellingPreference") = IIf(String.IsNullOrEmpty(subg("SellingPreference")), "NULL", subg("SellingPreference"))
                p("RecommendedSKU") = ReadyString(subg("RecommendedSKU"))
                p("IsDoNotIncludeBaseUOM") = IIf(String.IsNullOrEmpty(subg("IsDoNotIncludeBaseUOM")), "NULL", subg("IsDoNotIncludeBaseUOM"))

                Common.WebRequest.ProData.Execute(p.GetInsert())
            End If
        Next
    End Function

    Function SaveFowstBinProduct(ByVal FowstID As String, ByVal binid As String, ByVal motors As Object)

    End Function

    Function RemoveFOWSTHB()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pIU As New InsertUpdate("MobizLibraryOrganizationFOWST")
            pIU("DefaultHotButtonTemplateID") = "NULL"
            Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))))

            Return JsonDictionary.Serialize("Result", "removed")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function NewSaveSKU(ByVal ctr As Integer, ByVal data As Object, ByVal menuid As String, ByVal groupid As String, ByRef errMsg As String) As Boolean
        Try
            Dim pIU As New InsertUpdate("LibraryPimGroupPims")
            Dim pSql As String

            If groupid = "" Then
                pIU("GroupID") = "NULL"
            Else

                pIU("GroupID") = ReadyString(groupid.ToString())
            End If

            pIU("ProductID") = ReadyString(data("ProductID").ToString())
            pIU("ShortName") = ReadyString(data("ShortName"))
            pIU("MenuID") = ReadyString(menuid)
            pIU("IsActive") = CDec2(data("IsActive"))

            If data.ContainsKey("IsUseUOM") Then
                pIU("IsUseUOM") = ReadyString(CInt2(data("IsUseUOM")))
            End If

            If data.ContainsKey("RecommendedSKU") Then
                pIU("RecommendedSKU") = ReadyString(data("RecommendedSKU"))
            End If

            If data.ContainsKey("SellingPreference") Then
                pIU("SellingPreference") = ReadyString(data("SellingPreference"))
            End If

            If data.ContainsKey("IsDoNotIncludeBaseUOM") Then
                pIU("IsDoNotIncludeBaseUOM") = ReadyString(CInt2(data("IsDoNotIncludeBaseUOM")))
            End If

            If data.ContainsKey("VDMMotorNo") Then
                pIU("VDMMotorNo") = ReadyString(CStr2(data("VDMMotorNo")))
            End If

            If data.ContainsKey("VDMBinNo") Then
                pIU("VDMBinNo") = ReadyString(CStr2(data("VDMBinNo")))
            End If

            pIU("OrderNo") = IIf(CInt2(data("OrderNo")) = 0, ctr, CInt2(data("OrderNo")))

            If data("State").ToString().ToLower() = "new" Then
                pIU("ID") = ReadyString(data("ID").ToString())
                pSql = pIU.GetInsert()
                Common.WebRequest.ProData.Execute(pSql)
            ElseIf data("State") = "edit" Then
                pSql = pIU.GetUpdate(" ID = " & ReadyString(data("ID").ToString()))
                Common.WebRequest.ProData.Execute(pSql)
            Else
                pSql = "DELETE FROM LibraryPimGroupPims WHERE ID = " & ReadyString(data("ID").ToString())
                Common.WebRequest.ProData.Execute(pSql)
            End If

            Common.WebRequest.ProData.Execute("DELETE FROM LibraryPimMenuButtonAssignedStores WHERE HotButtonID = " & ReadyString(data("ID").ToString()))

            If data("State") <> "remove" Then
                For Each store As Object In data("StoreAssignment")
                    pIU = New InsertUpdate("LibraryPimMenuButtonAssignedStores")
                    pIU("ID") = "NEWID()"
                    pIU("HotButtonID") = ReadyString(data("ID").ToString())
                    pIU("FowstID") = ReadyString(store("ID").ToString())
                    pIU("IsGroup") = 0
                    Common.WebRequest.ProData.Execute(pIU.GetInsert())
                Next store
            End If

            If data.ContainsKey("VDMMotorNo") And data.ContainsKey("VDMBinNo") Then
                If String.IsNullOrEmpty(CStr2(data("VDMMotorNo"))) And String.IsNullOrEmpty(CStr2(data("VDMBinNo"))) And data("State") <> "remove" Then
                    For Each store As Object In Common.WebRequest.AllData("AssignedFowst")
                        Dim colName As String
                        Dim binname As String

                        If Not data.ContainsKey("CurVDMMotorNo") And Not data.ContainsKey("CurVDMBinNo") Then
                            Return True
                        End If

                        If Not String.IsNullOrWhiteSpace(CStr2(data("CurVDMMotorNo"))) Then
                            colName = "CurVDMMotorNo"
                            binname = "Motor No. "
                        Else
                            colName = "CurVDMBinNo"
                            binname = "Bin No. "
                        End If

                        Dim motor() As String = data(colName).ToString().Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)

                        If motor.Length > 0 Then
                            Dim updatedNames As String() = motor.Select(Function(name) binname & name).ToArray()
                            'Deactivate motor na hindi na ginamit ng hot Button
                            Dim p = New InsertUpdate("LibraryFowstBin")
                            p("IsActive") = 0
                            p("LastUpdated") = "getdate()"
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID IN (SELECT BinID FROM LibraryFowstBinProduct " & vbCrLf &
                                                    " WHERE ProductID = " & ReadyString(data("ProductID").ToString()) & vbCrLf &
                                                    " AND FOWSTID = " & ReadyString(store("ID")) & vbCrLf &
                                                    " AND BinID IN (SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(store("ID")) & vbCrLf &
                                                    " AND BinNo IN (" & String.Join(", ", motor) & ")" & vbCrLf &
                                                    " AND Name IN ('" & String.Join("', '", updatedNames) & "')))"))

                            p = New InsertUpdate("LibraryFowstBinProduct")
                            p("IsActive") = 0
                            p("IsDefault") = 0
                            p("Balance") = 0
                            p("LastUpdated") = "getdate()"
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID IN (SELECT ID FROM LibraryFowstBinProduct " & vbCrLf &
                                                    " WHERE ProductID = " & ReadyString(data("ProductID").ToString()) & vbCrLf &
                                                    " And FOWSTID = " & ReadyString(store("ID")) & vbCrLf &
                                                    " And BinID IN (SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(store("ID")) & vbCrLf &
                                                    " AND BinNo IN (" & String.Join(", ", motor) & ")" & vbCrLf &
                                                    " AND Name IN ('" & String.Join("', '", updatedNames) & "')))"))
                        End If
                    Next
                End If
            End If

            Return True
        Catch ex As Exception
            AppendToLog(ex.Message)
            errMsg = ex.Message
            Return False
        End Try
    End Function

    Function NewSaveGroup(ByVal ctr As Integer, ByVal data As Object, ByVal menuid As String, ByVal ParentID As String, ByRef errMsg As String) As Boolean
        Try
            Dim pIU As New InsertUpdate("LibraryPimGroups")
            Dim pSql As String
            Dim groupid As String = data("ID").ToString()

            pIU("Name") = ReadyString(data("Name"))
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("IsActive") = CDec(data("IsActive"))
            pIU("MenuID") = ReadyString(menuid)
            pIU("OrderNo") = IIf(CInt2(data("OrderNo")) = 0, ctr, CInt2(data("OrderNo")))
            pIU("DefaultOtherPaymentID") = IIf(CStr2(data("DefaultOtherPaymentID")) = "", "NULL", ReadyString(CStr2(data("DefaultOtherPaymentID"))))

            If ParentID = "" Then
                pIU("ParentID") = "NULL"
            Else
                pIU("ParentID") = ReadyString(ParentID.ToString())
            End If

            If data("State") <> "remove" Then
                'If data("SubGroup").Length > 0 Then
                If data("State").ToString().ToLower() = "new" Then
                    pIU("ID") = ReadyString(groupid)
                    pSql = pIU.GetInsert()
                ElseIf data("State") = "edit" Then
                    pSql = pIU.GetUpdate(" ID = " & ReadyString(groupid))
                End If

                Common.WebRequest.ProData.Execute(pSql)
                'End If

                Dim ctr2 As Integer = 1
                For Each subgroup As Object In data("SubGroup")
                    If CBool2(subgroup("IsGroup")) Then
                        If Not NewSaveGroup(ctr2, subgroup, menuid, groupid, errMsg) Then
                            Return JsonDictionary.Serialize("Results", errMsg)
                        End If
                    Else
                        If Not NewSaveSKU(ctr2, subgroup, menuid, groupid, errMsg) Then
                            Return JsonDictionary.Serialize("Results", errMsg)
                        End If
                    End If
                    ctr2 += 1
                Next subgroup
            End If

            If data("State") = "remove" Then
                pSql = "DELETE FROM LibraryPimGroups WHERE ID = " & ReadyString(groupid)
                Common.WebRequest.ProData.Execute(pSql)
                pSql = "DELETE FROM LibraryPimGroupPims WHERE GroupID = " & ReadyString(groupid)
                Common.WebRequest.ProData.Execute(pSql)
            End If

            Common.WebRequest.ProData.Execute("DELETE FROM LibraryPimMenuButtonAssignedStores WHERE HotButtonID = " & ReadyString(groupid))

            If data("State") <> "remove" Then
                For Each store As Object In data("StoreAssignment")
                    pIU = New InsertUpdate("LibraryPimMenuButtonAssignedStores")
                    pIU("ID") = "NEWID()"
                    pIU("HotButtonID") = ReadyString(groupid)
                    pIU("FowstID") = ReadyString(store("ID").ToString())
                    pIU("IsGroup") = 1
                    Common.WebRequest.ProData.Execute(pIU.GetInsert())
                Next store
            End If

            Return True
        Catch ex As Exception
            AppendToLog(ex.Message)
            errMsg = ex.Message
            Return False
        End Try
    End Function

    <HttpPost()>
    Function GetPimMenu() As String
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pSql As String

        If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
            pSql = "SELECT pim.ID" &
            ",pim.Name, pim.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname" & vbCrLf &
            " FROM LibraryPimMenu pim INNER JOIN MobizLibraryPeople ppl" & vbCrLf &
            " ON pim.UserID = ppl.ID WHERE pim.IsActive = 1"
        Else
            pSql = "SELECT pim.ID" &
            ",pim.Name, pim.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname" & vbCrLf &
            " FROM LibraryPimMenu pim INNER JOIN MobizLibraryPeople ppl" & vbCrLf &
            " ON pim.UserID = ppl.ID WHERE pim.IsActive = 1 And pim.Name Like " & ReadyStringWild(Common.WebRequest.Data("SearchKeyword"))
        End If

        pSql &= " ORDER BY pim.Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        strReturn = JsonDictionary.Serialize("PimMenuData", pDT)

        Return strReturn

    End Function

    <HttpPost()>
    Public Function CreateTemplate() As String
        Dim name As String
        Dim VDMID As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            name = Common.WebRequest.Data("Name")
            VDMID = Common.WebRequest.Data("VDMID")

            If Common.WebRequest.Data("ID") Is Nothing Then
                pSql = "SELECT ID, Name" & vbCrLf &
                " FROM LibraryPimMenu" &
                " WHERE IsActive = 1 And Name =" & ReadyString(name)
            Else

                pSql = "SELECT ID, Name" & vbCrLf &
                " FROM LibraryPimMenu" &
                " WHERE IsActive = 1 And ID <> " & ReadyString(Common.WebRequest.Data("ID")) & " And Name = " & ReadyString(name)
            End If

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then
                Return JsonDictionary.Serialize("Error", "There is already an existing menu with the same name. Please choose a different name.")
            End If

            pOut = New JsonDictionary
            pIU = New InsertUpdate("LibraryPimMenu")
            pIU("Name") = ReadyString(name)
            pIU("VDMModelID") = IIf(String.IsNullOrWhiteSpace(VDMID), "NULL", ReadyString(VDMID))
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("IsActive") = 1
            pIU("LastUpdated") = "getdate()"

            If Common.WebRequest.Data("ID") Is Nothing Then
                pID = Guid.NewGuid.ToString
                pIU("ID") = ReadyString(pID)
                pSql = pIU.GetInsert
            Else
                pSql = pIU.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID")))
            End If


            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        Catch ex As System.Exception

            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function


    <HttpPost()>
    Public Function SaveGroup() As String

        Dim pID As String ' optional
        Dim pName As String ' required
        Dim pMenuID As String ' required
        Dim pActive As String ' required 1 or 0
        Dim pParentID As String ' optional

        Try
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pID = Common.WebRequest.Data("ID")
            If CStr2(pID).Trim = "" Then
                ' ok lang
            ElseIf IsGuid(pID) Then
                ' ok lang
            Else
                Return JsonDictionary.Serialize("Error", "Invalid data for ID")
            End If

            pName = Common.WebRequest.Data("Name")
            If CStr2(pName).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "Name is required")
            End If
            pName = pName.Trim

            pMenuID = Common.WebRequest.Data("MenuID")
            If CStr2(pMenuID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "MenuID is required")
            End If

            pActive = Common.WebRequest.Data("Active")
            If CStr2(pActive).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "Active is required")
            End If
            If Not (pActive = "0" OrElse pActive = "1") Then
                Return JsonDictionary.Serialize("Error", "Invalid data for Active")
            End If

            pParentID = Common.WebRequest.Data("ParentID")
            If CStr2(pParentID).Trim = "" Then
                ' ok lang
            ElseIf IsGuid(pParentID) Then
                ' ok lang
            Else
                Return JsonDictionary.Serialize("Error", "Invalid data for ParentID")
            End If
            If IsExist(pID, pParentID, pMenuID, pName) Then
                Return JsonDictionary.Serialize("Error", "There is already an existing group with the same name. Please choose a different name.")
            End If

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pOut As JsonDictionary

            pOut = New JsonDictionary
            pIU = New InsertUpdate("LibraryPimGroups")
            pIU("Name") = ReadyString(pName)
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("IsActive") = CDec(pActive)
            pIU("MenuID") = ReadyString(pMenuID)

            If CStr2(pParentID) = "" Then
                pIU("ParentID") = "null"
            Else
                pOut.Add("ParentID", pParentID)
                pIU("ParentID") = ReadyString(pParentID)
            End If
            If CStr2(pID) = "" Then
                pID = Guid.NewGuid.ToString
                pIU("ID") = ReadyString(pID)
                pSql = pIU.GetInsert
            Else
                pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
            End If
            Common.WebRequest.ProData.BeginTransaction()


            Common.WebRequest.ProData.ExecuteSQL(pSql)


            pSql = "Update LibraryPimMenu Set LastUpdated = GetDate() Where ID =" & ReadyString(pMenuID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()


            pOut.Add("ID", pID)
            pOut.Add("Result", "ok")
            Return pOut.Serialize()
        Catch ex As System.Exception
            Common.WebRequest.ProData.RollbackTransaction()

            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function DeleteItem() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim id As String
        Dim pIU As InsertUpdate

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try

            'get selected id
            id = Common.WebRequest.Data("ID")

            Common.WebRequest.ProData.BeginTransaction()

            'if youre deleting a group, do this
            If Common.WebRequest.Data("Item") = "group" Then
                'check if the id is a parent 
                DeleteChildren(id.ToString())

            ElseIf Common.WebRequest.Data("Item") = "template" Then
                'if delete template, delete everything under it
                pSql = "Update LibraryPimMenu Set LastUpdated = GetDate() WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryPimGroupPims WHERE MenuID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryPimGroups  WHERE MenuID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryPimMenu WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)


            Else
                'if delete SKU single record

                pSql = "SELECT MenuID FROM LibraryPimGroupPims WHERE ID = " & ReadyString(id)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pSql = "Update LibraryPimMenu Set LastUpdated = GetDate() Where ID =" & ReadyString(pDT.Rows(0).Item("MenuID").ToString())
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryPimGroupPims WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)


            End If



            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function


    Private Sub DeleteChildren(ByVal id As String)

        Dim pSql As String
        Dim pDT As DataTable
        Dim i As Int16 = 0
        Dim count As Int16 = 0

        Try
            If isDone Then
                pSql = "SELECT MenuID FROM LibraryPimGroups WHERE ID = " & ReadyString(id)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pSql = "Update LibraryPimMenu Set LastUpdated = GetDate() Where ID =" & ReadyString(pDT.Rows(0).Item("MenuID").ToString())
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryPimGroupPims WHERE GroupID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryPimGroups WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                'delete the id in pim groups

                Return

            Else

                pSql = "SELECT ID" & vbCrLf &
               " FROM LibraryPimGroups" &
               " WHERE ParentID = " & ReadyString(id)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                'if it is a parent
                If (pDT.Rows.Count > 0) Then

                    If firstLevelID.Length = 0 Then
                        firstLevelID = id '1
                    End If

                    While i < pDT.Rows.Count
                        DeleteFunctionParent(pDT.Rows(i).Item("ID").ToString()) '1a

                        i = i + 1
                    End While

                    isDone = True
                    DeleteChildren(firstLevelID)
                Else

                    pSql = "SELECT MenuID FROM LibraryPimGroups WHERE ID = " & ReadyString(id)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    pSql = "Update LibraryPimMenu Set LastUpdated = GetDate() Where ID =" & ReadyString(pDT.Rows(0).Item("MenuID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "DELETE FROM LibraryPimGroupPims WHERE GroupID = " & ReadyString(id)
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    'delete the id in pim groups
                    pSql = "DELETE FROM LibraryPimGroups WHERE ID = " & ReadyString(id)
                    Common.WebRequest.ProData.ExecuteSQL(pSql)


                End If


            End If


        Catch ex As Exception

        End Try

    End Sub

    Private Sub DeleteFunction(ByVal id As String)
        Dim pSql As String
        Dim pDT As DataTable

        Try


            pSql = "DELETE FROM LibraryPimGroupPims WHERE GroupID = " & ReadyString(id)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            'delete the id in pim groups
            pSql = "DELETE FROM LibraryPimGroups WHERE ID = " & ReadyString(id)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


        Catch ex As Exception

        End Try
    End Sub


    Private Sub DeleteFunctionParent(ByVal id As String)
        Dim pSql As String
        Dim pDT As DataTable
        Dim i As Int16 = 0

        pSql = "SELECT ID" & vbCrLf &
       " FROM LibraryPimGroups" &
       " WHERE ParentID = " & ReadyString(id)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        'if it is a parent
        If (pDT.Rows.Count > 0) Then
            While i < pDT.Rows.Count
                DeleteFunction(pDT.Rows(i).Item("ID").ToString())

                i = i + 1
            End While

        Else


            Try


                pSql = "DELETE FROM LibraryPimGroupPims WHERE GroupID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                'delete the id in pim groups
                pSql = "DELETE FROM LibraryPimGroups WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)


            Catch ex As Exception

            End Try
        End If

    End Sub

    Private Function IsExist(ByVal vID As String, ByVal vParentID As String, ByVal vMenuID As String, ByVal vName As String, Optional ByVal IsFromGenHB As Boolean = False) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        If vName = "SKU" Then
            pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryPimGroupPims" &
            " WHERE IsActive = 1 AND ProductID = " & ReadyString(vID) & " AND MenuID =" & ReadyString(vMenuID) & vbCrLf
            If CStr2(vParentID) <> "" Then
                pSql &= " AND GroupID = " & ReadyString(vParentID) & vbCrLf
            Else
                pSql &= " AND GroupID IS NULL"
            End If

        Else
            pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryPimGroups" &
            " WHERE Name = " & ReadyString(vName) & vbCrLf &
            " AND IsActive = 1 AND MenuID =" & ReadyString(vMenuID) & vbCrLf
            If CStr2(vID) <> "" Then
                pSql &= " AND ID <> " & ReadyString(vID)
            End If
            If CStr2(vParentID) <> "" Then
                pSql &= " AND ParentID = " & ReadyString(vParentID) & vbCrLf
            Else
                pSql &= " AND ParentID IS NULL"
            End If
        End If

        pDT = Common.WebRequest.ProData.Execute(pSql)

        If IsFromGenHB Then
            If pDT.Rows.Count > 0 Then
                existingGroupID = pDT.Rows(0)("ID").ToString()
                Return True
            Else
                existingGroupID = ""
                Return False
            End If
        Else
            Return pDT.Rows.Count > 0
        End If

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pID As String
        Dim pDT As DataTable
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
        'If pID <> "" Then
        '    pOut.Add("Data", GetDataRow(pID))
        '    pOut.Add("Products", GetProducts(Common.WebRequest.Data("MenuID"), pID))
        'End If
        pOut.Add("Data", GetButtons(Common.WebRequest.Data("MenuID"), pID, Common.WebRequest.Data("PriceLevelID"), Common.WebRequest.Data("WithContentsOnly")))

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function GetTree() As String

        Dim pID As String
        Dim pDT As DataTable
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

        Dim menuID As String = Common.WebRequest.Data("MenuID")

        AttachBranch(menuID, pID, pOut)

        Return pOut.Serialize

    End Function

    Private Sub AttachBranch(ByVal menuID As String, ByVal vID As String, ByRef rJson As JsonDictionary)

        Dim pDT As DataTable
        Dim pJson As JsonDictionary

        If vID <> "" Then
            rJson.Add("Data", GetDataRow(vID))
            pDT = GetProducts(menuID, vID)
            If pDT.Rows.Count > 0 Then
                rJson.Add("Products", pDT)
            End If
        Else
            pDT = GetProductsMain(menuID)
            If pDT.Rows.Count > 0 Then
                rJson.Add("Products", pDT)
            End If
        End If

        pDT = GetSubgroups(menuID, vID)
        If pDT.Rows.Count > 0 Then
            With rJson.AddList("Subgroups")
                For Each pDR As DataRow In pDT.Rows
                    pJson = .AddDictionary()
                    AttachBranch(menuID, CStr2(pDR("ID")), pJson)
                Next pDR
            End With
        End If

    End Sub

    Private Function GetDataRow(ByVal vID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            ",Name" & vbCrLf &
            ",IsActive" & vbCrLf &
            " FROM LibraryPimGroups" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Function GetButtons(ByVal menuID As String, ByVal vID As String, ByVal vPriceLevelID As String, ByVal WithContentsOnly As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        'to make sure that there is a price level. default to retail price
        If vPriceLevelID Is Nothing Then
            vPriceLevelID = ReadyGuid(WebRequest.Instance.Business.SystemDefaults.GetRetailPriceID)
        End If

        'limitation, can only get upto first measurement level
        If CStr2(vID) = "" Then
            pSql = "SELECT DISTINCT ID as ID, ID as ProductID, Name as ProductName" &
          ",Name as Name, 'Group' as table_name, '1' as Quantity, '' as UnitMeasure, '1' as DiscountedPrice, ID as UnitMeasureID " & vbCrLf &
          " FROM LibraryPimGroups " & vbCrLf &
          " WHERE IsActive = 1 AND MenuID= " & ReadyString(menuID) & vbCrLf &
          " AND ParentID IS NULL" & vbCrLf &
          " UNION SELECT DISTINCT LibraryPimGroupPims.ID as ID, LibraryPimGroupPims.ProductID as ProductID, MobizLibraryPIMProduct.ProductName as ProductName" &
          ",LibraryPimGroupPims.ShortName as Name, 'SKU' as table_name, MobizLibraryPIMProductMeasurement.Quantity as Quantity,MobizLibraryCountriesLanguageLabel.Label as UnitMeasure,  MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price - (MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (MobizLibraryPIMProductMeasurement.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount as DiscountedPrice, MobizLibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID" & vbCrLf &
          " FROM LibraryPimGroupPims LEFT JOIN MobizLibraryPIMProduct ON LibraryPimGroupPims.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductMeasurement ON LibraryPimGroupPims.ProductID = MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductPriceLevel ON LibraryPimGroupPims.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID"
            If (CBool2(WithContentsOnly)) Then
                pSql &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If
            pSql &= " WHERE LibraryPimGroupPims.MenuID= " & ReadyString(menuID) & " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(vPriceLevelID) & vbCrLf &
             " AND MobizLibraryPIMProductPriceLevel.OrganizationID= " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND LibraryPimGroupPims.GroupID IS NULL AND MobizLibraryPIMProductMeasurement.MeasurementLevel = 1 AND LibraryPimGroupPims.IsActive = 1" & vbCrLf

        Else
            pSql = "SELECT DISTINCT ID as ID, ID as ProductID, Name as ProductName" &
          ",Name as Name, 'Group' as table_name, '1' as Quantity, '' as UnitMeasure, '1' as DiscountedPrice, ID as UnitMeasureID " & vbCrLf &
          " FROM LibraryPimGroups" & vbCrLf &
          " WHERE IsActive = 1 AND MenuID= " & ReadyString(menuID) & vbCrLf &
          " AND ParentID = " & ReadyString(vID) & vbCrLf &
          " UNION SELECT DISTINCT LibraryPimGroupPims.ID as ID, LibraryPimGroupPims.ProductID as ProductID, MobizLibraryPIMProduct.ProductName as ProductName" &
          ",LibraryPimGroupPims.ShortName as Name, 'SKU' as table_name, MobizLibraryPIMProductMeasurement.Quantity as Quantity, MobizLibraryCountriesLanguageLabel.Label as UnitMeasure, MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price - (MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (MobizLibraryPIMProductMeasurement.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount as DiscountedPrice, MobizLibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID" & vbCrLf &
          " FROM LibraryPimGroupPims LEFT JOIN MobizLibraryPIMProduct ON LibraryPimGroupPims.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductMeasurement ON LibraryPimGroupPims.ProductID = MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductPriceLevel ON LibraryPimGroupPims.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID"
            If (CBool2(WithContentsOnly)) Then
                pSql &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If
            pSql &= " WHERE LibraryPimGroupPims.MenuID= " & ReadyString(menuID) & " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(vPriceLevelID) & vbCrLf &
           " AND MobizLibraryPIMProductPriceLevel.OrganizationID= " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND LibraryPimGroupPims.GroupID =" & ReadyString(vID) & " AND MobizLibraryPIMProductMeasurement.MeasurementLevel = 1 AND LibraryPimGroupPims.IsActive = 1" & vbCrLf

        End If


        pSql &= " ORDER BY table_name, Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    <HttpPost()>
    Public Function GetGroupChildren() As String

        Dim pDT As DataTable
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        'to make sure that there is a price level. default to retail price
        Dim priceLevel = Common.WebRequest.Data("PriceLevelID")
        If priceLevel Is Nothing Then
            priceLevel = "01000012-0005-0000-0000-000000000000"
        End If

        If CStr2(Common.WebRequest.Data("ID")) = "" Then
            Return JsonDictionary.Serialize("Error", "Invalid Group ID")
        Else
            pSql = "SELECT DISTINCT ID as ID, '0' as AllowDecimal, ID as ProductID, Name as ProductName" &
          ",Name as Name, 'Group' as table_name, '1' as Quantity, '' as UnitMeasure, '1' as DiscountedPrice, ID as UnitMeasureID " & vbCrLf &
          " FROM LibraryPimGroups" & vbCrLf &
          " WHERE IsActive = 1 AND ParentID= " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
          " UNION SELECT sku.ID as ID, orgpim.AllowDecimal as AllowDecimal, sku.ProductID as ProductID, pim.ProductName as ProductName" &
          ",sku.ShortName as Name, 'SKU' as table_name, meas.Quantity as Quantity,MobizLibraryCountriesLanguageLabel.Label as UnitMeasure, meas.Quantity * MobizLibraryPIMProductPriceLevel.Price - (meas.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (meas.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount as DiscountedPrice, meas.UnitMeasureID as UnitMeasureID" & vbCrLf &
          " FROM LibraryPimGroupPims sku LEFT JOIN MobizLibraryPIMProduct pim ON sku.ProductID = pim.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductMeasurement meas ON sku.ProductID = meas.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductPriceLevel ON sku.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMMeasure ON meas.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
          " LEFT JOIN LibraryOrganizationPIM orgpim ON pim.ID = orgpim.ProductID"
            If (CBool2(Common.WebRequest.Data("WithContentsOnly"))) Then
                pSql &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = pim.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If

            pSql &= " WHERE MobizLibraryPIMProductPriceLevel.OrganizationID= " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND sku.GroupID= " & ReadyString(Common.WebRequest.Data("ID")) & " AND meas.MeasurementLevel = 1" & " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(priceLevel) & " AND sku.IsActive = 1" & vbCrLf
        End If

        pSql &= " ORDER BY table_name, Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Data", pDT)

    End Function

    Private Function GetSubgroups(ByVal menuID As String, ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ID" &
            ",Name" & vbCrLf &
            " FROM LibraryPimGroups" & vbCrLf &
            " WHERE IsActive = 1 AND MenuID= " & ReadyString(menuID) & vbCrLf
        If CStr2(vID) = "" Then
            pSql &= " AND ParentID IS NULL" & vbCrLf
        Else
            pSql &= " AND ParentID = " & ReadyString(vID) & vbCrLf
        End If
        pSql &= "ORDER BY Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetProducts(ByVal menuID As String, ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryPimGroupPims.ID, LibraryPimGroupPims.ProductID, LibraryPimGroupPims.GroupID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName ,LibraryPimGroupPims.ShortName" & vbCrLf &
            " FROM LibraryPimGroupPims" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryPimGroupPims.ProductID" & vbCrLf &
            " WHERE GroupID = " & ReadyString(vID) & " AND LibraryPimGroupPims.IsActive = 1 AND LibraryPimGroupPims.MenuID= " & ReadyString(menuID) & vbCrLf &
            " ORDER BY LibraryPimGroupPims.ShortName"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetProductsMain(ByVal menuID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryPimGroupPims.ID, LibraryPimGroupPims.ProductID, LibraryPimGroupPims.GroupID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName,LibraryPimGroupPims.ShortName" & vbCrLf &
            " FROM LibraryPimGroupPims" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryPimGroupPims.ProductID" & vbCrLf &
            " WHERE LibraryPimGroupPims.MenuID= " & ReadyString(menuID) & " AND LibraryPimGroupPims.IsActive = 1 AND LibraryPimGroupPims.GroupID IS NULL " & vbCrLf &
            " ORDER BY LibraryPimGroupPims.ShortName"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    <HttpPost()>
    Public Function SaveProduct() As String

        Dim pID As String
        Dim pProductID As String
        Dim pMenuID As String
        Dim pGroupID As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim newID As String

        Try
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pID = Common.WebRequest.Data("GroupID")
            If CStr2(pID).Trim = "" Then
                'Return JsonDictionary.Serialize("Error", "ID is required")
            ElseIf IsGuid(pID) Then
                ' ok lang
            Else
                Return JsonDictionary.Serialize("Error", "Invalid data for ID")
            End If

            pProductID = Common.WebRequest.Data("ProductID")
            If CStr2(pProductID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "ProductID is required")
            ElseIf IsGuid(pProductID) Then
                ' ok lang
            Else
                Return JsonDictionary.Serialize("Error", "Invalid data for ProductID")
            End If

            pMenuID = Common.WebRequest.Data("MenuID")
            If CStr2(pMenuID).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "MenuID is required")
            ElseIf IsGuid(pMenuID) Then
                ' ok lang
            Else
                Return JsonDictionary.Serialize("Error", "Invalid data for pMenuID")
            End If

            Dim pName As String = "SKU"

            If IsExist(pProductID, pID, pMenuID, pName) Then
                Return JsonDictionary.Serialize("Error", "The SKU you selected has already been added. Please select a different SKU.")
            End If


            pIU = New InsertUpdate("LibraryPimGroupPims")
            newID = Guid.NewGuid.ToString()
            pIU("ID") = ReadyString(newID)
            If (pID <> Nothing) Then
                pIU("GroupID") = ReadyString(pID)
            End If

            pIU("ProductID") = ReadyString(pProductID)
            pIU("MenuID") = ReadyString(pMenuID)
            pIU("IsActive") = 1
            pSql = pIU.GetInsert

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update LibraryPimMenu Set LastUpdated = GetDate() Where ID =" & ReadyString(pMenuID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPimMenu'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            'return ID
            Return JsonDictionary.Serialize("SKUData", newID)
        Catch ex As System.Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function SaveProductName() As String

        Dim pID As String
        Dim name As String
        Dim pMenuID As String
        Dim pGroupID As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            name = Common.WebRequest.Data("ShortName")
            If CStr2(name).Trim = "" Then
                Return JsonDictionary.Serialize("Error", "Short Name is required")
            End If

            If Common.WebRequest.Data("ID").Trim = "" Then
                Return JsonDictionary.Serialize("Error", "ID is required")
            End If

            Dim pDT As DataTable
            If Common.WebRequest.Data("GroupID").Trim() = "" Then
                pSql = "SELECT ID, ShortName" & vbCrLf &
                " FROM LibraryPimGroupPims" & vbCrLf &
                " WHERE IsActive = 1 AND MenuID = " & ReadyString(Common.WebRequest.Data("MenuID")) & " AND GroupID IS NULL AND ShortName= " & ReadyString(name)
            Else
                pSql = "SELECT ID, ShortName" & vbCrLf &
                " FROM LibraryPimGroupPims" & vbCrLf &
                " WHERE IsActive = 1 AND MenuID = " & ReadyString(Common.WebRequest.Data("MenuID")) & " AND GroupID = " & ReadyString(Common.WebRequest.Data("GroupID")) & " AND ShortName= " & ReadyString(name)
            End If

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Error", "There is already an existing SKU with the same short name. Please choose a different name.")
            End If

            pIU = New InsertUpdate("LibraryPimGroupPims")

            pIU("ShortName") = ReadyString(name)
            pSql = pIU.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("ID")))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As System.Exception

            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function


    <HttpPost()>
    Function GetMenuTemplateName() As String
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pSql As String
        Dim pOut As New JsonDictionary

        Try
            pSql = "SELECT LibraryPimMenu.Name, VDM.ID as VDMModelID, VDM.Name as VDMModelName, VDM.NoOfBins as VDMNoOfBins, IsDisplayAllProducts, " & vbCrLf &
                " (SELECT SUM(NoOfDirectConnMotors) + SUM(NoOfMotors) " & vbCrLf &
                "   FROM (SELECT NoOfDirectConnMotors, NoOfMotors FROM LibraryVendingMachineBoards WHERE ID = machine.BoardID " & vbCrLf &
                "   UNION ALL " & vbCrLf &
                "   SELECT NoOfDirectConnMotors, NoOfMotors FROM LibraryVendingMachineBoards " & vbCrLf &
                "   WHERE ID IN (SELECT BoardID FROM LibraryVendingMachineSubBoard WHERE MachineID =  " & vbCrLf &
                "   (SELECT ID FROM LibraryVendingMachines WHERE BoardID = machine.BoardID) " & vbCrLf &
                "   )) as t) as VDMNoOfMotors " & vbCrLf &
                " FROM LibraryPimMenu " & vbCrLf &
                " LEFT JOIN LibraryVendingMachines as machine ON machine.ID = VDMModelID " & vbCrLf &
                " LEFT JOIN LibraryVendingMachineBoards as VDM ON VDM.ID = machine.BoardID " & vbCrLf &
                " WHERE LibraryPimMenu.IsActive = 1 AND LibraryPimMenu.ID = " & ReadyString(Common.WebRequest.Data("ID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)
            pOut.Add("Result", pDT)

            'return template name
            strReturn = JsonDictionary.Serialize("Result", pDT)

            pSql = "SELECT ProductID, ISNULL(VDMMotorNo, '') as VDMMotorNo, ISNULL(VDMBinNo, '') as VDMBinNo, groups.Name + ' > ' + pim.ProductName as Path " & vbCrLf &
                    " , groups.Name as GroupName, pim.ProductName " & vbCrLf &
                    " FROM LibraryPimGroupPims as grppim " & vbCrLf &
                    " INNER JOIN LibraryPimGroups as groups ON groups.ID = grppim.GroupID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = grppim.ProductID " & vbCrLf &
                    " WHERE /*ISNULL(VDMMotorNo, '') <> 0 AND*/ grppim.MenuID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
                    " ORDER BY groups.OrderNo, pim.ProductName"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("ArrMotorAssignment", pDT)
            Return pOut.Serialize()

        Catch ex As Exception
            strReturn = JsonDictionary.Serialize("Error", ex.Message)

        End Try

    End Function


    <HttpPost()>
    Function GetMenuFOWST() As String
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pSql As String

        Try

            If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
                pSql = "SELECT LibraryPimGroupFowsts.FowstID, MobizLibraryOrganizationFOWST.Name" & vbCrLf &
              " FROM LibraryPimGroupFowsts INNER JOIN MobizLibraryOrganizationFOWST ON LibraryPimGroupFowsts.FowstID = MobizLibraryOrganizationFOWST.ID WHERE LibraryPimGroupFowsts.MenuID = " & ReadyString(Common.WebRequest.Data("MenuID")) & " "

            Else
                pSql = "SELECT LibraryPimGroupFowsts.FowstID, MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " FROM LibraryPimGroupFowsts INNER JOIN MobizLibraryOrganizationFOWST ON LibraryPimGroupFowsts.FowstID = MobizLibraryOrganizationFOWST.ID WHERE LibraryPimGroupFowsts.MenuID = " & ReadyString(Common.WebRequest.Data("MenuID")) & " AND MobizLibraryOrganizationFOWST.Name LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKeyword"))
            End If

            pSql &= " ORDER BY MobizLibraryOrganizationFOWST.Name"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            'return
            strReturn = JsonDictionary.Serialize("MenuFowstData", pDT)

            Return strReturn

        Catch ex As Exception
            strReturn = JsonDictionary.Serialize("Error", ex.Message)

        End Try

    End Function

    'TODO'
    <HttpPost()>
    Public Function SaveFOWST() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try

            pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryPimGroupFowsts" &
            " WHERE MenuID = " & ReadyString(Common.WebRequest.Data("MenuID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut = New JsonDictionary
            pIU = New InsertUpdate("LibraryPimGroupFowsts")
            pIU("MenuID") = ReadyString(Common.WebRequest.Data("MenuID"))
            pIU("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))

            If (pDT.Rows.Count > 0) Then
                pSql = pIU.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID")))
            Else
                pID = Guid.NewGuid.ToString
                pIU("ID") = ReadyString(pID)
                pSql = pIU.GetInsert

            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function


End Class