Public Class WebSession
    Inherits ProWeb.WebSession

    Protected mRole As String = ""
    Public Property dtCustomer As DataTable
    Public Property EmployeeID As String
    Public Property TeamID As Integer
    Public Property TeamName As String
    Public Property Month As String
    Public Property BranchName As String
    Public Property CustomerID As String
    Public Property CustomerName As String
    Public Property IsNew As Integer

    Public Property BranchID As Integer
    Public Property WarehouseID As String
    Public Property SkuID As String
    Public Property countType As Integer
    Public Property IncentiveItemID As String
    Public Property IncentiveItemName As String

    Public Property BackOrderSKUID As String
    Public Property OrderID As String
    Public Property SalesBackOrderID As String
    Public Property TransSalesBackOrderItems As String

    Public Property DrpCacheStart As Date?

    Public Property SubscriberOrganizationID As String = ""
    ''' <summary>
    ''' LibraryCustomersAddresses.ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubscriberOrganizationBranchID As String = ""
    Public Property SubscriberOrganizationBranchName As String = ""
    '<Obsolete("i'm not using this - arvin (aug 12, 2013)")>
    'Public Property SubscriberOrganizationDeviceID As String = ""
    Public Property DeviceSerial As String
    Public Property SubscriberTradeNameID As String
    Public Property SubscriberPropertyAndPlantID As String
    Public Property PeopleID As String
    Public Property PropertyAndPlantID As String
    Public Property DeviceID As String
    Public Property DevicePrinterID As String
    Public Property DevicePrinterDeviceID As String
    Public Property ReceiptSettingsID As String
    Public Property CountryID As String
    Public Property VocabularyID As String
    Public Property VocabularyLabelID As String
    Public Property PIMFeatureID As String
    Public Property PIMFeatureGroupID As String
    Public Property PIMMeasureID As String
    Public Property PIMMeasureSignID As String
    Public Property PIMUNSPSCCommodityID As String
    Public Property PIMUNSPSCCommodityFeatureID As String
    Public Property PIMProductFamilyID As String
    Public Property PIMProductID As String
    Public Property PIMTemplateID As String
    Public Property PIMTemplateUploaded As String
    Public Property PIMTemplateArray As New ArrayList 'added by Janine Oct 14 5pm

    Public Property IsEmailVerified As Boolean 'added by Janine Nov 4 2pm
    Public Property IsFirstPartySignUp As Boolean

    Public Property HasCurrentRequest As Boolean
    Dim mPeopleUserID As String
    Public Property SecurityRoleID As String
    Public Property SecurityHasAccess As String
    Public FowstID As String
    Public Property POSSalesReportType As Integer

    Public Property OrganizationID As String
    Public Property OrganizationFowstID As String
    Public IsMobile As Boolean

    Public Property SerializedString As String

    Public Property PeopleUserID As String
        Get

            Return mPeopleUserID

        End Get
        Set(value As String)

            mPeopleUserID = value

        End Set
    End Property

    'Public Shared Sub Start(ByVal vWebSession As WebSession)

    '    Dim pTkt As Web.Security.FormsAuthenticationTicket
    '    Dim pCookieStr As String
    '    Dim pCookie As HttpCookie

    '    HttpContext.Current.Session.Clear()
    '    HttpContext.Current.Session.Add("WebSession", vWebSession)

    '    pTkt = New Web.Security.FormsAuthenticationTicket( _
    '        1, "guess", DateTime.Now(), _
    '        DateTime.Now.AddMinutes(20), False, New Guid().ToString)
    '    pCookieStr = Web.Security.FormsAuthentication.Encrypt(pTkt)
    '    pCookie = New HttpCookie(Web.Security.FormsAuthentication.FormsCookieName, pCookieStr)
    '    HttpContext.Current.Response.Cookies.Add(pCookie)

    'End Sub

    Public Shared ReadOnly Property Instance As WebSession
        Get

            Return Common.WebSession

        End Get
    End Property

    Public Overrides Property UserID() As String
        Get

            Return mUserID

        End Get
        Set(ByVal value As String)

            Dim pDR As DataRow
            Dim pWS As WebSession

            MyBase.UserID = value
            If CStr2(value).Length = 0 Then
                Exit Property
            End If
            pWS = Instance
            pWS.mUserID = value

            If IsNumeric(value) Then
                '// employee ID
                pWS.EmployeeID = value
                pWS.mRole = "Employee"
                pDR = WebRequest.Instance.Business.EmployeeProfiles.ById(value)
                If pDR IsNot Nothing Then
                    pWS.FirstName = pDR("FirstName")
                    pWS.LastName = pDR("LastName")
                    pWS.FullName = pDR("FullName")
                    'pWS.mUsername = WebRequest.Instance.Business.MainSecurity.UserAccess.Username
                    'pWS.UserAccess = WebRequest.Instance.Business.MainSecurity.UserAccess
                End If
            Else
                ' todo
            End If
            If value = "admin" Then
                pWS.mUserID = "admin"
                pWS.mUsername = "admin"
                pWS.mRole = "Admin"
                Exit Property
            End If

            'pDR = Common.WebRequest.Business.LibUsers.ByID(value)
            'pOut.mUserID = pDR("ID") & ""
            'pOut.mUsername = pDR("Username") & ""
            'pOut.mRole = pDR("Role") & ""
            'Common.WebRequest.Business.LibUsers.UpdateLastOnline(pOut.mUserID)
            'pWS.OrganizationID = WebRequest.Instance.Business.SystemDefaults.PosSubscriberCustomerID

        End Set
    End Property

    Public ReadOnly Property Username As String
        Get

            Return mUsername

        End Get
    End Property

    Public ReadOnly Property Role As String
        Get

            Return mRole

        End Get
    End Property
End Class