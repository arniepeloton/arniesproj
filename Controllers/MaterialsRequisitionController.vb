Namespace WebProj
    Public Class MaterialsRequisitionController
        Inherits System.Web.Mvc.Controller

        Dim clsMaterialsRequisition As New MaterialsRequisition

        Function Index() As ActionResult
            Return View()
        End Function

        Function GeneratePDF() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function GetProjectBudgetByProject() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetProjectBudgetByProject())
        End Function

        <HttpPost()>
        Function GetTotalMaterialsRequisitionFromServer() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            clsBOMProjectExcel.ParentID = Common.WebRequest.Data("ParentID")
            strReturn = JsonDictionary.Serialize("Results", clsBOMProjectExcel.GetTotalMaterialsRequisitionFromServer())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetMaterialsVendor() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.OrganizationID = Common.WebRequest.Data("OrgID")
            clsMaterialsRequisition.ProjectID = Common.WebRequest.Data("ProjectID")
            clsMaterialsRequisition.ParentID = Common.WebRequest.Data("ParentID")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetMaterialsVendor())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetRequestMaterials() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("RequisitionID")
            clsMaterialsRequisition.ProjectID = Common.WebRequest.Data("ProjectID")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetRequestMaterials())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetRequestMaterialsHeader() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.ProjectID = Common.WebRequest.Data("ProjectID")
            clsMaterialsRequisition.IsSearch = Common.WebRequest.Data("IsSearch")
            clsMaterialsRequisition.SearchText = Common.WebRequest.Data("SearchText")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetRequestMaterialsHeader())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetRequestDetails() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("ReqID")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetRequestDetails())

            Return strReturn
        End Function

        <HttpPost()>
        Function SaveApprovedMaterialsRequisition() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("ReqID")
            clsMaterialsRequisition.IsApprove = Common.WebRequest.Data("IsApprove")
            clsMaterialsRequisition.ApprovedMatReq = Common.WebRequest.AllData("ApprovedMatReq")
            clsMaterialsRequisition.ApprovedBy = Common.WebRequest.AllData("ApprovedBy")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.SaveApprovedMaterialsRequisition())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetApprovedMaterials() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("reqID")
            clsMaterialsRequisition.ProjectID = Common.WebRequest.Data("projID")
            strReturn = clsMaterialsRequisition.GetApprovedMaterials()

            Return strReturn
        End Function

        <HttpPost()>
        Function GetProjectDetails() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("reqID")
            clsMaterialsRequisition.ProjectID = Common.WebRequest.Data("projID")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetProjectDetails())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetOrgDetails() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.OrganizationID = Common.WebRequest.Data("OrganizationID")
            strReturn = clsMaterialsRequisition.GetOrgDetails()

            Return strReturn
        End Function

        <HttpPost()>
        Function GetEachVendor() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("reqID")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetEachVendor())

            Return strReturn
        End Function

        <HttpPost()>
        Function SavePORequiredDetails() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.ObjVendor = Common.WebRequest.AllData("Vendor")
            clsMaterialsRequisition.RequisitionID = Common.WebRequest.Data("ReqID")
            clsMaterialsRequisition.CreatedBy = Common.WebRequest.Data("CreatedBy")
            clsMaterialsRequisition.InvoiceType = Common.WebRequest.Data("InvoiceType")
            clsMaterialsRequisition.DeliveryDate = Common.WebRequest.Data("DeliveryDate")
            clsMaterialsRequisition.BillingAddress = Common.WebRequest.Data("BillingAddress")
            clsMaterialsRequisition.BillingAddressID = Common.WebRequest.Data("BillingAddressID")
            clsMaterialsRequisition.DeliveryAddress = Common.WebRequest.Data("DeliveryAddress")
            clsMaterialsRequisition.DeliveryAddressID = Common.WebRequest.Data("DeliveryAddressID")
            clsMaterialsRequisition.Remarks = Common.WebRequest.Data("Remarks")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.SavePORequiredDetails())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetItemPerVendor() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.ObjVendor = Common.WebRequest.AllData("arrVendor")
            'strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetItemPerVendor())

            Return clsMaterialsRequisition.GetItemPerVendor()
            'Return strReturn
        End Function

        <HttpPost()>
        Function GetVendorItem() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.VendorID = Common.WebRequest.Data("VendorID")

            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetItemsNow())

            Return strReturn
        End Function

        <HttpPost()>
        Function SaveMaterialsRequisition() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.ObjRequestedMaterials = Common.WebRequest.AllData("reqArr")
            clsMaterialsRequisition.TotalRequestedMaterials = Common.WebRequest.Data("reqVal")
            clsMaterialsRequisition.RequestedMaterialsNo = Common.WebRequest.Data("reqNo")
            clsMaterialsRequisition.ProjectID = Common.WebRequest.Data("ProjectID")
            clsMaterialsRequisition.RequestedBy = Common.WebRequest.Data("RequestedBy")
            clsMaterialsRequisition.OrganizationID = Common.WebRequest.Data("OrgID")

            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.SaveMaterialsRequisition())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetProductRequestLogs() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsMaterialsRequisition As New MaterialsRequisition
            Dim strReturn As String

            clsMaterialsRequisition.ProductID = Common.WebRequest.Data("ProductID")
            strReturn = JsonDictionary.Serialize("Results", clsMaterialsRequisition.GetProductRequestLogs())

            Return strReturn
        End Function
    End Class
End Namespace