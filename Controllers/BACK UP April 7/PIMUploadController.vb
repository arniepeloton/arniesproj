Imports Aspose.Cells

Namespace WebProj

    Public Class PIMUploadController
        Inherits System.Web.Mvc.Controller

        Dim mProData As ProData = Common.WebRequest.ProData
        Public Property MaxReturnRow As Integer = 50

        Shared mDatabase As String
        Shared vPeopleUserID As String
        Shared strExcelFileProgress As String

        Function Index() As ActionResult

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
                objFile.SaveAs(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMTemplate.xls")

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
                        WebSession.Instance.PIMTemplateUploaded = "NewTemplate"
                        strSerializedCols = JsonDictionary.Serialize("PIMExcelUpload", dtaExcelHeadersMiss)
                        WebSession.Instance.SerializedString = strSerializedCols
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

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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

                '''Save The Excel File Data
                dtaExcelData = New DataTable
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls", strHeader, "")

                Static sSL As New Threading.SpinLock
                strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Uploading data from Excel File. This might take a few moments. Please wait ...")

                mDatabase = mProData.GetCurrentDatabase()
                vPeopleUserID = WebSession.Instance.PeopleUserID
                WebSession.Instance.OrganizationID = OrganizationID

                TimedThread(
                    Sub()

                        Try

                            blnResult = SavePIMTemplateData(dtaTemplate, dtaExcelData, OrganizationID, mDatabase, vPeopleUserID)

                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", IIf(blnResult, "Success", "Failed"))

                            If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")) Then
                                IO.File.Delete(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")
                            End If

                            Try
                                If pFS IsNot Nothing Then pFS.Dispose()
                            Catch ex As Exception
                            End Try

                        Catch ex As Exception
                            strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Encountered error while uploading data from Excel File.")
                            AppendToLog(ex.Message)
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

        Function SavePIMTemplateData(ByVal DTemplate As DataTable, ByVal DTable As DataTable, ByVal vOrgID As String, ByVal vDBaseName As String, ByVal vPeopleUserID As String) As Boolean
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
            Dim strMeasurementLevelID As String
            Dim intMeasurementLevel As Integer
            Dim pDTMeasurement As DataTable

            Dim clsPIMUpload As New PIMUpload

            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(vDBaseName))

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

                    mBusiness.ProData.BeginTransaction()

                    intCntr = 0
                    While (intCntr < DTable.Rows.Count)

                        strExcelFileProgress = JsonDictionary.Serialize("PIMExcelUpload", "Uploading " & intCntr.ToString("#,##0") & " of " & DTable.Rows.Count.ToString("#,##0") & " data from Excel File. Please wait...")

                        Dim dtaRow As DataRow
                        dtaRow = DTable.Rows(intCntr)

                        p = New InsertUpdate("MobizLibraryPIMProduct")

                        p("PublishedByID") = ReadyString(vOrgID)
                        p("UploadedByID") = ReadyString(vOrgID)
                        p("IsBrandOwner") = clsPIMUpload.IsOrgBrandOwner(dtaRow("Brand").ToString(), dtaOrgBrand)

                        '''Find Short Product Name Column fixed Feature ID DAFBB390-EA84-4EC8-948B-74B028CBE996
                        strExcelColName = clsPIMUpload.GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("ProductName") = ReadyString(dtaRow(strExcelColName).ToString())
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
                            p("SRP") = Convert.ToDouble(dtaRow(strExcelColName).ToString())
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

                        '''Find Product No Column fixed Feature ID F9CB775D-5E60-4D92-98A3-2A8444D5F32A
                        strExcelColName = clsPIMUpload.GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", DTemplate)
                        If (strExcelColName.Trim().Length > 0) Then
                            p("ProductNo") = ReadyString(dtaRow(strExcelColName).ToString())
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
                                With .Add("UploadedByID", SqlDbType.UniqueIdentifier)
                                    .Expression = "prd.UploadedByID"
                                End With
                            End With

                            .SqlFrom = "FROM MobizLibraryPIMProduct prd "
                            'additonal by janine - JAN 12 2015. COMPARE ONLY IN PRIVATE. NOT PUBLIC. so whatever 
                            'you upload, will be uploaded, not overwrittenn or ignored by an existing product in the public db
                            'add the uploaded by id in where clause to check against private if there's an existing. 
                            .SqlFrom &= " INNER JOIN LibraryOrganizationPIM orgPIM ON prd.ID = orgPIM.ProductID "
                           
                            .SqlWhere = "WHERE prd.ProductNo =" & ReadyString(dtaRow(strExcelColName).ToString().Trim())
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
                        End If

                        strProductSearchString = ""
                        intTemplateColCntr = 0
                        While (intTemplateColCntr < DTemplate.Rows.Count)
                            strProductSearchString = strProductSearchString & IIf(strProductSearchString.Trim().Length > 0, ", " & dtaRow(intTemplateColCntr).ToString().Trim(), dtaRow(intTemplateColCntr).ToString().Trim())
                            intTemplateColCntr = intTemplateColCntr + 1
                        End While

                        p("ProductSearchString") = ReadyString(strProductSearchString)

                        p("DateAdded") = "getdate()"
                        p("IsActive") = "1"
                        p("UserID") = ReadyString(vPeopleUserID)
                        p("LastUpdated") = "getdate()"

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
                                                intMeasurementQty = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                            End If
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
                                                    If (Convert.ToDouble(pDTMeasurement.Rows(intMeasurementCntr).Item("Quantity").ToString()) = intMeasurementQty) Then
                                                        strMeasurementID = pDTMeasurement.Rows(intMeasurementCntr).Item("ID").ToString()
                                                        intMeasurementLevel = Convert.ToInt32(pDTMeasurement.Rows(intMeasurementCntr).Item("MeasurementLevel").ToString())
                                                        Exit While
                                                    ElseIf (Convert.ToDouble(pDTMeasurement.Rows(intMeasurementCntr).Item("Quantity").ToString()) > intMeasurementQty) Then
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
                                            p("ContentUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                                        End If

                                        '''Find Content Qty Column fixed Feature ID 443C66E2-F7A3-4C8B-A287-3D5E2D1945F4
                                        strExcelColName = clsPIMUpload.GetExcelColumn("443C66E2-F7A3-4C8B-A287-3D5E2D1945F4", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                p("ContentQty") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                            End If
                                        End If

                                        '''Find Product Dimension Length Column fixed Feature ID 682E19BD-C47A-4760-B120-3DD931D5BB49
                                        strExcelColName = clsPIMUpload.GetExcelColumn("682E19BD-C47A-4760-B120-3DD931D5BB49", DTemplate)
                                        If (strExcelColName.Trim().Length > 0) Then
                                            If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                                p("ProdDimensionLength") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
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
                                                p("ProdDimensionWidth") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
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
                                                p("ProdDimensionHeight") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
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
                                                p("ProductWeight") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
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

                            End If


                            If (intMeasurementLevel = 1) Then

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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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

                                '''Find Shipping Cost Column fixed Feature ID 2254751E-E1FC-4704-B494-A39FDBA2A245
                                strExcelColName = clsPIMUpload.GetExcelColumn("2254751E-E1FC-4704-B494-A39FDBA2A245", DTemplate)
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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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


                                '''Find Cost from Vendor Column fixed Feature ID E531191E-203E-41CC-BA9B-C46A27D10CD8
                                strExcelColName = clsPIMUpload.GetExcelColumn("E531191E-203E-41CC-BA9B-C46A27D10CD8", DTemplate)
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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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

                                '''Find Landed Cost Column fixed Feature ID 1eb487fc-8b91-4d0c-901f-16bf4b80a9a3
                                strExcelColName = clsPIMUpload.GetExcelColumn("1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", DTemplate)
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
                                            p("Price") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                        End If

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
                                '''End Insert into MobizLibraryPIMProductPriceLevel----------------

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
                                    p("IsDeactivated") = "0"
                                    p("IsDoNotShareTransDataToBrandOwner") = "0"

                                    pSql = p.GetInsert()
                                    mBusiness.ProData.ExecuteSQL(pSql)
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


                            intProductCntr = intProductCntr + 1
                        End While

                        'pSql = p2.GetInsert()
                        'Common.WebRequest.ProData.ExecuteSQL(pSql)

                        intCntr = intCntr + 1
                    End While

                    'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
                    'Common.WebRequest.ProData.ExecuteSQL(pSql)

                    'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductMeasurementBarcode'"
                    'Common.WebRequest.ProData.ExecuteSQL(pSql)

                    'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductPriceLevel'"
                    'Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
                    mBusiness.ProData.ExecuteSQL(pSql)

                    mBusiness.ProData.CommitTransaction()

                End If

                blnRet = True

            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
            End Try

            Return blnRet

        End Function

        <HttpPost()>
        Public Function UploadRequestFollowUp() As String

            Return strExcelFileProgress

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

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (OrganizationID Is Nothing) Then
                OrganizationID = WebRequest.Instance.OrgID
            End If

            Dim mBusiness As Business
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE " & EscapeSqlName(mProData.GetCurrentDatabase()))

            pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls", IO.FileMode.Open)
            pWB = New Workbook
            pWB.Open(pFS)
            pWS = pWB.Worksheets(0)

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
            If (dtaTemplate.Rows.Count > 0) Then

                '''Check for required fields
                dtaTableRequiredFields = clsPIMUpload.CheckRequiredFields(dtaTemplate)
                If (dtaTableRequiredFields.Rows.Count > 0) Then

                    pOut.Add("PIMExcelUpload", "NeedRequiredFields")
                    pOut.Add("PIMExcelUploadRequired", dtaTableRequiredFields)

                    Try
                        If pFS IsNot Nothing Then pFS.Dispose()
                    Catch ex As Exception
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

                '''Save The Excel File Data
                dtaExcelData = New DataTable
                If pFS IsNot Nothing Then pFS.Dispose()
                dtaExcelData = ExcelToTable(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls", strHeader, "")

                If (dtaExcelData.Rows.Count > 0) Then
                    intCntr = 0
                    While (intCntr < dtaExcelData.Rows.Count)
                        intRow = intCntr + 1

                        blnIsRowOK = True
                        strMessage = ""

                        Dim dtaRow As DataRow
                        dtaRow = dtaExcelData.Rows(intCntr)

                        '''Product No
                        strColName = clsPIMUpload.GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", dtaTemplate)
                        If (dtaRow(strColName).ToString().Trim() = "") Then
                            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                            blnIsAllRowOk = False
                            blnIsRowOK = False
                        Else
                            strProductNo = dtaRow(strColName).ToString().Trim()
                        End If

                        '''Short Product Name
                        strColName = clsPIMUpload.GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", dtaTemplate)
                        If (dtaRow(strColName).ToString().Trim() = "") Then
                            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                            blnIsAllRowOk = False
                            blnIsRowOK = False
                        End If

                        '''Check For Packing Size Unit
                        strColName = clsPIMUpload.GetExcelColumn("C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9", dtaTemplate)
                        If (dtaRow(strColName).ToString().Trim() = "") Then
                            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                            blnIsAllRowOk = False
                            blnIsRowOK = False
                        End If

                        '''Check For Packing Size Qty
                        strColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", dtaTemplate)
                        If (dtaRow(strColName).ToString().Trim() = "") Then
                            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                            blnIsAllRowOk = False
                            blnIsRowOK = False
                        Else
                            strPackingSizeQty = dtaRow(strColName).ToString().Trim()
                            If (Not IsNumeric(strPackingSizeQty)) Then
                                strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                                blnIsAllRowOk = False
                                blnIsRowOK = False
                            End If
                        End If

                        '''Category
                        strColName = clsPIMUpload.GetExcelColumn("325A48B7-58F8-4678-B608-58CF43C59BFD", dtaTemplate)
                        If (dtaRow(strColName).ToString().Trim() = "") Then
                            strMessage = strMessage & IIf(strMessage.Trim().Count > 0, ", " & strColName, strColName)
                            blnIsAllRowOk = False
                            blnIsRowOK = False
                        End If

                        ' '' '''Check for the price if has multiple volume discount schemes
                        ' ''If (IsNumeric(strPackingSizeQty)) Then

                        ' ''    Dim strPriceLevels As String = ""
                        ' ''    Dim dtExcelBase As DataTable
                        ' ''    Dim dtBasePriceLevel As DataTable
                        ' ''    Dim dblRetailPrice As Double = 0
                        ' ''    Dim dblRetailPriceDiscount As Double = 0
                        ' ''    Dim dblWholesalePrice As Double = 0
                        ' ''    Dim dblWholesalePriceDiscount As Double = 0
                        ' ''    Dim dblCEBUPrice As Double = 0
                        ' ''    Dim dblCEBUPriceDiscount As Double = 0
                        ' ''    Dim objPriceLevelRow(1) As Object

                        ' ''    dtaExcelPriceLevel.Columns.Clear()
                        ' ''    dtaExcelPriceLevel.Columns.Add("PriceLevelID")
                        ' ''    dtaExcelPriceLevel.Columns.Add("PriceLevel")

                        ' ''    '''Get Retail Price Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C90
                        ' ''    strColName = clsPIMUpload.GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C90", dtaTemplate)
                        ' ''    If (strColName.Trim().Length > 0) Then
                        ' ''        strPriceLevels = IIf(strPriceLevels.Trim().Length > 0, strPriceLevels & ",'01000012-0005-0000-0000-000000000000'", "'01000012-0005-0000-0000-000000000000'")
                        ' ''        dblRetailPrice = Convert.ToDouble(dtaRow(strColName))

                        ' ''        objPriceLevelRow(0) = "01000012-0005-0000-0000-000000000000"
                        ' ''        objPriceLevelRow(1) = strColName
                        ' ''        dtaExcelPriceLevel.Rows.Add(objPriceLevelRow)
                        ' ''    End If
                        ' ''    '''Get Wholesale Price Column fixed Feature ID 997373c6-b8f1-4912-b03c-7b3bffd74563
                        ' ''    strColName = clsPIMUpload.GetExcelColumn("997373c6-b8f1-4912-b03c-7b3bffd74563", dtaTemplate)
                        ' ''    If (strColName.Trim().Length > 0) Then
                        ' ''        strPriceLevels = IIf(strPriceLevels.Trim().Length > 0, strPriceLevels & ",'01000012-0004-0000-0000-000000000000'", "'01000012-0004-0000-0000-000000000000'")
                        ' ''        dblWholesalePrice = Convert.ToDouble(dtaRow(strColName))

                        ' ''        objPriceLevelRow(0) = "01000012-0004-0000-0000-000000000000"
                        ' ''        objPriceLevelRow(1) = strColName
                        ' ''        dtaExcelPriceLevel.Rows.Add(objPriceLevelRow)
                        ' ''    End If
                        ' ''    '''Get CEBU Price Column fixed Feature ID 7655456c-2cf3-4e23-8f64-b63acf21525b
                        ' ''    strColName = clsPIMUpload.GetExcelColumn("7655456c-2cf3-4e23-8f64-b63acf21525b", dtaTemplate)
                        ' ''    If (strColName.Trim().Length > 0) Then
                        ' ''        strPriceLevels = IIf(strPriceLevels.Trim().Length > 0, strPriceLevels & ",'C5AF3C2C-8CC2-4BFA-A592-620D83368DF1'", "'C5AF3C2C-8CC2-4BFA-A592-620D83368DF1'")
                        ' ''        dblCEBUPrice = Convert.ToDouble(dtaRow(strColName))

                        ' ''        objPriceLevelRow(0) = "C5AF3C2C-8CC2-4BFA-A592-620D83368DF1"
                        ' ''        objPriceLevelRow(1) = strColName
                        ' ''        dtaExcelPriceLevel.Rows.Add(objPriceLevelRow)
                        ' ''    End If

                        ' ''    If (strPriceLevels.Trim().Length > 0) Then

                        ' ''        '''get Base Price of every Price Levels
                        ' ''        pFinder = New Finder
                        ' ''        With pFinder
                        ' ''            .ProData = mBusiness.ProData
                        ' ''            .MaxRecords = MaxReturnRow

                        ' ''            With .Fields
                        ' ''                With .Add("ID", SqlDbType.UniqueIdentifier)
                        ' ''                    .Expression = "orgplevel.ID"
                        ' ''                    .Searchable = False
                        ' ''                End With
                        ' ''                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                        ' ''                    .Expression = "orgplevel.PriceLevelID"
                        ' ''                    .Searchable = False
                        ' ''                End With
                        ' ''                With .Add("Price", SqlDbType.Money)
                        ' ''                    .Expression = "orgplevel.Price"
                        ' ''                End With
                        ' ''            End With

                        ' ''            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel orgplevel "

                        ' ''            .SqlWhere = "WHERE orgplevel.OrganizationID " & IIf(OrganizationID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(OrganizationID)) &
                        ' ''                " AND orgplevel.PriceLevelID IN (" & strPriceLevels & ") " &
                        ' ''                " AND orgplevel.ProductID IN (SELECT ID FROM MobizLibraryPIMProduct WHERE ProductNo = " & ReadyString(strProductNo) & " OR SUBSTRING(ProductNo, PATINDEX('%[^0 ]%', ProductNo + ' '), LEN(ProductNo)) = " & ReadyString(strProductNo.TrimStart("0"c)) & ") "

                        ' ''            dtBasePriceLevel = .Requery
                        ' ''        End With

                        ' ''        If (dtBasePriceLevel.Rows.Count > 0) Then
                        ' ''            '''Get Packing Size Qty Column fixed Feature ID E1059BD9-B365-478F-A443-F465B604D251
                        ' ''            Dim intMeasurementQty As Integer
                        ' ''            Dim intPriceLevelCntr As Integer
                        ' ''            intMeasurementQty = 1
                        ' ''            strColName = clsPIMUpload.GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", dtaTemplate)
                        ' ''            If (strColName.Trim().Length > 0) Then
                        ' ''                intMeasurementQty = Convert.ToDouble(dtaRow(strColName))
                        ' ''            End If

                        ' ''            If (intMeasurementQty > 1) Then ''' Check only if not base measure
                        ' ''                intPriceLevelCntr = 0
                        ' ''                dblRetailPriceDiscount = 0
                        ' ''                dblWholesalePriceDiscount = 0
                        ' ''                dblCEBUPriceDiscount = 0
                        ' ''                While (intPriceLevelCntr < dtBasePriceLevel.Rows.Count)

                        ' ''                    '''Check for Retail Price If has discount
                        ' ''                    If (dtBasePriceLevel.Rows(intPriceLevelCntr).Item("PriceLevelID").ToString().Trim() = "01000012-0005-0000-0000-000000000000") Then
                        ' ''                        If ((dblRetailPrice / intMeasurementQty) <> Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price"))) Then
                        ' ''                            dblRetailPriceDiscount = (((Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty) - dblRetailPrice) / (Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty)) * 100
                        ' ''                        End If
                        ' ''                    End If

                        ' ''                    '''Check for Wholesale Price If has discount
                        ' ''                    If (dtBasePriceLevel.Rows(intPriceLevelCntr).Item("PriceLevelID").ToString().Trim() = "01000012-0004-0000-0000-000000000000") Then
                        ' ''                        If ((dblWholesalePrice / intMeasurementQty) <> Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price"))) Then
                        ' ''                            dblWholesalePriceDiscount = (((Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty) - dblWholesalePrice) / (Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty)) * 100
                        ' ''                        End If
                        ' ''                    End If

                        ' ''                    '''Check for CEBU Price If has discount
                        ' ''                    If (dtBasePriceLevel.Rows(intPriceLevelCntr).Item("PriceLevelID").ToString().Trim() = "C5AF3C2C-8CC2-4BFA-A592-620D83368DF1") Then
                        ' ''                        If ((dblCEBUPrice / intMeasurementQty) <> Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price"))) Then
                        ' ''                            dblCEBUPriceDiscount = (((Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty) - dblCEBUPrice) / (Convert.ToDouble(dtBasePriceLevel.Rows(intPriceLevelCntr).Item("Price")) * intMeasurementQty)) * 100
                        ' ''                        End If
                        ' ''                    End If

                        ' ''                    intPriceLevelCntr = intPriceLevelCntr + 1
                        ' ''                End While

                        ' ''                If (dblRetailPriceDiscount <> dblWholesalePriceDiscount Or
                        ' ''                    dblRetailPriceDiscount <> dblCEBUPriceDiscount) Then

                        ' ''                    blnIsAllPricesOk = False

                        ' ''                End If
                        ' ''            End If

                        ' ''        End If

                        ' ''    End If

                        ' ''End If
                        ' '' '''END Check for the price if has multiple volume discount schemes --------------------

                        If (blnIsRowOK) Then '''clear the row of the red highlights
                            pWS.Cells(intRow, dtaExcelData.Columns.Count - 2).PutValue("")
                            pWS.Cells.Rows(intRow).ApplyStyle(style, styleFlag)
                        Else
                            pWS.Cells(intRow, dtaExcelData.Columns.Count - 2).PutValue("Please enter a value for " & strMessage & ".")
                            pWS.Cells.Rows(intRow).ApplyStyle(styleError, styleFlagError)
                        End If

                        intCntr = intCntr + 1
                    End While

                    ' ''If (Not blnIsAllRowOk) Then
                    ' ''    strFileNameError = OrganizationID & "_PIMTemplate-Errors.xls"
                    ' ''    pWB.Save(Common.ServerPath & "temp\PIMTemplate\" & strFileNameError)

                    ' ''    If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")) Then
                    ' ''        IO.File.Delete(pSourceURL & "\PIMTemplate\" & OrganizationID & "_PIMTemplate.xls")
                    ' ''    End If

                    ' ''End If

                    If (Not blnIsAllPricesOk) Then
                        strPriceDiscountDiff = "Price Discount Difference"
                    End If

                End If

            End If

            pOut.Add("PIMExcelUpload", strFileNameError)
            ' ''pOut.Add("PriceDiscountDiff", strPriceDiscountDiff)
            ' ''pOut.Add("PriceDiscountList", dtaExcelPriceLevel)

            Try
                If pFS IsNot Nothing Then pFS.Dispose()

            Catch ex As Exception
            End Try

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function IsPIMExcelUploaded() As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = WebSession.Instance.SerializedString

            WebSession.Instance.PIMTemplateUploaded = ""
            WebSession.Instance.SerializedString = ""

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
                     "m0b!zp30pl30nly!", , , , , True)

            strReturn = JsonDictionary.Serialize("PIMExcelUpload", "Sent")

            Return strReturn

        End Function


    End Class

End Namespace