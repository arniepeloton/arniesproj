Namespace WebProj
    Public Class FranchisorController
        Inherits System.Web.Mvc.Controller

        ' 
        ' GET: /Franchisor

        Function Index() As ActionResult
            Return View()
        End Function

        Function GetAllFranchisorGroup() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", FranchisorCentral.GetAllFranchisorGroup(Common.WebRequest.Data("SearchKeyword")))
        End Function

        Function GetFranchisorGroupInfo() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            FranchisorCentral.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", FranchisorCentral.GetFranchisorGroupInfo)

        End Function

        Function GetGroupFranchiseeFOWST() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            FranchisorCentral.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", FranchisorCentral.GetGroupFranchiseeFOWST)

        End Function

        Function GetGroupSchedule() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            FranchisorCentral.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", FranchisorCentral.GetGroupSchedule)
        End Function

        Function GetGroupHeads() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            FranchisorCentral.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", FranchisorCentral.GetGroupHeads)
        End Function


        Function SaveGroup() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            FranchisorCentral.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", FranchisorCentral.SaveGroup)
        End Function

        Function DisableSchedule() As Boolean
            Dim pSql As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update LibraryFranchiseGroupCutOffDetails set IsActive = 0 where ID = " & ReadyString(Common.WebRequest.Data("ID"))

            Common.WebRequest.ProData.Execute(pSql)
        End Function


        Function CheckIsGroupHead() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim pDT As DataTable


            pSql = "SELECT ID FROM MobizLibraryOrganizationFranchisee WHERE " &
                " FranchisorOrgID = " & ReadyString(WebRequest.Instance().OrgID) & " AND IsActive = 1 "

            pDT = Common.WebRequest.ProData.Execute(pSql)
            If (pDT.Rows.Count > 0) Then
                pSql = "SELECT LibraryFranchiseGroupHeads.ID FROM LibraryFranchiseGroup INNER JOIN LibraryFranchiseGroupHeads ON " &
               " LibraryFranchiseGroup.ID =LibraryFranchiseGroupHeads.FranchiseGroupID WHERE " &
               " LibraryFranchiseGroupHeads.PeopleID= " & ReadyString(WebSession.Instance.PeopleUserID)

                pDT = Common.WebRequest.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    Return JsonDictionary.Serialize("Franchisor", "True")
                Else

                    Return JsonDictionary.Serialize("Franchisor", "False")

                End If

            Else

                Return JsonDictionary.Serialize("Franchisee", "True")
            End If
           

        End Function

        Function RemoveFranchiseeFromGroup() As String
            Dim pSql As String

            pSql = "Update MobizLibraryOrganizationFranchisee set FranchiseGroupID = null " & vbCrLf &
                        " WHERE FranchiseeOrgFowstID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        End Function

        Function CheckIfExistingFranchisee() As String
            Dim FranchisorCentral As New FranchisorCentral

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim pDT As DataTable

            Dim pSql2 As String
            Dim pDT2 As DataTable

            pSql = "SELECT ID FROM MobizLibraryOrganizationFranchisee WHERE " &
                " FranchisorOrgID = " & ReadyString(WebRequest.Instance().OrgID) & " AND IsActive = 1 AND " &
                " IsFranchisorConfirmed = 1 AND IsFranchiseeConfirmed = 1 AND FranchiseeOrgFowstID = " & ReadyString(Common.WebRequest.Data("FowstID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            pSql2 = "SELECT ID FROM MobizLibraryOrganizationFOWST WHERE " &
               " OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) & " AND ID = " & ReadyString(Common.WebRequest.Data("FowstID"))

            pDT2 = Common.WebRequest.ProData.Execute(pSql2)


            If (pDT.Rows.Count) Then
                Return JsonDictionary.Serialize("Result", "True")

            Else
                If (pDT2.Rows.Count > 0) Then
                    Return JsonDictionary.Serialize("Result", "Own")
                Else
                    Return JsonDictionary.Serialize("Result", "False")
                End If


            End If


        End Function



    End Class
End Namespace