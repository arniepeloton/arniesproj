Namespace WebProj
    Public Class PurchaseOrderController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function Trans() As ActionResult
            Return View()
        End Function

        Function Preview() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function GetPOData() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPO As New PurchaseOrder

            Return JsonDictionary.Serialize("Results", clsPO.GetPOData())
        End Function

        <HttpPost()>
        Function SavePOData() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsPO As New PurchaseOrder

            clsPO.PurchaseOrderID = Common.WebRequest.Data("ID")
            clsPO.IsRestrict = Common.WebRequest.Data("IsRestrict")

            Return JsonDictionary.Serialize("Results", clsPO.SavePOData())
        End Function

        <HttpPost()>
        Function SaveSettings() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.Business.SystemDefaults.ExpirationDaysForPOLink = Request.Form.Get("expirationdaysforpolink")
            WebRequest.Instance.Business.SystemDefaults.POLinkDomainName = Request.Form.Get("polinkdomain")
            WebRequest.Instance.Business.SystemDefaults.POLinkDomainInternalIPAddress = Request.Form.Get("polinkinternalipaddress")
            WebRequest.Instance.Business.SystemDefaults.POLinkDomainExternalIPAddress = Request.Form.Get("polinkexternalipaddress")

            Return JsonDictionary.Serialize("Results", "saved")
        End Function

        <HttpPost()>
        Function SavePurchasingLimits() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim PLID As String = Request.Form.Get("PLID")
                Dim employeeID As String = Request.Form.Get("EmployeeID")
                Dim perPO As Decimal = Request.Form.Get("txtPurchasingLimitsPerPO")
                Dim perMonth As Decimal = Request.Form.Get("txtPurchasingLimitsPerMonth")
                Dim updatedBy As String = Request.Form.Get("UpdatedBy")
                Dim pSql As String
                Dim p As New InsertUpdate("LibraryPurchasingLimits")

                p("EmployeeID") = ReadyString(employeeID)
                p("LimitsPerPO") = perPO
                p("LimitsPerMonth") = perMonth
                p("LastUpdated") = "getdate()"
                p("UpdatedBy") = ReadyString(updatedBy)

                If String.IsNullOrWhiteSpace(PLID) Then
                    pSql = "SELECT * FROM LibraryPurchasingLimits WHERE EmployeeID = " & ReadyString(employeeID)

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                        Return JsonDictionary.Serialize("Exist", True)
                    End If

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(PLID))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        <HttpPost()>
        Function DeletePurchasingLimit() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Common.WebRequest.ProData.Execute("DELETE FROM LibraryPurchasingLimits WHERE ID = " & ReadyString(Common.WebRequest.Data("PLID")))

            Return JsonDictionary.Serialize("Results", "removed")
        End Function

        <HttpPost>
        Public Function GetSettings() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary
            Dim pSql As String

            pOut.Add("ExpirationDaysForPOLink", WebRequest.Instance.Business.SystemDefaults.ExpirationDaysForPOLink)
            pOut.Add("POLinkDomainName", WebRequest.Instance.Business.SystemDefaults.POLinkDomainName)
            pOut.Add("POLinkDomainInternalIPAddress", WebRequest.Instance.Business.SystemDefaults.POLinkDomainInternalIPAddress)
            pOut.Add("POLinkDomainExternalIPAddress", WebRequest.Instance.Business.SystemDefaults.POLinkDomainExternalIPAddress)

            pSql = " SELECT limits.*, emp.Surname + ', ' + emp.FirstName as EmployeeName, updatedby.FirstName + ' ' + updatedby.Surname as UpdatedByName " & vbCrLf &
                    " FROM LibraryPurchasingLimits as limits " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as updatedby ON updatedby.ID = limits.UpdatedBy " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as emp ON emp.ID = limits.EmployeeID " & vbCrLf &
                    " ORDER BY emp.Surname ASC "

            pOut.Add("PurchasingLimits", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()

        End Function

    End Class
End Namespace

