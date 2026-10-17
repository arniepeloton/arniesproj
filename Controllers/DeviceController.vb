Namespace WebProj
    Public Class DeviceController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Devices")) Then Exit Function
            End If

            Return View()

        End Function

        Function DeviceInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function BsDeviceInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function DeviceFowst() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function BsDeviceFowst() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function DevicePrinter() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function DeviceInfoSummary() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function BsDeviceInfoSummary() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.DeviceID = CStr(Common.WebRequest.Data("ID"))

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function DeactivateDevice() As String
            Dim strReturn As String
            Dim clsDevice As New Device

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebSession.Instance.DeviceID = Common.WebRequest.Data("ID").ToString()

            strReturn = JsonDictionary.Serialize("device", clsDevice.DeactivateDevice())

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("DeviceID", WebSession.Instance.DeviceID)

            Return pOut.Serialize


        End Function

        <HttpPost()>
        Function CheckDeviceUniquePhoneName() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DeviceID")
            clsDevice.PhoneName = Common.WebRequest.Data("PhoneName")
            clsDevice.IMEI = Common.WebRequest.Data("IMEI")
            clsDevice.UUID = Common.WebRequest.Data("UUID")

            strReturn = JsonDictionary.Serialize("DeviceData", clsDevice.CheckDeviceUniquePhoneName())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckDeviceUnique() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DeviceID")
            clsDevice.PhoneName = Common.WebRequest.Data("PhoneName")
            clsDevice.IMEI = Common.WebRequest.Data("IMEI")
            clsDevice.UUID = Common.WebRequest.Data("UUID")

            strReturn = JsonDictionary.Serialize("DeviceData", clsDevice.CheckDeviceUnique())

            Return strReturn

        End Function

        <HttpPost()>
        Function RequestDeviceApproval() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'is request has been made
            'put request saving here

            If Not DeviceIsExist(Common.WebRequest.Data("UUID")) Then
                clsDevice.ID = Common.WebRequest.Data("DeviceID")
                clsDevice.PhoneName = Common.WebRequest.Data("PhoneName")
                clsDevice.PhoneModel = Common.WebRequest.Data("PhoneModel")
                clsDevice.IMEI = Common.WebRequest.Data("IMEI")
                clsDevice.UUID = Common.WebRequest.Data("UUID")
                clsDevice.OS = Common.WebRequest.Data("OS")
                clsDevice.IsActive = 0
                clsDevice.MobizAppVersion = "2"

                WebSession.Instance.DeviceID = clsDevice.SaveDeviceInfo()
            End If

            Return JsonDictionary.Serialize("Result", "Your request has been sent. Please contact your administrator to approve your request")

        End Function

        <HttpPost()>
        Function SaveDeviceInfoName() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DeviceID")
            clsDevice.PhoneName = Common.WebRequest.Data("PhoneName")
            clsDevice.PhoneModel = Common.WebRequest.Data("PhoneModel")
            clsDevice.IMEI = Common.WebRequest.Data("IMEI")
            clsDevice.UUID = Common.WebRequest.Data("UUID")
            clsDevice.UuidDisplay = Common.WebRequest.Data("UuidDisplay")
            clsDevice.TinDisplay = Common.WebRequest.Data("TinDisplay")
            clsDevice.OS = Common.WebRequest.Data("OS")
            clsDevice.Accreditation = Common.WebRequest.Data("Accreditation")
            clsDevice.MobizAppVersion = Common.WebRequest.Data("MobizAppVersion")
            clsDevice.IsActive = CInt(Common.WebRequest.Data("IsActive"))
            clsDevice.BusinessName = Common.WebRequest.Data("BusinessName")
            clsDevice.BusinessRegisteredName = Common.WebRequest.Data("BusinessRegName")

            WebSession.Instance.DeviceID = clsDevice.SaveDeviceInfo()
            strReturn = JsonDictionary.Serialize("DeviceData", WebSession.Instance.DeviceID)

            Return strReturn

        End Function

        <HttpPost()>
        Function ResetTransactions() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDeviceID = Common.WebRequest.Data("DeviceID")
            Dim pMessage As String
            Dim newdeviceid As String = ""

            If WebRequest.Instance.Business.LibraryDevice.ResetTransaction(pDeviceID, WebRequest.Instance.OrgID, pMessage, newdeviceid) Then
                Dim pout As New JsonDictionary

                pout.Add("Result", "ok")
                pout.Add("NewDeviceID", newdeviceid)
                Return pout.Serialize()
            Else
                Return JsonDictionary.Serialize("Error", pMessage)
            End If

        End Function

        <HttpPost()>
        Function ConfirmResetDevice() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim ResetPassword = Common.WebRequest.Data("ResetPassword")

            If ResetPassword.ToLower() = "super!secret2" Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Result", "invalid")
            End If

        End Function

        <HttpPost()>
        Function getDeviceInfoName() As String
            Dim strID As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = IIf(Common.WebRequest.Data("DeviceID") Is Nothing, "", Common.WebRequest.Data("DeviceID"))
            'WebSession.Instance.DeviceID
            WebSession.Instance.DeviceID = strID
            strReturn = JsonDictionary.Serialize("DeviceData", clsDevice.getDeviceInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckDeviceFowst() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DeviceFowstID")
            clsDevice.DeviceID = WebSession.Instance.DeviceID
            clsDevice.FowstID = Common.WebRequest.Data("FowstID")

            strReturn = JsonDictionary.Serialize("DeviceData", clsDevice.CheckDeviceFowst())

            Return strReturn

        End Function

        <HttpPost()>
        Function getDeviceFOWSTList() As String
            Dim strID As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = IIf(WebSession.Instance.DeviceID Is Nothing, "", WebSession.Instance.DeviceID)

            strReturn = JsonDictionary.Serialize("DeviceFowstData", clsDevice.getDeviceFOWSTList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getDeviceFOWSTInfo() As String
            Dim strID As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.DeviceID = Common.WebRequest.Data("DeviceID")
            clsDevice.FowstID = Common.WebRequest.Data("FowstID")
            strID = IIf(Common.WebRequest.Data("DeviceFowstID") Is Nothing, "", Common.WebRequest.Data("DeviceFowstID"))
            strReturn = JsonDictionary.Serialize("DeviceData", clsDevice.getDeviceFOWSTInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetPrinterNames() As String
            Dim strID As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.FowstID = Common.WebRequest.Data("FowstID")
            strReturn = JsonDictionary.Serialize("DeviceData", clsDevice.GetPrinterNames())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveDeviceFOWST() As String
            Dim clsDevice As New Device
            Dim strReturn As String
            Dim strID As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DeviceFowstID")
            clsDevice.PrinterFowstID = Common.WebRequest.Data("PrinterFowstID")
            clsDevice.DeviceID = WebSession.Instance.DeviceID
            clsDevice.FowstID = Common.WebRequest.Data("FowstID")
            clsDevice.POSRegistrationNo = Common.WebRequest.Data("POSRegistrationNo")
            clsDevice.POSPTUDateIssued = Common.WebRequest.Data("POSPTUDateIssued")
            clsDevice.BIRMachineNo = Common.WebRequest.Data("BIRMachineNo")
            clsDevice.DevFowstPrinterIP = Common.WebRequest.Data("DevFowstPrinterIP")
            clsDevice.SMMachineNo = Common.WebRequest.Data("POSSMMachineNo")
            clsDevice.BegBalDate = Common.WebRequest.Data("BegBalDate")
            clsDevice.BegBalance = Common.WebRequest.Data("BegBalance")
            clsDevice.POSLastORNo = Common.WebRequest.Data("POSLastORNo")
            clsDevice.POSLastSRVNo = Common.WebRequest.Data("POSLastSRVNo")
            clsDevice.POSLastRefNo = Common.WebRequest.Data("POSLastRefNo")
            'clsDevice.PrintServer = Common.WebRequest.Data("PrintServer")
            clsDevice.IsAuthorizedForPOS = CBool2(Common.WebRequest.Data("IsAuthorizedForPOS"))
            clsDevice.ReasonForNonRegistration = CInt(Common.WebRequest.Data("ReasonForNonRegistration"))
            clsDevice.ApplicationReferenceNo = Common.WebRequest.Data("ApplicationReferenceNo")
            clsDevice.ApplicationDate = Common.WebRequest.Data("ApplicationDate")
            clsDevice.ShowCompanyDetailsInHeader = CBool2(Common.WebRequest.Data("ShowCompanyDetailsInHeader"))

            If (clsDevice.BegBalDate.Trim().Length > 0) Then

                If (clsDevice.BegBalDate.Trim().Length > 0) Then
                    If (Not IsDate(clsDevice.BegBalDate.Trim())) Then
                        strReturn = JsonDictionary.Serialize("DeviceData", "Invalid BegBalDate")
                        Return strReturn
                    End If
                End If

                If (clsDevice.BegBalance.Trim().Length > 0) Then
                    If (Not IsNumeric(clsDevice.BegBalance.Trim())) Then
                        strReturn = JsonDictionary.Serialize("DeviceData", "Invalid BegBal")
                        Return strReturn
                    End If
                End If

                If (clsDevice.POSLastORNo.Trim().Length > 0) Then
                    If (Not IsNumeric(clsDevice.POSLastORNo.Trim())) Then
                        strReturn = JsonDictionary.Serialize("DeviceData", "Invalid LastORNo")
                        Return strReturn
                    End If
                End If

                If (clsDevice.POSLastSRVNo.Trim().Length > 0) Then
                    If (Not IsNumeric(clsDevice.POSLastSRVNo.Trim())) Then
                        strReturn = JsonDictionary.Serialize("DeviceData", "Invalid LastSRVNo")
                        Return strReturn
                    End If
                End If

                If (clsDevice.POSLastRefNo.Trim().Length > 0) Then
                    If (Not IsNumeric(clsDevice.POSLastRefNo.Trim())) Then
                        strReturn = JsonDictionary.Serialize("DeviceData", "Invalid LastRefNo")
                        Return strReturn
                    End If
                End If

            End If

            strID = clsDevice.SaveDeviceFOWST()
            strReturn = JsonDictionary.Serialize("DeviceData", strID)

            Return strReturn

        End Function

        Function DeviceIsExist(ByVal UUID) As Boolean
            Dim pSql As String
            Dim pDT As DataTable

            pSql = "Select * from LibraryDevice where UUID = " & ReadyString(UUID)

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        <HttpPost()>
        Function CheckUniqueDevicePrinter() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DevicePrinterRecID")
            clsDevice.PrinterID = Common.WebRequest.Data("PrinterID")

            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevice.CheckUniqueDevicePrinter())

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterList() As String
            Dim strKeyWord As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevice.getDevicePrinterList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getDevicePrinterInfo() As String
            Dim strID As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("DevicePrinterRecID")
            strReturn = JsonDictionary.Serialize("DevicePrinterData", clsDevice.getDevicePrinterInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveDevicePrinterInfo() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("DevicePrinterRecID")
            clsDevice.DeviceID = WebSession.Instance().DeviceID
            clsDevice.PrinterID = Common.WebRequest.Data("PrinterID")
            clsDevice.IsActive = Common.WebRequest.Data("IsActive")

            WebSession.Instance.DevicePrinterDeviceID = clsDevice.SaveDevicePrinterInfo()
            strReturn = JsonDictionary.Serialize("DevicePrinterData", WebSession.Instance.DevicePrinterDeviceID)

            Return strReturn

        End Function

        <HttpPost()>
        Function GetLibraryDevicePrinter() As String
            Dim strKeyWord As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Results", clsDevice.GetLibraryDevicePrinter())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetLibraryDeviceFowstPrinter() As String
            Dim strKeyWord As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Results", clsDevice.GetLibraryDeviceFowstPrinter())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetLibraryDevicePrinterFowst() As String
            Dim strKeyWord As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Results", clsDevice.GetLibraryDevicePrinterFowst())

            Return strReturn
        End Function

        <HttpPost()>
        Function RemoveDeviceFowst() As String
            Dim clsDevice As New Device
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("Results", clsDevice.RemoveDeviceFowst())

            Return strReturn

        End Function

        <HttpPost()>
        Function ZeroOutDeviceFowstTransactions() As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDeviceFowstID = Common.WebRequest.Data("DeviceFowstID")
            Dim pError As String

            WebRequest.Instance.Business.LibraryDeviceFowst.ZeroOutDeviceFowstTransactions(pDeviceFowstID, WebSession.Instance.PeopleUserID, pError)
            If pError IsNot Nothing Then
                Return JsonDictionary.Serialize("error", pError)
            End If

            Return JsonDictionary.Serialize("Results", "ok")
        End Function
        <HttpPost()>
        Function SaveMallDeviceFOWST() As String
            Dim clsDevice As New Device
            Dim strReturn As String
        End Function


    End Class
End Namespace