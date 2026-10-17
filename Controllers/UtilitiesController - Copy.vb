Imports System.IO
Imports System.Runtime.Remoting.Messaging
Imports System.Threading
Imports System.Web.Services.Description
Imports Aspose.Cells
Imports Newtonsoft.Json
Imports PdfSharp.Pdf.IO
Imports WinSCP

Public Class UtilitiesController
    Inherits System.Web.Mvc.Controller

    Const mPassword As String = "1qaz2wsx"

    Function Index() As ActionResult

        Return View()

    End Function

    <HttpPost>
    Public Function GetOrganizations() As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        Try
            Return JsonDictionary.Serialize(WebRequest.Instance.Business.LibraryOrganization.GetAll(True))
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Private Function VerifyPassword() As Boolean

        Return Common.WebRequest.Data("Password") = mPassword

    End Function

    <HttpPost>
    Public Function GetByAmount() As String

        Dim pOrgID As String
        Dim pAmount As String
        Dim pDT As DataTable
        Dim pOut As JsonDictionary

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        pOrgID = Common.WebRequest.Data("OrgID")
        If IsGuid(pOrgID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
        End If
        pOrgID = ReadyGuid(pOrgID)

        pAmount = Common.WebRequest.Data("Amount")
        If IsNumeric(pAmount) = False Then
            Return JsonDictionary.Serialize("Error", "Amount must be a valid value")
        End If
        pAmount = CDec(pAmount)

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pDT = WebRequest.Instance.Business.TransSalesReturnLedger.GetByAmount(pAmount)
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "SRV does not exist")
        End If

        pOut = New JsonDictionary()
        pOut.Add("data", pDT)

        Return pOut.Serialize()

    End Function

    <HttpPost>
    Public Function LastInvoicePerUser() As String

        Dim pMonth As String
        Dim pYear As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pFilename As String
        Dim pBaseFilename As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        pMonth = Common.WebRequest.Data("Month")
        If IsNumeric(pMonth) = False Then
            Return JsonDictionary.Serialize("Error", "Month must be a number")
        End If
        pMonth = CInt(pMonth)

        pYear = Common.WebRequest.Data("Year")
        If IsNumeric(pYear) = False Then
            Return JsonDictionary.Serialize("Error", "Year must be a number")
        End If
        pYear = CInt(pYear)

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = " -- LastInvoicePerUser2" & vbCrLf &
            "  SELECT tblInvoice.InvoiceNumber AS LastInvoiceNumber" & vbCrLf &
            "  ,CONVERT(datetime, tblInvoice.InvoiceDate) AS LastInvoiceDate" & vbCrLf &
            "  ,tblInvoice.FowstName" & vbCrLf &
            "  ,tblInvoice.FullName" & vbCrLf &
            "  ,LibraryDevice.PhoneName" & vbCrLf &
            "  ,LibraryDevice.UUID" & vbCrLf &
            "  ,LibraryDevice.UuidDisplay" & vbCrLf &
            "  ,LibraryDeviceFowst.BIRMachineNo" & vbCrLf &
            "  ,LibraryDeviceFowst.POSRegistrationNo" & vbCrLf &
            "  FROM LibraryDevice" & vbCrLf &
            "  LEFT JOIN LibraryDeviceFowst ON LibraryDeviceFowst.DeviceID = LibraryDevice.ID" & vbCrLf &
            "  CROSS APPLY (" & vbCrLf &
            "  	SELECT TOP 1 TransInvoice.InvoiceDate" & vbCrLf &
            "     ,TransInvoice.InvoiceNumber" & vbCrLf &
            "     ,MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.MiddleName AS FullName" & vbCrLf &
            "     ,MobizLibraryOrganizationFowst.Name AS FowstName" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            "  	LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInvoice.UserID" & vbCrLf &
            "     LEFT JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = TransInvoice.FowstID" & vbCrLf &
            "  	WHERE LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            "  	AND LibraryDeviceFowst.FowstID = TransInvoice.FowstID" & vbCrLf &
            "  	AND MONTH(TransInvoice.InvoiceDate) = " & pMonth & vbCrLf &
            "  	AND YEAR(TransInvoice.InvoiceDate) = " & pYear & vbCrLf &
            "  	AND TransInvoice.InvoiceNumber > 0" & vbCrLf &
            "     ORDER BY TransInvoice.InvoiceDate DESC" & vbCrLf &
            "  ) tblInvoice" & vbCrLf &
            "  ORDER BY LastInvoiceDate"
        Common.WebRequest.ProData.CommandTimeout = 60 * 100 ' 100 minutes
        pDT = Common.WebRequest.ProData.Execute(pSql)
        pBaseFilename = "LastInvoicePerUser_" & pYear & "_" & CInt2(pMonth).ToString("00") & ".xls"
        pFilename = Common.ServerPath & "temp\documents\" & pBaseFilename
        CreateFolder(pFilename)
        TableToExcelFile(pDT, "", "", pFilename)

        Return JsonDictionary.Serialize("Link", pBaseFilename)

    End Function

    <HttpPost>
    Public Function GetInvoices() As String

        Dim pDT As DataTable
        Dim pInvoiceNum As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        Common.WebRequest.ProData.ExecuteSQL("USE wholesale")
        pInvoiceNum = Common.WebRequest.Data("InvoiceNum")
        pDT = WebRequest.Instance.Business.TransInvoiceLegacy.GetInvoiceID(pInvoiceNum)

        Return JsonDictionary.Serialize("data", pDT)

    End Function

    <HttpPost>
    Public Function ChangeInvoiceDate() As String

        Dim pInvoiceID As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pInvoiceDate As String
        Dim pRowCount As Integer

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid InvoiceID")
        End If

        pInvoiceDate = Common.WebRequest.Data("NewInvoiceDate")
        If IsDate(pInvoiceDate) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid NewInvoiceDate")
        End If

        Common.WebRequest.ProData.ExecuteSQL("USE wholesale")
        pIU = New InsertUpdate("TransInvoice")
        pIU("InvoiceDate") = ReadyString(pInvoiceDate)
        pSql = pIU.GetUpdate("ID = " & ReadyString(pInvoiceID))
        pRowCount = Common.WebRequest.ProData.ExecuteSQL(pSql, True)

        Return JsonDictionary.Serialize("RowCount", pRowCount)

    End Function

    <HttpPost>
    Public Function UntagReceivingAsPayable() As String

        Dim pSql As String
        Dim pReceivingNum As String
        Dim pDT As DataTable
        Dim pDR As DataRow

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If
        pReceivingNum = Common.WebRequest.Data("ReceivingNum")
        If IsNumeric(pReceivingNum) = False Then
            Return JsonDictionary.Serialize("Error", "ReceivingNum must be numeric")
        End If
        pReceivingNum = CInt2(pReceivingNum)

        Common.WebRequest.ProData.ExecuteSQL("USE wholesale")
        pSql = " SELECT IsPayables" & vbCrLf &
            " ,TransPayables.PayablesNo" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " LEFT JOIN TransPayables ON TransPayables.ReceivingID = TransReceiving.ID" & vbCrLf &
            " WHERE TransReceiving.ReceivingNo = " & pReceivingNum
        pDT = Common.WebRequest.ProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "Record with the specified receiving number does not exist.")
        End If
        pDR = pDT.Rows(0)
        If pDR("PayablesNo") IsNot DBNull.Value Then
            Return JsonDictionary.Serialize("Error", "Already paid with Payable No. " & pDR("PayablesNo"))
        End If
        If CBool2((pDR("IsPayables"))) = False Then
            Return JsonDictionary.Serialize("Error", "It seems that there is nothing wrong with the specified receiving number.")
        End If

        pSql = "UPDATE TransReceiving" &
            " SET IsPayables = 0" & vbCrLf &
            " WHERE ReceivingNo = " & pReceivingNum
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost>
    Public Function SetJit() As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOrgID As String

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            pOrgID = Common.WebRequest.Data("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)

            pIU = New InsertUpdate("LibraryPIMProductParent")
            pIU("ID") = "NEWID()"
            pIU("ProductID") = "LibraryPIMProductContents.ParentProductID"
            pIU("OrganizationID") = "LibraryPIMProductContents.OrganizationID"
            pIU("UserID") = "'F0F54F3F-DFB8-4C47-BFC6-8C4EA17AFA14'" ' web web web
            pIU("LastUpdated") = "GETDATE()"
            pIU("Jit") = 1
            pIU.FromSql = " FROM LibraryPIMProductContents" & vbCrLf &
                " LEFT JOIN LibraryPIMProductParent ON LibraryPIMProductParent.ProductID = LibraryPIMProductContents.ParentProductID" & vbCrLf &
                " AND LibraryPIMProductParent.OrganizationID = LibraryPIMProductContents.OrganizationID" & vbCrLf &
                " WHERE LibraryPIMProductParent.ID IS NULL" & vbCrLf &
                " AND LibraryPIMProductContents.OrganizationID = " & ReadyString(pOrgID) &
                " GROUP BY LibraryPIMProductContents.ParentProductID" & vbCrLf &
                ",LibraryPIMProductContents.OrganizationID"
            pSql = pIU.GetInsert(True)
            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql, True)

            Return JsonDictionary.Serialize("Result", pSql)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost>
    Public Function GetDucInvoiceTypes() As String

        Dim pBusiness = New Business(ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="wholesale",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
        ))

        Dim pSql = "SELECT MainInvoiceDefaults.ID" & vbCrLf &
            ",MainInvoiceDefaults.TransactionType" & vbCrLf &
            ",MainInvoiceDefaults.Abbrevation" & vbCrLf &
            " FROM MainInvoiceDefaults" & vbCrLf &
            " WHERE ID IN (" & ReadyString(TransInvoiceLegacy.gIdChiCas) & vbCrLf &
            "," & ReadyString(TransInvoiceLegacy.gIdTr) & vbCrLf &
            "," & ReadyString(TransInvoiceLegacy.gIdDr) & vbCrLf &
            ")" & vbCrLf &
            " ORDER BY MainInvoiceDefaults.Abbrevation"
        Dim pDT = pBusiness.ProData.Execute(pSql)
        Dim pOut = New JsonDictionary()
        pOut.Add("data", pDT)
        Return pOut.ToString()

    End Function

    <HttpPost>
    Public Function GetDucInvoiceInfo() As String

        Dim pBusiness = New Business(ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="wholesale",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
        ))

        Dim pInvoiceNumber = Common.WebRequest.Data("InvoiceNumber")
        If IsNumeric(pInvoiceNumber) = False Then
            Return JsonDictionary.ToString("error", "InvoiceNumber must be numeric. ref: vbE4GV6JZ6FM!")
        End If
        Dim pOldInvoiceDefaultID = Common.WebRequest.Data("OldInvoiceDefaultID")
        If IsGuid(pOldInvoiceDefaultID) = False Then
            Return JsonDictionary.ToString("error", "OldInvoiceDefaultID must be GUID. ref: vb3KVR4PQ7DZ!")
        End If
        Dim pNewInvoiceDefaultID = Common.WebRequest.Data("NewInvoiceDefaultID")
        If IsGuid(pNewInvoiceDefaultID) = False Then
            Return JsonDictionary.ToString("error", "NewInvoiceDefaultID must be GUID. ref: vbA93DHSWCP7!")
        End If
        If ReadyGuid(pOldInvoiceDefaultID) = ReadyGuid(pNewInvoiceDefaultID) Then
            Return JsonDictionary.ToString("error", "OldInvoiceDefaultID must not be identical to NewInvoiceDefaultID. ref: vbG86WTJEUTA!")
        End If
        Dim pDR = pBusiness.TransInvoiceLegacy.GetInfoByNumberAndTypeID(CInt2(pInvoiceNumber), pOldInvoiceDefaultID)
        If pDR Is Nothing Then
            Return JsonDictionary.ToString("error", "No record found")
        End If
        Dim pOut = New JsonDictionary()
        pOut.Add("data", pDR)

        Dim pAbbrevation As String
        If pBusiness.TransInvoice.GetInvoiceType(pNewInvoiceDefaultID, Nothing, pAbbrevation) = False Then
            Return JsonDictionary.ToString("error", "No record found for NewInvoiceDefaultID")
        End If
        Dim pNewInvoiceNumber = pBusiness.TransInvoiceLegacy.NextInvoiceNumber(pNewInvoiceDefaultID)
        pOut.Add("NewInvoiceDefaultID", pNewInvoiceDefaultID)
        pOut.Add("NewAbbrevation", pAbbrevation)
        pOut.Add("NewInvoiceNumber", pNewInvoiceNumber)
        Return pOut.ToString()

    End Function

    <HttpPost>
    Public Function ChangeDucInvoiceType() As String

        Dim pBusiness = New Business(ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="wholesale",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
        ))

        Dim pOldInvoiceNumber = Common.WebRequest.Data("OldInvoiceNumber")
        If IsNumeric(pOldInvoiceNumber) = False Then
            Return JsonDictionary.ToString("error", "OldInvoiceNumber must be numeric. ref: vbFTNNM998G7!")
        End If
        If CInt2(pOldInvoiceNumber) = 0 Then
            Return JsonDictionary.ToString("error", "OldInvoiceNumber must not be zero. ref: vb8R5L7KNFK5!")
        End If
        Dim pOldInvoiceDefaultID = Common.WebRequest.Data("OldInvoiceDefaultID")
        If IsGuid(pOldInvoiceDefaultID) = False Then
            Return JsonDictionary.ToString("error", "OldInvoiceDefaultID must be GUID. ref: vb3KVR4PQ7DZ!")
        End If
        Dim pExpectedInvoiceNumber = Common.WebRequest.Data("NewInvoiceNumber")
        If IsNumeric(pExpectedInvoiceNumber) = False Then
            Return JsonDictionary.ToString("error", "NewInvoiceNumber must be numeric. ref: vbAZ9SLX3MU8!")
        End If
        Dim pNewInvoiceDefaultID = Common.WebRequest.Data("NewInvoiceDefaultID")
        If IsGuid(pNewInvoiceDefaultID) = False Then
            Return JsonDictionary.ToString("error", "NewInvoiceDefaultID must be GUID. ref: vbZFE5RESX6M!")
        End If
        Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.ToString("error", "InvoiceID must be GUID. ref: vb3ZCNQPU9J4!")
        End If
        Dim pDrOldInvoice = pBusiness.TransInvoiceLegacy.ById(pInvoiceID)
        If pDrOldInvoice Is Nothing Then
            Return JsonDictionary.ToString("error", "No record found. ref: vbPMCRYB36JP!")
        End If

        Dim pDR = pBusiness.TransInvoiceLegacy.ById(pInvoiceID)
        If pDR Is Nothing Then
            Return JsonDictionary.ToString("error", "No record found. ref: vbT8ENSYDCQE!")
        End If
        If CInt2(pDR("InvoiceNumber")) <> CInt2(pOldInvoiceNumber) Then
            Return JsonDictionary.ToString("error", "OldInvoiceNumber and existing invoice number (" & CInt2(pDR("InvoiceNumber")) & ") does not match. ref: vbJYLSTGTT57!")
        End If

        Try
            pBusiness.ProData.BeginTransaction()
            Dim pNewInvoiceNumber = pBusiness.TransInvoiceLegacy.NextInvoiceNumber(pNewInvoiceDefaultID)
            If pExpectedInvoiceNumber <> pNewInvoiceNumber Then
                Return JsonDictionary.ToString("error", "Invoice new expected number and NewInvoiceNumber (" & pNewInvoiceNumber & ") does not match. ref: vbTXQJ5WXKQP!")
            End If

            Dim pIU = New InsertUpdate("TransInvoice")
            pIU("InvoiceDefaultID") = ReadyString(pNewInvoiceDefaultID)
            pIU("InvoiceNumber") = pNewInvoiceNumber
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pInvoiceID))
            pBusiness.ProData.ExecuteSQL(pSql)
            pBusiness.ProData.CommitTransaction()
            Return JsonDictionary.ToString("success", True)
        Catch ex As Exception
            pBusiness.ProData.RollbackTransaction(True, True)
        End Try

    End Function

    Function RecomputeZRead() As String
        Dim dtZReadData As DataTable
        Dim ZRead As New ZRead(Common.WebRequest.ProData)
        Dim ReportsPOS As New ReportPOS
        Dim FOWSTID As String
        Dim DeviceID As String
        Dim ZReadDate As String
        Dim DateFrom As Date
        Dim DateTo As Date

        DateFrom = CDate(Common.WebRequest.Data("From"))
        DateTo = CDate(Common.WebRequest.Data("To"))
        FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
        DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try

            While DateFrom <= DateTo

                Common.WebRequest.ProData.BeginTransaction()

                Dim pSql As String

                pSql = "Delete from TransZReadCache where DeviceID = " & ReadyString(DeviceID) &
                        " AND FOWSTID = " & ReadyString(FOWSTID) &
                        " AND convert(date,SalesDate) = " & ReadyString(DateFrom.ToString("MM/dd/yyyy"))

                Common.WebRequest.ProData.Execute(pSql)

                dtZReadData = ReportsPOS.getSalesZReportSummary("By Date", CStr2(DateFrom), FOWSTID, DeviceID)

                ZRead.SaveZReadCache(dtZReadData, FOWSTID, DeviceID, CStr2(DateFrom))

                DateFrom = DateFrom.AddDays(1)

                Common.WebRequest.ProData.CommitTransaction()
            End While


        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return JsonDictionary.ToString("Error", True)
        End Try

        Return JsonDictionary.ToString("success", True)
    End Function

    <HttpPost>
    Public Function ReimportFlush() As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        Dim pOrgID = Common.WebRequest.Data("FlushOrgID")
        If IsGuid(pOrgID) = False Then
            Return JsonDictionary.ToString("Error", "FlushOrgID must be a GUID. vbA9Z4DS2QTV")
        End If
        pOrgID = ReadyGuid(pOrgID)

        Dim pDeviceID = Common.WebRequest.Data("FlushDeviceID")
        If IsGuid(pDeviceID) = False Then
            Return JsonDictionary.ToString("Error", "FlushDeviceID must be a GUID. vbX824U72AM5")
        End If
        pDeviceID = ReadyGuid(pDeviceID)

        Dim pFolder = Common.ServerPath & "temp\Uploads\" & pOrgID & "\" & pDeviceID & "\"
        Dim pFileNameWithoutExtension = GetLastestFlushZip(pFolder)
        If pFileNameWithoutExtension Is Nothing Then
            Return JsonDictionary.ToString("Error", "Folder, " & pFolder & ", has no valid zip file. vb9VCKKW68CN")
        End If

        Dim pImportDataClass = New ImportDataClass
        pImportDataClass.OrgID = pOrgID
        pImportDataClass.DeviceID = pDeviceID
        pImportDataClass.InputFilename = pFileNameWithoutExtension
        pImportDataClass.Start()

        Return JsonDictionary.ToString("Result", pFolder & pFileNameWithoutExtension & ".zip")

    End Function

    <HttpPost>
    Public Function FakeOrgID() As String

        Dim pOriginalOrgID = Common.WebRequest.Data("OriginalOrgID")
        If IsGuid(pOriginalOrgID) = False Then
            Return JsonDictionary.ToString("Error", "please choose an organizaion first.")
        End If
        Dim pInfo = New DbDownloadFakeOrgID()
        pInfo.DateCreated = Now.ToString("s")
        pInfo.OriginalOrgID = pOriginalOrgID
        Dim pFileText = JsonConvert.SerializeObject(pInfo, Formatting.Indented)
        Dim pFilename = Common.ServerPath & "temp\DbDownloadFakeOrgID.txt"
        CreateFolder(pFilename)
        FileText(pFilename) = pFileText
        Return JsonDictionary.ToString("Result", "ok")

    End Function

    Shared Function GetLastestFlushZip(vFolder As String) As String

        Dim pLatest As String

        Dim pFiles = IO.Directory.GetFiles(vFolder, "*.zip")
        For Each pFile In pFiles
            Dim pFileNameWithoutExtension = IO.Path.GetFileNameWithoutExtension(pFile)
            If pFileNameWithoutExtension.Length <> 25 Then
                Continue For
            End If
            If pLatest Is Nothing Then
                pLatest = pFileNameWithoutExtension
            ElseIf pLatest < pFileNameWithoutExtension Then
                pLatest = pFileNameWithoutExtension
            End If
        Next

        Return pLatest

    End Function

    Public Class DbDownloadFakeOrgID
        Public DateCreated As String
        Public OriginalOrgID As String
    End Class

    <HttpPost>
    Public Function InvoiceTypeChangerUpload() As String

        Dim pOut As JsonDictionary
        Dim pID As String
        Dim pJson As LibraryDocumentControl.SaveJson
        Dim pError As String
        Dim pBaseFilename As String
        Dim pFilename As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pOut = New JsonDictionary()
        If Request.Files.Count > 0 _
                AndAlso Request.Files(0).ContentLength > 0 Then

            pBaseFilename = DateTimeStamp()
            pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChangerAnalyze_" & pBaseFilename & ".xlsx"
            CreateFolder(pFilename)
            Request.Files(0).SaveAs(pFilename)
            pOut.Add("BaseFilename", pBaseFilename)
        End If

        pOut.Add("Result", "ok")
        Return pOut.Serialize()

    End Function

    <HttpPost>
    Public Function InvoiceTypeChangerAnalyze() As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pBaseFilename As String = Common.WebRequest.Data("BaseFilename")
        If String.IsNullOrWhiteSpace(pBaseFilename) Then
            Return JsonDictionary.Serialize("Error", "vb8NC6S8RQS2! BaseFilename is required in POST")
        End If
        Dim pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChangerAnalyze_" & pBaseFilename & ".xlsx"
        If IO.File.Exists(pFilename) = False Then
            Return JsonDictionary.Serialize("Error", "vb4Z89A8XJVC! File, " & pFilename & ", does not exist.")
        End If

        Dim pError As String
        Dim pDT = ExcelToTable(pFilename, New String() {"Current Invoice Number", "New Invoice Type", "New Invoice Date"}, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If
        pDT.Columns.Add("ResultError")
        pDT.Columns.Add("ResultInvoiceID")
        pDT.Columns.Add("ResultFowstID")
        pDT.Columns.Add("ResultDeviceID")
        pDT.Columns.Add("ResultNewInvoiceTypeID")
        pDT.Columns.Add("ResultCustomer")
        pDT.Columns.Add("ResultInvoiceAmount", GetType(Decimal))

        Dim pErrorCount = 0
        For Each pDR As DataRow In pDT.Rows
            pErrorCount += 1
            Dim pFullNumber = CStr2(pDR("Current Invoice Number"))
            Dim pDrInvoice = WebRequest.Instance.Business.TransInvoice.ByFullNumber(pFullNumber)
            If pDrInvoice Is Nothing Then
                pDR("ResultError") = "No matching record for the specified 'Current Invoice Number'"
                Continue For
            End If
            pDR("ResultInvoiceID") = ReadyGuid(pDrInvoice("ID"))
            pDR("ResultCustomer") = CStr2(pDrInvoice("CustomerName"))
            pDR("ResultFowstID") = CStr2(pDrInvoice("FowstID"))
            pDR("ResultDeviceID") = CStr2(pDrInvoice("DeviceID"))
            pDR("ResultInvoiceAmount") = CDec2(pDrInvoice("InvoiceAmount"))

            If CStr2(pDR("New Invoice Type")).Trim() <> "" Then
                Dim pDrNewInvoiceType = WebRequest.Instance.Business.LibraryDocumentControl.GetByDocumentNameAndModuleSource(CStr2(pDR("New Invoice Type")), "Revenue > Invoicing")
                If pDrNewInvoiceType Is Nothing Then
                    pDR("ResultError") = "No matching record for the specified 'New Invoice Type'"
                    Continue For
                End If
                pDR("ResultNewInvoiceTypeID") = ReadyGuid(pDrNewInvoiceType("ID"))
            End If

            Dim pNewInvoiceTypeText = CStr2(pDR("New Invoice Date"))
            If String.IsNullOrWhiteSpace(pNewInvoiceTypeText) = False Then
                If IsDate(pNewInvoiceTypeText) = False Then
                    pDR("ResultError") = "Invalid date for 'New Invoice Date'"
                    Continue For
                End If
            End If
            pErrorCount -= 1
        Next
        pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChangerAnalyze_" & pBaseFilename & "_analysis.xlsx"
        TableToExcelFile(pDT, "", "", pFilename)

        Dim pOut = New JsonDictionary()
        pOut.Add("BaseFilename", pBaseFilename)
        pOut.Add("ErrorCount", pErrorCount)
        pOut.Add("Result", "ok")
        Return pOut.Serialize()

    End Function

    <HttpPost>
    Public Function InvoiceTypeChangerApply() As String

        If VerifyPassword() = False Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChangerAnalyze_" & WebRequest.Instance.Data("BaseFilename") & "_analysis.xlsx"
        If IO.File.Exists(pFilename) = False Then
            Return JsonDictionary.Serialize("Error", "File, " & pFilename & " not found.")
        End If

        Dim pError As String
        Dim pDT = ExcelToTable(pFilename, New String() {"Current Invoice Number", "New Invoice Type", "New Invoice Date",
            "ResultError",
            "ResultInvoiceID",
            "ResultFowstID",
            "ResultDeviceID",
            "ResultNewInvoiceTypeID",
            "ResultCustomer",
            "ResultInvoiceAmount"
        }, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If
        pDT.Columns.Add("ResultOfApply")

        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("TransInvoice")
            If CStr2(pDR("ResultNewInvoiceTypeID")).Trim() <> "" Then
                Dim pNum As Integer

                WebRequest.Instance.Business.TransInvoice.VerifyCounter(
                    CStr2(pDR("ResultDeviceID")),
                    CStr2(pDR("ResultFowstID")),
                    pNum,
                    CStr2(pDR("ResultNewInvoiceTypeID")))
                pIU("InvoiceNumber") = pNum + 1
                pIU("DocumentNameID") = ReadyString(pDR("ResultNewInvoiceTypeID"))
            End If

            If IsDate(pDR("New Invoice Date")) Then
                pIU("InvoiceDate") = ReadyString(Format(ParseDate(pDR("New Invoice Date")), cGlobal.FormatDateTimeSql))
            End If
            If pIU.Count > 0 Then
                Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ResultInvoiceID")))
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
                WebRequest.Instance.Business.TransInvoice.CacheInvoicesWithCredit(pDR("ResultInvoiceID"))
                Dim pDrResult = WebRequest.Instance.Business.TransInvoice.ByFullNumber("", pDR("ResultInvoiceID"))
                If pDrResult Is Nothing Then
                    pDR("ResultOfApply") = "ID not found"
                Else
                    pDR("ResultOfApply") = CStr2(pDrResult("FullNumber"))
                End If
            Else
                pDR("ResultOfApply") = "Nothing to change here"
            End If
        Next

        pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChanger_" & WebRequest.Instance.Data("BaseFilename") & "_result.xlsx"
        TableToExcelFile(pDT, "", "", pFilename)

        Dim pOut = New JsonDictionary()
        pOut.Add("BaseFilename", WebRequest.Instance.Data("BaseFilename"))
        'pOut.Add("ErrorCount", pErrorCount)
        pOut.Add("Result", "ok")
        Return pOut.Serialize()

    End Function

    Public Function DownloadInvoiceTypeChangerAnalyzeResult() As ActionResult

        Dim pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChangerAnalyze_" & WebRequest.Instance.QueryString("BaseFilename") & "_analysis.xlsx"

        Return File(pFilename, "application/xlsx", "InvoiceTypeChangerAnalyze_" & WebRequest.Instance.QueryString("BaseFilename") & ".xlsx")

    End Function

    Public Function DownloadInvoiceTypeChangerApplyResult() As ActionResult

        Dim pFilename = Common.ServerPath & "temp\InvoiceTypeChanger\InvoiceTypeChanger_" & WebRequest.Instance.QueryString("BaseFilename") & "_result.xlsx"

        Return File(pFilename, "application/xlsx", "InvoiceTypeChangerResult_" & WebRequest.Instance.QueryString("BaseFilename") & ".xlsx")

    End Function

    Function DownloadZRead()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim DownloadDataOnly As Boolean = CBool2(Request.Params.Get("DownloadDataOnly"))

        Dim fowstid As String = Request.Params.Get("FOWSTID")
        Dim deviceid As String = Request.Params.Get("DeviceID")
        Dim datefrom As String = Request.Params.Get("From")
        Dim dateto As String = Request.Params.Get("To")
        Dim GetFromRawData As Boolean = CBool2(Request.Params.Get("GetFromRawData"))
        Dim GenerateZeroSales As String = Request.Params.Get("GenerateZeroSales")
        Dim zread

        If DownloadDataOnly Then
            zread = New ReportPOS().DownloadZRead(DownloadDataOnly,
                                            fowstid, deviceid,
                                            datefrom, dateto,
                                            datefrom, dateto,
                                            True, GetFromRawData,
                                            GenerateZeroSales)

            Return zread
        Else
            Dim ExportType As String = Request.Params.Get("ExportType")
            Dim rawFile
            If ExportType = "period" Or ExportType Is Nothing Then
                zread = New ReportPOS().DownloadZRead(DownloadDataOnly,
                                            fowstid, deviceid,
                                            datefrom, dateto,
                                            datefrom, dateto,
                                            True, GetFromRawData,
                                            GenerateZeroSales)
                rawFile = zread
            Else
                Dim fowstname = Common.WebRequest.ProData.ExecuteScalar("SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(fowstid))
                Dim zipFilename = Common.ServerPath & "temp\ZRead\" & "ZREAD_" & fowstname.Replace(",", "") & "_" & datefrom & "_to_" & dateto & ".zip"
                Dim pZip = New ProZip(zipFilename)

                While CDate(datefrom) <= CDate(dateto)
                    zread = New ReportPOS().DownloadZRead(DownloadDataOnly,
                                            fowstid, deviceid,
                                            datefrom, datefrom,
                                            datefrom, datefrom,
                                            True, GetFromRawData,
                                            GenerateZeroSales)

                    pZip.AddFile(zread)
                    IO.File.Delete(zread)
                    datefrom = CDate(datefrom).AddDays(1).ToString("yyyy-MM-dd")
                End While

                rawFile = zipFilename
            End If
            Dim file As New System.IO.FileInfo(rawFile)
            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/octet-stream"
            Response.WriteFile(file.FullName)
            Response.End()
            Return "zread generated"
        End If
    End Function

    Function DownloadXRead()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pReport = New ProReport
        Dim fowstid As String = Request.Params.Get("FOWSTID")
        Dim userid As String = Request.Params.Get("UserID")
        Dim deviceid As String = Request.Params.Get("DeviceID")
        Dim rangetype As String = Request.Params.Get("RangeType")
        Dim rangefrom As String = Request.Params.Get("RangeFrom")
        Dim rangeto As String = Request.Params.Get("RangeTo")
        Dim rangeyear As String = Request.Params.Get("RangeYear")
        Dim pDtHeader As DataTable = New ReportPOS().getReportHeader(Common.WebRequest.ProData, fowstid, userid, deviceid)
        Dim dtaDSaleTable As DataTable = New ReportPOS().getCashierSalesTransactionSummary(userid, rangetype, rangefrom, rangeto, rangeyear, fowstid, deviceid)
        Dim dtDiscounts As DataTable = New MallLessorMegaworld().getDailyDiscountBreakdown(Common.WebRequest.ProData, CDate(rangefrom), CDate(rangeto), fowstid, deviceid, userid)

        Dim dRowSales = dtaDSaleTable.Rows(0)
        Dim netsales As String = New MallLessorMegaworld().Roundoff(dRowSales("CurGrossSales") - (dRowSales("CurSalesDiscount") + dRowSales("CurSCDiscount")) - dRowSales("CurSalesReturnRefund") - dRowSales("CurSalesReturnExchange") - dRowSales("CurVATExempt") + dRowSales("CurServiceCharge"))
        Dim scdiscvatablesales As Decimal = 0, scdiscvatableamount As Decimal = 0

        If CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) > 0 Then
            scdiscvatablesales = CDec2(CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) / 1.12)
            scdiscvatableamount = CDec2(CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) - scdiscvatablesales)
        End If

        dtDiscounts.Columns.Add("Code")
        If dtDiscounts.Rows.Count > 0 Then
            For Each disc As Object In dtDiscounts.Rows
                Dim code As String = ""

                If disc("DiscountTypeID").ToString().ToLower() = "35f954a0-c67c-499c-af60-f08e64477fdc" Then
                    code = "VIPDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "8e2dca9c-433f-42fc-a644-04476dba056a" Then
                    code = "REGDISC3"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "c4199347-6a00-45c9-be11-8934dd63ecd1" Then
                    code = "REGDISC2"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "764edacb-0131-4033-8e44-7b903368008c" Then
                    code = "REGDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "7804a35a-b531-4380-945e-fc9643e99843" Then
                    code = "EMPDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "055a0a33-dd98-47ec-add3-8b2009f3944a" Then
                    code = "SENDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "34a4032f-a142-4c15-a868-4ac4410434df" Then
                    code = "PWDDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "674ad791-188b-4238-b95c-97b5c2b9d544" Then
                    code = "SPESENDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "c2218c86-0450-4cd3-b019-3edd43e51741" Then
                    code = "GPCDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "21ebf55b-526c-4276-b3b7-0cbca504f425" Then
                    code = "NAACDISC"
                ElseIf disc("DiscountTypeID").ToString().ToLower() = "e95f0b64-1eea-47b4-bde5-b795234b462d" Then
                    code = "SOLOPARENTDISC"
                Else
                    code = disc("DiscountType")
                End If
                disc("Code") = code
            Next
        End If

        dtaDSaleTable.Columns.Add("TotalVATType")
        dtaDSaleTable.Columns.Add("VATableSales")
        dtaDSaleTable.Columns.Add("VATAmount")
        dtaDSaleTable.Columns.Add("NonVAT")
        dtaDSaleTable.Columns.Add("VATExempt")
        dtaDSaleTable.Columns.Add("ZeroRatedVAT")

        dtaDSaleTable.Rows(0).Item("TotalVATType") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSales")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSalesSalesReturn"))) + (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmount") + scdiscvatableamount) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmountSalesReturn"))) + CDec2(dtaDSaleTable.Rows(0).Item("CurNoTAXSales")) + CDec2(dtaDSaleTable.Rows(0).Item("CurVATExemptSales")), netsales)
        dtaDSaleTable.Rows(0).Item("VATableSales") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSales")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSalesSalesReturn")), 0)
        dtaDSaleTable.Rows(0).Item("VATAmount") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmount")) + scdiscvatableamount) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmountSalesReturn")), 0)
        dtaDSaleTable.Rows(0).Item("NonVAT") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurNoTAXSales")), netsales)
        dtaDSaleTable.Rows(0).Item("VATExempt") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurVATExemptSales")), 0)
        dtaDSaleTable.Rows(0).Item("ZeroRatedVAT") = 0

        pReport.SourceFilename = "XRead"
        pReport.AddTable("Header", pDtHeader)
        pReport.AddTable("ReportPOS", dtaDSaleTable)
        pReport.AddTable("OtherPayments", New MallLessorMegaworld().getOtherPaymentTypes(Common.WebRequest.ProData, CDate(rangefrom), CDate(rangeto), fowstid, deviceid, userid))
        pReport.AddTable("Discounts", dtDiscounts)
        pReport.ExportXmlFile()

        Dim fowstname = Common.WebRequest.ProData.ExecuteScalar("SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(fowstid))
        Dim zreadname = "XREAD_of_" & Regex.Replace(dtaDSaleTable.Rows(0).Item("CashierName"), "[^A-Za-z0-9]", "_") & "_Store-" & fowstname.Replace(",", "") & "_from_" & rangefrom & " - " & rangeto & ".pdf"
        Dim path = Common.ServerPath & "temp\" & CreateValidFilename(zreadname, True)
        pReport.ExportFile(ProReport.eExportFormat.Pdf, path)

        Dim file As New System.IO.FileInfo(path)
        Response.Clear()
        Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
        Response.AddHeader("Content-Length", file.Length.ToString())
        Response.ContentType = "application/octet-stream"
        Response.WriteFile(file.FullName)
        Response.End()
        Return "zread generated"
    End Function

    Function GetCacheZReadData(ByVal mProdata As ProData, ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing)

        Dim pSql As String

        pSql = " SELECT *, '" & IIf(dtDateFrom = dtDateTo, dtDateFrom.ToString("MM/dd/yyyy"), dtDateFrom.ToString("MM/dd/yyyy") & " - " & dtDateTo.ToString("MM/dd/yyyy")) & "' as TransDate " & vbCrLf &
                " , CurSCDiscount as TotalSCDiscount " & vbCrLf &
                " , OtherTypeAmount as OtherPaymentAmount " & vbCrLf &
                " , '0' as CurPotatoCornerGC " & vbCrLf &
                " , '0' as PrevTotalSCDiscount " & vbCrLf &
                " , '0' as PrevEndInvoiceNo " & vbCrLf &
                " FROM TransZReadCache " & vbCrLf &
                " WHERE DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
                " AND FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                " AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf

        Return mProdata.Execute(pSql)

    End Function

    <HttpPost>
    Public Function CreateReceiptPdfs() As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOrgID As String

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            pOrgID = Common.WebRequest.Data("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFowstID = Common.WebRequest.Data("FowstID")
            Dim pInvoiceDateFrom = CDate(Common.WebRequest.Data("InvoiceDateFrom")).Date
            Dim pInvoiceDateTo = CDate(Common.WebRequest.Data("InvoiceDateTo")).Date

            pSql = "SELECT TransInvoice.ID" & vbCrLf &
                ",TransInvoice.InvoiceNumber" & vbCrLf &
                ",TransInvoice.InvoiceDate" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Format(pInvoiceDateFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(pInvoiceDateTo, cGlobal.FormatDateTimeSql)) & vbCrLf &
                " AND TransInvoice.FowstID = " & ReadyString(pFowstID) & vbCrLf &
                " AND TransInvoice.InvoiceNumber > 0" & vbCrLf &
                " AND ISNULL(TransInvoice.TransType, 'Invoice') = 'Invoice'" & vbCrLf &
                " ORDER BY TransInvoice.InvoiceDate"
            Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
            Dim pTimeStamp = DateTimeStamp()
            Dim pFolder = Common.ServerPath & "temp\CreateReceiptPdfs\" & pTimeStamp & "\"
            Dim pExcelFilename = pFolder & "List.xlsx"
            CreateFolder(pExcelFilename)
            TableToExcelFile(pDT, "", "", pExcelFilename)

            Dim pFilename As String
            Dim pFilenames = New List(Of String)
            For Each pDR As DataRow In pDT.Rows
                Dim pInvoiceID = CStr2(pDR("ID"))
                Dim pPdfResult = WebRequest.Instance.Business.TransInvoice.CreatePdf(vInvoiceID:=pInvoiceID,
                    vOrgID:=WebRequest.Instance.OrgID,
                    vTemplate:="POSReceipt",
                    vAllowReprint:=True,
                    vPrintBlindCopy:=False,
                    vPrintAsDraft:=False,
                    vUuid:="CreateReceiptPdfs_UUID",
                    vTransType:="Invoice")
                For Each pBaseFilename In pPdfResult.BaseFilenames
                    pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
                    Dim pDestinationFilename = pFolder & pDR("InvoiceNumber") & ".pdf"
                    If System.IO.File.Exists(pDestinationFilename) Then
                        pDestinationFilename = pFolder & pDR("InvoiceNumber") & "-" & DateTimeStamp() & ".pdf"
                    End If
                    MoveFile(pFilename, pDestinationFilename)
                    pFilenames.Add(pDestinationFilename)
                Next
            Next
            pFilename = pFolder & "ReceiptPdfs_" & pTimeStamp & ".zip"
            Dim pZip = New ProZip(pFilename)
            pZip.AddFile(pExcelFilename)
            For Each pFilename In pFilenames
                pZip.AddFile(pFilename)
            Next

            Return JsonDictionary.Serialize("Result", pTimeStamp)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Public Function DownloadReceiptPdfs() As ActionResult

        Dim pTimestamp = WebRequest.Instance.QueryString("TimeStamp")
        Dim pFilename = Common.ServerPath & "temp\CreateReceiptPdfs\" & pTimestamp & "\ReceiptPdfs_" & pTimestamp & ".zip"

        Return File(pFilename, "application/zip", "ReceiptPdfs_" & pTimestamp & ".zip")

    End Function

    Shared ReadOnly mSemaphoreCacheInventoryMovement As New SemaphoreSlim(1)
    <HttpPost>
    Public Function CacheInventoryMovement() As String

        Dim pOrgID As String

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another CacheInventoryMovement process is running. ref: vbZXEM8Z3D33!")
        End If

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            pOrgID = Common.WebRequest.Data("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)

            Dim pTransactionDateFrom = CDate(Common.WebRequest.Data("InvoiceDateFrom")).Date
            Dim pTransactionDateTo = CDate(Common.WebRequest.Data("InvoiceDateTo")).Date
            Dim pTransactionDate = pTransactionDateFrom

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pAffectedRecords As Integer
            Do
                If pTransactionDateTo < pTransactionDate Then
                    Exit Do
                End If
                pAffectedRecords = WebRequest.Instance.Business.CacheInventoryMovement.CacheData(pTransactionDate)
                pTransactionDate = pTransactionDate.AddDays(1)
            Loop

            Dim pOut = New JsonDictionary()
            pOut.Add("Result", "ok")
            pOut.Add("AffectedRecords", pAffectedRecords)
            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            mSemaphoreCacheInventoryMovement.Release()
        End Try

    End Function

    <HttpPost>
    Public Function FranchiseeDataExporter() As String

        Dim pOrgID As String

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vbA7FCTJX6NB!")
        End If

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            pOrgID = Common.WebRequest.Data("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)

            Dim pTransactionDateFrom = CDate(Common.WebRequest.Data("InvoiceDateFrom")).Date
            Dim pTransactionDateTo = CDate(Common.WebRequest.Data("InvoiceDateTo")).Date
            Dim pDontStart = CBool2(WebRequest.Instance.Data("DontStart"))
            Dim pDetectInvoicesWithNoItems = CBool2(WebRequest.Instance.Data("DetectInvoicesWithNoItems"))
            Dim pFowstID = CStr2(Common.WebRequest.Data("FranchiseeFowstID")) ' optional. dont use "FowstID"
            Dim pStartFresh = CBool2(WebRequest.Instance.Data("StartFresh"))
            Dim pForceFranchiseeDatabaseID = CStr2(WebRequest.Instance.Data("ForceFranchiseeDatabaseID"))
            Dim pForceFranchisorDatabaseID = CStr2(WebRequest.Instance.Data("ForceFranchisorDatabaseID"))

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDtOrgs As DataTable
            If String.IsNullOrWhiteSpace(pForceFranchiseeDatabaseID) = False _
                OrElse String.IsNullOrWhiteSpace(pForceFranchisorDatabaseID) = False Then
                pDtOrgs = WebRequest.Instance.Business.LibraryOrganization.GetAll(True)
            End If
            Dim pForceFranchiseeDatabase As String
            Dim pForceFranchisorDatabase As String
            If pDtOrgs IsNot Nothing Then
                pForceFranchiseeDatabase = pDtOrgs.Select("ID = " & ReadyString(pForceFranchiseeDatabaseID)).FirstOrDefault()?.Item("Database")
                pForceFranchisorDatabase = pDtOrgs.Select("ID = " & ReadyString(pForceFranchisorDatabaseID)).FirstOrDefault()?.Item("Database")
            End If

            Try
                Dim pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pDR = New Business(pProData).LibraryOrganization.ByID(pOrgID)
                pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
                TimedThread(
                    Sub()
                        Try
                            'Dim pFranchiseeDataExporter = New FranchiseeDataExporter(pProData) ' new
                            'WebProj.Data.FranchiseeDataExporter.ErrorMessage = ""
                            'pFranchiseeDataExporter.StartDate = pTransactionDateFrom
                            'pFranchiseeDataExporter.EndDate = pTransactionDateTo
                            'pFranchiseeDataExporter.DetectInvoicesWithNoItems = pDetectInvoicesWithNoItems
                            'pFranchiseeDataExporter.FowstID = pFowstID
                            'pFranchiseeDataExporter.StartFresh = pStartFresh
                            'pFranchiseeDataExporter.ForceFranchiseeDatabase = pForceFranchiseeDatabase
                            'pFranchiseeDataExporter.ForceFranchisorDatabase = pForceFranchisorDatabase
                            'pFranchiseeDataExporter.CopyRecords(pOrgID)
                        Catch ex As Exception
                            WebProj.Data.FranchiseeDataExporter.ErrorMessage = ex.Message
                            AppendToDebug("FranchiseeDataExporterStart - vbRHR6XUPT99! " & vbCrLf & ex.Message, True, ex, True)
                        End Try
                    End Sub
                )
                'Return "maybe ok " & DateTimeStamp()
            Catch ex As Exception
                Return ex.Message & " " & DateTimeStamp()
            End Try

            Dim pOut = New JsonDictionary()
            pOut.Add("Result", "ok")
            pOut.Add("AffectedRecords", -2)
            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            mSemaphoreCacheInventoryMovement.Release()
        End Try

    End Function

    Function RecacheZread()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim fowstid As String = Common.WebRequest.Data("LocationID")
        Dim zdatefrom As Date = CDate(Common.WebRequest.Data("DateFrom"))
        Dim zdateto As Date = CDate(Common.WebRequest.Data("DateTo"))
        Dim deviceid As String = ""

        Try
            Dim activeFOWST As DataTable = Common.WebRequest.ProData.Execute(" SELECT DISTINCT FOWSTID FROM TransZReadCache " & vbCrLf &
                                                                               " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = FOWSTID and fowst.IsActive = 1 " & vbCrLf &
                                                                               " GROUP BY FOWSTID " & vbCrLf)

            For Each fowst As Object In activeFOWST.Rows
                fowstid = fowst("FOWSTID").ToString()
                Dim FOWSTDeviceID As DataTable = Common.WebRequest.ProData.Execute(" SELECT DeviceID " & vbCrLf &
                                                                               " FROM TransZReadCache " & vbCrLf &
                                                                               " INNER JOIN LibraryDevice as dev ON dev.ID = DeviceID AND dev.IsActive = 1 " & vbCrLf &
                                                                               " WHERE FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                                                                               " GROUP BY DeviceID " & vbCrLf)

                For Each device As Object In FOWSTDeviceID.Rows
                    'Dim cachezreaddate As DataTable = Common.WebRequest.ProData.Execute(" SELECT CONVERT(date, SalesDate) as SalesDate, CurTotalVatablePortionOfSCDTransaction, DiscountRefund " & vbCrLf &
                    '                                                               " FROM TransZReadCache " & vbCrLf &
                    '                                                               " INNER JOIN LibraryDevice as dev ON dev.ID = DeviceID AND dev.IsActive = 1 " & vbCrLf &
                    '                                                               " WHERE FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                    '                                                               " AND DeviceID = " & ReadyString(deviceID("DeviceID").ToString()) & vbCrLf &
                    '                                                               " GROUP BY CONVERT(date, SalesDate), CurTotalVatablePortionOfSCDTransaction, DiscountRefund " & vbCrLf &
                    '                                                               " ORDER BY CONVERT(date, SalesDate) ASC " & vbCrLf)
                    zdatefrom = CDate(Common.WebRequest.Data("DateFrom"))
                    deviceid = device("DeviceID").ToString()

                    While zdatefrom <= zdateto
                        Dim CurTotalVatablePortionOfSCDTransaction As String = Common.WebRequest.ProData.ExecuteScalar("SELECT ISNULL(SUM(inv.Vatable), 0) as Vatable " &
                                   " FROM TransCustomerPaymentInvoices custpayinv " &
                                   " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                   " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                   " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                                   " AND CONVERT(date, inv.InvoiceDate) = " & ReadyString(zdatefrom) &
                                   " AND inv.FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                                   " AND inv.DeviceID = " & ReadyString(deviceid))

                        Dim DiscountRefund As String = Common.WebRequest.ProData.ExecuteScalar("SELECT ISNULL(SUM(inv.SCDiscount ), 0) as Total " &
                                   " FROM TransSalesReturnLedger as ledger " &
                                   " INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " &
                                   " INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID  " &
                                   " INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " &
                                   " INNER JOIN TransInvoice inv ON (cuspayinv.InvoiceID = inv.ID)  " &
                                   " WHERE ISNULL(RefundNum, 0) > 0 AND CONVERT(date, ledger.LastUpdated) = " & ReadyString(zdatefrom) &
                                   " AND ledger.FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                                   " AND ledger.DeviceID = " & ReadyString(deviceid))

                        Dim OtherDiscount As String = Common.WebRequest.ProData.ExecuteScalar("SELECT ISNULL(SUM(custpay.DiscountAmount), 0) as Total " +
                                " FROM TransCustomerPaymentInvoices custpayinv " +
                                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                                " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                                " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                                " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df') -- Senior, PWD " & vbCrLf &
                                " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL " &
                                " AND CONVERT(date, inv.InvoiceDate) = " & ReadyString(zdatefrom) &
                                " AND inv.FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                                " AND inv.DeviceID = " & ReadyString(deviceid))

                        Dim SCDiscount As String = Common.WebRequest.ProData.ExecuteScalar("SELECT ISNULL(SUM(inv.SCDiscount ), 0) as Total " &
                                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                                    " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                                    " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                                    " WHERE CONVERT(date, inv.InvoiceDate) = " & ReadyString(zdatefrom) & vbCrLf &
                                    " AND (LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount' " & vbCrLf &
                                    " OR LibraryDiscountSettings.DiscountType = 'Person With Disability Discount') " & vbCrLf &
                                    " AND inv.FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                                    " AND inv.DeviceID = " & ReadyString(deviceid))

                        Dim p As New InsertUpdate("TransZReadCache")

                        p("CurTotalVatablePortionOfSCDTransaction") = CDec2(CurTotalVatablePortionOfSCDTransaction)

                        p("DiscountRefund") = CDec2(DiscountRefund)

                        p("CurTotalDiscount") = CDec2(OtherDiscount)

                        p("CurSCDiscount") = CDec2(SCDiscount)

                        Common.WebRequest.ProData.Execute(p.GetUpdate(" CONVERT(date, SalesDate) = " & ReadyString(zdatefrom) &
                                   " AND FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                                   " AND DeviceID = " & ReadyString(deviceid)))

                        zdatefrom = zdatefrom.AddDays(1)
                    End While
                Next
            Next

            Return JsonDictionary.Serialize("Result", "success")
        Catch ex As Exception
            AppendToDebug(" Recache ZRead CurTotalVatablePortionOfSCDTransaction column " & vbCrLf &
                        " Date: " & ReadyString(zdatefrom) & vbCrLf &
                        " FOWSTID: " & ReadyString(fowstid) & vbCrLf &
                        " DeviceID: " & ReadyString(deviceid))
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function CheckBegEndTrans()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String

            pSql = "SELECT fowst.ID as FowstID, DeviceID, UUID, Code as StoreCode, fowst.Name, 1 as FirstInvoiceNo, MIN(InvoiceDate) as FirstInvoiceDate, MAX(InvoiceNumber) as LastInvoiceNo, MAX(InvoiceDate) as LastInvoiceDate --, MAX(DateCreated) as LastInvoiceReceivedOnServer " & vbCrLf &
                    "FROM TransInvoice " & vbCrLf &
                    " Inner Join MobizLibraryOrganizationFowst as fowst ON fowst.ID = fowstid" & vbCrLf &
                    " Inner Join LibraryDevice as device ON device.ID = DeviceID" & vbCrLf &
                    " WHERE FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                    " AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & vbCrLf &
                    " GROUP BY fowst.ID, DeviceID, UUID, Code, fowst.Name"

            Return JsonDictionary.Serialize("Result", Common.WebRequest.ProData.Execute(pSql))
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function RecacheZReadByInvoiceID()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim mBusiness As Business = New Business(Common.WebRequest.ProData)
            Dim importdatac As New ImportDataClass(mBusiness)

            importdatac.UpdateZReadCache(Common.WebRequest.Data("InvoiceID"))

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function DownloadNagaEJournal()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim fowstid As String = Request.Params.Get("FOWSTID")
            Dim deviceid As String = Request.Params.Get("DeviceID")
            Dim datefrom As String = CDate(Request.Params.Get("From")).ToString("MM-dd-yyyy")
            Dim dateto As String = CDate(Request.Params.Get("To")).ToString("MM-dd-yyyy")
            Dim ForceNONVAT As String = Request.Params.Get("ForceNONVAT")
            Dim ExcludeSales As Boolean = CBool2(Request.Params.Get("ExcludeSales"))
            Dim ExcludeSC As Boolean = CBool2(Request.Params.Get("ExcludeSC"))
            Dim ExcludePWD As Boolean = CBool2(Request.Params.Get("ExcludePWD"))
            Dim ExcludeNAAC As Boolean = CBool2(Request.Params.Get("ExcludeNAAC"))
            Dim ExcludeSP As Boolean = CBool2(Request.Params.Get("ExcludeSP"))
            Dim DownloadDataOnly As String = Request.Params.Get("DownloadDataOnly")
            Dim fileformat As String = Request.Params.Get("Format")
            Dim filepath As String
            Dim mFilenameFolder = Common.ServerPath & "temp\EJournal\" & WebRequest.Instance().OrgID

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            Dim pOut As New JsonDictionary
            Dim pReport = New ProReport
            pReport.SourceFilename = "NagaEJournal"
            Common.WebRequest.ProData.CommandTimeout = 600000

            Dim pSql As String

            pSql = " SELECT ISNULL(dev.UuidDisplay, dev.UUID) as SerialNo, " & vbCrLf &
                    " devfow.POSRegistrationNo, " & vbCrLf &
                    " devfow.BIRMachineNo, " & vbCrLf &
                    " ISNULL(BusinessName, (SELECT TOP 1 BusinessTradeName FROM MobizLibraryOrganizationTradeName WHERE OrganizationID = fowst.OrganizationID)) as BusinessName, " & vbCrLf &
                    " ISNULL(fowst.Code, '') + ' ' + ISNULL(fowst.Name, '') as StoreName, " & vbCrLf &
                    IIf(ForceNONVAT, "CONVERT(bit, 'False')", "ISNULL(fowst.IsVAT, CONVERT(bit, 'True'))") & " as IsVAT " & vbCrLf &
                    " , ISNULL(NULLIF(LTRIM(RTRIM(fowst.StreetNumber)) + ' ', ' '), '') + " & vbCrLf &
                    "   ISNULL(NULLIF(LTRIM(RTRIM(fowst.Street)) + ', ',', '), '') + " & vbCrLf &
                    "   ISNULL(NULLIF(LTRIM(RTRIM(subcrcty.Name)) + ', ',', '), '') + " & vbCrLf &
                    "   ISNULL(NULLIF(LTRIM(RTRIM(subcrctry.Name)) + ', ',', '), '') as StoreAddress " & vbCrLf &
                    ", ISNULL((SELECT TinDisplay FROM LibraryDevice WHERE ID = devfow.DeviceID), subcr.TinNumber + '-00' + RIGHT(fowst.TINBranchCode, 3)) as TIN " & vbCrLf &
                    " FROM LibraryDevice as dev " & vbCrLf &
                    " INNER JOIN LibraryDeviceFowst as devfow ON devfow.DeviceID = dev.ID " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON devfow.FowstID = fowst.ID " & vbCrLf &
                    " INNER JOIN dbo.MobizLibraryOrganization subcr ON (fowst.OrganizationID = subcr.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCities subcrcty ON (fowst.FowstCityID = subcrcty.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCountries subcrctry ON (fowst.FowstCountryID = subcrctry.ID) " & vbCrLf &
                    " WHERE dev.ID = " & ReadyString(deviceid) & vbCrLf &
                    " AND devfow.FowstID = " & ReadyString(fowstid) & vbCrLf
            Dim header As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Dim filename As String = header(0)("StoreName") & "_Sales Summary_" & datefrom & "_to_" & dateto & "_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".pdf"
            filepath = Common.ServerPath & "temp\EJournal\" & CStr(WebRequest.Instance.OrgID) & "\" & filename

            pReport.AddTable("Header", header)
            pOut.Add("Header", header)

#Region "Details"
            'pSql = " IF OBJECT_ID('tempdb..##ESales_TransInvoice') IS NOT NULL DROP TABLE ##ESales_TransInvoice " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransInvoice FROM TransInvoice WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf &
            '        " AND FOWSTID = " & ReadyString(fowstid) & " AND DeviceID = " & ReadyString(deviceid) & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransInvoiceItems') IS NOT NULL DROP TABLE ##ESales_TransInvoiceItems " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransInvoiceItems FROM TransInvoiceItems WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransCustomerPaymentInvoices') IS NOT NULL DROP TABLE ##ESales_TransCustomerPaymentInvoices " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransCustomerPaymentInvoices FROM TransCustomerPaymentInvoices WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransCustomerPayment') IS NOT NULL DROP TABLE ##ESales_TransCustomerPayment " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransCustomerPayment FROM TransCustomerPayment WHERE ID IN (SELECT CustomerPaymentID FROM ##ESales_TransCustomerPaymentInvoices) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransCustomerPaymentOther') IS NOT NULL DROP TABLE ##ESales_TransCustomerPaymentOther " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransCustomerPaymentOther FROM TransCustomerPaymentOther WHERE CustomerPaymentID IN (SELECT CustomerPaymentID FROM ##ESales_TransCustomerPaymentInvoices) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransInvoiceDiscount') IS NOT NULL DROP TABLE ##ESales_TransInvoiceDiscount " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransInvoiceDiscount FROM TransInvoiceDiscount WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransMemoSalesReturn') IS NOT NULL DROP TABLE ##ESales_TransMemoSalesReturn " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransMemoSalesReturn FROM TransMemoSalesReturn WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransMemo') IS NOT NULL DROP TABLE ##ESales_TransMemo " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransMemo FROM TransMemo WHERE ID IN (SELECT MemoID FROM ##ESales_TransMemoSalesReturn) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransSalesReturnLedger') IS NOT NULL DROP TABLE ##ESales_TransSalesReturnLedger " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransSalesReturnLedger FROM TransSalesReturnLedger WHERE SourceMemoID IN (SELECT MemoID FROM ##ESales_TransMemoSalesReturn) " & vbCrLf &
            '        "" & vbCrLf &
            '        " IF OBJECT_ID('tempdb..##ESales_TransInvoiceSeniorCitizenPWDCustomer') IS NOT NULL DROP TABLE ##ESales_TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
            '        " SELECT * INTO ##ESales_TransInvoiceSeniorCitizenPWDCustomer FROM TransInvoiceSeniorCitizenPWDCustomer WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
            '        "" & vbCrLf

            pSql = "IF OBJECT_ID('tempdb..##ESales_VATExPerDisc') IS NOT NULL DROP TABLE ##ESales_VATExPerDisc " & vbCrLf &
                " SELECT CONVERT(date, memo.LastUpdated) as MemoDate, SUM(ROUND(items.UnitPrice - (items.UnitPrice * memosr.QuantityReturned) / 1.12, 2)) as VATEx, discset.DiscountType, discset.ID as DiscountTypeID " & vbCrLf &
                "   INTO ##ESales_VATExPerDisc " & vbCrLf &
                "   FROM TransMemoSalesReturn memosr " & vbCrLf &
                "   INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                "   INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID " & vbCrLf &
                "   INNER JOIN TransInvoiceItems as items ON items.InvoiceID = sub_inv.ID AND items.ProductID = memosr.ProductID " & vbCrLf &
                "   INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                "   INNER JOIN LibraryDiscountSettings as discset ON discset.ID = disc.DiscountTypeID " & vbCrLf &
                "   WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf &
                "   AND sub_inv.FowstID = " & ReadyString(fowstid) & " AND sub_inv.DeviceID = " & ReadyString(deviceid) & vbCrLf &
                "   GROUP BY discset.DiscountType, discset.ID, CONVERT(date, memo.LastUpdated) " & vbCrLf &
                " " & vbCrLf &
                "SELECT DISTINCT CONVERT(date, inv.InvoiceDate) as InvoiceDate" & vbCrLf &
                ", ISNULL((SELECT SUM(ROUND(invitem.GrossSalesNetOfVATExemptions, 2))" & vbCrLf &
                "FROM TransInvoiceItems invitem" & vbCrLf &
                "INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID)" & vbCrLf &
                "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) as GrossSales" & vbCrLf &
                ", ISNULL((SELECT TOP 1 MIN(inv.InvoiceNumber)" & vbCrLf &
                "FROM TransInvoice sub_inv" & vbCrLf &
                "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) as BegInvoiceNumber" & vbCrLf &
                ", ISNULL((SELECT TOP 1 MAX(inv.InvoiceNumber)" & vbCrLf &
                "FROM TransInvoice sub_inv" & vbCrLf &
                "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) as EndInvoiceNumber" & vbCrLf &
                ", (ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0))" & vbCrLf &
                "FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                "INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                "INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) +" & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0))" & vbCrLf &
                "FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                "INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                "INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                "WHERE CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0)) " & vbCrLf &
                " - " & vbCrLf &
                " ISNULL((SELECT ISNULL(SUM(ROUND(ledger.Amount, 2)), 0) as Total " & vbCrLf &
                " FROM TransSalesReturnLedger as ledger " & vbCrLf &
                " INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " & vbCrLf &
                " INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " & vbCrLf &
                " LEFT JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (cuspayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE ledger.FowstID = inv.FowstID AND ledger.DeviceID = inv.DeviceID AND ISNULL(RefundNum, 0) > 0 " & vbCrLf &
                " AND CONVERT(date, ledger.LastUpdated) = CONVERT(date, inv.InvoiceDate)) " & vbCrLf &
                " , 0) " & vbCrLf &
                " as TotalPayments" & vbCrLf &
                " , ISNULL((SELECT DISTINCT SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                " AND libdisc.ID = '055a0a33-dd98-47ec-add3-8b2009f3944a' " & vbCrLf &
                " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                " ,0) as SCAmount " & vbCrLf &
                " , ISNULL((SELECT DISTINCT SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                " AND libdisc.ID = '34a4032f-a142-4c15-a868-4ac4410434df' " & vbCrLf &
                " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                " ,0) as PWDAmount " & vbCrLf &
                " , ISNULL((SELECT DISTINCT SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                " AND libdisc.ID = '21ebf55b-526c-4276-b3b7-0cbca504f425' " & vbCrLf &
                " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                " ,0) as NAACAmount " & vbCrLf &
                " , ISNULL((SELECT DISTINCT SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                " AND libdisc.ID = 'e95f0b64-1eea-47b4-bde5-b795234b462d' " & vbCrLf &
                " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                " ,0) as SoloParentAmount " & vbCrLf &
                " , ISNULL((SELECT DISTINCT SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                " AND libdisc.ID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df', '21ebf55b-526c-4276-b3b7-0cbca504f425', 'e95f0b64-1eea-47b4-bde5-b795234b462d') -- SC, PWD, NAAC, SoloParent " & vbCrLf &
                " --GROUP BY disc.DiscountTypeID, libdisc.DiscountType " & vbCrLf &
                " ),0) as OtherDiscountAmount " & vbCrLf &
                " , 0 as CreditSales -- TO DO - 0 by default para maka generate lang og report si PC, since walay credit sales si PC " & vbCrLf &
                " , 0 as ZeroRated " & vbCrLf &
                " , ISNULL((SELECT SUM(ROUND(sub_inv.ForegoneVAT, 2)) FROM TransCustomerPaymentInvoices custpayinv INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) WHERE sub_inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) AND sub_inv.FOWSTID = inv.FowstID AND sub_inv.DeviceID = inv.DeviceID),0) as VATExemptTax" & vbCrLf &
                " , ISNULL((SELECT SUM(ROUND(sub_inv.VATExempt, 2) " & IIf(WebRequest.Instance.OrgID.ToLower() = "d362ffdb-15d1-451f-97dc-c567b94fb410" Or WebRequest.Instance.OrgID.ToLower() = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba", "- ROUND(sub_inv.SCDiscount, 2)", "") & ") " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                " ,0) " & vbCrLf &
                " - " & vbCrLf &
                " ISNULL((SELECT SUM(ROUND((items.UnitPrice * memosr.QuantityReturned) / 1.12, 2)) " & vbCrLf &
                " FROM TransMemoSalesReturn memosr " & vbCrLf &
                " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                " INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID " & vbCrLf &
                " INNER JOIN TransInvoiceItems as items ON items.InvoiceID = sub_inv.ID AND items.ProductID = memosr.ProductID " & vbCrLf &
                " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND inv.FOWSTID = sub_inv.FowstID  AND inv.DeviceID = sub_inv.DeviceID " & vbCrLf &
                " AND (ISNULL(sub_inv.SCDiscount, 0) > 0 OR ISNULL(sub_inv.SoloParentDiscount, 0) > 0)) ,0) " & vbCrLf &
                " as VATExempt " & vbCrLf &
                " , ISNULL((SELECT SUM(ROUND(sub_inv.Vatable, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND sub_inv.ForegoneVAT>0 " & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                " ,0) + " & vbCrLf &
                " ISNULL((SELECT ROUND(SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)), 2) " & vbCrLf &
                " FROM TransInvoice sub_inv " & vbCrLf &
                " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  --AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                " AND (ISNULL(sub_inv.SCDiscount, 0) = 0 AND ISNULL(sub_inv.SoloParentDiscount, 0) = 0) " & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                " ,0) - " & vbCrLf &
                "ISNULL((SELECT ROUND(SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)), 2)" & vbCrLf &
                "FROM TransMemoSalesReturn memosr" & vbCrLf &
                "INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID)" & vbCrLf &
                "INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID" & vbCrLf &
                "WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                "AND memo.DeviceID = inv.DeviceID AND (ISNULL(sub_inv.SCDiscount, 0) = 0 AND ISNULL(sub_inv.SoloParentDiscount, 0) = 0))" & vbCrLf &
                ",0) as VatableSales" & vbCrLf &
                " , ISNULL((SELECT ROUND(SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)), 2) " & vbCrLf &
                " FROM TransInvoice sub_inv " & vbCrLf &
                " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                " and ISNULL(sub_inv.SCDiscount, 0) = 0 AND ISNULL(sub_inv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) + " & vbCrLf &
                " ISNULL((SELECT SUM(ROUND(sub_inv.VAT, 2)) " & vbCrLf &
                " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN dbo.TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)  " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND sub_inv.ForegoneVAT>0 " & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) - " & vbCrLf &
                " ISNULL(tblSalesReturnVATAmount.VATAmount, 0) as VATAmount" & vbCrLf &
                ", ISNULL(tblSalesReturnVATAmount.VATAmount, 0) as VATOnReturns" & vbCrLf &
                ", ISNULL((SELECT VATEx FROM ##ESales_VATExPerDisc WHERE DiscountTypeID = '055A0A33-DD98-47EC-ADD3-8B2009F3944A' AND MemoDate = CONVERT(date, inv.InvoiceDate)), 0) as SCForegoneVat " & vbCrLf &
                ", ISNULL((SELECT VATEx FROM ##ESales_VATExPerDisc WHERE DiscountTypeID = '34a4032f-a142-4c15-a868-4ac4410434df' AND MemoDate = CONVERT(date, inv.InvoiceDate)), 0) as PWDForegoneVat " & vbCrLf &
                ", ISNULL((SELECT VATEx FROM ##ESales_VATExPerDisc WHERE DiscountTypeID = 'e95f0b64-1eea-47b4-bde5-b795234b462d' AND MemoDate = CONVERT(date, inv.InvoiceDate)), 0) as SoloParentForegoneVat " & vbCrLf &
                ", (ISNULL((SELECT SUM(ROUND(invitem.GrossSalesNetOfVATExemptions, 2))" & vbCrLf &
                " FROM TransInvoiceItems invitem" & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID)" & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) -" & vbCrLf &
                "(ISNULL((SELECT SUM(ROUND(custpay.DiscountAmount, 2))" & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID" & vbCrLf &
                " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df') -- Senior, PWD" & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL" & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) +" & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(sub_inv.SCDiscount, 2),0))" & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID" & vbCrLf &
                " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID" & vbCrLf &
                " WHERE CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                " AND (LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount'" & vbCrLf &
                " OR LibraryDiscountSettings.DiscountType = 'Person With Disability Discount')" & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0)" & vbCrLf &
                ") -" & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.GrossRefundAmount, 2), 0))" & vbCrLf &
                " FROM TransSalesReturnLedger srledger" & vbCrLf &
                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                " WHERE CONVERT(date, memo.LastUpdated) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                " AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                " AND memo.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) -" & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.GrossSrvAmount, 2), 0))" & vbCrLf &
                " FROM TransSalesReturnLedger srledger" & vbCrLf &
                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                " WHERE CONVERT(date, memo.LastUpdated) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                " AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                " AND memo.DeviceID = inv.DeviceID)" & vbCrLf &
                ",0) -" & vbCrLf &
                "ISNULL((SELECT SUM(ROUND(sub_inv.ForegoneVAT, 2)) FROM TransCustomerPaymentInvoices custpayinv INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) WHERE sub_inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate) AND sub_inv.FOWSTID = inv.FowstID AND sub_inv.DeviceID = inv.DeviceID),0) +" & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(custpay.ChargeAmount, 2),0)) FROM TransCustomerPaymentInvoices custpayinv INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) WHERE CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate) AND sub_inv.FOWSTID = inv.FowstID AND sub_inv.DeviceID = inv.DeviceID),0) +" & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.Fees, 2), 0) + ISNULL(ROUND(srledger.RefundFees, 2), 0)) FROM TransSalesReturnLedger srledger INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) WHERE CONVERT(date, memo.LastUpdated) < CONVERT(date, inv.InvoiceDate) AND memo.FOWSTID = inv.FowstID AND memo.DeviceID = inv.DeviceID),0))" & vbCrLf &
                "as OldGrandTotal " & vbCrLf &
                ", (ISNULL((SELECT SUM(ROUND(invitem.GrossSalesNetOfVATExemptions, 2)) " & vbCrLf &
                " FROM TransInvoiceItems invitem " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) - " & vbCrLf &
                " ISNULL(tblDiscount.Sum, 0) - " & vbCrLf &
                " ISNULL(tblRefund.Sum, 0) - " & vbCrLf &
                " ISNULL((SELECT SUM(ROUND(srledger.GrossSrvAmount, 2)) " & vbCrLf &
                " FROM TransSalesReturnLedger srledger " & vbCrLf &
                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) - " & vbCrLf &
                "ISNULL((SELECT SUM(ROUND(sub_inv.ForegoneVAT, 2)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                " AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) + " & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(custpay.ChargeAmount, 2),0)) " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                " WHERE CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) + " & vbCrLf &
                "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.Fees, 2), 0) + ISNULL(ROUND(srledger.RefundFees, 2), 0)) " & vbCrLf &
                " FROM TransSalesReturnLedger srledger " & vbCrLf &
                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                ",0) " & vbCrLf &
                ") as NewGrandTotal" & vbCrLf

            If Not CBool2(header.Rows(0)("IsVAT")) Then
                pSql &= ", ISNULL((SELECT SUM(ROUND(invitem.GrossSalesNetOfVATExemptions, 2))" & vbCrLf &
                        "FROM TransInvoiceItems invitem" & vbCrLf &
                        "INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID)" & vbCrLf &
                        "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                        "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                        "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                        "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                        ",0) " & vbCrLf &
                        " - ISNULL(tblDiscount.Sum, 0) " & vbCrLf &
                        " - ISNULL(tblRefund.Sum, 0) " & vbCrLf &
                        " - ISNULL((SELECT SUM(ROUND(srledger.GrossSrvAmount, 2)) " & vbCrLf &
                        " FROM TransSalesReturnLedger srledger " & vbCrLf &
                        " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                        " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                        " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                        " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                        ",0) " & vbCrLf &
                        " - ISNULL((SELECT SUM(ROUND(sub_inv.ForegoneVAT, 2)) " & vbCrLf &
                        " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                        " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                        " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)  " & vbCrLf &
                        " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                        " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                        " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                        " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                        ",0)" & vbCrLf &
                        " + ISNULL((SELECT SUM(ISNULL(ROUND(srledger.Fees, 2), 0) + ISNULL(ROUND(srledger.RefundFees, 2), 0)) " & vbCrLf &
                        " FROM TransSalesReturnLedger srledger " & vbCrLf &
                        " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                        " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                        " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                        " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                        ",0) +" & vbCrLf &
                        "ISNULL((SELECT SUM(ISNULL(ROUND(charge.ChargeAmount, 2), 0)) " & vbCrLf &
                        " FROM TransInvoice sub_inv " & vbCrLf &
                        " INNER JOIN TransInvoiceCharges charge ON (sub_inv.ID = charge.InvoiceID) " & vbCrLf &
                        " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                        " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                        " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                        " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                        ",0) " & vbCrLf &
                        " AS NonVAT " & vbCrLf
            Else
                pSql &= " , 0 as NonVAT " & vbCrLf
            End If

            pSql &= " , ISNULL(tblRefund.Sum, 0) as Refunds " & vbCrLf &
                    " , ISNULL((SELECT SUM(ROUND(srledger.GrossSrvAmount, 2)) " & vbCrLf &
                    " FROM TransSalesReturnLedger srledger " & vbCrLf &
                    " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                    " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                    " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                    ",0) as [Returns] " & vbCrLf &
                    ", ISNULL((SELECT SUM(ISNULL(ROUND(custpay.ChargeAmount, 2),0)) " &
                    " FROM TransCustomerPaymentInvoices custpayinv " &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)  " &
                    " WHERE CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID " &
                    " AND sub_inv.DeviceID = inv.DeviceID) " &
                    ",0) as ChargeAmount " & vbCrLf

            pSql &= " , ISNULL((SELECT ISNULL(ZReadCount, 1) FROM TransZReadCache WHERE ISNULL(ZReadCount, 0) > 0 AND FOWSTID = inv.FowstID AND DeviceID = inv.DeviceID AND CONVERT(date, SalesDate) = CONVERT(date, inv.InvoiceDate)), 0) as ZReadCount " & vbCrLf

            pSql &= "FROM TransInvoice as inv" & vbCrLf &
                "OUTER APPLY(" & vbCrLf &
                "SELECT SUM(ROUND(custpay.DiscountAmount, 2)) AS Sum" & vbCrLf &
                ",COUNT(DISTINCT custpayinv.InvoiceID) AS Count" & vbCrLf &
                "FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                "INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                "INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                "INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID" & vbCrLf &
                "--AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df')" & vbCrLf &
                "WHERE sub_inv.InvoiceNumber > 0 And custpay.ReceiptType <> 'RF' --AND disc.NoOfSeniorCitizens IS NULL" & vbCrLf &
                "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                " AND sub_inv.DeviceID = inv.DeviceID" & vbCrLf &
                ") as tblDiscount" & vbCrLf &
                "OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(ROUND(srledger.GrossRefundAmount, 2)) AS Sum" & vbCrLf &
                "   ,COUNT(DISTINCT memo.id) AS Count" & vbCrLf &
                "  	FROM TransSalesReturnLedger srledger" & vbCrLf &
                "  	JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                "   WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                "  	AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                "  	AND memo.DeviceID = inv.DeviceID" & vbCrLf &
                " 	AND srledger.GrossRefundAmount > 0" & vbCrLf &
                " ) as tblRefund" & vbCrLf &
                "OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(ROUND(memosr.TaxAmount, 2)) as VATAmount " & vbCrLf &
                " FROM TransMemoSalesReturn memosr " & vbCrLf &
                " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                " INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID " & vbCrLf &
                " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                " AND memo.DeviceID = inv.DeviceID AND ISNULL(sub_inv.SCDiscount, 0) = 0 AND ISNULL(sub_inv.SoloParentDiscount, 0) = 0" & vbCrLf &
                " ) as tblSalesReturnVATAmount" & vbCrLf &
                "WHERE inv.FOWSTID = " & ReadyString(fowstid) & " AND inv.DeviceID = " & ReadyString(deviceid) & vbCrLf &
                "AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf &
                "GROUP BY CONVERT(date, inv.InvoiceDate), inv.FOWSTID, inv.DeviceID, tblDiscount.Sum, tblRefund.Sum, tblSalesReturnVATAmount.VATAmount" & vbCrLf &
                "ORDER BY CONVERT(date, inv.InvoiceDate) ASC"

#End Region
            Dim salesdata As DataTable

            If Not ExcludeSales Then
                salesdata = Common.WebRequest.ProData.Execute(pSql)
                pReport.AddTable("Data", salesdata)
                pOut.Add("Sales", salesdata)
            End If


            If Not ExcludeSC Or
                Not ExcludePWD Or
                Not ExcludeNAAC Or
                Not ExcludeSP Then
                pSql = " IF OBJECT_ID('tempdb..##ESales_DiscountBookReport') IS NOT NULL DROP TABLE ##ESales_DiscountBookReport " & vbCrLf &
                    " SELECT inv.ID as InvoiceID, inv.InvoiceDate, inv.InvoiceNumber, TotalAmount as GrossAmount, ROUND(VAT, 2) as VAT, ROUND(Vatable, 2) as Vatable, ROUND(ForegoneVAT, 2) as ForegoneVAT, ROUND(VATExempt, 2) as VATExempt, disc.DiscountType, invdisc.DiscountTypeID, invdisc.DiscountPercentage, org.PersonFullName as CustomerName, org.TinNumber, " & vbCrLf &
                    " ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '') + ISNULL(PersonOtherIDNo, '') as CustomerIDNo, " & vbCrLf &
                    " CASE WHEN ISNULL(SCDiscount, 0) > SCDiscount THEN SCDiscount " & vbCrLf &
                    "   WHEN ISNULL(PhNatTeamDiscount, 0) > 0 THEN PhNatTeamDiscount " & vbCrLf &
                    "   WHEN ISNULL(SoloParentDiscount, 0) > 0 THEN SoloParentDiscount " & vbCrLf &
                    "   ELSE payinv.DiscountAmount END as DiscountAmount, " & vbCrLf &
                    "   ISNULL(ROUND(inv.SCDiscountExempt, 2), 0) as DiscountExempt, " & vbCrLf &
                    " ROUND(SCDiscountExempt, 2) as SCDiscountExempt, " & vbCrLf &
                    " ISNULL(org.ChildName, '') as ChildName, " & vbCrLf &
                    " ISNULL(org.ChildBirthdate, '') as ChildBirthdate, " & vbCrLf &
                    " ISNULL(org.ChildAge, '') as ChildAge " & vbCrLf &
                    " INTO ##ESales_DiscountBookReport " & vbCrLf &
                    " FROM TransInvoice as inv " & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as invdisc ON invdisc.InvoiceID = inv.ID " & vbCrLf &
                    " INNER JOIN LibraryDiscountSettings as disc ON disc.ID = invdisc.DiscountTypeID " & vbCrLf &
                    " INNER JOIN TransCustomerPaymentInvoices as payinv ON payinv.InvoiceID = inv.ID " & vbCrLf &
                    " LEFT JOIN TransInvoiceSeniorCitizenPWDCustomer as cust ON cust.InvoiceID = inv.ID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as org ON org.ID = cust.CustomerID " & vbCrLf &
                    " where CONVERT(date,InvoiceDate) BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf &
                    " AND inv.FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                    " AND inv.DeviceID = " & ReadyString(deviceid) & vbCrLf &
                    " --AND disc.ID IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df', '21ebf55b-526c-4276-b3b7-0cbca504f425', 'e95f0b64-1eea-47b4-bde5-b795234b462d') " & vbCrLf &
                    " ORDER BY inv.InvoiceDate DESC " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)
            End If

            If CBool2(DownloadDataOnly) Then
                If Not ExcludeSC Then
                    pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = '055A0A33-DD98-47EC-ADD3-8B2009F3944A' "
                    pOut.Add("SC", Common.WebRequest.ProData.Execute(pSql))
                End If

                If Not ExcludePWD Then
                    pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = '34A4032F-A142-4C15-A868-4AC4410434DF' "
                    pOut.Add("PWD", Common.WebRequest.ProData.Execute(pSql))
                End If

                If Not ExcludeNAAC Then
                    pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = '21EBF55B-526C-4276-B3B7-0CBCA504F425' "
                    pOut.Add("NAAC", Common.WebRequest.ProData.Execute(pSql))
                End If

                If Not ExcludeSP Then
                    pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = 'E95F0B64-1EEA-47B4-BDE5-B795234B462D' "
                    pOut.Add("SP", Common.WebRequest.ProData.Execute(pSql))
                End If

                Return pOut.Serialize()
            Else
                If fileformat = "xlsx" Then
                    Dim pWB As Aspose.Cells.Workbook
                    Dim worksheetSummary As Aspose.Cells.Worksheet
                    Dim worksheetSC As Aspose.Cells.Worksheet
                    Dim worksheetPWD As Aspose.Cells.Worksheet
                    Dim worksheetNAAC As Aspose.Cells.Worksheet
                    Dim worksheetSP As Aspose.Cells.Worksheet
                    Dim headerCtr As Integer = 0
                    Dim rowCtr As Integer = 0
                    Dim tblHeader
                    Dim cells As Cells
                    Dim pageSetup As PageSetup

                    pWB = New Aspose.Cells.Workbook

                    If Not ExcludeSales Then
                        worksheetSummary = pWB.Worksheets(0)
                        cells = worksheetSummary.Cells
                        worksheetSummary.Name = "Sales Summary"

                        ExcelHeader(worksheetSummary, "BIR Sales Summary", 0, 0, "#ffffff", "#ffffff", 17, 35)
                        cells.Merge(0, 0, 1, 32)

                        HeaderDetails(worksheetSummary, header, datefrom, dateto, 32, 17, 35)

                        ExcelHeader(worksheetSummary, "Deductions", 17, 11, "#ffc000", "#000000", 12, 35)
                        cells.Merge(17, 11, 1, 8)
                        ExcelHeader(worksheetSummary, "Adjustment on VAT", 17, 19, "#92d050", "#000000", 12, 35)
                        cells.Merge(17, 19, 1, 6)
                        ExcelHeader(worksheetSummary, "Discount", 18, 11, "#ffc000", "#000000", 12, 35)
                        cells.Merge(18, 11, 1, 5)
                        ExcelHeader(worksheetSummary, "Discount", 18, 19, "#92d050", "#000000", 12, 35)
                        cells.Merge(18, 19, 1, 3)

                        rowCtr = 19

                        tblHeader = {
                            New With {.Name = "Date", .Color = "#a6a6a6"},
                            New With {.Name = "Beg. SI No.", .Color = "#a6a6a6"},
                            New With {.Name = "End. SI No.", .Color = "#a6a6a6"},
                            New With {.Name = "Grand Accum. Sales  Ending Bal.", .Color = "#00b0f0"},
                            New With {.Name = "Grand Accum. Sales Beg. Bal.", .Color = "#00b0f0"},
                            New With {.Name = "Sales Issued w/ Manual SI/OR", .Color = "#00b0f0"},
                            New With {.Name = "Gross  Sales  for the Day", .Color = "#00b0f0"},
                            New With {.Name = "VATable  Sales", .Color = "#ffff00"},
                            New With {.Name = "VAT Amount", .Color = "#ffff00"},
                            New With {.Name = "VAT Exempt Sales", .Color = "#ffff00"},
                            New With {.Name = "Zero Rated", .Color = "#ffff00"},
                            New With {.Name = "SC", .Color = "#ffc000"},
                            New With {.Name = "PWD", .Color = "#ffc000"},
                            New With {.Name = "NAAC", .Color = "#ffc000"},
                            New With {.Name = "Solo  Parent", .Color = "#ffc000"},
                            New With {.Name = "Others", .Color = "#ffc000"},
                            New With {.Name = "Returns", .Color = "#ffc000"},
                            New With {.Name = "Refund", .Color = "#ffc000"},
                            New With {.Name = "Total Deduction", .Color = "#ffc000"},
                            New With {.Name = "SC", .Color = "#92d050"},
                            New With {.Name = "PWD", .Color = "#92d050"},
                            New With {.Name = "Others", .Color = "#92d050"},
                            New With {.Name = "VAT on Returns", .Color = "#92d050"},
                            New With {.Name = "Others", .Color = "#92d050"},
                            New With {.Name = "Total VAT  Adj.", .Color = "#92d050"},
                            New With {.Name = "VAT Payable", .Color = "#a6a6a6"},
                            New With {.Name = "Net Sales", .Color = "#a6a6a6"},
                            New With {.Name = "Sales Overrun/ Overflow", .Color = "#a6a6a6"},
                            New With {.Name = "Total Income", .Color = "#a6a6a6"},
                            New With {.Name = "Reset Ctr", .Color = "#a6a6a6"},
                            New With {.Name = "Z Ctr", .Color = "#a6a6a6"} 'New With {.Name = "Remarks", .Color = "#a6a6a6"}
                        }

                        For Each head As Object In tblHeader
                            ExcelHeader(worksheetSummary, head.Name, rowCtr, headerCtr, head.Color, "#000000", 11, False)
                            headerCtr += 1
                        Next

                        cells.Merge(17, 0, 3, 1)
                        cells.Merge(17, 1, 3, 1)
                        cells.Merge(17, 2, 3, 1)
                        cells.Merge(17, 3, 3, 1)
                        cells.Merge(17, 4, 3, 1)
                        cells.Merge(17, 5, 3, 1)
                        cells.Merge(17, 6, 3, 1)
                        cells.Merge(17, 7, 3, 1)
                        cells.Merge(17, 8, 3, 1)
                        cells.Merge(17, 9, 3, 1)
                        cells.Merge(17, 10, 3, 1)

                        cells.Merge(18, 16, 2, 1)
                        cells.Merge(18, 17, 2, 1)
                        cells.Merge(18, 18, 2, 1)

                        cells.Merge(18, 22, 2, 1)
                        cells.Merge(18, 23, 2, 1)
                        cells.Merge(18, 24, 2, 1)

                        cells.Merge(17, 25, 3, 1)
                        cells.Merge(17, 26, 3, 1)
                        cells.Merge(17, 27, 3, 1)
                        cells.Merge(17, 28, 3, 1)
                        cells.Merge(17, 29, 3, 1)
                        cells.Merge(17, 30, 3, 1)
                        cells.Merge(17, 31, 3, 1)
                        'cells.Merge(17, 32, 3, 1)

                        'worksheetSummary.AutoFitColumns()
                        worksheetSummary.Cells.SetColumnWidth(0, 11)
                        worksheetSummary.Cells.SetColumnWidth(1, 10)
                        worksheetSummary.Cells.SetColumnWidth(2, 10)
                        worksheetSummary.Cells.SetColumnWidth(3, 11)
                        worksheetSummary.Cells.SetColumnWidth(4, 11)
                        worksheetSummary.Cells.SetColumnWidth(5, 11)
                        worksheetSummary.Cells.SetColumnWidth(6, 13)
                        worksheetSummary.Cells.SetColumnWidth(7, 10)
                        worksheetSummary.Cells.SetColumnWidth(8, 10)
                        worksheetSummary.Cells.SetColumnWidth(9, 10)
                        worksheetSummary.Cells.SetColumnWidth(10, 8)
                        worksheetSummary.Cells.SetColumnWidth(11, 8)
                        worksheetSummary.Cells.SetColumnWidth(12, 8)
                        worksheetSummary.Cells.SetColumnWidth(13, 7)
                        worksheetSummary.Cells.SetColumnWidth(14, 7)
                        worksheetSummary.Cells.SetColumnWidth(15, 8)
                        worksheetSummary.Cells.SetColumnWidth(16, 8)
                        worksheetSummary.Cells.SetColumnWidth(17, 8)
                        worksheetSummary.Cells.SetColumnWidth(18, 9)
                        worksheetSummary.Cells.SetColumnWidth(19, 7)
                        worksheetSummary.Cells.SetColumnWidth(20, 7)
                        worksheetSummary.Cells.SetColumnWidth(21, 7)
                        worksheetSummary.Cells.SetColumnWidth(22, 9)
                        worksheetSummary.Cells.SetColumnWidth(23, 8)
                        worksheetSummary.Cells.SetColumnWidth(24, 8)
                        worksheetSummary.Cells.SetColumnWidth(25, 9)
                        worksheetSummary.Cells.SetColumnWidth(26, 10)
                        worksheetSummary.Cells.SetColumnWidth(27, 11)
                        worksheetSummary.Cells.SetColumnWidth(28, 9)
                        worksheetSummary.Cells.SetColumnWidth(29, 7)
                        worksheetSummary.Cells.SetColumnWidth(30, 7)
                        'worksheetSummary.Cells.SetColumnWidth(31, 9)
                        rowCtr += 1

                        For Each sale As DataRow In salesdata.Rows
                            worksheetSummary.Cells.SetRowHeight(rowCtr, 30)
                            worksheetSummary.Cells(rowCtr, 0).PutValue(Format(CDate(sale("InvoiceDate")), "MM-dd-yyyy"))
                            worksheetSummary.Cells(rowCtr, 1).PutValue(Format(CDec2(sale("BegInvoiceNumber")), "000000000000"))
                            worksheetSummary.Cells(rowCtr, 2).PutValue(Format(CDec2(sale("EndInvoiceNumber")), "000000000000"))
                            worksheetSummary.Cells(rowCtr, 3).PutValue(Format(CDec2(sale("OldGrandTotal")) + CDec2(sale("NewGrandTotal")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 4).PutValue(Format(CDec2(sale("OldGrandTotal")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 5).PutValue(Format(CDec2(0), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 6).PutValue(Format(CDec2(sale("GrossSales")), "#,##0.00"), True)

                            If CBool2(header(0)("IsVAT")) Then
                                worksheetSummary.Cells(rowCtr, 7).PutValue(Format(CDec2(sale("VatableSales")), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 8).PutValue(Format(CDec2(sale("VATAmount")), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 9).PutValue(Format(CDec2(sale("VATExempt")), "#,##0.00"), True)
                            Else
                                worksheetSummary.Cells(rowCtr, 7).PutValue("0.00", True)
                                worksheetSummary.Cells(rowCtr, 8).PutValue("0.00", True)
                                worksheetSummary.Cells(rowCtr, 9).PutValue("0.00", True)
                            End If

                            worksheetSummary.Cells(rowCtr, 10).PutValue("0.00", True) 'ZERO RATED

                            worksheetSummary.Cells(rowCtr, 11).PutValue(Format(CDec2(sale("SCAmount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 12).PutValue(Format(CDec2(sale("PWDAmount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 13).PutValue(Format(CDec2(sale("NAACAmount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 14).PutValue(Format(CDec2(sale("SoloParentAmount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 15).PutValue(Format(CDec2(sale("OtherDiscountAmount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 16).PutValue(Format(CDec2(sale("Refunds")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 17).PutValue(Format(CDec2(sale("Returns")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 18).PutValue(Format(CDec2(sale("SCAmount")) +
                                                                           CDec2(sale("PWDAmount")) +
                                                                           CDec2(sale("SoloParentAmount")) +
                                                                           CDec2(sale("OtherDiscountAmount")) +
                                                                           CDec2(sale("Refunds")) +
                                                                           CDec2(sale("Returns")), "#,##0.00"), True)

                            If CBool2(header(0)("IsVAT")) Then
                                Dim totalvatadj As Decimal = CDec2(sale("SCForegoneVat")) + CDec2(sale("PWDForegoneVat")) + CDec2(sale("SoloParentForegoneVat")) + CDec2(sale("VATOnReturns"))
                                worksheetSummary.Cells(rowCtr, 19).PutValue(Format(CDec2(sale("SCForegoneVat")), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 20).PutValue(Format(CDec2(sale("PWDForegoneVat")), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 21).PutValue(Format(CDec2(sale("SoloParentForegoneVat")), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 22).PutValue(Format(CDec2(sale("VATOnReturns")), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 23).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 24).PutValue(Format(CDec2(totalvatadj), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 25).PutValue(Format(CDec2(sale("VATAmount")) - CDec2(totalvatadj), "#,##0.00"), True)
                            Else
                                worksheetSummary.Cells(rowCtr, 19).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 20).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 21).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 22).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 23).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 24).PutValue(Format(CDec2(0), "#,##0.00"), True)
                                worksheetSummary.Cells(rowCtr, 25).PutValue(Format(CDec2(0), "#,##0.00"), True)
                            End If

                            'worksheetSummary.Cells(rowCtr, 26).PutValue(Format(CDec2(sale("ChargeAmount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 26).PutValue(Format(CDec2(sale("NewGrandTotal")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 27).PutValue(Format(CDec2(0), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 28).PutValue(Format(CDec2(sale("TotalPayments")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 29).PutValue(0)
                            worksheetSummary.Cells(rowCtr, 30).PutValue(Format(CDec2(sale("ZReadCount")), "#,##0.00"), True)
                            worksheetSummary.Cells(rowCtr, 31).PutValue("")

                            Dim lastCol As Integer = pWB.Worksheets(0).Cells.MaxDataColumn
                            Dim headerRange As Range = pWB.Worksheets(0).Cells.CreateRange(rowCtr, 0, 1, lastCol + 1)

                            Dim style As Style = pWB.Worksheets(0).Cells("A1").GetStyle()
                            style.Borders(BorderType.TopBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.TopBorder).Color = Color.Black
                            style.Borders(BorderType.BottomBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.BottomBorder).Color = Color.Black
                            style.Borders(BorderType.LeftBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.LeftBorder).Color = Color.Black
                            style.Borders(BorderType.RightBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.RightBorder).Color = Color.Black
                            style.Font.Size = 11
                            style.Font.Name = "Calibri"

                            ' 3. Use a StyleFlag so only the border is updated
                            Dim flag As New StyleFlag()
                            flag.Borders = True ' Only apply border settings
                            flag.FontSize = True
                            flag.WrapText = True
                            headerRange.ApplyStyle(style, flag)

                            headerRange.SetOutlineBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black)
                            headerRange.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black)
                            headerRange.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black)
                            headerRange.SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black)

                            rowCtr += 1
                        Next

                        worksheetSummary.TabColor = ColorTranslator.FromHtml("#8DB4E2")
                        'worksheetSummary.FreezePanes(10, 0, 10, 0)
                        worksheetSummary.AutoFitRows()

                        pageSetup = worksheetSummary.PageSetup
                        pageSetup.LeftMargin = 0.5
                        pageSetup.RightMargin = 0.5
                        pageSetup.TopMargin = 0.5
                        pageSetup.BottomMargin = 0.5
                        pageSetup.PaperSize = PaperSizeType.PaperLegal
                        pageSetup.Orientation = PageOrientationType.Landscape
                        pageSetup.FitToPagesWide = 1
                        pageSetup.FitToPagesTall = 0

                        'Dim remarksCol As Integer = worksheetSummary.Cells.MaxColumn
                        '' Logic to move column 0 (Remarks) to the very end
                        'worksheetSummary.Cells.InsertColumn(remarksCol + 1)
                        'worksheetSummary.Cells.CopyColumn(worksheetSummary.Cells, 0, remarksCol + 1)
                        'worksheetSummary.Cells.DeleteColumn(0)
                    End If

                    Dim dtCustomer As DataTable

                    If Not ExcludeSC Then
                        worksheetSC = pWB.Worksheets(pWB.Worksheets.Add())
                        cells = worksheetSC.Cells
                        worksheetSC.Name = "Senior Citizen Sales Book Report"

                        ExcelHeader(worksheetSC, "Senior Citizen Sales Book Report", 0, 0, "#ffffff", "#ffffff", 12, 25)
                        cells.Merge(0, 0, 1, 10)

                        HeaderDetails(worksheetSC, header, datefrom, dateto, 10, 12, 25)

                        tblHeader = {
                            New With {.Name = "Date", .Color = "#a6a6a6"},
                            New With {.Name = "Name of" + Environment.NewLine + "Senior Citizen (SC)", .Color = "#00b0f0"},
                            New With {.Name = "OSCA ID No./" + Environment.NewLine + "SC ID No.", .Color = "#ffff00"},
                            New With {.Name = "SC TIN", .Color = "#9999ff"},
                            New With {.Name = "SI/OR Number", .Color = "#ffc000"},
                            New With {.Name = "Sales" + Environment.NewLine + "(inclusive of VAT)", .Color = "#92d050"},
                            New With {.Name = "VAT Amount", .Color = "#ed7d31"},
                            New With {.Name = "VAT Ex. Sales", .Color = "#e7e6e6"},
                            New With {.Name = "Discount 20%", .Color = "#ffd966"},
                            New With {.Name = "Net Sales", .Color = "#a6a6a6"}
                        }
                        headerCtr = 0
                        rowCtr = 17

                        For Each head As Object In tblHeader
                            ExcelHeader(worksheetSC, head.Name, rowCtr, headerCtr, head.Color, "#000000", 10, 25)
                            headerCtr += 1
                        Next

                        worksheetSC.Cells.SetColumnWidth(0, 20)
                        worksheetSC.Cells.SetColumnWidth(1, 25)
                        worksheetSC.Cells.SetColumnWidth(2, 15)
                        worksheetSC.Cells.SetColumnWidth(3, 18)
                        worksheetSC.Cells.SetColumnWidth(4, 17)
                        worksheetSC.Cells.SetColumnWidth(5, 16)
                        worksheetSC.Cells.SetColumnWidth(6, 13)
                        worksheetSC.Cells.SetColumnWidth(7, 13)
                        worksheetSC.Cells.SetColumnWidth(8, 15)
                        worksheetSC.Cells.SetColumnWidth(9, 15)

                        worksheetSC.Cells.SetRowHeight(rowCtr, 40)

                        pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = '055A0A33-DD98-47EC-ADD3-8B2009F3944A' "
                        dtCustomer = Common.WebRequest.ProData.Execute(pSql)

                        rowCtr += 1
                        For Each cus As DataRow In dtCustomer.Rows
                            worksheetSC.Cells.SetRowHeight(rowCtr, 25)
                            worksheetSC.Cells(rowCtr, 0).PutValue(Format(CDate(cus("InvoiceDate")), "MM-dd-yyyy hh:mm tt"))
                            worksheetSC.Cells(rowCtr, 1).PutValue(cus("CustomerName"))
                            worksheetSC.Cells(rowCtr, 2).PutValue(cus("CustomerIDNo"))
                            worksheetSC.Cells(rowCtr, 3).PutValue(cus("TinNumber"))
                            worksheetSC.Cells(rowCtr, 4).PutValue(Format(CDec2(cus("InvoiceNumber")), "000000000000"))
                            worksheetSC.Cells(rowCtr, 5).PutValue(Format(CDec2(cus("GrossAmount")) - CDec2(cus("DiscountExempt")), "#,##0.00"), True)
                            worksheetSC.Cells(rowCtr, 6).PutValue(Format(CDec2(cus("ForegoneVAT")), "#,##0.00"), True)
                            worksheetSC.Cells(rowCtr, 7).PutValue(Format(CDec2(cus("VATExempt")), "#,##0.00"), True)
                            worksheetSC.Cells(rowCtr, 8).PutValue(Format(CDec2(cus("DiscountAmount")), "#,##0.00"), True)
                            worksheetSC.Cells(rowCtr, 9).PutValue(Format(CDec2(cus("VATExempt")) - CDec2(cus("DiscountAmount")), "#,##0.00"), True)

                            Dim lastCol As Integer = pWB.Worksheets(1).Cells.MaxDataColumn
                            Dim headerRange As Range = pWB.Worksheets(1).Cells.CreateRange(rowCtr, 0, 1, lastCol + 1)

                            Dim style As Style = pWB.Worksheets(1).Cells("A1").GetStyle()
                            style.Borders(BorderType.TopBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.TopBorder).Color = Color.Black
                            style.Borders(BorderType.BottomBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.BottomBorder).Color = Color.Black
                            style.Borders(BorderType.LeftBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.LeftBorder).Color = Color.Black
                            style.Borders(BorderType.RightBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.RightBorder).Color = Color.Black
                            style.Font.Size = 10
                            style.Font.Name = "Calibri"

                            Dim flag As New StyleFlag()
                            flag.Borders = True ' Only apply border settings
                            flag.FontSize = True
                            flag.WrapText = True
                            headerRange.ApplyStyle(style, flag)

                            rowCtr += 1
                        Next

                        worksheetSC.TabColor = ColorTranslator.FromHtml("#E6B8B7")
                        'worksheetSC.FreezePanes("A2", 1, 0)
                        'worksheetSC.AutoFitColumns()
                        'worksheetSC.AutoFitRows()

                        pageSetup = worksheetSC.PageSetup
                        pageSetup.LeftMargin = 0.5
                        pageSetup.RightMargin = 0.5
                        pageSetup.TopMargin = 0.5
                        pageSetup.BottomMargin = 0.5
                        pageSetup.PaperSize = PaperSizeType.PaperLegal
                        pageSetup.Orientation = PageOrientationType.Landscape
                        pageSetup.FitToPagesWide = 1
                        pageSetup.FitToPagesTall = 0
                    End If

                    If Not ExcludePWD Then
                        worksheetPWD = pWB.Worksheets(pWB.Worksheets.Add())
                        cells = worksheetPWD.Cells
                        worksheetPWD.Name = "PWD Sales Book Report"

                        ExcelHeader(worksheetPWD, "PWD Sales Book Report", 0, 0, "#ffffff", "#ffffff", 12, 25)
                        cells.Merge(0, 0, 1, 10)

                        HeaderDetails(worksheetPWD, header, datefrom, dateto, 10, 10, 25)

                        tblHeader = {
                            New With {.Name = "Date", .Color = "#a6a6a6"},
                            New With {.Name = "Name of Person" + Environment.NewLine + "with Disability" + Environment.NewLine + "(PWD)", .Color = "#00b0f0"},
                            New With {.Name = "PWD ID No.", .Color = "#ffff00"},
                            New With {.Name = "PWD TIN", .Color = "#9999ff"},
                            New With {.Name = "SI/OR Number", .Color = "#ffc000"},
                            New With {.Name = "Sales" + Environment.NewLine + "(inclusive of VAT)", .Color = "#92d050"},
                            New With {.Name = "VAT Amount", .Color = "#ed7d31"},
                            New With {.Name = "VAT Ex. Sales", .Color = "#e7e6e6"},
                            New With {.Name = "Discount 20%", .Color = "#ffd966"},
                            New With {.Name = "Net Sales", .Color = "#a6a6a6"}
                        }
                        headerCtr = 0
                        rowCtr = 17

                        For Each head As Object In tblHeader
                            ExcelHeader(worksheetPWD, head.Name, rowCtr, headerCtr, head.Color, "#000000", 10, 25, False)
                            headerCtr += 1
                        Next

                        worksheetPWD.Cells.SetColumnWidth(0, 20)
                        worksheetPWD.Cells.SetColumnWidth(1, 25)
                        worksheetPWD.Cells.SetColumnWidth(2, 15)
                        worksheetPWD.Cells.SetColumnWidth(3, 18)
                        worksheetPWD.Cells.SetColumnWidth(4, 17)
                        worksheetPWD.Cells.SetColumnWidth(5, 16)
                        worksheetPWD.Cells.SetColumnWidth(6, 13)
                        worksheetPWD.Cells.SetColumnWidth(7, 13)
                        worksheetPWD.Cells.SetColumnWidth(8, 15)
                        worksheetPWD.Cells.SetColumnWidth(9, 15)

                        worksheetPWD.Cells.SetRowHeight(rowCtr, 40)

                        pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = '34A4032F-A142-4C15-A868-4AC4410434DF' "
                        dtCustomer = Common.WebRequest.ProData.Execute(pSql)

                        rowCtr += 1
                        For Each cus As DataRow In dtCustomer.Rows
                            worksheetPWD.Cells.SetRowHeight(rowCtr, 25)
                            worksheetPWD.Cells(rowCtr, 0).PutValue(Format(CDate(cus("InvoiceDate")), "MM-dd-yyyy hh:mm tt"))
                            worksheetPWD.Cells(rowCtr, 1).PutValue(cus("CustomerName"))
                            worksheetPWD.Cells(rowCtr, 2).PutValue(cus("CustomerIDNo"))
                            worksheetPWD.Cells(rowCtr, 3).PutValue(cus("TinNumber"))
                            worksheetPWD.Cells(rowCtr, 4).PutValue(Format(CDec2(cus("InvoiceNumber")), "000000000000"))
                            worksheetPWD.Cells(rowCtr, 5).PutValue(Format(CDec2(cus("GrossAmount")) - CDec2(cus("DiscountExempt")), "#,##0.00"), True)
                            worksheetPWD.Cells(rowCtr, 6).PutValue(Format(CDec2(cus("ForegoneVAT")), "#,##0.00"), True)
                            worksheetPWD.Cells(rowCtr, 7).PutValue(Format(CDec2(cus("VATExempt")), "#,##0.00"), True)
                            worksheetPWD.Cells(rowCtr, 8).PutValue(Format(CDec2(cus("DiscountAmount")), "#,##0.00"), True)
                            worksheetPWD.Cells(rowCtr, 9).PutValue(Format(CDec2(cus("VATExempt")) - CDec2(cus("DiscountAmount")), "#,##0.00"), True)

                            Dim lastCol As Integer = pWB.Worksheets(2).Cells.MaxDataColumn
                            Dim headerRange As Range = pWB.Worksheets(2).Cells.CreateRange(rowCtr, 0, 1, lastCol + 1)

                            Dim style As Style = pWB.Worksheets(2).Cells("A1").GetStyle()
                            style.Borders(BorderType.TopBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.TopBorder).Color = Color.Black
                            style.Borders(BorderType.BottomBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.BottomBorder).Color = Color.Black
                            style.Borders(BorderType.LeftBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.LeftBorder).Color = Color.Black
                            style.Borders(BorderType.RightBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.RightBorder).Color = Color.Black
                            style.Font.Size = 10
                            style.Font.Name = "Calibri"

                            Dim flag As New StyleFlag()
                            flag.Borders = True ' Only apply border settings
                            flag.FontSize = True
                            flag.WrapText = True
                            headerRange.ApplyStyle(style, flag)

                            rowCtr += 1
                        Next

                        worksheetPWD.TabColor = ColorTranslator.FromHtml("#D8E4BC")
                        'worksheetPWD.FreezePanes("A2", 1, 0)
                        'worksheetPWD.AutoFitColumns()
                        'worksheetPWD.AutoFitRows()

                        pageSetup = worksheetPWD.PageSetup
                        pageSetup.LeftMargin = 0.5
                        pageSetup.RightMargin = 0.5
                        pageSetup.TopMargin = 0.5
                        pageSetup.BottomMargin = 0.5
                        pageSetup.PaperSize = PaperSizeType.PaperLegal
                        pageSetup.Orientation = PageOrientationType.Landscape
                        pageSetup.FitToPagesWide = 1
                        pageSetup.FitToPagesTall = 0
                    End If

                    If Not ExcludeNAAC Then
                        worksheetNAAC = pWB.Worksheets(pWB.Worksheets.Add())
                        cells = worksheetNAAC.Cells
                        worksheetNAAC.Name = "NAAC (Athlete) Sales Book Report"

                        ExcelHeader(worksheetNAAC, "National Athletes and Coaches Sales Book Report", 0, 0, "#ffffff", "#ffffff", 12, 25)
                        cells.Merge(0, 0, 1, 7)

                        HeaderDetails(worksheetNAAC, header, datefrom, dateto, 7, 12, 25)

                        tblHeader = {
                            New With {.Name = "Date", .Color = "#a6a6a6"},
                            New With {.Name = "Name of National Athlete/ Coach", .Color = "#00b0f0"},
                            New With {.Name = "PNSTM ID No.", .Color = "#ffff00"},
                            New With {.Name = "SI/OR Number", .Color = "#ffc000"},
                            New With {.Name = "Gross Sales/ Receipts", .Color = "#92d050"},
                            New With {.Name = "Sales Discount", .Color = "#ffd966"},
                            New With {.Name = "Net Sales", .Color = "#a6a6a6"}
                        }
                        headerCtr = 0
                        rowCtr = 17

                        For Each head As Object In tblHeader
                            ExcelHeader(worksheetNAAC, head.Name, rowCtr, headerCtr, head.Color, "#000000", 12, 25, False)
                            headerCtr += 1
                        Next

                        worksheetNAAC.Cells.SetColumnWidth(0, 20)
                        worksheetNAAC.Cells.SetColumnWidth(1, 40)
                        worksheetNAAC.Cells.SetColumnWidth(2, 20)
                        worksheetNAAC.Cells.SetColumnWidth(3, 20)
                        worksheetNAAC.Cells.SetColumnWidth(4, 30)
                        worksheetNAAC.Cells.SetColumnWidth(5, 20)
                        worksheetNAAC.Cells.SetColumnWidth(6, 20)

                        worksheetNAAC.Cells.SetRowHeight(rowCtr, 40)

                        pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = '21EBF55B-526C-4276-B3B7-0CBCA504F425' "
                        dtCustomer = Common.WebRequest.ProData.Execute(pSql)

                        rowCtr += 1
                        For Each cus As DataRow In dtCustomer.Rows
                            worksheetNAAC.Cells.SetRowHeight(rowCtr, 25)
                            worksheetNAAC.Cells(rowCtr, 0).PutValue(Format(CDate(cus("InvoiceDate")), "MM-dd-yyyy hh:mm tt"))
                            worksheetNAAC.Cells(rowCtr, 1).PutValue(cus("CustomerName"))
                            worksheetNAAC.Cells(rowCtr, 2).PutValue(cus("CustomerIDNo"))
                            worksheetNAAC.Cells(rowCtr, 3).PutValue(Format(CDec2(cus("InvoiceNumber")), "000000000000"))
                            worksheetNAAC.Cells(rowCtr, 4).PutValue(Format(CDec2(cus("GrossAmount")), "#,##0.00"), True)
                            worksheetNAAC.Cells(rowCtr, 5).PutValue(Format(CDec2(cus("DiscountAmount")), "#,##0.00"), True)
                            worksheetNAAC.Cells(rowCtr, 6).PutValue(Format(((CDec2(cus("GrossAmount")) - CDec2(cus("VAT"))) - CDec2(cus("DiscountAmount"))) + CDec2(cus("VAT")), "#,##0.00"), True)

                            Dim lastCol As Integer = pWB.Worksheets(3).Cells.MaxDataColumn
                            Dim headerRange As Range = pWB.Worksheets(3).Cells.CreateRange(rowCtr, 0, 1, lastCol + 1)

                            Dim style As Style = pWB.Worksheets(3).Cells("A1").GetStyle()
                            style.Borders(BorderType.TopBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.TopBorder).Color = Color.Black
                            style.Borders(BorderType.BottomBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.BottomBorder).Color = Color.Black
                            style.Borders(BorderType.LeftBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.LeftBorder).Color = Color.Black
                            style.Borders(BorderType.RightBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.RightBorder).Color = Color.Black
                            style.Font.Size = 10
                            style.Font.Name = "Calibri"

                            Dim flag As New StyleFlag()
                            flag.Borders = True ' Only apply border settings
                            flag.FontSize = True
                            flag.WrapText = True
                            headerRange.ApplyStyle(style, flag)

                            rowCtr += 1
                        Next

                        worksheetNAAC.TabColor = ColorTranslator.FromHtml("#CCC0DA")
                        'worksheetNAAC.FreezePanes("A2", 1, 0)
                        'worksheetNAAC.AutoFitColumns()
                        'worksheetNAAC.AutoFitRows()

                        pageSetup = worksheetNAAC.PageSetup
                        pageSetup.LeftMargin = 0.5
                        pageSetup.RightMargin = 0.5
                        pageSetup.TopMargin = 0.5
                        pageSetup.BottomMargin = 0.5
                        pageSetup.PaperSize = PaperSizeType.PaperLegal
                        pageSetup.Orientation = PageOrientationType.Landscape
                        pageSetup.FitToPagesWide = 1
                        pageSetup.FitToPagesTall = 0
                    End If

                    If Not ExcludeSP Then
                        worksheetSP = pWB.Worksheets(pWB.Worksheets.Add())
                        cells = worksheetSP.Cells
                        worksheetSP.Name = "Solo Parent Book Report"

                        ExcelHeader(worksheetSP, "Solo Parent Sales Book Report", 0, 0, "#ffffff", "#ffffff", 12, 25)
                        cells.Merge(0, 0, 1, 10)

                        HeaderDetails(worksheetSP, header, datefrom, dateto, 10, 12, 25)

                        tblHeader = {
                            New With {.Name = "Date", .Color = "#a6a6a6"},
                            New With {.Name = "Name of" + Environment.NewLine + "Solo Parent", .Color = "#ed7d31"},
                            New With {.Name = "SPIC ID No.", .Color = "#ffc000"},
                            New With {.Name = "Name of Child", .Color = "#ffff00"},
                            New With {.Name = "Birth Date" + Environment.NewLine + "Of Child", .Color = "#ffff00"},
                            New With {.Name = "Age Of Child", .Color = "#ffff00"},
                            New With {.Name = "SI/OR Number", .Color = "#92d050"},
                            New With {.Name = "Gross Sales", .Color = "#00b0f0"},
                            New With {.Name = "Discount", .Color = "#9999ff"},
                            New With {.Name = "Net Sales", .Color = "#a6a6a6"}
                        }
                        headerCtr = 0
                        rowCtr = 17

                        For Each head As Object In tblHeader
                            ExcelHeader(worksheetSP, head.Name, rowCtr, headerCtr, head.Color, "#000000", 12, 25, False)
                            headerCtr += 1
                        Next

                        worksheetSP.Cells.SetColumnWidth(0, 20)
                        worksheetSP.Cells.SetColumnWidth(1, 23)
                        worksheetSP.Cells.SetColumnWidth(2, 18)
                        worksheetSP.Cells.SetColumnWidth(3, 20)
                        worksheetSP.Cells.SetColumnWidth(4, 20)
                        worksheetSP.Cells.SetColumnWidth(5, 15)
                        worksheetSP.Cells.SetColumnWidth(6, 17)
                        worksheetSP.Cells.SetColumnWidth(7, 15)
                        worksheetSP.Cells.SetColumnWidth(8, 11)
                        worksheetSP.Cells.SetColumnWidth(9, 11)

                        worksheetSP.Cells.SetRowHeight(rowCtr, 40)

                        pSql = " SELECT * FROM ##ESales_DiscountBookReport WHERE DiscountTypeID = 'E95F0B64-1EEA-47B4-BDE5-B795234B462D' "
                        dtCustomer = Common.WebRequest.ProData.Execute(pSql)

                        rowCtr += 1
                        For Each cus As DataRow In dtCustomer.Rows
                            worksheetSP.Cells.SetRowHeight(rowCtr, 25)
                            worksheetSP.Cells(rowCtr, 0).PutValue(Format(CDate(cus("InvoiceDate")), "MM-dd-yyyy hh:mm tt"))
                            worksheetSP.Cells(rowCtr, 1).PutValue(cus("CustomerName"))
                            worksheetSP.Cells(rowCtr, 2).PutValue(cus("CustomerIDNo"))
                            worksheetSP.Cells(rowCtr, 3).PutValue(cus("ChildName"))
                            worksheetSP.Cells(rowCtr, 4).PutValue(cus("ChildBirthdate"))
                            worksheetSP.Cells(rowCtr, 5).PutValue(cus("ChildAge"))
                            worksheetSP.Cells(rowCtr, 6).PutValue(Format(CDec2(cus("InvoiceNumber")), "000000000000"))
                            worksheetSP.Cells(rowCtr, 7).PutValue(Format(CDec2(cus("GrossAmount")), "#,##0.00"), True)
                            worksheetSP.Cells(rowCtr, 8).PutValue(Format(CDec2(cus("DiscountAmount")), "#,##0.00"), True)
                            worksheetSP.Cells(rowCtr, 9).PutValue(Format(CDec2(cus("VATExempt")) - CDec2(cus("DiscountAmount")), "#,##0.00"), True)

                            Dim lastCol As Integer = pWB.Worksheets(4).Cells.MaxDataColumn
                            Dim headerRange As Range = pWB.Worksheets(4).Cells.CreateRange(rowCtr, 0, 1, lastCol + 1)

                            Dim style As Style = pWB.Worksheets(4).Cells("A1").GetStyle()
                            style.Borders(BorderType.TopBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.TopBorder).Color = Color.Black
                            style.Borders(BorderType.BottomBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.BottomBorder).Color = Color.Black
                            style.Borders(BorderType.LeftBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.LeftBorder).Color = Color.Black
                            style.Borders(BorderType.RightBorder).LineStyle = CellBorderType.Thin
                            style.Borders(BorderType.RightBorder).Color = Color.Black
                            style.Font.Size = 10
                            style.Font.Name = "Calibri"

                            Dim flag As New StyleFlag()
                            flag.Borders = True ' Only apply border settings
                            flag.FontSize = True
                            flag.WrapText = True
                            headerRange.ApplyStyle(style, flag)

                            rowCtr += 1
                        Next

                        worksheetSP.TabColor = ColorTranslator.FromHtml("#B7DEE8")
                        'worksheetSP.FreezePanes("A2", 1, 0)
                        'worksheetSP.AutoFitColumns()
                        'worksheetSP.AutoFitRows()

                        pageSetup = worksheetSP.PageSetup
                        pageSetup.LeftMargin = 0.5
                        pageSetup.RightMargin = 0.5
                        pageSetup.TopMargin = 0.5
                        pageSetup.BottomMargin = 0.5
                        pageSetup.PaperSize = PaperSizeType.PaperLegal
                        pageSetup.Orientation = PageOrientationType.Landscape
                        pageSetup.FitToPagesWide = 1
                        pageSetup.FitToPagesTall = 0
                    End If

                    pWB.Save(filepath)
                Else
                    pReport.ExportXmlFile()
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, filepath)
                End If

                Dim file As New System.IO.FileInfo(filepath)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=""" & filename & """")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
            End If

        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try
    End Function

    Function HeaderDetails(ByRef ws As Aspose.Cells.Worksheet, ByRef header As DataTable, ByVal datefrom As String, ByVal dateto As String, ByVal maxcol As Integer, ByVal fontsize As Integer, ByVal rowheight As Integer)
        Dim cells As Cells = ws.Cells
        Dim lastCol As Integer = ws.Cells.MaxDataColumn

        ExcelHeader(ws, header.Rows(0)("BusinessName"), 1, 0, "#ffffff", "#ffffff", fontsize, rowheight, False)
        cells.Merge(1, 0, 1, maxcol)
        ExcelHeader(ws, header.Rows(0)("StoreAddress"), 2, 0, "#ffffff", "#ffffff", fontsize, rowheight, False)
        cells.Merge(2, 0, 1, maxcol)
        ExcelHeader(ws, IIf(CBool2(header(0)("IsVAT")), "", "NON-") & "VAT REG. TIN: " & header.Rows(0)("TIN"), 3, 0, "#ffffff", "#ffffff", fontsize, rowheight, False)
        cells.Merge(3, 0, 1, maxcol)
        ExcelHeader(ws, Format(CDate(datefrom), "MMM dd, yyyy") & " - " & Format(CDate(dateto), "MMM dd, yyyy"), 4, 0, "#ffffff", "#ffffff", fontsize, rowheight, False)
        cells.Merge(4, 0, 1, maxcol)

        If WebRequest.Instance.OrgID = ReadyGuid("565c260c-3844-4069-ac41-d26f0598e562") Then
            ws.Cells(5, 0).PutValue("Software Name: Mobiz Complete Retail")
        Else
            ws.Cells(5, 0).PutValue("Software Name: MOBIZ POS")
        End If

        ws.Cells(5, 0).Style.Font.Size = fontsize
        ws.Cells.SetRowHeight(5, 25)
        cells.Merge(5, 0, 1, 8)

        If WebRequest.Instance.OrgID = ReadyGuid("565c260c-3844-4069-ac41-d26f0598e562") Then
            ws.Cells(6, 0).PutValue("Version No.: v2.000.000")
        Else
            ws.Cells(6, 0).PutValue("Version No.: Version 1")
        End If

        ws.Cells(6, 0).Style.Font.Size = fontsize
        ws.Cells.SetRowHeight(6, 25)
        cells.Merge(6, 0, 1, 8)

        ws.Cells(7, 0).PutValue("Release No.: 1134627568152015050299")
        ws.Cells(7, 0).Style.Font.Size = fontsize
        cells.Merge(7, 0, 1, 8)
        ws.Cells.SetRowHeight(7, 25)

        ws.Cells(8, 0).PutValue("Release Date: May 4, 2015")
        ws.Cells(8, 0).Style.Font.Size = fontsize
        cells.Merge(8, 0, 1, 8)
        ws.Cells.SetRowHeight(8, 25)

        ws.Cells(9, 0).PutValue("Serial No.: " & header(0)("SerialNo"))
        ws.Cells(9, 0).Style.Font.Size = fontsize
        cells.Merge(9, 0, 1, 8)
        ws.Cells.SetRowHeight(9, 25)

        ws.Cells(10, 0).PutValue("MIN: " & header(0)("BIRMachineNo"))
        ws.Cells(10, 0).Style.Font.Size = fontsize
        cells.Merge(10, 0, 1, 8)
        ws.Cells.SetRowHeight(10, 25)

        ws.Cells(11, 0).PutValue("POS Terminal No.: 1")
        ws.Cells(11, 0).Style.Font.Size = fontsize
        cells.Merge(11, 0, 1, 8)
        ws.Cells.SetRowHeight(11, 25)

        ws.Cells(12, 0).PutValue("Date/ Time Generated: " & Format(Now, "MMM dd, yyyy hh:mm tt"))
        ws.Cells(12, 0).Style.Font.Size = fontsize
        cells.Merge(12, 0, 1, 8)
        ws.Cells.SetRowHeight(12, 25)

        ws.Cells(13, 0).PutValue("User ID:")
        If Not String.IsNullOrWhiteSpace(CStr2(WebSession.Instance.PeopleUserID)) Then
            ws.Cells(13, 0).PutValue("User ID: " & Common.WebRequest.ProData.ExecuteScalar("SELECT TOP 1 CompanyID FROM LibraryOrganizationEmployee WHERE PeopleID = " & ReadyString(CStr2(WebSession.Instance.PeopleUserID))))
        End If

        ws.Cells(13, 0).Style.Font.Size = fontsize
        cells.Merge(13, 0, 1, 8)
        ws.Cells.SetRowHeight(13, 25)
    End Function

    Function ExcelHeader(ByRef ws As Aspose.Cells.Worksheet, ByRef head As String, ByRef rowCtr As Integer, ByRef headerCtr As Integer, ByRef color As String, ByRef bordercolor As String, ByVal fontsize As Integer, ByVal rowheight As Integer, Optional ByVal isbold As Boolean = True)
        ws.Cells(rowCtr, headerCtr).PutValue(head)
        ws.Cells.SetRowHeight(rowCtr, rowheight)
        ws.Cells(rowCtr, headerCtr).Style.ForegroundColor = ColorTranslator.FromHtml(color)
        ws.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
        ws.Cells(rowCtr, headerCtr).Style.Font.Size = fontsize
        ws.Cells(rowCtr, headerCtr).Style.Font.IsBold = isbold
        ws.Cells(rowCtr, headerCtr).Style.IsTextWrapped = True
        ws.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
        ws.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center

        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.TopBorder).LineStyle = CellBorderType.Thin
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.TopBorder).Color = ColorTranslator.FromHtml(bordercolor)
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.BottomBorder).LineStyle = CellBorderType.Thin
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.BottomBorder).Color = ColorTranslator.FromHtml(bordercolor)
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.LeftBorder).LineStyle = CellBorderType.Thin
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.LeftBorder).Color = ColorTranslator.FromHtml(bordercolor)
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.RightBorder).LineStyle = CellBorderType.Thin
        'ws.Cells(rowCtr, headerCtr).Style.Borders(BorderType.RightBorder).Color = ColorTranslator.FromHtml(bordercolor)
        'Dim cellrange As Range = ws.Cells.CreateRange(rowCtr, headerCtr, 1, 1)
        'cellrange.SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, ColorTranslator.FromHtml(bordercolor))
    End Function

    Public Function ImportPosVendorsFromExcel() As String

        Try
            Dim pFilename = Common.ServerPath & "temp\PosVendors.xlsx"
            If System.IO.File.Exists(pFilename) = False Then
                Return "Error: File, " & pFilename & ", does not exist."
            End If

            Dim pError As String
            Dim pDT = ExcelToTable(pFilename, New String() {"Name"}, pError)
            If (String.IsNullOrEmpty(pError) = False) Then
                Return pError
            End If
            Dim pUniques = New Dictionary(Of String, String)
            For Each pDR In pDT.Rows
                Dim pName = CStr2(pDR("Name")).Trim()
                If String.IsNullOrWhiteSpace(pName) Then
                    Continue For
                End If
                Dim pKey = pName.ToLower()
                If pUniques.ContainsKey(pKey) Then
                    Continue For
                End If
                pUniques.Add(pKey, pName)
            Next

            Dim pInsertCount As Integer = 0
            For Each pUnique In pUniques.Values
                Dim pSql = "SELECT COUNT(*) FROM MobizLibraryPosVendors" & vbCrLf &
                    " WHERE Name = " & ReadyString(pUnique)
                If CInt2(Common.WebRequest.ProData.ExecuteScalar(pSql)) > 0 Then
                    Continue For
                End If
                Dim pIU = New InsertUpdate("MobizLibraryPosVendors")
                pIU("ID") = "NEWID()"
                pIU("Name") = ReadyString(pUnique)
                pIU("LastUpdated") = "GETDATE()"
                pIU("IsActive") = "1"
                pSql = pIU.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pInsertCount += 1
            Next
            Return "Insert count: " & pInsertCount
        Catch ex As Exception
            Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function GenerateCopyCacheInvoicesSql() As String

#If DEBUG Then
        Const pDestinationDatabase = "MobizHungryApe"
#Else
        Const pDestinationDatabase = "MobizCinco_Corporation"
#End If
        Const pTableName = "CacheInvoices"
        Dim pExcludeColumns As New List(Of String)
        pExcludeColumns.Add("FranchiseGroupID".ToLower())
        pExcludeColumns.Add("GroupName".ToLower())
        pExcludeColumns.Add("RegionID".ToLower())
        pExcludeColumns.Add("RegionName".ToLower())
        pExcludeColumns.Add("FromFranchisee".ToLower())

        Dim pOrgID = Common.WebRequest.Data("OrgID")
        If IsGuid(pOrgID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
        End If
        pOrgID = ReadyGuid(pOrgID)

        Dim pDtOrgs = WebRequest.Instance.Business.LibraryOrganization.GetAll(True)
        Dim pSourceDatabase = pDtOrgs.Select("ID = " & ReadyString(pOrgID)).FirstOrDefault()?.Item("Database")

        WebRequest.Instance.ProData.ExecuteSQL("USE " & EscapeSqlName(pDestinationDatabase))
        Dim pSql = "SELECT name " & vbCrLf &
            ",is_computed" & vbCrLf &
            " FROM sys.columns" & vbCrLf &
            " WHERE is_computed = 0 " & vbCrLf &
            " AND object_id = OBJECT_ID(" & ReadyString(pTableName) & ")"
        Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
        pSql = ""
        Dim pFieldNames As New List(Of String)
        For Each pDR As DataRow In pDT.Rows
            If pExcludeColumns.Contains(CStr2(pDR("name")).Trim().ToLower()) Then
                Continue For
            End If
            pFieldNames.Add(pDR("name"))
        Next

        pSql = "INSERT INTO " & pDestinationDatabase & ".dbo." & pTableName & vbCrLf & "("
        Dim pIndex As Integer = 0
        For Each pFieldName In pFieldNames
            If pIndex > 0 Then
                pSql &= ", "
            End If
            pSql &= EscapeSqlName(pFieldName) & vbCrLf
            pIndex += 1
        Next
        pSql &= ")" & vbCrLf & "SELECT "
        pIndex = 0
        For Each pFieldName In pFieldNames
            If pIndex > 0 Then
                pSql &= ", "
            End If
            pSql &= "tblSource." & EscapeSqlName(pFieldName) & vbCrLf
            pIndex += 1
        Next
        pSql &= " FROM " & EscapeSqlName(pSourceDatabase) & ".dbo." & pTableName & " tblSource" & vbCrLf &
            " WHERE tblSource.ID NOT IN (" & vbCrLf &
            " SELECT ID" &
            " FROM " & EscapeSqlName(pDestinationDatabase) & ".dbo." & pTableName & vbCrLf &
            ")"

        Return JsonDictionary.Serialize("Result", pSql)

    End Function

    Public Function UpdateCacheInvoiceDaily() As String

        Dim pDontRelease As Boolean

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vbA7FCTJX6NB!")
        End If

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            Dim pInput = JsonConvert.DeserializeObject(Of TransInvoice.UpdateCacheInvoicesDailyInput)(WebRequest.Instance.InputStreamText)
            Dim pError = pInput.Validate()
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()

            TimedThread(
                Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            pInput.Progress =
                                Sub(vText As String)
                                    mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] " & vText
                                End Sub
                            pBusiness.TransInvoice.UpdateCacheInvoicesDailyNotTallyWithCacheInvoices(pInput)
                        End Using
                    Catch ex As Exception
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex)
                        AppendToDebug("UpdateCacheInvoiceDaily - vbPDPTW95QZ3! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCacheInventoryMovement.Release()
                    End Try
                End Sub
            )

            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreCacheInventoryMovement.Release()
            End If
        End Try

    End Function

    Public Function UpdateCacheInvoiceHourly() As String

        Dim pDontRelease As Boolean

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vbA7FCTJX6NB!")
        End If

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            Dim pInput = JsonConvert.DeserializeObject(Of TransInvoice.UpdateCacheInvoicesDailyInput)(WebRequest.Instance.InputStreamText)
            Dim pError = pInput.Validate()
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()

            TimedThread(
                Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            pInput.Progress =
                                Sub(vText As String)
                                    mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] " & vText
                                End Sub
                            pBusiness.TransInvoice.UpdateCacheInvoicesHourlyNotTallyWithCacheInvoices(pInput)
                        End Using
                    Catch ex As Exception
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex)
                        AppendToDebug("UpdateCacheInvoiceHourly - vbVRENEVCWRA! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCacheInventoryMovement.Release()
                    End Try
                End Sub
            )

            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreCacheInventoryMovement.Release()
            End If
        End Try

    End Function

    Shared mUpdateCacheInvoiceDailyProgress As String
    Public Function UpdateCacheInvoiceDailyProgress() As String
        Try
            Dim pOut = ""
            If mSemaphoreCacheInventoryMovement.Wait(2000) Then
                pOut &= "Not busy. " & vbCrLf
                mSemaphoreCacheInventoryMovement.Release()
            Else
                pOut &= "Busy. " & vbCrLf
            End If

            If String.IsNullOrWhiteSpace(mUpdateCacheInvoiceDailyProgress) Then
                pOut &= "No progress yet" & vbCrLf
                Return pOut
            End If

            pOut &= mUpdateCacheInvoiceDailyProgress
            pOut = pOut.Replace(vbCrLf, "<br />" & vbCrLf)
            Return pOut
        Catch ex As Exception

        End Try
    End Function

    Public Function DeleteCacheInvoicesDailyDuplicates() As String

        Dim pDontRelease As Boolean

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vbA7FCTJX6NB!")
        End If

        Try
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()

            TimedThread(
                Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            Dim pResult = pBusiness.TransInvoice.DeleteCacheInvoicesDailyDuplicates()
                            mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Records deleted: " & pResult
                        End Using
                    Catch ex As Exception
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex)
                        AppendToDebug("DeleteCacheInvoicesDailyDuplicates - vbPDPTW95QZ3! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCacheInventoryMovement.Release()
                    End Try
                End Sub
            )

            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreCacheInventoryMovement.Release()
            End If
        End Try

    End Function

    Public Function CacheSalesTables()

        Dim pDontRelease As Boolean

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vbA7FCTJX6NB!")
        End If

        Try
#If Not DEBUG Then
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If
#End If
            Dim pOrgID = Common.WebRequest.Data("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()
            Dim pFowstID = Common.WebRequest.Data("FowstID")
            Dim pInvoiceDateFrom = CDate(Common.WebRequest.Data("InvoiceDateFrom")).Date
            Dim pInvoiceDateTo = CDate(Common.WebRequest.Data("InvoiceDateTo")).Date

            TimedThread(
                Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            Dim pError As String
                            pBusiness.CacheSalesTables.InvoiceDateFrom = pInvoiceDateFrom ' "2025-08-22 13:45"
                            pBusiness.CacheSalesTables.InvoiceDateTo = pInvoiceDateTo ' "2025-08-24 08:38"
                            mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] pBusiness.CacheSalesTables.Execute "
                            pBusiness.CacheSalesTables.Execute(pError)
                            If String.IsNullOrWhiteSpace(pError) = False Then
                                mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] CacheSalesTables error: " & pError
                            Else
                                mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] CacheSalesTables success"
                            End If
                        End Using
                    Catch ex As ProSqlException
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex) & vbCrLf &
                            "SQL: " & ex.Sql
                        AppendToDebug("CacheSalesTables - vbY43Q866W93! " & vbCrLf & ex.Message, True, ex, True)
                    Catch ex As Exception
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex)
                        AppendToDebug("CacheSalesTables - vbY43Q866W93! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCacheInventoryMovement.Release()
                    End Try
                End Sub
            )

            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreCacheInventoryMovement.Release()
            End If
        End Try

    End Function
    Public Function FillMissingPaymentData()
        ' localhost:8080/utilities/FillMissingPaymentData?OrgID=9262FC46-DE29-4497-BDF2-8832F5E7576D&FowstID=2DF3D2F9-550C-4B0C-B896-FBFB54D69390&DeviceID=D4F7F572-9C44-44E1-A4B8-31261FE1EE7D
        ' 20.24.43.82/utilities/FillMissingPaymentData?OrgID=d362ffdb-15d1-451f-97dc-c567b94fb410&FowstID=d844f4fd-924d-4e3d-8f4d-da651411fe1c&DeviceID=6ee8f7bf-7e80-4f46-b758-a11042abd993
        Dim pDontRelease As Boolean

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vbA7FCTJX6NB!")
        End If

        Try
            '#If Not DEBUG Then
            '            If VerifyPassword() = False Then
            '                Return JsonDictionary.Serialize("Error", "Invalid password")
            '            End If
            '#End If
            'Dim pOrgID = Common.WebRequest.Data("OrgID")
            Dim pOrgID = Common.WebRequest.QueryString("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()
            Dim pFowstID = Common.WebRequest.QueryString("FowstID")
            Dim pDeviceID = Common.WebRequest.QueryString("DeviceID")
            'Dim pInvoiceDateFrom = CDate(Common.WebRequest.Data("InvoiceDateFrom")).Date
            'Dim pInvoiceDateTo = CDate(Common.WebRequest.Data("InvoiceDateTo")).Date

            TimedThread(
                Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            Dim pError As String
                            pBusiness.TransCustomerPayment.FillMissingPaymentData(
                                vFowstID:=pFowstID,
                                vDeviceID:=pDeviceID)
                            If String.IsNullOrWhiteSpace(pError) = False Then
                                mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] FillMissingPaymentData error: " & pError
                            Else
                                mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] FillMissingPaymentData success"
                            End If
                        End Using
                    Catch ex As Exception
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex)
                        AppendToDebug("FillMissingPaymentData - vbTQLPYAPWHM! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCacheInventoryMovement.Release()
                    End Try
                End Sub
            )

            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreCacheInventoryMovement.Release()
            End If
        End Try

    End Function

    Public Function SendSalesTables()

        Dim pDontRelease As Boolean

        If mSemaphoreCacheInventoryMovement.Wait(2000) = False Then
            Return JsonDictionary.Serialize("Error", "Another FranchiseeDataExporter or CacheInventoryMovement process is running. ref: vb3RLYS3VRRX!")
        End If

        Try
#If Not DEBUG Then
            If VerifyPassword() = False Then
                Return JsonDictionary.Serialize("Error", "Invalid password")
            End If
#End If
            Dim pOrgID = Common.WebRequest.Data("OrgID")
            If IsGuid(pOrgID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid or missing OrgID")
            End If
            pOrgID = ReadyGuid(pOrgID)
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDatabase = WebRequest.Instance.ProData.GetCurrentDatabase()
            Dim pFowstID = Common.WebRequest.Data("FowstID")
            Dim pInvoiceDateFrom = CDate(Common.WebRequest.Data("InvoiceDateFrom")).Date
            Dim pInvoiceDateTo = CDate(Common.WebRequest.Data("InvoiceDateTo")).Date

            TimedThread(
                Async Sub()
                    Try
                        Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                            pProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
                            Dim pBusiness = New Business(pProData)
                            Dim pError As String
                            pBusiness.CacheSalesTables.InvoiceDateFrom = pInvoiceDateFrom ' "2025-08-22 13:45"
                            pBusiness.CacheSalesTables.InvoiceDateTo = pInvoiceDateTo ' "2025-08-24 08:38"
                            Await pBusiness.CacheSalesTables.SendAllTables()
                            If String.IsNullOrWhiteSpace(pError) = False Then
                                mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] SendSalesTables error: " & pError
                            Else
                                mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] SendSalesTables success"
                            End If
                        End Using
                    Catch ex As Exception
                        mUpdateCacheInvoiceDailyProgress = "[" & Now.ToString(cGlobal.FormatDateTimeSql) & "] Error: " & ex.Message &
                            GetStackTrace(, ex)
                        AppendToDebug("SendSalesTables - vb2HPYUJVPLL! " & vbCrLf & ex.Message, True, ex, True)
                    Finally
                        mSemaphoreCacheInventoryMovement.Release()
                    End Try
                End Sub
            )

            pDontRelease = True
            Return JsonDictionary.Serialize("Result", "process was successfully started.")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreCacheInventoryMovement.Release()
            End If
        End Try

    End Function

    Public Function SendSalesTablesResult() As String
        Dim pFolder = System.IO.Path.Combine(Common.ServerPath, "temp\logs\SalesTable")
        Dim pFiles = New System.IO.DirectoryInfo(pFolder).GetFiles("*.log")
        Dim pLatestFile = pFiles.OrderByDescending(Function(f) f.LastWriteTime).FirstOrDefault()
        Dim pFileText = GetFileText(pLatestFile.FullName, 10123456)
        Return pFileText.Replace(vbCrLf, "<br />")
    End Function
End Class