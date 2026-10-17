Public Class LibraryPrinters

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID">LibraryDevicePrinterFowst.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByDevicePrinterFowstID(vID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT LibraryDevicePrinterFowst.ID" & vbCrLf &
            ",LibraryDevicePrinter.PrinterType" & vbCrLf &
            " ,LibraryDevicePrinterFowst.PrinterName" & vbCrLf &
            " ,LibraryDevicePrinterFowst.ConnectionType" & vbCrLf &
            " ,LibraryDevicePrinterFowst.PrinterIPAddress" & vbCrLf &
            ",LibraryDevicePrinterFowst.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,LibraryDevicePrinter.PrinterRole" & vbCrLf &
            ",LibraryDevicePrinterFowst.PrinterID" & vbCrLf &
            ",LibraryDevicePrinterFowst.SerialNum" & vbCrLf &
            ",LibraryDevicePrinter.Brand" & vbCrLf &
            ",LibraryDevicePrinter.Model" & vbCrLf &
            " FROM LibraryDevicePrinter" & vbCrLf &
            " JOIN LibraryDevicePrinterFowst ON LibraryDevicePrinterFowst.PrinterID = LibraryDevicePrinter.ID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryDevicePrinterFowst.FowstID" & vbCrLf &
            " WHERE LibraryDevicePrinterFowst.ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            pDR("PrinterRole") = ConvertRoleText(CStr2(pDR("PrinterRole")))
        End If

        Return pDR

    End Function

    Public Shared Function ConvertRoleText(vText As String) As String

        If String.IsNullOrWhiteSpace(vText) Then
            Return "POS/ General Purpose Printer"
        End If

        If vText = "POS/ General Purpose Printer" _
            OrElse vText = "Category Printer" Then

            Return vText
        End If

        If vText = "Point of Sale (POS)" Then
            Return "POS/ General Purpose Printer"
        End If

        Return "Category Printer"

    End Function

    ' LibraryDevicePrinter
    Function GetOrCreatePrinter(vBrand As String, vModel As String, vPrinterRole As String, vUserID As String) As String

        Dim pDR As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String

        pDR = GetPrinter(vBrand, vModel, vPrinterRole)
        If pDR IsNot Nothing Then
            If CStr2(pDR("Brand")) = "" Then
                pIU = New InsertUpdate("LibraryDevicePrinter")
                pIU("Brand") = ReadyString(vBrand)
                pIU("Model") = ReadyString(vModel)
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                mProData.ExecuteSQL(pSql)
            End If
            Return CStr2(pDR("ID"))
        End If

        pIU = New InsertUpdate("LibraryDevicePrinter")
        pID = Guid.NewGuid.ToString()
        pIU("ID") = ReadyString(pID)
        pIU("Brand") = ReadyString(vBrand)
        pIU("Model") = ReadyString(vModel)
        pIU("UserID") = ReadyString(vUserID)
        pIU("PrinterRole") = ReadyString(vPrinterRole)
        pIU("PrinterType") = ReadyString(vBrand & " " & vModel)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    ' LibraryDevicePrinter
    Function GetPrinter(vBrand As String, vModel As String, vPrinterRole As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String
        Dim pWhere As String

        pSql = "SELECT *" & vbCrLf &
            " FROM LibraryDevicePrinter" & vbCrLf &
            " WHERE PrinterRole = " & ReadyString(vPrinterRole) & vbCrLf
        pWhere = " AND Brand = " & ReadyString(vBrand) & vbCrLf &
            " AND Model = " & ReadyString(vModel)
        pDR = mProData.ExecuteRow(pSql & pWhere)
        If pDR IsNot Nothing Then
            Return pDR
        End If

        pWhere = " AND PrinterType = " & ReadyString(vBrand & " " & vModel)
        pDR = mProData.ExecuteRow(pSql & pWhere)
        Return pDR

    End Function

    ' LibraryDevicePrinterFowst
    Public Function SavePrinterFowst(vID As String,
        vFowstID As String,
        vPrinterID As String,
        vPrinterIPAddress As String,
        vPrinterName As String,
        vConnectionType As String,
        vSerialNum As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("LibraryDevicePrinterFowst")
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("PrinterID") = ReadyString(vPrinterID)
        pIU("PrinterIPAddress") = ReadyString(vPrinterIPAddress)
        pIU("PrinterName") = ReadyString(vPrinterName)
        pIU("ConnectionType") = ReadyString(vConnectionType)
        pIU("SerialNum") = ReadyString(vSerialNum)
        If String.IsNullOrWhiteSpace(vID) Then
            vID = Guid.NewGuid.ToString()
            pIU("ID") = ReadyString(vID)
            pSql = pIU.GetInsert()
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(vID))
        End If
        mProData.ExecuteSQL(pSql)

        Return vID

    End Function

    ''' <summary>
    ''' all PIM categories and included by printer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllCategories(vPrinterFowstID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category]" & vbCrLf &
            " ,CASE WHEN LibraryDevicePrinterCategory.ID IS NULL" & vbCrLf &
            " 	THEN 0" & vbCrLf &
            " 	ELSE 1" & vbCrLf &
            "     END AS Included" & vbCrLf &
            " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            " JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
            " LEFT JOIN LibraryDevicePrinterCategory ON LibraryDevicePrinterCategory.CategoryName = MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            " AND CONVERT(varchar(36), LibraryDevicePrinterCategory.PrinterFowstID) = " & ReadyString(vPrinterFowstID) & vbCrLf &
            " ORDER BY Category"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Sub SaveCategories(vPrinterFowstID As String, vCategories As List(Of String), Optional isNewSetup As Boolean = False)
        'isNewSetup meaning multiple FOWSTs na pwede sa Printer template.
        Dim pSql As String
        Dim pIU As InsertUpdate

        mProData.BeginTransaction()
        pSql = "DELETE FROM LibraryDevicePrinterCategory" & vbCrLf


        If isNewSetup Then
            'vPrinterFowstID ID sa Printer ang value ani
            pSql &= " WHERE PrinterID = " & ReadyString(vPrinterFowstID)
        Else
            pSql &= " WHERE PrinterFowstID = " & ReadyString(vPrinterFowstID)
        End If

        mProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("LibraryDevicePrinterCategory")

        If isNewSetup Then
            pIU("PrinterID") = ReadyString(vPrinterFowstID)
        Else
            pIU("PrinterFowstID") = ReadyString(vPrinterFowstID)
        End If

        For Each pCategory As String In vCategories
            pIU("CategoryName") = ReadyString(pCategory)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pCategory
        mProData.CommitTransaction()

    End Sub

    Public Function GetPrinterByFowstIdAndUuid(vFowstID As String, vUuid As String) As DataRow

        Dim pSql = " SELECT devfowstpntr.ID" & vbCrLf &
            " ,fowstpntr.PrinterName" & vbCrLf &
            " ,fowstpntr.PrinterIPAddress" & vbCrLf &
            " ,fowstpntr.ProductDescriptionInReceipt" & vbCrLf &
            " FROM LibraryDeviceFowstPrinter devfowstpntr  " & vbCrLf &
            " JOIN LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID)  " & vbCrLf &
            " JOIN LibraryDevice ON LibraryDevice.ID = devfowstpntr.DeviceID" & vbCrLf &
            " Where fowstpntr.FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
            " AND LibraryDevice.UUID = " & ReadyString(vUuid)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class