Namespace WebProj
    Public Class ReceivingController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Receiving

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Receiving and Transfers")) Then Exit Function
            End If

            ' ''If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            Return View()
        End Function

        Function StockViewer() As ActionResult
            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            'If (Not SecurityRole.Instance.HandleRoleAccess("Receiving and Transfers")) Then Exit Function

            Return View()
        End Function

        <HttpPost()>
        Function GetAllReceiving() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetAllReceiving())
        End Function

        <HttpPost()>
        Function GetAllReceivingSKU() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetAllReceivingSKU())
        End Function

        <HttpPost()>
        Function GetStockUnitMeasures() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("UnitMeasure", s.GetStockUnitMeasures())
        End Function

        <HttpPost()>
        Function SaveReceiving() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Save", s.SaveReceiving(WebRequest.Instance.Data("ID"), CBool(WebRequest.Instance.Data("IsNew"))))

        End Function

        <HttpPost()>
        Function GetOrganizationID()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("OrganizationID", WebSession.Instance.SubscriberOrganizationID)
        End Function

        Function GetReportHeaderInfo() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.GetReportHeaderInfo)

        End Function
    End Class
End Namespace