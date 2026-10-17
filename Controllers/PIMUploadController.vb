Imports Aspose.Cells
Imports System.Threading

Namespace WebProj

    Public Class PIMUploadController
        Inherits System.Web.Mvc.Controller

        Dim mProData As ProData = Common.WebRequest.ProData

        Public Property MaxReturnRow As Integer = 50

        Shared mDatabase As String
        Shared vPeopleUserID As String
        Shared strExcelFileProgress As String
        Shared isSameProduct As Boolean
        Shared mSaveExcelFileDataThread As Thread

        ' 2016_01_18__13_43_23_2785 added by arvin
        Const mDefaultCategory As String = "Uncategorized"
        Const mDefaultBrand As String = "No brand"
        Const mDefaultPackingSizeUnit As String = "PC"
        Const mDefaultPackingSizeQty As Decimal = 1
        Const mDefaultCostFromVendor As Decimal = 0
        Const mDefaultShippingCost As Decimal = 0
        Const mDefaultLandedCost As Decimal = 0
        Const mThresholdToSavePrice As Decimal = 0.001

        Private Property IsAsset As Boolean = False
        Private Property WebFormIsInventoryCountItem As Boolean = Nothing

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMWebForm() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function DownloadFailedTemplate() As ActionResult
            Dim pFileTitle As String
            Dim pContentType As String
            Dim pSourceURL As String

            pFileTitle = WebRequest.Instance().OrgID & "_PIMTemplate-Errors.xls"
            pSourceURL = Common.ServerPath & "/temp/PIMTemplate/" & pFileTitle
            pContentType = "application/xls"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        Function ReActivateProducts(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String) As String
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim pFilename As String
            'Dim pFS As IO.FileStream
            'Dim pWB As Workbook
            'Dim pWS As Worksheet
            'Dim pOut As JsonDictionary
            Dim dtaExcelData As New DataTable
            Dim objRow As Object
            Dim intCntr As Integer
            Dim pSql As String
            Dim strHeader(0) As String
            Dim vDBaseName As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pSql = "Select [Database] from LibrarySubscription where OrganizationID = " & ReadyString(OrgID)

            vDBaseName = Common.WebRequest.ProData.ExecuteScalar(pSql)

            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(vDBaseName))

            If (Not objFile Is Nothing) Then
                pFilename = pSourceURL & "\PIMTemplate\" & OrgID & "_ReActivatePIM.xls"
                CreateFolder(pFilename)
                objFile.SaveAs(pFilename)

                Try
                    'pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrgID & "_ReActivatePIM.xls", IO.FileMode.Open)
                    'pWB = New Workbook
                    'pWB.Open(pFS)
                    'pWS = pWB.Worksheets(0)

                    strHeader(0) = "Product No"

                    dtaExcelData = New DataTable
                    dtaExcelData = ExcelToTable(pSourceURL & "\PIMTemplate\" & OrgID & "_ReActivatePIM.xls", strHeader, "")

                    Dim pMissingProductNo As String

                    If (Not dtaExcelData Is Nothing) Then

                        Try

                            If (dtaExcelData.Rows.Count > 0) Then
                                intCntr = 0
                                While (intCntr < dtaExcelData.Rows.Count)
                                    Dim dtaRow As DataRow
                                    dtaRow = dtaExcelData.Rows(intCntr)

                                    If Not CStr2(dtaRow(0)) = vbNullString Then
                                        Dim vProductID As String

                                        pSql = "Select ID from MobizLibraryPIMProduct " & vbCrLf &
                                                "WHERE ProductNo = " & ReadyString(CStr2(dtaRow(1))) & vbCrLf &
                                                " AND PublishedByID = " & ReadyString(OrgID)

                                        vProductID = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

                                        If Not vProductID = vbNullString Then
                                            pSql = "Update LibraryOrganizationPIM set IsDeactivated=0 where ProductID = " & ReadyString(vProductID)
                                            mBusiness.ProData.Execute(pSql)
                                        Else
                                            pMissingProductNo &= ", " & dtaRow(1)
                                        End If
                                    End If

                                    intCntr = intCntr + 1
                                End While

                                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If

                            Return pMissingProductNo
                        Catch ex As Exception
                            mCurrentUploader = Nothing
                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Failed")
                            AppendToLog(ex.Message)
                        End Try
                    End If
                Catch ex As Exception
                    mCurrentUploader = Nothing
                    WebSession.Instance.PIMTemplateUploaded = "Failed"
                    WebSession.Instance.SerializedString = ""
                    'pOut.Add("Error", ex.Message) ' 2016_01_15__16_30_31_8727 added by arvin
                    'Finally
                    '    Try
                    '        If pFS IsNot Nothing Then pFS.Dispose()
                    '    Catch ex As Exception
                    '    End Try
                End Try
            End If
        End Function

        Private Shared mIndexLock As New Object()
        Private Class CurrentUploader
            Public UploaderKey As String
            Public OrgID As String
            Public DateStart As Date
            Public LastUpdated As Date
            Public FOWSTName As String
            Public BeginningBalanceDateAndTime As String
            Public UploaderUserID As String 'kailangan sa pagset sa beginning balance. mwala ang value sa websession.instance.peopleid pag-abot didto sa function
        End Class
        Private Shared mCurrentUploader As CurrentUploader

        Public Function ResetCurrentUploader() As String

            If mCurrentUploader Is Nothing Then
                Return "Current upload was already reset"
            Else
                mCurrentUploader = Nothing
                Return "Current upload was successfully reset"
            End If

        End Function

        <HttpPost()>
        Function Index(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String, ByVal UploaderKey As String) As String
            Dim strSerializedCols As String = ""
            Dim strTemplateID As String = ""
            Dim dtaExcelHeaders As New DataTable
            Dim dtaExcelHeadersMiss As New DataTable
            Dim dtaExcelTemplateMatching As New DataTable
            Dim objRow As Object
            Dim pFS As IO.FileStream
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim pTotalExcelColumn As Integer = 100
            Dim pX As Integer
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim clsPIMUpload As New PIMUpload
            Dim blnIsSuccess As Boolean = False
            Dim pOut As JsonDictionary
            pOut = New JsonDictionary
            Dim pFilename As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            Try ' 2016_01_15__16_30_31_8727 added by arvin
                If String.IsNullOrWhiteSpace(UploaderKey) Then
                    Return JsonDictionary.Serialize("Error", "UploaderKey must not be blank. ref: vbUPT54379HD!")
                End If
                If IsGuid(OrgID) = False Then
                    Return JsonDictionary.Serialize("Error", "OrgID  must not be a GUID. ref: vb6JWFKKVU6C!")
                End If
                OrgID = ReadyGuid(OrgID)
                Try
                    Do Until Monitor.TryEnter(mIndexLock)
                        Thread.Sleep(1000)
                    Loop
                    If mSaveExcelFileDataThread IsNot Nothing _
                        AndAlso mSaveExcelFileDataThread.ThreadState = ThreadState.Running Then

                        If ReadyGuid(mCurrentUploader.OrgID) = ReadyGuid(OrgID) Then
                            Return JsonDictionary.Serialize("Error", "Another upload process in your organization is currently running. Latest activity was on " & Format(mCurrentUploader.LastUpdated, cGlobal.FormatDateTimeGrid) & ". Please upload again after a few minutes. ref: vb2BPW2VPK4R!")
                        Else
                            Return JsonDictionary.Serialize("Error", "We cannot upload your records at this time due to an ongoing upload. Please wait for current upload to finish and try again later. Latest activity was on " & Format(mCurrentUploader.LastUpdated, cGlobal.FormatDateTimeGrid) & ". Please upload again after a few minutes. ref: vb2BPW2VPK4R!")
                        End If
                    Else
                        ' Start the current records upload.
                    End If
                    mCurrentUploader = Nothing
                    If mCurrentUploader Is Nothing Then
                        mCurrentUploader = New CurrentUploader
                        mCurrentUploader.UploaderKey = UploaderKey
                        mCurrentUploader.OrgID = OrgID
                        mCurrentUploader.DateStart = Now
                        mCurrentUploader.LastUpdated = Now
                        mCurrentUploader.UploaderUserID = CStr2(WebSession.Instance.PeopleUserID)
                    ElseIf mCurrentUploader.UploaderKey <> UploaderKey Then
                        Return JsonDictionary.Serialize("Error", "Another uploader is currently uploading. Latest activity was on " & Format(mCurrentUploader.LastUpdated, cGlobal.FormatDateTimeGrid) & ". Please upload again after a few minutes. ref: vbZ7EJSTXYJD!")
                    End If
                Finally
                    Try
                        Monitor.Exit(mIndexLock)
                    Catch ex As Exception
                        mCurrentUploader = Nothing
                        ' just ignore any errors here
                    End Try
                End Try

                WebSession.Instance.PIMTemplateUploaded = ""

                If (Not objFile Is Nothing) Then
                    pFilename = pSourceURL & "\PIMTemplate\" & OrgID & "_PIMTemplate.xls"
                    CreateFolder(pFilename)
                    objFile.SaveAs(pFilename)

                    Dim mOriginalFileName As String
                    Dim SplitFileName() As String

                    mOriginalFileName = CStr2(objFile.FileName)
                    SplitFileName = mOriginalFileName.Split("-")

                    If SplitFileName.Length > 0 Then
                        mCurrentUploader.FOWSTName = SplitFileName(0).Replace(".xls", "").Replace(".xlsx", "")
                    End If

                    Try
                        pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMTemplate.xls", IO.FileMode.Open)
                        pWB = New Workbook
                        pWB.Open(pFS)
                        pWS = pWB.Worksheets(0)

                        ''Get Headers
                        dtaExcelHeaders.Columns.Add("ExcelColumn")
                        For pX = 0 To pTotalExcelColumn
                            objRow = CStr2(pWS.Cells(0, pX).Value).Trim

                            If (objRow.ToString().Trim().Length > 0) Then
                                dtaExcelHeaders.Rows.Add(objRow)
                            End If
                        Next pX

                        dtaExcelHeadersMiss = clsPIMUpload.CheckExcelColumns(dtaExcelHeaders, OrgID)
                        If (dtaExcelHeadersMiss.Rows.Count <= 0) Then
                            If pFS IsNot Nothing Then pFS.Dispose()

                            dtaExcelTemplateMatching = New DataTable
                            dtaExcelTemplateMatching = clsPIMUpload.GetTemplateMatching(dtaExcelHeaders, OrgID)

                            strSerializedCols = JsonDictionary.Serialize("PIMExcelUpload", dtaExcelTemplateMatching)
                            WebSession.Instance.SerializedString = strSerializedCols
                            WebSession.Instance.PIMTemplateID = strTemplateID
                            WebSession.Instance.PIMTemplateUploaded = "TemplateExist"
                        Else
                            mCurrentUploader = Nothing
                            WebSession.Instance.PIMTemplateUploaded = "NewTemplate"
                            strSerializedCols = JsonDictionary.Serialize("PIMExcelUpload", dtaExcelHeadersMiss)
                            WebSession.Instance.SerializedString = strSerializedCols
                        End If

                    Catch ex As Exception
                        mCurrentUploader = Nothing
                        WebSession.Instance.PIMTemplateUploaded = "Failed"
                        WebSession.Instance.SerializedString = ""
                        pOut.Add("Error", ex.Message) ' 2016_01_15__16_30_31_8727 added by arvin
                    Finally
                        Try
                            If pFS IsNot Nothing Then pFS.Dispose()
                        Catch ex As Exception
                            mCurrentUploader = Nothing
                        End Try
                    End Try

                    pOut.Add("OrgID", OrgID)
                    pOut.Add("Result", WebSession.Instance.PIMTemplateUploaded)
                    Return pOut.Serialize
                    'Return RedirectToAction("Index", New With {.OrgID = OrgID, .Result = WebSession.Instance.PIMTemplateUploaded})
                Else

                    pOut.Add("OrgID", OrgID)
                    pOut.Add("Result", "Empty")
                    Return pOut.Serialize
                    'Return RedirectToAction("Index", New With {.OrgID = OrgID, .Result = "Empty"})
                End If
            Catch ex As Exception
                mCurrentUploader = Nothing
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function SaveExcelFileData(ByVal vTemplateID As String, ByVal OrganizationID As String) As String
            Dim strRet As String
            Dim blnResult As Boolean = False
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim clsPIMUpload As New PIMUpload
            Dim dtaTemplate As New DataTable
            Dim dtaExcelData As New DataTable
            Dim strHeader() As String
            Dim pFS As IO.FileStream
            Dim intCntr As Integer


            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim dtaExcelHeaders As New DataTable
            Dim pTotalExcelColumn As Integer = 100
            Dim objRow As Object
            Dim pX As Integer
            Dim dtaTableRequiredFields As DataTable
            Dim pOut As New JsonDictionary


            Dim strErrors As String
            Dim isProductNoError As Boolean = False
            Dim isProductNameError As Boolean = False
            Dim isCategoryError As Boolean = False
            Dim isPackingSizeQtyError As Boolean = False
            Dim isPackingSizeQtyNumError As Boolean = False
            Dim isPackingSizeUnitError As Boolean = False
            Dim isCostFromVendorError As Boolean = False
            Dim isCostFromVendorNumError As Boolean = False
            Dim isLandedCostError As Boolean = False
            Dim isLandedCostNumError As Boolean = False
            Dim isSRPError As Boolean = False
            Dim isSRPNumError As Boolean = False
            Dim isRetailPriceError As Boolean = False
            Dim isRetailPriceNumError As Boolean = False
            Dim IsHasDisableColumn As Boolean = False
            Dim IsHasBeginningBalanceColumn As Boolean = False
            Dim IsHasPOQuantityColumn As Boolean = False
            Dim IsHasProductAvailableToMobileColumn As Boolean = False
            Dim IsHasSCDPWDColumn As Boolean = False
            Dim IsHasInventoryCountItemColumn As Boolean = False
            Dim isUpcomingRetailPriceError As Boolean = False
            Dim isUpcomingRetailPriceEffectivityDateError As Boolean = False

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pEnableParsing = CBool2(Common.WebRequest.Data("EnableParsing"))
            Dim pDoNotOverwriteBomLandedCost = CBool2(Common.WebRequest.Data("DoNotOverwriteBomLandedCost"))

            If mCurrentUploader IsNot Nothing Then
                mCurrentUploader.BeginningBalanceDateAndTime = CStr2(Common.WebRequest.Data("BeginningBalanceDateAndTime"))
            End If

            If mSaveExcelFileDataThread IsNot Nothing _
                AndAlso mSaveExcelFileDataThread.ThreadState = ThreadState.Running Then

                Return JsonDictionary.Serialize("Error", "We cannot upload your records at this time due to an ongoing upload. Please wait for current upload to finish and try again later. Latest activity was on " & Format(mCurrentUploader.LastUpdated, cGlobal.FormatDateTimeGrid) & ". Please upload again after a few minutes. ref: vbD96WMLTJU7!")
            End If

            ''Get Template Columns
            If (vTemplateID Is Nothing) Then
                vTemplateID = WebSession.Instance.PIMTemplateID
            End If

            If (OrganizationID Is Nothing) Then
                OrganizationID = WebRequest.Instance.OrgID
            End If

            pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls", IO.FileMode.Open)
            pWB = New Workbook
            pWB.Open(pFS)
            pWS = pWB.Worksheets(0)

            ''Get Headers
            dtaExcelHeaders.Columns.Add("ExcelColumn")
            For pX = 0 To pTotalExcelColumn
                objRow = CStr2(pWS.Cells(0, pX).Value).Trim

                If (objRow.ToString().Trim().Length > 0) Then
                    dtaExcelHeaders.Rows.Add(objRow)
                    If objRow = "Is Deactivated" Then IsHasDisableColumn = True
                    If objRow = "Beginning Balance" Then IsHasBeginningBalanceColumn = True
                    If objRow = "PO Quantity" Then IsHasPOQuantityColumn = True
                    If objRow = "Include in mobile database" Then IsHasProductAvailableToMobileColumn = True
                    If objRow = "EligibleForSCDPWDDiscount" Then IsHasSCDPWDColumn = True
                    If objRow = "This Is An Inventory-Count Item?" Then IsHasInventoryCountItemColumn = True
                End If
            Next pX

            dtaTemplate = clsPIMUpload.GetTemplateColumns(dtaExcelHeaders, OrganizationID)
            If (dtaTemplate.Rows.Count > 0) Then

                '''Check for required fields
                dtaTableRequiredFields = clsPIMUpload.CheckRequiredFields(dtaTemplate)
                If (dtaTableRequiredFields.Rows.Count > 0) Then

                    pOut.Add("PIMExcelUpload", "NeedRequiredFields")
                    pOut.Add("PIMExcelUploadRequired", dtaTableRequiredFields)

                    Try
                        If pFS IsNot Nothing Then pFS.Dispose()
                    Catch ex As Exception
                        mCurrentUploader = Nothing
                    End Try

                    Return pOut.Serialize
                End If

                Dim TemplateCount As Integer = 0

                If IsHasInventoryCountItemColumn Then
                    TemplateCount += 1
                End If

                If IsHasSCDPWDColumn Then
                    TemplateCount += 1
                End If

                If IsHasDisableColumn Then
                    TemplateCount += 1
                End If

                If IsHasBeginningBalanceColumn Then
                    TemplateCount += 1
                End If

                If IsHasProductAvailableToMobileColumn Then
                    TemplateCount += 1
                End If

                ReDim Preserve strHeader((dtaTemplate.Rows.Count + TemplateCount))

                intCntr = 0
                While (intCntr < dtaTemplate.Rows.Count)
                    strHeader(intCntr) = dtaTemplate.Rows(intCntr).Item("ColumnName").ToString()
                    intCntr = intCntr + 1
                End While
                Dim pColumnList As New List(Of String)
                For Each pDR As DataRow In dtaTemplate.Rows
                    pColumnList.Add(CStr2(pDR("ColumnName")))
                Next pDR

                If IsHasPOQuantityColumn Then
                    pColumnList.Add("PO Quantity")
                    intCntr += 1
                End If

                If IsHasDisableColumn Then
                    pColumnList.Add("Is Deactivated")
                    intCntr += 1
                End If

                If IsHasBeginningBalanceColumn Then
                    pColumnList.Add("Beginning Balance")
                    intCntr += 1
                End If

                If IsHasProductAvailableToMobileColumn Then
                    pColumnList.Add("Include in mobile database")
                    intCntr += 1
                End If

                If IsHasSCDPWDColumn Then
                    pColumnList.Add("EligibleForSCDPWDDiscount")
                    intCntr += 1
                End If

                If IsHasInventoryCountItemColumn Then
                    pColumnList.Add("This Is An Inventory-Count Item?")
                    intCntr += 1
                End If

                pColumnList = UniqueItems(pColumnList)
                strHeader = pColumnList.ToArray()

                '''Save The Excel File Data
                dtaExcelData = New DataTable
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls", strHeader, "")

                '''''''''''''''''
                'additional by janine april 8 2015
                Dim NoError As Boolean = True
                Dim strExcelColName As String
                Dim productNameCheckMatch As String
                '''''
                If (Not dtaExcelData Is Nothing) Then
                    Try

                        If (dtaExcelData.Rows.Count > 0) Then

                            intCntr = 0
                            While (intCntr < dtaExcelData.Rows.Count)
                                Dim pNo As String
                                Dim dtaRow As DataRow
                                Dim pHasRowError As Boolean = False ' excel row
                                dtaRow = dtaExcelData.Rows(intCntr)

                                If Not isProductNoError Then
                                    '''Find Product No Column fixed Feature ID F9CB775D-5E60-4D92-98A3-2A8444D5F32A
                                    strExcelColName = clsPIMUpload.GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", dtaTemplate)
                                    pNo = dtaRow(strExcelColName).ToString()
                                    If (strExcelColName.Trim().Length > 0) Then
                                        'if required field is empty!
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Product Number is required. "
                                            pHasRowError = True
                                            NoError = False
                                            isProductNoError = True
                                        ElseIf (dtaRow(strExcelColName).ToString().Length >= 50) Then
                                            strErrors = strErrors & " Product Number must less than 51 characters in length. "
                                            pHasRowError = True
                                            NoError = False
                                            isProductNoError = True
                                        End If
                                    End If
                                End If

                                If Not isProductNameError Then

                                    '''Find Short Product Name Column fixed Feature ID DAFBB390-EA84-4EC8-948B-74B028CBE996
                                    strExcelColName = clsPIMUpload.GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", dtaTemplate)

                                    Dim pName = dtaRow(strExcelColName).ToString()

                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (pName.Length <= 0) Then
                                            strErrors = strErrors & " Product Name is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isProductNameError = True
                                        ElseIf (pName.Length >= 250) Then
                                            strErrors = strErrors & " Product Name must be less than 251 characters in length. "
                                            NoError = False
                                            pHasRowError = True
                                            isProductNameError = True
                                        Else
                                            'check product name if has match
                                            If Not isSameProduct Then
                                                If (CStr2(pNo) <> "") Then
                                                    productNameCheckMatch = IsProductNoUnique(pNo)

                                                    If (productNameCheckMatch <> "True") Then
                                                        If (pName <> productNameCheckMatch) Then
                                                            strErrors = strErrors & " Product No. " & pNo &
                                                                " already exists with a different product name: " & productNameCheckMatch
                                                            NoError = False
                                                            pHasRowError = True
                                                            'isProductNameError = True
                                                        End If
                                                    End If

                                                End If

                                            End If



                                        End If

                                    End If

                                End If


                                If Not isCategoryError Then
                                    '''Find Category
                                    strExcelColName = clsPIMUpload.GetExcelColumn("325A48B7-58F8-4678-B608-58CF43C59BFD", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Category is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isCategoryError = True
                                        End If
                                    End If
                                End If

                                If Not isPackingSizeQtyError Then
                                    '''Find Packing Size Qty
                                    strExcelColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Packing Size Qty is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isPackingSizeQtyError = True
                                        Else
                                            If Not isPackingSizeQtyNumError Then
                                                If (Not IsNumeric(dtaRow(strExcelColName))) Then
                                                    strErrors = strErrors & " Packing Size Qty must be numeric. "
                                                    NoError = False
                                                    pHasRowError = True
                                                    isPackingSizeQtyNumError = True
                                                End If
                                            End If

                                        End If
                                    End If
                                End If

                                If Not isPackingSizeUnitError Then
                                    '''Find Packing Size Unit
                                    strExcelColName = clsPIMUpload.GetExcelColumn("C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Packing Size Unit is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isPackingSizeUnitError = True
                                        End If
                                    End If
                                End If


                                If Not isCostFromVendorError Then
                                    '''Find cost from vendor 
                                    strExcelColName = clsPIMUpload.GetExcelColumn("E531191E-203E-41CC-BA9B-C46A27D10CD8", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Cost from Vendor is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isCostFromVendorError = True
                                        Else
                                            If Not isCostFromVendorNumError Then
                                                If (Not IsNumeric(dtaRow(strExcelColName))) Then
                                                    strErrors = strErrors & " Cost from Vendor must be numeric. "
                                                    NoError = False
                                                    pHasRowError = True
                                                    isCostFromVendorNumError = True
                                                End If
                                            End If

                                        End If
                                    End If


                                End If


                                If Not isLandedCostError Then
                                    '''Find landed cost
                                    strExcelColName = clsPIMUpload.GetExcelColumn("1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Landed Cost is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isLandedCostError = True
                                        Else
                                            If Not isLandedCostNumError Then
                                                If (Not IsNumeric(dtaRow(strExcelColName))) Then
                                                    strErrors = strErrors & " Landed Cost must be numeric. "
                                                    NoError = False
                                                    pHasRowError = True
                                                    isLandedCostNumError = True
                                                End If
                                            End If

                                        End If
                                    End If
                                End If

                                If Not isSRPError Then
                                    '''Find srp
                                    strExcelColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C99", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " SRP - Suggested Retail Price is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isSRPError = True
                                        Else
                                            If Not isSRPNumError Then
                                                If (Not IsNumeric(dtaRow(strExcelColName))) Then
                                                    strErrors = strErrors & " SRP - Suggested Retail Price must be numeric. "
                                                    NoError = False
                                                    pHasRowError = True
                                                    isSRPNumError = True
                                                End If
                                            End If

                                        End If
                                    End If
                                End If


                                If Not isRetailPriceError Then
                                    '''Find retail price                         
                                    strExcelColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C90", dtaTemplate)
                                    If (strExcelColName.Trim().Length > 0) Then
                                        If (dtaRow(strExcelColName).ToString().Length <= 0) Then
                                            strErrors = strErrors & " Retail Price is required. "
                                            NoError = False
                                            pHasRowError = True
                                            isRetailPriceError = True

                                        Else
                                            If Not isRetailPriceNumError Then
                                                If (Not IsNumeric(dtaRow(strExcelColName))) Then
                                                    strErrors = strErrors & " Retail Price must be numeric. "
                                                    NoError = False
                                                    pHasRowError = True
                                                    isRetailPriceNumError = True
                                                End If
                                            End If

                                        End If
                                    End If

                                End If

                                If Not isUpcomingRetailPriceError Or Not isUpcomingRetailPriceEffectivityDateError Then
                                    '''Find Upcoming retail price  and effectivity date                       
                                    Dim upcomingretailprice As String = clsPIMUpload.GetExcelColumn("3ec54904-f1b9-4be4-8ecb-6a28355f2d18", dtaTemplate)
                                    Dim upcomingretailpriceeffectivitydate As String = clsPIMUpload.GetExcelColumn("6de1c205-8d7d-499c-9a28-a1db0a7e2762", dtaTemplate)

                                    If (upcomingretailprice.Trim().Length > 0 Or upcomingretailpriceeffectivitydate.Trim().Length > 0) Then
                                        If (upcomingretailprice.Trim().Length > 0 And upcomingretailpriceeffectivitydate.Trim().Length = 0) Then
                                            strErrors = strErrors & " 'Upcoming Retail Price Effectivity Date' column is missing "
                                            NoError = False
                                            pHasRowError = True
                                            isUpcomingRetailPriceEffectivityDateError = True
                                        ElseIf (upcomingretailprice.Trim().Length = 0 And upcomingretailpriceeffectivitydate.Trim().Length > 0) Then
                                            strErrors = strErrors & " 'Upcoming Retail Price' column is missing "
                                            NoError = False
                                            pHasRowError = True
                                            isUpcomingRetailPriceError = True
                                        End If

                                        If NoError Then
                                            Dim urpval As String = dtaRow(upcomingretailprice)
                                            Dim urpedval As String = dtaRow(upcomingretailpriceeffectivitydate)
                                            If (Not IsNumeric(urpval) And Not CStr2(urpval) = "") Then
                                                strErrors = strErrors & " Upcoming Retail Price must be numeric. "
                                                NoError = False
                                                pHasRowError = True
                                                isUpcomingRetailPriceError = True
                                            End If

                                            If (Not IsDate(urpedval) And Not CStr2(urpedval) = "") Then
                                                strErrors = strErrors & " Upcoming Retail Price Effectivity Date must be valid date. "
                                                NoError = False
                                                pHasRowError = True
                                                isUpcomingRetailPriceEffectivityDateError = True
                                            End If

                                            If (Not CStr2(urpval) = "" And urpedval = "") Or (CStr2(urpval) = "" And Not urpedval = "") Then
                                                strErrors = strErrors & " 'Upcoming Retail Price' and 'Upcoming Retail Price Effectivity Date' column is required. "
                                                NoError = False
                                                pHasRowError = True
                                                isUpcomingRetailPriceError = True
                                            End If
                                        End If
                                    End If
                                End If

                                If pHasRowError Then ' 2016_01_15__16_30_31_8727 added by arvin
                                    strErrors &= ". The preceding error happens on row #" & dtaRow("Spreadsheet Row Number") & "."
                                End If

                                intCntr = intCntr + 1
                            End While

                            If NoError Then
                                Static sSL As New Threading.SpinLock
                                strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Uploading data from Excel File. This might take a few moments. Please wait ...")

                                mDatabase = mProData.GetCurrentDatabase()
                                vPeopleUserID = WebSession.Instance.PeopleUserID
                                WebSession.Instance.OrganizationID = OrganizationID

                                mSaveExcelFileDataThread = TimedThread(
                                    Sub()

                                        Try

                                            blnResult = SavePIMTemplateData(dtaTemplate, dtaExcelData, OrganizationID, mDatabase, vPeopleUserID, IsHasDisableColumn, IsHasSCDPWDColumn, IsHasProductAvailableToMobileColumn, pEnableParsing, pDoNotOverwriteBomLandedCost, IsHasBeginningBalanceColumn, IsHasInventoryCountItemColumn)

                                            mCurrentUploader = Nothing

                                            Dim mBusiness As Business
                                            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                                            mBusiness.ProData.Execute("USE " & EscapeSqlName(mDatabase))

                                            Dim dtPOProducts As New DataTable

                                            dtPOProducts.Columns.Add("ID")
                                            dtPOProducts.Columns.Add("POQuantity")

                                            For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                                                Dim dr As DataRow
                                                Dim pSql As String
                                                Dim ProductID As String
                                                Dim ProductNoColumn As String = "Product No"
                                                Dim IsPOQuantityExist As Boolean

                                                For y As Integer = 0 To dtaExcelHeaders.Rows.Count - 1
                                                    If dtaExcelHeaders.Rows(y).Item("ExcelColumn") = "Product No *" Then
                                                        ProductNoColumn = "Product No *"
                                                        Exit For
                                                    End If
                                                Next

                                                For z As Integer = 0 To dtaExcelHeaders.Rows.Count - 1
                                                    If dtaExcelHeaders.Rows(z).Item("ExcelColumn") = "PO Quantity" Then
                                                        IsPOQuantityExist = True
                                                        Exit For
                                                    End If
                                                Next

                                                If Not IsPOQuantityExist Then Exit For

                                                pSql = "Select MobizLibraryPIMProduct.ID from MobizLibraryPIMProduct " & vbCrLf &
                                                        "inner join LibraryOrganizationPIM on MobizLibraryPIMProduct.ID = LibraryOrganizationPIM.ProductID " & vbCrLf &
                                                        "where ProductNo = " & ReadyString(dtaExcelData.Rows(i).Item(ProductNoColumn))

                                                ProductID = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

                                                dr = dtPOProducts.NewRow

                                                dr(0) = ProductID
                                                dr(1) = CInt2(dtaExcelData.Rows(i).Item("PO Quantity"))

                                                dtPOProducts.Rows.Add(dr)
                                            Next

                                            pOut.Add("PIMExcelUpload", IIf(blnResult, "Success", "Failed"))
                                            pOut.Add("PIMPOProducts", dtPOProducts)

                                            strExcelFileProgress = pOut.Serialize

                                            'strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", IIf(blnResult, "Success", "Failed"))


                                            If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")) Then
                                                IO.File.Delete(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")
                                            End If

                                            Try
                                                If pFS IsNot Nothing Then pFS.Dispose()
                                            Catch ex As Exception
                                                mCurrentUploader = Nothing
                                            End Try

                                        Catch ex As Exception
                                            mCurrentUploader = Nothing
                                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Encountered error while uploading data from Excel File. " + Truncate(ex.Message, 300))
                                            AppendToDebug("vbRHV9XX58NF! " & ex.Message, True, ex, True)
                                        Finally
                                            If sSL.IsHeldByCurrentThread Then
                                                ' do something here
                                                sSL.Exit()
                                            End If
                                        End Try

                                    End Sub
                                )
                            Else
                                mCurrentUploader = Nothing
                                strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUploadError", strErrors)


                            End If

                        Else 'if template is correct but no data
                            mCurrentUploader = Nothing
                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Failed")
                            AppendToLog("pim template uploaded is correct but no data")
                        End If

                    Catch ex As Exception
                        mCurrentUploader = Nothing
                        strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Failed")
                        AppendToLog(ex.Message)
                    End Try

                Else
                    mCurrentUploader = Nothing
                    strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Failed")
                    AppendToLog("pim upload excel data error. ExcelToTable returned null")
                End If
                '''''''''''''''''
            End If

            strRet = strExcelFileProgress
            Return strRet

        End Function

        Function SavePIMTemplateData(ByVal DTemplate As DataTable, ByVal DTable As DataTable, ByVal vOrgID As String, ByVal vDBaseName As String, ByVal vPeopleUserID As String, ByVal IsHasDisableColumn As Boolean, ByVal IsHasSCDPWDColumn As Boolean, IsHasProductAvailableToMobileColumn As Boolean,
            vEnableParsing As Boolean, vDoNotOverwriteBomLandedCost As Boolean, IsHasBeginningBalanceColumn As Boolean, IsHasInventoryCountItemColumn As Boolean) As Boolean

            Dim p As New InsertUpdate("MobizLibraryPIMTemplate")
            Dim p2 As New InsertUpdate("MobizLibraryPIMTemplate")
            Dim blnRet As Boolean = False
            Dim pSql As String
            Dim intCntr As Integer
            Dim intProductCntr As Integer
            Dim intTemplateColCntr As Integer
            Dim strExcelColName As String
            Dim strProductSearchString As String
            Dim strProductID As String
            Dim strMeasurementID As String
            Dim strOrgPriceLevelID As String
            Dim clsOrganization As New Organization
            Dim dtaOrgBrand As DataTable
            Dim pFinder As Finder
            Dim pDTProduct As DataTable
            Dim pDT As DataTable
            Dim pDTPriceLevel As DataTable
            Dim pDTUnitMasure As DataTable
            Dim clsPIMMeasure As New PIMMeasure
            Dim strUnitMeasureID As String
            Dim strMeasurementLevelID As String
            Dim intMeasurementLevel As Integer
            Dim pDTMeasurement As DataTable
            Dim pProductNum As String
            Dim pProductName As String
            Dim pHasBomContents As Boolean
            Dim pParseData As LibraryStock.ProductDescriptionParser.Output
            Dim pProductIDs As New List(Of String)
            Dim clsPIMUpload As New PIMUpload

            Dim BinID As String
            Dim FOWSTID As String

            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(vDBaseName))
            Dim pParser = mBusiness.LibraryStock.CreateProductDescriptionParser()
            mProData = mBusiness.ProData

            Try

                '''Get Organization Brand Collections
                pFinder = New Finder
                With pFinder
                    .ProData = mBusiness.ProData
                    .MaxRecords = MaxReturnRow

                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "orgbrand.ID"
                            .Searchable = False
                        End With
                        With .Add("BrandName", SqlDbType.VarChar)
                            .Expression = "orgbrand.BrandName"
                        End With
                    End With

                    .SqlFrom = "FROM MobizLibraryOrganizationBrand orgbrand "

                    .SqlWhere = "WHERE orgbrand.OrganizationID " & IIf(vOrgID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(vOrgID))

                    .SqlOrder = "ORDER BY orgbrand.BrandName ASC"

                    dtaOrgBrand = .Requery
                End With

                If (DTable.Rows.Count > 0) Then

                    'mBusiness.ProData.BeginTransaction()

                    If mCurrentUploader IsNot Nothing _
                        AndAlso Not mCurrentUploader.BeginningBalanceDateAndTime = vbNullString Then
                        mCurrentUploader.BeginningBalanceDateAndTime = ProSqlite.ReadyDate(mCurrentUploader.BeginningBalanceDateAndTime)
                    End If

                    intCntr = 0
                    While (intCntr < DTable.Rows.Count)
                        Dim pSupplierID As String = Nothing
                        Dim pLandedCost As Decimal? = Nothing
                        Dim pContentUnit As String = Nothing
                        Dim pContentQty As String = Nothing  ' decimal

                        mBusiness.ProData.BeginTransaction()

                        strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Uploading " & intCntr.ToString("#,##0") & " of " & DTable.Rows.Count.ToString("#,##0") & " data from Excel File. Please wait...")

                        Dim dtaRow As DataRow
                        dtaRow = DTable.Rows(intCntr)

                        p = New InsertUpdate("MobizLibraryPIMProduct")

                        If IsAsset = True Then ' using PIM Web Form
                            p("IsAsset") = ReadyString(IsAsset)
                        Else
                            'using pim excel template
                            If dtaRow.Table.Columns.Contains("Rentable/PPE Asset?") Then
                                p("IsAsset") = ReadyString(IIf(dtaRow("Rentable/PPE Asset?") = "Yes", True, False))
                                IsAsset = IIf(dtaRow("Rentable/PPE Asset?") = "Yes", True, False)
                            End If
                        End If

                        p("PublishedByID") = ReadyString(vOrgID)
                        p("UploadedByID") = ReadyString(vOrgID)
                        p("IsBrandOwner") = clsPIMUpload.IsOrgBrandOwner(dtaRow("Brand").ToString(), dtaOrgBrand)

                        '''Find Short Product Name Column fixed Feature ID DAFBB390-EA84-4EC8-948B-74B028CBE996
                        strExcelColName = clsPIMUpload.GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            pProductName = dtaRow(strExcelColName).ToString()
                            If vEnableParsing Then
                                pParseData = pParser.ParseProductDescription(pProductName)
                                p("ParsedProductName") = ReadyString(pParseData.ProductDescription.Trim())
                            End If
                            p("ProductName") = ReadyString(pProductName)
                        End If

                        '''Find Short Product Name Column fixed Feature ID 7B3524D2-EFFC-4921-B752-3CF7D84434D6
                        strExcelColName = clsPIMUpload.GetExcelColumn("7B3524D2-EFFC-4921-B752-3CF7D84434D6", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("Brand") = ReadyString(dtaRow(strExcelColName).ToString())
                        End If

                        '''Find Division Description Column fixed Feature ID EBBE5A38-5A5A-4328-8EF2-2C832680B125
                        strExcelColName = clsPIMUpload.GetExcelColumn("EBBE5A38-5A5A-4328-8EF2-2C832680B125", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("DivisionDescription") = ReadyString(dtaRow(strExcelColName).ToString())
                        End If

                        '''Find Product Group No (Style Number - Color Code) Column fixed Feature ID E4FEFAF4-EBC5-47EA-BEA3-2DFBCB5DC107
                        strExcelColName = clsPIMUpload.GetExcelColumn("E4FEFAF4-EBC5-47EA-BEA3-2DFBCB5DC107", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("ProductGroupNo") = ReadyString(dtaRow(strExcelColName).ToString())
                        End If

                        '''Find Model Column fixed Feature ID 7D8D902D-5710-400A-A39A-FE4495409B5D
                        strExcelColName = clsPIMUpload.GetExcelColumn("7D8D902D-5710-400A-A39A-FE4495409B5D", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("ModelNo") = ReadyString(dtaRow(strExcelColName).ToString())
                        End If

                        '''Find SRP Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C99
                        strExcelColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C99", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("SRP") = cdec2(dtaRow(strExcelColName).ToString())
                        Else
                            p("SRP") = 0
                        End If

                        '''Find Marketing Text Column fixed Feature ID D1511CBE-FBAF-4F97-9A61-5E2684D7098F
                        strExcelColName = clsPIMUpload.GetExcelColumn("D1511CBE-FBAF-4F97-9A61-5E2684D7098F", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("MarketingText") = ReadyString(dtaRow(strExcelColName).ToString())
                        End If

                        '''Find Warranty Column fixed Feature ID B16B5DDA-6660-4E94-8FCE-DB00C8AFC5B7
                        strExcelColName = clsPIMUpload.GetExcelColumn("B16B5DDA-6660-4E94-8FCE-DB00C8AFC5B7", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("Warranty") = ReadyString(dtaRow(strExcelColName).ToString())
                        End If

                        '''Product Vendor
                        strExcelColName = clsPIMUpload.GetExcelColumn("4F2CF1F2-FB38-4F54-9244-EF3D4878D3E2", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            Dim pProductVendor = dtaRow(strExcelColName).ToString()
                            If String.IsNullOrWhiteSpace(pProductVendor) = False Then
                                pSupplierID = mBusiness.LibraryOrganization.GetOrAddByCompanyName(pProductVendor, "", False, True)
                            End If
                        End If

                        '''Find Product No Column fixed Feature ID F9CB775D-5E60-4D92-98A3-2A8444D5F32A
                        strExcelColName = clsPIMUpload.GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            If vEnableParsing = False _
                                OrElse pParseData.UnitQty = 1 Then

                                pProductNum = dtaRow(strExcelColName).ToString()
                                p("ProductNo") = ReadyString(pProductNum)
                            End If
                        End If

                        '''Get Product ID
                        pFinder = New Finder
                        With pFinder
                            .ProData = mBusiness.ProData
                            .MaxRecords = MaxReturnRow

                            With .Fields
                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prd.ID"
                                End With
                                With .Add("ProductNo", SqlDbType.VarChar)
                                    .Expression = "prd.ProductNo"
                                End With
                                With .Add("ProductName", SqlDbType.VarChar)
                                    .Expression = "prd.ProductName"
                                End With
                                With .Add("UploadedByID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prd.UploadedByID"
                                End With
                            End With

                            .SqlFrom = "FROM MobizLibraryPIMProduct prd "
                            'additonal by janine - JAN 12 2015. COMPARE ONLY IN PRIVATE. NOT PUBLIC. so whatever 
                            'you upload, will be uploaded, not overwrittenn or ignored by an existing product in the public db
                            'add the uploaded by id in where clause to check against private if there's an existing. 
                            .SqlFrom &= " INNER JOIN LibraryOrganizationPIM orgPIM ON prd.ID = orgPIM.ProductID "
                            If vEnableParsing Then
                                .SqlWhere = "WHERE prd.ParsedProductName = " & ReadyString(pParseData.ProductDescription.Trim())
                            Else
                                .SqlWhere = "WHERE prd.ProductNo =" & ReadyString(dtaRow(strExcelColName).ToString().Trim())
                            End If
                            'trim leading zeros both in sqlserver and in excel file to compare both
                            ' result example: if in the excel file product no 0000-0001 will become '-0001' to be compared with sql server's '-0001' because both are trimmed
                            ' also if product no 00009999999 in the excel, it will become 9999999. so everytime we compare, we exclude the leading zeros
                            ' because excel sometimes formats the numbers automaticaly as numeric and remove leading zeros
                            .SqlWhere &= " OR SUBSTRING(prd.ProductNo, PATINDEX('%[^0 ]%', prd.ProductNo + ' '), LEN(prd.ProductNo)) =" & ReadyString(dtaRow(strExcelColName).ToString().TrimStart("0"c))

                            .SqlOrder = " ORDER BY ISNULL(prd.IsBrandOwner,0) DESC "

                            pDTProduct = .Requery
                        End With

                        strProductID = ""
                        If (pDTProduct.Rows.Count > 0) Then

                            intProductCntr = 0
                            While (intProductCntr < pDTProduct.Rows.Count)

                                If (pDTProduct.Rows(intProductCntr).Item("UploadedByID").ToString().ToLower().Trim() = vOrgID.ToLower().Trim()) Then
                                    strProductID = strProductID & IIf(strProductID.Trim().Length > 0, ", '" & pDTProduct.Rows(intProductCntr).Item("ID").ToString().Trim() & "' ", "'" & pDTProduct.Rows(intProductCntr).Item("ID").ToString().Trim() & "'")
                                End If

                                intProductCntr = intProductCntr + 1
                            End While

                            If vEnableParsing _
                                AndAlso pParseData.BasicUnit IsNot Nothing Then

                                For Each pDrProduct As DataRow In pDTProduct.Rows
                                    If CStr2(pDrProduct("ProductName")).Trim().Length > pProductName.Length Then
                                        pProductName = CStr2(pDrProduct("ProductName")).Trim()
                                    End If
                                Next
                            End If
                        End If

                        pHasBomContents = False
                        If IsGuid(Trim2(strProductID, "'")) Then
                            If vDoNotOverwriteBomLandedCost Then
                                pHasBomContents = mBusiness.LibraryStock.GetContents(Trim2(strProductID, "'"), vOrgID).Rows.Count > 0
                            End If
                        End If

                        strProductSearchString = ""
                        intTemplateColCntr = 0
                        While (intTemplateColCntr < DTemplate.Rows.Count)
                            If dtaRow.Table.Columns.Count <= intTemplateColCntr Then
                                Exit While ' prevent ref: vbFZ6E22P3C8! Cannot find column 24
                            End If
                            strProductSearchString = strProductSearchString & IIf(strProductSearchString.Trim().Length > 0, ", " & dtaRow(intTemplateColCntr).ToString().Trim(), dtaRow(intTemplateColCntr).ToString().Trim())
                            intTemplateColCntr = intTemplateColCntr + 1
                        End While

                        p("ProductSearchString") = ReadyString(strProductSearchString)

                        p("DateAdded") = "getdate()"
                        p("IsActive") = "1"
                        p("UserID") = ReadyString(vPeopleUserID)
                        p("LastUpdated") = "getdate()"
                        p("ProductName") = ReadyString(pProductName)

                        If (pDTProduct.Rows.Count <= 0) Then
                            strProductID = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strProductID)

                            pSql = p.GetInsert()

                            Dim objRow(1) As Object
                            objRow(0) = strProductID
                            objRow(1) = dtaRow(strExcelColName).ToString()
                            pDTProduct.Rows.Add(objRow)

                            mBusiness.ProData.ExecuteSQL(pSql)
                        Else
                            '''Update only products owned/uploaded by the current Organization
                            If (strProductID.Trim().Length > 0) Then
                                pSql = p.GetUpdate(" ID IN (" & strProductID & ")")
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If
                        End If

                        '''Insert into LibraryPIMProductFeatureStructure - Brand
                        If (dtaRow("Brand").ToString().Trim().Length > 0) Then

                            '''Insert into LibraryPIMProductFeatureStructure - Brand Structure '''Fixed ID for brand feature
                            '''Fixed value for Brand feature 7B3524D2-EFFC-4921-B752-3CF7D84434D6
                            pFinder = New Finder
                            With pFinder
                                .ProData = mBusiness.ProData
                                .MaxRecords = MaxReturnRow

                                With .Fields
                                    With .Add("ID", SqlDbType.UniqueIdentifier)
                                        .Expression = "feastr.ID"
                                        .Searchable = False
                                    End With
                                    With .Add("StructuredValue", SqlDbType.VarChar)
                                        .Expression = "feastr.StructuredValue"
                                    End With
                                End With

                                .SqlFrom = "FROM MobizLibraryPIMFeatureStructure feastr "

                                .SqlWhere = " WHERE feastr.FeatureID = " & ReadyString("7B3524D2-EFFC-4921-B752-3CF7D84434D6") &
                                           " AND feastr.StructuredValue = " & ReadyString(dtaRow("Brand").ToString())
                                pDT = New DataTable()
                                pDT = .Requery

                            End With

                            If (pDT.Rows.Count <= 0) Then '''If not exist in the structured option, insert
                                p = New InsertUpdate("MobizLibraryPIMFeatureStructure")

                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("FeatureID") = ReadyString("7B3524D2-EFFC-4921-B752-3CF7D84434D6")
                                p("StructuredValue") = ReadyString(dtaRow("Brand").ToString())

                                pSql = p.GetInsert()
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If

                        End If
                        '''End Insert into Brand Structure ----------------

                        '''apply to All product with the same product No
                        intProductCntr = 0
                        While (intProductCntr < pDTProduct.Rows.Count)
                            strProductID = pDTProduct.Rows(intProductCntr).Item("ID").ToString().Trim()
                            ''''''''''''''''
                            'additional by janine jan 13 2015. edit only if uploaded by id is current org
                            ''''''''''''''''''this IF clause has been added by janine so that the unit measure of the product uploaded by the other 
                            ''org will not be modified. only modify the price for you org OR IF NEW PRODUCT (BLANK UPLOADED BY ID)
                            If (pDTProduct.Rows(intProductCntr).Item("UploadedByID").ToString().ToLower().Trim() = "" Or pDTProduct.Rows(intProductCntr).Item("UploadedByID").ToString().ToLower().Trim() = vOrgID.ToLower().Trim()) Then

                                '''Insert into MobizLibraryPIMProductMeasurement
                                '''Find Packing Size Unit Column fixed Feature ID C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9
                                strExcelColName = clsPIMUpload.GetExcelColumn("C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        '''get UnitMeasure ID Not Wild
                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "msr.ID"
                                                    .Searchable = False
                                                End With
                                                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "msr.MeasureNameID"
                                                    .Searchable = False
                                                End With
                                                With .Add("Measurement", SqlDbType.VarChar)
                                                    .Expression = "lbl.Label"
                                                End With
                                                With .Add("SignID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "msr.SignID"
                                                    .Searchable = False
                                                End With
                                                With .Add("Sign", SqlDbType.VarChar)
                                                    .Expression = "ISNULL(sgn.Sign,'')"
                                                End With
                                                With .Add("MeasurementAKA", SqlDbType.VarChar)
                                                    .Expression = "msr.MeasurementAKA"
                                                End With
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                                                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                                                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

                                            .SqlWhere = " WHERE lbl.Label = " & ReadyString(dtaRow(strExcelColName).ToString().Trim()) &
                                                    " OR ISNULL(sgn.Sign,'') = " & ReadyString(dtaRow(strExcelColName).ToString().Trim())

                                            .SqlOrder = "ORDER BY lbl.Label ASC"

                                            pDTUnitMasure = .Requery

                                        End With

                                        If (pDTUnitMasure.Rows.Count > 0) Then
                                            strUnitMeasureID = pDTUnitMasure.Rows(0).Item("ID").ToString()
                                        Else
                                            pFinder = New Finder
                                            With pFinder
                                                .ProData = mBusiness.ProData
                                                .MaxRecords = MaxReturnRow

                                                With .Fields
                                                    With .Add("ID", SqlDbType.UniqueIdentifier)
                                                        .Expression = "msr.ID"
                                                        .Searchable = False
                                                    End With
                                                    With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                                                        .Expression = "msr.MeasureNameID"
                                                        .Searchable = False
                                                    End With
                                                    With .Add("Measurement", SqlDbType.VarChar)
                                                        .Expression = "lbl.Label"
                                                    End With
                                                    With .Add("SignID", SqlDbType.UniqueIdentifier)
                                                        .Expression = "msr.SignID"
                                                        .Searchable = False
                                                    End With
                                                    With .Add("Sign", SqlDbType.VarChar)
                                                        .Expression = "ISNULL(sgn.Sign,'')"
                                                    End With
                                                    With .Add("MeasurementAKA", SqlDbType.VarChar)
                                                        .Expression = "msr.MeasurementAKA"
                                                    End With
                                                End With

                                                .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                                                        " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                                                        " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

                                                .SqlWhere = " WHERE msr.MeasurementAKA LIKE " & ReadyStringWild(dtaRow(strExcelColName).ToString().Trim())

                                                .SqlOrder = "ORDER BY lbl.Label ASC"

                                                pDTUnitMasure = .Requery

                                            End With

                                            If (pDTUnitMasure.Rows.Count > 0) Then
                                                strUnitMeasureID = pDTUnitMasure.Rows(0).Item("ID").ToString()
                                            End If
                                        End If

                                        p = New InsertUpdate("MobizLibraryPIMProductMeasurement")
                                        p("ProductID") = ReadyString(strProductID)
                                        p("UnitMeasureID") = ReadyString(strUnitMeasureID)

                                        '''Find Packing Size Qty Column fixed Feature ID E1059BD9-B365-478F-A443-F465B604D251
                                        Dim intMeasurementQty As Integer
                                        intMeasurementQty = 1

                                        strExcelColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                intMeasurementQty = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                            End If
                                        End If
                                        If vEnableParsing _
                                            AndAlso pParseData.BasicUnit IsNot Nothing Then

                                            intMeasurementQty = pParseData.UnitQty
                                        End If
                                        p("Quantity") = intMeasurementQty

                                        '''Get Measurement Level
                                        strMeasurementLevelID = vbNullString
                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                            End With
                                            With .Fields
                                                With .Add("MeasurementLevel", SqlDbType.Int)
                                                    .Expression = "MeasurementLevel"
                                                End With
                                            End With
                                            With .Fields
                                                With .Add("Quantity", SqlDbType.Decimal)
                                                    .Expression = "Quantity"
                                                End With
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductMeasurement "

                                            .SqlWhere = " WHERE ProductID = " & ReadyString(strProductID)

                                            .SqlOrder = " ORDER BY MeasurementLevel, Quantity ASC"

                                            pDTMeasurement = New DataTable()
                                            pDTMeasurement = .Requery

                                        End With

                                        strMeasurementID = ""
                                        intMeasurementLevel = 1
                                        If (pDTMeasurement.Rows.Count > 0) Then
                                            intMeasurementLevel = 0
                                            Dim intMeasurementCntr = 0
                                            While (intMeasurementCntr < pDTMeasurement.Rows.Count)

                                                If (intMeasurementLevel <= 0) Then
                                                    If (cdec2(pDTMeasurement.Rows(intMeasurementCntr).Item("Quantity").ToString()) = intMeasurementQty) Then
                                                        strMeasurementID = pDTMeasurement.Rows(intMeasurementCntr).Item("ID").ToString()
                                                        intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString())
                                                        Exit While
                                                    ElseIf (cdec2(pDTMeasurement.Rows(intMeasurementCntr).Item("Quantity").ToString()) > intMeasurementQty) Then
                                                        intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString())

                                                        '''Adjust Measurement Level of Next Level
                                                        p2 = New InsertUpdate("MobizLibraryPIMProductMeasurement")
                                                        p2("MeasurementLevel") = "ISNULL(MeasurementLevel,0) + 1"
                                                        p2("UserID") = ReadyString(vPeopleUserID)
                                                        p2("LastUpdated") = "getdate()"
                                                        pSql = p2.GetUpdate(" ID = " & ReadyString(pDTMeasurement.Rows(intMeasurementCntr).Item("ID").ToString()))
                                                        mBusiness.ProData.ExecuteSQL(pSql)

                                                    End If
                                                Else
                                                    '''Adjust Measurement Level of Next Level
                                                    p2 = New InsertUpdate("MobizLibraryPIMProductMeasurement")
                                                    p2("MeasurementLevel") = "ISNULL(MeasurementLevel,0) + 1"
                                                    p2("UserID") = ReadyString(vPeopleUserID)
                                                    p2("LastUpdated") = "getdate()"
                                                    pSql = p2.GetUpdate(" ID = " & ReadyString(pDTMeasurement.Rows(intMeasurementCntr).Item("ID").ToString()))
                                                    mBusiness.ProData.ExecuteSQL(pSql)
                                                End If

                                                If (intMeasurementCntr = (pDTMeasurement.Rows.Count - 1) And intMeasurementLevel <= 0) Then
                                                    intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString()) + 1
                                                    Exit While
                                                End If

                                                intMeasurementCntr = intMeasurementCntr + 1

                                            End While

                                        End If

                                        p("MeasurementLevel") = intMeasurementLevel

                                        '''Find Content Unit Column fixed Feature ID DD4563D3-9FC5-41E4-88EB-12B45279820E
                                        strExcelColName = clsPIMUpload.GetExcelColumn("DD4563D3-9FC5-41E4-88EB-12B45279820E", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            pContentUnit = dtaRow(strExcelColName).ToString().Trim()
                                            p("ContentUnit") = ReadyString(pContentUnit)

                                        End If

                                        '''Find Content Qty Column fixed Feature ID 443C66E2-F7A3-4C8B-A287-3D5E2D1945F4
                                        strExcelColName = clsPIMUpload.GetExcelColumn("443C66E2-F7A3-4C8B-A287-3D5E2D1945F4", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                pContentQty = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                                p("ContentQty") = pContentQty
                                            End If
                                        End If

                                        '''Find Product Dimension Length Column fixed Feature ID 682E19BD-C47A-4760-B120-3DD931D5BB49
                                        strExcelColName = clsPIMUpload.GetExcelColumn("682E19BD-C47A-4760-B120-3DD931D5BB49", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                p("ProdDimensionLength") = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                            End If
                                        End If

                                        '''Find Product Dimension Unit Column fixed Feature ID 9B32E0BC-E77E-4724-9BBC-C0C0989EF95B
                                        strExcelColName = clsPIMUpload.GetExcelColumn("9B32E0BC-E77E-4724-9BBC-C0C0989EF95B", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            p("ProdDimensionLengthUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                                        End If

                                        '''Find Product Dimension Width Column fixed Feature ID E35110CC-A508-463D-B440-DAA3B4CEF792
                                        strExcelColName = clsPIMUpload.GetExcelColumn("E35110CC-A508-463D-B440-DAA3B4CEF792", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                p("ProdDimensionWidth") = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                            End If
                                        End If

                                        '''Find Product Dimension Width Unit Column fixed Feature ID 97842368-D382-4D66-A00C-6C6F5250FE0F
                                        strExcelColName = clsPIMUpload.GetExcelColumn("97842368-D382-4D66-A00C-6C6F5250FE0F", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            p("ProdDimensionWidthUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                                        End If

                                        '''Find Product Dimension Height Column fixed Feature ID AAFAE8E0-4ABA-448A-A89E-CC5F2FCF0942
                                        strExcelColName = clsPIMUpload.GetExcelColumn("AAFAE8E0-4ABA-448A-A89E-CC5F2FCF0942", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                p("ProdDimensionHeight") = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                            End If
                                        End If

                                        '''Find Product Dimension Height Unit Column fixed Feature ID 530EFD37-3303-4903-B939-9B3FF19564CC
                                        strExcelColName = clsPIMUpload.GetExcelColumn("530EFD37-3303-4903-B939-9B3FF19564CC", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            p("ProdDimensionHeightUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                                        End If

                                        '''Find Product Weight Column fixed Feature ID E1DE6123-65ED-422F-B275-C8C2A0FBCA44
                                        strExcelColName = clsPIMUpload.GetExcelColumn("E1DE6123-65ED-422F-B275-C8C2A0FBCA44", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                p("ProductWeight") = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                            End If
                                        End If

                                        '''Find Product Weight Unit Column fixed Feature ID 770319EA-1948-4FE7-B4F1-CC669E947D53
                                        strExcelColName = clsPIMUpload.GetExcelColumn("770319EA-1948-4FE7-B4F1-CC669E947D53", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            p("ProductWeightUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                                        End If

                                        p("Discount") = 0
                                        p("UserID") = ReadyString(vPeopleUserID)
                                        p("LastUpdated") = "getdate()"

                                        If (strMeasurementID.Trim().Length > 0) Then
                                            pSql = p.GetUpdate(" ID = " & ReadyString(strMeasurementID))
                                        Else
                                            strMeasurementID = Guid.NewGuid.ToString()
                                            p("ID") = ReadyString(strMeasurementID)
                                            pSql = p.GetInsert()
                                        End If

                                        mBusiness.ProData.ExecuteSQL(pSql)

                                        '''Find UPC (Barcode) Column fixed Feature ID A4B843A6-CBB9-4429-8BC7-2D571E7AB48C
                                        strExcelColName = clsPIMUpload.GetExcelColumn("A4B843A6-CBB9-4429-8BC7-2D571E7AB48C", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            '''Insert into MobizLibraryPIMProductMeasurementBarcode
                                            If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                                pFinder = New Finder
                                                With pFinder
                                                    .ProData = mBusiness.ProData
                                                    .MaxRecords = MaxReturnRow

                                                    With .Fields
                                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                                            .Expression = "ID"
                                                            .Searchable = False
                                                        End With
                                                    End With

                                                    .SqlFrom = "FROM MobizLibraryPIMProductMeasurementBarcode "

                                                    .SqlWhere = " WHERE StockMeasurementsID = " & ReadyString(strMeasurementID)

                                                    pDT = New DataTable()
                                                    pDT = .Requery

                                                End With

                                                p = New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")

                                                p("StockMeasurementsID") = ReadyString(strMeasurementID)
                                                p("Barcode") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                                                p("ReadyBarcode") = ReadyString(dtaRow(strExcelColName).ToString().Trim())

                                                If (pDT.Rows.Count > 0) Then
                                                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                                Else
                                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                    pSql = p.GetInsert()
                                                End If

                                                mBusiness.ProData.ExecuteSQL(pSql)

                                            End If
                                            ''' End Insert into MobizLibraryPIMProductMeasurementBarcode
                                        End If
                                    End If

                                End If
                                '''End Insert into Product Dimension Length----------------
                            Else
                                '''Find Packing Size Qty Column fixed Feature ID E1059BD9-B365-478F-A443-F465B604D251
                                Dim intMeasurementQty As Integer
                                intMeasurementQty = 1

                                strExcelColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                        intMeasurementQty = cdec2(dtaRow(strExcelColName).ToString().Trim())
                                    End If
                                End If

                                '''Get Measurement Level
                                strMeasurementLevelID = vbNullString
                                pFinder = New Finder
                                With pFinder
                                    .ProData = mBusiness.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "ID"
                                            .Searchable = False
                                        End With
                                    End With
                                    With .Fields
                                        With .Add("MeasurementLevel", SqlDbType.Int)
                                            .Expression = "MeasurementLevel"
                                        End With
                                    End With
                                    With .Fields
                                        With .Add("Quantity", SqlDbType.Decimal)
                                            .Expression = "Quantity"
                                        End With
                                    End With

                                    .SqlFrom = "FROM MobizLibraryPIMProductMeasurement "

                                    .SqlWhere = " WHERE ProductID = " & ReadyString(strProductID)

                                    .SqlOrder = " ORDER BY Quantity ASC"

                                    pDTMeasurement = New DataTable()
                                    pDTMeasurement = .Requery

                                End With

                                strMeasurementID = ""
                                intMeasurementLevel = 1
                                If (pDTMeasurement.Rows.Count > 0) Then
                                    intMeasurementLevel = 0
                                    Dim intMeasurementCntr = 0
                                    While (intMeasurementCntr < pDTMeasurement.Rows.Count)

                                        If (intMeasurementLevel <= 0) Then
                                            If (cdec2(pDTMeasurement.Rows(intMeasurementCntr).Item("Quantity").ToString()) = intMeasurementQty) Then
                                                strMeasurementID = pDTMeasurement.Rows(intMeasurementCntr).Item("ID").ToString()
                                                intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString())
                                                Exit While
                                            ElseIf (cdec2(pDTMeasurement.Rows(intMeasurementCntr).Item("Quantity").ToString()) > intMeasurementQty) Then
                                                intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString())

                                            End If

                                        End If

                                        If (intMeasurementCntr = (pDTMeasurement.Rows.Count - 1) And intMeasurementLevel <= 0) Then
                                            intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString()) + 1
                                            Exit While
                                        End If

                                        intMeasurementCntr = intMeasurementCntr + 1

                                    End While

                                End If


                            End If

                            If Now.Date = New Date(2022, 3, 1) Then
                                AppendToLog("" & "pProductNum = " & pProductNum & vbCrLf & _
                                    "pProductName = " & pProductName & vbCrLf &
                                    "intMeasurementLevel = " & intMeasurementLevel, , "vbHTL4UY563U!")
                            End If

                            'If (intMeasurementLevel = 1) Then
                            If True Then

                                '''Insert into MobizLibraryPIMProductPriceLevel
                                '''Find Suggested Retail Price Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C99
                                strExcelColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C99", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = 'eef716c6-b290-4b28-b2a6-0a4f52cf4989'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'eef716c6-b290-4b28-b2a6-0a4f52cf4989'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then

                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If
                                End If

                                '''Find Retail Price Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C90
                                strExcelColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C90", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '01000012-0005-0000-0000-000000000000'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'01000012-0005-0000-0000-000000000000'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                Dim count As Integer = 0
                                                For Each obj As Object In pDT.Rows
                                                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))


                                                    If count > 0 Then
                                                        'delete all other duplicate retail price
                                                        pSql = "DELETE FROM MobizLibraryPIMProductPriceLevel WHERE ID = " & ReadyString(obj("ID").ToString())
                                                    End If

                                                    count = count + 1
                                                Next obj


                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If

                                '''Find Wholesale Price Column fixed Feature ID 997373c6-b8f1-4912-b03c-7b3bffd74563
                                strExcelColName = clsPIMUpload.GetExcelColumn("997373c6-b8f1-4912-b03c-7b3bffd74563", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '01000012-0004-0000-0000-000000000000'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'01000012-0004-0000-0000-000000000000'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If Now.Date = New Date(2022, 3, 1) Then
                                            Dim pLog = "pProductNum = " & pProductNum & vbCrLf & _
                                                "pProductName = " & pProductName & vbCrLf
                                            pLog &= "Excel price = " & CDec2(p("Price")) & vbCrLf
                                            If pDT.Rows.Count > 0 Then
                                                pLog &= "Database price = " & CDec2(pDT.Rows(0)("Price")) & vbCrLf
                                                pLog &= "Diff = " & Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) & vbCrLf
                                            End If
                                            pLog &= "mThresholdToSavePrice = " & mThresholdToSavePrice & vbCrLf
                                            AppendToLog("" & pLog, , "vbHTL4UY563U!")
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        Else

                                        End If
                                    End If

                                End If

                                '''Find NCR Price Column fixed Feature ID 7655456c-2cf3-4e23-8f64-b63acf21525c
                                strExcelColName = clsPIMUpload.GetExcelColumn("7655456c-2cf3-4e23-8f64-b63acf21525c", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '079C7533-F853-4EE7-9FC8-D29A3113D0D6'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'079C7533-F853-4EE7-9FC8-D29A3113D0D6'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim())
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If


                                '''Find Provincial Price Price Column fixed Feature ID 7655456c-2cf3-4e23-8f64-b63acf21525d
                                strExcelColName = clsPIMUpload.GetExcelColumn("7655456c-2cf3-4e23-8f64-b63acf21525d", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '079c7533-f853-4ee7-9fc8-d29a3113d0d7'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'079c7533-f853-4ee7-9fc8-d29a3113d0d7'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If

                                '''Find VISMIN Price Price Column fixed Feature ID 7655456c-2cf3-4e23-8f64-b63acf21525e
                                strExcelColName = clsPIMUpload.GetExcelColumn("7655456c-2cf3-4e23-8f64-b63acf21525e", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '079c7533-f853-4ee7-9fc8-d29a3113d0d8'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'079c7533-f853-4ee7-9fc8-d29a3113d0d8'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If

                                '''Find VISAYAS Price Price Column fixed Feature ID F741E903-F5F3-4299-B54F-D33908B455CE
                                strExcelColName = clsPIMUpload.GetExcelColumn("F741E903-F5F3-4299-B54F-D33908B455CE", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '079c7533-f853-4ee7-9fc8-d29a3113d0e6'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'079c7533-f853-4ee7-9fc8-d29a3113d0e6'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If

                                '''Find MINDANAO Price Price Column fixed Feature ID 0759C803-0654-4345-A769-39F81C4F1F73
                                strExcelColName = clsPIMUpload.GetExcelColumn("0759C803-0654-4345-A769-39F81C4F1F73", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = '079c7533-f853-4ee7-9fc8-d29a3113d0f6'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'079c7533-f853-4ee7-9fc8-d29a3113d0f6'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If

                                '''Find CEBU Price Column fixed Feature ID 7655456c-2cf3-4e23-8f64-b63acf21525b
                                strExcelColName = clsPIMUpload.GetExcelColumn("7655456c-2cf3-4e23-8f64-b63acf21525b", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                        pFinder = New Finder
                                        With pFinder
                                            .ProData = mBusiness.ProData
                                            .MaxRecords = MaxReturnRow

                                            With .Fields
                                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                                    .Expression = "ID"
                                                    .Searchable = False
                                                End With
                                                .Add("Price", SqlDbType.Money)
                                            End With

                                            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                            .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                " AND ProductID = " & ReadyString(strProductID) &
                                                " AND PriceLevelID = 'c5af3c2c-8cc2-4bfa-a592-620d83368df1'"

                                            pDT = New DataTable()
                                            pDT = .Requery

                                        End With

                                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                        p("OrganizationID") = ReadyString(vOrgID)
                                        p("ProductID") = ReadyString(strProductID)
                                        p("PriceLevelID") = "'c5af3c2c-8cc2-4bfa-a592-620d83368df1'"

                                        If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                            Dim pUnitQty = 1D
                                            If vEnableParsing Then
                                                pUnitQty = pParseData.UnitQty
                                                If pUnitQty <= 0 Then
                                                    pUnitQty = 1
                                                End If
                                            End If
                                            p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                        End If

                                        If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            If (pDT.Rows.Count > 0) Then
                                                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                            Else
                                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                pSql = p.GetInsert()
                                            End If

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If
                                    End If

                                End If

                                '''Find Shipping Cost Column fixed Feature ID 2254751E-E1FC-4704-B494-A39FDBA2A245
                                strExcelColName = clsPIMUpload.GetExcelColumn("2254751E-E1FC-4704-B494-A39FDBA2A245", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then
                                        If vDoNotOverwriteBomLandedCost = False _
                                            OrElse pHasBomContents = False Then

                                            pFinder = New Finder
                                            With pFinder
                                                .ProData = mBusiness.ProData
                                                .MaxRecords = MaxReturnRow

                                                With .Fields
                                                    With .Add("ID", SqlDbType.UniqueIdentifier)
                                                        .Expression = "ID"
                                                        .Searchable = False
                                                    End With
                                                    .Add("Price", SqlDbType.Money)
                                                End With

                                                .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                                .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                    " AND ProductID = " & ReadyString(strProductID) &
                                                    " AND PriceLevelID = '01000012-0001-0000-0000-000000000000'"

                                                pDT = New DataTable()
                                                pDT = .Requery

                                            End With

                                            p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                            p("OrganizationID") = ReadyString(vOrgID)
                                            p("ProductID") = ReadyString(strProductID)
                                            p("PriceLevelID") = "'01000012-0001-0000-0000-000000000000'"

                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                Dim pUnitQty = 1D
                                                If vEnableParsing Then
                                                    pUnitQty = pParseData.UnitQty
                                                    If pUnitQty <= 0 Then
                                                        pUnitQty = 1
                                                    End If
                                                End If
                                                p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                            End If

                                            If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                                p("DeactivateDiscount") = 0
                                                p("UserID") = ReadyString(vPeopleUserID)
                                                p("LastUpdated") = "getdate()"

                                                If (pDT.Rows.Count > 0) Then
                                                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                                Else
                                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                    pSql = p.GetInsert()
                                                End If

                                                mBusiness.ProData.ExecuteSQL(pSql)
                                            End If
                                        End If
                                    End If
                                End If

                                '''Find Cost from Vendor Column fixed Feature ID E531191E-203E-41CC-BA9B-C46A27D10CD8
                                strExcelColName = clsPIMUpload.GetExcelColumn("E531191E-203E-41CC-BA9B-C46A27D10CD8", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then
                                        If vDoNotOverwriteBomLandedCost = False _
                                            OrElse pHasBomContents = False Then
                                            pFinder = New Finder
                                            With pFinder
                                                .ProData = mBusiness.ProData
                                                .MaxRecords = MaxReturnRow

                                                With .Fields
                                                    With .Add("ID", SqlDbType.UniqueIdentifier)
                                                        .Expression = "ID"
                                                        .Searchable = False
                                                    End With
                                                    .Add("Price", SqlDbType.Money)
                                                End With

                                                .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                                .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                    " AND ProductID = " & ReadyString(strProductID) &
                                                    " AND PriceLevelID = '01000012-0002-0000-0000-000000000000'"

                                                pDT = New DataTable()
                                                pDT = .Requery

                                            End With

                                            p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                            p("OrganizationID") = ReadyString(vOrgID)
                                            p("ProductID") = ReadyString(strProductID)
                                            p("PriceLevelID") = "'01000012-0002-0000-0000-000000000000'"

                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                Dim pUnitQty = 1D
                                                If vEnableParsing Then
                                                    pUnitQty = pParseData.UnitQty
                                                    If pUnitQty <= 0 Then
                                                        pUnitQty = 1
                                                    End If
                                                End If
                                                p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                            End If

                                            If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                                p("DeactivateDiscount") = 0
                                                p("UserID") = ReadyString(vPeopleUserID)
                                                p("LastUpdated") = "getdate()"

                                                If (pDT.Rows.Count > 0) Then
                                                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                                Else
                                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                    pSql = p.GetInsert()
                                                End If

                                                mBusiness.ProData.ExecuteSQL(pSql)
                                            End If
                                        End If

                                    End If
                                End If

                                '''Find Landed Cost Column fixed Feature ID 1eb487fc-8b91-4d0c-901f-16bf4b80a9a3
                                strExcelColName = clsPIMUpload.GetExcelColumn("1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", DTemplate)
                                If (strExcelColName.Trim().Length > 0) Then
                                    If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then
                                        If vDoNotOverwriteBomLandedCost = False _
                                            OrElse pHasBomContents = False Then

                                            pFinder = New Finder
                                            With pFinder
                                                .ProData = mBusiness.ProData
                                                .MaxRecords = MaxReturnRow

                                                With .Fields
                                                    With .Add("ID", SqlDbType.UniqueIdentifier)
                                                        .Expression = "ID"
                                                        .Searchable = False
                                                    End With
                                                    .Add("Price", SqlDbType.Money)
                                                End With

                                                .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                                .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                    " AND ProductID = " & ReadyString(strProductID) &
                                                    " AND PriceLevelID = '01000012-0003-0000-0000-000000000000'"

                                                pDT = New DataTable()
                                                pDT = .Requery

                                            End With

                                            p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                            p("OrganizationID") = ReadyString(vOrgID)
                                            p("ProductID") = ReadyString(strProductID)
                                            p("PriceLevelID") = "'01000012-0003-0000-0000-000000000000'"

                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                Dim pUnitQty = 1D
                                                If vEnableParsing Then
                                                    pUnitQty = pParseData.UnitQty
                                                    If pUnitQty <= 0 Then
                                                        pUnitQty = 1
                                                    End If
                                                End If
                                                pLandedCost = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                                p("Price") = pLandedCost
                                            End If

                                            If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                                p("DeactivateDiscount") = 0
                                                p("UserID") = ReadyString(vPeopleUserID)
                                                p("LastUpdated") = "getdate()"

                                                If (pDT.Rows.Count > 0) Then
                                                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                                Else
                                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                    pSql = p.GetInsert()
                                                End If

                                                mBusiness.ProData.ExecuteSQL(pSql)
                                            End If
                                        End If
                                    End If
                                End If


                                'Get All Price Level
                                pFinder = New Finder
                                With pFinder
                                    .ProData = mBusiness.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "lvl.ID"
                                            .Searchable = False
                                        End With

                                        .Add("PriceLevel", SqlDbType.UniqueIdentifier)
                                    End With

                                    .SqlFrom = " FROM MobizLibraryPIMPriceLevel as lvl   "

                                    .SqlWhere = " WHERE (lvl.IsActive = 1 AND lvl.OrgID IS NULL OR lvl.OrgID =  " & ReadyString(vOrgID) & " )  " & vbCrLf &
                                                " AND lvl.ID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000', " & vbCrLf &
                                                " '01000012-0003-0000-0000-000000000000', '01000012-0004-0000-0000-000000000000', '01000012-0005-0000-0000-000000000000', " & vbCrLf &
                                                " 'eef716c6-b290-4b28-b2a6-0a4f52cf4989')   "

                                    pDTPriceLevel = New DataTable()
                                    pDTPriceLevel = .Requery

                                End With

                                If (pDTPriceLevel.Rows.Count > 0) Then
                                    For i As Integer = 0 To pDTPriceLevel.Rows.Count - 1
                                        strExcelColName = clsPIMUpload.GetExcelColumn(CStr2(pDTPriceLevel.Rows(i).Item("ID")), DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then

                                                pFinder = New Finder
                                                With pFinder
                                                    .ProData = mBusiness.ProData
                                                    .MaxRecords = MaxReturnRow

                                                    With .Fields
                                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                                            .Expression = "ID"
                                                            .Searchable = False
                                                        End With
                                                        .Add("Price", SqlDbType.Money)
                                                    End With

                                                    .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel "

                                                    .SqlWhere = " WHERE OrganizationID = " & ReadyString(vOrgID) &
                                                        " AND ProductID = " & ReadyString(strProductID) &
                                                        " AND PriceLevelID = " & ReadyString(CStr2(pDTPriceLevel.Rows(i).Item("ID")))

                                                    pDT = New DataTable()
                                                    pDT = .Requery

                                                End With

                                                p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                                p("OrganizationID") = ReadyString(vOrgID)
                                                p("ProductID") = ReadyString(strProductID)
                                                p("PriceLevelID") = ReadyString(CStr2(pDTPriceLevel.Rows(i).Item("ID")))

                                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                    Dim pUnitQty = 1D
                                                    If vEnableParsing Then
                                                        pUnitQty = pParseData.UnitQty
                                                        If pUnitQty <= 0 Then
                                                            pUnitQty = 1
                                                        End If
                                                    End If
                                                    p("Price") = CDec2(dtaRow(strExcelColName).ToString().Trim()) / pUnitQty
                                                End If

                                                If pDT.Rows.Count = 0 _
                                            OrElse Math.Abs(CDec2(p("Price")) - CDec2(pDT.Rows(0)("Price"))) > mThresholdToSavePrice Then
                                                    p("DeactivateDiscount") = 0
                                                    p("UserID") = ReadyString(vPeopleUserID)
                                                    p("LastUpdated") = "getdate()"

                                                    If (pDT.Rows.Count > 0) Then
                                                        pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                                                    Else
                                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                                        pSql = p.GetInsert()
                                                    End If

                                                    mBusiness.ProData.ExecuteSQL(pSql)
                                                End If
                                            End If

                                        End If
                                    Next
                                End If
                                '''End Insert into MobizLibraryPIMProductPriceLevel----------------
                            Else ' ELSE If (intMeasurementLevel = 1) Then

                            End If ' END  If (intMeasurementLevel = 1) Then

                            '''Find Retail Price Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C90
                            strExcelColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C90", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (dtaRow(strExcelColName).ToString().Trim().Length > 0) Then
                                    If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                        Dim pUnitQty = 1D
                                        If vEnableParsing Then
                                            pUnitQty = pParseData.UnitQty
                                            If pUnitQty > 1 Then
                                                mBusiness.LibraryStock.UpdateDiscount(strProductID, CDec2(dtaRow(strExcelColName).ToString().Trim()), pUnitQty, vOrgID)
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            '''Insert into LibraryOrganizationPIM
                            If (strProductID.Trim().Length > 0) Then
                                pFinder = New Finder
                                With pFinder
                                    .ProData = mBusiness.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "orgprd.ProductID"
                                            .Searchable = False
                                        End With
                                    End With

                                    .SqlFrom = "FROM LibraryOrganizationPIM orgprd "

                                    .SqlWhere = " WHERE orgprd.ProductID = " & ReadyString(strProductID)

                                    pDT = New DataTable()
                                    pDT = .Requery

                                End With

                                If (pDT.Rows.Count <= 0) Then '''If not exist in the structured option, insert
                                    p = New InsertUpdate("LibraryOrganizationPIM")

                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("ProductID") = ReadyString(strProductID)
                                    p("ActivationDate") = "getdate()"

                                    If IsHasDisableColumn Then IIf(dtaRow("Is Deactivated").ToString().Trim().ToLower = "yes", 1, 0)
                                    If IsHasSCDPWDColumn Then CInt2(dtaRow("EligibleForSCDPWDDiscount").ToString().Trim())
                                    If IsHasProductAvailableToMobileColumn Then IIf(dtaRow("Include in mobile database").ToString().Trim().ToLower() = "no", 0, 1)
                                    'p("IsDeactivated") = CInt2(dtaRow("Disable").ToString().Trim())
                                    p("IsDoNotShareTransDataToBrandOwner") = "0"

                                    If IsHasInventoryCountItemColumn Then
                                        If WebFormIsInventoryCountItem <> Nothing Then
                                            p("IsInventoryCountItem") = CInt2(WebFormIsInventoryCountItem)
                                        Else
                                            p("IsInventoryCountItem") = IIf(dtaRow("This Is An Inventory-Count Item?").ToString().Trim().ToLower = "no", 0, 1)
                                        End If
                                    End If

                                    pSql = p.GetInsert()
                                    mBusiness.ProData.ExecuteSQL(pSql)


                                    Dim dtFOWST As DataTable

                                    pSql = "Select ID from MobizLibraryOrganizationFOWST where organizationid = " & ReadyString(vOrgID)
                                    dtFOWST = mProData.Execute(pSql)

                                    ' commented for speed 2019_12_27__15_54_43_0564
                                    'For i As Integer = 0 To dtFOWST.Rows.Count - 1

                                    '    mBusiness.TransInventoryLedger.Add( _
                                    '       vStockID:=CStr2(strProductID), _
                                    '        vFowstID:=CStr2(dtFOWST.Rows(i).Item("ID")), _
                                    '        vTransactionID:=Guid.NewGuid.ToString, _
                                    '        vTransactionType:=TransInventoryLedger.eTransType.BeginningBalance, _
                                    '        vQty:=0.0,
                                    '        vBalance:=0,
                                    '        vUserID:=vPeopleUserID
                                    '    )

                                    'Next

                                Else
                                    If IsHasInventoryCountItemColumn Then
                                        p = New InsertUpdate("LibraryOrganizationPIM")

                                        If WebFormIsInventoryCountItem <> Nothing Then
                                            p("IsInventoryCountItem") = CInt2(WebFormIsInventoryCountItem)
                                        Else
                                            p("IsInventoryCountItem") = IIf(dtaRow("This Is An Inventory-Count Item?").ToString().Trim().ToLower = "no", 0, 1)
                                        End If

                                        pSql = p.GetUpdate("ProductID = " & ReadyString(pDT.Rows(0).Item("ID")))
                                        mBusiness.ProData.ExecuteSQL(pSql)
                                    End If

                                    If IsHasDisableColumn Then
                                        p = New InsertUpdate("LibraryOrganizationPIM")

                                        p("IsDeactivated") = IIf(dtaRow("Is Deactivated").ToString().Trim().ToLower = "yes", 1, 0)

                                        pSql = p.GetUpdate("ProductID = " & ReadyString(pDT.Rows(0).Item("ID")))
                                        mBusiness.ProData.ExecuteSQL(pSql)
                                    End If

                                    If IsHasProductAvailableToMobileColumn Then
                                        p = New InsertUpdate("LibraryOrganizationPIM")

                                        p("ProductAvailableToMobile") = IIf(dtaRow("Include in mobile database").ToString().Trim().ToLower() = "no", 0, 1) 'CInt2(dtaRow("Include in mobile database").ToString().Trim())

                                        pSql = p.GetUpdate("ProductID = " & ReadyString(pDT.Rows(0).Item("ID")))
                                        mBusiness.ProData.ExecuteSQL(pSql)
                                    End If

                                    If IsHasSCDPWDColumn Then
                                        p = New InsertUpdate("LibraryOrganizationPIM")

                                        p("IsVatExempt") = CInt2(dtaRow("EligibleForSCDPWDDiscount").ToString().Trim())

                                        pSql = p.GetUpdate("ProductID = " & ReadyString(pDT.Rows(0).Item("ID")))
                                        mBusiness.ProData.ExecuteSQL(pSql)
                                    End If

                                End If

                            End If
                            '''End Insert into LibraryOrganizationPIM----------------

                            '''Save Features
                            p = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")
                            p("ProductID") = ReadyString(strProductID)

                            'p2 = New InsertUpdate("MobizLibraryPIMProductVariableFeatures002")
                            'p2("ProductID") = ReadyString(strProductID)

                            intTemplateColCntr = 0
                            While (intTemplateColCntr < DTemplate.Rows.Count)

                                If (DTemplate.Rows(intTemplateColCntr).Item("PIMFeatureTable").ToString().ToUpper() = ("LibraryPIMProductVariableFeatures001").ToUpper()) Then
                                    If (DTemplate.Rows(intTemplateColCntr).Item("PIMFeatureColumn").ToString().Trim().Length > 0) Then
                                        p(DTemplate.Rows(intTemplateColCntr).Item("PIMFeatureColumn").ToString().Trim()) = ReadyString(dtaRow(DTemplate.Rows(intTemplateColCntr).Item("ColumnName").ToString().Trim()).ToString().Trim())
                                    End If
                                    'ElseIf (DTemplate.Rows(intTemplateColCntr).Item("PIMFeatureTable").ToString().ToUpper() = ("LibraryPIMProductVariableFeatures002").ToUpper()) Then
                                    '    If (DTemplate.Rows(intTemplateColCntr).Item("PIMFeatureColumn").ToString().Trim().Length > 0) Then
                                    '        p2(DTemplate.Rows(intTemplateColCntr).Item("PIMFeatureColumn").ToString().Trim()) = ReadyString(dtaRow(DTemplate.Rows(intTemplateColCntr).Item("ColumnName").ToString().Trim()).ToString().Trim())
                                    '    End If
                                End If

                                intTemplateColCntr = intTemplateColCntr + 1
                            End While
                            '''End save Features

                            Dim strProductFeature001ID As String

                            '''Check if Product Feature Exist
                            pFinder = New Finder
                            With pFinder
                                .ProData = mBusiness.ProData
                                .MaxRecords = MaxReturnRow

                                With .Fields
                                    With .Add("ID", SqlDbType.UniqueIdentifier)
                                        .Expression = "prdfea.ID"
                                    End With
                                End With

                                .SqlFrom = "FROM MobizLibraryPIMProductVariableFeatures001 prdfea "

                                .SqlWhere = "WHERE prdfea.ProductID = " & ReadyString(strProductID)

                                pDT = .Requery
                            End With

                            strProductFeature001ID = ""
                            If (pDT.Rows.Count > 0) Then
                                strProductFeature001ID = pDT.Rows(0).Item("ID").ToString().Trim()
                            End If

                            If (strProductFeature001ID.Trim().Length <= 0) Then
                                strProductFeature001ID = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strProductFeature001ID)

                                pSql = p.GetInsert()
                            Else
                                pSql = p.GetUpdate(" ID = " & ReadyString(strProductFeature001ID))
                            End If
                            mBusiness.ProData.ExecuteSQL(pSql)

                            ' commented 2016_09_05__10_55_49_9572
                            ' causes the PIM to have a "PC" unit
                            ' mBusiness.LibraryStock.AddPim(vOrgID, pProductNum, pProductName) 

                            intProductCntr = intProductCntr + 1
                        End While

                        'pSql = p2.GetInsert()
                        'Common.WebRequest.ProData.ExecuteSQL(pSql)

                        intCntr = intCntr + 1

                        'added by arnie 8122017 this is for Assets Management Module
                        If CBool2(IsAsset) Then
                            p = New InsertUpdate("LibraryRentableAssets")

                            pSql = "SELECT ProductID FROM LibraryRentableAssets WHERE ProductID = " & ReadyString(strProductID)

                            'do not save existing record
                            If mBusiness.ProData.Execute(pSql).Rows.Count = 0 Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("ProductID") = ReadyString(strProductID)
                                p("LastUpdated") = "getdate()"
                                p("CreatedBy") = ReadyString(vPeopleUserID)
                                mBusiness.ProData.ExecuteSQL(p.GetInsert())
                            End If

                            IsAsset = False
                        End If

                        If dtaRow.Table.Columns.Contains("Upcoming Retail Price") And dtaRow.Table.Columns.Contains("Upcoming Retail Price Effectivity Date") Then
                            If Not dtaRow("Upcoming Retail Price") = "" And Not dtaRow("Upcoming Retail Price Effectivity Date") = "" Then
                                'both must have values
                                p = New InsertUpdate("LibraryPimUpcomingPrices")

                                p("PriceLevelID") = ReadyString("01000012-0005-0000-0000-000000000000") 'RETAIL PRICE
                                p("EffectivityDate") = ReadyString(CDate(dtaRow("Upcoming Retail Price Effectivity Date")))
                                p("Price") = ReadyString(dtaRow("Upcoming Retail Price"))
                                p("LastUpdated") = "getdate()"
                                p("UpdatedBy") = ReadyString(vPeopleUserID)

                                If mBusiness.ProData.ExecuteSQL(p.GetUpdate(" ProductID = " & ReadyString(strProductID))) = 0 Then
                                    p("ProductID") = ReadyString(strProductID)
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    mBusiness.ProData.Execute(p.GetInsert())
                                End If
                            End If
                        End If

                        If String.IsNullOrWhiteSpace(pSupplierID) = False _
                            AndAlso pLandedCost IsNot Nothing Then

                            mBusiness.LibraryPriceCanvass.UpdateLibraryVendorPrices(pSupplierID, strProductID, pLandedCost, 1)
                        End If

                        If IsHasBeginningBalanceColumn Then
                            If CDec2(dtaRow("Beginning Balance")) > 0 Then

                                If Not mCurrentUploader.FOWSTName = vbNullString Then

                                    FOWSTID = GetFOWSTID()

                                    If Not FOWSTID = vbNullString Then
                                        BinID = GetDefaultBIN(strProductID, FOWSTID)

                                        If CStr2(BinID).Length = 0 Then
                                            BinID = mBusiness.SystemDefaults.GetOrAddDefaultBin(FOWSTID, strProductID)
                                        End If

                                        If Not mCurrentUploader.BeginningBalanceDateAndTime = vbNullString Then
                                            SetBeginningBalance(CDec2(dtaRow("Beginning Balance")), strProductID, FOWSTID, BinID)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If pProductIDs.Contains(ReadyGuid(strProductID)) = False Then
                            pProductIDs.Add(ReadyGuid(strProductID))
                        End If

                        If CDec2(pContentQty > 0) _
                            AndAlso String.IsNullOrWhiteSpace(pContentUnit) = False _
                            AndAlso IsGuid(strUnitMeasureID) _
                            AndAlso IsGuid(strProductID) Then

                            mBusiness.LibraryStock.CreateUnitDefinition(
                                vProductID:=strProductID,
                                vDestinationMeasure:=pContentUnit,
                                vQuantity:=pContentQty,
                                vSourceMeasureID:=strUnitMeasureID,
                                vOrgID:=vOrgID)
                        End If
                        mBusiness.ProData.CommitTransaction()
                    End While

                    If vDoNotOverwriteBomLandedCost Then
                        mBusiness.LibraryStock.UpdateParentProductPrice(pProductIDs, vOrgID)
                    End If

                    If IsHasBeginningBalanceColumn Then
                        If Not FOWSTID = vbNullString Then
                            mBusiness.TransInventoryLedger.UpdateLibraryFowstBinProductDetails(FOWSTID)
                        End If
                    End If


                    mBusiness.ProData.BeginTransaction()

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductMeasurementBarcode'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductPriceLevel'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMMeasure'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMMeasureSign'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductPriceLevel'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    mBusiness.ProData.CommitTransaction()

                End If

                blnRet = True

            Catch ex As ProSqlException
                Dim pErrorKey = GenerateString(10)
                AppendToDebug("ref: vbB5RGMWHBJW! ErrorKey: " & pErrorKey & ". intCntr: " & intCntr & vbCrLf &
                    ex.Message & vbCrLf & ex.Sql, True, ex, True)
                mCurrentUploader = Nothing
                mBusiness.ProData.RollbackTransaction(, True)
                Throw New ApplicationException(ex.Message & " pErrorKey: " & pErrorKey, ex)
            Catch ex As Exception
                Dim pErrorKey = GenerateString(10)
                AppendToDebug("ref: vbB5RGMWHBJW! ErrorKey: " & pErrorKey & ". intCntr: " & intCntr & vbCrLf &
                    ex.Message, True, ex, True)
                mCurrentUploader = Nothing
                mBusiness.ProData.RollbackTransaction(, True)
                Throw New ApplicationException(ex.Message & " pErrorKey: " & pErrorKey, ex)
            End Try

            Return blnRet

        End Function

        <HttpPost()>
        Public Function UploadRequestFollowUp() As String

            Return strExcelFileProgress

        End Function

        <HttpPost()>
        Function IsProductNoUnique(ByVal productNo As String) As String
            Dim clsPIMUpload As New PIMUpload
            Dim dtaTable As New DataTable

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            dtaTable = clsPIMUpload.CheckProductNoAndNameExistByOrg(productNo)

            If (dtaTable.Rows.Count > 0) Then
                Return dtaTable.Rows(0).Item("ProductName").ToString()
            Else
                Return "True"
            End If

        End Function


        <HttpPost()>
        Function CheckExcelFileData(ByVal OrganizationID As String) As String
            Dim strRet As String
            Dim blnResult As Boolean = False
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim clsPIMUpload As New PIMUpload
            Dim dtaTemplate As New DataTable
            Dim dtaExcelData As New DataTable
            Dim strHeader() As String
            Dim pFS As IO.FileStream
            Dim intCntr As Integer
            Dim strProductNo As String
            Dim strPackingSizeQty As String

            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim dtaExcelHeaders As New DataTable
            Dim pTotalExcelColumn As Integer = 100
            Dim objRow As Object
            Dim pX As Integer
            Dim dtaTableRequiredFields As DataTable
            Dim pOut As New JsonDictionary

            Dim intRow As Integer
            Dim blnIsRowOK As Boolean
            Dim blnIsAllRowOk As Boolean = True
            Dim strColName As String
            Dim strMessage As String
            Dim strFileNameError As String = ""

            Dim pFinder As Finder
            Dim dtaExcelPriceLevel As New DataTable
            Dim blnIsAllPricesOk As Boolean = True
            Dim strPriceDiscountDiff As String = ""
            Dim pExcelFilename As String
            Dim pExcelHeaders As Dictionary(Of String, Integer)
            Dim pMaxDataColumn As Integer
            Dim mUploaderKey = Common.WebRequest.Data("UploaderKey")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (OrganizationID Is Nothing) Then
                OrganizationID = WebRequest.Instance.OrgID
            End If
            OrganizationID = ReadyGuid(OrganizationID)

            Try
                If String.IsNullOrWhiteSpace(mUploaderKey) Then
                    Return JsonDictionary.Serialize("Error", "UploaderKey must not be blank. ref: vb8WXVJNR5Q4!")
                End If
                Do Until Monitor.TryEnter(mIndexLock)
                    Thread.Sleep(1000)
                Loop
                If mCurrentUploader IsNot Nothing _
                    AndAlso mCurrentUploader.UploaderKey <> mUploaderKey Then

                    Return JsonDictionary.Serialize("Error", "Another uploader is currently uploading. Latest activity was on " & Format(mCurrentUploader.LastUpdated, cGlobal.FormatDateTimeGrid) & ". Please upload again after a few minutes. ref: vbZ7EJSTXYJD!")
                End If
                If mCurrentUploader IsNot Nothing Then
                    mCurrentUploader.LastUpdated = Now()
                End If
            Finally
                Try
                    Monitor.Exit(mIndexLock)
                Catch ex As Exception
                    mCurrentUploader = Nothing
                    ' just ignore the error
                End Try
            End Try

            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(mProData.GetCurrentDatabase()))

            pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls", IO.FileMode.Open)
            pWB = New Workbook
            pWB.Open(pFS)
            pWS = pWB.Worksheets(0)
            pMaxDataColumn = pWS.Cells.MaxDataColumn

            'Define a Style -OK Rows
            Dim style As Style = pWB.Styles(pWB.Styles.Add())

            'Setting the foreground color to red
            style.ForegroundColor = Color.White

            'Setting the font color
            style.Font.Color = Color.Black

            'Setting the background pattern
            style.Pattern = BackgroundType.Solid

            Dim styleFlag As New StyleFlag
            styleFlag.CellShading = True
            styleFlag.FontColor = True

            'Define a Style - Error
            Dim styleError As Style = pWB.Styles(pWB.Styles.Add())

            'Setting the foreground color to red
            styleError.ForegroundColor = Color.Red

            'Setting the font color
            styleError.Font.Color = Color.White

            'Setting the background pattern
            styleError.Pattern = BackgroundType.Solid

            Dim styleFlagError As New StyleFlag
            styleFlagError.CellShading = True
            styleFlagError.FontColor = True

            ''Get Headers
            dtaExcelHeaders.Columns.Add("ExcelColumn")
            For pX = 0 To pTotalExcelColumn
                objRow = CStr2(pWS.Cells(0, pX).Value).Trim

                If (objRow.ToString().Trim().Length > 0) Then
                    dtaExcelHeaders.Rows.Add(objRow)
                End If
            Next pX

            dtaTemplate = clsPIMUpload.GetTemplateColumns(dtaExcelHeaders, OrganizationID)
            pExcelFilename = pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls"

            If (dtaTemplate.Rows.Count > 0) Then

                '''Check for required fields
                dtaTableRequiredFields = clsPIMUpload.CheckRequiredFields(dtaTemplate)
                If (dtaTableRequiredFields.Rows.Count > 0) Then

                    pOut.Add("PIMExcelUpload", "NeedRequiredFields")
                    pOut.Add("PIMExcelUploadRequired", dtaTableRequiredFields)

                    Try
                        If pFS IsNot Nothing Then pFS.Dispose()
                    Catch ex As Exception
                        mCurrentUploader = Nothing
                    End Try

                    Return pOut.Serialize
                End If

                '''Set the Headers
                ReDim Preserve strHeader(dtaTemplate.Rows.Count - 1)
                intCntr = 0
                While (intCntr < dtaTemplate.Rows.Count)
                    strHeader(intCntr) = dtaTemplate.Rows(intCntr).Item("ColumnName").ToString()
                    intCntr = intCntr + 1
                End While
                Dim pColumnList As New List(Of String)
                For Each pDR As DataRow In dtaTemplate.Rows
                    pColumnList.Add(CStr2(pDR("ColumnName")))
                Next pDR
                pColumnList = UniqueItems(pColumnList)
                strHeader = pColumnList.ToArray()
                pExcelHeaders = FindExcelHeaders(pWS, strHeader, Nothing)

                '''Save The Excel File Data
                dtaExcelData = New DataTable
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pExcelFilename, strHeader, "")

                'If (dtaExcelData.Rows.Count > 0) Then
                '    intCntr = 0

                '    Dim ProductNoLabel As String
                '    Dim ProductNameLabel As String
                '    Dim PackingSizeUnitLabel As String
                '    Dim PackingSizeQtyLabel As String
                '    Dim CategoryLabel As String
                '    Dim CostFromVendorLabel As String
                '    Dim ShippingCostLabel As String
                '    Dim LandedCostLabel As String

                '    ProductNoLabel = clsPIMUpload.GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", dtaTemplate)
                '    ProductNameLabel = clsPIMUpload.GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", dtaTemplate)
                '    PackingSizeUnitLabel = clsPIMUpload.GetExcelColumn("C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9", dtaTemplate)
                '    PackingSizeQtyLabel = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", dtaTemplate)
                '    CategoryLabel = clsPIMUpload.GetExcelColumn("325A48B7-58F8-4678-B608-58CF43C59BFD", dtaTemplate)
                '    CostFromVendorLabel = clsPIMUpload.GetExcelColumn("e531191e-203e-41cc-ba9b-c46a27d10cd8", dtaTemplate)
                '    ShippingCostLabel = clsPIMUpload.GetExcelColumn("2254751e-e1fc-4704-b494-a39fdba2a245", dtaTemplate)
                '    LandedCostLabel = clsPIMUpload.GetExcelColumn("1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", dtaTemplate)

                '    While (intCntr < dtaExcelData.Rows.Count)
                '        intRow = intCntr + 1

                '        blnIsRowOK = True
                '        strMessage = ""

                '        Dim dtaRow As DataRow
                '        dtaRow = dtaExcelData.Rows(intCntr)

                '        '''Product No
                '        'strColName = clsPIMUpload.GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", dtaTemplate)
                '        If (dtaRow(ProductNoLabel).ToString().Trim() = "") Then
                '            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & ProductNoLabel, ProductNoLabel)
                '            blnIsAllRowOk = False
                '            blnIsRowOK = False
                '        Else
                '            strProductNo = dtaRow(ProductNoLabel).ToString().Trim()
                '        End If

                '        '''Short Product Name
                '        'strColName = clsPIMUpload.GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", dtaTemplate)
                '        If (dtaRow(ProductNameLabel).ToString().Trim() = "") Then
                '            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & ProductNameLabel, ProductNameLabel)
                '            blnIsAllRowOk = False
                '            blnIsRowOK = False
                '        End If

                '        '''Check For Packing Size Unit
                '        'strColName = clsPIMUpload.GetExcelColumn("C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9", dtaTemplate)
                '        If (dtaRow(PackingSizeUnitLabel).ToString().Trim() = "") Then
                '            ' 2016_01_18__13_43_23_2785 added by arvin
                '            dtaRow(PackingSizeUnitLabel) = mDefaultPackingSizeUnit
                '            pWS.Cells(intRow, pExcelHeaders(PackingSizeUnitLabel.Trim().ToLower())).PutValue(dtaRow(PackingSizeUnitLabel))
                '            'strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                '            'blnIsAllRowOk = False
                '            'blnIsRowOK = False
                '        End If

                '        '''Check For Packing Size Qty 
                '        'strColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", dtaTemplate)
                '        If (dtaRow(PackingSizeQtyLabel).ToString().Trim() = "") Then
                '            ' 2016_01_18__13_43_23_2785 added by arvin
                '            dtaRow(PackingSizeQtyLabel) = mDefaultPackingSizeQty
                '            pWS.Cells(intRow, pExcelHeaders(PackingSizeQtyLabel.Trim().ToLower())).PutValue(dtaRow(PackingSizeQtyLabel))
                '            'strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                '            'blnIsAllRowOk = False
                '            'blnIsRowOK = False
                '        Else
                '            strPackingSizeQty = dtaRow(PackingSizeQtyLabel).ToString().Trim()
                '            If (Not IsNumeric(strPackingSizeQty)) Then
                '                strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & PackingSizeQtyLabel, PackingSizeQtyLabel)
                '                blnIsAllRowOk = False
                '                blnIsRowOK = False
                '            End If
                '        End If

                '        '''Category
                '        'strColName = clsPIMUpload.GetExcelColumn("325A48B7-58F8-4678-B608-58CF43C59BFD", dtaTemplate)
                '        If (dtaRow(CategoryLabel).ToString().Trim() = "") Then
                '            ' 2016_01_18__13_43_23_2785 added by arvin
                '            dtaRow(CategoryLabel) = mDefaultCategory
                '            pWS.Cells(intRow, pExcelHeaders(CategoryLabel.Trim().ToLower())).PutValue(dtaRow(CategoryLabel))
                '            'strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                '            'blnIsAllRowOk = False
                '            'blnIsRowOK = False
                '        End If

                '        '''Cost From Vendor
                '        'strColName = clsPIMUpload.GetExcelColumn("e531191e-203e-41cc-ba9b-c46a27d10cd8", dtaTemplate)
                '        If (dtaRow(CostFromVendorLabel).ToString().Trim() = "") Then
                '            ' 2016_01_18__13_43_23_2785 added by arvin
                '            dtaRow(CostFromVendorLabel) = mDefaultCostFromVendor
                '            pWS.Cells(intRow, pExcelHeaders(CostFromVendorLabel.Trim().ToLower())).PutValue(dtaRow(CostFromVendorLabel))
                '            'strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                '            'blnIsAllRowOk = False
                '            'blnIsRowOK = False
                '        Else
                '            strPackingSizeQty = dtaRow(CostFromVendorLabel).ToString().Trim()
                '            If (Not IsNumeric(strPackingSizeQty)) Then
                '                strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & CostFromVendorLabel, CostFromVendorLabel)
                '                blnIsAllRowOk = False
                '                blnIsRowOK = False
                '            End If
                '        End If

                '        '''Shipping Cost
                '        'strColName = clsPIMUpload.GetExcelColumn("2254751e-e1fc-4704-b494-a39fdba2a245", dtaTemplate)
                '        If (dtaRow(ShippingCostLabel).ToString().Trim() = "") Then
                '            ' 2016_01_18__13_43_23_2785 added by arvin
                '            dtaRow(ShippingCostLabel) = mDefaultShippingCost
                '            pWS.Cells(intRow, pExcelHeaders(ShippingCostLabel.Trim().ToLower())).PutValue(dtaRow(ShippingCostLabel))
                '            'strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                '            'blnIsAllRowOk = False
                '            'blnIsRowOK = False
                '        Else
                '            strPackingSizeQty = dtaRow(ShippingCostLabel).ToString().Trim()
                '            If (Not IsNumeric(strPackingSizeQty)) Then
                '                strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & ShippingCostLabel, ShippingCostLabel)
                '                blnIsAllRowOk = False
                '                blnIsRowOK = False
                '            End If
                '        End If

                '        '''Landed Cost
                '        'strColName = clsPIMUpload.GetExcelColumn("1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", dtaTemplate)
                '        If (dtaRow(LandedCostLabel).ToString().Trim() = "") Then
                '            ' 2016_01_18__13_43_23_2785 added by arvin
                '            dtaRow(LandedCostLabel) = mDefaultLandedCost
                '            pWS.Cells(intRow, pExcelHeaders(LandedCostLabel.Trim().ToLower())).PutValue(dtaRow(LandedCostLabel))
                '            'strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                '            'blnIsAllRowOk = False
                '            'blnIsRowOK = False
                '        Else
                '            strPackingSizeQty = dtaRow(LandedCostLabel).ToString().Trim()
                '            If (Not IsNumeric(strPackingSizeQty)) Then
                '                strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & LandedCostLabel, LandedCostLabel)
                '                blnIsAllRowOk = False
                '                blnIsRowOK = False
                '            End If
                '        End If

                '        ' '' '''Check for the price if has multiple volume discount schemes
                '        ' ''If (IsNumeric(strPackingSizeQty)) Then

                '        ' ''    Dim strPriceLevels As String = ""
                '        ' ''    Dim dtExcelBase As DataTable
                '        ' ''    Dim dtBasePriceLevel As DataTable
                '        ' ''    Dim dblRetailPrice As Double = 0
                '        ' ''    Dim dblRetailPriceDiscount As Double = 0
                '        ' ''    Dim dblWholesalePrice As Double = 0
                '        ' ''    Dim dblWholesalePriceDiscount As Double = 0
                '        ' ''    Dim dblCEBUPrice As Double = 0
                '        ' ''    Dim dblCEBUPriceDiscount As Double = 0
                '        ' ''    Dim objPriceLevelRow(1) As Object

                '        ' ''    dtaExcelPriceLevel.Columns.Clear()
                '        ' ''    dtaExcelPriceLevel.Columns.Add("PriceLevelID")
                '        ' ''    dtaExcelPriceLevel.Columns.Add("PriceLevel")

                '        ' ''    '''Get Retail Price Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C90
                '        ' ''    strColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C90", dtaTemplate)
                '        ' ''    If (strColName.Trim().Length > 0) Then
                '        ' ''        strPriceLevels = IIf(strPriceLevels.Trim().Length > 0, strPriceLevels & ",'01000012-0005-0000-0000-000000000000'", "'01000012-0005-0000-0000-000000000000'")
                '        ' ''        dblRetailPrice = cdec2(dtaRow(strColName))

                '        ' ''        objPriceLevelRow(0) = "01000012-0005-0000-0000-000000000000"
                '        ' ''        objPriceLevelRow(1) = strColName
                '        ' ''        dtaExcelPriceLevel.Rows.Add(objPriceLevelRow)
                '        ' ''    End If
                '        ' ''    '''Get Wholesale Price Column fixed Feature ID 997373c6-b8f1-4912-b03c-7b3bffd74563
                '        ' ''    strColName = clsPIMUpload.GetExcelColumn("997373c6-b8f1-4912-b03c-7b3bffd74563", dtaTemplate)
                '        ' ''    If (strColName.Trim().Length > 0) Then
                '        ' ''        strPriceLevels = IIf(strPriceLevels.Trim().Length > 0, strPriceLevels & ",'01000012-0004-0000-0000-000000000000'", "'01000012-0004-0000-0000-000000000000'")
                '        ' ''        dblWholesalePrice = cdec2(dtaRow(strColName))

                '        ' ''        objPriceLevelRow(0) = "01000012-0004-0000-0000-000000000000"
                '        ' ''        objPriceLevelRow(1) = strColName
                '        ' ''        dtaExcelPriceLevel.Rows.Add(objPriceLevelRow)
                '        ' ''    End If
                '        ' ''    '''Get CEBU Price Column fixed Feature ID 7655456c-2cf3-4e23-8f64-b63acf21525b
                '        ' ''    strColName = clsPIMUpload.GetExcelColumn("7655456c-2cf3-4e23-8f64-b63acf21525b", dtaTemplate)
                '        ' ''    If (strColName.Trim().Length > 0) Then
                '        ' ''        strPriceLevels = IIf(strPriceLevels.Trim().Length > 0, strPriceLevels & ",'C5AF3C2C-8CC2-4BFA-A592-620D83368DF1'", "'C5AF3C2C-8CC2-4BFA-A592-620D83368DF1'")
                '        ' ''        dblCEBUPrice = cdec2(dtaRow(strColName))

                '        ' ''        objPriceLevelRow(0) = "C5AF3C2C-8CC2-4BFA-A592-620D83368DF1"
                '        ' ''        objPriceLevelRow(1) = strColName
                '        ' ''        dtaExcelPriceLevel.Rows.Add(objPriceLevelRow)
                '        ' ''    End If

                '        ' ''    If (strPriceLevels.Trim().Length > 0) Then

                '        ' ''        '''get Base Price of every Price Levels
                '        ' ''        pFinder = New Finder
                '        ' ''        With pFinder
                '        ' ''            .ProData = mBusiness.ProData
                '        ' ''            .MaxRecords = MaxReturnRow

                '        ' ''            With .Fields
                '        ' ''                With .Add("ID", SqlDbType.UniqueIdentifier)
                '        ' ''                    .Expression = "orgplevel.ID"
                '        ' ''                    .Searchable = False
                '        ' ''                End With
                '        ' ''                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                '        ' ''                    .Expression = "orgplevel.PriceLevelID"
                '        ' ''                    .Searchable = False
                '        ' ''                End With
                '        ' ''                With .Add("Price", SqlDbType.Money)
                '        ' ''                    .Expression = "orgplevel.Price"
                '        ' ''                End With
                '        ' ''            End With

                '        ' ''            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel orgplevel "

                '        ' ''            .SqlWhere = "WHERE orgplevel.OrganizationID " & IIf(OrganizationID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(OrganizationID)) &
                '        ' ''                " AND orgplevel.PriceLevelID IN (" & strPriceLevels & ") " &
                '        ' ''                " AND orgplevel.ProductID IN (SELECT ID FROM MobizLibraryPIMProduct WHERE ProductNo = " & ReadyString(strProductNo) & " OR SUBSTRING(ProductNo, PATINDEX('%[^0 ]%', ProductNo + ' '), LEN(ProductNo)) = " & ReadyString(strProductNo.TrimStart("0"c)) & ") "

                '        ' ''            dtBasePriceLevel = .Requery
                '        ' ''        End With

                '        ' ''        If (dtBasePriceLevel.Rows.Count > 0) Then
                '        ' ''            '''Get Packing Size Qty Column fixed Feature ID E1059BD9-B365-478F-A443-F465B604D251
                '        ' ''            Dim intMeasurementQty As Integer
                '        ' ''            Dim intPriceLevelCntr As Integer
                '        ' ''            intMeasurementQty = 1
                '        ' ''            strColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", dtaTemplate)
                '        ' ''            If (strColName.Trim().Length > 0) Then
                '        ' ''                intMeasurementQty = cdec2(dtaRow(strColName))
                '        ' ''            End If

                '        ' ''            If (intMeasurementQty > 1) Then ''' Check only if not base measure
                '        ' ''                intPriceLevelCntr = 0
                '        ' ''                dblRetailPriceDiscount = 0
                '        ' ''                dblWholesalePriceDiscount = 0
                '        ' ''                dblCEBUPriceDiscount = 0
                '        ' ''                While (intPriceLevelCntr < dtBasePriceLevel.Rows.Count)

                '        ' ''                    '''Check for Retail Price If has discount
                '        ' ''                    If (dtBasePriceLevel.Rows(intPriceLevelCntr).Item("PriceLevelID").ToString().Trim() = "01000012-0005-0000-0000-000000000000") Then
                '        ' ''                        If ((dblRetailPrice / intMeasurementQty) <> cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price"))) Then
                '        ' ''                            dblRetailPriceDiscount = (((cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty) - dblRetailPrice) / (cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty)) * 100
                '        ' ''                        End If
                '        ' ''                    End If

                '        ' ''                    '''Check for Wholesale Price If has discount
                '        ' ''                    If (dtBasePriceLevel.Rows(intPriceLevelCntr).Item("PriceLevelID").ToString().Trim() = "01000012-0004-0000-0000-000000000000") Then
                '        ' ''                        If ((dblWholesalePrice / intMeasurementQty) <> cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price"))) Then
                '        ' ''                            dblWholesalePriceDiscount = (((cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty) - dblWholesalePrice) / (cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty)) * 100
                '        ' ''                        End If
                '        ' ''                    End If

                '        ' ''                    '''Check for CEBU Price If has discount
                '        ' ''                    If (dtBasePriceLevel.Rows(intPriceLevelCntr).Item("PriceLevelID").ToString().Trim() = "C5AF3C2C-8CC2-4BFA-A592-620D83368DF1") Then
                '        ' ''                        If ((dblCEBUPrice / intMeasurementQty) <> cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price"))) Then
                '        ' ''                            dblCEBUPriceDiscount = (((cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty) - dblCEBUPrice) / (cdec2(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty)) * 100
                '        ' ''                        End If
                '        ' ''                    End If

                '        ' ''                    intPriceLevelCntr = intPriceLevelCntr + 1
                '        ' ''                End While

                '        ' ''                If (dblRetailPriceDiscount <> dblWholesalePriceDiscount Or
                '        ' ''                    dblRetailPriceDiscount <> dblCEBUPriceDiscount) Then

                '        ' ''                    blnIsAllPricesOk = False

                '        ' ''                End If
                '        ' ''            End If

                '        ' ''        End If

                '        ' ''    End If

                '        ' ''End If
                '        ' '' '''END Check for the price if has multiple volume discount schemes --------------------

                '        If (blnIsRowOK) Then '''clear the row of the red highlights
                '            pWS.Cells(intRow, pMaxDataColumn + 1).PutValue("")
                '            pWS.Cells.Rows(intRow).ApplyStyle(style, styleFlag)
                '        Else
                '            pWS.Cells(intRow, pMaxDataColumn + 1).PutValue("Please enter a value for " & strMessage & ".")
                '            pWS.Cells.Rows(intRow).ApplyStyle(styleError, styleFlagError)
                '        End If

                '        intCntr = intCntr + 1
                '    End While

                '    ' ''If (Not blnIsAllRowOk) Then
                '    ' ''    strFileNameError = OrganizationID & "_PIMTemplate-Errors.xls"
                '    ' ''    pWB.Save(Common.ServerPath & "temp\PIMTemplate\" & strFileNameError)

                '    ' ''    If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")) Then
                '    ' ''        IO.File.Delete(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")
                '    ' ''    End If

                '    ' ''End If

                '    If (Not blnIsAllPricesOk) Then
                '        strPriceDiscountDiff = "Price Discount Difference"
                '    End If

                'End If

            End If

            pOut.Add("PIMExcelUpload", strFileNameError)
            ' ''pOut.Add("PriceDiscountDiff", strPriceDiscountDiff)
            ' ''pOut.Add("PriceDiscountList", dtaExcelPriceLevel)

            Try
                pWB.Save(pExcelFilename)
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
                mCurrentUploader = Nothing
            End Try

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function IsPIMExcelUploaded() As String
            Dim strReturn As String

            'Dim pUploaderKey = Common.WebRequest.Data("UploaderKey")
            'If String.IsNullOrWhiteSpace(pUploaderKey) = False _
            '    AndAlso mCurrentUploader IsNot Nothing _
            '    AndAlso mCurrentUploader.UploaderKey = pUploaderKey Then

            '    mCurrentUploader = Nothing
            'End If

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = WebSession.Instance.SerializedString

            WebSession.Instance.PIMTemplateUploaded = ""
            WebSession.Instance.SerializedString = ""


            isSameProduct = CBool2(Common.WebRequest.Data("IsSameProduct"))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckUniqueTemplateName() As String
            Dim clsPIMUpload As New PIMUpload
            Dim strReturn As String

            clsPIMUpload.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsPIMUpload.TemplateName = Common.WebRequest.Data("PIMTemplateName")

            strReturn = JsonDictionary.Serialize("PIMExcelUpload", clsPIMUpload.CheckUniqueTemplateName())

            Return strReturn

        End Function

        <HttpPost()>
        Function UploadPIMExcelTemplate() As String
            Dim strReturn As String
            Dim strResult As String
            Dim dtaTableRequiredFields As DataTable
            Dim dtaExcelHeaders As New DataTable
            Dim clsPIMUpload As New PIMUpload
            Dim strTemplateID As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMUpload.OrganizationID = Common.WebRequest.Data("OrganizationID")
            'clsPIMUpload.TemplateName = Common.WebRequest.Data("PIMTemplateName")
            clsPIMUpload.TemplateName = "New Template" & GenerateString(6, "0123456789")
            clsPIMUpload.PIMTemplateColList = Common.WebRequest.AllData("PIMTemplateColumnList")
            clsPIMUpload.PIMTemplateColAKAList = Common.WebRequest.AllData("PIMTemplateColumnAKAList")

            dtaTableRequiredFields = clsPIMUpload.CheckUnMatchedFields()

            If (dtaTableRequiredFields.Rows.Count > 0) Then
                strReturn = JsonDictionary.Serialize("PIMExcelUploadNM", dtaTableRequiredFields)
            Else
                '''Save The Excel File Template
                strTemplateID = clsPIMUpload.SavePIMTemplate()
                strReturn = JsonDictionary.Serialize("PIMExcelUpload", "Success")
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveTemplateAKA() As String
            Dim strReturn As String
            Dim blnIsSuccess As Boolean = False
            Dim clsPIMUpload As New PIMUpload

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMUpload.PIMTemplateColAKAList = Common.WebRequest.AllData("PIMTemplateColumnAKAList")

            blnIsSuccess = clsPIMUpload.SavePIMTemplateAKA()
            strReturn = JsonDictionary.Serialize("PIMExcelUpload", IIf(blnIsSuccess, "Success", "Failed"))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetDistinctColumnData() As String
            Dim strReturn As String
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim dtaExcelData As New DataTable
            Dim strOrgID As String
            Dim strFeatureID As String
            Dim intCntr As Integer
            Dim strHeader(0) As String
            Dim pFS As IO.FileStream
            Dim pDTAKA As DataTable
            Dim clsPIMMeasure As PIMMeasure

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            '''Set the Headers
            strHeader(0) = Common.WebRequest.Data("ExcelColumn")

            strOrgID = Common.WebRequest.Data("OrgID")

            dtaExcelData = New DataTable
            If (Common.WebRequest.Data("FeatureID").Trim().ToUpper() = "C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9") Then

                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pSourceURL & "\PIMTemplate\" & strOrgID & "_PIMTemplate.xls", strHeader, "")

                dtaExcelData = dtaExcelData.DefaultView.ToTable(True, strHeader)

                If (Not dtaExcelData.Columns.Contains("FeatureID")) Then
                    dtaExcelData.Columns.Add("FeatureID")
                End If

                If (Not dtaExcelData.Columns.Contains(Common.WebRequest.Data("Feature") & "ID")) Then
                    dtaExcelData.Columns.Add(Common.WebRequest.Data("Feature") & "ID")
                End If

                If (Not dtaExcelData.Columns.Contains(Common.WebRequest.Data("Feature"))) Then
                    dtaExcelData.Columns.Add(Common.WebRequest.Data("Feature"))
                End If

                intCntr = 0
                While (intCntr < dtaExcelData.Rows.Count)
                    clsPIMMeasure = New PIMMeasure()

                    dtaExcelData.Rows(intCntr).Item("FeatureID") = Common.WebRequest.Data("FeatureID")

                    pDTAKA = clsPIMMeasure.getMeasureAKA(dtaExcelData.Rows(intCntr).Item(0).ToString().Trim(), False)

                    If (pDTAKA.Rows.Count > 0) Then
                        dtaExcelData.Rows(intCntr).Item(Common.WebRequest.Data("Feature") & "ID") = pDTAKA.Rows(0).Item("ID")
                        dtaExcelData.Rows(intCntr).Item(Common.WebRequest.Data("Feature")) = pDTAKA.Rows(0).Item("Measurement")
                    Else
                        pDTAKA = clsPIMMeasure.getMeasureAKA(dtaExcelData.Rows(intCntr).Item(0).ToString().Trim(), True)
                        If (pDTAKA.Rows.Count > 0) Then
                            dtaExcelData.Rows(intCntr).Item(Common.WebRequest.Data("Feature") & "ID") = pDTAKA.Rows(0).Item("ID")
                            dtaExcelData.Rows(intCntr).Item(Common.WebRequest.Data("Feature")) = pDTAKA.Rows(0).Item("Measurement")
                        End If
                    End If

                    intCntr = intCntr + 1
                End While


                '''Filter only those records doesn't have a matching mobiz col
                Dim dtaView = New DataView
                dtaView = dtaExcelData.DefaultView
                dtaView.RowFilter = "[" & Common.WebRequest.Data("Feature") & "ID]" & " IS NULL"
                dtaExcelData = dtaView.ToTable()

            Else
                dtaExcelData.Columns.Add(Common.WebRequest.Data("ExcelColumn"))
                dtaExcelData.Columns.Add(Common.WebRequest.Data("MobizDataID"))
                dtaExcelData.Columns.Add(Common.WebRequest.Data("MobizData"))
            End If

            strReturn = JsonDictionary.Serialize("PIMExcelUpload", dtaExcelData)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
                mCurrentUploader = Nothing
            End Try

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveExcelMatchingData(ByVal vTemplateID As String, ByVal vOrgID As String) As Boolean
            Dim strReturn As String
            Dim clsPIMUpload As New PIMUpload
            Dim objPIMColDataList As Object()
            Dim intCntr As Integer

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            objPIMColDataList = Common.WebRequest.AllData("PIMColDataList")
            For Each objPIMColData As Object In objPIMColDataList

                '''Unit Meaures
                If (objPIMColData("PIMFeatureID").ToString().Trim().ToUpper() = "C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9") Then
                    clsPIMUpload.SavePIMMeasureAKA(objPIMColData("PIMFeatureDataID").ToString().Trim(), objPIMColData("ColumnNameData").ToString().Trim())
                End If

            Next objPIMColData

            Return strReturn

        End Function

        <HttpPost()>
        Function SendEmailUnMatchedCol() As String
            Dim clsOrganization As New Organization
            Dim clsPeople As New People
            Dim clsPIMUpload As New PIMUpload
            Dim strReturn As String
            Dim intCntr As Integer
            Dim strTemp As String
            Dim strClientEmail() As String
            Dim strRecipientEmail As String
            Dim strBody As String = ""
            Dim strmReader As System.IO.StreamReader
            Dim pDataUnMatchedCol As DataTable
            Dim dtaTable As DataTable
            Dim strDateTime As String = Now.ToString("MM/dd/yyyy hh:mm:ss tt")

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            '''Check Email Address
            If (Common.WebRequest.Data("EmailAddress").Trim().Length > 0) Then
                strClientEmail = Common.WebRequest.Data("EmailAddress").Trim().Split(",")
                intCntr = 0
                While (intCntr < strClientEmail.Length)

                    If (Not IsEmail(strClientEmail(intCntr).Trim())) Then
                        Return JsonDictionary.Serialize("PIMExcelUpload", "InvalidEmail")
                    End If

                    intCntr = intCntr + 1
                End While
            End If

            '''Fixed muna
            strRecipientEmail = "robert.chong@mobiz-apps.com, janine.son@mobiz-apps.com, francis.adan@mobiz-apps.com, francis.hernandez@mobiz-apps.com, arvin.meralpis@mobiz-apps.com"

            strmReader = New System.IO.StreamReader(Server.MapPath("~/Views/PIMUpload/EmailMobizTech.vbhtml"))
            strBody = strmReader.ReadToEnd

            '''Organization 
            dtaTable = New DataTable()
            dtaTable = clsOrganization.getOrganizationInfo(Common.WebRequest.Data("OrgID"))
            If (dtaTable.Rows.Count > 0) Then
                If (Convert.ToBoolean(dtaTable.Rows(0).Item("IsOrganization"))) Then
                    strBody = strBody.Replace("[OrganizationHere]", "Organization : " & dtaTable.Rows(0).Item("CompanyName").ToString().Trim())
                Else
                    strBody = strBody.Replace("[OrganizationHere]", "Organization : " & dtaTable.Rows(0).Item("PersonName").ToString().Trim())
                End If
            End If

            '''Sender Details
            dtaTable = New DataTable()
            dtaTable = clsPeople.getPeopleInfoName(WebSession.Instance.PeopleUserID)
            If (dtaTable.Rows.Count > 0) Then
                strBody = strBody.Replace("[SenderDetailsHere]", "Sender Name : " & dtaTable.Rows(0).Item("Surname").ToString().Trim() & ", " & dtaTable.Rows(0).Item("FirstName").ToString().Trim() & " " & dtaTable.Rows(0).Item("MiddleName").ToString().Trim())
            End If
            strBody = strBody.Replace("[EmailAddress]", "Email Address : " & Common.WebRequest.Data("EmailAddress").Trim())
            strBody = strBody.Replace("[DateTimeSent]", "Date/Time Sent : " & strDateTime)

            '''Set UnMatched Columns
            clsPIMUpload.PIMTemplateColList = Common.WebRequest.AllData("PIMTemplateColumnList")
            pDataUnMatchedCol = clsPIMUpload.CheckUnMatchedFields()
            If (pDataUnMatchedCol.Rows.Count > 0) Then

                strTemp = "<tbody>"
                intCntr = 0
                While (intCntr < pDataUnMatchedCol.Rows.Count)

                    strTemp = strTemp & "<tr>"
                    strTemp = strTemp & "<td style=""text-align:left; width: 75%; height:30px; border: 1px dotted Gray;"">" & pDataUnMatchedCol.Rows(intCntr).Item("ColumnName").ToString().Trim() & "</td>"
                    strTemp = strTemp & "<td style=""text-align:left; width: 25%; height:30px; border: 1px dotted Gray;"">" & pDataUnMatchedCol.Rows(intCntr).Item("FeatureType").ToString().Trim() & "</td>"
                    strTemp = strTemp & "</tr>"

                    intCntr = intCntr + 1

                End While

                strTemp = strTemp & "</tbody>"

                strBody = strBody.Replace("[BodyGoesHere]", strTemp)

            End If

            SendMail("customercare@mobiz-apps.com",
                     strRecipientEmail,
                     "Mobiz",
                     "Mobiz - PIM SKU Upload No Matching Column/Features " & strDateTime,
                     strBody,
                     "smtp.live.com",
                     587,
                     "customercare@mobiz-apps.com",
                     "MobizCustCare2221", , , , , True)

            strReturn = JsonDictionary.Serialize("PIMExcelUpload", "Sent")

            Return strReturn

        End Function

        Function GetPackingSizeUnit() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPIMUpload As New PIMUpload

            Return JsonDictionary.Serialize("Results", clsPIMUpload.GetMeasurement("PackingSizeUnit"))
        End Function

        Function GetContentUnit() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPIMUpload As New PIMUpload

            Return JsonDictionary.Serialize("Results", clsPIMUpload.GetMeasurement("ContentUnit"))
        End Function

        Function GetDimensionUnit() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPIMUpload As New PIMUpload

            Return JsonDictionary.Serialize("Results", clsPIMUpload.GetMeasurement("DimensionUnit"))
        End Function

        Function GetWeightUnit() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPIMUpload As New PIMUpload

            Return JsonDictionary.Serialize("Results", clsPIMUpload.GetMeasurement("WeightUnit"))
        End Function

        Function CheckIfHeadersExist() As String
            Dim strSerializedCols As String = ""
            Dim strTemplateID As String = ""
            Dim dtaExcelHeaders As New DataTable
            Dim dtaExcelHeadersMiss As New DataTable
            Dim dtaExcelTemplateMatching As New DataTable
            Dim fixHeader As Object = Common.WebRequest.AllData("s")
            Dim OrgID As String = Common.WebRequest.Data("OrgID")
            Dim clsPIMUpload As New PIMUpload
            Dim pOut As JsonDictionary
            pOut = New JsonDictionary

            dtaExcelHeaders.Columns.Add("ExcelColumn")

            For Each objHeader As String In fixHeader
                dtaExcelHeaders.Rows.Add(objHeader)
            Next
            dtaExcelHeadersMiss = clsPIMUpload.CheckExcelColumns(dtaExcelHeaders, OrgID)
            If (dtaExcelHeadersMiss.Rows.Count <= 0) Then
                dtaExcelTemplateMatching = New DataTable
                dtaExcelTemplateMatching = clsPIMUpload.GetTemplateMatching(dtaExcelHeaders, OrgID)
                If dtaExcelTemplateMatching IsNot Nothing Then
                    strSerializedCols = JsonDictionary.Serialize("PIMExcelUpload", dtaExcelTemplateMatching)
                    WebSession.Instance.SerializedString = strSerializedCols
                    WebSession.Instance.PIMTemplateID = strTemplateID
                    WebSession.Instance.PIMTemplateUploaded = "TemplateExist"
                End If
            Else
                WebSession.Instance.PIMTemplateUploaded = "NewTemplate"
                strSerializedCols = JsonDictionary.Serialize("PIMExcelUpload", dtaExcelHeadersMiss)
                WebSession.Instance.SerializedString = strSerializedCols
            End If

            pOut.Add("OrgID", OrgID)
            pOut.Add("Result", WebSession.Instance.PIMTemplateUploaded)
            Return pOut.Serialize
        End Function

        Function SavePIMWebForm() As String
            Dim webformdata As Object = Common.WebRequest.AllData("WebFormData")
            Dim ProductNo As String = webformdata("ProductNo")
            Dim ShortName As String = webformdata("ShortName")
            Dim Brand As String = webformdata("Brand")
            Dim Category As String = webformdata("Category")
            Dim UPCBarcode As String = webformdata("UPCBarcode")
            Dim CostFromVendor As String = webformdata("CostFromVendor")
            Dim ShippingCost As String = webformdata("ShippingCost")
            Dim LandedCost As String = webformdata("LandedCost")
            Dim SRP As String = webformdata("SRP")
            Dim RetailPrice As String = webformdata("RetailPrice")
            Dim PackingSizeQty As String = webformdata("PackingSizeQty")
            Dim PackingSizeUnit As String = webformdata("PackingSizeUnit")
            Dim ContentQty As String = webformdata("ContentQty")
            Dim ContentUnit As String = webformdata("ContentUnit")
            Dim DimensionLength As String = webformdata("DimensionLength")
            Dim DimensionLengthUnit As String = webformdata("DimensionLengthUnit")
            Dim DimensionWidth As String = webformdata("DimensionWidth")
            Dim DimensionWidthUnit As String = webformdata("DimensionWidthUnit")
            Dim DimensionHeight As String = webformdata("DimensionHeight")
            Dim DimensionHeightUnit As String = webformdata("DimensionHeightUnit")
            Dim ProductWeight As String = webformdata("ProductWeight")
            Dim ProductWeightUnit As String = webformdata("ProductWeightUnit")
            Dim OrgID As String = Common.WebRequest.Data("OrgID")
            Dim fixHeader As Object = Common.WebRequest.AllData("pDataHeader")
            Dim dtaExcelHeaders As New DataTable
            Dim dtaTemplate As New DataTable
            Dim dtaExcelData As New DataTable
            Dim productNameCheckMatch As String = IsProductNoUnique(ProductNo)
            Dim errors As String = ""
            Dim blnResult As Boolean = False
            Dim datarow As DataRow
            Dim clsPIMUpload As New PIMUpload
            Dim pOut As JsonDictionary
            pOut = New JsonDictionary

            IsAsset = CBool2(Common.WebRequest.Data("IsAsset"))
            WebFormIsInventoryCountItem = CBool2(Common.WebRequest.Data("IsInventoryCountItem"))

            If (productNameCheckMatch <> "True") Then
                If ShortName <> productNameCheckMatch Then
                    errors = "Product No. " & ProductNo & " already exists with a different product name: " & productNameCheckMatch & vbCrLf
                End If
            End If

            If errors = "" Then
                dtaExcelHeaders.Columns.Add("ExcelColumn")

                For Each objHeader As String In fixHeader
                    dtaExcelHeaders.Rows.Add(objHeader)
                Next

                dtaExcelData.Columns.Add("Spreadsheet")
                For Each objHeader As String In fixHeader
                    dtaExcelData.Columns.Add(objHeader)
                Next
                dtaExcelData.Columns.Add("Import Result")

                datarow = dtaExcelData.NewRow
                datarow(0) = "2"
                datarow(1) = ProductNo
                datarow(2) = ShortName
                datarow(3) = Brand
                datarow(4) = Category
                datarow(5) = UPCBarcode
                datarow(6) = CostFromVendor
                datarow(7) = LandedCost
                datarow(8) = ShippingCost
                datarow(9) = SRP
                datarow(10) = RetailPrice
                datarow(11) = PackingSizeQty
                datarow(12) = PackingSizeUnit
                datarow(13) = ContentQty
                datarow(14) = ContentUnit
                datarow(15) = DimensionLength
                datarow(16) = DimensionLengthUnit
                datarow(17) = DimensionWidth
                datarow(18) = DimensionWidthUnit
                datarow(19) = DimensionHeight
                datarow(20) = DimensionHeightUnit
                datarow(21) = ProductWeight
                datarow(22) = ProductWeightUnit
                datarow(23) = ""
                dtaExcelData.Rows.Add(datarow)

                dtaTemplate = clsPIMUpload.GetTemplateColumns(dtaExcelHeaders, OrgID)
                mDatabase = mProData.GetCurrentDatabase()

                blnResult = SavePIMTemplateData(dtaTemplate, dtaExcelData, OrgID, mDatabase, WebSession.Instance.PeopleUserID, False, False, False, False, False, False, True)

                Dim pimlib As New OrganizationController

                pimlib.SavePimBrandsToBrandsLibrary()
                pimlib.SavePimCategoriesToCategoriesLibrary()

                Return JsonDictionary.Serialize("PIMExcelUpload", IIf(blnResult, "Success", "Failed"))
            Else
                Return JsonDictionary.Serialize("PIMExcelUploadError", errors)
            End If

        End Function

        <HttpPost>
        Public Function DeleteColumnMappings() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pAffectedCount = WebRequest.Instance.Business.LibraryStock.DeleteColumnMappings(WebRequest.Instance.OrgID)

            Return JsonDictionary.Serialize("AffectedCount", pAffectedCount)

        End Function

        Public Function SetBeginningBalance(BegBalance As Double, ProductID As String, FOWSTID As String, BinID As String) As String
            Dim pSql As String
            Dim pDTFirstCurrentRecord As DataTable
            Dim pDTInventoryCountRecord As DataTable
            Dim pDTRecordList As DataTable
            Dim PrevBalance As Double = 0.0

            Try

                'recompute previous trasaction records

                Dim pDTPreviousTransactions As DataTable
                Dim PreviousBalance As Double
                Dim pTransDate As String

                pTransDate = mCurrentUploader.BeginningBalanceDateAndTime

                pDTPreviousTransactions = GetPreviousInventoryLedgerTransactionCount(pTransDate, FOWSTID, BinID, ProductID)

                PreviousBalance = BegBalance

                If pDTPreviousTransactions IsNot Nothing Then
                    For i As Integer = 0 To pDTPreviousTransactions.Rows.Count - 1
                        If pDTPreviousTransactions.Rows(i).Item("TransactionType") = "InventoryCount" Then

                            Dim InventoryCountMovement As Double

                            InventoryCountMovement = CDec2(pDTPreviousTransactions.Rows(i).Item("Balance")) - PreviousBalance
                            PreviousBalance = pDTPreviousTransactions.Rows(i).Item("Balance")

                            pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & ", Qty = " & InventoryCountMovement & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                            mProData.ExecuteSQL(pSql)
                        Else
                            PreviousBalance += CDbl(pDTPreviousTransactions.Rows(i).Item("Qty"))

                            pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                            mProData.ExecuteSQL(pSql)
                        End If
                    Next
                End If

                'get first record
                pSql = "Select top 1 * from TransInventoryLedger where " & vbCrLf &
                        "FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND BinID = " & ReadyString(BinID) & vbCrLf &
                        " AND ProductID = " & ReadyString(ProductID) & vbCrLf &
                        " AND LastUpdated >= " & mCurrentUploader.BeginningBalanceDateAndTime & vbCrLf &
                        " order by LastUpdated"

                pDTFirstCurrentRecord = mProData.Execute(pSql)

                'Beggining Balance
                pSql = "Insert into TransInventoryLedger(ProductID, FowstID, BinID, TransactionID, TransactionType, " &
                        "Qty, Balance, UserID, LastUpdated, TransactionDate) values(" &
                        ReadyString(ProductID) & "," &
                        ReadyString(FOWSTID) & "," &
                        ReadyString(BinID) & "," &
                        ReadyString(Guid.NewGuid.ToString) & "," &
                        ReadyString("Beginning Balance") & "," &
                        CDec2(BegBalance - PrevBalance) & "," &
                        BegBalance & "," &
                        ReadyString(mCurrentUploader.UploaderUserID) & "," &
                        mCurrentUploader.BeginningBalanceDateAndTime & "," &
                        mCurrentUploader.BeginningBalanceDateAndTime &
                        ")"

                mProData.Execute(pSql)

                ''' LibraryFowstBinProduct Table
                Dim dtFowstBinProduct As DataTable
                Dim pIU As InsertUpdate
                Dim IsNewFowstBinProduct As Boolean 'important for Fowst Product computation
                Dim pOut As String

                dtFowstBinProduct = GetDataFromLibraryFowstBinProduct(ProductID, FOWSTID, BinID)

                pIU = New InsertUpdate("LibraryFowstBinProduct")

                pIU("Balance") = BegBalance
                pIU("LastUpdated") = "GETDATE()"

                If dtFowstBinProduct.Rows.Count > 0 Then

                    pSql = pIU.GetUpdate(" ID = " & ReadyString(CStr2(dtFowstBinProduct.Rows(0).Item("ID"))))

                Else

                    IsNewFowstBinProduct = True

                    pOut = Guid.NewGuid().ToString()

                    pIU("ID") = ReadyString(pOut)
                    pIU("FowstID") = ReadyString(FOWSTID)
                    pIU("BinID") = ReadyString(BinID)
                    pIU("ProductID") = ReadyString(ProductID)

                    pSql = pIU.GetInsert()

                End If

                mProData.ExecuteSQL(pSql)

                Return JsonDictionary.Serialize("Results", "Success")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "Error")
            End Try
        End Function

        Function GetDefaultBIN(ByVal vProductID As String, ByVal vFOWSTID As String) As String
            Dim pSql As String

            pSql = "Select BinID from LibraryFowstBinProduct where IsDefault = 1 AND ProductID = " & ReadyString(vProductID) & " AND FOWSTID = " & ReadyString(vFOWSTID)

            Return CStr2(mProData.ExecuteScalar(pSql))

        End Function

        Private Function GetDataFromLibraryFowstBinProduct(ByVal vStockID As String, ByVal vFowstID As String, ByVal vBinID As String) As DataTable

            Dim pSql As String

            pSql = "SELECT TOP 1 *" & vbCrLf &
                " FROM LibraryFowstBinProduct" & vbCrLf &
                " WHERE ProductID = " & ReadyString(vStockID) & vbCrLf &
                " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
                " AND BinID = " & ReadyString(vBinID) & vbCrLf

            Return mProData.Execute(pSql)
        End Function

        Private Function GetPreviousInventoryLedgerTransactionCount(vTransDate, vFowstID, vBinID, vProductID) As DataTable
            Dim pSql As String

            pSql = "Select * from TransInventoryLedger " & vbCrLf &
                    "WHERE TransactionDate > " & vTransDate & vbCrLf &
                    " AND BinID = " & ReadyString(vBinID) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                    " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
                    " ORDER BY TransactionDate"
            Return mProData.Execute(pSql)
        End Function

        Function GetFOWSTID() As String
            Dim pSql As String

            pSql = "Select ID from MobizLibraryOrganizationFOWST where Name = " & ReadyString(mCurrentUploader.FOWSTName)

            Return CStr2(mProData.ExecuteScalar(pSql))
        End Function
    End Class

End Namespace