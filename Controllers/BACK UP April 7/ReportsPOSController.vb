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

        <HttpPost()>
        Function setPOSSalesReportType() As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
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

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strFOWSTID = WebRequest.Instance.FowstID
            strUserID = WebSession.Instance().PeopleUserID

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getReportHeader(strFOWSTID, strUserID))

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
            strCashierID = WebSession.Instance().PeopleUserID

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getCashierSalesTransactionSummary(strCashierID, strRangeType, strRangeFrom, strRangeTo, strRangeYear))

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
        Function getSalesZReportSummary() As String
            Dim strRangeType As String
            Dim strDate As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strRangeType = Common.WebRequest.Data("RangeType")
            strDate = Common.WebRequest.Data("ZDate")

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getSalesZReportSummary(strRangeType, strDate))

            Return strReturn

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

    End Class
End Namespace