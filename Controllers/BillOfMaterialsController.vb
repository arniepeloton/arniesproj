Namespace WebProj
    Public Class BillOfMaterialsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /DiscountSettings

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("Bill of Materials")) Then Exit Function
            End If

            Return View()
        End Function

       
    End Class
End Namespace