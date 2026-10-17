Imports Newtonsoft.Json

Public Class POSSettingsController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /POSSettings

    Function Index() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function CanTransactWithUnflushedData()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOut As New JsonDictionary

        With WebRequest.Instance.Business.SystemDefaults
            pOut.Add("CanTransactWithUnflushedData", .CanTransactWithUnflushedData)
        End With

        Return pOut.Serialize
    End Function

    <HttpPost()>
    Public Function GetSystemBehavior() As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary
        Try
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableDiningMode", .IsEnableDiningMode)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableBillingOption", .IsEnableBillingOption)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("WhenDoYouWantToPrintOrderSlip", .WhenDoYouWantToPrintOrderSlip)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsPrintOrderSlip", .IsPrintOrderSlip)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsShowTableNo", .IsShowTableNo)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsShowGasPumpNo", .IsShowGasPumpNo)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("NoOfTables", .NoOfTables)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("NoOfGasPumps", .NoOfGasPumps)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableVATExemptOption", .IsEnableVATExemptOption)
                pOut.Add("EnforceZRead", If(.EnforceZRead, 1, 0))
                pOut.Add("Only1SalesRep", If(.Only1SalesRep, 1, 0))
                pOut.Add("RequireSalesRep", If(.RequireSalesRep, 1, 0))
                pOut.Add("DontAllowGreaterFreeTypePrices", If(.DontAllowGreaterFreeTypePrices, 1, 0))
                pOut.Add("AlwaysTestFreeTypeAPrice", If(.AlwaysTestFreeTypeAPrice, 1, 0))
                pOut.Add("SearchOnlyForBarcode", If(.SearchOnlyForBarcode, 1, 0))
                pOut.Add("ShowCondensedProductInformationInSearchResult", If(.ShowCondensedProductInformationInSearchResult, 1, 0))
                pOut.Add("ShowCondensedProductInformationInCart", If(.ShowCondensedProductInformationInCart, 1, 0))
                pOut.Add("ShowCondensedProductInformationInItemDetailsPage", If(.ShowCondensedProductInformationInItemDetailsPage, 1, 0))
                pOut.Add("VatPercent", .VatPercent)
                pOut.Add("Vatable", If(.Vatable, 1, 0))
                pOut.Add("PictureOfIDDurationSCPWDDiscount", If(.PictureOfIDDurationSCPWDDiscount, 1, 0))
                pOut.Add("AllowOfflineSrvPayments", If(.AllowOfflineSrvPayments, 1, 0))
                pOut.Add("MaxMobizAppDatabaseAge", .MaxMobizAppDatabaseAge)

                pOut.Add("HideBarcodeScan", If(.HideBarcodeScan, 1, 0))
                pOut.Add("HideHotButtonsButton", If(.HideHotButtonsButton, 1, 0))
                pOut.Add("HideSearchWithinResults", If(.HideSearchWithinResults, 1, 0))
                pOut.Add("PrintDispatchSlip", If(.PrintDispatchSlip, 1, 0))

                pOut.Add("IsLineItemDisplay", If(.FMCGDisplayCart, 1, 0))
                pOut.Add("RealTimePrintReceipt", If(.FMCGPrintReceipt, 1, 0))
                pOut.Add("AllItemsEligibleForSCPWDDiscount", If(.AllItemsEligibleForSCPWDDiscount, 1, 0))

                pOut.Add("IsAutoServiceCharge", If(.IsAutoServiceCharge, 1, 0))
                pOut.Add("ServiceCharge", .ServiceCharge)
                pOut.Add("ServiceChargeBasis", .ServiceChargeBasis)
                pOut.Add("CanTransactWithUnflushedData", .CanTransactWithUnflushedData)

                pOut.Add("RoundUpSellingPrices", .RoundUpSellingPrices)
                pOut.Add("RoundUpSellingPricesCentavo", .RoundUpSellingPricesCentavo)

                pOut.Add("ShowPaymentCheck", .ShowPaymentCheck)
                pOut.Add("ShowPaymentDebit", .ShowPaymentDebit)
                pOut.Add("ShowPaymentCredit", .ShowPaymentCredit)
                pOut.Add("ShowPaymentSRV", .ShowPaymentSRV)
                pOut.Add("ShowOtherPayment", .ShowOtherPayment)
                pOut.Add("ShowPaymentOnCredit", .ShowPaymentOnCredit)
                pOut.Add("ShowPaymentBillingInfo", .ShowPaymentBillingInfo)
                pOut.Add("ShowPaymentDeliveryInfo", .ShowPaymentDeliveryInfo)
                pOut.Add("ShowPaymentDiscounts", .ShowPaymentDiscounts)
                pOut.Add("ShowPaymentCharges", .ShowPaymentCharges)
                pOut.Add("ShowPaymentRewards", .ShowPaymentRewards)
                pOut.Add("ShowPaymentSalesReprensentative", .ShowPaymentSalesReprensentative)
                pOut.Add("ShowDeliveryPayment", .ShowDeliveryPayment)
                pOut.Add("UnflushedReminderMinutes", .UnflushedReminderMinutes)
                pOut.Add("UnflushedReminderMessage", .UnflushedReminderMessage)
                pOut.Add("UnflushedCountSendingInterval", .UnflushedCountSendingInterval)
            End With
            pOut.Add("OrderSlipFowsts", WebRequest.Instance.Business.LibraryOrganizationFOWST.GetByOrgID(WebRequest.Instance.OrgID, True))
        Catch ex As Exception
            pOut.Add("Error", ex.Message)
        End Try

        Return pOut.Serialize
    End Function

    Function AddOtherPaymentsToLibrary()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String = "SELECT * FROM MobizLibraryOtherPayments"

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                Dim op(13) As Object
                op(0) = New With {.Description = "AliPay", .Value = "AliPay"}
                op(1) = New With {.Description = "Complimentary", .Value = "Complimentary"}
                op(2) = New With {.Description = "Discount Voucher / On Line Deals", .Value = "Discount Voucher / On Line Deals"}
                op(3) = New With {.Description = "E-Plus", .Value = "E-Plus"}
                op(4) = New With {.Description = "Food Panda", .Value = "FoodPanda"}
                op(5) = New With {.Description = "GCash", .Value = "GCash"}
                op(6) = New With {.Description = "Gold Card", .Value = "GoldCard"}
                op(7) = New With {.Description = "Grab Food", .Value = "GrabFood"}
                op(8) = New With {.Description = "Local Delivery", .Value = "LocalDelivery"}
                op(9) = New With {.Description = "PayMaya", .Value = "PayMaya"}
                op(10) = New With {.Description = "Shopee E-Voucher", .Value = "ShopeeEVoucher"}
                op(11) = New With {.Description = "Shopee Pay", .Value = "ShopeePay"}
                op(12) = New With {.Description = "SMAC", .Value = "SMAC"}
                op(13) = New With {.Description = "V Plus", .Value = "V-Plus"}

                Dim serializer As New JavaScriptSerializer()
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To op.Length - 1
                    Dim p As New InsertUpdate("MobizLibraryOtherPayments")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("Description") = ReadyString(op(i).Description)
                    p("Value") = ReadyString(op(i).Value)

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Else
                Return JsonDictionary.Serialize("Results", "saved")
            End If
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function GetOtherPayments()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String = "SELECT * FROM MobizLibraryOtherPayments ORDER BY Description ASC"

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Function SaveNewOtherPayment()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim val As String = Common.WebRequest.Data("Description").Replace(" ", "")
            Dim p As New InsertUpdate("MobizLibraryOtherPayments")

            p("Description") = ReadyString(val)
            p("Value") = ReadyString(val)
            p("PaymentReferenceLength") = ReadyString(Common.WebRequest.Data("RefNoCharLength"))
            p("StrictReferenceCharLength") = CInt2(Common.WebRequest.Data("IsRequired"))
            p("DataType") = ReadyString(Common.WebRequest.Data("DataType"))

            If String.IsNullOrWhiteSpace(Common.WebRequest.Data("OPID")) Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                Common.WebRequest.ProData.Execute(p.GetInsert())
            Else
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("OPID"))))
            End If

            WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("MobizLibraryOtherPayments")

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    <HttpPost()>
    Public Function SaveSystemBehavior() As String

        Dim pIU As InsertUpdate
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pIU = New InsertUpdate("SystemDefaults")
        Try
            Common.WebRequest.ProData.BeginTransaction()

            With WebRequest.Instance.Business.SystemDefaults
                .IsEnableDiningMode = Common.WebRequest.Data("IsEnableDiningMode")
                .IsEnableBillingOption = Common.WebRequest.Data("IsEnableBillingOption")
                .WhenDoYouWantToPrintOrderSlip = Common.WebRequest.Data("WhenDoYouWantToPrintOrderSlip")
                .IsPrintOrderSlip = Common.WebRequest.Data("IsPrintOrderSlip")
                .IsShowTableNo = Common.WebRequest.Data("IsShowTableNo")
                .IsShowGasPumpNo = Common.WebRequest.Data("IsShowGasPumpNo")
                .IsEnableVATExemptOption = Common.WebRequest.Data("IsEnableVATExemptOption")
                .EnforceZRead = CBool2(Common.WebRequest.Data("EnforceZRead"))
                .Only1SalesRep = CBool2(Common.WebRequest.Data("Only1SalesRep"))
                .RequireSalesRep = CBool2(Common.WebRequest.Data("RequireSalesRep"))
                .DontAllowGreaterFreeTypePrices = CBool2(Common.WebRequest.Data("DontAllowGreaterFreeTypePrices"))
                .AlwaysTestFreeTypeAPrice = CBool2(Common.WebRequest.Data("AlwaysTestFreeTypeAPrice"))
                .SearchOnlyForBarcode = CBool2(Common.WebRequest.Data("SearchOnlyForBarcode"))
                .ShowCondensedProductInformationInSearchResult = CBool2(Common.WebRequest.Data("ShowCondensedProductInformationInSearchResult"))
                .ShowCondensedProductInformationInCart = CBool2(Common.WebRequest.Data("ShowCondensedProductInformationInCart"))
                .ShowCondensedProductInformationInItemDetailsPage = CBool2(Common.WebRequest.Data("ShowCondensedProductInformationInItemDetailsPage"))
                .VatPercent = CDec2(Common.WebRequest.Data("VatPercent"))
                .Vatable = CBool2(Common.WebRequest.Data("Vatable"))
                .PictureOfIDDurationSCPWDDiscount = CBool2(Common.WebRequest.Data("PictureOfIDDurationSCPWDDiscount"))
                .AllowOfflineSrvPayments = CBool2(Common.WebRequest.Data("AllowOfflineSrvPayments"))
                .MaxMobizAppDatabaseAge = CInt2(Common.WebRequest.Data("MaxMobizAppDatabaseAge"))

                .HideBarcodeScan = CBool2(Common.WebRequest.Data("HideBarcodeScan"))
                .HideHotButtonsButton = CBool2(Common.WebRequest.Data("HideHotButtonsButton"))
                .HideSearchWithinResults = CBool2(Common.WebRequest.Data("HideSearchWithinResults"))
                .PrintDispatchSlip = CBool2(Common.WebRequest.Data("PrintDispatchSlip"))

                .FMCGDisplayCart = CBool2(Common.WebRequest.Data("IsLineItemDisplay"))
                .FMCGPrintReceipt = CBool2(Common.WebRequest.Data("RealTimePrintReceipt"))
                .AllItemsEligibleForSCPWDDiscount = CBool2(Common.WebRequest.Data("AllItemsEligibleForSCPWDDiscount"))

                .IsAutoServiceCharge = CBool2(Common.WebRequest.Data("IsAutoServiceCharge"))
                .ServiceCharge = CDec2(Common.WebRequest.Data("ServiceCharge"))
                .ServiceChargeBasis = Common.WebRequest.Data("ServiceChargeBasis")
                .CanTransactWithUnflushedData = CBool2(Common.WebRequest.Data("CanTransactWithUnflushedData"))

                .RoundUpSellingPrices = Common.WebRequest.Data("RoundUpSellingPrices")
                .RoundUpSellingPricesCentavo = CInt2(Common.WebRequest.Data("RoundUpSellingPricesCentavo"))

                .ShowPaymentCheck = CInt2(Common.WebRequest.Data("ShowPaymentCheck"))
                .ShowPaymentDebit = CInt2(Common.WebRequest.Data("ShowPaymentDebit"))
                .ShowPaymentCredit = CInt2(Common.WebRequest.Data("ShowPaymentCredit"))
                .ShowPaymentSRV = CInt2(Common.WebRequest.Data("ShowPaymentSRV"))
                .ShowOtherPayment = CInt2(Common.WebRequest.Data("ShowOtherPayment"))
                .ShowPaymentOnCredit = CInt2(Common.WebRequest.Data("ShowPaymentOnCredit"))
                .ShowPaymentBillingInfo = CInt2(Common.WebRequest.Data("ShowPaymentBillingInfo"))
                .ShowPaymentDeliveryInfo = CInt2(Common.WebRequest.Data("ShowPaymentDeliveryInfo"))
                .ShowPaymentDiscounts = CInt2(Common.WebRequest.Data("ShowPaymentDiscounts"))
                .ShowPaymentCharges = CInt2(Common.WebRequest.Data("ShowPaymentCharges"))
                .ShowPaymentRewards = CInt2(Common.WebRequest.Data("ShowPaymentRewards"))
                .ShowPaymentSalesReprensentative = CInt2(Common.WebRequest.Data("ShowPaymentSalesReprensentative"))
                .ShowDeliveryPayment = CInt2(Common.WebRequest.Data("ShowDeliveryPayment"))
                .UnflushedReminderMinutes = CInt2(Common.WebRequest.Data("UnflushedReminderMinutes"))
                .UnflushedCountSendingInterval = CInt2(Common.WebRequest.Data("UnflushedCountSendingInterval"))
                .UnflushedReminderMessage = Truncate(CStr2(Common.WebRequest.Data("UnflushedReminderMessage")).Trim(), 1000)
            End With

            'save only if 
            If (Common.WebRequest.Data("IsShowTableNo") = "1") Then
                With WebRequest.Instance.Business.SystemDefaults
                    .NoOfTables = Common.WebRequest.Data("NoOfTables")
                End With
            End If

            'save only if 
            If (Common.WebRequest.Data("IsShowGasPumpNo") = "1") Then
                With WebRequest.Instance.Business.SystemDefaults
                    .NoOfGasPumps = Common.WebRequest.Data("NoOfGasPumps")
                End With
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizSystemDefaults'"

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If Common.WebRequest.AllData.ContainsKey("ConsolidateOrderSlipPrintingFowstIDs") Then
                Dim pConsolidateOrderSlipPrintingFowstIDs = Common.WebRequest.AllData("ConsolidateOrderSlipPrintingFowstIDs")
                If GetType(IEnumerable).IsAssignableFrom(pConsolidateOrderSlipPrintingFowstIDs.GetType()) Then
                    pSql = "UPDATE MobizLibraryOrganizationFOWST" & vbCrLf &
                        " SET ConsolidateOrderSlipPrinting = 0" & vbCrLf &
                        " WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                    For Each pID As String In CType(pConsolidateOrderSlipPrintingFowstIDs, IEnumerable)
                        pSql = "UPDATE MobizLibraryOrganizationFOWST" & vbCrLf &
                            " SET ConsolidateOrderSlipPrinting = 1" & vbCrLf &
                            " WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                            " AND ID = " & ReadyString(pID)
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Next pID
                End If
            End If

            Dim pOut As New JsonDictionary
            ' commented 2025_02_19__16_36_04_2912
            'Dim pModifiedPriceCount = WebRequest.Instance.Business.LibraryStock.RoundUpSellingPrice(WebRequest.Instance.OrgID)
            'pOut.Add("ModifiedPriceCount", pModifiedPriceCount)
            Common.WebRequest.ProData.CommitTransaction()

            pOut.Add("Result", "Success")
            Return pOut.Serialize()
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return JsonDictionary.Serialize("Error", ex.Message)

        End Try

    End Function
End Class
