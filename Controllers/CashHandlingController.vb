Public Class CashHandlingController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function CashHandling() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function DepositMonies() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function DepositReport() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function MoniesForDepositOrTurnOver() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function GetDeposits() As String
        Dim s As New CashHandling
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'If Common.WebRequest.Data("IsGetDetails") = "True" Then
        '    Return JsonDictionary.Serialize("Results", s.GetDepositsDetails())
        'Else
        Return JsonDictionary.Serialize("Results", s.GetDeposits())
        'End If

    End Function

    Function GetTurnover() As String
        Dim s As New CashHandling
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'If Common.WebRequest.Data("IsGetDetails") = "True" Then
        '    Return JsonDictionary.Serialize("Results", s.GetDepositsDetails())
        'Else
        Return JsonDictionary.Serialize("Results", s.GetTurnover())
        'End If
    End Function

    Function GetAllDeposits() As String
        Dim s As New CashHandling
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'Deposits lang ni tanan. walay turn over

        Return JsonDictionary.Serialize("Results", s.GetAllDeposits())

    End Function
End Class
