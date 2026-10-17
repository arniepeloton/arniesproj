Public Class LibraryOrganizationFOWST

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ByID(vID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
            " ,ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '')" & vbCrLf &
            " + ',' + ISNULL(MobizLibraryOrganizationFOWST.Street, '')" & vbCrLf &
            " + ',' + ISNULL(MobizLibraryCities.Name, '') AS Address" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " LEFT JOIN MobizLibraryCities ON MobizLibraryCities.ID = MobizLibraryOrganizationFOWST.FowstCityID" & vbCrLf &
            " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetOrCreateByStoreID(vOrgID As String, vStoreID As String, vName As String) As String

        Dim pDR As DataRow
        Dim pID As String

        pDR = GetByStoreID(vOrgID, vStoreID)
        If pDR Is Nothing Then
            pID = CreateByStoreID(vOrgID, vStoreID, vName)
        Else
            pID = CStr2(pDR("ID"))
        End If

        Return pID

    End Function

    Public Function GetByStoreID(vOrgID As String, vStoreID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND (StoreID = " & ReadyString(vStoreID) & vbCrLf &
            " OR NetSuiteID = " & ReadyString(vStoreID) & ")" & vbCrLf
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetByCode(vOrgID As String, vCode As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND Code = " & ReadyString(vCode)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vOrgID"></param>
    ''' <param name="vStoreID">this will also set the NetSuiteID</param>
    ''' <param name="vName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function CreateByStoreID(vOrgID As String, vStoreID As String, vName As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid.ToString
        pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pIU("ID") = ReadyString(pID)
        pIU("OrganizationID") = ReadyString(vOrgID)
        pIU("Code") = ReadyString(vStoreID)
        pIU("StoreID") = ReadyString(vStoreID)
        pIU("NetSuiteID") = ReadyString(vStoreID)
        pIU("Name") = ReadyString(vName)
        pIU("StreetNumber") = "''"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    ''' <summary>
    ''' Darlington-Trail NetSuite format
    ''' </summary>
    ''' <param name="vOrgID"></param>
    ''' <param name="vFilename"></param>
    ''' <remarks></remarks>
    Public Sub CreateFromExcel(vOrgID As String, vFilename As String)

        Dim pDT As DataTable
        Dim pError As String
        Dim pExcelRow As ExcelRow
        Dim pExcelRows As List(Of ExcelRow)

        pDT = ExcelToTable(vFilename, New String() {"Internal ID", "Name"}, pError)
        If String.IsNullOrWhiteSpace(pError) Then
            pExcelRows = New List(Of ExcelRow)
            For Each pDR As DataRow In pDT.Rows
                pExcelRow = New ExcelRow
                pExcelRow.InternalID = CStr2(pDR("Internal ID"))
                pExcelRow.Name = CStr2(pDR("Name"))
                pExcelRow.ExcelRowNumber = CInt2(pDR("Spreadsheet Row Number"))
                pExcelRows.Add(pExcelRow)
            Next pDR

            CreateFromExcelRows(vOrgID, pExcelRows)
        End If

    End Sub

    Public Sub CreateFromExcelRows(vOrgID As String, vRows As List(Of ExcelRow))

        If vRows.Count = 0 Then
            Exit Sub
        End If

        For Each pRow As ExcelRow In vRows
            GetOrCreateByStoreID(vOrgID, pRow.InternalID, pRow.Name)
        Next pRow

        mParent.SystemDefaults.UpdateTableLastUpdated("MobizLibraryOrganization")

    End Sub

    Public Function GetByOrgID(vOrgID As String, Optional vWithDevice As Boolean = False) As DataTable

        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            ",Name" & vbCrLf &
            ",ConsolidateOrderSlipPrinting" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " --AND FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b' -- exclude FOWST for Mobiz Project" & vbCrLf
        If vWithDevice Then
            pSql &= " AND EXISTS(SELECT *" & vbCrLf &
                " FROM LibraryDeviceFowst" & vbCrLf &
                " WHERE FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                ")" & vbCrLf
        End If
        pSql &= " ORDER BY Name"
        Return mProData.Execute(pSql)

    End Function

    Public Function GetFirstByOrgID(vOrgID As String) As DataRow

        Dim pDT As DataTable

        pDT = GetByOrgID(vOrgID)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function CreateFromNetSuite(vName As String, vOrgID As String, vNetSuiteID As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE Name = " & ReadyString(vName) & vbCrLf &
            " AND OrganizationID = " & ReadyString(vOrgID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

        pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("Name") = ReadyString(vName)
        pIU("OrganizationID") = ReadyString(vOrgID)
        pIU("NetSuiteID") = ReadyString(vNetSuiteID)
        pIU("StreetNumber") = "''"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function GetOrCreateBin(vName As String, vFowstID As String, vNetSuiteID As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryFowstBin" & vbCrLf &
            " WHERE Name = " & ReadyString(vName) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

        pIU = New InsertUpdate("LibraryFowstBin")
        pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("Name") = ReadyString(vName)
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("NetSuiteID") = ReadyString(vNetSuiteID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Class ExcelRow
        Public InternalID As String
        Public Name As String
        ''' <summary>
        ''' 1-based
        ''' </summary>
        ''' <remarks></remarks>
        Public ExcelRowNumber As Integer
    End Class

    Public Function GetOrAddByStreetNumber(vOrgID As String, vStreetNumber As String,
        Optional vThirdPartySystemID As String = Nothing) As String

        Dim pID As String

        pID = GetByStreetNumber(vOrgID, vStreetNumber)
        If String.IsNullOrWhiteSpace(pID) = False Then
            Return pID
        End If
        pID = AddStreetNumber(vOrgID, vStreetNumber, vThirdPartySystemID)

        Return pID

    End Function

    Public Function GetByStreetNumber(vOrgID As String, vStreetNumber As String) As String

        Dim pSql As String
        Dim pOut As String

        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrgID) &
            " AND StreetNumber = " & ReadyString(vStreetNumber)
        pOut = CStr2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function AddStreetNumber(vOrgID As String, vStreetNumber As String,
        Optional vThirdPartySystemID As String = Nothing) As String

        Dim pID As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("OrganizationID") = ReadyString(vOrgID)
        pIU("StreetNumber") = ReadyString(vStreetNumber.Trim())
        If String.IsNullOrWhiteSpace(vThirdPartySystemID) = False Then
            pIU("ThirdPartySystemID") = ReadyString(vThirdPartySystemID)
        End If
        pIU("IsActive") = 1
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function HasDevice(vFowstID As String, vDeviceID As String) As Boolean

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT TOP 1 ID" & vbCrLf &
            " FROM LibraryDeviceFowst" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR IsNot Nothing

    End Function

    Public Function AddDevice(vFowstID As String, vDeviceID As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        If HasDevice(vFowstID, vDeviceID) Then
            Return False
        End If

        pIU = New InsertUpdate("LibraryDeviceFowst")
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("DeviceID") = ReadyString(vDeviceID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return True

    End Function

    Public Function AddOrUpdateFromMainAddress(vOrgID As String, vBuildingNum As String,
        vStreet As String, vCityID As String, vCountryID As String, Optional vAddressRequired As Boolean = True) As String

        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pDrFowst As DataRow
        Dim pFowstID As String

        pDrFowst = GetMainAddress(vOrgID)
        If pDrFowst IsNot Nothing Then
            pFowstID = CStr2(pDrFowst("ID"))
        End If

        If String.IsNullOrWhiteSpace(vBuildingNum) _
            AndAlso String.IsNullOrWhiteSpace(vStreet) _
            AndAlso String.IsNullOrWhiteSpace(vCityID) _
            AndAlso String.IsNullOrWhiteSpace(vCountryID) _
            AndAlso vAddressRequired Then

            Return pFowstID
        End If

        pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pIU("IsMainAddress") = 1

        If String.IsNullOrWhiteSpace(vBuildingNum) Then
            pIU("StreetNumber") = "''"
        Else
            pIU("StreetNumber") = ReadyString(vBuildingNum)
        End If

        If String.IsNullOrWhiteSpace(vStreet) Then
            pIU("Street") = "''"
        Else
            pIU("Street") = ReadyString(vStreet)
        End If

        If String.IsNullOrWhiteSpace(vCityID) Then
            pIU("FowstCityID") = "NULL"
        Else
            pIU("FowstCityID") = ReadyString(vCityID)
        End If

        If String.IsNullOrWhiteSpace(vCountryID) Then
            pIU("FowstCountryID") = "NULL"
        Else
            pIU("FowstCountryID") = ReadyString(vCountryID)
        End If

        If String.IsNullOrWhiteSpace(pFowstID) Then
            pFowstID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pFowstID)
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("Name") = ReadyString("Main Location")
            pSql = pIU.GetInsert()
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(pFowstID))
        End If
        mProData.ExecuteSQL(pSql)

        Return pFowstID

    End Function

    Public Function GetOrAddFowstByName(vOrgID As String, vFowstName As String, Optional vStreet As String = Nothing,
        Optional vThirdPartySystemID As String = Nothing,
        Optional vTINBranchCode As String = Nothing) As DataRow

        Dim pDR = GetFowstByName(vOrgID, vFowstName)
        If pDR IsNot Nothing Then
            Return pDR
        End If

        Dim pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pIU("ID") = "NEWID()"
        pIU("OrganizationID") = ReadyString(vOrgID)
        pIU("StreetNumber") = "''"
        pIU("Street") = ReadyString(vStreet)
        pIU("FowstCityID") = "NULL"
        pIU("FowstCountryID") = "NULL"
        pIU("Name") = ReadyString(vFowstName)
        If String.IsNullOrWhiteSpace(vThirdPartySystemID) = False Then
            pIU("ThirdPartySystemID") = ReadyString(vThirdPartySystemID)
        End If
        If String.IsNullOrWhiteSpace(vTINBranchCode) = False Then
            pIU("TINBranchCode") = ReadyString(vTINBranchCode)
        End If
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        pDR = GetFowstByName(vOrgID, vFowstName)

        Return pDR

    End Function

    Public Function GetFowstByName(vOrgID As String, vFowstName As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            ",Name" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE Name = " & ReadyString(vFowstName)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function AddOrUpdateFromMainAddress(vOrgID As String, Optional vAddressRequired As Boolean = True) As String

        Dim pDR = GetOrgAddress(vOrgID)
        If pDR Is Nothing Then
            Return Nothing
        End If

        Return AddOrUpdateFromMainAddress(
            vOrgID:=vOrgID,
            vBuildingNum:=CStr2(pDR("BuildingNum")),
            vStreet:=CStr2(pDR("Street")),
            vCityID:=CStr2(pDR("CityID")),
            vCountryID:=CStr2(pDR("CountryID")),
            vAddressRequired:=vAddressRequired)

    End Function

    Private Function GetOrgAddress(vOrgID As String) As DataRow

        Dim pSql = "SELECT MobizLibraryOrganization.OfficeAddressBuildingNo AS BuildingNum" & vbCrLf &
            ",MobizLibraryOrganization.OfficeAddressStreet AS Street" & vbCrLf &
            ",MobizLibraryOrganization.OfficeAddressCityID AS CityID" & vbCrLf &
            ",MobizLibraryOrganization.OfficeAddressCountryID AS CountryID " & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " WHERE MobizLibraryOrganization.ID = " & ReadyString(vOrgID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetMainAddress(vOrgID As String) As DataRow

        Dim pOut As DataRow
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            ",Name" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND IsMainAddress = 1"
        pOut = mProData.ExecuteRow(pSql)

        Return pOut

    End Function

    Public Function FillFowstNumber() As Integer

        Dim pSql As String
        Dim pDT As DataTable
        Dim pDtFowsts As DataTable
        Dim pMaxFowstNum As Integer
        Dim pIU As InsertUpdate
        Dim pRecordsAffected As Integer

        mProData.BeginTransaction()
        pSql = " SELECT MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
            " ,COUNT(*)" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE MobizLibraryOrganizationFOWST.FOWSTNo IS NULL" & vbCrLf &
            " GROUP BY MobizLibraryOrganizationFOWST.OrganizationID"
        pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            pSql = " SELECT TOP 1 MAX(MobizLibraryOrganizationFOWST.FOWSTNo) AS FOWSTNo" & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(pDR("OrganizationID"))
            pMaxFowstNum = CInt2(mProData.ExecuteScalar(pSql))
            pMaxFowstNum = Math.Max(pMaxFowstNum, 0)

            pSql = " SELECT ID" & vbCrLf &
                " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
                " ,MobizLibraryOrganizationFOWST.FOWSTNo" & vbCrLf &
                " ,MobizLibraryOrganizationFOWST.LastUpdated " & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(pDR("OrganizationID")) & vbCrLf &
                " AND MobizLibraryOrganizationFOWST.FOWSTNo IS NULL" & vbCrLf &
                " ORDER BY MobizLibraryOrganizationFOWST.LastUpdated" & vbCrLf &
                " ,MobizLibraryOrganizationFOWST.Name "
            pDtFowsts = mProData.Execute(pSql)
            For Each pDrFowst As DataRow In pDtFowsts.Rows
                pMaxFowstNum += 1
                pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
                pIU("FOWSTNo") = pMaxFowstNum
                pIU("LastUpdated") = "GETDATE()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDrFowst("ID")))
                mProData.ExecuteSQL(pSql)
                pRecordsAffected += 1
            Next pDrFowst
        Next pDR
        mProData.CommitTransaction()

        Return pRecordsAffected

    End Function

    Public Function GetOrCreateByOrg(vOrgID As String, vName As String) As DataRow

        Dim pDR As DataRow
        Dim pID As String

        pDR = GetFirstByOrgID(vOrgID)
        If pDR Is Nothing Then
            pID = CreateByOrgID(vOrgID, vName)
            pDR = GetFirstByOrgID(vOrgID)
        End If

        Return pDR

    End Function

    Public Function CreateByOrgID(vOrgID As String, vName As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid.ToString
        pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pIU("ID") = ReadyString(pID)
        pIU("OrganizationID") = ReadyString(vOrgID)
        pIU("Name") = ReadyString(vName)
        pIU("StreetNumber") = "''"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        FillFowstNumber()

        Return pID

    End Function

    Public Function FowstType(vFOWSTTypeID As String) As DataRow

        Dim pSql = "SELECT Name" & vbCrLf &
            " FROM MobizLibraryFOWSTType" & vbCrLf &
            " WHERE ID = " & ReadyString(vFOWSTTypeID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    'Public Enum eAddReceiptSettingIfNeeded
    Public Function AddReceiptSettingIfNeeded(vFowstID As String) As Boolean

        Dim pDR = GetReceiptSetting(vFowstID)
        If pDR IsNot Nothing Then
            Return False
        End If


    End Function

    Public Function GetReceiptSetting(vFowstID As String) As DataRow

        Dim pSql = " SELECT LibraryReceiptSettings.ID" & vbCrLf &
            " FROM LibraryReceiptSettings" & vbCrLf &
            " JOIN LibraryReceiptSettingsFowst ON LibraryReceiptSettingsFowst.ReceiptSettingsID = LibraryReceiptSettings.ID" & vbCrLf &
            " WHERE LibraryReceiptSettingsFowst.FowstID = " & ReadyString(vFowstID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Function GetReceiptSettings() As DataTable

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryReceiptSettings"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function AddReceiptSetting(vFowstID As String, vReceiptSettingID As String)


    End Function

    Public Function FowstGroupPrice(vFowstID As String) As DataRow

        Dim pSql = " SELECT MobizLibraryPIMPriceLevel.ID" & vbCrLf &
            " ,MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMPriceLevel ON MobizLibraryPIMPriceLevel.ID = MobizLibraryOrganizationFranchisee.DefaultSellingPriceLevel" & vbCrLf &
            " WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = " & ReadyString(vFowstID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function AddInCacheInvoicesButNotInGroup(vInvoiceDateFrom As Date, vInvoiceDateTo As Date, vGroupName As String, vOrgID As String) As DataTable

        Dim pDT = InCacheInvoicesButNotInGroup(vInvoiceDateFrom, vInvoiceDateTo, vGroupName)
        If pDT.Rows.Count = 0 Then
            Return pDT
        End If

        Dim pDrFranchiseGroup = GetFranchiseGroup(vGroupName)
        If pDrFranchiseGroup Is Nothing Then
            Throw New ApplicationException("vGroupName, " & vGroupName & ", does not exist. ref: vbCPTLK5SJ8K!")
        End If
        For Each pDR As DataRow In pDT.Rows
            UpdateStatus(CStr2(pDR("FowstID")))
            'GetOrAddOrganizationFranchisee(CStr2(pDR("FowstID")), CStr2(pDrFranchiseGroup("ID")), vOrgID)
        Next

    End Function

    Sub UpdateStatus(vFowstID As String)

        Dim pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pIU("Status") = "'Store and device are operational.'"
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(vFowstID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function InCacheInvoicesButNotInGroup(vInvoiceDateFrom As Date, vInvoiceDateTo As Date, vGroupName As String) As DataTable

        Dim pSql = " -- in CacheInvoices but not in group" & vbCrLf &
            " -- InCacheInvoicesButNotInGroup" & vbCrLf &
            " DECLARE @InvoiceDateFrom DATE;" & vbCrLf &
            " DECLARE @InvoiceDateTo DATE;" & vbCrLf &
            " DECLARE @GroupName varchar(MAX);" & vbCrLf &
            " SET @InvoiceDateFrom = " & ReadyString(vInvoiceDateFrom.Date.ToString(cGlobal.FormatDateTimeSql)) & ";" & vbCrLf &
            " SET @InvoiceDateTo = " & ReadyString(vInvoiceDateTo.Date.ToString(cGlobal.FormatDateTimeSql)) & ";" & vbCrLf &
            " SET @GroupName = " & ReadyString(vGroupName) & ";" & vbCrLf &
            " SELECT CacheInvoices.FowstID" & vbCrLf &
            " ,CacheInvoices.FOWSTName" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE 1=1 --GroupName = @GroupName" & vbCrLf &
            " AND CONVERT(date,InvoiceDate) Between @InvoiceDateFrom AND @InvoiceDateTo" & vbCrLf &
            " AND CacheInvoices.fowstid NOT IN (" & vbCrLf &
            " Select MobizLibraryOrganizationFOWST.id" & vbCrLf &
            "  FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "  LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID" & vbCrLf &
            "  LEFT JOIN CacheInvoices on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID and convert(date,InvoiceDate) Between @InvoiceDateFrom AND @InvoiceDateTo" & vbCrLf &
            "  WHERE  1=1" & vbCrLf &
            "  /*" & vbCrLf &
            "  MobizLibraryOrganizationFranchisee.FranchiseGroupID IN" & vbCrLf &
            "  ('7c3897bd-4501-4211-93cb-0ed2a4749141','0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4'," & vbCrLf &
            "  '1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'," & vbCrLf &
            "  '61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915'," & vbCrLf &
            "  'f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548')" & vbCrLf &
            "  */" & vbCrLf &
            "  AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.')" & vbCrLf &
            " -- and LibraryFranchiseGroup.GroupName = @GroupName" & vbCrLf &
            " )" & vbCrLf &
            " GROUP BY CacheInvoices.fowstid" & vbCrLf &
            " ,CacheInvoices.FOWSTName" & vbCrLf &
            " ORDER BY CacheInvoices.FOWSTName"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Function GetFranchiseGroup(vGroupName As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryFranchiseGroup" & vbCrLf &
            " WHERE LibraryFranchiseGroup.GroupName = " & ReadyString(vGroupName)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Function GetOrAddOrganizationFranchisee(vFowstID As String, vFranchiseGroupID As String, vOrgID As String) As DataRow

        Dim pSql = "SELECT *" & vbCrLf &
            " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            " WHERE FranchiseeOrgFowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND FranchiseGroupID = " & ReadyString(vFranchiseGroupID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR
        End If

        Dim p = New InsertUpdate("MobizLibraryOrganizationFranchisee")

        p("FranchisorOrgID") = ReadyString(vOrgID)
        p("FranchiseeOrgFowstID") = ReadyString(vFowstID)
        p("IsFranchisorConfirmed") = 1
        p("IsFranchiseeConfirmed") = 1
        p("IsActive") = 1
        p("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        p("FranchiseGroupID") = ReadyString(vFranchiseGroupID)
        p("LastUpdated") = "getDate()"
        pSql = p.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return GetOrAddOrganizationFranchisee(vFowstID, vFranchiseGroupID, vOrgID)

    End Function
End Class
