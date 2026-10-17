Public Class LoyaltyProgramController
    Inherits System.Web.Mvc.Controller

    ''' <summary>
    ''' get image captcha
    ''' </summary>
    ''' <param name="vKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Index() As ActionResult


    End Function

    Public Function Create() As String

        'Return JsonDictionary.Serialize("Key", Common.WebSession.Captchas.Create)

    End Function

    Public Function GetVPlusPoints() As String
        Dim pSql As String
        Dim Balance As Double

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = "Select top 1 balance from TransCustomerPaymentType where ClientID = " & ReadyString(Common.WebRequest.Data("CustomerID")) & vbCrLf &
            " order by LastUpdated desc"

        Balance = CDec2(Common.WebRequest.ProData.ExecuteScalar(pSql))

        Return JsonDictionary.Serialize("Result", Balance)
    End Function
End Class
