Namespace WebProj
    Public Class ConfirmEmploymentController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function CheckIfAlreadyConfirm() As String
            Dim clsConfirm As New ConfirmEmployment
            Dim strReturn As String

            clsConfirm.OrgID = Common.WebRequest.Data("OrgID")
            clsConfirm.PeopleID = Common.WebRequest.Data("PeopleID")
            strReturn = JsonDictionary.Serialize("CheckingResult", clsConfirm.CheckIfAlreadyConfirm)

            Return strReturn
        End Function

        Function ConfirmEmployment() As String
            Dim strReturn As String
            Dim ConfirmKeyID As String = Common.WebRequest.Data("ConfirmKeyID")

            Dim p As New InsertUpdate("MobizLibraryPeopleOrganization")
            Dim pSql As String
            Dim strIDReturn As String
            Dim pOut As JsonDictionary
            pOut = New JsonDictionary

            Try

                p("IsConfirmed") = 1
                p("DateTimeConfirmed") = "getdate()"

                If (ConfirmKeyID.Trim().Length <= 0) Then
                    strIDReturn = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDReturn)
                    pSql = p.GetInsert()
                Else
                    strIDReturn = ConfirmKeyID
                    pSql = p.GetUpdate(" ID = " & ReadyString(ConfirmKeyID))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                Common.WebRequest.ProData.CommitTransaction()

                pOut.Add("Result", "Confirmed")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                pOut.Add("Result", "Error: " & ex.Message)
            End Try

            Return pOut.Serialize
        End Function
    End Class
End Namespace

