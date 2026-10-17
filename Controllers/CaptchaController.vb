Public Class CaptchaController
    Inherits System.Web.Mvc.Controller

    ''' <summary>
    ''' get image captcha
    ''' </summary>
    ''' <param name="vKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Index(vKey As String) As ActionResult

        Dim pFilename As String

        If CStr2(vKey).Length < 10 Then Return Nothing
        vKey = vKey.Substring(0, 10)
        pFilename = Common.ServerPath & "temp\sessions\" & HttpContext.Session.SessionID & "\captchas\" & vKey & ".jpg"
        If IO.File.Exists(pFilename) Then
            Response.TransmitFile(pFilename)
        End If

    End Function

    Public Function Create() As String

        'Return JsonDictionary.Serialize("Key", Common.WebSession.Captchas.Create)

    End Function
End Class
