Public Class PIMUpload
    Public Property MaxReturnRow As Integer = 50

    Public Property OrganizationID As String
    Public Property TemplateName As String
    Public Property PIMTemplateColList As Object()
    Public Property PIMTemplateColAKAList As Object()

    Function CheckRequiredFields() As DataTable
        Dim dtaTableRet As New DataTable
        Dim objRow As Object
        Dim blnIsProductNoExist As Boolean = False
        Dim blnIsShortProductNameExist As Boolean = False
        Dim blnIsBrandExist As Boolean = False
        Dim blnIsUnitPackingSizeExist As Boolean = False
        Dim blnIsUnitPackingQtyExist As Boolean = False

        Dim blnCategoryExist As Boolean = False
        Dim blnIsCostFromVendorExist As Boolean = False
        Dim blnIsShippingCostExist As Boolean = False
        Dim blnIsLandedCostExist As Boolean = False

        Dim intCntr As Integer = 0

        For Each objPIMTemplateCol As Object In PIMTemplateColList

            '''Product No
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "F9CB775D-5E60-4D92-98A3-2A8444D5F32A") Then
                blnIsProductNoExist = True
            End If

            '''Check For Category
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "325A48B7-58F8-4678-B608-58CF43C59BFD") Then
                blnCategoryExist = True
            End If

            '''Short Product Name
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "DAFBB390-EA84-4EC8-948B-74B028CBE996") Then
                blnIsShortProductNameExist = True
            End If

            '''Check For Brand
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "7B3524D2-EFFC-4921-B752-3CF7D84434D6") Then
                blnIsBrandExist = True
            End If

            '''Check For Packing Size Unit
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9") Then
                blnIsUnitPackingSizeExist = True
            End If

            '''Check For Packing Size Qty
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "E1059BD9-B365-478F-A443-F465B604D251") Then
                blnIsUnitPackingQtyExist = True
            End If

            '''Check For Cost From Vendor
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "E531191E-203E-41CC-BA9B-C46A27D10CD8") Then
                blnIsCostFromVendorExist = True
            End If

            '''Check For Shiping Cost 
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "2254751E-E1FC-4704-B494-A39FDBA2A245") Then
                blnIsShippingCostExist = True
            End If

            '''Check For Landed Cost
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "1EB487FC-8B91-4D0C-901F-16BF4B80A9A3") Then
                blnIsLandedCostExist = True
            End If

        Next objPIMTemplateCol

        dtaTableRet.Columns.Add("ExcelColumn")

        '''Check For Product No
        If (Not blnIsProductNoExist) Then
            objRow = "Please include [Product No] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Category
        If (Not blnCategoryExist) Then
            objRow = "Please include [Category] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Short Product Name
        If (Not blnIsShortProductNameExist) Then
            objRow = "Please include [Short Product Name] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Brand
        If (Not blnIsBrandExist) Then
            objRow = "Please include [Brand] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Packing Size Unit
        If (Not blnIsUnitPackingSizeExist) Then
            objRow = "Please include [Packing Size Unit] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Packing Size Qty
        If (Not blnIsUnitPackingQtyExist) Then
            objRow = "Please include [Packing Size Qty] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Cost From Vendor
        If (Not blnIsCostFromVendorExist) Then
            objRow = "Please include [Cost From Vendor] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Shiping Cost 
        If (Not blnIsShippingCostExist) Then
            objRow = "Please include [Shipping Cost] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Landed Cost
        If (Not blnIsLandedCostExist) Then
            objRow = "Please include [Landed Cost] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        Return dtaTableRet

    End Function

    Function CheckRequiredFields(ByVal vDColumns As DataTable) As DataTable
        Dim dtaTableRet As New DataTable
        Dim objRow As Object
        Dim blnIsProductNoExist As Boolean = False
        Dim blnIsShortProductNameExist As Boolean = False
        Dim blnIsBrandExist As Boolean = False
        Dim blnIsUnitPackingSizeExist As Boolean = False
        Dim blnIsUnitPackingQtyExist As Boolean = False
        Dim intCntr As Integer = 0

        Dim blnCategoryExist As Boolean = False
        Dim blnIsCostFromVendorExist As Boolean = False
        Dim blnIsShippingCostExist As Boolean = False
        Dim blnIsLandedCostExist As Boolean = False

        intCntr = 0
        While (intCntr < vDColumns.Rows.Count)

            '''Product No
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "F9CB775D-5E60-4D92-98A3-2A8444D5F32A") Then
                blnIsProductNoExist = True
            End If

            '''Check For Category
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "325A48B7-58F8-4678-B608-58CF43C59BFD") Then
                blnCategoryExist = True
            End If

            '''Short Product Name
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "DAFBB390-EA84-4EC8-948B-74B028CBE996") Then
                blnIsShortProductNameExist = True
            End If

            '''Check For Brand
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "7B3524D2-EFFC-4921-B752-3CF7D84434D6") Then
                blnIsBrandExist = True
            End If

            '''Check For Packing Size Unit
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9") Then
                blnIsUnitPackingSizeExist = True
            End If

            '''Check For Packing Size Qty
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "E1059BD9-B365-478F-A443-F465B604D251") Then
                blnIsUnitPackingQtyExist = True
            End If

            '''Check For Cost From Vendor
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "E531191E-203E-41CC-BA9B-C46A27D10CD8") Then
                blnIsCostFromVendorExist = True
            End If

            '''Check For Shiping Cost 
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "2254751E-E1FC-4704-B494-A39FDBA2A245") Then
                blnIsShippingCostExist = True
            End If

            '''Check For Landed Cost
            If (vDColumns.Rows(intCntr).Item("PIMFeatureID").ToString().Trim().ToUpper() = "1EB487FC-8B91-4D0C-901F-16BF4B80A9A3") Then
                blnIsLandedCostExist = True
            End If

            intCntr = intCntr + 1
        End While

        dtaTableRet.Columns.Add("ExcelColumn")

        '''Check For Product No
        If (Not blnIsProductNoExist) Then
            objRow = "Please include [Product No] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Category
        If (Not blnCategoryExist) Then
            objRow = "Please include [Category] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Short Product Name
        If (Not blnIsShortProductNameExist) Then
            objRow = "Please include [Short Product Name] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Brand
        If (Not blnIsBrandExist) Then
            objRow = "Please include [Brand] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Packing Size Unit
        If (Not blnIsUnitPackingSizeExist) Then
            objRow = "Please include [Packing Size Unit] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Packing Size Qty
        If (Not blnIsUnitPackingQtyExist) Then
            objRow = "Please include [Packing Size Qty] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Cost From Vendor
        If (Not blnIsCostFromVendorExist) Then
            objRow = "Please include [Cost From Vendor] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Shiping Cost 
        If (Not blnIsShippingCostExist) Then
            objRow = "Please include [Shipping Cost] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        '''Check For Landed Cost
        If (Not blnIsLandedCostExist) Then
            objRow = "Please include [Landed Cost] column in your Excel file."
            dtaTableRet.Rows.Add(objRow)
        End If

        Return dtaTableRet

    End Function

    Function CheckUnMatchedFields() As DataTable
        Dim dtaTableRet As New DataTable
        Dim objRow(3) As Object
        Dim intCntr As Integer = 0

        dtaTableRet.Columns.Add("ColumnName")
        dtaTableRet.Columns.Add("FeatureID")
        dtaTableRet.Columns.Add("Feature")
        dtaTableRet.Columns.Add("FeatureType")

        For Each objPIMTemplateCol As Object In PIMTemplateColList

            '''No Matching Column
            If (objPIMTemplateCol("PIMFeatureID").ToString().Trim().ToUpper() = "7E0D90A2-7520-4372-B97D-9CEA41D59E82") Then
                objRow(0) = objPIMTemplateCol("ColumnName").ToString().Trim()
                objRow(1) = objPIMTemplateCol("PIMFeatureID").ToString().Trim()
                objRow(2) = objPIMTemplateCol("PIMFeature").ToString().Trim()
                objRow(3) = objPIMTemplateCol("FeatureType").ToString().Trim()
                dtaTableRet.Rows.Add(objRow)
            End If
        Next objPIMTemplateCol

        Return dtaTableRet

    End Function

    Function GetTemplateID(ByVal vDTable As DataTable, ByVal vOrgID As String) As String
        Dim pFinder As Finder
        Dim pDTemplate As DataTable
        Dim pDT As DataTable
        Dim blnRet As Boolean = False
        Dim intTemplateCntr As Int16
        Dim intTemplateColCntr As Int16
        Dim intExcelColCntr As Int16
        Dim strTemplateID As String = ""
        Dim blnIsColumnExist As Boolean = False

        ''Get Org Templates 
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmplte.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmplte.OrganizationID"
                    .Searchable = False
                End With
                With .Add("TemplateName", SqlDbType.VarChar)
                    .Expression = "tmplte.TemplateName"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplate tmplte "

            .SqlWhere = " WHERE tmplte.OrganizationID = " & ReadyString(vOrgID)

            '.SqlWhere = " WHERE tmplte.OrganizationID = " & ReadyString(vOrgID) &
            '    " AND ISNULL((SELECT COUNT(ID) FROM MobizLibraryPIMTemplateColumns WHERE PIMTemplateID = tmplte.ID),0) = " & vDTable.Rows.Count.ToString()

            .SqlOrder = " ORDER BY tmplte.LastUpdated DESC"

            pDTemplate = New DataTable()
            pDTemplate = .Requery

        End With

        If (pDTemplate.Rows.Count > 0) Then
            intTemplateCntr = 0
            While (intTemplateCntr < pDTemplate.Rows.Count)

                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData
                    .Distinct = True
                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "tmpltecol.ID"
                            .Searchable = False
                        End With
                        With .Add("PIMTemplateID", SqlDbType.UniqueIdentifier)
                            .Expression = "tmpltecol.PIMTemplateID"
                            .Searchable = False
                        End With
                        With .Add("ColumnName", SqlDbType.VarChar)
                            .Expression = "tmpltecol.ColumnName"
                        End With
                        With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                            .Expression = "tmpltecol.PIMFeatureID"
                            .Searchable = False
                        End With
                        With .Add("FeatureType", SqlDbType.VarChar)
                            .Expression = "tmpltecol.FeatureType"
                        End With
                    End With

                    .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltecol "

                    .SqlWhere = " WHERE tmpltecol.PIMTemplateID = " & ReadyString(pDTemplate.Rows(intTemplateCntr).Item("ID").ToString())

                    .SqlOrder = " ORDER BY tmpltecol.ColumnName ASC"

                    pDT = New DataTable()
                    pDT = .Requery

                End With

                If (pDT.Rows.Count > 0) Then
                    intTemplateColCntr = 0
                    While (intTemplateColCntr < vDTable.Rows.Count)
                        intExcelColCntr = 0
                        blnIsColumnExist = False
                        While (intExcelColCntr < pDT.Rows.Count)
                            If (vDTable.Rows(intTemplateColCntr).Item("ExcelColumn").ToString().Trim() = pDT.Rows(intExcelColCntr).Item("ColumnName").ToString().Trim()) Then
                                blnIsColumnExist = True
                            End If
                            intExcelColCntr = intExcelColCntr + 1
                        End While

                        If (Not blnIsColumnExist) Then
                            Exit While
                        End If

                        intTemplateColCntr = intTemplateColCntr + 1
                    End While

                    If (blnIsColumnExist) Then
                        strTemplateID = pDTemplate.Rows(intTemplateCntr).Item("ID").ToString()
                        Exit While
                    End If

                End If

                intTemplateCntr = intTemplateCntr + 1
            End While

        End If

        blnRet = blnIsColumnExist

        Return strTemplateID

    End Function

    Function CheckExcelColumns(ByVal vDTable As DataTable, ByVal vOrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDTReturn As DataTable
        Dim pDTemplate As DataTable
        Dim pDT As DataTable
        Dim blnRet As Boolean = False
        Dim intTemplateColCntr As Int16
        Dim intExcelColCntr As Int16
        Dim strTemplateID As String = ""
        Dim blnIsColumnExist As Boolean = False

        ''Get Org Templates 
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.ID"
                    .Searchable = False
                End With
                With .Add("PIMTemplateID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMTemplateID"
                    .Searchable = False
                End With
                With .Add("ColumnName", SqlDbType.VarChar)
                    .Expression = "tmpltecol.ColumnName"
                End With
                With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMFeatureID"
                    .Searchable = False
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "tmpltecol.FeatureType"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltecol " &
                " INNER JOIN MobizLibraryPIMTemplate tmplte ON (tmpltecol.PIMTemplateID = tmplte.ID)"

            .SqlWhere = " WHERE tmplte.OrganizationID = " & ReadyString(vOrgID)

            .SqlOrder = " ORDER BY tmpltecol.ColumnName ASC"

            pDT = New DataTable()
            pDT = .Requery

        End With

        pDTReturn = vDTable.Clone
        If (pDT.Rows.Count > 0) Then
            intExcelColCntr = 0
            While (intExcelColCntr < vDTable.Rows.Count)

                intTemplateColCntr = 0
                blnIsColumnExist = False
                While (intTemplateColCntr < pDT.Rows.Count)
                    If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = pDT.Rows(intTemplateColCntr).Item("ColumnName").ToString().Trim()) Then
                        blnIsColumnExist = True
                        Exit While
                    End If

                    intTemplateColCntr = intTemplateColCntr + 1
                End While

                If (Not blnIsColumnExist And Not (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "PO Quantity" Or vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "Is Deactivated" Or vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "EligibleForSCDPWDDiscount" Or vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "This Is An Inventory-Count Item?" Or vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "Include in mobile database" Or vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = "Beginning Balance")) Then
                    pDTReturn.Rows.Add(vDTable.Rows(intExcelColCntr).ItemArray())
                End If

                intExcelColCntr = intExcelColCntr + 1
            End While
        Else
            pDTReturn = vDTable

            Dim i As Integer = pDTReturn.Rows.Count - 1

            While i > 0
                Dim dr As DataRow = pDTReturn.Rows(i)

                If CStr2(dr("ExcelColumn")) = "Beginning Balance" Or CStr2(dr("ExcelColumn")) = "Is Deactivated" Or CStr2(dr("ExcelColumn")) = "Include in mobile database" Or CStr2(dr("ExcelColumn")) = "PO Quantity" Then
                    pDTReturn.Rows.Remove(dr)
                End If

                i -= 1
            End While

        End If

        blnRet = blnIsColumnExist

        Return pDTReturn

    End Function

    Function GetTemplateMatching(ByVal vDTable As DataTable, ByVal vOrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDTemplate As DataTable
        Dim pDT As DataTable
        Dim blnRet As Boolean = False
        Dim intTemplateCntr As Int16
        Dim intTemplateColCntr As Int16
        Dim intExcelColCntr As Int16
        Dim objRow(2) As Object

        ''Get Org Templates 
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.ID"
                    .Searchable = False
                End With
                With .Add("PIMTemplateID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMTemplateID"
                    .Searchable = False
                End With
                With .Add("ColumnName", SqlDbType.VarChar)
                    .Expression = "tmpltecol.ColumnName"
                End With
                With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMFeatureID"
                    .Searchable = False
                End With
                With .Add("Feature", SqlDbType.VarChar)
                    .Expression = "ftrname.Label"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "tmpltecol.FeatureType"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltecol " &
                " INNER JOIN MobizLibraryPIMTemplate tmplte ON (tmpltecol.PIMTemplateID = tmplte.ID) " &
                " INNER JOIN MobizLibraryPIMFeature fea ON (tmpltecol.PIMFeatureID = fea.ID) " &
                " INNER JOIN MobizLibraryCountriesLanguageLabel ftrname ON (fea.FeatureNameID = ftrname.ID) "

            .SqlWhere = " WHERE tmplte.OrganizationID = " & ReadyString(vOrgID)

            .SqlOrder = " ORDER BY tmpltecol.ColumnName ASC"

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then
            pDTemplate = New DataTable
            pDTemplate.Columns.Add("ExcelColumn")
            pDTemplate.Columns.Add("FeatureID")
            pDTemplate.Columns.Add("Feature")

            intTemplateColCntr = 0
            While (intTemplateColCntr < vDTable.Rows.Count)
                intExcelColCntr = 0
                While (intExcelColCntr < pDT.Rows.Count)
                    If (vDTable.Rows(intTemplateColCntr).Item("ExcelColumn").ToString().Trim() = pDT.Rows(intExcelColCntr).Item("ColumnName").ToString().Trim()) Then

                        objRow(0) = CStr2(vDTable.Rows(intTemplateColCntr).Item("ExcelColumn").ToString().Trim())
                        objRow(1) = CStr2(pDT.Rows(intExcelColCntr).Item("PIMFeatureID").ToString().Trim())
                        objRow(2) = CStr2(pDT.Rows(intExcelColCntr).Item("Feature").ToString().Trim())

                        pDTemplate.Rows.Add(objRow)

                    End If
                    intExcelColCntr = intExcelColCntr + 1
                End While

                intTemplateColCntr = intTemplateColCntr + 1
            End While

        End If

        Return pDTemplate

    End Function

    '' ''Function GetTemplateMatching(ByVal vDTable As DataTable, ByVal vTemplateID As String) As DataTable
    '' ''    Dim pFinder As Finder
    '' ''    Dim pDTemplate As DataTable
    '' ''    Dim pDT As DataTable
    '' ''    Dim blnRet As Boolean = False
    '' ''    Dim intTemplateCntr As Int16
    '' ''    Dim intTemplateColCntr As Int16
    '' ''    Dim intExcelColCntr As Int16
    '' ''    Dim objRow(2) As Object

    '' ''    ''Get Org Templates 
    '' ''    pFinder = New Finder
    '' ''    With pFinder
    '' ''        .ProData = Common.WebRequest.ProData
    '' ''        .Distinct = True
    '' ''        With .Fields
    '' ''            With .Add("ID", SqlDbType.UniqueIdentifier)
    '' ''                .Expression = "tmpltecol.ID"
    '' ''                .Searchable = False
    '' ''            End With
    '' ''            With .Add("PIMTemplateID", SqlDbType.UniqueIdentifier)
    '' ''                .Expression = "tmpltecol.PIMTemplateID"
    '' ''                .Searchable = False
    '' ''            End With
    '' ''            With .Add("ColumnName", SqlDbType.VarChar)
    '' ''                .Expression = "tmpltecol.ColumnName"
    '' ''            End With
    '' ''            With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
    '' ''                .Expression = "tmpltecol.PIMFeatureID"
    '' ''                .Searchable = False
    '' ''            End With
    '' ''            With .Add("Feature", SqlDbType.VarChar)
    '' ''                .Expression = "ftrname.Label"
    '' ''            End With
    '' ''            With .Add("FeatureType", SqlDbType.VarChar)
    '' ''                .Expression = "tmpltecol.FeatureType"
    '' ''            End With
    '' ''        End With

    '' ''        .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltecol " &
    '' ''            " INNER JOIN MobizLibraryPIMFeature fea ON (tmpltecol.PIMFeatureID = fea.ID) " &
    '' ''            " INNER JOIN MobizLibraryCountriesLanguageLabel ftrname ON (fea.FeatureNameID = ftrname.ID) "

    '' ''        .SqlWhere = " WHERE tmpltecol.PIMTemplateID = " & ReadyString(vTemplateID)

    '' ''        .SqlOrder = " ORDER BY tmpltecol.ColumnName ASC"

    '' ''        pDT = New DataTable()
    '' ''        pDT = .Requery

    '' ''    End With

    '' ''    If (pDT.Rows.Count > 0) Then
    '' ''        pDTemplate = New DataTable
    '' ''        pDTemplate.Columns.Add("ExcelColumn")
    '' ''        pDTemplate.Columns.Add("FeatureID")
    '' ''        pDTemplate.Columns.Add("Feature")

    '' ''        intTemplateColCntr = 0
    '' ''        While (intTemplateColCntr < vDTable.Rows.Count)
    '' ''            intExcelColCntr = 0
    '' ''            While (intExcelColCntr < pDT.Rows.Count)
    '' ''                If (vDTable.Rows(intTemplateColCntr).Item("ExcelColumn").ToString().Trim() = pDT.Rows(intExcelColCntr).Item("ColumnName").ToString().Trim()) Then

    '' ''                    objRow(0) = CStr2(vDTable.Rows(intTemplateColCntr).Item("ExcelColumn").ToString().Trim())
    '' ''                    objRow(1) = CStr2(pDT.Rows(intExcelColCntr).Item("PIMFeatureID").ToString().Trim())
    '' ''                    objRow(2) = CStr2(pDT.Rows(intExcelColCntr).Item("Feature").ToString().Trim())

    '' ''                    pDTemplate.Rows.Add(objRow)

    '' ''                End If
    '' ''                intExcelColCntr = intExcelColCntr + 1
    '' ''            End While

    '' ''            intTemplateColCntr = intTemplateColCntr + 1
    '' ''        End While

    '' ''    End If

    '' ''    Return pDTemplate

    '' ''End Function

    Function GetTemplateInfo(ByVal vID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmplte.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmplte.OrganizationID"
                    .Searchable = False
                End With
                With .Add("TemplateName", SqlDbType.VarChar)
                    .Expression = "tmplte.TemplateName"
                End With
                With .Add("ColumnName", SqlDbType.VarChar)
                    .Expression = "tmpltecol.ColumnName"
                End With
                With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMFeatureID"
                    .Searchable = False
                End With
                With .Add("PIMFeatureTable", SqlDbType.VarChar)
                    .Expression = "fea.PIMFeatureTable"
                    .Searchable = False
                End With
                With .Add("PIMFeatureColumn", SqlDbType.VarChar)
                    .Expression = "fea.PIMFeatureColumn"
                    .Searchable = False
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "tmpltecol.FeatureType"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplate tmplte " &
                " INNER JOIN MobizLibraryPIMTemplateColumns tmpltecol ON (tmpltecol.PIMTemplateID = tmplte.ID) " &
                " INNER JOIN MobizLibraryPIMFeature fea ON (tmpltecol.PIMFeatureID = fea.ID) "

            .SqlWhere = " WHERE tmplte.ID = " & ReadyString(vID)

            pDT = New DataTable()
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetTemplateColumns(ByVal vDTable As DataTable, ByVal vOrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDTemplate As DataTable
        Dim pDT As DataTable
        Dim blnRet As Boolean = False
        Dim intTemplateCntr As Int16
        Dim intTemplateColCntr As Int16
        Dim intExcelColCntr As Int16
        Dim objRow(2) As Object

        ''Get Org Templates 
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            With .Fields
                'With .Add("ID", SqlDbType.UniqueIdentifier)
                '    .Expression = "tmplte.ID"
                '    .Searchable = False
                'End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmplte.OrganizationID"
                    .Searchable = False
                End With
                'With .Add("TemplateName", SqlDbType.VarChar)
                '    .Expression = "tmplte.TemplateName"
                'End With
                With .Add("ColumnName", SqlDbType.VarChar)
                    .Expression = "tmpltecol.ColumnName"
                End With
                With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMFeatureID"
                    .Searchable = False
                End With
                With .Add("PIMFeatureTable", SqlDbType.VarChar)
                    .Expression = "fea.PIMFeatureTable"
                    .Searchable = False
                End With
                With .Add("PIMFeatureColumn", SqlDbType.VarChar)
                    .Expression = "fea.PIMFeatureColumn"
                    .Searchable = False
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "tmpltecol.FeatureType"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltecol " &
                " INNER JOIN MobizLibraryPIMTemplate tmplte ON (tmpltecol.PIMTemplateID = tmplte.ID) " &
                " INNER JOIN MobizLibraryPIMFeature fea ON (tmpltecol.PIMFeatureID = fea.ID) "
                

            .SqlWhere = " WHERE tmplte.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
                " UNION " & vbCrLf &
                "Select " & vbCrLf &
                "OrgID as OrganizationID, " & vbCrLf &
                "PriceLevel as ColumnName, " & vbCrLf &
                "lvl.ID  as PIMFeatureID, " & vbCrLf &
                "'MobizLibraryPIMPriceLevel' as PIMFeatureTable, " & vbCrLf &
                "PriceLevel as PIMFeatureColumn, " & vbCrLf &
                "'Textbox (Decimal)' as FeatureType " & vbCrLf &
                "FROM MobizLibraryPIMPriceLevel as lvl " & vbCrLf &
                "WHERE (lvl.IsActive = 1 AND lvl.OrgID IS NULL OR lvl.OrgID =  " & ReadyString(vOrgID) & " ) " & vbCrLf &
                "AND lvl.ID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000', " & vbCrLf &
                "'01000012-0003-0000-0000-000000000000', '01000012-0004-0000-0000-000000000000', '01000012-0005-0000-0000-000000000000', " & vbCrLf &
                "'eef716c6-b290-4b28-b2a6-0a4f52cf4989')"

            '.SqlOrder = " ORDER BY tmpltecol.ColumnName ASC"

            pDT = New DataTable()
            pDT = .Requery

        End With

        pDTemplate = pDT.Clone

        If (pDT.Rows.Count > 0) Then

            intTemplateColCntr = 0
            While (intTemplateColCntr < vDTable.Rows.Count)
                intExcelColCntr = 0
                While (intExcelColCntr < pDT.Rows.Count)
                    If (vDTable.Rows(intTemplateColCntr).Item("ExcelColumn").ToString().ToUpper().Trim() = pDT.Rows(intExcelColCntr).Item("ColumnName").ToString().ToUpper().Trim()) Then

                        pDTemplate.Rows.Add(pDT.Rows(intExcelColCntr).ItemArray())

                    End If
                    intExcelColCntr = intExcelColCntr + 1
                End While

                intTemplateColCntr = intTemplateColCntr + 1
            End While

        End If

        Return pDTemplate

    End Function

    Function CheckUniqueTemplateName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmplte.ID"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMTemplate tmplte "

            .SqlWhere = "WHERE tmplte.OrganizationID " & IIf(OrganizationID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(OrganizationID)) &
                        " AND tmplte.TemplateName = " & ReadyString(TemplateName)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SavePIMTemplate() As String
        Dim p As New InsertUpdate("MobizLibraryPIMTemplate")
        Dim pSql As String
        Dim strIDReturn As String

        Try
            p("OrganizationID") = ReadyString(OrganizationID)
            p("TemplateName") = ReadyString(TemplateName)
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "getdate()"

            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Template Column List
            For Each objPIMTemplate As Object In PIMTemplateColList

                p = New InsertUpdate("MobizLibraryPIMTemplateColumns")
                p("PIMTemplateID") = ReadyString(strIDReturn)
                p("ColumnName") = IIf(objPIMTemplate("ColumnName").ToString().Trim().Length = 0, "NULL", ReadyString(objPIMTemplate("ColumnName").ToString().Trim()))
                p("PIMFeatureID") = ReadyString(IIf(objPIMTemplate("PIMFeatureID").ToString().Trim().Length > 0, objPIMTemplate("PIMFeatureID").ToString().Trim(), ""))
                p("FeatureType") = ReadyString(IIf(objPIMTemplate("FeatureType").ToString().Trim().Length > 0, objPIMTemplate("FeatureType").ToString().Trim(), ""))
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            Next objPIMTemplate

            If (Not SavePIMTemplateAKA()) Then
                Common.WebRequest.ProData.RollbackTransaction()
                strIDReturn = ""
            End If

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function SavePIMTemplateAKA() As Boolean
        Dim blnReturn As Boolean = False

        Try
            Common.WebRequest.ProData.BeginTransaction()
            '''Save AKA
            For Each objPIMTemplateAKA As Object In PIMTemplateColAKAList

                '''Unit Measure
                If (objPIMTemplateAKA("PIMFeatureID").ToString().Trim().ToUpper() = "C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9") Then
                    If (objPIMTemplateAKA("PIMFeatureDataID").ToString().Trim().Length > 0) Then
                        SavePIMMeasureAKA(objPIMTemplateAKA("PIMFeatureDataID").ToString().Trim(), objPIMTemplateAKA("ColumnData").ToString().Trim())
                    End If
                End If

            Next objPIMTemplateAKA

            blnReturn = True
            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return blnReturn

    End Function

    Function SavePIMTemplateData(ByVal DTemplate As DataTable, ByVal DTable As DataTable, ByVal vOrgID As String, ByVal vDBaseName As String, ByVal vPeopleUserID As String) As Boolean
        Dim p As New InsertUpdate("MobizLibraryPIMTemplate")
        Dim p2 As New InsertUpdate("MobizLibraryPIMTemplate")
        Dim blnRet As Boolean = False
        Dim pSql As String
        Dim intCntr As Integer
        Dim intTemplateColCntr As Integer
        Dim strExcelColName As String
        Dim strProductSearchString As String
        Dim strProductID As String
        Dim strMeasurementID As String
        Dim strOrgPriceLevelID As String
        Dim clsOrganization As New Organization
        Dim dtaOrgBrand As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDTPriceLevel As DataTable
        Dim pDTUnitMasure As DataTable
        Dim clsPIMMeasure As New PIMMeasure
        Dim strUnitMeasureID As String

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
                intCntr = 0
                While (intCntr < DTable.Rows.Count)
                    Dim dtaRow As DataRow
                    dtaRow = DTable.Rows(intCntr)

                    mBusiness.ProData.BeginTransaction()

                    p = New InsertUpdate("MobizLibraryPIMProduct")

                    p("PublishedByID") = ReadyString(vOrgID)
                    p("UploadedByID") = ReadyString(vOrgID)
                    p("IsBrandOwner") = IsOrgBrandOwner(dtaRow("Brand").ToString(), dtaOrgBrand)

                    '''Find Short Product Name Column fixed Feature ID DAFBB390-EA84-4EC8-948B-74B028CBE996
                    strExcelColName = GetExcelColumn("DAFBB390-EA84-4EC8-948B-74B028CBE996", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("ProductName") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find Short Product Name Column fixed Feature ID 7B3524D2-EFFC-4921-B752-3CF7D84434D6
                    strExcelColName = GetExcelColumn("7B3524D2-EFFC-4921-B752-3CF7D84434D6", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("Brand") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find Division Description Column fixed Feature ID EBBE5A38-5A5A-4328-8EF2-2C832680B125
                    strExcelColName = GetExcelColumn("EBBE5A38-5A5A-4328-8EF2-2C832680B125", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("DivisionDescription") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find Product Group No (Style Number - Color Code) Column fixed Feature ID E4FEFAF4-EBC5-47EA-BEA3-2DFBCB5DC107
                    strExcelColName = GetExcelColumn("E4FEFAF4-EBC5-47EA-BEA3-2DFBCB5DC107", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("ProductGroupNo") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find Model Column fixed Feature ID 7D8D902D-5710-400A-A39A-FE4495409B5D
                    strExcelColName = GetExcelColumn("7D8D902D-5710-400A-A39A-FE4495409B5D", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("ModelNo") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find SRP Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C99
                    strExcelColName = GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C99", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("SRP") = Convert.ToDouble(dtaRow(strExcelColName).ToString())
                    Else
                        p("SRP") = 0
                    End If

                    '''Find Marketing Text Column fixed Feature ID D1511CBE-FBAF-4F97-9A61-5E2684D7098F
                    strExcelColName = GetExcelColumn("D1511CBE-FBAF-4F97-9A61-5E2684D7098F", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("MarketingText") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find Warranty Column fixed Feature ID B16B5DDA-6660-4E94-8FCE-DB00C8AFC5B7
                    strExcelColName = GetExcelColumn("B16B5DDA-6660-4E94-8FCE-DB00C8AFC5B7", DTemplate)
                    If (strExcelColName.Trim().Length > 0) Then
                        p("Warranty") = ReadyString(dtaRow(strExcelColName).ToString())
                    End If

                    '''Find Product No Column fixed Feature ID F9CB775D-5E60-4D92-98A3-2A8444D5F32A
                    strExcelColName = GetExcelColumn("F9CB775D-5E60-4D92-98A3-2A8444D5F32A", DTemplate)
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
                        End With

                        .SqlFrom = "FROM MobizLibraryPIMProduct prd "

                        .SqlWhere = "WHERE prd.ProductNo = " & ReadyString(dtaRow(strExcelColName).ToString()) &
                            " AND prd.UploadedByID = " & ReadyString(vOrgID)

                        .SqlOrder = " ORDER BY prd.IsBrandOwner DESC "

                        pDT = .Requery
                    End With

                    If (pDT.Rows.Count > 0) Then
                        strProductID = pDT.Rows(0).Item("ID").ToString().Trim()
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

                    If (strProductID.Trim().Length <= 0) Then
                        strProductID = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strProductID)

                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(strProductID))
                    End If

                    mBusiness.ProData.ExecuteSQL(pSql)

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

                    '''Insert into MobizLibraryPIMProductMeasurement
                    '''Find Packing Size Unit Column fixed Feature ID C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9
                    strExcelColName = GetExcelColumn("C91A5759-BB33-4F08-AAC1-FC7B1F94E7B9", DTemplate)
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

                                .SqlFrom = "FROM MobizLibraryPIMProductMeasurement "

                                .SqlWhere = " WHERE ProductID = " & ReadyString(strProductID) &
                                           " AND MeasurementLevel = 1"
                                pDT = New DataTable()
                                pDT = .Requery

                            End With

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
                            p("MeasurementLevel") = 1
                            ' ''p("UnitMeasureID") = "(SELECT TOP 1 mea.[ID] FROM MobizLibraryPIMMeasure mea " &
                            ' ''                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (mea.[MeasureNameID] = lbl.ID) " &
                            ' ''                    " INNER JOIN MobizLibraryPIMMeasureSign sgn ON (mea.[SignID] = sgn.ID) " &
                            ' ''                    " WHERE lbl.Label = " & ReadyString(dtaRow(strExcelColName).ToString().Trim()) &
                            ' ''                    " OR sgn.Sign = " & ReadyString(dtaRow(strExcelColName).ToString().Trim()) & ")"

                            p("UnitMeasureID") = ReadyString(strUnitMeasureID)

                            '''Find Packing Size Qty Column fixed Feature ID E1059BD9-B365-478F-A443-F465B604D251
                            strExcelColName = GetExcelColumn("E1059BD9-B365-478F-A443-F465B604D251", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                    p("Quantity") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                Else
                                    p("Quantity") = 1
                                End If
                            End If

                            '''Find Content Unit Column fixed Feature ID DD4563D3-9FC5-41E4-88EB-12B45279820E
                            strExcelColName = GetExcelColumn("DD4563D3-9FC5-41E4-88EB-12B45279820E", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                p("ContentUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                            End If

                            '''Find Content Qty Column fixed Feature ID 443C66E2-F7A3-4C8B-A287-3D5E2D1945F4
                            strExcelColName = GetExcelColumn("443C66E2-F7A3-4C8B-A287-3D5E2D1945F4", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                    p("ContentQty") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                End If
                            End If

                            '''Find Product Dimension Length Column fixed Feature ID 682E19BD-C47A-4760-B120-3DD931D5BB49
                            strExcelColName = GetExcelColumn("682E19BD-C47A-4760-B120-3DD931D5BB49", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                    p("ProdDimensionLength") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                End If
                            End If

                            '''Find Product Dimension Unit Column fixed Feature ID 9B32E0BC-E77E-4724-9BBC-C0C0989EF95B
                            strExcelColName = GetExcelColumn("9B32E0BC-E77E-4724-9BBC-C0C0989EF95B", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                p("ProdDimensionLengthUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                            End If

                            '''Find Product Dimension Width Column fixed Feature ID E35110CC-A508-463D-B440-DAA3B4CEF792
                            strExcelColName = GetExcelColumn("E35110CC-A508-463D-B440-DAA3B4CEF792", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                    p("ProdDimensionWidth") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                End If
                            End If

                            '''Find Product Dimension Width Unit Column fixed Feature ID 97842368-D382-4D66-A00C-6C6F5250FE0F
                            strExcelColName = GetExcelColumn("97842368-D382-4D66-A00C-6C6F5250FE0F", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                p("ProdDimensionWidthUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                            End If

                            '''Find Product Dimension Height Column fixed Feature ID AAFAE8E0-4ABA-448A-A89E-CC5F2FCF0942
                            strExcelColName = GetExcelColumn("AAFAE8E0-4ABA-448A-A89E-CC5F2FCF0942", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                    p("ProdDimensionHeight") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                End If
                            End If

                            '''Find Product Dimension Height Unit Column fixed Feature ID 530EFD37-3303-4903-B939-9B3FF19564CC
                            strExcelColName = GetExcelColumn("530EFD37-3303-4903-B939-9B3FF19564CC", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                p("ProdDimensionHeightUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                            End If

                            '''Find Product Weight Column fixed Feature ID E1DE6123-65ED-422F-B275-C8C2A0FBCA44
                            strExcelColName = GetExcelColumn("E1DE6123-65ED-422F-B275-C8C2A0FBCA44", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                If (IsNumeric(dtaRow(strExcelColName).ToString().Trim())) Then
                                    p("ProductWeight") = Convert.ToDouble(dtaRow(strExcelColName).ToString().Trim())
                                End If
                            End If

                            '''Find Product Weight Unit Column fixed Feature ID 770319EA-1948-4FE7-B4F1-CC669E947D53
                            strExcelColName = GetExcelColumn("770319EA-1948-4FE7-B4F1-CC669E947D53", DTemplate)
                            If (strExcelColName.Trim().Length > 0) Then
                                p("ProductWeightUnit") = ReadyString(dtaRow(strExcelColName).ToString().Trim())
                            End If

                            p("Discount") = 0
                            p("UserID") = ReadyString(vPeopleUserID)
                            p("LastUpdated") = "getdate()"

                            If (pDT.Rows.Count > 0) Then
                                strMeasurementID = pDT.Rows(0).Item("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strMeasurementID))
                            Else
                                strMeasurementID = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strMeasurementID)
                                pSql = p.GetInsert()
                            End If

                            mBusiness.ProData.ExecuteSQL(pSql)

                            '''Find UPC (Barcode) Column fixed Feature ID A4B843A6-CBB9-4429-8BC7-2D571E7AB48C
                            strExcelColName = GetExcelColumn("A4B843A6-CBB9-4429-8BC7-2D571E7AB48C", DTemplate)
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

                    '''Insert into MobizLibraryPIMProductPriceLevel
                    '''Find Suggested Retail Price Column fixed Feature ID C5BD7B11-369D-404A-AD18-C53566744C99
                    strExcelColName = GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C99", DTemplate)
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
                    strExcelColName = GetExcelColumn("C5BD7B11-369D-404A-AD18-C53566744C90", DTemplate)
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

                    '''Find Shipping Cost Column fixed Feature ID 2254751E-E1FC-4704-B494-A39FDBA2A245
                    strExcelColName = GetExcelColumn("2254751E-E1FC-4704-B494-A39FDBA2A245", DTemplate)
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
                    strExcelColName = GetExcelColumn("E531191E-203E-41CC-BA9B-C46A27D10CD8", DTemplate)
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
                    strExcelColName = GetExcelColumn("1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", DTemplate)
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
                            strExcelColName = GetExcelColumn(CStr2(pDTPriceLevel.Rows(i).Item("ID")), DTemplate)
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
                                            " AND PriceLevelID = " & ReadyString(CStr2(pDTPriceLevel.Rows(i).Item("ID")))

                                        pDT = New DataTable()
                                        pDT = .Requery

                                    End With

                                    p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                                    p("OrganizationID") = ReadyString(vOrgID)
                                    p("ProductID") = ReadyString(strProductID)
                                    p("PriceLevelID") = ReadyString(CStr2(pDTPriceLevel.Rows(i).Item("ID")))

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
                        Next
                    End If
                    '''End Insert into MobizLibraryPIMProductPriceLevel----------------

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

                    'pSql = p2.GetInsert()
                    'Common.WebRequest.ProData.ExecuteSQL(pSql)

                    mBusiness.ProData.CommitTransaction()

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

            End If

            blnRet = True

        Catch ex As Exception
            mBusiness.ProData.RollbackTransaction()
        End Try

        Return blnRet

    End Function

    Function SavePIMTemplateUnMatchedCol(ByVal DTable As DataTable) As String
        'Dim p As New InsertUpdate("MobizLibraryPIMTemplate")
        'Dim pSql As String
        Dim strIDReturn As String

        'Try
        '    p("OrganizationID") = ReadyString(OrganizationID)
        '    p("TemplateName") = ReadyString(TemplateName)
        '    p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        '    p("LastUpdated") = "getdate()"

        '    strIDReturn = Guid.NewGuid.ToString()
        '    p("ID") = ReadyString(strIDReturn)
        '    pSql = p.GetInsert()

        '    Common.WebRequest.ProData.BeginTransaction()
        '    Common.WebRequest.ProData.ExecuteSQL(pSql)

        '    '''Save Trade Name List
        '    For Each objPIMTemplate As Object In PIMTemplateColList

        '        p = New InsertUpdate("MobizLibraryPIMTemplateColumns")
        '        p("PIMTemplateID") = ReadyString(strIDReturn)
        '        p("ColumnName") = IIf(objPIMTemplate("ColumnName").ToString().Trim().Length = 0, "NULL", ReadyString(objPIMTemplate("ColumnName").ToString().Trim()))
        '        p("PIMFeatureID") = ReadyString(IIf(objPIMTemplate("PIMFeatureID").ToString().Trim().Length > 0, objPIMTemplate("PIMFeatureID").ToString().Trim(), ""))
        '        p("FeatureType") = ReadyString(IIf(objPIMTemplate("FeatureType").ToString().Trim().Length > 0, objPIMTemplate("FeatureType").ToString().Trim(), ""))
        '        p("ID") = ReadyString(Guid.NewGuid.ToString())
        '        pSql = p.GetInsert()

        '        Common.WebRequest.ProData.ExecuteSQL(pSql)

        '    Next objPIMTemplate

        '    Common.WebRequest.ProData.CommitTransaction()

        'Catch ex As Exception
        '    Common.WebRequest.ProData.RollbackTransaction()
        'End Try

        Return strIDReturn

    End Function

    Function SavePIMMeasureAKA(ByVal vID As String, ByVal vMeasurementAKA As String) As String
        Dim p As New InsertUpdate("MobizLibraryPIMMeasure")
        Dim pSql As String
        Dim strIDReturn As String

        Try
            p("MeasurementAKA") = "ISNULL(NULLIF(LTRIM(RTRIM(ISNULL(MeasurementAKA,''))) + ', ',', '), '') + " & ReadyString(vMeasurementAKA)
            p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            p("LastUpdated") = "getdate()"

            strIDReturn = vID
            pSql = p.GetUpdate(" ID = " & ReadyString(vID))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

        Catch ex As Exception
        End Try

        Return strIDReturn

    End Function

    Function GetExcelColumn(ByVal vFeatureID As String, ByVal vDTable As DataTable) As String
        Dim strRet As String = ""
        Dim dtaRow As DataRow
        Dim dtaTemplateCols() As DataRow
        dtaTemplateCols = vDTable.Select("PIMFeatureID = '" & vFeatureID.Trim().ToUpper() & "'")

        For Each dtaRow In dtaTemplateCols
            strRet = dtaRow("ColumnName").ToString().Trim()
        Next

        Return strRet

    End Function

    Function IsOrgBrandOwner(ByVal vBrand As String, ByVal vDTable As DataTable) As String
        Dim strRet As String = "0"
        Dim dtaRow As DataRow
        Dim dtaBrandList() As DataRow
        dtaBrandList = vDTable.Select("BrandName = " & ReadyString(vBrand))

        If (dtaBrandList.Count > 0) Then
            strRet = "1"
        End If

        Return strRet

    End Function

    Function CheckProductExist(ByVal vProductNo As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProduct prd "

            .SqlWhere = "WHERE prd.ProductNo = " & ReadyString(vProductNo)

            pDT = .Requery
        End With

        If (pDT.Rows.Count > 0) Then
            strRet = pDT.Rows(0).Item("ID").ToString().Trim()
        End If

        Return strRet

    End Function

    Function CheckProductExist(ByVal vProductNo As String, ByVal vOrgID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProduct prd "

            .SqlWhere = "WHERE prd.ProductNo = " & ReadyString(vProductNo) &
                " AND prd.UploadedByID = " & ReadyString(vOrgID)

            .SqlOrder = " ORDER BY prd.IsBrandOwner DESC "

            pDT = .Requery
        End With

        If (pDT.Rows.Count > 0) Then
            strRet = pDT.Rows(0).Item("ID").ToString().Trim()
        End If

        Return strRet

    End Function

    Function CheckProductNoAndNameExistByOrg(ByVal vProductNo As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As Boolean

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
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
            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct prd INNER JOIN LibraryOrganizationPIM orgPIM ON prd.ID = orgPIM.ProductID "

            .SqlWhere = " WHERE prd.ProductNo = " & ReadyString(vProductNo)

            pDT = .Requery
        End With


        Return pDT

    End Function

    Function CheckProductFeatureExist(ByVal ProductFeatureTable As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdfea.ID"
                End With
            End With

            .SqlFrom = "FROM " & ProductFeatureTable & " prdfea "

            .SqlWhere = "WHERE prdfea.ProductID = " & ReadyString(vProductID)

            pDT = .Requery
        End With

        If (pDT.Rows.Count > 0) Then
            strRet = pDT.Rows(0).Item("ID").ToString().Trim()
        End If

        Return strRet

    End Function

    Function GetMeasurement(ByVal type As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With

                With .Add("MeasureNameID", SqlDbType.VarChar)
                    .Expression = "msr.MeasureNameID" & vbCrLf
                End With

                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label" & vbCrLf
                End With

                With .Add("SignID", SqlDbType.VarChar)
                    .Expression = "msr.SignID" & vbCrLf
                End With

                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')" & vbCrLf
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasure msr" & vbCrLf &
                        "   INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                        "   LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID) "

            If type = "PackingSizeUnit" Then
                .SqlWhere = "WHERE IsStandardArea <> 1 AND IsStandardVolCapacity <> 1 " & vbCrLf

            ElseIf type = "ContentUnit" Then
                .SqlWhere = "WHERE IsStandardArea = 1 OR IsStandardVolCapacity = 1 OR IsStandardVolDry = 1 OR IsStandardWeight = 1 " & vbCrLf &
                        "OR IsStandardCooking = 1 OR IsStandardTime = 1 OR IsStandardElectricity = 1 OR IsStandardLength = 1 "

            ElseIf type = "DimensionUnit" Then
                .SqlWhere = "WHERE IsStandardLength = 1 " & vbCrLf

            ElseIf type = "WeightUnit" Then
                .SqlWhere = "WHERE IsStandardWeight = 1 " & vbCrLf
            End If


            .SqlOrder = "ORDER BY lbl.Label ASC"

            pDT = .Requery
        End With

        Return pDT
    End Function
End Class
