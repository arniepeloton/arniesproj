Imports Newtonsoft.Json

Namespace WebProj
    Public Class SystemBehaviorSettingsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /SystemBehaviorSettings

        Function Index() As ActionResult
            Return View()
        End Function

        Function SalesOrder() As ActionResult
            Return View()
        End Function

        Function SaveReceivingBehavior() As String
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pIU = New InsertUpdate("SystemDefaults")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    '.IsAutomaticallyReceivedStocks = Common.WebRequest.Data("IsAutomaticallyReceivedStocks")
                    .IsShowQuantities = Common.WebRequest.Data("IsShowQuantities")
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    .IsThirdPartyCompanyOrOrganization = Common.WebRequest.Data("IsThirdPartyCompanyOrOrganization")
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    .IsExternalInReceiveIn = Common.WebRequest.Data("IsExternalInReceiveIn")
                End With

                WebRequest.Instance.Business.SystemDefaults.SRDefaultSourceDocType = Common.WebRequest.Data("SRDefaultSourceDocType")
                WebRequest.Instance.Business.SystemDefaults.IsReceiveInInternalSetToDefault = Common.WebRequest.Data("IsReceiveInInternalSetToDefault")
                WebRequest.Instance.Business.SystemDefaults.SRUseThirdPartyAsSource = Common.WebRequest.Data("SRUseThirdPartyAsSource")
                WebRequest.Instance.Business.SystemDefaults.SRThirdPartySupplier = Common.WebRequest.Data("SRThirdPartySupplier")
                WebRequest.Instance.Business.SystemDefaults.SRThirdPartySupplierOutlet = Common.WebRequest.Data("SRThirdPartySupplierOutlet")
                WebRequest.Instance.Business.SystemDefaults.AllowSRBadCount = Common.WebRequest.Data("AllowSRBadCount")
                WebRequest.Instance.Business.SystemDefaults.RemindUserAboutTheDraftTransactionsStockRecTrans = Common.WebRequest.Data("RemindUserAboutTheDraftTransactionsStockRecTrans")
                WebRequest.Instance.Business.SystemDefaults.RecTransPackSizeDiscprepancy = Common.WebRequest.Data("RecTransPackSizeDiscprepancy")

                'With WebRequest.Instance.Business.SystemDefaults
                '    .OrganizationReceiveFrom = Common.WebRequest.Data("OrganizationReceiveFrom")
                'End With

                Dim SKUList As Object = Common.WebRequest.AllData("SKUList")
                Dim skuids As New List(Of String)
                For Each sku As Object In SKUList
                    pIU = New InsertUpdate("LibraryOrganizationPIM")

                    pIU("StockReceivingRankingNo") = CInt(sku("OrderNo"))
                    pIU("StockTransferRankingNo") = CInt(sku("OrderNo"))
                    pIU("LastUpdated") = "getdate()"
                    skuids.Add(sku("ID"))
                    Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ProductID = " & ReadyString(sku("ID"))))
                Next

                If skuids.Count > 0 Then
                    pIU = New InsertUpdate("LibraryOrganizationPIM")
                    pIU("StockReceivingRankingNo") = "NULL"
                    pIU("StockTransferRankingNo") = "NULL"
                    Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ProductID NOT IN ('" & String.Join("', '", skuids) & "')"))
                End If

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()


                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try
        End Function

        Function SaveInventoryBehavior() As String
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pIU = New InsertUpdate("SystemDefaults")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    '.IsAutomaticallyReceivedStocks = Common.WebRequest.Data("IsAutomaticallyReceivedStocks")
                    .IsAutomaticEffect = Common.WebRequest.Data("IsAutomaticEffect")
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    '.IsAutomaticallyReceivedStocks = Common.WebRequest.Data("IsAutomaticallyReceivedStocks")
                    .IsAllowDecimalinInventoryCount = Common.WebRequest.Data("IsAllowDecimalinInventoryCount")
                End With

                WebRequest.Instance.Business.SystemDefaults.DefaultInventoryOption = Common.WebRequest.Data("DefaultInventoryOption")
                WebRequest.Instance.Business.SystemDefaults.ShowDefaulUomInventoryCount = Common.WebRequest.Data("ShowDefaulUomInventoryCount")
                WebRequest.Instance.Business.SystemDefaults.AllowBasedUomInventoryCount = Common.WebRequest.Data("AllowBasedUomInventoryCount")
                WebRequest.Instance.Business.SystemDefaults.DontAllowViewZreadWhenNoInventory = Common.WebRequest.Data("DontAllowViewZreadWhenNoInventory")

                Dim SKUList As Object = Common.WebRequest.AllData("SKUList")
                Dim skuids As New List(Of String)
                For Each sku As Object In SKUList
                    pIU = New InsertUpdate("LibraryOrganizationPIM")

                    pIU("InventoryRankingNo") = CInt(sku("OrderNo"))
                    pIU("LastUpdated") = "getdate()"
                    skuids.Add(sku("ID"))
                    Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ProductID = " & ReadyString(sku("ID"))))
                Next

                If skuids.Count > 0 Then
                    pIU = New InsertUpdate("LibraryOrganizationPIM")
                    pIU("InventoryRankingNo") = "NULL"
                    Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ProductID NOT IN ('" & String.Join("', '", skuids) & "')"))
                End If

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()


                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try
        End Function

        Function SaveDailyBeginningBalanceSettings() As String
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pIU = New InsertUpdate("SystemDefaults")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    .IsSaveDailyBeginningBalance = Common.WebRequest.Data("IsSaveDailyBeginningBalance")
                End With

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()


                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try
        End Function

        Function SaveSalesOrderBehavior() As String
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pIU = New InsertUpdate("SystemDefaults")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    .IsEnableInventoryCountMode = Common.WebRequest.Data("IsEnableInventoryCountMode")
                    .RequireSignature = Common.WebRequest.Data("RequireSignature")
                    .RequireCustomerOutlet = CBool2(Common.WebRequest.Data("RequireCustomerOutlet"))
                    .RequireCustomerDepartment = CBool2(Common.WebRequest.Data("RequireCustomerDepartment"))
                    .ShowQtyAvailableToCommit = CBool2(Common.WebRequest.Data("ShowQtyAvailableToCommit"))
                End With

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()


                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try
        End Function

        Function SaveLoyaltyProgram() As String
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pIU = New InsertUpdate("LibraryLoyaltyProgram")
            Try
                pIU("PointToCurrency") = CDec2(Common.WebRequest.Data("PointToCurrency"))
                pIU("Cash") = CDec2(Common.WebRequest.Data("Cash"))
                pIU("CashPoint") = CDec2(Common.WebRequest.Data("CashPoint"))
                pIU("Cheque") = CDec2(Common.WebRequest.Data("Cheque"))
                pIU("ChequePoint") = CDec2(Common.WebRequest.Data("ChequePoint"))
                pIU("DebitCard") = CDec2(Common.WebRequest.Data("DebitCard"))
                pIU("DebitCardPoint") = CDec2(Common.WebRequest.Data("DebitCardPoint"))
                pIU("CreditCard") = CDec2(Common.WebRequest.Data("CreditCard"))
                pIU("CreditCardPoint") = CDec2(Common.WebRequest.Data("CreditCardPoint"))
                pIU("SRV") = CDec2(Common.WebRequest.Data("SRV"))
                pIU("SRVPoint") = CDec2(Common.WebRequest.Data("SRVPoint"))
                pIU("GiftCertificate") = CDec2(Common.WebRequest.Data("GiftCertificate"))
                pIU("GiftCertificatePoint") = CDec2(Common.WebRequest.Data("GiftCertificatePoint"))

                If CStr2(Common.WebRequest.Data("ID")) = vbNullString Then
                    pSql = pIU.GetInsert()
                Else
                    pSql = pIU.GetUpdate("ID=" & ReadyString(CStr2(Common.WebRequest.Data("ID"))))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try
        End Function

        Function GetLoyaltyPointSetting() As String
            Dim pDT As DataTable
            Dim pSql As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select * from LibraryLoyaltyProgram"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Result", pDT)

        End Function

        Function GetSalesOrderBehavior() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pOut As JsonDictionary
            pOut = New JsonDictionary
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableInventoryCountMode", .IsEnableInventoryCountMode)
                pOut.Add("RequireSignature", .RequireSignature)
                pOut.Add("RequireCustomerOutlet", .RequireCustomerOutlet)
                pOut.Add("RequireCustomerDepartment", .RequireCustomerDepartment)
                pOut.Add("ShowQtyAvailableToCommit", .ShowQtyAvailableToCommit)
            End With

            Return pOut.Serialize


        End Function

        Function GetReceivingAndTransferOutBehavior() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim pOut As JsonDictionary
                pOut = New JsonDictionary

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsAutomaticallyReceivedStocks", .IsAutomaticallyReceivedStocks)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsShowQuantities", .IsShowQuantities)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsThirdPartyCompanyOrOrganization", .IsThirdPartyCompanyOrOrganization)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsExternalInReceiveIn", .IsExternalInReceiveIn)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsReceiveInInternalSetToDefault", .IsReceiveInInternalSetToDefault)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("AllowSRBadCount", .AllowSRBadCount)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("RemindUserAboutTheDraftTransactionsStockRecTrans", .RemindUserAboutTheDraftTransactionsStockRecTrans)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("RecTransPackSizeDiscprepancy", .RecTransPackSizeDiscprepancy)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("SRDefaultSourceDocType", .SRDefaultSourceDocType)
                End With

                Dim SRUseThirdPartyAsSource As String = WebRequest.Instance.Business.SystemDefaults.SRUseThirdPartyAsSource

                pOut.Add("SRUseThirdPartyAsSource", SRUseThirdPartyAsSource)

                If CBool2(SRUseThirdPartyAsSource) Then
                    Dim SRThirdPartySupplier As String = WebRequest.Instance.Business.SystemDefaults.SRThirdPartySupplier
                    Dim SRThirdPartySupplierOutlet As String = WebRequest.Instance.Business.SystemDefaults.SRThirdPartySupplierOutlet

                    pOut.Add("SRThirdPartySupplier", SRThirdPartySupplier)
                    pOut.Add("SRThirdPartySupplierName", Common.WebRequest.ProData.ExecuteScalar("SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = " & ReadyString(SRThirdPartySupplier)))
                    pOut.Add("SRThirdPartySupplierOutlet", SRThirdPartySupplierOutlet)
                    pOut.Add("SRThirdPartySupplierOutletName", Common.WebRequest.ProData.ExecuteScalar("SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(SRThirdPartySupplierOutlet)))
                End If

                Dim pSql As String = "SELECT orgpim.ProductID as ID, pim.ProductName, pim.ProductNo, orgpim.StockReceivingRankingNo as OrderNo " & vbCrLf &
                                    " FROM LibraryOrganizationPIM as orgpim " & vbCrLf &
                                    " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = orgpim.ProductID " & vbCrLf &
                                    " WHERE ISNULL(StockReceivingRankingNo, 0) <> 0 " & vbCrLf &
                                    " ORDER BY StockReceivingRankingNo ASC "

                pOut.Add("SKUList", Common.WebRequest.ProData.Execute(pSql))

                'With WebRequest.Instance.Business.SystemDefaults
                '    pOut.Add("OrganizationReceiveFrom", .OrganizationReceiveFrom)
                'End With

                Return pOut.Serialize
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetInventoryCountBehavior() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim pOut As JsonDictionary
                pOut = New JsonDictionary

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsAutomaticEffect", .IsAutomaticEffect)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsAllowDecimalinInventoryCount", .IsAllowDecimalinInventoryCount)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("DefaultInventoryOption", .DefaultInventoryOption)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("ShowDefaulUomInventoryCount", .ShowDefaulUomInventoryCount)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("AllowBasedUomInventoryCount", .AllowBasedUomInventoryCount)
                End With

                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("DontAllowViewZreadWhenNoInventory", .DontAllowViewZreadWhenNoInventory)
                End With

                Dim pSql As String = "SELECT orgpim.ProductID as ID, pim.ProductName, pim.ProductNo, orgpim.InventoryRankingNo as OrderNo " & vbCrLf &
                                    " FROM LibraryOrganizationPIM as orgpim " & vbCrLf &
                                    " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = orgpim.ProductID " & vbCrLf &
                                    " WHERE ISNULL(InventoryRankingNo, 0) <> 0 " & vbCrLf &
                                    " ORDER BY InventoryRankingNo ASC"

                pOut.Add("SKUList", Common.WebRequest.ProData.Execute(pSql))

                'With WebRequest.Instance.Business.SystemDefaults
                '    pOut.Add("IsShowQuantities", .IsShowQuantities)
                'End With

                Return pOut.Serialize
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Function GetDailyBeginningBalanceSettings() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pOut As JsonDictionary
            pOut = New JsonDictionary

            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsSaveDailyBeginningBalance", .IsSaveDailyBeginningBalance)
            End With

            'With WebRequest.Instance.Business.SystemDefaults
            '    pOut.Add("IsShowQuantities", .IsShowQuantities)
            'End With

            Return pOut.Serialize
        End Function

        Function ReceivingAndTransferOut() As ActionResult
            Return View()
        End Function

        Function InventoryCount() As ActionResult
            Return View()
        End Function

        Function DailyBeginningBalance() As ActionResult
            Return View()
        End Function

        Function PrintSettings() As ActionResult
            Return View()
        End Function

        Function GetListOfInvoiceTypes()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT LibraryDocumentControl.ID " & vbCrLf &
                                " ,LibraryDocumentControl.DocumentName " & vbCrLf &
                                " ,LibraryDocumentControl.PrinterName " & vbCrLf &
                                " FROM LibraryDocumentControl " & vbCrLf &
                                " WHERE LibraryDocumentControl.ModuleSource = 'Revenue > Invoicing' " & vbCrLf &
                                " ORDER BY LibraryDocumentControl.DocumentName "

            Return JsonDictionary.Serialize("Result", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function SavePrintSettings()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    .IsAutoPrintOrders = Common.WebRequest.Data("IsAutoPrintOrders")
                End With

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try
        End Function

        Function GetPrinterSettings()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsAutoPrintOrders", .IsAutoPrintOrders)
            End With

            Return pOut.Serialize()
        End Function

        Public Function PurchaseOrder() As ActionResult
            Return View()
        End Function

        Function LoyaltyProgram() As ActionResult
            Return View()
        End Function

        <HttpPost>
        Public Function SavePurchaseOrderSettings() As String

            Dim pJson As PurchaseOrderSettingsJson
            Dim pError As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pJson = JsonConvert.DeserializeObject(Of PurchaseOrderSettingsJson)(Common.WebRequest.InputStreamText)
                If pJson.Verify(pError) Then
                    With WebRequest.Instance.Business.SystemDefaults
                        .HideSomeSopaFields = CBool2(pJson.HideSomeSopaFields)
                        .DeliveryMethodWhenAutoPO = pJson.DeliveryMethodWhenAutoPO

                        .PurchaseOrderSupplierSetting = pJson.SupplierSetting
                        .PurchaseOrderSupplierID = pJson.SupplierID
                        .PurchaseOrderSupplierFowstID = pJson.SupplierFowstID

                        .PurchaseOrderCustomerSetting = pJson.CustomerSetting
                        .PurchaseOrderCustomerID = pJson.CustomerID
                        .PurchaseOrderCustomerFowstID = pJson.CustomerFowstID

                        .UpdateTableLastUpdated("SystemDefaults")
                    End With
                    Return JsonDictionary.Serialize("Result", "ok")
                Else
                    Return JsonDictionary.Serialize("Error", pError)
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function GetPurchaseOrderSettings() As String

            Dim pJson As PurchaseOrderSettingsJson
            Dim pError As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pJson = New PurchaseOrderSettingsJson
                With WebRequest.Instance.Business.SystemDefaults
                    pJson.HideSomeSopaFields = .HideSomeSopaFields
                    pJson.DeliveryMethodWhenAutoPO = .DeliveryMethodWhenAutoPO

                    pJson.SupplierSetting = .PurchaseOrderSupplierSetting
                    pJson.SupplierID = .PurchaseOrderSupplierID
                    pJson.SupplierFowstID = .PurchaseOrderSupplierFowstID

                    pJson.CustomerSetting = .PurchaseOrderCustomerSetting
                    pJson.CustomerID = .PurchaseOrderCustomerID
                    pJson.CustomerFowstID = .PurchaseOrderCustomerFowstID
                End With
                pJson.FillNames()
                Return JsonConvert.SerializeObject(pJson)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Public Class PurchaseOrderSettingsJson
            Public HideSomeSopaFields As String
            Public DeliveryMethodWhenAutoPO As String

            Public SupplierSetting As String
            Public SupplierID As String
            Public SupplierName As String
            Public SupplierFowstID As String
            Public SupplierFowstName As String

            Public CustomerSetting As String
            Public CustomerID As String
            Public CustomerName As String
            Public CustomerFowstID As String
            Public CustomerFowstName As String

            Public Function Verify(ByRef oError As String) As Boolean

                Dim pSupplierSetting As String
                Dim pSupplierID As String
                Dim pCustomerSetting As String
                Dim pCustomerID As String

                oError = Nothing

                pSupplierSetting = CStr2(SupplierSetting).ToLower()
                pSupplierID = CStr2(SupplierID)
                Select Case pSupplierSetting
                    Case "", "no default"

                    Case "default to", "locked to"
                        If String.IsNullOrWhiteSpace(pSupplierID) Then
                            oError &= "SupplierID must not be blank because SupplierSetting = " & pSupplierSetting & ". "
                        End If
                        If IsGuid(pSupplierID) = False Then
                            oError &= "SupplierID must be GUID because SupplierSetting = " & pSupplierSetting & ". "
                        End If
                End Select

                pCustomerSetting = CStr2(CustomerSetting).ToLower()
                pCustomerID = CStr2(CustomerID)
                Select Case pCustomerSetting
                    Case "", "no default"

                    Case "default to", "locked to"
                        If String.IsNullOrWhiteSpace(pCustomerID) Then
                            oError &= "CustomerID must not be blank because CustomerSetting = " & pSupplierSetting & ". "
                        End If
                        If IsGuid(pCustomerID) = False Then
                            oError &= "CustomerID must be GUID because CustomerSetting = " & pSupplierSetting & ". "
                        End If
                End Select

                Return oError Is Nothing

            End Function

            Public Sub FillNames()

                Dim pDR As DataRow

                pDR = Nothing
                If IsGuid(SupplierID) Then
                    pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(SupplierID)
                End If
                If pDR Is Nothing Then
                    SupplierName = Nothing
                    SupplierFowstID = Nothing
                    SupplierFowstName = Nothing
                Else
                    SupplierName = CStr2(pDR("CompanyName"))

                    pDR = Nothing
                    If IsGuid(SupplierFowstID) Then
                        pDR = WebRequest.Instance.Business.LibraryOrganizationFOWST.ByID(SupplierFowstID)
                    End If
                    If pDR Is Nothing Then
                        SupplierFowstID = Nothing
                        SupplierFowstName = Nothing
                    Else
                        SupplierFowstName = CStr2(pDR("Name"))
                    End If
                End If

                pDR = Nothing
                If IsGuid(CustomerID) Then
                    pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(CustomerID)
                End If
                If pDR Is Nothing Then
                    CustomerName = Nothing
                    CustomerFowstID = Nothing
                    CustomerFowstName = Nothing
                Else
                    CustomerName = CStr2(pDR("CompanyName"))

                    pDR = Nothing
                    If IsGuid(CustomerFowstID) Then
                        pDR = WebRequest.Instance.Business.LibraryOrganizationFOWST.ByID(CustomerFowstID)
                    End If
                    If pDR Is Nothing Then
                        CustomerFowstID = Nothing
                        CustomerFowstName = Nothing
                    Else
                        CustomerFowstName = CStr2(pDR("Name"))
                    End If
                End If

            End Sub
        End Class

        <HttpPost>
        Public Function GetErpSettings() As String

            Dim pJson As JsonDictionary
            Dim pPriceLevelID As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pJson = New JsonDictionary()
                pPriceLevelID = WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelID
                If IsGuid(pPriceLevelID) Then
                    pJson.Add("DefaultPriceLevel", pPriceLevelID)
                    pJson.Add("DefaultPriceLevelName", WebRequest.Instance.Business.SystemDefaults.GetPriceLevelName(pPriceLevelID))
                End If
                pJson.Add("AvailableCreditLimitComputation", WebRequest.Instance.Business.SystemDefaults.AvailableCreditLimitComputation)
                Return pJson.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function SaveErpSettings() As String

            Dim pJson As JsonDictionary
            Dim pPriceLevelID As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pPriceLevelID = Common.WebRequest.Data("DefaultPriceLevelID")
                If String.IsNullOrWhiteSpace(pPriceLevelID) = False Then
                    If IsGuid(pPriceLevelID) Then
                        WebRequest.Instance.Business.SystemDefaults.DefaultPriceLevelID = pPriceLevelID
                    Else
                        Return JsonDictionary.Serialize("Error", "Invalid DefaultPriceLevelID: " & pPriceLevelID)
                    End If
                End If
                WebRequest.Instance.Business.SystemDefaults.AvailableCreditLimitComputation = Common.WebRequest.Data("AvailableCreditLimitComputation")

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Public Class ProductMakerSettingsJson
            Public HotButtonTemplates As DataTable
            Public DefaultHotButtonTemplateID As String

            Public Function Verify(ByRef oError As String) As Boolean

                oError = Nothing
                If CStr2(DefaultHotButtonTemplateID).Trim() <> "" _
                    AndAlso IsGuid(DefaultHotButtonTemplateID) = False Then

                    oError = "DefaultHotButtonTemplateID must be a GUID."
                    Return False
                End If

                Return True

            End Function
        End Class

        <HttpPost>
        Public Function GetProductMakerSettings() As String

            Dim pJson As ProductMakerSettingsJson
            Dim pError As String
            Dim clsOrganization As New Organization

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                clsOrganization.OrganizationID = WebRequest.Instance.OrgID
                clsOrganization.PageNoOfRows = 500
                pJson = New ProductMakerSettingsJson
                With WebRequest.Instance.Business.SystemDefaults
                    pJson.DefaultHotButtonTemplateID = .ProductMakerDefaultHotButtonTemplateID
                    pJson.HotButtonTemplates = clsOrganization.GetHotButton()
                End With

                Return JsonConvert.SerializeObject(pJson)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function SaveProductMakerSettings() As String

            Dim pJson As ProductMakerSettingsJson
            Dim pError As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pJson = JsonConvert.DeserializeObject(Of ProductMakerSettingsJson)(Common.WebRequest.InputStreamText)
                If pJson.Verify(pError) Then
                    With WebRequest.Instance.Business.SystemDefaults
                        .ProductMakerDefaultHotButtonTemplateID = pJson.DefaultHotButtonTemplateID
                        .UpdateTableLastUpdated("SystemDefaults")
                    End With
                    Return JsonDictionary.Serialize("Result", "ok")
                Else
                    Return JsonDictionary.Serialize("Error", pError)
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Function SaveMaxQty()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim sku As Object = Common.WebRequest.AllData("data")

                Common.WebRequest.ProData.BeginTransaction()

                For Each data As Object In sku
                    Dim pIU As New InsertUpdate("LibraryOrganizationPIM")

                    pIU("MaxQtyToBeRcvTfer") = data("MaxQtyToBeRcvTfer")

                    Common.WebRequest.ProData.Execute(pIU.GetUpdate(" ProductID = " & ReadyString(data("ID"))))
                Next

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryOrganizationPIM")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try
        End Function
    End Class
End Namespace