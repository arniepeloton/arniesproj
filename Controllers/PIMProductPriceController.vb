Imports Aspose.Cells

Namespace WebProj
    Public Class PIMProductPriceController
        Inherits System.Web.Mvc.Controller


        Dim mProData As ProData = Common.WebRequest.ProData
        Public Property MaxReturnRow As Integer = 50

        Shared mDatabase As String
        Shared vPeopleUserID As String
        Shared strExcelFileProgress As String

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Product Price")) Then Exit Function
            End If

            Return View()

        End Function


        <HttpPost()>
        Function Index(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String) As String
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


            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            WebSession.Instance.PIMTemplateUploaded = ""

            If (Not objFile Is Nothing) Then
                objFile.SaveAs(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMProductPriceList.xls")

                Try
                    pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMProductPriceList.xls", IO.FileMode.Open)
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


                    If (CheckExcelRequiredColumns(dtaExcelHeaders)) Then
                        If pFS IsNot Nothing Then pFS.Dispose()

                        WebSession.Instance.PIMTemplateUploaded = "Ok"
                    Else
                        WebSession.Instance.PIMTemplateUploaded = "Incomplete"
                    End If

                Catch ex As Exception
                    WebSession.Instance.PIMTemplateUploaded = "Failed"
                    WebSession.Instance.SerializedString = ""
                Finally
                    Try
                        If pFS IsNot Nothing Then pFS.Dispose()
                    Catch ex As Exception
                    End Try
                End Try


            Else
                WebSession.Instance.PIMTemplateUploaded = "Empty"

            End If

            pOut.Add("OrgID", OrgID)
            pOut.Add("Result", WebSession.Instance.PIMTemplateUploaded)
            Return pOut.Serialize

            'Return RedirectToAction("UploadDownloadExcelFile", New With {.OrgID = OrgID, .Result = WebSession.Instance.PIMTemplateUploaded})

        End Function


        Function UploadDownloadExcelFile() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function

            End If

            Return View()

        End Function


        <HttpPost()>
        Function getProductPriceByStyleNumberColorCodeList() As String
            Dim strID As String
            Dim strPriceLevelID As String
            Dim clsPIMProductPrice As New PIMProductPrice
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMProductID")
            strPriceLevelID = Common.WebRequest.Data("PIMPriceLevelID")
            strReturn = JsonDictionary.Serialize("PIMProductPriceData", clsPIMProductPrice.getProductPriceByStyleNumberColorCodeList(strID, strPriceLevelID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveProductPriceInfo() As String
            Dim clsPIMProductPrice As New PIMProductPrice
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProductPrice.ProductPriceList = Common.WebRequest.AllData("ProductPrice")

            strReturn = JsonDictionary.Serialize("PIMProductPriceData", IIf(clsPIMProductPrice.SaveProductPriceInfo(), "Success", "Failed"))

            Return strReturn

        End Function


        'generate product list to template
        <HttpPost()>
        Function GetProductList() As String
            Dim pDT As DataTable
            Dim clsPimProductPrice As New PIMProductPrice
            Dim strFileName As String
            Dim pSourceURL As String = Common.ServerPath & "temp\PIMProductList"

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPimProductPrice.FilterName = Common.WebRequest.Data("FilterName")
            clsPimProductPrice.PriceLevelName = Common.WebRequest.Data("PriceLevelName")

            If clsPimProductPrice.FilterName.Trim() <> "" Then
                clsPimProductPrice.KeywordList = Common.WebRequest.AllData("Keywords")
            End If
            
            pDT = clsPimProductPrice.GetProductList()

            strFileName = ""
            If (pDT.Rows.Count > 0) Then
                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                strFileName = "PIMProductList-" & WebRequest.Instance.Data("OrgID") & ".xls"

                If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\PIMTemplate\" & strFileName)
                End If

                TableToExcelFile(pDT, "", "", pSourceURL & "\" & strFileName)

                'additional by jainne april 10 2015 readonly all except price column

                Dim pFS As IO.FileStream
                Dim pWB As Workbook
                Dim pWS As Worksheet

                pFS = New IO.FileStream(pSourceURL & "\" & strFileName, IO.FileMode.Open)
                pWB = New Workbook
                pWB.Open(pFS)
                pWS = pWB.Worksheets(0)
                'Dim rng As Range
                ''pWS.Cells("A1").Style.IsLocked = False
                'Dim col As Column = pWS.Cells.Columns.Item(10)
                ''pWS.Cells("K:K").Style.IsLocked = False
                'Dim style As Style = pWB.Styles(pWB.Styles.Add())
                'style.IsLocked = False
                ''rng = pWS.Cells.CreateRange("K0", "K" & pDT.Rows.Count)
                ''Dim range As Range = pWS.Cells("K:K").Style.

                'Dim styleFlag As New StyleFlag
                'styleFlag.Locked = False
                ''rng.ApplyStyle(style, styleFlag)


                'Dim column As Column = pWS.Cells.Columns(0)
                'column.Style.IsLocked = False
                'column.ApplyStyle(style, styleFlag)

                Dim i As Integer = 0

                While i < pDT.Rows.Count + 2
                    pWS.Cells("K" & i).Style.IsLocked = False
                    i = i + 1
                End While

                pWS.Protect(ProtectionType.Contents)

                pFS.Close()


                If pFS IsNot Nothing Then pFS.Dispose()
                pWB.Save(pSourceURL & "\" & strFileName)
            End If

            Return JsonDictionary.Serialize("PIMProductList", strFileName)

        End Function

        <HttpPost()>
        Function GetFilterResult() As String
            Dim pDT As DataTable
            Dim clsPimProductPrice As New PIMProductPrice
            Dim strFileName As String
            Dim pSourceURL As String = Common.ServerPath & "temp\PIMProductList"

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPimProductPrice.FilterName = Common.WebRequest.Data("FilterName")

            If clsPimProductPrice.FilterName = "Brand" Then
                pDT = clsPimProductPrice.GetBrand()
            ElseIf clsPimProductPrice.FilterName = "Category" Then
                pDT = clsPimProductPrice.GetCategory()


            End If


            Return JsonDictionary.Serialize("Filter", pDT)



        End Function


        <HttpPost()>
        Function SavePriceExcelFileData(ByVal OrganizationID As String) As String
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
            Dim pTotalExcelColumn As Integer = 10
            Dim objRow As Object
            Dim pX As Integer
            Dim dtaTableRequiredFields As DataTable
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization



            If (OrganizationID Is Nothing) Then
                OrganizationID = WebRequest.Instance.OrgID
            End If

            pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMProductPriceList.xls", IO.FileMode.Open)
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

            'dtaTemplate = clsPIMUpload.GetTemplateColumns(dtaExcelHeaders, OrganizationID)
            If (dtaExcelHeaders.Rows.Count > 0) Then

                '''Set the Headers
                ReDim Preserve strHeader(dtaExcelHeaders.Rows.Count - 1)
                intCntr = 0
                While (intCntr < dtaExcelHeaders.Rows.Count)
                    strHeader(intCntr) = dtaExcelHeaders.Rows(intCntr).Item(0).ToString()
                    intCntr = intCntr + 1
                End While

                '''Save The Excel File Data
                dtaExcelData = New DataTable
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMProductPriceList.xls", strHeader, "")

                Static sSL As New Threading.SpinLock
                strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Uploading data from Excel File. This might take a few moments. Please wait ...")

                mDatabase = mProData.GetCurrentDatabase()
                vPeopleUserID = WebSession.Instance.PeopleUserID
                WebSession.Instance.OrganizationID = OrganizationID

                TimedThread(
                    Sub()

                        Try
                            AppendToLog("BEGIN SaveProductPriceData. ref: vbWYF8BKC5BU!")
                            blnResult = SaveProductPriceData(dtaTemplate, dtaExcelData, OrganizationID, mDatabase, vPeopleUserID)
                            AppendToLog("END SaveProductPriceData. ref: vbB3CT87GWZW!")

                            AppendToLog("BEGIN serializing PIMExcelUpload. ref: vbQFJ386T4SJ!")
                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", IIf(blnResult, "Success", "Failed"))
                            AppendToLog("END serializing PIMExcelUpload. ref: vbVL3KN8ARGU!")

                            If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")) Then
                                IO.File.Delete(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")
                            End If

                            Try
                                If pFS IsNot Nothing Then pFS.Dispose()
                            Catch ex As Exception
                            End Try
                            AppendToLog("ref: vbSVES6NCZHX!")
                        Catch ex As Exception
                            AppendToDebug(ex.Message & vbCrLf & "ref: vbDGVE78R66W!", True, ex, True)
                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Encountered error while uploading data from Excel File. ref: vb2DD3YB5X9R")
                        Finally
                            If sSL.IsHeldByCurrentThread Then
                                ' do something here
                                sSL.Exit()
                            End If
                        End Try

                    End Sub
                )

            End If

            strRet = strExcelFileProgress
            Return strRet

        End Function




        Function SaveProductPriceData(ByVal DTemplate As DataTable, ByVal DTable As DataTable, ByVal vOrgID As String, ByVal vDBaseName As String, ByVal vPeopleUserID As String) As Boolean
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
            Dim pDTUnitMasure As DataTable
            Dim clsPIMMeasure As New PIMMeasure
            Dim strUnitMeasureID As String
            Dim intCntr2 As Integer
            Dim clsPIMUpload As New PIMUpload

            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(vDBaseName))

            Try

                If (DTable.Rows.Count > 0) Then

                    mBusiness.ProData.BeginTransaction()

                    intCntr = 0
                    While (intCntr < DTable.Rows.Count)

                        strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Uploading " & intCntr.ToString("#,##0") & " of " & DTable.Rows.Count.ToString("#,##0") & " data from Excel File. Please wait...")

                        Dim dtaRow As DataRow
                        dtaRow = DTable.Rows(intCntr)

                        '''Get Product ID
                        pFinder = New Finder
                        With pFinder
                            .ProData = mBusiness.ProData
                            .MaxRecords = MaxReturnRow
                            .Distinct = True

                            With .Fields
                                With .Add("ID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prd.ID"
                                End With
                                With .Add("ProductNo", SqlDbType.VarChar)
                                    .Expression = "prd.ProductNo"
                                End With
                                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prcLvl.PriceLevelID"
                                End With
                                With .Add("PriceLevel", SqlDbType.VarChar)
                                    .Expression = "prcLvlDef.PriceLevel"
                                End With
                                With .Add("ProductPriceLevelID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prcLvl.ID"
                                End With
                                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prcLvl.OrganizationID"
                                End With
                            End With

                            .SqlFrom = " FROM LibraryOrganizationPIM orgpim" &
                                        " LEFT JOIN MobizLibraryPIMProduct prd ON (prd.ID = orgpim.ProductID)" &
                                        " LEFT JOIN MobizLibraryPIMProductPriceLevel prcLvl ON (prcLvl.ProductID = orgpim.ProductID)" &
                                        " LEFT JOIN MobizLibraryPIMPriceLevel prcLvlDef ON (prcLvlDef.ID = prcLvl.PriceLevelID)" &
                                        " LEFT JOIN MobizLibraryPIMProductMeasurement meas ON (meas.ProductID = prd.ID)" &
                                        " LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)" &
                                        " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)" &
                                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (orgpim.ProductID = prd001.ProductID)" &
                                        " LEFT JOIN MobizLibraryPIMProductMeasurementBarcode measbarcode ON (meas.ID = measbarcode.StockMeasurementsID)"
                            'required fields
                            .SqlWhere = " WHERE prd.ID =" & ReadyString(dtaRow("ID").ToString().Trim()) &
                                " AND prcLvlDef.PriceLevel = " & ReadyString(dtaRow("PriceLevel").ToString()) &
                                " AND meas.Quantity = " & ReadyString(dtaRow("Quantity").ToString()) &
                                " AND lbl.Label = " & ReadyString(dtaRow("UnitMeasure").ToString()) &
                                " AND prcLvl.OrganizationID = " & ReadyString(vOrgID)

                            Try
                                pDTProduct = .Requery
                            Catch ex As Exception
                                Throw New ApplicationException("ref: vbD7L79NE7ZC!" & vbCrLf & ex.Message & vbCrLf & .CreateSql(), ex)
                            End Try
                        End With

                        strProductID = ""
                        intCntr2 = 0
                        Dim hasMatch = False
                        If (pDTProduct.Rows.Count > 0) Then
                            While intCntr2 < pDTProduct.Rows.Count

                                strProductID = pDTProduct.Rows(intCntr2).Item("ID").ToString().Trim()
                                ' '''Find the org of the uploader. if none, create new entry in product price level table

                                'If (vOrgID.ToString().ToUpper().Trim() = pDTProduct.Rows(intCntr2).Item("OrganizationID").ToString().ToUpper().Trim()) Then
                                '    '''Find the corresponding price level Column 
                                '    hasMatch = True
                                If (dtaRow("PriceLevel").ToString().ToUpper().Trim() = pDTProduct.Rows(intCntr2).Item("PriceLevel").ToString().ToUpper().Trim()) Then

                                    If (dtaRow("Price").ToString().Trim().Length > 0) Then

                                        If (IsNumeric(dtaRow("Price").ToString().Trim())) Then
                                            p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                            p("ProductID") = ReadyString(strProductID)
                                            p("PriceLevelID") = ReadyString(pDTProduct.Rows(intCntr2).Item("PriceLevelID").ToString().Trim())

                                            p("Price") = Convert.ToDouble(dtaRow("Price").ToString().Trim())

                                            p("DeactivateDiscount") = 0
                                            p("UserID") = ReadyString(vPeopleUserID)
                                            p("LastUpdated") = "getdate()"

                                            'always update. no insert. we only deal with existing data here
                                            pSql = p.GetUpdate(" ID = " & ReadyString(pDTProduct.Rows(intCntr2).Item("ProductPriceLevelID").ToString()))

                                            mBusiness.ProData.ExecuteSQL(pSql)
                                        End If

                                    End If

                                End If
                                'Else
                                'If (Not hasMatch) Then
                                '    If (dtaRow("PriceLevel").ToString().ToUpper().Trim() = pDTProduct.Rows(intCntr2).Item("PriceLevel").ToString().ToUpper().Trim()) Then

                                '        'if no matching org. meaning new or no price jud
                                '        If (IsNumeric(dtaRow("Price").ToString().Trim())) Then
                                '            p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")
                                '            'this is the time where we add org id
                                '            p("OrganizationID") = ReadyString(vOrgID)
                                '            p("ProductID") = ReadyString(strProductID)
                                '            p("PriceLevelID") = ReadyString(pDTProduct.Rows(intCntr2).Item("PriceLevelID").ToString().Trim())

                                '            p("Price") = Convert.ToDouble(dtaRow("Price").ToString().Trim())

                                '            p("DeactivateDiscount") = 0
                                '            p("UserID") = ReadyString(vPeopleUserID)
                                '            p("LastUpdated") = "getdate()"

                                '            'insert new record
                                '            pSql = p.GetInsert()


                                '            mBusiness.ProData.ExecuteSQL(pSql)
                                '        End If

                                '    End If

                                'End If



                                'End If

                                intCntr2 = intCntr2 + 1


                            End While


                        End If

                        '''End Insert into MobizLibraryPIMProductPriceLevel----------------

                        intCntr = intCntr + 1
                    End While

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    mBusiness.ProData.CommitTransaction()

                End If

                blnRet = True

            Catch ex As Exception
                AppendToDebug("ref: vbQEN77HUP3G!" & vbCrLf & ex.Message, True, ex, True)
                mBusiness.ProData.RollbackTransaction()
            End Try

            Return blnRet

        End Function

        <HttpPost()>
        Public Function UploadRequestFollowUp() As String

            Return strExcelFileProgress

        End Function

        Function CheckExcelRequiredColumns(ByVal vDTable As DataTable) As Boolean
            Dim intExcelColCntr As Integer = 0
            Dim isIDExist As Boolean = False
            Dim isPriceLevelExist As Boolean = False
            Dim isPriceExist As Boolean = False

            While (intExcelColCntr < vDTable.Rows.Count)
                If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "ID") Then
                    isIDExist = True
                End If
                If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "Quantity") Then
                    isPriceLevelExist = True
                End If
                If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "UnitMeasure") Then
                    isPriceLevelExist = True
                End If
                If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "PriceLevel") Then
                    isPriceLevelExist = True
                End If
                If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "Price") Then
                    isPriceExist = True
                End If

                intExcelColCntr = intExcelColCntr + 1
            End While

            If (isIDExist And isPriceLevelExist And isPriceExist) Then
                Return True
            Else
                Return False
            End If

        End Function

        <HttpPost()>
        Function IsProductPriceExcelUploaded() As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = WebSession.Instance.SerializedString

            WebSession.Instance.PIMTemplateUploaded = ""
            WebSession.Instance.SerializedString = ""

            Return strReturn

        End Function

        Public Class PimPriceAdjustJson
            Public PriceLevelToAdjustID As String
            Public Class ProductClass
                Public ProductID As String
                Public ResultingPrice As String

                Public ReadOnly Property ResultingPriceProper As Decimal
                    Get

                    End Get
                End Property
            End Class

            Public Products As List(Of ProductClass)
        End Class

        <HttpPost()>
        Function PimPriceAdjust() As String

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                Dim pInput = Newtonsoft.Json.JsonConvert.DeserializeObject(Of PimPriceAdjustJson)(Common.WebRequest.InputStreamText)

                WebRequest.Instance.ProData.BeginTransaction()
                For Each pProduct In pInput.Products
                    WebRequest.Instance.Business.LibraryStock.SetPrice(pProduct.ProductID, pInput.PriceLevelToAdjustID, pProduct.ResultingPrice, WebRequest.Instance.OrgID)
                Next pProduct
                WebRequest.Instance.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                AppendToDebug("ref: vb4NV32MU9CK!" & ex.Message, True, ex, True)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function
    End Class

End Namespace