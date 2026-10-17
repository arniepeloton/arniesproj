Imports Aspose.Cells
Imports System.Threading

Namespace WebProj

    Public Class ProductMappingController
        Inherits System.Web.Mvc.Controller

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
        Function Index(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String, ByVal UploaderKey As String) As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If Request.Files.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Please upload a file.")
            End If

            Dim pFile = Request.Files(0)
            If pFile.ContentLength = 0 Then
                Return JsonDictionary.Serialize("Error", "Please upload a file with contents.")
            End If

            Dim pFowstID = Request.Form("FowstID")
            If String.IsNullOrWhiteSpace(pFowstID) Then
                Return JsonDictionary.Serialize("Error", "FowstID is required.")
            End If
            If IsGuid(pFowstID) = False Then
                Return JsonDictionary.Serialize("Error", "FowstID must be a GUID.")
            End If

            Dim pFilename = Common.ServerPath & "temp\ProductMappingUploads\" & DateTimeStamp() & ".xlsx"
            CreateFolder(pFilename)
            pFile.SaveAs(pFilename)

            Dim pError As String
            Dim pWarning As String
            WebRequest.Instance.Business.LibraryProductMap.ImportExcel(WebRequest.Instance.OrgID,
                pFowstID,
                WebSession.Instance.PeopleUserID,
                pFilename,
                pError, pWarning)
            If String.IsNullOrWhiteSpace(pError) Then
                Dim pOut = New JsonDictionary()
                pOut.Add("Result", "ok")
                pOut.Add("Warning", pWarning)
                Return pOut.Serialize()
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function

        <HttpPost>
        Public Function CreateExcel() As String

            AppendToLog("ProductMappingController.CreateExcel vbTEWDFTB4JB!")

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFowstID As String = WebRequest.Instance.Data("FowstID")
            If String.IsNullOrWhiteSpace(pFowstID) Then
                Return JsonDictionary.Serialize("Error", "FowstID is required.")
            End If

            AppendToLog("ProductMappingController.CreateExcel vb7K9G6DTCNF!")
            Dim pOut = WebRequest.Instance.Business.LibraryProductMap.CreateExcel(pFowstID)
            AppendToLog("ProductMappingController.CreateExcel vbRV7XSR243J!")

            Return JsonDictionary.Serialize("Result", pOut)

        End Function
    End Class

End Namespace