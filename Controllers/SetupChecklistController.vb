Imports System.Threading
Imports System.Web.Services.Description
Imports Newtonsoft.Json

Namespace WebProj

    Public Class SetupChecklistController
        Inherits System.Web.Mvc.Controller
        Shared mSaveExcelFileDataThread As Thread
        Shared strExcelFileProgress As String
        Shared strMessage As String
        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function SystemAdministration() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function GeneralSettings() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function BsSystemAdministration() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function BOMProjectExcel() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function CheckIfVDMFeaturesIsActivated()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim EnableVDMFeatures As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.EnableVDMFeatures)

                pSql = " SELECT machines.* " & vbCrLf &
                ", ISNULL(NULLIF(LTRIM(RTRIM(modified.Firstname)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(modified.Surname)) + '', ''), '')  AS [ModifiedByName]" & vbCrLf &
                " FROM LibraryVendingMachines machines " & vbCrLf &
                " INNER JOIN MobizLibraryPeople modified ON machines.ModifiedBy = modified.ID " & vbCrLf &
                " ORDER BY machines.Name ASC "

                pOut.Add("EnableVDMFeatures", EnableVDMFeatures)

                If EnableVDMFeatures Then
                    pOut.Add("VDMMachines", Common.WebRequest.ProData.Execute(pSql))
                End If

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function
        Function DownloadPromoCodeTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "\FileTemplates\PromoCodeTemplate.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "\FileTemplates\PromoCodeTemplate.xlsx"
            pContentType = "application/xlsx"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "PromoCodeTemplate.xlsx")
        End Function
        Function DownloadVoucherTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "\FileTemplates\VouchersTemplate.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "\FileTemplates\VouchersTemplate.xlsx"
            pContentType = "application/xlsx"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "VouchersTemplate.xlsx")
        End Function

        Function SavePromoCampaign()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim file As Object = Request.Files
                Dim template As HttpPostedFileBase = file(0)
                Dim pFilename As String
                Dim pFS As IO.FileStream
                Dim CampaignID As String = Request.Form.Item("CampaignID")
                Dim CampaignName As String = Request.Form.Item("CampaignName")
                Dim PromoMessage As String = Request.Form.Item("PromoMessage")
                Dim PromoStart As String = Request.Form.Item("PromoStart")
                Dim PromoEnd As String = Request.Form.Item("PromoEnd")
                Dim Products As DataTable = JsonConvert.DeserializeObject(Of DataTable)(Request.Form.Item("Products"))
                Dim ismultiplepromocode As Boolean = Request.Form.Item("IsMultiplePromoCode")
                Dim strHeader = {"Promo Code", "Store Code", "Store Name"}
                Dim sourceorgid As String = WebRequest.Instance.OrgID
                Dim uploadedbyid As String = WebSession.Instance.PeopleUserID
                Dim mDatabase = Common.WebRequest.ProData.GetCurrentDatabase()

                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("MobizLibraryPromoCampaign")
                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & sourceorgid & "\"
                If Not System.IO.Directory.Exists(pSourceURL) Then
                    System.IO.Directory.CreateDirectory(pSourceURL)
                End If

                pFilename = pSourceURL & "PromoCodeTemplate.xls"
                CreateFolder(pFilename)
                template.SaveAs(pFilename)

                strExcelFileProgress = JsonDictionary.Serialize("Progress", "Validating your data. Please wait ...")

                p("CampaignName") = ReadyString(CampaignName)
                p("PromoMessage") = ReadyString(PromoMessage)
                p("PromoStart") = ReadyString(PromoStart)
                p("PromoEnd") = ReadyString(PromoEnd)
                p("IsAllowMultiplePromoCode") = ReadyString(ismultiplepromocode)
                p("LastUpdated") = "getdate()"
                p("IsActive") = 1
                p("OrgID") = ReadyString(sourceorgid)
                p("ModifiedBy") = ReadyString(uploadedbyid)

                If String.IsNullOrWhiteSpace(CampaignID) Then
                    CampaignID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(CampaignID)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(CampaignID)))
                End If

                For Each sku As DataRow In Products.Rows
                    p = New InsertUpdate("MobizLibraryPromoCampaignProducts")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("CampaignID") = ReadyString(CampaignID)
                    p("ProductID") = ReadyString(sku("ProductID"))
                    p("ProductNo") = ReadyString(sku("ProductNo"))
                    p("ProductName") = ReadyString(sku("ProductName"))

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Next

                pFS = New IO.FileStream(pFilename, IO.FileMode.Open)

                If pFS IsNot Nothing Then pFS.Dispose()
                Dim dtaExcelData As DataTable = ExcelToTable(pFilename, strHeader, "")

                If dtaExcelData Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid Template. Please upload valid template.")
                End If

                Dim uniquestorecodes As DataTable = dtaExcelData.DefaultView.ToTable(True, "Store Code")
                Dim intCntr As Integer = 0
                strMessage = ""

                'Check first if the store codes exist in the database
                For Each code As DataRow In uniquestorecodes.Rows
                    If Common.WebRequest.ProData.Execute("SELECT * FROM MobizLibraryOrganizationFOWST WHERE Code = " & ReadyString(code("Store Code"))).Rows.Count = 0 Then
                        strMessage += "Store code <b>" & code("Store Code") & "</b> does not exist.<br>"
                    End If
                Next

                While (intCntr < dtaExcelData.Rows.Count)
                    Dim dtaRow As DataRow
                    dtaRow = dtaExcelData.Rows(intCntr)
                    Dim promocode As String = dtaRow("Promo Code")
                    Dim storecode As String = dtaRow("Store Code")

                    If String.IsNullOrWhiteSpace(promocode) Then
                        strMessage += "In row " & (intCntr + 2) & ", the promo code is empty.<br>"
                    End If

                    If String.IsNullOrWhiteSpace(storecode) Then
                        strMessage += "In row " & (intCntr + 2) & ", the store code is empty.<br>"
                    End If

                    intCntr += 1
                End While

                If String.IsNullOrWhiteSpace(strMessage) Then
                    Common.WebRequest.ProData.CommitTransaction()

                    Static sSL As New Threading.SpinLock

                    strExcelFileProgress = JsonDictionary.Serialize("Progress", "Uploading data from Excel File. This might take a few moments. Please wait ...")

                    mSaveExcelFileDataThread = TimedThread(
                                            Sub()
                                                Try
                                                    SavePromoCampaignCodes(dtaExcelData, sourceorgid, uploadedbyid, mDatabase, CampaignID)
                                                Catch ex As Exception
                                                    strExcelFileProgress = JsonDictionary.Serialize("Progress", "Encountered error while uploading data from Excel File. " + Truncate(ex.Message, 300))
                                                    AppendToDebug("vbRHV9XAJMM! " & ex.Message, True, ex, True)
                                                Finally
                                                    If sSL.IsHeldByCurrentThread Then
                                                        ' do something here
                                                        sSL.Exit()
                                                    End If
                                                End Try

                                            End Sub
                                        )

                    Return JsonDictionary.Serialize("Progress", "Please wait...")
                Else
                    Dim pOut As New JsonDictionary
                    pOut.Add("Progress", "invalid")
                    pOut.Add("Advisory", strMessage)
                    strExcelFileProgress = pOut.Serialize()
                    Return strExcelFileProgress
                End If
            Catch ex As Exception
                strExcelFileProgress = JsonDictionary.Serialize("Error", ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Function SavePromoCampaignCodes(ByVal DTable As DataTable, ByVal sourceorgid As String, ByVal uploadedbyid As String, ByVal dbname As String, ByVal campaignid As String)
            Dim intCntr As Integer = 0
            strMessage = ""

            Dim pOut As New JsonDictionary
            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(dbname))

            If (DTable.Rows.Count > 0) Then
                mBusiness.ProData.BeginTransaction()

                While (intCntr < DTable.Rows.Count)
                    strExcelFileProgress = JsonDictionary.Serialize("Progress", "Uploading " & intCntr.ToString("#,##0") & " of " & DTable.Rows.Count.ToString("#,##0") & " data from Excel File. Please wait...")

                    Dim dtaRow As DataRow
                    dtaRow = DTable.Rows(intCntr)
                    Dim promocode As String = dtaRow("Promo Code")
                    Dim storecode As String = dtaRow("Store Code")
                    Dim storename As String = dtaRow("Store Name")
                    Dim fowstid As String = ""

                    Dim data As DataTable = mBusiness.ProData.Execute("SELECT ID, Code, Name FROM MobizLibraryOrganizationFOWST WHERE Code = " & ReadyString(storecode))

                    If data.Rows.Count > 0 Then
                        fowstid = data.Rows(0)("ID").ToString()
                        Dim p = New InsertUpdate("MobizLibraryPromoCampaignCode")

                        p("CampaignID") = ReadyString(campaignid)
                        p("PromoCode") = ReadyString(promocode)
                        p("FowstID") = ReadyString(fowstid)
                        p("FowstCode") = ReadyString(data.Rows(0)("Code"))
                        p("FowstName") = ReadyString(data.Rows(0)("Name"))
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("Status") = ReadyString("Unused")
                        p("DateUploaded") = "getdate()"
                        p("DateImported") = "getdate()" 'temporary para dili lang sya macount as unflushed sa app
                        mBusiness.ProData.Execute(p.GetInsert())
                    End If

                    intCntr += 1
                End While

                mBusiness.SystemDefaults.UpdateTableLastUpdated("LibraryPromoCampaign")

                mBusiness.ProData.CommitTransaction()

                pOut.Add("Progress", "success")
                pOut.Add("Advisory", strMessage)
            Else
                pOut.Add("Progress", "nodata")
            End If

            strExcelFileProgress = pOut.Serialize
        End Function

        Function UploadVouchers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim file As Object = Request.Files
                Dim strHeader = {"Serial Number", "Product Number", "Voucher Description"}
                Dim pFS As IO.FileStream
                Dim template As HttpPostedFileBase = file(0)
                Dim pFilename, pSql As String
                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\"
                Dim dtaExcelData = New DataTable
                Dim mDatabase = Common.WebRequest.ProData.GetCurrentDatabase()
                Dim sourceorgid As String = WebRequest.Instance.OrgID
                Dim uploadedbyid As String = WebSession.Instance.PeopleUserID

                If Not System.IO.Directory.Exists(pSourceURL) Then
                    System.IO.Directory.CreateDirectory(pSourceURL)
                End If

                pFilename = pSourceURL & "VouchersTemplate.xls"
                CreateFolder(pFilename)
                template.SaveAs(pFilename)

                pFS = New IO.FileStream(pFilename, IO.FileMode.Open)

                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pFilename, strHeader, "")

                If dtaExcelData Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Invalid Template. Please upload valid template.")
                End If

                Static sSL As New Threading.SpinLock

                'Do Until Monitor.TryEnter(mIndexLock)
                '    Thread.Sleep(1000)
                'Loop

                strExcelFileProgress = JsonDictionary.Serialize("Progress", "Uploading data from Excel File. This might take a few moments. Please wait ...")

                mSaveExcelFileDataThread = TimedThread(
                                        Sub()
                                            Try
                                                SaveVouchers(dtaExcelData, sourceorgid, uploadedbyid, mDatabase)
                                            Catch ex As Exception
                                                strExcelFileProgress = JsonDictionary.Serialize("Progress", "Encountered error while uploading data from Excel File. " + Truncate(ex.Message, 300))
                                                AppendToDebug("vbRHV9XAJMM! " & ex.Message, True, ex, True)
                                            Finally
                                                If sSL.IsHeldByCurrentThread Then
                                                    ' do something here
                                                    sSL.Exit()
                                                End If
                                            End Try

                                        End Sub
                                    )
                Return JsonDictionary.Serialize("Progress", "Please wait...")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message())
            End Try
        End Function

        Function SaveVouchers(ByVal DTable As DataTable, ByVal sourceorgid As String, ByVal uploadedbyid As String, ByVal dbname As String)
            Dim intCntr As Integer = 0
            strMessage = ""

            Dim pOut As New JsonDictionary
            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(dbname))

            If (DTable.Rows.Count > 0) Then
                mBusiness.ProData.BeginTransaction()

                While (intCntr < DTable.Rows.Count)
                    strExcelFileProgress = JsonDictionary.Serialize("Progress", "Uploading " & intCntr.ToString("#,##0") & " of " & DTable.Rows.Count.ToString("#,##0") & " data from Excel File. Please wait...")

                    Dim dtaRow As DataRow
                    dtaRow = DTable.Rows(intCntr)
                    Dim refno As String = dtaRow("Serial Number")
                    Dim productno As String = dtaRow("Product Number")
                    Dim voucherdescription As String = dtaRow("Voucher Description")
                    Dim rowid As String = Guid.NewGuid.ToString()

                    Dim data As DataTable = mBusiness.ProData.Execute("SELECT * FROM MobizLibraryVouchers WHERE ReferenceNo = " & ReadyString(refno))

                    If data.Rows.Count > 0 Then
                        rowid = data.Rows(0)("ID").ToString()

                        If Not CBool2(data.Rows(0)("IsActive")) Then
                            intCntr += 1
                            Continue While
                        End If
                    End If

                    If mBusiness.ProData.Execute("SELECT ID FROM MobizLibraryPimProduct WHERE ProductNo = " & ReadyString(productno)).Rows.Count = 0 Then
                        strMessage += "In row " & (intCntr + 2) & ", the product number <i>" & productno & "</i> does not exist in your product list.<br>"
                        productno = ""
                    End If

                    Dim p = New InsertUpdate("MobizLibraryVouchers")

                    p("ReferenceNo") = ReadyString(refno)

                    p("ProductCode") = IIf(productno = "", "NULL", ReadyString(productno))
                    p("VoucherDescription") = IIf(voucherdescription = "", "NULL", ReadyString(voucherdescription))
                    p("SourceOrgID") = ReadyString(sourceorgid)
                    p("UploadedByID") = ReadyString(uploadedbyid)
                    p("LastUpdated") = "getdate()"
                    p("DateImported") = "getdate()"

                    If mBusiness.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(rowid))) = 0 Then
                        p("ID") = ReadyString(rowid)
                        p("IsActive") = 1
                        mBusiness.ProData.Execute(p.GetInsert())
                    End If

                    intCntr += 1
                End While

                mBusiness.SystemDefaults.UpdateTableLastUpdated("MobizLibraryVouchers")

                mBusiness.ProData.CommitTransaction()

                pOut.Add("Progress", "success")
                pOut.Add("Advisory", strMessage)
            Else
                pOut.Add("Progress", "nodata")
            End If

            strExcelFileProgress = pOut.Serialize
        End Function

        <HttpPost()>
        Public Function UploadFollowUp() As String

            Return strExcelFileProgress

        End Function

        Private Shared mIndexLock As New Object()

        Function CheckAddLibrary()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim libname As String = Common.WebRequest.Data("LibraryName")
            Dim IsSearchRecord As Integer

            If libname = "fowst" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.FOWSTAddRecord
            ElseIf libname = "authorizeddevices" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.AuthorizedDevicesAddRecord
            ElseIf libname = "receiptsettings" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.ReceiptSettingsAddRecord
            ElseIf libname = "securityroles" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.SecurityRolesAddRecord
            ElseIf libname = "employees" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.EmployeesAddRecord
            ElseIf libname = "printerroles" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.PrinterRolesAddRecord
            ElseIf libname = "hotbuttonsettings" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.HotButtonSettingsAddRecord
            ElseIf libname = "groupsettings" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.GroupSettingsAddRecord
            ElseIf libname = "customer/clientlist" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.CustomerListAddRecord
            ElseIf libname = "supplier/vendorlist" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.SupplierListAddRecord
            ElseIf libname = "salesteam" Then
                IsSearchRecord = WebRequest.Instance.Business.SystemDefaults.SalesTeamAddRecord
            End If

            Return JsonDictionary.Serialize("IsSearchRecords", IsSearchRecord)
        End Function

        Function GetGeneralSettings()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pOut As New JsonDictionary

            pOut.Add("Fowst", WebRequest.Instance.Business.SystemDefaults.FOWSTAddRecord)
            pOut.Add("AuthorizedDevices", WebRequest.Instance.Business.SystemDefaults.AuthorizedDevicesAddRecord)
            pOut.Add("ReceiptSettings", WebRequest.Instance.Business.SystemDefaults.ReceiptSettingsAddRecord)
            pOut.Add("SecurityRoles", WebRequest.Instance.Business.SystemDefaults.SecurityRolesAddRecord)
            pOut.Add("Employees", WebRequest.Instance.Business.SystemDefaults.EmployeesAddRecord)
            pOut.Add("PrinterRoles", WebRequest.Instance.Business.SystemDefaults.PrinterRolesAddRecord)
            pOut.Add("HotButtonSettings", WebRequest.Instance.Business.SystemDefaults.HotButtonSettingsAddRecord)
            pOut.Add("GroupSettings", WebRequest.Instance.Business.SystemDefaults.GroupSettingsAddRecord)
            pOut.Add("CustomerList", WebRequest.Instance.Business.SystemDefaults.CustomerListAddRecord)
            pOut.Add("SupplierList", WebRequest.Instance.Business.SystemDefaults.SupplierListAddRecord)
            pOut.Add("SalesTeam", WebRequest.Instance.Business.SystemDefaults.SalesTeamAddRecord)
            pOut.Add("Enable2FA", WebRequest.Instance.Business.SystemDefaults.Enable2FA)
            pOut.Add("AutoApproveFranchiseeRequests", WebRequest.Instance.Business.SystemDefaults.AutoApproveFranchiseeRequests)
            pOut.Add("ShowAnnouncements", WebRequest.Instance.Business.SystemDefaults.ShowAnnouncements)
            pOut.Add("RequiredCredentialsTurnoverMonies", WebRequest.Instance.Business.SystemDefaults.RequiredCredentialsTurnoverMonies)
            pOut.Add("EnableVDMFeatures", WebRequest.Instance.Business.SystemDefaults.EnableVDMFeatures)
            pOut.Add("DownloadSKUPics", WebRequest.Instance.Business.SystemDefaults.DownloadSKUPics)
            pOut.Add("ShowMCRModulePOS", WebRequest.Instance.Business.SystemDefaults.ShowMCRModulePOS)
            pOut.Add("ShowMCRModulePriceChecker", WebRequest.Instance.Business.SystemDefaults.ShowMCRModulePriceChecker)
            pOut.Add("ShowMCRModulePO", WebRequest.Instance.Business.SystemDefaults.ShowMCRModulePO)
            pOut.Add("ShowMCRModuleSalesReturn", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleSalesReturn)
            pOut.Add("ShowMCRModuleStockReceiving", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleStockReceiving)
            pOut.Add("ShowMCRModuleStockTransfer", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleStockTransfer)
            pOut.Add("ShowMCRModuleInventoryCount", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleInventoryCount)
            pOut.Add("ShowMCRModuleRouteExpenseTracking", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleRouteExpenseTracking)
            pOut.Add("ShowMCRModuleProductMaker", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleProductMaker)
            pOut.Add("ShowMCRModuleShelfBinLabelling", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleShelfBinLabelling)
            pOut.Add("ShowMCRModuleRequestApprovals", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleRequestApprovals)
            pOut.Add("ShowMCRModuleCashHandling", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleCashHandling)
            pOut.Add("ShowMCRModuleDashboard", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleDashboard)
            pOut.Add("ShowMCRModuleReportZRead", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleReportZRead)
            pOut.Add("ShowMCRModuleReconstructSalesData", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleReconstructSalesData)
            pOut.Add("ShowMCRModuleLazyUpdate", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleLazyUpdate)
            pOut.Add("ShowMCRModuleRestoreSystem", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleRestoreSystem)
            pOut.Add("ShowMCRModuleCustomerDisplay", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleCustomerDisplay)
            pOut.Add("ShowMCRModuleOrderTakerSetup", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleOrderTakerSetup)
            pOut.Add("ShowMCRModuleSMSSetup", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleSMSSetup)
            pOut.Add("ShowMCRModuleAdjustZReadDate", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleAdjustZReadDate)
            pOut.Add("ShowMCRModuleUpdateCacheDB", WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleUpdateCacheDB)

            Return pOut.Serialize()
        End Function

        Function SaveGeneralSettings()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                WebRequest.Instance.Business.SystemDefaults.FOWSTAddRecord = Common.WebRequest.Data("Fowst")
                WebRequest.Instance.Business.SystemDefaults.AuthorizedDevicesAddRecord = Common.WebRequest.Data("AuthorizedDevices")
                WebRequest.Instance.Business.SystemDefaults.ReceiptSettingsAddRecord = Common.WebRequest.Data("ReceiptSettings")
                WebRequest.Instance.Business.SystemDefaults.SecurityRolesAddRecord = Common.WebRequest.Data("SecurityRoles")
                WebRequest.Instance.Business.SystemDefaults.EmployeesAddRecord = Common.WebRequest.Data("Employees")
                WebRequest.Instance.Business.SystemDefaults.PrinterRolesAddRecord = Common.WebRequest.Data("PrinterRoles")
                WebRequest.Instance.Business.SystemDefaults.HotButtonSettingsAddRecord = Common.WebRequest.Data("HotButtonSettings")
                WebRequest.Instance.Business.SystemDefaults.GroupSettingsAddRecord = Common.WebRequest.Data("GroupSettings")
                WebRequest.Instance.Business.SystemDefaults.CustomerListAddRecord = Common.WebRequest.Data("CustomerList")
                WebRequest.Instance.Business.SystemDefaults.SupplierListAddRecord = Common.WebRequest.Data("SupplierList")
                WebRequest.Instance.Business.SystemDefaults.SalesTeamAddRecord = Common.WebRequest.Data("SalesTeam")
                WebRequest.Instance.Business.SystemDefaults.Enable2FA = Common.WebRequest.Data("Enable2FA")
                WebRequest.Instance.Business.SystemDefaults.AutoApproveFranchiseeRequests = Common.WebRequest.Data("AutoApproveFranchiseeRequests")
                WebRequest.Instance.Business.SystemDefaults.ShowAnnouncements = Common.WebRequest.Data("ShowAnnouncements")
                WebRequest.Instance.Business.SystemDefaults.RequiredCredentialsTurnoverMonies = Common.WebRequest.Data("RequiredCredentialsTurnoverMonies")
                WebRequest.Instance.Business.SystemDefaults.EnableVDMFeatures = Common.WebRequest.Data("EnableVDMFeatures")
                WebRequest.Instance.Business.SystemDefaults.DownloadSKUPics = Common.WebRequest.Data("DownloadSKUPics")

                WebRequest.Instance.Business.SystemDefaults.ShowMCRModulePOS = Common.WebRequest.Data("ShowMCRModulePOS")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModulePriceChecker = Common.WebRequest.Data("ShowMCRModulePriceChecker")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModulePO = Common.WebRequest.Data("ShowMCRModulePO")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleSalesReturn = Common.WebRequest.Data("ShowMCRModuleSalesReturn")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleStockReceiving = Common.WebRequest.Data("ShowMCRModuleStockReceiving")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleStockTransfer = Common.WebRequest.Data("ShowMCRModuleStockTransfer")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleInventoryCount = Common.WebRequest.Data("ShowMCRModuleInventoryCount")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleRouteExpenseTracking = Common.WebRequest.Data("ShowMCRModuleRouteExpenseTracking")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleProductMaker = Common.WebRequest.Data("ShowMCRModuleProductMaker")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleShelfBinLabelling = Common.WebRequest.Data("ShowMCRModuleShelfBinLabelling")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleRequestApprovals = Common.WebRequest.Data("ShowMCRModuleRequestApprovals")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleCashHandling = Common.WebRequest.Data("ShowMCRModuleCashHandling")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleDashboard = Common.WebRequest.Data("ShowMCRModuleDashboard")

                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleReportZRead = Common.WebRequest.Data("ShowMCRModuleReportZRead")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleReconstructSalesData = Common.WebRequest.Data("ShowMCRModuleReconstructSalesData")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleLazyUpdate = Common.WebRequest.Data("ShowMCRModuleLazyUpdate")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleRestoreSystem = Common.WebRequest.Data("ShowMCRModuleRestoreSystem")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleCustomerDisplay = Common.WebRequest.Data("ShowMCRModuleCustomerDisplay")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleOrderTakerSetup = Common.WebRequest.Data("ShowMCRModuleOrderTakerSetup")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleSMSSetup = Common.WebRequest.Data("ShowMCRModuleSMSSetup")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleAdjustZReadDate = Common.WebRequest.Data("ShowMCRModuleAdjustZReadDate")
                WebRequest.Instance.Business.SystemDefaults.ShowMCRModuleUpdateCacheDB = Common.WebRequest.Data("ShowMCRModuleUpdateCacheDB")

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("SystemDefaults")

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "error")
            End Try
        End Function

        Function SaveRegionalGroup()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim rgID As String = Common.WebRequest.Data("ID")
                Dim name As String = Common.WebRequest.Data("Name")
                Dim isactive As String = CInt2(Common.WebRequest.Data("IsActive"))
                Dim groups As Object = Common.WebRequest.AllData("Groups")
                Dim p As New InsertUpdate("LibraryRegionalGroupSettings")
                Dim pSql As String

                p("RegionName") = ReadyString(name)
                p("IsActive") = isactive
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If String.IsNullOrWhiteSpace(rgID) Then
                    rgID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(rgID)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(rgID))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                For Each group In groups
                    If group("State") = "new" Then
                        p = New InsertUpdate("LibraryRegionalGroupSettingsDetails")

                        p("GroupID") = ReadyString(group("ID"))
                        p("RegionalGroupID") = ReadyString(rgID)
                        p("LastUpdated") = "getdate()"
                        p("ID") = ReadyString(Guid.NewGuid.ToString())

                        pSql = p.GetInsert()
                    ElseIf group("State") = "delete" Then
                        pSql = "DELETE FROM LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID = " & ReadyString(rgID) & vbCrLf &
                                " AND GroupID = " & ReadyString(group("ID"))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try
        End Function

        Function SavePriceLevelCustom()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim p As New InsertUpdate("MobizLibraryPIMPriceLevel")
            Dim id As String = Guid.NewGuid.ToString()

            p("PriceLevel") = ReadyString(Common.WebRequest.Data("Name"))
            p("PriceLevelType") = ReadyString(Common.WebRequest.Data("Type"))
            p("IsActive") = ReadyString(Common.WebRequest.Data("Status"))
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("OrgID") = ReadyString(WebRequest.Instance.OrgID)
            p("IsAllowedToEdit") = ReadyString(1)
            p("LastUpdated") = "GetDate()"

            If Common.WebRequest.Data("FromDate") <> "" Then
                p("FromDate") = ReadyString(Common.WebRequest.Data("FromDate"))
            Else
                p("FromDate") = "NULL"
            End If

            If Common.WebRequest.Data("ToDate") <> "" Then
                p("ToDate") = ReadyString(Common.WebRequest.Data("ToDate"))
            Else
                p("ToDate") = "NULL"
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Try
                If Common.WebRequest.Data("ID") = "" Then
                    p("ID") = ReadyString(id)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    id = Common.WebRequest.Data("ID")
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(id)))
                End If

                Common.WebRequest.ProData.CommitTransaction()
                Return Json(New With {Key .Results = "saved"})
            Catch ex As Exception
                Return Json(New With {Key .Results = ex.Message})
            End Try
            
        End Function

        Function SaveSalesRepresentativeCustom()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim p As New InsertUpdate("LibrarySalesRep")
            Dim id As String = Guid.NewGuid.ToString()

            p("Name") = ReadyString(Common.WebRequest.Data("Name"))
            p("IsActive") = ReadyString(Common.WebRequest.Data("Status"))
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "GetDate()"

            Common.WebRequest.ProData.BeginTransaction()
            Try
                If Common.WebRequest.Data("ID") = "" Then
                    p("ID") = ReadyString(id)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    id = Common.WebRequest.Data("ID")
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(id)))
                End If

                Common.WebRequest.ProData.CommitTransaction()
                Return Json(New With {Key .Results = "saved"})
            Catch ex As Exception
                Return Json(New With {Key .Results = ex.Message})
            End Try

        End Function

        Function SendSalesReport() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim header As String = "<div>Hello Juan," &
                                            "<br><br>" &
                                            "Here are the results of your group" &
                                            "<br><br>" &
                                            "Group Name: <span style='margin-left:20px;'>NCR</span><br>" &
                                            "Date: <span style='margin-left:65px;'>Monday, December 7, 2015</span>" &
                                        "</div>" &
                                        "<br>"

                Dim body As String = "<table style='width:100%;' border='0' cellpadding='3' cellspacing='0'>" &
                                        "<thead>" &
                                            "<tr style='background-color:#3498db; color:#FFF;'>" &
                                                "<th style='padding:10px;'>FOWST</th>" &
                                                "<th style='padding:10px;'>Gross Sales</th>" &
                                                "<th style='padding:10px;'>Sales Return</th>" &
                                                "<th style='padding:10px;'>Discounts</th>" &
                                                "<th style='padding:10px;'>Net Sales</th>" &
                                                "<th style='padding:10px;'>Inventory Value</th>" &
                                                "<th style='padding:10px;'>Accumulated # Of Unapproved<br> Inventory Counts</th>" &
                                                "<th style='padding:10px;'>Shortage/Overage of <br> Monies for the Day</th>" &
                                            "</tr>" &
                                        "</thead>" &
                                        "<tbody>" &
                                            "<tr style='background-color:#EEE;'>" &
                                                "<td>ABC</td>" &
                                                "<td style='text-align:right;'>1,000,000</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                                "<td style='text-align:right;'>100</td>" &
                                                "<td style='text-align:right;'>999,900</td>" &
                                                "<td style='text-align:right;'>200</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                            "</tr>" &
                                            "<tr style='background-color:#FFF;'>" &
                                                "<td>XYZ</td>" &
                                                "<td style='text-align:right;'>99,999</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                                "<td style='text-align:right;'>100</td>" &
                                                "<td style='text-align:right;'>99,899</td>" &
                                                "<td style='text-align:right;'>150</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                            "</tr>" &
                                            "<tr style='background-color:#EEE;'>" &
                                                "<td><b>GROUP TOTAL</b></td>" &
                                                "<td style='text-align:right;'>1,099,999</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                                "<td style='text-align:right;'>200</td>" &
                                                "<td style='text-align:right;'>1,099,799</td>" &
                                                "<td style='text-align:right;'>350</td>" &
                                                "<td style='text-align:right;'>0</td>" &
                                            "</tr>" &
                                        "</tbody>" &
                                    "</table>"

                Dim footer As String = "<br>" &
                                        "<div>" &
                                            "<label style='font-weight:bold; font-size:16px;'>Note:</label> <br>" &
                                            "To view more details of group performance reports, please go to:<br>" &
                                            "<a href='http://www.mobiz-apps.com'>www.mobiz-apps.com</a>" &
                                            "<br>" &
                                            "After signing in, please go to 'Dashboards and Reports'" &
                                            "<br><br><br>" &
                                            "All the best,<br>" &
                                            "The Mobiz Team" &
                                        "</div>"

                Dim msg As String = header & body & footer

#If DEBUG Then
                SendMail("customercare@mobiz-apps.com",
                 "arniepeloton17@gmail.com",
                 "Mobiz",
                 "Auto-Emailed Report",
                 msg,
                 "smtp.live.com",
                 587,
                 "customercare@mobiz-apps.com",
                 "MobizCustCare2221", , , , , True)

#Else

#End If

                Return "Sent"
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        <HttpPost()>
        Function DeviceSearch() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT As DataTable
            Dim strReturn As String
            Dim pSql As String
            Dim DeviceSearchName = Common.WebRequest.Data("SearchName")

            pSql = "SELECT * FROM LibraryDevice WHERE PhoneName LIKE '%" & DeviceSearchName & "%' OR IMEI LIKE '%" & DeviceSearchName & "%'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("AuthorizedDevice", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function FowstSearch() As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim strID As String
            Dim strReturn As String
            Dim FowstSearchName = Common.WebRequest.Data("SearchName")

            strID = WebSession.Instance.OrganizationID
            If (Common.WebRequest.Data("OrganizationID") <> Nothing) Then
                strID = Common.WebRequest.Data("OrganizationID")
            End If

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = 50

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "orgadd.ID"
                        .Searchable = False
                    End With
                    With .Add("FOWSTTypeID", SqlDbType.UniqueIdentifier)
                        .Expression = "orgadd.FOWSTTypeID"
                        .Searchable = False
                    End With
                    With .Add("Code", SqlDbType.VarChar)
                        .Expression = "orgadd.Code"
                    End With
                    With .Add("Name", SqlDbType.VarChar)
                        .Expression = "orgadd.Name"
                    End With
                    With .Add("Description", SqlDbType.VarChar)
                        .Expression = "orgadd.Description"
                    End With
                    With .Add("BusinessTradeName", SqlDbType.VarChar)
                        .Expression = "custtrade.BusinessTradeName"
                    End With
                    With .Add("CompleteFowstAddress", SqlDbType.VarChar)
                        .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgadd.StreetNumber)) + ' ', ' '), '') + " &
                                      "ISNULL(NULLIF(LTRIM(RTRIM(orgadd.Street)) + ', ',', '), '') + " &
                                      "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                      "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                    End With
                End With

                .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST orgadd " &
                           " LEFT JOIN dbo.MobizLibraryOrganizationTradeName custtrade ON (orgadd.BusinessTradeNameID = custtrade.ID) " &
                           " LEFT JOIN dbo.MobizLibraryCities cty ON (orgadd.FowstCityID = cty.ID) " &
                           " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgadd.FowstCountryID = ctry.ID) " &
                           " LEFT JOIN dbo.MobizLibraryFOWSTType rtype ON (orgadd.SubFOWSTTypeID = rtype.ID) " &
                           " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgadd.SubFOWSTTypeID = wtype.ID) " &
                           " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTPOSSettings posset ON (orgadd.ID = posset.FOWSTID) "

                .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgadd.OrganizationID IS NULL AND orgadd.FOWSTTypeID <> 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B'", " WHERE orgadd.OrganizationID = " + ReadyString(strID)) + " AND orgadd.Name LIKE '%" + FowstSearchName + "%' AND orgadd.FOWSTTypeID <> 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B'"

                .SqlOrder = " ORDER BY orgadd.Name ASC"

                pDT = .Requery
            End With

            strReturn = JsonDictionary.Serialize("FOWST", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function getModuleList() As String
            Dim pDT As DataTable
            Dim strReturn As String
            Dim pSql As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT 'Organization' as ModuleType " &
                " ,NULL AS Cnt " &
                " ,LastUpdated " &
                " ,1  as SortOrder " &
                " FROM MobizLibraryOrganization " &
                " WHERE ID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) & " AND FOWSTTypeID <> 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B' ORDER BY LastUpdated DESC) as LastUpdated " &
                ",2  as SortOrder " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) &
                "  AND FOWSTTypeID <> 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B'" 'excluding PROJECT type
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Authorized Device' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDevice ORDER BY LastUpdated DESC) as LastUpdated " &
                ",3  as SortOrder " &
                " FROM LibraryDevice "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Receipt Settings' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryReceiptSettings ORDER BY LastUpdated DESC) as LastUpdated " &
                ",4  as SortOrder " &
                " FROM LibraryReceiptSettings "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Security Access' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibrarySecurityRoleUser ORDER BY LastUpdated DESC) as LastUpdated " &
                ",5  as SortOrder " &
                " FROM LibrarySecurityRoleUser "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Employee' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationEmployee ORDER BY LastUpdated DESC) as LastUpdated " &
                ",6  as SortOrder " &
                " FROM LibraryOrganizationEmployee "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Products' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationPIM ORDER BY LastUpdated DESC) as LastUpdated " &
                ",7  as SortOrder " &
                " FROM LibraryOrganizationPIM "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Customers/Clients' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationCustomer ORDER BY LastUpdated DESC) as LastUpdated " &
                ",8  as SortOrder " &
                " FROM LibraryOrganizationCustomer "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Supplier/Vendor' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationSupplier ORDER BY LastUpdated DESC) as LastUpdated " &
                ",9  as SortOrder " &
                " FROM LibraryOrganizationSupplier "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Discount Settings' as ModultType " &
                ",NULL As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDiscountSettings ORDER BY LastUpdated Desc) as LastUpdated " &
                ",10  as SortOrder " &
                "FROM LibraryDiscountSettings "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Device Printer' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDevicePrinter ORDER BY LastUpdated Desc) as LastUpdated " &
                ",11 as SortOrder " &
                "FROM LibraryDevicePrinter"
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Hot Button' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryPimMenu ORDER BY LastUpdated Desc) as LastUpdated " &
                ",12 as SortOrder " &
                "FROM LibraryPimMenu "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Group Setting' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryFranchiseGroup ORDER BY LastUpdated Desc) as LastUpdated " &
                ",13 as SortOrder " &
                "FROM LibraryFranchiseGroup "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'CRM' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryCRMTeams ORDER BY LastUpdated Desc) as LastUpdated " &
                ",14 as SortOrder " &
                "FROM LibraryCRMTeams "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'BrandsLibrary' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryPimBrands ORDER BY LastUpdated Desc) as LastUpdated " &
                ",15 as SortOrder " &
                "FROM LibraryPimBrands "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'ProductCategories' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryPimCategories ORDER BY LastUpdated Desc) as LastUpdated " &
                ",16 as SortOrder " &
                "FROM LibraryPimCategories "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'PrinterRoles' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDevicePrinter ORDER BY LastUpdated Desc) as LastUpdated " &
                ",17 as SortOrder " &
                "FROM LibraryDevicePrinter "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Printers' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDevicePrinter ORDER BY LastUpdated Desc) as LastUpdated " &
                ",17 as SortOrder " &
                "FROM LibraryDevicePrinter "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'TableView' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryTableTemplate ORDER BY LastUpdated Desc) as LastUpdated " &
                ",18 as SortOrder " &
                "FROM LibraryTableTemplate "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'PrintSettings' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",'' as LastUpdated " &
                ",19 as SortOrder " &
                "FROM LibraryDocumentControl "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'PurchaseOrder' as ModultType " &
                ",IsRestrict " &
                ",LastUpdated as LastUpdated " &
                ",18 as SortOrder " &
                "FROM LibraryPurchaseOrderSettings " &
                " ORDER BY SortOrder"


            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("PIMChecklistData", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function getModuleRequiredDataList() As String
            Dim pDT As DataTable
            Dim strReturn As String
            Dim pSql As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT ModuleType ,RequiredData ,IsOk FROM ("
            pSql = pSql & "SELECT " &
                "'Organization' as ModuleType " &
                ",'Trade Name' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(OrganizationID) > 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationTradeName " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Name' as RequiredData " &
                ",(CASE " &
                "   WHEN RTRIM(LTRIM([Name])) <> '' THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Trade Name' as RequiredData " &
                ",(CASE " &
                "   WHEN BusinessTradeNameID IS NOT NULL THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Address' as RequiredData " &
                ",(CASE " &
                "   WHEN RTRIM(LTRIM(Street)) <> '' THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Telephone No.' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(fowsttel.FOWSTID) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST fowst" &
                " LEFT JOIN MobizLibraryOrganizationFOWSTTel fowsttel ON (fowsttel.FOWSTID = fowst.ID) " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) &
                " GROUP BY fowst.ID "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Printer' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(fowstptr.FOWSTID) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST fowst " &
                " LEFT JOIN LibraryDevicePrinterFowst fowstptr ON (fowstptr.FOWSTID = fowst.ID) " &
                " WHERE fowst.OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) &
                " GROUP BY fowst.ID "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Authorized Device' as ModuleType " &
                ",'Device' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(*) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM LibraryDevice dev"
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Authorized Device' as ModuleType " &
                ",'Device FOWST' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(devfowst.ID) <> 0 THEN 1 " &
                "	ELSE 0 " &
                " END) as IsOk " &
                " FROM LibraryDevice dev " &
                " LEFT JOIN LibraryDeviceFowst devfowst ON (dev.ID = devfowst.DeviceID) " &
                " GROUP BY dev.ID "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Receipt Settings' as ModuleType " &
                ",'Receipt Settings' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(rset.ID) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM LibraryReceiptSettings rset "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Receipt Settings' as ModuleType " &
                ",'Receipt Settings FOWST' as RequiredData " &
                ",(CASE " &
             "   WHEN COUNT(fowst.ID) <> 0 THEN 1 " &
             "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST fowst " &
                " LEFT JOIN LibraryReceiptSettingsFowst rsetfowst ON (fowst.ID = rsetfowst.FowstID) " &
                " LEFT JOIN LibraryReceiptSettings rset ON (rset.ID = rsetfowst.ReceiptSettingsID) " &
                " GROUP BY fowst.ID"
            pSql = pSql & ") test "
            pSql = pSql & " WHERE IsOk = 0 "
            pSql = pSql & " ORDER BY ModuleType "

            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("PIMChecklistData", pDT)

            Return strReturn

        End Function

        Function EmployeesEmailAdd()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim filter As String = Common.WebRequest.Data("FilterBy")
            Dim pOut As New JsonDictionary
            Dim pSql As String = " SELECT * FROM ( " & vbCrLf &
                                " SELECT " & vbCrLf &
                                "   ppol.ID, ppol.Surname + ', ' + ppol.FirstName + ' ' + ISNULL(ppol.MiddleName, '') as EmployeeName, " & vbCrLf &
                                "   ISNULL(STUFF((SELECT ', ' + EmailAddress " & vbCrLf &
                                "   FROM MobizLibraryPeopleContactEmail " & vbCrLf &
                                "   WHERE PeopleID = emp.PeopleID " & vbCrLf &
                                "   FOR XML PATH('') " & vbCrLf &
                                "   ), 1, 1,''), '') as EmailAdd " & vbCrLf &
                                "   FROM LibraryOrganizationEmployee as emp " & vbCrLf &
                                "   INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = emp.PeopleID " & vbCrLf &
                                "   WHERE emp.Deactivated = 0 " & vbCrLf &
                                " ) as E WHERE ISNULL(EmailAdd, '') " & IIf(filter = "with", "<>", "=") & " '' " & vbCrLf &
                                " ORDER BY EmployeeName " & vbCrLf
            Dim arrEmp As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For Each emp As Object In arrEmp
                pSql = "SELECT ID, EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(emp("ID"))
                emp.Add("EmailAddress", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
            Next

            pOut.Add("Employees", arrEmp)

            Return pOut.Serialize()
        End Function

        Function SaveEmpEmailAdd()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim EmployeeID As String = Common.WebRequest.Data("EmployeeID")
            Dim arrEmailAdd As Object = Common.WebRequest.AllData("EmailAddress")

            For Each email As Object In arrEmailAdd
                Dim p As New InsertUpdate("MobizLibraryPeopleContactEmail")

                p("PeopleID") = ReadyString(EmployeeID)
                p("EmailAddress") = ReadyString(email("EmailAddress"))
                p("LastUpdated") = "getdate()"

                If email.ContainsKey("State") Then
                    If email("State") = "remove" Then
                        Common.WebRequest.ProData.Execute("DELETE FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(email("ID")))
                        Continue For
                    End If
                End If

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(email("ID")))) = 0 Then
                    p("ID") = ReadyString(email("ID"))
                    p("EmailType") = ReadyString("Work Email")
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If
            Next

            Return JsonDictionary.Serialize("Results", "saved")
        End Function

        Function SaveSortedInventoryItemCategories()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")

            For Each d As Object In data
                Dim p As New InsertUpdate("LibraryPimCategories")

                p("SortOrder") = d("Order")

                Common.WebRequest.ProData.Execute(p.GetUpdate(" Category = " & ReadyString(d("Category"))))
            Next

            WebRequest.Instance.Business.SystemDefaults.MaxMobizAppDatabaseAge = 0
            WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryPimCategories")

            Return JsonDictionary.Serialize("Result", "saved")
        End Function
    End Class

End Namespace