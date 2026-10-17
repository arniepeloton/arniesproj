Public Class WarehouseReleasingTrackerController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    <HttpPost()>
    Public Function PostReleaseDocument() As String

        Dim pSql As String
        Dim pStr As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pStr = ReadyString(Common.WebRequest.Data("PeopleID"))
        If pStr.Length <= 2 Then
            pSql = "INSERT INTO TransDocumentTracker VALUES (NEWID(), " & ReadyString(Common.WebRequest.Data("docNum")) &
               ", NULL" &
               ", GetDate(), " & ReadyString(WebSession.Instance.PeopleUserID) &
               ", " & ReadyString(Common.WebRequest.Data("FowstID")) &
               ", " & ReadyString(Common.WebRequest.Data("IsManual")) &
               ", " & ReadyString(Common.WebRequest.Data("CustomerName")) & ")"
        Else
            pSql = "INSERT INTO TransDocumentTracker VALUES (NEWID(), " & ReadyString(Common.WebRequest.Data("docNum")) &
               ", " & ReadyString(Common.WebRequest.Data("PeopleID")) &
               ", GetDate(), " & ReadyString(WebSession.Instance.PeopleUserID) &
               ", " & ReadyString(Common.WebRequest.Data("FowstID")) &
               ", " & ReadyString(Common.WebRequest.Data("IsManual")) &
               ", " & ReadyString(Common.WebRequest.Data("CustomerName")) & ")"
        End If

        Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpPost()>
    Public Function CompareDocument() As String

        Dim pSql As String
        Dim pDT As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = "SELECT COUNT(ReleaseDoc) As DocCount " &
               " FROM TransDocumentTracker " &
               " WHERE ReleaseDoc = " & ReadyString(Common.WebRequest.Data("docNum"))
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Result", pDT.Rows(0).Item("DocCount"))

    End Function

    <HttpPost()>
    Public Function GetDetails() As String

        Dim pSql As String
        Dim pDT As DataTable
        Dim operationSwitch As Integer

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        operationSwitch = Common.WebRequest.Data("operationCase")

        Select Case operationSwitch
            Case 1
                pSql = "WITH PeopleLib AS " &
                       "(" &
                       "SELECT DISTINCT " &
                       "ISNULL(pub.ID, prv.ID) AS [ID], " &
                       "ISNULL(NULLIF(ISNULL(NULLIF(pub.PersonSurname + ',' + pub.PersonFirstName + ',' + pub.PersonMiddleName + ' ' + pub.PersonSuffixName, ',, '), pub.CompanyName), ''), prv.Surname + ',' + prv.FirstName + ',' + prv.MiddleName + ' ' + prv.PersonalSuffixName) AS [Name] " &
                       "FROM MobizLibraryPeople AS prv " &
                       "RIGHT JOIN LibraryOrganizationEmployee ON prv.ID = LibraryOrganizationEmployee.PeopleID " &
                       "FULL JOIN MobizLibraryOrganization AS pub ON prv.ID = pub.ID " &
                       ") " &
                       "SELECT DISTINCT TransDocumentTracker.ID, " &
                       "TransDocumentTracker.ReleaseDoc, " &
                       "ISNULL(NULLIF(ISNULL(NULLIF(pub.PersonSurname + ',' + pub.PersonFirstName + ',' + pub.PersonMiddleName + ' ' + pub.PersonSuffixName, ',, '), pub.CompanyName), ''), prv.Surname + ',' + prv.FirstName + ',' + prv.MiddleName + ' ' + prv.PersonalSuffixName) AS ReleaseTo, " &
                       "TransDocumentTracker.ReleaseDate, " &
                       "PeopleLib.Name AS ReleasedBy, " &
                       "TransDocumentTracker.CustomerName " &
                       "FROM TransDocumentTracker " &
                       "LEFT JOIN MobizLibraryPeople AS prv ON transDocumentTracker.ReleaseTo = prv.ID " &
                       "LEFT JOIN LibraryOrganizationEmployee ON prv.ID = LibraryOrganizationEmployee.PeopleID " &
                       "LEFT JOIN MobizLibraryOrganization AS pub ON prv.ID = pub.ID " &
                       "LEFT JOIN PeopleLib ON TransDocumentTracker.ReleasedBy = PeopleLib.ID " &
                       "WHERE ReleaseDoc = " & ReadyString(Common.WebRequest.Data("docNum")) &
                       " ORDER BY ReleaseDate ASC"
                pDT = Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Result", pDT)

            Case 2
                pSql = "WITH PeopleLib AS " &
                       "(" &
                       "SELECT DISTINCT " &
                       "ISNULL(pub.ID, prv.ID) AS [ID], " &
                       "ISNULL(NULLIF(ISNULL(NULLIF(pub.PersonSurname + ',' + pub.PersonFirstName + ',' + pub.PersonMiddleName + ' ' + pub.PersonSuffixName, ',, '), pub.CompanyName), ''), prv.Surname + ',' + prv.FirstName + ',' + prv.MiddleName + ' ' + prv.PersonalSuffixName) AS [Name] " &
                       "FROM MobizLibraryPeople AS prv " &
                       "RIGHT JOIN LibraryOrganizationEmployee ON prv.ID = LibraryOrganizationEmployee.PeopleID " &
                       "FULL JOIN MobizLibraryOrganization AS pub ON prv.ID = pub.ID " &
                       ") " &
                       "SELECT DISTINCT TOP 50 TransDocumentTracker.ID, " &
                       "TransDocumentTracker.ReleaseDoc, " &
                       "ISNULL(NULLIF(ISNULL(NULLIF(pub.PersonSurname + ',' + pub.PersonFirstName + ',' + pub.PersonMiddleName + ' ' + pub.PersonSuffixName, ',, '), pub.CompanyName), ''), prv.Surname + ',' + prv.FirstName + ',' + prv.MiddleName + ' ' + prv.PersonalSuffixName) AS ReleaseTo, " &
                       "TransDocumentTracker.ReleaseDate, " &
                       "PeopleLib.Name AS ReleasedBy, " &
                       "TransDocumentTracker.CustomerName " &
                       "FROM TransDocumentTracker " &
                       "LEFT JOIN MobizLibraryPeople AS prv ON transDocumentTracker.ReleaseTo = prv.ID " &
                       "LEFT JOIN LibraryOrganizationEmployee ON prv.ID = LibraryOrganizationEmployee.PeopleID " &
                       "LEFT JOIN MobizLibraryOrganization AS pub ON prv.ID = pub.ID " &
                       "LEFT JOIN PeopleLib ON TransDocumentTracker.ReleasedBy = PeopleLib.ID " &
                       " ORDER BY ReleaseDate DESC"
                pDT = Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Result", pDT)
            Case Else
                Return JsonDictionary.Serialize("Result", "Invalid Operation")
        End Select

    End Function

End Class
