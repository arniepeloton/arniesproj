Imports Newtonsoft.Json

Public Class ProductMakerController
    Inherits System.Web.Mvc.Controller

    <HttpPost()>
    Public Function Save() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInput = JsonConvert.DeserializeObject(Of TransBomMaker.SaveInputJson)(Common.WebRequest.InputStreamText)
        Dim pUuid = Common.WebRequest.Data("UUID")
        pInput.DeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
        pInput.UserID = WebSession.Instance.PeopleUserID
        Dim pResult = WebRequest.Instance.Business.TransBomMaker.Save(pInput)

        Return JsonConvert.SerializeObject(pResult)

    End Function

    <HttpPost()>
    Public Function Cart() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pID = Common.WebRequest.Data("ID")
        Dim pDS = WebRequest.Instance.Business.TransBomMaker.GetCart(pID)

        Dim pOut = New JsonDictionary()
        If pDS.Tables("Master").Rows.Count > 0 Then
            pOut.Add("Master", pDS.Tables("Master").Rows(0))
        Else
            pOut.Add("Master", pDS.Tables("Master").NewRow())
        End If
        pOut.Add("Products", pDS.Tables("Products"))

        Return pOut.Serialize()

    End Function

    <HttpPost>
    Public Function GetStockInfo() As String

        Dim pStockInfo As TransBomMaker.StockInfo
        Dim pProductID As String = Common.WebRequest.Data("ProductID")
        Dim pParentID As String = Common.WebRequest.Data("ParentID")

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pStockInfo = WebRequest.Instance.Business.TransBomMaker.GetStockInfo(pProductID, pParentID)

        Return JsonConvert.SerializeObject(pStockInfo)

    End Function

    <HttpPost()>
    Public Function SaveProduct() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInput = JsonConvert.DeserializeObject(Of TransBomMaker.SaveProductInputJson)(Common.WebRequest.InputStreamText)
        Dim pUuid = Common.WebRequest.Data("UUID")
        'pInput.Master.DeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
        'pInput.Master.UserID = WebSession.Instance.PeopleUserID
        Dim pResult = WebRequest.Instance.Business.TransBomMaker.SaveProduct(pInput)

        Return JsonDictionary.Serialize("Result", "ok")
        Return JsonConvert.SerializeObject(pResult)

    End Function

    Public Function CreatePdf()

        Dim pTransID As String
        Dim pReport As ProReport
        Dim pFilename As String
        Dim pHeaderInfo As LibraryOrganization.HeaderInfo
        Dim pUuid As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pUuid = Common.WebRequest.Data("UUID")
        If String.IsNullOrWhiteSpace(pUuid) Then
            Return JsonDictionary.Serialize("Error", "UUID is required.")
        End If
        If pUuid.Length < 1 Then
            Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
        End If

        pTransID = Common.WebRequest.Data("TransID")

        Dim pDS = WebRequest.Instance.Business.TransBomMaker.GetCart(pTransID)

        If pDS.Tables("Master").Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Error", "Transaction does not exist.")
        End If

        Dim pDT = WebRequest.Instance.Business.TransBomMaker.GetContents(pTransID)
        pDT.TableName = "Contents"
        pDS.Tables.Add(pDT)

        For pDocumentType As Integer = 1 To 2
            pReport = New ProReport
            pReport.AutoFillHeader = False
            If pDocumentType = 1 Then
                pReport.SourceFilename = "ProductMaker"
            Else
                pReport.SourceFilename = "MaterialsRequest"
            End If
            pReport.AddTable("Master", pDS.Tables("Master"))
            pReport.AddTable("Products", pDS.Tables("Products"))
            pReport.AddTable("Contents", pDS.Tables("Contents"))

            pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
            pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
            pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress
            pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos

            pReport.ExportXmlFile()

            pFilename = Common.ServerPath & "temp\PrintablePdf\" & CreateValidFilename(pUuid) & "-" & DateTimeStamp() & ".pdf"
            CreateFolder(pFilename)
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
        Next pDocumentType

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

End Class
