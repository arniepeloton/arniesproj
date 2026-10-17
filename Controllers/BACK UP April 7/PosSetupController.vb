Public Class PosSetupController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        
#If DEBUG Then
        If WebRequest.Instance.HandleAccess = False Then Exit Function
        Return View()
#Else
        Return Content("Not yet implemented in the production server")
#End If

    End Function

    <HttpPost()>
    Public Function Addresses() As String

        Dim pDT As DataTable
        Dim pSql As String

#If DEBUG Then
#Else
        Return JsonDictionary.Serialize("Error", "Not yet implemented in the production server")
#End If

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse WebSession.Instance.UserAccess.LevelType.ToLower <> "administrator" Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.Database.ToLower <> Client.MobizDUC.ToLower Then
            Return JsonDictionary.Serialize("Error", "This function is not valid for this subscriber")
        End If

        pSql = "SELECT LibraryBranch.ID AS BranchID" & vbCrLf &
            ",LibraryBranch.Name AS BranchName" & vbCrLf & _
            " ,LibraryBranch.Address" & vbCrLf & _
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf & _
            " ,MobizLibraryOrganizationFOWST.Description AS FowstDescription" & vbCrLf & _
            " ,dbo.SplitJoinComma(ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '')" & vbCrLf & _
            " 	+ ', ' + ISNULL(MobizLibraryOrganizationFOWST.Street, '')" & vbCrLf & _
            " 	+ ', ' + ISNULL(MobizLibraryCities.Name, '') + ', ' + ISNULL(MobizLibraryCountries.Name, '')) AS FowstAddress" & vbCrLf & _
            " FROM DavaoUnifiedPayroll.dbo.LibraryBranch LibraryBranch" & vbCrLf & _
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryBranch.FowstID" & vbCrLf & _
            " LEFT JOIN MobizLibraryCities ON MobizLibraryOrganizationFOWST.FowstCityID = MobizLibraryCities.ID" & vbCrLf & _
            " LEFT JOIN MobizLibraryCountries ON MobizLibraryOrganizationFOWST.FowstCountryID = MobizLibraryCountries.ID" & vbCrLf & _
            " ORDER BY LibraryBranch.Name" & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("data", pDT)

    End Function

    <HttpPost()>
    Public Function UpdateBranch() As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse WebSession.Instance.UserAccess.LevelType.ToLower <> "administrator" Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.Database.ToLower <> Client.MobizDUC.ToLower Then
            Return JsonDictionary.Serialize("Error", "This function is not valid for this subscriber")
        End If

        Dim pFowstID As String
        Dim pBranchID As String ' integer
        Dim pSql As String
        'Dim pPosOrCounter As Integer

        pFowstID = Common.WebRequest.Data("FowstID")
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid pFowstID")
        End If
        pFowstID = ReadyGuid(pFowstID)

        pBranchID = Common.WebRequest.Data("BranchID")
        If Integer.TryParse(pBranchID, 0) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid BranchID")
        End If
        pBranchID = CInt(pBranchID)

        'If Integer.TryParse(Common.WebRequest.Data("PosOrCounter"), pPosOrCounter) = False Then
        '    Return JsonDictionary.Serialize("Error", "Invalid PosOrCounter")
        'End If
        'If pPosOrCounter < 0 Then
        '    Return JsonDictionary.Serialize("Error", "Invalid PosOrCounter")
        'End If

        Common.WebRequest.ProData.BeginTransaction()
        Try
            pSql = "UPDATE DavaoUnifiedPayroll.dbo.LibraryBranch" & vbCrLf & _
                " SET FowstID = NULL" & vbCrLf & _
                " WHERE FowstID = " & ReadyString(pFowstID) & vbCrLf
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "UPDATE DavaoUnifiedPayroll.dbo.LibraryBranch" & vbCrLf & _
                " SET FowstID = " & ReadyString(pFowstID) & vbCrLf & _
                " WHERE ID = " & pBranchID & vbCrLf
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'pSql = "UPDATE LibraryCustomersAddressesPOSSettings" & vbCrLf &
            '    " SET POSORCounter = " & ReadyString(pPosOrCounter) & vbCrLf &
            '    " WHERE CustomerAddressID = " & ReadyString(pCustomerAddressID) & vbCrLf
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction(, True)
            Throw
        End Try

    End Function
End Class
