Imports Aspose.Cells

Namespace WebProj

    Public Class PIMTemplateController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMGenerateTemplate() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function UploadFailed() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()


        End Function

        Function UploadFailedReturnTemplate() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()


        End Function

        Function DownloadPIMProductListFile() As ActionResult
            Dim pFileName = Common.WebRequest.QueryString("FileName")
            Dim pSourceURL As String = Common.ServerPath & "\temp\PIMProductList\" & pFileName
            Dim pContentType As String = "application/xls"

            Try
                Return File(pSourceURL, pContentType, pFileName)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        <HttpPost()>
        Function Index(objFile As HttpPostedFileBase, OrgID As String) As ActionResult
            Dim clsPIMTemplate As New PIMTemplate
            Dim strHeader(32) As String '20
            Dim strMeassge As String
            Dim dtaTable As DataTable
            Dim pFS As IO.FileStream
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim pX As Integer
            Dim pTotalColumn As Integer = 32 '20
            Dim intCntr As Integer

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If


            Dim pSourceURL As String = Common.ServerPath & "temp"

            If (Not objFile Is Nothing) Then
                objFile.SaveAs(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMTemplate.xls")

                Try
                    pFS = New IO.FileStream(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMTemplate.xls", IO.FileMode.Open)
                    pWB = New Workbook
                    pWB.Open(pFS)
                    pWS = pWB.Worksheets(0)

                    ''Get Headers
                    For pX = 0 To pTotalColumn
                        strHeader(pX) = CStr2(pWS.Cells(1, pX).Value).Trim
                    Next pX

                Catch ex As Exception
                Finally
                    Try
                        If pFS IsNot Nothing Then pFS.Dispose()
                    Catch ex As Exception
                    End Try
                End Try

                dtaTable = ExcelToTable(pSourceURL & "\PIMTemplate\" & OrgID & "_PIMTemplate.xls", strHeader, strMeassge)
                Dim isRedirect As Boolean = False
                Dim isRowOK As Boolean
                Dim arrayPackingSize As New ArrayList
                Dim arrayPackingSizeQty As New ArrayList

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

                If (Not dtaTable Is Nothing) Then
                    Try

                        If (dtaTable.Rows.Count > 0) Then
                            intCntr = 0
                            While (intCntr < dtaTable.Rows.Count)
                                Dim rowPos As Integer = intCntr + 2 '''data starts here on the 3rd row

                                Dim dtaRow As DataRow
                                dtaRow = dtaTable.Rows(intCntr)
                                isRowOK = True

                                '''Short Product Name, Landed Cost and Internal Product Code are required fields. However, there are times that
                                ''' they will be left blank if there are multiple packing size, measurements

                                If (dtaRow("Short Product Name *").ToString().Trim().Length > 0) Then
                                    '''' Clear array of packing size for every new product
                                    arrayPackingSize.Clear()
                                    arrayPackingSizeQty.Clear()

                                    If (dtaRow("Internal Product Code *").ToString().Trim().Length <= 0 Or dtaRow("Landed Cost *").ToString().Trim().Length <= 0) Then
                                        If (dtaRow("Internal Product Code *").ToString().Trim().Length <= 0) Then
                                            pWS.Cells(rowPos, 33).PutValue("Please enter a value for Internal Product Code.")
                                        End If

                                        If (dtaRow("Landed Cost *").ToString().Trim().Length <= 0) Then
                                            pWS.Cells(rowPos, 33).PutValue("Please enter a value for Landed Cost.")
                                        End If


                                        isRedirect = True
                                        isRowOK = False
                                    End If

                                    '''Check if the first or the base packing size is not 1
                                    If (dtaRow("Packing Size Qty *").ToString().Trim().Length > 0) Then
                                        If (dtaRow("Packing Size Qty *").ToString().Trim() <> "1") Then
                                            pWS.Cells(rowPos, 33).PutValue("The first Packing Size Qty must be 1.")
                                            isRedirect = True
                                            isRowOK = False
                                        End If

                                    End If

                                End If


                                ''' Save the packing size unit in an array and check if duplicate exists
                                If (dtaRow("Packing Size Unit *").ToString().Trim().Length > 0) Then
                                    If Not arrayPackingSize.Contains(dtaRow("Packing Size Unit *").ToString()) Then
                                        arrayPackingSize.Add(dtaRow("Packing Size Unit *").ToString())
                                    Else
                                        pWS.Cells(rowPos, 33).PutValue("Packing Size Unit already exists. Please choose another.")
                                        isRedirect = True
                                        isRowOK = False
                                    End If


                                    ''' Save the packing size unit qty in an array and check if duplicate exists
                                    If (dtaRow("Packing Size Qty *").ToString().Trim().Length > 0) Then
                                        If Not arrayPackingSizeQty.Contains(dtaRow("Packing Size Qty *").ToString()) Then
                                            arrayPackingSizeQty.Add(dtaRow("Packing Size Qty *").ToString())
                                        Else
                                            pWS.Cells(rowPos, 33).PutValue("Packing Size Qty already exists. Please enter another value.")
                                            isRedirect = True
                                            isRowOK = False
                                        End If
                                    Else '''if qty is null
                                        isRedirect = True
                                        isRowOK = False
                                    End If

                                Else '''if Packing Size Unit is empty
                                    pWS.Cells(rowPos, 33).PutValue("Please select a Packing Size Unit.")
                                    If (dtaRow("Packing Size Qty *").ToString().Trim().Length <= 0) Then
                                        pWS.Cells(rowPos, 33).PutValue("Please select a Packing Size Unit and enter a value for the Packing Size Qty.")
                                    End If

                                    isRedirect = True
                                    isRowOK = False
                                End If

                                If (isRowOK) Then '''clear the row of the red highlights
                                    pWS.Cells(rowPos, 33).PutValue("")
                                    pWS.Cells.Rows(rowPos).ApplyStyle(style, styleFlag)
                                Else
                                    pWS.Cells.Rows(rowPos).ApplyStyle(styleError, styleFlagError)
                                End If

                                pWB.Save(Common.ServerPath & "temp\PIMTemplate\PIMTemplate-Errors.xls")

                                intCntr = intCntr + 1
                            End While

                            If (isRedirect) Then
                                Return RedirectToAction("UploadFailedReturnTemplate", New With {.OrgID = OrgID})
                                WebSession.Instance.PIMTemplateUploaded = "Failed"
                            Else
                                WebSession.Instance.PIMTemplateArray = clsPIMTemplate.UploadToDBase(dtaTable, OrgID)
                                If (WebSession.Instance.PIMTemplateArray.Count > 0) Then
                                    WebSession.Instance.PIMTemplateUploaded = "Success"
                                Else
                                    WebSession.Instance.PIMTemplateUploaded = "Failed"
                                End If
                            End If

                        Else 'if template is correct but no data
                            Return RedirectToAction("UploadFailed", New With {.OrgID = OrgID})
                        End If

                    Catch ex As Exception
                        Return RedirectToAction("UploadFailed", New With {.OrgID = OrgID})
                    End Try
                End If

            Else 'if no file is uploaded, reload the page
                Return RedirectToAction("Index", New With {.OrgID = OrgID})
            End If


            If (WebSession.Instance.PIMTemplateUploaded = "Success") Then

                Return RedirectToAction("Index", New With {.OrgID = OrgID})

            Else
                Return RedirectToAction("UploadFailed", New With {.OrgID = OrgID})
            End If

        End Function

        Function DownloadReturnTemplate() As ActionResult

            '''Download the File
            Dim pContentType As String
            Dim pSourceURL As String
            pSourceURL = Common.ServerPath & "temp\PIMTemplate\PIMTemplate-Errors.xls"
            pContentType = "application/xls"

            Return File(pSourceURL, pContentType, "PIMTemplate-Errors.xls")

        End Function


        Sub SaveToOrganizationPIM(ByVal lstProductID As ArrayList)
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Sub

            Dim clsPIM As New PIMTemplate

            clsPIM.ProductList = lstProductID

            WebSession.Instance.PIMTemplateUploaded = IIf(clsPIM.SaveToOrganizationPIM(), "Success", "Failed")


        End Sub

        Function DownloadReActivationTemplate() As ActionResult
            '''Download the File
            Dim pContentType As String
            Dim pSourceURL As String
            pSourceURL = Common.ServerPath & "temp\PIMTemplate\PIMReActivationTemplate.xlsx"
            pContentType = "application/xls"

            Return File(pSourceURL, pContentType, "PIMReActivationTemplate.xlsx")

        End Function

        Function DownloadTemplate() As ActionResult
            Dim clsPIMTemplate As New PIMTemplate
            Dim pDT As DataTable
            Dim pFS As IO.FileStream
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim pWSUnitMeasure As Worksheet
            Dim pWSSample As Worksheet
            Dim pWSUNSPSCClass As Worksheet
            Dim pCell As Cells
            Dim intCntr As String
            Dim strUnitMeasure As String

            Try
                If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                    If WebRequest.Instance.HandleAccess = False Then Exit Function
                End If
                If WebRequest.Instance.HandleAjax = False Then Return Content(JsonDictionary.Serialize("Error", "Access denied"))
                If WebRequest.Instance.HandleOrganization(True) = False Then Return Content(WebRequest.InvalidOrganization)

                pFS = New IO.FileStream(Common.ServerPath & "/Content/PIM/PIMTemplate.xls", IO.FileMode.Open)
                pWB = New Workbook
                pWB.Open(pFS)
                pWS = pWB.Worksheets(0)
                pWSUnitMeasure = pWB.Worksheets(2)
                pCell = pWS.Cells

                Dim validationAll As Validation = pWS.Validations(pWS.Validations.Add())
                validationAll.Type = ValidationType.List
                validationAll.Operator = OperatorType.None
                validationAll.InCellDropDown = True
                validationAll.ShowError = True
                validationAll.AlertStyle = ValidationAlertType.Stop
                validationAll.ErrorTitle = "Error"
                validationAll.ErrorMessage = "Please select a measurement from the list"

                '''get List of UnitMeasure (ALL)
                pDT = New DataTable()
                pDT = clsPIMTemplate.GetUnitMeasureList("")
                Dim rngMeasureAll As Range = pWSUnitMeasure.Cells.CreateRange("A1", "A" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
                rngMeasureAll.Name = "MeasureAllRange"
                If (pDT.Rows.Count > 0) Then


                    If (pDT.Rows.Count > 0) Then
                        intCntr = 0
                        While (intCntr < pDT.Rows.Count)
                            rngMeasureAll(intCntr, 0).PutValue(pDT.Rows(intCntr).Item("Measurement").ToString())

                            intCntr = intCntr + 1
                        End While

                    End If

                End If

                validationAll.Formula1 = "=MeasureAllRange"

                Dim areaUnitMeasure As CellArea
                areaUnitMeasure.StartRow = 1
                areaUnitMeasure.EndRow = 300
                areaUnitMeasure.StartColumn = 11
                areaUnitMeasure.EndColumn = 11
                validationAll.AreaList.Add(areaUnitMeasure)

                '''get List of Standard Units for Content
                Dim validationContentUnit As Validation = pWS.Validations(pWS.Validations.Add())
                validationContentUnit.Type = ValidationType.List
                validationContentUnit.Operator = OperatorType.None
                validationContentUnit.InCellDropDown = True
                validationContentUnit.ShowError = True
                validationContentUnit.AlertStyle = ValidationAlertType.Stop
                validationContentUnit.ErrorTitle = "Error"
                validationContentUnit.ErrorMessage = "Please select a measurement from the list"

                pDT = New DataTable()
                pDT = clsPIMTemplate.GetUnitMeasureList("Content")
                Dim rngMeasureContent As Range = pWSUnitMeasure.Cells.CreateRange("D1", "D" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
                rngMeasureContent.Name = "MeasureContentRange"
                If (pDT.Rows.Count > 0) Then

                    If (pDT.Rows.Count > 0) Then
                        intCntr = 0
                        While (intCntr < pDT.Rows.Count)
                            rngMeasureContent(intCntr, 0).PutValue(pDT.Rows(intCntr).Item("Measurement").ToString() & " (" & pDT.Rows(intCntr).Item("Sign").ToString() & ")")

                            intCntr = intCntr + 1
                        End While

                    End If

                End If

                validationContentUnit.Formula1 = "=MeasureContentRange"


                Dim areaContentUnit As CellArea
                areaContentUnit.StartRow = 1
                areaContentUnit.EndRow = 300
                areaContentUnit.StartColumn = 13
                areaContentUnit.EndColumn = 13
                validationContentUnit.AreaList.Add(areaContentUnit)

                '''''
                '''get List of UnitMeasure (Length)
                Dim validationLength As Validation = pWS.Validations(pWS.Validations.Add())
                validationLength.Type = ValidationType.List
                validationLength.Operator = OperatorType.None
                validationLength.InCellDropDown = True
                validationLength.ShowError = True
                validationLength.AlertStyle = ValidationAlertType.Stop
                validationLength.ErrorTitle = "Error"
                validationLength.ErrorMessage = "Please select a measurement from the list"

                pDT = New DataTable()
                pDT = clsPIMTemplate.GetUnitMeasureList("Length")
                Dim rngMeasureLength As Range = pWSUnitMeasure.Cells.CreateRange("H1", "H" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
                rngMeasureLength.Name = "MeasureLengthRange"
                If (pDT.Rows.Count > 0) Then


                    If (pDT.Rows.Count > 0) Then
                        intCntr = 0
                        While (intCntr < pDT.Rows.Count)
                            rngMeasureLength(intCntr, 0).PutValue(pDT.Rows(intCntr).Item("Measurement").ToString() & " (" & pDT.Rows(intCntr).Item("Sign").ToString() & ")")

                            intCntr = intCntr + 1
                        End While

                    End If

                End If

                validationLength.Formula1 = "=MeasureLengthRange"

                Dim areaPrdLengthMeasure As CellArea
                areaPrdLengthMeasure.StartRow = 1
                areaPrdLengthMeasure.EndRow = 300
                areaPrdLengthMeasure.StartColumn = 15
                areaPrdLengthMeasure.EndColumn = 15
                validationLength.AreaList.Add(areaPrdLengthMeasure)

                Dim areaPrdWidthMeasure As CellArea
                areaPrdWidthMeasure.StartRow = 1
                areaPrdWidthMeasure.EndRow = 300
                areaPrdWidthMeasure.StartColumn = 17
                areaPrdWidthMeasure.EndColumn = 17
                validationLength.AreaList.Add(areaPrdWidthMeasure)

                Dim areaPrdHeightMeasure As CellArea
                areaPrdHeightMeasure.StartRow = 1
                areaPrdHeightMeasure.EndRow = 300
                areaPrdHeightMeasure.StartColumn = 19
                areaPrdHeightMeasure.EndColumn = 19
                validationLength.AreaList.Add(areaPrdHeightMeasure)

                '''get List of UnitMeasure (WEight)
                Dim validationWeight As Validation = pWS.Validations(pWS.Validations.Add())
                validationWeight.Type = ValidationType.List
                validationWeight.Operator = OperatorType.None
                validationWeight.InCellDropDown = True
                validationWeight.ShowError = True
                validationWeight.AlertStyle = ValidationAlertType.Stop
                validationWeight.ErrorTitle = "Error"
                validationWeight.ErrorMessage = "Please select a measurement from the list"

                pDT = New DataTable()
                pDT = clsPIMTemplate.GetUnitMeasureList("Weight/Mass")
                Dim rngMeasureWeightMass As Range = pWSUnitMeasure.Cells.CreateRange("J1", "J" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
                rngMeasureWeightMass.Name = "MeasureWeightMassRange"
                If (pDT.Rows.Count > 0) Then

                    If (pDT.Rows.Count > 0) Then
                        intCntr = 0
                        While (intCntr < pDT.Rows.Count)
                            rngMeasureWeightMass(intCntr, 0).PutValue(pDT.Rows(intCntr).Item("Measurement").ToString() & " (" & pDT.Rows(intCntr).Item("Sign").ToString() & ")")

                            intCntr = intCntr + 1
                        End While

                    End If

                End If

                validationWeight.Formula1 = "=MeasureWeightMassRange"


                Dim areaWeightUnit As CellArea
                areaWeightUnit.StartRow = 1
                areaWeightUnit.EndRow = 300
                areaWeightUnit.StartColumn = 21
                areaWeightUnit.EndColumn = 21
                validationWeight.AreaList.Add(areaWeightUnit)

                pWSUnitMeasure.Protect(ProtectionType.All, "un!tm3asur3restricted", DBNull.Value.ToString())
                AddPriceLevelColumns(pWS)
                pWS.AutoFitColumns()

                ''Freeze the panes
                'pWS.FreezePanes(2, 1, 2, 1)


                Dim pTempFilename As String = Common.ServerPath & "temp\PIMTemplate\PIMTemplate.xls"
                CreateFolder(pTempFilename)
                pWB.Save(pTempFilename)

                '''Download the File
                Dim pContentType As String
                Dim pSourceURL As String
                pSourceURL = pTempFilename
                pContentType = "application/xls"

                Try
                    If pFS IsNot Nothing Then pFS.Dispose()
                Catch ex As Exception
                End Try

                Return File(pSourceURL, pContentType, "PIMTemplate.xls")
            Catch ex As Exception
                AppendToDebug("ref: vb78D3V4ZE2K! " & ex.Message, True, ex, True)
                Throw
            End Try

        End Function

        Private Sub AddPriceLevelColumns(ByRef rWS As Worksheet)

            Dim pY = 0
            Dim pText As String
            Dim pPriceLevelIDs As New List(Of String)
            Dim pLastPriceColumn As Integer = rWS.Cells.MaxDataColumn

            WebRequest.Instance.Business.LibraryStock.AddPricelLevelsToPimTemplate(WebRequest.Instance.OrgID, WebSession.Instance.PeopleUserID)
            Dim pDtPriceLevels = WebRequest.Instance.Business.LibraryStock.PriceLevelsPimTemplate(WebRequest.Instance.OrgID, False)

            Dim pLastColumn = rWS.Cells.MaxDataColumn
            For pX = 0 To pLastColumn
                pText = CStr2(rWS.Cells(pY, pX).Value).Trim().ToLower()
                pDtPriceLevels.DefaultView.RowFilter = "ColumnName = " & ReadyString(pText)
                If pDtPriceLevels.DefaultView.Count > 0 Then
                    pPriceLevelIDs.Add(ReadyGuid(pDtPriceLevels.DefaultView(0)("ID")))
                    pLastPriceColumn = pX
                End If
            Next pX

            For Each pDR As DataRow In pDtPriceLevels.Rows
                Dim pColumnName = CStr2(pDR("ColumnName")).Trim()
                If String.IsNullOrWhiteSpace(pColumnName) Then
                    Continue For
                End If
                Dim pPriceLevelID = ReadyGuid(pDR("ID"))
                If pPriceLevelIDs.Contains(pPriceLevelID) Then
                    Continue For
                End If
                pPriceLevelIDs.Add(pPriceLevelID)

                Dim pFound = False
                pLastColumn = rWS.Cells.MaxDataColumn
                For pX = 0 To pLastColumn
                    pText = CStr2(rWS.Cells(pY, pX).Value).Trim().ToLower()
                    If pText = pColumnName.ToLower() Then
                        pFound = True
                        Exit For
                    End If
                Next pX
                If pFound = False Then
                    pLastPriceColumn += 1
                    rWS.Cells.InsertColumn(pLastPriceColumn)
                    rWS.Cells(pY, pLastPriceColumn).PutValue(pColumnName)
                End If
            Next pDR

        End Sub

        <HttpPost()>
        Function IsPIMTemplateUploaded() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("PIMTemplateUpload", IIf(WebSession.Instance.PIMTemplateUploaded = "Success", "TRUE", "FALSE"))

            '''Save to private database
            If (WebSession.Instance.PIMTemplateUploaded = "Success") Then
                SaveToOrganizationPIM(WebSession.Instance.PIMTemplateArray)
            End If


            WebSession.Instance.PIMTemplateUploaded = ""

            Return pOut.Serialize

        End Function



    End Class

End Namespace