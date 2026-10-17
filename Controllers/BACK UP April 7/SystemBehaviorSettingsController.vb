Namespace WebProj
    Public Class SystemBehaviorSettingsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /SystemBehaviorSettings

        Function Index() As ActionResult
            Return View()
        End Function

        Function SalesOrder() As ActionResult
            Return View()
        End Function

        Function SaveSalesOrderBehavior() As String
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pIU = New InsertUpdate("SystemDefaults")
            Try
                Common.WebRequest.ProData.BeginTransaction()



                With WebRequest.Instance.Business.SystemDefaults
                    .IsEnableInventoryCountMode = Common.WebRequest.Data("IsEnableInventoryCountMode")
                    .RequireSignature = Common.WebRequest.Data("RequireSignature")
                End With

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizSystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()


                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try
        End Function

        Function GetSalesOrderBehavior() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pOut As JsonDictionary
            pOut = New JsonDictionary
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableInventoryCountMode", .IsEnableInventoryCountMode)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("RequireSignature", .RequireSignature)
            End With

            Return pOut.Serialize


        End Function
    End Class
End Namespace