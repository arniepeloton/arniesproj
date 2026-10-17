Imports WebProj.WebProj

Partial Public Class WebRequest
    Inherits ProWeb.WebRequest
    ' project-specific WebRequest class
    ' Put code common to all web project in WebRequest.vb


    Dim mBusiness As Business
    Dim mOrgID As String = ""
    Dim mDatabase As String ' Mobiz.LibrarySubscription.Database
    Dim mDeviceID As String
    Dim mFreeUser As Boolean ' set during HandleOrganization

    Public Shared ReadOnly Property Instance As WebRequest
        Get

            Return Common.WebRequest

        End Get
    End Property

    Public ReadOnly Property Business As Business
        Get

            If mBusiness Is Nothing Then
                mBusiness = New Business(MyBase.ProData)
            End If

            Return mBusiness

        End Get
    End Property

    Public Function HandleAccess() As Boolean

        If WebSession.Instance.PeopleUserID Is Nothing Then
            Transfer("/SignIn")
            Return False
        End If

        Return True

    End Function

    Public Function HandleAjax() As Boolean

        If CStr2(WebSession.Instance.PeopleUserID).Trim <> "" Then Return True
        If CStr2(Data("Username")) <> "" _
            AndAlso (
                    CStr2(Data("UserPassword")) <> "" _
                    Or CStr2(Data("HashedPassword")) <> ""
            ) Then

            Call New PeopleController().CheckPeopleUserAccount()
            If CStr2(WebSession.Instance.PeopleUserID).Trim <> "" Then Return True
        End If

    End Function

    Public Sub RedirectToLogin(ByVal vMobile As Boolean)

        If vMobile Then
            Transfer("/login3")
        Else
            Transfer("/login")
        End If

    End Sub

    'Private Sub WebRequest_DatabaseConnected() Handles Me.DatabaseConnected

    '    Business.MainSecurity.UserAccess = WebSession.Instance.UserAccess

    'End Sub

    'Public Function HandleWebKey() As Boolean

    '    Dim pWebKey As String

    '    Try
    '        pWebKey = Common.WebRequest.QueryString("webkey")
    '        If CStr2(pWebKey).Length <> 50 Then Exit Function

    '        Dim pDT As DataTable
    '        Dim pSql As String

    '        pSql = "SELECT ID" &
    '               " FROM MainSecurityAccess" &
    '               " WHERE WebKey = " & ReadyString(pWebKey)
    '        pDT = Common.WebRequest.ProData.Execute(pSql)
    '        If pDT.Rows.Count > 0 Then
    '            WebRequest.Instance.Business.MainSecurity.SetAccessByAccessID(CStr2(pDT.Rows(0)("ID")))
    '            Common.WebSession.UserID = WebRequest.Instance.Business.MainSecurity.UserAccess.EmployeeID
    '            pSql = "UPDATE MainSecurityAccess" &
    '                   " SET WebKey = NULL" &
    '                   " WHERE WebKey = " & ReadyString(pWebKey)
    '            Common.WebRequest.ProData.ExecuteSQL(pSql)
    '        End If
    '        Return True
    '    Catch ex As System.Exception
    '        Stop2()
    '    End Try

    'End Function

    Public Property OrgID As String
        Get

            Dim pOut As String

            If mOrgID <> "" Then Return mOrgID

            pOut = Common.WebRequest.QueryString("OrgID")
            If IsGuid(pOut) Then
                mOrgID = ReadyGuid(pOut)
                Return mOrgID
            End If

            pOut = Data("OrgID")
            If IsGuid(pOut) Then
                mOrgID = ReadyGuid(pOut)
                Return mOrgID
            End If

            Return ""

        End Get
        Set(value As String)

            mOrgID = value

        End Set
    End Property

    Public Function OrgName() As String

        Static sOrganizationName As String
        Dim pSql As String

        If sOrganizationName Is Nothing Then
            pSql = "SELECT CompanyName" &
                " FROM MobizLibraryOrganization" &
                " WHERE ID = " & ReadyString(OrgID)
            sOrganizationName = ProData.ExecuteScalar(pSql)
        End If

        Return sOrganizationName

    End Function

    ''' <summary>
    ''' redirects to organization page if has no or invalid organization. Connects to private database if has valid organization.
    ''' Return true if has valid organization.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HandleOrganization(vAjax As Boolean, Optional ByRef oError As String = Nothing) As Boolean

        Dim pDR As DataRow

        Try
            If OrgID = "" Then
                If vAjax = False Then Transfer("/People/PeopleMenu")
                Return False
            End If

            pDR = Business.LibraryOrganization.ByID(OrgID)
            If pDR Is Nothing Then
                oError = "Organization,  " & OrgID & ", does not exist in LibraryOrganization. vb48PL3THKJJ!"
                If vAjax = False Then Transfer("/People/PeopleMenu")
                Return False
            End If
            If CStr2(pDR("Database")) = "" Then
                oError = "Organization,  " & OrgID & ", exist in LibraryOrganization but Database is null. vbDLY3J5PVBB!"
                If vAjax = False Then Transfer("/People/PeopleMenu")
                Return False
            End If
            mDatabase = pDR("Database")
            mFreeUser = pDR("FreeUser")
            ProData.ExecuteSQL("USE [" & mDatabase & "]")
            Return True
        Catch ex As System.Exception
            oError = "Organization, " & OrgID & ", causes an error. " & ex.Message & vbCrLf & GetStackTrace(, ex)
            'Stop2() ' commented 2022_09_01__09_37_48_3073
            AppendToDebug("HandleOrganization")
        End Try

    End Function

    Private Function HandleFreeUser(vDrOrg As DataRow, vPeopleID As String, ByRef oError As String) As Boolean



    End Function

    ''' <summary>
    ''' dont call before calling HandleOrganization
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HandleInvoiceCountByMonth(ByRef oError As String) As Boolean

        Dim pCount As Integer

        oError = ""
        If FreeUser = False Then
            Return True
        End If

        pCount = Business.TransInvoice.GetCountByMonth
        If pCount < TransInvoice.FreeUserMaxInvoiceCount Then
            Return True
        End If

        oError = "You have exceeded your monthly invoice count of " & TransInvoice.FreeUserMaxInvoiceCount & "."

        Return False

    End Function

    Public ReadOnly Property Database As String
        Get

            Return mDatabase

        End Get
    End Property

    Public Shared Function InvalidOrganization() As String

        Return JsonDictionary.Serialize("Error", "Invalid organization")

    End Function

    Public Function FowstID() As String

        '// code replaced with DeviceID
        'If (ReadyGuid(Data("FowstID")).Trim().Length > 0) Then

        '    Dim pFinder As Finder
        '    Dim pDT As DataTable

        '    pFinder = New Finder
        '    With pFinder
        '        .ProData = Common.WebRequest.ProData

        '        With .Fields
        '            With .Add("DeviceID", SqlDbType.UniqueIdentifier)
        '                .Expression = "devfowst.DeviceID"
        '            End With
        '        End With

        '        .SqlFrom = " FROM dbo.LibraryDeviceFowst devfowst " 

        '        .SqlWhere = " WHERE devfowst.FowstID = " + ReadyString(ReadyGuid(Data("FowstID")))

        '        pDT = .Requery

        '    End With

        '    'If (pDT.Rows.Count > 0) Then

        '    '    WebSession.Instance.SubscriberOrganizationDeviceID = pDT.Rows(0).Item("DeviceID").ToString()

        '    'End If
        'End If


        Return ReadyGuid(Data("FowstID"))

    End Function

    Public ReadOnly Property DeviceID As String
        Get
            Dim pSql As String
            Dim pDR As DataRow
            Dim pDeviceSerial As String

            Static sValue As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return ""

            If sValue IsNot Nothing Then
                Return sValue
            End If

            pDeviceSerial = Common.WebRequest.Data("DeviceSerial")
            If String.IsNullOrWhiteSpace(pDeviceSerial) Then
                pDeviceSerial = Common.WebRequest.Data("UUID")
            End If
            If CStr2(pDeviceSerial).Trim <> "" Then
                ' just do nothing here
            ElseIf WebSession.Instance.IsMobile Then
                pDeviceSerial = WebSession.Instance.DeviceSerial
            Else
                pDeviceSerial = ""
            End If

            pSql = "SELECT LibraryDevice.ID" &
                " FROM LibraryDevice" &
                " JOIN LibraryDeviceFowst ON LibraryDeviceFowst.DeviceID = LibraryDevice.ID" &
                " WHERE LibraryDevice.UUID = " & ReadyString(pDeviceSerial) & vbCrLf &
                " AND LibraryDevice.IsActive = 1"
            '" AND LibraryDeviceFowst.FowstID = " & ReadyString(FowstID)
            pDR = ProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                sValue = CStr2(pDR(0))

                Return sValue
            End If

            'Throw New Exception("This device (with serial number " & pDeviceSerial & ") is not assigned to any FOWST in this organization (with database " & ProData.GetCurrentDatabase & "). vb793SLWZ4YU!")

        End Get
    End Property

    Public ReadOnly Property FreeUser As Boolean
        Get

            Return mFreeUser

        End Get
    End Property

    Dim mStopWatch As Stopwatch
    Dim mStopWatchFile As String

    Public Sub StopWatch(vText As String)

        Dim pFilename As String
        Dim pText As String

        If mStopWatch Is Nothing Then
            mStopWatchFile = DateTimeStamp()
            mStopWatch = New Stopwatch()
            mStopWatch.Start()
            pText = "StopWatch started"
        Else
            pText = vText & " " & Format(mStopWatch.ElapsedMilliseconds, cGlobal.FormatInteger) & " ms"
            mStopWatch.Restart()
        End If

        pFilename = Common.ServerPath & "temp\logs\StopWatch\StopWatch-" & mStopWatchFile & ".txt"
        CreateFolder(pFilename)
        AppendToFile(pFilename, "[" & Format(Now, "MM/dd/yyyy hh:mm:ss tt") & "] " & pText & vbCrLf & vbCrLf)

    End Sub
End Class
