Imports Newtonsoft.Json

Namespace WebProj
    Public Class ReportsPOSController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function

                If (WebSession.Instance.POSSalesReportType = 0) Then
                    If (Not SecurityRole.Instance.HandleRoleAccess("Cashier Sales Report")) Then Exit Function
                Else
                    If (Not SecurityRole.Instance.HandleRoleAccess("Manager Sales Report")) Then Exit Function
                End If

            End If

            Return View()

        End Function

        Function SalesReport() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function

                If (WebSession.Instance.POSSalesReportType = 0) Then
                    If (Not SecurityRole.Instance.HandleRoleAccess("Cashier Sales Report")) Then Exit Function
                Else
                    If (Not SecurityRole.Instance.HandleRoleAccess("Manager Sales Report")) Then Exit Function
                End If

            End If

            Return View()

        End Function

        Function SalesAdjustmentReport() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Sales Adjustment Report")) Then Exit Function
            End If

            Return View()
        End Function

        Function ZReport() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Z Report")) Then Exit Function
            End If

            Return View()
        End Function

        Function EJournal() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                'If (Not SecurityRole.Instance.HandleRoleAccess("E Journal")) Then Exit Function
            End If

            Return View()
        End Function

        Function SKUUnitSalesReport() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function setPOSSalesReportType() As String
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ''' 0 - Cashier Sales Report
            ''' 1 - Manager Sales Report

            WebSession.Instance.POSSalesReportType = Convert.ToInt16(Common.WebRequest.Data("POSSalesReportType"))

            strReturn = JsonDictionary.Serialize("ReportPOS", IIf(WebSession.Instance.POSSalesReportType = 0, "CashierSalesReport", "ManagerSalesReport"))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPOSSalesReportType() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("POSSalesReportType", IIf(WebSession.Instance.POSSalesReportType = 0, "CashierSalesReport", "ManagerSalesReport"))

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function getReportHeader() As String
            Dim strFOWSTID As String
            Dim strUserID As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strFOWSTID = WebRequest.Instance.FowstID
            strUserID = WebSession.Instance().PeopleUserID

            Dim pDeviceID As String
            Dim pUuid = Common.WebRequest.Data("UUID")
            'Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.ByUuid(pUuid)
            'If pDrDevice IsNot Nothing Then
            '    pDeviceID = CStr2(pDrDevice("ID"))
            'Else
            pDeviceID = Common.WebRequest.Data("DeviceID")
            'End If
            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getReportHeader(Common.WebRequest.ProData, strFOWSTID, strUserID, pDeviceID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getCashierSalesTransactionSummary() As String
            Dim strCashierID As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim clsMegaworld As New MallLessorMegaworld
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")
            strCashierID = Common.WebRequest.Data("UserID") 'WebSession.Instance().PeopleUserID

            Dim pOut As New JsonDictionary
            Dim caashierData As DataTable = clsSalesReport.getCashierSalesTransactionSummary(strCashierID, strRangeType, strRangeFrom, strRangeTo, strRangeYear, Common.WebRequest.Data("FowstID"))
            pOut.Add("Data", caashierData)
            pOut.Add("ReportPOS", caashierData)
            pOut.Add("OtherPayments", clsMegaworld.getOtherPaymentTypes(Common.WebRequest.ProData, CDate(strRangeFrom), CDate(strRangeTo), Common.WebRequest.Data("FowstID"), Common.WebRequest.Data("DeviceID"), strCashierID))
            pOut.Add("Discounts", clsMegaworld.getDailyDiscountBreakdown(Common.WebRequest.ProData, CDate(strRangeFrom), CDate(strRangeTo), Common.WebRequest.Data("FowstID"), Common.WebRequest.Data("DeviceID"), strCashierID))

            Return pOut.Serialize()

        End Function

        <HttpPost()>
        Function getCashierTurnOverAndDeposits() As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String
            Dim strCashierID As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")
            strCashierID = Common.WebRequest.Data("UserID") 'WebSession.Instance().PeopleUserID

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getCashierTurnOverAndDeposits(strCashierID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn
        End Function


        ' ''<HttpPost()>
        ' ''Function getSalesReportPaymentInfo() As String
        ' ''    Dim strFOWSTID As String
        ' ''    Dim strCashierID As String
        ' ''    Dim strDeviceID As String
        ' ''    Dim strRangeType As String
        ' ''    Dim strRangeFrom As String
        ' ''    Dim strRangeTo As String
        ' ''    Dim strRangeYear As String
        ' ''    Dim clsSalesReport As New ReportPOS
        ' ''    Dim strReturn As String

        ' ''    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ' ''    If (Common.WebRequest.Data("RangeType") = "Day") Then
        ' ''        If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
        ' ''        ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
        ' ''        End If

        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
        ' ''        strRangeYear = ""
        ' ''    Else
        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Common.WebRequest.Data("RangeTo")
        ' ''        strRangeYear = Common.WebRequest.Data("RangeYear")
        ' ''    End If

        ' ''    strRangeType = Common.WebRequest.Data("RangeType")
        ' ''    strFOWSTID = WebRequest.Instance.FowstID
        ' ''    strCashierID = WebSession.Instance().PeopleUserID
        ' ''    strDeviceID = WebSession.Instance().SubscriberOrganizationDeviceID

        ' ''    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesReportPaymentInfo(strFOWSTID, strDeviceID, strCashierID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

        ' ''    Return strReturn

        ' ''End Function

        ' ''<HttpPost()>
        ' ''Function getSalesReportAdjustmentInfo() As String
        ' ''    Dim strFOWSTID As String
        ' ''    Dim strCashierID As String
        ' ''    Dim strDeviceID As String
        ' ''    Dim strRangeType As String
        ' ''    Dim strRangeFrom As String
        ' ''    Dim strRangeTo As String
        ' ''    Dim strRangeYear As String
        ' ''    Dim clsSalesReport As New ReportPOS
        ' ''    Dim strReturn As String

        ' ''    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ' ''    If (Common.WebRequest.Data("RangeType") = "Day") Then
        ' ''        If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
        ' ''        ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
        ' ''        End If

        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
        ' ''        strRangeYear = ""
        ' ''    Else
        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Common.WebRequest.Data("RangeTo")
        ' ''        strRangeYear = Common.WebRequest.Data("RangeYear")
        ' ''    End If

        ' ''    strRangeType = Common.WebRequest.Data("RangeType")
        ' ''    strFOWSTID = WebRequest.Instance.FowstID
        ' ''    strCashierID = WebSession.Instance().PeopleUserID
        ' ''    strDeviceID = WebSession.Instance().SubscriberOrganizationDeviceID

        ' ''    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesReportAdjustmentInfo(strFOWSTID, strDeviceID, strCashierID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

        ' ''    Return strReturn

        ' ''End Function

        ' ''<HttpPost()>
        ' ''Function getSalesReportVATInfo() As String
        ' ''    Dim strFOWSTID As String
        ' ''    Dim strCashierID As String
        ' ''    Dim strDeviceID As String
        ' ''    Dim strRangeType As String
        ' ''    Dim strRangeFrom As String
        ' ''    Dim strRangeTo As String
        ' ''    Dim strRangeYear As String
        ' ''    Dim clsSalesReport As New ReportPOS
        ' ''    Dim strReturn As String

        ' ''    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ' ''    If (Common.WebRequest.Data("RangeType") = "Day") Then
        ' ''        If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
        ' ''        ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
        ' ''        End If

        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
        ' ''        strRangeYear = ""
        ' ''    Else
        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Common.WebRequest.Data("RangeTo")
        ' ''        strRangeYear = Common.WebRequest.Data("RangeYear")
        ' ''    End If

        ' ''    strRangeType = Common.WebRequest.Data("RangeType")
        ' ''    strFOWSTID = WebRequest.Instance.FowstID
        ' ''    strCashierID = WebSession.Instance().PeopleUserID
        ' ''    strDeviceID = WebSession.Instance().SubscriberOrganizationDeviceID

        ' ''    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesReportVATInfo(strFOWSTID, strDeviceID, strCashierID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

        ' ''    Return strReturn

        ' ''End Function

        ' ''<HttpPost()>
        ' ''Function getManagerSalesReporPaymentInfo() As String
        ' ''    Dim strFOWSTID As String
        ' ''    Dim strRangeType As String
        ' ''    Dim strRangeFrom As String
        ' ''    Dim strRangeTo As String
        ' ''    Dim strRangeYear As String
        ' ''    Dim clsSalesReport As New ReportPOS
        ' ''    Dim strReturn As String

        ' ''    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ' ''    If (Common.WebRequest.Data("RangeType") = "Day") Then
        ' ''        If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
        ' ''        ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
        ' ''        End If

        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
        ' ''        strRangeYear = ""
        ' ''    Else
        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Common.WebRequest.Data("RangeTo")
        ' ''        strRangeYear = Common.WebRequest.Data("RangeYear")
        ' ''    End If

        ' ''    strRangeType = Common.WebRequest.Data("RangeType")
        ' ''    strFOWSTID = WebRequest.Instance.FowstID

        ' ''    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportPaymentInfo(strFOWSTID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

        ' ''    Return strReturn

        ' ''End Function

        ' ''<HttpPost()>
        ' ''Function getManagerSalesReportVATInfo() As String
        ' ''    Dim strFOWSTID As String
        ' ''    Dim strRangeType As String
        ' ''    Dim strRangeFrom As String
        ' ''    Dim strRangeTo As String
        ' ''    Dim strRangeYear As String
        ' ''    Dim clsSalesReport As New ReportPOS
        ' ''    Dim strReturn As String

        ' ''    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ' ''    If (Common.WebRequest.Data("RangeType") = "Day") Then
        ' ''        If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
        ' ''        ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
        ' ''        End If

        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
        ' ''        strRangeYear = ""
        ' ''    Else
        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Common.WebRequest.Data("RangeTo")
        ' ''        strRangeYear = Common.WebRequest.Data("RangeYear")
        ' ''    End If

        ' ''    strRangeType = Common.WebRequest.Data("RangeType")
        ' ''    strFOWSTID = WebRequest.Instance.FowstID

        ' ''    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportVATInfo(strFOWSTID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

        ' ''    Return strReturn

        ' ''End Function

        ' ''<HttpPost()>
        ' ''Function getManagerSalesReportAdjustmentInfo() As String
        ' ''    Dim strFOWSTID As String
        ' ''    Dim strRangeType As String
        ' ''    Dim strRangeFrom As String
        ' ''    Dim strRangeTo As String
        ' ''    Dim strRangeYear As String
        ' ''    Dim clsSalesReport As New ReportPOS
        ' ''    Dim strReturn As String

        ' ''    If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        ' ''    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ' ''    If (Common.WebRequest.Data("RangeType") = "Day") Then
        ' ''        If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
        ' ''        ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
        ' ''            Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
        ' ''        End If

        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
        ' ''        strRangeYear = ""
        ' ''    Else
        ' ''        strRangeFrom = Common.WebRequest.Data("RangeFrom")
        ' ''        strRangeTo = Common.WebRequest.Data("RangeTo")
        ' ''        strRangeYear = Common.WebRequest.Data("RangeYear")
        ' ''    End If

        ' ''    strRangeType = Common.WebRequest.Data("RangeType")
        ' ''    strFOWSTID = WebRequest.Instance.FowstID

        ' ''    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportAdjustmentInfo(strFOWSTID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

        ' ''    Return strReturn

        ' ''End Function

        <HttpPost()>
        Function getManagerSalesReportPaymentInfoByCashier() As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportPaymentInfoByCashier(strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function getManagerSalesReportPaymentInfoByMachine() As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If Now.Date = #9/28/2015# Then
                AppendToLog("vbAXE7GQSY6S! Common.WebRequest.InputStreamText = " & Common.WebRequest.InputStreamText, , "Manager Sales Report Error")
            End If

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportPaymentInfoByMachine(strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function getManagerSalesReportPaymentInfoByCashierMachine() As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportPaymentInfoByCashierMachine(strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function getManagerSalesTransactionSummary() As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Convert.ToDateTime(Common.WebRequest.Data("RangeFrom"))
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesTransactionSummary(strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function getManagerSalesReportOtherPaymentInfo() As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Convert.ToDateTime(Common.WebRequest.Data("RangeFrom"))
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getManagerSalesReportOtherPaymentInfo(strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn
        End Function

        <HttpPost()>
        Function getSalesZReportSummary() As String
            Dim strRangeType As String
            Dim strDate As String
            Dim FOWSTID As String
            Dim clsSalesReport As New ReportPOS
            Dim clsMallLessor As New MallLessorMegaworld
            Dim strReturn As New JsonDictionary

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strRangeType = Common.WebRequest.Data("RangeType")
            strDate = Common.WebRequest.Data("ZDate")
            If String.IsNullOrWhiteSpace(strDate) Then
                strDate = Now.Date
            End If

            If Common.WebRequest.Data("FOWSTID") Is Nothing Then
                FOWSTID = WebRequest.Instance.FowstID
            Else
                FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            End If

            'Dim mZread As New ZRead(Common.WebRequest.ProData)
            Dim pDeviceID = Common.WebRequest.Data("DeviceID")
            If IsGuid(pDeviceID) = False Then
                Dim pUuid = Common.WebRequest.Data("UUID")
                pDeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
            End If

            strReturn.Add("ReportPOS", clsSalesReport.getSalesZReportSummaryFromCache(strRangeType, strDate, FOWSTID, CStr2(pDeviceID)))
            strReturn.Add("OtherPaymentType", clsMallLessor.getOtherPaymentTypes(Common.WebRequest.ProData, strDate, strDate, FOWSTID, CStr2(pDeviceID)))
            strReturn.Add("Discounts", clsMallLessor.getDailyDiscountBreakdown(Common.WebRequest.ProData, strDate, strDate, FOWSTID, CStr2(pDeviceID)))

            'strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesZReportSummary(strRangeType, strDate, FOWSTID, CStr2(Common.WebRequest.Data("DeviceID"))))

            Return strReturn.Serialize()

        End Function

        <HttpPost()>
        Function getSalesAdjustmentReportInfo() As String
            Dim strFOWSTID As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")
            strFOWSTID = WebRequest.Instance.FowstID

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesAdjustmentReportInfo(strFOWSTID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function getSalesAdjustmentRefundReport() As String
            Dim strFOWSTID As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")
            strFOWSTID = WebRequest.Instance.FowstID

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesAdjustmentRefundReport(strFOWSTID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function getSalesAdjustmentSRVReport() As String
            Dim strFOWSTID As String
            Dim strRangeType As String
            Dim strRangeFrom As String
            Dim strRangeTo As String
            Dim strRangeYear As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Day") Then
                If (Not IsDate(Common.WebRequest.Data("RangeFrom"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeFromError")
                ElseIf (Not IsDate(Common.WebRequest.Data("RangeTo"))) Then
                    Return JsonDictionary.Serialize("ReportPOS", "RangeToError")
                End If

                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Convert.ToDateTime(Common.WebRequest.Data("RangeTo"))
                strRangeYear = ""
            Else
                strRangeFrom = Common.WebRequest.Data("RangeFrom")
                strRangeTo = Common.WebRequest.Data("RangeTo")
                strRangeYear = Common.WebRequest.Data("RangeYear")
            End If

            strRangeType = Common.WebRequest.Data("RangeType")
            strFOWSTID = WebRequest.Instance.FowstID

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesAdjustmentSRVReport(strFOWSTID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

            Return strReturn

        End Function

        <HttpPost()>
        Function UpdateZReportPrintCount() As String
            Dim pSql As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update LibraryDeviceFOWST set ZReadCount = ISNULL(ZReadCount,0) + 1 " & vbCrLf &
                    " where DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("FOWSTID")))

            Common.WebRequest.ProData.Execute(pSql)

            JsonDictionary.Serialize("ReportPOS", "True")

        End Function

        <HttpPost()>
        Function UpdateCSRPrintCount() As String
            Dim pSql As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update LibraryDeviceFOWST set XReadCount = ISNULL(XReadCount,0) + 1 " & vbCrLf &
                    " where DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("FOWSTID")))

            Common.WebRequest.ProData.Execute(pSql)

            JsonDictionary.Serialize("ReportPOS", "True")

        End Function

        <HttpPost>
        Function getEJournal() As String
            Dim dtZRead As DataTable
            Dim dtAuditTrail As DataTable
            Dim strRangeType As String
            Dim strDate As String
            Dim FOWSTID As String
            Dim clsSalesReport As New ReportPOS
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strRangeType = Common.WebRequest.Data("RangeType")
            strDate = Common.WebRequest.Data("ZDate")

            If Common.WebRequest.Data("FOWSTID") Is Nothing Then
                FOWSTID = WebRequest.Instance.FowstID
            Else
                FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            End If

            'dtZRead = clsSalesReport.getSalesZReportSummary(strRangeType, strDate, FOWSTID, CStr2(Common.WebRequest.Data("DeviceID")))

            Dim dtDateFrom As DateTime
            Dim dtDateto As DateTime

            'If (strRangeType = "Today") Then
            '    dtDateFrom = Date.Today
            '    dtDateto = Date.Today
            'ElseIf (strRangeType = "Yesterday") Then
            '    dtDateFrom = Date.Today.AddDays(-1)
            '    dtDateto = Date.Today.AddDays(-1)
            'ElseIf (strRangeType = "By Date") Then
            dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
            dtDateto = Convert.ToDateTime(Common.WebRequest.Data("DateTo"))
            'End If

            clsDashBoardsAndReports.DateFrom = dtDateFrom
            clsDashBoardsAndReports.DateTo = dtDateto

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.FOWSTID = FOWSTID
            clsDashBoardsAndReports.IsRawData = 0
            clsDashBoardsAndReports.GroupBy = "Day"

            dtAuditTrail = clsDashBoardsAndReports.GetRawData()

            'dtAuditTrail.DefaultView.RowFilter = " TransactionType = 'Invoice'"

            'dtAuditTrail = dtAuditTrail.DefaultView.ToTable

            'strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getEJournal(dtZRead, dtAuditTrail))

            Return strReturn

        End Function

        Function GetVATType() As String
            Dim pSql As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            pSql = "Select IsVAT from MobizLibraryOrganization WHERE ID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            pSql = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            If String.IsNullOrWhiteSpace(pSql) Then
                pSql = 1
            Else
                pSql = If(CBool2(pSql), 1, 0)
            End If
            Return JsonDictionary.Serialize("ReportPOS", pSql)

        End Function

        Function DownloadReportFile() As ActionResult
            Dim pFileTitle As String
            Dim pContentType As String
            Dim pSourceURL As String
            Dim pLoginHash As String
            Dim pDR As DataRow

            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function

            pFileTitle = Common.WebRequest.QueryString("ReportLink")
            pLoginHash = Common.WebRequest.QueryString("LoginHash")

            'If CStr2(pLoginHash).Length <> 50 Then
            '    If WebRequest.Instance.HandleAccess() = False Then Exit Function
            'Else
            '    pDR = MallLessorController.LoginData.GetByLoginHash(pLoginHash)
            '    If pDR Is Nothing Then
            '        Return Nothing
            '    End If
            '    If ReadyGuid(IO.Path.GetFileNameWithoutExtension(pFileTitle)) <> ReadyGuid(pDR("OrganizationID")) Then
            '        Return Nothing ' not same OrganizationID
            '    End If
            'End If

            pSourceURL = Common.ServerPath & "Content\EJournal\" & WebRequest.Instance().OrgID & "\" & pFileTitle
            pContentType = "text"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        'for java code
        <HttpPost()>
        Public Function GetZReadDataCache() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT As DataTable
            Dim pSql As String
            Dim pStr As String

            pSql = "Select top 10 * from TransZReadCache where FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & " Order by SalesDate desc"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("data", pDT)

        End Function

        Public Function CreateZReadPdf() As String

            Dim pUuid = Common.WebRequest.Data("UUID")
            pUuid = CStr2(pUuid).Replace(";", "_")
            If String.IsNullOrWhiteSpace(pUuid) Then
                Return JsonDictionary.Serialize("Error", "UUID is required.")
            End If
            If pUuid.Length < 1 Then
                Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
            End If

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsSalesReport As New ReportPOS
            Dim pDtHeader = clsSalesReport.getReportHeader(Common.WebRequest.ProData, Common.WebRequest.Data("FowstID"), WebSession.Instance.PeopleUserID)

            Dim pDtData = New DataTable
            Dim pValues = JsonConvert.DeserializeObject(Of List(Of KeyValue))(Common.WebRequest.AllData("Values"))
            For Each pValue In pValues
                If CStr2(pValue.DataType).Trim().ToLower() = "numeric" Then
                    pDtData.Columns.Add(pValue.Key, GetType(Decimal))
                ElseIf CStr2(pValue.DataType).Trim().ToLower() = "date" Then
                    pDtData.Columns.Add(pValue.Key, GetType(Date))
                Else
                    pDtData.Columns.Add(pValue.Key)
                End If
            Next

            Dim pDR = pDtData.NewRow
            For Each pValue In pValues
                If CStr2(pValue.DataType).Trim().ToLower() = "numeric" Then
                    pDR(pValue.Key) = CDec2(pValue.Value)
                ElseIf CStr2(pValue.DataType).Trim().ToLower() = "date" Then
                    pDR(pValue.Key) = ParseDate(pValue.Value)
                Else
                    pDR(pValue.Key) = pValue.Value
                End If
            Next
            pDtData.Rows.Add(pDR)

            Dim pReport = New ProReport
            pReport.SourceFilename = "ZRead"
            pReport.AddTable("Data", pDtData)
            pReport.AddTable("Header", pDtHeader)
            pReport.ExportXmlFile()

            Dim pFilename = Common.ServerPath & "temp\PrintablePdf\" & CreateValidFilename(pUuid) & ";" & "ZRead" & ";" & WebRequest.Instance.OrgID & ";" & DateTimeStamp() & ".pdf"
            CreateFolder(pFilename)

            ' BEGIN same code from ERPController
            pReport.Report.Pages(0).PaperSize = Printing.PaperKind.Custom
            Dim pMarginTop = pReport.Report.Pages(0).Margins.Top
            Dim pMarginBottom = pReport.Report.Pages(0).Margins.Bottom
            pReport.Report.Pages(0).Margins.Top = 0
            pReport.Report.Pages(0).Margins.Bottom = 0
            pReport.Report.Pages(0).Height = 1

            pReport.Render(True)

            Dim pClientHeight = pReport.Report.Pages(0).Height * pReport.Report.RenderedPages.Count + pMarginTop + pMarginBottom
            pReport.Report.Pages(0).Height = pClientHeight
            pReport.Report.Pages(0).Margins.Top = pMarginTop
            pReport.Report.Pages(0).Margins.Bottom = pMarginBottom

            pReport.Render(True)
            Do
                AppendToLog("vb6ECZJMSWWN! pages: " & pReport.Report.RenderedPages.Count & "; height=" & pReport.Report.RenderedPages.Item(0).Height & "; pFilename=" & pFilename)
                If pReport.Report.RenderedPages.Count = 1 Then
                    pReport.Report.Pages(0).Height -= 1
                    pReport.Render(True)
                Else
                    pReport.Report.Pages(0).Height += 1
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                    Exit Do
                End If
            Loop
            ' END same code from ERPController

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Function CreateCashierSalesReportPdf() As String

            Dim pUuid = Common.WebRequest.Data("UUID")
            pUuid = CStr2(pUuid).Replace(";", "_")
            If String.IsNullOrWhiteSpace(pUuid) Then
                Return JsonDictionary.Serialize("Error", "UUID is required.")
            End If
            If pUuid.Length < 1 Then
                Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
            End If

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsSalesReport As New ReportPOS
            Dim pDtHeader = clsSalesReport.getReportHeader(Common.WebRequest.ProData, Common.WebRequest.Data("FowstID"), WebSession.Instance.PeopleUserID)

            Dim pDtData = New DataTable
            Dim pInput = JsonConvert.DeserializeObject(Of ManagerSalesReportInput)(Common.WebRequest.InputStreamText)
            For Each pValue In pInput.Input.Values
                If CStr2(pValue.DataType).Trim().ToLower() = "numeric" Then
                    pDtData.Columns.Add(pValue.Key, GetType(Decimal))
                ElseIf CStr2(pValue.DataType).Trim().ToLower() = "date" Then
                    pDtData.Columns.Add(pValue.Key, GetType(Date))
                Else
                    pDtData.Columns.Add(pValue.Key)
                End If
            Next

            Dim pDR = pDtData.NewRow
            For Each pValue In pInput.Input.Values
                If CStr2(pValue.DataType).Trim().ToLower() = "numeric" Then
                    pDR(pValue.Key) = CDec2(pValue.Value)
                ElseIf CStr2(pValue.DataType).Trim().ToLower() = "date" Then
                    pDR(pValue.Key) = ParseDate(pValue.Value)
                Else
                    pDR(pValue.Key) = pValue.Value
                End If
            Next
            pDtData.Rows.Add(pDR)

            Dim pReport = New ProReport
            pReport.SourceFilename = "CashierSalesReport"
            pReport.AddTable("Data", pDtData)
            pReport.AddTable("Header", pDtHeader)
            pReport.ExportXmlFile()

            Dim pFilename = Common.ServerPath & "temp\PrintablePdf\" & CreateValidFilename(pUuid) & ";" & "CashierSalesReport" & ";" & WebRequest.Instance.OrgID & ";" & DateTimeStamp() & ".pdf"
            CreateFolder(pFilename)

            ' BEGIN same code from ERPController
            pReport.Report.Pages(0).PaperSize = Printing.PaperKind.Custom
            Dim pMarginTop = pReport.Report.Pages(0).Margins.Top
            Dim pMarginBottom = pReport.Report.Pages(0).Margins.Bottom
            pReport.Report.Pages(0).Margins.Top = 0
            pReport.Report.Pages(0).Margins.Bottom = 0
            pReport.Report.Pages(0).Height = 1

            pReport.Render(True)

            Dim pClientHeight = pReport.Report.Pages(0).Height * pReport.Report.RenderedPages.Count + pMarginTop + pMarginBottom
            pReport.Report.Pages(0).Height = pClientHeight
            pReport.Report.Pages(0).Margins.Top = pMarginTop
            pReport.Report.Pages(0).Margins.Bottom = pMarginBottom

            pReport.Render(True)
            Do
                AppendToLog("vb6ECZJMSWWN! pages: " & pReport.Report.RenderedPages.Count & "; height=" & pReport.Report.RenderedPages.Item(0).Height & "; pFilename=" & pFilename)
                If pReport.Report.RenderedPages.Count = 1 Then
                    pReport.Report.Pages(0).Height -= 1
                    pReport.Render(True)
                Else
                    pReport.Report.Pages(0).Height += 1
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                    Exit Do
                End If
            Loop
            ' END same code from ERPController

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Function CreateManagerSalesReportPdf() As String

            Dim pUuid = Common.WebRequest.Data("UUID")
            pUuid = CStr2(pUuid).Replace(";", "_")
            If String.IsNullOrWhiteSpace(pUuid) Then
                Return JsonDictionary.Serialize("Error", "UUID is required.")
            End If
            If pUuid.Length < 1 Then
                Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
            End If

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsSalesReport As New ReportPOS
            Dim pDtHeader = clsSalesReport.getReportHeader(Common.WebRequest.ProData, Common.WebRequest.Data("FowstID"), WebSession.Instance.PeopleUserID)

            Dim pDtData = New DataTable
            Dim pInput = JsonConvert.DeserializeObject(Of ManagerSalesReportInput)(Common.WebRequest.InputStreamText)
            For Each pValue In pInput.Input.Values
                If CStr2(pValue.DataType).Trim().ToLower() = "numeric" Then
                    pDtData.Columns.Add(pValue.Key, GetType(Decimal))
                ElseIf CStr2(pValue.DataType).Trim().ToLower() = "date" Then
                    pDtData.Columns.Add(pValue.Key, GetType(Date))
                Else
                    pDtData.Columns.Add(pValue.Key)
                End If
            Next

            Dim pDR = pDtData.NewRow
            For Each pValue In pInput.Input.Values
                If CStr2(pValue.DataType).Trim().ToLower() = "numeric" Then
                    pDR(pValue.Key) = CDec2(pValue.Value)
                ElseIf CStr2(pValue.DataType).Trim().ToLower() = "date" Then
                    pDR(pValue.Key) = ParseDate(pValue.Value)
                Else
                    pDR(pValue.Key) = pValue.Value
                End If
            Next
            pDtData.Rows.Add(pDR)

            ' BEGIN by cashier
            Dim pDtByCashier As New DataTable
            pDtByCashier.Columns.Add("Name")
            pDtByCashier.Columns.Add("TotalSales",GetType (Decimal))
            pDtByCashier.Columns.Add("TotalSalesPercent", GetType(Decimal))

            For Each pValue In pInput.Input.ByCashier
                Dim pDrNew = pDtByCashier.NewRow
                pDrNew("Name") = pValue.Key
                pDrNew("TotalSales") = CDec2(pValue.Value)
                pDrNew("TotalSalesPercent") = CDec2(pValue.Percent)
                pDtByCashier.Rows.Add(pDrNew)
            Next
            ' END by cashier

            ' BEGIN by machine
            Dim pDtByMachine As New DataTable
            pDtByMachine.Columns.Add("Name")
            pDtByMachine.Columns.Add("TotalSales", GetType(Decimal))
            pDtByMachine.Columns.Add("TotalSalesPercent", GetType(Decimal))

            For Each pValue In pInput.Input.ByMachine
                Dim pDrNew = pDtByMachine.NewRow
                pDrNew("Name") = pValue.Key
                pDrNew("TotalSales") = CDec2(pValue.Value)
                pDrNew("TotalSalesPercent") = CDec2(pValue.Percent)
                pDtByMachine.Rows.Add(pDrNew)
            Next
            ' END by machine

            ' BEGIN by cashier-machine
            Dim pDtByCashierMachine As New DataTable
            pDtByCashierMachine.Columns.Add("Name")
            pDtByCashierMachine.Columns.Add("TotalSales", GetType(Decimal))
            pDtByCashierMachine.Columns.Add("TotalSalesPercent", GetType(Decimal))

            For Each pValue In pInput.Input.ByCashierMachine
                Dim pDrNew = pDtByCashierMachine.NewRow
                pDrNew("Name") = pValue.Key
                pDrNew("TotalSales") = CDec2(pValue.Value)
                pDrNew("TotalSalesPercent") = CDec2(pValue.Percent)
                pDtByCashierMachine.Rows.Add(pDrNew)
            Next
            ' END by cashier-machine

            Dim pReport = New ProReport
            pReport.SourceFilename = "ManagerSalesReport"
            pReport.AddTable("Data", pDtData)
            pReport.AddTable("Header", pDtHeader)
            pReport.AddTable("ByCashier", pDtByCashier)
            pReport.AddTable("ByMachine", pDtByMachine)
            pReport.AddTable("ByCashierMachine", pDtByCashierMachine)
            pReport.ExportXmlFile()

            Dim pFilename = Common.ServerPath & "temp\PrintablePdf\" & CreateValidFilename(pUuid) & ";" & "ManagerSalesReport" & ";" & WebRequest.Instance.OrgID & ";" & DateTimeStamp() & ".pdf"
            CreateFolder(pFilename)

            ' BEGIN same code from ERPController
            pReport.Report.Pages(0).PaperSize = Printing.PaperKind.Custom
            Dim pMarginTop = pReport.Report.Pages(0).Margins.Top
            Dim pMarginBottom = pReport.Report.Pages(0).Margins.Bottom
            pReport.Report.Pages(0).Margins.Top = 0
            pReport.Report.Pages(0).Margins.Bottom = 0
            pReport.Report.Pages(0).Height = 1

            pReport.Render(True)

            Dim pClientHeight = pReport.Report.Pages(0).Height * pReport.Report.RenderedPages.Count + pMarginTop + pMarginBottom
            pReport.Report.Pages(0).Height = pClientHeight
            pReport.Report.Pages(0).Margins.Top = pMarginTop
            pReport.Report.Pages(0).Margins.Bottom = pMarginBottom

            pReport.Render(True)
            Do
                AppendToLog("vbPYG5TR6P6Z! pages: " & pReport.Report.RenderedPages.Count & "; height=" & pReport.Report.RenderedPages.Item(0).Height & "; pFilename=" & pFilename)
                If pReport.Report.RenderedPages.Count = 1 Then
                    pReport.Report.Pages(0).Height -= 1
                    pReport.Render(True)
                Else
                    pReport.Report.Pages(0).Height += 1
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                    Exit Do
                End If
            Loop
            ' END same code from ERPController

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Class ManagerSalesReportInput

            Public Input As InputClass

            Public Class InputClass
                Public Values As List(Of KeyValue)
                Public ByCashier As List(Of KeyValue)
                Public ByMachine As List(Of KeyValue)
                Public ByCashierMachine As List(Of KeyValue)
            End Class

        End Class

        Public Class KeyValue
            Public Key As String
            Public DataType As String
            Public Value As String

            Public Percent As String ' used only in ManagerSalesReportInput
        End Class
    End Class
End Namespace