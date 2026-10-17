Namespace WebProj
    Public Class SCDAndPWDReportsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Inventory Form

        Function Index() As ActionResult
            Return View()
        End Function

        Function GetSCAndPWDDiscount() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim disctype As String = Common.WebRequest.Data("ShowDiscount")

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'SC, PWD, PHNATTEAM, SOLOPARENT Discounts
            pSql = " SELECT inv.ID as InvoiceID, inv.InvoiceDate, inv.InvoiceNumber, TotalAmount as GrossAmount, VAT, Vatable, ForegoneVAT, VATExempt, disc.DiscountType, invdisc.DiscountTypeID, invdisc.DiscountPercentage, org.PersonFullName as CustomerName, " & vbCrLf &
                    " ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '') + ISNULL(PersonOtherIDNo, '') as CustomerIDNo, " & vbCrLf &
                    " CASE WHEN ISNULL(SCDiscount, 0) > SCDiscount THEN SCDiscount " & vbCrLf &
                    "   WHEN ISNULL(PhNatTeamDiscount, 0) > 0 THEN PhNatTeamDiscount " & vbCrLf &
                    "   WHEN ISNULL(SoloParentDiscount, 0) > 0 THEN SoloParentDiscount " & vbCrLf &
                    "   ELSE payinv.DiscountAmount END as DiscountAmount, " & vbCrLf &
                    " SCDiscountExempt " & vbCrLf &
                    " FROM TransInvoice as inv " & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as invdisc ON invdisc.InvoiceID = inv.ID " & vbCrLf &
                    " INNER JOIN LibraryDiscountSettings as disc ON disc.ID = invdisc.DiscountTypeID " & vbCrLf &
                    " INNER JOIN TransCustomerPaymentInvoices as payinv ON payinv.InvoiceID = inv.ID " & vbCrLf &
                    " LEFT JOIN TransInvoiceSeniorCitizenPWDCustomer as cust ON cust.InvoiceID = inv.ID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as org ON org.ID = cust.CustomerID " & vbCrLf &
                    " where CONVERT(date,InvoiceDate) BETWEEN " & ReadyString(CStr2(Common.WebRequest.Data("From"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("To"))) & vbCrLf &
                    " AND inv.FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("FOWSTID"))) & vbCrLf &
                    " --AND disc.ID IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df', '21ebf55b-526c-4276-b3b7-0cbca504f425', 'e95f0b64-1eea-47b4-bde5-b795234b462d') " & vbCrLf

            If disctype <> "all" Then
                pSql += " AND disc.ID = " & ReadyString(disctype) & vbCrLf
            End If

            pSql += " ORDER BY inv.InvoiceDate DESC " & vbCrLf


            'OLD QUERY - commented by Arnie 08-29-25
            'pSql = "Select " & vbCrLf &
            '        "invoice.ID " & vbCrLf &
            '        ",InvoiceDate " & vbCrLf &
            '        ",InvoiceNumber " & vbCrLf &
            '        ",NoOfPeople " & vbCrLf &
            '        ",NoOfSeniorCitizens " & vbCrLf &
            '        ",(SELECT DISTINCT STUFF((" & vbCrLf &
            '        "Select ', ' +" & vbCrLf &
            '        "ISNULL(customer.PersonFullname, customer.PersonFirstName + ' ' + customer.PersonSurname )" & vbCrLf &
            '        "from TransInvoice invCustomers" & vbCrLf &
            '        "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = invCustomers.ID" & vbCrLf &
            '        "INNER JOIN MobizLibraryOrganization customer on customer.ID = TransInvoiceSeniorCitizenPWDCustomer.CustomerID" & vbCrLf &
            '        "WHERE invCustomers.ID = invoice.ID" & vbCrLf &
            '        "FOR XML PATH('')), 1, 1,'')) as CustomerNames" & vbCrLf &
            '        ",(SELECT DISTINCT STUFF((" & vbCrLf &
            '        "Select ', ' +" & vbCrLf &
            '        "ISNULL(PersonPWDIDNo,ISNULL(PersonPassportNo,ISNULL(PersonDriversLicenseNo,ISNULL(PersonSSSNo,ISNULL(PersonGSISIDNo,ISNULL(PersonPRCIDNo,ISNULL(PersonPostalIDNo,ISNULL(PersonVotersIDNo,ISNULL(PersonOtherIDType,ISNULL(PersonOtherIDNo,''))))))))))" & vbCrLf &
            '        "from TransInvoice invCustomers" & vbCrLf &
            '        "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = invCustomers.ID" & vbCrLf &
            '        "INNER JOIN MobizLibraryOrganization customer on customer.ID = TransInvoiceSeniorCitizenPWDCustomer.CustomerID" & vbCrLf &
            '        "WHERE invCustomers.ID = invoice.ID" & vbCrLf &
            '        "FOR XML PATH('')), 1, 1,'')) as CustomersID" & vbCrLf &
            '        ",SUM(SubTotal) GrossAmount " & vbCrLf &
            '        ",SUM(SubTotal) / NoOfPeople * NoOfSeniorCitizens GrossAmountSubjectDiscount" & vbCrLf &
            '        ",SCDiscount " & vbCrLf &
            '        "from TransInvoice invoice" & vbCrLf &
            '        "inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = invoice.ID" & vbCrLf &
            '        "INNER JOIN TransInvoiceDiscount on TransInvoiceDiscount.InvoiceID = invoice.ID" & vbCrLf &
            '        "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = invoice.ID" & vbCrLf &
            '        "where InvoiceNumber>0 and SCDiscount>0 " & vbCrLf &
            '        " AND convert(date,InvoiceDate) between " & ReadyString(CStr2(Common.WebRequest.Data("From"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("To"))) & vbCrLf &
            '        " AND invoice.FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("FOWSTID"))) & vbCrLf &
            '        "group by invoice.ID, InvoiceDate, NoOfPeople, NoOfSeniorCitizens, InvoiceNumber, SCDiscount"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            pDT.Columns.Add("DiscountCode")
            If pDT.Rows.Count > 0 Then
                For Each disc As Object In pDT.Rows
                    Dim code As String = ""

                    If disc("DiscountTypeID").ToString().ToLower() = "35f954a0-c67c-499c-af60-f08e64477fdc" Then
                        code = "VIPDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "8e2dca9c-433f-42fc-a644-04476dba056a" Then
                        code = "REGDISC3"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "c4199347-6a00-45c9-be11-8934dd63ecd1" Then
                        code = "REGDISC2"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "764edacb-0131-4033-8e44-7b903368008c" Then
                        code = "REGDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "7804a35a-b531-4380-945e-fc9643e99843" Then
                        code = "EMPDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "055a0a33-dd98-47ec-add3-8b2009f3944a" Then
                        code = "SENDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "34a4032f-a142-4c15-a868-4ac4410434df" Then
                        code = "PWDDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "674ad791-188b-4238-b95c-97b5c2b9d544" Then
                        code = "SPESENDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "c2218c86-0450-4cd3-b019-3edd43e51741" Then
                        code = "GPCDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "21ebf55b-526c-4276-b3b7-0cbca504f425" Then
                        code = "NAACDISC"
                    ElseIf disc("DiscountTypeID").ToString().ToLower() = "e95f0b64-1eea-47b4-bde5-b795234b462d" Then
                        code = "SOLOPARENTDISC"
                    Else
                        code = disc("DiscountType")
                    End If
                    disc("DiscountCode") = code
                Next
            End If

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        Function DownloadInventoryReport() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("p")
            Dim pSourceURL As String = Common.ServerPath & "temp\" & pFileTitle
            Dim pContentType As String = "application/csv"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

    End Class
End Namespace