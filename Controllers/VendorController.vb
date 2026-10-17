Public Class VendorController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /Vendor

    Function Index() As ActionResult
        Return View()
    End Function

    Function Edit() As ActionResult
        Return View()
    End Function

    Function GetAllScanVendorInvoices()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
            Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
            Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
            Dim whereSearchContent As String = ""
            Dim operators As String = ""
            Dim arr As Object = Split(keyword, "+")
            Dim showtickets As String = Request.Params.Get("ShowTickets")

            Dim mainquery As String = "SELECT ROW_NUMBER() OVER (ORDER BY org.CompanyName ASC) AS RowNum, files.*, org.CompanyName as VendorName " & vbCrLf &
                    " FROM LibraryCRMAttachments as files " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganization as org ON org.ID = files.ClientID " & vbCrLf &
                    " INNER JOIN LibraryOrganizationSupplier as sup ON sup.SupplierID = org.ID AND ISNULL(TaggedAs, '') = '' " & vbCrLf &
                    " WHERE InvoiceRefNo IS NOT NULL "

            If CStr2(keyword) <> "" Then
                mainquery += " AND ("
                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " AND "
                    End If

                    whereSearchContent = whereSearchContent & operators & vbCrLf &
                         " ISNULL(InvoiceRefNo, '') + ' ' + " & vbCrLf &
                         " ISNULL(files.Remarks, '') + ' ' + " & vbCrLf &
                         " ISNULL(CONVERT(varchar, InvoiceAmount), '') + ' ' + " & vbCrLf &
                         " ISNULL(org.CompanyName, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next

                mainquery += whereSearchContent & ") "
            End If

            pSql = "SELECT * FROM (" & vbCrLf &
                    mainquery & vbCrLf &
                    " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                    " ORDER BY VendorName ASC"

            Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
            'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function
End Class
