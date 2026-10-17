Namespace WebProj
    Public Class FowstBinController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /FowstBin

        Function Index() As ActionResult
            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function
            Return View()
        End Function

        Function GetOrganizationID()
            Return JsonDictionary.Serialize("OrganizationID", WebSession.Instance.SubscriberOrganizationID)
        End Function

        <HttpPost()>
        Function SaveBinStock()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryFowstBinProduct")
            Dim pSql As String

            p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
            p("BinID") = ReadyString(WebRequest.Instance.Data("BinID"))
            p("ProductID") = ReadyString(WebSession.Instance.PIMProductID)

            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function SaveBin()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryFowstBin")
            Dim pSql As String

            If CInt(WebRequest.Instance.Data("IsCustomerBin")) Then
                p = New InsertUpdate("LibraryOrganizationCustomerFowstBin")
            End If

            p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
            p("Name") = ReadyString(WebRequest.Instance.Data("Name"))
            p("Description") = ReadyString(WebRequest.Instance.Data("Description"))

            If CBool(WebRequest.Instance.Data("IsEdit")) Then
                pSql = p.GetUpdate("ID = " & ReadyString(WebRequest.Instance.Data("ID")))
            Else
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function SaveCustomerBin()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryOrganizationCustomerFowstBin")
            Dim pSql As String

            p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
            p("Name") = ReadyString(WebRequest.Instance.Data("Name"))
            p("Description") = ReadyString(WebRequest.Instance.Data("Description"))

            If CBool(WebRequest.Instance.Data("IsEdit")) Then
                pSql = p.GetUpdate("ID = " & ReadyString(WebRequest.Instance.Data("ID")))
            Else
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        Function BinInfo() As ActionResult
            Return View()
        End Function

        Function AssignBinToStock() As ActionResult
            Return View()
        End Function
    End Class
End Namespace