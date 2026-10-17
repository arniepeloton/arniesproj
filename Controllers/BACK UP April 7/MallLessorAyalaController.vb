Namespace WebProj
    Public Class MallLessorAyalaController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Mall Lessor Report")) Then Exit Function
            End If

            Return View()
        End Function

        Function DownloadReportFile() As ActionResult
            Dim pFileTitle As String
            Dim pContentType As String
            Dim pSourceURL As String
            Dim pLoginHash As String
            Dim pDR As DataRow

            pFileTitle = Common.WebRequest.QueryString("ReportLink")
            pLoginHash = Common.WebRequest.QueryString("LoginHash")
            If CStr2(pLoginHash).Length <> 50 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            Else
                pDR = MallLessorController.LoginData.GetByLoginHash(pLoginHash)
                If pDR Is Nothing Then
                    Return Nothing
                End If
                If ReadyGuid(IO.Path.GetFileNameWithoutExtension(pFileTitle)) <> ReadyGuid(pDR("OrganizationID")) Then
                    Return Nothing ' not same OrganizationID
                End If
            End If

            pSourceURL = Common.ServerPath & "/Content/AyalaTenant/" & pFileTitle
            pContentType = "application/zip"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        <HttpPost()>
        Function GenerateReportFile() As String
            Dim strGeneratedLink As String
            Dim clsMallLessorAyala As New MallLessorAyala
            Dim dtaTable As DataTable
            Dim dtDate As Date
            Dim strTenantCode As String
            Dim strTenantName As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If IsGuid(WebRequest.Instance.FowstID) = False Then Return JsonDictionary.Serialize("Error", "Invalid FOWST")

            If (Common.WebRequest.Data("RangeType") = "Today") Then
                dtDate = DateTime.Today
            ElseIf (Common.WebRequest.Data("RangeType") = "Yesterday") Then
                dtDate = DateTime.Today.AddDays(-1)
            ElseIf (Common.WebRequest.Data("RangeType") = "By Date") Then
                dtDate = Convert.ToDateTime(Common.WebRequest.Data("SalesDate"))
            End If

            dtaTable = clsMallLessorAyala.getAyalaTenantInfo()

            If (dtaTable.Rows.Count > 0) Then
                strGeneratedLink = clsMallLessorAyala.GenerateLessorSalesReport(dtDate, dtaTable.Rows(0).Item("TenantCode").ToString(), dtaTable.Rows(0).Item("TenantName").ToString())
            End If

            strReturn = JsonDictionary.Serialize("GeneratedReport", strGeneratedLink)

            Return strReturn

        End Function

    End Class
End Namespace