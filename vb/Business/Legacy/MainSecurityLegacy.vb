Public Class MainSecurityLegacy
    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    'Dim mUserAccess As UserAccessClass
    'Public Property UserAccess As ErpAccess.UserAccess
    '    Get

    '        Return mUserAccess

    '    End Get
    '    Set(value As ErpAccess.UserAccess)

    '        mUserAccess = value

    '    End Set
    'End Property

    '    Public Function SetAccessByUsernameAndPassword(ByVal vUsername As String, ByVal vPassword As String) As Boolean

    '        Dim pUserAccess As ErpAccess.UserAccess

    '        pUserAccess = New UserAccessClass(mProData)
    '        If pUserAccess.SetAccessByUsernameAndPassword(vUsername, vPassword) Then
    '            mUserAccess = pUserAccess
    '#If _MYTYPE = "Web" Then
    '            WebSession.Instance.EmployeeID = mUserAccess.EmployeeID
    '#Else
    '            User.EmployeeID = mUserAccess.EmployeeID
    '#End If
    '            Return True
    '        End If

    '    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vAccessID">MainSecurityAccess.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '    Public Function SetAccessByAccessID(ByVal vAccessID As String) As Boolean

    '        Dim pUserAccess As UserAccessClass

    '        pUserAccess = New UserAccessClass(mProData)
    '        If pUserAccess.SetAccessByAccessID(vAccessID) Then
    '            mUserAccess = pUserAccess
    '#If _MYTYPE = "Web" Then
    '            WebSession.Instance.EmployeeID = mUserAccess.EmployeeID
    '#Else
    '            User.EmployeeID = mUserAccess.EmployeeID
    '            User.FullName = mUserAccess.FullName
    '            User.ID = mUserAccess.AccessID
    '#End If
    '            Return True
    '        End If

    '    End Function

    Public Function ByEmployeeID(ByVal vEmployeeID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MainSecurityAccess.ID" &
            " FROM MainSecurityAccess" &
            " WHERE MainSecurityAccess.EmployeeID = " & ReadyString(vEmployeeID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ByUsername(vUsername As String) As DataRow

        Dim pSql As String

        pSql = "SELECT MainSecurityAccess.EmployeeID" & vbCrLf & _
            " ,tblEmp.LastName + ', ' + tblEmp.FirstName + ' ' + tblEmp.MiddleName AS FullName" & vbCrLf & _
            " ,MainSecurityAccess.LevelID" & vbCrLf & _
            " ,MainSecurityLevel.LevelType" & vbCrLf & _
            " FROM MainSecurityAccess" & vbCrLf & _
            " LEFT JOIN MainSecurityLevel ON MainSecurityLevel.ID = MainSecurityAccess.LevelID" & vbCrLf & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmp ON tblEmp.ID = MainSecurityAccess.EmployeeID" & vbCrLf & _
            " WHERE LTRIM(RTRIM(MainSecurityAccess.Username)) = " & ReadyString(vUsername.Trim) & vbCrLf & _
            " AND MainSecurityAccess.IsCancelled = 0" & vbCrLf
        Return mProData.ExecuteRow(pSql)

    End Function

#Region " setup user "
    Public Function SetupUser(vEmployeeID As Integer, vSecurityLevelID As String, vEmail As String, vUserID As Integer) As String

        Dim pLink As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        pLink = GenerateString(30)
        pIU = New InsertUpdate("MainSecurityAccessPending")
        pIU("EmployeeID") = vEmployeeID
        pIU("SecurityLevelID") = ReadyString(vSecurityLevelID)
        pIU("Link") = ReadyString(pLink)
        pIU("Email") = ReadyString(vEmail)
        pIU("LastUpdated") = "getdate()"
        pIU("UserID") = vUserID
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        Return pLink

    End Function

    Public Function GetSetupUser(vEmployeeID As Integer, vSecurityLevelID As String) As String

        Dim pSql As String

        pSql = "SELECT TOP 1 Link" &
            " FROM MainSecurityAccessPending" &
            " WHERE EmployeeID = " & vEmployeeID &
            " AND SecurityLevelID = " & ReadyString(vSecurityLevelID)

        Return CStr2(mProData.ExecuteScalar(pSql))

    End Function

    Public Function GetByLink(vLink As String) As DataRow

        Dim pSql As String

        pSql = "SELECT MainSecurityAccessPending.EmployeeID" & vbCrLf & _
            " ,tblEmp.LastName + ', ' + tblEmp.FirstName + ' ' + tblEmp.MiddleName AS FullName" & vbCrLf & _
            " ,MainSecurityAccessPending.SecurityLevelID" & vbCrLf & _
            " ,MainSecurityLevel.LevelType" & vbCrLf & _
            " ,MainSecurityAccessPending.UserID" & vbCrLf & _
            " FROM MainSecurityAccessPending" & vbCrLf & _
            " JOIN MainSecurityLevel ON MainSecurityLevel.ID = MainSecurityAccessPending.SecurityLevelID" & vbCrLf & _
            " JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmp ON tblEmp.ID = MainSecurityAccessPending.EmployeeID" & vbCrLf & _
            " WHERE MainSecurityAccessPending.Link = " & ReadyString(vLink)

        Return mProData.ExecuteRow(pSql)

    End Function

    'Public Function Apply(vLink As String, vUsername As String, vPassword As String, ByRef oError As String) As Boolean

    '    Dim pIU As InsertUpdate
    '    Dim pDR As DataRow
    '    Dim pSql As String

    '    mProData.BeginTransaction()
    '    Try
    '        oError = ""
    '        pDR = ByUsername(vUsername)
    '        If pDR IsNot Nothing Then
    '            oError = "Username is already taken by " & pDR("FullName") & " (" & pDR("LevelType") & ")"
    '            Return False
    '        End If

    '        pDR = GetByLink(vLink)

    '        pIU = New InsertUpdate("MainSecurityAccess")
    '        pIU("ID") = "newid()"
    '        pIU("LevelID") = ReadyString(pDR("SecurityLevelID"))
    '        pIU("EmployeeID") = ReadyString(pDR("EmployeeID"))
    '        pIU("Username") = ReadyString(vUsername.Trim)
    '        pIU("Password") = ReadyString(ErpAccess.UserAccess.EncryptText(vPassword))
    '        pIU("UserID") = pDR("UserID")
    '        pSql = pIU.GetInsert
    '        mProData.ExecuteSQL(pSql)

    '        pSql = "DELETE FROM MainSecurityAccessPending" &
    '            " WHERE Link = " & ReadyString(vLink)
    '        mProData.ExecuteSQL(pSql)

    '        mProData.CommitTransaction()
    '        Return True
    '    Catch ex As Exception
    '        mProData.RollbackTransaction(, True)
    '        Throw
    '    End Try

    'End Function
#End Region

#If _MyType = "WindowsForms" _
    OrElse _MyType = "Windows" _
    OrElse _mytype = "WindowsFormsWithCustomSubMain" Then

    ''' <summary>
    ''' return Employees.ID if valid. 0 if invalid
    ''' </summary>
    Public Function CheckAccess(ByVal vAccess As ErpAccess.eAccess, ByVal vMode As ErpAccess.eAccessMode,
        Optional ByVal vNoPrompt As Boolean = False) As Integer

        Dim pFrm As frmUserPassword
        Dim pAT As ErpAccess.AccessText
        Dim pStr As String

        If mUserAccess Is Nothing Then
            If vNoPrompt = False Then
                ShowMessage("UserAccess was not set", , , MessageBoxIcon.Exclamation)
            End If
        Else
            If mUserAccess.GetAccessMode(vAccess, vMode) Then
                Return mUserAccess.EmployeeID
            ElseIf vMode <> ErpAccess.eAccessMode.IsPassword Then
                pAT = ErpAccess.UserAccess.GetAccessText(vAccess, vMode)
                pStr = "Transaction: " & pAT.Trans & vbCrLf &
                    "Subtransaction: " & pAT.SubTrans & vbCrLf &
                    "Mode: " & pAT.Mode
                pStr = "You need the following access rights to do this transaction: " & vbCrLf & pStr
                If mUserAccess.GetAccessMode(vAccess, ErpAccess.eAccessMode.IsPassword) Then
                    pFrm = New frmUserPassword
                    Return pFrm.ShowDialog(vAccess, vMode, pStr)
                ElseIf vNoPrompt = False Then
                    ShowMessage(pStr, "Unauthorized", , MessageBoxIcon.Exclamation)
                End If
            End If
        End If


    End Function
#End If

    'Public Class UserAccessClass
    '    Inherits ErpAccess.UserAccess

    '    ReadOnly mProData As ProData

    '    Public Sub New(ByVal vProData As ProData)

    '        mProData = vProData

    '    End Sub

    '    Public Overrides Function GetDataTable(ByVal vSql As String) As System.Data.DataTable

    '        Return mProData.Execute(vSql)

    '    End Function
    'End Class
End Class