Imports Aspose.Cells


Namespace WebProj

    Public Class GenerateTemplateController
        Inherits System.Web.Mvc.Controller
        Public Shared FeatureList As ArrayList
        Public Shared DefaultFeatureList As ArrayList
        '
        ' GET: /GenerateTemplate

        Function Index() As ActionResult
            FeatureList = New ArrayList

            DefaultFeatureList = New ArrayList

            DefaultFeatureList.Add("Short Product Name")
            DefaultFeatureList.Add("UNSPSC Classification by Class")
            DefaultFeatureList.Add("Your Custom Classification")
            DefaultFeatureList.Add("Brand")
            DefaultFeatureList.Add("Model Number")
            DefaultFeatureList.Add("Internal Product Code")
            DefaultFeatureList.Add("Product Grouping Number")
            DefaultFeatureList.Add("UNSPSC Commodity Type")
            DefaultFeatureList.Add("Product Type")
            DefaultFeatureList.Add("Price - Suggested Retail")
            DefaultFeatureList.Add("Marketing Text")
            DefaultFeatureList.Add("Warranty")
            DefaultFeatureList.Add("UPC")
            DefaultFeatureList.Add("Packing Size Unit")
            DefaultFeatureList.Add("Packing Size Quantity")
            DefaultFeatureList.Add("Content")
            DefaultFeatureList.Add("Product Dimensions")
            DefaultFeatureList.Add("Product Gross Weight")


            Return View()
        End Function

        Function getUNSPSCClass() As String
            Dim strReturn As String
            Dim dataTblUNSPSCClassList As DataTable

            If Common.WebRequest.QueryString("term").Trim() <> "" Then
                dataTblUNSPSCClassList = Common.WebRequest.ProData.Execute("SELECT ID as ProductID, Class as Product FROM LibraryUNSPSCClass WHERE Class LIKE " + ReadyStringWild(Common.WebRequest.QueryString("term").Trim()) + " UNION SELECT ID as ProductID, Commodity as Product FROM LibraryUNSPSCCommodity WHERE Commodity LIKE " + ReadyStringWild(Common.WebRequest.QueryString("term").Trim()) + " ORDER BY Product")
                strReturn = JsonDictionary.Serialize("UNSPSCClassData", dataTblUNSPSCClassList)
            End If

            Return strReturn

        End Function


        Function getPIMProductFeature() As String
            Dim strReturn As String
            Dim dataTblPIMProductFeatureList As DataTable

            If Common.WebRequest.QueryString("term").Trim() <> "" Then
                dataTblPIMProductFeatureList = Common.WebRequest.ProData.Execute("SELECT DISTINCT dbo.LibraryPIMFeature.FeatureLabelID ProductFeatureID, dbo.LibraryCountriesLanguageLabel.Label as ProductFeature " &
                    "FROM dbo.LibraryPIMFeature INNER JOIN " &
                     " dbo.LibraryCountriesLanguageLabel ON dbo.LibraryPIMFeature.FeatureLabelID = dbo.LibraryCountriesLanguageLabel.ID " &
                         " WHERE dbo.LibraryCountriesLanguageLabel.Label LIKE " + ReadyStringWild(Common.WebRequest.QueryString("term").Trim()) + " AND dbo.LibraryPIMFeature.PIMFeatureColumn != '' ORDER BY dbo.LibraryCountriesLanguageLabel.Label")

                strReturn = JsonDictionary.Serialize("PIMProductFeatureData", dataTblPIMProductFeatureList)
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function getUNSPSCFeature() As String
            Dim strReturn As String
            Dim dataTblUNSPSCFeatureList As DataTable

            dataTblUNSPSCFeatureList = Common.WebRequest.ProData.Execute("SELECT commodityFeat.FeatureID as FeatureIDValue, lbl.Label as FeatureLabelValue FROM LibraryUNSPSCCommodityFeature commodityFeat INNER JOIN LibraryPIMFeature feat ON commodityFeat.FeatureID =  feat.ID INNER  JOIN LibraryCountriesLanguageLabel lbl ON feat.FeatureLabelID = lbl.ID WHERE commodityFeat.SegmentID = '13A2E587-57A3-4F9A-B1E6-364CC10CF9FF' ORDER BY lbl.Label")
            'dataTblUNSPSCFeatureList = Common.WebRequest.ProData.Execute("SELECT commodityFeat.FeatureID as FeatureIDValue, lbl.Label as FeatureLabelValue FROM LibraryUNSPSCCommodityFeature commodityFeat INNER JOIN LibraryPIMFeature feat ON commodityFeat.FeatureID =  feat.ID INNER  JOIN LibraryCountriesLanguageLabel lbl ON feat.FeatureLabelID = lbl.ID WHERE commodityFeat.ClassID = '" + Common.WebRequest.Data("ProductID") + "' OR commodityFeat.CommodityID = '" + Common.WebRequest.Data("ProductID") + "' ORDER BY lbl.Label")
            strReturn = JsonDictionary.Serialize("UNSPSCFeatureData", dataTblUNSPSCFeatureList)

            Dim total As Integer

            'Count the number of rows
            total = dataTblUNSPSCFeatureList.Rows.Count - 1

            For i As Integer = 0 To total
                If Not DefaultFeatureList.Contains(dataTblUNSPSCFeatureList.Rows(i).ItemArray(1)) Then 'Check first if there is a duplicate record with the default
                    If Not FeatureList.Contains(dataTblUNSPSCFeatureList.Rows(i).ItemArray(1)) Then 'Check if there are duplicates in the existing features added
                        FeatureList.Add(dataTblUNSPSCFeatureList.Rows(i).ItemArray(1))
                    End If
                End If
            Next


            Return strReturn

        End Function

        'Remove custom feature from the array list
        <HttpPost()>
        Function removeCustomFeature() As String
            Dim dataTblFeatureID As DataTable
            Dim strReturn As String

            'To remove the item from the array declared in javascript (see GenerateTemplateInfo.js), we must find the ID of the item and pass it back
            dataTblFeatureID = Common.WebRequest.ProData.Execute("SELECT DISTINCT dbo.LibraryPIMFeature.FeatureLabelID ProductFeatureID " &
                    "FROM dbo.LibraryPIMFeature INNER JOIN " &
                     " dbo.LibraryCountriesLanguageLabel ON dbo.LibraryPIMFeature.FeatureLabelID = dbo.LibraryCountriesLanguageLabel.ID " &
                         " WHERE dbo.LibraryCountriesLanguageLabel.Label = '" + Common.WebRequest.Data("CustomFeature") + "'")

            strReturn = JsonDictionary.Serialize("FeatureIDData", dataTblFeatureID)

            FeatureList.Remove(Common.WebRequest.Data("CustomFeature"))


            Return strReturn
        End Function

        'Add custom feature to the array list
        <HttpPost()>
        Sub getCustomFeature()

            FeatureList.Add(Common.WebRequest.Data("CustomFeature"))


        End Sub

        ''Add custom feature to the array list
        '<HttpPost()>
        'Function getCustomFeature() As String
        '    Dim strReturn As String
        '    Dim dataTblAddPIMFeature As DataTable

        '    dataTblAddPIMFeature = Common.WebRequest.ProData.Execute("SELECT DISTINCT dbo.LibraryPIMFeature.FeatureLabelID ProductFeatureID, dbo.LibraryCountriesLanguageLabel.Label as ProductFeature " &
        '            "FROM dbo.LibraryPIMFeature INNER JOIN " &
        '             " dbo.LibraryCountriesLanguageLabel ON dbo.LibraryPIMFeature.FeatureLabelID = dbo.LibraryCountriesLanguageLabel.ID " &
        '                 " WHERE dbo.LibraryCountriesLanguageLabel.Label = '%" + Common.WebRequest.QueryString("term").Trim() + "%' AND dbo.LibraryPIMFeature.PIMFeatureColumn != '' ORDER BY dbo.LibraryCountriesLanguageLabel.Label")

        '    strReturn = JsonDictionary.Serialize("PIMFeatureData", dataTblAddPIMFeature)

        '    FeatureList.Add(Common.WebRequest.Data("CustomFeature"))
        '    Return strReturn

        'End Function

        Function TestDownload() As ActionResult
            Dim clsPIMTemplate As New PIMTemplate
            Dim pDT As DataTable
            Dim intCntr As String
            Dim workbook As Workbook = New Workbook()
            Dim wsDataSource As Worksheet

            'Open template
            Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~")
            path = path.Substring(0, path.LastIndexOf("\"))
            path += "/Content/PIM/PIMTemplate.xls"
            workbook.Open(path)

            Dim worksheet As Worksheet = workbook.Worksheets(0)
            wsDataSource = workbook.Worksheets(3)

            Dim validationAll As Validation = worksheet.Validations(worksheet.Validations.Add())
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
            Dim rngMeasureAll As Range = wsDataSource.Cells.CreateRange("A1", "A" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
            rngMeasureAll.Name = "MeasureAllRange"
            If (pDT.Rows.Count > 0) Then


                If (pDT.Rows.Count > 0) Then
                    intCntr = 0
                    While (intCntr < pDT.Rows.Count)
                        rngMeasureAll(intCntr, 0).PutValue(pDT.Rows(intCntr).Item("Measurement").ToString() & " (" & pDT.Rows(intCntr).Item("Sign").ToString() & ")")

                        intCntr = intCntr + 1
                    End While

                End If

            End If

            validationAll.Formula1 = "=MeasureAllRange"

            Dim areaUnitMeasure As CellArea
            areaUnitMeasure.StartRow = 2
            areaUnitMeasure.EndRow = IIf(pDT.Rows.Count > 0, pDT.Rows.Count, 1)
            areaUnitMeasure.StartColumn = 13
            areaUnitMeasure.EndColumn = 13
            validationAll.AreaList.Add(areaUnitMeasure)


            Dim areaContentUnit As CellArea
            areaContentUnit.StartRow = 2
            areaContentUnit.EndRow = IIf(pDT.Rows.Count > 0, pDT.Rows.Count, 1)
            areaContentUnit.StartColumn = 16
            areaContentUnit.EndColumn = 16
            validationAll.AreaList.Add(areaContentUnit)


            '''''
            '''get List of UnitMeasure (Length)
            Dim validationLength As Validation = worksheet.Validations(worksheet.Validations.Add())
            validationLength.Type = ValidationType.List
            validationLength.Operator = OperatorType.None
            validationLength.InCellDropDown = True
            validationLength.ShowError = True
            validationLength.AlertStyle = ValidationAlertType.Stop
            validationLength.ErrorTitle = "Error"
            validationLength.ErrorMessage = "Please select a measurement from the list"

            pDT = New DataTable()
            pDT = clsPIMTemplate.GetUnitMeasureList("Length")
            Dim rngMeasureLength As Range = wsDataSource.Cells.CreateRange("H1", "H" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
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
            areaPrdLengthMeasure.StartRow = 2
            areaPrdLengthMeasure.EndRow = IIf(pDT.Rows.Count > 0, pDT.Rows.Count, 1)
            areaPrdLengthMeasure.StartColumn = 18
            areaPrdLengthMeasure.EndColumn = 18
            validationLength.AreaList.Add(areaPrdLengthMeasure)

            Dim areaPrdWidthMeasure As CellArea
            areaPrdWidthMeasure.StartRow = 2
            areaPrdWidthMeasure.EndRow = IIf(pDT.Rows.Count > 0, pDT.Rows.Count, 1)
            areaPrdWidthMeasure.StartColumn = 20
            areaPrdWidthMeasure.EndColumn = 20
            validationLength.AreaList.Add(areaPrdWidthMeasure)

            Dim areaPrdHeightMeasure As CellArea
            areaPrdHeightMeasure.StartRow = 2
            areaPrdHeightMeasure.EndRow = IIf(pDT.Rows.Count > 0, pDT.Rows.Count, 1)
            areaPrdHeightMeasure.StartColumn = 22
            areaPrdHeightMeasure.EndColumn = 22
            validationLength.AreaList.Add(areaPrdHeightMeasure)

            '''get List of UnitMeasure (WEight)
            Dim validationWeight As Validation = worksheet.Validations(worksheet.Validations.Add())
            validationWeight.Type = ValidationType.List
            validationWeight.Operator = OperatorType.None
            validationWeight.InCellDropDown = True
            validationWeight.ShowError = True
            validationWeight.AlertStyle = ValidationAlertType.Stop
            validationWeight.ErrorTitle = "Error"
            validationWeight.ErrorMessage = "Please select a measurement from the list"

            pDT = New DataTable()
            pDT = clsPIMTemplate.GetUnitMeasureList("Weight/Mass")
            Dim rngMeasureWeightMass As Range = wsDataSource.Cells.CreateRange("J1", "J" & IIf(pDT.Rows.Count > 0, pDT.Rows.Count.ToString(), "1"))
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
            areaWeightUnit.StartRow = 0
            areaWeightUnit.EndRow = IIf(pDT.Rows.Count > 0, pDT.Rows.Count, 1)
            areaWeightUnit.StartColumn = 24
            areaWeightUnit.EndColumn = 24
            validationWeight.AreaList.Add(areaWeightUnit)



            'Add features to the list as string values

            'Importing the contents of ArrayList to 2nd row and 27th column horizontally
            worksheet.Cells.ImportArrayList(FeatureList, 1, 27, False)


            worksheet.AutoFitColumns()

            'Freeze the panes
            worksheet.FreezePanes(2, 1, 2, 1)

            workbook.Save(Common.ServerPath & "temp\PIMTemplate\PIMTemplate.xls")
            '''Download the File
            Dim pContentType As String
            Dim pSourceURL As String
            pSourceURL = Common.ServerPath & "temp\PIMTemplate\PIMTemplate.xls"
            pContentType = "application/xls"

            Return File(pSourceURL, pContentType, "PIMTemplate.xls")
            ' End response to avoid unneeded HTML after XLS
            System.Web.HttpContext.Current.Response.End()


        End Function

    End Class
End Namespace