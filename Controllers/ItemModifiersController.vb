Namespace WebProj
    Public Class ItemModifiersController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost>
        Function SaveItemModifier() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim GroupModifierID As String = CStr2(Common.WebRequest.Data("ID"))
            Dim GroupModifierName As String = CStr2(Common.WebRequest.Data("GroupModifierName"))
            Dim GroupModifierDescription As String = CStr2(Common.WebRequest.Data("GroupModifierDescription"))
            Dim ModifierType As String = CStr2(Common.WebRequest.Data("ModifierType"))
            Dim IsRequired As String = CBool2(Common.WebRequest.Data("IsRequired"))
            Dim IsActive As String = CBool2(Common.WebRequest.Data("IsActive"))
            Dim PreferenceType As String = CStr2(Common.WebRequest.Data("PreferenceType"))
            Dim IsChangePreferenceType As String = CBool2(Common.WebRequest.Data("IsChangePreferenceType"))
            Dim ProductCategories As Object = Common.WebRequest.AllData("ProductCategories")
            Dim PrevProductCategories As Object = Common.WebRequest.AllData("PrevProductCategories")
            Dim IsRestrictedModifier As String = Common.WebRequest.Data("IsRestrictedModifier")
            Dim RestrictedDay As Object = Common.WebRequest.AllData("RestrictedDay")
            Dim Items As Object = Common.WebRequest.AllData("Items")
            Dim AssignedFowst As Object = Common.WebRequest.AllData("AssignedFowst")
            Dim AssignedCustomer As Object = Common.WebRequest.AllData("AssignedCustomer")
            Dim DateFrom As String = CStr2(Common.WebRequest.Data("DateFrom"))
            Dim DateTo As String = CStr2(Common.WebRequest.Data("DateTo"))
            Dim IsSaveAs As String = CBool2(Common.WebRequest.Data("IsSaveAs"))
            Dim pError As String
            Dim pList As List(Of String)
            Dim pSql As String
            Dim pDT As DataTable
            Dim ChoicesReference As String = ""
            Dim PromoBuyQty As String = 0
            Dim PromoTakeQty As String = 0
            Dim PromoPrice As String = 0
            Dim PromoDiscount As String = 0

            Common.WebRequest.ProData.BeginTransaction()

            If ModifierType = "Inventory Affecting" Then
                ChoicesReference = Common.WebRequest.Data("ChoicesReference")
                PromoBuyQty = Common.WebRequest.Data("PromoBuyQty")
                PromoTakeQty = Common.WebRequest.Data("PromoTakeQty")
                PromoPrice = Common.WebRequest.Data("PromoPrice")
                PromoDiscount = Common.WebRequest.Data("PromoDiscount")
            End If

            If GroupModifierID = "" Then
                If IsSaveAs Then
                    GroupModifierName = GroupModifierName & " - Copy"
                End If

                GroupModifierID = WebRequest.Instance.Business.LibraryItemModifiers.Add(
                                        vName:=GroupModifierName,
                                        vDescription:=GroupModifierDescription,
                                        vIsRestrictedModifier:=IsRestrictedModifier,
                                        vRestrictedDay:=RestrictedDay,
                                        vUserID:=WebSession.Instance.PeopleUserID,
                                        vType:=ModifierType,
                                        vRequired:=IsRequired,
                                        vIsActive:=IsActive,
                                        vPreferenceType:=PreferenceType,
                                        vChoicesReference:=ChoicesReference,
                                        vPromoBuyQty:=PromoBuyQty,
                                        vPromoTakeQty:=PromoTakeQty,
                                        vPromoPrice:=PromoPrice,
                                        vPromoDiscount:=PromoDiscount,
                                        vDateFrom:=DateFrom,
                                        vDateTo:=DateTo
                                    )
            Else
                If WebRequest.Instance.Business.LibraryItemModifiers.Rename(GroupModifierID,
                            GroupModifierName, GroupModifierDescription, WebSession.Instance.PeopleUserID,
                            pError, IsRestrictedModifier, RestrictedDay, ModifierType,
                            IsRequired, IsActive, PreferenceType,
                            ChoicesReference, PromoBuyQty, PromoTakeQty, PromoPrice, PromoDiscount,
                            vDateFrom:=DateFrom, vDateTo:=DateTo) = False Then
                    Return JsonDictionary.Serialize("Error", pError)
                End If
            End If

            For Each item As Object In Items
                If CType(item, Dictionary(Of String, Object)).ContainsKey("OrderNo") = False Then
                    item("OrderNo") = 1
                End If
                If ModifierType = "Inventory Affecting" Then
                    If WebRequest.Instance.Business.LibraryItemModifiers.SaveInventoryAffecting(GroupModifierID,
                        item("ProductID").ToString(), item("Description"), item("Qty"),
                        item("UnitMeasureID").ToString(), item("UnitMeasure"), item("UnitPrice"), item("PremiumPrice"), item("State"),
                        item("ID"), item("Stores"), item("OrderNo"), IsSaveAs, pError) = False Then
                        Return JsonDictionary.Serialize("Error", pError)
                    End If
                ElseIf ModifierType = "Non-Inventory Affecting" Then
                    Dim Type As String = CStr2(Common.WebRequest.Data("Type"))
                    Dim OtherDescription As String = ""
                    Dim NoOfCharacters As Integer = 0

                    If item.ContainsKey("OtherDescription") Then
                        OtherDescription = item("OtherDescription")
                    End If

                    If item.ContainsKey("NoOfCharacters") Then
                        NoOfCharacters = item("NoOfCharacters")
                    End If

                    If WebRequest.Instance.Business.LibraryItemModifiers.SaveNonInventoryAffecting(GroupModifierID,
                        item("Description"),
                        OtherDescription,
                        NoOfCharacters,
                        Type, item("State"),
                        item("ID"), item("OrderNo"), IsSaveAs, pError) = False Then
                        Return JsonDictionary.Serialize("Error", pError)
                    End If
                ElseIf ModifierType = "Return For Rebates" Then
                    Try
                        If item("State") = "remove" Then
                            pList = New List(Of String)
                            pList.Add(item("ProductID").ToString())
                            WebRequest.Instance.Business.LibraryItemModifiers.DeleteRebates(GroupModifierID, pList, WebSession.Instance.PeopleUserID)
                        Else
                            WebRequest.Instance.Business.LibraryItemModifiers.AddRebates(
                                vItemModifierID:=GroupModifierID,
                                vRebateID:=item("ID").ToString(),
                                vProductID:=item("ProductID").ToString(),
                                vUnitMeasure:=item("UnitMeasure"),
                                vUnitQuantity:=item("UnitQuantity"),
                                vQuantity:=item("Quantity"),
                                vAmount:=item("Amount"),
                                vUserID:=WebSession.Instance.PeopleUserID,
                                vIsSaveAs:=IsSaveAs
                            )
                        End If
                    Catch ex As Exception
                        Return JsonDictionary.Serialize("Error", ex.Message)
                    End Try
                End If
            Next item

            For Each category As Object In ProductCategories
                pList = New List(Of String)
                If PreferenceType = "category" Then
                    pList.Add(category("CategoryName").ToString())
                Else
                    pList.Add(category("ProductID").ToString())
                End If

                If category("State") <> "remove" Then
                    WebRequest.Instance.Business.LibraryItemModifiers.AddCategories(GroupModifierID, pList, WebSession.Instance.PeopleUserID, PreferenceType)
                Else
                    WebRequest.Instance.Business.LibraryItemModifiers.DeleteCategories(GroupModifierID, pList, WebSession.Instance.PeopleUserID, PreferenceType)
                End If
            Next category

            If IsChangePreferenceType Then 'DELETE ALL Previous Preferences
                For Each category As Object In PrevProductCategories
                    Dim pref As String
                    pList = New List(Of String)
                    If PreferenceType = "category" Then
                        pref = "sku"
                        pList.Add(category("ProductID").ToString())
                    Else
                        pref = "category"
                        pList.Add(category("CategoryName").ToString())
                    End If

                    WebRequest.Instance.Business.LibraryItemModifiers.DeleteCategories(GroupModifierID, pList, WebSession.Instance.PeopleUserID, pref)
                Next category
            End If

            For Each fowst As Object In AssignedFowst
                Dim p As New InsertUpdate("LibraryItemModifierAssignedFowst")

                If fowst("State") <> "delete" Then
                    p("ItemModifierID") = ReadyString(GroupModifierID)
                    p("FowstID") = ReadyString(fowst("ID").ToString())

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ItemModifierID = " & ReadyString(GroupModifierID) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("LastUpdated") = "getdate()"

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Else
                    pSql = "DELETE FROM LibraryItemModifierAssignedFowst WHERE ItemModifierID = " & ReadyString(GroupModifierID) & " AND FowstID = " & ReadyString(fowst("ID").ToString())
                    Common.WebRequest.ProData.Execute(pSql)
                End If
            Next

            For Each customer As Object In AssignedCustomer
                Dim p As New InsertUpdate("LibraryItemModifierAssignedCustomer")

                If customer("State") <> "delete" Then
                    p("ItemModifierID") = ReadyString(GroupModifierID)
                    p("CustomerOrgID") = ReadyString(customer("ID").ToString())

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ItemModifierID = " & ReadyString(GroupModifierID) & " AND CustomerOrgID = " & ReadyString(customer("ID").ToString()))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("LastUpdated") = "getdate()"

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Else
                    pSql = "DELETE FROM LibraryItemModifierAssignedCustomer WHERE ItemModifierID = " & ReadyString(GroupModifierID) & " AND CustomerOrgID = " & ReadyString(customer("ID").ToString())
                    Common.WebRequest.ProData.Execute(pSql)
                End If
            Next

            'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
            pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
            pDT = Common.WebRequest.ProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()
            Return JsonDictionary.Serialize("Result", "ok")
        End Function

        <HttpPost>
        Public Function Add() As String

            Dim pName As String
            Dim pSql As String
            Dim pDR As DataRow
            Dim pID As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pName = CStr2(Common.WebRequest.Data("Name")).Trim()
            If pName.Length = 0 Then
                Return JsonDictionary.Serialize("Error", "Name must not be blank")
            End If
            If pName.Length > 100 Then
                Return JsonDictionary.Serialize("Error", "Name can be only 100 or less characters in length. The name you specified is " & pName.Length & " characters long.")
            End If

            pDR = WebRequest.Instance.Business.LibraryItemModifiers.ByName(pName)
            If pDR IsNot Nothing Then
                Return JsonDictionary.Serialize("Error", "That name already exist. Please choose a unique name.")
            Else
                'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
                pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
                pDT = Common.WebRequest.ProData.Execute(pSql)
                If pDT.Rows.Count > 0 Then
                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
                
            End If

            pID = WebRequest.Instance.Business.LibraryItemModifiers.Add(
                vName:=pName,
                vDescription:="",
                vUserID:=WebSession.Instance.PeopleUserID
            )
            Return JsonDictionary.Serialize("Result", pID)

        End Function

        <HttpPost>
        Public Function AddCategory() As String

            Dim pCategoryName As String
            Dim pCategoryNames As Object
            Dim pDR As DataRow
            Dim pID As String
            Dim pList As List(Of String)
            Dim pCount As Integer
            Dim pSql As String
            Dim pDT As DataTable
            Dim pPreferenceType As String = Common.WebRequest.Data("PreferenceType")

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pID = Common.WebRequest.Data("ID")
                If IsGuid(pID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ID")
                End If

                pList = New List(Of String)
                pCategoryNames = Common.WebRequest.AllData("CategoryNames")
                For Each pItem As String In pCategoryNames
                    If TypeOf pItem Is String Then
                        pList.Add(pItem)
                    End If
                Next pItem

                pCategoryName = CStr2(Common.WebRequest.Data("CategoryName")).Trim()
                If pCategoryName.Length > 0 Then
                    pList.Add(pCategoryName)
                End If

                If pList.Count = 0 Then
                    Return JsonDictionary.Serialize("Error", "Please specify at least 1 category.")
                End If
                pList = UniqueItems(pList)

                pCount = WebRequest.Instance.Business.LibraryItemModifiers.AddCategories(pID, pList, WebSession.Instance.PeopleUserID, pPreferenceType)
                If pCount > 0 Then
                    'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
                    pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If

                    Return JsonDictionary.Serialize("Result", "ok")
                Else
                    Return JsonDictionary.Serialize("Error", "Category already exist for that Item Modifier.")
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function DeleteCategories() As String

            Dim pCategoryNames As Object
            Dim pDR As DataRow
            Dim pID As String
            Dim pList As List(Of String)
            Dim pCount As Integer
            Dim pSql As String
            Dim pDT As DataTable
            Dim pPreferenceType As String = Common.WebRequest.Data("PreferenceType")

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pID = Common.WebRequest.Data("ID")
                If IsGuid(pID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ID")
                End If

                pList = New List(Of String)
                pCategoryNames = Common.WebRequest.AllData("CategoryNames")
                For Each pItem As String In pCategoryNames
                    If TypeOf pItem Is String Then
                        pList.Add(pItem)
                    End If
                Next pItem

                If pList.Count = 0 Then
                    Return JsonDictionary.Serialize("Error", "Please specify at least 1 category.")
                End If
                pList = UniqueItems(pList)

                pCount = WebRequest.Instance.Business.LibraryItemModifiers.DeleteCategories(pID, pList, WebSession.Instance.PeopleUserID, pPreferenceType)
                If pCount > 0 Then
                    'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
                    pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                    Return JsonDictionary.Serialize("Result", "ok")
                Else
                    Return JsonDictionary.Serialize("Error", "Categories don't exist or were already deleted.")
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function GetData() As String

            Dim pDT As DataTable
            Dim pID As String
            Dim pOut As JsonDictionary

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pID = Common.WebRequest.Data("ID")
                If IsGuid(pID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ID")
                End If

                pOut = New JsonDictionary()

                pDT = Common.WebRequest.ProData.Execute("SELECT * FROM LibraryItemModifiers WHERE ID = " & ReadyString(pID))
                pOut.Add("Info", pDT)

                pDT = WebRequest.Instance.Business.LibraryItemModifiers.GetCategories(pID)
                pOut.Add("Categories", pDT)

                pDT = WebRequest.Instance.Business.LibraryItemModifiers.GetRebates(pID)
                pOut.Add("Rebates", pDT)

                pOut.Add("IA", WebRequest.Instance.Business.LibraryItemModifiers.GetInventoryAffecting(pID))

                pDT = WebRequest.Instance.Business.LibraryItemModifiers.GetNonInventoryAffecting(pID)
                pOut.Add("NIA", pDT)

                pDT = WebRequest.Instance.Business.LibraryItemModifiers.GetAssignedFowst(pID)
                pOut.Add("AssignedFowst", pDT)

                pDT = WebRequest.Instance.Business.LibraryItemModifiers.GetAssignedCustomer(pID)
                pOut.Add("AssignedCustomer", pDT)

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function AddRebate() As String

            Dim pProductID As String
            Dim pDR As DataRow
            Dim pItemModifierID As String
            Dim pRebateID As String
            Dim pAmount As Decimal
            Dim pUnitMeasure As String
            Dim pUnitQuantity As String
            Dim pQuantity As String
            Dim pSql As String
            Dim pDT As DataTable

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pItemModifierID = Common.WebRequest.Data("ItemModifierID")
                If IsGuid(pItemModifierID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ItemModifierID")
                End If

                pRebateID = Common.WebRequest.Data("RebateID")

                pProductID = Common.WebRequest.Data("ProductID")
                If IsGuid(pProductID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ProductID")
                End If

                pUnitMeasure = Common.WebRequest.Data("UnitMeasure")
                If CStr2(pUnitMeasure).Trim() = "" Then
                    Return JsonDictionary.Serialize("Error", "Missing UnitMeasure")
                End If

                pAmount = Common.WebRequest.Data("Amount")
                If IsNumeric(pAmount) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing Amount")
                End If
                pAmount = CDec2(pAmount)

                pUnitQuantity = Common.WebRequest.Data("UnitQuantity")
                If IsNumeric(pUnitQuantity) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing UnitQuantity")
                End If

                pQuantity = Common.WebRequest.Data("Quantity")
                If IsNumeric(pQuantity) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing Quantity")
                End If
                pQuantity = CDec2(pQuantity)

                pItemModifierID = WebRequest.Instance.Business.LibraryItemModifiers.AddRebates(
                    vItemModifierID:=pItemModifierID,
                    vRebateID:=pRebateID,
                    vProductID:=pProductID,
                    vUnitMeasure:=pUnitMeasure,
                    vUnitQuantity:=pUnitQuantity,
                    vQuantity:=pQuantity,
                    vAmount:=pAmount,
                    vUserID:=WebSession.Instance.PeopleUserID,
                    vIsSaveAs:=False
                )

                'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
                pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
                pDT = Common.WebRequest.ProData.Execute(pSql)
                If pDT.Rows.Count > 0 Then
                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                Return JsonDictionary.Serialize("Result", pItemModifierID)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function DeleteRebates() As String

            Dim pProductIDs As Object
            Dim pDR As DataRow
            Dim pID As String
            Dim pList As List(Of String)
            Dim pCount As Integer
            Dim pSql As String
            Dim pDt As DataTable

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pID = Common.WebRequest.Data("ID")
                If IsGuid(pID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ID")
                End If

                pList = New List(Of String)
                pProductIDs = Common.WebRequest.AllData("ProductIDs")
                For Each pItem As String In pProductIDs
                    If TypeOf pItem Is String Then
                        pList.Add(pItem)
                    End If
                Next pItem

                If pList.Count = 0 Then
                    Return JsonDictionary.Serialize("Error", "Please specify at least 1 product.")
                End If
                pList = UniqueItems(pList)

                pCount = WebRequest.Instance.Business.LibraryItemModifiers.DeleteRebates(pID, pList, WebSession.Instance.PeopleUserID)
                If pCount > 0 Then
                    'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
                    pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                    Return JsonDictionary.Serialize("Result", "ok")
                Else
                    Return JsonDictionary.Serialize("Error", "Rebate products  don't exist or were already deleted.")
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function GetPimData() As String

            Dim pDtUnits As DataTable
            Dim pOut As JsonDictionary
            Dim pProductID As String
            Dim pCustomerID As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pProductID = Common.WebRequest.Data("ProductID")
                If IsGuid(pProductID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ProductID")
                End If

                pCustomerID = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
                If IsGuid(pCustomerID) = False Then
                    Return JsonDictionary.Serialize("Error", "No default customer. Please set Customer Organization ID.")
                End If

                pDtUnits = WebRequest.Instance.Business.LibraryStock.PriceTable(
                    WebRequest.Instance.OrgID,
                    pProductID,
                    pCustomerID,
                    ,
                    ,
                    True)
                Return JsonDictionary.Serialize("data", pDtUnits)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function Rename() As String

            Dim pID As String
            Dim pNewName As String
            Dim pError As String
            Dim pSql As String
            Dim pDt As DataTable

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pID = Common.WebRequest.Data("ID")
                If IsGuid(pID) = False Then
                    Return JsonDictionary.Serialize("Error", "Invalid or missing ID")
                End If

                pNewName = Common.WebRequest.Data("NewName")
                If CStr2(pNewName).Trim() = "" Then
                    Return JsonDictionary.Serialize("Error", "Missing Name")
                End If

                If WebRequest.Instance.Business.LibraryItemModifiers.Rename(pID, pNewName, "", WebSession.Instance.PeopleUserID, pError) Then
                    'Notify the TablesLastUpdated that there were changes done in the LibraryItemModifiers
                    pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryItemModifiers'"
                    pDt = Common.WebRequest.ProData.Execute(pSql)
                    If pDt.Rows.Count > 0 Then
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryItemModifiers', NULL, NULL)"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                        pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryItemModifiers'"
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                    Return JsonDictionary.Serialize("Result", "ok")
                Else
                    Return JsonDictionary.Serialize("Error", pError)
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Function GetSortingItemModifier()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim skudetails As String = ""
            Dim pSql As String = ""

            Try

                Dim pDT As Object
                Dim arr As New List(Of Dictionary(Of String, Object))

                pSql = " SELECT * FROM (" & vbCrLf &
                        "   SELECT CASE WHEN CategoryName = '' THEN CONVERT(varchar(max), ProductID) ELSE CategoryName END as SkuDetails " & vbCrLf &
                        "   FROM LibraryItemModifierCategories " & vbCrLf &
                        "   ) as T " & vbCrLf &
                        " GROUP BY SkuDetails " & vbCrLf
                pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each dt As Object In pDT
                    pSql = "SELECT ItemModifierID " & vbCrLf &
                            " FROM LibraryItemModifierCategories "

                    skudetails = dt("SkuDetails").ToString()

                    If IsGuid(skudetails) Then
                        pSql += " WHERE ISNULL(CategoryName, '') = '' AND ProductID = " & ReadyString(skudetails)
                    Else
                        pSql += " WHERE ISNULL(CONVERT(varchar(max), ProductID), '') = '' AND CategoryName = " & ReadyString(skudetails)
                    End If

                    Dim pDtIM As Object = Common.WebRequest.ProData.Execute(pSql)

                    If pDtIM.Rows.Count > 1 Then
                        Dim subq As String
                        If IsGuid(dt("SkuDetails").ToString()) Then
                            subq = "(SELECT TOP 1 ISNULL(OrderNo, 0) FROM LibraryItemModifierSorting WHERE ItemModifierID = LibraryItemModifiers.ID AND ProductID =  " & ReadyString(skudetails) & "  GROUP BY ItemModifierID, OrderNo)"
                        Else
                            subq = "(SELECT TOP 1 ISNULL(OrderNo, 0) FROM LibraryItemModifierSorting WHERE ItemModifierID = LibraryItemModifiers.ID AND CategoryName =  " & ReadyString(skudetails) & ")"
                        End If
                        pSql = " SELECT ID, Name, " & vbCrLf &
                                "ISNULL(" & subq & ", 0) as OrderNo " & vbCrLf &
                                "FROM LibraryItemModifiers WHERE ID IN (" & pSql & ")" & vbCrLf &
                                "ORDER BY OrderNo, Name ASC "
                        dt.Add("ItemModifiers", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                        If IsGuid(dt("SkuDetails").ToString()) Then
                            pSql = "SELECT ProductName FROM MobizLibraryPIMProduct WHERE ID = " & ReadyString(dt("SkuDetails").ToString())
                            dt.Add("SkuName", Common.WebRequest.ProData.Execute(pSql).Rows(0)("ProductName"))
                        Else
                            dt.Add("SkuName", dt("SkuDetails"))
                        End If

                        arr.Add(dt)
                    End If
                Next dt

                arr.Sort(Function(x, y) x("SkuName").CompareTo(y("SkuName")))

                Return Json(New With {Key .Results = arr})
            Catch ex As Exception
                Return Json(New With {Key .Results = ex.Message, .SKU = skudetails, .Query = psql})
            End Try
        End Function

        Function SaveSorting()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")

                Common.WebRequest.ProData.BeginTransaction()

                For Each d As Object In data
                    For Each i As Object In d("ItemModifiers")
                        Dim p As New InsertUpdate("LibraryItemModifierSorting")
                        Dim where As String

                        p("OrderNo") = CInt2(i("OrderNo"))
                        p("ItemModifierID") = ReadyString(i("ID").ToString())

                        If IsGuid(d("SkuDetails")) Then
                            p("ProductID") = ReadyString(d("SkuDetails").ToString())
                            where = " ProductID = " & ReadyString(d("SkuDetails").ToString())
                        Else
                            p("CategoryName") = ReadyString(d("SkuDetails").ToString())
                            where = " CategoryName = " & ReadyString(d("SkuDetails").ToString())
                        End If

                        where += " AND ItemModifierID = " & ReadyString(i("ID").ToString())

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(where)) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    Next i
                Next d

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryItemModifierSorting")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetFowstOfSelectedGroup()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT fowst.ID, fowst.Name as FowstName " & vbCrLf

                If CBool2(WebRequest.Instance.Data("ShowHBTemplate")) Then
                    pSql &= ", ISNULL(convert(varchar(max), fowst.DefaultHotButtonTemplateID), '') as HBTemplateID " & vbCrLf
                End If

                pSql &= " FROM MobizLibraryOrganizationFOWST as fowst " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFranchisee as franchisee ON fowst.ID = franchisee.FranchiseeOrgFowstID " & vbCrLf &
                        " WHERE franchisee.FranchiseGroupID = " & ReadyString(Common.WebRequest.Data("GroupID")) & vbCrLf &
                        " GROUP BY fowst.ID, fowst.Name " & vbCrLf &
                        " ORDER BY fowst.Name ASC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetHBItemModifiers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim categoryName As String = Common.WebRequest.Data("CategoryName")
                Dim vProductID As String = Common.WebRequest.Data("ProductID")
                Dim fowstid As String = Common.WebRequest.Data("FowstID").ToLower()
                Dim customerid As String = Common.WebRequest.Data("CustomerID").ToLower()
                Dim currentDate As Date = CDate(Common.WebRequest.Data("ZReadDate"))
                Dim pSql = " WHERE 1=0 "
                Dim subq As String
                Dim pDTItemModifier As Object
                Dim pDTFowstModifier As New List(Of Object)

                If Not String.IsNullOrWhiteSpace(categoryName) Then
                    pSql += " OR cat.CategoryName = " & ReadyString(categoryName)
                    subq = "(SELECT ISNULL(OrderNo, 0) FROM LibraryItemModifierSorting WHERE ItemModifierID = im.ID AND CategoryName =  " & ReadyString(categoryName) & ")"
                Else
                    subq = "(SELECT ISNULL(OrderNo, 0) FROM LibraryItemModifierSorting WHERE ItemModifierID = im.ID AND ProductID =  " & ReadyString(vProductID) & ")"
                End If

                If Not String.IsNullOrWhiteSpace(vProductID) Then
                    pSql += " OR cat.ProductID = " & ReadyString(vProductID)
                End If

                pSql = " SELECT * FROM (SELECT im.* " & vbCrLf &
                        " ,ISNULL(" & subq & ", 0) as OrderNo" & vbCrLf &
                        " FROM LibraryItemModifierCategories as cat " & vbCrLf &
                        " INNER JOIN LibraryItemModifiers as im ON im.ID = cat.ItemModifierID " & vbCrLf &
                        pSql & vbCrLf &
                        " AND (im.Type <> '' OR im.Type <> 'Return For Rebates')) as T WHERE ISNULL(IsActive, 1) = 1 ORDER BY OrderNo ASC, Name ASC " & vbCrLf
                pDTItemModifier = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each imData As Object In pDTItemModifier
                    imData.Add("ParentProductID", vProductID)
                    pSql = "SELECT * FROM LibraryItemModifierAssignedFowst WHERE ItemModifierID = " & ReadyString(imData("ID").ToString())
                    Dim fowstDT As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
                    Dim fowstincludeIM As Boolean = True

                    For Each fowst As Object In fowstDT
                        If fowst("FowstID").ToString().ToLower() = fowstid Then
                            fowstincludeIM = True
                            Exit For
                        Else
                            fowstincludeIM = False
                        End If
                    Next

                    If Not fowstincludeIM Then
                        Continue For
                    End If

                    pSql = "SELECT * FROM LibraryItemModifierAssignedCustomer WHERE ItemModifierID = " & ReadyString(imData("ID").ToString())
                    Dim cusDT As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
                    Dim cusincludeIM As Boolean = True

                    For Each customer As Object In cusDT
                        If customer("CustomerOrgID").ToString().ToLower() = customerid Then
                            cusincludeIM = True
                            Exit For
                        Else
                            cusincludeIM = False
                        End If
                    Next

                    If Not cusincludeIM Then
                        Continue For
                    End If

                    If CStr2(imData("DateFrom")) <> "" And CStr2(imData("DateTo")) <> "" Then
                        If currentDate >= CDate(imData("DateFrom")) And currentDate <= CDate(imData("DateTo")) Then
                            'show
                        Else
                            Continue For
                        End If
                    Else
                        If Not IsAllowedNow(imData) Then
                            Dim msg As String = ""
                            Dim arrDay() As String = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}
                            For Each d As Object In arrDay
                                If CStr2(imData(d & "TimeFrom")) <> "" Then
                                    msg += "<br> - " & d & " at " & imData(d & "TimeFrom") & " to " & imData(d & "TimeTo")
                                End If
                            Next
                            imData.Add("NotAvailableThisTime", True)
                            imData.Add("Message", "The availability of this modifier is every: <br>" & msg)
                            Continue For
                        End If
                    End If

                    Dim arrItems As Object

                    If CStr2(imData("ChoicesReference")).ToLower() = "buy1take1" Then
                        imData("Max") = imData("PromoTakeQty")
                        arrItems = GetBuyTake(imData)
                    ElseIf CStr2(imData("Type")) = "" Or CStr2(imData("Type")) = "Return For Rebates" Then
                        arrItems = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(GetNewRebates(imData("ID"))))
                    ElseIf CStr2(imData("Type")) = "Inventory Affecting" Then
                        arrItems = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(GetInventoryAffecting(imData("ID"))))
                    ElseIf CStr2(imData("Type")) = "Non-Inventory Affecting" Then
                        arrItems = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(GetNonInventoryAffecting(imData("ID"))))
                    End If

                    imData.Add("Items", arrItems)
                    pDTFowstModifier.Add(imData)
                Next imData

                Dim pOut As New JsonDictionary

                pOut.Add("Results", pDTFowstModifier)

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetNonInventoryAffecting(ByVal id As String)
            Return Common.WebRequest.ProData.Execute(" SELECT nia.*" & vbCrLf &
                                                     " FROM LibraryItemModifierNonInventoryAffecting as nia " & vbCrLf &
                                                     " WHERE nia.ItemModifierID = " & ReadyString(id))
        End Function

        Function GetInventoryAffecting(ByVal id As String)
            Return Common.WebRequest.ProData.Execute(" SELECT ia.*, prod.ProductName" & vbCrLf &
                                                     " FROM LibraryItemModifierInventoryAffecting as ia " & vbCrLf &
                                                     " INNER JOIN MobizLibraryPIMProduct as prod ON prod.ID = ia.ProductID " & vbCrLf &
                                                     " WHERE ia.ItemModifierID = " & ReadyString(id) & vbCrLf &
                                                     " ORDER BY CASE WHEN ISNULL(ia.PremiumPrice, 0) > 0 THEN 1 ELSE 0 END ASC, ISNULL(ia.OrderNo, 0) ASC, ia.Description ASC ")
        End Function

        Function GetNewRebates(ByVal id As String)
            Return Common.WebRequest.ProData.Execute(" SELECT reb.*, prod.ProductName" & vbCrLf &
                                                     " FROM LibraryItemModifierRebates as reb " & vbCrLf &
                                                     " INNER JOIN MobizLibraryPIMProduct as prod ON prod.ID = reb.ProductID " & vbCrLf &
                                                     " WHERE reb.ItemModifierID = " & ReadyString(id))
        End Function

        Function GetBuyTake(ByRef vItemModifierData As Object)
            If CDec2(vItemModifierData("PromoBuyQty")) <= 0 Then
                vItemModifierData("PromoBuyQty") = 1
            End If

            Dim pData As New Dictionary(Of String, Object)

            pData.Add("ID", Guid.NewGuid.ToString())
            pData.Add("ItemModifierID", vItemModifierData("ID"))
            pData.Add("ProductID", vItemModifierData("ParentProductID"))
            pData.Add("Description", "")
            pData.Add("Max", vItemModifierData("PromoTakeQty"))
            pData.Add("ProductName", "Buy " & vItemModifierData("PromoBuyQty") & " Take " & vItemModifierData("PromoTakeQty"))

            If CDec2(vItemModifierData("PromoTakePrice")) > 0 Then
                pData.Add("UnitPrice", CDec2(vItemModifierData("PromoTakePrice")))
            Else
                Dim pRetailPrice As String = Common.WebRequest.ProData.Execute(" SELECT Price " & vbCrLf &
                                        " ,LastUpdated " & vbCrLf &
                                        " FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                                        " WHERE PriceLevelID = '01000012-0005-0000-0000-000000000000' --RETAIL PRICE " & vbCrLf &
                                        " AND ProductID = " & ReadyString(vItemModifierData("ParentProductID")) & vbCrLf &
                                        " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)).Rows(0)("Price")
                pData.Add("UnitPrice", (CDec2(pRetailPrice) * CDec2(vItemModifierData("PromoTakeDiscount"))) / 100)
            End If

            Dim pUnit As Object = Common.WebRequest.ProData.Execute("SELECT TOP 1" & vbCrLf &
                                        "   MobizLibraryPIMProductMeasurement.UnitMeasureID, MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                                        "   FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                                        "   JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                                        "   JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                                        "    WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(vItemModifierData("ParentProductID")) & " AND MobizLibraryPIMProductMeasurement.Quantity = 1" & vbCrLf &
                                        "    ORDER BY MobizLibraryPIMProductMeasurement.Quantity ").Rows(0)

            pData.Add("UnitMeasureID", pUnit("UnitMeasureID"))
            pData.Add("UnitMeasure", pUnit("UnitMeasure"))
            Dim newPdata As New List(Of Dictionary(Of String, Object))
            newPdata.Add(pData)

            Return newPdata
        End Function

        Function IsAllowedNow(ByVal vImData As Object) As Boolean
            If Not CBool(vImData("IsRestrictedModifier")) Then
                Return True
            End If

            Dim pCurrentDateTime As Date = DateTime.Now()
            Dim pWeekDay As String = pCurrentDateTime.ToString("dddd")
            Dim pFieldNameFrom As String = pWeekDay & "TimeFrom"
            Dim pFieldNameTo As String = pWeekDay & "TimeTo"
            Dim pSchedFrom, pSchedTo As String

            If CStr2(vImData(pFieldNameFrom)) = "" Then
                Return False
            End If

            If CStr2(vImData(pFieldNameTo)) = "" Then
                Return False
            End If

            If CDate("1/1/1 " & vImData(pFieldNameFrom)) <= CDate("1/1/1 " & pCurrentDateTime.ToString("hh:mm tt")) And
                CDate("1/1/1 " & pCurrentDateTime.ToString("hh:mm tt")) <= CDate("1/1/1 " & vImData(pFieldNameTo)) Then
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace