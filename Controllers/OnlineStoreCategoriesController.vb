Public Class OnlineStoreCategoriesController
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

    Function MenuTemplate() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function

        End If

        Return View()

    End Function

    Function AssignFOWST() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function

        End If

        Return View()

    End Function

    <HttpPost>
    Function GenerateHB() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim arrCat As Object = jss.DeserializeObject(New BrowserController().PimCategories(True))("d")("Results")("Rows")
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim pSql As String
            Dim pDT As DataTable
            Dim ctr1 As Integer = 1

            Common.WebRequest.ProData.BeginTransaction()

            For Each cat As Object In arrCat
                If cat.Count > 0 Then
                    If CStr2(cat("Name")) <> "" Then
                        Dim groupid As String
                        Dim pIU As InsertUpdate
                        Dim ctr2 As Integer = 1

                        If Not IsExist(Nothing, Nothing, menuid, CStr2(cat("Name").ToString()), True) Then

                            pIU = New InsertUpdate("LibraryStoreCategoryGroup")
                            pIU("Name") = ReadyString(cat("Name"))
                            pIU("LastUpdated") = "GETDATE()"
                            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pIU("IsActive") = CDec(1)
                            pIU("MenuID") = ReadyString(menuid)
                            pIU("OrderNo") = CInt2(ctr1)

                            groupid = Guid.NewGuid.ToString
                            pIU("ID") = ReadyString(groupid)
                            pSql = pIU.GetInsert()

                            Common.WebRequest.ProData.Execute(pSql)
                        Else
                            groupid = existingGroupID
                        End If

                        ctr1 += 1

                        pSql = "SELECT MobizLibraryPIMProduct.ID, MobizLibraryPIMProduct.ProductName, LibraryOrganizationPIM.IsDeactivated " & vbCrLf &
                                " FROM MobizLibraryPIMProduct " & vbCrLf &
                                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                                " WHERE MobizLibraryPIMProductVariableFeatures001.C0020 = " & ReadyString(cat("Name")) & vbCrLf &
                                " AND ISNULL(MobizLibraryPIMProduct.IsAsset, 0) = 0 " & vbCrLf &
                                " Order By MobizLibraryPIMProduct.ProductName ASC "

                        pDT = Common.WebRequest.ProData.Execute(pSql)

                        For Each prod As Object In pDT.Rows
                            If Not CBool2(prod("IsDeactivated")) Then
                                Dim shortname As String = prod("ProductName") 'Left(, 23)
                                Dim newshortname As String = CheckShortName(menuid, groupid, shortname, 1)
                                If newshortname <> "" Then
                                    shortname = newshortname
                                End If

                                If Not IsExist(prod("ID").ToString(), groupid, menuid, "SKU", True) Then
                                    pIU = New InsertUpdate("LibraryStoreCategoryGroupPIM")

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
                                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE GroupID = " & ReadyString(groupid) & vbCrLf &
                                        " AND ProductID = " & ReadyString(prod("ID").ToString())
                                Common.WebRequest.ProData.Execute(pSql)
                            End If

                            ctr2 += 1
                        Next prod
                    End If
                End If
            Next cat

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
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
                            " FROM LibraryStoreCategoryGroupPIM" & vbCrLf &
                            " WHERE IsActive = 1 AND MenuID = " & ReadyString(menuid) & " AND GroupID = " & ReadyString(groupid) & " AND ShortName= " & ReadyString(name)
        Dim row As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

        If row > 0 Then
            Dim newname As String = (ctrRename + 1) & " " & Left(name, 21)
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
            Dim p As New InsertUpdate("LibraryStoreCategoryGroup")
            Dim NewName As String = Common.WebRequest.Data("Name")
            Dim ID As String = Common.WebRequest.Data("ID")
            Dim Type As String = Common.WebRequest.Data("Type")

            If Type = "sku" Then
                p = New InsertUpdate("LibraryStoreCategoryGroupPIM")
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

    Function NewGetHotButtons(Optional ByVal tempcatid As String = "", Optional ByVal IncludeSubGroup As Boolean = True) As Object
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim menuid As String = IIf(tempcatid = "", Common.WebRequest.Data("MenuID"), tempcatid)
            Dim firstLevel As Object = GetHB(menuid, "").Rows
            Dim isIncludeSubGroup = IIf(Common.WebRequest.Data("IncludeSubGroup") = Nothing, IncludeSubGroup, Common.WebRequest.Data("IncludeSubGroup"))
            Dim arrHB As New List(Of Dictionary(Of String, Object))
            Dim arrSubGroup As New List(Of Dictionary(Of String, Object))

            For Each first As Object In firstLevel
                Dim pHB As New Dictionary(Of String, Object)
                pHB.Add("ID", first("ID"))
                pHB.Add("Name", first("Name"))
                pHB.Add("IsActive", first("IsActive"))
                pHB.Add("IsGroup", first("IsGroup"))
                pHB.Add("OrderNo", first("OrderNo"))
                pHB.Add("IsFeature", CBool2(first("IsFeature")))
                pHB.Add("State", "edit")
                pHB.Add("MenuID", menuid)

                If isIncludeSubGroup Then
                    pHB.Add("SubGroup", GetSubGroup(menuid, first("ID").ToString(), 2)) ' GetSubGroup(menuid, first("ID").ToString(), 2)
                Else
                    pHB.Add("SubGroup", New List(Of Dictionary(Of String, Object)))
                End If

                If CBool2(first("IsGroup")) Then
                    pHB.Add("Level", 1)
                Else
                    pHB.Add("ProductID", first("ProductID"))
                    pHB.Add("ShortName", first("ShortName"))
                End If
                arrHB.Add(pHB)
            Next first

            Return Json(New With {Key .Results = arrHB}, JsonRequestBehavior.AllowGet)

        Catch ex As Exception
            Return Json(New With {Key .Results = ex.Message})
        End Try
    End Function

    Function GetSubGroup(Optional ByVal menuid As String = "",
                         Optional ByVal GroupID As String = "",
                         Optional ByVal Level As Integer = Nothing,
                         Optional ByVal IncludeSubGroup As Boolean = True,
                         Optional MaxRow As Integer = Nothing) As Object
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim arrHB As New List(Of Dictionary(Of String, Object))
        Dim arrSubGroup As New List(Of Dictionary(Of String, Object))
        Dim pMenuID As String = IIf(menuid = "", Common.WebRequest.Data("MenuID"), menuid)
        Dim pGroupID As String = IIf(GroupID = "", Common.WebRequest.Data("GroupID"), GroupID)
        Dim pLevel As String = IIf(Level = Nothing, Common.WebRequest.Data("Level"), Level)
        Dim isIncludeSubGroup = IIf(Common.WebRequest.Data("IncludeSubGroup") = Nothing, IncludeSubGroup, Common.WebRequest.Data("IncludeSubGroup"))
        Dim subgroup As Object = GetHB(pMenuID, pGroupID, MaxRow).Rows

        For Each subg As Object In subgroup
            Dim pHB As New Dictionary(Of String, Object)
            pHB.Add("ID", subg("ID"))
            pHB.Add("Name", subg("Name"))
            pHB.Add("IsActive", subg("IsActive"))
            pHB.Add("IsGroup", subg("IsGroup"))
            pHB.Add("OrderNo", subg("OrderNo"))
            pHB.Add("IsFeature", CBool2(subg("IsFeature")))
            pHB.Add("MenuID", menuid)
            pHB.Add("State", "edit")

            If isIncludeSubGroup Then
                pHB.Add("SubGroup", GetSubGroup(menuid, subg("ID").ToString(), (pLevel + 1), isIncludeSubGroup, MaxRow))
            Else
                pHB.Add("SubGroup", New List(Of Dictionary(Of String, Object)))
            End If

            If CBool2(subg("IsGroup")) Then
                pHB.Add("Level", pLevel)
            Else
                pHB.Add("Price", subg("Price"))
                pHB.Add("UnitMeasure", subg("UnitMeasure"))
                pHB.Add("FileName", subg("FileName"))
                pHB.Add("FileSize", subg("FileSize"))
                pHB.Add("ProductID", subg("ProductID"))
                pHB.Add("ShortName", subg("ShortName"))
            End If
            arrHB.Add(pHB)
        Next subg

        Try
            Return arrHB
        Catch ex As Exception
            Return Json(New With {Key .Results = arrHB}, JsonRequestBehavior.AllowGet)
        End Try
    End Function

    Function GetHB(ByVal menuid As String, ByVal groupid As String, Optional MaxRow As Integer = Nothing) As DataTable
        Dim pSql As String = "SELECT * FROM ( " & vbCrLf &
                                "   SELECT gr.ID, '' as ProductID, gr.OrderNo, 'edit' as State, 1 as IsGroup, 0 as Price, " & vbCrLf &
                                "       gr.Name as Name, '' as ShortName, gr.IsActive, gr.IsFeature, '' as UnitMeasure, " & vbCrLf &
                                "       '' as FileName, '' as FileSize " & vbCrLf &
                                "       FROM LibraryStoreCategoryGroup as gr " & vbCrLf &
                                "       WHERE gr.MenuID = " & ReadyString(menuid) & vbCrLf &
                                IIf(groupid = "", " AND ISNULL(convert(varchar(max), ParentID), '') = '' ", " AND ParentID = " & ReadyString(groupid)) & vbCrLf &
                                "   UNION ALL " & vbCrLf &
                                "   SELECT " & IIf(MaxRow = Nothing, "", "TOP " & MaxRow) & " sku.ID, convert(varchar(max), product.ID) as ProductID, sku.OrderNo, " & vbCrLf &
                                "       'edit' as State, 0 as IsGroup, prc.Price," & vbCrLf &
                                "       product.ProductName as Name, " & vbCrLf &
                                "       sku.ShortName, sku.IsActive, 0 as IsFeature, um.UnitMeasure, " & vbCrLf &
                                "       (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = sku.ProductID AND Size = 'big' AND IsDefault = 1 AND IsFromVendor = 0) as FileName, " & vbCrLf &
                                "       'big' as FileSize " & vbCrLf &
                                "       FROM LibraryStoreCategoryGroupPIM as sku " & vbCrLf &
                                "       INNER JOIN MobizLibraryPIMProduct as product ON product.ID = sku.ProductID " & vbCrLf &
                                "       LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = sku.ProductID " & vbCrLf &
                                "       OUTER APPLY (SELECT TOP 1" & vbCrLf &
                                "           MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                                "           FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                                "           JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                                "           JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                                "           WHERE MobizLibraryPIMProductMeasurement.ProductID = sku.ProductID" & vbCrLf &
                                "           ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf &
                                "       WHERE MenuID = " & ReadyString(menuid) & vbCrLf &
                                "       AND prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                                "       AND prc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID.ToString()) & vbCrLf &
                                "       AND ISNULL(um.UnitMeasure, '') <> '' " & vbCrLf &
                                IIf(groupid = "", " AND ISNULL(convert(varchar(max), GroupID), '') = '' ", " AND GroupID = " & ReadyString(groupid)) & vbCrLf &
                                IIf(MaxRow = Nothing, "", " ORDER BY OrderNo asc, Name asc ") & vbCrLf &
                                ") as t ORDER BY IsGroup desc, OrderNo asc, Name asc  " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function SaveHotButtons() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim menuid As String = Common.WebRequest.Data("MenuID")
            Dim arrHB As Object = Common.WebRequest.AllData("HotButtons")
            Dim isDefault As String = Common.WebRequest.Data("IsDefault")
            Dim errMsg As String = ""
            Dim ctr As Integer = 1

            Common.WebRequest.ProData.BeginTransaction()

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

            If CBool2(isDefault) Then
                pSql = "Update LibraryStoreCategoryTemplate Set IsDefault = 0 "
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate(), UserID = " & ReadyString(WebSession.Instance.PeopleUserID()) & ", IsDefault = " & CInt2(isDefault) & " Where ID =" & ReadyString(menuid)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function NewSaveSKU(ByVal ctr As Integer, ByVal data As Object, ByVal menuid As String, ByVal groupid As String, ByRef errMsg As String) As Boolean
        Try
            Dim pIU As New InsertUpdate("LibraryStoreCategoryGroupPIM")
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
            pIU("OrderNo") = IIf(CInt2(data("OrderNo")) = 0, ctr, CInt2(data("OrderNo")))

            If data("State") = "new" Then
                pIU("ID") = ReadyString(data("ID").ToString())
                pSql = pIU.GetInsert()
            ElseIf data("State") = "edit" Then
                pSql = pIU.GetUpdate(" ID = " & ReadyString(data("ID").ToString()))
            Else
                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE ID = " & ReadyString(data("ID").ToString())
            End If

            Common.WebRequest.ProData.Execute(pSql)

            Return True
        Catch ex As Exception
            AppendToLog(ex.Message)
            errMsg = ex.Message
            Return False
        End Try
    End Function

    Function NewSaveGroup(ByVal ctr As Integer, ByVal data As Object, ByVal menuid As String, ByVal ParentID As String, ByRef errMsg As String) As Boolean
        Try
            Dim pIU As New InsertUpdate("LibraryStoreCategoryGroup")
            Dim pSql As String
            Dim groupid As String = data("ID").ToString()

            pIU("Name") = ReadyString(data("Name"))
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("IsActive") = CDec2(data("IsActive"))
            pIU("IsFeature") = CDec2(data("IsFeature"))
            pIU("MenuID") = ReadyString(menuid)
            pIU("OrderNo") = IIf(CInt2(data("OrderNo")) = 0, ctr, CInt2(data("OrderNo")))

            If ParentID = "" Then
                pIU("ParentID") = "NULL"
            Else
                pIU("ParentID") = ReadyString(ParentID.ToString())
            End If

            If data("State") = "new" Then
                pIU("ID") = ReadyString(groupid)
                pSql = pIU.GetInsert()
            ElseIf data("State") = "edit" Then
                pSql = pIU.GetUpdate(" ID = " & ReadyString(groupid))
            Else
                pSql = "DELETE FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(groupid)
                Common.WebRequest.ProData.Execute(pSql)
                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE GroupID = " & ReadyString(groupid)
            End If

            Common.WebRequest.ProData.Execute(pSql)

            If data("State") <> "remove" Then
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
            pSql = "SELECT template.ID" &
            ",template.Name, template.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname" & vbCrLf &
            " FROM LibraryStoreCategoryTemplate template INNER JOIN MobizLibraryPeople ppl" & vbCrLf &
            " ON template.UserID = ppl.ID WHERE template.IsActive = 1"
        Else
            pSql = "SELECT template.ID" &
            ",template.Name, template.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname" & vbCrLf &
            " FROM LibraryStoreCategoryTemplate template INNER JOIN MobizLibraryPeople ppl" & vbCrLf &
            " ON template.UserID = ppl.ID WHERE template.IsActive = 1 AND template.Name LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKeyword"))
        End If

        pSql &= " ORDER BY template.Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        strReturn = JsonDictionary.Serialize("Results", pDT)

        Return strReturn

    End Function

    <HttpPost()>
    Public Function CreateTemplate() As String
        Dim name As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            name = Common.WebRequest.Data("Name")

            If Common.WebRequest.Data("ID") Is Nothing Then
                pSql = "SELECT ID, Name" & vbCrLf &
                " FROM LibraryStoreCategoryTemplate" &
                " WHERE IsActive = 1 AND Name =" & ReadyString(name)
            Else

                pSql = "SELECT ID, Name" & vbCrLf &
                " FROM LibraryStoreCategoryTemplate" &
                " WHERE IsActive = 1 AND ID <> " & ReadyString(Common.WebRequest.Data("ID")) & " AND Name = " & ReadyString(name)
            End If

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then
                Return JsonDictionary.Serialize("Error", "There is already an existing menu with the same name. Please choose a different name.")
            End If

            pOut = New JsonDictionary
            pIU = New InsertUpdate("LibraryStoreCategoryTemplate")
            pIU("Name") = ReadyString(name)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("IsActive") = 1
            pIU("LastUpdated") = "getdate()"

            If Common.WebRequest.ProData.Execute("SELECT ID From LibraryStoreCategoryTemplate").Rows.Count = 0 Then
                pIU("IsDefault") = 1 'make as default the first template
            End If

            If Common.WebRequest.Data("ID") Is Nothing Then
                pID = Guid.NewGuid.ToString
                pIU("ID") = ReadyString(pID)
                pSql = pIU.GetInsert
            Else
                pSql = pIU.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID")))
            End If


            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
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
            pIU = New InsertUpdate("LibraryStoreCategoryGroup")
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


            pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate() Where ID =" & ReadyString(pMenuID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
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
                pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate() WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE MenuID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryStoreCategoryGroup  WHERE MenuID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryStoreCategoryTemplate WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)


            Else
                'if delete SKU single record

                pSql = "SELECT MenuID FROM LibraryStoreCategoryGroupPIM WHERE ID = " & ReadyString(id)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate() Where ID =" & ReadyString(pDT.Rows(0).Item("MenuID").ToString())
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE ID = " & ReadyString(id)
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
                pSql = "SELECT MenuID FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(id)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate() Where ID =" & ReadyString(pDT.Rows(0).Item("MenuID").ToString())
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE GroupID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                'delete the id in pim groups

                Return

            Else

                pSql = "SELECT ID" & vbCrLf &
               " FROM LibraryStoreCategoryGroup" &
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

                    pSql = "SELECT MenuID FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(id)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate() Where ID =" & ReadyString(pDT.Rows(0).Item("MenuID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE GroupID = " & ReadyString(id)
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    'delete the id in pim groups
                    pSql = "DELETE FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(id)
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


            pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE GroupID = " & ReadyString(id)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            'delete the id in pim groups
            pSql = "DELETE FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(id)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


        Catch ex As Exception

        End Try
    End Sub


    Private Sub DeleteFunctionParent(ByVal id As String)
        Dim pSql As String
        Dim pDT As DataTable
        Dim i As Int16 = 0

        pSql = "SELECT ID" & vbCrLf &
       " FROM LibraryStoreCategoryGroup" &
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


                pSql = "DELETE FROM LibraryStoreCategoryGroupPIM WHERE GroupID = " & ReadyString(id)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                'delete the id in pim groups
                pSql = "DELETE FROM LibraryStoreCategoryGroup WHERE ID = " & ReadyString(id)
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
            " FROM LibraryStoreCategoryGroupPIM" &
            " WHERE IsActive = 1 AND ProductID = " & ReadyString(vID) & " AND MenuID =" & ReadyString(vMenuID) & vbCrLf
            If CStr2(vParentID) <> "" Then
                pSql &= " AND GroupID = " & ReadyString(vParentID) & vbCrLf
            Else
                pSql &= " AND GroupID IS NULL"
            End If

        Else
            pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryStoreCategoryGroup" &
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
            " FROM LibraryStoreCategoryGroup" & vbCrLf &
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
          " FROM LibraryStoreCategoryGroup " & vbCrLf &
          " WHERE IsActive = 1 AND MenuID= " & ReadyString(menuID) & vbCrLf &
          " AND ParentID IS NULL" & vbCrLf &
          " UNION SELECT DISTINCT LibraryStoreCategoryGroupPIM.ID as ID, LibraryStoreCategoryGroupPIM.ProductID as ProductID, MobizLibraryPIMProduct.ProductName as ProductName" &
          ",LibraryStoreCategoryGroupPIM.ShortName as Name, 'SKU' as table_name, MobizLibraryPIMProductMeasurement.Quantity as Quantity,MobizLibraryCountriesLanguageLabel.Label as UnitMeasure,  MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price - (MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (MobizLibraryPIMProductMeasurement.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount as DiscountedPrice, MobizLibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID" & vbCrLf &
          " FROM LibraryStoreCategoryGroupPIM LEFT JOIN MobizLibraryPIMProduct ON LibraryStoreCategoryGroupPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductMeasurement ON LibraryStoreCategoryGroupPIM.ProductID = MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductPriceLevel ON LibraryStoreCategoryGroupPIM.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID"
            If (CBool2(WithContentsOnly)) Then
                pSql &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If
            pSql &= " WHERE LibraryStoreCategoryGroupPIM.MenuID= " & ReadyString(menuID) & " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(vPriceLevelID) & vbCrLf &
             " AND MobizLibraryPIMProductPriceLevel.OrganizationID= " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND LibraryStoreCategoryGroupPIM.GroupID IS NULL AND MobizLibraryPIMProductMeasurement.MeasurementLevel = 1 AND LibraryStoreCategoryGroupPIM.IsActive = 1" & vbCrLf

        Else
            pSql = "SELECT DISTINCT ID as ID, ID as ProductID, Name as ProductName" &
          ",Name as Name, 'Group' as table_name, '1' as Quantity, '' as UnitMeasure, '1' as DiscountedPrice, ID as UnitMeasureID " & vbCrLf &
          " FROM LibraryStoreCategoryGroup" & vbCrLf &
          " WHERE IsActive = 1 AND MenuID= " & ReadyString(menuID) & vbCrLf &
          " AND ParentID = " & ReadyString(vID) & vbCrLf &
          " UNION SELECT DISTINCT LibraryStoreCategoryGroupPIM.ID as ID, LibraryStoreCategoryGroupPIM.ProductID as ProductID, MobizLibraryPIMProduct.ProductName as ProductName" &
          ",LibraryStoreCategoryGroupPIM.ShortName as Name, 'SKU' as table_name, MobizLibraryPIMProductMeasurement.Quantity as Quantity, MobizLibraryCountriesLanguageLabel.Label as UnitMeasure, MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price - (MobizLibraryPIMProductMeasurement.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (MobizLibraryPIMProductMeasurement.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount as DiscountedPrice, MobizLibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID" & vbCrLf &
          " FROM LibraryStoreCategoryGroupPIM LEFT JOIN MobizLibraryPIMProduct ON LibraryStoreCategoryGroupPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductMeasurement ON LibraryStoreCategoryGroupPIM.ProductID = MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMProductPriceLevel ON LibraryStoreCategoryGroupPIM.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
          " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMProductMeasurement.UnitMeasureID = MobizLibraryPIMMeasure.ID" & vbCrLf &
          " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID"
            If (CBool2(WithContentsOnly)) Then
                pSql &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If
            pSql &= " WHERE LibraryStoreCategoryGroupPIM.MenuID= " & ReadyString(menuID) & " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(vPriceLevelID) & vbCrLf &
           " AND MobizLibraryPIMProductPriceLevel.OrganizationID= " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND LibraryStoreCategoryGroupPIM.GroupID =" & ReadyString(vID) & " AND MobizLibraryPIMProductMeasurement.MeasurementLevel = 1 AND LibraryStoreCategoryGroupPIM.IsActive = 1" & vbCrLf

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
          " FROM LibraryStoreCategoryGroup" & vbCrLf &
          " WHERE IsActive = 1 AND ParentID= " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
          " UNION SELECT sku.ID as ID, orgpim.AllowDecimal as AllowDecimal, sku.ProductID as ProductID, pim.ProductName as ProductName" &
          ",sku.ShortName as Name, 'SKU' as table_name, meas.Quantity as Quantity,MobizLibraryCountriesLanguageLabel.Label as UnitMeasure, meas.Quantity * MobizLibraryPIMProductPriceLevel.Price - (meas.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (meas.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount as DiscountedPrice, meas.UnitMeasureID as UnitMeasureID" & vbCrLf &
          " FROM LibraryStoreCategoryGroupPIM sku LEFT JOIN MobizLibraryPIMProduct pim ON sku.ProductID = pim.ID" & vbCrLf &
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
            " FROM LibraryStoreCategoryGroup" & vbCrLf &
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

        pSql = " SELECT LibraryStoreCategoryGroupPIM.ID, LibraryStoreCategoryGroupPIM.ProductID, LibraryStoreCategoryGroupPIM.GroupID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName ,LibraryStoreCategoryGroupPIM.ShortName" & vbCrLf &
            " FROM LibraryStoreCategoryGroupPIM" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryStoreCategoryGroupPIM.ProductID" & vbCrLf &
            " WHERE GroupID = " & ReadyString(vID) & " AND LibraryStoreCategoryGroupPIM.IsActive = 1 AND LibraryStoreCategoryGroupPIM.MenuID= " & ReadyString(menuID) & vbCrLf &
            " ORDER BY LibraryStoreCategoryGroupPIM.ShortName"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetProductsMain(ByVal menuID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT LibraryStoreCategoryGroupPIM.ID, LibraryStoreCategoryGroupPIM.ProductID, LibraryStoreCategoryGroupPIM.GroupID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName,LibraryStoreCategoryGroupPIM.ShortName" & vbCrLf &
            " FROM LibraryStoreCategoryGroupPIM" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryStoreCategoryGroupPIM.ProductID" & vbCrLf &
            " WHERE LibraryStoreCategoryGroupPIM.MenuID= " & ReadyString(menuID) & " AND LibraryStoreCategoryGroupPIM.IsActive = 1 AND LibraryStoreCategoryGroupPIM.GroupID IS NULL " & vbCrLf &
            " ORDER BY LibraryStoreCategoryGroupPIM.ShortName"
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


            pIU = New InsertUpdate("LibraryStoreCategoryGroupPIM")
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

            pSql = "Update LibraryStoreCategoryTemplate Set LastUpdated = GetDate() Where ID =" & ReadyString(pMenuID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryStoreCategoryTemplate'"
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
                " FROM LibraryStoreCategoryGroupPIM" & vbCrLf &
                " WHERE IsActive = 1 AND MenuID = " & ReadyString(Common.WebRequest.Data("MenuID")) & " AND GroupID IS NULL AND ShortName= " & ReadyString(name)
            Else
                pSql = "SELECT ID, ShortName" & vbCrLf &
                " FROM LibraryStoreCategoryGroupPIM" & vbCrLf &
                " WHERE IsActive = 1 AND MenuID = " & ReadyString(Common.WebRequest.Data("MenuID")) & " AND GroupID = " & ReadyString(Common.WebRequest.Data("GroupID")) & " AND ShortName= " & ReadyString(name)
            End If

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Error", "There is already an existing SKU with the same short name. Please choose a different name.")
            End If

            pIU = New InsertUpdate("LibraryStoreCategoryGroupPIM")

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

        Try
            pSql = "SELECT Name, IsDefault" & vbCrLf &
            " FROM LibraryStoreCategoryTemplate WHERE IsActive = 1 AND ID = " & ReadyString(Common.WebRequest.Data("ID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            'return template name
            strReturn = JsonDictionary.Serialize("Result", pDT)

            Return strReturn

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