Namespace WebProj
    Public Class PIMProductController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMProductInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.PIMProductID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMProductID", WebSession.Instance.PIMProductID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckUniqueProduct() As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProduct.ID = Common.WebRequest.Data("PIMProductID")
            clsPIMProduct.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsPIMProduct.ProductNo = Common.WebRequest.Data("ProductNo")
            clsPIMProduct.ProductName = Common.WebRequest.Data("ProductName")
            clsPIMProduct.ProductFamilyID = Common.WebRequest.Data("ProductFamilyID")
            clsPIMProduct.ProductUNSPSCCommID = Common.WebRequest.Data("UNSPSCCommodityID")

            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.CheckUniqueProduct())

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductList() As String
            Dim strKeyWord As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductFeatureList() As String
            Dim strID As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("UNSPSCCommID")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductFeatureList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductFeatureValueList() As String
            Dim strID As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMProductID")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductFeatureValueList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductInfo() As String
            Dim strID As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMProductID")
            strReturn = JsonDictionary.Serialize("PIMProductData", clsPIMProduct.getProductInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveProductInfo() As String
            Dim clsPIMProduct As New PIMProduct
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProduct.ID = Common.WebRequest.Data("PIMProductID")
            clsPIMProduct.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsPIMProduct.Brand = Common.WebRequest.Data("ProductNo")
            clsPIMProduct.ProductNo = Common.WebRequest.Data("ProductNo")
            clsPIMProduct.ProductName = Common.WebRequest.Data("ProductName")
            clsPIMProduct.ProductFamilyID = Common.WebRequest.Data("ProductFamilyID")
            clsPIMProduct.ProductUNSPSCCommIDOrig = Common.WebRequest.Data("UNSPSCCommodityIDOrig")
            clsPIMProduct.ProductUNSPSCCommID = Common.WebRequest.Data("UNSPSCCommodityID")
            clsPIMProduct.IsActive = Common.WebRequest.Data("IsActive")
            clsPIMProduct.ProductFeatureList = Common.WebRequest.AllData("ProductFeatureList")

            WebSession.Instance.PIMProductID = clsPIMProduct.SaveProductInfo()
            strReturn = JsonDictionary.Serialize("PIMProductData", WebSession.Instance.PIMProductID)

            Return strReturn

        End Function




    End Class

End Namespace