Namespace WebProj
    Public Class DeliveryInfoController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function EmailPriceQuote() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function getCurrentSubcriberOrgID() As String
            Dim strReturn As String
            Dim pOut As JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("DeliveryInfoData", WebRequest.Instance.OrgID)
            Return strReturn

        End Function

        <HttpPost()>
        Function getDeliveryInfo() As String
            Dim strSalesOrderID As String
            Dim strInvoiceID As String
            Dim clsDeliveryInfo As New DeliveryInfo
            Dim strReturn As String
            Dim dtaTable As New DataTable
            Dim pFowstID As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFowstID = Common.WebRequest.Data("FowstID")
            If IsGuid(pFowstID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
            End If
            pFowstID = ReadyGuid(pFowstID)

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then
                Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            strInvoiceID = Common.WebRequest.Data("InvoiceID")

            If (Not strInvoiceID Is Nothing) Then
                If (strInvoiceID.Trim().Length > 0) Then
                    dtaTable = clsDeliveryInfo.getInvoiceInfo(strInvoiceID, pFowstID)
                End If
            End If

            If (dtaTable.Rows.Count > 0) Then
                WebSession.Instance.IsNew = Convert.ToInt16(False)
                WebSession.Instance.OrganizationID = dtaTable.Rows(0).Item("CustomerID").ToString()
            End If

            strReturn = JsonDictionary.Serialize("DeliveryInfoData", dtaTable)

            Return strReturn

        End Function

        <HttpPost()>
        Function getOutOfTownBusDefault() As String
            Dim clsDeliveryInfo As New DeliveryInfo
            Dim strReturn As String

            If WebRequest.Instance.OrgID Is Nothing Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strReturn = JsonDictionary.Serialize("OutOfTownBusDefault", clsDeliveryInfo.getOutOfTownBusDefault())

            Return strReturn

        End Function

        <HttpPost()>
        Function getPickUpCustomerIsWaitingDefault() As String
            Dim clsDeliveryInfo As New DeliveryInfo
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("PickUpCustomerIsWaitingDefault", clsDeliveryInfo.getPickUpCustomerIsWaitingDefault())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveInvoiceDeliveryInfo() As String
            Dim clsDeliveryInfo As New DeliveryInfo
            Dim strReturn As String
            Dim strEmail As String()
            Dim intCntr As Integer

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("EmailAddress").Trim().Length > 0) Then
                strEmail = Common.WebRequest.Data("EmailAddress").Trim().Split(",")
                intCntr = 0
                While (intCntr < strEmail.Length)

                    If (Not IsEmail(strEmail(intCntr).Trim())) Then
                        Return JsonDictionary.Serialize("DeliveryInfoData", "InvalidEmail")
                    End If

                    intCntr = intCntr + 1
                End While
            End If

            clsDeliveryInfo.ID = Common.WebRequest.Data("ID")
            clsDeliveryInfo.CusRepresentativeID = Common.WebRequest.Data("CusRepresentativeID")
            clsDeliveryInfo.CustomerAddressID = Common.WebRequest.Data("BillToID")
            clsDeliveryInfo.DeliverToID = Common.WebRequest.Data("ShipToID")

            clsDeliveryInfo.PickUpLocationID = Common.WebRequest.Data("PickUpLocationID")
            clsDeliveryInfo.DeliveryMethod = Common.WebRequest.Data("DeliveryMethod")
            clsDeliveryInfo.IsDownTown = Common.WebRequest.Data("IsDownTown")
            clsDeliveryInfo.IsRushDelivery = Common.WebRequest.Data("IsRushDelivery")
            clsDeliveryInfo.DeliverWithin = If(Common.WebRequest.Data("DeliverWithin").ToString().Trim().Length > 0, Common.WebRequest.Data("DeliverWithin"), 0)
            clsDeliveryInfo.DeliveryType = Common.WebRequest.Data("DeliveryType")
            clsDeliveryInfo.PickupDate = Common.WebRequest.Data("PickupDate")

            If (clsDeliveryInfo.DeliverWithin.Trim().Length > 0 And Not IsNumeric(clsDeliveryInfo.DeliverWithin)) Then
                strReturn = JsonDictionary.Serialize("DeliveryInfoData", "MinutesOrHoursError")
                Return strReturn
            ElseIf (clsDeliveryInfo.PickupDate.Trim().Length > 0 And Not IsDate(clsDeliveryInfo.PickupDate)) Then
                strReturn = JsonDictionary.Serialize("DeliveryInfoData", "PickUpDateAndTimeError")
                Return strReturn
            End If

            strReturn = JsonDictionary.Serialize("DeliveryInfoData", clsDeliveryInfo.SaveInvoiceDeliveryInfo())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveInvoiceToQuotation() As String
            Dim clsDeliveryInfo As New DeliveryInfo
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDeliveryInfo.ID = Common.WebRequest.Data("ID")
            clsDeliveryInfo.CustomerID = Common.WebRequest.Data("CustomerID")

            strReturn = JsonDictionary.Serialize("DeliveryInfoData", clsDeliveryInfo.SaveInvoiceToQuotation())

            Return strReturn

        End Function

        <HttpPost()>
        Function SendEmailQuotation() As String
            Dim clsSalesQuotation As New DeliveryInfo
            Dim strReturn As String
            Dim dtaTable As DataTable
            Dim intCntr As Integer
            Dim strTemp As String
            Dim strQuoteNo As String = ""
            Dim strEmail As String()

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strBody As String = ""
            Dim strmReader As System.IO.StreamReader = New System.IO.StreamReader(Server.MapPath("~/Views/DeliveryInfo/EmailPriceQuote.vbhtml"))
            strBody = strmReader.ReadToEnd

            '''Get Header Info
            dtaTable = New DataTable()
            dtaTable = clsSalesQuotation.getHeaderInfo(WebRequest.Instance.FowstID)
            If (dtaTable.Rows.Count > 0) Then

                strBody = strBody.Replace("[TradeNameHere]", dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim())
                strBody = strBody.Replace("[CorporateNameHere]", dtaTable.Rows(0).Item("CorporateName").ToString().Trim())
                strBody = strBody.Replace("[TINHere]", "TIN " & dtaTable.Rows(0).Item("TIN").ToString().Trim())
                strBody = strBody.Replace("[AddressHere]", dtaTable.Rows(0).Item("CompleteAddress").ToString().Trim())
                strBody = strBody.Replace("[TelNoHere]", "Tel. No. " & dtaTable.Rows(0).Item("TelNo").ToString().Trim())

            End If

            '''Get Payment Info
            dtaTable = New DataTable()
            dtaTable = clsSalesQuotation.getQuotationHeaderInfo(Common.WebRequest.Data("QuotationID"))
            If (dtaTable.Rows.Count > 0) Then

                strBody = strBody.Replace("[CustomerNameHere]", "Customer : " & dtaTable.Rows(0).Item("CustomerName").ToString().Trim())
                strBody = strBody.Replace("[CustomerAddressHere]", "Address : " & dtaTable.Rows(0).Item("CustomerAddress").ToString().Trim())
                strBody = strBody.Replace("[CustomerTINHere]", "TIN : " & dtaTable.Rows(0).Item("CustomerTIN").ToString().Trim())

                strBody = strBody.Replace("[CustomerRepresentative]", "Representative : " & dtaTable.Rows(0).Item("Representative").ToString().Trim())
                strBody = strBody.Replace("[CustomerBillingAddress]", "Billing Address : " & dtaTable.Rows(0).Item("BillingAddress").ToString().Trim())
                strBody = strBody.Replace("[CustomerDeliveryAddress]", "Delivery Address : " & dtaTable.Rows(0).Item("DeliveryAddress").ToString().Trim())

                strQuoteNo = Convert.ToInt32(dtaTable.Rows(0).Item("QuotationNo")).ToString("000000000000").Trim()

                strBody = strBody.Replace("[QuotationNoHere]", "Quotation No. " & Convert.ToInt32(dtaTable.Rows(0).Item("QuotationNo")).ToString("000000000000").Trim())
                strBody = strBody.Replace("[QuotationDateTimeHere]", "Date/Time : " & Convert.ToDateTime(dtaTable.Rows(0).Item("QuotationDate")).ToString("MMM dd, yyyy HH:mm").Trim())
                strBody = strBody.Replace("[CashierNameHere]", "Quoted By : " & dtaTable.Rows(0).Item("CashierName").ToString().Trim())

            End If

            '''Get Payment Items
            dtaTable = New DataTable()
            dtaTable = clsSalesQuotation.getQuotationBodyInfo(Common.WebRequest.Data("QuotationID"))
            If (dtaTable.Rows.Count > 0) Then

                strTemp = "<tbody>"
                intCntr = 0
                While (intCntr < dtaTable.Rows.Count)

                    strTemp = strTemp & "<tr>"
                    strTemp = strTemp & "<td style=""text-align:left; width: 35%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("ProductName").ToString().Trim() & "</td>"
                    strTemp = strTemp & "<td style=""text-align:right; width: 10%; height:30px; border: 1px dotted Gray;"">" & Convert.ToInt32(dtaTable.Rows(intCntr).Item("Quantity")).ToString("#,##0").Trim() & "</td>"
                    strTemp = strTemp & "<td style=""text-align:right; width: 15%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("UnitMeasure").ToString().Trim() & "</td>"
                    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("UnitPrice")).ToString("#,##0.00").Trim() & "</td>"
                    strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
                    strTemp = strTemp & "</tr>"

                    intCntr = intCntr + 1

                End While

                strTemp = strTemp & "</tbody>"

                strBody = strBody.Replace("[BodyGoesHere]", strTemp)

            End If

            '''Get Total Due
            dtaTable = New DataTable()
            dtaTable = clsSalesQuotation.getDeliveryInfoFooterAmountsInfo(Common.WebRequest.Data("QuotationID"))
            If (dtaTable.Rows.Count > 0) Then

                strTemp = "<table style=""width:100%"">"
                strTemp = strTemp & "<thead>"
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:30px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:30px;"">Total Amount Due</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TotalAmountDue")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "</thead>"
                strTemp = strTemp & "</table>"

                strBody = strBody.Replace("[FooterGoesHere]", strTemp)

            End If

            SendMail("customercare@mobiz-apps.com",
                     Common.WebRequest.Data("EmailAddress").Trim(),
                     "Mobiz",
                     "Mobiz - Quotation No. " & strQuoteNo,
                     strBody,
                     "smtp.live.com",
                     587,
                     "customercare@mobiz-apps.com",
                     "m0b!zp30pl30nly!", , , , , True)

            strReturn = JsonDictionary.Serialize("SalesPaymentData", "Sent")

            Return strReturn

        End Function





    End Class
End Namespace