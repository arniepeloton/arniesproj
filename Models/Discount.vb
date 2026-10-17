Public Class Discount
    Public Property ID As String
    Public Property SeniorCitizenDiscount As String
    Public Property PersonWithDisabilityDiscount As String
    Public Property SoloParentDiscount As String
    Public Property EmployeeDiscount As String
    Public Property RegularDiscount1 As String
    Public Property RegularDiscount2 As String
    Public Property RegularDiscount3 As String
    Public Property VIPDiscount As String
    Public Property GPCDiscount As String
    Public Property PNSTMDiscount As String
    Public Property AllowOneDiscountProfileOnly As String
    Public Property MaxAllowableDiscountableAmount As String
    Public Property CustomDiscounts As Object
    Public Property ShowHeadCount As Boolean

    Public Property UserID As String
    Public Property InvoiceID As String
    Public Property FowstID As String

    Function GetCustomDiscounts()
        Dim pSql As String

        pSql = "SELECT cd.*, ppol.FirstName + ' ' + ppol.Surname as UserName, " & vbCrLf &
                " CASE WHEN cd.DiscountAmount IS NULL THEN 'percentage' ELSE 'fixamount' END as Type, " & vbCrLf &
                " 'edit' as State, ISNULL(cd.DiscountPercentage, cd.DiscountAmount) as DiscountValue" & vbCrLf &
                " FROM LibraryCustomDiscounts as cd " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = cd.UserID "

        Dim row As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        For Each dt As Object In row
            pSql = "SELECT fowst.ID, fowst.Name as FowstName " & vbCrLf &
                    " FROM LibraryCustomDiscountsFowst as cdfowst " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = cdfowst.FowstID " & vbCrLf &
                    " WHERE cdfowst.CustomDiscountID = " & ReadyString(dt("ID").ToString())

            dt.Add("FowstList", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
        Next

        Return row
    End Function

    'CREATED BY Janine Aug 24, 2013 1pm
    Function SaveDiscountSettings() As Boolean
        Dim sql As String
        'Update only if value has changed

        Common.WebRequest.ProData.BeginTransaction()

        If (SeniorCitizenDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(SeniorCitizenDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Senior Citizen Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (PersonWithDisabilityDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(PersonWithDisabilityDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Person With Disability Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (SoloParentDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(SoloParentDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Solo Parent Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (EmployeeDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(EmployeeDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Employee Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (RegularDiscount1 <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(RegularDiscount1) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Regular Discount 1'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (RegularDiscount2 <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(RegularDiscount2) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Regular Discount 2'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With
        End If

        If (RegularDiscount3 <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(RegularDiscount3) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Regular Discount 3'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If
        

        If (VIPDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(VIPDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'VIP Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (GPCDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(GPCDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'GPC Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        If (PNSTMDiscount <> "no changes") Then
            sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(PNSTMDiscount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Philippine National Sports Team Discount'"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If


        If (MaxAllowableDiscountableAmount <> "no changes") Then
            Dim dtTemp As DataTable
            Dim pSql As String

            pSql = "Select * from LibraryDiscountSettings  where DiscountType = 'Max Allowable Discountable Amount Subject for SCD'"
            dtTemp = Common.WebRequest.ProData.Execute(pSql)

            If dtTemp.Rows.Count > 0 Then
                sql = "Update LibraryDiscountSettings set PercentDiscount = " & ReadyString(MaxAllowableDiscountableAmount) & ", UserID = " & ReadyString(UserID) & ", LastUpdated = getdate() where DiscountType = 'Max Allowable Discountable Amount Subject for SCD'"
            Else
                sql = "Insert into LibraryDiscountSettings(DiscountType, PercentDiscount, UserID, LastUpdated) values(" & vbCrLf &
                                                        ReadyString("Max Allowable Discountable Amount Subject for SCD") & "," & vbCrLf &
                                                        ReadyString(MaxAllowableDiscountableAmount) & "," & vbCrLf &
                                                        ReadyString(UserID) & "," & vbCrLf &
                                                        "getdate()" & ")"
            End If

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

        End If

        For Each pObj As Object In Common.WebRequest.AllData("MaxAllowableDiscountableAmountPerTradeName")
            Dim pSql As String

            pSql = "Update MobizLibraryOrganizationTradeName set MaxAllowableDiscountableAmount = " & CDec2(pObj("MaxAllowableDiscountableAmount")) & vbCrLf &
                " WHERE ID = " & ReadyString(pObj("ID"))

            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Next

        WebRequest.Instance.Business.SystemDefaults.SetSystemDefault("AllowOneDiscountProfileOnly", AllowOneDiscountProfileOnly)

        For Each pObj As Object In CustomDiscounts
            Dim pSql, cdID As String
            Dim p As New InsertUpdate("LibraryCustomDiscounts")

            p("Name") = ReadyString(pObj("Name"))
            p("Description") = ReadyString(pObj("Description"))
            p("MinPurchaseAmount") = ReadyString(pObj("MinPurchaseAmount"))

            If CStr2(pObj("Type")) = "fixamount" Then
                p("DiscountAmount") = ReadyString(pObj("DiscountValue"))
                p("DiscountPercentage") = "NULL"
            Else
                p("DiscountAmount") = "NULL"
                p("DiscountPercentage") = ReadyString(pObj("DiscountValue"))
            End If

            p("IsActive") = CInt2(pObj("IsActive"))
            p("IsApplyToAllFowst") = CInt2(pObj("IsApplyToAllFowst"))
            p("AvailabilityDateFrom") = IIf(pObj("AvailabilityDateFrom") = "", "NULL", ReadyString(pObj("AvailabilityDateFrom")))
            p("AvailabilityDateTo") = IIf(pObj("AvailabilityDateTo") = "", "NULL", ReadyString(pObj("AvailabilityDateTo")))

            If pObj("State") = "new" Then
                cdID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(cdID)
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(UserID)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            ElseIf pObj("State") = "edited" Then
                cdID = pObj("ID").ToString()
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(UserID)
                pSql = p.GetUpdate(" ID = " & ReadyString(pObj("ID").ToString()))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            ElseIf pObj("State") = "remove" Then
                cdID = pObj("ID").ToString()
                pSql = "DELETE FROM LibraryCustomDiscountsFowst WHERE CustomDiscountID = " & ReadyString(cdID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryCustomDiscounts WHERE ID = " & ReadyString(cdID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If CStr2(cdID) = "" Then
                cdID = pObj("ID").ToString()
            End If

            If Not CBool2(pObj("IsApplyToAllFowst")) And pObj("State") <> "remove" Then
                'apply only to specific fowst(s)
                Dim selectedfowst As New List(Of String)()
                For Each fowst As Object In pObj("FowstList")
                    p = New InsertUpdate("LibraryCustomDiscountsFowst")
                    p("FowstID") = ReadyString(fowst("ID").ToString())
                    selectedfowst.Add(fowst("ID").ToString())

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" CustomDiscountID = " & ReadyString(cdID) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("CustomDiscountID") = ReadyString(cdID)

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next

                pSql = "DELETE FROM LibraryCustomDiscountsFowst WHERE CustomDiscountID = " & ReadyString(cdID) & " AND FowstID NOT IN ('" & String.Join("', '", selectedfowst) & "')"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If
        Next

        sql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDiscountSettings'"
        Common.WebRequest.ProData.ExecuteSQL(sql)

        Common.WebRequest.ProData.CommitTransaction()

        Return True
    End Function

    Function getDiscountSettingsInfo(strID) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
                With .Add("DiscountType", SqlDbType.VarChar)
                    .Expression = "DiscountType"
                End With
                With .Add("PercentDiscount", SqlDbType.Float)
                    .Expression = "PercentDiscount"
                End With
            End With

            .SqlFrom = "FROM LibraryDiscountSettings "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ID IS NULL", " WHERE ID = " & ReadyString(strID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getDiscountSettings() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
                With .Add("DiscountType", SqlDbType.VarChar)
                    .Expression = "DiscountType"
                End With
                With .Add("PercentDiscount", SqlDbType.Float)
                    .Expression = "PercentDiscount"
                End With
            End With

            .SqlFrom = "FROM LibraryDiscountSettings"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getMaxAllowableDiscountableAmount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "BusinessTradeName"
                End With
                With .Add("MaxAllowableDiscountableAmount", SqlDbType.Decimal)
                    .Expression = "MaxAllowableDiscountableAmount"
                End With
            End With


            .SqlFrom = "FROM MobizLibraryOrganizationTradeName"

            .SqlWhere = " WHERE OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetSSDItemsWithVatExempt()
        Dim pOut As New JsonDictionary

        Try
            Dim IsEverythingEligibleForSCD As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.AllItemsEligibleForSCPWDDiscount)
            Dim pSql As String
            Dim pDt As DataTable
            Dim total As Decimal = 0
            Dim DiscountedPriceAction As String

            pSql = "Select TransInvoiceItems.ID as ID, TransInvoiceItems.ProductID, SubTotal as Total, MaxAllowableDiscountableAmount*TransInvoiceItems.Quantity as MaxAllowableDiscountableAmount, DiscountedPriceAction, ItemBasedMaxAllowableDiscount " +
                    " from TransInvoiceItems " +
                    " inner join LibraryOrganizationPIM " +
                    " ON LibraryOrganizationPIM.ProductID = TransInvoiceItems.ProductID " +
                    " where InvoiceID = " + ReadyString(InvoiceID)

            If Not IsEverythingEligibleForSCD Then
                pSql += " And LibraryOrganizationPIM.IsVatExempt = 1 "
            End If

            pDt = Common.WebRequest.ProData.Execute(pSql)

            Dim ItemBasedMaxAllowableDiscount As String = CStr2(pDt.Rows(0)("ItemBasedMaxAllowableDiscount"))

            If Not String.IsNullOrWhiteSpace(ItemBasedMaxAllowableDiscount) Then
                Dim priceLevelID As String = Common.WebRequest.ProData.ExecuteScalar("Select Top 1 DefaultSellingPriceLevel from LibraryOrganizationFranchisee where FranchiseeOrgFowstID=" & ReadyString(FowstID))

                If Not String.IsNullOrWhiteSpace(priceLevelID) Then
                    total = CDec2(GetProductPrice(ItemBasedMaxAllowableDiscount, "", priceLevelID, WebRequest.Instance.OrgID))
                End If
            Else
                For Each item As Object In pDt.Rows
                    total += CDec2(IIf(CDec2(item("MaxAllowableDiscountableAmount")) > 0, CDec2(item("MaxAllowableDiscountableAmount")), item("Total")))
                    DiscountedPriceAction = CStr2(item("DiscountedPriceAction"))
                Next
            End If

            pOut.Add("Total", total)
            pOut.Add("DiscountedPriceAction", DiscountedPriceAction)
        Catch ex As Exception
            pOut.Add("Error", ex.Message)
        End Try

        Return pOut.Serialize()
    End Function

    Function GetProductPrice(ByVal ProductID As String, ByVal PricelevelText As String, ByVal PriceLevelID As String, ByVal OrgID As String)
        Dim pSql As String = ""
        Dim pDt As DataTable

        If Not IsGuid(PriceLevelID) Then
            pSql = WebRequest.Instance.Business.SystemDefaults.GetSystemDefault(PricelevelText)
        End If

        If Not IsGuid(pSql) Then
            pSql = PriceLevelID
        End If

        pSql = "SELECT Price " +
                ",LastUpdated" +
                " FROM LibraryPIMProductPriceLevel " +
                " WHERE PriceLevelID =" + ReadyString(ReadyGuid(pSql)) +
                " AND ProductID = " + ReadyString(ReadyGuid(ProductID)) +
                " AND OrganizationID = " + ReadyString(ReadyGuid(OrgID))

        pDt = Common.WebRequest.ProData.Execute(pSql)

        If pDt.Rows.Count > 0 Then
            Return CDec2(pDt.Rows(0)("Price"))
        Else
            Return 0
        End If
    End Function
End Class
