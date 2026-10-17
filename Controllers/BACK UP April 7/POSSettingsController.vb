Public Class POSSettingsController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /POSSettings

    Function Index() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function GetSystemBehavior() As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pOut = New JsonDictionary
        Try
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableDiningMode", .IsEnableDiningMode)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableBillingOption", .IsEnableBillingOption)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("WhenDoYouWantToPrintOrderSlip", .WhenDoYouWantToPrintOrderSlip)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsPrintOrderSlip", .IsPrintOrderSlip)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsShowTableNo", .IsShowTableNo)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsShowGasPumpNo", .IsShowGasPumpNo)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("NoOfTables", .NoOfTables)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("NoOfGasPumps", .NoOfGasPumps)
            End With
            With WebRequest.Instance.Business.SystemDefaults
                pOut.Add("IsEnableVATExemptOption", .IsEnableVATExemptOption)
            End With
        Catch ex As Exception

            pOut.Add("Error", ex.Message)


        End Try

        Return pOut.Serialize
    End Function

    <HttpPost()>
    Public Function SaveSystemBehavior() As String

        Dim pIU As InsertUpdate
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pIU = New InsertUpdate("SystemDefaults")
        Try
            Common.WebRequest.ProData.BeginTransaction()

            With WebRequest.Instance.Business.SystemDefaults
                .IsEnableDiningMode = Common.WebRequest.Data("IsEnableDiningMode")
                .IsEnableBillingOption = Common.WebRequest.Data("IsEnableBillingOption")
                .WhenDoYouWantToPrintOrderSlip = Common.WebRequest.Data("WhenDoYouWantToPrintOrderSlip")
                .IsPrintOrderSlip = Common.WebRequest.Data("IsPrintOrderSlip")
                .IsShowTableNo = Common.WebRequest.Data("IsShowTableNo")
                .IsShowGasPumpNo = Common.WebRequest.Data("IsShowGasPumpNo")
                .IsEnableVATExemptOption = Common.WebRequest.Data("IsEnableVATExemptOption")
            End With

            'save only if 
            If (Common.WebRequest.Data("IsShowTableNo") = "1") Then
                With WebRequest.Instance.Business.SystemDefaults
                    .NoOfTables = Common.WebRequest.Data("NoOfTables")
                End With
            End If

            'save only if 
            If (Common.WebRequest.Data("IsShowGasPumpNo") = "1") Then
                With WebRequest.Instance.Business.SystemDefaults
                    .NoOfGasPumps = Common.WebRequest.Data("NoOfGasPumps")
                End With
            End If

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizSystemDefaults'"

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()
            Return JsonDictionary.Serialize("Result", "Success")

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return JsonDictionary.Serialize("Error", ex.Message)

        End Try

    End Function
End Class
