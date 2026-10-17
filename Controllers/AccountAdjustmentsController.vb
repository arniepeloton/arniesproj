Public Class AccountAdjustmentsController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function

    Function GetAdjustments() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        Dim pSql As String
        Dim pDTAR As DataTable
        Dim pDTAP As DataTable
        Dim pOut As JsonDictionary

        pOut = New JsonDictionary

        pSql = "SELECT acc.*, ISNULL(ppol.FirstName + ' ', '') + ISNULL(ppol.Surname + ' ', '') FROM LibraryAccountAdjustments as acc " & vbCrLf &
                "INNER JOIN MobizLibraryPeople as ppol ON acc.UserID = ppol.ID WHERE AccountType = 'ar'"
        pDTAR = Common.WebRequest.ProData.Execute(pSql)

        pSql = "SELECT acc.*, ISNULL(ppol.FirstName + ' ', '') + ISNULL(ppol.Surname + ' ', '') FROM LibraryAccountAdjustments as acc " & vbCrLf &
                "INNER JOIN MobizLibraryPeople as ppol ON acc.UserID = ppol.ID WHERE AccountType = 'ap'"
        pDTAP = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("Ar", pDTAR)
        pOut.Add("Ap", pDTAP)

        Return pOut.Serialize()
    End Function

    Function SaveAdjustment() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim ID As String = Common.WebRequest.Data("ID")
        Dim Type As String = Common.WebRequest.Data("Type")
        Dim Effect As String = Common.WebRequest.Data("Effect")
        Dim Reason As String = Common.WebRequest.Data("Reason")
        Dim Status As String = Common.WebRequest.Data("Status")
        Dim AccountType As String = Common.WebRequest.Data("AccountType")
        Dim p As New InsertUpdate("LibraryAccountAdjustments")
        Dim pSql As String

        Try
            p("AdjustmentType") = ReadyString(Type)
            p("Effect") = ReadyString(Effect)
            p("Reason") = ReadyString(Reason)
            p("IsActive") = ReadyString(Status)
            p("AccountType") = ReadyString(AccountType)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "GetDate()"

            If ID = "" Then
                p("ID ") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return Json(New With {Key .Results = "saved"})
        Catch ex As Exception
            Return Json(New With {Key .Results = "ERROR: " & ex.Message()})
        End Try
    End Function

End Class
