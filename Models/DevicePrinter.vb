Public Class DevicePrinter
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property PrinterRole As String
    Public Property PrinterType As String
    Public Property FowstID As String
    Public Property ConnectionType As String
    Public Property IsDefaultDispatchPrinter As Boolean
    Public Property PrinterIP As String
    Public Property IsActive As String
    Public Property UserID As String
    Public Property LastUpdated As DateTime

    Public Property PrinterID As String
    Public Property DeviceID As String
    Public Property CategoryName As String
    Public Property PrinterName As String

    Function CheckUniquePrinterRole() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String

        If PrinterID = "" Then
            pSql = "SELECT ID, PrinterRole FROM LibraryDevicePrinter WHERE PrinterRole = " & ReadyString(PrinterRole)
        Else

            pSql = "SELECT ID, PrinterRole FROM LibraryDevicePrinter WHERE ID <> " & ReadyString(PrinterID) & " AND PrinterRole = " & ReadyString(PrinterRole)
        End If

        pDT = Common.WebRequest.ProData.Execute(pSql)
        Return pDT

    End Function

    Function CheckUniquePrinterName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String

        If ID = "" Then
            pSql = "SELECT ID, PrinterName FROM LibraryDevicePrinterFowst WHERE PrinterName = " & ReadyString(PrinterName) & " AND FowstID = " & ReadyString(FowstID)
        Else

            pSql = "SELECT ID, PrinterName FROM LibraryDevicePrinterFowst WHERE ID <> " & ReadyString(ID) & " AND PrinterName = " & ReadyString(PrinterName) & " AND FowstID = " & ReadyString(FowstID)

        End If
        
        pDT = Common.WebRequest.ProData.Execute(pSql)
        Return pDT

    End Function


    Function getDevicePrinterList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterRole", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterRole"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterType"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "prntr.IsActive"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "prntr.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinter prntr " &
                        " LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = prntr.UserID)"

            .SearchString = strKeyWord

            .SqlOrder = "ORDER BY prntr.PrinterRole ASC, prntr.IsActive DESC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDevicePrinterCategoryList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
                With .Add("CategoryName", SqlDbType.VarChar)
                    .Expression = "CategoryName"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinterCategory "

            .SqlWhere = "WHERE PrinterID = " & ReadyString(ID)

            .SqlOrder = "ORDER BY CategoryName ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDevicePrinterListByFowstID(ByVal id As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                End With
                With .Add("PrinterRole", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterRole"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterType"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "prntr.IsActive"
                End With
                With .Add("FowstID", SqlDbType.VarChar)
                    .Expression = "fowst.FowstID"
                End With
                With .Add("PrinterIPAddress", SqlDbType.VarChar)
                    .Expression = "fowst.PrinterIPAddress"
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "fowst.PrinterName"
                End With
            End With

            .SqlFrom = " FROM LibraryDevicePrinter prntr LEFT JOIN LibraryDevicePrinterFowst fowst ON fowst.PrinterID = prntr.ID"


            .SqlWhere = " WHERE fowst.FowstID = " & ReadyString(id)

            .SqlOrder = " ORDER BY prntr.PrinterRole ASC, prntr.IsActive DESC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDevicePrinterListAvailable() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterRole", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterRole"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterType"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "prntr.IsActive"
                End With
            End With

            .SqlFrom = " FROM LibraryDevicePrinter prntr "
            .SqlWhere = " WHERE prntr.IsActive = 1 "
            '"AND prntr.PrinterRole NOT IN (SELECT DISTINCT pr.PrinterRole FROM LibraryDevicePrinter pr " &
            '"LEFT JOIN LibraryDevicePrinterFowst fw ON (pr.ID = fw.PrinterID) LEFT JOIN LibraryDeviceFowstPrinter fp ON (fw.ID = fp.PrinterFowstID)" &
            '" WHERE pr.PrinterRole <> 'Point of Sale (POS)') "

            'orginal code

            '.SqlWhere = " WHERE prntr.IsActive = 1 AND prntr.PrinterRole NOT IN (SELECT DISTINCT pr.PrinterRole FROM LibraryDevicePrinter pr " &
            '    "LEFT JOIN LibraryDevicePrinterFowst fw ON (pr.ID = fw.PrinterID) LEFT JOIN LibraryDeviceFowstPrinter fp ON (fw.ID = fp.PrinterFowstID)" &
            '    " WHERE pr.PrinterRole <> 'Point of Sale (POS)') "

            .SqlOrder = " ORDER BY prntr.PrinterRole ASC, prntr.IsActive DESC"

            pDT = .Requery

        End With

        Return pDT

    End Function


    Function getDevicePrinterInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterRole", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterRole"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterType"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "prntr.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinter prntr " 

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prntr.ID IS NULL", " WHERE prntr.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveDevicePrinterInfo() As String
        Dim p As New InsertUpdate("LibraryDevicePrinter")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("PrinterRole") = ReadyString(PrinterRole)
        p("PrinterType") = ReadyString(PrinterType)

        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                p("IsActive") = "1"
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                p("IsActive") = IsActive
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ID = strIDReturn

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevicePrinter'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            'remove all first, then save the newly added
            If (WebRequest.Instance.Data("Categories") <> "") Then

                SaveCategories(ID)

            End If

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn
    End Function

    Function SaveFOWSTPrinterIP() As String
        Dim p As New InsertUpdate("LibraryDevicePrinterFowst")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("PrinterName") = ReadyString(PrinterName)
        p("PrinterID") = ReadyString(PrinterID)
        p("PrinterIPAddress") = ReadyString(PrinterIP)
        p("FowstID") = ReadyString(FowstID)
        p("ConnectionType") = ReadyString(ConnectionType)
        p("IsDefaultDispatchPrinter") = CInt2(IsDefaultDispatchPrinter)
       
        Try
            Common.WebRequest.ProData.BeginTransaction()

            If IsDefaultDispatchPrinter Then
                pSql = "UPDATE LibraryDevicePrinterFowst set IsDefaultDispatchPrinter=0 WHERE FowstID = " & ReadyString(FowstID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ID = strIDReturn


            pSql = "Update LibraryDevicePrinter Set LastUpdated = GetDate() Where ID = " & ReadyString(PrinterID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn
    End Function


    Function CheckUniquePrinterDevice() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntrdev.ID"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceFowstPrinter prntrdev "

            .SqlWhere = "WHERE prntrdev.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND prntrdev.DeviceID = " & ReadyString(DeviceID) &
                        " AND prntrdev.PrinterID = " & ReadyString(WebSession.Instance().DevicePrinterID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getDevicePrinterDeviceList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntrdev.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.VarChar)
                    .Expression = "prntrdev.DeviceID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowstPrinter prntrdev " &
                " INNER JOIN LibraryDevice dev ON (prntrdev.DeviceID = dev.ID) "

            .SqlWhere = " WHERE prntrdev.PrinterID = " & ReadyString(WebSession.Instance().DevicePrinterID)

            .SearchString = strKeyWord

            .SqlOrder = "ORDER BY dev.PhoneName ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDevicePrinterDeviceInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntrdev.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.VarChar)
                    .Expression = "prntrdev.DeviceID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceFowstPrinter prntrdev " &
                " INNER JOIN LibraryDevice dev ON (prntrdev.DeviceID = dev.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prntrdev.ID IS NULL", " WHERE prntrdev.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveDevicePrinterDeviceInfo() As String
        Dim p As New InsertUpdate("LibraryDeviceFowstPrinter")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("DeviceID") = IIf(DeviceID.Trim().Length <= 0, "NULL", ReadyString(DeviceID))
        p("PrinterID") = IIf(PrinterID.Trim().Length <= 0, "NULL", ReadyString(PrinterID))
        p("IsActive") = IsActive
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            ID = strIDReturn


            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn
    End Function

    Function GetAllCategories() As DataTable
        Dim pSql As String
        Dim pFinder As Finder
        Dim pDt As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "ISNULL(CASE WHEN MobizLibraryPIMProductVariableFeatures001.C0020 = '' THEN 'N/A' ELSE MobizLibraryPIMProductVariableFeatures001.C0020 END,'N/A')" '"ISNULL(dbo." & pTableName & "." & pColumn & ",'N/A')"
                End With
            End With


            .SqlFrom = "FROM MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                        "LibraryOrganizationPIM on MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryOrganizationPIM.ProductID "

            .SqlWhere = "WHERE LibraryOrganizationPIM.IsDeactivated = 0 "

            Return .Requery
        End With
    End Function

    Function GetDevicePrinterCategories() As DataTable
        Dim pSql As String
        Dim pFinder As Finder
        Dim pDt As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("CategoryName", SqlDbType.VarChar)
                    .Expression = "CategoryName"
                End With
            End With


            .SqlFrom = "FROM LibraryDevicePrinterCategory "

            .SqlWhere = "WHERE PrinterID = " & ReadyString(PrinterID)

            Return .Requery
        End With
    End Function

    Function SaveCategories(ByVal pID) As String

        Dim p As New InsertUpdate("LibraryDevicePrinterCategory")
        Dim pSql As String
        Dim Categories() As String

        Categories = Split(WebRequest.Instance.Data("Categories"), "|")

        Try

            Common.WebRequest.ProData.BeginTransaction()

            Dim i As Integer
            Dim result As DataTable
            
            If RemoveAllCategoriesByID(ID) Then
                While i < UBound(Categories)
                    result = IsCategoryExist(pID, Categories(i))

                    'if it exists, replace the printer id with the new one
                    If result.Rows.Count > 0 Then
                        pSql = "DELETE FROM LibraryDevicePrinterCategory WHERE CategoryName = " & ReadyString(Categories(i))

                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    p("PrinterID") = ReadyString(pID)
                    p("CategoryName") = ReadyString(Categories(i))
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.Execute(pSql)

                    i += 1
                End While

            End If

            pSql = "Update LibraryDevicePrinter Set LastUpdated = GetDate() Where ID = " & ReadyString(pID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()

            Return True
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return False
        End Try
    End Function

    Function IsCategoryExist(ByVal printer, ByVal category) As DataTable
        Dim pSql As String
        Dim dt As DataTable

        If PrinterID = "" Then
            pSql = "SELECT ID FROM LibraryDevicePrinterCategory WHERE CategoryName = " & ReadyString(category)
        Else

            pSql = "SELECT ID FROM LibraryDevicePrinterCategory WHERE PrinterID <> " & ReadyString(printer) & " AND CategoryName = " & ReadyString(category)
        End If


        dt = Common.WebRequest.ProData.Execute(pSql)


        Return dt

    End Function

    Function RemoveAllCategoriesByID(ByVal pID) As Boolean
        Dim pSql As String

        pSql = "DELETE FROM LibraryDevicePrinterCategory WHERE PrinterID = " & ReadyString(pID)

        Try
            Common.WebRequest.ProData.Execute(pSql)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function CheckDefaultDispatchPrinter() As DataTable
        Dim pSql As String
        Dim dtTemp As DataTable

        pSql = "SELECT LibraryDevicePrinterFowst.* " & vbCrLf &
                "FROM LibraryDevicePrinterFowst  " & vbCrLf &
                "WHERE IsDefaultDispatchPrinter = 1"

        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        Return dtTemp
    End Function
End Class
