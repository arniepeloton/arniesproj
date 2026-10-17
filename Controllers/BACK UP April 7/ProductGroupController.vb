Public Class ProductGroupController
    Inherits System.Web.Mvc.Controller
    Public firstLevelID As String = ""
    Public secondLevelID As String = ""
    Public thirdLevelID As String = ""
    Public skuID As String = ""
    Dim isDone As Boolean = False
    Dim isParent As Boolean = False

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
            " ON pim.UserID = ppl.ID WHERE pim.IsActive = 1 AND pim.Name LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKeyword"))
        End If

        pSql &= " ORDER BY pim.Name"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        strReturn = JsonDictionary.Serialize("PimMenuData", pDT)

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
                " FROM LibraryPimMenu" &
                " WHERE IsActive = 1 AND Name =" & ReadyString(name)
            Else

                pSql = "SELECT ID, Name" & vbCrLf &
                " FROM LibraryPimMenu" &
                " WHERE IsActive = 1 AND ID <> " & ReadyString(Common.WebRequest.Data("ID")) & " AND Name = " & ReadyString(name)
            End If

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then
                Return JsonDictionary.Serialize("Error", "There is already an existing menu with the same name. Please choose a different name.")
            End If

            pOut = New JsonDictionary
            pIU = New InsertUpdate("LibraryPimMenu")
            pIU("Name") = ReadyString(name)
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

    Private Function IsExist(ByVal vID As String, ByVal vParentID As String, ByVal vMenuID As String, ByVal vName As String) As Boolean

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

        Return pDT.Rows.Count > 0

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

        Try
            pSql = "SELECT Name" & vbCrLf &
            " FROM LibraryPimMenu WHERE IsActive = 1 AND ID = " & ReadyString(Common.WebRequest.Data("ID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            'return template name
            strReturn = JsonDictionary.Serialize("Result", pDT.Rows(0).Item("Name"))

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




