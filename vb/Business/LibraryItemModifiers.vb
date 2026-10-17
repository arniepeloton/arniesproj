Imports PdfSharp.Pdf.Annotations

Public Class LibraryItemModifiers

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Public Function GetAssignedCustomer(vID As String) As DataTable
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT 'edit' as State, CASE WHEN org.CompanyName = '' THEN org.PersonSurname + ', ' + org.PersonFirstName + ' ' + org.PersonMiddleName ELSE org.CompanyName END as CustomerName, org.ID " & vbCrLf &
                " FROM LibraryItemModifierAssignedCustomer as imcus " & vbCrLf &
                " INNER JOIN MobizLibraryOrganization as org ON org.ID = imcus.CustomerOrgID" & vbCrLf &
                " WHERE imcus.ItemModifierID = " & ReadyString(vID) & vbCrLf
        pDT = mProData.Execute(pSql)
        Return pDT
    End Function

    Public Function GetAssignedFowst(vID As String) As DataTable
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT 'edit' as State, fowst.Name as FowstName, fowst.ID " & vbCrLf &
                " FROM LibraryItemModifierAssignedFowst as imaf " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = imaf.FowstID" & vbCrLf &
                " WHERE imaf.ItemModifierID = " & ReadyString(vID) & vbCrLf
        pDT = mProData.Execute(pSql)
        Return pDT
    End Function

    Public Function GetNonInventoryAffecting(vID As String) As DataTable
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT nia.*, 'edit' as State " & vbCrLf &
                " FROM LibraryItemModifierNonInventoryAffecting as nia " & vbCrLf &
                " WHERE nia.ItemModifierID = " & ReadyString(vID) & vbCrLf &
                " ORDER BY nia.OrderNo ASC "
        pDT = mProData.Execute(pSql)
        Return pDT
    End Function

    Public Function GetInventoryAffecting(vID As String)
        Dim pDT As Object
        Dim pSql As String

        pSql = "SELECT ia.*, prod.ProductName, prod.ProductNo, 'edit' as State " & vbCrLf &
                " FROM LibraryItemModifierInventoryAffecting as ia " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as prod ON prod.ID = ia.ProductID " & vbCrLf &
                " WHERE ia.ItemModifierID = " & ReadyString(vID) & vbCrLf &
                " ORDER BY ISNULL(ia.OrderNo, 0) ASC, prod.ProductName ASC "
        pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mProData.Execute(pSql)))

        For Each im As Object In pDT
            pSql = " SELECT fowst.ID, fowst.Name as FowstName " & vbCrLf &
                    " FROM LibraryItemModifierInventoryAffectingFowst as imfowst " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = imfowst.FowstID " & vbCrLf &
                    " WHERE InventoryAffectingID = " & ReadyString(im("ID").ToString()) &
                    " ORDER BY fowst.Name ASC "

            im.Add("Stores", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mProData.Execute(pSql))))
        Next

        Return pDT
    End Function

    Public Function SaveNonInventoryAffecting(vItemModifierID As String,
            vDescription As String, vOthersDescription As String, vNoOfCharacters As Integer, vType As String, vState As String, vID As String, vOrderNo As String, vIsSaveAs As Boolean, ByRef vError As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pIU = New InsertUpdate("LibraryItemModifierNonInventoryAffecting")
        pIU("ItemModifierID") = ReadyString(vItemModifierID)
        pIU("Type") = ReadyString(vType)
        pIU("Description") = ReadyString(vDescription)
        pIU("OrderNo") = ReadyString(vOrderNo)
        pIU("NoOfCharacters") = ReadyString(vNoOfCharacters)

        If Not String.IsNullOrWhiteSpace(vOthersDescription) Then
            pIU("OthersDescription") = ReadyString(vOthersDescription)
        End If

        If vState <> "remove" Then
            If vID = "" Or vIsSaveAs Then
                vID = Guid.NewGuid().ToString()
                pIU("ID") = ReadyString(vID)
                pSql = pIU.GetInsert()
            Else
                pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
            End If
        Else
            pSql = "DELETE FROM LibraryItemModifierNonInventoryAffecting WHERE ID = " & ReadyString(vID)
        End If

        Try
            mProData.ExecuteSQL(pSql, True)
            Return True
        Catch ex As Exception
            vError = ex.Message
            Return False
        End Try
    End Function

    Public Function SaveInventoryAffecting(vItemModifierID As String, vProductID As String,
            vDescription As String, vQty As Decimal, vUomID As String,
            vUomName As String, vUnitPrice As Decimal, vPremiumPrice As Decimal, vState As String,
            vID As String, vStores As Object, vOrderNo As Integer, vIsSaveAs As Boolean, ByRef vError As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pIU = New InsertUpdate("LibraryItemModifierInventoryAffecting")
        pIU("ItemModifierID") = ReadyString(vItemModifierID)
        pIU("ProductID") = ReadyString(vProductID)
        pIU("Description") = ReadyString(vDescription)
        pIU("Qty") = vQty
        pIU("UnitMeasureID") = ReadyString(vUomID)
        pIU("UnitMeasure") = ReadyString(vUomName)
        pIU("UnitPrice") = vUnitPrice
        pIU("PremiumPrice") = vPremiumPrice
        pIU("OrderNo") = IIf(String.IsNullOrWhiteSpace(vOrderNo), "NULL", vOrderNo)

        If vState <> "remove" Then
            If vID = "" Or vIsSaveAs Then
                vID = Guid.NewGuid().ToString()
                pIU("ID") = ReadyString(vID)
                pSql = pIU.GetInsert()
            Else
                pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
            End If

            mProData.ExecuteSQL(pSql, True)

            If vStores.Length = 0 Then
                pSql = "DELETE FROM LibraryItemModifierInventoryAffectingFowst WHERE InventoryAffectingID = " & ReadyString(vID) & vbCrLf
                mProData.ExecuteSQL(pSql)
            Else
                Dim storeid As New ArrayList

                For Each store As Object In vStores
                    storeid.Add(store("ID"))

                    pIU = New InsertUpdate("LibraryItemModifierInventoryAffectingFowst")

                    pIU("InventoryAffectingID") = ReadyString(vID)
                    pIU("FowstID") = ReadyString(store("ID"))

                    If mProData.ExecuteSQL(pIU.GetUpdate(" InventoryAffectingID = " & ReadyString(vID) & " AND FowstID = " & ReadyString(store("ID")))) = 0 Then
                        pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                        mProData.ExecuteSQL(pIU.GetInsert())
                    End If
                Next

                If storeid.Count > 0 Then
                    pSql = "DELETE FROM LibraryItemModifierInventoryAffectingFowst WHERE InventoryAffectingID = " & ReadyString(vID) & vbCrLf &
                        " AND FowstID NOT IN ('" & String.Join("', '", storeid.ToArray()) & "')"
                    mProData.ExecuteSQL(pSql)
                End If
            End If
        Else
            pSql = "DELETE FROM LibraryItemModifierInventoryAffecting WHERE ID = " & ReadyString(vID)
            mProData.ExecuteSQL(pSql, True)

            pSql = "DELETE FROM LibraryItemModifierInventoryAffectingFowst WHERE InventoryAffectingID = " & ReadyString(vID)
            mProData.ExecuteSQL(pSql, True)
        End If

        Try
            Return True
        Catch ex As Exception
            vError = ex.Message
            Return False
        End Try
    End Function

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ByName(vName As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryItemModifiers" & vbCrLf &
            " WHERE Name = " & ReadyString(vName)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function Add(vName As String,
        vDescription As String,
        vUserID As String,
        Optional vIsRestrictedModifier As String = "0",
        Optional vRestrictedDay As Object = Nothing,
        Optional vType As String = "Rebates",
        Optional vRequired As Boolean = False,
        Optional vIsActive As Boolean = True,
        Optional vPreferenceType As String = "categories",
        Optional vChoicesReference As String = "fromsku",
        Optional vPromoBuyQty As String = "0",
        Optional vPromoTakeQty As String = "0",
        Optional vPromoPrice As String = "0",
        Optional vPromoDiscount As String = "0",
        Optional vDateFrom As String = "",
        Optional vDateTo As String = "") As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid.ToString()
        pIU = New InsertUpdate("LibraryItemModifiers")
        pIU("ID") = ReadyString(pID)
        pIU("Name") = ReadyString(vName)
        pIU("Description") = ReadyString(vDescription)
        pIU("Type") = ReadyString(vType)
        pIU("IsRequired") = ReadyString(vRequired)
        pIU("IsActive") = ReadyString(vIsActive)
        pIU("PreferenceType") = ReadyString(vPreferenceType)
        pIU("IsRestrictedModifier") = ReadyString(vIsRestrictedModifier)
        pIU("ChoicesReference") = ReadyString(vChoicesReference)
        pIU("PromoBuyQty") = ReadyString(vPromoBuyQty)
        pIU("PromoTakeQty") = ReadyString(vPromoTakeQty)
        pIU("PromoTakePrice") = IIf(vPromoPrice = "", "NULL", ReadyString(vPromoPrice))
        pIU("PromoTakeDiscount") = IIf(vPromoDiscount = "", "NULL", ReadyString(vPromoDiscount))
        pIU("DateFrom") = IIf(vDateFrom = "", "NULL", ReadyString(vDateFrom))
        pIU("DateTo") = IIf(vDateTo = "", "NULL", ReadyString(vDateTo))
        pIU("LastUpdated") = "GETDATE()"
        pIU("UpdatedBy") = ReadyString(vUserID)

        For Each day As Object In vRestrictedDay
            pIU(day("Day") & "TimeFrom") = ReadyString(day("From"))
            pIU(day("Day") & "TimeTo") = ReadyString(day("To"))
        Next day

        If vType = "Inventory Affecting" Or vType = "Non-Inventory Affecting" Then
            pIU("Max") = ReadyString(CInt2(Common.WebRequest.Data("Max")))
        End If

        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function AddCategories(vID As String, vCategoryNames As List(Of String), vUserID As String, vPreferenceType As String) As Integer

        Dim p As Integer

        For Each pItem As String In vCategoryNames
            If AddCategory(vID, pItem, vUserID, vPreferenceType) Then
                p += 1
            End If
        Next pItem

        Return p

    End Function

    Public Function AddCategory(vID As String, vCategoryName As String, vUserID As String, vPreferenceType As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        If HasCategory(vID, vCategoryName, vPreferenceType) Then
            Return False
        End If

        pIU = New InsertUpdate("LibraryItemModifierCategories")
        pIU("ID") = "newid()"
        pIU("ItemModifierID") = ReadyString(vID)
        pIU("CategoryName") = IIf(vPreferenceType = "category", ReadyString(vCategoryName), "' '")
        pIU("ProductID") = IIf(vPreferenceType = "sku", ReadyString(vCategoryName), "NULL")

        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("LibraryItemModifiers")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UpdatedBy") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

        Return True

    End Function

    Public Function GetCategories(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ISNULL(PreferenceType, '') as PreferenceType FROM LibraryItemModifiers WHERE ID = " & ReadyString(vID)
        Dim preferenceType As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("PreferenceType")

        If CStr2(preferenceType) = "" Then
            preferenceType = "category"
        End If

        If preferenceType = "category" Then
            pSql = " SELECT LibraryItemModifierCategories.*, 'edit' as State" & vbCrLf &
            " FROM LibraryItemModifierCategories" & vbCrLf &
            " WHERE LibraryItemModifierCategories.ItemModifierID = " & ReadyString(vID) & vbCrLf &
            " ORDER BY LibraryItemModifierCategories.CategoryName"
        Else
            pSql = "SELECT pimpro.ProductName as Name, pimpro.ProductNo, pimpro.ID as ProductID, 'edit' as State " & vbCrLf &
                    " FROM MobizLibraryPIMProduct pimpro " & vbCrLf &
                    " INNER JOIN LibraryItemModifierCategories as cat ON cat.ProductID = pimpro.ID " & vbCrLf &
                    " WHERE cat.ItemModifierID = " & ReadyString(vID) & vbCrLf &
                    " ORDER BY pimpro.ProductName ASC "
        End If

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function HasCategory(vID As String, vCategoryName As String, vPreferenceType As String) As Boolean

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT ID " & vbCrLf &
            " FROM LibraryItemModifierCategories" & vbCrLf

        If vPreferenceType = "category" Then
            pSql += " WHERE CategoryName = " & ReadyString(vCategoryName) & vbCrLf
        Else
            pSql += " WHERE ProductID = " & ReadyString(vCategoryName) & vbCrLf
        End If

        pSql += " AND ItemModifierID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR IsNot Nothing

    End Function

    Public Function DeleteCategories(vID As String, vCategoryNames As List(Of String), vUserID As String, vPreferenceType As String) As Integer

        Dim pSql As String = ""
        Dim pIU As InsertUpdate
        Dim pOut As Integer
        Dim cat As String = ""

        For Each pItem As String In vCategoryNames
            If cat <> "" Then
                cat &= ", "
            End If
            cat &= ReadyString(pItem)
        Next pItem

        pSql = "DELETE FROM LibraryItemModifierCategories" & vbCrLf &
            " WHERE ItemModifierID = " & ReadyString(vID) & vbCrLf

        If vPreferenceType = "category" Then
            pSql += " AND CategoryName IN (" & cat & ")"
        Else
            pSql += " AND ProductID IN (" & cat & ")"
        End If

        pOut = mProData.ExecuteSQL(pSql, True)

        pIU = New InsertUpdate("LibraryItemModifiers")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UpdatedBy") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Function GetRebates(vID As String, Optional vProductID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        If vProductID IsNot Nothing Then
            pSql = " AND LibraryItemModifierRebates.ProductID = " & ReadyString(vProductID) & vbCrLf
        End If

        pSql = " SELECT LibraryItemModifierRebates.ID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,LibraryItemModifierRebates.Amount" & vbCrLf &
            ",LibraryItemModifierRebates.ProductID" & vbCrLf &
            ",LibraryItemModifierRebates.UnitMeasure" & vbCrLf &
            ",LibraryItemModifierRebates.UnitQuantity" & vbCrLf &
            ",LibraryItemModifierRebates.Quantity" & vbCrLf &
            ",'edit' as State" & vbCrLf &
            " FROM LibraryItemModifierRebates" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryItemModifierRebates.ProductID" & vbCrLf &
            " WHERE LibraryItemModifierRebates.ItemModifierID = " & ReadyString(vID) & vbCrLf &
            pSql &
            " ORDER BY MobizLibraryPIMProduct.ProductName"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function AddRebates(vItemModifierID As String, vRebateID As String,
        vProductID As String, vAmount As Decimal,
        vUnitQuantity As Decimal, vUnitMeasure As String, vQuantity As Decimal, vUserID As String, vIsSaveAs As Boolean) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pIU = New InsertUpdate("LibraryItemModifierRebates")
        pIU("Amount") = vAmount
        pIU("ProductID") = ReadyString(vProductID)
        pIU("Amount") = vAmount
        pIU("UnitQuantity") = vUnitQuantity
        pIU("UnitMeasure") = ReadyString(vUnitMeasure)
        pIU("Quantity") = vQuantity
        If CStr2(vRebateID) = "" Or vIsSaveAs Then
            vRebateID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(vRebateID)
            pIU("ItemModifierID") = ReadyString(vItemModifierID)
            pSql = pIU.GetInsert()
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(vRebateID) & vbCrLf &
                " AND ItemModifierID = " & ReadyString(vItemModifierID))
        End If
        pCount = mProData.ExecuteSQL(pSql, True)
        If pCount = 0 Then
            Throw New Exception("vbXQG4VZ3ZLY! Nothing was updated. Probably, there are no ID-ItemModifierID match.")
        End If

        pIU = New InsertUpdate("LibraryItemModifiers")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UpdatedBy") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vItemModifierID))
        mProData.ExecuteSQL(pSql)

        Return vRebateID

    End Function

    Public Function DeleteRebates(vID As String, vProductIDs As List(Of String), vUserID As String) As Integer

        Dim pOut As Integer
        Dim pSql As String = ""
        Dim pIU As InsertUpdate

        For Each pItem As String In vProductIDs
            If pSql <> "" Then
                pSql &= ", "
            End If
            pSql &= ReadyString(pItem)
        Next pItem

        pSql = "DELETE FROM LibraryItemModifierRebates" & vbCrLf &
            " WHERE ItemModifierID = " & ReadyString(vID) & vbCrLf &
            " AND ProductID IN (" & pSql & ")"
        pOut = mProData.ExecuteSQL(pSql, True)

        pIU = New InsertUpdate("LibraryItemModifiers")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UpdatedBy") = ReadyString(vUserID)
        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Function Rename(vID As String,
        vNewName As String,
        vDescription As String,
        vUserID As String,
        ByRef oError As String,
        Optional ByRef vIsRestrictedModifier As String = "0",
        Optional ByRef vRestrictedDay As Object = Nothing,
        Optional vType As String = "",
        Optional vRequired As Boolean = False,
        Optional vIsActive As Boolean = True,
        Optional vPreferenceType As String = "categories",
        Optional vChoicesReference As String = "fromsku",
        Optional vPromoBuyQty As String = "0",
        Optional vPromoTakeQty As String = "0",
        Optional vPromoPrice As String = "0",
        Optional vPromoDiscount As String = "0",
        Optional vDateFrom As String = "",
        Optional vDateTo As String = "") As Boolean

        Dim pDR As DataRow
        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pCount As Integer
        Dim cleardays As New ArrayList()

        cleardays.Add("Monday")
        cleardays.Add("Tuesday")
        cleardays.Add("Wednesday")
        cleardays.Add("Thursday")
        cleardays.Add("Friday")
        cleardays.Add("Saturday")
        cleardays.Add("Sunday")

        'pDR = ByName(vNewName)
        'If pDR IsNot Nothing _
        '    AndAlso ReadyGuid(pDR("ID")) = ReadyGuid(vID) Then

        '    oError = "Item modifier with the name, " & vNewName & " already exist. Please choose a unique item modifier name."
        '    Return False
        'End If

        pIU = New InsertUpdate("LibraryItemModifiers")
        pIU("Name") = ReadyString(vNewName)
        pIU("Description") = ReadyString(vDescription)
        pIU("IsRequired") = ReadyString(vRequired)
        pIU("IsActive") = ReadyString(vIsActive)
        pIU("PreferenceType") = ReadyString(vPreferenceType)
        pIU("IsRestrictedModifier") = ReadyString(vIsRestrictedModifier)
        pIU("ChoicesReference") = ReadyString(vChoicesReference)
        pIU("PromoBuyQty") = ReadyString(vPromoBuyQty)
        pIU("PromoTakeQty") = ReadyString(vPromoTakeQty)
        pIU("PromoTakePrice") = IIf(vPromoPrice = "", "NULL", ReadyString(vPromoPrice))
        pIU("PromoTakeDiscount") = IIf(vPromoDiscount = "", "NULL", ReadyString(vPromoDiscount))
        pIU("DateFrom") = IIf(vDateFrom = "", "NULL", ReadyString(vDateFrom))
        pIU("DateTo") = IIf(vDateTo = "", "NULL", ReadyString(vDateTo))
        pIU("LastUpdated") = "GETDATE()"
        pIU("UpdatedBy") = ReadyString(vUserID)
        If vType = "Inventory Affecting" Or vType = "Non-Inventory Affecting" Then
            pIU("Min") = ReadyString(CInt2(Common.WebRequest.Data("Min")))
            pIU("Max") = ReadyString(CInt2(Common.WebRequest.Data("Max")))
        End If

        For Each day As Object In vRestrictedDay
            cleardays.Remove(day("Day"))

            pIU(day("Day") & "TimeFrom") = ReadyString(day("From"))
            pIU(day("Day") & "TimeTo") = ReadyString(day("To"))
        Next day

        For Each pItem As String In cleardays 'iremove ang day na wala sa napiling restricted days
            pIU(pItem & "TimeFrom") = "NULL"
            pIU(pItem & "TimeTo") = "NULL"
        Next pItem

        pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        pCount = mProData.ExecuteSQL(pSql, True)

        If pCount = 0 Then
            oError = "The item modifier that you were trying to rename no longer exists."
            Return False
        End If

        Return True

    End Function

    Public Function ByParentProductIdAndModifierProductNumber(vParentProductID As String, vModifierProductNumber As String) As DataRow

        Dim pSql = " DECLARE @ProductID uniqueidentifier = " & ReadyString(vParentProductID) & ";" & vbCrLf &
            " DECLARE @ProductNo varchar(max) = " & ReadyString(vModifierProductNumber) & ";" & vbCrLf

        pSql &= "  SELECT DISTINCT LibraryItemModifierCategories.ItemModifierID" & vbCrLf &
            "  ,LibraryItemModifierInventoryAffecting.ID AS ModifierID" & vbCrLf &
            "  FROM LibraryItemModifiers" & vbCrLf &
            "  JOIN LibraryItemModifierCategories ON LibraryItemModifierCategories.ItemModifierID = LibraryItemModifiers.ID" & vbCrLf &
            "  AND LibraryItemModifierCategories.ProductID = @ProductID" & vbCrLf &
            "  JOIN LibraryItemModifierInventoryAffecting ON LibraryItemModifierInventoryAffecting.ItemModifierID = LibraryItemModifiers.ID" & vbCrLf &
            "  JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            "  WHERE MobizLibraryPIMProduct.ProductNo = @ProductNo" & vbCrLf &
            "  AND LibraryItemModifiers.IsActive = 1" & vbCrLf &
            "  " & vbCrLf &
            "  UNION " & vbCrLf &
            "  " & vbCrLf &
            "  SELECT DISTINCT LibraryItemModifierCategories.ItemModifierID" & vbCrLf &
            "  ,LibraryItemModifierInventoryAffecting.ID AS ModifierID" & vbCrLf &
            "  FROM LibraryItemModifiers" & vbCrLf &
            "  JOIN LibraryItemModifierCategories ON LibraryItemModifierCategories.ItemModifierID = LibraryItemModifiers.ID" & vbCrLf &
            "  JOIN  MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.C0020 = LibraryItemModifierCategories.CategoryName" & vbCrLf &
            "  JOIN LibraryItemModifierInventoryAffecting ON LibraryItemModifierInventoryAffecting.ItemModifierID = LibraryItemModifiers.ID" & vbCrLf &
            "  JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            "  WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = @ProductID" & vbCrLf &
            "  AND MobizLibraryPIMProduct.ProductNo = @ProductNo" & vbCrLf &
            "  AND LibraryItemModifiers.IsActive = 1" & vbCrLf &
"  "
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class