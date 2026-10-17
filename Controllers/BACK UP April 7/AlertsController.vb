Public Class AlertsController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function NoPrice() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function NoPriceData() As String

        Dim pDT As DataTable
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pSql = "SELECT TOP 50 TransPIMAlerts.ProductID" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + " & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '') AS EmployeeName" & vbCrLf & _
            " ,TransPIMAlerts.LastUpdated" & vbCrLf & _
            " FROM TransPIMAlerts" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransPIMAlerts.ProductID" & vbCrLf & _
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransPIMAlerts.UserID" & vbCrLf & _
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf & _
            " ORDER BY TransPIMAlerts.LastUpdated" & vbCrLf
        pDT = WebRequest.Instance.ProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Warning", "No record")
        Else
            Return JsonDictionary.Serialize("data", pDT)
        End If

    End Function

    <HttpPost()>
    Public Function NoPriceDelete() As String

        Dim pIDs As String = ""
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        For Each pID As String In Common.WebRequest.AllData("ProductIDs")
            If pIDs.Length > 0 Then pIDs &= ", "
            pIDs &= ReadyString(pID)
        Next pID
        If pIDs.Length = 0 Then
            Return JsonDictionary.Serialize("Error", "Nothing to delete")
        End If

        pSql = "DELETE FROM TransPIMAlerts" &
            " WHERE ProductID IN (" & pIDs & ")"
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function
End Class
