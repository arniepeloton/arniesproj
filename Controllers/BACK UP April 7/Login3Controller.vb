Public Class Login3Controller
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CStr2(Common.WebRequest.QueryString("mode")).ToLower = "logout" Then
            Common.WebSession.UserID = ""
        End If

        Return View()

    End Function

    '    <HttpPost()>
    '    Public Function Login() As String

    '        Dim pUsername As String
    '        Dim pPassword As String
    '        Dim pBankUser As New Dictionary(Of String, String)
    '        Dim pOut As JsonDictionary
    '        Dim pCookie As HttpCookie

    '        Try
    '            pUsername = Common.WebRequest.Data("Username")
    '            pPassword = Common.WebRequest.Data("Password")
    '            If WebRequest.Instance.Business.MainSecurity.SetAccessByUsernameAndPassword(pUsername, pPassword) Then
    '                Request.Cookies.Clear()
    '                WebSession.Instance.UserID = WebRequest.Instance.Business.MainSecurity.UserAccess.EmployeeID

    '                pOut = New JsonDictionary

    '                If WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable Then
    '                    pOut("AllowSfa") = 1
    '                End If
    '                If WebSession.Instance.UserAccess.PointOfSales_PointOfSales.IsEnable Then
    '                    pOut("AllowPos") = 1
    '                End If
    '                If WebSession.Instance.UserAccess.Memos_SalesReturns.IsEnable Then
    '                    pOut("AllowSalesReturns") = 1
    '                End If
    '                pOut.Add("Text", "Ok")

    '                If CDec2(Common.WebRequest.Data("IsCookieless")) <> 0 Then
    '                    pOut.Add("ASP.NET_SessionId", HttpContext.Session.SessionID)
    '                    '// the ff commented code dont work
    '                    'For Each pCookieName As String In Response.Cookies
    '                    '    pCookie = Response.Cookies(pCookieName)
    '                    '    pOut.Add(pCookie.Name, pCookie.Value)
    '                    'Next pCookieName
    '                End If

    '#If DEBUG Then
    '                Dim pJson As JsonDictionary

    '                If Environment.MachineName.ToUpper = "IT" _
    '                    AndAlso Debugger.IsAttached Then

    '                    pJson = New JsonDictionary
    '                    pJson.Add("Username", pUsername)
    '                    pJson.Add("Password", pPassword)
    '                    FileText(Common.ServerPath & "temp\login.txt") = pJson.ToString()
    '                End If
    '#End If
    '                Return pOut.Serialize
    '            Else
    '                Return JsonDictionary.Serialize("Error", "Invalid username and/or password")
    '            End If
    '        Catch ex As Exception
    '            Return JsonDictionary.Serialize("Error", Truncate(ex.Message, 500))
    '        End Try

    '    End Function

    <HttpPost()>
    Public Function TestCookies() As String

        Dim pOut As JsonDictionary

        pOut = New JsonDictionary
        pOut.Add("MinVersion", 40)
        Try
            If Request.Cookies("TestCookie") IsNot Nothing _
                AndAlso Request.Cookies("TestCookie").Value = "test" Then

                Response.Cookies("TestCookie").Value = "success"
                pOut.Add("Result", "ok")
            Else
                pOut.Add("Result", "Server was unable to receive cookies")
            End If
        Catch ex As Exception
            pOut.Add("Result", "Server was unable to receive cookies")
        End Try

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function GetUser() As String

        Dim pOut As JsonDictionary
        Dim pDR As DataRow
        Dim pError As String

        If Client.CompareLocalTimeAndSqlServerTime(pError) = False Then
            Return JsonDictionary.Serialize("Error", pError)
        End If

        'If WebRequest.Instance.HandleAjax = False Then Return ""
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary
        pOut("FirstName") = WebSession.Instance.FirstName
        pOut("FullName") = WebSession.Instance.FullName

        If IsGuid(Common.WebRequest.Data("FowstID")) Then
            pDR = WebRequest.Instance.Business.LibraryCustomers.GetByFowstID(Common.WebRequest.Data("FowstID"))
            If pDR IsNot Nothing Then
                pOut("FowstID") = CStr2(pDR("ID"))
                pOut("FowstName") = pDR("Name")
            End If
        End If

        If WebRequest.Instance.HandleOrganization(True) Then
            pOut("AllowPriceAdjustment") = If(WebRequest.Instance.Business.SystemDefaults.AllowPriceAdjustment, 1, 0)
        End If
        'pOut("PosSubscriberCustomerID") = WebSession.Instance.SubscriberOrganizationID
        'pOut("CustomerAddressID") = WebSession.Instance.SubscriberOrganizationBranchID
        'pOut("CustomerAddressName") = WebSession.Instance.SubscriberOrganizationBranchName

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function VerifyDevice() As String

        Dim pSerial As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.FowstID = "" Then Return JsonDictionary.Serialize("Error", "Invalid foswt ID")

        Try
            If CStr2(WebRequest.Instance.DeviceID) = "" Then
                ' // dummy if; just execute DeviceID property
            End If
            Return JsonDictionary.Serialize("Result", "ok") ' we should be here if there is no error
        Catch ex As System.Exception
            pSerial = WebSession.Instance.DeviceSerial
            If CStr2(pSerial) = "" Then
                pSerial = "blank serial number"
            Else
                pSerial = "serial number of " & pSerial
            End If
            Return JsonDictionary.Serialize("Error", "Device with a " & pSerial & " is not yet enrolled in the current organization and FOWST. Please setup via Mobiz Home -> System Administration -> Security Settings -> Devices -> Enroll a Device or Computer")
        End Try

    End Function
End Class
