Namespace WebProj
    Public Class MallLessorSMController
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
            Dim pOrgID As String
            Dim pFowstID As String
            Dim pContentType As String
            Dim pSourceURL As String
            Dim pLoginHash As String
            Dim pDR As DataRow

            pOrgID = Common.WebRequest.QueryString("ReportOrgID")
            pFowstID = Common.WebRequest.QueryString("ReportFowstID")
            pFileTitle = Common.WebRequest.QueryString("ReportLink")

            pLoginHash = Common.WebRequest.QueryString("LoginHash")
            If (Not pLoginHash Is Nothing) Then
                If CStr2(pLoginHash).Length <> 50 Then
                    If WebRequest.Instance.HandleAccess() = False Then Exit Function
                Else
                    pDR = MallLessorController.LoginData.GetByLoginHash(pLoginHash)
                    If pDR Is Nothing Then
                        Return Nothing
                    End If
                End If
            End If

            pSourceURL = Common.ServerPath & "temp\" & pOrgID & "\" & pFowstID & "\" & pFileTitle
            pContentType = "application/zip"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        <HttpPost()>
        Function GenerateReportFile() As String
            Dim strGeneratedLink As String
            Dim clsMallLessorSM As New MallLessorSM
            Dim dtaTable As DataTable
            Dim dtDate As Date
            Dim blnIsGenerateFlg As Boolean
            Dim strTenantCode As String
            Dim strTenantName As String
            Dim strReturn As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

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

            blnIsGenerateFlg = Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("FlagClose")))

            strGeneratedLink = clsMallLessorSM.GenerateLessorSalesReport(dtDate, blnIsGenerateFlg, "", "")

            strReturn = JsonDictionary.Serialize("GeneratedReport", strGeneratedLink)

            Return strReturn

        End Function

    End Class
End Namespace