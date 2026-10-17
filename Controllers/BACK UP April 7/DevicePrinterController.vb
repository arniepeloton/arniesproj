Namespace WebProj
    Public Class DevicePrinterController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function DevicePrinterSummary() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function DevicePrinterInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function DevicePrinterDevice() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function PrinterSetup() As ActionResult

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
            WebSession.Instance.DevicePrinterID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("DevicePrinterID", WebSession.Instance.DevicePrinterID)

            Return pOut.Serialize

        End Function


        <HttpPost()>
        Function CheckUniquePrinterRole() As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.PrinterRole = Common.WebRequest.Data("PrinterRole")
            clsDevicePrinter.PrinterID = Common.WebRequest.Data("PrinterID")

            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.CheckUniquePrinterRole())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckUniquePrinterName() As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.PrinterName = Common.WebRequest.Data("PrinterName")
            clsDevicePrinter.ID = Common.WebRequest.Data("PrinterFowstID")
            clsDevicePrinter.FowstID = Common.WebRequest.Data("FowstID")

            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.CheckUniquePrinterName())

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterList() As String
            Dim strKeyWord As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.getDevicePrinterList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterListByFowstID() As String
            Dim strKeyWord As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.getDevicePrinterListByFowstID(Common.WebRequest.Data("FowstID")))

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterCategoryList() As String
            Dim strKeyWord As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.ID = Common.WebRequest.Data("PrinterID")
            strReturn = JsonDictionary.Serialize("Categories", clsDevicePrinter.getDevicePrinterCategoryList())

            Return strReturn

        End Function


        <HttpPost()>
        Function getDevicePrinterListAvailable() As String
            Dim strKeyWord As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.getDevicePrinterListAvailable())

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterInfo() As String
            Dim strID As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("DevicePrinterID")
            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.getDevicePrinterInfo(strID))

            Return strReturn

        End Function


        <HttpPost()>
        Function SaveFOWSTPrinterIP() As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.ID = Common.WebRequest.Data("PrinterFowstID")
            clsDevicePrinter.PrinterID = Common.WebRequest.Data("PrinterID")
            clsDevicePrinter.PrinterIP = Common.WebRequest.Data("PrinterIP")
            clsDevicePrinter.FowstID = Common.WebRequest.Data("FowstID")
            clsDevicePrinter.PrinterName = Common.WebRequest.Data("PrinterName")

            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.SaveFOWSTPrinterIP())

            Return strReturn

        End Function


        <HttpPost()>
        Function SaveDevicePrinterInfo() As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.ID = Common.WebRequest.Data("DevicePrinterID")
            clsDevicePrinter.PrinterRole = Common.WebRequest.Data("PrinterRole")
            clsDevicePrinter.PrinterType = Common.WebRequest.Data("PrinterType")
            clsDevicePrinter.IsActive = Common.WebRequest.Data("IsActive")

            WebSession.Instance.DevicePrinterID = clsDevicePrinter.SaveDevicePrinterInfo()
            strReturn = JsonDictionary.Serialize("DevicePrinterData", WebSession.Instance.DevicePrinterID)

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckUniquePrinterDevice() As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.ID = Common.WebRequest.Data("DevicePrinterDeviceRecID")
            clsDevicePrinter.DeviceID = Common.WebRequest.Data("DeviceID")

            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.CheckUniquePrinterDevice())

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterDeviceList() As String
            Dim strKeyWord As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.getDevicePrinterDeviceList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterDeviceInfo() As String
            Dim strID As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("DevicePrinterDeviceRecID")
            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevicePrinter.getDevicePrinterDeviceInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveDevicePrinterDeviceInfo() As String
            Dim clsDevicePrinter As New DevicePrinter
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.ID = Common.WebRequest.Data("DevicePrinterDeviceRecID")
            clsDevicePrinter.DeviceID = Common.WebRequest.Data("DeviceID")
            clsDevicePrinter.PrinterID = Common.WebRequest.Data("PrinterID")
            clsDevicePrinter.IsActive = Common.WebRequest.Data("IsActive")

            WebSession.Instance.DevicePrinterDeviceID = clsDevicePrinter.SaveDevicePrinterDeviceInfo()
            strReturn = JsonDictionary.Serialize("DevicePrinterData", WebSession.Instance.DevicePrinterDeviceID)

            Return strReturn

        End Function

        Function GetAllCategories() As String
            Dim clsDevicePrinter As New DevicePrinter
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDevicePrinter.GetAllCategories())
        End Function

        Function IsCategoryExist() As String
            Dim clsDevicePrinter As New DevicePrinter
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDevicePrinter.IsCategoryExist(Common.WebRequest.Data("PrinterID"), Common.WebRequest.Data("CategoryName")))
        End Function

        Function GetDevicePrinterCategories() As String
            Dim clsDevicePrinter As New DevicePrinter
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevicePrinter.PrinterID = Common.WebRequest.Data("PrinterID")

            Return JsonDictionary.Serialize("Results", clsDevicePrinter.GetDevicePrinterCategories())
        End Function

    End Class

End Namespace