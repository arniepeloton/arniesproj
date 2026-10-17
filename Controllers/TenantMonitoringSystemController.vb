Public Class TenantMonitoringSystemController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function

    'Function Index() As ActionResult

    '    If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
    '        If WebRequest.Instance.HandleAccess = False Then Exit Function
    '        ' If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
    '    End If

    '    Return View()

    'End Function

    Function TenantMonitoringSystemDashboard() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            ' If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
        End If

        Return View()

    End Function

    Function GetPOSSalesReportBySalesDistributionByTime() As String
        Dim tenant As New TenantMonitoringSystem

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        Return JsonDictionary.Serialize("ReportPOS", tenant.GetPOSSalesReportBySalesDistributionByTime())
    End Function

    Function GetTotalForAllFowst() As String
        Dim tenant As New TenantMonitoringSystem

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        Return JsonDictionary.Serialize("Results", tenant.GetTotalForAllFowst())
    End Function

    Function GetDistibutionByFOWST() As String
        Dim tenant As New TenantMonitoringSystem

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        Return JsonDictionary.Serialize("Results", tenant.GetDistibutionByFOWST())
    End Function

    Function GetTrendByFOWST() As String
        Dim tenant As New TenantMonitoringSystem

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAccess = False Then Exit Function

        Return tenant.GetTrendByFOWST()
    End Function

    'Function SignIn() As ActionResult
    '    If CStr2(Common.WebRequest.QueryString("mode")).ToLower = "logout" Then
    '        Common.WebSession.UserID = ""
    '    End If

    '    FormsAuthentication.SignOut()

    '    Response.Cache.SetCacheability(HttpCacheability.NoCache)
    '    Response.Cache.SetNoStore()

    '    Return View()

    'End Function

End Class
