Public Class LibraryProductMap

    Dim mProData As ProData
    Dim mParent As Business

    Dim mDtShipperRoutes As DataTable

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ImportExcel(vOrgID As String, vFowstID As String, vUserID As String, vFilename As String,
        ByRef oError As String, ByRef oWarning As String) As Boolean

        oError = Nothing
        oWarning = Nothing

        Dim pHeaders = New String() {
            "Third Party Product Number",
            "Third Party Product Name",
            "MPOS Product Number"
        }
        Dim pDT = ExcelToTable(vFilename, pHeaders, oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        Dim pMap = GetOrCreateMapByFowstID(vFowstID)

        For Each pDR As DataRow In pDT.Rows
            Dim pWarning As String = Nothing
            SaveProduct(vMapID:=CStr2(pMap("ProductMapID")),
                vThirdPartyProductNumber:=CStr2(pDR("Third Party Product Number")),
                vThirdPartyProductName:=CStr2(pDR("Third Party Product Name")),
                vProductNumber:=CStr2(pDR("MPOS Product Number")),
                vUserID:=vUserID,
                oError:=pWarning)
            If String.IsNullOrWhiteSpace(pWarning) = False Then
                oWarning &= "Warning in row " & pDR("Spreadsheet Row Number") & ": " & pWarning & vbCrLf
                If oWarning.Length > 5000 Then
                    Return False
                End If
            End If
        Next

        Return True

    End Function

    Sub SaveProduct(vMapID As String, vThirdPartyProductNumber As String,
        vThirdPartyProductName As String,
        vProductNumber As String,
        vUserID As String,
        ByRef oError As String)

        oError = Nothing

        If String.IsNullOrWhiteSpace(vThirdPartyProductNumber) Then
            oError = "Third Party Product Number is required."
            Return
        End If
        vThirdPartyProductNumber = vThirdPartyProductNumber.Trim()

        If String.IsNullOrWhiteSpace(vThirdPartyProductName) Then
            oError = "Third Party Product Name is required."
            Return
        End If
        vThirdPartyProductName = vThirdPartyProductName.Trim()

        If String.IsNullOrWhiteSpace(vProductNumber) Then
            oError = "MPOS Product Number is required."
            Return
        End If
        vProductNumber = vProductNumber.Trim()

        Dim pDrProduct = mParent.LibraryStock.ByNumber(vProductNumber)
        If pDrProduct Is Nothing Then
            oError = "MPOS product number " & vProductNumber & " does not exist."
            Return
        End If

        Dim pIU = New InsertUpdate("LibraryProductMapProducts")
        pIU("ProductName") = ReadyString(vThirdPartyProductName)
        pIU("ProductID") = ReadyString(pDrProduct("ID"))
        pIU("UserID") = ReadyString(vUserID)
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetUpdate("ProductMapID = " & ReadyString(vMapID) & vbCrLf &
            " AND ProductNumber = " & ReadyString(vThirdPartyProductNumber))
        Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
        If pAffectedRecords = 0 Then
            pIU("ID") = "NEWID()"
            pIU("ProductMapID") = ReadyString(vMapID)
            pIU("ProductNumber") = ReadyString(vThirdPartyProductNumber)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Function GetOrCreateMapByFowstID(vFowstID As String) As DataRow

        Dim pOut = GetMapByFowstID(vFowstID)
        If pOut Is Nothing Then
            CreateMap(vFowstID)
            pOut = GetMapByFowstID(vFowstID)
        End If

        Return pOut

    End Function

    Sub CreateMap(vFowstID As String)

        Dim pIU = New InsertUpdate("LibraryProductMapFowsts")
        pIU("ID") = "NEWID()"
        pIU("ProductMapID") = "NEWID()"
        pIU("FowstID") = ReadyString(vFowstID)
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Function GetMapByFowstID(vFowstID As String) As DataRow

        Dim pSql = "SELECT ProductMapID" & vbCrLf &
            " FROM LibraryProductMapFowsts" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Function ThirdPartyProductNumberToProductID(vFowstID As String, vThirdPartyProductNumber As String) As String

        Dim pSql = "SELECT LibraryProductMapProducts.ProductID" & vbCrLf &
            " FROM LibraryProductMapProducts" & vbCrLf &
            " JOIN LibraryProductMapFowsts ON LibraryProductMapFowsts.ProductMapID = LibraryProductMapProducts.ProductMapID" & vbCrLf &
            " WHERE LibraryProductMapProducts.ProductNumber = " & ReadyString(vThirdPartyProductNumber.Trim()) & vbCrLf &
            " AND LibraryProductMapFowsts.FowstID = " & ReadyString(vFowstID)
        Dim pOut = CStr2(mProData.ExecuteScalar(pSql))
        Return pOut
    End Function

    Public Function CreateExcel(vFowstID As String) As String

        Dim pSql = " SELECT LibraryProductMapProducts.ProductNumber" & vbCrLf &
            " ,LibraryProductMapProducts.ProductName" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName AS MobizProductName" & vbCrLf &
            " FROM LibraryProductMapProducts" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryProductMapProducts.ProductID" & vbCrLf &
            " JOIN LibraryProductMapFowsts ON LibraryProductMapFowsts.ProductMapID = LibraryProductMapProducts.ProductMapID" & vbCrLf &
            " WHERE LibraryProductMapFowsts.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " ORDER BY LibraryProductMapProducts.ProductNumber" & vbCrLf &
            " ,LibraryProductMapProducts.ProductName"
        Dim pDT = mProData.Execute(pSql)
        pDT.Columns("ProductNumber").ColumnName = "Third Party Product Number"
        pDT.Columns("ProductName").ColumnName = "Third Party Product Name"
        pDT.Columns("ProductNo").ColumnName = "MPOS Product Number"
        pDT.Columns("MobizProductName").ColumnName = "MPOS Product Name"
        Dim pFilename As String
        pFilename = Common.ServerPath & "temp\documents\"
        Dim pOut = "ProductMapping " & DateTimeStamp() & ".xlsx"
        pFilename &= pOut
        CreateFolder(pFilename)
        TableToExcelFile(pDT, "", "", pFilename)
        Return pOut

    End Function
End Class
